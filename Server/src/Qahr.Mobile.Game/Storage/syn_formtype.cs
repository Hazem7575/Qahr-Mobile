using System;
using System.Collections.Generic;
using System.IO;

namespace Qahr.Mobile.Database
{
    public class syn_formtype
    {
        public static Dictionary<uint, Dictionary<uint, syn_formtype.formtype>> _formtype;
        public class formtype
        {
            public uint id;
            public uint type;
            public uint Level;
            public ulong EXP;
            public ulong Data;
            public ulong GateRepair { get; set; }
        }
        public static bool TryGetValue(uint id, uint level, out syn_formtype.formtype form)
        {
            if (syn_formtype._formtype.ContainsKey(id) && syn_formtype._formtype[id].TryGetValue(level, out form))
                return true;
            form = (syn_formtype.formtype)null;
            return false;
        }
        public static void Load()
        {
            if (!File.Exists(Program.ServerConfig.DbLocation + "syn_formtype.txt"))
            {
                Console.WriteLine("!File.Exists: " + Program.ServerConfig.DbLocation + "syn_formtype.txt");
            }
            else
            {
                syn_formtype._formtype = new Dictionary<uint, Dictionary<uint, syn_formtype.formtype>>();
                foreach (string readAllLine in File.ReadAllLines(Program.ServerConfig.DbLocation + "syn_formtype.txt"))
                {
                    string[] separator = new string[2] { "@@", " " };
                    string[] strArray = readAllLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    syn_formtype.formtype formtype = new syn_formtype.formtype();
                    formtype.id = Convert.ToUInt32(strArray[0]);
                    formtype.type = Convert.ToUInt32(strArray[1]);
                    formtype.Level = Convert.ToUInt32(strArray[2]);
                    formtype.EXP = Convert.ToUInt64(strArray[3]);
                    formtype.Data = Convert.ToUInt64(strArray[4]);
                    if (syn_formtype._formtype.ContainsKey(formtype.type))
                    {
                        if (!syn_formtype._formtype[formtype.type].ContainsKey(formtype.Level))
                            syn_formtype._formtype[formtype.type].Add(formtype.Level, formtype);
                    }
                    else
                    {
                        syn_formtype._formtype.Add(formtype.type, new Dictionary<uint, syn_formtype.formtype>());
                        syn_formtype._formtype[formtype.type].Add(formtype.Level, formtype);
                    }
                }
            }
        }
    }
}
