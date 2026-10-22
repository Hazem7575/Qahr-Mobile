using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Qahr.Mobile.Database.DBActions
{
    public class Read : IDisposable
    {

        private string location = "";
        private StreamReader SR;
        public int Count = 0;

        private string[] items;
        private Int32 curent_count = -1;

        public Read(string file, bool setlocation = false)
        {
            if (setlocation)
                location = file;
            else
                location = Program.ServerConfig.DbLocation + file;
        }
        public void Dispose()
        {
            location = string.Empty;
            SR = null;
            items = null;
            Count = 0;

        }
        public string[] OutBase() { return items; }
        public uint aCount;
        public bool UseRead()
        {
            aCount++;
            return items.Length >= aCount;
        }
        public bool Reader(bool useinvalid = true)
        {
            string[] data = null;

            bool Exit = true;
            if (Exit == File.Exists(location))
            {
                try
                {
                    using (SR = File.OpenText(location))
                    {
                        try
                        {
                            Count = int.Parse(SR.ReadLine().Split('=')[1]);
                            data = new string[Count];
                            for (int x = 0; x < Count; x++)
                            {
                                data[x] = SR.ReadLine();
                            }
                            if (Count == 0)
                                return false;
                        }
                        catch
                        {
                            MyConsole.WriteLine("[Error] An error occurred while reading " + location, ConsoleColor.DarkRed);
                            SR.Close();
                            return false;
                        }
                        SR.Close();
                    }
                }
                catch (Exception e)
                {
                    Exit = false;
                    MyConsole.WriteLine(e.ToString());
                }
            }
            else if (useinvalid)
            {
                Exit = false;

                data = null;

                MyConsole.WriteLine("Invalid Reader " + location + " location", ConsoleColor.DarkRed);
            }

            items = new string[Count];
            for (int x = 0; x < Count; x++)
            {
                items[x] = data[x];
            }
            return Exit;
        }
        public UInt32 ReadUInt32(UInt32 add_def)
        {
            curent_count += 1;
            if (curent_count < Count)
            {
                if (items[curent_count] == null)
                    return add_def;
                else
                    return UInt32.Parse(items[curent_count]);
            }
            else
                return add_def;
        }
        public UInt16 ReadUInt16(UInt16 add_def)
        {
            curent_count += 1;
            if (curent_count < Count)
            {
                if (items[curent_count] == null)
                    return add_def;
                else
                    return UInt16.Parse(items[curent_count]);
            }
            else
                return add_def;
        }
        public UInt64 ReadUInt64(UInt64 add_def)
        {
            curent_count += 1;
            if (curent_count < Count)
            {
                if (items[curent_count] == null)
                    return add_def;
                else
                    return UInt64.Parse(items[curent_count]);
            }
            else
                return add_def;
        }
        public Byte ReadByte(Byte add_def)
        {
            curent_count += 1;
            if (curent_count < Count)
            {
                if (items[curent_count] == null)
                    return add_def;
                else
                    return Byte.Parse(items[curent_count]);
            }
            else
                return add_def;
        }
        public String ReadString(String add_def)
        {
            curent_count += 1;
            if (curent_count < Count)
            {
                if (items[curent_count] == null)
                    return add_def;
                else
                    return items[curent_count];
            }
            else
                return add_def;
        }

    }
}
