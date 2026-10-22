using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using System.IO;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class MsgSpiritInteractive
    {
        [ProtoContract]
        public class MsgSpiritInteractiveProto
        {
            [ProtoMember(1, IsRequired = true)]
            public byte Type;
            [ProtoMember(2)]
            public uint[] Items;
        }
        public static unsafe ServerSockets.Packet CreateSpiritInteractive(this ServerSockets.Packet stream, MsgSpiritInteractiveProto obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgSpiritInteractive);

            return stream;
        }
        public static unsafe void GetSpiritInteractive(this ServerSockets.Packet stream, out MsgSpiritInteractiveProto pQuery)
        {
            pQuery = new MsgSpiritInteractiveProto();
            pQuery = stream.ProtoBufferDeserialize<MsgSpiritInteractiveProto>(pQuery);
        }
        [PacketAttribute(GamePackets.MsgSpiritInteractive)]
        private unsafe static void Process(Client.GameClient client, ServerSockets.Packet stream)
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
            MsgSpiritInteractiveProto Info;
            stream.GetSpiritInteractive(out Info);
            MsgGameItem item;
            if (!client.Player.TREPIN2 && client.Player.CheckPin())
            {
                client.Player.MessageBox("You must first activate your security code from [Anima Protection].", null, null, 60);
                return;
            }
            if (Info.Type == 3)//Upgrade
            {
                var ITEM_ID = Info.Items[0];
                //if (client.Inventory.Contain(Info.Items[0], out item))
                {
                    if (client.Inventory.Contain(ITEM_ID, 2) && ITEM_ID % 100 < 18)
                    {
                        if (!(ITEM_ID >= 4200001 && ITEM_ID <= 4200018))
                        {
                            client.Socket.Disconnect();
                            Database.SystemBannedAccount.AddBan(client.Player.UID, client.Player.Name, 999999, "Your Account Has Been Banned For Using Hack.");
                            MyConsole.WriteLine("Player [ " + client.Player.Name + " ] Banned To [Bug Anima] .", ConsoleColor.White);
                            return;
                        }
                        if (ITEM_ID == 4200018)
                        {
                            client.CreateBoxDialog("Anima Upgrade Is Closed At (P18).");
                            return;
                        }
                        if (ITEM_ID >= 4200001 && ITEM_ID <= 4200005 && Role.Core.Rate(40)
                                 || ITEM_ID == 4200006 && Role.Core.Rate(40)
                                 || ITEM_ID == 4200007 && Role.Core.Rate(40)
                                 || ITEM_ID == 4200008 && Role.Core.Rate(40)
                                 || ITEM_ID == 4200009 && Role.Core.Rate(40)
                                 || ITEM_ID == 4200010 && Role.Core.Rate(40)
                                 || ITEM_ID == 4200011 && Role.Core.Rate(40)
                                 || ITEM_ID == 4200012 && Role.Core.Rate(40)
                                 || ITEM_ID == 4200013 && Role.Core.Rate(100)
                                 || ITEM_ID == 4200014 && Role.Core.Rate(100)
                                 || ITEM_ID == 4200015 && Role.Core.Rate(100)
                                 || ITEM_ID == 4200016 && Role.Core.Rate(100)
                                 || ITEM_ID == 4200017 && Role.Core.Rate(100))
                        {
                            client.Inventory.Remove(ITEM_ID, 2, stream);
                            var Chances = Database.SpiritTable.SpiritRates.Where(z => z.AnimaID == ITEM_ID).ToArray();
                            Chances = Chances.Where(z => z.Type != Database.SpiritTable.Type.Failed).ToArray();
                            Chances = Chances.OrderBy(q => q.NewAnimaID).ToArray();//This is new
                            if (Role.Core.Rate(0))
                            {
                                var r = Pool.GetRandom.Next(0, Chances.Count());
                                Chances = Chances.Where(q => q.Index == Chances.ToArray()[r].Index).ToArray();

                            }

                            client.Inventory.AddItemWitchStack(Chances.FirstOrDefault().NewAnimaID, 0, 1, stream, false);

                            uint index2 = Chances.FirstOrDefault().Index;
                            if (ITEM_ID == 4200013)
                                index2 = 71;
                            if (ITEM_ID == 4200014)
                                index2 = 72;
                            if (ITEM_ID == 4200015)
                                index2 = 73;
                            if (ITEM_ID == 4200016)
                                index2 = 74;
                            if (ITEM_ID == 4200017)
                                index2 = 75;
                            Info.Items = new uint[3] { Info.Items[0], Chances.FirstOrDefault().NewAnimaID, index2 };
                            foreach (var user in Pool.GamePoll.Values)
                            {
                                user.Send(new MsgMessage(Program.ServerConfig.ServerName, client.Player.Name, "<STR_ITEM_TYPE_" + ITEM_ID + "@@>@@<STR_ITEM_TYPE_" + Chances.FirstOrDefault().NewAnimaID + "@@>@@FF03FA01 </F>NpcPosition_PathFind</N>23735@@", 3).GetArray(stream));

                            }
                        }
                        else
                        {
                            var Chances = Database.SpiritTable.SpiritRates.Where(z => z.AnimaID == ITEM_ID).ToArray();
                            Chances = Chances.Where(q => q.Type == Database.SpiritTable.Type.Failed).ToArray();
                            if (Chances.FirstOrDefault().IsItem && Role.MyMath.Success(50))//Lost 1
                            {
                                client.Inventory.Remove(ITEM_ID, 1, stream);
                               // client.GainExpBall(500, false, Role.Flags.ExperienceEffect.angelwing);
                            }
                            else
                            {
                                client.Inventory.Remove(ITEM_ID, 2, stream);
                               // client.GainExpBall(2000, false, Role.Flags.ExperienceEffect.angelwing);
                            }
                            uint index = Chances.FirstOrDefault().Index;
                            if (ITEM_ID >= 4200013)
                                index = 60;
                            Info.Items = new uint[3] { Info.Items[0], 0 , index };
                        }
                    }

                }
            }
            else if (Info.Type == 1)//Disassemble
            {
                for (int i = 0; i < Info.Items.Length; i++)
                {
                    if (client.Inventory.TryGetItem(Info.Items[i], out item))
                    {

                        if (!(item.ITEM_ID >= 4200001 && item.ITEM_ID <= 4200018))
                        {
                            client.Socket.Disconnect();
                            Database.SystemBannedAccount.AddBan(client.Player.UID, client.Player.Name, 999999, "Your Account Has Been Banned For Using Hack.");
                            MyConsole.WriteLine("Player [ " + client.Player.Name + " ] Banned To [Bug Anima] .", ConsoleColor.White);
                            return;
                        }
                        if (client.Inventory.AddItemWitchStack((uint)(item.ITEM_ID - 1), 0, (byte)(item.StackSize * 2), stream, item.Bound > 0) && client.Inventory.RemoveStackItem(item.UID, item.StackSize, stream))
                            Info.Items[i] = 1;
                        else
                        {
                            Info.Items[i] = 0;
                            client.SendSysMesage("Please empty some space in your inventory.");
                        }

                    }
                }
            }
            client.Send(stream.CreateSpiritInteractive(Info));
        }
    }
}
