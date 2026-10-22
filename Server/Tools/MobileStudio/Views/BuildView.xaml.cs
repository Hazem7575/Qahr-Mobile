using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MobileStudio.Core;
using ScriptHazem.Kit;

namespace MobileStudio.Views;

public partial class BuildView : UserControl, IPage, IShotTab
{
    private readonly ObservableCollection<BuildIssue> _issues = new();
    private CancellationTokenSource _cts;
    private bool _busy;
    private string _lastApkOut;

    // build_apk.py steps (in the order it prints them) → progress %
    private static readonly (string mark, int pct)[] ApkSteps =
    {
        ("RSA key", 5), ("code patch", 15), ("bytes ->", 30), ("login:", 40), ("autoupdate.ini", 50), ("signed", 75),
        ("single APK", 92), ("install:", 98),
    };

    public BuildView()
    {
        InitializeComponent();
        IssueList.ItemsSource = _issues;
        var s = Svc.Settings;
        IpBox.Text = s.ApkIp;
        PortBox.Text = s.ApkPort.ToString();
        RegBox.Text = s.ApkRegisterPort.ToString();
        PatchHostBox.Text = s.ApkPatchHost;
        SingleBox.IsChecked = s.ApkSingle;
        OutBox.Text = s.ApkOut;
        CdnBox.Text = s.ApkCdnUrl;
        foreach (var tb in new[] { IpBox, PortBox, RegBox, PatchHostBox, OutBox, CdnBox }) tb.TextChanged += (_, _) => UpdateCmd();
        SingleBox.Checked += (_, _) => UpdateCmd();
        SingleBox.Unchecked += (_, _) => UpdateCmd();
        UpdateCmd();
        UpdateTargetHint();
    }

    public void SelectTab(string name) => Tabs.SelectedItem = name.Equals("Apk", StringComparison.OrdinalIgnoreCase) ? TabApk : TabServer;
    public void OnShown() { if (DeviceBox.Items.Count == 0) _ = LoadDevicesAsync(); UpdateApkFile(); }
    public void OnHidden() { }
    public void OnTick(int tick) { }
    public void OnStatus(ServerSnapshot s) { UpdateTargetHint(); if (!_busy) LiveBtn.IsEnabled = true; }
    public void OnLanguage() { UpdateTargetHint(); UpdateApkFile(); }
    public async Task PrepareScreenshotAsync() => await LoadDevicesAsync();

    // ================================================================== server build
    private bool Game => TgtGame.IsChecked == true;

    private void Tgt_Changed(object sender, RoutedEventArgs e) { if (IsLoaded) UpdateTargetHint(); }

    private void UpdateTargetHint()
    {
        if (TgtHint == null) return;
        var s = Svc.Monitor.Last;
        bool running = Game ? s.GameRunning : s.AccRunning;
        TgtHint.Text = Loc.Get(running ? (Game ? "build.hintGameRunning" : "build.hintAccRunning") : "build.hintStopped");
    }

    private void SetBusy(bool b)
    {
        _busy = b;
        CheckBtn.IsEnabled = !b;
        LiveBtn.IsEnabled = !b;
        ApkBtn.IsEnabled = !b;
        CancelBtn.IsEnabled = b;
        ApkCancel.IsEnabled = b;
        InstallBtn.IsEnabled = !b;
        if (b) MainWindow.BusyJobs++; else MainWindow.BusyJobs--;
    }

    private void Banner(string style, string glyph, string text)
    {
        ResultBanner.Style = (Style)FindResource(style);
        ResultGlyph.Text = glyph;
        ResultGlyph.Foreground = style switch { "OkBanner" => Brushes2.Ok, "ErrBanner" => Brushes2.Err, "WarnBanner" => Brushes2.Warn, _ => Brushes2.Info };
        ResultText.Text = text;
    }

    private void OnBuildLine(string l)
    {
        Dispatcher.BeginInvoke(() =>
        {
            Ui.Append(OutputBox, l);
            var i = BuildIssue.Parse(l);
            if (i != null && !_issues.Any(x => x.File == i.File && x.Line == i.Line && x.Code == i.Code && x.Message == i.Message))
            {
                if (i.IsWarning) _issues.Add(i); else _issues.Insert(_issues.Count(x => !x.IsWarning), i);
                IssuesTitle.Text = Loc.F("build.issuesN", _issues.Count(x => !x.IsWarning), _issues.Count(x => x.IsWarning));
            }
        });
    }

    private async void Check_Click(object sender, RoutedEventArgs e)
    {
        string proj = Game ? Svc.Settings.GameCsproj : Svc.Settings.AccCsproj;
        _issues.Clear();
        OutputBox.Clear();
        IssuesTitle.Text = Loc.Get("build.issues");
        Banner("InfoBanner", "", Loc.F("build.running", Game ? "GameServer" : "AccServer"));
        SetBusy(true);
        _cts = new CancellationTokenSource();
        var sw = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            var r = await ServerOps.CheckBuildAsync(proj, Game ? "game-check" : "acc-check", OnBuildLine, _cts.Token);
            int errs = _issues.Count(x => !x.IsWarning), warns = _issues.Count(x => x.IsWarning);
            if (r.Cancelled) Banner("WarnBanner", "", Loc.Get("build.cancelled"));
            else if (r.Ok) Banner("OkBanner", "", Loc.F("build.ok", sw.Elapsed.TotalSeconds.ToString("0.0", Loc.Culture), warns));
            else Banner("ErrBanner", "", Loc.F("build.failed", errs, warns));
            ActionLog.Write("check-build", $"{Path.GetFileName(proj)} code={r.Code} errors={errs}");
        }
        finally { SetBusy(false); }
    }

    private async void Live_Click(object sender, RoutedEventArgs e)
    {
        // the real build needs the server stopped (its exe is locked); the start/stop happens on the Servers page
        var s = Svc.Monitor.Last;
        if (Game ? s.GameRunning : s.AccRunning)
        {
            if (ConfirmDialog.Ask(Loc.Get("build.liveRunningTitle"), Loc.Get(Game ? "build.liveRunningGame" : "build.liveRunningAcc"), null, Loc.Get("build.goServers")))
                MainWindow.Instance.Navigate("Servers");
            return;
        }
        string proj = Game ? Svc.Settings.GameCsproj : Svc.Settings.AccCsproj;
        if (!ConfirmDialog.Ask(Loc.Get("build.liveTitle"), Loc.F("build.liveMsg", Game ? "GameServer" : "AccServer"),
                new[] { Loc.FB("build.liveStep", "dotnet build " + Path.GetFileName(proj) + " -c Debug" + (Game ? " --property:Optimize=false" : "")),
                        Loc.Get("build.liveStep2") }, Loc.Get("build.live")))
            return;
        _issues.Clear();
        OutputBox.Clear();
        Banner("InfoBanner", "", Loc.F("build.running", Game ? "GameServer" : "AccServer"));
        SetBusy(true);
        _cts = new CancellationTokenSource();
        try
        {
            var r = await ServerOps.LiveBuildAsync(proj, Game, OnBuildLine, _cts.Token);
            int errs = _issues.Count(x => !x.IsWarning), warns = _issues.Count(x => x.IsWarning);
            if (r.Ok) Banner("OkBanner", "", Loc.F("build.liveOk", warns));
            else Banner("ErrBanner", "", Loc.F("build.failed", errs, warns));
            ActionLog.Write("live-build", $"{Path.GetFileName(proj)} code={r.Code}");
        }
        finally { SetBusy(false); }
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) => _cts?.Cancel();

    private void Issue_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (IssueList.SelectedItem is BuildIssue i) BuildIssue.Open(i.File, i.Line, i.Col);
    }

    // ================================================================== APK
    private (string ip, int port, int reg, string patch, bool single, string outDir, string error, string cdn) Form()
    {
        string ip = IpBox.Text.Trim();
        if (!System.Net.IPAddress.TryParse(ip, out _) && !System.Text.RegularExpressions.Regex.IsMatch(ip, @"^[A-Za-z0-9.-]+$"))
            return (ip, 0, 0, null, false, null, Loc.Get("apk.badIp"), null);
        if (!int.TryParse(PortBox.Text.Trim(), out int port) || port < 1 || port > 65535) return (ip, 0, 0, null, false, null, Loc.Get("apk.badPort"), null);
        if (!int.TryParse(RegBox.Text.Trim(), out int reg) || reg < 0 || reg > 65535) return (ip, 0, 0, null, false, null, Loc.Get("apk.badPort"), null);
        string cdn = CdnBox.Text.Trim();
        if (cdn.Length > 0 && !System.Text.RegularExpressions.Regex.IsMatch(cdn, @"^https?://[A-Za-z0-9.\-]+(:\d+)?(/\S*)?$"))
            return (ip, 0, 0, null, false, null, Loc.Get("apk.badCdn"), null);
        return (ip, port, reg, PatchHostBox.Text.Trim(), SingleBox.IsChecked == true, OutBox.Text.Trim(), null, cdn);
    }

    private string OutDirOrDefault(string outDir) => string.IsNullOrWhiteSpace(outDir) ? Path.Combine(Svc.Settings.ComobileRoot, "build") : outDir;

    private void UpdateCmd()
    {
        var f = Form();
        if (f.error != null) { ApkCmd.Text = f.error; return; }
        var a = ApkOps.BuildArgs(f.ip, f.port, f.patch, f.single, f.outDir, f.reg, f.cdn);
        ApkCmd.Text = "python " + string.Join(" ", a.Skip(1).Select(x => x.Contains(' ') ? "\"" + x + "\"" : x)).Replace(Svc.Settings.ComobileRoot + "\\", "");
        UpdateApkFile();
    }

    private void UpdateApkFile()
    {
        var f = Form();
        string dir = OutDirOrDefault(f.outDir);
        var (single, splits) = ApkOps.Outputs(dir);
        ApkFileText.Text = single != null ? $"{single}   ({Fmt.Size(new FileInfo(single).Length)}, {File.GetLastWriteTime(single):yyyy-MM-dd HH:mm})"
                         : splits.Count > 0 ? Loc.FB("apk.splits", splits.Count, dir)
                         : Loc.Get("apk.noFile");
    }

    private void CdnOurs_Click(object sender, RoutedEventArgs e)
    {
        string ip = IpBox.Text.Trim().Length > 0 ? IpBox.Text.Trim() : Svc.Config()?.ServerIp ?? "127.0.0.1";
        CdnBox.Text = $"http://{ip}:{Svc.Settings.CdnPort}/ydzf/android/zf";
    }

    private void OutBrowse_Click(object sender, RoutedEventArgs e)
    {
        var d = new Microsoft.Win32.OpenFolderDialog { Title = Loc.Get("apk.out") };
        if (d.ShowDialog() == true) OutBox.Text = d.FolderName;
    }

    private void OpenOut_Click(object sender, RoutedEventArgs e) => Ui.OpenFolder(OutDirOrDefault(Form().outDir));
    private void ApkOutPath_Click(object sender, MouseButtonEventArgs e) { if (_lastApkOut != null) Ui.OpenFolder(_lastApkOut); }

    private void ApkBanner_(string style, string glyph, string text)
    {
        ApkBanner.Style = (Style)FindResource(style);
        ApkGlyph.Text = glyph;
        ApkGlyph.Foreground = style switch { "OkBanner" => Brushes2.Ok, "ErrBanner" => Brushes2.Err, "WarnBanner" => Brushes2.Warn, _ => Brushes2.Info };
        ApkStatus.Text = text;
    }

    private async void Apk_Click(object sender, RoutedEventArgs e)
    {
        var f = Form();
        if (f.error != null) { ApkBanner_("ErrBanner", "", f.error); return; }
        if (!File.Exists(Svc.Settings.BuildApkPy)) { ApkBanner_("ErrBanner", "", Loc.FB("apk.noScript", Svc.Settings.BuildApkPy)); return; }
        string outDir = OutDirOrDefault(f.outDir);
        var steps = new List<string>
        {
            Loc.FB("apk.step.server", f.ip + ":" + f.port),
            string.IsNullOrEmpty(f.patch) ? Loc.FB("apk.step.patchSame", f.ip) : f.patch == "127.0.0.1" ? Loc.Get("apk.step.patchNone") : Loc.FB("apk.step.patch", f.patch),
            f.reg > 0 ? Loc.FB("apk.step.reg", f.reg) : Loc.Get("apk.step.regOff"),
            string.IsNullOrEmpty(f.cdn) ? Loc.Get("apk.step.cdnOfficial") : Loc.FB("apk.step.cdn", f.cdn),
            Loc.FB("apk.step.out", outDir) + (f.single ? " · Qahr_single.apk" : ""),
        };
        if (!ConfirmDialog.Ask(Loc.Get("apk.confirmTitle"), Loc.Get("apk.confirmMsg"), steps, Loc.Get("apk.build"),
                note: Directory.Exists(outDir) && ApkOps.Outputs(outDir).splits.Count > 0 ? Loc.Get("apk.overwrite") : null))
            return;
        var s = Svc.Settings;
        s.ApkIp = f.ip; s.ApkPort = f.port; s.ApkRegisterPort = f.reg; s.ApkPatchHost = f.patch; s.ApkSingle = f.single; s.ApkOut = f.outDir; s.ApkCdnUrl = f.cdn;
        Svc.SaveSettings();

        ApkOutput.Clear();
        ApkProgress.Visibility = Visibility.Visible;
        ApkProgress.Value = 2;
        ApkOutPath.Visibility = Visibility.Collapsed;
        ApkBanner_("InfoBanner", "", Loc.Get("apk.building"));
        SetBusy(true);
        _cts = new CancellationTokenSource();
        var sw = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            if (!string.IsNullOrWhiteSpace(f.outDir)) Directory.CreateDirectory(f.outDir);
            var args = ApkOps.BuildArgs(f.ip, f.port, f.patch, f.single, f.outDir, f.reg, f.cdn);
            var r = await Proc.RunAsync(s.PythonExe, args, s.ComobileRoot, l => Dispatcher.BeginInvoke(() =>
            {
                Ui.Append(ApkOutput, l);
                foreach (var (mark, pct) in ApkSteps)
                    if (l.Contains(mark, StringComparison.OrdinalIgnoreCase) && ApkProgress.Value < pct) ApkProgress.Value = pct;
            }), _cts.Token);
            ApkProgress.Value = 100;
            var (single, splits) = ApkOps.Outputs(outDir);
            if (r.Ok && (single != null || splits.Count > 0))
            {
                _lastApkOut = single ?? splits[0];
                ApkBanner_("OkBanner", "", Loc.F("apk.done", sw.Elapsed.TotalSeconds.ToString("0", Loc.Culture)));
                ApkOutPath.Text = single ?? outDir;
                ApkOutPath.Visibility = Visibility.Visible;
            }
            else if (r.Cancelled) ApkBanner_("WarnBanner", "", Loc.Get("build.cancelled"));
            else ApkBanner_("ErrBanner", "", Loc.F("apk.failed", MirrorOps.LastLines(r.Output, 1)));
            ActionLog.Write("build-apk", $"ip={f.ip}:{f.port} patch={f.patch} cdn={f.cdn} single={f.single} out={outDir} code={r.Code}");
        }
        finally
        {
            SetBusy(false);
            ApkProgress.Visibility = Visibility.Collapsed;
            UpdateApkFile();
        }
    }

    private async Task LoadDevicesAsync()
    {
        var list = await ApkOps.DevicesAsync();
        var prev = (DeviceBox.SelectedItem as ApkOps.Device)?.Serial;
        DeviceBox.ItemsSource = list;
        DeviceBox.SelectedItem = list.FirstOrDefault(d => d.Serial == prev) ?? list.FirstOrDefault(d => d.Ready);
        DeviceHint.Text = list.Count == 0 ? Loc.Get("apk.noDevices") : Loc.F("apk.devices", list.Count(d => d.Ready));
    }

    private async void Devices_Click(object sender, RoutedEventArgs e) => await LoadDevicesAsync();

    private async void Install_Click(object sender, RoutedEventArgs e)
    {
        if (DeviceBox.SelectedItem is not ApkOps.Device d || !d.Ready) { ApkBanner_("WarnBanner", "", Loc.Get("apk.pickDevice")); return; }
        string dir = OutDirOrDefault(Form().outDir);
        var (single, splits) = ApkOps.Outputs(dir);
        if (single == null && splits.Count == 0) { ApkBanner_("WarnBanner", "", Loc.Get("apk.noFile")); return; }
        if (!ConfirmDialog.Ask(Loc.Get("apk.installTitle"), Loc.F("apk.installMsg", d.Label),
                new[] { single != null ? Loc.FB("apk.step.installSingle", single) : Loc.FB("apk.step.installSplits", splits.Count, dir), Loc.Get("apk.step.keepData") },
                Loc.Get("apk.install"), note: Loc.Get("apk.installNote")))
            return;
        ApkOutput.Clear();
        ApkBanner_("InfoBanner", "", Loc.F("apk.installing", d.Label));
        SetBusy(true);
        _cts = new CancellationTokenSource();
        try
        {
            var r = await ApkOps.InstallAsync(d.Serial, single, splits, Ui.Sink(ApkOutput), _cts.Token);
            bool ok = r.Ok && r.Output.Contains("Success");
            ApkBanner_(ok ? "OkBanner" : "ErrBanner", ok ? "" : "", ok ? Loc.F("apk.installed", d.Label) : Loc.F("apk.installFailed", MirrorOps.LastLines(r.Output, 1)));
            ActionLog.Write("adb-install", $"{d.Serial} ok={ok}");
        }
        finally { SetBusy(false); }
    }
}
