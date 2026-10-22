using System.IO;
using System.Windows;
using System.Windows.Controls;
using MobileStudio.Core;
using ScriptHazem.Kit;

namespace MobileStudio.Views;

/// <summary>The mobile tools in one place: studios (Tools\*\publish\*.exe, found by themselves), PatchConsole, the client
/// source project (build.py) and the engine sources / Ghidra project.</summary>
public partial class ToolsView : UserControl, IPage
{
    private CancellationTokenSource _cts;
    private bool _busy;

    public ToolsView()
    {
        InitializeComponent();
        BannerText.Text = Loc.Get("tools.idle");
    }

    public void OnShown() => Reload();
    public void OnHidden() { }
    public void OnTick(int tick) { }
    public void OnStatus(ServerSnapshot s) { }
    public void OnLanguage() { Reload(); if (!_busy) BannerText.Text = Loc.Get("tools.idle"); }
    public Task PrepareScreenshotAsync() { Reload(); return Task.CompletedTask; }

    private static AppSettings S => Svc.Settings;
    private string ClientDir => S.ClientSourceDir;
    private string BuildPy => Path.Combine(ClientDir, "build.py");

    private void Reload()
    {
        var list = ToolCatalog.Load();
        Cards.ItemsSource = list;
        SubText.Text = Loc.FB("tools.sub", list.Count(t => t.Exists), list.Count, S.ToolsRoot);
        ClientPath.Text = ClientDir;
        EnginePath.Text = S.EngineSourceDir;
        bool hasBuild = File.Exists(BuildPy);
        CheckBtn.IsEnabled = BuildBtn.IsEnabled = ZipBtn.IsEnabled = hasBuild && !_busy;
        string outDir = Path.Combine(ClientDir, "out");
        var last = Directory.Exists(outDir) ? Directory.GetFiles(outDir, "*.apk").Select(f => new FileInfo(f)).OrderByDescending(f => f.LastWriteTime).FirstOrDefault() : null;
        ZipHint.Text = !hasBuild ? Loc.FB("tools.noBuild", BuildPy)
            : last != null ? Loc.FB("tools.lastApk", last.Name, last.LastWriteTime.ToString("yyyy-MM-dd HH:mm"))
            : Loc.Get("tools.zipHint");
        EngineReadmeBtn.IsEnabled = File.Exists(Path.Combine(S.EngineSourceDir, "README_AR.md"));
        GhidraBtn.IsEnabled = File.Exists(S.GhidraRun) && File.Exists(S.GhidraProject);
        GhidraHint.Text = GhidraBtn.IsEnabled ? Loc.FB("tools.ghidraHint", S.GhidraProject) : Loc.FB("tools.noGhidra", S.GhidraRun);
    }

    private void Refresh_Click(object sender, RoutedEventArgs e) => Reload();
    private void Home_Click(object sender, RoutedEventArgs e) => Ui.OpenFolder(AppSettings.Home);

    private void Open_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.Tag is ToolEntry t && !ToolCatalog.Launch(t)) MainWindow.Toast_(Loc.FB("tools.cantOpen", t.Exe), true);
    }

    private void Folder_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.Tag is ToolEntry t) Ui.OpenFolder(t.Exists ? t.Exe : t.Folder);
    }

    private void ClientFolder_Click(object sender, RoutedEventArgs e) => Ui.OpenFolder(ClientDir);
    private void ClientOut_Click(object sender, RoutedEventArgs e) => Ui.OpenFolder(Path.Combine(ClientDir, "out"));
    private void ClientReadme_Click(object sender, RoutedEventArgs e) => Ui.OpenFile(Path.Combine(ClientDir, "README_AR.md"));
    private void EngineFolder_Click(object sender, RoutedEventArgs e) => Ui.OpenFolder(S.EngineSourceDir);
    private void EngineReadme_Click(object sender, RoutedEventArgs e) => Ui.OpenFile(Path.Combine(S.EngineSourceDir, "README_AR.md"));

    private void Ghidra_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var psi = new System.Diagnostics.ProcessStartInfo(S.GhidraRun) { UseShellExecute = false, CreateNoWindow = true, WorkingDirectory = Path.GetDirectoryName(S.GhidraRun) };
            psi.ArgumentList.Add(S.GhidraProject);
            System.Diagnostics.Process.Start(psi);
            ActionLog.Write("open-ghidra", S.GhidraProject);
            MainWindow.Toast_(Loc.Get("tools.ghidraOpening"));
        }
        catch (Exception ex) { MainWindow.Toast_(ex.Message, true); }
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) => _cts?.Cancel();

    private void SetBanner(string style, string glyph, string text)
    {
        Banner.Style = (Style)FindResource(style);
        BannerGlyph.Text = glyph;
        BannerGlyph.Foreground = style switch { "OkBanner" => Brushes2.Ok, "ErrBanner" => Brushes2.Err, "WarnBanner" => Brushes2.Warn, _ => Brushes2.Info };
        BannerText.Text = text;
    }

    /// <summary>build.py in the client source folder, output streamed to the console.</summary>
    private async Task RunBuild(string what, params string[] extra)
    {
        _busy = true;
        CheckBtn.IsEnabled = BuildBtn.IsEnabled = ZipBtn.IsEnabled = false;
        CancelBtn.IsEnabled = true;
        MainWindow.BusyJobs++;
        OutputBox.Clear();
        var args = new List<string> { "-I", BuildPy };
        args.AddRange(extra);
        Ui.Append(OutputBox, "> python " + string.Join(" ", args.Skip(1).Select(a => a.Contains(' ') ? "\"" + a + "\"" : a)));
        SetBanner("InfoBanner", "", what);
        _cts = new CancellationTokenSource();
        var sw = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            var r = await Proc.RunAsync(S.PythonExe, args, ClientDir, Ui.Sink(OutputBox), _cts.Token);
            if (r.Cancelled) SetBanner("WarnBanner", "", Loc.Get("build.cancelled"));
            else if (r.Ok) SetBanner("OkBanner", "", Loc.F("tools.done", sw.Elapsed.TotalSeconds.ToString("0", Loc.Culture)));
            else SetBanner("ErrBanner", "", Loc.F("tools.failed", r.Code));
            ActionLog.Write("client-build", $"{string.Join(" ", extra)} code={r.Code}");
        }
        finally
        {
            _busy = false;
            CancelBtn.IsEnabled = false;
            MainWindow.BusyJobs--;
            Reload();
        }
    }

    private async void ClientCheck_Click(object sender, RoutedEventArgs e) => await RunBuild(Loc.Get("tools.checking"), "--check");

    private async void ClientBuild_Click(object sender, RoutedEventArgs e)
    {
        if (!ConfirmDialog.Ask(Loc.Get("tools.buildTitle"), Loc.Get("tools.buildMsg"),
                new[] { Loc.FB("tools.step.build", "build.py"), Loc.FB("tools.step.out", Path.Combine(ClientDir, "out")), Loc.Get("tools.step.noServer") },
                Loc.Get("tools.build")))
            return;
        await RunBuild(Loc.Get("tools.building"));
    }

    private async void ClientZip_Click(object sender, RoutedEventArgs e)
    {
        string v = ZipVersion.Text.Trim();
        if (!int.TryParse(v, out int n) || n <= 5150 || n >= 9999) { SetBanner("ErrBanner", "", Loc.Get("upd.badVersion")); return; }
        if (!ConfirmDialog.Ask(Loc.F("tools.zipTitle", n), Loc.Get("tools.zipMsg"),
                new[] { Loc.FB("tools.step.build", "build.py --patch-zip " + n), Loc.FB("tools.step.zip", Path.Combine(ClientDir, "out", n + ".zip")), Loc.Get("tools.step.publishLater") },
                Loc.Get("tools.zip")))
            return;
        await RunBuild(Loc.F("tools.zipping", n), "--patch-zip", n.ToString());
    }
}
