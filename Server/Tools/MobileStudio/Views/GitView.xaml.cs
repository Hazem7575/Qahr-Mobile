using System.IO;
using System.Windows;
using System.Windows.Controls;
using MobileStudio.Core;
using ScriptHazem.Kit;

namespace MobileStudio.Views;

public partial class GitView : UserControl, IPage
{
    private List<GitChange> _changes = new();
    private List<SecretHit> _hits;
    private string _scannedFor;       // a fingerprint of the changes that were scanned
    private string _branch = "";
    private bool _busy;
    private CancellationTokenSource _cts;

    public GitView() => InitializeComponent();

    public void OnShown() => _ = RefreshAsync();
    public void OnHidden() { }
    public void OnTick(int tick) { }
    public void OnStatus(ServerSnapshot s) { }
    public void OnLanguage() => _ = RefreshAsync();
    public Task PrepareScreenshotAsync() => RefreshAsync();

    private string Fingerprint() => string.Join("|", _changes.Select(c => c.Code + c.Path)) + "#" +
                                    _changes.Where(c => c.Kind != "del").Sum(c => { try { return File.GetLastWriteTimeUtc(Path.Combine(Svc.Settings.RepoDir, c.Path)).Ticks % 1000003; } catch { return 0; } });

    private async Task RefreshAsync()
    {
        if (!Directory.Exists(Path.Combine(Svc.Settings.RepoDir, ".git"))) { SubText.Text = Loc.FB("git.noRepo", Svc.Settings.RepoDir); return; }
        var (branch, ahead, changes, last) = await GitOps.StatusAsync();
        _branch = branch;
        _changes = changes;
        SubText.Text = Loc.FB("git.sub", Svc.Settings.RepoDir, branch) + (ahead.Length > 0 ? "  ·  " + ahead : "") + "\n" + Loc.FB("git.last", last) +
                       "\n" + Loc.FB("git.author", $"{Svc.Settings.GitAuthorName} <{Svc.Settings.GitAuthorEmail}>");
        ChangeList.ItemsSource = changes.Take(3000).ToList();
        ChangesTitle.Text = Loc.F("git.changesN", changes.Count, changes.Count(c => c.Kind == "new"), changes.Count(c => c.Kind == "mod"), changes.Count(c => c.Kind == "del"));
        Clean.Visibility = changes.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        if (_scannedFor != null && _scannedFor != Fingerprint()) ShowScan(null);
        UpdateCommit();
    }

    private void Refresh_Click(object sender, RoutedEventArgs e) => _ = RefreshAsync();
    private void Msg_Changed(object sender, TextChangedEventArgs e) => UpdateCommit();

    private void UpdateCommit()
    {
        bool scannedOk = _hits != null && _hits.Count == 0 && _scannedFor == Fingerprint();
        CommitBtn.IsEnabled = !_busy && _changes.Count > 0 && MsgBox.Text.Trim().Length >= 3 && scannedOk;
        CommitBtn.ToolTip = scannedOk ? null : Loc.Get("git.scanFirst");
    }

    private void SetBusy(bool b)
    {
        _busy = b;
        PreviewBtn.IsEnabled = SyncBtn.IsEnabled = ScanBtn.IsEnabled = !b;
        if (b) MainWindow.BusyJobs++; else MainWindow.BusyJobs--;
        UpdateCommit();
    }

    private void ShowScan(List<SecretHit> hits)
    {
        _hits = hits;
        if (hits == null)
        {
            _scannedFor = null;
            ScanBanner.Style = (Style)FindResource("InfoBanner");
            ScanGlyph.Text = ""; ScanGlyph.Foreground = Brushes2.Info;
            ScanText.Text = Loc.Get("git.notScanned");
            HitList.ItemsSource = null;
            return;
        }
        _scannedFor = Fingerprint();
        bool ok = hits.Count == 0;
        ScanBanner.Style = (Style)FindResource(ok ? "OkBanner" : "ErrBanner");
        ScanGlyph.Text = ok ? "" : "";
        ScanGlyph.Foreground = ok ? Brushes2.Ok : Brushes2.Err;
        ScanText.Text = ok ? Loc.F("git.scanOk", _changes.Count) : Loc.F("git.scanHits", hits.Count);
        HitList.ItemsSource = hits.Take(40).ToList();
    }

    private async void Preview_Click(object sender, RoutedEventArgs e)
    {
        OutputBox.Clear();
        SetBusy(true);
        _cts = new CancellationTokenSource();
        try
        {
            var files = await GitOps.SyncAsync(true, Ui.Sink(OutputBox), _cts.Token);
            Ui.Append(OutputBox, Loc.F("git.previewDone", files.Count));
        }
        finally { SetBusy(false); }
    }

    private async void Sync_Click(object sender, RoutedEventArgs e)
    {
        var maps = GitOps.Maps();
        var steps = maps.Select(m => Loc.FB("git.step.map", m.From, m.To.Replace(Svc.Settings.RepoDir + "\\", ""))).ToList();
        steps.Add(Loc.FB("git.step.exclude", string.Join(" ", Svc.Settings.SyncExcludeDirs) + " · " + string.Join(" ", Svc.Settings.SyncExcludeFiles)));
        if (!ConfirmDialog.Ask(Loc.Get("git.syncTitle"), Loc.Get("git.syncMsg"), steps, Loc.Get("git.sync"), true, note: Loc.Get("git.syncNote")))
            return;
        OutputBox.Clear();
        SetBusy(true);
        _cts = new CancellationTokenSource();
        try
        {
            var files = await GitOps.SyncAsync(false, Ui.Sink(OutputBox), _cts.Token);
            Ui.Append(OutputBox, Loc.F("git.syncDone", files.Count));
        }
        finally { SetBusy(false); await RefreshAsync(); }
    }

    private async void Scan_Click(object sender, RoutedEventArgs e)
    {
        await RefreshAsync();
        SetBusy(true);
        ScanText.Text = Loc.Get("git.scanning");
        _cts = new CancellationTokenSource();
        try
        {
            var hits = await GitOps.ScanAsync(_changes, s => Dispatcher.BeginInvoke(() => ScanText.Text = s), _cts.Token);
            ShowScan(hits);
            ActionLog.Write("secret-scan", $"{_changes.Count} files, {hits.Count} hits");
        }
        catch (Exception ex) { ScanText.Text = ex.Message; }
        finally { SetBusy(false); }
    }

    private async void Commit_Click(object sender, RoutedEventArgs e)
    {
        await RefreshAsync();
        if (_scannedFor != Fingerprint()) { ShowScan(null); MainWindow.Toast_(Loc.Get("git.changedSinceScan"), true); UpdateCommit(); return; }
        string msg = MsgBox.Text.Trim();
        bool push = PushBox.IsChecked == true;
        var steps = new List<string>
        {
            Loc.F("git.step.add", _changes.Count),
            Loc.FB("git.step.commit", $"{Svc.Settings.GitAuthorName} <{Svc.Settings.GitAuthorEmail}>"),
        };
        if (push) steps.Add(Loc.FB("git.step.push", _branch));
        if (!ConfirmDialog.Ask(Loc.Get("git.commitTitle"), msg, steps, Loc.Get(push ? "git.commitPush" : "git.commit"), push))
            return;
        OutputBox.Clear();
        SetBusy(true);
        _cts = new CancellationTokenSource();
        try
        {
            var r = await GitOps.CommitAsync(msg, Ui.Sink(OutputBox), _cts.Token);
            if (!r.Ok) { MainWindow.Toast_(Loc.Get("git.commitFailed"), true); return; }
            if (push)
            {
                var p = await GitOps.PushAsync(Ui.Sink(OutputBox), _cts.Token);
                MainWindow.Toast_(p.Ok ? Loc.Get("git.pushed") : Loc.Get("git.pushFailed"), !p.Ok);
            }
            else MainWindow.Toast_(Loc.Get("git.committed"));
            MsgBox.Clear();
            ShowScan(null);
        }
        finally { SetBusy(false); await RefreshAsync(); }
    }
}
