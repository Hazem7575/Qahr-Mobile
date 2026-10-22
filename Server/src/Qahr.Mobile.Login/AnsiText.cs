using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Qahr.Mobile.Login
{
    /// <summary>
    /// [net10] Explicit replacement for <c>Encoding.Default</c>.
    /// On .NET Framework 4.8 <c>Encoding.Default</c> is the system ANSI code page (GetACP(), e.g. 1252).
    /// On .NET Core / .NET 5+ it is ALWAYS UTF-8, which would silently change how account names,
    /// passwords and other wire strings are decoded/encoded. This helper returns the same ANSI code page
    /// that 4.8 uses today, so behaviour stays identical after the migration.
    ///
    /// [net10] On .NET 10 the legacy code pages (1252, 1256, 936, ...) are NOT available until the provider is
    /// registered. DONE: the first line of Program.Main (inside #if NET) calls
    ///     System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
    /// Do not touch AnsiText.Value before that line runs (e.g. from a static field initializer of Program).
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
