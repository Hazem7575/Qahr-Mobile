// ============================================================================
//  ModuleManager - discovers every ModuleBase in the assembly (reflection),
//  loads them in Priority order and forwards the lifecycle/player hooks.
//  Every hook call is wrapped in try/catch so a broken module can never take
//  the server down. Wired from:
//     * Database.Server.Initialize()  (end)        -> LoadAll()
//     * MsgLoginClient (after "Was Login")          -> Login(client)
//     * Program.Game_Disconnect()                   -> Logout(client)
//     * the periodic save loop                      -> Save()
//     * the shutdown path                           -> Shutdown()
// ============================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Qahr.Mobile.Modules
{
    public static class ModuleManager
    {
        private static readonly List<ModuleBase> Loaded = new List<ModuleBase>();
        private static readonly object SaveLock = new object();
        private static volatile bool _ready;
        private static System.Threading.Timer _scheduleTimer;

        public static int Count { get { return Loaded.Count; } }

        /// <summary>Discovers and loads every enabled ModuleBase. Call once after server data is loaded.</summary>
        public static void LoadAll()
        {
            if (_ready) return;
            // OnShutdown for every module, whichever exit path the server takes.
            try { AppDomain.CurrentDomain.ProcessExit += (s, e) => Shutdown(); } catch { }
            var found = new List<ModuleBase>();
            try
            {
                foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
                {
                    if (t.IsAbstract || !typeof(ModuleBase).IsAssignableFrom(t)) continue;
                    if (t.GetConstructor(Type.EmptyTypes) == null) continue;
                    try
                    {
                        var m = (ModuleBase)Activator.CreateInstance(t);
                        if (m.Enabled) found.Add(m);
                    }
                    catch (Exception e) { SafeLog("instantiate '" + t.Name + "' failed: " + e.Message); }
                }
            }
            catch (Exception e) { SafeLog("discovery failed: " + e.Message); }

            found.Sort((a, b) => a.Priority.CompareTo(b.Priority));
            foreach (var m in found)
            {
                try { m.OnLoad(); Loaded.Add(m); SafeLog("loaded module: " + m.Name + " v" + m.Version + (m.Schedule != null ? " (scheduled)" : "")); }
                catch (Exception e) { SafeLog("module '" + m.Name + "' OnLoad failed (not loaded): " + e.Message); }
            }
            // apply the initial active state (always-on -> active now; scheduled -> only if in its window)
            CheckSchedules();
            // re-check every 30s so scheduled modules appear / disappear by themselves
            try { _scheduleTimer = new System.Threading.Timer(delegate { CheckSchedules(); }, null, 30000, 30000); } catch { }
            _ready = true;
            SafeLog(Loaded.Count + " module(s) loaded.");
        }

        /// <summary>Opens / closes scheduled modules as their time windows change. Safe to call anytime.</summary>
        private static void CheckSchedules()
        {
            for (int i = 0; i < Loaded.Count; i++)
            {
                ModuleBase m = Loaded[i];
                bool want;
                try { want = (m.Schedule == null) || m.Schedule.IsActiveNow(); } catch { continue; }
                if (m.IsActive == want) continue;
                m.IsActive = want;
                try
                {
                    if (want) { m.OnActivate(); if (m.Schedule != null) SafeLog("module '" + m.Name + "' is now ACTIVE (schedule window opened)"); }
                    else { m.OnDeactivate(); SafeLog("module '" + m.Name + "' is now INACTIVE (schedule window closed)"); }
                }
                catch (Exception e) { SafeLog("module '" + m.Name + (want ? "' OnActivate: " : "' OnDeactivate: ") + e.Message); }
            }
        }

        public static void Shutdown()
        {
            for (int i = Loaded.Count - 1; i >= 0; i--)
                try { Loaded[i].OnShutdown(); } catch (Exception e) { SafeLog("module '" + Loaded[i].Name + "' OnShutdown: " + e.Message); }
        }

        /// <summary>Serialized across callers (never two OnSave at once).</summary>
        public static void Save()
        {
            if (Loaded.Count == 0) return;
            lock (SaveLock)
                for (int i = 0; i < Loaded.Count; i++)
                {
                    if (!Loaded[i].IsActive) continue;
                    try { Loaded[i].OnSave(); } catch (Exception e) { SafeLog("module '" + Loaded[i].Name + "' OnSave: " + e.Message); }
                }
        }

        public static void Login(Client.GameClient client)
        {
            if (client == null) return;
            for (int i = 0; i < Loaded.Count; i++)
            {
                if (!Loaded[i].IsActive) continue;
                try { Loaded[i].OnLogin(client); } catch (Exception e) { SafeLog("module '" + Loaded[i].Name + "' OnLogin: " + e.Message); }
            }
        }

        public static void Logout(Client.GameClient client)
        {
            if (client == null) return;
            for (int i = 0; i < Loaded.Count; i++)
            {
                if (!Loaded[i].IsActive) continue;
                try { Loaded[i].OnLogout(client); } catch (Exception e) { SafeLog("module '" + Loaded[i].Name + "' OnLogout: " + e.Message); }
            }
        }

        private static void SafeLog(string line)
        {
            try { MyConsole.WriteLine("[Modules] " + line, ConsoleColor.Cyan); }
            catch { try { Console.WriteLine("[Modules] " + line); } catch { } }
        }
    }
}
