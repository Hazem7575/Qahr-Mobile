using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet RouletteSignUpCreate(this ServerSockets.Packet stream, MsgRouletteSignUp.ActionJoin Action,uint UID)
        {
            stream.InitWriter();

            stream.Write((byte)Action);
            stream.Write(UID);

            stream.Finalize(GamePackets.MsgRoulette1ArgAction);
            return stream;
        }

        public static void GetRouletteSignUp(this ServerSockets.Packet stream, out MsgRouletteSignUp.ActionJoin Action, out uint UID)
        {
            Action = (MsgRouletteSignUp.ActionJoin)stream.ReadUInt8();
            UID = stream.ReadUInt32();

        }
    }

    public unsafe struct MsgRouletteSignUp
    {
        public enum ActionJoin : byte
        {
            Join = 0,
            Watch = 4,
            InfoTable = 5,//?????
            Quit = 7
        }

        [PacketAttribute(GamePackets.MsgRoulette1ArgAction)]
        private static void Poroces(Client.GameClient user, ServerSockets.Packet stream)
        {
            MsgRouletteSignUp.ActionJoin Typ; 
            uint UID;

            stream.GetRouletteSignUp(out Typ, out UID);


            switch (Typ)
            {
                case ActionJoin.InfoTable:
                    {
                         Database.Roulettes.RouletteTable Table;
                         if (Database.Roulettes.RoulettesPoll.TryGetValue(UID, out Table))
                         {
                             Table.SpawnPacket.PlayersCount = (byte)Table.RegistredPlayers.Count;
                             user.Send(stream.RouletteTableCreate(Table.SpawnPacket.UID, Table.SpawnPacket.TableNumber, Table.SpawnPacket.MoneyType
                                 , Table.SpawnPacket.X, Table.SpawnPacket.Y, Table.SpawnPacket.Mesh, Table.SpawnPacket.PlayersCount));
                         }
                        break;
                    }
                case ActionJoin.Join:
                    {
                        Database.Roulettes.RouletteTable Table;
                        if (Database.Roulettes.RoulettesPoll.TryGetValue(UID, out Table))
                        {
                            switch (Table.SpawnPacket.MoneyType)
                            {
                                case MsgRouletteTable.TableType.ConquerPoints:
                                    {
                                        if (user.Player.ConquerPoints >= 5)
                                        {
                                            if (user.InTrade) return;
                                            Table.AddPlayer(stream, user);
                                        }
                                        else
                                        {
#if Arabic
                                              user.SendSysMesage("Sorry, you should have at least 5 CPs to join in the Roulette.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
#else
                                            user.SendSysMesage("Sorry, you should have at least 5 CPs to join in the Roulette.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
#endif
                                          
                                        }
                                        break;
                                    }
                                case MsgRouletteTable.TableType.Money:
                                    {
                                        if (user.Player.Money >= 5000)
                                        {
                                            Table.AddPlayer(stream, user);
                                        }
                                        else
                                        {
#if Arabic
                                             user.SendSysMesage("Sorry, you should have at least 5,000 Silvers to join in the Roulette.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
#else
                                            user.SendSysMesage("Sorry, you should have at least 5,000 Silvers to join in the Roulette.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
#endif
                                           
                                        }
                                        break;
                                    }
                            }
                        }
                        break;
                    }
                case ActionJoin.Watch:
                    {
                        Database.Roulettes.RouletteTable Table;
                        if (Database.Roulettes.RoulettesPoll.TryGetValue(UID, out Table))
                        {
                            Table.AddWatch(stream,user);
                        }
                        break;
                    }
            }
        }
    }
}
