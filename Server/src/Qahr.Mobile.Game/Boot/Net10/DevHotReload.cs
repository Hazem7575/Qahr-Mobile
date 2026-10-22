// ============================================================================
//  [DevHotReload 2026-10-16] .NET Hot Reload observer.
//
//  WHAT THIS IS
//    A MetadataUpdateHandler. The .NET runtime calls it ONLY when a hot-reload
//    delta is applied to the running process - which only ever happens when the
//    server was launched through `dotnet watch` (see tools\ServerCtl\dev_hotreload.bat).
//    A normal production launch (bin-net10\GameServer.exe) never applies a delta,
//    so this code is completely inert there. There is no runtime cost and no
//    behaviour change in production; it is not a dev/prod #if switch, it simply
//    never fires unless `dotnet watch` is driving the process.
//
//  WHAT IT DOES
//    Prints a line to the console so the owner can see a code edit landed live,
//    and how many types changed. It does NOT touch game state, caches, festivals
//    or players - applying the new method bodies is the runtime's job; this only
//    reports it. (A cache refresh after a delta can be added later if a specific
//    edit needs it, but it is left out on purpose so this handler can never cause
//    a side effect.)
//
//  This file lives under Net10\ so it is compiled ONLY by Qahr.Mobile.Net10.csproj
//  (the .NET Framework 4.8 x86 build never sees it).
// ============================================================================
using System;

[assembly: System.Reflection.Metadata.MetadataUpdateHandler(typeof(Qahr.Mobile.Net10Dev.DevHotReload))]

namespace Qahr.Mobile.Net10Dev
{
    internal static class DevHotReload
    {
        // Called after a delta is applied. 'updatedTypes' may be null (runtime did not list them).
        internal static void UpdateApplication(Type[] updatedTypes)
        {
            try
            {
                int n = updatedTypes == null ? 0 : updatedTypes.Length;
                string names = "";
                if (updatedTypes != null && updatedTypes.Length > 0)
                {
                    var sb = new System.Text.StringBuilder();
                    for (int i = 0; i < updatedTypes.Length && i < 6; i++)
                    {
                        if (i > 0) sb.Append(", ");
                        sb.Append(updatedTypes[i].Name);
                    }
                    if (updatedTypes.Length > 6) sb.Append(" (+").Append(updatedTypes.Length - 6).Append(" more)");
                    names = "  -> " + sb;
                }
                string msg = "[HotReload] code update applied live" +
                             (n > 0 ? " (" + n + " type(s))" : "") + names +
                             "  -  method-body edits are live; data edits still need their reload* command.";
                try { MyConsole.WriteLine(msg, ConsoleColor.Magenta); }
                catch { Console.WriteLine(msg); }
            }
            catch { /* a diagnostics print must never disturb the running server */ }
        }

        // Optional seam the runtime may call before UpdateApplication to let handlers drop caches.
        // Left empty on purpose: we do not clear any game cache automatically.
        internal static void ClearCache(Type[] updatedTypes) { }
    }
}
