using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Qahr.Mobile.Database.DBActions
{
    public enum Mode : byte
    {
        Open = 1,
        Create = 2
    }
    public class Write : IDisposable
    {
        private string location = "";
        private List<string> Items;

        private StreamWriter SW;



        public Write(string loc)
        {
            Items = new List<string>();
            location = Program.ServerConfig.DbLocation + loc;

        }
        public void ChangeLocation(string loc)
        {
            location = Program.ServerConfig.DbLocation + loc;

        }
        public void Dispose()
        {
            Items = null;
            location = string.Empty;
            SW = null;
        }
        public Write Add(string data)
        {
            Items.Add(data);
            return this;
        }

        public Write Execute(Mode mod)
        {
            // Atomic write: build the whole file in memory, write to a temp file, flush, then
            // replace the real file in one shot. A crash/exception mid-write can no longer leave the
            // old file truncated or half-written (all call sites rewrite the full file anyway).
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Count=" + Items.Count);
                for (int x = 0; x < Items.Count; x++)
                {
                    sb.AppendLine(Items[x]);
                }

                string tmp = location + ".tmp";
                using (SW = new StreamWriter(tmp, false))
                {
                    SW.Write(sb.ToString());
                    SW.Flush();
                }

                if (File.Exists(location))
                {
                    try { File.Replace(tmp, location, null); }
                    catch
                    {
                        // File.Replace can fail across volumes / on some FS; fall back to delete+move.
                        try { File.Delete(location); } catch { }
                        File.Move(tmp, location);
                    }
                }
                else
                {
                    File.Move(tmp, location);
                }
            }
            catch (Exception e)
            {
                MyConsole.WriteException(e);
            }
            return this;
        }
        
       
    }
}
