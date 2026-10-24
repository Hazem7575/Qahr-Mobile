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
        [NpcAttribute(NpcID.MysterCheast3)]
        public static void MysterCheast3(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
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
        [NpcAttribute(NpcID.MysterCheast4)]
        public static void MysterCheast4(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
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
        [NpcAttribute(NpcID.MysterCheast5)]
        public static void MysterCheast5(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
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

        [NpcAttribute(NpcID.FairyCloud)]
        public static void FairyCloud(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 1:
                case 0:
                    {
                        data.AddText("Wow, you`re so lucky to enter the Heaven Treasury! Let me introduce you the rules here.")
                           .AddText("~You can open one Mystery Chest in the Treasury, free of charge. Once opened, the chest will disappear. For other chests, you need to use CPs or the Heaven Keys to open them.")
                           .AddText("~Of course, the rewards will be better and better. Oh, if you`re a Jiang Hu fighter, you`ll also get a special reward.");
                        data.AddOption("Learn~about~opening~chests.", 2)
                            .AddOption("Send~me~to~Twin~City.", 6)
                             .AddOption("What`re~in~those~chests?", 4)
                              .AddOption("I~see.", 255)
                            .AddAvatar(233).FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        client.Teleport(Places.TwinCity);
                        break;
                    }
                case 2:
                    {
                        data.AddText("You can open one Mystery Chest for free, once in a day. For the other chests, you need to use CPs.")
               .AddText("~or the Heaven Keys to open. If you decide to pay CPs, the charge is, in order, 5 CPs, 20 CPs, 35 CPs and 100 CPs. While if you have a Heaven Key, ")
               .AddText("~you can open a chest worth 5 - 100 CPs. However, you can use up to 2 Heaven Keys in a day. Each 3 Heaven Key Scraps that you may obtain from the Mystery Chests")
               .AddText("~can be combined into a Heaven Key.");
                        data.AddOption("Learn~about~other~rules.", 1).AddOption("I~see.", 255).AddAvatar(233).FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        data.AddText("1st Time: 60 minutes of EXP, 10 Study Points, 1 Chi Token and 1 Heaven Key Scrap;\n")
                            .AddText("2nd Time: 60 minutes of EXP, 30 Chi Points;\n")
                            .AddText("3rd Time: 60 minutes of EXP, 100 Study Points and 60 Chi Points;\n")
                            .AddText("4th Time: 60 minutes of EXP, 150 Study Points and 100 Chi Points;\n")
                            .AddText("5th Time: 60 minutes of EXP, 200 Study Points and 150 Chi Points.");
                        data.AddOption("What`s~for~Jiang~Hu~fighters.", 5)
                            .AddOption("Learn~about~other~rules.", 1)
                            .AddOption("I~see.", 255).AddAvatar(233).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        data.AddText("1st Time Extra Reward: 1 Talent point;\n")
                         .AddText("2nd Time Extra Reward: 1 Talent point and 1 Free Course;\n")
                         .AddText("3rd Time Extra Reward: 1 Talent point and 1 Free Course;\n")
                         .AddText("4th Time Extra Reward: 1 Talent point, 1 Super Protection Pill and 1 Free Course;\n")
                         .AddText("5th Time Extra Reward: 1 Talent point, 5 Protection Pills, 4 Super Protection Pills and 2 Free Courses;\nPlease note that, you won`t receive Talent points, if your Talent is already full.");
                        data.AddOption("Learn~about~other~rules.", 1)
                            .AddOption("I~see.", 255).AddAvatar(233).FinalizeDialog();

                        break;
                    }

            }
        }

        public static void CheckDesertGuardian(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.DesertGuardian, client.Player.Class, 6049);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (Role.Core.GetDistance(client.Player.X, client.Player.Y, client.OnRemoveNpc.X, client.OnRemoveNpc.Y) > 5)
                                client.CreateBoxDialog("You~are~too~far~away~from~the~Soldier`s~Remains!");
                            else
                                client.Player.AddPick(stream, "Sure", 4);
                        }

                        break;
                    }
            }
        }


        [NpcAttribute(NpcID.MagnoliaEnvoy)]
        public static void MagnoliaEnvoy(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.DailyQuestEnvoy, client.Player.Class, 6014);
            switch (Option)
            {
                case 0:
                    {
                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                             && !client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("I`m here to deliver seeds, and I can also teleport you back to Twin City.");
                            data.AddOption("Claim~1~seed.", 1);

                        }
                        else
                            data.AddText("I don`t have any Magnolia Seeds. You`ll need to wait until tomorrow!");
                        data.AddOption("Back~to~Twin~City.", 9);
                        data.AddOption("How~to~plant~them.", 10);
                        data.AddOption("I`m~just~passing~by.", 255);

                        data.AddAvatar(113).FinalizeDialog();

                        break;
                    }
                case 9:
                    {
                        client.Teleport(Places.TwinCity);
                        break;
                    }
                case 1:
                    {
                        if (client.Player.DailyMagnoliaItemId == 0)
                        {
                            client.Player.DailyMagnoliaChance += 1;
                            client.Player.DailyMagnoliaItemId = 729306;//5
                            client.Player.DailyMagnoliaItemId += (byte)Pool.GetRandom.Next(0, 4);
                        }
                        var DbItem = Pool.ItemsBase[client.Player.DailyMagnoliaItemId];

                        data.AddText("" + DbItem.Name + " \n");
                        data.AddText("You`ve already changed [" + client.Player.DailyMagnoliaChance.ToString() + "] times.\n");
                        data.AddOption("Claim~the~seed.", 2);
                        data.AddOption("Another~seed.~(50~CP)", 3);
                        data.AddOption("Another~seed.~(25~Study~Points)", 4);
                        data.AddOption("Buy~1~Super~seed.~(600~CPs)", 5);
                        data.AddOption("I`ll~think~about~it.", 255);
                        data.AddAvatar(113).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).FinalizeDialog();
                            break;
                        }
                        var DbItem = Pool.ItemsBase[client.Player.DailyMagnoliaItemId];
                        client.Inventory.Add(stream, DbItem.ID);
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        client.CreateBoxDialog("You~received~1~" + DbItem.Name + "!~Go~plant~it~in~the~garden!");

                        break;
                    }
                case 3:
                    {
                        if (client.Player.ConquerPoints >= 50)
                        {
                            if (client.InTrade) return;
                            client.Player.DailyMagnoliaItemId = 0;
                            client.Player.ConquerPoints -= 50;
                            goto case 1;
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough CPs.");
                            data.AddOption("Alright.", 255);
                            data.AddAvatar(113).FinalizeDialog();
                        }

                        break;
                    }
                case 4:
                    {
                        if (client.Player.SubClass.StudyPoints >= 25)
                        {
                            client.Player.DailyMagnoliaItemId = 0;
                            client.Player.SubClass.RemoveStudy(client, 25, stream);
                            goto case 1;
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough StudyPoints.");
                            data.AddOption("Alright.", 255);
                            data.AddAvatar(113).FinalizeDialog();
                        }

                        break;
                    }
                case 5:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).FinalizeDialog();
                            break;
                        }
                        if (client.Player.ConquerPoints >= 600)
                        {
                            if (client.InTrade) return;
                            client.Player.ConquerPoints -= 600;
                            client.Inventory.Add(stream, 729310);
                            client.Player.DailyMagnoliaItemId = 729310;
                            client.Player.QuestGUI.Accept(ActiveQuest, 0);
                            client.CreateBoxDialog("You~received~1~Super~Magnolia~Seed!~Go~plant~it~in~the~garden!");
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough CPs.");
                            data.AddOption("Alright.", 255);
                            data.AddAvatar(113).FinalizeDialog();
                        }

                        break;
                    }
                case 10:
                    {
                        data.AddText("I`ll tell you a secret! Did you know that different seeds will bring you different rewards?")
                            .AddText("~Which kind of seed were you interested in?");
                        data.AddOption("Normal~Magnolia~Seed..", 11)
                            .AddOption("Refined~Magnolia~Seed.", 12)
                            .AddOption("Unique~Magnolia~Seed.", 13)
                            .AddOption("Elite~Magnolia~Seed.", 14)
                            .AddOption("Super~Magnolia~Seed.", 15)
                            .AddOption("Bye.", 255)
                        .AddAvatar(113).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        data.AddText("If your Normal Magnolia Seed becomes a flower, you`ll receive 60 minutes of EXP, 10 Study Points and 1 Chi Token.");
                        data.AddOption("Other~rewards.", 10).AddOption("Leave.", 255);
                        data.AddAvatar(113).FinalizeDialog();

                        break;
                    }
                case 12:
                    {
                        data.AddText("If your Refined Magnolia Seed becomes a flower, you`ll have the chance to receive 90 minutes of EXP, 20 Study Points, 1 Chi Token, and more!");
                        data.AddOption("Other~rewards.", 10).AddOption("Leave.", 255);
                        data.AddAvatar(113).FinalizeDialog();

                        break;
                    }
                case 13:
                    {
                        data.AddText("If your Unique Magnolia Seed becomes a flower, you`ll have the chance to receive 120 minutes of EXP, 50 Study Points, 1 Chi Token, and more!");
                        data.AddOption("Other~rewards.", 10).AddOption("Leave.", 255);
                        data.AddAvatar(113).FinalizeDialog();

                        break;
                    }
                case 14:
                    {
                        data.AddText("If your Elite Magnolia Seed becomes a flower, you`ll have the chance to receive 180 minutes of EXP, 100 Study Points, 1 Chi Token, and more!");
                        data.AddOption("Other~rewards.", 10).AddOption("Leave.", 255);
                        data.AddAvatar(113).FinalizeDialog();

                        break;
                    }
                case 15:
                    {
                        data.AddText("If your Super Magnolia Seed becomes a flower, you`ll have the chance to receive 300 minutes of EXP, 300 Study Points, 1 Chi Token, and more!");
                        data.AddOption("Other~rewards.", 10).AddOption("Leave.", 255);
                        data.AddAvatar(113).FinalizeDialog();

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.DesertGuardian)]
        public static void DesertGuardian(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.DesertGuardian, client.Player.Class, 6049);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("Several days ago, a group of bandits appeared in the Desert, and we failed to defeat them. Many of our soldiers were killed.")
                                .AddText("~I need someone to release the trapped souls of these brave soldiers. Could you please help me? Well, if you`re too busy,")
                                .AddText("~you can give 10 CPs so that I can hire other heroes to help me. Of course, you`ll be handsomely rewarded!");
                            data.AddOption("Let~me~help~you.", 1);
                            data.AddAvatar(101).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You`ve helped us a lot, already! You`d better have a rest. If you want, you can come back tomorrow!");
                            data.AddOption("Alright.", 255);
                            data.AddAvatar(101).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 3))
                        {
                            data.AddText("You have released the souls of some dead soldiers, right? Just a few? Well, thanks for your help! I`ve got something to give you,")
                           .AddText("~for all that you`re doing for us. May those soldiers find peace in the afterlife...");
                            data.AddOption("Thanks!", 255)
                            .AddAvatar(101).FinalizeDialog();
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                            {
                                client.Player.SubClass.AddStudyPoints(client, 50, stream);
                                client.Inventory.AddItemWitchStack(729304, 0, 1, stream);
                                client.GainExpBall(1200, true, Role.Flags.ExperienceEffect.angelwing);
                                client.CreateBoxDialog("You`ve~completed~the~quest:~Release~the~Souls!~You~received~50~Study~Points,~120~minutes~of~EXP~and~1~Chi~Token!");

                            }
                            break;
                        }

                        data.AddText("Eh? Could it be there are still decent men in the world? It`s very kind of you to offer! If you can find 8 Soldier`s Remains and release their souls,")
                           .AddText("~I`ll reward you handsomely for all your effort. You have no idea what this will mean to the families. Take care, out there.");
                        data.AddOption("Okay!~I`ll~go~now!", 4)
                            .AddOption("Pay~700~CPs.", 3)
                            .AddAvatar(101).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (client.Player.ConquerPoints >= 700)
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory");
                                data.AddOption("Let me check.", 255).FinalizeDialog();
                                break;
                            }
                            if (client.InTrade) return;
                            client.Player.ConquerPoints -= 700;
                            if (!client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                client.Player.QuestGUI.Accept(ActiveQuest, 0);
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                            {
                                client.Player.SubClass.AddStudyPoints(client, 50, stream);
                                client.Inventory.AddItemWitchStack(729304, 0, 1, stream);
                                client.GainExpBall(1200, true, Role.Flags.ExperienceEffect.angelwing);
                                client.CreateBoxDialog("You`ve~completed~the~quest:~Release~the~Souls!~You~received~50~Study~Points,~120~minutes~of~EXP~and~1~Chi~Token!");

                            }
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough CPs.")
                              .AddOption("Okay.", 255).AddAvatar(84)
                .FinalizeDialog();
                        }

                        break;
                    }
                case 4:
                    {
                        data.AddText("You`ve~accepted~the~quest:~Release~the~Souls.~Find~at~least~8~Soldier`s~Remains~in~the~Desert!");
                        data.AddOption("Yes", 5)
                            .AddOption("No", 255)
                            .AddAvatar(101).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        client.Player.QuestGUI.SendAutoPatcher(1000, 631, 500, 0);
                        break;
                    }
            }
        }
        public static uint GetNextSpitiBead(uint spiritbead)
        {
            if (spiritbead == 0)
                return 729611;
            if (spiritbead == 729611)
                return 729611 + 1;
            if (spiritbead == 729612)
                return spiritbead + 1;
            if (spiritbead == 729613)
                return spiritbead + 1;
            if (spiritbead == 729614)
                return 729703;
            return 729703;
        }
        public static uint GetDailySpiritBeadKills(Client.GameClient client)
        {

            switch (client.Player.DailySpiritBeadItem)
            {
                case 729611:
                    return 2500;
                case 729612:
                    return 2000;
                case 729613:
                    return 1500;
                case 729614:
                    return 1000;
                case 729703:
                    return 500;
            }
            return 0;

        }
        [NpcAttribute(NpcID.DailyNormalSpiritBead)]
        public static void DailyNormalSpiritBead(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.DailyQuestEnvoy, client.Player.Class, 2375);
            Dialog data = new Dialog(client, stream);
            if (!client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                client.Player.QuestGUI.Accept(ActiveQuest, 0);
            switch (Option)
            {
                case 0:
                    {
                        if (GetDailySpiritBeadKills(client) == 0)
                            return;
                        if (client.Player.DailySpiritBeadCount >= GetDailySpiritBeadKills(client))
                        {
                            data.AddText("You`ve collect enough spirits. Do you want to claim 120 minutes of EXP, 7 hours of Blessing, 30 Study Points and a Chi Token, now?")
                              .AddOption("Yes,~of~course!", 3)
                              .AddOption("No,~not~now.", 255).FinalizeDialog();
                            break;
                        }
                        data.AddText("This bead needs to absorb " + GetDailySpiritBeadKills(client) + " spirits. Current spirits: " + client.Player.DailySpiritBeadCount + ".")
                               .AddOption("Fill~it~up.~(500CPs)", 1)
                               .AddOption("Okay.", 255).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("This is a Normal Spirit Bead. Once fully charged, you`ll receive 120 minutes of EXP, 7 hours of Blessing, 30 Study Points and a Chi Token.")
                              .AddText("~Are you sure you want to pay 500 CPs to get the reward, directly?")
                             .AddOption("Yeah,~please!", 2)
                             .AddOption("Wait,~I~haven`t~decided!", 255).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId) || client.Player.DailySpiritBeadCount >= GetDailySpiritBeadKills(client))
                        {
                            client.Player.DailySpiritBeadCount = 0;
                            client.Inventory.Remove(client.Player.DailySpiritBeadItem, 1, stream);
                            client.Inventory.AddItemWitchStack(729304, 0, 1, stream);

                            client.GainExpBall(1200, true, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.SubClass.AddStudyPoints(client, 30, stream);
                            client.Player.AddHeavenBlessing(stream, 60 * 60 * 7);
                            client.CreateBoxDialog("You received 120 minutes of EXP, 7 hours of Blessing, 30 Study Points and 1 Chi Token.");

                            client.Player.DailySpiritBeadItem = GetNextSpitiBead(client.Player.DailySpiritBeadItem);
                        }

                        break;
                    }
                case 2:
                    {
                        if (client.Player.ConquerPoints >= 500)
                        {
                            //  if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                            {
                                if (client.InTrade) return;
                                client.Player.ConquerPoints -= 500;
                                goto case 3;
                            }
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough CPs.");
                            data.AddOption("I~see.", 255).AddAvatar(84).FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.DailyRefinedSpiritBead)]
        public static void DailyRefinedSpiritBead(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.DailyQuestEnvoy, client.Player.Class, 2375);
            if (!client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                client.Player.QuestGUI.Accept(ActiveQuest, 0);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (GetDailySpiritBeadKills(client) == 0)
                            return;
                        if (client.Player.DailySpiritBeadCount >= GetDailySpiritBeadKills(client))
                        {
                            data.AddText("You`ve collect enough spirits. Do you want to claim 180 minutes of EXP, 10 hours of Blessing, 40 Study Points and 1 Chi Token, now?")
                              .AddOption("Yes,~of~course!", 3)
                              .AddOption("No,~not~now.", 255).FinalizeDialog();
                            break;
                        }
                        data.AddText("This bead needs to absorb " + GetDailySpiritBeadKills(client) + " spirits. Current spirits: " + client.Player.DailySpiritBeadCount + ".")
                               .AddOption("Fill~it~up.~(500CPs)", 1)
                               .AddOption("Okay.", 255).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("This is a Normal Spirit Bead. Once fully charged, you`ll receive 180 minutes of EXP, 10 hours of Blessing, 40 Study Points and 1 Chi Token")
                               .AddText("~Are you sure you want to pay 500 CPs to get the reward, directly?")
                             .AddOption("Yeah,~please!", 2)
                             .AddOption("Wait,~I~haven`t~decided!", 255).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId) || client.Player.DailySpiritBeadCount >= GetDailySpiritBeadKills(client))
                        {
                            client.Player.DailySpiritBeadCount = 0;
                            client.Inventory.Remove(client.Player.DailySpiritBeadItem, 1, stream);
                            client.Inventory.AddItemWitchStack(729304, 0, 1, stream);

                            client.GainExpBall(1800, true, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.SubClass.AddStudyPoints(client, 40, stream);
                            client.Player.AddHeavenBlessing(stream, 60 * 60 * 10);
                            client.CreateBoxDialog("You received 180 minutes of EXP, 10 hours of Blessing, 40 Study Points and 1 Chi Token.");

                            client.Player.DailySpiritBeadItem = GetNextSpitiBead(client.Player.DailySpiritBeadItem);
                        }

                        break;
                    }
                case 2:
                    {
                        if (client.Player.ConquerPoints >= 500)
                        {
                            //   if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                            {
                                if (client.InTrade) return;
                                client.Player.ConquerPoints -= 500;
                                goto case 3;
                            }
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough CPs.");
                            data.AddOption("I~see.", 255).AddAvatar(84).FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.DailyUniqueSpiritBead)]
        public static void DailyUniqueSpiritBead(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.DailyQuestEnvoy, client.Player.Class, 2375);
            if (!client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                client.Player.QuestGUI.Accept(ActiveQuest, 0);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (GetDailySpiritBeadKills(client) == 0)
                            return;
                        if (client.Player.DailySpiritBeadCount >= GetDailySpiritBeadKills(client))
                        {
                            data.AddText("You`ve collect enough spirits. Do you want to claim 240 minutes of EXP, 15 hours of Blessing, 50 Study Points and 1 Chi Token, now?")
                              .AddOption("Yes,~of~course!", 3)
                              .AddOption("No,~not~now.", 255).FinalizeDialog();
                            break;
                        }
                        data.AddText("This bead needs to absorb " + GetDailySpiritBeadKills(client) + " spirits. Current spirits: " + client.Player.DailySpiritBeadCount + ".")
                               .AddOption("Fill~it~up.~(500CPs)", 1)
                               .AddOption("Okay.", 255).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("This is a Normal Spirit Bead. Once fully charged, you`ll receive 240 minutes of EXP, 15 hours of Blessing, 50 Study Points and 1 Chi Token")
                             .AddText("~Are you sure you want to pay 500 CPs to get the reward, directly?")
                             .AddOption("Yeah,~please!", 2)
                             .AddOption("Wait,~I~haven`t~decided!", 255).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId) || client.Player.DailySpiritBeadCount >= GetDailySpiritBeadKills(client))
                        {
                            client.Player.DailySpiritBeadCount = 0;
                            client.Inventory.Remove(client.Player.DailySpiritBeadItem, 1, stream);
                            client.Inventory.AddItemWitchStack(729304, 0, 1, stream);

                            client.GainExpBall(2300, true, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.SubClass.AddStudyPoints(client, 50, stream);
                            client.Player.AddHeavenBlessing(stream, 60 * 60 * 15);
                            client.CreateBoxDialog("You received 240 minutes of EXP, 15 hours of Blessing, 50 Study Points and 1 Chi Token.");

                            client.Player.DailySpiritBeadItem = GetNextSpitiBead(client.Player.DailySpiritBeadItem);
                        }

                        break;
                    }
                case 2:
                    {
                        if (client.Player.ConquerPoints >= 500)
                        {
                            //    if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                            {
                                if (client.InTrade) return;
                                client.Player.ConquerPoints -= 500;
                                goto case 3;
                            }
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough CPs.");
                            data.AddOption("I~see.", 255).AddAvatar(84).FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.DailyEliteSpiritBead)]
        public static void DailyEliteSpiritBead(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.DailyQuestEnvoy, client.Player.Class, 2375);
            Dialog data = new Dialog(client, stream);
            if (!client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                client.Player.QuestGUI.Accept(ActiveQuest, 0);
            switch (Option)
            {
                case 0:
                    {
                        if (GetDailySpiritBeadKills(client) == 0)
                            return;
                        if (client.Player.DailySpiritBeadCount >= GetDailySpiritBeadKills(client))
                        {
                            data.AddText("You`ve collect enough spirits. Do you want to claim 300 minutes of EXP, 24 hours of Blessing, 80 Study Points and 1 Chi Token, now?")
                              .AddOption("Yes,~of~course!", 3)
                              .AddOption("No,~not~now.", 255).FinalizeDialog();
                            break;
                        }
                        data.AddText("This bead needs to absorb " + GetDailySpiritBeadKills(client) + " spirits. Current spirits: " + client.Player.DailySpiritBeadCount + ".")
                               .AddOption("Fill~it~up.~(500CPs)", 1)
                               .AddOption("Okay.", 255).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("This is a Normal Spirit Bead. Once fully charged, you`ll receive 300 minutes of EXP, 24 hours of Blessing, 80 Study Points and 1 Chi Token.")
                                .AddText("~Are you sure you want to pay 500 CPs to get the reward, directly?")
                             .AddOption("Yeah,~please!", 2)
                             .AddOption("Wait,~I~haven`t~decided!", 255).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId) || client.Player.DailySpiritBeadCount >= GetDailySpiritBeadKills(client))
                        {
                            client.Player.DailySpiritBeadCount = 0;
                            client.Inventory.Remove(client.Player.DailySpiritBeadItem, 1, stream);
                            client.Inventory.AddItemWitchStack(729304, 0, 1, stream);

                            client.GainExpBall(3000, true, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.SubClass.AddStudyPoints(client, 80, stream);
                            client.Player.AddHeavenBlessing(stream, 60 * 60 * 24);
                            client.CreateBoxDialog("You received 300 minutes of EXP, 24 hours of Blessing, 80 Study Points and 1 Chi Token.");

                            client.Player.DailySpiritBeadItem = GetNextSpitiBead(client.Player.DailySpiritBeadItem);
                        }

                        break;
                    }
                case 2:
                    {
                        if (client.Player.ConquerPoints >= 500)
                        {
                            //if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                            {
                                if (client.InTrade) return;
                                client.Player.ConquerPoints -= 500;
                                goto case 3;
                            }
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough CPs.");
                            data.AddOption("I~see.", 255).AddAvatar(84).FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.DailySuperSpiritBead)]
        public static void DailySuperSpiritBead(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.DailyQuestEnvoy, client.Player.Class, 2375);
            Dialog data = new Dialog(client, stream);
            if (!client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                client.Player.QuestGUI.Accept(ActiveQuest, 0);
            switch (Option)
            {
                case 0:
                    {
                        if (GetDailySpiritBeadKills(client) == 0)
                            return;
                        if (client.Player.DailySpiritBeadCount >= GetDailySpiritBeadKills(client))
                        {
                            data.AddText("You`ve collect enough spirits. Do you want to claim 360 minutes of EXP, 48 hours of Blessing, 100 Study Points and 1 Chi Token, now?")
                              .AddOption("Yes,~of~course!", 3)
                              .AddOption("No,~not~now.", 255).FinalizeDialog();
                            break;
                        }
                        data.AddText("This bead needs to absorb " + GetDailySpiritBeadKills(client) + " spirits. Current spirits: " + client.Player.DailySpiritBeadCount + ".")
                               .AddOption("Fill~it~up.~(500CPs)", 1)
                               .AddOption("Okay.", 255).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("This is a Normal Spirit Bead. Once fully charged, you`ll receive 360 minutes of EXP, 48 hours of Blessing, 100 Study Points and 1 Chi Token.")
                             .AddText("~Are you sure you want to pay 500 CPs to get the reward, directly?")
                            .AddOption("Yeah,~please!", 2)
                            .AddOption("Wait,~I~haven`t~decided!", 255).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId) || client.Player.DailySpiritBeadCount >= GetDailySpiritBeadKills(client))
                        {
                            client.Player.DailySpiritBeadCount = 0;
                            client.Inventory.Remove(client.Player.DailySpiritBeadItem, 1, stream);
                            client.Inventory.AddItemWitchStack(729304, 0, 1, stream);

                            client.GainExpBall(3600, true, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.SubClass.AddStudyPoints(client, 100, stream);
                            client.Player.AddHeavenBlessing(stream, 60 * 60 * 48);
                            client.CreateBoxDialog("You received 360 minutes of EXP, 48 hours of Blessing, 100 Study Points and 1 Chi Token.");

                            client.Player.DailySpiritBeadItem = GetNextSpitiBead(client.Player.DailySpiritBeadItem);
                        }

                        break;
                    }
                case 2:
                    {
                        if (client.Player.ConquerPoints >= 500)
                        {
                            //   if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                            {
                                if (client.InTrade) return;
                                client.Player.ConquerPoints -= 500;
                                goto case 3;
                            }
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough CPs.");
                            data.AddOption("I~see.", 255).AddAvatar(84).FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.HeavenlyMaster)]
        public static void HeavenlyMaster(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.HeavenlyMaster, client.Player.Class, 6126);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Every day when you successfully challenged the Tower of Mystery, you can claim")
                                .AddText("~a Chi Token, 200 Champion Points, 3 Exorcism Tokens and 100 Study Points from me.");
                        data.AddOption("Take~me~to~the~tower.", 2);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Claim~my~rewards.", 30);
                        data.AddOption("Exchange~for~Exorcism~Pack.", 3)
                    .AddOption("Leave.", 255)
                    .AddAvatar(67)
                    .FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        // [QuestIntegration] was Teleport(410, 354, 1002) = Twin City centre, not the tower (dailies C-09)
                        client.Teleport(85, 75, 4020);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "movego");
                        break;
                    }
                case 30:
                    {
                        if (!client.Inventory.HaveSpace(2))
                        {
                            data.AddText("Please make 2 more spaces in your inventory.");
                            data.AddOption("I~see.", 255)
                                .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1))
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                            {

                                client.SendSysMesage("Congratulations! You've received a Chi Token, 3 Exorcism Tokens, 100 Study Points and 200 Champion Points!");
                                client.Player.AddChampionPoints(200);
                                client.Player.SubClass.AddStudyPoints(client, 100, stream);
                                client.Inventory.AddItemWitchStack(711211, 0, 3, stream);
                                client.Inventory.AddItemWitchStack(729304, 0, 1, stream);
                            }
                        }
                        else
                        {

                            data.AddText("You haven`t completed the challenge of the Tower of Mystery, and you can`t claim the rewards, otherwise you wold like to pay 1500 CPs or a Mystery Amulet.");
                            data.AddOption("Pay 1500 CPs for reward.", 11)
                                .AddOption("Swap MysteryAmulet for reward.", 10)
                                .AddOption("I haven`t decided, yet!", 255)
                                .AddAvatar(67).FinalizeDialog();
                            //You haven`t completed the challenge of the Tower of Mystery, and you can`t claim the rewards.
                        }

                        break;
                    }
                case 11:
                    {
                        if (client.Player.ConquerPoints >= 1500)
                        {
                            if (!client.Inventory.HaveSpace(2))
                            {
                                data.AddText("Please make 2 more spaces in your inventory.");
                                data.AddOption("I~see.", 255)
                                    .AddAvatar(67).FinalizeDialog();
                                break;
                            }
                            client.Player.QuestGUI.Accept(ActiveQuest, 0);
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                            {
                                if (client.InTrade) return;
                                client.Player.ConquerPoints -= 1500;
                                client.SendSysMesage("Congratulations! You've received a Chi Token, 3 Exorcism Tokens, 100 Study Points and 200 Champion Points!");
                                client.Player.AddChampionPoints(200);
                                client.Player.SubClass.AddStudyPoints(client, 100, stream);
                                client.Inventory.AddItemWitchStack(711211, 0, 3, stream);
                                client.Inventory.AddItemWitchStack(729304, 0, 1, stream);
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough CPs to directly get the rewards.");
                            data.AddOption("Alright.", 255)
                                .AddAvatar(67).FinalizeDialog();
                        }
                        break;
                    }
                case 10:
                    {
                        if (client.Inventory.Contain(3600012, 1))
                        {
                            if (!client.Inventory.HaveSpace(2))
                            {
                                data.AddText("Please make 2 more spaces in your inventory.");
                                data.AddOption("I~see.", 255)
                                    .AddAvatar(67).FinalizeDialog();
                                break;
                            }
                            client.Player.QuestGUI.Accept(ActiveQuest, 0);
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                            {
                                client.Inventory.Remove(3600012, 1, stream);
                                client.SendSysMesage("Congratulations! You've received a Chi Token, 3 Exorcism Tokens, 100 Study Points and 200 Champion Points!");
                                client.Player.AddChampionPoints(200);
                                client.Player.SubClass.AddStudyPoints(client, 100, stream);
                                client.Inventory.AddItemWitchStack(711211, 0, 3, stream);
                                client.Inventory.AddItemWitchStack(729304, 0, 1, stream);
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have Mystery amulet to directly get the rewards.");
                            data.AddOption("Alright.", 255)
                                .AddAvatar(67).FinalizeDialog();
                        }
                        break;
                    }

                case 3:
                    {
                        data.AddText("If you have 3 Exorcism Tokens, you can exchange for an Exorcism Pack. And if you have 9 tokens, you can get a")
                            .AddText("~Golden Exorcism Pack! Which one do you want to claim?");
                        data.AddOption("Exorcism~Pack.", 20)
                            .AddOption("Golden~Exorcism~Pack.", 21)
                            .AddOption("I`ll~think~about~it.", 255)
                            .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 20:
                    {
                        if (client.Inventory.Contain(711211, 3))
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory.");
                                data.AddOption("I~see.", 255)
                                    .AddAvatar(67).FinalizeDialog();
                                break;
                            }
                            client.Inventory.Remove(711211, 3, stream);
                            client.Inventory.Add(stream, 720948);
                            client.SendSysMesage("You've successfully exchanged 3 Exorcism Tokens for an Exorcism Pack!", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have 3 Exorcism Tokens.");
                            data.AddOption("I~see.", 255)
                                .AddAvatar(67).FinalizeDialog();
                        }

                        break;
                    }
                case 21:
                    {
                        if (client.Inventory.Contain(711211, 9))
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory.");
                                data.AddOption("I~see.", 255)
                                    .AddAvatar(67).FinalizeDialog();
                                break;
                            }
                            client.Inventory.Remove(711211, 9, stream);
                            client.Inventory.Add(stream, 729594);
                            client.SendSysMesage("You've successfully exchanged 3 Exorcism Tokens for an Gold Exorcism Pack!", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have 9 Exorcism Tokens.");
                            data.AddOption("I~see.", 255)
                                .AddAvatar(67).FinalizeDialog();
                        }

                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.EartMaster)]
        public static void EartMaster(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.HeavenlyMaster, client.Player.Class, 6126);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Welcome, my friend. Are you sure you want to release the Thrilling Spook now, or would you rather return to the TwinCity");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted) == false
                            || !client.Map.ContainMobID(4212, client.Player.DynamicID))
                            data.AddOption("Release~a~Thrilling~Spook.", 1);
                        data.AddOption("Send~me~to~the~TwinCity.", 2)
                        .AddOption("Nothing.", 255)
                        .AddAvatar(67)
                        .FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        client.Teleport(Places.TwinCity);
                        break;
                    }
                case 1:
                    {
                        data.AddText("I just want to be sure. Shall I release the Thrilling Spook, now?");
                        data.AddOption("Okay.", 3)
                            .AddOption("Not~now.", 255)
                            .AddAvatar(67)
                            .FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (client.Player.Level < 70)
                        {
                            data.AddText("Sorry,~you~cannot~release the Thrilling~Spook ~before~your~Level~is~70.~Please~train~harder.")
                                .AddOption("Oops.", 255)
                                .AddAvatar(67).FinalizeDialog();
                            return;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted) == false
                             || !client.Map.ContainMobID(4212, client.Player.DynamicID))
                        {
                            client.Player.QuestGUI.Accept(ActiveQuest, 0);
                            client.CreateBoxDialog("You~killed~a~Thrilling~Spook!~Speak~with~the~Heavenly~Master~to~claim~a~reward.");
                            Server.AddMapMonster(stream, client.Map, 4212, client.Player.X, client.Player.Y, 10, 10, 1, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.EarthquakeAndNight);
                        }

                        break;
                    }
            }
        }
        public static void ItemArena(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("STR_ID_tNewPKReward[3314674][Text111]@@");
                        dialog.AddText("STR_ID_tNewPKReward[3314674][Text112]@@");
                        dialog.AddOption("STR_ID_tNewPKReward[3314674][Option112]@@", 2);
                        dialog.AddOption("STR_ID_tNewPKReward[3314674][Option111]@@", 1);
                        dialog.AddOption("STR_ID_tNewPKReward[3314674][Option113]@@", 3);
                        dialog.AddAvatar(2700);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(id, 1, 1) || client.Inventory.Contain(id, 1))
                        {
                            client.Inventory.RemoveStackItem(id, 1, stream);
                            uint[] Items = new uint[] { 3009001, 3009101, 730002, 3303247, 4050001, 4040001, 3006915, 3314252, 3314253, 3314254 };
                            uint ItemID = Items[(uint)Pool.GetRandom.Next(0, Items.Length)];
                            if (ItemID == 3009001)
                            {
                                client.Inventory.Add(stream, ItemID, 0, 2, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false, Role.Flags.ItemEffect.None, true, "", 2 * 24 * 60 * 60);//7 Ã˜Â§Ã™Å Ã˜Â§Ã™â€¦
                                client.SendSysMesage("You received " + Pool.ItemsBase[ItemID].Name + " (B) * 2.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, true, true);
                            }
                            if (ItemID == 3009101)
                            {
                                client.Inventory.AddItemWitchStack(3009101, 0, 1, stream);
                                client.SendSysMesage("You received " + Pool.ItemsBase[ItemID].Name + " (B).", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, true, true);
                            }
                            if (ItemID == 730002)
                            {
                                client.Inventory.Add(stream, ItemID, 0, 2, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true, Role.Flags.ItemEffect.None, true, "", 2 * 24 * 60 * 60);//7 Ã˜Â§Ã™Å Ã˜Â§Ã™â€¦
                                client.SendSysMesage("You received " + Pool.ItemsBase[ItemID].Name + " (B) * 2 .", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, true, true);
                            }
                            if (ItemID == 3303247 || ItemID == 4050001 || ItemID == 4040001)
                            {
                                if (ItemID == 4050001 || ItemID == 4040001)
                                {
                                    client.Inventory.AddItemWitchStack(ItemID, 0, 10, stream, true);
                                    client.SendSysMesage("You received " + Pool.ItemsBase[ItemID].Name + " (B) * 10.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, true, true);
                                }
                                else
                                {
                                    client.Inventory.Add(stream, ItemID, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false, Role.Flags.ItemEffect.None, true);
                                    client.SendSysMesage("You received " + Pool.ItemsBase[ItemID].Name + " (B).", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, true, true);
                                }
                            }
                            if (ItemID == 3314252 || ItemID == 3314253 || ItemID == 3314254)
                            {
                                client.Inventory.AddItemWitchStack(ItemID, 0, 2, stream, true);
                                client.SendSysMesage("You received " + Pool.ItemsBase[ItemID].Name + " (B) * 2.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, true, true);
                            }

                        }

                        break;
                    }
                case 2:
                    {
                        if (client.Player.Money >= 100000)
                        {
                            client.Player.Money -= 100000;
                            if (client.Inventory.Contain(id, 1, 1) || client.Inventory.Contain(id, 1))
                            {
                                client.Inventory.RemoveStackItem(id, 1, stream);
                                for (int i = 0; i < 2; i++)
                                {
                                    uint[] Items = new uint[] { 3009001, 3009101, 730002, 3303247, 4050001, 4040001, 3006915, 3314252, 3314253, 3314254 };
                                    uint ItemID = Items[(uint)Pool.GetRandom.Next(0, Items.Length)];
                                    if (ItemID == 3009001)
                                    {
                                        client.Inventory.Add(stream, ItemID, 0, 2, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false, Role.Flags.ItemEffect.None, true, "", 2 * 24 * 60 * 60);//7 Ã˜Â§Ã™Å Ã˜Â§Ã™â€¦
                                        client.SendSysMesage("You received " + Pool.ItemsBase[ItemID].Name + " (B) * 2.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, true, true);
                                    }
                                    if (ItemID == 3009101)
                                    {
                                        client.Inventory.AddItemWitchStack(3009101, 0, 1, stream);
                                        client.SendSysMesage("You received " + Pool.ItemsBase[ItemID].Name + " (B).", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, true, true);
                                    }
                                    if (ItemID == 730002)
                                    {
                                        client.Inventory.Add(stream, ItemID, 0, 2, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true, Role.Flags.ItemEffect.None, true, "", 2 * 24 * 60 * 60);//7 Ã˜Â§Ã™Å Ã˜Â§Ã™â€¦
                                        client.SendSysMesage("You received " + Pool.ItemsBase[ItemID].Name + " (B) * 2 .", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, true, true);
                                    }
                                    if (ItemID == 3303247 || ItemID == 4050001 || ItemID == 4040001)
                                    {
                                        if (ItemID == 4050001 || ItemID == 4040001)
                                        {
                                            client.Inventory.AddItemWitchStack(ItemID, 0, 10, stream, true);
                                            client.SendSysMesage("You received " + Pool.ItemsBase[ItemID].Name + " (B) * 10.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, true, true);
                                        }
                                        else
                                        {
                                            client.Inventory.Add(stream, ItemID, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false, Role.Flags.ItemEffect.None, true);
                                            client.SendSysMesage("You received " + Pool.ItemsBase[ItemID].Name + " (B).", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, true, true);
                                        }
                                    }
                                    if (ItemID == 3314252 || ItemID == 3314253 || ItemID == 3314254)
                                    {
                                        client.Inventory.AddItemWitchStack(ItemID, 0, 2, stream, true);
                                        client.SendSysMesage("You received " + Pool.ItemsBase[ItemID].Name + " (B) * 2.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, true, true);
                                    }
                                }
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        goto case 0;

                    }
            }
        }
        public static void ItemArenaTwo(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("STR_ID_tNewPKReward[3314675][Text111]@@");
                        dialog.AddOption("STR_ID_tNewPKReward[3314675][Option111]@@", 1);
                        dialog.AddOption("STR_ID_tNewPKReward[3314675][Option112]@@", 2);
                        dialog.AddOption("STR_ID_tNewPKReward[3314675][Option113]@@", 3);
                        dialog.AddAvatar(918);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(id, 1, 1) || client.Inventory.Contain(id, 1))
                        {
                            client.Inventory.RemoveStackItem(id, 1, stream);
                            client.Inventory.Add(stream, 3009001, 0, 2, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false, Role.Flags.ItemEffect.None, true, "", 2 * 24 * 60 * 60);//7 Ã˜Â§Ã™Å Ã˜Â§Ã™â€¦
                            client.Inventory.Add(stream, 3009001, 0, 2, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false, Role.Flags.ItemEffect.None, true, "", 2 * 24 * 60 * 60);//7 Ã˜Â§Ã™Å Ã˜Â§Ã™â€¦
                            client.SendSysMesage("You received " + Pool.ItemsBase[3009001].Name + " (B).", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, true, true);
                        }

                        break;
                    }
                case 2:
                    {

                        if (client.Inventory.Contain(id, 1, 1) || client.Inventory.Contain(id, 1))
                        {
                            client.Inventory.RemoveStackItem(id, 1, stream);
                            client.Inventory.Add(stream, 730002, 0, 2, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true, Role.Flags.ItemEffect.None, true, "", 2 * 24 * 60 * 60);//7 Ã˜Â§Ã™Å Ã˜Â§Ã™â€¦
                            client.Inventory.Add(stream, 730002, 0, 2, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true, Role.Flags.ItemEffect.None, true, "", 2 * 24 * 60 * 60);//7 Ã˜Â§Ã™Å Ã˜Â§Ã™â€¦
                            client.SendSysMesage("You received " + Pool.ItemsBase[730002].Name + " (B).", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, true, true);

                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.Contain(id, 1, 1) || client.Inventory.Contain(id, 1))
                        {
                            client.Inventory.RemoveStackItem(id, 1, stream);
                            client.Inventory.AddItemWitchStack(3009101, 0, 1, stream, true);
                            client.SendSysMesage("You received " + Pool.ItemsBase[3009101].Name + " (B).", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, true, true);

                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.DailyQuestEnvoy)]
        public static void DailyQuestEnvoy(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.DailyQuestEnvoy, client.Player.Class, 6329);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DailyQuestEnvoy, client.Player.Class, 6245);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DailyQuestEnvoy, client.Player.Class, 6390);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DesertGuardian, client.Player.Class, 6049);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DailyQuestEnvoy, client.Player.Class, 6366);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DailyQuestEnvoy, client.Player.Class, 6014);
            var ActiveQuest7 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DailyQuestEnvoy, client.Player.Class, 2375);
            var ActiveQuest8 = Database.QuestInfo.GetFinishQuest((uint)NpcID.HeavenlyMaster, client.Player.Class, 6126);


            switch (Option)
            {
                case 0:
                    {
                        data.Text("Queen of Evil has led a devil army to invade the Dragon Island, aiming to reactivate the Twisting Hell Array.  Go break the devil invasion to win EXP boost, Realm Quest Token and Daily Quest Token.");
                        data.Text("~What`s more, you can come back and start over, then you can take these daily tasks again.");

                        data.Option("Head~to~the~Dragon~Island.", 155);
                        data.AddOption("Bright~Fortune.~(" + (string)((client.Player.ClaimTowerAmulets == 0) ? "[Lv.110]" : "completed") + ")", 121);
                        data.AddOption("Open~Treasure~Chest~(" + GetQuestModes(client, ActiveQuest.MissionId, 50) + ")", 1);
                        data.AddOption("Everything~Has~A~Price~(" + GetQuestModes(client, ActiveQuest2.MissionId, 60) + ")", 3);
                        data.AddOption("Fellow~Fighters~(" + GetQuestModes(client, ActiveQuest4.MissionId, 70) + ")", 20);
                        data.AddOption("Rare~Materials.~(" + GetQuestModes(client, ActiveQuest5.MissionId, 75) + ")", 30);
                        data.AddOption("Magnolias~All~Around~(" + GetQuestModes(client, ActiveQuest6.MissionId, 80) + ")", 40);
                        data.AddOption("Tower~of~Mystery.~[(" + GetQuestModes(client, ActiveQuest8.MissionId, 90) + ")", 60);
                        data.AddOption("Spirit~Beads~(" + GetQuestModes(client, ActiveQuest7.MissionId, 100) + ")", 50);
                        data.Option("Do~the~quests~again!~(270~CPs)", 156);
                        data.Option("Leave.", 255);
                        data.Avatar(281);
                        data.FinalizeDialog();
                        break;
                    }
                case 155:
                    {
                        if (client.Player.Reborn == 2)
                        {
                            client.Teleport(95, 409, 10137);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "moveback" });
                        }
                        else
                        {
                            data.AddText("STR_ID_tUndergroundPalace[NotUpLevelText]@@");
                            data.AddOption("I,See.", 255);
                            data.AddAvatar(55).FinalizeDialog();

                        }
                        break;
                    }
                case 156:
                    {
                        if (client.Player.ConquerPoints >= 270)
                        {
                            client.Player.ConquerPoints -= 270;

                            //---- reset Quests
                            client.Player.QuestGUI.RemoveQuest(6390);
                            client.Player.QuestGUI.RemoveQuest(6329);
                            client.Player.QuestGUI.RemoveQuest(6245);
                            client.Player.QuestGUI.RemoveQuest(6049);
                            client.Player.QuestGUI.RemoveQuest(6366);
                            client.Player.QuestGUI.RemoveQuest(6014);
                            client.Player.QuestGUI.RemoveQuest(2375);
                            client.Player.QuestGUI.RemoveQuest(6126);
                            client.Player.DailyHeavenChance = client.Player.DailyMagnoliaChance
                                = client.Player.DailyMagnoliaItemId
                                = client.Player.DailyHeavenChance = client.Player.DailySpiritBeadCount = client.Player.DailyRareChance = 0;
                            client.Player.TowerOfMysterychallenge = 3;
                            client.Player.TOMChallengeToday = 0;
                            client.Player.TowerOfMysteryChallengeFlag = 0;
                            client.Player.TOMSelectChallengeToday = 0;
                            client.Player.ClaimTowerAmulets = 0;
                            client.Player.TOMClaimTeamReward = 0;
                            client.Player.TOMRefreshReward = 0;
                        }
                        else
                        {
                            data.Text("Are you kidding me? You don`t have enough CPs! Talk to me again when you have the dough!");

                            data.Option("Alright.", 255);
                            data.Avatar(281);
                            data.FinalizeDialog();

                        }
                        break;
                    }
                case 121:
                    {
                        if (client.Player.ClaimTowerAmulets == 0)
                        {
                            client.Teleport(88, 362, 3998);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "movego");
                        }
                        else
                        {

                            data.AddText("You`ve already made a big harvest in the Fiery Dragon region today. If you want to hunt for more treasures, please come to me tomorrow.");
                            data.AddOption("Learn~more~about~daily~quests.", 122)
                                .AddOption("Enter~the~Fiery~Dragon~region.", 123).AddAvatar(84).FinalizeDialog();
                        }
                        break;
                    }
                case 122:
                    {
                        goto case 0;
                    }
                case 123:
                    {
                        client.Teleport(88, 362, 3998);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "movego");
                        break;
                    }
                case 120:
                    {
                        client.SendSysMesage("Every day if you complete the cross-server missions first, you'll win many special tokens which enable you can directly complete corresponding daily quests. Don`t miss the change!", MsgMessage.ChatMode.System);
                        client.Player.QuestGUI.SendAutoPatcher(1002, 410, 354, 17400);
                        break;
                    }
                case 60:
                    {
                        if (client.Player.Level < 90)
                        {
                            data.AddText("I`m sorry, but you have to reach Level 90 to accept the quest.");
                            data.AddOption("I~see", 255).AddAvatar(84).FinalizeDialog();
                            break;
                        }

                        data.AddText("In the Wind Plain, there is the Tower of Mystery sealing the most cruel devils. For heroes who defeat those devils to energize the Mystery Pearl,")
       .AddText("~the Bright Tribe promises to give them wonderful rewards which include <tip color=0xfff05050 desc=collect#9#fragments#to#get#270#CPs#(B)>Blazing CP Fragments, </tip>??")
       .AddText("<tip color=0xfff05050 desc=randomly#give#Luxury#Protection#Pack,#Golden#Training#Pack,#Plentiful#Talent#Pack,#etc.>Concerted Effort Pack,</tip>")
       .AddText("~Chi Points and Study Points. If you`re willing to help, please talk to the Heavenly Master.");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest8.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Head~to~see~Heavenly~Master.", 61);
                        data.AddOption("Leave.", 255)
                        .AddAvatar(84).FinalizeDialog();

                        break;
                    }
                case 61:
                    {
                        client.Teleport(Places.TwinCity);
                        break;
                    }
                case 50:
                    {

                        if (client.Player.Level < 100)
                        {
                            data.AddText("I`m sorry, but you have to reach Level 100 to accept the quest.");
                            data.AddOption("I~see", 255).AddAvatar(84).FinalizeDialog();
                            break;
                        }

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.DailySpiritBeadCount >= GetDailySpiritBeadKills(client))
                            {
                                client.Inventory.AddItemWitchStack(729304, 0, 1, stream);
                                switch (client.Player.DailySpiritBeadItem)
                                {
                                    case 729611:
                                        {
                                            client.GainExpBall(1200, true, Role.Flags.ExperienceEffect.angelwing);
                                            client.Player.SubClass.AddStudyPoints(client, 30, stream);
                                            client.Player.AddHeavenBlessing(stream, 60 * 60 * 7);
                                            client.CreateBoxDialog("You received 120 minutes of EXP, 7 hours of Blessing, 30 Study Points and 1 Chi Token.");

                                            break;
                                        }
                                    case 729612:
                                        {
                                            client.GainExpBall(1800, true, Role.Flags.ExperienceEffect.angelwing);
                                            client.Player.SubClass.AddStudyPoints(client, 40, stream);
                                            client.Player.AddHeavenBlessing(stream, 60 * 60 * 10);
                                            client.CreateBoxDialog("You received 180 minutes of EXP, 10 hours of Blessing, 40 Study Points and 1 Chi Token.");

                                            break;
                                        }
                                    case 729613:
                                        {
                                            client.GainExpBall(2400, true, Role.Flags.ExperienceEffect.angelwing);
                                            client.Player.SubClass.AddStudyPoints(client, 50, stream);
                                            client.Player.AddHeavenBlessing(stream, 60 * 60 * 15);
                                            client.CreateBoxDialog("You received 240 minutes of EXP, 15 hours of Blessing, 50 Study Points and 1 Chi Token.");

                                            break;
                                        }
                                    case 729614:
                                        {
                                            client.GainExpBall(3000, true, Role.Flags.ExperienceEffect.angelwing);
                                            client.Player.SubClass.AddStudyPoints(client, 80, stream);
                                            client.Player.AddHeavenBlessing(stream, 60 * 60 * 24);
                                            client.CreateBoxDialog("You received 300 minutes of EXP, 24 hours of Blessing, 80 Study Points and 1 Chi Token.");

                                            break;
                                        }
                                    case 729703:
                                        {
                                            client.GainExpBall(3600, true, Role.Flags.ExperienceEffect.angelwing);
                                            client.Player.SubClass.AddStudyPoints(client, 100, stream);
                                            client.Player.AddHeavenBlessing(stream, 60 * 60 * 48);
                                            client.CreateBoxDialog("You received 360 minutes of EXP, 48 hours of Blessing, 100 Study Points and 1 Chi Token.");

                                            break;
                                        }
                                }
                                client.Player.DailySpiritBeadItem = GetNextSpitiBead(client.Player.DailySpiritBeadItem);
                                break;
                            }

                        }
                        if (client.Player.DailySpiritBeadItem == 0)
                            client.Player.DailySpiritBeadItem = GetNextSpitiBead(0);
                        var DbItem = Pool.ItemsBase[client.Player.DailySpiritBeadItem];
                        var DbItem2 = Pool.ItemsBase[GetNextSpitiBead(client.Player.DailySpiritBeadItem)];

                        data.AddText("You can claim a " + DbItem.Name + ", today. If you complete the task, I`ll give you a " + DbItem2.Name + ", tomorrow.\nBy the way, you can do this job with other monster elimination tasks, at the same time.");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddOption("Claim~a~bead.", 51);
                            data.AddOption("Get~a~" + DbItem2.Name + ".~(100CP)", 52);
                            data.AddOption("Complete~the~task.~(500CPs)", 53);
                        }
                        data.AddOption("What~are~the~beads?", 54);
                        data.AddOption("Leave.", 255)
                          .AddAvatar(84).FinalizeDialog();


                        break;
                    }
                case 54:
                    {


                        data.AddText("The Spirit Beads have 5 different qualities: Normal, Refined, Unique, Elite and Super. When you kill monsters, the bead will absorb the ")
                            .AddText("~monster spirits. When it`s fully charged, you can use the bead to claim a reward. If you complete the task, you`ll be able to claim")
                            .AddText("~a better bead, next time. As for the Super Spirit Bead, the next one will go back to a Normal bead.")
                            .AddOption("How~many~spirits~are~needed?", 55)
                            .AddOption("What`re~the~rewards?", 56)
                            .AddOption("How~to~calculate~the~spirits?", 57)
                            .AddOption("I~want~to~know~something~else.", 50)
                            .AddOption("I~see.", 255)
                            .AddAvatar(84).FinalizeDialog();

                        break;
                    }
                case 57:
                    {
                        data.AddText("Killing a monster below Level 70 will add 1 spirit. Level 70 - 99, 2 spirits. Level 100 - 119, 3 spirits. Level 120 - 140, 4 spirits.")
                              .AddText("~A monster leader will provide 10 spirits. As for the Terato Dragon, Snow Banshee, Snow Banshee Soul, Sword Master and Thrilling Spook 3,")
                              .AddText("~you`ll receive 1,000 spirits, if you`re able to kill anyone of them!")
                            .AddOption("I~want~to~know~something~else.", 54)
                            .AddOption("I~see.", 255)
                            .AddAvatar(84).FinalizeDialog();

                        break;
                    }
                case 56:
                    {
                        data.AddText("Normal Spirit Bead: 120 minutes of EXP, 7 hours of Blessing, 30 Study Points and 1 Chi Token.\n")
                              .AddText("Refined Spirit Bead: 180 minutes of EXP, 10 hours of Blessing, 40 Study Points and 1 Chi Token.\n")
                              .AddText("Unique Spirit Bead: 240 minutes of EXP, 15 hours of Blessing, 50 Study Points and 1 Chi Token.\n")
                              .AddText("Elite Spirit Bead: 300 minutes of EXP, 24 hours of Blessing, 80 Study Points and 1 Chi Token.\n")
                              .AddText("Super bead: 360 minutes of EXP, 48 hours of Blessing, 100 Study Points and 1 Chi Token.\n")
                            .AddOption("I~want~to~know~something~else.", 54)
                            .AddOption("I~see.", 255)
                            .AddAvatar(84).FinalizeDialog();

                        break;
                    }
                case 55:
                    {
                        data.AddText("A Normal Spirit Bead can absorb 2,500 monster spirits. A Refined bead needs 2,000 spirits to fill it up,")
                              .AddText("~1,500 spirits for the Unique bead, 1,000 spirits for the Elite bead, and 500 spirits for a Super bead.")
                              .AddText("~a better bead, next time. As for the Super Spirit Bead, the next one will go back to a Normal bead.")
                            .AddOption("I~want~to~know~something~else.", 54)
                            .AddOption("I~see.", 255)
                            .AddAvatar(84).FinalizeDialog();

                        break;
                    }
                case 51:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).AddAvatar(84).FinalizeDialog();
                            break;
                        }
                        if (client.Player.DailySpiritBeadItem != 0)
                        {
                            if (client.Inventory.Contain(client.Player.DailySpiritBeadItem, 1))
                            {
                                data.AddText("You~already~have~a~Spirit~Bead~in~your~inventory.");
                                data.AddOption("Ok", 255).AddAvatar(84).FinalizeDialog();
                                break;
                            }
                        }
                        client.Player.QuestGUI.Accept(ActiveQuest7, 0);
                        var DbItem = Pool.ItemsBase[client.Player.DailySpiritBeadItem];
                        client.Player.DailySpiritBeadItem = DbItem.ID;
                        client.Inventory.Add(stream, DbItem.ID);
                        client.CreateBoxDialog("You received a " + DbItem.Name + "!");

                        break;
                    }
                case 53:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).AddAvatar(84).FinalizeDialog();
                            break;
                        }

                        if (client.Player.ConquerPoints >= 500)
                        {
                            if (client.InTrade) return;
                            client.Player.ConquerPoints -= 500;
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest7.MissionId))
                            {
                                client.Inventory.AddItemWitchStack(729304, 0, 1, stream);
                                switch (client.Player.DailySpiritBeadItem)
                                {
                                    case 729611:
                                        {
                                            client.GainExpBall(1200, true, Role.Flags.ExperienceEffect.angelwing);
                                            client.Player.SubClass.AddStudyPoints(client, 30, stream);
                                            client.Player.AddHeavenBlessing(stream, 60 * 60 * 7);
                                            client.CreateBoxDialog("You received 120 minutes of EXP, 7 hours of Blessing, 30 Study Points and 1 Chi Token.");

                                            break;
                                        }
                                    case 729612:
                                        {
                                            client.GainExpBall(1800, true, Role.Flags.ExperienceEffect.angelwing);
                                            client.Player.SubClass.AddStudyPoints(client, 40, stream);
                                            client.Player.AddHeavenBlessing(stream, 60 * 60 * 10);
                                            client.CreateBoxDialog("You received 180 minutes of EXP, 10 hours of Blessing, 40 Study Points and 1 Chi Token.");

                                            break;
                                        }
                                    case 729613:
                                        {
                                            client.GainExpBall(2400, true, Role.Flags.ExperienceEffect.angelwing);
                                            client.Player.SubClass.AddStudyPoints(client, 50, stream);
                                            client.Player.AddHeavenBlessing(stream, 60 * 60 * 15);
                                            client.CreateBoxDialog("You received 240 minutes of EXP, 15 hours of Blessing, 50 Study Points and 1 Chi Token.");

                                            break;
                                        }
                                    case 729614:
                                        {
                                            client.GainExpBall(3000, true, Role.Flags.ExperienceEffect.angelwing);
                                            client.Player.SubClass.AddStudyPoints(client, 80, stream);
                                            client.Player.AddHeavenBlessing(stream, 60 * 60 * 24);
                                            client.CreateBoxDialog("You received 300 minutes of EXP, 24 hours of Blessing, 80 Study Points and 1 Chi Token.");

                                            break;
                                        }
                                    case 729703:
                                        {
                                            client.GainExpBall(3600, true, Role.Flags.ExperienceEffect.angelwing);
                                            client.Player.SubClass.AddStudyPoints(client, 100, stream);
                                            client.Player.AddHeavenBlessing(stream, 60 * 60 * 48);
                                            client.CreateBoxDialog("You received 360 minutes of EXP, 48 hours of Blessing, 100 Study Points and 1 Chi Token.");

                                            break;
                                        }
                                }
                            }
                            client.Player.DailySpiritBeadItem = GetNextSpitiBead(client.Player.DailySpiritBeadItem);
                        }
                        else
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Sorry, but you don`t have enough CPs.");
                                data.AddOption("I~see.", 255).AddAvatar(84).FinalizeDialog();

                                break;
                            }
                        }
                        break;
                    }
                case 52:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).AddAvatar(84).FinalizeDialog();
                            break;
                        }
                        if (client.Player.DailySpiritBeadItem != 0)
                        {
                            if (client.Inventory.Contain(client.Player.DailySpiritBeadItem, 1))
                            {
                                data.AddText("You~already~have~a~Spirit~Bead~in~your~inventory.");
                                data.AddOption("Ok", 255).AddAvatar(84).FinalizeDialog();
                                break;
                            }
                        }
                        if (client.Player.ConquerPoints >= 100)
                        {
                            if (client.InTrade) return;
                            client.Player.QuestGUI.Accept(ActiveQuest7, 0);
                            client.Player.ConquerPoints -= 100;
                            client.Player.DailySpiritBeadItem = GetNextSpitiBead(client.Player.DailySpiritBeadItem);
                            var DbItem = Pool.ItemsBase[client.Player.DailySpiritBeadItem];
                            client.Player.DailySpiritBeadItem = DbItem.ID;
                            client.Inventory.Add(stream, DbItem.ID);
                            client.CreateBoxDialog("You received a " + DbItem.Name + "!");
                        }
                        else
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Sorry, but you don`t have enough CPs.");
                                data.AddOption("I~see.", 255).AddAvatar(84).FinalizeDialog();
                                break;
                            }
                        }


                        break;
                    }
                case 40:
                    {
                        if (client.Player.Level < 80)
                        {
                            data.AddText("I`m sorry, but you have to reach Level 80 to accept the quest.");
                            data.AddOption("I~see", 255).AddAvatar(84).FinalizeDialog();
                            break;
                        }
                        data.AddText("If you help the Magnolia Envoy plant a beautiful magnolia in the Garden, you`ll receive some EXP, Study Points, 1 Chi Token, and")
                            .AddText("~even some precious items like a Dragon Ball and Super Gems!");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Visit~the~Magnolia~Envoy.", 41);
                        data.AddOption("Leave.", 255)
                          .AddAvatar(84).FinalizeDialog();

                        break;
                    }
                case 41:
                    {
                        client.Teleport(106, 108, 1806);
                        break;
                    }
                case 30:
                    {
                        if (client.Player.Level < 75)
                        {
                            data.AddText("I`m sorry, but you have to reach Level 75 to accept the quest.");
                            data.AddOption("I~see", 255).AddAvatar(84).FinalizeDialog();
                            break;
                        }
                        data.AddText("Hey, I`m looking for some able men to collect items for me! You know, I always pay a high price for what I want.");
                        data.AddOption("Count~me~in!", 31).AddOption("Sorry,~I`m~busy~now.", 255).AddAvatar(84).FinalizeDialog();

                        break;
                    }
                case 31:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("You`ve finished today! Please come again tomorrow!");
                            data.AddOption("Okay.", 255).FinalizeDialog();
                            break;
                        }

                        data.AddText("     \t\t\t\t\t\t\t\t\tRare Materials\n")
                            .AddText("\t\t\t\t-----------------------\n")
                            .AddText("\t\t\t\tQuest Quality: [" + client.Player.DailyRareChance + 1 + "-Star]\n")
                            .AddText("\t\t\t\tItem: [QuestChanceA]; Amount: 2\n")
                            .AddText("\t\t\t\tChanged Times:  [ " + client.Player.DailyRareChance + " ]\n");
                        if (client.Player.DailyRareChance == 1)
                            data.AddText("\t\t\t\tRewards: 30 Study Points, 120 minutes of EXP and 1 Chi Token.\n");
                        else if (client.Player.DailyRareChance == 2)
                            data.AddText("50 Study Points, 180 minutes of EXP and 1 Chi Token.\n");
                        else if (client.Player.DailyRareChance == 3)
                            data.AddText("70 Study Points, 240 minutes of EXP, 1 Chi Token and 1 Protection Pill.");
                        data.AddText("\t\t\t\t-----------------------");
                        data.AddOption("Hand~in~the~item.", 32)
                            .AddOption("Change~the~required~item.", 33)
                            .AddOption("Directly~complete~it.", 34)
                            .AddOption("Sorry,~I`m~busy~now.", 255).AddAvatar(84).FinalizeDialog();


                        break;
                    }
                case 34:
                    {
                        data.AddText("If you would like to pay 50 CPs, you can directly claim the rewards without collecting the items for me.");
                        data.AddOption("Pay~50~CPs~to~complete~it.", 35)
                            .AddOption("I`ll~think~about~it.", 255).AddAvatar(84).FinalizeDialog();

                        break;
                    }
                case 35:
                    {
                        if (client.Player.ConquerPoints >= 50)
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory");
                                data.AddOption("Let me check.", 255).AddAvatar(84).FinalizeDialog();
                                break;
                            }
                            if (!client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                client.Player.QuestGUI.Accept(ActiveQuest5, 0);

                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest5.MissionId))
                            {
                                if (client.InTrade) return;
                                client.Player.ConquerPoints -= 50;
                                client.CreateBoxDialog("You received 70 Study Points, 240 minutes of EXP, 1 Chi Token and 1 Protection Pill.");
                                client.GainExpBall(2400, true, Role.Flags.ExperienceEffect.angelwing);
                                client.Player.SubClass.AddStudyPoints(client, 70, stream);
                                client.Inventory.AddItemWitchStack(729304, 0, 1, stream);
                            }
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough CPs.");
                            data.AddOption("Okay.", 255).AddAvatar(84).FinalizeDialog();
                        }

                        break;
                    }
                case 33:
                    {
                        if (client.Player.DailyRareChance == 3)
                        {
                            data.AddText("You`ve finished today! Please come again tomorrow!");
                            data.AddOption("Okay.", 255).FinalizeDialog();
                            break;
                        }
                        if (client.Player.RacePoints >= 200)
                        {
                            client.Player.RacePoints -= 200;
                            client.Player.SendUpdate(stream, client.Player.RacePoints, MsgUpdate.DataType.RaceShopPoints);
                            client.Player.DailyRareChance += 1;
                            data.AddText("You are good. Now you have the item.");
                            data.AddOption("Thanks", 255).AddAvatar(84).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("I won`t change my mind if you don`t have 200 Horse Racing Points!");
                            data.AddOption("Alright.", 255).AddAvatar(84).FinalizeDialog();
                        }

                        break;
                    }
                case 32:
                    {
                        if (client.Player.DailyRareChance > 0)
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory");
                                data.AddOption("Let me check.", 255).AddAvatar(84).FinalizeDialog();
                                break;
                            }
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest5.MissionId))
                            {
                                if (client.Player.DailyRareChance == 1)
                                {
                                    client.GainExpBall(1200, true, Role.Flags.ExperienceEffect.angelwing);
                                    client.CreateBoxDialog("You received 30 Study Points, 120 minutes of EXP and 1 Chi Token.");
                                    client.Player.SubClass.AddStudyPoints(client, 30, stream);
                                    client.Inventory.AddItemWitchStack(729304, 0, 1, stream);
                                }
                                else if (client.Player.DailyRareChance == 2)
                                {
                                    client.CreateBoxDialog("You received 50 Study Points, 180 minutes of EXP and 1 Chi Token.");
                                    client.GainExpBall(1800, true, Role.Flags.ExperienceEffect.angelwing);
                                    client.Player.SubClass.AddStudyPoints(client, 50, stream);
                                    client.Inventory.AddItemWitchStack(729304, 0, 1, stream);
                                }
                                else if (client.Player.DailyRareChance == 3)
                                {
                                    client.CreateBoxDialog("You received 70 Study Points, 240 minutes of EXP, 1 Chi Token and 1 Protection Pill.");
                                    client.GainExpBall(2400, true, Role.Flags.ExperienceEffect.angelwing);
                                    client.Player.SubClass.AddStudyPoints(client, 70, stream);
                                    client.Inventory.AddItemWitchStack(729304, 0, 1, stream);
                                }
                            }
                        }
                        else
                        {
                            data.AddText("Are you kidding me? You don`t have the items I want!");
                            data.AddOption("I`ll~bring~you~some!", 255).AddAvatar(84).FinalizeDialog();
                        }

                        break;
                    }
                case 20:
                    {
                        if (client.Player.Level < 70)
                        {
                            data.AddText("I`m sorry, but you have to reach Level 70 to accept the quest.");
                            data.AddOption("I~see", 255).AddAvatar(84).FinalizeDialog();
                            break;
                        }
                        data.AddText("Help the Desert Guardian release the souls of 8 soldiers in the Desert. Once completed, you`ll receive")
                        .AddText("~120 minutes of EXP, 50 Study Points and 1 Chi Token.");
                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Go~to~the~Desert.", 21);
                        data.AddOption("Leave.", 255).AddAvatar(84).FinalizeDialog();

                        break;
                    }
                case 21:
                    {
                        client.Teleport(539, 646, 1000);
                        break;
                    }
                case 10:
                    {
                        if (client.Player.Level < 110 && client.Player.Reborn == 0)
                        {
                            data.AddText("I`m sorry, but you have to reach Level 110 to accept the quest.");
                            data.AddOption("I~see", 255).AddAvatar(84).FinalizeDialog();
                            break;
                        }

                        data.AddText("Trust me, I can never over praise the Heaven Treasury. It`s the top place that keeps fabulous treasures.")
                            .AddText("~If you`ve reached Level 110 or been reborn, I`ll give you a chance to enter that amazing land.");
                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Take~me~to~Heaven~Treasury!", 11);
                        data.AddOption("I`m~just~passing~by.", 255).AddAvatar(84).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.CreateBoxDialog("You~entered~the~Heaven~Treasury!~Go~pick~a~Mystery~Chest!");
                        client.Teleport(61, 53, 9972);


                        break;
                    }
                case 1:
                    {
                        if (client.Player.Level < 50)
                        {
                            data.AddText("I`m sorry, but you have to reach Level 50 to accept the quest.");
                            data.AddOption("I~see", 255).AddAvatar(84).FinalizeDialog();
                            break;
                        }
                        data.AddText("If you worship the leaders of the Supreme Guilds in the Ancestral Hall, you`ll receive some rewards and a Chi Token. If the Guild Leaders are touched by your sincerity,");
                        data.AddText("~you`ll receive 200 Study Points and 20 CPs!");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Cool!~Send~me~there!", 2)
                                .AddOption("Awesome!", 255);
                        else
                            data.AddOption("Check~other~quests.", 255);
                        data.AddAvatar(84).FinalizeDialog();


                        break;
                    }
                case 2:
                    {
                        client.Teleport(Places.TwinCity);
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }
                case 3:
                    {
                        if (client.Player.Level < 60)
                        {
                            data.AddText("I`m sorry, but you have to reach Level 60 to accept the quest.");
                            data.AddOption("I~see", 255).AddAvatar(84).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("When you`re killing monsters, you`ll have the chance to get the items I need. If you hand in the items I need according to")
                                .AddText("~the list, you`ll be handsomely rewarded.");
                            data.AddOption("What`s~the~reward?", 7).AddOption("Leave.", 255).AddAvatar(84).FinalizeDialog();
                            break;
                        }
                        data.AddText("\t\t\t\t\t\t\t\t\t\tItem List\n")
                            .AddText("\t\t\t??????????????????????????????????????????????\n")
                            .AddText("\t\t\tItem: [BirdClaw]\n")
                            .AddText("\t\t\tAmount: [25]\n")
                            .AddText("\t\t\tMonster: [Birdman][BirdmanL88]\n")
                            .AddText("\t\t\tLocation: [Island]\n")
                            .AddText("\t\t\tChanged times: [1]\n")
                            .AddText("\t\t\t??????????????????????????????????????????????\n")
                         .AddOption("Hand~in~items.", 4)
                         .AddOption("Pay~50~CPs~to~complete~it.", 5)
                         .AddOption("Directly~complete~the~quest.", 110)
                         .AddOption("Claim~a~list.", 6)
                         .AddOption("What`s~the~reward?", 7)
                         .AddAvatar(84)
                        .FinalizeDialog();

                        break;
                    }
                case 110:
                    {
                        data.AddText("If you think it`s too troublesome to collect these materials, you can give me 50 CPs")
                            .AddText("~or a Collection Order to directly complete the quest and claim the reward.")
  .AddOption("Use~a~[Collection~Order].", 111)
  .AddOption("Directly~complete.~(50~CPs).", 112)
  .AddOption("I`ll~think~about~it.", 255)
  .AddAvatar(226).FinalizeDialog();
                        break;
                    }
                case 112:
                    {

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            break;
                        if (client.Player.ConquerPoints >= 50)
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory");
                                data.AddOption("Let me check.", 255).AddAvatar(226).FinalizeDialog();
                                break;
                            }
                            client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                            {
                                if (client.InTrade) return;
                                client.Player.ConquerPoints -= 50;
                                client.GainExpBall(600, true, Role.Flags.ExperienceEffect.bless);
                                client.Inventory.Add(stream, 729304);
                                client.Player.SubClass.AddStudyPoints(client, 50, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "eidolon");
                                client.CreateBoxDialog("You~received~600~minutes~of~EXP,~50~Study~Points~and~1~Chi~Token!");
                            }
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough CPs.");
                            data.AddOption("I~see.", 255).AddAvatar(226).FinalizeDialog();
                        }
                        break;
                    }
                case 111:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            break;
                        if (client.Inventory.Contain(3008731, 1))
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory");
                                data.AddOption("Let me check.", 255).AddAvatar(226).FinalizeDialog();
                                break;
                            }
                            client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                            {
                                client.Inventory.Remove(3008731, 1, stream);
                                client.GainExpBall(600, true, Role.Flags.ExperienceEffect.bless);
                                client.Inventory.Add(stream, 729304);
                                client.Player.SubClass.AddStudyPoints(client, 50, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "eidolon");
                                client.CreateBoxDialog("You~received~600~minutes~of~EXP,~50~Study~Points~and~1~Chi~Token!");
                            }
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have [Collection~Order] Token.");
                            data.AddOption("I~see.", 255).AddAvatar(226).FinalizeDialog();
                        }
                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.Contain(729094, 25))
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                            {
                                client.Inventory.Remove(729094, 25, stream);

                                data.AddText("Well done! Here is your reward.");
                                data.AddOption("Thanks.", 255).AddAvatar(84).FinalizeDialog();

                                uint MinutesExp = 0;
                                if (client.Player.Level >= 60 && client.Player.Level <= 80)
                                    MinutesExp = 180;
                                else if (client.Player.Level >= 81 && client.Player.Level <= 100)
                                    MinutesExp = 240;
                                if (client.Player.Level >= 101 && client.Player.Level <= 110)
                                    MinutesExp = 300;
                                else if (client.Player.Level >= 111 && client.Player.Level <= 120)
                                    MinutesExp = 360;
                                else if (client.Player.Level >= 121 && client.Player.Level <= 130)
                                    MinutesExp = 480;
                                else if (client.Player.Level >= 131)
                                    MinutesExp = 600;

                                client.Inventory.Remove(729087, 1, stream);
                                client.GainExpBall(MinutesExp / 60 * 600, true, Role.Flags.ExperienceEffect.angelwing);
                                client.CreateBoxDialog("You~received~" + MinutesExp + "~minutes~of~EXP,~50~Study~Points~and~1~Chi~Token!");

                                client.Inventory.AddItemWitchStack(729304, 0, 1, stream);
                                client.Player.SubClass.AddStudyPoints(client, 50, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "eidolon");
                            }
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have 25 BirdClaw. Hurry up!")
    .AddOption("Okay.", 255)
    .AddAvatar(226).FinalizeDialog();
                        }

                        break;
                    }
                case 7:
                    {
                        data.AddText("Level 60 - 80: 180 minutes of EXP, 50 Study Points and a Chi Token\n")
                            .AddText("Level 81 - 100: 240 minutes of EXP, 50 Study Points and a Chi Token\n")
                            .AddText("Level 101 - 110: 300 minutes of EXP, 50 Study Points and a Chi Token\n")
                            .AddText("Level 111 - 120: 360 minutes of EXP, 50 Study Points and a Chi Token\n")
                            .AddText("Level 121 - 130: 480 minutes of EXP, 50 Study Points and a Chi Token\n")
                            .AddText("Level 131+: 600 minutes of EXP, 50 Study Points and a Chi Token\n")
                              .AddOption("Leave.", 255).AddAvatar(84)
                .FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.Contain(729087, 1) == false)
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory");
                                data.AddOption("Let me check.", 255).FinalizeDialog();
                                break;
                            }
                            client.Inventory.Add(stream, 729087);
                            client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        }
                        else
                        {
                            data.AddText("Sorry, but you already had a list in your inventory.")
    .AddOption("I~forgot.", 255)
    .AddAvatar(226).FinalizeDialog();
                        }

                        break;
                    }
                case 5:
                    {
                        if (client.Player.ConquerPoints >= 50)
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory");
                                data.AddOption("Let me check.", 255).FinalizeDialog();
                                break;
                            }
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                            {
                                if (client.InTrade) return;
                                uint MinutesExp = 0;
                                if (client.Player.Level >= 60 && client.Player.Level <= 80)
                                    MinutesExp = 180;
                                else if (client.Player.Level >= 81 && client.Player.Level <= 100)
                                    MinutesExp = 240;
                                if (client.Player.Level >= 101 && client.Player.Level <= 110)
                                    MinutesExp = 300;
                                else if (client.Player.Level >= 111 && client.Player.Level <= 120)
                                    MinutesExp = 360;
                                else if (client.Player.Level >= 121 && client.Player.Level <= 130)
                                    MinutesExp = 480;
                                else if (client.Player.Level >= 131)
                                    MinutesExp = 600;

                                client.Inventory.Remove(729087, 1, stream);
                                client.GainExpBall(MinutesExp / 60 * 600, true, Role.Flags.ExperienceEffect.angelwing);
                                client.CreateBoxDialog("You~received~" + MinutesExp + "~minutes~of~EXP,~50~Study~Points~and~1~Chi~Token!");
                                client.Player.ConquerPoints -= 50;
                                client.Inventory.AddItemWitchStack(729304, 0, 1, stream);
                                client.Player.SubClass.AddStudyPoints(client, 50, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "eidolon");
                            }
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough CPs.")
                                .AddOption("Okay.", 255).AddAvatar(84)
                  .FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.GoodManLiu)]
        public static void GoodManLiu(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Thanks for coming to the Ancestral Hall. They`re all the greatest Guild Leaders of all time!")
                       .AddOption("How~to~worship?", 2).AddOption("Back~to~Twin~City.", 3).AddOption("Indeed!", 255).AddAvatar(47)
                      .FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        data.AddText("You just need to click on the incense burners in front of statues to worship Robin~, iEatBabies and ~*~PureHeart~*~. Each day, you only have one chance to worship one of them.")
                        .AddOption("Worship~xHolic.", 4).AddOption("Worship~Robin~.", 5).AddOption("Worship~*~PureHeart~*~.", 6).AddOption("Got~it.", 255)
                        .AddAvatar(47)
                            .FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        client.Teleport(Places.TwinCity);
                        break;
                    }
                case 4:
                    {
                        data.AddText("xHolic is the leader of the 2nd Supreme Guild MakeMeFamous. Worshipping xHolic will bring you 60 minutes` EXP, 50 Study Points and 1 Chi Token.")
                     .AddOption("I~see.", 255)
                      .AddAvatar(47)
                          .FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        data.AddText("Robin~ is the leader of the 1st Supreme Guild SkillzDatKillz. Worshipping Robin~ will bring you 5 hours` Blessing, 60 minutes` EXP and 1 Chi Token.")
                     .AddOption("I~see.", 255)
                      .AddAvatar(47)
                          .FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        data.AddText("~*~PureHeart~*~ is the leader of the 3rd Supreme Guild World?hH??aven. Worshipping ~*~PureHeart~*~ will briing you 5 hours` Blessing, 50 Study Points and 1 Chi Token.")
                     .AddOption("I~see.", 255)
                      .AddAvatar(47)
                          .FinalizeDialog();

                        break;
                    }
            }
        }


        [NpcAttribute(NpcID.Cense3)]
        public static void Cense3(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        client.CreateBoxDialog("Failed~to~worship~HuangDi.~Please~go~to~the~front~of~the~HuangDi~statue~to~worship.");

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Censer2)]
        public static void Censer2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        client.CreateBoxDialog("Failed~to~worship~Nuwa~.~Please~go~to~the~front~of~the~Nuwa~statue~to~worship.");

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Censer1)]
        public static void Censer1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.DailyQuestEnvoy, client.Player.Class, 6329);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Worshipping ~YanDi~ will briing you 5 hours` Blessing, 50 Study Points and 1 Chi Token. Continue?");
                        data.AddOption("Yes.", 1);
                        data.AddOption("No.", 255).FinalizeDialog();

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
                            client.Player.AddPick(stream, "Sure!", 2);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "accession1");
                            client.CreateBoxDialog("You~received~5~hours~Blessing,~50~Study~Points~and~1~Chi~Token~from~YanDi~.");
                            client.Inventory.AddItemWitchStack(729304, 0, 1, stream);
                            client.Player.SubClass.AddStudyPoints(client, 50, stream);
                            client.Player.AddHeavenBlessing(stream, (int)(5 * 60 * 60));
                        }
                        else
                        {
                            data.AddText("You`ve worshipped today! Please come again tomorrow!");
                            data.AddOption("Okay.", 255).FinalizeDialog();
                        }

                        break;
                    }
            }
        }

        #region DailyQuestLevel50
        [NpcAttribute(NpcID.HeavenlyTreasureChest)]
        public static void HeavenlyTreasureChest(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.DailyQuestEnvoy, client.Player.Class, 6329);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Worshipping ~YanDi~ will briing you 5 hours` Blessing, 50 Study Points and 1 Chi Token. Continue?");
                        data.AddOption("Yes.", 1);
                        data.AddOption("No.", 255).FinalizeDialog();

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
                            client.Player.AddPick(stream, "Sure!", 2);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "accession1");
                            client.CreateBoxDialog("You~received~5~hours~Blessing,~50~Study~Points~and~1~Chi~Token~from~YanDi~.");
                            client.Inventory.AddItemWitchStack(729304, 0, 1, stream);
                            client.Player.SubClass.AddStudyPoints(client, 50, stream);
                            client.Player.AddHeavenBlessing(stream, (int)(5 * 60 * 60));
                        }
                        else
                        {
                            data.AddText("You`ve worshipped today! Please come again tomorrow!");
                            data.AddOption("Okay.", 255).FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.MythicTreasureChest)]
        public static void MythicTreasureChest(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.DailyQuestEnvoy, client.Player.Class, 6329);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Worshipping ~YanDi~ will briing you 5 hours` Blessing, 50 Study Points and 1 Chi Token. Continue?");
                        data.AddOption("Yes.", 1);
                        data.AddOption("No.", 255).FinalizeDialog();

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
                            client.Player.AddPick(stream, "Sure!", 2);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "accession1");
                            client.CreateBoxDialog("You~received~5~hours~Blessing,~50~Study~Points~and~1~Chi~Token~from~YanDi~.");
                            client.Inventory.AddItemWitchStack(729304, 0, 1, stream);
                            client.Player.SubClass.AddStudyPoints(client, 50, stream);
                            client.Player.AddHeavenBlessing(stream, (int)(5 * 60 * 60));
                        }
                        else
                        {
                            data.AddText("You`ve worshipped today! Please come again tomorrow!");
                            data.AddOption("Okay.", 255).FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.FairyTreasureChest)]
        public static void FairyTreasureChest(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.DailyQuestEnvoy, client.Player.Class, 6329);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Worshipping ~YanDi~ will briing you 5 hours` Blessing, 50 Study Points and 1 Chi Token. Continue?");
                        data.AddOption("Yes.", 1);
                        data.AddOption("No.", 255).FinalizeDialog();

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
                            client.Player.AddPick(stream, "Sure!", 2);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "accession1");
                            client.CreateBoxDialog("You~received~5~hours~Blessing,~50~Study~Points~and~1~Chi~Token~from~YanDi~.");
                            client.Inventory.AddItemWitchStack(729304, 0, 1, stream);
                            client.Player.SubClass.AddStudyPoints(client, 50, stream);
                            client.Player.AddHeavenBlessing(stream, (int)(5 * 60 * 60));
                        }
                        else
                        {
                            data.AddText("You`ve worshipped today! Please come again tomorrow!");
                            data.AddOption("Okay.", 255).FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        #endregion
        [NpcAttribute(NpcID.BanditBoss)]
        public static void BanditBoss(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.IntelligenceAgent, client.Player.Class, 1116);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PhoenixCastellan, client.Player.Class, 1118);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("What are you looking for?");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted) && client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0))
                            data.AddOption("Bandits`~Plot~II.", 1);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted) && client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 1, 1) == false)
                            data.AddOption("Bandits` Plot IV.", 6);
                        data.AddOption("Nothing.", 255)
     .AddAvatar(100).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Player.TransformationID != 0)
                        {
                            data.AddText("Well, we are going to take Phoenix Castle, tomorrow. Pass the message to the other remembers and don`t tell anyone else!");
                            data.AddOption("Sure~I~won`t.", 2)
                                 .AddAvatar(100).FinalizeDialog();
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~put~on~the~coat~you~stole~from~level~33~Bandit.");
                        }

                        break;
                    }
                case 2:
                    {
                        data.AddText("You~are~heading~to~tell~the~Intelligence~Agent~the~news.");
                        data.AddOption("Yes.", 3).AddOption("No.", 255)
                             .AddAvatar(100).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        client.SendSysMesage("You've put on the Bandit`s Coat. Hurry to find the Bandit Boss to find out about the confidential letter!", MsgMessage.ChatMode.System);
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                        client.Player.QuestGUI.SendAutoPatcher(ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);

                        break;
                    }
                case 6:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 1))
                        {
                            client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest2.MissionId, 0, 1);
                            client.Player.QuestGUI.SendAutoPatcher("You~are~heading~to~report~to~the~Phoenix~Castellan.", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);
                            break;
                        }
                        data.AddText("You spy! I`ll kill you!");
                        data.AddOption("Keep~dreaming!", 7)
                             .AddAvatar(100).FinalizeDialog();

                        break;
                    }
                case 7:
                    {
                        if (!client.Map.ContainMobID(11008))
                        {
                            Server.AddMapMonster(stream, client.Map, 11008, client.Player.X, client.Player.Y, 18, 18, 1);
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.WarrantOfArrest)]
        public static void WarrantOfArrest(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.WarrantOfArrest, client.Player.Class, 1144);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("		Most Wanted").AddText("  	Eidolon! Dead or alive!").AddText("                                 -----Militia Captain.");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted) == false)
                            data.AddOption("Slay~the~Eidolon~Envoy.", 1);
                        data.AddOption("Quit.", 255);
                        data.FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("Objective: 15 Fire Spirits\n").AddText("Fire Spirit Zone\n").AddText("Risk: Low\n").AddText("Issuer: Militia Captain\n")
                         .AddOption("Accept~Quest.", 3).AddOption("Quit.", 255)
                        .FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Kill~the~Fire~Spirits~and~then~report~back~to~the~Militia~Captain.", 1011, 613, 710, 0);

                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.MilitiaCaptain)]
        public static void MilitiaCaptain(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.MilitiaLeopard, client.Player.Class, 1143);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.BoldBoy, client.Player.Class, 1131);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.WarrantOfArrest, client.Player.Class, 1144);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.MilitiaCaptain, client.Player.Class, 1300);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I love my family and my village! It is my duty to stay here protect them. It will be great if you can offer me a hand. I will give you some jobs to do. Which one of the following do you want to try?");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("I~am~ordered~to~report~to~you!", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("I~am~ordered~to~report~to~you!", 3);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Solve~the~Fire~Spirit.", 8);
                        data.AddOption("Quest:~The~Ape~City.", 9);
                        data.AddOption("They~miss~you~too!", 255);
                        data.AddAvatar(102).FinalizeDialog();

                        break;
                    }
                case 8:
                    {
                        if (!client.Inventory.HaveSpace(3))
                        {
                            data.AddText("Please make 2 more spaces in your inventory")
                          .AddOption("Let me check.", 255)
                          .AddAvatar(102).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {

                            client.IncreaseExperience(stream, 161000, Role.Flags.ExperienceEffect.angelwing, true);
                            client.Player.Money += 500;
                            client.Inventory.AddItemWitchStack(720172, 0, 5, stream);
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've receive some EXP 1,000 Silver, 5 EXP Ball Scraps and Level 40 Super Weapon(B)! ", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            if (Database.AtributesStatus.IsTrojan(client.Player.Class))
                                client.Inventory.Add(stream, 480079, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else if (Database.AtributesStatus.IsWarrior(client.Player.Class))
                                client.Inventory.Add(stream, 561079, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else if (Database.AtributesStatus.IsArcher(client.Player.Class))
                                client.Inventory.Add(stream, 500189, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else if (Database.AtributesStatus.IsNinja(client.Player.Class))
                                client.Inventory.Add(stream, 601079, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else if (Database.AtributesStatus.IsMonk(client.Player.Class))
                                client.Inventory.Add(stream, 610079, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else if (Database.AtributesStatus.IsPirate(client.Player.Class))
                                client.Inventory.Add(stream, 611079, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else if (Database.AtributesStatus.IsLee(client.Player.Class))
                                client.Inventory.Add(stream, 617079, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                                client.Inventory.Add(stream, 620079, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else if (Database.AtributesStatus.IsWindWalker(client.Player.Class))
                                client.Inventory.Add(stream, 626079, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            break;
                        }
                        data.AddText("Well done! Maple Forest is a little bit safer, now! Take your reward. You did good, today.")
                            .AddOption("Thanks.", 9)
                            .AddAvatar(102).FinalizeDialog();


                        break;
                    }
                case 9:
                    {
                        data.AddText("The AC-Lieutenant in Ape City need our help! I hope you can offer him a hand!")
                          .AddOption("No~problem!", 10)
                          .AddOption("I~don`t~want~to~go~there!", 255)
                          .AddAvatar(102).FinalizeDialog();

                        break;
                    }
                case 10:
                    {
                        data.AddText("Speak~with~the~AC-Lieutenant~in~Ape~City!")
                           .AddOption("Yes.", 11)
                           .AddOption(" No.", 255)
                           .AddAvatar(102).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        client.Teleport(573, 625, 1020);
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        client.Player.QuestGUI.SendAutoPatcher(ActiveQuest4.FinishNpcId.Map, ActiveQuest4.FinishNpcId.X, ActiveQuest4.FinishNpcId.Y, ActiveQuest4.FinishNpcId.ID);
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.IncreaseExperience(stream, 161000, Role.Flags.ExperienceEffect.angelwing, true);
                            client.Player.Money += 500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You receive 161,000 Experience and 500 Gold.. ", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                        }
                        data.AddText("This is not a safe place! You have to be careful, if you`re going to wander around, here!")
                           .AddOption("Yes~sir!", 2)
                           .AddAvatar(102).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(1011, 656, 698, 8278);
                        break;
                    }
                case 3:
                    {
                        data.AddText("What? He is gone? Oh, my god! He is really gone? Not Leopard!  LEOPARD! NOOOOOOOOOOOOOOOOOOOOOOOOOO!!!!!!!!!")
                             .AddOption("Control~yourself!", 4)
                             .AddAvatar(102).FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                           .AddOption("Let me check", 255)
                           .AddAvatar(102).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.Inventory.AddItemWitchStack(1000030, 0, 5, stream);
                            client.IncreaseExperience(stream, 100000, Role.Flags.ExperienceEffect.angelwing, true);
                            client.Player.Money += 500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You received some EXP, 500 Silver and 5 Amritas!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                        }
                        data.AddText("What? He is gone? Oh, my god! He is really gone? Not Leopard!  LEOPARD! NOOOOOOOOOOOOOOOOOOOOOOOOOO!!!!!!!!!")
                             .AddOption("Control~yourself!", 5)
                             .AddAvatar(102).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        data.AddText("Would you like to some Fire Spirits! I guarantee you would be well paid!")
                               .AddOption("Great!", 6)
                               .AddAvatar(102).FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(1011, 791, 478, 8281);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.MilitiaLeopard)]
        public static void MilitiaLeopard(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.MilitiaDragon, client.Player.Class, 1141);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.MilitiaLeopard, client.Player.Class, 1142);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.MilitiaLeopard, client.Player.Class, 1143);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I am a soldier! Whether I`m alive or dead, I will fight for my people! But I have many unfinished wishes. I want to return my identity token, and I want my revenge. Will you help me?");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Are~you~Leopard?", 1);
                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("I~will~revenge~for~you!", 3);
                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("What identity token?", 5);
                        data.AddOption("Great Soldier", 255);
                        data.AddAvatar(236).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.IncreaseExperience(stream, 194000, Role.Flags.ExperienceEffect.angelwing, true);
                            client.Player.Money += 600;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You receive 200,000 Experience and 600 Gold. ", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                        }

                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("Are you looking for me? I am a dead man now!")
                                     .AddOption("Rest~in~peace,~soldier!", 3)
                                     .AddAvatar(236).FinalizeDialog();
                        }

                        break;
                    }
                case 3:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 5))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                                {
                                    client.IncreaseExperience(stream, 194000, Role.Flags.ExperienceEffect.angelwing, true);
                                    client.Player.Money += 1000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    client.Inventory.AddItemWitchStack(720172, 0, 1, stream);
                                    client.SendSysMesage("You've received some EXP, 1,000 Silver and 5 EXP Ball Scraps! ", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                    data.AddText("Die! Die! You bastard!")
                                          .AddOption("I~know~you~are~angry!", 5)
                                          .AddAvatar(236).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Please, collect 5 Fury Cores for me! My soul will be appeased.")
                               .AddOption("Okay!", 255)
                               .AddAvatar(236).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("I went too deep into this dangerous area and was trapped by the Fire Spirits. I`m a dead man, now.. But I didn`t lost the heart of a soldier! I will keep fighting to save my village! I know these beasts carry Fury Cores to boost their powers. Will you")
                            .AddText("~collect 5 cores for me? I need them to put my enraged soul at peace. Only then will I be able to move on to the next world...")
                            .AddOption("I~will~go!~Now!", 4)
                                 .AddAvatar(236).FinalizeDialog();


                        break;
                    }
                case 4:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Collect~5~Fury~Cores.", 1011, 683, 492, 0);

                        break;
                    }
                case 5:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("This is my identity token... I want you to take it to my Captain. He will know what happen to me when he sees it!")
             .AddOption("I~will.", 255)
             .AddAvatar(236).FinalizeDialog();
                            break;
                        }
                        data.AddText("This is a token used to recognize my identity. I want you to take it to my Captain. He will know what happened to me, when he sees it. I can`t forget Tiger and Dragon. They will always be my friends, even in the afterlife! I will miss them all...")
                                    .AddOption("I~will~do~that~for~you!", 6)
                                    .AddAvatar(236).FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                                .AddOption("Let me check", 255)
                                .AddAvatar(236).FinalizeDialog();
                            break;
                        }
                        client.Inventory.Add(stream, 711333);
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest3.MissionId, 1);
                        client.Player.QuestGUI.SendAutoPatcher("Send~the~Token~to~Militia~Captain.", ActiveQuest3.FinishNpcId.Map, ActiveQuest3.FinishNpcId.X, ActiveQuest3.FinishNpcId.Y, ActiveQuest3.FinishNpcId.ID);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.MilitiaDragon)]
        public static void MilitiaDragon(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.MilitiaDragon, client.Player.Class, 1133);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.MilitiaDragon, client.Player.Class, 1141);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("It is tough living out here in the wild! I just got a little water left, and unsavoury food to chow. And the worst thing is that we lost one of my team mate.");
                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("I~can~get~some~water~for~you!.", 1);
                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("I~can~get~some~water~for~you!.", 3);
                        data.AddOption("Homesick!~Right?", 255);
                        data.AddAvatar(48).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1))
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                            {
                                client.Inventory.Remove(711331, 1, stream);
                                client.IncreaseExperience(stream, 194000, Role.Flags.ExperienceEffect.angelwing, true);
                                client.Player.Money += 600;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.SendSysMesage("You receive 194,000 Experience and 600 Gold. ", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                data.AddText("Ahhhhhh! Fresh water! You saved my life, man. Nothing like cool water, if you`re standing in this hell! Thank you!")
                                     .AddOption("You`re~welcome!", 3)
                                     .AddAvatar(48).FinalizeDialog();
                                break;
                            }
                        }
                        data.AddText("It`s hard staying near the territory of the Fire Spirits. It`s so hot and I need some water, but I can`t leave my post.")
                                 .AddText("~Hey! Will you go and get some water for me?")
                                 .AddOption("My~pleasure.", 2)
                                 .AddAvatar(48).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                                .AddOption("Let me to check", 255)
                                .AddAvatar(48).FinalizeDialog();
                            break;
                        }
                        client.Inventory.Add(stream, 711331);
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Use~the~Empty~Bucket~to~fetch~a~pail~of~water~by~the~river~near~the~bridge.", 1011, 643, 622, 0);

                        break;
                    }
                case 3:
                    {
                        data.AddText("I wonder where another of our team, Militia Leopard, might have gone? He has been missing for quite a while. I want to make sure he`s okay. Do you think you could take a look around, and see if you can find him?")
                                .AddOption("Show~me~the~direction!", 4)
                                .AddAvatar(48).FinalizeDialog();

                        break;
                    }

                case 4:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher(ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.MilitiaTiger)]
        public static void MilitiaTiger(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.BoldBoy, client.Player.Class, 1131);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.MilitiaTiger, client.Player.Class, 1135);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I miss my wife and my son back home. My son`s birthday is also approaching. But I will not come home until those blasted Fire Spirits have all been wiped out!");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("I~got~a~letter~for~you!", 2);
                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("You~get~anything~for~your~son?.", 3);
                        data.AddOption("You~are~a~good~father.", 255);
                        data.AddAvatar(57).FinalizeDialog();


                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {

                            client.IncreaseExperience(stream, 161000, Role.Flags.ExperienceEffect.angelwing, true);
                            client.Player.Money += 500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You receive 161,000 Experience and 500 Gold.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                        }
                        data.AddText("Oh, I miss them, too! But I have job to do here! We have to save the village! I can`t just walk away from my duty.")
                                  .AddOption("Great~man!", 3)
                                  .AddAvatar(57).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 10))
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                            {
                                client.Inventory.Remove(711332, 10, stream);
                                client.Player.Money += 1000;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.Player.RateExp = 2;
                                client.Player.DExpTime = 60 * 60;
                                client.Player.CreateExtraExpPacket(stream);
                                client.Inventory.AddItemWitchStack(1000030, 0, 5, stream);
                                client.SendSysMesage("You received 60 minutes EXP, 1,000 Silver and 5 Amritas!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            }
                            data.AddText("Wooh! What beautiful teeth you have! I can feel the heat coming off them, from here! My son will love it!")
                         .AddOption("You`re~a~good~father.", 5)
                         .AddAvatar(57).FinalizeDialog();
                            break;
                        }
                        data.AddText("My son holds great enthusiasm, joy, and pleasure for this world! I want to make him a necklace for his birthday, but I need 10 Fire Spirit Teeth to finish making it. Will you help me to collect some?")
                             .AddOption("Okay!", 4)
                             .AddAvatar(57).FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Collect~10~Spirit~Teeth~from~the~Fire~Spirits.", 1011, 704, 673, 0);

                        break;
                    }
                case 5:
                    {
                        data.AddText("Wait a second, please. My brother, Militia Dragon, needs some water badly. Go help him, if you can!")
                        .AddOption("OK,~I~will.", 6)
                        .AddAvatar(57).FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(1011, 661, 704, 8279);
                        break;
                    }
            }

        }
        [NpcAttribute(NpcID.BoldBoy)]
        public static void BoldBoy(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.BoldBoy, client.Player.Class, 1121);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.BoldBoy, client.Player.Class, 1131);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I used to spend a night in the hill, alone! Don`t you think I`m brave? If you are also a brave man, I will have some job for you to do. Which of the following quests do you want to try?");
                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Kill~10~Ratlings.", 2);
                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Letter~From~Home.", 10);
                        data.AddOption("It`s~great!", 255);
                        data.AddAvatar(36).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        data.AddText("I bet you wouldn`t  even dare to kill a Ratling! If you think you can, kill 10 for me, and I`ll pay you!")
                                     .AddOption("It`s~a~cinch.", 3)
                                     .AddAvatar(36).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 10))
                        {

                            data.AddText("You are great! Thank you! You have definitely earned the right to join my gang! How cool is that?")
                                  .AddOption("Really?~That`s~great!", 5)
                                  .AddAvatar(36).FinalizeDialog();
                            break;
                        }
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        data.AddText("You~will~find~some~Ratlings~running~outside~the~village.~go~slay~them.")
                                    .AddOption("Yes.", 4).AddOption("No.", 255)
                                    .AddAvatar(36).FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        client.Player.QuestGUI.SendAutoPatcher("You`ve~killed~10~Ratlings!", 1011, 638, 456, 0);

                        break;
                    }
                case 5:
                    {

                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 800;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Player.RateExp = 2;
                            client.Player.DExpTime = 60 * 60;
                            client.Player.CreateExtraExpPacket(stream);
                            client.Inventory.Add(stream, 721710);
                            client.SendSysMesage("You received 60 minutes EXP, 800 Silver and Castle Gate", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            data.AddText(" I hear that the Fur Dealer is badly in need of some Ratling Fur. I`m sure you`ll be well paid, if you help him!")
                                    .AddOption("Thanks~for~the~info..", 6)
                                    .AddAvatar(36).FinalizeDialog();
                        }

                        break;
                    }
                case 6:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(1011, 796, 475, 8275);
                        break;
                    }
                case 10:
                    {
                        data.AddText("Are you going to find the Militia in the forest? Can you take me with you? I miss my father, he has been away from home for over 2 months! No? Can you take this letter to my father? My mother and I miss him, very much! Oh, my father is the Militia Tiger.")
                                .AddOption("Sure,~I~can.", 11)
                                .AddAvatar(36).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Send~the~letter~to~the~Militia~Tiger.", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.FurDealer)]
        public static void FurDealer(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.FurDealer, client.Player.Class, 1123);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Ratlings have beautiful fur! I can make good money from selling their fur to the other cities.");
                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Are~you~buying~the~Fur?", 2);
                        data.AddOption("What~a~good~business!", 255);
                        data.AddAvatar(53).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 10))
                        {
                            //client.Inventory.Remove(711325, 10, stream);
                            data.AddText("Oh! Beautiful, shining fur! I can make good money by selling these. Here is your reward! Thank you, again!")
                                   .AddOption("See~you!", 4)
                                   .AddAvatar(53).FinalizeDialog();
                            break;
                        }
                        data.AddText("Let me introduce myself, I am the Fur Dealer! A far travelling businessman, with quite the eye for animals! Would you like to make some money? I need to get 10 Ratling Furs, quick! If you can help, you will be paid handsomely!")
                                     .AddOption("Okay!", 3)
                                     .AddAvatar(53).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Collect~10~Ratling~Furs.", 1011, 638, 456, 0);

                        break;
                    }
                case 4:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Inventory.Remove(711325, 10, stream);
                            client.Player.Money += 500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.IncreaseExperience(stream, 500, Role.Flags.ExperienceEffect.angelwing, true);
                            if (Database.AtributesStatus.IsTrojan(client.Player.Class))
                                client.Inventory.Add(stream, 130038, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else if (Database.AtributesStatus.IsWarrior(client.Player.Class))
                                client.Inventory.Add(stream, 131038, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else if (Database.AtributesStatus.IsArcher(client.Player.Class))
                                client.Inventory.Add(stream, 133028, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else if (Database.AtributesStatus.IsNinja(client.Player.Class))
                                client.Inventory.Add(stream, 135038, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else if (Database.AtributesStatus.IsMonk(client.Player.Class))
                                client.Inventory.Add(stream, 136038, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else if (Database.AtributesStatus.IsPirate(client.Player.Class))
                                client.Inventory.Add(stream, 139038, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else if (Database.AtributesStatus.IsLee(client.Player.Class))
                                client.Inventory.Add(stream, 138038, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                                client.Inventory.Add(stream, 134038, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else if (Database.AtributesStatus.IsWindWalker(client.Player.Class))
                                client.Inventory.Add(stream, 170038, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);

                            client.SendSysMesage("You received some EXP, 500 Silver and a level 40 Elite Armor(B).", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            data.AddText("Hey, wait! The Village Head is looking for you, about the Ratling, I guess.")
                                 .AddOption("Got~it.", 5)
                                 .AddAvatar(53).FinalizeDialog();
                        }

                        break;
                    }
                case 5:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(1011, 772, 475, 8273);
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.VillageHead)]
        public static void VillageHead(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.PhoenixCastellan, client.Player.Class, 1120);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.VillageHead, client.Player.Class, 1125);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.VillageHead, client.Player.Class, 1126);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.VillageHead, client.Player.Class, 1127);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.VillageHead, client.Player.Class, 1128);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.VillageHead, client.Player.Class, 1129);
            var ActiveQuest7 = Database.QuestInfo.GetFinishQuest((uint)NpcID.VillageHead, client.Player.Class, 1130);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Phoenix Castle is always our friend! My village is in trouble now, I hope our friends in the Castle can help us out! If you are here to help me, I have a bunch of jobs for you to do! Which of the following quests do you want to try?");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("I~was~sent~to~help~you!", 2);
                        else if (!client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Ratling~King~1.", 5);
                        else if (!client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Ratling~King~2.", 7);
                        else if (!client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Ratling~King~3.", 10);
                        else if (!client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Ratling~King~4.", 12);
                        else if (!client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Ratling~King~5.", 14);
                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Help~Militia~Captain.", 20);
                        data.AddOption("See~you.", 255);
                        data.AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                        client.Player.Money += 300;
                        client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        client.SendSysMesage("You've received 300 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                        data.AddText("Phoenix Castle will send help to us? Thank you! Thank you, so much!")
                                       .AddOption("You`re~welcome.", 3)
                                       .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        data.AddText("You`re welcome! By the way, there`s a Bold Boy who`s going to  kill a Ratling!")
                                        .AddOption("~That is dangerous! Go help him, please.", 4)
                                        .AddOption("Okay!", 255)
                                        .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(1011, 761, 473, 8274);
                        break;
                    }
                case 5:
                    {
                        if (!client.Inventory.HaveSpace(2))
                        {
                            data.AddText("Please make 2 more spaces in your inventory.")
                                         .AddOption("Let me check.", 255)
                                         .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 1))
                        {
                            client.Inventory.Remove(711327, 1, stream);
                            client.Inventory.AddItemWitchStack(720172, 0, 3, stream);
                            client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId);
                            if (Database.AtributesStatus.IsTrojan(client.Player.Class))
                                client.Inventory.Add(stream, 118038, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else if (Database.AtributesStatus.IsWarrior(client.Player.Class))
                                client.Inventory.Add(stream, 111038, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else if (Database.AtributesStatus.IsArcher(client.Player.Class))
                                client.Inventory.Add(stream, 113038, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else if (Database.AtributesStatus.IsNinja(client.Player.Class))
                                client.Inventory.Add(stream, 112018, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else if (Database.AtributesStatus.IsMonk(client.Player.Class))
                                client.Inventory.Add(stream, 143038, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else if (Database.AtributesStatus.IsPirate(client.Player.Class))
                                client.Inventory.Add(stream, 144038, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else if (Database.AtributesStatus.IsLee(client.Player.Class))
                                client.Inventory.Add(stream, 148038, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                                client.Inventory.Add(stream, 114038, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else if (Database.AtributesStatus.IsWindWalker(client.Player.Class))
                                client.Inventory.Add(stream, 101038, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);

                            client.IncreaseExperience(stream, 138000, Role.Flags.ExperienceEffect.angelwing, true);
                            client.Player.Money += 800;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've received some EXP, 800 Silver, 3 EXP Ball Scraps and a Level 37 Elite Helment! ", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);


                            data.AddText("I am quite impressed by your performance! You really are great! We could use more people like you in the village.")
                           .AddOption(" Thank~you!", 7)
                                   .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        data.AddText("The Ratlings are out of control! We found that there is an elusive Ratling King, hiding out there. If you killed it, I think the Ratlings would get confused and stop attacking our village. Snake Meat is the favorite food of the Ratling King,")
                            .AddText("~so if you get some from the Winged Snakes, we should be able to use it to charm the Ratling King out of hiding!")
                            .AddOption("Winged~Snake~Meat?~Okay!", 6)
                                    .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Head~to~Hunter~Wang~for~more.", 1011, 790, 447, 8268);


                        break;
                    }
                case 7:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest3.MissionId, 1))
                        {
                            client.Inventory.Remove(711328, 1, stream);
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                            {
                                client.IncreaseExperience(stream, 151000, Role.Flags.ExperienceEffect.angelwing, true);
                                client.Player.Money += 500;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.SendSysMesage("You received 151000 Experience and 500 Gold. ", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                data.AddText("Good Wine! I could smell it as soon as you were walking up the path! Well, let`s get busy making that lure.")
                  .AddOption("Great!~Let`s~get~busy!", 10)
                          .AddAvatar(67).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("You must be thinking what I am doing. I will make a lure using the Snake Meat! To make the lure more attractive! I need some Wine! Could you buy a bottle of wine from Deeler Shen?")
                              .AddOption("Okay!", 8)
                                      .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 8:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Buy~a~bottle~of~Wine~from~the~Dealer~Shen(779,447).", 1011, 182, 280, 4596);

                        break;
                    }
                case 10:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more spaces in your inventory.")
                                     .AddOption("Let me check.", 255)
                                     .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest4.MissionId, 10))
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                            {
                                client.Inventory.AddItemWitchStack(720173, 0, 3, stream);
                                client.IncreaseExperience(stream, 151000, Role.Flags.ExperienceEffect.angelwing, true);
                                client.Player.Money += 800;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.SendSysMesage("You've received some EXP, 800 Silver and 3 Small Joy Stones. ", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                data.AddText("I was quite impressed by you performance! Good job!")
                          .AddOption("Thank~you.", 12)
                       .AddAvatar(67).FinalizeDialog();

                            }


                            break;
                        }
                        data.AddText("I will soak the Snake Meet in the Wine, so it will take a bit. Why not use this time to slay some Ratlings! This will help make you smell like the Ratlings. That way, it will make it harder for the Ratling King to discover you!")
                            .AddOption("Okay!", 11)
                         .AddAvatar(67).FinalizeDialog();


                        break;
                    }
                case 11:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Slay~10~Ratlings~outside~the~village.", 1011, 638, 456, 0);

                        break;
                    }
                case 12:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest5.MissionId, 1))
                        {
                            data.AddText("Good! You have it! Now take a little break. I want study the Frost Token first, and will instruct you on what to do, later.")
                            .AddOption("Okay!", 14)
                         .AddAvatar(67).FinalizeDialog();

                            client.Player.QuestGUI.FinishQuest(ActiveQuest5.MissionId);
                            client.Inventory.Remove(711329, 1, stream);
                            client.IncreaseExperience(stream, 242000, Role.Flags.ExperienceEffect.angelwing, true);
                            client.Player.Money += 800;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You received 242,000 Experience and 800 Gold. ", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            break;
                        }
                        data.AddText("You still need a Frost Token. It will provide you with extra defense, especially your resistance against heat! I`ll show you where to find it!")
                        .AddOption("Okay!", 13)
                     .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 13:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        client.Player.QuestGUI.SendAutoPatcher(1011, 532, 434, 0);
                        break;
                    }
                case 14:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest6.MissionId, 1))
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more spaces in your inventory.")
                                         .AddOption("Let me check.", 255)
                                         .AddAvatar(67).FinalizeDialog();
                                break;
                            }
                            client.Player.QuestGUI.FinishQuest(ActiveQuest6.MissionId);
                            client.IncreaseExperience(stream, 242000, Role.Flags.ExperienceEffect.angelwing, true);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You received some EXP, 2,000 Silver and a Bound EXP Ball! ", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            client.Inventory.Add(stream, Database.ItemType.ExpBall, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            data.AddText("I can`t believe my ears! You killed it and saved our village! See? The Ratlings are scattering! Your name will be remembered for as long as our village remains! Thank you, hero!")
                      .AddOption("Thank~you!", 20)
                .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        data.AddText("Listen! The Ratling King excels at fire attacks. This Frost Token can provide with you extra resistance against heat and fire, for 5 minutes. Use the Wine Soaked Snake to charm it out, then slay it while you`re protected. If you don`t make it in time,")
                    .AddText("~you will be sent back to village, before anything bad happens to you! Are you ready to challenge the Ratling King?")
                            .AddOption("I~am~ready!", 15)
                      .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 15:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more spaces in your inventory.")
                                     .AddOption("Let me check.", 255)
                                     .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        client.Inventory.Add(stream, 720840);
                        client.Player.QuestGUI.Accept(ActiveQuest6, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Use~the~Wine~Soaked~Snake~to~charm~the~Ratling~King~out,~and~then~slay~it!", 1011, 648, 441, 0);

                        break;
                    }
                case 20:
                    {
                        data.AddText("Fire Spirits are another big threat to the village! In fact, they`re a threat to Phoenix Castle, too! There are some militiaman fighting desperately against these evil spirits. Maybe you can offer them some help? You can speak with the Militia Captain")
                          .AddText("~for more information!")
                            .AddOption("Find~the~Militia~Captain.", 21)
                            .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 21:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest7, 0);
                        client.Player.QuestGUI.SendAutoPatcher(1011, 761, 473, 8274);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.WineKiddo)]
        public static void WineKiddo(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.VillageHead, client.Player.Class, 1126);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.VillagerChou, client.Player.Class, 401);

            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("My wine enjoys a great reputation among people! Would you like a bottle of wine? It`s free!");
                            data.AddOption("Do~you~have~wine~to~sell?", 5);
                            data.AddOption("Wonderful!");
                            data.AddAvatar(138);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("My wine enjoys a great reputation among people! Would you like a bottle of wine? It`s free!");
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                data.AddOption("Quest:~Ratling~King~2.", 2);
                            data.AddOption("Wonderful!", 255);
                            data.AddAvatar(138).FinalizeDialog();
                        }
                        break;
                    }
                case 5:
                    {
                        data.AddText("Hey, my friend! Welcome to my store. You see, the Bamboo Wine is our best seller. Expensive? Nah, hah. It`s only 10,000");
                        data.AddText("~Silver for each bottle! Don`t get drunk, ok? Oh, I`ll have a branch in Twin City, soon. You can come and have a drink.");
                        data.AddOption("Buy~one.", 6);
                        data.AddOption("Let~me~check~my~wage,~first.");
                        data.AddAvatar(138);
                        data.FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.Contain(721800, 1))
                        {
                            data.AddText("I tell you, this Bamboo Wine is pretty strong. I`ve never seen one guy buy 2 bottles before. Take care.");
                            data.AddOption("Don`t~worry~about~me.");
                            data.AddAvatar(138);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Player.Money < 10000)
                                {
                                    data.AddText("You need 10,000 silvers for this. It`s already the floor price.");
                                    data.AddOption("Alright.");
                                    data.AddAvatar(138);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.Money -= 10000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                                    client.Inventory.Add(stream, 721800, 1);
                                    client.SendSysMesage("You brought a bottle of Bamboo Wine with 10,000 silvers.");
                                }
                            }
                            else
                            {
                                data.AddText("Oh, look at your inventory. I have to remind you that the bottle is very fragile. Make some room for it, first.");
                                data.AddOption("One~second.");
                                data.AddAvatar(138);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 2:
                    {
                        data.AddText(" Come! Best wine! Cheap! 1000 gold a bottle! Come! Come! Best wine for sale!")
                                .AddOption(" I~need~a~bottle~of~wine!", 3)
                                .AddAvatar(138).FinalizeDialog();


                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.Contain(711328, 1))
                        {
                            data.AddText("You have bought a bottle of wine from me! Don`t drink too much!")
                                .AddOption("Thanks.", 255)
                                .AddAvatar(138).FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                              .AddOption("let me check.", 255)
                              .AddAvatar(138).FinalizeDialog();
                            break;
                        }
                        if (client.Player.Money >= 1000)
                        {
                            client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                            client.Player.Money -= 1000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Inventory.Add(stream, 711328);

                            data.AddText("You have bought a bottle of wine from me! Don`t drink too much!")
                              .AddOption("Thanks.", 4)
                              .AddAvatar(138).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You do not have 1000 silvers with you.")
                              .AddOption("I~see.", 255)
                              .AddAvatar(138).FinalizeDialog();

                        }

                        break;
                    }
                case 4:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.HunterWong)]
        public static void HunterWong(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.HunterWong, client.Player.Class, 1107);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("He who fights with monsters should take care, lest he wind up becoming a monster, himself! Would you like to help me with any quest?");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Snake~Scale.", 2);
                        data.AddOption("Sooner~or~later.", 255);
                        data.AddAvatar(45).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 10))
                        {
                            data.AddText("I can`t thank you enough. That`s perfect!")
                                .AddOption("Fantastic!.", 5)
                                .AddAvatar(45).FinalizeDialog();

                            break;
                        }
                        data.AddText("To do a good job, you must first sharpen your tools. I heard that poisonous Snake Scales can make animals dizzy.")
                                    .AddOption("~Can you get 10 Snake Scales, for me?", 3)
                                    .AddOption("I`d~love~to.", 255)
                                    .AddAvatar(45).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        data.AddText("You~are~going~to~kill~Winged~Snakes~and~collect~10~Snake~Scales.")
                                  .AddOption("Yes.", 4)
                                  .AddOption("No.", 255)
                                  .AddAvatar(45).FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        client.Player.QuestGUI.SendAutoPatcher(1011, 342, 132, 0);
                        break;
                    }
                case 5:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Inventory.Remove(711321, 10, stream);
                            client.IncreaseExperience(stream, 138000, Role.Flags.ExperienceEffect.angelwing, true);
                            client.Player.Money += 600;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You received 138,000 EXP and 600 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            data.AddText("Have you got some Snake Meat, by the way? Go speak with the Village Head. He`s worried about it.")
                                  .AddOption("I`m~on~my~way.", 6)
                                  .AddAvatar(45).FinalizeDialog();
                        }

                        break;
                    }
                case 6:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(1011, 772, 475, 8273);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.PhoenixCastellan)]
        public static void PhoenixCastellan(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.IntelligenceAgent, client.Player.Class, 1117);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PhoenixCastellan, client.Player.Class, 1118);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PhoenixCastellan, client.Player.Class, 1120);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("I will always protect the people, here.")
                                     .AddOption("Bandits`~Plot~III.", 2)
                                     .AddOption("Greetings,~Castellan!", 255)
                                     .AddAvatar(71).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("I will always protect the people, here.");
                            if (!client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                data.AddOption("Bandits`~Plot~IV.", 4);
                            else if (!client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                data.AddOption("Help~the~Village~Head..", 10);
                            data.AddOption("Greetings,~Castellan!", 255)
                               .AddAvatar(71).FinalizeDialog();
                        }


                        break;
                    }
                case 2:
                    {
                        data.AddText("This letter is very important. Thank you very much!")
                                   .AddOption("It`s~OK.", 3)
                                   .AddAvatar(71).FinalizeDialog();


                        break;
                    }
                case 3:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Inventory.Remove(711323, 1, stream);
                            client.IncreaseExperience(stream, 10000, Role.Flags.ExperienceEffect.angelwing, true);
                            client.Player.Money += 500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've received some exp and 500 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                        }
                        data.AddText("Kill the leader and the gang will collapse. Please, help us kill the Bandit Boss!")
                                   .AddOption("Yes,~sir!", 5)
                                   .AddAvatar(71).FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 1, 1))
                        {
                            client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId);
                            client.IncreaseExperience(stream, 10000, Role.Flags.ExperienceEffect.angelwing, true);
                            client.Player.Money += 1200;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've received some exp and 1200 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            data.AddText("You must be tired. Take a break, buddy.")
                              .AddOption("Okay.", 10)
                              .AddAvatar(71).FinalizeDialog();
                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("What`s the matter? You need to go kill the Bandit Boss!")
                                .AddOption("I~will.", 255)
                                .AddAvatar(71).FinalizeDialog();
                            break;
                        }
                        data.AddText("Kill the leader and the gang will collapse. Please, help us kill the Bandit Boss!")
                                 .AddOption("Yes,~sir!", 5)
                                 .AddAvatar(71).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        client.SendSysMesage("You are heading to kill the Bandit Boss.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher(1011, 227, 404, 8272);

                        break;
                    }
                case 10:
                    {
                        data.AddText("Yes, there is a small village outside the Phoenix Castle in the forest. We maintain a good relationship with the village. Recently, the village was badly damaged by rats. As a neighbor, we need to do something! Will you go see the Village Head first,")
                            .AddText("~and let him know about our plan to help out?")
                            .AddOption("Of~course!", 11)
                               .AddAvatar(71).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Tell~the~Village~Head~that~the~Castellan~will~help~him~deal~with~the~ratalings.", ActiveQuest3.FinishNpcId.Map, ActiveQuest3.FinishNpcId.X, ActiveQuest3.FinishNpcId.Y, ActiveQuest3.FinishNpcId.ID);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.IntelligenceAgent)]
        public static void IntelligenceAgent(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.IntelligenceAgent, client.Player.Class, 1115);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.IntelligenceAgent, client.Player.Class, 1116);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.IntelligenceAgent, client.Player.Class, 1117);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hey, there! I`m Intelligence Agent. Want to take the quests and destroy Bandits` plots?");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Bandits`~Plot~I.", 1);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Bandits`~Plot~II.", 4);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Bandits`~Plot~III.", 10);
                        data.AddOption("Nice~to~meet~you..", 255)
                        .AddAvatar(236).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1))
                            {
                                client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                                client.IncreaseExperience(stream, 15000, Role.Flags.ExperienceEffect.angelwing, true);
                                client.Player.Money += 800;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.SendSysMesage("You've received some exp and 800 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                data.AddText("You did a good job!")
                                        .AddOption("No~problem.", 4)
                                        .AddAvatar(236).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You need to get a Bandit`s Coat from the Level 33 Bandits. We will be able to blend in with the rest of the gang, once we have it.")
                                    .AddOption("Roger.", 255)
                                    .AddAvatar(236).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("We have been here trying to collect information for weeks, but haven`t found anything, yet. We are planning to steal a Bandit`s Coat from")
                        .AddText("~the Level 33 Bandits, so we will be able to blend into the gang. Would you like to get one for me?")
                        .AddOption("Sure.", 2)
                               .AddAvatar(236).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        data.AddText("You~are~going~to~get~a~Bandit`s~Coat~from~the~Level~33~Bandits.")
                       .AddOption("Yes.", 3).AddOption("No.", 255)
                              .AddAvatar(236).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (!client.Inventory.Contain(711323, 1))
                                client.Inventory.Add(stream, 711323);
                            data.AddText("OK. You`ve got the letter. Deliver it to the Phoenix Castellan.")
                          .AddOption("Take~care.", 255)
                          .AddAvatar(236).FinalizeDialog();
                            break;
                        }
                        data.AddText("I didn`t expect you would be back so soon! Did you read the letter yet?")
                            .AddOption("Yes.~I~remember~everything.", 11)
                            .AddAvatar(236).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        data.AddText("That`s great. I`m writing a letter. Please deliver it to the Phoenix Castellan.")
                          .AddOption("I`ll~wait.", 12)
                          .AddAvatar(236).FinalizeDialog();

                        break;
                    }
                case 12:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                                .AddOption("Let me check", 255)
                                .AddAvatar(236).FinalizeDialog();
                            break;
                        }
                        client.Inventory.Add(stream, 711323);
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.Player.QuestGUI.SendAutoPatcher("You~are~going~to~deliver~the~letter~to~the~Phoenix~Castellan.", ActiveQuest3.FinishNpcId.Map, ActiveQuest3.FinishNpcId.X, ActiveQuest3.FinishNpcId.Y, ActiveQuest3.FinishNpcId.ID);

                        break;
                    }
                case 4:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory.")
                                    .AddOption("Let me check", 255)
                                    .AddAvatar(236).FinalizeDialog();
                                break;
                            }
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 1))
                            {
                                client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId);
                                client.IncreaseExperience(stream, 16000, Role.Flags.ExperienceEffect.angelwing, true);
                                client.Player.Money += 1000;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.Inventory.AddItemWitchStack(1000030, 0, 5, stream);
                                client.SendSysMesage("You've received some Exp, 1,000 Silver and 5 Amritas!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                data.AddText("I didn`t expect you would be back so soon! Did you read the letter yet?")
    .AddOption("Yes.~I~remember~everything.", 11)
    .AddAvatar(236).FinalizeDialog();
                                break;
                            }
                            if (client.Inventory.Contain(720837, 1) == false)
                                client.Inventory.Add(stream, 720837);
                            data.AddText("If you happen to be recognized, just try to get another coat from the Level 33 Bandits and try again, later.")
     .AddOption("Copy~that.", 255)
     .AddAvatar(236).FinalizeDialog();
                            break;
                        }
                        data.AddText("Put on the Bandit`s Coat, and you won`t be recognized. If you happen to be recognized, just try to get another coat from the Level 33 Bandits.")
                     .AddOption("What~shall~I~do~then?", 5)
                            .AddAvatar(236).FinalizeDialog();


                        break;
                    }
                case 5:
                    {
                        data.AddText("You need to talk to the Bandit Boss and read the confidential letter. If he asks you what the code is, you should answer `Success is around the corner`.")
                            .AddOption("Got~it.", 6)
                            .AddAvatar(236).FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        data.AddText("You~put~on~the~coat~and~are~going~to~see~the~Bandit~Boss.")
                            .AddOption("Yes", 7).AddOption("No.", 255)
                            .AddAvatar(236).FinalizeDialog();

                        break;
                    }
                case 7:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (!client.Inventory.Contain(720837, 1))
                                client.Inventory.Add(stream, 720837);
                            client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                            client.Player.QuestGUI.SendAutoPatcher(1011, 227, 404, (uint)NpcID.BanditBoss);
                        }
                        else
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                          .AddOption("let me check.", 255)
                           .AddAvatar(236).FinalizeDialog();
                            break;
                        }

                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.ForestGuvernor)]
        public static void ForestGuvernor(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.ForestGuvernor, client.Player.Class, 1114);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hey! Who are you and why are you here? Did you take any quest, or what?");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Regain~Supplies.", 3);
                        data.AddOption("Good.", 255)
                        .AddAvatar(95).FinalizeDialog();

                        break;
                    }
                case 3:
                    {

                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 10))
                        {
                            if (!client.Inventory.HaveSpace(2))
                            {
                                data.AddText("Please make 2 more spaces in your inventory.")
                         .AddOption("Let me check.", 255)
                                .AddAvatar(95).FinalizeDialog();
                                break;
                            }
                            client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                            client.Inventory.Remove(711322, 10, stream);
                            client.Inventory.Add(stream, 723017, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 150057, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.IncreaseExperience(stream, 5000, Role.Flags.ExperienceEffect.angelwing, true);
                            client.Player.Money += 500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've received some exp, 500 Silver, a Bound EXP Potion and a Gold Ring(Bound)!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            data.AddText("Thank god! Please accept these gifts, as thanks. You`ve really managed to get me out of this mess! You`re a lifesaver!")
                           .AddOption("Thanks.", 6)
                                  .AddAvatar(95).FinalizeDialog();
                            break;
                        }

                        data.AddText("We got big trouble! Well, we were carrying military supplies to Phoenix Castle, when all of a sudden the Bandits rushed out")
                            .AddOption("Relax.~I`ll~be~back,~soon.", 4)
                                   .AddAvatar(95).FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        data.AddText("You~are~going~to~get~10~bags~of~grain~from~the~Bandits.")
                          .AddOption("Yes.", 5).AddOption("No.", 255)
                                 .AddAvatar(95).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        client.Player.QuestGUI.SendAutoPatcher(1011, 235, 373, 0);
                        break;
                    }
                case 6:
                    {
                        data.AddText("Listen! Since you`re smart, I`m wondering if you can go help the Intelligence Agent. He`ll give you a challenging mission!")
                            .AddOption("I`m~on~my~way.", 7)
                            .AddAvatar(95).FinalizeDialog();

                        break;
                    }
                case 7:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(1011, 215, 360, 8270);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.YuJing)]
        public static void YuJing(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.YuLin, client.Player.Class, 1102);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.YuJing, client.Player.Class, 1103);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.YuJing, client.Player.Class, 1104);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("I can`t think of any need in childhood as strong as the need for a father`s protection. Well, are you coming for any quest?")
                               .AddOption("Father~and~Daughter~I.", 2)
                               .AddOption("Leave.", 255)
                               .AddAvatar(116).FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            {
                                data.AddText("I can`t think of any need in childhood as strong as the need for a father`s protection. Well, are you coming for any quest?")
                                  .AddOption("Father~and~Daughter~II.", 1)
                                  .AddOption("Leave.", 255)
                                  .AddAvatar(116).FinalizeDialog();
                            }
                            else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            {
                                data.AddText("I can`t think of any need in childhood as strong as the need for a father`s protection. Well, are you coming for any quest?")
                                  .AddOption("Father~and~Daughter~III.", 5)
                                  .AddOption("Leave.", 255)
                                  .AddAvatar(116).FinalizeDialog();
                            }
                            else
                            {

                                data.AddText("I can`t think of any need in childhood as strong as the need for a father`s protection. Well, are you coming for any quest?")
                                 .AddOption("Leave.", 255)
                                 .AddAvatar(116).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("I can`t think of any need in childhood as strong as the need for a father`s protection. Well, are you coming for any quest?")
                             .AddOption("Leave.", 255)
                             .AddAvatar(116).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 10))
                        {
                            data.AddText("Great! You are my hero!")
                       .AddOption("No~problem.", 5)
                       .AddAvatar(116).FinalizeDialog();
                            break;
                        }
                        data.AddText("Haha, hi! Are you looking for me? Do you want to play a game? Let`s play hide-and-seek!")
                        .AddOption("No,~you~should~go~home.", 3)
                        .AddAvatar(116).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.IncreaseExperience(stream, 5000, Role.Flags.ExperienceEffect.angelwing, true);
                            client.Player.Money += 300;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've received some EXP and 300 Silver", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                        }
                        data.AddText("But I`m still waiting for my dad. You see those Bandits, over there? What if my dad walks home at night and they see him?")
                            .AddText("~Hey, do you think you can scare them off or something? If you can get rid of 10, I bet they`ll all run off.")
                      .AddOption("Alright.", 1)
                      .AddAvatar(116).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("You~are~going~to~kill~10~Bandits.")
                                .AddOption("Yes.", 4)
                                .AddOption("No.", 255)

                                .AddAvatar(116).FinalizeDialog();
                            client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        }

                        break;
                    }
                case 4:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(1011, 48, 381, 0);
                        break;
                    }
                case 5:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId);
                            client.IncreaseExperience(stream, 5000, Role.Flags.ExperienceEffect.angelwing, true);
                            client.Player.Money += 500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've received some EXP and 500 Silver", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                        }
                        data.AddText("Give me a break! I need to focus, if I`m going to spot Dad, in time. Can you go first, and tell my mum that I will be home soon?")
                                .AddOption("OK.~Take~care.", 6).AddAvatar(116).FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        data.AddText("You~are~on~the~way~to~see~Yu~Lin.")
                                .AddOption("Yes.", 7).AddOption("No.", 255).AddAvatar(116).FinalizeDialog();

                        break;
                    }
                case 7:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(ActiveQuest3.FinishNpcId.Map, ActiveQuest3.FinishNpcId.X, ActiveQuest3.FinishNpcId.Y, ActiveQuest3.FinishNpcId.ID);
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.YuLin)]
        public static void YuLin(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.MasterHao, client.Player.Class, 1023);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.YuLin, client.Player.Class, 1102);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.YuJing, client.Player.Class, 1104);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Ever has it been, that love knows not its own depth until the hour of separation... Oh, sorry, I miss my husband... Well, which quest are you coming for?")
                            .AddOption("Father~and~Daughter~III.", 6)
                            .AddAvatar(116).FinalizeDialog();

                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Ever has it been, that love knows not its own depth until the hour of separation... Oh, sorry, I miss my husband... Well, which quest are you coming for?");
                            data.AddOption("Here`s~a~Letter~Home~for~you.", 1);
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                                data.AddOption("Father~and~Daughter~I.", 2);
                            data.AddOption("See~ya.", 255)
                       .AddAvatar(116).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("Ever has it been, that love knows not its own depth until the hour of separation... Oh, sorry, I miss my husband... Well, which quest are you coming for?")
                         .AddOption("Leave.", 255)
                         .AddAvatar(116).FinalizeDialog();
                            break;
                        }
                        data.AddText("Ever has it been, that love knows not its own depth until the hour of separation... Oh, sorry, I miss my husband... Well, which quest are you coming for?");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Here`s~a~Letter~Home~for~you.", 1);
                        else
                            data.AddOption("Father~and~Daughter~I.", 2);
                        data.AddOption("See~ya.", 255)
                         .AddAvatar(116).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                            {
                                //Some EXP, 300 Silver, Bound EXP Potion
                                client.IncreaseExperience(stream, 5000, Role.Flags.ExperienceEffect.angelwing, true);

                                client.Player.Money += 300;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.Inventory.Remove(720777, 1, stream);
                                client.Inventory.Add(stream, Database.ItemType.DoubleExp, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                data.AddText("What? My husband is dead? You can`t be serious!")
                                .AddText("~Please don`t tell my little girl. She couldn`t take it...")
                           .AddOption("I~won`t.", 2)
                           .AddAvatar(116).FinalizeDialog();
                            }
                        }


                        break;
                    }
                case 2:
                    {
                        data.AddText("My daughter, Yu Jing, goes to the northwest everyday, in hopes that she can see her father returning home.")
                               .AddText("~It has been many hours since she went out, today. I`m worried about her. Can you go there and bring her home, please?")
                          .AddOption("You~can~count~on~me.", 3)
                          .AddAvatar(116).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        data.AddText("You~are~on~the~way~to~look~for~Yu~Jing.")
                            .AddOption("Yes.", 4).AddOption("No.", 255)
                            .AddAvatar(116).FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        data.AddText("I can`t think of any need in childhood as strong as the need for a father`s protection. Well, are you coming for any quest?")
                                  .AddOption("Yes.", 5).AddOption("No.", 255)
                                  .AddAvatar(116).FinalizeDialog();


                        break;
                    }
                case 5:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher(ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);
                        break;
                    }
                case 6:
                    {
                        data.AddText("Why are you alone? Where`s my daughter?")
                                .AddOption("She~will~be~back,~soon.", 7)
                                .AddAvatar(116).FinalizeDialog();

                        break;
                    }
                case 7:
                    {
                        if (!client.Inventory.HaveSpace(3))
                        {
                            data.AddText("Please make 3 more spaces in your inventory.")
                             .AddOption("let me check.", 255)
                             .AddAvatar(116).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            client.IncreaseExperience(stream, 5000, Role.Flags.ExperienceEffect.angelwing, true);
                            client.Player.Money += 500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've received some EXP and 500 Silver, and Uniquer Soft Boots(B).", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            client.Inventory.Add(stream, 160057, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 721673);
                            if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                                client.Inventory.Add(stream, 121047, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else

                                client.Inventory.Add(stream, 120047, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);

                            client.Player.QuestGUI.SendAutoPatcher(1011, 211, 312, 8266);
                        }


                        //8266,1011,0211,0312
                        break;
                    }
            }
        }
        //[NpcAttribute(NpcID.MasterHao)]
        //public static void MasterHao(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog data = new Dialog(client, stream);
        //    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.MasterHao, client.Player.Class, 1023);
        //    var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Fortuneteller, client.Player.Class, 1017);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
        //                {
        //                    if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
        //                    {
        //                        client.CreateBoxDialog("The~quest~`Capture~Spirit~I`~is~completed.~You`ve~received~500~Silver~and~some~EXP!");
        //                        client.Player.Money += 500;
        //                        client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
        //                        client.GainExpBall(10, true, Role.Flags.ExperienceEffect.angelwing);
        //                    }
        //                }
        //                data.AddText("A holy habit cannot cleanse a foul soul. What can I do for you, my child?")
        //       .AddOption("Letter~Home.", 1)
        //       .AddOption("See~ya.", 255)
        //       .AddAvatar(67).FinalizeDialog();


        //                break;
        //            }
        //        case 1:
        //            {
        //                data.AddText("I just found this letter. It was a poem written by a villager, specially for his wife.")
        //                    .AddText("~Unfortunately, it seems he was killed by the Grisly Specter, while he was on his way home...")
        //                .AddOption("I`m~sorry~to~hear~that.", 2)
        //                .AddAvatar(67).FinalizeDialog();


        //                break;
        //            }
        //        case 2:
        //            {
        //                data.AddText("I have a bad feeling that awful things are going to happen in Phoenix Castle.")
        //                    .AddText("~Can you pay a visit to the Forest Governor and pass along this message, for me?")
        //                .AddOption("Sure.", 3)
        //                .AddAvatar(67).FinalizeDialog();


        //                break;
        //            }
        //        case 3:
        //            {
        //                if (client.Inventory.HaveSpace(1))
        //                {
        //                    client.Teleport(212, 258, 1011);
        //                    if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
        //                    {
        //                        client.Inventory.Add(stream, 720777);
        //                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
        //                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
        //                    }
        //                    data.AddText("You~were~teleported~to~Phoenix~Castle.~Hurry~to~visit~the~Forest~Governor!").FinalizeDialog();
        //                    client.Player.QuestGUI.SendAutoPatcher("This~is~extremely~important.~Take~this~letter~to~the~widow,~Yu~Lin.", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);
        //                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "archer-s");


        //                }
        //                else
        //                {
        //                    data.AddText("Please make 1 more space in your inventory.")
        //                .AddOption("let me check.", 255)
        //                .AddAvatar(67).FinalizeDialog();
        //                }


        //                break;
        //            }
        //    }
        //}
        //[NpcAttribute(NpcID.Fortuneteller)]
        //public static void Fortuneteller(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog data = new Dialog(client, stream);
        //    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.Fortuneteller, client.Player.Class, 1016);
        //    var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Fortuneteller, client.Player.Class, 1017);
        //    switch (Option)
        //    {
        //        //Hello, there! Would you like to challenge the Eight-Diagram Dimensions, or any other quests?
        //        case 0:
        //            {
        //                data.AddText("Hello, there! Would you like to challenge the Eight-Diagram Dimensions, or any other quests?")
        //                  .AddOption("Capture~Spirit.", 1)
        //                    //  .AddOption("Dimensions?", 2) not add.
        //                  .AddOption("Just~passing~by.", 255)
        //                  .AddAvatar(7).FinalizeDialog();


        //                break;
        //            }
        //        case 1:
        //            {
        //                if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
        //                {
        //                    data.AddText("It is strange that the spirits of Apparitions have been sealed by a mysterious power. I tried every possible way but none have worked!")
        //                                                      .AddText("~Can you pay a visit to Master Hao and ask him how to break the seal?")
        //                                                      .AddOption("Sure.", 3)
        //                                                      .AddAvatar(7).FinalizeDialog();
        //                    break;
        //                }
        //                if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted) == false)
        //                {
        //                    data.AddText("I have been studying about spirits for years. Recently I invented this Spirit Container, but I`m not sure if it works.")
        //                        .AddText("~Would you like to capture the spirits of 10 Apparitions for me? I need to test if the aura is strong enough.")
        //            .AddOption("I`m~on~my~way.", 255)
        //            .AddAvatar(7).FinalizeDialog();
        //                    if (client.Inventory.HaveSpace(1))
        //                    {
        //                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
        //                        client.Player.QuestGUI.SendAutoPatcher("You~are~going~to~capture~the~spirits~of~10~Apparitions.", 1002, 610, 510, 0);
        //                        client.Inventory.Add(stream, 711243);
        //                    }
        //                    else
        //                    {
        //                        data.AddText("Please make 1 more space in your inventory.")
        //                               .AddOption("Let me check.", 255)
        //                               .AddAvatar(7).FinalizeDialog();
        //                    }
        //                    break;
        //                }
        //                else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
        //                {
        //                    if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 10))
        //                    {
        //                        data.AddText("The Spirit Container is working! Thanks a million!")
        //                   .AddOption("You`re~welcome..", 2)//2
        //                   .AddAvatar(7).FinalizeDialog();
        //                    }
        //                    else
        //                    {
        //                        data.AddText("Please take this Spirit Container and head out to capture the spirits of 10 Apparitions.")
        //                            .AddOption("I`m~on~my~way.", 255)
        //                            .AddAvatar(7).FinalizeDialog();
        //                    }
        //                    break;
        //                }


        //                break;
        //            }
        //        case 2:
        //            {
        //                if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
        //                {
        //                    client.CreateBoxDialog("The~quest~`Capture~Spirit~I`~is~completed.~You`ve~received~500~Silver~and~some~EXP!");
        //                    client.Player.Money += 500;
        //                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
        //                    client.GainExpBall(10, true, Role.Flags.ExperienceEffect.angelwing);
        //                    client.Inventory.Remove(711243, 1, stream);
        //                    data.AddText("It is strange that the spirits of Apparitions have been sealed by a mysterious power. I tried every possible way but none have worked!")
        //                           .AddText("~Can you pay a visit to Master Hao and ask him how to break the seal?")
        //                           .AddOption("Sure.", 3)
        //                           .AddAvatar(7).FinalizeDialog();
        //                }


        //                break;
        //            }
        //        case 3:
        //            {
        //                if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
        //                {
        //                    data.AddText("Please visit Master Hao and ask him how to break the seal that is binding the spirits.")
        //                           .AddOption("I`ll~go~now.", 255)
        //                           .AddAvatar(7).FinalizeDialog();
        //                    break;
        //                }
        //                client.Player.QuestGUI.SendAutoPatcher("You~are~on~the~way~to~see~Master~Hao.", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);
        //                client.Player.QuestGUI.Accept(ActiveQuest2, 0);

        //                break;
        //            }
        //    }
        //}

        [NpcAttribute(NpcID.RuHua)]
        public static void RuHua(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.XuLiang, client.Player.Class, 1012);//1013
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.RuHua, client.Player.Class, 1013);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.RuHua, client.Player.Class, 1014);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.XuLiang, client.Player.Class, 1015);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("To the world you may be one person, but to one person you may be the world! I hope I can find my Mr. Right, soon.")
                                .AddOption("Good~luck!", 255)
                                .AddAvatar(129).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("What are you doing back here, so soon? Did my love enjoy the handkerchief? I do hope so!")
                       .AddOption("Send~you~a~message.", 10)
                       .AddOption("Just~passying~by.", 255)
                       .AddAvatar(129).FinalizeDialog();
                            break;
                        }
                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("To the world you may be one person, but to one person you may be the world! I hope I can find my Mr. Right, soon.")
                                .AddText("~Oh, what? You have something for me?")
                        .AddOption("A~surprise~for~you.", 1)
                        .AddOption("Good~luck!", 255)
                        .AddAvatar(129).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (!client.Inventory.Contain(711241, 1))
                            {
                                data.AddText("Well, I have always had a bit of a crush on him... I wanted to make a Handkerchief and give it to him, but it was stolen by the Apparitions!")
                      .AddText("~Could you please help me to retrieve it?")
                      .AddOption("I`ll~go~now.", 255)
                      .AddAvatar(129).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("To the world you may be one person, but to one person you may be the world. I hope I can find my Mr. Right soon. What are you doing here?")
                    .AddOption("Do~you~love~Xu~Liang?", 5).AddOption("Good~luck!", 255)
                    .AddAvatar(129).FinalizeDialog();
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("I`ve sewn some patterns on the handkerchief. Could you please help me to send this Mandarin Duck Hanky to Xu Liang?")
                .AddOption("Okay.", 255)
                .AddAvatar(129).FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("To the world you may be one person, but to one person you may be the world. I hope I can find my Mr. Right soon. What are you doing here?")
                      .AddOption("Do~you~love~Xu~Liang?", 6)
                      .AddOption("Good~luck!", 255)
                      .AddAvatar(129).FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("To the world you may be one person, but to one person you may be the world. I hope I can find my Mr. Right soon. What are you doing here?")
                       .AddOption("Do~you~love~Xu~Liang?", 3)
                       .AddOption("Good~luck!", 255)
                       .AddAvatar(129).FinalizeDialog();
                        }



                        break;
                    }
                case 10:
                    {
                        data.AddText("You mean Xu Liang wants to take me on a date beside the river, tonight? Okay, I see. Thanks for telling me! I`d love to!")
                          .AddOption("Best~wishes~for~you.", 11)
                          .AddAvatar(129).FinalizeDialog();


                        break;
                    }
                case 11:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId);
                            client.CreateBoxDialog("You`ve~completed~`Love~Confession~5`~and~received~200~Silver~and~some~EXP!");
                            client.Player.Money += 200;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.GainExpBall(10, true, Role.Flags.ExperienceEffect.angelwing);

                            data.AddText("It`s said that the Fortuneteller nearby has been studying spirits for years and recently invented some new things.")
                                .AddText("~Go pay him a visit, if you wish.")
                       .AddOption("I`m~on~my~way.", 12)
                       .AddOption("Not~interested.", 255)
                       .AddAvatar(129).FinalizeDialog();

                        }
                        break;
                    }
                case 5:
                    {
                        data.AddText("Yes! This is the Handkerchief I made it, myself. Thanks for your help!")
                         .AddOption("No~problem.", 6)
                         .AddAvatar(129).FinalizeDialog();
                        client.Inventory.Remove(711241, 1, stream);
                        client.CreateBoxDialog("You`ve~completed~`Love~Confession~3`~and~received~500~Silver~and~EXP~worth~1/12~of~an~EXP~Ball.");
                        client.Player.Money += 200;
                        client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        client.GainExpBall(30, true, Role.Flags.ExperienceEffect.angelwing);
                        client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId);


                        break;
                    }
                case 6:
                    {
                        data.AddText("I`ve sewn some beautiful patterns on this handkerchief. Could you please help me send this Mandarin Duck Hanky to Xu Liang?")
                        .AddOption("Okay.", 7)
                        .AddAvatar(129).FinalizeDialog();

                        break;
                    }
                case 7:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                            client.Player.QuestGUI.SendAutoPatcher("Go~send~the~Mandarin~Duck~Hanky~to~Xu~Liang.", ActiveQuest3.FinishNpcId.Map, ActiveQuest3.FinishNpcId.X, ActiveQuest3.FinishNpcId.Y, ActiveQuest3.FinishNpcId.ID);
                            client.Inventory.Add(stream, 711242);
                            client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest3.MissionId, 1);
                        }
                        else
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                        .AddOption("let me check", 255)
                        .AddAvatar(129).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        data.AddText("Wow, a Bouquet of Peach Blossoms! It`s from Xu Liang? Well, why don`t he send this by himself?")
                        .AddOption("Do~you~know~he~loves~you?", 2)
                        .AddAvatar(129).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        client.Inventory.Remove(711240, 1, stream);
                        client.CreateBoxDialog("You`ve~completed~Love~Confession~2~and~received~200~silver~and~EXP~worth~1/30~of~an~EXP~Ball");
                        client.Player.Money += 200;
                        client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        client.GainExpBall(30, true, Role.Flags.ExperienceEffect.angelwing);
                        client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);


                        goto case 3;
                    }
                case 3:
                    {
                        data.AddText("Well, I have always had a bit of a crush on him... I wanted to make a Handkerchief and give it to him, but it was stolen by the Apparitions!")
                      .AddText("~Could you please help me to retrieve it?")
                            .AddOption("No~problem.", 4)
                          .AddAvatar(129).FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Go~kill~the~Apparitions~and~bring~back~the~Handkerchief~to~Ru~Hua.", 1002, 627, 526, 0);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.XuLiang)]
        public static void XuLiang(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TCViceCaptain, client.Player.Class, 1008);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.XuLiang, client.Player.Class, 1009);//2365
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.XuLiang, client.Player.Class, 2365);//1011
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.XuLiang, client.Player.Class, 1011);//1012
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.XuLiang, client.Player.Class, 1012);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.RuHua, client.Player.Class, 1014);
            var ActiveQuest7 = Database.QuestInfo.GetFinishQuest((uint)NpcID.XuLiang, client.Player.Class, 1015);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("My mission is to ensure the security and peace of Twin City. What are you doing here?")
                   .AddOption("Nothing.", 255)
                   .AddAvatar(30).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Splendid! I`m madly in love with Ru Hua! I really appreciate your help! Hmm, I want to take her on a date beside the river at night.")
                                .AddText("~Please do me one last favor and send this message to her, okay?")
                     .AddOption("I`ll~go~now.", 255)
                     .AddAvatar(30).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("My mission is to ensure the security and peace of Twin City. Hey, how is it going?")
                     .AddOption("I~have~something~for~you.", 30)
                     .AddOption("Nothing.", 255)
                     .AddAvatar(30).FinalizeDialog();
                            break;
                        }
                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("My mission is to ensure the security and peace of Twin City. What are you doing here?")
                        .AddOption("Here~is~a~letter~for~you.", 1)
                        .AddOption("Nothing.", 255)
                        .AddAvatar(30).FinalizeDialog();
                            break;
                        }

                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 10))
                            {
                                if (client.Inventory.HaveSpace(2) == false)
                                {
                                    data.AddText("Please make 2 more spaces in your inventory.")
                                    .AddOption("Let me check.", 255)
                                    .AddAvatar(30).FinalizeDialog();
                                    break;
                                }
                                client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId);
                                client.CreateBoxDialog("You~completed~`Robin~Elimination`~and~received~500~Silver,~some~EXP~and~3~Small~Joy~Stones!");
                                client.Player.Money += 500;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.GainExpBall(200, true, Role.Flags.ExperienceEffect.angelwing);
                                client.Inventory.AddItemWitchStack(720173, 0, 3, stream);
                                client.Inventory.Add(stream, 729508);
                                data.AddText("Good job! Thanks for your help! Here is a reward for you. Take it!")
                                  .AddOption("Okay,~see~you.", 8)
                                  .AddAvatar(30).FinalizeDialog();
                                break;
                            }
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("My mission is to ensure the security and peace of Twin City. You still want to take the Robins Elimination quest?")
                                    .AddOption("Absolutely.", 7)
                                    .AddOption("Nothing.", 255)
                                    .AddAvatar(30).FinalizeDialog();
                                break;
                            }
                            goto case 4;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest3.MissionId, 100))
                            {
                                client.CreateBoxDialog("You~received~1,300~silver~and~13~minutes~of~EXP,~and~you`ll~receive~Double~EXP~for~the~next~25~minutes!");
                                client.Player.Money += 1200;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.Player.RateExp = 2;
                                client.Player.DExpTime = 25 * 60;
                                client.Player.CreateExtraExpPacket(stream);
                                client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId);
                                goto case 20;
                            }

                            data.AddText("My~mission~is~to~ensure~the~security~and~peace~of~Twin~City.~What~are~you~doing~here?")
                                .AddOption("Experience~XP~skill.", 12)
                                .AddOption("Nothing.", 255)
                                .AddAvatar(30).FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest4.MissionId, 5))
                            {
                                data.AddText("My mission is to ensure the security and peace of Twin City. Would you please do me a favor? ")
                      .AddOption("Sure~thing.", 24)
                      .AddOption(" Leave.", 255)
                      .AddAvatar(30).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("My mission is to ensure the security and peace of Twin City. Would you please do me a favor? ")
                        .AddOption("Sure~thing.", 22)
                        .AddOption(" Leave.", 255)
                        .AddAvatar(30).FinalizeDialog();
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Well, I`ve made a Bouquet of Peach Blossoms , but I`m afraid that she may refuse me when she saw the flowers...")
                                .AddText("Nothing.")
                                .AddAvatar(30).FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            goto case 31;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("My mission is to ensure the security and peace of Twin City. What are you doing here?")
                                 .AddText(" ~Can you help me to send this Bouquet of Peach Blossoms to Ru Hua?")
                         .AddOption("No problem.", 255)
                         .AddAvatar(30).FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            goto case 27;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            goto case 20;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            goto case 8;
                        }


                        break;
                    }
                case 30:
                    {
                        client.Inventory.Remove(711242, 1, stream);
                        data.AddText("Wow, Ru Hua asked you to give me this Mandarin Duck Hanky? How terrific! She has accepted my love! I`m so happy! ")
                         .AddOption("Best~wishes~for~you!", 31)
                         .AddAvatar(30).FinalizeDialog();
                        client.CreateBoxDialog("You`ve~completed~`Love~Confession~4`~and~received~300~Silver~and~EXP~worth~1/20~of~an~EXP~Ball.");
                        client.Player.Money += 300;
                        client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        client.GainExpBall(15, true, Role.Flags.ExperienceEffect.angelwing);
                        client.Player.QuestGUI.FinishQuest(ActiveQuest6.MissionId);


                        break;
                    }
                case 31:
                    {
                        data.AddText("Splendid! I`m madly in love with Ru Hua! I really appreciate your help! Hmm, I want to take her on a date beside the river at night. ")
                                .AddText("~Please do me one last favor and send this message to her, okay?")
                            .AddOption("No~problem.", 32)
                            .AddAvatar(30).FinalizeDialog();

                        break;
                    }
                case 32:
                    {
                        data.AddText("Splendid! I`m madly in love with Ru Hua! I really appreciate your help! Hmm, I want to take her on a date beside the river at night.")
                              .AddText("~Please do me one last favor and send this message to her, okay?")
                          .AddOption("I`ll~go~now.", 255)
                          .AddAvatar(30).FinalizeDialog();
                        client.Player.QuestGUI.Accept(ActiveQuest7, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Send~a~message~to~Ru~Hua.", ActiveQuest7.FinishNpcId.Map, ActiveQuest7.FinishNpcId.X, ActiveQuest7.FinishNpcId.Y, ActiveQuest7.FinishNpcId.ID);


                        break;
                    }
                case 26:
                    {
                        data.AddText("Well, I`ve made a bouquet of peach blossoms, but I`m afraid that she may refuse me when she sees the flowers... ")
                             .AddText("~Can you help me to send this Bouquet of Peach Blossoms to Ru Hua?")
                         .AddOption("My~pleasure.", 28)
                         .AddAvatar(30).FinalizeDialog();


                        break;
                    }
                case 27:
                    {
                        data.AddText("My mission is to ensure the security and peace of Twin City. Well, I do need your help again. You won`t say no, right? ")
                               .AddText("~Can you help me to send this Bouquet of Peach Blossoms to Ru Hua?")
                           .AddOption("Just~tell~me.", 26)
                          .AddOption("Leave.", 255)
                           .AddAvatar(30).FinalizeDialog();


                        break;
                    }
                case 28:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Add(stream, 711240);
                            client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                            client.Player.QuestGUI.SendAutoPatcher("Please~give~the~Bouquet~of~Peach~Blossoms~to~Ru~Hua.", ActiveQuest5.FinishNpcId.Map, ActiveQuest5.FinishNpcId.X, ActiveQuest5.FinishNpcId.Y, ActiveQuest5.FinishNpcId.ID);
                            client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest5.MissionId, 1);
                        }
                        else
                        {
                            data.AddText("Please make 1 more space in you inventory. ")
                         .AddOption("Let me check", 255)
                          .AddAvatar(30).FinalizeDialog();

                        }


                        break;
                    }
                case 24:
                    {
                        data.AddText(" Wow, look at the beautiful peach blossoms! Thanks a lot! I`ll make a bouquet of peach blossoms and give them to Ru Hua!")
                         .AddOption("Good~luck!", 26)
                         .AddAvatar(30).FinalizeDialog();
                        client.CreateBoxDialog("You`ve~completed~`Love~Confession~1`~and~received~400~Silver~and~EXP~worth~1/15~of~an~EXP~Ball!");
                        client.Player.Money += 400;
                        client.Inventory.Remove(711239, 5, stream);
                        client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        client.GainExpBall(15, true, Role.Flags.ExperienceEffect.angelwing);
                        client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId);


                        break;
                    }
                case 22:
                    {
                        data.AddText("Hmm, I have been secretly in love with Ru Hua for a long time... But I`m still not courageous enough to tell her of my love. ")
                           .AddText("~Could you please help me pick 5 Peach Blossoms from the Peach Trees, so I can make a bouquet of Peach Blossoms to give to her?")
                       .AddOption("I`ll~go~now.", 255)
                       .AddAvatar(30).FinalizeDialog();


                        break;
                    }
                case 20:
                    {
                        data.AddText(" Hmm, I have been secretly in love with Ru Hua for a long time... But I`m still not courageous enough to tell her of my love.")
                            .AddText("~Could you please help me pick 5 Peach Blossoms from the Peach Trees, so I can make a bouquet of Peach Blossoms to give to her?")

                        .AddOption("No~problem.", 21)
                        .AddOption(" Leave.", 255)
                        .AddAvatar(30).FinalizeDialog();

                        break;
                    }
                case 21:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Go~pick~5~Peach~Blossoms~from~the~Peach~Trees,~nearby.", 1002, 480, 701, 7997);


                        break;
                    }
                case 1:
                    {
                        data.AddText("Oh, it`s a Plume Letter from the TC Vice Captain! Something must be urgent. Thanks for your help!")
                        .AddOption("You`re~welcome.", 2)
                        .AddAvatar(30).FinalizeDialog();


                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(711237, 1))
                        {
                            client.Inventory.Remove(711237, 1, stream);
                            client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                            client.CreateBoxDialog("You~completed~`Letter~Delivery~3`,~and~received~500~Silver~and~5~minutes~of~EXP!");
                            client.Player.Money += 500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Player.RateExp = 2;
                            client.Player.DExpTime = 5 * 60;
                            client.Player.CreateExtraExpPacket(stream);
                            data.AddText("Recently, lots of robins invaded the Wind Plain. Could you go kill 5 Robins and restore peace to Wind Plain?")
                     .AddOption("Piece~of~cake!", 4)
                     .AddAvatar(30).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Oh, where it`s a Plume Letter from the TC Vice Captain?")
              .AddOption("I lost that.", 255)
              .AddAvatar(30).FinalizeDialog();
                        }


                        break;
                    }

                case 4:
                    {
                        data.AddText("My mission is to ensure the security and peace of Twin City. Hey, I have 2 quests for you: Robins Elimination and Love Confession. Which one are you interested in?")
                            .AddOption("Robin~Elimination.", 5)
                            .AddOption("Nothing.", 255)
                            .AddAvatar(30).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        data.AddText("By the way, Artisan Luo is looking for someone to help him gather some Robin Feather. Maybe you can give him a hand?")
                             .AddOption("Sure.", 6)
                             .AddAvatar(30).FinalizeDialog();
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Go~kill~10~Robins~and~then~return~to~Xu~Liang.", 1002, 468, 638, 0);


                        break;
                    }
                case 6:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(1002, 442, 431, 4467);//4467,2,5510,1002,442,431
                        break;
                    }

                case 7:
                    {
                        data.AddText("Good. Then go kill 10 Robins and restore peace to Wind Plain!")
                              .AddOption("Piece~of~cake.", 255)
                              .AddAvatar(30).FinalizeDialog();


                        break;
                    }
                case 8:
                    {
                        Role.Flags.SpellID spell = (Role.Flags.SpellID)0;
                        if (Database.AtributesStatus.IsTrojan(client.Player.Class))
                            spell = Role.Flags.SpellID.Cyclone;
                        else if (Database.AtributesStatus.IsWarrior(client.Player.Class))
                            spell = Role.Flags.SpellID.Superman;
                        else if (Database.AtributesStatus.IsArcher(client.Player.Class))
                            spell = Role.Flags.SpellID.XpFly;
                        else if (Database.AtributesStatus.IsNinja(client.Player.Class))
                            spell = Role.Flags.SpellID.FatalStrike;
                        else if (Database.AtributesStatus.IsMonk(client.Player.Class))
                            spell = Role.Flags.SpellID.Oblivion;
                        else if (Database.AtributesStatus.IsPirate(client.Player.Class))
                            spell = Role.Flags.SpellID.CannonBarrage;
                        else if (Database.AtributesStatus.IsLee(client.Player.Class))
                            spell = Role.Flags.SpellID.DragonCyclone;
                        else if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                            spell = Role.Flags.SpellID.Lightning;
                        else if (Database.AtributesStatus.IsWindWalker(client.Player.Class))
                            spell = Role.Flags.SpellID.Omnipotence;
                        else if (Database.AtributesStatus.IsThunderStriker(client.Player.Class))
                            spell = Role.Flags.SpellID.ThunderRampage;

                        if (spell != 0)
                        {
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)spell))
                                client.MySpells.Add(stream, (ushort)spell);
                        }
                        data.AddText("In this dangerous world, a powerful XP skill can make you stronger. Now I`ll teach you a unique XP skill, " + spell.ToString() + "")
                                .AddOption("What~is~it?", 10)
                                .AddOption("I`m~just~passing~by.", 255)
                                .AddAvatar(30).FinalizeDialog();


                        break;
                    }
                case 10:
                    {
                        if (Database.AtributesStatus.IsTrojan(client.Player.Class))
                        {
                            data.AddText("The Trojans have unique XP skill which disguises them as a robot and wreaks havoc amongst enemies.")
                              .AddOption("Kill~Robins.", 11)
                              .AddOption("I~see.", 255)
                              .AddAvatar(30).FinalizeDialog();
                        }
                        else if (Database.AtributesStatus.IsWarrior(client.Player.Class))
                        {
                            data.AddText("The 'Superman' XP Skill is unique, possessed only by the Warrior. When activated it causes a death blow, decupled damage!")
                              .AddOption("Kill~Robins.", 11)
                              .AddOption("I~see.", 255)
                              .AddAvatar(30).FinalizeDialog();
                        }
                        else if (Database.AtributesStatus.IsArcher(client.Player.Class))
                        {
                            data.AddText("The ability to 'Fly' is the unique ability of the archer. Archers who fly are ")
                                .AddText(" immune to physical attacks and can shoot without fear of being attacked.")
                              .AddOption("Kill~Robins.", 11)
                              .AddOption("I~see.", 255)
                              .AddAvatar(30).FinalizeDialog();
                        }
                        else if (Database.AtributesStatus.IsNinja(client.Player.Class))
                        {
                            data.AddText("The ninja's skills can cause massive damage in a split second, or disable enemies, or prevent them from healing themselves.\n\n")
                                 .AddText("Fatal Strike Boosts Ninja's speed and attack, allowing the Ninja to blink between ")
                                 .AddText("targets and kill them instantly. This can only be used against monsters (not players).")
                               .AddOption("Kill~Robins.", 11)
                               .AddOption("I~see.", 255)
                               .AddAvatar(30).FinalizeDialog();

                        }
                        else if (Database.AtributesStatus.IsMonk(client.Player.Class))
                        {
                            data.AddText("Move speed will increase. Inflict 200% damage on targets. For each monster killed within ")
                                .AddText(" 30 seconds after the XP skill is activated, the EXP awarded will be increased by 1/2.")
                              .AddOption("Kill~Robins.", 11)
                              .AddOption("I~see.", 255)
                              .AddAvatar(30).FinalizeDialog();
                        }
                        else if (Database.AtributesStatus.IsPirate(client.Player.Class))
                        {
                            data.AddText("(Cannon Barrage)  Inflict 400% damage on all monsters (100% damage on bosses), 10% of normal damage ")
                                .AddText(" on players within 5 paces every 2 seconds. ")
                              .AddOption("Kill~Robins.", 11)
                              .AddOption("I~see.", 255)
                              .AddAvatar(30).FinalizeDialog();
                        }
                        else if (Database.AtributesStatus.IsLee(client.Player.Class))
                        {
                            data.AddText("The XP skill needs a certain period of time to accumulate its power. Once cast, you can kill a lot of monsters in a very short time. ")
                              .AddText("You`ve already learned DragonCyclone, and have 5 minutes to experience it. Use it to kill 100 Robins in ")
                              .AddText("~5 minutes, and you`ll be handsomely rewarded. What`s more, if you can kill double the amount, your reward will be even better!")
                            .AddOption("Kill~Robins.", 11)
                            .AddOption("I~see.", 255)
                            .AddAvatar(30).FinalizeDialog();
                        }
                        else if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                        {
                            data.AddText("Taoists with no magic is a sitting duck. They cast skills or magic from range and their  ")
                                .AddText("XP skills will cause even greater damage.\n")
                                .AddText("The XP skill needs a certain period of time to accumulate its power. Once cast, you can kill a lot of monsters in a very short time.")
                              .AddOption("Kill~Robins.", 11)
                              .AddOption("I~see.", 255)
                              .AddAvatar(30).FinalizeDialog();
                        }
                        else if (Database.AtributesStatus.IsWindWalker(client.Player.Class))
                        {
                            data.AddText("Windwalker XP skill. Equip Windwalker fan in both hands to cast. Cast it to enter Omnipotence status which deals 10% of ")
                                .AddText("normal damage on enemies within 12 paces (350% damage on monsters), increases normal attack`s damage on ")
                                .AddText("monsters by 500%, and enhances the effects of Swirling Storm and Burnt Frost")
                              .AddOption("Kill~Robins.", 11)
                              .AddOption("I~see.", 255)
                              .AddAvatar(30).FinalizeDialog();
                        }

                        break;
                    }
                case 11:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Use your XP skill to kill 100 Robins!", 1002, 468, 638, 0);

                        break;
                    }
                case 12:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest3.MissionId, 100))
                        {
                            //  a
                        }
                        else
                        {
                            data.AddText("Use your XP skill to kill 100 Robins!")
                               .AddOption("Okay.", 255)
                               .AddAvatar(30).FinalizeDialog();
                        }


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.TCViceCaptain)]
        public static void TCViceCaptain(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TCViceCaptain, client.Player.Class, 1006);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TCViceCaptain, client.Player.Class, 1007);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TCViceCaptain, client.Player.Class, 1008);
            switch (Option)
            {
                case 0:
                    {
                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("Hi! I`m the TC Vice Captain, and I`m here to guard Twin City. Hey, do you think you can help me deliver a letter?")
                        .AddOption("Tell~me~more!", 1)
                        .AddOption("Just~passing~by.", 255)
                        .AddAvatar(18).FinalizeDialog();
                            break;
                        }
                        else if (!client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            goto case 4;
                        }
                        else if (!client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            goto case 7;
                        }
                        data.AddText("Hi! I`m the TC Vice Captain, and I`m here to guard Twin City. Don`t make trouble, or we`re going to have problems!")
                         .AddOption("I see.", 255)
                         .AddAvatar(18).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(711235, 1))
                        {
                            data.AddText("Well done! You`ve got the letter. Thanks a lot!")
                             .AddOption("No~problem.", 3)
                             .AddAvatar(18).FinalizeDialog();
                            break;
                        }
                        data.AddText("I want you to deliver a letter to Xu Liang, but the letter is being held by the Warehouseman. Could you please get it back for me?")
                            .AddOption("No~problem.", 2)
                            .AddOption(" I~have~no~time.", 255)
                            .AddAvatar(18).FinalizeDialog();



                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Go~retrieve~the~Military~Letter~from~the~Warehouseman.", 1002, 281, 251, 8);


                        break;
                    }
                case 3:
                    {
                        client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                        client.Inventory.Remove(711235, 1, stream);
                        client.Player.Money += 100;
                        client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        client.CreateBoxDialog("You~completed~Letter~Delivery~1~and~received~100~silver.");


                        goto case 4;
                    }
                case 4:
                    {
                        if (client.Inventory.Contain(711236, 1))
                        {
                            data.AddText("Hi! I`m the TC Vice Captain, and I`m here to guard Twin City. Hmm, do you want to take the quest `Letter Delivery 2`?")
    .AddOption("Yeah,~I`d~like~to.", 6)
    .AddOption(" I~have~no~time.", 255)
    .AddAvatar(18).FinalizeDialog();
                            break;
                        }
                        data.AddText("The Military Letter is quite important, so I need a Turtledove Plume to mark the letter. Could you please hunt some Turtledoves and bring back a Turtledove Plume for me?")
     .AddOption("No~problem.", 5)
     .AddOption(" I~have~no~time.", 255)
     .AddAvatar(18).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Hunt~the~Turtledoves~and~collect~a~Turtledove~Plume!", 1002, 340, 484, 0);


                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.HaveSpace(2) == false)
                        {
                            data.AddText("Please make 2 more spaces in your inventory.")
                            .AddOption("Let me check.", 255)
                            .AddAvatar(18).FinalizeDialog();
                            break;
                        }
                        if (client.Inventory.Contain(711236, 1))
                        {
                            client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId);
                            client.Inventory.Remove(711236, 1, stream);
                            client.CreateBoxDialog("You~completed~`Letter~Delivery~2`~and~received~300~Silver,~some~EXP,~5~Resolutives~and~a~Twin~City~Gate!");
                            client.GainExpBall(200, true, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.Money += 300;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Inventory.AddItemWitchStack(1000010, 0, 5, stream);
                            client.Inventory.Add(stream, 1060020);
                            goto case 7;
                        }



                        break;
                    }
                case 7:
                    {
                        data.AddText("Now, please hurry and deliver the Military Letter marked with the Turtledove Plume to Xu Liang in Wind Plain! It`s urgent!")
                              .AddOption("Got~it!", 8)
                              .AddOption("Leave.", 255)
                              .AddAvatar(18).FinalizeDialog();


                        break;
                    }
                case 8:
                    {
                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                            .AddOption("Let me check.", 255)
                            .AddAvatar(18).FinalizeDialog();
                            break;
                        }
                        if (client.Inventory.Contain(711237, 1))
                        {
                            data.AddText("Now, please hurry and deliver the Plume Letter to Xu Liang in Wind Plain! It`s urgent!")
                                .AddOption("Got~it!", 255)
                                .AddAvatar(18).FinalizeDialog();
                            break;
                        }
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Hurry~and~give~the~Plume~Letter~to~Xu~Liang~in~Wind~Plain.", ActiveQuest3.FinishNpcId.Map, ActiveQuest3.FinishNpcId.X, ActiveQuest3.FinishNpcId.Y, ActiveQuest3.FinishNpcId.ID);
                        client.Inventory.Add(stream, 711237);


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Shelby)]
        public static void Shelby(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("We~hope~all~can~help~each~other.~If~you~power~level~the~newbies,~we~may~reward~you~Experience~or~meteors.~Are~you~interested?")
                    .AddOption("Tell~me~more~details.", 1)
                    .AddOption("Check~my~virtue~points.", 5)
                    .AddOption("Claim~Prize.", 6)
                    .AddOption("Just~passing~by.", 255)
                    .AddAvatar(7).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        data.AddText("If~you~are~above~level~70~and~try~to~power~level~the~newbies~(at~least~20~levels~lower~than~you),~you~may~gain~virtue~points.")
                            .AddOption("What~are~the~virtue~points?", 2)
                            .AddAvatar(7).FinalizeDialog();



                        break;
                    }
                case 2:
                    {
                        data.AddText("The~more~newbies~you~power~level,~the~more~virtue~points~you~gain.~I~shall~give~you~a~good~reward~for~a~certain~virtue~points.")
                           .AddOption("How~can~I~gain~virtue~points?", 3)
                           .AddOption("What~prize~can~I~expect?", 4)
                           .AddAvatar(7).FinalizeDialog();


                        break;
                    }
                case 3:
                    {
                        data.AddText("Once~the~newbies~are~one~level~up,~the~team~captain~can~gain~virtue~points~accordingly.")
                             .AddOption("I~see.", 255)
                             .AddAvatar(7).FinalizeDialog();


                        break;
                    }
                case 4:
                    {
                        data.AddText("I~shall~reward~you~exp.~of~equivalent~to~an~ExpBall~for~15,000~virtue~points~or~a~meteor~for~5000~virtue~points or DragonBall for 20,000 virtue~points.")
                         .AddOption("I~see.", 255)
                         .AddAvatar(7).FinalizeDialog();


                        break;
                    }
                case 5:
                    {
                        data.AddText("Your~current~virtue~points~are~" + client.Player.VirtutePoints + ",~please~try~to~gain~more.")
                           .AddOption("Thanks.", 255)
                           .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        data.AddText("What~prize~do~you~prefer?")
                            .AddOption("Meteor.", 10)
                            .AddOption("Exp.", 11)
                            .AddOption("DragonBall", 12)
                            .AddOption("Let~me~think~it~over", 255)
                            .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 10:
                    {
                        if (client.Player.VirtutePoints >= 5000)
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                client.Player.VirtutePoints -= 5000;
                                client.Player.SendUpdate(stream, client.Player.VirtutePoints, MsgUpdate.DataType.VirtutePoints);
                                client.Inventory.Add(stream, Database.ItemType.Meteor);
                            }
                            else
                            {
                                data.AddText("Please make 1 more space in your inventory")
                                                         .AddOption("Let me check.", 255).AddAvatar(7)
                                                        .FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Sorry,~you~do~not~have~the~required~virtue~points.")
                            .AddOption("I~see.", 255)
                            .AddAvatar(7).FinalizeDialog();
                        }

                        break;
                    }
                case 11:
                    {
                        if (client.Player.Level >= 130)
                        {
                            data.AddText("Only the characters below level 130 can exchange the virtue points for exp. You may find Simon")
                                .AddText(" (Twin City 393,235) and spend 2,000 virtue points to hunt the treasure in the Labyrinth.")
                              .AddOption("Wow.~So~cool.", 255)
                              .AddAvatar(7).FinalizeDialog();
                            break;
                        }
                        if (client.Player.VirtutePoints >= 15000)
                        {
                            client.Player.VirtutePoints -= 15000;
                            client.Player.SendUpdate(stream, client.Player.VirtutePoints, MsgUpdate.DataType.VirtutePoints);
                            client.GainExpBall(600, true, Role.Flags.ExperienceEffect.angelwing);

                        }
                        else
                        {
                            data.AddText("Sorry,~you~do~not~have~the~required~virtue~points.")
                            .AddOption("I~see.", 255)
                            .AddAvatar(7).FinalizeDialog();
                        }

                        break;
                    }
                case 12:
                    {
                        if (client.Player.VirtutePoints >= 20000)
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                client.Player.VirtutePoints -= 20000;
                                client.Player.SendUpdate(stream, client.Player.VirtutePoints, MsgUpdate.DataType.VirtutePoints);
                                client.Inventory.Add(stream, Database.ItemType.DragonBall);
                            }
                            else
                            {
                                data.AddText("Please make 1 more space in your inventory")
                                                         .AddOption("Let me check.", 255).AddAvatar(7)
                                                        .FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Sorry,~you~do~not~have~the~required~virtue~points.")
                            .AddOption("I~see.", 255)
                            .AddAvatar(7).FinalizeDialog();
                        }

                        break;
                    }

            }
        }



        [NpcAttribute(NpcID.WhiteDragon)]
        public static void WhiteDragon(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("When you burn a [RosewoodIncrense], you`ll feel refreshed and clear-minded. ")
                   .AddText("I`d like to give you Buddha`s gift for this incrense. The gift are ")
                           .AddText("Monk`s Skill Bock, Chi Points. If you have those items, you can also swap ")
                           .AddText("them for the incense with me. So, what you say?")
                    .AddOption("Swap the incense for gifts.", 1)

                    .AddOption("I`ll think about it.", 255)
                    .AddAvatar(226).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        data.AddText("Diffrent gift requires diffrent amount of RoseWood incense. Here's an exchange ")
                  .AddText("list: [Grace of Heaven Scroll] - 10 incenses; [Wrath of the Emperor Scroll ] - 1 ")
                          .AddText("incense; [Infernal Echo Scroll] - 20 incenses; [2000 Chi Points] - 10 incenses;  Wich one do you like? ")
                   .AddOption("Infernal Echo Scroll (20).", 2)
                   .AddOption("Grace of Heaven Scroll (10).", 3)
                   .AddOption("Wrath of the Emperor Scroll (1).", 4)
                   .AddOption("2000 Chi Points (10).", 5)
                   .AddOption("I`ll think about it.", 255)
                   .AddAvatar(226).FinalizeDialog();


                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(3007565, 20))
                        {
                            client.Inventory.Remove(3007565, 20, stream);
                            client.Inventory.Add(stream, 3007568);
                        }
                        else
                        {
                            data.AddText("Please come back when you will have 20 RosewoodIncense in your inventory.")
       .AddOption("Oh.", 255)
       .AddAvatar(226).FinalizeDialog();
                        }



                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.Contain(3007565, 10))
                        {
                            client.Inventory.Remove(3007565, 10, stream);
                            client.Inventory.Add(stream, 3007567);
                        }
                        else
                        {
                            data.AddText("Please come back when you will have 10 RosewoodIncense in your inventory.")
       .AddOption("Oh.", 255)
       .AddAvatar(226).FinalizeDialog();
                        }

                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.Contain(3007565, 1))
                        {
                            client.Inventory.Remove(3007565, 1, stream);
                            client.Inventory.Add(stream, 3007566);
                        }
                        else
                        {
                            data.AddText("Please come back when you will have 1 RosewoodIncense in your inventory.")
       .AddOption("Oh.", 255)
       .AddAvatar(226).FinalizeDialog();
                        }

                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.Contain(3007565, 10))
                        {
                            client.Inventory.Remove(3007565, 10, stream);
                            client.Inventory.Add(stream, 3005128);
                        }
                        else
                        {
                            data.AddText("Please come back when you will have 5 RosewoodIncense in your inventory.")
       .AddOption("Oh.", 255)
       .AddAvatar(226).FinalizeDialog();
                        }


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.VicotryBuddha)]
        public static void VicotryBuddha(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("The mortal world is stil the best place for me . I received anlghtement here thousands years ago . ")
                   .AddText(" I hope you achieve the essence of Buddhism , too. With the [Buddha's Necklace]")
                           .AddText(", you'll have a chance to achieve that during meditation.")
                    .AddOption("Remove distractions (1 Necklace)", 1)
                    .AddOption("I wana to leave here", 3)
                    .AddOption("I prefer to keep them.", 255)
                    .AddAvatar(226).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("Are you sure you want to start this trial with the power of Buddha`sNecklace?")
                            .AddOption("Yes.", 2)
                            .AddOption("No.", 255)
                            .AddAvatar(226).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(3007563, 1))
                        {
                            client.Inventory.Remove(3007563, 1, stream);
                            if (client.Player.Rate(30))
                            {
                                client.CreateBoxDialog("You received BoundlessHeart.");
                                client.Inventory.AddItemWitchStack(3007564, 0, 1, stream);
                            }
                            else
                            {
                                client.CreateBoxDialog("You received RosewoodIncense.");
                                client.Inventory.AddItemWitchStack(3007565, 0, 1, stream);
                            }
                        }
                        else
                        {
                            data.AddText("Please come back when you will have 1 Buddha`sNecklace in your inventory.")
       .AddOption("Oh.", 255)
       .AddAvatar(226).FinalizeDialog();
                        }


                        break;
                    }
                case 3:
                    {
                        client.Teleport(37, 36, 1004);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.AltarCleanser)]
        public static void AltarCleanser(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Why did you keep me waiting so long ? Well i'm not wordy. To many Emotions and desires will ")
                   .AddText(" affect your study of Buddhism. You have 1/10 chance to remove a distracting")
                           .AddText(" thought with a [Spiritual Gem] each time .")
                           .AddText(" If you succeed , Ill make you a [Buddha`s Necklace] for you")
                    .AddOption("Remove distractions (1 gem)", 1)
                    .AddOption("Try 5 times. (5 gems)", 3)
                    .AddOption("I prefer to keep them.", 255)
                    .AddAvatar(226).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("Are you sure you want to start this trial with the power of SpiritualGem?")
            .AddOption("Yes.", 2)
            .AddOption("No.", 255)
            .AddAvatar(226).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(3007562, 1))
                        {
                            client.Inventory.Remove(3007562, 1, stream);
                            if (client.Player.Rate(10))
                            {
                                client.CreateBoxDialog("You received Buddha`sNecklace.");
                                client.Inventory.AddItemWitchStack(3007563, 0, 1, stream);
                            }
                            else
                            {
                                client.CreateBoxDialog("You received RosewoodIncense.");
                                client.Inventory.AddItemWitchStack(3007565, 0, 1, stream);
                            }
                        }
                        else
                        {
                            data.AddText("Please come back when you will have 1 SpiritualGem in your inventory.")
       .AddOption("Oh.", 255)
       .AddAvatar(226).FinalizeDialog();


                        }
                        break;
                    }
                case 3:
                    {
                        data.AddText("Are you sure you want to start this trial with the power of SpiritualGem?")
                 .AddOption("Yes.", 4)
                 .AddOption("No.", 255)
                 .AddAvatar(226).FinalizeDialog();


                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.Contain(3007562, 5))
                        {
                            client.Inventory.Remove(3007562, 5, stream);
                            if (client.Player.Rate(10))
                            {
                                client.CreateBoxDialog("You received Buddha`sNecklace.");
                                client.Inventory.AddItemWitchStack(3007563, 0, 1, stream);
                            }
                            else
                            {
                                client.CreateBoxDialog("You received RosewoodIncense.");
                                client.Inventory.AddItemWitchStack(3007565, 0, 1, stream);
                            }
                        }
                        else
                        {
                            data.AddText("Please come back when you will have 5 SpiritualGem in your inventory.")
       .AddOption("Oh.", 255)
       .AddAvatar(226).FinalizeDialog();
                        }

                        break;
                    }

            }
        }

        [NpcAttribute(NpcID.SuperPureOver)]
        public static void SuperPureOver(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("The oven contain great cosmological principle of everything. If you're ready,")
                                .AddText(" put the Star Crystal into the oven, and you'll have a 30% change to receive a  Divine Panacea. \n\n")
                                .AddText("This mythical pill is said to contain endless power to reshape your backsword on the main hand into an Epic Weapon.")

                     .AddOption("Make a Divine Panacea.", 1)
                     .AddOption("I see.", 255).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.HaveSpace(2))
                        {
                            if (client.Inventory.Contain(3006014, 1))
                            {
                                client.Player.EpicQuestThree_rdChance += 1;
                                client.Inventory.Remove(3006014, 1, stream);
                                if (Role.Core.Rate(3, 10))
                                {
                                    client.CreateBoxDialog("You've acquired the essence of Taoism, and received a Divine Panacea.");

                                    data.AddText("You've completely accquired the essence of Taoism, and received a Divine Panacea.")
                                    .AddText("With this mythycal pill, you can reshape the backsword on your main hand into an Epic Weapon.")
                                      .AddOption("I finally made it.", 255).FinalizeDialog();
                                    client.Inventory.Add(stream, 3006016, 1);
                                }
                                else
                                {
                                    client.Inventory.AddItemWitchStack(3006015, 0, 5, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "angelwing");

                                    data.AddText("You've ran the oven " + client.Player.EpicQuestThree_rdChance + " time(s), but failed to make Divine Panacea. Don't ")
                                        .AddText("lose hear.You still got 5 pieces of Yin Yang Fish from the oven. Every time")
                                        .AddText(" when you run the oven, you`ll make a 10% change to make a Divine Panacea.")
                                        .AddText(" Do you want to try it again with a Star Crystal?")
                                        .AddOption("Make a Divine Panacea.", 1)
                                       .AddOption("I see.", 255).FinalizeDialog();

                                }
                            }
                            else
                            {
                                data.AddText("You~don`t~have~the~Star~Crystal~with~you.~You~need~to~make~one~with~the~Elite~Pure~Oven,~first.")
                                       .AddOption("I see.", 255).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Please make 2 more spaces in your inventory")
                                                      .AddOption("Let me check.", 255)
                                                      .FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.PureTaoist)]
        public static void PureTaoist(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Yin Yang Fish is a special token to balance the energy of Yin and Yand. If you happen to ")
                                .AddText(" collect some, you can excgange them for Taoist Epic-skill books, Elite Hossu for left hand, Chi Points.")
                     .AddOption("Swap the Fish for rewards.", 1)
                     .AddOption("I'll think about it.", 255).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("Look, i have so many things for you to choose from. Each Item require different amount of ")
                               .AddText(" Yin Yang Fish. So, what would you like to exchange for?");

                        if (Database.AtributesStatus.IsWater(client.Player.Class))
                        {
                            data.AddOption("AuroraLotusScroll (20 Fish)", 2);
                            data.AddOption("BlessingTouchScroll (10 Fish)", 4);
                        }
                        else if (Database.AtributesStatus.IsFire(client.Player.Class))
                        {
                            data.AddOption("FlameLotusScroll (20 Fish)", 6);
                            data.AddOption("BreakingTouchScroll (10 Fish)", 8);
                        }
                        data.AddOption("2000 Chi Points (10 Fish)", 10)
                            .AddOption("DesireHossu (15 Fish)", 12)
                            .AddOption("I'll think about it.", 255).FinalizeDialog();

                        break;
                    }
                case 12:
                    {
                        data.AddText("You are sure you want to swap 15 pieces of Yin Yang Fish for an DesireHossu?")
                    .AddOption("Yes.", 13)
                    .AddOption("I'll think about it.", 255).FinalizeDialog();

                        break;
                    }
                case 13:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {

                            if (client.Inventory.Contain(3006015, 15))
                            {
                                client.Inventory.Remove(3006015, 15, stream);
                                client.Inventory.Add(stream, 619028);
                                client.CreateBoxDialog("You successfully exchange the Yin Yang Fish for an DesireHossu!");
                            }
                            else
                            {
                                data.AddText("Please come back when you will have 15 Yin Yang Fish in your inventory.")
                                     .AddOption("I see.", 255).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Please make 1 more space in your inventory")
                                                        .AddOption("Let me check.", 255)
                                                       .FinalizeDialog();
                        }

                        break;
                    }
                case 10:
                    {
                        data.AddText("You are sure you want to swap 10 pieces of Yin Yang Fish for 2000 Chi Points?")
                    .AddOption("Yes.", 11)
                    .AddOption("I'll think about it.", 255).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {

                            if (client.Inventory.Contain(3006015, 10))
                            {
                                client.Inventory.Remove(3006015, 10, stream);
                                client.Inventory.Add(stream, 3005128);
                                client.CreateBoxDialog("You successfully exchange the Yin Yang Fish for 2000 Chi Points!");
                            }
                            else
                            {
                                data.AddText("Please come back when you will have 10 Yin Yang Fish in your inventory.")
                                     .AddOption("I see.", 255).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Please make 1 more space in your inventory")
                                                        .AddOption("Let me check.", 255)
                                                       .FinalizeDialog();
                        }
                        break;
                    }
                case 8:
                    {
                        data.AddText("You are sure you want to swap 10 pieces of Yin Yang Fish for an Breaking Touch Scroll?")
                    .AddOption("Yes.", 9)
                    .AddOption("I'll think about it.", 255).FinalizeDialog();


                        break;
                    }
                case 9:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {

                            if (client.Inventory.Contain(3006015, 10))
                            {
                                client.Inventory.Remove(3006015, 10, stream);
                                client.Inventory.Add(stream, 3006019);
                                client.CreateBoxDialog("You successfully exchange the Yin Yang Fish for an Breaking Touch Scroll!");
                            }
                            else
                            {
                                data.AddText("Please come back when you will have 10 Yin Yang Fish in your inventory.")
                                     .AddOption("I see.", 255).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Please make 1 more space in your inventory")
                                                        .AddOption("Let me check.", 255)
                                                       .FinalizeDialog();
                        }

                        break;
                    }
                case 6:
                    {
                        data.AddText("You are sure you want to swap 20 pieces of Yin Yang Fish for an Flame Lotus Scroll?")
                    .AddOption("Yes.", 7)
                    .AddOption("I'll think about it.", 255).FinalizeDialog();

                        break;
                    }
                case 7:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {

                            if (client.Inventory.Contain(3006015, 20))
                            {
                                client.Inventory.Remove(3006015, 20, stream);
                                client.Inventory.Add(stream, 3006017);
                                client.CreateBoxDialog("You successfully exchange the Yin Yang Fish for an Flame Lotus Scroll!");
                            }
                            else
                            {
                                data.AddText("Please come back when you will have 20 Yin Yang Fish in your inventory.")
                                     .AddOption("I see.", 255).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Please make 1 more space in your inventory")
                                                        .AddOption("Let me check.", 255)
                                                       .FinalizeDialog();
                        }


                        break;
                    }
                case 4:
                    {
                        data.AddText("You are sure you want to swap 10 pieces of Yin Yang Fish for an BlessingTouchScroll?")
                    .AddOption("Yes.", 5)
                    .AddOption("I'll think about it.", 255).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {

                            if (client.Inventory.Contain(3006015, 10))
                            {
                                client.Inventory.Remove(3006015, 10, stream);
                                client.Inventory.Add(stream, 3006020);
                                client.CreateBoxDialog("You successfully exchange the Yin Yang Fish for an BlessingTouchScroll!");
                            }
                            else
                            {
                                data.AddText("Please come back when you will have 10 Yin Yang Fish in your inventory.")
                                     .AddOption("I see.", 255).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Please make 1 more space in your inventory")
                                                        .AddOption("Let me check.", 255)
                                                       .FinalizeDialog();
                        }

                        break;
                    }
                case 2:
                    {
                        data.AddText("You are sure you want to swap 20 pieces of Yin Yang Fish for an Aurora Lotus Scroll?")
                    .AddOption("Yes.", 3)
                    .AddOption("I'll think about it.", 255).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {

                            if (client.Inventory.Contain(3006015, 20))
                            {
                                client.Inventory.Remove(3006015, 20, stream);
                                client.Inventory.Add(stream, 3006018);
                                client.CreateBoxDialog("You successfully exchange the Yin Yang Fish for an Aurora Lotus Scroll!");
                            }
                            else
                            {
                                data.AddText("Please come back when you will have 20 Yin Yang Fish in your inventory.")
                                     .AddOption("I see.", 255).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Please make 1 more space in your inventory")
                                                        .AddOption("Let me check.", 255)
                                                       .FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ElitePureOver)]
        public static void ElitePureOver(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("This oven can balance the energy of Yin and Yang. When you put the ")
                                .AddText("Treasure Leaf, you'll have a 45% chance to get Star Crystal from the oven.")
                     .AddOption("Make a Star Crystal.", 2)
                     .AddOption("Make a Star Crystal(5 times)", 3)
                     .AddOption("I see.", 255).FinalizeDialog();


                        break;
                    }

                case 3:
                    {
                        if (client.Inventory.HaveSpace(3))
                        {
                            if (client.Inventory.Contain(3006013, 5))
                            {
                                client.Inventory.Remove(3006013, 5, stream);
                                bool ReceivedStar = false;
                                if (client.Player.Rate(30))
                                {
                                    client.Player.EpicQuestTwo_ndChance += 1;
                                    ReceivedStar = true;
                                }


                                for (int x = 0; x < (int)(ReceivedStar == true ? 4 : 5); x++)
                                {
                                    client.Player.EpicQuestTwo_ndChance += 1;

                                    {
                                        data.AddText("You`re ran the oven " + client.Player.EpicQuestTwo_ndChance + " time(s). Received YinYangFish. \n");
                                        client.Inventory.AddItemWitchStack(3006015, 0, 1, stream);
                                    }

                                }


                                if (ReceivedStar)
                                {
                                    client.CreateBoxDialog("You Successfully made a Star Crystal Now, you're qualified to take furthe alchemy at the Super Pure Oven.");
                                }
                                else
                                {
                                    data.AddOption("Run it 5 times more.", 3)
                                   .AddOption("I see.", 255).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Please come back when you will have 5 TreasureLeaf in your inventory.")
                                       .AddOption("I see.", 255).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Please make 4 more spaces in your inventory")
                                                       .AddOption("Let me check.", 255)
                                                      .FinalizeDialog();
                        }


                        break;
                    }
                case 5:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(3933, 48, 48, 25012);

                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Contain(3006013, 1))
                            {
                                client.Inventory.Remove(3006013, 1, stream);
                                if (client.Player.Rate(10))
                                {
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "zf2-e300");
                                    data.AddText("You've got a deep understanding on the truth of Taoism and received a Star ")
                                      .AddText("Crystal. Now you can put the Star Crystal into the Super Pure Oven and burn it ")
                                      .AddText(" with the World true fires to make a Divine Panacea. However it's not for 100% success rate.")
                                        .AddOption("Head to the Super Pure Oven.", 5)
                                        .AddOption("Keep making the Star Crystal.", 0)
                                        .FinalizeDialog();
                                    client.CreateBoxDialog("You Successfully made a Star Crystal Now, you're qualified to take furthe alchemy at the Super Pure Oven.");
                                    client.Inventory.Add(stream, 3006014);
                                }
                                else
                                {
                                    client.Inventory.AddItemWitchStack(3006015, 0, 1, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "angelwing");
                                    data.AddText("You`ve ran the oven " + client.Player.EpicQuestTwo_ndChance + " time(s). but failed to make a Star Crystald. Don't lose heart. ")
                                .AddText("You still got Ying Yang Fish from the oven. Every time when you run the oven, you'll have")
                                .AddText(" a 10% chance to make a Star Crystal. Do you want to try it again with a Treasure Leaf?")
                           .AddOption("Make a Star Crystal.", 2)
                           .AddOption("Make a Star Crystal(5 times)", 3)
                           .AddOption("I see.", 255).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Please come back when you will have 1 TreasureLeaf in your inventory.")
                                       .AddOption("I see.", 255).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Please make 1 more spaces in your inventory")
                                                       .AddOption("Let me check.", 255)
                                                      .FinalizeDialog();
                        }


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.RefinedPureOven)]
        public static void RefinedPureOven(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.EpicQuestChance == 0)
                        {
                            data.AddText("This oven is empty. To make it work, you need to put in 3 Red Flowers and 3 White Flowers, everyday.")
                        .AddOption("Put in the Flowers.", 1)
                        .AddOption("Active it with 27 Cps.", 2)
                        .AddOption("Enter~in~the~Evil~Abyss.", 3)
                        .AddOption("I see.", 255).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("The Red Flower and the White Flowers are composed in the oven. By collecting elements of the East,North, ")
                                .AddText("  West and South, this oven give a 10% change of making a Treasure Leaf.")
                     .AddOption("Run the process with 27 Cps.", 10)
                     .AddOption("Run 10 times with 270 Cps.", 12)
                     .AddOption("I see.", 255).FinalizeDialog();
                        }


                        break;
                    }
                case 12:
                    {
                        data.AddText("You are sure you want to pay 270 Cps to ran the oven 10 times? ")
                      .AddOption("Yes.", 13)
                      .AddOption("I'm not ready, yet.", 255).FinalizeDialog();


                        break;
                    }
                case 13:
                    {
                        if (client.Inventory.HaveSpace(4))
                        {
                            if (client.Player.ConquerPoints >= 270)
                            {
                                if (client.InTrade) return;
                                client.Player.ConquerPoints -= 270;
                                for (int x = 0; x < 10; x++)
                                {
                                    uint[] array_Items = new uint[]
                       {
                               0,0, 3004466, 3004467, 0, 3006013,0 , 0, 0,0,3004466,0,0, 0,3004467,0,0,0,0,0,3004466,3004466,0,0,0,0,3006013 , 0, 0, 0, 0 ,3004467,3004467,3004466,0,0,0,3004467,3004467
                       };
                                    int index = (int)Pool.GetRandom.Next(0, array_Items.Length);
                                    uint ItemId = array_Items[index];
                                    if (ItemId != 0)
                                    {
                                        var item = Pool.ItemsBase[ItemId];
                                        client.Inventory.AddItemWitchStack((uint)ItemId, 0, 1, stream);
                                        data.AddText("You`re ran the oven " + client.Player.EpicQuestChance + " time(s). Received " + item.Name + ". \n");
                                    }
                                    else
                                        data.AddText("You`re ran the oven " + client.Player.EpicQuestChance + " time(s). Received nothing. \n");
                                    client.Player.EpicQuestChance += 1;
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "angelwing");
                                }
                                data.AddOption("Head to The Unique Pure Oven.", 15).AddOption("Run 10 times more (270 CPs)", 12).AddOption("I see.", 255).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Please come back when you will have 270 ConquerPoints")
     .AddOption("I see", 255).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Please make 4 more spaces in your inventory")
                                                       .AddOption("Let me check.", 255)
                                                       .FinalizeDialog();
                        }
                        break;
                    }
                case 15:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(3933, 64, 64, 25011);
                        break;
                    }
                case 10:
                    {
                        data.AddText("You are sure you want to pay 27 Cps to activate this oven? ")
                        .AddOption("Yes.", 11)
                        .AddOption("I'm not ready, yet.", 255).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.ConquerPoints >= 27)
                            {
                                if (client.InTrade) return;
                                client.Player.ConquerPoints -= 27;
                                uint[] array_Items = new uint[]
                       {
                               0,0, 3004466, 3004467, 0, 3006013,0 , 0, 0,0,3004466,0,0, 3006013,3004467,0,3004466,3004466,0 , 0, 0, 0, 0 ,3004467,3004467,3004466,0,0,0,3004467,3004467
                       };
                                int index = (int)Pool.GetRandom.Next(0, array_Items.Length);
                                uint ItemId = array_Items[index];
                                if (ItemId != 0)
                                {
                                    var item = Pool.ItemsBase[ItemId];
                                    client.Inventory.AddItemWitchStack((uint)ItemId, 0, 1, stream);
                                    client.CreateBoxDialog("You received " + item.Name + ".");
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "angelwing");
                                }
                                else
                                    client.CreateBoxDialog("You received nothing.");
                            }
                            else
                            {
                                data.AddText("Please come back when you will have 27 ConquerPoints")
     .AddOption("Oh.", 255).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Please make 1 more spaces in your inventory")
                                                       .AddOption("Let me check.", 255)
                                                       .FinalizeDialog();
                        }

                        break;
                    }
                case 3:
                    {
                        client.Teleport(612, 634, 1700, 0);
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(711506, 3) && client.Inventory.Contain(711508, 3))
                        {
                            client.Inventory.Remove(711506, 3, stream);
                            client.Inventory.Remove(711508, 3, stream);
                            client.Player.EpicQuestChance = 1;
                            goto case 0;

                        }
                        else
                        {
                            data.AddText("Please come back when you will have 3 Red Flowers and 3 White Flowers in your inventory.")
         .AddOption("Oh.", 255).FinalizeDialog();
                        }

                        break;
                    }
                case 2:
                    {
                        if (client.Player.ConquerPoints >= 27)
                        {
                            if (client.InTrade) return;
                            client.Player.ConquerPoints -= 27;
                            client.Player.EpicQuestChance = 1;
                            goto case 0;
                        }
                        else
                        {
                            data.AddText("Please come back when you will have 27 ConquerPoints")
       .AddOption("Oh.", 255).FinalizeDialog();
                        }


                        break;
                    }
            }
        }


        [NpcAttribute(NpcID.SunKing)]
        public static void SunKing(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Once you acquire the essence of Taoism, you'll be able to make a Divine")
                   .AddText(" Panacea, and win a peerless Epic Weapon of Taoist. All heroes who have")
                           .AddText(" reached Level 70 of 2nd rebirn are welcome to take the trial with the three Pure Ovens.")
                    .AddOption("Show me the Way", 1)
                    .AddOption("I wana to leave.", 2)
                    .AddOption("I'm not ready, yet.", 255)
                    .AddAvatar(6).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        data.AddText("Stage 1: Activate the Refined Pure Oven (Taoist Temple 80,80) with 3 Red Flowers and 3 White Flowers")
                            .AddText(" to make a Treasure Leaf. You can probably obtain such flowers in the Evil Abyss, where you can do the 2nd Rebirth Quest. \n")
                              .AddText("Stage 2: When you obtain a Treasure Leaf, put it into the Elite Pure Oven (Taoist Temple 64,64), and you'll have a 10% chance to obtain a Star Crystal. \n")
                          .AddText("Stage 3: When you successfully get a Star Crystal from the Elite Pure Oven, use it to make the mystical Divine Panacea in the Super Pure Oven (Taoist Temple 48,48) at a 10% success rate. \n\n")
                          .AddText("Once you obtained a Divine Panacea, use it to reshape your right-handed Backsword into an Epic Weapon of Taoist. \n")
                          .AddText("If you failed to obtain the key items in the Elite Pure Oven and Super Pure Oven, instead you'll receive Yin Yang Fish. With Yin Yang Fish, you can exchange ")
                          .AddText(" with the Pure Taoist (Taoist Temple 76,83) for fabulous rewards, such as an Elite Bliss Hossu, which can")
                          .AddText(" be equipped as a secondary weapon at the left hand. The eye-catching Hossu is exclusive for Taoists which perfectly reveals the purity and grace of Taoists.")
                              .AddOption("I see.", 255)
                            .AddAvatar(6).FinalizeDialog();


                        break;
                    }
                case 2:
                    {
                        client.Teleport(53, 36, 1004);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.FortuneArhat)]
        public static void FortuneArhat(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hell is so close to Heavem, if you commit a sin, you could easily on your way to Hell, but ")
                   .AddText("you can always be saved. Give me 3 Red Flowers and 3 White Flowers, and get ready for a trial")
                           .AddText("to clear your sin.")
                    .AddOption("Here`re the flowers.", 1)
                    .AddOption("Pay 197 CPs to start the trial.", 2)
                    .AddOption("Human beling are complicated", 255)
                    .AddAvatar(226).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        data.AddText("Are you sure you want to start this trial with the power of 197 ConquerPoints?")
                   .AddOption("Yes.", 3)
                   .AddOption("No.", 255)
                   .AddAvatar(226).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (client.Player.ConquerPoints >= 197)
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.InTrade) return;
                                client.Player.ConquerPoints -= 197;
                                if (Role.Core.Rate(20))
                                {
                                    client.CreateBoxDialog("You received SpiritualGem.");
                                    client.Inventory.AddItemWitchStack(3007562, 0, 1, stream);
                                }
                                else
                                {
                                    client.CreateBoxDialog("You received RosewoodIncense.");
                                    client.Inventory.AddItemWitchStack(3007565, 0, 1, stream);
                                }
                            }
                            else
                            {
                                data.AddText("Please make 1 more spaces in your inventory")
                                                         .AddOption("Let me check.", 255)
                                                         .AddAvatar(226).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You do not have 97 ConquerPoints with you.")
              .AddOption("Oh.", 255)
              .AddAvatar(226).FinalizeDialog();
                        }


                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(722732, 3) && client.Inventory.Contain(722736, 3))
                        {
                            client.Inventory.Remove(722736, 3, stream);
                            client.Inventory.Remove(722732, 3, stream);
                            if (Role.Core.Rate(30))
                            {
                                client.CreateBoxDialog("You received SpiritualGem.");
                                client.Inventory.AddItemWitchStack(3007562, 0, 1, stream);
                            }
                            else
                            {
                                client.CreateBoxDialog("You received RosewoodIncense.");
                                client.Inventory.AddItemWitchStack(3007565, 0, 1, stream);
                            }
                        }
                        else
                        {
                            data.AddText("Please come back when you will have 3 Red Flowers and 3 White Flowers in your inventory.")
         .AddOption("Oh.", 255)
         .AddAvatar(226).FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.GoldenCicada)]
        public static void GoldenCicada(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Devils are rampant again and people are living in misery. That is the reason why i came ")
                  .AddText("here with my disciples. We need Monks and Saints who have reached Level 70 of 2nd-reborn ")
                          .AddText("to repeat the journey to the Westrn Fairyland. Perhaps, the power of sacred sutras in the")
                          .AddText(" Fairland can alleviate the miseries.")
                   .AddOption("What can i do? ", 1)
                   .AddOption("I wana to leave here.", 2)
                   .AddOption("Enter~in~the~Evil~Abyss.", 5)
                   .AddOption("Nothing is real.", 255)
                   .AddAvatar(226).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("Trial 1: Submit 3 Red Flowers and 3 White Flowers to Fortune Arhat, and complete this trial to get a [Spiritual Gem]. \n")
                   .AddText("Trial 2: Submit [Spiritual Gem] to the Altar Cleanser, and remove distracting thoughts to get [BuddhaÃ¢â‚¬â„¢s Necklace]. \n")
                           .AddText("Trial 3: Submit [BuddhaÃ¢â‚¬â„¢s Necklace] to the Victory Buddha, and complete this trial to get a [Boundless Heart]. \n\n")
                           .AddText("Use The[Boundless Heart] to reshape your Prayer Beads into an Epic Weapon. \n")
                           .AddText("Equipping 2 Epic Weapons on your hands will inflict ranged damage while attacking.")
                    .AddOption("Head To see Fortune Arhat. ", 3)
                    .AddOption("I see.", 255)
                    .AddAvatar(226).FinalizeDialog();



                        break;
                    }
                case 5:
                    {
                        client.Teleport(612, 634, 1700, 0);
                        break;
                    }
                case 3:
                    {
                        client.Teleport(79, 82, 3983);
                        break;
                    }
                case 2:
                    {
                        client.Teleport(37, 36, 1004);
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.TcBoxer)]
        public static void TcBoxer(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("If you are level 20 or above, you may level up at the training grounds.it does ")
                            .AddText(" not consume any durability,life and mana. Do you want me to teleport you there?")
                            .AddText("It is free for my guild members, and 1000 silver for others.")
                    .AddOption("Please teleport me there.", 1)
                    .AddOption("Just passing by.", 255)
                    .AddAvatar(8).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        if (client.Player.Money > 1000)
                        {
                            client.Player.Money -= 1000;
                            client.Teleport(226, 218, 1039);
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            data.AddText("When you gain experience at the training ground, you can only attack the dummies or stakes which ")
                            .AddText(" level is lower than your.")
                    .AddOption("I see.", 255)
                    .AddAvatar(8).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You do not have 1000 silvers with you.")
                                           .AddOption("oh, sorry", 255)
                                           .AddAvatar(8).FinalizeDialog();
                        }


                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.BirdBoxer)]
        public static void BirdBoxer(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("If you are level 20 or above, you may level up at the training grounds.it does ")
                            .AddText(" not consume any durability,life and mana. Do you want me to teleport you there?")
                            .AddText("It is free for my guild members, and 1000 silver for others.")
                    .AddOption("Please teleport me there.", 1)
                    .AddOption("Just passing by.", 255)
                    .AddAvatar(8).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        if (client.Player.Money > 1000)
                        {
                            client.Player.Money -= 1000;
                            client.Teleport(226, 218, 1039);
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            data.AddText("When you gain experience at the training ground, you can only attack the dummies or stakes which ")
                            .AddText(" level is lower than your.")
                    .AddOption("I see.", 255)
                    .AddAvatar(8).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You do not have 1000 silvers with you.")
                                           .AddOption("oh, sorry", 255)
                                           .AddAvatar(8).FinalizeDialog();
                        }


                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.PheonixBoxer)]
        public static void PheonixBoxer(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("If you are level 20 or above, you may level up at the training grounds.it does ")
                            .AddText(" not consume any durability,life and mana. Do you want me to teleport you there?")
                            .AddText("It is free for my guild members, and 1000 silver for others.")
                    .AddOption("Please teleport me there.", 1)
                    .AddOption("Just passing by.", 255)
                    .AddAvatar(8).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        if (client.Player.Money > 1000)
                        {
                            client.Player.Money -= 1000;
                            client.Teleport(226, 218, 1039);
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            data.AddText("When you gain experience at the training ground, you can only attack the dummies or stakes which ")
                            .AddText(" level is lower than your.")
                    .AddOption("I see.", 255)
                    .AddAvatar(8).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You do not have 1000 silvers with you.")
                                           .AddOption("oh, sorry", 255)
                                           .AddAvatar(8).FinalizeDialog();
                        }


                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.ApeBoxer)]
        public static void ApeBoxer(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("If you are level 20 or above, you may level up at the training grounds.it does ")
                            .AddText(" not consume any durability,life and mana. Do you want me to teleport you there?")
                            .AddText("It is free for my guild members, and 1000 silver for others.")
                    .AddOption("Please teleport me there.", 1)
                    .AddOption("Just passing by.", 255)
                    .AddAvatar(8).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        if (client.Player.Money > 1000)
                        {
                            client.Player.Money -= 1000;
                            client.Teleport(226, 218, 1039);
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            data.AddText("When you gain experience at the training ground, you can only attack the dummies or stakes which ")
                            .AddText(" level is lower than your.")
                    .AddOption("I see.", 255)
                    .AddAvatar(8).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You do not have 1000 silvers with you.")
                                           .AddOption("oh, sorry", 255)
                                           .AddAvatar(8).FinalizeDialog();
                        }



                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.DesertBoxer)]
        public static void DesertBoxer(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("If you are level 20 or above, you may level up at the training grounds.it does ")
                            .AddText(" not consume any durability,life and mana. Do you want me to teleport you there?")
                            .AddText("It is free for my guild members, and 1000 silver for others.")
                    .AddOption("Please teleport me there.", 1)
                    .AddOption("Just passing by.", 255)
                    .AddAvatar(8).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Player.Money > 1000)
                        {
                            client.Player.Money -= 1000;
                            client.Teleport(226, 218, 1039);
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            data.AddText("When you gain experience at the training ground, you can only attack the dummies or stakes which ")
                            .AddText(" level is lower than your.")
                    .AddOption("I see.", 255)
                    .AddAvatar(8).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You do not have 1000 silvers with you.")
                                           .AddOption("oh, sorry", 255)
                                           .AddAvatar(8).FinalizeDialog();
                        }


                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.DicCityMap4)]
        public static void DicCityMap4(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello there. Just let me know if you want to go to TwinCity.")
                   .AddOption("TwinCity.", 2)
                   .AddOption("I`m have the DarkHorn.", 20)
                   .AddOption("Let~me~think...", 255)
                   .AddAvatar(63).FinalizeDialog();

                        break;
                    }
                case 20:
                    {
                        if (client.Inventory.Contain(790001, 1))
                        {
                            if (client.Inventory.HaveSpace(8))
                            {
                                MsgTournaments.MsgSchedules.DisCity.RewardDarkHorn(client, stream);
                            }
                            else
                            {
                                data.AddText("Please make 8 more spaces in your inventory")
                                              .AddOption("Let me check.", 255)
                                              .AddAvatar(8).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Sorry you not have DarkHorn.")
                      .AddOption("Ah. ok", 255).AddAvatar(154).FinalizeDialog();
                        }


                        break;
                    }
                case 2:
                    {
                        data.AddText("Sure you want to leave in TwinCity?")
                            .AddOption("Yes.", 3)
                            .AddOption("Let~me~think...", 255)
                            .AddAvatar(63).FinalizeDialog();


                        break;
                    }
                case 3:
                    {
                        client.Teleport(Places.TwinCity);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.DisCityMap2)]
        public static void DisCityMap2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello, can you help me? If you kill the required monsters I desire you can win some EXP. You must kill " + MsgTournaments.MsgSchedules.DisCity.KillsMap2Records(client.Player.Class) + " monsters.")
                                .AddText("You have killed " + client.Player.KillersDisCity + " monsters.")
                                .AddOption("I finished", 1)
                           .AddOption("Sorry, I'll be back", 255).AddAvatar(154).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Player.KillersDisCity >= MsgTournaments.MsgSchedules.DisCity.KillsMap2Records(client.Player.Class))
                        {
                            if (client.Inventory.HaveSpace(2))
                            {
                                client.Inventory.Add(stream, 723087);
                                if (Role.Core.Rate(2, 10))
                                {
                                    if (Role.Core.Rate(1, 2))
                                    {
                                        client.Inventory.Add(stream, 724155, 1);
                                        client.SendSysMesage("You passed the 2nd level in the Dis City quest and got the Medium Veneer Pack.");
                                    }
                                    else
                                    {
                                        client.Inventory.Add(stream, 724161, 1);
                                        client.SendSysMesage("You passed the 2nd level in the Dis City quest and got the Medium Cutter Pack.");
                                    }
                                }
                                else
                                {
                                    switch (Pool.GetRandom.Next(0, 8))
                                    {
                                        case 0:
                                            {
                                                client.Inventory.Add(stream, 724158, 1);
                                                client.SendSysMesage("You passed the 2nd level in the Dis City quest and got the Medium Red Tassel Pack.");
                                                break;
                                            }
                                        case 1:
                                            {
                                                client.Inventory.Add(stream, 724137, 1);
                                                client.SendSysMesage("You passed the 2nd level in the Dis City quest and got the Medium Plate Pack.");
                                                break;
                                            }
                                        case 2:
                                            {
                                                client.Inventory.Add(stream, 724140, 1);
                                                client.SendSysMesage("You passed the 2nd level in the Dis City quest and got the Medium Visor Pack.");
                                                break;
                                            }
                                        case 3:
                                            {
                                                client.Inventory.Add(stream, 724164, 1);
                                                client.SendSysMesage("You passed the 2nd level in the Dis City quest and got the Medium Cauterant Pack.");
                                                break;
                                            }
                                        case 4:
                                            {
                                                client.Inventory.Add(stream, 724143, 1);
                                                client.SendSysMesage("You passed the 2nd level in the Dis City quest and got the Medium Breastplate Pack.");
                                                break;
                                            }
                                        case 5:
                                            {
                                                client.Inventory.Add(stream, 724173, 1);
                                                client.SendSysMesage("You passed the 2nd level in the Dis City quest and got the Medium Tri Plume Pack.");
                                                break;
                                            }
                                        case 6:
                                            {
                                                client.Inventory.Add(stream, 724176, 1);
                                                client.SendSysMesage("You passed the 2nd level in the Dis City quest and got the Medium Belt Pack.");
                                                break;
                                            }
                                        case 7:
                                            {
                                                client.Inventory.Add(stream, 724146, 1);
                                                client.SendSysMesage("You passed the 2nd level in the Dis City quest and got the Medium Hat Rope Pack.");
                                                break;
                                            }
                                    }
                                }
                                MsgTournaments.MsgSchedules.DisCity.TeleportToMap3(client);

                            }
                            else
                            {
                                data.AddText("Please make 2 empty space in your inventory to receive the reward for passing the 2nd level.");
                                data.AddOption("OK.");
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You`ll~pass~if~you~kill~" + MsgTournaments.MsgSchedules.DisCity.KillsMap2Records(client.Player.Class) + "~monsters~in~HellHall.~Hurry~up!~Only~30~persons~can~get~to~HellCloister~with~me.");
                            data.AddOption("I~see.");
                            data.FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.DisCityMap1)]
        public static void DisCityMap1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Inventory.Contain(723085, 5))
                        {
                            data.AddText("Well, now that you`ve got 5 Soul Stones, I`ll give you 2 Exp Ball(B), a Cordiality Book(B), an Exp Pill(B) and a Dis City Bless Stone(12H)(B). Then, send you to the next level. Are you ready?");
                            data.AddOption("I`m~ready.", 2);
                            data.AddOption("Leave~Dis~City.", 1);
                            data.AddOption("Wait~a~moment.");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("I~need~five~SoulStones~in~order~to~help~you.");
                            data.AddOption("I~see.");
                            data.FinalizeDialog();
                        }


                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(723085, 5))
                        {
                            client.Inventory.Remove(723085, 5, stream);

                            client.Inventory.AddItemWitchStack(Database.ItemType.ExpBall2, 0, 2, stream, true);
                            client.Inventory.Add(stream, 728883, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 720393, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 728888, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);

                            MsgTournaments.MsgSchedules.DisCity.TeleportToMap2(client);
                        }
                        else
                        {
                            data.AddText("I~need~five~SoulStones~in~order~to~help~you.");
                            data.AddOption("I~see.");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        client.Teleport(Places.TwinCity);



                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.TwinDisCityMain)]
        public static void TwinDisCityMain(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Our ancestors exerted their utmost efforts and defeathed the demons. Since")
                              .AddText(" then the world has been kept in peace for hundreads of years. Now the demons have come back and the world is getting into turbulence again.")
                              .AddText("The Dis City will take place during 7:30-7:35 and 17:30-17:35 every day, except Sundays.")
                              .AddOption("Could you tell me more?", 1)
                              .AddOption("I am here to drive away the evils!", 10)
                              .AddOption("I`m have the DarkHorn.", 20)
                         .AddOption("I'd better leave.", 255).AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        data.AddText("The decisive battle bewtween human and the demon broke out here. The ferocious battle lasted for World days and nights. Countless heroes lost their")
                           .AddText(" lives. And the justice won. Afterwards we can live in peace for hundreads of years.")
                           .AddOption("And then?", 2).AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        data.AddText("UltimatePluto, leader of the demons sworn to take revenge in one thousand  years befote he managed to run away. To prevent the demons coming back, I")
                            .AddText(" have been scouting their land cautiously. Yesterday I found something  unwanted. It seems that UltimatePluto had come round to endanger humans again.")
                            .AddOption("What should we do?", 3).AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        data.AddText("Fortunately UltimatePluto is still unfledged. He must resort to a Battle Formation for the moments. So i am about to organize an army too infiltrate his")
                            .AddText(" land and destroy his formation before he becomes stronger.")
                            .AddOption("I'd like to know more.", 4).AddOption("Sigh, I'm helpless.", 255).AddAvatar(67).FinalizeDialog();


                        break;
                    }
                case 4:
                    {
                        data.AddText("You must kill UltimatePluto rapisly to get Darkhorn so that i can use it to diable the formation. Before you can do that, you must break into HellGate,")
                           .AddText(" enter the HellHall, and fight through the HellCloister. Countless ferocious  demons are watching those strongholds. I'll give you some strategies on")
                           .AddText(" breaking through those fortresses if you like.")
                           .AddOption("I want to know HellGate.", 6)
                           .AddOption("I want to know HellHall.", 7)
                           .AddOption("I want to know HellCloister.", 8)
                           .AddOption("I want to know BattleFormation.", 9)
                           .AddOption("Thanks. I know what to do.", 255)
                           .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        data.AddText("HellGate is shielded from poisonous fog, so you can't approach it. But demons do not fear the gas. They may turn into SoulStones after they die. If")
                            .AddText(" you get 5 stones for me, i'll help you break through the gate. To protect  the unrealted persons, i'll send the others back as soon as the first 60")
                            .AddText(" persons pass through the gate.")
                            .AddOption("Thank you.", 255)
                            .AddOption("I'd like to know more.", 4).AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 7:
                    {
                        data.AddText("HellHall is the very spot where the demons swear their oaths of allegiance to UltimatePluto. Everybody must do his best to make a way out. Due to limited")
                            .AddText(" time, i can lead only 30 persons to hellCloister!")
                            .AddOption("Thank you.", 255)
                            .AddOption("I'd like to know more.", 4).AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 8:
                    {
                        data.AddText("You will be divided into 2 groups to attack from the left and the right flank of HellCloister. Kill the Wraithas many as you can, because you can't reach")
                            .AddText(" BattleFormation untill the amount of wraith is decreased to a certain level.")
                            .AddOption("Thank you.", 255)
                            .AddOption("I'd like to know more.", 4).AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 9:
                    {
                        data.AddText("BattleFormation is protected by Syrens. After they are killed out, UltimatePluto will appear. Make the best effort to kill him, get his Darkhorn")
                            .AddText(" and give it to me. I can disable the BattleFormation with it. Then i'll send you back. But if you fail to do it, we have to retreat and wait for another opportunity")
                            .AddOption("Thank you.", 255)
                            .AddOption("I'd like to know more.", 4).AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 20:
                    {
                        if (client.Inventory.Contain(790001, 1))
                        {
                            if (client.Inventory.HaveSpace(8))
                            {
                                MsgTournaments.MsgSchedules.DisCity.RewardDarkHorn(client, stream);
                            }
                            else
                            {
                                data.AddText("Please make 8 more spaces in your inventory")
                                              .AddOption("Let me check.", 255)
                                              .AddAvatar(67).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Sorry you not have DarkHorn.")
                      .AddOption("Ah. ok", 255).AddAvatar(67).FinalizeDialog();
                        }
                        break;
                    }
                case 11:
                    {
                        client.SendSysMesage("As~long~as~you~reach~level110,~you~can~enter~the~Dis~City.");
                        break;
                    }
                case 10:
                    {
                        if (MsgTournaments.MsgSchedules.DisCity.AllowJoin())
                        {
                            if (client.Player.Level >= 110)
                            {
                                if (client.Inventory.HaveSpace(1))
                                {
                                    data.AddText("Kill~the~demons~to~gather~five~SoulStones.~Then~go~northeast~through~HellBridge,~and~look~for~me~at~HellGate.");
                                    data.AddOption("I~see.");
                                    data.AddAvatar(67);
                                    data.FinalizeDialog();



                                    MsgTournaments.MsgSchedules.DisCity.TeleportMap1(stream, client);
                                }
                                else
                                    client.CreateBoxDialog("Please make 1 more space in your inventory.");

                            }
                            else
                            {
                                data.AddText("You`re~not~strong~enough~for~the~moment.~Don`t~lose~your~life~in~vain.");
                                data.AddOption("I~see.", 11);
                                data.AddAvatar(67);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Please sign up between 7:30-7:35 and 17:30-17:35 every day, except Sundays.")
 .AddOption("I see. ", 255)
 .AddAvatar(67).FinalizeDialog();
                        }
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.DisCityMain)]
        public static void DisCityMain(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.StoneApe, client.Player.Class, 1312);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DisCityMain, client.Player.Class, 1313);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("I have been questing the meaning of life for decades.")
                              .AddOption("Quest:~The~Stone~Ape(1).", 40)
                              .AddOption("Have~you~found~anything?", 41)
                              .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("I have been questing the meaning of life for decades.")
                            .AddOption("Have~you~found~anything?", 41)
                            .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        data.AddText("I have been questing the meaning of life for decades.");
                        data.AddOption("Have~you~found~anything?");
                        data.AddAvatar(67);
                        data.FinalizeDialog();




                        break;
                    }
                case 40:
                    {
                        data.AddText("The Stone Ape was born from the rock used to restore cracks in the sky. He never know about it. He gets potentials. If guided properly, he can be a great immortal!")
                               .AddOption("Thank~you~for~the~information!", 42)
                               .AddOption("See~you~then!", 255)
                               .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 42:
                    {
                        data.AddText("I have a Divine Calix. Get some water from the fountain in the Maple Forest. It will help the Stone Ape to unleash his potential!")
                              .AddOption("Okay!", 43)
                              .AddOption("See~you~then!", 255)
                              .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 43:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Add(stream, 711354);
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                            {
                                client.SendSysMesage("You received 238,000 points of EXP, 1,500 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                                client.IncreaseExperience(stream, 238000, Role.Flags.ExperienceEffect.angelwing, true);
                                client.Player.Money += 500;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            }
                            client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        }
                        else
                        {
                            data.AddText("Please make 1 more space in your inventory")
                                          .AddOption("Let me check.", 255)
                                          .AddAvatar(67).FinalizeDialog();
                        }


                        break;
                    }
                case 41:
                    {
                        data.AddText("Our ancestors exerted their utmost efforts and defeathed the demons. Since")
                         .AddText(" then the world has been kept in peace for hundreads of years. Now the demons have come back and the world is getting into turbulence again.")
                         .AddText("The Dis City will take place during 7:30-7:35 and 17:30-17:35 every day, except Sundays.")
                         .AddOption("Could you tell me more?", 1)
                         .AddOption("I am here to drive away the evils!", 10)
                         .AddOption("I`m have the DarkHorn.", 20)
                    .AddOption("I'd better leave.", 255).AddAvatar(67).FinalizeDialog();


                        break;
                    }

            }
        }
        //[NpcAttribute(NpcID.PkWarQuit)]
        //public static void PkWarQuit(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog data = new Dialog(client, stream);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                data.AddText("Hello , you're the last player alive?")
        //                    .AddOption("Yes", 1)
        //                    .AddOption("Teleport~to~Twin~City.", 2)
        //               .AddOption("Ah. wait", 255).AddAvatar(154).FinalizeDialog();


        //                break;
        //            }
        //        case 1:
        //            {
        //                if (MsgTournaments.MsgSchedules.PkWar.IsFinished())
        //                {
        //                    if (MsgTournaments.MsgSchedules.PkWar.TheLastPlayer())
        //                    {
        //                        if (client.Inventory.HaveSpace(2))
        //                        {
        //                            MsgTournaments.MsgSchedules.PkWar.GiveReward(client, stream);
        //                        }
        //                        else
        //                        {
        //                            data.AddText("Please make 2 more spaces in your inventory")
        //                                       .AddOption("Let me check.", 255)
        //                                       .AddAvatar(8).FinalizeDialog();
        //                        }
        //                    }
        //                    else
        //                    {
        //                        data.AddText("Sorry, the war is not finished, or other players are still alive.")
        //       .AddOption("Ah ok", 255).AddAvatar(154).FinalizeDialog();
        //                    }
        //                }
        //                else
        //                {
        //                    data.AddText("Sorry, the war is not finished")
        //        .AddOption("Ah ok", 255).AddAvatar(154).FinalizeDialog();
        //                }




        //                break;

        //            }
        //        case 2:
        //            {
        //                data.AddText("You wana to teleport back to TwinCity? You are sure?")
        //   .AddOption("Yes", 3).AddOption("Ah no.", 255).AddAvatar(154).FinalizeDialog();

        //                break;
        //            }
        //        case 3:
        //            {
        //                client.Teleport(409, 353, 1002, 0);
        //                break;
        //            }
        //    }
        //}
        //[NpcAttribute(NpcID.PkWar)]
        //public static void PkWar(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog data = new Dialog(client, stream);

        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                data.AddText("Welcome, you will be accepted into the weekly war from 21:00 until 21:30.")
        //                                  .AddOption("I want to join", 1)
        //                                  .AddOption("No thank you ", 255)
        //                                  .AddAvatar(63).FinalizeDialog();


        //                break;
        //            }
        //        case 1:
        //            {
        //                if (Game.MsgTournaments.MsgSchedules.PkWar.AllowJoin())
        //                {
        //                    client.Teleport(87, 88, 1508);
        //                }
        //                else
        //                {
        //                    data.AddText("Please sign up between 20:00 and 20:20 every Saturday.")
        //    .AddOption("I see. ", 255)
        //    .AddAvatar(63).FinalizeDialog();
        //                }


        //                break;
        //            }
        //    }
        //}//TeamDeathMatch


        [NpcAttribute(NpcID.SteedRace)]
        public static void SteedRace(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hey there " + client.Player.Name + "! The race is held every two hours, on even hours(11,15,23,..). Are you interested in joining the race? You might receive amazing gifts upon reaching the finish line!")

                   .AddOption("Sign~me~up", 1)
                   .AddOption("Open the shop.", 2)
                   .AddOption("I will come later. ", 255)
                   .AddAvatar(63).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        if (client.Equipment.FreeEquip(Role.Flags.ConquerItem.Steed))
                        {
                            data.AddText("You need a horse first.")
             .AddOption("I see. ", 255)
             .AddAvatar(63).FinalizeDialog();
                            break;
                        }
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Riding))
                        {
                            data.AddText("You need to learn the ride spell first!")
             .AddOption("I see. ", 255)
             .AddAvatar(63).FinalizeDialog();
                            break;
                        }
                        if (Game.MsgTournaments.MsgSchedules.SteedRace.CanJoin)
                        {
                            Game.MsgTournaments.MsgSchedules.SteedRace.Join(client, stream);
                        }
                        else
                        {
                            data.AddText("Please sign up at 11:00 AM and 15:00 and 23:00 every day.")
             .AddOption("I see. ", 255)
             .AddAvatar(63).FinalizeDialog();
                        }


                        break;
                    }
                case 2:
                    {
                        //464 
                        ActionQuery action = new ActionQuery()
                        {
                            Type = ActionType.OpenDialog,
                            ObjId = client.Player.UID,
                            dwParam = 464,
                            PositionX = client.Player.X,
                            PositionY = client.Player.Y
                        };
                        client.Send(stream.ActionCreate(action));
                        break;
                    }
            }
        }


        //[NpcAttribute(NpcID.ExtremePk)] // [NpcOfficial 2026-10-14] custom (non-official) NPC removed from the data; handler disabled
        public static void ExtremePk(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Glad to see you! I take it that you haven`t heard of the ExtremePk tournament? please allow me to introduce this exciting event for you.")
                        .AddText("It`s a Pk Tournament that takes place every day. The player who make the best score,he has won the tournament.")
                        .AddText("When you kill others players, you receive points.")
                   .AddOption("Sign~me~up", 1)
                   .AddOption("I will come later. ", 255)
                   .AddAvatar(63).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.ExtremePk)
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
        [NpcAttribute(NpcID.DBShower)]
        public static void DBShower(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Glad to see you! I take it that you haven`t heard of the DBShower tournament? please allow me to introduce this exciting event for you.")
                        .AddText("It`s a new Tournament that takes place every day at 8:00 AM and 13:30. You can not use pk mode there, You play just to collect the dragonballs.")
                   .AddOption("Sign~me~up", 1)
                   .AddOption("I will come later. ", 255)
                   .AddAvatar(63).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.DBShower)
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
        [NpcAttribute(NpcID.GodlyArtisan)]
        public static void GodlyArtisan(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hey, I`m proficient in repairing max Durability. What items do you wana me to repair?")
                   .AddOption("Repair the Talisman", 20)
                   .AddOption("Hat,Earring", 1)
                   .AddOption("Necklace", 2)
                   .AddOption("Coat", 3)
                   .AddOption("Weapon", 4)
                   .AddOption("Ring/Heavy Ring/Bracelet", 6)
                   .AddOption("Boots", 8)
                   .AddOption("Riding Crop", 18)
                   .AddOption("I will come later. ", 255)
                   .AddAvatar(63).FinalizeDialog();


                        break;
                    }
                case 20:
                    {
                        data.AddText("What Talisman do you want me to repair?")
                  .AddOption("Attack Talisman", 11)
                  .AddOption("Defense Talisman", 12)
                  .AddOption("I will come later. ", 255)
                  .AddAvatar(63).FinalizeDialog();


                        break;
                    }
                default:
                    {
                        MsgServer.MsgGameItem GameItem;
                        if (client.Equipment.TryGetEquip((Role.Flags.ConquerItem)Option, out GameItem))
                        {
                            if (GameItem.Durability < GameItem.MaximDurability)
                            {
                                data.AddText("Sorry, please ask a shopkeeper to repair this item first. I only repair the max durability.")
         .AddOption("I see. ", 255)
         .AddAvatar(63).FinalizeDialog();
                                break;
                            }

                            Database.ItemType.DBItem DBItem;
                            if (Pool.ItemsBase.TryGetValue(GameItem.ITEM_ID, out DBItem))
                            {
                                if (GameItem.MaximDurability == DBItem.Durability)
                                {
                                    data.AddText("The max durability of this item is in good condition")
           .AddOption("I see. ", 255)
           .AddAvatar(63).FinalizeDialog();
                                    break;
                                }

                                GameItem.Durability = GameItem.MaximDurability = DBItem.Durability;
                                GameItem.Mode = Role.Flags.ItemMode.Update;
                                GameItem.Send(client, stream);

                                data.AddText("You durability was repaired successfully.")
       .AddOption("Thank you.", 255)
       .AddAvatar(63).FinalizeDialog();
                            }

                        }
                        else
                        {
                            data.AddText("Sorry,you do not wear this item. Please equip first.")
            .AddOption("I see. ", 255)
            .AddAvatar(63).FinalizeDialog();
                        }


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.EquipmentBlacksmith)]
        public static void EquipmentBlacksmith(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Greetings! If you want to enchance you equipment, you came to the right place!")
                           .AddText("I`m the expert at this type of work. I can help you upgrade you equipment level,")
                           .AddText("Improve the quality, or make repairs! Now, tell me what you wana me to do.")

                   .AddOption("Lower Equipment Level.", 1)
                   .AddOption("I will come later. ", 255)
                   .AddAvatar(63).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        ActionQuery action = new ActionQuery()
                        {
                            Type = ActionType.OpenDialog,
                            ObjId = client.Player.UID,
                            dwParam = MsgServer.DialogCommands.DegradeEquip,
                            PositionX = client.Player.X,
                            PositionY = client.Player.Y
                        };
                        client.Send(stream.ActionCreate(action));
                        break;
                    }

            }
        }

        [NpcAttribute(NpcID.OpenAstredgeSystem)]
        public static void OpenAstredgeSystem(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.Reborn < 2 || client.Player.Level < 120)
                        {
                            data.AddText("You must be Second Reborn and at least Level 120 to unlock the Astredge System.")
                                .AddOption("Alright.", 255)
                                .AddAvatar(63).FinalizeDialog();
                            return;
                        }
                        data.AddText("Hello, You can unlock the Astredge System here for free. Would you like to unlock it now? \n");
                        if (client.Player.Reborn == 2 && client.Player.Level >= 120 &&
                            !client.MyAstredge.Items.ContainsKey(Astredge.TypeID.ViodragonClub))
                        {
                            data.AddOption("ViodragonClub. (Unlocking Astredge)", 2);

                        }

                        if (client.Player.Reborn == 2 && client.Player.Level >= 120
                            && !client.MyAstredge.Items.ContainsKey(Astredge.TypeID.LoveForEver))
                        {
                            data.AddOption("Love For ever. (Unlocking Astredge)", 3);

                        }

                        if (client.Player.Reborn == 2 && client.Player.Level >= 120
                            && !client.MyAstredge.Items.ContainsKey(Astredge.TypeID.WarlordSpear))
                        {
                            data.AddOption("WarlordSpear. (Unlocking Astredge)", 20);

                        }

                        if (client.Player.Reborn == 2 && client.Player.Level >= 120
                            && !client.MyAstredge.Items.ContainsKey(Astredge.TypeID.HeartLock))
                        {
                            data.AddOption("HeartLock. (Unlocking Astredge)", 9);

                        }
                        data.AddOption("I~will~come~later.", 255).AddAvatar(63).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        if (!client.MyAstredge.Items.ContainsKey(Astredge.TypeID.ViodragonClub))
                        {
                            client.MyAstredge.AddItem(Astredge.TypeID.ViodragonClub, 1);
                        }
                        break;
                    }
                case 3:
                    {
                        if (!client.MyAstredge.Items.ContainsKey(Astredge.TypeID.LoveForEver))
                        {
                            client.MyAstredge.AddItem(Astredge.TypeID.LoveForEver, 1);
                        }
                        break;
                    }
                case 9:
                    {
                        if (!client.MyAstredge.Items.ContainsKey(Astredge.TypeID.HeartLock))
                        {
                            client.MyAstredge.AddItem(Astredge.TypeID.HeartLock, 1);
                        }
                        break;
                    }
                case 20:
                    {
                        if (!client.MyAstredge.Items.ContainsKey(Astredge.TypeID.WarlordSpear))
                        {
                            client.MyAstredge.AddItem(Astredge.TypeID.WarlordSpear, 1);
                        }
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.GWJail)]
        public static void GWJail(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            DateTime Timer = DateTime.Now;
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Welcome~in~NEWGUILDWAR,~GuildArea~jail.~I~am~giving~jail~pardons~each~five~minutes.")
                    .AddOption("huuuh??", 3)
                    .AddOption("Lets~rock! ", 255)
                    .AddAvatar(63).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (Timer > client.JailStamp)
                        {
                            data.AddText("It~is~time~for~NEWGUILDWAR~pardon~I~will~let~you~get~out~of~here.~Enjoy.")
                                .AddOption("Cool~thx.", 3)
                                .AddOption("I~wanna~stay.", 255)
                                .AddAvatar(63).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Rest~for~a~while~get~a~cup~of~tea~and~sit.You exit to 1 min.")
                                                       .AddOption("Okeyz", 255)
                                                       .AddAvatar(63).FinalizeDialog();
                        }


                        break;
                    }
                case 3:
                    {
                        client.Teleport(Places.TwinCity, 0, true, true);
                        break;
                    }
            }
        }


        [NpcAttribute(NpcID.SellItems)]
        public static void SellItems(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hey how I can help you ? I`m Sell MoonBox for 200 ConquerPoints, Emerald for 100 ConquerPoints, EuxeniteOre for 20 ConquerPoints.")
                    .AddOption("MoonBox", 1)
                    .AddOption("Emerald", 2)
                    .AddOption("EuxeniteOre", 3)
                    .AddOption("Let~me~think.", 255)
                    .AddAvatar(63).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.ConquerPoints >= 200)
                            {
                                if (client.InTrade) return;
                                client.Player.ConquerPoints -= 200;

                                client.Inventory.Add(stream, Database.ItemType.MoonBox);
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
                case 2:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.ConquerPoints >= 100)
                            {
                                if (client.InTrade) return;
                                client.Player.ConquerPoints -= 100;

                                client.Inventory.Add(stream, 1080001);
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
                case 3:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.ConquerPoints >= 20)
                            {
                                if (client.InTrade) return;
                                client.Player.ConquerPoints -= 20;

                                client.Inventory.Add(stream, 1072031);
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
            }
        }
        [NpcAttribute(NpcID.WorldPoints)]
        public static void WorldPoints(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.Text("Hello There, I Can Exchange World Points For Cool Things.\n");
                        dialog.Text("You Can Obtain It From Event&Bosses.\n");
                        dialog.Text("Wanna Proceed? Here Are My Options.\n");
                        dialog.Text("You current points: " + client.Player.WorldPoints + ".\n");
                        dialog.Option("x1" + Pool.ItemsBase[3324226].Name + " [15 WorldPoints].", 1);
                        dialog.Option("Random MythSoul LV4 [50 WorldPoints].", 2);
                        dialog.Option("Random Items Collection [200 WorldPoints].", 3);
                        dialog.Option("x1 " + Pool.ItemsBase[3310699].Name + " [3 WorldPoints].", 4);
                        dialog.Option("x1 " + Pool.ItemsBase[3310698].Name + " [2 WorldPoints].", 5);
                        dialog.Option("x1 " + Pool.ItemsBase[3330856].Name + " [100 WorldPoints].", 6);
                        dialog.Option("x1 RareYellowRunePack [100 WorldPoints].", 7);
                        dialog.Option("I~see.", 255).FinalizeDialog();
                        break;
                    }
                case 7:
                    {
                        if (client.Player.WorldPoints >= 100)
                        {
                            client.Player.WorldPoints -= 100;
                            client.Inventory.AddItemWitchStack(3316736, 0, 1, stream);

                            dialog.Text("Congratulations! You've got x1 RareYellowRunePack !");

                            dialog.Option("Thanks.", 255).FinalizeDialog();
                        }
                        else
                        {
                            dialog.Text("Hey hey! you don't have the required amount of Points!");

                            dialog.Option("I~see.", 255).FinalizeDialog();
                        }
                        break;
                    }
                case 6:
                    {
                        if (client.Player.WorldPoints >= 100)
                        {
                            client.Player.WorldPoints -= 100;
                            client.Inventory.AddItemWitchStack(3330856, 0, 1, stream);

                            dialog.Text("Congratulations! You've got x1 " + Pool.ItemsBase[3330856].Name + " !");

                            dialog.Option("Thanks.", 255).FinalizeDialog();
                        }
                        else
                        {
                            dialog.Text("Hey hey! you don't have the required amount of Points!");

                            dialog.Option("I~see.", 255).FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        if (client.Player.WorldPoints >= 15)
                        {
                            client.Player.WorldPoints -= 15;
                            client.Inventory.AddItemWitchStack(3324226, 0, 1, stream);

                            dialog.Text("Congratulations! You've got x1 " + Pool.ItemsBase[3324226].Name + " !");

                            dialog.Option("Thanks.", 255).FinalizeDialog();
                        }
                        else
                        {
                            dialog.Text("Hey hey! you don't have the required amount of Points!");

                            dialog.Option("I~see.", 255).FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Player.WorldPoints >= 50)
                        {
                            client.Player.WorldPoints -= 50;
                            var Myth = Database.MythTable.RandomMythSoul.Values.Where(p => p.ItemType % 10 == 4).ToArray();
                            int MythSoul = Pool.GetRandom.Next(0, Myth.Length);
                            client.Inventory.AddItemWitchStack(Myth[MythSoul].ItemType, 0, 1, stream);

                            dialog.Text("Congratulations! You've got One " + Pool.ItemsBase[Myth[MythSoul].ItemType].Name + " !");

                            dialog.Option("Thanks.", 255).FinalizeDialog();
                        }
                        else
                        {
                            dialog.Text("Hey hey! you don't have the required amount of Points!");

                            dialog.Option("I~see.", 255).FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Player.WorldPoints >= 200)
                        {
                            client.Player.WorldPoints -= 200;
                            var Collection = Pool.ItemsBase.Values.Where(p => p.ID / 10000 == 430 || p.ID / 10000 == 431 || p.ID / 10000 == 432 || p.ID / 10000 == 433).ToArray();
                            int CollectionS = Pool.GetRandom.Next(0, Collection.Length);
                            client.Inventory.AddItemWitchStack(Collection[CollectionS].ID, 0, 1, stream);

                            dialog.Text("Congratulations! You've got One " + Pool.ItemsBase[Collection[CollectionS].ID].Name + " !");

                            dialog.Option("Thanks.", 255).FinalizeDialog();
                        }
                        else
                        {
                            dialog.Text("Hey hey! you don't have the required amount of Points!");

                            dialog.Option("I~see.", 255).FinalizeDialog();
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Player.WorldPoints >= 3)
                        {
                            client.Player.WorldPoints -= 3;
                            client.Inventory.AddItemWitchStack(3310699, 0, 1, stream);

                            dialog.Text("Congratulations! You've got One " + Pool.ItemsBase[3310698].Name + " !");

                            dialog.Option("Thanks.", 255).FinalizeDialog();
                        }
                        else
                        {
                            dialog.Text("Hey hey! you don't have the required amount of Points!");

                            dialog.Option("I~see.", 255).FinalizeDialog();
                        }
                        break;
                    }
                case 5:
                    {
                        if (client.Player.WorldPoints >= 2)
                        {
                            client.Player.WorldPoints -= 2;
                            client.Inventory.AddItemWitchStack(3310698, 0, 1, stream);

                            dialog.Text("Congratulations! You've got One " + Pool.ItemsBase[3310697].Name + " !");

                            dialog.Option("Thanks.", 255).FinalizeDialog();
                        }
                        else
                        {
                            dialog.Text("Hey hey! you don't have the required amount of Points!");

                            dialog.Option("I~see.", 255).FinalizeDialog();
                        }
                        break;
                    }
            }
        }

        //[NpcAttribute(NpcID.SellRunes)]
        //public static void SellRunes(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog dialog = new Dialog(client, stream);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                dialog.Text("Hello there, i can exchange Rare Yellows Rune for ConquerPoints.\n");
        //                dialog.Option("Ninja Susanoo", 1);
        //                dialog.Option("I~see.", 255).FinalizeDialog();
        //                break;
        //            }
        //        case 1:
        //            {
        //                if (client.Inventory.Contain(4200018, 1))
        //                {
        //                    client.Inventory.Remove(4200018, 1, stream);
        //                    if (client.MyNinja.Score >= 40000)
        //                    {
        //                        if (client.MyNinja.SusanooLevel == 0)
        //                        {
        //                            client.MyNinja.SusanooLevel = 1;
        //                            MsgGouYuAptitude.MsgGouYuAptitudeProto.Create(client, client.MyNinja, client.Player.UID);
        //                        }

        //                    }

        //                    dialog.Text("Congratulations! You've Ninja Susanoo!");

        //                    dialog.Option("Thanks.", 255).FinalizeDialog();
        //                }
        //                else
        //                {
        //                    dialog.Text("Hey hey! you don't have the required amount of Points!");

        //                    dialog.Option("I~see.", 255).FinalizeDialog();
        //                }
        //                break;
        //            }
        //        case 2:
        //            {
        //                if (client.Inventory.Contain(4200018, 1))
        //                {
        //                    client.Inventory.Remove(4200018, 1, stream);
        //                    var obj = new Qahr.Mobile.Game.MsgServer.MsgSwordAncestor();
        //                    obj.Type = 1;
        //                    obj.Level = client.HundredWeapons.SwordAncestorLevel;
        //                    obj.Exp = client.HundredWeapons.SwordAncestorExp;
        //                    obj.UID = client.Player.UID;
        //                    obj.Unlocked = true;
        //                    client.Send(obj);

        //                    dialog.Text("Congratulations! You've Power Focus!");

        //                    dialog.Option("Thanks.", 255).FinalizeDialog();
        //                }
        //                else
        //                {
        //                    dialog.Text("Hey hey! you don't have the required amount of Points!");

        //                    dialog.Option("I~see.", 255).FinalizeDialog();
        //                }
        //                break;
        //            }
        //        case 3:
        //            {
        //                if (client.Inventory.Contain(4200018, 1) && client.MyArchives.NewTaoEquip2 == 0)
        //                {
        //                    client.Inventory.Remove(4200018, 1, stream);

        //                    if (client.MyArchives.NewTaoEquip2 == 0)
        //                    {
        //                        client.MyArchives.NewTaoEquip2 = 13;
        //                    }

        //                    dialog.Text("Congratulations! You've NewTao!");

        //                    dialog.Option("Thanks.", 255).FinalizeDialog();
        //                }
        //                else
        //                {
        //                    dialog.Text("Hey hey! you don't have the required amount of Points!");

        //                    dialog.Option("I~see.", 255).FinalizeDialog();
        //                }
        //                break;
        //            }
        //        case 4:
        //            {
        //                if (client.Inventory.Contain(4200018, 1))
        //                {
        //                    client.Inventory.Remove(4200018, 1, stream);

        //                    client.MyArchives.NpcCombatHeart = 1;
        //                    client.MyArchives.LoadingCombatHeart();
        //                    dialog.Text("Congratulations! You've Ninja Susanoo!");

        //                    dialog.Option("Thanks.", 255).FinalizeDialog();
        //                }
        //                else
        //                {
        //                    dialog.Text("Hey hey! you don't have the required amount of Points!");

        //                    dialog.Option("I~see.", 255).FinalizeDialog();
        //                }
        //                break;
        //            }
        //    }
        //}
        //[NpcAttribute(NpcID.AllNewArchives)] // [NpcOfficial 2026-10-14] custom (non-official) NPC removed from the data; handler disabled
        public static void AllNewArchives(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.Text("Hello, I Can Help You to Open All New Archives For Free.\n");
                        //  dialog.Option("Open Ninja Susanoo.", 1);
                        dialog.Option("Open Power Focus.", 2);
                        //dialog.Option("Open New Tao Gate.", 3);
                        //dialog.Option("Open New Archive Warrior.", 4);
                        dialog.Option("I~will~come~later.", 255).FinalizeDialog();
                        break;
                    }
                //case 1:
                //    {
                //        if (!(client.Player.Class >= 5000 && client.Player.Class <= 5058))
                //        {
                //            dialog.AddText("Sorry, but you are not a Ninja.").AddOption("Alright.", 255).FinalizeDialog();
                //            return;
                //        }
                //        if (Database.AtributesStatus.IsNinja(client.Player.Class) && client.MyNinja.SusanooLevel == 0 && client.MyNinja.Score >= 40000)
                //        {

                //            client.MyNinja.SusanooLevel = 1;
                //            MsgGouYuAptitude.MsgGouYuAptitudeProto.Create(client, client.MyNinja, client.Player.UID);
                //            dialog.Text("Congratulations! You've Ninja Susanoo!");

                //            dialog.Option("Thanks.", 255).FinalizeDialog();
                //        }
                //        else
                //        {
                //            dialog.Text("Hey hey! you don't have the required amount of Points!");

                //            dialog.Option("I~see.", 255).FinalizeDialog();
                //        }
                //        break;
                //    }
                case 2:
                    {
                        if (!(client.Player.Class >= 1000 && client.Player.Class <= 1058))
                        {
                            dialog.AddText("Sorry, but you are not a Trojan.").AddOption("Alright.", 255).AddAvatar(203).FinalizeDialog();
                            return;
                        }
                        if (Database.AtributesStatus.IsTrojan(client.Player.Class) && client.HundredWeapons.SwordAncestorUnlocked == 0)
                        {

                            var obj = new Qahr.Mobile.Game.MsgServer.MsgSwordAncestor();
                            obj.Type = 1;
                            obj.Level = client.HundredWeapons.SwordAncestorLevel;
                            obj.Exp = client.HundredWeapons.SwordAncestorExp;
                            obj.UID = client.Player.UID;
                            obj.Unlocked = true;
                            client.Send(obj);
                            client.HundredWeapons.SwordAncestorUnlocked = 1;
                            dialog.Text("Congratulations! You've Power Focus!");

                            dialog.Option("Thanks.", 255).FinalizeDialog();
                        }
                        else
                        {
                            dialog.Text("Hey hey! you don't have the required amount of Points!");

                            dialog.Option("I~see.", 255).FinalizeDialog();
                        }
                        break;
                    }
                //case 3:
                //    {
                //        if (!(client.Player.Class >= 13000 && client.Player.Class <= 14058 || client.Player.Class == 10000 || client.Player.Class == 10001))
                //        {
                //            dialog.AddText("Sorry, but you are not a Taoist.").AddOption("Alright.", 255).FinalizeDialog();
                //            return;
                //        }
                //        if (Database.AtributesStatus.IsTaoist(client.Player.Class) && client.MyArchives.NewTaoEquip2 == 0)
                //        {


                //            if (client.MyArchives.NewTaoEquip2 == 0)
                //            {
                //                client.MyArchives.NewTaoEquip2 = 13;
                //            }

                //            dialog.Text("Congratulations! You've NewTao!");

                //            dialog.Option("Thanks.", 255).FinalizeDialog();
                //        }
                //        else
                //        {
                //            dialog.Text("Hey hey! you don't have the required amount of Points!");

                //            dialog.Option("I~see.", 255).FinalizeDialog();
                //        }

                //        break;
                //    }
                //case 4:
                //    {
                //        if (!(client.Player.Class >= 2000 && client.Player.Class <= 2058))
                //        {
                //            dialog.AddText("Sorry, but you are not a Warrior.").AddOption("Alright.", 255).FinalizeDialog();
                //            return;
                //        }
                //        if (Database.AtributesStatus.IsWarrior(client.Player.Class)
                //            &&
                //            client.MyArchives.NpcCombatHeart == 0)
                //        {


                //            client.MyArchives.NpcCombatHeart = 1;
                //            client.MyArchives.LoadingCombatHeart();
                //            dialog.Text("Congratulations! You've Combat Heart!");

                //            dialog.Option("Thanks.", 255).FinalizeDialog();
                //        }
                //        else
                //        {
                //            dialog.Text("Hey hey! you don't have the required amount of Points!");

                //            dialog.Option("I~see.", 255).FinalizeDialog();
                //        }

                //        break;
                //    }
            }
        }

        [NpcAttribute(NpcID.OnlinePoints)]
        public static void OnlinePoints(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;

                        dialog.AddText("You Have [ " + client.Player.OnlineMinutes + " ] Online Points \n");
                        dialog.AddOption("Vip 6 Token (2 Hours) [500 Pts] ", 1);
                        dialog.AddOption("Vip 6 Token (1 Day) [2000 Pts] ", 2);
                        dialog.Option("(1) Garment 1-3 Stars [150 Pts] ", 3);
                        dialog.Option("(1) Mount 1-3 Stars [150 Pts] ", 4);
                        dialog.Option("(1) Garment 4-5 Stars [300 Pts] ", 5);
                        dialog.Option("(1) Mount 4-5 Stars [300 Pts] ", 6);
                        dialog.Option("(1) Random Accessory [100 Pts] ", 12);
                        dialog.Option("(1) Random Relic [150 Pts] ", 7);
                        dialog.Option("(1) RedBodhiBean [100 Pts] ", 8);
                        dialog.Option("(10) RedBodhiBean [1000 Pts] ", 9);
                        dialog.Option("(1) Cosmos Hammer [100 Pts] ", 10);
                        dialog.Option("(10) Cosmos Hammer [1000 Pts] ", 11);
                        dialog.AddOption("I~will~come~later.", 255).AddAvatar(3497).FinalizeDialog();
                        break;
                    }

                case 1:
                    {

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;

                        if (client.Player.OnlineMinutes >= 500)
                        {
                            client.Player.OnlineMinutes -= 500;

                            client.Inventory.AddItemWitchStack(3004120, 0, 1, stream, false); // VipToken(2Hours)                           

                            MsgSchedules.SendSysMesage(" [ " + client.Player.Name + " ] Got VIP 6 Token (2 Hours) for 500 Online Points ", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                        }
                        else
                        {

                            dialog.AddText("Sorry, But You Do Not Have (500) Online Points")
                            .AddOption("Okay thanks.", 255).AddAvatar(3497).FinalizeDialog();

                        }
                        break;
                    }
                case 2:
                    {

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;

                        if (client.Player.OnlineMinutes >= 2000)
                        {
                            client.Player.OnlineMinutes -= 2000;

                            client.Inventory.AddItemWitchStack(3004121, 0, 1, stream, false); // Vip 6 Token [1day]

                            MsgSchedules.SendSysMesage(" [ " + client.Player.Name + " ] Got VIP 6 Token (1 Day) for 2000 Online Points ", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                        }
                        else
                        {

                            dialog.AddText("Sorry, But You Do Not Have (2000) Online Points")
                            .AddOption("Okay thanks.", 255).AddAvatar(3497).FinalizeDialog();

                        }
                        break;
                    }

                case 3:
                    {
                        if (client.Player.OnlineMinutes >= 150)
                        {
                            Database.ItemType.DBItem aitem;
                            for (int _hg1 = 500000; _hg1 > 0; _hg1--)  // [hang-fix] cap to avoid infinite spin on bad data (normal play exits in a few iterations)
                            {
                                var array = Database.ItemType.Garments.Values.ToArray();
                                aitem = array[Pool.GetRandom.Next(0, array.Length)];
                                Database.CoatStorage.StorageItem DbGarment;
                                if (Database.CoatStorage.Garments.TryGetValue(aitem.ID, out DbGarment))
                                {
                                    if (DbGarment.Stars == 1 || DbGarment.Stars == 2 || DbGarment.Stars == 3)
                                    {
                                        if (DbGarment.Stars == 3 && Role.Core.Rate(50)) continue;
                                        if (client.Inventory.Add(stream, aitem.ID))
                                        {
                                            client.Player.OnlineMinutes -= 150;
                                            Server.SendGlobalPacket(new Game.MsgServer.MsgMessage("Congratulations, [ " + client.Player.Name + " ] He won " + aitem.Name + " " + DbGarment.Stars + " Star [Garment] For 150 (Online Points) ", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                                        }
                                        else
                                        {
                                            client.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            dialog.AddText("Sorry, But You Do Not Have (150) Online Points")
                            .AddOption("Okay thanks.", 255).AddAvatar(3497).FinalizeDialog();
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Player.OnlineMinutes >= 150)
                        {
                            Database.ItemType.DBItem aitem;
                            for (int _hg2 = 500000; _hg2 > 0; _hg2--)  // [hang-fix] cap to avoid infinite spin on bad data (normal play exits in a few iterations)
                            {
                                var array = Database.ItemType.SteedMounts.Values.ToArray();
                                aitem = array[Pool.GetRandom.Next(0, array.Length)];
                                Database.CoatStorage.StorageItem DBMount;
                                if (Database.CoatStorage.Mounts.TryGetValue(aitem.ID, out DBMount))
                                {
                                    if (DBMount.Stars == 1 || DBMount.Stars == 2 || DBMount.Stars == 3)
                                    {
                                        if (DBMount.Stars == 3 && Role.Core.Rate(50)) continue;
                                        if (client.Inventory.Add(stream, aitem.ID))
                                        {
                                            client.Player.OnlineMinutes -= 150;
                                            Server.SendGlobalPacket(new Game.MsgServer.MsgMessage("Congratulations, [ " + client.Player.Name + " ] He won " + aitem.Name + " " + DBMount.Stars + " Star [Mount] For 150 (Online Points) ", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                                        }
                                        else
                                        {
                                            client.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            dialog.AddText("Sorry, But You Do Not Have (150) Online Points")
                            .AddOption("Okay thanks.", 255).AddAvatar(3497).FinalizeDialog();
                        }
                        break;
                    }
                case 5:
                    {
                        if (client.Player.OnlineMinutes >= 300)
                        {
                            Database.ItemType.DBItem aitem;
                            for (int _hg3 = 500000; _hg3 > 0; _hg3--)  // [hang-fix] cap to avoid infinite spin on bad data (normal play exits in a few iterations)
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
                                            client.Player.OnlineMinutes -= 300;
                                            Server.SendGlobalPacket(new Game.MsgServer.MsgMessage("Congratulations, [ " + client.Player.Name + " ] He won " + aitem.Name + " " + DbGarment.Stars + " Star [Garment] For 300 (Online Points) ", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                                        }
                                        else
                                        {
                                            client.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            dialog.AddText("Sorry, But You Do Not Have (300) Online Points")
                            .AddOption("Okay thanks.", 255).AddAvatar(3497).FinalizeDialog();
                        }
                        break;
                    }
                case 6:
                    {
                        if (client.Player.OnlineMinutes >= 300)
                        {
                            Database.ItemType.DBItem aitem;
                            for (int _hg4 = 500000; _hg4 > 0; _hg4--)  // [hang-fix] cap to avoid infinite spin on bad data (normal play exits in a few iterations)
                            {
                                var array = Database.ItemType.SteedMounts.Values.ToArray();
                                aitem = array[Pool.GetRandom.Next(0, array.Length)];
                                Database.CoatStorage.StorageItem DBMount;
                                if (Database.CoatStorage.Mounts.TryGetValue(aitem.ID, out DBMount))
                                {
                                    if (DBMount.Stars > 3)
                                    {
                                        if (DBMount.Stars == 5 && Role.Core.Rate(50)) continue;
                                        if (client.Inventory.Add(stream, aitem.ID))
                                        {
                                            client.Player.OnlineMinutes -= 300;
                                            Server.SendGlobalPacket(new Game.MsgServer.MsgMessage("Congratulations, [ " + client.Player.Name + " ] He won " + aitem.Name + " " + DBMount.Stars + " Star [Mount] For 300 (Online Points) ", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                                        }
                                        else
                                        {
                                            client.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            dialog.AddText("Sorry, But You Do Not Have (300) Online Points")
                            .AddOption("Okay thanks.", 255).AddAvatar(3497).FinalizeDialog();
                        }
                        break;
                    }

                case 7:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                            break;
                        }
                        if (client.Player.OnlineMinutes >= 150)
                        {
                            client.Inventory.AddItemWitchStack(3307146, 0, 1, stream, false); // StunningRelicsBox
                            client.Player.OnlineMinutes -= 150;
                            Server.SendGlobalPacket(new Game.MsgServer.MsgMessage("Congratulations, [ " + client.Player.Name + " ] He won Random Relic For 150 (Online Points) " + "", "", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                        }
                        else
                        {
                            dialog.AddText("Sorry, But You Do Not Have (150) Online Points")
                            .AddOption("Okay thanks.", 255).AddAvatar(3497).FinalizeDialog();
                        }
                        break;
                    }
                case 8:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                            break;
                        }
                        if (client.Player.OnlineMinutes >= 100)
                        {
                            client.Inventory.AddItemWitchStack(3009104, 0, 1, stream, false);
                            Server.SendGlobalPacket(new Game.MsgServer.MsgMessage("Congratulations, [ " + client.Player.Name + " ] He won x1 RedBodhiBean For 100 (Online Points) " + "", "", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                            client.Player.OnlineMinutes -= 100;
                        }
                        else
                        {
                            dialog.AddText("Sorry, But You Do Not Have (100) Online Points")
                            .AddOption("Okay thanks.", 255).AddAvatar(3497).FinalizeDialog();
                        }
                        break;
                    }
                case 9:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                            break;
                        }
                        if (client.Player.OnlineMinutes >= 1000)
                        {
                            client.Inventory.AddItemWitchStack(3009104, 0, 10, stream, false);
                            Server.SendGlobalPacket(new Game.MsgServer.MsgMessage("Congratulations, [ " + client.Player.Name + " ] He won x10 RedBodhiBeans For 1000 (Online Points) " + "", "", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                            client.Player.OnlineMinutes -= 1000;
                        }
                        else
                        {
                            dialog.AddText("Sorry, But You Do Not Have (1000) Online Points")
                            .AddOption("Okay thanks.", 255).AddAvatar(3497).FinalizeDialog();
                        }
                        break;
                    }
                case 10:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                            break;
                        }
                        if (client.Player.OnlineMinutes >= 100)
                        {
                            client.Inventory.AddItemWitchStack(3324226, 0, 1, stream, false);
                            Server.SendGlobalPacket(new Game.MsgServer.MsgMessage("Congratulations, [ " + client.Player.Name + " ] He won x1 Cosmos Hammer For 100 (Online Points) " + "", "", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                            client.Player.OnlineMinutes -= 100;
                        }
                        else
                        {
                            dialog.AddText("Sorry, But You Do Not Have (100) Online Points.")
                            .AddOption("Okay thanks.", 255).AddAvatar(3497).FinalizeDialog();
                        }
                        break;
                    }
                case 11: 
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                            break;
                        }
                        if (client.Player.OnlineMinutes >= 1000)
                        {
                            client.Inventory.AddItemWitchStack(3324226, 0, 10, stream, false);
                            Server.SendGlobalPacket(new Game.MsgServer.MsgMessage("Congratulations, [ " + client.Player.Name + " ] He won x10 Cosmos Hammers For 1000 (Online Points) " + "", "", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                            client.Player.OnlineMinutes -= 1000;
                        }
                        else
                        {
                            dialog.AddText("Sorry, But You Do Not Have (1000) Online Points.")
                            .AddOption("Okay thanks.", 255).AddAvatar(3497).FinalizeDialog();
                        }
                        break;
                    }

                case 12:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                            break;
                        }
                        if (client.Player.OnlineMinutes >= 100)
                        {
                            var Accessorys = Database.ItemType.Accessorys.Values.ToArray();
                            int position = Pool.GetRandom.Next(0, Accessorys.Length);
                            var accesory = Accessorys[position];
                            client.Inventory.Add(stream, accesory.ID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false);
                            Server.SendGlobalPacket(new Game.MsgServer.MsgMessage("Congratulations, [ " + client.Player.Name + " ] He won x1 Rare Accessory For 100 (Online Points) " + "", "", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                            client.Player.OnlineMinutes -= 100;
                        }
                        else
                        {
                            dialog.AddText("Sorry, But You Do Not Have (100) Online Points.")
                            .AddOption("Okay thanks.", 255).AddAvatar(3497).FinalizeDialog();
                        }
                        break;
                    }
            }
        }
    }
}

