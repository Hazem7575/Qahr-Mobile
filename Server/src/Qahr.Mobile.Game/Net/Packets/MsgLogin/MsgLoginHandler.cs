using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgNpc;
using System.Xml.Linq;
using System.IO;
using Qahr.Mobile.Game.MsgTournaments;

namespace Qahr.Mobile.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet LoginHandlerCreate(this ServerSockets.Packet stream, uint Type, uint Map)
        {
            stream.InitWriter();

            stream.Write(0);
            stream.Write(Type);
            stream.Write(Map);

            stream.Finalize(GamePackets.MapLoading);

            return stream;
        }

    }
    public unsafe struct MsgLoginHandler
    {
        private static void SendShopUpdateCompatibilityPackets(Client.GameClient client, ServerSockets.Packet packet)
        {
            if (client == null || client.Player == null || packet == null)
                return;

            // 0E 00 5F 09 08 00 18 00 20 11 28 00 32 00
            packet.InitWriter();
            packet.Write((byte)0x08);
            packet.Write((byte)0x00);
            packet.Write((byte)0x18);
            packet.Write((byte)0x00);
            packet.Write((byte)0x20);
            packet.Write((byte)0x11);
            packet.Write((byte)0x28);
            packet.Write((byte)0x00);
            packet.Write((byte)0x32);
            packet.Write((byte)0x00);
            packet.Finalize(GamePackets.MsgAuctionItem);
            client.Send(packet);

            // 0A 00 5F 09 08 00 10 00 18 00
            packet.InitWriter();
            packet.Write((byte)0x08);
            packet.Write((byte)0x00);
            packet.Write((byte)0x10);
            packet.Write((byte)0x00);
            packet.Write((byte)0x18);
            packet.Write((byte)0x00);
            packet.Finalize(GamePackets.MsgAuctionItem);
            client.Send(packet);

            // 1C 00 89 09 followed by 24 zero bytes.
            packet.InitWriter();
            packet.ZeroFill(24);
            packet.Finalize(GamePackets.MsgAuctionQuery);
            client.Send(packet);

            // 18 00 EC 07 followed by 20 zero bytes.
            packet.InitWriter();
            packet.ZeroFill(20);
            packet.Finalize(GamePackets.MsgAuction);
            client.Send(packet);
        }
        [PacketAttribute(GamePackets.MapLoading)]
        public unsafe static void LoadMap(Client.GameClient client, ServerSockets.Packet packet)
        {
            if ((client.ClientFlag & Client.ServerFlag.AcceptLogin) == Client.ServerFlag.AcceptLogin)
            {
                try
                {//
                    client.Player.ServerID = (ushort)Database.GroupServerList.MyServerInfo.ID;
                    client.Send(packet.ServerVerCreate());
                    client.Send(packet.ServerTimerCreate());
                    client.Send(packet.ServerConfig());
                    client.Send(packet.LoaderHeroInfo(client.Player));

                    client.Collection.Load();
                    client.Send(packet.ClientInfo(client.Player));
                    SendShopUpdateCompatibilityPackets(client, packet);
                    //MsgTwistedFututr.Loading(client);
                    /*90 00 1D 08 08 00 2A 0C 08 00 10 02 18 01 20 00 28 00 28 01 2A 08 08 01 10 67 18 01 20 00 2A 08 08 02 10 01 18 01 20 00 2A 08 08 03 10 01 18 01 20 00 2A 08 08 04 10 01 18 00 20 00 2A 08 08 04 10 03 18 01 20 00 2A 08 08 05 10 01 18 01 20 00 2A 08 08 06 10 01 18 01 20 00 2A 08 08 07 10 01 18 00 20 00 2A 08 08 07 10 02 18 01 20 00 2A 08 08 08 10 01 18 00 20 00 2A 08 08 08 10 02 18 01 20 00 2A 08 08 09 10 01 18 01 20 00 30 C4 94 3E*/
                    //client.HairfaceStorage.Loading();

                    foreach (var chipower in client.Player.MyChi)
                        client.Player.MyChi.SendQueryUpdate(client, chipower, packet);
                    MsgChiInfo.MsgHandleChi.SendInfo(client, MsgChiInfo.Action.Upgrade, client);

                    if (client.Player.SecurityPassword != 0)
                    {
                        client.Send(packet.SecondaryPasswordCreate(MsgSecondaryPassword.ActionID.PasswordCorrect, 1, 0));
                    }
                    else
                        client.Player.IsCheckedPass = true;


                    if (client.Player.Achievement != null)
                        client.Player.Achievement.Send(client, packet);
                    if (client.HundredWeapons.Unlocked && client.HundredWeapons.Valid())
                    {
                        client.Send(packet.CreateHundredWeaponsInfo(client, MsgHundredWeaponsInfo.ActionID.RequestInfo));
                        client.Send(packet.ActionCreate(new ActionQuery()
                        {
                            Type = ActionType.OpenDialog,
                            ObjId = client.Player.UID,
                            Timestamp = (uint)Time32.timeGetTime().GetHashCode(),
                            dwParam = DialogCommands.TrojanWeaponArchive,
                            PositionX = client.Player.X,
                            PositionY = client.Player.Y
                        }));

                    }

                    client.Send(packet.NobilityIconCreate(client.Player.Nobility));

                    client.MyNinja.Loading();

                    client.Inventory.ShowALL(packet);
                    MsgYuanshen.RemvedGUI(client);
                    client.Equipment.Show(packet);

                    client.Rune.Show(packet);

                    try
                    {
                        MsgXuanBaoOpt.Loading(client, packet);
                        client.MainRelic.Apply(client, packet);
                    }
                    catch (Exception ex)
                    {
                        MyConsole.WriteLine(
                            "[LOGIN-RELIC ERROR] " + ex,
                            ConsoleColor.Red
                        );
                    }


                    //client.Send(packet.CreategRelicFuse(new MsgRelicFuse.RelicFuse()));

                    client.Player.Send(packet.AutoHuntCreate(0, 341));

                    client.Activeness.CheckTasks(packet);
                    //MsgTwistedFututr.Loading(client);


                    client.MedalStorage.Loading(); // Open All Medals For Free
                    client.Activeness.IncreaseTask(1);
                    client.Activeness.IncreaseTask(13);
                    client.Activeness.IncreaseTask(25);


                    #region New System Chat Bubble

                    var mohe167 = new MsgMsgScreenChatSkinOpt()
                    {
                        Action = (MsgMsgScreenChatSkinOpt.ActionID)1,
                        TotNum = 10000,
                    };
                    client.Send(packet.CreateMsgScreenChatSkinOpt(mohe167));
                    mohe167 = new MsgMsgScreenChatSkinOpt()
                    {
                        Action = (MsgMsgScreenChatSkinOpt.ActionID)1,
                        TotNum = 10002,
                    };
                    client.Send(packet.CreateMsgScreenChatSkinOpt(mohe167));
                    mohe167 = new MsgMsgScreenChatSkinOpt()
                    {
                        Action = (MsgMsgScreenChatSkinOpt.ActionID)1,
                        TotNum = 10003,
                    };
                    client.Send(packet.CreateMsgScreenChatSkinOpt(mohe167));

                    mohe167 = new MsgMsgScreenChatSkinOpt()
                    {
                        Action = (MsgMsgScreenChatSkinOpt.ActionID)1,
                        TotNum = 10005,
                    };
                    client.Send(packet.CreateMsgScreenChatSkinOpt(mohe167));
                    mohe167 = new MsgMsgScreenChatSkinOpt()
                    {
                        Action = (MsgMsgScreenChatSkinOpt.ActionID)1,
                        TotNum = 10006,
                    };
                    client.Send(packet.CreateMsgScreenChatSkinOpt(mohe167));
                    mohe167 = new MsgMsgScreenChatSkinOpt()
                    {
                        Action = (MsgMsgScreenChatSkinOpt.ActionID)1,
                        TotNum = 10007,
                    };
                    client.Send(packet.CreateMsgScreenChatSkinOpt(mohe167));
                    mohe167 = new MsgMsgScreenChatSkinOpt()
                    {
                        Action = (MsgMsgScreenChatSkinOpt.ActionID)1,
                        TotNum = 10008,
                    };
                    client.Send(packet.CreateMsgScreenChatSkinOpt(mohe167));
                    mohe167 = new MsgMsgScreenChatSkinOpt()
                    {
                        Action = (MsgMsgScreenChatSkinOpt.ActionID)1,
                        TotNum = 10009,
                    };
                    client.Send(packet.CreateMsgScreenChatSkinOpt(mohe167));
                    mohe167 = new MsgMsgScreenChatSkinOpt()
                    {
                        Action = (MsgMsgScreenChatSkinOpt.ActionID)1,
                        TotNum = 10010,
                    };
                    client.Send(packet.CreateMsgScreenChatSkinOpt(mohe167));
                    mohe167 = new MsgMsgScreenChatSkinOpt()
                    {
                        Action = (MsgMsgScreenChatSkinOpt.ActionID)1,
                        TotNum = 10001,
                    };
                    client.Send(packet.CreateMsgScreenChatSkinOpt(mohe167));

                    var mohe168 = new MsgFamePacket.MsgFameHall()
                    {
                        Action = (uint)4,
                        FamePoints = client.Player.TableBetDice,
                        Unk5 = 3,

                    };
                    client.Send(packet.ActionCreate(mohe168));
                    mohe167 = new MsgMsgScreenChatSkinOpt()
                    {
                        Action = (MsgMsgScreenChatSkinOpt.ActionID)1,
                        TotNum = client.Player.ChatSkinID,
                    };
                    client.Send(packet.CreateMsgScreenChatSkinOpt(mohe167));
                    #endregion

                    if (client.Player.VipLevel > 1)
                    {
                        client.Activeness.IncreaseTask(2);
                        client.Activeness.IncreaseTask(14);
                        client.Activeness.IncreaseTask(26);
                    }
                    client.Activeness.UpdateActivityPoints(packet);
                    client.Activeness.UpdateClaimRewards(packet);

                    if (client.Player.BlessTime > 0)
                        client.Player.SendUpdate(packet, client.Player.BlessTime, MsgUpdate.DataType.LuckyTimeTimer);

                    client.Send(packet.FlowerCreate(Role.Core.IsBoy(client.Player.Body) ? MsgFlower.FlowerAction.Flower : MsgFlower.FlowerAction.FlowerSender
                        , 0, 0, client.Player.Flowers.RedRoses, client.Player.Flowers.RedRoses.Amount2day
                        , client.Player.Flowers.Lilies, client.Player.Flowers.Lilies.Amount2day
                        , client.Player.Flowers.Orchids, client.Player.Flowers.Orchids.Amount2day
                        , client.Player.Flowers.Tulips, client.Player.Flowers.Tulips.Amount2day));

                    if (Role.Core.IsBoy(client.Player.Body) && client.Player.FairyForm >= 4)
                        client.Player.SendUpdate(packet, 0, 0, client.Player.TodayFlowerType, client.Player.FairyForm, MsgUpdate.DataType.FlowerRank, true);
                    else
                        client.Player.SendUpdate(packet, 0, 0, client.Player.TodayFlowerType, client.Player.FairyForm, MsgUpdate.DataType.FlowerRank, true);
                    if (client.Player.Flowers.FreeFlowers > 0)
                    {
                        client.Send(packet.FlowerCreate(Role.Core.IsBoy(client.Player.Body)
                            ? MsgFlower.FlowerAction.FlowerSender : MsgFlower.FlowerAction.Flower
                            , 0, 0, client.Player.Flowers.FreeFlowers));
                    }


                    client.Player.CreateHeavenBlessPacket(packet, true);

                    if (client.Player.HitPoints < 1)
                        client.Player.HitPoints = 1;

                    client.Player.ProtectAttack(5 * 1000); // 5 Seconds


                    if (MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.QuizShow
                        && MsgTournaments.MsgSchedules.CurrentTournament.Process == MsgTournaments.ProcesType.Alive)
                        MsgTournaments.MsgSchedules.CurrentTournament.Join(client, packet);


                    if (client.Player.DExpTime > 0)
                        client.Player.CreateExtraExpPacket(packet);


                    if (client.Player.MyClan != null)
                    {
                        client.Player.MyClan.SendThat(packet, client);

                        foreach (var ally in client.Player.MyClan.Ally.Values)
                            client.Send(packet.ClanRelationCreate(ally.ID, ally.Name, ally.LeaderName, MsgClan.Info.AddAlly));
                        foreach (var enemy in client.Player.MyClan.Enemy.Values)
                            client.Send(packet.ClanRelationCreate(enemy.ID, enemy.Name, enemy.LeaderName, MsgClan.Info.AddEnemy));
                    }

                    var AllEmoji = Database.HairfaceStorageTable.Hairfaces.Where(i => i.Type == MsgHairfaceStorage.Type.Emoji).ToArray();
                    foreach (var AddAllEmoji in AllEmoji)
                    {
                        if (AddAllEmoji == null) continue;
                        if (client.HairfaceStorage.Exists(AddAllEmoji.ID, AddAllEmoji.Type)) continue;
                        client.HairfaceStorage.Add(AddAllEmoji, true);
                    }
                    var AllFrame = Database.HairfaceStorageTable.Hairfaces.Where(i => i.Type == MsgHairfaceStorage.Type.Frame).ToArray();
                    foreach (var AddAllFrame in AllFrame)
                    {
                        if (AddAllFrame == null) continue;
                        if (client.HairfaceStorage.Exists(AddAllFrame.ID, AddAllFrame.Type)) continue;
                        client.HairfaceStorage.Add(AddAllFrame, true);
                    }

                    var TexasStyle = client.HairfaceStorage.Objects.Where(i => i.Type >= MsgHairfaceStorage.Type.Hairstyle && i.Type <= MsgHairfaceStorage.Type.Emoji).ToArray();

                    if (TexasStyle.Count() > 0)
                    {
                        var lists = new GenericSplit().Lists<Database.HairfaceStorageTable.Hairface>(40, TexasStyle.ToArray());
                        MsgHairfaceStorage.MsgHairfaceStorageProto Info = new MsgHairfaceStorage.MsgHairfaceStorageProto();
                        Info.Items = new MsgHairfaceStorage.Item[TexasStyle.Count()];
                        Info.Type = MsgHairfaceStorage.Actions.Login;
                        for (int i = 0; i < Info.Items.Length; i++)
                        {
                            Info.Items[i] = new MsgHairfaceStorage.Item();
                            Info.Items[i].ID = TexasStyle[i].ID;
                            Info.Items[i].Type = TexasStyle[i].Type;
                            Info.Items[i].Equipped = TexasStyle[i].Equiped;
                            if (Info.Items[i].Type == MsgHairfaceStorage.Type.Hairstyle)
                            {
                                Info.Items[i].Colors = new long[1];
                                Info.Items[i].Colors[0] = 1;

                            }
                        }
                        Info.UID = client.Player.UID;
                        client.Send(packet.CreateHairface(Info));
                    }

                    if (client.Player.MyJiangHu != null)
                        client.Player.MyJiangHu.LoginClient(packet, client);
                    else if (client.Player.Reborn == 2)
                    {
                        client.Send(packet.JiangHuInfoCreate(MsgJiangHuInfo.JiangMode.IconBar, "0"));
                    }

                    foreach (var item in client.Confiscator.RedeemContainer.Values)
                    {
                        var Dataitem = item;
                        Dataitem.DaysLeft = (uint)(TimeSpan.FromTicks(DateTime.Now.Ticks).Days - TimeSpan.FromTicks(Role.Instance.Confiscator.GetTimer(item.Date).Ticks).Days);
                        if (Dataitem.DaysLeft > 7)
                        {
                            Dataitem.Action = MsgDetainedItem.ContainerType.RewardCps;
                        }
                        if (Dataitem.Action != MsgDetainedItem.ContainerType.RewardCps)
                        {
                            Dataitem.Action = MsgDetainedItem.ContainerType.DetainPage;
                            Dataitem.Send(client, packet);
                        }
                        if (Dataitem.Action == MsgDetainedItem.ContainerType.RewardCps)
                            client.Confiscator.RedeemContainer.TryRemove(item.UID, out Dataitem);
                    }
                    foreach (var item in client.Confiscator.ClaimContainer.Values)
                    {
                        var Dataitem = item;
                        Dataitem.DaysLeft = (uint)(TimeSpan.FromTicks(DateTime.Now.Ticks).Days - TimeSpan.FromTicks(Role.Instance.Confiscator.GetTimer(item.Date).Ticks).Days);
                        if (Dataitem.RewardConquerPoints != 0)
                        {
                            Dataitem.Action = MsgDetainedItem.ContainerType.RewardCps;
                        }
                        Dataitem.Send(client, packet);
                        client.Confiscator.ClaimContainer[item.UID] = Dataitem;
                    }

                    if (Program.ServerConfig.ConquerorWinner == client.Player.UID)
                        client.Player.AddFlag(Role.Core.IsBoy(client.Player.Body) ? MsgUpdate.Flags.TopMrConquer : MsgUpdate.Flags.TopMrsConquer, Role.StatusFlagsBigVector32.PermanentFlag, false);

                    if (MsgTournaments.MsgSchedules.GuildWar.RewardDeputiLeader.Contains(client.Player.UID))
                        client.Player.AddFlag(MsgUpdate.Flags.TopDeputyLeader, Role.StatusFlagsBigVector32.PermanentFlag, false);

                    if (MsgTournaments.MsgSchedules.GuildWar.RewardLeader.Contains(client.Player.UID))
                        client.Player.AddFlag(MsgUpdate.Flags.TopGuildLeader, Role.StatusFlagsBigVector32.PermanentFlag, false);

                    if (MsgTournaments.MsgSchedules.SuperGuildWar.RewardDeputiLeader.Contains(client.Player.UID))
                        client.Player.AddFlag(MsgUpdate.Flags.TopSuperGuildWarThreeStars, Role.StatusFlagsBigVector32.PermanentFlag, false);

                    if (MsgTournaments.MsgSchedules.SuperGuildWar.RewardLeader.Contains(client.Player.UID))
                    {
                        client.Player.AddFlag(MsgUpdate.Flags.TopSuperGuildWarFiveStars, Role.StatusFlagsBigVector32.PermanentFlag, false);
                        client.Player.AddSpecialWings(Game.MsgServer.MsgTitleStorage.WingsType.WingsofSolarDra, packet);
                    }

                    if (client.Player.WinnerBest == 1)
                        client.Player.AddFlag(MsgUpdate.Flags.rygh_hglx1, Role.StatusFlagsBigVector32.PermanentFlag, false);

                    if (client.Player.CursedTimer > 0)
                    {
                        client.Player.AddCursed(client.Player.CursedTimer);
                    }

                    MsgTournaments.MsgSchedules.ClassPkWar.LoginClient(client);
                    MsgTournaments.MsgSchedules.ElitePkTournament.GetTitle(client, packet);
                    if (MsgTournaments.MsgBroadcast.CurrentBroadcast.EntityID != 1)
                    {
                        client.Send(new MsgServer.MsgMessage(MsgTournaments.MsgBroadcast.CurrentBroadcast.Message
                            , "ALLUSERS"
                            , MsgTournaments.MsgBroadcast.CurrentBroadcast.EntityName
                            , MsgServer.MsgMessage.MsgColor.red
                            , MsgServer.MsgMessage.ChatMode.BroadcastMessage
                            ).GetArray(packet));
                    }


                    if (client.Player.RacePoints > 0)
                        client.Player.SendUpdate(packet, client.Player.RacePoints, MsgUpdate.DataType.RaceShopPoints);

                    client.Player.InnerPower.AddPotency(packet, client, 0);
                    client.Player.UpdateVip(packet);
                    global::Qahr.Mobile.LuaUi.LuaPush.OnLogin(client);// [LuaUi] top-bar Fgui buttons + running festival icons
                    global::Qahr.Mobile.BattlePass.BattlePassManager.OnLogin(client);// [BattlePass] Martial Token icon (2064 type 0 = season) + login tasks
                    global::Qahr.Mobile.Poker.PokerModule.OnLogin(client);// [Poker 2026-10-14] pay back poker chips left open by a crash/restart
                    global::Qahr.Mobile.AuctionHouse.AuctionModule.OnLogin(client);// [Auction 2026-10-14] load store, settle this player's escrow, expiry timer

                    client.Player.SendUpdate(packet, 255, MsgUpdate.DataType.Merchant);
                    Database.TitleStorage.CheckUpUser(client, packet);
                    foreach (var item in client.Warehouse.FuseRelics)
                    {
                        item.Mode = Role.Flags.ItemMode.AddItem;
                        item.Send(client, packet);
                    }
                    foreach (var title in client.Player.TitleWithTime.Values)
                    {
                        client.Player.AddTimeTitle((uint)title.titleID, packet);
                    }
                    foreach (var title in client.Player.SpecialTitles)
                    {
                        client.Player.AddSpecialTitle(title, packet);

                    }
                    foreach (var titles in client.Player.WingsTitles)
                    {
                        client.Player.AddSpecialWings(titles, packet);

                    }
                    foreach (var titles in client.Player.HaloTitles)
                    {
                        client.Player.AddSpecialHalo(titles, packet);

                    }
                    client.Player.SendSpecialTitle(packet);
                    client.Player.SendSpecialTitleTime(packet);
                    client.Player.ActiveSpecialTitles(packet);


                    //MsgTournaments.MsgSchedules.PkWar.AddTop(client);


                   // client.SendSysMesage("Enjoy " + Program.ServerConfig.ServerName + ".", MsgMessage.ChatMode.Talk);
                    client.SendSysMesage("Welcome to " + Program.ServerConfig.ServerName + "!", MsgMessage.ChatMode.Talk);

                    //client.Send(packet.StaticGUI(MsgNpc.MsgBuilder.StaticGUIType.Header, "  Our Version 7829 Arabic and English translation system:"));
                    //client.Send(packet.StaticGUI(MsgNpc.MsgBuilder.StaticGUIType.Body, "Welcome to " + Program.ServerConfig.ServerName + "."));
                    //client.Send(packet.StaticGUI(MsgNpc.MsgBuilder.StaticGUIType.Body, "- New Monk Archive"));
                    //client.Send(packet.StaticGUI(MsgNpc.MsgBuilder.StaticGUIType.Body, "- System~New~Eonspir"));
                    //client.Send(packet.StaticGUI(MsgNpc.MsgBuilder.StaticGUIType.Body, "- System~New~Archive~Trojan"));
                    //client.Send(packet.StaticGUI(MsgNpc.MsgBuilder.StaticGUIType.Body, "- Cross server"));
                    //client.Send(packet.StaticGUI(MsgNpc.MsgBuilder.StaticGUIType.Body, "- Cross Wars"));
                    //client.Send(packet.StaticGUI(MsgNpc.MsgBuilder.StaticGUIType.Body, "- System~New~Collection"));
                    //client.Send(packet.StaticGUI(MsgNpc.MsgBuilder.StaticGUIType.Body, "- Cross Arena"));
                    //client.Send(packet.StaticGUI(MsgNpc.MsgBuilder.StaticGUIType.Body, "- New Pirate Archive"));
                    //client.Send(packet.StaticGUI(MsgNpc.MsgBuilder.StaticGUIType.Body, "- New streaming system"));
                    //client.Send(packet.StaticGUI(MsgNpc.MsgBuilder.StaticGUIType.Body, "- New~Runa"));
                    //client.Send(packet.StaticGUI(MsgNpc.MsgBuilder.StaticGUIType.Body, "- New~Warrior"));
                    //client.Send(packet.StaticGUI(MsgNpc.MsgBuilder.StaticGUIType.Body, "- ThunderStrike"));
                    //client.Send(packet.StaticGUI(MsgNpc.MsgBuilder.StaticGUIType.Body, "- System~Archive~Trojan"));
                    //client.Send(packet.StaticGUI(MsgNpc.MsgBuilder.StaticGUIType.Body, "- System~Awkeend~Ninja"));
                    //client.Send(packet.StaticGUI(MsgNpc.MsgBuilder.StaticGUIType.Body, "- System~Almighty~Warrior"));
                    //client.Send(packet.StaticGUI(MsgNpc.MsgBuilder.StaticGUIType.Body, "- Soaring Archers"));
                    //client.Send(packet.StaticGUI(MsgNpc.MsgBuilder.StaticGUIType.Body, "- System~Soaring~Tao"));
                    //client.Send(packet.StaticGUI(MsgNpc.MsgBuilder.StaticGUIType.Body, "- New~Poker"));
                    //client.Send(packet.StaticGUI(MsgNpc.MsgBuilder.StaticGUIType.Body, "-New Clan"));
                    //client.Send(packet.StaticGUI(MsgNpc.MsgBuilder.StaticGUIType.Footer, "Thank you. " + Program.ServerConfig.Program + ""));

                    client.Player.UpdateInventorySash(packet);
                    if (client.Player.ExpProtection > 0)
                        client.Player.CreateExpProtection(packet, 0, false);


                    if (client.Player.VipLevel == 7)
                    {
                        TimeSpan timer1 = new TimeSpan(client.Player.ExpireVip.Ticks);
                        TimeSpan Now2 = new TimeSpan(DateTime.Now.Ticks);
                        int days_left = (int)(timer1.TotalDays - Now2.TotalDays);
                        int hour_left = (int)(timer1.TotalHours - Now2.TotalHours);
                        int left_minutes = (int)(timer1.TotalMinutes - Now2.TotalMinutes);

                        if (days_left > 0)
                            client.SendSysMesage("Your VIP " + client.Player.VipLevel + " will expire in : " + days_left + " days.", MsgMessage.ChatMode.System);
                        else if (hour_left > 0)
                            client.SendSysMesage("Your VIP " + client.Player.VipLevel + " will expire in : " + hour_left + " hours.", MsgMessage.ChatMode.System);
                        else if (left_minutes > 0)
                            client.SendSysMesage("Your VIP " + client.Player.VipLevel + " will expire in : " + left_minutes + " minutes.", MsgMessage.ChatMode.System);
                    }
                    else if (client.Player.VipLevel == 6)
                    {
                        TimeSpan timer1 = new TimeSpan(client.Player.ExpireVip.Ticks);
                        TimeSpan Now2 = new TimeSpan(DateTime.Now.Ticks);
                        int days_left = (int)(timer1.TotalDays - Now2.TotalDays);
                        int hour_left = (int)(timer1.TotalHours - Now2.TotalHours);
                        int left_minutes = (int)(timer1.TotalMinutes - Now2.TotalMinutes);

                        if (days_left > 0)
                            client.SendSysMesage("Your VIP " + client.Player.VipLevel + " will expire in : " + days_left + " days.", MsgMessage.ChatMode.System);
                        else if (hour_left > 0)
                            client.SendSysMesage("Your VIP " + client.Player.VipLevel + " will expire in : " + hour_left + " hours.", MsgMessage.ChatMode.System);
                        else if (left_minutes > 0)
                            client.SendSysMesage("Your VIP " + client.Player.VipLevel + " will expire in : " + left_minutes + " minutes.", MsgMessage.ChatMode.System);
                    }
                    else if (client.Player.VipLevel == 5)
                    {
                        TimeSpan timer1 = new TimeSpan(client.Player.ExpireVip.Ticks);
                        TimeSpan Now2 = new TimeSpan(DateTime.Now.Ticks);
                        int days_left = (int)(timer1.TotalDays - Now2.TotalDays);
                        int hour_left = (int)(timer1.TotalHours - Now2.TotalHours);
                        int left_minutes = (int)(timer1.TotalMinutes - Now2.TotalMinutes);

                        if (days_left > 0)
                            client.SendSysMesage("Your VIP " + client.Player.VipLevel + " will expire in : " + days_left + " days.", MsgMessage.ChatMode.System);
                        else if (hour_left > 0)
                            client.SendSysMesage("Your VIP " + client.Player.VipLevel + " will expire in : " + hour_left + " hours.", MsgMessage.ChatMode.System);
                        else if (left_minutes > 0)
                            client.SendSysMesage("Your VIP " + client.Player.VipLevel + " will expire in : " + left_minutes + " minutes.", MsgMessage.ChatMode.System);
                    }
                    else if (client.Player.VipLevel == 4)
                    {
                        TimeSpan timer1 = new TimeSpan(client.Player.ExpireVip.Ticks);
                        TimeSpan Now2 = new TimeSpan(DateTime.Now.Ticks);
                        int days_left = (int)(timer1.TotalDays - Now2.TotalDays);
                        int hour_left = (int)(timer1.TotalHours - Now2.TotalHours);
                        int left_minutes = (int)(timer1.TotalMinutes - Now2.TotalMinutes);

                        if (days_left > 0)
                            client.SendSysMesage("Your VIP " + client.Player.VipLevel + " will expire in : " + days_left + " days.", MsgMessage.ChatMode.System);
                        else if (hour_left > 0)
                            client.SendSysMesage("Your VIP " + client.Player.VipLevel + " will expire in : " + hour_left + " hours.", MsgMessage.ChatMode.System);
                        else if (left_minutes > 0)
                            client.SendSysMesage("Your VIP " + client.Player.VipLevel + " will expire in : " + left_minutes + " minutes.", MsgMessage.ChatMode.System);
                    }
                    else if (client.Player.VipLevel == 3)
                    {
                        TimeSpan timer1 = new TimeSpan(client.Player.ExpireVip.Ticks);
                        TimeSpan Now2 = new TimeSpan(DateTime.Now.Ticks);
                        int days_left = (int)(timer1.TotalDays - Now2.TotalDays);
                        int hour_left = (int)(timer1.TotalHours - Now2.TotalHours);
                        int left_minutes = (int)(timer1.TotalMinutes - Now2.TotalMinutes);

                        if (days_left > 0)
                            client.SendSysMesage("Your VIP " + client.Player.VipLevel + " will expire in : " + days_left + " days.", MsgMessage.ChatMode.System);
                        else if (hour_left > 0)
                            client.SendSysMesage("Your VIP " + client.Player.VipLevel + " will expire in : " + hour_left + " hours.", MsgMessage.ChatMode.System);
                        else if (left_minutes > 0)
                            client.SendSysMesage("Your VIP " + client.Player.VipLevel + " will expire in : " + left_minutes + " minutes.", MsgMessage.ChatMode.System);
                    }
                    else if (client.Player.VipLevel == 2)
                    {
                        TimeSpan timer1 = new TimeSpan(client.Player.ExpireVip.Ticks);
                        TimeSpan Now2 = new TimeSpan(DateTime.Now.Ticks);
                        int days_left = (int)(timer1.TotalDays - Now2.TotalDays);
                        int hour_left = (int)(timer1.TotalHours - Now2.TotalHours);
                        int left_minutes = (int)(timer1.TotalMinutes - Now2.TotalMinutes);

                        if (days_left > 0)
                            client.SendSysMesage("Your VIP " + client.Player.VipLevel + " will expire in : " + days_left + " days.", MsgMessage.ChatMode.System);
                        else if (hour_left > 0)
                            client.SendSysMesage("Your VIP " + client.Player.VipLevel + " will expire in : " + hour_left + " hours.", MsgMessage.ChatMode.System);
                        else if (left_minutes > 0)
                            client.SendSysMesage("Your VIP " + client.Player.VipLevel + " will expire in : " + left_minutes + " minutes.", MsgMessage.ChatMode.System);
                    }
                    {
                        TimeSpan timer1 = new TimeSpan(client.Player.ExpireVip.Ticks);
                        TimeSpan Now2 = new TimeSpan(DateTime.Now.Ticks);
                        int days_left = (int)(timer1.TotalDays - Now2.TotalDays);
                        int hour_left = (int)(timer1.TotalHours - Now2.TotalHours);
                        int left_minutes = (int)(timer1.TotalMinutes - Now2.TotalMinutes);

                        if (days_left > 0)
                            client.SendSysMesage("Your VIP " + client.Player.VipLevel + " will expire in : " + days_left + " days.", MsgMessage.ChatMode.System);
                        else if (hour_left > 0)
                            client.SendSysMesage("Your VIP " + client.Player.VipLevel + " will expire in : " + hour_left + " hours.", MsgMessage.ChatMode.System);
                        else if (left_minutes > 0)
                            client.SendSysMesage("Your VIP " + client.Player.VipLevel + " will expire in : " + left_minutes + " minutes.", MsgMessage.ChatMode.System);
                    }
                    client.HundredWeapons.LoadPowerFoucs(client);
                    if (Database.AtributesStatus.IsTrojan(client.Player.Class)
                        || Database.AtributesStatus.IsTrojan(client.Player.FirstClass)
                        || Database.AtributesStatus.IsTrojan(client.Player.SecoundeClass))
                    {
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Cyclone))
                            client.MySpells.Add(packet, (ushort)Role.Flags.SpellID.Cyclone);
                    }
                    if (Database.AtributesStatus.IsThunderStriker(client.Player.Class)
                        || Database.AtributesStatus.IsThunderStriker(client.Player.FirstClass)
                        || Database.AtributesStatus.IsThunderStriker(client.Player.SecoundeClass))
                    {
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SparkShield))
                            client.MySpells.Add(packet, (ushort)Role.Flags.SpellID.SparkShield);
                    }


                    if (client.Inventory.HaveSpace(1))
                    {
                        foreach (var item in client.Equipment.ClientItems.Values)
                        {
                            if (item.Position >= (uint)Role.Flags.ConquerItem.Head && item.Position <= (uint)Role.Flags.ConquerItem.Wing)
                            {
                                if (client.Inventory.HaveSpace(1) && item.Position == (uint)Role.Flags.ConquerItem.RightWeapon
                                    && item.Position == (uint)Role.Flags.ConquerItem.LeftWeapon)
                                {
                                    if (!Database.ItemType.IsShield(item.ITEM_ID))
                                    {
                                        if (!Database.ItemType.Equipable(item.ITEM_ID, client))
                                        {
                                            client.Equipment.Remove((Role.Flags.ConquerItem)item.Position, packet);
                                        }
                                    }
                                }
                            }
                            else if (item.Position >= (uint)Role.Flags.ConquerItem.AleternanteHead && item.Position <= (uint)Role.Flags.ConquerItem.AleternanteGarment)
                            {
                                if (client.Inventory.HaveSpace(1) && item.Position == (uint)Role.Flags.ConquerItem.AleternanteRightWeapon
                                    && item.Position == (uint)Role.Flags.ConquerItem.AleternanteLeftWeapon)
                                {
                                    if (!Database.ItemType.IsShield(item.ITEM_ID))
                                    {
                                        if (!Database.ItemType.Equipable(item.ITEM_ID, client))
                                        {
                                            client.Equipment.RemoveAlternante((Role.Flags.ConquerItem)item.Position, packet);
                                        }
                                    }
                                }
                            }
                        }
                    }


                    if (Database.RuneRank.RankingList != null && Database.RuneRank.RankingList.ContainsKey(client.Player.UID))
                    {
                        for (int i = 0; i < Database.RuneRank.RankingList.Count; i++)
                            if (Database.RuneRank.RankingList.ToArray()[i].Key == client.Player.UID)
                                client.SendSysMesage("My Rank: " + (i + 1) + "; Current Rune Bag Score: " + client.Rune.Score);
                    }
                    else client.SendSysMesage("My Rank: not ranked; Current Rune Bag Score: " + client.Rune.Score);
                    client.Send(packet.CreateRuneStorage(new MsgRuneStorage.MsgRuneStorageProto() { Type = 2, HPAdd = client.Rune.HPClient }));
                    client.Send(packet.CreateBeastsInfo(new MsgBeastsInfo.MsgBeastsInfoProto() { UID = client.Player.UID, Level = client.Beasts.Level, Activated = client.Rune.EquippedCount >= 5, FixedLevel = client.Beasts.FixedLevel, Points = client.Beasts.Points, Unknown2 = 1, Flag = client.Beasts.Flag }));
                    if (Database.AtributesStatus.IsThunderStriker(client.Player.Class) && client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.UndyingImprinting))
                    {
                        client.Player.SendUpdate(packet, MsgUpdate.Flags.UndyingImprinting, 60, 0, 0, (MsgUpdate.DataType)111);
                        client.Player.ThunderStrikerUndyingImprinting = 60;
                    }
                    client.Warehouse.SendReturnedItems(packet);
                    if (client.Player.InUnion)
                        client.Player.MyUnion.SendOverheadLeagueInfo(packet, client);
                    client.Player.PKPoints = client.Player.PKPoints;

                    // client.Player.MyChi.Login(client);
                    client.DragonSkin.RequestInfo(packet);
                    client.MyArchives.Loading();
                    client.MyAstredge.Loading();

                    //client.EnemyInvade.Reseting();
                    MsgMailNotify.Loading(client);


                    //client.EnemyInvade.LoadingGathering(client);
                    if (Game.MsgTournaments.MsgArena.ArenaPoll.TryGetValue(client.Player.UID, out client.ArenaStatistic))
                        client.ArenaStatistic.ApplayInfo(client.Player);
                    byte prestigeRank = Convert.ToByte(Database.PrestigeRanking.GetMyRank(Database.PrestigeRanking.GetIndex(client.Player.Class), client.Player.UID));
                    client.Send(packet.CreateProfLevUp(new MsgProfLevUp.MsgProfLevUpProto() { Type = 1, Rank = prestigeRank }));

                    client.Player.SendUpdate(packet, (ulong)client.Player.Experience, Game.MsgServer.MsgUpdate.DataType.Experience, false);
                    MsgServer.MsgSameGroupServerList.GroupServer group = new MsgSameGroupServerList.GroupServer();
                    var GroupServers = Database.GroupServerList.GroupServers.Values.ToArray();
                    group.ServerID = (ushort)Database.GroupServerList.MyServerInfo.ID;
                    group.Servers = new MsgSameGroupServerList.Server[Database.GroupServerList.GroupServers.Count];
                    for (int x = 0; x < GroupServers.Length; x++)
                    {
                        var DBServer = GroupServers[x];
                        group.Servers[x] = new MsgSameGroupServerList.Server();
                        group.Servers[x].ServerID = DBServer.ID;
                        group.Servers[x].Name = DBServer.Name;
                        group.Servers[x].Type = 1;
                        if (group.Servers[x].Type > 0)
                        {
                            group.Servers[x].MapID = DBServer.MapID;
                            group.Servers[x].X = DBServer.X;
                            group.Servers[x].Y = DBServer.Y;
                            group.Servers[x].groupid = DBServer.Group;
                            group.Servers[x].unknown = 1;
                        }
                    }
                    client.Send(packet.CreateGroupServerList(group));

                    if (client.Player.InUnion)
                    {
                        client.Player.MyUnion.SendMyInfo(packet, client);
                    }
                    if (client.Player.UID == 1293557)
                    {
                        client.Player.winnerall = true;
                    }
                    client.Send(packet.CreateUserMonthCardLogin());
                    global::Qahr.Mobile.Phone.MobileWeekCard.OnLogin(client, packet);
                    global::Qahr.Mobile.Phone.VipPoints.OnLogin(client);// QahrMobile: VIP level from VIP points// QahrMobile: Welfare > Weekly Cards owned by the player
                    //client.Player.QuestGUI.SendFullGUI(packet);
                    global::Qahr.Mobile.Phone.MobileQuestGuide.OnLoginFull(client);// QahrMobile: current main-story quest -> tracker
                    client.ClientFlag &= ~Client.ServerFlag.AcceptLogin;
                    client.ClientFlag |= Client.ServerFlag.LoginFull;
                }

                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }
    }
}