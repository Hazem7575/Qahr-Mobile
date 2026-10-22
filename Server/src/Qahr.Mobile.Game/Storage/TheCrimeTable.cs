using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace Qahr.Mobile.Database
{
    public class TheCrimeTable : ConcurrentDictionary<uint, TheCrimeTable.TheCrime>
    {
        public class TheCrime
        {
            public string OwnerName;
            public uint OwnerUID;
            public int ConquerPoints;

            public override string ToString()
            {
                Database.DBActions.WriteLine writer = new DBActions.WriteLine('/');
                writer.Add(OwnerName).Add(OwnerUID).Add(ConquerPoints);
                return writer.Close();
            }
        }
        public void AddCrime(string OwnerName, uint OwnerUID, int ConquerPoints)
        {
            TheCrime crime;
            if (TryGetValue(OwnerUID, out crime))
            {
                crime.OwnerName = OwnerName;
                crime.ConquerPoints += ConquerPoints;
            }
            else
            {
                crime = new TheCrime()
                {
                    OwnerName = OwnerName,
                    ConquerPoints = ConquerPoints,
                    OwnerUID = OwnerUID
                };
                TryAdd(OwnerUID, crime);
            }
        }
        public int Claim(out string name)
        {
            if (Count > 0)
            {
                TheCrime crime;
                if (TryRemove(this.Values.First().OwnerUID, out crime))
                {
                    name = crime.OwnerName;
                    return crime.ConquerPoints;
                }
            }
            name = "None";
            return 0;
        }

        internal static void Load()
        {
            using (Database.DBActions.Read Reader = new DBActions.Read("Crime.ini"))
            {
                if (Reader.Reader())
                {
                    for (int x = 0; x < Reader.Count; x++)
                    {
                        string Line = Reader.ReadString("^");
                        uint OwnerUID = uint.Parse(Line.Split('^')[0]);
                        DBActions.ReadLine Readline = new DBActions.ReadLine(Line.Split('^')[1], '/');
                        TheCrimeTable CrimeClient;
                        if (Pool.TheCrimePoll.TryGetValue(OwnerUID, out CrimeClient))
                        {
                            CrimeClient.AddCrime(Readline.Read(""), Readline.Read((uint)0), Readline.Read(0));
                        }
                        else
                        {
                            CrimeClient = new TheCrimeTable();
                            CrimeClient.AddCrime(Readline.Read(""), Readline.Read((uint)0), Readline.Read(0));
                            Pool.TheCrimePoll.TryAdd(OwnerUID, CrimeClient);
                        }
                    }
                }
            }
        }
        internal static void Save()
        {
            using (Database.DBActions.Write writer = new DBActions.Write("Crime.ini"))
            {
                foreach (var client in Pool.TheCrimePoll)
                {
                    uint OwnerUID = client.Key;
                    foreach (var crimes in client.Value.Values)
                    {
                        writer.Add(OwnerUID.ToString() + "^" + crimes.ToString());
                    }
                }
                writer.Execute(DBActions.Mode.Open);
            }
        }
    }
}
