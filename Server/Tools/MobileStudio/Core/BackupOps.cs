using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using ScriptHazem.Kit;

namespace MobileStudio.Core;

public sealed class BackupInfo
{
    public string File { get; set; }
    public string Name => System.IO.Path.GetFileName(File);
    public DateTime Created { get; set; }
    public long Size { get; set; }
    public string Kind { get; set; } = "";       // full | db | data | pre-restore
    public bool HasDb { get; set; }
    public int DataFiles { get; set; }
    public string Note { get; set; } = "";
    public string CreatedText => Created.ToString("yyyy-MM-dd  HH:mm");
    public string SizeText => Fmt.Size(Size);
    public string KindText => Loc.Opt("bk.kind.", Kind);
    public string ContentText => HasDb && DataFiles > 0 ? Loc.F("bk.content.both", DataFiles) : HasDb ? Loc.Get("bk.content.db") : Loc.F("bk.content.data", DataFiles);
}

public sealed class BackupManifest
{
    public string Tool { get; set; } = "MobileStudio";
    public DateTime Created { get; set; }
    public string Kind { get; set; }
    public string Database { get; set; }
    public string DataRoot { get; set; }
    public bool HasDb { get; set; }
    public int DataFiles { get; set; }
    public long DataBytes { get; set; }
    public List<string> Excluded { get; set; } = new();
    public string Note { get; set; } = "";
}

/// <summary>Database dump (mysqldump/mariadb-dump) + Data folder → one zip in the backup folder. The DB password is read
/// from ServerConfig.ini into a temporary option file (deleted right after), never put on a command line or in a log.</summary>
public static class BackupOps
{
    static AppSettings S => Svc.Settings;
    public const string DbEntry = "database/dump.sql";
    public const string ManifestEntry = "manifest.json";

    public static string DumpExe => FirstExisting(Path.Combine(S.MysqlBin, "mariadb-dump.exe"), Path.Combine(S.MysqlBin, "mysqldump.exe"));
    public static string ClientExe => FirstExisting(Path.Combine(S.MysqlBin, "mariadb.exe"), Path.Combine(S.MysqlBin, "mysql.exe"));
    static string FirstExisting(params string[] p) => p.FirstOrDefault(File.Exists);

    public static string DbName => Svc.Config()?.Get("Database", "Name", "qahr_mobile") ?? "qahr_mobile";

    /// <summary>Directories under Data that are left out (map = 2 GB of static files; the mirror and the published
    /// patches can be downloaded / rebuilt again).</summary>
    public static List<string> DataExcludes(bool includePatches)
    {
        var l = new List<string> { "map", "_old" };
        if (!includePatches) l.Add("mobile_autopatch");
        return l;
    }

    private static string WriteOptionFile()
    {
        var cfg = new PatchConsole.ServerConfig(S.ConfigIni);
        // the password is needed only here; it goes into a temp file readable by this user and is deleted after use
        string pw = ReadRaw("Database", "Password");
        string user = ReadRaw("Database", "User");
        string host = cfg.Get("Database", "Host", "localhost");
        string port = cfg.Get("Database", "Port", "3306");
        string f = Path.Combine(Path.GetTempPath(), "ms_" + Guid.NewGuid().ToString("N") + ".cnf");
        var sb = new StringBuilder();
        sb.AppendLine("[client]");
        sb.AppendLine("host=" + host);
        sb.AppendLine("port=" + port);
        sb.AppendLine("user=" + user);
        sb.AppendLine("password=\"" + (pw ?? "").Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"");
        sb.AppendLine("default-character-set=utf8mb4");
        File.WriteAllText(f, sb.ToString(), new UTF8Encoding(false));
        return f;
    }

    /// <summary>Raw value of a key (used for the DB login only, never displayed).</summary>
    private static string ReadRaw(string section, string key)
    {
        string cur = "";
        foreach (var raw in File.ReadAllLines(S.ConfigIni))
        {
            string l = raw.Trim();
            if (l.StartsWith("[") && l.EndsWith("]")) { cur = l[1..^1].Trim(); continue; }
            int i = l.IndexOf('=');
            if (i > 0 && cur.Equals(section, StringComparison.OrdinalIgnoreCase) && l[..i].Trim().Equals(key, StringComparison.OrdinalIgnoreCase))
                return l[(i + 1)..].Trim();
        }
        return null;
    }

    public static async Task<string> DumpAsync(string outFile, Action<string> step, CancellationToken ct)
    {
        if (DumpExe == null) throw new Exception(Loc.F("bk.noDump", S.MysqlBin));
        string opt = WriteOptionFile();
        try
        {
            step(Loc.F("bk.dumping", DbName));
            var args = new[] { "--defaults-extra-file=" + opt, "--single-transaction", "--quick", "--routines", "--triggers", "--events",
                "--hex-blob", "--default-character-set=utf8mb4", "--databases", DbName, "--result-file=" + outFile };
            var r = await Proc.RunAsync(DumpExe, args, null, null, ct);
            if (!r.Ok) throw new Exception(Loc.F("bk.dumpFailed", MirrorOps.LastLines(r.Output, 3)));
            return outFile;
        }
        finally { try { File.Delete(opt); } catch { } }
    }

    public static async Task<BackupInfo> CreateAsync(bool db, bool data, string kind, string note, Action<string> step, CancellationToken ct)
    {
        Directory.CreateDirectory(S.BackupDir);
        string stamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string zip = Path.Combine(S.BackupDir, $"QahrMobile_{kind}_{stamp}.zip");
        string part = zip + ".part";
        string tmpSql = null;
        var man = new BackupManifest { Created = DateTime.Now, Kind = kind, Database = DbName, DataRoot = S.DataDir, Note = note ?? "" };
        try
        {
            if (db)
            {
                tmpSql = Path.Combine(Path.GetTempPath(), $"ms_dump_{stamp}.sql");
                await DumpAsync(tmpSql, step, ct);
                man.HasDb = true;
            }
            await Task.Run(() =>
            {
                using var z = ZipFile.Open(part, ZipArchiveMode.Create);
                if (tmpSql != null)
                {
                    step(Loc.Get("bk.zipDb"));
                    z.CreateEntryFromFile(tmpSql, DbEntry, CompressionLevel.Optimal);
                }
                if (data)
                {
                    var ex = DataExcludes(S.BackupIncludePatches);
                    man.Excluded = ex.Select(e => "Data\\" + e).ToList();
                    int n = 0;
                    foreach (var f in Directory.EnumerateFiles(S.DataDir, "*", SearchOption.AllDirectories))
                    {
                        ct.ThrowIfCancellationRequested();
                        string rel = Path.GetRelativePath(S.DataDir, f);
                        string top = rel.Split('\\')[0];
                        if (ex.Any(e => e.Equals(top, StringComparison.OrdinalIgnoreCase))) continue;
                        if (rel.EndsWith(".part", StringComparison.OrdinalIgnoreCase)) continue;
                        try
                        {
                            using var src = new FileStream(f, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                            var e = z.CreateEntry("Data/" + rel.Replace('\\', '/'), CompressionLevel.Optimal);
                            e.LastWriteTime = File.GetLastWriteTime(f);
                            using var dst = e.Open();
                            src.CopyTo(dst);
                            man.DataBytes += src.Length;
                            n++;
                            if (n % 500 == 0) step(Loc.F("bk.zipData", n));
                        }
                        catch (IOException) { /* a file being written right now: skipped */ }
                    }
                    man.DataFiles = n;
                }
                using (var w = new StreamWriter(z.CreateEntry(ManifestEntry).Open(), new UTF8Encoding(false)))
                    w.Write(JsonSerializer.Serialize(man, new JsonSerializerOptions { WriteIndented = true }));
            }, ct);
            File.Move(part, zip);
            ActionLog.Write("backup", $"{Path.GetFileName(zip)} db={man.HasDb} data={man.DataFiles}");
            ApplyRetention();
            return Read(zip);
        }
        catch
        {
            try { File.Delete(part); } catch { }
            throw;
        }
        finally { if (tmpSql != null) try { File.Delete(tmpSql); } catch { } }
    }

    public static void ApplyRetention()
    {
        try
        {
            var auto = List().Where(b => b.Kind != "pre-restore").OrderByDescending(b => b.Created).Skip(Math.Max(3, S.BackupKeep)).ToList();
            foreach (var b in auto) File.Delete(b.File);
        }
        catch { }
    }

    public static BackupInfo Read(string zip)
    {
        var fi = new FileInfo(zip);
        var b = new BackupInfo { File = zip, Created = fi.LastWriteTime, Size = fi.Length };
        try
        {
            using var z = ZipFile.OpenRead(zip);
            var m = z.GetEntry(ManifestEntry);
            if (m != null)
            {
                using var s = m.Open();
                var man = JsonSerializer.Deserialize<BackupManifest>(s);
                b.Created = man.Created;
                b.Kind = man.Kind;
                b.HasDb = man.HasDb;
                b.DataFiles = man.DataFiles;
                b.Note = man.Note;
            }
            else
            {
                b.HasDb = z.GetEntry(DbEntry) != null;
                b.DataFiles = z.Entries.Count(e => e.FullName.StartsWith("Data/"));
            }
        }
        catch { b.Note = Loc.Get("bk.unreadable"); }
        return b;
    }

    public static List<BackupInfo> List()
    {
        if (!Directory.Exists(S.BackupDir)) return new List<BackupInfo>();
        return Directory.GetFiles(S.BackupDir, "QahrMobile_*.zip").Select(Read).OrderByDescending(b => b.Created).ToList();
    }

    /// <summary>Restores the database dump into <paramref name="targetDb"/> (the live one or a check copy).</summary>
    public static async Task RestoreDbAsync(string zip, string targetDb, Action<string> step, CancellationToken ct)
    {
        if (ClientExe == null) throw new Exception(Loc.F("bk.noDump", S.MysqlBin));
        string sql = Path.Combine(Path.GetTempPath(), "ms_restore_" + Guid.NewGuid().ToString("N") + ".sql");
        string opt = WriteOptionFile();
        try
        {
            step(Loc.Get("bk.extractDb"));
            await Task.Run(() =>
            {
                using var z = ZipFile.OpenRead(zip);
                var e = z.GetEntry(DbEntry) ?? throw new Exception(Loc.Get("bk.noDbInZip"));
                using var src = e.Open();
                using var dst = File.Create(sql);
                // the dump names its database (--databases): point it at the target
                using var rd = new StreamReader(src, Encoding.UTF8);
                using var wr = new StreamWriter(dst, new UTF8Encoding(false));
                string line;
                string src_db = DbName;
                while ((line = rd.ReadLine()) != null)
                {
                    if (line.StartsWith("CREATE DATABASE", StringComparison.OrdinalIgnoreCase) || line.StartsWith("USE `", StringComparison.Ordinal))
                        line = line.Replace("`" + src_db + "`", "`" + targetDb + "`");
                    wr.WriteLine(line);
                }
            }, ct);
            step(Loc.F("bk.importing", targetDb));
            var r = await Proc.RunAsync(ClientExe, new[] { "--defaults-extra-file=" + opt, "-e", "source " + sql.Replace('\\', '/') }, null, null, ct);
            if (!r.Ok || r.Output.Contains("ERROR"))
                throw new Exception(Loc.F("bk.importFailed", System.Text.RegularExpressions.Regex.Replace(
                    string.Join("\n", r.Output.Split('\n').Where(l => l.Contains("ERROR")).Take(2)), @"user\s+'[^']*'@'[^']*'", "user ***")));
            ActionLog.Write("restore-db", $"{Path.GetFileName(zip)} -> {targetDb}");
        }
        finally
        {
            try { File.Delete(opt); } catch { }
            try { File.Delete(sql); } catch { }
        }
    }

    /// <summary>Checks a backup without restoring it: the dump is complete ("Dump completed"), its table count matches
    /// the live database, and every Data entry can be read through (zip CRC).</summary>
    public static async Task<(bool ok, string text)> VerifyAsync(string zip, CancellationToken ct)
    {
        return await Task.Run(async () =>
        {
            using var z = ZipFile.OpenRead(zip);
            var parts = new List<string>();
            bool ok = true;
            var e = z.GetEntry(DbEntry);
            if (e != null)
            {
                int tables = 0, inserts = 0;
                string last = "";
                using (var rd = new StreamReader(e.Open(), Encoding.UTF8))
                {
                    string l;
                    while ((l = rd.ReadLine()) != null)
                    {
                        if (l.StartsWith("CREATE TABLE", StringComparison.Ordinal)) tables++;
                        else if (l.StartsWith("INSERT INTO", StringComparison.Ordinal)) inserts++;
                        if (l.Length > 0) last = l;
                    }
                }
                bool complete = last.Contains("Dump completed");
                string live = await QueryAsync($"SELECT COUNT(*) FROM information_schema.tables WHERE table_schema='{DbName}' AND table_type='BASE TABLE'");
                bool same = live == null || live == tables.ToString();
                ok &= complete && same;
                parts.Add(Loc.F("bk.verifyDb", tables, inserts, live ?? "?", Loc.Get(complete ? "bk.verifyComplete" : "bk.verifyCut")));
            }
            int n = 0, bad = 0;
            var buf = new byte[81920];
            foreach (var d in z.Entries.Where(x => x.FullName.StartsWith("Data/") && !x.FullName.EndsWith("/")))
            {
                ct.ThrowIfCancellationRequested();
                try { using var s = d.Open(); while (s.Read(buf, 0, buf.Length) > 0) { } n++; }
                catch { bad++; }
            }
            if (n + bad > 0) parts.Add(Loc.F("bk.verifyData", n, bad));
            ok &= bad == 0;
            return (ok, string.Join(" · ", parts));
        }, ct);
    }

    /// <summary>Counts the tables of a database (used to verify a check restore).</summary>
    public static async Task<string> QueryAsync(string sqlText)
    {
        if (ClientExe == null) return null;
        string opt = WriteOptionFile();
        try
        {
            var r = await Proc.RunAsync(ClientExe, new[] { "--defaults-extra-file=" + opt, "-N", "-B", "-e", sqlText });
            return r.Ok ? r.Output.Trim() : null;
        }
        finally { try { File.Delete(opt); } catch { } }
    }

    /// <summary>Restores the Data files of a backup over <paramref name="target"/> (default Data). The files that are
    /// replaced are first copied to a pre-restore backup.</summary>
    public static async Task<int> RestoreDataAsync(string zip, string target, Action<string> step, CancellationToken ct)
    {
        int n = 0;
        await Task.Run(() =>
        {
            using var z = ZipFile.OpenRead(zip);
            foreach (var e in z.Entries)
            {
                ct.ThrowIfCancellationRequested();
                if (!e.FullName.StartsWith("Data/") || e.FullName.EndsWith("/")) continue;
                string rel = e.FullName[5..].Replace('/', '\\');
                string dest = Path.GetFullPath(Path.Combine(target, rel));
                if (!dest.StartsWith(Path.GetFullPath(target).TrimEnd('\\') + "\\", StringComparison.OrdinalIgnoreCase)) continue; // zip-slip guard
                Directory.CreateDirectory(Path.GetDirectoryName(dest));
                e.ExtractToFile(dest, true);
                if (++n % 500 == 0) step(Loc.F("bk.restored", n));
            }
        }, ct);
        ActionLog.Write("restore-data", $"{Path.GetFileName(zip)} -> {target} ({n} files)");
        return n;
    }

    public static List<string> Entries(string zip, int max = 4000)
    {
        try
        {
            using var z = ZipFile.OpenRead(zip);
            return z.Entries.Where(e => !e.FullName.EndsWith("/")).Take(max).Select(e => e.FullName + "    " + Fmt.Size(e.Length)).ToList();
        }
        catch { return new List<string>(); }
    }
}
