using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qahr.Mobile.Database
{
    public class SwordAncestorType
    {
        public static System.Collections.Generic.Dictionary<uint, Items> SwordAncestor = new System.Collections.Generic.Dictionary<uint, Items>();
        public class Items
        {
            public uint Index;//0
            public uint Unknow;//1
            public uint Level;//2
            public uint Rating;//3
            public uint Exp;//4
            public uint SkillID;//5
            public uint SkillLevel;//6
            public uint SkillID2;//7
            public uint SkillLevel2;//8
            public uint SkillID3;//9
            public uint SkillLevel3;//10
        }
        internal static void Load()
        {
            try
            {
                if (!System.IO.File.Exists(Program.ServerConfig.DbLocation + "sword_ancestor_type.txt"))
                {
                    System.Console.WriteLine("!File.Exists: " + Program.ServerConfig.DbLocation + "sword_ancestor_type.txt");
                    return;
                }
                SwordAncestor = new System.Collections.Generic.Dictionary<uint, Items>();
                var str = System.IO.File.ReadAllLines(Program.ServerConfig.DbLocation + "sword_ancestor_type.txt");
                foreach (string Line in str)
                {
                    string[] Data = Line.Split(new string[] { "@@", " " }, System.StringSplitOptions.RemoveEmptyEntries);
                    var obj = new Items();
                    obj.Index = System.Convert.ToUInt32(Data[0]);
                    obj.Unknow = System.Convert.ToUInt32(Data[1]);
                    obj.Level = System.Convert.ToUInt32(Data[2]);
                    obj.Rating = System.Convert.ToUInt32(Data[3]);
                    obj.Exp = System.Convert.ToUInt32(Data[4]);
                    obj.SkillID = System.Convert.ToUInt32(Data[5]);
                    obj.SkillLevel = System.Convert.ToUInt32(Data[6]);
                    obj.SkillID2 = System.Convert.ToUInt32(Data[7]);
                    obj.SkillLevel2 = System.Convert.ToUInt32(Data[8]);
                    obj.SkillID3 = System.Convert.ToUInt32(Data[9]);
                    obj.SkillLevel3 = System.Convert.ToUInt32(Data[10]);
                    if (!SwordAncestor.ContainsKey(obj.Index))
                        SwordAncestor.Add(obj.Index, obj);
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
