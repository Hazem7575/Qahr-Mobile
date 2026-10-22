// ============================================================================
//  Module system (Infinity source) - professional, scalable plugin framework.
//
//  A "module" is any non-abstract class deriving ModuleBase. It is discovered
//  automatically by ModuleManager (reflection) and started after the server
//  data is loaded. Put each module in its own folder under  Modules\  and add
//  its .cs files to GameServer.csproj. ALL custom (non-official-Conquer)
//  content belongs under  Modules\Custom\ .
//
//  Design rules (hooks run on game threads):
//   * keep hooks fast - never block on files / MySQL / network inside a hook;
//   * assume concurrent calls: guard shared state with locks / Concurrent*;
//   * an exception inside a hook never reaches the game: it is caught + logged.
//
//  Only safe, non-hot-path hooks are exposed (load / shutdown / save / login /
//  logout). Combat and movement are intentionally NOT hooked here so a module
//  can never slow those hot paths down. Custom packet handlers keep using the
//  existing [PacketAttribute] / [NpcAttribute] dispatch.
// ============================================================================
using System;
using System.IO;

namespace Qahr.Mobile.Modules
{
    public abstract class ModuleBase
    {
        // ---------------------------------------------------------- identity
        /// <summary>Unique module name (also its data folder name).</summary>
        public abstract string Name { get; }
        public virtual string Version { get { return "1.0"; } }
        /// <summary>Lower runs first (load + hook order). Shutdown runs in reverse.</summary>
        public virtual int Priority { get { return 0; } }
        /// <summary>Set false to keep the code but not load the module.</summary>
        public virtual bool Enabled { get { return true; } }

        // ---------------------------------------------------------- scheduling
        /// <summary>
        /// When this module is "live". null = ALWAYS active. Return a ModuleSchedule (Daily / Weekly /
        /// MonthlyDays / StartOfMonth / Yearly / Range...) to make the module appear and disappear by
        /// itself on a pre-registered schedule - OnActivate fires when the window opens, OnDeactivate when
        /// it closes. The manager re-checks every ~30 seconds.
        /// </summary>
        public virtual ModuleSchedule Schedule { get { return null; } }

        /// <summary>True while the schedule window is open (always true for unscheduled modules).</summary>
        public bool IsActive { get; internal set; }

        /// <summary>Called when the schedule window opens (and once at load for always-on modules).</summary>
        public virtual void OnActivate() { }
        /// <summary>Called when the schedule window closes.</summary>
        public virtual void OnDeactivate() { }

        // ---------------------------------------------------------- lifecycle
        /// <summary>Once, after all server data is loaded (BEFORE any schedule check). Throwing = not loaded.</summary>
        public virtual void OnLoad() { }
        /// <summary>Once, on server shutdown (reverse priority order).</summary>
        public virtual void OnShutdown() { }
        /// <summary>Periodic world save (only while active). Serialized by the manager; must be thread safe.</summary>
        public virtual void OnSave() { }

        // ---------------------------------------------------------- player
        /// <summary>After the player finished logging in.</summary>
        public virtual void OnLogin(Client.GameClient client) { }
        /// <summary>When the player leaves (disconnect / logout).</summary>
        public virtual void OnLogout(Client.GameClient client) { }

        // ---------------------------------------------------------- helpers
        /// <summary>&lt;exe&gt;\Modules\&lt;Name&gt;\ (created on first access, trailing separator).</summary>
        public string DataPath
        {
            get
            {
                if (_dataPath != null) return _dataPath;
                string dir = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules"), SafeName);
                try { Directory.CreateDirectory(dir); } catch { }
                if (!dir.EndsWith(Path.DirectorySeparatorChar.ToString())) dir += Path.DirectorySeparatorChar;
                _dataPath = dir;
                return dir;
            }
        }
        private string _dataPath;

        private string SafeName
        {
            get
            {
                string n = Name ?? "Module";
                foreach (char c in Path.GetInvalidFileNameChars()) n = n.Replace(c, '_');
                return n;
            }
        }

        /// <summary>Console log line prefixed with the module name. Never throws.</summary>
        public void Log(string line)
        {
            try { MyConsole.WriteLine("[" + Name + "] " + line, ConsoleColor.Cyan); }
            catch { try { Console.WriteLine("[" + Name + "] " + line); } catch { } }
        }
    }
}
