using System.Linq;
using System.Runtime.InteropServices;

namespace Qahr.Mobile.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe void GetLottery(this ServerSockets.Packet stream, out MsgLottery.Action Mode, out byte Count, out ulong PostionItem, out byte GateID, out byte RanksItem, out uint JadesAddeds, out uint ItemID)
        {
            Mode = (MsgLottery.Action)stream.ReadUInt8();//4
            Count = stream.ReadUInt8();//5
            stream.ReadUInt16();//6
            PostionItem = stream.ReadUInt64();//8
            GateID = stream.ReadUInt8();//16
            RanksItem = stream.ReadUInt8();//17
            stream.ReadUInt32();//18
            stream.ReadUInt16();//22
            JadesAddeds = stream.ReadUInt32();//24
            ItemID = stream.ReadUInt32();//28
        }
        public static unsafe ServerSockets.Packet LotteryCreate(this ServerSockets.Packet stream, MsgLottery.Action Mode, byte Count, ulong PostionItem)
        {
            stream.InitWriter();
            stream.Write((byte)Mode);//4
            stream.Write(Count);//5 count
            stream.Write((ushort)0);//6
            stream.Write(PostionItem);//8
            return stream;
        }
        public static unsafe ServerSockets.Packet AddItemsLottery(this ServerSockets.Packet stream, byte GateID, byte RanksItem, uint JadesAddeds, uint ItemID)
        {
            stream.Write(GateID);//16
            stream.Write(RanksItem);//17
            stream.Write(0);//18
            stream.Write((ushort)0);//22
            stream.Write(JadesAddeds);//24
            stream.Write(ItemID);//28
            stream.Write((uint)0);//32
            return stream;
        }
        public static unsafe ServerSockets.Packet LotteryFinalize(this ServerSockets.Packet stream)
        {
            stream.Finalize(GamePackets.MsgLottery);
            return stream;
        }
    }
    [StructLayout(LayoutKind.Explicit, Size = 28)]
    public unsafe struct MsgLottery
    {
        public enum Action : uint
        {
            Accept = 0,
            AddJade = 1,
            Continue = 2,
            Show = 3,
            Buy = 4,
            Draw = 5,
            Melte = 6,
        }
        [Packet(GamePackets.MsgLottery)]
        private static void Procesor(Client.GameClient user, ServerSockets.Packet stream)
        {
            if (user.InTrade) return;
            
            Action Mode;
            byte GateID, RanksItem, Count;
            uint JadesAddeds, ItemID;
            ulong PostionItem;
            stream.GetLottery(out Mode, out Count, out PostionItem, out GateID, out RanksItem, out JadesAddeds, out ItemID);
            uint LottryItem = 0;
            if (GateID >= 0 && GateID < 6)
                LottryItem = 711504;
            else if (GateID >= 6 && GateID <= 9)
                LottryItem = 712018;
            else if (GateID >= 10 && GateID <= 15)
                LottryItem = 712019;
            else
                return;
            if (user.Player.PlayLunar == true)
                LottryItem = 712019;
            switch (Mode)
            {
                case Action.Melte:
                    {
                        user.CreateBoxDialog("Sorry, Melte System is temporarily closed and will be back up soon.");
                        break;
                        uint ID = 0;
                        if (user.Player.OldGate == true)
                            ID = 3329239;
                        if (user.Player.NewGate == true)
                            ID = 3329240;
                        Database.ItemType.DBItem DBItem;
                        if (user.Player.LotteryItem != null && user.Player.PlayOne == true)
                        {
                            user.Player.LotteryItem[PostionItem].ITEM_ID = ID;
                            if (Pool.ItemsBase.TryGetValue(user.Player.LotteryItem[PostionItem].ITEM_ID, out DBItem))
                            {
                                if (DBItem.StackSize > 0)
                                {
                                    user.Inventory.Add(user.Player.LotteryItem[PostionItem], DBItem, stream);
                                    user.Player.OldGate = false;
                                    user.Player.NewGate = false;
                                    user.Player.PlayOne = false;
                                }
                                else
                                {
                                    user.Inventory.Update(user.Player.LotteryItem[PostionItem], Role.Instance.AddMode.ADD, stream);
                                    user.Player.OldGate = false;
                                    user.Player.NewGate = false;
                                    user.Player.PlayOne = false;
                                }
                            }
                        }
                        stream.LotteryCreate(Mode, 0, PostionItem);
                        stream.AddItemsLottery(0, 0, 0, 0);
                        stream.LotteryFinalize();
                        user.Send(stream);
                        break;
                    }
                case Action.Draw:
                    {
                        uint ID = 0;
                        if (LottryItem == 711504)
                        {
                            user.Player.OldGate = true;
                            ID = 3329239;
                        }
                        if (LottryItem == 712019)
                        {
                            user.Player.NewGate = true;
                            ID = 3329240;
                        }
                        if (user.Inventory.Contain(LottryItem, 30))
                        {
                            user.Inventory.Remove(LottryItem, 30, stream);
                            user.Player.LotteryItem = new MsgGameItem[10];
                            stream.LotteryCreate(Mode, (byte)user.Player.LotteryItem.Length, 0);
                            user.Player.LotteryList.Clear();
                            for (int i = 0; i < user.Player.LotteryItem.Length; i++)
                            {
                                var GenerateLotteryPrize = Pool.Lottery.GenerateLotteryPrize((byte)GateID, user);
                                user.Player.LotteryItem[i] = Pool.Lottery.CreateGameItem(GenerateLotteryPrize[0]);
                                user.Player.LotteryList.Add(GenerateLotteryPrize);
                                stream.AddItemsLottery(GateID, (byte)GenerateLotteryPrize[1], user.Player.AddJade, GenerateLotteryPrize[0]);
                                if (!Pool.Lottery.NULLGETITEMS.Contains(GenerateLotteryPrize[0]))
                                    MsgBigLottery.AddWinner(new Pool.LvLottery
                                    {
                                        ItemID = GenerateLotteryPrize[0],
                                        GateID = GateID,
                                        RanksItem = GenerateLotteryPrize[1],
                                        Name = user.Player.Name
                                    });
                            }
                            user.Send(stream.LotteryFinalize());
                            if (user.Player.OldGate == true || user.Player.NewGate == true)
                            {
                                if (PostionItem > 0)
                                {
                                    foreach (var item in user.Player.LotteryList)
                                    {
                                        if (PostionItem == 8)
                                        {
                                            if (item[1] == PostionItem)
                                            {
                                                item[0] = ID;
                                                item[1] = 0;
                                            }
                                        }
                                        if (PostionItem != 8)
                                        {
                                            if (item[1] <= PostionItem)
                                            {
                                                item[0] = ID;
                                                item[1] = 0;
                                            }
                                        }
                                    }
                                    for (int o = 0; o < user.Player.LotteryItem.Length; o++)
                                    {
                                        var neww = user.Player.LotteryList[o];
                                        user.Player.LotteryItem[o].ITEM_ID = neww[0];
                                        if (neww[0] == ID)
                                        {
                                            stream.LotteryCreate(Action.Melte, 0, (ulong)o);
                                            stream.AddItemsLottery(0, 0, 0, 0);
                                            user.Send(stream.LotteryFinalize());
                                        }
                                        if (neww[0] == ID)
                                        {
                                            stream.LotteryCreate(Action.Melte, 0, (ulong)o);
                                            stream.AddItemsLottery(0, 0, 0, 0);
                                            user.Send(stream.LotteryFinalize());
                                        }
                                    }
                                }
                            }
                            var Info = new MsgBigLottery.MsgBigLotteryProto();
                            Info.Type = 0;
                            Info.Items = MsgBigLottery.Board();
                            user.Send(stream.CreateBigLottery(Info));
                        }
                        break;
                    }
                case Action.Buy:
                    {
                        user.CreateBoxDialog("Sorry, the System for Buying Lottery is now closed.");
                        break;
                    }
                        //int Cost = 9;
                        //if (LottryItem == 712019)
                        //    Cost = 45;
                        //if (user.Player.ConquerPoints > Cost * JadesAddeds)
                        //{
                        //    if (user.Inventory.HaveSpace(2))
                        //    {
                        //        user.Player.ConquerPoints -= (uint)(Cost * JadesAddeds);
                        //        user.Inventory.AddItemWitchStack(3331782, 0, (ushort)JadesAddeds, stream, false);
                        //        user.Inventory.AddItemWitchStack(LottryItem, 0, (ushort)JadesAddeds, stream, false);
                        //    }
                        //    else
                        //    {
                        //        user.CreateBoxDialog("Please make 2 space in your inventory.");
                        //    }
                    //    }
                    //    break;
                    //}
                case Action.Accept:
                    {
                        if ((user.Player.NewGate == true || user.Player.OldGate == true) && user.Player.PlayOne != true || user.Player.PlayLunar != true)
                        {
                            if (user.Player.LotteryItem != null)
                            {
                                if (RanksItem > 0)
                                {
                                    user.Inventory.AddItemWitchStack(LottryItem, 0, 1, stream, false);
                                    user.Player.GateID = 0;
                                    user.Player.AddJade = 3;
                                    user.Player.PlayOne = false;
                                    user.Player.NewGate = false;
                                    user.Player.OldGate = false;
                                    user.Player.PlayLunar = false;
                                    user.Player.LotteryItem = null;
                                    user.Player.LotteryList.Clear();
                                    stream.LotteryCreate(Mode, 0, PostionItem);
                                    stream.AddItemsLottery(GateID, RanksItem, JadesAddeds, ItemID);
                                    stream.LotteryFinalize();
                                    user.Send(stream);
                                    break;
                                }
                                for (int i = 0; i < user.Player.LotteryItem.Length; i++)
                                {
                                    Database.ItemType.DBItem DBItem;
                                    if (Pool.ItemsBase.TryGetValue(user.Player.LotteryItem[i].ITEM_ID, out DBItem))
                                    {
                                        if (user.Inventory.HaveSpace(1))
                                        {
                                            if (DBItem.StackSize > 0)
                                            {
                                                user.Inventory.Add(user.Player.LotteryItem[i], DBItem, stream);
                                            }
                                            else
                                            {
                                                user.Inventory.Update(user.Player.LotteryItem[i], Role.Instance.AddMode.ADD, stream);
                                            }
                                        }
                                        else
                                        {
                                            using (var recItem = new ServerSockets.RecycledPacket()) user.Inventory.AddReturnedItem(recItem.GetStream(), user.Player.LotteryItem[i].ITEM_ID, 1);
                                        }
                                    }
                                }
                            }
                            user.Player.NewGate = false;
                            user.Player.OldGate = false;
                            user.Player.PlayLunar = false;
                            user.Player.PlayOne = false;
                            user.Player.LotteryItem = null;
                            user.Player.LotteryList.Clear();
                            user.Player.GateID = 0;
                            user.Player.AddJade = 3;
                            stream.LotteryCreate(Mode, 0, PostionItem);
                            stream.AddItemsLottery(GateID, RanksItem, JadesAddeds, ItemID);
                            user.Send(stream.LotteryFinalize());
                            break;
                        }
                        else
                        {
                            if (user.Player.LotteryItem != null)
                            {
                                if (RanksItem > 0)
                                {
                                    user.Inventory.AddItemWitchStack(LottryItem, 0, 1, stream, false);
                                    user.Player.GateID = 0;
                                    user.Player.AddJade = 3;
                                    user.Player.PlayOne = false;
                                    user.Player.NewGate = false;
                                    user.Player.OldGate = false;
                                    user.Player.PlayLunar = false;
                                    user.Player.LotteryItem = null;
                                    user.Player.LotteryList.Clear();
                                    stream.LotteryCreate(Mode, 0, PostionItem);
                                    stream.AddItemsLottery(GateID, RanksItem, JadesAddeds, ItemID);
                                    stream.LotteryFinalize();
                                    user.Send(stream);
                                    break;
                                }
                                Database.ItemType.DBItem DBItem;



                                if (user.Inventory.HaveSpace(1))
                                {
                                    if (Pool.ItemsBase.TryGetValue(user.Player.LotteryItem[0].ITEM_ID, out DBItem))
                                    {
                                        if (DBItem.StackSize > 0)
                                        {
                                            user.Inventory.Add(user.Player.LotteryItem[0], DBItem, stream);
                                        }
                                        else
                                        {
                                            user.Inventory.Update(user.Player.LotteryItem[0], Role.Instance.AddMode.ADD, stream);
                                        }
                                    }
                                }
                                else
                                {
                                    using (var recItem = new ServerSockets.RecycledPacket()) user.Inventory.AddReturnedItem(recItem.GetStream(), user.Player.LotteryItem[0].ITEM_ID, 1);
                                }
                                if (user.Player.LotteryItem.Rank < 5)
                                {
                                    Server.SendGlobalPacket(new MsgMessage("Congratulations! " + user.Player.Name + " Won " + Pool.ItemsBase[user.Player.LotteryItem[0].ITEM_ID].Name + " in Lottery.", MsgMessage.MsgColor.white, MsgMessage.ChatMode.System).GetArray(stream));
                                    user.SendSysMesage("You won a " + Pool.ItemsBase[user.Player.LotteryItem[0].ITEM_ID].Name + " from the lottery!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                }
                                user.Player.GateID = 0;
                                user.Player.AddJade = 3;
                                user.Player.PlayLunar = false;
                                user.Player.PlayOne = false;
                                user.Player.NewGate = false;
                                user.Player.OldGate = false;
                                user.Player.LotteryItem = null;
                                user.Player.LotteryList.Clear();
                                stream.LotteryCreate(Mode, 0, PostionItem);
                                stream.AddItemsLottery(GateID, RanksItem, JadesAddeds, ItemID);
                                user.Send(stream.LotteryFinalize());
                            }
                        }
                        break;
                    }
                case Action.AddJade:
                    {
                        //   if (System.DateTime.Now >= user.Player.LotteryTime.AddSeconds(1))
                        {
                            if (user.Inventory.Contain(LottryItem, 1) && user.Player.AddJade > 0) // for anti-proxy
                            {
                                user.Activeness.IncreaseTask(4);
                                user.Activeness.IncreaseTask(16);
                                user.Activeness.IncreaseTask(28);
                                user.Inventory.Remove(LottryItem, 1, stream);
                                user.Player.AddJade--;
                                user.Player.PlayOne = true;
                                if (LottryItem == 711504)
                                {
                                    user.Player.OldGate = true;
                                }
                                if (LottryItem == 712019)
                                {
                                    user.Player.NewGate = true;
                                }
                                user.Player.LotteryItem = new MsgGameItem[1];
                                var GenerateLotteryPrize = Pool.Lottery.GenerateLotteryPrize((byte)GateID, user);
                                user.Player.LotteryItem[0] = Pool.Lottery.CreateGameItem(GenerateLotteryPrize[0]);
                                stream.LotteryCreate(Action.Show, 1, PostionItem);
                                stream.AddItemsLottery(GateID, (byte)GenerateLotteryPrize[1], user.Player.AddJade, GenerateLotteryPrize[0]);
                                user.Send(stream.LotteryFinalize());
                                if (!Pool.Lottery.NULLGETITEMS.Contains(GenerateLotteryPrize[0]))
                                    MsgBigLottery.AddWinner(new Pool.LvLottery
                                    {
                                        ItemID = GenerateLotteryPrize[0],
                                        GateID = GateID,
                                        RanksItem = GenerateLotteryPrize[1],
                                        Name = user.Player.Name
                                    });
                                user.Player.LotteryTime = System.DateTime.Now;
                            }
                        }
                        // else
                        {
                            // user.CreateBoxDialog("Please Wait 1 Second Before Changing The Prize");
                        }
                        break;
                    }
                case Action.Continue:
                    {
                        if (user.Player.LotteryEntries < Pool.Lottery.LotteryEntry(user.Player.VipLevel))
                        {
                            if (user.Inventory.Contain(LottryItem, 3))
                            {
                                user.Activeness.IncreaseTask(4);
                                user.Activeness.IncreaseTask(16);
                                user.Activeness.IncreaseTask(28);
                                user.Inventory.Remove(LottryItem, 3, stream);
                                user.Player.LotteryEntries++;
                                user.Player.GateID = GateID;
                                if (GateID == 11) user.Player.PlayLunar = true;
                                //reset lottery jade
                                user.Player.AddJade = 3;
                                user.Player.PlayOne = true;
                                if (LottryItem == 711504)
                                {
                                    user.Player.OldGate = true;
                                }
                                if (LottryItem == 712019)
                                {
                                    user.Player.NewGate = true;
                                }
                                user.Player.LotteryItem = new MsgGameItem[1];
                                var GenerateLotteryPrize = Pool.Lottery.GenerateLotteryPrize((byte)GateID, user);
                                user.Player.LotteryItem[0] = Pool.Lottery.CreateGameItem(GenerateLotteryPrize[0]);
                                stream.LotteryCreate(Action.Show, 1, PostionItem);
                                stream.AddItemsLottery(GateID, (byte)GenerateLotteryPrize[1], user.Player.AddJade, GenerateLotteryPrize[0]);
                                user.Send(stream.LotteryFinalize());
                                if (!Pool.Lottery.NULLGETITEMS.Contains(GenerateLotteryPrize[0]))
                                {
                                    MsgBigLottery.AddWinner(new Pool.LvLottery
                                    {
                                        ItemID = GenerateLotteryPrize[0],
                                        GateID = GateID,
                                        RanksItem = GenerateLotteryPrize[1],
                                        Name = user.Player.Name
                                    });
                                    var Info = new MsgBigLottery.MsgBigLotteryProto();
                                    Info.Type = 0;
                                    Info.Items = MsgBigLottery.Board();
                                    user.Send(stream.CreateBigLottery(Info));
                                }
                            }
                            else
                            {
                                user.SendSysMesage("You need to pay 3 Small Lottery Tickets to draw from the lottery in the lottery Center.");
                            }
                        }
                        else
                        {
                            user.SendSysMesage("Sorry, your LotteryEntries has finished come tomorrow.");
                        }
                        break;
                    }
                default:
                    {
                        MyConsole.WriteLine("MsgLottery not found ->" + Mode, System.ConsoleColor.DarkRed);
                        break;
                    }
            }
        }
    }
}