using System.IO;
using System.Windows;
using System.Windows.Threading;
using MobileStudio.Core;
using ScriptHazem.Kit;

namespace MobileStudio;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        DispatcherUnhandledException += OnUiCrash;
        AppDomain.CurrentDomain.UnhandledException += (_, a) => LogCrash(a.ExceptionObject as Exception);
        TaskScheduler.UnobservedTaskException += (_, a) => { LogCrash(a.Exception); a.SetObserved(); };

        // args: --lang ar|en   --screenshot <png> <Page>   (the screenshot run never starts/stops anything)
        string lang = null, shot = null, page = null;
        var args = e.Args;
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--lang" && i + 1 < args.Length) lang = args[++i];
            else if (args[i] == "--screenshot" && i + 1 < args.Length)
            {
                shot = args[++i];
                if (i + 1 < args.Length && !args[i + 1].StartsWith("--")) page = args[++i];
            }
        }
        int opi = Array.IndexOf(args, "--op");
        if (opi >= 0 && opi + 2 < args.Length)
        {
            Svc.Init(false);
            Loc.Init("en");
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            string op = args[opi + 1], rep = Path.GetFullPath(args[opi + 2]);
            _ = Task.Run(async () =>
            {
                int code = 99;
                try { code = await SelfTest.OpAsync(op, rep); } catch (Exception ex) { LogCrash(ex); }
                Dispatcher.Invoke(() => Shutdown(code));
            });
            return;
        }
        int st = Array.IndexOf(args, "--selftest");
        if (st >= 0)
        {
            Svc.Init(true);
            Loc.Init("en");
            string rep = st + 1 < args.Length ? args[st + 1] : "selftest.txt";
            int ai = Array.IndexOf(args, "--apk");
            string apk = ai >= 0 && ai + 1 < args.Length ? args[ai + 1] : null;
            bool bk = args.Contains("--backup");
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            _ = Task.Run(async () =>
            {
                int fails = 99;
                try { fails = await SelfTest.RunAsync(Path.GetFullPath(rep), apk, bk); } catch (Exception ex) { LogCrash(ex); }
                Dispatcher.Invoke(() => Shutdown(fails));
            });
            return;
        }
        Svc.Init(shot != null);
        Loc.Init(lang ?? Svc.Settings.Language ?? "ar");
        CopySupport.Register();
        base.OnStartup(e);
        var w = new MainWindow();
        if (shot != null) w.ScreenshotRequest = (Path.GetFullPath(shot), page ?? "Dashboard");
        MainWindow = w;
        w.Show();
    }

    private void OnUiCrash(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        LogCrash(e.Exception);
        e.Handled = true;
        try { (MainWindow as MainWindow)?.ShowToast(Loc.F("toast.crash", e.Exception.Message), true); } catch { }
    }

    public static void LogCrash(Exception ex)
    {
        if (ex == null) return;
        try
        {
            Directory.CreateDirectory(AppSettings.AppDataDir);
            File.AppendAllText(Path.Combine(AppSettings.AppDataDir, "crash.log"),
                $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {ex}\r\n\r\n");
        }
        catch { }
    }
}
