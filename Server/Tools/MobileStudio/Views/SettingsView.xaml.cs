using System.IO;
using System.Windows;
using System.Windows.Controls;
using MobileStudio.Core;
using ScriptHazem.Kit;

namespace MobileStudio.Views;

public partial class SettingsView : UserControl, IPage
{
    private readonly List<(string prop, TextBox box, Ellipse_ dot, bool folder)> _fields = new();

    private sealed class Ellipse_ { public System.Windows.Shapes.Ellipse E; }

    private static readonly (string prop, string key, bool folder)[] Paths =
    {
        ("QahrRoot", "set.p.qahr", true), ("ComobileRoot", "set.p.comobile", true), ("PythonExe", "set.p.python", false),
        ("DotnetExe", "set.p.dotnet", false), ("AdbExe", "set.p.adb", false), ("MysqlBin", "set.p.mysql", true),
        ("RepoDir", "set.p.repo", true), ("BackupDir", "set.p.backup", true), ("MirrorScript", "set.p.mirror", false),
        ("ToolsRoot", "set.p.tools", true), ("ClientSourceDir", "set.p.client", true), ("EngineSourceDir", "set.p.engine", true),
        ("GhidraRun", "set.p.ghidra", false), ("GhidraProject", "set.p.ghidraProj", false),
    };

    public SettingsView()
    {
        InitializeComponent();
        foreach (var (prop, key, folder) in Paths)
        {
            var label = new TextBlock { Style = (Style)FindResource("FieldLabel"), Margin = new Thickness(0, 10, 0, 6) };
            label.SetBinding(TextBlock.TextProperty, new System.Windows.Data.Binding("[" + key + "]") { Source = Loc.I });
            var dot = new System.Windows.Shapes.Ellipse { Width = 7, Height = 7, Margin = new Thickness(10, 0, 2, 0), VerticalAlignment = VerticalAlignment.Center };
            var box = new TextBox { Style = (Style)FindResource("LtrField") };
            box.TextChanged += (_, _) => Check();
            var dp = new DockPanel();
            DockPanel.SetDock(dot, Dock.Right);
            dp.Children.Add(dot);
            dp.Children.Add(box);
            PathPanel.Children.Add(label);
            PathPanel.Children.Add(dp);
            _fields.Add((prop, box, new Ellipse_ { E = dot }, folder));
        }
        PathPanel.Children.Add(new TextBlock { Style = (Style)FindResource("Hint"), Margin = new Thickness(0, 12, 0, 0), Text = Loc.Get("set.pathsHint") });
        Load();
    }

    public void OnShown() { Load(); }
    public void OnHidden() { }
    public void OnTick(int tick) { }
    public void OnStatus(ServerSnapshot s) { }
    public void OnLanguage() => Load();
    public Task PrepareScreenshotAsync() => Task.CompletedTask;

    private void Load()
    {
        var s = Svc.Settings;
        foreach (var f in _fields) f.box.Text = typeof(AppSettings).GetProperty(f.prop)!.GetValue(s) as string ?? "";
        GitName.Text = s.GitAuthorName;
        GitEmail.Text = s.GitAuthorEmail;
        ExDirs.Text = string.Join(", ", s.SyncExcludeDirs);
        ExFiles.Text = string.Join(", ", s.SyncExcludeFiles);
        KeepBox.Text = s.BackupKeep.ToString();
        AppData.Text = AppSettings.AppDataDir;
        Check();

        var cfg = Svc.Config();
        var rows = new List<KeyValuePair<string, string>>
        {
            new(Loc.Get("set.c.file"), s.ConfigIni + (cfg == null ? "  (" + Loc.Get("set.missing") + ")" : "")),
        };
        if (cfg != null)
        {
            rows.Add(new(Loc.Get("set.c.ip"), cfg.ServerIp));
            rows.Add(new(Loc.Get("set.c.ports"), $"Game {cfg.GamePort} · Login {cfg.LoginPort} · Register {cfg.RegisterPort}"));
            rows.Add(new(Loc.Get("set.c.patch"), $"{cfg.Mode} · check {cfg.CheckPort} · http {cfg.HttpPort}"));
            rows.Add(new(Loc.Get("set.c.patchDir"), cfg.PatchDir));
            rows.Add(new(Loc.Get("set.c.db"), $"{cfg.Get("Database", "Host", "localhost")}:{cfg.Get("Database", "Port", "3306")} / {cfg.Get("Database", "Name", "")}"));
        }
        rows.Add(new("AdminApi", AdminClient.Configured ? Loc.F("set.c.apiOn", AdminClient.Port) : Loc.Get("set.c.apiOff")));
        rows.Add(new("official_mirror.py", s.MirrorPy));
        ConfigList.ItemsSource = rows;
    }

    private void Check()
    {
        foreach (var f in _fields)
        {
            string v = f.box.Text.Trim();
            bool ok = v.Length > 0 && (f.folder ? Directory.Exists(v) : File.Exists(v) || (!v.Contains('\\') && v.Length > 0));
            if (f.prop == "MirrorScript" && v.Length == 0) ok = true;
            f.dot.E.Fill = ok ? Brushes2.Ok : Brushes2.Err;
            f.dot.E.ToolTip = ok ? null : Loc.Get("set.missing");
        }
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        var s = Svc.Settings;
        foreach (var f in _fields) typeof(AppSettings).GetProperty(f.prop)!.SetValue(s, f.box.Text.Trim());
        s.GitAuthorName = GitName.Text.Trim();
        s.GitAuthorEmail = GitEmail.Text.Trim();
        s.SyncExcludeDirs = ExDirs.Text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
        s.SyncExcludeFiles = ExFiles.Text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
        if (int.TryParse(KeepBox.Text.Trim(), out int k) && k >= 3) s.BackupKeep = k;
        Svc.SaveSettings();
        ActionLog.Write("settings", "saved");
        MainWindow.Toast_(Loc.Get("set.saved"));
        Load();
    }

    private void Open_Click(object sender, RoutedEventArgs e)
    {
        Directory.CreateDirectory(AppSettings.AppDataDir);
        Ui.OpenFolder(AppSettings.AppDataDir);
    }
}
