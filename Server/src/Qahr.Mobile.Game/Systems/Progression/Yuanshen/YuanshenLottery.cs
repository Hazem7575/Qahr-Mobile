namespace Qahr.Mobile.Database
{
    public class YuanshenLottery
    {
        [System.Flags]
        public enum LotteryType : byte
        {
            RegularCall = 1,
            PreciousCalling = 2,
        }
        public static System.Collections.Generic.Dictionary<uint, item> YuanshenLotteryItem;
        public class item
        {
            public uint id;//0
            public LotteryType Type;//1
            public uint Unknow;//2
            public uint Unknow1;//3
            public uint Unknow2;//4
            public uint ItemID;//5
            public ushort CountItem;//6
        }
        public static void Load()
        {
            try
            {
                if (!System.IO.File.Exists(Program.ServerConfig.DbLocation  + "\\yuanshen_lottery.txt"))
                {
                    System.Console.WriteLine("!File.Exists: " + Program.ServerConfig.DbLocation  + "\\yuanshen_lottery.txt");
                    return;
                }
                string[] text = System.IO.File.ReadAllLines(Program.ServerConfig.DbLocation  + "\\yuanshen_lottery.txt");
                YuanshenLotteryItem = new System.Collections.Generic.Dictionary<uint, item>();
                for (int x = 0; x < text.Length; x++)
                {
                    string line = text[x];
                    string[] split = line.Split(new string[] { "@@" }, System.StringSplitOptions.RemoveEmptyEntries);
                    var obj = new item();
                    obj.id = uint.Parse(split[0]);
                    obj.Type = (LotteryType)uint.Parse(split[1]);
                    obj.Unknow = uint.Parse(split[2]);
                    obj.Unknow1 = uint.Parse(split[3]);
                    obj.Unknow2 = uint.Parse(split[4]);
                    obj.ItemID = uint.Parse(split[5]);
                    obj.CountItem = ushort.Parse(split[6]);
                    YuanshenLotteryItem.Add(obj.id, obj);
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