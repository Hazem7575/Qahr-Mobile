using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Database
{
    public class AchievementCollection
    {
        public int Key;
        public uint[] Value = new uint[Game.MsgServer.ClientAchievement.File_Size];

        public override string ToString()
        {
            DBActions.WriteLine write = new DBActions.WriteLine('/');
            write.Add(Key);
            foreach (uint val in Value)
                write.Add(val);
            return write.Close();
        }
        public void Load(string data)
        {

            using (DBActions.ReadLine line = new DBActions.ReadLine(data, '/'))
            {
                Key = line.Read((int)0);
                for (int x = 0; x < Value.Length; x++)
                    Value[x] = line.Read((uint)0);
            }
        }
    }
}
