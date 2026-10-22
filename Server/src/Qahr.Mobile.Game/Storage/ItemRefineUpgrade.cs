using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Qahr.Mobile.Database
{
    public class ItemRefineUpgrade
    {
        public static Dictionary<uint, uint> ProgresUpdates = new Dictionary<uint, uint>();
        public static Dictionary<uint, EffectEX> EffectsEX = new Dictionary<uint, EffectEX>();
        public static Dictionary<int, Effect> Effects = new Dictionary<int, Effect>();
        public class EffectEX
        {
            public uint ID;
            public uint ItemID;
            public uint ReqLevel;
            public uint RefineType;
            public uint RefineValue;
        }
        public class Effect
        {
            public ushort Level;
            public int PAttack;
            public int MAttack;
            public int PDefense;
            public int MDefense;
            public List<uint> Effects;
        }
        public static void Load()
        {
            string[] baseText = File.ReadAllLines(Program.ServerConfig.DbLocation + "item_refine_upgrade.txt");
            foreach (var bas_line in baseText)
            {
                var line = bas_line.Split(new string[] { "@@", " " }, StringSplitOptions.RemoveEmptyEntries);
                if (Convert.ToUInt32(line[0]) != 1) continue;
                uint Level = Convert.ToUInt32(line[1]);
                uint Progres = Convert.ToUInt32(line[2]);
                ProgresUpdates.Add(Level, Progres);
            }
            baseText = File.ReadAllLines(Program.ServerConfig.DbLocation + "item_refine_effect.txt");
            foreach (var bas_line in baseText)
            {
                var spilitline = bas_line.Split(new string[] { "@@", " " }, StringSplitOptions.RemoveEmptyEntries);
                var iru = new Effect();
                iru.Level = Convert.ToUInt16(spilitline[0]);
                iru.PAttack = Convert.ToInt32(spilitline[1]);
                iru.PDefense = Convert.ToInt32(spilitline[2]);
                iru.MAttack = Convert.ToInt32(spilitline[3]);
                iru.MDefense = Convert.ToInt32(spilitline[4]);
                iru.Effects = new List<uint>();
                for (int x = 5; x < spilitline.Length; x++)
                {
                    uint effect = uint.Parse(spilitline[x]);
                    if (effect != 0)
                        iru.Effects.Add(effect);
                }
                Effects.Add(iru.Level, iru);
            }
            baseText = File.ReadAllLines(Program.ServerConfig.DbLocation + "item_refine_effect_ex.txt");
            foreach (var bas_line in baseText)
            {
                var data = bas_line.Split(new string[] { "@@", " " }, StringSplitOptions.RemoveEmptyEntries);
                var T = new EffectEX();
                T.ID = Convert.ToUInt32(data[0]);
                T.ItemID = Convert.ToUInt32(data[1]);
                T.ReqLevel = Convert.ToUInt32(data[2]);
                T.RefineType = Convert.ToUInt32(data[3]);
                T.RefineValue = Convert.ToUInt32(data[4]);
                EffectsEX.Add(T.ID, T);
            }
        }
    }
}
