using System.IO;
using System.Windows.Media;
using ScriptHazem.Kit;

namespace MobileStudio.Core;

/// <summary>One tool card on the Tools page.</summary>
public sealed class ToolEntry
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string DescKey { get; set; }
    public string Desc => DescKey != null ? Loc.Get(DescKey) : Loc.Get("tools.auto");
    public string Glyph { get; set; } = "";
    public string Folder { get; set; }
    public string Exe { get; set; }
    public bool Exists => Exe != null && File.Exists(Exe);
    public string StateText => Loc.Get(Exists ? "tools.ready" : Directory.Exists(Folder) ? "tools.notBuilt" : "tools.missing");
    public Brush StateBrush => Exists ? Brushes2.Ok : Brushes2.Warn;
    public string PathText => Exists ? Exe : Folder;
    public string Updated => Exists ? File.GetLastWriteTime(Exe).ToString("yyyy-MM-dd HH:mm") : "";
    public bool Top { get; set; }
}

public static class ToolCatalog
{
    static AppSettings S => Svc.Settings;

    /// <summary>First exe in a publish folder (prefers the folder's own name).</summary>
    public static string FindExe(string folder, string preferred = null)
    {
        string pub = Path.Combine(folder, "publish");
        if (!Directory.Exists(pub)) return preferred != null ? Path.Combine(pub, preferred) : null;
        if (preferred != null && File.Exists(Path.Combine(pub, preferred))) return Path.Combine(pub, preferred);
        var exes = Directory.GetFiles(pub, "*.exe");
        string own = exes.FirstOrDefault(e => Path.GetFileNameWithoutExtension(e).Equals(Path.GetFileName(folder), StringComparison.OrdinalIgnoreCase));
        return own ?? exes.FirstOrDefault() ?? (preferred != null ? Path.Combine(pub, preferred) : null);
    }

    public static List<ToolEntry> Load()
    {
        var list = new List<ToolEntry>();
        void Add(string id, string name, string desc, string glyph, string folder, string exeName, bool top = false)
        {
            list.Add(new ToolEntry { Id = id, Name = name, DescKey = desc, Glyph = glyph, Folder = folder, Exe = FindExe(folder, exeName), Top = top });
        }
        Add("conquer", "Mobile Conquer Studio", "tools.d.conquer", "", Path.Combine(S.ToolsRoot, "MobileConquerStudio"), "MobileConquerStudio.exe", true);
        Add("map", "Mobile Map Studio", "tools.d.map", "", Path.Combine(S.ToolsRoot, "MobileMapStudio"), "MobileMapStudio.exe", true);
        Add("ui", "Mobile UI Explorer", "tools.d.ui", "", Path.Combine(S.ToolsRoot, "MobileUiExplorer"), null, true);
        Add("patch", "PatchConsole", "tools.d.patch", "", Path.Combine(S.QahrRoot, "Tools", "PatchConsole"), "PatchConsole.exe");
        // anything else under Tools\<name>\publish\*.exe shows up by itself
        if (Directory.Exists(S.ToolsRoot))
            foreach (var d in Directory.GetDirectories(S.ToolsRoot).OrderBy(x => x))
            {
                if (list.Any(t => string.Equals(Path.GetFullPath(t.Folder), Path.GetFullPath(d), StringComparison.OrdinalIgnoreCase))) continue;
                string exe = FindExe(d);
                if (exe == null || !File.Exists(exe)) continue;
                list.Add(new ToolEntry { Id = "auto:" + Path.GetFileName(d), Name = Path.GetFileName(d), Folder = d, Exe = exe });
            }
        return list;
    }

    public static bool Launch(ToolEntry t)
    {
        if (!t.Exists) return false;
        try
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(t.Exe) { UseShellExecute = true, WorkingDirectory = Path.GetDirectoryName(t.Exe) });
            ActionLog.Write("open-tool", t.Exe);
            return true;
        }
        catch { return false; }
    }
}
