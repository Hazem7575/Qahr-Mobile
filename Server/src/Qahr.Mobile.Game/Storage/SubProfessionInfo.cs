using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Database
{
    public class SubProfessionInfo : Dictionary<DBLevExp.Sort, Dictionary<byte, SubProfessionInfo.ProfessionInfo>>
    {
        public uint[] GetDamage(DBLevExp.Sort ID, byte level)
        {
            return this[ID][level].Damage;
        }
        public bool AllowUpdate(Client.GameClient client, DBLevExp.Sort ID, byte level)
        {
            var item = this[ID][level];
            if (client.Player.Level < item.NeedLevel)
                return false;
            if (client.Player.Reborn < item.NeedReborn)
                return false;
            return true;
        }
        public class ProfessionInfo
        {
            public uint[] Damage;
            public uint NeedLevel;
            public byte NeedReborn;
        }

        public void Load()
        {
            WindowsAPI.IniFile reader = new WindowsAPI.IniFile("SubProfessionInfo.ini");
            for (byte x = 1; x < 10; x++)
            {
                if (x != 7 && x != 8)
                {
                    string line = x.ToString();
                    for (byte level = 0; level < 10; level++)
                    {
                        if (level != 0)
                            line = x.ToString() + level.ToString();


                        ProfessionInfo info = new ProfessionInfo();
                        if (x == 6)
                        {
                            info.Damage = new uint[2];
                            string intro = reader.ReadString(line, "intro", "");
                            string[] linedmg = intro.Split(';');

                            info.Damage[0] = uint.Parse(linedmg[0].Split('+')[1].ToString());
                            info.Damage[1] = uint.Parse(linedmg[1].Split('+')[1].ToString());
                        }
                        else
                        {
                            info.Damage = new uint[1];
                            string intro = reader.ReadString(line, "intro", "");
                            string[] linedmg = intro.Split('+');
                            string damage = linedmg[1].Split('%')[0];
                            info.Damage[0] = uint.Parse(damage);
                            info.NeedLevel = reader.ReadByte(line, "req_lv", 0);
                            info.NeedReborn = reader.ReadByte(line, "req_meto", 0);
                        }
                        if (!ContainsKey((DBLevExp.Sort)x))
                            Add((DBLevExp.Sort)x, new Dictionary<byte, ProfessionInfo>());

                        this[(DBLevExp.Sort)x].Add(level, info);
                    }
                }
            }

        }
    }
}
