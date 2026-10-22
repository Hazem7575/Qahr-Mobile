using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MobileStudio.Core;
using ScriptHazem.Kit;

namespace MobileStudio.Views;

public partial class DashboardView : UserControl, IPage
{
    public sealed class Service
    {
        public string Name { get; set; }
        public string Detail { get; set; }
        public string State { get; set; }
        public string Glyph { get; set; }
        public Brush Dot { get; set; }
    }

    public sealed class ErrRow
    {
        public string Source { get; set; }
        public string Text { get; set; }
        public Brush Brush { get; set; }
    }

    public sealed class VerRow
    {
        public string Version { get; set; }
        public string CountText { get; set; }
        public double BarWidth { get; set; }
        public Brush Bar { get; set; }
    }

    private readonly FileTail _watch, _err, _acc;
    private readonly List<ErrRow> _errors = new();
    private DateTime _versionsAt = DateTime.MinValue;

    public DashboardView()
    {
        InitializeComponent();
        var s = Svc.Settings;
        _watch = new FileTail(s.WatchLog);
        _err = new FileTail(s.WatchErrLog);
        _acc = new FileTail(s.AccLog);
    }

    public void OnShown() { ReadErrors(); _ = LoadVersionsAsync(); QuickTools.ItemsSource = ToolCatalog.Load().Where(t => t.Top).ToList(); }

    private void Quick_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.Tag is ToolEntry t && !ToolCatalog.Launch(t)) MainWindow.Toast_(Loc.FB("tools.cantOpen", t.Exe), true);
    }
    public void OnHidden() { }
    public void OnTick(int tick)
    {
        if (tick % 5 == 0) ReadErrors();
        if ((DateTime.Now - _versionsAt).TotalSeconds > 60) _ = LoadVersionsAsync();
    }
    public void OnLanguage() { OnStatus(Svc.Monitor.Last); _ = LoadVersionsAsync(); }
    public async Task PrepareScreenshotAsync() { ReadErrors(); await LoadVersionsAsync(); }

    private async void Refresh_Click(object sender, RoutedEventArgs e)
    {
        await MainWindow.Instance.Poll();
        ReadErrors();
        await LoadVersionsAsync();
    }

    private void Players_Click(object sender, MouseButtonEventArgs e) => MainWindow.Instance.Navigate("Players");
    private void Logs_Click(object sender, RoutedEventArgs e) => MainWindow.Instance.Navigate("Logs");

    public void OnStatus(ServerSnapshot s)
    {
        if (s == null) return;
        string Up(bool b) => Loc.Get(b ? "svc.up" : "svc.down");
        Brush Dot(bool b) => b ? Brushes2.Ok : Brushes2.Err;
        Services.ItemsSource = new List<Service>
        {
            new() { Name = "GameServer", Glyph = "", Dot = Brushes2.For(s.GameHealth),
                    State = Loc.Get(s.GameRunning ? (s.GamePortUp ? "svc.up" : "svc.starting") : "svc.down"),
                    Detail = Loc.FB("svc.port", s.GamePort) + (s.GameRunning ? " · " + Loc.Get(s.GameUnderWatch ? "svc.hotReload" : "svc.noWatch") : "") },
            new() { Name = "AccServer", Glyph = "", Dot = Brushes2.For(s.AccHealth),
                    State = Loc.Get(s.AccRunning ? (s.AccPortUp ? "svc.up" : "svc.starting") : "svc.down"),
                    Detail = Loc.FB("svc.port", s.AccPort) + " · " + Loc.Get("svc.login") },
            new() { Name = Loc.Get("svc.check"), Glyph = "", Dot = Dot(s.CheckUp), State = Up(s.CheckUp),
                    Detail = Loc.FB("svc.port", s.CheckPort) + " · " + Loc.Ltr(s.PatchMode) },
            new() { Name = Loc.Get("svc.http"), Glyph = "", Dot = Dot(s.HttpUp), State = Up(s.HttpUp), Detail = Loc.FB("svc.port", s.HttpPort) },
            new() { Name = Loc.Get("svc.register"), Glyph = "", Dot = s.RegisterPort > 0 ? Dot(s.RegisterUp) : Brushes2.Faint,
                    State = s.RegisterPort > 0 ? Up(s.RegisterUp) : Loc.Get("svc.off"), Detail = Loc.FB("svc.port", s.RegisterPort) },
            new() { Name = "MySQL", Glyph = "", Dot = Dot(s.MysqlUp), State = Up(s.MysqlUp),
                    Detail = Loc.FB("svc.port", s.MysqlPort) + " · " + Loc.Ltr(BackupOps.DbName) },
            new() { Name = Loc.Get("svc.cdn"), Glyph = "", Dot = Dot(s.CdnUp), State = Up(s.CdnUp),
                    Detail = Loc.FB("svc.port", s.CdnPort) + " · " + Loc.Get("svc.cdnWhat") },
            new() { Name = "AdminApi", Glyph = "", Dot = s.GameRunning ? Dot(s.AdminUp) : Brushes2.Faint, State = Up(s.AdminUp),
                    Detail = Loc.FB("svc.port", s.AdminPort) + " · 127.0.0.1" },
        };

        if (s.Status != null)
        {
            OnlineText.Text = s.Online.ToString();
            OnlineSub.Text = Loc.F("dash.onlineSub", AdminClient.Int(s.Status, "dbPlayers"), AdminClient.Int(s.Status, "maxLevel"));
            UptimeText.Text = s.GameUptime is { } u ? Fmt.Duration(u) : "—";
            RamText.Text = AdminClient.Int(s.Status, "ramMb") + " MB";
            TickText.Text = (s.Status["tickAvgMs"]?.ToString() ?? "?") + " ms";
            long failed = AdminClient.Long(s.Status, "dbFailed");
            DbText.Text = failed == 0 ? Loc.Get("dash.dbOk") : Loc.F("dash.dbFailed", failed);
            DbText.Foreground = failed == 0 ? Brushes2.Ok : Brushes2.Err;
        }
        else
        {
            OnlineText.Text = "—";
            OnlineSub.Text = s.GameRunning ? (s.StatusError ?? Loc.Get("api.down")) : Loc.Get("dash.serverOff");
            UptimeText.Text = s.GameUptime is { } u ? Fmt.Duration(u) : "—";
            RamText.Text = s.GameRam > 0 ? Fmt.Size(s.GameRam) : "—";
            TickText.Text = "—";
            DbText.Text = "—";
            DbText.Foreground = Brushes2.Text;
        }

        var hr = s.HotReload;
        HrDot.Fill = Brushes2.For(hr.Health);
        HrState.Text = Loc.Get(hr.Key);
        HrState.Foreground = Brushes2.For(hr.Health);
        HrMode.Text = !s.GameRunning ? Loc.Get("dash.hrServerOff")
                    : s.GameUnderWatch ? (hr.Health == Health.Bad ? Loc.Get("dash.hrNeedsRestart") : Loc.Get("dash.hrOn"))
                    : Loc.Get("dash.hrOff");
        HrFile.Text = hr.LastFile;
        HrLine.Text = hr.LastLine;
        HrCard.BorderBrush = hr.Health == Health.Bad && s.GameRunning ? (Brush)FindResource("ErrEdgeBrush") : (Brush)FindResource("LineBrush");
    }

    private void ReadErrors()
    {
        void Take(FileTail t, string src)
        {
            var lines = LogClassifier.Lines(t.Read(512 * 1024));
            foreach (var l in lines.Where(l => l.Level == LogLevel.Error && !l.Text.StartsWith("   at ")))
                _errors.Add(new ErrRow { Source = src, Text = l.Text.Trim(), Brush = l.Brush });
        }
        Take(_watch, "watch.log");
        Take(_err, "watch.err");
        Take(_acc, "acc.log");
        if (_errors.Count > 60) _errors.RemoveRange(0, _errors.Count - 60);
        if (ErrorList.SelectedItems.Count > 0) return;   // the user is selecting: do not reset the list
        var view = _errors.AsEnumerable().Reverse().Take(14).ToList();
        ErrorList.ItemsSource = view;
        NoErrors.Visibility = view.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        ErrorList.Visibility = view.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
    }

    private async Task LoadVersionsAsync()
    {
        _versionsAt = DateTime.Now;
        var cfg = Svc.Config();
        string dir = cfg?.PatchDir;
        if (dir == null) return;
        var data = await Task.Run(() =>
        {
            var list = PatchConsole.PatchCore.List(dir);
            int current = PatchConsole.PatchCore.CurrentVersion(list);
            var man = PatchConsole.OfficialManifest.Read(dir);
            var ev = PatchConsole.Stats.Read(dir, false);
            var since = DateTime.Now.AddDays(-14);
            // the newest version each device asked with (the 7123 APK-number check is not a resource version)
            var last = ev.Where(e => e.Kind == "check" && e.Time >= since && e.Version != "7123")
                         .GroupBy(e => e.Device).Select(g => g.OrderBy(e => e.Time).Last())
                         .Select(e => e.Result == "READY" ? e.Version : e.Version)
                         .GroupBy(v => v).Select(g => (v: g.Key, n: g.Count())).OrderByDescending(x => x.v).ToList();
            return (current, official: man?.LatestOfficial, last);
        });
        PublishedText.Text = data.current.ToString();
        OfficialText.Text = string.IsNullOrEmpty(data.official) ? "—" : data.official;
        int max = Math.Max(1, data.last.Select(x => x.n).DefaultIfEmpty(1).Max());
        VersionList.ItemsSource = data.last.Take(6).Select(x => new VerRow
        {
            Version = x.v,
            CountText = Loc.F("dash.devCount", x.n),
            BarWidth = Math.Max(6, 160.0 * x.n / max),
            Bar = x.v == data.current.ToString() ? Brushes2.Ok : Brushes2.Warn,
        }).ToList();
        VersionHint.Text = data.last.Count == 0 ? Loc.Get("dash.noDevices") : Loc.Get("dash.devicesHint");
    }
}
