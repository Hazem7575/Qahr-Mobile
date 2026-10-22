using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace MobileStudio.Core;

/// <summary>Shared services (one instance per app).</summary>
public static class Svc
{
    public static AppSettings Settings { get; private set; }
    /// <summary>--screenshot run: nothing is saved, nothing is started or stopped.</summary>
    public static bool Headless { get; private set; }
    public static ServerMonitor Monitor { get; private set; }

    public static void Init(bool headless)
    {
        Headless = headless;
        Settings = AppSettings.Load();
        Settings.AutoDetect();
        Monitor = new ServerMonitor();
    }

    public static void SaveSettings()
    {
        if (Headless) return;
        try { Settings.Save(); } catch { }
    }

    /// <summary>The server config (non-secret keys only). Null when the file is missing.</summary>
    public static PatchConsole.ServerConfig Config()
    {
        try { return File.Exists(Settings.ConfigIni) ? new PatchConsole.ServerConfig(Settings.ConfigIni) : null; }
        catch { return null; }
    }
}

/// <summary>Every start / stop / build / publish / restore / commit is written to actions.log (no secrets).</summary>
public static class ActionLog
{
    public static string FilePath => Path.Combine(AppSettings.AppDataDir, "actions.log");

    public static void Write(string action, string detail)
    {
        if (Svc.Headless) return;
        try
        {
            Directory.CreateDirectory(AppSettings.AppDataDir);
            File.AppendAllText(FilePath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}\t{Environment.UserName}\t{action}\t{(detail ?? "").Replace("\r", " ").Replace("\n", " | ")}\r\n",
                new UTF8Encoding(false));
        }
        catch { }
    }

    public static List<string> Tail(int max)
    {
        try { return File.Exists(FilePath) ? File.ReadAllLines(FilePath).Reverse().Take(max).ToList() : new List<string>(); }
        catch { return new List<string>(); }
    }
}

/// <summary>Status colours from the theme — nothing hard-coded.</summary>
public static class Brushes2
{
    public static Brush Err => Get("ErrBrush");
    public static Brush Warn => Get("WarnBrush");
    public static Brush Ok => Get("OkBrush");
    public static Brush Info => Get("InfoBrush");
    public static Brush Text => Get("TextBrush");
    public static Brush Subtle => Get("SubtleBrush");
    public static Brush Faint => Get("FaintBrush");
    public static Brush Gold => Get("GoldText");
    public static Brush Icon => Get("IconBrush");

    private static readonly Dictionary<string, Brush> Cache = new();

    public static Brush Get(string key)
    {
        lock (Cache)
        {
            if (Cache.TryGetValue(key, out var b)) return b;
            b = (Application.Current?.TryFindResource(key) as Brush)?.CloneCurrentValue() ?? Brushes.Gray.CloneCurrentValue();
            b.Freeze();
            Cache[key] = b;
            return b;
        }
    }

    public static Brush For(Health h) => h switch
    {
        Health.Ok => Ok,
        Health.Warn => Warn,
        Health.Bad => Err,
        _ => Faint,
    };
}

public enum Health { Unknown, Ok, Warn, Bad }

public static class Fmt
{
    public static string Size(long b)
    {
        if (b < 1024) return b + " B";
        if (b < 1024 * 1024) return (b / 1024.0).ToString("0.0", ScriptHazem.Kit.Loc.Culture) + " KB";
        if (b < 1024L * 1024 * 1024) return (b / 1048576.0).ToString("0.0", ScriptHazem.Kit.Loc.Culture) + " MB";
        return (b / 1073741824.0).ToString("0.00", ScriptHazem.Kit.Loc.Culture) + " GB";
    }

    public static string Duration(TimeSpan t)
    {
        if (t.TotalSeconds < 0) t = TimeSpan.Zero;
        if (t.TotalMinutes < 1) return ScriptHazem.Kit.Loc.F("dur.s", (int)t.TotalSeconds);
        if (t.TotalHours < 1) return ScriptHazem.Kit.Loc.F("dur.ms", (int)t.TotalMinutes, t.Seconds);
        if (t.TotalDays < 1) return ScriptHazem.Kit.Loc.F("dur.hm", (int)t.TotalHours, t.Minutes);
        return ScriptHazem.Kit.Loc.F("dur.dh", (int)t.TotalDays, t.Hours);
    }

    public static string Ago(DateTime t) => ScriptHazem.Kit.Loc.F("dur.ago", Duration(DateTime.Now - t));
}
