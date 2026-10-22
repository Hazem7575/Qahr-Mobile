using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Database
{
    public class ActivityTask : System.SafeDictionary<uint, ActivityTask.Task>
    {

        public class Task
        {
            public uint ID;
            public byte NeedProgress = 0;
            public byte ActiveValue = 0;
            public uint OpenLev;
            public uint CloseLev;

            public bool CheckOpen(ushort level, byte reborn)
            {
                uint needlevel = OpenLev % 1000;
                uint needreborn = (OpenLev - needlevel) / 1000;

                // CloseLev uses the same reborn*1000+level encoding (0 = never closes)
                if (CloseLev != 0 && (uint)reborn * 1000 + level >= CloseLev)
                    return false;

                return level >= needlevel && reborn >= needreborn;
            }

        }

        
        public void Load()
        {
            WindowsAPI.IniFile Reader = new WindowsAPI.IniFile("ActivityTask.ini");

            int TaskCount = Reader.ReadByte("ActivityTask", "Num", 0);

            for (uint x = 1; x <= TaskCount; x++)
            {
                // key by id= (matches the client and the IncreaseTask calls), not by section index
                uint id = Reader.ReadUInt32(x.ToString(), "id", x);
                if (ContainsKey(id))
                    continue;
                Add(id, new Task()
                {
                    ID = id, NeedProgress = Reader.ReadByte(x.ToString(), "Progress",0),
                    ActiveValue = Reader.ReadByte(x.ToString(), "ActiveValue", 0),
                    OpenLev = Reader.ReadUInt32(x.ToString(), "OpenLev", 0),
                    CloseLev = Reader.ReadUInt32(x.ToString(), "CloseLev", 0)
                });
            }

        }
    }
}
