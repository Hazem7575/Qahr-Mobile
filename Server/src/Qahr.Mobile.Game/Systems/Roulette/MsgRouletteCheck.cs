using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe struct MsgRouletteCheck
    {
        public class Item
        {
            public byte Number;
            public uint BetPrice;
        }

        [PacketAttribute(GamePackets.MsgRoulettePlayerBet)]
        private static void Poroces(Client.GameClient user, ServerSockets.Packet stream)
        {
            byte Count = stream.ReadUInt8();


            if (user.PlayRouletteUID != 0)
            {
                Database.Roulettes.RouletteTable Table;
                if (Database.Roulettes.RoulettesPoll.TryGetValue(user.PlayRouletteUID, out Table))
                {
                    Database.Roulettes.RouletteTable.Member player;
                    if (Table.RegistredPlayers.TryGetValue(user.Player.UID, out player))
                    {
                        if (player.MyLuckNumber.Count != 0)
                            return;
                        if (player.MyLuckExtra.Count != 0)
                            return;
                        for (int x = 0; x < Count; x++)
                        {
                            var element = new Item();
                            element.Number = stream.ReadUInt8();
                            element.BetPrice = stream.ReadUInt32();
                            switch (Table.SpawnPacket.MoneyType)
                            {
                                case MsgRouletteTable.TableType.Money:
                                    {
                                        if (player.Owner.Player.Money < element.BetPrice)
                                            continue;
                                        else
                                            player.Owner.Player.Money -= element.BetPrice;
                                        using (var rec = new ServerSockets.RecycledPacket())
                                        {
                                            var stream2 = rec.GetStream();

                                            player.Owner.Player.SendUpdate(stream2,player.Owner.Player.Money, MsgUpdate.DataType.Money, false);
                                        }
                                        break;
                                    }
                                case MsgRouletteTable.TableType.ConquerPoints:
                                    {
                                        if (player.Owner.InTrade) return;
                                        if (player.Owner.Player.ConquerPoints < element.BetPrice)
                                            continue;
                                        else
                                            player.Owner.Player.ConquerPoints -= (uint)element.BetPrice;

                                        break;
                                    }
                            }
                            if (element.Number >= 0 && element.Number <= 37)
                            {
                                if (!player.MyLuckNumber.ContainsKey(element.Number))
                                {
                                    player.MyLuckNumber.TryAdd(element.Number, element);
                                }
                            }
                            else
                            {
                                if (!player.MyLuckExtra.ContainsKey(element.Number))
                                {
                                    player.MyLuckExtra.TryAdd(element.Number, element);
                                }
                            }
                        }

                        player.ShareBetting(Table);
                    }
                }
            }
        }
    }
}
