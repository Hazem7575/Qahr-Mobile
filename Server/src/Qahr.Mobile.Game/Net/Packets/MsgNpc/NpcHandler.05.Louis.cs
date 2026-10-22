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
        [NpcAttribute(NpcID.Louis)]
        public static void Louis(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1828);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Louis, client.Player.Class, 1829);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Louis, client.Player.Class, 1830);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Shark, client.Player.Class, 1832);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Louis, client.Player.Class, 1833);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Louis, client.Player.Class, 1834);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Mystic Castle! What a fascinating scenery! Millions of people travelled here to seek exotic treasures.");
                            data.AddOption("Wow,~I~love~treasures!", 2);
                            data.AddOption("I~see.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Mystic Castle! What a fascinating scenery! Millions of people travelled here to seek exotic treasures.");
                            data.AddOption("I~love~all~treasures!", 30);
                            data.AddOption("I~see.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                            && !client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {

                            data.AddText("Mystic Castle! What a fascinating scenery! Millions of people travelled here to seek exotic treasures.");
                            data.AddOption("That`s~why~I`m~here!", 8);
                            data.AddOption("I`ll~think~about~it.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();

                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                      && !client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("Mystic Castle! What a fascinating scenery! Millions of people travelled here to seek exotic treasures.");
                                data.AddOption("I~love~treasures~more.", 15);
                                data.AddOption("I~see.");
                                data.AddAvatar(52);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Mystic Castle! What a fascinating scenery! Millions of people travelled here to seek exotic treasures.");
                                data.AddOption("Tell~me~about~the~second~one.", 14);
                                data.AddOption("I~see.");
                                data.AddAvatar(52);
                                data.FinalizeDialog();
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
            && !client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {

                            data.AddText("Mystic Castle! What a fascinating scenery! Millions of people travelled here to seek exotic treasures.");
                            data.AddOption("Tell~me~about~the~last~one!", 36);
                            data.AddOption("I~see.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();

                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
      && !client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (client.Player.QuestGUI.CheckObjectives(ActiveQuest6.MissionId, 1))
                                {
                                    data.AddText("Congratulations, you have answered all of my questions.");
                                    data.AddOption("Hurray!");
                                    data.AddAvatar(52);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("Thank you for all this. If you still have time, I have some riddles for you. Are you interested?");
                                    data.AddOption("Tell~me~more.", 40);
                                    data.AddOption("Sorry,~but~I`m~busy.");
                                    data.AddAvatar(52);
                                    data.FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Thanks! You`ve brought me all the three treasures. Now you can find Taskmaster Chang to claim your rewards.");
                                data.AddOption("I~see.", 60);
                                data.AddOption("I`ll~think~about~it.");
                                data.AddAvatar(52);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Mystic Castle! What a fascinating scenery! Millions of people travelled here to seek exotic treasures.");
                            data.AddOption("I~see.", 1);
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }

                        break;
                    }
                case 60:
                    {
                        client.CreateBoxDialog("Find~Taskmaster~Chang~to~claim~the~rewards.");
                        client.Player.QuestGUI.Accept(ActiveQuest6, 0);
                        data.AddText("Incredible! You are just so brave and kind. Now you can find Taskmaster Chang to claim your rewards.");
                        data.AddOption("OK.", 40);
                        data.AddAvatar(52);
                        data.FinalizeDialog();
                        break;
                    }
                case 50:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest6.MissionId, 1))
                        {
                            data.AddText("Congratulations, you have answered all of my questions.");
                            data.AddOption("Hurray!");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Mystic Castle! What a fascinating scenery! Millions of people travelled here to seek exotic treasures.");
                            data.AddOption("I~see.", 41);
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 43:
                    {
                        data.AddText("Hey, you can do better than that, right?");
                        data.AddOption("Try~again.", 41);
                        data.AddAvatar(52);
                        data.FinalizeDialog();
                        break;
                    }
                case 42:
                    {
                        client.Player.LuiseQuestions += 1;
                        data.AddText("Very good, very good. Want more?");
                        data.AddOption("Yes.", 41);
                        data.AddAvatar(52);
                        data.FinalizeDialog();
                        break;
                    }
                case 44:
                    {
                        client.Player.LuiseQuestions += 1;
                        data.AddText("Brilliant.");
                        data.AddOption("Yes.", 41);
                        data.AddAvatar(52);
                        data.FinalizeDialog();
                        break;
                    }
                case 45:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest6.MissionId, 1) == false)
                        {
                            client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest6.MissionId, 1);
                            client.Player.LuiseQuestions += 1;
                            client.GainExpBall(300);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "recovery");
                            client.SendSysMesage("You received 30 minutes EXP!");
                            data.AddText("Very good. Your intelligence matches your valor!");
                            data.AddOption("Cannot~agree~more.", 50);
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 41:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest6.MissionId, 1))
                        {
                            data.AddText("Congratulations, you have answered all of my questions.");
                            data.AddOption("Hurray!");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                            break;
                        }
                        if (client.Player.LuiseQuestions == 0)
                        {
                            data.AddText("What comes down but never goes up?");
                            data.AddOption("Rain.", 42);
                            data.AddOption("Sun.", 43);
                            data.AddOption("Moon.", 43);
                            data.AddOption("Earth.", 43);
                            data.AddOption("Quit.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        else if (client.Player.LuiseQuestions == 1)
                        {
                            data.AddText("What is black when it is clean and white when it is dirty?");
                            data.AddOption("Desk.", 43);
                            data.AddOption("Blackboard.", 44);
                            data.AddOption("Chair.", 43);
                            data.AddOption("Chalk.", 43);
                            data.AddOption("Quit.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        else if (client.Player.LuiseQuestions == 2)
                        {
                            data.AddText("What has ears, but can`t hear?");
                            data.AddOption("Barley.", 43);
                            data.AddOption("Rice.", 43);
                            data.AddOption("Corn.", 45);
                            data.AddOption("Oat.", 43);
                            data.AddOption("Quit.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        else if (client.Player.LuiseQuestions >= 3)
                        {
                            data.AddText("Congratulations, you have answered all of my questions.");
                            data.AddOption("Hurray!");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 40://
                    {
                        data.AddText("Ok, now listen carefully.");
                        data.AddOption("Can`t~wait.", 41);
                        data.AddOption("Sorry,~but~I`m~busy.");
                        data.AddAvatar(52);
                        data.FinalizeDialog();
                        break;
                    }
                case 37:
                    {
                        if (client.Inventory.Contain(721872, 1))
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest5.MissionId))
                            {
                                client.Inventory.Remove(721872, 1, stream);
                                client.GainExpBall(300);
                                client.Player.Money += 6000;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "recovery");
                                client.SendSysMesage("You received 1 hour EXP and 6,000 Silver! Find Taskmaster Chang to claim your rewards.");
                                client.CreateBoxDialog("You~gave~the~Buddha~Relic~to~Louis.");
                                client.Player.QuestGUI.Accept(ActiveQuest6, 0);
                                data.AddText("Incredible! You are just so brave and kind. Now you can find Taskmaster Chang to claim your rewards.");
                                data.AddOption("OK.", 40);
                                data.AddAvatar(52);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("My hero, don`t forget to take this Pure Bottle with you. Kill the Jinx Tomb Bat (728,676) first, then use the Bottle before");
                            data.AddText("~its body disappears to get the Buddha Relic out of his body. Here you are, take this Pure Bottle.");
                            data.AddOption("Claim~the~Pure~Bottle.", 38);
                            data.AddOption("I`ll~think~about~it.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 38:
                    {
                        if (client.Inventory.Contain(721874, 1))
                        {
                            data.AddText("Take good care of this Pure Bottle. It will come in handy when you get there.");
                            data.AddOption("I~see.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Never mind. I have a backup one. But don`t lose it again, will you?");
                            data.AddOption("Sorry.", 39);
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 39:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Add(stream, 721874);
                            client.CreateBoxDialog("You~received~a~Pure~Bottle~from~Louis!");
                        }
                        else
                        {
                            data.AddText("Your inventory is full. You`ll need to make some room, first.");
                            data.AddOption("OK.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 36:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Did you get the Buddha Relic?");
                            data.AddOption("Yes.", 37);
                            data.AddOption("I`m~on~my~way.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("The last one is the Buddha Relic. It was stolen by a Jinx Tomb Bat. But this time I won`t sell it.");
                            data.AddText("~I will return it to those faithful monks. Join me, you won`t regret it!");
                            data.AddOption("Good.~I`m~in.", 32);
                            data.AddOption("Not~this~time.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 34:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Add(stream, 721874);
                            client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                            client.CreateBoxDialog("You~received~a~Pure~Bottle~from~Louis!");

                        }
                        else
                        {
                            data.AddText("Your inventory is full. You`ll need to make some room, first.");
                            data.AddOption("OK.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 33:
                    {
                        data.AddText("But that is not all. Take this Pure Bottle with you. Kill the Jinx Tomb Bat (728,676) first, then use the Bottle before");
                        data.AddText("~its body disappears to get the Buddha Relic out of its body. Hear you are, take this Pure Bottle.");
                        data.AddOption("Claim~the~Pure~Bottle.", 34);
                        data.AddOption("I`ll~think~about~it.");
                        data.AddAvatar(52);
                        data.FinalizeDialog();
                        break;
                    }
                case 32:
                    {
                        data.AddText("Good! I knew you would do this! Jinx Tomb Bat is now in Mystic Castle (728,676). The Buddha Relic is right inside its belly.");
                        data.AddOption("I~see.", 33);
                        data.AddAvatar(52);
                        data.FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId) && client.Player.QuestGUI.CheckObjectives(ActiveQuest4.MissionId, 5, 1))
                        {
                            client.Inventory.Remove(721871, 1, stream);
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "recovery");
                            client.SendSysMesage("You received 100 Silver!");
                            client.CreateBoxDialog("You~gave~the~Victory~Portrait~to~Louis.");
                            data.AddText("The last one is the Buddha Relic. It was stolen by a Jinx Tomb Bat. But this time I won`t sell it.");
                            data.AddText("~I will return it to those faithful monks. Join me, you won`t regret it!");
                            data.AddOption("Good.~I`m~in.", 32);
                            data.AddOption("Not~this~time.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 30:
                    {
                        if (client.Inventory.Contain(721871, 1))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest4.MissionId, 5, 1))
                            {
                                if (DateTime.Now > client.Player.RealPortraitStamp.AddSeconds(60))
                                {
                                    client.Player.QuestGUI.SetQuestObjectives(stream, ActiveQuest4.MissionId, 0, 0);
                                    data.AddText("Hey man! Nothing here? What are you up to?");
                                    data.AddOption("Easy~there!");
                                    data.AddAvatar(52);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("Wonderful! This is the Victory Portrait. Maybe I can draw one like this and sell at a high price too.");
                                    data.AddOption("Are~you~kidding?", 31);
                                    data.AddAvatar(52);
                                    data.FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Hey man! Nothing here? What are you up to?");
                                data.AddOption("Easy~there!");
                                data.AddAvatar(52);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            if (client.Inventory.Contain(721878, 1))
                            {
                                data.AddText("Hey man! Nothing here? What are you up to?");
                                data.AddOption("Easy~there!");
                                data.AddAvatar(52);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Go find Shark in Mystic Castle (310,631) and see what he knows about the Victory Portrait.");
                                data.AddOption("Got~it.");
                                data.AddAvatar(52);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 15:
                    {
                        data.AddText("Go find Shark in Mystic Castle (310,631) and see what he knows about the Victory Portrait.");
                        data.AddOption("Got~it.");
                        data.AddAvatar(52);
                        data.FinalizeDialog();
                        break;
                    }
                case 14:
                    {
                        data.AddText("Well, the second treasure is Victory Portrait. It is worth more than the Zen Scripture. Wanna join me?");
                        data.AddOption("Let`s~go.", 13);
                        data.AddOption("Sorry,~but~I`m~busy.");
                        data.AddAvatar(52);
                        data.FinalizeDialog();
                        break;
                    }
                case 8:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("The Zen Scripture is kept in the Scripture Vault in Mystic Castle (91,283). You`ll need to use the Reveal Vial to reveal the compartment.");
                            data.AddText("~Then use the key you will get from the Bats to open it.");
                            data.AddOption("I~see.");
                            data.AddOption("I~lost~the~Reveal~Vial.", 9);
                            data.AddOption("Hand~in~the~scripture.", 11);//1273620
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Here it is! Mystic Castle! They said you could find all the treasures here! This time I came for three of them in particular.");
                            data.AddText("~My hero, will you help me? I will reward you handsomely if you find them all!");
                            data.AddOption("Sounds~interesting.", 4);
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 11:
                    {
                        if (client.Inventory.Contain(721870, 1))
                        {
                            data.AddText("Thank you, my hero! Now I just need to wait for those greedy buyers to come and set a price for this.");
                            data.AddOption("There~you~are.", 12);
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Are you kidding me? You don`t have the scripture. Keep working!");
                            data.AddOption("Just~a~joke.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 13:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        data.AddText("The desert around sucks, but how can I leave without the Victory Portrait! My hero, go find Shark there (310,631).");
                        data.AddText("~He should be able to help.");
                        data.AddOption("Got~it.");
                        data.AddAvatar(52);
                        data.FinalizeDialog();
                        break;
                    }
                case 12:
                    {
                        if (client.Inventory.Contain(721870, 1))
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                            {
                                client.Inventory.Remove(721870, 1, stream);
                                client.Inventory.Remove(721873, 1, stream);
                                client.Inventory.Remove(721876, 1, stream);

                                client.Player.Money += 100;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "recovery");
                                client.SendSysMesage("You received 100 Silver!");

                                client.CreateBoxDialog("You~gave~the~Zen~Scripture~to~Louis.");
                                data.AddText("Well, the second treasure is Victory Portrait. It is worth more than the Zen Scripture. Wanna join me?");
                                data.AddOption("Let`s~go.", 13);
                                data.AddOption("Sorry,~but~I`m~busy.");
                                data.AddAvatar(52);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Are you kidding me? You don`t have the scripture. Keep working!");
                            data.AddOption("Just~a~joke.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 9:
                    {
                        if (client.Inventory.Contain(721873, 1))
                        {
                            data.AddText("Sorry, but you`ve had the Reveal Vial in your inventory.");
                            data.AddOption("I~forgot.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Neat! Zen Scripture, kept in a Scripture Box, is in a compartment in Scripture Vault in Mystic Castle (91,283). Take this");
                            data.AddText("~Reveal Vial and use it there to reveal the compartment. Then use the key you will get from the Bats to open it. Clear?");
                            data.AddOption("Claim~the~Reveal~Vial.", 10);
                            data.AddOption("I`ll~think~about~it.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 10:
                    {
                        client.Inventory.Add(stream, 721873, 1);
                        client.SendSysMesage("You received a Reveal Vial from Louis!");
                        client.CreateBoxDialog("Now~go~to~Scripture~Vault~in~Mystic~Castle~(91,283).");
                        break;
                    }
                case 2:
                    {
                        data.AddText("It`s hard to make money nowadays. I have been noticing this area for a long time, and I`m sure that there are plenty of treasures here.");
                        data.AddOption("Wow.", 3);
                        data.AddAvatar(52);
                        data.FinalizeDialog();
                        break;
                    }
                case 7:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Add(stream, 721873, 1);
                            client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                            client.SendSysMesage("You received a Reveal Vial from Louis!");
                            client.CreateBoxDialog("Now~go~to~Scripture~Vault~in~Mystic~Castle~(91,283).");
                        }
                        else
                        {
                            data.AddText("Your inventory is full. You`ll need to make some room, first.");
                            data.AddOption("OK.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 6:
                    {
                        data.AddText("Neat! Zen Scripture, kept in a Scripture Box, is in a compartment in Scripture Vault in Mystic Castle (91,283). Take this");
                        data.AddText("~Reveal Vial and use it there to reveal the compartment. Then use the key you will get from the Bats to open it. Clear?");
                        data.AddOption("Claim~the~Reveal~Vial.", 7);
                        data.AddOption("I`ll~think~about~it.");
                        data.AddAvatar(52);
                        data.FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        data.AddText("I heard through the grapevine that it is now kept in the Scripture Vault in Mystic Castle. But those Tomb Bats are guarding it.");
                        data.AddText("~I can`t get any closer to it...Will you help me with this?");
                        data.AddOption("I`m~in.", 6);
                        data.AddOption("Sorry,~but~I`m~busy.");
                        data.AddAvatar(52);
                        data.FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        data.AddText("Well, the first one is Zen Scripture. It was taken from Japan by some unknown eminent monk...I don`t get it...But it could sell");
                        data.AddText("~a high price in the open market.");
                        data.AddOption("Where~is~it~now?", 5);
                        data.AddAvatar(52);
                        data.FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                            {
                                client.Player.Money += 100;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "recovery");
                                client.SendSysMesage("You received 100 Silver!");
                                data.AddText("Here it is! Mystic Castle! They said you could find all the treasures here! This time I came for three of them in particular.");
                                data.AddText("~My hero, will you help me? I will reward you handsomely if you find them all!");
                                data.AddOption("Sounds~interesting.", 4);
                                data.AddAvatar(52);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Your inventory is full. You`ll need to make some room, first.");
                            data.AddOption("OK.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        data.AddText("Well, some of the Scriptures are even more valuable than boxes of gold...");
                        data.AddOption("Gorgeous.");
                        data.AddAvatar(52);
                        data.FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.MrFree)]
        public static void MrFree(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Inventory.Contain(723087, 1))
                        {
                            client.Player.OnRemoveLukyAmulet = true;
                            data.AddText("I~love~living~a~seclusive~life.~Don`t~bother~me!");
                            data.AddOption("I~heard~that~you~need~LuckyAmulets.", 1);
                            data.AddAvatar(74);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("I~love~living~a~seclusive~life.~Don`t~bother~me!");
                            data.AddOption("I`ll~leave~now.");
                            data.AddAvatar(74);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        data.AddText("Yes.~My~only~hobby~is~to~collect~LuckyAmulets.");
                        data.AddOption("I~happen~to~have~one.", 2);
                        data.AddOption("I`d~better~keep~it.");
                        data.AddAvatar(74);
                        data.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("Great!~If~you~present~LuckyAmulet~to~me,~I`ll~make~a~unique~hairstyle~for~you.");
                        data.AddOption("Thank~you.", 3);
                        data.AddOption("I`d~better~keep~it.");
                        data.AddAvatar(74);
                        data.FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        if (client.Player.IsBoy())
                        {
                            data.AddText("Thank~you~very~much.~Which~hairstyle~would~you~like~to~select~from?");
                            data.AddOption("Dignified~one.", 4);
                            data.AddOption("Unruly~one.", 5);
                            data.AddOption("Staid~one.", 6);
                            data.AddOption("Cool~one.", 7);
                            data.AddOption("Handsome~one.", 8);
                            data.AddAvatar(74);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Thank~you~very~much.~Which~hairstyle~would~you~like~to~select~from?");
                            data.AddOption("Lovely~one.", 4);
                            data.AddOption("Comely~one.", 5);
                            data.AddOption("Pretty~one.", 6);
                            data.AddOption("Elegant~one.", 7);
                            data.AddOption("Nifty~one.", 8);
                            data.AddAvatar(74);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Player.OnRemoveLukyAmulet == true)
                        {
                            client.Inventory.Remove(723087, 1, stream);
                            client.Player.OnRemoveLukyAmulet = false;
                        }
                        client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 21);
                        client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                        data.AddText("Are~you~satisfied~with~your~new~hairsyle?");
                        data.AddOption("Great!~Thank~you.");
                        data.AddOption("I`d~like~to~try~another~one.", 3);
                        data.AddAvatar(74);
                        data.FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        if (client.Player.OnRemoveLukyAmulet == true)
                        {
                            client.Inventory.Remove(723087, 1, stream);
                            client.Player.OnRemoveLukyAmulet = false;
                        }
                        client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 22);
                        client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                        data.AddText("Are~you~satisfied~with~your~new~hairsyle?");
                        data.AddOption("Great!~Thank~you.");
                        data.AddOption("I`d~like~to~try~another~one.", 3);
                        data.AddAvatar(74);
                        data.FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        if (client.Player.OnRemoveLukyAmulet == true)
                        {
                            client.Inventory.Remove(723087, 1, stream);
                            client.Player.OnRemoveLukyAmulet = false;
                        }
                        client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 23);
                        client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                        data.AddText("Are~you~satisfied~with~your~new~hairsyle?");
                        data.AddOption("Great!~Thank~you.");
                        data.AddOption("I`d~like~to~try~another~one.", 3);
                        data.AddAvatar(74);
                        data.FinalizeDialog();
                        break;
                    }
                case 7:
                    {
                        if (client.Player.OnRemoveLukyAmulet == true)
                        {
                            client.Inventory.Remove(723087, 1, stream);
                            client.Player.OnRemoveLukyAmulet = false;
                        }
                        client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 24);
                        client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                        data.AddText("Are~you~satisfied~with~your~new~hairsyle?");
                        data.AddOption("Great!~Thank~you.");
                        data.AddOption("I`d~like~to~try~another~one.", 3);
                        data.AddAvatar(74);
                        data.FinalizeDialog();
                        break;
                    }
                case 8:
                    {
                        if (client.Player.OnRemoveLukyAmulet == true)
                        {
                            client.Inventory.Remove(723087, 1, stream);
                            client.Player.OnRemoveLukyAmulet = false;
                        }
                        client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 25);
                        client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                        data.AddText("Are~you~satisfied~with~your~new~hairsyle?");
                        data.AddOption("Great!~Thank~you.");
                        data.AddOption("I`d~like~to~try~another~one.", 3);
                        data.AddAvatar(74);
                        data.FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Performer)]
        public static void Performer(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello~there!~Please~allow~me~to~introduce~the~sub-class~of~Performers~to~you.~Performers~are~gifted~in~the~harmonizing~of~song~and~swordplay~into~beautiful,~unique~dances.~These~dances~will~surely~make~you~the~center~of~people`s~attention!");
                        data.AddText("~Would~you~like~to~join~us?~After~you~join~this~sub-class,~you~can~level~it~up~and~get~promoted~to~learn~more~stylish~dances.");
                        data.AddText("Performers~are~gifted~in~the~harmonizing~of~song~and~swordplay~into~beautiful,~unique~dances.~Besides~that,~their~physical~and~magical~attacks~improve~when~their~Sub-Class~level~improves.");
                        data.AddOption("Yes,~I`d~like~to~join!", 20);
                        data.AddOption("I~want~to~get~promoted.", 40);
                        data.AddOption("I~want~to~learn~new~dances.", 3);
                        data.AddOption("How~can~I~get~promoted?", 1);
                        data.AddOption("Nice.~I`ll~think~about~it.");
                        data.AddAvatar(110);
                        data.FinalizeDialog();
                        break;
                    }
                case 40:
                    {
                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Performer) < 1)
                        {
                            data.AddText("Well,~you~need~to~join~us~before~you~can~be~promoted.~Would~you~like~to~become~a~Performer~now?");
                            data.AddOption("Sure.", 20);
                            data.AddOption("Oh,~I`m~not~ready~yet.");
                            data.AddAvatar(110);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Performer) == 1)
                            {
                                data.AddText("To~get~promoted~to~Phase~2~and~learn~more~dances,~you~should~have~a~character~level~of~90~and~become~a~level~2~Performer.~Have~you~met~all~these~requirements?");
                                data.AddOption("Positive.", 41);
                                data.AddOption("Oh,~not~yet.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Performer) == 2)
                                {
                                    data.AddText("To~get~promoted~to~Phase~3~and~learn~more~dances,~you~should~have~a~character~level~of~110~and~become~a~level~3~Performer.~Have~you~met~all~these~requirements?");
                                    data.AddOption("Positive.", 42);
                                    data.AddOption("Oh,~not~yet.");
                                    data.AddAvatar(110);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Performer) == 3)
                                    {
                                        data.AddText("To~get~promoted~to~Phase~4~and~learn~more~dances,~you~should~have~a~character~level~of~120~and~become~a~level~4~Performer.~Have~you~met~all~these~requirements?");
                                        data.AddOption("Positive.", 43);
                                        data.AddOption("Oh,~not~yet.");
                                        data.AddAvatar(110);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Performer) == 4)
                                        {
                                            data.AddText("To~get~promoted~to~Phase~5~and~learn~more~dances,~you~should~have~a~character~level~of~90,~become~a~level~5~Performer,~and~have~been~reborn~once.~Have~you~met~all~these~requirements?");
                                            data.AddOption("Positive.", 44);
                                            data.AddOption("Oh,~not~yet.");
                                            data.AddAvatar(110);
                                            data.FinalizeDialog();
                                        }
                                        else
                                        {
                                            if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Performer) == 5)
                                            {
                                                data.AddText("To~get~promoted~to~Phase~6~and~learn~more~dances,~you~should~have~a~character~level~of~110,~become~a~level~6~Performer,~and~have~been~reborn~once.~Have~you~met~all~these~requirements?");
                                                data.AddOption("Positive.", 45);
                                                data.AddOption("Oh,~not~yet.");
                                                data.AddAvatar(110);
                                                data.FinalizeDialog();
                                            }
                                            else
                                            {
                                                if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Performer) == 6)
                                                {
                                                    data.AddText("To~get~promoted~to~Phase~7~and~learn~more~dances,~you~should~have~a~character~level~of~120,~become~a~level~7~Performer,~and~have~been~reborn~once.~Have~you~met~all~these~requirements?");
                                                    data.AddOption("Positive.", 46);
                                                    data.AddOption("Oh,~not~yet.");
                                                    data.AddAvatar(110);
                                                    data.FinalizeDialog();
                                                }
                                                else
                                                {
                                                    if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Performer) == 7)
                                                    {
                                                        data.AddText("To~get~promoted~to~Phase~8~and~learn~more~dances,~you~should~have~a~character~level~of~120,~become~a~level~8~Performer,~and~have~been~reborn~twice.~Have~you~met~all~these~requirements?");
                                                        data.AddOption("Positive.", 47);
                                                        data.AddOption("Oh,~not~yet.");
                                                        data.AddAvatar(110);
                                                        data.FinalizeDialog();
                                                    }
                                                    else
                                                    {
                                                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Performer) == 8)
                                                        {
                                                            data.AddText("To~get~promoted~to~Phase~9~and~learn~more~dances,~you~should~have~a~character~level~of~130,~become~a~level~9~Performer,~and~have~been~reborn~twice.~Have~you~met~all~these~requirements?");
                                                            data.AddOption("Positive.", 48);
                                                            data.AddOption("Oh,~not~yet.");
                                                            data.AddAvatar(110);
                                                            data.FinalizeDialog();
                                                        }
                                                        else
                                                        {
                                                            data.AddText("There~is~nothing~more~I~can~teach~you,~now~that~you`ve~reached~the~highest~phase~of~this~sub-class.~Well~done!");
                                                            data.AddOption("Oh.~Thanks~anyway.");
                                                            data.AddAvatar(110);
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
                        break;
                    }
                case 48:
                    {
                        if (client.Player.Level < 130)
                        {
                            data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                            data.AddOption("Oh.");
                            data.AddAvatar(110);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Performer) < 9)
                            {
                                data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                data.AddOption("Oh.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 2)
                                {
                                    data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(110);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Performer, 9, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~been~promoted~to~a~Phase~9~Performer!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~have~been~promoted~to~a~Phase~9~Performer!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(110);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 47:
                    {
                        if (client.Player.Level < 120)
                        {
                            data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                            data.AddOption("Oh.");
                            data.AddAvatar(110);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Performer) < 8)
                            {
                                data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                data.AddOption("Oh.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 2)
                                {
                                    data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(110);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Performer, 8, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~been~promoted~to~a~Phase~8~Performer!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~have~been~promoted~to~a~Phase~8~Performer!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(110);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 46:
                    {
                        if (client.Player.Reborn < 2)
                        {
                            if (client.Player.Level < 120)
                            {
                                data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                data.AddOption("Oh.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Performer) < 7)
                                {
                                    data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(110);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Reborn < 1)
                                    {
                                        data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                        data.AddOption("Oh.");
                                        data.AddAvatar(110);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Performer, 7, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~been~promoted~to~a~Phase~7~Performer!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("You~have~been~promoted~to~a~Phase~7~Performer!");
                                        data.AddOption("Great.");
                                        data.AddAvatar(110);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Performer) < 7)
                            {
                                data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                data.AddOption("Oh.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 1)
                                {
                                    data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(110);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Performer, 7, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~been~promoted~to~a~Phase~7~Performer!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~have~been~promoted~to~a~Phase~7~Performer!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(110);
                                    data.FinalizeDialog();
                                }
                            }
                        }

                        break;
                    }
                case 45:
                    {
                        if (client.Player.Reborn < 2)
                        {
                            if (client.Player.Level < 110)
                            {
                                data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                data.AddOption("Oh.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Performer) < 6)
                                {
                                    data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(110);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Reborn < 1)
                                    {
                                        data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                        data.AddOption("Oh.");
                                        data.AddAvatar(110);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Performer, 6, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~been~promoted~to~a~Phase~6~Performer!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("You~have~been~promoted~to~a~Phase~6~Performer!");
                                        data.AddOption("Great.");
                                        data.AddAvatar(110);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Performer) < 6)
                            {
                                data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                data.AddOption("Oh.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 1)
                                {
                                    data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(110);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Performer, 6, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~been~promoted~to~a~Phase~6~Performer!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~have~been~promoted~to~a~Phase~6~Performer!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(110);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 44:
                    {
                        if (client.Player.Reborn < 2)
                        {
                            if (client.Player.Level < 90)
                            {
                                data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                data.AddOption("Oh.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Performer) < 5)
                                {
                                    data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(110);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Reborn < 1)
                                    {
                                        data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                        data.AddOption("Oh.");
                                        data.AddAvatar(110);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Performer, 5, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~been~promoted~to~a~Phase~5~Performer!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("You~have~been~promoted~to~a~Phase~5~Performer!");
                                        data.AddOption("Great.");
                                        data.AddAvatar(110);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Performer) < 5)
                            {
                                data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                data.AddOption("Oh.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 1)
                                {
                                    data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(110);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Performer, 5, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~been~promoted~to~a~Phase~5~Performer!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~have~been~promoted~to~a~Phase~5~Performer!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(110);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 43:
                    {
                        if (client.Player.Reborn < 1)
                        {
                            if (client.Player.Level < 120)
                            {
                                data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                data.AddOption("Oh.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Performer) < 4)
                                {
                                    data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(110);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Performer, 4, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~been~promoted~to~a~Phase~4~Performer!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~have~been~promoted~to~a~Phase~4~Performer!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(110);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Performer) < 4)
                            {
                                data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                data.AddOption("Oh.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Performer, 4, client, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~been~promoted~to~a~Phase~4~Performer!", MsgMessage.ChatMode.TopLeftSystem);
                                data.AddText("You~have~been~promoted~to~a~Phase~4~Performer!");
                                data.AddOption("Great.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 42:
                    {
                        if (client.Player.Reborn < 1)
                        {
                            if (client.Player.Level < 110)
                            {
                                data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                data.AddOption("Oh.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Performer) < 3)
                                {
                                    data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(110);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Performer, 3, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~been~promoted~to~a~Phase~3~Performer!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~have~been~promoted~to~a~Phase~3~Performer!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(110);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Performer) < 3)
                            {
                                data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                data.AddOption("Oh.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Performer, 3, client, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~been~promoted~to~a~Phase~3~Performer!", MsgMessage.ChatMode.TopLeftSystem);
                                data.AddText("You~have~been~promoted~to~a~Phase~3~Performer!");
                                data.AddOption("Great.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 41:
                    {
                        if (client.Player.Reborn < 1)
                        {
                            if (client.Player.Level < 90)
                            {
                                data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                data.AddOption("Oh.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Performer) < 2)
                                {
                                    data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(110);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Performer, 2, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~been~promoted~to~a~Phase~2~Performer!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~have~been~promoted~to~a~Phase~2~Performer!");
                                    data.AddOption("Great!");
                                    data.AddAvatar(110);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Performer) < 2)
                            {
                                data.AddText("My~friend,~you~haven`t~met~the~requirements~to~get~promoted,~yet.~You~can~try~again~when~you`ve~gotten~better!");
                                data.AddOption("Oh.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Performer, 2, client, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~been~promoted~to~a~Phase~2~Performer!", MsgMessage.ChatMode.TopLeftSystem);
                                data.AddText("You~have~been~promoted~to~a~Phase~2~Performer!");
                                data.AddOption("Great!");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 20:
                    {

                        data.AddText("Performers~always~attract~public~attention~by~unique~Dances.~If~you`ve~reached~Level~70~and~you~want~to~join~us,~bring~me~15~Orchids~or~800~Study~Points.");
                        data.AddText("~it~requires~a~Performer~Card~which~can~be~obtained~by~handing~in~15~Orchids.~After~joining~the~Performer~sub-class,~click~open~your~character~sheet~and~you`ll~find~the~sub-class~button~on~the~left-hand~corner~where~you~may~level~up~your~sub-class.");
                        data.AddOption("Pay~15~Orchids~to~join.", 23);
                        data.AddOption("Pay~800~Study~Points~to~join.", 21);
                        data.AddOption("Oh,~it~does~not~suit~me.");
                        data.AddAvatar(110);
                        data.FinalizeDialog();

                        break;
                    }
                case 23:
                    {
                        if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Performer) < 1)
                        {
                            if (client.Player.Reborn < 1)
                            {
                                if (client.Player.Level < 70)
                                {
                                    data.AddText("You~will~be~invited~to~join~us~after~you~reach~level~70.~Now~please~leave~me~alone,~I`ve~got~some~dancing~classes~to~prepare~for.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(110);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Inventory.Contain(711679, 1))
                                    {
                                        data.AddText("Hmmm,~I`ve~accepted~the~15~Orchids~you~just~handed~in~and~here~is~your~Performer~Card.~Well,~if~you`ve~reached~level~70,~you~will~become~a~Performer~immediately.~Are~you~sure~you~want~to~join~us?");
                                        data.AddOption("Sure.", 25);
                                        data.AddOption("I`ve~changed~my~mind.");
                                        data.AddAvatar(110);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        data.AddText("How~many~orchids~would~you~like~to~hand~in~this~time?~It~takes~15~orchids~to~get~a~Performer~Card~which~is~required~to~join~the~Performers.");
                                        data.AddOption("1~Orchid.", 26);
                                        data.AddOption("3~Orchids.", 28);
                                        data.AddOption("9~Orchids.", 29);
                                        data.AddOption("How~many~have~I~given~you?", 30);
                                        data.AddOption("Just~passing~by.");
                                        data.AddAvatar(110);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                            else
                            {
                                if (client.Inventory.Contain(711679, 1))
                                {
                                    data.AddText("Hmmm,~I`ve~accepted~the~15~Orchids~you~just~handed~in~and~here~is~your~Performer~Card.~Well,~if~you`ve~reached~level~70,~you~will~become~a~Performer~immediately.~Are~you~sure~you~want~to~join~us?");
                                    data.AddOption("Sure.", 25);
                                    data.AddOption("I`ve~changed~my~mind.");
                                    data.AddAvatar(110);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("How~many~orchids~would~you~like~to~hand~in~this~time?~It~takes~15~orchids~to~get~a~Performer~Card~which~is~required~to~join~the~Performers.");
                                    data.AddOption("1~Orchid.", 26);
                                    data.AddOption("3~Orchids.", 28);
                                    data.AddOption("9~Orchids.", 29);
                                    data.AddOption("How~many~have~I~given~you?", 30);
                                    data.AddOption("Just~passing~by.");
                                    data.AddAvatar(110);
                                    data.FinalizeDialog();
                                }

                            }
                        }
                        else
                        {
                            data.AddText("Aha,~you~are~already~a~Performer,~remember?~I~suggest~you~focus~your~attention~on~upgrading~your~sub-class~level,~now.");
                            data.AddOption("Oh.");
                            data.AddAvatar(110);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 30:
                    {
                        if (client.Player.GiveFlowersToPerformer >= 1)
                        {
                            data.AddText("You`ve~handed~in~" + client.Player.GiveFlowersToPerformer + "~Orchids.~As~you~meet~the~requirement,~you`ll~get~a~Performer~Card.");
                            data.AddOption("Oh.");
                            data.AddAvatar(110);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You~haven`t~handed~in~any~Orchids~yet.~It~charges~15~Orchids~for~a~Performer~Card.");
                            data.AddOption("Oh.");
                            data.AddAvatar(110);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 29:
                    {
                        if (client.Inventory.Contain(753009, 1))
                        {
                            client.Inventory.Remove(753009, 1, stream);
                            client.Player.GiveFlowersToPerformer += 9;
                            if (client.Player.GiveFlowersToPerformer >= 15)
                            {
                                client.Player.GiveFlowersToPerformer -= 15;
                                client.Inventory.Add(stream, 711679, 1);
                                data.AddText("Hmmm,~I`ve~accepted~the~15~Orchids~you~just~handed~in~and~here~is~your~Performer~Card.~Well,~if~you`ve~reached~level~70,~you~will~become~a~Performer~immediately.~Are~you~sure~you~want~to~join~us?");
                                data.AddOption("Sure.", 27);
                                data.AddOption("I`ve~changed~my~mind.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You`ve~handed~in~" + client.Player.GiveFlowersToPerformer + "~Orchids.~As~there~are~15~Orchids,~you~can~get~a~Performer~Card.");
                                data.AddOption("Oh.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You~don`t~have~9~Orchids~to~give.");
                            data.AddOption("Oh.");
                            data.AddAvatar(110);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 28:
                    {
                        if (client.Inventory.Contain(753003, 1))
                        {
                            client.Inventory.Remove(753003, 1, stream);
                            client.Player.GiveFlowersToPerformer += 3;
                            if (client.Player.GiveFlowersToPerformer >= 15)
                            {
                                client.Player.GiveFlowersToPerformer -= 15;
                                client.Inventory.Add(stream, 711679, 1);

                                data.AddText("Hmmm,~I`ve~accepted~the~15~Orchids~you~just~handed~in~and~here~is~your~Performer~Card.~Well,~if~you`ve~reached~level~70,~you~will~become~a~Performer~immediately.~Are~you~sure~you~want~to~join~us?");
                                data.AddOption("Sure.", 27);
                                data.AddOption("I`ve~changed~my~mind.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {

                                data.AddText("You`ve~handed~in~" + client.Player.GiveFlowersToPerformer + "~Orchids.~As~there~are~15~Orchids,~you~can~get~a~Performer~Card.");
                                data.AddOption("Oh.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("~You~don`t~have~3~Orchids~to~give.");
                            data.AddOption("Oh.");
                            data.AddAvatar(110);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 26:
                    {
                        if (client.Inventory.Contain(753001, 1))
                        {
                            client.Inventory.Remove(753001, 1, stream);
                            client.Player.GiveFlowersToPerformer += 1;
                            if (client.Player.GiveFlowersToPerformer >= 15)
                            {
                                client.Player.GiveFlowersToPerformer -= 15;
                                client.Inventory.Add(stream, 711679, 1);
                                data.AddText("Hmmm,~I`ve~accepted~the~15~Orchids~you~just~handed~in~and~here~is~your~Performer~Card.~Well,~if~you`ve~reached~level~70,~you~will~become~a~Performer~immediately.~Are~you~sure~you~want~to~join~us?");
                                data.AddOption("Sure.", 27);
                                data.AddOption("I`ve~changed~my~mind.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You`ve~handed~in~" + client.Player.GiveFlowersToPerformer + "~Orchids.~As~there~are~15~Orchids,~you~can~get~a~Performer~Card.");
                                data.AddOption("Oh.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }

                        }
                        else
                        {
                            data.AddText("You~don`t~have~an~Orchid~to~give~me.");
                            data.AddOption("Oh.");
                            data.AddAvatar(110);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 27:
                    {
                        if (client.Inventory.Contain(711679, 1))
                        {
                            client.Inventory.Remove(711679, 1, stream);
                            client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Performer, client, stream);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                            Role.Core.SendGlobalMessage(stream, "Excellent!~" + client.Player.Name + "~is~now~a~beautiful~Performer!", MsgMessage.ChatMode.TopLeftSystem);
                            data.AddText("Welcome~to~the~dancing~community!~This~is~a~big~community~and~we~have~9~phases~for~the~advancement~of~performers.~Work~hard~in~the~future,~and~when~you`re~ready~to~get~promoted,~I`m~always~here~for~you.");
                            data.AddOption("Thank~you.");
                            data.AddAvatar(110);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 25:
                    {
                        if (client.Inventory.Contain(711679, 1))
                        {
                            client.Inventory.Remove(711679, 1, stream);
                            client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Performer, client, stream);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                            Role.Core.SendGlobalMessage(stream, "Excellent!~" + client.Player.Name + "~is~now~a~beautiful~Performer!", MsgMessage.ChatMode.TopLeftSystem);
                            data.AddText("Welcome~to~the~dancing~community!~This~is~a~big~community~and~we~have~9~phases~for~the~advancement~of~performers.~Work~hard~in~the~future,~and~when~you`re~ready~to~get~promoted,~I`m~always~here~for~you.");
                            data.AddOption("Thank~you.");
                            data.AddAvatar(110);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("How~many~orchids~would~you~like~to~hand~in~this~time?~It~takes~15~orchids~to~get~a~Performer~Card~which~is~required~to~join~the~Performers.");
                            data.AddOption("1~Orchid.", 26);
                            data.AddOption("3~Orchids.", 28);
                            data.AddOption("9~Orchids.", 29);
                            data.AddOption("How~many~have~I~given~you?", 30);
                            data.AddOption("Just~passing~by.");
                            data.AddAvatar(110);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 21:
                    {
                        data.AddText("~~~~Are~you~sure~you~want~to~pay~800~Study~Points~to~join~us?");
                        data.AddOption("Yes,~I`ve~decided!", 22);
                        data.AddOption("No,~I~changed~my~mind.");
                        data.AddAvatar(110);
                        data.FinalizeDialog();
                        break;
                    }
                case 22:
                    {
                        if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Performer) < 1)
                        {
                            if (client.Player.Reborn < 1)
                            {
                                if (client.Player.Level < 70)
                                {
                                    data.AddText("You~will~be~invited~to~join~us~after~you~reach~level~70.~Now~please~leave~me~alone,~I`ve~got~some~dancing~classes~to~prepare~for.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(110);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.SubClass.StudyPoints < 800)
                                    {
                                        data.AddText("You~don`t~have~800~Study~Points~to~take~the~sub-class.");
                                        data.AddOption("Alright.");
                                        data.AddAvatar(110);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.RemoveStudy(client, 800, stream);
                                        {
                                            client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Performer, client, stream);
                                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                            Role.Core.SendGlobalMessage(stream, "Excellent!~" + client.Player.Name + "~is~now~a~beautiful~Performer!", MsgMessage.ChatMode.TopLeftSystem);
                                            data.AddText("Welcome~to~the~dancing~community!~This~is~a~big~community~and~we~have~9~phases~for~the~advancement~of~performers.~Work~hard~in~the~future,~and~when~you`re~ready~to~get~promoted,~I`m~always~here~for~you.");
                                            data.AddOption("Thank~you.");
                                            data.AddAvatar(110);
                                            data.FinalizeDialog();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (client.Player.SubClass.StudyPoints < 800)
                                {
                                    data.AddText("You~don`t~have~800~Study~Points~to~take~the~sub-class.");
                                    data.AddOption("Alright.");
                                    data.AddAvatar(110);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.RemoveStudy(client, 800, stream);
                                    {
                                        client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Performer, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "Excellent!~" + client.Player.Name + "~is~now~a~beautiful~Performer!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("Welcome~to~the~dancing~community!~This~is~a~big~community~and~we~have~9~phases~for~the~advancement~of~performers.~Work~hard~in~the~future,~and~when~you`re~ready~to~get~promoted,~I`m~always~here~for~you.");
                                        data.AddOption("Thank~you.");
                                        data.AddAvatar(110);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            data.AddText("Aha,~you~are~already~a~Performer,~remember?~I~suggest~you~focus~your~attention~on~upgrading~your~sub-class~level,~now.");
                            data.AddOption("Oh.");
                            data.AddAvatar(110);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        data.AddText("It~takes~higher~phases~to~learn~more~advanced~dances.~Learn~the~Battle~Dance~at~Phase~1,~learn~the~Triumph~at~Phase~3,~learn~the~Step~Stomp~at~Phase~5,~learn~Moonlight~at~Phase~7,~and~Snow~Wind~at~Phase~9.");
                        data.AddText("~Which~of~these~dances~do~you~want~to~learn?");
                        data.AddOption("Learn~Partner~Dances.", 9);
                        data.AddOption("The~Battle~Dance.", 8);
                        data.AddOption("Triumph.", 7);
                        data.AddOption("The~Step~Stomp.", 6);
                        data.AddOption("Moonlight.", 5);
                        data.AddOption("Snow~Wind.", 4);
                        data.AddOption("I`ll~think~about~it.");
                        data.AddAvatar(110);
                        data.FinalizeDialog();
                        break;
                    }
                case 9:
                    {
                        data.AddText("The~dance~skill~you`re~able~to~learn~depends~on~your~Performer~Phase.~The~higher~your~phase~is,~the~more~dances~you~can~learn.~So,~what~kind~of~dance~would~you~like~to~learn?");
                        data.AddOption("Learn~Phoenix.~(P3~Performer)", 12);
                        data.AddOption("Melody~(Phase~6).", 11);
                        data.AddOption("Tipsy~(Phase~9).", 10);
                        data.AddOption("Previous.", 3);
                        data.AddAvatar(110);
                        data.FinalizeDialog();
                        break;
                    }
                case 12:
                    {
                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Performer) < 3)
                        {
                            data.AddText("I`m~sorry,~but~only~Phase~3~Performers~can~learn~this~dance.~I~can`t~teach~you~this~dance~now.");
                            data.AddOption("Alright.");
                            data.AddAvatar(110);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.MySpells.ClientSpells.ContainsKey(1420))
                            {
                                data.AddText("But~you`ve~already~learned~this~dance,~remember?");
                                data.AddOption("Oh,~alright.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.MySpells.Add(stream, 1420);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                client.SendSysMesage("You`ve~learned~the~Partner~Dance,~Phoenix!");
                                data.AddText("You`ve~learned~the~Partner~Dance,~Phoenix!");
                                data.AddOption("Thanks.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 11:
                    {
                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Performer) < 6)
                        {
                            data.AddText("I`m~sorry,~but~only~Phase~6~Performers~can~learn~this~dance.~I~can`t~teach~you~this~dance~now.");
                            data.AddOption("Alright.");
                            data.AddAvatar(110);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.MySpells.ClientSpells.ContainsKey(1421))
                            {
                                data.AddText("But~you`ve~already~learned~this~dance,~remember?");
                                data.AddOption("Oh,~alright.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.MySpells.Add(stream, 1421);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                client.SendSysMesage("You`ve~learned~the~Partner~Dance,~Melody!");
                                data.AddText("You`ve~learned~the~Partner~Dance,~Melody!");
                                data.AddOption("Thanks.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 10:
                    {
                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Performer) < 9)
                        {
                            data.AddText("I`m~sorry,~but~only~Phase~9~Performers~can~learn~this~dance.~I~can`t~teach~you~this~dance~now.");
                            data.AddOption("Alright.");
                            data.AddAvatar(110);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.MySpells.ClientSpells.ContainsKey(1422))
                            {
                                data.AddText("But~you`ve~already~learned~this~dance,~remember?");
                                data.AddOption("Oh,~alright.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.MySpells.Add(stream, 1422);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                client.SendSysMesage("You`ve~learned~the~Partner~Dance,~Tipsy!");
                                data.AddText("You`ve~learned~the~Partner~Dance,~Tipsy!");
                                data.AddOption("Thanks.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 8:
                    {
                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Performer) < 1)
                        {
                            data.AddText("You~can~only~learn~it~at~Phase~1.");
                            data.AddOption("Oh.");
                            data.AddAvatar(110);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.MySpells.ClientSpells.ContainsKey(1415))
                            {
                                data.AddText("You`ve~already~learned~this~dance.");
                                data.AddOption("Oh.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.MySpells.Add(stream, 1415);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                client.SendSysMesage("You`ve~learned~the~Battle~Dance.");
                                data.AddText("You`ve~learned~the~Battle~Dance.~Hope~you`ll~work~on~it~and~learn~more~dances~in~the~future.~See~you!");
                                data.AddOption("See~you!");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                        }

                        break;
                    }
                case 7:
                    {
                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Performer) < 3)
                        {
                            data.AddText("You~can~only~learn~it~at~Phase~3.");
                            data.AddOption("Oh.");
                            data.AddAvatar(110);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.MySpells.ClientSpells.ContainsKey(1416))
                            {
                                data.AddText("You`ve~already~learned~this~dance.");
                                data.AddOption("Oh.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.MySpells.Add(stream, 1416);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                client.SendSysMesage("You`ve~learned~the~Triumph.");
                                data.AddText("You`ve~learned~the~Triumph.~Hope~you`ll~work~on~it~and~learn~more~dances~in~the~future.~See~you!");
                                data.AddOption("See~you!");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 6:
                    {
                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Performer) < 5)
                        {
                            data.AddText("You~can~only~learn~it~at~Phase~5.");
                            data.AddOption("Oh.");
                            data.AddAvatar(110);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.MySpells.ClientSpells.ContainsKey(1417))
                            {
                                data.AddText("You`ve~already~learned~this~dance.");
                                data.AddOption("Oh.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.MySpells.Add(stream, 1417);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                client.SendSysMesage("You`ve~learned~the~Step~Stomp.");
                                data.AddText("You`ve~learned~the~Step~Stomp.~Hope~you`ll~work~on~it~and~learn~more~dances~in~the~future.~See~you!");
                                data.AddOption("See~you!");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 5:
                    {
                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Performer) < 7)
                        {
                            data.AddText("You~can~only~learn~it~at~Phase~7.");
                            data.AddOption("Oh.");
                            data.AddAvatar(110);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.MySpells.ClientSpells.ContainsKey(1418))
                            {
                                data.AddText("You`ve~already~learned~this~dance.");
                                data.AddOption("Oh.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.MySpells.Add(stream, 1418);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                client.SendSysMesage("You`ve~learned~the~Moonlight.");
                                data.AddText("You`ve~learned~the~Moonlight.~Hope~you`ll~work~on~it~and~learn~more~dances~in~the~future.~See~you!");
                                data.AddOption("See~you!");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Performer) < 9)
                        {
                            data.AddText("You~can~only~learn~it~at~Phase~9.");
                            data.AddOption("Oh.");
                            data.AddAvatar(110);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.MySpells.ClientSpells.ContainsKey(1419))
                            {
                                data.AddText("You`ve~already~learned~this~dance.");
                                data.AddOption("Oh.");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.MySpells.Add(stream, 1419);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                client.SendSysMesage("You`ve~learned~the~Snow~Wind.");
                                data.AddText("You`ve~learned~the~Snow~Wind.~You~are~the~master~of~dances~now!");
                                data.AddOption("Yay!");
                                data.AddAvatar(110);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 1:
                    {
                        data.AddText("Good~day!~Please~spare~me~a~few~minutes~so~that~I~can~tell~you~about~the~sub-classes~in~Twin~City.~Each~Sub-Class~has~9~phases,~and~the~higher~the~phase~is,~the~more~bonus~effects~the~sub-class~will~offer.~But~keep~in~mind~that~these");
                        data.AddText("~sub-classes~also~have~a~level~system~and~you~need~to~level~them~up~before~you~can~get~promoted!~You~can~level~up~your~sub-class~by~gaining~study~points.");
                        data.AddOption("How~can~I~get~study~points?", 2);
                        data.AddOption("Oh.~Thanks~for~telling~me.");
                        data.AddAvatar(110);
                        data.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("Well,~the~only~way~to~get~those~points~is~to~read~the~sub-class~books,~such~as~the~Diligence~Book.~When~you`ve~gained~enough~study~points,~open~the~sub-class~sheet~from~the~character~panel~and");
                        data.AddText("~level~up~your~sub-class~there.~Monsters~in~the~wild~are~always~bothering~young~adventurers,~so~you~might~find~some~books~from~them.");
                        data.AddOption("I~see.");
                        data.AddAvatar(110);
                        data.FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ApothecarySubClass)]
        public static void ApothecarySubClass(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello~there!~Please~allow~me~to~introduce~to~you~the~sub-class~of~the~Apothecary.~An~Apothecary~is~a~master~of~various~oriental~herbs~and~can~use~their~healing~powers~to~protect~themselves~from~poison~damage.");
                        data.AddText("~Would~you~like~to~join~us?~After~you~join~this~sub-class,~you~can~level~it~up~and~get~promoted~to~gain~more~protection.");
                        data.AddOption("Yes,~I`d~like~to~join!", 3);
                        data.AddOption("I~want~to~get~promoted.", 7);
                        data.AddOption("How~can~I~get~promoted?", 1);
                        data.AddOption("Nice.~I`ll~think~about~it.");
                        data.AddAvatar(113);
                        data.FinalizeDialog();
                        break;
                    }
                case 7:
                    {
                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Apothecary) < 1)
                        {
                            data.AddText("You~are~not~an~Apothecary~yet~and~can`t~get~promoted.~Do~you~want~to~join~the~Apothecary~sub-class~now?");
                            data.AddOption("Yes,~I`d~like~to~join.", 3);
                            data.AddOption("Oh.~Not~now.");
                            data.AddAvatar(113);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Apothecary) == 1)
                            {
                                data.AddText("To~get~promoted~to~Phase~2~and~increase~more~poison~damage~reduced,~you~should~have~a~character~level~of~90~and~become~a~level~2~Apothecary.~Have~you~met~all~these~requirements?");
                                data.AddOption("Positive.", 8);
                                data.AddOption("Oh,~not~yet.");
                                data.AddAvatar(113);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Apothecary) == 2)
                                {
                                    data.AddText("To~get~promoted~to~Phase~3~and~reduce~poison~damage~further,~you~should~have~a~character~level~of~110~and~become~a~level~3~Apothecary.~Have~you~met~all~these~requirements?");
                                    data.AddOption("Positive.", 9);
                                    data.AddOption("Oh,~not~yet.");
                                    data.AddAvatar(113);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Apothecary) == 3)
                                    {
                                        data.AddText("To~get~promoted~to~Phase~4~and~reduce~poison~damage~further,~you~should~have~a~character~level~of~120~and~become~a~level~4~Apothecary.~Have~you~met~all~these~requirements?");
                                        data.AddOption("Positive.", 10);
                                        data.AddOption("Oh,~not~yet.");
                                        data.AddAvatar(113);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Apothecary) == 4)
                                        {
                                            data.AddText("To~get~promoted~to~Phase~5~and~reduce~poison~damage~further,~you~should~have~a~character~level~of~90~and~become~a~level~5~Apothecary.~Have~you~met~all~these~requirements?");
                                            data.AddOption("Positive.", 11);
                                            data.AddOption("Oh,~not~yet.");
                                            data.AddAvatar(113);
                                            data.FinalizeDialog();
                                        }
                                        else
                                        {
                                            if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Apothecary) == 5)
                                            {
                                                data.AddText("To~get~promoted~to~Phase~6~and~reduce~poison~damage~further,~you~should~have~a~character~level~of~110,~~become~a~level~6~Apothecary~and~have~been~reborn~once.~Have~you~met~all~these~requirements?");
                                                data.AddOption("Positive.", 12);
                                                data.AddOption("Oh,~not~yet.");
                                                data.AddAvatar(113);
                                                data.FinalizeDialog();
                                            }
                                            else
                                            {
                                                if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Apothecary) == 6)
                                                {
                                                    data.AddText("To~get~promoted~to~Phase~7~and~reduce~poison~damage~further,~you~should~have~a~character~level~of~120,~~become~a~level~7~Apothecary~and~have~been~reborn~once.~Have~you~met~all~these~requirements?");
                                                    data.AddOption("Positive.", 13);
                                                    data.AddOption("Oh,~not~yet.");
                                                    data.AddAvatar(113);
                                                    data.FinalizeDialog();
                                                }
                                                else
                                                {
                                                    if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Apothecary) == 7)
                                                    {
                                                        data.AddText("To~get~promoted~to~Phase~8~and~reduce~poison~damage~further,~you~should~have~a~character~level~of~120,~~become~a~level~8~Apothecary~and~have~been~reborn~twice.~Have~you~met~all~these~requirements?");
                                                        data.AddOption("Positive.", 14);
                                                        data.AddOption("Oh,~not~yet.");
                                                        data.AddAvatar(113);
                                                        data.FinalizeDialog();
                                                    }
                                                    else
                                                    {
                                                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Apothecary) == 8)
                                                        {
                                                            data.AddText("To~get~promoted~to~Phase~9~and~reduce~poison~damage~fully,~you~should~have~a~character~level~of~130,~~become~a~level~9~Apothecary~and~have~been~reborn~twice.~Have~you~met~all~these~requirements?");
                                                            data.AddOption("Positive.", 15);
                                                            data.AddOption("Oh,~not~yet.");
                                                            data.AddAvatar(113);
                                                            data.FinalizeDialog();
                                                        }
                                                        else
                                                        {
                                                            data.AddText("There~is~nothing~more~I~can~teach~you,~now~that~you`ve~reached~the~highest~phase~of~this~sub-class.~Well~done!");
                                                            data.AddOption("Oh,~thanks~anyway.");
                                                            data.AddAvatar(113);
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
                        break;
                    }
                case 15:
                    {
                        if (client.Player.Level < 130)
                        {
                            data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                            data.AddOption("Oh.");
                            data.AddAvatar(113);
                            data.AddAvatar(113);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Apothecary) < 9)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(113);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 2)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(113);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Apothecary, 9, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~9~Apothecary~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~9~Apothecary~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(113);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 14:
                    {
                        if (client.Player.Level < 120)
                        {
                            data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                            data.AddOption("Oh.");
                            data.AddAvatar(113);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Apothecary) < 8)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(113);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 2)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(113);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Apothecary, 8, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~8~Apothecary~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~8~Apothecary~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(113);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 13:
                    {
                        if (client.Player.Reborn < 2)
                        {
                            if (client.Player.Level < 120)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(113);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Apothecary) < 7)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(113);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Reborn < 1)
                                    {
                                        data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                        data.AddOption("Oh.");
                                        data.AddAvatar(113);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Apothecary, 7, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~7~Apothecary~now!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("You~are~a~Phase~7~Apothecary~now!");
                                        data.AddOption("Great.");
                                        data.AddAvatar(113);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Apothecary) < 7)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(113);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 1)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(113);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Apothecary, 7, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~7~Apothecary~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~7~Apothecary~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(113);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 12:
                    {

                        if (client.Player.Reborn < 2)
                        {
                            if (client.Player.Level < 110)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(113);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Apothecary) < 6)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(113);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Reborn < 1)
                                    {
                                        data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                        data.AddOption("Oh.");
                                        data.AddAvatar(113);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Apothecary, 6, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~6~Apothecary~now!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("You~are~a~Phase~6~Apothecary~now!");
                                        data.AddOption("Great.");
                                        data.AddAvatar(113);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Apothecary) < 6)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(113);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 1)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(113);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Apothecary, 6, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~6~Apothecary~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~6~Apothecary~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(113);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }

                case 11:
                    {
                        if (client.Player.Reborn < 2)
                        {
                            if (client.Player.Level < 90)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(113);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Apothecary) < 5)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(113);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Reborn < 1)
                                    {
                                        data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                        data.AddOption("Oh.");
                                        data.AddAvatar(113);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Apothecary, 5, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~5~Apothecary~now!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("You~are~a~Phase~5~Apothecary~now!");
                                        data.AddOption("Great.");
                                        data.AddAvatar(113);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Apothecary) < 5)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(113);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 1)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(113);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Apothecary, 5, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~5~Apothecary~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~5~Apothecary~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(113);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 10:
                    {
                        if (client.Player.Reborn < 1)
                        {
                            if (client.Player.Level < 120)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(113);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Apothecary) < 4)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(113);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Apothecary, 4, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~4~Apothecary~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~4~Apothecary~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(113);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Apothecary) < 4)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(113);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Apothecary, 4, client, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~4~Apothecary~now!", MsgMessage.ChatMode.TopLeftSystem);
                                data.AddText("You~are~a~Phase~4~Apothecary~now!");
                                data.AddOption("Great.");
                                data.AddAvatar(113);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 9:
                    {
                        if (client.Player.Reborn < 1)
                        {
                            if (client.Player.Level < 110)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(113);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Apothecary) < 3)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(113);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Apothecary, 3, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~3~Apothecary~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~2~Apothecary~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(113);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Apothecary) < 3)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(113);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Apothecary, 3, client, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~3~Apothecary~now!", MsgMessage.ChatMode.TopLeftSystem);
                                data.AddText("You~are~a~Phase~2~Apothecary~now!");
                                data.AddOption("Great.");
                                data.AddAvatar(113);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 8:
                    {
                        if (client.Player.Reborn < 1)
                        {
                            if (client.Player.Level < 90)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(113);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Apothecary) < 2)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(113);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Apothecary, 2, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~2~Apothecary~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~2~Apothecary~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(113);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Apothecary) < 2)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(113);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Apothecary, 2, client, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~2~Apothecary~now!", MsgMessage.ChatMode.TopLeftSystem);
                                data.AddText("You~are~a~Phase~2~Apothecary~now!");
                                data.AddOption("Great.");
                                data.AddAvatar(113);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        data.AddText("The~Apothecaries~are~quite~good~at~protecting~themselves~from~poison~damage.~If~you~want~to~join~us,~you~should~have~reached~Level~70~and~pay~me~10~Meteors~or~800~Study~Points.");
                        data.AddText("~After~that,~you~can~start~your~sub-class~training~on~the~`Status`~sheet.~Remember,~a~certain~sub-class~level~is~required~to~promote~the~phase~of~your~sub-class.");
                        data.AddOption("Pay~10~Meteors~to~join.", 6);
                        data.AddOption("Pay~800~Study~Points~to~join.", 4);
                        data.AddOption("Oh,~it~does~not~suit~me.");
                        data.AddAvatar(113);
                        data.FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Apothecary) < 1)
                        {
                            if (client.Player.Reborn < 1)
                            {
                                if (client.Player.Level < 70)
                                {
                                    data.AddText("You~will~be~invited~to~join~us~after~you~reach~level~70.~Work~harder!");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(113);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Inventory.Contain(1088001, 10))
                                    {
                                        if (client.Inventory.Remove(1088001, 10, stream))
                                        {
                                            client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Apothecary, client, stream);
                                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                            Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~an~Apothecary~now!", MsgMessage.ChatMode.TopLeftSystem);
                                            data.AddText("Welcome~to~the~Apothecary~community!~This~is~a~big~community~and~we~have~9~phases~for~the~advancement~of~Apothecaries.~Work~hard~in~the~future~and~when~you~are~ready~to~get~promoted,~I`m~always~here~for~you.");
                                            data.AddOption("Thank~you.");
                                            data.AddAvatar(113);
                                            data.FinalizeDialog();
                                        }
                                    }
                                    else
                                    {
                                        data.AddText("You~don`t~have~10~Meteor.~There~is~nothing~I~can~do~for~you.");
                                        data.AddOption("Oh.~What~a~pity.");
                                        data.AddAvatar(113);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                            else
                            {
                                if (client.Inventory.Contain(1088001, 10))
                                {
                                    if (client.Inventory.Remove(1088001, 10, stream))
                                    {
                                        client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Apothecary, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~an~Apothecary~now!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("Welcome~to~the~Apothecary~community!~This~is~a~big~community~and~we~have~9~phases~for~the~advancement~of~Apothecaries.~Work~hard~in~the~future~and~when~you~are~ready~to~get~promoted,~I`m~always~here~for~you.");
                                        data.AddOption("Thank~you.");
                                        data.AddAvatar(113);
                                        data.FinalizeDialog();
                                    }
                                }
                                else
                                {
                                    data.AddText("You~don`t~have~10~Meteor.~There~is~nothing~I~can~do~for~you.");
                                    data.AddOption("Oh.~What~a~pity.");
                                    data.AddAvatar(113);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            data.AddText("Aha,~you~are~already~an~Apothecary,~remember?~I~suggest~you~focus~your~attention~on~upgrading~your~sub-class~level,~now.");
                            data.AddOption("Oh.");
                            data.AddAvatar(113);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 4:
                    {
                        data.AddText("~~~~Are~you~sure~you~want~to~pay~800~Study~Points~to~join~us?");
                        data.AddOption("Yes,~I`ve~decided!", 5);
                        data.AddOption("No,~I~changed~my~mind.");
                        data.AddAvatar(113);
                        data.FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Apothecary) < 1)
                        {
                            if (client.Player.Reborn < 1)
                            {
                                if (client.Player.Level < 70)
                                {
                                    data.AddText("You~will~be~invited~to~join~us~after~you~reach~level~70.~Work~harder!");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(113);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.SubClass.StudyPoints < 800)
                                    {
                                        data.AddText("You~don`t~have~800~Study~Points~to~take~the~sub-class.");
                                        data.AddOption("Alright.");
                                        data.AddAvatar(113);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.RemoveStudy(client, 800, stream);
                                        {
                                            client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Apothecary, client, stream);
                                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                            Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~an~Apothecary~now!", MsgMessage.ChatMode.TopLeftSystem);
                                            data.AddText("Welcome~to~the~Apothecary~community!~This~is~a~big~community~and~we~have~9~phases~for~the~advancement~of~Apothecaries.~Work~hard~in~the~future~and~when~you~are~ready~to~get~promoted,~I`m~always~here~for~you.");
                                            data.AddOption("Thank~you.");
                                            data.AddAvatar(113);
                                            data.FinalizeDialog();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (client.Player.SubClass.StudyPoints < 800)
                                {
                                    data.AddText("You~don`t~have~800~Study~Points~to~take~the~sub-class.");
                                    data.AddOption("Alright.");
                                    data.AddAvatar(113);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.RemoveStudy(client, 800, stream);
                                    {
                                        client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Apothecary, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~an~Apothecary~now!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("Welcome~to~the~Apothecary~community!~This~is~a~big~community~and~we~have~9~phases~for~the~advancement~of~Apothecaries.~Work~hard~in~the~future~and~when~you~are~ready~to~get~promoted,~I`m~always~here~for~you.");
                                        data.AddOption("Thank~you.");
                                        data.AddAvatar(113);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            data.AddText("Aha,~you~are~already~an~Apothecary,~remember?~I~suggest~you~focus~your~attention~on~upgrading~your~sub-class~level,~now.");
                            data.AddOption("Oh.");
                            data.AddAvatar(113);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        data.AddText("Good~day!~Please~spare~me~a~few~minutes~so~that~I~can~tell~you~about~the~sub-classes~in~Twin~City.~Each~Sub-Class~has~9~phases,~and~the~higher~the~phase~is,~the~more~bonus~effects~the~sub-class~will~offer.~But~keep~in~mind~that~these");
                        data.AddText("~sub-classes~also~have~a~level~system~and~you~need~to~level~them~up~before~you~can~get~promoted.~You~can~level~up~your~sub-class~by~gaining~study~points.");
                        data.AddOption("How~can~I~get~study~points?", 2);
                        data.AddOption("Oh.~Thanks~for~telling~me.");
                        data.AddAvatar(113);
                        data.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("Well,~the~only~way~to~get~those~points~is~to~read~the~sub-class~books,~such~as~the~Diligence~Book.~When~you`ve~gained~enough~study~points,~open~the~sub-class~sheet~from~the~character~panel~and");
                        data.AddText("~level~up~the~sub-class~there.~Monsters~in~the~wild~are~always~bothering~young~adventurers,~so~you~might~find~some~books~from~them.");
                        data.AddOption("I~see.");
                        data.AddAvatar(113);
                        data.FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.MartialArtist)]
        public static void MartialArtist(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello~there!~You~look~quite~capable.~Perhaps~you`ll~make~a~great~Martial~Artist.~Please~allow~me~to~explain~the~Martial~Artist~sub-class.~The~Martial~Artist`s~awesome~displays~of~combat~prowess~are~a~wonder~to~behold,~but~a~GameServer~to~face.");
                        data.AddText("~When~you~are~a~Martial~Artist,~you~will~be~able~to~increase~the~chances~of~hitting~with~1.5x~Attack~Power!~");
                        data.AddText("~Would~you~like~to~join~us?~After~you~join~this~sub-class,~you~can~level~it~up~and~get~promoted~to~increase~your~chances~of~hitting~your~enemies~with~more~power!");
                        data.AddOption("Yes,~I`d~like~to~join.", 3);
                        data.AddOption("I~want~to~get~promoted.", 7);
                        data.AddOption("How~can~I~get~promoted?", 1);
                        data.AddOption("Nice.~I`ll~think~about~it.");
                        data.AddAvatar(32);
                        data.FinalizeDialog();
                        break;
                    }
                case 7:
                    {
                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.MartialArtist) < 1)
                        {
                            data.AddText("You~are~not~a~Martial~Artist~yet~and~can`t~get~promoted.~Do~you~want~to~join~the~Martial~Artist~sub-class~now?");
                            data.AddOption("Yes,~I`d~like~to~join.", 3);
                            data.AddOption("Oh.~Not~now.");
                            data.AddAvatar(32);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.MartialArtist) == 1)
                            {
                                data.AddText("To~get~promoted~to~Phase~2~and~increase~the~chance~of~making~1.5x~Attack,~you~should~have~a~character~level~of~90~and~become~a~level~2~Martial~Artist.~Have~you~met~all~these~requirements?");
                                data.AddOption("Positive.", 8);
                                data.AddOption("Oh,~not~yet.");
                                data.AddAvatar(32);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.MartialArtist) == 2)
                                {
                                    data.AddText("To~get~promoted~to~Phase~3~and~increase~your~chances~of~hitting~with~1.5x~Attack,~you~should~have~a~character~level~of~110~and~become~a~level~3~Martial~Artist.~Have~you~met~all~these~requirements?");
                                    data.AddOption("Positive.", 9);
                                    data.AddOption("Oh,~not~yet.");
                                    data.AddAvatar(32);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.MartialArtist) == 3)
                                    {
                                        data.AddText("To~get~promoted~to~Phase~4~and~increase~your~chances~of~hitting~with~1.5x~Attack,~you~should~have~a~character~level~of~120~and~become~a~level~4~Martial~Artist.~Have~you~met~all~these~requirements?");
                                        data.AddOption("Positive.", 10);
                                        data.AddOption("Oh,~not~yet.");
                                        data.AddAvatar(32);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.MartialArtist) == 4)
                                        {
                                            data.AddText("To~get~promoted~to~Phase~5~and~increase~your~chances~of~hitting~with~1.5x~Attack,~you~should~have~a~character~level~of~90,~become~a~level~5~Martial~Artist,~and~have~been~reborn~once.~Have~you~met~all~these~requirements?");
                                            data.AddOption("Positive.", 11);
                                            data.AddOption("Oh,~not~yet.");
                                            data.AddAvatar(32);
                                            data.FinalizeDialog();
                                        }
                                        else
                                        {
                                            if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.MartialArtist) == 5)
                                            {
                                                data.AddText("To~get~promoted~to~Phase~6~and~increase~your~chances~of~hitting~with~1.5x~Attack,~you~should~have~a~character~level~of~110,~become~a~level~6~Martial~Artist,~and~have~been~reborn~once.~Have~you~met~all~these~requirements?");
                                                data.AddOption("Positive.", 12);
                                                data.AddOption("Oh,~not~yet.");
                                                data.AddAvatar(32);
                                                data.FinalizeDialog();
                                            }
                                            else
                                            {
                                                if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.MartialArtist) == 6)
                                                {
                                                    data.AddText("To~get~promoted~to~Phase~7~and~increase~your~chances~of~hitting~with~1.5x~Attack,~you~should~have~a~character~level~of~120,~become~a~level~7~Martial~Artist,~and~have~been~reborn~once.~Have~you~met~all~these~requirements?");
                                                    data.AddOption("Positive.", 13);
                                                    data.AddOption("Oh,~not~yet.");
                                                    data.AddAvatar(32);
                                                    data.FinalizeDialog();
                                                }
                                                else
                                                {
                                                    if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.MartialArtist) == 7)
                                                    {
                                                        data.AddText("To~get~promoted~to~Phase~8~and~increase~your~chances~of~hitting~with~1.5x~Attack,~you~should~have~a~character~level~of~120,~become~a~level~8~Martial~Artist,~and~have~been~reborn~twice.~Have~you~met~all~these~requirements?");
                                                        data.AddOption("Positive.", 14);
                                                        data.AddOption("Oh,~not~yet.");
                                                        data.AddAvatar(32);
                                                        data.FinalizeDialog();
                                                    }
                                                    else
                                                    {
                                                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.MartialArtist) == 8)
                                                        {
                                                            data.AddText("To~get~promoted~to~Phase~9~and~increase~your~chances~of~hitting~with~1.5x~Attack,~you~should~have~a~character~level~of~130,~become~a~level~9~Martial~Artist,~and~have~been~reborn~twice.~Have~you~met~all~these~requirements?");
                                                            data.AddOption("Positive.", 15);
                                                            data.AddOption("Oh,~not~yet.");
                                                            data.AddAvatar(32);
                                                            data.FinalizeDialog();
                                                        }
                                                        else
                                                        {
                                                            data.AddText("There~is~nothing~more~I~can~teach~you,~now~that~you`ve~reached~the~highest~phase~of~this~sub-class.~Well~done!");
                                                            data.AddOption("Thank~you.");
                                                            data.AddAvatar(32);
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
                        break;
                    }
                case 15:
                    {
                        if (client.Player.Level < 130)
                        {
                            data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                            data.AddOption("Oh.");
                            data.AddAvatar(32);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.MartialArtist) < 9)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(32);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 2)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(32);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.MartialArtist, 9, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~9~Martial~Artist~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~9~Martial~Artist~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(32);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 14:
                    {
                        if (client.Player.Level < 120)
                        {
                            data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                            data.AddOption("Oh.");
                            data.AddAvatar(32);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.MartialArtist) < 8)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(32);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 2)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(32);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.MartialArtist, 8, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~8~Martial~Artist~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~8~Martial~Artist~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(32);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 13:
                    {
                        if (client.Player.Reborn < 2)
                        {
                            if (client.Player.Level < 120)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(32);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.MartialArtist) < 7)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(32);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Reborn < 1)
                                    {
                                        data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                        data.AddOption("Oh.");
                                        data.AddAvatar(32);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.MartialArtist, 7, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~7~Martial~Artist~now!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("You~are~a~Phase~7~Martial~Artist~now!");
                                        data.AddOption("Great.");
                                        data.AddAvatar(32);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.MartialArtist) < 7)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(32);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 1)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(32);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.MartialArtist, 7, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~7~Martial~Artist~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~7~Martial~Artist~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(32);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 12:
                    {
                        if (client.Player.Reborn < 2)
                        {
                            if (client.Player.Level < 110)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(32);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.MartialArtist) < 6)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(32);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Reborn < 1)
                                    {
                                        data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                        data.AddOption("Oh.");
                                        data.AddAvatar(32);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.MartialArtist, 6, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~6~Martial~Artist~now!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("You~are~a~Phase~6~Martial~Artist~now!");
                                        data.AddOption("Great.");
                                        data.AddAvatar(32);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.MartialArtist) < 6)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(32);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 1)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(32);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.MartialArtist, 6, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~6~Martial~Artist~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~6~Martial~Artist~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(32);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 11:
                    {
                        if (client.Player.Reborn < 2)
                        {
                            if (client.Player.Level < 90)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(32);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.MartialArtist) < 5)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(32);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Reborn < 1)
                                    {
                                        data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                        data.AddOption("Oh.");
                                        data.AddAvatar(32);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.MartialArtist, 5, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~5~Martial~Artist~now!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("You~are~a~Phase~5~Martial~Artist~now!");
                                        data.AddOption("Great.");
                                        data.AddAvatar(32);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.MartialArtist) < 5)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(32);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 1)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(32);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.MartialArtist, 5, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~5~Martial~Artist~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~5~Martial~Artist~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(32);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 10:
                    {
                        if (client.Player.Reborn < 1)
                        {
                            if (client.Player.Level < 120)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(32);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.MartialArtist) < 4)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(32);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.MartialArtist, 4, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~4~Martial~Artist~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~4~Martial~Artist~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(32);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.MartialArtist) < 4)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(32);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.MartialArtist, 4, client, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~4~Martial~Artist~now!", MsgMessage.ChatMode.TopLeftSystem);
                                data.AddText("You~are~a~Phase~4~Martial~Artist~now!");
                                data.AddOption("Great.");
                                data.AddAvatar(32);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 9:
                    {
                        if (client.Player.Reborn < 1)
                        {
                            if (client.Player.Level < 110)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(32);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.MartialArtist) < 3)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(32);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.MartialArtist, 3, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~3~Martial~Artist~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~3~Martial~Artist~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(32);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.MartialArtist) < 3)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(32);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.MartialArtist, 3, client, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~3~Martial~Artist~now!", MsgMessage.ChatMode.TopLeftSystem);
                                data.AddText("You~are~a~Phase~3~Martial~Artist~now!");
                                data.AddOption("Great.");
                                data.AddAvatar(32);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 8:
                    {
                        if (client.Player.Reborn < 1)
                        {
                            if (client.Player.Level < 90)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(32);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.MartialArtist) < 2)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(32);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.MartialArtist, 2, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~2~Martial~Artist~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~2~Martial~Artist~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(32);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.MartialArtist) < 2)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(32);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.MartialArtist, 2, client, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~2~Martial~Artist~now!", MsgMessage.ChatMode.TopLeftSystem);
                                data.AddText("You~are~a~Phase~2~Martial~Artist~now!");
                                data.AddOption("Great.");
                                data.AddAvatar(32);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        data.AddText("Being~a~Martial~Artist,~you`ll~find~your~Critical~Strike~nicely~enhanced.~Interested?~Make~sure~you`ve~reached~Level~70,~and~pay~5~Celestial~Stones~or~800~Study~Points~to~join~us!");
                        data.AddText("~After~that,~you~can~working~on~your~sub-class~skill~on~the~`Status`~sheet.~Remember,~a~certain~sub-class~level~is~required~to~promote~the~phase~of~your~sub-class.");
                        data.AddOption("Pay~5~CelestialStones~to~join.", 6);
                        data.AddOption("Pay~800~Study~Points~to~join.", 4);
                        data.AddOption("Oh,~it~does~not~suit~me.");
                        data.AddAvatar(32);
                        data.FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.MartialArtist) < 1)
                        {
                            if (client.Player.Reborn < 1)
                            {
                                if (client.Player.Level < 70)
                                {
                                    data.AddText("You~will~be~invited~to~join~us~after~you~reach~level~70.~Work~harder!");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(32);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Inventory.Contain(721259, 5) || client.Inventory.Contain(721259, 5, 1))
                                    {
                                        if (client.Inventory.Remove(721259, 5, stream))
                                        {
                                            client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.MartialArtist, client, stream);
                                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                            Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Martial~Artist~now!", MsgMessage.ChatMode.TopLeftSystem);
                                            data.AddText("Welcome~to~the~Martial~Artist~community!~This~is~a~big~community~and~we~have~9~phases~for~the~advancement~of~Martial~Artists.~Work~hard~in~the~future~and~when~you~are~ready~to~get~promoted,~I`m~always~here~for~you.");
                                            data.AddOption("Thanks~in~advance.");
                                            data.AddAvatar(32);
                                            data.FinalizeDialog();
                                        }
                                    }
                                    else
                                    {
                                        data.AddText("You~don`t~have~5~Celestial~Stones~to~join~this~sub-class.~Sorry.");
                                        data.AddOption("Oh.");
                                        data.AddAvatar(32);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                            else
                            {
                                if (client.Inventory.Contain(721259, 5) || client.Inventory.Contain(721259, 5, 1))
                                {
                                    if (client.Inventory.Remove(721259, 5, stream))
                                    {
                                        client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.MartialArtist, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Martial~Artist~now!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("Welcome~to~the~Martial~Artist~community!~This~is~a~big~community~and~we~have~9~phases~for~the~advancement~of~Martial~Artists.~Work~hard~in~the~future~and~when~you~are~ready~to~get~promoted,~I`m~always~here~for~you.");
                                        data.AddOption("Thanks~in~advance.");
                                        data.AddAvatar(32);
                                        data.FinalizeDialog();
                                    }
                                }
                                else
                                {
                                    data.AddText("You~don`t~have~5~Celestial~Stones~to~join~this~sub-class.~Sorry.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(32);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            data.AddText("Aha,~you~are~already~a~Martial~Artist,~remember?~I~suggest~you~focus~your~attention~on~upgrading~your~sub-class~level,~now.");
                            data.AddOption("Oh.");
                            data.AddAvatar(32);
                            data.FinalizeDialog();
                        }

                        break;
                    }
                case 4:
                    {
                        data.AddText("~~~~Are~you~sure~you~want~to~pay~800~Study~Points~to~join~us?");
                        data.AddOption("Yes,~I`ve~decided!", 5);
                        data.AddOption("No,~I~changed~my~mind.");
                        data.AddAvatar(32);
                        data.FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.MartialArtist) < 1)
                        {
                            if (client.Player.Reborn < 1)
                            {
                                if (client.Player.Level < 70)
                                {
                                    data.AddText("You~will~be~invited~to~join~us~after~you~reach~level~70.~Work~harder!");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(32);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.SubClass.StudyPoints < 800)
                                    {
                                        data.AddText("You~don`t~have~800~Study~Points~to~take~the~sub-class.");
                                        data.AddOption("Alright.");
                                        data.AddAvatar(32);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.RemoveStudy(client, 800, stream);
                                        {
                                            client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.MartialArtist, client, stream);
                                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                            Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Martial~Artist~now!", MsgMessage.ChatMode.TopLeftSystem);
                                            data.AddText("Welcome~to~the~Martial~Artist~community!~This~is~a~big~community~and~we~have~9~phases~for~the~advancement~of~Martial~Artists.~Work~hard~in~the~future~and~when~you~are~ready~to~get~promoted,~I`m~always~here~for~you.");
                                            data.AddOption("Thanks~in~advance.");
                                            data.AddAvatar(32);
                                            data.FinalizeDialog();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (client.Player.SubClass.StudyPoints < 800)
                                {
                                    data.AddText("You~don`t~have~800~Study~Points~to~take~the~sub-class.");
                                    data.AddOption("Alright.");
                                    data.AddAvatar(32);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.RemoveStudy(client, 800, stream);
                                    {
                                        client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.MartialArtist, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Martial~Artist~now!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("Welcome~to~the~Martial~Artist~community!~This~is~a~big~community~and~we~have~9~phases~for~the~advancement~of~Martial~Artists.~Work~hard~in~the~future~and~when~you~are~ready~to~get~promoted,~I`m~always~here~for~you.");
                                        data.AddOption("Thanks~in~advance.");
                                        data.AddAvatar(32);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            data.AddText("Aha,~you~are~already~a~Martial~Artist,~remember?~I~suggest~you~focus~your~attention~on~upgrading~your~sub-class~level,~now.");
                            data.AddOption("Oh.");
                            data.AddAvatar(32);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        data.AddText("Good~day!~Please~spare~me~a~few~minutes~so~that~I~can~tell~you~about~the~sub-classes~in~Twin~City.~Each~Sub-Class~has~9~phases,~and~the~higher~the~phase~is,~the~more~bonus~effects~the~sub-class~will~offer.~But~keep~in~mind~that~these");
                        data.AddText("~sub-classes~also~have~a~level~system~and~you~need~to~level~them~up~before~you~can~get~promoted.~You~can~level~up~your~sub-class~by~gaining~study~points.");
                        data.AddOption("How~can~I~get~study~points?", 2);
                        data.AddOption("Oh.");
                        data.AddAvatar(32);
                        data.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("Well,~the~only~way~to~get~those~points~is~to~read~the~sub-class~books,~such~as~the~Diligence~Book.~When~you`ve~gained~enough~study~points,~open~the~sub-class~sheet~from~the~character~panel~and");
                        data.AddText("~level~up~the~sub-class~there.~Monsters~in~the~wild~are~always~bothering~young~adventurers,~so~you~might~find~some~books~from~them.");
                        data.AddOption("Oh.");
                        data.AddAvatar(32);
                        data.FinalizeDialog();
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.Warlock)]
        public static void Warlock(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Good~day!~Have~you~ever~wondered~what~a~Warlock~is?~Well,~please~allow~me~to~introduce~this~sub-class~to~you.~Warlocks~are~masters~of~the~the~psionic~energies~and~can~increase~their~chances~of~casting~spells~with~2x~Magical~Attack.");
                        data.AddText("~Isn`t~that~wonderful?~Would~you~like~to~join~us?~After~you~join~this~sub-class,~you~can~level~it~up~and~get~promoted~to~increase~your~power!");
                        data.AddOption("Yes,~I`d~like~to~join.", 3);
                        data.AddOption("I~want~to~get~promoted.", 7);
                        data.AddOption("How~can~I~get~promoted?", 1);
                        data.AddOption("Not~my~concern.");
                        data.AddAvatar(23);
                        data.FinalizeDialog();
                        break;
                    }
                case 7:
                    {
                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Warlock) < 1)
                        {
                            data.AddText("You~are~not~a~Warlock~and~can`t~get~promoted.~Do~you~want~to~join~the~Warlock~sub-class~now?");
                            data.AddOption("Yes,~I`d~like~to~join~it.", 3);
                            data.AddOption("Oh.~I`ll~think~about~it.");
                            data.AddAvatar(23);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Warlock) == 1)
                            {
                                data.AddText("To~get~promoted~to~Phase~2~and~increase~your~chances~of~casting~with~2x~Magical~Attack,~you~should~have~a~character~level~of~90~and~become~a~level~2~Warlock.~Have~you~met~all~these~requirements?");
                                data.AddOption("Sure.", 8);
                                data.AddOption("Oh,~not~yet.");
                                data.AddAvatar(23);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Warlock) == 2)
                                {
                                    data.AddText("To~get~promoted~to~Phase~3~and~increase~your~chances~of~casting~with~2x~Magical~Attack,~you~should~have~a~character~level~of~110~and~become~a~level~3~Warlock.~Have~you~met~all~these~requirements?");
                                    data.AddOption("Sure.", 9);
                                    data.AddOption("Oh,~not~yet.");
                                    data.AddAvatar(23);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Warlock) == 3)
                                    {
                                        data.AddText("To~get~promoted~to~Phase~4~and~increase~your~chances~of~casting~with~2x~Magical~Attack,~you~should~have~a~character~level~of~120~and~become~a~level~4~Warlock.~Have~you~met~all~these~requirements?");
                                        data.AddOption("Sure.", 10);
                                        data.AddOption("Oh,~not~yet.");
                                        data.AddAvatar(23);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Warlock) == 4)
                                        {
                                            data.AddText("To~get~promoted~to~Phase~5~and~increase~your~chances~of~casting~with~2x~Magical~Attack,~you~should~have~a~character~level~of~90,~become~a~level~5~Warlock~and~have~been~reborn~once.~Have~you~met~all~these~requirements?");
                                            data.AddOption("Sure.", 11);
                                            data.AddOption("Oh,~not~yet.");
                                            data.AddAvatar(23);
                                            data.FinalizeDialog();
                                        }
                                        else
                                        {
                                            if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Warlock) == 5)
                                            {
                                                data.AddText("To~get~promoted~to~Phase~6~and~increase~your~chances~of~casting~with~2x~Magical~Attack,~you~should~have~a~character~level~of~110,~become~a~level~6~Warlock~and~have~been~reborn~once.~Have~you~met~all~these~requirements?");
                                                data.AddOption("Sure.", 12);
                                                data.AddOption("Oh,~not~yet.");
                                                data.AddAvatar(23);
                                                data.FinalizeDialog();
                                            }
                                            else
                                            {
                                                if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Warlock) == 6)
                                                {
                                                    data.AddText("To~get~promoted~to~Phase~7~and~increase~your~chances~of~casting~with~2x~Magical~Attack,~you~should~have~a~character~level~of~120,~become~a~level~7~Warlock~and~have~been~reborn~once.~Have~you~met~all~these~requirements?");
                                                    data.AddOption("Sure.", 13);
                                                    data.AddOption("Oh,~not~yet.");
                                                    data.AddAvatar(23);
                                                    data.FinalizeDialog();
                                                }
                                                else
                                                {
                                                    if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Warlock) == 7)
                                                    {
                                                        data.AddText("To~get~promoted~to~Phase~8~and~increase~your~chances~of~casting~with~2x~Magical~Attack,~you~should~have~a~character~level~of~120,~become~a~level~8~Warlock~and~have~been~reborn~twice.~Have~you~met~all~these~requirements?");
                                                        data.AddOption("Sure.", 14);
                                                        data.AddOption("Oh,~not~yet.");
                                                        data.AddAvatar(23);
                                                        data.FinalizeDialog();
                                                    }
                                                    else
                                                    {
                                                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Warlock) == 8)
                                                        {
                                                            data.AddText("To~get~promoted~to~Phase~9~and~increase~your~chances~of~casting~with~2x~Magical~Attack,~you~should~have~a~character~level~of~130,~become~a~level~9~Warlock~and~have~been~reborn~twice.~Have~you~met~all~these~requirements?");
                                                            data.AddOption("Sure.", 15);
                                                            data.AddOption("Oh,~not~yet.");
                                                            data.AddAvatar(23);
                                                            data.FinalizeDialog();
                                                        }
                                                        else
                                                        {
                                                            data.AddText("There~is~nothing~more~I~can~teach~you,~now~that~you`ve~reached~the~highest~phase~of~this~sub-class.~Well~done!");
                                                            data.AddOption("Oh,~it~does~not~suit~me.");
                                                            data.AddAvatar(23);
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
                        break;
                    }
                case 15:
                    {
                        if (client.Player.Level < 130)
                        {
                            data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                            data.AddOption("Oh.");
                            data.AddAvatar(23);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Warlock) < 9)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(23);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 2)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(23);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Warlock, 9, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~9~Warlock~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~9~Warlock~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(23);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 14:
                    {
                        if (client.Player.Level < 120)
                        {
                            data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                            data.AddOption("Oh.");
                            data.AddAvatar(23);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Warlock) < 8)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(23);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 2)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(23);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Warlock, 8, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~8~Warlock~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~8~Warlock~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(23);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 13:
                    {
                        if (client.Player.Reborn < 2)
                        {
                            if (client.Player.Level < 120)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(23);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Warlock) < 7)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(23);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Reborn < 1)
                                    {
                                        data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                        data.AddOption("Oh.");
                                        data.AddAvatar(23);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Warlock, 7, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~7~Warlock~now!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("You~are~a~Phase~7~Warlock~now!");
                                        data.AddOption("Great.");
                                        data.AddAvatar(23);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Warlock) < 7)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(23);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 1)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(23);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Warlock, 7, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~7~Warlock~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~7~Warlock~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(23);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 12:
                    {
                        if (client.Player.Reborn < 2)
                        {
                            if (client.Player.Level < 110)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(23);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Warlock) < 6)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(23);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Reborn < 1)
                                    {
                                        data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                        data.AddOption("Oh.");
                                        data.AddAvatar(23);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Warlock, 6, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~6~Warlock~now!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("You~are~a~Phase~6~Warlock~now!");
                                        data.AddOption("Great.");
                                        data.AddAvatar(23);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Warlock) < 6)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(23);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 1)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(23);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Warlock, 6, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~6~Warlock~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~6~Warlock~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(23);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 11:
                    {
                        if (client.Player.Reborn < 2)
                        {
                            if (client.Player.Level < 90)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(23);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Warlock) < 5)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(23);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Reborn < 1)
                                    {
                                        data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                        data.AddOption("Oh.");
                                        data.AddAvatar(23);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Warlock, 5, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~5~Warlock~now!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("You~are~a~Phase~5~Warlock~now!");
                                        data.AddOption("Great.");
                                        data.AddAvatar(23);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Warlock) < 5)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(23);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 1)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(23);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Warlock, 5, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~5~Warlock~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~5~Warlock~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(23);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 10:
                    {
                        if (client.Player.Reborn < 1)
                        {
                            if (client.Player.Level < 120)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(23);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Warlock) < 4)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(23);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Warlock, 4, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~4~Warlock~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~4~Warlock~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(23);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Warlock) < 4)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(23);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Warlock, 4, client, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~4~Warlock~now!", MsgMessage.ChatMode.TopLeftSystem);
                                data.AddText("You~are~a~Phase~4~Warlock~now!");
                                data.AddOption("Great.");
                                data.AddAvatar(23);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 9:
                    {
                        if (client.Player.Reborn < 1)
                        {
                            if (client.Player.Level < 110)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(23);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Warlock) < 3)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(23);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Warlock, 3, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~3~Warlock~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~3~Warlock~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(23);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Warlock) < 3)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(23);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Warlock, 3, client, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~3~Warlock~now!", MsgMessage.ChatMode.TopLeftSystem);
                                data.AddText("You~are~a~Phase~3~Warlock~now!");
                                data.AddOption("Great.");
                                data.AddAvatar(23);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 8:
                    {
                        if (client.Player.Reborn < 1)
                        {
                            if (client.Player.Level < 90)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(23);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Warlock) < 2)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(23);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Warlock, 2, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~2~Warlock~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~2~Warlock~now");
                                    data.AddOption("Great.");
                                    data.AddAvatar(23);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Warlock) < 2)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(23);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Warlock, 2, client, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~2~Warlock~now!", MsgMessage.ChatMode.TopLeftSystem);
                                data.AddText("You~are~a~Phase~2~Warlock~now");
                                data.AddOption("Great.");
                                data.AddAvatar(23);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        data.AddText("Warlocks~are~masters~of~psionic~energies~who~have~the~chance~to~make~Skill~Critical~Strike.~If~you`ve~reached~Level~70,~just~pay~10~Bombs~or~800~Study~Points,");
                        data.AddText("~and~you`ll~be~one~of~us.~After~that,~you~can~enhance~your~sub-class~skill~on~the~`Status`~sheet.~Remember,~a~certain~sub-class~level~is~required~to~promote~the~phase~of~your~sub-class.");
                        data.AddOption("Pay~10~Bombs~to~join.", 6);
                        data.AddOption("Pay~800~Study~Points~to~join.", 4);
                        data.AddOption("Oh,~it~does~not~suit~me.");
                        data.AddAvatar(23);
                        data.FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Warlock) < 1)
                        {
                            if (client.Player.Reborn < 1)
                            {
                                if (client.Player.Level < 70)
                                {
                                    data.AddText("You~will~be~invited~to~join~us~after~you~reach~level~70.~Work~harder!");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(23);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Inventory.Contain(721261, 10))
                                    {
                                        if (client.Inventory.Remove(721261, 10, stream))
                                        {
                                            client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Warlock, client, stream);
                                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                            Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Warlock~now.", MsgMessage.ChatMode.TopLeftSystem);
                                            data.AddText("Welcome~to~the~Warlock~community.~This~is~a~big~community~and~we~have~9~phases~for~the~advancement~of~Warlocks.~Work~hard~in~the~future~and~when~you~are~ready~to~get~promoted,~I`m~always~here~for~you.");
                                            data.AddOption("Thanks.");
                                            data.AddAvatar(23);
                                            data.FinalizeDialog();
                                        }
                                    }
                                    else
                                    {
                                        data.AddText("You~don`t~have~the~10~Bombs~to~join~the~Warlock~sub-class.");
                                        data.AddOption("Oh.");
                                        data.AddAvatar(23);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                            else
                            {
                                if (client.Inventory.Contain(721261, 10))
                                {
                                    if (client.Inventory.Remove(721261, 10, stream))
                                    {
                                        client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Warlock, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Warlock~now.", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("Welcome~to~the~Warlock~community.~This~is~a~big~community~and~we~have~9~phases~for~the~advancement~of~Warlocks.~Work~hard~in~the~future~and~when~you~are~ready~to~get~promoted,~I`m~always~here~for~you.");
                                        data.AddOption("Thanks.");
                                        data.AddAvatar(23);
                                        data.FinalizeDialog();
                                    }
                                }
                                else
                                {
                                    data.AddText("You~don`t~have~the~10~Bombs~to~join~the~Warlock~sub-class.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(23);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            data.AddText("Aha,~you~are~already~a~Warlock,~remember?~I~suggest~you~focus~your~attention~on~upgrading~your~sub-class~level,~now.");
                            data.AddOption("Oh.");
                            data.AddAvatar(23);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 4:
                    {
                        data.AddText("~~~~Are~you~sure~you~want~to~pay~800~Study~Points~to~join~us?");
                        data.AddOption("Yes,~I`ve~decided!", 5);
                        data.AddOption("No,~I~changed~my~mind.");
                        data.AddAvatar(23);
                        data.FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Warlock) < 1)
                        {
                            if (client.Player.Reborn < 1)
                            {
                                if (client.Player.Level < 70)
                                {
                                    data.AddText("You~will~be~invited~to~join~us~after~you~reach~level~70.~Work~harder!");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(23);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.SubClass.StudyPoints < 800)
                                    {
                                        data.AddText("You~don`t~have~800~Study~Points~to~take~the~sub-class.");
                                        data.AddOption("Alright.");
                                        data.AddAvatar(23);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.RemoveStudy(client, 800, stream);
                                        {
                                            client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Warlock, client, stream);
                                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                            Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Warlock~now.", MsgMessage.ChatMode.TopLeftSystem);
                                            data.AddText("Welcome~to~the~Warlock~community.~This~is~a~big~community~and~we~have~9~phases~for~the~advancement~of~Warlocks.~Work~hard~in~the~future~and~when~you~are~ready~to~get~promoted,~I`m~always~here~for~you.");
                                            data.AddOption("Thanks.");
                                            data.AddAvatar(23);
                                            data.FinalizeDialog();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (client.Player.SubClass.StudyPoints < 800)
                                {
                                    data.AddText("You~don`t~have~800~Study~Points~to~take~the~sub-class.");
                                    data.AddOption("Alright.");
                                    data.AddAvatar(23);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.RemoveStudy(client, 800, stream);
                                    {
                                        client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Warlock, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Warlock~now.", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("Welcome~to~the~Warlock~community.~This~is~a~big~community~and~we~have~9~phases~for~the~advancement~of~Warlocks.~Work~hard~in~the~future~and~when~you~are~ready~to~get~promoted,~I`m~always~here~for~you.");
                                        data.AddOption("Thanks.");
                                        data.AddAvatar(23);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            data.AddText("Aha,~you~are~already~a~Warlock,~remember?~I~suggest~you~focus~your~attention~on~upgrading~your~sub-class~level,~now.");
                            data.AddOption("Oh.");
                            data.AddAvatar(23);
                            data.FinalizeDialog();
                        }
                        break;
                    }

                case 1:
                    {
                        data.AddText("Good~day!~Please~spare~me~a~few~minutes~so~that~I~can~tell~you~about~the~sub-classes~in~Twin~City.~Each~Sub-Class~has~9~phases,~and~the~higher~the~phase~is,~the~more~bonus~effects~the~sub-class~will~offer.~But~keep~in~mind~that~these");
                        data.AddText("~sub-classes~also~have~a~level~system~and~you~need~to~level~them~up~before~you~can~get~promoted.~You~can~level~up~your~sub-class~by~gaining~study~points.");
                        data.AddOption("How~can~I~get~study~points?", 2);
                        data.AddOption("Oh.");
                        data.AddAvatar(23);
                        data.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("Well,~the~only~way~to~get~those~points~is~to~read~the~sub-class~books,~such~as~the~Diligence~Book.~When~you`ve~gained~enough~study~points,~open~the~sub-class~sheet~from~the~character~panel~and");
                        data.AddText("~level~up~the~sub-class~there.~Monsters~in~the~wild~are~always~bothering~young~adventurers,~so~you~might~find~some~books~from~them.");
                        data.AddOption("I~see.");
                        data.AddAvatar(23);
                        data.FinalizeDialog();
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.Sage)]
        public static void Sage(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello,~my~friend.~Are~you~here~to~join~the~Sage~sub-class?~Before~we~start,~please~allow~me~to~introduce~this~sub-class~to~you.~Sages~have~strong~spiritual~powers~and~have~a~gruesome~ability~to~reduce~a~target`s");
                        data.AddText("~Magical~Defense.~This~is~very~helpful~in~defeating~those~who~might~underestimate~your~power!");
                        data.AddText("~Would~you~like~to~join~us?~After~you~join~this~sub-class,~you~can~level~it~up~and~get~promoted~to~reduce~more~of~your~target`s~Magical~Defense.");
                        data.AddOption("Yes,~I`d~like~to~join.", 3);
                        data.AddOption("I~want~to~get~promoted.", 7);
                        data.AddOption("How~can~I~get~promoted?", 1);
                        data.AddOption("Not~my~concern.");
                        data.AddAvatar(87);
                        data.FinalizeDialog();
                        break;
                    }
                case 7:
                    {
                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Sage) < 1)
                        {
                            data.AddText("Well,~you~need~to~join~us~before~getting~promoted.~Do~you~want~to~become~a~Sage~now?");
                            data.AddOption("Yes,~I`d~like~to~join~it.", 3);
                            data.AddOption("Oh,~I`m~not~ready~yet.");
                            data.AddAvatar(87);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Sage) == 1)
                            {
                                data.AddText("To~get~promoted~to~Phase~2~and~reduce~more~of~your~target`s~Magical~Defense,~you~should~have~a~character~level~of~90~and~become~a~level~2~Sage.~Have~you~met~all~these~requirements?");
                                data.AddOption("Positive.", 8);
                                data.AddOption("Oh,~not~yet.");
                                data.AddAvatar(87);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Sage) == 2)
                                {
                                    data.AddText("To~get~promoted~to~Phase~3~and~reduce~more~of~your~target`s~Magical~Defense,~you~should~have~a~character~level~of~110~and~become~a~level~3~Sage.~Have~you~met~all~these~requirements?");
                                    data.AddOption("Positive.", 9);
                                    data.AddOption("Oh,~not~yet.");
                                    data.AddAvatar(87);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Sage) == 3)
                                    {
                                        data.AddText("To~get~promoted~to~Phase~4~and~reduce~more~of~your~target`s~Magical~Defense,~you~should~have~a~character~level~of~120~and~become~a~level~4~Sage.~Have~you~met~all~these~requirements?");
                                        data.AddOption("Positive.", 10);
                                        data.AddOption("Oh,~not~yet.");
                                        data.AddAvatar(87);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Sage) == 4)
                                        {
                                            data.AddText("To~get~promoted~to~Phase~5~and~reduce~more~of~your~target`s~Magical~Defense,~you~should~have~a~character~level~of~90,~become~a~level~5~Sage~and~have~been~reborn~once.~Have~you~met~all~these~requirements?");
                                            data.AddOption("Positive.", 11);
                                            data.AddOption("Oh,~not~yet.");
                                            data.AddAvatar(87);
                                            data.FinalizeDialog();
                                        }
                                        else
                                        {
                                            if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Sage) == 5)
                                            {
                                                data.AddText("To~get~promoted~to~Phase~6~and~reduce~more~of~your~target`s~Magical~Defense,~you~should~have~a~character~level~of~110,~become~a~level~6~Sage~and~have~been~reborn~once.~Have~you~met~all~these~requirements?");
                                                data.AddOption("Positive.", 12);
                                                data.AddOption("Oh,~not~yet.");
                                                data.AddAvatar(87);
                                                data.FinalizeDialog();
                                            }
                                            else
                                            {
                                                if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Sage) == 6)
                                                {
                                                    data.AddText("To~get~promoted~to~Phase~7~and~reduce~more~of~your~target`s~Magical~Defense,~you~should~have~a~character~level~of~120,~become~a~level~7~Sage~and~have~been~reborn~once.~Have~you~met~all~these~requirements?");
                                                    data.AddOption("Positive.", 13);
                                                    data.AddOption("Oh,~not~yet.");
                                                    data.AddAvatar(87);
                                                    data.FinalizeDialog();
                                                }
                                                else
                                                {
                                                    if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Sage) == 7)
                                                    {
                                                        data.AddText("To~get~promoted~to~Phase~8~and~reduce~more~of~your~target`s~Magical~Defense,~you~should~have~a~character~level~of~120,~become~a~level~8~Sage~and~have~been~reborn~twice.~Have~you~met~all~these~requirements?");
                                                        data.AddOption("Positive.", 14);
                                                        data.AddOption("Oh,~not~yet.");
                                                        data.AddAvatar(87);
                                                        data.FinalizeDialog();
                                                    }
                                                    else
                                                    {
                                                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Sage) == 8)
                                                        {
                                                            data.AddText("To~get~promoted~to~Phase~9~and~reduce~more~of~your~target`s~Magical~Defense,~you~should~have~a~character~level~of~130,~become~a~level~9~Sage~and~have~been~reborn~twice.~Have~you~met~all~these~requirements?");
                                                            data.AddOption("Positive.", 15);
                                                            data.AddOption("Oh,~not~yet.");
                                                            data.AddAvatar(87);
                                                            data.FinalizeDialog();
                                                        }
                                                        else
                                                        {
                                                            data.AddText("There~is~nothing~more~I~can~teach~you,~now~that~you`ve~reached~the~highest~phase~of~this~sub-class.~Well~done!");
                                                            data.AddOption("Oh,~it~does~not~suit~me.");
                                                            data.AddAvatar(87);
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
                        break;
                    }
                case 15:
                    {
                        if (client.Player.Level < 130)
                        {
                            data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                            data.AddOption("Oh.");
                            data.AddAvatar(87);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Sage) < 9)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(87);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 2)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(87);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Sage, 9, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~9~Sage~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~9~Sage~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(87);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 14:
                    {
                        if (client.Player.Level < 120)
                        {
                            data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                            data.AddOption("Oh.");
                            data.AddAvatar(87);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Sage) < 8)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(87);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 2)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(87);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Sage, 8, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~8~Sage~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~8~Sage~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(87);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 13:
                    {
                        if (client.Player.Reborn < 2)
                        {
                            if (client.Player.Level < 120)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(87);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Sage) < 7)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(87);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Reborn < 1)
                                    {
                                        data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                        data.AddOption("Oh.");
                                        data.AddAvatar(87);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Sage, 7, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~7~Sage~now!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("You~are~a~Phase~7~Sage~now!");
                                        data.AddOption("Great.");
                                        data.AddAvatar(87);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Sage) < 7)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(87);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 1)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(87);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Sage, 7, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~7~Sage~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~7~Sage~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(87);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 12:
                    {
                        if (client.Player.Reborn < 2)
                        {
                            if (client.Player.Level < 110)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(87);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Sage) < 6)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(87);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Reborn < 1)
                                    {
                                        data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                        data.AddOption("Oh.");
                                        data.AddAvatar(87);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Sage, 6, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~6~Sage~now!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("You~are~a~Phase~6~Sage~now!");
                                        data.AddOption("Great.");
                                        data.AddAvatar(87);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Sage) < 6)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(87);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 1)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(87);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Sage, 6, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~6~Sage~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~6~Sage~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(87);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 11:
                    {
                        if (client.Player.Reborn < 2)
                        {
                            if (client.Player.Level < 90)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(87);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Sage) < 5)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(87);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Reborn < 1)
                                    {
                                        data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                        data.AddOption("Oh.");
                                        data.AddAvatar(87);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Sage, 5, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~5~Sage~now!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("You~are~a~Phase~5~Sage~now!");
                                        data.AddOption("Great.");
                                        data.AddAvatar(87);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Sage) < 5)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(87);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 1)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(87);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Sage, 5, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~5~Sage~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~5~Sage~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(87);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 10:
                    {
                        if (client.Player.Reborn < 1)
                        {
                            if (client.Player.Level < 120)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(87);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Sage) < 4)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(87);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Sage, 4, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~4~Sage~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~4~Sage~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(87);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Sage) < 4)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(87);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Sage, 4, client, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~4~Sage~now!", MsgMessage.ChatMode.TopLeftSystem);
                                data.AddText("You~are~a~Phase~4~Sage~now!");
                                data.AddOption("Great.");
                                data.AddAvatar(87);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 9:
                    {
                        if (client.Player.Reborn < 1)
                        {
                            if (client.Player.Level < 110)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(87);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Sage) < 3)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(87);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Sage, 3, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~3~Sage~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~3~Sage~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(87);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Sage) < 3)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(87);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Sage, 3, client, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~3~Sage~now!", MsgMessage.ChatMode.TopLeftSystem);
                                data.AddText("You~are~a~Phase~3~Sage~now!");
                                data.AddOption("Great.");
                                data.AddAvatar(87);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 8:
                    {
                        if (client.Player.Reborn < 1)
                        {
                            if (client.Player.Level < 90)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(87);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Sage) < 2)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(87);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Sage, 2, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~2~Sage~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~2~Sage~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(87);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Sage) < 2)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(87);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Sage, 2, client, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~2~Sage~now!", MsgMessage.ChatMode.TopLeftSystem);
                                data.AddText("You~are~a~Phase~2~Sage~now!");
                                data.AddOption("Great.");
                                data.AddAvatar(87);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        data.AddText("Want~to~be~a~Sage?~We`re~able~to~reduce~a~target`s~Magical~Defense.~Make~sure~you`ve~reached~Level~70,~and~pay~20~Lucky~Amulets~or~800~Study~Points~to~join~us!~");
                        data.AddText("~After~being~one~of~us,~you~can~work~on~your~sub-class~training~on~the~`Status`~sheet.~Remember,~a~certain~sub-class~level~is~required~to~promote~the~phase~of~your~sub-class.");
                        data.AddOption("Pay~20~Lucky~Amulets~to~join.", 6);
                        data.AddOption("Pay~800~Study~Points~to~join.", 4);
                        data.AddOption("Oh,~it~does~not~suit~me.");
                        data.AddAvatar(87);
                        data.FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Sage) < 1)
                        {
                            if (client.Player.Reborn < 1)
                            {
                                if (client.Player.Level < 70)
                                {
                                    data.AddText("You~will~be~invited~to~join~us~after~you~reach~level~70.~Work~harder!");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(87);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Inventory.Contain(723087, 20))
                                    {
                                        if (client.Inventory.Remove(723087, 20, stream))
                                        {
                                            client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Sage, client, stream);
                                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                            Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~become~a~Sage.~Excellent!", MsgMessage.ChatMode.TopLeftSystem);
                                            data.AddText("Welcome~to~the~Sage~community!~This~is~a~big~community~and~we~have~9~phases~for~the~advancement~of~Sages.~Work~hard~in~the~future~and~when~you~are~ready~to~get~promoted,~I`m~always~here~for~you.");
                                            data.AddOption("Thanks.");
                                            data.AddAvatar(87);
                                            data.FinalizeDialog();
                                        }
                                    }
                                    else
                                    {
                                        data.AddText("Well,~I`m~sorry,~but~you~don`t~have~the~20~Lucky~Amulets~needed~to~join~us.");
                                        data.AddOption("Oh.");
                                        data.AddAvatar(87);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                            else
                            {
                                if (client.Inventory.Contain(723087, 20))
                                {
                                    if (client.Inventory.Remove(723087, 20, stream))
                                    {
                                        client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Sage, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~become~a~Sage.~Excellent!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("Welcome~to~the~Sage~community!~This~is~a~big~community~and~we~have~9~phases~for~the~advancement~of~Sages.~Work~hard~in~the~future~and~when~you~are~ready~to~get~promoted,~I`m~always~here~for~you.");
                                        data.AddOption("Thanks.");
                                        data.AddAvatar(87);
                                        data.FinalizeDialog();
                                    }
                                }
                                else
                                {
                                    data.AddText("Well,~I`m~sorry,~but~you~don`t~have~the~20~Lucky~Amulets~needed~to~join~us.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(87);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            data.AddText("Aha,~you~are~already~a~Sage,~remember?~I~suggest~you~focus~your~attention~on~upgrading~your~sub-class~level,~now.");
                            data.AddOption("Oh.");
                            data.AddAvatar(87);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 4:
                    {
                        data.AddText("~~~~Are~you~sure~you~want~to~pay~800~Study~Points~to~join~us?");
                        data.AddOption("Yes,~I`ve~decided!", 5);
                        data.AddOption("No,~I~changed~my~mind.");
                        data.AddAvatar(87);
                        data.FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Sage) < 1)
                        {
                            if (client.Player.Reborn < 1)
                            {
                                if (client.Player.Level < 70)
                                {
                                    data.AddText("You~will~be~invited~to~join~us~after~you~reach~level~70.~Work~harder!");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(87);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.SubClass.StudyPoints < 800)
                                    {
                                        data.AddText("You~don`t~have~800~Study~Points~to~take~the~sub-class.");
                                        data.AddOption("Alright.");
                                        data.AddAvatar(87);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.RemoveStudy(client, 800, stream);
                                        {
                                            client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Sage, client, stream);
                                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                            Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~become~a~Sage.~Excellent!", MsgMessage.ChatMode.TopLeftSystem);
                                            data.AddText("Welcome~to~the~Sage~community!~This~is~a~big~community~and~we~have~9~phases~for~the~advancement~of~Sages.~Work~hard~in~the~future~and~when~you~are~ready~to~get~promoted,~I`m~always~here~for~you.");
                                            data.AddOption("Thanks.");
                                            data.AddAvatar(87);
                                            data.FinalizeDialog();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (client.Player.SubClass.StudyPoints < 800)
                                {
                                    data.AddText("You~don`t~have~800~Study~Points~to~take~the~sub-class.");
                                    data.AddOption("Alright.");
                                    data.AddAvatar(87);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.RemoveStudy(client, 800, stream);
                                    {
                                        client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Sage, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~become~a~Sage.~Excellent!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("Welcome~to~the~Sage~community!~This~is~a~big~community~and~we~have~9~phases~for~the~advancement~of~Sages.~Work~hard~in~the~future~and~when~you~are~ready~to~get~promoted,~I`m~always~here~for~you.");
                                        data.AddOption("Thanks.");
                                        data.AddAvatar(87);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            data.AddText("Aha,~you~are~already~a~Sage,~remember?~I~suggest~you~focus~your~attention~on~upgrading~your~sub-class~level,~now.");
                            data.AddOption("Oh.");
                            data.AddAvatar(87);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        data.AddText("Good~day!~Please~spare~me~a~few~minutes~so~that~I~can~tell~you~about~the~sub-classes~in~Twin~City.~Each~Sub-Class~has~9~phases,~and~the~higher~the~phase~is,~the~more~bonus~effects~the~sub-class~will~offer.~But~keep~in~mind~that~these");
                        data.AddText("~sub-classes~also~have~a~level~system~and~you~need~to~level~them~to~a~certain~level~before~getting~promoted~to~some~phase.~You~can~level~up~your~sub-class~by~gaining~study~points.");
                        data.AddOption("How~can~I~get~study~points?", 2);
                        data.AddOption("I~see.");
                        data.AddAvatar(87);
                        data.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("Well,~the~only~way~to~get~those~points~is~to~read~the~sub-class~books,~such~as~the~Diligence~Book.~When~you`ve~gained~enough~study~points,~open~the~sub-class~sheet~from~the~character~panel~and");
                        data.AddText("~level~up~the~sub-class~there.~Monsters~in~the~wild~are~always~bothering~young~adventurers,~so~you~might~find~some~books~from~them.");
                        data.AddOption("I~see.~Thanks.");
                        data.AddAvatar(87);
                        data.FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ChiMaster)]
        public static void ChiMaster(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello~there!~Please~allow~me~to~introduce~to~you~the~sub-class~of~the~Chi~Master.~The~Chi~Master~is~a~potent~combatant~so~versed~in~counter~attacks,~that~they~have~a~chance~of~preventing~their~enemy~from~landing~Critical~Strikes!");
                        data.AddText("~Would~you~like~to~join~us?~After~you~join~this~sub-class,~you~can~level~it~up~and~get~promoted~to~increase~your~immunity~to~critical~strikes.");
                        data.AddOption("Yes,~I`d~like~to~join~it.", 1);
                        data.AddOption("I~want~to~get~promoted.", 5);
                        data.AddOption("How~can~I~get~promoted?", 14);
                        data.AddOption("Where~to~buy~Yin-Yang~Fruits?", 16);
                        data.AddOption("Not~my~concern.");
                        data.AddAvatar(79);
                        data.FinalizeDialog();
                        break;
                    }
                case 16:
                    {
                        data.AddText("You~may~pay~10,000~Horse~Racing~points~to~the~Horse~Race~Manager~to~buy~a~Yin~Yang~Fruit.");
                        data.AddOption("Thanks.");
                        data.AddAvatar(79);
                        data.FinalizeDialog();
                        break;
                    }
                case 14:
                    {
                        data.AddText("Good~day!~Please~spare~me~a~few~minutes~so~that~I~can~tell~you~about~the~sub-classes~in~Twin~City.~Each~Sub-Class~has~9~phases,~and~the~higher~the~phase~is,~the~more~bonus~effects~the~sub-class~will~offer.~But~keep~in~mind~that~these");
                        data.AddText("~sub-classes~also~have~a~level~system~and~you~need~to~level~them~up~before~you~can~get~promoted.~You~can~level~up~your~sub-class~by~gaining~study~points.");
                        data.AddOption("How~can~I~get~study~points?", 15);
                        data.AddOption("Oh.");
                        data.AddAvatar(79);
                        data.FinalizeDialog();
                        break;
                    }
                case 15:
                    {
                        data.AddText("Well,~the~only~way~to~get~those~points~is~to~read~the~sub-class~books,~such~as~the~Diligence~Book.~When~you`ve~gained~enough~study~points,~open~the~sub-class~sheet~from~the~character~panel~and");
                        data.AddText("~level~up~the~sub-class~there.~Monsters~in~the~wild~are~always~bothering~young~adventurers,~so~you~might~find~some~books~from~them.");
                        data.AddOption("Okay.");
                        data.AddAvatar(79);
                        data.FinalizeDialog();
                        break;
                    }

                case 5:
                    {
                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.ChiMaster) < 1)
                        {
                            data.AddText("You~are~not~a~Chi~Master~yet~and~can`t~get~promoted.~Do~you~want~to~join~the~Chi~Master~sub-class~now?");
                            data.AddOption("Yes,~I`d~like~to~join.", 1);
                            data.AddOption("Oh.~Not~now.");
                            data.AddAvatar(79);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.ChiMaster) == 1)
                            {
                                data.AddText("To~get~promoted~to~Phase~2~and~increase~your~immunity~rate~to~critical~strikes,~you~should~have~a~character~level~of~90~and~become~a~level~2~Chi~Master.~Have~you~met~all~these~requirements?");
                                data.AddText("~");
                                data.AddOption("Positive.", 6);
                                data.AddOption("Oh,~not~yet.");
                                data.AddAvatar(79);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.ChiMaster) == 2)
                                {
                                    data.AddText("To~get~promoted~to~Phase~3~and~increase~your~immunity~rate~to~critical~strikes,~you~should~have~a~character~level~of~110~and~become~a~level~3~Chi~Master.~Have~you~met~all~these~requirements?");
                                    data.AddText("~");
                                    data.AddOption("Positive.", 7);
                                    data.AddOption("Oh,~not~yet.");
                                    data.AddAvatar(79);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.ChiMaster) == 3)
                                    {
                                        data.AddText("To~get~promoted~to~Phase~4~and~increase~your~immunity~rate~to~critical~strikes,~you~should~have~a~character~level~of~120~and~become~a~level~4~Chi~Master.~Have~you~met~all~these~requirements?");
                                        data.AddText("~");
                                        data.AddOption("Positive.", 8);
                                        data.AddOption("Oh,~not~yet.");
                                        data.AddAvatar(79);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.ChiMaster) == 4)
                                        {
                                            data.AddText("To~get~promoted~to~Phase~5~and~increase~your~immunity~rate~to~critical~strikes,~you~should~have~a~character~level~of~90~,~become~a~level~5~Chi~Master~and~have~been~reborn~once.~Have~you~met~all~these~requirements?");
                                            data.AddText("~");
                                            data.AddOption("Positive.", 9);
                                            data.AddOption("Oh,~not~yet.");
                                            data.AddAvatar(79);
                                            data.FinalizeDialog();
                                        }
                                        else
                                        {
                                            if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.ChiMaster) == 5)
                                            {
                                                data.AddText("To~get~promoted~to~Phase~6~and~increase~your~immunity~rate~to~critical~strikes,~you~should~have~a~character~level~of~110~,~become~a~level~6~Chi~Master~and~have~been~reborn~once.~Have~you~met~all~these~requirements?");
                                                data.AddText("~");
                                                data.AddOption("Positive.", 10);
                                                data.AddOption("Oh,~not~yet.");
                                                data.AddAvatar(79);
                                                data.FinalizeDialog();
                                            }
                                            else
                                            {
                                                if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.ChiMaster) == 6)
                                                {
                                                    data.AddText("To~get~promoted~to~Phase~7~and~increase~your~immunity~rate~to~critical~strikes,~you~should~have~a~character~level~of~120~,~become~a~level~7~Chi~Master~and~have~been~reborn~once.~Have~you~met~all~these~requirements?");
                                                    data.AddText("~");
                                                    data.AddOption("Positive.", 11);
                                                    data.AddOption("Oh,~not~yet.");
                                                    data.AddAvatar(79);
                                                    data.FinalizeDialog();
                                                }
                                                else
                                                {
                                                    if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.ChiMaster) == 7)
                                                    {
                                                        data.AddText("To~get~promoted~to~Phase~8~and~increase~your~immunity~rate~to~critical~strikes,~you~should~have~a~character~level~of~120~,~become~a~level~8~Chi~Master~and~have~been~reborn~twice.~Have~you~met~all~these~requirements?");
                                                        data.AddText("~");
                                                        data.AddOption("Positive.", 12);
                                                        data.AddOption("Oh,~not~yet.");
                                                        data.AddAvatar(79);
                                                        data.FinalizeDialog();
                                                    }
                                                    else
                                                    {
                                                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.ChiMaster) == 8)
                                                        {
                                                            data.AddText("To~get~promoted~to~Phase~9~and~increase~your~immunity~rate~to~critical~strikes,~you~should~have~a~character~level~of~130~,~become~a~level~9~Chi~Master~and~have~been~reborn~twice.~Have~you~met~all~these~requirements?");
                                                            data.AddText("~");
                                                            data.AddOption("Positive.", 13);
                                                            data.AddOption("Oh,~not~yet.");
                                                            data.AddAvatar(79);
                                                            data.FinalizeDialog();
                                                        }
                                                        else
                                                        {
                                                            data.AddText("There~is~nothing~more~I~can~teach~you,~now~that~you`ve~reached~the~highest~phase~of~this~sub-class.~Well~done!");
                                                            data.AddOption("Oh,~it~does~not~suit~me.");
                                                            data.AddAvatar(79);
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
                        break;
                    }
                case 13:
                    {
                        if (client.Player.Level < 130)
                        {
                            data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                            data.AddOption("Oh.");
                            data.AddAvatar(79);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.ChiMaster) < 9)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(79);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 2)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(79);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.ChiMaster, 9, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~9~Chi~Master~now.", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~9~Chi~Master~now.");
                                    data.AddOption("Great.");
                                    data.AddAvatar(79);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 12:
                    {
                        if (client.Player.Level < 120)
                        {
                            data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                            data.AddOption("Oh.");
                            data.AddAvatar(79);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.ChiMaster) < 8)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(79);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 2)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(79);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.ChiMaster, 8, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~8~Chi~Master~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~8~Chi~Master~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(79);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 11:
                    {
                        if (client.Player.Reborn < 2)
                        {
                            if (client.Player.Level < 120)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(79);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.ChiMaster) < 7)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(79);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Reborn < 1)
                                    {
                                        data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                        data.AddOption("Oh.");
                                        data.AddAvatar(79);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.ChiMaster, 7, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~7~Chi~Master~now!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("You~are~a~Phase~7~Chi~Master~now!");
                                        data.AddOption("Great.");
                                        data.AddAvatar(79);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.ChiMaster) < 7)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(79);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 1)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(79);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.ChiMaster, 7, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~7~Chi~Master~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~7~Chi~Master~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(79);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 10:
                    {
                        if (client.Player.Reborn < 2)
                        {
                            if (client.Player.Level < 110)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(79);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.ChiMaster) < 6)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(79);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Reborn < 1)
                                    {
                                        data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                        data.AddOption("Oh.");
                                        data.AddAvatar(79);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.ChiMaster, 6, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~6~Chi~Master~now!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("You~are~a~Phase~6~Chi~Master~now!");
                                        data.AddOption("Great.");
                                        data.AddAvatar(79);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.ChiMaster) < 6)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(79);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 1)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(79);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.ChiMaster, 6, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~6~Chi~Master~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~6~Chi~Master~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(79);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 9:
                    {
                        if (client.Player.Reborn < 2)
                        {
                            if (client.Player.Level < 90)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(79);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.ChiMaster) < 5)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(79);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Reborn < 1)
                                    {
                                        data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                        data.AddOption("Oh.");
                                        data.AddAvatar(79);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.ChiMaster, 5, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~5~Chi~Master~now!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("You~are~a~Phase~5~Chi~Master~now!");
                                        data.AddOption("Great.");
                                        data.AddAvatar(79);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.ChiMaster) < 5)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(79);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 1)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(79);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.ChiMaster, 5, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~5~Chi~Master~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~5~Chi~Master~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(79);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 8:
                    {
                        if (client.Player.Reborn < 1)
                        {
                            if (client.Player.Level < 120)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(79);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.ChiMaster) < 4)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(79);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.ChiMaster, 4, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~4~Chi~Master~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~4~Chi~Master~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(79);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.ChiMaster) < 4)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(79);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.ChiMaster, 4, client, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~4~Chi~Master~now!", MsgMessage.ChatMode.TopLeftSystem);
                                data.AddText("You~are~a~Phase~4~Chi~Master~now!");
                                data.AddOption("Great.");
                                data.AddAvatar(79);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 7:
                    {
                        if (client.Player.Reborn < 1)
                        {
                            if (client.Player.Level < 110)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(79);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.ChiMaster) < 3)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(79);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.ChiMaster, 3, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~3~Chi~Master~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~3~Chi~Master~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(79);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.ChiMaster) < 3)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(79);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.ChiMaster, 3, client, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~3~Chi~Master~now!", MsgMessage.ChatMode.TopLeftSystem);
                                data.AddText("You~are~a~Phase~3~Chi~Master~now!");
                                data.AddOption("Great.");
                                data.AddAvatar(79);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 6:
                    {
                        if (client.Player.Reborn < 1)
                        {
                            if (client.Player.Level < 90)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(79);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.ChiMaster) < 2)
                                {
                                    data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(79);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.ChiMaster, 2, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~2~Chi~Master~now!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("You~are~a~Phase~2~Chi~Master~now!");
                                    data.AddOption("Great.");
                                    data.AddAvatar(79);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.ChiMaster) < 2)
                            {
                                data.AddText("You~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("Oh.");
                                data.AddAvatar(79);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.ChiMaster, 2, client, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~is~a~Phase~2~Chi~Master~now!", MsgMessage.ChatMode.TopLeftSystem);
                                data.AddText("You~are~a~Phase~2~Chi~Master~now!");
                                data.AddOption("Great.");
                                data.AddAvatar(79);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 1:
                    {
                        data.AddText("The~Chi~Master~focuses~on~immunity~rate~to~critical~strikes~from~enemies.~If~you`ve~reached~Level~70~and~you~want~to~join~us,~give~me~800~Study~Points~or~a~Yin-Yang~Fruit");
                        data.AddText("~that~you~can~buy~from~the~Horse~Race~Manager~at~10,000~Horse~Racing~Points.~After~that,~you~can~start~your~study~of~the~sub-class~on~the~`Status`~sheet.");
                        data.AddText("~Remember,~a~certain~sub-class~level~is~required~to~promote~the~phase~of~your~sub-class.");
                        data.AddOption("Pay~a~Yin-Yang~Fruit~to~join.", 4);
                        data.AddOption("Pay~800~Study~Points~to~join.", 2);
                        data.AddOption("Oh,~it~does~not~suit~me.");
                        data.AddAvatar(79);
                        data.FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.ChiMaster) < 1)
                        {
                            if (client.Player.Reborn < 1)
                            {
                                if (client.Player.Level < 70)
                                {
                                    data.AddText("You~will~be~invited~to~join~us~after~you~reach~level~70.~Work~harder!");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(79);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Inventory.Contain(711188, 1))
                                    {
                                        client.Inventory.Remove(711188, 1, stream);
                                        client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.ChiMaster, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~become~a~Chi~Master!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("Welcome~to~the~Chi~Master~community!~This~is~a~big~community~and~we~have~9~phases~for~the~advancement~of~Chi~Masters.~Work~hard~in~the~future~and~when~you~are~ready~to~get~promoted,~I`m~always~here~for~you.");
                                        data.AddOption("Thanks.");
                                        data.AddAvatar(79);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        data.AddText("I`m~sorry,~you~need~to~buy~a~Yin-Yang~Fruit~from~the~Horse~Race~Manager~and~reach~level~70~to~join~us.");
                                        data.AddOption("Oh.");
                                        data.AddAvatar(79);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                            else
                            {
                                if (client.Inventory.Contain(711188, 1))
                                {
                                    client.Inventory.Remove(711188, 1, stream);
                                    client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.ChiMaster, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~become~a~Chi~Master!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("Welcome~to~the~Chi~Master~community!~This~is~a~big~community~and~we~have~9~phases~for~the~advancement~of~Chi~Masters.~Work~hard~in~the~future~and~when~you~are~ready~to~get~promoted,~I`m~always~here~for~you.");
                                    data.AddOption("Thanks.");
                                    data.AddAvatar(79);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("I`m~sorry,~you~need~to~buy~a~Yin-Yang~Fruit~from~the~Horse~Race~Manager~and~reach~level~70~to~join~us.");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(79);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            data.AddText("Aha,~you~are~already~a~Chi~Master,~remember?~I~suggest~you~focus~your~attention~on~upgrading~your~sub-class~level~now.");
                            data.AddOption("Oh.");
                            data.AddAvatar(79);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        data.AddText("~~~~Are~you~sure~you~want~to~pay~800~Study~Points~to~join~us?");
                        data.AddOption("Yes,~I`ve~decided!", 3);
                        data.AddOption("No,~I~changed~my~mind.");
                        data.AddAvatar(79);
                        data.FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.ChiMaster) < 1)
                        {
                            if (client.Player.Reborn < 1)
                            {
                                if (client.Player.Level < 70)
                                {
                                    data.AddText("You~will~be~invited~to~join~us~after~you~reach~level~70.~Work~harder!");
                                    data.AddOption("Oh.");
                                    data.AddAvatar(79);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.SubClass.StudyPoints < 800)
                                    {
                                        data.AddText("You~don`t~have~800~Study~Points~to~take~the~sub-class.");
                                        data.AddOption("Alright.");
                                        data.AddAvatar(79);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.RemoveStudy(client, 800, stream);
                                        {
                                            client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.ChiMaster, client, stream);
                                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                            Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~become~a~Chi~Master!", MsgMessage.ChatMode.TopLeftSystem);
                                            data.AddText("Welcome~to~the~Chi~Master~community!~This~is~a~big~community~and~we~have~9~phases~for~the~advancement~of~Chi~Masters.~Work~hard~in~the~future~and~when~you~are~ready~to~get~promoted,~I`m~always~here~for~you.");
                                            data.AddOption("Thanks.");
                                            data.AddAvatar(79);
                                            data.FinalizeDialog();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (client.Player.SubClass.StudyPoints < 800)
                                {
                                    data.AddText("You~don`t~have~800~Study~Points~to~take~the~sub-class.");
                                    data.AddOption("Alright.");
                                    data.AddAvatar(79);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.RemoveStudy(client, 800, stream);
                                    {
                                        client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.ChiMaster, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~become~a~Chi~Master!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("Welcome~to~the~Chi~Master~community!~This~is~a~big~community~and~we~have~9~phases~for~the~advancement~of~Chi~Masters.~Work~hard~in~the~future~and~when~you~are~ready~to~get~promoted,~I`m~always~here~for~you.");
                                        data.AddOption("Thanks.");
                                        data.AddAvatar(79);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            data.AddText("Aha,~you~are~already~a~Chi~Master,~remember?~I~suggest~you~focus~your~attention~on~upgrading~your~sub-class~level~now.");
                            data.AddOption("Oh.");
                            data.AddAvatar(79);
                            data.FinalizeDialog();
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.LeadWrangler)]
        public static void LeadWrangler(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Hey,~there!~Let~me~tell~you~about~the~joys~of~the~Wrangler!~Wranglers~are~able~to~equip~special~armor~on~their~mounts.~As~their~Sub-Class~level~improves,");
                        data.AddText("~their~Max~HP~increases.~Isn`t~it~great?~Would~you~like~to~join~us?");
                        data.AddOption("Yes,~I`d~like~to~join.", 1);
                        data.AddOption("I`d~like~to~get~promoted.", 7);
                        data.AddOption("How~can~I~get~promoted?", 5);
                        data.AddOption("Not~my~concern.");
                        data.AddAvatar(233);
                        data.FinalizeDialog();
                        break;
                    }
                case 7:
                    {
                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Wrangler) < 1)
                        {
                            data.AddText("You`re~not~a~Wrangler~yet,~and~can`t~get~promoted.~Would~you~like~to~become~a~Wrangler?");
                            data.AddOption("Yes,~I`d~like~to~join.", 1);
                            data.AddOption("Not~now.");
                            data.AddAvatar(233);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Wrangler) == 1)
                            {
                                data.AddText("To~get~promoted~to~Phase~2~and~be~able~to~equip~better~mount~armor,~your~character~should~reach~Level~90~and~your~Sub-Class~reach~Level~2.");
                                data.AddText("~Have~you~met~all~these~requirements?");
                                data.AddOption("Yup!", 8);
                                data.AddOption("Nope.");
                                data.AddAvatar(233);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Wrangler) == 2)
                                {
                                    data.AddText("To~get~promoted~to~Phase~3~and~be~able~to~equip~better~mount~armor,~your~character~should~reach~Level~110~and~your~Sub-Class~at~Level~3.");
                                    data.AddText("~Have~you~met~all~these~requirements?");
                                    data.AddOption("Yup!", 9);//90028189
                                    data.AddOption("Nope.");
                                    data.AddAvatar(233);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Wrangler) == 3)
                                    {
                                        data.AddText("To~get~promoted~to~Phase~4~and~be~able~to~equip~better~mount~armor,~your~character~should~reach~Level~120~and~your~Sub-Class~at~Level~4.");
                                        data.AddText("~Have~you~met~all~these~requirements?");
                                        data.AddOption("Yup!", 10);//90028239
                                        data.AddOption("Nope.");
                                        data.AddAvatar(233);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Wrangler) == 4)
                                        {
                                            data.AddText("To~get~promoted~to~Phase~5~and~be~able~to~equip~better~mount~armor,~you~should~meet~the~following~requirements:~Be~Reborn~once,~Character~Level~90,~and~Sub-Class~Level~5.");
                                            data.AddText("~Have~you~met~all~these~requirements?");
                                            data.AddOption("Yup!", 11);
                                            data.AddOption("Nope.");
                                            data.AddAvatar(233);
                                            data.FinalizeDialog();
                                        }
                                        else
                                        {
                                            if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Wrangler) == 5)
                                            {
                                                data.AddText("To~get~promoted~to~Phase~6~and~be~able~to~equip~better~mount~armor,~you~should~meet~the~following~requirements:~Be~Reborn~once,~Character~Level~110,~and~Sub-Class~Level~6.");
                                                data.AddText("~Have~you~met~all~these~requirements?");
                                                data.AddOption("Yup!", 12);
                                                data.AddOption("Nope.");
                                                data.AddAvatar(233);
                                                data.FinalizeDialog();
                                            }
                                            else
                                            {
                                                if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Wrangler) == 6)
                                                {
                                                    data.AddText("To~get~promoted~to~Phase~7~and~be~able~to~equip~better~mount~armor,~you~should~meet~the~following~requirements:~Be~Reborn~once,~Character~Level~120,~and~Sub-Class~Level~7.");
                                                    data.AddText("~Have~you~met~all~these~requirements?");
                                                    data.AddOption("Yup!", 13);
                                                    data.AddOption("Nope.");
                                                    data.AddAvatar(233);
                                                    data.FinalizeDialog();
                                                }
                                                else
                                                {
                                                    if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Wrangler) == 7)
                                                    {
                                                        data.AddText("To~get~promoted~to~Phase~8~and~be~able~to~equip~better~mount~armor,~you~should~meet~the~following~requirements:~Be~Reborn~twice,~Character~Level~120,~and~Sub-Class~Level~8.");
                                                        data.AddText("~Have~you~met~all~these~requirements?");
                                                        data.AddOption("Yup!", 14);
                                                        data.AddOption("Nope.");
                                                        data.AddAvatar(233);
                                                        data.FinalizeDialog();
                                                    }
                                                    else
                                                    {
                                                        if (client.Player.SubClass.GetClassPhrase(Database.DBLevExp.Sort.Wrangler) == 8)
                                                        {
                                                            data.AddText("To~get~promoted~to~Phase~9~and~be~able~to~equip~better~mount~armor,~you~should~meet~the~following~requirements:~Be~Reborn~twice,~Character~Level~130,~and~Sub-Class~Level~9.");
                                                            data.AddText("~Have~you~met~all~these~requirements?");
                                                            data.AddOption("Yup!", 15);
                                                            data.AddOption("Nope.");
                                                            data.AddAvatar(233);
                                                            data.FinalizeDialog();
                                                        }
                                                        else
                                                        {
                                                            data.AddText("You`ve~reached~the~highest~phase!~There~is~nothing~else~I~have~to~teach~you.~Go~forth~and~make~us~proud!");
                                                            data.AddOption("Bye.");
                                                            data.AddAvatar(233);
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
                        break;
                    }
                case 15:
                    {
                        if (client.Player.Level < 130)
                        {
                            data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                            data.AddOption("I~see.");
                            data.AddAvatar(233);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Wrangler) < 9)
                            {
                                data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("I~see.");
                                data.AddAvatar(233);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 2)
                                {
                                    data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("I~see.");

                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Wrangler, 9, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~become~a~P9~Wrangler!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("Congratulations!~You`ve~been~promoted~to~a~P9~Wrangler!");
                                    data.AddOption("Thanks.");
                                    data.AddAvatar(233);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 14:
                    {
                        if (client.Player.Level < 120)
                        {
                            data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                            data.AddOption("I~see.");
                            data.AddAvatar(233);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Wrangler) < 8)
                            {
                                data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("I~see.");
                                data.AddAvatar(233);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 2)
                                {
                                    data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("I~see.");
                                    data.AddAvatar(233);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Wrangler, 8, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~become~a~P8~Wrangler!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("Congratulations!~You`ve~been~promoted~to~P8~Wrangler!");
                                    data.AddOption("Thanks.");
                                    data.AddAvatar(233);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 13:
                    {
                        if (client.Player.Reborn < 2)
                        {
                            if (client.Player.Level < 110)
                            {
                                data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("I~see.");
                                data.AddAvatar(233);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Wrangler) < 7)
                                {
                                    data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("I~see.");
                                    data.AddAvatar(233);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Reborn < 1)
                                    {
                                        data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                        data.AddOption("I~see.");
                                        data.AddAvatar(233);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Wrangler, 7, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~become~a~P7~Wrangler!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("Congratulations!~You`ve~been~promoted~to~P7~Wrangler!");
                                        data.AddOption("Thanks.");
                                        data.AddAvatar(233);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Wrangler) < 7)
                            {
                                data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("I~see.");
                                data.AddAvatar(233);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 1)
                                {
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Wrangler, 7, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~become~a~P7~Wrangler!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("Congratulations!~You`ve~been~promoted~to~P7~Wrangler!");
                                    data.AddOption("Thanks.");
                                    data.AddAvatar(233);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 12:
                    {
                        if (client.Player.Reborn < 2)
                        {
                            if (client.Player.Level < 110)
                            {
                                data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("I~see.");
                                data.AddAvatar(233);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Wrangler) < 6)
                                {
                                    data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("I~see.");
                                    data.AddAvatar(233);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Reborn < 1)
                                    {
                                        data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                        data.AddOption("I~see.");
                                        data.AddAvatar(233);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Wrangler, 6, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~become~a~P6~Wrangler!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("Congratulations!~You`ve~been~promoted~to~a~P6~Wrangler!");
                                        data.AddOption("Thanks.");
                                        data.AddAvatar(233);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Wrangler) < 6)
                            {
                                data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("I~see.");
                                data.AddAvatar(233);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 1)
                                {
                                    data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("I~see.");
                                    data.AddAvatar(233);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Wrangler, 6, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~become~a~P6~Wrangler!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("Congratulations!~You`ve~been~promoted~to~a~P6~Wrangler!");
                                    data.AddOption("Thanks.");
                                    data.AddAvatar(233);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 11:
                    {
                        if (client.Player.Reborn < 2)
                        {
                            if (client.Player.Level < 90)
                            {
                                data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("I~see.");
                                data.AddAvatar(233);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Wrangler) < 5)
                                {
                                    data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("I~see.");
                                    data.AddAvatar(233);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Reborn < 1)
                                    {
                                        data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                        data.AddOption("I~see.");
                                        data.AddAvatar(233);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Wrangler, 5, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~become~a~P5~Wrangler!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("Congratulations!~You`ve~been~promoted~to~a~P5~Wrangler!");
                                        data.AddOption("Thanks.");
                                        data.AddAvatar(233);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Wrangler) < 5)
                            {
                                data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("I~see.");
                                data.AddAvatar(233);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Reborn < 1)
                                {
                                    data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("I~see.");
                                    data.AddAvatar(233);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Wrangler, 5, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~become~a~P5~Wrangler!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("Congratulations!~You`ve~been~promoted~to~a~P5~Wrangler!");
                                    data.AddOption("Thanks.");
                                    data.AddAvatar(233);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 10:
                    {
                        if (client.Player.Reborn < 1)
                        {
                            if (client.Player.Level < 120)
                            {
                                data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("I~see.");
                                data.AddAvatar(233);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Wrangler) < 4)
                                {
                                    data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("I~see.");
                                    data.AddAvatar(233);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Wrangler, 4, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~become~a~P4~Wrangler!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("Congratulations!~You`ve~been~promoted~to~a~P4~Wrangler!");
                                    data.AddOption("Thanks.");
                                    data.AddAvatar(233);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Wrangler) < 4)
                            {
                                data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("I~see.");
                                data.AddAvatar(233);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Wrangler, 4, client, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~become~a~P4~Wrangler!", MsgMessage.ChatMode.TopLeftSystem);
                                data.AddText("Congratulations!~You`ve~been~promoted~to~a~P4~Wrangler!");
                                data.AddOption("Thanks.");
                                data.AddAvatar(233);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 9:
                    {
                        if (client.Player.Reborn < 1)
                        {
                            if (client.Player.Level < 110)
                            {
                                data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("I~see.");
                                data.AddAvatar(233);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Wrangler) < 3)
                                {
                                    data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("I~see.");
                                    data.AddAvatar(233);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Wrangler, 3, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~become~a~P3~Wrangler!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("Congratulations!~You`ve~been~promoted~to~a~P3~Wrangler!");
                                    data.AddOption("Thanks.");
                                    data.AddAvatar(233);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Wrangler) < 3)
                            {
                                data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("I~see.");
                                data.AddAvatar(233);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Wrangler, 3, client, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~become~a~P3~Wrangler!", MsgMessage.ChatMode.TopLeftSystem);
                                data.AddText("Congratulations!~You`ve~been~promoted~to~a~P3~Wrangler!");
                                data.AddOption("Thanks.");
                                data.AddAvatar(233);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 8:
                    {
                        if (client.Player.Reborn < 1)
                        {
                            if (client.Player.Level < 90)
                            {
                                data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("I~see.");
                                data.AddAvatar(233);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Wrangler) < 2)
                                {
                                    data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                    data.AddOption("I~see.");
                                    data.AddAvatar(233);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Wrangler, 2, client, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                    Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~become~a~P2~Wrangler!", MsgMessage.ChatMode.TopLeftSystem);
                                    data.AddText("Congratulations!~You`ve~been~promoted~to~a~P2~Wrangler!");
                                    data.AddOption("Thanks.");
                                    data.AddAvatar(233);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Wrangler) < 2)
                            {
                                data.AddText("I`m~sorry,~but~you~haven`t~met~all~the~requirements~for~promotion~yet.");
                                data.AddOption("I~see.");
                                data.AddAvatar(233);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Wrangler, 2, client, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~become~a~P2~Wrangler!", MsgMessage.ChatMode.TopLeftSystem);
                                data.AddText("Congratulations!~You`ve~been~promoted~to~a~P2~Wrangler!");
                                data.AddOption("Thanks.");
                                data.AddAvatar(233);
                                data.FinalizeDialog();
                            }

                        }
                        break;
                    }
                case 5:
                    {
                        data.AddText("Each~Sub-Class~has~9~Phases.~The~higher~the~phase~is,~the~more~bonus~effects~you~will~get.");
                        data.AddText("~But~keep~in~mind~that~these~Sub-Classes~also~have~a~level~system,~and~you~need~to~level~them~up~before~you~can~get~promoted.~You~can~level~up~your~Sub-Class~by~gaining~Study~Points.");
                        data.AddOption("How~can~I~get~Study~Points?", 6);
                        data.AddOption("I~see.");
                        data.AddAvatar(233);
                        data.FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        data.AddText("Well,~the~only~way~to~get~those~points~is~to~read~the~Sub-Class~books,~such~as~the~Diligence~Book.~When~you`ve~gained~enough~Study~Points,");
                        data.AddText("~open~the~Sub-Class~sheet~from~the~character~panel~and~level~up~the~Sub-Class,~there.~Monsters~in~the~wild~are~always~bothering~young~adventurers,");
                        data.AddText("~so~you~might~find~some~books~after~you~kill~them.");
                        data.AddOption("Got~it.");
                        data.AddAvatar(233);
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        data.AddText("Wranglers~are~able~to~equip~special~armor~on~their~mounts.~If~you`ve~reached~Level~40,~you~can~pay~40~Saddles~or~800~Study~Points~to~join~us.~");
                        data.AddText("~After~being~a~Wrangler,~you~can~enhance~your~sub-class~skill~on~`Status`~sheet.~Remember,~a~certain~sub-class~level~is~required~to~promote~the~phase~of~your~sub-class.");
                        data.AddOption("Pay~40~Saddles~to~join.", 2);
                        data.AddOption("Pay~800~Study~Points~to~join.", 3);
                        data.AddOption("I~see.");
                        data.AddAvatar(233);
                        data.FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        data.AddText("~~~~Are~you~sure~you~want~to~pay~800~Study~Points~to~join~us?");
                        data.AddOption("Yes,~I`ve~decided!", 4);
                        data.AddOption("No,~I~changed~my~mind.");
                        data.AddAvatar(233);
                        data.FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Wrangler) < 1)
                        {
                            if (client.Player.Reborn < 1)
                            {
                                if (client.Player.Level < 70)
                                {
                                    data.AddText("I`m~sorry,~but~you~have~to~reach~at~least~Level~70~to~join~our~fine~group~of~adventurers.");
                                    data.AddOption("Alright.");
                                    data.AddAvatar(233);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.SubClass.StudyPoints < 800)
                                    {
                                        data.AddText("You~don`t~have~800~Study~Points~to~take~the~sub-class.");
                                        data.AddOption("Alright.");
                                        data.AddAvatar(233);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.SubClass.RemoveStudy(client, 800, stream);

                                        client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Wrangler, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~joined~up~with~the~Wranglers!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("Each~Sub-Class~has~9~phases.~If~you~want~to~get~promoted~one~day,~you`ll~come~talk~to~me.");
                                        data.AddOption("I~will.");
                                        data.AddAvatar(233);
                                        data.FinalizeDialog();

                                    }
                                }
                            }
                            else
                            {
                                if (client.Player.SubClass.StudyPoints < 800)
                                {
                                    data.AddText("You~don`t~have~800~Study~Points~to~take~the~sub-class.");
                                    data.AddOption("Alright.");
                                    data.AddAvatar(233);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.SubClass.RemoveStudy(client, 800, stream);
                                    {
                                        client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Wrangler, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~joined~up~with~the~Wranglers!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("Each~Sub-Class~has~9~phases.~If~you~want~to~get~promoted~one~day,~you`ll~come~talk~to~me.");
                                        data.AddOption("I~will.");
                                        data.AddAvatar(233);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            data.AddText("Aha,~you~are~already~a~Wrangler,~remember?~I~suggest~you~focus~your~attention~on~upgrading~your~standing~with~us!");
                            data.AddOption("Thanks~for~reminding~me.");
                            data.AddAvatar(233);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Player.SubClass.GetClassLevel(Database.DBLevExp.Sort.Wrangler) < 1)
                        {
                            if (client.Player.Reborn < 1)
                            {
                                if (client.Player.Level < 70)
                                {
                                    data.AddText("I`m~sorry,~but~you~have~to~reach~at~least~Level~70~to~join~our~fine~group~of~adventurers.");
                                    data.AddOption("Alright.");
                                    data.AddAvatar(233);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Inventory.Contain(723903, 40))
                                    {
                                        if (client.Inventory.Remove(723903, 40, stream))
                                        {
                                            client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Wrangler, client, stream);
                                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                            Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~joined~up~with~the~Wranglers!", MsgMessage.ChatMode.TopLeftSystem);
                                            data.AddText("Each~Sub-Class~has~9~phases.~If~you~want~to~get~promoted~one~day,~you`ll~come~talk~to~me.");
                                            data.AddOption("I~will.");
                                            data.AddAvatar(233);
                                            data.FinalizeDialog();
                                        }
                                    }
                                    else
                                    {
                                        data.AddText("Sorry,~but~I~don`t~see~40~Saddles~in~your~inventory.~You~should~bring~them~with~you,~next~time.");
                                        data.AddOption("OK.");
                                        data.AddAvatar(233);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                            else
                            {
                                if (client.Inventory.Contain(723903, 40))
                                {
                                    if (client.Inventory.Remove(723903, 40, stream))
                                    {
                                        client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Wrangler, client, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                                        Role.Core.SendGlobalMessage(stream, "" + client.Player.Name + "~has~joined~up~with~the~Wranglers!", MsgMessage.ChatMode.TopLeftSystem);
                                        data.AddText("Each~Sub-Class~has~9~phases.~If~you~want~to~get~promoted~one~day,~you`ll~come~talk~to~me.");
                                        data.AddOption("I~will.");
                                        data.AddAvatar(233);
                                        data.FinalizeDialog();
                                    }
                                }
                                else
                                {
                                    data.AddText("Sorry,~but~I~don`t~see~40~Saddles~in~your~inventory.~You~should~bring~them~with~you,~next~time.");
                                    data.AddOption("OK.");
                                    data.AddAvatar(233);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            data.AddText("Aha,~you~are~already~a~Wrangler,~remember?~I~suggest~you~focus~your~attention~on~upgrading~your~standing~with~us!");
                            data.AddOption("Thanks~for~reminding~me.");
                            data.AddAvatar(233);
                            data.FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.SubClassManager)]
        public static void SubClassManager(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("In addition to the existing 7 classes, there are 7 sub-classes for you to choose, which will grant you many bonus effects.");
                        data.AddText(" The 7 sub-classes are Apothecary, Chi Master, Martial Artist, Warlock, Sage, Performer, Lead Wrangler. Do you want to choose a sub-class to");
                        data.AddText("~enhance the strength of your character?");
                        data.AddOption("Sub-Class~Hall", 1);
                        data.AddOption("See~you~later.");
                        data.AddAvatar(76);
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Teleport(51, 71, 6072);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ArenaGuard)]
        public static void ArenaGuard(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("A suited weapon is a thing that you can`t do without nowadays. I keep faith in it.");
                        data.AddOption("Attractive.");
                        data.AddAvatar(94);
                        data.FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.IronsmithChou)]
        public static void IronsmithChou(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddOption("Sub-Class~Hall", 1);
                        data.AddAvatar(85);
                        data.FinalizeDialog();
                        break;
                    }
            }
        }


        [NpcAttribute(NpcID.BirdExorcist)]
        public static void BirdExorcist(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Sigh, I should have felt the danger earlier. The Ancient Devil sealed away a thousand years ago is gonna wake up! I`m afraid a war is inevitable.");
                        data.AddOption("What~can~I~do?", 1);
                        data.AddOption("Goodness!~I`d~better~flee!");
                        data.AddAvatar(87);
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        data.AddText("Bring your people together, and collect amulets of 7 classes by completing the given tests in the Ancient Island. If you have Ninjas, Monks, Saints or Pirates in your team,");
                        data.AddText("~they`ll get the amulets after finishing the test of any classes.");
                        data.AddOption("Okay,~what~to~do~next?", 2);
                        data.AddAvatar(87);
                        data.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("If your team holds a Monk Amulet, use that amulet to combine all the other amulets. If not, ask any one in your team to take all the 7 amulets and turn on a shining yellow switch");
                        data.AddText("~to bring out the Ancient Devil. Kill the devil before its power fully recovered! So, what do you say?");
                        data.AddOption("I~love~challenge!", 3);
                        data.AddOption("I~need~a~break!");
                        data.AddAvatar(87);
                        data.FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        if (client.Player.Level < 50)
                        {
                            client.CreateBoxDialog("You~should~be~Level~50~or~above~to~take~this~quest.");
                        }
                        else
                        {
                            client.Teleport(191, 232, 1052);
                        }
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.CommanderKerry)]
        public static void CommanderKerry(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.Felix, client.Player.Class, 1825);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GuideZhang, client.Player.Class, 1826);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("My friend. Can you tell Felix that I miss him so much?");
                            data.AddOption("Yep.~I~will.", 1);
                            data.AddOption("I`m~just~passing~by.");
                            data.AddAvatar(43);
                            data.FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                    && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted) == false)
                            {
                                data.AddText("My friend. Can you tell Felix that I miss him so much?");
                                data.AddOption("Yep.~I~will.", 20);
                                data.AddOption("I`m~just~passing~by.");
                                data.AddAvatar(43);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("My friend. Can you tell Felix that I miss him so much?");
                                data.AddOption("Yep.~I~will.", 30);
                                data.AddOption("I`m~just~passing~by.");
                                data.AddAvatar(43);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("My friend. Can you tell Felix that I miss him so much?");
                            data.AddOption("Yep.~I~will.", 1);//1273300
                            data.AddOption("I`m~just~passing~by.");
                            data.AddAvatar(43);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 30:
                    {
                        data.AddText("My days are numbered...will you tell him that I am really sorry, and I miss him very much? Will you?");
                        data.AddOption("OK.", 12);
                        data.AddAvatar(43);
                        data.FinalizeDialog();
                        break;
                    }
                case 20:
                    {
                        data.AddText("My days are numbered...will you tell him that I am really sorry, and I miss him very much? Will you?");
                        data.AddOption("OK.", 12);
                        data.AddAvatar(43);
                        data.FinalizeDialog();
                        break;
                    }

                case 1:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1, 5))
                        {
                            data.AddText("I don`t even know who you are. Why do you want to kill me?");
                            data.AddOption("For~Felix!", 4);
                            data.AddAvatar(43);
                            data.FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1, 4))
                        {
                            data.AddText("Be careful and be quick...Step into 1 of the eyes of the Maze Puzzle in 10 seconds and I will send you out to Felix...");
                            data.AddOption("I`ll~try.", 10);
                            data.AddAvatar(43);
                            data.FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1, 3))
                        {
                            data.AddText("Be careful and be quick...Step into 2 of the eyes of the Maze Puzzle in 10 seconds and I will send you out to Felix...");
                            data.AddOption("I`ll~try.", 10);
                            data.AddAvatar(43);
                            data.FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1, 2))
                        {
                            data.AddText("Be careful and be quick...Step into 3 of the eyes of the Maze Puzzle in 10 seconds and I will send you out to Felix...");
                            data.AddOption("I`ll~try.", 10);
                            data.AddAvatar(43);
                            data.FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1, 1))
                        {
                            data.AddText("Be careful and be quick...Step into 4 of the eyes of the Maze Puzzle in 10 seconds and I will send you out to Felix...");
                            data.AddOption("I`ll~try.", 10);
                            data.AddAvatar(43);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("What did you come for, young hero?");
                            data.AddOption("For~your~life.", 2);
                            data.AddAvatar(43);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 10:
                    {
                        client.CreateBoxDialog("Get ready for the Maze Puzzle.");
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 5);
                        break;
                    }
                case 4:
                    {
                        data.AddText("Oh, Felix! My son! He is still alive. I miss him so much!");
                        data.AddOption("What?", 5);
                        data.AddAvatar(43);
                        data.FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        data.AddText("It is a long story. Many years ago, I mistakenly killed Felix`s birth parents, that is why I took to raise him up. Felix finally got to know that it was me,");
                        data.AddText("~and he will never forgive me. I know that.");
                        data.AddOption("I~see.", 6);
                        data.AddAvatar(43);
                        data.FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        data.AddText("My days are numbered...will you tell him that I am really sorry, and I miss him very much? Will you?");
                        data.AddOption("OK.", 11);
                        data.AddAvatar(43);
                        data.FinalizeDialog();//finish quest here!!
                        break;
                    }
                case 11:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Inventory.Remove(710567, 1, stream);
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                            client.CreateBoxDialog("You~received~100~Silver.");

                            data.AddText("My days are numbered...will you tell him that I am really sorry, and I miss him very much? Will you?");
                            data.AddOption("OK.", 12);
                            data.AddAvatar(43);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 12:
                    {
                        client.Teleport(671, 782, 1015);
                        client.CreateBoxDialog("You~have~been~teleported~out~of~the~Bandit~Chamber.~Now~tell~Felix~the~whole~story.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 3:
                    {
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 1);
                        client.CreateBoxDialog("Step into 5 of the eyes of the Maze Puzzle within 10 seconds and break the Maze Puzzle.");
                        break;
                    }
                case 2:
                    {
                        data.AddText("For my life? What makes you so cocky...Hey, see what, if you could step into 5 of the eyes of the Maze Puzzle within 10 seconds");
                        data.AddText("~I will give you a chance.");
                        data.AddOption("Interesting.", 3);
                        data.AddAvatar(43);
                        data.FinalizeDialog();
                        break;
                    }
            }
        }


        [NpcAttribute(NpcID.GuideZhang)]
        public static void GuideZhang(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.Felix, client.Player.Class, 1825);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GuideZhang, client.Player.Class, 1826);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Sorry, Commander Kerry is busy, and you don`t have the Golden Token. You`d better leave.");
                            data.AddOption("Hey,~it`s~an~important~thing.", 1);
                            data.AddOption("OK.");
                            data.AddAvatar(13);
                            data.FinalizeDialog();

                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                      && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("Sorry, Commander Kerry is busy, and you don`t have the Golden Token. You`d better leave.");
                            data.AddOption("OK.");
                            data.AddAvatar(13);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Sorry, Commander Kerry is busy, and you don`t have the Golden Token. You`d better leave.");
                            data.AddOption("OK.");
                            data.AddAvatar(13);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        if (client.Player.OnTransform)
                        {
                            if (client.Inventory.Contain(710567, 1))
                            {
                                data.AddText("What? You have the Golden Token. I will teleport you to Commander Kerry, now.");
                                data.AddOption("OK.", 2);
                                data.AddAvatar(13);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, Commander Kerry is busy, and you don`t have the Golden Token. You`d better leave.");
                                data.AddOption("OK.");
                                data.AddAvatar(13);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            if (client.Inventory.Contain(710567, 1))
                            {
                                data.AddText("Though you have the Golden Token, you are not a bandit. I can`t teleport you to Commander Kerry.");
                                data.AddOption("I~see.");
                                data.AddAvatar(13);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, Commander Kerry is busy, and you don`t have the Golden Token. You`d better leave.");
                                data.AddOption("OK.");
                                data.AddAvatar(13);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(710567, 1))
                        {
                            client.Teleport(78, 57, 1818);
                            client.CreateBoxDialog("You were teleported to the Bandit Chamber.");
                            client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                        }
                        else
                        {
                            data.AddText("Sorry, Commander Kerry is busy, and you don`t have the Golden Token. You`d better leave.");
                            data.AddOption("OK.");
                            data.AddAvatar(13);
                            data.FinalizeDialog();
                        }

                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.Felix)]
        public static void Felix(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1819);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Felix, client.Player.Class, 1820);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Felix, client.Player.Class, 1821);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Felix, client.Player.Class, 1822);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Felix, client.Player.Class, 1823);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Felix, client.Player.Class, 1824);
            var ActiveQuest7 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Felix, client.Player.Class, 1825);
            var ActiveQuest8 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GuideZhang, client.Player.Class, 1826);
            var ActiveQuest9 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Felix, client.Player.Class, 1827);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Hero, I need yor help! Years ago, my parents were killed by Commander Kerry. I have to avenge my parents,");
                            data.AddText("~but Commander Kerry is so powerful that I can`t defeat him by myself. Can you help me?");
                            data.AddOption("Sure.~I~can~do~it.", 1);
                            data.AddOption("It`s~none~of~my~business.");
                            data.AddAvatar(117);
                            data.FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                        && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted) == false)
                            {
                                data.AddText("Hero, I need yor help! Years ago, my parents were killed by Commander Kerry. I have to avenge my parents,");
                                data.AddText("~but Commander Kerry is so powerful that I can`t defeat him by myself. Can you help me?");
                                data.AddOption("Sure.~I~can~do~it.", 4);
                                data.AddOption("It`s~none~of~my~business.");
                                data.AddAvatar(117);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Hero, I need yor help! Years ago, my parents were killed by Commander Kerry. I have to avenge my parents,");
                                data.AddText("~but Commander Kerry is so powerful that I can`t defeat him by myself. Can you help me?");
                                data.AddOption("Sure.~I~can~do~it.", 5);
                                data.AddOption("It`s~none~of~my~business.");
                                data.AddAvatar(117);
                                data.FinalizeDialog();
                            }

                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                   && client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted) == false)
                            {
                                data.AddText("Hero, I need yor help! Years ago, my parents were killed by Commander Kerry. I have to avenge my parents,");
                                data.AddText("~but Commander Kerry is so powerful that I can`t defeat him by myself. Can you help me?");
                                data.AddOption("Sure.~I~can~do~it.", 8);
                                data.AddOption("It`s~none~of~my~business.");
                                data.AddAvatar(117);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Hero, I need yor help! Years ago, my parents were killed by Commander Kerry. I have to avenge my parents,");
                                data.AddText("~but Commander Kerry is so powerful that I can`t defeat him by myself. Can you help me?");
                                data.AddOption("Sure.~I~can~do~it.", 9);
                                data.AddOption("It`s~none~of~my~business.");
                                data.AddAvatar(117);
                                data.FinalizeDialog();
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
             && client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted) == false)
                            {
                                data.AddText("Hero, I need yor help! Years ago, my parents were killed by Commander Kerry. I have to avenge my parents,");
                                data.AddText("~but Commander Kerry is so powerful that I can`t defeat him by myself. Can you help me?");
                                data.AddOption("Sure.~I~can~do~it.", 12);
                                data.AddOption("It`s~none~of~my~business.");
                                data.AddAvatar(117);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Hero, I need yor help! Years ago, my parents were killed by Commander Kerry. I have to avenge my parents,");
                                data.AddText("~but Commander Kerry is so powerful that I can`t defeat him by myself. Can you help me?");
                                data.AddOption("Sure.~I~can~do~it.", 13);
                                data.AddOption("It`s~none~of~my~business.");
                                data.AddAvatar(117);
                                data.FinalizeDialog();
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
         && client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted) == false)
                            {
                                data.AddText("Hero, I need yor help! Years ago, my parents were killed by Commander Kerry. I have to avenge my parents,");
                                data.AddText("~but Commander Kerry is so powerful that I can`t defeat him by myself. Can you help me?");
                                data.AddOption("Sure.~I~can~do~it.", 16);
                                data.AddOption("It`s~none~of~my~business.");
                                data.AddAvatar(117);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Hero, I need yor help! Years ago, my parents were killed by Commander Kerry. I have to avenge my parents,");
                                data.AddText("~but Commander Kerry is so powerful that I can`t defeat him by myself. Can you help me?");
                                data.AddOption("Sure.~I~can~do~it.", 17);
                                data.AddOption("It`s~none~of~my~business.");
                                data.AddAvatar(117);
                                data.FinalizeDialog();
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
   && client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted) == false)
                            {
                                data.AddText("Hero, I need yor help! Years ago, my parents were killed by Commander Kerry. I have to avenge my parents,");
                                data.AddText("~but Commander Kerry is so powerful that I can`t defeat him by myself. Can you help me?");
                                data.AddOption("Sure.~I~can~do~it.", 20);
                                data.AddOption("It`s~none~of~my~business.");
                                data.AddAvatar(117);
                                data.FinalizeDialog();
                            }

                            else
                            {
                                data.AddText("Hero, I need yor help! Years ago, my parents were killed by Commander Kerry. I have to avenge my parents,");
                                data.AddText("~but Commander Kerry is so powerful that I can`t defeat him by myself. Can you help me?");
                                data.AddOption("Sure.~I~can~do~it.", 21);
                                data.AddOption("It`s~none~of~my~business.");
                                data.AddAvatar(117);
                                data.FinalizeDialog();
                            }

                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
&& client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted) == false)
                            {
                                data.AddText("Hero, I need yor help! Years ago, my parents were killed by Commander Kerry. I have to avenge my parents,");
                                data.AddText("~but Commander Kerry is so powerful that I can`t defeat him by myself. Can you help me?");
                                data.AddOption("Sure.", 24);
                                data.AddOption("It`s~none~of~my~business.");
                                data.AddAvatar(117);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Hero, I need yor help! Years ago, my parents were killed by Commander Kerry. I have to avenge my parents,");
                                data.AddText("~but Commander Kerry is so powerful that I can`t defeat him by myself. Can you help me?");
                                data.AddOption("Sure.~I~can~do~it.", 25);
                                data.AddOption("It`s~none~of~my~business.");
                                data.AddAvatar(117);
                                data.FinalizeDialog();
                            }

                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
&& client.Player.QuestGUI.CheckQuest(ActiveQuest8.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest8.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("Hero, I need yor help! Years ago, my parents were killed by Commander Kerry. I have to avenge my parents,");
                                data.AddText("~but Commander Kerry is so powerful that I can`t defeat him by myself. Can you help me?");
                                data.AddOption("Sure.~I~can~do~it.", 30);
                                data.AddOption("It`s~none~of~my~business.");
                                data.AddAvatar(117);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Hero, I need yor help! Years ago, my parents were killed by Commander Kerry. I have to avenge my parents,");
                                data.AddText("~but Commander Kerry is so powerful that I can`t defeat him by myself. Can you help me?");
                                data.AddOption("It`s~none~of~my~business.");
                                data.AddAvatar(117);
                                data.FinalizeDialog();
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest8.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
&& client.Player.QuestGUI.CheckQuest(ActiveQuest9.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest9.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("Hero, I need yor help! Years ago, my parents were killed by Commander Kerry. I have to avenge my parents,");
                                data.AddText("~but Commander Kerry is so powerful that I can`t defeat him by myself. Can you help me?");
                                data.AddOption("Sure.~I~can~do~it.", 33);
                                data.AddOption("It`s~none~of~my~business.");
                                data.AddAvatar(117);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.CreateBoxDialog("You~are~heading~for~Taskmaster~Chang.");
                            }
                        }
                        else
                        {
                            data.AddText("Hero, I need yor help! Years ago, my parents were killed by Commander Kerry. I have to avenge my parents,");
                            data.AddText("~but Commander Kerry is so powerful that I can`t defeat him by myself. Can you help me?");
                            data.AddOption("It`s~none~of~my~business.");
                            data.AddAvatar(117);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 33:
                    {
                        data.AddText("I don`t know. I need some time to think about it. It just happened so suddenly that I`m totally confused now.");
                        data.AddText("~Thanks, you really helped me a lot. Now you can claim your rewards from Taskmaster Chang.");
                        data.AddOption("OK.", 32);
                        data.AddAvatar(117);
                        data.FinalizeDialog();
                        break;
                    }
                case 30:
                    {
                        data.AddText("Have I done all the things wrong? Commander Kerry really treated me well. Maybe there is no right or wrong in the world.");
                        data.AddOption("Do~you~want~to~go~back?", 31);
                        data.AddAvatar(117);
                        data.FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest8.MissionId))
                        {
                            client.Player.Money += 6000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.GainExpBall(100);
                            client.SendSysMesage("You received 1 hour EXP and 6,000 Silver!");
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "recovery");

                            data.AddText("I don`t know. I need some time to think about it. It just happened so suddenly that I`m totally confused now.");
                            data.AddText("~Thanks, you really helped me a lot. Now you can claim your rewards from Taskmaster Chang.");
                            data.AddOption("OK.", 32);
                            data.AddAvatar(117);
                            data.FinalizeDialog();
                        }

                        break;
                    }
                case 32:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest9, 0);
                        client.CreateBoxDialog("You~are~heading~for~Taskmaster~Chang.");
                        break;
                    }
                case 25:
                    {
                        data.AddText("Great! You`ve collected all 5 tokens! Now I will compose them into 1 token. It is called the Token. All the Bandits will respect you once them see the token.");
                        data.AddText("~I will dress you just like a Bandit. Then you can meet with Guide Zhang, and he can teleport you to the Bandit Chamber. But you will");
                        data.AddText("~turn back after 2 minutes.");
                        data.AddOption("I~see.", 26);
                        data.AddOption("I~lost~the~token.", 27);
                        data.AddAvatar(117);
                        data.FinalizeDialog();
                        break;
                    }
                case 27:
                    {
                        if (client.Inventory.Contain(710567, 1))
                        {
                            data.AddText("Sorry, but you`ve had the token in your inventory.");
                            data.AddOption("I~forgot.");
                            data.AddAvatar(117);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("The token is very important. You should take good care of it.");
                            data.AddOption("Sorry.", 28);
                            data.AddAvatar(117);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 28:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Add(stream, 710567);
                            client.CreateBoxDialog("You received a Golden Token!");
                        }
                        else
                        {
                            client.CreateBoxDialog("Your inventory is full. You`ll need to make some room, first.");
                        }
                        break;
                    }
                case 26://trans
                    {
                        if (client.Player.OnTransform)
                        {
                            data.AddText("Guide Zhang (Bird Island, 636, 656) can teleport you to Commander Kerry in the Bandit Chamber.");
                            data.AddOption("I~see.");
                            data.AddAvatar(117);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            client.Player.TransformInfo = new Role.ClientTransform(client.Player);
                            client.Player.TransformInfo.CreateTransform(stream, (uint)client.Player.HitPoints, 201, (int)90, 8213);
                            data.AddText("Now you look just like a bandit. You can find Guide Zhang (Bird Island, 636, 656) to teleport you to the Bandit Chamber. But the disguise will only last for 2 minutes.");
                            data.AddOption("OK.");
                            data.AddAvatar(117);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 24:
                    {

                        data.AddText("I will dress you just like a bandit. Then you can meet with Guide Zhang, and he can teleport you to the Bandit Chamber. But you will");
                        data.AddText("~turn back after 90 seonds.");
                        data.AddOption("I~see.", 23);
                        data.AddAvatar(117);
                        data.FinalizeDialog();
                        break;
                    }
                case 23:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (!client.Inventory.Contain(710567, 1))
                            {
                                client.Player.TransformInfo = new Role.ClientTransform(client.Player);
                                client.Player.TransformInfo.CreateTransform(stream, (uint)client.Player.HitPoints, 201, (int)90, 8213);
                                client.Player.QuestGUI.Accept(ActiveQuest7, 0);
                                client.CreateBoxDialog("You~received~a~Golden~Token~and~were~disguised~as~a~bandit,~and~it~will~only~last~for~2~minutes.");
                                client.Inventory.Add(stream, 710567);
                            }
                            else
                            {
                                client.CreateBoxDialog("You~received~a~Golden~Token~and~were~disguised~as~a~bandit,~and~it~will~only~last~for~2~minutes.");
                            }
                        }
                        else
                        {
                            data.AddText("You inventory is full. You`ll need to make some room, first.");
                            data.AddOption("OK.");
                            data.AddAvatar(117);
                            data.FinalizeDialog();
                        }

                        break;
                    }
                case 21:
                    {
                        if (client.Inventory.Contain(710566, 1))
                        {
                            data.AddText("Great! You`ve collected all 5 tokens! Now I will compose them into 1 Golden Token. All the Bandits will respect you once them see the token.");
                            data.AddOption("OK.", 22);//1273000
                            data.AddAvatar(117);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("If you can kill the Senior Bandit, you will have the chance to get the Scarlet Token.");
                            data.AddOption("I~see.");
                            data.AddAvatar(117);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 22:
                    {
                        if (client.Inventory.Contain(710566, 1))
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest6.MissionId))
                            {
                                client.Inventory.Remove(710566, 1, stream);
                                client.Player.Money += 100;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                                client.CreateBoxDialog("You~received~100~Silver.");
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "recovery");
                                data.AddText("I will dress you just like a bandit. Then you can meet with Guide Zhang, and he can teleport you to the Bandit Chamber. But you will");
                                data.AddText("~turn back after 90 seonds.");
                                data.AddOption("I~see.", 23);
                                data.AddAvatar(117);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("If you can kill the Senior Bandit, you will have the chance to get the Scarlet Token.");
                            data.AddOption("I~see.");
                            data.AddAvatar(117);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 20:
                    {
                        data.AddText("The Senior Bandits (Bird Island, 570, 416) has the Scarlet Token. He is much stronger than all the former Bandits, so you`ll need to pay more attention.");
                        data.AddText("~You`ll have the chance to get the token by killing him.");
                        data.AddOption("I~see.", 19);
                        data.AddAvatar(117);
                        data.FinalizeDialog();
                        break;
                    }
                case 17:
                    {
                        if (client.Inventory.Contain(710565, 1))
                        {
                            data.AddText("You are much stronger than I imagined. I think my revenge will succeed, soon! There is still one thing we need to find Commander Kerry.");
                            data.AddOption("What~is~it?", 18);//1272960
                            data.AddAvatar(117);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("The Bandit Deliverers appear around Bird Island (431, 453). Only killing 10 of them will the Pitch Token appears.");
                            data.AddOption("OK.");
                            data.AddAvatar(117);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 18:
                    {
                        if (client.Inventory.Contain(710565, 1))
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest5.MissionId))
                            {
                                client.Inventory.Remove(710565, 1, stream);
                                client.Player.Money += 100;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                                client.CreateBoxDialog("You~received~100~Silver.");
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "recovery");
                                data.AddText("The Senior Bandits (Bird Island, 570, 416) has the Scarlet Token. He is much stronger than all the former Bandits, so you`ll need to pay more attention.");
                                data.AddText("~You`ll have the chance to get the token by killing him.");
                                data.AddOption("I~see.", 19);
                                data.AddAvatar(117);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("The Bandit Deliverers appear around Bird Island (431, 453). Only killing 10 of them will the Pitch Token appears.");
                            data.AddOption("OK.");
                            data.AddAvatar(117);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 19:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest6, 0);
                        client.CreateBoxDialog("Kill~the~Senior~Bandits~(Bird~Island,~570~416).");
                        break;
                    }
                case 16:
                    {
                        data.AddText("Actually I don`t want you to fight against the Bandit Deliverers, but we have to get the Pitch Token. You`ll need to kill 10");
                        data.AddText("~Bandit Deliverers on Bird Island (525,~345) to get the Pitch Token.");
                        data.AddOption("I~see.", 15);
                        data.AddAvatar(117);
                        data.FinalizeDialog();
                        break;
                    }
                case 13:
                    {
                        if (client.Inventory.Contain(710564, 1))
                        {
                            data.AddText("Great! You`ve got the Black Token. Now, caccording to the information on it, the Pitch Token is held by the Bandit Deliverers.");
                            data.AddOption("What~should~I~do?", 14);//1272920
                            data.AddAvatar(117);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("I think only 10 Bandits and 1 Bandit Courier around Bird Island (316, 567) won`t be a trouble to you!");
                            data.AddText("~Kill them, and then you`ll get the Black Token.");
                            data.AddOption("I~see.");
                            data.AddAvatar(117);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 15:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        client.CreateBoxDialog("Kill~10~Bandit~Deliverers~on~Bird~Island~(525,~345).");
                        break;
                    }
                case 14:
                    {
                        if (client.Inventory.Contain(710564, 1))
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                            {
                                client.Inventory.Remove(710564, 1, stream);
                                client.Player.Money += 100;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                                client.CreateBoxDialog("You~received~100~Silver.");
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "recovery");

                                data.AddText("Actually I don`t want you to fight against the Bandit Deliverers, but we have to get the Pitch Token. You`ll need to kill 10");
                                data.AddText("~Bandit Deliverers on Bird Island (525,~345) to get the Pitch Token.");
                                data.AddOption("I~see.", 15);
                                data.AddAvatar(117);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("I think only 10 Bandits and 1 Bandit Courier around Bird Island (316, 567) won`t be a trouble to you!");
                            data.AddText("~Kill them, and then you`ll get the Black Token.");
                            data.AddOption("I~see.");
                            data.AddAvatar(117);
                            data.FinalizeDialog();
                        }

                        break;
                    }
                case 12:
                    {
                        data.AddText("The Indigo Token shows that a Bandit Courier has the Black Token, who is hiding among 10 Bandits. If you kill all the Bandits,");
                        data.AddText("~the Bandit Couriers will appear.");
                        data.AddOption("I~see.", 11);
                        data.AddAvatar(117);
                        data.FinalizeDialog();
                        break;
                    }
                case 9:
                    {
                        if (client.Inventory.Contain(710563, 1))
                        {
                            data.AddText("Great! You`ve found the Indigo Token! I must check it carefully. Thanks a lot.");
                            data.AddOption("You`re~welcome.", 10);//1272880
                            data.AddAvatar(117);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Well, be patient! You will find the Indigo Token if you kill enough Bandits L98 (around Bird Island 455, 466).");
                            data.AddOption("Alright.");
                            data.AddAvatar(117);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 10:
                    {
                        if (client.Inventory.Contain(710563, 1))
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                            {
                                client.Inventory.Remove(710563, 1, stream);
                                client.Player.Money += 100;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                                client.CreateBoxDialog("You~received~100~Silver.");
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "recovery");

                                data.AddText("The Indigo Token shows that a Bandit Courier has the Black Token, who is hiding among 10 Bandits. If you kill all the Bandits,");
                                data.AddText("~the Bandit Couriers will appear.");
                                data.AddOption("I~see.", 11);
                                data.AddAvatar(117); data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Well, be patient! You will find the Indigo Token if you kill enough Bandits L98 (around Bird Island 455, 466).");
                            data.AddOption("Alright.");
                            data.AddAvatar(117);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 11:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        client.CreateBoxDialog("Kill~the~Bandits~around~Bird~Island~(530,349)~and~find~the~Bandit~Courier.");
                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.Contain(710562, 1) && client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 50))
                        {
                            data.AddText("Good! It is the Red Token. It shows that the Indigo Token is kept by a Bandit L98 .");
                            data.AddOption("How~to~find~it?", 6);//1272840
                            data.AddAvatar(117);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("The Red Token is held by the Bandit Leader L97  (around Bird Island 486, 481). Only if you kill 50 Bandits L97 will He appears.");
                            data.AddText("~Without this token, we won`t be able to find the other tokens.");
                            data.AddOption("I~see.");
                            data.AddAvatar(117);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.Contain(710562, 1) && client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 50))
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                            {
                                client.Inventory.Remove(710562, 1, stream);
                                client.Player.Money += 100;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                                client.CreateBoxDialog("You~received~100~Silver.");
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "recovery");
                                data.AddText("The Red Token shows the Indigo Token is kept by a Bandit L98, but I don`t know which one he is exactly.");
                                data.AddText("~You`ll need to keep killing the Bandits L98 until you find the token.");
                                data.AddOption("I~see.", 7);//1272855
                                data.AddAvatar(117);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 8:
                    {
                        data.AddText("The Red Token shows the Indigo Token is kept by a Bandit L98, but I don`t know which one he is exactly.");
                        data.AddText("~You`ll need to keep killing the Bandits L98 until you find the token.");
                        data.AddOption("I~see.", 7);
                        data.AddAvatar(117);
                        data.FinalizeDialog();
                        break;
                    }
                case 7:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.CreateBoxDialog("Kill~Bandits~L98~(around~Bird~Island~257,~162).");
                        break;
                    }
                case 1:
                    {
                        data.AddText("Your are much stronger than me. I think it will be no problem for you to defeat Commander Kerry, but it`s difficult to get to him.");
                        data.AddText("~You must get 4 tokens from the Bandits around Bird Island, first.");
                        data.AddOption("I`ll~help~you.", 2);
                        data.AddOption("Too~dangerous~for~me.");
                        data.AddAvatar(117);
                        data.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You completed the quest and received 100 Silver!");
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "recovery");

                            data.AddText("Great! Now let`s start from the Red Token. You`ll need to kill 50 Bandits L97 (around Bird Island 262,117) until the Bandit Leader L97 shows up.");
                            data.AddText("~When he has been killed, you will obtain a Red Token. Only with it, I can find out where is the next token.");
                            data.AddOption("I~see.", 3);
                            data.AddAvatar(117);
                            data.FinalizeDialog();

                        }
                        break;
                    }
                case 4:
                    {
                        data.AddText("Great! Now let`s start from the Red Token. You`ll need to kill 50 Bandits L97 (around Bird Island 262,117) until the Bandit Leader L97 shows up.");
                        data.AddText("~When he has been killed, you will obtain a Red Token. Only with it, I can find out where is the next token.");
                        data.AddOption("I~see.", 3);
                        data.AddAvatar(117);
                        data.FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.CreateBoxDialog("Kill~Bandits~L97~(around~Bird~Island~262,117).");
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.CommanderAid4)]
        public static void CommanderAid4(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        switch (Pool.GetRandom.Next(0, 8))
                        {
                            case 0:
                                {
                                    data.AddText("Long live Commander Kerry!");
                                    data.AddOption("Whatever.");
                                    data.AddAvatar(100);
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 1:
                                {
                                    data.AddText("Long live Commander Kerry!");
                                    data.AddOption("Whatever.");
                                    data.AddAvatar(100);
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 2:
                                {
                                    data.AddText("I have a rendezvous with death.");
                                    data.AddOption("Neat!");
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 3:
                                {
                                    data.AddText("I have a rendezvous with death.");
                                    data.AddOption("Neat!");
                                    data.AddAvatar(100);
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 4:
                                {
                                    data.AddText("I will do anything for Commander Kerry.");
                                    data.AddOption("As~you~wish.");
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 5:
                                {
                                    data.AddText("I will do anything for Commander Kerry.");
                                    data.AddOption("As~you~wish.");
                                    data.AddAvatar(100);
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 6:
                                {
                                    data.AddText("Hey, you look just like me....why don`t you join us instead?");
                                    data.AddOption("Like~you?~No~way.");
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 7:
                                {
                                    data.AddText("Hey, you look just like me....why don`t you join us instead?");
                                    data.AddOption("Like~you?~No~way.");
                                    data.AddAvatar(100);
                                    data.FinalizeDialog();
                                    break;
                                }
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.CommanderAid3)]
        public static void CommanderAid3(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        switch (Pool.GetRandom.Next(0, 8))
                        {
                            case 0:
                                {
                                    data.AddText("Long live Commander Kerry!");
                                    data.AddOption("Whatever.");
                                    data.AddAvatar(100);
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 1:
                                {
                                    data.AddText("Long live Commander Kerry!");
                                    data.AddOption("Whatever.");
                                    data.AddAvatar(100);
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 2:
                                {
                                    data.AddText("I have a rendezvous with death.");
                                    data.AddOption("Neat!");
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 3:
                                {
                                    data.AddText("I have a rendezvous with death.");
                                    data.AddOption("Neat!");
                                    data.AddAvatar(100);
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 4:
                                {
                                    data.AddText("I will do anything for Commander Kerry.");
                                    data.AddOption("As~you~wish.");
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 5:
                                {
                                    data.AddText("I will do anything for Commander Kerry.");
                                    data.AddOption("As~you~wish.");
                                    data.AddAvatar(100);
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 6:
                                {
                                    data.AddText("Hey, you look just like me....why don`t you join us instead?");
                                    data.AddOption("Like~you?~No~way.");
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 7:
                                {
                                    data.AddText("Hey, you look just like me....why don`t you join us instead?");
                                    data.AddOption("Like~you?~No~way.");
                                    data.AddAvatar(100);
                                    data.FinalizeDialog();
                                    break;
                                }
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.CommanderAid2)]
        public static void CommanderAid2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        switch (Pool.GetRandom.Next(0, 8))
                        {
                            case 0:
                                {
                                    data.AddText("Long live Commander Kerry!");
                                    data.AddOption("Whatever.");
                                    data.AddAvatar(100);
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 1:
                                {
                                    data.AddText("Long live Commander Kerry!");
                                    data.AddOption("Whatever.");
                                    data.AddAvatar(100);
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 2:
                                {
                                    data.AddText("I have a rendezvous with death.");
                                    data.AddOption("Neat!");
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 3:
                                {
                                    data.AddText("I have a rendezvous with death.");
                                    data.AddOption("Neat!");
                                    data.AddAvatar(100);
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 4:
                                {
                                    data.AddText("I will do anything for Commander Kerry.");
                                    data.AddOption("As~you~wish.");
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 5:
                                {
                                    data.AddText("I will do anything for Commander Kerry.");
                                    data.AddOption("As~you~wish.");
                                    data.AddAvatar(100);
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 6:
                                {
                                    data.AddText("Hey, you look just like me....why don`t you join us instead?");
                                    data.AddOption("Like~you?~No~way.");
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 7:
                                {
                                    data.AddText("Hey, you look just like me....why don`t you join us instead?");
                                    data.AddOption("Like~you?~No~way.");
                                    data.AddAvatar(100);
                                    data.FinalizeDialog();
                                    break;
                                }
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.CommanderAid1)]
        public static void CommanderAid1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        switch (Pool.GetRandom.Next(0, 8))
                        {
                            case 0:
                                {
                                    data.AddText("Long live Commander Kerry!");
                                    data.AddOption("Whatever.");
                                    data.AddAvatar(100);
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 1:
                                {
                                    data.AddText("Long live Commander Kerry!");
                                    data.AddOption("Whatever.");
                                    data.AddAvatar(100);
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 2:
                                {
                                    data.AddText("I have a rendezvous with death.");
                                    data.AddOption("Neat!");
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 3:
                                {
                                    data.AddText("I have a rendezvous with death.");
                                    data.AddOption("Neat!");
                                    data.AddAvatar(100);
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 4:
                                {
                                    data.AddText("I will do anything for Commander Kerry.");
                                    data.AddOption("As~you~wish.");
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 5:
                                {
                                    data.AddText("I will do anything for Commander Kerry.");
                                    data.AddOption("As~you~wish.");
                                    data.AddAvatar(100);
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 6:
                                {
                                    data.AddText("Hey, you look just like me....why don`t you join us instead?");
                                    data.AddOption("Like~you?~No~way.");
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 7:
                                {
                                    data.AddText("Hey, you look just like me....why don`t you join us instead?");
                                    data.AddOption("Like~you?~No~way.");
                                    data.AddAvatar(100);
                                    data.FinalizeDialog();
                                    break;
                                }
                        }
                        break;
                    }
            }
        }


        public static uint[] GetItemMr_MirrorEpicTrojanQuest()
        {
            switch (Pool.GetRandom.Next(0, 17))
            {
                case 0: return new uint[2] { 729304, 2 };
                case 1: return new uint[2] { 711211, 2 };
                case 2: return new uint[2] { 721258, 2 };
                case 3: return new uint[2] { 729094, 100 };
                case 4: return new uint[2] { 729095, 100 };
                case 5: return new uint[2] { 729096, 100 };
                case 6: return new uint[2] { 723903, 20 };
                case 7: return new uint[2] { Database.ItemType.MoonBox, 1 };
                case 8: return new uint[2] { 723087, 3 };
                case 9: return new uint[2] { 721261, 5 };
                case 10: return new uint[2] { 3003125, 3 };
                case 11: return new uint[2] { Database.ItemType.MeteorScroll, 2 };
                case 12: return new uint[2] { 720598, 3 };
                case 13: return new uint[2] { 727061, 3 };
                case 14: return new uint[2] { 727060, 3 };
                case 15: return new uint[2] { 1200001, 2 };
                default:
                    return new uint[2] { 721259, 1 };
            }
        }
        public static uint[] GetItemArrayEpicTrojanQuest()
        {
            switch (Pool.GetRandom.Next(0, 20))
            {
                case 0: return new uint[2] { 700012, 5 };
                case 1: return new uint[2] { 700022, 5 };
                case 2: return new uint[2] { 700032, 5 };
                case 3: return new uint[2] { 700042, 5 };
                case 4: return new uint[2] { 700052, 5 };
                case 5: return new uint[2] { 700062, 5 };
                case 6: return new uint[2] { 700072, 2 };
                case 7: return new uint[2] { 752003, 3 };
                case 8: return new uint[2] { Database.ItemType.MoonBox, 1 };
                case 9: return new uint[2] { 723903, 10 };
                case 10: return new uint[2] { Database.ItemType.MeteorScroll, 1 };
                case 11: return new uint[2] { 730001, 3 };
                case 12: return new uint[2] { 720842, 3 };
                case 13: return new uint[2] { 729090, 100 };
                case 14: return new uint[2] { 729091, 100 };
                case 15: return new uint[2] { 729092, 100 };
                case 16: return new uint[2] { 729093, 100 };
                case 17: return new uint[2] { 3003124, 1 };
                case 18: return new uint[2] { 723727, 2 };
                default: return new uint[2] { 720828, 3 };
            }
        }
        public static string GetArrayEpicTrojanItemStr(uint[] item)
        {
            string str = "";
            Database.ItemType.DBItem DBItem;
            if (Enum.IsDefined(typeof(Role.Flags.Gem), (byte)(item[0] % 1000)))
            {
                str += ((Role.Flags.Gem)(item[0] % 1000)).ToString() + "*" + item[1];
            }
            else if (Pool.ItemsBase.TryGetValue(item[0], out DBItem))
            {

                str += DBItem.Name + "*" + item[1];

            }
            return str;
        }
        public static uint[] GetItemEpicTrojanQuest()
        {
            switch (Pool.GetRandom.Next(0, 15))
            {
                case 0:
                    return new uint[2] { Database.ItemType.Meteor, 5 };
                case 1:
                    return new uint[2] { Database.ItemType.DoubleExp, 10 };
                case 2:
                    return new uint[2] { Database.ItemType.ExpBall, 3 };
                case 3: return new uint[2] { 723341, 3 };
                case 4: return new uint[2] { 1200000, 3 };
                case 5: return new uint[2] { 727066, 3 };
                case 6: return new uint[2] { 727065, 3 };
                case 7: return new uint[2] { 727064, 3 };
                case 8: return new uint[2] { 727063, 3 };
                case 9: return new uint[2] { 727062, 3 };
                case 10: return new uint[2] { 727067, 3 };
                case 11: return new uint[2] { 727068, 3 };
                case 12: return new uint[2] { Database.ItemType.MeteorScroll, 1 };
                case 13: return new uint[2] { 1080001, 5 };
                default:
                    {
                        return new uint[2] { Database.ItemType.DragonBall, 1 };
                    }
                //to do: apeskin, heard snake birdclaw.
            }
        }
        public static string GetEpicTrojanItemStr(uint[] item)
        {
            string str = "";
            Database.ItemType.DBItem DBItem;
            if (Pool.ItemsBase.TryGetValue(item[0], out DBItem))
                str += DBItem.Name + "*" + item[1];
            return str;
        }


        [NpcAttribute(NpcID.MrMirrorMap2)]
        public static void MrMirrorMap2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("The~sky~begins~to~brighten.~It`s~an~omen~of~good~fortune.");
                        data.AddOption("Great!");
                        data.AddAvatar(247);
                        data.FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.MonkMisery1)]
        public static void MonkMisery1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("You`re~in~the~Evil~Array.~The~magic~is~quite~weak,~now.~Listen,~I`ll~uncover~the~Array`s~Eye,~first.~So,~you~can~enchant~it.");
                        data.AddOption("Uncover~the~Array`s~Eye.", 1);
                        data.AddAvatar(248);
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Player.EpicTrojanAbysalStage = 1;
                        client.Teleport(client.Player.X, client.Player.Y, 3826);
                        client.Player.AddMapEffect(stream, 39, 45, "zf2-e280");
                        client.Player.AddMapEffect(stream, 45, 45, "tj");
                        client.CreateBoxDialog("Monk~Misery~has~uncovered~the~Evil~Array`s~Eye.");

                        break;
                    }
            }
        }



        [NpcAttribute(NpcID.Epic2MonkMisery)]
        public static void Epic2MonkMisery(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralPak, 0, 3270);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralPak, 0, 3276);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.StageEpicTrojanQuest >= 20)
                        {

                            data.AddText("Great~job!~You`ve~enchant~the~Evil~Array~with~10,000~points~of~magic.~Hurry~and~go~find~General~Pak!");
                            data.AddOption("Report~back~to~General~Pak.", 1);
                            data.AddAvatar(248);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.EpicTrojanEvilArrayPoints >= 10000)
                            {
                                client.Player.StageEpicTrojanQuest = 20;
                                client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                                client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "dgbos_addr");
                                data.AddText("Great~job!~You`ve~enchant~the~Evil~Array~with~10,000~points~of~magic.~Hurry~and~go~find~General~Pak!");
                                data.AddOption("Report~back~to~General~Pak.", 1);
                                data.FinalizeDialog();
                                break;

                            }
                            else
                            {
                                if (client.Player.EpicTrojanArrayItemChange[0] == 0)
                                    client.Player.EpicTrojanArrayItemChange = GetItemArrayEpicTrojanQuest();
                                if (client.Player.CanChangeArrayEpicMaterial == 1)
                                {
                                    data.AddText("Currently,~the~Evil~Array~is~holding~%stc(121,87)~points~of~magic.~It~needs~10,000~points~to~recover~the~seal.~I~can~enchant~it~once~a~day~with~450~points~of~magic.~It`ll~take~23~days~in~total.\n\n");
                                    data.AddText("Enchanting~Material:" + GetArrayEpicTrojanItemStr(client.Player.EpicTrojanArrayItemChange) + "\n~~~~~~~~~~~~~~Exorcism~Token*1\n\n");
                                    data.AddOption("Enhance~it!", 2);
                                    data.AddOption("Change~the~materials.", 3);
                                    data.AddOption("Leave.", 100);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("You~don`t~have~enough~Energies~to~continue.~1~Energy~will~be~restored~when~you~log~in~each~day.~You~can~also~continue~the~enhancement~by~enhancing~the~Array`s~Eye.");
                                    data.AddText("\n\n~~------------------------------------------------------\n~~~[Tomorrow`s~Material]~" + GetArrayEpicTrojanItemStr(client.Player.EpicTrojanArrayItemChange) + "\n~~~~~~~~~~~~~~~Exorcism~Token*1\n~~~[Change~Times]~" + client.Player.ChangeArrayEpicTrojan + "\n~~------------------------------------------------------\n\n");
                                    data.AddOption("Enhance~via~the~Array`s~Eye.", 20);//todo
                                    data.AddOption("Change~the~material.~(1~CP)", 10);
                                    data.AddOption("Leave.", 100);
                                    data.FinalizeDialog();
                                }
                            }


                            break;
                        }
                        break;
                    }
                case 10:
                    {
                        if (client.Player.ConquerPoints < 1)
                        {
                            data.AddText("You~don`t~have~enough~CPs~to~change~the~enchanting~materials.");
                            data.AddOption("Alright.");
                            data.AddAvatar(248);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.InTrade) return;
                            client.Player.ConquerPoints -= 1;
                            client.Player.ChangeArrayEpicTrojan += 1;
                            client.Player.EpicTrojanArrayItemChange = GetItemArrayEpicTrojanQuest();
                            data.AddText("You~don`t~have~enough~Energies~to~continue.~1~Energy~will~be~restored~when~you~log~in~each~day.~You~can~also~continue~the~enhancement~by~enhancing~the~Array`s~Eye.");
                            data.AddText("\n\n~~------------------------------------------------------\n~~~[Tomorrow`s~Material]~" + GetArrayEpicTrojanItemStr(client.Player.EpicTrojanArrayItemChange) + "\n~~~~~~~~~~~~~~~Exorcism~Token*1\n~~~[Change~Times]~" + client.Player.ChangeArrayEpicTrojan + "\n~~------------------------------------------------------\n\n");
                            data.AddOption("Enhance~via~the~Array`s~Eye.", 20);//todo
                            data.AddOption("Change~the~material.~(1~CP)", 10);
                            data.AddOption("Leave.", 100);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 100:
                    {
                        client.Teleport(88, 170, 3830);
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(client.Player.EpicTrojanArrayItemChange[0], client.Player.EpicTrojanArrayItemChange[1]) && client.Player.CanChangeArrayEpicMaterial == 1)
                        {
                            client.Player.CanChangeArrayEpicMaterial = 0;
                            client.Inventory.Remove(client.Player.EpicTrojanArrayItemChange[0], client.Player.EpicTrojanArrayItemChange[1], stream);
                            client.CreateBoxDialog("You`ve~successfully~enchanted~the~Evil~Array~with~450~points~of~magic!");
                            client.Player.EpicTrojanEvilArrayPoints += 450;
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "stipple_dhz");
                            client.Player.AddMapEffect(stream, 45, 45, "zf2-e121");
                            if (client.Player.EpicTrojanEvilArrayPoints >= 10000)
                            {
                                client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                                client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                                client.Player.StageEpicTrojanQuest = 20;
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "dgbos_addr");
                                data.AddText("Great~job!~You`ve~enchant~the~Evil~Array~with~10,000~points~of~magic.~Hurry~and~go~find~General~Pak!");
                                data.AddOption("Report~back~to~General~Pak.", 1);
                                data.FinalizeDialog();
                                break;
                            }

                            if (client.Player.Map == 3826)
                            {

                                client.Player.AddMapEffect(stream, 39, 45, "stipple_bdh");
                            }
                            else if (client.Player.Map == 3827)
                            {
                                client.Player.AddMapEffect(stream, 39, 45, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 45, 39, "stipple_bdh");
                            }
                            else if (client.Player.Map == 3828)
                            {
                                //45 51 stipple_bdh

                                client.Player.AddMapEffect(stream, 51, 45, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 45, 39, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 39, 45, "stipple_bdh");
                            }
                            else if (client.Player.Map == 3829)
                            {
                                client.Player.AddMapEffect(stream, 045, 051, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 51, 45, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 45, 39, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 39, 45, "stipple_bdh");
                            }

                            if (client.Player.EpicTrojanEvilArrayPoints >= 1500 && client.Player.Map == 3826)
                            {
                                client.Player.EpicTrojanAbysalStage = 2;
                                client.Teleport(client.Player.X, client.Player.Y, 3827);
                                client.Player.AddMapEffect(stream, 45, 39, "zf2-e280");
                                client.Player.AddMapEffect(stream, 45, 45, "tj");

                            }
                            else if (client.Player.EpicTrojanEvilArrayPoints >= 3500 && client.Player.Map == 3827)
                            {
                                client.Player.EpicTrojanAbysalStage = 3;
                                client.Teleport(client.Player.X, client.Player.Y, 3828);
                                client.Player.AddMapEffect(stream, 051, 045, "zf2-e280");
                                client.Player.AddMapEffect(stream, 45, 45, "tj");


                            }
                            else if (client.Player.EpicTrojanEvilArrayPoints >= 6000 && client.Player.Map == 3828)
                            {
                                client.Player.EpicTrojanAbysalStage = 4;
                                client.Teleport(client.Player.X, client.Player.Y, 3829);
                                client.Player.AddMapEffect(stream, 045, 051, "zf2-e280");
                                client.Player.AddMapEffect(stream, 45, 45, "tj");



                            }
                            else
                            {

                                if (client.Player.CanChangeArrayEpicMaterial == 1)
                                {
                                    data.AddText("Currently,~the~Evil~Array~is~holding~" + client.Player.EpicTrojanEvilArrayPoints + "~points~of~magic.~It~needs~10,000~points~to~recover~the~seal.~I~can~enchant~it~once~a~day~with~450~points~of~magic.~It`ll~take~23~days~in~total.\n\n");
                                    data.AddText("Enchanting~Material:" + GetArrayEpicTrojanItemStr(client.Player.EpicTrojanArrayItemChange) + "\n~~~~~~~~~~~~~~Exorcism~Token*1\n\n");
                                    data.AddOption("Enhance~it!", 2);
                                    data.AddOption("Change~the~materials.", 3);
                                    data.AddOption("Leave.", 100);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("You~don`t~have~enough~Energies~to~continue.~1~Energy~will~be~restored~when~you~log~in~each~day.~You~can~also~continue~the~enhancement~by~enhancing~the~Array`s~Eye.");
                                    data.AddText("\n\n~~------------------------------------------------------\n~~~[Tomorrow`s~Material]~" + GetArrayEpicTrojanItemStr(client.Player.EpicTrojanArrayItemChange) + "\n~~~~~~~~~~~~~~~Exorcism~Token*1\n~~~[Change~Times]~" + client.Player.ChangeArrayEpicTrojan + "\n~~------------------------------------------------------\n\n");
                                    data.AddOption("Enhance~via~the~Array`s~Eye.", 20);//todo
                                    data.AddOption("Change~the~material.~(1~CP)", 10);
                                    data.AddOption("Leave.", 100);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            data.AddText("You~don`t~have~enough~materials~to~enchant~the~Evil~Array.\n\n~~~~Enchanting~Material:" + GetArrayEpicTrojanItemStr(client.Player.EpicTrojanArrayItemChange) + "\n");
                            data.AddOption("Alright.");
                            data.AddOption("Change~the~material.", 3);
                            data.AddOption("Entrust~the~work.", 4);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        data.AddText("Are~you~sure~you~want~to~pay~18~CPs~to~entrust~me~with~the~whole~enchanting~work~of~the~Evil~Array?");
                        data.AddOption("Yes!~(18~CPs)", 4);
                        data.AddOption("No,~I~changed~my~mind.");
                        data.AddAvatar(248);
                        data.FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        if (client.Player.CanChangeArrayEpicMaterial == 0)
                            break;
                        if (client.Player.ConquerPoints < 18)
                        {
                            data.AddText("Sorry,~I~can`t~help~you~if~you~don`t~have~18~CPs.");
                            data.AddOption("Alright.");
                            data.AddAvatar(248);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.InTrade) return;
                            client.Player.ConquerPoints -= 18;
                            client.Player.CanChangeArrayEpicMaterial = 0;
                            client.CreateBoxDialog("You`ve~successfully~enchanted~the~Evil~Array~with~450~points~of~magic!");
                            client.Player.EpicTrojanEvilArrayPoints += 450;
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "stipple_dhz");
                            client.Player.AddMapEffect(stream, 45, 45, "zf2-e121");
                            if (client.Player.EpicTrojanEvilArrayPoints >= 10000)
                            {
                                client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                                client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                                client.Player.StageEpicTrojanQuest = 20;
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "dgbos_addr");
                                data.AddText("Great~job!~You`ve~enchant~the~Evil~Array~with~10,000~points~of~magic.~Hurry~and~go~find~General~Pak!");
                                data.AddOption("Report~back~to~General~Pak.", 1);
                                data.FinalizeDialog();
                                break;
                            }
                            if (client.Player.Map == 3826)
                            {

                                client.Player.AddMapEffect(stream, 39, 45, "stipple_bdh");
                            }
                            else if (client.Player.Map == 3827)
                            {
                                client.Player.AddMapEffect(stream, 39, 45, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 45, 39, "stipple_bdh");
                            }
                            else if (client.Player.Map == 3828)
                            {
                                //45 51 stipple_bdh

                                client.Player.AddMapEffect(stream, 51, 45, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 45, 39, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 39, 45, "stipple_bdh");
                            }
                            else if (client.Player.Map == 3829)
                            {
                                client.Player.AddMapEffect(stream, 045, 051, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 51, 45, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 45, 39, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 39, 45, "stipple_bdh");
                            }
                            if (client.Player.EpicTrojanEvilArrayPoints >= 1500 && client.Player.Map == 3826)
                            {
                                client.Player.EpicTrojanAbysalStage = 2;
                                client.Teleport(client.Player.X, client.Player.Y, 3827);
                                client.Player.AddMapEffect(stream, 45, 39, "zf2-e280");
                                client.Player.AddMapEffect(stream, 45, 45, "tj");
                            }
                            else if (client.Player.EpicTrojanEvilArrayPoints >= 3500 && client.Player.Map == 3827)
                            {
                                client.Player.EpicTrojanAbysalStage = 3;
                                client.Teleport(client.Player.X, client.Player.Y, 3828);
                                client.Player.AddMapEffect(stream, 051, 045, "zf2-e280");
                                client.Player.AddMapEffect(stream, 45, 45, "tj");
                            }
                            else if (client.Player.EpicTrojanEvilArrayPoints >= 6000 && client.Player.Map == 3828)
                            {
                                client.Player.EpicTrojanAbysalStage = 4;
                                client.Teleport(client.Player.X, client.Player.Y, 3829);
                                client.Player.AddMapEffect(stream, 045, 051, "zf2-e280");
                                client.Player.AddMapEffect(stream, 45, 45, "tj");
                            }
                            else
                            {

                                if (client.Player.CanChangeArrayEpicMaterial == 1)
                                {
                                    data.AddText("Currently,~the~Evil~Array~is~holding~" + client.Player.EpicTrojanEvilArrayPoints + "~points~of~magic.~It~needs~10,000~points~to~recover~the~seal.~I~can~enchant~it~once~a~day~with~450~points~of~magic.~It`ll~take~23~days~in~total.\n\n");
                                    data.AddText("Enchanting~Material:" + GetArrayEpicTrojanItemStr(client.Player.EpicTrojanArrayItemChange) + "\n~~~~~~~~~~~~~~Exorcism~Token*1\n\n");
                                    data.AddOption("Enhance~it!", 2);
                                    data.AddOption("Change~the~materials.", 3);
                                    data.AddOption("Leave.", 100);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("You~don`t~have~enough~Energies~to~continue.~1~Energy~will~be~restored~when~you~log~in~each~day.~You~can~also~continue~the~enhancement~by~enhancing~the~Array`s~Eye.");
                                    data.AddText("\n\n~~------------------------------------------------------\n~~~[Tomorrow`s~Material]~" + GetArrayEpicTrojanItemStr(client.Player.EpicTrojanArrayItemChange) + "\n~~~~~~~~~~~~~~~Exorcism~Token*1\n~~~[Change~Times]~" + client.Player.ChangeArrayEpicTrojan + "\n~~------------------------------------------------------\n\n");
                                    data.AddOption("Enhance~via~the~Array`s~Eye.", 20);//todo
                                    data.AddOption("Change~the~material.~(1~CP)", 10);
                                    data.AddOption("Leave.", 100);
                                    data.FinalizeDialog();
                                }
                            }


                        }
                        break;
                    }
                case 20:
                    {
                        data.AddText("Currently,~the~Evil~Array~is~holding~" + client.Player.EpicTrojanEvilArrayPoints + "~points~of~magic.~It~needs~10,000~points~of~magic~to~recover~the~seal.\n\n");
                        data.AddText("You~can~enchant~" + GetMapEpicTrojanPoints(client.Player.Map) + "~points~of~magic,~per~time.~If~you~enchant~the~array~10~times,~at~once,~you`ll~uncover~an~extra~Array`s~Eye.\n");
                        data.AddText("~The~added~Array`s~Eye~can~be~enchanted~with~" + Math.Min(300, GetMapEpicTrojanPoints(client.Player.Map) + 50) + "~points~of~magic,~per~time.\n\n");
                        data.AddOption("Enchant~it~1~time.~(160~CPs)", 21);
                        if (client.Player.EpicTrojanEvilArrayPoints < 8650)
                            data.AddOption("Enchant~it~10~time.~(1600~CPs)", 23);
                        data.AddOption("Leave", 255);
                        data.FinalizeDialog();
                        break;
                    }
                case 23:
                    {
                        data.AddText("Currently,~the~Evil~Array~is~holding~" + client.Player.EpicTrojanEvilArrayPoints + "~points~of~magic.~Do~you~want~to~pay~1600~CPs~to~enchant~the~array~with~" + GetMapEpicTrojanPoints(client.Player.Map) * 10 + "~points~of~magic?");
                        data.AddOption("Yes!~(1600~CPs)", 24);
                        data.AddOption("I~haven`t~decided,~yet!");
                        data.FinalizeDialog();
                        break;
                    }
                case 24:
                    {
                        if (client.Player.ConquerPoints < 1600)
                        {
                            data.AddText("Sorry,~I~can`t~help~you~if~you~don`t~have~1600~CPs.");
                            data.AddOption("Alright.");
                            data.AddAvatar(248);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.InTrade) return;
                            client.Player.ConquerPoints -= 1600;

                            client.CreateBoxDialog("You`ve~successfully~enchanted~the~Evil~Array~with~" + GetMapEpicTrojanPoints(client.Player.Map) * 10 + "~points~of~magic!");
                            client.Player.EpicTrojanEvilArrayPoints += (uint)GetMapEpicTrojanPoints(client.Player.Map) * 10;
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "stipple_dhz");
                            client.Player.AddMapEffect(stream, 45, 45, "zf2-e121");
                            if (client.Player.EpicTrojanEvilArrayPoints >= 10000)
                            {
                                client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                                client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                                client.Player.StageEpicTrojanQuest = 20;
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "dgbos_addr");
                                data.AddText("Great~job!~You`ve~enchant~the~Evil~Array~with~10,000~points~of~magic.~Hurry~and~go~find~General~Pak!");
                                data.AddOption("Report~back~to~General~Pak.", 1);
                                data.FinalizeDialog();
                                break;
                            }
                            if (client.Player.Map == 3826)
                            {

                                client.Player.AddMapEffect(stream, 39, 45, "stipple_bdh");
                            }
                            else if (client.Player.Map == 3827)
                            {
                                client.Player.AddMapEffect(stream, 39, 45, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 45, 39, "stipple_bdh");
                            }
                            else if (client.Player.Map == 3828)
                            {
                                //45 51 stipple_bdh

                                client.Player.AddMapEffect(stream, 51, 45, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 45, 39, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 39, 45, "stipple_bdh");
                            }
                            else if (client.Player.Map == 3829)
                            {
                                client.Player.AddMapEffect(stream, 045, 051, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 51, 45, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 45, 39, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 39, 45, "stipple_bdh");
                            }
                            if (client.Player.EpicTrojanEvilArrayPoints >= 1500 && client.Player.Map == 3826)
                            {
                                client.Player.EpicTrojanAbysalStage = 2;
                                client.Teleport(client.Player.X, client.Player.Y, 3827);
                                client.Player.AddMapEffect(stream, 45, 39, "zf2-e280");
                                client.Player.AddMapEffect(stream, 45, 45, "tj");
                            }
                            else if (client.Player.EpicTrojanEvilArrayPoints >= 3500 && client.Player.Map == 3827)
                            {
                                client.Player.EpicTrojanAbysalStage = 3;
                                client.Teleport(client.Player.X, client.Player.Y, 3828);
                                client.Player.AddMapEffect(stream, 051, 045, "zf2-e280");
                                client.Player.AddMapEffect(stream, 45, 45, "tj");
                            }
                            else if (client.Player.EpicTrojanEvilArrayPoints >= 6000 && client.Player.Map == 3828)
                            {
                                client.Player.EpicTrojanAbysalStage = 4;
                                client.Teleport(client.Player.X, client.Player.Y, 3829);
                                client.Player.AddMapEffect(stream, 045, 051, "zf2-e280");
                                client.Player.AddMapEffect(stream, 45, 45, "tj");
                            }
                            else
                            {

                                if (client.Player.CanChangeArrayEpicMaterial == 1)
                                {
                                    data.AddText("Currently,~the~Evil~Array~is~holding~" + client.Player.EpicTrojanEvilArrayPoints + "~points~of~magic.~It~needs~10,000~points~to~recover~the~seal.~I~can~enchant~it~once~a~day~with~450~points~of~magic.~It`ll~take~23~days~in~total.\n\n");
                                    data.AddText("Enchanting~Material:" + GetArrayEpicTrojanItemStr(client.Player.EpicTrojanArrayItemChange) + "\n~~~~~~~~~~~~~~Exorcism~Token*1\n\n");
                                    data.AddOption("Enhance~it!", 2);
                                    data.AddOption("Change~the~materials.", 3);
                                    data.AddOption("Leave.", 100);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("You~don`t~have~enough~Energies~to~continue.~1~Energy~will~be~restored~when~you~log~in~each~day.~You~can~also~continue~the~enhancement~by~enhancing~the~Array`s~Eye.");
                                    data.AddText("\n\n~~------------------------------------------------------\n~~~[Tomorrow`s~Material]~" + GetArrayEpicTrojanItemStr(client.Player.EpicTrojanArrayItemChange) + "\n~~~~~~~~~~~~~~~Exorcism~Token*1\n~~~[Change~Times]~" + client.Player.ChangeArrayEpicTrojan + "\n~~------------------------------------------------------\n\n");
                                    data.AddOption("Enhance~via~the~Array`s~Eye.", 20);//todo
                                    data.AddOption("Change~the~material.~(1~CP)", 10);
                                    data.AddOption("Leave.", 100);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 21:
                    {
                        data.AddText("Currently,~the~Evil~Array~is~holding~" + client.Player.EpicTrojanEvilArrayPoints + "~points~of~magic.~Do~you~want~to~pay~160~CPs~to~enchant~the~array~with~" + GetMapEpicTrojanPoints(client.Player.Map) + "~points~of~magic?");
                        data.AddOption("Yes!~(160~CPs)", 22);
                        data.AddOption("I~haven`t~decided,~yet!");
                        data.FinalizeDialog();
                        break;
                    }
                case 22:
                    {
                        if (client.Player.ConquerPoints < 160)
                        {
                            data.AddText("Sorry,~I~can`t~help~you~if~you~don`t~have~160~CPs.");
                            data.AddOption("Alright.");
                            data.AddAvatar(248);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.InTrade) return;
                            client.Player.ConquerPoints -= 160;

                            client.CreateBoxDialog("You`ve~successfully~enchanted~the~Evil~Array~with~" + GetMapEpicTrojanPoints(client.Player.Map) + "~points~of~magic!");
                            client.Player.EpicTrojanEvilArrayPoints += (uint)GetMapEpicTrojanPoints(client.Player.Map);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "stipple_dhz");
                            client.Player.AddMapEffect(stream, 45, 45, "zf2-e121");
                            if (client.Player.EpicTrojanEvilArrayPoints >= 10000)
                            {
                                client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                                client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                                client.Player.StageEpicTrojanQuest = 20;
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "dgbos_addr");
                                data.AddText("Great~job!~You`ve~enchant~the~Evil~Array~with~10,000~points~of~magic.~Hurry~and~go~find~General~Pak!");
                                data.AddOption("Report~back~to~General~Pak.", 1);
                                data.FinalizeDialog();
                                break;
                            }
                            if (client.Player.Map == 3826)
                            {

                                client.Player.AddMapEffect(stream, 39, 45, "stipple_bdh");
                            }
                            else if (client.Player.Map == 3827)
                            {
                                client.Player.AddMapEffect(stream, 39, 45, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 45, 39, "stipple_bdh");
                            }
                            else if (client.Player.Map == 3828)
                            {
                                //45 51 stipple_bdh

                                client.Player.AddMapEffect(stream, 51, 45, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 45, 39, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 39, 45, "stipple_bdh");
                            }
                            else if (client.Player.Map == 3829)
                            {
                                client.Player.AddMapEffect(stream, 45, 51, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 51, 45, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 45, 39, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 39, 45, "stipple_bdh");
                            }
                            if (client.Player.EpicTrojanEvilArrayPoints >= 1500 && client.Player.Map == 3826)
                            {
                                client.Player.EpicTrojanAbysalStage = 2;
                                client.Teleport(client.Player.X, client.Player.Y, 3827);
                                client.Player.AddMapEffect(stream, 45, 39, "zf2-e280");
                                client.Player.AddMapEffect(stream, 45, 45, "tj");
                            }
                            else if (client.Player.EpicTrojanEvilArrayPoints >= 3500 && client.Player.Map == 3827)
                            {
                                client.Player.EpicTrojanAbysalStage = 3;
                                client.Teleport(client.Player.X, client.Player.Y, 3828);
                                client.Player.AddMapEffect(stream, 051, 045, "zf2-e280");
                                client.Player.AddMapEffect(stream, 45, 45, "tj");
                            }
                            else if (client.Player.EpicTrojanEvilArrayPoints >= 6000 && client.Player.Map == 3828)
                            {
                                client.Player.EpicTrojanAbysalStage = 4;
                                client.Teleport(client.Player.X, client.Player.Y, 3829);
                                client.Player.AddMapEffect(stream, 045, 051, "zf2-e280");
                                client.Player.AddMapEffect(stream, 45, 45, "tj");
                            }
                            else
                            {

                                if (client.Player.CanChangeArrayEpicMaterial == 1)
                                {
                                    data.AddText("Currently,~the~Evil~Array~is~holding~" + client.Player.EpicTrojanEvilArrayPoints + "~points~of~magic.~It~needs~10,000~points~to~recover~the~seal.~I~can~enchant~it~once~a~day~with~450~points~of~magic.~It`ll~take~23~days~in~total.\n\n");
                                    data.AddText("Enchanting~Material:" + GetArrayEpicTrojanItemStr(client.Player.EpicTrojanArrayItemChange) + "\n~~~~~~~~~~~~~~Exorcism~Token*1\n\n");
                                    data.AddOption("Enhance~it!", 2);
                                    data.AddOption("Change~the~materials.", 3);
                                    data.AddOption("Leave.", 100);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("You~don`t~have~enough~Energies~to~continue.~1~Energy~will~be~restored~when~you~log~in~each~day.~You~can~also~continue~the~enhancement~by~enhancing~the~Array`s~Eye.");
                                    data.AddText("\n\n~~------------------------------------------------------\n~~~[Tomorrow`s~Material]~" + GetArrayEpicTrojanItemStr(client.Player.EpicTrojanArrayItemChange) + "\n~~~~~~~~~~~~~~~Exorcism~Token*1\n~~~[Change~Times]~" + client.Player.ChangeArrayEpicTrojan + "\n~~------------------------------------------------------\n\n");
                                    data.AddOption("Enhance~via~the~Array`s~Eye.", 20);//todo
                                    data.AddOption("Change~the~material.~(1~CP)", 10);
                                    data.AddOption("Leave.", 100);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 1:
                    {
                        client.Teleport(88, 170, 3830);
                        break;
                    }
            }
        }
        public static int GetMapEpicTrojanPoints(uint MapID)
        {
            if (MapID == 3826)
                return 150;
            if (MapID == 3827)
                return 200;
            if (MapID == 3828)
                return 250;
            if (MapID == 3829)
                return 300;
            return 0;
        }


        public static void ArraysEye(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralPak, 0, 3270);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralPak, 0, 3276);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Currently,~the~Evil~Array~is~holding~" + client.Player.EpicTrojanEvilArrayPoints + "~points~of~magic.~It~needs~10,000~points~of~magic~to~recover~the~seal.\n\n");
                        data.AddText("You~can~enchant~" + GetMapEpicTrojanPoints(client.Player.Map) + "~points~of~magic,~per~time.~If~you~enchant~the~array~10~times,~at~once,~you`ll~uncover~an~extra~Array`s~Eye.\n");
                        data.AddText("~The~added~Array`s~Eye~can~be~enchanted~with~" + Math.Min(300, GetMapEpicTrojanPoints(client.Player.Map) + 50) + "~points~of~magic,~per~time.\n\n");
                        data.AddOption("Enchant~it~1~time.~(160~CPs)", 21);
                        if (client.Player.EpicTrojanEvilArrayPoints < 8650)
                            data.AddOption("Enchant~it~10~time.~(1600~CPs)", 23);
                        data.AddOption("Leave", 255);
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Teleport(88, 170, 3830);
                        break;
                    }
                case 23:
                    {
                        data.AddText("Currently,~the~Evil~Array~is~holding~" + client.Player.EpicTrojanEvilArrayPoints + "~points~of~magic.~Do~you~want~to~pay~1600~CPs~to~enchant~the~array~with~" + GetMapEpicTrojanPoints(client.Player.Map) * 10 + "~points~of~magic?");
                        data.AddOption("Yes!~(1600~CPs)", 24);
                        data.AddOption("I~haven`t~decided,~yet!");
                        data.FinalizeDialog();
                        break;
                    }
                case 24:
                    {
                        if (client.Player.ConquerPoints < 1600)
                        {
                            data.AddText("Sorry,~I~can`t~help~you~if~you~don`t~have~1600~CPs.");
                            data.AddOption("Alright.");
                            data.AddAvatar(248);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.InTrade) return;
                            client.Player.ConquerPoints -= 1600;

                            client.CreateBoxDialog("You`ve~successfully~enchanted~the~Evil~Array~with~" + GetMapEpicTrojanPoints(client.Player.Map) * 10 + "~points~of~magic!");
                            client.Player.EpicTrojanEvilArrayPoints += (uint)GetMapEpicTrojanPoints(client.Player.Map) * 10;
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "stipple_dhz");
                            client.Player.AddMapEffect(stream, 45, 45, "zf2-e121");
                            if (client.Player.EpicTrojanEvilArrayPoints >= 10000)
                            {
                                client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                                client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                                client.Player.StageEpicTrojanQuest = 20;
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "dgbos_addr");
                                data.AddText("Great~job!~You`ve~enchant~the~Evil~Array~with~10,000~points~of~magic.~Hurry~and~go~find~General~Pak!");
                                data.AddOption("Report~back~to~General~Pak.", 1);
                                data.FinalizeDialog();
                                break;
                            }
                            if (client.Player.Map == 3826)
                            {

                                client.Player.AddMapEffect(stream, 39, 45, "stipple_bdh");
                            }
                            else if (client.Player.Map == 3827)
                            {
                                client.Player.AddMapEffect(stream, 39, 45, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 45, 39, "stipple_bdh");
                            }
                            else if (client.Player.Map == 3828)
                            {
                                //45 51 stipple_bdh

                                client.Player.AddMapEffect(stream, 51, 45, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 45, 39, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 39, 45, "stipple_bdh");
                            }
                            else if (client.Player.Map == 3829)
                            {
                                client.Player.AddMapEffect(stream, 045, 051, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 51, 45, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 45, 39, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 39, 45, "stipple_bdh");
                            }
                            if (client.Player.EpicTrojanEvilArrayPoints >= 1500 && client.Player.Map == 3826)
                            {
                                client.Player.EpicTrojanAbysalStage = 2;
                                client.Teleport(client.Player.X, client.Player.Y, 3827);
                                client.Player.AddMapEffect(stream, 45, 39, "zf2-e280");
                                client.Player.AddMapEffect(stream, 45, 45, "tj");
                            }
                            else if (client.Player.EpicTrojanEvilArrayPoints >= 3500 && client.Player.Map == 3827)
                            {
                                client.Player.EpicTrojanAbysalStage = 3;
                                client.Teleport(client.Player.X, client.Player.Y, 3828);
                                client.Player.AddMapEffect(stream, 051, 045, "zf2-e280");
                                client.Player.AddMapEffect(stream, 45, 45, "tj");
                            }
                            else if (client.Player.EpicTrojanEvilArrayPoints >= 6000 && client.Player.Map == 3828)
                            {
                                client.Player.EpicTrojanAbysalStage = 4;
                                client.Teleport(client.Player.X, client.Player.Y, 3829);
                                client.Player.AddMapEffect(stream, 045, 051, "zf2-e280");
                                client.Player.AddMapEffect(stream, 45, 45, "tj");
                            }
                            else
                            {

                                data.AddText("Currently,~the~Evil~Array~is~holding~" + client.Player.EpicTrojanEvilArrayPoints + "~points~of~magic.~It~needs~10,000~points~of~magic~to~recover~the~seal.\n\n");
                                data.AddText("You~can~enchant~" + GetMapEpicTrojanPoints(client.Player.Map) + "~points~of~magic,~per~time.~If~you~enchant~the~array~10~times,~at~once,~you`ll~uncover~an~extra~Array`s~Eye.\n");
                                data.AddText("~The~added~Array`s~Eye~can~be~enchanted~with~" + Math.Min(300, GetMapEpicTrojanPoints(client.Player.Map) + 50) + "~points~of~magic,~per~time.\n\n");
                                data.AddOption("Enchant~it~1~time.~(160~CPs)", 21);
                                if (client.Player.EpicTrojanEvilArrayPoints < 8650)
                                    data.AddOption("Enchant~it~10~time.~(1600~CPs)", 23);
                                data.AddOption("Leave", 255);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 21:
                    {
                        data.AddText("Currently,~the~Evil~Array~is~holding~" + client.Player.EpicTrojanEvilArrayPoints + "~points~of~magic.~Do~you~want~to~pay~160~CPs~to~enchant~the~array~with~" + GetMapEpicTrojanPoints(client.Player.Map) + "~points~of~magic?");
                        data.AddOption("Yes!~(160~CPs)", 22);
                        data.AddOption("I~haven`t~decided,~yet!");
                        data.FinalizeDialog();
                        break;
                    }
                case 22:
                    {
                        if (client.Player.ConquerPoints < 160)
                        {
                            data.AddText("Sorry,~I~can`t~help~you~if~you~don`t~have~160~CPs.");
                            data.AddOption("Alright.");
                            data.AddAvatar(248);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.InTrade) return;
                            client.Player.ConquerPoints -= 160;

                            client.CreateBoxDialog("You`ve~successfully~enchanted~the~Evil~Array~with~" + GetMapEpicTrojanPoints(client.Player.Map) + "~points~of~magic!");
                            client.Player.EpicTrojanEvilArrayPoints += (uint)GetMapEpicTrojanPoints(client.Player.Map);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "stipple_dhz");
                            client.Player.AddMapEffect(stream, 45, 45, "zf2-e121");
                            if (client.Player.EpicTrojanEvilArrayPoints >= 10000)
                            {
                                client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                                client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                                client.Player.StageEpicTrojanQuest = 20;
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "dgbos_addr");
                                data.AddText("Great~job!~You`ve~enchant~the~Evil~Array~with~10,000~points~of~magic.~Hurry~and~go~find~General~Pak!");
                                data.AddOption("Report~back~to~General~Pak.", 1);
                                data.FinalizeDialog();
                                break;
                            }
                            if (client.Player.Map == 3826)
                            {

                                client.Player.AddMapEffect(stream, 39, 45, "stipple_bdh");
                            }
                            else if (client.Player.Map == 3827)
                            {
                                client.Player.AddMapEffect(stream, 39, 45, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 45, 39, "stipple_bdh");
                            }
                            else if (client.Player.Map == 3828)
                            {
                                //45 51 stipple_bdh

                                client.Player.AddMapEffect(stream, 51, 45, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 45, 39, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 39, 45, "stipple_bdh");
                            }
                            else if (client.Player.Map == 3829)
                            {
                                client.Player.AddMapEffect(stream, 45, 51, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 51, 45, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 45, 39, "stipple_bdh");
                                client.Player.AddMapEffect(stream, 39, 45, "stipple_bdh");
                            }
                            if (client.Player.EpicTrojanEvilArrayPoints >= 1500 && client.Player.Map == 3826)
                            {
                                client.Player.EpicTrojanAbysalStage = 2;
                                client.Teleport(client.Player.X, client.Player.Y, 3827);
                                client.Player.AddMapEffect(stream, 45, 39, "zf2-e280");
                                client.Player.AddMapEffect(stream, 45, 45, "tj");
                            }
                            else if (client.Player.EpicTrojanEvilArrayPoints >= 3500 && client.Player.Map == 3827)
                            {
                                client.Player.EpicTrojanAbysalStage = 3;
                                client.Teleport(client.Player.X, client.Player.Y, 3828);
                                client.Player.AddMapEffect(stream, 051, 045, "zf2-e280");
                                client.Player.AddMapEffect(stream, 45, 45, "tj");
                            }
                            else if (client.Player.EpicTrojanEvilArrayPoints >= 6000 && client.Player.Map == 3828)
                            {
                                client.Player.EpicTrojanAbysalStage = 4;
                                client.Teleport(client.Player.X, client.Player.Y, 3829);
                                client.Player.AddMapEffect(stream, 045, 051, "zf2-e280");
                                client.Player.AddMapEffect(stream, 45, 45, "tj");
                            }
                            else
                            {
                                data.AddText("Currently,~the~Evil~Array~is~holding~" + client.Player.EpicTrojanEvilArrayPoints + "~points~of~magic.~It~needs~10,000~points~of~magic~to~recover~the~seal.\n\n");
                                data.AddText("You~can~enchant~" + GetMapEpicTrojanPoints(client.Player.Map) + "~points~of~magic,~per~time.~If~you~enchant~the~array~10~times,~at~once,~you`ll~uncover~an~extra~Array`s~Eye.\n");
                                data.AddText("~The~added~Array`s~Eye~can~be~enchanted~with~" + Math.Min(300, GetMapEpicTrojanPoints(client.Player.Map) + 50) + "~points~of~magic,~per~time.\n\n");
                                data.AddOption("Enchant~it~1~time.~(160~CPs)", 21);
                                if (client.Player.EpicTrojanEvilArrayPoints < 8650)
                                    data.AddOption("Enchant~it~10~time.~(1600~CPs)", 23);
                                data.AddOption("Leave", 255);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.MonkMisery)]
        public static void MonkMisery(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralPak, 0, 3270);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralPak, 0, 3276);
            switch (Option)
            {
                case 0:
                    {

                        if (client.Player.StageEpicTrojanQuest >= 11)
                        {
                            if (client.Player.StageEpicTrojanQuest >= 20)
                            {
                                if (client.Player.StageEpicTrojanQuest >= 31)
                                {

                                    data.AddText("Ah,~how~dare~you~come~back~for~death!~I`ve~made~a~perfect~plan,~but~you~destroyed~it.");
                                    data.AddText("~I~won`t~let~you~go.");
                                    data.AddOption("Why~you~betrayed?", 3);
                                    data.AddAvatar(248);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("The~Evil~Array~is~getting~weak.~We~have~no~time~to~waste,~or~the~Flame~Devastator~will~break~through~the~array~and~return.");
                                    data.AddOption("I`ll~come~back~later.");
                                    data.AddAvatar(248);
                                    data.FinalizeDialog();
                                }
                            }
                            else
                            {
                                if (client.Player.EpicTrojanEvilArrayPoints >= 10000)
                                {
                                    client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                                    client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                                    client.Player.StageEpicTrojanQuest = 20;
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "dgbos_addr");
                                    data.AddText("Great~job!~You`ve~enchant~the~Evil~Array~with~10,000~points~of~magic.~Hurry~and~go~find~General~Pak!");
                                    data.AddOption("Report~back~to~General~Pak.", 2);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("The~magic~of~the~Evil~Array~is~getting~weak.~We~need~to~enhance~its~power~before~it`s~too~late.~You~know,~the~Flame~Devastator~is~just~behind~the~seal.");
                                    data.AddOption("Enter~the~Evil~Array.", 1);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            data.AddText("The~Evil~Array~is~getting~weak.~We~have~no~time~to~waste,~or~the~Flame~Devastator~will~break~through~the~array~and~return.");
                            data.AddOption("I`ll~come~back~later.");
                            data.AddAvatar(248);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        data.AddText("Why?~For~the~power.~On~the~first~day~I~was~here,~the~Flame~Devastator~and~I~came~to~an~agreement.~I~nourished~him~with~creatures`~souls,");
                        data.AddText("~while~he~would~help~me~take~the~throne,~ruling~Twin~City,~even~the~whole~world.");
                        data.AddOption("Is~that~so~important?", 4);
                        data.AddAvatar(248);
                        data.FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        data.AddText("Yeah,~I~need~power.~You~know,~it~was~me~who~taught~those~Ghost~Reavers~to~be~invisible.");
                        data.AddText("~I`m~the~best~spell~caster,~but~people~envied~me,~and~tried~to~kill~me~here.~I~have~to~disguise~myself~to~survive.");
                        data.AddText("~The~time~to~revenge~has~come!");
                        data.AddOption("You...", 5);
                        data.AddAvatar(248);
                        data.FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        client.Player.AddPick(stream, "MonkMisery(Transforming)", 1);
                        client.Player.MonkMiseryTransforming = 1;
                        client.SendSysMesage("With~a~chilling~roaring,~Monk~Misery~transformed~into~a~disgusting~monster.~Be~careful!");
                        foreach (var npc in client.Player.View.Roles(Role.MapObjectType.Npc))
                            if (npc.UID == id)
                            {
                                Game.MsgFloorItem.MsgItemPacket effect = Game.MsgFloorItem.MsgItemPacket.Create();
                                effect.m_UID = (uint)Game.MsgFloorItem.MsgItemPacket.EffectMonsters.EarthquakeLeftRight;
                                effect.DropType = MsgDropID.Earth;
                                effect.m_X = 82;
                                effect.m_Y = 166;
                                client.Send(stream.ItemPacketCreate(effect));
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, npc.UID, true, "ssch_wlhd_hit");
                            }
                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);
                        break;
                    }
                case 1:
                    {
                        switch (client.Player.EpicTrojanAbysalStage)
                        {
                            case 0: client.Teleport(42, 51, 3825); break;
                            case 1: client.Teleport(42, 51, 3826); break;
                            case 2: client.Teleport(42, 51, 3827); break;
                            case 3: client.Teleport(42, 51, 3828); break;
                            case 4: client.Teleport(42, 51, 3829); break;
                        }
                        client.SendSysMesage("You`ve~entered~in~the~Evil~Array.", MsgMessage.ChatMode.System);
                        break;
                    }
            }

        }
    }
}

