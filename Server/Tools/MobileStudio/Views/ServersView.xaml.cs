using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using MobileStudio.Core;
using ScriptHazem.Kit;

namespace MobileStudio.Views;

public partial class ServersView : UserControl, IPage
{
    private readonly ObservableCollection<LogLine> _gLines = new(), _aLines = new(), _aBuild = new();
    private FileTail _gTail, _aTail;
    private LogLevel _gLast, _aLast;
    private bool _gBusy, _aBusy;
    private CancellationTokenSource _cts;

    public ServersView()
    {
        InitializeComponent();
        GConsole.ItemsSource = _gLines;
        AConsole.ItemsSource = _aLines;
        _gTail = new FileTail(Svc.Settings.WatchLog);
        _aTail = new FileTail(Svc.Settings.AccLog);
        GPath.Text = Svc.Settings.GameExe;
        APath.Text = Svc.Settings.AccExe;
    }

    public void OnShown() => ReadConsoles();
    public void OnHidden() { }
    public void OnTick(int tick) => ReadConsoles();
    public void OnLanguage() => OnStatus(Svc.Monitor.Last);
    public Task PrepareScreenshotAsync() { ReadConsoles(); return Task.CompletedTask; }

    // ------------------------------------------------------------------ consoles
    private void GSrc_Changed(object sender, RoutedEventArgs e)
    {
        if (GSrcErr == null || GConsole == null || _gTail == null) return;
        _gTail = new FileTail(GSrcErr.IsChecked == true ? Svc.Settings.WatchErrLog : Svc.Settings.WatchLog);
        _gLines.Clear();
        ReadConsoles();
    }

    private void ASrc_Changed(object sender, RoutedEventArgs e)
    {
        if (ASrcBuild == null || AConsole == null) return;
        AConsole.ItemsSource = ASrcBuild.IsChecked == true ? _aBuild : _aLines;
        Scroll(AConsole);
    }

    private void ReadConsoles()
    {
        Add(_gLines, _gTail.Read(128 * 1024), ref _gLast, GConsole);
        Add(_aLines, _aTail.Read(128 * 1024), ref _aLast, AConsole);
    }

    private static void Add(ObservableCollection<LogLine> col, List<string> lines, ref LogLevel last, ListBox box)
    {
        if (lines.Count == 0) return;
        if (lines.Count > 1500) lines = lines.GetRange(lines.Count - 1500, 1500);
        foreach (var t in lines)
        {
            last = LogClassifier.Classify(t, last);
            col.Add(new LogLine { Text = t, Level = last });
        }
        while (col.Count > 2000) col.RemoveAt(0);
        Scroll(box);
    }

    private static void Scroll(ListBox box)
    {
        if (box.Items.Count > 0 && !CopySupport.UserIsReading(box)) box.ScrollIntoView(box.Items[^1]);
    }

    private void BuildLine(string l) => Dispatcher.BeginInvoke(() =>
    {
        var lv = LogClassifier.Classify(l, LogLevel.Info);
        _aBuild.Add(new LogLine { Text = l, Level = lv });
        while (_aBuild.Count > 3000) _aBuild.RemoveAt(0);
        if (ASrcBuild.IsChecked == true) Scroll(AConsole);
    });

    // ------------------------------------------------------------------ status
    public void OnStatus(ServerSnapshot s)
    {
        if (s == null) return;
        GDot.Fill = Brushes2.For(s.GameHealth);
        GState.Text = Loc.Get(s.GameRunning ? (s.GamePortUp ? "svc.up" : "svc.starting") : "svc.down");
        GState.Foreground = Brushes2.For(s.GameHealth);
        GPid.Text = s.GameRunning ? s.GamePid.ToString() : "—";
        GUptime.Text = s.GameRunning && s.GameUptime is { } u ? Fmt.Duration(u) : "—";
        GOnline.Text = s.Online >= 0 ? s.Online.ToString() : "—";
        GHr.Text = !s.GameRunning ? "—" : Loc.Get(s.GameUnderWatch ? "srv.hrOn" : "srv.hrOff");
        GHr.Foreground = !s.GameRunning ? Brushes2.Text : s.GameUnderWatch ? (s.HotReload.Health == Health.Bad ? Brushes2.Err : Brushes2.Ok) : Brushes2.Warn;
        GNoteText.Text = !s.GameRunning ? Loc.Get("srv.gNoteOff")
            : s.GameUnderWatch ? (s.HotReload.Health == Health.Bad ? Loc.F("srv.gNoteRestart", Loc.Get(s.HotReload.Key)) : Loc.Get("srv.gNoteWatch"))
            : Loc.Get("srv.gNoteNoWatch");
        GNote.Style = (Style)FindResource(s.GameRunning && s.GameUnderWatch && s.HotReload.Health == Health.Bad ? "WarnBanner" : "InfoBanner");

        ADot.Fill = Brushes2.For(s.AccHealth);
        AState.Text = Loc.Get(s.AccRunning ? (s.AccPortUp ? "svc.up" : "svc.starting") : "svc.down");
        AState.Foreground = Brushes2.For(s.AccHealth);
        APid.Text = s.AccRunning ? s.AccPid.ToString() : "—";
        AUptime.Text = s.AccRunning && s.AccUptime is { } a ? Fmt.Duration(a) : "—";
        ARam.Text = s.AccRam > 0 ? Fmt.Size(s.AccRam) : "—";
        APorts.Text = $"{s.AccPort} · {s.CheckPort} · {s.HttpPort} · {s.RegisterPort}";

        CDot.Fill = s.CdnUp ? Brushes2.Ok : Brushes2.Err;
        CState.Text = Loc.Get(s.CdnUp ? "svc.up" : "svc.down");
        CState.Foreground = CDot.Fill;
        CInfo.Text = Loc.FB("srv.cdnInfo", s.CdnPort, $"http://{Svc.Config()?.ServerIp ?? "IP"}:{s.CdnPort}/ydzf/android/zf");
        CStart.IsEnabled = !_cBusy && !s.CdnUp;
        CStop.IsEnabled = !_cBusy && s.CdnUp;

        GStart.IsEnabled = !_gBusy && !s.GameRunning;
        GStop.IsEnabled = !_gBusy && s.GameRunning;
        GRestart.IsEnabled = !_gBusy;
        GSave.IsEnabled = !_gBusy && s.AdminUp;
        AStart.IsEnabled = !_aBusy && !s.AccRunning;
        AStop.IsEnabled = !_aBusy && s.AccRunning;
        ARestart.IsEnabled = !_aBusy && s.AccRunning;
        ARebuild.IsEnabled = !_aBusy;
    }

    /// <summary>The texts of the stop confirmation (also used for the documentation screenshot).</summary>
    public static (string title, string msg, List<string> steps, string note) StopGameTexts(ServerSnapshot s)
    {
        var (game, watch) = ServerOps.GameTargets();
        var steps = new List<string>
        {
            s.AdminUp ? Loc.F("srv.step.save", s.Online >= 0 ? s.Online : 0) : Loc.Get("srv.step.saveNoApi"),
            watch.Count > 0 ? Loc.FB("srv.step.killWatch", string.Join(", ", watch)) : Loc.Get("srv.step.noWatch"),
            Loc.FB("srv.step.killGame", game > 0 ? game.ToString() : "—"),
            Loc.Get("srv.step.waitPort"),
        };
        return (Loc.Get("srv.stopTitle"), Loc.Get("srv.stopMsg"), steps, s.Online > 0 ? Loc.F("srv.stopNotePlayers", s.Online) : null);
    }

    // ------------------------------------------------------------------ GameServer actions
    private async Task RunGame(Func<Action<string>, Task<bool>> job)
    {
        _gBusy = true;
        MainWindow.BusyJobs++;
        OnStatus(Svc.Monitor.Last);
        void Step(string t) => Dispatcher.BeginInvoke(() => { GProgress.Text = t; _gLines.Add(new LogLine { Text = "» " + t, Level = LogLevel.Warning }); Scroll(GConsole); });
        try
        {
            bool ok = await job(Step);
            MainWindow.Toast_(ok ? Loc.Get("srv.done") : Loc.Get("srv.failed"), !ok);
        }
        catch (Exception ex) { Step(ex.Message); MainWindow.Toast_(ex.Message, true); }
        finally
        {
            _gBusy = false;
            MainWindow.BusyJobs--;
            _gTail.Reset();
            _gLines.Clear();
            await MainWindow.Instance.Poll();
        }
    }

    private async void GStart_Click(object sender, RoutedEventArgs e)
    {
        if (!ConfirmDialog.Ask(Loc.Get("srv.startTitle"), Loc.Get("srv.startMsg"),
                new[] { Loc.Get("srv.step.archive"), Loc.Get("srv.step.watch"), Loc.Get("srv.step.waitListen") }, Loc.Get("srv.start")))
            return;
        await RunGame(ServerOps.StartGameAsync);
    }

    private async void GStop_Click(object sender, RoutedEventArgs e)
    {
        var s = Svc.Monitor.Last;
        var (title, msg, steps, note) = StopGameTexts(s);
        if (!ConfirmDialog.Ask(title, msg, steps, Loc.Get("srv.stopOk"), true, note)) return;
        if (!s.AdminUp && !ConfirmDialog.Ask(Loc.Get("srv.noApiTitle"), Loc.Get("srv.noApiMsg"), null, Loc.Get("srv.stopAnyway"), true)) return;
        await RunGame(step => ServerOps.StopGameAsync(step));
    }

    private async void GRestart_Click(object sender, RoutedEventArgs e)
    {
        var s = Svc.Monitor.Last;
        var steps = new List<string>
        {
            Loc.Get("srv.step.check"),
            s.AdminUp ? Loc.F("srv.step.save", Math.Max(0, s.Online)) : Loc.Get("srv.step.saveNoApi"),
            Loc.Get("srv.step.stopAll"),
            Loc.Get("srv.step.liveBuild"),
            Loc.Get("srv.step.watch"),
        };
        if (!ConfirmDialog.Ask(Loc.Get("srv.restartTitle"), Loc.Get("srv.restartMsg"), steps, Loc.Get("srv.safeRestart"), true,
                s.Online > 0 ? Loc.F("srv.stopNotePlayers", s.Online) : null))
            return;
        _cts = new CancellationTokenSource();
        await RunGame(step => ServerOps.SafeRestartGameAsync(step, l => Dispatcher.BeginInvoke(() =>
        {
            _gLines.Add(new LogLine { Text = l, Level = LogClassifier.Classify(l, LogLevel.Info) });
            Scroll(GConsole);
        }), _cts.Token));
    }

    private async void GSave_Click(object sender, RoutedEventArgs e)
    {
        GSave.IsEnabled = false;
        var (ok, msg) = await ServerOps.SaveAllAsync();
        ActionLog.Write("save-all", msg);
        GProgress.Text = ok ? Loc.F("ops.saved", msg) : Loc.F("ops.saveFailed", msg);
        MainWindow.Toast_(GProgress.Text, !ok);
        GSave.IsEnabled = true;
    }

    // ------------------------------------------------------------------ CDN mirror
    private bool _cBusy;

    private async Task RunCdn(bool stop)
    {
        _cBusy = true;
        OnStatus(Svc.Monitor.Last);
        try
        {
            bool ok = await ServerOps.CdnAsync(stop, t => Dispatcher.BeginInvoke(() => CInfo.Text = t));
            MainWindow.Toast_(ok ? Loc.Get("srv.done") : Loc.Get("srv.failed"), !ok);
        }
        catch (Exception ex) { MainWindow.Toast_(ex.Message, true); }
        finally { _cBusy = false; await MainWindow.Instance.Poll(); }
    }

    private async void CStart_Click(object sender, RoutedEventArgs e)
    {
        if (!ConfirmDialog.Ask(Loc.Get("srv.cdnStartTitle"), Loc.Get("srv.cdnStartMsg"),
                new[] { Loc.FB("srv.step.cdnRun", "run_cdn.ps1", Svc.Settings.CdnPort), Loc.Get("srv.step.cdnLogs") }, Loc.Get("srv.start")))
            return;
        await RunCdn(false);
    }

    private async void CStop_Click(object sender, RoutedEventArgs e)
    {
        if (!ConfirmDialog.Ask(Loc.Get("srv.cdnStopTitle"), Loc.Get("srv.cdnStopMsg"),
                new[] { Loc.FB("srv.step.cdnStop", "run_cdn.ps1 -Stop") }, Loc.Get("srv.stopOk"), true))
            return;
        await RunCdn(true);
    }

    // ------------------------------------------------------------------ AccServer actions
    private async Task RunAcc(Func<Action<string>, Task<bool>> job, bool showBuild = false)
    {
        _aBusy = true;
        MainWindow.BusyJobs++;
        OnStatus(Svc.Monitor.Last);
        if (showBuild) { _aBuild.Clear(); ASrcBuild.IsChecked = true; }
        void Step(string t) => Dispatcher.BeginInvoke(() => { AProgress.Text = t; BuildLine("» " + t); });
        try
        {
            bool ok = await job(Step);
            MainWindow.Toast_(ok ? Loc.Get("srv.done") : Loc.Get("srv.failed"), !ok);
        }
        catch (Exception ex) { Step(ex.Message); MainWindow.Toast_(ex.Message, true); }
        finally
        {
            _aBusy = false;
            MainWindow.BusyJobs--;
            _aTail.Reset();
            _aLines.Clear();
            await MainWindow.Instance.Poll();
        }
    }

    private async void AStart_Click(object sender, RoutedEventArgs e)
    {
        if (!ConfirmDialog.Ask(Loc.Get("srv.accStartTitle"), Loc.Get("srv.accStartMsg"),
                new[] { Loc.Get("srv.step.archiveAcc"), Loc.FB("srv.step.accExe", Svc.Settings.AccExe), Loc.Get("srv.step.waitListen") }, Loc.Get("srv.start")))
            return;
        await RunAcc(ServerOps.StartAccAsync);
    }

    private async void AStop_Click(object sender, RoutedEventArgs e)
    {
        if (!ConfirmDialog.Ask(Loc.Get("srv.accStopTitle"), Loc.Get("srv.accStopMsg"),
                new[] { Loc.FB("srv.step.killAcc", Svc.Monitor.Last.AccPid), Loc.Get("srv.step.accDown") }, Loc.Get("srv.stopOk"), true))
            return;
        await RunAcc(ServerOps.StopAccAsync);
    }

    private async void ARestart_Click(object sender, RoutedEventArgs e)
    {
        if (!ConfirmDialog.Ask(Loc.Get("srv.accRestartTitle"), Loc.Get("srv.accStopMsg"),
                new[] { Loc.FB("srv.step.killAcc", Svc.Monitor.Last.AccPid), Loc.Get("srv.step.archiveAcc"), Loc.Get("srv.step.waitListen") }, Loc.Get("srv.restart"), true))
            return;
        await RunAcc(async step => await ServerOps.StopAccAsync(step) && await ServerOps.StartAccAsync(step));
    }

    private async void ARebuild_Click(object sender, RoutedEventArgs e)
    {
        var steps = new[] { Loc.Get("srv.step.check"), Loc.Get("srv.step.stopAcc"), Loc.Get("srv.step.accBuild"), Loc.Get("srv.step.accStart") };
        if (!ConfirmDialog.Ask(Loc.Get("srv.rebuildTitle"), Loc.Get("srv.rebuildMsg"), steps, Loc.Get("srv.rebuild"), true)) return;
        _cts = new CancellationTokenSource();
        await RunAcc(step => ServerOps.RebuildAccAsync(step, BuildLine, _cts.Token), true);
    }
}
