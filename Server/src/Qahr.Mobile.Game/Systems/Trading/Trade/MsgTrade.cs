using ProtoBuf;
using System;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static class MsgTrade
    {
        [ProtoContract]
        public class Trade
        {
            [ProtoMember(1, IsRequired = true)]
            public uint Action;
            [ProtoMember(2, IsRequired = true)]
            public ulong Money;
            [ProtoMember(3, IsRequired = true)]
            public uint ItemUID;
            [ProtoMember(4, IsRequired = true)]
            public uint CPS;
            [ProtoMember(5, IsRequired = true)]
            public uint Unknown;
            [ProtoMember(6, IsRequired = true)]
            public uint UID;
        }
        [ProtoContract]
        public class TradeInfo
        {
            [ProtoMember(1, IsRequired = true)]
            public bool un1;
            [ProtoMember(2, IsRequired = true)]
            public uint un2;
            [ProtoMember(3, IsRequired = true)]
            public uint Sender;
            [ProtoMember(4, IsRequired = true)]
            public string Name;
            [ProtoMember(5, IsRequired = true)]
            public uint Receiver;
            [ProtoMember(6, IsRequired = true)]
            public uint Level;
            [ProtoMember(7, IsRequired = true)]
            public uint BattlePower;

            [ProtoMember(8, IsRequired = true)]
            public uint un8;

            [ProtoMember(9, IsRequired = true)]
            public uint un9;

            [ProtoMember(10, IsRequired = true)]
            public bool Apprentice;

            [ProtoMember(11, IsRequired = true)]
            public bool Spouse;

            [ProtoMember(12, IsRequired = true)]
            public bool Friend;

            [ProtoMember(13, IsRequired = true)]
            public bool TradePartner;

            [ProtoMember(14, IsRequired = true)]
            public bool Mentor;

            [ProtoMember(15, IsRequired = true)]
            public bool unk15;

            [ProtoMember(16, IsRequired = true)]
            public bool Teammate;

            [ProtoMember(17, IsRequired = true)]
            public bool GuildMember;

            [ProtoMember(18, IsRequired = true)]
            public bool Enemy;

            [ProtoMember(19, IsRequired = true)]
            public uint un19;

            [ProtoMember(20, IsRequired = true)]
            public long un20;

            public ServerSockets.Packet ToArray()
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    stream.InitWriter();
                    stream.ProtoBufferSerialize(this);
                    stream.Finalize(2255);
                    return stream;
                }
            }
        }

        public enum TradeID : uint
        {
            Request = 0,
            AcceptRequest = 1,
            Close = 2,
            ShowTable = 3,
            Finish = 4,
            HideTable = 5,
            AddItem = 6,
            SetMoney = 7,
            ShowMoney = 8,
            Accept = 9,
            RemoveItem = 10,
            SetCPS = 11,
            ShowCPS = 12,
        }

        public static unsafe ServerSockets.Packet CreateTradeInfo(this ServerSockets.Packet stream, Trade obj)
        {
            stream.InitWriter();
            obj.Unknown = Phone.MobileTrade.ToMobile(obj.Action);// QahrMobile: the mobile's action number (field 5 was unused; the 2029 layout writes it at @12)
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgTrade);
            return stream;
        }
        public static unsafe void GetTradeInfo(this ServerSockets.Packet stream, out Trade pQuery)
        {
            pQuery = new Trade();
            pQuery = stream.ProtoBufferDeserialize<Trade>(pQuery);
        }

        // Serializes the final trade-Accept transfer. Two Accept packets (one per partner) can land on
        // different I/O threads at the same instant; without this lock both ran the transfer, doubling
        // the Money/CP. The critical section is tiny, so a single global lock is fine.
        private static readonly object TradeAcceptLock = new object();

        [PacketAttribute(GamePackets.MsgTrade)]
        private static void HandlerTrade(Client.GameClient user, ServerSockets.Packet stream)
        {
            if (user.IsVendor) // الشوب بتاع المركت الى بتقعد تبيع فيه 
            {
                user.CreateBoxDialog("Sorry, You Can`t Trade With Anyone Sitting In The Shop .");
                return;
            }
            if (!user.Player.IsCheckedPass) return;
            Client.GameClient INGame;
            if (Pool.GamePoll.TryGetValue(user.Player.UID, out INGame))
            {
                if (INGame.Socket != user.Socket)
                {
                    Database.ServerDatabase.LoginQueue.Enqueue("[Login_Dub] [ " + user.Player.Name + " ] [ " + user.Player.UID + " ] Another Socket On Game IP : [ " + user.Socket.RemoteIp + " ] On Game MacAddress [ " + user.OnLogin.MacAddress + " ]" + "  [The Clock] " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second);
                    MyConsole.WriteLine("[Login_Dub] [ " + user.Player.Name + " ] [ " + user.Player.UID + " ] (Another Socket) .", ConsoleColor.White);
                    user.Socket.Disconnect();
                    INGame.Socket.Disconnect();
                    return;
                }

            }
            else
            {
                Database.ServerDatabase.LoginQueue.Enqueue("[Login_Dub] " + user.Player.Name + " " + user.Player.UID + " >> Recv Not IN GamePoll On Game IP : [ " + user.Socket.RemoteIp + " ] On Game MacAddress [ " + user.OnLogin.MacAddress + " ]" + "  [The Clock] " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second);
                MyConsole.WriteLine("[Login_Dub] [ " + user.Player.Name + " ] [ " + user.Player.UID + " ] (Recv Not IN Game Poll) .", ConsoleColor.White);
                user.Socket.Disconnect();
                return;
            }
            if (!user.Player.IsCheckedPass)
                return;
            Trade trade = new Trade();
            stream.GetTradeInfo(out trade);
            bool mobileUser = Phone.MobileTrade.IsMobile(user);// QahrMobile: the mobile uses other action numbers (MobileTrade.cs)
            if (mobileUser) { trade.Action = Phone.MobileTrade.FromMobile(user, trade.Action, trade.UID); Phone.MobileTrade.Normalize(trade); }
            if (user.Player.UID == trade.UID)
            {
                // no ban: the server itself sends the Finish packet carrying the receiver's own UID,
                // so a client echo is not proof of a hack. Only Request/AcceptRequest use trade.UID; refuse those.
                MyConsole.WriteLine("[Trade_Own_UID] [ " + user.Player.Name + " ] [ " + user.Player.UID + " ] own UID in trade Action " + trade.Action + ".", ConsoleColor.White);
                Database.ServerDatabase.LoginQueue.Enqueue("[Bug_Trade_UID] [UID Player] >> ( " + user.Player.UID + " ) [Name Player] >> ( " + user.Player.Name + " ) sent a trade packet with his own UID, Action " + trade.Action + ".");
                if ((TradeID)trade.Action == TradeID.Request || (TradeID)trade.Action == TradeID.AcceptRequest)
                    return;
            }
            switch ((TradeID)trade.Action)
            {
                case TradeID.Request:
                    {
                        if (user.Player.Map == 601)
                        {
                            user.SendSysMesage("You can`t trade in this map.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                            return;
                        }
                        if (user.IsVendor) // الشوب بتاع المركت الى بتقعد تبيع فيه 
                        {
                            user.CreateBoxDialog("Sorry, You Can`t Trade With Anyone Sitting In The Shop .");
                            return;
                        }
                        if (user.Player.Map == 3852)
                        {
                            user.CreateBoxDialog("Sorry, trading with anyone else on the roulette map is closed.");
                            return;
                        }
                        if (user.MyTrade == null)
                            user.MyTrade = new Role.Instance.Trade(user);

                        if (trade.UID == user.Player.UID)
                            return;
                        Role.IMapObj obj;
                        if (user.Player.View.TryGetValue((uint)trade.UID, out obj, Role.MapObjectType.Player))
                        {
                            Client.GameClient Partner = (obj as Role.Player).Owner;
                            if (Partner != null)
                            {
                                if (!Partner.InTrade && !user.InTrade)
                                {
                                    user.Player.targetTrade = (uint)Partner.Player.UID;
                                    user.MyTrade.Target = Partner;

                                    user.MyTrade.Target.MyTrade = new Role.Instance.Trade(user.MyTrade.Target) { Target = user };

                                    Partner.MyTrade.Target = user;

                                    if (!mobileUser) user.Send(stream.CreateTradeInfo(trade));// QahrMobile: the echo would show the mobile requester its own request
                                    trade.UID = user.Player.UID;
                                    Partner.Send(stream.CreateTradeInfo(trade));

                                    Partner.Send(new TradeInfo()
                                    {
                                        un1 = true,
                                        Sender = user.Player.UID,
                                        Level = user.Player.Level,
                                        Receiver = Partner.Player.UID,
                                        Name = user.Player.Name,
                                        BattlePower = (uint)user.Player.BattlePower,
                                        Spouse = Partner.Player.Name == user.Player.Spouse,
                                        Enemy = user.Player.MyGuild != null && user.Player.MyGuild.Enemy.ContainsKey(Partner.Player.UID),
                                        Friend = user.Player.Associate.Contain(Role.Instance.Associate.Friends, Partner.Player.UID),
                                        TradePartner = user.Player.Associate.Contain(Role.Instance.Associate.Partener, Partner.Player.UID),
                                        Apprentice = user.Player.Associate.Contain(Role.Instance.Associate.Apprentice, Partner.Player.UID),
                                        Mentor = (user.Player.MyMentor != null && user.Player.MyMentor.MyUID == Partner.Player.UID),
                                        Teammate = (user.Team != null && user.Team.Members.ContainsKey(Partner.Player.UID)),
                                        GuildMember = (user.Player.MyGuild != null && user.Player.MyGuild.Members.ContainsKey(Partner.Player.UID)),
                                        // un22 = true,
                                    }.ToArray());

                                }
                                else
                                {

                                    user.SendSysMesage("The Player is Already in The Trade!");
                                }
                            }
                        }
                        break;
                    }
                case TradeID.AcceptRequest:
                    {
                        if (user.Player.Map == 601)
                        {
                            user.SendSysMesage("You can`t trade in this map.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                            return;
                        }
                        if (user.IsVendor) // الشوب بتاع المركت الى بتقعد تبيع فيه 
                        {
                            user.CreateBoxDialog("Sorry, You Can`t Trade With Anyone Sitting In The Shop .");
                            return;
                        }
                        if (user.Player.Map == 3852)
                        {
                            user.CreateBoxDialog("Sorry, trading with anyone else on the roulette map is closed.");
                            return;
                        }
                        if (trade.UID == user.Player.UID)
                            return;
                        Role.IMapObj obj;
                        if (user.Player.View.TryGetValue((uint)trade.UID, out obj, Role.MapObjectType.Player))
                        {
                            Client.GameClient Target = (obj as Role.Player).Owner;
                            if (Target != null)
                            {
                                if (!Target.InTrade && !user.InTrade)
                                {
                                    user.Player.targetTrade = (uint)Target.Player.UID;
                                    user.MyTrade.Target = Target;
                                    if (Target.Player.targetTrade == user.Player.UID && user.Player.targetTrade == Target.Player.UID)
                                    {
                                        Target.MyTrade.WindowOpen = user.MyTrade.WindowOpen = true;
                                        Target.MyTrade.Confirmed = user.MyTrade.Confirmed = false;

                                        trade.Action = (uint)TradeID.ShowTable;
                                        user.Send(stream.CreateTradeInfo(trade));

                                        trade.UID = user.Player.UID;
                                        trade.Action = (uint)TradeID.ShowTable;
                                        Target.Send(stream.CreateTradeInfo(trade));
                                    }
                                }
                            }
                        }
                        break;
                    }
                case TradeID.Close:
                    {
                        if (user.InTrade)
                        {
                            user.MyTrade.CloseTrade(trade);
                        }
                        break;
                    }
                case TradeID.AddItem:
                    {
                        if (user.InTrade)
                        {
                            Game.MsgServer.MsgGameItem DataItem;
                            if (user.Inventory.TryGetItem((uint)trade.ItemUID, out DataItem))
                                user.MyTrade.AddItem(stream, trade, DataItem);

                        }
                        break;
                    }
                case TradeID.ShowCPS:
                    {
                        if (user.InTrade)
                            user.MyTrade.AddConquerPoints(trade, stream);
                        break;
                    }
                case TradeID.SetMoney:
                    {
                        if (user.InTrade)
                            user.MyTrade.AddMoney(trade, stream);
                        break;
                    }
                case TradeID.Accept:
                    {
                        try
                        {
                            if (user.MyTrade.Target.MyTrade.Target.Player.UID != user.Player.UID)
                            {
                                trade.Action = (uint)TradeID.Finish;
                                user.Send(stream.CreateTradeInfo(trade));
                                user.MyTrade = null;
                                break;
                            }
                        }
                        catch
                        {
                            trade.Action = (uint)TradeID.Finish;
                            user.Send(stream.CreateTradeInfo(trade));
                            user.MyTrade = null;
                            break;
                        }
                        if (user.InTrade)
                        {
                            if (user.MyTrade.Target.InTrade)
                            {
                              lock (TradeAcceptLock)
                              {
                                // Re-validate inside the lock: a simultaneous Accept from the partner may have
                                // already finished and nulled both trades, which used to let this run twice.
                                if (!user.InTrade || user.MyTrade == null || user.MyTrade.Target == null
                                    || user.MyTrade.Target.MyTrade == null || !user.MyTrade.Target.InTrade)
                                    break;
                                user.MyTrade.Confirmed = true;
                                if (!user.MyTrade.Target.MyTrade.Confirmed)
                                {
                                    trade.Action = (uint)TradeID.Accept;
                                    user.MyTrade.Target.Send(stream.CreateTradeInfo(trade));
                                }
                                else
                                {
                                    bool Acceped = false;
                                    if (user.Inventory.HaveSpace((byte)user.MyTrade.Target.MyTrade.Items.Count))
                                    {
                                        if (user.MyTrade.Target.MyTrade.ValidItems())
                                        {
                                            if (user.MyTrade.Target.Inventory.HaveSpace((byte)user.MyTrade.Items.Count))
                                            {
                                                if (user.MyTrade.ValidItems())
                                                {

                                                    user.Player.ConquerPoints += (uint)user.MyTrade.Target.MyTrade.ConquerPoints;


                                                    user.Player.Money += (ulong)user.MyTrade.Target.MyTrade.Money;
                                                    user.Player.SendUpdate(stream, user.Player.Money, MsgUpdate.DataType.Money);

                                                    user.MyTrade.Target.Player.ConquerPoints += (uint)user.MyTrade.ConquerPoints;
                                                    user.MyTrade.Target.Player.Money += (ulong)user.MyTrade.Money;
                                                    user.MyTrade.Target.Player.SendUpdate(stream, user.MyTrade.Target.Player.Money, MsgUpdate.DataType.Money);


                                                    foreach (var item in user.MyTrade.Target.MyTrade.Items.Values)
                                                    {
                                                        user.Inventory.Update(item, Role.Instance.AddMode.MOVE, stream);
                                                        user.MyTrade.Target.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream, true);

                                                    }
                                                    foreach (var item in user.MyTrade.Items.Values)
                                                    {
                                                        user.MyTrade.Target.Inventory.Update(item, Role.Instance.AddMode.MOVE, stream);
                                                        user.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream, true);

                                                    }
                                                }
                                                Acceped = true;
                                            }
                                        }
                                    }
                                    if (!Acceped)
                                    {
                                        user.SendSysMesage("There was an error with the trade", MsgServer.MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                        user.MyTrade.Target.SendSysMesage("There was an error with the trade", MsgServer.MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                        // Refund escrow (CP/Money were deducted when added to the window); otherwise
                                        // a failed Accept silently destroyed both players' staked CP and Money.
                                        user.MyTrade.DestroyItems(stream);
                                        user.MyTrade.Target.MyTrade.DestroyItems(stream);
                                    }
                                    user.Player.targetTrade = 0;
                                    user.MyTrade.Target.Player.targetTrade = 0;
                                    trade.Action = (uint)TradeID.Finish;

                                    trade.UID = user.Player.UID;
                                    user.Send(stream.CreateTradeInfo(trade));

                                    trade.UID = user.MyTrade.Target.Player.UID;
                                    user.MyTrade.Target.Send(stream.CreateTradeInfo(trade));

                                    user.Player.SendUpdate(stream, user.Player.Money, MsgUpdate.DataType.Money);
                                    user.MyTrade.Target.Player.SendUpdate(stream, user.MyTrade.Target.Player.Money, MsgUpdate.DataType.Money);

                                    user.MyTrade.Target.MyTrade = null;
                                    user.MyTrade = null;
                                }
                              }
                            }
                        }
                        break;
                    }

            }
        }
    }
}