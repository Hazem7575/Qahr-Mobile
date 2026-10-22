using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PatchConsole
{
    /// <summary>Reads only the non-secret keys this tool needs from ServerConfig.ini and writes single keys back
    /// (the rest of the file is kept as it is, a .bak copy is made first). Secret values are never read into the UI.</summary>
    public sealed class ServerConfig
    {
        public readonly string Path;
        readonly Dictionary<string, string> map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public ServerConfig(string path)
        {
            Path = path;
            string section = "";
            foreach (string raw in File.ReadAllLines(path))
            {
                string l = raw.Trim();
                if (l.Length == 0 || l.StartsWith(";") || l.StartsWith("#"))
                    continue;
                if (l.StartsWith("[") && l.EndsWith("]"))
                {
                    section = l.Substring(1, l.Length - 2).Trim();
                    continue;
                }
                int eq = l.IndexOf('=');
                if (eq <= 0)
                    continue;
                string key = section + "/" + l.Substring(0, eq).Trim();
                string v = l.Substring(eq + 1);
                int c = v.IndexOf(" ;");
                if (c >= 0)
                    v = v.Substring(0, c);
                if (!map.ContainsKey(key))
                    map[key] = v.Trim();
            }
        }

        public string Get(string section, string key, string def)
        {
            string v;
            return map.TryGetValue(section + "/" + key, out v) && v.Length > 0 ? v : def;
        }

        public int GetInt(string section, string key, int def)
        {
            int v;
            return int.TryParse(Get(section, key, ""), out v) ? v : def;
        }

        // the same defaults as Qahr.Mobile.Login (MobileAutoPatch.cs / MobileAutoPatchUpdate.cs)
        public string Mode => Get("AutoPatch", "Mode", "Ready");
        public bool UpdateMode => string.Equals(Mode, "Update", StringComparison.OrdinalIgnoreCase);
        public int CheckPort => GetInt("Server", "AutoPatchPort", 9547);
        public int HttpPort => GetInt("AutoPatch", "HttpPort", 9549);
        public int RegisterPort => GetInt("Server", "MobileRegisterPort", 0);
        public int LoginPort => GetInt("Server", "LoginPort", 0);
        public int GamePort => GetInt("Server", "GamePort", 0);
        public string ServerIp => Get("Server", "IP", "127.0.0.1");
        public bool PatchDirSet => Get("AutoPatch", "PatchDir", "").Length > 0;

        public string PatchDir
        {
            get
            {
                string data = Get("Server", "DataFolder", "");
                if (data.Length == 0)
                    data = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Path), "..", "Data");
                return System.IO.Path.GetFullPath(Get("AutoPatch", "PatchDir", System.IO.Path.Combine(data, "mobile_autopatch")));
            }
        }

        public string UrlBase
        {
            get
            {
                string u = Get("AutoPatch", "UrlBase", ServerIp + ":" + HttpPort + "/patch").Trim();
                if (u.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
                    u = u.Substring(7);
                return u.TrimEnd('/');
            }
        }

        /// <summary>Port problems in Arabic (empty = fine).</summary>
        public List<string> PortProblems(int check, int http)
        {
            var r = new List<string>();
            var used = new Dictionary<int, string> { { LoginPort, "تسجيل الدخول LoginPort" }, { GamePort, "سيرفر اللعبة GamePort" }, { RegisterPort, "التسجيل MobileRegisterPort" } };
            foreach (var p in new[] { new { P = check, N = "فحص الإصدار" }, new { P = http, N = "تحميل HTTP" } })
            {
                if (p.P < 1 || p.P > 65535)
                    r.Add(p.N + ": رقم بورت غير صحيح");
                else if (used.ContainsKey(p.P) && p.P != 0)
                    r.Add(p.N + " " + p.P + " مستخدم بالفعل لـ " + used[p.P]);
            }
            if (check == http)
                r.Add("بورت الفحص وبورت HTTP لازم يكونوا مختلفين");
            return r;
        }

        /// <summary>Sets [section] key=value, keeping every other line, the encoding and the line ends.</summary>
        public static void SetKey(string path, string section, string key, string value)
        {
            byte[] bytes = File.ReadAllBytes(path);
            bool bom = bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF;
            string text = new UTF8Encoding(false).GetString(bytes, bom ? 3 : 0, bytes.Length - (bom ? 3 : 0));
            string nl = text.Contains("\r\n") ? "\r\n" : "\n";
            var lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None).ToList();
            int sec = -1, end = lines.Count;
            for (int i = 0; i < lines.Count; i++)
            {
                string t = lines[i].Trim();
                if (t.StartsWith("[") && t.EndsWith("]"))
                {
                    if (sec >= 0) { end = i; break; }
                    if (string.Equals(t, "[" + section + "]", StringComparison.OrdinalIgnoreCase))
                        sec = i;
                }
            }
            string line = key + "=" + value;
            if (sec < 0)
            {
                while (lines.Count > 0 && lines[lines.Count - 1].Length == 0)
                    lines.RemoveAt(lines.Count - 1);
                lines.Add("");
                lines.Add("[" + section + "]");
                lines.Add(line);
                lines.Add("");
            }
            else
            {
                int at = -1, commented = -1;
                for (int i = sec + 1; i < end; i++)
                {
                    if (Regex.IsMatch(lines[i], @"^\s*" + Regex.Escape(key) + @"\s*=", RegexOptions.IgnoreCase)) { at = i; break; }
                    if (commented < 0 && Regex.IsMatch(lines[i], @"^\s*;\s*" + Regex.Escape(key) + @"\s*=", RegexOptions.IgnoreCase)) commented = i;
                }
                if (at >= 0) lines[at] = line;
                else if (commented >= 0) lines.Insert(commented + 1, line);
                else lines.Insert(sec + 1, line);
            }
            var outBytes = new List<byte>();
            if (bom)
                outBytes.AddRange(new byte[] { 0xEF, 0xBB, 0xBF });
            outBytes.AddRange(new UTF8Encoding(false).GetBytes(string.Join(nl, lines)));
            File.Copy(path, path + ".bak", true);
            string tmp = path + ".tmp";
            File.WriteAllBytes(tmp, outBytes.ToArray());
            File.Move(tmp, path, true);
        }
    }

    /// <summary>What the console stores next to each patch: PatchDir\_meta\NNNN.json (the AccServer ignores it).</summary>
    public sealed class PatchMeta
    {
        public int Version { get; set; }
        public string Title { get; set; } = "";
        public string Notes { get; set; } = "";
        public DateTime Created { get; set; }
        public string Author { get; set; } = "";
        public string Sha256 { get; set; } = "";
        public long Size { get; set; }
        /// <summary>"ours" (made with this console: a hotfix or our own files) or "official" (a TQ patch republished
        /// under our number, see Official).</summary>
        public string Kind { get; set; } = "ours";
        /// <summary>The official TQ name of the patch (e.g. "5151-5171"), empty for ours.</summary>
        public string Official { get; set; } = "";
        public List<MetaFile> Files { get; set; } = new List<MetaFile>();
    }

    public sealed class MetaFile
    {
        public string Path { get; set; }
        public long Size { get; set; }
        public string Sha256 { get; set; }
        public string Status { get; set; }
    }

    public sealed class PatchInfo
    {
        public int Version { get; set; }
        public string File { get; set; }
        public long Size { get; set; }
        public int FileCount { get; set; }
        public DateTime Date { get; set; }
        public string Problem { get; set; }
        public List<string> Entries { get; } = new List<string>();
        public PatchMeta Meta { get; set; }
        public int Downloads { get; set; }// distinct devices that finished it
        public string SizeText => PatchCore.FormatSize(Size);
        public string DateText => PatchCore.Ltr((Meta != null ? Meta.Created : Date).ToString("yyyy-MM-dd  HH:mm"));
        public string DateShort => (Meta != null ? Meta.Created : Date).ToString("yyyy-MM-dd HH:mm");
        public string Title => Meta != null && Meta.Title.Length > 0 ? Meta.Title : (Meta != null ? FirstLine(Meta.Notes) : "");
        public string StateText => Problem == null ? "سليم" : "لن يرسل: " + Problem;
        /// <summary>From the official manifest (patchmirror) first, then from our own _meta file.</summary>
        public OfficialEntry OfficialInfo { get; set; }
        public bool IsOfficial => OfficialInfo != null || (Meta != null && string.Equals(Meta.Kind, "official", StringComparison.OrdinalIgnoreCase));
        public string KindText => IsOfficial ? "رسمي" : "بتاعنا";
        public string KindKey => IsOfficial ? "New" : "Ok";
        public string OfficialText => OfficialInfo != null && !string.IsNullOrEmpty(OfficialInfo.Official) ? PatchCore.Ltr(OfficialInfo.Official)
                                    : Meta != null && !string.IsNullOrEmpty(Meta.Official) ? PatchCore.Ltr(Meta.Official) : "-";
        public bool Ok => Problem == null;
        static string FirstLine(string s) => (s ?? "").Split('\n')[0].Trim();
    }

    public enum ItemStatus { New, Changed, Same, Bad }

    /// <summary>One file chosen for the next patch.</summary>
    public sealed class StageItem : System.ComponentModel.INotifyPropertyChanged
    {
        string target;
        bool include = true;
        public string Source { get; set; }
        public long Size { get; set; }
        public string Hash { get; set; }
        public ItemStatus Status { get; set; }
        public string Reason { get; set; } = "";
        public string Target { get => target; set { target = PatchCore.NormTarget(value); Changed(nameof(Target)); } }
        public bool Include { get => include; set { include = value; Changed(nameof(Include)); } }
        public string SizeText => PatchCore.FormatSize(Size);
        public string StatusText => Status == ItemStatus.New ? "جديد" : Status == ItemStatus.Changed ? "معدّل" : Status == ItemStatus.Same ? "مطابق (مستبعد)" : "مرفوض";
        public string StatusKey => Status.ToString();
        public void Refresh() { Changed(nameof(StatusText)); Changed(nameof(StatusKey)); Changed(nameof(Reason)); Changed(nameof(SizeText)); }
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        void Changed(string n) => PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(n));
    }

    public static class PatchCore
    {
        public const int BaseVersion = 5150;// version.dat of the APK
        public const string RemovedFolder = "_removed", MetaFolder = "_meta", StatsFolder = "_stats";
        public const string TestAgent = "PatchConsole-Test";
        static readonly Regex PatchName = new Regex(@"^(\d{4})\.zip$", RegexOptions.IgnoreCase);
        static readonly JsonSerializerOptions Json = new JsonSerializerOptions { WriteIndented = true };

        // ---------------------------------------------------------------------------------------------- patches
        public static List<PatchInfo> List(string dir)
        {
            var r = new List<PatchInfo>();
            if (dir == null || !Directory.Exists(dir))
                return r;
            var man = OfficialManifest.Read(dir);
            foreach (string f in Directory.GetFiles(dir, "*.zip"))
            {
                var m = PatchName.Match(Path.GetFileName(f));
                if (!m.Success)
                    continue;
                int v = int.Parse(m.Groups[1].Value);
                if (v >= 9999)
                    continue;
                var fi = new FileInfo(f);
                var p = new PatchInfo { Version = v, File = f, Size = fi.Length, Date = fi.LastWriteTime };
                p.Problem = Inspect(f, v, p.Entries);
                p.FileCount = p.Entries.Count(e => !e.Equals("version.dat", StringComparison.OrdinalIgnoreCase) && !e.EndsWith("/"));
                p.Meta = ReadMeta(dir, v);
                p.OfficialInfo = man?.Find(v);
                r.Add(p);
            }
            return r.OrderBy(p => p.Version).ToList();
        }

        public static int CurrentVersion(List<PatchInfo> list) => list.Where(p => p.Ok).Select(p => p.Version).DefaultIfEmpty(BaseVersion).Max();

        /// <summary>The highest number ever used (published or removed): a phone may already have it.</summary>
        public static int LastUsedVersion(string dir)
        {
            int max = BaseVersion;
            foreach (var p in List(dir))
                max = Math.Max(max, p.Version);
            string rem = Path.Combine(dir, RemovedFolder);
            if (Directory.Exists(rem))
                foreach (string f in Directory.GetFiles(rem))
                {
                    var m = Regex.Match(Path.GetFileName(f), @"^(\d{4})\.zip");
                    if (m.Success)
                        max = Math.Max(max, int.Parse(m.Groups[1].Value));
                }
            return max;
        }

        /// <summary>The AccServer's check plus path checks. null = good.</summary>
        public static string Inspect(string file, int version, List<string> entries)
        {
            string want = version.ToString("0000");
            try
            {
                using (var z = ZipFile.OpenRead(file))
                {
                    string problem = null, versionProblem = "لا يوجد version.dat";
                    foreach (var e in z.Entries)
                    {
                        string n = e.FullName;
                        entries?.Add(n);
                        string bad = CheckEntryName(n);
                        if (bad != null && problem == null)
                            problem = bad + ": " + n;
                        if (n == "version.dat")
                            using (var s = new StreamReader(e.Open()))
                            {
                                string v = (s.ReadLine() ?? "").Trim();
                                versionProblem = v == want ? null : "version.dat = '" + v + "' والمفروض " + want;
                            }
                    }
                    if (versionProblem != null)
                        return versionProblem;
                    if (z.Entries.Count < 2)
                        return "لا يحتوي على ملفات";
                    return problem;
                }
            }
            catch (Exception ex)
            {
                return "ملف zip تالف: " + ex.Message;
            }
        }

        public static string CheckEntryName(string n)
        {
            if (string.IsNullOrWhiteSpace(n))
                return "مسار فارغ";
            if (n.Contains("\\"))
                return "مسار فيه \\";
            if (n.StartsWith("/") || n.Contains(":"))
                return "مسار مطلق";
            if (n.EndsWith("/"))
                return null;
            foreach (string part in n.Split('/'))
                if (part.Length == 0 || part == "." || part == "..")
                    return "مسار غير صحيح";
            foreach (char c in n)
                if (c < 0x20 || c > 0x7E || c == '"' || c == '*' || c == '?' || c == '<' || c == '>' || c == '|')
                    return "اسم فيه حروف غير مسموحة (إنجليزي فقط)";
            return null;
        }

        public static string Forbidden(string target)
        {
            string name = Path.GetFileName(target).ToLowerInvariant();
            string ext = Path.GetExtension(name);
            if (name == "serverconfig.ini" || name == "shell.ini" || name.StartsWith("appsettings") ||
                new[] { ".cs", ".pdb", ".exe", ".dll", ".pfx", ".key", ".pem", ".sql", ".bak", ".log", ".zip", ".apk" }.Contains(ext))
                return "ملف ممنوع إرساله للاعبين";
            if (name == "version.dat" && !target.Contains("/"))
                return "version.dat تضيفه الأداة تلقائيا";
            return null;
        }

        public static string NormTarget(string t)
        {
            t = (t ?? "").Trim().Replace('\\', '/');
            while (t.StartsWith("./"))
                t = t.Substring(2);
            while (t.Contains("//"))
                t = t.Replace("//", "/");
            return t.TrimStart('/');
        }

        // ---------------------------------------------------------------------------------------------- compare
        public static string Sha256File(string file)
        {
            using (var s = File.OpenRead(file))
                return Convert.ToHexString(SHA256.HashData(s)).ToLowerInvariant();
        }

        /// <summary>Path -> hash of the copy the phones have now: the newest published patch holding it, else the base
        /// resources (the APK's ConquerRes).</summary>
        public sealed class Baseline
        {
            readonly string baseRoot;
            readonly Dictionary<string, string> patched = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            readonly Dictionary<string, int> patchedIn = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            public readonly Dictionary<string, List<string>> NameIndex = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

            public Baseline(string baseRoot, string patchDir)
            {
                this.baseRoot = baseRoot != null && Directory.Exists(baseRoot) ? baseRoot : null;
                foreach (var p in List(patchDir).Where(p => p.Ok))
                    using (var z = ZipFile.OpenRead(p.File))
                        foreach (var e in z.Entries)
                        {
                            if (e.FullName == "version.dat" || e.FullName.EndsWith("/"))
                                continue;
                            using (var s = e.Open())
                                patched[e.FullName] = Convert.ToHexString(SHA256.HashData(s)).ToLowerInvariant();
                            patchedIn[e.FullName] = p.Version;
                        }
                if (this.baseRoot != null)
                    foreach (string f in Directory.EnumerateFiles(this.baseRoot, "*", SearchOption.AllDirectories))
                    {
                        string rel = Path.GetRelativePath(this.baseRoot, f).Replace('\\', '/');
                        string n = Path.GetFileName(f);
                        if (!NameIndex.TryGetValue(n, out var l))
                            NameIndex[n] = l = new List<string>();
                        l.Add(rel);
                    }
            }

            /// <summary>Sets Status/Reason of one item.</summary>
            public void Classify(StageItem it)
            {
                string bad = CheckEntryName(it.Target) ?? Forbidden(it.Target);
                if (bad == null && !File.Exists(it.Source))
                    bad = "الملف غير موجود";
                if (bad != null)
                {
                    it.Status = ItemStatus.Bad;
                    it.Reason = bad;
                    it.Include = false;
                    it.Refresh();
                    return;
                }
                it.Size = new FileInfo(it.Source).Length;
                it.Hash = Sha256File(it.Source);
                string cur;
                int ver;
                if (patched.TryGetValue(it.Target, out cur))
                {
                    patchedIn.TryGetValue(it.Target, out ver);
                    it.Status = cur == it.Hash ? ItemStatus.Same : ItemStatus.Changed;
                    it.Reason = cur == it.Hash ? "نفس النسخة المنشورة في " + ver : "يختلف عن النسخة المنشورة في " + ver;
                }
                else if (baseRoot != null && File.Exists(Path.Combine(baseRoot, it.Target.Replace('/', '\\'))))
                {
                    bool same = Sha256File(Path.Combine(baseRoot, it.Target.Replace('/', '\\'))) == it.Hash;
                    it.Status = same ? ItemStatus.Same : ItemStatus.Changed;
                    it.Reason = same ? "نفس ملف النسخة الأساسية" : "يختلف عن النسخة الأساسية";
                }
                else
                {
                    it.Status = ItemStatus.New;
                    it.Reason = baseRoot == null ? "مجلد النسخة الأساسية غير محدد" : "غير موجود في النسخة الأساسية";
                }
                it.Include = it.Status != ItemStatus.Same;
                it.Refresh();
            }
        }

        // ---------------------------------------------------------------------------------------------- build / publish
        /// <summary>Builds NNNN.zip in the temp folder and checks it again after writing.</summary>
        public static string Build(int version, IList<StageItem> files, IProgress<double> progress = null)
        {
            if (version <= BaseVersion || version >= 9999)
                throw new Exception("رقم الإصدار لازم يكون 4 أرقام بين 5151 و 9998.");
            if (files.Count == 0)
                throw new Exception("لا توجد ملفات في التحديث.");
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var f in files)
            {
                string bad = CheckEntryName(f.Target) ?? Forbidden(f.Target);
                if (bad != null)
                    throw new Exception(bad + ": " + f.Target);
                if (!seen.Add(f.Target))
                    throw new Exception("نفس المسار مكرر: " + f.Target);
                if (!File.Exists(f.Source))
                    throw new Exception("الملف غير موجود: " + f.Source);
            }
            string tmp = Path.Combine(Path.GetTempPath(), "PatchConsole_" + version + "_" + Guid.NewGuid().ToString("N") + ".zip");
            using (var z = ZipFile.Open(tmp, ZipArchiveMode.Create))
            {
                int i = 0;
                foreach (var f in files)
                {
                    z.CreateEntryFromFile(f.Source, f.Target, CompressionLevel.Optimal);
                    progress?.Report(++i / (double)(files.Count + 1));
                }
                using (var s = z.CreateEntry("version.dat", CompressionLevel.Optimal).Open())
                {
                    byte[] b = Encoding.ASCII.GetBytes(version.ToString("0000"));
                    s.Write(b, 0, b.Length);
                }
            }
            progress?.Report(1);
            string problem = Inspect(tmp, version, null);
            if (problem != null)
            {
                File.Delete(tmp);
                throw new Exception("فشل التحقق: " + problem);
            }
            return tmp;
        }

        /// <summary>Copies the checked zip into PatchDir as .part with progress, checks the hash, then renames it to
        /// NNNN.zip (the AccServer only ever sees a complete file) and writes _meta\NNNN.json.</summary>
        public static async Task<string> PublishAsync(string tmpZip, PatchMeta meta, string dir, IProgress<double> progress)
        {
            Directory.CreateDirectory(dir);
            string dest = Path.Combine(dir, meta.Version.ToString("0000") + ".zip");
            if (File.Exists(dest))
                throw new Exception("التحديث " + meta.Version + " موجود بالفعل.");
            string part = dest + ".part";
            using (var src = new FileStream(tmpZip, FileMode.Open, FileAccess.Read))
            using (var dst = new FileStream(part, FileMode.Create, FileAccess.Write))
            {
                var buf = new byte[256 * 1024];
                long done = 0, total = Math.Max(1, src.Length);
                int n;
                while ((n = await src.ReadAsync(buf, 0, buf.Length)) > 0)
                {
                    await dst.WriteAsync(buf, 0, n);
                    done += n;
                    progress?.Report(done / (double)total);
                    if (total < 4 * 1024 * 1024)
                        await Task.Delay(15);// a visible progress even for small patches
                }
            }
            if (Sha256File(part) != meta.Sha256)
            {
                File.Delete(part);
                throw new Exception("الـ checksum بعد النسخ لا يطابق، لم يتم النشر.");
            }
            WriteMeta(dir, meta);
            File.Move(part, dest);
            File.Delete(tmpZip);
            return dest;
        }

        public static PatchMeta ReadMeta(string dir, int version)
        {
            try
            {
                string f = Path.Combine(dir, MetaFolder, version.ToString("0000") + ".json");
                return File.Exists(f) ? JsonSerializer.Deserialize<PatchMeta>(File.ReadAllText(f)) : null;
            }
            catch { return null; }
        }

        static void WriteMeta(string dir, PatchMeta meta)
        {
            string d = Path.Combine(dir, MetaFolder);
            Directory.CreateDirectory(d);
            File.WriteAllText(Path.Combine(d, meta.Version.ToString("0000") + ".json"), JsonSerializer.Serialize(meta, Json), new UTF8Encoding(false));
        }

        /// <summary>Takes a patch out of PatchDir. Nothing is deleted: the zip and its notes go to PatchDir\_removed.</summary>
        public static string Remove(string dir, int version)
        {
            string name = version.ToString("0000");
            string zip = Path.Combine(dir, name + ".zip");
            if (!File.Exists(zip))
                throw new Exception("التحديث " + version + " غير موجود.");
            string rem = Path.Combine(dir, RemovedFolder);
            Directory.CreateDirectory(rem);
            string stamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string dest = Path.Combine(rem, name + ".zip." + stamp);
            File.Move(zip, dest);
            string meta = Path.Combine(dir, MetaFolder, name + ".json");
            if (File.Exists(meta))
                File.Move(meta, Path.Combine(rem, name + ".json." + stamp));
            return dest;
        }

        // ---------------------------------------------------------------------------------------------- network
        public static bool IsListening(int port)
        {
            if (port <= 0)
                return false;
            try { return IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners().Any(e => e.Port == port); }
            catch { return false; }
        }

        public static async Task<bool> CanConnect(int port)
        {
            try
            {
                using (var c = new TcpClient())
                {
                    var t = c.ConnectAsync("127.0.0.1", port);
                    return await Task.WhenAny(t, Task.Delay(2000)) == t && c.Connected;
                }
            }
            catch { return false; }
        }

        /// <summary>The version chain the server answers for a phone at fromVersion (like AutoUpdateMgr::GetUpdateUrl).</summary>
        public static async Task<List<(string sent, string reply)>> AskChain(int port, int fromVersion)
        {
            var r = new List<(string, string)>();
            using (var c = new TcpClient())
            {
                var t = c.ConnectAsync("127.0.0.1", port);
                if (await Task.WhenAny(t, Task.Delay(3000)) != t || !c.Connected)
                    throw new Exception("لا يوجد رد على البورت " + port);
                var s = c.GetStream();
                string send = fromVersion.ToString();
                var seen = new HashSet<string>();
                var buf = new byte[256];
                for (int round = 0; round < 50; round++)
                {
                    byte[] b = Encoding.ASCII.GetBytes(send);
                    await s.WriteAsync(b, 0, b.Length);
                    var read = s.ReadAsync(buf, 0, buf.Length);
                    if (await Task.WhenAny(read, Task.Delay(5000)) != read)
                        throw new Exception("السيرفر لم يرد خلال 5 ثواني");
                    int n = read.Result;
                    string reply = n > 0 ? Encoding.ASCII.GetString(buf, 0, n).Trim() : "";
                    r.Add((send, reply));
                    if (!reply.Contains("UPDATE"))
                        break;
                    string url = UrlFromReply(reply);
                    if (!seen.Add(url))
                        break;
                    int dot = url.LastIndexOf('.');
                    if (dot < 4)
                        break;
                    send = url.Substring(dot - 4, 4);
                }
            }
            return r;
        }

        public static string UrlFromReply(string reply) => "http://" + Regex.Replace(reply.Trim(), " +", "/").Substring(7);

        /// <summary>The full client test: version chain from 5150, then for every zip HEAD (Content-Length), a full GET,
        /// a resumed GET (Range), the zip and its version.dat checked, and the checksum compared with the published file.
        /// Downloads are done on 127.0.0.1 with the URL's port and path, and marked as a test in the server's stats.</summary>
        public static async Task<bool> SimulateClient(int checkPort, string patchDir, Action<string, bool?> log)
        {
            bool ok = true;
            List<(string sent, string reply)> chain;
            try { chain = await AskChain(checkPort, BaseVersion); }
            catch (Exception e) { log("فحص الإصدار: " + e.Message, false); return false; }
            foreach (var (sent, reply) in chain)
                log("الموبايل أرسل " + sent + "  ←  السيرفر رد: " + Ltr(reply.Length > 0 ? reply : "(لا شيء)"), reply.Length > 0);
            if (chain.Count == 0 || chain[chain.Count - 1].reply != "READY")
            {
                log("السلسلة لم تنته بـ READY", false);
                ok = false;
            }
            using (var http = new HttpClient { Timeout = TimeSpan.FromSeconds(60) })
            {
                http.DefaultRequestHeaders.UserAgent.ParseAdd(TestAgent);
                foreach (var (_, reply) in chain.Where(c => c.reply.Contains("UPDATE")))
                {
                    var u = new Uri(UrlFromReply(reply));
                    string local = "http://127.0.0.1:" + u.Port + u.AbsolutePath;
                    string name = Path.GetFileName(u.AbsolutePath);
                    try
                    {
                        var head = await http.SendAsync(new HttpRequestMessage(HttpMethod.Head, local));
                        long? len = head.Content.Headers.ContentLength;
                        log(Ltr("HEAD " + name) + ": " + (int)head.StatusCode + ", الحجم " + (len.HasValue ? FormatSize(len.Value) : "غير معروف"), head.IsSuccessStatusCode && len.HasValue);
                        byte[] data = await http.GetByteArrayAsync(local);
                        string tmp = Path.Combine(Path.GetTempPath(), "PatchConsole_test_" + name);
                        File.WriteAllBytes(tmp, data);
                        int v = int.Parse(Path.GetFileNameWithoutExtension(name));
                        string problem = Inspect(tmp, v, null);
                        log("تنزيل " + Ltr(name) + ": " + FormatSize(data.Length) + (problem == null ? "، الملف سليم و version.dat = " + v : "، مشكلة: " + problem), problem == null);
                        ok &= problem == null && head.IsSuccessStatusCode;
                        string published = Path.Combine(patchDir, name);
                        if (File.Exists(published))
                        {
                            bool same = Sha256File(tmp) == Sha256File(published);
                            log("checksum " + Ltr(name) + (same ? " مطابق للملف المنشور" : " لا يطابق الملف المنشور!"), same);
                            ok &= same;
                        }
                        File.Delete(tmp);
                        if (data.Length > 10)
                        {
                            var req = new HttpRequestMessage(HttpMethod.Get, local);
                            req.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(10, null);
                            var part = await http.SendAsync(req);
                            byte[] rest = await part.Content.ReadAsByteArrayAsync();
                            bool resume = (int)part.StatusCode == 206 && rest.Length == data.Length - 10;
                            log("استكمال تنزيل مقطوع (Range): " + (int)part.StatusCode + (resume ? " سليم" : " غير سليم"), resume);
                            ok &= resume;
                        }
                    }
                    catch (Exception e)
                    {
                        log(Ltr(name) + ": " + e.Message, false);
                        ok = false;
                    }
                }
            }
            return ok;
        }

        // ---------------------------------------------------------------------------------------------- misc
        /// <summary>Keeps numbers, units, dates and IPs in left-to-right order inside Arabic text.</summary>
        public static string Ltr(string s) => "\u200E" + s + "\u200E";

        public static string FormatSize(long b) => Ltr(FormatSizeRaw(b));

        static string FormatSizeRaw(long b)
        {
            if (b < 1024) return b + " B";
            if (b < 1024 * 1024) return (b / 1024.0).ToString("0.0") + " KB";
            if (b < 1024L * 1024 * 1024) return (b / 1024.0 / 1024.0).ToString("0.00") + " MB";
            return (b / 1024.0 / 1024.0 / 1024.0).ToString("0.00") + " GB";
        }

        public static string Ago(DateTime t)
        {
            var d = DateTime.Now - t;
            if (d.TotalSeconds < 60) return "منذ لحظات";
            if (d.TotalMinutes < 60) return "منذ " + (int)d.TotalMinutes + " دقيقة";
            if (d.TotalHours < 24) return "منذ " + (int)d.TotalHours + " ساعة";
            return "منذ " + (int)d.TotalDays + " يوم";
        }

        public static string FindConfig()
        {
            string dir = AppContext.BaseDirectory;
            for (int i = 0; i < 10 && !string.IsNullOrEmpty(dir); i++)
            {
                foreach (string p in new[] { Path.Combine(dir, "src", "Qahr.Mobile.Shared", "ServerConfig.ini"), Path.Combine(dir, "ServerConfig.ini") })
                    if (File.Exists(p))
                        return p;
                dir = Path.GetDirectoryName(dir.TrimEnd('\\', '/'));
            }
            string def = @"D:\conquer\QahrMobile\Qahr.Shared\ServerConfig.ini";
            return File.Exists(def) ? def : null;
        }
    }

    /// <summary>One official TQ patch as republished by us (written by patchmirror).</summary>
    public sealed class OfficialEntry
    {
        public int Ours { get; set; }
        public string Official { get; set; } = "";
        public string Kind { get; set; } = "official";
        public long Size { get; set; }
        public string Sha256 { get; set; } = "";
        public string SourceUrl { get; set; } = "";
        public DateTime? Created { get; set; }
        public string Status { get; set; } = "";
        public string OursText => Ours > 0 ? PatchCore.Ltr(Ours + ".zip") : "لم ينشر بعد";
        public string OfficialText => PatchCore.Ltr(Official ?? "");
        public string SizeText => Size > 0 ? PatchCore.FormatSize(Size) : "";
        public string CreatedText => Created.HasValue ? PatchCore.Ltr(Created.Value.ToString("yyyy-MM-dd  HH:mm")) : "";
    }

    /// <summary>The official-patch manifest kept by patchmirror: PatchDir\_official\manifest.json
    ///   {"latestOfficial":"5171","checkedAt":"...","source":"...","patches":[{"ours":5171,"official":"5151-5171",
    ///    "kind":"official","size":0,"sha256":"","sourceUrl":"","created":"...","status":""}]}
    /// Read only; a missing or unreadable file just means "no data yet".</summary>
    public sealed class OfficialManifest
    {
        public string LatestOfficial { get; set; } = "";
        public DateTime? CheckedAt { get; set; }
        public string Source { get; set; } = "";
        public List<OfficialEntry> Patches { get; set; } = new List<OfficialEntry>();
        public string Error;

        public static string PathOf(string patchDir) => Path.Combine(patchDir, "_official", "manifest.json");

        public static OfficialManifest Read(string patchDir)
        {
            string f = patchDir == null ? null : PathOf(patchDir);
            if (f == null || !File.Exists(f))
                return null;
            try
            {
                var o = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString };
                return JsonSerializer.Deserialize<OfficialManifest>(File.ReadAllText(f), o) ?? new OfficialManifest();
            }
            catch (Exception e)
            {
                return new OfficialManifest { Error = e.Message };
            }
        }

        public OfficialEntry Find(int ours) => Patches?.Find(p => p.Ours == ours);
    }
}
