using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace MobileStudio.Core;

public enum LogLevel { Info = 0, Warning = 1, Error = 2 }

public sealed class LogLine
{
    public string Text { get; init; }
    public LogLevel Level { get; init; }
    public System.Windows.Media.Brush Brush => Level switch
    {
        LogLevel.Error => Brushes2.Err,
        LogLevel.Warning => Brushes2.Warn,
        _ => Text.Contains("changes applied", StringComparison.Ordinal) || Text.Contains(" OK ", StringComparison.Ordinal) ? Brushes2.Ok : Brushes2.Text,
    };
    public override string ToString() => Text;
}

/// <summary>Level of a server / dotnet watch / AccServer line (the servers have no log levels of their own).
/// Same idea as ServerControl's classifier, plus the mobile layer and Hot Reload messages.</summary>
public static class LogClassifier
{
    private static readonly Regex ErrorRx = new(@"exception|\[error|\berror\b|unhandled|\bfailed\b|\bcrash|cannot access a disposed|error CS\d+|won't be applied|rude edit",
        RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex WarnRx = new(@"\[warning|\bwarn|⚠|mismatch|not found|expired|timed out|\bdropped\b|denied|\bblocked\b|\bbanned\b|no layout|bad escape",
        RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex ZeroRx = new(@"^\s*0 (Error|Warning)\(s\)|\b0 errors?\b|fail(ed)?=0\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static LogLevel Classify(string text, LogLevel previous)
    {
        if (string.IsNullOrEmpty(text)) return LogLevel.Info;
        if (text.StartsWith("   at ", StringComparison.Ordinal) || text.StartsWith("\tat ", StringComparison.Ordinal) ||
            text.StartsWith(" ---> ", StringComparison.Ordinal))
            return previous == LogLevel.Info ? LogLevel.Error : previous;
        if (text.Contains("WinFormsMetadataUpdateHandler", StringComparison.Ordinal)) return LogLevel.Info; // harmless dotnet watch noise
        if (ZeroRx.IsMatch(text)) return LogLevel.Info;                                                      // "0 Error(s)" build summary
        if (text.Contains("ONLINE ", StringComparison.Ordinal) && text.Contains("│", StringComparison.Ordinal)) return LogLevel.Info; // perf line
        if (ErrorRx.IsMatch(text)) return LogLevel.Error;
        if (WarnRx.IsMatch(text)) return LogLevel.Warning;
        return LogLevel.Info;
    }

    public static List<LogLine> Lines(IEnumerable<string> raw)
    {
        var r = new List<LogLine>();
        var last = LogLevel.Info;
        foreach (var t in raw)
        {
            last = Classify(t, last);
            r.Add(new LogLine { Text = t, Level = last });
        }
        return r;
    }
}

/// <summary>Reads a growing log file incrementally (shared read, survives truncation / re-creation).</summary>
public sealed class FileTail
{
    public string Path { get; }
    private long _pos = -1;
    private string _partial = "";

    public FileTail(string path) => Path = path;

    public void Reset() { _pos = -1; _partial = ""; }

    /// <summary>New complete lines since the last call; the first call returns the last <paramref name="initialBytes"/>.</summary>
    public List<string> Read(int initialBytes = 256 * 1024)
    {
        var lines = new List<string>();
        try
        {
            if (!File.Exists(Path)) { _pos = 0; return lines; }
            using var fs = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
            long len = fs.Length;
            if (_pos < 0 || len < _pos)
            {
                _pos = Math.Max(0, len - initialBytes);
                _partial = "";
                if (_pos > 0)
                {
                    // skip to the next line start
                    fs.Position = _pos;
                    int b;
                    while ((b = fs.ReadByte()) >= 0 && b != '\n') _pos++;
                    _pos++;
                }
            }
            if (len == _pos) return lines;
            fs.Position = _pos;
            var buf = new byte[len - _pos];
            int read = 0;
            while (read < buf.Length)
            {
                int n = fs.Read(buf, read, buf.Length - read);
                if (n <= 0) break;
                read += n;
            }
            _pos += read;
            string text = _partial + Encoding.UTF8.GetString(buf, 0, read);
            int lastNl = text.LastIndexOf('\n');
            if (lastNl < 0) { _partial = text; return lines; }
            _partial = text[(lastNl + 1)..];
            foreach (var l in text[..lastNl].Split('\n')) lines.Add(l.TrimEnd('\r'));
        }
        catch { }
        return lines;
    }

    /// <summary>The last <paramref name="n"/> lines (one shot, no state).</summary>
    public static List<string> Last(string path, int n, int maxBytes = 512 * 1024)
    {
        var t = new FileTail(path);
        var l = t.Read(maxBytes);
        return l.Count > n ? l.GetRange(l.Count - n, n) : l;
    }
}

/// <summary>dotnet watch state from watch.err.log.</summary>
public sealed class HotReloadState
{
    public Health Health = Health.Unknown;
    public string Key = "hr.unknown";       // string key for the UI
    public string LastLine = "";
    public DateTime? LastApplied;
    public string LastFile = "";

    public static HotReloadState Read(string errLog)
    {
        var s = new HotReloadState();
        var lines = FileTail.Last(errLog, 400, 256 * 1024);
        if (lines.Count == 0) { s.Key = "hr.nolog"; return s; }
        DateTime written = File.GetLastWriteTime(errLog);
        for (int i = lines.Count - 1; i >= 0; i--)
        {
            string l = lines[i];
            if (l.Contains("Further changes won't be applied", StringComparison.OrdinalIgnoreCase) ||
                l.Contains("rude edit", StringComparison.OrdinalIgnoreCase) || l.Contains("Restart is needed", StringComparison.OrdinalIgnoreCase) ||
                l.Contains("Unable to apply", StringComparison.OrdinalIgnoreCase))
            { s.Health = Health.Bad; s.Key = "hr.needsRestart"; s.LastLine = Clean(l); break; }
            if (Regex.IsMatch(l, @"error CS\d+")) { s.Health = Health.Bad; s.Key = "hr.compileError"; s.LastLine = Clean(l); break; }
            if (l.Contains("changes applied", StringComparison.OrdinalIgnoreCase))
            { s.Health = Health.Ok; s.Key = "hr.applied"; s.LastLine = Clean(l); s.LastApplied = written; break; }
            if (l.Contains("No managed code changes", StringComparison.OrdinalIgnoreCase))
            { s.Health = Health.Ok; s.Key = "hr.nochange"; s.LastLine = Clean(l); s.LastApplied = written; break; }
            if (l.Contains("Waiting for changes", StringComparison.OrdinalIgnoreCase))
            { s.Health = Health.Ok; s.Key = "hr.waiting"; s.LastLine = Clean(l); break; }
            if (l.Contains("Exited with error", StringComparison.OrdinalIgnoreCase) || l.Contains("exited with", StringComparison.OrdinalIgnoreCase))
            { s.Health = Health.Bad; s.Key = "hr.exited"; s.LastLine = Clean(l); break; }
        }
        var f = lines.LastOrDefault(x => x.Contains("File updated:") || x.Contains("Files updated:"));
        if (f != null) s.LastFile = Clean(f[(f.IndexOf("updated:", StringComparison.Ordinal) + 8)..]);
        if (s.Key == "hr.unknown") { s.Health = Health.Warn; s.LastLine = Clean(lines[^1]); }
        return s;
    }

    /// <summary>Drops the "dotnet watch" prefix and the emoji marks.</summary>
    public static string Clean(string l)
    {
        l = l.Replace("dotnet watch", "").Trim();
        int i = 0;
        while (i < l.Length && (char.IsSurrogate(l[i]) || l[i] > 0x2000 && !char.IsLetterOrDigit(l[i]) || l[i] == ' ' || l[i] == '️')) i++;
        return l[i..].Trim();
    }
}
