using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Qahr.Mobile.Game.MsgNpc
{
    public unsafe static partial class MsgBuilder
    {
        [ProtoContract]
        public class NpcSpawnProto
        {
            [ProtoMember(1, IsRequired = true)]
            public uint UID;
            [ProtoMember(2, IsRequired = true)]
            public uint UID2;
            [ProtoMember(3, IsRequired = true)]
            public uint UNK;
            [ProtoMember(4, IsRequired = true)]
            public ushort X;
            [ProtoMember(5, IsRequired = true)]
            public ushort Y;
            [ProtoMember(6, IsRequired = true)]
            public uint Mesh;
            [ProtoMember(7, IsRequired = true)]
            public ushort Type;
            [ProtoMember(8, IsRequired = true)]
            public ushort Action;
            [ProtoMember(9)]
            public string Name;// QahrMobile: CMsgNpcInfoPB SzBuf, the name shown above the NPC (null = the body's name)
        }
        public static unsafe ServerSockets.Packet NpcCreate(this ServerSockets.Packet stream, Npc npc, ushort newMesh =0)
        {
            stream.InitWriter();
            var proto = new NpcSpawnProto() { UID = npc.UID, X = npc.X, Y = npc.Y, Type = (ushort)npc.NpcType, Action = npc.Sort, UID2 = npc.UnKnow };
            if (newMesh != 0)
                proto.Mesh = newMesh;
            else
                proto.Mesh = npc.Mesh;
            // QahrMobile: the body and name the mobile client knows for this NPC (Data\mobile_npclooks.txt)
            string mobileName;
            proto.Mesh = Phone.MobileNpcLooks.Mesh(proto.Mesh, out mobileName);
            proto.Name = mobileName == "-" ? " " : !string.IsNullOrEmpty(npc.Name) ? npc.Name : mobileName;// "-" = no name over it
            stream.ProtoBufferSerialize(proto);
            stream.Finalize(GamePackets.MsgNpcInfo);
            return stream;
        }
        public static unsafe void GetNpc(this ServerSockets.Packet stream, out Npc npc)
        {
              var proto = new NpcSpawnProto();
              proto = stream.ProtoBufferDeserialize<NpcSpawnProto>(proto);

            npc = new Npc();
            npc.UID = proto.UID;
            npc.UnKnow = proto.UID2;
            npc.X = proto.X;
            npc.Y = proto.Y;
            npc.Mesh = proto.Mesh;
            npc.NpcType = (Role.Flags.NpcType)proto.Type;
            npc.Sort = proto.Action;
        }
    }
    public unsafe class Npc : Role.IMapObj
    {
        public const byte SeedDistance = 19;//16 old , 18

        public DateTime Respawn = new DateTime();
        public uint IndexInScreen { get; set; }
        public bool IsTrap() { return false; }

        public ushort Leng;
        public ushort PacketID;
        public int Stamp;
        public uint UID { get; set; }
        public uint UnKnow;
        public ushort X { get; set; }
        public ushort Y { get; set; }
        public uint Mesh;
        public Role.Flags.NpcType NpcType;
        public Role.MapObjectType ObjType { get; set; }
        public ushort Sort;
        public ushort ServerID;
        public bool Alive { get; set; }
        public string Name { get; set; }
        public uint DynamicID { get; set; }
        public uint Map { get; set; }
        public bool AllowDynamic { get; set; }
        Role.GameMap _gmap;
        public Role.GameMap GMap
        {
            get {
                if (_gmap == null)
                    _gmap = Pool.ServerMaps[Map];
                return _gmap;
            }
        }

        public static Npc Create()
        {
            
            Npc _npc = new Npc();
            _npc.Leng = 26;
            _npc.AllowDynamic = false;
            _npc.PacketID = GamePackets.MsgNpcInfo;
         //   _npc.Stamp= DateTime.Now.Value();
            _npc.ObjType = Role.MapObjectType.Npc;
            _npc.Alive = true;
            return _npc;
        }
        public bool CanSee(Role.IMapObj obj)
        {
            try
            {
                if (obj == null)
                    return false;
                if (obj.Map != Map)
                    return false;
                if (!AllowDynamic)
                {
                    if (obj.DynamicID != DynamicID)
                        return false;
                }
                if (obj.UID == UID)
                    return false;
                return Role.Core.GetDistance(obj.X, obj.Y, X, Y) <= SeedDistance;
            }
            catch (Exception e)
            {
                MyConsole.WriteException(e);
                return false;
            }
        }
        public void Send(ServerSockets.Packet stream)
        {
            foreach (var user in GMap.View.Roles(Role.MapObjectType.Player, X, Y))
            {
                if (CanSee(user))
                {
                    UnKnow = UID;
                    (user as Role.Player).Send(stream.NpcCreate(this));
                }
            }
        }
        public ServerSockets.Packet GetArray(ServerSockets.Packet stream, bool View)
        {
            UnKnow = UID;
            return stream = stream.NpcCreate(this);
        }
        [PacketAttribute(GamePackets.MsgNpcInfo)]
        public unsafe static void Procesor(Client.GameClient client, ServerSockets.Packet packet)
        {
            Npc stream;

            packet.GetNpc(out stream);


            if (client.Player.Map == 1038 && client.UseItem == 720020)//NEWGUILDWAR Statue
            {
                if (Game.MsgTournaments.MsgSchedules.GuildWar.Proces != MsgTournaments.ProcesType.Dead)
                {
                    if (Game.MsgTournaments.MsgSchedules.GuildWar.Winner.GuildID == client.Player.GuildID)
                    {
                        if (client.Inventory.Contain(client.UseItem, 1))
                        {

                            switch (client.Player.GuildRank)
                            {
                                case Role.Flags.GuildMemberRank.GuildLeader:
                                    {
                                        ushort x = MsgTournaments.MsgSchedules.GuildWar.StatueCoords[0][0];
                                        ushort y = MsgTournaments.MsgSchedules.GuildWar.StatueCoords[0][1];
                                        if (Role.Core.GetDistance(client.Player.X, client.Player.Y, x, y) <= 3)
                                        {
                                            if (!Role.Statue.ContainStatue(client.Map, x, y))
                                            {
                                                client.Inventory.Remove(client.UseItem, 1, packet);
                                                Role.Statue.CreateStatue(client, x, y, stream.Sort, (int)stream.NpcType);
                                            }
                                        }
                                        break;
                                    }
                                case Role.Flags.GuildMemberRank.DeputyLeader:
                                    {
                                        for (int i = 1; i < 3; i++)
                                        {
                                            ushort x = MsgTournaments.MsgSchedules.GuildWar.StatueCoords[i][0];
                                            ushort y = MsgTournaments.MsgSchedules.GuildWar.StatueCoords[i][1];
                                            if (Role.Core.GetDistance(client.Player.X, client.Player.Y, x, y) <= 3)
                                            {
                                                if (!Role.Statue.ContainStatue(client.Map, x, y))
                                                {
                                                    client.Inventory.Remove(client.UseItem, 1, packet);
                                                    Role.Statue.CreateStatue(client, x, y, stream.Sort, (int)stream.NpcType);
                                                    break;
                                                }
                                            }
                                        }
                                        break;
                                    }
                                case Role.Flags.GuildMemberRank.Member:
                                    {
                                        for (int i = 3; i < MsgTournaments.MsgSchedules.GuildWar.StatueCoords.Length; i++)
                                        {
                                            ushort x = MsgTournaments.MsgSchedules.GuildWar.StatueCoords[i][0];
                                            ushort y = MsgTournaments.MsgSchedules.GuildWar.StatueCoords[i][1];
                                            if (Role.Core.GetDistance(client.Player.X, client.Player.Y, x, y) <= 3)
                                            {
                                                if (!Role.Statue.ContainStatue(client.Map, x, y))
                                                {
                                                    client.Inventory.Remove(client.UseItem, 1, packet);
                                                    Role.Statue.CreateStatue(client, x, y, stream.Sort, (int)stream.NpcType);
                                                    break;
                                                }
                                            }
                                        }
                                        break;
                                    }
                            }
                        }
                    }
                    else
                        client.SendSysMesage("Your guild not dominated the pole");
                }
                return;
            }
            // if (!client.Player.Name.Contains("[GM]"))
            if (stream.Mesh - (stream.Mesh % 10) == client.MoveNpcMesh - (client.MoveNpcMesh % 10))
            {
                if (client.UseItem >= 720021 && client.UseItem <= 720024)
                {
                    if (client.Player.GuildID == MsgTournaments.MsgSchedules.GuildWar.Winner.GuildID && client.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader)
                    {
                        ushort X = stream.X;
                        ushort Y = stream.Y;
                        if (client.Map.IsValidFlagNpc(X, Y) && !MsgTournaments.MsgGuildWar.GuildConductor.BlockMaps.Contains(client.Player.Map) && client.Player.DynamicID == 0)
                        {
                            uint ToMap = client.Player.Map;
                            ushort ToX = X;
                            ushort ToY = Y;
                            if (client.Map.AddGuildTeleporterItem(ref ToX, ref ToY))
                            {
                                var conductor = MsgTournaments.MsgSchedules.GuildWar.GuildConductors[(NpcID)client.MoveNpcUID];
                                //remove oldconductor--------------------
                                Role.GameMap map;
                                Role.GameMap.EnterMap((int)conductor.Npc.Map);
                                if (Pool.ServerMaps.TryGetValue(conductor.Npc.Map, out map))
                                    map.RemoveNpc(conductor.Npc, packet);
                                //---------------------------------------

                                var npc = Npc.Create();
                                npc.UID = client.MoveNpcUID;
                                npc.Mesh = stream.Mesh;
                                npc.Map = client.Player.Map;
                                npc.X = X;
                                npc.Y = Y;
                                npc.NpcType = Role.Flags.NpcType.Talker;

                                client.Map.AddNpc(npc);
                                client.Player.View.SendView(packet.NpcCreate(npc), true);

                                MsgTournaments.MsgSchedules.GuildWar.GuildConductors[(NpcID)client.MoveNpcUID].Npc = npc;
                                MsgTournaments.MsgSchedules.GuildWar.GuildConductors[(NpcID)client.MoveNpcUID].ToX = ToX;
                                MsgTournaments.MsgSchedules.GuildWar.GuildConductors[(NpcID)client.MoveNpcUID].ToY = ToY;
                                MsgTournaments.MsgSchedules.GuildWar.GuildConductors[(NpcID)client.MoveNpcUID].ToMap = ToMap;

                                client.Inventory.Remove(client.UseItem, 1, packet);

                            }
                            else
                                client.SendSysMesage("Invalid map Location.");
                        }
                        else
                            client.SendSysMesage("Invalid map Location.");
                    }
                    else
                    {
                        client.SendSysMesage("Sorry,~only~the~guild~leader~who~won~the~guild~war~can~change~my~location.");
                    }
                    return;
                }
                //if (stream->UID == client.MoveNpcUID && client.MoveNpcUID != 0)
                {
                    if (client.MyHouse == null)
                    {
                        client.SendSysMesage("Sorry, you not have house.");
                        return;
                    }
                    if (Database.HouseTable.CountFurnitures(client.MyHouse.Level) < client.MyHouse.Furnitures.Count)
                    {
                        client.SendSysMesage("Sorry, you can have only" + Database.HouseTable.CountFurnitures(client.MyHouse.Level) + " Furnitures in you house.");
                        return;
                    }
                    if (client.MyHouse.Furnitures.ContainsKey(client.MoveNpcUID))
                    {
                        client.SendSysMesage("Sorry, but you have that npc");
                        return;
                    }
                    if (client.Player.DynamicID != client.Player.UID)
                    {
                        client.SendSysMesage("You have to be in your own house to be able to display it.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                        return;
                    }
                    if (client.Player.Name.Contains("[GM]"))
                    {
                        Npc np2 = Npc.Create();
                        np2.UID = client.MoveNpcUID;
                        np2.X = stream.X;
                        np2.Y = stream.Y;
                        np2.Mesh = stream.Mesh;
                        np2.Map = client.Player.Map;
                        np2.DynamicID = client.Player.UID;
                        np2.NpcType = Role.Flags.NpcType.Furniture;
                        // if (np2.Mesh == 1002)
                        np2.NpcType = Role.Flags.NpcType.Talker;
                        client.MyHouse.Furnitures.TryAdd(np2.UID, np2);
                        client.Player.View.SendView(packet.NpcCreate(np2), true);
                    }
                    Npc np = Npc.Create();
                    np.UID = client.MoveNpcUID;
                    np.X = stream.X;
                    np.Y = stream.Y;
                    np.Mesh = stream.Mesh;
                    np.Map = client.Player.Map;
                    np.DynamicID = client.Player.UID;
                    np.NpcType = Role.Flags.NpcType.Furniture;


                    if (np.Mesh == 8200)
                        np.NpcType = Role.Flags.NpcType.Talker;
                    client.MyHouse.Furnitures.TryAdd(np.UID, np);
                    client.Player.View.SendView(packet.NpcCreate(np), true);
                }
            }
        }
    }
}
