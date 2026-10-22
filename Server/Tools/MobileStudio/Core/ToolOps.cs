using System.IO;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace MobileStudio.Core;

/// <summary>official_mirror.py (patchmirror's tool): every command takes --json and prints one JSON object on its last line.</summary>
public static class MirrorOps
{
    public sealed class Run
    {
        public Proc.Result Proc;
        public JsonObject Json;
        public bool Ok => Json != null ? AdminClient.Bool(Json, "ok") : Proc?.Ok == true;
        public JsonNode ResultNode => Json?["result"];
        public string Error => Json?["error"]?.ToString() ?? (Proc?.Ok == false ? LastLines(Proc.Output, 3) : null);
    }

    public static bool Available => File.Exists(Svc.Settings.MirrorPy) && File.Exists(Svc.Settings.PythonExe);

    public static async Task<Run> RunAsync(IEnumerable<string> args, Action<string> line, CancellationToken ct = default)
    {
        var s = Svc.Settings;
        var a = new List<string> { "-I", s.MirrorPy };
        a.AddRange(args);
        a.Add("--json");
        var r = new Run();
        r.Proc = await Core.Proc.RunAsync(s.PythonExe, a, Path.GetDirectoryName(s.MirrorPy), l =>
        {
            if (!l.StartsWith("{\"ok\"", StringComparison.Ordinal)) line?.Invoke(l);
        }, ct);
        foreach (var l in r.Proc.Output.Split('\n').Reverse())
        {
            string t = l.Trim();
            if (!t.StartsWith("{")) continue;
            try { r.Json = JsonNode.Parse(t) as JsonObject; } catch { }
            break;
        }
        return r;
    }

    public static string LastLines(string text, int n) =>
        string.Join("\n", (text ?? "").Split('\n').Select(x => x.TrimEnd('\r')).Where(x => x.Length > 0).TakeLast(n));
}

/// <summary>tools\build_apk.py and adb.</summary>
public static class ApkOps
{
    public sealed class Device
    {
        public string Serial { get; set; }
        public string State { get; set; }
        public string Model { get; set; }
        public string Label => string.IsNullOrEmpty(Model) ? Serial : $"{Model}  ·  {Serial}";
        public bool Ready => State == "device";
    }

    public static List<string> BuildArgs(string ip, int port, string patchHost, bool single, string outDir, int registerPort, string cdnUrl = null)
    {
        var a = new List<string> { "-I", Svc.Settings.BuildApkPy, "--ip", ip, "--port", port.ToString() };
        if (!string.IsNullOrWhiteSpace(patchHost)) { a.Add("--patch-host"); a.Add(patchHost.Trim()); }
        if (single) a.Add("--single");
        if (!string.IsNullOrWhiteSpace(outDir)) { a.Add("--out"); a.Add(outDir.Trim()); }
        a.Add("--register-port");
        a.Add(registerPort.ToString());
        if (!string.IsNullOrWhiteSpace(cdnUrl)) { a.Add("--cdn-url"); a.Add(cdnUrl.Trim()); }
        return a;
    }

    public static async Task<List<Device>> DevicesAsync()
    {
        var r = await Proc.RunAsync(Svc.Settings.AdbExe, new[] { "devices", "-l" });
        var list = new List<Device>();
        foreach (var raw in r.Output.Split('\n'))
        {
            string l = raw.Trim();
            if (l.Length == 0 || l.StartsWith("List of") || l.StartsWith("*")) continue;
            var parts = Regex.Split(l, @"\s+");
            if (parts.Length < 2) continue;
            var d = new Device { Serial = parts[0], State = parts[1] };
            var m = Regex.Match(l, @"model:(\S+)");
            if (m.Success) d.Model = m.Groups[1].Value.Replace('_', ' ');
            list.Add(d);
        }
        return list;
    }

    /// <summary>The APK files a build produced in <paramref name="outDir"/> (single first).</summary>
    public static (string single, List<string> splits) Outputs(string outDir)
    {
        if (!Directory.Exists(outDir)) return (null, new List<string>());
        string single = Path.Combine(outDir, "Qahr_single.apk");
        var splits = new[] { "com.Tq.CQ2ClientAndroid.apk", "config.armeabi_v7a.apk", "config.mdpi.apk", "install_time_1001.apk" }
            .Select(n => Path.Combine(outDir, n)).Where(File.Exists).ToList();
        return (File.Exists(single) ? single : null, splits);
    }

    public static Task<Proc.Result> InstallAsync(string serial, string single, List<string> splits, Action<string> line, CancellationToken ct)
    {
        var a = new List<string> { "-s", serial };
        if (single != null) { a.Add("install"); a.Add("-r"); a.Add(single); }
        else { a.Add("install-multiple"); a.Add("-r"); a.AddRange(splits); }
        line("adb " + string.Join(" ", a));
        return Proc.RunAsync(Svc.Settings.AdbExe, a, null, line, ct);
    }
}
