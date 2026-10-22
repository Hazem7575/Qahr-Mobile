using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace Qahr.Mobile.Database
{
    public class DeityAltar
    {

        public static ConcurrentDictionary<uint, DeityAltarDB> DeityAltarS = new ConcurrentDictionary<uint, DeityAltarDB>();
        public class DeityAltarDB
        {
            public uint UID;
            public string Name;
            public uint Jades;
            public int Rank;

            public override string ToString()
            {
                var writer = new DBActions.WriteLine('/');
                writer.Add(UID).Add(Name).Add(Jades).Add(Rank);
                return writer.Close();
            }
        }

        public static void Save()
        {
            using (Database.DBActions.Write _wr = new Database.DBActions.Write("DeityAltar.txt"))
            {
                foreach (var _obj in DeityAltarS.Values)
                    _wr.Add(_obj.ToString());
                _wr.Execute(DBActions.Mode.Open);
            }
        }
        public static void Load()
        {
            using (Database.DBActions.Read r = new Database.DBActions.Read("DeityAltar.txt"))
            {
                if (r.Reader())
                {
                    int count = r.Count;
                    for (uint x = 0; x < count; x++)
                    {
                        Database.DBActions.ReadLine reader = new DBActions.ReadLine(r.ReadString(""), '/');
                        DeityAltarDB Value = new DeityAltarDB();
                        Value.UID = reader.Read((uint)0);
                        Value.Name = reader.Read("");
                        Value.Jades = reader.Read((uint)0);
                        Value.Rank = reader.Read((int)0);
                        DeityAltarS.Add(Value.UID, Value);
                    }
                }
            }

        }

    }
}
