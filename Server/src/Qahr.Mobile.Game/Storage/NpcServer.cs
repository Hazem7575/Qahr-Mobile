using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Qahr.Mobile.Database
{
    public class NpcServer
    {
        public class Furniture
        {
            public string Name;
            public uint ItemID;
            public uint MoneyCost;
            public uint UID;
            public ushort Mesh;
        }
        public static Dictionary<uint, Furniture> FurnitureInformations = new Dictionary<uint, Furniture>();

        public static Furniture GetNpc(uint ItemID)
        {
            foreach (var npc in FurnitureInformations.Values)
            {
                if (npc.ItemID == ItemID)
                {
                    return npc;
                }
            }
            return null;
        }
        public static Furniture GetNpcFromMesh(uint mesh)
        {
            foreach (var npc in FurnitureInformations.Values)
            {
                if (npc.Mesh == mesh)
                {
                    return npc;
                }
            }
            return null;
        }
        internal static void NpcTime()
        {
            uint Count = 0;
            string[] baseText = File.ReadAllLines(Program.ServerConfig.DbLocation + "NpcTime.txt");
            foreach (var bas_line in baseText)
            {
                Database.DBActions.ReadLine line = new DBActions.ReadLine(bas_line, ',');
                Role.SobNpc npc = new Role.SobNpc();
                npc.ObjType = Role.MapObjectType.SobNpc;
                npc.UID = line.Read((uint)0);
                npc.Name = line.Read("");
                npc.Type = (Role.Flags.NpcType)line.Read((ushort)0);
                npc.Mesh = (Role.SobNpc.StaticMesh)line.Read((uint)0);
                npc.Map = line.Read((ushort)0);
                npc.X = line.Read((ushort)0);
                npc.Y = line.Read((ushort)0);
                npc.HitPoints = line.Read((int)0);
                npc.MaxHitPoints = line.Read((int)0);
                npc.Sort = line.Read((ushort)0);
                if (npc.Map == 1039)
                    npc.MaxHitPoints = npc.HitPoints = 10000;
                if (line.Read((byte)0) == 0)
                    npc.Name = null;
                if (Pool.ServerMaps.ContainsKey(npc.Map))
                {
                    Pool.ServerMaps[npc.Map].View.EnterMap<Role.IMapObj>(npc);
                    Count++;
                    if (Role.GameMap.IsGate(npc.UID))
                        Pool.ServerMaps[npc.Map].SetGateFlagNpc(npc.X, npc.Y);
                    else
                        Pool.ServerMaps[npc.Map].SetFlagNpc(npc.X, npc.Y);
                }
            }
            MyConsole.WriteLine("Loaded " + Count + " NpcTime");

        }
        internal static void LoadSobNpcs(uint id = 0)
        {
            string[] baseText = File.ReadAllLines(Program.ServerConfig.DbLocation + "SobNpcs.txt");
            foreach (var bas_line in baseText)
            {
                Database.DBActions.ReadLine line = new DBActions.ReadLine(bas_line, ',');
                Role.SobNpc npc = new Role.SobNpc();
                npc.ObjType = Role.MapObjectType.SobNpc;
                npc.UID = line.Read((uint)0);
                npc.Name = line.Read("");
                npc.Type = (Role.Flags.NpcType)line.Read((ushort)0);
                npc.Mesh = (Role.SobNpc.StaticMesh)line.Read((uint)0);
                npc.Map = line.Read((ushort)0);
                if (npc.Map != id && id != 0) continue;
                npc.X = line.Read((ushort)0);
                npc.Y = line.Read((ushort)0);
                npc.HitPoints = line.Read((int)0);
                npc.MaxHitPoints = line.Read((int)0);
                npc.Sort = line.Read((ushort)0);
                if (npc.Map == 1039)
                    npc.MaxHitPoints = npc.HitPoints = 10000;
                if (line.Read((byte)0) == 0)
                    npc.Name = null;
                if (Pool.ServerMaps.Base.ContainsKey(npc.Map))
                {
                    Pool.ServerMaps[npc.Map].View.EnterMap<Role.IMapObj>(npc);

                    if (Role.GameMap.IsGate(npc.UID))
                        Pool.ServerMaps[npc.Map].SetGateFlagNpc(npc.X, npc.Y);
                    else
                        Pool.ServerMaps[npc.Map].SetFlagNpc(npc.X, npc.Y);
                }
            }

        }
        public static void LoadServerTraps(uint id = 0)
        {
            if (System.IO.File.Exists(Program.ServerConfig.DbLocation + "Traps.txt"))
            {
                using (System.IO.StreamReader read = System.IO.File.OpenText(Program.ServerConfig.DbLocation + "Traps.txt"))
                {
                    while (true)
                    {
                        string aline = read.ReadLine();
                        if (aline != null && aline != "")
                        {
                            string[] line = aline.Split(',');

                            uint ID = uint.Parse(line[3]);
                            ushort map = ushort.Parse(line[0]);
                            if (id != 0 && id != map) continue;
                            if (ID == 1063 && (map == 1002 || map == 1010 || map == 1011 || map == 1020 || map == 1000 || map == 1015))
                                continue;
                            var Item = new Game.MsgFloorItem.MsgItem(null, ushort.Parse(line[1]), ushort.Parse(line[2]), Game.MsgFloorItem.MsgItem.ItemType.Effect, 0, 0, ushort.Parse(line[0]), 0, false, Pool.ServerMaps[ushort.Parse(line[0])], 60 * 60 * 1000);
                            Item.MsgFloor.m_ID = ID;
                            Item.MsgFloor.m_Color = 2;
                            Item.MsgFloor.DropType = Game.MsgFloorItem.MsgDropID.Effect;
                            if (line.Length > 4)
                                Item.AllowDynamic = byte.Parse(line[4]) == 1;
                            Item.GMap.View.EnterMap<Role.IMapObj>(Item);

                        }
                        else
                            break;
                    }
                }
            }
        }
        /// <summary>
        /// One Npcs.txt line (already split on ',') -> Npc, exactly as the startup loader reads it.
        /// Returns null for lines the loader skips (mesh 42580; ids 15987/15745 when built without Jiang).
        /// Throws on a malformed line (same as before). Shared by LoadNpcs and HotReload.NpcHotReload.
        /// </summary>
        public static Game.MsgNpc.Npc ParseNpcLine(string[] line)
        {
            Game.MsgNpc.Npc np = Game.MsgNpc.Npc.Create();
            np.UID = uint.Parse(line[0]);
            np.NpcType = (Role.Flags.NpcType)byte.Parse(line[1]);
            np.Mesh = uint.Parse(line[2]);
            np.Map = ushort.Parse(line[3]);
            np.X = ushort.Parse(line[4]);
            np.Y = ushort.Parse(line[5]);
            if (np.Mesh == 42580)
                return null;
            if (np.UID == 16851 || np.UID == 168052)
            {
                np.AllowDynamic = true;
            }
            else if (line.Length > 6)
                np.AllowDynamic = ushort.Parse(line[6]) == 1;
#if !Jiang
            if (np.UID == 15987 || np.UID == 15745)
                return null;
#endif
            return np;
        }
        public static void LoadNpcs(uint id = 0)
        {
            uint Count = 0;
            var loaded = new List<Game.MsgNpc.Npc>(); // [HotReload] what this call put on the maps from Npcs.txt
            if (System.IO.File.Exists(Program.ServerConfig.DbLocation + "npcs.txt"))
            {
                using (System.IO.StreamReader read = System.IO.File.OpenText(Program.ServerConfig.DbLocation + "npcs.txt"))
                {
                    while (true)
                    {
                        string aline = read.ReadLine();
                        if (aline != null && aline != "")
                        {

                            string[] line = aline.Split(',');
                            if (id != 0 && id != ushort.Parse(line[3])) continue;
                            Game.MsgNpc.Npc np = ParseNpcLine(line); // [HotReload] shared with NpcHotReload
                            if (np == null)
                                continue;

#if !TEST
                            /* if (np.NpcType == Role.Flags.NpcType.Talker)
                           {
                               if (!Enum.IsDefined(typeof(Game.MsgNpc.NpcID), np.UID))
                                   continue;
                           }*/
#endif
                            if (Pool.ServerMaps.Base.ContainsKey(np.Map))
                            {
                                Count++;
                                Pool.ServerMaps[np.Map].AddNpc(np);
                                loaded.Add(np);
                            }

                        }
                        else
                            break;
                    }
                }
            }
            // [HotReload] remember what Npcs.txt put on the loaded map(s), so a live reload can diff against it
            global::Qahr.Mobile.HotReload.NpcHotReload.RecordLoaded(id, loaded);
            if (System.IO.File.Exists(Program.ServerConfig.DbLocation + "furnitures.txt"))
            {
                using (StreamReader read = File.OpenText(Program.ServerConfig.DbLocation + "furnitures.txt"))
                {
                    //read.ReadLine();//first line
                    while (true)
                    {
                        string aline = read.ReadLine();
                        if (aline != null)
                        {

                            string[] line = aline.Split(',');
                            if (id != 0 && id != ushort.Parse(line[3])) continue;
                            Game.MsgNpc.Npc np = Game.MsgNpc.Npc.Create();
                            np.UID = uint.Parse(line[0]);
                            np.NpcType = (Role.Flags.NpcType)byte.Parse(line[1]);
                            np.Mesh = uint.Parse(line[2]);
                            np.Map = ushort.Parse(line[3]);
                            np.X = ushort.Parse(line[4]);
                            np.Y = ushort.Parse(line[5]);

                            Furniture furnit = new Furniture();
                            furnit.Name = line[6];
                            furnit.ItemID = uint.Parse(line[7]);
                            furnit.UID = np.UID;
                            furnit.Mesh = (ushort)np.Mesh;
                            furnit.MoneyCost = uint.Parse(line[8]);
                            if (!FurnitureInformations.ContainsKey(np.UID))
                                FurnitureInformations.Add(np.UID, furnit);

                            if (Pool.ServerMaps.Base.ContainsKey(np.Map))
                            {
                                Pool.ServerMaps[np.Map].AddNpc(np);
                            }
                        }
                        else
                            break;
                    }
                }
            }
        }
    }
}
