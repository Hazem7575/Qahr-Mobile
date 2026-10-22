using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using MobileStudio.Core;
using MobileStudio.Views;
using ScriptHazem.Kit;

namespace MobileStudio;

/// <summary>A page. OnTick runs once a second only while the page is visible and the window is not minimized.</summary>
public interface IPage
{
    void OnShown();
    void OnHidden();
    void OnTick(int tick);
    void OnStatus(ServerSnapshot s);
    void OnLanguage();
    Task PrepareScreenshotAsync();
}

public partial class MainWindow : Window
{
    public static MainWindow Instance { get; private set; }
    public (string Png, string Page)? ScreenshotRequest { get; set; }

    private readonly Dictionary<string, UserControl> _pages = new();
    private IPage _current;
    private string _currentName;
    private readonly DispatcherTimer _timer = new(DispatcherPriority.Background) { Interval = TimeSpan.FromSeconds(1) };
    private int _tick;
    /// <summary>Long jobs (start/stop/build/publish/restore) register here so the window asks before closing.</summary>
    public static int BusyJobs;

    public MainWindow()
    {
        Instance = this;
        InitializeComponent();
        UpdateAboutVersion();
        _timer.Tick += (_, _) => OnTimer();
        StateChanged += (_, _) =>
        {
            Root.Margin = WindowState == WindowState.Maximized ? new Thickness(7) : new Thickness(0);
            MaxButton.Content = WindowState == WindowState.Maximized ? "" : "";
            if (WindowState == WindowState.Minimized) _timer.Stop();
            else if (!_timer.IsEnabled) { _timer.Start(); _ = Poll(); }
        };
        PreviewKeyDown += (_, e) =>
        {
            if (e.Key == Key.Escape && AboutOverlay.Visibility == Visibility.Visible) { About_Close(this, null); e.Handled = true; }
        };
        Svc.Monitor.Updated += s => Dispatcher.BeginInvoke(() => OnSnapshot(s));
        Loaded += async (_, _) =>
        {
            NavDashboard.IsChecked = true;
            await Poll();
            if (ScreenshotRequest is { } req) { await TakeScreenshotAsync(req); return; }
            _timer.Start();
        };
        Loc.LanguageChanged += OnLanguageChanged;
        Closed += (_, _) => Loc.LanguageChanged -= OnLanguageChanged;
        Closing += (_, e) =>
        {
            if (ScreenshotRequest != null) return;
            if (BusyJobs > 0 && !ConfirmDialog.Ask(Loc.Get("close.title"), Loc.Get("close.msg"), null, Loc.Get("close.ok"), true))
                e.Cancel = true;
        };
    }

    // ===================== language =====================
    private void Lang_Click(object sender, RoutedEventArgs e) => Loc.Toggle();

    private void OnLanguageChanged()
    {
        Svc.Settings.Language = Loc.Lang;
        Svc.SaveSettings();
        UpdateAboutVersion();
        OnSnapshot(Svc.Monitor.Last);
        foreach (var p in _pages.Values.OfType<IPage>()) p.OnLanguage();
        ShowToast(Loc.Get("toast.lang"));
    }

    private void UpdateAboutVersion()
    {
        var v = Assembly.GetExecutingAssembly().GetName().Version;
        AboutVersion.Text = Loc.F("about.version", $"{v?.Major}.{v?.Minor}.{v?.Build}");
    }

    // ===================== navigation =====================
    private void Nav_Checked(object sender, RoutedEventArgs e)
    {
        if (sender is RadioButton rb && rb.CommandParameter is string name) ShowPage(name);
    }

    public void Navigate(string name)
    {
        var rb = name switch
        {
            "Servers" => NavServers, "Build" => NavBuild, "Tools" => NavTools, "Updates" => NavUpdates, "Mirror" => NavMirror, "Players" => NavPlayers,
            "Logs" => NavLogs, "Backup" => NavBackup, "Git" => NavGit, "Settings" => NavSettings, _ => NavDashboard,
        };
        if (rb.IsChecked == true) ShowPage(name); else rb.IsChecked = true;
    }

    public T Page<T>(string name) where T : UserControl
    {
        Navigate(name);
        return _pages.GetValueOrDefault(name) as T;
    }

    private void ShowPage(string name)
    {
        if (!_pages.TryGetValue(name, out var page))
        {
            page = name switch
            {
                "Servers" => new ServersView(),
                "Build" => new BuildView(),
                "Tools" => new ToolsView(),
                "Updates" => new UpdatesView(),
                "Mirror" => new MirrorView(),
                "Players" => new PlayersView(),
                "Logs" => new LogsView(),
                "Backup" => new BackupView(),
                "Git" => new GitView(),
                "Settings" => new SettingsView(),
                _ => new DashboardView(),
            };
            _pages[name] = page;
        }
        _current?.OnHidden();
        PageHost.Content = page;
        _current = page as IPage;
        _currentName = name;
        _current?.OnShown();
        _current?.OnStatus(Svc.Monitor.Last);
        Dispatcher.BeginInvoke(DispatcherPriority.Loaded, () => CopySupport.AttachTree(page));
        page.Opacity = 0;
        page.BeginAnimation(OpacityProperty, new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(180)));
    }

    private void Pill_Click(object sender, MouseButtonEventArgs e) => Navigate("Servers");
    private void Players_Click(object sender, MouseButtonEventArgs e) => Navigate("Players");

    // ===================== polling =====================
    private void OnTimer()
    {
        _tick++;
        int every = _currentName is "Dashboard" or "Servers" ? 3 : 10;
        if (_tick % every == 0) _ = Poll();
        _current?.OnTick(_tick);
    }

    public Task Poll() => Svc.Monitor.PollAsync();

    private void OnSnapshot(ServerSnapshot s)
    {
        GameDot.Fill = Brushes2.For(s.GameHealth);
        GamePill.Text = s.GameRunning ? Loc.FB("pill.running", "GameServer", s.GameUptime is { } u ? Fmt.Duration(u) : "") : Loc.FB("pill.stopped", "GameServer");
        AccDot.Fill = Brushes2.For(s.AccHealth);
        AccPill.Text = s.AccRunning ? Loc.FB("pill.running", "AccServer", s.AccUptime is { } a ? Fmt.Duration(a) : "") : Loc.FB("pill.stopped", "AccServer");
        OnlinePill.Text = s.Online >= 0 ? Loc.F("pill.online", s.Online) : Loc.Get("pill.onlineUnknown");
        SideStatus.Text = Loc.Get(s.GameUnderWatch ? "side.hrOn" : s.GameRunning ? "side.hrOff" : "side.gameOff") + "\n" +
                          Loc.F("side.hr", Loc.Get(s.HotReload.Key));
        _current?.OnStatus(s);
    }

    // ===================== window chrome =====================
    private void Min_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
    private void Max_Click(object sender, RoutedEventArgs e) => WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    private void Close_Click(object sender, RoutedEventArgs e) => Close();

    // ===================== About =====================
    private void Logo_Click(object sender, MouseButtonEventArgs e)
    {
        AboutOverlay.Visibility = Visibility.Visible;
        var ease = new BackEase { EasingMode = EasingMode.EaseOut, Amplitude = 0.3 };
        AboutOverlay.BeginAnimation(OpacityProperty, new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200)));
        AboutScale.BeginAnimation(ScaleTransform.ScaleXProperty, new DoubleAnimation(0.94, 1, TimeSpan.FromMilliseconds(300)) { EasingFunction = ease });
        AboutScale.BeginAnimation(ScaleTransform.ScaleYProperty, new DoubleAnimation(0.94, 1, TimeSpan.FromMilliseconds(300)) { EasingFunction = ease });
    }
    private void About_CardClick(object sender, MouseButtonEventArgs e) => e.Handled = true;
    private void About_Close(object sender, RoutedEventArgs e)
    {
        var a = new DoubleAnimation(0, TimeSpan.FromMilliseconds(180));
        a.Completed += (_, _) => AboutOverlay.Visibility = Visibility.Collapsed;
        AboutOverlay.BeginAnimation(OpacityProperty, a);
    }

    private const string ContactUrl = "https://www.facebook.com/ScriptHazem";
    private const string ContactPhone = "01032044960";

    private void Contact_Click(object sender, RoutedEventArgs e)
    {
        try { Process.Start(new ProcessStartInfo(ContactUrl) { UseShellExecute = true }); }
        catch { try { Clipboard.SetText(ContactUrl); } catch { } ShowToast(ContactUrl); }
    }

    private void Phone_Click(object sender, RoutedEventArgs e)
    {
        try { Clipboard.SetText(ContactPhone); } catch { }
        ShowToast(Loc.F("about.phoneCopied", ContactPhone));
    }

    private string _lastToast;

    public void ShowToast(string text, bool error = false)
    {
        _lastToast = text;
        ToastText.Text = text;
        ToastGlyph.Text = error ? "" : "";
        ToastGlyph.Foreground = error ? Brushes2.Err : Brushes2.Gold;
        var ease = new CubicEase { EasingMode = EasingMode.EaseOut };
        var fade = new DoubleAnimationUsingKeyFrames();
        fade.KeyFrames.Add(new EasingDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(220)), ease));
        fade.KeyFrames.Add(new EasingDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(error ? 6000 : 3400))));
        fade.KeyFrames.Add(new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(error ? 6500 : 3900))));
        Toast.IsHitTestVisible = true;
        fade.Completed += (_, _) => { if (ToastText.Text == text) Toast.IsHitTestVisible = false; };
        Toast.BeginAnimation(OpacityProperty, fade);
        ToastShift.BeginAnimation(TranslateTransform.YProperty, new DoubleAnimation(20, 0, TimeSpan.FromMilliseconds(260)) { EasingFunction = ease });
    }

    public static void Toast_(string text, bool error = false) => Instance?.ShowToast(text, error);

    /// <summary>Clicking a toast copies its text (error messages included).</summary>
    private void Toast_Click(object sender, MouseButtonEventArgs e)
    {
        if (string.IsNullOrEmpty(_lastToast) || Toast.Opacity < 0.05) return;
        try { Clipboard.SetDataObject(_lastToast, true); } catch { }
        string t = _lastToast;
        ShowToast(Loc.Get("copy.copied"));
        _lastToast = t;
    }

    // ===================== headless UI snapshot (--screenshot x.png Page) =====================
    private async Task TakeScreenshotAsync((string Png, string Page) req)
    {
        try
        {
            string page = req.Page ?? "Dashboard";
            if (page == "About") Logo_Click(this, null);
            else if (page == "Confirm") { await ConfirmShotAsync(req.Png); return; }
            else
            {
                string sub = null;
                int dot = page.IndexOf('.');
                if (dot > 0) { sub = page[(dot + 1)..]; page = page[..dot]; }
                Navigate(page);
                await Task.Delay(300);
                if (_current is IShotTab t && sub != null) t.SelectTab(sub);
            }
            await Task.Delay(400);
            if (_current != null && page != "About") await _current.PrepareScreenshotAsync();
            await Task.Delay(page == "About" ? 700 : 900);
            UpdateLayout();
            var rtb = new RenderTargetBitmap((int)Root.ActualWidth, (int)Root.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(Root);
            var enc = new PngBitmapEncoder();
            enc.Frames.Add(BitmapFrame.Create(rtb));
            Directory.CreateDirectory(Path.GetDirectoryName(req.Png));
            using (var fs = File.Create(req.Png)) enc.Save(fs);
        }
        catch (Exception ex) { App.LogCrash(ex); }
        finally { Application.Current.Shutdown(); }
    }

    /// <summary>Renders the stop-GameServer confirmation off-screen (nothing is executed).</summary>
    private async Task ConfirmShotAsync(string png)
    {
        var (title, msg, steps, note) = ServersView.StopGameTexts(Svc.Monitor.Last);
        var d = ConfirmDialog.Create(title, msg, steps, Loc.Get("srv.stopOk"), true, note, null, "");
        d.WindowStartupLocation = WindowStartupLocation.Manual;
        d.Left = -20000;
        d.Top = 0;
        d.ShowActivated = false;
        d.Show();
        await Task.Delay(700);
        var root = (FrameworkElement)d.Content;
        d.UpdateLayout();
        var rtb = new RenderTargetBitmap((int)root.ActualWidth + 24, (int)root.ActualHeight + 24, 96, 96, PixelFormats.Pbgra32);
        var host = new DrawingVisual();
        using (var dc = host.RenderOpen())
        {
            dc.DrawRectangle((Brush)FindResource("BgBrush"), null, new Rect(0, 0, rtb.Width, rtb.Height));
            dc.DrawRectangle(new VisualBrush(root), null, new Rect(12, 8, root.ActualWidth, root.ActualHeight));
        }
        rtb.Render(host);
        var enc = new PngBitmapEncoder();
        enc.Frames.Add(BitmapFrame.Create(rtb));
        using (var fs = File.Create(png)) enc.Save(fs);
        d.Close();
    }
}

/// <summary>Pages with tabs let a screenshot pick one ("Updates.History").</summary>
public interface IShotTab
{
    void SelectTab(string name);
}

/// <summary>Helpers shared by the pages.</summary>
public static class Ui
{
    public static void OpenFolder(string path)
    {
        try
        {
            if (File.Exists(path)) Process.Start(new ProcessStartInfo("explorer.exe", $"/select,\"{path}\"") { UseShellExecute = true });
            else if (Directory.Exists(path)) Process.Start(new ProcessStartInfo("explorer.exe", $"\"{path}\"") { UseShellExecute = true });
        }
        catch { }
    }

    public static void OpenFile(string path)
    {
        try { Process.Start(new ProcessStartInfo(path) { UseShellExecute = true }); } catch { }
    }

    /// <summary>Appends a line to a log TextBox (kept short) and scrolls to the end.</summary>
    public static void Append(TextBox box, string line, int maxChars = 400_000)
    {
        bool reading = CopySupport.UserIsReading(box);
        if (box.Text.Length > maxChars && !reading) box.Text = box.Text[^(maxChars / 2)..];
        box.AppendText(line + Environment.NewLine);
        if (!reading) box.ScrollToEnd();   // the user is selecting or scrolled up: keep their place
    }

    /// <summary>Marshals a log line from a worker thread to a TextBox.</summary>
    public static Action<string> Sink(TextBox box) => l => box.Dispatcher.BeginInvoke(() => Append(box, l));
}
