using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.IO;

namespace Qahr.Mobile.Role.Instance
{
    public class House
    {
        public struct DBNpc
        {
            public uint UID;
            public uint UnKnow;
            public ushort X;
            public ushort Y;
            public uint Mesh;
            public Role.Flags.NpcType NpcType;
            public Role.MapObjectType ObjType;
            public ushort Sort;
            public uint DynamicID;
            public uint Map;

            public static DBNpc Create(Game.MsgNpc.Npc npc)
            {
                DBNpc Dbnpc = new DBNpc();
                Dbnpc.UID = npc.UID;
                Dbnpc.UnKnow = npc.UnKnow;
                Dbnpc.X = npc.X;
                Dbnpc.Y = npc.Y;
                Dbnpc.Mesh = npc.Mesh;
                Dbnpc.NpcType = npc.NpcType;
                Dbnpc.ObjType = npc.ObjType;
                Dbnpc.Sort = npc.Sort;
                Dbnpc.DynamicID = npc.DynamicID;
                Dbnpc.Map = npc.Map;
                return Dbnpc;
            }
            public static Game.MsgNpc.Npc GetServerNpc(DBNpc Dbnpc)
            {
                Game.MsgNpc.Npc npc = new Game.MsgNpc.Npc();
                npc.UID = Dbnpc.UID;
                npc.UnKnow = Dbnpc.UnKnow;
                npc.X = Dbnpc.X;
                npc.Y = Dbnpc.Y;
                npc.Mesh = Dbnpc.Mesh;
                npc.NpcType = Dbnpc.NpcType;
                npc.ObjType = Dbnpc.ObjType;
                npc.Sort = Dbnpc.Sort;
                npc.DynamicID = Dbnpc.DynamicID;
                npc.Map = Dbnpc.Map;
                return npc;
            }
        }
        public House(uint UID)
        {
            if (!HousePoll.ContainsKey(UID))
                HousePoll.TryAdd(UID, this);

            if (!Pool.Constants.BlockAttackMap.Contains(UID))
                Pool.Constants.BlockAttackMap.Add(UID);
        }
        public static ConcurrentDictionary<uint, House> HousePoll = new ConcurrentDictionary<uint, House>();

        public byte Level = 1;
        public ConcurrentDictionary<uint, Game.MsgNpc.Npc> Furnitures = new ConcurrentDictionary<uint, Game.MsgNpc.Npc>();

        internal unsafe static void Load(Client.GameClient client)
        {
            if (HousePoll.ContainsKey(client.Player.UID))
            {
                client.MyHouse = HousePoll[client.Player.UID];
            }
            else
            {
                WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
                if (binary.Open(Program.ServerConfig.DbLocation + "\\Houses\\" + client.Player.UID + ".bin", FileMode.Open))
                {
                    int ItemCount = 0;
                    byte Level = 0;
                    binary.Read(&Level, sizeof(byte));

                    client.MyHouse = new House(client.Player.UID);
                    client.MyHouse.Level = Level;

                    binary.Read(&ItemCount, sizeof(int));

                    Game.MsgNpc.Npc Furnitures;
                    for (int x = 0; x < ItemCount; x++)
                    {
                        DBNpc DBNpcc = new DBNpc();
                        binary.Read(&DBNpcc, sizeof(DBNpc));
                        Furnitures = DBNpc.GetServerNpc(DBNpcc);
                        if (!client.MyHouse.Furnitures.ContainsKey(Furnitures.UID))
                            client.MyHouse.Furnitures.TryAdd(Furnitures.UID, Furnitures);
                    }
                    binary.Close();
                }
            }
        }
        internal unsafe static void Save(Client.GameClient client)
        {
            if (client.MyHouse != null)
            {
                WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
                string housePath = Program.ServerConfig.DbLocation + "\\Houses\\" + client.Player.UID + ".bin";
                if (binary.Open(housePath, FileMode.Create))
                {
                    byte level = client.MyHouse.Level;
                    int ItemCount = client.MyHouse.Furnitures.Count;
                    binary.Write(&level, sizeof(byte));
                    binary.Write(&ItemCount, sizeof(int));
                    foreach (var furniture in client.MyHouse.Furnitures.Values)
                    {
                        Game.MsgNpc.Npc Furnitures = furniture;
                        DBNpc Db_npc = DBNpc.Create(Furnitures);
                        binary.Write(&Db_npc, sizeof(DBNpc));
                    }
                    binary.Close();
                    global::Qahr.Mobile.Database.Storage.PlayerBinaryStorage.OnBlobSaved(housePath); // [DB-only] keep the DB blob fresh (load reads it)
                }
            }
        }
    }
}
