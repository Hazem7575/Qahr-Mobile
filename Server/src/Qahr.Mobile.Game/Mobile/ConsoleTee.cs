using System;
using System.IO;
using System.Text;

namespace Qahr.Mobile.Phone
{
    /// <summary>QahrMobile: a copy of all console output in console.log (next to the exe), so the protocol work can be
    /// followed without the console window. Each start begins a new file (the previous one becomes console.old.log).</summary>
    public static class ConsoleTee
    {
        class TeeWriter : TextWriter
        {
            readonly TextWriter console, file;
            public TeeWriter(TextWriter console, TextWriter file) { this.console = console; this.file = file; }
            public override Encoding Encoding { get { return console.Encoding; } }
            public override void Write(char value) { console.Write(value); lock (file) file.Write(value); }
            public override void Write(string value) { console.Write(value); lock (file) file.Write(value); }
            public override void WriteLine(string value) { console.WriteLine(value); lock (file) file.WriteLine(value); }
            public override void Flush() { console.Flush(); lock (file) file.Flush(); }
        }

        public static void Start()
        {
            try
            {
                string dir = AppDomain.CurrentDomain.BaseDirectory;
                string path = Path.Combine(dir, "console.log");
                if (File.Exists(path))
                    File.Copy(path, Path.Combine(dir, "console.old.log"), true);
                var file = new StreamWriter(new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite), new UTF8Encoding(false)) { AutoFlush = true };
                Console.SetOut(new TeeWriter(Console.Out, file));
            }
            catch { }
        }
    }
}
