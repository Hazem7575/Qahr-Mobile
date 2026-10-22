using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
namespace Qahr.Mobile.Database
{
    public class RankMonster
    {

        public static ConcurrentDictionary<uint, HuntCoins> HuntCoinsMap = new ConcurrentDictionary<uint, HuntCoins>();
        public class HuntCoins
        {
            public uint UID;
            public string Name;
            public uint KillMonster;
            public uint KillNew;
            public int Rank;
            public byte Calim;
            public byte CalimNew;
            public override string ToString()
            {
                var writer = new DBActions.WriteLine('/');
                writer.Add(UID).Add(Name).Add(KillMonster).Add(Rank).Add(Calim).Add(KillNew).Add(CalimNew);
                return writer.Close();
            }
        }

        public static void Save()
        {
            using (Database.DBActions.Write _wr = new Database.DBActions.Write("RankMonster.txt"))
            {
                foreach (var _obj in HuntCoinsMap.Values)
                    _wr.Add(_obj.ToString());
                _wr.Execute(DBActions.Mode.Open);
            }
        }
        public static void Load()
        {
            using (Database.DBActions.Read r = new Database.DBActions.Read("RankMonster.txt"))
            {
                if (r.Reader())
                {
                    int count = r.Count;
                    for (uint x = 0; x < count; x++)
                    {
                        Database.DBActions.ReadLine reader = new DBActions.ReadLine(r.ReadString(","), '/');
                        HuntCoins Value = new HuntCoins();
                        Value.UID = reader.Read((uint)0);
                        Value.Name = reader.Read("");
                        Value.KillMonster = reader.Read((uint)0);
                        Value.Rank = reader.Read((int)0);
                        Value.Calim = reader.Read((byte)0);
                        Value.KillNew = reader.Read((uint)0);// كان byte فالقيم فوق 255 بتضيع - الحقل uint
                        Value.CalimNew = reader.Read((byte)0);
                        HuntCoinsMap.Add(Value.UID, Value);
                    }
                }
            }

        }

    }
}
