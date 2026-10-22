using System.Diagnostics;
using System.IO;
using System.Text;
using MobileStudio.Core;

namespace MobileStudio;

/// <summary>MobileStudio.exe --selftest report.txt [--apk outDir] [--backup]: runs every operation that does not start or
/// stop a server and writes a report. Never prints a secret.</summary>
public static class SelfTest
{
    /// <summary>--op name report.txt: runs ONE server operation with the same code as the buttons (for supervised tests).
    /// restart-acc | rebuild-acc | start-acc | stop-acc | safe-restart-game | start-game | stop-game | start-cdn | stop-cdn</summary>
    public static async Task<int> OpAsync(string op, string report)
    {
        var sb = new StringBuilder();
        void L(string s) { lock (sb) { sb.AppendLine($"{DateTime.Now:HH:mm:ss} {s}"); try { File.WriteAllText(report, sb.ToString()); } catch { } } }
        L("op " + op);
        bool ok;
        switch (op)
        {
            case "restart-acc": ok = await ServerOps.StopAccAsync(L) && await ServerOps.StartAccAsync(L); break;
            case "rebuild-acc": ok = await ServerOps.RebuildAccAsync(L, l => L("  " + l), default); break;
            case "start-acc": ok = await ServerOps.StartAccAsync(L); break;
            case "stop-acc": ok = await ServerOps.StopAccAsync(L); break;
            case "safe-restart-game": ok = await ServerOps.SafeRestartGameAsync(L, l => L("  " + l), default); break;
            case "start-game": ok = await ServerOps.StartGameAsync(L); break;
            case "stop-game": ok = await ServerOps.StopGameAsync(L); break;
            case "start-cdn": ok = await ServerOps.CdnAsync(false, L); break;
            case "stop-cdn": ok = await ServerOps.CdnAsync(true, L); break;
            default: L("unknown op"); return 2;
        }
        var m = await Svc.Monitor.PollAsync();
        L($"result {(ok ? "OK" : "FAILED")} | game pid={m.GamePid} watch={m.GameUnderWatch} port={m.GamePortUp} | acc pid={m.AccPid} port={m.AccPortUp} | cdn {m.CdnUp} | online {m.Online}");
        return ok ? 0 : 1;
    }

    public static async Task<int> RunAsync(string report, string apkOut, bool backup)
    {
        var sb = new StringBuilder();
        int fails = 0;
        void Line(string s) { sb.AppendLine(s); try { File.WriteAllText(report, sb.ToString()); } catch { } }
        async Task Step(string name, Func<Task<string>> f)
        {
            var sw = Stopwatch.StartNew();
            try { string res = await f(); Line($"[ OK ] {name} ({sw.Elapsed.TotalSeconds:0.0}s): {res}"); }
            catch (Exception ex) { fails++; Line($"[FAIL] {name} ({sw.Elapsed.TotalSeconds:0.0}s): {ex.Message}"); }
        }
        var s = Svc.Settings;
        Line($"MobileStudio self test {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

        await Step("monitor", async () =>
        {
            var m = await Svc.Monitor.PollAsync();
            return $"game pid={m.GamePid} watch=[{string.Join(",", m.WatchPids)}] underWatch={m.GameUnderWatch} port{m.GamePort}={m.GamePortUp} | acc pid={m.AccPid} port{m.AccPort}={m.AccPortUp} | " +
                   $"check {m.CheckUp} http {m.HttpUp} reg {m.RegisterUp} mysql {m.MysqlUp} admin {m.AdminUp} cdn {m.CdnUp} | online {m.Online} | hotreload {m.HotReload.Key} ({m.HotReload.LastLine})";
        });
        await Step("copy support (Ctrl+C on a log list, clipboard read back)", () => Task.FromResult(
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                CopySupport.Register();
                var w = new System.Windows.Window { Width = 400, Height = 300, Left = -20000, ShowInTaskbar = false, ShowActivated = false };
                var lb = new System.Windows.Controls.ListBox();
                var lines = Enumerable.Range(1, 50).Select(i => new LogLine { Text = "line " + i }).ToList();
                lb.ItemsSource = lines;
                w.Content = lb;
                w.Show();
                lb.UpdateLayout();
                var frame = new System.Windows.Threading.DispatcherFrame();   // let Loaded run (that is where copy support attaches)
                w.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, () => frame.Continue = false);
                System.Windows.Threading.Dispatcher.PushFrame(frame);
                CopySupport.AttachTree(w);
                if (lb.SelectionMode == System.Windows.Controls.SelectionMode.Single) { w.Close(); throw new Exception($"not attached: loaded={lb.IsLoaded} menu={lb.ContextMenu != null}"); }
                lb.SelectedItems.Add(lines[4]); lb.SelectedItems.Add(lines[2]);
                System.Windows.Input.ApplicationCommands.Copy.Execute(null, lb);
                string a = "";
                for (int i = 0; i < 10 && a.Length == 0; i++) { try { a = System.Windows.Clipboard.GetText(); } catch { } if (a.Length == 0) { Thread.Sleep(100); System.Windows.Input.ApplicationCommands.Copy.Execute(null, lb); } }
                var row = CopySupport.RowText(new PatchConsole.StatEvent { Time = new DateTime(2026, 10, 21, 17, 0, 0), Kind = "get", Version = "5171", Result = "ok", Bytes = 1024, Ip = "1.2.*.*", Device = "abc" });
                bool menu = lb.ContextMenu != null;
                w.Close();
                if (a.Replace("\r", "") != "line 3\nline 5") throw new Exception("clipboard: " + a);
                return $"selection copied in order ({a.Replace(Environment.NewLine, " | ")}); menu attached={menu}; table row = {row}";
            })));
        await Step("admin online", async () =>
        {
            var r = await AdminClient.CallAsync("online");
            if (!r.Ok) throw new Exception(r.Error);
            return (r.Json["players"] as System.Text.Json.Nodes.JsonArray)?.Count + " players";
        });
        await Step("admin listgms", async () => { var r = await AdminClient.CallAsync("listgms"); return r.Ok ? r.Message : throw new Exception(r.Error); });
        await Step("admin save all", async () => { var (ok, m) = await ServerOps.SaveAllAsync(); return ok ? m : throw new Exception(m); });
        await Step("stop targets (not stopped)", () => { var (g, w) = ServerOps.GameTargets(); return Task.FromResult($"game {g}, watch {string.Join(",", w)}, acc {ServerOps.AccPid()}"); });
        await Step("logs tail", () =>
        {
            var w = LogClassifier.Lines(FileTail.Last(s.WatchLog, 2000));
            var a = FileTail.Last(s.AccLog, 200);
            return Task.FromResult($"watch.log {w.Count} lines ({w.Count(l => l.Level == LogLevel.Error)} errors), acc.log {a.Count} lines");
        });
        await Step("updates (PatchCore)", () =>
        {
            var dir = Svc.Config().PatchDir;
            var l = PatchConsole.PatchCore.List(dir);
            var ev = PatchConsole.Stats.Read(dir, false);
            return Task.FromResult($"{l.Count} patches [{string.Join(", ", l.Select(p => p.Version + (p.Ok ? "" : "!")))}], current {PatchConsole.PatchCore.CurrentVersion(l)}, " +
                                   $"last used {PatchConsole.PatchCore.LastUsedVersion(dir)}, {ev.Count} stat events, official {PatchConsole.OfficialManifest.Read(dir)?.LatestOfficial}");
        });
        await Step("updates: test like a phone", async () =>
        {
            var cfg = Svc.Config();
            var log = new List<string>();
            bool ok = await PatchConsole.PatchCore.SimulateClient(cfg.CheckPort, cfg.PatchDir, (t, g) => log.Add((g == false ? "x " : "") + t));
            return (ok ? "passed" : "FAILED") + " | " + string.Join(" | ", log.Take(12));
        });
        await Step("mirror status", async () =>
        {
            var r = await MirrorOps.RunAsync(new[] { "status" }, null);
            if (!r.Ok) throw new Exception(r.Error);
            return $"latest {r.ResultNode?["latest_official_fetched"]}, cdn {r.ResultNode?["cdn"]?["have"]}/{r.ResultNode?["cdn"]?["files"]}";
        });
        await Step("mirror check (official servers, no download)", async () =>
        {
            var r = await MirrorOps.RunAsync(new[] { "check" }, null);
            if (!r.Ok) throw new Exception(r.Error);
            return $"steps {r.ResultNode?["steps"]?.ToJsonString()} new {r.ResultNode?["new_patches"]?.ToJsonString()}";
        });
        await Step("mirror republish dry run", async () =>
        {
            string next = (PatchConsole.PatchCore.LastUsedVersion(Svc.Config().PatchDir) + 1).ToString();
            var r = await MirrorOps.RunAsync(new[] { "republish", "5151-5171", "--as", next, "--dry-run" }, null);
            if (!r.Ok) throw new Exception(r.Error);
            var t = r.ResultNode?.ToJsonString() ?? "";
            return t.Length > 300 ? t[..300] + "…" : t;
        });
        await Step("tools page (catalog)", () => Task.FromResult(string.Join(", ", ToolCatalog.Load().Select(t => $"{t.Name}={(t.Exists ? "ready" : "not built")}"))));
        await Step("client source build.py --check", async () =>
        {
            var r = await Proc.RunAsync(s.PythonExe, new[] { "-I", Path.Combine(s.ClientSourceDir, "build.py"), "--check" }, s.ClientSourceDir);
            if (!r.Ok) throw new Exception(MirrorOps.LastLines(r.Output, 2));
            return MirrorOps.LastLines(r.Output, 1);
        });
        await Step("adb devices", async () => string.Join(", ", (await ApkOps.DevicesAsync()).Select(d => d.Label + " " + d.State)));
        await Step("git status", async () => { var (b, a, c, l) = await GitOps.StatusAsync(); return $"branch {b} {a}, {c.Count} changes, last {l}"; });
        await Step("git sync preview (robocopy /L)", async () =>
        {
            var lines = await GitOps.SyncAsync(true, _ => { }, default);
            return $"{lines.Count} lines; first: {string.Join(" | ", lines.Take(5))}";
        });
        await Step("secret scan (changes)", async () =>
        {
            var (_, _, c, _) = await GitOps.StatusAsync();
            var hits = await GitOps.ScanAsync(c, null, default);
            return $"{c.Count} files, {hits.Count} hits {string.Join(", ", hits.Take(5).Select(h => h.Where + " " + h.Rule))}";
        });
        await Step("secret scan self-check (planted test file)", async () =>
        {
            // a temp file under the repo with fake secrets must be caught, then it is deleted
            string f = Path.Combine(s.RepoDir, "_ms_selftest_secret.txt");
            File.WriteAllText(f, "password=Hunter2xyz\n-----BEGIN RSA PRIVATE KEY-----\n");
            try
            {
                var hits = await GitOps.ScanAsync(new List<GitChange> { new() { Code = "??", Path = "_ms_selftest_secret.txt" } }, null, default);
                if (hits.Count < 2) throw new Exception("planted secrets not found: " + hits.Count);
                return string.Join(", ", hits.Select(h => h.Where + " " + h.Rule));
            }
            finally { File.Delete(f); }
        });
        await Step("check build GameServer (staging, server untouched)", async () =>
        {
            var issues = new List<BuildIssue>();
            var r = await ServerOps.CheckBuildAsync(s.GameCsproj, "game-check", l => { var i = BuildIssue.Parse(l); if (i != null) issues.Add(i); }, default);
            return $"exit {r.Code}, {issues.Count(i => !i.IsWarning)} errors, {issues.Count(i => i.IsWarning)} warnings";
        });
        await Step("check build AccServer (staging)", async () =>
        {
            var issues = new List<BuildIssue>();
            var r = await ServerOps.CheckBuildAsync(s.AccCsproj, "acc-check", l => { var i = BuildIssue.Parse(l); if (i != null) issues.Add(i); }, default);
            return $"exit {r.Code}, {issues.Count(i => !i.IsWarning)} errors, {issues.Count(i => i.IsWarning)} warnings";
        });
        if (backup)
        {
            BackupInfo b = null;
            await Step("backup db + data", async () =>
            {
                b = await BackupOps.CreateAsync(true, true, "full", "self test", _ => { }, default);
                return $"{b.Name} {b.SizeText} db={b.HasDb} files={b.DataFiles}";
            });
            if (b != null)
            {
                await Step("verify backup (dump complete, tables = live, zip CRC)", async () =>
                {
                    var (ok, text) = await BackupOps.VerifyAsync(b.File, default);
                    return ok ? text : throw new Exception(text);
                });
                if (Environment.GetEnvironmentVariable("MS_TEST_DBCOPY") == "1")
                await Step("restore db into a check copy + drop it", async () =>
                {
                    const string copy = "qahr_mobile_mscheck";
                    await BackupOps.RestoreDbAsync(b.File, copy, _ => { }, default);
                    string live = await BackupOps.QueryAsync($"SELECT COUNT(*) FROM information_schema.tables WHERE table_schema='{BackupOps.DbName}'");
                    string got = await BackupOps.QueryAsync($"SELECT COUNT(*) FROM information_schema.tables WHERE table_schema='{copy}'");
                    await BackupOps.QueryAsync($"DROP DATABASE `{copy}`");
                    if (live != got) throw new Exception($"tables live {live} vs copy {got}");
                    return $"tables {got} = live {live}; copy dropped";
                });
                await Step("restore data into a check folder", async () =>
                {
                    string dir = Path.Combine(Path.GetTempPath(), "ms_restore_check");
                    if (Directory.Exists(dir)) Directory.Delete(dir, true);
                    int n = await BackupOps.RestoreDataAsync(b.File, dir, _ => { }, default);
                    string probe = Path.Combine(dir, "mobile_ids.txt");
                    bool same = File.Exists(probe) && File.ReadAllBytes(probe).SequenceEqual(File.ReadAllBytes(Path.Combine(s.DataDir, "mobile_ids.txt")));
                    Directory.Delete(dir, true);
                    return $"{n} files restored, mobile_ids.txt identical={same}";
                });
                if (fails == 0) await Step("delete the self test backup", () => { File.Delete(b.File); return Task.FromResult("deleted"); });
            }
        }
        if (apkOut != null)
        {
            await Step("build APK (scratch output)", async () =>
            {
                Directory.CreateDirectory(apkOut);
                var ip = Svc.Config().ServerIp;
                var args = ApkOps.BuildArgs(ip, 16100, "", true, apkOut, 9548, $"http://{ip}:9550/ydzf/android/zf");
                var r = await Proc.RunAsync(s.PythonExe, args, s.ComobileRoot);
                var (single, splits) = ApkOps.Outputs(apkOut);
                if (!r.Ok || single == null) throw new Exception($"exit {r.Code}: {MirrorOps.LastLines(r.Output, 3)}");
                return $"{single} {Fmt.Size(new FileInfo(single).Length)}, {splits.Count} splits";
            });
        }
        Line(fails == 0 ? "ALL OK" : $"{fails} FAILED");
        return fails;
    }
}
