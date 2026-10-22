using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace Qahr.Mobile.Database
{
    public class GlobalLotteryTable
    {
        public static bool Loaded = false;
        public static uint Count = 0;
        public static DateTime NextRewardUpdateSpan;
        public static uint TickCount
        {
            get
            {
                TimeSpan Date = DateTime.Now - NextRewardUpdateSpan;
                return (uint)Date.TotalSeconds;
            }
        }
        public static GlobalLotteryCondition[] Conditions;
        public static GlobalLotteryCondition TodayCondition = null;
        public static uint TodayPrizeID = 0;
        public static bool TryGetCondition(byte Type, out GlobalLotteryCondition Condition)
        {
            Condition = null;
            foreach (var condition in Conditions)
                if (condition.Type == Type)
                {
                    Condition = condition;
                    return true;
                }
            return false;
        }
        private static void SetTodayCondition()
        {
            TodayCondition = Conditions[(int)(new Random().Next(0, Conditions.Length))];
        }
        //public static uint GetRandomPrizeToday()
        //{
        //    //if (DateTime.Now >= NextRewardUpdateSpan)
        //    //{
        //    //    TodayPrizeID = 0;
        //    //    NextRewardUpdateSpan = DateTime.Now.AddHours(2);
        //    //}
        //    //if (TodayPrizeID == 0)
        //    //{
        //    //    if (TodayCondition == null)
        //    //        SetTodayCondition();
        //    //    int indexer = (int)(new Random().Next(0, TodayCondition.Prizes.Count));
        //    //    TodayPrizeID = TodayCondition.Prizes.GetElement(indexer).ID;
        //    //}
        //    //return TodayPrizeID;
        //}
        public class GlobalLotteryCondition
        {
            public uint ID;
            public uint Type;
            public byte CostType;
            public uint CostValue;
            public byte CD;
            public byte CDCost;
            public DateTime StartTime;
            public string Desc;
           // public FlexbilArray<GlobalLotteryConditionPrizes> Prizes = new FlexbilArray<GlobalLotteryConditionPrizes>();
            //public GlobalLotteryConditionPrizes PrizeItemID(uint UID)
            //{
            //    foreach (var prize in Prizes.GetValues())
            //    {
            //        if (prize.ID == UID)
            //            return prize;
            //    }
            //    return null;
            //}
        }
        public class GlobalLotteryConditionPrizes
        {
            public uint ID;
            public uint Type;
            public byte PerfectionLevel;
            public string PrizeName;
            public uint PrizeItemID;
            public byte dwparam;
            public byte dwparam2;
            public uint dwparam3;
            public byte dwparam4;
            public byte dwparam5;
            public ushort Count;
            public uint dwparam7;
            public void Parse(string Line)
            {
                string[] data = Line.Split(new string[] { "@@" }, StringSplitOptions.RemoveEmptyEntries);
                if (!uint.TryParse(data[0], out ID))
                    MyConsole.WriteLine("Error In Loading GlobalLotteryConditionPrizes Field 'ID'.");
                if (!uint.TryParse(data[1], out Type))
                    MyConsole.WriteLine("Error In Loading GlobalLotteryConditionPrizes Field 'Type'.");
                if (!byte.TryParse(data[2], out PerfectionLevel))
                    MyConsole.WriteLine("Error In Loading GlobalLotteryConditionPrizes Field 'PerfectionLevel'.");
                if (data.Length < 3)
                    MyConsole.WriteLine("Error In Loading GlobalLotteryConditionPrizes Field 'PrizeName'.");
                PrizeName = data[3];
                if (!uint.TryParse(data[4], out PrizeItemID))
                    MyConsole.WriteLine("Error In Loading GlobalLotteryConditionPrizes Field 'PrizeItemID'.");
                if (!byte.TryParse(data[5], out dwparam))
                    MyConsole.WriteLine("Error In Loading GlobalLotteryConditionPrizes Field 'dwparam'.");
                if (!byte.TryParse(data[6], out dwparam2))
                    MyConsole.WriteLine("Error In Loading GlobalLotteryConditionPrizes Field 'dwparam2'.");
                if (!uint.TryParse(data[7], out dwparam3))
                    MyConsole.WriteLine("Error In Loading GlobalLotteryConditionPrizes Field 'dwparam3'.");
                if (!byte.TryParse(data[8], out dwparam4))
                    MyConsole.WriteLine("Error In Loading GlobalLotteryConditionPrizes Field 'dwparam4'.");
                if (!byte.TryParse(data[9], out dwparam5))
                    MyConsole.WriteLine("Error In Loading GlobalLotteryConditionPrizes Field 'dwparam5'.");
                if (!ushort.TryParse(data[10], out Count))
                    MyConsole.WriteLine("Error In Loading GlobalLotteryConditionPrizes Field 'dwparam6'.");
                if (!uint.TryParse(data[11], out dwparam7))
                    MyConsole.WriteLine("Error In Loading GlobalLotteryConditionPrizes Field 'dwparam7'.");
            }
            public static implicit operator GlobalLotteryConditionPrizes(string Line)
            {
                string[] data = Line.Split(new string[] { "@@" }, StringSplitOptions.RemoveEmptyEntries);
                GlobalLotteryConditionPrizes Prize = new GlobalLotteryConditionPrizes();
                if (!uint.TryParse(data[0], out Prize.ID))
                    MyConsole.WriteLine("Error In Loading GlobalLotteryConditionPrizes Field 'ID'.");
                if (!uint.TryParse(data[1], out Prize.Type))
                    MyConsole.WriteLine("Error In Loading GlobalLotteryConditionPrizes Field 'Type'.");
                if (!byte.TryParse(data[2], out Prize.PerfectionLevel))
                    MyConsole.WriteLine("Error In Loading GlobalLotteryConditionPrizes Field 'PerfectionLevel'.");
                if (data.Length < 3)
                    MyConsole.WriteLine("Error In Loading GlobalLotteryConditionPrizes Field 'PrizeName'.");
                Prize.PrizeName = data[3];
                if (!uint.TryParse(data[4], out Prize.PrizeItemID))
                    MyConsole.WriteLine("Error In Loading GlobalLotteryConditionPrizes Field 'PrizeItemID'.");
                if (!byte.TryParse(data[5], out Prize.dwparam))
                    MyConsole.WriteLine("Error In Loading GlobalLotteryConditionPrizes Field 'dwparam'.");
                if (!byte.TryParse(data[6], out Prize.dwparam2))
                    MyConsole.WriteLine("Error In Loading GlobalLotteryConditionPrizes Field 'dwparam2'.");
                if (!uint.TryParse(data[7], out Prize.dwparam3))
                    MyConsole.WriteLine("Error In Loading GlobalLotteryConditionPrizes Field 'dwparam3'.");
                if (!byte.TryParse(data[8], out Prize.dwparam4))
                    MyConsole.WriteLine("Error In Loading GlobalLotteryConditionPrizes Field 'dwparam4'.");
                if (!byte.TryParse(data[9], out Prize.dwparam5))
                    MyConsole.WriteLine("Error In Loading GlobalLotteryConditionPrizes Field 'dwparam5'.");
                if (!ushort.TryParse(data[10], out Prize.Count))
                    MyConsole.WriteLine("Error In Loading GlobalLotteryConditionPrizes Field 'dwparam6'.");
                if (!uint.TryParse(data[11], out Prize.dwparam7))
                    MyConsole.WriteLine("Error In Loading GlobalLotteryConditionPrizes Field 'dwparam7'.");
                return Prize;
            }

        }

        private static void AddPrize(GlobalLotteryConditionPrizes Prize)
        {
            //foreach (var condition in Conditions)
            //    //if (condition.Type == Prize.Type)
            //        //condition.Prizes.Add(Prize);
        }
        public static void LoadDBConditions()
        {
            if (File.Exists(Program.ServerConfig.DbLocation + "globallotterycondition.ini"))
            {
                Loaded = true;
                WindowsAPI.IniFile Reader = new WindowsAPI.IniFile("globallotterycondition.ini");
                Count = Reader.ReadUInt32("ConditionAmount", "Amount", 0);
                Conditions = new GlobalLotteryCondition[Count];
                for (int x = 0; x <= Count; x++)
                {
                    GlobalLotteryCondition Condition = new GlobalLotteryCondition()
                    {
                        ID = Reader.ReadUInt32(x.ToString(), "id", 0),
                        Type = Reader.ReadUInt32(x.ToString(), "Type", 0),
                        CostType = Reader.ReadByte(x.ToString(), "CostType", 0),
                        CostValue = Reader.ReadUInt32(x.ToString(), "CostValue", 0),
                        CD = Reader.ReadByte(x.ToString(), "CD", 0),
                        CDCost = Reader.ReadByte(x.ToString(), "CDCost", 0),
                        StartTime = DateTime.FromBinary(Reader.ReadInt64(x.ToString(), "StartTime", 0)),
                        Desc = Reader.ReadString(x.ToString(), "Desc", null),
                        //Prizes = new FlexbilArray<GlobalLotteryConditionPrizes>()
                    };
                    Conditions[x - 1] = Condition;
                }
            }
            if (File.Exists(Program.ServerConfig.DbLocation + "global_lottery_pool.txt"))
            {
                Loaded &= true;
                string[] baseText = File.ReadAllLines(Program.ServerConfig.DbLocation + "global_lottery_pool.txt");
                foreach (string line in baseText)
                {
                    GlobalLotteryConditionPrizes Prize = line;
                    AddPrize(Prize);
                }
                if (TodayCondition == null)
                    SetTodayCondition();
            }
        }
    }
}