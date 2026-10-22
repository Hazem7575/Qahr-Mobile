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


        [NpcAttribute(NpcID.BountyHall)]
        public static void BountyHall(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {
                        data.Text("STR_ID_tTaskAssembly[26228][Text111]@@");
                        data.AddText("STR_ID_tTaskAssembly[26228][Text112]@@");
                        data.AddText("STR_ID_tTaskAssembly[26228][Text113]@@");
                        data.AddText("STR_ID_tTaskAssembly[26228][Text114]@@");
                        data.AddText("STR_ID_tTaskAssembly[26228][Text115]@@");
                        if (client.Player.Level < 110)
                        {
                            data.Option("STR_ID_tTaskAssembly[26228][Option111]@@\n", 1);
                        }
                        else
                        {
                            data.Option("STR_ID_tTaskAssembly[26228][Option121]@@\n", 1);
                        }
                        data.AddAvatar(93).FinalizeDialog();
                        break;
                    }


            }
        }
        [NpcAttribute(NpcID.HideEffect)]
        public static void HideEffect(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {
                        data.Text("Please choose The Effect you want Hide");
                        data.Option("All Effect [ " + (client.Player.SomeHide ? "UnHide ]" : "Hide ]"), 1);
                        data.Option("Sigil Ninja Effect [ " + (client.Player.SigilNinja ? "UnHide ]" : "Hide ]"), 2);
                        data.Option("Sigil Pirate Effect [ " + (client.Player.SigilPirate ? "UnHide ]" : "Hide ]"), 3);
                        data.Option("All Floor Effect [ " + (client.Player.HideFloor ? "UnHide ]" : "Hide ]"), 4);
                        data.Option("Wing&&Title&&Hallo Effect [ " + (client.Player.WingTitleHaloHide ? "UnHide ]" : "Hide ]"), 5);
                        data.AddOption("Thx#World.", 255);
                        data.AddAvatar(93).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Player.SomeHide)
                            client.Player.MessageBox("You Need Hide Press Yes ,You Client Will Exit Open Agian to Change Settings Effect. " + client.Player.SomeHide + "", new Action<Client.GameClient>(p =>
                            {
                                using (var recBox = new ServerSockets.RecycledPacket())
                                    client.Send(recBox.GetStream().ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.Effects, 0, 0, (uint)DateTime.Now.Value(), client.Player.SomeHide ? (uint)0 : (uint)1, 0, 0, 0));
                            }), null, 60);
                        else
                            client.Player.MessageBox("You Need UnHide Press Yes , You Client Will Exit Open Agian to Change Settings Effect. " + client.Player.SomeHide + "", new Action<Client.GameClient>(p =>
                            {
                                using (var recBox = new ServerSockets.RecycledPacket())
                                    client.Send(recBox.GetStream().ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.Effects, 0, 0, (uint)DateTime.Now.Value(), client.Player.SomeHide ? (uint)0 : (uint)1, 0, 0, 0));
                            }), null, 60);

                        break;
                    }
                case 2:
                    {
                        client.Player.SigilNinja = !client.Player.SigilNinja;
                        client.SendSysMesage("Sigil Ninja Effect [ " + (client.Player.SigilNinja ? "Hide ]" : "UnHide ]"));
                        break;
                    }
                case 3:
                    {
                        client.Player.SigilPirate = !client.Player.SigilPirate;
                        client.SendSysMesage("Sigil Pirate Effect [ " + (client.Player.SigilPirate ? "Hide ]" : "UnHide ]"));
                        break;
                    }
                case 4:
                    {
                        client.Player.HideFloor = !client.Player.HideFloor;
                        client.SendSysMesage("All Floor Effect [ " + (client.Player.HideFloor ? "Hide ]" : "UnHide ]"));
                        break;
                    }
                case 5:
                    {
                        client.Player.WingTitleHaloHide = !client.Player.WingTitleHaloHide;
                        client.SendSysMesage("Wing Title Hallo Effect [ " + (client.Player.WingTitleHaloHide ? "Hide ]" : "UnHide ]"));
                        break;
                    }

            }
        }
        //[NpcAttribute(NpcID.NewPlayer)]
        //public static void NewPlayer(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog data = new Dialog(client, stream);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                client.firstchar = client.secondchar = client.finalchar = 0;
        //                data.AddText("Hello [ <tip color=0xffc501e7> "+  client.Player.Name+" </tip> ] ,Welcome to TroyConquer\n");
        //                data.AddText("Please select your first character");
        //                data.AddOption("Trojan.", 15);
        //                data.AddOption("Warrior.", 25);
        //                data.AddOption("Archer.", 45);
        //                data.AddOption("Ninja.", 55);
        //                data.AddOption("Monk", 65);
        //                data.AddOption("Pirate", 75);
        //                data.AddOption("Dragon~Warrior", 85);
        //                data.AddOption("WaterTaoist.", 135);
        //                data.AddOption("FireTaoist.", 145);
        //                data.AddOption("WindWalker", 165);
        //                data.AddOption("Thunderstriker", 95);
        //                data.AddOption("I`ll~think~about~it.", 255);
        //                data.AddAvatar(67).FinalizeDialog();
        //                break;
        //            }

        //        #region First Classes change
        //        case 15:
        //        case 25:
        //        case 45:
        //        case 55:
        //        case 65:
        //        case 85:
        //        case 75:
        //        case 135:
        //        case 145:
        //        case 165:
        //        case 95:
        //            {
        //                client.firstchar = Database.AtributesStatus.NewClassNumber(Option);
        //                data.AddText("<tip color=0xff00fa00 desc=char#which#you#selected#when#you#created#your#account>FirstCharacter</tip>: " + Class(client.firstchar) + "\n");
        //                data.AddText("<tip color=0xff00fa00 desc=your#first#rebirth#character>SecondCharacter</tip>: " + Class(0) + "\n");
        //                data.AddText("<tip color=0xff00fa00 desc=your#second#rebirth#character#&#the#char#that#you#will#play#with>FinalCharacter</tip>: " + Class(0) + "\n");
        //                data.AddText("Please select your second character");
        //                data.AddOption("Trojan.", 15 + 1);
        //                data.AddOption("Warrior.", 25 + 1);
        //                data.AddOption("Archer.", 45 + 1);
        //                data.AddOption("WaterTaoist.", 135 + 1);
        //                data.AddOption("FireTaoist.", 145 + 1);
        //                data.AddOption("Ninja.", 55 + 1);
        //                data.AddOption("Monk.", 65 + 1);
        //                data.AddOption("Pirate.", 75 + 1);
        //                data.AddOption("Dragon-Warrior.", 85 + 1);
        //                data.AddOption("WindWalker", 165 + 1);
        //                data.AddOption("Thunderstriker", 95 + 1);
        //                data.AddOption("Not now ", 255);
        //                data.AddAvatar(67).FinalizeDialog();
        //                break;
        //            }
        //        #endregion
        //        #region Secand Class change
        //        case 16:
        //        case 26:
        //        case 46:
        //        case 56:
        //        case 66:
        //        case 86:
        //        case 76:
        //        case 136:
        //        case 146:
        //        case 166:
        //        case 96:
        //            {

        //                client.secondchar = Option - 1;
        //                client.secondchar = Database.AtributesStatus.NewClassNumber(client.secondchar);
        //                data.AddText("<tip color=0xff00fa00 desc=char#which#you#selected#when#you#created#your#account>FirstCharacter</tip>: " + Class(client.firstchar) + "\n");
        //                data.AddText("<tip color=0xff00fa00 desc=your#first#rebirth#character>SecondCharacter</tip>: " + Class(client.secondchar) + "\n");
        //                data.AddText("<tip color=0xff00fa00 desc=your#second#rebirth#character#&#the#char#that#you#will#play#with>FinalCharacter</tip>: " + Class(0) + "\n");

        //                data.AddText("Please select your Final character");
        //                data.AddOption("Trojan.", 15 + 2);
        //                data.AddOption("Warrior.", 25 + 2);
        //                data.AddOption("Archer.", 45 + 2);
        //                data.AddOption("WaterTaoist.", 135 + 2);
        //                data.AddOption("FireTaoist.", 145 + 2);
        //                data.AddOption("Ninja.", 55 + 2);
        //                data.AddOption("Monk.", 65 + 2);
        //                data.AddOption("Pirate.", 75 + 2);
        //                data.AddOption("Dragon-Warrior.", 85 + 2);
        //                data.AddOption("WindWalker", 165 + 2);
        //                data.AddOption("Thunderstriker", 95 + 2);
        //                data.AddOption("Not now ", 255);
        //                data.AddAvatar(67).FinalizeDialog();
        //                break;
        //            }
        //        #endregion
        //        #region  Third Class
        //        case 17:
        //        case 27:
        //        case 47:
        //        case 57:
        //        case 67:
        //        case 87:
        //        case 77:
        //        case 137:
        //        case 147:
        //        case 167:
        //        case 97:
        //            {

        //                client.finalchar = Option - 2;
        //                client.finalchar = Database.AtributesStatus.NewClassNumber(client.finalchar);
        //                data.AddText("<tip color=0xff00fa00 desc=char#which#you#selected#when#you#created#your#account>Your FirstCharacter is</tip>: " + Class(client.firstchar) + "\n");
        //                data.AddText("<tip color=0xff00fa00 desc=your#first#rebirth#character>Your SecondCharacter is</tip>: " + Class(client.secondchar) + "\n");
        //                data.AddText("<tip color=0xff00fa00 desc=your#second#rebirth#character#&#the#char#that#you#will#play#with>Your FinalCharacter is</tip>: " + Class(client.finalchar) + "\n");
        //                data.AddText("Please select your Mode Atributes .")
        //                .AddOption("[MaxHP]+[P-Attack]+[PStrike]+[Immunity]", 201)
        //                .AddOption("[MaxHP]+[P-Attack]+[Break]+[Immunity]", 202)
        //                .AddOption("[MaxHP]+[M-Attack]+[M-Strike]+[Immunity]", 203)
        //                 .AddOption("[MaxHP]+[P-Damge]+[AntiBreak]+[Immunity]", 204);
        //                data.AddAvatar(67).FinalizeDialog();
        //                break;
        //            }
        //        case 201:
        //        case 202:
        //        case 203:
        //        case 204:
        //            {
        //                client.ModeAtributes = Option;
        //                data.AddText("<tip color=0xff00fa00 desc=char#which#you#selected#when#you#created#your#account>Your FirstCharacter is</tip>: " + Class(client.firstchar) + "\n");
        //                data.AddText("<tip color=0xff00fa00 desc=your#first#rebirth#character>Your SecondCharacter is</tip>: " + Class(client.secondchar) + "\n");
        //                data.AddText("<tip color=0xff00fa00 desc=your#second#rebirth#character#&#the#char#that#you#will#play#with>Your FinalCharacter is</tip>: " + Class(client.finalchar) + "\n");
        //                data.AddText("<tip color=0xff00fa00 desc=your#second#rebirth#character#&#the#char#that#you#will#play#with>Your FinalCharacter is</tip>: " + ATTKs(client.ModeAtributes) + "\n");
        //                data.AddText("Do you want to Finalize you account now ?");
        //                data.AddOption("Yes", 200);
        //                data.AddOption("Not yet", 255);
        //                data.AddAvatar(67).FinalizeDialog();
        //                break;
        //            }
        //        #endregion
        //        case 200:
        //            {
        //                if (client.Rune.RebornRunesEquip.Count() > 0)
        //                {
        //                    var Item = client.Rune.RebornRunesEquip.ToArray();
        //                    for (byte x = 0; x < Item.Length; x++)
        //                    {
        //                        client.Rune.Unequip(Item[x], true);
        //                    }
        //                }

        //                if (client.firstchar == 0 || client.secondchar == 0 || client.finalchar == 0)
        //                {
        //                    client.Player.MessageBox("sorry please select all class reborn");
        //                    return;
        //                }
        //                #region FixSkill By(MT)
        //                if (AtributesStatus.IsTaoist((uint)client.firstchar))
        //                {
        //                    Pool.RebornInfo.Reborn(client.Player, (uint)client.firstchar - 3, stream);
        //                }
        //                else
        //                {
        //                    Pool.RebornInfo.Reborn(client.Player, (uint)client.firstchar - 4, stream);
        //                }
        //                if (AtributesStatus.IsTaoist((uint)client.secondchar))
        //                {
        //                    Pool.RebornInfo.Reborn(client.Player, (uint)client.secondchar - 3, stream);
        //                }
        //                else
        //                {
        //                    Pool.RebornInfo.Reborn(client.Player, (uint)client.secondchar - 4, stream);
        //                }
        //                if (AtributesStatus.IsTaoist((uint)client.finalchar))
        //                {
        //                    Pool.RebornInfo.Reborn(client.Player, (uint)client.finalchar - 3, stream);
        //                }
        //                else
        //                {
        //                    Pool.RebornInfo.Reborn(client.Player, (uint)client.finalchar - 4, stream);
        //                }

        //                #endregion
        //                #region SendUpdate(Class)
        //                client.Player.Reborn = 2;
        //                client.Player.Class = (uint)client.finalchar;//
        //                client.Player.SendUpdate(stream, client.Player.Class, Game.MsgServer.MsgUpdate.DataType.Class);
        //                client.Player.FirstClass = (uint)client.firstchar;//
        //                client.Player.FirstRebornLevel = 140;
        //                client.Player.SecoundeRebornLevel = 140;
        //                client.Player.SecoundeClass = (uint)client.secondchar;//
        //                client.Player.SendUpdate(stream, client.Player.FirstClass, Game.MsgServer.MsgUpdate.DataType.FirsRebornClass);
        //                client.Player.SendUpdate(stream, client.Player.SecoundeClass, Game.MsgServer.MsgUpdate.DataType.SecondRebornClass);
        //                #endregion
        //                #region Level 140
        //                client.UpdateLevel(stream, 140, false, true);
        //                #endregion
        //                #region SendUpdate(Atributes)
        //                client.Player.Atributes = 500;
        //                client.Player.Agility = 36;
        //                client.Player.Strength = 0;
        //                client.Player.Vitality = 1;
        //                client.Player.Spirit = 0;
        //                client.Player.SendUpdate(stream, client.Player.Agility, Game.MsgServer.MsgUpdate.DataType.Agility);
        //                client.Player.SendUpdate(stream, client.Player.Strength, Game.MsgServer.MsgUpdate.DataType.Strength);
        //                client.Player.SendUpdate(stream, client.Player.Vitality, Game.MsgServer.MsgUpdate.DataType.Vitality);
        //                client.Player.SendUpdate(stream, client.Player.Spirit, Game.MsgServer.MsgUpdate.DataType.Spirit);
        //                client.Player.SendUpdate(stream, client.Player.Atributes, Game.MsgServer.MsgUpdate.DataType.Atributes);
        //                #endregion
        //                #region LearnNewSubClass
        //                client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.MartialArtist, client, stream);
        //                client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Warlock, client, stream);
        //                client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.ChiMaster, client, stream);
        //                client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Sage, client, stream);
        //                client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Apothecary, client, stream);
        //                client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Wrangler, client, stream);
        //                client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Performer, client, stream);
        //                #endregion
        //                #region Update Proficiency
        //                if (client.Player.Class >= 1000 && client.Player.Class <= 1005)
        //                {
        //                    client.MyProfs.Add(stream, (ushort)480, 20, 0);
        //                    client.MyProfs.Add(stream, (ushort)410, 20, 0);
        //                    client.MyProfs.Add(stream, (ushort)561, 20, 0);
        //                    client.MyProfs.Add(stream, (ushort)420, 20, 0);
        //                    client.MyProfs.Add(stream, (ushort)490, 20, 0);

        //                }
        //                if (Database.AtributesStatus.IsWarrior(client.Player.Class))
        //                {
        //                    client.MyProfs.Add(stream, (ushort)561, 20, 0);
        //                    client.MyProfs.Add(stream, (ushort)561, 20, 0);
        //                    client.MyProfs.Add(stream, (ushort)624, 20, 0);
        //                    client.MyProfs.Add(stream, (ushort)560, 20, 0);


        //                }
        //                if (Database.AtributesStatus.IsArcher(client.Player.Class))
        //                {
        //                    client.MyProfs.Add(stream, (ushort)613, 20, 0);
        //                    client.MyProfs.Add(stream, (ushort)500, 20, 0);
        //                    client.MyProfs.Add(stream, (ushort)561, 20, 0);


        //                }
        //                if (Database.AtributesStatus.IsNinja(client.Player.Class))
        //                {
        //                    client.MyProfs.Add(stream, (ushort)601, 20, 0);
        //                    client.MyProfs.Add(stream, (ushort)561, 20, 0);
        //                    client.MyProfs.Add(stream, (ushort)511, 20, 0);


        //                }
        //                if (Database.AtributesStatus.IsMonk(client.Player.Class))
        //                {
        //                    client.MyProfs.Add(stream, (ushort)610, 20, 0);
        //                    client.MyProfs.Add(stream, (ushort)561, 20, 0);


        //                }
        //                if (Database.AtributesStatus.IsPirate(client.Player.Class))
        //                {
        //                    client.MyProfs.Add(stream, (ushort)612, 20, 0);
        //                    client.MyProfs.Add(stream, (ushort)611, 20, 0);
        //                    client.MyProfs.Add(stream, (ushort)561, 20, 0);

        //                }
        //                if (Database.AtributesStatus.IsLee(client.Player.Class))
        //                {
        //                    client.MyProfs.Add(stream, (ushort)617, 20, 0);
        //                    client.MyProfs.Add(stream, (ushort)561, 20, 0);


        //                }
        //                if (Database.AtributesStatus.IsWindWalker(client.Player.Class))
        //                {
        //                    client.MyProfs.Add(stream, (ushort)626, 20, 0);
        //                    client.MyProfs.Add(stream, (ushort)561, 20, 0);

        //                }

        //                #endregion
        //                #region OpenChi
        //                if (!Role.Instance.Chi.ChiPool.ContainsKey(client.Player.UID))
        //                    Role.Instance.Chi.ChiPool.TryAdd(client.Player.UID, client.Player.MyChi);
        //                client.Player.MyChi.Dragon.UnLocked = true;
        //                client.Player.MyChi.Phoenix.UnLocked = true;
        //                client.Player.MyChi.Tiger.UnLocked = true;
        //                client.Player.MyChi.Turtle.UnLocked = true;
        //                client.Player.MyChi.Dragon.UID = client.Player.UID;
        //                client.Player.MyChi.Dragon.Name = client.Player.Name;
        //                client.Player.MyChi.Phoenix.UID = client.Player.UID;
        //                client.Player.MyChi.Phoenix.Name = client.Player.Name;
        //                client.Player.MyChi.Tiger.UID = client.Player.UID;
        //                client.Player.MyChi.Tiger.Name = client.Player.Name;
        //                client.Player.MyChi.Turtle.UID = client.Player.UID;
        //                client.Player.MyChi.Turtle.Name = client.Player.Name;
        //                client.Player.MyChi.Dragon.Reroll(MsgChiInfo.LockedFlags.None);
        //                client.Player.MyChi.Phoenix.Reroll(MsgChiInfo.LockedFlags.None);
        //                client.Player.MyChi.Tiger.Reroll(MsgChiInfo.LockedFlags.None);
        //                client.Player.MyChi.Turtle.Reroll(MsgChiInfo.LockedFlags.None);
        //                MsgChiInfo.MsgHandleChi.SendInfo(client, MsgChiInfo.Action.Send);


        //                #endregion
        //                #region OpenJiang

        //                client.Player.MyJiangHu = new Role.Instance.JiangHu(client.Player.UID);
        //                client.Player.MyJiangHu.CustomizedName = client.Player.Name;
        //                client.Player.MyJiangHu.Name = client.Player.Name;

        //                client.Player.MyJiangHu.ActiveJiangMode(client);
        //                client.Player.MyJiangHu.Level = (byte)client.Player.Level;

        //                client.Player.MyJiangHu.SendInfo(client, MsgJiangHuInfo.JiangMode.SetName, false, client.Player.UID.ToString(), "1", "1");

        //                client.Send(stream.JiangHuStatusCreate(client.Player.Name, 1, client.Player.MyJiangHu.Talent
        //                 , 0, client.Player.SubClass.StudyPoints, client.Player.MyJiangHu.FreeTimeToday));

        //                client.Player.SubClass.AddStudyPoints(client, 100, stream);
        //                client.Player.MyJiangHu.CreateTime();
        //                client.Player.MyJiangHu.SendInfo(client, MsgJiangHuInfo.JiangMode.UpdateTime, false, client.Player.MyJiangHu.FreeCourse.ToString(), client.Player.MyJiangHu.Time.ToString());
        //                client.Player.MyJiangHu.SendInfo(client, MsgJiangHuInfo.JiangMode.UpdateTalent, true, client.Player.UID.ToString(), client.Player.MyJiangHu.Talent.ToString());


        //                #endregion
        //                #region Get (ModeAtributes)

        //                #region Jiang(Mode)
        //                if (client.ModeAtributes == 201)
        //                {
        //                    #region JiangSuper
        //                    for (byte x = 1; x < 10; x++)
        //                    {
        //                        if (client.Player.MyJiangHu != null)
        //                        {
        //                            client.Player.MyJiangHu.CreateRollSuper(stream, client, x, 1, 14);
        //                            client.Player.MyJiangHu.ApplayNewStar(client);
        //                        }
        //                    }
        //                    for (byte x = 1; x < 10; x++)
        //                    {
        //                        if (client.Player.MyJiangHu != null)
        //                        {
        //                            client.Player.MyJiangHu.CreateRollSuper(stream, client, x, 2, 12);
        //                            client.Player.MyJiangHu.ApplayNewStar(client);
        //                        }
        //                    }
        //                    for (byte x = 1; x < 10; x++)
        //                    {
        //                        if (client.Player.MyJiangHu != null)
        //                        {
        //                            client.Player.MyJiangHu.CreateRollSuper(stream, client, x, 3, 2);
        //                            client.Player.MyJiangHu.ApplayNewStar(client);
        //                        }
        //                    }
        //                    for (byte x = 1; x < 10; x++)
        //                    {
        //                        if (client.Player.MyJiangHu != null)
        //                        {
        //                            client.Player.MyJiangHu.CreateRollSuper(stream, client, x, 4, 10);
        //                            client.Player.MyJiangHu.ApplayNewStar(client);
        //                        }
        //                    }
        //                    for (byte x = 1; x < 10; x++)
        //                    {
        //                        if (client.Player.MyJiangHu != null)
        //                        {
        //                            client.Player.MyJiangHu.CreateRollSuper(stream, client, x, 5, 1);
        //                            client.Player.MyJiangHu.ApplayNewStar(client);
        //                        }
        //                    }
        //                    for (byte x = 1; x < 10; x++)
        //                    {
        //                        if (client.Player.MyJiangHu != null)
        //                        {
        //                            client.Player.MyJiangHu.CreateRollSuper(stream, client, x, 6, 12);
        //                            client.Player.MyJiangHu.ApplayNewStar(client);
        //                        }
        //                    }
        //                    for (byte x = 1; x < 10; x++)
        //                    {
        //                        if (client.Player.MyJiangHu != null)
        //                        {
        //                            client.Player.MyJiangHu.CreateRollSuper(stream, client, x, 7, 2);
        //                            client.Player.MyJiangHu.ApplayNewStar(client);
        //                        }
        //                    }
        //                    for (byte x = 1; x < 10; x++)
        //                    {
        //                        if (client.Player.MyJiangHu != null)
        //                        {
        //                            client.Player.MyJiangHu.CreateRollSuper(stream, client, x, 8, 10);
        //                            client.Player.MyJiangHu.ApplayNewStar(client);
        //                        }
        //                    }
        //                    for (byte x = 1; x < 10; x++)
        //                    {
        //                        if (client.Player.MyJiangHu != null)
        //                        {
        //                            client.Player.MyJiangHu.CreateRollSuper(stream, client, x, 9, 1);
        //                            client.Player.MyJiangHu.ApplayNewStar(client);
        //                        }
        //                    }
        //                    #endregion
        //                }
        //                if (client.ModeAtributes == 202)
        //                {
        //                    #region JiangSuper
        //                    for (byte x = 1; x < 10; x++)
        //                    {
        //                        if (client.Player.MyJiangHu != null)
        //                        {
        //                            client.Player.MyJiangHu.CreateRollSuper(stream, client, x, 1, 13);
        //                            client.Player.MyJiangHu.ApplayNewStar(client);
        //                        }
        //                    }
        //                    for (byte x = 1; x < 10; x++)
        //                    {
        //                        if (client.Player.MyJiangHu != null)
        //                        {
        //                            client.Player.MyJiangHu.CreateRollSuper(stream, client, x, 2, 12);
        //                            client.Player.MyJiangHu.ApplayNewStar(client);
        //                        }
        //                    }
        //                    for (byte x = 1; x < 10; x++)
        //                    {
        //                        if (client.Player.MyJiangHu != null)
        //                        {
        //                            client.Player.MyJiangHu.CreateRollSuper(stream, client, x, 3, 2);
        //                            client.Player.MyJiangHu.ApplayNewStar(client);
        //                        }
        //                    }
        //                    for (byte x = 1; x < 10; x++)
        //                    {
        //                        if (client.Player.MyJiangHu != null)
        //                        {
        //                            client.Player.MyJiangHu.CreateRollSuper(stream, client, x, 4, 13);
        //                            client.Player.MyJiangHu.ApplayNewStar(client);
        //                        }
        //                    }
        //                    for (byte x = 1; x < 10; x++)
        //                    {
        //                        if (client.Player.MyJiangHu != null)
        //                        {
        //                            client.Player.MyJiangHu.CreateRollSuper(stream, client, x, 5, 1);
        //                            client.Player.MyJiangHu.ApplayNewStar(client);
        //                        }
        //                    }
        //                    for (byte x = 1; x < 10; x++)
        //                    {
        //                        if (client.Player.MyJiangHu != null)
        //                        {
        //                            client.Player.MyJiangHu.CreateRollSuper(stream, client, x, 6, 12);
        //                            client.Player.MyJiangHu.ApplayNewStar(client);
        //                        }
        //                    }
        //                    for (byte x = 1; x < 10; x++)
        //                    {
        //                        if (client.Player.MyJiangHu != null)
        //                        {
        //                            client.Player.MyJiangHu.CreateRollSuper(stream, client, x, 7, 2);
        //                            client.Player.MyJiangHu.ApplayNewStar(client);
        //                        }
        //                    }
        //                    for (byte x = 1; x < 10; x++)
        //                    {
        //                        if (client.Player.MyJiangHu != null)
        //                        {
        //                            client.Player.MyJiangHu.CreateRollSuper(stream, client, x, 8, 13);
        //                            client.Player.MyJiangHu.ApplayNewStar(client);
        //                        }
        //                    }
        //                    for (byte x = 1; x < 10; x++)
        //                    {
        //                        if (client.Player.MyJiangHu != null)
        //                        {
        //                            client.Player.MyJiangHu.CreateRollSuper(stream, client, x, 9, 1);
        //                            client.Player.MyJiangHu.ApplayNewStar(client);
        //                        }
        //                    }
        //                    #endregion
        //                }
        //                if (client.ModeAtributes == 203)
        //                {
        //                    #region JiangSuper
        //                    for (byte x = 1; x < 10; x++)
        //                    {
        //                        if (client.Player.MyJiangHu != null)
        //                        {
        //                            client.Player.MyJiangHu.CreateRollSuper(stream, client, x, 1, 3);
        //                            client.Player.MyJiangHu.ApplayNewStar(client);
        //                        }
        //                    }
        //                    for (byte x = 1; x < 10; x++)
        //                    {
        //                        if (client.Player.MyJiangHu != null)
        //                        {
        //                            client.Player.MyJiangHu.CreateRollSuper(stream, client, x, 2, 12);
        //                            client.Player.MyJiangHu.ApplayNewStar(client);
        //                        }
        //                    }
        //                    for (byte x = 1; x < 10; x++)
        //                    {
        //                        if (client.Player.MyJiangHu != null)
        //                        {
        //                            client.Player.MyJiangHu.CreateRollSuper(stream, client, x, 3, 3);
        //                            client.Player.MyJiangHu.ApplayNewStar(client);
        //                        }
        //                    }
        //                    for (byte x = 1; x < 10; x++)
        //                    {
        //                        if (client.Player.MyJiangHu != null)
        //                        {
        //                            client.Player.MyJiangHu.CreateRollSuper(stream, client, x, 4, 3);
        //                            client.Player.MyJiangHu.ApplayNewStar(client);
        //                        }
        //                    }
        //                    for (byte x = 1; x < 10; x++)
        //                    {
        //                        if (client.Player.MyJiangHu != null)
        //                        {
        //                            client.Player.MyJiangHu.CreateRollSuper(stream, client, x, 5, 1);
        //                            client.Player.MyJiangHu.ApplayNewStar(client);
        //                        }
        //                    }
        //                    for (byte x = 1; x < 10; x++)
        //                    {
        //                        if (client.Player.MyJiangHu != null)
        //                        {
        //                            client.Player.MyJiangHu.CreateRollSuper(stream, client, x, 6, 12);
        //                            client.Player.MyJiangHu.ApplayNewStar(client);
        //                        }
        //                    }
        //                    for (byte x = 1; x < 10; x++)
        //                    {
        //                        if (client.Player.MyJiangHu != null)
        //                        {
        //                            client.Player.MyJiangHu.CreateRollSuper(stream, client, x, 7, 3);
        //                            client.Player.MyJiangHu.ApplayNewStar(client);
        //                        }
        //                    }
        //                    for (byte x = 1; x < 10; x++)
        //                    {
        //                        if (client.Player.MyJiangHu != null)
        //                        {
        //                            client.Player.MyJiangHu.CreateRollSuper(stream, client, x, 8, 3);
        //                            client.Player.MyJiangHu.ApplayNewStar(client);
        //                        }
        //                    }
        //                    for (byte x = 1; x < 10; x++)
        //                    {
        //                        if (client.Player.MyJiangHu != null)
        //                        {
        //                            client.Player.MyJiangHu.CreateRollSuper(stream, client, x, 9, 1);
        //                            client.Player.MyJiangHu.ApplayNewStar(client);
        //                        }
        //                    }
        //                    #endregion
        //                }
        //                if (client.ModeAtributes == 204)
        //                {
        //                    #region JiangSuper
        //                    for (byte x = 1; x < 10; x++)
        //                    {
        //                        if (client.Player.MyJiangHu != null)
        //                        {
        //                            client.Player.MyJiangHu.CreateRollSuper(stream, client, x, 1, 14);
        //                            client.Player.MyJiangHu.ApplayNewStar(client);
        //                        }
        //                    }
        //                    for (byte x = 1; x < 10; x++)
        //                    {
        //                        if (client.Player.MyJiangHu != null)
        //                        {
        //                            client.Player.MyJiangHu.CreateRollSuper(stream, client, x, 2, 8);
        //                            client.Player.MyJiangHu.ApplayNewStar(client);
        //                        }
        //                    }
        //                    for (byte x = 1; x < 10; x++)
        //                    {
        //                        if (client.Player.MyJiangHu != null)
        //                        {
        //                            client.Player.MyJiangHu.CreateRollSuper(stream, client, x, 3, 14);
        //                            client.Player.MyJiangHu.ApplayNewStar(client);
        //                        }
        //                    }
        //                    for (byte x = 1; x < 10; x++)
        //                    {
        //                        if (client.Player.MyJiangHu != null)
        //                        {
        //                            client.Player.MyJiangHu.CreateRollSuper(stream, client, x, 4, 14);
        //                            client.Player.MyJiangHu.ApplayNewStar(client);
        //                        }
        //                    }
        //                    for (byte x = 1; x < 10; x++)
        //                    {
        //                        if (client.Player.MyJiangHu != null)
        //                        {
        //                            client.Player.MyJiangHu.CreateRollSuper(stream, client, x, 5, 1);
        //                            client.Player.MyJiangHu.ApplayNewStar(client);
        //                        }
        //                    }
        //                    for (byte x = 1; x < 10; x++)
        //                    {
        //                        if (client.Player.MyJiangHu != null)
        //                        {
        //                            client.Player.MyJiangHu.CreateRollSuper(stream, client, x, 6, 8);
        //                            client.Player.MyJiangHu.ApplayNewStar(client);
        //                        }
        //                    }
        //                    for (byte x = 1; x < 10; x++)
        //                    {
        //                        if (client.Player.MyJiangHu != null)
        //                        {
        //                            client.Player.MyJiangHu.CreateRollSuper(stream, client, x, 7, 14);
        //                            client.Player.MyJiangHu.ApplayNewStar(client);
        //                        }
        //                    }
        //                    for (byte x = 1; x < 10; x++)
        //                    {
        //                        if (client.Player.MyJiangHu != null)
        //                        {
        //                            client.Player.MyJiangHu.CreateRollSuper(stream, client, x, 8, 14);
        //                            client.Player.MyJiangHu.ApplayNewStar(client);
        //                        }
        //                    }
        //                    for (byte x = 1; x < 10; x++)
        //                    {
        //                        if (client.Player.MyJiangHu != null)
        //                        {
        //                            client.Player.MyJiangHu.CreateRollSuper(stream, client, x, 9, 1);
        //                            client.Player.MyJiangHu.ApplayNewStar(client);
        //                        }
        //                    }
        //                    #endregion
        //                }
        //                #endregion

        //                #region Chi(Mode)
        //                #region [MaxHP]+[P-Attack]+[PStrike]+[Immunity]
        //                if (client.ModeAtributes == 201)
        //                {

        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 1 7", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 2 3", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 3 1", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 1 7", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 2 3", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 3 1", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 1 7", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 2 3", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 3 1", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 1 7", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 2 3", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 3 1", 0, 0));


        //                }
        //                #endregion

        //                #region [MaxHP]+[P-Attack]+[Break]+[Immunity]
        //                if (client.ModeAtributes == 202)
        //                {

        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 1 7", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 2 3", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 3 4", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 1 7", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 2 3", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 3 4", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 1 7", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 2 3", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 3 4", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 1 7", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 2 3", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 3 4", 0, 0));
        //                }
        //                #endregion

        //                #region [MaxHP]+[M-Attack]+[M-Strike]+[Immunity]
        //                if (client.ModeAtributes == 203)
        //                {

        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 1 8", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 2 3", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 3 2", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 1 8", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 2 3", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 3 2", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 1 8", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 2 3", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 3 2", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 1 8", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 2 3", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 3 2", 0, 0));

        //                }

        //                #endregion

        //                #region [MaxHP]+[P-Damge]+[AntiBreak]+[Immunity]
        //                if (client.ModeAtributes == 204)
        //                {   

        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 1 5", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 2 3", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 3 12", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 1 5", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 2 3", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 3 12", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 1 5", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 2 3", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 3 12", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 1 5", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 2 3", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 3 12", 0, 0));

        //                }

        //                #endregion
        //                #endregion

        //                #region UpdateChi
        //                Role.Instance.Chi.ComputeStatus(client.Player.MyChi);
        //                client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
        //                MsgChiInfo.MsgHandleChi.SendInfo(client, MsgChiInfo.Action.Send);
        //                MsgChiInfo.MsgHandleChi.SendInfo(client, MsgChiInfo.Action.Upgrade);
        //                Pool.ChiRanking.Upadte(Pool.ChiRanking.Dragon, client.Player.MyChi.Dragon);
        //                Pool.ChiRanking.Upadte(Pool.ChiRanking.Dragon, client.Player.MyChi.Phoenix);
        //                Pool.ChiRanking.Upadte(Pool.ChiRanking.Dragon, client.Player.MyChi.Tiger);
        //                Pool.ChiRanking.Upadte(Pool.ChiRanking.Dragon, client.Player.MyChi.Turtle);
        //                #endregion

        //                #endregion
        //                client.Player.MyJiangHu.DisableJiang(client);
        //                if (Database.AtributesStatus.IsTrojan(client.Player.Class))
        //                {
        //                    client.Inventory.AddPrize(stream, 120029, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 150039, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 160039, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 130009, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 118009, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 420029, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 410029, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 480029, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 300000, 1, 12, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true); // horse
        //                    client.Inventory.AddPrize(stream, 204009, 1, 12, 0, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperGloryGem, true); // wing
        //                    client.Inventory.AddPrize(stream, 203009, 1, 12, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true); // crop
        //                    client.Inventory.AddPrize(stream, 202009, 1, 12, 0, 0, Role.Flags.Gem.SuperGloryGem, Role.Flags.Gem.SuperGloryGem, true); // twoer
        //                    client.Inventory.AddPrize(stream, 201009, 1, 12, 0, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperThunderGem, true); // fan
        //                }
        //                if (Database.AtributesStatus.IsWarrior(client.Player.Class))
        //                {

        //                    client.Inventory.AddPrize(stream, 120029, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 150039, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 160039, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 131009, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 111009, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 561029, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 560029, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 511029, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 900009, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 624029, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.Add(stream, 624029, 1, 8, 5, 230, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 300000, 1, 12, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true); // horse
        //                    client.Inventory.AddPrize(stream, 204009, 1, 12, 0, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperGloryGem, true); // wing
        //                    client.Inventory.AddPrize(stream, 203009, 1, 12, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true); // crop
        //                    client.Inventory.AddPrize(stream, 202009, 1, 12, 0, 0, Role.Flags.Gem.SuperGloryGem, Role.Flags.Gem.SuperGloryGem, true); // twoer
        //                    client.Inventory.AddPrize(stream, 201009, 1, 12, 0, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperThunderGem, true); // fan
        //                }
        //                if (Database.AtributesStatus.IsArcher(client.Player.Class))
        //                {

        //                    client.Inventory.AddPrize(stream, 120029, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 150039, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 160039, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 613019, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 613019, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 133009, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 113009, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);

        //                    client.Inventory.AddPrize(stream, 300000, 1, 12, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true); // horse
        //                    client.Inventory.AddPrize(stream, 204009, 1, 12, 0, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperGloryGem, true); // wing
        //                    client.Inventory.AddPrize(stream, 203009, 1, 12, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true); // crop
        //                    client.Inventory.AddPrize(stream, 202009, 1, 12, 0, 0, Role.Flags.Gem.SuperGloryGem, Role.Flags.Gem.SuperGloryGem, true); // twoer
        //                    client.Inventory.AddPrize(stream, 201009, 1, 12, 0, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperThunderGem, true); // fan
        //                }
        //                if (Database.AtributesStatus.IsNinja(client.Player.Class))
        //                {

        //                    client.Inventory.AddPrize(stream, 120029, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 150039, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 160039, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 601029, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 601029, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 135009, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 123009, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);

        //                    client.Inventory.AddPrize(stream, 300000, 1, 12, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true); // horse
        //                    client.Inventory.AddPrize(stream, 204009, 1, 12, 0, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperGloryGem, true); // wing
        //                    client.Inventory.AddPrize(stream, 203009, 1, 12, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true); // crop
        //                    client.Inventory.AddPrize(stream, 202009, 1, 12, 0, 0, Role.Flags.Gem.SuperGloryGem, Role.Flags.Gem.SuperGloryGem, true); // twoer
        //                    client.Inventory.AddPrize(stream, 201009, 1, 12, 0, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperThunderGem, true); // fan
        //                }
        //                if (Database.AtributesStatus.IsMonk(client.Player.Class))
        //                {

        //                    client.Inventory.AddPrize(stream, 120029, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 150039, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 160039, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 610029, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 610029, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 136009, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 143009, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);

        //                    client.Inventory.AddPrize(stream, 300000, 1, 12, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true); // horse
        //                    client.Inventory.AddPrize(stream, 204009, 1, 12, 0, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperGloryGem, true); // wing
        //                    client.Inventory.AddPrize(stream, 203009, 1, 12, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true); // crop
        //                    client.Inventory.AddPrize(stream, 202009, 1, 12, 0, 0, Role.Flags.Gem.SuperGloryGem, Role.Flags.Gem.SuperGloryGem, true); // twoer
        //                    client.Inventory.AddPrize(stream, 201009, 1, 12, 0, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperThunderGem, true); // fan
        //                }
        //                if (Database.AtributesStatus.IsPirate(client.Player.Class))
        //                {

        //                    client.Inventory.AddPrize(stream, 120029, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 150039, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 160039, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 611029, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 612029, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 139009, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 144009, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);

        //                    client.Inventory.AddPrize(stream, 300000, 1, 12, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true); // horse
        //                    client.Inventory.AddPrize(stream, 204009, 1, 12, 0, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperGloryGem, true); // wing
        //                    client.Inventory.AddPrize(stream, 203009, 1, 12, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true); // crop
        //                    client.Inventory.AddPrize(stream, 202009, 1, 12, 0, 0, Role.Flags.Gem.SuperGloryGem, Role.Flags.Gem.SuperGloryGem, true); // twoer
        //                    client.Inventory.AddPrize(stream, 201009, 1, 12, 0, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperThunderGem, true); // fan
        //                }
        //                if (Database.AtributesStatus.IsLee(client.Player.Class))
        //                {

        //                    client.Inventory.AddPrize(stream, 120029, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 150039, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 160039, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 617029, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 617029, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 138009, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 148009, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);

        //                    client.Inventory.AddPrize(stream, 300000, 1, 12, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true); // horse
        //                    client.Inventory.AddPrize(stream, 204009, 1, 12, 0, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperGloryGem, true); // wing
        //                    client.Inventory.AddPrize(stream, 203009, 1, 12, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true); // crop
        //                    client.Inventory.AddPrize(stream, 202009, 1, 12, 0, 0, Role.Flags.Gem.SuperGloryGem, Role.Flags.Gem.SuperGloryGem, true); // twoer
        //                    client.Inventory.AddPrize(stream, 201009, 1, 12, 0, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperThunderGem, true); // fan
        //                }
        //                if (Database.AtributesStatus.IsTaoist(client.Player.Class))
        //                {

        //                    client.Inventory.AddPrize(stream, 152019, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 121029, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 160039, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 421029, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 134009, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 114009, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);

        //                    client.Inventory.AddPrize(stream, 300000, 1, 12, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true); // horse
        //                    client.Inventory.AddPrize(stream, 204009, 1, 12, 0, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperGloryGem, true); // wing
        //                    client.Inventory.AddPrize(stream, 203009, 1, 12, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true); // crop
        //                    client.Inventory.AddPrize(stream, 202009, 1, 12, 0, 0, Role.Flags.Gem.SuperGloryGem, Role.Flags.Gem.SuperGloryGem, true); // twoer
        //                    client.Inventory.AddPrize(stream, 201009, 1, 12, 0, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperThunderGem, true); // fan
        //                }
        //                if (Database.AtributesStatus.IsWindWalker(client.Player.Class))
        //                {

        //                    client.Inventory.AddPrize(stream, 120029, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 150039, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 160039, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 626029, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 626029, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 101009, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 170009, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);

        //                    client.Inventory.AddPrize(stream, 300000, 1, 12, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true); // horse
        //                    client.Inventory.AddPrize(stream, 204009, 1, 12, 0, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperGloryGem, true); // wing
        //                    client.Inventory.AddPrize(stream, 203009, 1, 12, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true); // crop
        //                    client.Inventory.AddPrize(stream, 202009, 1, 12, 0, 0, Role.Flags.Gem.SuperGloryGem, Role.Flags.Gem.SuperGloryGem, true); // twoer
        //                    client.Inventory.AddPrize(stream, 201009, 1, 12, 0, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperThunderGem, true); // fan
        //                }
        //                if (Database.AtributesStatus.IsThunderStriker(client.Player.Class))
        //                {

        //                    client.Inventory.AddPrize(stream, 120029, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 150039, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 160039, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 680029, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 681029, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 102009, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);
        //                    client.Inventory.AddPrize(stream, 146009, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);

        //                    client.Inventory.AddPrize(stream, 300000, 1, 12, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true); // horse
        //                    client.Inventory.AddPrize(stream, 204009, 1, 12, 0, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperGloryGem, true); // wing
        //                    client.Inventory.AddPrize(stream, 203009, 1, 12, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true); // crop
        //                    client.Inventory.AddPrize(stream, 202009, 1, 12, 0, 0, Role.Flags.Gem.SuperGloryGem, Role.Flags.Gem.SuperGloryGem, true); // twoer
        //                    client.Inventory.AddPrize(stream, 201009, 1, 12, 0, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperThunderGem, true); // fan
        //                }
        //                client.Teleport(410, 354, 1002);
        //                break;

        //            }

        //    }
        //}
        [NpcAttribute(NpcID.EU)]
        public static void EU(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ServerInfo = Database.GroupServerList.GetServer(1);
            if (ServerInfo == null)
                return;

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I`m the keeper of [" + ServerInfo.Name + "] Server. You`ll find a keeper like me in each Server.")
                            .AddText("~If you want to visit another server, you need to register with me. I can also help you")
                            .AddText("~return to your original Server. Please carefully learn about the rules before taking the trip.");
                        data.AddOption("Take~me~to~server~[" + ServerInfo.Name + "].", 1)
                            .AddOption("How~to~travel~across~server?", 10)
                            .AddOption("What~should~I~pay~attention~to?", 11)
                           .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Player.OnTransform)
                        {
                            client.Player.TransformInfo.FinishTransform();
                        }
                        if (client.Player.ServerID == ServerInfo.ID)
                        {
                            client.Socket.Disconnect();
                        }
                        else
                        {
                            client.Send(stream.ConnectionInfoCreate(1, ServerInfo.ID));
                        }
                        break;
                    }
                case 10:
                    {
                        data.AddText("If you want to go to another server, talk to the Cross-server Keeper in your server to enter the Realm,")
                            .AddText("~and then register with the Cross-server Keeper of the destination server in the Realm. If return,")
                            .AddText("~you`ll take the same procedures. First, go transit in the Realm. Then, find the Keeper of your original server")
                            .AddText("~to get back. Please make sure you`ve learned all rules and matters before taking a cross-server trip.");
                        data.AddOption("What~should~I~pay~attention~to?", 11)
                           .AddAvatar(7).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        data.AddText("While visiting a new server, your information about character level, rebirth times, equipment, nobility,")
                             .AddText("~mentor and apprentice, and bonus BP of guild and clan will also be registered in the server, however,")
                             .AddText("~only chat and PK functions are available now. The chat channels include Talk, Whisper, Team, Guild,")
                             .AddText("~Clan, Friend, Service, World and Ally. If you were killed in the Realm, you`ll also revive there.");
                        data.AddOption("What~functions~are~unavailable?", 12)
                           .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 12:
                    {
                        data.AddText("In the Realm, you`re not able to use the functions related to money, item trade, forging,")
                             .AddText("~social contact, Arena, Mentor gains, welfare, enlightenment, sending flowers and all PvP")
                             .AddText("~matches. Things will get right after I send you back to your original Server.");
                        data.AddOption("I~see.", 255)
                           .AddAvatar(7).FinalizeDialog();

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.PowerBoosterPack)]
        public static void PowerBoosterPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.Text("STR_ID_tRuneDecompose[3319466][Text111]@@\n");
                        data.Option("STR_ID_tRuneDecompose[3319466][Option111]@@\n", 1);
                        data.Option("STR_ID_tRuneDecompose[3319466][Option112]@@\n", 2);
                        data.Option("STR_ID_tRuneDecompose[3319466][Option113]@@\n", 3);
                        data.Option("STR_ID_tRuneDecompose[3319466][Option114]@@\n", 4);
                        data.AddAvatar(0).FinalizeDialog();
                        break;

                    }
                case 1:
                    {
                        data.Text("STR_ID_tRuneDecompose[3319466][Text211]@@\n");
                        data.Option("STR_ID_tRuneDecompose[3319466][Option211]@@\n", 5);
                        data.Option("STR_ID_tRuneDecompose[3319466][Option212]@@\n", 255);
                        data.AddAvatar(0).FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.Contain(3319466, 1) || client.Inventory.Contain(3319466, 1, 1))
                        {
                            client.Inventory.RemoveStackItem(3319466, 1, stream);
                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 10000;
                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                            client.CreateBoxDialog("STR_ID_tRuneDecompose[3319466][Msg][Award][1]@@");
                        }


                        break;
                    }
                case 2:
                    {
                        data.Text("STR_ID_tRuneDecompose[3319466][Text221]@@\n");
                        data.Option("STR_ID_tRuneDecompose[3319466][Option221]@@\n", 6);
                        data.Option("STR_ID_tRuneDecompose[3319466][Option212]@@\n", 255);
                        data.AddAvatar(0).FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.Contain(3319466, 1) || client.Inventory.Contain(3319466, 1, 1))
                        {
                            client.Inventory.RemoveStackItem(3319466, 1, stream);
                            client.Player.InnerPower.AddPotency(stream, client, 10000);
                            client.CreateBoxDialog("STR_ID_tRuneDecompose[3319466][Msg][Award][2]@@");
                        }
                        break;
                    }
                case 3:
                    {
                        data.Text("STR_ID_tRuneDecompose[3319466][Text231]@@\n");
                        data.Option("STR_ID_tRuneDecompose[3319466][Option231]@@\n", 7);
                        data.Option("STR_ID_tRuneDecompose[3319466][Option212]@@\n", 255);
                        data.AddAvatar(0).FinalizeDialog();
                        break;
                    }
                case 7:
                    {
                        if (client.Inventory.Contain(3319466, 1) || client.Inventory.Contain(3319466, 1, 1))
                        {
                            client.Inventory.RemoveStackItem(3319466, 1, stream);
                            client.Player.SubClass.AddStudyPoints(client, 10000, stream);
                            client.CreateBoxDialog("STR_ID_tRuneDecompose[3319466][Msg][Award][3]@@");
                        }
                        break;
                    }
                case 4:
                    {
                        data.Text("STR_ID_tRuneDecompose[3319466][Text241]@@\n");
                        data.Option("STR_ID_tRuneDecompose[3319466][Option241]@@\n", 8);
                        data.Option("STR_ID_tRuneDecompose[3319466][Option212]@@\n", 255);
                        data.AddAvatar(0).FinalizeDialog();
                        break;
                    }
                case 8:
                    {

                        if (client.Inventory.Contain(3319466, 1) || client.Inventory.Contain(3319466, 1, 1))
                        {
                            client.Inventory.RemoveStackItem(3319466, 1, stream);
                            client.Inventory.AddItemWitchStack(3324226, 0, 1, stream);
                            client.CreateBoxDialog("STR_ID_tRuneDecompose[3319466][Msg][Award][4]@@");
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.LuxuryRelicChest)]
        public static void LuxuryRelicChest(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.Text("STR_ID_tTreasureArea[3322219][Text111]@@\n");
                        data.Text("STR_ID_tTreasureArea[3322219][Text112]@@\n");
                        data.Option("STR_ID_tTreasureArea[3322219][Option111]@@\n", 1);
                        data.Option("STR_ID_tTreasureArea[3322219][Option112]@@\n", 2);
                        data.Option("STR_ID_tTreasureArea[3322219][Option113]@@\n", 3);
                        data.Option("STR_ID_tTreasureArea[3322219][Option114]@@\n", 4);
                        data.Option("STR_ID_tTreasureArea[3322219][Option115]@@\n", 4);
                        data.AddAvatar(0).FinalizeDialog();
                        break;

                    }
                case 1:
                    {
                        data.Text(" Are you sure you want to select Western~Mirror? You can`t change it after you make a choice.\n");
                        data.Option("STR_ID_tTreasureArea[3322219][Option211]@@\n", 6);
                        data.Option("STR_ID_tTreasureArea[3322219][Option212]@@\n", 255);
                        data.AddAvatar(0).FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.Contain(3322219, 1) || client.Inventory.Contain(3322219, 1, 1))
                        {
                            client.Inventory.RemoveStackItem(3322219, 1, stream);
                            client.Inventory.AddRandomRelicL5(stream, 4100001);
                        }
                        break;
                    }
                case 2:
                    {
                        data.Text(" Are you sure you want to select Imperial~Tripod? You can`t change it after you make a choice.\n");
                        data.Option("STR_ID_tTreasureArea[3322219][Option211]@@\n", 7);
                        data.Option("STR_ID_tTreasureArea[3322219][Option212]@@\n", 255);
                        data.AddAvatar(0).FinalizeDialog();
                        break;
                    }
                case 7:
                    {
                        if (client.Inventory.Contain(3322219, 1) || client.Inventory.Contain(3322219, 1, 1))
                        {
                            client.Inventory.RemoveStackItem(3322219, 1, stream);
                            client.Inventory.AddRandomRelicL5(stream, 4100002);
                        }
                        break;
                    }
                case 3:
                    {
                        data.Text(" Are you sure you want to select Beads~of~Calm~Sea? You can`t change it after you make a choice.\n");
                        data.Option("STR_ID_tTreasureArea[3322219][Option211]@@\n", 8);
                        data.Option("STR_ID_tTreasureArea[3322219][Option212]@@\n", 255);
                        data.AddAvatar(0).FinalizeDialog();
                        break;
                    }
                case 8:
                    {
                        if (client.Inventory.Contain(3322219, 1) || client.Inventory.Contain(3322219, 1, 1))
                        {
                            client.Inventory.RemoveStackItem(3322219, 1, stream);
                            client.Inventory.AddRandomRelicL5(stream, 4100003);
                        }
                        break;
                    }
                case 4:
                    {
                        data.Text(" Are you sure you want to select Heartless~Seal? You can`t change it after you make a choice.\n");
                        data.Option("STR_ID_tTreasureArea[3322219][Option211]@@\n", 9);
                        data.Option("STR_ID_tTreasureArea[3322219][Option212]@@\n", 255);
                        data.AddAvatar(0).FinalizeDialog();
                        break;
                    }
                case 9:
                    {

                        if (client.Inventory.Contain(3322219, 1) || client.Inventory.Contain(3322219, 1, 1))
                        {
                            client.Inventory.RemoveStackItem(3322219, 1, stream);
                            client.Inventory.AddRandomRelicL5(stream, 4100004);
                        }
                        break;
                    }
                case 5:
                    {
                        data.Text(" Are you sure you want to select Bell~of~Doom? You can`t change it after you make a choice.\n");
                        data.Option("STR_ID_tTreasureArea[3322219][Option211]@@\n", 10);
                        data.Option("STR_ID_tTreasureArea[3322219][Option212]@@\n", 255);
                        data.AddAvatar(0).FinalizeDialog();
                        break;
                    }
                case 10:
                    {

                        if (client.Inventory.Contain(3322219, 1) || client.Inventory.Contain(3322219, 1, 1))
                        {
                            client.Inventory.RemoveStackItem(3322219, 1, stream);
                            client.Inventory.AddRandomRelicL5(stream, 4100005);
                        }
                        break;
                    }
            }


        }
        [NpcAttribute(NpcID.RuneStarStonePack)]
        public static void RuneStarStonePack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~one~from~1~Bright~Star~Ticket/1~Sigil~Box~Scrap/10~Rune~Essences.");
                        dialog.AddOption("1~Bright~Star~Ticket", 1);
                        dialog.AddOption("1~Sigil~Box~Scrap", 2);
                        dialog.AddOption("10~Rune~Essences", 3);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }

                case 1:
                    {
                        // Security fix: verify the consumed item is the Rune Star Stone Pack (3331667)
                        // and not an arbitrary client-supplied id.
                        if (id != 3331667) break;
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3200726, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false, Role.Flags.ItemEffect.None, true, "");
                        }
                        break;
                    }
                case 2:
                    {
                        // Security fix: verify the consumed item is the Rune Star Stone Pack (3331667).
                        if (id != 3331667) break;
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3327202, 0, 1, stream);
                        }
                        break;
                    }
                case 3:
                    {
                        // Security fix: verify the consumed item is the Rune Star Stone Pack (3331667).
                        if (id != 3331667) break;
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(4060001, 0, 10, stream);
                        }
                        break;
                    }


            }
        }
        [NpcAttribute(NpcID.RareSteedPack)]
        public static void RareSteedPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain(3318571, 1))
            {
                client.CreateBoxDialog("Sorry, you don`t have a +6 Rare Steed Pack");
                return;
            }

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello. You can chose a horse: SpitfireSteed, FrostbiteSteed, BlazehoofSteed, SpottedSteed or Zebra.")
                            .AddOption("+6~Spitfire~Steed", 1)
                       .AddOption("+6~Frostbite~Steed", 2)
                       .AddOption("+6~Blazehoof~Steed", 3)
                       .AddOption("+6~Spotted~Steed", 4)
                       .AddOption("+6~Zebra", 5)
                       .AddOption("Nevermind", 255)
                       .FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        client.Inventory.Remove(3318571, 1, stream);
                        client.Inventory.AddSteed(stream, 300000, 1, 6, false, 201, 135, 187);

                        break;
                    }
                case 2:
                    {
                        client.Inventory.Remove(3318571, 1, stream);
                        client.Inventory.AddSteed(stream, 300000, 1, 6, false, 184, 63, 62);

                        break;
                    }
                case 3:
                    {
                        client.Inventory.Remove(3318571, 1, stream);
                        client.Inventory.AddSteed(stream, 300000, 1, 6, false, 137, 156, 148);

                        break;
                    }
                case 4:
                    {
                        client.Inventory.Remove(3318571, 1, stream);
                        client.Inventory.AddSteed(stream, 300000, 1, 6, false, 122, 134, 147);

                        break;
                    }
                case 5:
                    {
                        client.Inventory.Remove(3318571, 1, stream);
                        client.Inventory.AddSteed(stream, 300000, 1, 6, false, 46, 39, 142);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.RareSteedPack8)]
        public static void RareSteedPack8(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain(3318567, 1))
            {
                client.CreateBoxDialog("Sorry, you don`t have a +8 Rare Steed Pack");
                return;
            }

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello. You can chose a horse: SpitfireSteed, FrostbiteSteed, BlazehoofSteed, SpottedSteed or Zebra.")
                            .AddOption("+8~Spitfire~Steed", 1)
                       .AddOption("+8~Frostbite~Steed", 2)
                       .AddOption("+8~Blazehoof~Steed", 3)
                       .AddOption("+8~Spotted~Steed", 4)
                       .AddOption("+8~Zebra", 5)
                       .AddOption("Nevermind", 255)
                       .FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        client.Inventory.Remove(3318567, 1, stream);
                        client.Inventory.AddSteed(stream, 300000, 1, 8, false, 201, 135, 187);

                        break;
                    }
                case 2:
                    {
                        client.Inventory.Remove(3318567, 1, stream);
                        client.Inventory.AddSteed(stream, 300000, 1, 8, false, 184, 63, 62);

                        break;
                    }
                case 3:
                    {
                        client.Inventory.Remove(3318567, 1, stream);
                        client.Inventory.AddSteed(stream, 300000, 1, 8, false, 137, 156, 148);

                        break;
                    }
                case 4:
                    {
                        client.Inventory.Remove(3318567, 1, stream);
                        client.Inventory.AddSteed(stream, 300000, 1, 8, false, 122, 134, 147);

                        break;
                    }
                case 5:
                    {
                        client.Inventory.Remove(3318567, 1, stream);
                        client.Inventory.AddSteed(stream, 300000, 1, 8, false, 46, 39, 142);

                        break;
                    }
            }
        }
        #region ItemLottery
        [NpcAttribute(NpcID.LilyCard)]
        public static void LilyCard(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain(3331782, 1))
            {
                client.SendSysMesage("Check your inventory!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                return;
            }
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Every Lily Card can be exchanged for 1 Lily or 5 Chi Points. Which reward would you like to exchange for now?");
                        dialog.AddOption("1~Lilies.", 1);
                        dialog.AddOption("3~Lilies.", 2);
                        dialog.AddOption("9~Lilies.", 3);
                        dialog.AddOption("99~Lilies.", 4);
                        dialog.AddOption("999~Lilies.", 5);
                        dialog.AddOption("Exchange Lily Card for Chi Points.", 6);
                        dialog.AddAvatar(781);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(3331782, 1))
                        {
                            if (client.Inventory.Add(stream, 752001, 1))
                                client.SendSysMesage("You have received 1 Lily in your inventory.", MsgMessage.ChatMode.System);
                            {
                                client.Inventory.Remove(3331782, 1, stream);
                            }
                        }
                        else
                        {
                            dialog.Text("Sorry, you do not have 1 Lily Card in your inventory to be able to obtain 1 Lily.");

                            dialog.Option("Okay.", 255).AddAvatar(781).FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(3331782, 3))
                        {
                            if (client.Inventory.Add(stream, 752003, 1))
                                client.SendSysMesage("You have received 3 Lilies in your inventory.", MsgMessage.ChatMode.System);
                            {
                                client.Inventory.Remove(3331782, 3, stream);
                            }
                        }
                        else
                        {
                            dialog.Text("Sorry, you do not have 3 Lily Card in your inventory to be able to obtain 3 Lilies.");

                            dialog.Option("Okay.", 255).AddAvatar(781).FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(3331782, 9))
                        {
                            if (client.Inventory.Add(stream, 752009, 1))
                                client.SendSysMesage("You have received 9 Lilies in your inventory.", MsgMessage.ChatMode.System);
                            {
                                client.Inventory.Remove(3331782, 9, stream);
                            }
                        }
                        else
                        {
                            dialog.Text("Sorry, you do not have 9 Lily Card in your inventory to be able to obtain 9 Lilies.");

                            dialog.Option("Okay.", 255).AddAvatar(781).FinalizeDialog();
                        }
                        break;
                    }
                case 4:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(3331782, 99))
                        {
                            if (client.Inventory.Add(stream, 752099, 1))
                                client.SendSysMesage("You have received 99 Lilies in your inventory.", MsgMessage.ChatMode.System);
                            {
                                client.Inventory.Remove(3331782, 99, stream);
                            }
                        }
                        else
                        {
                            dialog.Text("Sorry, you do not have 99 Lily Card in your inventory to be able to obtain 99 Lilies.");

                            dialog.Option("Okay.", 255).AddAvatar(781).FinalizeDialog();
                        }
                        break;
                    }
                case 5:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(3331782, 999))
                        {
                            if (client.Inventory.Add(stream, 752999, 1))
                                client.SendSysMesage("You have received 999 Lilies in your inventory.", MsgMessage.ChatMode.System);
                            {
                                client.Inventory.Remove(3331782, 999, stream);
                            }
                        }
                        else
                        {
                            dialog.Text("Sorry, you do not have 999 Lily Card in your inventory to be able to obtain 999 Lilies.");

                            dialog.Option("Okay.", 255).AddAvatar(781).FinalizeDialog();
                        }
                        break;
                    }
                case 6:
                    {
                        dialog.AddText("You Can Exchange Lily Cards for Chi Points and You Well Can Exchange up to 1000 Lily Card The one from Lily Card gives You 5 Chi Points. Do You Want Exchange Now?");
                        dialog.AddOption("Exchange 1 Lily Card.", 10);
                        dialog.AddOption("Exchange 5 Lily Card.", 11);
                        dialog.AddOption("Exchange 10 Lily Card.", 12);
                        dialog.AddOption("Exchange 50 Lily Card.", 13);
                        dialog.AddOption("Exchange 100 Lily Card.", 14);
                        dialog.AddOption("Exchange 500 Lily Card.", 15);
                        dialog.AddOption("Exchange 1000 Lily Card.", 16);
                        dialog.AddAvatar(781);//done
                        dialog.FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        // Security fix: verify the consumed item is a Lily Card (3331782) and not an arbitrary client-supplied id.
                        if (id != 3331782) break;
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 5;
                            client.SendSysMesage("Received 5 Chi Point(s).", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                        }
                        else
                        {
                            dialog.Text("Sorry, you do not have 1 Lily Card in your inventory to be able to obtain 5 Chi Points.");

                            dialog.Option("Okay.", 255).AddAvatar(781).FinalizeDialog();
                        }
                        break;
                    }
                case 11:
                    {
                        if (id != 3331782) break; // Security fix: require Lily Card (3331782)
                        if (client.Inventory.Remove(id, 5, stream))
                        {
                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 25;
                            client.SendSysMesage("Received 25 Chi Point(s).", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                        }
                        else
                        {
                            dialog.Text("Sorry, you do not have 5 Lily Card in your inventory to be able to obtain 25 Chi Points.");

                            dialog.Option("Okay.", 255).AddAvatar(781).FinalizeDialog();
                        }
                        break;
                    }
                case 12:
                    {
                        if (id != 3331782) break; // Security fix: require Lily Card (3331782)
                        if (client.Inventory.Remove(id, 10, stream))
                        {
                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 50;
                            client.SendSysMesage("Received 50 Chi Point(s).", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                        }
                        else
                        {
                            dialog.Text("Sorry, you do not have 10 Lily Card in your inventory to be able to obtain 50 Chi Points.");

                            dialog.Option("Okay.", 255).AddAvatar(781).FinalizeDialog();
                        }
                        break;
                    }
                case 13:
                    {
                        if (id != 3331782) break; // Security fix: require Lily Card (3331782)
                        if (client.Inventory.Remove(id, 50, stream))
                        {
                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 250;
                            client.SendSysMesage("Received 250 Chi Point(s).", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                        }
                        else
                        {
                            dialog.Text("Sorry, you do not have 50 Lily Card in your inventory to be able to obtain 250 Chi Points.");

                            dialog.Option("Okay.", 255).AddAvatar(781).FinalizeDialog();
                        }
                        break;
                    }
                case 14:
                    {
                        if (id != 3331782) break; // Security fix: require Lily Card (3331782)
                        if (client.Inventory.Remove(id, 100, stream))
                        {
                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 500;
                            client.SendSysMesage("Received 500 Chi Point(s).", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                        }
                        else
                        {
                            dialog.Text("Sorry, you do not have 100 Lily Card in your inventory to be able to obtain 500 Chi Points.");

                            dialog.Option("Okay.", 255).AddAvatar(781).FinalizeDialog();
                        }
                        break;
                    }
                case 15:
                    {
                        if (id != 3331782) break; // Security fix: require Lily Card (3331782)
                        if (client.Inventory.Remove(id, 500, stream))
                        {
                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 2500;
                            client.SendSysMesage("Received 2500 Chi Point(s).", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                        }
                        else
                        {
                            dialog.Text("Sorry, you do not have 500 Lily Card in your inventory to be able to obtain 2500 Chi Points.");

                            dialog.Option("Okay.", 255).AddAvatar(781).FinalizeDialog();
                        }
                        break;
                    }
                case 16:
                    {
                        if (id != 3331782) break; // Security fix: require Lily Card (3331782)
                        if (client.Inventory.Remove(id, 1000, stream))
                        {
                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 5000;
                            client.SendSysMesage("Received 5000 Chi Point(s).", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                        }
                        else
                        {
                            dialog.Text("Sorry, you do not have 1000 Lily Card in your inventory to be able to obtain 5000 Chi Points.");

                            dialog.Option("Okay.", 255).AddAvatar(781).FinalizeDialog();
                        }
                        break;
                    }
                case 17:
                    {
                        if (id != 3331782) break; // Security fix: require Lily Card (3331782)
                        if (client.Inventory.Remove(id, 5000, stream))
                        {
                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 25000;
                            client.SendSysMesage("Received 25,000 Chi Point(s).", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                        }
                        break;
                    }
                case 18:
                    {
                        if (id != 3331782) break; // Security fix: require Lily Card (3331782)
                        if (client.Inventory.Remove(id, 10000, stream))
                        {
                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 50000;
                            client.SendSysMesage("Received 50,000 Chi Point(s).", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                        }
                        break;
                    }
            }
        }
        #region New Item Lottery
        public static void L1MythsoulPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        dialog.Text("Please choose the Mythsoul you want. ");
                        dialog.Option("1-handed Weapon Mythsoul.", 1);
                        dialog.Option("Shield Mythsoul.", 2);
                        dialog.Option("Ring Mythsoul.", 3);
                        dialog.Option("Armor Mythsoul.", 4);
                        dialog.Option("2-handed Weapon Mythsoul.", 5);
                        dialog.Option("Headgear Mythsoul.", 6);
                        dialog.Option("Necklace Mythsoul.", 7);
                        dialog.Option("Boot Mythsoul.", 8);
                        dialog.Avatar(93).FinalizeDialog();
                        break;

                    }
                case 1:
                    {
                        dialog.Text("Please choose the 1-handed Mythsoul you want.\n");
                        dialog.Text("Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€*Mythsoul Attribue*Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€ \n");
                        dialog.Text("       [Mythsoul]                            [Attribute].\n ");
                        dialog.Text("Venom (L1)               Uses strong venom to poison enemies.\n ");
                        dialog.Text("Elan (L1)                   Improves MP restoration.\n ");
                        dialog.Text("Ethereal (L1)              Improves Dodge. \n");
                        dialog.Text("Bloodthirst (L1)          Enhances the HP recovery. \n");
                        dialog.Text("Solid (L1)                   Improves Defense largely. \n");
                        dialog.Option("Venom (L1).", 9);
                        dialog.Option("Elan (L1).", 10);
                        dialog.Option("Ethereal (L1).", 11);
                        dialog.Option("Bloodthirst (L1).", 12);
                        dialog.Option("Solid (L1).", 13);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        dialog.Text("Please choose the Shield Mythsoul you want. ");
                        dialog.Text("Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€*Mythsoul Attribue*Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€ ");
                        dialog.Text("       [Mythsoul]                            [Attribute] ");
                        dialog.Text("Venom (L1)               Uses strong venom to poison enemies.\n ");
                        dialog.Text("Elan (L1)                   Improves MP restoration.\n ");
                        dialog.Text("Ethereal (L1)              Improves Dodge. \n");
                        dialog.Text("Bloodthirst (L1)          Enhances the HP recovery. \n");
                        dialog.Text("Solid (L1)                   Improves Defense largely. \n");
                        dialog.Option("Venom (L1).", 14);
                        dialog.Option("Elan (L1).", 15);
                        dialog.Option("Ethereal (L1).", 16);
                        dialog.Option("Bloodthirst (L1).", 17);
                        dialog.Option("Solid (L1).", 18);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        dialog.Text("Please choose the Ring Mythsoul you want. ");
                        dialog.Text("Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€*Mythsoul Attribue*Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€ ");
                        dialog.Text("       [Mythsoul]                            [Attribute] ");
                        dialog.Text("Venom (L1)               Uses strong venom to poison enemies.\n ");
                        dialog.Text("Elan (L1)                   Improves MP restoration.\n ");
                        dialog.Text("Ethereal (L1)              Improves Dodge. \n");
                        dialog.Text("Bloodthirst (L1)          Enhances the HP recovery. \n");
                        dialog.Text("Solid (L1)                   Improves Defense largely. \n");
                        dialog.Option("Venom (L1).", 19);
                        dialog.Option("Elan (L1).", 20);
                        dialog.Option("Ethereal (L1).", 21);
                        dialog.Option("Bloodthirst (L1).", 22);
                        dialog.Option("Solid (L1).", 23);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        dialog.Text("Please choose the Armor Mythsoul you want. ");
                        dialog.Text("Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€*Mythsoul Attribue*Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€ ");
                        dialog.Text("       [Mythsoul]                            [Attribute] ");
                        dialog.Text("Venom (L1)               Uses strong venom to poison enemies.\n ");
                        dialog.Text("Elan (L1)                   Improves MP restoration.\n ");
                        dialog.Text("Ethereal (L1)              Improves Dodge. \n");
                        dialog.Text("Bloodthirst (L1)          Enhances the HP recovery. \n");
                        dialog.Text("Solid (L1)                   Improves Defense largely. \n");
                        dialog.Option("Venom (L1).", 24);
                        dialog.Option("Elan (L1).", 25);
                        dialog.Option("Ethereal (L1).", 26);
                        dialog.Option("Bloodthirst (L1).", 27);
                        dialog.Option("Solid (L1).", 28);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        dialog.Text("Please choose the 2-handed Mythsoul you want. ");
                        dialog.Text("Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€*Mythsoul Attribue*Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€ ");
                        dialog.Text("       [Mythsoul]                            [Attribute] ");
                        dialog.Text("Venom (L1)               Uses strong venom to poison enemies.\n ");
                        dialog.Text("Elan (L1)                   Improves MP restoration.\n ");
                        dialog.Text("Ethereal (L1)              Improves Dodge. \n");
                        dialog.Text("Bloodthirst (L1)          Enhances the HP recovery. \n");
                        dialog.Text("Solid (L1)                   Improves Defense largely. \n");
                        dialog.Option("Venom (L1).", 29);
                        dialog.Option("Elan (L1).", 30);
                        dialog.Option("Ethereal (L1).", 31);
                        dialog.Option("Bloodthirst (L1).", 32);
                        dialog.Option("Solid (L1).", 33);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        dialog.Text("Please choose the Headgear Mythsoul you want. ");
                        dialog.Text("Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€*Mythsoul Attribue*Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€ ");
                        dialog.Text("       [Mythsoul]                            [Attribute] ");
                        dialog.Text("Venom (L1)               Uses strong venom to poison enemies.\n ");
                        dialog.Text("Elan (L1)                   Improves MP restoration.\n ");
                        dialog.Text("Ethereal (L1)              Improves Dodge. \n");
                        dialog.Text("Bloodthirst (L1)          Enhances the HP recovery. \n");
                        dialog.Text("Solid (L1)                   Improves Defense largely. \n");
                        dialog.Option("Venom (L1).", 34);
                        dialog.Option("Elan (L1).", 35);
                        dialog.Option("Ethereal (L1).", 36);
                        dialog.Option("Bloodthirst (L1).", 37);
                        dialog.Option("Solid (L1).", 38);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 7:
                    {
                        dialog.Text("Please choose the Necklace Mythsoul you want. ");
                        dialog.Text("Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€*Mythsoul Attribue*Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€ ");
                        dialog.Text("       [Mythsoul]                            [Attribute] ");
                        dialog.Text("Venom (L1)               Uses strong venom to poison enemies.\n ");
                        dialog.Text("Elan (L1)                   Improves MP restoration.\n ");
                        dialog.Text("Ethereal (L1)              Improves Dodge. \n");
                        dialog.Text("Bloodthirst (L1)          Enhances the HP recovery. \n");
                        dialog.Text("Solid (L1)                   Improves Defense largely. \n");
                        dialog.Option("Venom (L1).", 39);
                        dialog.Option("Elan (L1).", 40);
                        dialog.Option("Ethereal (L1).", 41);
                        dialog.Option("Bloodthirst (L1).", 42);
                        dialog.Option("Solid (L1).", 43);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 8:
                    {
                        dialog.Text("Please choose the Boot Mythsoul you want. ");
                        dialog.Text("Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€*Mythsoul Attribue*Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€ ");
                        dialog.Text("       [Mythsoul]                            [Attribute] ");
                        dialog.Text("Venom (L1)               Uses strong venom to poison enemies.\n ");
                        dialog.Text("Elan (L1)                   Improves MP restoration.\n ");
                        dialog.Text("Ethereal (L1)              Improves Dodge. \n");
                        dialog.Text("Bloodthirst (L1)          Enhances the HP recovery. \n");
                        dialog.Text("Solid (L1)                   Improves Defense largely. \n");
                        dialog.Option("Venom (L1).", 44);
                        dialog.Option("Elan (L1).", 45);
                        dialog.Option("Ethereal (L1).", 46);
                        dialog.Option("Bloodthirst (L1).", 47);
                        dialog.Option("Solid (L1).", 48);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 9:
                    {
                        dialog.Text("Are you sure you want 1-handed Venom");
                        dialog.Option("Yes.", 49);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        dialog.Text("Are you sure you want 1-handed Elan");
                        dialog.Option("Yes.", 50);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        dialog.Text("Are you sure you want 1-handed Ethereal");
                        dialog.Option("Yes.", 51);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 12:
                    {
                        dialog.Text("Are you sure you want 1-handed Bloodthirst");
                        dialog.Option("Yes.", 52);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 13:
                    {
                        dialog.Text("Are you sure you want 1-handed Solid");
                        dialog.Option("Yes.", 53);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }

                case 14:
                    {
                        dialog.Text("Are you sure you want Shield Venom");
                        dialog.Option("Yes.", 54);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 15:
                    {
                        dialog.Text("Are you sure you want Shield Elan");
                        dialog.Option("Yes.", 55);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 16:
                    {
                        dialog.Text("Are you sure you want Shield Ethereal");
                        dialog.Option("Yes.", 56);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 17:
                    {
                        dialog.Text("Are you sure you want Shield Bloodthirst");
                        dialog.Option("Yes.", 57);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 18:
                    {
                        dialog.Text("Are you sure you want Shield Solid");
                        dialog.Option("Yes.", 58);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }

                case 19:
                    {
                        dialog.Text("Are you sure you want Ring Venom");
                        dialog.Option("Yes.", 59);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 20:
                    {
                        dialog.Text("Are you sure you want Ring Elan");
                        dialog.Option("Yes.", 60);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        dialog.Text("Are you sure you want Ring Ethereal");
                        dialog.Option("Yes.", 61);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 22:
                    {
                        dialog.Text("Are you sure you want Ring Bloodthirst");
                        dialog.Option("Yes.", 62);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 23:
                    {
                        dialog.Text("Are you sure you want Ring Solid");
                        dialog.Option("Yes.", 63);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }

                case 24:
                    {
                        dialog.Text("Are you sure you want Armor Venom");
                        dialog.Option("Yes.", 64);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 25:
                    {
                        dialog.Text("Are you sure you want Armor Elan");
                        dialog.Option("Yes.", 65);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 26:
                    {
                        dialog.Text("Are you sure you want Armor Ethereal");
                        dialog.Option("Yes.", 66);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 27:
                    {
                        dialog.Text("Are you sure you want Armor Bloodthirst");
                        dialog.Option("Yes.", 67);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 28:
                    {
                        dialog.Text("Are you sure you want Armor Solid");
                        dialog.Option("Yes.", 68);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }


                case 29:
                    {
                        dialog.Text("Are you sure you want 2-handed Venom");
                        dialog.Option("Yes.", 69);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 30:
                    {
                        dialog.Text("Are you sure you want 2-handed Elan");
                        dialog.Option("Yes.", 70);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        dialog.Text("Are you sure you want 2-handed Ethereal");
                        dialog.Option("Yes.", 71);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 32:
                    {
                        dialog.Text("Are you sure you want 2-handed Bloodthirst");
                        dialog.Option("Yes.", 72);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 33:
                    {
                        dialog.Text("Are you sure you want 2-handed Solid");
                        dialog.Option("Yes.", 73);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }


                case 34:
                    {
                        dialog.Text("Are you sure you want Headgear Venom");
                        dialog.Option("Yes.", 74);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 35:
                    {
                        dialog.Text("Are you sure you want Headgear Elan");
                        dialog.Option("Yes.", 75);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 36:
                    {
                        dialog.Text("Are you sure you want Headgear Ethereal");
                        dialog.Option("Yes.", 76);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 37:
                    {
                        dialog.Text("Are you sure you want Headgear Bloodthirst");
                        dialog.Option("Yes.", 77);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 38:
                    {
                        dialog.Text("Are you sure you want Headgear Solid");
                        dialog.Option("Yes.", 78);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }

                case 39:
                    {
                        dialog.Text("Are you sure you want Necklace Venom");
                        dialog.Option("Yes.", 79);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 40:
                    {
                        dialog.Text("Are you sure you want Necklace Elan");
                        dialog.Option("Yes.", 80);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 41:
                    {
                        dialog.Text("Are you sure you want Necklace Ethereal");
                        dialog.Option("Yes.", 81);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 42:
                    {
                        dialog.Text("Are you sure you want Necklace Bloodthirst");
                        dialog.Option("Yes.", 82);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 43:
                    {
                        dialog.Text("Are you sure you want Necklace Solid");
                        dialog.Option("Yes.", 83);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }



                case 44:
                    {
                        dialog.Text("Are you sure you want Boot Venom");
                        dialog.Option("Yes.", 84);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 45:
                    {
                        dialog.Text("Are you sure you want Boot Elan");
                        dialog.Option("Yes.", 85);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 46:
                    {
                        dialog.Text("Are you sure you want Boot Ethereal");
                        dialog.Option("Yes.", 86);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 47:
                    {
                        dialog.Text("Are you sure you want Boot Bloodthirst");
                        dialog.Option("Yes.", 87);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 48:
                    {
                        dialog.Text("Are you sure you want Boot Solid");
                        dialog.Option("Yes.", 88);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                default:
                    {
                        if (client.Inventory.Contain(client.ActiveNpc, 1))
                        {
                            if (client.Inventory.Remove(client.ActiveNpc, 1, stream))
                            {
                                uint itemID = 0;
                                switch (Option)
                                {
                                    case 49: { itemID = 40704011; break; }
                                    case 50: { itemID = 40704171; break; }
                                    case 51: { itemID = 40704081; break; }
                                    case 52: { itemID = 40704041; break; }
                                    case 53: { itemID = 40704181; break; }
                                    case 54: { itemID = 40706011; break; }
                                    case 55: { itemID = 40706171; break; }
                                    case 56: { itemID = 40706081; break; }
                                    case 57: { itemID = 40706041; break; }
                                    case 58: { itemID = 40706181; break; }
                                    case 59: { itemID = 40707011; break; }
                                    case 60: { itemID = 40707171; break; }
                                    case 61: { itemID = 40707081; break; }
                                    case 62: { itemID = 40707041; break; }
                                    case 63: { itemID = 40707181; break; }
                                    case 64: { itemID = 40703011; break; }
                                    case 65: { itemID = 40703171; break; }
                                    case 66: { itemID = 40703081; break; }
                                    case 67: { itemID = 40703041; break; }
                                    case 68: { itemID = 40703181; break; }
                                    case 69: { itemID = 40705011; break; }
                                    case 70: { itemID = 40705171; break; }
                                    case 71: { itemID = 40705081; break; }
                                    case 72: { itemID = 40705041; break; }
                                    case 73: { itemID = 40705181; break; }
                                    case 74: { itemID = 40701011; break; }
                                    case 75: { itemID = 40701171; break; }
                                    case 76: { itemID = 40701081; break; }
                                    case 77: { itemID = 40701041; break; }
                                    case 78: { itemID = 40701181; break; }
                                    case 79: { itemID = 40702011; break; }
                                    case 80: { itemID = 40702171; break; }
                                    case 81: { itemID = 40702081; break; }
                                    case 82: { itemID = 40702041; break; }
                                    case 83: { itemID = 40702181; break; }
                                    case 84: { itemID = 40708011; break; }
                                    case 85: { itemID = 40708171; break; }
                                    case 86: { itemID = 40708081; break; }
                                    case 87: { itemID = 40708041; break; }
                                    case 88: { itemID = 40708181; break; }



                                }
                                client.Inventory.Add(stream, itemID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(client.ActiveNpc, 1, stream))
                            {
                                uint itemID = 0;
                                switch (Option)
                                {
                                    case 49: { itemID = 40704011; break; }
                                    case 50: { itemID = 40704171; break; }
                                    case 51: { itemID = 40704081; break; }
                                    case 52: { itemID = 40704041; break; }
                                    case 53: { itemID = 40704181; break; }
                                    case 54: { itemID = 40706011; break; }
                                    case 55: { itemID = 40706171; break; }
                                    case 56: { itemID = 40706081; break; }
                                    case 57: { itemID = 40706041; break; }
                                    case 58: { itemID = 40706181; break; }
                                    case 59: { itemID = 40707011; break; }
                                    case 60: { itemID = 40707171; break; }
                                    case 61: { itemID = 40707081; break; }
                                    case 62: { itemID = 40707041; break; }
                                    case 63: { itemID = 40707181; break; }
                                    case 64: { itemID = 40703011; break; }
                                    case 65: { itemID = 40703171; break; }
                                    case 66: { itemID = 40703081; break; }
                                    case 67: { itemID = 40703041; break; }
                                    case 68: { itemID = 40703181; break; }
                                    case 69: { itemID = 40705011; break; }
                                    case 70: { itemID = 40705171; break; }
                                    case 71: { itemID = 40705081; break; }
                                    case 72: { itemID = 40705041; break; }
                                    case 73: { itemID = 40705181; break; }
                                    case 74: { itemID = 40701011; break; }
                                    case 75: { itemID = 40701171; break; }
                                    case 76: { itemID = 40701081; break; }
                                    case 77: { itemID = 40701041; break; }
                                    case 78: { itemID = 40701181; break; }
                                    case 79: { itemID = 40702011; break; }
                                    case 80: { itemID = 40702171; break; }
                                    case 81: { itemID = 40702081; break; }
                                    case 82: { itemID = 40702041; break; }
                                    case 83: { itemID = 40702181; break; }
                                    case 84: { itemID = 40708011; break; }
                                    case 85: { itemID = 40708171; break; }
                                    case 86: { itemID = 40708081; break; }
                                    case 87: { itemID = 40708041; break; }
                                    case 88: { itemID = 40708181; break; }

                                }
                                client.Inventory.Add(stream, itemID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            }
                        }

                        break;
                    }
            }
        }
        public static void L2MythsoulPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        dialog.Text("Please choose the Mythsoul you want. ");
                        dialog.Option("1-handed Weapon Mythsoul.", 1);
                        dialog.Option("Shield Mythsoul.", 2);
                        dialog.Option("Ring Mythsoul.", 3);
                        dialog.Option("Armor Mythsoul.", 4);
                        dialog.Option("2-handed Weapon Mythsoul.", 5);
                        dialog.Option("Headgear Mythsoul.", 6);
                        dialog.Option("Necklace Mythsoul.", 7);
                        dialog.Option("Boot Mythsoul.", 8);
                        dialog.Avatar(93).FinalizeDialog();
                        break;

                    }
                case 1:
                    {
                        dialog.Text("Please choose the 1-handed Mythsoul you want.\n");
                        dialog.Text("Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€*Mythsoul Attribue*Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€ \n");
                        dialog.Text("       [Mythsoul]                            [Attribute].\n ");
                        dialog.Text("Venom (L2)               Uses strong venom to poison enemies.\n ");
                        dialog.Text("Elan (L2)                   Improves MP restoration.\n ");
                        dialog.Text("Ethereal (L2)              Improves Dodge. \n");
                        dialog.Text("Bloodthirst (L2)          Enhances the HP recovery. \n");
                        dialog.Text("Solid (L2)                   Improves Defense largely. \n");
                        dialog.Option("Venom (L2).", 9);
                        dialog.Option("Elan (L2).", 10);
                        dialog.Option("Ethereal (L2).", 11);
                        dialog.Option("Bloodthirst (L2).", 12);
                        dialog.Option("Solid (L2).", 13);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        dialog.Text("Please choose the Shield Mythsoul you want. ");
                        dialog.Text("Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€*Mythsoul Attribue*Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€ ");
                        dialog.Text("       [Mythsoul]                            [Attribute] ");
                        dialog.Text("Venom (L2)               Uses strong venom to poison enemies.\n ");
                        dialog.Text("Elan (L2)                   Improves MP restoration.\n ");
                        dialog.Text("Ethereal (L2)              Improves Dodge. \n");
                        dialog.Text("Bloodthirst (L2)          Enhances the HP recovery. \n");
                        dialog.Text("Solid (L2)                   Improves Defense largely. \n");
                        dialog.Option("Venom (L2).", 14);
                        dialog.Option("Elan (L2).", 15);
                        dialog.Option("Ethereal (L2).", 16);
                        dialog.Option("Bloodthirst (L2).", 17);
                        dialog.Option("Solid (L2).", 18);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        dialog.Text("Please choose the Ring Mythsoul you want. ");
                        dialog.Text("Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€*Mythsoul Attribue*Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€ ");
                        dialog.Text("       [Mythsoul]                            [Attribute] ");
                        dialog.Text("Venom (L2)               Uses strong venom to poison enemies.\n ");
                        dialog.Text("Elan (L2)                   Improves MP restoration.\n ");
                        dialog.Text("Ethereal (L2)              Improves Dodge. \n");
                        dialog.Text("Bloodthirst (L2)          Enhances the HP recovery. \n");
                        dialog.Text("Solid (L2)                   Improves Defense largely. \n");
                        dialog.Option("Venom (L2).", 19);
                        dialog.Option("Elan (L2).", 20);
                        dialog.Option("Ethereal (L2).", 21);
                        dialog.Option("Bloodthirst (L2).", 22);
                        dialog.Option("Solid (L2).", 23);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        dialog.Text("Please choose the Armor Mythsoul you want. ");
                        dialog.Text("Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€*Mythsoul Attribue*Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€ ");
                        dialog.Text("       [Mythsoul]                            [Attribute] ");
                        dialog.Text("Venom (L2)               Uses strong venom to poison enemies.\n ");
                        dialog.Text("Elan (L2)                   Improves MP restoration.\n ");
                        dialog.Text("Ethereal (L2)              Improves Dodge. \n");
                        dialog.Text("Bloodthirst (L2)          Enhances the HP recovery. \n");
                        dialog.Text("Solid (L2)                   Improves Defense largely. \n");
                        dialog.Option("Venom (L2).", 24);
                        dialog.Option("Elan (L2).", 25);
                        dialog.Option("Ethereal (L2).", 26);
                        dialog.Option("Bloodthirst (L2).", 27);
                        dialog.Option("Solid (L2).", 28);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        dialog.Text("Please choose the 2-handed Mythsoul you want. ");
                        dialog.Text("Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€*Mythsoul Attribue*Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€ ");
                        dialog.Text("       [Mythsoul]                            [Attribute] ");
                        dialog.Text("Venom (L2)               Uses strong venom to poison enemies.\n ");
                        dialog.Text("Elan (L2)                   Improves MP restoration.\n ");
                        dialog.Text("Ethereal (L2)              Improves Dodge. \n");
                        dialog.Text("Bloodthirst (L2)          Enhances the HP recovery. \n");
                        dialog.Text("Solid (L2)                   Improves Defense largely. \n");
                        dialog.Option("Venom (L2).", 29);
                        dialog.Option("Elan (L2).", 30);
                        dialog.Option("Ethereal (L2).", 31);
                        dialog.Option("Bloodthirst (L2).", 32);
                        dialog.Option("Solid (L2).", 33);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        dialog.Text("Please choose the Headgear Mythsoul you want. ");
                        dialog.Text("Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€*Mythsoul Attribue*Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€ ");
                        dialog.Text("       [Mythsoul]                            [Attribute] ");
                        dialog.Text("Venom (L2)               Uses strong venom to poison enemies.\n ");
                        dialog.Text("Elan (L2)                   Improves MP restoration.\n ");
                        dialog.Text("Ethereal (L2)              Improves Dodge. \n");
                        dialog.Text("Bloodthirst (L2)          Enhances the HP recovery. \n");
                        dialog.Text("Solid (L2)                   Improves Defense largely. \n");
                        dialog.Option("Venom (L2).", 34);
                        dialog.Option("Elan (L2).", 35);
                        dialog.Option("Ethereal (L2).", 36);
                        dialog.Option("Bloodthirst (L2).", 37);
                        dialog.Option("Solid (L2).", 38);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 7:
                    {
                        dialog.Text("Please choose the Necklace Mythsoul you want. ");
                        dialog.Text("Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€*Mythsoul Attribue*Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€ ");
                        dialog.Text("       [Mythsoul]                            [Attribute] ");
                        dialog.Text("Venom (L2)               Uses strong venom to poison enemies.\n ");
                        dialog.Text("Elan (L2)                   Improves MP restoration.\n ");
                        dialog.Text("Ethereal (L2)              Improves Dodge. \n");
                        dialog.Text("Bloodthirst (L2)          Enhances the HP recovery. \n");
                        dialog.Text("Solid (L2)                   Improves Defense largely. \n");
                        dialog.Option("Venom (L2).", 39);
                        dialog.Option("Elan (L2).", 40);
                        dialog.Option("Ethereal (L2).", 41);
                        dialog.Option("Bloodthirst (L2).", 42);
                        dialog.Option("Solid (L2).", 43);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 8:
                    {
                        dialog.Text("Please choose the Boot Mythsoul you want. ");
                        dialog.Text("Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€*Mythsoul Attribue*Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€ ");
                        dialog.Text("       [Mythsoul]                            [Attribute] ");
                        dialog.Text("Venom (L2)               Uses strong venom to poison enemies.\n ");
                        dialog.Text("Elan (L2)                   Improves MP restoration.\n ");
                        dialog.Text("Ethereal (L2)              Improves Dodge. \n");
                        dialog.Text("Bloodthirst (L2)          Enhances the HP recovery. \n");
                        dialog.Text("Solid (L2)                   Improves Defense largely. \n");
                        dialog.Option("Venom (L2).", 44);
                        dialog.Option("Elan (L2).", 45);
                        dialog.Option("Ethereal (L2).", 46);
                        dialog.Option("Bloodthirst (L2).", 47);
                        dialog.Option("Solid (L2).", 48);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 9:
                    {
                        dialog.Text("Are you sure you want 1-handed Venom");
                        dialog.Option("Yes.", 49);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        dialog.Text("Are you sure you want 1-handed Elan");
                        dialog.Option("Yes.", 50);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        dialog.Text("Are you sure you want 1-handed Ethereal");
                        dialog.Option("Yes.", 51);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 12:
                    {
                        dialog.Text("Are you sure you want 1-handed Bloodthirst");
                        dialog.Option("Yes.", 52);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 13:
                    {
                        dialog.Text("Are you sure you want 1-handed Solid");
                        dialog.Option("Yes.", 53);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }

                case 14:
                    {
                        dialog.Text("Are you sure you want Shield Venom");
                        dialog.Option("Yes.", 54);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 15:
                    {
                        dialog.Text("Are you sure you want Shield Elan");
                        dialog.Option("Yes.", 55);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 16:
                    {
                        dialog.Text("Are you sure you want Shield Ethereal");
                        dialog.Option("Yes.", 56);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 17:
                    {
                        dialog.Text("Are you sure you want Shield Bloodthirst");
                        dialog.Option("Yes.", 57);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 18:
                    {
                        dialog.Text("Are you sure you want Shield Solid");
                        dialog.Option("Yes.", 58);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }

                case 19:
                    {
                        dialog.Text("Are you sure you want Ring Venom");
                        dialog.Option("Yes.", 59);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 20:
                    {
                        dialog.Text("Are you sure you want Ring Elan");
                        dialog.Option("Yes.", 60);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        dialog.Text("Are you sure you want Ring Ethereal");
                        dialog.Option("Yes.", 61);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 22:
                    {
                        dialog.Text("Are you sure you want Ring Bloodthirst");
                        dialog.Option("Yes.", 62);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 23:
                    {
                        dialog.Text("Are you sure you want Ring Solid");
                        dialog.Option("Yes.", 63);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }

                case 24:
                    {
                        dialog.Text("Are you sure you want Armor Venom");
                        dialog.Option("Yes.", 64);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 25:
                    {
                        dialog.Text("Are you sure you want Armor Elan");
                        dialog.Option("Yes.", 65);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 26:
                    {
                        dialog.Text("Are you sure you want Armor Ethereal");
                        dialog.Option("Yes.", 66);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 27:
                    {
                        dialog.Text("Are you sure you want Armor Bloodthirst");
                        dialog.Option("Yes.", 67);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 28:
                    {
                        dialog.Text("Are you sure you want Armor Solid");
                        dialog.Option("Yes.", 68);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }


                case 29:
                    {
                        dialog.Text("Are you sure you want 2-handed Venom");
                        dialog.Option("Yes.", 69);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 30:
                    {
                        dialog.Text("Are you sure you want 2-handed Elan");
                        dialog.Option("Yes.", 70);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        dialog.Text("Are you sure you want 2-handed Ethereal");
                        dialog.Option("Yes.", 71);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 32:
                    {
                        dialog.Text("Are you sure you want 2-handed Bloodthirst");
                        dialog.Option("Yes.", 72);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 33:
                    {
                        dialog.Text("Are you sure you want 2-handed Solid");
                        dialog.Option("Yes.", 73);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }


                case 34:
                    {
                        dialog.Text("Are you sure you want Headgear Venom");
                        dialog.Option("Yes.", 74);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 35:
                    {
                        dialog.Text("Are you sure you want Headgear Elan");
                        dialog.Option("Yes.", 75);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 36:
                    {
                        dialog.Text("Are you sure you want Headgear Ethereal");
                        dialog.Option("Yes.", 76);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 37:
                    {
                        dialog.Text("Are you sure you want Headgear Bloodthirst");
                        dialog.Option("Yes.", 77);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 38:
                    {
                        dialog.Text("Are you sure you want Headgear Solid");
                        dialog.Option("Yes.", 78);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }

                case 39:
                    {
                        dialog.Text("Are you sure you want Necklace Venom");
                        dialog.Option("Yes.", 79);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 40:
                    {
                        dialog.Text("Are you sure you want Necklace Elan");
                        dialog.Option("Yes.", 80);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 41:
                    {
                        dialog.Text("Are you sure you want Necklace Ethereal");
                        dialog.Option("Yes.", 81);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 42:
                    {
                        dialog.Text("Are you sure you want Necklace Bloodthirst");
                        dialog.Option("Yes.", 82);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 43:
                    {
                        dialog.Text("Are you sure you want Necklace Solid");
                        dialog.Option("Yes.", 83);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }



                case 44:
                    {
                        dialog.Text("Are you sure you want Boot Venom");
                        dialog.Option("Yes.", 84);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 45:
                    {
                        dialog.Text("Are you sure you want Boot Elan");
                        dialog.Option("Yes.", 85);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 46:
                    {
                        dialog.Text("Are you sure you want Boot Ethereal");
                        dialog.Option("Yes.", 86);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 47:
                    {
                        dialog.Text("Are you sure you want Boot Bloodthirst");
                        dialog.Option("Yes.", 87);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 48:
                    {
                        dialog.Text("Are you sure you want Boot Solid");
                        dialog.Option("Yes.", 88);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                default:
                    {
                        if (client.Inventory.Contain(client.ActiveNpc, 1))
                        {
                            if (client.Inventory.Remove(client.ActiveNpc, 1, stream))
                            {
                                uint itemID = 0;
                                switch (Option)
                                {
                                    case 49: { itemID = 40704012; break; }
                                    case 50: { itemID = 40704172; break; }
                                    case 51: { itemID = 40704082; break; }
                                    case 52: { itemID = 40704042; break; }
                                    case 53: { itemID = 40704182; break; }
                                    case 54: { itemID = 40706012; break; }
                                    case 55: { itemID = 40706172; break; }
                                    case 56: { itemID = 40706082; break; }
                                    case 57: { itemID = 40706042; break; }
                                    case 58: { itemID = 40706182; break; }
                                    case 59: { itemID = 40707012; break; }
                                    case 60: { itemID = 40707172; break; }
                                    case 61: { itemID = 40707082; break; }
                                    case 62: { itemID = 40707042; break; }
                                    case 63: { itemID = 40707182; break; }
                                    case 64: { itemID = 40703012; break; }
                                    case 65: { itemID = 40703172; break; }
                                    case 66: { itemID = 40703082; break; }
                                    case 67: { itemID = 40703042; break; }
                                    case 68: { itemID = 40703182; break; }
                                    case 69: { itemID = 40705012; break; }
                                    case 70: { itemID = 40705172; break; }
                                    case 71: { itemID = 40705082; break; }
                                    case 72: { itemID = 40705042; break; }
                                    case 73: { itemID = 40705182; break; }
                                    case 74: { itemID = 40701012; break; }
                                    case 75: { itemID = 40701172; break; }
                                    case 76: { itemID = 40701082; break; }
                                    case 77: { itemID = 40701042; break; }
                                    case 78: { itemID = 40701182; break; }
                                    case 79: { itemID = 40702012; break; }
                                    case 80: { itemID = 40702172; break; }
                                    case 81: { itemID = 40702082; break; }
                                    case 82: { itemID = 40702042; break; }
                                    case 83: { itemID = 40702182; break; }
                                    case 84: { itemID = 40708012; break; }
                                    case 85: { itemID = 40708172; break; }
                                    case 86: { itemID = 40708082; break; }
                                    case 87: { itemID = 40708042; break; }
                                    case 88: { itemID = 40708182; break; }



                                }
                                client.Inventory.Add(stream, itemID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(client.ActiveNpc, 1, stream))
                            {
                                uint itemID = 0;
                                switch (Option)
                                {
                                    case 49: { itemID = 40704012; break; }
                                    case 50: { itemID = 40704172; break; }
                                    case 51: { itemID = 40704082; break; }
                                    case 52: { itemID = 40704042; break; }
                                    case 53: { itemID = 40704182; break; }
                                    case 54: { itemID = 40706012; break; }
                                    case 55: { itemID = 40706172; break; }
                                    case 56: { itemID = 40706082; break; }
                                    case 57: { itemID = 40706042; break; }
                                    case 58: { itemID = 40706182; break; }
                                    case 59: { itemID = 40707012; break; }
                                    case 60: { itemID = 40707172; break; }
                                    case 61: { itemID = 40707082; break; }
                                    case 62: { itemID = 40707042; break; }
                                    case 63: { itemID = 40707182; break; }
                                    case 64: { itemID = 40703012; break; }
                                    case 65: { itemID = 40703172; break; }
                                    case 66: { itemID = 40703082; break; }
                                    case 67: { itemID = 40703042; break; }
                                    case 68: { itemID = 40703182; break; }
                                    case 69: { itemID = 40705012; break; }
                                    case 70: { itemID = 40705172; break; }
                                    case 71: { itemID = 40705082; break; }
                                    case 72: { itemID = 40705042; break; }
                                    case 73: { itemID = 40705182; break; }
                                    case 74: { itemID = 40701012; break; }
                                    case 75: { itemID = 40701172; break; }
                                    case 76: { itemID = 40701082; break; }
                                    case 77: { itemID = 40701042; break; }
                                    case 78: { itemID = 40701182; break; }
                                    case 79: { itemID = 40702012; break; }
                                    case 80: { itemID = 40702172; break; }
                                    case 81: { itemID = 40702082; break; }
                                    case 82: { itemID = 40702042; break; }
                                    case 83: { itemID = 40702182; break; }
                                    case 84: { itemID = 40708012; break; }
                                    case 85: { itemID = 40708172; break; }
                                    case 86: { itemID = 40708082; break; }
                                    case 87: { itemID = 40708042; break; }
                                    case 88: { itemID = 40708182; break; }

                                }
                                client.Inventory.Add(stream, itemID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            }
                        }

                        break;
                    }
            }
        }
        public static void L3MythsoulPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        dialog.Text("Please choose the Mythsoul you want. ");
                        dialog.Option("1-handed Weapon Mythsoul.", 1);
                        dialog.Option("Shield Mythsoul.", 2);
                        dialog.Option("Ring Mythsoul.", 3);
                        dialog.Option("Armor Mythsoul.", 4);
                        dialog.Option("2-handed Weapon Mythsoul.", 5);
                        dialog.Option("Headgear Mythsoul.", 6);
                        dialog.Option("Necklace Mythsoul.", 7);
                        dialog.Option("Boot Mythsoul.", 8);
                        dialog.Avatar(93).FinalizeDialog();
                        break;

                    }
                case 1:
                    {
                        dialog.Text("Please choose the 1-handed Mythsoul you want.\n");
                        dialog.Text("Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€*Mythsoul Attribue*Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€ \n");
                        dialog.Text("       [Mythsoul]                            [Attribute].\n ");
                        dialog.Text("Venom (L3)               Uses strong venom to poison enemies.\n ");
                        dialog.Text("Elan (L3)                   Improves MP restoration.\n ");
                        dialog.Text("Ethereal (L3)              Improves Dodge. \n");
                        dialog.Text("Bloodthirst (L3)          Enhances the HP recovery. \n");
                        dialog.Text("Solid (L3)                   Improves Defense largely. \n");
                        dialog.Option("Venom (L3).", 9);
                        dialog.Option("Elan (L3).", 10);
                        dialog.Option("Ethereal (L3).", 11);
                        dialog.Option("Bloodthirst (L3).", 12);
                        dialog.Option("Solid (L3).", 13);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        dialog.Text("Please choose the Shield Mythsoul you want. ");
                        dialog.Text("Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€*Mythsoul Attribue*Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€ ");
                        dialog.Text("       [Mythsoul]                            [Attribute] ");
                        dialog.Text("Venom (L3)               Uses strong venom to poison enemies.\n ");
                        dialog.Text("Elan (L3)                   Improves MP restoration.\n ");
                        dialog.Text("Ethereal (L3)              Improves Dodge. \n");
                        dialog.Text("Bloodthirst (L3)          Enhances the HP recovery. \n");
                        dialog.Text("Solid (L3)                   Improves Defense largely. \n");
                        dialog.Option("Venom (L3).", 14);
                        dialog.Option("Elan (L3).", 15);
                        dialog.Option("Ethereal (L3).", 16);
                        dialog.Option("Bloodthirst (L3).", 17);
                        dialog.Option("Solid (L3).", 18);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        dialog.Text("Please choose the Ring Mythsoul you want. ");
                        dialog.Text("Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€*Mythsoul Attribue*Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€ ");
                        dialog.Text("       [Mythsoul]                            [Attribute] ");
                        dialog.Text("Venom (L3)               Uses strong venom to poison enemies.\n ");
                        dialog.Text("Elan (L3)                   Improves MP restoration.\n ");
                        dialog.Text("Ethereal (L3)              Improves Dodge. \n");
                        dialog.Text("Bloodthirst (L3)          Enhances the HP recovery. \n");
                        dialog.Text("Solid (L3)                   Improves Defense largely. \n");
                        dialog.Option("Venom (L3).", 19);
                        dialog.Option("Elan (L3).", 20);
                        dialog.Option("Ethereal (L3).", 21);
                        dialog.Option("Bloodthirst (L3).", 22);
                        dialog.Option("Solid (L3).", 23);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        dialog.Text("Please choose the Armor Mythsoul you want. ");
                        dialog.Text("Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€*Mythsoul Attribue*Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€ ");
                        dialog.Text("       [Mythsoul]                            [Attribute] ");
                        dialog.Text("Venom (L3)               Uses strong venom to poison enemies.\n ");
                        dialog.Text("Elan (L3)                   Improves MP restoration.\n ");
                        dialog.Text("Ethereal (L3)              Improves Dodge. \n");
                        dialog.Text("Bloodthirst (L3)          Enhances the HP recovery. \n");
                        dialog.Text("Solid (L3)                   Improves Defense largely. \n");
                        dialog.Option("Venom (L3).", 24);
                        dialog.Option("Elan (L3).", 25);
                        dialog.Option("Ethereal (L3).", 26);
                        dialog.Option("Bloodthirst (L3).", 27);
                        dialog.Option("Solid (L3).", 28);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        dialog.Text("Please choose the 2-handed Mythsoul you want. ");
                        dialog.Text("Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€*Mythsoul Attribue*Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€ ");
                        dialog.Text("       [Mythsoul]                            [Attribute] ");
                        dialog.Text("Venom (L3)               Uses strong venom to poison enemies.\n ");
                        dialog.Text("Elan (L3)                   Improves MP restoration.\n ");
                        dialog.Text("Ethereal (L3)              Improves Dodge. \n");
                        dialog.Text("Bloodthirst (L3)          Enhances the HP recovery. \n");
                        dialog.Text("Solid (L3)                   Improves Defense largely. \n");
                        dialog.Option("Venom (L3).", 29);
                        dialog.Option("Elan (L3).", 30);
                        dialog.Option("Ethereal (L3).", 31);
                        dialog.Option("Bloodthirst (L3).", 32);
                        dialog.Option("Solid (L3).", 33);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        dialog.Text("Please choose the Headgear Mythsoul you want. ");
                        dialog.Text("Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€*Mythsoul Attribue*Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€ ");
                        dialog.Text("       [Mythsoul]                            [Attribute] ");
                        dialog.Text("Venom (L3)               Uses strong venom to poison enemies.\n ");
                        dialog.Text("Elan (L3)                   Improves MP restoration.\n ");
                        dialog.Text("Ethereal (L3)              Improves Dodge. \n");
                        dialog.Text("Bloodthirst (L3)          Enhances the HP recovery. \n");
                        dialog.Text("Solid (L3)                   Improves Defense largely. \n");
                        dialog.Option("Venom (L3).", 34);
                        dialog.Option("Elan (L3).", 35);
                        dialog.Option("Ethereal (L3).", 36);
                        dialog.Option("Bloodthirst (L3).", 37);
                        dialog.Option("Solid (L3).", 38);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 7:
                    {
                        dialog.Text("Please choose the Necklace Mythsoul you want. ");
                        dialog.Text("Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€*Mythsoul Attribue*Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€ ");
                        dialog.Text("       [Mythsoul]                            [Attribute] ");
                        dialog.Text("Venom (L3)               Uses strong venom to poison enemies.\n ");
                        dialog.Text("Elan (L3)                   Improves MP restoration.\n ");
                        dialog.Text("Ethereal (L3)              Improves Dodge. \n");
                        dialog.Text("Bloodthirst (L3)          Enhances the HP recovery. \n");
                        dialog.Text("Solid (L3)                   Improves Defense largely. \n");
                        dialog.Option("Venom (L3).", 39);
                        dialog.Option("Elan (L3).", 40);
                        dialog.Option("Ethereal (L3).", 41);
                        dialog.Option("Bloodthirst (L3).", 42);
                        dialog.Option("Solid (L3).", 43);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 8:
                    {
                        dialog.Text("Please choose the Boot Mythsoul you want. ");
                        dialog.Text("Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€*Mythsoul Attribue*Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€ ");
                        dialog.Text("       [Mythsoul]                            [Attribute] ");
                        dialog.Text("Venom (L3)               Uses strong venom to poison enemies.\n ");
                        dialog.Text("Elan (L3)                   Improves MP restoration.\n ");
                        dialog.Text("Ethereal (L3)              Improves Dodge. \n");
                        dialog.Text("Bloodthirst (L3)          Enhances the HP recovery. \n");
                        dialog.Text("Solid (L3)                   Improves Defense largely. \n");
                        dialog.Option("Venom (L3).", 44);
                        dialog.Option("Elan (L3).", 45);
                        dialog.Option("Ethereal (L3).", 46);
                        dialog.Option("Bloodthirst (L3).", 47);
                        dialog.Option("Solid (L3).", 48);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 9:
                    {
                        dialog.Text("Are you sure you want 1-handed Venom");
                        dialog.Option("Yes.", 49);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        dialog.Text("Are you sure you want 1-handed Elan");
                        dialog.Option("Yes.", 50);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        dialog.Text("Are you sure you want 1-handed Ethereal");
                        dialog.Option("Yes.", 51);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 12:
                    {
                        dialog.Text("Are you sure you want 1-handed Bloodthirst");
                        dialog.Option("Yes.", 52);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 13:
                    {
                        dialog.Text("Are you sure you want 1-handed Solid");
                        dialog.Option("Yes.", 53);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }

                case 14:
                    {
                        dialog.Text("Are you sure you want Shield Venom");
                        dialog.Option("Yes.", 54);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 15:
                    {
                        dialog.Text("Are you sure you want Shield Elan");
                        dialog.Option("Yes.", 55);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 16:
                    {
                        dialog.Text("Are you sure you want Shield Ethereal");
                        dialog.Option("Yes.", 56);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 17:
                    {
                        dialog.Text("Are you sure you want Shield Bloodthirst");
                        dialog.Option("Yes.", 57);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 18:
                    {
                        dialog.Text("Are you sure you want Shield Solid");
                        dialog.Option("Yes.", 58);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }

                case 19:
                    {
                        dialog.Text("Are you sure you want Ring Venom");
                        dialog.Option("Yes.", 59);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 20:
                    {
                        dialog.Text("Are you sure you want Ring Elan");
                        dialog.Option("Yes.", 60);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        dialog.Text("Are you sure you want Ring Ethereal");
                        dialog.Option("Yes.", 61);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 22:
                    {
                        dialog.Text("Are you sure you want Ring Bloodthirst");
                        dialog.Option("Yes.", 62);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 23:
                    {
                        dialog.Text("Are you sure you want Ring Solid");
                        dialog.Option("Yes.", 63);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }

                case 24:
                    {
                        dialog.Text("Are you sure you want Armor Venom");
                        dialog.Option("Yes.", 64);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 25:
                    {
                        dialog.Text("Are you sure you want Armor Elan");
                        dialog.Option("Yes.", 65);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 26:
                    {
                        dialog.Text("Are you sure you want Armor Ethereal");
                        dialog.Option("Yes.", 66);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 27:
                    {
                        dialog.Text("Are you sure you want Armor Bloodthirst");
                        dialog.Option("Yes.", 67);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 28:
                    {
                        dialog.Text("Are you sure you want Armor Solid");
                        dialog.Option("Yes.", 68);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }


                case 29:
                    {
                        dialog.Text("Are you sure you want 2-handed Venom");
                        dialog.Option("Yes.", 69);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 30:
                    {
                        dialog.Text("Are you sure you want 2-handed Elan");
                        dialog.Option("Yes.", 70);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        dialog.Text("Are you sure you want 2-handed Ethereal");
                        dialog.Option("Yes.", 71);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 32:
                    {
                        dialog.Text("Are you sure you want 2-handed Bloodthirst");
                        dialog.Option("Yes.", 72);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 33:
                    {
                        dialog.Text("Are you sure you want 2-handed Solid");
                        dialog.Option("Yes.", 73);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }


                case 34:
                    {
                        dialog.Text("Are you sure you want Headgear Venom");
                        dialog.Option("Yes.", 74);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 35:
                    {
                        dialog.Text("Are you sure you want Headgear Elan");
                        dialog.Option("Yes.", 75);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 36:
                    {
                        dialog.Text("Are you sure you want Headgear Ethereal");
                        dialog.Option("Yes.", 76);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 37:
                    {
                        dialog.Text("Are you sure you want Headgear Bloodthirst");
                        dialog.Option("Yes.", 77);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 38:
                    {
                        dialog.Text("Are you sure you want Headgear Solid");
                        dialog.Option("Yes.", 78);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }

                case 39:
                    {
                        dialog.Text("Are you sure you want Necklace Venom");
                        dialog.Option("Yes.", 79);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 40:
                    {
                        dialog.Text("Are you sure you want Necklace Elan");
                        dialog.Option("Yes.", 80);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 41:
                    {
                        dialog.Text("Are you sure you want Necklace Ethereal");
                        dialog.Option("Yes.", 81);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 42:
                    {
                        dialog.Text("Are you sure you want Necklace Bloodthirst");
                        dialog.Option("Yes.", 82);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 43:
                    {
                        dialog.Text("Are you sure you want Necklace Solid");
                        dialog.Option("Yes.", 83);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }



                case 44:
                    {
                        dialog.Text("Are you sure you want Boot Venom");
                        dialog.Option("Yes.", 84);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 45:
                    {
                        dialog.Text("Are you sure you want Boot Elan");
                        dialog.Option("Yes.", 85);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 46:
                    {
                        dialog.Text("Are you sure you want Boot Ethereal");
                        dialog.Option("Yes.", 86);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 47:
                    {
                        dialog.Text("Are you sure you want Boot Bloodthirst");
                        dialog.Option("Yes.", 87);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 48:
                    {
                        dialog.Text("Are you sure you want Boot Solid");
                        dialog.Option("Yes.", 88);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                default:
                    {
                        if (client.Inventory.Contain(client.ActiveNpc, 1))
                        {
                            if (client.Inventory.Remove(client.ActiveNpc, 1, stream))
                            {
                                uint itemID = 0;
                                switch (Option)
                                {
                                    case 49: { itemID = 40704013; break; }
                                    case 50: { itemID = 40704173; break; }
                                    case 51: { itemID = 40704083; break; }
                                    case 52: { itemID = 40704043; break; }
                                    case 53: { itemID = 40704183; break; }
                                    case 54: { itemID = 40706013; break; }
                                    case 55: { itemID = 40706173; break; }
                                    case 56: { itemID = 40706083; break; }
                                    case 57: { itemID = 40706043; break; }
                                    case 58: { itemID = 40706183; break; }
                                    case 59: { itemID = 40707013; break; }
                                    case 60: { itemID = 40707173; break; }
                                    case 61: { itemID = 40707083; break; }
                                    case 62: { itemID = 40707043; break; }
                                    case 63: { itemID = 40707183; break; }
                                    case 64: { itemID = 40703013; break; }
                                    case 65: { itemID = 40703173; break; }
                                    case 66: { itemID = 40703083; break; }
                                    case 67: { itemID = 40703043; break; }
                                    case 68: { itemID = 40703183; break; }
                                    case 69: { itemID = 40705013; break; }
                                    case 70: { itemID = 40705173; break; }
                                    case 71: { itemID = 40705083; break; }
                                    case 72: { itemID = 40705043; break; }
                                    case 73: { itemID = 40705183; break; }
                                    case 74: { itemID = 40701013; break; }
                                    case 75: { itemID = 40701173; break; }
                                    case 76: { itemID = 40701083; break; }
                                    case 77: { itemID = 40701043; break; }
                                    case 78: { itemID = 40701183; break; }
                                    case 79: { itemID = 40702013; break; }
                                    case 80: { itemID = 40702173; break; }
                                    case 81: { itemID = 40702083; break; }
                                    case 82: { itemID = 40702043; break; }
                                    case 83: { itemID = 40702183; break; }
                                    case 84: { itemID = 40708013; break; }
                                    case 85: { itemID = 40708173; break; }
                                    case 86: { itemID = 40708083; break; }
                                    case 87: { itemID = 40708043; break; }
                                    case 88: { itemID = 40708183; break; }



                                }
                                client.Inventory.Add(stream, itemID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(client.ActiveNpc, 1, stream))
                            {
                                uint itemID = 0;
                                switch (Option)
                                {
                                    case 49: { itemID = 40704013; break; }
                                    case 50: { itemID = 40704173; break; }
                                    case 51: { itemID = 40704083; break; }
                                    case 52: { itemID = 40704043; break; }
                                    case 53: { itemID = 40704183; break; }
                                    case 54: { itemID = 40706013; break; }
                                    case 55: { itemID = 40706173; break; }
                                    case 56: { itemID = 40706083; break; }
                                    case 57: { itemID = 40706043; break; }
                                    case 58: { itemID = 40706183; break; }
                                    case 59: { itemID = 40707013; break; }
                                    case 60: { itemID = 40707173; break; }
                                    case 61: { itemID = 40707083; break; }
                                    case 62: { itemID = 40707043; break; }
                                    case 63: { itemID = 40707183; break; }
                                    case 64: { itemID = 40703013; break; }
                                    case 65: { itemID = 40703173; break; }
                                    case 66: { itemID = 40703083; break; }
                                    case 67: { itemID = 40703043; break; }
                                    case 68: { itemID = 40703183; break; }
                                    case 69: { itemID = 40705013; break; }
                                    case 70: { itemID = 40705173; break; }
                                    case 71: { itemID = 40705083; break; }
                                    case 72: { itemID = 40705043; break; }
                                    case 73: { itemID = 40705183; break; }
                                    case 74: { itemID = 40701013; break; }
                                    case 75: { itemID = 40701173; break; }
                                    case 76: { itemID = 40701083; break; }
                                    case 77: { itemID = 40701043; break; }
                                    case 78: { itemID = 40701183; break; }
                                    case 79: { itemID = 40702013; break; }
                                    case 80: { itemID = 40702173; break; }
                                    case 81: { itemID = 40702083; break; }
                                    case 82: { itemID = 40702043; break; }
                                    case 83: { itemID = 40702183; break; }
                                    case 84: { itemID = 40708013; break; }
                                    case 85: { itemID = 40708173; break; }
                                    case 86: { itemID = 40708083; break; }
                                    case 87: { itemID = 40708043; break; }
                                    case 88: { itemID = 40708183; break; }

                                }
                                client.Inventory.Add(stream, itemID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            }
                        }

                        break;
                    }
            }
        }
        public static void L4MythsoulPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        dialog.Text("Please choose the Mythsoul you want. ");
                        dialog.Option("1-handed Weapon Mythsoul.", 1);
                        dialog.Option("Shield Mythsoul.", 2);
                        dialog.Option("Ring Mythsoul.", 3);
                        dialog.Option("Armor Mythsoul.", 4);
                        dialog.Option("2-handed Weapon Mythsoul.", 5);
                        dialog.Option("Headgear Mythsoul.", 6);
                        dialog.Option("Necklace Mythsoul.", 7);
                        dialog.Option("Boot Mythsoul.", 8);
                        dialog.Avatar(93).FinalizeDialog();
                        break;

                    }
                case 1:
                    {
                        dialog.Text("Please choose the 1-handed Mythsoul you want.\n");
                        dialog.Text("Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€*Mythsoul Attribue*Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€ \n");
                        dialog.Text("       [Mythsoul]                            [Attribute].\n ");
                        dialog.Text("Venom (L4)               Uses strong venom to poison enemies.\n ");
                        dialog.Text("Elan (L4)                   Improves MP restoration.\n ");
                        dialog.Text("Ethereal (L4)              Improves Dodge. \n");
                        dialog.Text("Bloodthirst (L4)          Enhances the HP recovery. \n");
                        dialog.Text("Solid (L4)                   Improves Defense largely. \n");
                        dialog.Option("Venom (L4).", 9);
                        dialog.Option("Elan (L4).", 10);
                        dialog.Option("Ethereal (L4).", 11);
                        dialog.Option("Bloodthirst (L4).", 12);
                        dialog.Option("Solid (L4).", 13);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        dialog.Text("Please choose the Shield Mythsoul you want. ");
                        dialog.Text("Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€*Mythsoul Attribue*Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€ ");
                        dialog.Text("       [Mythsoul]                            [Attribute] ");
                        dialog.Text("Venom (L4)               Uses strong venom to poison enemies.\n ");
                        dialog.Text("Elan (L4)                   Improves MP restoration.\n ");
                        dialog.Text("Ethereal (L4)              Improves Dodge. \n");
                        dialog.Text("Bloodthirst (L4)          Enhances the HP recovery. \n");
                        dialog.Text("Solid (L4)                   Improves Defense largely. \n");
                        dialog.Option("Venom (L4).", 14);
                        dialog.Option("Elan (L4).", 15);
                        dialog.Option("Ethereal (L4).", 16);
                        dialog.Option("Bloodthirst (L4).", 17);
                        dialog.Option("Solid (L4).", 18);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        dialog.Text("Please choose the Ring Mythsoul you want. ");
                        dialog.Text("Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€*Mythsoul Attribue*Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€ ");
                        dialog.Text("       [Mythsoul]                            [Attribute] ");
                        dialog.Text("Venom (L4)               Uses strong venom to poison enemies.\n ");
                        dialog.Text("Elan (L4)                   Improves MP restoration.\n ");
                        dialog.Text("Ethereal (L4)              Improves Dodge. \n");
                        dialog.Text("Bloodthirst (L4)          Enhances the HP recovery. \n");
                        dialog.Text("Solid (L4)                   Improves Defense largely. \n");
                        dialog.Option("Venom (L4).", 19);
                        dialog.Option("Elan (L4).", 20);
                        dialog.Option("Ethereal (L4).", 21);
                        dialog.Option("Bloodthirst (L4).", 22);
                        dialog.Option("Solid (L4).", 23);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        dialog.Text("Please choose the Armor Mythsoul you want. ");
                        dialog.Text("Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€*Mythsoul Attribue*Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€ ");
                        dialog.Text("       [Mythsoul]                            [Attribute] ");
                        dialog.Text("Venom (L4)               Uses strong venom to poison enemies.\n ");
                        dialog.Text("Elan (L4)                   Improves MP restoration.\n ");
                        dialog.Text("Ethereal (L4)              Improves Dodge. \n");
                        dialog.Text("Bloodthirst (L4)          Enhances the HP recovery. \n");
                        dialog.Text("Solid (L4)                   Improves Defense largely. \n");
                        dialog.Option("Venom (L4).", 24);
                        dialog.Option("Elan (L4).", 25);
                        dialog.Option("Ethereal (L4).", 26);
                        dialog.Option("Bloodthirst (L4).", 27);
                        dialog.Option("Solid (L4).", 28);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        dialog.Text("Please choose the 2-handed Mythsoul you want. ");
                        dialog.Text("Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€*Mythsoul Attribue*Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€ ");
                        dialog.Text("       [Mythsoul]                            [Attribute] ");
                        dialog.Text("Venom (L4)               Uses strong venom to poison enemies.\n ");
                        dialog.Text("Elan (L4)                   Improves MP restoration.\n ");
                        dialog.Text("Ethereal (L4)              Improves Dodge. \n");
                        dialog.Text("Bloodthirst (L4)          Enhances the HP recovery. \n");
                        dialog.Text("Solid (L4)                   Improves Defense largely. \n");
                        dialog.Option("Venom (L4).", 29);
                        dialog.Option("Elan (L4).", 30);
                        dialog.Option("Ethereal (L4).", 31);
                        dialog.Option("Bloodthirst (L4).", 32);
                        dialog.Option("Solid (L4).", 33);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        dialog.Text("Please choose the Headgear Mythsoul you want. ");
                        dialog.Text("Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€*Mythsoul Attribue*Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€ ");
                        dialog.Text("       [Mythsoul]                            [Attribute] ");
                        dialog.Text("Venom (L4)               Uses strong venom to poison enemies.\n ");
                        dialog.Text("Elan (L4)                   Improves MP restoration.\n ");
                        dialog.Text("Ethereal (L4)              Improves Dodge. \n");
                        dialog.Text("Bloodthirst (L4)          Enhances the HP recovery. \n");
                        dialog.Text("Solid (L4)                   Improves Defense largely. \n");
                        dialog.Option("Venom (L4).", 34);
                        dialog.Option("Elan (L4).", 35);
                        dialog.Option("Ethereal (L4).", 36);
                        dialog.Option("Bloodthirst (L4).", 37);
                        dialog.Option("Solid (L4).", 38);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 7:
                    {
                        dialog.Text("Please choose the Necklace Mythsoul you want. ");
                        dialog.Text("Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€*Mythsoul Attribue*Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€ ");
                        dialog.Text("       [Mythsoul]                            [Attribute] ");
                        dialog.Text("Venom (L4)               Uses strong venom to poison enemies.\n ");
                        dialog.Text("Elan (L4)                   Improves MP restoration.\n ");
                        dialog.Text("Ethereal (L4)              Improves Dodge. \n");
                        dialog.Text("Bloodthirst (L4)          Enhances the HP recovery. \n");
                        dialog.Text("Solid (L4)                   Improves Defense largely. \n");
                        dialog.Option("Venom (L4).", 39);
                        dialog.Option("Elan (L4).", 40);
                        dialog.Option("Ethereal (L4).", 41);
                        dialog.Option("Bloodthirst (L4).", 42);
                        dialog.Option("Solid (L4).", 43);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 8:
                    {
                        dialog.Text("Please choose the Boot Mythsoul you want. ");
                        dialog.Text("Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€*Mythsoul Attribue*Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€ ");
                        dialog.Text("       [Mythsoul]                            [Attribute] ");
                        dialog.Text("Venom (L4)               Uses strong venom to poison enemies.\n ");
                        dialog.Text("Elan (L4)                   Improves MP restoration.\n ");
                        dialog.Text("Ethereal (L4)              Improves Dodge. \n");
                        dialog.Text("Bloodthirst (L4)          Enhances the HP recovery. \n");
                        dialog.Text("Solid (L4)                   Improves Defense largely. \n");
                        dialog.Option("Venom (L4).", 44);
                        dialog.Option("Elan (L4).", 45);
                        dialog.Option("Ethereal (L4).", 46);
                        dialog.Option("Bloodthirst (L4).", 47);
                        dialog.Option("Solid (L4).", 48);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 9:
                    {
                        dialog.Text("Are you sure you want 1-handed Venom");
                        dialog.Option("Yes.", 49);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        dialog.Text("Are you sure you want 1-handed Elan");
                        dialog.Option("Yes.", 50);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        dialog.Text("Are you sure you want 1-handed Ethereal");
                        dialog.Option("Yes.", 51);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 12:
                    {
                        dialog.Text("Are you sure you want 1-handed Bloodthirst");
                        dialog.Option("Yes.", 52);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 13:
                    {
                        dialog.Text("Are you sure you want 1-handed Solid");
                        dialog.Option("Yes.", 53);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }

                case 14:
                    {
                        dialog.Text("Are you sure you want Shield Venom");
                        dialog.Option("Yes.", 54);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 15:
                    {
                        dialog.Text("Are you sure you want Shield Elan");
                        dialog.Option("Yes.", 55);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 16:
                    {
                        dialog.Text("Are you sure you want Shield Ethereal");
                        dialog.Option("Yes.", 56);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 17:
                    {
                        dialog.Text("Are you sure you want Shield Bloodthirst");
                        dialog.Option("Yes.", 57);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 18:
                    {
                        dialog.Text("Are you sure you want Shield Solid");
                        dialog.Option("Yes.", 58);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }

                case 19:
                    {
                        dialog.Text("Are you sure you want Ring Venom");
                        dialog.Option("Yes.", 59);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 20:
                    {
                        dialog.Text("Are you sure you want Ring Elan");
                        dialog.Option("Yes.", 60);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        dialog.Text("Are you sure you want Ring Ethereal");
                        dialog.Option("Yes.", 61);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 22:
                    {
                        dialog.Text("Are you sure you want Ring Bloodthirst");
                        dialog.Option("Yes.", 62);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 23:
                    {
                        dialog.Text("Are you sure you want Ring Solid");
                        dialog.Option("Yes.", 63);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }

                case 24:
                    {
                        dialog.Text("Are you sure you want Armor Venom");
                        dialog.Option("Yes.", 64);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 25:
                    {
                        dialog.Text("Are you sure you want Armor Elan");
                        dialog.Option("Yes.", 65);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 26:
                    {
                        dialog.Text("Are you sure you want Armor Ethereal");
                        dialog.Option("Yes.", 66);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 27:
                    {
                        dialog.Text("Are you sure you want Armor Bloodthirst");
                        dialog.Option("Yes.", 67);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 28:
                    {
                        dialog.Text("Are you sure you want Armor Solid");
                        dialog.Option("Yes.", 68);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }


                case 29:
                    {
                        dialog.Text("Are you sure you want 2-handed Venom");
                        dialog.Option("Yes.", 69);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 30:
                    {
                        dialog.Text("Are you sure you want 2-handed Elan");
                        dialog.Option("Yes.", 70);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        dialog.Text("Are you sure you want 2-handed Ethereal");
                        dialog.Option("Yes.", 71);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 32:
                    {
                        dialog.Text("Are you sure you want 2-handed Bloodthirst");
                        dialog.Option("Yes.", 72);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 33:
                    {
                        dialog.Text("Are you sure you want 2-handed Solid");
                        dialog.Option("Yes.", 73);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }


                case 34:
                    {
                        dialog.Text("Are you sure you want Headgear Venom");
                        dialog.Option("Yes.", 74);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 35:
                    {
                        dialog.Text("Are you sure you want Headgear Elan");
                        dialog.Option("Yes.", 75);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 36:
                    {
                        dialog.Text("Are you sure you want Headgear Ethereal");
                        dialog.Option("Yes.", 76);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 37:
                    {
                        dialog.Text("Are you sure you want Headgear Bloodthirst");
                        dialog.Option("Yes.", 77);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 38:
                    {
                        dialog.Text("Are you sure you want Headgear Solid");
                        dialog.Option("Yes.", 78);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }

                case 39:
                    {
                        dialog.Text("Are you sure you want Necklace Venom");
                        dialog.Option("Yes.", 79);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 40:
                    {
                        dialog.Text("Are you sure you want Necklace Elan");
                        dialog.Option("Yes.", 80);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 41:
                    {
                        dialog.Text("Are you sure you want Necklace Ethereal");
                        dialog.Option("Yes.", 81);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 42:
                    {
                        dialog.Text("Are you sure you want Necklace Bloodthirst");
                        dialog.Option("Yes.", 82);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 43:
                    {
                        dialog.Text("Are you sure you want Necklace Solid");
                        dialog.Option("Yes.", 83);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }



                case 44:
                    {
                        dialog.Text("Are you sure you want Boot Venom");
                        dialog.Option("Yes.", 84);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 45:
                    {
                        dialog.Text("Are you sure you want Boot Elan");
                        dialog.Option("Yes.", 85);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 46:
                    {
                        dialog.Text("Are you sure you want Boot Ethereal");
                        dialog.Option("Yes.", 86);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 47:
                    {
                        dialog.Text("Are you sure you want Boot Bloodthirst");
                        dialog.Option("Yes.", 87);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 48:
                    {
                        dialog.Text("Are you sure you want Boot Solid");
                        dialog.Option("Yes.", 88);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                default:
                    {
                        if (client.Inventory.Contain(client.ActiveNpc, 1))
                        {
                            if (client.Inventory.Remove(client.ActiveNpc, 1, stream))
                            {
                                uint itemID = 0;
                                switch (Option)
                                {
                                    case 49: { itemID = 40704014; break; }
                                    case 50: { itemID = 40704174; break; }
                                    case 51: { itemID = 40704084; break; }
                                    case 52: { itemID = 40704044; break; }
                                    case 53: { itemID = 40704184; break; }
                                    case 54: { itemID = 40706014; break; }
                                    case 55: { itemID = 40706174; break; }
                                    case 56: { itemID = 40706084; break; }
                                    case 57: { itemID = 40706044; break; }
                                    case 58: { itemID = 40706184; break; }
                                    case 59: { itemID = 40707014; break; }
                                    case 60: { itemID = 40707174; break; }
                                    case 61: { itemID = 40707084; break; }
                                    case 62: { itemID = 40707044; break; }
                                    case 63: { itemID = 40707184; break; }
                                    case 64: { itemID = 40703014; break; }
                                    case 65: { itemID = 40703174; break; }
                                    case 66: { itemID = 40703084; break; }
                                    case 67: { itemID = 40703044; break; }
                                    case 68: { itemID = 40703184; break; }
                                    case 69: { itemID = 40705014; break; }
                                    case 70: { itemID = 40705174; break; }
                                    case 71: { itemID = 40705084; break; }
                                    case 72: { itemID = 40705044; break; }
                                    case 73: { itemID = 40705184; break; }
                                    case 74: { itemID = 40701014; break; }
                                    case 75: { itemID = 40701174; break; }
                                    case 76: { itemID = 40701084; break; }
                                    case 77: { itemID = 40701044; break; }
                                    case 78: { itemID = 40701184; break; }
                                    case 79: { itemID = 40702014; break; }
                                    case 80: { itemID = 40702174; break; }
                                    case 81: { itemID = 40702084; break; }
                                    case 82: { itemID = 40702044; break; }
                                    case 83: { itemID = 40702184; break; }
                                    case 84: { itemID = 40708014; break; }
                                    case 85: { itemID = 40708174; break; }
                                    case 86: { itemID = 40708084; break; }
                                    case 87: { itemID = 40708044; break; }
                                    case 88: { itemID = 40708184; break; }



                                }
                                client.Inventory.Add(stream, itemID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(client.ActiveNpc, 1, stream))
                            {
                                uint itemID = 0;
                                switch (Option)
                                {
                                    case 49: { itemID = 40704014; break; }
                                    case 50: { itemID = 40704174; break; }
                                    case 51: { itemID = 40704084; break; }
                                    case 52: { itemID = 40704044; break; }
                                    case 53: { itemID = 40704184; break; }
                                    case 54: { itemID = 40706014; break; }
                                    case 55: { itemID = 40706174; break; }
                                    case 56: { itemID = 40706084; break; }
                                    case 57: { itemID = 40706044; break; }
                                    case 58: { itemID = 40706184; break; }
                                    case 59: { itemID = 40707014; break; }
                                    case 60: { itemID = 40707174; break; }
                                    case 61: { itemID = 40707084; break; }
                                    case 62: { itemID = 40707044; break; }
                                    case 63: { itemID = 40707184; break; }
                                    case 64: { itemID = 40703014; break; }
                                    case 65: { itemID = 40703174; break; }
                                    case 66: { itemID = 40703084; break; }
                                    case 67: { itemID = 40703044; break; }
                                    case 68: { itemID = 40703184; break; }
                                    case 69: { itemID = 40705014; break; }
                                    case 70: { itemID = 40705174; break; }
                                    case 71: { itemID = 40705084; break; }
                                    case 72: { itemID = 40705044; break; }
                                    case 73: { itemID = 40705184; break; }
                                    case 74: { itemID = 40701014; break; }
                                    case 75: { itemID = 40701174; break; }
                                    case 76: { itemID = 40701084; break; }
                                    case 77: { itemID = 40701044; break; }
                                    case 78: { itemID = 40701184; break; }
                                    case 79: { itemID = 40702014; break; }
                                    case 80: { itemID = 40702174; break; }
                                    case 81: { itemID = 40702084; break; }
                                    case 82: { itemID = 40702044; break; }
                                    case 83: { itemID = 40702184; break; }
                                    case 84: { itemID = 40708014; break; }
                                    case 85: { itemID = 40708174; break; }
                                    case 86: { itemID = 40708084; break; }
                                    case 87: { itemID = 40708044; break; }
                                    case 88: { itemID = 40708184; break; }

                                }
                                client.Inventory.Add(stream, itemID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            }
                        }

                        break;
                    }
            }
        }
        public static void L5MythsoulPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        dialog.Text("Please choose the Mythsoul you want. ");
                        dialog.Option("1-handed Weapon Mythsoul.", 1);
                        dialog.Option("Shield Mythsoul.", 2);
                        dialog.Option("Ring Mythsoul.", 3);
                        dialog.Option("Armor Mythsoul.", 4);
                        dialog.Option("2-handed Weapon Mythsoul.", 5);
                        dialog.Option("Headgear Mythsoul.", 6);
                        dialog.Option("Necklace Mythsoul.", 7);
                        dialog.Option("Boot Mythsoul.", 8);
                        dialog.Avatar(93).FinalizeDialog();
                        break;

                    }
                case 1:
                    {
                        dialog.Text("Please choose the 1-handed Mythsoul you want.\n");
                        dialog.Text("Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€*Mythsoul Attribue*Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€ \n");
                        dialog.Text("       [Mythsoul]                            [Attribute].\n ");
                        dialog.Text("Venom (L5)               Uses strong venom to poison enemies.\n ");
                        dialog.Text("Elan (L5)                   Improves MP restoration.\n ");
                        dialog.Text("Ethereal (L5)              Improves Dodge. \n");
                        dialog.Text("Bloodthirst (L5)          Enhances the HP recovery. \n");
                        dialog.Text("Solid (L5)                   Improves Defense largely. \n");
                        dialog.Option("Venom (L5).", 9);
                        dialog.Option("Elan (L5).", 10);
                        dialog.Option("Ethereal (L5).", 11);
                        dialog.Option("Bloodthirst (L5).", 12);
                        dialog.Option("Solid (L5).", 13);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        dialog.Text("Please choose the Shield Mythsoul you want. ");
                        dialog.Text("Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€*Mythsoul Attribue*Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€ ");
                        dialog.Text("       [Mythsoul]                            [Attribute] ");
                        dialog.Text("Venom (L5)               Uses strong venom to poison enemies.\n ");
                        dialog.Text("Elan (L5)                   Improves MP restoration.\n ");
                        dialog.Text("Ethereal (L5)              Improves Dodge. \n");
                        dialog.Text("Bloodthirst (L5)          Enhances the HP recovery. \n");
                        dialog.Text("Solid (L5)                   Improves Defense largely. \n");
                        dialog.Option("Venom (L5).", 14);
                        dialog.Option("Elan (L5).", 15);
                        dialog.Option("Ethereal (L5).", 16);
                        dialog.Option("Bloodthirst (L5).", 17);
                        dialog.Option("Solid (L5).", 18);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        dialog.Text("Please choose the Ring Mythsoul you want. ");
                        dialog.Text("Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€*Mythsoul Attribue*Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€ ");
                        dialog.Text("       [Mythsoul]                            [Attribute] ");
                        dialog.Text("Venom (L5)               Uses strong venom to poison enemies.\n ");
                        dialog.Text("Elan (L5)                   Improves MP restoration.\n ");
                        dialog.Text("Ethereal (L5)              Improves Dodge. \n");
                        dialog.Text("Bloodthirst (L5)          Enhances the HP recovery. \n");
                        dialog.Text("Solid (L5)                   Improves Defense largely. \n");
                        dialog.Option("Venom (L5).", 19);
                        dialog.Option("Elan (L5).", 20);
                        dialog.Option("Ethereal (L5).", 21);
                        dialog.Option("Bloodthirst (L5).", 22);
                        dialog.Option("Solid (L5).", 23);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        dialog.Text("Please choose the Armor Mythsoul you want. ");
                        dialog.Text("Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€*Mythsoul Attribue*Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€ ");
                        dialog.Text("       [Mythsoul]                            [Attribute] ");
                        dialog.Text("Venom (L5)               Uses strong venom to poison enemies.\n ");
                        dialog.Text("Elan (L5)                   Improves MP restoration.\n ");
                        dialog.Text("Ethereal (L5)              Improves Dodge. \n");
                        dialog.Text("Bloodthirst (L5)          Enhances the HP recovery. \n");
                        dialog.Text("Solid (L5)                   Improves Defense largely. \n");
                        dialog.Option("Venom (L5).", 24);
                        dialog.Option("Elan (L5).", 25);
                        dialog.Option("Ethereal (L5).", 26);
                        dialog.Option("Bloodthirst (L5).", 27);
                        dialog.Option("Solid (L5).", 28);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        dialog.Text("Please choose the 2-handed Mythsoul you want. ");
                        dialog.Text("Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€*Mythsoul Attribue*Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€ ");
                        dialog.Text("       [Mythsoul]                            [Attribute] ");
                        dialog.Text("Venom (L5)               Uses strong venom to poison enemies.\n ");
                        dialog.Text("Elan (L5)                   Improves MP restoration.\n ");
                        dialog.Text("Ethereal (L5)              Improves Dodge. \n");
                        dialog.Text("Bloodthirst (L5)          Enhances the HP recovery. \n");
                        dialog.Text("Solid (L5)                   Improves Defense largely. \n");
                        dialog.Option("Venom (L5).", 29);
                        dialog.Option("Elan (L5).", 30);
                        dialog.Option("Ethereal (L5).", 31);
                        dialog.Option("Bloodthirst (L5).", 32);
                        dialog.Option("Solid (L5).", 33);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        dialog.Text("Please choose the Headgear Mythsoul you want. ");
                        dialog.Text("Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€*Mythsoul Attribue*Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€ ");
                        dialog.Text("       [Mythsoul]                            [Attribute] ");
                        dialog.Text("Venom (L5)               Uses strong venom to poison enemies.\n ");
                        dialog.Text("Elan (L5)                   Improves MP restoration.\n ");
                        dialog.Text("Ethereal (L5)              Improves Dodge. \n");
                        dialog.Text("Bloodthirst (L5)          Enhances the HP recovery. \n");
                        dialog.Text("Solid (L5)                   Improves Defense largely. \n");
                        dialog.Option("Venom (L5).", 34);
                        dialog.Option("Elan (L5).", 35);
                        dialog.Option("Ethereal (L5).", 36);
                        dialog.Option("Bloodthirst (L5).", 37);
                        dialog.Option("Solid (L5).", 38);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 7:
                    {
                        dialog.Text("Please choose the Necklace Mythsoul you want. ");
                        dialog.Text("Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€*Mythsoul Attribue*Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€ ");
                        dialog.Text("       [Mythsoul]                            [Attribute] ");
                        dialog.Text("Venom (L5)               Uses strong venom to poison enemies.\n ");
                        dialog.Text("Elan (L5)                   Improves MP restoration.\n ");
                        dialog.Text("Ethereal (L5)              Improves Dodge. \n");
                        dialog.Text("Bloodthirst (L5)          Enhances the HP recovery. \n");
                        dialog.Text("Solid (L5)                   Improves Defense largely. \n");
                        dialog.Option("Venom (L5).", 39);
                        dialog.Option("Elan (L5).", 40);
                        dialog.Option("Ethereal (L5).", 41);
                        dialog.Option("Bloodthirst (L5).", 42);
                        dialog.Option("Solid (L5).", 43);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 8:
                    {
                        dialog.Text("Please choose the Boot Mythsoul you want. ");
                        dialog.Text("Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€*Mythsoul Attribue*Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€ ");
                        dialog.Text("       [Mythsoul]                            [Attribute] ");
                        dialog.Text("Venom (L5)               Uses strong venom to poison enemies.\n ");
                        dialog.Text("Elan (L5)                   Improves MP restoration.\n ");
                        dialog.Text("Ethereal (L5)              Improves Dodge. \n");
                        dialog.Text("Bloodthirst (L5)          Enhances the HP recovery. \n");
                        dialog.Text("Solid (L5)                   Improves Defense largely. \n");
                        dialog.Option("Venom (L5).", 44);
                        dialog.Option("Elan (L5).", 45);
                        dialog.Option("Ethereal (L5).", 46);
                        dialog.Option("Bloodthirst (L5).", 47);
                        dialog.Option("Solid (L5).", 48);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 9:
                    {
                        dialog.Text("Are you sure you want 1-handed Venom");
                        dialog.Option("Yes.", 49);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        dialog.Text("Are you sure you want 1-handed Elan");
                        dialog.Option("Yes.", 50);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        dialog.Text("Are you sure you want 1-handed Ethereal");
                        dialog.Option("Yes.", 51);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 12:
                    {
                        dialog.Text("Are you sure you want 1-handed Bloodthirst");
                        dialog.Option("Yes.", 52);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 13:
                    {
                        dialog.Text("Are you sure you want 1-handed Solid");
                        dialog.Option("Yes.", 53);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }

                case 14:
                    {
                        dialog.Text("Are you sure you want Shield Venom");
                        dialog.Option("Yes.", 54);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 15:
                    {
                        dialog.Text("Are you sure you want Shield Elan");
                        dialog.Option("Yes.", 55);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 16:
                    {
                        dialog.Text("Are you sure you want Shield Ethereal");
                        dialog.Option("Yes.", 56);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 17:
                    {
                        dialog.Text("Are you sure you want Shield Bloodthirst");
                        dialog.Option("Yes.", 57);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 18:
                    {
                        dialog.Text("Are you sure you want Shield Solid");
                        dialog.Option("Yes.", 58);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }

                case 19:
                    {
                        dialog.Text("Are you sure you want Ring Venom");
                        dialog.Option("Yes.", 59);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 20:
                    {
                        dialog.Text("Are you sure you want Ring Elan");
                        dialog.Option("Yes.", 60);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        dialog.Text("Are you sure you want Ring Ethereal");
                        dialog.Option("Yes.", 61);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 22:
                    {
                        dialog.Text("Are you sure you want Ring Bloodthirst");
                        dialog.Option("Yes.", 62);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 23:
                    {
                        dialog.Text("Are you sure you want Ring Solid");
                        dialog.Option("Yes.", 63);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }

                case 24:
                    {
                        dialog.Text("Are you sure you want Armor Venom");
                        dialog.Option("Yes.", 64);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 25:
                    {
                        dialog.Text("Are you sure you want Armor Elan");
                        dialog.Option("Yes.", 65);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 26:
                    {
                        dialog.Text("Are you sure you want Armor Ethereal");
                        dialog.Option("Yes.", 66);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 27:
                    {
                        dialog.Text("Are you sure you want Armor Bloodthirst");
                        dialog.Option("Yes.", 67);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 28:
                    {
                        dialog.Text("Are you sure you want Armor Solid");
                        dialog.Option("Yes.", 68);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }


                case 29:
                    {
                        dialog.Text("Are you sure you want 2-handed Venom");
                        dialog.Option("Yes.", 69);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 30:
                    {
                        dialog.Text("Are you sure you want 2-handed Elan");
                        dialog.Option("Yes.", 70);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        dialog.Text("Are you sure you want 2-handed Ethereal");
                        dialog.Option("Yes.", 71);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 32:
                    {
                        dialog.Text("Are you sure you want 2-handed Bloodthirst");
                        dialog.Option("Yes.", 72);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 33:
                    {
                        dialog.Text("Are you sure you want 2-handed Solid");
                        dialog.Option("Yes.", 73);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }


                case 34:
                    {
                        dialog.Text("Are you sure you want Headgear Venom");
                        dialog.Option("Yes.", 74);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 35:
                    {
                        dialog.Text("Are you sure you want Headgear Elan");
                        dialog.Option("Yes.", 75);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 36:
                    {
                        dialog.Text("Are you sure you want Headgear Ethereal");
                        dialog.Option("Yes.", 76);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 37:
                    {
                        dialog.Text("Are you sure you want Headgear Bloodthirst");
                        dialog.Option("Yes.", 77);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 38:
                    {
                        dialog.Text("Are you sure you want Headgear Solid");
                        dialog.Option("Yes.", 78);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }

                case 39:
                    {
                        dialog.Text("Are you sure you want Necklace Venom");
                        dialog.Option("Yes.", 79);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 40:
                    {
                        dialog.Text("Are you sure you want Necklace Elan");
                        dialog.Option("Yes.", 80);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 41:
                    {
                        dialog.Text("Are you sure you want Necklace Ethereal");
                        dialog.Option("Yes.", 81);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 42:
                    {
                        dialog.Text("Are you sure you want Necklace Bloodthirst");
                        dialog.Option("Yes.", 82);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 43:
                    {
                        dialog.Text("Are you sure you want Necklace Solid");
                        dialog.Option("Yes.", 83);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }



                case 44:
                    {
                        dialog.Text("Are you sure you want Boot Venom");
                        dialog.Option("Yes.", 84);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 45:
                    {
                        dialog.Text("Are you sure you want Boot Elan");
                        dialog.Option("Yes.", 85);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 46:
                    {
                        dialog.Text("Are you sure you want Boot Ethereal");
                        dialog.Option("Yes.", 86);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 47:
                    {
                        dialog.Text("Are you sure you want Boot Bloodthirst");
                        dialog.Option("Yes.", 87);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 48:
                    {
                        dialog.Text("Are you sure you want Boot Solid");
                        dialog.Option("Yes.", 88);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                default:
                    {
                        if (client.Inventory.Contain(client.ActiveNpc, 1))
                        {
                            if (client.Inventory.Remove(client.ActiveNpc, 1, stream))
                            {
                                uint itemID = 0;
                                switch (Option)
                                {
                                    case 49: { itemID = 40704015; break; }
                                    case 50: { itemID = 40704175; break; }
                                    case 51: { itemID = 40704085; break; }
                                    case 52: { itemID = 40704045; break; }
                                    case 53: { itemID = 40704185; break; }
                                    case 54: { itemID = 40706015; break; }
                                    case 55: { itemID = 40706175; break; }
                                    case 56: { itemID = 40706085; break; }
                                    case 57: { itemID = 40706045; break; }
                                    case 58: { itemID = 40706185; break; }
                                    case 59: { itemID = 40707015; break; }
                                    case 60: { itemID = 40707175; break; }
                                    case 61: { itemID = 40707085; break; }
                                    case 62: { itemID = 40707045; break; }
                                    case 63: { itemID = 40707185; break; }
                                    case 64: { itemID = 40703015; break; }
                                    case 65: { itemID = 40703175; break; }
                                    case 66: { itemID = 40703085; break; }
                                    case 67: { itemID = 40703045; break; }
                                    case 68: { itemID = 40703185; break; }
                                    case 69: { itemID = 40705015; break; }
                                    case 70: { itemID = 40705175; break; }
                                    case 71: { itemID = 40705085; break; }
                                    case 72: { itemID = 40705045; break; }
                                    case 73: { itemID = 40705185; break; }
                                    case 74: { itemID = 40701015; break; }
                                    case 75: { itemID = 40701175; break; }
                                    case 76: { itemID = 40701085; break; }
                                    case 77: { itemID = 40701045; break; }
                                    case 78: { itemID = 40701185; break; }
                                    case 79: { itemID = 40702015; break; }
                                    case 80: { itemID = 40702175; break; }
                                    case 81: { itemID = 40702085; break; }
                                    case 82: { itemID = 40702045; break; }
                                    case 83: { itemID = 40702185; break; }
                                    case 84: { itemID = 40708015; break; }
                                    case 85: { itemID = 40708175; break; }
                                    case 86: { itemID = 40708085; break; }
                                    case 87: { itemID = 40708045; break; }
                                    case 88: { itemID = 40708185; break; }



                                }
                                client.Inventory.Add(stream, itemID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(client.ActiveNpc, 1, stream))
                            {
                                uint itemID = 0;
                                switch (Option)
                                {
                                    case 49: { itemID = 40704015; break; }
                                    case 50: { itemID = 40704175; break; }
                                    case 51: { itemID = 40704085; break; }
                                    case 52: { itemID = 40704045; break; }
                                    case 53: { itemID = 40704185; break; }
                                    case 54: { itemID = 40706015; break; }
                                    case 55: { itemID = 40706175; break; }
                                    case 56: { itemID = 40706085; break; }
                                    case 57: { itemID = 40706045; break; }
                                    case 58: { itemID = 40706185; break; }
                                    case 59: { itemID = 40707015; break; }
                                    case 60: { itemID = 40707175; break; }
                                    case 61: { itemID = 40707085; break; }
                                    case 62: { itemID = 40707045; break; }
                                    case 63: { itemID = 40707185; break; }
                                    case 64: { itemID = 40703015; break; }
                                    case 65: { itemID = 40703175; break; }
                                    case 66: { itemID = 40703085; break; }
                                    case 67: { itemID = 40703045; break; }
                                    case 68: { itemID = 40703185; break; }
                                    case 69: { itemID = 40705015; break; }
                                    case 70: { itemID = 40705175; break; }
                                    case 71: { itemID = 40705085; break; }
                                    case 72: { itemID = 40705045; break; }
                                    case 73: { itemID = 40705185; break; }
                                    case 74: { itemID = 40701015; break; }
                                    case 75: { itemID = 40701175; break; }
                                    case 76: { itemID = 40701085; break; }
                                    case 77: { itemID = 40701045; break; }
                                    case 78: { itemID = 40701185; break; }
                                    case 79: { itemID = 40702015; break; }
                                    case 80: { itemID = 40702175; break; }
                                    case 81: { itemID = 40702085; break; }
                                    case 82: { itemID = 40702045; break; }
                                    case 83: { itemID = 40702185; break; }
                                    case 84: { itemID = 40708015; break; }
                                    case 85: { itemID = 40708175; break; }
                                    case 86: { itemID = 40708085; break; }
                                    case 87: { itemID = 40708045; break; }
                                    case 88: { itemID = 40708185; break; }

                                }
                                client.Inventory.Add(stream, itemID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            }
                        }

                        break;
                    }
            }
        }
        public static void L6MythsoulPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        dialog.Text("Please choose the Mythsoul you want. ");
                        dialog.Option("1-handed Weapon Mythsoul.", 1);
                        dialog.Option("Shield Mythsoul.", 2);
                        dialog.Option("Ring Mythsoul.", 3);
                        dialog.Option("Armor Mythsoul.", 4);
                        dialog.Option("2-handed Weapon Mythsoul.", 5);
                        dialog.Option("Headgear Mythsoul.", 6);
                        dialog.Option("Necklace Mythsoul.", 7);
                        dialog.Option("Boot Mythsoul.", 8);
                        dialog.Avatar(93).FinalizeDialog();
                        break;

                    }
                case 1:
                    {
                        dialog.Text("Please choose the 1-handed Mythsoul you want.\n");
                        dialog.Text("Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€*Mythsoul Attribue*Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€ \n");
                        dialog.Text("       [Mythsoul]                            [Attribute].\n ");
                        dialog.Text("Venom (L6)               Uses strong venom to poison enemies.\n ");
                        dialog.Text("Elan (L6)                   Improves MP restoration.\n ");
                        dialog.Text("Ethereal (L6)              Improves Dodge. \n");
                        dialog.Text("Bloodthirst (L6)          Enhances the HP recovery. \n");
                        dialog.Text("Solid (L6)                   Improves Defense largely. \n");
                        dialog.Option("Venom (L6).", 9);
                        dialog.Option("Elan (L6).", 10);
                        dialog.Option("Ethereal (L6).", 11);
                        dialog.Option("Bloodthirst (L6).", 12);
                        dialog.Option("Solid (L6).", 13);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        dialog.Text("Please choose the Shield Mythsoul you want. ");
                        dialog.Text("Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€*Mythsoul Attribue*Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€ ");
                        dialog.Text("       [Mythsoul]                            [Attribute] ");
                        dialog.Text("Venom (L6)               Uses strong venom to poison enemies.\n ");
                        dialog.Text("Elan (L6)                   Improves MP restoration.\n ");
                        dialog.Text("Ethereal (L6)              Improves Dodge. \n");
                        dialog.Text("Bloodthirst (L6)          Enhances the HP recovery. \n");
                        dialog.Text("Solid (L6)                   Improves Defense largely. \n");
                        dialog.Option("Venom (L6).", 14);
                        dialog.Option("Elan (L6).", 15);
                        dialog.Option("Ethereal (L6).", 16);
                        dialog.Option("Bloodthirst (L6).", 17);
                        dialog.Option("Solid (L6).", 18);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        dialog.Text("Please choose the Ring Mythsoul you want. ");
                        dialog.Text("Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€*Mythsoul Attribue*Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€ ");
                        dialog.Text("       [Mythsoul]                            [Attribute] ");
                        dialog.Text("Venom (L6)               Uses strong venom to poison enemies.\n ");
                        dialog.Text("Elan (L6)                   Improves MP restoration.\n ");
                        dialog.Text("Ethereal (L6)              Improves Dodge. \n");
                        dialog.Text("Bloodthirst (L6)          Enhances the HP recovery. \n");
                        dialog.Text("Solid (L6)                   Improves Defense largely. \n");
                        dialog.Option("Venom (L6).", 19);
                        dialog.Option("Elan (L6).", 20);
                        dialog.Option("Ethereal (L6).", 21);
                        dialog.Option("Bloodthirst (L6).", 22);
                        dialog.Option("Solid (L6).", 23);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        dialog.Text("Please choose the Armor Mythsoul you want. ");
                        dialog.Text("Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€*Mythsoul Attribue*Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€ ");
                        dialog.Text("       [Mythsoul]                            [Attribute] ");
                        dialog.Text("Venom (L6)               Uses strong venom to poison enemies.\n ");
                        dialog.Text("Elan (L6)                   Improves MP restoration.\n ");
                        dialog.Text("Ethereal (L6)              Improves Dodge. \n");
                        dialog.Text("Bloodthirst (L6)          Enhances the HP recovery. \n");
                        dialog.Text("Solid (L6)                   Improves Defense largely. \n");
                        dialog.Option("Venom (L6).", 24);
                        dialog.Option("Elan (L6).", 25);
                        dialog.Option("Ethereal (L6).", 26);
                        dialog.Option("Bloodthirst (L6).", 27);
                        dialog.Option("Solid (L6).", 28);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        dialog.Text("Please choose the 2-handed Mythsoul you want. ");
                        dialog.Text("Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€*Mythsoul Attribue*Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€ ");
                        dialog.Text("       [Mythsoul]                            [Attribute] ");
                        dialog.Text("Venom (L6)               Uses strong venom to poison enemies.\n ");
                        dialog.Text("Elan (L6)                   Improves MP restoration.\n ");
                        dialog.Text("Ethereal (L6)              Improves Dodge. \n");
                        dialog.Text("Bloodthirst (L6)          Enhances the HP recovery. \n");
                        dialog.Text("Solid (L6)                   Improves Defense largely. \n");
                        dialog.Option("Venom (L6).", 29);
                        dialog.Option("Elan (L6).", 30);
                        dialog.Option("Ethereal (L6).", 31);
                        dialog.Option("Bloodthirst (L6).", 32);
                        dialog.Option("Solid (L6).", 33);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        dialog.Text("Please choose the Headgear Mythsoul you want. ");
                        dialog.Text("Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€*Mythsoul Attribue*Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€ ");
                        dialog.Text("       [Mythsoul]                            [Attribute] ");
                        dialog.Text("Venom (L6)               Uses strong venom to poison enemies.\n ");
                        dialog.Text("Elan (L6)                   Improves MP restoration.\n ");
                        dialog.Text("Ethereal (L6)              Improves Dodge. \n");
                        dialog.Text("Bloodthirst (L6)          Enhances the HP recovery. \n");
                        dialog.Text("Solid (L6)                   Improves Defense largely. \n");
                        dialog.Option("Venom (L6).", 34);
                        dialog.Option("Elan (L6).", 35);
                        dialog.Option("Ethereal (L6).", 36);
                        dialog.Option("Bloodthirst (L6).", 37);
                        dialog.Option("Solid (L6).", 38);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 7:
                    {
                        dialog.Text("Please choose the Necklace Mythsoul you want. ");
                        dialog.Text("Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€*Mythsoul Attribue*Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€ ");
                        dialog.Text("       [Mythsoul]                            [Attribute] ");
                        dialog.Text("Venom (L6)               Uses strong venom to poison enemies.\n ");
                        dialog.Text("Elan (L6)                   Improves MP restoration.\n ");
                        dialog.Text("Ethereal (L6)              Improves Dodge. \n");
                        dialog.Text("Bloodthirst (L6)          Enhances the HP recovery. \n");
                        dialog.Text("Solid (L6)                   Improves Defense largely. \n");
                        dialog.Option("Venom (L6).", 39);
                        dialog.Option("Elan (L6).", 40);
                        dialog.Option("Ethereal (L6).", 41);
                        dialog.Option("Bloodthirst (L6).", 42);
                        dialog.Option("Solid (L6).", 43);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 8:
                    {
                        dialog.Text("Please choose the Boot Mythsoul you want. ");
                        dialog.Text("Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€*Mythsoul Attribue*Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€Ã¢â‚¬â€ ");
                        dialog.Text("       [Mythsoul]                            [Attribute] ");
                        dialog.Text("Venom (L6)               Uses strong venom to poison enemies.\n ");
                        dialog.Text("Elan (L6)                   Improves MP restoration.\n ");
                        dialog.Text("Ethereal (L6)              Improves Dodge. \n");
                        dialog.Text("Bloodthirst (L6)          Enhances the HP recovery. \n");
                        dialog.Text("Solid (L6)                   Improves Defense largely. \n");
                        dialog.Option("Venom (L6).", 44);
                        dialog.Option("Elan (L6).", 45);
                        dialog.Option("Ethereal (L6).", 46);
                        dialog.Option("Bloodthirst (L6).", 47);
                        dialog.Option("Solid (L6).", 48);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 9:
                    {
                        dialog.Text("Are you sure you want 1-handed Venom");
                        dialog.Option("Yes.", 49);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        dialog.Text("Are you sure you want 1-handed Elan");
                        dialog.Option("Yes.", 50);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        dialog.Text("Are you sure you want 1-handed Ethereal");
                        dialog.Option("Yes.", 51);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 12:
                    {
                        dialog.Text("Are you sure you want 1-handed Bloodthirst");
                        dialog.Option("Yes.", 52);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 13:
                    {
                        dialog.Text("Are you sure you want 1-handed Solid");
                        dialog.Option("Yes.", 53);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }

                case 14:
                    {
                        dialog.Text("Are you sure you want Shield Venom");
                        dialog.Option("Yes.", 54);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 15:
                    {
                        dialog.Text("Are you sure you want Shield Elan");
                        dialog.Option("Yes.", 55);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 16:
                    {
                        dialog.Text("Are you sure you want Shield Ethereal");
                        dialog.Option("Yes.", 56);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 17:
                    {
                        dialog.Text("Are you sure you want Shield Bloodthirst");
                        dialog.Option("Yes.", 57);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 18:
                    {
                        dialog.Text("Are you sure you want Shield Solid");
                        dialog.Option("Yes.", 58);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }

                case 19:
                    {
                        dialog.Text("Are you sure you want Ring Venom");
                        dialog.Option("Yes.", 59);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 20:
                    {
                        dialog.Text("Are you sure you want Ring Elan");
                        dialog.Option("Yes.", 60);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        dialog.Text("Are you sure you want Ring Ethereal");
                        dialog.Option("Yes.", 61);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 22:
                    {
                        dialog.Text("Are you sure you want Ring Bloodthirst");
                        dialog.Option("Yes.", 62);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 23:
                    {
                        dialog.Text("Are you sure you want Ring Solid");
                        dialog.Option("Yes.", 63);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }

                case 24:
                    {
                        dialog.Text("Are you sure you want Armor Venom");
                        dialog.Option("Yes.", 64);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 25:
                    {
                        dialog.Text("Are you sure you want Armor Elan");
                        dialog.Option("Yes.", 65);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 26:
                    {
                        dialog.Text("Are you sure you want Armor Ethereal");
                        dialog.Option("Yes.", 66);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 27:
                    {
                        dialog.Text("Are you sure you want Armor Bloodthirst");
                        dialog.Option("Yes.", 67);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 28:
                    {
                        dialog.Text("Are you sure you want Armor Solid");
                        dialog.Option("Yes.", 68);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }


                case 29:
                    {
                        dialog.Text("Are you sure you want 2-handed Venom");
                        dialog.Option("Yes.", 69);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 30:
                    {
                        dialog.Text("Are you sure you want 2-handed Elan");
                        dialog.Option("Yes.", 70);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        dialog.Text("Are you sure you want 2-handed Ethereal");
                        dialog.Option("Yes.", 71);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 32:
                    {
                        dialog.Text("Are you sure you want 2-handed Bloodthirst");
                        dialog.Option("Yes.", 72);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 33:
                    {
                        dialog.Text("Are you sure you want 2-handed Solid");
                        dialog.Option("Yes.", 73);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }


                case 34:
                    {
                        dialog.Text("Are you sure you want Headgear Venom");
                        dialog.Option("Yes.", 74);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 35:
                    {
                        dialog.Text("Are you sure you want Headgear Elan");
                        dialog.Option("Yes.", 75);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 36:
                    {
                        dialog.Text("Are you sure you want Headgear Ethereal");
                        dialog.Option("Yes.", 76);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 37:
                    {
                        dialog.Text("Are you sure you want Headgear Bloodthirst");
                        dialog.Option("Yes.", 77);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 38:
                    {
                        dialog.Text("Are you sure you want Headgear Solid");
                        dialog.Option("Yes.", 78);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }

                case 39:
                    {
                        dialog.Text("Are you sure you want Necklace Venom");
                        dialog.Option("Yes.", 79);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 40:
                    {
                        dialog.Text("Are you sure you want Necklace Elan");
                        dialog.Option("Yes.", 80);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 41:
                    {
                        dialog.Text("Are you sure you want Necklace Ethereal");
                        dialog.Option("Yes.", 81);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 42:
                    {
                        dialog.Text("Are you sure you want Necklace Bloodthirst");
                        dialog.Option("Yes.", 82);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 43:
                    {
                        dialog.Text("Are you sure you want Necklace Solid");
                        dialog.Option("Yes.", 83);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }



                case 44:
                    {
                        dialog.Text("Are you sure you want Boot Venom");
                        dialog.Option("Yes.", 84);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 45:
                    {
                        dialog.Text("Are you sure you want Boot Elan");
                        dialog.Option("Yes.", 85);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 46:
                    {
                        dialog.Text("Are you sure you want Boot Ethereal");
                        dialog.Option("Yes.", 86);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 47:
                    {
                        dialog.Text("Are you sure you want Boot Bloodthirst");
                        dialog.Option("Yes.", 87);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 48:
                    {
                        dialog.Text("Are you sure you want Boot Solid");
                        dialog.Option("Yes.", 88);
                        dialog.Option("Choose again.", 0);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                default:
                    {
                        if (client.Inventory.Contain(client.ActiveNpc, 1))
                        {
                            if (client.Inventory.Remove(client.ActiveNpc, 1, stream))
                            {
                                uint itemID = 0;
                                switch (Option)
                                {
                                    case 49: { itemID = 40704016; break; }
                                    case 50: { itemID = 40704176; break; }
                                    case 51: { itemID = 40704086; break; }
                                    case 52: { itemID = 40704046; break; }
                                    case 53: { itemID = 40704186; break; }
                                    case 54: { itemID = 40706016; break; }
                                    case 55: { itemID = 40706176; break; }
                                    case 56: { itemID = 40706086; break; }
                                    case 57: { itemID = 40706046; break; }
                                    case 58: { itemID = 40706186; break; }
                                    case 59: { itemID = 40707016; break; }
                                    case 60: { itemID = 40707176; break; }
                                    case 61: { itemID = 40707086; break; }
                                    case 62: { itemID = 40707046; break; }
                                    case 63: { itemID = 40707186; break; }
                                    case 64: { itemID = 40703016; break; }
                                    case 65: { itemID = 40703176; break; }
                                    case 66: { itemID = 40703086; break; }
                                    case 67: { itemID = 40703046; break; }
                                    case 68: { itemID = 40703186; break; }
                                    case 69: { itemID = 40705016; break; }
                                    case 70: { itemID = 40705176; break; }
                                    case 71: { itemID = 40705086; break; }
                                    case 72: { itemID = 40705046; break; }
                                    case 73: { itemID = 40705186; break; }
                                    case 74: { itemID = 40701016; break; }
                                    case 75: { itemID = 40701176; break; }
                                    case 76: { itemID = 40701086; break; }
                                    case 77: { itemID = 40701046; break; }
                                    case 78: { itemID = 40701186; break; }
                                    case 79: { itemID = 40702016; break; }
                                    case 80: { itemID = 40702176; break; }
                                    case 81: { itemID = 40702086; break; }
                                    case 82: { itemID = 40702046; break; }
                                    case 83: { itemID = 40702186; break; }
                                    case 84: { itemID = 40708016; break; }
                                    case 85: { itemID = 40708176; break; }
                                    case 86: { itemID = 40708086; break; }
                                    case 87: { itemID = 40708046; break; }
                                    case 88: { itemID = 40708186; break; }



                                }
                                client.Inventory.Add(stream, itemID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(client.ActiveNpc, 1, stream))
                            {
                                uint itemID = 0;
                                switch (Option)
                                {
                                    case 49: { itemID = 40704016; break; }
                                    case 50: { itemID = 40704176; break; }
                                    case 51: { itemID = 40704086; break; }
                                    case 52: { itemID = 40704046; break; }
                                    case 53: { itemID = 40704186; break; }
                                    case 54: { itemID = 40706016; break; }
                                    case 55: { itemID = 40706176; break; }
                                    case 56: { itemID = 40706086; break; }
                                    case 57: { itemID = 40706046; break; }
                                    case 58: { itemID = 40706186; break; }
                                    case 59: { itemID = 40707016; break; }
                                    case 60: { itemID = 40707176; break; }
                                    case 61: { itemID = 40707086; break; }
                                    case 62: { itemID = 40707046; break; }
                                    case 63: { itemID = 40707186; break; }
                                    case 64: { itemID = 40703016; break; }
                                    case 65: { itemID = 40703176; break; }
                                    case 66: { itemID = 40703086; break; }
                                    case 67: { itemID = 40703046; break; }
                                    case 68: { itemID = 40703186; break; }
                                    case 69: { itemID = 40705016; break; }
                                    case 70: { itemID = 40705176; break; }
                                    case 71: { itemID = 40705086; break; }
                                    case 72: { itemID = 40705046; break; }
                                    case 73: { itemID = 40705186; break; }
                                    case 74: { itemID = 40701016; break; }
                                    case 75: { itemID = 40701176; break; }
                                    case 76: { itemID = 40701086; break; }
                                    case 77: { itemID = 40701046; break; }
                                    case 78: { itemID = 40701186; break; }
                                    case 79: { itemID = 40702016; break; }
                                    case 80: { itemID = 40702176; break; }
                                    case 81: { itemID = 40702086; break; }
                                    case 82: { itemID = 40702046; break; }
                                    case 83: { itemID = 40702186; break; }
                                    case 84: { itemID = 40708016; break; }
                                    case 85: { itemID = 40708176; break; }
                                    case 86: { itemID = 40708086; break; }
                                    case 87: { itemID = 40708046; break; }
                                    case 88: { itemID = 40708186; break; }

                                }
                                client.Inventory.Add(stream, itemID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            }
                        }

                        break;
                    }
            }
        }
        public static void ParasolScrap(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        dialog.Text("Please pick the Parasol Scrap you want.");
                        dialog.Option("Lotus Dew Scrap.", 1);
                        dialog.Option("Begonia Frost Scrap.", 2);
                        dialog.Option("Snowsweet Melody Scrap.", 3);
                        dialog.Option("Peach Bloom Scrap.", 4);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(id, 1))
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(3326260, 0, 1, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(3326260, 0, 1, stream, true);
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
                                client.Inventory.AddItemWitchStack(3326261, 0, 1, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(3326261, 0, 1, stream, true);
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
                                client.Inventory.AddItemWitchStack(3326262, 0, 1, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(3326262, 0, 1, stream, true);
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
                                client.Inventory.AddItemWitchStack(3326263, 0, 1, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(3326263, 0, 1, stream, true);
                            }

                        }
                        break;
                    }


            }
        }
        public static void FancyParasol(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        dialog.Text("Please pick the Parasol you wan.");
                        dialog.Option("Lotus Dew.", 1);
                        dialog.Option("Begonia Frost.", 2);
                        dialog.Option("Snowsweet Melody.", 3);
                        dialog.Option("Peach Bloom.", 4);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(id, 1))
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4300001, 0, 1, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4300001, 0, 1, stream, true);
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
                                client.Inventory.AddItemWitchStack(4300002, 0, 1, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4300002, 0, 1, stream, true);
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
                                client.Inventory.AddItemWitchStack(4300003, 0, 1, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4300003, 0, 1, stream, true);
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
                                client.Inventory.AddItemWitchStack(4300004, 0, 1, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(4300004, 0, 1, stream, true);
                            }

                        }
                        break;
                    }


            }
        }
        public static void PrimaryAwakening(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        dialog.Text("Please pick the reward you want.");
                        dialog.Option("10 Polar Pebbles.", 1);
                        dialog.Option("10 Normal Soul Stones.", 2);
                        dialog.Option("10 Gold Triggers.", 3);
                        dialog.Option("3 Sigil Box Scraps.", 4);
                        dialog.Option("10 Arcane Essences.", 5);
                        dialog.Option("10 Legendary Essences.", 6);
                        dialog.Option("10 Spirit Stones.", 7);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(id, 1))
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(3328251, 0, 10, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(3328251, 0, 10, stream, true);
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
                                client.Inventory.AddItemWitchStack(3333400, 0, 10, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(3333400, 0, 10, stream, true);
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
                                client.Inventory.AddItemWitchStack(3335125, 0, 10, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(3335125, 0, 10, stream, true);
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
                                client.Inventory.AddItemWitchStack(3327202, 0, 3, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(3327202, 0, 3, stream, true);
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
                                client.Inventory.AddItemWitchStack(3316501, 0, 10, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(3316501, 0, 10, stream, true);
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
                                client.Inventory.AddItemWitchStack(3321098, 0, 10, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(3321098, 0, 10, stream, true);
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
                                client.Inventory.AddItemWitchStack(3321107, 0, 10, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(3321107, 0, 10, stream, true);
                            }
                        }
                        break;
                    }

            }
        }
        public static void MediumAwakening(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        dialog.Text("Please pick the reward you want.");
                        dialog.Option("10 Orient Pebbles.", 1);
                        dialog.Option("10 Refined Soul Stones.", 2);
                        dialog.Option("10 Kylin Triggers.", 3);
                        dialog.Option("10 Sigil Box Scraps.", 4);
                        dialog.Option("40 Arcane Essences.", 5);
                        dialog.Option("40 Legendary Essences.", 6);
                        dialog.Option("40 Spirit Stones.", 7);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(id, 1))
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(3328252, 0, 10, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(3328252, 0, 10, stream, true);
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
                                client.Inventory.AddItemWitchStack(3333401, 0, 10, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(3333401, 0, 10, stream, true);
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
                                client.Inventory.AddItemWitchStack(3335126, 0, 10, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(3335126, 0, 10, stream, true);
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
                                client.Inventory.AddItemWitchStack(3327202, 0, 10, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(3327202, 0, 10, stream, true);
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
                                client.Inventory.AddItemWitchStack(3316501, 0, 40, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(3316501, 0, 40, stream);
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
                                client.Inventory.AddItemWitchStack(3321098, 0, 40, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(3321098, 0, 40, stream, true);
                            }
                        }
                        break;
                    }
                case 7:
                    {
                        if (client.Inventory.Contain(id, 1))
                        {
                            client.Inventory.AddItemWitchStack(3321098, 0, 40, stream);

                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(3321107, 0, 40, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(3321107, 0, 40, stream, true);
                            }
                        }
                        break;
                    }
            }
        }
        public static void AdvancedAwakening(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        dialog.Text("Please pick the reward you want.");
                        dialog.Option("10 Dipper Pebbles.", 1);
                        dialog.Option("10 Elite Soul Stones.", 2);
                        dialog.Option("10 Dragon Triggers.", 3);
                        dialog.Option("33 Sigil Box Scraps.", 4);
                        dialog.Option("120 Arcane Essences.", 5);
                        dialog.Option("120 Legendary Essences.", 6);
                        dialog.Option("120 Spirit Stones.", 7);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(id, 1))
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(3328253, 0, 10, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(3328253, 0, 10, stream, true);
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
                                client.Inventory.AddItemWitchStack(3333402, 0, 10, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(3333402, 0, 10, stream, true);
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
                                client.Inventory.AddItemWitchStack(3335127, 0, 10, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(3335127, 0, 10, stream, true);
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
                                client.Inventory.AddItemWitchStack(3327202, 0, 33, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(3327202, 0, 33, stream, true);
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
                                client.Inventory.AddItemWitchStack(3316501, 0, 120, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(3316501, 0, 120, stream, true);
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
                                client.Inventory.AddItemWitchStack(3321098, 0, 120, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(3321098, 0, 120, stream, true);
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
                                client.Inventory.AddItemWitchStack(3321107, 0, 120, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(3321107, 0, 120, stream, true);
                            }
                        }
                        break;
                    }
            }
        }
        public static void UltimateAwakening(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        dialog.Text("Please pick the reward you want.");
                        dialog.Option("10 Focus Pebbles.", 1);
                        dialog.Option("10 Super Soul Stones.", 2);
                        dialog.Option("10 Saint Triggers.", 3);
                        dialog.Option("100 Sigil Box Scraps.", 4);
                        dialog.Option("360 Arcane Essences.", 5);
                        dialog.Option("360 Legendary Essences.", 6);
                        dialog.Option("360 Spirit Stones.", 7);
                        dialog.Avatar(93).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(id, 1))
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(3328254, 0, 10, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(3328254, 0, 10, stream, true);
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
                                client.Inventory.AddItemWitchStack(3333403, 0, 10, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(3333403, 0, 10, stream, true);
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
                                client.Inventory.AddItemWitchStack(3335128, 0, 10, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(3335128, 0, 10, stream, true);
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
                                client.Inventory.AddItemWitchStack(3327202, 0, 100, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(3327202, 0, 100, stream, true);
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
                                client.Inventory.AddItemWitchStack(3316501, 0, 360, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(3316501, 0, 360, stream, true);
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
                                client.Inventory.AddItemWitchStack(3321098, 0, 360, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(3321098, 0, 360, stream, true);
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
                                client.Inventory.AddItemWitchStack(3321107, 0, 360, stream);
                            }
                        }
                        else
                        {
                            if (client.Inventory.Remove(id, 1, stream))
                            {
                                client.Inventory.AddItemWitchStack(3321107, 0, 360, stream, true);
                            }
                        }
                        break;
                    }

            }
        }
        public static void FateRuneSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~Rare~Yellow~Rune~Fragment~or~50~Peach~Jades.");
                        dialog.AddOption("1~Rare~Yellow~Rune~Fragment", 1);
                        dialog.AddOption("50~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3311744, 1);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 50, stream);
                        }
                        break;
                    }
            }
        }
        public static void FateWealthSelectionBox2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~Class5~Money~Bag~or~100~Peach~Jades.");
                        dialog.AddOption("1~Class5~Money~Bag", 1);
                        dialog.AddOption("100~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 723717, 1);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 100, stream);
                        }
                        break;
                    }
            }
        }
        public static void FateStoneSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~+3~Stone~or~20~Peach~Jades.");
                        dialog.AddOption("1~+3~Stone", 1);
                        dialog.AddOption("20~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 730003, 1);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 20, stream);
                        }
                        break;
                    }
            }
        }
        public static void RuneStoneSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~Rune~Stone~Pack~or~30~Peach~Jades.");
                        dialog.AddOption("1~Rune~Stone~Pack", 1);
                        dialog.AddOption("30~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3319320, 1);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 30, stream);
                        }
                        break;
                    }
            }
        }
        public static void FateScrapSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~Bliss~Pack~Fragment~or~50~Peach~Jades.");
                        dialog.AddOption("1~Bliss~Pack~Fragment", 1);
                        dialog.AddOption("50~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3331670, 1);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 50, stream);
                        }
                        break;
                    }
            }
        }
        public static void P8BlissPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~one~from~5~Penitence~Amulets/1~Bright~Star~Ticket/1~Sigil~Box~Scrap/10~Rune~Essences/1~+3~Stone/1~Rare~Yellow~Rune~Fragment/1~Class5~Money~Bag.");
                        dialog.AddOption("5~Penitence~Amulets", 1);
                        dialog.AddOption("1~Bright~Star~Ticket", 2);
                        dialog.AddOption("1~Sigil~Box~Scrap", 3);
                        dialog.AddOption("10~Rune~Essences", 4);
                        dialog.AddOption("1~+3~Stone", 5);
                        dialog.AddOption("1~Rare~Yellow~Rune~Fragment", 6);
                        dialog.AddOption("1~Class5~Money~Bag.", 7);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(720128, 0, 5, stream);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3200726, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false, Role.Flags.ItemEffect.None, true, "");
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3327202, 0, 1, stream);
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(4060001, 0, 10, stream);
                        }
                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 730003, 1);
                        }
                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3330765, 1);
                        }
                        break;
                    }
                case 7:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 723717, 1);
                        }
                        break;
                    }
            }
        }
        public static void P8PeachSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~P8~Bliss~Pack~or~40~Peach~Jades.");
                        dialog.AddOption("1~P8~Bliss~Pack", 1);
                        dialog.AddOption("40~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3331653, 1);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 40, stream);
                        }
                        break;
                    }
            }
        }
        public static void FatePotencySelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~3000~Potency~Pack~or~100~Peach~Jades.");
                        dialog.AddOption("3000~Potency~Pack", 1);
                        dialog.AddOption("100~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3322497, 1);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 100, stream);
                        }
                        break;
                    }
            }
        }
        public static void ValueStoneSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~+4~Stone~or~50~Peach~Jades.");
                        dialog.AddOption("1~+4~Stone", 1);
                        dialog.AddOption("50~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 730004, 1);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 50, stream);
                        }
                        break;
                    }
            }
        }
        public static void FateWealthSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~10KK~Silver~Pack~or~200~Peach~Jades.");
                        dialog.AddOption("10KK~Silver~Pack", 1);
                        dialog.AddOption("200~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3330289, 1);
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
        public static void RelicSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~Luxury~Relic~Box~or~99~Peach~Jades.");
                        dialog.AddOption("1~Luxury~Relic~Box", 1);
                        dialog.AddOption("99~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3314915, 1);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 99, stream);
                        }
                        break;
                    }
            }
        }
        public static void RuneSigilSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~Arcane~Rune~Pack~or~80~Peach~Jades.");
                        dialog.AddOption("1~Arcane~Rune~Pack", 1);
                        dialog.AddOption("80~Peach~Jades.", 2);
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
                            client.Inventory.AddItemWitchStack(3316793, 0, 80, stream);
                        }
                        break;
                    }
            }
        }
        public static void P7BlissPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~one~from~10~Arcane~Essences/30~Universal~Rune~Essences/1~Soul~Fruit/1~Luxury~Relic~Box/1~+4~Stone/10KK~Silver~Pack.");
                        dialog.AddOption("10~Arcane~Essences", 1);
                        dialog.AddOption("30~Universal~Rune~Essences", 2);
                        dialog.AddOption("1~Soul~Fruit", 3);
                        dialog.AddOption("1~Luxury~Relic~Box", 4);
                        dialog.AddOption("1~+4~Stone", 5);
                        dialog.AddOption("10KK~Silver~Pack", 6);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316501, 0, 10, stream);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(4060001, 0, 30, stream);
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3009103, 0, 1, stream);
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3314915, 1, 0, 0);
                        }
                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 730004, 1);
                        }
                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3330289, 1);
                        }
                        break;
                    }
            }
        }
        public static void P7FateSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~P7~Bliss~Pack~or~100~Peach~Jades.");
                        dialog.AddOption("1~P7~Bliss~Pack", 1);
                        dialog.AddOption("100~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3331654, 1);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 100, stream);
                        }
                        break;
                    }
            }
        }
        public static void FateSteedSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~+5~Steed~or~200~Peach~Jades.");
                        dialog.AddOption("1~+5~Steed", 1);
                        dialog.AddOption("200~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddSteed(stream, 300000, 1, 5, false, 0, 0, 0);
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
        public static void HonorStoneSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
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
        public static void YellowRuneSelectionBox2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~Yellow~Rune~Pack~or~300~Peach~Jades.");
                        dialog.AddOption("1~Yellow~Rune~Pack", 1);
                        dialog.AddOption("300~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3314248, 1);
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
        public static void Class5FortuneSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~30KK~Silver~Pack~or~600~Peach~Jades.");
                        dialog.AddOption("30KK~Silver~Pack", 1);
                        dialog.AddOption("600~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 3331789, 1);
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
        public static void DragonBallSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~Dragon~Ball~or~30~Peach~Jades.");
                        dialog.AddOption("1~Super~Tortoise~Gem", 1);
                        dialog.AddOption("30~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 1088000, 1);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316793, 0, 30, stream);
                        }
                        break;
                    }
            }
        }
        public static void TortoiseGemSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~Super~Tortoise~Gem~or~150~Peach~Jades.");
                        dialog.AddOption("1~Super~Tortoise~Gem", 1);
                        dialog.AddOption("150~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 700073, 1);
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
        public static void ThunderGemSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~Super~Thunder~Gem~or~300~Peach~Jades.");
                        dialog.AddOption("1~Super~Thunder~Gem", 1);
                        dialog.AddOption("300~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 700103, 1);
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
        public static void GloryGemSelectionBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~1~Super~Glory~Gem~or~300~Peach~Jades.");
                        dialog.AddOption("1~Super~Glory~Gem", 1);
                        dialog.AddOption("300~Peach~Jades.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.Add(stream, 700123, 1);
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
        public static void ArcaneRunePack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open~to~choose~one~from~20~Arcane~Essences/50~Universal~Rune~Essences.");
                        dialog.AddOption("20~Arcane~Essences", 1);
                        dialog.AddOption("50~Universal~Rune~Essences.", 2);
                        dialog.AddAvatar(93);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(3316501, 0, 20, stream);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(id, 1, stream))
                        {
                            client.Inventory.AddItemWitchStack(4060001, 0, 50, stream);
                        }
                        break;
                    }
            }
        }
        #endregion
        #endregion
    }
}

