namespace Qahr.Mobile.Database
{
    public class YuanshenLevUP
    {
        public static System.Collections.Generic.Dictionary<uint, item> YuanshenLevUPItem;
        public class item
        {
            public uint id;//0
            public uint Level;//1
            public uint TypeLevel;//2
            public uint Exp;//3
            public uint Rating;//4
            public uint Rating5;//5
            public uint Rating6;//6
        }
        public static void Load()
        {
            try
            {
                if (!System.IO.File.Exists(Program.ServerConfig.DbLocation + "\\yuanshen_levup.txt"))
                {
                    System.Console.WriteLine("!File.Exists: " + Program.ServerConfig.DbLocation + "\\yuanshen_levup.txt");
                    return;
                }
                string[] text = System.IO.File.ReadAllLines(Program.ServerConfig.DbLocation + "\\yuanshen_levup.txt");
                YuanshenLevUPItem = new System.Collections.Generic.Dictionary<uint, item>();
                for (int x = 0; x < text.Length; x++)
                {
                    string line = text[x];
                    string[] split = line.Split(new string[] { "@@" }, System.StringSplitOptions.RemoveEmptyEntries);
                    var obj = new item();
                    obj.id = uint.Parse(split[0]);
                    obj.Level = uint.Parse(split[1]);
                    obj.TypeLevel = uint.Parse(split[2]);
                    obj.Exp = uint.Parse(split[3]);
                    obj.Rating = uint.Parse(split[4]);
                    obj.Rating5 = uint.Parse(split[5]);
                    obj.Rating6 = uint.Parse(split[6]);
                    YuanshenLevUPItem.Add(obj.id, obj);
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