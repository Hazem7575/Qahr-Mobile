using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Qahr.Mobile
{
    /// <summary>
    /// [net10] Explicit replacement for <c>Encoding.Default</c>.
    /// On .NET Framework 4.8 <c>Encoding.Default</c> is the system ANSI code page (GetACP(), e.g. 1252).
    /// On .NET Core / .NET 5+ it is ALWAYS UTF-8, which would silently change how player names and other
    /// wire strings are encoded/decoded. This helper returns the same ANSI code page that 4.8 uses today,
    /// so behaviour stays identical after the migration. (IniFile.cs has its own equivalent; not used here.)
    ///
    /// TODO [net10 migration]: on .NET 10 the legacy code pages (1252, 1256, 936, ...) are NOT available
    /// until the provider is registered. Add this as the FIRST line of Program.Main when migrating:
    ///     System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
    /// Without it, GetEncoding(GetACP()) throws and this helper falls back to Encoding.Default (= UTF-8 there).
    /// </summary>
    internal static class AnsiText
    {
        public static readonly Encoding Value = Create();

        private static Encoding Create()
        {
            try { return Encoding.GetEncoding((int)GetACP()); }
            catch { return Encoding.Default; } // .NET Framework: identical to GetACP(); .NET 10: see TODO above.
        }

        [DllImport("kernel32.dll")]
        private static extern uint GetACP();
    }
}
