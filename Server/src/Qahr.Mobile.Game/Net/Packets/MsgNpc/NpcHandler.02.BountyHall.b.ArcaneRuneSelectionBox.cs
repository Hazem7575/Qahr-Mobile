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
        #region ItemLottery
        #region New Item Lottery
        public static void ArcaneRuneSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~Arcane~Rune~Pack~or~160~Peach~Jades.");
                        dialog.AddOption("1~Arcane~Rune~Pack", 1);
                        dialog.AddOption("160~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3331664, 1);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 160, stream);
                        }
                        break;
                    }
            }
        }
        public static void LegendarySpiritStonePack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~one~from~30~Spirit~Stones/30~Legendary~Essences/50~Universal~Rune~Essences.");
                        dialog.AddOption("30~Spirit~Stones", 1);
                        dialog.AddOption("30~Legendary~Essences", 2);
                        dialog.AddOption("50~Universal~Rune~Essences.", 3);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3321107, 0, 30, stream);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3321098, 0, 30, stream);
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(4060001, 0, 50, stream);
                        }
                        break;
                    }
            }
        }
        public static void SoldierSoulSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~Legendary~Spirit~Stone~Pack~or~150~Peach~Jades.");
                        dialog.AddOption("1~Legendary~Spirit~Stone~Pack", 1);
                        dialog.AddOption("150~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3331669, 1);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 150, stream);
                        }
                        break;
                    }
            }
        }
        public static void P6BlissPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~one~from~50~Rune~Essences/30~Spirit~Stones/30~Legendary~Essences/20~Arcane~Essences/1~Super~Glory~Gem/1~Super~Thunder~Gem/1~Super~Tortoise~Gem/1~Dragon~Ball/30KK~Silver~Pack/etc.");
                        dialog.AddOption("50~Rune~Essences", 1);
                        dialog.AddOption("30~Spirit~Stones", 2);
                        dialog.AddOption("30~Legendary~Essences", 3);
                        dialog.AddOption("20~Arcane~Essences", 4);
                        dialog.AddOption("1~Super~Glory~Gem", 5);
                        dialog.AddOption("1~Super~Thunder~Gem", 6);
                        dialog.AddOption("1~Super~Tortoise~Gem", 7);
                        dialog.AddOption("1~Dragon~Ball", 8);
                        dialog.AddOption("30KK~Silver~Pack", 9);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(4060001, 0, 50, stream);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3321107, 0, 30, stream);
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3321098, 0, 30, stream);
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316501, 0, 20, stream);
                        }
                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 700123, 1, 0);
                        }
                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 700103, 1, 0);
                        }
                        break;
                    }
                case 7:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 700073, 1);
                        }
                        break;
                    }
                case 8:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 1088000, 1);
                        }
                        break;
                    }
                case 9:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3331789, 1);
                        }
                        break;
                    }
            }
        }
        public static void P6FateSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~P5~Bliss~Pack~or~300~Peach~Jades.");
                        dialog.AddOption("1~P6~Bliss~Pack", 1);
                        dialog.AddOption("300~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3331655, 1);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 300, stream);
                        }
                        break;
                    }
            }
        }
        public static void BlueRuneBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            string add = Input == "3314244" ? "~(B)" : "";
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Please choose one from the following Blue Rune " + add + ".")
                       .AddOption("Trojan-Rampage" + add + ".", 1)
                       .AddOption("Warrior-All-round" + add + ".", 2)
                       .AddOption("Archer-Infinity" + add + ".", 3)
                       .AddOption("Ninja-Absolution" + add + ".", 4)
                       .AddOption("Monk-Iron~Shield" + add + ".", 5)
                       .AddOption("Pirate-Slayer" + add + ".", 6)
                       .AddOption("DragonWarrior-Counter Punch" + add + ".", 7)
                       .AddOption("Next.", 8)
                       .FinalizeDialog();
                        break;
                    }
                case 8:
                    {
                        data.AddText("Please choose one from the following Blue Rune " + add + ".")
                       .AddOption("WaterTaoist-Fine~Rain" + add + ".", 9)
                       .AddOption("FireTaoist-Fire~Curse" + add + ".", 10)
                       .AddOption("Windwalker-Wild~Wind" + add + ".", 11)
                       .AddOption("Thunderstriker`s~Flash~Shield" + add + ".", 12)
                       .AddOption("WindWalker~Double~Thunder" + add + ".", 13)
                            //  .AddOption("Next.", 80)
                       .FinalizeDialog();
                        break;
                    }
                case 80:
                    {
                        data.AddText("Please choose one from the following Blue Rune " + add + ".")
                        .AddOption("WindWalker~Crack~Star" + add + ".", 14)
                        .AddOption("DragonWarrior~Shadow~Fist" + add + ".", 15)
                        .AddOption("Warrior~Retaliation" + add + ".", 16)
                        .AddOption("Archer~Star~Raid" + add + ".", 17)
                        .AddOption("Ninja~Doul" + add + ".", 18)
                        .AddOption("Next.", 70)
                        .FinalizeDialog();
                        break;
                    }
                case 70:
                    {
                        data.AddText("Please choose one from the following Blue Rune " + add + ".")
                          .AddOption("Pirate~Neptune`sCurs" + add + ".", 19)//Neptune`sCurs
                          .AddOption("WaterTaoist~Space~Leap" + add + ".", 20)//Neptune`sCurs
                          .AddOption("FireTaoist~RiseofTaoism" + add + ".", 21)//Neptune`sCurs
                          .AddOption("Monk~Fury~Strike" + add + ".", 22)//Neptune`sCurs
                          .AddOption("ThunderStrike~Flash~Shield" + add + ".", 23)
                          .AddOption("Trojan~Blood~Tide" + add + ".", 24)
                          .AddOption("Thx.", 0)
                          .FinalizeDialog();
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
                                    case 1: { itemID = 4020127; break; }
                                    case 2: { itemID = 4020227; break; }
                                    case 3: { itemID = 4020327; break; }
                                    case 4: { itemID = 4020427; break; }
                                    case 5: { itemID = 4020527; break; }
                                    case 6: { itemID = 4020627; break; }
                                    case 7: { itemID = 4020727; break; }
                                    case 9: { itemID = 4020827; break; }
                                    case 10: { itemID = 4020927; break; }
                                    case 11: { itemID = 4021027; break; }
                                    case 12: { itemID = 4022227; break; }
                                    case 13: { itemID = 4021127; break; }
                                    case 14: { itemID = 4021227; break; }
                                    case 15: { itemID = 4021327; break; }
                                    case 16: { itemID = 4021427; break; }
                                    case 17: { itemID = 4021527; break; }
                                    case 18: { itemID = 4021627; break; }
                                    case 19: { itemID = 4021727; break; }
                                    case 20: { itemID = 4021827; break; }
                                    case 21: { itemID = 4021927; break; }
                                    case 22: { itemID = 4022027; break; }
                                    case 23: { itemID = 4022227; break; }
                                    case 24: { itemID = 4023027; break; }
                                }
                                if (item.Bound > 0)
                                    client.Inventory.Add(stream, itemID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                else
                                    client.Inventory.Add(stream, itemID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false);
                            }
                        }

                        break;
                    }
            }
        }
        public static void OptionalRareBlueRunePack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain(      3311747, 1))
            {
                client.SendSysMesage("Check your inventory!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                return; // ( Done )
            }
            string add = client.ActiveNpc == 3311747 ? "" : "";
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        goto case 50;
                    }
                case 50:
                    {
                        data.AddText("Please choose one from the following Rare Blue Rune " + add + "(+1).")
                       .AddOption("Trojan Blood Tide (+1)" + add + ".", 1)
                       .AddOption("Warrior Retaliation (+1)" + add + ".", 2)
                       .AddOption("Archer Star Raid (+1)" + add + ".", 3)
                       .AddOption("Ninja Duel (+1)" + add + ".", 4)
                       .AddOption("Monk Fury Strike (+1)" + add + ".", 5)
                       .AddOption("Pirate Neptune`s Curse (+1)" + add + ".", 6)
                       .AddOption("Dragon Warrior Shadow Fist (+1)" + add + ".", 7)
                       .AddOption("Next Page.", 8)
                       .AddAvatar(2947).FinalizeDialog();//done
                        break;
                    }
                case 8:
                    {
                        data.AddText("Please choose one from the following Rare Blue Rune " + add + "(+1).")
                       .AddOption("Water Taoist Space Leap (+1)" + add + ".", 9)
                       .AddOption("Fire Taoist Rise of Taoism (+1)" + add + ".", 10)
                       .AddOption("Wind Walker Double Thunder (+1)" + add + ".", 11)
                       .AddOption("Wind Walker Crack Star (+1)" + add + ".", 12)
                       .AddOption("Archer Caprice (+1)" + add + ".", 13)
                       .AddOption("Previous.", 50)
                       .AddOption("Leave.", 255)
                       .AddAvatar(2947).FinalizeDialog();//done
                        break;
                    }
                default:
                    {
                        if (client.Inventory.Remove(client.ActiveNpc, 1, stream))
                        {
                            uint itemID = 0;
                            switch (Option)
                            {
                                case 1: { itemID = 4023001; break; }
                                case 2: { itemID = 4021401; break; }
                                case 3: { itemID = 4021501; break; }
                                case 4: { itemID = 4021601; break; }
                                case 5: { itemID = 4022001; break; }
                                case 6: { itemID = 4021701; break; }
                                case 7: { itemID = 4021301; break; }
                                case 9: { itemID = 4021801; break; }
                                case 10: { itemID = 4021901; break; }
                                case 11: { itemID = 4021101; break; }
                                case 12: { itemID = 4021201; break; }
                                case 13: { itemID = 4028001; break; }
                            }
                            // client.Inventory.Add(stream, itemID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, client.ActiveNpc == 3311747);
                            client.Inventory.Add(stream, itemID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false);
                        }
                        break;
                    }
            }
        }
        public static void OptionalRareYellowRunePack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain(3311746, 1))
            {
                client.SendSysMesage("Check your inventory!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                return; // ( Done )
            }
            string add = Input == "3311746" ? "" : "";
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("Please choose one from the following Rare Yellow Rune " + add + "(+1).")
                       .AddOption("QuickMount(+1)" + add + ".", 1) //done
                       .AddOption("Breakdown(+1)" + add + ".", 2) //done
                       .AddOption("Barrier(+1)" + add + ".", 3)   //done
                       .AddOption("Sanctity(+1)" + add + ".", 4)  //done
                       .AddOption("FireBlast(+1)" + add + ".", 5) //done
                       .AddOption("Serenity(+1)" + add + ".", 6)  //done
                       .AddOption("Wonder(+1)" + add + ".", 7)  //done
                       .AddOption("SoulChant(+1)" + add + ".", 8) //done
                       .AddOption("WideSwipe(+1)" + add + ".", 9) //done
                       .AddOption("Next~Page.", 70)
                       .AddAvatar(766)//done
                       .FinalizeDialog();
                        break;
                    }
                case 70:
                    {
                        data.AddText("Please choose one from the following Rare Yellow Rune " + add + "(+1).")
                       .AddOption("Nirvana(+1)" + add + ".", 10) //done
                       .AddOption("Sharpness(+1)" + add + ".", 11) //done
                       .AddOption("Evocation(+1)" + add + ".", 12) //done
                       .AddOption("Overwhelm(+1)" + add + ".", 13) //done
                       .AddOption("Judgment(+1)" + add + ".", 14) //done
                       .AddOption("DeadlySight(+1)" + add + ".", 15) //done
                       .AddOption("TacitStrike(+1)" + add + ".", 16) //done
                       .AddOption("ReverseMagic(+1)" + add + ".", 17) //done
                       .AddOption("DivineShield(+1)" + add + ".", 18) //done
                       .AddOption("Next~Page.", 100)
                       .AddOption("Previous" + add + ".", 80)
                       .AddAvatar(766)//done
                       .FinalizeDialog();
                        break;
                    }
                case 100:
                    {
                        data.AddText("Please choose one from the following Rare Yellow Rune " + add + "(+1).")
                       .AddOption("Deterrence(+1)" + add + ".", 19) //done
                       .AddOption("Nature`sChant(+1)" + add + ".", 20) //done
                       .AddOption("JusticeGuard(+1)" + add + ".", 21) //done
                       .AddOption("SurgingForce(+1)" + add + ".", 22) //done
                       .AddOption("MiracleTouch(+1)" + add + ".", 23) //done
                       .AddOption("CurseBlock(+1)" + add + ".", 24) //done
                       .AddOption("UniversalShield(+1)" + add + ".", 25) //done
                       .AddOption("TortoiseSmasher(+1)" + add + ".", 26) //done
                       .AddOption("NatureShield(+1)" + add + ".", 27) //done
                       .AddOption("VioletShield(+1)" + add + ".", 28)
                       .AddOption("Previous" + add + ".", 70)
                       .AddAvatar(766)//done
                       .FinalizeDialog();
                        break;
                    }
                case 80:
                    {
                        data.AddText("Please choose one from the following Rare Yellow Rune " + add + "(+1).")
                       .AddOption("QuickMount(+1)" + add + ".", 1) //done
                       .AddOption("Breakdown(+1)" + add + ".", 2) //done
                       .AddOption("Barrier(+1)" + add + ".", 3)   //done
                       .AddOption("Sanctity(+1)" + add + ".", 4)  //done
                       .AddOption("FireBlast(+1)" + add + ".", 5) //done
                       .AddOption("Serenity(+1)" + add + ".", 6)  //done
                       .AddOption("Wonder(+1)" + add + ".", 7)  //done
                       .AddOption("SoulChant(+1)" + add + ".", 8) //done
                       .AddOption("WideSwipe(+1)" + add + ".", 9) //done
                       .AddOption("Next~Page.", 70)
                       .AddAvatar(766)//done
                       .FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(id, 1))
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4031401, 0, 1, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4031401, 0, 1, stream, true);
                            }
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(id, 1))
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4031501, 0, 1, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4031501, 0, 1, stream, true);
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.Contain(id, 1))
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4031601, 0, 1, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4031601, 0, 1, stream, true);
                            }
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.Contain(id, 1))
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4032001, 0, 1, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4032001, 0, 1, stream, true);
                            }
                        }
                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.Contain(id, 1))
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4032101, 0, 1, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4032101, 0, 1, stream, true);
                            }
                        }
                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.Contain(id, 1))
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4032201, 0, 1, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4032201, 0, 1, stream, true);
                            }
                        }
                        break;
                    }
                case 7:
                    {
                        if (client.Inventory.Contain(id, 1))
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4032301, 0, 1, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4032301, 0, 1, stream, true);
                            }
                        }
                        break;
                    }
                case 8:
                    {
                        if (client.Inventory.Contain(id, 1))
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4032401, 0, 1, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4032401, 0, 1, stream, true);
                            }
                        }
                        break;
                    }
                case 9:
                    {
                        if (client.Inventory.Contain(id, 1))
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4032501, 0, 1, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4032501, 0, 1, stream, true);
                            }
                        }
                        break;
                    }
                case 10:
                    {
                        if (client.Inventory.Contain(id, 1))
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4032601, 0, 1, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4032601, 0, 1, stream, true);
                            }
                        }
                        break;
                    }
                case 11:
                    {
                        if (client.Inventory.Contain(id, 1))
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4032701, 0, 1, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4032701, 0, 1, stream, true);
                            }
                        }
                        break;
                    }
                case 12:
                    {
                        if (client.Inventory.Contain(id, 1))
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4032801, 0, 1, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4032801, 0, 1, stream, true);
                            }
                        }
                        break;
                    }
                case 13:
                    {
                        if (client.Inventory.Contain(id, 1))
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4032901, 0, 1, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4032901, 0, 1, stream, true);
                            }
                        }
                        break;
                    }
                case 14:
                    {
                        if (client.Inventory.Contain(id, 1))
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4033001, 0, 1, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4033001, 0, 1, stream, true);
                            }
                        }
                        break;
                    }
                case 15:
                    {
                        if (client.Inventory.Contain(id, 1))
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4033301, 0, 1, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4033301, 0, 1, stream, true);
                            }
                        }
                        break;
                    }
                case 16:
                    {
                        if (client.Inventory.Contain(id, 1))
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4033401, 0, 1, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4033401, 0, 1, stream, true);
                            }
                        }
                        break;
                    }
                case 17:
                    {
                        if (client.Inventory.Contain(id, 1))
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4033901, 0, 1, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4033901, 0, 1, stream, true);
                            }
                        }
                        break;
                    }
                case 18:
                    {
                        if (client.Inventory.Contain(id, 1))
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4034001, 0, 1, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4034001, 0, 1, stream, true);
                            }
                        }
                        break;
                    }
                case 19:
                    {
                        if (client.Inventory.Contain(id, 1))
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4034101, 0, 1, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4034101, 0, 1, stream, true);
                            }
                        }
                        break;
                    }
                case 20:
                    {
                        if (client.Inventory.Contain(id, 1))
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4034201, 0, 1, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4034201, 0, 1, stream, true);
                            }
                        }
                        break;
                    }
                case 21:
                    {
                        if (client.Inventory.Contain(id, 1))
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4034301, 0, 1, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4034301, 0, 1, stream, true);
                            }
                        }
                        break;
                    }
                case 22:
                    {
                        if (client.Inventory.Contain(id, 1))
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4034401, 0, 1, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4034401, 0, 1, stream, true);
                            }
                        }
                        break;
                    }
                case 23:
                    {
                        if (client.Inventory.Contain(id, 1))
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4034501, 0, 1, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4034501, 0, 1, stream, true);
                            }
                        }
                        break;
                    }
                case 24:
                    {
                        if (client.Inventory.Contain(id, 1))
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4034601, 0, 1, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4034601, 0, 1, stream, true);
                            }
                        }
                        break;
                    }
                case 25:
                    {
                        if (client.Inventory.Contain(id, 1))
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4034701, 0, 1, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4034701, 0, 1, stream, true);
                            }
                        }
                        break;
                    }
                case 26:
                    {
                        if (client.Inventory.Contain(id, 1))
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4034801, 0, 1, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4034801, 0, 1, stream, true);
                            }
                        }
                        break;
                    }
                case 27:
                    {
                        if (client.Inventory.Contain(id, 1))
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4035101, 0, 1, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4035101, 0, 1, stream, true);
                            }
                        }
                        break;
                    }
                case 28:
                    {
                        if (client.Inventory.Contain(id, 1))
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4035201, 0, 1, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4035201, 0, 1, stream, true);
                            }
                        }
                        break;
                    }
            }
        }
        public static void YellowRuneBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain(3314243, 1))
            {
                client.SendSysMesage("Check your inventory!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                return; // ( Done )
            }
            string add = Input == "3314243" ? "" : "";
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("Please choose one from the following Yellow Rune " + add + "(+1).")
                       .AddOption("TortoiseBreaker(+1)" + add + ".", 1)
                       .AddOption("DrainingTouchBooster(+1)" + add + ".", 2)
                       .AddOption("BloodSpawnBooster(+1)" + add + ".", 3)
                       .AddOption("Healer(+1)" + add + ".", 4)
                       .AddOption("Sturdiness(+1)" + add + ".", 5)
                       .AddOption("XPKiller(+1)" + add + ".", 6)
                       .AddOption("Grabber(+1)" + add + ".", 7)
                       .AddOption("MPMaster(+1)" + add + ".", 8)
                       .AddOption("BossKiller(+1)" + add + ".", 9)
                       .AddOption("Next~Page.", 70)
                            .AddAvatar(2683)//done
                       .FinalizeDialog();
                        break;
                    }
                case 70:
                    {
                        data.AddText("Please choose one from the following Yellow Rune " + add + "(+1).")
                       .AddOption("NoMercy(+1)" + add + ".", 10)
                       .AddOption("XPBooster(+1)" + add + ".", 11)
                       .AddOption("FreeSoulBooster(+1)" + add + ".", 12)
                       .AddOption("Witchery(+1)" + add + ".", 13)
                       .AddOption("TidalWave(+1)" + add + ".", 14)
                       .AddOption("Quench(+1)" + add + ".", 15)
                       .AddOption("BloodFeast(+1)" + add + ".", 16)
                       .AddOption("LifeDrain(+1)" + add + ".", 17)
                       .AddOption("Previous" + add + ".", 80)
                       .AddOption("Leave" + add + ".", 255)
                            .AddAvatar(2683)//done
                       .FinalizeDialog();
                        break;
                    }
                case 80:
                    {
                        data.AddText("Please choose one from the following Yellow Rune " + add + "(+1).")
                       .AddOption("TortoiseBreaker(+1)" + add + ".", 1)
                       .AddOption("DrainingTouchBooster(+1)" + add + ".", 2)
                       .AddOption("BloodSpawnBooster(+1)" + add + ".", 3)
                       .AddOption("Healer(+1)" + add + ".", 4)
                       .AddOption("Sturdiness(+1)" + add + ".", 5)
                       .AddOption("XPKiller(+1)" + add + ".", 6)
                       .AddOption("Grabber(+1)" + add + ".", 7)
                       .AddOption("MPMaster(+1)" + add + ".", 8)
                       .AddOption("BossKiller(+1)" + add + ".", 9)
                       .AddOption("Next~Page.", 70)
                            .AddAvatar(2683)//done
                       .FinalizeDialog();
                        break;
                    }
                default:
                    {
                        if (client.Inventory.Remove(client.ActiveNpc, 1, stream))
                        {
                            uint itemID = 0;
                            switch (Option)
                            {
                                case 1: { itemID = 4030101; break; }
                                case 2: { itemID = 4030201; break; }
                                case 3: { itemID = 4030301; break; }
                                case 4: { itemID = 4030401; break; }
                                case 5: { itemID = 4030501; break; }
                                case 6: { itemID = 4030601; break; }
                                case 7: { itemID = 4030701; break; }
                                case 8: { itemID = 4030801; break; }
                                case 9: { itemID = 4030901; break; }
                                case 10: { itemID = 4031001; break; }
                                case 11: { itemID = 4031101; break; }
                                case 12: { itemID = 4031201; break; }
                                case 13: { itemID = 4031301; break; }
                                case 14: { itemID = 4033101; break; }
                                case 15: { itemID = 4033601; break; }
                                case 16: { itemID = 4033701; break; }
                                case 17: { itemID = 4033801; break; }
                            }
                            client.Inventory.Add(stream, itemID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, Input == "3314243");
                        }
                        break;
                    }
            }
        }
        public static void BlueRuneSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~Blue~Rune~Box~or~600~Peach~Jades.");
                        dialog.AddOption("1~Blue~Rune~Box", 1);
                        dialog.AddOption("600~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3314244, 1);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 600, stream);
                        }
                        break;
                    }
            }
        }
        public static void YellowRuneSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~Yellow~Rune~Box~or~600~Peach~Jades.");
                        dialog.AddOption("1~Yellow~Rune~Box", 1);
                        dialog.AddOption("600~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3314243, 1);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 600, stream);
                        }
                        break;
                    }
            }
        }
        public static void RedRuneSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~Red~Rune~Box~or~600~Peach~Jades.");
                        dialog.AddOption("1~Red~Rune~Box", 1);
                        dialog.AddOption("600~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3314242, 1);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 600, stream);
                        }
                        break;
                    }
            }
        }
        public static void FateSigilSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~Sigil~Box~or~450~Peach~Jades.");
                        dialog.AddOption("1~Sigil~Box", 1);
                        dialog.AddOption("450~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3316499, 1);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 450, stream);
                        }
                        break;
                    }
            }
        }
        public static void FateEraserSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~Power~Eraser~or~330~Peach~Jades.");
                        dialog.AddOption("1~Power~Eraser", 1);
                        dialog.AddOption("330~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3005412, 1);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 330, stream);
                        }
                        break;
                    }
            }
        }
        public static void WindSigilOrchidPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~one~from~1~Wind~Sigil~Box/1~Paradise~Orchid/1~+5~Stone.");
                        dialog.AddOption("1~Wind~Sigil~Box", 1);
                        dialog.AddOption("1~Paradise~Orchid", 2);
                        dialog.AddOption("1~+5~Stone.", 3);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3330859, 1);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 1, stream);
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 730005, 1);
                        }
                        break;
                    }
            }
        }
        public static void WindFateSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~Wind~Sigil~Orchid~Box~or~320~Peach~Jades.");
                        dialog.AddOption("1~Wind~Sigil~Orchid~Box~", 1);
                        dialog.AddOption("320~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3331661, 1);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 320, stream);
                        }
                        break;
                    }
            }
        }
        public static void FireSigilOrchidPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~one~from~1~Water~Sigil~Box/1~Paradise~Orchid/1~+5~Stone.");
                        dialog.AddOption("1~Fire~Sigil~Box", 1);
                        dialog.AddOption("1~Paradise~Orchid", 2);
                        dialog.AddOption("1~+5~Stone.", 3);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3330858, 1);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 1, stream);
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 730005, 1);
                        }
                        break;
                    }
            }
        }
        public static void FireFateSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~Fire~Sigil~Orchid~Box~or~320~Peach~Jades.");
                        dialog.AddOption("1~Fire~Sigil~Orchid~Box~", 1);
                        dialog.AddOption("320~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3331660, 1);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 320, stream);
                        }
                        break;
                    }
            }
        }
        public static void WaterSigilOrchidPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~one~from~1~Water~Sigil~Box/1~Paradise~Orchid/1~+5~Stone.");
                        dialog.AddOption("1~Water~Sigil~Box", 1);
                        dialog.AddOption("1~Paradise~Orchid", 2);
                        dialog.AddOption("1~+5~Stone.", 3);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3330857, 1);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 1, stream);
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 730005, 1);
                        }
                        break;
                    }
            }
        }
        public static void WaterFateSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~Water~Sigil~Orchid~Box~or~320~Peach~Jades.");
                        dialog.AddOption("1~Water~Sigil~Orchid~Box", 1);
                        dialog.AddOption("320~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3331659, 1);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 320, stream);
                        }
                        break;
                    }
            }
        }
        public static void PureStoneSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~+5~Stone~or~200~Peach~Jades.");
                        dialog.AddOption("1~+5~Stone", 1);
                        dialog.AddOption("200~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 730005, 1);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 200, stream);
                        }
                        break;
                    }
            }
        }
        public static void RuneFateSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~Radiant~Star~Ticket~or~300~Peach~Jades.");
                        dialog.AddOption("1~Radiant~Star~Ticket", 1);
                        dialog.AddOption("300~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3200727, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false, Role.Flags.ItemEffect.None, true, "");
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 300, stream);
                        }
                        break;
                    }
            }
        }
        public static void BlueRunePack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            client.ActiveNpcBound = id;
            string add = Input == "3314249" ? "~(B)" : "";
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("Please choose one from the following Blue Rune " + add + ".")
                       .AddOption("Trojan-Rampage" + add + ".", 1)
                       .AddOption("Warrior-All-round" + add + ".", 2)
                       .AddOption("Archer-Infinity" + add + ".", 3)
                       .AddOption("Ninja-Absolution" + add + ".", 4)
                       .AddOption("Monk-Iron~Shield" + add + ".", 5)
                       .AddOption("Pirate-Slayer" + add + ".", 6)
                       .AddOption("DragonWarrior-Counter Punch" + add + ".", 7)
                       .AddOption("Next.", 8)
                       .FinalizeDialog();
                        break;
                    }
                case 8:
                    {
                        data.AddText("Please choose one from the following Blue Rune " + add + ".")
                       .AddOption("WaterTaoist-Fine~Rain" + add + ".", 9)
                       .AddOption("FireTaoist-Fire~Curse" + add + ".", 10)
                       .AddOption("Windwalker-Wild~Wind" + add + ".", 11)
                       .AddOption("Thunderstriker`s~Flash~Shield" + add + ".", 12)
                       .AddOption("WindWalker~Double~Thunder" + add + ".", 13)
                       .FinalizeDialog();
                        break;
                    }

                default:
                    {
                        MsgServer.MsgGameItem item;
                        if (client.Inventory.TryGetItem(client.ActiveNpcBound, out item))
                        {
                            if (item.Bound == 1)
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

                                }
                                if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                                {
                                    client.Inventory.Add(stream, itemID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                }
                            }
                        }
                        else
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

                                }
                                client.Inventory.Add(stream, itemID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false);
                            }
                        }

                        break;
                    }
            }
        }
        public static void YellowRunePack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            string add = Input == "3314248" ? "~(B)" : "";
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("Please choose one from the following Yellow Rune " + add + ".")
                       .AddOption("TortoiseBreaker" + add + ".", 1)
                       .AddOption("DrainingTouchBooster" + add + ".", 2)
                       .AddOption("BloodSpawnBooster" + add + ".", 3)
                       .AddOption("Healer" + add + ".", 4)
                       .AddOption("Sturdiness" + add + ".", 5)
                       .AddOption("XPKiller" + add + ".", 6)
                       .AddOption("Grabber" + add + ".", 7)
                       .AddOption("MPMaster" + add + ".", 8)
                       .AddOption("BossKiller" + add + ".", 9)
                       .AddOption("Next.", 70)
                       .FinalizeDialog();
                        break;
                    }
                case 70:
                    {
                        data.AddText("Please choose one from the following Yellow Rune " + add + ".")
                       .AddOption("NoMercy" + add + ".", 10)
                       .AddOption("XPBooster" + add + ".", 11)
                       .AddOption("FreeSoulBooster" + add + ".", 12)
                       .AddOption("Witchery" + add + ".", 13)
                            //      .AddOption("QuickMount" + add + ".", 14)
                            //    .AddOption("Breakdown" + add + ".", 15)
                            //         .AddOption("Barrier" + add + ".", 16)
                            //       .AddOption("Sanctity" + add + ".", 17)
                            //     .AddOption("FireBlast" + add + ".", 18)
                            //      .AddOption("Next.", 80)
                       .FinalizeDialog();
                        break;
                    }
                case 80:
                    {
                        data.AddText("Please choose one from the following Yellow Rune " + add + ".")
                       .AddOption("Serenity" + add + ".", 19)
                       .AddOption("Wonder" + add + ".", 20)
                       .AddOption("SoulChant" + add + ".", 21)
                       .AddOption("WideSwipe" + add + ".", 22)
                       .AddOption("Nirvana" + add + ".", 23)
                       .AddOption("Sharpness" + add + ".", 24)
                       .AddOption("Evocation" + add + ".", 25)
                       .AddOption("Overwhelm" + add + ".", 26)
                       .AddOption("Judgment" + add + ".", 27)
                       .AddOption("Nexy.", 90)
                       .FinalizeDialog();
                        break;
                    }
                case 90:
                    {
                        data.AddText("Please choose one from the following Yellow Rune " + add + ".")
                       .AddOption("TidalWave" + add + ".", 28)
                       .AddOption("DeadlySight" + add + ".", 29)
                       .AddOption("TacitStrike" + add + ".", 30)
                       .AddOption("Quench" + add + ".", 31)
                       .AddOption("BloodFeast" + add + ".", 32)
                       .AddOption("LifeDrain" + add + ".", 33)
                       .AddOption("ReverseMagic" + add + ".", 34)
                       .AddOption("DivineShield" + add + ".", 35)
                       .AddOption("Deterrence" + add + ".", 36)
                       .AddOption("Previous.", 0)
                       .FinalizeDialog();
                        break;
                    }
                default:
                    {
                        if (client.Inventory.Remove(client.ActiveNpc, 1, stream))
                        {
                            uint itemID = 0;
                            switch (Option)
                            {
                                case 1: { itemID = 4030101; break; }
                                case 2: { itemID = 4030201; break; }
                                case 3: { itemID = 4030301; break; }
                                case 4: { itemID = 4030401; break; }
                                case 5: { itemID = 4030501; break; }
                                case 6: { itemID = 4030601; break; }
                                case 7: { itemID = 4030701; break; }
                                case 8: { itemID = 4030801; break; }
                                case 9: { itemID = 4030901; break; }
                                case 10: { itemID = 4031001; break; }
                                case 11: { itemID = 4031101; break; }
                                case 12: { itemID = 4031201; break; }
                                case 13: { itemID = 4031301; break; }
                            }
                            client.Inventory.Add(stream, itemID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, Input == "3314248");
                        }
                        break;
                    }
            }
        }
        public static void RedRuneBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            string add = Input == "3314242" ? "~(B)" : "";
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("Please choose one from the following Red Rune " + add + ".")
                       .AddOption("Trojan-Comprehension" + add + ".", 1)
                       .AddOption("Warrior-Pitching" + add + ".", 2)
                       .AddOption("Archer-Frost~Arrows" + add + ".", 3)
                       .AddOption("Ninja-Assassin" + add + ".", 4)
                       .AddOption("Monk-Acalanatha" + add + ".", 5)
                       .AddOption("Pirate-Tide~Trap" + add + ".", 6)
                       .AddOption("DragonWarrior-Swinging~Tail" + add + ".", 7)
                       .AddOption("Next.", 8)
                       .FinalizeDialog();
                        break;
                    }
                case 8:
                    {
                        data.AddText("Please choose one from the following Red Rune " + add + ".")
                       .AddOption("WaterTaoist-Sacrifice" + add + ".", 9)
                       .AddOption("FireTaoist-Burning~Sky" + add + ".", 10)
                       .AddOption("Windwalker-Revenge~Gale" + add + ".", 11)
                       .AddOption("Thunderstriker`s~Chained~Storm" + add + ".", 12)
                        .AddOption("Thunderstriker`s~FightingWill" + add + ".", 13)
                       .AddOption("Previous.", 0)
                       .FinalizeDialog();
                        break;
                    }
                default:
                    {
                        if (client.Inventory.Remove(client.ActiveNpc, 1, stream))
                        {
                            uint itemID = 0;
                            switch (Option)
                            {
                                case 1: { itemID = 4010001; break; }
                                case 2: { itemID = 4010101; break; }
                                case 3: { itemID = 4010201; break; }
                                case 4: { itemID = 4010301; break; }
                                case 5: { itemID = 4010401; break; }
                                case 6: { itemID = 4010501; break; }
                                case 7: { itemID = 4010601; break; }
                                case 9: { itemID = 4010701; break; }
                                case 10: { itemID = 4010801; break; }
                                case 11: { itemID = 4010901; break; }
                                case 12: { itemID = 4011501; break; }
                                case 13: { itemID = 4011401; break; }
                            }
                            client.Inventory.Add(stream, itemID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, Input == "3314242");
                        }
                        break;
                    }
            }
        }
        public static void P5BlissPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~one~from~1~Paradise~Orchid/1~Radiant~Star~Ticket/1~+5~Stone/1~Water~Sigil~Box/1~Fire~Sigil~Box/1~Wind~Sigil~Box/1~Sigil~Box/1~Red~Rune~Box/1~Yellow~Rune~Pack/1~Blue~Rune~Box.");
                        dialog.AddOption("1~Paradise~Orchid", 1);
                        dialog.AddOption("1~Radiant~Star~Ticket", 2);
                        dialog.AddOption("1~+5~Stone", 3);
                        dialog.AddOption("1~Water~Sigil~Box", 4);
                        dialog.AddOption("1~Fire~Sigil~Box", 5);
                        dialog.AddOption("1~Wind~Sigil~Box", 6);
                        dialog.AddOption("1~Sigil~Box", 7);
                        dialog.AddOption("1~Red~Rune~Box", 8);
                        dialog.AddOption("1~Yellow~Rune~Pack", 9);
                        dialog.AddOption("1~Blue~Rune~Pack", 10);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 1, stream);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3200727, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false, Role.Flags.ItemEffect.None, true, "");
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 730005, 1, 0);
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3330857, 1, 0);
                        }
                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3330858, 1, 0);
                        }
                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3330859, 1, 0);
                        }
                        break;
                    }
                case 7:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3316499, 1);
                        }
                        break;
                    }
                case 8:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3314247, 1);
                        }
                        break;
                    }
                case 9:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3314248, 1);
                        }
                        break;
                    }
                case 10:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3314249, 1);
                        }
                        break;
                    }
            }
        }
        public static void P5FateSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~P5~Bliss~Pack~or~500~Peach~Jades.");
                        dialog.AddOption("1~P5~Bliss~Pack", 1);
                        dialog.AddOption("500~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3331656, 1);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 500, stream);
                        }
                        break;
                    }
            }
        }
        public static void JadeStoneSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~+6~Stone~or~540~Peach~Jades.");
                        dialog.AddOption("1~+6~Stone", 1);
                        dialog.AddOption("540~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 730006, 1);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 540, stream);
                        }
                        break;
                    }
            }
        }
        public static void FightingSoulSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~200~Legendary~Essences~Pack~or~1000~Peach~Jades.");
                        dialog.AddOption("200~Legendary~Essences~Pack", 1);
                        dialog.AddOption("1000~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3330882, 1);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 1000, stream);
                        }
                        break;
                    }
            }
        }
        public static void FateSoulSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~200~Spirit~Stones~Pack~or~1000~Peach~Jades.");
                        dialog.AddOption("200~Spirit~Stones~Pack", 1);
                        dialog.AddOption("1000~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3330881, 1);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 1000, stream);
                        }
                        break;
                    }
            }
        }
        public static void DragonBallScrollSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~Dragon~Ball~Scroll~or~300~Peach~Jades.");
                        dialog.AddOption("1~Dragon~Ball~Scroll", 1);
                        dialog.AddOption("300~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 720028, 1, 0);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 300, stream);
                        }
                        break;
                    }
            }
        }
        public static void LightningSigilOrchidPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~one~from~1~Lightning~Sigil~Box/1~Rare~Yellow~Rune~Pack.");
                        dialog.AddOption("1~Lightning~Sigil~Box", 1);
                        dialog.AddOption("1~Rare~Yellow~Rune~Pack.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3330861, 1, 0);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3314250, 1, 0);
                        }
                        break;
                    }
            }
        }
        public static void LightningFateSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~Lightning~Sigil~Orchid~Box~or~900~Peach~Jades.");
                        dialog.AddOption("1~Lightning~Sigil~Orchid~Box", 1);
                        dialog.AddOption("900~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3331663, 1, 0);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 900, stream);
                        }
                        break;
                    }
            }
        }
        public static void EarthSigilOrchidPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~one~from~1~Earth~Sigil~Box/1~Rare~Yellow~Rune~Pack.");
                        dialog.AddOption("1~Earth~Sigil~Box", 1);
                        dialog.AddOption("1~Rare~Yellow~Rune~Pack.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3330860, 1, 0);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3314250, 1, 0);
                        }
                        break;
                    }
            }
        }
        public static void EarthFateSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~Earth~Sigil~Orchid~Box~or~900~Peach~Jades.");
                        dialog.AddOption("1~Earth~Sigil~Orchid~Box", 1);
                        dialog.AddOption("900~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3331662, 1, 0);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 900, stream);
                        }
                        break;
                    }
            }
        }
        public static void P4BlissPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~one~from~1~Earth~Sigil~Box/1~Lightning~Sigil~Box/1~Rare~Yellow~Rune~Pack/1~Rare~Blue~Rune~Pack/200~Spirit~Stones/200~Legendary~Essences/1~+6~Stone.");
                        dialog.AddOption("1~Earth~Sigil~Box", 1);
                        dialog.AddOption("1~Lightning~Sigil~Box", 2);
                        dialog.AddOption("1~Rare~Yellow~Rune~Pack", 3);
                        dialog.AddOption("1~Rare~Blue~Rune~Pack", 4);
                        dialog.AddOption("200~Spirit~Stones", 5);
                        dialog.AddOption("200~Legendary~Essences", 6);
                        dialog.AddOption("1~+6~Stone.", 7);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3330860, 1, 0);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3330861, 1, 0);
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3314250, 1, 0);
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3314251, 1, 0);
                        }
                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3321107, 0, 200, stream);
                        }
                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3321098, 0, 200, stream);
                        }
                        break;
                    }
                case 7:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 730006, 1);
                        }
                        break;
                    }
            }
        }
        public static void P4FateSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~P4~Bliss~Pack~or~1000~Peach~Jades.");
                        dialog.AddOption("1~P4~Bliss~Pack", 1);
                        dialog.AddOption("1000~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3331657, 1);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 1000, stream);
                        }
                        break;
                    }
            }
        }
        public static void GreatRichSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~250KK~Silver~Pack~or~5000~Peach~Jades.");
                        dialog.AddOption("250KK~Silver~Pack", 1);
                        dialog.AddOption("5000~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3331790, 1, 0);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 5000, stream);
                        }
                        break;
                    }
            }
        }
        public static void RareBlueRuneSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~Rare~Blue~Rune~Box~or~2000~Peach~Jades.");
                        dialog.AddOption("1~Rare~Blue~Rune~Box", 1);
                        dialog.AddOption("2000~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3316737, 1, 0);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 2000, stream);
                        }
                        break;
                    }
            }
        }
        public static void PermanentOrchidPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~one~from~2~Permanent~Stones/5~Paradise~Orchids.");
                        dialog.AddOption("2~Permanent~Stones", 1);
                        dialog.AddOption("5~Paradise~Orchids.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 723694, 2);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 5, stream);
                        }
                        break;
                    }
            }
        }
        public static void PermanentOrchidSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~Permanent~Orchid~Pack~or~1400~Peach~Jades.");
                        dialog.AddOption("1~Permanent~Orchid~Pack", 1);
                        dialog.AddOption("1400~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3331665, 1, 0);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 1400, stream);
                        }
                        break;
                    }
            }
        }
        public static void ValueSteedSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~Rare~+6~Steed~Pack~or~800~Peach~Jades.");
                        dialog.AddOption("1~Rare~+6~Steed~Pack", 1);
                        dialog.AddOption("800~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddSteed(stream, 300000, 1, 6, false, 0, 0, 0);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 800, stream);
                        }
                        break;
                    }
            }
        }
        public static void HonorSteedSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~+6~Steed~Pack~or~660~Peach~Jades.");
                        dialog.AddOption("1~+6~Steed~Pack", 1);
                        dialog.AddOption("660~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddSteed(stream, 300000, 1, 6, false, 0, 0, 0);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 660, stream);
                        }
                        break;
                    }
            }
        }
        public static void RareBlueRuneBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~Rare~Blue~Rune~Pack~or~1000~Peach~Jades.");
                        dialog.AddOption("1~Rare~Blue~Rune~Pack", 1);
                        dialog.AddOption("1000~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3314251, 1);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 1000, stream);
                        }
                        break;
                    }
            }
        }
        public static void P3BlissPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~one~from~1~+6~Steed/2~Permanent~Stones/5~Paradise~Orchids/1~+6~Rare~Steed/1~Rare~Blue~Rune~Box/1~DB~Scroll/250KK~Silver~Pack.");
                        dialog.AddOption("1~+6~Steed", 1);
                        dialog.AddOption("2~Permanent~Stones", 2);
                        dialog.AddOption("5~Paradise~Orchids", 3);
                        dialog.AddOption("1~+6~Rare~Steed", 4);
                        dialog.AddOption("1~Rare~Blue~Rune~Box", 5);
                        dialog.AddOption("1~DB~Scroll", 6);
                        dialog.AddOption("250KK~Silver~Pack.", 7);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddSteed(stream, 300000, 1, 6, false, 0, 0, 0);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 723694, 2);
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 5, stream);
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddSteed(stream, 300000, 1, 6, false, 0, 0, 0);
                        }
                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3316737, 1);
                        }
                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 720028, 1);
                        }
                        break;
                    }
                case 7:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3331790, 1);
                        }
                        break;
                    }
            }
        }
        public static void P3FateSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~P3~Bliss~Pack~or~2000~Peach~Jades.");
                        dialog.AddOption("1~P3~Bliss~Pack", 1);
                        dialog.AddOption("2000~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3331658, 1);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 2000, stream);
                        }
                        break;
                    }
            }
        }
        public static void PureSteedSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~+7~Steed~or~2000~Peach~Jades.");
                        dialog.AddOption("1~+7~Steed", 1);
                        dialog.AddOption("2000~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddSteed(stream, 300000, 1, 7, false, 0, 0, 0);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 2000, stream);
                        }
                        break;
                    }
            }
        }
        public static void MysteryStarStonePack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~one~from~20~Mystery~Fruits/1~Splendid~Star~Ticket/10~Paradise~Orchids.");
                        dialog.AddOption("20~Mystery~Fruits", 1);
                        dialog.AddOption("1~Splendid~Star~Ticket", 2);
                        dialog.AddOption("10~Paradise~Orchids.", 3);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3001044, 0, 20, stream);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3200728, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false, Role.Flags.ItemEffect.None, true, "");
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 10, stream);
                        }
                        break;
                    }
            }
        }
        public static void ForgingStoneSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~Mystery~Star~Stone~Pack~or~3000~Peach~Jades.");
                        dialog.AddOption("1~Mystery~Star~Stone~Pack", 1);
                        dialog.AddOption("3000~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3330064, 1, 0);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 3000, stream);
                        }
                        break;
                    }
            }
        }
        public static void AdvancedJadeSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~Advanced~Sigil~Box~or~3800~Peach~Jades.");
                        dialog.AddOption("1~Advanced~Sigil~Box", 1);
                        dialog.AddOption("3800~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3330064, 1, 0);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 3800, stream);
                        }
                        break;
                    }
            }
        }
        public static void ChasteStoneSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~+1~+7~Stone~or~1620~Peach~Jades.");
                        dialog.AddOption("1~+1~+7~Stone", 1);
                        dialog.AddOption("1620~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 730007, 1, 0);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 1620, stream);
                        }
                        break;
                    }
            }
        }
        public static void RareYellowRuneBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~Rare~Yellow~Rune~Pack~or~1000~Peach~Jades.");
                        dialog.AddOption("1~Rare~Yellow~Rune~Pack", 1);
                        dialog.AddOption("1000~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3314250, 1, 0);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 1000, stream);
                        }
                        break;
                    }
            }
        }
        public static void RareYellowRuneSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~Rare~Yellow~Rune~Box~or~3000~Peach~Jades.");
                        dialog.AddOption("1~Rare~Yellow~Rune~Box", 1);
                        dialog.AddOption("3000~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3316736, 1, 0);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 3000, stream);
                        }
                        break;
                    }
            }
        }
        public static void BloodlineSigilSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~Bloodline~Sigil~Box~or~20000~Peach~Jades.");
                        dialog.AddOption("1~Bloodline~Sigil~Box", 1);
                        dialog.AddOption("20000~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3330856, 1, 0);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 20000, stream);
                        }
                        break;
                    }
            }
        }
        public static void SimpleSteedSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~+8~Rare~Steed~Pack~or~7500~Peach~Jades.");
                        dialog.AddOption("1~+8~Rare~Steed~Pack", 1);
                        dialog.AddOption("7500~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddSteed(stream, 300000, 1, 8, false, 0, 0, 0);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 7500, stream);
                        }
                        break;
                    }
            }
        }
        public static void ClearSteedSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~+8~Steed~Pack~or~6000~Peach~Jades.");
                        dialog.AddOption("1~+8~Steed~Pack", 1);
                        dialog.AddOption("6000~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddSteed(stream, 300000, 1, 8, false, 0, 0, 0);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 6000, stream);
                        }
                        break;
                    }
            }
        }
        public static void SimpleStoneSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~+8~Stone~or~4860~Peach~Jades.");
                        dialog.AddOption("1~+8~Stone", 1);
                        dialog.AddOption("4860~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 730008, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false, Role.Flags.ItemEffect.None, true);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 4860, stream);
                        }
                        break;
                    }
            }
        }
        public static void PeachGearSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~[Gear]~Lucky~+8~Gear~Pack~or~4374~Peach~Jades.");
                        dialog.AddOption("1~[Gear]~Lucky~+8~Gear~Pack", 1);
                        //   dialog.AddOption("4374~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            uint[] Items = new uint[]
                                {
                                    421078,500078,900008,410078,420088,430088,440088,450088,460088,480088,481088,490088,510088,530088,540088,560088,580088,561088,130038,118038,131038,111038,133038,113018,134038,114038,121088,120088,152108,117038,150078,151078,160078,
                                };
                            uint ItemID = Items[(uint)Pool.GetRandom.Next(0, Items.Length)];
                            client.Inventory.Add(stream, ItemID, 1, 8, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false, Role.Flags.ItemEffect.None, true);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 4374, stream);
                        }
                        break;
                    }
            }
        }
        public static void BigPermanentSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~Big~Permanent~Stone~or~6750~Peach~Jades.");
                        dialog.AddOption("1~Big~Permanent~Stone", 1);
                        dialog.AddOption("6750~Peach~Jades", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 723695, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false, Role.Flags.ItemEffect.None, true);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 6750, stream);
                        }
                        break;
                    }
            }
        }
        public static void TiangongKaiwu(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog obj = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        obj.AddText("STR_ID_tArcherEpicWeapon[3324521][Text111]@@");
                        obj.AddText("STR_ID_tArcherEpicWeapon[3324521][Text112]@@");
                        obj.AddText("STR_ID_tArcherEpicWeapon[3324521][Text113]@@");
                        obj.AddOption("STR_ID_tArcherEpicWeapon[3324521][Option1]@@", 1);
                        obj.AddOption("STR_ID_tArcherEpicWeapon[3324521][Option2]@@", 255);
                        obj.AddAvatar(3392);
                        obj.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        obj.AddText("STR_ID_tArcherEpicWeapon[3324521][Text111]@@");
                        obj.AddText("STR_ID_tArcherEpicWeapon[3324521][Text112]@@");
                        obj.AddText("STR_ID_tArcherEpicWeapon[3324521][Text113]@@");
                        obj.AddOption("STR_ID_tArcherEpicWeapon[3324521][SuccPos4]@@", 3);
                        obj.AddOption("STR_ID_tArcherEpicWeapon[3324521][SuccPos5]@@", 2);
                        obj.AddAvatar(3392);
                        obj.FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        MsgGameItem RightWeapon;
                        if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.RightWeapon, out RightWeapon))
                        {
                            if (RightWeapon.ITEM_ID / 1000 == 606)
                            {
                                obj.AddText("STR_ID_tArcherEpicWeapon[3324521][AllChanged]@@");
                                obj.AddOption("STR_ID_tArcherEpicWeapon[3324521][Option2]@@", 255);
                                obj.AddAvatar(3392);
                                obj.FinalizeDialog();
                                break;
                            }
                            if (RightWeapon.ITEM_ID / 1000 == 613)
                            {
                                if (client.Inventory.Remove(id, 1, stream))
                                {
                                    uint id2 = RightWeapon.ITEM_ID;
                                    RightWeapon.ITEM_ID = (uint)(606000 + (RightWeapon.ITEM_ID % 1000));
                                    RightWeapon.Mode = Role.Flags.ItemMode.Update;
                                    RightWeapon.Refinary = new MsgItemExtra.Refinery();
                                    RightWeapon.Purification = new MsgItemExtra.Purification();
                                    RightWeapon.Send(client, stream);
                                    client.Equipment.QueryEquipment(client.Equipment.Alternante);
                                    obj.AddText("STR_ID_tArcherEpicWeapon[3324521][Succ]@@<STR_ITEM_TYPE_" + id + "@@>@@");
                                    obj.AddOption("STR_ID_tArcherEpicWeapon[3324521][SuccOption]@@", 255);
                                    obj.AddAvatar(3392);
                                    obj.FinalizeDialog();
                                }
                            }
                            else
                            {
                                obj.AddText("STR_ID_tArcherEpicWeapon[3324521][NoAll]@@");
                                obj.AddOption("STR_ID_tArcherEpicWeapon[3324521][Option2]@@", 255);
                                obj.AddAvatar(3392);
                                obj.FinalizeDialog();
                            }
                        }
                        break;
                    }
            }
        }
        public static void RuneSystem(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain(3319466, 1))
            {
                client.CreateBoxDialog("Sorry, you don`t have a PowerBosterPack.");
                return;
            }
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("Please Choose a reward you want.")
                       .AddOption("10000 ChiPoints.", 1)
                       .AddOption("10000 PotencyPoints", 2)
                        .AddOption("10000 StudyPoints", 3)
                         .AddOption("1 Cosmos Hammer", 4)
                         .AddAvatar(1806)
                       .FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        data.AddText("Are you sure want to pick 10000 ChiPoints?")
                            .AddOption("Yes", 5)
                            .AddOption("No Wait a minute", 255)
                              .AddAvatar(1806)
                            .FinalizeDialog();


                        break;
                    }
                case 5:
                    {
                        data.AddText("\t\t\t\t\t\t\t\t\t\t\t\t\t*Success*\n")
                            .AddText("\t\t\t==============================================\n")
                             .AddText("\t\t\t\t\t\t\t\t\t\t You Received: 10000 ChiPoints \n")
                              .AddText("\t\t\t==============================================\n")
                                .AddAvatar(1806)
                              .FinalizeDialog();
                        client.Inventory.Remove(3319466, 1, stream);
                        client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 10000;
                        break;
                    }
                case 6:
                    {
                        data.AddText("\t\t\t\t\t\t\t\t\t\t\t\t\t*Success*\n")
                           .AddText("\t\t\t==============================================\n")
                            .AddText("\t\t\t\t\t\t\t\t\t\t You Received: 10000 PotencyPoints \n")
                             .AddText("\t\t\t==============================================\n")
                               .AddAvatar(1806)
                             .FinalizeDialog();
                        client.Inventory.Remove(3319466, 1, stream);

                        client.Player.InnerPower.AddPotency(stream, client, 10000);

                        break;
                    }
                case 2:
                    {
                        data.AddText("Are you sure want to pick 10000 PotencyPoints?")
                             .AddOption("Yes", 6)
                             .AddOption("No Wait a minute", 255)
                               .AddAvatar(1806)
                             .FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        data.AddText("Are you sure want to pick 10000 StudyPoints?")
                         .AddOption("Yes", 7)
                         .AddOption("No Wait a minute", 255)
                           .AddAvatar(1806)
                         .FinalizeDialog();
                        break;
                    }
                case 7:
                    {
                        data.AddText("\t\t\t\t\t\t\t\t\t\t\t\t\t*Success*\n")
                           .AddText("\t\t\t==============================================\n")
                            .AddText("\t\t\t\t\t\t\t\t\t\t You Received: 10000 StudyPoints \n")
                             .AddText("\t\t\t==============================================\n")
                               .AddAvatar(1806)
                             .FinalizeDialog();
                        client.Inventory.Remove(3319466, 1, stream);

                        client.Player.SubClass.AddStudyPoints(client, 10000, stream);

                        break;
                    }
                case 4:
                    {
                        data.AddText("Are you sure want to pick Cosmos Hammer?")
                         .AddOption("Yes", 8)
                         .AddOption("No Wait a minute", 255)
                           .AddAvatar(1806)
                         .FinalizeDialog();
                        break;
                    }
                case 8:
                    {
                        data.AddText("\t\t\t\t\t\t\t\t\t\t\t\t\t*Success*\n")
                           .AddText("\t\t\t==============================================\n")
                            .AddText("\t\t\t\t\t\t\t\t\t\tYou Received: Cosmos Hammer \n")
                             .AddText("\t\t\t==============================================\n")
                               .AddAvatar(1806)
                             .FinalizeDialog();
                        client.Inventory.Remove(3319466, 1, stream);
                        client.Inventory.Add(stream, 3324226, 1);
                        break;
                    }
            }
        }
        public static void RedRuneSlayer(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            string add = Input == "3306562" ? "~(B)" : "";
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("Please choose one from the following Red Rune " + add + ".")
                       .AddOption("Trojan-Comprehension" + add + ".", 1)
                       .AddOption("Warrior-Pitching" + add + ".", 2)
                       .AddOption("Archer-Frost~Arrows" + add + ".", 3)
                       .AddOption("Ninja-Assassin" + add + ".", 4)
                       .AddOption("Monk-Acalanatha" + add + ".", 5)
                       .AddOption("Pirate-Tide~Trap" + add + ".", 6)
                       .AddOption("DragonWarrior-Swinging~Tail" + add + ".", 7)
                       .AddOption("Next.", 8)
                       .FinalizeDialog();
                        break;
                    }
                case 8:
                    {
                        data.AddText("Please choose one from the following Red Rune " + add + ".")
                       .AddOption("WaterTaoist-Sacrifice" + add + ".", 9)
                       .AddOption("FireTaoist-Burning~Sky" + add + ".", 10)
                       .AddOption("Windwalker-Revenge~Gale" + add + ".", 11)
                       .AddOption("Thunderstriker`s~Chained~Storm" + add + ".", 12)
                        .AddOption("Thunderstriker`s~FightingWill" + add + ".", 13)
                       .AddOption("Previous.", 0)
                       .FinalizeDialog();
                        break;
                    }
                default:
                    {
                        if (client.Inventory.Remove(client.ActiveNpc, 1, stream))
                        {
                            uint itemID = 0;
                            switch (Option)
                            {
                                case 1: { itemID = 4010001; break; }
                                case 2: { itemID = 4010101; break; }
                                case 3: { itemID = 4010201; break; }
                                case 4: { itemID = 4010301; break; }
                                case 5: { itemID = 4010401; break; }
                                case 6: { itemID = 4010501; break; }
                                case 7: { itemID = 4010601; break; }
                                case 9: { itemID = 4010701; break; }
                                case 10: { itemID = 4010801; break; }
                                case 11: { itemID = 4010901; break; }
                                case 12: { itemID = 4011501; break; }
                                case 13: { itemID = 4011401; break; }
                            }
                            client.Inventory.Add(stream, itemID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, Input == "3306562");
                        }
                        break;
                    }
            }
        }
        public static void YellowRuneSlayer(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            string add = Input == "3307212" ? "~(B)" : "";
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("Please choose one from the following Yellow Rune " + add + ".")
                       .AddOption("TortoiseBreaker" + add + ".", 1)
                       .AddOption("DrainingTouchBooster" + add + ".", 2)
                       .AddOption("BloodSpawnBooster" + add + ".", 3)
                       .AddOption("Healer" + add + ".", 4)
                       .AddOption("Sturdiness" + add + ".", 5)
                       .AddOption("XPKiller" + add + ".", 6)
                       .AddOption("Grabber" + add + ".", 7)
                       .AddOption("MPMaster" + add + ".", 8)
                       .AddOption("BossKiller" + add + ".", 9)
                       .AddOption("Next.", 70)
                       .FinalizeDialog();
                        break;
                    }
                case 70:
                    {
                        data.AddText("Please choose one from the following Yellow Rune " + add + ".")
                       .AddOption("NoMercy" + add + ".", 10)
                       .AddOption("XPBooster" + add + ".", 11)
                       .AddOption("FreeSoulBooster" + add + ".", 12)
                       .AddOption("Witchery" + add + ".", 13)
                       .AddOption("QuickMount" + add + ".", 14)
                       .AddOption("Breakdown" + add + ".", 15)
                       .AddOption("Barrier" + add + ".", 16)
                       .AddOption("Sanctity" + add + ".", 17)
                       .AddOption("FireBlast" + add + ".", 18)
                       .AddOption("Next.", 80)
                       .FinalizeDialog();
                        break;
                    }
                case 80:
                    {
                        data.AddText("Please choose one from the following Yellow Rune " + add + ".")
                       .AddOption("Serenity" + add + ".", 19)
                       .AddOption("Wonder" + add + ".", 20)
                       .AddOption("SoulChant" + add + ".", 21)
                       .AddOption("WideSwipe" + add + ".", 22)
                       .AddOption("Nirvana" + add + ".", 23)
                       .AddOption("Sharpness" + add + ".", 24)
                       .AddOption("Evocation" + add + ".", 25)
                       .AddOption("Overwhelm" + add + ".", 26)
                       .AddOption("Judgment" + add + ".", 27)
                       .AddOption("Nexy.", 90)
                       .FinalizeDialog();
                        break;
                    }
                case 90:
                    {
                        data.AddText("Please choose one from the following Yellow Rune " + add + ".")
                       .AddOption("TidalWave" + add + ".", 28)
                       .AddOption("DeadlySight" + add + ".", 29)
                       .AddOption("TacitStrike" + add + ".", 30)
                       .AddOption("Quench" + add + ".", 31)
                       .AddOption("BloodFeast" + add + ".", 32)
                       .AddOption("LifeDrain" + add + ".", 33)
                       .AddOption("ReverseMagic" + add + ".", 34)
                       .AddOption("DivineShield" + add + ".", 35)
                       .AddOption("Deterrence" + add + ".", 36)
                       .AddOption("Next.", 100)
                       .FinalizeDialog();
                        break;
                    }
                case 100:
                    {
                        //4034209 - Nature`sChant
                        //4034309 - JusticeGuard
                        //4034409 - SurgingForce
                        //4034509 - MiracleTouch
                        //4034609 - CurseBlock
                        //4034709 - UniversalShield
                        //4034809 - TortoiseBreaker
                        //4035009 - StrikeBooster
                        //4035109 - NatureShield
                        //4035209 - VioletShield
                        data.AddText("Please choose one from the following Yellow Rune " + add + ".")
                       .AddOption("Nature`sChant" + add + ".", 37)
                       .AddOption("JusticeGuard" + add + ".", 38)
                       .AddOption("SurgingForce" + add + ".", 39)
                       .AddOption("MiracleTouch" + add + ".", 40)
                       .AddOption("CurseBlock" + add + ".", 41)
                       .AddOption("UniversalShield" + add + ".", 42)
                       .AddOption("TortoiseBreaker" + add + ".", 43)
                      // .AddOption("StrikeBooster" + add + ".", 44)
                       .AddOption("NatureShield" + add + ".", 45)
                       .AddOption("VioletShield" + add + ".", 46)
                        .AddOption("Previous.", 0)
                       .FinalizeDialog();
                        break;
                    }
                default:
                    {
                        if (client.Inventory.Remove(client.ActiveNpc, 1, stream))
                        {
                            uint itemID = 0;
                            switch (Option)
                            {
                                case 1: { itemID = 4030101; break; }
                                case 2: { itemID = 4030201; break; }
                                case 3: { itemID = 4030301; break; }
                                case 4: { itemID = 4030401; break; }
                                case 5: { itemID = 4030501; break; }
                                case 6: { itemID = 4030601; break; }
                                case 7: { itemID = 4030701; break; }
                                case 8: { itemID = 4030801; break; }
                                case 9: { itemID = 4030901; break; }
                                case 10: { itemID = 4031001; break; }
                                case 11: { itemID = 4031101; break; }
                                case 12: { itemID = 4031201; break; }
                                case 13: { itemID = 4031301; break; }
                                case 14: { itemID = 4031401; break; }
                                case 15: { itemID = 4031501; break; }
                                case 16: { itemID = 4031601; break; }
                                case 17: { itemID = 4032001; break; }
                                case 18: { itemID = 4032101; break; }
                                case 19: { itemID = 4032201; break; }
                                case 20: { itemID = 4032301; break; }
                                case 21: { itemID = 4032401; break; }
                                case 22: { itemID = 4032501; break; }
                                case 23: { itemID = 4032601; break; }
                                case 24: { itemID = 4032701; break; }
                                case 25: { itemID = 4032801; break; }
                                case 26: { itemID = 4032901; break; }
                                case 27: { itemID = 4033001; break; }
                                case 28: { itemID = 4033101; break; }
                                case 29: { itemID = 4033301; break; }
                                case 30: { itemID = 4033401; break; }
                                case 31: { itemID = 4033601; break; }
                                case 32: { itemID = 4033701; break; }
                                case 33: { itemID = 4033801; break; }
                                case 34: { itemID = 4033901; break; }
                                case 35: { itemID = 4034001; break; }
                                case 36: { itemID = 4034101; break; }
                                case 37: { itemID = 4034201; break; }//4034209 - Nature`sChant
                                case 38: { itemID = 4034301; break; }//4034309 - JusticeGuard
                                case 39: { itemID = 4034401; break; }//4034409 - SurgingForce
                                case 40: { itemID = 4034501; break; }//4034509 - MiracleTouch
                                case 41: { itemID = 4034601; break; }//4034609 - CurseBlock
                                case 42: { itemID = 4034701; break; }//4034709 - UniversalShield
                                case 43: { itemID = 4034801; break; }//4034809 - TortoiseBreaker
                               // case 44: { itemID = 4035001; break; }//4035009 - StrikeBooster
                                case 45: { itemID = 4035101; break; }//4035109 - NatureShield
                                case 46: { itemID = 4035201; break; }//4035209 - VioletShield
                            }
                            client.Inventory.Add(stream, itemID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, Input == "3306562");
                        }
                        break;
                    }
            }
        }
        public static void BlueRuneSlayer(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            string add = Input == "3306563" ? "~(B)" : "";
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("Please choose one from the following Blue Rune " + add + ".")
                       .AddOption("Trojan-Rampage" + add + ".", 1)
                       .AddOption("Warrior-All-round" + add + ".", 2)
                       .AddOption("Archer-Infinity" + add + ".", 3)
                       .AddOption("Ninja-Absolution" + add + ".", 4)
                       .AddOption("Monk-Iron~Shield" + add + ".", 5)
                       .AddOption("Pirate-Slayer" + add + ".", 6)
                       .AddOption("DragonWarrior-Counter Punch" + add + ".", 7)
                       .AddOption("Next.", 8)
                       .FinalizeDialog();
                        break;
                    }
                case 8:
                    {
                        data.AddText("Please choose one from the following Blue Rune " + add + ".")
                       .AddOption("WaterTaoist-Fine~Rain" + add + ".", 9)
                       .AddOption("FireTaoist-Fire~Curse" + add + ".", 10)
                       .AddOption("Windwalker-Wild~Wind" + add + ".", 11)
                       .AddOption("Thunderstriker`s~Flash~Shield" + add + ".", 12)
                       .AddOption("WindWalker~Double~Thunder" + add + ".", 13)
                       .AddOption("Next.", 80)
                       .FinalizeDialog();
                        break;
                    }
                case 80:
                    {
                        data.AddText("Please choose one from the following Blue Rune " + add + ".")
                        .AddOption("WindWalker~Crack~Star" + add + ".", 14)
                        .AddOption("DragonWarrior~Shadow~Fist" + add + ".", 15)
                        .AddOption("Warrior~Retaliation" + add + ".", 16)
                        .AddOption("Archer~Star~Raid" + add + ".", 17)
                        .AddOption("Ninja~Doul" + add + ".", 18)
                        .AddOption("Next.", 70)
                        .FinalizeDialog();
                        break;
                    }
                case 70:
                    {
                        data.AddText("Please choose one from the following Blue Rune " + add + ".")
                          .AddOption("Pirate~Neptune`sCurs" + add + ".", 19)//Neptune`sCurs
                          .AddOption("WaterTaoist~Space~Leap" + add + ".", 20)//Neptune`sCurs
                          .AddOption("FireTaoist~RiseofTaoism" + add + ".", 21)//Neptune`sCurs
                          .AddOption("Monk~Fury~Strike" + add + ".", 22)//Neptune`sCurs
                          .AddOption("ThunderStrike~Flash~Shield" + add + ".", 23)
                          .AddOption("Trojan~Blood~Tide" + add + ".", 24)
                          .AddOption("Thx.", 0)
                          .FinalizeDialog();
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
                            }
                            client.Inventory.Add(stream, itemID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, Input == "3306563");
                        }
                        break;
                    }
            }
        }
        #endregion
        #endregion
        //[NpcAttribute(NpcID.PeachJade)]
        //public static void PeachJade(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog data = new Dialog(client, stream);

        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                data.Text("STR_ID_tPeachGarden[27058][Text111]@@\n");
        //                data.Text("STR_ID_tPeachGarden[27058][Text112]@@\n");
        //                data.Option("STR_ID_tPeachGarden[27058][Option111]@@", 1);
        //                data.Option("STR_ID_tPeachGarden[27058][Option112]@@", 255);
        //                data.AddAvatar(93).FinalizeDialog();
        //                break;
        //            }
        //        case 1:
        //            {
        //                client.MyExchangeShop.Open(stream, 3316793, 196);

        //                break;
        //            }


        //    }

        //}
        //[NpcAttribute(NpcID.ShopCollection)]
        //public static void ShopCollection(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog data = new Dialog(client, stream);

        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                client.MyExchangeShop.Open(stream, 3303060, 254);

        //                break;
        //            }


        //    }

        //}
        #region EpicArcherQuest
        [NpcAttribute(NpcID.MoChu)]
        public static void MoChu(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {
                        data.Text("Mo Chu:\n");
                        data.Text("The running of the device requires a lot of energy. Follow the order and infuse energy into those statues to make the device run again.\n");
                        data.Text("The energy infusing costs <tip color=0xff76EE00>27 CPs</tip> each time. When the energy gets to a certain level, infuse again and you`ll have a chance to make the statue`s energy fully infused. Come to me and claim <tip color=0xff76EE00>your rewards</tip> each time you activate a statue!\n");
                        data.Option("Find Statues.", 1);
                        data.Option("Leave.", 2);
                        data.AddAvatar(93).FinalizeDialog();
                        break;
                    }
                case 1:
                    {

                        client.Player.QuestGUI.SendAutoPatcher(client.Player.Map, 47, 58, (uint)NpcID.UniversalLoveStatue);
                        break;
                    }
                case 2:
                    {
                        client.Teleport(73, 72, 10451);
                        break;
                    }
            }

        }

        [NpcAttribute(NpcID.UniversalLoveStatue)]
        public static void UniversalLoveStatue(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            System.Random FoxConquer = new System.Random();
            uint Points = (uint)FoxConquer.Next(1, 10);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.EnergyPoints < 100)
                        {
                            data.Text("Universal Love Statue:\n");
                            data.Text("********************Universal Love Statue**************************\n");
                            data.Text("Energy Points " + client.Player.EnergyPoints + " / 100\n");
                            data.Text("********************************************************\n");
                            data.Text("The energy infusing costs <tip color=0xff76EE00>27 CPs</tip> each time. While infusing, it`s <tip color=0xff76EE00>extremely possible</tip> to get the <tip color=0xff76EE00>energy boosted</tip>, helping you activate the statue faster.\n");
                            data.Text("After reaching <tip color=0xff76EE00>80</tip> Energy Points, you may get the chance to have it <tip color=0xff76EE00>fully infused</tip>.\n");

                            data.Option("Infuse energy. (27 CPs)", 1);
                            data.Option("Infuse energy. (3 White/Red Flowers)", 2);
                            data.Option("Infuse 10 times. (270 CPs).", 3);
                            data.AddAvatar(3143).FinalizeDialog();

                        }
                        else if (client.Player.EnergyPoints >= 100)
                        {
                            data.Text("Universal Love Statue:\n");
                            data.Text("********************Universal Love Statue**************************\n");

                            data.Text("Energy Points " + client.Player.EnergyPoints + " / 100\n");

                            data.Text("********************************************************\n");

                            data.Text("STR_ID_tArcherEpicWeapon[28684][Text117]@@\n");
                            data.Option("Got it.", 5);
                            data.AddAvatar(3143).FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        if (client.Player.ConquerPoints >= 27)
                        {
                            client.Player.ConquerPoints -= 27;

                            if (client.Player.EnergyPoints < 100)
                            {
                                client.Player.EnergyPoints += Points;
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, id, true, "zf2-e233");

                                if (client.Player.EnergyPoints >= 100)
                                {
                                    client.Inventory.Add(stream, 3324995);
                                    client.Player.EnergyPoints = 100;
                                    client.Player.stage = 2;
                                    data.Text("Universal Love Statue:\n");
                                    data.Text("********************Universal Love Statue**************************\n");

                                    data.Text("Energy Points " + client.Player.EnergyPoints + " / 100\n");

                                    data.Text("********************************************************\n");
                                    data.Text("STR_ID_tArcherEpicWeapon[28684][Text418]@@\n");
                                    data.Option("Awesome!", 5);
                                    data.AddAvatar(3143).FinalizeDialog();
                                    MsgServer.MsgGameItem item = new MsgServer.MsgGameItem();
                                    item.Color = (Role.Flags.Color)2;
                                    item.ITEM_ID = 284;
                                    MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(item, 56, 56, MsgFloorItem.MsgItem.ItemType.Effect, 0, 0, client.Player.Map
                                              , 0, false, client.Map, 60 * 60 * 24 * 30);

                                    if (client.Map.EnqueueItem(DropItem))
                                        DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Effect);

                                    client.CreateBoxDialog("STR_ID_tArcherEpicWeapon[Content][28684]@@");
                                    return;

                                }

                                data.Text("Universal Love Statue:\n");
                                data.Text("********************Universal Love Statue**************************\n");

                                data.Text("Energy Points " + client.Player.EnergyPoints + " / 100\n");

                                data.Text("********************************************************\n");
                                data.Text("STR_ID_tArcherEpicWeapon[28684][Text416]@@");
                                data.Option("Infuse energy. (27 CPs)", 1);
                                data.Option("Infuse 10 times. (270 CPs).", 3);
                                data.AddAvatar(3143).FinalizeDialog();

                            }
                            else
                            {
                                data.Text("Universal Love Statue:\n");
                                data.Text("********************Universal Love Statue**************************\n");

                                data.Text("Energy Points " + client.Player.EnergyPoints + " / 100\n");

                                data.Text("********************************************************\n");
                                data.Text("STR_ID_tArcherEpicWeapon[28684][Text418]@@\n");
                                data.Option("Awesome!", 5);
                                data.AddAvatar(3143).FinalizeDialog();
                                MsgServer.MsgGameItem item = new MsgServer.MsgGameItem();
                                item.Color = (Role.Flags.Color)2;
                                item.ITEM_ID = 284;
                                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(item, 56, 56, MsgFloorItem.MsgItem.ItemType.Effect, 0, 0, client.Player.Map
                                      , 0, false, client.Map, 60 * 60 * 24 * 30);

                                if (client.Map.EnqueueItem(DropItem))
                                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Effect);


                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("STR_ID_tArcherEpicWeapon[NoEmoney]@@");

                        }
                        break;

                    }
                case 2:
                    {
                        if (client.Inventory.Contain(722732, 3) && client.Inventory.Contain(722736, 3) || client.Inventory.Contain(722732, 3, 1) && client.Inventory.Contain(722736, 3, 1))
                        {
                            client.Inventory.RemoveStackItem(722736, 3, stream);
                            client.Inventory.RemoveStackItem(722732, 3, stream);


                            if (client.Player.EnergyPoints < 100)
                            {
                                client.Player.EnergyPoints += Points;
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, id, true, "zf2-e233");

                                if (client.Player.EnergyPoints >= 100)
                                {
                                    client.Inventory.Add(stream, 3324995);
                                    client.Player.EnergyPoints = 100;
                                    client.Player.stage = 2;
                                    data.Text("Universal Love Statue:\n");
                                    data.Text("********************Universal Love Statue**************************\n");

                                    data.Text("Energy Points " + client.Player.EnergyPoints + " / 100\n");

                                    data.Text("********************************************************\n");
                                    data.Text("STR_ID_tArcherEpicWeapon[28684][Text418]@@\n");
                                    data.Option("Awesome!", 5);
                                    data.AddAvatar(3143).FinalizeDialog();
                                    MsgServer.MsgGameItem item = new MsgServer.MsgGameItem();
                                    item.Color = (Role.Flags.Color)2;
                                    item.ITEM_ID = 284;
                                    MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(item, 56, 56, MsgFloorItem.MsgItem.ItemType.Effect, 0, 0, client.Player.Map
                                       , 0, false, client.Map, 60 * 60 * 24 * 30);

                                    if (client.Map.EnqueueItem(DropItem))
                                        DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Effect);

                                    client.CreateBoxDialog("STR_ID_tArcherEpicWeapon[Content][28684]@@");
                                    return;

                                }

                                data.Text("Universal Love Statue:\n");
                                data.Text("********************Universal Love Statue**************************\n");

                                data.Text("Energy Points " + client.Player.EnergyPoints + " / 100\n");

                                data.Text("********************************************************\n");
                                data.Text("STR_ID_tArcherEpicWeapon[28684][Text416]@@");
                                data.Option("Infuse energy. (3 White/Red Flowers)", 2);
                                data.AddAvatar(3143).FinalizeDialog();

                            }
                            else
                            {
                                data.Text("Universal Love Statue:\n");
                                data.Text("********************Universal Love Statue**************************\n");

                                data.Text("Energy Points " + client.Player.EnergyPoints + " / 100\n");

                                data.Text("********************************************************\n");
                                data.Text("STR_ID_tArcherEpicWeapon[28684][Text418]@@\n");
                                data.Option("Awesome!", 5);
                                data.AddAvatar(3143).FinalizeDialog();
                                MsgServer.MsgGameItem item = new MsgServer.MsgGameItem();
                                item.Color = (Role.Flags.Color)2;
                                item.ITEM_ID = 284;
                                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(item, 56, 56, MsgFloorItem.MsgItem.ItemType.Effect, 0, 0, client.Player.Map
                                        , 0, false, client.Map, 60 * 60 * 24 * 30);

                                if (client.Map.EnqueueItem(DropItem))
                                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Effect);


                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("STR_ID_tArcherEpicWeapon[28541][NoFree]@@");

                        }
                        break;

                    }
                case 3:
                    {
                        if (client.Player.ConquerPoints >= 270)
                        {
                            client.Player.ConquerPoints -= 270;

                            if (client.Player.EnergyPoints < 100)
                            {

                                for (int x = 0; x < 10; x++)
                                {
                                    client.Player.EnergyPoints += Points;
                                }

                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, id, true, "zf2-e233");
                                if (client.Player.EnergyPoints >= 100)
                                {
                                    client.Player.EnergyPoints = 100;
                                    client.Inventory.Add(stream, 3324995);
                                    client.Player.stage = 2;
                                    data.Text("Universal Love Statue:\n");
                                    data.Text("********************Universal Love Statue**************************\n");

                                    data.Text("Energy Points " + client.Player.EnergyPoints + " / 100\n");

                                    data.Text("********************************************************\n");
                                    data.Text("STR_ID_tArcherEpicWeapon[28684][Text418]@@\n");
                                    data.Option("Awesome!", 5);
                                    data.AddAvatar(3143).FinalizeDialog();
                                    MsgServer.MsgGameItem item = new MsgServer.MsgGameItem();
                                    item.Color = (Role.Flags.Color)2;
                                    item.ITEM_ID = 284;
                                    MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(item, 56, 56, MsgFloorItem.MsgItem.ItemType.Effect, 0, 0, client.Player.Map
                                           , 0, false, client.Map, 60 * 60 * 24 * 30);

                                    if (client.Map.EnqueueItem(DropItem))
                                        DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Effect);

                                    client.CreateBoxDialog("STR_ID_tArcherEpicWeapon[Content][28684]@@");
                                    return;
                                }
                                data.Text("Universal Love Statue:\n");
                                data.Text("********************Universal Love Statue**************************\n");

                                data.Text("Energy Points " + client.Player.EnergyPoints + " / 100\n");

                                data.Text("********************************************************\n");
                                data.Text("STR_ID_tArcherEpicWeapon[28684][Text416]@@");
                                data.Option("Infuse energy. (27 CPs)", 1);
                                data.Option("Infuse 10 times. (270 CPs).", 3);
                                data.AddAvatar(3143).FinalizeDialog();

                            }
                            else
                            {
                                data.Text("Universal Love Statue:\n");
                                data.Text("********************Universal Love Statue**************************\n");
                                data.Text("Energy Points " + client.Player.EnergyPoints + " / 100\n");
                                data.Text("********************************************************\n");
                                data.Text("STR_ID_tArcherEpicWeapon[28684][Text418]@@\n");
                                data.Option("Awesome!", 5);
                                data.AddAvatar(3143).FinalizeDialog();
                                MsgServer.MsgGameItem item = new MsgServer.MsgGameItem();
                                item.Color = (Role.Flags.Color)2;
                                item.ITEM_ID = 284;
                                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(item, 56, 56, MsgFloorItem.MsgItem.ItemType.Effect, 0, 0, client.Player.Map
                                               , 0, false, client.Map, 60 * 60 * 24 * 30);

                                if (client.Map.EnqueueItem(DropItem))
                                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Effect);


                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("STR_ID_tArcherEpicWeapon[NoEmoney]@@");
                        }
                        break;
                    }
                case 5:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(client.Player.Map, 59, 47, (uint)NpcID.TalentStatue);
                        break;
                    }

            }

        }

        [NpcAttribute(NpcID.TalentStatue)]//stage2 //150
        public static void TalentStatue(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            System.Random FoxConquer = new System.Random();
            uint Points = (uint)FoxConquer.Next(1, 10);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.stage == 2)
                        {
                            if (client.Player.EnergyPointsTalent < 150)
                            {
                                data.Text("Talent Statue:\n");
                                data.Text("********************Talent Statue**************************\n");
                                data.Text("Energy Points " + client.Player.EnergyPointsTalent + " / 150\n");
                                data.Text("********************************************************\n");
                                data.Text("The energy infusing costs <tip color=0xff76EE00>27 CPs</tip> each time. While infusing, it`s <tip color=0xff76EE00>extremely possible</tip> to get the <tip color=0xff76EE00>energy boosted</tip>, helping you activate the statue faster.\n");
                                data.Text("After reaching <tip color=0xff76EE00>100</tip> Energy Points, you may get the chance to have it <tip color=0xff76EE00>fully infused</tip>.\n");

                                data.Option("Infuse energy. (27 CPs)", 1);
                                data.Option("Infuse energy. (3 White/Red Flowers)", 2);
                                data.Option("Infuse 10 times. (270 CPs).", 3);
                                data.AddAvatar(3143).FinalizeDialog();

                            }
                            else
                            {
                                data.Text("Talent Statue:\n");
                                data.Text("********************Talent Statue**************************\n");
                                data.Text("Energy Points " + client.Player.EnergyPointsTalent + " / 150\n");

                                data.Text("********************************************************\n");

                                data.Text("STR_ID_tArcherEpicWeapon[28684][Text117]@@\n");
                                data.Option("Got it.", 5);
                                data.AddAvatar(3143).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.Text("Talent Statue:\n");
                            data.Text("STR_ID_tArcherEpicWeapon[28684][Text118]@@\n");
                            data.Option("Got it.", 5);
                            data.AddAvatar(3143).FinalizeDialog();
                        }
                        break;

                    }
                case 1:
                    {
                        if (client.Player.ConquerPoints >= 27)
                        {
                            client.Player.ConquerPoints -= 27;
                            if (client.Player.EnergyPointsTalent < 150)
                            {
                                client.Player.EnergyPointsTalent += Points;
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, id, true, "zf2-e233");

                                if (client.Player.EnergyPointsTalent >= 150)
                                {
                                    client.Player.EnergyPointsTalent = 150;
                                    client.Inventory.Add(stream, 3324996);
                                    client.Player.stage = 3;
                                    data.Text("Talent Statue:\n");
                                    data.Text("********************Talent Statue**************************\n");

                                    data.Text("Energy Points " + client.Player.EnergyPointsTalent + " / 150\n");

                                    data.Text("********************************************************\n");
                                    data.Text("STR_ID_tArcherEpicWeapon[28684][Text418]@@\n");
                                    data.Option("Awesome!", 5);
                                    data.AddAvatar(3143).FinalizeDialog();
                                    if (client.Map.ContainItem(286) == 0)
                                    {
                                        Game.MsgServer.MsgGameItem item = new Game.MsgServer.MsgGameItem();
                                        item.Color = (Role.Flags.Color)2;
                                        item.ITEM_ID = 286;
                                        Game.MsgFloorItem.MsgItem DropItem = new Game.MsgFloorItem.MsgItem(item, 56, 56, Game.MsgFloorItem.MsgItem.ItemType.Effect, 0, 0, client.Player.Map
                                                       , 0, false, client.Map, 60 * 60 * 24 * 30);

                                        if (client.Map.EnqueueItem(DropItem))
                                            DropItem.SendAll(stream, Game.MsgFloorItem.MsgDropID.Effect);
                                    }
                                    client.CreateBoxDialog("STR_ID_tArcherEpicWeapon[Content][28686]@@");
                                    return;

                                }
                                data.Text("Talent Statue:\n");
                                data.Text("********************Talent Statue**************************\n");

                                data.Text("Energy Points " + client.Player.EnergyPointsTalent + " / 150\n");

                                data.Text("********************************************************\n");
                                data.Text("STR_ID_tArcherEpicWeapon[28684][Text416]@@");
                                data.Option("Infuse energy. (27 CPs)", 1);
                                data.Option("Infuse 10 times. (270 CPs).", 3);
                                data.AddAvatar(3143).FinalizeDialog();

                            }
                            else
                            {
                                data.Text("Talent Statue:\n");
                                data.Text("********************Talent Statue**************************\n");

                                data.Text("Energy Points " + client.Player.EnergyPointsTalent + " / 150\n");

                                data.Text("********************************************************\n");
                                data.Text("STR_ID_tArcherEpicWeapon[28684][Text418]@@\n");
                                data.Option("Awesome!", 5);
                                data.AddAvatar(3143).FinalizeDialog();
                                if (client.Map.ContainItem(286) == 0)
                                {
                                    Game.MsgServer.MsgGameItem item = new Game.MsgServer.MsgGameItem();
                                    item.Color = (Role.Flags.Color)2;
                                    item.ITEM_ID = 286;
                                    Game.MsgFloorItem.MsgItem DropItem = new Game.MsgFloorItem.MsgItem(item, 56, 56, Game.MsgFloorItem.MsgItem.ItemType.Effect, 0, 0, client.Player.Map
                                                   , 0, false, client.Map, 60 * 60 * 24 * 30);

                                    if (client.Map.EnqueueItem(DropItem))
                                        DropItem.SendAll(stream, Game.MsgFloorItem.MsgDropID.Effect);
                                }

                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("STR_ID_tArcherEpicWeapon[NoEmoney]@@");
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(722732, 3) && client.Inventory.Contain(722736, 3) || client.Inventory.Contain(722732, 3, 1) && client.Inventory.Contain(722736, 3, 1))
                        {
                            client.Inventory.RemoveStackItem(722736, 3, stream);
                            client.Inventory.RemoveStackItem(722732, 3, stream);

                            if (client.Player.EnergyPointsTalent < 150)
                            {
                                client.Player.EnergyPointsTalent += Points;
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, id, true, "zf2-e233");

                                if (client.Player.EnergyPointsTalent >= 150)
                                {
                                    client.Player.EnergyPointsTalent = 150;
                                    client.Inventory.Add(stream, 3324996);
                                    client.Player.stage = 3;
                                    data.Text("Talent Statue:\n");
                                    data.Text("********************Talent Statue**************************\n");

                                    data.Text("Energy Points " + client.Player.EnergyPointsTalent + " / 150\n");

                                    data.Text("********************************************************\n");
                                    data.Text("STR_ID_tArcherEpicWeapon[28684][Text418]@@\n");
                                    data.Option("Awesome!", 5);
                                    data.AddAvatar(3143).FinalizeDialog();
                                    if (client.Map.ContainItem(286) == 0)
                                    {
                                        Game.MsgServer.MsgGameItem item = new Game.MsgServer.MsgGameItem();
                                        item.Color = (Role.Flags.Color)2;
                                        item.ITEM_ID = 286;
                                        Game.MsgFloorItem.MsgItem DropItem = new Game.MsgFloorItem.MsgItem(item, 56, 56, Game.MsgFloorItem.MsgItem.ItemType.Effect, 0, 0, client.Player.Map
                                                       , 0, false, client.Map, 60 * 60 * 24 * 30);

                                        if (client.Map.EnqueueItem(DropItem))
                                            DropItem.SendAll(stream, Game.MsgFloorItem.MsgDropID.Effect);
                                    }
                                    client.CreateBoxDialog("STR_ID_tArcherEpicWeapon[Content][28686]@@");
                                    return;

                                }
                                data.Text("Talent Statue:\n");
                                data.Text("********************Talent Statue**************************\n");

                                data.Text("Energy Points " + client.Player.EnergyPointsTalent + " / 150\n");

                                data.Text("********************************************************\n");
                                data.Text("STR_ID_tArcherEpicWeapon[28684][Text416]@@");
                                data.Option("Infuse energy. (3 White/Red Flowers)", 2);
                                data.AddAvatar(3143).FinalizeDialog();

                            }
                            else
                            {
                                data.Text("Talent Statue:\n");
                                data.Text("********************Talent Statue**************************\n");

                                data.Text("Energy Points " + client.Player.EnergyPointsTalent + " / 150\n");

                                data.Text("********************************************************\n");
                                data.Text("STR_ID_tArcherEpicWeapon[28684][Text418]@@\n");
                                data.Option("Awesome!", 5);
                                data.AddAvatar(3143).FinalizeDialog();
                                if (client.Map.ContainItem(286) == 0)
                                {
                                    Game.MsgServer.MsgGameItem item = new Game.MsgServer.MsgGameItem();
                                    item.Color = (Role.Flags.Color)2;
                                    item.ITEM_ID = 286;
                                    Game.MsgFloorItem.MsgItem DropItem = new Game.MsgFloorItem.MsgItem(item, 56, 56, Game.MsgFloorItem.MsgItem.ItemType.Effect, 0, 0, client.Player.Map
                                                   , 0, false, client.Map, 60 * 60 * 24 * 30);

                                    if (client.Map.EnqueueItem(DropItem))
                                        DropItem.SendAll(stream, Game.MsgFloorItem.MsgDropID.Effect);
                                }

                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("STR_ID_tArcherEpicWeapon[28541][NoFree]@@");
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Player.ConquerPoints >= 270)
                        {
                            client.Player.ConquerPoints -= 270;
                            if (client.Player.EnergyPointsTalent < 150)
                            {
                                for (int x = 0; x < 10; x++)
                                {
                                    client.Player.EnergyPointsTalent += Points;
                                }
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, id, true, "zf2-e233");
                                if (client.Player.EnergyPointsTalent >= 150)
                                {
                                    client.Player.EnergyPointsTalent = 150;
                                    client.Inventory.Add(stream, 3324996);
                                    client.Player.stage = 3;
                                    data.Text("Talent Statue:\n");
                                    data.Text("********************Talent Statue**************************\n");

                                    data.Text("Energy Points " + client.Player.EnergyPointsTalent + " / 150\n");

                                    data.Text("********************************************************\n");
                                    data.Text("STR_ID_tArcherEpicWeapon[28684][Text418]@@\n");
                                    data.Option("Awesome!", 5);
                                    data.AddAvatar(3143).FinalizeDialog();
                                    if (client.Map.ContainItem(286) == 0)
                                    {
                                        Game.MsgServer.MsgGameItem item = new Game.MsgServer.MsgGameItem();
                                        item.Color = (Role.Flags.Color)2;
                                        item.ITEM_ID = 286;
                                        Game.MsgFloorItem.MsgItem DropItem = new Game.MsgFloorItem.MsgItem(item, 56, 56, Game.MsgFloorItem.MsgItem.ItemType.Effect, 0, 0, client.Player.Map
                                                       , 0, false, client.Map, 60 * 60 * 24 * 30);

                                        if (client.Map.EnqueueItem(DropItem))
                                            DropItem.SendAll(stream, Game.MsgFloorItem.MsgDropID.Effect);
                                    }
                                    client.CreateBoxDialog("STR_ID_tArcherEpicWeapon[Content][28686]@@");
                                    return;
                                }
                                data.Text("Talent Statue:\n");
                                data.Text("********************Talent Statue**************************\n");

                                data.Text("Energy Points " + client.Player.EnergyPointsTalent + " / 150\n");

                                data.Text("********************************************************\n");
                                data.Text("STR_ID_tArcherEpicWeapon[28684][Text416]@@");
                                data.Option("Infuse energy. (27 CPs)", 1);
                                data.Option("Infuse 10 times. (270 CPs).", 3);
                                data.AddAvatar(3143).FinalizeDialog();

                            }
                            else
                            {
                                data.Text("Talent Statue:\n");
                                data.Text("********************Talent Statue**************************\n");
                                data.Text("Energy Points " + client.Player.EnergyPointsTalent + " / 150\n");
                                data.Text("********************************************************\n");
                                data.Text("STR_ID_tArcherEpicWeapon[28684][Text418]@@\n");
                                data.Option("Awesome!", 5);
                                data.AddAvatar(3143).FinalizeDialog();
                                if (client.Map.ContainItem(286) == 0)
                                {
                                    Game.MsgServer.MsgGameItem item = new Game.MsgServer.MsgGameItem();
                                    item.Color = (Role.Flags.Color)2;
                                    item.ITEM_ID = 286;
                                    Game.MsgFloorItem.MsgItem DropItem = new Game.MsgFloorItem.MsgItem(item, 56, 56, Game.MsgFloorItem.MsgItem.ItemType.Effect, 0, 0, client.Player.Map
                                                   , 0, false, client.Map, 60 * 60 * 24 * 30);

                                    if (client.Map.EnqueueItem(DropItem))
                                        DropItem.SendAll(stream, Game.MsgFloorItem.MsgDropID.Effect);
                                }

                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("STR_ID_tArcherEpicWeapon[NoEmoney]@@");
                        }
                        break;

                    }
                case 5:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(client.Player.Map, 58, 72, (uint)NpcID.AntiwarStatue);
                        break;
                    }

            }

        }

        [NpcAttribute(NpcID.AntiwarStatue)]//stage 3 //200 
        public static void AntiwarStatue(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            System.Random FoxConquer = new System.Random();
            uint Points = (uint)FoxConquer.Next(1, 10);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.stage == 3)
                        {
                            if (client.Player.EnergyPointsAntiwar < 200)
                            {
                                data.Text("AntiWar Statue:\n");
                                data.Text("********************AntiWar Statue**************************\n");
                                data.Text("Energy Points " + client.Player.EnergyPointsAntiwar + " / 200\n");
                                data.Text("********************************************************\n");
                                data.Text("The energy infusing costs <tip color=0xff76EE00>27 CPs</tip> each time. While infusing, it`s <tip color=0xff76EE00>extremely possible</tip> to get the <tip color=0xff76EE00>energy boosted</tip>, helping you activate the statue faster.\n");
                                data.Text("After reaching <tip color=0xff76EE00>120</tip> Energy Points, you may get the chance to have it <tip color=0xff76EE00>fully infused</tip>.\n");

                                data.Option("Infuse energy. (27 CPs)", 1);
                                data.Option("Infuse energy. (3 White/Red Flowers)", 2);
                                data.Option("Infuse 10 times. (270 CPs).", 3);
                                data.AddAvatar(3143).FinalizeDialog();

                            }
                            else
                            {
                                data.Text("AntiWar Statue:\n");
                                data.Text("********************AntiWar Statue**************************\n");
                                data.Text("Energy Points " + client.Player.EnergyPointsAntiwar + " / 200\n");

                                data.Text("********************************************************\n");

                                data.Text("STR_ID_tArcherEpicWeapon[28684][Text117]@@\n");
                                data.Option("Got it.", 5);
                                data.AddAvatar(3143).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.Text("AntiWar Statue:\n");
                            data.Text("STR_ID_tArcherEpicWeapon[28684][Text118]@@\n");
                            data.Option("Got it.", 5);
                            data.AddAvatar(3143).FinalizeDialog();
                        }
                        break;

                    }
                case 1:
                    {
                        if (client.Player.ConquerPoints >= 27)
                        {
                            client.Player.ConquerPoints -= 27;
                            if (client.Player.EnergyPointsAntiwar < 200)
                            {
                                client.Player.EnergyPointsAntiwar += Points;
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, id, true, "zf2-e233");

                                if (client.Player.EnergyPointsAntiwar >= 200)
                                {
                                    client.Player.EnergyPointsAntiwar = 200;
                                    client.Inventory.Add(stream, 3324997);
                                    client.Player.stage = 4;
                                    data.Text("AntiWar Statue:\n");
                                    data.Text("********************AntiWar Statue**************************\n");

                                    data.Text("Energy Points " + client.Player.EnergyPointsAntiwar + " / 200\n");

                                    data.Text("********************************************************\n");
                                    data.Text("STR_ID_tArcherEpicWeapon[28684][Text418]@@\n");
                                    data.Option("Awesome!", 5);
                                    data.AddAvatar(3143).FinalizeDialog();
                                    if (client.Map.ContainItem(285) == 0)
                                    {
                                        Game.MsgServer.MsgGameItem item = new Game.MsgServer.MsgGameItem();
                                        item.Color = (Role.Flags.Color)2;
                                        item.ITEM_ID = 285;
                                        Game.MsgFloorItem.MsgItem DropItem = new Game.MsgFloorItem.MsgItem(item, 56, 56, Game.MsgFloorItem.MsgItem.ItemType.Effect, 0, 0, client.Player.Map
                                                       , 0, false, client.Map, 60 * 60 * 24 * 30);

                                        if (client.Map.EnqueueItem(DropItem))
                                            DropItem.SendAll(stream, Game.MsgFloorItem.MsgDropID.Effect);
                                    }
                                    client.CreateBoxDialog("STR_ID_tArcherEpicWeapon[Content][28685]@@");
                                    return;

                                }
                                data.Text("AntiWar Statue:\n");
                                data.Text("********************AntiWar Statue**************************\n");

                                data.Text("Energy Points " + client.Player.EnergyPointsAntiwar + " / 200\n");

                                data.Text("********************************************************\n");
                                data.Text("STR_ID_tArcherEpicWeapon[28684][Text416]@@");
                                data.Option("Infuse energy. (27 CPs)", 1);
                                data.Option("Infuse 10 times. (270 CPs).", 3);
                                data.AddAvatar(3143).FinalizeDialog();

                            }
                            else
                            {
                                data.Text("AntiWar Statue:\n");
                                data.Text("********************AntiWar Statue**************************\n");

                                data.Text("Energy Points " + client.Player.EnergyPointsAntiwar + " / 200\n");

                                data.Text("********************************************************\n");
                                data.Text("STR_ID_tArcherEpicWeapon[28684][Text418]@@\n");
                                data.Option("Awesome!", 5);
                                data.AddAvatar(3143).FinalizeDialog();
                                if (client.Map.ContainItem(285) == 0)
                                {
                                    Game.MsgServer.MsgGameItem item = new Game.MsgServer.MsgGameItem();
                                    item.Color = (Role.Flags.Color)2;
                                    item.ITEM_ID = 285;
                                    Game.MsgFloorItem.MsgItem DropItem = new Game.MsgFloorItem.MsgItem(item, 56, 56, Game.MsgFloorItem.MsgItem.ItemType.Effect, 0, 0, client.Player.Map
                                                   , 0, false, client.Map, 60 * 60 * 24 * 30);

                                    if (client.Map.EnqueueItem(DropItem))
                                        DropItem.SendAll(stream, Game.MsgFloorItem.MsgDropID.Effect);
                                }
                                break;

                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("STR_ID_tArcherEpicWeapon[NoEmoney]@@");
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(722732, 3) && client.Inventory.Contain(722736, 3) || client.Inventory.Contain(722732, 3, 1) && client.Inventory.Contain(722736, 3, 1))
                        {
                            client.Inventory.RemoveStackItem(722736, 3, stream);
                            client.Inventory.RemoveStackItem(722732, 3, stream);

                            if (client.Player.EnergyPointsAntiwar < 200)
                            {
                                client.Player.EnergyPointsAntiwar += Points;
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, id, true, "zf2-e233");

                                if (client.Player.EnergyPointsAntiwar >= 200)
                                {
                                    client.Player.EnergyPointsAntiwar = 200;
                                    client.Inventory.Add(stream, 3324997);
                                    client.Player.stage = 4;
                                    data.Text("AntiWar Statue:\n");
                                    data.Text("********************AntiWar Statue**************************\n");

                                    data.Text("Energy Points " + client.Player.EnergyPointsAntiwar + " / 200\n");

                                    data.Text("********************************************************\n");
                                    data.Text("STR_ID_tArcherEpicWeapon[28684][Text418]@@\n");
                                    data.Option("Awesome!", 5);
                                    data.AddAvatar(3143).FinalizeDialog();
                                    if (client.Map.ContainItem(285) == 0)
                                    {
                                        Game.MsgServer.MsgGameItem item = new Game.MsgServer.MsgGameItem();
                                        item.Color = (Role.Flags.Color)2;
                                        item.ITEM_ID = 285;
                                        Game.MsgFloorItem.MsgItem DropItem = new Game.MsgFloorItem.MsgItem(item, 56, 56, Game.MsgFloorItem.MsgItem.ItemType.Effect, 0, 0, client.Player.Map
                                                       , 0, false, client.Map, 60 * 60 * 24 * 30);

                                        if (client.Map.EnqueueItem(DropItem))
                                            DropItem.SendAll(stream, Game.MsgFloorItem.MsgDropID.Effect);
                                    }
                                    client.CreateBoxDialog("STR_ID_tArcherEpicWeapon[Content][28685]@@");
                                    return;

                                }
                                data.Text("AntiWar Statue:\n");
                                data.Text("********************AntiWar Statue**************************\n");

                                data.Text("Energy Points " + client.Player.EnergyPointsAntiwar + " / 200\n");

                                data.Text("********************************************************\n");
                                data.Text("STR_ID_tArcherEpicWeapon[28684][Text416]@@");
                                data.Option("Infuse energy. (27 CPs)", 1);
                                data.Option("Infuse energy. (3 White/Red Flowers)", 2);
                                data.Option("Infuse 10 times. (270 CPs).", 3);
                                data.AddAvatar(3143).FinalizeDialog();

                            }
                            else
                            {
                                data.Text("AntiWar Statue:\n");
                                data.Text("********************AntiWar Statue**************************\n");

                                data.Text("Energy Points " + client.Player.EnergyPointsAntiwar + " / 200\n");

                                data.Text("********************************************************\n");
                                data.Text("STR_ID_tArcherEpicWeapon[28684][Text418]@@\n");
                                data.Option("Awesome!", 5);
                                data.AddAvatar(3143).FinalizeDialog();

                                if (client.Map.ContainItem(285) == 0)
                                {
                                    Game.MsgServer.MsgGameItem item = new Game.MsgServer.MsgGameItem();
                                    item.Color = (Role.Flags.Color)2;
                                    item.ITEM_ID = 285;
                                    Game.MsgFloorItem.MsgItem DropItem = new Game.MsgFloorItem.MsgItem(item, 56, 56, Game.MsgFloorItem.MsgItem.ItemType.Effect, 0, 0, client.Player.Map
                                                   , 0, false, client.Map, 60 * 60 * 24 * 30);

                                    if (client.Map.EnqueueItem(DropItem))
                                        DropItem.SendAll(stream, Game.MsgFloorItem.MsgDropID.Effect);
                                }
                                break;

                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("STR_ID_tArcherEpicWeapon[28541][NoFree]@@");
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Player.ConquerPoints >= 270)
                        {
                            client.Player.ConquerPoints -= 270;
                            if (client.Player.EnergyPointsAntiwar < 200)
                            {
                                for (int x = 0; x < 10; x++)
                                {
                                    client.Player.EnergyPointsAntiwar += Points;
                                }

                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, id, true, "zf2-e233");
                                if (client.Player.EnergyPointsAntiwar >= 200)
                                {
                                    client.Player.EnergyPointsAntiwar = 200;
                                    client.Inventory.Add(stream, 3324997);
                                    client.Player.stage = 4;
                                    data.Text("AntiWar Statue:\n");
                                    data.Text("********************AntiWar Statue**************************\n");

                                    data.Text("Energy Points " + client.Player.EnergyPointsAntiwar + " / 200\n");

                                    data.Text("********************************************************\n");
                                    data.Text("STR_ID_tArcherEpicWeapon[28684][Text418]@@\n");
                                    data.Option("Awesome!", 5);
                                    data.AddAvatar(3143).FinalizeDialog();
                                    if (client.Map.ContainItem(285) == 0)
                                    {
                                        Game.MsgServer.MsgGameItem item = new Game.MsgServer.MsgGameItem();
                                        item.Color = (Role.Flags.Color)2;
                                        item.ITEM_ID = 285;
                                        Game.MsgFloorItem.MsgItem DropItem = new Game.MsgFloorItem.MsgItem(item, 56, 56, Game.MsgFloorItem.MsgItem.ItemType.Effect, 0, 0, client.Player.Map
                                                       , 0, false, client.Map, 60 * 60 * 24 * 30);

                                        if (client.Map.EnqueueItem(DropItem))
                                            DropItem.SendAll(stream, Game.MsgFloorItem.MsgDropID.Effect);
                                    }
                                    client.CreateBoxDialog("STR_ID_tArcherEpicWeapon[Content][28685]@@");
                                    return;
                                }
                                data.Text("AntiWar Statue:\n");
                                data.Text("********************AntiWar Statue**************************\n");

                                data.Text("Energy Points " + client.Player.EnergyPointsAntiwar + " / 200\n");

                                data.Text("********************************************************\n");
                                data.Text("STR_ID_tArcherEpicWeapon[28684][Text416]@@");
                                data.Option("Infuse energy. (27 CPs)", 1);
                                data.Option("Infuse 10 times. (270 CPs).", 3);
                                data.AddAvatar(3143).FinalizeDialog();

                            }
                            else
                            {
                                data.Text("AntiWar Statue:\n");
                                data.Text("********************AntiWar Statue**************************\n");
                                data.Text("Energy Points " + client.Player.EnergyPointsAntiwar + " / 200\n");
                                data.Text("********************************************************\n");
                                data.Text("STR_ID_tArcherEpicWeapon[28684][Text418]@@\n");
                                data.Option("Awesome!", 5);
                                data.AddAvatar(3143).FinalizeDialog();
                                if (client.Map.ContainItem(285) == 0)
                                {
                                    Game.MsgServer.MsgGameItem item = new Game.MsgServer.MsgGameItem();
                                    item.Color = (Role.Flags.Color)2;
                                    item.ITEM_ID = 285;
                                    Game.MsgFloorItem.MsgItem DropItem = new Game.MsgFloorItem.MsgItem(item, 56, 56, Game.MsgFloorItem.MsgItem.ItemType.Effect, 0, 0, client.Player.Map
                                                   , 0, false, client.Map, 60 * 60 * 24 * 30);

                                    if (client.Map.EnqueueItem(DropItem))
                                        DropItem.SendAll(stream, Game.MsgFloorItem.MsgDropID.Effect);
                                }
                                break;

                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("STR_ID_tArcherEpicWeapon[NoEmoney]@@");
                        }
                        break;

                    }
                case 5:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(client.Player.Map, 72, 59, (uint)NpcID.AntifatalismStatue);
                        break;
                    }
            }

        }

        [NpcAttribute(NpcID.AntifatalismStatue)]//stage 4 //300
        public static void AntifatalismStatue(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            System.Random FoxConquer = new System.Random();
            uint Points = (uint)FoxConquer.Next(1, 10);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.stage == 4)
                        {
                            if (client.Player.EnergyPointsAntifatalism < 300)
                            {
                                data.Text("Antifatalism Statue:\n");
                                data.Text("********************Antifatalism Statue**************************\n");
                                data.Text("Energy Points " + client.Player.EnergyPointsAntifatalism + " / 300\n");
                                data.Text("********************************************************\n");
                                data.Text("The energy infusing costs <tip color=0xff76EE00>27 CPs</tip> each time. While infusing, it`s <tip color=0xff76EE00>extremely possible</tip> to get the <tip color=0xff76EE00>energy boosted</tip>, helping you activate the statue faster.\n");
                                data.Text("After reaching <tip color=0xff76EE00>200</tip> Energy Points, you may get the chance to have it <tip color=0xff76EE00>fully infused</tip>.\n");

                                data.Option("Infuse energy. (27 CPs)", 1);
                                data.Option("Infuse energy. (3 White/Red Flowers)", 2);
                                data.Option("Infuse 10 times. (270 CPs).", 3);
                                data.AddAvatar(3143).FinalizeDialog();

                            }
                            else
                            {
                                data.Text("Antifatalism Statue:\n");
                                data.Text("********************Antifatalism Statue**************************\n");
                                data.Text("Energy Points " + client.Player.EnergyPointsAntifatalism + " / 300\n");

                                data.Text("********************************************************\n");

                                data.Text("STR_ID_tArcherEpicWeapon[28684][Text117]@@\n");
                                data.Option("Got it.", 5);
                                data.AddAvatar(3143).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.Text("Antifatalism Statue:\n");
                            data.Text("STR_ID_tArcherEpicWeapon[28684][Text118]@@\n");
                            data.Option("Got it.", 5);
                            data.AddAvatar(3143).FinalizeDialog();
                        }
                        break;

                    }
                case 1:
                    {
                        if (client.Player.ConquerPoints >= 27)
                        {
                            client.Player.ConquerPoints -= 27;
                            if (client.Player.EnergyPointsAntifatalism < 300)
                            {
                                client.Player.EnergyPointsAntifatalism += Points;
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, id, true, "zf2-e233");
                                if (client.Player.EnergyPointsAntifatalism > 300)
                                {
                                    client.Player.EnergyPointsAntifatalism = 300;
                                    client.Inventory.Add(stream, 3324998);
                                    client.Player.Laststage = 1;
                                    data.Text("Antifatalism Statue:\n");
                                    data.Text("********************Antifatalism Statue**************************\n");

                                    data.Text("Energy Points " + client.Player.EnergyPointsAntifatalism + " / 300\n");


                                    data.Text("********************************************************\n");
                                    data.Text("STR_ID_tArcherEpicWeapon[28684][Text418]@@\n");
                                    data.Option("Awesome!", 5);
                                    data.AddAvatar(3143).FinalizeDialog();
                                    if (client.Map.ContainItem(287) == 0)
                                    {
                                        Game.MsgServer.MsgGameItem item = new Game.MsgServer.MsgGameItem();
                                        item.Color = (Role.Flags.Color)2;
                                        item.ITEM_ID = 287;
                                        Game.MsgFloorItem.MsgItem DropItem = new Game.MsgFloorItem.MsgItem(item, 56, 56, Game.MsgFloorItem.MsgItem.ItemType.Effect, 0, 0, client.Player.Map
                                                       , 0, false, client.Map, 60 * 60 * 24 * 30);

                                        if (client.Map.EnqueueItem(DropItem))
                                            DropItem.SendAll(stream, Game.MsgFloorItem.MsgDropID.Effect);
                                    }
                                    client.CreateBoxDialog("STR_ID_tArcherEpicWeapon[Content][28687]@@");
                                    break;

                                }
                                data.Text("Antifatalism Statue:\n");
                                data.Text("********************Antifatalism Statue**************************\n");

                                data.Text("Energy Points " + client.Player.EnergyPointsAntifatalism + " / 300\n");

                                data.Text("********************************************************\n");
                                data.Text("STR_ID_tArcherEpicWeapon[28684][Text416]@@");
                                data.Option("Infuse energy. (27 CPs)", 1);
                                data.Option("Infuse 10 times. (270 CPs).", 3);
                                data.AddAvatar(3143).FinalizeDialog();
                                break;
                            }
                            else
                            {
                                data.Text("Antifatalism Statue:\n");
                                data.Text("********************Antifatalism Statue**************************\n");

                                data.Text("Energy Points " + client.Player.EnergyPointsAntifatalism + " / 300\n");

                                data.Text("********************************************************\n");
                                data.Text("STR_ID_tArcherEpicWeapon[28684][Text418]@@\n");
                                data.Option("Awesome!", 5);
                                data.AddAvatar(3143).FinalizeDialog();
                                if (client.Map.ContainItem(287) == 0)
                                {

                                    Game.MsgServer.MsgGameItem item = new Game.MsgServer.MsgGameItem();
                                    item.Color = (Role.Flags.Color)2;
                                    item.ITEM_ID = 287;
                                    Game.MsgFloorItem.MsgItem DropItem = new Game.MsgFloorItem.MsgItem(item, 56, 56, Game.MsgFloorItem.MsgItem.ItemType.Effect, 0, 0, client.Player.Map
                                                   , 0, false, client.Map, 60 * 60 * 24 * 30);

                                    if (client.Map.EnqueueItem(DropItem))
                                        DropItem.SendAll(stream, Game.MsgFloorItem.MsgDropID.Effect);
                                }
                                break;
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("STR_ID_tArcherEpicWeapon[NoEmoney]@@");
                        }
                        break;
                    }

                case 2:
                    {
                        if (client.Inventory.Contain(722732, 3) && client.Inventory.Contain(722736, 3) || client.Inventory.Contain(722732, 3, 1) && client.Inventory.Contain(722736, 3, 1))
                        {
                            client.Inventory.RemoveStackItem(722736, 3, stream);
                            client.Inventory.RemoveStackItem(722732, 3, stream);

                            if (client.Player.EnergyPointsAntifatalism < 300)
                            {
                                client.Player.EnergyPointsAntifatalism += Points;
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, id, true, "zf2-e233");
                                if (client.Player.EnergyPointsAntifatalism > 300)
                                {
                                    client.Player.EnergyPointsAntifatalism = 300;
                                    client.Inventory.Add(stream, 3324998);
                                    client.Player.Laststage = 1;
                                    data.Text("Antifatalism Statue:\n");
                                    data.Text("********************Antifatalism Statue**************************\n");

                                    data.Text("Energy Points " + client.Player.EnergyPointsAntifatalism + " / 300\n");

                                    data.Text("********************************************************\n");
                                    data.Text("STR_ID_tArcherEpicWeapon[28684][Text418]@@\n");
                                    data.Option("Awesome!", 5);
                                    data.AddAvatar(3143).FinalizeDialog();
                                    if (client.Map.ContainItem(287) == 0)
                                    {

                                        Game.MsgServer.MsgGameItem item = new Game.MsgServer.MsgGameItem();
                                        item.Color = (Role.Flags.Color)2;
                                        item.ITEM_ID = 287;
                                        Game.MsgFloorItem.MsgItem DropItem = new Game.MsgFloorItem.MsgItem(item, 56, 56, Game.MsgFloorItem.MsgItem.ItemType.Effect, 0, 0, client.Player.Map
                                                       , 0, false, client.Map, 60 * 60 * 24 * 30);

                                        if (client.Map.EnqueueItem(DropItem))
                                            DropItem.SendAll(stream, Game.MsgFloorItem.MsgDropID.Effect);
                                    }
                                    client.CreateBoxDialog("STR_ID_tArcherEpicWeapon[Content][28687]@@");
                                    break;

                                }
                                data.Text("Antifatalism Statue:\n");
                                data.Text("********************Antifatalism Statue**************************\n");

                                data.Text("Energy Points " + client.Player.EnergyPointsAntifatalism + " / 300\n");

                                data.Text("********************************************************\n");
                                data.Text("STR_ID_tArcherEpicWeapon[28684][Text416]@@");
                                data.Option("Infuse energy. (3 White/Red Flowers)", 2);
                                data.AddAvatar(3143).FinalizeDialog();
                                break;
                            }
                            else
                            {
                                data.Text("Antifatalism Statue:\n");
                                data.Text("********************Antifatalism Statue**************************\n");

                                data.Text("Energy Points " + client.Player.EnergyPointsAntifatalism + " / 300\n");

                                data.Text("********************************************************\n");
                                data.Text("STR_ID_tArcherEpicWeapon[28684][Text418]@@\n");
                                data.Option("Awesome!", 5);
                                data.AddAvatar(3143).FinalizeDialog();
                                if (client.Map.ContainItem(287) == 0)
                                {

                                    Game.MsgServer.MsgGameItem item = new Game.MsgServer.MsgGameItem();
                                    item.Color = (Role.Flags.Color)2;
                                    item.ITEM_ID = 287;
                                    Game.MsgFloorItem.MsgItem DropItem = new Game.MsgFloorItem.MsgItem(item, 56, 56, Game.MsgFloorItem.MsgItem.ItemType.Effect, 0, 0, client.Player.Map
                                                   , 0, false, client.Map, 60 * 60 * 24 * 30);

                                    if (client.Map.EnqueueItem(DropItem))
                                        DropItem.SendAll(stream, Game.MsgFloorItem.MsgDropID.Effect);
                                }
                                break;
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("STR_ID_tArcherEpicWeapon[28541][NoFree]@@");
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Player.ConquerPoints >= 270)
                        {
                            client.Player.ConquerPoints -= 270;
                            if (client.Player.EnergyPointsAntifatalism < 300)
                            {
                                for (int x = 0; x < 10; x++)
                                {
                                    client.Player.EnergyPointsAntifatalism += Points;
                                }
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, id, true, "zf2-e233");

                                if (client.Player.EnergyPointsAntifatalism >= 300)
                                {
                                    client.Player.EnergyPointsAntifatalism = 300;
                                    client.Inventory.Add(stream, 3324998);
                                    client.Player.Laststage = 1;
                                    data.Text("Antifatalism Statue:\n");
                                    data.Text("********************Antifatalism Statue**************************\n");

                                    data.Text("Energy Points " + client.Player.EnergyPointsAntifatalism + " / 300\n");

                                    data.Text("********************************************************\n");
                                    data.Text("STR_ID_tArcherEpicWeapon[28684][Text418]@@\n");
                                    data.Option("Awesome!", 5);
                                    data.AddAvatar(3143).FinalizeDialog();
                                    if (client.Map.ContainItem(287) == 0)
                                    {

                                        Game.MsgServer.MsgGameItem item = new Game.MsgServer.MsgGameItem();
                                        item.Color = (Role.Flags.Color)2;
                                        item.ITEM_ID = 287;
                                        Game.MsgFloorItem.MsgItem DropItem = new Game.MsgFloorItem.MsgItem(item, 56, 56, Game.MsgFloorItem.MsgItem.ItemType.Effect, 0, 0, client.Player.Map
                                                       , 0, false, client.Map, 60 * 60 * 24 * 30);

                                        if (client.Map.EnqueueItem(DropItem))
                                            DropItem.SendAll(stream, Game.MsgFloorItem.MsgDropID.Effect);
                                    }
                                    client.CreateBoxDialog("STR_ID_tArcherEpicWeapon[Content][28687]@@");
                                    return;
                                }
                                data.Text("Antifatalism Statue:\n");
                                data.Text("********************Antifatalism Statue**************************\n");

                                data.Text("Energy Points " + client.Player.EnergyPointsAntifatalism + " / 300\n");

                                data.Text("********************************************************\n");
                                data.Text("STR_ID_tArcherEpicWeapon[28684][Text416]@@");
                                data.Option("Infuse energy. (27 CPs)", 1);
                                data.Option("Infuse 10 times. (270 CPs).", 3);
                                data.AddAvatar(3143).FinalizeDialog();

                            }
                            else
                            {
                                data.Text("Antifatalism Statue:\n");
                                data.Text("********************Antifatalism Statue**************************\n");
                                data.Text("Energy Points " + client.Player.EnergyPointsAntifatalism + " / 300\n");
                                data.Text("********************************************************\n");
                                data.Text("STR_ID_tArcherEpicWeapon[28684][Text418]@@\n");
                                data.Option("Awesome!", 5);
                                data.AddAvatar(3143).FinalizeDialog();
                                if (client.Map.ContainItem(287) == 0)
                                {

                                    Game.MsgServer.MsgGameItem item = new Game.MsgServer.MsgGameItem();
                                    item.Color = (Role.Flags.Color)2;
                                    item.ITEM_ID = 287;
                                    Game.MsgFloorItem.MsgItem DropItem = new Game.MsgFloorItem.MsgItem(item, 56, 56, Game.MsgFloorItem.MsgItem.ItemType.Effect, 0, 0, client.Player.Map
                                                   , 0, false, client.Map, 60 * 60 * 24 * 30);

                                    if (client.Map.EnqueueItem(DropItem))
                                        DropItem.SendAll(stream, Game.MsgFloorItem.MsgDropID.Effect);
                                }
                                break;

                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("STR_ID_tArcherEpicWeapon[NoEmoney]@@");
                        }
                        break;

                    }
                case 5:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(client.Player.Map, 56, 56, (uint)NpcID.MysticAltar);
                        break;
                    }

            }

        }

        [NpcAttribute(NpcID.MysticAltar)]//LastStage //400 3324521
        public static void MysticAltar(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            System.Random FoxConquer = new System.Random();
            uint Points = (uint)FoxConquer.Next(1, 10);
            switch (Option)
            {
                case 0:
                    {

                        if (client.Player.Laststage == 1)
                        {
                            if (client.Player.EnergyPointsMystic < 400)
                            {
                                data.Text("MysticAltar Statue:\n");
                                data.Text("********************MysticAltar Statue**************************\n");
                                data.Text("Energy Points " + client.Player.EnergyPointsMystic + " / 400\n");
                                data.Text("********************************************************\n");
                                data.Text("The energy infusing costs <tip color=0xff76EE00>27 CPs</tip> each time. While infusing, it`s <tip color=0xff76EE00>extremely possible</tip> to get the <tip color=0xff76EE00>energy boosted</tip>, helping you activate the statue faster.\n");
                                data.Text("After reaching <tip color=0xff76EE00>200</tip> Energy Points, you may get the chance to have it <tip color=0xff76EE00>fully infused</tip>.\n");

                                data.Option("Infuse energy. (27 CPs)", 1);
                                data.Option("Infuse energy. (3 White/Red Flowers)", 2);
                                data.Option("Infuse 10 times. (270 CPs).", 3);
                                data.AddAvatar(3143).FinalizeDialog();
                                break;

                            }
                            else
                            {
                                data.Text("MysticAltar Statue:\n");
                                data.Text("********************MysticAltar Statue**************************\n");
                                data.Text("Energy Points " + client.Player.EnergyPointsMystic + " / 400\n");

                                data.Text("********************************************************\n");

                                data.Text("STR_ID_tArcherEpicWeapon[28684][Text1193]@@\n");
                                data.Text("STR_ID_tArcherEpicWeapon[28684][Text1194]@@\n");
                                data.Option("STR_ID_tArcherEpicWeapon[28684][Option116]@@\n", 5);
                                data.AddAvatar(3143).FinalizeDialog();
                                break;
                            }
                        }
                        else
                        {
                            data.Text("MysticAltar Statue:\n");
                            data.Text("STR_ID_tArcherEpicWeapon[28684][Text1193]@@\n");
                            data.Text("STR_ID_tArcherEpicWeapon[28684][Text1194]@@\n");
                            data.Option("STR_ID_tArcherEpicWeapon[28684][Option116]@@\n", 5);
                            data.AddAvatar(3143).FinalizeDialog();
                        }

                        break;

                    }
                case 1:
                    {
                        if (client.Player.ConquerPoints >= 27)
                        {
                            client.Player.ConquerPoints -= 27;

                            if (client.Player.EnergyPointsMystic < 400)
                            {
                                client.Player.EnergyPointsMystic += Points;
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, id, true, "zf2-e233");

                                if (client.Player.EnergyPointsMystic >= 400)
                                {
                                    client.Player.EnergyPointsMystic = 400;

                                    data.Text("MysticAltar Statue:\n");
                                    data.Text("********************MysticAltar Statue**************************\n");

                                    data.Text("Energy Points " + client.Player.EnergyPointsMystic + " / 400\n");

                                    data.Text("********************************************************\n");
                                    data.Text("STR_ID_tArcherEpicWeapon[28684][Text1193]@@\n");
                                    data.Text("STR_ID_tArcherEpicWeapon[28684][Text1194]@@\n");
                                    data.Option("STR_ID_tArcherEpicWeapon[28684][Option116]@@\n", 5);
                                    data.AddAvatar(3143).FinalizeDialog();
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, 200013, true, "gssswq_xunhuan5");
                                    if (client.Map.ContainItem(2924) == 0)
                                    {

                                        Game.MsgServer.MsgGameItem item = new Game.MsgServer.MsgGameItem();
                                        item.Color = (Role.Flags.Color)2;
                                        item.ITEM_ID = 2924;
                                        Game.MsgFloorItem.MsgItem DropItem = new Game.MsgFloorItem.MsgItem(item, 56, 56, Game.MsgFloorItem.MsgItem.ItemType.Effect, 0, 0, client.Player.Map
                                                       , 0, false, client.Map, 60 * 60 * 24 * 30);

                                        if (client.Map.EnqueueItem(DropItem))
                                            DropItem.SendAll(stream, Game.MsgFloorItem.MsgDropID.Effect);
                                    }
                                    client.CreateBoxDialog("STR_ID_tArcherEpicWeapon[Content][28688]@@");
                                    return;

                                }
                                data.Text("MysticAltar Statue:\n");
                                data.Text("********************MysticAltar Statue**************************\n");

                                data.Text("Energy Points " + client.Player.EnergyPointsMystic + " / 400\n");

                                data.Text("********************************************************\n");
                                data.Text("STR_ID_tArcherEpicWeapon[28684][Text416]@@");
                                data.Option("Infuse energy. (27 CPs)", 1);
                                data.Option("Infuse 10 times. (270 CPs).", 3);
                                data.AddAvatar(3143).FinalizeDialog();

                            }
                            else
                            {
                                data.Text("MysticAltar Statue:\n");
                                data.Text("********************MysticAltar Statue**************************\n");

                                data.Text("Energy Points " + client.Player.EnergyPointsMystic + " / 400\n");

                                data.Text("********************************************************\n");

                                data.Text("STR_ID_tArcherEpicWeapon[28684][Text1193]@@\n");
                                data.Text("STR_ID_tArcherEpicWeapon[28684][Text1194]@@\n");
                                data.Option("STR_ID_tArcherEpicWeapon[28684][Option116]@@\n", 5);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, 200013, true, "gssswq_xunhuan5");
                                if (client.Map.ContainItem(2924) == 0)
                                {
                                    Game.MsgServer.MsgGameItem item = new Game.MsgServer.MsgGameItem();
                                    item.Color = (Role.Flags.Color)2;
                                    item.ITEM_ID = 2924;
                                    Game.MsgFloorItem.MsgItem DropItem = new Game.MsgFloorItem.MsgItem(item, 56, 56, Game.MsgFloorItem.MsgItem.ItemType.Effect, 0, 0, client.Player.Map
                                                   , 0, false, client.Map, 60 * 60 * 24 * 30);

                                    if (client.Map.EnqueueItem(DropItem))
                                        DropItem.SendAll(stream, Game.MsgFloorItem.MsgDropID.Effect);
                                }
                                data.AddAvatar(3143).FinalizeDialog();

                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("STR_ID_tArcherEpicWeapon[NoEmoney]@@");
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(722732, 3) && client.Inventory.Contain(722736, 3) || client.Inventory.Contain(722732, 3, 1) && client.Inventory.Contain(722736, 3, 1))
                        {
                            client.Inventory.RemoveStackItem(722736, 3, stream);
                            client.Inventory.RemoveStackItem(722732, 3, stream);

                            if (client.Player.EnergyPointsMystic < 400)
                            {
                                client.Player.EnergyPointsMystic += Points;
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, id, true, "zf2-e233");

                                if (client.Player.EnergyPointsMystic >= 400)
                                {
                                    client.Player.EnergyPointsMystic = 400;


                                    data.Text("MysticAltar Statue:\n");
                                    data.Text("********************MysticAltar Statue**************************\n");

                                    data.Text("Energy Points " + client.Player.EnergyPointsMystic + " / 400\n");

                                    data.Text("********************************************************\n");
                                    data.Text("STR_ID_tArcherEpicWeapon[28684][Text1193]@@\n");
                                    data.Text("STR_ID_tArcherEpicWeapon[28684][Text1194]@@\n");
                                    data.Option("STR_ID_tArcherEpicWeapon[28684][Option116]@@\n", 5);
                                    data.AddAvatar(3143).FinalizeDialog();
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, 200013, true, "gssswq_xunhuan5");
                                    Game.MsgServer.MsgGameItem item = new Game.MsgServer.MsgGameItem();
                                    item.Color = (Role.Flags.Color)2;
                                    item.ITEM_ID = 2924;
                                    Game.MsgFloorItem.MsgItem DropItem = new Game.MsgFloorItem.MsgItem(item, 56, 56, Game.MsgFloorItem.MsgItem.ItemType.Effect, 0, 0, client.Player.Map
                                                   , 0, false, client.Map, 60 * 60 * 24 * 30);

                                    if (client.Map.EnqueueItem(DropItem))
                                        DropItem.SendAll(stream, Game.MsgFloorItem.MsgDropID.Effect);
                                    client.CreateBoxDialog("STR_ID_tArcherEpicWeapon[Content][28688]@@");

                                    return;

                                }
                                data.Text("MysticAltar Statue:\n");
                                data.Text("********************MysticAltar Statue**************************\n");

                                data.Text("Energy Points " + client.Player.EnergyPointsMystic + " / 400\n");

                                data.Text("********************************************************\n");
                                data.Text("STR_ID_tArcherEpicWeapon[28684][Text416]@@");
                                data.Option("Infuse energy. (3 White/Red Flowers)", 2);
                                data.AddAvatar(3143).FinalizeDialog();

                            }
                            else
                            {
                                data.Text("MysticAltar Statue:\n");
                                data.Text("********************MysticAltar Statue**************************\n");

                                data.Text("Energy Points " + client.Player.EnergyPointsMystic + " / 400\n");

                                data.Text("********************************************************\n");
                                data.Text("STR_ID_tArcherEpicWeapon[28684][Text1193]@@\n");
                                data.Text("STR_ID_tArcherEpicWeapon[28684][Text1194]@@\n");
                                data.Option("STR_ID_tArcherEpicWeapon[28684][Option116]@@\n", 5);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, 200013, true, "gssswq_xunhuan5");
                                if (client.Map.ContainItem(2924) == 0)
                                {
                                    Game.MsgServer.MsgGameItem item = new Game.MsgServer.MsgGameItem();
                                    item.Color = (Role.Flags.Color)2;
                                    item.ITEM_ID = 2924;
                                    Game.MsgFloorItem.MsgItem DropItem = new Game.MsgFloorItem.MsgItem(item, 56, 56, Game.MsgFloorItem.MsgItem.ItemType.Effect, 0, 0, client.Player.Map
                                                   , 0, false, client.Map, 60 * 60 * 24 * 30);

                                    if (client.Map.EnqueueItem(DropItem))
                                        DropItem.SendAll(stream, Game.MsgFloorItem.MsgDropID.Effect);
                                }
                                data.AddAvatar(3143).FinalizeDialog();

                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("STR_ID_tArcherEpicWeapon[28541][NoFree]@@");
                        }
                        break;
                    }

                case 3:
                    {
                        if (client.Player.ConquerPoints >= 270)
                        {
                            client.Player.ConquerPoints -= 270;

                            if (client.Player.EnergyPointsMystic < 400)
                            {
                                for (int x = 0; x < 10; x++)
                                {
                                    client.Player.EnergyPointsMystic += Points;
                                }
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, id, true, "zf2-e233");

                                if (client.Player.EnergyPointsMystic >= 400)
                                {
                                    client.Player.EnergyPointsMystic = 400;


                                    data.Text("MysticAltar Statue:\n");
                                    data.Text("********************MysticAltar Statue**************************\n");

                                    data.Text("Energy Points " + client.Player.EnergyPointsMystic + " / 400\n");

                                    data.Text("********************************************************\n");
                                    data.Text("STR_ID_tArcherEpicWeapon[28684][Text1193]@@\n");
                                    data.Text("STR_ID_tArcherEpicWeapon[28684][Text1194]@@\n");
                                    data.Option("STR_ID_tArcherEpicWeapon[28684][Option116]@@\n", 5);
                                    data.AddAvatar(3143).FinalizeDialog();
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, 200013, true, "gssswq_xunhuan5");
                                    if (client.Map.ContainItem(2924) == 0)
                                    {
                                        Game.MsgServer.MsgGameItem item = new Game.MsgServer.MsgGameItem();
                                        item.Color = (Role.Flags.Color)2;
                                        item.ITEM_ID = 2924;
                                        Game.MsgFloorItem.MsgItem DropItem = new Game.MsgFloorItem.MsgItem(item, 56, 56, Game.MsgFloorItem.MsgItem.ItemType.Effect, 0, 0, client.Player.Map
                                                       , 0, false, client.Map, 60 * 60 * 24 * 30);

                                        if (client.Map.EnqueueItem(DropItem))
                                            DropItem.SendAll(stream, Game.MsgFloorItem.MsgDropID.Effect);
                                    }
                                    client.CreateBoxDialog("STR_ID_tArcherEpicWeapon[Content][28688]@@");
                                    return;

                                }
                                data.Text("MysticAltar Statue:\n");
                                data.Text("********************MysticAltar Statue**************************\n");

                                data.Text("Energy Points " + client.Player.EnergyPointsMystic + " / 400\n");

                                data.Text("********************************************************\n");
                                data.Text("STR_ID_tArcherEpicWeapon[28684][Text416]@@");
                                data.Option("Infuse energy. (27 CPs)", 1);
                                data.Option("Infuse 10 times. (270 CPs).", 3);
                                data.AddAvatar(3143).FinalizeDialog();


                            }
                            else
                            {
                                data.Text("MysticAltar Statue:\n");
                                data.Text("********************MysticAltar Statue**************************\n");

                                data.Text("Energy Points " + client.Player.EnergyPointsMystic + " / 400\n");

                                data.Text("********************************************************\n");
                                data.Text("STR_ID_tArcherEpicWeapon[28684][Text1193]@@\n");
                                data.Text("STR_ID_tArcherEpicWeapon[28684][Text1194]@@\n");
                                data.Option("STR_ID_tArcherEpicWeapon[28684][Option116]@@\n", 5);
                                data.AddAvatar(3143).FinalizeDialog();
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, 200013, true, "gssswq_xunhuan5");
                                if (client.Map.ContainItem(2924) == 0)
                                {
                                    Game.MsgServer.MsgGameItem item = new Game.MsgServer.MsgGameItem();
                                    item.Color = (Role.Flags.Color)2;
                                    item.ITEM_ID = 2924;
                                    Game.MsgFloorItem.MsgItem DropItem = new Game.MsgFloorItem.MsgItem(item, 56, 56, Game.MsgFloorItem.MsgItem.ItemType.Effect, 0, 0, client.Player.Map
                                                   , 0, false, client.Map, 60 * 60 * 24 * 30);

                                    if (client.Map.EnqueueItem(DropItem))
                                        DropItem.SendAll(stream, Game.MsgFloorItem.MsgDropID.Effect);
                                }
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("STR_ID_tArcherEpicWeapon[NoEmoney]@@");
                        }
                        break;
                    }
                case 5:
                    {

                        client.Player.EnergyPoints = 0;
                        client.Player.EnergyPointsTalent = 0;
                        client.Player.EnergyPointsAntiwar = 0;
                        client.Player.EnergyPointsAntifatalism = 0;
                        client.Player.EnergyPointsMystic = 0;
                        client.Player.stage = 0;
                        client.Player.Laststage = 0;
                        client.Teleport(73, 72, 10451);
                        client.Inventory.Add(stream, 3324521);
                        client.CreateBoxDialog("STR_ID_tArcherEpicWeapon[28541][Reward]@@");
                        var Map = Pool.ServerMaps[11257];
                        foreach (var obj in Map.View.GetAllMapRoles(MapObjectType.Item))
                        {
                            Game.MsgFloorItem.MsgItem Item = obj as Game.MsgFloorItem.MsgItem;
                            if (Item != null)
                            {
                                Map.RemoveTrapGuild(Item.X, Item.Y, Item);
                            }
                        }
                        break;
                    }

            }

        }

        [NpcAttribute(NpcID.EpicArcher)]
        public static void EpicArcher(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain((uint)NpcID.EpicArcher, 1))
            {
                client.SendSysMesage("You Don't have this item in Inventory");
                return;
            }
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.Text("STR_ID_tArcherEpicWeapon[3324521][Text111]@@\n");
                        data.Text("STR_ID_tArcherEpicWeapon[3324521][Text112]@@\n");
                        data.Text("STR_ID_tArcherEpicWeapon[3324521][Text113]@@\n");
                        data.Option("STR_ID_tArcherEpicWeapon[3324521][Option1]@@", 1);
                        data.Option("STR_ID_tArcherEpicWeapon[3324521][Option2]@@", byte.MaxValue);
                        data.AddAvatar(3392).FinalizeDialog();
                        break;

                    }
                case 1:
                    {
                        data.Text("STR_ID_tArcherEpicWeapon[3324521][Text111]@@\n");
                        data.Text("STR_ID_tArcherEpicWeapon[3324521][Text112]@@\n");
                        data.Text("STR_ID_tArcherEpicWeapon[3324521][Text113]@@\n");
                        data.Option("STR_ID_tArcherEpicWeapon[3324521][SuccPos4]@@", 3);
                        data.Option("STR_ID_tArcherEpicWeapon[3324521][SuccPos5]@@", 4);
                        data.AddAvatar(3392).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        MsgGameItem GameItem;
                        if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.RightWeapon, out GameItem))
                        {
                            if (GameItem.Purification.PurificationItemID != 0)
                            {
                                client.SendSysMesage("Soryy, You're  equipped  the Knife's with soul.");
                                break;
                            }
                            if (Database.ItemType.IsKnife(GameItem.ITEM_ID))
                            {
                                uint UpdateToEpic = (GameItem.ITEM_ID % 1000) + 606000;
                                GameItem.ITEM_ID = UpdateToEpic;
                                GameItem.Mode = Role.Flags.ItemMode.Update;
                                GameItem.Send(client, stream);
                                client.Inventory.Remove(3324521, 1, stream);
                                client.Equipment.QueryEquipment(client.Equipment.Alternante, true);


                                Server.SendGlobalPacket(new MsgMessage("Congratulation! " + client.Player.Name + " has finished the Archer Epic Quest.", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));


                                client.Inventory.Update(GameItem, Role.Instance.AddMode.REMOVE, stream);
                            }
                            else
                            {

                                client.SendSysMesage("Soryy, You're not equipped with the Knife's(right weapon).");


                            }


                        }
                        else
                        {


                            client.SendSysMesage("Soryy, You're not equipped with the Knife's(right weapon).");

                        }
                        break;
                    }
                case 4:
                    {
                        MsgGameItem GameItem;
                        if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.LeftWeapon, out GameItem))
                        {
                            if (GameItem.Purification.PurificationItemID != 0)
                            {
                                client.SendSysMesage("Soryy, You're  equipped  the Knife's with soul.");
                                break;
                            }
                            if (Database.ItemType.IsKnife(GameItem.ITEM_ID))
                            {
                                uint UpdateToEpic = (GameItem.ITEM_ID % 1000) + 606000;
                                GameItem.ITEM_ID = UpdateToEpic;
                                GameItem.Mode = Role.Flags.ItemMode.Update;
                                GameItem.Send(client, stream);
                                client.Inventory.Remove(3324521, 1, stream);
                                client.Equipment.QueryEquipment(client.Equipment.Alternante, true);


                                Server.SendGlobalPacket(new MsgMessage("Congratulation! " + client.Player.Name + " has finished the Archer Epic Quest.", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));


                                client.Inventory.Update(GameItem, Role.Instance.AddMode.REMOVE, stream);
                            }
                            else
                            {

                                client.SendSysMesage("Soryy, You're not equipped with the Knife's(right weapon).");


                            }


                        }
                        else
                        {


                            client.SendSysMesage("Soryy, You're not equipped with the Knife's(right weapon).");

                        }
                        break;

                    }

            }

        }
        #endregion
        [NpcAttribute(NpcID.EpicMonk)]
        public static void EpicMonk(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain((uint)NpcID.EpicMonk, 1))
            {
                client.SendSysMesage("You Don't have this item in Inventory");
                return;
            }
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.Text("When your right hand is equipped with an epic weapon, the Prayed Bead on your hand can be converted into an epic weapon\n");
                        data.Option("Transform~RightWeapon~into~an~Epic~Weapon.", 3);
                        data.Option("Transform~LeftWeapon~into~an~Epic~Weapon.", 4);
                        data.AddAvatar(0).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        MsgGameItem GameItem;
                        if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.RightWeapon, out GameItem))
                        {
                            if (Database.ItemType.IsPrayedBead(GameItem.ITEM_ID))
                            {
                                uint UpdateToEpic = (GameItem.ITEM_ID % 1000) + 622000;
                                GameItem.ITEM_ID = UpdateToEpic;
                                GameItem.Mode = Role.Flags.ItemMode.Update;
                                GameItem.Send(client, stream);
                                client.Inventory.Remove(3007564, 1, stream);
                                client.Equipment.QueryEquipment(client.Equipment.Alternante, true);


                                Server.SendGlobalPacket(new MsgMessage("Congratulation! " + client.Player.Name + " has finished the Monk Epic Quest.", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));


                                client.Inventory.Update(GameItem, Role.Instance.AddMode.REMOVE, stream);
                            }
                            else
                            {

                                client.SendSysMesage("Soryy, You're not equipped with the PrayerBeads's(right weapon).");


                            }


                        }
                        else
                        {

                            client.SendSysMesage("Soryy, You're not equipped with the PrayerBeads's(right weapon).");


                        }
                        break;
                    }
                case 4:
                    {
                        MsgGameItem GameItem;
                        if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.LeftWeapon, out GameItem))
                        {
                            if (Database.ItemType.IsPrayedBead(GameItem.ITEM_ID))
                            {
                                uint UpdateToEpic = (GameItem.ITEM_ID % 1000) + 622000;
                                GameItem.ITEM_ID = UpdateToEpic;
                                GameItem.Mode = Role.Flags.ItemMode.Update;
                                GameItem.Send(client, stream);
                                client.Inventory.Remove(3007564, 1, stream);
                                client.Equipment.QueryEquipment(client.Equipment.Alternante, true);


                                Server.SendGlobalPacket(new MsgMessage("Congratulation! " + client.Player.Name + " has finished the Monk Epic Quest.", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));


                                client.Inventory.Update(GameItem, Role.Instance.AddMode.REMOVE, stream);
                            }
                            else
                            {

                                client.SendSysMesage("Soryy, You're not equipped with the PrayerBeads's(Left weapon).");
                            }
                        }
                        else
                        {

                            client.SendSysMesage("Soryy, You're not equipped with the PrayerBeads's(Left weapon).");


                        }
                        break;
                    }

            }

        }
    }
}

