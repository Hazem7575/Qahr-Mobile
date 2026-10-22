using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.Role
{
    public unsafe class Statue
    {
        public const ushort ElitePkDisplayMap = 1002;
        public const ushort ElitePkDisplayX = 313;
        public const ushort ElitePkDisplayY = 276;
        public const ushort DisplayAppearanceScale = 150;
        public const byte CurrentPacketVersion = 1;

        public unsafe static System.Counter CounterUID = new System.Counter(105175);
        public static Statue StaticStatue = null;
        public static SobNpc StaticSobNpc = null;
        public static bool ContainStatue(Role.GameMap map, ushort x, ushort y)
        {
            foreach (var npc in map.View.Roles(MapObjectType.SobNpc, x, y))
                if (npc.X == x && npc.Y == y)
                    return true;
            return false;
        }
        public int Action = 0;
        public ushort Action2;
        public uint UID;
        public int HitPotion = 0;
        public Client.GameClient user;

        public byte[] StatuePacket;
        public byte PacketVersion;
        public bool Static = false;
        public unsafe static void CreateStatue(Client.GameClient client, ushort x, ushort y, int Action, int action2, bool Static = false)
        {
            try
            {

                Statue stat = new Statue();
                stat.user = client;

                stat.UID = CounterUID.Next;
                stat.HitPotion = client.Player.HitPoints / 100;
                stat.Action = Action;
                stat.Static = Static;
                stat.PacketVersion = Static ? CurrentPacketVersion : (byte)0;
                stat.StatuePacket = null;
                if (stat.Static)
                {
                    stat.user.Player.Action = Flags.ConquerAction.Sit;
                    stat.user.Player.Angle = Flags.ConquerAngle.South;
                    StaticStatue = stat;
                }
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    SobNpc npc = new SobNpc(stat);
                    npc.ObjType = MapObjectType.SobNpc;
                    npc.UID = stat.UID;
                    npc.Name = client.Player.Name;
                    npc.X = x;
                    npc.Y = y;
                    npc.Map = Static ? ElitePkDisplayMap : client.Player.Map;
                    npc.MaxHitPoints = (int)(client.Status.MaxHitpoints * 10);
                    npc.HitPoints = client.Player.HitPoints * 10;

                    var spawnMap = Pool.ServerMaps[npc.Map];
                    spawnMap.View.EnterMap<Role.IMapObj>(npc);
                    BroadcastStatue(stream, npc, spawnMap);

                    if (Static)
                        StaticSobNpc = npc;
                }
            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
        }
        public unsafe static void BroadcastStatue(ServerSockets.Packet stream, SobNpc npc, Role.GameMap map)
        {
            var packet = npc.GetArray(stream, true);
            foreach (var viewer in Pool.GamePoll.Values)
            {
                if (viewer.Player.Map != npc.Map)
                    continue;
                if (Role.Core.GetDistance(viewer.Player.X, viewer.Player.Y, npc.X, npc.Y) <= SobNpc.SeedDistrance)
                    viewer.Send(packet);
            }
        }
        public unsafe static void RemoveStatue(ServerSockets.Packet stream, Client.GameClient killer, uint UID, Role.IMapObj obj)
        {
            Role.GameMap map;
            if (obj.Map != 0 && Pool.ServerMaps.ContainsKey(obj.Map))
                map = Pool.ServerMaps[obj.Map];
            else if (killer.Map != null)
                map = killer.Map;
            else
                map = Pool.ServerMaps[1002];

            map.View.LeaveMap<Role.IMapObj>(obj);

            ActionQuery action = new ActionQuery()
            {
                ObjId = UID,
                Type = ActionType.RemoveEntity
            };
            var removePacket = stream.ActionCreate(action);
            foreach (var viewer in Pool.GamePoll.Values)
            {
                if (viewer.Player.Map == map.ID)
                    viewer.Send(removePacket);
            }
        }

        public static void ElitePkStatue(Client.GameClient user)
        {
            if (StaticStatue != null)
                StaticStatue.StatuePacket = null;

            if (StaticStatue == null && StaticSobNpc == null)
            {
                CreateStatue(user, ElitePkDisplayX, ElitePkDisplayY, 0, 0, true);
            }
            else
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    RemoveStatue(stream, user, StaticSobNpc.UID, StaticSobNpc);
                    StaticStatue = null;
                    StaticSobNpc = null;
                    CreateStatue(user, ElitePkDisplayX, ElitePkDisplayY, 0, 0, true);
                }
            }
        }
        public static void Save()
        {
            using (Database.DBActions.Write _wr = new Database.DBActions.Write("StaticStatue.txt"))
            {
                if (StaticStatue != null && StaticStatue.StatuePacket != null && StaticSobNpc != null)
                {
                    int Size = StaticStatue.StatuePacket.Length;
                    Database.DBActions.WriteLine line = new Database.DBActions.WriteLine('/');


                    line.Add(Size);
                    for (int x = 0; x < Size; x++)
                        line.Add(StaticStatue.StatuePacket[x]);
                    line.Add(StaticSobNpc.UID).Add(StaticSobNpc.X).Add(StaticSobNpc.Y).Add(StaticSobNpc.Map).Add(StaticSobNpc.MaxHitPoints).Add(StaticSobNpc.HitPoints).Add(StaticStatue.PacketVersion);
                    _wr.Add(line.Close());
                    _wr.Execute(Database.DBActions.Mode.Open);
                }
            }
        }
        public static void Load()
        {
            using (Database.DBActions.Read r = new Database.DBActions.Read("StaticStatue.txt"))
            {
                if (r.Reader())
                {
                    int count = r.Count;
                    for (uint x = 0; x < count; x++)
                    {

                        Database.DBActions.ReadLine readerline = new Database.DBActions.ReadLine(r.ReadString(""), '/');
                        int Size = readerline.Read((int)0);
                        if (Size != 0)
                        {
                            StaticStatue = new Statue();
                            StaticStatue.Static = true;

                            StaticStatue.StatuePacket = new byte[Size];
                            for (int i = 0; i < StaticStatue.StatuePacket.Length; i++)
                                StaticStatue.StatuePacket[i] = readerline.Read((byte)0);

                            StaticSobNpc = new SobNpc(StaticStatue);
                            StaticSobNpc.ObjType = MapObjectType.SobNpc;
                            StaticSobNpc.UID = readerline.Read((uint)0);
                            StaticSobNpc.X = readerline.Read((ushort)0);
                            StaticSobNpc.Y = readerline.Read((ushort)0);
                            StaticSobNpc.Map = readerline.Read((ushort)0);
                            StaticSobNpc.MaxHitPoints = readerline.Read((int)0);
                            StaticSobNpc.HitPoints = readerline.Read((int)0);
                            StaticStatue.PacketVersion = readerline.Read((byte)0);

                            Pool.ServerMaps[StaticSobNpc.Map].View.EnterMap<Role.IMapObj>(StaticSobNpc);
                        }
                    }
                }
            }
        }

    }
}
