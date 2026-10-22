using System.IO;
using System.Net.NetworkInformation;
using System.Text.Json.Nodes;

namespace MobileStudio.Core;

/// <summary>One look at the machine: processes, ports, the AdminApi status and Hot Reload.</summary>
public sealed class ServerSnapshot
{
    public DateTime Time = DateTime.Now;
    // GameServer
    public int GamePid;
    public bool GameUnderWatch;
    public List<int> WatchPids = new();
    public DateTime? GameStart;
    public long GameRam;
    public bool GamePortUp;
    public int GamePort = 5828;
    // AccServer
    public int AccPid;
    public DateTime? AccStart;
    public long AccRam;
    public bool AccPortUp;
    public int AccPort = 16100;
    // the rest
    public int CheckPort = 9547, HttpPort = 9549, RegisterPort = 9548, MysqlPort = 3306, AdminPort = 5899;
    public bool CheckUp, HttpUp, RegisterUp, MysqlUp, AdminUp, CdnUp;
    public int CdnPort = 9550;
    public string PatchMode = "";
    public JsonObject Status;          // AdminApi "status"
    public string StatusError;
    public HotReloadState HotReload = new();

    public bool GameRunning => GamePid > 0;
    public bool AccRunning => AccPid > 0;
    public int Online => Status != null ? AdminClient.Int(Status, "online") : -1;
    public TimeSpan? GameUptime => Status != null ? TimeSpan.FromSeconds(AdminClient.Long(Status, "uptimeSec")) : GameStart.HasValue ? DateTime.Now - GameStart.Value : null;
    public TimeSpan? AccUptime => AccStart.HasValue ? DateTime.Now - AccStart.Value : null;

    public Health GameHealth => !GameRunning ? Health.Bad : !GamePortUp ? Health.Warn : Health.Ok;
    public Health AccHealth => !AccRunning ? Health.Bad : !AccPortUp ? Health.Warn : Health.Ok;
}

public sealed class ServerMonitor
{
    public ServerSnapshot Last { get; private set; } = new();
    public event Action<ServerSnapshot> Updated;
    private int _busy;

    public static HashSet<int> Listening()
    {
        try { return IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners().Select(e => e.Port).ToHashSet(); }
        catch { return new HashSet<int>(); }
    }

    public async Task<ServerSnapshot> PollAsync(bool withApi = true)
    {
        if (Interlocked.Exchange(ref _busy, 1) == 1) return Last;
        try
        {
            var s = await Task.Run(() => Take());
            if (withApi && s.AdminUp)
            {
                var r = await AdminClient.CallAsync("status", null, 4000);
                if (r.Ok) s.Status = r.Json; else s.StatusError = r.Error;
            }
            Last = s;
            Updated?.Invoke(s);
            return s;
        }
        finally { Interlocked.Exchange(ref _busy, 0); }
    }

    private static ServerSnapshot Take()
    {
        var st = Svc.Settings;
        var s = new ServerSnapshot();
        var cfg = Svc.Config();
        if (cfg != null)
        {
            s.GamePort = cfg.GamePort > 0 ? cfg.GamePort : 5828;
            s.AccPort = cfg.LoginPort > 0 ? cfg.LoginPort : 16100;
            s.CheckPort = cfg.CheckPort;
            s.HttpPort = cfg.HttpPort;
            s.RegisterPort = cfg.RegisterPort;
            s.MysqlPort = cfg.GetInt("Database", "Port", 3306);
            s.PatchMode = cfg.Mode;
        }
        s.AdminPort = AdminClient.Port;
        var ports = Listening();
        s.GamePortUp = ports.Contains(s.GamePort);
        s.AccPortUp = ports.Contains(s.AccPort);
        s.CheckUp = ports.Contains(s.CheckPort);
        s.HttpUp = ports.Contains(s.HttpPort);
        s.RegisterUp = s.RegisterPort > 0 && ports.Contains(s.RegisterPort);
        s.MysqlUp = ports.Contains(s.MysqlPort);
        s.AdminUp = ports.Contains(s.AdminPort);
        s.CdnPort = st.CdnPort;
        s.CdnUp = ports.Contains(s.CdnPort);

        var snap = ProcTree.Snapshot();
        var game = ProcTree.FindUnder(snap, "Qahr.Mobile.Game.exe", st.QahrRoot).FirstOrDefault();
        if (game.e != null)
        {
            s.GamePid = game.e.Pid;
            s.GameStart = ProcTree.StartTime(s.GamePid);
            s.GameRam = ProcTree.WorkingSet(s.GamePid);
            var anc = ProcTree.DotnetAncestors(snap, s.GamePid);
            s.WatchPids = anc.Select(a => a.Pid).ToList();
            s.GameUnderWatch = anc.Count >= 2; // dotnet run + dotnet-watch (+ dotnet watch)
        }
        var acc = ProcTree.FindUnder(snap, "Qahr.Mobile.Login.exe", st.QahrRoot).FirstOrDefault();
        if (acc.e != null)
        {
            s.AccPid = acc.e.Pid;
            s.AccStart = ProcTree.StartTime(s.AccPid);
            s.AccRam = ProcTree.WorkingSet(s.AccPid);
        }
        s.HotReload = HotReloadState.Read(st.WatchErrLog);
        return s;
    }
}
