using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.WindowsAPI;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Qahr.Mobile
{
    public struct ErrorCodes
    {
        public const int INVALID_HANDLE_VALUE = -1;
    }
    public class MyConsole
    {
        private const int MF_BYCOMMAND = 0x00000000;
        public const int SC_CLOSE = 0xF060;

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        public static void DissableButton()
        {
            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_CLOSE, MF_BYCOMMAND);

        }

        private static IntPtr _InputHandle;
        private static IntPtr _OutputHandle;


        private static System.Counter ExceptionsCounter = new System.Counter(1);

        /// <summary> The handle Input handle is not recommended </summary>
        public static IntPtr InputHandle
        {
            get { return _InputHandle; }
            set
            {
                if (!Kernel32.SetStdHandle(STD_INPUT_HANDLE, value))
                    throw new Exception("Unable to set the new Input handle");
                _InputHandle = value;
            }
        }

        /// <summary> handle Output handle is not recommended </summary>
        public static IntPtr OutputHandle
        {
            get { return _OutputHandle; }
            set
            {
                if (!Kernel32.SetStdHandle(STD_OUTPUT_HANDLE, value))
                    throw new Exception("Unable to set the new Output handle");
                _OutputHandle = value;
            }
        }

        public static string Title
        {
            get
            {
                StringBuilder sb = new StringBuilder(32767);
                uint size = (uint)sb.Capacity;
                Kernel32.GetConsoleTitle(sb, size);
                sb.Capacity = sb.Length;
                return sb.ToString();
            }
            set { Kernel32.SetConsoleTitle(value); }
        }

        public const int STD_INPUT_HANDLE = -10;
        public const int STD_OUTPUT_HANDLE = -11;
        public const int STD_ERROR_HANDLE = -12; //Not being used yet

        static MyConsole()
        {
            InputHandle = Kernel32.GetStdHandle(STD_INPUT_HANDLE);
            OutputHandle = Kernel32.GetStdHandle(STD_OUTPUT_HANDLE);

            if (InputHandle.ToInt64() == ErrorCodes.INVALID_HANDLE_VALUE ||
                OutputHandle.ToInt64() == ErrorCodes.INVALID_HANDLE_VALUE)
            {
                throw new Exception("Unable to get the Console Handle");
            }
        }

        public static void Write(string value)
        {
            uint written = 0;
            Kernel32.WriteConsole(OutputHandle, value, (uint)value.Length, out written, IntPtr.Zero);
        }

        private static DateTime NOW = DateTime.Now;
        private static System.Time32 NOW32 = System.Time32.Now;
        public static string TimeStamp()
        {
            return "[" + NOW.AddMilliseconds((System.Time32.Now - NOW32).GetHashCode()).ToString("hh:mm:ss") + "]";
        }
        public static void WriteLine(object value, ConsoleColor color = ConsoleColor.White)
        {
            if (BootScreen.Active) { BootScreen.OnLine(value == null ? "" : value.ToString(), color); return; }
            lock (WriteLock)
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.Write(TimeStamp() + " ");
                System.Console.ForegroundColor = color;
                System.Console.Write(value.ToString() + "\n");
            }
        }

        public static readonly object WriteLock = new object();

        /// <summary>سطر واحد بأكتر من لون: WriteParts("label", ConsoleColor.DarkGray, "value", ConsoleColor.White, ...).</summary>
        public static void WriteParts(params object[] textColorPairs)
        {
            var sb = new StringBuilder();
            for (int i = 0; i + 1 < textColorPairs.Length; i += 2) sb.Append(textColorPairs[i]);
            if (BootScreen.Active) { BootScreen.OnLine(sb.ToString(), ConsoleColor.Gray); return; }
            lock (WriteLock)
            {
                System.Console.ForegroundColor = ConsoleColor.DarkGray;
                System.Console.Write(TimeStamp() + " ");
                for (int i = 0; i + 1 < textColorPairs.Length; i += 2)
                {
                    System.Console.ForegroundColor = (ConsoleColor)textColorPairs[i + 1];
                    System.Console.Write(Convert.ToString(textColorPairs[i]));
                }
                System.Console.ForegroundColor = ConsoleColor.White;
                System.Console.Write("\n");
            }
        }

        public static ConsoleKeyInfo ReadKey()
        {
            return Console.ReadKey();
        }
        ///<summary> Attach a console to a specified process, CAUTION: If the console will be closed the process will close! </summary>
        public static void AttachConsole(uint PID)
        {
            if (!Kernel32.AttachConsole(PID))
                throw new Exception("Unable to attach the console.");
        }

        /// <summary>
        /// Create a console for the current process, Caution: Closing the console will also close the process!
        /// </summary>
        public static void AllocConsole()
        {
            if (!Kernel32.AllocConsole())
                throw new Exception("Unable to allocate a console.");
            //System.Console.read
        }
        public static void WriteException(Exception e)
        {
            try
            {
                MyConsole.WriteLine(e.ToString());
                SaveException(e);
            }
            catch (Exception ex)
            {
                WriteLine(ex.ToString());
            }
        }
        // Throttle exception-to-disk writes so a flood of bad packets can't fill the disk with one
        // file per exception. Over the cap we still print to the console but skip the file write.
        private static int _excThisWindow = 0;
        private static DateTime _excWindowStart = DateTime.Now;
        private const int MaxExceptionsPerMinute = 120;
        private static readonly object _excThrottleLock = new object();

        private static bool AllowExceptionFileWrite()
        {
            lock (_excThrottleLock)
            {
                var now = DateTime.Now;
                if ((now - _excWindowStart).TotalSeconds >= 60)
                {
                    _excWindowStart = now;
                    _excThisWindow = 0;
                }
                if (_excThisWindow >= MaxExceptionsPerMinute)
                    return false;
                _excThisWindow++;
                return true;
            }
        }

        public static void SaveException(Exception e)
        {
            if (e == null)
                return;
            // Rate-limit disk writes (keep the console line so nothing is lost silently).
            if (!AllowExceptionFileWrite())
            {
                try { WriteLine(e.ToString()); } catch { }
                return;
            }
            try
            {
                SaveExceptionToDisk(e);
            }
            catch
            {
                try { WriteLine(e.ToString()); } catch { }
            }
        }

        private static void SaveExceptionToDisk(Exception e)
        {


            const string UnhandledExceptionsPath = "Exceptions\\";

            var dt = DateTime.Now;
            string date = dt.Month + "-" + dt.Day + "//";

            if (!Directory.Exists((Application.StartupPath + UnhandledExceptionsPath).RemoveIllegalCharacters(true, false)))
                Directory.CreateDirectory((Application.StartupPath + "\\" + UnhandledExceptionsPath).RemoveIllegalCharacters(true, false));
            if (!Directory.Exists((Application.StartupPath + "\\" + UnhandledExceptionsPath + date).RemoveIllegalCharacters(true, false)))
                Directory.CreateDirectory((Application.StartupPath + "\\" + UnhandledExceptionsPath + date).RemoveIllegalCharacters(true, false));
            if (!Directory.Exists((Application.StartupPath + "\\" + UnhandledExceptionsPath + date + e.TargetSite.Name).RemoveIllegalCharacters(true, false)))
                Directory.CreateDirectory((Application.StartupPath + "\\" + UnhandledExceptionsPath + date + e.TargetSite.Name).RemoveIllegalCharacters(true, false));

            string fullPath = (Application.StartupPath + "\\" + UnhandledExceptionsPath + date + e.TargetSite.Name + "\\").RemoveIllegalCharacters(true, false);

            string date2 = (dt.DayOfYear + "-" + dt.Hour + "-" + dt.Minute + "-" + dt.Second + "E" + ExceptionsCounter.Next).RemoveIllegalCharacters(false, true);
            List<string> Lines = new List<string>();

            Lines.Add("----Exception message----");
            Lines.Add(e.Message);
            Lines.Add("----End of exception message----\r\n");

            Lines.Add("----Stack trace----");
            Lines.Add(e.StackTrace);
            Lines.Add("----End of stack trace----\r\n");
            File.WriteAllLines((fullPath + date2 + ".txt"), Lines.ToArray());
            WriteLine(e.ToString());
        }

        public unsafe static string log1(string name, byte* Data, int leng)
        {
            byte[] packet = new byte[leng];
            fixed (byte* recv_ptr = packet)
            {
                Kernel32.memcpy(recv_ptr, Data, leng);
            }
            return log2(name, packet);
        }
        public unsafe static string log2(string PacketName, byte[] Data)
        {
            try
            {

                if (Data == null) return "";
                ushort PacketLength = (ushort)Data.Length;
                if (ASCIIEncoding.ASCII.GetString(Data).Contains("TQServer") || ASCIIEncoding.ASCII.GetString(Data).Contains("TQClient"))
                    PacketLength += 8;
                if (PacketLength > Data.Length) PacketLength = (ushort)Data.Length;

                string DataStr = "";
                DataStr += PacketName + ", Packet Length : " + PacketLength + ", PacketType: " + BitConverter.ToInt16(Data, 2) + Environment.NewLine;


                for (int i = 0; i < Math.Ceiling((double)PacketLength / 16); i++)
                {
                    int t = 16;
                    if (((i + 1) * 16) > PacketLength)
                        t = PacketLength - (i * 16);
                    for (int a = 0; a < t; a++)
                    {
                        DataStr += Data[i * 16 + a].ToString("X2") + " ";

                    }
                    if (t < 16)
                        for (int a = t; a < 16; a++)
                            DataStr += "   ";
                    DataStr += "     ;";

                    for (int a = 0; a < t; a++)//unde e clasa de sockete
                    {
                        DataStr += Convert.ToChar(Data[i * 16 + a]);
                    }
                    DataStr += Environment.NewLine;
                }
                DataStr.Replace(Convert.ToChar(0), '.');
                DataStr += Environment.NewLine;
                return DataStr;
            }
            catch (Exception Exc)
            {
                MyConsole.WriteException(Exc);
            }
            return "";
        }
        public unsafe static void PrintPacketAdvanced(byte* Data, int leng)
        {
#if TEST
            byte[] packet = new byte[leng];
            fixed (byte* recv_ptr = packet)
            {
                Kernel32.memcpy(recv_ptr, Data, leng);
            }
            PrintPacketAdvanced(packet);
#endif
        }
        public unsafe static void PrintPacketAdvanced(byte* Data)
        {
#if TEST
            ushort Size =*((ushort*)Data);
            byte[] packet = new byte[Size];
            for (int x = 0; x < Size; x++)
                packet[x] = Data[x];
            PrintPacketAdvanced(packet);
#endif
        }
        public static void SavePacket(byte[] Data)
        {
#if TEST
            try
            {

                if (Data == null) return;
                ushort PacketLength = (ushort)Data.Length;
                if (ASCIIEncoding.ASCII.GetString(Data).Contains("TQServer") || ASCIIEncoding.ASCII.GetString(Data).Contains("TQClient"))
                    PacketLength += 8;
                if (PacketLength > Data.Length) PacketLength = (ushort)Data.Length;

                string DataStr = "";
                DataStr += "Packet Length : " + PacketLength + ", PacketType: " + BitConverter.ToInt16(Data, 2) + Environment.NewLine;


                for (int i = 0; i < Math.Ceiling((double)PacketLength / 16); i++)
                {
                    int t = 16;
                    if (((i + 1) * 16) > PacketLength)
                        t = PacketLength - (i * 16);
                    for (int a = 0; a < t; a++)
                    {
                        DataStr += Data[i * 16 + a].ToString("X2") + " ";

                    }
                    if (t < 16)
                        for (int a = t; a < 16; a++)
                            DataStr += "   ";
                    DataStr += "     ;";

                    for (int a = 0; a < t; a++)//unde e clasa de sockete
                    {
                        DataStr += Convert.ToChar(Data[i * 16 + a]);
                    }
                    DataStr += Environment.NewLine;
                }
                DataStr.Replace(Convert.ToChar(0), '.');
                DataStr += Environment.NewLine;
                System.IO.StreamWriter SW = new System.IO.StreamWriter(@"C:\PacketSniffing\P" + 1 + ".txt", true);
                SW.WriteLine(DataStr);
                SW.Flush();
                SW.Close();
                //MyConsole.WriteLine(DataStr);
            }
            catch (Exception Exc)
            {
                MyConsole.WriteException(Exc);
            }
#endif
        }
        public static void PrintPacketAdvanced(byte[] Data)
        {
#if TEST
            try
            {

                if (Data == null) return;
                ushort PacketLength = (ushort)Data.Length;
                if (ASCIIEncoding.ASCII.GetString(Data).Contains("TQServer") || ASCIIEncoding.ASCII.GetString(Data).Contains("TQClient"))
                    PacketLength += 8;
                if (PacketLength > Data.Length) PacketLength = (ushort)Data.Length;

                string DataStr = "";
                DataStr += "Packet Length : " + PacketLength + ", PacketType: " + BitConverter.ToInt16(Data, 2) + Environment.NewLine;


                for (int i = 0; i < Math.Ceiling((double)PacketLength / 16); i++)
                {
                    int t = 16;
                    if (((i + 1) * 16) > PacketLength)
                        t = PacketLength - (i * 16);
                    for (int a = 0; a < t; a++)
                    {
                        DataStr += Data[i * 16 + a].ToString("X2") + " ";

                    }
                    if (t < 16)
                        for (int a = t; a < 16; a++)
                            DataStr += "   ";
                    DataStr += "     ;";

                    for (int a = 0; a < t; a++)//unde e clasa de sockete
                    {
                        DataStr += Convert.ToChar(Data[i * 16 + a]);
                    }
                    DataStr += Environment.NewLine;
                }
                DataStr.Replace(Convert.ToChar(0), '.');
                DataStr += Environment.NewLine;
                System.IO.StreamWriter SW = new System.IO.StreamWriter(@"C:\PacketSniffing\P" + 1 + ".txt", true);
                SW.WriteLine(DataStr);
                SW.Flush();
                SW.Close();
                MyConsole.WriteLine(DataStr);
            }
            catch (Exception Exc)
            {
                MyConsole.WriteException(Exc);
            }
#endif
        }
    }
}
