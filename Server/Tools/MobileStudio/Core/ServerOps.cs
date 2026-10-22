using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using ScriptHazem.Kit;

namespace MobileStudio.Core;

public sealed class BuildIssue
{
    public string File { get; set; }
    public int Line { get; set; }
    public int Col { get; set; }
    public string Code { get; set; }
    public string Message { get; set; }
    public bool IsWarning { get; set; }
    public string Project { get; set; } = "";
    public string Where => $"{System.IO.Path.GetFileName(File)}:{Line}";
    public string SeverityText => Loc.Get(IsWarning ? "build.sev.warning" : "build.sev.error");

    private static readonly Regex Rx = new(
        @"^\s*(?<file>[A-Za-z]:\\[^(\r\n]+?)\((?<line>\d+),(?<col>\d+)(?:,\d+,\d+)?\)\s*:\s*(?<sev>error|warning)\s+(?<code>[A-Z]+\d+)\s*:\s*(?<msg>.*?)(?:\s+\[[A-Za-z]:\\[^\r\n]*?\.csproj(?:::[^\]]*)?\])?\s*$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static BuildIssue Parse(string line)
    {
        var m = Rx.Match(line ?? "");
        if (!m.Success) return null;
        return new BuildIssue
        {
            File = m.Groups["file"].Value.Trim(),
            Line = int.Parse(m.Groups["line"].Value),
            Col = int.Parse(m.Groups["col"].Value),
            Code = m.Groups["code"].Value,
            Message = m.Groups["msg"].Value.Trim(),
            IsWarning = m.Groups["sev"].Value.Equals("warning", StringComparison.OrdinalIgnoreCase),
        };
    }

    /// <summary>Opens the file at the line: VS Code if installed, else the default editor.</summary>
    public static void Open(string file, int line, int col)
    {
        try
        {
            string code = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Programs", "Microsoft VS Code", "Code.exe");
            if (System.IO.File.Exists(code))
            {
                var psi = new ProcessStartInfo(code) { UseShellExecute = false };
                psi.ArgumentList.Add("-g");
                psi.ArgumentList.Add($"{file}:{line}:{Math.Max(1, col)}");
                Process.Start(psi);
                return;
            }
            Process.Start(new ProcessStartInfo(file) { UseShellExecute = true });
        }
        catch { }
    }
}

/// <summary>Start / stop / build of the two mobile servers. Same steps as run_live.ps1 (save every player first, Hot
/// Reload build, dotnet watch), but only processes whose exe lies under QahrRoot are ever touched (the PC servers in
/// D:\conquer-3d are never stopped by mistake).</summary>
public static class ServerOps
{
    static AppSettings S => Svc.Settings;

    public static Dictionary<string, string> DotnetEnv()
    {
        var env = new Dictionary<string, string>();
        string root = Path.GetDirectoryName(S.DotnetExe);
        if (!string.IsNullOrEmpty(root) && Directory.Exists(root)) env["DOTNET_ROOT"] = root;
        return env;
    }

    // ------------------------------------------------------------------------------------------------ GameServer
    /// <summary>What a stop will kill (for the confirmation text).</summary>
    public static (int game, List<int> watch) GameTargets()
    {
        var snap = ProcTree.Snapshot();
        var g = ProcTree.FindUnder(snap, "Qahr.Mobile.Game.exe", S.QahrRoot).FirstOrDefault();
        if (g.e == null) return (0, new List<int>());
        return (g.e.Pid, ProcTree.DotnetAncestors(snap, g.e.Pid).Select(a => a.Pid).ToList());
    }

    /// <summary>Saves every online player through the AdminApi. Returns (ok, message).</summary>
    public static async Task<(bool ok, string msg)> SaveAllAsync()
    {
        var r = await AdminClient.CallAsync("save", ("all", (object)1));
        return (r.Ok, r.Ok ? r.Message : r.Error);
    }

    public static async Task<bool> StopGameAsync(Action<string> step, bool saveFirst = true)
    {
        if (saveFirst)
        {
            step(Loc.Get("ops.saving"));
            var (ok, msg) = await SaveAllAsync();
            step(ok ? Loc.F("ops.saved", msg) : Loc.F("ops.saveFailed", msg));
            if (ok) await Task.Delay(1500); // let the DB writer flush
        }
        var (game, watch) = GameTargets();
        step(Loc.F("ops.stopping", "GameServer"));
        // the watch chain first (otherwise dotnet watch restarts the server), then the server itself
        foreach (int pid in watch.AsEnumerable().Reverse()) ProcTree.Kill(pid);
        if (game > 0) ProcTree.Kill(game);
        ActionLog.Write("stop-game", $"pid={game} watch={string.Join(",", watch)} save={saveFirst}");
        int port = Svc.Config()?.GamePort ?? 5828;
        for (int i = 0; i < 30; i++)
        {
            if (!ServerMonitor.Listening().Contains(port)) { step(Loc.Get("ops.stopped")); return true; }
            await Task.Delay(500);
        }
        step(Loc.Get("ops.stillListening"));
        return false;
    }

    /// <summary>Compiles GameServer into a staging folder (%TEMP%) — never touches the live bin or the running server.</summary>
    public static Task<Proc.Result> CheckBuildAsync(string csproj, string tag, Action<string> line, CancellationToken ct)
    {
        string stage = Path.Combine(Path.GetTempPath(), "ScriptHazem.MobileStudio", tag);
        string outDir = Path.Combine(stage, "out") + "\\";
        string objDir = Path.Combine(stage, "obj") + "\\";
        var args = new List<string> { "build", csproj, "-c", "Debug", "-v", "minimal", "-nologo", "-clp:NoSummary", "-nodeReuse:false",
            "--property:Optimize=false", $"-p:OutDir={outDir}", $"-p:IntermediateOutputPath={objDir}" };
        line(Loc.F("build.cmd", "dotnet " + string.Join(" ", args.Select(a => a.Contains(' ') ? "\"" + a + "\"" : a))));
        return Proc.RunAsync(S.DotnetExe, args, Path.GetDirectoryName(csproj), line, ct, DotnetEnv());
    }

    /// <summary>The real build into the live output (only while that server is stopped).</summary>
    public static Task<Proc.Result> LiveBuildAsync(string csproj, bool hotReload, Action<string> line, CancellationToken ct)
    {
        var args = new List<string> { "build", csproj, "-c", "Debug", "-v", "minimal", "-nologo", "-clp:NoSummary", "-nodeReuse:false" };
        if (hotReload) args.Add("--property:Optimize=false");
        line(Loc.F("build.cmd", "dotnet " + string.Join(" ", args)));
        return Proc.RunAsync(S.DotnetExe, args, Path.GetDirectoryName(csproj), line, ct, DotnetEnv());
    }

    /// <summary>Keeps the previous console logs (they are overwritten on start).</summary>
    private static void ArchiveLogs(params string[] files)
    {
        try
        {
            string dir = Path.Combine(AppSettings.AppDataDir, "oldlogs");
            Directory.CreateDirectory(dir);
            string stamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            foreach (var f in files)
                if (File.Exists(f) && new FileInfo(f).Length > 0)
                    File.Copy(f, Path.Combine(dir, Path.GetFileNameWithoutExtension(f) + "_" + stamp + Path.GetExtension(f)), true);
            foreach (var old in Directory.GetFiles(dir).OrderByDescending(File.GetLastWriteTime).Skip(40)) File.Delete(old);
        }
        catch { }
    }

    public static async Task<bool> StartGameAsync(Action<string> step)
    {
        if (GameTargets().game > 0) { step(Loc.Get("ops.alreadyRunning")); return true; }
        ArchiveLogs(S.WatchLog, S.WatchErrLog);
        var env = DotnetEnv();
        env["DOTNET_WATCH_RESTART_ON_RUDE_EDIT"] = "false";
        step(Loc.Get("ops.startingWatch"));
        var r = await Proc.StartDetachedAsync(S.DotnetExe,
            new[] { "watch", "--non-interactive", "run", "--project", "Qahr.Mobile.Game.csproj", "-c", "Debug", "--property:Optimize=false" },
            S.GameDir, S.WatchLog, S.WatchErrLog, env);
        if (!r.Ok) { step(r.Output.Trim()); return false; }
        ActionLog.Write("start-game", "dotnet watch run (Hot Reload)");
        int port = Svc.Config()?.GamePort ?? 5828;
        var sw = Stopwatch.StartNew();
        while (sw.Elapsed.TotalSeconds < 240)
        {
            await Task.Delay(1000);
            if (ServerMonitor.Listening().Contains(port)) { step(Loc.F("ops.listening", "GameServer", port, (int)sw.Elapsed.TotalSeconds)); return true; }
            if ((int)sw.Elapsed.TotalSeconds % 10 == 0) step(Loc.F("ops.waiting", (int)sw.Elapsed.TotalSeconds));
        }
        step(Loc.Get("ops.notListening"));
        return false;
    }

    /// <summary>run_live.ps1 with a compile check first: check build (server untouched) → save → stop → Hot Reload build → start.</summary>
    public static async Task<bool> SafeRestartGameAsync(Action<string> step, Action<string> line, CancellationToken ct)
    {
        step(Loc.Get("ops.checkBuild"));
        var chk = await CheckBuildAsync(S.GameCsproj, "game-check", line, ct);
        if (!chk.Ok) { step(Loc.Get("ops.checkFailed")); return false; }
        if (!await StopGameAsync(step)) return false;
        step(Loc.Get("ops.liveBuild"));
        var b = await LiveBuildAsync(S.GameCsproj, true, line, ct);
        if (!b.Ok) step(Loc.Get("ops.liveBuildFailed"));  // dotnet watch builds again by itself and reports errors in watch.err.log
        return await StartGameAsync(step);
    }

    // ------------------------------------------------------------------------------------------------ CDN mirror
    /// <summary>run_cdn.ps1 (patchmirror): official_mirror.py cdn-serve on the CDN port. Start / -Stop / -Status.</summary>
    public static async Task<bool> CdnAsync(bool stop, Action<string> step)
    {
        if (!File.Exists(S.RunCdn)) { step(Loc.F("ops.noExe", S.RunCdn)); return false; }
        step(stop ? Loc.F("ops.stopping", "CDN") : Loc.F("ops.starting", "CDN"));
        string cmd = "& '" + S.RunCdn.Replace("'", "''") + "' -Port " + S.CdnPort + (stop ? " -Stop" : "");
        var r = await Proc.PowerShellNoPipeAsync(cmd, S.QahrRoot);
        step(r.Output.Trim());
        ActionLog.Write(stop ? "stop-cdn" : "start-cdn", r.Output.Trim());
        await Task.Delay(400);
        bool up = ServerMonitor.Listening().Contains(S.CdnPort);
        return stop ? !up : up;
    }

    // ------------------------------------------------------------------------------------------------ AccServer
    public static int AccPid() => ProcTree.FindUnder(ProcTree.Snapshot(), "Qahr.Mobile.Login.exe", S.QahrRoot).FirstOrDefault().e?.Pid ?? 0;

    public static async Task<bool> StopAccAsync(Action<string> step)
    {
        int pid = AccPid();
        step(Loc.F("ops.stopping", "AccServer"));
        if (pid > 0) ProcTree.Kill(pid);
        ActionLog.Write("stop-acc", $"pid={pid}");
        int port = Svc.Config()?.LoginPort ?? 16100;
        for (int i = 0; i < 20; i++)
        {
            if (!ServerMonitor.Listening().Contains(port) && AccPid() == 0) { step(Loc.Get("ops.stopped")); return true; }
            await Task.Delay(500);
        }
        step(Loc.Get("ops.stillListening"));
        return false;
    }

    public static async Task<bool> StartAccAsync(Action<string> step)
    {
        if (AccPid() > 0) { step(Loc.Get("ops.alreadyRunning")); return true; }
        if (!File.Exists(S.AccExe)) { step(Loc.F("ops.noExe", S.AccExe)); return false; }
        ArchiveLogs(S.AccLog, S.AccErrLog);
        step(Loc.F("ops.starting", "AccServer"));
        var r = await Proc.StartDetachedAsync(S.AccExe, Array.Empty<string>(), Path.GetDirectoryName(S.AccExe), S.AccLog, S.AccErrLog);
        if (!r.Ok) { step(r.Output.Trim()); return false; }
        ActionLog.Write("start-acc", S.AccExe);
        int port = Svc.Config()?.LoginPort ?? 16100;
        var sw = Stopwatch.StartNew();
        while (sw.Elapsed.TotalSeconds < 60)
        {
            await Task.Delay(700);
            if (ServerMonitor.Listening().Contains(port)) { step(Loc.F("ops.listening", "AccServer", port, (int)sw.Elapsed.TotalSeconds)); return true; }
        }
        step(Loc.Get("ops.notListening"));
        return false;
    }

    /// <summary>Stop → build (bin\Debug) → start. A failed build starts the previous exe again.</summary>
    public static async Task<bool> RebuildAccAsync(Action<string> step, Action<string> line, CancellationToken ct)
    {
        step(Loc.Get("ops.checkBuild"));
        var chk = await CheckBuildAsync(S.AccCsproj, "acc-check", line, ct);
        if (!chk.Ok) { step(Loc.Get("ops.checkFailed")); return false; }
        if (!await StopAccAsync(step)) return false;
        step(Loc.Get("ops.liveBuild"));
        var b = await LiveBuildAsync(S.AccCsproj, false, line, ct);
        if (!b.Ok) step(Loc.Get("ops.liveBuildFailedOld"));
        return await StartAccAsync(step);
    }
}
