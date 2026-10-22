// ============================================================================
//  [FestivalIcons 2026-10-14] which festival icons each online player has, so hides are never sent blind.
//
//  Client quirk: FguiActivityMenu_UpdateSigle|{...['Id']='X',['Show']=false...} goes through
//  Fgui_BtnBaseProxy:UpdateOnceData, which ADDS the id when it is not in its list yet - a "hide" sent to a player
//  who does not have the button SHOWS it. (A lua-dialog hide, LuaUIMgr_HideDialog, is harmless either way.)
//  So:
//   * per player (keyed by the GameClient session) the set of icon keys the server showed him;
//     reset at login (FestivalManager.SendIcons) and dropped at logout (FestivalIconsModule.OnLogout);
//   * icon_show goes to the players who do not have it yet, icon_hide only to the players who have it;
//   * shared buttons (one top-bar id / dialog for two festivals, e.g. Fgui 10459 MidAutumnVillage + JadeRabbit,
//     DlgWhiteDayBtn DreamTheater + TheaterAftermath, DlgChildrenDayBtn JourneyOfWonder + WenwenDetective) are
//     reference-counted by the icon key: a stopping festival hides it only when no running festival still uses it;
//   * login sends each distinct running icon once.
//  Icon key = "fgui:<Id>" for an FguiActivityMenu_UpdateSigle call (the button itself), else the trimmed icon_show.
//  Level: festival.json has no level field (2026-10-14), so icons go to every online player like before.
// ============================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Qahr.Mobile.Festivals
{
    public static class FestivalIcons
    {
        sealed class Shown { public readonly HashSet<string> Keys = new HashSet<string>(StringComparer.Ordinal); }

        // keyed by the client object: a relog is a new GameClient (and Reset clears it anyway), a dropped client is collected
        static ConditionalWeakTable<Client.GameClient, Shown> _shown = new ConditionalWeakTable<Client.GameClient, Shown>();

        /// <summary>Test seam (null on the server): the online players instead of Pool.GamePoll.</summary>
        public static Func<IEnumerable<Client.GameClient>> OnlineOverride;

        static readonly Regex FguiId = new Regex(@"^\s*FguiActivityMenu_UpdateSigle\|.*?(?:\['Id'\]|\bId)\s*=\s*'?(\d+)", RegexOptions.Compiled | RegexOptions.Singleline);

        /// <summary>The button an icon_show call stands for ("fgui:10459" or the trimmed call). null for an empty call.</summary>
        public static string Key(string showCall)
        {
            if (string.IsNullOrWhiteSpace(showCall)) return null;
            var m = FguiId.Match(showCall);
            return m.Success ? "fgui:" + m.Groups[1].Value : showCall.Trim();
        }

        static IEnumerable<Client.GameClient> Online()
        {
            var o = OnlineOverride;
            if (o != null) return o() ?? Enumerable.Empty<Client.GameClient>();
            if (Pool.GamePoll == null) return Enumerable.Empty<Client.GameClient>();
            return Pool.GamePoll.Values.ToArray();
        }

        static Shown Get(Client.GameClient c) { return _shown.GetValue(c, _ => new Shown()); }

        public static bool Has(Client.GameClient c, string showCall)
        {
            string k = Key(showCall);
            if (c == null || k == null) return false;
            Shown s;
            if (!_shown.TryGetValue(c, out s)) return false;
            lock (s) return s.Keys.Contains(k);
        }

        /// <summary>Login: forget what the previous session saw.</summary>
        public static void Reset(Client.GameClient c)
        {
            if (c == null) return;
            Shown s;
            if (_shown.TryGetValue(c, out s)) lock (s) s.Keys.Clear();
        }

        /// <summary>Logout.</summary>
        public static void Forget(Client.GameClient c)
        {
            if (c == null) return;
            try { _shown.Remove(c); } catch { }
        }

        /// <summary>One player: send icon_show unless he already has that button. True = sent.</summary>
        public static bool ShowTo(Client.GameClient c, string showCall)
        {
            string k = Key(showCall);
            if (c == null || c.Player == null || k == null) return false;
            var s = Get(c);
            lock (s)
            {
                if (s.Keys.Contains(k)) return false;
                if (!LuaUi.LuaPush.Send(c, showCall.Trim())) return false;
                s.Keys.Add(k);
                return true;
            }
        }

        /// <summary>Festival started: icon_show to the online players who do not have the button yet.</summary>
        public static int Show(string showCall)
        {
            if (Key(showCall) == null) return 0;
            int n = 0;
            foreach (var c in Online())
                if (c != null && c.Player != null && ShowTo(c, showCall)) n++;
            return n;
        }

        /// <summary>Festival stopped: icon_hide only to the players who have the button, and only when no running festival
        /// (runningShows = the icon_show calls of the festivals still running) uses the same button. Returns -1 when the
        /// button is kept because another running festival shares it.</summary>
        public static int Hide(string showCall, string hideCall, IEnumerable<string> runningShows)
        {
            string k = Key(showCall);
            if (k == null) return 0;
            if (runningShows != null && runningShows.Any(x => Key(x) == k)) return -1;
            int n = 0;
            foreach (var c in Online())
            {
                if (c == null || c.Player == null) continue;
                Shown s;
                if (!_shown.TryGetValue(c, out s)) continue;
                lock (s)
                {
                    if (!s.Keys.Remove(k)) continue;
                    if (!string.IsNullOrWhiteSpace(hideCall) && LuaUi.LuaPush.Send(c, hideCall.Trim())) n++;
                }
            }
            return n;
        }

        /// <summary>Login: reset, then each distinct running icon once.</summary>
        public static int Login(Client.GameClient c, IEnumerable<string> runningShows)
        {
            if (c == null || c.Player == null) return 0;
            Reset(c);
            int n = 0;
            if (runningShows == null) return 0;
            foreach (var sc in runningShows)
                if (ShowTo(c, sc)) n++;
            return n;
        }
    }

    /// <summary>Logout hook through the module system (no edit in Program.Game_Disconnect).</summary>
    public sealed class FestivalIconsModule : Modules.ModuleBase
    {
        public override string Name { get { return "FestivalIcons"; } }
        public override void OnLogout(Client.GameClient client) { FestivalIcons.Forget(client); }
    }
}
