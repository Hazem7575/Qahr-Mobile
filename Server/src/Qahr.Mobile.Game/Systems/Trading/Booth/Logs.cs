using System;
using System.IO;

namespace Qahr.Mobile
{
    public static class Logs
    {
        public static void BoothsLog(string vendor, string buying, string moneyamount, Game.MsgServer.MsgGameItem item)
        {
            String folderN = DateTime.Now.Year + "-" + DateTime.Now.Month, path = "LoggServer\\Logs\\Booths\\", newPath = Path.Combine(path, folderN);
            FileExists(newPath, folderN, path);

            using (StreamWriter file = new StreamWriter(newPath + "\\" + DateTime.Now.Day + " - " + DateTime.Now.DayOfWeek.ToString() + ".txt", true))
            {
                file.WriteLine("{0} HAS BOUGHT AN ITEM : {2} FROM {1} SHOP - for {3}", vendor, buying, item.ToLog(), moneyamount);
                file.WriteLine("------------------------------------------------------------------------------------");
            }
        }

        public static void FileExists(string newPath, string folderN, string path)
        {
            if (!File.Exists(newPath + folderN))
                Directory.CreateDirectory(Path.Combine(path, folderN));

            if (!File.Exists(newPath + "\\" + DateTime.Now.Day + " - " + DateTime.Now.DayOfWeek.ToString() + ".txt"))
                using (FileStream fs = File.Create(newPath + "\\" + DateTime.Now.Day + " - " + DateTime.Now.DayOfWeek.ToString() + "" + ".txt"))
                    fs.Close();
        }
    }
}
