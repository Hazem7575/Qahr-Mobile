using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace MobileStudio.Core;

/// <summary>Runs a program asynchronously and streams its output lines (never blocks the UI).</summary>
public static class Proc
{
    public sealed class Result
    {
        public int Code;
        public string Output = "";
        public bool Cancelled;
        public bool Ok => Code == 0 && !Cancelled;
    }

    public static async Task<Result> RunAsync(string exe, IEnumerable<string> args, string workDir = null, Action<string> onLine = null,
        CancellationToken ct = default, IDictionary<string, string> env = null, string stdin = null, bool capture = true)
    {
        var psi = new ProcessStartInfo(exe)
        {
            WorkingDirectory = workDir ?? Environment.CurrentDirectory,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            RedirectStandardInput = stdin != null,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8,
        };
        foreach (var a in args) psi.ArgumentList.Add(a);
        psi.Environment["PYTHONIOENCODING"] = "utf-8";
        psi.Environment["PYTHONUTF8"] = "1";
        psi.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1";
        psi.Environment["DOTNET_NOLOGO"] = "1";
        psi.Environment["GIT_TERMINAL_PROMPT"] = "0";
        if (env != null) foreach (var kv in env) psi.Environment[kv.Key] = kv.Value;

        var sb = new StringBuilder();
        var r = new Result();
        using var p = new Process { StartInfo = psi };
        void Line(string s)
        {
            if (s == null) return;
            if (capture) lock (sb) { if (sb.Length < 4_000_000) sb.AppendLine(s); }
            try { onLine?.Invoke(s); } catch { }
        }
        p.OutputDataReceived += (_, e) => Line(e.Data);
        p.ErrorDataReceived += (_, e) => Line(e.Data);
        try { p.Start(); }
        catch (Exception ex)
        {
            r.Code = -1;
            r.Output = ex.Message;
            onLine?.Invoke(ex.Message);
            return r;
        }
        p.BeginOutputReadLine();
        p.BeginErrorReadLine();
        if (stdin != null)
        {
            try { await p.StandardInput.WriteAsync(stdin); p.StandardInput.Close(); } catch { }
        }
        try { await p.WaitForExitAsync(ct); }
        catch (OperationCanceledException)
        {
            try { p.Kill(true); } catch { }
            r.Cancelled = true;
        }
        try { p.WaitForExit(); } catch { }
        r.Code = r.Cancelled ? -1 : p.ExitCode;
        lock (sb) r.Output = sb.ToString();
        return r;
    }

    /// <summary>Starts a detached program through PowerShell's Start-Process (same as run_live.ps1): it keeps running
    /// when this tool closes, and its console output goes straight to the given files.</summary>
    public static async Task<Result> StartDetachedAsync(string exe, IEnumerable<string> args, string workDir, string stdoutFile, string stderrFile,
        IDictionary<string, string> env = null)
    {
        static string Q(string s) => "'" + (s ?? "").Replace("'", "''") + "'";
        var sb = new StringBuilder("$ErrorActionPreference='Stop'; ");
        if (env != null) foreach (var kv in env) sb.Append("$env:").Append(kv.Key).Append('=').Append(Q(kv.Value)).Append("; ");
        sb.Append("Start-Process -FilePath ").Append(Q(exe)).Append(" -WorkingDirectory ").Append(Q(workDir)).Append(" -WindowStyle Minimized");
        var list = args.ToList();
        if (list.Count > 0) sb.Append(" -ArgumentList ").Append(string.Join(",", list.Select(Q)));
        if (stdoutFile != null) sb.Append(" -RedirectStandardOutput ").Append(Q(stdoutFile));
        if (stderrFile != null) sb.Append(" -RedirectStandardError ").Append(Q(stderrFile));
        return await PowerShellNoPipeAsync(sb.ToString(), workDir);
    }

    /// <summary>Runs a PowerShell command whose children keep running (Start-Process). No pipes are given to it: a
    /// detached child would inherit them and the read would never end. The output goes through a temp file.</summary>
    public static async Task<Result> PowerShellNoPipeAsync(string command, string workDir)
    {
        string tmp = Path.Combine(Path.GetTempPath(), "ms_ps_" + Guid.NewGuid().ToString("N") + ".txt");
        var psi = new ProcessStartInfo("powershell.exe") { UseShellExecute = false, CreateNoWindow = true, WorkingDirectory = workDir ?? Environment.CurrentDirectory };
        foreach (var a in new[] { "-NoProfile", "-NonInteractive", "-ExecutionPolicy", "Bypass", "-Command",
                     "& { " + command + " } *> '" + tmp.Replace("'", "''") + "'; exit $LASTEXITCODE" })
            psi.ArgumentList.Add(a);
        var r = new Result();
        try
        {
            using var p = Process.Start(psi);
            await Task.Run(() => p.WaitForExit(120000));
            r.Code = p.HasExited ? p.ExitCode : -1;
        }
        catch (Exception ex) { r.Code = -1; r.Output = ex.Message; return r; }
        try { r.Output = File.Exists(tmp) ? File.ReadAllText(tmp) : ""; File.Delete(tmp); } catch { }
        return r;
    }
}

/// <summary>Process tree helpers (Toolhelp snapshot: parent ids without WMI).</summary>
public static class ProcTree
{
    public sealed class Entry
    {
        public int Pid, Parent;
        public string Name;
        public override string ToString() => $"{Name} ({Pid})";
    }

    public static List<Entry> Snapshot()
    {
        var list = new List<Entry>();
        IntPtr snap = CreateToolhelp32Snapshot(2, 0);
        if (snap == IntPtr.Zero || snap == new IntPtr(-1)) return list;
        try
        {
            var pe = new PROCESSENTRY32W { dwSize = (uint)Marshal.SizeOf<PROCESSENTRY32W>() };
            if (!Process32FirstW(snap, ref pe)) return list;
            do list.Add(new Entry { Pid = (int)pe.th32ProcessID, Parent = (int)pe.th32ParentProcessID, Name = pe.szExeFile });
            while (Process32NextW(snap, ref pe));
        }
        finally { CloseHandle(snap); }
        return list;
    }

    public static string ImagePath(int pid)
    {
        IntPtr h = OpenProcess(0x1000, false, pid); // PROCESS_QUERY_LIMITED_INFORMATION
        if (h == IntPtr.Zero) return null;
        try
        {
            var sb = new StringBuilder(1024);
            int n = sb.Capacity;
            return QueryFullProcessImageNameW(h, 0, sb, ref n) ? sb.ToString(0, n) : null;
        }
        finally { CloseHandle(h); }
    }

    /// <summary>Processes with this exe name whose image lies under <paramref name="root"/>.</summary>
    public static List<(Entry e, string path)> FindUnder(List<Entry> snap, string exeName, string root)
    {
        var r = new List<(Entry, string)>();
        foreach (var e in snap.Where(x => string.Equals(x.Name, exeName, StringComparison.OrdinalIgnoreCase)))
        {
            string p = ImagePath(e.Pid);
            if (p != null && p.StartsWith(root.TrimEnd('\\') + "\\", StringComparison.OrdinalIgnoreCase)) r.Add((e, p));
        }
        return r;
    }

    /// <summary>The dotnet.exe ancestors (dotnet run / dotnet-watch / dotnet watch) of a process.</summary>
    public static List<Entry> DotnetAncestors(List<Entry> snap, int pid)
    {
        var r = new List<Entry>();
        var byPid = snap.GroupBy(e => e.Pid).ToDictionary(g => g.Key, g => g.First());
        var cur = byPid.GetValueOrDefault(pid);
        for (int guard = 0; cur != null && guard < 8; guard++)
        {
            var parent = byPid.GetValueOrDefault(cur.Parent);
            if (parent == null || !string.Equals(parent.Name, "dotnet.exe", StringComparison.OrdinalIgnoreCase)) break;
            r.Add(parent);
            cur = parent;
        }
        return r;
    }

    public static DateTime? StartTime(int pid)
    {
        try { using var p = Process.GetProcessById(pid); return p.StartTime; } catch { return null; }
    }

    public static long WorkingSet(int pid)
    {
        try { using var p = Process.GetProcessById(pid); return p.WorkingSet64; } catch { return 0; }
    }

    public static bool Kill(int pid)
    {
        try { using var p = Process.GetProcessById(pid); p.Kill(false); p.WaitForExit(5000); return true; } catch { return false; }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct PROCESSENTRY32W
    {
        public uint dwSize, cntUsage, th32ProcessID;
        public IntPtr th32DefaultHeapID;
        public uint th32ModuleID, cntThreads, th32ParentProcessID;
        public int pcPriClassBase;
        public uint dwFlags;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)] public string szExeFile;
    }

    [DllImport("kernel32.dll", SetLastError = true)] private static extern IntPtr CreateToolhelp32Snapshot(uint flags, uint pid);
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)] private static extern bool Process32FirstW(IntPtr snap, ref PROCESSENTRY32W pe);
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)] private static extern bool Process32NextW(IntPtr snap, ref PROCESSENTRY32W pe);
    [DllImport("kernel32.dll")] private static extern bool CloseHandle(IntPtr h);
    [DllImport("kernel32.dll")] private static extern IntPtr OpenProcess(uint access, bool inherit, int pid);
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)] private static extern bool QueryFullProcessImageNameW(IntPtr h, int flags, StringBuilder sb, ref int size);
}
