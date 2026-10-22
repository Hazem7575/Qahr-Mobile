using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MobileStudio.Core;
using PatchConsole;
using ScriptHazem.Kit;

namespace MobileStudio.Views;

/// <summary>The mobile update pages, built on PatchConsole's own PatchCore/Stats (same rules, same files, same
/// operations log), in the ScriptHazem look.</summary>
public partial class UpdatesView : UserControl, IPage, IShotTab
{
    public sealed class Bar
    {
        public string Label { get; set; }
        public double H { get; set; }
        public Brush Fill { get; set; }
        public string Tip { get; set; }
    }

    private readonly ObservableCollection<StageItem> _stage = new();
    private string _patchDir, _sourceRoot;
    private ServerConfig _cfg;
    private bool _busy;

    public UpdatesView()
    {
        InitializeComponent();
        StageList.ItemsSource = _stage;
    }

    public void SelectTab(string name) => Tabs.SelectedItem = name switch
    {
        "History" => TabHistory, "Stats" => TabStats, "Official" => TabOfficial, _ => TabNew,
    };

    public void OnShown() => _ = ReloadAsync();
    public void OnHidden() { }
    public void OnTick(int tick) { }
    public void OnStatus(ServerSnapshot s) { }
    public void OnLanguage() => _ = ReloadAsync();
    public Task PrepareScreenshotAsync() => ReloadAsync();

    private static string BaseRoot()
    {
        // same base as PatchConsole (its settings.txt), else the APK resources
        try
        {
            string f = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PatchConsole", "settings.txt");
            if (File.Exists(f))
                foreach (var l in File.ReadAllLines(f))
                    if (l.StartsWith("base=") && Directory.Exists(l[5..])) return l[5..];
        }
        catch { }
        string def = Path.Combine(Svc.Settings.ComobileRoot, "work", "base_smali", "assets", "ConquerRes");
        return Directory.Exists(def) ? def : null;
    }

    private async Task ReloadAsync()
    {
        _cfg = Svc.Config();
        _patchDir = _cfg?.PatchDir;
        if (_patchDir == null) { SubText.Text = Loc.Get("upd.noConfig"); return; }
        OpsLog.Dir = _patchDir;
        bool tests = ShowTests.IsChecked == true;
        var data = await Task.Run(() =>
        {
            var list = PatchCore.List(_patchDir);
            var ev = Stats.Read(_patchDir, tests);
            var per = Stats.DownloadsPerVersion(ev);
            foreach (var p in list) p.Downloads = per.GetValueOrDefault(p.Version);
            int last = PatchCore.LastUsedVersion(_patchDir);
            var man = OfficialManifest.Read(_patchDir);
            return (list, ev, last, man);
        });
        int current = PatchCore.CurrentVersion(data.list);
        SubText.Text = Loc.FB("upd.sub", current, data.list.Count, _cfg.Mode, _patchDir);
        if (string.IsNullOrWhiteSpace(VersionBox.Text) || !_busy) VersionBox.Text = (data.last + 1).ToString();
        VersionHint.Text = Loc.F("upd.versionHint", data.last);

        HistoryList.ItemsSource = data.list.AsEnumerable().Reverse().ToList();
        RollbackBtn.IsEnabled = data.list.Count > 0;

        // stats
        var ev = data.ev;
        StOk.Text = ev.Count(e => e.Kind == "get" && e.Result == "ok").ToString();
        StFail.Text = ev.Count(e => e.Kind == "get" && e.Result != "ok").ToString();
        StDev.Text = ev.Select(e => e.Device).Distinct().Count().ToString();
        StToday.Text = ev.Count(e => e.Kind == "check" && e.Time.Date == DateTime.Today).ToString();
        var days = Enumerable.Range(0, 14).Select(i => DateTime.Today.AddDays(i - 13)).ToList();
        var counts = days.Select(d => (d, ok: ev.Count(e => e.Kind == "get" && e.Result == "ok" && e.Time.Date == d),
                                         checks: ev.Count(e => e.Kind == "check" && e.Time.Date == d))).ToList();
        int max = Math.Max(1, counts.Max(c => c.ok + c.checks));
        Chart.ItemsSource = counts.Select(c => new Bar
        {
            Label = c.d.ToString("dd/MM"),
            H = Math.Max(2, 90.0 * (c.ok + c.checks) / max),
            Fill = c.ok > 0 ? Brushes2.Get("GoldAccentBrush") : Brushes2.Get("LineStrongBrush"),
            Tip = Loc.F("upd.barTip", c.d.ToString("yyyy-MM-dd"), c.ok, c.checks),
        }).ToList();
        EventList.ItemsSource = ev.AsEnumerable().Reverse().Take(500).ToList();

        // official
        var man = data.man;
        OffLatest.Text = man?.LatestOfficial is { Length: > 0 } l ? l : "—";
        OffChecked.Text = man?.CheckedAt?.ToString("yyyy-MM-dd HH:mm") ?? "—";
        OffSource.Text = man?.Source is { Length: > 0 } src ? src : man?.Error ?? Loc.Get("upd.off.none");
        OfficialList.ItemsSource = man?.Patches ?? new List<OfficialEntry>();
        UpdateReview();
    }

    private void Refresh_Click(object sender, RoutedEventArgs e) => _ = ReloadAsync();

    private void OpenConsole_Click(object sender, RoutedEventArgs e)
    {
        string exe = Svc.Settings.PatchConsoleExe;
        if (File.Exists(exe)) Process.Start(new ProcessStartInfo(exe) { UseShellExecute = true, WorkingDirectory = Path.GetDirectoryName(exe) });
        else MainWindow.Toast_(Loc.FB("upd.noConsole", exe), true);
    }

    private void GoMirror_Click(object sender, RoutedEventArgs e) => MainWindow.Instance.Navigate("Mirror");

    // ================================================================== new update
    private void Drop_DragOver(object sender, DragEventArgs e)
    {
        e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        e.Handled = true;
    }

    private void Drop_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetData(DataFormats.FileDrop) is string[] paths && paths.Length > 0)
        {
            var dir = paths.FirstOrDefault(Directory.Exists);
            if (dir != null) _ = LoadFolderAsync(dir);
            else _ = LoadFilesAsync(paths.Where(File.Exists).ToList());
        }
    }

    private void PickFolder_Click(object sender, RoutedEventArgs e)
    {
        var d = new Microsoft.Win32.OpenFolderDialog { Title = Loc.Get("upd.pickFolder") };
        if (d.ShowDialog() == true) _ = LoadFolderAsync(d.FolderName);
    }

    /// <summary>A folder laid out like ConquerRes (ini\, luaui\ ...); a folder named like a top folder (ini) goes under it.</summary>
    private async Task LoadFolderAsync(string dir)
    {
        string baseRoot = BaseRoot();
        string prefix = "";
        if (baseRoot != null && Directory.Exists(Path.Combine(baseRoot, Path.GetFileName(dir))) &&
            !Directory.GetDirectories(dir).Any(d => Directory.Exists(Path.Combine(baseRoot, Path.GetFileName(d)))))
            prefix = Path.GetFileName(dir) + "/";
        _sourceRoot = dir;
        var files = Directory.GetFiles(dir, "*", SearchOption.AllDirectories);
        await Classify(files.Select(f => (f, prefix + Path.GetRelativePath(dir, f).Replace('\\', '/'))).ToList(), baseRoot);
        SourceText.Text = Loc.FB("upd.source", dir, files.Length);
    }

    private async Task LoadFilesAsync(List<string> files)
    {
        string baseRoot = BaseRoot();
        var bl = await Task.Run(() => new PatchCore.Baseline(baseRoot, _patchDir));
        var items = files.Select(f =>
        {
            string n = Path.GetFileName(f);
            string target = bl.NameIndex.TryGetValue(n, out var l) && l.Count == 1 ? l[0] : n;
            return (f, target);
        }).ToList();
        await Classify(items, baseRoot);
        SourceText.Text = Loc.F("upd.sourceFiles", files.Count);
    }

    private async Task Classify(List<(string src, string target)> files, string baseRoot)
    {
        _stage.Clear();
        StageTitle.Text = Loc.Get("upd.comparing");
        var items = await Task.Run(() =>
        {
            var bl = new PatchCore.Baseline(baseRoot, _patchDir);
            var list = files.Select(x => new StageItem { Source = x.src, Target = x.target }).ToList();
            foreach (var it in list) bl.Classify(it);
            return list;
        });
        foreach (var it in items.OrderBy(i => i.Status == ItemStatus.Same).ThenBy(i => i.Target)) _stage.Add(it);
        UpdateReview();
    }

    private async void Target_LostFocus(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.DataContext is not StageItem it) return;
        string baseRoot = BaseRoot();
        await Task.Run(() => new PatchCore.Baseline(baseRoot, _patchDir).Classify(it));
        UpdateReview();
    }

    private void Include_Changed(object sender, RoutedEventArgs e) => UpdateReview();

    private void UpdateReview()
    {
        if (ReviewText == null) return;
        var inc = _stage.Where(s => s.Include && s.Status != ItemStatus.Bad).ToList();
        StageTitle.Text = _stage.Count == 0 ? Loc.Get("upd.files")
            : Loc.F("upd.filesN", _stage.Count, _stage.Count(s => s.Status == ItemStatus.New), _stage.Count(s => s.Status == ItemStatus.Changed),
                    _stage.Count(s => s.Status == ItemStatus.Same), _stage.Count(s => s.Status == ItemStatus.Bad));
        long size = inc.Sum(s => s.Size);
        bool modeOk = _cfg?.UpdateMode == true;
        bool httpUp = _cfg != null && PatchCore.IsListening(_cfg.HttpPort);
        ReviewText.Text = inc.Count == 0 ? Loc.Get("upd.reviewEmpty")
            : Loc.F("upd.review", inc.Count, Fmt.Size(size)) + "\n" +
              (modeOk ? Loc.Get("upd.modeOk") : Loc.Get("upd.modeReady")) + "\n" +
              (httpUp ? Loc.Get("upd.httpOk") : Loc.Get("upd.httpDown")) +
              (size > 50L * 1024 * 1024 ? "\n" + Loc.Get("upd.big") : "");
        PublishBtn.IsEnabled = inc.Count > 0 && !_busy;
    }

    private async void Publish_Click(object sender, RoutedEventArgs e)
    {
        var inc = _stage.Where(s => s.Include && s.Status != ItemStatus.Bad).ToList();
        if (inc.Count == 0 || _patchDir == null) return;
        if (!int.TryParse(VersionBox.Text.Trim(), out int v) || v <= PatchCore.BaseVersion || v >= 9999)
        { MainWindow.Toast_(Loc.Get("upd.badVersion"), true); return; }
        int last = PatchCore.LastUsedVersion(_patchDir);
        if (v <= last) { MainWindow.Toast_(Loc.F("upd.versionUsed", last), true); return; }
        var steps = new List<string>
        {
            Loc.F("upd.step.build", v, inc.Count),
            Loc.Get("upd.step.verify"),
            Loc.FB("upd.step.copy", Path.Combine(_patchDir, v.ToString("0000") + ".zip")),
            Loc.Get("upd.step.live"),
        };
        if (!ConfirmDialog.Ask(Loc.F("upd.confirmTitle", v), Loc.Get("upd.confirmMsg"), steps, Loc.Get("upd.publish"),
                note: _cfg.UpdateMode ? null : Loc.Get("upd.modeReady")))
            return;
        _busy = true;
        PublishBtn.IsEnabled = false;
        PubProgress.Visibility = Visibility.Visible;
        MainWindow.BusyJobs++;
        try
        {
            var prog = new Progress<double>(p => PubProgress.Value = p);
            string tmp = await Task.Run(() => PatchCore.Build(v, inc, new Progress<double>(p => Dispatcher.BeginInvoke(() => PubProgress.Value = p * 0.5))));
            var meta = new PatchMeta
            {
                Version = v, Title = TitleBox.Text.Trim(), Notes = NotesBox.Text.Trim(), Created = DateTime.Now, Author = Environment.UserName,
                Sha256 = PatchCore.Sha256File(tmp), Size = new FileInfo(tmp).Length,
                Files = inc.Select(i => new MetaFile { Path = i.Target, Size = i.Size, Sha256 = i.Hash, Status = i.Status.ToString() }).ToList(),
            };
            string dest = await PatchCore.PublishAsync(tmp, meta, _patchDir, new Progress<double>(p => PubProgress.Value = 0.5 + p * 0.5));
            OpsLog.Write("نشر", $"{v}.zip ({inc.Count} ملف، {Fmt.Size(meta.Size)}) من MobileStudio");
            ActionLog.Write("publish-update", $"{v}.zip files={inc.Count}");
            MainWindow.Toast_(Loc.FB("upd.published", v));
            _stage.Clear();
            TitleBox.Text = NotesBox.Text = "";
            _busy = false;
            await ReloadAsync();
            Tabs.SelectedItem = TabHistory;
        }
        catch (Exception ex) { MainWindow.Toast_(ex.Message, true); }
        finally
        {
            _busy = false;
            MainWindow.BusyJobs--;
            PubProgress.Visibility = Visibility.Collapsed;
            UpdateReview();
        }
    }

    // ================================================================== history
    private void History_Selected(object sender, SelectionChangedEventArgs e)
    {
        if (HistoryList.SelectedItem is not PatchInfo p) return;
        DetailTitle.Text = Loc.FB("upd.detailOf", p.Version);
        DetailNotes.Text = (p.Meta?.Notes is { Length: > 0 } n ? n + "\n" : "") + (p.Meta?.Sha256 is { Length: > 0 } h ? "SHA-256 " + h[..16] + "…" : "");
        DetailFiles.ItemsSource = p.Entries.Where(x => !x.EndsWith("/")).Take(3000).ToList();
        RemoveBtn.IsEnabled = true;
    }

    private async void Rollback_Click(object sender, RoutedEventArgs e)
    {
        var list = PatchCore.List(_patchDir);
        var lastP = list.LastOrDefault();
        if (lastP == null) return;
        int prev = list.Count > 1 ? list[^2].Version : PatchCore.BaseVersion;
        if (!ConfirmDialog.Ask(Loc.F("upd.rollbackTitle", lastP.Version), Loc.Get("upd.rollbackMsg"),
                new[] { Loc.FB("upd.step.move", lastP.Version, Path.Combine(_patchDir, "_removed")), Loc.F("upd.step.newCurrent", prev), Loc.Get("upd.step.phones") },
                Loc.Get("upd.rollback"), true, note: lastP.Downloads > 0 ? Loc.F("upd.rollbackNote", lastP.Downloads) : null))
            return;
        await RemoveAsync(lastP.Version, "رجوع");
    }

    private async void Remove_Click(object sender, RoutedEventArgs e)
    {
        if (HistoryList.SelectedItem is not PatchInfo p) { MainWindow.Toast_(Loc.Get("upd.pickOne"), true); return; }
        var list = PatchCore.List(_patchDir);
        bool isLast = list.LastOrDefault()?.Version == p.Version;
        if (!ConfirmDialog.Ask(Loc.F("upd.removeTitle", p.Version), Loc.Get(isLast ? "upd.rollbackMsg" : "upd.removeMsgMiddle"),
                new[] { Loc.FB("upd.step.move", p.Version, Path.Combine(_patchDir, "_removed")), Loc.Get("upd.step.phones") },
                Loc.Get("upd.remove"), true, typeWord: p.Version.ToString()))
            return;
        await RemoveAsync(p.Version, "حذف");
    }

    private async Task RemoveAsync(int v, string op)
    {
        try
        {
            string dest = await Task.Run(() => PatchCore.Remove(_patchDir, v));
            OpsLog.Write(op, $"{v}.zip -> {Path.GetFileName(dest)} (MobileStudio)");
            ActionLog.Write("remove-update", $"{v}.zip");
            MainWindow.Toast_(Loc.FB("upd.removed", v));
        }
        catch (Exception ex) { MainWindow.Toast_(ex.Message, true); }
        await ReloadAsync();
    }

    private async void Test_Click(object sender, RoutedEventArgs e)
    {
        if (_cfg == null) return;
        TestBtn.IsEnabled = false;
        var lines = new ObservableCollection<string>();
        DetailTitle.Text = Loc.Get("upd.testTitle");
        DetailNotes.Text = Loc.Get("upd.testRunning");
        DetailFiles.ItemsSource = lines;
        try
        {
            bool ok = await PatchCore.SimulateClient(_cfg.CheckPort, _patchDir, (t, good) =>
                Dispatcher.BeginInvoke(() => lines.Add((good == false ? "✗ " : good == true ? "✓ " : "· ") + t)));
            DetailNotes.Text = Loc.Get(ok ? "upd.testOk" : "upd.testFail");
            OpsLog.Write("اختبار", (ok ? "نجح" : "فشل") + " (MobileStudio)");
        }
        catch (Exception ex) { DetailNotes.Text = ex.Message; }
        finally { TestBtn.IsEnabled = true; }
    }

    private void ShowTests_Changed(object sender, RoutedEventArgs e) { if (IsLoaded) _ = ReloadAsync(); }
}
