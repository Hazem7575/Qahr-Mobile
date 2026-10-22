using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace PatchConsole
{
    /// <summary>Command line, for scripts (the window is the normal way):
    ///   PatchConsole.exe --list
    ///   PatchConsole.exe --publish next|NNNN &lt;folder laid out like the resource root&gt; [--notes "text"]
    ///   PatchConsole.exe --test</summary>
    static class Cli
    {
        [DllImport("kernel32.dll")]
        static extern bool AttachConsole(int pid);

        public static int Run(string[] args)
        {
            AttachConsole(-1);
            try
            {
                Console.OutputEncoding = System.Text.Encoding.UTF8;
                return Task.Run(() => RunAsync(args)).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                Console.WriteLine("error: " + e.Message);
                return 1;
            }
        }

        static async Task<int> RunAsync(string[] args)
        {
            string cfgPath = PatchCore.FindConfig() ?? throw new Exception("ServerConfig.ini not found");
            var cfg = new ServerConfig(cfgPath);
            OpsLog.Dir = cfg.PatchDir;
            Console.WriteLine("config " + cfgPath + " | Mode=" + cfg.Mode + " | PatchDir " + cfg.PatchDir + " | URL http://" + cfg.UrlBase + "/");
            Console.WriteLine("port " + cfg.CheckPort + (PatchCore.IsListening(cfg.CheckPort) ? " listening" : " closed") +
                              ", http " + cfg.HttpPort + (PatchCore.IsListening(cfg.HttpPort) ? " listening" : " closed"));
            switch (args[0])
            {
                case "--list":
                    var dl = Stats.DownloadsPerVersion(Stats.Read(cfg.PatchDir, false));
                    foreach (var p in PatchCore.List(cfg.PatchDir))
                        Console.WriteLine(p.Version + "  " + p.SizeText + "  " + p.FileCount + " file(s)  " + p.DateText + "  downloads " +
                                          (dl.TryGetValue(p.Version, out int n) ? n : 0) + "  " + (p.Problem ?? "ok") + "\n    " + string.Join("\n    ", p.Entries));
                    return 0;
                case "--test":
                    bool ok = await PatchCore.SimulateClient(cfg.CheckPort, cfg.PatchDir, (t, good) => Console.WriteLine((good == false ? "[X] " : "[OK] ") + t));
                    Console.WriteLine(ok ? "test passed" : "test FAILED");
                    return ok ? 0 : 2;
                case "--publish":
                    if (args.Length < 3 || !Directory.Exists(args[2]))
                        throw new Exception("usage: --publish next|NNNN <folder> [--notes text]");
                    int last = PatchCore.LastUsedVersion(cfg.PatchDir);
                    int version = args[1] == "next" ? last + 1 : int.Parse(args[1]);
                    if (version <= last)
                        throw new Exception("version must be above " + last);
                    string notes = "";
                    int ni = Array.IndexOf(args, "--notes");
                    if (ni > 0 && ni + 1 < args.Length)
                        notes = args[ni + 1];
                    var files = new List<StageItem>();
                    foreach (string f in Directory.GetFiles(args[2], "*", SearchOption.AllDirectories).OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
                    {
                        string rel = Path.GetRelativePath(args[2], f).Replace('\\', '/');
                        if (!rel.Equals("version.dat", StringComparison.OrdinalIgnoreCase))
                            files.Add(new StageItem { Source = Path.GetFullPath(f), Target = rel, Size = new FileInfo(f).Length, Hash = PatchCore.Sha256File(f) });
                    }
                    string tmp = PatchCore.Build(version, files);
                    var meta = new PatchMeta
                    {
                        Version = version, Notes = notes, Created = DateTime.Now, Author = Environment.UserName,
                        Sha256 = PatchCore.Sha256File(tmp), Size = new FileInfo(tmp).Length,
                        Files = files.Select(f => new MetaFile { Path = f.Target, Size = f.Size, Sha256 = f.Hash }).ToList()
                    };
                    string dest = await PatchCore.PublishAsync(tmp, meta, cfg.PatchDir, null);
                    OpsLog.Write("نشر", "نشر التحديث " + version + " (" + files.Count + " ملف، " + PatchCore.FormatSize(meta.Size) + ") من سطر الأوامر");
                    Console.WriteLine("published " + dest + " (" + PatchCore.FormatSize(meta.Size) + "): " + string.Join(", ", files.Select(f => f.Target)) + " + version.dat=" + version);
                    return 0;
            }
            throw new Exception("unknown option " + args[0]);
        }
    }
}
