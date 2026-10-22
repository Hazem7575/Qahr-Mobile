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
        [NpcAttribute(NpcID.EpicTrojan)]
        public static void EpicTrojan(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain((uint)NpcID.EpicTrojan, 1))
            {
                client.SendSysMesage("You Don't have this item in Inventory");
                return;
            }
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.Text("When your right hand is equipped with an epic weapon, the Weapon Trojan on your hand can be converted into an epic weapon\n");
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
                            if (Database.ItemType.IsClub(GameItem.ITEM_ID) || Database.ItemType.IsScepter(GameItem.ITEM_ID)
                                ||
                                Database.ItemType.IsDagger(GameItem.ITEM_ID)
                                || Database.ItemType.IsBlade(GameItem.ITEM_ID) || Database.ItemType.IsSword(GameItem.ITEM_ID))
                            {
                                uint UpdateToEpic = (GameItem.ITEM_ID % 1000) + 614000;
                                GameItem.ITEM_ID = UpdateToEpic;
                                GameItem.Mode = Role.Flags.ItemMode.Update;
                                GameItem.Send(client, stream);
                                client.Inventory.Remove(3305414, 1, stream);
                                client.Equipment.QueryEquipment(client.Equipment.Alternante, true);


                                Server.SendGlobalPacket(new MsgMessage("Congratulation! " + client.Player.Name + " has finished the Trojan Epic Quest.", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));


                                client.Inventory.Update(GameItem, Role.Instance.AddMode.REMOVE, stream);
                            }


                        }
                        else
                        {
                            client.SendSysMesage("Soryy, You're not equipped with the Weapon Trojan(right weapon).");

                        }
                        break;
                    }
                case 4:
                    {
                        MsgGameItem GameItem;
                        if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.LeftWeapon, out GameItem))
                        {
                            if (Database.ItemType.IsClub(GameItem.ITEM_ID) || Database.ItemType.IsScepter(GameItem.ITEM_ID) ||
                                Database.ItemType.IsDagger(GameItem.ITEM_ID)|| Database.ItemType.IsBlade(GameItem.ITEM_ID) || Database.ItemType.IsSword(GameItem.ITEM_ID))
                            {
                                uint UpdateToEpic = (GameItem.ITEM_ID % 1000) + 614000;
                                GameItem.ITEM_ID = UpdateToEpic;
                                GameItem.Mode = Role.Flags.ItemMode.Update;
                                GameItem.Send(client, stream);
                                client.Inventory.Remove(3305414, 1, stream);
                                client.Equipment.QueryEquipment(client.Equipment.Alternante, true);


                                Server.SendGlobalPacket(new MsgMessage("Congratulation! " + client.Player.Name + " has finished the Trojan Epic Quest.", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));


                                client.Inventory.Update(GameItem, Role.Instance.AddMode.REMOVE, stream);
                            }


                        }
                        else
                        {
                            client.SendSysMesage("Soryy, You're not equipped with the Weapon Trojan(right weapon).");

                        }
                        break;
                    }

            }

        }
        #region Epic Ninja
        [NpcAttribute(NpcID.OneStDivineShadow)]
        public static void OneStDivineShadow(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.Text("97221200_0101@@");
                        data.Text("97221201_0101@@");
                        data.Option("97221204_0102@@", 1);

                        if (client.Inventory.Contain(3004462, 1))
                            data.Option("97221205_0102@@", 2);
                        else
                            data.Option("97221208_0102@@", 255);

                        if (client.Inventory.Contain(3004463, 1))
                            data.Option("97221206_0102@@", 3);
                        else
                            data.Option("97221212_0102@@", 255);
                        data.Option("Tell me More.", 255);
                        data.Option("97221216_0102@@", 4);
                        data.Option("Close ", 255);
                        data.AddAvatar(0).FinalizeDialog();
                        break;
                    }

                case 2:
                    {
                        client.Teleport(32, 35, 3850, 0);
                        break;
                    }
                case 3:
                    {
                        client.Teleport(32, 35, 3851, 0);
                        break;
                    }
                case 4:
                    {
                        client.Teleport(70, 78, 10445);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.TwoStDivineShadow)]
        public static void TwoStDivineShadow(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.Text("97221200_0101@@");
                        data.Text("97221201_0101@@");
                        data.Option("97221208_0102@@", 2);
                        if (client.Inventory.Contain(3004463, 1))
                            data.Option("97221206_0102@@", 3);
                        else
                            data.Option("97221212_0102@@", 255);
                        data.Option("97221210_0102@@", 1);
                        data.Option("Tell me More.", 255);
                        data.Option("97221216_0102@@", 4);
                        data.Option("Close ", 255);
                        data.AddAvatar(0).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Teleport(32, 35, 3849, 0);
                        break;
                    }

                case 3:
                    {
                        client.Teleport(32, 35, 3851, 0);
                        break;
                    }
                case 4:
                    {
                        client.Teleport(70, 78, 10445);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ThreeStDivineShadow)]
        public static void ThreeStDivineShadow(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.Text("97221200_0101@@");
                        data.Text("97221201_0101@@");
                        data.Option("97221212_0102@@", 3);
                        data.Option("97221213_0102@@", 2);
                        data.Option("97221210_0102@@", 1);
                        data.Option("Tell me More.", 255);
                        data.Option("97221216_0102@@", 4);
                        data.Option("Close ", 255);
                        data.AddAvatar(0).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Teleport(32, 35, 3849, 0);
                        break;
                    }
                case 2:
                    {
                        client.Teleport(32, 35, 3850, 0);
                        break;
                    }
                case 4:
                    {
                        client.Teleport(70, 78, 10445);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.OneStSwapperStarry)]
        public static void OneStSwapperStarry(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.Text("97221600_0101@@");
                        data.Option("97221602_0102@@", 1);
                        data.Option("97221603_0102@@", 2);
                        data.Option("97221604_0102@@", 3);
                        data.Option("97221605_0102@@", 4);
                        data.Option("97221606_0102@@", 5);
                        data.Option("I`ll think about it.", 255);
                        data.AddAvatar(0).FinalizeDialog();
                        break;
                    }
                case 1:
                    {

                        data.Text("97221610_0101@@");
                        data.Text("97221611_0101@@");
                        data.Text("97221612_0101@@");
                        data.Option("97221614_0102@@", 10);
                        data.Option("97221615_0102@@", 11);
                        data.Option("97221616_0102@@", 12);
                        data.Option("Previous .", 13);
                        data.AddAvatar(0).FinalizeDialog();
                        break;
                    }
                case 2:
                    {

                        data.Text("97221671_0101@@");
                        data.Option("Yes", 17);
                        data.Option("No", 255);
                        data.AddAvatar(0).FinalizeDialog();
                        break;
                    }
                case 3:
                    {

                        data.Text("97221691_0101@@");
                        data.Option("Yes", 18);
                        data.Option("No", 255);
                        data.AddAvatar(0).FinalizeDialog();
                        break;
                    }
                case 4:
                    {

                        data.Text("97222212_0101@@");
                        data.Option("Yes", 19);
                        data.Option("No", 255);
                        data.AddAvatar(0).FinalizeDialog();
                        break;
                    }
                case 17:
                    {
                        if (client.Inventory.Contain(3004465, 5))
                        {
                            client.Inventory.RemoveStackItem(3004465, 5, stream);
                            client.Player.MyChi.ChiPoints += 1200;
                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                            client.CreateBoxDialog("97221685_0105@@");
                        }
                        else
                        {
                            client.CreateBoxDialog("97221681_0105@@");
                        }
                        break;
                    }
                case 18:
                    {
                        if (client.Inventory.Contain(3004465, 1))
                        {
                            client.Inventory.Remove(3004465, 1, stream);
                            client.Inventory.AddItemWitchStack(3002030, 0, 3, stream);
                            client.CreateBoxDialog("97221707_0105@@");
                        }
                        else
                        {
                            client.CreateBoxDialog("97221701_0105@@");
                        }
                        break;
                    }
                case 19:
                    {

                        if (client.Player.MyJiangHu != null)
                        {
                            if (client.Player.MyJiangHu.FreeCourse <= 900000)
                            {
                                if (client.Inventory.Contain(3004465, 2))
                                {
                                    client.Inventory.RemoveStackItem(3004465, 2, stream);
                                    client.Player.MyJiangHu.FreeCourse += 100000;
                                    client.CreateBoxDialog("97222209_0105@@");
                                }
                                else
                                {
                                    client.CreateBoxDialog("97222201_0105@@");
                                }
                            }
                            else
                            {
                                client.CreateBoxDialog("97222203_0105@@");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("97222203_0105@@");
                        }

                        break;
                    }
                case 5:
                    {
                        data.Text("97221720_0101@@");
                        data.Option("97221614_0102@@", 14);
                        data.Option("97221615_0102@@", 15);
                        data.Option("97221616_0102@@", 16);
                        data.Option("Previous .", 13);
                        data.AddAvatar(0).FinalizeDialog();
                        break;
                    }
                case 10:
                    {

                        if (client.Inventory.Contain(3004465, 1))
                        {
                            client.Inventory.Remove(3004465, 1, stream);
                            client.Inventory.Add(stream, 3004459, 1);
                            client.CreateBoxDialog("97221647_0105@@");
                        }
                        else
                        {
                            client.CreateBoxDialog("97221621_0105@@");
                        }
                        break;
                    }

                case 11:
                    {

                        if (client.Inventory.Contain(3004465, 10))
                        {
                            client.Inventory.RemoveStackItem(3004465, 10, stream);
                            client.Inventory.Add(stream, 3004461, 1);
                            client.CreateBoxDialog("97221627_0105@@");
                        }
                        else
                        {
                            client.CreateBoxDialog("97221621_0105@@");

                        }
                        break;
                    }
                case 12:
                    {

                        if (client.Inventory.Contain(3004465, 20))
                        {
                            client.Inventory.RemoveStackItem(3004465, 20, stream);
                            client.Inventory.Add(stream, 3004460, 1);
                            client.CreateBoxDialog("97221667_0105@@");
                        }
                        else
                        {
                            client.CreateBoxDialog("97221621_0105@@");
                        }
                        break;
                    }
                case 13:
                    {
                        goto case 0;
                    }
                case 14:
                    {

                        if (client.Inventory.Contain(3004459, 1))
                        {
                            client.Inventory.Remove(3004459, 1, stream);
                            client.Inventory.AddItemWitchStack(3004465, 0, 1, stream);

                            client.CreateBoxDialog("97221765_0105@@");
                        }
                        else
                        {
                            client.CreateBoxDialog("97221741_0105@@");
                        }
                        break;
                    }
                case 15:
                    {

                        if (client.Inventory.Contain(3004461, 1))
                        {
                            client.Inventory.Remove(3004461, 1, stream);
                            client.Inventory.AddItemWitchStack(3004465, 0, 10, stream);

                            client.CreateBoxDialog("97221745_0105@@");
                        }
                        else
                        {
                            client.CreateBoxDialog("97221741_0105@@");
                        }
                        break;
                    }
                case 16:
                    {

                        if (client.Inventory.Contain(3004460, 1))
                        {
                            client.Inventory.Remove(3004460, 1, stream);
                            client.Inventory.AddItemWitchStack(3004465, 0, 20, stream);

                            client.CreateBoxDialog("97221785_0105@@");
                        }
                        else
                        {
                            client.CreateBoxDialog("97221741_0105@@");
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.TwoStSwapperStarry)]
        public static void TwoStSwapperStarry(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.Text("97221600_0101@@");
                        data.Option("97221602_0102@@", 1);
                        data.Option("97221603_0102@@", 2);
                        data.Option("97221604_0102@@", 3);
                        data.Option("97221605_0102@@", 4);
                        data.Option("97221606_0102@@", 5);
                        data.Option("I`ll think about it.", 255);
                        data.AddAvatar(0).FinalizeDialog();
                        break;
                    }
                case 1:
                    {

                        data.Text("97221610_0101@@");
                        data.Text("97221611_0101@@");
                        data.Text("97221612_0101@@");
                        data.Option("97221614_0102@@", 10);
                        data.Option("97221615_0102@@", 11);
                        data.Option("97221616_0102@@", 12);
                        data.Option("Previous .", 13);
                        data.AddAvatar(0).FinalizeDialog();
                        break;
                    }
                case 2:
                    {

                        data.Text("97221671_0101@@");
                        data.Option("Yes", 17);
                        data.Option("No", 255);
                        data.AddAvatar(0).FinalizeDialog();
                        break;
                    }
                case 3:
                    {

                        data.Text("97221691_0101@@");
                        data.Option("Yes", 18);
                        data.Option("No", 255);
                        data.AddAvatar(0).FinalizeDialog();
                        break;
                    }
                case 4:
                    {

                        data.Text("97222212_0101@@");
                        data.Option("Yes", 19);
                        data.Option("No", 255);
                        data.AddAvatar(0).FinalizeDialog();
                        break;
                    }
                case 17:
                    {
                        if (client.Inventory.Contain(3004465, 5))
                        {
                            client.Inventory.RemoveStackItem(3004465, 5, stream);
                            client.Player.MyChi.ChiPoints += 1200;
                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                            client.CreateBoxDialog("97221685_0105@@");
                        }
                        else
                        {
                            client.CreateBoxDialog("97221681_0105@@");
                        }
                        break;
                    }
                case 18:
                    {
                        if (client.Inventory.Contain(3004465, 1))
                        {
                            client.Inventory.Remove(3004465, 1, stream);
                            client.Inventory.AddItemWitchStack(3002030, 0, 3, stream);
                            client.CreateBoxDialog("97221707_0105@@");
                        }
                        else
                        {
                            client.CreateBoxDialog("97221701_0105@@");
                        }
                        break;
                    }
                case 19:
                    {

                        if (client.Player.MyJiangHu != null)
                        {
                            if (client.Player.MyJiangHu.FreeCourse <= 900000)
                            {
                                if (client.Inventory.Contain(3004465, 2))
                                {
                                    client.Inventory.RemoveStackItem(3004465, 2, stream);
                                    client.Player.MyJiangHu.FreeCourse += 100000;
                                    client.CreateBoxDialog("97222209_0105@@");
                                }
                                else
                                {
                                    client.CreateBoxDialog("97222201_0105@@");
                                }
                            }
                            else
                            {
                                client.CreateBoxDialog("97222203_0105@@");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("97222203_0105@@");
                        }

                        break;
                    }
                case 5:
                    {
                        data.Text("97221720_0101@@");
                        data.Option("97221614_0102@@", 14);
                        data.Option("97221615_0102@@", 15);
                        data.Option("97221616_0102@@", 16);
                        data.Option("Previous .", 13);
                        data.AddAvatar(0).FinalizeDialog();
                        break;
                    }
                case 10:
                    {

                        if (client.Inventory.Contain(3004465, 1))
                        {
                            client.Inventory.Remove(3004465, 1, stream);
                            client.Inventory.Add(stream, 3004459, 1);
                            client.CreateBoxDialog("97221647_0105@@");
                        }
                        else
                        {
                            client.CreateBoxDialog("97221621_0105@@");
                        }
                        break;
                    }

                case 11:
                    {

                        if (client.Inventory.Contain(3004465, 10))
                        {
                            client.Inventory.RemoveStackItem(3004465, 10, stream);
                            client.Inventory.Add(stream, 3004461, 1);
                            client.CreateBoxDialog("97221627_0105@@");
                        }
                        else
                        {
                            client.CreateBoxDialog("97221621_0105@@");

                        }
                        break;
                    }
                case 12:
                    {

                        if (client.Inventory.Contain(3004465, 20))
                        {
                            client.Inventory.RemoveStackItem(3004465, 20, stream);
                            client.Inventory.Add(stream, 3004460, 1);
                            client.CreateBoxDialog("97221667_0105@@");
                        }
                        else
                        {
                            client.CreateBoxDialog("97221621_0105@@");
                        }
                        break;
                    }
                case 13:
                    {
                        goto case 0;
                    }
                case 14:
                    {

                        if (client.Inventory.Contain(3004459, 1))
                        {
                            client.Inventory.Remove(3004459, 1, stream);
                            client.Inventory.AddItemWitchStack(3004465, 0, 1, stream);

                            client.CreateBoxDialog("97221765_0105@@");
                        }
                        else
                        {
                            client.CreateBoxDialog("97221741_0105@@");
                        }
                        break;
                    }
                case 15:
                    {

                        if (client.Inventory.Contain(3004461, 1))
                        {
                            client.Inventory.Remove(3004461, 1, stream);
                            client.Inventory.AddItemWitchStack(3004465, 0, 10, stream);

                            client.CreateBoxDialog("97221745_0105@@");
                        }
                        else
                        {
                            client.CreateBoxDialog("97221741_0105@@");
                        }
                        break;
                    }
                case 16:
                    {

                        if (client.Inventory.Contain(3004460, 1))
                        {
                            client.Inventory.Remove(3004460, 1, stream);
                            client.Inventory.AddItemWitchStack(3004465, 0, 20, stream);

                            client.CreateBoxDialog("97221785_0105@@");
                        }
                        else
                        {
                            client.CreateBoxDialog("97221741_0105@@");
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ThreeStSwapperStarry)]
        public static void ThreeStSwapperStarry(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.Text("97221600_0101@@");
                        data.Option("97221602_0102@@", 1);
                        data.Option("97221603_0102@@", 2);
                        data.Option("97221604_0102@@", 3);
                        data.Option("97221605_0102@@", 4);
                        data.Option("97221606_0102@@", 5);
                        data.Option("I`ll think about it.", 255);
                        data.AddAvatar(0).FinalizeDialog();
                        break;
                    }
                case 1:
                    {

                        data.Text("97221610_0101@@");
                        data.Text("97221611_0101@@");
                        data.Text("97221612_0101@@");
                        data.Option("97221614_0102@@", 10);
                        data.Option("97221615_0102@@", 11);
                        data.Option("97221616_0102@@", 12);
                        data.Option("Previous .", 13);
                        data.AddAvatar(0).FinalizeDialog();
                        break;
                    }
                case 2:
                    {

                        data.Text("97221671_0101@@");
                        data.Option("Yes", 17);
                        data.Option("No", 255);
                        data.AddAvatar(0).FinalizeDialog();
                        break;
                    }
                case 3:
                    {

                        data.Text("97221691_0101@@");
                        data.Option("Yes", 18);
                        data.Option("No", 255);
                        data.AddAvatar(0).FinalizeDialog();
                        break;
                    }
                case 4:
                    {

                        data.Text("97222212_0101@@");
                        data.Option("Yes", 19);
                        data.Option("No", 255);
                        data.AddAvatar(0).FinalizeDialog();
                        break;
                    }
                case 17:
                    {
                        if (client.Inventory.Contain(3004465, 5))
                        {
                            client.Inventory.RemoveStackItem(3004465, 5, stream);
                            client.Player.MyChi.ChiPoints += 1200;
                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                            client.CreateBoxDialog("97221685_0105@@");
                        }
                        else
                        {
                            client.CreateBoxDialog("97221681_0105@@");
                        }
                        break;
                    }
                case 18:
                    {
                        if (client.Inventory.Contain(3004465, 1))
                        {
                            client.Inventory.Remove(3004465, 1, stream);
                            client.Inventory.AddItemWitchStack(3002030, 0, 3, stream);
                            client.CreateBoxDialog("97221707_0105@@");
                        }
                        else
                        {
                            client.CreateBoxDialog("97221701_0105@@");
                        }
                        break;
                    }
                case 19:
                    {

                        if (client.Player.MyJiangHu != null)
                        {
                            if (client.Player.MyJiangHu.FreeCourse <= 900000)
                            {
                                if (client.Inventory.Contain(3004465, 2))
                                {
                                    client.Inventory.RemoveStackItem(3004465, 2, stream);
                                    client.Player.MyJiangHu.FreeCourse += 100000;
                                    client.CreateBoxDialog("97222209_0105@@");
                                }
                                else
                                {
                                    client.CreateBoxDialog("97222201_0105@@");
                                }
                            }
                            else
                            {
                                client.CreateBoxDialog("97222203_0105@@");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("97222203_0105@@");
                        }

                        break;
                    }
                case 5:
                    {
                        data.Text("97221720_0101@@");
                        data.Option("97221614_0102@@", 14);
                        data.Option("97221615_0102@@", 15);
                        data.Option("97221616_0102@@", 16);
                        data.Option("Previous .", 13);
                        data.AddAvatar(0).FinalizeDialog();
                        break;
                    }
                case 10:
                    {

                        if (client.Inventory.Contain(3004465, 1))
                        {
                            client.Inventory.Remove(3004465, 1, stream);
                            client.Inventory.Add(stream, 3004459, 1);
                            client.CreateBoxDialog("97221647_0105@@");
                        }
                        else
                        {
                            client.CreateBoxDialog("97221621_0105@@");
                        }
                        break;
                    }

                case 11:
                    {

                        if (client.Inventory.Contain(3004465, 10))
                        {
                            client.Inventory.RemoveStackItem(3004465, 10, stream);
                            client.Inventory.Add(stream, 3004461, 1);
                            client.CreateBoxDialog("97221627_0105@@");
                        }
                        else
                        {
                            client.CreateBoxDialog("97221621_0105@@");

                        }
                        break;
                    }
                case 12:
                    {

                        if (client.Inventory.Contain(3004465, 20))
                        {
                            client.Inventory.RemoveStackItem(3004465, 20, stream);
                            client.Inventory.Add(stream, 3004460, 1);
                            client.CreateBoxDialog("97221667_0105@@");
                        }
                        else
                        {
                            client.CreateBoxDialog("97221621_0105@@");
                        }
                        break;
                    }
                case 13:
                    {
                        goto case 0;
                    }
                case 14:
                    {

                        if (client.Inventory.Contain(3004459, 1))
                        {
                            client.Inventory.Remove(3004459, 1, stream);
                            client.Inventory.AddItemWitchStack(3004465, 0, 1, stream);

                            client.CreateBoxDialog("97221765_0105@@");
                        }
                        else
                        {
                            client.CreateBoxDialog("97221741_0105@@");
                        }
                        break;
                    }
                case 15:
                    {

                        if (client.Inventory.Contain(3004461, 1))
                        {
                            client.Inventory.Remove(3004461, 1, stream);
                            client.Inventory.AddItemWitchStack(3004465, 0, 10, stream);

                            client.CreateBoxDialog("97221745_0105@@");
                        }
                        else
                        {
                            client.CreateBoxDialog("97221741_0105@@");
                        }
                        break;
                    }
                case 16:
                    {

                        if (client.Inventory.Contain(3004460, 1))
                        {
                            client.Inventory.Remove(3004460, 1, stream);
                            client.Inventory.AddItemWitchStack(3004465, 0, 20, stream);

                            client.CreateBoxDialog("97221785_0105@@");
                        }
                        else
                        {
                            client.CreateBoxDialog("97221741_0105@@");
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.YinYangStele)]
        public static void YinYangStele(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.EpicNinjaQuestChance == 0)
                        {
                            if (client.Inventory.Contain(3004462, 1))
                            {
                                data.Text("97221504_0101@@");
                                data.Text("97221505_0101@@");
                                data.Option("97221394_0102@@", 20);
                                data.Option("97221395_0102@@", 21);
                                data.Option("Leave.", 255);
                                data.AddAvatar(0).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("97221290_0101@@")
                              .AddText("97221291_0101@@")
                      .AddOption("97221292_0102@@.", 1)
                      .AddOption("97221293_0102@@.", 2)
                      .AddOption("Leave.", 255).FinalizeDialog();
                            }

                        }
                        else
                        {
                            if (client.Inventory.Contain(3004462, 1))
                            {
                                data.Text("97221504_0101@@");
                                data.Text("97221505_0101@@");
                                data.Option("97221394_0102@@", 20);
                                data.Option("97221395_0102@@", 21);
                                data.Option("Leave.", 255);
                                data.AddAvatar(0).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("97221380_0101@@")
                                .AddText("97221381_0101@@")
                                    .AddOption("97221385_0102@@", 10)
                                    .AddOption("97221386_0102@@", 12)
                                    .AddOption("I see.", 255).FinalizeDialog();
                            }

                        }


                        break;
                    }
                case 20:
                    {
                        client.Teleport(32, 35, 3850, 0);
                        break;
                    }
                case 21:
                    {
                        data.AddText("97221380_0101@@")
                                .AddText("97221381_0101@@")
                                    .AddOption("97221385_0102@@", 10)
                                    .AddOption("97221386_0102@@", 12)
                                    .AddOption("I see.", 255).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("97221380_0101@@")
                                   .AddText("97221381_0101@@")
                        .AddOption("97221385_0102@@", 10)
                        .AddOption("97221386_0102@@", 12)
                        .AddOption("I see.", 255).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(722732, 4) && client.Inventory.Contain(722736, 4))
                        {
                            client.Inventory.Remove(722732, 4, stream);
                            client.Inventory.Remove(722736, 4, stream);
                            client.Player.EpicNinjaQuestChance = 1;
                            if (Role.Core.Rate(10))
                            {
                                client.Inventory.AddItemWitchStack(3004462, 0, 1, stream);
                            }
                            else
                            {
                                data.AddText("97221380_0101@@")
                                   .AddText("97221381_0101@@")
                                    .AddOption("97221384_0102@@.", 15)
                                    .AddOption("Leave.", 255).FinalizeDialog();
                                if (client.Player.MyJiangHu != null)
                                {
                                    if (client.Player.MyJiangHu.FreeCourse <= 900000)
                                    {
                                        client.Player.MyJiangHu.FreeCourse += 40000;
                                    }
                                }
                                if (client.Player.SubClass != null)
                                {
                                    client.Player.SubClass.AddStudyPoints(client, 100, stream);
                                }
                            }
                            client.CreateBoxDialog("97221320_0105@@");
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "angelwing");


                        }
                        else
                        {
                            client.CreateBoxDialog("97221302_0101@@");
                        }

                        break;
                    }
                case 15:
                    {
                        client.Inventory.AddItemWitchStack(3002030, 0, 3, stream);
                        data.AddText("You`ve felt it for " + client.Player.EpicNinjaQuestChance + " times today, but failed to feel the seal. Don`t lose heart, you received 3 Super Protection Pills.\n")
                                     .AddText("97222301_0101@@")
                                 .AddOption("97221385_0102@@", 10)
                     .AddOption("97221386_0102@@ ", 12)
                                      .AddOption("Leave.", 255).FinalizeDialog();
                        break;
                    }
                case 12:
                    {
                        data.AddText("97221800_0101@@ ")
                    .AddOption("Yes.", 13)
                    .AddOption("No", 255).FinalizeDialog();
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
                               0,0, 3004466, 3004467, 0, 3004462,0 , 0, 0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0,0,0,0,0 , 0, 0, 0, 0 ,3004467,3004466,3004466,0,0,0,3004467,3004467
                       };
                                    int index = (int)Pool.GetRandom.Next(0, array_Items.Length);
                                    uint ItemId = array_Items[index];
                                    if (ItemId != 0)
                                    {
                                        var item = Pool.ItemsBase[ItemId];
                                        client.Inventory.AddItemWitchStack((uint)ItemId, 0, 1, stream);
                                        data.AddText("You`ve felt it for" + client.Player.EpicNinjaQuestChance + " times today, Received " + item.Name + ". \n");
                                    }
                                    else
                                        data.AddText("You`ve felt it for" + client.Player.EpicNinjaQuestChance + " times today, Received nothing. \n");
                                    client.Player.EpicNinjaQuestChance += 1;
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "angelwing");
                                }
                                data.AddOption("97221914_0102@@", 12);
                                if (client.Inventory.Contain(3004462, 1))
                                    data.AddOption("97221917_0102@@", 16);
                                data.AddOption("Leave.", 255)
                                    .FinalizeDialog();
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
                case 16:
                    {
                        client.Teleport(32, 35, 3850, 0);
                        break;
                    }
                case 10:
                    {
                        data.AddText("97221800_0101@@ ")
                        .AddOption("Yes.", 11)
                        .AddOption("No", 255).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        if (client.Inventory.HaveSpace(4))
                        {
                            if (client.Player.ConquerPoints >= 27)
                            {
                                if (client.InTrade) return;
                                client.Player.ConquerPoints -= 27;

                                uint[] array_Items = new uint[]
                       {
                               0,0, 3004466, 3004467, 0, 3004462,0 , 0, 0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0,0,0,0,0 , 0, 0, 0, 0 ,3004467,3004466,3004466,0,0,0,3004467,3004467
                       };
                                int index = (int)Pool.GetRandom.Next(0, array_Items.Length);
                                uint ItemId = array_Items[index];
                                if (ItemId != 0)
                                {
                                    var item = Pool.ItemsBase[ItemId];
                                    client.Inventory.AddItemWitchStack((uint)ItemId, 0, 1, stream);
                                    data.AddText("You`ve felt it for" + client.Player.EpicNinjaQuestChance + " times today, Received " + item.Name + ". \n");
                                }
                                else
                                    data.AddText("You`ve felt it for" + client.Player.EpicNinjaQuestChance + " times today, Received nothing. \n");
                                client.Player.EpicNinjaQuestChance += 1;
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "angelwing");
                                if (client.Inventory.Contain(3004462, 1))
                                    data.AddOption("97221917_0102@@", 16);
                                data.AddOption("Leave.", 255)
                                    .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Please come back when you will have 27 ConquerPoints")
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
            }
        }
        [NpcAttribute(NpcID.ElementalGuard)]
        public static void ElementalGuard(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Inventory.Contain(3004463, 1))
                        {
                            data.Text("97221504_0101@@");
                            data.Text("97221505_0101@@");
                            data.Option("97221506_0102@@", 2);
                            data.Option("97221507_0102@@", 3);
                            data.Option("Leave.", 255);
                            data.AddAvatar(0).FinalizeDialog();
                        }
                        else
                        {
                            data.Text("You`ve tried for " + client.Player.EpicNinjaTwoQuestChance + " times today, but failed to awaken the Elemental Guard. Don`t lose heart, you received a Stardust.\n");
                            data.Text("97221530_0101@@");
                            data.Option("97221532_0102@@", 1);
                            data.Option("Leave.", 255);
                            data.AddAvatar(0).FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(3004462, 1))
                        {
                            client.Inventory.Remove(3004462, 1, stream);
                            client.Player.EpicNinjaTwoQuestChance += 1;

                            if (Role.Core.Rate(10))
                            {
                                client.Inventory.AddItemWitchStack(3004463, 0, 1, stream);
                                client.Player.MessageBox("97221523_0105@@", new Action<Client.GameClient>(p => { p.Teleport(32, 35, 3851, 0); }), null, 60);
                            }
                            else
                            {
                                client.Inventory.AddItemWitchStack(3004465, 0, 1, stream);
                            }
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "angelwing");
                            if (!client.Inventory.Contain(3004462, 1) && !client.Inventory.Contain(3004463, 1))
                            {
                                client.Teleport(32, 35, 3849, 0);
                            }
                            else
                            {
                                data.Text("You`ve tried for " + client.Player.EpicNinjaTwoQuestChance + " times today, but failed to awaken the Elemental Guard. Don`t lose heart, you received a Stardust.\n");
                                data.Text("97221530_0101@@");
                                data.Option("97221532_0102@@", 1);
                                data.AddAvatar(0).FinalizeDialog();
                            }

                        }
                        else
                        {
                            client.Player.MessageBox("97221512_0102@@", new Action<Client.GameClient>(p => { p.Teleport(32, 35, 3849, 0); }), null, 60);
                        }

                        break;
                    }
                case 2:
                    {
                        client.Teleport(32, 35, 3851, 0);
                        break;
                    }
                case 3:
                    {
                        data.Text("97221530_0101@@");
                        data.Text("97221531_0101@@");
                        data.Option("97221532_0102@@", 1);
                        data.Option("Leave.", 255);
                        data.AddAvatar(0).FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.WheelOfLife)]
        public static void WheelOfLife(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Inventory.GetCountItem(3004463) == 0)
                        {
                            data.Text("97221543_0101@@");
                            data.Text("97221544_0101@@");
                            data.Option("97221553_0102@@", 2);
                            data.Option("Leave.", 255);
                            data.AddAvatar(0).FinalizeDialog();
                        }
                        else
                        {
                            data.Text("97221543_0101@@");
                            data.Text("97221544_0101@@");
                            data.Option("97221545_0102@@", 1);
                            data.Option("Leave.", 255);
                            data.AddAvatar(0).FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(3004463, 1))
                        {
                            client.Inventory.Remove(3004463, 1, stream);
                            client.Player.EpicNinjaThreeQuestChance += 1;

                            if (Role.Core.Rate(10))
                            {
                                client.Inventory.AddItemWitchStack(3004464, 0, 1, stream);
                                data.Text("97221565_0101@@");
                                data.Text("97221566_0101@@");
                                data.Option("Leave.", 255);
                                data.AddAvatar(0).FinalizeDialog();
                            }
                            else
                            {
                                client.Inventory.AddItemWitchStack(3004465, 0, 5, stream);
                                if (client.Inventory.GetCountItem(3004463) == 0)
                                {
                                    data.Text("You`ve tried for " + client.Player.EpicNinjaThreeQuestChance + " times today, but failed to get rid of the wheel. Don`t lose heart, you received 5 Stardust.\n");
                                    data.Text("97221577_0101@@");
                                    data.Text(" 97221552_0101@@");
                                    data.Option("97221553_0102@@", 2);
                                    data.Option("Leave.", 255);
                                    data.AddAvatar(0).FinalizeDialog();
                                }
                                else
                                {
                                    data.Text("You`ve tried for " + client.Player.EpicNinjaThreeQuestChance + " times today, but failed to get rid of the wheel. Don`t lose heart, you received 5 Stardust.\n");
                                    data.Text("97221577_0101@@");
                                    data.Option("97221545_0102@@", 1);
                                    data.Option("Leave.", 255);
                                    data.AddAvatar(0).FinalizeDialog();
                                }

                            }

                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "angelwing");


                        }
                        else
                        {
                            data.Text("97221565_0101@@");
                            data.Text("97221566_0101@@");
                            data.Option("97221553_0102@@", 2);
                            data.AddAvatar(0).FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        client.Teleport(32, 35, 3850, 0);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.EpicNinja)]
        public static void EpicNinja(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain((uint)NpcID.EpicNinja, 1) && !client.Inventory.Contain(3004464, 1))
            {
                client.SendSysMesage("You Don't have this item in Inventory");
                return;
            }
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.Text("When your right hand is equipped with an epic weapon, the Katana on your hand can be converted into an epic weapon\n");
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
                            if (Database.ItemType.IsKatana(GameItem.ITEM_ID))
                            {
                                uint UpdateToEpic = (GameItem.ITEM_ID % 1000) + 616000;
                                GameItem.ITEM_ID = UpdateToEpic;
                                GameItem.Mode = Role.Flags.ItemMode.Update;
                                GameItem.Send(client, stream);
                                client.Inventory.Remove(3305415, 1, stream);
                                client.Equipment.QueryEquipment(client.Equipment.Alternante, true);


                                Server.SendGlobalPacket(new MsgMessage("Congratulation! " + client.Player.Name + " has finished the Ninja Epic Quest.", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));


                                client.Inventory.Update(GameItem, Role.Instance.AddMode.REMOVE, stream);
                            }


                        }
                        else
                        {
                            client.SendSysMesage("Soryy, You're not equipped with the Katana(right weapon).");

                        }
                        break;
                    }
                case 4:
                    {
                        MsgGameItem GameItem;
                        if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.LeftWeapon, out GameItem))
                        {
                            if (Database.ItemType.IsKatana(GameItem.ITEM_ID))
                            {
                                uint UpdateToEpic = (GameItem.ITEM_ID % 1000) + 616000;
                                GameItem.ITEM_ID = UpdateToEpic;
                                GameItem.Mode = Role.Flags.ItemMode.Update;
                                GameItem.Send(client, stream);
                                client.Inventory.Remove(3305415, 1, stream);
                                client.Equipment.QueryEquipment(client.Equipment.Alternante, true);


                                Server.SendGlobalPacket(new MsgMessage("Congratulation! " + client.Player.Name + " has finished the Ninja Epic Quest.", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));


                                client.Inventory.Update(GameItem, Role.Instance.AddMode.REMOVE, stream);
                            }


                        }
                        else
                        {
                            client.SendSysMesage("Soryy, You're not equipped with the Katana(right weapon).");

                        }
                        break;
                    }

            }

        }
        #endregion
        #region EpicPirateQuest
        //[NpcAttribute(NpcID.Mermaid)]
        //public static void Mermaid(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog data = new Dialog(client, stream);
        //    switch (Option)
        //    {
        //        case 0:
        //            {

        //                data.Text("STR_ID_tPirateEpicMission[21426][Text111]@@\n");
        //                data.Text("STR_ID_tPirateEpicMission[21426][Text112]@@\n");
        //                data.Text("STR_ID_tPirateEpicMission[21426][Text113]@@\n");
        //                data.Option("STR_ID_tPirateEpicMission[21426][Option111]@@", 1);
        //                data.AddAvatar(0).FinalizeDialog();
        //                break;
        //            }
        //        case 1:
        //            {
        //                client.MyExchangeShop.Open(stream, 3307450, 70);
        //                break;
        //            }
        //    }
        //}
        [NpcAttribute(NpcID.EpicPirate)]
        public static void EpicPirate(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain(3307449, 1))
            {
                client.SendSysMesage("Check your inventory!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                return; // ( Done )
            }

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (!Database.AtributesStatus.IsPirate(client.Player.Class))
                        {
                            data.Text("Sorry, You`re not a Pirate and you can`t use (HeartoftheSea) to transform the Rapier in your right hand or Pistol In the left hand into a Pirate`s Epic Weapon.");
                            data.Option("Alright.", 255);
                            data.AddAvatar(1365).FinalizeDialog();
                            return;
                        }
                        data.Text("Are you sure you want to activate the (HeartoftheSea) and transform the Rapier in your right hand or Pistol In the left hand into a Pirate`s Epic Weapon.");
                        data.Option("STR_ID_tEpicWeaponsInNewServer[3321029][Option111]@@", 1);
                        data.Option("Leave.", byte.MaxValue);
                        data.AddAvatar(1365).FinalizeDialog();
                        break;

                    }
                case 1:
                    {
                        data.Text("Choose any legendary weapon you want and transform the Rapier in your right hand or Pistol In the left hand into a Pirate`s Epic Weapon.");
                        data.Option("Main Weapon.", 3);
                        data.Option("Alternate Weapon.", 4);
                        data.AddAvatar(1365).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        MsgGameItem GameItem;
                        if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.RightWeapon, out GameItem))
                        {

                            if (Database.ItemType.IsRapier(GameItem.ITEM_ID))
                            {
                                uint UpdateToEpic = (GameItem.ITEM_ID % 1000) + 671000;
                                GameItem.ITEM_ID = UpdateToEpic;
                                GameItem.Mode = Role.Flags.ItemMode.Update;
                                GameItem.Send(client, stream);
                                client.Inventory.Remove(3307449, 1, stream);
                                client.Equipment.QueryEquipment(client.Equipment.Alternante, true);


                                data.AddText("Congrats! Your Rapier Weapon has been Successfully into an epic Weapon.");
                                data.AddOption("Awesome!", 255);
                                data.AddAvatar(1365);//done
                                data.FinalizeDialog();
                                client.Inventory.Update(GameItem, Role.Instance.AddMode.REMOVE, stream);
                            }
                            else
                            {
                                data.AddText("Sorry, You`re not equipped with the Rapier`s (Main Weapon).");
                                data.Option("Alright.", 255);
                                data.AddAvatar(1365);//done
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Sorry, You`re not equipped with the Rapier`s (Main Weapon).");
                            data.Option("Alright.", 255);
                            data.AddAvatar(1365);//done
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 4:
                    {
                        MsgGameItem GameItem;
                        if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.LeftWeapon, out GameItem))
                        {

                            if (Database.ItemType.IsPistol(GameItem.ITEM_ID))
                            {
                                uint UpdateToEpic = (GameItem.ITEM_ID % 1000) + 670000;
                                GameItem.ITEM_ID = UpdateToEpic;
                                GameItem.Mode = Role.Flags.ItemMode.Update;
                                GameItem.Send(client, stream);
                                client.Inventory.Remove(3307449, 1, stream);
                                client.Equipment.QueryEquipment(client.Equipment.Alternante, true);


                                data.AddText("Congrats! Your Pistol Weapon has been Successfully into an epic Weapon.");
                                data.AddOption("Awesome!", 255);
                                data.AddAvatar(1365);//done
                                data.FinalizeDialog();


                                client.Inventory.Update(GameItem, Role.Instance.AddMode.REMOVE, stream);
                            }
                            else
                            {
                                data.AddText("Sorry, You`re not equipped with the Pistol`s (Left weapon).");
                                data.Option("Alright.", 255);
                                data.AddAvatar(1365);//done
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Sorry, You`re not equipped with the Pistol`s (Left weapon).");
                            data.Option("Alright.", 255);
                            data.AddAvatar(1365);//done
                            data.FinalizeDialog();
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Luffy)]
        public static void Luffy(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.Text("<tip desc=He#is#the#famous#captain#of#the#Straw#Hat#Pirates,#and#also#a#brilliant#treasure#hunter.#What#really#attracts#him#is#the#excitement#during");
                        data.Text("#the#adventure,#but#not#the#value#of#the#treasures.>Luffy</tip>:\nI`m quite experienced with treasure hunting. Just feel free to ask me for help");
                        data.Text("~if you have a treasure map. No worry, I have no interest in the treasures.");
                        data.Option("I`ll~go~find~a~map.", 1);
                        data.Avatar(237);
                        data.FinalizeDialog();
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.JackSparrow)]
        public static void JackSparrow(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.Text("STR_ID_tPirateEpicTask[21427][Text111]@@");
                        data.Text("STR_ID_tPirateEpicTask[21427][Text112]@@");
                        data.Text("STR_ID_tPirateEpicTask[21427][Text113]@@");
                        data.Text("STR_ID_tPirateEpicTask[21427][Text114]@@");
                        data.Text("1. Submit 3 White Flowers and 3 Red Flowers to claim a Drift Bottle which contains a Treasure Map of Star 1-3.\n");
                        data.Text("2. 1-Star Treasure Map gives you a 30% chance of getting a Treasure Map of Star 1-4.\n");
                        data.Text("    2-Star Treasure Map gives you a 25% chance of getting a Treasure Map of Star 2-5\n");
                        data.Text("Ã¯Â¿Â½Ã¯Â¿Â½3-Star Treasure Map gives you a 20% chance of getting a Treasure Map of Star 3-5\n");
                        data.Text("Ã¯Â¿Â½Ã¯Â¿Â½4-Star Treasure Map gives you a 20% chance of getting a Treasure Map of Star 4-5\n");
                        data.Text("3. 5-Star Treasure Map directly shows you the location of the lost Heart of the Sea. This powerful gem\n");
                        data.Text("Ã¯Â¿Â½Ã¯Â¿Â½can transform your equipped Pistol or Rapier into an Epic Weapon.\n");
                        data.Text("Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½\n");
                        if (client.Player.EpicPirateQuestChance == 0)
                        {
                            data.Option("Submit~White/Red~Flowers.", 1);
                            data.Option("Pay~27~CPs~instead~of~flower.", 2);
                        }
                        else
                        {
                            data.Option("Buy~1-Star~Treasure~Map.", 3);
                        }

                        data.Avatar(236);
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(722732, 3) && client.Inventory.Contain(722736, 3))
                        {
                            client.Inventory.Remove(722732, 3, stream);
                            client.Inventory.Remove(722736, 3, stream);
                            client.Player.EpicPirateQuestChance = 1;
                            client.Inventory.Add(stream, 3307448);
                            client.SendSysMesage("STR_ID_tPirateEpicTask[21427][ExchangeBottle][UseEMoneySucc]@@", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            client.Send(stream.CreateDialogItem(8, 272, 52504, 49610, 987, "STR_ID_tPirateEpicTask[21427][ExchangeBottle][ImmediaUse]@@"));
                        }
                        else
                        {
                            data.Text("White and Red Flowers, I`ve heard about them for a long time. If you bring me 3 White Flowers and 3 Red Flowers, I`ll give you a Drift Bottle that contains a clue about the Heart of the Sea.");
                            data.Option("I`ll~go~collect~some.", 255);
                            data.Avatar(236);
                            data.FinalizeDialog();
                            break;
                        }
                        break;
                    }
                case 2:
                    {
                        data.Text("Are you sure you want to pay 27 CPs instead of submitting the White and Red Flowers?");
                        data.Option("Yes,~here`re~27~CPs.", 10);
                        data.Option("I~haven`t~decided~yet.", 255);
                        data.Avatar(236);
                        data.FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        if (client.Player.ConquerPoints >= 27)
                        {
                            client.Player.ConquerPoints -= 27;
                            client.Player.EpicPirateQuestChance = 1;
                            client.Inventory.Add(stream, 3307448);
                            client.SendSysMesage("STR_ID_tPirateEpicTask[21427][ExchangeBottle][UseEMoneySucc]@@", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                        }
                        else
                        {
                            data.Text("It seems you don`t have 27 CPs. Don`t try to cheat me!");

                            data.Option("Alright.", 1);
                            data.Avatar(236);
                            data.FinalizeDialog();
                        }
                        break;

                    }
                case 3:
                    {
                        data.Text("STR_ID_tPirateEpicTask[21427][Text311]@@");
                        data.Text("STR_ID_tPirateEpicTask[21427][Text312]@@");
                        data.Option("STR_ID_tPirateEpicTask[21427][Option311]@@", 4);//BuyOne
                        data.Option("STR_ID_tPirateEpicTask[21427][Option312]@@", 15);//BuyTen
                        data.Avatar(236);
                        data.FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        data.Text("STR_ID_tPirateEpicTask[21427][BuyOneMap][SecondConfirm]@@");
                        data.Option("STR_ID_tPirateEpicTask[21427][BuyOneMap][Confirm]@@", 5);//BuyOne
                        data.Option("STR_ID_tPirateEpicTask[21427][BuyOneMap][RemoveSecConf]@@", 6);//DontShow
                        data.Option("STR_ID_tPirateEpicTask[21427][BuyOneMap][NoConfirm]@@", 255);//NoConfirm
                        data.Avatar(236);
                        data.FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        if (client.Player.ConquerPoints >= 27)
                        {
                            client.Player.ConquerPoints -= 27;
                            client.SendSysMesage("STR_ID_tPirateEpicTask[21427][BuyOneMap][GoHuntingTreasure]@@", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            client.Inventory.AddItemWitchStack(3307443, 0, 1, stream);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                            goto case 4;

                        }
                        else
                        {
                            client.CreateBoxDialog("STR_ID_tPirateEpicTask[21427][BuyOneMap][NoEMoney]@@");
                        }
                        break;
                    }
                case 6:
                    {
                        data.Text("STR_ID_tPirateEpicTask[21427][BuyOneMap][SecondConfirm]@@");
                        data.Option("STR_ID_tPirateEpicTask[21427][BuyOneMap][GoNow]@@", 7);//Go~find~treasures~now
                        data.Option("STR_ID_tPirateEpicTask[21427][BuyOneMap][BuyAgain]@@", 8);//Buy~more~maps
                        data.Avatar(236);
                        data.FinalizeDialog();
                        break;
                    }
                case 8:
                    {
                        if (client.Player.ConquerPoints >= 27)
                        {
                            client.Player.ConquerPoints -= 27;
                            client.SendSysMesage("STR_ID_tPirateEpicTask[21427][BuyOneMap][GoHuntingTreasure]@@", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            client.Inventory.AddItemWitchStack(3307443, 0, 1, stream);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                            goto case 6;

                        }
                        else
                        {
                            client.CreateBoxDialog("STR_ID_tPirateEpicTask[21427][BuyOneMap][NoEMoney]@@");
                        }
                        break;
                    }
                case 15:
                    {
                        data.Text("STR_ID_tPirateEpicTask[21427][BuyTenMaps][SecondConfirm]@@");
                        data.Option("STR_ID_tPirateEpicTask[21427][BuyTenMaps][Confirm]@@", 16);//Ten
                        data.Option("STR_ID_tPirateEpicTask[21427][BuyTenMaps][RemoveSecConf]@@", 17);//DontShow
                        data.Option("STR_ID_tPirateEpicTask[21427][BuyTenMaps][NoConfirm]@@", 255);//NoConfirm
                        data.Avatar(236);
                        data.FinalizeDialog();
                        break;
                    }
                case 16:
                    {
                        if (client.Player.ConquerPoints >= 270)
                        {
                            client.Player.ConquerPoints -= 270;
                            client.SendSysMesage("STR_ID_tPirateEpicTask[21427][BuyTenMaps][GoHuntingTreasure]@@", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            client.Inventory.AddItemWitchStack(3307443, 0, 10, stream);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                            goto case 15;

                        }
                        else
                        {
                            client.CreateBoxDialog("STR_ID_tPirateEpicTask[21427][BuyTenMaps][NoEMoney]@@");
                        }
                        break;
                    }
                case 17:
                    {
                        data.Text("STR_ID_tPirateEpicTask[21427][BuyTenMaps][SecondConfirm]@@");
                        data.Option("STR_ID_tPirateEpicTask[21427][BuyTenMaps][GoNow]@@", 18);//Go~find~treasures~now
                        data.Option("STR_ID_tPirateEpicTask[21427][BuyTenMaps][BuyAgain]@@", 19);//Buy~more~maps
                        data.Avatar(236);
                        data.FinalizeDialog();
                        break;
                    }
                case 19:
                    {
                        if (client.Player.ConquerPoints >= 270)
                        {
                            client.Player.ConquerPoints -= 270;
                            client.SendSysMesage("STR_ID_tPirateEpicTask[21427][BuyTenMaps][GoHuntingTreasure]@@", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            client.Inventory.AddItemWitchStack(3307443, 0, 10, stream);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                            goto case 17;

                        }
                        else
                        {
                            client.CreateBoxDialog("STR_ID_tPirateEpicTask[21427][BuyTenMaps][NoEMoney]@@");
                        }
                        break;
                    }
                case 7:
                case 18:
                    {
                        var Map = Pool.ServerMaps[10266];
                        ushort x = 0;
                        ushort y = 0;

                        Map.GetRandCoord(ref x, ref y);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "moveback" });
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "hdsshhyx" });
                        client.Teleport(x, y, 10266);
                        client.Player.EpicPirateQuest(stream, "STR_ID_tPirateEpicTask[SetExploreContent]@@", 2, 1, 1);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.OneStarTreasureMap)]
        public static void OneStarTreasureMap(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.Text("STR_ID_tPirateEpicTask[3307443][Text111]@@");
                        data.Option("STR_ID_tPirateEpicTask[3307443][Option111]@@", 1);
                        data.Option("STR_ID_tPirateEpicTask[3307443][Option112]@@", 2);
                        data.Option("STR_ID_tPirateEpicTask[3307443][Option113]@@", 3);
                        data.Avatar(236);
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        var Map = Pool.ServerMaps[10266];
                        ushort x = 0;
                        ushort y = 0;

                        Map.GetRandCoord(ref x, ref y);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "moveback" });
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "hdsshhyx" });
                        client.Teleport(x, y, 10266);
                        client.Player.EpicPirateQuest(stream, "STR_ID_tPirateEpicTask[SetExploreContent]@@", 2, 1, 1);
                        break;
                    }
                case 2:
                    {
                        var Map = Pool.ServerMaps[10266];
                        ushort x = 0;
                        ushort y = 0;

                        Map.GetRandCoord(ref x, ref y);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "moveback" });
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "hdsshhyx" });
                        client.Teleport(x, y, 10266);
                        client.Player.EpicPirateQuest(stream, "STR_ID_tPirateEpicTask[SetExploreContent]@@", 2, 1, 10);
                        break;
                    }
                case 3:
                    {
                        client.Teleport(120, 187, 10271, 0);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.TwoStarTreasureMap)]
        public static void TwoStarTreasureMap(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.Text("STR_ID_tPirateEpicTask[3307444][Text111]@@");
                        data.Option("STR_ID_tPirateEpicTask[3307444][Option111]@@", 1);
                        data.Option("STR_ID_tPirateEpicTask[3307444][Option112]@@", 2);
                        data.Option("STR_ID_tPirateEpicTask[3307444][Option113]@@", 3);
                        data.Avatar(236);
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        var Map = Pool.ServerMaps[10267];
                        ushort x = 0;
                        ushort y = 0;

                        Map.GetRandCoord(ref x, ref y);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "moveback" });
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "hdsslhex" });
                        client.Teleport(x, y, 10267);
                        client.Player.EpicPirateQuest(stream, "STR_ID_tPirateEpicTask[SetExploreContent]@@", 2, 2, 1);
                        break;
                    }
                case 2:
                    {
                        var Map = Pool.ServerMaps[10267];
                        ushort x = 0;
                        ushort y = 0;

                        Map.GetRandCoord(ref x, ref y);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "moveback" });
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "hdsslhex" });
                        client.Teleport(x, y, 10267);
                        client.Player.EpicPirateQuest(stream, "STR_ID_tPirateEpicTask[SetExploreContent]@@", 2, 2, 5);
                        break;
                    }
                case 3:
                    {
                        client.Teleport(120, 187, 10271, 0);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ThreeStarTreasureMap)]
        public static void ThreeStarTreasureMap(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.Text("STR_ID_tPirateEpicTask[3307445][Text111]@@");
                        data.Option("STR_ID_tPirateEpicTask[3307445][Option111]@@", 1);
                        data.Option("STR_ID_tPirateEpicTask[3307445][Option112]@@", 2);
                        data.Option("STR_ID_tPirateEpicTask[3307445][Option113]@@", 3);
                        data.Avatar(236);
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        var Map = Pool.ServerMaps[10268];
                        ushort x = 0;
                        ushort y = 0;

                        Map.GetRandCoord(ref x, ref y);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "moveback" });
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "hdssaqhsx" });
                        client.Teleport(x, y, 10268);
                        client.Player.EpicPirateQuest(stream, "STR_ID_tPirateEpicTask[SetExploreContent]@@", 2, 3, 1);
                        break;
                    }
                case 2:
                    {
                        var Map = Pool.ServerMaps[10268];
                        ushort x = 0;
                        ushort y = 0;

                        Map.GetRandCoord(ref x, ref y);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "moveback" });
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "hdssaqhsx" });
                        client.Teleport(x, y, 10268);
                        client.Player.EpicPirateQuest(stream, "STR_ID_tPirateEpicTask[SetExploreContent]@@", 2, 3, 5);
                        break;
                    }
                case 3:
                    {
                        client.Teleport(120, 187, 10271, 0);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.FourStarTreasureMap)]
        public static void FourStarTreasureMap(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.Text("STR_ID_tPirateEpicTask[3307446][Text111]@@");
                        data.Option("STR_ID_tPirateEpicTask[3307446][Option111]@@", 1);
                        data.Option("STR_ID_tPirateEpicTask[3307446][Option112]@@", 2);
                        data.Option("STR_ID_tPirateEpicTask[3307446][Option113]@@", 3);
                        data.Avatar(236);
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        var Map = Pool.ServerMaps[10269];
                        ushort x = 0;
                        ushort y = 0;

                        Map.GetRandCoord(ref x, ref y);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "moveback" });
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "hdssdzhsx" });
                        client.Teleport(x, y, 10269);
                        client.Player.EpicPirateQuest(stream, "STR_ID_tPirateEpicTask[SetExploreContent]@@", 2, 4, 1);
                        break;
                    }
                case 2:
                    {
                        var Map = Pool.ServerMaps[10269];
                        ushort x = 0;
                        ushort y = 0;

                        Map.GetRandCoord(ref x, ref y);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "moveback" });
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "hdssdzhsx" });
                        client.Teleport(x, y, 10269);
                        client.Player.EpicPirateQuest(stream, "STR_ID_tPirateEpicTask[SetExploreContent]@@", 2, 4, 5);
                        break;
                    }
                case 3:
                    {
                        client.Teleport(120, 187, 10271, 0);
                        break;
                    }
            }
        }

        #endregion
        [NpcAttribute(NpcID.TaoBless)]
        public static void TaoBless(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (!(client.Player.Class >= 13002 && client.Player.Class <= 14058))
                        {
                            data.AddText("The blessing spell is only effective on Taoist`s talisman.")
                                .AddOption("Alright.", 255).AddAvatar(53).FinalizeDialog();
                            return;
                        }
                        data.AddText("Blessmann:\n");
                        data.AddText("    Talisman is thought to have amazing power. It can be more powerful when\n blessed.\n    I can help L120+ heroes attach Blessed attribute to their talismans. Note\n that heroes at different levels may need different materials and get different results.")
                            // data.AddText("   The blessing spell is only effective on Taoist`s talisman.")
                         .AddOption("Bless talisman.", 1)
                         .AddAvatar(53).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        data.AddText("Blessmann:\n");
                        data.AddText("    Talisman is thought to have amazing power. It can be more powerful when\n blessed.\n    I can help L120+ heroes attach Blessed attribute to their talismans. Note\n that heroes at different levels may need different materials and get different results.")
                            // data.AddText("   The blessing spell is only effective on Taoist`s talisman.")
                         .AddOption("Bless talisman.", 1)
                         .AddAvatar(53).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("                      * Talisman Blessing *       		 \n");
                        data.AddText("-------------------------------------------------------------\n")
                                .AddText("       [Requirement]            [Objective]        [Materials]\n")
                                 .AddText("        Hero L120                -0% -> -1%   1 Super Tortoise Gem (20% success)\n")
                                  .AddText("                                                         5 Super Tortoise Gems,\n")
                                  .AddText("                                                        10 million Silver (100% success)\n")
                                 .AddText("\nHero L130 & -1% Talisman  -1% -> -3%   1 Super Tortoise Gem, 1 million\n")
                                  .AddText(" Silver,                                                         1 Metacrystal (10% success)\n")
                                  .AddText("\nHero L140 & -3% Talisman  -3% -> -5%   3 Super Tortoise Gems, 3 million\n")
                                  .AddText(" Silver,                                                          2 Metacrystals (8% success)\n")
                                  .AddText("\nHero L140 & -5% Talisman  -5% -> -7%    5 Super Tortoise Gems, 10 million\n")
                                  .AddText(" Silver,                                                         3 Metacrystals (3% success)\n")
                                .AddText("-------------------------------------------------------------\n")
                        .AddOption("Learn about something else.", 3)
                        .AddAvatar(53).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        data.AddText("                      * Talisman Blessing *       		 \n");
                        data.AddText("-------------------------------------------------------------\n")
                                .AddText("Requirement: Character Level 120\n")
                                 .AddText("Objective: Blessed Attribute -0% -> -1% damage taken\n")
                                  .AddText("Materials Required: 1 Super Tortoise Gem (Normal Bless: 20% success rate)\n")
                                 .AddText("\n 5 Super Tortoise Gems, 10000000 Silver (Perfect Bless: 100% success rate)\n")
                                .AddText("-------------------------------------------------------------\n")
                        .AddOption("Normal bless. (20% success rate)", 5);
                        Game.MsgServer.MsgGameItem item;
                        if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.LeftWeapon, out item))
                        {


                            if (item.Bless == 0)
                            {
                                data.AddOption("Perfect bless.", 4);
                            }
                            else
                            {
                                data.AddOption("Swap Gem for 3 MetaCrystal.", 7);
                                data.AddOption("Swap Gem(B) for 3 MetaCrystal(B).", 6);
                            }

                        }
                        data.AddOption("Tell me more.", 2)
                      .AddAvatar(53).FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.Contain(700073, 1, 1))
                        {
                            client.Inventory.Remove(700073, 1, stream);
                            client.Inventory.AddItemWitchStack(3316505, 0, 3, stream, true);
                            client.SendSysMesage("You received 3 Metacrystal (B). ", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);

                        }
                        else { client.Player.MessageBox("Please make sure you have 1 Super Tortoise Gem(s) (B) first.", null, null); }
                        break;
                    }
                case 7:
                    {
                        if (client.Inventory.Contain(700073, 1, 0))
                        {
                            client.Inventory.Remove(700073, 1, stream);
                            //client.Inventory.Add(stream, 3316505, 3, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false);
                            client.Inventory.AddItemWitchStack(3316505, 0, 3, stream);
                            client.SendSysMesage("You received 3 Metacrystal. ", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                        }
                        else { client.Player.MessageBox("Please make sure you have 1 Super Tortoise Gem(s) first.", null, null); }
                        break;
                    }
                case 5:
                    {

                        Game.MsgServer.MsgGameItem item;
                        if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.LeftWeapon, out item))
                        {
                            if (Database.ItemType.IsHossu(item.ITEM_ID))
                            {
                                #region 0 t 1
                                if (item.Bless == 0)
                                {
                                    if (client.Inventory.Contain(700073, 1, 0))
                                    {
                                        client.Inventory.Remove(700073, 1, stream);
                                        if (Role.Core.Rate(20))
                                        {
                                            using (var recPick = new ServerSockets.RecycledPacket()) client.Player.AddPick(recPick.GetStream(), "Forging.", 2);
                                            item.Bless = 1;
                                            client.SendSysMesage("Congratulations, your Hossu has been successfully merged and now it is Damage 1.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                                            item.Mode = Role.Flags.ItemMode.Update;
                                            item.Send(client, stream);
                                            client.Status.ItemBless += 1;
                                        }
                                        else
                                        {
                                            client.SendSysMesage("Failed.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                                        }
                                    }
                                    else { client.Player.MessageBox("Please make sure you have 1 Super Tortoise Gem(s) first.", null, null); }

                                }
                                #endregion
                                #region 1 t 3
                                else if (item.Bless == 1)
                                {
                                    if (client.Inventory.Contain(700073, 1, 0) && client.Inventory.Contain(3316505, 1) && client.Player.Money >= 1000000)
                                    {
                                        using (var recPick = new ServerSockets.RecycledPacket()) client.Player.AddPick(recPick.GetStream(), "Forging.", 2);

                                        client.Inventory.Remove(700073, 1, stream);
                                        client.Inventory.Remove(3316505, 1, stream);
                                        client.Player.Money -= 1000000;
                                        if (Role.Core.Rate(10))
                                        {
                                            item.Bless = 3;
                                            client.SendSysMesage("Congratulations, your Hossu has been successfully merged and now it is Damage 3.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                                            item.Mode = Role.Flags.ItemMode.Update;
                                            item.Send(client, stream);
                                            client.Status.ItemBless += 2;
                                        }
                                        else
                                        {
                                            client.SendSysMesage("Failed.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                                        }
                                    }
                                    //  else { client.Player.MessageBox("Please make sure you have 1 Metacrystal and 1 Super Tortoise Gem(s) and 1000000 Silver first.", null, null); }
                                    //client.SendSysMesage("Please make sure you have 1 Metacrystal and 1 Super Tortoise Gem(s) and 1000000 Silver first.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);

                                }
                                #endregion
                                #region 3 t 5
                                else if (item.Bless == 3)
                                {
                                    if (client.Inventory.Contain(700073, 3, 0) && client.Inventory.Contain(3316505, 2) && client.Player.Money >= 3000000)
                                    {

                                        using (var recPick = new ServerSockets.RecycledPacket()) client.Player.AddPick(recPick.GetStream(), "Forging.", 2);
                                        client.Inventory.Remove(700073, 3, stream);
                                        client.Inventory.Remove(3316505, 2, stream);
                                        client.Player.Money -= 3000000;
                                        if (Role.Core.Rate(8))
                                        {
                                            item.Bless = 5;
                                            client.SendSysMesage("Congratulations, your Hossu has been successfully merged and now it is Damage 5.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                                            item.Mode = Role.Flags.ItemMode.Update;
                                            item.Send(client, stream);
                                            client.Status.ItemBless += 2;
                                        }
                                        else
                                        {
                                            client.SendSysMesage("Failed.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                                        }
                                    }
                                    // else { client.Player.MessageBox("Please make sure you have 2 Metacrystal and 3 Super Tortoise Gem(s) and 3000000 Silver first.", null, null); }
                                    //client.SendSysMesage("Please make sure you have 2 Metacrystal and 3 Super Tortoise Gem(s) and 3000000 Silver first.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);

                                }
                                #endregion
                                #region 5 t 7
                                else if (item.Bless == 5)
                                {
                                    if (client.Inventory.Contain(700073, 5, 0) && client.Inventory.Contain(3316505, 3) && client.Player.Money >= 10000000)
                                    {

                                        using (var recPick = new ServerSockets.RecycledPacket()) client.Player.AddPick(recPick.GetStream(), "Forging.", 2);
                                        client.Inventory.Remove(700073, 5, stream);
                                        client.Inventory.Remove(3316505, 3, stream);
                                        client.Player.Money -= 10000000;
                                        if (Role.Core.Rate(3))
                                        {
                                            item.Bless = 7;
                                            client.SendSysMesage("Congratulations, your Hossu has been successfully merged and now it is Damage 7.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                                            item.Mode = Role.Flags.ItemMode.Update;
                                            item.Send(client, stream);
                                            client.Status.ItemBless += 2;
                                        }
                                        else
                                        {
                                            client.SendSysMesage("Failed.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                                        }
                                    }
                                    //  else { client.Player.MessageBox("Please make sure you have 3 Metacrystal and 5 Super Tortoise Gem(s) and 10000000 Silver first.", null, null); }
                                    //  client.SendSysMesage("Please make sure you have 3 Metacrystal and 5 Super Tortoise Gem(s) and 10000000 Silver first.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);

                                }
                                #endregion
                                #region 7
                                else if (item.Bless == 7)
                                {
                                    client.Player.MessageBox("Your Hossu already has Damage 7 !", null, null);

                                }
                                #endregion


                            }
                            else client.Player.MessageBox("Sorry, There is no hossu on Left Hand.", null, null);
                        }
                        break;
                    }
                case 4:
                    {
                        Game.MsgServer.MsgGameItem item;
                        if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.LeftWeapon, out item))
                        {
                            if (Database.ItemType.IsHossu(item.ITEM_ID))
                            {
                                #region 0 t 1
                                if (item.Bless == 0)
                                {
                                    if (client.Inventory.Contain(700073, 5, 0) && client.Player.Money >= 10000000)
                                    {
                                        client.Inventory.Remove(700073, 5, stream);
                                        client.Player.Money -= 10000000;
                                        item.Bless = 1;
                                        item.Mode = Role.Flags.ItemMode.Update;
                                        item.Send(client, stream);
                                        client.Status.ItemBless += 1;

                                    }
                                    else { client.Player.MessageBox("Please make sure you have 10000000 Silver , 5 Super Tortoise Gem(s) first.", null, null); }
                                    client.SendSysMesage("Please make sure you have 10000000 Silver , 5 Super Tortoise Gem(s) first.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);

                                }
                                #endregion
                            }
                            else client.Player.MessageBox("Sorry, There is no hossu on Left Hand.", null, null);
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.DelicateFoxEars)]
        public static void DelicateFoxEars(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.Text("STR_ID_tBigCarnival_EMoneyMono[3320764][Text111]@@\n");
                        data.Option("STR_ID_tBigCarnival_EMoneyMono[3320764][Option111]@@", 1);
                        data.Option("STR_ID_tBigCarnival_EMoneyMono[3320764][Option112]@@", 2);
                        data.AddAvatar(0).FinalizeDialog();
                        break;

                    }
                case 1:
                    {
                        client.Inventory.Remove(3320764, 1, stream);

                        client.Inventory.Add(stream, 3005977, 1, 0, 1, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        break;
                    }
                case 2:
                    {
                        client.Inventory.Remove(3320764, 1, stream);

                        client.Inventory.Add(stream, 3005974, 1, 0, 1, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        break;
                    }

            }

        }
        [NpcAttribute(NpcID.LoveHat)]
        public static void LoveHat(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.Text("Hair~Style.~Open~to~choose~one~from~Love~Hat~(Female)~and~Love~Hat~(Male).\n");
                        data.Option("LoveHat(Male)", 1);
                        data.Option("LoveHat(Female)", 2);
                        data.AddAvatar(0).FinalizeDialog();
                        break;

                    }
                case 1:
                    {
                        client.Inventory.Remove(3320765, 1, stream);

                        client.Inventory.Add(stream, 3005978, 1, 0, 1, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        break;
                    }
                case 2:
                    {
                        client.Inventory.Remove(3320765, 1, stream);

                        client.Inventory.Add(stream, 3005975, 1, 0, 1, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        break;
                    }

            }

        }
        [NpcAttribute(NpcID.MysteryFruit)]
        public static void MysteryFruit(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.Text("98149500_0101@@");
                        data.Text("98149501_0101@@");
                        data.Option("98149502_0102@@", 1);
                        data.Option("98149504_0102@@", 2);
                        data.Option("98149505_0102@@", 3);
                        data.Option("98149506_0102@@", 255);
                        data.AddAvatar(0).FinalizeDialog();
                        break;

                    }
                case 1:
                    {
                        int limitPoints2 = client.Player.Atributes;
                        if (limitPoints2 > 900)
                        {
                            client.CreateBoxDialog("Sorry u Got The Limit Of Fruit.");
                            break;
                        }
                        int limitPoints = client.Player.Agility + client.Player.Strength + client.Player.Vitality + client.Player.Spirit + client.Player.Atributes;
                        if (limitPoints > 900)
                        {
                            client.CreateBoxDialog("Sorry u Got The Limit Of Fruit.");
                            break;
                        }

                        if (client.Inventory.Contain(3001045, 1))
                        {
                            client.Inventory.Remove(3001044, 1, stream);
                            client.Inventory.Remove(3001045, 1, stream);
                            client.Player.AddExtraAtributes(stream, 3);
                            client.CreateBoxDialog("Success ! You successfully and received 3 Attribute Point`s.");
                        }
                        else
                        {
                            client.CreateBoxDialog("98149524_0105@@");

                        }


                        break;
                    }
                case 2:
                    {
                        int limitPoints2 = client.Player.Atributes;
                        if (limitPoints2 > 900)
                        {
                            client.CreateBoxDialog("Sorry u Got The Limit Of Fruit.");
                            break;
                        }
                        int limitPoints = client.Player.Agility + client.Player.Strength + client.Player.Vitality + client.Player.Spirit + client.Player.Atributes;
                        if (limitPoints > 900)
                        {
                            client.CreateBoxDialog("Sorry u Got The Limit Of Fruit.");
                            break;
                        }
                        if (client.Player.ConquerPoints >= 270)
                        {
                            client.Inventory.Remove(3001044, 1, stream);
                            client.Player.ConquerPoints -= 270;
                            client.Player.AddExtraAtributes(stream, 3);
                            client.CreateBoxDialog("Success ! You successfully and received 3 Attribute Point`s.");
                        }
                        else
                        {
                            client.CreateBoxDialog("98149521_0105@@");

                        }



                        break;
                    }
                case 3:
                    {
                        int limitPoints2 = client.Player.Atributes;
                        if (limitPoints2 > 900)
                        {
                            client.CreateBoxDialog("Sorry u Got The Limit Of Fruit.");
                            break;
                        }
                        int limitPoints = client.Player.Agility + client.Player.Strength + client.Player.Vitality + client.Player.Spirit + client.Player.Atributes;
                        if (limitPoints > 900)
                        {
                            client.CreateBoxDialog("Sorry u Got The Limit Of Fruit.");
                            break;
                        }

                        client.Inventory.Remove(3001044, 1, stream);
                        System.Random FoxConquer = new System.Random();
                        byte Points = (byte)FoxConquer.Next(1, 3);
                        client.Player.AddExtraAtributes(stream, Points);
                        client.CreateBoxDialog("Success ! You successfully and received " + Points.ToString() + " Attribute Point`s.");


                        break;
                    }


            }

        }

        [NpcAttribute(NpcID.TomatoUmbrella)]
        public static void TomatoUmbrella(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Wich One Do You Want Your Accessory .");
                        data.AddOption("1-handed~Tomato~Umbrella~(B)", 1);
                        data.AddOption("2-handed~Tomato~Umbrella~(B)", 2);
                        data.AddAvatar(63).FinalizeDialog();
                        break;

                    }
                case 1:
                    {
                        client.Inventory.Remove(3318834, 1, stream);

                        client.Inventory.Add(stream, 360216, 1, 0, 1, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        break;
                    }
                case 2:
                    {
                        client.Inventory.Remove(3318834, 1, stream);
                        client.Inventory.Add(stream, 350118, 1, 0, 1, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        break;
                    }




            }

        }
        [NpcAttribute(NpcID.HotTurkey)]
        public static void HotTurkey(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Wich One Do You Want Your Accessory .");
                        data.AddOption("1-handed~Hot~Turkey~(B)", 1);
                        data.AddOption("2-handed~Hot~Turkey~(B)", 2);
                        data.AddAvatar(63).FinalizeDialog();
                        break;

                    }

                case 1:
                    {
                        client.Inventory.Remove(3318841, 1, stream);
                        client.Inventory.Add(stream, 360050, 1, 0, 1, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        break;
                    }
                case 2:
                    {
                        client.Inventory.Remove(3318841, 1, stream);
                        client.Inventory.Add(stream, 350170, 1, 0, 1, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        break;
                    }


            }

        }
        [NpcAttribute(NpcID.OceanSecret)]
        public static void OceanSecret(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Wich One Do You Want Your Accessory .");
                        data.AddOption("1-handed~Ocean~Secret~(B)", 1);
                        data.AddOption("2-handed~Ocean~Secret~(B)", 2);
                        data.AddAvatar(63).FinalizeDialog();
                        break;
                    }

                case 1:
                    {
                        client.Inventory.Remove(3318842, 1, stream);
                        client.Inventory.Add(stream, 360310, 1, 0, 1, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        break;
                    }
                case 2:
                    {
                        client.Inventory.Remove(3318842, 1, stream);
                        client.Inventory.Add(stream, 350173, 1, 0, 1, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        break;
                    }


            }

        }
        [NpcAttribute(NpcID.PhoenixLegend)]
        public static void PhoenixLegend(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Wich One Do You Want Your Accessory .");
                        data.AddOption("1-handed~Phoenix~Legend~(B)", 1);
                        data.AddOption("2-handed~Phoenix~Legend~(B)", 2);
                        data.AddAvatar(63).FinalizeDialog();
                        break;
                    }

                case 1:
                    {
                        client.Inventory.Remove(3318848, 1, stream);
                        client.Inventory.Add(stream, 360314, 1, 0, 1, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        break;
                    }
                case 2:
                    {
                        client.Inventory.Remove(3318848, 1, stream);
                        client.Inventory.Add(stream, 350177, 1, 0, 1, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        break;
                    }


            }

        }
        [NpcAttribute(NpcID.GraspofLove)]
        public static void GraspofLove(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Wich One Do You Want Your Accessory .");
                        data.AddOption("1-handed~Grasp~of~Love~(B)", 1);
                        data.AddOption("2-handed~Grasp~of~Love~(B)", 2);
                        data.AddAvatar(63).FinalizeDialog();
                        break;

                    }
                case 1:
                    {
                        client.Inventory.Remove(3318849, 1, stream);
                        client.Inventory.Add(stream, 360192, 1, 0, 1, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        break;
                    }
                case 2:
                    {
                        client.Inventory.Remove(3318849, 1, stream);
                        client.Inventory.Add(stream, 350099, 1, 0, 1, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        break;
                    }


            }

        }
        [NpcAttribute(NpcID.ArchivesArcher)]
        public static void ArchivesArcher(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.Text("Commander Yee:\n");
                        data.Text("    Evils are attacking furiously! As the commander of Archers, I am distraught! So why are you here?");
                        data.Option("Twisted Note is odd.", 1);
                        data.AddAvatar(101).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        data.Text("Commander Yee:\n");
                        data.Text("    It seems that the future has been written. The elders might have foreseen the crisis today when they gave you Twisted Note.");

                        data.Option("What is going on?", 2);
                        data.AddAvatar(101).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.Text("Commander Yee:\n");
                        data.Text("    Our base in Love Canyon was attacked by Evils tonight. The situation is critical. We need to repulse enemies before everything. Go to back them up with me.");

                        data.Option("Let`s go.", 3);
                        data.AddAvatar(101).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        client.Teleport(171, 165, 11223);


                        break;
                    }

            }

        }

        static readonly System.Collections.Concurrent.ConcurrentDictionary<uint, DateTime> CommanderYeeCalls = new System.Collections.Concurrent.ConcurrentDictionary<uint, DateTime>();// [QuestIntegration]
        [NpcAttribute(NpcID.CommanderYee)]
        public static void CommanderYee(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.Text("Commander Yee:\n");
                        data.Text("    Evil General Gongsun is foxy and wily. He sent Szeto to fight, and I was hurt by her hidden weapons.");

                        data.Option("Are you alright?", 1);
                        data.AddAvatar(101).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.Text("Commander Yee:\n");
                        data.Text("    Yes. It is no big deal, but I can`t use my power now. I have hurt Szeto`s lungs, so you can definitely defeat her now!");

                        data.Option("Leave it to me.", 2);
                        data.AddAvatar(101).FinalizeDialog();
                        break;

                    }
                case 2:
                    {
                        // [QuestIntegration] was an unlimited spawn (5 EvilDogface per click, anyone, any time): now only
                        // when fewer than 5 are alive around the spot, and one call per player every 60 seconds.
                        var map = Pool.ServerMaps[11039];
                        if (map == null)
                            break;
                        DateTime last;
                        if (CommanderYeeCalls.TryGetValue(client.Player.UID, out last) && DateTime.Now < last.AddSeconds(60))
                        {
                            client.SendSysMesage("Commander Yee: Catch your breath first. Come back in a minute.");
                            break;
                        }
                        int alive = 0;
                        foreach (var o in map.View.Roles(Role.MapObjectType.Monster, 100, 95))
                        {
                            var m = o as Game.MsgMonster.MonsterRole;
                            if (m != null && m.Alive && m.Family != null && m.Family.ID == 4221)
                                alive++;
                        }
                        if (alive >= 5)
                        {
                            client.SendSysMesage("Commander Yee: Szeto's men are still out there. Defeat them first!");
                            break;
                        }
                        CommanderYeeCalls[client.Player.UID] = DateTime.Now;
                        Server.AddMapMonster(stream, map, 4221, 91, 86, 18, 18, (byte)Math.Min(5, 5 - alive), 0, true);
                        break;
                    }

            }

        }

        [NpcAttribute(NpcID.BlackShadow1)]
        public static void BlackShadow1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.Text("Black Shadow:\n");
                        data.Text("    Please defeat the 4 Trapped Beasts first.");

                        data.Option("Challenge the beasts.", 1);
                        data.AddAvatar(13).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Player.Portal == 1)
                        {
                            client.Player.QuestGUI.SendAutoPatcher("STR_ID_tArchersTask[Msg][Enter][5][4]@@", 11223, 136, 162, 0);
                        }

                        break;

                    }


            }

        }

        [NpcAttribute(NpcID.BlackShadow2)]
        public static void BlackShadow2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.Text("Black Shadow:\n");
                        data.Text("    Please defeat the 4 Trapped Beasts first.");

                        data.Option("Challenge the beasts.", 1);
                        data.AddAvatar(13).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Player.Portal == 1)
                        {
                            client.Player.QuestGUI.SendAutoPatcher("STR_ID_tArchersTask[Msg][Enter][5][4]@@", 11223, 136, 162, 0);
                        }
                        break;

                    }


            }

        }

        [NpcAttribute(NpcID.BlackShadow3)]
        public static void BlackShadow3(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.Text("Black Shadow:\n");
                        data.Text("    Please defeat the 4 Trapped Beasts first.");

                        data.Option("Challenge the beasts.", 1);
                        data.AddAvatar(13).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Player.Portal == 1)
                        {
                            client.Player.QuestGUI.SendAutoPatcher("STR_ID_tArchersTask[Msg][Enter][5][4]@@", 11223, 136, 162, 0);
                        }
                        break;

                    }


            }

        }

        [NpcAttribute(NpcID.LuBan)]
        public static void LuBan(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.Text("Lu Ban:\n");
                        data.Text("    I feel like having a long dream! How are my old friends?");

                        data.Option("Lu Ban? What happened?", 1);
                        data.AddAvatar(13).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        data.Text("Lu Ban:\n");
                        data.Text("    Out of concern that our descendants can`t resist Evils, we recorded all skills in Twisted Note and sealed a breath of use in Mo Token. Once Mo Token is activated, Twisted Note can be opened.");

                        data.Option("You are forethoughtful.", 2);
                        data.AddAvatar(13).FinalizeDialog();
                        break;

                    }
                case 2:
                    {
                        data.Text("Lu Ban:\n");
                        data.Text("    Would you like to have a competition with me? Let me see how good my descendant Archer is.");

                        data.Option("My pleasure.", 3);
                        data.AddAvatar(13).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        if (!client.MyArchives.Items.ContainsKey(Archives.TypeID.StoneCracker))// [QuestIntegration] a second click reset the suit to level 1
                            client.MyArchives.AddItem(Archives.TypeID.StoneCracker, 1, 0, 1, 0);
                        break;
                    }


            }

        }

        [NpcAttribute(NpcID.MasterYan)]
        public static void MasterYan(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.Text("Master Yan:\n");
                        data.Text("    Glad to see you! Would you like to have a competition with me?");

                        data.Option("Sure!", 2);
                        data.AddAvatar(118).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        foreach (var obj in client.MyArchives.Items.Values)
                        {
                            if (obj.ItemID == Archives.TypeID.StoneCracker && obj.Level < 5)
                            {
                                data.Text("Master Yan:\n");
                                data.Text("STR_ID_tArchersTask[35672][Text212]@@");
                                data.Option("STR_ID_tArchersTask[35672][Option213]@@", 255);
                                data.Option("STR_ID_tArchersTask[35672][Option214]@@", 255);
                                data.AddAvatar(101).FinalizeDialog();
                                return;
                            }
                            else if (obj.ItemID == Archives.TypeID.StoneCracker && obj.Level >= 5 && !client.MyArchives.Items.ContainsKey(Archives.TypeID.ColdMoon))// [QuestIntegration] was "> 5" (level 5 fell through), and re-added it on every click
                            {
                                client.MyArchives.AddItem(Archives.TypeID.ColdMoon, 1, 0, 1, 0);

                            }
                        }

                        break;

                    }



            }

        }
        //[NpcAttribute(NpcID.LeeLongArchives)] // [NpcOfficial 2026-10-14] custom (non-official) NPC removed from the data; handler disabled
        public static void LeeLongArchives(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.Text("Hello, Sir I will Help You To Open LeeLongArchives\n");

                        data.Option("Dragon", 1);
                        data.Option("Kunpeng", 2);
                        data.Option("Suanni", 3);

                        data.Option("i,See!", 255);

                        data.AddAvatar(118).FinalizeDialog();
                        break;
                    }
                case 1:
                    {

                        client.MyArchives.AddItem(Archives.TypeID.Dragon, 1, 0, 1, 0);
                        break;

                    }
                case 2:
                    {

                        client.MyArchives.AddItem(Archives.TypeID.Kunpeng, 1, 0, 1, 0);
                        break;

                    }
                case 3:
                    {

                        client.MyArchives.AddItem(Archives.TypeID.Suanni, 1, 0, 1, 0);
                        break;

                    }



            }

        }

        //[NpcAttribute(NpcID.TaoOpen)] // [NpcOfficial 2026-10-14] custom (non-official) NPC removed from the data; handler disabled
        public static void TaoOpen(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.Text("STR_ID_tTaoistNewAreaTask[28972][Text111]@@\n");
                        data.Option("STR_ID_tTaoistNewMainTask[28975][Option1111]@@\n", 1);
                        data.Option("STR_ID_tTaoistNewMainTask[28975][Option1112]@@\n", 2);
                        data.Option("STR_ID_tTaoistNewMainTask[28975][Option1113]@@\n", 3);
                        data.Option("Unlock Thrill (Earth)", 4);
                        data.Option("Unlock Birthdeath (Earth)", 5);
                        data.Option("i,See!", 255);
                        data.AddAvatar(118).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.MyArchives.AddItem(Archives.TypeID.Vicissitude, 1, 0, 0, 0);
                        break;
                    }
                case 2:
                    {
                        foreach (var obj in client.MyArchives.Items.Values)
                        {
                            if (obj.ItemID == Archives.TypeID.Vicissitude && obj.Level < 10)
                            {
                                data.Text("STR_ID_tTaoistNewAreaTask[28972][Text111]@@\n");
                                data.Text("You need to have L10 War Suit first\n");
                                data.Option("i,See!", 255);
                                data.AddAvatar(118).FinalizeDialog();
                                return;
                            }
                        }
                        client.MyArchives.AddItem(Archives.TypeID.HighestGood, 1, 0, 0, 0);
                        break;
                    }
                case 3:
                    {
                        foreach (var obj in client.MyArchives.Items.Values)
                        {
                            if (obj.ItemID == Archives.TypeID.HighestGood && obj.Level < 10)
                            {
                                data.Text("STR_ID_tTaoistNewAreaTask[28972][Text111]@@\n");
                                data.Text("You need to have L10 War Suit first\n");
                                data.Option("i,See!", 255);
                                data.AddAvatar(118).FinalizeDialog();
                                return;
                            }
                        }
                        client.MyArchives.AddItem(Archives.TypeID.Evolution, 1, 0, 0, 0);
                        break;
                    }
                case 4:
                    {
                        foreach (var obj in client.MyArchives.Items.Values)
                        {
                            if (obj.ItemID == Archives.TypeID.Evolution && obj.Level < 10)
                            {
                                data.Text("STR_ID_tTaoistNewAreaTask[28972][Text111]@@\n");
                                data.Text("You need to have L10 War Suit first\n");
                                data.Option("i,See!", 255);
                                data.AddAvatar(118).FinalizeDialog();
                                return;
                            }
                        }
                        client.MyArchives.AddItem(Archives.TypeID.Thrill, 1, 0, 0, 0);
                        break;
                    }
                case 5:
                    {
                        foreach (var obj in client.MyArchives.Items.Values)
                        {
                            if (obj.ItemID == Archives.TypeID.Thrill && obj.Level < 10)
                            {
                                data.Text("STR_ID_tTaoistNewAreaTask[28972][Text111]@@\n");
                                data.Text("You need to have L10 War Suit first\n");
                                data.Option("i,See!", 255);
                                data.AddAvatar(118).FinalizeDialog();
                                return;
                            }
                        }
                        client.MyArchives.AddItem(Archives.TypeID.BirthDeath, 1, 0, 0, 0);
                        break;
                    }


            }

        }
        //[NpcAttribute(NpcID.PirateOpen)] // [NpcOfficial 2026-10-14] custom (non-official) NPC removed from the data; handler disabled
        public static void PirateOpen(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.Text("Hello, Sir I will Help You To Open PirateArchives\n");
                        data.Option("Thunder Nut", 1);
                        data.Option("Frozen Nut", 2);
                        data.Option("Lava Nut", 3);
                        data.Option("i,See!", 255);

                        data.AddAvatar(118).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        ActionQuery action = new ActionQuery()
                        {
                            Type = ActionType.InspiredPirate,
                            ObjId = client.Player.UID,
                            dwParam3 = 22,
                            Timestamp = (uint)Time32.timeGetTime().GetHashCode(),

                        };
                        client.Send(stream.ActionCreate(action));
                        client.MyArchives.AddItem(Archives.TypeID.ThunderNut, 1, 0, 1, 0);

                        break;


                    }
                case 2:
                    {
                        ActionQuery action = new ActionQuery()
                        {
                            Type = ActionType.InspiredPirate,
                            ObjId = client.Player.UID,
                            dwParam3 = 22,
                            Timestamp = (uint)Time32.timeGetTime().GetHashCode(),

                        };
                        client.Send(stream.ActionCreate(action));
                        client.MyArchives.AddItem(Archives.TypeID.FrozenNut, 1, 0, 1, 0);
                        break;

                    }
                case 3:
                    {
                        ActionQuery action = new ActionQuery()
                        {
                            Type = ActionType.InspiredPirate,
                            ObjId = client.Player.UID,
                            dwParam3 = 22,
                            Timestamp = (uint)Time32.timeGetTime().GetHashCode(),

                        };
                        client.Send(stream.ActionCreate(action));
                        client.MyArchives.AddItem(Archives.TypeID.LavaNut, 1, 0, 1, 0);
                        break;

                    }



            }

        }

        [NpcAttribute(NpcID.MoDi)]
        public static void MoDi(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.Text("Mo Di:\n");
                        data.Text("STR_ID_tArchersTask[35673][Text122]@@");

                        data.Option("STR_ID_tArchersTask[35673][Option121]@@", 1);

                        data.AddAvatar(120).FinalizeDialog();

                        break;
                    }
                case 1:
                    {

                        foreach (var obj in client.MyArchives.Items.Values)
                        {
                            if (obj.ItemID == Archives.TypeID.ColdMoon && obj.Level < 10)
                            {
                                data.Text("Commander Yee:\n");
                                data.Text("STR_ID_tArchersTask[35673][Text212]@@");
                                data.Option("STR_ID_tArchersTask[35673][Option213]@@", 255);
                                data.Option("STR_ID_tArchersTask[35673][Option214]@@", 255);
                                data.AddAvatar(101).FinalizeDialog();
                                return;
                            }
                            else if (obj.ItemID == Archives.TypeID.ColdMoon && obj.Level >= 10 && !client.MyArchives.Items.ContainsKey(Archives.TypeID.ThornCutter))// [QuestIntegration] was "> 10" (level 10 fell through), and re-added it on every click
                            {
                                client.MyArchives.AddItem(Archives.TypeID.ThornCutter, 1, 0, 1, 0);
                            }
                        }
                        break;

                    }



            }

        }
        [NpcAttribute(NpcID.ArchivesWarrior)]
        public static void ArchivesWarrior(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.Text("STR_ID_tWarriorAwakenTask[27666][Text1091]@@");
                        data.Text("STR_ID_tWarriorAwakenTask[27666][Text1092]@@");

                        data.Option("STR_ID_tWarriorAwakenTask[27666][Option1091]@@", 1);

                        data.AddAvatar(3186).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.Text("STR_ID_tWarriorAwakenTask[27666][Text10121]@@");
                        data.Text("STR_ID_tWarriorAwakenTask[27666][Text10122]@@");

                        data.Option("STR_ID_tWarriorAwakenTask[27666][Option10121]@@", 2);

                        data.AddAvatar(3186).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        client.Teleport(93, 73, 11041);
                        client.Player.QuestGUI.SendAutoPatcher(11041, 97, 46, (uint)NpcID.HuaMulan);
                        break;
                    }
            }

        }
        [NpcAttribute(NpcID.GeneralXiao)]
        public static void GeneralXiao(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.HuaMulan != 1 || client.Player.Ares == 2)
                        {

                            data.Text("STR_ID_tWarriorAwakenAfter[27644][Text511]@@");
                            data.Text("STR_ID_tWarriorAwakenAfter[27644][Text512]@@");

                            data.Option("STR_ID_tWarriorAwakenAfter[27644][Option511]@@", 1);

                            data.AddAvatar(3186).FinalizeDialog();
                            return;
                        }
                        data.Text("STR_ID_tWarriorAwakenAfter[27644][Text1091]@@");
                        data.Text("STR_ID_tWarriorAwakenAfter[27644][Text1092]@@");
                        if (client.Player.Tyre == 0)
                        {
                            data.Option("STR_ID_tWarriorAwakenAfter[27644][Option1091]@@", 2);
                            data.AddAvatar(3186).FinalizeDialog();
                            return;
                        }
                        data.Option("STR_ID_tWarriorAwakenAfter[27644][Option1092]@@", 3);
                        data.AddAvatar(3186).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(11041, 97, 46, (uint)NpcID.HuaMulan);

                        break;
                    }
                case 2:
                    {
                        foreach (var obj in client.MyArchives.Items.Values)
                        {

                            if (obj.ItemID == Archives.TypeID.Dragonhowl && obj.Level < 15)
                            {

                                client.CreateBoxDialog("STR_ID_tWarriorAwakenAfter[Msg][CombatLevel][2]@@");
                                return;
                            }

                        }
                        client.Player.QuestGUI.SendAutoPatcher(11041, 108, 46, (uint)NpcID.Tyre);
                        client.Player.Tyre = 1;
                        break;
                    }
                case 3:
                    {
                        if (client.Player.Archives == 1)
                        {
                            client.CreateBoxDialog("STR_ID_tWarriorAwakenAfter[Msg][CombatLevel][2]@@");
                            return;
                        }

                        foreach (var obj in client.MyArchives.Items.Values)
                        {
                            if (obj.ItemID == Archives.TypeID.Bloodlust && obj.Level < 15)
                            {

                                client.CreateBoxDialog("STR_ID_tWarriorAwakenAfter[Msg][CombatLevel][2]@@");
                                return;

                            }



                        }
                        client.Player.QuestGUI.SendAutoPatcher(11041, 86, 46, (uint)NpcID.Ares);
                        if (client.Player.Tyre != 1 && client.Player.Ares != 1)
                        {
                            client.Player.Ares = 1;
                        }
                        break;
                    }

            }

        }
        [NpcAttribute(NpcID.HuaMulan)]
        public static void HuaMulan(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.Text("STR_ID_tWarriorAwakenAfter[27641][Text411]@@");
                        data.Text("STR_ID_tWarriorAwakenAfter[27641][Text412]@@");
                        data.Option("STR_ID_tWarriorAwakenAfter[27641][Option411]@@", 1);
                        data.AddAvatar(3183).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        ActionQuery naction = new ActionQuery()
                        {
                            ObjId = client.Player.UID,
                            dwParam3 = 104,//158 //186
                            Type = 456, //137
                            PositionX = client.Player.X,
                            PositionY = client.Player.Y,

                        };
                        client.Send(stream.ActionCreate(naction));
                        client.MyArchives.AddItem(Archives.TypeID.Dragonhowl, 1, 0, 1, 0);
                        client.Player.HuaMulan = 1;
                        client.Player.Archives = 1;
                        break;
                    }

            }

        }
        [NpcAttribute(NpcID.Ares)]
        public static void Ares(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.Text("STR_ID_tWarriorAwakenAfter[27642][Text811]@@");
                        data.Text("STR_ID_tWarriorAwakenAfter[27642][Text812]@@");
                        data.Option("STR_ID_tWarriorAwakenAfter[27642][Option811]@@", 1);
                        data.AddAvatar(3184).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        ActionQuery naction = new ActionQuery()
                        {
                            ObjId = client.Player.UID,
                            dwParam3 = 105,//158 //186
                            Type = 456, //137
                            PositionX = client.Player.X,
                            PositionY = client.Player.Y,

                        };
                        client.Send(stream.ActionCreate(naction));
                        client.MyArchives.AddItem(Archives.TypeID.Bloodlust, 1, 0, 1, 0);
                        client.Player.Tyre = 2;
                        client.Player.Archives = 2;
                        break;
                    }


            }

        }
        [NpcAttribute(NpcID.Tyre)]
        public static void Tyre(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.Text("STR_ID_tWarriorAwakenAfter[27643][Text811]@@");
                        data.Text("STR_ID_tWarriorAwakenAfter[27643][Text812]@@");
                        data.Option("STR_ID_tWarriorAwakenAfter[27643][Option811]@@", 1);
                        data.AddAvatar(3185).FinalizeDialog();

                        break;
                    }
                case 1:
                    {

                        ActionQuery naction = new ActionQuery()
                        {
                            ObjId = client.Player.UID,
                            dwParam3 = 106,//158 //186
                            Type = 456, //137
                            PositionX = client.Player.X,
                            PositionY = client.Player.Y,

                        };
                        client.Send(stream.ActionCreate(naction));
                        client.MyArchives.AddItem(Archives.TypeID.Redcurse, 1, 0, 1, 0);
                        client.Player.Ares = 2;
                        break;
                    }


            }

        }
        [NpcAttribute(NpcID.ClassFull)]
        public static void ClassFull(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hey , Iam Selling Class Full For 500M .");
                        data.AddOption("Trojan ", 1);
                        data.AddOption("Warrior ", 2);
                        data.AddOption("Archer ", 3);
                        data.AddOption("Ninja ", 4);
                        data.AddOption("Monk ", 5);
                        data.AddOption("Pirate ", 6);
                        data.AddOption("DragonWarrior ", 7);
                        data.AddOption("ThunderStrike ", 8);
                        data.AddOption("Water ", 9);
                        data.AddOption("Fire ", 10);
                        data.AddOption("WindWalker ", 11);
                        data.AddOption("THX .");
                        data.AddAvatar(263).FinalizeDialog();
                        break;
                    }
                #region Trojan
                case 1:
                    {
                        if (client.Player.ConquerPoints >= 500000000)
                        {
                            if (client.Player.Class >= 1000 && client.Player.Class <= 1049)
                            {
                                client.Player.ConquerPoints -= 500000000;
                                client.Player.Class = 1049;//trojan
                                client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                            }
                            else
                            {
                                data.Text("your not trojan ..");
                                data.AddOption("THX .", 255);
                                data.AddAvatar(6).FinalizeDialog();
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
                        if (client.Player.ConquerPoints >= 500000000)
                        {
                            if (client.Player.Class >= 2000 && client.Player.Class <= 2049)
                            {
                                client.Player.ConquerPoints -= 500000000;
                                client.Player.Class = 2049;//warrior
                                client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                            }
                            else
                            {
                                data.Text("your not warrior ..");
                                data.AddOption("THX .", 255);
                                data.AddAvatar(6).FinalizeDialog();
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
                        if (client.Player.ConquerPoints >= 500000000)
                        {
                            if (client.Player.Class >= 4000 && client.Player.Class <= 4049)
                            {
                                client.Player.ConquerPoints -= 500000000;
                                client.Player.Class = 4049;//Archer
                                client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                            }
                            else
                            {
                                data.Text("your not archer ..");
                                data.AddOption("THX .", 255);
                                data.AddAvatar(6).FinalizeDialog();
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
                        if (client.Player.ConquerPoints >= 500000000)
                        {
                            if (client.Player.Class >= 5000 && client.Player.Class <= 5049)
                            {
                                client.Player.ConquerPoints -= 500000000;
                                client.Player.Class = 5049;//Ninja
                                client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                            }
                            else
                            {
                                data.Text("your not ninja ..");
                                data.AddOption("THX .", 255);
                                data.AddAvatar(6).FinalizeDialog();
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
                        if (client.Player.ConquerPoints >= 500000000)
                        {
                            if (client.Player.Class >= 6000 && client.Player.Class <= 6049)
                            {
                                client.Player.ConquerPoints -= 500000000;
                                client.Player.Class = 6049;//Monk
                                client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                            }
                            else
                            {
                                data.Text("your not monk ..");
                                data.AddOption("THX .", 255);
                                data.AddAvatar(6).FinalizeDialog();
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
                        if (client.Player.ConquerPoints >= 500000000)
                        {
                            if (client.Player.Class >= 7000 && client.Player.Class <= 7049)
                            {
                                client.Player.ConquerPoints -= 500000000;
                                client.Player.Class = 7049;//Pirate
                                client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                            }
                            else
                            {
                                data.Text("your not pirate ..");
                                data.AddOption("THX .", 255);
                                data.AddAvatar(6).FinalizeDialog();
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
                        if (client.Player.ConquerPoints >= 500000000)
                        {
                            if (client.Player.Class >= 8000 && client.Player.Class <= 8049)
                            {
                                client.Player.ConquerPoints -= 500000000;
                                client.Player.Class = 8049;//DragonWarrior
                                client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                            }
                            else
                            {
                                data.Text("your not dragonwarrior ..");
                                data.AddOption("THX .", 255);
                                data.AddAvatar(6).FinalizeDialog();
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
                        if (client.Player.ConquerPoints >= 500000000)
                        {
                            if (client.Player.Class >= 9000 && client.Player.Class <= 9049)
                            {
                                client.Player.ConquerPoints -= 500000000;
                                client.Player.Class = 9049;//ThunderStrike
                                client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                            }
                            else
                            {
                                data.Text("your not thunderstrike ..");
                                data.AddOption("THX .", 255);
                                data.AddAvatar(6).FinalizeDialog();
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
                        if (client.Player.ConquerPoints >= 500000000)
                        {
                            if (client.Player.Class >= 10000 && client.Player.Class <= 13049)
                            {
                                client.Player.ConquerPoints -= 500000000;
                                client.Player.Class = 13049;//Water
                                client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                            }
                            else
                            {
                                data.Text("your not water ..");
                                data.AddOption("THX .", 255);
                                data.AddAvatar(6).FinalizeDialog();
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
                        if (client.Player.ConquerPoints >= 500000000)
                        {
                            if (client.Player.Class >= 14002 && client.Player.Class <= 14049)
                            {
                                client.Player.ConquerPoints -= 500000000;
                                client.Player.Class = 14049;//Fire
                                client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                            }
                            else
                            {
                                data.Text("your not fire ..");
                                data.AddOption("THX .", 255);
                                data.AddAvatar(6).FinalizeDialog();
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
                        if (client.Player.ConquerPoints >= 500000000)
                        {
                            if (client.Player.Class >= 16000 && client.Player.Class <= 16049)
                            {
                                client.Player.ConquerPoints -= 500000000;
                                client.Player.Class = 16049;//Windwalker
                                client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                            }
                            else
                            {
                                data.Text("your not windwalker ..");
                                data.AddOption("THX .", 255);
                                data.AddAvatar(6).FinalizeDialog();
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
        //[NpcAttribute(NpcID.NewPlayer2)]
        //public static void NewPlayer2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog data = new Dialog(client, stream);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                client.firstchar = client.secondchar = client.finalchar = 0;
        //                data.AddText("Hello [ <tip color=0xffc501e7> " + client.Player.Name + " </tip> ] ,Welcome to TroyConquer\n");
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
        //                data.AddOption("TwinCity ", 150);
        //                data.AddOption("I`ll~think~about~it.", 255);
        //                data.AddAvatar(67).FinalizeDialog();
        //                break;
        //            }
        //        case 122:
        //            {
        //                data.AddText("Hello [ <tip color=0xffc501e7> " + client.Player.Name + " </tip> ] ,Welcome to TroyConquer\n");
        //                foreach (var info in client.MySpells.ClientSpells.Values)
        //                {
        //                    data.AddOption(Pool.Magic[info.ID].Values.FirstOrDefault().Name.ToString(), 123);
        //                }
        //                data.AddAvatar(67).FinalizeDialog();
        //                break;
        //            }
        //        case 123:
        //            {
        //                break;
        //            }
        //        case 121:
        //            {
        //                client.Player.SetLocationType = 2;
        //                MsgInterServer.PipeClient.Connect(client, Database.GroupServerList.InterServer.IPAddress, Database.GroupServerList.InterServer.Port);
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
        //        case 150:
        //            {
        //                client.Teleport(410, 354, 1002);
        //                break;
        //            }
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
        //                client.Player.Class = (uint)Database.AtributesStatus.NewClass(client.finalchar);//
        //                client.Player.SendUpdate(stream, client.Player.Class, Game.MsgServer.MsgUpdate.DataType.Class);

        //                client.Player.FirstClass = (uint)Database.AtributesStatus.NewClass(client.firstchar);//
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

        //                 foreach (var m_stage in client.Player.InnerPower.Stages)
        //                {
        //                    foreach (var m_gong in m_stage.NeiGongs)
        //                    {
        //                        m_stage.UnLocked = m_gong.Unlocked = true;
        //                        m_gong.Score = 100;
        //                        Database.InnerPowerTable.Stage DBStage = null;
        //                        Database.InnerPowerTable.Stage.NeiGong DBGong = null;
        //                        if (Database.InnerPowerTable.GetDBInfo(m_gong.ID, out DBStage, out DBGong))
        //                            m_gong.level = DBGong.MaxLevel;
        //                    }
        //                }
        //                client.Player.MyJiangHu.DisableJiang(client);

        //                break;

        //            }

        //    }
        //}
        //[NpcAttribute(NpcID.InnerPower2)] // [NpcOfficial 2026-10-14] custom (non-official) NPC removed from the data; handler disabled
        public static void InnerPower2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.Level >= 100 && client.Player.Reborn >= 2)
                        {
                            if (client.Player.InnerPower.Stages[0].UnLocked == false)
                            {
                                data.AddText("Hello ! [ <tip color=0xfff1895>" + client.Player.Name + " </tip> ] aim InnerPower NPC , I can fixed your InnerPower /n")
                                 .AddText(" For [ 10 ] He will stay with you forever .Finally, Do you agree to fixed it, Just Try !.")
                                .AddOption("Fixed [InnerPower]", 1)
                                .AddOption("No Thx.. .", 255)
                                .AddAvatar(265).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Kid, You Have Already Fixed Your InnerPower")
                               .AddOption("I'm just passing by.", 255)
                               .AddAvatar(261).FinalizeDialog();
                                break;
                            }
                        }
                        else
                        {
                            data.AddText("Sorry Kid, Sorry, you not have 2nd Reborn + Lev 100 to use [ InnerPower ].")
                           .AddOption("I'm just passing by.", 255)
                           .AddAvatar(261).FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
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
                            }
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.WorldInnerPower)]
        public static void WorldInnerPower(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("Hello, I Give You Full Inner Powers For 10.000 CPs  ");
                        data.AddOption("I~Want~Full~Inner~Powers.", 1);
                        data.AddOption("I~will~come~later.", 255);

                        data.AddAvatar(288).FinalizeDialog();

                        break;
                    }


                case 1:
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
                                    data.AddText("Gratz You received Full Inner Powers For 10.000 CPs")
                          .AddOption("Thanks.", 255)
                          .AddAvatar(6).FinalizeDialog();

                                }
                            }
                        }
                        else
                        {
                            data.AddText("You do not have 10.000 CPs with you.")
                           .AddOption("I'm just passing by.", 255)
                           .AddAvatar(6).FinalizeDialog();
                        }
                        break;
                    }
            }

        }

        //[NpcAttribute(NpcID.FullAccountNightMareCo)]
        //public static void FullAccountNightMareCo(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog data = new Dialog(client, stream);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                data.AddText("Hey," + client.Player.Name + "Which you need stuff");
        //                data.AddOption("FullStuff", 98);
        //                data.AddOption("FullPromotion", 111);
        //                data.AddOption("FullInnerPower", 112);
        //                data.AddOption("FullChi", 113);
        //                data.AddOption("FullJiang", 114);
        //                data.AddOption("FullTailedBeast", 115);
        //                data.AddOption("MaxYellowRune", 116);
        //                data.AddOption("MaxBlueRune", 117);
        //                data.AddOption("Relic", 118);
        //                data.AddOption("RedRune", 122);
        //                data.AddOption("SacredRefinery", 119); 
        //                data.AddOption("Level140", 120);
        //                data.AddOption("MaxMythSoul", 121);
        //                data.AddOption("SacredRefineryPack", 127);
        //                data.AddOption("P7WeaponSoulPack", 124);
        //                data.AddOption("P7EquipmentSoul", 125);
        //                data.AddOption("Full Anima P18", 128);
        //                data.AddOption("900Attribute", 126);

        //                data.AddAvatar(288).FinalizeDialog();

        //                break;
        //            }
        //        case 128:
        //            {
        //                if (client.Inventory.HaveSpace(7))
        //                {
        //                    client.Inventory.AddItemWitchStack(4200018, 0, 7, stream, false);
        //                }
        //                else
        //                {
        //                    data.AddText("Inventory Full Empty 7 Space");
        //                    data.AddAvatar(263).FinalizeDialog();

        //                }
        //                break;
        //            }
        //        case 126:
        //            {
        //                client.Player.Vitality = 1; client.Player.Agility = client.Player.Strength = client.Player.Spirit = 0; client.Player.Atributes = 901;
        //                client.Player.SendUpdate(stream, client.Player.Strength, Game.MsgServer.MsgUpdate.DataType.Strength);
        //                client.Player.SendUpdate(stream, client.Player.Agility, Game.MsgServer.MsgUpdate.DataType.Agility);
        //                client.Player.SendUpdate(stream, client.Player.Spirit, Game.MsgServer.MsgUpdate.DataType.Spirit);
        //                client.Player.SendUpdate(stream, client.Player.Vitality, Game.MsgServer.MsgUpdate.DataType.Vitality);
        //                client.Player.SendUpdate(stream, client.Player.Atributes, Game.MsgServer.MsgUpdate.DataType.Atributes);
        //                client.CreateBoxDialog("Success ! You successfully and received 902 Attribute Point`s.");
        //                break;
        //            }
        //        case 124:
        //            {
        //                if (client.Inventory.HaveSpace(7))
        //                {
        //                    client.Inventory.AddItemWitchStack(3004247, 0, 7, stream, false);
        //                }
        //                else
        //                {
        //                    data.AddText("Inventory Full Empty 7 Space");
        //                    data.AddAvatar(263).FinalizeDialog();

        //                }
        //                break;
        //            }
        //        case 125:
        //            {
        //                if (client.Inventory.HaveSpace(7))
        //                {
        //                    client.Inventory.AddItemWitchStack(3004248, 0, 7, stream, false);
        //                }
        //                else
        //                {
        //                    data.AddText("Inventory Full Empty 7 Space");
        //                    data.AddAvatar(263).FinalizeDialog();

        //                }
        //                break;
        //            }
        //        case 127:
        //            {
        //                if (client.Inventory.HaveSpace(7))
        //                {
        //                    client.Inventory.AddItemWitchStack(3004249, 0, 7, stream, false);
        //                }
        //                else
        //                {
        //                    data.AddText("Inventory Full Empty 7 Space");
        //                    data.AddAvatar(263).FinalizeDialog();

        //                }
        //                break;
        //            }
        //        case 122:
        //            {
        //                if (client.Inventory.HaveSpace(1))
        //                {
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@redrunes", 0, 0));
        //                }
        //                else
        //                {
        //                    data.AddText("Inventory Full Empty 1 Space");
        //                    data.AddAvatar(263).FinalizeDialog();

        //                }
        //                break;
        //            }
        //        case 121:
        //            {
        //                if (client.Inventory.HaveSpace(1))
        //                {
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@myhthss", 0, 0));
        //                }
        //                else
        //                {
        //                    data.AddText("Inventory Full Empty 1 Space");
        //                    data.AddAvatar(263).FinalizeDialog();

        //                }
        //                break;
        //            }
        //        case 120:
        //            {
        //                if (client.Player.Level >= 140)
        //                {
        //                    client.CreateBoxDialog("You Highest Level");
        //                }
        //                client.UpdateLevel(stream, 140);
        //                break;

        //            }

        //        case 111:
        //            {

        //                client.Player.ClassExperience += 500000;

        //                break;
        //            }
        //        case 112:
        //            {
        //                client.Player.InnerPower.AddPotency(stream, client, 7100000);
        //                foreach (var stage in Database.InnerPowerTable.Stages)
        //                {
        //                    foreach (var gong in stage.NeiGongAtributes)
        //                    {
        //                        using (var rec = new ServerSockets.RecycledPacket())
        //                        {
        //                            var packet = rec.GetStream();
        //                            client.Inventory.Add(packet, gong.ItemID);
        //                        }
        //                    }
        //                }
        //                break;
        //            }
        //        case 113:
        //            {
        //                data.AddText("Wich One Do You Want Your Chi Fixed With NightMareFoooo2 .");
        //                data.AddOption("[HP]+[Attack]+[Immy]+[Strike].", 91);
        //                data.AddOption("[HP]+[Attack]+[Immy]+[Break].", 92);
        //                data.AddOption("[HP]+[M-Attack]+[Immy]+[M-Strike].", 93);
        //                data.AddOption("[HP]+[P-Damge]+[AntiBreak]+[Immy].", 94);
        //                data.AddOption("NeverMind .", 255);
        //                data.AddAvatar(63).FinalizeDialog();
        //                break;

        //            }
        //            case 91:
        //            {
        //                if (client.Player.ConquerPoints >= 1)
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



        //                    client.CreateBoxDialog("You've Chi~Strike.");
        //                }
        //                else
        //                {
        //                    data.AddText("You can not use this option for You've Chi~Strike .")
        //                        .AddOption("I see.", 255)
        //                          .AddAvatar(264)
        //                        .FinalizeDialog();
        //                }


        //                break;
        //            }
        //        case 92:
        //            {
        //                if (client.Player.ConquerPoints >= 1)
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
        //                    client.CreateBoxDialog("You've Chi~Break.");
        //                }
        //                else
        //                {
        //                    data.AddText("You can not use this option for You've Chi~Break .")
        //                        .AddOption("I see.", 255)
        //                          .AddAvatar(264)
        //                        .FinalizeDialog();
        //                }


        //                break;
        //            }
        //        case 93:
        //            {
        //                if (client.Player.ConquerPoints >= 1)
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
        //                    client.CreateBoxDialog("You've Chi~MAttack.");
        //                }
        //                else
        //                {
        //                    data.AddText("You can not use this option for You've Chi~Fire .")
        //                        .AddOption("I see.", 255)
        //                          .AddAvatar(264)
        //                        .FinalizeDialog();
        //                }


        //                break;
        //            }
        //        case 94:
        //            {
        //                if (client.Player.ConquerPoints >= 1)
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
        //                    client.CreateBoxDialog("You've Chi~Defence.");
        //                }
        //                else
        //                {
        //                    data.AddText("You can not use this option for You've Chi~Defense .")
        //                        .AddOption("I see.", 255)
        //                          .AddAvatar(264)
        //                        .FinalizeDialog();
        //                }


        //                break;
        //            }
        //        case 114:
        //            {
        //                if (client.Inventory.HaveSpace(9))
        //                {
        //                    client.Inventory.AddItemWitchStack(728173, 0, 9, stream);
        //                }
        //                else
        //                {
        //                    data.AddText("Inventory Full Empty 9 Space");
        //                    data.AddAvatar(263).FinalizeDialog();

        //                }

        //                break;
        //            }
        //        case 115:
        //            {
        //                client.Inventory.AddItemWitchStack(3009104, 0, 130, stream);
        //                break;
        //            }
        //        case 116:
        //            {
        //                if (client.Inventory.HaveSpace(1))
        //                {
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@maxyellow", 0, 0));
        //                }
        //                else
        //                {
        //                    data.AddText("Inventory Full Empty 1 Space");
        //                    data.AddAvatar(263).FinalizeDialog();

        //                }
        //                break;
        //            }
        //        case 117:
        //            {
        //                if (client.Inventory.HaveSpace(40))
        //                {
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@maxblue", 0, 0));
        //                }
        //                else
        //                {
        //                    data.AddText("Inventory Full Empty 40 Space");
        //                    data.AddAvatar(263).FinalizeDialog();

        //                }
        //                break;
        //            }
        //        case 118:
        //            {
        //                if (client.Inventory.HaveSpace(1))
        //                {
        //                    client.Inventory.AddItemWitchStack(3307146, 0, 1, stream);
        //                }
        //                else
        //                {
        //                    data.AddText("Inventory Full Empty 1 Space");
        //                    data.AddAvatar(263).FinalizeDialog();

        //                }
        //                break;
        //            }
        //        case 119:
        //            {
        //                if (client.Inventory.HaveSpace(7))
        //                {
        //                    client.Inventory.AddItemWitchStack(3004249, 0, 7, stream);
        //                }
        //                else
        //                {
        //                    data.AddText("Inventory Full Empty 7 Space");
        //                    data.AddAvatar(263).FinalizeDialog();

        //                }
        //                break;
        //            }



        //        case 123:
        //            {

        //                break;

        //            }

        //        #region stuff
        //        case 98:
        //            {
        //                data.AddOption("Headgear", 1)
        //                .AddOption("Armor", 2)
        //                .AddOption("Necklace", 3)
        //                .AddOption("Weapons", 4)
        //                .AddOption("Ring", 5)
        //                .AddOption("Boots", 6)
        //                .AddOption("Talisman", 7)
        //                .AddOption("Thx,NightMareCo .", 255)
        //                .AddAvatar(285)
        //                .FinalizeDialog(); 

        //                break;

        //            }
        //        case 1:
        //            data.AddText("Select What's Character u want?")
        //                .AddOption("Trojan", 8)
        //                .AddOption("Warrior", 9)
        //                .AddOption("Taoist", 10)
        //                .AddOption("Archer", 100)
        //                .AddOption("Ninja", 11)
        //                .AddOption("Monk", 12)
        //                .AddOption("Pirate", 13)
        //                .AddOption("DragonWarrior", 14)
        //                .AddOption("Windwalker", 15)
        //                .AddOption("Thunderstriker", 254)
        //                .AddAvatar(292)
        //                .FinalizeDialog();
        //            break;
        //        case 2:
        //            data.AddText("Select What's Character u want?")
        //                .AddOption("Trojan", 16)
        //                .AddOption("Warrior", 17)
        //                .AddOption("Taoist", 18)
        //                .AddOption("Archer", 101)
        //                .AddOption("Ninja", 19)
        //                .AddOption("Monk", 20)
        //                .AddOption("Pirate", 21)
        //                .AddOption("DragonWarrior", 22)
        //                .AddOption("Windwalker", 23)
        //                .AddOption("Thunderstriker", 253)
        //                .AddAvatar(292)
        //                .FinalizeDialog();
        //            break;
        //        case 3:
        //            data.AddText("Select What's Character u want?")
        //                .AddOption("Necklace", 24)
        //                .AddOption("Bag", 25)
        //                .AddAvatar(292)
        //                .FinalizeDialog();
        //            break;
        //        case 4:
        //            data.AddText("Select What's Character u want?")
        //                .AddOption("Trojan", 26)
        //                .AddOption("Warrior", 27)
        //                .AddOption("Taoist", 28)
        //                .AddOption("Archer", 102)
        //                .AddOption("Ninja", 29)
        //                .AddOption("Monk", 30)
        //                .AddOption("Pirate", 31)
        //                .AddOption("DragonWarrior", 32)
        //                .AddOption("Windwalker", 33)
        //                .AddOption("Thunderstriker", 252)
        //                .AddAvatar(292)
        //                .FinalizeDialog();
        //            break;
        //        case 5:
        //            data.AddText("Select What u want?")
        //                .AddOption("Ring", 34)
        //                .AddOption("HeavyRing", 35)
        //                .AddOption("Braclate", 36)
        //                .AddAvatar(292)
        //                .FinalizeDialog();
        //            break;
        //        case 6:
        //            data.AddText("Select What u want?")
        //                .AddOption("Normal - Strike", 37)
        //                .AddOption("Normal - Break", 70)
        //                .AddOption("Taoist - MAttack", 38)
        //                .AddAvatar(292)
        //                .FinalizeDialog();
        //            break;
        //        case 7:
        //            {

        //                client.Inventory.AddFullTag(stream, 201009, 1, 12, 1, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperThunderGem, false);
        //                client.Inventory.AddFullTag(stream, 202009, 1, 12, 1, 0, Role.Flags.Gem.SuperGloryGem, Role.Flags.Gem.SuperGloryGem, false);
        //                client.Inventory.AddFullTag(stream, 203009, 1, 12, 1, 0, 0, 0, false);   // RidingCrop
        //                client.Inventory.AddFullTag(stream, 204009, 1, 0, 1, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperGloryGem, false);
        //                client.Inventory.AddSteedFull(stream, 300000, 1, 12, false, 255, 0, 150);
        //                break;
        //            }
        //        case 8:
        //            data.AddText("Select What u want?")
        //                .AddOption("Headgear Immunity", 43)
        //                .AddOption("Headgear AntiBreak", 44)
        //                .AddAvatar(292)
        //                .FinalizeDialog();
        //            break;
        //        case 170:
        //            {
        //                if (!client.Inventory.HaveSpace(1))
        //                {
        //                    client.CreateBoxDialog("Please make 1 more space in your inventory.");
        //                    break;
        //                }
        //                if (client.Player.ConquerPoints >= 1)
        //                {
        //                    client.Player.ConquerPoints -= 1;
        //                    client.Inventory.AddSteed(stream, 300000, 1, 12, false, 255, 0, 150);
        //                }
        //                else
        //                {

        //                    data.AddText("Please take 1 cps .")
        //                    .AddOption("Thanks!", 255)
        //                    .AddAvatar(166).FinalizeDialog();

        //                }

        //                break;
        //            }
        //        case 43:
        //            client.Inventory.AddSoul2(stream, client, 118309, 820073, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            break;
        //        case 44:
        //            client.Inventory.AddSoul2(stream, client, 118309, 820074, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            break;
        //        case 9:
        //            data.AddText("Select What u want?")
        //                .AddOption("Headgear Immunity", 45)
        //                .AddOption("Headgear AntiBreak", 46)
        //                .AddAvatar(292)
        //                .FinalizeDialog();
        //            break;
        //        case 45:
        //            client.Inventory.AddSoul2(stream, client, 141309, 820073, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            break;
        //        case 46:
        //            client.Inventory.AddSoul2(stream, client, 141309, 820074, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            break;
        //        case 10:
        //            data.AddText("Select What u want?")
        //                .AddOption("Headgear Immunity", 47)
        //                .AddOption("Headgear AntiBreak", 48)
        //                .AddAvatar(292)
        //                .FinalizeDialog();
        //            break;
        //        case 47:
        //            client.Inventory.AddSoul2(stream, client, 114309, 820075, 6, 12, 1, Role.Flags.Gem.SuperPhoenixGem, Role.Flags.Gem.SuperPhoenixGem, false);
        //            break;
        //        case 48:
        //            client.Inventory.AddSoul2(stream, client, 114309, 820076, 6, 12, 1, Role.Flags.Gem.SuperPhoenixGem, Role.Flags.Gem.SuperPhoenixGem, false);
        //            break;
        //        case 11:
        //            data.AddText("Select What u want?")
        //                .AddOption("Headgear Immunity", 49)
        //                .AddOption("Headgear AntiBreak", 50)
        //                .AddAvatar(292)
        //                .FinalizeDialog();
        //            break;
        //        case 49:
        //            client.Inventory.AddSoul2(stream, client, 123309, 820073, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            break;
        //        case 50:
        //            client.Inventory.AddSoul2(stream, client, 123309, 820074, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            break;
        //        case 12:
        //            data.AddText("Select What u want?")
        //                .AddOption("Headgear Immunity", 51)
        //                .AddOption("Headgear AntiBreak", 52)
        //                .AddAvatar(292)
        //                .FinalizeDialog();
        //            break;
        //        case 51:
        //            client.Inventory.AddSoul2(stream, client, 143309, 820073, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            break;
        //        case 52:
        //            client.Inventory.AddSoul2(stream, client, 143309, 820074, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            break;
        //        case 13:
        //            data.AddText("Select What u want?")
        //                .AddOption("Headgear Immunity", 53)
        //                .AddOption("Headgear AntiBreak", 54)
        //                .AddAvatar(292)
        //                .FinalizeDialog();
        //            break;
        //        case 53:
        //            client.Inventory.AddSoul2(stream, client, 144309, 820073, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            break;
        //        case 54:
        //            client.Inventory.AddSoul2(stream, client, 144309, 820074, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            break;
        //        case 14:
        //            data.AddText("Select What u want?")
        //                .AddOption("Headgear Immunity", 55)
        //                .AddOption("Headgear AntiBreak", 56)
        //                .AddAvatar(292)
        //                .FinalizeDialog();
        //            break;
        //        case 55:
        //            client.Inventory.AddSoul2(stream, client, 148309, 820073, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            break;
        //        case 56:
        //            client.Inventory.AddSoul2(stream, client, 148309, 820074, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            break;
        //        case 15:
        //            data.AddText("Select What u want?")
        //                .AddOption("Headgear Immunity", 57)
        //                .AddOption("Headgear AntiBreak", 58)
        //                .AddAvatar(292)
        //                .FinalizeDialog();
        //            break;
        //        case 57:
        //            client.Inventory.AddSoul2(stream, client, 170309, 820073, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            break;
        //        case 58:
        //            client.Inventory.AddSoul2(stream, client, 170309, 820074, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            break;
        //        case 100:
        //            data.AddText("Select What u want?")
        //                .AddOption("Headgear Immunity", 103)
        //                .AddOption("Headgear AntiBreak", 104)
        //                .AddAvatar(292)
        //                .FinalizeDialog();
        //            break;
        //        case 103:
        //            client.Inventory.AddSoul2(stream, client, 113309, 820073, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            break;
        //        case 104:
        //            client.Inventory.AddSoul2(stream, client, 113309, 820074, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            break;
        //        case 16:
        //            client.Inventory.AddSoul2(stream, client, 130309, 822071, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            break;  // Trojan Armor
        //        case 17:
        //            client.Inventory.AddSoul2(stream, client, 131309, 822071, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            break;  // Warrior Armor
        //        case 18:
        //            client.Inventory.AddSoul2(stream, client, 134309, 822072, 6, 12, 1, Role.Flags.Gem.SuperPhoenixGem, Role.Flags.Gem.SuperPhoenixGem, false);
        //            break;    // Taoist Armor
        //        case 19:
        //            client.Inventory.AddSoul2(stream, client, 135309, 822071, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            break;  // Ninja Armor
        //        case 20:
        //            client.Inventory.AddSoul2(stream, client, 136309, 822071, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            break;  // Monk Armor
        //        case 21:
        //            client.Inventory.AddSoul2(stream, client, 139309, 822071, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            break;  // Pirate Armor
        //        case 22:
        //            client.Inventory.AddSoul2(stream, client, 138309, 822071, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            break;  // DW Armor
        //        case 23:
        //            client.Inventory.AddSoul2(stream, client, 101309, 822071, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            break;  // Windwalker Armor
        //        case 101:
        //            client.Inventory.AddSoul2(stream, client, 133309, 822071, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            break; // Archer Armor
        //        case 24:
        //            client.Inventory.AddSoul2(stream, client, 120269, 821033, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            break;  // Necklace
        //        case 25:
        //            client.Inventory.AddSoul2(stream, client, 121269, 821034, 6, 12, 1, Role.Flags.Gem.SuperPhoenixGem, Role.Flags.Gem.SuperPhoenixGem, false);
        //            break;    // Bag
        //        case 26:
        //            data.AddText("Select What u want?")
        //                .AddOption("FangCross - Break", 59)
        //                .AddOption("FangCross - Strike", 60)
        //                .AddOption("Scepter[Trojan]", 89)
        //                .AddOption("Blade", 205)
        //                .AddOption("Sword", 206)
        //                .AddOption("Club", 207)
        //                .AddAvatar(292)
        //                .FinalizeDialog();
        //            break;
        //        case 205:
        //            client.Inventory.AddSoul2(stream, client, 410439, 800111, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false, Role.Flags.ItemEffect.None, false, "");
        //            break;

        //        case 206:
        //            client.Inventory.AddSoul2(stream, client, 420439, 800111, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false, Role.Flags.ItemEffect.None, false, "");
        //            break;
        //        case 89:        
        //                if (!client.Inventory.HaveSpace(1))
        //                {
        //                    client.CreateBoxDialog("Please make 1 more space in your inventory.");
        //                    break;
        //                }
        //                if (client.Player.ConquerPoints >= 1)
        //                {
        //                    client.Player.ConquerPoints -= 1;

        //                    // client.Inventory.AddSoul2(stream, client, 671439, 800811, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false); break; // Monk AlmightyEmperor

        //                    client.Inventory.AddSoul2(stream, client, 481439, 800020, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //                    // 800020
        //                    //client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "zf2-e128" });
        //                }
        //                else
        //                {
        //                    data.Text("Please check your CPS...");

        //                    data.Option("I~see.", 255).FinalizeDialog();
        //                }
        //                break;

        //        case 207:
        //            client.Inventory.AddSoul2(stream, client, 480439, 800111, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false, Role.Flags.ItemEffect.None, false, "");
        //            break;
        //        case 59:
        //            client.Inventory.AddSoul2(stream, client, 614439, 800111, 6, 12, 2, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            break; // Trojan FangCrossSaber
        //        case 60:
        //            client.Inventory.AddSoul2(stream, client, 614439, 800020, 6, 12, 2, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            break; // Trojan FangCrossSaber
        //        case 27:
        //            data.AddText("Select What u want?")
        //                .AddOption("Fist - Break", 61)
        //                .AddOption("Fist - Strike", 62)
        //                .AddOption("Shield", 198)

        //                .AddAvatar(292)
        //                .FinalizeDialog();
        //            break;
        //        case 198:
        //            client.Inventory.AddSoul2(stream, client, 900309, 800422, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);

        //            break;

        //        case 61:
        //            client.Inventory.AddSoul2(stream, client, 624439, 801218, 6, 12, 2, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            break; // Warrior Fist
        //        case 62:
        //            client.Inventory.AddSoul2(stream, client, 624439, 801216, 6, 12, 2, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            break; // Warrior Fist
        //        case 28:
        //            client.Inventory.AddSoul2(stream, client, 620439, 800522, 6, 12, 1, Role.Flags.Gem.SuperPhoenixGem, Role.Flags.Gem.SuperPhoenixGem, false);
        //            // Taoist ImperialBackSword
        //            client.Inventory.AddSoul2(stream, client, 619439, 801104, 6, 12, 1, Role.Flags.Gem.SuperInfinityGem, Role.Flags.Gem.SuperInfinityGem, false);
        //            break; // Taoist UniverseHossu
        //        case 29:
        //            data.AddText("Select What u want?")
        //                .AddOption("SwiftClaw - Break(SkyHammer)", 63)
        //                .AddOption("SwiftClaw - Strike(MonsterSaber)", 64)
        //                .AddOption("SwiftClaw - (ShadowKatana)", 65)
        //                 .AddOption("Scythe - (DemonScythe)", 199)
        //                .AddAvatar(292)
        //                .FinalizeDialog();
        //            break;
        //        case 199:
        //            client.Inventory.AddSoul2(stream, client, 511439, 800255, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            break;
        //        case 63:
        //            client.Inventory.AddSoul2(stream, client, 616439, 800111, 6, 12, 2, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            break; // Ninja SwiftClaw
        //        case 64:
        //            client.Inventory.AddSoul2(stream, client, 616439, 800020, 6, 12, 2, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            break; // Ninja SwiftClaw
        //        case 65:
        //            client.Inventory.AddSoul2(stream, client, 616439, 800142, 6, 12, 2, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            break; // Ninja SwiftClaw
        //        case 102:
        //            data.AddText("Select What u want?")
        //                 .AddOption("EpicArcher", 107)
        //                .AddOption("Knife", 105)
        //                .AddOption("Bow", 106)
        //                .AddAvatar(292)
        //                .FinalizeDialog();
        //            break;
        //        case 105:
        //            client.Inventory.AddSoul2(stream, client, 613429, 800917, 6, 12, 2, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            break;
        //        case 106:
        //            client.Inventory.AddSoul2(stream, client, 500429, 800618, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            break;
        //        case 107:
        //            client.Inventory.AddSoul2(stream, client, 606429, 800633, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            break;
        //        case 30:
        //            client.Inventory.AddSoul2(stream, client, 622439, 800725, 6, 12, 2, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            break; // Monk AlmightyEmperor
        //        case 31:
        //            data.AddText("Select What u want?")
        //                .AddOption("Pistol-Rapier", 176)
        //                .AddOption("Epic", 87)
        //                .AddAvatar(292)
        //                .FinalizeDialog();
        //            break;
        //        case 176:
        //            //Pirate Rapier
        //            client.Inventory.AddSoul2(stream, client, 611439, 800811, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            //Pirate Pistol
        //            client.Inventory.AddSoul2(stream, client, 612439, 800810, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            break;
        //        case 32:
        //            client.Inventory.AddSoul2(stream, client, 617439, 801004, 6, 12, 2, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            break; // DW SkyNunchaku
        //        case 33:
        //            client.Inventory.AddSoul2(stream, client, 626439, 801308, 6, 12, 2, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            break; // Windwalker DragonFan
        //        case 34:
        //            data.AddText("Select What u want?")
        //                .AddOption("Ring - Break", 66)
        //                .AddOption("Ring - Strike", 67)
        //                .AddAvatar(292)
        //                .FinalizeDialog();
        //            break;
        //        case 66:
        //            client.Inventory.AddSoul2(stream, client, 150269, 823059, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            break; // Ring - Break
        //        case 67:
        //            client.Inventory.AddSoul2(stream, client, 150269, 823058, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            break; // Ring - Strike 
        //        case 35:
        //            data.AddText("Select What u want?")
        //                .AddOption("HeavyRing - Break", 68)
        //                .AddOption("HeavyRing - Strike", 69)
        //                .AddAvatar(292)
        //                .FinalizeDialog();
        //            break;
        //        case 68:
        //            client.Inventory.AddSoul2(stream, client, 151269, 823062, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            break; // HeavyRing - Break
        //        case 69:
        //            client.Inventory.AddSoul2(stream, client, 151269, 823061, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            break; // HeavyRing - Strike 
        //        case 36:
        //            client.Inventory.AddSoul2(stream, client, 152279, 823060, 6, 12, 1, Role.Flags.Gem.SuperPhoenixGem, Role.Flags.Gem.SuperPhoenixGem, false);
        //            break;   //Taoist - Bracelet 
        //        case 37:
        //            client.Inventory.AddSoul2(stream, client, 160249, 824018, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            break; //Boot - Strike 
        //        case 70:
        //            client.Inventory.AddSoul2(stream, client, 160249, 824019, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            break; //Boot - Break 
        //        case 38:
        //            client.Inventory.AddSoul2(stream, client, 160249, 824020, 6, 12, 1, Role.Flags.Gem.SuperPhoenixGem, Role.Flags.Gem.SuperPhoenixGem, false);
        //            break;   //Boot - MAttack 


        //        case 254:
        //            data.AddText("Select What u want?")
        //                .AddOption("Headgear Immunity", 250)
        //                .AddOption("Headgear AntiBreak", 249)
        //                .AddAvatar(292)
        //                .FinalizeDialog();
        //            break;
        //        case 87:
        //            if (!client.Inventory.HaveSpace(4))
        //            {
        //                client.CreateBoxDialog("Please make 4 more space in your inventory.");
        //                break;
        //            }
        //            if (client.Player.ConquerPoints >= 1)
        //            {
        //                client.Player.ConquerPoints -= 1;

        //                client.Inventory.AddSoul2(stream, client, 671439, 800811, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //                client.Inventory.AddSoul2(stream, client, 670439, 800810, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);


        //            }
        //            else
        //            {
        //                data.Text("Please pocket 1 CPS..");

        //                data.Option("I~see.", 255).FinalizeDialog();
        //            }
        //            break;
        //        case 250:
        //            client.Inventory.AddSoul2(stream, client, 146309, 820073, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            break;
        //        case 249:
        //            client.Inventory.AddSoul2(stream, client, 146309, 820074, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            break;

        //        case 253:
        //            client.Inventory.AddSoul2(stream, client, 102309, 822071, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            break;

        //        case 252:
        //            client.Inventory.AddSoul2(stream, client, 681439, 827012, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            client.Inventory.AddSoul2(stream, client, 680439, 827013, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
        //            break;

        //    }
        //        #endregion

        //}
        [NpcAttribute(NpcID.GodzillaNPC)]
        public static void GodzillaNPC(Client.GameClient client, ServerSockets.Packet stream, byte Option, String Input, UInt32 id)
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

                        data.AddText("Hello! Through me, you can earn points from wars, events, and powerful monsters, then exchange them for very rare rewards. Are you ready to battle and earn your rewards? \n");
                        data.AddOption("Show (Ideal Runes)", 253);
                        data.AddOption("Show (Rare Sigils Ninja)", 252);
                      //  data.AddOption("Show (Yellow Rare Runes)", 250);
                        data.AddOption("[Informations]", 254);
                        data.AddOption("I~will~come~later.", 255).AddAvatar(3685).FinalizeDialog();
                        break;
                    }
                case 253:
                    {

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;
                        data.AddText("(1) Rare Purple Ideal Rune +9 = 10.000 Godzilla Points \n");
                        data.AddText("You Have [ " + client.Player.GodzillaPoints + " ] Godzilla Points \n");
                        data.AddOption("Supreme Armor (Ideal Rune)", 3);
                        data.AddOption("Indestructible (Ideal Rune)", 1);
                        data.AddOption("Shadow Flow (Ideal Rune)", 2);
                        data.AddOption("Tough Steel (Ideal Rune)", 4);
                        data.AddOption("Unbreakable (Ideal Rune)", 5);
                        data.AddOption("Cosmic Balance (Ideal Rune)", 6);
                        data.AddOption("Invincible Resolve (Ideal Rune)", 7);
                        data.AddOption("Tortoise Predator (Ideal Rune)", 8);
                        data.AddOption("Honed Soul (Ideal Rune)", 9);
                        data.AddOption("Battle Reaper (Ideal Rune)", 10);
                        data.AddOption("XP Devourer (Ideal Rune)", 11);
                        data.AddOption("Deathhold (Ideal Rune)", 12);
                        data.AddOption("Peakcarver (Ideal Rune)", 13);
                        data.AddOption("Soulrazor (Ideal Rune)", 14);
                        data.AddOption("Sanctified Pierce (Ideal Rune)", 15);
                        data.AddOption("I~will~come~later.", 255).AddAvatar(3685).FinalizeDialog();
                        break;
                    }
                case 252:
                    {

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;
                        data.AddText("(1) Rare Sigil Ninja = 10.000 Godzilla Points \n");
                        data.AddText("You Have [ " + client.Player.GodzillaPoints + " ] Godzilla Points \n");
                        data.AddOption("PaperDance (Rare Sigil)", 16);
                        data.AddOption("HellVortex (Rare Sigil)", 17);
                        data.AddOption("InfiniteMist (Rare Sigil)", 18);
                        data.AddOption("Heaven`sWonder (Rare Sigil)", 19);
                        data.AddOption("BonePulse (Rare Sigil)", 20);
                        data.AddOption("I~will~come~later.", 255).AddAvatar(3685).FinalizeDialog();
                        break;
                    }
                case 250:
                    {

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;
                        data.AddText("(1) Yellow Rare Rune = 10.000 Godzilla Points \n");
                        data.AddText("You Have [ " + client.Player.GodzillaPoints + " ] Godzilla Points \n");
                       // data.AddOption("Wild Cleave (+9)", 21);
                      //  data.AddOption("Strike Booster (+9)", 22);
                        data.AddOption("I~will~come~later.", 255).AddAvatar(3685).FinalizeDialog();
                        break;
                    }
                case 254:
                    {

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;

                        data.AddText("1- Supreme Armor (Ideal Rune) For 10.000 Points \n");
                        data.AddText("2- Indestructible Balance (Ideal Rune) For 10.000 Points \n");
                        data.AddText("3- Shadow Flow (Ideal Rune) For 10.000 Points \n");
                        data.AddText("4- Tough Steel (Ideal Rune) For 10.000 Points \n");
                        data.AddText("5- Unbreakable (Ideal Rune) For 10.000 Points \n");
                        data.AddText("6- Cosmic Balance (Ideal Rune) For 10.000 Points \n");
                        data.AddText("7- Invincible Resolve (Ideal Rune) For 10.000 Points \n");
                        data.AddText("8- Tortoise Predator (Ideal Rune) For 10.000 Points \n");
                        data.AddText("9- Honed Soul (Ideal Rune) For 10.000 Points \n");
                        data.AddText("10- Battle Reaper (Ideal Rune) For 10.000 Points \n");
                        data.AddText("11- XP Devourer (Ideal Rune) For 10.000 Points \n");
                        data.AddText("12- Deathhold (Ideal Rune) For 10.000 Points \n");
                        data.AddText("13- Peakcarver (Ideal Rune) For 10.000 Points \n");
                        data.AddText("14- Soulrazor (Ideal Rune) For 10.000 Points \n");
                        data.AddText("15- Sanctified Pierce (Ideal Rune) For 10.000 Points \n");
                        data.AddText("16- Supreme Armor (Ideal Rune) For 10.000 Points \n");
                        data.AddText("17- SigilofPaperDance For 10.000 Points \n");
                        data.AddText("18- SigilofHellVortex For 10.000 Points \n");
                        data.AddText("19- SigilofInfiniteMist For 10.000 Points \n");
                        data.AddText("20- SigilofHeaven`sWonder For 10.000 Points \n");
                        data.AddText("21- SigilofBonePulse For 10.000 Points \n");
                       // data.AddText("22- Strike Booster (+9) For 10.000 Points \n");
                       // data.AddText("23- Wild Cleave (+9) For 10.000 Points \n");
                        data.AddOption("Okay Thanks.", 255).AddAvatar(3685).FinalizeDialog();
                        break;
                    }
                case 1:
                    {

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;

                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in inventory.");
                            data.AddOption("Okay.", 255).FinalizeDialog();
                            break;
                        }
                        if (client.Player.GodzillaPoints >= 10000)
                        {
                            client.Player.GodzillaPoints -= 10000;
                            MsgSchedules.SendSysMesage("Congratulations, [ " + client.Player.Name + " ] Got Indestructible Balance (+9) (Ideal Rune) For 10.000 [Godzilla Points] ", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                            client.Inventory.AddItemWitchStack(4031709, 0, 1, stream, false); // Indestructible Balance (+9)
                        }
                        else
                        {
                            data.AddText("Sorry, But You Do Not Have (10.000) Godzilla Points")
                            .AddOption("Okay thanks.", 255).AddAvatar(3685).FinalizeDialog();
                        }
                        break;
                    }

                case 2:
                    {

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;

                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in inventory.");
                            data.AddOption("Okay.", 255).FinalizeDialog();
                            break;
                        }
                        if (client.Player.GodzillaPoints >= 10000)
                        {
                            client.Player.GodzillaPoints -= 10000;
                            MsgSchedules.SendSysMesage("Congratulations, [ " + client.Player.Name + " ] Got Shadow Flow (+9) (Ideal Rune) For 10.000 [Godzilla Points] ", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                            client.Inventory.AddItemWitchStack(4031809, 0, 1, stream, false); // Shadow Flow (+9)
                        }
                        else
                        {
                            data.AddText("Sorry, But You Do Not Have (10.000) Godzilla Points")
                            .AddOption("Okay thanks.", 255).AddAvatar(3685).FinalizeDialog();
                        }
                        break;
                    }

                case 3:
                    {

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;

                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in inventory.");
                            data.AddOption("Okay.", 255).FinalizeDialog();
                            break;
                        }
                        if (client.Player.GodzillaPoints >= 10000)
                        {
                            client.Player.GodzillaPoints -= 10000;
                            MsgSchedules.SendSysMesage("Congratulations, [ " + client.Player.Name + " ] Got Supreme Armor (+9) (Ideal Rune) For 10.000 [Godzilla Points] ", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                            client.Inventory.AddItemWitchStack(4037909, 0, 1, stream, false); // Supreme Armor (+9)
                        }
                        else
                        {
                            data.AddText("Sorry, But You Do Not Have (10.000) Godzilla Points")
                            .AddOption("Okay thanks.", 255).AddAvatar(3685).FinalizeDialog();
                        }
                        break;
                    }

                case 4:
                    {

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;

                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in inventory.");
                            data.AddOption("Okay.", 255).FinalizeDialog();
                            break;
                        }
                        if (client.Player.GodzillaPoints >= 10000)
                        {
                            client.Player.GodzillaPoints -= 10000;
                            MsgSchedules.SendSysMesage("Congratulations, [ " + client.Player.Name + " ] Got Tough Steel (+9) (Ideal Rune) For 10.000 [Godzilla Points] ", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                            client.Inventory.AddItemWitchStack(4038009, 0, 1, stream, false); // Tough Steel (+9)
                        }
                        else
                        {
                            data.AddText("Sorry, But You Do Not Have (10.000) Godzilla Points")
                            .AddOption("Okay thanks.", 255).AddAvatar(3685).FinalizeDialog();
                        }
                        break;
                    }

                case 5:
                    {

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;

                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in inventory.");
                            data.AddOption("Okay.", 255).FinalizeDialog();
                            break;
                        }
                        if (client.Player.GodzillaPoints >= 10000)
                        {
                            client.Player.GodzillaPoints -= 10000;
                            MsgSchedules.SendSysMesage("Congratulations, [ " + client.Player.Name + " ] Got Unbreakable (+9) (Ideal Rune) For 10.000 [Godzilla Points] ", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                            client.Inventory.AddItemWitchStack(4039109, 0, 1, stream, false); // Unbreakable (+9)
                        }
                        else
                        {
                            data.AddText("Sorry, But You Do Not Have (10.000) Godzilla Points")
                            .AddOption("Okay thanks.", 255).AddAvatar(3685).FinalizeDialog();
                        }
                        break;
                    }

                case 6:
                    {

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;

                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in inventory.");
                            data.AddOption("Okay.", 255).FinalizeDialog();
                            break;
                        }
                        if (client.Player.GodzillaPoints >= 10000)
                        {
                            client.Player.GodzillaPoints -= 10000;
                            MsgSchedules.SendSysMesage("Congratulations, [ " + client.Player.Name + " ] Got Cosmic Balance (+9) (Ideal Rune) For 10.000 [Godzilla Points] ", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                            client.Inventory.AddItemWitchStack(4039509, 0, 1, stream, false); // Cosmic Balance (+9)
                        }
                        else
                        {
                            data.AddText("Sorry, But You Do Not Have (10.000) Godzilla Points")
                            .AddOption("Okay thanks.", 255).AddAvatar(3685).FinalizeDialog();
                        }
                        break;
                    }

                case 7:
                    {

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;

                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in inventory.");
                            data.AddOption("Okay.", 255).FinalizeDialog();
                            break;
                        }
                        if (client.Player.GodzillaPoints >= 10000)
                        {
                            client.Player.GodzillaPoints -= 10000;
                            MsgSchedules.SendSysMesage("Congratulations, [ " + client.Player.Name + " ] Got Invincible Resolve (+9) (Ideal Rune) For 10.000 [Godzilla Points] ", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                            client.Inventory.AddItemWitchStack(4070609, 0, 1, stream, false); // Invincible Resolve (+9)
                        }
                        else
                        {
                            data.AddText("Sorry, But You Do Not Have (10.000) Godzilla Points")
                            .AddOption("Okay thanks.", 255).AddAvatar(3685).FinalizeDialog();
                        }
                        break;
                    }

                case 8:
                    {

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;

                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in inventory.");
                            data.AddOption("Okay.", 255).FinalizeDialog();
                            break;
                        }
                        if (client.Player.GodzillaPoints >= 10000)
                        {
                            client.Player.GodzillaPoints -= 10000;
                            MsgSchedules.SendSysMesage("Congratulations, [ " + client.Player.Name + " ] Got Tortoise Predator (+9) (Ideal Rune) For 10.000 [Godzilla Points] ", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                            client.Inventory.AddItemWitchStack(4070309, 0, 1, stream, false); // Tortoise Predator (+9)
                        }
                        else
                        {
                            data.AddText("Sorry, But You Do Not Have (10.000) Godzilla Points")
                            .AddOption("Okay thanks.", 255).AddAvatar(3685).FinalizeDialog();
                        }
                        break;
                    }

                case 9:
                    {

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;

                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in inventory.");
                            data.AddOption("Okay.", 255).FinalizeDialog();
                            break;
                        }
                        if (client.Player.GodzillaPoints >= 10000)
                        {
                            client.Player.GodzillaPoints -= 10000;
                            MsgSchedules.SendSysMesage("Congratulations, [ " + client.Player.Name + " ] Got Honed Soul (+9) (Ideal Rune) For 10.000 [Godzilla Points] ", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                            client.Inventory.AddItemWitchStack(4072809, 0, 1, stream, false); // Honed Soul (+9)
                        }
                        else
                        {
                            data.AddText("Sorry, But You Do Not Have (10.000) Godzilla Points")
                            .AddOption("Okay thanks.", 255).AddAvatar(3685).FinalizeDialog();
                        }
                        break;
                    }

                case 10:
                    {

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;

                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in inventory.");
                            data.AddOption("Okay.", 255).FinalizeDialog();
                            break;
                        }
                        if (client.Player.GodzillaPoints >= 10000)
                        {
                            client.Player.GodzillaPoints -= 10000;
                            MsgSchedules.SendSysMesage("Congratulations, [ " + client.Player.Name + " ] Got Battle Reaper (+9) (Ideal Rune) For 10.000 [Godzilla Points] ", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                            client.Inventory.AddItemWitchStack(4072609, 0, 1, stream, false); // Battle Reaper (+9)
                        }
                        else
                        {
                            data.AddText("Sorry, But You Do Not Have (10.000) Godzilla Points")
                            .AddOption("Okay thanks.", 255).AddAvatar(3685).FinalizeDialog();
                        }
                        break;
                    }

                case 11:
                    {

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;

                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in inventory.");
                            data.AddOption("Okay.", 255).FinalizeDialog();
                            break;
                        }
                        if (client.Player.GodzillaPoints >= 10000)
                        {
                            client.Player.GodzillaPoints -= 10000;
                            MsgSchedules.SendSysMesage("Congratulations, [ " + client.Player.Name + " ] Got XP Devourer (+9) (Ideal Rune) For 10.000 [Godzilla Points] ", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                            client.Inventory.AddItemWitchStack(4039009, 0, 1, stream, false); // XP Devourer (+9)
                        }
                        else
                        {
                            data.AddText("Sorry, But You Do Not Have (10.000) Godzilla Points")
                            .AddOption("Okay thanks.", 255).AddAvatar(3685).FinalizeDialog();
                        }
                        break;
                    }

                case 12:
                    {

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;

                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in inventory.");
                            data.AddOption("Okay.", 255).FinalizeDialog();
                            break;
                        }
                        if (client.Player.GodzillaPoints >= 10000)
                        {
                            client.Player.GodzillaPoints -= 10000;
                            MsgSchedules.SendSysMesage("Congratulations, [ " + client.Player.Name + " ] Got Deathhold (+9) (Ideal Rune) For 10.000 [Godzilla Points] ", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                            client.Inventory.AddItemWitchStack(4072109, 0, 1, stream, false); // Deathhold (+9)
                        }
                        else
                        {
                            data.AddText("Sorry, But You Do Not Have (10.000) Godzilla Points")
                            .AddOption("Okay thanks.", 255).AddAvatar(3685).FinalizeDialog();
                        }
                        break;
                    }

                case 13:
                    {

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;

                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in inventory.");
                            data.AddOption("Okay.", 255).FinalizeDialog();
                            break;
                        }
                        if (client.Player.GodzillaPoints >= 10000)
                        {
                            client.Player.GodzillaPoints -= 10000;
                            MsgSchedules.SendSysMesage("Congratulations, [ " + client.Player.Name + " ] Got Peakcarver (+9) (Ideal Rune) For 10.000 [Godzilla Points] ", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                            client.Inventory.AddItemWitchStack(4072709, 0, 1, stream, false); // Peakcarver (+9)
                        }
                        else
                        {
                            data.AddText("Sorry, But You Do Not Have (10.000) Godzilla Points")
                            .AddOption("Okay thanks.", 255).AddAvatar(3685).FinalizeDialog();
                        }
                        break;
                    }

                case 14:
                    {

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;

                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in inventory.");
                            data.AddOption("Okay.", 255).FinalizeDialog();
                            break;
                        }
                        if (client.Player.GodzillaPoints >= 10000)
                        {
                            client.Player.GodzillaPoints -= 10000;
                            MsgSchedules.SendSysMesage("Congratulations, [ " + client.Player.Name + " ] Got Soulrazor (+9) (Ideal Rune) For 10.000 [Godzilla Points] ", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                            client.Inventory.AddItemWitchStack(4073309, 0, 1, stream, false); // Soulrazor (+9)
                        }
                        else
                        {
                            data.AddText("Sorry, But You Do Not Have (10.000) Godzilla Points")
                            .AddOption("Okay thanks.", 255).AddAvatar(3685).FinalizeDialog();
                        }
                        break;
                    }

                case 15:
                    {

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;

                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in inventory.");
                            data.AddOption("Okay.", 255).FinalizeDialog();
                            break;
                        }
                        if (client.Player.GodzillaPoints >= 10000)
                        {
                            client.Player.GodzillaPoints -= 10000;
                            MsgSchedules.SendSysMesage("Congratulations, [ " + client.Player.Name + " ] Got Sanctified Pierce (+9) (Ideal Rune) For 10.000 [Godzilla Points] ", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                            client.Inventory.AddItemWitchStack(4073409, 0, 1, stream, false); // Sanctified Pierce (+9)
                        }
                        else
                        {
                            data.AddText("Sorry, But You Do Not Have (10.000) Godzilla Points")
                            .AddOption("Okay thanks.", 255).AddAvatar(3685).FinalizeDialog();
                        }
                        break;
                    }

                case 16:
                    {

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;

                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in inventory.");
                            data.AddOption("Okay.", 255).FinalizeDialog();
                            break;
                        }
                        if (client.Player.GodzillaPoints >= 10000)
                        {
                            client.Player.GodzillaPoints -= 10000;
                            MsgSchedules.SendSysMesage("Congratulations, [ " + client.Player.Name + " ] Got SigilofPaperDance (Rare Sigil) For 10.000 [Godzilla Points] ", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                            client.Inventory.AddItemWitchStack(3330058, 0, 1, stream, false); // SigilofPaperDance
                        }
                        else
                        {
                            data.AddText("Sorry, But You Do Not Have (10.000) Godzilla Points")
                            .AddOption("Okay thanks.", 255).AddAvatar(3685).FinalizeDialog();
                        }
                        break;
                    }

                case 17:
                    {

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;

                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in inventory.");
                            data.AddOption("Okay.", 255).FinalizeDialog();
                            break;
                        }
                        if (client.Player.GodzillaPoints >= 10000)
                        {
                            client.Player.GodzillaPoints -= 10000;
                            MsgSchedules.SendSysMesage("Congratulations, [ " + client.Player.Name + " ] Got SigilofHellVortex (Rare Sigil) For 10.000 [Godzilla Points] ", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                            client.Inventory.AddItemWitchStack(3330059, 0, 1, stream, false); // SigilofHellVortex
                        }
                        else
                        {
                            data.AddText("Sorry, But You Do Not Have (10.000) Godzilla Points")
                            .AddOption("Okay thanks.", 255).AddAvatar(3685).FinalizeDialog();
                        }
                        break;
                    }

                case 18:
                    {

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;

                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in inventory.");
                            data.AddOption("Okay.", 255).FinalizeDialog();
                            break;
                        }
                        if (client.Player.GodzillaPoints >= 10000)
                        {
                            client.Player.GodzillaPoints -= 10000;
                            MsgSchedules.SendSysMesage("Congratulations, [ " + client.Player.Name + " ] Got SigilofInfiniteMist (Rare Sigil) For 10.000 [Godzilla Points] ", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                            client.Inventory.AddItemWitchStack(3330060, 0, 1, stream, false); // SigilofInfiniteMist
                        }
                        else
                        {
                            data.AddText("Sorry, But You Do Not Have (10.000) Godzilla Points")
                            .AddOption("Okay thanks.", 255).AddAvatar(3685).FinalizeDialog();
                        }
                        break;
                    }

                case 19:
                    {

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;

                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in inventory.");
                            data.AddOption("Okay.", 255).FinalizeDialog();
                            break;
                        }
                        if (client.Player.GodzillaPoints >= 10000)
                        {
                            client.Player.GodzillaPoints -= 10000;
                            MsgSchedules.SendSysMesage("Congratulations, [ " + client.Player.Name + " ] Got SigilofHeaven`sWonder (Rare Sigil) For 10.000 [Godzilla Points] ", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                            client.Inventory.AddItemWitchStack(3330061, 0, 1, stream, false); // SigilofHeaven`sWonder
                        }
                        else
                        {
                            data.AddText("Sorry, But You Do Not Have (10.000) Godzilla Points")
                            .AddOption("Okay thanks.", 255).AddAvatar(3685).FinalizeDialog();
                        }
                        break;
                    }

                case 20:
                    {

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;

                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in inventory.");
                            data.AddOption("Okay.", 255).FinalizeDialog();
                            break;
                        }
                        if (client.Player.GodzillaPoints >= 10000)
                        {
                            client.Player.GodzillaPoints -= 10000;
                            MsgSchedules.SendSysMesage("Congratulations, [ " + client.Player.Name + " ] Got SigilofBonePulse (Rare Sigil) For 10.000 [Godzilla Points] ", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                            client.Inventory.AddItemWitchStack(3330062, 0, 1, stream, false); // SigilofBonePulse
                        }
                        else
                        {
                            data.AddText("Sorry, But You Do Not Have (10.000) Godzilla Points")
                            .AddOption("Okay thanks.", 255).AddAvatar(3685).FinalizeDialog();
                        }
                        break;
                    }

                case 21:
                    {

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;

                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in inventory.");
                            data.AddOption("Okay.", 255).FinalizeDialog();
                            break;
                        }
                        if (client.Player.GodzillaPoints >= 10000)
                        {
                            client.Player.GodzillaPoints -= 10000;
                            MsgSchedules.SendSysMesage("Congratulations, [ " + client.Player.Name + " ] Got Wild Cleave (+9) (Yellow Rare Rune) For 10.000 [Godzilla Points] ", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                            client.Inventory.AddItemWitchStack(4037409, 0, 1, stream, false); // Wild Cleave (+9)
                        }
                        else
                        {
                            data.AddText("Sorry, But You Do Not Have (10.000) Godzilla Points")
                            .AddOption("Okay thanks.", 255).AddAvatar(3685).FinalizeDialog();
                        }
                        break;
                    }

                case 22:
                    {

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;

                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in inventory.");
                            data.AddOption("Okay.", 255).FinalizeDialog();
                            break;
                        }
                        if (client.Player.GodzillaPoints >= 10000)
                        {
                            client.Player.GodzillaPoints -= 10000;
                            MsgSchedules.SendSysMesage("Congratulations, [ " + client.Player.Name + " ] Got Strike Booster (+9) (Yellow Rare Rune) For 10.000 [Godzilla Points] ", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                            client.Inventory.AddItemWitchStack(4035009, 0, 1, stream, false); // Strike Booster (+9)
                        }
                        else
                        {
                            data.AddText("Sorry, But You Do Not Have (10.000) Godzilla Points")
                            .AddOption("Okay thanks.", 255).AddAvatar(3685).FinalizeDialog();
                        }
                        break;
                    }
            }
        }
        //[NpcAttribute(NpcID.FullStuff)] // [NpcOfficial 2026-10-14] custom (non-official) NPC removed from the data; handler disabled
        public static void FullStuff(Client.GameClient client, ServerSockets.Packet stream, byte Option, String Input, UInt32 id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    data.AddText("Hello i`m Seller For Free All Select What's Equipment U Want?")
                        .AddOption("Headgear", 1)
                        .AddOption("Armor", 2)
                        .AddOption("Necklace", 3)
                        .AddOption("Weapons", 4)
                        .AddOption("Ring", 5)
                        .AddOption("Boots", 6)
                        .AddOption("Talisman", 7)
                        .AddOption("Give Me Level 140", 251)
                        .AddAvatar(285)
                        .FinalizeDialog();
                    break;
                case 251:
                    {
                        if (client.Player.Level >= 140)
                        {
                            data.AddText("Your level is already higher than or equal to Level 140!")
                                .AddOption("Okay.", 255)
                                .AddAvatar(285)
                                .FinalizeDialog();
                            return;
                        }

                        client.UpdateLevel(stream, 140);

                        data.AddText("Congratulations, You Have Reached Level 140 Successfully.")
                            .AddOption("Thanks.", 255)
                            .AddAvatar(285)
                            .FinalizeDialog();

                        break;
                    }
                case 107:
                    {
                        if (!client.Inventory.HaveSpace(2))
                        {
                            client.CreateBoxDialog("Please make 2 more space in your inventory.");
                            break;
                        }
                        if (client.Player.ConquerPoints >= 1)
                        {
                            client.Player.ConquerPoints -= 1;
                            client.Inventory.AddItemWitchStack(3321098, 0, 1000, stream);
                        }
                        else
                        {
                            data.Text("Please check your CPS...");

                            data.Option("I~see.", 255).FinalizeDialog();
                        }
                        break;
                    }
                case 99:
                    {
                        if (!client.Inventory.HaveSpace(2))
                        {
                            client.CreateBoxDialog("Please make 2 more space in your inventory.");
                            break;
                        }
                        if (client.Player.ConquerPoints >= 1)
                        {
                            client.Player.ConquerPoints -= 1;
                            client.Inventory.AddItemWitchStack(3321098, 0, 100, stream);
                        }
                        else
                        {
                            data.Text("Please check your CPS...");

                            data.Option("I~see.", 255).FinalizeDialog();
                        }
                        break;
                    }
                case 89:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more space in your inventory.");
                            break;
                        }
                        if (client.Player.ConquerPoints >= 1)
                        {
                            client.Player.ConquerPoints -= 1;

                            // client.Inventory.AddSoul(stream, client, 671439, 800811, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false); break; // Monk AlmightyEmperor

                            client.Inventory.AddSoul(stream, client, 481439, 800020, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                            // 800020
                            //client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "zf2-e128" });
                        }
                        else
                        {
                            data.Text("Please check your CPS...");

                            data.Option("I~see.", 255).FinalizeDialog();
                        }
                        break;
                    }
                case 88:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more space in your inventory.");
                            break;
                        }
                        if (client.Player.ConquerPoints >= 1)
                        {
                            client.Player.ConquerPoints -= 1;

                            // client.Inventory.AddSoul(stream, client, 671439, 800811, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false); break; // Monk AlmightyEmperor

                            client.Inventory.AddSoul(stream, client, 670439, 800810, 6, 12, 2, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);

                            //client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "zf2-e128" });
                        }
                        else
                        {
                            data.Text("Please check your CPS...");

                            data.Option("I~see.", 255).FinalizeDialog();
                        }
                        break;
                    }
                case 200:
                    if (!client.Inventory.HaveSpace(4))
                    {
                        client.CreateBoxDialog("Please make 4 more space in your inventory.");
                        break;
                    }
                    if (client.Player.ConquerPoints >= 1)
                    {
                        client.Player.ConquerPoints -= 1;

                        client.Inventory.AddSoul(stream, client, 606429, 800633, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                        client.Inventory.AddSoul(stream, client, 606429, 800633, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    }
                    else
                    {
                        data.Text("Please pocket 1 CPS..");

                        data.Option("I~see.", 255).FinalizeDialog();
                    }
                    break;
                case 87:
                    if (!client.Inventory.HaveSpace(4))
                    {
                        client.CreateBoxDialog("Please make 4 more space in your inventory.");
                        break;
                    }
                    if (client.Player.ConquerPoints >= 1)
                    {
                        client.Player.ConquerPoints -= 1;

                        client.Inventory.AddSoul(stream, client, 671439, 800811, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                        client.Inventory.AddSoul(stream, client, 670439, 800810, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);

                        //client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "zf2-e128" });
                    }
                    else
                    {
                        data.Text("Please pocket 1 CPS..");

                        data.Option("I~see.", 255).FinalizeDialog();
                    }
                    break;
                case 1:
                    data.AddText("Select What's Character u want?")
                        .AddOption("Trojan [Head]", 8)
                        .AddOption("Warrior [Head]", 9)
                        .AddOption("Taoist [Head]", 10)
                        .AddOption("Archer [Head]", 100)
                        .AddOption("Ninja [Head]", 11)
                        .AddOption("Monk [Head]", 12)
                        .AddOption("Pirate [Head]", 13)
                        .AddOption("DragonWarrior [Head]", 14)
                        .AddOption("Windwalker [Head]", 15)
                        .AddOption("Thunderstriker [Head]", 254)
                        .AddOption("Dune Wanderer [Head]", 211)
                        .AddAvatar(292)
                        .FinalizeDialog();
                    break;
                case 211:
                    if (!client.Inventory.HaveSpace(1))
                    {
                        client.CreateBoxDialog("Please make 1 more space in your inventory.");
                        break;
                    }
                    if (client.Player.ConquerPoints >= 1)
                    {
                        client.Player.ConquerPoints -= 1;

                        client.Inventory.AddSoul(stream, client, 171309, 820073, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    }
                    else
                    {
                        data.Text("Please pocket 1 CPS..");

                        data.Option("I~see.", 255).FinalizeDialog();
                    }
                    break;
                case 212:
                    if (!client.Inventory.HaveSpace(1))
                    {
                        client.CreateBoxDialog("Please make 1 more space in your inventory.");
                        break;
                    }
                    if (client.Player.ConquerPoints >= 1)
                    {
                        client.Player.ConquerPoints -= 1;

                        client.Inventory.AddSoul(stream, client, 103309, 822071, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    }
                    else
                    {
                        data.Text("Please pocket 1 CPS..");

                        data.Option("I~see.", 255).FinalizeDialog();
                    }
                    break;
                case 2:
                    data.AddText("Select What's Character u want?")
                        .AddOption("Trojan [Armor]", 16)
                        .AddOption("Warrior [Armor]", 17)
                        .AddOption("Taoist [Armor]", 18)
                        .AddOption("Archer [Armor]", 101)
                        .AddOption("Ninja [Armor]", 19)
                        .AddOption("Monk [Armor]", 20)
                        .AddOption("Pirate [Armor]", 21)
                        .AddOption("DragonWarrior [Armor]", 22)
                        .AddOption("Windwalker [Armor]", 23)
                        .AddOption("Thunderstriker [Armor]", 253)
                        .AddOption("Dune Wanderer [Armor]", 212)
                        .AddAvatar(292)
                        .FinalizeDialog();
                    break;
                case 3:
                    data.AddText("Select What's Character u want?")
                        .AddOption("Necklace", 24)
                        .AddOption("Bag", 25)
                        .AddAvatar(292)
                        .FinalizeDialog();
                    break;
                case 4:
                    data.AddText("Select What's Character u want?")
                        .AddOption("Trojan [Weapons]", 26)
                        .AddOption("Warrior [Weapons]", 27)
                        .AddOption("Taoist [Weapons]", 28)
                        .AddOption("Archer [Weapons]", 102)
                        .AddOption("Ninja [Weapons]", 29)
                        .AddOption("Monk [Weapons]", 30)
                        .AddOption("Pirate [Weapons]", 31)
                        .AddOption("DragonWarrior [Weapons]", 32)
                        .AddOption("Windwalker [Weapons]", 33)
                        .AddOption("Thunderstriker [Weapons]", 252)
                        .AddOption("Archer Epics [Weapons]", 200)
                        .AddOption("Dune Wanderer [Weapons]", 210)
                        .AddAvatar(292)
                        .FinalizeDialog();
                    break;
                case 210:
                    if (!client.Inventory.HaveSpace(4))
                    {
                        client.CreateBoxDialog("Please make 4 more space in your inventory.");
                        break;
                    }
                    if (client.Player.ConquerPoints >= 1)
                    {
                        client.Player.ConquerPoints -= 1;

                        client.Inventory.AddSoul(stream, client, 608439, 801513, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                        client.Inventory.AddSoul(stream, client, 608439, 801513, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    }
                    else
                    {
                        data.Text("Please pocket 1 CPS..");

                        data.Option("I~see.", 255).FinalizeDialog();
                    }
                    break;
                case 5:
                    data.AddText("Select What u want?")
                        .AddOption("Ring", 34)
                        .AddOption("HeavyRing", 35)
                        .AddOption("Braclate", 36)
                        .AddAvatar(292)
                        .FinalizeDialog();
                    break;
                case 6:
                    data.AddText("Select What u want?")
                        .AddOption("Normal - Strike", 37)
                        .AddOption("Normal - Break", 70)
                        .AddOption("Taoist - MAttack", 38)
                        .AddAvatar(292)
                        .FinalizeDialog();
                    break;
                case 7:
                    data.AddText("Select What u want?")
                        .AddOption("HeavenFan", 39)
                        .AddOption("StarTower", 40)
                        .AddOption("RidingCrop", 41)
                        .AddOption("TempestWing", 42)
                        .AddOption("Steed", 170)
                        .AddAvatar(292)
                        .FinalizeDialog();
                    break;
                case 8:
                    data.AddText("Select What u want?")
                        .AddOption("Headgear Immunity", 43)
                        .AddOption("Headgear AntiBreak", 44)
                        .AddAvatar(292)
                        .FinalizeDialog();
                    break;
                case 170:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more space in your inventory.");
                            break;
                        }
                        if (client.Player.ConquerPoints >= 1)
                        {
                            client.Player.ConquerPoints -= 1;
                            client.Inventory.AddSteed1(stream, 300000, 1, 12, false, 255, 0, 150);
                        }
                        else
                        {

                            data.AddText("Please take 1 cps .")
                            .AddOption("Thanks!", 255)
                            .AddAvatar(166).FinalizeDialog();

                        }

                        break;
                    }
                case 43:
                    client.Inventory.AddSoul(stream, client, 118309, 820073, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    break;
                case 44:
                    client.Inventory.AddSoul(stream, client, 118309, 820074, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    break;
                case 9:
                    data.AddText("Select What u want?")
                        .AddOption("Headgear Immunity", 45)
                        .AddOption("Headgear AntiBreak", 46)
                        .AddAvatar(292)
                        .FinalizeDialog();
                    break;
                case 45:
                    client.Inventory.AddSoul(stream, client, 141309, 820073, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    break;
                case 46:
                    client.Inventory.AddSoul(stream, client, 141309, 820074, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    break;
                case 10:
                    data.AddText("Select What u want?")
                        .AddOption("Headgear Immunity", 47)
                        .AddOption("Headgear AntiBreak", 48)
                        .AddAvatar(292)
                        .FinalizeDialog();
                    break;
                case 47:
                    client.Inventory.AddSoul(stream, client, 114309, 820075, 6, 12, 1, Role.Flags.Gem.SuperPhoenixGem, Role.Flags.Gem.SuperPhoenixGem, false);
                    break;
                case 48:
                    client.Inventory.AddSoul(stream, client, 114309, 820076, 6, 12, 1, Role.Flags.Gem.SuperPhoenixGem, Role.Flags.Gem.SuperPhoenixGem, false);
                    break;
                case 11:
                    data.AddText("Select What u want?")
                        .AddOption("Headgear Immunity", 49)
                        .AddOption("Headgear AntiBreak", 50)
                        .AddAvatar(292)
                        .FinalizeDialog();
                    break;
                case 49:
                    client.Inventory.AddSoul(stream, client, 123309, 820073, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    break;
                case 50:
                    client.Inventory.AddSoul(stream, client, 123309, 820074, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    break;
                case 12:
                    data.AddText("Select What u want?")
                        .AddOption("Headgear Immunity", 51)
                        .AddOption("Headgear AntiBreak", 52)
                        .AddAvatar(292)
                        .FinalizeDialog();
                    break;
                case 51:
                    client.Inventory.AddSoul(stream, client, 143309, 820073, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    break;
                case 52:
                    client.Inventory.AddSoul(stream, client, 143309, 820074, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    break;
                case 13:
                    data.AddText("Select What u want?")
                        .AddOption("Headgear Immunity", 53)
                        .AddOption("Headgear AntiBreak", 54)
                        .AddAvatar(292)
                        .FinalizeDialog();
                    break;
                case 53:
                    client.Inventory.AddSoul(stream, client, 144309, 820073, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    break;
                case 54:
                    client.Inventory.AddSoul(stream, client, 144309, 820074, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    break;
                case 14:
                    data.AddText("Select What u want?")
                        .AddOption("Headgear Immunity", 55)
                        .AddOption("Headgear AntiBreak", 56)
                        .AddAvatar(292)
                        .FinalizeDialog();
                    break;
                case 55:
                    client.Inventory.AddSoul(stream, client, 148309, 820073, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    break;
                case 56:
                    client.Inventory.AddSoul(stream, client, 148309, 820074, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    break;
                case 15:
                    data.AddText("Select What u want?")
                        .AddOption("Headgear Immunity", 57)
                        .AddOption("Headgear AntiBreak", 58)
                        .AddAvatar(292)
                        .FinalizeDialog();
                    break;
                case 57:
                    client.Inventory.AddSoul(stream, client, 170309, 820073, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    break;
                case 58:
                    client.Inventory.AddSoul(stream, client, 170309, 820074, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    break;
                case 100:
                    data.AddText("Select What u want?")
                        .AddOption("Headgear Immunity", 103)
                        .AddOption("Headgear AntiBreak", 104)
                        .AddAvatar(292)
                        .FinalizeDialog();
                    break;
                case 103:
                    client.Inventory.AddSoul(stream, client, 113309, 820073, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    break;
                case 104:
                    client.Inventory.AddSoul(stream, client, 113309, 820074, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    break;
                case 16:
                    client.Inventory.AddSoul(stream, client, 130309, 822071, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    break;  // Trojan Armor
                case 17:
                    client.Inventory.AddSoul(stream, client, 131309, 822071, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    break;  // Warrior Armor
                case 18:
                    client.Inventory.AddSoul(stream, client, 134309, 822072, 6, 12, 1, Role.Flags.Gem.SuperPhoenixGem, Role.Flags.Gem.SuperPhoenixGem, false);
                    break;    // Taoist Armor
                case 19:
                    client.Inventory.AddSoul(stream, client, 135309, 822071, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    break;  // Ninja Armor
                case 20:
                    client.Inventory.AddSoul(stream, client, 136309, 822071, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    break;  // Monk Armor
                case 21:
                    client.Inventory.AddSoul(stream, client, 139309, 822071, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    break;  // Pirate Armor
                case 22:
                    client.Inventory.AddSoul(stream, client, 138309, 822071, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    break;  // DW Armor
                case 23:
                    client.Inventory.AddSoul(stream, client, 101309, 822071, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    break;  // Windwalker Armor
                case 101:
                    client.Inventory.AddSoul(stream, client, 133309, 822071, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    break; // Archer Armor
                case 24:
                    client.Inventory.AddSoul(stream, client, 120269, 821033, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    break;  // Necklace
                case 25:
                    client.Inventory.AddSoul(stream, client, 121269, 821034, 6, 12, 1, Role.Flags.Gem.SuperPhoenixGem, Role.Flags.Gem.SuperPhoenixGem, false);
                    break;    // Bag
                case 26:
                    data.AddText("Select What u want?")
                        .AddOption("FangCross - Break", 59)
                        .AddOption("FangCross - Strike", 60)
                        .AddOption("Scepter[Trojan]", 89)
                        .AddOption("Blade", 205)
                        .AddOption("Sword", 206)
                        .AddOption("Club", 207)
                        .AddAvatar(292)
                        .FinalizeDialog();
                    break;
                case 205:
                    client.Inventory.AddSoul(stream, client, 410439, 800111, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false, Role.Flags.ItemEffect.None, false, "");
                    break;

                case 206:
                    client.Inventory.AddSoul(stream, client, 420439, 800111, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false, Role.Flags.ItemEffect.None, false, "");
                    break;

                case 207:
                    client.Inventory.AddSoul(stream, client, 480439, 800111, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false, Role.Flags.ItemEffect.None, false, "");
                    break;
                case 59:
                    client.Inventory.AddSoul(stream, client, 614439, 800111, 6, 12, 2, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    break; // Trojan FangCrossSaber
                case 60:
                    client.Inventory.AddSoul(stream, client, 614439, 800020, 6, 12, 2, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    break; // Trojan FangCrossSaber
                case 27:
                    data.AddText("Select What u want?")
                        .AddOption("Fist - Break", 61)
                        .AddOption("Fist - Strike", 62)
                        .AddOption("Shield", 198)
                        .AddAvatar(292)
                        .FinalizeDialog();
                    break;
                case 198:
                    client.Inventory.AddSoul(stream, client, 900309, 800422, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    client.Inventory.Remove(3008225, 1, stream);
                    break;
                case 61:
                    client.Inventory.AddSoul(stream, client, 624439, 801218, 6, 12, 2, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    break; // Warrior Fist
                case 62:
                    client.Inventory.AddSoul(stream, client, 624439, 801216, 6, 12, 2, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    break; // Warrior Fist
                case 28:
                    client.Inventory.AddSoul(stream, client, 620439, 800522, 6, 12, 1, Role.Flags.Gem.SuperPhoenixGem, Role.Flags.Gem.SuperTortoiseGem, false);
                    // Taoist ImperialBackSword
                    client.Inventory.AddSoul(stream, client, 619439, 801104, 6, 12, 1, Role.Flags.Gem.SuperInfinityGem, Role.Flags.Gem.SuperInfinityGem, false);
                    break; // Taoist UniverseHossu
                case 29:
                    data.AddText("Select What u want?")
                        .AddOption("SwiftClaw - Break(SkyHammer)", 63)
                        .AddOption("SwiftClaw - Strike(MonsterSaber)", 64)
                        .AddOption("SwiftClaw - (ShadowKatana)", 65)
                        .AddOption("SilenceSeythe - (Scythe)", 129)
                        .AddAvatar(292)
                        .FinalizeDialog();
                    break;
                case 63:
                    client.Inventory.AddSoul(stream, client, 616439, 800111, 6, 12, 2, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    break; // Ninja SwiftClaw
                case 64:
                    client.Inventory.AddSoul(stream, client, 616439, 800020, 6, 12, 2, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    break; // Ninja SwiftClaw
                case 65:
                    client.Inventory.AddSoul(stream, client, 616439, 800142, 6, 12, 2, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    break; // Ninja SwiftClaw
                case 129:
                    client.Inventory.AddSoul(stream, client, 511439, 800255, 6, 12, 2, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    break; // Ninja Scythe
                case 102:
                    data.AddText("Select What u want?")
                        .AddOption("Knife", 105)
                        .AddOption("Bow", 106)
                        .AddAvatar(292)
                        .FinalizeDialog();
                    break;
                case 105:
                    client.Inventory.AddSoul(stream, client, 613429, 800917, 6, 12, 2, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    break;
                case 106:
                    client.Inventory.AddSoul(stream, client, 500429, 800618, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    break;
                case 30:
                    client.Inventory.AddSoul(stream, client, 622439, 800725, 6, 12, 2, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    break; // Monk AlmightyEmperor
                case 31:
                    data.AddText("Select What u want?")
                        .AddOption("Pistol-Rapier", 176)
                        .AddOption("Epic", 87)
                        .AddAvatar(292)
                        .FinalizeDialog();
                    break;
                case 176:
                    //Pirate Rapier
                    client.Inventory.AddSoul(stream, client, 611439, 800811, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    //Pirate Pistol
                    client.Inventory.AddSoul(stream, client, 612439, 800810, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    break;
                case 32:
                    client.Inventory.AddSoul(stream, client, 617439, 801004, 6, 12, 2, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    break; // DW SkyNunchaku
                case 33:
                    client.Inventory.AddSoul(stream, client, 626439, 801308, 6, 12, 2, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    break; // Windwalker DragonFan
                case 34:
                    data.AddText("Select What u want?")
                        .AddOption("Ring - Break", 66)
                        .AddOption("Ring - Strike", 67)
                        .AddAvatar(292)
                        .FinalizeDialog();
                    break;
                case 66:
                    client.Inventory.AddSoul(stream, client, 150269, 823059, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    break; // Ring - Break
                case 67:
                    client.Inventory.AddSoul(stream, client, 150269, 823058, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    break; // Ring - Strike 
                case 35:
                    data.AddText("Select What u want?")
                        .AddOption("HeavyRing - Break", 68)
                        .AddOption("HeavyRing - Strike", 69)
                        .AddAvatar(292)
                        .FinalizeDialog();
                    break;
                case 68:
                    client.Inventory.AddSoul(stream, client, 151269, 823062, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    break; // HeavyRing - Break
                case 69:
                    client.Inventory.AddSoul(stream, client, 151269, 823061, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    break; // HeavyRing - Strike 
                case 36:
                    client.Inventory.AddSoul(stream, client, 152279, 823060, 6, 12, 1, Role.Flags.Gem.SuperPhoenixGem, Role.Flags.Gem.SuperPhoenixGem, false);
                    break;   //Taoist - Bracelet 
                case 37:
                    client.Inventory.AddSoul(stream, client, 160249, 824018, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    break; //Boot - Strike 
                case 70:
                    client.Inventory.AddSoul(stream, client, 160249, 824019, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    break; //Boot - Break 
                case 38:
                    client.Inventory.AddSoul(stream, client, 160249, 824020, 6, 12, 1, Role.Flags.Gem.SuperPhoenixGem, Role.Flags.Gem.SuperPhoenixGem, false);
                    break;   //Boot - MAttack 
                case 39:
                    client.Inventory.Add23(stream, 201009, 1, 12, 1, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperThunderGem, false);
                    break; // HeavenFan
                case 40:
                    client.Inventory.Add23(stream, 202009, 1, 12, 1, 0, Role.Flags.Gem.SuperGloryGem, Role.Flags.Gem.SuperGloryGem, false);
                    break; // StarTower
                case 41:
                    client.Inventory.Add23(stream, 203009, 1, 12, 1, 0, 0, 0, false); break;     // RidingCrop
                case 42: client.Inventory.Add23(stream, 204009, 1, 12, 0, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperGloryGem, false);
                    break; // TempestWing

                case 254:
                    data.AddText("Select What u want?")
                        .AddOption("Headgear Immunity", 250)
                        .AddOption("Headgear AntiBreak", 249)
                        .AddAvatar(292)
                        .FinalizeDialog();
                    break;
                case 250:
                    client.Inventory.AddSoul(stream, client, 146309, 820073, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    break;
                case 249:
                    client.Inventory.AddSoul(stream, client, 146309, 820074, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    break;

                case 253:
                    client.Inventory.AddSoul(stream, client, 102309, 822071, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    break;

                case 252:
                    client.Inventory.AddSoul(stream, client, 681439, 827012, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    client.Inventory.AddSoul(stream, client, 680439, 827013, 6, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                    break;
            }
        }

        //[NpcAttribute(NpcID.FullStuff)]
        //public static void FullStuff(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog data = new Dialog(client, stream);
        //    switch (Option)
        //    {
        //        case 0:
        //            data.AddText("Hello i`m Seller Items For Free Select What's Class U Want?");
        //            data.AddOption("Trojan", 1);
        //            data.AddOption("Warrior", 2);
        //            data.AddOption("Dune", 3);
        //            data.AddOption("Archer", 4);
        //            data.AddOption("Ninja", 5);
        //            data.AddOption("Monk", 6);
        //            data.AddOption("Pirate", 7);
        //            data.AddOption("DragonWarrior", 8);
        //            data.AddOption("ThunderStriker", 9);
        //            data.AddOption("Taoist", 10);
        //            data.AddOption("WindWalker", 11);
        //            data.AddAvatar(285);
        //            data.FinalizeDialog();
        //            break;

        //        #region Trojan
        //        case 1:
        //            {
        //                client.Inventory.AddSoul(stream, client, 160249, 824018, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//Boot
        //                client.Inventory.AddSoul(stream, client, 150269, 823058, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//Ring
        //                client.Inventory.AddSoul(stream, client, 120269, 821033, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//Necklace
        //                //client.Inventory.AddSoul(stream, client, 121269, 821034, 7, 12, 1, Role.Flags.Gem.SuperPhoenixGem, Role.Flags.Gem.SuperPhoenixGem, false);//Bag


        //                client.Inventory.Add23(stream, 201009, 1, 12, 1, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperThunderGem, false);//Fan
        //                client.Inventory.Add23(stream, 202009, 1, 12, 1, 0, Role.Flags.Gem.SuperGloryGem, Role.Flags.Gem.SuperGloryGem, false);//Tower
        //                client.Inventory.Add23(stream, 203009, 1, 12, 1, 0, 0, 0, false);// RidingCrop
        //                client.Inventory.Add23(stream, 300000, 1, 12, 0, 0, 0, 0, false);// Steed
        //                client.Inventory.Add23(stream, 204009, 1, 12, 1, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperGloryGem, false);//Wing

        //                client.Inventory.AddSoul(stream, client, 614439, 800020, 7, 12, 2, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//FangCross

        //                client.Inventory.Add(stream, 481439, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Scepter
        //                //client.Inventory.AddSoul(stream, client, 616439, 800111, 7, 12, 2, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);

        //                client.Inventory.AddSoul(stream, client, 118309, 820073, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//HeadGear
        //                client.Inventory.AddSoul(stream, client, 130309, 822071, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//Armor
        //                break;
        //            }
        //        #endregion

        //        #region Warrior
        //        case 2:
        //            {
        //                client.Inventory.AddSoul(stream, client, 160249, 824018, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//Boot
        //                client.Inventory.AddSoul(stream, client, 150269, 823058, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//Ring
        //                client.Inventory.AddSoul(stream, client, 120269, 821033, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//Necklace

        //                client.Inventory.Add23(stream, 201009, 1, 12, 1, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperThunderGem, false);//Fan
        //                client.Inventory.Add23(stream, 202009, 1, 12, 1, 0, Role.Flags.Gem.SuperGloryGem, Role.Flags.Gem.SuperGloryGem, false);//Tower
        //                client.Inventory.Add23(stream, 203009, 1, 12, 1, 0, 0, 0, false);// RidingCrop
        //                client.Inventory.Add23(stream, 300000, 1, 12, 0, 0, 0, 0, false);// Steed
        //                client.Inventory.Add23(stream, 204009, 1, 12, 1, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperGloryGem, false);//Wing

        //                client.Inventory.AddSoul(stream, client, 624439, 801216, 7, 12, 2, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//Fist

        //                client.Inventory.Add(stream, 560439, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Spear
        //                client.Inventory.Add(stream, 900309, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, true);//Shield

        //                client.Inventory.AddSoul(stream, client, 111309, 820073, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//HeadGear
        //                client.Inventory.AddSoul(stream, client, 131309, 822071, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//Armor
        //                break;
        //            }
        //        #endregion

        //        #region Dune
        //        case 3:
        //            {
        //                client.Inventory.AddSoul(stream, client, 160249, 824018, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//Boot
        //                client.Inventory.AddSoul(stream, client, 150269, 823058, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//Ring
        //                client.Inventory.AddSoul(stream, client, 120269, 821033, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//Necklace

        //                client.Inventory.Add23(stream, 201009, 1, 12, 1, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperThunderGem, false);//Fan
        //                client.Inventory.Add23(stream, 202009, 1, 12, 1, 0, Role.Flags.Gem.SuperGloryGem, Role.Flags.Gem.SuperGloryGem, false);//Tower
        //                client.Inventory.Add23(stream, 203009, 1, 12, 1, 0, 0, 0, false);// RidingCrop
        //                client.Inventory.Add23(stream, 300000, 1, 12, 0, 0, 0, 0, false);// Steed
        //                client.Inventory.Add23(stream, 204009, 1, 12, 1, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperGloryGem, false);//Wing

        //                client.Inventory.AddSoul(stream, client, 608439, 801513, 7, 12, 2, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//Fist

        //                client.Inventory.AddSoul(stream, client, 171309, 820073, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//HeadGear
        //                client.Inventory.AddSoul(stream, client, 103309, 822071, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//Armor
        //                break;
        //            }
        //        #endregion

        //        #region Archer
        //        case 4:
        //            {
        //                client.Inventory.AddSoul(stream, client, 160249, 824018, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//Boot
        //                client.Inventory.AddSoul(stream, client, 150269, 823058, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//Ring
        //                client.Inventory.AddSoul(stream, client, 120269, 821033, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//Necklace

        //                client.Inventory.Add23(stream, 201009, 1, 12, 1, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperThunderGem, false);//Fan
        //                client.Inventory.Add23(stream, 202009, 1, 12, 1, 0, Role.Flags.Gem.SuperGloryGem, Role.Flags.Gem.SuperGloryGem, false);//Tower
        //                client.Inventory.Add23(stream, 203009, 1, 12, 1, 0, 0, 0, false);// RidingCrop
        //                client.Inventory.Add23(stream, 300000, 1, 12, 0, 0, 0, 0, false);// Steed
        //                client.Inventory.Add23(stream, 204009, 1, 12, 1, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperGloryGem, false);//Wing

        //                client.Inventory.AddSoul(stream, client, 613429, 800917, 7, 12, 2, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//Knife
        //                client.Inventory.AddSoul(stream, client, 606429, 606423, 7, 12, 2, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//Epic

        //                client.Inventory.AddSoul(stream, client, 113309, 820073, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//HeadGear
        //                client.Inventory.AddSoul(stream, client, 133309, 822071, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//Armor
        //                break;
        //            }
        //        #endregion

        //        #region Ninja
        //        case 5:
        //            {
        //                client.Inventory.AddSoul(stream, client, 160249, 824018, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//Boot
        //                client.Inventory.AddSoul(stream, client, 150269, 823058, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//Ring
        //                client.Inventory.AddSoul(stream, client, 120269, 821033, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//Necklace

        //                client.Inventory.Add23(stream, 201009, 1, 12, 1, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperThunderGem, false);//Fan
        //                client.Inventory.Add23(stream, 202009, 1, 12, 1, 0, Role.Flags.Gem.SuperGloryGem, Role.Flags.Gem.SuperGloryGem, false);//Tower
        //                client.Inventory.Add23(stream, 203009, 1, 12, 1, 0, 0, 0, false);// RidingCrop
        //                client.Inventory.Add23(stream, 300000, 1, 12, 0, 0, 0, 0, false);// Steed
        //                client.Inventory.Add23(stream, 204009, 1, 12, 1, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperGloryGem, false);//Wing

        //                client.Inventory.AddSoul(stream, client, 616439, 800111, 7, 12, 2, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//SwiftClaw

        //                client.Inventory.AddSoul(stream, client, 112309, 820073, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//HeadGear
        //                client.Inventory.AddSoul(stream, client, 135309, 822071, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//Armor
        //                client.Inventory.AddSoul(stream, client, 511439, 800255, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false); // Scythe (2-Handed Weapon)
        //                break;
        //            }
        //        #endregion

        //        #region Monk
        //        case 6:
        //            {
        //                client.Inventory.AddSoul(stream, client, 160249, 824018, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//Boot
        //                client.Inventory.AddSoul(stream, client, 150269, 823058, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//Ring
        //                client.Inventory.AddSoul(stream, client, 120269, 821033, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//Necklace

        //                client.Inventory.Add23(stream, 201009, 1, 12, 1, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperThunderGem, false);//Fan
        //                client.Inventory.Add23(stream, 202009, 1, 12, 1, 0, Role.Flags.Gem.SuperGloryGem, Role.Flags.Gem.SuperGloryGem, false);//Tower
        //                client.Inventory.Add23(stream, 203009, 1, 12, 1, 0, 0, 0, false);// RidingCrop
        //                client.Inventory.Add23(stream, 300000, 1, 12, 0, 0, 0, 0, false);// Steed
        //                client.Inventory.Add23(stream, 204009, 1, 12, 1, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperGloryGem, false);//Wing

        //                client.Inventory.AddSoul(stream, client, 622439, 800725, 7, 12, 2, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//Buddha

        //                client.Inventory.AddSoul(stream, client, 143309, 820073, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//HeadGear
        //                client.Inventory.AddSoul(stream, client, 136309, 822071, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//Armor
        //                break;
        //            }
        //        #endregion

        //        #region Pirate
        //        case 7:
        //            {
        //                client.Inventory.AddSoul(stream, client, 160249, 824018, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//Boot
        //                client.Inventory.AddSoul(stream, client, 150269, 823058, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//Ring
        //                client.Inventory.AddSoul(stream, client, 120269, 821033, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//Necklace

        //                client.Inventory.Add23(stream, 201009, 1, 12, 1, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperThunderGem, false);//Fan
        //                client.Inventory.Add23(stream, 202009, 1, 12, 1, 0, Role.Flags.Gem.SuperGloryGem, Role.Flags.Gem.SuperGloryGem, false);//Tower
        //                client.Inventory.Add23(stream, 203009, 1, 12, 1, 0, 0, 0, false);// RidingCrop
        //                client.Inventory.Add23(stream, 300000, 1, 12, 0, 0, 0, 0, false);// Steed
        //                client.Inventory.Add23(stream, 204009, 1, 12, 1, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperGloryGem, false);//Wing

        //                client.Inventory.AddSoul(stream, client, 671439, 800811, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//Rapier
        //                client.Inventory.AddSoul(stream, client, 670439, 800810, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//Pistol

        //                client.Inventory.AddSoul(stream, client, 144309, 820073, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//HeadGear
        //                client.Inventory.AddSoul(stream, client, 139309, 822071, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//Armor
        //                break;
        //            }
        //        #endregion

        //        #region DragonWarrior
        //        case 8:
        //            {
        //                client.Inventory.AddSoul(stream, client, 160249, 824018, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//Boot
        //                client.Inventory.AddSoul(stream, client, 150269, 823058, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//Ring
        //                client.Inventory.AddSoul(stream, client, 120269, 821033, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//Necklace

        //                client.Inventory.Add23(stream, 201009, 1, 12, 1, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperThunderGem, false);//Fan
        //                client.Inventory.Add23(stream, 202009, 1, 12, 1, 0, Role.Flags.Gem.SuperGloryGem, Role.Flags.Gem.SuperGloryGem, false);//Tower
        //                client.Inventory.Add23(stream, 203009, 1, 12, 1, 0, 0, 0, false);// RidingCrop
        //                client.Inventory.Add23(stream, 300000, 1, 12, 0, 0, 0, 0, false);// Steed
        //                client.Inventory.Add23(stream, 204009, 1, 12, 1, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperGloryGem, false);//Wing

        //                client.Inventory.AddSoul(stream, client, 617439, 801004, 7, 12, 2, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//Nunchaku

        //                client.Inventory.AddSoul(stream, client, 144309, 820073, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//HeadGear
        //                client.Inventory.AddSoul(stream, client, 138309, 822071, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//Armor
        //                break;
        //            }
        //        #endregion

        //        #region ThunderStriker
        //        case 9:
        //            {
        //                client.Inventory.AddSoul(stream, client, 160249, 824018, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//Boot
        //                client.Inventory.AddSoul(stream, client, 150269, 823058, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//Ring
        //                client.Inventory.AddSoul(stream, client, 120269, 821033, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//Necklace

        //                client.Inventory.Add23(stream, 201009, 1, 12, 1, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperThunderGem, false);//Fan
        //                client.Inventory.Add23(stream, 202009, 1, 12, 1, 0, Role.Flags.Gem.SuperGloryGem, Role.Flags.Gem.SuperGloryGem, false);//Tower
        //                client.Inventory.Add23(stream, 203009, 1, 12, 1, 0, 0, 0, false);// RidingCrop
        //                client.Inventory.Add23(stream, 300000, 1, 12, 0, 0, 0, 0, false);// Steed
        //                client.Inventory.Add23(stream, 204009, 1, 12, 1, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperGloryGem, false);//Wing

        //                client.Inventory.AddSoul(stream, client, 681439, 0, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//W
        //                client.Inventory.AddSoul(stream, client, 680439, 0, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//W

        //                client.Inventory.AddSoul(stream, client, 146309, 820073, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//HeadGear
        //                client.Inventory.AddSoul(stream, client, 102309, 822071, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//Armor
        //                break;
        //            }
        //        #endregion

        //        #region Taoist
        //        case 10:
        //            {
        //                client.Inventory.AddSoul(stream, client, 160249, 824018, 7, 12, 1, Role.Flags.Gem.SuperPhoenixGem, Role.Flags.Gem.SuperPhoenixGem, false);//Boot
        //                client.Inventory.AddSoul(stream, client, 150269, 823058, 7, 12, 1, Role.Flags.Gem.SuperPhoenixGem, Role.Flags.Gem.SuperPhoenixGem, false);//Ring
        //                client.Inventory.AddSoul(stream, client, 121269, 821034, 7, 12, 1, Role.Flags.Gem.SuperPhoenixGem, Role.Flags.Gem.SuperPhoenixGem, false);

        //                client.Inventory.Add23(stream, 201009, 1, 12, 1, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperThunderGem, false);//Fan
        //                client.Inventory.Add23(stream, 202009, 1, 12, 1, 0, Role.Flags.Gem.SuperGloryGem, Role.Flags.Gem.SuperGloryGem, false);//Tower
        //                client.Inventory.Add23(stream, 203009, 1, 12, 1, 0, 0, 0, false);// RidingCrop
        //                client.Inventory.Add23(stream, 300000, 1, 12, 0, 0, 0, 0, false);// Steed
        //                client.Inventory.Add23(stream, 204009, 1, 12, 1, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperGloryGem, false);//Wing

        //                client.Inventory.AddSoul(stream, client, 620439, 800522, 7, 12, 1, Role.Flags.Gem.SuperPhoenixGem, Role.Flags.Gem.SuperPhoenixGem, false);//Weapon
        //                client.Inventory.AddSoul(stream, client, 619439, 801104, 0, 12, 1, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false);//Hossu

        //                client.Inventory.AddSoul(stream, client, 114309, 820073, 7, 12, 1, Role.Flags.Gem.SuperPhoenixGem, Role.Flags.Gem.SuperPhoenixGem, false);//HeadGear
        //                client.Inventory.AddSoul(stream, client, 134309, 822071, 7, 12, 1, Role.Flags.Gem.SuperPhoenixGem, Role.Flags.Gem.SuperPhoenixGem, false);//Armor
        //                break;
        //            }
        //        #endregion

        //        #region WindWalker
        //        case 11:
        //            {
        //                client.Inventory.AddSoul(stream, client, 160249, 824018, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//Boot
        //                client.Inventory.AddSoul(stream, client, 150269, 823058, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//Ring
        //                client.Inventory.AddSoul(stream, client, 120269, 821033, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//Necklace

        //                client.Inventory.Add23(stream, 201009, 1, 12, 1, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperThunderGem, false);//Fan
        //                client.Inventory.Add23(stream, 202009, 1, 12, 1, 0, Role.Flags.Gem.SuperGloryGem, Role.Flags.Gem.SuperGloryGem, false);//Tower
        //                client.Inventory.Add23(stream, 203009, 1, 12, 1, 0, 0, 0, false);// RidingCrop
        //                client.Inventory.Add23(stream, 300000, 1, 12, 0, 0, 0, 0, false);// Steed
        //                client.Inventory.Add23(stream, 204009, 1, 12, 1, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperGloryGem, false);//Wing

        //                client.Inventory.AddSoul(stream, client, 626439, 801308, 7, 12, 2, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//Weapon

        //                client.Inventory.AddSoul(stream, client, 170309, 820073, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//HeadGear
        //                client.Inventory.AddSoul(stream, client, 101309, 822071, 7, 12, 1, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);//Armor
        //                break;
        //            }
        //        #endregion
        //    }
        //}

        //[NpcAttribute(NpcID.NPCLevel130)] // [NpcOfficial 2026-10-14] custom (non-official) NPC removed from the data; handler disabled
        public static void NPCLevel130(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello, I can raise your level to Level 130 for free. Do you want to raise your level now?")
                            .AddOption("Yes.", 1)
                            .AddOption("No.", 255)
                            .AddAvatar(28)
                            .FinalizeDialog();
                        break;
                    }

                case 1:
                    {
                        if (client.Player.Level >= 130)
                        {
                            data.AddText("Your level is already higher than or equal to Level 130!")
                                .AddOption("Okay.", 255)
                                .AddAvatar(28)
                                .FinalizeDialog();
                            return;
                        }

                        client.UpdateLevel(stream, 130);

                        data.AddText("Congratulations, You Have Reached Level 130 Successfully.")
                            .AddOption("Thanks.", 255)
                            .AddAvatar(28)
                            .FinalizeDialog();

                        break;
                    }
            }
        }

        //[NpcAttribute(NpcID.NewChi)]
        //public static void NewChi(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog a = new Dialog(client, stream);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                a.AddText("Wich One Do You Want Your Chi Fixed With NightMareFoooo2 .");
        //                a.AddOption("[HP]+[Attack]+[Immy]+[Strike].", 1);
        //                a.AddOption("[HP]+[Attack]+[Immy]+[Break].", 2);
        //                a.AddOption("[HP]+[M-Attack]+[Immy]+[M-Strike].", 3);
        //                a.AddOption("[HP]+[P-Damge]+[AntiBreak]+[Immy].", 4);
        //                a.AddOption("NeverMind .", 255);
        //                a.AddAvatar(63).FinalizeDialog();
        //                break;
        //            }
        //        case 1:
        //            {
        //                if (client.Player.ConquerPoints >= 1)
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



        //                    client.CreateBoxDialog("You've Chi~Strike.");
        //                }
        //                else
        //                {
        //                    a.AddText("You can not use this option for You've Chi~Strike .")
        //                        .AddOption("I see.", 255)
        //                          .AddAvatar(264)
        //                        .FinalizeDialog();
        //                }


        //                break;
        //            }
        //        case 2:
        //            {
        //                if (client.Player.ConquerPoints >= 1)
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
        //                    client.CreateBoxDialog("You've Chi~Strike.");
        //                }
        //                else
        //                {
        //                    a.AddText("You can not use this option for You've Chi~Break .")
        //                        .AddOption("I see.", 255)
        //                          .AddAvatar(264)
        //                        .FinalizeDialog();
        //                }


        //                break;
        //            }
        //        case 3:
        //            {
        //                if (client.Player.ConquerPoints >= 1)
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
        //                    client.CreateBoxDialog("You've Chi~Strike.");
        //                }
        //                else
        //                {
        //                    a.AddText("You can not use this option for You've Chi~Fire .")
        //                        .AddOption("I see.", 255)
        //                          .AddAvatar(264)
        //                        .FinalizeDialog();
        //                }


        //                break;
        //            }
        //        case 4:
        //            {
        //                if (client.Player.ConquerPoints >= 1)
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
        //                    client.CreateBoxDialog("You've Chi~Strike.");
        //                }
        //                else
        //                {
        //                    a.AddText("You can not use this option for You've Chi~Defense .")
        //                        .AddOption("I see.", 255)
        //                          .AddAvatar(264)
        //                        .FinalizeDialog();
        //                }


        //                break;



        //            }
        //    }
        //}
        [NpcAttribute(NpcID.ArchivesNinja)]
        public static void ArchivesNinja(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.MyNinja.Valid() && client.MyNinja.Unlocked) return;
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.Text("STR_ID_tNinjaQuest[25850][Text711]@@");
                        data.Text("STR_ID_tNinjaQuest[25850][Text712]@@");
                        data.Text("STR_ID_tNinjaQuest[25850][Text713]@@");
                        // [OfficialFix 2026-10-14] Option711 is the official "OK." of a story line; it used to unlock the Ninja
                        // Archive and all 16 Eight-Gates flags for free. Officially the Archive opens at 4301 and each gate at
                        // its Ninja Awakening quest (4306/4323/4327/4330/4333/4338/4368/4369, QuestEngine prizeExtra).
                        data.Option("STR_ID_tNinjaQuest[25850][Option711]@@", 255);
                        data.Avatar(2886);
                        data.FinalizeDialog();
                        break;
                    }


            }
        }
        [NpcAttribute(NpcID.ClassSkillTeleporter)]
        public static void ClassSkillTeleporter(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog obj = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {
                        obj.AddText("Class Skill Teleporter");
                        obj.AddText("Every Class has distinctive skills. If you forget a skill after your 1st rebirth and want to regain it, I can send you to see your Class tutor.");
                        if (client.Player.Reborn < 2)
                        {
                            obj.Option("I~haven`t~been~2nd-reborn~yet.", 1);
                        }
                        else
                        {
                            if (Database.AtributesStatus.IsNinja(client.Player.Class))
                            {
                                obj.Option("Regain~Ninja`s~skill.", 140);


                            }
                            if (Database.AtributesStatus.IsTrojan(client.Player.Class))
                            {
                                obj.Option("Regain~Trojan`s~skill.", 140);

                            }
                            if (Database.AtributesStatus.IsPirate(client.Player.Class))
                            {
                                obj.Option("Regain~Pirate`s~skill.", 140);

                            }
                            if (Database.AtributesStatus.IsMonk(client.Player.Class))
                            {
                                obj.Option("Regain~Monk/Saint`s~skill.", 140);

                            }
                            if (Database.AtributesStatus.IsThunderStriker(client.Player.Class))
                            {
                                obj.Option("Regain~Thunderstriker`s~skill.", 140);

                            }
                            if (Database.AtributesStatus.IsWater(client.Player.FirstClass) || Database.AtributesStatus.IsWater(client.Player.SecoundeClass))
                            {

                                obj.Option("Regain~Taoist`s~skill.", 150);

                            }
                            if (Database.AtributesStatus.IsArcher(client.Player.Class))
                            {
                                obj.Option("Regain~Archer`s~skill.", 140);

                            }
                            if (Database.AtributesStatus.IsFire(client.Player.Class))
                            {
                                obj.Option("Regain~Taoist`s~skill.", 140);

                            }
                            if (Database.AtributesStatus.IsWarrior(client.Player.Class))
                            {
                                obj.Option("Regain~Warrior`s~skill.", 140);

                            }
                            if (Database.AtributesStatus.IsLee(client.Player.Class))
                            {
                                obj.Option("Regain~Dragon~Warrior`s~skill.", 140);
                            }
                            if (Database.AtributesStatus.IsWindWalker(client.Player.Class))
                            {
                                obj.Option("Regain~Windwalker`s~skill.", 140);


                            }
                        }
                        obj.AddAvatar(47);
                        obj.FinalizeDialog();
                        break;


                    }
                case 150:
                    {
                        obj.AddText("Class Skill Teleporter");
                        obj.AddText("Every Class has distinctive skills. If you forget a skill after your 1st rebirth and want to regain it, I can send you to see your Class tutor.");
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Stigma))
                            obj.Option("Stigma.", 151);
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MagicShield))
                            obj.Option("MagicShield.", 152);
                        obj.AddAvatar(47);
                        obj.FinalizeDialog();
                        break;
                    }
                case 151:
                    {
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Stigma))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Stigma, 1);

                        break;
                    }
                case 152:
                    {
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MagicShield))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.MagicShield, 1);

                        break;
                    }
                case 140:
                    {
                        obj.AddText("Class Skill Teleporter");
                        obj.AddText("Every Class has distinctive skills. If you forget a skill after your 1st rebirth and want to regain it, I can send you to see your Class tutor.");
                        var Spell = client.MySpells.ClientSpells.Values.ToArray();
                        for (int x = 0; x < Spell.Length; x++)
                        {
                            if (Pool.Magic.ContainsKey(Spell[x].ID))
                                obj.Option("" + Pool.Magic[Spell[x].ID][Spell[x].Level].Name + "", (byte)x);
                        }
                        obj.AddAvatar(47);
                        obj.FinalizeDialog();
                        break;
                    }

                default:
                    {
                        var Spell = client.MySpells.ClientSpells.Values.ToArray();
                        for (int x = 0; x < Spell.Length; x++)
                        {
                            if (x == Option)
                            {
                                client.MySpells.Remove(Spell[x].ID, stream);
                                Game.MsgServer.MsgSpell my_spell = new MsgSpell();
                                my_spell.ID = Spell[x].ID;
                                my_spell.Level = Spell[x].Level;
                                my_spell.Experience = 0;
                                my_spell.SoulLevel = 0;
                                my_spell.PreviousLevel = 0;
                                my_spell.Experience = 0;
                                client.MySpells.RecoverSkill.Add(Spell[x].ID, my_spell);
                            }
                        }
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.NewPromotion2)] // New NPC Promotion
        public static void NewPromotion2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.Text("if you want to a job promotion in the Dragland, i can send you there.");
                        dialog.Option("Please send me there.", 1);
                        dialog.AddAvatar(116).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (Database.AtributesStatus.IsNinja(client.Player.Class))
                        {
                            client.Teleport(70, 78, 10445);// Ninja

                        }
                        if (Database.AtributesStatus.IsTrojan(client.Player.Class))
                        {
                            client.Teleport(70, 78, 10446);// Trojan
                        }
                        if (Database.AtributesStatus.IsPirate(client.Player.Class))
                        {
                            client.Teleport(70, 78, 10447);//Pirate
                        }
                        if (Database.AtributesStatus.IsMonk(client.Player.Class))
                        {
                            client.Teleport(70, 78, 10448);// Monk
                        }
                        if (Database.AtributesStatus.IsThunderStriker(client.Player.Class))
                        {
                            client.Teleport(70, 78, 10449);//ThunderStrike
                        }
                        if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                        {
                            client.Teleport(70, 78, 10450);//Taiost
                        }
                        if (Database.AtributesStatus.IsArcher(client.Player.Class))
                        {
                            client.Teleport(70, 78, 10451);//Archer
                        }
                        if (Database.AtributesStatus.IsFire(client.Player.Class))
                        {
                            client.Teleport(70, 78, 10450);//Fire
                        }
                        if (Database.AtributesStatus.IsWarrior(client.Player.Class))
                        {
                            client.Teleport(70, 78, 10452);//warrior

                        }
                        if (Database.AtributesStatus.IsLee(client.Player.Class))
                        {
                            client.Teleport(70, 78, 10453);//LeeLong
                        }
                        if (Database.AtributesStatus.IsWindWalker(client.Player.Class))
                        {
                            client.Teleport(70, 78, 10454);//wind

                        }
                        if (Database.AtributesStatus.IsDune(client.Player.Class))
                        {
                            client.Teleport(70, 78, 10455);//DuneWanderer

                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Star3GarmenMount)]
        public static void Star3GarmenMount(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        dialog.Text("STR_ID_tCoatWarehouse_Sale[18997][Text121]@@");
                        dialog.Text("STR_ID_tCoatWarehouse_Sale[18997][Text122]@@");
                        dialog.Text("STR_ID_tCoatWarehouse_Sale[18997][Text123]@@");
                        dialog.Text("STR_ID_tCoatWarehouse_Sale[18997][Text124]@@");
                        dialog.Text("STR_ID_tCoatWarehouse_Sale[18997][Text125]@@");
                        dialog.AddOption("STR_ID_tCoatWarehouse_Sale[18997][Option121]@@", 1);
                        dialog.AddOption("STR_ID_tCoatWarehouse_Sale[18997][Option122]@@", 3);
                        dialog.AddAvatar(124).FinalizeDialog();
                        break;
                    }

                case 1:
                    {
                        dialog.Text("STR_ID_tCoatWarehouse_Sale[18997][Text124]@@");
                        dialog.AddOption("STR_ID_tCoatWarehouse_Sale[18997][Option231]@@", 2);
                        dialog.AddAvatar(124).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        dialog.Text("STR_ID_tCoatWarehouse_Sale[18997][Text124]@@");
                        dialog.AddOption("STR_ID_tCoatWarehouse_Sale[18997][Option231]@@", 4);
                        dialog.AddAvatar(124).FinalizeDialog();
                        break;
                    }


                case 2:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Contain(3008223, 1))
                            {

                                client.Inventory.Remove(3008223, 1, stream);

                                uint[] O = {
#region Item
                                           200622,200621,200563,187565,193115,200574,  200525,  192165,  192495, 
200453,  200539, 
200494,  200500, 
200543,  200589, 
200590,  200591, 
200593,  200595, 
200475,  195345, 
195335,  192515, 
195325,  188185, 
195315,  192465, 
195305,  187775, 
195295,  187415, 
192425,  193205, 192745,  192745, 
192135,  194385, 
192615,  194405, 
192635,  194695, 
192695,  194705, 
187325,  194715, 
192785,  194725, 
192625,  189645, 
188675,  195205, 
192645,  195195, 
188495,  195185, 
193295,  195175, 
192605,  189685, 
188265,  193725, 
193695,  194885, 
193555,  194895, 
193775,  194905, 
193785,  194915, 
193795,  194925, 
193805,  194935, 
193805,  194945, 
192665,  193325 ,
200403 ,
200407 ,
200409 ,
200410 ,
200411 ,
200412 ,
200413 ,
200414 ,
200415 ,
200416 ,
200417 ,
200418 ,
200419 ,
200420 ,
200421 ,
200425 ,
200426 ,
200427 ,
200428 ,
200431 ,
200433 ,
200437 ,
200438 ,
200439 ,
200440 ,
200441 ,
200442 ,
200443 ,
200444 ,
200445 ,
200446 ,
200447 ,
200449 ,
200453 ,
200454 ,
200455 ,
200456 ,
200458 ,
200459 ,
200461 ,
200463 ,
200464 ,
200465 ,
200466 ,
200467 ,
200471 ,
200474 ,
200475 ,
200476 ,
200477 ,
200478 ,
200479 ,
200480 ,
200481 ,
200482 ,
200485 ,
200486 ,
200487 ,
200488 ,
200490 ,
200491 ,
200492 ,
200493 ,
200494 ,
200495 ,
200499 ,
200500 ,
200503 ,
200514 ,
200515 ,
200516 ,
200517 ,
200524 ,
200525 ,
200527 ,
200528 ,
200529 ,
200530 ,
200531 ,
200532 ,
200533 ,
200537 ,
200538 ,
200539 ,
200540 ,
200541 ,
200542 ,
200543 ,
200544 ,
200547 ,
200549 ,
200553 ,
200554 ,
200555 ,
200556 ,
200557 ,
200558 ,
200559 ,
200560 ,
200562 ,
200563 ,
200564 ,
200570 ,
200571 ,
200572 ,
200573 ,
200574 ,
200575 ,
200579 ,
200580 ,
200584 ,
200585 ,
200586 ,
200587 ,
200588 ,
200589 ,
200590 ,
200591 ,
200592 ,
200593 ,
200595 ,
200596 ,
200601 ,
200606 ,
200613 ,
200614 ,
200618 ,
200619 ,
200620 ,
200621 ,
200622 ,
200623 ,
200624 ,
200625 ,
200626 ,
200629 ,
200637 ,
200638 ,
200639 ,
200640 ,
200641 ,
200642 ,
200643 ,
200644 ,
200645 ,
200646 ,
200647};// ID Garmant &&Mount
#endregion
                                int index = (int)Pool.GetRandom.Next(0, O.Length);
                                uint ItemId = O[index];
                                var item = Pool.ItemsBase[ItemId];
                                client.Inventory.AddTime(stream, (uint)ItemId, 0, 0, 0, false, 7);


                            }
                            else
                            {

                                dialog.AddText("Dont~Have~IvoryJade.")
                                .AddOption("Thanks", 255)
                                .AddAvatar(166).FinalizeDialog();

                            }

                        }
                        else
                        {

                            dialog.AddText("Dont~Have~HaveSpace.")
                            .AddOption("Thanks", 255)
                            .AddAvatar(166).FinalizeDialog();

                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.HaveSpace(3))
                        {
                            if (client.Inventory.Contain(3008223, 3))
                            {

                                client.Inventory.Remove(3008223, 3, stream);
                                for (int x = 0; x < 3; x++)
                                {
                                    uint[] O = new uint[]
                                #region Item
                       {

                                               200622,200621,200563,187565,193115,200574,  200525,  192165,  192495, 
200453,  200539, 
200494,  200500, 
200543,  200589, 
200590,  200591, 
200593,  200595, 
200475,  195345, 
195335,  192515, 
195325,  188185, 
195315,  192465, 
195305,  187775, 
195295,  187415, 
192425,  193205, 192745,  192745, 
192135,  194385, 
192615,  194405, 
192635,  194695, 
192695,  194705, 
187325,  194715, 
192785,  194725, 
192625,  189645, 
188675,  195205, 
192645,  195195, 
188495,  195185, 
193295,  195175, 
192605,  189685, 
188265,  193725, 
193695,  194885, 
193555,  194895, 
193775,  194905, 
193785,  194915, 
193795,  194925, 
193805,  194935, 
193805,  194945, 
192665,  193325 ,
200403 ,
200407 ,
200409 ,
200410 ,
200411 ,
200412 ,
200413 ,
200414 ,
200415 ,
200416 ,
200417 ,
200418 ,
200419 ,
200420 ,
200421 ,
200425 ,
200426 ,
200427 ,
200428 ,
200431 ,
200433 ,
200437 ,
200438 ,
200439 ,
200440 ,
200441 ,
200442 ,
200443 ,
200444 ,
200445 ,
200446 ,
200447 ,
200449 ,
200453 ,
200454 ,
200455 ,
200456 ,
200458 ,
200459 ,
200461 ,
200463 ,
200464 ,
200465 ,
200466 ,
200467 ,
200471 ,
200474 ,
200475 ,
200476 ,
200477 ,
200478 ,
200479 ,
200480 ,
200481 ,
200482 ,
200485 ,
200486 ,
200487 ,
200488 ,
200490 ,
200491 ,
200492 ,
200493 ,
200494 ,
200495 ,
200499 ,
200500 ,
200503 ,
200514 ,
200515 ,
200516 ,
200517 ,
200524 ,
200525 ,
200527 ,
200528 ,
200529 ,
200530 ,
200531 ,
200532 ,
200533 ,
200537 ,
200538 ,
200539 ,
200540 ,
200541 ,
200542 ,
200543 ,
200544 ,
200547 ,
200549 ,
200553 ,
200554 ,
200555 ,
200556 ,
200557 ,
200558 ,
200559 ,
200560 ,
200562 ,
200563 ,
200564 ,
200570 ,
200571 ,
200572 ,
200573 ,
200574 ,
200575 ,
200579 ,
200580 ,
200584 ,
200585 ,
200586 ,
200587 ,
200588 ,
200589 ,
200590 ,
200591 ,
200592 ,
200593 ,
200595 ,
200596 ,
200601 ,
200606 ,
200613 ,
200614 ,
200618 ,
200619 ,
200620 ,
200621 ,
200622 ,
200623 ,
200624 ,
200625 ,
200626 ,
200629 ,
200637 ,
200638 ,
200639 ,
200640 ,
200641 ,
200642 ,
200643 ,
200644 ,
200645 ,
200646 ,
200647};// ID Garmant &&Mount
                                #endregion
                                    int index = (int)Pool.GetRandom.Next(0, O.Length);
                                    uint ItemId = O[index];
                                    var item = Pool.ItemsBase[ItemId];
                                    client.Inventory.AddTime(stream, (uint)ItemId, 0, 0, 0, false, 7);

                                }
                            }
                            else
                            {

                                dialog.AddText("Dont~Have~IvoryJade.")
                                .AddOption("Thanks", 255)
                                .AddAvatar(166).FinalizeDialog();

                            }
                        }
                        else
                        {

                            dialog.AddText("Dont~Have~HaveSpace.")
                            .AddOption("Thanks", 255)
                            .AddAvatar(166).FinalizeDialog();

                        }
                        break;
                    }





            }
        }
        [NpcAttribute(NpcID.Star5GarmenMount)]
        public static void Star5GarmenMount(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.Text("STR_ID_tCoatWarehouse_Sale[18998][Text121]@@");
                        dialog.Text("STR_ID_tCoatWarehouse_Sale[18998][Text122]@@");
                        dialog.Text("STR_ID_tCoatWarehouse_Sale[18998][Text123]@@");
                        dialog.Text("STR_ID_tCoatWarehouse_Sale[18998][Text124]@@");
                        dialog.Text("STR_ID_tCoatWarehouse_Sale[18998][Text125]@@");
                        dialog.AddOption("STR_ID_tCoatWarehouse_Sale[ItemName][2][2]@@", 1);
                        dialog.AddOption("3 Bloody Jade", 3);
                        dialog.AddAvatar(124).FinalizeDialog();
                        break;
                    }

                case 1:
                    {
                        dialog.Text("STR_ID_tCoatWarehouse_Sale[18996][Text348]@@");
                        dialog.AddOption("STR_ID_tCoatWarehouse_Sale[18996][Option341]@@", 2);
                        dialog.AddAvatar(124).FinalizeDialog();
                        break;
                    }
                case 3:
                    {

                        dialog.Text("STR_ID_tCoatWarehouse_Sale[18996][Text348]@@");
                        dialog.AddOption("STR_ID_tCoatWarehouse_Sale[18996][Option341]@@", 4);
                        dialog.AddAvatar(124).FinalizeDialog();
                        break;
                    }

                case 2:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Contain(3008225, 1))
                            {

                                client.Inventory.Remove(3008225, 1, stream);

                                uint[] O = {
                            #region item 
                                           187405, 
188915 ,
189065  ,
189135 ,
189675,
189695 ,
193275,
193445,
193525 ,
193565 ,
193625 ,
194395 ,
194875,
195015,
195045,
195225,
195365,
195405,
195425,
195485,
195575,
195605,
195625,
195645,
200021,
200022,
200220,
200221,
200410,
200428,
200449,
200482,
200494,
200517,
200524,
200531,
200532,
200533,
200537,
200540 ,
200544 ,
200559 ,
200560 ,
200564 ,
200572 ,
200573 ,
200575,
200580,
200592,
200596,
200601,
200606,
200613,
200614,
200623,
200624,
200626,
200629,
200637,
200638,
200639,
200640,
200644,
200646,
200647};// ID Garmant &&Mount
                            #endregion
                                int index = (int)Pool.GetRandom.Next(0, O.Length);
                                uint ItemId = O[index];
                                var item = Pool.ItemsBase[ItemId];
                                client.Inventory.AddTime(stream, (uint)ItemId, 0, 0, 0, false, 7);

                            }
                            else
                            {

                                dialog.AddText("Dont~Have~BloodyJade.")
                                .AddOption("Thanks", 255)
                                .AddAvatar(166).FinalizeDialog();

                            }
                        }
                        else
                        {

                            dialog.AddText("Dont~Have~HaveSpace.")
                            .AddOption("Thanks", 255)
                            .AddAvatar(166).FinalizeDialog();

                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.HaveSpace(3))
                        {
                            if (client.Inventory.Contain(3008225, 3))
                            {

                                client.Inventory.Remove(3008225, 3, stream);
                                for (int x = 0; x < 3; x++)
                                {
                                    uint[] O = {
                            #region item 
                                           187405, 
188915 ,
189065  ,
189135 ,
189675,
189695 ,
193275,
193445,
193525 ,
193565 ,
193625 ,
194395 ,
194875,
195015,
195045,
195225,
195365,
195405,
195425,
195485,
195575,
195605,
195625,
195645,
200021,
200022,
200220,
200221,
200410,
200428,
200449,
200482,
200494,
200517,
200524,
200531,
200532,
200533,
200537,
200540 ,
200544 ,
200559 ,
200560 ,
200564 ,
200572 ,
200573 ,
200575,
200580,
200592,
200596,
200601,
200606,
200613,
200614,
200623,
200624,
200626,
200629,
200637,
200638,
200639,
200640,
200644,
200646,
200647};// ID Garmant &&Mount
                            #endregion
                                    int index = (int)Pool.GetRandom.Next(0, O.Length);
                                    uint ItemId = O[index];
                                    var item = Pool.ItemsBase[ItemId];
                                    client.Inventory.AddTime(stream, (uint)ItemId, 0, 0, 0, false, 7);
                                }

                            }
                            else
                            {

                                dialog.AddText("Dont~Have~BloodyJade.")
                                .AddOption("Thanks", 255)
                                .AddAvatar(166).FinalizeDialog();

                            }
                        }
                        else
                        {

                            dialog.AddText("Dont~Have~HaveSpace.")
                            .AddOption("Thanks", 255)
                            .AddAvatar(166).FinalizeDialog();

                        }
                        break;
                    }

            }
        }


        //[NpcAttribute(NpcID.BankCps)]
        //public static void BankCps(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog data = new Dialog(client, stream);
        //    switch (Option)
        //    {

        //        case 0:
        //            {
        //                if (client.InTrade) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™ÂÃ˜Â§Ã˜ÂªÃ˜Â­ Ã˜ÂªÃ˜Â±Ã™Å Ã˜Â¯ Ã™â€¦Ã˜Â¹ Ã˜Â´Ã˜Â®Ã˜Âµ Ã˜ÂªÃ˜Â§Ã™â€ Ã™Å 
        //                {
        //                    client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
        //                    return;
        //                }
        //                if (client.IsVendor) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™â€šÃ˜Â§Ã˜Â¹Ã˜Â¯ Ã™ÂÃ™Å  Ã˜Â´Ã™Ë†Ã˜Â¨ Ã˜Â§Ã™â€žÃ™â€¦Ã˜Â±Ã™Æ’Ã˜Âª 
        //                {
        //                    client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
        //                    return;
        //                }
        //                data.AddText("Hello, Through Me You Can Deposit Or Withdraw Your CPs At Any Time\n");
        //                data.AddText("All Your CPs In The Bank [ " + client.Player.CpsBank + " ] ");
        //                data.AddOption("I Want Withdraw [CPS].", 50);
        //                data.AddOption("I Want Save [CPS].", 1);
        //                data.AddOption("I`ll talk to you later.", 255).AddAvatar(3221).FinalizeDialog();
        //                break;
        //            }
        //        #region Save
        //        case 1:
        //            {
        //                if (client.InTrade) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™ÂÃ˜Â§Ã˜ÂªÃ˜Â­ Ã˜ÂªÃ˜Â±Ã™Å Ã˜Â¯ Ã™â€¦Ã˜Â¹ Ã˜Â´Ã˜Â®Ã˜Âµ Ã˜ÂªÃ˜Â§Ã™â€ Ã™Å 
        //                {
        //                    client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
        //                    return;
        //                }
        //                if (client.IsVendor) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™â€šÃ˜Â§Ã˜Â¹Ã˜Â¯ Ã™ÂÃ™Å  Ã˜Â´Ã™Ë†Ã˜Â¨ Ã˜Â§Ã™â€žÃ™â€¦Ã˜Â±Ã™Æ’Ã˜Âª 
        //                {
        //                    client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
        //                    return;
        //                }
        //                data.AddText("(Choose)");
        //                data.AddOption("[ 10,000,000 ] Cps .", 2);
        //                data.AddOption("[ 25,000,000 ] Cps .", 4);
        //                data.AddOption("[ 50,000,000 ] Cps .", 6);
        //                data.AddOption("[ 100,000,000 ] Cps .", 8);
        //                data.AddOption("[ 500,000,000 ] Cps .", 10);
        //                data.AddOption("[ 1,000,000,000 ] Cps .", 100);
        //                data.AddOption("[ 2,000,000,000 ] Cps .", 101);
        //                // data.AddOption("[ 3,000,000,000 ] Cps .", 110);
        //                //  data.AddOption("[ 4,000,000,000 ] Cps .", 111);
        //                data.AddOption("I`ll talk to you later.", 255).AddAvatar(3221).FinalizeDialog();
        //                break;
        //            }
        //        #region 10M
        //        case 2:
        //            {
        //                if (client.InTrade) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™ÂÃ˜Â§Ã˜ÂªÃ˜Â­ Ã˜ÂªÃ˜Â±Ã™Å Ã˜Â¯ Ã™â€¦Ã˜Â¹ Ã˜Â´Ã˜Â®Ã˜Âµ Ã˜ÂªÃ˜Â§Ã™â€ Ã™Å 
        //                {
        //                    client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
        //                    return;
        //                }
        //                if (client.IsVendor) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™â€šÃ˜Â§Ã˜Â¹Ã˜Â¯ Ã™ÂÃ™Å  Ã˜Â´Ã™Ë†Ã˜Â¨ Ã˜Â§Ã™â€žÃ™â€¦Ã˜Â±Ã™Æ’Ã˜Âª 
        //                {
        //                    client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
        //                    return;
        //                }
        //                if (client.Player.ConquerPoints >= 10000000)
        //                {
        //                    client.Player.CpsBank += 10000000;
        //                    client.Player.ConquerPoints -= 10000000;
        //                    data.AddText("You Have Add 10.000.000 CPs In ( Bank Cps ) ");
        //                    data.AddOption("Thanks. ", 255).AddAvatar(0).FinalizeDialog();
        //                }
        //                else
        //                {
        //                    data.AddText("Check Your Inventory You Don`t Have CPs !! ");
        //                    data.AddOption("Okay. ", 255).AddAvatar(0).FinalizeDialog();
        //                    break;
        //                }
        //                break;
        //            }
        //        #endregion
        //        #region 25M
        //        case 4:
        //            {
        //                if (client.InTrade) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™ÂÃ˜Â§Ã˜ÂªÃ˜Â­ Ã˜ÂªÃ˜Â±Ã™Å Ã˜Â¯ Ã™â€¦Ã˜Â¹ Ã˜Â´Ã˜Â®Ã˜Âµ Ã˜ÂªÃ˜Â§Ã™â€ Ã™Å 
        //                {
        //                    client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
        //                    return;
        //                }
        //                if (client.IsVendor) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™â€šÃ˜Â§Ã˜Â¹Ã˜Â¯ Ã™ÂÃ™Å  Ã˜Â´Ã™Ë†Ã˜Â¨ Ã˜Â§Ã™â€žÃ™â€¦Ã˜Â±Ã™Æ’Ã˜Âª 
        //                {
        //                    client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
        //                    return;
        //                }
        //                if (client.Player.ConquerPoints >= 25000000)
        //                {
        //                    client.Player.CpsBank += 25000000;
        //                    client.Player.ConquerPoints -= 25000000;
        //                    data.AddText("You Have Add 250.000.000 CPs In ( Bank Cps ) ");
        //                    data.AddOption("Thanks. ", 255).AddAvatar(0).FinalizeDialog();
        //                }
        //                else
        //                {
        //                    data.AddText("Check Your Inventory You Don`t Have CPs !! ");
        //                    data.AddOption("Okay. ", 255).AddAvatar(0).FinalizeDialog();
        //                    break;
        //                }
        //                break;
        //            }
        //        #endregion
        //        #region 50M
        //        case 6:
        //            {
        //                if (client.InTrade) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™ÂÃ˜Â§Ã˜ÂªÃ˜Â­ Ã˜ÂªÃ˜Â±Ã™Å Ã˜Â¯ Ã™â€¦Ã˜Â¹ Ã˜Â´Ã˜Â®Ã˜Âµ Ã˜ÂªÃ˜Â§Ã™â€ Ã™Å 
        //                {
        //                    client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
        //                    return;
        //                }
        //                if (client.IsVendor) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™â€šÃ˜Â§Ã˜Â¹Ã˜Â¯ Ã™ÂÃ™Å  Ã˜Â´Ã™Ë†Ã˜Â¨ Ã˜Â§Ã™â€žÃ™â€¦Ã˜Â±Ã™Æ’Ã˜Âª 
        //                {
        //                    client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
        //                    return;
        //                }
        //                if (client.Player.ConquerPoints >= 50000000)
        //                {
        //                    client.Player.CpsBank += 50000000;
        //                    client.Player.ConquerPoints -= 50000000;
        //                    data.AddText("You Have Add 50.000.000 CPs In ( Bank Cps ) ");
        //                    data.AddOption("Thanks. ", 255).AddAvatar(0).FinalizeDialog();
        //                }
        //                else
        //                {
        //                    data.AddText("Check Your Inventory You Don`t Have CPs !! ");
        //                    data.AddOption("Okay. ", 255).AddAvatar(0).FinalizeDialog();
        //                    break;
        //                }
        //                break;
        //            }
        //        #endregion
        //        #region 100M
        //        case 8:
        //            {
        //                if (client.InTrade) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™ÂÃ˜Â§Ã˜ÂªÃ˜Â­ Ã˜ÂªÃ˜Â±Ã™Å Ã˜Â¯ Ã™â€¦Ã˜Â¹ Ã˜Â´Ã˜Â®Ã˜Âµ Ã˜ÂªÃ˜Â§Ã™â€ Ã™Å 
        //                {
        //                    client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
        //                    return;
        //                }
        //                if (client.IsVendor) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™â€šÃ˜Â§Ã˜Â¹Ã˜Â¯ Ã™ÂÃ™Å  Ã˜Â´Ã™Ë†Ã˜Â¨ Ã˜Â§Ã™â€žÃ™â€¦Ã˜Â±Ã™Æ’Ã˜Âª 
        //                {
        //                    client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
        //                    return;
        //                }
        //                if (client.Player.ConquerPoints >= 100000000)
        //                {
        //                    client.Player.CpsBank += 100000000;
        //                    client.Player.ConquerPoints -= 100000000;
        //                    data.AddText("You Have Add 100.000.000 CPs In ( Bank Cps ) ");
        //                    data.AddOption("Thanks. ", 255).AddAvatar(0).FinalizeDialog();
        //                }
        //                else
        //                {
        //                    data.AddText("Check Your Inventory You Don`t Have CPs !! ");
        //                    data.AddOption("Okay. ", 255).AddAvatar(0).FinalizeDialog();
        //                    break;
        //                }
        //                break;
        //            }
        //        #endregion
        //        #region 500M
        //        case 10:
        //            {
        //                if (client.InTrade) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™ÂÃ˜Â§Ã˜ÂªÃ˜Â­ Ã˜ÂªÃ˜Â±Ã™Å Ã˜Â¯ Ã™â€¦Ã˜Â¹ Ã˜Â´Ã˜Â®Ã˜Âµ Ã˜ÂªÃ˜Â§Ã™â€ Ã™Å 
        //                {
        //                    client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
        //                    return;
        //                }
        //                if (client.IsVendor) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™â€šÃ˜Â§Ã˜Â¹Ã˜Â¯ Ã™ÂÃ™Å  Ã˜Â´Ã™Ë†Ã˜Â¨ Ã˜Â§Ã™â€žÃ™â€¦Ã˜Â±Ã™Æ’Ã˜Âª 
        //                {
        //                    client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
        //                    return;
        //                }
        //                if (client.Player.ConquerPoints >= 500000000)
        //                {
        //                    client.Player.CpsBank += 500000000;
        //                    client.Player.ConquerPoints -= 500000000;
        //                    data.AddText("You Have Add 500.000.000 CPs In ( Bank Cps ) ");
        //                    data.AddOption("Thanks. ", 255).AddAvatar(0).FinalizeDialog();
        //                }
        //                else
        //                {
        //                    data.AddText("Check Your Inventory You Don`t Have CPs !! ");
        //                    data.AddOption("Okay. ", 255).AddAvatar(0).FinalizeDialog();
        //                    break;
        //                }
        //                break;
        //            }
        //        #endregion
        //        #region 1b
        //        case 100:
        //            {
        //                if (client.InTrade) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™ÂÃ˜Â§Ã˜ÂªÃ˜Â­ Ã˜ÂªÃ˜Â±Ã™Å Ã˜Â¯ Ã™â€¦Ã˜Â¹ Ã˜Â´Ã˜Â®Ã˜Âµ Ã˜ÂªÃ˜Â§Ã™â€ Ã™Å 
        //                {
        //                    client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
        //                    return;
        //                }
        //                if (client.IsVendor) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™â€šÃ˜Â§Ã˜Â¹Ã˜Â¯ Ã™ÂÃ™Å  Ã˜Â´Ã™Ë†Ã˜Â¨ Ã˜Â§Ã™â€žÃ™â€¦Ã˜Â±Ã™Æ’Ã˜Âª 
        //                {
        //                    client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
        //                    return;
        //                }
        //                if (client.Player.ConquerPoints >= 1000000000)
        //                {
        //                    client.Player.CpsBank += 1000000000;
        //                    client.Player.ConquerPoints -= 1000000000;
        //                    data.AddText("You Have Add 1.000.000.000 CPs In ( Bank Cps ) ");
        //                    data.AddOption("Thanks. ", 255).AddAvatar(0).FinalizeDialog();
        //                }
        //                else
        //                {
        //                    data.AddText("Check Your Inventory You Don`t Have CPs !! ");
        //                    data.AddOption("Okay. ", 255).AddAvatar(0).FinalizeDialog();
        //                    break;
        //                }
        //                break;
        //            }
        //        #endregion
        //        #region 2b
        //        case 101:
        //            {
        //                if (client.InTrade) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™ÂÃ˜Â§Ã˜ÂªÃ˜Â­ Ã˜ÂªÃ˜Â±Ã™Å Ã˜Â¯ Ã™â€¦Ã˜Â¹ Ã˜Â´Ã˜Â®Ã˜Âµ Ã˜ÂªÃ˜Â§Ã™â€ Ã™Å 
        //                {
        //                    client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
        //                    return;
        //                }
        //                if (client.IsVendor) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™â€šÃ˜Â§Ã˜Â¹Ã˜Â¯ Ã™ÂÃ™Å  Ã˜Â´Ã™Ë†Ã˜Â¨ Ã˜Â§Ã™â€žÃ™â€¦Ã˜Â±Ã™Æ’Ã˜Âª 
        //                {
        //                    client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
        //                    return;
        //                }
        //                if (client.Player.ConquerPoints >= 2000000000)
        //                {
        //                    client.Player.CpsBank += 2000000000;
        //                    client.Player.ConquerPoints -= 2000000000;
        //                    data.AddText("You Have Add 2.000.000.000 CPs In ( Bank Cps ) ");
        //                    data.AddOption("Thanks. ", 255).AddAvatar(0).FinalizeDialog();
        //                }
        //                else
        //                {
        //                    data.AddText("Check Your Inventory You Don`t Have CPs !! ");
        //                    data.AddOption("Okay. ", 255).AddAvatar(0).FinalizeDialog();
        //                    break;
        //                }
        //                break;
        //            }
        //        #endregion
        //        #region 3b
        //        case 110:
        //            {
        //                if (client.InTrade) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™ÂÃ˜Â§Ã˜ÂªÃ˜Â­ Ã˜ÂªÃ˜Â±Ã™Å Ã˜Â¯ Ã™â€¦Ã˜Â¹ Ã˜Â´Ã˜Â®Ã˜Âµ Ã˜ÂªÃ˜Â§Ã™â€ Ã™Å 
        //                {
        //                    client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
        //                    return;
        //                }
        //                if (client.IsVendor) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™â€šÃ˜Â§Ã˜Â¹Ã˜Â¯ Ã™ÂÃ™Å  Ã˜Â´Ã™Ë†Ã˜Â¨ Ã˜Â§Ã™â€žÃ™â€¦Ã˜Â±Ã™Æ’Ã˜Âª 
        //                {
        //                    client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
        //                    return;
        //                }
        //                if (client.Player.ConquerPoints >= 3000000000)
        //                {
        //                    client.Player.CpsBank += 3000000000;
        //                    client.Player.ConquerPoints -= 3000000000;
        //                    data.AddText("You Have Add 3.000.000.000 CPs In ( Bank Cps ) ");
        //                    data.AddOption("Thanks. ", 255).AddAvatar(0).FinalizeDialog();
        //                }
        //                else
        //                {
        //                    data.AddText("Check Your Inventory You Don`t Have CPs !! ");
        //                    data.AddOption("Okay. ", 255).AddAvatar(0).FinalizeDialog();
        //                    break;
        //                }
        //                break;
        //            }
        //        #endregion
        //        #region 4b
        //        case 111:
        //            {
        //                if (client.InTrade) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™ÂÃ˜Â§Ã˜ÂªÃ˜Â­ Ã˜ÂªÃ˜Â±Ã™Å Ã˜Â¯ Ã™â€¦Ã˜Â¹ Ã˜Â´Ã˜Â®Ã˜Âµ Ã˜ÂªÃ˜Â§Ã™â€ Ã™Å 
        //                {
        //                    client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
        //                    return;
        //                }
        //                if (client.IsVendor) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™â€šÃ˜Â§Ã˜Â¹Ã˜Â¯ Ã™ÂÃ™Å  Ã˜Â´Ã™Ë†Ã˜Â¨ Ã˜Â§Ã™â€žÃ™â€¦Ã˜Â±Ã™Æ’Ã˜Âª 
        //                {
        //                    client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
        //                    return;
        //                }
        //                if (client.Player.ConquerPoints >= 4000000000)
        //                {
        //                    client.Player.CpsBank += 4000000000;
        //                    client.Player.ConquerPoints -= 4000000000;
        //                    data.AddText("You Have Add 4.000.000.000 CPs In ( Bank Cps ) ");
        //                    data.AddOption("Thanks. ", 255).AddAvatar(0).FinalizeDialog();
        //                }
        //                else
        //                {
        //                    data.AddText("Check Your Inventory You Don`t Have CPs !! ");
        //                    data.AddOption("Okay. ", 255).AddAvatar(0).FinalizeDialog();
        //                    break;
        //                }
        //                break;
        //            }
        //        #endregion
        //        #endregion
        //        #region Withdraw
        //        case 50:
        //            {
        //                if (client.InTrade) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™ÂÃ˜Â§Ã˜ÂªÃ˜Â­ Ã˜ÂªÃ˜Â±Ã™Å Ã˜Â¯ Ã™â€¦Ã˜Â¹ Ã˜Â´Ã˜Â®Ã˜Âµ Ã˜ÂªÃ˜Â§Ã™â€ Ã™Å 
        //                {
        //                    client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
        //                    return;
        //                }
        //                if (client.IsVendor) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™â€šÃ˜Â§Ã˜Â¹Ã˜Â¯ Ã™ÂÃ™Å  Ã˜Â´Ã™Ë†Ã˜Â¨ Ã˜Â§Ã™â€žÃ™â€¦Ã˜Â±Ã™Æ’Ã˜Âª 
        //                {
        //                    client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
        //                    return;
        //                }
        //                data.AddText("(Choose)");
        //                data.AddOption("[ 10,000,000 ] Cps .", 52);
        //                data.AddOption("[ 25,000,000 ] Cps .", 54);
        //                data.AddOption("[ 50,000,000 ] Cps .", 56);
        //                data.AddOption("[ 100,000,000 ] Cps .", 58);
        //                data.AddOption("[ 500,000,000 ] Cps .", 60);
        //                data.AddOption("[ 1,000,000,000 ] Cps .", 103);
        //                data.AddOption("[ 2,000,000,000 ] Cps .", 104);
        //                // data.AddOption("[ 3,000,000,000 ] Cps .", 115);
        //                // data.AddOption("[ 4,000,000,000 ] Cps .", 116);
        //                data.AddOption("I`ll talk to you later.", 255).AddAvatar(3221).FinalizeDialog();

        //                break;
        //            }
        //        #region 10M
        //        case 52:
        //            {
        //                if (client.InTrade) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™ÂÃ˜Â§Ã˜ÂªÃ˜Â­ Ã˜ÂªÃ˜Â±Ã™Å Ã˜Â¯ Ã™â€¦Ã˜Â¹ Ã˜Â´Ã˜Â®Ã˜Âµ Ã˜ÂªÃ˜Â§Ã™â€ Ã™Å 
        //                {
        //                    client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
        //                    return;
        //                }
        //                if (client.IsVendor) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™â€šÃ˜Â§Ã˜Â¹Ã˜Â¯ Ã™ÂÃ™Å  Ã˜Â´Ã™Ë†Ã˜Â¨ Ã˜Â§Ã™â€žÃ™â€¦Ã˜Â±Ã™Æ’Ã˜Âª 
        //                {
        //                    client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
        //                    return;
        //                }
        //                if (client.Player.CpsBank >= 10000000)
        //                {
        //                    client.Player.ConquerPoints += 10000000;
        //                    client.Player.CpsBank -= 10000000;
        //                    data.AddText("You Have Withdraw 10.000.000 CPs In ( Bank Cps ) ");
        //                    data.AddOption("Thanks. ", 255).AddAvatar(0).FinalizeDialog();
        //                }
        //                else
        //                {
        //                    data.AddText("Check Your Inventory You Don`t Have CPs !! ");
        //                    data.AddOption("Okay. ", 255).AddAvatar(0).FinalizeDialog();
        //                    break;
        //                }
        //                break;
        //            }
        //        #endregion
        //        #region 25M
        //        case 54:
        //            {
        //                if (client.InTrade) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™ÂÃ˜Â§Ã˜ÂªÃ˜Â­ Ã˜ÂªÃ˜Â±Ã™Å Ã˜Â¯ Ã™â€¦Ã˜Â¹ Ã˜Â´Ã˜Â®Ã˜Âµ Ã˜ÂªÃ˜Â§Ã™â€ Ã™Å 
        //                {
        //                    client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
        //                    return;
        //                }
        //                if (client.IsVendor) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™â€šÃ˜Â§Ã˜Â¹Ã˜Â¯ Ã™ÂÃ™Å  Ã˜Â´Ã™Ë†Ã˜Â¨ Ã˜Â§Ã™â€žÃ™â€¦Ã˜Â±Ã™Æ’Ã˜Âª 
        //                {
        //                    client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
        //                    return;
        //                }
        //                if (client.Player.CpsBank >= 25000000)
        //                {
        //                    client.Player.ConquerPoints += 25000000;
        //                    client.Player.CpsBank -= 25000000;
        //                    data.AddText("You Have Add 25.000.000 CPs In ( Bank Cps ) ");
        //                    data.AddOption("Thanks. ", 255).AddAvatar(0).FinalizeDialog();
        //                }
        //                else
        //                {
        //                    data.AddText("Check Your Inventory You Don`t Have CPs !! ");
        //                    data.AddOption("Okay. ", 255).AddAvatar(0).FinalizeDialog();
        //                    break;
        //                }
        //                break;
        //            }
        //        #endregion
        //        #region 50M
        //        case 56:
        //            {
        //                if (client.InTrade) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™ÂÃ˜Â§Ã˜ÂªÃ˜Â­ Ã˜ÂªÃ˜Â±Ã™Å Ã˜Â¯ Ã™â€¦Ã˜Â¹ Ã˜Â´Ã˜Â®Ã˜Âµ Ã˜ÂªÃ˜Â§Ã™â€ Ã™Å 
        //                {
        //                    client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
        //                    return;
        //                }
        //                if (client.IsVendor) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™â€šÃ˜Â§Ã˜Â¹Ã˜Â¯ Ã™ÂÃ™Å  Ã˜Â´Ã™Ë†Ã˜Â¨ Ã˜Â§Ã™â€žÃ™â€¦Ã˜Â±Ã™Æ’Ã˜Âª 
        //                {
        //                    client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
        //                    return;
        //                }
        //                if (client.Player.CpsBank >= 50000000)
        //                {
        //                    client.Player.ConquerPoints += 50000000;
        //                    client.Player.CpsBank -= 50000000;
        //                    data.AddText("You Have Add 50.000.000 CPs In ( Bank Cps ) ");
        //                    data.AddOption("Thanks. ", 255).AddAvatar(0).FinalizeDialog();
        //                }
        //                else
        //                {
        //                    data.AddText("Check Your Inventory You Don`t Have CPs !! ");
        //                    data.AddOption("Okay. ", 255).AddAvatar(0).FinalizeDialog();
        //                    break;
        //                }
        //                break;
        //            }
        //        #endregion
        //        #region 100M
        //        case 58:
        //            {
        //                if (client.InTrade) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™ÂÃ˜Â§Ã˜ÂªÃ˜Â­ Ã˜ÂªÃ˜Â±Ã™Å Ã˜Â¯ Ã™â€¦Ã˜Â¹ Ã˜Â´Ã˜Â®Ã˜Âµ Ã˜ÂªÃ˜Â§Ã™â€ Ã™Å 
        //                {
        //                    client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
        //                    return;
        //                }
        //                if (client.IsVendor) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™â€šÃ˜Â§Ã˜Â¹Ã˜Â¯ Ã™ÂÃ™Å  Ã˜Â´Ã™Ë†Ã˜Â¨ Ã˜Â§Ã™â€žÃ™â€¦Ã˜Â±Ã™Æ’Ã˜Âª 
        //                {
        //                    client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
        //                    return;
        //                }
        //                if (client.Player.CpsBank >= 100000000)
        //                {
        //                    client.Player.ConquerPoints += 100000000;
        //                    client.Player.CpsBank -= 100000000;
        //                    data.AddText("You Have Withdraw 100.000.000 CPs In ( Bank Cps ) ");
        //                    data.AddOption("Thanks. ", 255).AddAvatar(0).FinalizeDialog();
        //                }
        //                else
        //                {
        //                    data.AddText("Check Your Inventory You Don`t Have CPs !! ");
        //                    data.AddOption("Okay. ", 255).AddAvatar(0).FinalizeDialog();
        //                    break;
        //                }
        //                break;
        //            }
        //        #endregion
        //        #region 500M
        //        case 60:
        //            {
        //                if (client.InTrade) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™ÂÃ˜Â§Ã˜ÂªÃ˜Â­ Ã˜ÂªÃ˜Â±Ã™Å Ã˜Â¯ Ã™â€¦Ã˜Â¹ Ã˜Â´Ã˜Â®Ã˜Âµ Ã˜ÂªÃ˜Â§Ã™â€ Ã™Å 
        //                {
        //                    client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
        //                    return;
        //                }
        //                if (client.IsVendor) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™â€šÃ˜Â§Ã˜Â¹Ã˜Â¯ Ã™ÂÃ™Å  Ã˜Â´Ã™Ë†Ã˜Â¨ Ã˜Â§Ã™â€žÃ™â€¦Ã˜Â±Ã™Æ’Ã˜Âª 
        //                {
        //                    client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
        //                    return;
        //                }
        //                if (client.Player.CpsBank >= 500000000)
        //                {
        //                    client.Player.ConquerPoints += 500000000;
        //                    client.Player.CpsBank -= 500000000;
        //                    data.AddText("You Have Withdraw 500.000.000 CPs In ( Bank Cps ) ");
        //                    data.AddOption("Thanks. ", 255).AddAvatar(0).FinalizeDialog();
        //                }
        //                else
        //                {
        //                    data.AddText("Check Your Inventory You Don`t Have CPs !! ");
        //                    data.AddOption("Okay. ", 255).AddAvatar(0).FinalizeDialog();
        //                    break;
        //                }
        //                break;
        //            }
        //        #endregion
        //        #region 1b
        //        case 103:
        //            {
        //                if (client.InTrade) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™ÂÃ˜Â§Ã˜ÂªÃ˜Â­ Ã˜ÂªÃ˜Â±Ã™Å Ã˜Â¯ Ã™â€¦Ã˜Â¹ Ã˜Â´Ã˜Â®Ã˜Âµ Ã˜ÂªÃ˜Â§Ã™â€ Ã™Å 
        //                {
        //                    client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
        //                    return;
        //                }
        //                if (client.IsVendor) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™â€šÃ˜Â§Ã˜Â¹Ã˜Â¯ Ã™ÂÃ™Å  Ã˜Â´Ã™Ë†Ã˜Â¨ Ã˜Â§Ã™â€žÃ™â€¦Ã˜Â±Ã™Æ’Ã˜Âª 
        //                {
        //                    client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
        //                    return;
        //                }
        //                if (client.Player.CpsBank >= 1000000000)
        //                {
        //                    client.Player.ConquerPoints += 1000000000;
        //                    client.Player.CpsBank -= 1000000000;
        //                    data.AddText("You Have Withdraw 1.000.000.000 CPs In ( Bank Cps ) ");
        //                    data.AddOption("Thanks. ", 255).AddAvatar(0).FinalizeDialog();
        //                }
        //                else
        //                {
        //                    data.AddText("Check Your Inventory You Don`t Have CPs !! ");
        //                    data.AddOption("Okay. ", 255).AddAvatar(0).FinalizeDialog();
        //                    break;
        //                }
        //                break;
        //            }
        //        #endregion
        //        #region 2b
        //        case 104:
        //            {
        //                if (client.InTrade) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™ÂÃ˜Â§Ã˜ÂªÃ˜Â­ Ã˜ÂªÃ˜Â±Ã™Å Ã˜Â¯ Ã™â€¦Ã˜Â¹ Ã˜Â´Ã˜Â®Ã˜Âµ Ã˜ÂªÃ˜Â§Ã™â€ Ã™Å 
        //                {
        //                    client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
        //                    return;
        //                }
        //                if (client.IsVendor) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™â€šÃ˜Â§Ã˜Â¹Ã˜Â¯ Ã™ÂÃ™Å  Ã˜Â´Ã™Ë†Ã˜Â¨ Ã˜Â§Ã™â€žÃ™â€¦Ã˜Â±Ã™Æ’Ã˜Âª 
        //                {
        //                    client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
        //                    return;
        //                }
        //                if (client.Player.CpsBank >= 2000000000)
        //                {
        //                    client.Player.ConquerPoints += 2000000000;
        //                    client.Player.CpsBank -= 2000000000;
        //                    data.AddText("You Have Withdraw 2.000.000.000 CPs In ( Bank Cps ) ");
        //                    data.AddOption("Thanks. ", 255).AddAvatar(0).FinalizeDialog();
        //                }
        //                else
        //                {
        //                    data.AddText("Check Your Inventory You Don`t Have CPs !! ");
        //                    data.AddOption("Okay. ", 255).AddAvatar(0).FinalizeDialog();
        //                    break;
        //                }
        //                break;
        //            }
        //        #endregion
        //        #region 3b
        //        case 115:
        //            {
        //                if (client.InTrade) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™ÂÃ˜Â§Ã˜ÂªÃ˜Â­ Ã˜ÂªÃ˜Â±Ã™Å Ã˜Â¯ Ã™â€¦Ã˜Â¹ Ã˜Â´Ã˜Â®Ã˜Âµ Ã˜ÂªÃ˜Â§Ã™â€ Ã™Å 
        //                {
        //                    client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
        //                    return;
        //                }
        //                if (client.IsVendor) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™â€šÃ˜Â§Ã˜Â¹Ã˜Â¯ Ã™ÂÃ™Å  Ã˜Â´Ã™Ë†Ã˜Â¨ Ã˜Â§Ã™â€žÃ™â€¦Ã˜Â±Ã™Æ’Ã˜Âª 
        //                {
        //                    client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
        //                    return;
        //                }
        //                if (client.Player.CpsBank >= 3000000000)
        //                {
        //                    client.Player.ConquerPoints += 3000000000;
        //                    client.Player.CpsBank -= 3000000000;
        //                    data.AddText("You Have Withdraw 3.000.000.000 CPs In ( Bank Cps ) ");
        //                    data.AddOption("Thanks. ", 255).AddAvatar(0).FinalizeDialog();
        //                }
        //                else
        //                {
        //                    data.AddText("Check Your Inventory You Don`t Have CPs !! ");
        //                    data.AddOption("Okay. ", 255).AddAvatar(0).FinalizeDialog();
        //                    break;
        //                }
        //                break;
        //            }
        //        #endregion
        //        #region 4b
        //        case 116:
        //            {
        //                if (client.InTrade) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™ÂÃ˜Â§Ã˜ÂªÃ˜Â­ Ã˜ÂªÃ˜Â±Ã™Å Ã˜Â¯ Ã™â€¦Ã˜Â¹ Ã˜Â´Ã˜Â®Ã˜Âµ Ã˜ÂªÃ˜Â§Ã™â€ Ã™Å 
        //                {
        //                    client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
        //                    return;
        //                }
        //                if (client.IsVendor) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™â€šÃ˜Â§Ã˜Â¹Ã˜Â¯ Ã™ÂÃ™Å  Ã˜Â´Ã™Ë†Ã˜Â¨ Ã˜Â§Ã™â€žÃ™â€¦Ã˜Â±Ã™Æ’Ã˜Âª 
        //                {
        //                    client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
        //                    return;
        //                }
        //                if (client.Player.CpsBank >= 4000000000)
        //                {
        //                    client.Player.ConquerPoints += 4000000000;
        //                    client.Player.CpsBank -= 4000000000;
        //                    data.AddText("You Have Withdraw 4.000.000.000 CPs In ( Bank Cps ) ");
        //                    data.AddOption("Thanks. ", 255).AddAvatar(0).FinalizeDialog();
        //                }
        //                else
        //                {
        //                    data.AddText("Check Your Inventory You Don`t Have CPs !! ");
        //                    data.AddOption("Okay. ", 255).AddAvatar(0).FinalizeDialog();
        //                    break;
        //                }
        //                break;
        //            }
        //        #endregion
        //        #endregion
        //    }
        //}

        private static string InspiredWeapon(Client.GameClient user, Database.MagicType.WeaponsType type)
        {
            try
            {
                if (user.HundredWeapons.Objects.Values.FirstOrDefault(i => i.WeaponSubtype == type) != null)
                    return "(Inspired)";
            }
            catch
            { return ""; }
            return "";
        }

        [NpcAttribute(NpcID.OpenArchives)]
        public static void OpenArchives(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!Database.AtributesStatus.IsTrojan(client.Player.Class)) return;
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        goto case 5;

                    }
                case 5:
                    {
                        data.Text("STR_ID_tHundredWeaponGridOne[23971][Text211]@@");
                        data.Text("STR_ID_tHundredWeaponGridOne[23971][Text212]@@");
                        data.Text("STR_ID_tHundredWeaponGridOne[23971][Text213]@@");
                        data.Text("STR_ID_tHundredWeaponGridOne[23971][Text214]@@");
                        data.Text("STR_ID_tHundredWeaponGridOne[23971][Text215]@@");
                        data.Text("STR_ID_tHundredWeaponGridOne[23971][Text216]@@");
                        data.Text("STR_ID_tHundredWeaponGridOne[23971][Text217]@@");
                        data.Text("STR_ID_tHundredWeaponGridOne[23971][Text218]@@");
                        data.Text("STR_ID_tHundredWeaponGridOne[23971][Text219]@@");
                        data.Text("STR_ID_tHundredWeaponGridOne[23971][Text2110]@@");
                        data.Text("STR_ID_tHundredWeaponGridOne[23971][Text2111]@@");
                        data.Text("STR_ID_tHundredWeaponGridOne[23971][Text2112]@@");
                        data.Text("STR_ID_tHundredWeaponGridOne[23971][Text2113]@@");
                        if (client.HundredWeapons.Valid())
                            data.Option("STR_ID_tHundredWeaponGridOne[23971][Option212]@@", 1);
                        data.AddOption("Read~the~Legendary~Weapons~book.", 2);
                        data.AddAvatar(2323).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("Li Xiaoyao:\n");
                        data.AddText("The ancient book tells the stories of nine legendary weapons. Which chapter would you like to read?\n");
                        data.AddOption("Dominator.", 20);
                        data.AddOption("Judge.", 21);
                        data.AddOption("Doomhammer.", 22);
                        data.AddOption("Fury.", 23);
                        data.AddOption("Soul", 24);
                        data.AddOption("Power.", 25);
                        data.AddOption("Aurora.", 26);
                        data.AddOption("Dragonfang.", 27);
                        data.AddOption("Faith.", 28);
                        data.AddAvatar(2323).FinalizeDialog();
                        break;
                    }
                case 20:
                    {
                        data.AddText("Li Xiaoyao:\n");
                        data.AddText("              * The Legendary Weapons - Dominator *\n");
                        data.AddText("Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾\n");
                        data.AddText("Wielded by the western god of war Ares, the Dominator Blade was said to have supernatural powers. Ares had taken this invincible heavy blade in battle, killing 999 formidable enemies.\n");
                        data.AddText("Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾\n");
                        data.AddText("Skill for the Weapon Spirit - [Clean Sweep]: Inflict 140% damage to a single target and extra damage of 3% Atttack to other targets within a fan area forward (400% extra damage to monsters).\n");
                        data.AddOption("Read~other~chapters.", 2);
                        data.AddOption("Close.", 255);
                        data.AddAvatar(2323).FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        data.AddText("Li Xiaoyao:\n");
                        data.AddText("              * The Legendary Weapons - Judge *\n");
                        data.AddText("Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾\n");
                        data.AddText("\t\tJudge was a holy sword owned by the legendary leader Arthur and given to him as a gift by the goddess of lake. The sword was said to have never let down a hero who wielded it and kill anyone it managed to damage.\n");
                        data.AddText("Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾\n");
                        data.AddText("Skill for the Weapon Spirit - [Song of Phoenix]: Inflict 135% damage to a single target (extra 300% damage to monsters).\n");
                        data.AddOption("Read~other~chapters.", 2);
                        data.AddOption("Close.", 255);
                        data.AddAvatar(2323).FinalizeDialog();
                        break;
                    }
                case 22:
                    {
                        data.AddText("Li Xiaoyao:\n");
                        data.AddText("              * The Legendary Weapons - Doomhammer *\n");
                        data.AddText("Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾\n");
                        data.AddText("\t\tAs its name implies, the Doomhammer may have the power to destroy the whole world. It was said to be forged by the ancient god Aiolia with star core and divine fire in ");
                        data.AddText("ten thousand years ago. With this, conquer a city alone!\nÃ˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾\n");
                        data.AddText("Skill for the Weapon Spirit - [Thunder Strike]: Inflict 155% damage to heroes within 4 paces (extra 800% damage on monsters).\n");
                        data.AddOption("Read~other~chapters.", 2);
                        data.AddOption("Close.", 255);
                        data.AddAvatar(2323).FinalizeDialog();
                        break;
                    }
                case 23:
                    {
                        data.AddText("Li Xiaoyao:\n");
                        data.AddText("              * The Legendary Weapons - Fury *\n");
                        data.AddText("Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾\n");
                        data.AddText("\t\tIn the battle of Thermopylae, Spartan leader Leonidas led only 300 Spartans defending against a force of 300,000 men, and wielded the Fury Axe killing 20,000 enemies.\n");
                        data.AddText("Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾\n");
                        data.AddText("Skill for the Weapon Spirit - [Axe Shadow]: Inflict 190% damage to heroes within a rectangle area (9*3) (extra 500% damage to monsters).\n");
                        data.AddOption("Read~other~chapters.", 2);
                        data.AddOption("Close.", 255);
                        data.AddAvatar(2323).FinalizeDialog();
                        break;
                    }
                case 24:
                    {
                        data.AddText("Li Xiaoyao:\n");
                        data.AddText("              * The Legendary Weapons - Soul *\n");
                        data.AddText("Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾\n");
                        data.AddText("\t\tSoul was the name of a hook wielded by Hardess who was titled the god of death. Hardess has reaped countless souls of enemies with this legendary weapon.\n");
                        data.AddText("Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾\n");
                        data.AddText("Skill for the Weapon Spirit - [Hook Moon]: Inflict 135% damage to heroes within 11 paces forward (extra 250% damage to monsters).\n");
                        data.AddOption("Read~other~chapters.", 2);
                        data.AddOption("Close.", 255);
                        data.AddAvatar(2323).FinalizeDialog();
                        break;
                    }
                case 25:
                    {
                        data.AddText("Li Xiaoyao:\n");
                        data.AddText("              * The Legendary Weapons - Power *\n");
                        data.AddText("Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾\n");
                        data.AddText("\t\tThe Power Whip was wield by Alsace, the warden of ancient divine prison, to punish corrupted deities and devils. The weapon was said to be imbued with both angelic and demonic powers.\n");
                        data.AddText("Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾\n");
                        data.AddText("Skill for the Weapon Spirit - [Hawk`s Ambition]: Immune from damage once and rebound up to 40% damage to opponent for 10 seconds.\n");
                        data.AddOption("Read~other~chapters.", 2);
                        data.AddOption("Close.", 255);
                        data.AddAvatar(2323).FinalizeDialog();
                        break;
                    }
                case 26:
                    {
                        data.AddText("Li Xiaoyao:\n");
                        data.AddText("              * The Legendary Weapons - Aurora *\n");
                        data.AddText("Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾\n");
                        data.AddText("\t\tLegend says that the Aurora Scepter was created in the universeÃ˜Å’Ã‚Â¯s beginning. The scepter was also believed to have shone brightly in the Sun and was said to have infinite pure powers.");
                        data.AddText("Only the most upright owner like Arroyo can control its terrific power.\nÃ˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾\n");
                        data.AddText("Skill for the Weapon Spirit - [Sacred Blessing]: Has a chance to trigger the Immunity status and make the target bleed. If you die in the Sacred Blessing status, your soul will not be shackled.\n");
                        data.AddOption("Read~other~chapters.", 2);
                        data.AddOption("Close.", 255);
                        data.AddAvatar(2323).FinalizeDialog();
                        break;
                    }
                case 27:
                    {
                        data.AddText("Li Xiaoyao:\n");
                        data.AddText("              * The Legendary Weapons - Dragonfang *\n");
                        data.AddText("Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾\n");
                        data.AddText("\t\tThousands of years ago, LucisÃ˜Å’Ã‚Â¯ rebellion against the divine caused heavy causalities on both deities and humans. The half-god Trojan Perseus was given the Dragonfang Dagger by the First Dragon.");
                        data.AddText("The dagger was meant to symbolize glory and victory. Perseus finally killed Lucis and saved the whole world from destruction.Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾\n");
                        data.AddText("Skill for the Weapon Spirit - [Death`s Sigh]: Inflict 210% damage to heroes (900% to monsters).\n");
                        data.AddOption("Read~other~chapters.", 2);
                        data.AddOption("Close.", 255);
                        data.AddAvatar(2323).FinalizeDialog();
                        break;
                    }
                case 28:
                    {
                        data.AddText("Li Xiaoyao:\n");
                        data.AddText("              * The Legendary Weapons - Faith *\n");
                        data.AddText("Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾\n");
                        data.AddText("\t\tTo save the mortal world from atrocious Dark Glutton, Achilles vowed permanent loyalty to the Great King Austin and was given the Faith Club. Achilles killed the Dark Glutton");
                        data.AddText("after a fierce battle, saving countless humans from misery. The Faith Club was worshiped as one of the nine legendary weapons.Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾\n");
                        data.AddText("Skill for the Weapon Spirit - [Deadly Strike]: Inflict 195% damage to heroes within 4 paces (extra 600% damage to monster).\n");
                        data.AddOption("Read~other~chapters.", 2);
                        data.AddOption("Close.", 255);
                        data.AddAvatar(2323).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        data.Text("STR_ID_tHundredWeaponGridOne[23971][Text311]@@");
                        data.Text("STR_ID_tHundredWeaponGridOne[23971][Text312]@@");
                        data.Option("STR_ID_tHundredWeaponGridOne[23971][Option411]@@ " + InspiredWeapon(client, Database.MagicType.WeaponsType.Blade) + "@@", 10);
                        data.Option("STR_ID_tHundredWeaponGridOne[23971][Option412]@@" + InspiredWeapon(client, Database.MagicType.WeaponsType.Sword) + "@@", 11);
                        data.Option("STR_ID_tHundredWeaponGridOne[23971][Option413]@@" + InspiredWeapon(client, Database.MagicType.WeaponsType.Hammer) + "@@", 12);
                        data.Option("STR_ID_tHundredWeaponGridOne[23971][Option414]@@" + InspiredWeapon(client, Database.MagicType.WeaponsType.Axe) + "@@", 13);
                        data.Option("STR_ID_tHundredWeaponGridOne[23971][Option415]@@" + InspiredWeapon(client, Database.MagicType.WeaponsType.Hook) + "@@", 14);
                        data.Option("STR_ID_tHundredWeaponGridOne[23971][Option416]@@" + InspiredWeapon(client, Database.MagicType.WeaponsType.Whip) + "@@", 15);
                        data.Option("STR_ID_tHundredWeaponGridOne[23971][Option417]@@" + InspiredWeapon(client, Database.MagicType.WeaponsType.Scepter) + "@@", 16);
                        data.Option("STR_ID_tHundredWeaponGridOne[23971][Option418]@@" + InspiredWeapon(client, Database.MagicType.WeaponsType.Dagger) + "@@", 17);
                        data.Option("STR_ID_tHundredWeaponGridOne[23971][Option419]@@" + InspiredWeapon(client, Database.MagicType.WeaponsType.Club) + "@@", 18);
                        data.AddAvatar(2323).FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        data.Text("STR_ID_tHundredWeaponGridOne[23971][Text321]@@");
                        data.Text("STR_ID_tHundredWeaponGridOne[23971][Text322]@@");
                        data.Option("STR_ID_tHundredWeaponGridOne[23971][Option321]@@", 30);
                        data.Option("STR_ID_tHundredWeaponGridOne[23971][Option322]@@", 255);
                        data.AddAvatar(2323).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        data.Text("STR_ID_tHundredWeaponGridOne[23971][Text331]@@");
                        data.Text("STR_ID_tHundredWeaponGridOne[23971][Text332]@@");
                        data.Option("STR_ID_tHundredWeaponGridOne[23971][Option331]@@", 31);
                        data.Option("STR_ID_tHundredWeaponGridOne[23971][Option332]@@", 255);
                        data.AddAvatar(2323).FinalizeDialog();
                        break;
                    }
                case 12:
                    {
                        data.Text("STR_ID_tHundredWeaponGridOne[23971][Text341]@@");
                        data.Text("STR_ID_tHundredWeaponGridOne[23971][Text342]@@");
                        data.Option("STR_ID_tHundredWeaponGridOne[23971][Option341]@@", 32);
                        data.Option("STR_ID_tHundredWeaponGridOne[23971][Option342]@@", 255);
                        data.AddAvatar(2323).FinalizeDialog();
                        break;
                    }
                case 13:
                    {
                        data.Text("STR_ID_tHundredWeaponGridOne[23971][Text351]@@");
                        data.Text("STR_ID_tHundredWeaponGridOne[23971][Text352]@@");
                        data.Option("STR_ID_tHundredWeaponGridOne[23971][Option351]@@", 33);
                        data.Option("STR_ID_tHundredWeaponGridOne[23971][Option352]@@", 255);
                        data.AddAvatar(2323).FinalizeDialog();
                        break;
                    }
                case 14:
                    {
                        data.Text("STR_ID_tHundredWeaponGridOne[23971][Text361]@@");
                        data.Text("STR_ID_tHundredWeaponGridOne[23971][Text362]@@");
                        data.Option("STR_ID_tHundredWeaponGridOne[23971][Option361]@@", 34);
                        data.Option("STR_ID_tHundredWeaponGridOne[23971][Option362]@@", 255);
                        data.AddAvatar(2323).FinalizeDialog();
                        break;
                    }
                case 15:
                    {
                        data.Text("STR_ID_tHundredWeaponGridOne[23971][Text371]@@");
                        data.Text("STR_ID_tHundredWeaponGridOne[23971][Text372]@@");
                        data.Option("STR_ID_tHundredWeaponGridOne[23971][Option371]@@", 35);
                        data.Option("STR_ID_tHundredWeaponGridOne[23971][Option372]@@", 255);
                        data.AddAvatar(2323).FinalizeDialog();
                        break;
                    }
                case 16:
                    {
                        data.Text("STR_ID_tHundredWeaponGridOne[23971][Text381]@@");
                        data.Text("STR_ID_tHundredWeaponGridOne[23971][Text382]@@");
                        data.Option("STR_ID_tHundredWeaponGridOne[23971][Option381]@@", 36);
                        data.Option("STR_ID_tHundredWeaponGridOne[23971][Option382]@@", 255);
                        data.AddAvatar(2323).FinalizeDialog();
                        break;
                    }
                case 17:
                    {
                        data.Text("STR_ID_tHundredWeaponGridOne[23971][Text391]@@");
                        data.Text("STR_ID_tHundredWeaponGridOne[23971][Text392]@@");
                        data.Option("STR_ID_tHundredWeaponGridOne[23971][Option391]@@", 37);
                        data.Option("STR_ID_tHundredWeaponGridOne[23971][Option392]@@", 255);
                        data.AddAvatar(2323).FinalizeDialog();
                        break;
                    }
                case 18:
                    {
                        data.Text("STR_ID_tHundredWeaponGridOne[23971][Text3101]@@");
                        data.Text("STR_ID_tHundredWeaponGridOne[23971][Text3102]@@");
                        data.Option("STR_ID_tHundredWeaponGridOne[23971][Option3101]@@", 38);
                        data.Option("STR_ID_tHundredWeaponGridOne[23971][Option3102]@@", 255);
                        data.AddAvatar(2323).FinalizeDialog();
                        break;
                    }
                case 30:
                case 31:
                case 32:
                case 33:
                case 34:
                case 35:
                case 36:
                case 37:
                case 38:
                    {
                        if (Option >= 30 && Option <= 38)
                        {
                            Database.MagicType.WeaponsType type = 0;
                            if (Option == 30)
                                type = Database.MagicType.WeaponsType.Blade;
                            if (Option == 31)
                                type = Database.MagicType.WeaponsType.Sword;
                            if (Option == 32)
                                type = Database.MagicType.WeaponsType.Hammer;
                            if (Option == 33)
                                type = Database.MagicType.WeaponsType.Axe;
                            if (Option == 34)
                                type = Database.MagicType.WeaponsType.Hook;
                            if (Option == 35)
                                type = Database.MagicType.WeaponsType.Whip;
                            if (Option == 36)
                                type = Database.MagicType.WeaponsType.Scepter;
                            if (Option == 37)
                                type = Database.MagicType.WeaponsType.Dagger;
                            if (Option == 38)
                                type = Database.MagicType.WeaponsType.Club;
                            if (client.HundredWeapons.Valid() && client.HundredWeapons.Unlocked)
                            {
                                var web = client.HundredWeapons.Objects.Values.FirstOrDefault(i => i.WeaponSubtype == type);
                                if (web == null)
                                {
                                    client.HundredWeapons.AddWeapon(type, 1, true);
                                    client.Send(stream.CreateHundredWeaponsInfoNPC(client, type, MsgHundredWeaponsInfo.ActionID.Add));
                                }
                            }
                            data.AddText("Li Xiaoyao:\n");
                            data.AddText("Different Weapon Spirits can be inspired in different ways. I`ll inspire one Weapon Spirit for you. It`s free.\n");
                            data.AddOption("Thanks!~Please~inspire~one~for~me.", 255);
                            data.AddAvatar(2323).FinalizeDialog();
                        }
                        break;
                    }
            }
        }
    }
}

