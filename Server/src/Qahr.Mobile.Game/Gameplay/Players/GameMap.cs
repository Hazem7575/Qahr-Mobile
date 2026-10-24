using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.IO;
using Qahr.Mobile.Game.MsgServer;
using Qahr.Mobile.Game.MsgFloorItem;
using Qahr.Mobile.Client;


namespace Qahr.Mobile.Role
{
    public class MapView
    {
        const int CELLS_PER_BLOCK = 18;

        private Extensions.Counter CounterMovement = new Extensions.Counter(1);

        public ViewPtr[,] m_setBlock;

        private int Width, Height;

        private int GetWidthOfBlock() { return (Width - 1) / CELLS_PER_BLOCK + 1; }
        private int GetHeightOfBlock() { return (Height - 1) / CELLS_PER_BLOCK + 1; }

        public MapView(int _Width, int _Height)
        {
            Width = _Width;
            Height = _Height;

            m_setBlock = new ViewPtr[GetWidthOfBlock(), GetHeightOfBlock()];
            for (int x = 0; x < GetWidthOfBlock(); x++)
                for (int y = 0; y < GetHeightOfBlock(); y++)
                    m_setBlock[x, y] = new ViewPtr();
        }

        private int Block(int nPos)
        {
            return nPos / CELLS_PER_BLOCK;
        }
        private ViewPtr BlockSet(int nPosX, int nPosY) { return m_setBlock[Block(nPosX), Block(nPosY)]; }

        public bool MoveTo<T>(T obj, int nNewPosX, int nNewPosY)
            where T : IMapObj
        {

            int nOldPosX = obj.X;
            int nOldPosY = obj.Y;
            if ((nOldPosX >= 0 && nOldPosX < Width) == false)
                return false;
            if ((nOldPosY >= 0 && nOldPosY < Height) == false)
                return false;
            if ((nNewPosX >= 0 && nNewPosX < Width) == false)
                return false;
            if ((nNewPosY >= 0 && nNewPosY < Height) == false)
                return false;

            if (Block(nOldPosX) == Block(nNewPosX) && Block(nOldPosY) == Block(nNewPosY))
                return false;

            BlockSet(nOldPosX, nOldPosY).RemoveObject<T>(obj);
            BlockSet(nNewPosX, nNewPosY).AddObject<T>(obj);

            if (obj.ObjType == MapObjectType.Player)
                obj.IndexInScreen = CounterMovement.Next;

            return true;
        }

        public bool EnterMap<T>(T obj)
            where T : IMapObj
        {
            if ((obj.X >= 0 && obj.X < Width) == false)
                return false;
            if ((obj.Y >= 0 && obj.Y < Height) == false)
                return false;

            BlockSet(obj.X, obj.Y).AddObject<T>(obj);

            if (obj.ObjType == MapObjectType.Player)
                obj.IndexInScreen = CounterMovement.Next;

            return true;
        }
        public bool LeaveMap<T>(T obj)
             where T : IMapObj
        {
            if ((obj.X >= 0 && obj.X < Width) == false)
                return false;
            if ((obj.Y >= 0 && obj.Y < Height) == false)
                return false;

            BlockSet(obj.X, obj.Y).RemoveObject<T>(obj);

            return true;
        }
        public IEnumerable<IMapObj> Roles(MapObjectType typ, int X, int Y, Predicate<IMapObj> P = null)
        {

            for (int x = Math.Max(Block(X) - 1, 0); x <= Block(X) + 1 && x < GetWidthOfBlock(); x++)
                for (int y = Math.Max(Block(Y) - 1, 0); y <= Block(Y) + 1 && y < GetHeightOfBlock(); y++)
                {
                    var list = m_setBlock[x, y].GetObjects(typ);
                    if (list != null)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            if (i >= list.Count)
                                break;
                            var element = list[i];
                            if (element != null)
                            {
                                if (P != null)
                                {
                                    if (P(element))
                                        yield return element;
                                }
                                else if (element != null)
                                    yield return element;
                            }
                        }
                    }
                }


        }
        public int CountRoles(MapObjectType typ, int X, int Y)
        {
            int count = 0;
            for (int x = Math.Max(Block(X) - 1, 0); x <= Block(X) + 1 && x < GetWidthOfBlock(); x++)
                for (int y = Math.Max(Block(Y) - 1, 0); y <= Block(Y) + 1 && y < GetHeightOfBlock(); y++)
                {
                    var list = m_setBlock[x, y].GetObjects(typ);
                    count += list.Count;
                }
            return count;
        }
        public IEnumerable<IMapObj> GetAllMapRoles(MapObjectType typ, Predicate<IMapObj> P = null)
        {
            for (int x = 0; x < GetWidthOfBlock(); x++)
                for (int y = 0; y < GetHeightOfBlock(); y++)
                {
                    var list = m_setBlock[x, y].GetObjects(typ);
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (i >= list.Count)
                            break;
                        var element = list[i];
                        if (element != null)
                        {
                            if (P != null)
                            {
                                if (P(element))
                                    yield return element;
                            }
                            else if (element != null)
                                yield return element;
                        }
                    }
                }
        }
        public int GetAllMapRolesCount(MapObjectType typ, Predicate<IMapObj> P = null)
        {
            return GetAllMapRoles(typ, P).Count();
        }
        public T GetMapObject<T>(MapObjectType typ, uint UID, Predicate<IMapObj> P = null)
        {
            foreach (var obj in GetAllMapRoles(typ, P))
                if (obj.UID == UID)
                    return (T)obj;
            return default(T);
        }
        public bool MapContain(MapObjectType typ, uint UID, Predicate<IMapObj> P = null)
        {
            foreach (var obj in GetAllMapRoles(typ, P))
                if (obj.UID == UID)
                    return true;
            return false;
        }
        public void ClearMap(MapObjectType typ)
        {
            for (int x = 0; x < GetWidthOfBlock(); x++)
                for (int y = 0; y < GetHeightOfBlock(); y++)
                {
                    m_setBlock[x, y].Clear(typ);
                }
        }
        public bool TryGetObject<T>(uint UID, MapObjectType typ, int X, int Y, out T obj)
            where T : IMapObj
        {
            for (int x = Math.Max(Block(X) - 1, 0); x <= Block(X) + 1 && x < GetWidthOfBlock(); x++)
                for (int y = Math.Max(Block(Y) - 1, 0); y <= Block(Y) + 1 && y < GetHeightOfBlock(); y++)
                {
                    var list = m_setBlock[x, y];
                    if (list.TryGetObject<T>(typ, UID, out obj))
                        return true;

                }
            obj = default(T);
            return false;
        }
        public bool Contain(uint UID, int X, int Y)
        {
            for (int x = Math.Max(Block(X) - 1, 0); x <= Block(X) + 1 && x < GetWidthOfBlock(); x++)
                for (int y = Math.Max(Block(Y) - 1, 0); y <= Block(Y) + 1 && y < GetHeightOfBlock(); y++)
                {
                    var list = m_setBlock[x, y];
                    for (int i = 0; i < (int)MapObjectType.Count; i++)
                        if (list.ContainObject((MapObjectType)i, UID))
                            return true;

                }
            return false;
        }
    }
    public class ViewPtr
    {
        private Extensions.MyList<Role.IMapObj>[] Objects;
        public ViewPtr()
        {
            Objects = new Extensions.MyList<IMapObj>[(int)MapObjectType.Count];
            for (int x = 0; x < (int)MapObjectType.Count; x++)
                Objects[x] = new Extensions.MyList<IMapObj>();
        }


        public void AddObject<T>(T obj)
             where T : IMapObj
        {

            Objects[(int)obj.ObjType].Add(obj);
        }

        public void RemoveObject<T>(T obj)
            where T : IMapObj
        {
            Objects[(int)obj.ObjType].Remove(obj);
        }


        public bool ContainObject(MapObjectType obj_t, uint UID)
        {
            for (int x = 0; x < Objects[(int)obj_t].Count; x++)
            {
                var list = Objects[(int)obj_t];
                if (x >= list.Count)
                    break;
                if (list[x].UID == UID)
                    return true;
            }
            return false;
        }

        public bool TryGetObject<T>(MapObjectType obj_t, uint UID, out T obj)
        {
            for (int x = 0; x < Objects[(int)obj_t].Count; x++)
            {
                var list = Objects[(int)obj_t];
                if (x >= list.Count)
                    break;
                if (list[x].UID == UID)
                {
                    obj = (T)list[x];
                    return true;
                }
            }
            obj = default(T);
            return false;
        }
        public Extensions.MyList<IMapObj> GetObjects(MapObjectType typ)
        {
            return Objects[(int)typ];
        }

        public void Clear(MapObjectType typ)
        {
            Objects[(int)typ].Clear();
        }
    }
   
    //public class MapView
    //{
    //    const int CELLS_PER_BLOCK = 18;

    //    private System.Counter CounterMovement = new System.Counter(1);

    //    public ViewPtr[,] m_setBlock;

    //    private int Width, Height;

    //    private int GetWidthOfBlock() { return (Width - 1) / CELLS_PER_BLOCK + 1; }
    //    private int GetHeightOfBlock() { return (Height - 1) / CELLS_PER_BLOCK + 1; }

    //    public MapView(int _Width, int _Height)
    //    {
    //        Width = _Width;
    //        Height = _Height;

    //        m_setBlock = new ViewPtr[GetWidthOfBlock(), GetHeightOfBlock()];
    //        for (int x = 0; x < GetWidthOfBlock(); x++)
    //            for (int y = 0; y < GetHeightOfBlock(); y++)
    //                m_setBlock[x, y] = new ViewPtr();
    //    }

    //    private int Block(int nPos)
    //    {
    //        return nPos / CELLS_PER_BLOCK;
    //    }
    //    private ViewPtr BlockSet(int nPosX, int nPosY) { return m_setBlock[Block(nPosX), Block(nPosY)]; }

    //    public bool MoveTo<T>(T obj, int nNewPosX, int nNewPosY)
    //        where T : IMapObj
    //    {

    //        int nOldPosX = obj.X;
    //        int nOldPosY = obj.Y;
    //        if ((nOldPosX >= 0 && nOldPosX < Width) == false)
    //            return false;
    //        if ((nOldPosY >= 0 && nOldPosY < Height) == false)
    //            return false;
    //        if ((nNewPosX >= 0 && nNewPosX < Width) == false)
    //            return false;
    //        if ((nNewPosY >= 0 && nNewPosY < Height) == false)
    //            return false;

    //        if (Block(nOldPosX) == Block(nNewPosX) && Block(nOldPosY) == Block(nNewPosY))
    //            return false;

    //        BlockSet(nOldPosX, nOldPosY).RemoveObject<T>(obj);
    //        BlockSet(nNewPosX, nNewPosY).AddObject<T>(obj);

    //        if (obj.ObjType == MapObjectType.Player)
    //            obj.IndexInScreen = CounterMovement.Next;

    //        return true;
    //    }

    //    public bool EnterMap<T>(T obj)
    //        where T : IMapObj
    //    {
    //        if ((obj.X >= 0 && obj.X < Width) == false)
    //            return false;
    //        if ((obj.Y >= 0 && obj.Y < Height) == false)
    //            return false;

    //        BlockSet(obj.X, obj.Y).AddObject<T>(obj);

    //        if (obj.ObjType == MapObjectType.Player)
    //            obj.IndexInScreen = CounterMovement.Next;

    //        return true;
    //    }
    //    public bool LeaveMap<T>(T obj)
    //         where T : IMapObj
    //    {
    //        if (!(obj.X >= 0 && obj.X < Width))
    //            return false;
    //        if (!(obj.Y >= 0 && obj.Y < Height))
    //            return false;

    //        BlockSet(obj.X, obj.Y).RemoveObject<T>(obj);

    //        return true;
    //    }
    //    public IEnumerable<IMapObj> Roles(MapObjectType typ, int X, int Y, Predicate<IMapObj> P = null)
    //    {

    //        for (int x = Math.Max(Block(X) - 1, 0); x <= Block(X) + 1 && x < GetWidthOfBlock(); x++)
    //            for (int y = Math.Max(Block(Y) - 1, 0); y <= Block(Y) + 1 && y < GetHeightOfBlock(); y++)
    //            {
    //                IMapObj[] list = null;
    //                try
    //                {
    //                    list = m_setBlock[x, y].GetObjects(typ).ToArray();
    //                }
    //                catch { continue; }
    //                if (list != null)
    //                {
    //                    for (int i = 0; i < list.Length; i++)
    //                    {
    //                        var element = list[i];
    //                        if (element != null)
    //                        {
    //                            if (P != null)
    //                            {
    //                                if (P(element))
    //                                    yield return element;
    //                            }
    //                            else if (element != null)
    //                                yield return element;
    //                        }
    //                    }
    //                }
    //            }


    //    }
    //    public int CountRoles(MapObjectType typ, int X, int Y)
    //    {
    //        int count = 0;
    //        for (int x = Math.Max(Block(X) - 1, 0); x <= Block(X) + 1 && x < GetWidthOfBlock(); x++)
    //            for (int y = Math.Max(Block(Y) - 1, 0); y <= Block(Y) + 1 && y < GetHeightOfBlock(); y++)
    //            {
    //                var list = m_setBlock[x, y].GetObjects(typ);
    //                count += list.Count;
    //            }
    //        return count;
    //    }
    //    public IEnumerable<IMapObj> GetAllMapRoles(MapObjectType typ, Predicate<IMapObj> P = null)
    //    {
    //        for (int x = 0; x < GetWidthOfBlock(); x++)
    //            for (int y = 0; y < GetHeightOfBlock(); y++)
    //            {
    //                var list = m_setBlock[x, y].GetObjects(typ);
    //                for (int i = 0; i < list.Count; i++)
    //                {
    //                    if (i >= list.Count)
    //                        break;
    //                    var element = list[i];
    //                    if (element != null)
    //                    {
    //                        if (P != null)
    //                        {
    //                            if (P(element))
    //                                yield return element;
    //                        }
    //                        else if (element != null)
    //                            yield return element;
    //                    }
    //                }
    //            }
    //    }
    //    public int GetAllMapRolesCount(MapObjectType typ, Predicate<IMapObj> P = null)
    //    {
    //        return GetAllMapRoles(typ, P).Count();
    //    }
    //    public T GetMapObject<T>(MapObjectType typ, uint UID, Predicate<IMapObj> P = null)
    //    {
    //        foreach (var obj in GetAllMapRoles(typ, P))
    //            if (obj.UID == UID)
    //                return (T)obj;
    //        return default(T);
    //    }
    //    public bool MapContain(MapObjectType typ, uint UID, Predicate<IMapObj> P = null)
    //    {
    //        foreach (var obj in GetAllMapRoles(typ, P))
    //            if (obj.UID == UID)
    //                return true;
    //        return false;
    //    }
    //    public void ClearMap(MapObjectType typ)
    //    {
    //        for (int x = 0; x < GetWidthOfBlock(); x++)
    //            for (int y = 0; y < GetHeightOfBlock(); y++)
    //            {
    //                m_setBlock[x, y].Clear(typ);
    //            }
    //    }
    //    public bool TryGetObject<T>(uint UID, MapObjectType typ, int X, int Y, out T obj)
    //        where T : IMapObj
    //    {
    //        for (int x = Math.Max(Block(X) - 1, 0); x <= Block(X) + 1 && x < GetWidthOfBlock(); x++)
    //            for (int y = Math.Max(Block(Y) - 1, 0); y <= Block(Y) + 1 && y < GetHeightOfBlock(); y++)
    //            {
    //                var list = m_setBlock[x, y];
    //                if (list.TryGetObject<T>(typ, UID, out obj))
    //                    return true;

    //            }
    //        obj = default(T);
    //        return false;
    //    }
    //    public bool Contain(uint UID, int X, int Y)
    //    {
    //        for (int x = Math.Max(Block(X) - 1, 0); x <= Block(X) + 1 && x < GetWidthOfBlock(); x++)
    //            for (int y = Math.Max(Block(Y) - 1, 0); y <= Block(Y) + 1 && y < GetHeightOfBlock(); y++)
    //            {
    //                var list = m_setBlock[x, y];
    //                for (int i = 0; i < (int)MapObjectType.Count; i++)
    //                    if (list.ContainObject((MapObjectType)i, UID))
    //                        return true;

    //            }
    //        return false;
    //    }
    //}
    //public class ViewPtr
    //{
    //    private List<Role.IMapObj>[] Objects;
    //    public ViewPtr()
    //    {
    //        Objects = new List<IMapObj>[(int)MapObjectType.Count];
    //        for (int x = 0; x < (int)MapObjectType.Count; x++)
    //            Objects[x] = new List<IMapObj>();
    //    }


    //    public void AddObject<T>(T obj)
    //         where T : IMapObj
    //    {
    //        try
    //        {
    //            Objects[(int)obj.ObjType].Add(obj);
    //        }
    //        catch { }
    //    }

    //    public void RemoveObject<T>(T obj)
    //        where T : IMapObj
    //    {
    //        try
    //        {
    //            Objects[(int)obj.ObjType].Remove(obj);
    //        }
    //        catch { }
    //    }


    //    public bool ContainObject(MapObjectType obj_t, uint UID)
    //    {
    //        for (int x = 0; x < Objects[(int)obj_t].Count; x++)
    //        {
    //            var list = Objects[(int)obj_t];
    //            if (x >= list.Count)
    //                break;
    //            if (list[x] != null && list[x].UID == UID)
    //                return true;
    //        }
    //        return false;
    //    }

    //    public bool TryGetObject<T>(MapObjectType obj_t, uint UID, out T obj)
    //    {
    //        try
    //        {
    //            for (int x = 0; x < Objects[(int)obj_t].Count; x++)
    //            {
    //                var list = Objects[(int)obj_t];
    //                if (x >= list.Count)
    //                    break;
    //                if (list[x] != null)
    //                {
    //                    if (list[x].UID == UID)
    //                    {
    //                        obj = (T)list[x];
    //                        return true;
    //                    }
    //                }
    //            }
    //        }
    //        catch { }
    //        obj = default(T);
    //        return false;
    //    }
    //    public List<IMapObj> GetObjects(MapObjectType typ)
    //    {
    //        return Objects[(int)typ];
    //    }

    //    public void Clear(MapObjectType typ)
    //    {
    //        Objects[(int)typ].Clear();
    //    }
    //}

    public class Portal
    {
        public ushort MapID { get; set; }
        public ushort X { get; set; }
        public ushort Y { get; set; }

        public ushort Destiantion_MapID { get; set; }
        public ushort Destiantion_X { get; set; }
        public ushort Destiantion_Y { get; set; }
    }
    [Flags]
    public enum MapFlagType : byte
    {
        None = 0,
        Valid = 1 << 0,
        Monster = 1 << 1,
        Item = 1 << 2,
        Player = 1 << 3,
        Npc = 1 << 4

    }
    [Flags]
    public enum MapTypeFlags
    {
        Normal = 0,
        PkField = 1 << 0,
        ChangeMapDisable = 1 << 1,
        RecordDisable = 1 << 2,
        PkDisable = 1 << 3,
        BoothEnable = 1 << 4,
        TeamDisable = 1 << 5,
        TeleportDisable = 1 << 6,
        GuildMap = 1 << 7,
        PrisonMap = 1 << 8,
        FlyDisable = 1 << 9,
        Family = 1 << 10,
        MineEnable = 1 << 11,
        FreePk = 1 << 12,
        NeverWound = 1 << 13,
        DeadIsland = 1 << 14
    }
    public class GameMap
    {
        public void GuildWarItem(int type)
        {
            Tuple<ushort, ushort> tuple = RandomCoordinates();
            ushort newx = tuple.Item1;
            ushort newy = tuple.Item2;
            if (IsFlagPresent(newx, newy, MapFlagType.Item) == false && IsFlagPresent(newx, newy, MapFlagType.Valid))
            {
                var Item = new Game.MsgFloorItem.MsgItem(null, newx, newy, Game.MsgFloorItem.MsgItem.ItemType.Effect, 0, 0, ID, 0, false, this, 60 * 60 * 1000);
                Item.MsgFloor.m_ID = (uint)type;
                Item.MsgFloor.m_Color = 1;
                Item.MsgFloor.Name = "STR_TRAP_ID_" + type + "@@";
                Item.MsgFloor.DropType = Game.MsgFloorItem.MsgDropID.Effect;
                cells[newx, newy] |= MapFlagType.Item;
                View.EnterMap<Role.IMapObj>(Item);
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    Item.SendAll(stream, MsgDropID.Effect);
                }
            }
        }
        public int ContainItem(uint Look)
        {
            int Count = 0;
            foreach (var Item in View.GetAllMapRoles(MapObjectType.Item))
            {
                Game.MsgFloorItem.MsgItem obj = Item as Game.MsgFloorItem.MsgItem;
                if (obj != null)
                {
                    if (obj.MsgFloor.m_ID == Look)
                    {
                        Count++;
                    }
                }
            }
            return Count;
        }
        public Tuple<ushort, ushort> RandomCoordinates()
        {
            int times = 10000;
            int x = Pool.GetRandom.Next(bounds.Width), y = Pool.GetRandom.Next(bounds.Height);
            while (times-- > 0)
            {
                if ((cells[x, y] & MapFlagType.Valid) != MapFlagType.Valid)
                {
                    x = Pool.GetRandom.Next(bounds.Width);
                    y = Pool.GetRandom.Next(bounds.Height);
                }
                else break;
            }
            return new Tuple<ushort, ushort>((ushort)x, (ushort)y);
        }
        public static object EnterObj = new object();
        public uint RecordSteedRace = 0;

        public static sbyte[] XDir = new sbyte[]
        {
            -1, -2, -2, -1, 1, 2, 2, 1,
             0, -2, -2, -2, 0, 2, 2, 2,
            -1, -2, -2, -1, 1, 2, 2, 1,
             0, -1, -1, -1, 0, 1, 1, 1,
        };
        public static sbyte[] YDir = new sbyte[]
        {
            2,  1, -1, -2, -2, -1, 1, 2,
            2,  2,  0, -2, -2, -2, 0, 2,
            2,  1, -1, -2, -2, -1, 1, 2,
            1,  1,  0, -1, -1, -1, 0, 1
        };

        public static bool IsGate(uint UID)
        {
            return UID == 516076 || UID == 516077 || UID == 516074 || UID == 516075 || UID == 516078 || UID == 516079 || UID == 516080;
        }
        public static bool IsFrozengrotoMaps(uint Map)
        {
            return Map == 1762 || Map == 1927 || Map == 1999 || Map == 2054 || Map == 2055 || Map == 2056;
        }
        public void Pushback(ref uint x, ref uint y, Role.Flags.ConquerAngle angle, int paces)
        {
            sbyte xi = 0, yi = 0;
            for (int i = 0; i < paces; i++)
            {
                switch (angle)
                {
                    case Role.Flags.ConquerAngle.North: xi = -1; yi = -1; break;
                    case Role.Flags.ConquerAngle.South: xi = 1; yi = 1; break;
                    case Role.Flags.ConquerAngle.East: xi = 1; yi = -1; break;
                    case Role.Flags.ConquerAngle.West: xi = -1; yi = 1; break;
                    case Role.Flags.ConquerAngle.NorthWest: xi = -1; break;
                    case Role.Flags.ConquerAngle.SouthWest: yi = 1; break;
                    case Role.Flags.ConquerAngle.NorthEast: yi = -1; break;
                    case Role.Flags.ConquerAngle.SouthEast: xi = 1; break;
                }
                if (!ValidLocation((ushort)(x + xi), (ushort)(y + yi))) break;
                x = (ushort)(x + xi);
                y = (ushort)(y + yi);
            }
        }
        private static List<ushort> UsingMaps = new List<ushort>()
        {
            601,//offlineTG
            9855,//arena map, lotery map
            1000,//Desert
            1001,//MysticCastel
            1002,//TwinCity
            1004,//Prommoter
            1005,//Arena
            1006,//Steeding TC
            1008,//color you armors/heah
            1010,//bird vilage
            1011,//PhoenixCastle
            1013,//HalkingCave
            1015,//BirdIsland
            1020,//ApeMoutain
            1036,//Market
            //1039,//TrainingGrounds
            1511,//buy mobila
            1038,//GuildWar
            2068,//elitepk map
            6001,//GuildWarJaill
            1098,1099,2080,601,3024,//house id`s
            1351,1352,1353,1354,//lab`s
            1762,//fg1
            1927,//fg2
            1999,//fg3
            2054,//fg4
            2055,//fg5
            2056,//fg6
            3053,//roulette
            3846,//Nemesys Map
            1700,//2nd reborn quest !!!
            3851,//epic ninja quest
            3055,//first map nemesys
            3056,//pestera
            3846,//nemesys map
            1039,//
            6000,//jail
            3825,//trojan epic quest
            2057,
             1993,
            10364,
            Game.MsgTournaments.MsgClassPKWar.MapID,
            Game.MsgTournaments.MsgEliteGroup.WaitingAreaID
        };

        public List<Portal> Portals = new List<Portal>();

        public unsafe void SendSysMesage(string Messaj, Game.MsgServer.MsgMessage.ChatMode ChatType = Game.MsgServer.MsgMessage.ChatMode.TopLeft
           , Game.MsgServer.MsgMessage.MsgColor color = Game.MsgServer.MsgMessage.MsgColor.red)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                var Packet = new Game.MsgServer.MsgMessage(Messaj, color, ChatType).GetArray(stream);
                foreach (var kv in Pool.GamePoll) // [perf] live lock-free enumeration (no .Values snapshot)
                {
                    var client = kv.Value;
                    if (client.Player.Map == ID)
                        client.Send(Packet);
                }
            }
        }

        public string Name = "";

        public uint BaseID = 0;
        public MapFlagType[,] cells { get; set; }
        public System.Drawing.Size bounds;
        public Game.MsgMonster.MobCollection MonstersColletion;

        public MapView View;

        public bool AddStaticRole(StaticRole role)
        {
            if (View.EnterMap<StaticRole>(role))
            {
                SetFlagNpc(role.X, role.Y);
                return true;
            }
            return false;
        }
        public bool RemoveStaticRole(Role.IMapObj obj)
        {

            if (View.LeaveMap<Role.IMapObj>(obj))
            {
                RemoveFlagNpc(obj.X, obj.Y);
                return true;
            }
            return false;
        }

        public Game.MsgNpc.Npc Magnolia = null;
        public void AddMagnolia(ServerSockets.Packet stream, uint Quality)
        {
            bool Location = false;

            if (Magnolia != null)
            {
                if (Magnolia.X == 99)
                    Location = true;
                RemoveNpc(Magnolia, stream);
            }
            Magnolia = Game.MsgNpc.Npc.Create();
            if (Location)
            {
                Magnolia.UID = 999900;
                Magnolia.X = 106;
                Magnolia.Y = 99;
            }
            else
            {
                Magnolia.UID = 999901;
                Magnolia.X = 99;
                Magnolia.Y = 112;
            }
            Magnolia.ObjType = MapObjectType.Npc;
            Magnolia.NpcType = Flags.NpcType.Talker;
            uint mesh = 0;
            if (Quality % 10 == 7)
                mesh = 10;
            else if (Quality % 10 == 8)
                mesh = 20;
            if (Quality % 10 == 9)
                mesh = 30;
            if (Quality % 10 == 0)
                mesh = 40;
            Magnolia.Mesh = (ushort)(19340 + mesh);
            Magnolia.Map = this.ID;
            AddNpc(Magnolia);
        }



        public void GenerateSectorTraps(ushort x, ushort y, int type)
        {
            if (View.CountRoles(MapObjectType.Item, x, y) < 6)
            {
                ushort newx = (ushort)Pool.GetRandom.Next(1, 18);
                ushort newy = (ushort)Pool.GetRandom.Next(1, 18);
                newx += x;
                newy += y;
                if (IsFlagPresent(newx, newy, MapFlagType.Item) == false && IsFlagPresent(newx, newy, MapFlagType.Valid))
                {
                    var Item = new Game.MsgFloorItem.MsgItem(null, newx, newy, Game.MsgFloorItem.MsgItem.ItemType.Effect, 0, 0, ID, 0, false, this, 60 * 60 * 1000);
                    Item.MsgFloor.m_ID = (uint)type;
                    Item.MsgFloor.m_Color = 2;
                    Item.MsgFloor.DropType = Game.MsgFloorItem.MsgDropID.Effect;
                    cells[newx, newy] |= MapFlagType.Item;
                    View.EnterMap<Role.IMapObj>(Item);


                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        Item.SendAll(stream, MsgDropID.Effect);
                    }
                }
            }
        }
        public void RemoveTrap(ushort x, ushort y, Role.IMapObj item)
        {


            cells[item.X, item.Y] &= ~MapFlagType.Item;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                var ittem = item as Game.MsgFloorItem.MsgItem;
                ittem.SendAll(stream, item.IsTrap() ? Game.MsgFloorItem.MsgDropID.RemoveEffect : Game.MsgFloorItem.MsgDropID.Remove);
                View.LeaveMap<Role.IMapObj>(item);
            }

        }
        public ConcurrentDictionary<uint, Game.MsgNpc.Npc> soldierRemains = new ConcurrentDictionary<uint, Game.MsgNpc.Npc>();
        public void CheckUpSoldierReamins()
        {
            List<Game.MsgNpc.Npc> remove = new List<Game.MsgNpc.Npc>();
            foreach (var npc in soldierRemains.Values)
            {
                if (ID == 1000)
                {
                    if (DateTime.Now > npc.Respawn)
                    {
                        npc.X = (ushort)Pool.GetRandom.Next(624 - 32, 624 + 32);
                        npc.Y = (ushort)Pool.GetRandom.Next(477 - 32, 477 + 32);
                        AddNpc(npc);
                        remove.Add(npc);
                    }
                }
                else if (ID == 1015)
                {
                    if (npc.UID == 8551)
                    {
                        npc.X = (ushort)Pool.GetRandom.Next(551 - 32, 551 + 32);
                        npc.Y = (ushort)Pool.GetRandom.Next(342 - 32, 342 + 32);
                        AddNpc(npc);
                        remove.Add(npc);
                    }
                    else
                    {
                        npc.X = (ushort)Pool.GetRandom.Next(454 - 90, 454 + 90);
                        npc.Y = (ushort)Pool.GetRandom.Next(574 - 90, 574 + 90);
                        AddNpc(npc);
                        remove.Add(npc);
                    }
                }
            }
            foreach (var npc in remove)
            {
                Game.MsgNpc.Npc rem;
                soldierRemains.TryRemove(npc.UID, out rem);
            }
        }

        public void AddNpc(Game.MsgNpc.Npc npc)
        {
            // [InstanceNpc] only an NPC with the same id on the same DynamicID is "already there" (a quest-instance
            // copy of the same id on another DynamicID must not block / remove the base-map NPC)
            uint dyn = npc.DynamicID;
            if (!View.MapContain(MapObjectType.Npc, npc.UID, p => p.DynamicID == dyn))
            {
                View.EnterMap<Role.IMapObj>(npc);
                SetFlagNpcCell(npc.X, npc.Y);// [NpcBlock] like official: a talking NPC blocks only its own cell
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    npc.Send(stream);
                }
            }
            else
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var streamm = rec.GetStream();
                    RemoveNpc(npc, streamm);
                }
                if (!View.MapContain(MapObjectType.Npc, npc.UID))
                {
                    View.EnterMap<Role.IMapObj>(npc);
                    SetFlagNpcCell(npc.X, npc.Y);// [NpcBlock]
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        npc.Send(stream);
                    }
                }
            }
        }
        public unsafe void RemoveNpc(SobNpc npc, ServerSockets.Packet stream)
        {
            if (View.MapContain(MapObjectType.SobNpc, npc.UID))
            {
                View.LeaveMap<Role.IMapObj>(npc);
                RemoveFlagNpc(npc.X, npc.Y);
                ActionQuery action;
                action = new ActionQuery()
                {
                    ObjId = npc.UID,
                    Type = ActionType.RemoveEntity
                };
                foreach (var client in View.Roles(MapObjectType.Player, npc.X, npc.Y))
                {
                    if (Core.GetDistance(client.X, client.Y, npc.X, npc.Y) <= Game.MsgNpc.Npc.SeedDistance)
                    {
                        client.Send(stream.ActionCreate(action));
                    }
                }
            }

        }
        public unsafe void RemoveNpc(Game.MsgNpc.Npc npc, ServerSockets.Packet stream)
        {
            if (View.MapContain(MapObjectType.Npc, npc.UID))
            {
                View.LeaveMap<Role.IMapObj>(npc);
                RemoveFlagNpcCell(npc.X, npc.Y);// [NpcBlock]


                ActionQuery action;

                action = new ActionQuery()
                {
                    ObjId = npc.UID,
                    Type = ActionType.RemoveEntity
                };

                foreach (var client in View.Roles(MapObjectType.Player, npc.X, npc.Y))
                {
                    if (Core.GetDistance(client.X, client.Y, npc.X, npc.Y) <= Game.MsgNpc.Npc.SeedDistance)
                    {
                        client.Send(stream.ActionCreate(action));
                    }
                }
            }

        }
        public bool ValidLocation(ushort X, ushort Y)
        {
            if (bounds.Width > X && this.bounds.Height > Y)
            {
                return (cells[X, Y] & MapFlagType.Valid) == MapFlagType.Valid;
            }
            return false;
        }
        public bool MonsterOnTile(ushort X, ushort Y)
        {
            if (bounds.Width > X && this.bounds.Height > Y)
            {
                return (cells[X, Y] & MapFlagType.Monster) == MapFlagType.Monster;
            }
            return false;
        }
        public void SetMonsterOnTile(ushort X, ushort Y, bool Value)
        {
            try
            {
                if (Value)
                    cells[X, Y] |= MapFlagType.Monster;
                else
                    cells[X, Y] &= ~MapFlagType.Monster;
            }
            catch (Exception)
            {

                MyConsole.WriteLine("Problem monsters on map " + ID.ToString(), ConsoleColor.DarkRed);
            }
        }
        public void RemoveTrapGuild(ushort x, ushort y, Role.IMapObj item)
        {

            cells[item.X, item.Y] &= ~MapFlagType.Item;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                var ittem = item as Game.MsgFloorItem.MsgItem;
                ittem.SendAll(stream, ittem.IsTrap() ? Game.MsgFloorItem.MsgDropID.RemoveEffect : Game.MsgFloorItem.MsgDropID.Remove);
                ittem.MsgFloor.DropType = ittem.IsTrap() ? MsgDropID.RemoveEffect : MsgDropID.Remove;
                ittem.MsgFloor.SendScreen(stream.ItemPacketCreate(ittem.MsgFloor));
                if (item.Map == 1002)
                    View.LeaveMap<Role.IMapObj>(item);
            }


        }
        /// <summary>[InstanceNpc] like SearchNpcInScreen, but an NPC that lives in a private copy (DynamicID != 0) answers
        /// only a player on that same DynamicID; the copy on the player's own DynamicID wins over a base-map NPC with
        /// the same id. Normal NPCs (DynamicID 0) keep answering everyone, as before.</summary>
        public bool SearchNpcInScreen(uint UID, ushort X, ushort Y, uint dynamicId, out Game.MsgNpc.Npc obj)
        {
            Game.MsgNpc.Npc fallback = null;
            foreach (var o in View.Roles(MapObjectType.Npc, X, Y, p => p.UID == UID))
            {
                var n = o as Game.MsgNpc.Npc;
                if (n == null || Core.GetDistance(X, Y, n.X, n.Y) >= Game.MsgNpc.Npc.SeedDistance) continue;
                if (n.DynamicID == dynamicId || n.AllowDynamic) { obj = n; return true; }
                if (n.DynamicID == 0 && fallback == null) fallback = n;
            }
            obj = fallback;
            return fallback != null;
        }
        public bool SearchNpcInScreen(uint UID, ushort X, ushort Y, out Game.MsgNpc.Npc obj)
        {
            if (View.TryGetObject<Game.MsgNpc.Npc>(UID, MapObjectType.Npc, X, Y, out obj))
            {
                return Core.GetDistance(X, Y, obj.X, obj.Y) < Game.MsgNpc.Npc.SeedDistance;
            }
            obj = default(Game.MsgNpc.Npc);
            return false;
        }


        public uint ID { get; private set; }
        public GameMap(int width, int height, int m_id)
        {
            Clients = new ConcurrentDictionary<uint, Client.GameClient>();
            this.cells = new MapFlagType[width, height];
            this.bounds = new System.Drawing.Size(width, height);

            this.ID = (uint)m_id;
        }

        public static System.Counter DinamicIDS = new System.Counter(10000001);

        public uint GenerateDynamicID()
        {
            return DinamicIDS.Next;
        }
        public ushort Reborn_Map = 0;
        public ushort Reborn_X = 0;
        public ushort Reborn_Y = 0;

        public static Dictionary<int, string> MapContents = new Dictionary<int, string>();
        public static bool CheckMap(uint ID)
        {
            if (!Pool.ServerMaps.ContainsKey(ID))
            {
                try
                {
                    if (MapContents.ContainsKey((int)ID))
                        return LoadMap((int)ID, MapContents[(int)ID]);
                    else return false;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    MyConsole.WriteLine("MapID = " + ID + " not exist.", ConsoleColor.DarkRed);
                    return false;
                }
            }
            else return true;
        }
        public ulong TypeStatus { get; set; }
        /// <summary>The map whose DMap a server map id uses (the id itself when it has no remap).</summary>
        public static uint ResolveBase(uint baseID)
        {
            { uint fmDoc; if (FestivalMaps.FestivalMapTable.TryResolve(baseID, out fmDoc)) return fmDoc; } // official festival map id
            return MapBases.Resolve(baseID);// Data\map_bases.txt
        }
        public static void EnterMap(int id)
        {
            try
            {
                if (Pool.ServerMaps.Base.ContainsKey((uint)id) || id == 0) return;
                uint baseID = ResolveBase((uint)id);

                var mlog = new MapLoadLog(id); // [perf] زمن كل مرحلة -> mapload_log.txt
                if (MapContents.ContainsKey((int)baseID) && LoadMap(id, MapContents[(int)baseID], id != baseID ? baseID : 0))
                {
                    mlog.Mark("dmap");
                    Server.LoadMapName((uint)id);
                    mlog.Mark("name");
                    Database.NpcServer.LoadNpcs((uint)id);
                    mlog.Mark("npcs");
                    Database.NpcServer.LoadSobNpcs((uint)id);
                    mlog.Mark("sob");
                    Server.LoadPortals((uint)id);
                    mlog.Mark("portals");
                    Database.NpcServer.LoadServerTraps((uint)id);
                    mlog.Mark("traps");

                    if (id == 1002)
                        Server.LoadMapMonsters("Monsters1002.txt");
                    else if (id == 1020)
                        Server.LoadMapMonsters("Monsters1020.txt");
                    else if (id == 1011)
                        Server.LoadMapMonsters("Monsters1011.txt");
                    else if (id == 15757)
                        Server.LoadMonsters((uint)id);
                    else if (id == 1768)
                        Server.LoadMapMonsters("Monsters1768.txt");
                    else if (id == 4460)
                        Server.LoadMapMonsters("GameServer[EU]Monster.txt");
                    else if (id == 10005)
                        Server.LoadMapMonsters("Monsters10005.txt");
                    else if (id == 10006)
                        Server.LoadMapMonsters("Monsters10006.txt");
                    else if (id == 10007)
                        Server.LoadMapMonsters("Monsters10007.txt");
                    else if (id == 3819)
                        Server.LoadMapMonsters("Monsters3819.txt");
                    else if (id == 10008)
                        Server.LoadMapMonsters("Monsters10008.txt");
                    else if (id == 10040)
                        Server.LoadMapMonsters("Monsters10040.txt");
                    else if (id == 10041)
                        Server.LoadMapMonsters("Monsters10041.txt");
                    else if (id == 10042)
                        Server.LoadMapMonsters("Monsters10042.txt");
                    else if (id == 10043)
                        Server.LoadMapMonsters("Monsters10043.txt");
                    else if (id == 10044)
                        Server.LoadMapMonsters("Monsters10044.txt");
                    else if (id == 10045)
                        Server.LoadMapMonsters("Monsters10045.txt");
                    else if (id == 10020)
                        Server.LoadMapMonsters("Monsters10020.txt");
                    else if (id == 1000)
                        Server.LoadMapMonsters("Monsters1000.txt");
                    else if (id == 1779)
                        Server.LoadMapMonsters("Monsters1779.txt");
                    else if (id == 1780)
                        Server.LoadMapMonsters("Monsters1780.txt");
                    else if (id == 3935)
                        Server.LoadMapMonsters("Monsters3935.txt");
                    else if (id == 10137)
                        Server.LoadMapMonsters("Monsters10137.txt");
                    else if (id == 3998)
                        Server.LoadMapMonsters("Monsters3998.txt");
                    else if (id == 10089)
                        Server.LoadMapMonsters("Monsters10089.txt");
                    else if (id == 1015)
                        Server.LoadMapMonsters("Monsters1015.txt");
                    else if (id == 1016)
                        Server.LoadMapMonsters("Monsters1016.txt");
                    else if (id == 3845)
                        Server.LoadMapMonsters("Monsters3845.txt");
                    else if (id == 10250)
                        Server.LoadMapMonsters("Monsters10250.txt");

                    else Server.LoadMonsters((uint)id);

                    Server.LoadMyMonsters((uint)id);
                    //  if (id == 16785 || id == 16786 || id == 3860 || id == 15757 || id == 15758 || id == 15759 || id == 15760 || id == 15761 || id == 15762 || id == 15763 || id == 15764 || id == 15765 || id == 15766 || id == 15767 || id == 15768)
                    Server.LoadNewMonsters((uint)id);
                    mlog.Mark("monsters");
                }
                // [perf] كان GC.Collect() هنا: GC كامل بيوقّف كل الثريدات (~21ms على heap 150MB). الـ runtime بيجمع لوحده.
                mlog.Mark("gc");
                mlog.Done();
            }
            catch (Exception e) { MyConsole.WriteException(e); }
        }
        public static void LoadMaps()
        {
            using (var gamemap = new BinaryReader(new FileStream(Path.Combine(Program.ServerConfig.CO2Folder, "ini/GameMap.dat"), FileMode.Open)))
            {
                var amount = gamemap.ReadInt32();
                for (var i = 0; i < amount; i++)
                {

                    var id = gamemap.ReadInt32();
                    var fileName = Encoding.ASCII.GetString(gamemap.ReadBytes(gamemap.ReadInt32()));
                    var puzzleSize = gamemap.ReadInt32();
                    MapContents[id] = fileName.Replace(".7z", ".dmap");
                }
            }
        }
        public uint MapColor = 0;
        public int[,] FloorType;
        public int[,] Altitude;
        public static bool LoadMap(int id, string mapFile, uint baseid = 0)
        {
            try
            {
                lock (EnterObj)
                {
                    GameMap ourInst;
                    // [perf] الملف كله بيتقري مرة واحدة ويتفك من الذاكرة (كان BinaryReader بـ 3 استدعاءات لكل خلية). نفس القيم
                    // بنفس الترتيب: 268 بايت header، width، height، وبعدين لكل خلية 3 int16 ولكل صف int32 زيادة.
                    byte[] dmap = MapFiles.Read(mapFile);
                    {
                        int pos = 268;
                        int mapW = BitConverter.ToInt32(dmap, pos); pos += 4;
                        int mapH = BitConverter.ToInt32(dmap, pos); pos += 4;
                        ourInst = new GameMap(mapW, mapH, id);
                        ourInst.MonstersColletion = new Game.MsgMonster.MobCollection((uint)id);
                        ourInst.View = new MapView(ourInst.bounds.Width, ourInst.bounds.Height);
                        ourInst.MonstersColletion = new Game.MsgMonster.MobCollection((uint)id);
                        ourInst.BaseID = baseid;
                        if (id == 1038) ourInst.FloorType = new int[ourInst.bounds.Width, ourInst.bounds.Height];
                        ourInst.Altitude = new int[ourInst.bounds.Width, ourInst.bounds.Height];

                        int needed = pos + ourInst.bounds.Height * (ourInst.bounds.Width * 6 + 4);
                        if (dmap.Length < needed)
                            throw new EndOfStreamException("dmap too short: " + mapFile);
                        bool floor = id == 1038;
                        for (int y = 0; y < ourInst.bounds.Height; y++)
                        {
                            for (int x = 0; x < ourInst.bounds.Width; x++)
                            {
                                ourInst.cells[x, y] = ((BitConverter.ToInt16(dmap, pos) & 1) == 0) ? MapFlagType.Valid : MapFlagType.None; // [DmapAccess 2026-10-15] 3D maps use access 4 for walkable/jumpable cells (bit0 = blocked); the client lets players land on them
                                if (floor) ourInst.FloorType[x, y] = BitConverter.ToInt16(dmap, pos + 2);
                                ourInst.Altitude[x, y] = BitConverter.ToInt16(dmap, pos + 4);
                                pos += 6;
                            }
                            pos += 4;
                        }
                    }
                    int info = baseid != 0 ? (int)baseid : (int)id;

                    if (File.Exists(Program.ServerConfig.DbLocation + "maps\\" + info + ".ini"))
                    {
                        WindowsAPI.IniFile reader = new WindowsAPI.IniFile("\\maps\\" + info + ".ini");
                        ourInst.TypeStatus = reader.ReadUInt64("info", "type", 0);
                        ourInst.Reborn_X = reader.ReadUInt16("info", "portal0_x", 0);
                        ourInst.Reborn_Y = reader.ReadUInt16("info", "portal0_y", 0);
                        ourInst.Reborn_Map = reader.ReadUInt16("info", "reborn_map", 0);
                        ourInst.RecordSteedRace = reader.ReadUInt16("info", "race_record", 0);
                        ourInst.MapColor = reader.ReadUInt32("info", "color", 0);
                    }
                    Pool.ServerMaps.Add((uint)id, ourInst);
                    return true;
                }
            }
            catch (FileNotFoundException)
            {
                MyConsole.WriteLine("\tMap not found: " + id + " - " + mapFile + "");
            }
            catch (Exception e)
            {
                MyConsole.WriteException(e);
            }
            return false;
        }
        private bool Update = false;
        private Client.GameClient[] Users = new Client.GameClient[0];
        public Client.GameClient[] Values
        {
            get
            {
                if (Update)
                {
                    Users = Clients.Values.ToArray();
                    Update = false;
                }
                return Users;
            }
            set { }
        }
        private ConcurrentDictionary<uint, Client.GameClient> Clients;
        public void Enquer(Client.GameClient client)
        {
            if (Clients.TryAdd(client.Player.UID, client))
            {
                View.EnterMap<Role.IMapObj>(client.Player);
                client.Map = this;
                Update = true;
            }
        }
        public void Denquer(Client.GameClient client)
        {
            Client.GameClient aclient;
            if (Clients.TryRemove(client.Player.UID, out aclient))
            {
                View.LeaveMap<Role.IMapObj>(client.Player);

                Update = true;
            }
        }
        // [NpcBlock] official Conquer: an ordinary (talking) NPC occupies only its own cell, so players can walk
        // right next to it. The 5x5 block of SetFlagNpc is kept for SobNpcs/static roles (gates, poles, statues).
        public void SetFlagNpcCell(ushort x, ushort y)
        {
            if (!(x > 0 && y > 0 && x < bounds.Width && y < bounds.Height)) return;
            cells[x, y] = MapFlagType.Npc;
        }
        public void RemoveFlagNpcCell(ushort x, ushort y)
        {
            if (!(x > 0 && y > 0 && x < bounds.Width && y < bounds.Height)) return;
            if (cells[x, y] == MapFlagType.Npc) cells[x, y] = MapFlagType.Valid;
        }
        public void SetFlagNpc(ushort x, ushort y)
        {
            if (!(x > 0 && y > 0 && x < bounds.Width && y < bounds.Height)) return;
            cells[x, y] = MapFlagType.Npc;

            ushort limy = (ushort)Math.Min(this.bounds.Height - 2, y + 2);
            ushort limx = (ushort)Math.Min(this.bounds.Width - 2, x + 2);
            ushort xstart = (ushort)Math.Max(x - 2, 0);

            for (ushort ay = (ushort)Math.Max(y - 2, 0); ay <= limy; ay++)
            {
                for (ushort ax = xstart; ax <= limx; ax++)
                {
                    cells[ax, ay] = MapFlagType.Npc;
                }
            }
        }
        public void SetGateFlagNpc(ushort x, ushort y)
        {
            if (!(x > 0 && y > 0 && x < bounds.Width && y < bounds.Height)) return;
            cells[x, y] = MapFlagType.None;

            ushort limy = (ushort)Math.Min(this.bounds.Height - 2, y + 2);
            ushort limx = (ushort)Math.Min(this.bounds.Width - 2, x + 2);
            ushort xstart = (ushort)Math.Max(x - 2, 0);

            for (ushort ay = (ushort)Math.Max(y - 2, 0); ay <= limy; ay++)
            {
                for (ushort ax = xstart; ax <= limx; ax++)
                {
                    cells[ax, ay] = MapFlagType.None;
                }
            }
        }
        public void RemoveFlagNpc(ushort x, ushort y)
        {
            if (!(x > 0 && y > 0 && x < bounds.Width && y < bounds.Height)) return;
            cells[x, y] = MapFlagType.Valid;

            ushort limy = (ushort)Math.Min(this.bounds.Height - 1, y + 1);
            ushort limx = (ushort)Math.Min(this.bounds.Width - 1, x + 1);
            ushort xstart = (ushort)Math.Max(x - 1, 0);

            for (ushort ay = (ushort)Math.Max(y - 1, 0); ay <= limy; ay++)
            {
                for (ushort ax = xstart; ax <= limx; ax++)
                {
                    cells[ax, ay] = MapFlagType.Valid;
                }
            }
        }
        public bool ContainMobID(uint ID, uint Dynamic = 0, int count = 1)
        {
            foreach (var monster in View.GetAllMapRoles(MapObjectType.Monster))
            {
                var mob = monster as Game.MsgMonster.MonsterRole;
                if (mob.Family != null)
                    if (mob.Family.ID == ID && mob.Alive)
                    {
                        if (Dynamic == 0)
                            count--;
                        else if (Dynamic == monster.DynamicID)
                            count--;
                    }
            }
            return count <= 0;
        }

        public object SyncRoot = new object();
        public void GetRandCoord(ref ushort x, ref ushort y)
        {
            lock (SyncRoot)
            {
                // [hang-fix] cap attempts: a map with no (or almost no) Valid cell used to spin this
                // do-while FOREVER while holding SyncRoot, freezing the whole map for every thread.
                int times = 10000;
                do
                {
                    x = (ushort)Pool.GetRandom.Next(20, (ushort)(bounds.Width - 1));
                    y = (ushort)Pool.GetRandom.Next(20, (ushort)(bounds.Height - 1));
                }
                while ((cells[x, y] & MapFlagType.Valid) != MapFlagType.Valid && --times > 0);
            }
        }
        public void GetRandCoord(ref ushort _x, ref ushort _y, int radius)
        {
            int times = 10000;
            lock (SyncRoot)
            {
                _x = (ushort)Pool.GetRandom.Next(_x - radius, _x + radius);
                _y = (ushort)Pool.GetRandom.Next(_y - radius, _y + radius);
                while (times-- > 0)
                {
                    if ((cells[_x, _y] & MapFlagType.Valid) != MapFlagType.Valid)
                    {
                        _x = (ushort)Pool.GetRandom.Next(_x + (radius * -1), _x + radius);
                        _y = (ushort)Pool.GetRandom.Next(_y + (radius * -1), _y + radius);
                    }
                    else break;
                }
            }
        }

        public bool IsFlagPresent(int x, int y, MapFlagType flag)
        {
            if (x > 0 && y > 0 && x < bounds.Width && y < bounds.Height)
                return (cells[x, y] & flag) == flag;
            return false;
        }
        public bool EnqueueItem(Game.MsgFloorItem.MsgItem item)
        {
            return View.EnterMap<Role.IMapObj>(item);
        }
        public bool IsValidFlagNpc(ushort x, ushort y)
        {
            ushort limy = (ushort)Math.Min(this.bounds.Height - 1, y + 1);
            ushort limx = (ushort)Math.Min(this.bounds.Width - 1, x + 1);
            ushort xstart = (ushort)Math.Max(x - 1, 0);

            for (ushort ay = (ushort)Math.Max(y - 1, 0); ay <= limy; ay++)
            {
                for (ushort ax = xstart; ax <= limx; ax++)
                {
                    if (!this.IsFlagPresent(x, y, MapFlagType.Valid))
                        return false;
                }
            }
            return true;
        }
        public bool AddGuildTeleporterItem(ref ushort x, ref ushort y)
        {
            if (IsValidFlagNpc(x, y))
            {
                ushort limy = (ushort)Math.Min(this.bounds.Height - 6, y + 6);
                ushort limx = (ushort)Math.Min(this.bounds.Width - 6, x + 6);
                ushort xstart = (ushort)Math.Max(x - 6, 0);
                ushort ystart = (ushort)Math.Max(y - 6, 0);

                for (ushort ay = ystart; ay <= limy; ay++)
                {
                    for (ushort ax = xstart; ax <= limx; ax++)
                    {
                        if (IsValidFlagNpc(ax, ay))
                        {
                            x = ax;
                            y = ay;

                            cells[ax, ay] |= MapFlagType.Item;

                            return true;
                        }
                    }
                }
                x = 0;
                y = 0;
                return false;
            }

            cells[x, y] |= MapFlagType.Item;
            return true;
        }
        public bool AddGroundItem(ref ushort x, ref ushort y, byte Range = 0)
        {
            if (this.IsFlagPresent(x, y, MapFlagType.Item) || !this.IsFlagPresent(x, y, MapFlagType.Valid))
            {
                ushort limy = (ushort)Math.Min(this.bounds.Height - (1 + Range), y + (1 + Range));
                ushort limx = (ushort)Math.Min(this.bounds.Width - (1 + Range), x + (1 + Range));
                ushort xstart = (ushort)Math.Max(x - (1 + Range), 0);
                ushort ystart = (ushort)Math.Max(y - (1 + Range), 0);

                for (ushort ay = ystart; ay <= limy; ay++)
                {
                    for (ushort ax = xstart; ax <= limx; ax++)
                    {
                        if (!this.IsFlagPresent(ax, ay, MapFlagType.Item))
                        {
                            if (this.IsFlagPresent(ax, ay, MapFlagType.Valid))
                            {
                                x = ax;
                                y = ay;

                                cells[ax, ay] |= MapFlagType.Item;

                                return true;
                            }
                        }
                    }
                }
                x = 0;
                y = 0;
                return false;
            }

            cells[x, y] |= MapFlagType.Item;
            return true;
        }
        public unsafe void AddMapMonster(ServerSockets.Packet stream, uint ID, ushort x, ushort y, ushort max_x, ushort max_y, byte count, uint DinamicID = 0, bool RemoveOnDead = false
            , Game.MsgFloorItem.MsgItemPacket.EffectMonsters m_effect = Game.MsgFloorItem.MsgItemPacket.EffectMonsters.None, string streffect = "")
        {
            if (MonstersColletion == null)
            {
                MonstersColletion = new Game.MsgMonster.MobCollection(ID);
            }
            if (MonstersColletion.ReadMap())
            {

                Game.MsgMonster.MonsterFamily famil;
                if (Pool.MonsterFamilies.TryGetValue(ID, out famil))
                {
                    Game.MsgMonster.MonsterFamily Monster = famil.Copy();

                    Monster.SpawnX = x;
                    Monster.SpawnY = y;
                    Monster.MaxSpawnX = (ushort)(x + max_x);
                    Monster.MaxSpawnY = (ushort)(y + max_y);
                    Monster.MapID = ID;
                    Monster.SpawnCount = count;
                    Game.MsgMonster.MonsterRole rolemonster = MonstersColletion.Add(Monster, RemoveOnDead, DinamicID, true);

                    if (rolemonster == null)
                    {
                        // MyConsole.WriteLine("[Error] Add Map Monster (" + ID.ToString() + ")", ConsoleColor.Magenta);
                        return;
                    }
                    //   View.EnterMap<Role.IMapObj>(rolemonster);

                    Game.MsgServer.ActionQuery action = new Game.MsgServer.ActionQuery()
                    {
                        ObjId = rolemonster.UID,
                        Type = Game.MsgServer.ActionType.ReviveMonster,
                        PositionX = rolemonster.X,
                        PositionY = rolemonster.Y
                    };
                    rolemonster.Send(stream.ActionCreate(action));
                    rolemonster.Send(rolemonster.GetArray(stream, false));

                    if (streffect != null)
                    {
                        rolemonster.SendString(stream, MsgStringPacket.StringID.Effect, streffect);
                    }



                    if (m_effect != Game.MsgFloorItem.MsgItemPacket.EffectMonsters.None && rolemonster != null)
                    {
                        Game.MsgFloorItem.MsgItemPacket effect = Game.MsgFloorItem.MsgItemPacket.Create();
                        effect.m_UID = (uint)m_effect;
                        effect.m_X = rolemonster.X;
                        effect.m_Y = rolemonster.Y;
                        effect.DropType = MsgDropID.Earth;
                        rolemonster.Send(stream.ItemPacketCreate(effect));
                        rolemonster.SendString(stream, Game.MsgServer.MsgStringPacket.StringID.Effect, "glebesword");
                    }
                    if (rolemonster.HitPoints > 65535)
                    {
                        Game.MsgServer.MsgUpdate Upd = new Game.MsgServer.MsgUpdate(stream, rolemonster.UID, 1);
                        stream = Upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.MaxHitpoints, rolemonster.Family.MaxHealth);
                        stream = Upd.GetArray(stream);
                        rolemonster.Send(stream);
                        stream = Upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.Hitpoints, rolemonster.HitPoints);
                        stream = Upd.GetArray(stream);
                        rolemonster.Send(stream);
                    }
                }
            }
        }
        public unsafe bool AddFloor(ServerSockets.Packet stream, uint ID, ushort x, ushort y, ushort spelllevel, Database.MagicType.Magic dbspell, Client.GameClient Owner, uint GuildID, uint OwnerUID, uint DinamicID = 0, string Name = "", bool RemoveOnDead = true)
        {
            try
            {
                if (MonstersColletion == null)
                {
                    MonstersColletion = new Game.MsgMonster.MobCollection(ID);
                }
                if (MonstersColletion.ReadMap())
                {

                    Game.MsgMonster.MonsterFamily famil;
                    if (Pool.MonsterFamilies.TryGetValue(1, out famil))
                    {
                        Game.MsgMonster.MonsterFamily Monster = famil.Copy();

                        Monster.SpawnX = x;
                        Monster.SpawnY = y;
                        Monster.MaxSpawnX = (ushort)(x + 1);
                        Monster.MaxSpawnY = (ushort)(y + 1);
                        Monster.MapID = ID;
                        Monster.SpawnCount = 1;
                        Game.MsgMonster.MonsterRole rolemonster = MonstersColletion.Add(Monster, RemoveOnDead, DinamicID, true);
                        if (rolemonster == null)
                        {
                            //invalid x ,y
                            return false;
                        }
                        rolemonster.Family.ID = ID;
                        rolemonster.IsFloor = true;
                        rolemonster.FloorStampTimer = DateTime.Now.AddSeconds(7);
                        rolemonster.Family.Settings = Game.MsgMonster.MonsterSettings.Lottus;

                        rolemonster.FloorPacket = new MsgItemPacket();
                        rolemonster.FloorPacket.m_UID = rolemonster.UID;
                        rolemonster.FloorPacket.m_ID = ID;
                        rolemonster.FloorPacket.m_X = x;
                        rolemonster.FloorPacket.m_Y = y;
                        rolemonster.FloorPacket.MaxLife = 25;
                        rolemonster.FloorPacket.Life = 25;
                        rolemonster.FloorPacket.DropType = MsgDropID.Effect;
                        rolemonster.FloorPacket.m_Color = 13;
                        rolemonster.FloorPacket.ItemOwnerUID = OwnerUID;
                        rolemonster.FloorPacket.GuildID = GuildID;
                        rolemonster.FloorPacket.FlowerType = 2;//2;
                        rolemonster.FloorPacket.Timer = Role.Core.TqTimer(rolemonster.FloorStampTimer);
                        rolemonster.FloorPacket.Name = Name;

                        rolemonster.DBSpell = dbspell;
                        rolemonster.Family.MaxHealth = 25;
                        rolemonster.HitPoints = 25;
                        rolemonster.OwnerFloor = Owner;
                        rolemonster.SpellLevel = spelllevel;


                        if (rolemonster == null)
                        {
                            Console.WriteLine("Eror monster spawn. Server.");
                            return false;
                        }
                        View.EnterMap<Role.IMapObj>(rolemonster);
                        rolemonster.Send(rolemonster.GetArray(stream, false));
                        return true;
                    }
                }
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }
            return false;

        }
        //public void RemoveAI(GameClient entity)
        //{
        //    if (BotAttack.Pool.ContainsKey(entity.Player.UID))
        //    {
        //        GameClient remov;
        //        BotAttack.Pool.TryRemove(entity.Player.UID, out remov);
        //        View.LeaveMap<IMapObj>(entity.Player);
        //        Update = true;
        //    }
        //}
        //public void AddAI(GameClient entity)
        //{
        //    if (!BotAttack.Pool.ContainsKey(entity.Player.UID))
        //    {
        //        BotAttack.Pool.TryAdd(entity.Player.UID, entity);
        //        Role.GameMap map;
        //        if (Pool.ServerMaps.TryGetValue(entity.Player.Map, out map))
        //        {
        //            entity.Map = map;
        //            View.EnterMap<IMapObj>(entity.Player);
        //        }

        //        Update = true;
        //    }
        //    else
        //    {
        //        View.LeaveMap<IMapObj>(entity.Player);
        //        BotAttack.Pool.TryAdd(entity.Player.UID, entity);
        //        View.EnterMap<IMapObj>(entity.Player);
        //    }
        //}
        public bool SelectJump(ref ushort X, ref ushort Y, byte z)
        {

            ushort x = X;
            ushort y = Y;
            ushort XX = (ushort)Pool.GetRandom.Next((int)(x) - z, (int)(x) + z);
            ushort YY = (ushort)Pool.GetRandom.Next((int)(y) - z, (int)(y) + z);
            for (int i = 0; i < 100; i++)
            {
                if (ValidLocation((ushort)(XX), (ushort)(YY)))
                {
                    X = XX;
                    Y = YY;
                    break;
                }
                XX = (ushort)Pool.GetRandom.Next((int)(x) - z, (int)(x) + z);
                YY = (ushort)Pool.GetRandom.Next((int)(y) - z, (int)(y) + z);
            }
            return true;
        }
        //======================end========================//
    }
}
