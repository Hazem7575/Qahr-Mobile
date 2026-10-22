using System.Text.Json.Nodes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MobileStudio.Core;
using ScriptHazem.Kit;

namespace MobileStudio.Views;

/// <summary>Players and live operations through the GameServer AdminApi (same ops as tools\admin.py).</summary>
public partial class PlayersView : UserControl, IPage
{
    public sealed class Player
    {
        public long Uid { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public int Reborn { get; set; }
        public string Cls { get; set; }
        public string Where { get; set; }
        public int Vip { get; set; }
        public string Hp { get; set; }
        public long Bp { get; set; }
        public bool Gm { get; set; }
        public bool Bot { get; set; }
        public string LevelText => Reborn > 0 ? $"{Level} ({Reborn}R)" : Level.ToString();
        public Visibility GmVis => Gm ? Visibility.Visible : Visibility.Collapsed;
        public Visibility BotVis => Bot ? Visibility.Visible : Visibility.Collapsed;
    }

    public sealed class Gm
    {
        public string Name { get; set; }
        public long Uid { get; set; }
        public string Label => $"{Name}  ·  {Uid}";
    }

    private List<Player> _all = new();
    private DateTime _loaded = DateTime.MinValue;

    public PlayersView() => InitializeComponent();

    public void OnShown() { _ = LoadAsync(); _ = LoadGmsAsync(); }
    public void OnHidden() { }
    public void OnTick(int tick) { if (tick % 10 == 0) _ = LoadAsync(); }
    public void OnStatus(ServerSnapshot s) { }
    public void OnLanguage() => Filter();
    public async Task PrepareScreenshotAsync() { await LoadAsync(); await LoadGmsAsync(); }

    private void Reply(string text, bool ok = true) =>
        Ui.Append(ReplyBox, $"{DateTime.Now:HH:mm:ss}  {(ok ? "✓" : "✗")}  {text}");

    private static string ClassName(int cls)
    {
        int c = cls / 10;
        return c switch
        {
            1 => "Trojan", 2 => "Warrior", 4 => "Archer", 5 => "Ninja", 6 => "Monk", 7 => "Pirate", 8 => "Dragon", 13 => "Water", 14 => "Fire",
            16 => "Windwalker", _ => cls.ToString(),
        };
    }

    private async Task LoadAsync()
    {
        var r = await AdminClient.CallAsync("online");
        _loaded = DateTime.Now;
        if (!r.Ok)
        {
            _all = new List<Player>();
            EmptyTitle.Text = Loc.Get("pl.apiDown");
            EmptyHint.Text = r.Error ?? "";
            Filter();
            return;
        }
        EmptyTitle.Text = Loc.Get("pl.empty");
        EmptyHint.Text = Loc.Get("pl.emptyHint");
        _all = (r.Json["players"] as JsonArray)?.OfType<JsonObject>().Select(p => new Player
        {
            Uid = AdminClient.Long(p, "uid"), Name = AdminClient.Str(p, "name"), Level = AdminClient.Int(p, "level"), Reborn = AdminClient.Int(p, "reborn"),
            Cls = ClassName(AdminClient.Int(p, "cls")), Where = $"{AdminClient.Int(p, "map")}  {AdminClient.Int(p, "x")},{AdminClient.Int(p, "y")}",
            Vip = AdminClient.Int(p, "vip"), Hp = $"{AdminClient.Long(p, "hp")} / {AdminClient.Long(p, "maxHp")}", Bp = AdminClient.Long(p, "bp"),
            Gm = AdminClient.Bool(p, "gm"), Bot = AdminClient.Bool(p, "bot"),
        }).ToList() ?? new List<Player>();
        Filter();
    }

    private void Filter()
    {
        string q = SearchBox.Text.Trim();
        var sel = (PlayerList.SelectedItem as Player)?.Uid;
        var view = q.Length == 0 ? _all : _all.Where(p => p.Name.Contains(q, StringComparison.OrdinalIgnoreCase) || p.Uid.ToString().Contains(q)).ToList();
        PlayerList.ItemsSource = view;
        PlayerList.SelectedItem = view.FirstOrDefault(p => p.Uid == sel);
        EmptyState.Visibility = view.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        CountText.Text = Loc.F("pl.count", _all.Count(p => !p.Bot), _all.Count(p => p.Bot));
        UpdateSel();
    }

    private void Search_Changed(object sender, TextChangedEventArgs e) { if (IsLoaded) Filter(); }
    private void Player_Selected(object sender, SelectionChangedEventArgs e) => UpdateSel();

    private void UpdateSel()
    {
        var p = PlayerList.SelectedItem as Player;
        SelTitle.Text = p == null ? Loc.Get("pl.noSel") : Loc.FB("pl.sel", p.Name, p.Uid);
        MsgBtn.IsEnabled = KickBtn.IsEnabled = p != null;
    }

    private async void Refresh_Click(object sender, RoutedEventArgs e) { await LoadAsync(); await LoadGmsAsync(); }

    private async void SaveAll_Click(object sender, RoutedEventArgs e)
    {
        var (ok, msg) = await ServerOps.SaveAllAsync();
        ActionLog.Write("save-all", msg);
        Reply(msg, ok);
    }

    private async void Msg_Click(object sender, RoutedEventArgs e)
    {
        if (PlayerList.SelectedItem is not Player p || MsgBox.Text.Trim().Length == 0) return;
        var r = await AdminClient.CallAsync("message", ("uid", (object)p.Uid), ("text", MsgBox.Text.Trim()), ("popup", PopupBox.IsChecked == true));
        Reply(r.Ok ? r.Message : r.Error, r.Ok);
        ActionLog.Write("message", $"{p.Name}: {MsgBox.Text.Trim()}");
        if (r.Ok) MsgBox.Clear();
    }

    private async void Kick_Click(object sender, RoutedEventArgs e)
    {
        if (PlayerList.SelectedItem is not Player p) return;
        string why = KickBox.Text.Trim();
        if (!ConfirmDialog.Ask(Loc.FB("pl.kickTitle", p.Name), Loc.Get("pl.kickMsg"),
                new[] { why.Length > 0 ? Loc.F("pl.step.reason", why) : Loc.Get("pl.step.noReason"), Loc.Get("pl.step.disconnect"), Loc.Get("pl.step.relog") },
                Loc.Get("pl.kick"), true))
            return;
        var args = new Dictionary<string, object> { ["uid"] = p.Uid };
        if (why.Length > 0) args["reason"] = why;
        var r = await AdminClient.CallAsync("kick", args);
        Reply(r.Ok ? r.Message : r.Error, r.Ok);
        ActionLog.Write("kick", $"{p.Name} ({p.Uid}) {why}");
        await LoadAsync();
    }

    private async void Broadcast_Click(object sender, RoutedEventArgs e)
    {
        string text = BcBox.Text.Trim();
        if (text.Length == 0) return;
        string mode = (BcMode.SelectedItem as ComboBoxItem)?.Tag as string ?? "center";
        if (!ConfirmDialog.Ask(Loc.Get("pl.bcConfirm"), text, new[] { Loc.F("pl.step.bc", _all.Count(p => !p.Bot), (BcMode.SelectedItem as ComboBoxItem)?.Content) },
                Loc.Get("pl.bcSend")))
            return;
        var r = await AdminClient.CallAsync("broadcast", ("text", (object)text), ("mode", mode));
        Reply(r.Ok ? r.Message : r.Error, r.Ok);
        ActionLog.Write("broadcast", $"{mode}: {text}");
        if (r.Ok) BcBox.Clear();
    }

    private async void Reload_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as Button)?.Tag is not string op) return;
        string label = (sender as Button).Content?.ToString();
        if (!ConfirmDialog.Ask(Loc.FB("pl.reloadConfirm", label), Loc.Get("pl.reloadMsg"),
                new[] { Loc.FB("pl.step.reload", op), Loc.Get("pl.step.reloadSafe") }, Loc.Get("pl.reloadOk")))
            return;
        foreach (var b in ReloadPanel.Children.OfType<Button>()) b.IsEnabled = false;
        try
        {
            var r = await AdminClient.CallAsync(op, null, 60000);
            Reply($"{op}: {(r.Ok ? r.Message : r.Error)}", r.Ok);
            ActionLog.Write(op, r.Ok ? r.Message : r.Error);
        }
        finally { foreach (var b in ReloadPanel.Children.OfType<Button>()) b.IsEnabled = true; }
    }

    private async Task LoadGmsAsync()
    {
        var r = await AdminClient.CallAsync("listgms");
        var list = r.Ok ? (r.Json["gms"] as JsonArray)?.OfType<JsonObject>().Select(g => new Gm { Name = AdminClient.Str(g, "name"), Uid = AdminClient.Long(g, "uid") }).ToList() : null;
        list ??= new List<Gm>();
        var prev = (GmBox.SelectedItem as Gm)?.Uid;
        GmBox.ItemsSource = list;
        GmBox.SelectedItem = list.FirstOrDefault(g => g.Uid == prev) ?? list.FirstOrDefault();
        CmdBtn.IsEnabled = list.Count > 0;
        CmdBtn.ToolTip = list.Count == 0 ? Loc.Get("pl.noGm") : null;
    }

    private async void Gms_Click(object sender, RoutedEventArgs e) => await LoadGmsAsync();

    private void Cmd_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter) Cmd_Click(sender, e);
    }

    private static readonly string[] Dangerous = { "@restart", "@exit", "@kill", "@ban", "@clear", "@reset", "@delete", "@remove", "@shutdown", "@kickall", "@reloadall" };

    private async void Cmd_Click(object sender, RoutedEventArgs e)
    {
        string cmd = CmdBox.Text.Trim();
        if (cmd.Length == 0 || GmBox.SelectedItem is not Gm gm) { if (GmBox.SelectedItem == null) Reply(Loc.Get("pl.noGm"), false); return; }
        if (!cmd.StartsWith("@")) cmd = "@" + cmd;
        string head = cmd.Split(' ')[0].ToLowerInvariant();
        if (Dangerous.Any(d => head.StartsWith(d)) &&
            !ConfirmDialog.Ask(Loc.Get("pl.cmdDangerTitle"), cmd, new[] { Loc.FB("pl.step.as", gm.Name) }, Loc.Get("pl.run"), true))
            return;
        CmdBtn.IsEnabled = false;
        try
        {
            var r = await AdminClient.CallAsync("runcommand", ("as", (object)gm.Uid), ("command", cmd), ("caller", "MobileStudio"));
            Reply($"{cmd} → {(r.Ok ? r.Message : r.Error)}", r.Ok);
            foreach (var m in (r.Json?["sysMessages"] as JsonArray)?.Select(x => x?.ToString()) ?? Enumerable.Empty<string>())
                Reply("   " + m);
            ActionLog.Write("runcommand", $"as {gm.Name}: {cmd}");
        }
        finally { CmdBtn.IsEnabled = true; }
    }
}
