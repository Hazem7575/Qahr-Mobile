using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.Game.MsgTournaments
{
    public class MsgSteedRace
    {
        public class UsableRacePotion
        {
            public MsgRacePotion.RaceItemType Type;
            public int Count;
        }

        public enum Maps : uint
        {
            MarketRace = 1950, // Start: 88, 149 End: 420, 431
            IceRace = 2064, //Start: 175,250 End: 200,170
            DungeonRace = 2063, //Start: 450, 520 End: 690,450
            LavaRace = 2062, // Start: 150,350 End:320,160
            IslandRace = 2061, // Start: 60,400 End: 870,830

        }

        public bool InSteedRace(uint mapid)
        {
            return mapid == (ushort)Maps.MarketRace || mapid == (ushort)Maps.IceRace || mapid == (ushort)Maps.DungeonRace || mapid == (ushort)Maps.LavaRace || mapid == (ushort)Maps.IslandRace;
        }
        public uint[][] RaceSettings = new[]
        {
            new uint[]{ (uint)Maps.MarketRace, 88, 149, 
                420, 431, 4,  
                65, 174, 621,
                123, 243, 60, 
                214, 334, 70, 
                346, 459, 100},
            new uint[]{ (uint)Maps.IceRace, 175, 250, 
                200, 153, 6, //255, 535*
                154, 267, 621,
                146, 392, 70,
                283, 351, 100,
                295, 079, 100},
            new uint[]{ (uint)Maps.IslandRace, 60, 400, 
                899, 816, 10,
                96, 392, 621,
                220, 234, 200,
                472, 160, 200,
                777, 464, 300},
            new uint[]{ (uint)Maps.DungeonRace, 450, 520,
                682, 484 , 10,
                435, 559, 621,
                471, 759, 200,
                714, 598, 250,
                489, 679, 20},
            new uint[]{ (uint)Maps.LavaRace, 150, 350,
                330, 170, 6,
                101, 397, 623,
                327, 553, 100,
                526, 477, 200,
                283, 275, 100}
        };
        public static ushort MAPID = 1950;
        public static uint[] Settings;
        public static uint RaceRecord;

        private Role.GameMap Map;
        private bool isOn, InvitationsOut, InvitationsExpired, FiveSecondsLeft, GateOpen;
        private DateTime InvitationsSentOut, InvitationsExpireDate, Last5Seconds, GateOpened;

        public int SecondsLeftUntilStart { get { return (int)(InvitationsSentOut.AddMinutes(1) - DateTime.Now).TotalSeconds - 5; } }
        public bool CanJoin { get { return isOn && !GateOpen; } }
        public bool IsOn { get { return isOn; } }
        private Role.SobNpc Gate;
        private ushort GateSetX, GateSetY;
        public ushort GateX { get { return GateSetX; } }
        public ushort GateY { get { return GateSetY; } }

        private volatile int Records;

        public void Create()
        {
            while (true)
            {
               // Console.WriteLine("Looking for a race map!");
                int rand = Pool.GetRandom.Next(RaceSettings.Length);
                if (Pool.ServerMaps.ContainsKey((ushort)RaceSettings[rand][0]))
                {
                    Create(RaceSettings[rand][0]);
                    break;
                }
            }
        }
        public void Create(uint mapId)
        {
            int index = -1;
            for (int i = 0; i < RaceSettings.Length; i++)
            {
                if (RaceSettings[i][0] == mapId)
                {
                    index = i;
                    break;
                }
            }
            if (index == -1) return;
            Settings = RaceSettings[index];
            MAPID = (ushort)Settings[0];
           
            Map = Pool.ServerMaps[MAPID];

            var array = Map.View.GetAllMapRoles(Role.MapObjectType.StaticRole);
            foreach (var obj in array)
            {
                Map.RemoveStaticRole(obj);
            }

            Map.View.ClearMap(Role.MapObjectType.SobNpc);

            Gate = new Role.SobNpc();
            Gate.UID = 19501;
            Gate.ObjType = Role.MapObjectType.SobNpc;
            Gate.Map = MAPID;
            Gate.X = (ushort)Settings[6];
            Gate.Y = (ushort)Settings[7];
            Gate.Mesh =  (Role.SobNpc.StaticMesh)Settings[8];
            Gate.Name = " ";
            Gate.Type = Role.Flags.NpcType.Furniture;
            Gate.Sort = 1;
            GateSetX = Gate.X; GateSetY = Gate.Y;
            Map.View.EnterMap<Role.IMapObj>(Gate);
            Map.SetFlagNpc(Gate.X, Gate.Y);

            Init();
        }
        private void Init()
        {
            isOn = InvitationsOut = InvitationsExpired = GateOpen = FiveSecondsLeft = false;
            GeneratePotions();
            Records = 0;
        }

        private void GeneratePotions()
        {
            uint count = 100;
            ushort x, y;
            Tuple<ushort, ushort, int>[] limits = new[]
            {
                new  Tuple<ushort,ushort,int>( (ushort)Settings[9], (ushort)Settings[10], (int)Settings[11] ),
                new  Tuple<ushort,ushort,int>( (ushort)Settings[12], (ushort)Settings[13], (int)Settings[14] ),
                new  Tuple<ushort,ushort,int>( (ushort)Settings[15], (ushort)Settings[16], (int)Settings[17] )
            };

            while (count > 0)
            {
                x = (ushort)Pool.GetRandom.Next(0, Map.bounds.Width);// Kernel.Random.Next(0, Map.Floor.Bounds.Width);
                y = (ushort)Pool.GetRandom.Next(0, Map.bounds.Height);
                bool valid = false;
                foreach (var range in limits)
                    valid |= (Role.Core.GetDistance(x, y, range.Item1, range.Item2) < range.Item3);
                if (valid)
                {
                    if (Map.IsFlagPresent(x,y, Role.MapFlagType.Valid))
                    {
                        bool v = true;
                        // so they wont be anywhere near the bounds
                        // and also there wont be one too near to another
                        for (int i = 0; i < Role.GameMap.XDir.Length; i++)
                            if (Map.IsFlagPresent(x + Role.GameMap.XDir[i], y + Role.GameMap.YDir[i], Role.MapFlagType.Valid) == false)
                                v = false;
                        if (!v) continue;

                        Role.StaticRole role = new Role.StaticRole(x, y, "");
                        role.Map = MAPID;
                        role.Pick();
                        Map.AddStaticRole(role);
                        count--;
                    }
                }
            }
        }

        public void work()
        {
            //2.Horse Racing Monday-Sunday   time: 3:30-4:00  , 11:30-12:00 , 21:30-22:00  PRIZE: 1ST place 1k HP's 2ND 700 HP's 3RD 500 HP's others 300 HP's

            DateTime now = DateTime.Now;
            bool rightHour = (now.Hour == 11 || now.Hour == 15 || now.Hour == 23);//&& (now.DayOfWeek == DayOfWeek.Monday || now.DayOfWeek == DayOfWeek.Sunday);
            if (rightHour && now.Minute < 30)
            {
                if (!InvitationsOut)
                {
                    if (now.Minute == 5)
                    {
                        Create();
                        SendInvitations();
                    }
                }
                else if (!InvitationsExpired)
                {
                    if (now >= InvitationsExpireDate)
                    {
                        InvitationsExpired = true;
                        FiveSecondsLeft = false;
                        Last5Seconds = InvitationsSentOut.AddMinutes(1).AddSeconds(-12);
                    }
                }
                else if (!FiveSecondsLeft)
                {
                    if (now > Last5Seconds)
                    {
                        FiveSecondsLeft = true;
                        SendData(ActionType.BeginSteedRace, uid: 1);
                        Last5Seconds= DateTime.Now.AddSeconds(5);
                    }
                }
                else if (!GateOpen)
                {
                    if (now > Last5Seconds)
                    {
                        OpenGate();
                    }
                }
            }
            else if (rightHour && now.Minute >= 30)
            {
                if (isOn)
                {
                    End();
                }
            }
        }

        private void SendInvitations()
        {
            isOn = true;
            InvitationsOut = true;
            InvitationsExpired = false;
            InvitationsSentOut= DateTime.Now;
            InvitationsExpireDate = InvitationsSentOut.AddSeconds(60);
#if Arabic
              MsgSchedules.SendInvitation("SteedRace", "RacePoints", 286, 147, 1002, 0, 60, MsgStaticMessage.Messages.HorseRace);
#else
            MsgSchedules.SendInvitation("SteedRace", "RacePoints", 386, 471, 1002, 0, 60, MsgStaticMessage.Messages.HorseRace);
#endif

        }

        public unsafe void Join(Client.GameClient client, ServerSockets.Packet stream)
        {
            int seconds = SecondsLeftUntilStart;
            if (seconds > 0)
            {

                ActionQuery action = new ActionQuery();
                action.ObjId = client.Player.UID;
                action.Type = ActionType.CountDown;

                action.dwParam = (uint)seconds;

                client.Send(stream.ActionCreate(action));

            }

            client.Player.AddFlag(MsgUpdate.Flags.Ride, Role.StatusFlagsBigVector32.PermanentFlag, true);

            client.Vigor = client.Status.MaxVigor;

            client.Send(stream.ServerInfoCreate(client.Vigor));

            client.Teleport((ushort)Settings[1], (ushort)Settings[2], MAPID);

            client.Send(stream.RaceRecordCreate(MsgRaceRecord.RaceRecordTypes.BestTime, (int)Map.RecordSteedRace, 1800000, 0, 0, 0));
            client.Send(stream.CreateRecePotion(new MsgRacePotion.RacePotion() {  PotionType = MsgRacePotion.RaceItemType.Null, Amount = 1 }));
            client.Send(stream.CreateRecePotion(new MsgRacePotion.RacePotion() { PotionType = MsgRacePotion.RaceItemType.Null, Amount = 0 }));

            client.Player.RacePotions = new UsableRacePotion[5];
        }
        private unsafe void OpenGate()
        {
            GateOpened = DateTime.Now;
            GateOpen = true;
            Gate.X = 0;
            Gate.Y = 0;

            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                ActionQuery action = new ActionQuery();
                action.ObjId = Gate.UID;
                action.Type = ActionType.RemoveEntity;

                Send(stream.ActionCreate(action));
            }
        }

        private void Send(ServerSockets.Packet stream)
        {
            foreach (var user in Map.Values)
                user.Send(stream);
        }
        private unsafe void SendData(ushort ID, uint value = 0, uint uid = 0)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                ActionQuery action = new ActionQuery();
                action.ObjId = uid;
                action.Type = ID;
                action.dwParam = value;


                foreach (var player in Map.Values)
                {
                    if (uid == 0)
                    {
                        ActionQuery aaction = new ActionQuery();
                        action.ObjId = player.Player.UID;
                        action.Type = ID;
                        action.dwParam = value;

                        player.Send(stream.ActionCreate(aaction));
                    }
                    else
                    {
                        player.Send(stream.ActionCreate(action));
                    }

                }
            }
        }

        private void Status(Client.GameClient client, int rank, int time, int award)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                stream.RaceRecordCreate(MsgRaceRecord.RaceRecordTypes.AddRecord, rank, client.Player.Name, time, award);

                Send(stream.RaceRecordCreate(MsgRaceRecord.RaceRecordTypes.AddRecord, rank, client.Player.Name, time, award));
            }
        }

        private void End()
        {
            InvitationsOut = false;
            Gate.X = GateSetX;
            Gate.Y = GateSetY;
            foreach (var user in Map.Values)
                Exit(user);
            Init();
        }

        public void FinishRace(Client.GameClient client)
        {
            if (Role.Core.GetDistance(client.Player.X, client.Player.Y, (ushort)Settings[3], (ushort)Settings[4]) > 22)
            {
                return;
            }
            else
            {
                if (Records < 5)
                {
                    Records++;
                    int rank = Records;
                    TimeSpan span = DateTime.Now - GateOpened;
                    if (Map.RecordSteedRace > span.TotalMilliseconds)
                    {
                        Map.RecordSteedRace = (uint)span.TotalMilliseconds;
                        Database.ServerDatabase.LoginQueue.TryEnqueue(Map);
                    }

                    int award = AwardPlayer(client, (int)span.TotalMilliseconds, rank);
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        if (client.Inventory.HaveSpace(1))
                            client.Inventory.Add(stream, 723744, 2);
                        else
                            client.Inventory.AddReturnedItem(stream, 723744, 2);

                        client.Player.RacePoints += (uint)award;
                        client.Player.SendUpdate(stream, client.Player.RacePoints, MsgUpdate.DataType.RaceShopPoints);

                        Status(client, rank, (int)span.TotalMilliseconds, award);
                        client.Send(stream.RaceRecordCreate(MsgRaceRecord.RaceRecordTypes.EndTime, (int)rank, (int)span.TotalMilliseconds, award, (int)span.TotalMilliseconds, award));

                    }
                }
                Exit(client);
            }
        }
        public unsafe void CheckForRaceItems(Client.GameClient client)
        {

            if (client.Map.IsFlagPresent(client.Player.X, client.Player.Y, Role.MapFlagType.None))
            {
                foreach (var _item in client.Map.View.Roles(Role.MapObjectType.StaticRole, client.Player.X, client.Player.Y))
                {
                    var item = _item as Role.StaticRole;
                    if (Role.Core.GetDistance(client.Player.X, client.Player.Y, item.X, item.Y) <= 1)
                    {
                        if (item == null) return;
                        if (!item.Viable) return;
                        var type = item.Type; // for super exclamation mark 
                        bool successful = false;
                        if (type == MsgRacePotion.RaceItemType.FrozenTrap && !item.QuestionMark)
                        {
                            if (item.SetBy != client.Player.UID)
                            {
                                client.ApplyRacePotion(type, uint.MaxValue);
                                client.Map.RemoveStaticRole(item);
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    ActionQuery action;

                                    action = new ActionQuery()
                                    {
                                        ObjId = item.UID,
                                        Type = ActionType.RemoveEntity
                                    };
                                    client.Send(stream.ActionCreate(action));
                                }
                                successful = true;
                            }
                        }
                        else
                        {
                            if (client.Player.RacePotions == null) client.Player.RacePotions = new UsableRacePotion[5];
                            for (ushort i = 0; i < client.Player.RacePotions.Length; i++)
                            {
                                var pot = client.Player.RacePotions[i];
                                if (pot == null)
                                {
                                    pot = (client.Player.RacePotions[i] = new UsableRacePotion());
                                    pot.Type = type;
                                    pot.Count = item.Level;
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        client.Send(stream.CreateRecePotion(new MsgRacePotion.RacePotion() { PotionType = type, Amount = (ushort)pot.Count, Location = (ushort)(i + 1) }));
                                    }

                                    successful = true;
                                    break;
                                }
                                else
                                {
                                    if (pot.Type == type)
                                    {
                                        pot.Count += item.Level;
                                        using (var rec = new ServerSockets.RecycledPacket())
                                        {
                                            var stream = rec.GetStream();
                                            client.Send(stream.CreateRecePotion(new MsgRacePotion.RacePotion() { PotionType = type, Amount = (ushort)pot.Count, Location = (ushort)(i + 1) }));
                                        }
                                        successful = true;
                                        break;
                                    }
                                }
                            }
                        }
                        if (successful)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "eidolon");
                            }

                            client.Map.RemoveStaticRole(item);
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                ActionQuery action;

                                action = new ActionQuery()
                                {
                                    ObjId = item.UID,
                                    Type = ActionType.RemoveEntity
                                };
                                client.Send(stream.ActionCreate(action));
                            }
                            item.Viable = false;

                        }

                        break;
                    }
                }
            }
        }
        private int AwardPlayer(Client.GameClient client, int time, int rank)
        {
            return Math.Max(10000, 100000 / rank - time * 2);
        }

        public void Exit(Client.GameClient client)
        {
            client.TeleportCallBack();
        }
    }
}
