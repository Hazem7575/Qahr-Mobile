using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Qahr.Mobile.Database
{
    public class Tranformation
    {
        public static ushort[] Disguises = new ushort[] { 111, 224, 117, 152, 113, 833, 116, 245, 223, 112, 222, 114, 221, 115, 220 };

        
        public static ushort GetRandomTransform()
        {
            int id = Pool.GetRandom.Next(0, (int)(Disguises.Length + 1));
            if (id < Disguises.Length)
                return Disguises[id];
            else return 116;
        }

        public static Dictionary<ushort, Dictionary<byte, DBTranform>> TransformInfo;

        public class DBTranform
        {
            public ushort SpellID;
            public byte Level;
            public string Name = "";
            public ushort ID;
            public ushort HitPoints;
        }
        public static void Int()
        {
            TransformInfo = new Dictionary<ushort, Dictionary<byte, DBTranform>>();

            string[] baseText = File.ReadAllLines(Program.ServerConfig.DbLocation + "TransformInfo.txt");
            foreach (string aline in baseText)
            {
                string[] line = aline.Split(' ');
                DBTranform info = new DBTranform()
                {
                    SpellID = ushort.Parse(line[0]),
                    Level = byte.Parse(line[1]),
                    Name = line[2],
                    ID = ushort.Parse(line[3]),
                    HitPoints = ushort.Parse(line[4])
                };
                if (TransformInfo.ContainsKey(info.SpellID))
                {
                    TransformInfo[info.SpellID].Add(info.Level, info);
                }
                else
                {
                    TransformInfo.Add(info.SpellID, new Dictionary<byte, DBTranform>());
                    TransformInfo[info.SpellID].Add(info.Level, info);
                }
            }
        }
    }
}
