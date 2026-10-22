using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Qahr.Mobile
{
    /// <summary>
    /// Bounded, background file logger. Keeps disk I/O off the receive / save threads so that
    /// a flood of chat/trade packets can no longer stall player saving or fill the disk at line
    /// rate. When the queue is full new entries are dropped (and counted) instead of blocking.
    /// </summary>
    public static class AsyncLog
    {
        private struct Entry { public string Path; public string Text; }

        private static readonly System.Collections.Concurrent.ConcurrentQueue<Entry> _queue
            = new System.Collections.Concurrent.ConcurrentQueue<Entry>();
        private static int _count;
        private static long _dropped;
        private const int MaxQueued = 20000;      // hard cap on buffered lines
        private const int MaxPerDrain = 5000;     // writers held open per drain pass

        private static readonly System.Threading.AutoResetEvent _signal
            = new System.Threading.AutoResetEvent(false);

        static AsyncLog()
        {
            var t = new System.Threading.Thread(Loop)
            {
                IsBackground = true,
                Name = "AsyncLog"
            };
            t.Start();
        }

        public static void Enqueue(string path, string text)
        {
            if (string.IsNullOrEmpty(path))
                return;
            if (System.Threading.Interlocked.CompareExchange(ref _count, 0, 0) >= MaxQueued)
            {
                System.Threading.Interlocked.Increment(ref _dropped);
                return;
            }
            _queue.Enqueue(new Entry { Path = path, Text = text });
            System.Threading.Interlocked.Increment(ref _count);
            _signal.Set();
        }

        private static void Loop()
        {
            while (true)
            {
                try
                {
                    _signal.WaitOne(1000);
                    Drain();
                }
                catch { /* never let the logger thread die */ }
            }
        }

        private static void Drain()
        {
            var writers = new Dictionary<string, StreamWriter>();
            try
            {
                Entry e;
                int processed = 0;
                while (_queue.TryDequeue(out e))
                {
                    System.Threading.Interlocked.Decrement(ref _count);
                    StreamWriter sw;
                    if (!writers.TryGetValue(e.Path, out sw))
                    {
                        try
                        {
                            string dir = Path.GetDirectoryName(e.Path);
                            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                                Directory.CreateDirectory(dir);
                            sw = new StreamWriter(e.Path, true);
                            writers[e.Path] = sw;
                        }
                        catch { continue; }
                    }
                    try { sw.WriteLine(e.Text); }
                    catch { }
                    if (++processed >= MaxPerDrain)
                        break;
                }
            }
            finally
            {
                foreach (var sw in writers.Values)
                {
                    try { sw.Flush(); sw.Close(); }
                    catch { }
                }
            }
        }
    }
}

namespace Qahr.Mobile.Game
{
    class ServerLogs
    {

        public static void AddTradeLog(Role.Instance.Trade first, Client.GameClient client, Role.Instance.Trade second, String secondN) // التريد مابين اللاعبين سواء في الجولد او السيبي اس او الايتم 
        {
            try
            {
                if (Program.ServerConfig.IsInterServer)
                    return;

                String folderN = client.Player.Name.RemoveIllegalCharacters(false, true),
                    Path = "[All Server Logs]\\[Trade Logs]\\",
                    NewPath = System.IO.Path.Combine(Path, folderN);
                string fileName = NewPath + "\\" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + ".txt";

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("**************************************************************************************************************************************");
                sb.AppendLine("You Traded With : [ " + secondN + " ]");
                sb.AppendLine("You Sent : [ " + first.Money + " ] Money to [ " + secondN + " ]");
                sb.AppendLine("You Sent :  [ " + first.ConquerPoints + " ] CPs to [ " + secondN + " ]");
                sb.AppendLine("You received : [ " + second.Money + " ] Money From [ " + secondN + " ]");
                sb.AppendLine("You received :  [ " + second.ConquerPoints + " ] CPs From [ " + secondN + " ]");
                foreach (var i in first.Items.Values)
                    sb.AppendLine("You Sent : " + i.ToLog());
                foreach (var i in second.Items.Values)
                    sb.AppendLine("You received : " + i.ToLog());
                sb.AppendLine("Trading Date : [ " + DateTime.Now.Month + " - " + DateTime.Now.Day + " - " + DateTime.Now.Year + " ]  [Hour] : [ " + DateTime.Now.Hour + " ] [Minute] : [ " + DateTime.Now.Minute + " ] [Second] : [ " + DateTime.Now.Second + " ]");
                sb.Append("**************************************************************************************************************************************");
                AsyncLog.Enqueue(fileName, sb.ToString());
            }
            catch
            {
                MyConsole.WriteLine(client.Player.Name.ToString().Replace("~", "").Replace("#!", "").Replace("@", "").Replace("$", "").Replace("%", "").Replace("^", "").Replace("&", "").Replace("+", "").Replace(",", "").Replace("(", "").Replace("(", "").Replace(")", "").Replace("_", "").Replace("-", "").Replace("+", "").Replace("/", "").Replace("*", "").Replace("-", "")
                    .Replace("/", "").Replace("<", "").Replace(">", "").Replace(";", "").Replace(":", ""));

            }
        }
        public static void AddChatLog(MsgServer.MsgMessage msg, Client.GameClient client, string bc = "") // جميع الشاتات بتاعت اللاعبين الى بيكلمو فيها بعضهم وكمان معاهم البوردكاست أي لاعب بيكتب رسالة عليه بتتسجل
        {
            try
            {
                if (Program.ServerConfig.IsInterServer)
                    return;

                String folderN = client.Player.Name.RemoveIllegalCharacters(false, true),
                 Path = "[All Server Logs]\\[All Players Chats]\\",
                 NewPath = System.IO.Path.Combine(Path, folderN);
                string fileName = NewPath + "\\" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + ".txt";

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("**************************************************************************************************************************************");
                if (msg != null)
                {
                    sb.AppendLine("Your Chat With : [ " + msg._To + " ]");
                    sb.AppendLine("Messages Sent : [ " + msg.__Message + " ]");
                }
                if (bc != "")
                {
                    sb.AppendLine("Your Broadcast Messages :");
                    sb.AppendLine("Messages Sent: [ " + bc + " ]");
                }
                sb.AppendLine("Chats Date : [ " + DateTime.Now.Month + " - " + DateTime.Now.Day + " - " + DateTime.Now.Year + " ]  [Hour] : [ " + DateTime.Now.Hour + " ] [Minute] : [ " + DateTime.Now.Minute + " ] [Second] : [ " + DateTime.Now.Second + " ]");
                sb.Append("**************************************************************************************************************************************");
                AsyncLog.Enqueue(fileName, sb.ToString());
            }
            catch
            {
                MyConsole.WriteLine(client.Player.Name.ToString().Replace("~", "").Replace("#!", "").Replace("@", "").Replace("$", "").Replace("%", "").Replace("^", "").Replace("&", "").Replace("+", "").Replace(",", "").Replace("(", "").Replace("(", "").Replace(")", "").Replace("_", "").Replace("-", "").Replace("+", "").Replace("/", "").Replace("*", "").Replace("-", "")
                    .Replace("/", "").Replace("<", "").Replace(">", "").Replace(";", "").Replace(":", ""));

            }

        }
        public static void AddBuyingLogShopMallCps(Client.GameClient client, Database.ItemType.DBItem Item, string price, string type) // الأيتم الى بتتشرا من الشوب ميل بسيبي اس
        {
            try
            {
                if (Program.ServerConfig.IsInterServer)
                    return;

                String folderN = client.Player.Name.RemoveIllegalCharacters(false, true),
                          Path = "[All Server Logs]\\Shopping Mall [CPs]\\",
                NewPath = System.IO.Path.Combine(Path, folderN);
                string fileName = NewPath + "\\" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + ".txt";
                string nl = Environment.NewLine;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("**************************************************************************************************************************************");
                sb.AppendLine(string.Format("All Items Purchased From The Shopping Mall For CPs : " + nl + " {0} " + nl + " ID : {1} " + nl + " Price : {2} {3}" + nl + " Chats Date : [ " + DateTime.Now.Month + " - " + DateTime.Now.Day + " - " + DateTime.Now.Year + " ]  [Hour] : [ " + DateTime.Now.Hour + " ] [Minute] : [ " + DateTime.Now.Minute + " ] [Second] : [ " + DateTime.Now.Second + " ] ", Item.Name, Item.ID, price, type));
                sb.Append("**************************************************************************************************************************************");
                AsyncLog.Enqueue(fileName, sb.ToString());
            }
            catch
            {
                MyConsole.WriteLine(client.Player.Name.ToString().Replace("~", "").Replace("#!", "").Replace("@", "").Replace("$", "").Replace("%", "").Replace("^", "").Replace("&", "").Replace("+", "").Replace(",", "").Replace("(", "").Replace("(", "").Replace(")", "").Replace("_", "").Replace("-", "").Replace("+", "").Replace("/", "").Replace("*", "").Replace("-", "")
                    .Replace("/", "").Replace("<", "").Replace(">", "").Replace(";", "").Replace(":", ""));

            }
        }
        public static void AddBuyingLogShopMallBCps(Client.GameClient client, Database.ItemType.DBItem Item, string price, string type) // الأيتم الى بتتشرا من الشوب ميل بسيبي اس بوند
        {
            try
            {
                if (Program.ServerConfig.IsInterServer)
                    return;

                String folderN = client.Player.Name.RemoveIllegalCharacters(false, true),
                          Path = "[All Server Logs]\\Shopping Mall CPs [B]\\",
                NewPath = System.IO.Path.Combine(Path, folderN);
                string fileName = NewPath + "\\" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + ".txt";
                string nl = Environment.NewLine;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("**************************************************************************************************************************************");
                sb.AppendLine(string.Format("All Items Purchased From The Shopping Mall For CPs Bound : " + nl + " {0} " + nl + " ID : {1} " + nl + " Price : {2} {3}" + nl + " Chats Date : [ " + DateTime.Now.Month + " - " + DateTime.Now.Day + " - " + DateTime.Now.Year + " ]  [Hour] : [ " + DateTime.Now.Hour + " ] [Minute] : [ " + DateTime.Now.Minute + " ] [Second] : [ " + DateTime.Now.Second + " ] ", Item.Name, Item.ID, price, type));
                sb.Append("**************************************************************************************************************************************");
                AsyncLog.Enqueue(fileName, sb.ToString());
            }
            catch
            {
                MyConsole.WriteLine(client.Player.Name.ToString().Replace("~", "").Replace("#!", "").Replace("@", "").Replace("$", "").Replace("%", "").Replace("^", "").Replace("&", "").Replace("+", "").Replace(",", "").Replace("(", "").Replace("(", "").Replace(")", "").Replace("_", "").Replace("-", "").Replace("+", "").Replace("/", "").Replace("*", "").Replace("-", "")
                    .Replace("/", "").Replace("<", "").Replace(">", "").Replace(";", "").Replace(":", ""));

            }
        }
        public static void AddBuyingLogShopMallGold(Client.GameClient client, Database.ItemType.DBItem Item, string price, string type) // الأيتم الى بتتشرا من الشوب ميل بفلوس
        {
            try
            {
                if (Program.ServerConfig.IsInterServer)
                    return;

                String folderN = client.Player.Name.RemoveIllegalCharacters(false, true),
                         Path = "[All Server Logs]\\Shopping Mall [Money]\\",
                NewPath = System.IO.Path.Combine(Path, folderN);
                string fileName = NewPath + "\\" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + ".txt";
                string nl = Environment.NewLine;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("**************************************************************************************************************************************");
                sb.AppendLine(string.Format("All Items Purchased From The Shopping Mall For Money : " + nl + " {0} " + nl + " ID : {1} " + nl + " Price : {2} {3}" + nl + " Chats Date : [ " + DateTime.Now.Month + " - " + DateTime.Now.Day + " - " + DateTime.Now.Year + " ]  [Hour] : [ " + DateTime.Now.Hour + " ] [Minute] : [ " + DateTime.Now.Minute + " ] [Second] : [ " + DateTime.Now.Second + " ] ", Item.Name, Item.ID, price, type));
                sb.Append("**************************************************************************************************************************************");
                AsyncLog.Enqueue(fileName, sb.ToString());
            }
            catch
            {
                MyConsole.WriteLine(client.Player.Name.ToString().Replace("~", "").Replace("#!", "").Replace("@", "").Replace("$", "").Replace("%", "").Replace("^", "").Replace("&", "").Replace("+", "").Replace(",", "").Replace("(", "").Replace("(", "").Replace(")", "").Replace("_", "").Replace("-", "").Replace("+", "").Replace("/", "").Replace("*", "").Replace("-", "")
                    .Replace("/", "").Replace("<", "").Replace(">", "").Replace(";", "").Replace(":", ""));

            }

        }
    }
}
