using System.IO;
using System.Windows;
using System.Windows.Controls;
using MobileStudio.Core;
using ScriptHazem.Kit;

namespace MobileStudio.Views;

public partial class LogsView : UserControl, IPage
{
    private const int MaxLines = 30000;
    private readonly List<LogLine> _buf = new();
    private FileTail _tail;
    private string _src = "watch";
    private LogLevel _last;

    public LogsView()
    {
        InitializeComponent();
        Open("watch");
    }

    public void OnShown() => Pull();
    public void OnHidden() { }
    public void OnTick(int tick) { if (AutoBtn.IsChecked == true && tick % 2 == 0) Pull(true); }
    public void OnStatus(ServerSnapshot s) { }
    public void OnLanguage() => Apply();
    public Task PrepareScreenshotAsync() { Pull(); return Task.CompletedTask; }

    private string PathFor(string src)
    {
        var s = Svc.Settings;
        return src switch
        {
            "watcherr" => s.WatchErrLog,
            "acc" => s.AccLog,
            "accerr" => s.AccErrLog,
            "packets" => Directory.Exists(s.PacketLogDir) ? Directory.GetFiles(s.PacketLogDir, "*.log").OrderByDescending(File.GetLastWriteTime).FirstOrDefault() ?? s.PacketLogDir : s.PacketLogDir,
            "adminapi" => Path.Combine(s.DataDir, "adminapi_log.txt"),
            "cdn" => s.CdnAccessLog,
            "cdnerr" => Path.Combine(s.MirrorRoot, "cdn_serve.err.log"),
            "actions" => ActionLog.FilePath,
            _ => s.WatchLog,
        };
    }

    private void Open(string src)
    {
        _src = src;
        string p = PathFor(src);
        _tail = new FileTail(p);
        _buf.Clear();
        _last = LogLevel.Info;
        PathText.Text = File.Exists(p) ? $"{p}   ·   {Fmt.Size(new FileInfo(p).Length)}   ·   {File.GetLastWriteTime(p):yyyy-MM-dd HH:mm:ss}" : Loc.FB("logs.missing", p);
        Pull();
    }

    private void Pull(bool auto = false)
    {
        if (_tail == null) return;
        int initial = _src == "packets" ? 2 * 1024 * 1024 : 1024 * 1024;
        var lines = _tail.Read(initial);
        if (lines.Count == 0 && _buf.Count > 0) return;
        foreach (var t in lines)
        {
            _last = LogClassifier.Classify(t, _last);
            _buf.Add(new LogLine { Text = t, Level = _last });
        }
        if (_buf.Count > MaxLines) _buf.RemoveRange(0, _buf.Count - MaxLines);
        // the user is selecting or scrolled up: keep the view (and the selection) as it is, new lines wait
        if (auto && LogBox.Items.Count > 0 && CopySupport.UserIsReading(LogBox))
        {
            if (lines.Count > 0) CountText.Text = Loc.F("logs.paused", _buf.Count);
            return;
        }
        Apply();
    }

    private void Apply()
    {
        string q = SearchBox.Text.Trim();
        var min = LvErr.IsChecked == true ? LogLevel.Error : LvWarn.IsChecked == true ? LogLevel.Warning : LogLevel.Info;
        IEnumerable<LogLine> v = _buf;
        if (min > LogLevel.Info) v = v.Where(l => l.Level >= min);
        if (q.Length > 0) v = v.Where(l => l.Text.Contains(q, StringComparison.OrdinalIgnoreCase));
        var list = v.ToList();
        bool atEnd = LogBox.Items.Count == 0 || !CopySupport.UserIsReading(LogBox);
        LogBox.ItemsSource = list;
        if (atEnd && list.Count > 0) LogBox.ScrollIntoView(list[^1]);
        CountText.Text = Loc.F("logs.count", list.Count, _buf.Count, _buf.Count(l => l.Level == LogLevel.Error), _buf.Count(l => l.Level == LogLevel.Warning));
    }

    private bool IsAtEnd()
    {
        var sv = FindScroll(LogBox);
        return sv == null || sv.VerticalOffset >= sv.ScrollableHeight - 2;
    }

    private static ScrollViewer FindScroll(DependencyObject d)
    {
        if (d is ScrollViewer s) return s;
        for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(d); i++)
        {
            var r = FindScroll(System.Windows.Media.VisualTreeHelper.GetChild(d, i));
            if (r != null) return r;
        }
        return null;
    }

    private void Source_Checked(object sender, RoutedEventArgs e)
    {
        if (!IsLoaded && _tail != null && (sender as RadioButton)?.Tag as string == "watch") return;
        if ((sender as RadioButton)?.Tag is string t && PathText != null) Open(t);
    }

    private void Level_Checked(object sender, RoutedEventArgs e) { if (CountText != null) Apply(); }
    private void Search_Changed(object sender, TextChangedEventArgs e) { if (CountText != null) Apply(); }
    private void Reload_Click(object sender, RoutedEventArgs e) => Open(_src);
    private void Open_Click(object sender, RoutedEventArgs e) { string p = PathFor(_src); if (File.Exists(p)) Ui.OpenFile(p); else Ui.OpenFolder(p); }

    private void Copy_Click(object sender, RoutedEventArgs e)
    {
        var set = new HashSet<LogLine>(LogBox.SelectedItems.Cast<LogLine>());
        var sel = LogBox.Items.Cast<LogLine>().Where(set.Contains).ToList();
        if (sel.Count == 0) sel = LogBox.Items.Cast<LogLine>().ToList();
        try { Clipboard.SetText(string.Join(Environment.NewLine, sel.Select(l => l.Text))); MainWindow.Toast_(Loc.F("logs.copied", sel.Count)); } catch { }
    }
}
