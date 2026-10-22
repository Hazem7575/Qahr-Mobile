// شاشة التحميل في الكونسول: بدل ما الكونسول يتملي ~100 سطر وقت الإقلاع، بيظهر شريط تحميل بيتحدّث في مكانه
// لحد ما الداتا تخلص، وبعدين ملخّص منظّم. ولا سطر بيضيع: كل اللي اتكتب وقت الإقلاع بيتحفظ في boot_log.txt،
// والتحذيرات والأخطاء بتظهر فوق الشريط على طول.
// لو الكونسول متحوّل لملف (redirect) أو [Console] BootScreen=0 في ServerConfig.ini => السلوك القديم بالظبط.
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Qahr.Mobile
{
    public static class BootScreen
    {
        private static readonly object sync = new object();
        private static volatile bool active;
        private static TextWriter realOut;
        private static readonly StringBuilder bootLog = new StringBuilder();
        private static readonly List<string> highlights = new List<string>();
        private static readonly System.Diagnostics.Stopwatch clock = new System.Diagnostics.Stopwatch();
        private static int done, total = 110;
        private static string current = "";
        private static long lastDraw;
        private static int warnings;
        private static long stepStartMs;

        public static bool Active { get { return active; } }
        public static string Slowest = "";

        public static void Begin(string title, string subtitle)
        {
            try
            {
                if (Console.IsOutputRedirected || SharedConfig.Int("Console", "BootScreen", 1) == 0)
                    return;
                try { Console.OutputEncoding = Encoding.UTF8; } catch { }
                // عدد الخطوات من آخر إقلاع (startup_times.txt) عشان النسبة تبقى مظبوطة
                try
                {
                    string f = AppDomain.CurrentDomain.BaseDirectory + "startup_times.txt";
                    if (File.Exists(f))
                    {
                        int n = File.ReadAllLines(f).Length - 1;
                        if (n > 10) total = n + 3;
                    }
                }
                catch { }

                realOut = Console.Out;
                clock.Start();
                Console.CursorVisible = false;
                DrawHeader(title, subtitle);
                Console.SetOut(new Interceptor());
                active = true;
                Draw(true);
            }
            catch
            {
                active = false;
                if (realOut != null) try { Console.SetOut(realOut); } catch { }
            }
        }

        public static void StepStart(string name)
        {
            if (!active) return;
            lock (sync) { current = name; stepStartMs = clock.ElapsedMilliseconds; }
            Draw(false);
        }

        public static void StepDone()
        {
            if (!active) return;
            lock (sync)
            {
                if (done < total - 1) done++;
                long ms = clock.ElapsedMilliseconds - stepStartMs;
                if (ms >= 250) bootLog.Append(MyConsole.TimeStamp()).Append(" [Boot] slow step: ").Append(current).Append(' ').Append(ms).AppendLine(" ms");
            }
            Draw(false);
        }

        /// <summary>من MyConsole.WriteLine وقت الإقلاع.</summary>
        public static void OnLine(string text, ConsoleColor color)
        {
            lock (sync)
            {
                bootLog.Append(MyConsole.TimeStamp()).Append(' ').AppendLine(text);
                if (IsSummaryLine(text)) highlights.Add(text);
                bool important = color == ConsoleColor.Red || color == ConsoleColor.DarkRed || color == ConsoleColor.Yellow
                                 || color == ConsoleColor.DarkYellow || LooksLikeProblem(text);
                if (important)
                    PrintAbove(text, color == ConsoleColor.Red || color == ConsoleColor.DarkRed || LooksLikeError(text) ? ConsoleColor.Red : ConsoleColor.Yellow);
            }
        }

        public static void Finish(string readyText)
        {
            if (!active) return;
            lock (sync)
            {
                active = false;
                done = total;
                current = "";
                var o = realOut;
                ClearLine();
                Bar(o, 1.0);
                Console.ForegroundColor = ConsoleColor.Green;
                o.Write("  " + (clock.ElapsedMilliseconds / 1000.0).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) + "s");
                o.WriteLine();
                o.WriteLine();

                foreach (var h in highlights)
                {
                    Console.ForegroundColor = ConsoleColor.Green; o.Write("   √ ");
                    Console.ForegroundColor = ConsoleColor.Gray; o.WriteLine(h.Trim());
                }
                if (Slowest.Length > 0)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    string slow = "     Slowest: " + Slowest;
                    int max = SafeWidth() - 2;
                    if (slow.Length > max) slow = slow.Substring(0, Math.Max(10, max - 3)) + "...";
                    o.WriteLine(slow);
                }
                if (warnings > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    o.WriteLine("   ! " + warnings + " warning(s)/error(s) while loading (shown above, full list in boot_log.txt)");
                }
                Console.ForegroundColor = ConsoleColor.DarkGray;
                o.WriteLine("     Full boot log: boot_log.txt");
                o.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
                o.WriteLine("  ══════════════  " + readyText + "  ══════════════");
                Console.ForegroundColor = ConsoleColor.White;
                o.WriteLine();
                try { File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "boot_log.txt", bootLog.ToString(), Encoding.UTF8); } catch { }
                try { Console.SetOut(o); } catch { }
                try { Console.CursorVisible = true; } catch { }
            }
        }

        /// <summary>لو الإقلاع وقع بـ exception: رجّع الكونسول واطبع كل اللي اتكتب عشان السبب يبان.</summary>
        public static void Abort()
        {
            if (!active) return;
            lock (sync)
            {
                active = false;
                ClearLine();
                try { Console.SetOut(realOut); } catch { }
                try { Console.CursorVisible = true; } catch { }
                Console.ForegroundColor = ConsoleColor.Gray;
                realOut.Write(bootLog.ToString());
                try { File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "boot_log.txt", bootLog.ToString(), Encoding.UTF8); } catch { }
            }
        }

        private static bool IsSummaryLine(string t)
        {
            return t.StartsWith("[Storage] Player backend") || (t.StartsWith("[Modules]") && t.Contains("module(s) loaded"))
                   || t.StartsWith("[Network]") || t.StartsWith("[Threads]") || t.StartsWith("[AdminApi] listening");
        }

        private static bool LooksLikeError(string t)
        {
            return t.IndexOf("exception", StringComparison.OrdinalIgnoreCase) >= 0 || t.IndexOf("error", StringComparison.OrdinalIgnoreCase) >= 0
                   || t.IndexOf("failed", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static bool LooksLikeProblem(string t)
        {
            return LooksLikeError(t) || t.IndexOf("warn", StringComparison.OrdinalIgnoreCase) >= 0
                   || t.IndexOf("missing", StringComparison.OrdinalIgnoreCase) >= 0 || t.IndexOf("not found", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static void DrawHeader(string title, string subtitle)
        {
            var o = realOut;
            int w = 58;
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            o.WriteLine();
            o.WriteLine("  ╔" + new string('═', w) + "╗");
            Console.ForegroundColor = ConsoleColor.DarkCyan; o.Write("  ║");
            Console.ForegroundColor = ConsoleColor.Cyan; o.Write(Center(title, w));
            Console.ForegroundColor = ConsoleColor.DarkCyan; o.WriteLine("║");
            o.Write("  ║");
            Console.ForegroundColor = ConsoleColor.Gray; o.Write(Center(subtitle, w));
            Console.ForegroundColor = ConsoleColor.DarkCyan; o.WriteLine("║");
            o.WriteLine("  ╚" + new string('═', w) + "╝");
            o.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static string Center(string s, int w)
        {
            if (s.Length >= w) return s.Substring(0, w);
            int left = (w - s.Length) / 2;
            return new string(' ', left) + s + new string(' ', w - s.Length - left);
        }

        private static void Draw(bool force)
        {
            if (!active) return;
            lock (sync)
            {
                long now = clock.ElapsedMilliseconds;
                if (!force && now - lastDraw < 30) return; // ماترسمش أكتر من ~30 مرة في الثانية
                lastDraw = now;
                try
                {
                    var o = realOut;
                    o.Write('\r');
                    double p = total == 0 ? 0 : (double)done / total;
                    Bar(o, p);
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    string name = current;
                    int room = Math.Max(10, SafeWidth() - 50);
                    if (name.Length > room) name = name.Substring(0, room - 1) + "…";
                    o.Write("  " + name.PadRight(room));
                    Console.ForegroundColor = ConsoleColor.White;
                }
                catch { }
            }
        }

        private static void Bar(TextWriter o, double p)
        {
            const int width = 30;
            int fill = (int)Math.Round(p * width);
            Console.ForegroundColor = ConsoleColor.White; o.Write("  Loading ");
            Console.ForegroundColor = ConsoleColor.Green; o.Write(new string('█', fill));
            Console.ForegroundColor = ConsoleColor.DarkGray; o.Write(new string('░', width - fill));
            Console.ForegroundColor = ConsoleColor.White; o.Write(" " + ((int)(p * 100)).ToString().PadLeft(3) + "%");
        }

        private static void PrintAbove(string text, ConsoleColor color)
        {
            try
            {
                warnings++;
                ClearLine();
                Console.ForegroundColor = color;
                realOut.WriteLine("  " + (color == ConsoleColor.Red ? "x " : "! ") + text);
                Console.ForegroundColor = ConsoleColor.White;
                lastDraw = 0;
                Draw(true);
            }
            catch { }
        }

        private static void ClearLine()
        {
            try { realOut.Write("\r" + new string(' ', SafeWidth() - 1) + "\r"); } catch { }
        }

        private static int SafeWidth()
        {
            try { int w = Console.WindowWidth; return w > 20 ? w : 80; } catch { return 80; }
        }

        /// <summary>أي Console.Write مباشر وقت الإقلاع (مش من MyConsole) بيتجمع هنا سطر سطر.</summary>
        private sealed class Interceptor : TextWriter
        {
            private readonly StringBuilder line = new StringBuilder();
            public override Encoding Encoding { get { return Encoding.UTF8; } }
            public override void Write(char value)
            {
                lock (sync)
                {
                    if (value == '\n')
                    {
                        string t = line.ToString().TrimEnd('\r');
                        line.Clear();
                        if (!active) { realOut.WriteLine(t); return; }
                        if (t.Length != 0) OnLine(t, ConsoleColor.Gray);
                    }
                    else line.Append(value);
                }
            }
            public override void Write(string value)
            {
                if (value == null) return;
                foreach (char c in value) Write(c);
            }
        }
    }
}
