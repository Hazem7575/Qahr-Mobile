using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PatchConsole
{
    /// <summary>One line of PatchDir\_stats\downloads.log, written by the AccServer (MobileAutoPatchStats.cs):
    /// time, kind (check|get), masked ip, device hash, version, result, bytes, agent.</summary>
    public sealed class StatEvent
    {
        public DateTime Time { get; set; }
        public string Kind { get; set; }
        public string Ip { get; set; }
        public string Device { get; set; }
        public string Version { get; set; }
        public string Result { get; set; }
        public long Bytes { get; set; }
        public string Agent { get; set; }
        public bool IsTest => Agent != null && Agent.StartsWith(PatchCore.TestAgent);
        public string TimeText => PatchCore.Ltr(Time.ToString("yyyy-MM-dd  HH:mm:ss"));
        public string IpText => PatchCore.Ltr(Ip);
        public string KindText => Kind == "get" ? "تنزيل" : "فحص إصدار";
        public string ResultText
        {
            get
            {
                if (Kind == "check")
                    return Result == "READY" ? "محدّث (READY)" : "عُرض عليه " + Result;
                return Result == "ok" ? "نجح" : Result == "fail" ? "فشل / انقطع" : Result == "notfound" ? "غير موجود" : Result;
            }
        }
        public string ResultKey => Kind == "check" ? "Info" : Result == "ok" ? "Ok" : "Bad";
        public string BytesText => Bytes > 0 ? PatchCore.FormatSize(Bytes) : "";
        public string DeviceText => IsTest ? "اختبار الأداة" : Device;
    }

    public static class Stats
    {
        public static string LogPath(string patchDir) => Path.Combine(patchDir, PatchCore.StatsFolder, "downloads.log");

        public static List<StatEvent> Read(string patchDir, bool includeTests)
        {
            var r = new List<StatEvent>();
            string f = patchDir == null ? null : LogPath(patchDir);
            if (f == null || !File.Exists(f))
                return r;
            string text;
            using (var s = new FileStream(f, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
            using (var rd = new StreamReader(s, Encoding.UTF8))
                text = rd.ReadToEnd();
            foreach (string line in text.Split('\n'))
            {
                string[] c = line.TrimEnd('\r').Split('\t');
                if (c.Length < 7)
                    continue;
                DateTime t;
                if (!DateTime.TryParse(c[0], out t))
                    continue;
                long b;
                long.TryParse(c[6], out b);
                var e = new StatEvent { Time = t, Kind = c[1], Ip = c[2], Device = c[3], Version = c[4], Result = c[5], Bytes = b, Agent = c.Length > 7 ? c[7] : "" };
                if (includeTests || !e.IsTest)
                    r.Add(e);
            }
            return r;
        }

        /// <summary>Distinct devices that finished downloading each version.</summary>
        public static Dictionary<int, int> DownloadsPerVersion(List<StatEvent> ev)
        {
            return ev.Where(e => e.Kind == "get" && e.Result == "ok")
                     .GroupBy(e => e.Version)
                     .Where(g => int.TryParse(g.Key, out _))
                     .ToDictionary(g => int.Parse(g.Key), g => g.Select(e => e.Device).Distinct().Count());
        }
    }

    /// <summary>The console's own operations log: PatchDir\_meta\operations.log (shared by everyone who runs the console
    /// on this server) with a copy in %LOCALAPPDATA%\PatchConsole when the folder is not writable.</summary>
    public static class OpsLog
    {
        public static string Dir;

        static string File_ => Dir != null ? Path.Combine(Dir, PatchCore.MetaFolder, "operations.log")
                                           : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PatchConsole", "operations.log");

        public static event Action<string> Added;

        public static void Write(string level, string text)
        {
            string line = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t" + level + "\t" + Environment.UserName + "\t" + text.Replace("\r", " ").Replace("\n", " | ");
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(File_));
                File.AppendAllText(File_, line + "\r\n", new UTF8Encoding(false));
            }
            catch { }
            Added?.Invoke(line);
        }

        public static List<string[]> Read(int max)
        {
            try
            {
                if (!File.Exists(File_))
                    return new List<string[]>();
                return File.ReadAllLines(File_).Reverse().Take(max).Select(l => l.Split('\t')).Where(c => c.Length >= 4).ToList();
            }
            catch { return new List<string[]>(); }
        }
    }
}
