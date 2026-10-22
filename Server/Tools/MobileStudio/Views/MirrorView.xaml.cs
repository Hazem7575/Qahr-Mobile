using System.IO;
using System.Text.Json.Nodes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MobileStudio.Core;
using ScriptHazem.Kit;

namespace MobileStudio.Views;

/// <summary>Official mirror: everything goes through official_mirror.py (patchmirror's tool) with --json.</summary>
public partial class MirrorView : UserControl, IPage, IShotTab
{
    public sealed class PatchRow
    {
        public string Name { get; set; }
        public int Files { get; set; }
        public long Size { get; set; }
        public string SizeText => Fmt.Size(Size);
        public bool Report { get; set; }
        public int? Republished { get; set; }
        public int To { get; set; }
        public string State => Republished is { } v ? Loc.F("mir.st.published", v) : Report ? Loc.Get("mir.st.analysed") : Loc.Get("mir.st.downloaded");
        public Brush StateBrush => Republished != null ? Brushes2.Ok : Report ? Brushes2.Info : Brushes2.Warn;
    }

    private CancellationTokenSource _cts;
    private bool _busy;
    private DateTime _loaded = DateTime.MinValue;

    public MirrorView() => InitializeComponent();

    public void SelectTab(string name) => RightTabs.SelectedItem = name == "Output" ? TabOutput : name == "CdnLog" ? TabCdnLog : TabReport;
    public void OnShown() { if ((DateTime.Now - _loaded).TotalSeconds > 30) _ = StatusAsync(); LoadCdnLog(); }
    public void OnHidden() { }
    public void OnTick(int tick) { }
    public void OnStatus(ServerSnapshot s) { }
    public void OnLanguage() => _ = StatusAsync();
    public async Task PrepareScreenshotAsync()
    {
        await StatusAsync();
        LoadCdnLog();
        if (PatchList.Items.Count > 0) PatchList.SelectedIndex = 0;
        await Task.Delay(800);
    }

    private void SetBanner(string style, string glyph, string text)
    {
        Banner.Style = (Style)FindResource(style);
        BannerGlyph.Text = glyph;
        BannerGlyph.Foreground = style switch { "OkBanner" => Brushes2.Ok, "ErrBanner" => Brushes2.Err, "WarnBanner" => Brushes2.Warn, _ => Brushes2.Info };
        BannerText.Text = text;
    }

    private void SetBusy(bool b)
    {
        _busy = b;
        CheckBtn.IsEnabled = FetchBtn.IsEnabled = AnalyseBtn.IsEnabled = RepubBtn.IsEnabled = CdnBtn.IsEnabled = !b;
        CancelBtn.IsEnabled = b;
        if (b) MainWindow.BusyJobs++; else MainWindow.BusyJobs--;
    }

    private async Task<MirrorOps.Run> Run(string what, params string[] args)
    {
        if (!MirrorOps.Available) { SetBanner("ErrBanner", "", Loc.FB("mir.noScript", Svc.Settings.MirrorPy)); return null; }
        SetBusy(true);
        RightTabs.SelectedItem = TabOutput;
        Ui.Append(OutputBox, "> official_mirror.py " + string.Join(" ", args));
        SetBanner("InfoBanner", "", what);
        _cts = new CancellationTokenSource();
        try { return await MirrorOps.RunAsync(args, Ui.Sink(OutputBox), _cts.Token); }
        finally { SetBusy(false); }
    }

    private async Task StatusAsync()
    {
        _loaded = DateTime.Now;
        if (!MirrorOps.Available) { SetBanner("WarnBanner", "", Loc.FB("mir.noScript", Svc.Settings.MirrorPy)); return; }
        var r = await MirrorOps.RunAsync(new[] { "status" }, null);
        if (r.Json == null || !r.Ok) { SetBanner("ErrBanner", "", Loc.F("mir.statusFailed", r.Error ?? "?")); return; }
        Show(r.ResultNode);
        SetBanner("InfoBanner", "", Loc.FB("mir.ready", Svc.Settings.MirrorRoot));
    }

    private void Show(JsonNode res)
    {
        if (res == null) return;
        LatestText.Text = res["latest_official_fetched"]?.ToString() ?? "—";
        LatestSub.Text = Loc.FB("mir.latestSub", res["base_version"]?.ToString() ?? "5150", Fmt.Size(AdminClient.Long(res, "patches_disk")));
        var chk = res["last_check"];
        if (chk != null)
        {
            CheckText.Text = (chk["time"]?.ToString() ?? "").Replace('T', ' ');
            var steps = (chk["steps"] as JsonArray)?.Select(x => x?.ToString()).ToList() ?? new();
            var errs = (chk["errors"] as JsonArray)?.Select(x => x?.ToString()).ToList() ?? new();
            CheckSub.Text = Loc.FB("mir.checkSub", chk["asked"]?.ToString() ?? "?", chk["server"]?.ToString() ?? "?", string.Join(" → ", steps)) +
                            (errs.Count > 0 ? "\n" + string.Join("\n", errs) : "");
        }
        var rep = (res["republished"] as JsonArray)?.OfType<JsonObject>().ToList() ?? new();
        if (rep.Count > 0)
        {
            var last = rep[^1];
            RepubText.Text = $"{last["version"]}.zip";
            RepubSub.Text = Loc.FB("mir.repubSub", last["official"]?.ToString() ?? "", (last["time"]?.ToString() ?? "").Replace('T', ' '), Fmt.Size(AdminClient.Long(last, "size")));
        }
        else { RepubText.Text = "—"; RepubSub.Text = Loc.Get("mir.notRepub"); }
        var cdn = res["cdn"];
        long have = AdminClient.Long(cdn, "have"), files = AdminClient.Long(cdn, "files");
        bool serving = ServerMonitor.Listening().Contains(9550);
        CdnText.Text = files > 0 ? Loc.F("mir.cdnFiles", have, files) : Loc.Get("mir.cdnEmpty");
        CdnSub.Text = Loc.FB("mir.cdnSub", cdn?["version"]?.ToString() is { Length: > 0 } v ? v : "—", Fmt.Size(AdminClient.Long(cdn, "disk")),
                            Loc.Get(serving ? "mir.cdnServing" : "mir.cdnNotServing")) + "\n" + LastVerifyText();

        var rows = new List<PatchRow>();
        foreach (var p in (res["patches"] as JsonArray)?.OfType<JsonObject>() ?? Enumerable.Empty<JsonObject>())
        {
            string name = p["name"]?.ToString();
            var rp = rep.LastOrDefault(x => x["official"]?.ToString() == name);
            rows.Add(new PatchRow
            {
                Name = name, Files = AdminClient.Int(p, "file_count"), Size = AdminClient.Long(p, "size"), Report = AdminClient.Bool(p, "report"),
                To = AdminClient.Int(p, "to"), Republished = rp != null ? AdminClient.Int(rp, "version") : null,
            });
        }
        var sel = (PatchList.SelectedItem as PatchRow)?.Name;
        PatchList.ItemsSource = rows;
        PatchList.SelectedItem = rows.FirstOrDefault(x => x.Name == sel) ?? rows.LastOrDefault();
    }

    // ---------------------------------------------------------------- CDN verify record + access log
    private static string VerifyFile => Path.Combine(AppSettings.AppDataDir, "cdn_verify.txt");

    private static void SaveVerify(bool ok, string summary)
    {
        if (Svc.Headless) return;
        try { Directory.CreateDirectory(AppSettings.AppDataDir); File.WriteAllText(VerifyFile, $"{DateTime.Now:yyyy-MM-dd HH:mm}\t{(ok ? 1 : 0)}\t{summary}"); } catch { }
    }

    private static string LastVerifyText()
    {
        try
        {
            if (!File.Exists(VerifyFile)) return Loc.Get("mir.noVerify");
            var c = File.ReadAllText(VerifyFile).Split('\t');
            return Loc.F("mir.lastVerify", c[0], c.Length > 2 ? c[2] : "");
        }
        catch { return Loc.Get("mir.noVerify"); }
    }

    private void CdnLogRefresh_Click(object sender, RoutedEventArgs e) => LoadCdnLog();

    /// <summary>CDN\cdn_access.log: time, ip, method, path, status, bytes (tab separated).</summary>
    private void LoadCdnLog()
    {
        var lines = FileTail.Last(Svc.Settings.CdnAccessLog, 3000, 1024 * 1024);
        var rows = lines.Select(l =>
        {
            var c = l.Split('\t');
            bool bad = c.Length > 4 && int.TryParse(c[4], out int code) && code >= 400;
            return new LogLine { Text = l.Replace('\t', ' '), Level = bad ? LogLevel.Warning : LogLevel.Info };
        }).ToList();
        CdnLogList.ItemsSource = rows;
        if (rows.Count > 0) CdnLogList.ScrollIntoView(rows[^1]);
        string today = DateTime.Today.ToString("yyyy-MM-dd");
        var t = lines.Select(l => l.Split('\t')).Where(c => c.Length >= 6 && c[0].StartsWith(today)).ToList();
        long bytes = t.Sum(c => long.TryParse(c[5], out long b) ? b : 0);
        CdnLogSummary.Text = File.Exists(Svc.Settings.CdnAccessLog)
            ? Loc.F("mir.cdnLogSum", t.Count, Fmt.Size(bytes), t.Select(c => c[1]).Distinct().Count(), t.Count(c => int.TryParse(c[4], out int k) && k >= 400))
            : Loc.FB("logs.missing", Svc.Settings.CdnAccessLog);
    }

    private void Patch_Selected(object sender, SelectionChangedEventArgs e)
    {
        if (PatchList.SelectedItem is not PatchRow p) return;
        // the official end number, or the next free one when it is already used (numbers only go up)
        int last = 0;
        try { last = PatchConsole.PatchCore.LastUsedVersion(Svc.Config()?.PatchDir); } catch { }
        AsBox.Text = p.To > last ? p.To.ToString() : (last + 1).ToString();
        LoadReport(p.Name);
    }

    private void LoadReport(string name)
    {
        string md = Path.Combine(Svc.Settings.MirrorRoot, "reports", name + ".md");
        if (!File.Exists(md)) { ReportView.Document = null; NoReport.Visibility = Visibility.Visible; return; }
        NoReport.Visibility = Visibility.Collapsed;
        ReportView.Document = MarkdownDoc.Render(File.ReadAllText(md));
        RightTabs.SelectedItem = TabReport;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) => _cts?.Cancel();

    private async void Check_Click(object sender, RoutedEventArgs e)
    {
        var r = await Run(Loc.Get("mir.checking"), "check");
        if (r == null) return;
        var res = r.ResultNode;
        if (r.Ok)
        {
            var steps = (res?["steps"] as JsonArray)?.Select(x => x?.ToString()).ToList() ?? new();
            var np = res?["new_patches"] as JsonArray;
            bool newOne = np != null ? np.Count > 0 : steps.Any(s => s != null && s.Contains("UPDATE"));
            if (np != null && np.Count > 0) steps = np.Select(x => x?.ToString()).ToList();
            SetBanner(newOne ? "WarnBanner" : "OkBanner", newOne ? "" : "", newOne ? Loc.F("mir.newFound", string.Join(" → ", steps)) : Loc.Get("mir.upToDate"));
        }
        else SetBanner("ErrBanner", "", Loc.F("mir.failed", r.Error));
        ActionLog.Write("mirror-check", r.Ok ? "ok" : r.Error);
        await StatusAsync();
    }

    private async void Fetch_Click(object sender, RoutedEventArgs e)
    {
        if (!ConfirmDialog.Ask(Loc.Get("mir.fetchTitle"), Loc.Get("mir.fetchMsg"),
                new[] { Loc.Get("mir.step.ask"), Loc.FB("mir.step.download", Path.Combine(Svc.Settings.MirrorRoot, "patches")), Loc.Get("mir.step.noPublish") },
                Loc.Get("mir.fetch")))
            return;
        var r = await Run(Loc.Get("mir.fetching"), "fetch");
        if (r == null) return;
        SetBanner(r.Ok ? "OkBanner" : "ErrBanner", r.Ok ? "" : "", r.Ok ? Loc.Get("mir.fetched") : Loc.F("mir.failed", r.Error));
        ActionLog.Write("mirror-fetch", r.Ok ? "ok" : r.Error);
        await StatusAsync();
    }

    private async void Analyse_Click(object sender, RoutedEventArgs e)
    {
        if (PatchList.SelectedItem is not PatchRow p) { SetBanner("WarnBanner", "", Loc.Get("mir.pick")); return; }
        var r = await Run(Loc.FB("mir.analysing", p.Name), "analyse", p.Name);
        if (r == null) return;
        SetBanner(r.Ok ? "OkBanner" : "ErrBanner", r.Ok ? "" : "", r.Ok ? Loc.FB("mir.analysed", p.Name) : Loc.F("mir.failed", r.Error));
        await StatusAsync();
        if (r.Ok) LoadReport(p.Name);
    }

    private async void Republish_Click(object sender, RoutedEventArgs e)
    {
        if (PatchList.SelectedItem is not PatchRow p) { SetBanner("WarnBanner", "", Loc.Get("mir.pick")); return; }
        var args = new List<string> { "republish", p.Name };
        if (!string.IsNullOrWhiteSpace(AsBox.Text))
        {
            if (!int.TryParse(AsBox.Text.Trim(), out int v) || v <= 5150 || v >= 9999) { SetBanner("ErrBanner", "", Loc.Get("upd.badVersion")); return; }
            args.Add("--as"); args.Add(v.ToString());
        }
        // 1) dry run: shows exactly what would be written, nothing changes
        var dry = await Run(Loc.FB("mir.dryRun", p.Name), args.Append("--dry-run").ToArray());
        if (dry == null) return;
        if (!dry.Ok) { SetBanner("ErrBanner", "", Loc.F("mir.failed", dry.Error)); return; }
        var res = dry.ResultNode;
        string target = res?["target"]?.ToString() ?? res?["zip"]?.ToString() ?? (res?["version"]?.ToString() is { } vv ? vv + ".zip" : "?");
        var skipped = (res?["skipped"] as JsonArray)?.Select(x => x?.ToString()).ToList() ?? new();
        var steps = new List<string>
        {
            Loc.FB("mir.step.build", p.Name, target),
            skipped.Count > 0 ? Loc.FB("mir.step.skip", string.Join(", ", skipped)) : Loc.Get("mir.step.noSkip"),
            Loc.FB("mir.step.into", Svc.Config()?.PatchDir ?? "PatchDir"),
            Loc.Get("upd.step.live"),
        };
        if (!ConfirmDialog.Ask(Loc.FB("mir.repubTitle", p.Name), Loc.Get("mir.repubMsg"), steps, Loc.Get("mir.republish"), true,
                note: p.Republished != null ? Loc.F("mir.repubAgain", p.Republished) : null))
            return;
        var r = await Run(Loc.FB("mir.republishing", p.Name), args.ToArray());
        if (r == null) return;
        SetBanner(r.Ok ? "OkBanner" : "ErrBanner", r.Ok ? "" : "", r.Ok ? Loc.FB("mir.republished_", p.Name) : Loc.F("mir.failed", r.Error));
        ActionLog.Write("mirror-republish", $"{p.Name} {(r.Ok ? "ok" : r.Error)}");
        await StatusAsync();
    }

    private async void CdnVerify_Click(object sender, RoutedEventArgs e)
    {
        var r = await Run(Loc.Get("mir.cdnVerifying"), "cdn-verify");
        if (r == null) return;
        SetBanner(r.Ok ? "OkBanner" : "ErrBanner", r.Ok ? "" : "", r.Ok ? Loc.Get("mir.cdnVerified") : Loc.F("mir.failed", r.Error));
        await StatusAsync();
    }
}
