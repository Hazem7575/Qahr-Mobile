using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.Concurrent;
using Qahr.Mobile.Game.MsgServer;
using System.Security.Cryptography;

namespace Qahr.Mobile.Database
{

    public class Roulettes
    {
        public class NumberInformation
        {
            //Number 37 is 00
            //Number 0 is 0
            public List<byte> Red = new List<byte>() { 1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36 };
            public List<byte> Black = new List<byte>() { 2, 4, 6, 8, 10, 11, 13, 15, 17, 20, 22, 24, 26, 28, 29, 31, 33, 35 };
            public List<byte> Line1 = new List<byte>() { 1, 4, 7, 10, 13, 16, 19, 22, 25, 28, 31, 34 };
            public List<byte> Line2 = new List<byte>() { 2, 5, 8, 11, 14, 17, 20, 23, 26, 29, 32, 35 };
            public List<byte> Line3 = new List<byte>() { 3, 6, 9, 12, 15, 18, 21, 24, 27, 30, 33, 36 };

            public bool IsOdd(byte Number)
            {
                if (Number == 0 || Number >= 37)
                    return false;
                return Number % 2 != 0;
            }
            public bool IWorld(byte Number)
            {
                if (Number == 0 || Number >= 37)
                    return false;
                return Number % 2 == 0;
            }
            public bool IsSmall(byte Number)
            {
                return Number >= 1 && Number <= 18;
            }
            public bool IsBig(byte Number)
            {
                return Number >= 19 && Number <= 36;
            }
            public bool IsFront(byte Number)
            {
                return Number >= 1 && Number <= 12;
            }
            public bool IsMiddle(byte Number)
            {
                return Number >= 13 && Number <= 24;
            }
            public bool IsBack(byte Number)
            {
                return Number >= 25 && Number <= 36;
            }
        }

        public unsafe class RouletteTable : NumberInformation
        {
            public Random Rand;
            public const uint MapID = 3053, MaxPlayerOnTable = 5, MaxTimerStamp = 30;


            public DateTime Stamp = DateTime.Now.AddMilliseconds(1000);

            public Extensions.Time32 TimerStamp = new Extensions.Time32();
            public int GetTimerStamp
            {
                get
                {
                    int value = TimerStamp.AllSeconds - Extensions.Time32.Now.AllSeconds;
                    if (value < 0)
                        return 0;
                    else
                        return value;
                }
            }
            public Extensions.Time32 StampRound = new Extensions.Time32();
            public byte LuckyNumber = 1;

            public class Member
            {
                public Client.GameClient Owner;
                public uint MyCost
                {
                    get
                    {
                        long Cost = 0;
                        Cost += MyLuckNumber.Values.Sum(p => p.BetPrice);
                        Cost += MyLuckExtra.Values.Sum(p => p.BetPrice);
                        return (uint)Cost;
                    }

                }
                public Game.MsgServer.MsgRouletteOpenGui.Color Color;
                public uint Betting = 0;
                public uint Winning = 0;

                public uint smalwin = 0;
                public uint Noumber = 0;
                public ConcurrentDictionary<byte, Game.MsgServer.MsgRouletteCheck.Item> MyLuckNumber = new ConcurrentDictionary<byte, Game.MsgServer.MsgRouletteCheck.Item>();
                public ConcurrentDictionary<byte, Game.MsgServer.MsgRouletteCheck.Item> MyLuckExtra = new ConcurrentDictionary<byte, Game.MsgServer.MsgRouletteCheck.Item>();

                public void ShareBetting(RouletteTable table)
                {

                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        table.SendPacketTable(stream.RouletteShareBettingCreate(this, Color));

                    }
                }
            }
            public ConcurrentDictionary<uint, Member> RegistredPlayers;
            public ConcurrentDictionary<uint, Client.GameClient> ClientsWatch;
            public Game.MsgServer.MsgRouletteTable SpawnPacket;



            public RouletteTable()
            {
                Rand = new System.Random();
                TimerStamp = Extensions.Time32.Now.AddSeconds((int)MaxTimerStamp);
                ClientsWatch = new ConcurrentDictionary<uint, Client.GameClient>();
                RegistredPlayers = new ConcurrentDictionary<uint, Member>();
                SpawnPacket = new MsgRouletteTable();
            }
            public void AddWatch(ServerSockets.Packet stream, Client.GameClient client)
            {
                if (RegistredPlayers.Count == 0)
                {
                    client.SendSysMesage("Failed to spectate. No one is at the table.", Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.red);

                    return;
                }
                if (client.PlayRouletteUID != 0)
                {
                    client.SendSysMesage("You are play another table. Please quit before you spectate this one.", Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.red);
                    return;
                }
                if (client.WatchRoulette != 0)
                {
                    client.SendSysMesage("You are spectating another table. Please quit before you spectate this one.", Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.red);
                    return;
                }
                if (!ClientsWatch.ContainsKey(client.Player.UID))
                {
                    client.WatchRoulette = SpawnPacket.UID;
                    ClientsWatch.TryAdd(client.Player.UID, client);


                    SendPacketTable(stream.RouletteScreenCreate(client.Player.UID));


                    client.Send(stream.RouletteOpenGuiCreate(SpawnPacket.MoneyType, Game.MsgServer.MsgRouletteOpenGui.Color.Watch
                        , (byte)GetTimerStamp, RegistredPlayers.Values.Where(p => p.Owner.Player.UID != client.Player.UID).ToArray()));

                }
            }
            public void RemoveWatch(uint UID)
            {
                Client.GameClient client;
                if (ClientsWatch.TryRemove(UID, out client))
                {
                    client.WatchRoulette = 0;

                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();

                        client.Send(stream.RouletteSignUpCreate(MsgRouletteSignUp.ActionJoin.Quit, client.Player.UID));
                    }
                }
            }
            public unsafe void AddPlayer(ServerSockets.Packet stream, Client.GameClient client)
            {
                if (RegistredPlayers.Count >= MaxPlayerOnTable)
                {
                    client.SendSysMesage("Sorry, this game table is full.", Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.red);
                    return;
                }
                if (client.PlayRouletteUID != 0)
                {
                    client.SendSysMesage("You are play another table. Please quit before you playing this one.", Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.red);
                    return;
                }
                if (!RegistredPlayers.ContainsKey(client.Player.UID))
                {
                    client.PlayRouletteUID = SpawnPacket.UID;

                    Member player = new Member();
                    player.Owner = client;
                    GeneratePlayerColor(out player.Color);
                    RegistredPlayers.TryAdd(player.Owner.Player.UID, player);

                    ApplyTime(stream, player);

                    if (RegistredPlayers.Count > 1)
                    {
                        ShareTableBetting(stream, client);
                        foreach (var user in RegistredPlayers.Values)
                        {
                            if (user.Owner.Player.UID != client.Player.UID)
                            {
                                user.Owner.Send(stream.RoulettedAddNewPlayerCreate(client.Player.UID, client.Player.Mesh, player.Color, client.Player.Name));
                            }
                        }
                        foreach (var user in ClientsWatch.Values)
                            user.Send(stream.RoulettedAddNewPlayerCreate(client.Player.UID, client.Player.Mesh, player.Color, client.Player.Name));
                    }
                }
            }
            public void ShareTableBetting(ServerSockets.Packet stream, Client.GameClient client)
            {
                foreach (var use in RegistredPlayers.Values)
                {
                    if (use.MyLuckExtra.Count != 0 || use.MyLuckNumber.Count != 0)
                        client.Send(stream.RouletteShareBettingCreate(use, use.Color));
                }
            }
            public void RemovePlayer(Client.GameClient client)
            {

                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    SendPacketTable(stream.RouletteSignUpCreate(MsgRouletteSignUp.ActionJoin.Quit, client.Player.UID));
                }


                Member player;
                if (RegistredPlayers.TryRemove(client.Player.UID, out player))
                {
                    client.PlayRouletteUID = 0;

                    if (RegistredPlayers.Count == 0)
                    {
                        foreach (var user in ClientsWatch.Values)
                            RemoveWatch(user.Player.UID);
                    }
                }
            }
            public unsafe void SendPacketTable(ServerSockets.Packet packet)
            {
                foreach (var player in RegistredPlayers.Values)
                    player.Owner.Send(packet);
                foreach (var user in ClientsWatch.Values)
                    user.Send(packet);
            }
            public bool ContainColor(Game.MsgServer.MsgRouletteOpenGui.Color color)
            {
                foreach (var player in RegistredPlayers.Values)
                {
                    if (player.Color == color)
                        return true;
                }
                return false;
            }
            public void GeneratePlayerColor(out Game.MsgServer.MsgRouletteOpenGui.Color color)
            {
                color = Game.MsgServer.MsgRouletteOpenGui.Color.None;
                for (int x = 0; x < 5; x++)
                {
                    if (!ContainColor((Game.MsgServer.MsgRouletteOpenGui.Color)x))
                    {
                        color = (Game.MsgServer.MsgRouletteOpenGui.Color)x;
                        break;
                    }
                }
            }
            public void ApplyTime(ServerSockets.Packet stream, Member player)
            {
                if (RegistredPlayers.Count == 1)
                    player.Owner.Send(stream.RouletteOpenGuiCreate(SpawnPacket.MoneyType, player.Color, (byte)GetTimerStamp, new Member[0]));
                else
                    player.Owner.Send(stream.RouletteOpenGuiCreate(SpawnPacket.MoneyType, player.Color, (byte)GetTimerStamp, RegistredPlayers.Values.Where(p => p.Owner.Player.UID != player.Owner.Player.UID).ToArray()));
            }
            public void ApplayNumberWinner(Client.GameClient client)
            {

                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    client.Send(stream.RouletteNoWinnerCreate((byte)LuckyNumber));
                }
            }
            private bool Reset = false;

            public void work(DateTime clock)
            {
                if (clock > Stamp)
                {
                    Extensions.Time32 TimerNow = Extensions.Time32.Now;
                    if (RegistredPlayers.Count > 0)
                    {

                        if (TimerNow > TimerStamp)
                        {
                            if (TimerNow > StampRound && Reset == true)
                            {
                                ResetRoulette();
                            }
                            else if (!Reset)
                            {
                                //create Numbers lucky;
                                GenerateLuckyNumber();

                                Reset = true;
                                StampRound = TimerNow.AddSeconds(3);
                                foreach (var player in RegistredPlayers.Values)
                                {
                                    ApplayNumberWinner(player.Owner);
                                    GetRewrad(player, true);
                                }
                                foreach (var user in ClientsWatch.Values)
                                {
                                    ApplayNumberWinner(user);
                                }
                            }
                        }
                    }
                    else
                    {
                        ResetRoulette();
                    }

                    Stamp = DateTime.Now.AddMilliseconds(1000);
                }
            }
            public void ResetRoulette()
            {
                foreach (var player in RegistredPlayers.Values)
                {
                    player.MyLuckExtra.Clear();
                    player.MyLuckNumber.Clear();
                }
                Reset = false;
                TimerStamp = Extensions.Time32.Now.AddSeconds((int)MaxTimerStamp);
            }
            public void CheckUpMember(Member player)
            {
                switch (SpawnPacket.MoneyType)
                {
                    case Game.MsgServer.MsgRouletteTable.TableType.ConquerPoints:
                        {
                            uint Cost = player.MyCost;
                            if (player.Owner.InTrade) return;
                            int Amount = (int)Cost;
                            if (Amount < 0)
                                return;
                            if (player.Owner.Player.ConquerPoints >= Cost)
                            {
                                player.Owner.Player.ConquerPoints += (uint)-Cost;

                            }
                            else
                            {
                                player.Owner.CreateBoxDialog("You do not have " + Cost.ToString() + " ConquerPoints with you.");
                                RegistredPlayers.TryRemove(player.Owner.Player.UID, out player);
                                player.Owner.PlayRouletteUID = 0;
                            }
                            break;
                        }
                    case Game.MsgServer.MsgRouletteTable.TableType.Money:
                        {
                            //if (player.Owner.PokerPlayer != null)
                            //    return;
                            uint Cost = player.MyCost;
                            if (Cost < 0)
                                return;
                            if (player.Owner.Player.Money >= Cost)
                            {
                                player.Owner.Player.Money -= (ulong)Cost;
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    player.Owner.Player.SendUpdate(stream, player.Owner.Player.Money, Game.MsgServer.MsgUpdate.DataType.Money);
                                }
                            }
                            else
                            {
                                player.Owner.CreateBoxDialog("You do not have " + Cost.ToString() + " silvers with you.");
                                RegistredPlayers.TryRemove(player.Owner.Player.UID, out player);
                                player.Owner.PlayRouletteUID = 0;
                            }
                            break;
                        }
                }
            }
            public bool Rate(int value)
            {
                return value > Rand.Next(0, 100);
            }





            public void GenerateLuckyNumber()
            {

                var Array = RegistredPlayers.Values.ToArray();
                foreach (var user in Array)
                    user.smalwin = 0;

                LuckyNumber = (byte)Pool.GetRandom.Next(0, 38);

               
            }

            public bool CountainNo(byte noumber)
            {
                var Array = RegistredPlayers.Values.ToArray();
                bool contain = false;
                foreach (var player in Array)
                {
                    if (player.MyLuckNumber.ContainsKey(noumber))
                    {
                        contain = true;
                    }
                    if (CheckLines(player))
                        contain = true;
                }
                return contain;
            }
            public bool CheckLines(Member amember)
            {
                foreach (var item in amember.MyLuckExtra.Values)
                {
                    switch (item.Number)
                    {
                        case 152:
                            {
                                if (Black.Contains(LuckyNumber))
                                {
                                    return true;
                                }
                                break;
                            }
                        case 151:
                            {
                                if (Red.Contains(LuckyNumber))
                                {
                                    return true;
                                }
                                break;
                            }
                        case 112:
                            {
                                if (IsOdd(LuckyNumber))
                                {
                                    return true;
                                }
                                break;
                            }
                        case 111:
                            {
                                if (IWorld(LuckyNumber))
                                {
                                    return true;
                                }
                                break;
                            }
                        case 141:
                            {
                                if (IsSmall(LuckyNumber))
                                {
                                    return true;
                                }
                                break;
                            }
                        case 142:
                            {
                                if (IsBig(LuckyNumber))
                                {
                                    return true;
                                }
                                break;
                            }
                        case 121:
                            {
                                if (IsFront(LuckyNumber))
                                {
                                    return true;
                                }
                                break;
                            }
                        case 122:
                            {
                                if (IsMiddle(LuckyNumber))
                                {
                                    return true;
                                }
                                break;
                            }
                        case 123:
                            {
                                if (IsBack(LuckyNumber))
                                {
                                    return true;
                                }
                                break;
                            }
                        case 131:
                            {
                                if (Line1.Contains(LuckyNumber))
                                {
                                    return true;
                                }
                                break;
                            }
                        case 132:
                            {
                                if (Line2.Contains(LuckyNumber))
                                {
                                    return true;
                                }
                                break;
                            }
                        case 133:
                            {
                                if (Line3.Contains(LuckyNumber))
                                {
                                    return true;
                                }
                                break;
                            }
                    }
                }
                return false;
            }
            public void GetRewrad(Member Member, bool Update = false)
            {
                Member.Winning = 0;
                Member.Betting = Member.MyCost;

                foreach (var item in Member.MyLuckNumber.Values)
                {
                    if (item.Number == LuckyNumber)
                    {
                        Member.Winning += (uint)(item.BetPrice * 36);
                      
                    }
                }
                foreach (var item in Member.MyLuckExtra.Values)
                {
                    switch (item.Number)
                    {
                        case 152:
                            {
                                if (Black.Contains(LuckyNumber))
                                {
                                    Member.Winning += item.BetPrice * 2;
                                    // Member.Betting += item.BetPrice;
                                }
                                break;
                            }
                        case 151:
                            {
                                if (Red.Contains(LuckyNumber))
                                {
                                    Member.Winning += item.BetPrice * 2;
                                    // Member.Betting += item.BetPrice;
                                }
                                break;
                            }
                        case 112:
                            {
                                if (IsOdd(LuckyNumber))
                                {
                                    Member.Winning += item.BetPrice * 2;
                                    // Member.Betting += item.BetPrice;
                                }
                                break;
                            }
                        case 111:
                            {
                                if (IWorld(LuckyNumber))
                                {
                                    Member.Winning += item.BetPrice * 2;
                                    // Member.Betting += item.BetPrice;
                                }
                                break;
                            }
                        case 141:
                            {
                                if (IsSmall(LuckyNumber))
                                {
                                    Member.Winning += item.BetPrice * 2;
                                    // Member.Betting += item.BetPrice;
                                }
                                break;
                            }
                        case 142:
                            {
                                if (IsBig(LuckyNumber))
                                {
                                    Member.Winning += item.BetPrice * 2;
                                    // Member.Betting += item.BetPrice;
                                }
                                break;
                            }
                        case 121:
                            {
                                if (IsFront(LuckyNumber))
                                {
                                    Member.Winning += item.BetPrice * 3;
                                    // Member.Betting += item.BetPrice;
                                }
                                break;
                            }
                        case 122:
                            {
                                if (IsMiddle(LuckyNumber))
                                {
                                    Member.Winning += item.BetPrice * 3;
                                    // Member.Betting += item.BetPrice;
                                }
                                break;
                            }
                        case 123:
                            {
                                if (IsBack(LuckyNumber))
                                {
                                    Member.Winning += item.BetPrice * 3;
                                    // Member.Betting += item.BetPrice;
                                }
                                break;
                            }
                        case 131:
                            {
                                if (Line1.Contains(LuckyNumber))
                                {
                                    Member.Winning += item.BetPrice * 3;
                                    // Member.Betting += item.BetPrice;
                                }
                                break;
                            }
                        case 132:
                            {
                                if (Line2.Contains(LuckyNumber))
                                {
                                    Member.Winning += item.BetPrice * 3;
                                    // Member.Betting += item.BetPrice;
                                }
                                break;
                            }
                        case 133:
                            {
                                if (Line3.Contains(LuckyNumber))
                                {
                                    Member.Winning += item.BetPrice * 3;
                                    // Member.Betting += item.BetPrice;
                                }
                                break;
                            }
                    }
                }
                // foreach (var item in Member.MyLuckNumber.Values)
                {
                    if (Member.Winning != 0)
                    {
                        if (Member.smalwin > Member.Winning || Member.smalwin == 0)
                        {
                            Member.smalwin = Member.Winning;
                            //Console.WriteLine(Member.Winning + " " + LuckyNumber);
                            Member.Noumber = LuckyNumber;
                        }
                    }
                }
                if (Member.Winning > 0 && Update)
                {
                    string logs = "[Roulette] Type: " + SpawnPacket.MoneyType.ToString() + " Name: " + Member.Owner.Player.Name + " Winning: ";
                    logs += Member.Winning;

                    switch (SpawnPacket.MoneyType)
                    {
                        case Game.MsgServer.MsgRouletteTable.TableType.Money:
                            {
                                //if (Member.Owner.PokerPlayer != null)
                                //    return;
                                Member.Owner.Player.Money += (ulong)Member.Winning;
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    Member.Owner.Player.SendUpdate(stream, Member.Owner.Player.Money, Game.MsgServer.MsgUpdate.DataType.Money);
                                }
                                break;
                            }
                        case Game.MsgServer.MsgRouletteTable.TableType.ConquerPoints:
                            {

                                Member.Owner.Player.ConquerPoints += (uint)Member.Winning;

                                break;
                            }
                    }
                    Database.ServerDatabase.LoginQueue.Enqueue(logs);
                }
            }
        }
        public static Dictionary<uint, RouletteTable> RoulettesPoll = new Dictionary<uint, RouletteTable>();

        internal static void Load()
        {
            string[] baseText = File.ReadAllLines(Program.ServerConfig.DbLocation + "Roulettes.txt");
            if (baseText.Length <= 1)
                return;
            for (int x = 0; x < baseText.Length; x++)
            {
                if (baseText[x].StartsWith(";")) continue;
                Database.DBActions.ReadLine line = new DBActions.ReadLine(baseText[x], ',');
                RouletteTable Roulette = new RouletteTable();
                Roulette.SpawnPacket.UID = line.Read((uint)0);
                Roulette.SpawnPacket.TableNumber = line.Read((uint)0);
                Roulette.SpawnPacket.MoneyType = (Game.MsgServer.MsgRouletteTable.TableType)line.Read((uint)0);
                ushort MapID = line.Read((ushort)0);
                Roulette.SpawnPacket.X = line.Read((ushort)0);
                Roulette.SpawnPacket.Y = line.Read((ushort)0);
                Roulette.SpawnPacket.Mesh = line.Read((ushort)0);
                if (!RoulettesPoll.ContainsKey(Roulette.SpawnPacket.UID))
                    RoulettesPoll.Add(Roulette.SpawnPacket.UID, Roulette);
            }

        }
    }
}
