using System.IO;
using System.Text.Json;

namespace MobileStudio.Core;

/// <summary>Tool settings (%AppData%\ScriptHazem\MobileStudio\settings.json). Holds paths and form values only - never a
/// password or secret: the database password and the AdminApi secret are read from the server's own files when needed.</summary>
public sealed class AppSettings
{
    public string Language { get; set; } = "ar";

    // ---- folders and programs (auto-detected, editable on the Settings page)
    // Since 2026-10-21 every mobile folder lives under D:\Qahr Mobile (the old D:\conquer paths are junctions to it).
    public const string Home = @"D:\Qahr Mobile";
    public string QahrRoot { get; set; } = Home + @"\Server";
    public string ComobileRoot { get; set; } = Home + @"\MobileTools";
    public string PythonExe { get; set; } = Home + @"\MobileTools\venv\Scripts\python.exe";
    public string ToolsRoot { get; set; } = Home + @"\Tools";
    public string ClientSourceDir { get; set; } = Home + @"\ClientSource";
    public string EngineSourceDir { get; set; } = Home + @"\EngineSource";
    public string AnalysisDir { get; set; } = Home + @"\AnalysisTools";
    public string GhidraRun { get; set; } = Home + @"\AnalysisTools\tools\ghidra_12.1.4_PUBLIC\ghidraRun.bat";
    public string GhidraProject { get; set; } = Home + @"\AnalysisTools\ghidra_projects\QahrClient.gpr";
    public string DotnetExe { get; set; } = @"D:\dotnet10\dotnet.exe";
    public string AdbExe { get; set; } = @"D:\Android\Sdk\platform-tools\adb.exe";
    public string MysqlBin { get; set; } = @"C:\Program Files\MariaDB 10.6\bin";
    public string GitExe { get; set; } = "git";
    public string RepoDir { get; set; } = Home + @"\GitHub";
    public string BackupDir { get; set; } = Home + @"\Backups\QahrMobile";
    public string MirrorScript { get; set; } = "";   // empty = auto (tools\official_mirror.py or Tools\OfficialMirror\official_mirror.py)

    // ---- git author (every commit from the tool)
    public string GitAuthorName { get; set; } = "Hazem";
    public string GitAuthorEmail { get; set; } = "Hazem7575@users.noreply.github.com";

    // ---- APK form (remembered)
    public string ApkIp { get; set; } = "";
    public int ApkPort { get; set; } = 16100;
    public string ApkPatchHost { get; set; } = "";
    public int ApkRegisterPort { get; set; } = 9548;
    public bool ApkSingle { get; set; } = true;
    public string ApkOut { get; set; } = "";
    public string ApkCdnUrl { get; set; } = "";
    public int CdnPort { get; set; } = 9550;

    // ---- backup
    public bool BackupIncludePatches { get; set; } = false;
    public int BackupKeep { get; set; } = 20;

    // ---- git sync excludes (robocopy /XD and /XF)
    public List<string> SyncExcludeDirs { get; set; } = new()
    {
        "obj", "bin", "bin-net10", "obj-net10", "PacketLogs", ".vs", "__pycache__", "_old", "publish",
        "map_pc_backup", "mobile_autopatch", "official_mirror",
    };
    public List<string> SyncExcludeFiles { get; set; } = new()
    {
        "*.log", "ServerConfig.ini", "ServerConfig.ini.bak", "adminapi.txt", "packetinject.txt", "adminapi_log.txt",
        "*.user", "*.suo",
    };

    // ---------------------------------------------------------------- derived paths
    public string GameDir => Path.Combine(QahrRoot, "src", "Qahr.Mobile.Game");
    public string GameCsproj => Path.Combine(GameDir, "Qahr.Mobile.Game.csproj");
    public string GameExe => Path.Combine(GameDir, "bin", "Qahr.Mobile.Game.exe");
    public string AccDir => Path.Combine(QahrRoot, "src", "Qahr.Mobile.Login");
    public string AccCsproj => Path.Combine(AccDir, "Qahr.Mobile.Login.csproj");
    public string AccExe => Path.Combine(AccDir, "bin", "Debug", "Qahr.Mobile.Login.exe");
    public string ConfigIni => Path.Combine(QahrRoot, "src", "Qahr.Mobile.Shared", "ServerConfig.ini");
    public string DataDir => Path.Combine(QahrRoot, "Data");
    public string WatchLog => Path.Combine(Home, "Logs", "watch.log");
    public string WatchErrLog => Path.Combine(Home, "Logs", "watch.err.log");
    public string AccLog => Path.Combine(Home, "Logs", "acc.log");
    public string AccErrLog => Path.Combine(Home, "Logs", "acc.err.log");
    public string PacketLogDir => Path.Combine(GameDir, "bin", "PacketLogs");
    public string RunLive => Path.Combine(QahrRoot, "scripts", "run_live.ps1");
    public string RunCdn => Path.Combine(QahrRoot, "scripts", "run_cdn.ps1");
    public string CdnAccessLog => Path.Combine(MirrorRoot, "cdn_access.log");
    public string AdminApiTxt => Path.Combine(DataDir, "adminapi.txt");
    public string BuildApkPy => Path.Combine(ComobileRoot, "tools", "build_apk.py");
    public string PatchConsoleExe => Path.Combine(QahrRoot, "Tools", "PatchConsole", "publish", "PatchConsole.exe");

    public string MirrorPy
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(MirrorScript) && File.Exists(MirrorScript)) return MirrorScript;
            foreach (var p in new[]
                     {
                         Path.Combine(ComobileRoot, "tools", "official_mirror.py"),
                         Path.Combine(QahrRoot, "Tools", "OfficialMirror", "official_mirror.py"),
                     })
                if (File.Exists(p)) return p;
            return Path.Combine(ComobileRoot, "tools", "official_mirror.py");
        }
    }

    public string MirrorRoot => Path.Combine(Home, "CDN");

    // ---------------------------------------------------------------- load / save
    public static string AppDataDir => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ScriptHazem", "MobileStudio");
    public static string FilePath => Path.Combine(AppDataDir, "settings.json");
    private static readonly JsonSerializerOptions Json = new() { WriteIndented = true };

    public static AppSettings Load()
    {
        try
        {
            if (File.Exists(FilePath))
                return JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(FilePath), Json) ?? new AppSettings();
        }
        catch { }
        return new AppSettings();
    }

    public void Save()
    {
        Directory.CreateDirectory(AppDataDir);
        string tmp = FilePath + ".tmp";
        File.WriteAllText(tmp, JsonSerializer.Serialize(this, Json));
        File.Move(tmp, FilePath, true);
    }

    /// <summary>Finds the programs when the saved paths do not exist on this machine.</summary>
    public void AutoDetect()
    {
        // move saved old paths to the new home (the old ones are junctions, but the new ones are the real folders)
        string Mig(string p)
        {
            if (string.IsNullOrEmpty(p) || !Directory.Exists(Home)) return p;
            foreach (var (o, n) in new[] { (@"D:\conquer\QahrMobile", Home + @"\Server"), (@"D:\conquer\_private\comobile", Home + @"\MobileTools"),
                                           (@"D:\conquer\conquer-mobile", Home + @"\GitHub"), (@"D:\conquer\Backups\QahrMobile", Home + @"\Backups\QahrMobile") })
                if (p.StartsWith(o, StringComparison.OrdinalIgnoreCase)) return n + p[o.Length..];
            return p;
        }
        QahrRoot = Mig(QahrRoot); ComobileRoot = Mig(ComobileRoot); PythonExe = Mig(PythonExe); RepoDir = Mig(RepoDir); BackupDir = Mig(BackupDir);
        MirrorScript = Mig(MirrorScript); ApkOut = Mig(ApkOut);
        if (!File.Exists(DotnetExe)) DotnetExe = "dotnet";
        if (!File.Exists(AdbExe))
        {
            string sdk = Environment.GetEnvironmentVariable("ANDROID_HOME") ?? Environment.GetEnvironmentVariable("ANDROID_SDK_ROOT") ?? "";
            string cand = Path.Combine(sdk, "platform-tools", "adb.exe");
            AdbExe = File.Exists(cand) ? cand : "adb";
        }
        if (!Directory.Exists(MysqlBin))
        {
            foreach (var root in new[] { @"C:\Program Files", @"C:\Program Files (x86)" })
            {
                if (!Directory.Exists(root)) continue;
                var hit = Directory.GetDirectories(root, "MariaDB*").Concat(Directory.GetDirectories(root, "MySQL"))
                    .SelectMany(d => Directory.GetDirectories(d, "*", SearchOption.TopDirectoryOnly).Append(d))
                    .Select(d => d.EndsWith("bin") ? d : Path.Combine(d, "bin"))
                    .FirstOrDefault(d => File.Exists(Path.Combine(d, "mysqldump.exe")) || File.Exists(Path.Combine(d, "mariadb-dump.exe")));
                if (hit != null) { MysqlBin = hit; break; }
            }
        }
        if (string.IsNullOrWhiteSpace(ApkIp))
        {
            try { ApkIp = new PatchConsole.ServerConfig(ConfigIni).ServerIp; } catch { }
        }
    }
}
