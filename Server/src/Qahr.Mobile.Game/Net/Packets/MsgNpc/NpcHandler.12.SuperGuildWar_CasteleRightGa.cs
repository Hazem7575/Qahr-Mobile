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
        [NpcAttribute(NpcID.SuperGuildWar_CasteleRightGate)]
        public static void SuperGuildWar_CasteleRightGate(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Select the option you want to pursue.")
                                                .AddOption("Open~the~gate", 1)
                                                .AddOption("Close gate.", 2)
                                                .AddOption("Get inside.", 3)
                                                .AddOption("Not~interested.", 255).FinalizeDialog();


                        break;
                    }
                case 3:
                    {
                        if (client.Player.GuildID == MsgTournaments.MsgSchedules.SuperGuildWar.Winner.GuildID)
                        {
                            client.Teleport(208, 179, 1138);
                        }
                        else
                        {
                            data.AddText("Sorry,~you~cannot~open~it.Only DSl or GLs can, from guild who dominated the pole.")
                              .AddOption("Okay.", 255).FinalizeDialog();

                        }


                        break;
                    }
                case 2:
                    {
                        if ((client.Player.GuildRank == Role.Flags.GuildMemberRank.DeputyLeader
                                                  || client.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader) && MsgTournaments.MsgSchedules.SuperGuildWar.Winner.GuildID == client.Player.GuildID)
                        {

                            var Gate = MsgTournaments.MsgSchedules.SuperGuildWar.Furnitures[MsgTournaments.MsgSuperGuildWar.FurnituresType.CastleRightGate];

                            if (Gate.Mesh == Role.SobNpc.StaticMesh.RightGate)
                            {
                                data.AddText("Gate~is~already~closed.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                                break;
                            }
                            if (Gate.HitPoints == 0)
                                Gate.HitPoints = Gate.MaxHitPoints;

                            Gate.Mesh = Role.SobNpc.StaticMesh.RightGate;


                            MsgServer.MsgUpdate upd = new MsgServer.MsgUpdate(stream, Gate.UID, 1);

                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Mesh, (long)Gate.Mesh);
                            stream = upd.GetArray(stream);
                            foreach (var pclient in Pool.GamePoll.Values)
                            {
                                if (pclient.Player.Map == MsgTournaments.MsgSuperGuildWar.MapID)
                                {
                                    if (Role.Core.GetDistance(pclient.Player.X, pclient.Player.Y, Gate.X
                                        , Gate.Y) <= Role.SobNpc.SeedDistrance)
                                    {
                                        pclient.Send(stream);
                                    }
                                }
                            }
                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Hitpoints, (long)Gate.HitPoints);
                            stream = upd.GetArray(stream);
                            foreach (var pclient in Pool.GamePoll.Values)
                            {
                                if (pclient.Player.Map == MsgTournaments.MsgSuperGuildWar.MapID)
                                {
                                    if (Role.Core.GetDistance(pclient.Player.X, pclient.Player.Y, Gate.X
                                        , Gate.Y) <= Role.SobNpc.SeedDistrance)
                                    {
                                        pclient.Send(stream);
                                    }
                                }
                            }
                        }
                        else
                        {

                            data.AddText("Sorry,~you~cannot~open~it.Only DSl or GLs can, from guild who dominated the pole.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                        }


                        break;
                    }
                case 1:
                    {
                        if ((client.Player.GuildRank == Role.Flags.GuildMemberRank.DeputyLeader
                            || client.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader) && MsgTournaments.MsgSchedules.SuperGuildWar.Winner.GuildID == client.Player.GuildID)
                        {
                            var Gate = MsgTournaments.MsgSchedules.SuperGuildWar.Furnitures[MsgTournaments.MsgSuperGuildWar.FurnituresType.CastleRightGate];

                            if (Gate.Mesh == Role.SobNpc.StaticMesh.OpenRightGate)
                            {
                                data.AddText("Gate~is~already~opened.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                                break;
                            }
                            Gate.Mesh = Role.SobNpc.StaticMesh.OpenRightGate;
                            MsgServer.MsgUpdate upd = new MsgServer.MsgUpdate(stream, Gate.UID, 1);

                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Mesh, (long)Gate.Mesh);
                            stream = upd.GetArray(stream);
                            foreach (var pclient in Pool.GamePoll.Values)
                            {
                                if (pclient.Player.Map == MsgTournaments.MsgSuperGuildWar.MapID)
                                {
                                    if (Role.Core.GetDistance(pclient.Player.X, pclient.Player.Y, Gate.X, Gate.Y) <= Role.SobNpc.SeedDistrance)
                                    {
                                        pclient.Send(stream);
                                    }
                                }
                            }
                        }
                        else
                        {

                            data.AddText("Sorry,~you~cannot~open~it.Only DSl or GLs can, from guild who dominated the pole.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                        }


                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.SuperGuildWar_LeftGate)]
        public static void SuperGuildWar_LeftGate(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Select the option you want to pursue.")
                        .AddOption("Open~the~gate", 1)
                        .AddOption("Close gate.", 2)
                        .AddOption("Get inside.", 3)
                        .AddOption("Not~interested.", 255).FinalizeDialog();


                        break;
                    }
                case 3:
                    {
                        if (client.Player.GuildID == MsgTournaments.MsgSchedules.SuperGuildWar.Winner.GuildID)
                        {
                            client.Teleport(164, 194, 1138);
                        }
                        else
                        {
                            data.AddText("Sorry,~you~cannot~open~it.Only DSl or GLs can, from guild who dominated the pole.")
                              .AddOption("Okay.", 255).FinalizeDialog();

                        }


                        break;
                    }
                case 2:
                    {
                        if ((client.Player.GuildRank == Role.Flags.GuildMemberRank.DeputyLeader
                           || client.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader) && MsgTournaments.MsgSchedules.SuperGuildWar.Winner.GuildID == client.Player.GuildID)
                        {

                            var Gate = MsgTournaments.MsgSchedules.SuperGuildWar.Furnitures[MsgTournaments.MsgSuperGuildWar.FurnituresType.LeftGate];

                            if (Gate.Mesh == Role.SobNpc.StaticMesh.LeftGate)
                            {
                                data.AddText("Gate~is~already~closed.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                                break;
                            }
                            if (Gate.HitPoints == 0)
                                Gate.HitPoints = Gate.MaxHitPoints;

                            Gate.Mesh = Role.SobNpc.StaticMesh.LeftGate;

                            MsgServer.MsgUpdate upd = new MsgServer.MsgUpdate(stream, Gate.UID, 1);

                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Mesh, (long)Gate.Mesh);
                            stream = upd.GetArray(stream);
                            foreach (var pclient in Pool.GamePoll.Values)
                            {
                                if (pclient.Player.Map == MsgTournaments.MsgSuperGuildWar.MapID)
                                {
                                    if (Role.Core.GetDistance(pclient.Player.X, pclient.Player.Y, Gate.X
                                        , Gate.Y) <= Role.SobNpc.SeedDistrance)
                                    {
                                        pclient.Send(stream);
                                    }
                                }
                            }
                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Hitpoints, (long)Gate.HitPoints);
                            stream = upd.GetArray(stream);
                            foreach (var pclient in Pool.GamePoll.Values)
                            {
                                if (pclient.Player.Map == MsgTournaments.MsgSuperGuildWar.MapID)
                                {
                                    if (Role.Core.GetDistance(pclient.Player.X, pclient.Player.Y, Gate.X
                                        , Gate.Y) <= Role.SobNpc.SeedDistrance)
                                    {
                                        pclient.Send(stream);
                                    }
                                }
                            }
                        }
                        else
                        {

                            data.AddText("Sorry,~you~cannot~open~it.Only DSl or GLs can, from guild who dominated the pole.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                        }


                        break;
                    }
                case 1:
                    {
                        if ((client.Player.GuildRank == Role.Flags.GuildMemberRank.DeputyLeader
                            || client.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader) && MsgTournaments.MsgSchedules.SuperGuildWar.Winner.GuildID == client.Player.GuildID)
                        {
                            var Gate = MsgTournaments.MsgSchedules.SuperGuildWar.Furnitures[MsgTournaments.MsgSuperGuildWar.FurnituresType.LeftGate];

                            if (Gate.Mesh == Role.SobNpc.StaticMesh.OpenLeftGate)
                            {
                                data.AddText("Gate~is~already~opened.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                                break;
                            }
                            Gate.Mesh = Role.SobNpc.StaticMesh.OpenLeftGate;
                            MsgServer.MsgUpdate upd = new MsgServer.MsgUpdate(stream, Gate.UID, 1);

                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Mesh, (long)Gate.Mesh);
                            stream = upd.GetArray(stream);
                            foreach (var pclient in Pool.GamePoll.Values)
                            {
                                if (pclient.Player.Map == MsgTournaments.MsgSuperGuildWar.MapID)
                                {
                                    if (Role.Core.GetDistance(pclient.Player.X, pclient.Player.Y, Gate.X, Gate.Y) <= Role.SobNpc.SeedDistrance)
                                    {
                                        pclient.Send(stream);
                                    }
                                }
                            }
                        }
                        else
                        {

                            data.AddText("Sorry,~you~cannot~open~it.Only DSl or GLs can, from guild who dominated the pole.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.SuperGuildWar_MidleGate)]
        public static void SuperGuildWar_MidleGate(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Select the option you want to pursue.")
                       .AddOption("Open~the~gate", 1)
                       .AddOption("Close gate.", 2)
                       .AddOption("Get inside.", 3)
                       .AddOption("Not~interested.", 255).FinalizeDialog();


                        break;
                    }
                case 3:
                    {
                        if (client.Player.GuildID == MsgTournaments.MsgSchedules.SuperGuildWar.Winner.GuildID)
                        {
                            client.Teleport(164, 194, 1138);
                        }
                        else
                        {
                            data.AddText("Sorry,~you~cannot~open~it.Only DSl or GLs can, from guild who dominated the pole.")
                              .AddOption("Okay.", 255).FinalizeDialog();

                        }

                        break;
                    }
                case 2:
                    {
                        if ((client.Player.GuildRank == Role.Flags.GuildMemberRank.DeputyLeader
                                                   || client.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader) && MsgTournaments.MsgSchedules.SuperGuildWar.Winner.GuildID == client.Player.GuildID)
                        {

                            var Gate = MsgTournaments.MsgSchedules.SuperGuildWar.Furnitures[MsgTournaments.MsgSuperGuildWar.FurnituresType.CenterGate];

                            if (Gate.Mesh == Role.SobNpc.StaticMesh.LeftGate)
                            {
                                data.AddText("Gate~is~already~closed.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                                break;
                            }
                            if (Gate.HitPoints == 0)
                                Gate.HitPoints = Gate.MaxHitPoints;

                            Gate.Mesh = Role.SobNpc.StaticMesh.LeftGate;

                            MsgServer.MsgUpdate upd = new MsgServer.MsgUpdate(stream, Gate.UID, 1);

                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Mesh, (long)Gate.Mesh);
                            stream = upd.GetArray(stream);
                            foreach (var pclient in Pool.GamePoll.Values)
                            {
                                if (pclient.Player.Map == MsgTournaments.MsgSuperGuildWar.MapID)
                                {
                                    if (Role.Core.GetDistance(pclient.Player.X, pclient.Player.Y, Gate.X
                                        , Gate.Y) <= Role.SobNpc.SeedDistrance)
                                    {
                                        pclient.Send(stream);
                                    }
                                }
                            }
                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Hitpoints, (long)Gate.HitPoints);
                            stream = upd.GetArray(stream);
                            foreach (var pclient in Pool.GamePoll.Values)
                            {
                                if (pclient.Player.Map == MsgTournaments.MsgSuperGuildWar.MapID)
                                {
                                    if (Role.Core.GetDistance(pclient.Player.X, pclient.Player.Y, Gate.X
                                        , Gate.Y) <= Role.SobNpc.SeedDistrance)
                                    {
                                        pclient.Send(stream);
                                    }
                                }
                            }
                        }
                        else
                        {

                            data.AddText("Sorry,~you~cannot~open~it.Only DSl or GLs can, from guild who dominated the pole.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                        }


                        break;
                    }
                case 1:
                    {
                        if ((client.Player.GuildRank == Role.Flags.GuildMemberRank.DeputyLeader
                            || client.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader) && MsgTournaments.MsgSchedules.SuperGuildWar.Winner.GuildID == client.Player.GuildID)
                        {
                            var Gate = MsgTournaments.MsgSchedules.SuperGuildWar.Furnitures[MsgTournaments.MsgSuperGuildWar.FurnituresType.CenterGate];

                            if (Gate.Mesh == Role.SobNpc.StaticMesh.OpenLeftGate)
                            {
                                data.AddText("Gate~is~already~opened.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                                break;
                            }
                            Gate.Mesh = Role.SobNpc.StaticMesh.OpenLeftGate;
                            MsgServer.MsgUpdate upd = new MsgServer.MsgUpdate(stream, Gate.UID, 1);

                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Mesh, (long)Gate.Mesh);
                            stream = upd.GetArray(stream);
                            foreach (var pclient in Pool.GamePoll.Values)
                            {
                                if (pclient.Player.Map == MsgTournaments.MsgSuperGuildWar.MapID)
                                {
                                    if (Role.Core.GetDistance(pclient.Player.X, pclient.Player.Y, Gate.X, Gate.Y) <= Role.SobNpc.SeedDistrance)
                                    {
                                        pclient.Send(stream);
                                    }
                                }
                            }
                        }
                        else
                        {

                            data.AddText("Sorry,~you~cannot~open~it.Only DSl or GLs can, from guild who dominated the pole.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.SuperGuildWar_RightGate)]
        public static void SuperGuildWar_RightGate(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Select the option you want to pursue.")
                        .AddOption("Open~the~gate", 1)
                        .AddOption("Close gate.", 2)
                        .AddOption("Get inside.", 3)
                        .AddOption("Not~interested.", 255).FinalizeDialog();


                        break;
                    }
                case 3:
                    {
                        if (client.Player.GuildID == MsgTournaments.MsgSchedules.SuperGuildWar.Winner.GuildID)
                        {
                            client.Teleport(208, 179, 1138);
                        }
                        else
                        {
                            data.AddText("Sorry,~you~cannot~open~it.Only DSl or GLs can, from guild who dominated the pole.")
                              .AddOption("Okay.", 255).FinalizeDialog();

                        }

                        break;
                    }
                case 2:
                    {
                        if ((client.Player.GuildRank == Role.Flags.GuildMemberRank.DeputyLeader
                                                   || client.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader) && MsgTournaments.MsgSchedules.SuperGuildWar.Winner.GuildID == client.Player.GuildID)
                        {

                            var Gate = MsgTournaments.MsgSchedules.SuperGuildWar.Furnitures[MsgTournaments.MsgSuperGuildWar.FurnituresType.RightGate];

                            if (Gate.Mesh == Role.SobNpc.StaticMesh.LeftGate)
                            {
                                data.AddText("Gate~is~already~closed.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                                break;
                            }
                            if (Gate.HitPoints == 0)
                                Gate.HitPoints = Gate.MaxHitPoints;

                            Gate.Mesh = Role.SobNpc.StaticMesh.LeftGate;

                            MsgServer.MsgUpdate upd = new MsgServer.MsgUpdate(stream, Gate.UID, 1);

                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Mesh, (long)Gate.Mesh);
                            stream = upd.GetArray(stream);
                            foreach (var pclient in Pool.GamePoll.Values)
                            {
                                if (pclient.Player.Map == MsgTournaments.MsgSuperGuildWar.MapID)
                                {
                                    if (Role.Core.GetDistance(pclient.Player.X, pclient.Player.Y, Gate.X
                                        , Gate.Y) <= Role.SobNpc.SeedDistrance)
                                    {
                                        pclient.Send(stream);
                                    }
                                }
                            }
                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Hitpoints, (long)Gate.HitPoints);
                            stream = upd.GetArray(stream);
                            foreach (var pclient in Pool.GamePoll.Values)
                            {
                                if (pclient.Player.Map == MsgTournaments.MsgSuperGuildWar.MapID)
                                {
                                    if (Role.Core.GetDistance(pclient.Player.X, pclient.Player.Y, Gate.X
                                        , Gate.Y) <= Role.SobNpc.SeedDistrance)
                                    {
                                        pclient.Send(stream);
                                    }
                                }
                            }
                        }
                        else
                        {

                            data.AddText("Sorry,~you~cannot~open~it.Only DSl or GLs can, from guild who dominated the pole.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                        }


                        break;
                    }
                case 1:
                    {
                        if ((client.Player.GuildRank == Role.Flags.GuildMemberRank.DeputyLeader
                            || client.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader) && MsgTournaments.MsgSchedules.SuperGuildWar.Winner.GuildID == client.Player.GuildID)
                        {
                            var Gate = MsgTournaments.MsgSchedules.SuperGuildWar.Furnitures[MsgTournaments.MsgSuperGuildWar.FurnituresType.RightGate];

                            if (Gate.Mesh == Role.SobNpc.StaticMesh.OpenLeftGate)
                            {
                                data.AddText("Gate~is~already~opened.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                                break;
                            }
                            Gate.Mesh = Role.SobNpc.StaticMesh.OpenLeftGate;
                            MsgServer.MsgUpdate upd = new MsgServer.MsgUpdate(stream, Gate.UID, 1);

                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Mesh, (long)Gate.Mesh);
                            stream = upd.GetArray(stream);
                            foreach (var pclient in Pool.GamePoll.Values)
                            {
                                if (pclient.Player.Map == MsgTournaments.MsgSuperGuildWar.MapID)
                                {
                                    if (Role.Core.GetDistance(pclient.Player.X, pclient.Player.Y, Gate.X, Gate.Y) <= Role.SobNpc.SeedDistrance)
                                    {
                                        pclient.Send(stream);
                                    }
                                }
                            }
                        }
                        else
                        {

                            data.AddText("Sorry,~you~cannot~open~it.Only DSl or GLs can, from guild who dominated the pole.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Class6House)]
        public static void Class6House(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Welcome, this is the Class 6 model house with many new features. When you`re walking around the house,")
                            .AddText("~you can talk to our guide in each area to learn more. If you want to upgrade your house to Class 6,")
                            .AddText("~I`m the right person to serve you./n")
                            .AddText("You will need 99,000 Conquer Points in order to update your House to the next level.");
                        data.AddOption("Upgrade~my~house~with~CPs.", 1);
                        data.AddOption("Use~materials~to~upgrade.", 3);
                        data.AddOption("How~to~upgrade~my~house?", 6);
                        data.AddOption("Where~is~the~exit?", 2);
                        data.AddOption("Just passing by.", 255).AddAvatar(111).FinalizeDialog();


                        break;
                    }
                case 6:
                    {
                        data.AddText("There are two ways to upgrade to Class 6: pay 8,999 CPs or submit 500 Class 6 House Packs. You can collect the Class 6 House Packs")
                            .AddText("~by participating in Dis City, Evil Labyrinth, cross-server competitions and other quests, or buy them with Honor Points,")
                            .AddText("~Horse Racing Points, or Champion Points.")
                              .AddOption("I~see", 255).AddAvatar(111).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        data.AddText("We need totally 500 Class 6 House Packs to upgrade your house to Class 6. So, how many packs would you like to submit this time?")//asta?
                                .AddOption("100.", 4)
                                .AddOption("500.", 5)
                                .AddOption("Let~me~see.", 255).AddAvatar(111).FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.Contain(3008100, 100))
                        {
                            client.Inventory.RemoveStackItem(3008100, 100, stream);
                            if (Role.Core.Rate(30))
                            {
                                if (client.MyHouse == null)
                                    break;

                                ushort House_ID = Database.HouseTable.GetHouseId(client.MyHouse.Level);
                                if (House_ID != 0)
                                {
                                    //remove players house
                                    foreach (var pClient in Pool.GamePoll.Values)
                                        if (pClient.Player.Map == House_ID && pClient.Player.DynamicID == client.Player.UID)
                                            pClient.Teleport(1036, 200, 200);
                                }
                                if (client.MyHouse.Furnitures.Count != 0)
                                {
                                    data.AddText("Please remove all your furnitures from your house.")//asta?
                                       .AddOption("I~see.", 255).AddAvatar(111).FinalizeDialog();
                                    break;
                                }
                                if (client.MyHouse.Level == 5)
                                {

                                    client.MyHouse.Level++;
                                    data.AddText("You have successfully updated your House.")
                                 .AddOption("Thank you.", 255).AddAvatar(111).FinalizeDialog();
                                    Server.SendGlobalPacket(new MsgServer.MsgMessage("Hurray ! " + client.Player.Name + " has successfully updated his house to the level 6.", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.System).GetArray(stream));

                                    client.Teleport(200, 95, 1036);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "movego");
                                }
                            }
                            else
                            {
                                data.AddText("You have failed to upgrade the house to level 6 , give a try again later maybe you will have more luck .")
                              .AddOption("I~see.", 255).AddAvatar(111).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("I`m afraid I can`t help you if you don`t have enough materials. Please go collect more.")
                               .AddOption("Okay.", 255).AddAvatar(111).FinalizeDialog();
                        }
                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.Contain(3008100, 500))
                        {

                            if (client.MyHouse == null)
                                break;

                            ushort House_ID = Database.HouseTable.GetHouseId(client.MyHouse.Level);
                            if (House_ID != 0)
                            {
                                //remove players house
                                foreach (var pClient in Pool.GamePoll.Values)
                                    if (pClient.Player.Map == House_ID && pClient.Player.DynamicID == client.Player.UID)
                                        pClient.Teleport(1036, 200, 200);
                            }
                            if (client.MyHouse.Furnitures.Count != 0)
                            {
                                data.AddText("Please remove all your furnitures from your house.")//asta?
                                   .AddOption("I~see.", 255).AddAvatar(111).FinalizeDialog();
                                break;
                            }
                            if (client.MyHouse.Level == 5)
                            {
                                client.Inventory.RemoveStackItem(3008100, 500, stream);
                                client.MyHouse.Level++;
                                data.AddText("You have successfully updated your House.")
                             .AddOption("Thank you.", 255).AddAvatar(111).FinalizeDialog();
                                Server.SendGlobalPacket(new MsgServer.MsgMessage("Hurray ! " + client.Player.Name + " has successfully updated his house to the level 6.", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.System).GetArray(stream));

                                client.Teleport(200, 95, 1036);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "movego");
                            }
                        }
                        else
                        {
                            data.AddText("I`m afraid I can`t help you if you don`t have enough materials. Please go collect more.")
                               .AddOption("Okay.", 255).AddAvatar(111).FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        client.Teleport(200, 95, 1036);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "movego");
                        break;
                    }
                case 1:
                    {
                        if (client.Player.ConquerPoints >= 99000)
                        {
                            if (client.InTrade) return;
                            if (client.MyHouse == null)
                                break;

                            ushort House_ID = Database.HouseTable.GetHouseId(client.MyHouse.Level);
                            if (House_ID != 0)
                            {
                                //remove players house
                                foreach (var pClient in Pool.GamePoll.Values)
                                    if (pClient.Player.Map == House_ID && pClient.Player.DynamicID == client.Player.UID)
                                        pClient.Teleport(1036, 200, 200);
                            }
                            if (client.MyHouse.Furnitures.Count != 0)
                            {
                                data.AddText("Please remove all your furnitures from your house.")//asta?
                                   .AddOption("I~see.", 255).AddAvatar(111).FinalizeDialog();
                                break;
                            }
                            if (client.MyHouse.Level == 5)
                            {
                                client.Player.ConquerPoints -= 99000;
                                client.MyHouse.Level++;
                                data.AddText("You have successfully updated your House.")
                             .AddOption("Thank you.", 255).AddAvatar(111).FinalizeDialog();
                                Server.SendGlobalPacket(new MsgServer.MsgMessage("Hurray ! " + client.Player.Name + " has successfully updated his house to the level 6.", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.System).GetArray(stream));

                                client.Teleport(200, 95, 1036);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "movego");
                            }


                        }
                        else
                        {
                            data.AddText("Sorry, I can`t help you in this matter since you don`t have enough CPs. Please bring the money, next time.")
                            .AddOption("Okay.", 255).AddAvatar(111).FinalizeDialog();
                        }
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.HouseAdmin)]
        public static void HouseAdmin(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.MyHouse == null)
                        {
                            data.AddText("What can i do for you?");
                            data.AddOption("buy a house.", 1);
                            data.AddOption("buy house certificate.", 2);
                            data.AddOption("Visit my wife`s House.", 3);
                            data.AddOption("Just passing by.", 255).AddAvatar(7).FinalizeDialog();

                        }
                        else if (client.MyHouse.Level == 0)
                        {
                            data.AddText("What can i do for you?");
                            data.AddOption("buy a house.", 1);
                            data.AddOption("buy house certificate.", 2);
                            data.AddOption("Visit my wife`s House.", 3);
                            data.AddOption("Just passing by.", 255).AddAvatar(7).FinalizeDialog();
                        }
                        else if (client.MyHouse.Level == 1)
                        {
                            data.AddText("What can i do for you?");
                            data.AddOption("Visit house.", 4);
                            data.AddOption("Upgrade house.", 5);
                            data.AddOption("buy house certificate.", 2);
                            data.AddOption("Visit my wife`s House.", 3);
                            data.AddOption("Just passing by.", 255).AddAvatar(7).FinalizeDialog();

                        }
                        else if (client.MyHouse.Level == 5)
                        {
                            data.AddText("As the owner of a Class 5 house, you`ll be able to hold a Grand Wedding. Additionally, you can upgrade it to Class 6")
                                .AddText("~to get a more luxurious living space and more services.");
                            data.AddOption("I~wanna~go~home.", 4);
                            if (client.MyHouse.Level < 6)
                            {
                                data.AddOption("Upgrade~to~Class~6.", 20);
                                data.AddOption("Remove All furnitures.", 6);
                            }
                            data.AddOption("Visit my wife`s House.", 3);
                            data.AddOption("Just passing by.", 255).AddAvatar(7).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("What can i do for you?");
                            data.AddOption("Visit house.", 4);
                            data.AddOption("Upgrade house.", 5);
                            data.AddOption("Remove All furnitures.", 6);
                            if (client.MyHouse.Level < 5)
                                data.AddOption("buy house certificate.", 2);
                            data.AddOption("Visit my wife`s House.", 3);
                            data.AddOption("Just passing by.", 255).AddAvatar(7).FinalizeDialog();

                        }

                        break;
                    }
                case 20:
                    {
                        data.AddText("To upgrade your house to Class 6, you need to talk to the Class 6 House Admin in the Class 6 model house. I cannot help you in this matter.");
                        data.AddOption("Okay,~send~me~there.", 21);
                        data.AddOption("Just~passing~by.", 255).AddAvatar(7).FinalizeDialog();
                        break;
                    }
                case 21:
                    {

                        client.Teleport(127, 138, 3995, 0);
                        break;
                    }
                case 6:
                    {
                        if (client.MyHouse != null)
                        {
                            client.MyHouse.Furnitures.Clear();

                            data.AddText("You furnitures remove Successful")
                                .AddOption("Thank~you", 255)
                            .AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }
                case 1:
                    {
                        if (client.MyHouse == null)
                        {
                            if (client.Inventory.Remove(721170, 1, stream))
                            {
                                client.HeroRewards.AddGoal(704);

                                client.MyHouse = new Role.Instance.House(client.Player.UID);
                                data.AddText("You're create a house Successful. ")
                               .AddOption("Thank~you", 255).AddAvatar(7).FinalizeDialog();

                            }
                            else
                            {
                                data.AddText("Sorry you not have house Permit")
                                .AddOption("Ok", 255).AddAvatar(7).FinalizeDialog();

                            }

                        }


                        break;
                    }
                case 5:
                    {
                        if (client.MyHouse == null) break;

                        if (client.MyHouse.Level == 0)
                        {
                            data.AddText("You don`t have house.")
                              .AddOption("Ok", 255).AddAvatar(7).FinalizeDialog();
                            break;
                        }
                        ushort House_ID = Database.HouseTable.GetHouseId(client.MyHouse.Level);
                        if (House_ID != 0)
                        {
                            //remove players house
                            foreach (var pClient in Pool.GamePoll.Values)
                                if (pClient.Player.Map == House_ID && pClient.Player.DynamicID == client.Player.UID)
                                    pClient.Teleport(1036, 200, 200);
                        }
                        if (client.MyHouse.Furnitures.Count != 0)
                        {
                            data.AddText("Please remove all your furnitures from your house.")
                               .AddOption("Ok", 255).AddAvatar(7).FinalizeDialog();
                            break;
                        }
                        if (client.MyHouse.Level == 1)
                        {
                            if (client.Inventory.Remove(721174, 1, stream))
                            {
                                client.MyHouse.Level++;
                                data.AddText("You have successfully updated your House.")
                             .AddOption("Thank you.", 255).AddAvatar(7).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you not have Upgrade Cert for update.")
                           .AddOption("Ah Ok", 255).AddAvatar(7).FinalizeDialog();
                            }
                        }
                        else if (client.MyHouse.Level == 2)
                        {
                            if (client.Inventory.Remove(711198, 1, stream))
                            {
                                client.MyHouse.Level++;
                                data.AddText("You have successfully updated your House.")
                             .AddOption("Thank you.", 255).AddAvatar(7).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you not have Upgrade Cert(class 3) for update.")
                                     .AddOption("Ah Ok", 255).AddAvatar(7).FinalizeDialog();
                            }
                        }
                        else if (client.MyHouse.Level == 3)
                        {
                            if (client.Inventory.Remove(711200, 1, stream))
                            {
                                client.MyHouse.Level++;
                                data.AddText("You have successfully updated your House.")
                             .AddOption("Thank you.", 255).AddAvatar(7).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you not have Upgrade Cert(class 4) for update.")
                                       .AddOption("Ah Ok", 255).AddAvatar(7).FinalizeDialog();
                            }
                        }
                        else if (client.MyHouse.Level == 4)
                        {
                            if (client.Inventory.Remove(711553, 1, stream))
                            {
                                client.MyHouse.Level++;
                                data.AddText("You have successfully updated your House.")
                             .AddOption("Thank you.", 255).AddAvatar(7).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you not have Upgrade Cert(class 5) for update.")
                                     .AddOption("Ah Ok", 255).AddAvatar(7).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You house is the best level")
                                      .AddOption("Ah Ok", 255).AddAvatar(7).FinalizeDialog();
                        }



                        break;
                    }
                case 4:
                    {
                        if (client.MyHouse == null)
                            break;
                        if (client.MyHouse.Level == 0)
                        {
                            data.AddText("You don`t have house.")
                              .AddOption("Ok", 255).AddAvatar(7).FinalizeDialog();
                            break;
                        }
                        ushort HouseID = 0;
                        ushort house_x = 0;
                        ushort house_y = 0;
                        if (client.MyHouse.Level == 1)
                        {
                            HouseID = 1098;
                            house_x = 32;//3
                            house_y = 39;//40
                        }
                        else if (client.MyHouse.Level == 2)
                        {
                            HouseID = 1099;
                            house_x = 53;
                            house_y = 83;
                        }
                        else if (client.MyHouse.Level == 3)
                        {
                            HouseID = 2080;
                            house_x = 78;
                            house_y = 57;
                        }
                        else if (client.MyHouse.Level == 4)
                        {
                            HouseID = 601;
                            house_x = 80;
                            house_y = 58;
                        }
                        else if (client.MyHouse.Level == 5)
                        {
                            HouseID = 3024;
                            house_x = 105;
                            house_y = 139;
                        }
                        else if (client.MyHouse.Level == 6)
                        {
                            HouseID = 3995;
                            house_x = 127;
                            house_y = 138;
                        }
                        if (HouseID != 0)
                        {

                            if (client.Team != null)
                            {
                                foreach (var members in client.Team.Temates)
                                {
                                    if (members.client != null)
                                    {
                                        if (members.client.Player.UID != client.Player.UID)
                                        {
                                            if (Role.Core.GetDistance(client.Player.X, client.Player.Y, members.client.Player.X, members.client.Player.Y) < 18)
                                            {
                                                members.client.Teleport(house_x, house_y, HouseID, client.Player.UID);
                                            }
                                        }
                                    }
                                }
                            }
                            client.Teleport(house_x, house_y, HouseID, client.Player.UID);
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Player.SpouseUID != 0)
                        {
                            if (Pool.GamePoll.ContainsKey(client.Player.SpouseUID))
                            {
                                var MySpouse = Pool.GamePoll[client.Player.SpouseUID];
                                if (MySpouse.MyHouse != null)
                                {
                                    ushort HouseID = 0;
                                    ushort house_x = 0;
                                    ushort house_y = 0;
                                    if (MySpouse.MyHouse.Level == 1)
                                    {
                                        HouseID = 1098;
                                        house_x = 33;
                                        house_y = 40;
                                    }
                                    else if (MySpouse.MyHouse.Level == 2)
                                    {
                                        HouseID = 1099;
                                        house_x = 53;
                                        house_y = 83;
                                    }
                                    else if (MySpouse.MyHouse.Level == 3)
                                    {
                                        HouseID = 2080;
                                        house_x = 78;
                                        house_y = 57;
                                    }
                                    else if (MySpouse.MyHouse.Level == 4)
                                    {
                                        HouseID = 601;
                                        house_x = 80;
                                        house_y = 58;
                                    }
                                    else if (MySpouse.MyHouse.Level == 5)
                                    {
                                        HouseID = 3024;
                                        house_x = 105;
                                        house_y = 139;
                                    }
                                    else if (client.MyHouse.Level == 6)
                                    {
                                        HouseID = 3995;
                                        house_x = 127;
                                        house_y = 138;
                                    }
                                    client.Teleport(house_x, house_y, HouseID, MySpouse.Player.UID);
                                }
                                else
                                {
                                    data.AddText("Sorry, but you vife not have House")
                                                              .AddOption("Ah Ok", 255).AddAvatar(7).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Sorry, but you vife is not online.")
                                                              .AddOption("Ah Ok", 255).AddAvatar(7).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Sorry, but you vife not have House")
                                   .AddOption("Ah Ok", 255).AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }
                case 2:
                    {
                        data.AddText("Do you want a memorable wedding ceremony? it`s not easy, you know However,");
                        data.AddText("I can help you with that. First of all, we need a professional emcee.");
                        data.AddOption("House Permit: 99 CPs.", 10);
                        data.AddOption("Upgrade Cert: 199 Cps", 11);
                        data.AddOption("UpgradeCert(Class 3): 259 CPs", 12);
                        data.AddOption("UpgradeCert(Class 4): 299 CPs", 13);
                        data.AddOption("UpgradeCert(Class 5): 9999 CPs", 14);
                        data.AddOption("Let me think about it.", 255).AddAvatar(7).FinalizeDialog();


                        break;
                    }
                case 10:
                    {
                        ushort cost = 99;
                        if (client.Player.ConquerPoints >= cost)
                        {
                            if (client.InTrade) return;
                            if (client.Inventory.Add(stream, 721170))
                            {
                                client.Player.ConquerPoints -= cost;

                            }
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough CPs to buy that.")
                        .AddOption("Alright.", 255).AddAvatar(7).FinalizeDialog();
                        }



                        break;
                    }
                case 11:
                    {
                        ushort cost = 199;
                        if (client.Player.ConquerPoints >= cost)
                        {
                            if (client.InTrade) return;
                            if (client.Inventory.Add(stream, 721174))
                            {
                                client.Player.ConquerPoints -= cost;

                            }
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough CPs to buy that.")
                     .AddOption("Alright.", 255).AddAvatar(7).FinalizeDialog();
                        }



                        break;
                    }
                case 12:
                    {
                        ushort cost = 259;
                        if (client.Player.ConquerPoints >= cost)
                        {
                            if (client.InTrade) return;
                            if (client.Inventory.Add(stream, 711198))
                            {
                                client.Player.ConquerPoints -= cost;

                            }
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough CPs to buy that.")
                   .AddOption("Alright.", 255).AddAvatar(7).FinalizeDialog();
                        }



                        break;
                    }
                case 13:
                    {
                        ushort cost = 299;
                        if (client.Player.ConquerPoints >= cost)
                        {
                            if (client.InTrade) return;
                            if (client.Inventory.Add(stream, 711200))
                            {
                                client.Player.ConquerPoints -= cost;

                            }
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough CPs to buy that.")
                   .AddOption("Alright.", 255).AddAvatar(7).FinalizeDialog();
                        }



                        break;
                    }
                case 14:
                    {
                        ushort cost = 9999;
                        if (client.Player.ConquerPoints >= cost)
                        {
                            if (client.InTrade) return;
                            if (client.Inventory.Add(stream, 711553))
                            {
                                client.Player.ConquerPoints -= cost;

                            }
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough CPs to buy that.")
                   .AddOption("Alright.", 255).AddAvatar(7).FinalizeDialog();
                        }

                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.BarberSha)]
        public static void BarberSha(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (client.IsVendor)
            {
                client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                return;
            }
            if (client.InTrade)
            {
                client.CreateBoxDialog("Sorry, You Must Close The Trade First.");
                return;
            }
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.Pharmacist, client.Player.Class, 815);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)id, client.Player.Class);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.IsVendor)
                        {
                            client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        if (client.InTrade)
                        {
                            client.CreateBoxDialog("Sorry, You Must Close The Trade First.");
                            return;
                        }
                        if (client.Player.QuestGUI.IsActiveQuest(ActiveQuest.MissionId))
                        {
                            data.AddText("I`m capable of doing more hairstyles now! Tell me the style you like!")
                           .AddOption("You~can~prettify~my~face?", 103)
                           .AddOption("I~want~to~change~hair~style!", 1)
                           .AddAvatar(64).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.IsActiveQuest(ActiveQuest2.MissionId))
                        {
                            data.AddText("Of course I can. I can even help you change your appearance! Come to me if you need my service! I will give you a discount!")
                 .AddOption("Tell~me~more.", 101)
                   .AddOption("I~want~to~change~hair~style!.", 1).AddAvatar(64).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            goto case 100;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("Of course I can. I can even help you change your appearance! Come to me if you need my service! I will give you a discount!")
                .AddOption("Tell~me~more.", 105)
                  .AddOption("I~want~to~change~hair~style!.", 100)
                .AddAvatar(64).FinalizeDialog();


                        }
                        goto case 100;

                    }
                case 105:
                    {
                        data.AddText("Hm, I guess you will need to store your items in the Warehouse in the future! Pay a visit to the Warehouseman, you may need his help!")
                                    .AddOption("Thanks!.", 255)
                                    .AddAvatar(64).FinalizeDialog();

                        break;
                    }
                case 103:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.SendSysMesage("You received 100 minute(s) of EXP.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                            data.AddText("Of course I can. I can even help you change your appearance! Come to me if you need my service! I will give you a discount!")
              .AddOption("Tell~me~more.", 101)
                .AddOption("I~want~to~change~hair~style!.", 100)
              .AddAvatar(64).FinalizeDialog();
                        }

                        break;
                    }
                case 101:
                    {
                        data.AddText("Hm, I guess you will need to store your items in the Warehouse in the future! Pay a visit to the Warehouseman, you may need his help!")
           .AddOption("Thanks!.", 102)
           .AddAvatar(64).FinalizeDialog();


                        break;
                    }
                case 102:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher("The~Barber~asked~you~to~speak~with~the~Warehouseman~in~the~Twin~City!", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);


                        break;
                    }
                case 100:
                    {
                        if (client.IsVendor)
                        {
                            client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        if (client.InTrade)
                        {
                            client.CreateBoxDialog("Sorry, You Must Close The Trade First.");
                            return;
                        }
                        //data.AddText("<tip desc=Sha#has#opened#this#barber#shop#after#winning#the#1st#TC#Hair#Design###############Championship.Though#charge#a#lot,#Sha#still#has#many#loyal#customers#who#come#for#the#hottest#hairstyles.#");
                        data.AddText("Barber~Sha:\n");
                        data.AddText("Want to try a new hairstyle? Get one of the hottest haircuts, and you`ll look fantastic!")

                        //.AddOption("New~Dynasty~Hairstyles.", 1)
                        .AddOption("New~styles.", 2)
                        .AddOption("Nostalgic~styles.", 3)
                        .AddOption("(Level~4+)~VIP~Hairstyles.", 6)
                        .AddOption("(Level~6+)~VIP~Hairstyles.", 9)
                        .AddOption("I`ll think about it.", 255)
                        .AddAvatar(64).FinalizeDialog();

                        break;
                    }


                case 1:
                    {
                        data.AddText("Which style would you like to change to?")

                      .AddOption("New~Dynasty~Hairstyle~01", 10)
                      .AddOption("New~Dynasty~Hairstyle~02", 11)
                      .AddOption("New~Dynasty~Hairstyle~03", 12)
                      .AddOption("New~Dynasty~Hairstyle~04", 13)
                      .AddOption("New~Dynasty~Hairstyle~05", 14)
                      .AddOption("Next~Page.", 4)
                      .AddAvatar(64).FinalizeDialog();



                        break;
                    }
                case 75:
                    {
                        if (client.Player.Money < 10000)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 10000)
                            {
                                client.Player.Money -= 10000;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 75);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It was completed. Are you satisfied with your new hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 1);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 76:
                    {
                        if (client.Player.Money < 10000)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 10000)
                            {
                                client.Player.Money -= 10000;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 76);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It was completed. Are you satisfied with your new hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 1);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 77:
                    {
                        if (client.Player.Money < 10000)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 10000)
                            {
                                client.Player.Money -= 10000;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 77);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It was completed. Are you satisfied with your new hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 1);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 78:
                    {
                        if (client.Player.Money < 10000)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 10000)
                            {
                                client.Player.Money -= 10000;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 78);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It was completed. Are you satisfied with your new hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 1);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 79:
                    {
                        if (client.Player.Money < 10000)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 10000)
                            {
                                client.Player.Money -= 10000;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 79);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It was completed. Are you satisfied with your new hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 1);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 80:
                    {
                        if (client.Player.Money < 10000)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 10000)
                            {
                                client.Player.Money -= 10000;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 80);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It was completed. Are you satisfied with your new hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 1);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 81:
                    {
                        if (client.Player.Money < 10000)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 10000)
                            {
                                client.Player.Money -= 10000;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 81);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It was completed. Are you satisfied with your new hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 1);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 82:
                    {
                        if (client.Player.Money < 10000)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 10000)
                            {
                                client.Player.Money -= 10000;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 82);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It was completed. Are you satisfied with your new hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 1);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 83:
                    {
                        if (client.Player.Money < 10000)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 10000)
                            {
                                client.Player.Money -= 10000;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 83);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It was completed. Are you satisfied with your new hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 1);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 84:
                    {
                        if (client.Player.Money < 10000)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 10000)
                            {
                                client.Player.Money -= 10000;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 84);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It was completed. Are you satisfied with your new hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 1);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 85:
                    {
                        if (client.Player.Money < 10000)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 10000)
                            {
                                client.Player.Money -= 10000;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 85);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It was completed. Are you satisfied with your new hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 1);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 86:
                    {
                        if (client.Player.Money < 10000)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 10000)
                            {
                                client.Player.Money -= 10000;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 86);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It was completed. Are you satisfied with your new hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 1);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 87:
                    {
                        if (client.Player.Money < 10000)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 10000)
                            {
                                client.Player.Money -= 10000;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 87);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It was completed. Are you satisfied with your new hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 1);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 88:
                    {
                        if (client.Player.Money < 10000)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 10000)
                            {
                                client.Player.Money -= 10000;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 88);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It was completed. Are you satisfied with your new hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 1);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 89:
                    {
                        if (client.Player.Money < 10000)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 10000)
                            {
                                client.Player.Money -= 10000;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 89);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It was completed. Are you satisfied with your new hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 1);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 90:
                    {
                        if (client.Player.Money < 10000)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 10000)
                            {
                                client.Player.Money -= 10000;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 90);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It was completed. Are you satisfied with your new hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 1);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 16:
                    {
                        if (client.Player.Money < 10000)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 10000)
                            {
                                client.Player.Money -= 10000;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 48);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It was completed. Are you satisfied with your new hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 1);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 15:
                    {
                        if (client.Player.Money < 10000)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 10000)
                            {
                                client.Player.Money -= 10000;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 47);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It was completed. Are you satisfied with your new hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 1);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 14:
                    {
                        if (client.Player.Money < 10000)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 10000)
                            {
                                client.Player.Money -= 10000;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 46);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It was completed. Are you satisfied with your new hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 1);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 13:
                    {
                        if (client.Player.Money < 10000)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 10000)
                            {
                                client.Player.Money -= 10000;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 45);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It was completed. Are you satisfied with your new hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 1);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 12:
                    {
                        if (client.Player.Money < 10000)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 10000)
                            {
                                client.Player.Money -= 10000;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 44);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It was completed. Are you satisfied with your new hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 1);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 11:
                    {
                        if (client.Player.Money < 10000)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 10000)
                            {
                                client.Player.Money -= 10000;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 43);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It was completed. Are you satisfied with your new hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 1);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 10:
                    {
                        if (client.Player.Money < 10000)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 10000)
                            {
                                client.Player.Money -= 10000;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 42);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It was completed. Are you satisfied with your new hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 1);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                default:
                    {
                        if (client.Player.Hair < 300)
                            client.Player.Hair = 300;
                        ushort NewHaire = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + Option);
                        if (client.Player.Money >= 500)
                        {
                            client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                            client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                            client.Player.Money -= 500;
                            client.Player.Hair = NewHaire;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                            client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                            client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                        }
                        else
                        {
                            data.AddText("You do not have 500 silvers with you.")
                  .AddOption("I~see.", 255)
                  .AddAvatar(64).FinalizeDialog();
                        }

                        break;
                    }
                case 17:
                    {
                        if (client.Player.Money < 10000)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 10000)
                            {
                                client.Player.Money -= 10000;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 49);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It was completed. Are you satisfied with your new hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 1);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 21:
                    {
                        if (client.Player.Money < 10000)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 10000)
                            {
                                client.Player.Money -= 10000;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 50);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It was completed. Are you satisfied with your new hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 1);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 22:
                    {
                        if (client.Player.Money < 10000)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 10000)
                            {
                                client.Player.Money -= 10000;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 51);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It was completed. Are you satisfied with your new hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 1);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 56:
                    {
                        if (client.Player.Money < 500)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 500)
                            {
                                client.Player.Money -= 500;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 70);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("Done! What do you think? Do you like it?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 6);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 57:
                    {
                        if (client.Player.Money < 500)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 500)
                            {
                                client.Player.Money -= 500;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 71);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("Done! What do you think? Do you like it?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 6);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 70:
                    {
                        if (client.Player.Money < 500)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 500)
                            {
                                client.Player.Money -= 500;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 72);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("Done! What do you think? Do you like it?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 6);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 71:
                    {
                        if (client.Player.Money < 500)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 500)
                            {
                                client.Player.Money -= 500;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 73);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("Done! What do you think? Do you like it?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 6);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 72:
                    {
                        if (client.Player.Money < 500)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 500)
                            {
                                client.Player.Money -= 500;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 74);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("Done! What do you think? Do you like it?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 6);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 6:
                    {
                        if (client.Player.VipLevel < 4)
                        {
                            data.AddText("Sorry, your VIP Level has to be at least Level 4 to purchase this hairstyle.");
                            data.AddOption("Alright.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You can try out the different hairstyles, until you find your favorite!")
                        .AddOption("(Level~4+)~VIP~Hairstyle~1.", 56)
                        .AddOption("(Level~4+)~VIP~Hairstyle~2.", 57)
                        .AddOption("(Level~4+)~VIP~Hairstyle~3.", 70)
                        .AddOption("(Level~4+)~VIP~Hairstyle~4.", 71)
                        .AddOption("(Level~4+)~VIP~Hairstyle~5.", 72)
                                //  .AddOption("Special~HairStyle06", 73)
                                // .AddOption("Special~HairStyle07", 74)
                       .AddOption("I~won`t~change~my~hairstyle.")
                        .AddAvatar(64).FinalizeDialog();

                        }
                        break;
                    }
                case 9:
                    {
                        if (client.Player.VipLevel < 6)
                        {
                            data.AddText("Sorry, your VIP Level has to be at least Level 6 to purchase this hairstyle.");
                            data.AddOption("Alright.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You can try out the different hairstyles, until you find your favorite!")
                        .AddOption("VIP Hairstyle 1", 75)
                                                    .AddOption("VIP Hairstyle 2", 76)
                                                    .AddOption("VIP Hairstyle 3", 77)
                                                    .AddOption("VIP Hairstyle 4", 78)
                                                    .AddOption("VIP Hairstyle 5", 79)
                                                    .AddOption("VIP Hairstyle 6", 80)
                                                    .AddOption("VIP Hairstyle 7", 81)
                                                    .AddOption("VIP Hairstyle 8", 82)
                                                   .AddOption("VIP Hairstyle 9", 83)
                                                   .AddOption("VIP Hairstyle 10", 84)
                                                    .AddOption("VIP Hairstyle 11", 85)
                                                   .AddOption("VIP Hairstyle 12", 86)
                                                   .AddOption("VIP Hairstyle 13", 87)
                                                    .AddOption("VIP Hairstyle 14", 88)
                                                    .AddOption("VIP Hairstyle 15", 89)
                                                   .AddOption("VIP Hairstyle 16", 90)
                       .AddOption("I~won`t~change~my~hairstyle.")
                        .AddAvatar(64).FinalizeDialog();

                        }
                        break;
                    }
                case 4:
                    {
                        data.AddText("Which style would you like to change to?")
                      .AddOption("New~Dynasty~Hairstyle~06", 15)
                      .AddOption("New~Dynasty~Hairstyle~07", 16)
                     .AddOption("New~Dynasty~Hairstyle~08", 17)
                     .AddOption("New~Dynasty~Hairstyle~09", 21)
                     .AddOption("New~Dynasty~Hairstyle~10", 22)
                     .AddOption("Previous.", 1)
                     .AddAvatar(64).FinalizeDialog();


                        break;
                    }
                case 30:
                    {
                        if (client.Player.Money < 500)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 500)
                            {
                                client.Player.Money -= 500;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 30);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It`s~completed.~Are~you~satisfied~with~your~new~hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 2);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 31:
                    {
                        if (client.Player.Money < 500)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 500)
                            {
                                client.Player.Money -= 500;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 33);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It`s~completed.~Are~you~satisfied~with~your~new~hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 2);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 32:
                    {
                        if (client.Player.Money < 500)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 500)
                            {
                                client.Player.Money -= 500;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 34);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It`s~completed.~Are~you~satisfied~with~your~new~hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 2);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 33:
                    {
                        if (client.Player.Money < 500)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 500)
                            {
                                client.Player.Money -= 500;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 35);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It`s~completed.~Are~you~satisfied~with~your~new~hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 2);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 34:
                    {
                        if (client.Player.Money < 500)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 500)
                            {
                                client.Player.Money -= 500;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 36);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It`s~completed.~Are~you~satisfied~with~your~new~hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 2);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 35:
                    {
                        if (client.Player.Money < 500)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 500)
                            {
                                client.Player.Money -= 500;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 37);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It`s~completed.~Are~you~satisfied~with~your~new~hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 2);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 36:
                    {
                        if (client.Player.Money < 500)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 500)
                            {
                                client.Player.Money -= 500;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 38);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It`s~completed.~Are~you~satisfied~with~your~new~hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 2);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 37:
                    {
                        if (client.Player.Money < 500)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 500)
                            {
                                client.Player.Money -= 500;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 39);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It`s~completed.~Are~you~satisfied~with~your~new~hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 2);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 38:
                    {
                        if (client.Player.Money < 500)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 500)
                            {
                                client.Player.Money -= 500;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 40);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It`s~completed.~Are~you~satisfied~with~your~new~hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 2);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 2:
                    {
                        data.AddText("Which~style~would~you~like~to~select~from?")
                                                .AddOption("New~HairStyle01", 30)
                                                .AddOption("New~HairStyle02", 31)
                                                .AddOption("New~HairStyle03", 32)
                                                .AddOption("New~HairStyle04", 33)
                                                .AddOption("New~HairStyle05", 34)
                                                .AddOption("New~HairStyle06", 35)
                            //  .AddOption("New~HairStyle07", 36)
                                                .AddOption("Next~Page.", 5)
                                                .AddAvatar(64).FinalizeDialog();


                        break;
                    }
                case 5:
                    {
                        data.AddText("Which~style~would~you~like~to~select~from?")
                            //.AddOption("New~HairStyle08", 37)
                            // .AddOption("New~HairStyle09", 38)
                            // .AddOption("New~HairStyle10", 39)
                            // .AddOption("New~HairStyle11", 40)
                            // .AddOption("New~HairStyle12", 41)
                            //  .AddOption("New~HairStyle13", 42)
                            //.AddOption("New~HairStyle14", 43)
                         .AddOption("Previous.", 2)
                         .AddAvatar(64).FinalizeDialog();

                        break;
                    }
                case 44:
                    {
                        if (client.Player.ConquerPoints < 1500)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.ConquerPoints >= 1500)
                            {
                                client.Player.ConquerPoints -= 1500;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 10);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It`s~completed.~Are~you~satisfied~with~your~new~hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 3);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 45:
                    {
                        if (client.Player.Money < 500)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 500)
                            {
                                client.Player.Money -= 500;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 11);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It`s~completed.~Are~you~satisfied~with~your~new~hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 3);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 46:
                    {
                        if (client.Player.Money < 500)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 500)
                            {
                                client.Player.Money -= 500;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 12);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It`s~completed.~Are~you~satisfied~with~your~new~hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 3);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 47:
                    {
                        if (client.Player.Money < 500)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 500)
                            {
                                client.Player.Money -= 500;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 13);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It`s~completed.~Are~you~satisfied~with~your~new~hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 3);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 48:
                    {
                        if (client.Player.Money < 500)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 500)
                            {
                                client.Player.Money -= 500;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 14);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It`s~completed.~Are~you~satisfied~with~your~new~hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 3);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        break;

                    }
                case 49:
                    {
                        if (client.Player.Money < 500)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 500)
                            {
                                client.Player.Money -= 500;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 15);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It`s~completed.~Are~you~satisfied~with~your~new~hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 3);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 50:
                    {
                        if (client.Player.Money < 500)
                        {
                            data.AddText("You~have~not~enough~money.");
                            data.AddOption("I`m~sorry.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Money >= 500)
                            {
                                client.Player.Money -= 500;

                                client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + 0);
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "Health");
                                client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Sound, 0, false, "sound/gethp.wav", "1");
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                data.AddText("It`s~completed.~Are~you~satisfied~with~your~new~hairstyle?");
                                data.AddOption("Cool!");
                                data.AddOption("I~want~to~change~it.", 3);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        data.AddText("Which style would you like to change to?")
                                            .AddOption("Nostalgic01", 44)
                                            .AddOption("Nostalgic02", 45)
                                            .AddOption("Nostalgic03", 46)
                                            .AddOption("Nostalgic04", 47)
                                            .AddOption("Nostalgic05", 48)
                                            .AddOption("Nostalgic06", 49)
                                            .AddOption("Bald~head", 50)
                            //.AddOption("Nostalgic08", 51)
                                           .AddOption("I~chanded~my~mind.")
                                            .AddAvatar(64).FinalizeDialog();


                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.MailManager)]
        public static void MailManager(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            stream = rec.GetStream();
                            ActionQuery action = new ActionQuery()
                            {
                                Type = ActionType.OpenDialog,
                                ObjId = client.Player.UID,
                                dwParam = 576,
                                PositionX = client.Player.X,
                                PositionY = client.Player.Y,

                            };
                            client.Send(stream.ActionCreate(action));
                        }

                        break;
                    }
            }
        }
        //[NpcAttribute(NpcID.HairStyles)]
        //public static void HairStyles(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog data = new Dialog(client, stream);

        //    switch (Option)
        //    {

        //        case 0:
        //            {
        //                data.AddText("Hello You Can Open All HairStyles For 1.000.000.000 CPs.");
        //                data.AddOption("Open All HairStyles.", 1);
        //                data.AddOption("I~will~come~later.", 255);
        //                data.FinalizeDialog();

        //                break;
        //            }
        //        case 1:
        //            {
        //                if (client.Player.ConquerPoints >= 1000000000)
        //                {
        //                    client.Player.ConquerPoints -= 1000000000;
        //                    var AllFrame = Database.HairfaceStorageTable.Hairfaces.Where(i => i.Type == MsgHairfaceStorage.Type.Hairstyle).ToArray();

        //                    data.Text("Congratulations! You have successfully unlocked all your hairstyles for 1.000.000.000 CPs.");

        //                    data.Option("Thanks.", 255).FinalizeDialog();

        //                    foreach (var AddAllFrame in AllFrame)
        //                    {
        //                        client.HairfaceStorage.Add(AddAllFrame, true);
        //                    }
        //                }
        //                else
        //                {
        //                    data.Text("Sorry, You Don`t Have 1.000.000.000 CPs In Your Inventory !");

        //                    data.Option("Okay Thanks.", 255).FinalizeDialog();
        //                }
        //                break;
        //            }

        //    }
        //}
        [NpcAttribute(NpcID.GuildGateKeeper)]
        public static void GuildGateKeeper(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Are~you~going~to~leave~here?")
                       .AddOption("Yes.", 1)
                       .AddOption("No.~Wait~a~moment.", 255)
                       .AddAvatar(7).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        client.Teleport(Places.TwinCity);
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.GuildConductor)]
        public static void GuildConductor(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello, How Can I Help You !")
                                                .AddOption("Enter~the~guild~area.", 1)
                                                .AddOption("Buy~statue.", 255)
                                                .AddOption("Just passing by.", 255)
                                                .AddAvatar(123).FinalizeDialog();

                        break;
                    }               
                case 1:
                    {
                        if (Game.MsgTournaments.MsgSchedules.GuildWar.Proces == MsgTournaments.ProcesType.Alive)
                        {
                            byte RandTele = (byte)Pool.GetRandom.Next(1, 14);
                            switch (RandTele)
                            {
                                case 1: client.Teleport(326, 374, 1038); break;
                                case 2: client.Teleport(299, 371, 1038); break;
                                case 3: client.Teleport(293, 352, 1038); break;
                                case 4: client.Teleport(262, 337, 1038); break;
                                case 5: client.Teleport(285, 324, 1038); break;
                                case 6: client.Teleport(315, 289, 1038); break;
                                case 7: client.Teleport(323, 268, 1038); break;
                                case 8: client.Teleport(351, 282, 1038); break;
                                case 9: client.Teleport(371, 305, 1038); break;
                                case 10: client.Teleport(370, 326, 1038); break;
                                case 11: client.Teleport(357, 335, 1038); break;
                                case 12: client.Teleport(333, 330, 1038); break;
                                default: client.Teleport(322, 321, 1038); break;
                            }
                        }
                        else
                        {
                            client.Teleport(349, 340, 1038);
                        }
                        if (client.Player.ContainFlag(MsgUpdate.Flags.Ride))
                            client.Player.RemoveFlag(MsgUpdate.Flags.Ride);
                        break;
                    }               

            }
        }
        [NpcAttribute(NpcID.CaptureTheFlag)]
        public static void CaptureTheFlag(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("The Capture the Flag is held every Saturday 21:00 to 22:00\n")
                        .AddOption("Sign~me~up.", 150)
                        .AddOption("Just passing by.", 255).AddAvatar(107).FinalizeDialog();
                        break;
                    }
                case 100:
                    {
                        // [OfficialFix 2026-10-14] the old claim paid the guild leader of ranks 1/2/3 a free 500,000 / 400,000 /
                        // 300,000 CPs. Officially the CTF bonus is not created by an NPC: the winning guild sets the next
                        // preliminary bonus from its own guild funds (client STR_WARFLAG_REWARD_SET, tBattlePassTaskTest[36771];
                        // server MsgCaptureTheFlagRankings SetReward*). The option is gone; this answers an old dialog.
                        data.AddText("The Capture the Flag bonus is set by the guild leaders from their guild funds in the Capture the Flag panel. I do not hand out any other prize.")
                        .AddOption("I~see.", 255).AddAvatar(107).FinalizeDialog();
                        break;
                    }
                case 150:
                    {
                        client.Player.ProtectAttack(5 * 1000); // 5 Seconds


                        if (client.Player.MyGuild == null)
                        {
                            data.AddText("Sorry, You must have a guild to be able to go to Capture the Flag.")
                    .AddOption("Alright.", 255).AddAvatar(107).FinalizeDialog();
                            break;
                        }
                        if (!MsgTournaments.MsgSchedules.CaptureTheFlag.Join(client, stream))
                        {
                            data.AddText("It`s not the time for the Capture the Flag contest. Come see me at 21:00 each Saturday, and sign up!")
                     .AddOption("Alright.", 255).AddAvatar(107).FinalizeDialog();
                        }
                        break;
                    }
            }
        }


        [NpcAttribute(NpcID.Tinter)]
        public static void Tinter(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Welcome!~Please~wear~the~armors~first.~What~armors~do~you~want~me~to~dye?")
                        .AddOption("Cap", 1)
                        .AddOption("Coat", 2)
                        .AddAvatar(7).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("There~are~World~colors~for~you~to~choose.~You~can~try~all~the~colors~for~no~more~charges.~What~color~do~you~like~best?")
                       .AddOption("Color~1", 13)
                       .AddOption("Color~2", 14)
                       .AddOption("Color~3", 15)
                       .AddOption("Color~4", 16)
                       .AddOption("Color~5", 17)
                       .AddOption("Color~6", 18)
                       .AddOption("Color~7", 19)
                       .AddAvatar(7).FinalizeDialog();
                        break;
                    }
                case 13:
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                case 19:
                    {
                        MsgServer.MsgGameItem Coat;
                        if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.Armor, out Coat))
                        {
                            Coat.Color = (Role.Flags.Color)(Option - 10);
                            Coat.Mode = Role.Flags.ItemMode.Update;
                            Coat.Send(client, stream);
                            client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                        }
                        break;
                    }
                case 1:
                    {
                        data.AddText("There~are~World~colors~for~you~to~choose.~You~can~try~all~the~colors~for~no~more~charges.~What~color~do~you~like~best?")
                        .AddOption("Color~1", 3)
                        .AddOption("Color~2", 4)
                        .AddOption("Color~3", 5)
                        .AddOption("Color~4", 6)
                        .AddOption("Color~5", 7)
                        .AddOption("Color~6", 8)
                        .AddOption("Color~7", 9)
                        .AddAvatar(7).FinalizeDialog();
                        break;
                    }
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                    {
                        MsgServer.MsgGameItem Cap;
                        if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.Head, out Cap))
                        {
                            Cap.Color = (Role.Flags.Color)Option;
                            Cap.Mode = Role.Flags.ItemMode.Update;
                            Cap.Send(client, stream);
                            client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ArenaNpc)]
        public static void ArenaNpc(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("The~arena~is~open.~Welcome~to~challenge~other~people.~The~admission~fee~is~only~50~silver.")
                            .AddText("If~you~PK~in~the~arena,~you~will~not~gain~or~lose~any~experience~or~items~equipped,~and~will~get~revived~at~the~place~you~die.")
                            .AddText("The~Kungfu~circle~is~very~dangerous,~I~suggest~you~PK~in~area.")
                        .AddOption("Enter~the~arena.", 1)
                        .AddOption("Just~passing~by.", 255)
                        .AddAvatar(7).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        if (client.Player.Money >= 50)
                        {
                            client.Player.Money -= 50;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                            client.Teleport(51, 69, 1005);
                        }
                        else
                        {
                            data.AddText("You do not have 50 silvers with you.")
                      .AddOption("I~see.", 255)
                      .AddAvatar(7).FinalizeDialog();

                        }

                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.MasterXuan)]
        public static void MasterXuan(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello! May I help you?")
                                                 .AddOption("Yes!~Upgrade~my~sashes!", 2)
                                                 .AddOption("How~to~upgrade~my~sashes?", 1)
                                                 .AddOption("Nothing.", 255)
                                                 .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("You can pay 100 CPs to combine 2 Sashes (S) into a Sash (M), and pay 200 CPs to combine 2 Sashes (M) into a Sash (L).")
                                              .AddOption("I~see.", 255)
                                              .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        data.AddText("First of all, you should take all items out of the sashes. Otherwise, they will disappear after the upgrade. You will need to pay 100 CPs to combine 2 Sashes (S) into a Sash (M),")
                                                     .AddText("~or pay 200 CPs to combine 2 Sashes (M) into a Sash (L). Are you ready?")
                                                 .AddOption("Yes.~I~want~a~Sash(M)(100CPs).", 3)
                                                 .AddOption("Yes.~I~want~a~Sash(L)(200CPs).", 4)
                                                 .AddOption("Not~yet.", 255)
                                                 .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.Contain(1100003, 2) && client.Player.ConquerPoints >= 100)
                        {
                            List<MsgServer.MsgGameItem> Items;
                            if (client.Inventory.SearchItemByID(1100003, 2, out Items))
                            {
                                bool Allow = true;
                                foreach (var item in Items)
                                {
                                    if (item.Deposite.Count != 0)
                                    {
                                        Allow = false;

                                        break;
                                    }
                                }
                                if (Allow)
                                {
                                    if (client.InTrade) return;
                                    foreach (var item in Items)
                                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                                    client.Inventory.Add(stream, 1100006, 1);
                                    client.Player.ConquerPoints -= 100;

                                    data.AddText("You Sash was update Successful.")
                                        .AddOption("Thnks~you.", 255)
                                        .AddAvatar(7).FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("Buddy, you don`t have 2 Sashes (S), or your sashes are not empty. Remember, 2 Sashes (S) can be composed into a Sash (M).")
                                   .AddOption("Let~me~see.", 255)
                                   .AddAvatar(7).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Buddy, you don`t have 2 Sashes (S), or your sashes are not empty. Remember, 2 Sashes (S) can be composed into a Sash (M).")
                                     .AddOption("Let~me~see.", 255)
                                     .AddAvatar(7).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Buddy, you don`t have 2 Sashes (S), or your sashes are not empty. Remember, 2 Sashes (S) can be composed into a Sash (M).")
                                     .AddOption("Let~me~see.", 255)
                                     .AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.Contain(1100006, 2) && client.Player.ConquerPoints >= 200)
                        {
                            List<MsgServer.MsgGameItem> Items;
                            if (client.Inventory.SearchItemByID(1100006, 2, out Items))
                            {
                                bool Allow = true;
                                foreach (var item in Items)
                                {
                                    if (item.Deposite.Count != 0)
                                    {
                                        Allow = false;

                                        break;
                                    }
                                }
                                if (Allow)
                                {
                                    if (client.InTrade) return;
                                    foreach (var item in Items)
                                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                                    client.Inventory.Add(stream, 1100009, 1);
                                    client.Player.ConquerPoints -= 200;

                                    data.AddText("You Sash was update Successful.")
                                        .AddOption("Thnks~you.", 255)
                                        .AddAvatar(7).FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("Buddy, you don`t have 2 Sashes (M), or your sashes are not empty. Remember, 2 Sashes (M) can be composed into a Sash (L).")
                                     .AddOption("Let~me~see.", 255)
                                     .AddAvatar(7).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Buddy, you don`t have 2 Sashes (M), or your sashes are not empty. Remember, 2 Sashes (M) can be composed into a Sash (L).")
                                     .AddOption("Let~me~see.", 255)
                                     .AddAvatar(7).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Buddy, you don`t have 2 Sashes (M), or your sashes are not empty. Remember, 2 Sashes (M) can be composed into a Sash (L).")
                                     .AddOption("Let~me~see.", 255)
                                     .AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }

            }

        }
        [NpcAttribute(NpcID.MCaptain)]
        public static void MCaptain(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.TCCaptainTimes == 3)
                        {
                            data.AddText("You already helped me out today my warrior, please come back to me tomorrow!");
                            data.AddOption("Okay.", 255);
                            data.AddAvatar(102).FinalizeDialog();
                            break;
                        }
                        Game.MsgServer.MsgGameItem Jar;
                        if (client.Inventory.TryGetItem(client.DemonExterminator.ItemUID, out Jar))
                        {
                            if (Jar.Durability <= client.DemonExterminator.HuntKills)
                            {
                                client.GainExpBall(600, true, Role.Flags.ExperienceEffect.angelwing);
                                client.Inventory.Remove(750000, 1, stream);
                                client.DemonExterminator.HuntKills = 0;
                                client.DemonExterminator.ItemUID = 0;

                                string itemname = client.DemonExterminator.SpecialReward(client, stream);

                                data.AddText("Thanks for the help. The quest finished! You have gaine a " + itemname + "");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You didn't finish killing the amount of monsters.");
                                data.AddOption("ah ok.", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("We're always looking for brave adventurers to protect the city from monsters. Are you interesed helping?");
                        data.AddOption("Yes please!", 103);
                        data.AddOption("I`m busy.", 255);
                        data.AddAvatar(102).FinalizeDialog();


                        break;
                    }
                case 211:
                    {
                        client.Teleport(229, 258, 1011);
                        break;
                    }
                case 220:
                    {
                        client.Teleport(569, 620, 1020);
                        break;
                    }
                case 215:
                    {
                        client.Teleport(791, 568, 1015);
                        break;
                    }
                case 200:
                    {
                        client.Teleport(476, 634, 1000);
                        break;
                    }
                case 201:
                    {
                        client.Teleport(82, 321, 1000);
                        break;
                    }
                case 103:
                    {
                        var array = Database.InfoDemonExterminators.CityQuests[1001].Where(p => p.Level + 5 > client.Player.Level).ToArray();
                        if (array.Length == 0)
                        {
                            ushort next_map = Database.InfoDemonExterminators.GetNextMap((byte)client.Player.Level);
                            if (next_map == 0)
                            {
                                data.AddText("You're a bit too powerful for the monsters near here to be any real challenge.");
                                data.AddOption("Okay", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            else
                            {
                                var DBMap = Pool.ServerMaps[next_map];
                                data.AddText("You are powerfull now. I suggest you to go Captain of " + DBMap.Name + ", because i heard that is ")
                                              .AddText("annoyed by the rampant monsters there. I believe you can help a lot. I will teleport you in")
                                              .AddText(" " + DBMap.Name + ", so you can reach there very soon.");
                                data.AddOption("Please send me to " + DBMap.Name + "", (byte)(200 + (next_map % 100)));
                                data.AddOption("I don`d wana to go there.", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("" + client.Map.Name + " used to be a paradise, until those monsters came and ruined people`s life.")
                            .AddText("If want to hunt monsters for me. I will play you. Now, select the monsters you want to hunt.");
                        foreach (var mob in array)
                        {
                            data.AddOption(mob.Name + " Level(" + mob.Level + ")", (byte)mob.ID);
                        }
                        data.AddOption("See you later.", 255);
                        data.AddAvatar(102).FinalizeDialog();


                        break;
                    }
                default:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.DemonExterminator.CreateDemonExterminator(client, stream, 50, Option);
                            data.AddText("Perfect! I`ve added a jar to your inventory to track how many monsters you have killed.")
                                .AddText(" Come back and see me when you are finished and i will reward you");
                            data.AddOption("Okay.", 255);
                            data.AddAvatar(102).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You need atleast 1 empty spot in your inventory to be able to accept this quest.");
                            data.AddOption("Okay", 255);
                            data.AddAvatar(102).FinalizeDialog();
                        }


                        break;
                    }
            }
        }
        //[NpcAttribute(NpcID.DragonMassagner)] // [MaintNpc 2026-10-15] custom Demon Exterminator script; official Dragon Messenger 23746 is in Base\MaintNpc
        public static void DragonMassagner(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.TCCaptainTimes == 3)
                        {
                            data.AddText("You already helped me out today my warrior, please come back to me tomorrow!");
                            data.AddOption("Okay.", 255);
                            data.AddAvatar(102).FinalizeDialog();
                            break;
                        }
                        Game.MsgServer.MsgGameItem Jar;
                        if (client.Inventory.TryGetItem(client.DemonExterminator.ItemUID, out Jar))
                        {
                            if (Jar.Durability <= client.DemonExterminator.HuntKills)
                            {
                                client.Inventory.Remove(750000, 1, stream);
                                client.DemonExterminator.HuntKills = 0;
                                client.DemonExterminator.ItemUID = 0;

                                string itemname = client.DemonExterminator.SpecialReward(client, stream);

                                data.AddText("Thanks for the help. The quest finished! You have gaine a " + itemname + "");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You didn't finish killing the amount of monsters.");
                                data.AddOption("ah ok.", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("We're always looking for brave adventurers to protect the city from monsters. Are you interesed helping?");
                        data.AddOption("Yes please!", 103);
                        data.AddOption("I`m busy.", 255);
                        data.AddAvatar(102).FinalizeDialog();


                        break;
                    }
                case 211:
                    {
                        client.Teleport(229, 258, 1011);
                        break;
                    }
                case 220:
                    {
                        client.Teleport(569, 620, 1020);
                        break;
                    }
                case 215:
                    {
                        client.Teleport(791, 568, 1015);
                        break;
                    }
                case 200:
                    {
                        client.Teleport(476, 634, 1000);
                        break;
                    }
                case 201:
                    {
                        client.Teleport(82, 321, 1000);
                        break;
                    }
                case 103:
                    {
                        var array = Database.InfoDemonExterminators.CityQuests[1020].ToArray();
                        if (array.Length == 0)
                        {
                            ushort next_map = Database.InfoDemonExterminators.GetNextMap((byte)client.Player.Level);
                            if (next_map == 0)
                            {
                                data.AddText("You're a bit too powerful for the monsters near here to be any real challenge.");
                                data.AddOption("Okay", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            else
                            {
                                var DBMap = Pool.ServerMaps[next_map];
                                data.AddText("You are powerfull now. I suggest you to go Captain of " + DBMap.Name + ", because i heard that is ")
                                              .AddText("annoyed by the rampant monsters there. I believe you can help a lot. I will teleport you in")
                                              .AddText(" " + DBMap.Name + ", so you can reach there very soon.");
                                data.AddOption("Please send me to " + DBMap.Name + "", (byte)(200 + (next_map % 100)));
                                data.AddOption("I don`d wana to go there.", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("" + client.Map.Name + " used to be a paradise, until those monsters came and ruined people`s life.")
                            .AddText("If want to hunt monsters for me. I will play you. Now, select the monsters you want to hunt.");

                        if (client.Player.TCCaptainTimes == 0)
                        {
                            data.AddOption(array[0].Name + " Level(" + array[0].Level + ")", 10);
                        }
                        if (client.Player.TCCaptainTimes == 1)
                        {
                            data.AddOption(array[1].Name + " Level(" + array[1].Level + ")", 11);
                        }
                        if (client.Player.TCCaptainTimes == 2)
                        {
                            data.AddOption(array[2].Name + " Level(" + array[2].Level + ")", 12);
                        }
                        data.AddOption("See you later.", 255);
                        data.AddAvatar(102).FinalizeDialog();


                        break;
                    }
                default:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.DemonExterminator.CreateDemonExterminator(client, stream, 2000, Option);
                            data.AddText("Perfect! I`ve added a jar to your inventory to track how many monsters you have killed.")
                                .AddText(" Come back and see me when you are finished and i will reward you");
                            data.AddOption("Okay.", 255);
                            data.AddAvatar(102).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You need atleast 1 empty spot in your inventory to be able to accept this quest.");
                            data.AddOption("Okay", 255);
                            data.AddAvatar(102).FinalizeDialog();
                        }


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Solo)]
        public static void Solo(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.TCCaptainTimes == 3)
                        {
                            data.AddText("You already helped me out today my warrior, please come back to me tomorrow!");
                            data.AddOption("Okay.", 255);
                            data.AddAvatar(102).FinalizeDialog();
                            break;
                        }
                        Game.MsgServer.MsgGameItem Jar;
                        if (client.Inventory.TryGetItem(client.DemonExterminator.ItemUID, out Jar))
                        {
                            if (Jar.Durability <= client.DemonExterminator.HuntKills)
                            {
                                client.Inventory.Remove(750000, 1, stream);
                                client.DemonExterminator.HuntKills = 0;
                                client.DemonExterminator.ItemUID = 0;

                                string itemname = client.DemonExterminator.SpecialReward(client, stream);

                                data.AddText("Thanks for the help. The quest finished! You have gaine a " + itemname + "");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You didn't finish killing the amount of monsters.");
                                data.AddOption("ah ok.", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("We're always looking for brave adventurers to protect the city from monsters. Are you interesed helping?");
                        data.AddOption("Yes please!", 103);
                        data.AddOption("I`m busy.", 255);
                        data.AddAvatar(102).FinalizeDialog();


                        break;
                    }
                case 211:
                    {
                        client.Teleport(229, 258, 1011);
                        break;
                    }
                case 220:
                    {
                        client.Teleport(569, 620, 1020);
                        break;
                    }
                case 215:
                    {
                        client.Teleport(791, 568, 1015);
                        break;
                    }
                case 200:
                    {
                        client.Teleport(476, 634, 1000);
                        break;
                    }
                case 201:
                    {
                        client.Teleport(82, 321, 1000);
                        break;
                    }
                case 103:
                    {
                        var array = Database.InfoDemonExterminators.CityQuests[1011].ToArray();
                        if (array.Length == 0)
                        {
                            ushort next_map = 1011;
                            if (next_map == 0)
                            {
                                data.AddText("You're a bit too powerful for the monsters near here to be any real challenge.");
                                data.AddOption("Okay", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            else
                            {
                                var DBMap = Pool.ServerMaps[next_map];
                                data.AddText("You are powerfull now. I suggest you to go Captain of " + DBMap.Name + ", because i heard that is ")
                                              .AddText("annoyed by the rampant monsters there. I believe you can help a lot. I will teleport you in")
                                              .AddText(" " + DBMap.Name + ", so you can reach there very soon.");
                                data.AddOption("Please send me to " + DBMap.Name + "", (byte)(200 + (next_map % 100)));
                                data.AddOption("I don`d wana to go there.", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("" + client.Map.Name + " used to be a paradise, until those monsters came and ruined people`s life.")
                            .AddText("If want to hunt monsters for me. I will play you. Now, select the monsters you want to hunt.");

                        if (client.Player.TCCaptainTimes == 0)
                        {
                            data.AddOption(array[0].Name + " Level(" + array[0].Level + ")", 10);
                        }
                        if (client.Player.TCCaptainTimes == 1)
                        {
                            data.AddOption(array[1].Name + " Level(" + array[1].Level + ")", 11);
                        }
                        if (client.Player.TCCaptainTimes == 2)
                        {
                            data.AddOption(array[2].Name + " Level(" + array[2].Level + ")", 12);
                        }
                        data.AddOption("See you later.", 255);
                        data.AddAvatar(102).FinalizeDialog();


                        break;
                    }
                default:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.DemonExterminator.CreateDemonExterminator(client, stream, 1, Option);
                            data.AddText("Perfect! I`ve added a jar to your inventory to track how many monsters you have killed.")
                                .AddText(" Come back and see me when you are finished and i will reward you");
                            data.AddOption("Okay.", 255);
                            data.AddAvatar(102).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You need atleast 1 empty spot in your inventory to be able to accept this quest.");
                            data.AddOption("Okay", 255);
                            data.AddAvatar(102).FinalizeDialog();
                        }


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.BCaptain)]
        public static void BCaptain(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.TCCaptainTimes == 3)
                        {
                            data.AddText("You already helped me out today my warrior, please come back to me tomorrow!");
                            data.AddOption("Okay.", 255);
                            data.AddAvatar(102).FinalizeDialog();
                            break;
                        }
                        Game.MsgServer.MsgGameItem Jar;
                        if (client.Inventory.TryGetItem(client.DemonExterminator.ItemUID, out Jar))
                        {
                            if (Jar.Durability <= client.DemonExterminator.HuntKills)
                            {
                                client.GainExpBall(600, true, Role.Flags.ExperienceEffect.angelwing);
                                client.Inventory.Remove(750000, 1, stream);
                                client.DemonExterminator.HuntKills = 0;
                                client.DemonExterminator.ItemUID = 0;

                                string itemname = client.DemonExterminator.SpecialReward(client, stream);

                                data.AddText("Thanks for the help. The quest finished! You have gaine a " + itemname + "");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You didn't finish killing the amount of monsters.");
                                data.AddOption("ah ok.", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("We're always looking for brave adventurers to protect the city from monsters. Are you interesed helping?");
                        data.AddOption("Yes please!", 103);
                        data.AddOption("I`m busy.", 255);
                        data.AddAvatar(102).FinalizeDialog();


                        break;
                    }
                case 211:
                    {
                        client.Teleport(229, 258, 1011);
                        break;
                    }
                case 220:
                    {
                        client.Teleport(569, 620, 1020);
                        break;
                    }
                case 215:
                    {
                        client.Teleport(791, 568, 1015);
                        break;
                    }
                case 200:
                    {
                        client.Teleport(476, 634, 1000);
                        break;
                    }
                case 201:
                    {
                        client.Teleport(82, 321, 1000);
                        break;
                    }
                case 103:
                    {
                        var array = Database.InfoDemonExterminators.CityQuests[client.Player.Map].Where(p => p.Level + 5 > client.Player.Level).ToArray();
                        if (array.Length == 0)
                        {
                            ushort next_map = Database.InfoDemonExterminators.GetNextMap((byte)client.Player.Level);
                            if (next_map == 0)
                            {
                                data.AddText("You're a bit too powerful for the monsters near here to be any real challenge.");
                                data.AddOption("Okay", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            else
                            {
                                var DBMap = Pool.ServerMaps[next_map];
                                data.AddText("You are powerfull now. I suggest you to go Captain of " + DBMap.Name + ", because i heard that is ")
                                              .AddText("annoyed by the rampant monsters there. I believe you can help a lot. I will teleport you in")
                                              .AddText(" " + DBMap.Name + ", so you can reach there very soon.");
                                data.AddOption("Please send me to " + DBMap.Name + "", (byte)(200 + (next_map % 100)));
                                data.AddOption("I don`d wana to go there.", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("" + client.Map.Name + " used to be a paradise, until those monsters came and ruined people`s life.")
                            .AddText("If want to hunt monsters for me. I will play you. Now, select the monsters you want to hunt.");
                        foreach (var mob in array)
                        {
                            data.AddOption(mob.Name + " Level(" + mob.Level + ")", (byte)mob.ID);
                        }
                        data.AddOption("See you later.", 255);
                        data.AddAvatar(102).FinalizeDialog();


                        break;
                    }
                default:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.DemonExterminator.CreateDemonExterminator(client, stream, 50, Option);
                            data.AddText("Perfect! I`ve added a jar to your inventory to track how many monsters you have killed.")
                                .AddText(" Come back and see me when you are finished and i will reward you");
                            data.AddOption("Okay.", 255);
                            data.AddAvatar(102).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You need atleast 1 empty spot in your inventory to be able to accept this quest.");
                            data.AddOption("Okay", 255);
                            data.AddAvatar(102).FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.DCCaptain)]
        public static void DCCaptain(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.TCCaptainTimes == 3)
                        {
                            data.AddText("You already helped me out today my warrior, please come back to me tomorrow!");
                            data.AddOption("Okay.", 255);
                            data.AddAvatar(102).FinalizeDialog();
                            break;
                        }
                        Game.MsgServer.MsgGameItem Jar;
                        if (client.Inventory.TryGetItem(client.DemonExterminator.ItemUID, out Jar))
                        {
                            if (Jar.Durability <= client.DemonExterminator.HuntKills)
                            {
                                client.GainExpBall(600, true, Role.Flags.ExperienceEffect.angelwing);
                                client.Inventory.Remove(750000, 1, stream);
                                client.DemonExterminator.HuntKills = 0;
                                client.DemonExterminator.ItemUID = 0;

                                string itemname = client.DemonExterminator.SpecialReward(client, stream);

                                data.AddText("Thanks for the help. The quest finished! You have gaine a " + itemname + "");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You didn't finish killing the amount of monsters.");
                                data.AddOption("ah ok.", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("We're always looking for brave adventurers to protect the city from monsters. Are you interesed helping?");
                        data.AddOption("Yes please!", 103);
                        data.AddOption("I`m busy.", 255);
                        data.AddAvatar(102).FinalizeDialog();


                        break;
                    }
                case 211:
                    {
                        client.Teleport(229, 258, 1011);
                        break;
                    }
                case 220:
                    {
                        client.Teleport(569, 620, 1020);
                        break;
                    }
                case 215:
                    {
                        client.Teleport(791, 568, 1015);
                        break;
                    }
                case 200:
                    {
                        client.Teleport(476, 634, 1000);
                        break;
                    }
                case 201:
                    {
                        client.Teleport(82, 321, 1000);
                        break;
                    }
                case 103:
                    {
                        var array = Database.InfoDemonExterminators.CityQuests[client.Player.Map].Where(p => p.Level + 5 > client.Player.Level).ToArray();
                        if (array.Length == 0)
                        {
                            ushort next_map = Database.InfoDemonExterminators.GetNextMap((byte)client.Player.Level);
                            if (next_map == 0)
                            {
                                data.AddText("You're a bit too powerful for the monsters near here to be any real challenge.");
                                data.AddOption("Okay", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            else
                            {
                                var DBMap = Pool.ServerMaps[next_map];
                                data.AddText("You are powerfull now. I suggest you to go Captain of " + DBMap.Name + ", because i heard that is ")
                                              .AddText("annoyed by the rampant monsters there. I believe you can help a lot. I will teleport you in")
                                              .AddText(" " + DBMap.Name + ", so you can reach there very soon.");
                                data.AddOption("Please send me to " + DBMap.Name + "", (byte)(200 + (next_map % 100)));
                                data.AddOption("I don`d wana to go there.", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("" + client.Map.Name + " used to be a paradise, until those monsters came and ruined people`s life.")
                            .AddText("If want to hunt monsters for me. I will play you. Now, select the monsters you want to hunt.");
                        foreach (var mob in array)
                        {
                            data.AddOption(mob.Name + " Level(" + mob.Level + ")", (byte)mob.ID);
                        }
                        data.AddOption("See you later.", 255);
                        data.AddAvatar(102).FinalizeDialog();


                        break;
                    }
                default:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.DemonExterminator.CreateDemonExterminator(client, stream, 50, Option);
                            data.AddText("Perfect! I`ve added a jar to your inventory to track how many monsters you have killed.")
                                .AddText(" Come back and see me when you are finished and i will reward you");
                            data.AddOption("Okay.", 255);
                            data.AddAvatar(102).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You need atleast 1 empty spot in your inventory to be able to accept this quest.");
                            data.AddOption("Okay", 255);
                            data.AddAvatar(102).FinalizeDialog();
                        }


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.AMCaptain)]
        public static void AMCaptain(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.TCCaptainTimes == 3)
                        {
                            data.AddText("You already helped me out today my warrior, please come back to me tomorrow!");
                            data.AddOption("Okay.", 255);
                            data.AddAvatar(102).FinalizeDialog();
                            break;
                        }
                        Game.MsgServer.MsgGameItem Jar;
                        if (client.Inventory.TryGetItem(client.DemonExterminator.ItemUID, out Jar))
                        {
                            if (Jar.Durability <= client.DemonExterminator.HuntKills)
                            {
                                client.GainExpBall(600, true, Role.Flags.ExperienceEffect.angelwing);
                                client.Inventory.Remove(750000, 1, stream);
                                client.DemonExterminator.HuntKills = 0;
                                client.DemonExterminator.ItemUID = 0;

                                string itemname = client.DemonExterminator.SpecialReward(client, stream);

                                data.AddText("Thanks for the help. The quest finished! You have gaine a " + itemname + "");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You didn't finish killing the amount of monsters.");
                                data.AddOption("ah ok.", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("We're always looking for brave adventurers to protect the city from monsters. Are you interesed helping?");
                        data.AddOption("Yes please!", 103);
                        data.AddOption("I`m busy.", 255);
                        data.AddAvatar(102).FinalizeDialog();



                        break;
                    }
                case 211:
                    {
                        client.Teleport(229, 258, 1011);
                        break;
                    }
                case 220:
                    {
                        client.Teleport(569, 620, 1020);
                        break;
                    }
                case 215:
                    {
                        client.Teleport(791, 568, 1015);
                        break;
                    }
                case 200:
                    {
                        client.Teleport(476, 634, 1000);
                        break;
                    }
                case 201:
                    {
                        client.Teleport(82, 321, 1000);
                        break;
                    }
                case 103:
                    {
                        var array = Database.InfoDemonExterminators.CityQuests[1020].ToArray();
                        if (array.Length == 0)
                        {
                            ushort next_map = Database.InfoDemonExterminators.GetNextMap((byte)client.Player.Level);
                            if (next_map == 0)
                            {
                                data.AddText("You're a bit too powerful for the monsters near here to be any real challenge.");
                                data.AddOption("Okay", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            else
                            {
                                var DBMap = Pool.ServerMaps[next_map];
                                data.AddText("You are powerfull now. I suggest you to go Captain of " + DBMap.Name + ", because i heard that is ")
                                              .AddText("annoyed by the rampant monsters there. I believe you can help a lot. I will teleport you in")
                                              .AddText(" " + DBMap.Name + ", so you can reach there very soon.");
                                data.AddOption("Please send me to " + DBMap.Name + "", (byte)(200 + (next_map % 100)));
                                data.AddOption("I don`d wana to go there.", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("" + client.Map.Name + " used to be a paradise, until those monsters came and ruined people`s life.")
                            .AddText("If want to hunt monsters for me. I will play you. Now, select the monsters you want to hunt.");
                        foreach (var mob in array)
                        {
                            data.AddOption(mob.Name + " Level(" + mob.Level + ")", (byte)mob.ID);
                        }
                        data.AddOption("See you later.", 255);
                        data.AddAvatar(102).FinalizeDialog();


                        break;
                    }
                default:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.DemonExterminator.CreateDemonExterminator(client, stream, 50, Option);
                            data.AddText("Perfect! I`ve added a jar to your inventory to track how many monsters you have killed.")
                                .AddText(" Come back and see me when you are finished and i will reward you");
                            data.AddOption("Okay.", 255);
                            data.AddAvatar(102).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You need atleast 1 empty spot in your inventory to be able to accept this quest.");
                            data.AddOption("Okay", 255);
                            data.AddAvatar(102).FinalizeDialog();
                        }


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.PCCaptain)]
        public static void PCCaptain(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.TCCaptainTimes == 3)
                        {
                            data.AddText("You already helped me out today my warrior, please come back to me tomorrow!");
                            data.AddOption("Okay.", 255);
                            data.AddAvatar(102).FinalizeDialog();
                            break;
                        }
                        Game.MsgServer.MsgGameItem Jar;
                        if (client.Inventory.TryGetItem(client.DemonExterminator.ItemUID, out Jar))
                        {
                            if (Jar.Durability <= client.DemonExterminator.HuntKills)
                            {
                                client.GainExpBall(600, true, Role.Flags.ExperienceEffect.angelwing);
                                client.Inventory.Remove(750000, 1, stream);
                                client.DemonExterminator.HuntKills = 0;
                                client.DemonExterminator.ItemUID = 0;

                                string itemname = client.DemonExterminator.SpecialReward(client, stream);

                                data.AddText("Thanks for the help. The quest finished! You have gaine a " + itemname + "");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You didn't finish killing the amount of monsters.");
                                data.AddOption("ah ok.", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("We're always looking for brave adventurers to protect the city from monsters. Are you interesed helping?");
                        data.AddOption("Yes please!", 103);
                        data.AddOption("I`m busy.", 255);
                        data.AddAvatar(102).FinalizeDialog();


                        break;
                    }
                case 211:
                    {
                        client.Teleport(229, 258, 1011);
                        break;
                    }
                case 220:
                    {
                        client.Teleport(569, 620, 1020);
                        break;
                    }
                case 215:
                    {
                        client.Teleport(791, 568, 1015);
                        break;
                    }
                case 200:
                    {
                        client.Teleport(476, 634, 1000);
                        break;
                    }
                case 201:
                    {
                        client.Teleport(82, 321, 1000);
                        break;
                    }
                case 103:
                    {
                        var array = Database.InfoDemonExterminators.CityQuests[client.Player.Map].Where(p => p.Level + 5 > client.Player.Level).ToArray();
                        if (array.Length == 0)
                        {
                            ushort next_map = Database.InfoDemonExterminators.GetNextMap((byte)client.Player.Level);
                            if (next_map == 0)
                            {
                                data.AddText("You're a bit too powerful for the monsters near here to be any real challenge.");
                                data.AddOption("Okay", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            else
                            {
                                var DBMap = Pool.ServerMaps[next_map];
                                data.AddText("You are powerfull now. I suggest you to go Captain of " + DBMap.Name + ", because i heard that is ")
                                              .AddText("annoyed by the rampant monsters there. I believe you can help a lot. I will teleport you in")
                                              .AddText(" " + DBMap.Name + ", so you can reach there very soon.");
                                data.AddOption("Please send me to " + DBMap.Name + "", (byte)(200 + (next_map % 100)));
                                data.AddOption("I don`d wana to go there.", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("" + client.Map.Name + " used to be a paradise, until those monsters came and ruined people`s life.")
                            .AddText("If want to hunt monsters for me. I will play you. Now, select the monsters you want to hunt.");
                        foreach (var mob in array)
                        {
                            data.AddOption(mob.Name + " Level(" + mob.Level + ")", (byte)mob.ID);
                        }
                        data.AddOption("See you later.", 255);
                        data.AddAvatar(102).FinalizeDialog();

                        break;
                    }
                default:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.DemonExterminator.CreateDemonExterminator(client, stream, 50, Option);
                            data.AddText("Perfect! I`ve added a jar to your inventory to track how many monsters you have killed.")
                                .AddText(" Come back and see me when you are finished and i will reward you");
                            data.AddOption("Okay.", 255);
                            data.AddAvatar(102).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You need atleast 1 empty spot in your inventory to be able to accept this quest.");
                            data.AddOption("Okay", 255);
                            data.AddAvatar(102).FinalizeDialog();
                        }


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.TCCaptain)]
        public static void TCCaptain(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.BlackSmith, client.Player.Class, 823);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TCCaptain, client.Player.Class, 824);
            Database.QuestInfo.DBQuest ActiveQuest3 = null;
            if (Database.AtributesStatus.IsLee(client.Player.Class))
                ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.BlackSmith, client.Player.Class, 6426);
            else if (Database.AtributesStatus.IsWindWalker(client.Player.Class))
                ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.BlackSmith, client.Player.Class, 3795);
            else
                ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)id, client.Player.Class, 0, p => p.MissionId != 824 && p.FinishNpcId.Map == 1004);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            goto case 230;
                        }
                        if (client.Player.QuestGUI.IsActiveQuest(ActiveQuest.MissionId))
                        {
                            client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);

                            data.AddText("Good day, my friend, I`m the TC Captain, I`m in charge of the public security around")
                                .AddText(" here. It is indeed an important job, isn`t it? Well, ever talking about contributing")
                                .AddText(" a bit to the public? You'll be rewarded.")
                           .AddOption("What can i do for you?", 240)
                           .AddOption("I want to take a Daily Quest.", 230)
                           .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.IsActiveQuest(ActiveQuest2.MissionId))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 10))
                            {
                                client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId);
                                client.GainExpBall(200);
                                data.AddText("Good job! That is what i want! You returned peace to Twin City!")
         .AddOption("Thank you!", 231)
         .AddAvatar(10).FinalizeDialog();
                                break;
                            }
                            data.AddText("Defeat 10 Pheasants outside Twin City. You are doing for this for people of Twin City!")
            .AddOption("Okay!", 255)
            .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            goto case 231;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {

                            goto case 240;
                        }

                        goto case 230;

                    }
                case 240:
                    {
                        data.AddText("I have some jobs for you to do here! The Pheasants outside Twin City are becoming more")
                                                        .AddText(" and more aggressive! I want you to defeat 10 of them. This is for the safety of the people")
                                                        .AddText(" in Twin City.")
                                                   .AddOption("Okay, right on my way.", 232)
                                                   .AddOption("I want to take a Daily Quest.", 230)
                                                   .AddAvatar(10).FinalizeDialog();

                        break;
                    }
                case 233:
                    {
                        data.AddText("Good job! That is what i want! You returned peace to Twin City!")
                            .AddOption("Thank you!", 233)
                            .AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 231:
                    {
                        data.AddText("There is a long way to go! You need some professional guidance to grow stronger! Find the NPC of your class")
                                                           .AddText(" in the Job center to learn more about job promotion!")
                                                      .AddOption("Job promotion 1.", 234)
                                                      .AddOption("I want to take a Daily Quest.", 230)
                                                      .AddAvatar(10).FinalizeDialog();

                        break;
                    }
                case 234:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.Player.QuestGUI.SendAutoPatcher("The TC Captain asked you to enter the Job Center and find the NPC of you class to get promoted.", ActiveQuest3.FinishNpcId.Map, ActiveQuest3.FinishNpcId.X, ActiveQuest3.FinishNpcId.Y, ActiveQuest3.FinishNpcId.ID);

                        break;
                    }
                case 232:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher("The TC Captain asked you to slay 10 Pheasants outside Twin City! Remember to wear your equipment first!", 1002, 341, 401, 0);

                        break;
                    }
                case 230:
                    {
                        if (client.Player.TCCaptainTimes == 3)
                        {
                            data.AddText("You already helped me out today my warrior, please come back to me tomorrow!");
                            data.AddOption("Okay.", 255);
                            data.AddAvatar(102).FinalizeDialog();
                            break;
                        }
                        Game.MsgServer.MsgGameItem Jar;
                        if (client.Inventory.TryGetItem(client.DemonExterminator.ItemUID, out Jar))
                        {
                            if (Jar.Durability <= client.DemonExterminator.HuntKills)
                            {
                                client.IncreaseExperience(stream, 1400, Role.Flags.ExperienceEffect.angelwing);
                                client.Inventory.Remove(750000, 1, stream);
                                client.DemonExterminator.HuntKills = 0;
                                client.DemonExterminator.ItemUID = 0;

                                string itemname = client.DemonExterminator.SpecialReward(client, stream);

                                data.AddText("Thanks for the help. The quest finished! You have gaine a " + itemname + "");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You didn't finish killing the amount of monsters.");
                                data.AddOption("ah ok.", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("We're always looking for brave adventurers to protect the city from monsters. Are you interesed helping?");
                        data.AddOption("Yes please!", 103);
                        data.AddOption("I`m busy.", 255);
                        data.AddAvatar(102).FinalizeDialog();

                        break;
                    }
                case 211:
                    {
                        client.Teleport(229, 258, 1011);
                        break;
                    }
                case 220:
                    {
                        client.Teleport(569, 620, 1020);
                        break;
                    }
                case 215:
                    {
                        client.Teleport(791, 568, 1015);
                        break;
                    }
                case 200:
                    {
                        client.Teleport(476, 634, 1000);
                        break;
                    }
                case 201:
                    {
                        client.Teleport(82, 321, 1000);
                        break;
                    }
                case 103:
                    {
                        var array = Database.InfoDemonExterminators.CityQuests[client.Player.Map].Where(p => p.Level + 5 > client.Player.Level).ToArray();
                        if (array.Length == 0)
                        {
                            ushort next_map = Database.InfoDemonExterminators.GetNextMap((byte)client.Player.Level);
                            if (next_map == 0)
                            {
                                data.AddText("You're a bit too powerful for the monsters near here to be any real challenge.");
                                data.AddOption("Okay", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            else
                            {
                                var DBMap = Pool.ServerMaps[next_map];
                                data.AddText("You are powerfull now. I suggest you to go Captain of " + DBMap.Name + ", because i heard that is ")
                                              .AddText("annoyed by the rampant monsters there. I believe you can help a lot. I will teleport you in")
                                              .AddText(" " + DBMap.Name + ", so you can reach there very soon.");
                                data.AddOption("Please send me to " + DBMap.Name + "", (byte)(200 + (next_map % 100)));
                                data.AddOption("I don`d wana to go there.", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("" + client.Map.Name + " used to be a paradise, until those monsters came and ruined people`s life.")
                            .AddText("If want to hunt monsters for me. I will play you. Now, select the monsters you want to hunt.");
                        foreach (var mob in array)
                        {
                            data.AddOption(mob.Name + " Level(" + mob.Level + ")", (byte)mob.ID);
                        }
                        data.AddOption("See you later.", 255);
                        data.AddAvatar(102).FinalizeDialog();

                        break;
                    }
                default:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.DemonExterminator.CreateDemonExterminator(client, stream, 50, Option);
                            data.AddText("Perfect! I`ve added a jar to your inventory to track how many monsters you have killed.")
                                .AddText(" Come back and see me when you are finished and i will reward you");
                            data.AddOption("Okay.", 255);
                            data.AddAvatar(102).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You need atleast 1 empty spot in your inventory to be able to accept this quest.");
                            data.AddOption("Okay", 255);
                            data.AddAvatar(102).FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.CloudSaint)]
        public static void CloudSaint(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.DemonExterminator.FinishToday == 1)
                        {
                            data.AddText("You already helped me out today my warrior, please come back to me tomorrow!");
                            data.AddOption("Okay.", 255);
                            data.AddAvatar(102).FinalizeDialog();
                            break;
                        }
                        Game.MsgServer.MsgGameItem Jar;
                        if (client.Inventory.TryGetItem(client.DemonExterminator.ItemUID, out Jar))
                        {
                            if (Jar.Durability <= client.DemonExterminator.HuntKills)
                            {
                                byte stage = client.DemonExterminator.GetDemonExterminatorStage(Jar);
                                if (stage == 3)
                                {
                                    data.AddText("You have finished killing all the monsters for today. Do you want to claim your prize?");
                                    data.AddOption("Yes.", 3);
                                    data.AddOption("Nevermind.", 255);
                                    data.AddAvatar(102).FinalizeDialog();
                                }
                                else if (stage != byte.MaxValue)
                                {
                                    data.AddText("You have killed the amount required for this stage. Do you want to advance?");
                                    data.AddOption("Yes.", 2);
                                    data.AddOption("Nevermind.", 255);
                                    data.AddAvatar(102).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("You didn't finish killing the amount of monsters for your current stage. Are you giving up?");
                                data.AddOption("Yes, I want to give up.", 4);
                                data.AddOption("Nevermind.", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                        }

                        else
                        {

                            if (client.DemonExterminator.FinishToday == 1)
                            {
                                data.AddText("You already helped me out today my warrior, please come back to me tomorrow!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                            if (client.Player.Level < 120)
                            {
                                data.AddText("You are too weak to take quests from me. Go and find the MC Captain.");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(102).FinalizeDialog();
                                break;
                            }
                            data.AddText("Stay calm and listem, the Demons are taking over the control of ");
                            data.AddText("the world. My family is the only one that can make a contract with ");
                            data.AddText("the humans to purify the world from the devils. I can give you two ");
                            data.AddText("expballs as reward for doing it. But you can do it once one time per ");
                            data.AddText("day, what will you do?");
                            data.AddOption("Begin quest.", 1);
                            data.AddOption("Wana to buy That Quest", 5);
                            data.AddOption("Just passing by", 255);
                            data.AddAvatar(102).FinalizeDialog();
                        }

                        break;
                    }
                case 2:
                    {
                        Game.MsgServer.MsgGameItem Jar;
                        if (client.Inventory.TryGetItem(client.DemonExterminator.ItemUID, out Jar))
                        {
                            if (Jar.Durability <= client.DemonExterminator.HuntKills)
                            {
                                client.DemonExterminator.HuntKills = 0;

                                byte stage = client.DemonExterminator.GetDemonExterminatorStage(Jar);
                                stage++;

                                client.DemonExterminator.CreateDemonExterminator(client, stream, Database.InfoDemonExterminators.StageKills[stage], Database.InfoDemonExterminators.QuestTyp[client.DemonExterminator.QuestTyp][stage]);
                                data.AddText(string.Concat("Now go and kill ", Database.InfoDemonExterminators.StageKills[stage], " ", Database.InfoDemonExterminators.QuestName[client.DemonExterminator.QuestTyp][stage], " then come back."));
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(102).FinalizeDialog();

                            }
                            else
                            {
                                data.AddText("You do not have the jar.");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(102).FinalizeDialog();
                            }
                        }

                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.DemonExterminator.FinishToday = 1;
                            int questtype = Pool.GetRandom.Next(0, Database.InfoDemonExterminators.QuestTyp.Length);
                            client.DemonExterminator.QuestTyp = (uint)questtype;
                            client.DemonExterminator.CreateDemonExterminator(client, stream, Database.InfoDemonExterminators.StageKills[0], Database.InfoDemonExterminators.QuestTyp[questtype][0]);
                            data.AddText("Thank you, brave soldier, for accepting this quest. Kill " + Database.InfoDemonExterminators.StageKills[0] + " " + Database.InfoDemonExterminators.QuestTyp[questtype][0] + " and come back.");
                            data.AddOption("Okay", 255);
                            data.AddAvatar(102).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You need atleast 1 empty spot in your inventory to be able to accept this quest.");
                            data.AddOption("Okay", 255);
                            data.AddAvatar(102).FinalizeDialog();
                        }

                        break;
                    }
                case 4:
                    {

                        client.Inventory.Remove(750000, 1, stream);
                        client.DemonExterminator.HuntKills = 0;
                        client.DemonExterminator.ItemUID = 0;
                        break;
                    }
                case 3:
                    {

                        client.GainExpBall(600 * 5, true, Role.Flags.ExperienceEffect.angelwing);
                        client.Inventory.Remove(750000, 1, stream);
                        client.DemonExterminator.HuntKills = 0;
                        client.DemonExterminator.ItemUID = 0;
                        break;
                    }
                case 5:
                    {
                        data.AddText("You need to buy that quest for 3.000 ConquerPoints?");
                        data.AddOption("Yes. I`m sure,", 6);
                        data.AddOption("Just passing by", 255);
                        data.AddAvatar(102).FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        if (client.Player.ConquerPoints >= 3000)
                        {
                            if (client.InTrade) return;
                            client.Player.ConquerPoints -= 3000;


                            client.GainExpBall(600 * 5, true, Role.Flags.ExperienceEffect.angelwing);
                            client.Inventory.Remove(750000, 1, stream);
                            client.DemonExterminator.ItemUID = 0;
                            client.DemonExterminator.HuntKills = 0;
                            client.DemonExterminator.FinishToday = 1;
                        }
                        else
                        {
                            data.AddText("Sorry you not have 98 ConquerPoints");
                            data.AddOption("Ah. Ok", 255);
                            data.AddAvatar(102).FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Confiscator)]
        public static void Confiscator(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("The red/black name can redeem their detained equipment within 7 days. The CPs they pay for their guilt will be rewarded to the")
                                                    .AddText("~who killed them. If they do not pay, the equipment detained will be given to the one who killed them as rewards.")
                                            .AddOption("I~want~to~redeem~my~equipment.", 1)
                                            .AddOption("I~want~to~claim~my~rewards.", 3)
                                            .AddOption("Give up all detained items.", 80)
                                            .AddOption("What", 255)
                                            .AddAvatar(102).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("You can pay some CPs to redeem your equipment.")
                                            .AddOption("I~want~my~equipment.", 2)
                                            .AddOption("Let~me~think.", 255)
                                            .AddAvatar(102).FinalizeDialog();
                        break;
                    }
                case 80:
                    {
                        data.AddText("Are you sure you want to give up your detained items?")
                                            .AddOption("Yes.", 255)
                                            .AddAvatar(102).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        if (!client.Confiscator.IsEmptyReadem())
                        {

                            ActionQuery action = new ActionQuery()
                            {
                                Type = ActionType.OpenDialog,
                                ObjId = client.Player.UID,
                                dwParam = MsgServer.DialogCommands.DetainRedeem,
                                PositionX = client.Player.X,
                                PositionY = client.Player.Y
                            };
                            client.Send(stream.ActionCreate(action));

                        }
                        else
                        {
                            data.AddText("You do not have any equipment detained.")
                 .AddOption("Got~it.", 255)
                 .AddAvatar(102).FinalizeDialog();
                        }

                        break;
                    }
                case 3:
                    {
                        data.AddText("The red/black name you killed can redeem equipment within 7 days. You will be rewarded the CPs that he/she pay for the guilt.")
                           .AddText("~If not, you will get the detained equipment as rewards.")
                  .AddOption("I~want~my~equipment.", 4)
                  .AddOption("Let~me~think.", 255)
                  .AddAvatar(102).FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        if (!client.Confiscator.IsEmptyClaim())
                        {
                            ActionQuery action = new ActionQuery()
                            {
                                Type = ActionType.OpenDialog,
                                ObjId = client.Player.UID,
                                dwParam = MsgServer.DialogCommands.DetainClaim,
                                PositionX = client.Player.X,
                                PositionY = client.Player.Y
                            };
                            client.Send(stream.ActionCreate(action));

                        }
                        else
                        {
                            data.AddText("Sorry, you do not have anything to claim now.")
                 .AddOption("Alright.", 255)
                 .AddAvatar(102).FinalizeDialog();
                        }

                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.JailConfiscator)]
        public static void JailConfiscator(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("The red/black name can redeem their detained equipment within 7 days. The CPs they pay for their guilt will be rewarded to the")
                            .AddText("~who killed them. If they do not pay, the equipment detained will be given to the one who killed them as rewards.")
                    .AddOption("I~want~to~redeem~my~equipment.", 1)
                    .AddOption("I~want~to~claim~my~rewards.", 3)
                    .AddOption("What?", 255)
                    .AddAvatar(102).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("You can pay some CPs to redeem your equipment.")
                  .AddOption("I~want~my~equipment.", 2)
                  .AddOption("Let~me~think.", 255)
                  .AddAvatar(102).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (!client.Confiscator.IsEmptyReadem())
                        {
                            ActionQuery action = new ActionQuery()
                            {
                                Type = ActionType.OpenDialog,
                                ObjId = client.Player.UID,
                                dwParam = MsgServer.DialogCommands.DetainRedeem,
                                PositionX = client.Player.X,
                                PositionY = client.Player.Y
                            };
                            client.Send(stream.ActionCreate(action));


                        }
                        else
                        {
                            data.AddText("You do not have any equipment detained.")
                 .AddOption("Got~it.", 255)
                 .AddAvatar(102).FinalizeDialog();
                        }

                        break;
                    }
                case 3:
                    {
                        data.AddText("The red/black name you killed can redeem equipment within 7 days. You will be rewarded the CPs that he/she pay for the guilt.")
                            .AddText("~If not, you will get the detained equipment as rewards.")
                   .AddOption("I~want~my~equipment.", 4)
                   .AddOption("Let~me~think.", 255)
                   .AddAvatar(102).FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        if (!client.Confiscator.IsEmptyClaim())
                        {

                            ActionQuery action = new ActionQuery()
                            {
                                ObjId = client.Player.UID,
                                Type = ActionType.OpenDialog,
                                dwParam = MsgServer.DialogCommands.DetainClaim,
                                PositionX = client.Player.X,
                                PositionY = client.Player.Y
                            };
                            client.Send(stream.ActionCreate(action));

                        }
                        else
                        {
                            data.AddText("Sorry, you do not have anything to claim now.")
                 .AddOption("I~see.", 255)
                 .AddAvatar(102).FinalizeDialog();
                        }

                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.GarmentShopKeeper)]
        public static void GarmentShopKeeper(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        ActionQuery action = new ActionQuery()
                        {
                            Type = ActionType.OpenDialog,
                            ObjId = client.Player.UID,
                            dwParam = MsgServer.DialogCommands.GarmentShop,
                            PositionX = client.Player.X,
                            PositionY = client.Player.Y
                        };
                        client.Send(stream.ActionCreate(action));
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Shopboy)]
        public static void Shopboy(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {                       
                        data.AddText("Shopboy:\n");
                        data.AddText(" There is never lack of opportunity to get rich in Twin City, and you just need to find it. See what, I`ve decided to expand my business to packing services.    The charges must be certainly fair. How may I help you")
                    .AddOption("Further pack Meteor Scrolls.", 38)
                    .AddOption("Further pack DB Scrolls.", 40)
                    .AddOption("Pack~Meteors.", 34)
                    .AddOption("Pack~+1~Stones.", 1)
                    .AddOption("Pack~+2~Stones.", 3)
                    .AddOption("Pack~+3~Stones.", 5)
                    .AddOption("Pack~Dragon Balls.", 36)
                    .AddOption("Pack~normal~Gems.", 7)
                    .AddOption("Next~page.", 72)
                    .AddOption("I`m just passing by.", 255)
                    .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 73:
                    {
                        data.AddText("Shopboy:\n");
                        data.AddText(" There is never lack of opportunity to get rich in Twin City, and you just need to find it. See what, I`ve decided to expand my business to packing services.    The charges must be certainly fair. How may I help you")
                    .AddOption("Further pack Meteor Scrolls.", 38)
                    .AddOption("Further pack DB Scrolls.", 40)
                    .AddOption("Pack~Meteors.", 34)
                    .AddOption("Pack~+1~Stones.", 1)
                    .AddOption("Pack~+2~Stones.", 3)
                    .AddOption("Pack~+3~Stones.", 5)
                    .AddOption("Pack~Dragon Balls.", 36)
                    .AddOption("Pack~normal~Gems.", 7)
                    .AddOption("Next~page.", 72)
                    .AddOption("I`m just passing by.", 255)
                    .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 72:
                    {
                        data.AddText("Shopboy:\n");
                        data.AddText(" There is never lack of opportunity to get rich in Twin City, and you just need to find it. See what, I`ve decided to expand my business to packing services.    The charges must be certainly fair. How may I help you?")
                           .AddOption("Pack~EXP~Balls~for~me!", 32)
                           .AddOption("Pack~Diligence~Books.", 255)
                           .AddOption("Pack~Endurance~Books.", 255)
                           .AddOption("Pack~EXP~Ball~Fragment.", 255)
                           .AddOption("Previous~page.", 73)
                           .AddOption("I`m just passing by.", 255)
                           .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 40:
                    {
                        data.AddText("I can pack 10 DB Scrolls into a Super DB Scroll. Do you want me to pack them for you?")
                           .AddOption("Yes.", 41)
                           .AddOption("I`ll talk to you later.", 255)
                           .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 41:
                    {
                        if (client.Inventory.Contain(Database.ItemType.DBScroll, 10, 1))
                        {
                            client.Inventory.Remove(Database.ItemType.DBScroll, 10, stream);
                            client.Inventory.Add(stream, Database.ItemType.SuperDBScroll, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        }
                        else if (client.Inventory.Contain(Database.ItemType.DBScroll, 10))
                        {
                            client.Inventory.Remove(Database.ItemType.DBScroll, 10, stream);
                            client.Inventory.Add(stream, Database.ItemType.SuperDBScroll, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false);

                        }
                        else
                        {
                            data.AddText("Please make sure you have 10 DB Scrolls in your inventory, first.")
                  .AddOption("Okay!", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }

                        break;
                    }
                case 38:
                    {
                        data.AddText("I can pack 10 Meteor Scrolls into a Super Meteor Scroll. Do you want me to pack them for you?")
                           .AddOption("Yes.", 39)
                           .AddOption("I`ll talk to you later.", 255)
                           .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 39:
                    {
                        if (client.Inventory.Contain(Database.ItemType.MeteorScroll, 10, 1))
                        {
                            client.Inventory.Remove(Database.ItemType.MeteorScroll, 10, stream);
                            client.Inventory.Add(stream, Database.ItemType.SuperMeteorScroll, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        }
                        else if (client.Inventory.Contain(Database.ItemType.MeteorScroll, 10, 0))
                        {
                            client.Inventory.Remove(Database.ItemType.MeteorScroll, 10, stream);
                            client.Inventory.Add(stream, Database.ItemType.SuperMeteorScroll, 1);
                        }
                        else
                        {
                            data.AddText("Please make sure you have 10 Meteor Scrolls in your inventory, first.")
                  .AddOption("Okay!", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }

                        break;
                    }
                case 36:
                    {
                        data.AddText("I can pack 10 Dragon Balls into a DB Scroll.")
                           .AddOption("Okay. Pack my DBs.", 37)
                           .AddOption("Bye.", 255)
                           .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 37:
                    {
                        if (client.Inventory.Contain(Database.ItemType.DragonBall, 10, 1))
                        {
                            client.Inventory.Remove(Database.ItemType.DragonBall, 10, stream);
                            client.Inventory.Add(stream, Database.ItemType.DBScroll, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        }
                        else if (client.Inventory.Contain(Database.ItemType.DragonBall, 10, 0))
                        {
                            client.Inventory.Remove(Database.ItemType.DragonBall, 10, stream);
                            client.Inventory.Add(stream, Database.ItemType.DBScroll, 1);
                        }
                        else
                        {
                            data.AddText("Please make sure you have 10 Dragon Balls, first.")
                  .AddOption("Okay!", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }

                        break;
                    }

                case 34:
                    {
                        data.AddText("I can pack 10 Meteors into a Meteor Scroll. Do you want me to pack them for you")
                            .AddOption("Yes.", 35)
                            .AddOption("Bye.", 255)
                            .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 35:
                    {
                        if (client.Inventory.Contain(Database.ItemType.Meteor, 10, 1))
                        {
                            client.Inventory.Remove(Database.ItemType.Meteor, 10, stream);
                            client.Inventory.Add(stream, Database.ItemType.MeteorScroll, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        }
                        else if (client.Inventory.Contain(Database.ItemType.Meteor, 10, 0))
                        {
                            client.Inventory.Remove(Database.ItemType.Meteor, 10, stream);
                            client.Inventory.Add(stream, Database.ItemType.MeteorScroll, 1);
                        }
                        else
                        {
                            data.AddText("Please make sure you have 10 Meteors, first.")
                  .AddOption("Okay!", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }

                        break;
                    }
                case 32:
                    {
                        data.AddText("Pay 100,000 Silver and give me 5 EXP Balls, I can pack them into an EXP Ball Gift Box for you!")
                                                    .AddOption("Pack~EXP~Balls~for~me!", 33)
                                                    .AddOption("No, thanks.", 255)
                                                    .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 33:
                    {
                        if (client.Inventory.Contain(Database.ItemType.ExpBall, 5, 1))
                        {
                            if (client.Player.Money >= 100000)
                            {
                                client.Player.Money -= 100000;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Inventory.Remove(Database.ItemType.ExpBall, 5, stream);
                                client.Inventory.Add(stream, 727464, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            }
                            else
                            {
                                data.AddText("You do not have 100,000 silvers with you.")
                 .AddOption("Okay!", 255)
                 .AddAvatar(7).FinalizeDialog();
                            }
                        }
                        else if (client.Inventory.Contain(Database.ItemType.ExpBall, 5, 0))
                        {
                            if (client.Player.Money >= 100000)
                            {
                                client.Player.Money -= 100000;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Inventory.Remove(Database.ItemType.ExpBall, 5, stream);
                                client.Inventory.Add(stream, 727464, 1);
                            }
                            else
                            {
                                data.AddText("You do not have 100,000 silvers with you.")
                 .AddOption("Okay!", 255)
                 .AddAvatar(7).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You need to give me 5 EXP Balls to have them packed into an EXP Ball Gift Box.")
                  .AddOption("Okay!", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }

                        break;
                    }
                case 7:
                    {
                        data.AddText("Which kind of Normal Gems do you want to pack up?")
                   .AddOption("Glory~Gems.", 8)
                   .AddOption("Thunder~Gems.", 9)
                   .AddOption("Kylin~Gems.", 10)
                   .AddOption("Rainbow~Gems.", 11)
                   .AddOption("Fury~Gems.", 12)
                   .AddOption("Next~page.", 13)
                   .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 13:
                    {
                        data.AddText("Which kind of Normal Gems do you want to pack up?")
                  .AddOption("Dragon~Gems.", 14)
                  .AddOption("Phoenix~Gems.", 15)
                  .AddOption("Violet~Gems.", 16)
                  .AddOption("Moon~Gems.", 17)
                  .AddOption("Tortoise~Gems.", 18)
                  .AddOption("Previous.", 7)
                  .AddOption("Just~passing~by.", 255)
                  .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 18:
                    {
                        data.AddText("5 Normal Tortoise Gems can be packed into a Tortoise Gem Pack! Would you like me to pack them for you?")
                 .AddOption("Yes,~please.", 29)
                 .AddOption("Bye.", 255)
                 .AddAvatar(7).FinalizeDialog();


                        break;
                    }
                case 29:
                    {
                        if (client.Inventory.Contain((uint)(700000 + (uint)Role.Flags.Gem.NormalTortoiseGem), 5))
                        {
                            client.Inventory.Remove((uint)(700000 + (uint)Role.Flags.Gem.NormalTortoiseGem), 5, stream);
                            client.Inventory.Add(stream, 727069, 1);
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have 5 Tortoise Gems.")
                  .AddOption("Okay!", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }

                        break;
                    }
                case 17:
                    {
                        data.AddText("5 Normal Moon Gems can be packed into a Moon Gem Pack! Would you like me to pack them for you?")
                  .AddOption("Yes,~please.", 28)
                  .AddOption("Bye.", 255)
                  .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 28:
                    {
                        if (client.Inventory.Contain((uint)(700000 + (uint)Role.Flags.Gem.NormalMoonGem), 5))
                        {
                            client.Inventory.Remove((uint)(700000 + (uint)Role.Flags.Gem.NormalMoonGem), 5, stream);
                            client.Inventory.Add(stream, 727068, 1);
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have 5 Moon Gems.")
                  .AddOption("Okay!", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }
                case 16:
                    {
                        data.AddText("5 Normal Violet Gems can be packed into a Violet Gem Pack! Would you like me to pack them for you?")
                 .AddOption("Yes,~please.", 27)
                 .AddOption("Bye.", 255)
                 .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 27:
                    {
                        if (client.Inventory.Contain((uint)(700000 + (uint)Role.Flags.Gem.NormalVioletGem), 5))
                        {
                            client.Inventory.Remove((uint)(700000 + (uint)Role.Flags.Gem.NormalVioletGem), 5, stream);
                            client.Inventory.Add(stream, 727067, 1);
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have 5 Violet Gems.")
                  .AddOption("Okay!", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }
                case 15:
                    {
                        data.AddText("5 Normal Phoenix Gems can be packed into a Phoenix Gem Pack! Would you like me to pack them for you?")
                .AddOption("Yes,~please.", 26)
                .AddOption("Bye.", 255)
                .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 26:
                    {
                        if (client.Inventory.Contain((uint)(700000 + (uint)Role.Flags.Gem.NormalPhoenixGem), 5))
                        {
                            client.Inventory.Remove((uint)(700000 + (uint)Role.Flags.Gem.NormalPhoenixGem), 5, stream);
                            client.Inventory.Add(stream, 727066, 1);
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have 5 Phoenix Gems.")
                  .AddOption("Okay!", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }
                case 14:
                    {
                        data.AddText("5 Normal Dragon Gems can be packed into a Dragon Gem Pack! Would you like me to pack them for you?")
                  .AddOption("Yes,~please.", 25)
                  .AddOption("Bye.", 255)
                  .AddAvatar(7).FinalizeDialog();


                        break;
                    }
                case 25:
                    {
                        if (client.Inventory.Contain((uint)(700000 + (uint)Role.Flags.Gem.NormalDragonGem), 5))
                        {
                            client.Inventory.Remove((uint)(700000 + (uint)Role.Flags.Gem.NormalDragonGem), 5, stream);
                            client.Inventory.Add(stream, 727065, 1);
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have 5 Dragon Gems.")
                  .AddOption("Okay!", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }
                case 12:
                    {
                        data.AddText("5 Normal Fury Gems can be packed into a Fury Gem Pack! Would you like me to pack them for you?")
                  .AddOption("Yes,~please.", 24)
                  .AddOption("Bye.", 255)
                  .AddAvatar(7).FinalizeDialog();


                        break;
                    }
                case 24:
                    {
                        if (client.Inventory.Contain((uint)(700000 + (uint)Role.Flags.Gem.NormalFuryGem), 5))
                        {
                            client.Inventory.Remove((uint)(700000 + (uint)Role.Flags.Gem.NormalFuryGem), 5, stream);
                            client.Inventory.Add(stream, 727064, 1);
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have 5 Fury Gems.")
                  .AddOption("Okay!", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }
                case 11:
                    {
                        data.AddText("5 Normal Rainbow Gems can be packed into a Rainbow Gem Pack! Would you like me to pack them for you?")
                 .AddOption("Yes,~please.", 23)
                 .AddOption("Bye.", 255)
                 .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 23:
                    {
                        if (client.Inventory.Contain((uint)(700000 + (uint)Role.Flags.Gem.NormalRainbowGem), 5))
                        {
                            client.Inventory.Remove((uint)(700000 + (uint)Role.Flags.Gem.NormalRainbowGem), 5, stream);
                            client.Inventory.Add(stream, 727063, 1);
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have 5 Rainbow Gems.")
                  .AddOption("Okay!", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }

                        break;
                    }
                case 10:
                    {
                        data.AddText("5 Normal Kylin Gems can be packed into a Kylin Gem Pack! Would you like me to pack them for you?")
                  .AddOption("Yes,~please.", 22)
                  .AddOption("Bye.", 255)
                  .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 22:
                    {
                        if (client.Inventory.Contain((uint)(700000 + (uint)Role.Flags.Gem.NormalKylinGem), 5))
                        {
                            client.Inventory.Remove((uint)(700000 + (uint)Role.Flags.Gem.NormalKylinGem), 5, stream);
                            client.Inventory.Add(stream, 727062, 1);
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have 5 Kylin Gems.")
                  .AddOption("Okay!", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }
                case 9:
                    {
                        data.AddText("5 Normal Thunder Gems can be packed into a Thunder Gem Pack! Would you like me to pack them for you?")
                 .AddOption("Yes,~please.", 21)
                 .AddOption("Bye.", 255)
                 .AddAvatar(7).FinalizeDialog();


                        break;
                    }
                case 21:
                    {
                        if (client.Inventory.Contain((uint)(700000 + (uint)Role.Flags.Gem.NormalThunderGem), 5))
                        {
                            client.Inventory.Remove((uint)(700000 + (uint)Role.Flags.Gem.NormalThunderGem), 5, stream);
                            client.Inventory.Add(stream, 727061, 1);
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have 5 Thunder Gems.")
                  .AddOption("Okay!", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }

                case 8:
                    {
                        data.AddText("5 Normal Glory Gems can be packed into a Glory Gem Pack! Would you like me to pack them for you?")
                  .AddOption("Yes,~please.", 20)
                  .AddOption("Bye.", 255)
                  .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 20:
                    {
                        if (client.Inventory.Contain((uint)(700000 + (uint)Role.Flags.Gem.NormalGloryGem), 5))
                        {
                            client.Inventory.Remove((uint)(700000 + (uint)Role.Flags.Gem.NormalGloryGem), 5, stream);
                            client.Inventory.Add(stream, 727060, 1);
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have 5 Normal Glory Gems.")
                  .AddOption("Okay!", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }
                case 5:
                    {
                        data.AddText("10 +3 Stones can be packed into a +3 Stone Pack. Would you like me to pack them for you?")
                .AddOption("Yes,~please.", 6)
                .AddOption("Bye.", 255)
                .AddAvatar(7).FinalizeDialog();


                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.Contain(730003, 10))
                        {
                            client.Inventory.Remove(730003, 10, stream);
                            client.Inventory.Add(stream, 729023, 1);
                        }
                        else
                        {
                            data.AddText("You don`t have 10 +3 Stones in your inventory!")
                  .AddOption("Okay!", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }

                        break;
                    }
                case 3:
                    {
                        data.AddText("10 +2 Stones can be packed into a +2 Stone Pack. Would you like me to pack them for you?")
                  .AddOption("Yes,~please.", 4)
                  .AddOption("Bye.", 255)
                  .AddAvatar(7).FinalizeDialog();


                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.Contain(730002, 10))
                        {
                            client.Inventory.Remove(730002, 10, stream);
                            client.Inventory.Add(stream, 729022, 1);
                        }
                        else
                        {
                            data.AddText("You don`t have 10 +2 Stones in your inventory!")
                  .AddOption("Okay!", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }



                        break;
                    }
                case 1:
                    {
                        data.AddText("5 +1 Stones can be packed into a +1 Stone Pack. Would you like me to pack them for you?")
                 .AddOption("Yes,~please.", 2)
                 .AddOption("Bye.", 255)
                 .AddAvatar(7).FinalizeDialog();


                        break;
                    }
                case 2:
                    {

                        if (client.Inventory.Contain(730001, 5, 1))
                        {
                            client.Inventory.Remove(730001, 5, stream);
                            client.Inventory.Add(stream, 723712, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        }
                        else if (client.Inventory.Contain(730001, 5, 0))
                        {
                            client.Inventory.Remove(730001, 5, stream);
                            client.Inventory.Add(stream, 723712, 1);
                        }
                        else
                        {
                            data.AddText("You don`t have 5 +1 Stones in your inventory!")
                  .AddOption("Okay!", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Shopboy2)]
        public static void Shopboy2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Shopboy:\n");
                        data.AddText(" There is never lack of opportunity to get rich in Twin City, and you just need to find it. See what, I`ve decided to expand my business to packing services.    The charges must be certainly fair. How may I help you")
                    .AddOption("Further pack Meteor Scrolls.", 38)
                    .AddOption("Further pack DB Scrolls.", 40)
                    .AddOption("Pack~Meteors.", 34)
                    .AddOption("Pack~+1~Stones.", 1)
                    .AddOption("Pack~+2~Stones.", 3)
                    .AddOption("Pack~+3~Stones.", 5)
                    .AddOption("Pack~Dragon Balls.", 36)
                    .AddOption("Pack~normal~Gems.", 7)
                    .AddOption("Next~page.", 72)
                    .AddOption("I`m just passing by.", 255)
                    .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 73:
                    {
                        data.AddText("Shopboy:\n");
                        data.AddText(" There is never lack of opportunity to get rich in Twin City, and you just need to find it. See what, I`ve decided to expand my business to packing services.    The charges must be certainly fair. How may I help you")
                    .AddOption("Further pack Meteor Scrolls.", 38)
                    .AddOption("Further pack DB Scrolls.", 40)
                    .AddOption("Pack~Meteors.", 34)
                    .AddOption("Pack~+1~Stones.", 1)
                    .AddOption("Pack~+2~Stones.", 3)
                    .AddOption("Pack~+3~Stones.", 5)
                    .AddOption("Pack~Dragon Balls.", 36)
                    .AddOption("Pack~normal~Gems.", 7)
                    .AddOption("Next~page.", 72)
                    .AddOption("I`m just passing by.", 255)
                    .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 72:
                    {
                        data.AddText("Shopboy:\n");
                        data.AddText(" There is never lack of opportunity to get rich in Twin City, and you just need to find it. See what, I`ve decided to expand my business to packing services.    The charges must be certainly fair. How may I help you?")
                           .AddOption("Pack~EXP~Balls~for~me!", 32)
                           .AddOption("Pack~Diligence~Books.", 255)
                           .AddOption("Pack~Endurance~Books.", 255)
                           .AddOption("Pack~EXP~Ball~Fragment.", 255)
                           .AddOption("Previous~page.", 73)
                           .AddOption("I`m just passing by.", 255)
                           .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 40:
                    {
                        data.AddText("I can pack 10 DB Scrolls into a Super DB Scroll. Do you want me to pack them for you?")
                           .AddOption("Yes.", 41)
                           .AddOption("I`ll talk to you later.", 255)
                           .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 41:
                    {
                        if (client.Inventory.Contain(Database.ItemType.DBScroll, 10, 1))
                        {
                            client.Inventory.Remove(Database.ItemType.DBScroll, 10, stream);
                            client.Inventory.Add(stream, Database.ItemType.SuperDBScroll, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        }
                        else if (client.Inventory.Contain(Database.ItemType.DBScroll, 10))
                        {
                            client.Inventory.Remove(Database.ItemType.DBScroll, 10, stream);
                            client.Inventory.Add(stream, Database.ItemType.SuperDBScroll, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false);

                        }
                        else
                        {
                            data.AddText("Please make sure you have 10 DB Scrolls in your inventory, first.")
                  .AddOption("Okay!", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }

                        break;
                    }
                case 38:
                    {
                        data.AddText("I can pack 10 Meteor Scrolls into a Super Meteor Scroll. Do you want me to pack them for you?")
                           .AddOption("Yes.", 39)
                           .AddOption("I`ll talk to you later.", 255)
                           .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 39:
                    {
                        if (client.Inventory.Contain(Database.ItemType.MeteorScroll, 10, 1))
                        {
                            client.Inventory.Remove(Database.ItemType.MeteorScroll, 10, stream);
                            client.Inventory.Add(stream, Database.ItemType.SuperMeteorScroll, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        }
                        else if (client.Inventory.Contain(Database.ItemType.MeteorScroll, 10, 0))
                        {
                            client.Inventory.Remove(Database.ItemType.MeteorScroll, 10, stream);
                            client.Inventory.Add(stream, Database.ItemType.SuperMeteorScroll, 1);
                        }
                        else
                        {
                            data.AddText("Please make sure you have 10 Meteor Scrolls in your inventory, first.")
                  .AddOption("Okay!", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }

                        break;
                    }
                case 36:
                    {
                        data.AddText("I can pack 10 Dragon Balls into a DB Scroll.")
                           .AddOption("Okay. Pack my DBs.", 37)
                           .AddOption("Bye.", 255)
                           .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 37:
                    {
                        if (client.Inventory.Contain(Database.ItemType.DragonBall, 10, 1))
                        {
                            client.Inventory.Remove(Database.ItemType.DragonBall, 10, stream);
                            client.Inventory.Add(stream, Database.ItemType.DBScroll, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        }
                        else if (client.Inventory.Contain(Database.ItemType.DragonBall, 10, 0))
                        {
                            client.Inventory.Remove(Database.ItemType.DragonBall, 10, stream);
                            client.Inventory.Add(stream, Database.ItemType.DBScroll, 1);
                        }
                        else
                        {
                            data.AddText("Please make sure you have 10 Dragon Balls, first.")
                  .AddOption("Okay!", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }

                        break;
                    }

                case 34:
                    {
                        data.AddText("I can pack 10 Meteors into a Meteor Scroll. Do you want me to pack them for you")
                            .AddOption("Yes.", 35)
                            .AddOption("Bye.", 255)
                            .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 35:
                    {
                        if (client.Inventory.Contain(Database.ItemType.Meteor, 10, 1))
                        {
                            client.Inventory.Remove(Database.ItemType.Meteor, 10, stream);
                            client.Inventory.Add(stream, Database.ItemType.MeteorScroll, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        }
                        else if (client.Inventory.Contain(Database.ItemType.Meteor, 10, 0))
                        {
                            client.Inventory.Remove(Database.ItemType.Meteor, 10, stream);
                            client.Inventory.Add(stream, Database.ItemType.MeteorScroll, 1);
                        }
                        else
                        {
                            data.AddText("Please make sure you have 10 Meteors, first.")
                  .AddOption("Okay!", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }

                        break;
                    }
                case 32:
                    {
                        data.AddText("Pay 100,000 Silver and give me 5 EXP Balls, I can pack them into an EXP Ball Gift Box for you!")
                                                    .AddOption("Pack~EXP~Balls~for~me!", 33)
                                                    .AddOption("No, thanks.", 255)
                                                    .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 33:
                    {
                        if (client.Inventory.Contain(Database.ItemType.ExpBall, 5, 1))
                        {
                            if (client.Player.Money >= 100000)
                            {
                                client.Player.Money -= 100000;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Inventory.Remove(Database.ItemType.ExpBall, 5, stream);
                                client.Inventory.Add(stream, 727464, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            }
                            else
                            {
                                data.AddText("You do not have 100,000 silvers with you.")
                 .AddOption("Okay!", 255)
                 .AddAvatar(7).FinalizeDialog();
                            }
                        }
                        else if (client.Inventory.Contain(Database.ItemType.ExpBall, 5, 0))
                        {
                            if (client.Player.Money >= 100000)
                            {
                                client.Player.Money -= 100000;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Inventory.Remove(Database.ItemType.ExpBall, 5, stream);
                                client.Inventory.Add(stream, 727464, 1);
                            }
                            else
                            {
                                data.AddText("You do not have 100,000 silvers with you.")
                 .AddOption("Okay!", 255)
                 .AddAvatar(7).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You need to give me 5 EXP Balls to have them packed into an EXP Ball Gift Box.")
                  .AddOption("Okay!", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }

                        break;
                    }
                case 7:
                    {
                        data.AddText("Which kind of Normal Gems do you want to pack up?")
                   .AddOption("Glory~Gems.", 8)
                   .AddOption("Thunder~Gems.", 9)
                   .AddOption("Kylin~Gems.", 10)
                   .AddOption("Rainbow~Gems.", 11)
                   .AddOption("Fury~Gems.", 12)
                   .AddOption("Next~page.", 13)
                   .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 13:
                    {
                        data.AddText("Which kind of Normal Gems do you want to pack up?")
                  .AddOption("Dragon~Gems.", 14)
                  .AddOption("Phoenix~Gems.", 15)
                  .AddOption("Violet~Gems.", 16)
                  .AddOption("Moon~Gems.", 17)
                  .AddOption("Tortoise~Gems.", 18)
                  .AddOption("Previous.", 7)
                  .AddOption("Just~passing~by.", 255)
                  .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 18:
                    {
                        data.AddText("5 Normal Tortoise Gems can be packed into a Tortoise Gem Pack! Would you like me to pack them for you?")
                 .AddOption("Yes,~please.", 29)
                 .AddOption("Bye.", 255)
                 .AddAvatar(7).FinalizeDialog();


                        break;
                    }
                case 29:
                    {
                        if (client.Inventory.Contain((uint)(700000 + (uint)Role.Flags.Gem.NormalTortoiseGem), 5))
                        {
                            client.Inventory.Remove((uint)(700000 + (uint)Role.Flags.Gem.NormalTortoiseGem), 5, stream);
                            client.Inventory.Add(stream, 727069, 1);
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have 5 Tortoise Gems.")
                  .AddOption("Okay!", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }

                        break;
                    }
                case 17:
                    {
                        data.AddText("5 Normal Moon Gems can be packed into a Moon Gem Pack! Would you like me to pack them for you?")
                  .AddOption("Yes,~please.", 28)
                  .AddOption("Bye.", 255)
                  .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 28:
                    {
                        if (client.Inventory.Contain((uint)(700000 + (uint)Role.Flags.Gem.NormalMoonGem), 5))
                        {
                            client.Inventory.Remove((uint)(700000 + (uint)Role.Flags.Gem.NormalMoonGem), 5, stream);
                            client.Inventory.Add(stream, 727068, 1);
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have 5 Moon Gems.")
                  .AddOption("Okay!", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }
                case 16:
                    {
                        data.AddText("5 Normal Violet Gems can be packed into a Violet Gem Pack! Would you like me to pack them for you?")
                 .AddOption("Yes,~please.", 27)
                 .AddOption("Bye.", 255)
                 .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 27:
                    {
                        if (client.Inventory.Contain((uint)(700000 + (uint)Role.Flags.Gem.NormalVioletGem), 5))
                        {
                            client.Inventory.Remove((uint)(700000 + (uint)Role.Flags.Gem.NormalVioletGem), 5, stream);
                            client.Inventory.Add(stream, 727067, 1);
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have 5 Violet Gems.")
                  .AddOption("Okay!", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }
                case 15:
                    {
                        data.AddText("5 Normal Phoenix Gems can be packed into a Phoenix Gem Pack! Would you like me to pack them for you?")
                .AddOption("Yes,~please.", 26)
                .AddOption("Bye.", 255)
                .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 26:
                    {
                        if (client.Inventory.Contain((uint)(700000 + (uint)Role.Flags.Gem.NormalPhoenixGem), 5))
                        {
                            client.Inventory.Remove((uint)(700000 + (uint)Role.Flags.Gem.NormalPhoenixGem), 5, stream);
                            client.Inventory.Add(stream, 727066, 1);
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have 5 Phoenix Gems.")
                  .AddOption("Okay!", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }
                case 14:
                    {
                        data.AddText("5 Normal Dragon Gems can be packed into a Dragon Gem Pack! Would you like me to pack them for you?")
                  .AddOption("Yes,~please.", 25)
                  .AddOption("Bye.", 255)
                  .AddAvatar(7).FinalizeDialog();


                        break;
                    }
                case 25:
                    {
                        if (client.Inventory.Contain((uint)(700000 + (uint)Role.Flags.Gem.NormalDragonGem), 5))
                        {
                            client.Inventory.Remove((uint)(700000 + (uint)Role.Flags.Gem.NormalDragonGem), 5, stream);
                            client.Inventory.Add(stream, 727065, 1);
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have 5 Dragon Gems.")
                  .AddOption("Okay!", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }
                case 12:
                    {
                        data.AddText("5 Normal Fury Gems can be packed into a Fury Gem Pack! Would you like me to pack them for you?")
                  .AddOption("Yes,~please.", 24)
                  .AddOption("Bye.", 255)
                  .AddAvatar(7).FinalizeDialog();


                        break;
                    }
                case 24:
                    {
                        if (client.Inventory.Contain((uint)(700000 + (uint)Role.Flags.Gem.NormalFuryGem), 5))
                        {
                            client.Inventory.Remove((uint)(700000 + (uint)Role.Flags.Gem.NormalFuryGem), 5, stream);
                            client.Inventory.Add(stream, 727064, 1);
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have 5 Fury Gems.")
                  .AddOption("Okay!", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }
                case 11:
                    {
                        data.AddText("5 Normal Rainbow Gems can be packed into a Rainbow Gem Pack! Would you like me to pack them for you?")
                 .AddOption("Yes,~please.", 23)
                 .AddOption("Bye.", 255)
                 .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 23:
                    {
                        if (client.Inventory.Contain((uint)(700000 + (uint)Role.Flags.Gem.NormalRainbowGem), 5))
                        {
                            client.Inventory.Remove((uint)(700000 + (uint)Role.Flags.Gem.NormalRainbowGem), 5, stream);
                            client.Inventory.Add(stream, 727063, 1);
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have 5 Rainbow Gems.")
                  .AddOption("Okay!", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }

                        break;
                    }
                case 10:
                    {
                        data.AddText("5 Normal Kylin Gems can be packed into a Kylin Gem Pack! Would you like me to pack them for you?")
                  .AddOption("Yes,~please.", 22)
                  .AddOption("Bye.", 255)
                  .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 22:
                    {
                        if (client.Inventory.Contain((uint)(700000 + (uint)Role.Flags.Gem.NormalKylinGem), 5))
                        {
                            client.Inventory.Remove((uint)(700000 + (uint)Role.Flags.Gem.NormalKylinGem), 5, stream);
                            client.Inventory.Add(stream, 727062, 1);
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have 5 Kylin Gems.")
                  .AddOption("Okay!", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }
                case 9:
                    {
                        data.AddText("5 Normal Thunder Gems can be packed into a Thunder Gem Pack! Would you like me to pack them for you?")
                 .AddOption("Yes,~please.", 21)
                 .AddOption("Bye.", 255)
                 .AddAvatar(7).FinalizeDialog();


                        break;
                    }
                case 21:
                    {
                        if (client.Inventory.Contain((uint)(700000 + (uint)Role.Flags.Gem.NormalThunderGem), 5))
                        {
                            client.Inventory.Remove((uint)(700000 + (uint)Role.Flags.Gem.NormalThunderGem), 5, stream);
                            client.Inventory.Add(stream, 727061, 1);
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have 5 Thunder Gems.")
                  .AddOption("Okay!", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }

                case 8:
                    {
                        data.AddText("5 Normal Glory Gems can be packed into a Glory Gem Pack! Would you like me to pack them for you?")
                  .AddOption("Yes,~please.", 20)
                  .AddOption("Bye.", 255)
                  .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 20:
                    {
                        if (client.Inventory.Contain((uint)(700000 + (uint)Role.Flags.Gem.NormalGloryGem), 5))
                        {
                            client.Inventory.Remove((uint)(700000 + (uint)Role.Flags.Gem.NormalGloryGem), 5, stream);
                            client.Inventory.Add(stream, 727060, 1);
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have 5 Normal Glory Gems.")
                  .AddOption("Okay!", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }
                case 5:
                    {
                        data.AddText("10 +3 Stones can be packed into a +3 Stone Pack. Would you like me to pack them for you?")
                .AddOption("Yes,~please.", 6)
                .AddOption("Bye.", 255)
                .AddAvatar(7).FinalizeDialog();


                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.Contain(730003, 10))
                        {
                            client.Inventory.Remove(730003, 10, stream);
                            client.Inventory.Add(stream, 729023, 1);
                        }
                        else
                        {
                            data.AddText("You don`t have 10 +3 Stones in your inventory!")
                  .AddOption("Okay!", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }

                        break;
                    }
                case 3:
                    {
                        data.AddText("10 +2 Stones can be packed into a +2 Stone Pack. Would you like me to pack them for you?")
                  .AddOption("Yes,~please.", 4)
                  .AddOption("Bye.", 255)
                  .AddAvatar(7).FinalizeDialog();


                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.Contain(730002, 10))
                        {
                            client.Inventory.Remove(730002, 10, stream);
                            client.Inventory.Add(stream, 729022, 1);
                        }
                        else
                        {
                            data.AddText("You don`t have 10 +2 Stones in your inventory!")
                  .AddOption("Okay!", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }



                        break;
                    }
                case 1:
                    {
                        data.AddText("5 +1 Stones can be packed into a +1 Stone Pack. Would you like me to pack them for you?")
                 .AddOption("Yes,~please.", 2)
                 .AddOption("Bye.", 255)
                 .AddAvatar(7).FinalizeDialog();


                        break;
                    }
                case 2:
                    {

                        if (client.Inventory.Contain(730001, 5, 1))
                        {
                            client.Inventory.Remove(730001, 5, stream);
                            client.Inventory.Add(stream, 723712, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        }
                        else if (client.Inventory.Contain(730001, 5, 0))
                        {
                            client.Inventory.Remove(730001, 5, stream);
                            client.Inventory.Add(stream, 723712, 1);
                        }
                        else
                        {
                            data.AddText("You don`t have 5 +1 Stones in your inventory!")
                  .AddOption("Okay!", 255)
                  .AddAvatar(7).FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.UnknowMan)]
        public static void UnknowMan(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("The Dragon Ball is really a precious item. You can use it to refine equipment. What`s more, if you use it properly,")
                            .AddText("~you can gain a large amount of EXP from it. Well, if you have a Dragon Ball, I can help you gain some EXP.")
                    .AddOption("Please~help~me.", 1)
                    .AddOption("Definitely.", 255)
                    .AddAvatar(67).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        data.AddText("According to your ability, I can help you gain EXP from 1 Dragon Ball, each day.")
                    .AddOption("Please~help~me.", 2)
                    .AddOption("I~see.", 255)
                    .AddAvatar(67).FinalizeDialog();


                        break;
                    }
                case 2:
                    {
                        if (client.Player.Level == 140)
                        {
                            data.AddText("Sorry, but you have a big level.")
              .AddOption("I~see.", 255)
              .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        if (client.Inventory.Contain(Database.ItemType.DragonBall, 1))
                        {
                            if (!client.Player.DbTry)
                            {
                                client.Player.DbTry = true;
                                client.GainExpBall(600 * 10, true, Role.Flags.ExperienceEffect.angelwing);
                                client.Inventory.Remove(Database.ItemType.DragonBall, 1, stream);
                            }
                            else
                            {
                                data.AddText("Sorry, but you my service today, come back tomorow with one dragonball.")
                 .AddOption("I~see.", 255)
                 .AddAvatar(67).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough Dragon Balls.")
                   .AddOption("I~see.", 255)
                   .AddAvatar(67).FinalizeDialog();
                        }


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.CollectorZhao)]//Edit-By[MohyElDin]..
        public static void CollectorZhao(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I~am~fond~of~collecting~all~kinds~of~treasure.~If~you~happen~to~have~any~Disguise Amulets,~Life Fruit Basket,~or")
                            .AddText("~PenitenceAmulets,~you~may~sell~them~to~me.~I~will~pay~you~ten~CPs~for~each~one.")
                    .AddOption("Here are my Disguise Amulets (No~more~than~15).", 1)
                    .AddOption("Here are my Life Fruit Baskets (No~more~than~15).", 2)
                    .AddOption("Here are my Penitence Amulets (No~more~than~15).", 3)
                    .AddOption("I~have~none~of~them.", 255)
                    .AddAvatar(10).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("How many do you want to sell")
                           .AddOption("1.", 80)
                           .AddOption("5.", 81)
                           .AddOption("10.", 82)
                           .AddOption("15.", 83)
                           .AddOption("I don`t have any.", 255)
                           .AddAvatar(10).FinalizeDialog();

                        break;
                    }






                case 80:
                    {
                        if (client.Inventory.Contain(723724, 1))
                        {
                            client.Inventory.Remove(723724, 1, stream);
                            client.Player.ConquerPoints += 10;


                            data.AddText("You received 10 CPs in your inventory.")
                   .AddOption("Okay~thank~you.", 255)
                   .AddAvatar(10).FinalizeDialog();
                        }
                        else
                        {

                            data.AddText("You~do~not~have~what~I~want.~Whenever~you~have~one~of~DisguiseAmulets,~LifeFruitBaskets~or~PenitenceAmulets,~just~come~here.")
                   .AddOption("See~you~again.", 255)
                   .AddAvatar(10).FinalizeDialog();

                        }


                        break;
                    }
                case 81:
                    {
                        if (client.Inventory.Contain(723724, 5))
                        {
                            client.Inventory.Remove(723724, 5, stream);
                            client.Player.ConquerPoints += 50;


                            data.AddText("You received 50 CPs in your inventory.")
                   .AddOption("Okay~thank~you.", 255)
                   .AddAvatar(10).FinalizeDialog();
                        }
                        else
                        {

                            data.AddText("You~do~not~have~what~I~want.~Whenever~you~have~one~of~DisguiseAmulets,~LifeFruitBaskets~or~PenitenceAmulets,~just~come~here.")
                   .AddOption("See~you~again.", 255)
                   .AddAvatar(10).FinalizeDialog();

                        }


                        break;
                    }
                case 82:
                    {
                        if (client.Inventory.Contain(723724, 10))
                        {
                            client.Inventory.Remove(723724, 10, stream);
                            client.Player.ConquerPoints += 100;


                            data.AddText("You received 100 CPs in your inventory.")
                   .AddOption("Okay~thank~you.", 255)
                   .AddAvatar(10).FinalizeDialog();
                        }
                        else
                        {

                            data.AddText("You~do~not~have~what~I~want.~Whenever~you~have~one~of~DisguiseAmulets,~LifeFruitBaskets~or~PenitenceAmulets,~just~come~here.")
                   .AddOption("See~you~again.", 255)
                   .AddAvatar(10).FinalizeDialog();

                        }


                        break;
                    }
                case 83:
                    {
                        if (client.Inventory.Contain(723724, 15))
                        {
                            client.Inventory.Remove(723724, 15, stream);
                            client.Player.ConquerPoints += 150;


                            data.AddText("You received 150 CPs in your inventory.")
                   .AddOption("Okay~thank~you.", 255)
                   .AddAvatar(10).FinalizeDialog();
                        }
                        else
                        {

                            data.AddText("You~do~not~have~what~I~want.~Whenever~you~have~one~of~DisguiseAmulets,~LifeFruitBaskets~or~PenitenceAmulets,~just~come~here.")
                   .AddOption("See~you~again.", 255)
                   .AddAvatar(10).FinalizeDialog();

                        }


                        break;
                    }
                case 2:
                    {
                        data.AddText("How many do you want to sell")
                           .AddOption("1.", 85)
                           .AddOption("5.", 86)
                           .AddOption("10.", 87)
                           .AddOption("15.", 88)
                           .AddOption("I don`t have any.", 255)
                           .AddAvatar(10).FinalizeDialog();

                        break;
                    }






                case 85:
                    {
                        if (client.Inventory.Contain(723725, 1))
                        {
                            client.Inventory.Remove(723725, 1, stream);
                            client.Player.ConquerPoints += 10;


                            data.AddText("You received 10 CPs in your inventory.")
                   .AddOption("Okay~thank~you.", 255)
                   .AddAvatar(10).FinalizeDialog();
                        }
                        else
                        {

                            data.AddText("You~do~not~have~what~I~want.~Whenever~you~have~one~of~DisguiseAmulets,~LifeFruitBaskets~or~PenitenceAmulets,~just~come~here.")
                   .AddOption("See~you~again.", 255)
                   .AddAvatar(10).FinalizeDialog();

                        }


                        break;
                    }
                case 86:
                    {
                        if (client.Inventory.Contain(723725, 5))
                        {
                            client.Inventory.Remove(723725, 5, stream);
                            client.Player.ConquerPoints += 50;


                            data.AddText("You received 50 CPs in your inventory.")
                   .AddOption("Okay~thank~you.", 255)
                   .AddAvatar(10).FinalizeDialog();
                        }
                        else
                        {

                            data.AddText("You~do~not~have~what~I~want.~Whenever~you~have~one~of~DisguiseAmulets,~LifeFruitBaskets~or~PenitenceAmulets,~just~come~here.")
                   .AddOption("See~you~again.", 255)
                   .AddAvatar(10).FinalizeDialog();

                        }


                        break;
                    }
                case 87:
                    {
                        if (client.Inventory.Contain(723725, 10))
                        {
                            client.Inventory.Remove(723725, 10, stream);
                            client.Player.ConquerPoints += 100;


                            data.AddText("You received 100 CPs in your inventory.")
                   .AddOption("Okay~thank~you.", 255)
                   .AddAvatar(10).FinalizeDialog();
                        }
                        else
                        {

                            data.AddText("You~do~not~have~what~I~want.~Whenever~you~have~one~of~DisguiseAmulets,~LifeFruitBaskets~or~PenitenceAmulets,~just~come~here.")
                   .AddOption("See~you~again.", 255)
                   .AddAvatar(10).FinalizeDialog();

                        }


                        break;
                    }
                case 88:
                    {
                        if (client.Inventory.Contain(723725, 15))
                        {
                            client.Inventory.Remove(723725, 15, stream);
                            client.Player.ConquerPoints += 150;


                            data.AddText("You received 150 CPs in your inventory.")
                   .AddOption("Okay~thank~you.", 255)
                   .AddAvatar(10).FinalizeDialog();
                        }
                        else
                        {

                            data.AddText("You~do~not~have~what~I~want.~Whenever~you~have~one~of~DisguiseAmulets,~LifeFruitBaskets~or~PenitenceAmulets,~just~come~here.")
                   .AddOption("See~you~again.", 255)
                   .AddAvatar(10).FinalizeDialog();

                        }


                        break;
                    }
                case 3:
                    {
                        data.AddText("How many do you want to sell")
                           .AddOption("1.", 89)
                           .AddOption("5.", 90)
                           .AddOption("10.", 91)
                           .AddOption("15.", 92)
                           .AddOption("I don`t have any.", 255)
                           .AddAvatar(10).FinalizeDialog();

                        break;
                    }






                case 89:
                    {
                        if (client.Inventory.Contain(723727, 1))
                        {
                            client.Inventory.Remove(723727, 1, stream);
                            client.Player.ConquerPoints += 10;


                            data.AddText("You received 10 CPs in your inventory.")
                   .AddOption("Okay~thank~you.", 255)
                   .AddAvatar(10).FinalizeDialog();
                        }
                        else
                        {

                            data.AddText("You~do~not~have~what~I~want.~Whenever~you~have~one~of~DisguiseAmulets,~LifeFruitBaskets~or~PenitenceAmulets,~just~come~here.")
                   .AddOption("See~you~again.", 255)
                   .AddAvatar(10).FinalizeDialog();

                        }


                        break;
                    }
                case 90:
                    {
                        if (client.Inventory.Contain(723727, 5))
                        {
                            client.Inventory.Remove(723727, 5, stream);
                            client.Player.ConquerPoints += 50;


                            data.AddText("You received 50 CPs in your inventory.")
                   .AddOption("Okay~thank~you.", 255)
                   .AddAvatar(10).FinalizeDialog();
                        }
                        else
                        {

                            data.AddText("You~do~not~have~what~I~want.~Whenever~you~have~one~of~DisguiseAmulets,~LifeFruitBaskets~or~PenitenceAmulets,~just~come~here.")
                   .AddOption("See~you~again.", 255)
                   .AddAvatar(10).FinalizeDialog();

                        }


                        break;
                    }
                case 91:
                    {
                        if (client.Inventory.Contain(723727, 10))
                        {
                            client.Inventory.Remove(723727, 10, stream);
                            client.Player.ConquerPoints += 100;


                            data.AddText("You received 100 CPs in your inventory.")
                   .AddOption("Okay~thank~you.", 255)
                   .AddAvatar(10).FinalizeDialog();
                        }
                        else
                        {

                            data.AddText("You~do~not~have~what~I~want.~Whenever~you~have~one~of~DisguiseAmulets,~LifeFruitBaskets~or~PenitenceAmulets,~just~come~here.")
                   .AddOption("See~you~again.", 255)
                   .AddAvatar(10).FinalizeDialog();

                        }


                        break;
                    }
                case 92:
                    {
                        if (client.Inventory.Contain(723727, 15))
                        {
                            client.Inventory.Remove(723727, 15, stream);
                            client.Player.ConquerPoints += 150;


                            data.AddText("You received 150 CPs in your inventory.")
                   .AddOption("Okay~thank~you.", 255)
                   .AddAvatar(10).FinalizeDialog();
                        }
                        else
                        {

                            data.AddText("You~do~not~have~what~I~want.~Whenever~you~have~one~of~DisguiseAmulets,~LifeFruitBaskets~or~PenitenceAmulets,~just~come~here.")
                   .AddOption("See~you~again.", 255)
                   .AddAvatar(10).FinalizeDialog();

                        }


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.LotteryCollectorWong)]
        public static void LotteryCollectorWong(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I~am~fond~of~collecting~all~kinds~of~treasure.~If~you~happen~to~have~any~Disguise Amulets,~Life Fruit Basket,~or")
                            .AddText("~PenitenceAmulets,~you~may~sell~them~to~me.~I~will~pay~you~ten~CPs~for~each~one.")
                    .AddOption("Here~are~my~Disguise~Amulets.", 1)
                    .AddOption("Here~are~my~Life~Fruit~Baskets.", 2)
                    .AddOption("Here~are~my~Penitence~Amulets.", 3)
                    .AddOption("Really?~I~don`t~believe~it.", 255)
                    .AddAvatar(10).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(723724, 1))
                        {
                            client.Inventory.Remove(723724, 1, stream);
                            client.Player.ConquerPoints += 15;


                            data.AddText("You receive 15 ConquerPoints for you item.")
                   .AddOption("Ok~thank~you.", 255)
                   .AddAvatar(10).FinalizeDialog();
                        }
                        else
                        {

                            data.AddText("You~do~not~have~what~I~want.~Whenever~you~have~one~of~DisguiseAmulets,~LifeFruitBaskets~or~PenitenceAmulets,~just~come~here.")
                   .AddOption("See~you~again.", 255)
                   .AddAvatar(10).FinalizeDialog();

                        }


                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(723725, 1))
                        {
                            client.Inventory.Remove(723725, 1, stream);
                            client.Player.ConquerPoints += 15;


                            data.AddText("You receive 15 ConquerPoints for you item.")
                   .AddOption("Ok~thank~you.", 255)
                   .AddAvatar(10).FinalizeDialog();
                        }
                        else
                        {

                            data.AddText("You~do~not~have~what~I~want.~Whenever~you~have~one~of~DisguiseAmulets,~LifeFruitBaskets~or~PenitenceAmulets,~just~come~here.")
                   .AddOption("See~you~again.", 255)
                   .AddAvatar(10).FinalizeDialog();

                        }


                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.Contain(723727, 1))
                        {
                            client.Inventory.Remove(723727, 1, stream);
                            client.Player.ConquerPoints += 15;


                            data.AddText("You receive 15 ConquerPoints for you item.")
                   .AddOption("Ok~thank~you.", 255)
                   .AddAvatar(10).FinalizeDialog();
                        }
                        else
                        {

                            data.AddText("You~do~not~have~what~I~want.~Whenever~you~have~one~of~DisguiseAmulets,~LifeFruitBaskets~or~PenitenceAmulets,~just~come~here.")
                   .AddOption("See~you~again.", 255)
                   .AddAvatar(10).FinalizeDialog();

                        }


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.LotteryLadyLuck)]
        public static void LotteryLadyLuck(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Look!~Here~is~the~LuckyBox,~a~kind~of~magical~box~which~makes~your~dream~come~true.")
                                            .AddOption("Get~me~out~of~here!", 1)
                                            .AddOption("Tell~me~more~about~it~please.", 2)
                                            .AddOption("Really?~I~don`t~believe~it.", 255)
                                            .AddAvatar(159).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        client.Teleport(Places.TwinCity);
                        break;
                    }
                case 2:
                    {
                        data.AddText("You~have~chance~to~get~whatever~you~desire~from~a~LuckyBox.~It~May~be~a~Dragonball,~+8~weapon,~or")
                            .AddText("~two-socket~boot,~you~don`t~know~what~you~will~get~from~the~Box,~it~totally~depends~on~your~luck.~Well,")
                            .AddText("~let`s~get~started~now.~Good~luck!")
                   .AddOption("Thanks~a~lot.", 255)
                   .AddAvatar(123).FinalizeDialog();

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.MarketLadyLuck)] // The NPC Edit By Mohy ElDin ...
        public static void MarketLadyLuck(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Welcome to the Lottery Center! There are many Lucky Boxes here, each containing incredible treasures! Pay me 3 Small Lottery Tickets to draw from the Lucky Box in the Lottery Center. You are allowed to enter the Lottery Center and try your luck upto 10")
                            .AddText("~times a day. You will have extra lottery chances if you are a VIP player.")
                    .AddOption("Let`s~try~my~luck!", 1)
                    .AddOption("Exchange~Small~Lottery~Ticket.", 4)
                    .AddOption("May~I~know~the~rules~first", 3)
                    .AddOption("How~about~the~VIP~privilege", 30)
                    .AddOption("Nice~to~see~you.", 255)
                    .AddAvatar(42).FinalizeDialog();


                        break;
                    }
                case 12:
                    {
                        data.AddText("Welcome to the Lottery Center! There are many Lucky Boxes here, each containing incredible treasures! Pay me 3 Small Lottery Tickets to draw from the Lucky Box in the Lottery Center. You are allowed to enter the Lottery Center and try your luck upto 10")
                            .AddText("~times a day. You will have extra lottery chances if you are a VIP player.")
                    .AddOption("Let`s~try~my~luck!", 1)
                    .AddOption("Exchange~Anima~P8~Ticket.", 4)
                    .AddOption("May~I~know~the~rules~first", 3)
                    .AddOption("How~about~the~VIP~privilege", 30)
                    .AddOption("Nice~to~see~you.", 255)
                    .AddAvatar(42).FinalizeDialog();


                        break;
                    }
                case 30:
                    {
                        data.AddText("Each player can draw from the Lottery up to 10 times, each day. But if you are a VIP, you will have extra chances to play! Level 1 VIP: 10 extra chances. Level 2 VIP: 20 extra chances. Level 3 VIP: 30 extra chances. Level 4 VIP: 40 extra chances. Level 5 VIP: 50 extra chances. Level 6 VIP: 60 extra chances.")
                            // .AddText("~times a day. You will have extra lottery chances if you are a VIP player.")
                            //.AddOption("Let`s~try~my~luck!", 1)
                            //.AddOption("Exchange~Small~Lottery~Ticket.", 4)
                            //.AddOption("May~I~know~the~rules~first", 3)
                            //.AddOption("How~about~the~VIP~privilege", 4)
                    .AddOption("Got it!", 255)
                    .AddAvatar(42).FinalizeDialog();


                        break;
                    }
                case 1:
                    {

                        var Mohy666 = new ActionQuery();
                        Mohy666.ObjId = client.Player.UID;
                        Mohy666.dwParam = 0;
                        Mohy666.Timestamp = (uint)Time32.timeGetTime().GetHashCode();
                        Mohy666.Type = 258;
                        Mohy666.Fascing = 0;
                        Mohy666.PositionX = 0;
                        Mohy666.PositionY = 0;
                        Mohy666.ScheduleTime = 0;
                        Mohy666.RemainTime = 0;
                        Mohy666.Strings = new string[] { "LuaUIMgr_OpenWindow|924" };
                        client.Send(stream.ActionCreate(Mohy666));
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(710212, 1))
                        {
                            client.Inventory.Remove(710212, 1, stream);
                            client.Inventory.AddItemWitchStack(711504, 0, 3, stream);
                        }
                        else
                        {
                            data.AddText("You don`t have any Lottery Tickets to exchange for the Small Lottery Tickets.")
                   .AddOption("Okay.", 255)
                   .AddAvatar(42).FinalizeDialog();
                        }


                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.Contain(710212, 5))
                        {
                            client.Inventory.Remove(710212, 5, stream);
                            client.Inventory.AddItemWitchStack(711504, 0, 15, stream);
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have enough Lottery Tickets. You can exchange the tickets one by one, or find me again when you have 5 Lottery Tickets.")
                   .AddOption("Okay.", 255)
                   .AddAvatar(42).FinalizeDialog();
                        }


                        break;
                    }
                case 4:
                    {
                        data.AddText("How many Lottery Tickets would you like to exchange?")
                    .AddOption("1.", 2)
                    .AddOption("5.", 6)
                    .AddOption("i have something else to ask.", 12)
                    .AddOption("I`ll think about it.", 255)
                    .AddAvatar(42).FinalizeDialog();


                        break;
                    }
                case 3:
                    {
                        data.AddText("There are many Lucky Boxes in the Lottery Center. Pay 3 Small Lottery Tickets, and you can draw from the Lucky Box! If you are not satisfied with the item you get, you can pay 1 more Small Lottery Ticket to change the item you drew. However, you can")
                            .AddText("~only pay and change the item for 2 times. Each player is allowed to draw from the Lottery up to 10 times, each day.")
                   .AddOption("Okay,~I~see.", 255)
                   .AddAvatar(42).FinalizeDialog();

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.RebirthMaster)]
        public static void RebirthMaster(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I devote all my life to the research of eternity. And finally I know the secret of rebirth! Well, anything I can help you with?")
                       .AddOption("1st~Rebirth.", 1)
                       .AddOption("2nd~Rebirth.", 4)
                       .AddOption("Reincarnation.", 5)
                              .AddOption("Reallot~my~Attribute~Points.", 8)
                             .AddOption("I want to buy Oblivion Dew.", 150)
                            // .AddOption("Give Me Level 140.", 252)
                       .AddOption("Just~passing~by.", 255)
                       .AddAvatar(15).FinalizeDialog();

                        break;
                    }
                //case 252:
                //    {
                //        data.AddText("Hello, you can raise your account to Level 140 through me for free. Do you want to raise your level now?")
                //                 .AddOption("Yes.", 251)
                //                 .AddOption("No.", 255)
                //                 .AddAvatar(15).FinalizeDialog();
                //        break;
                //    }
                //case 251:
                //    {
                //        if (client.Player.Level == 140)
                //        {
                //            data.AddText("You already achieved the highest level possible! ")
                //            .AddOption("Okay thanks.", 255).AddAvatar(15).FinalizeDialog();
                //            return;
                //        }
                //        client.UpdateLevel(stream, 140);
                //        data.AddText("Congratulations, You Have Reached Level 140 Successfully.")
                //           .AddOption("Thanks.", 255).AddAvatar(15).FinalizeDialog();
                //        break;
                //    }
                case 150:
                    {
                        data.AddText(" You can buy Oblivion Dew for only 1 CPs Want to buy now?")
                .AddOption("Buy Oblivion Dew for 1 CPs.", 160)
                .AddOption("I`ll talk to you later.", 255)
                .AddAvatar(15).FinalizeDialog();
                        break;
                    }
                case 160:
                    {
                        data.AddText(" You will purchase Oblivion Dew now It will be withdrawn 1 CPs in your inventory Are you sure you want to buy?")
                     .AddOption("Yes.", 140)
                     .AddOption("No.", 255)
                     .AddAvatar(15).FinalizeDialog();
                        break;
                    }
                case 8:
                    {
                        data.AddText("Reborn~level~70+~players~can~redistribute~their~attribute~points~at~the~cost~of~a~dragonball.")
                    .AddOption("I~will~reallot~my~points.", 9)
                    .AddOption("Let~me~think~it~over.", 255)
                    .AddAvatar(15).FinalizeDialog();


                        break;

                    }
                case 140:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                            break;
                        }
                        if (client.Player.ConquerPoints >= 1)
                        {
                            client.Player.ConquerPoints -= 1;
                            client.Inventory.AddItemWitchStack(711083, 0, 1, stream);
                            client.SendSysMesage("You received 1 Oblivion Dew in your inventory.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                        }
                        break;
                    }
                case 9:
                    {
                        if (client.Player.Level >= 70)
                        {
                            if (client.Inventory.Contain(Database.ItemType.DragonBall, 1))
                            {
                                client.Inventory.Remove(Database.ItemType.DragonBall, 1, stream);

                                client.Player.Agility = 0;
                                client.Player.Strength = 0;
                                client.Player.Vitality = 1;
                                client.Player.Spirit = 0;

                                client.CreateBoxDialog("You have successfully reloaded your attribute points.");

                                if (client.Player.Reborn == 0)
                                {
                                    client.Player.Atributes = 0;
                                    Database.DataCore.AtributeStatus.ResetStatsNonReborn(client.Player);
                                    if (Database.AtributesStatus.IsWater(client.Player.Class))
                                    {
                                        if (client.Player.Level > 110)
                                            client.Player.Atributes = (ushort)((client.Player.Level - 110) * 3 + client.Player.ExtraAtributes);
                                    }
                                    else
                                    {
                                        if (client.Player.Level > 120)
                                            client.Player.Atributes = (ushort)((client.Player.Level - 120) * 3 + client.Player.ExtraAtributes);
                                    }
                                }
                                else if (client.Player.Reborn == 1)
                                {
                                    client.Player.Atributes = (ushort)(Pool.RebornInfo.ExtraAtributePoints(client.Player.FirstRebornLevel, client.Player.FirstClass)
                                        + 52 + 3 * (client.Player.Level - 15) + client.Player.ExtraAtributes);
                                }
                                else
                                {
                                    if (client.Player.SecoundeRebornLevel == 0)
                                        client.Player.SecoundeRebornLevel = 130;
                                    client.Player.Atributes = (ushort)(Pool.RebornInfo.ExtraAtributePoints(client.Player.FirstRebornLevel, client.Player.FirstClass) +
                                        Pool.RebornInfo.ExtraAtributePoints(client.Player.SecoundeRebornLevel, client.Player.SecoundeClass) + 52 + 3 * (client.Player.Level - 15) + client.Player.ExtraAtributes);
                                }
                                client.Player.SendUpdate(stream, client.Player.Strength, Game.MsgServer.MsgUpdate.DataType.Strength);
                                client.Player.SendUpdate(stream, client.Player.Agility, Game.MsgServer.MsgUpdate.DataType.Agility);
                                client.Player.SendUpdate(stream, client.Player.Spirit, Game.MsgServer.MsgUpdate.DataType.Spirit);
                                client.Player.SendUpdate(stream, client.Player.Vitality, Game.MsgServer.MsgUpdate.DataType.Vitality);
                                client.Player.SendUpdate(stream, client.Player.Atributes, Game.MsgServer.MsgUpdate.DataType.Atributes);

                            }
                            else
                            {
                                data.AddText("Sorry, you don't have one DragonBall.")
                   .AddOption("I~see.", 255)
                   .AddAvatar(15).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You~cannot~reallot~your~ability~points~unless~you~are~reborn~and~above~level~70.")
                  .AddOption("I~see", 255)
                  .AddAvatar(15).FinalizeDialog();
                        }


                        break;
                    }
                case 5:
                    {
                        data.AddText("From now on, have total control of your life! If you've been Reborn twice and are at least Level 110, just bring me some Oblivion Dew and I'll help you become Reincarnated! After the Reincarnation, your class skills will revert to level 0,")
                                                    .AddText("~but they`ll return to their previous levels when you reach Level 110, again. If your classes have been the same for all three lives, including your current one, you can learn the Pure Skills from your class trainer,")
                                                    .AddText("~in the job center.")
                                          .AddOption("I~want~to~get~Reincarnated.", 6)
                                          .AddOption("Where~can~I~get~the~Dew?", 7)
                                          .AddAvatar(15).FinalizeDialog();


                        break;
                    }
                case 7:
                    {
                        data.AddText("Oblivion Dew is available in the Honor Store and the Horse Race Store. You can use your Honor Points or Horse Race Points to purchase one.")
                    .AddOption("Got~it!~Thanks!", 255)
                    .AddAvatar(15).FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        int limitPoints2 = client.Player.Agility + client.Player.Strength + client.Player.Vitality + client.Player.Spirit + client.Player.Atributes;

                        if (client.Player.Reborn == 2 && client.Inventory.Contain(711083, 1))
                        {
                            ActionQuery action = new ActionQuery()
                            {
                                Type = ActionType.OpenDialog,
                                ObjId = client.Player.UID,
                                dwParam = MsgServer.DialogCommands.Reincarnation,
                                PositionX = client.Player.X,
                                PositionY = client.Player.Y
                            };
                            client.Send(stream.ActionCreate(action));


                        }
                        else
                        {
                            data.AddText("You have not been Reborn twice. I`m afraid you will not withstand the power of the Oblivion Dew. Come to me when you get stronger.")
                     .AddOption("Got~it!~Thanks!", 255)
                     .AddAvatar(15).FinalizeDialog();
                        }


                        break;
                    }
                case 1:
                    {
                        if (client.Player.Reborn == 0 && (client.Player.Level >= 110 && Database.AtributesStatus.IsWater(client.Player.Class) || client.Player.Level >= 120 & client.Player.Class % 10 >= 5))
                        {
                            if (client.Inventory.Contain(721259, 1) || client.Inventory.Contain(721259, 1, 1))
                            {
                                data.AddText("There are two kinds of reborns. One is the normal one and the second one is blessed.")
                                    .AddText("The normal reborn will give you the chance to get a Super Gem and the blessed reborn will set a -1 into one of your equipment that you wear during the reborn. What do you chose?")
                             .AddOption("Normal reborn.", 2)
                             .AddOption("Blessed reborn.", 3)
                             .AddOption("I`ll~think~about~it.", 255)
                             .AddAvatar(15).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you don't have CelestialStone.")
                      .AddOption("I~see.", 255)
                      .AddAvatar(15).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You~haven`t~reached~the~highest~rank~yet.~Please~work~harder. You cannot reborn if your level is not 110+ for water saints and 120+ for other masters.")
                        .AddOption("I~see.", 255)
                        .AddAvatar(15).FinalizeDialog();
                        }

                        break;
                    }
                case 4:
                    {
                        if (client.Player.Reborn == 1 && (client.Player.Level >= 110 && Database.AtributesStatus.IsWater(client.Player.Class) || client.Player.Level >= 120 & client.Player.Class % 10 >= 5))
                        {
                            if (client.Inventory.Contain(723701, 1) || client.Inventory.Contain(723701, 1, 1))
                            {
                                data.AddText("You don't worry, it's not gonna hurt you a bit when getting the third life. You just need an exemption token and it's all going to be fine.")
                             .AddOption("Here is the ExemptionToken.", 3)
                             .AddOption("I`ll~think~about~it.", 255)
                             .AddAvatar(15).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you don't have ExemptionToken.")
                      .AddOption("I~see.", 255)
                      .AddAvatar(15).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Sorry,~you~are~ineligible~for~second~rebirth.~Please~wait~until~you~are~a~level~120~reborn~person.")
                        .AddOption("I~see.", 255)
                        .AddAvatar(15).FinalizeDialog();
                        }



                        break;
                    }
                case 2:
                    {
                        data.AddText("Select the super gem you desire.")
                      .AddOption("SuperPhoenixGem.", 203)
                      .AddOption("SuperDragonGem.", 213)
                      .AddOption("SuperFuryGem.", 223)
                      .AddOption("SuperRainbowGem.", 233)
                      .AddOption("SuperVioletGem.", 253)
                      .AddOption("SuperMoonGem.", 254)
                      .AddOption("SuperKylinGem", 243)
                      .AddOption("I`ll~think~about~it.", 255)
                      .AddAvatar(15).FinalizeDialog();


                        break;
                    }
                case 203:
                case 213:
                case 223:
                case 233:
                case 253:
                case 254:
                case 243:
                    {
                        if (client.Player.Reborn == 0)
                        {
                            client.RebornGem = (uint)(Option % 100);
                            if (client.RebornGem == 254)
                                client.RebornGem = 263;
                        }
                        data.AddText("Select the class you want to reborn in.")
                      .AddOption("Trojan.", 11)
                      .AddOption("Warrior.", 21)
                        .AddOption("DuueWanderer.", 31)
                      .AddOption("Archer.", 41)
                      .AddOption("WaterTaoist.", 132)
                      .AddOption("FireTaoist.", 142)
                      .AddOption("Ninja.", 51)
                      .AddOption("Monk", 61)
                      .AddOption("Pirate", 71)
                      .AddOption("Dragon-Warrior", 81)
                      .AddOption("Windwalker", 161)
                      .AddOption("Thunderstriker.", 91)
                      .AddOption("I`ll~think~about~it.", 255)
                      .AddAvatar(15).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        data.AddText("Select the class you want to reborn in.")
                    .AddOption("Trojan.", 11)
                    .AddOption("Warrior.", 21)
                     .AddOption("DuueWanderer.", 31)
                    .AddOption("Archer.", 41)
                    .AddOption("WaterTaoist.", 132)
                    .AddOption("FireTaoist.", 142)
                    .AddOption("Ninja.", 51)
                    .AddOption("Monk", 61)
                      .AddOption("Pirate", 71)
                       .AddOption("Dragon-Warrior", 81)
                       .AddOption("Windwalker", 161)
                         .AddOption("Thunderstriker.", 91)
                    .AddOption("I`ll~think~about~it.", 255)
                    .AddAvatar(15).FinalizeDialog();

                        break;
                    }
                case 11:
                case 21:
                case 31:
                case 41:
                case 132:
                case 142:
                case 51:
                case 161:
                case 61:
                case 81:
                case 71:
                case 91:
                    {

                        if (!client.Inventory.HaveSpace(5))
                        {
                            data.AddText("Sorry, but you need atleast 5 free spaces in your inventory.")
            .AddOption("I~see.", 255)
            .AddAvatar(15).FinalizeDialog();
                            break;
                        }

                        if (client.Player.Class / 1000 != Option / 10 && client.Rune.WornObjects.Count(i => Role.Instance.Rune.GetSkillForRune(i) != null) > 0)
                        {
                            client.SendSysMesage("Please unequip red/blue runes before changing your class.", MsgMessage.ChatMode.SystemWhisper);
                            client.CreateBoxDialog("Please unequip red/blue/Yellow runes before changing your class.");
                            break;
                        }

                        if (client.Player.Reborn == 0 && (client.Player.Level >= 110 && Database.AtributesStatus.IsWater(client.Player.Class) || client.Player.Level >= 120 & client.Player.Class % 10 >= 5))
                        {
                            if (client.Inventory.Contain(721259, 1) || client.Inventory.Contain(721259, 1, 1))
                            {
                                client.Inventory.Remove(721259, 1, stream);
                                if (client.RebornGem != 0)
                                {
                                    client.Inventory.Add(stream, (uint)(700000 + client.RebornGem), 1);
                                    client.RebornGem = 0;

                                }
                                else
                                {
                                    foreach (var item in client.Equipment.CurentEquip)
                                    {
                                        if (item.Position == (ushort)Role.Flags.ConquerItem.Steed || item.Position == (ushort)Role.Flags.ConquerItem.Relics || item.Position == (ushort)Role.Flags.ConquerItem.Wing)
                                        {
                                            continue;

                                        }
                                        if (item.Bless == 0)
                                        {
                                            item.Bless = 1;
                                            item.Mode = Role.Flags.ItemMode.Update;
                                            item.Send(client, stream);
                                            break;
                                        }

                                        /*if (item.Position != (ushort)Role.Flags.ConquerItem.Steed || 
                                           item.Position != (ushort)Role.Flags.ConquerItem.Relics || item.Position != (ushort)Role.Flags.ConquerItem.Wing)
                                        {
                                            if (item.Bless == 0)
                                            {
                                                item.Bless = 1;
                                                item.Mode = Role.Flags.ItemMode.Update;
                                                item.Send(client, stream);
                                                break;
                                            }
                                        }*/
                                        //enta msh labes 8er elrelic s7 ? aywa swany 
                                    }
                                }
                                switch (Option)
                                {
                                    case 161:
                                    case 11:
                                    case 21:
                                    case 51:
                                    case 61:
                                    case 71:
                                    case 81:
                                    case 91:
                                        {
                                            client.Inventory.Add(stream, 410077, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false, Role.Flags.ItemEffect.Poison);
                                            break;
                                        }
                                    case 41:
                                        {
                                            client.Inventory.Add(stream, 410077, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false, Role.Flags.ItemEffect.Poison);
                                            break;
                                        }
                                    case 132:
                                    case 142:
                                        {
                                            client.Inventory.Add(stream, 421077, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false, Role.Flags.ItemEffect.MP);
                                            break;
                                        }
                                }

                                uint RebornClass = (uint)(((Option / 10) * 1000) + (Option % 10));
                                Pool.RebornInfo.Reborn(client.Player, RebornClass, stream);
                            }
                            else
                            {
                                data.AddText("Sorry, you don't have CelatialStone.")
                      .AddOption("I~see.", 255)
                      .AddAvatar(15).FinalizeDialog();
                            }
                        }
                        else if (client.Player.Reborn == 1 && (client.Player.Level >= 110 && client.Player.Class >= 13005 && client.Player.Class <= 13049 || client.Player.Level >= 120 & client.Player.Class % 10 >= 5))
                        {
                            if (client.Inventory.Contain(723701, 1) || client.Inventory.Contain(723701, 1, 1))
                            {
                                client.Inventory.Remove(723701, 1, stream);
                                uint RebornClass = (uint)(((Option / 10) * 1000) + (Option % 10));
                                Pool.RebornInfo.Reborn(client.Player, RebornClass, stream);
                            }
                            else
                            {
                                data.AddText("Sorry, you don't have ExemptionToken.")
                      .AddOption("I~see.", 255)
                      .AddAvatar(15).FinalizeDialog();
                            }
                        }


                        break;
                    }
            }
        }
    }
}

