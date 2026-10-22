using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Qahr.Mobile.Game.MsgServer;
using System.Net.Sockets;
using System.Net;
using Qahr.Mobile.MsgInterServer.Packets;

namespace Qahr.Mobile.MsgInterServer
{
    public class PipeClient
    {
        public bool Alive { get; private set; }
        public ServerSockets.SecuritySocket Socket;
        public Client.GameClient Owner;
        public uint NewUserID = 0;
        public string ConnectToAddress = "";
        public ushort ConnectToPort = 0;
        public PipeClient()
        {

        }
        public PipeClient(ServerSockets.SecuritySocket _socket, Client.GameClient obj)
        {
            Socket = _socket;
            Alive = true;
            Owner = obj;
            Socket.Client = this;
        }/*44 00 45 09 08 00 10 B2 E4 01 18 E0 5D 20 90 4E 28 00 30 00 38 06 38 04 38 04 38 03 38 04 38 01 38 02 38 01  
38 02 38 01 38 02 38 62 38 05 38 01 38 03 38 62 38 02 42 04 08 08 10 02 42 04 08 11 10 03 48 DA A7 A9 C8 05*/
        public void CompleteConnected()
        {

            Owner.PipeClient = this;
            using (var rec = new ServerSockets.RecycledPacket())
            {

                var stream = rec.GetStream();

                if (Owner.Player.CheckTransfer)
                {
                    Send(stream.InterServerCheckTransferCreate(0, Owner.Player.UID));
                }
                else
                {
                    if (Owner.Player.OnTransfer)
                        Send(stream.ClientInfo(Owner.Player, 1));
                    else
                        Send(stream.ClientInfo(Owner.Player, 0));
                    //if (Owner.Player.SetLocationType == 5)
                    //{
                    //    Send(stream.InterCrossPoker(Owner.Player.TableID));

                    //}
                    foreach (var item in Owner.Inventory.ClientItems.Values)
                        item.Send(this, stream);
                    foreach (var item in Owner.Equipment.ClientItems.Values)
                        item.Send(this, stream);
                    foreach (var item in Owner.Rune.Objects)
                        item.Send(this, stream);
                    foreach (var item in Owner.MythSoulBag.Values)
                        item.Send(this, stream);
                    Send(stream.SubClassCreate(MsgSubClass.Action.ShowGUI, Owner.Player.SubClass.StudyPoints, 0, Owner.Player.SubClass.src.Values.ToArray()));

                    if (Owner.Player.OnTransfer == false)
                    {
                        if (Owner.Player.MyChi != null)
                            Send(stream.ChiInfoCreate(MsgChiInfo.Action.InterServerStatus, Owner.Player.MyChi));
                    }
                    //send loader info ---- 
                   // Send(stream.InterServerLoaderCreate(Owner.EncryptTokenSpell));

                    //send spells---
                    foreach (var spell in Owner.MySpells.ClientSpells.Values)
                        Send(stream.SpellCreate(spell));
                    //--
                    //send profs--
                    foreach (var prof in Owner.MyProfs.ClientProf.Values)
                        Send(stream.ProficiencyCreate(prof.ID, prof.Level, prof.Experience, prof.NeededExperience));
                    //----
                    Send(stream.CreateBeastsInfo(new MsgBeastsInfo.MsgBeastsInfoProto() { UID = Owner.Player.UID, Level = Owner.Beasts.Level, Activated = Owner.Rune.EquippedCount >= 5, FixedLevel = Owner.Beasts.FixedLevel, Points = Owner.Beasts.Points, Unknown2 = 1, Flag = Owner.Beasts.Flag }));

                    Send(stream.InterServerStringTextCreate(Owner));
                    
                    if (Owner.Player.OnTransfer)
                    {

                        if (Owner.Player.MyJiangHu != null)
                            Send(stream.InterServerJiangHuCreate(Owner));
                        if (Owner.Player.MyChi != null)
                            Send(stream.InterServerChiCreate((uint)Owner.Player.MyChi.ChiPoints, Owner.Player.MyChi.Dragon.ToString(), Owner.Player.MyChi.Phoenix.ToString()
                                , Owner.Player.MyChi.Turtle.ToString(), Owner.Player.MyChi.Tiger.ToString()));

                        if (Owner.Player.InnerPower != null)
                            Send(stream.InterServerInnerPowerCreate(Owner.Player.InnerPower.ToString()));

                        if (Owner.Achievement != null)
                        {
                            Owner.Player.Achievement.Save(Owner.Achievement);
                            Send(stream.InterServerAchievementCreate(Owner.Achievement.ToString()));
                        }
                        if (Owner.Player.SpecialTitles.Count > 0)
                        {
                            Send(stream.InterServerSpecialTitlesCreate(Database.ServerDatabase.GetSpecialTitles(Owner)));
                        }
                        Send(stream.InterServerRoleInfoCreate(Owner.Player));
                    }
                    else
                    {
                        //send guild--
                        if (Owner.Player.MyGuild != null && Owner.Player.MyGuildMember != null)
                        {
                            Send(stream.GuildInfoCreate((uint)(Database.GroupServerList.MyServerInfo.ID * 100000 + Owner.Player.MyGuild.Info.GuildID), Owner.Player.GuildRank, Owner.Player.MyGuild.GuildName
                                , Owner.Player.MyGuild.Info.LeaderName));

                        }
                        //----
                        //send union
                        if (Owner.Player.InUnion)
                        {
                            Send(stream.UnionInfoCreate((uint)(Database.GroupServerList.MyServerInfo.ID * 100000 + Owner.Player.MyUnion.UID), Owner.Player.UnionMemeber.Rank
                                , Owner.Player.MyUnion.Name, Owner.Player.MyUnion.Emperor, Owner.Player.MyUnion.IsKingdom));
                        }

                        //----

                        //send quests
                        if (ConnectToAddress == Database.GroupServerList.InterServer.IPAddress && ConnectToPort == Database.GroupServerList.InterServer.Port)
                        {
                            if (Owner.Player.SetLocationType == 0)
                            {
                                AcceptQuest(stream, Owner, 35024);
                                AcceptQuest(stream, Owner, 35007);
                                AcceptQuest(stream, Owner, 35025);
                                AcceptQuest(stream, Owner, 35028);
                                AcceptQuest(stream, Owner, 35034);
                            }
                        }
                        //---
                    }
                }

                Owner.Player.View.Clear(stream);
            }
            Owner.Map.Denquer(Owner);
            MyConsole.WriteLine("[WARNING] Information of " + Owner.Player.Name + " has been sended successfully to PipeServer .");
        }

        public void Send(ServerSockets.Packet msg)
        {
            if (Alive)
                Socket.Send(msg);
        }
        public void Disconnect()
        {
            if (Alive)
            {
                Alive = false;
                Socket.Disconnect();
            }

        }

        public static void Connect(Client.GameClient user, string IPAddres, ushort Port)
        {
            if (user.Player.ContainFlag(MsgUpdate.Flags.Ride))
                user.Player.RemoveFlag(MsgUpdate.Flags.Ride);
            MyConsole.WriteLine("[WARNING] " + user.Player.Name + " is connecting to PipeServer .");
            LoginQueue.Enqueue(new PipeClient() { Owner = user, ConnectToAddress = IPAddres, ConnectToPort = Port });
        }

        public unsafe static void FilterPackets(ServerSockets.SecuritySocket obj, ServerSockets.Packet stream)
        {
            var pipe = obj.Client as PipeClient;
            ushort PacketID = stream.ReadUInt16();
            if (pipe == null)
            {
                MyConsole.WriteLine("[PipeClient] Receiving packets error (Null Client) .");
                return;
            }
            try
            {
                switch (PacketID)
                {
                    #region PacketManual(PacketTypes)

                    #region (PacketTypes)CheckTransfer
                    case PacketTypes.InterServer_CheckTransfer:
                        {
                            uint type;
                            uint UID;
                            stream.GetInterServerCheckTransfer(out type, out UID);
                            switch (type)
                            {
                                case 1:
                                    {
                                        pipe.Owner.CreateDialog(stream, "Transfer Failed ! is look like you already have account on " + pipe.Owner.Player.TransferToServer + ", please delete it then come back at me .", "Let me check.");
                                        pipe.Disconnect();
                                        break;
                                    }
                                case 2:
                                    {
                                        pipe.Owner.Player.CheckTransfer = false;
                                        pipe.Owner.Player.OnTransfer = true;
                                        // if (pipe.Owner.Player.ConquerPoints > 20000000)
                                        //    pipe.Owner.Player.ConquerPoints -= 20000000;
                                        pipe.CompleteConnected();
                                        break;
                                    }
                                case 3://complete transfer
                                    {
                                        string logs = "[Transfer]" + pipe.Owner.Player.Name + " UID [" + pipe.Owner.Player.UID + "]";
                                        Database.ServerDatabase.LoginQueue.Enqueue(logs);
                                        string MSG = "Success ! " + pipe.Owner.Player.Name + " has successfully transferred to " + pipe.Owner.Player.TransferToServer + " side .";
                                        Server.SendGlobalPacket(new MsgMessage(MSG, MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
                                        pipe.Owner.Player.Delete = true;
                                        pipe.Owner.Socket.Disconnect();
                                        break;
                                    }
                            }
                            break;
                        }
                    #endregion

                    #region (PacketTypes)ConnectionInfo
                    case PacketTypes.InterServer_ConnectionInfo:
                        {
                            uint ServerID;
                            uint Type;
                            stream.GetConnectionInfo(out Type, out ServerID);
                            if (Type == 1)
                            {
                                var ServerInfo = Database.GroupServerList.GetServer(ServerID);

                                pipe.Owner.Send(stream.MsgInterServerIdentifier(0, pipe.NewUserID, pipe.Owner.Player.UID));
                                pipe.Owner.Send(stream.MsgInterServerIdentifier(1, pipe.Owner.Player.UID, pipe.Owner.Player.UID));
                                RemoveQuest(stream, pipe.Owner, 35024);
                                RemoveQuest(stream, pipe.Owner, 35007);
                                RemoveQuest(stream, pipe.Owner, 35025);
                                RemoveQuest(stream, pipe.Owner, 35034);
                                RemoveQuest(stream, pipe.Owner, 35028);
                                pipe.NewUserID = 0;
                                pipe.Disconnect();
                                pipe.Owner.PipeClient = null;
                                if (ServerInfo != null)
                                    Connect(pipe.Owner, ServerInfo.IPAddress, ServerInfo.Port);
                            }
                            break;
                        }
                    #endregion

                    #region (PacketTypes)CreateItem
                    case PacketTypes.InterServer_CreateItem:
                        {
                            uint ID;
                            stream.GetInterCreateItem(out ID);
                            pipe.Owner.Inventory.Add(stream, ID);
                            break;
                        }
                    #endregion

                    #endregion

                    #region GamePackets

                    #region MsgCrossSwitch
                    case Game.GamePackets.MsgCrossSwitch:
                        {
                            if (pipe.Owner.Player.OnTransfer)
                            {
                                var action = new ActionQuery()
                                {
                                    ObjId = pipe.Owner.Player.UID,
                                    Type = ActionType.CompleteLogin
                                };
                                pipe.Send(stream.ActionCreate(action));

                            }
                            else
                            {
                                uint mode, dwparam1, dwparam2;
                                stream.GetInterServerIdentifier(out mode, out dwparam1, out dwparam2);
                                pipe.NewUserID = dwparam2;
                                stream.Seek(stream.Size);
                                pipe.Owner.Send(stream.MsgInterServerIdentifier(0, pipe.Owner.Player.UID, dwparam2));
                                var _server = Database.GroupServerList.GetServer(pipe.ConnectToAddress, pipe.ConnectToPort);
                                if (_server != null)
                                    pipe.Owner.Player.SendString(stream, MsgStringPacket.StringID.ServerName, 0, false, Database.GroupServerList.InterServer.Name);
                                else
                                {

                                    pipe.Owner.Player.SendString(stream, MsgStringPacket.StringID.ServerName, 0, false, Database.GroupServerList.InterServer.Name);
                                }

                                if (pipe.Owner.Player.MyChi != null)
                                {
                                    Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(pipe.Owner, MsgChiInfo.Action.Upgrade, pipe.Owner);
                                    foreach (var chipower in pipe.Owner.Player.MyChi)
                                        pipe.Owner.Player.MyChi.SendQueryUpdate(pipe.Owner, chipower, stream);
                                }
                                if (pipe.Owner.Player.MyJiangHu != null)
                                {
                                    pipe.Send(stream.StatusJiangHuCreate(pipe.Owner.Player.MyJiangHu.statusclient));
                                    pipe.Owner.Player.MyJiangHu.SendInfo(pipe.Owner, MsgJiangHuInfo.JiangMode.InfoStauts, false);
                                    pipe.Owner.Player.MyJiangHu.SendStatus(stream, pipe.Owner, pipe.Owner);
                                }
                                if (pipe.Owner.Player.MyGuild != null)
                                {
                                    pipe.Owner.Player.MyGuild.SendGuildAlly(stream, true, pipe.Owner);
                                    pipe.Owner.Player.MyGuild.SendGuilEnnemy(stream, true, pipe.Owner);
                                }

                                var action = new ActionQuery()
                                {
                                    ObjId = pipe.Owner.Player.UID,
                                    Type = ActionType.CompleteLogin
                                };
                                pipe.Send(stream.ActionCreate(action));
                            }
                            break;
                        }
                    #endregion

                    #region Item
                    case Game.GamePackets.MsgItemInfo:
                        {
                            MsgGameItem item;
                            stream.GetItemPacketPacket(out item);
                            if (item.Mode == Role.Flags.ItemMode.Update)
                            {
                                MsgGameItem ClientItem;
                                if (pipe.Owner.TryGetItem(item.UID, out ClientItem))
                                {
                                    if (ClientItem.Position == 0)
                                    {
                                        if (item.StackSize >= 1)
                                            ClientItem.StackSize = item.StackSize;
                                    }
                                }
                            }
                            stream.Seek(stream.Size);
                            pipe.Owner.Send(stream);
                            break;
                        }
                    #endregion

                    #region Usage
                    case Game.GamePackets.MsgItem:
                        {
                            Qahr.Mobile.Game.MsgServer.MsgItemUsuagePacket.ItemUsuageID action;
                            uint id;
                            ulong dwParam;
                            uint timestamp;
                            uint dwParam2;
                            uint dwParam3;
                            uint dwparam4;//unknow
                            uint dwparam5;
                            List<uint> args;
                            ulong A7A;
                            Qahr.Mobile.Game.MsgServer.MsgItemUsuagePacket.MsgItemProto Info;
                            stream.GetUsageItem(out action, out id, out dwParam, out timestamp, out dwParam2, out dwParam3, out dwparam4, out A7A, out dwparam5, out args, out Info);

                            if (action == MsgItemUsuagePacket.ItemUsuageID.RemoveInventory)
                            {
                                MsgGameItem item;
                                if (pipe.Owner.Inventory.TryGetItem(id, out item))
                                {
                                    if (item.ITEM_ID >= 1000000 && item.ITEM_ID <= 1000040
                                        || item.ITEM_ID >= 1002000 && item.ITEM_ID <= 1002030
                                        || item.ITEM_ID == 1002050
                                        || item.ITEM_ID == 725065 || item.ITEM_ID == 1003010
                                        || item.ITEM_ID >= 1001000 && item.ITEM_ID <= 1001040
                                        || item.ITEM_ID == 1002030 || item.ITEM_ID == 1002040
                                        || item.ITEM_ID == 725066 || item.ITEM_ID == 1004010)
                                    {
                                        pipe.Owner.Inventory.ClientItems.TryRemove(id, out item);
                                    }
                                }
                            }
                            stream.Seek(stream.Size);
                            pipe.Owner.Send(stream);
                            break;
                        }
                    #endregion

                    #region QuestList
                    case Game.GamePackets.MsgTaskStatus:
                        {
                            MsgQuestList.QuestMode Mode;
                            ushort Count;
                            uint QuestID;
                            MsgQuestList.QuestListItem.QuestStatus QuestaMode;
                            uint QuestTimer;

                            stream.GetQuestList(out Mode, out Count, out QuestID, out QuestaMode, out QuestTimer);
                            if (QuestID == 35028 && QuestaMode == MsgQuestList.QuestListItem.QuestStatus.Finished)
                            {
                                if (pipe.Owner.Player.QuestGUI.CheckQuest(QuestID, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                {
                                    pipe.Owner.Inventory.Add(stream, 3600029);
                                    pipe.Owner.Player.QuestGUI.FinishQuest(QuestID);
                                }
                            }

                            stream.Seek(stream.Size);
                            pipe.Owner.Send(stream);
                            break;
                        }
                    #endregion

                    #region QuestData
                    case Game.GamePackets.MsgTaskDetailInfo:
                        {
                            uint UnKnow; uint UID;
                            uint[] intents;
                            stream.GetQuestData(out UnKnow, out UID, out intents);
                            if (pipe.Owner.OnInterServer)
                            {
                                if (Database.QuestInfo.IsKingDomMission(UID))
                                {
                                    pipe.Owner.Player.QuestGUI.SetKingDomQuestObjectives(stream, UID, intents);
                                }
                            }
                            stream.Seek(stream.Size);
                            pipe.Owner.Send(stream);
                            break;

                        }
                    #endregion

                    #region Update
                    case Game.GamePackets.MsgUserAttrib:
                        {

                           // stream.Seek(stream.Size);
                            pipe.Owner.Send(stream);
                            break;

                        }
                    #endregion
                    #endregion
                    default:
                        {
                            stream.Seek(stream.Size);

                            pipe.Owner.Send(stream);
                            break;
                        }
                }

            }
            catch (Exception e)
            {
                MyConsole.SaveException(e);
            }
            finally
            {
                ServerSockets.PacketRecycle.Reuse(stream);
            }
        }

        public static void AcceptQuest(ServerSockets.Packet stream, Client.GameClient user, uint MissionID)
        {
            Game.MsgServer.MsgQuestList.QuestListItem Quest;

            if (MissionID == 35028)
            {
                if (user.Player.QuestGUI.AcceptKingDomMission(Database.QuestInfo.GetFinishQuest((ushort)Game.MsgNpc.NpcID.Crystal, user.Player.Class, MissionID), 0, out Quest))
                    CreateQuest(stream, user, MissionID, Quest);
                return;
            }
            if (user.Player.QuestGUI.AcceptKingDomMission(Database.QuestInfo.GetFinishQuest((ushort)Game.MsgNpc.NpcID.KingdomMissionEnvoy, user.Player.Class, MissionID), 0, out Quest))
                CreateQuest(stream, user, MissionID, Quest);
        }
        public static void RemoveQuest(ServerSockets.Packet stream, Client.GameClient user, uint MissionID)
        {
            stream.QuestListCreate(MsgQuestList.QuestMode.QuitQuest, 1);
            stream.AddItemQuestList(new MsgQuestList.QuestListItem() { UID = MissionID, Status = MsgQuestList.QuestListItem.QuestStatus.Available });
            stream.QuestListFinalize();
            user.Send(stream);
        }

        public static void CreateQuest(ServerSockets.Packet stream, Client.GameClient user, uint MissionID, Game.MsgServer.MsgQuestList.QuestListItem Quest)
        {
            if (user.Player.QuestGUI.CheckQuest(MissionID, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
            {
                stream.QuestListCreate(MsgQuestList.QuestMode.AcceptQuest, 1);
                stream.AddItemQuestList(new MsgQuestList.QuestListItem() { UID = MissionID, Status = MsgQuestList.QuestListItem.QuestStatus.Accepted });
                stream.QuestListFinalize();
                user.PipeClient.Send(stream);
                user.PipeClient.Send(stream.MsgQuestDataCreate(0, Quest.UID, Quest.Intentions));
            }
        }

        public static ExecuteLogin LoginQueue = new ExecuteLogin();

        public class ExecuteLogin : ConcurrentSmartThreadQueue<PipeClient>
        {

            public ExecuteLogin()
                : base(5)
            {
                Start(10);
            }
            public void TryEnqueue(PipeClient obj)
            {
                base.Enqueue(obj);
            }
            protected unsafe override void OnDequeue(PipeClient obj, int time)
            {
                try
                {
                    var socket = new ServerSockets.SecuritySocket(new Action<ServerSockets.SecuritySocket>(Disconnect), new Action<ServerSockets.SecuritySocket, ServerSockets.Packet>(FilterPackets));
                    Socket _socket;
                    if (socket.Connect(obj.ConnectToAddress, obj.ConnectToPort, out _socket))
                    {
                        socket.Create(_socket);
                        socket.OnInterServer = true;
                        new PipeClient(socket, obj.Owner) { ConnectToAddress = obj.ConnectToAddress, ConnectToPort = obj.ConnectToPort }.CompleteConnected();
                        socket.ConnectFull = true;
                    }
                    else obj.Owner.SendSysMesage("Server is offline at this moment, try again later.");
                }
                catch (Exception e)
                {
                    MyConsole.SaveException(e);
                }
            }
        }
        public static void Disconnect(ServerSockets.SecuritySocket obj)
        {
            var pipe = obj.Client as PipeClient;
            if (pipe.NewUserID != 0)
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var msg = rec.GetStream();
                    if (!pipe.Owner.Player.CheckTransfer)
                    {
                        pipe.Owner.Send(msg.MsgInterServerIdentifier(0, pipe.NewUserID, pipe.Owner.Player.UID));
                        pipe.Owner.Send(msg.MsgInterServerIdentifier(1, pipe.Owner.Player.UID, pipe.Owner.Player.UID));


                        if (pipe.Owner.Player.SetLocationType == 0)
                        {
                            RemoveQuest(msg, pipe.Owner, 35024);
                            RemoveQuest(msg, pipe.Owner, 35007);
                            RemoveQuest(msg, pipe.Owner, 35025);
                            RemoveQuest(msg, pipe.Owner, 35034);
                            RemoveQuest(msg, pipe.Owner, 35028);
                        }
                        pipe.Owner.Player.Stamina = 100;
                        pipe.Owner.Player.SendUpdate(msg, pipe.Owner.Player.Stamina, MsgUpdate.DataType.Stamina);
                        pipe.Owner.Player.SetLocationType = 0;


                        pipe.Owner.Player.CreateHeavenBlessPacket(msg, false);

                        if (pipe.Owner.Player.MyGuild != null && pipe.Owner.Player.MyGuildMember != null)
                        {
                            pipe.Owner.Player.GuildBattlePower = pipe.Owner.Player.MyGuild.ShareMemberPotency(pipe.Owner.Player.MyGuildMember.Rank);
                            pipe.Owner.Player.MyGuild.SendGuildAlly(msg, true, pipe.Owner);
                            pipe.Owner.Player.MyGuild.SendGuilEnnemy(msg, true, pipe.Owner);
                        }
                        if (pipe.Owner.Player.VipLevel >= 6)
                            pipe.Owner.Player.UpdateVip(msg);

                        pipe.Owner.Equipment.QueryEquipment(pipe.Owner.Equipment.Alternante);
                        pipe.Owner.Player.CreateHeavenBlessPacket(msg, true);
                    }

                    pipe.NewUserID = 0;
                    pipe.Disconnect();
                    pipe.Owner.PipeClient = null;
                    pipe.Owner.Teleport(pipe.Owner.Player.X, pipe.Owner.Player.Y, pipe.Owner.Player.Map);
                    pipe.Owner.Player.SetPkMode(Role.Flags.PKMode.Capture);


                    pipe.Owner.Player.RemoveFlag(MsgUpdate.Flags.Ride);
                    pipe.Owner.Player.UpdateFlagOffset();


                    if (!pipe.Owner.Player.CheckTransfer)
                    {
                        if (pipe.Owner.Player.MyGuild != null)
                            pipe.Owner.Player.MyGuild.SendThat(pipe.Owner.Player);

                        pipe.Owner.Player.SendUpdateHP();

                    }
                }
            }
        }
    }
}
