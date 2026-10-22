using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Database
{
    public static class JianHuTable
    {

        private static ushort[] MinutesOnTalent = new ushort[5] { 1001, 501, 385, 313, 193 };
        private static ushort[] FreeCourseInCastle = new ushort[5] { 294, 625, 833, 1000, 1562 };
        private static byte[] FreeCourse = new byte[5] { 10, 20, 26, 31, 52 };
        private static double[] MinutesInCastle = new double[5] { 31.25, 31.25, 32, 32.2, 30 };
        private static ushort[] GetPoints = new ushort[6] { 100, 120, 150, 200, 300, 500 };
        private static ushort[] PrestigePoints = new ushort[6] { 30, 36, 45, 60, 90, 150 };

        private static ushort[] GetAlignmentExtraPoints = new ushort[9] { 0, 10, 13, 15, 18, 21, 25, 30, 50 };

        public static Dictionary<byte, List<byte>> CultivateStatus = new Dictionary<byte, List<byte>>();

        public static ushort AlignmentExtraPoints(byte amount)
        {
            return GetAlignmentExtraPoints[Math.Min(8, (int)amount)];
        }
        public static ushort GetMinutesOnTalent(byte Talent)
        {
            if (Talent == 0) Talent = 1;
            return MinutesOnTalent[Math.Min(4, (int)(Talent - 1))];
        }
        public static ushort GetFreeCourseInCastle(byte Talent)
        {
            if (Talent == 0) Talent = 1;
            return FreeCourseInCastle[Math.Min(4, (int)(Talent - 1))];
        }
        public static ushort GetFreeCourse(byte Talent)
        {
            if (Talent == 0)
                Talent = 1;
            return FreeCourse[Math.Min(4, (int)(Talent - 1))];
        }
        public static double GetMinutesInCastle(byte Talent)
        {
            if (Talent == 0)
                Talent = 1;
            return MinutesInCastle[Math.Min(4, (int)(Talent - 1))];
        }
        public static ushort GetStatusPoints(byte Level)
        {
            if (Level == 0)
                Level = 1;
            return GetPoints[Math.Min(5, (int)(Level - 1))];
        }
        public static ushort GetPrestigePoints(byte Level)
        {
            if (Level == 0)
                Level = 1;
            return PrestigePoints[Math.Min(5, (int)(Level - 1))];
        }
        public class Atribut
        {
            public byte Level;
            public byte Type;
            public ushort Power;
        }
        public static Dictionary<ushort, Atribut> Atributes = new Dictionary<ushort, Atribut>();
        public static ushort ValueToRoll(byte typ, byte level)
        {
            return (ushort)((ushort)typ + level * 256);
        }
        public static ushort GetPower(ushort UID)
        {
            return Atributes[UID].Power;
        }
        public static void LoadStatus()
        {

            try
            {
                using (Database.DBActions.Read Reader = new DBActions.Read("JianghuAttributes.txt"))
                {
                    if (Reader.Reader())
                    {
                        uint count = (uint)Reader.Count;
                        for (uint x = 0; x < count; x++)
                        {
                            DBActions.ReadLine line = new DBActions.ReadLine(Reader.ReadString(""), ' ');
                            Atribut atr = new Atribut();
                            int noumber = line.Read((int)0);
                            atr.Type = line.Read((byte)0);
                            atr.Level = line.Read((byte)0);
                            atr.Power = line.Read((ushort)0);

                            if (atr.Type == (byte)Role.Instance.JiangHu.Stage.AtributesType.CriticalStrike
                             || atr.Type == (byte)Role.Instance.JiangHu.Stage.AtributesType.SkillCriticalStrike
                             || atr.Type == (byte)Role.Instance.JiangHu.Stage.AtributesType.Immunity)
                                atr.Power = (ushort)(atr.Power * 10);

                            ushort atr_val = ValueToRoll(atr.Type, atr.Level);
                            Atributes.Add(atr_val, atr);
                        }
                    }
                }

                using (Database.DBActions.Read Reader = new DBActions.Read("JingHuCultivateStatus.txt"))
                {
                    if (Reader.Reader())
                    {
                        uint count = (uint)Reader.Count;
                        for (uint x = 0; x < count; x++)
                        {
                            DBActions.ReadLine line = new DBActions.ReadLine(Reader.ReadString(""), ' ');
                            byte Stage = line.Read((byte)0);
                            byte count_status = line.Read((byte)0);
                            List<byte> StatusAllows = new List<byte>();
                            for (byte i = 0; i < count_status; i++)
                                StatusAllows.Add(line.Read((byte)0));

                            CultivateStatus.Add(Stage, StatusAllows);
                        }
                    }
                }
            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }

        }
        public static void LoadJiangHu()
        {

            using (Database.DBActions.Read r = new Database.DBActions.Read("JiangHu.txt"))
            {
                if (r.Reader())
                {
                    int count = r.Count;
                    for (uint x = 0; x < count; x++)
                    {
                        Database.DBActions.ReadLine readerline = new DBActions.ReadLine(r.ReadString(""), '/');
                        Role.Instance.JiangHu jiang = new Role.Instance.JiangHu(readerline.Read((uint)0));
                        jiang.Name = readerline.Read("");
                        jiang.CustomizedName = readerline.Read("");
                        jiang.Level = readerline.Read((byte)0);
                        jiang.Talent = readerline.Read((byte)0);
                        jiang.FreeTimeToday = readerline.Read((uint)0);
                        jiang.OnJiangMode = readerline.Read((byte)0) == 1;
                        jiang.FreeCourse = readerline.Read((uint)0);
                        jiang.StartCountDwon = DateTime.Now;
                        jiang.CountDownEnd = DateTime.Now.AddSeconds(readerline.Read((uint)0));
                        jiang.RoundBuyPoints = readerline.Read((uint)0);

                        uint cStage = 0;
                        foreach (var stage in jiang.ArrayStages)
                        {
                            cStage += 1;
                            stage.Activate = readerline.Read((byte)0) == 1;
                     
                            foreach (var star in stage.ArrayStars)
                            {
                              
                                star.Activate = readerline.Read((byte)0) == 1;
                                star.UID = readerline.Read((ushort)0);
                                if (star.Activate)
                                {
                                    star.Typ = jiang.GetValueType(star.UID);
                                    star.Level = jiang.GetValueLevel(star.UID);
                                    if (cStage == 9)
                                    {
                                        if (star.Typ == Role.Instance.JiangHu.Stage.AtributesType.PAttack)
                                        {
                                            star.Typ = Role.Instance.JiangHu.Stage.AtributesType.MaxLife;
                                            star.UID = jiang.ValueToRoll(star.Typ, star.Level);
                                        }
                                    }    
                                }
                               
                              
                            }
                        }
                        jiang.CreateStatusAtributes(null);
                    }
                }
            }

        }
        public static void SaveJiangHu()
        {

            using (Database.DBActions.Write _wr = new Database.DBActions.Write("JiangHu.txt"))
            {

                var dictionary = Role.Instance.JiangHu.Poll.Values.Where(p => p.UID < 3999900001).ToArray();
                foreach (var jiang in dictionary)
                    _wr.Add(jiang.ToString());
                _wr.Execute(DBActions.Mode.Open);
            }

        }
    }
}
