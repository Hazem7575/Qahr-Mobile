using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using ScriptHazem.Kit;

namespace MobileStudio.Core;

public sealed class GitChange
{
    public string Code { get; set; }      // porcelain XY
    public string Path { get; set; }
    public string StatusText => Code.Trim() switch
    {
        "??" => Loc.Get("git.st.new"),
        "M" or "MM" or "AM" => Loc.Get("git.st.modified"),
        "A" => Loc.Get("git.st.added"),
        "D" => Loc.Get("git.st.deleted"),
        "R" or "RM" => Loc.Get("git.st.renamed"),
        _ => Code.Trim(),
    };
    public string Kind => Code.Trim() switch { "??" or "A" => "new", "D" => "del", _ => "mod" };
}

public sealed class SecretHit
{
    public string File { get; set; }
    public int Line { get; set; }
    public string Rule { get; set; }
    public string Where => Line > 0 ? $"{File}:{Line}" : File;
    public string RuleText => Loc.Opt("sec.rule.", Rule);
}

public sealed class SyncMap
{
    public string From { get; set; }
    public string To { get; set; }
    public bool IsFile { get; set; }
}

/// <summary>git status / sync (robocopy) / secret scan / commit + push for the conquer-mobile repository.</summary>
public static class GitOps
{
    static AppSettings S => Svc.Settings;

    public static Task<Proc.Result> Git(IEnumerable<string> args, Action<string> line = null, CancellationToken ct = default) =>
        Proc.RunAsync(S.GitExe, new[] { "-c", "core.quotepath=false" }.Concat(args), S.RepoDir, line, ct);

    public static async Task<(string branch, string ahead, List<GitChange> changes, string lastCommit)> StatusAsync()
    {
        var r = await Git(new[] { "status", "--porcelain=v1", "-b", "-uall" });
        var list = new List<GitChange>();
        string branch = "", ahead = "";
        foreach (var raw in r.Output.Split('\n'))
        {
            string l = raw.TrimEnd('\r');
            if (l.StartsWith("## "))
            {
                var m = Regex.Match(l, @"^## (?<b>[^.\s]+)(?:\.\.\.(?<u>\S+))?(?: \[(?<a>[^\]]+)\])?");
                branch = m.Groups["b"].Value;
                ahead = m.Groups["a"].Value;
                continue;
            }
            if (l.Length < 4) continue;
            string p = l[3..];
            if (p.Contains(" -> ")) p = p[(p.IndexOf(" -> ", StringComparison.Ordinal) + 4)..];
            list.Add(new GitChange { Code = l[..2], Path = p.Trim('"') });
        }
        var log = await Git(new[] { "log", "-1", "--format=%h  %an  %ad  %s", "--date=format:%Y-%m-%d %H:%M" });
        return (branch, ahead, list, log.Output.Trim());
    }

    /// <summary>Live folders → repository folders (QahrMobile → server\).</summary>
    public static List<SyncMap> Maps()
    {
        string srv = Path.Combine(S.RepoDir, "server");
        var l = new List<SyncMap>();
        foreach (var d in new[] { "Data", "libs", "src", "Tools", "scripts" })
            if (Directory.Exists(Path.Combine(S.QahrRoot, d)))
                l.Add(new SyncMap { From = Path.Combine(S.QahrRoot, d), To = Path.Combine(srv, d) });
        return l;
    }

    public static List<string> RoboArgs(SyncMap m, bool listOnly)
    {
        var a = new List<string>();
        if (m.IsFile)
        {
            a.Add(Path.GetDirectoryName(m.From));
            a.Add(Path.GetDirectoryName(m.To));
            a.Add(Path.GetFileName(m.From));
        }
        else
        {
            a.Add(m.From);
            a.Add(m.To);
            a.Add("/MIR");
            a.Add("/XD");
            a.AddRange(S.SyncExcludeDirs);
        }
        a.Add("/XF");
        a.AddRange(S.SyncExcludeFiles);
        a.AddRange(new[] { "/R:1", "/W:1", "/NP", "/NJH", "/NJS", "/NDL", "/FFT" });
        if (listOnly) a.Add("/L");
        return a;
    }

    /// <summary>Runs (or previews with /L) every map. Returns the robocopy lines that name a file.</summary>
    public static async Task<List<string>> SyncAsync(bool listOnly, Action<string> line, CancellationToken ct)
    {
        var files = new List<string>();
        foreach (var m in Maps())
        {
            line(Loc.F(listOnly ? "git.sync.preview" : "git.sync.copy", m.From, m.To));
            var r = await Proc.RunAsync("robocopy.exe", RoboArgs(m, listOnly), null, l =>
            {
                string t = l.Trim();
                if (t.Length == 0) return;
                files.Add(t);
                line("  " + t);
            }, ct);
            if (r.Code >= 8) line(Loc.F("git.sync.error", r.Code));
        }
        if (!listOnly) ActionLog.Write("git-sync", $"{files.Count} entries");
        return files;
    }

    // -------------------------------------------------------------------------------------------- secret scan
    /// <summary>Known secret values (DB password, AdminApi / PacketInject secrets, transfer key) — kept only in memory
    /// to search the changed files for them; never shown.</summary>
    private static List<string> KnownSecrets()
    {
        var l = new List<string>();
        void FromIni(string file, params (string sec, string key)[] keys)
        {
            if (!File.Exists(file)) return;
            string cur = "";
            foreach (var raw in File.ReadAllLines(file))
            {
                string t = raw.Trim();
                if (t.StartsWith("[") && t.EndsWith("]")) { cur = t[1..^1]; continue; }
                int i = t.IndexOf('=');
                if (i <= 0) continue;
                string k = t[..i].Trim(), v = t[(i + 1)..].Trim();
                if (v.Length >= 6 && keys.Any(x => (x.sec == null || x.sec.Equals(cur, StringComparison.OrdinalIgnoreCase)) && x.key.Equals(k, StringComparison.OrdinalIgnoreCase)))
                    l.Add(v);
            }
        }
        FromIni(S.ConfigIni, ("Database", "Password"), ("Transfer", "Key"), ("Transfer", "Salt"));
        FromIni(S.AdminApiTxt, (null, "secret"));
        FromIni(Path.Combine(S.DataDir, "packetinject.txt"), (null, "secret"));
        return l.Distinct().ToList();
    }

    private static readonly (string rule, Regex rx)[] Patterns =
    {
        ("privatekey", new Regex(@"-----BEGIN (RSA |EC |OPENSSH |ENCRYPTED )?PRIVATE KEY-----", RegexOptions.Compiled)),
        ("password", new Regex(@"(?i)\b(password|passwd|pwd)\s*[=:]\s*[""']?(?!\s*$)(?!\*+)(?!<)(?!\$)(?!(change_?me|xxx+|your|example|placeholder|none|null|false|true)\b)[^\s""';,]{4,}", RegexOptions.Compiled)),
        ("secret", new Regex(@"(?i)^\s*secret\s*=\s*\S{6,}", RegexOptions.Compiled)),
        ("token", new Regex(@"\b(ghp_[A-Za-z0-9]{30,}|github_pat_[A-Za-z0-9_]{40,}|AKIA[0-9A-Z]{16}|xox[baprs]-[A-Za-z0-9-]{10,})\b", RegexOptions.Compiled)),
    };

    private static readonly string[] SecretFileNames = { "serverconfig.ini", "adminapi.txt", "packetinject.txt", "server_rsa.pem", "apk.keystore" };
    private static readonly string[] SecretExts = { ".pem", ".pfx", ".p12", ".jks", ".keystore", ".key" };

    public static async Task<List<SecretHit>> ScanAsync(List<GitChange> changes, Action<string> progress, CancellationToken ct)
    {
        var secrets = KnownSecrets();
        return await Task.Run(() =>
        {
            var hits = new List<SecretHit>();
            int i = 0;
            foreach (var c in changes)
            {
                ct.ThrowIfCancellationRequested();
                if (c.Kind == "del") continue;
                string full = Path.Combine(S.RepoDir, c.Path.Replace('/', '\\'));
                if (!File.Exists(full)) continue;
                string name = Path.GetFileName(full).ToLowerInvariant();
                if (SecretFileNames.Contains(name) || SecretExts.Contains(Path.GetExtension(name)))
                    hits.Add(new SecretHit { File = c.Path, Rule = "file" });
                var fi = new FileInfo(full);
                if (fi.Length > 95L * 1024 * 1024) { hits.Add(new SecretHit { File = c.Path, Rule = "big" }); continue; }
                if (fi.Length > 8L * 1024 * 1024 || IsBinary(full)) continue;
                int ln = 0;
                foreach (var line in File.ReadLines(full))
                {
                    ln++;
                    if (line.Length > 4000) continue;
                    foreach (var s in secrets)
                        if (line.Contains(s, StringComparison.Ordinal)) { hits.Add(new SecretHit { File = c.Path, Line = ln, Rule = "known" }); goto next; }
                    foreach (var (rule, rx) in Patterns)
                        if (rx.IsMatch(line)) { hits.Add(new SecretHit { File = c.Path, Line = ln, Rule = rule }); break; }
                    next:;
                }
                if (++i % 200 == 0) progress?.Invoke(Loc.F("sec.progress", i, changes.Count));
            }
            return hits;
        }, ct);
    }

    private static bool IsBinary(string f)
    {
        try
        {
            using var s = File.OpenRead(f);
            var b = new byte[4096];
            int n = s.Read(b, 0, b.Length);
            for (int i = 0; i < n; i++) if (b[i] == 0) return true;
            return false;
        }
        catch { return true; }
    }

    // -------------------------------------------------------------------------------------------- commit / push
    public static async Task<Proc.Result> CommitAsync(string message, Action<string> line, CancellationToken ct)
    {
        var add = await Git(new[] { "add", "-A" }, line, ct);
        if (!add.Ok) return add;
        string msgFile = Path.Combine(Path.GetTempPath(), "ms_commit_" + Guid.NewGuid().ToString("N") + ".txt");
        File.WriteAllText(msgFile, message.Trim() + "\n", new UTF8Encoding(false));
        try
        {
            string who = $"{S.GitAuthorName} <{S.GitAuthorEmail}>";
            var r = await Git(new[] { "-c", "user.name=" + S.GitAuthorName, "-c", "user.email=" + S.GitAuthorEmail,
                "commit", "--author=" + who, "-F", msgFile }, line, ct);
            if (r.Ok) ActionLog.Write("git-commit", message.Split('\n')[0]);
            return r;
        }
        finally { try { File.Delete(msgFile); } catch { } }
    }

    public static async Task<Proc.Result> PushAsync(Action<string> line, CancellationToken ct)
    {
        var r = await Git(new[] { "push", "origin", "HEAD" }, line, ct);
        if (r.Ok) ActionLog.Write("git-push", "origin HEAD");
        return r;
    }
}
