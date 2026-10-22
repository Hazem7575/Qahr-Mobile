using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace PatchConsole
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            if (e.Args.Length > 0 && e.Args[0].StartsWith("--"))
            {
                ShutdownMode = ShutdownMode.OnExplicitShutdown;
                Shutdown(Cli.Run(e.Args));
                return;
            }
            DispatcherUnhandledException += OnError;
            // files or folders dropped on PatchConsole.exe (or "Open with") start a new patch with them
            new MainWindow(e.Args.Where(a => System.IO.File.Exists(a) || System.IO.Directory.Exists(a)).ToArray()).Show();
        }

        void OnError(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            OpsLog.Write("خطأ", e.Exception.Message);
            Dialog.Show(Current.MainWindow, "حصل خطأ غير متوقع", e.Exception.Message, "حسنا", null, DialogKind.Error);
            e.Handled = true;
        }
    }
}
