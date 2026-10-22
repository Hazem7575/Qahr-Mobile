namespace Qahr.Mobile.Database
{
    public class YuanshenAttr
    {
        [System.Flags]
        public enum AttrType : uint
        {
            //MaxHP = 33,
            //PAttack = 34,
        }
        public static System.Collections.Generic.Dictionary<uint, item> YuanshenAttrItem;
        public class item
        {
            public uint id;//0
            public uint ItemID;//1
            public uint TypeLevel;//2
            public AttrType Type1;//3
            // public uint HPValue;//4
            public AttrType Type2;//5
            // public uint AttackValue;//6
            public uint Unknow;//7
            public uint Unknow2;//8
            public uint Unknow3;//9
            public uint Unknow4;//10
            public uint Unknow5;//11
            public uint Unknow6;//12
            public uint Unknow7;//13
            public uint Unknow8;//14
            public uint Unknow9;//15
            public uint Unknow10;//16
            public ushort IDSkill;//17
            public byte LevelSkill;//18
            public ushort IDSkill2;//19
            public byte LevelSkill2;//20
            public ushort IDSkill3;//21
            public byte LevelSkill3;//22
            public ushort IDSkill4;//23
            public byte LevelSkill4;//24
            public ushort IDSkill5;//25
            public byte LevelSkill5;//26
            public ushort IDSkill6;//27
            public byte LevelSkill6;//28
            public ushort IDSkill7;//29
            public byte LevelSkill7;//30
            public uint Unknow11;//31
            public uint Unknow12;//32
            public uint Unknow13;//33
            public uint Unknow14;//34
        }
        public static void Load()
        {
            try
            {
                if (!System.IO.File.Exists(Program.ServerConfig.DbLocation + "\\yuanshen_attr.txt"))
                {
                    System.Console.WriteLine("!File.Exists: " + Program.ServerConfig.DbLocation + "\\yuanshen_attr.txt");
                    return;
                }
                string[] text = System.IO.File.ReadAllLines(Program.ServerConfig.DbLocation + "\\yuanshen_attr.txt");
                YuanshenAttrItem = new System.Collections.Generic.Dictionary<uint, item>();
                for (int x = 0; x < text.Length; x++)
                {
                    string line = text[x];
                    string[] split = line.Split(new string[] { "@@" }, System.StringSplitOptions.RemoveEmptyEntries);
                    var obj = new item();
                    obj.id = uint.Parse(split[0]);
                    obj.ItemID = uint.Parse(split[1]);
                    obj.TypeLevel = uint.Parse(split[2]);
                    obj.Type1 = (AttrType)uint.Parse(split[3]);
                    // obj.HPValue = uint.Parse(split[4]);
                    obj.Type2 = (AttrType)uint.Parse(split[5]);
                    // obj.AttackValue = uint.Parse(split[6]);
                    obj.Unknow = uint.Parse(split[7]);
                    obj.Unknow2 = uint.Parse(split[8]);
                    obj.Unknow3 = uint.Parse(split[9]);
                    obj.Unknow4 = uint.Parse(split[10]);
                    obj.Unknow5 = uint.Parse(split[11]);
                    obj.Unknow6 = uint.Parse(split[12]);
                    obj.Unknow7 = uint.Parse(split[13]);
                    obj.Unknow8 = uint.Parse(split[14]);
                    obj.Unknow9 = uint.Parse(split[15]);
                    obj.Unknow10 = uint.Parse(split[16]);
                    obj.IDSkill = ushort.Parse(split[17]);
                    obj.LevelSkill = byte.Parse(split[18]);
                    obj.IDSkill2 = ushort.Parse(split[19]);
                    obj.LevelSkill2 = byte.Parse(split[20]);
                    obj.IDSkill3 = ushort.Parse(split[21]);
                    obj.LevelSkill3 = byte.Parse(split[22]);
                    obj.IDSkill4 = ushort.Parse(split[23]);
                    obj.LevelSkill4 = byte.Parse(split[24]);
                    obj.IDSkill5 = ushort.Parse(split[25]);
                    obj.LevelSkill5 = byte.Parse(split[26]);
                    obj.IDSkill6 = ushort.Parse(split[27]);
                    obj.LevelSkill6 = byte.Parse(split[28]);
                    obj.IDSkill7 = ushort.Parse(split[29]);
                    obj.LevelSkill7 = byte.Parse(split[30]);
                    obj.Unknow11 = uint.Parse(split[31]);
                    obj.Unknow12 = uint.Parse(split[32]);
                    obj.Unknow13 = uint.Parse(split[33]);
                    obj.Unknow14 = uint.Parse(split[34]);
                    YuanshenAttrItem.Add(obj.id, obj);
                }
                return;
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex);
                return;
            }
        }
    }
}