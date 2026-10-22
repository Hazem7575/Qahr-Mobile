using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace Qahr.Mobile.Phone
{
    /// <summary>
    /// QahrMobile: event invitations on the mobile client.
    /// The server pops a yes/no dialog (MsgTaskDialog 2015, NpcReply MessageBox) for every event start and boss spawn:
    /// "&lt;Event&gt; Just Started Do You Want To Join Now !" (MsgSchedules.SendInvitation, Infinity War, Mortal Knights War)
    /// and "&lt;Boss&gt; Has Now Appeared ..." (ThreadPool.SpawnBoss). That is about 15 dialogs an hour (:00 Shadow Castle +
    /// Thrilling Spook, :05 Dragon Wraith, :06 Infinity War, :10 Dark Flag Base, :15 Nemesis Tyrant, :20 Lucky Golden Boxes,
    /// :27 Snow Banshee, :30 Flags War + Chilling Spook, :35 Elite Guild War, :41 Mortal Knights War, :45 Nether Tyrant,
    /// :50 Squidward Octopus + Golden Octopus, :57 Bloody Banshee). On the phone each one covers the screen and stops play.
    ///
    /// Those call sites go through Invite(); Player.MessageBox calls Divert() after its own checks (dead, map, arena ...).
    /// For a mobile player (MobileProtocol.Login sets MachineName "MOBILE-...") the dialog becomes a system chat line
    /// "... Type @join to go." and @join (ChatCommand) runs the dialog's "yes" action while the invitation is valid.
    /// Data\mobile_events.txt (key = value, re-read when it changes):
    ///   mode            chat | limited | dialog  (limited: at most one dialog per dialog_interval, the rest as chat lines)
    ///   dialog_interval seconds between two event dialogs for one player (limited mode)
    ///   join_window     seconds @join stays valid (0 = the dialog's own time)
    ///   chat_color      MsgColor name of the chat line
    /// A player can also type "@events dialog" / "@events chat" to choose for himself (until he logs out).
    /// PC clients keep the dialog unchanged.
    /// </summary>
    public static class MobileEvents
    {
        public enum Mode { Chat, Limited, Dialog }

        sealed class State
        {
            public Action<Client.GameClient> Join;
            public string Text;
            public DateTime JoinUntil;
            public DateTime LastDialog = DateTime.MinValue;
            public Mode? Own;
        }

        static readonly ConditionalWeakTable<Client.GameClient, State> states = new ConditionalWeakTable<Client.GameClient, State>();

        [ThreadStatic] static bool inInvite;

        public static Mode DefaultMode = Mode.Limited;
        public static int DialogInterval = 1800;
        public static int JoinWindow = 0;
        public static Game.MsgServer.MsgMessage.MsgColor ChatColor = Game.MsgServer.MsgMessage.MsgColor.yellow;

        public static string Path
        {
            get { return System.IO.Path.Combine(Program.ServerConfig.DbLocation ?? "", "mobile_events.txt"); }
        }

        static DateTime loadedStamp = DateTime.MinValue, nextCheck = DateTime.MinValue;
        static readonly object sync = new object();

        static void Refresh()
        {
            DateTime now = DateTime.UtcNow;
            if (now < nextCheck)
                return;
            lock (sync)
            {
                if (now < nextCheck)
                    return;
                nextCheck = now.AddSeconds(2);
                try
                {
                    DateTime stamp = File.Exists(Path) ? File.GetLastWriteTimeUtc(Path) : DateTime.MinValue;
                    if (stamp == loadedStamp)
                        return;
                    loadedStamp = stamp;
                    Mode mode = Mode.Limited;
                    int interval = 1800, window = 0;
                    var color = Game.MsgServer.MsgMessage.MsgColor.yellow;
                    if (File.Exists(Path))
                    {
                        foreach (var raw in File.ReadAllLines(Path))
                        {
                            string line = raw;
                            int c = line.IndexOf('#');
                            if (c >= 0) line = line.Substring(0, c);
                            int eq = line.IndexOf('=');
                            if (eq <= 0) continue;
                            string k = line.Substring(0, eq).Trim().ToLowerInvariant(), v = line.Substring(eq + 1).Trim();
                            switch (k)
                            {
                                case "mode":
                                    {
                                        Mode m;
                                        if (TryMode(v, out m)) mode = m;
                                        break;
                                    }
                                case "dialog_interval": int.TryParse(v, out interval); break;
                                case "join_window": int.TryParse(v, out window); break;
                                case "chat_color":
                                    foreach (Game.MsgServer.MsgMessage.MsgColor x in Enum.GetValues(typeof(Game.MsgServer.MsgMessage.MsgColor)))
                                        if (string.Equals(x.ToString(), v, StringComparison.OrdinalIgnoreCase)) { color = x; break; }
                                    break;
                            }
                        }
                    }
                    DefaultMode = mode;
                    DialogInterval = Math.Max(0, interval);
                    JoinWindow = Math.Max(0, window);
                    ChatColor = color;
                    MyConsole.WriteLine("[Mobile] events: mode " + mode + ", dialog_interval " + DialogInterval + "s, join_window " + JoinWindow + "s", ConsoleColor.Gray);
                }
                catch (Exception e)
                {
                    MyConsole.WriteLine("[Mobile] " + Path + ": " + e.Message, ConsoleColor.Red);
                }
            }
        }

        static bool TryMode(string s, out Mode m)
        {
            switch ((s ?? "").Trim().ToLowerInvariant())
            {
                case "chat": m = Mode.Chat; return true;
                case "limited": m = Mode.Limited; return true;
                case "dialog": m = Mode.Dialog; return true;
            }
            m = Mode.Limited;
            return false;
        }

        /// <summary>A client logged in through MobileProtocol.Login (10124 CMsgConnect).</summary>
        public static bool IsMobile(Client.GameClient client)
        {
            if (client == null)
                return false;
            string m = client.OnLogin.MachineName;// MsgLoginClient is a struct
            return m != null && m.StartsWith("MOBILE-", StringComparison.Ordinal);
        }

        /// <summary>An event invitation: Player.MessageBox for PC clients; for mobile clients Divert decides.</summary>
        public static void Invite(Client.GameClient client, string text, Action<Client.GameClient> join, int seconds,
            Game.MsgServer.MsgStaticMessage.Messages staticMessage = Game.MsgServer.MsgStaticMessage.Messages.None)
        {
            if (client == null || client.Player == null)
                return;
            bool old = inInvite;
            inInvite = true;
            try
            {
                client.Player.MessageBox(text, join, null, seconds, staticMessage);
            }
            finally
            {
                inInvite = old;
            }
        }

        /// <summary>Player.MessageBox, after its own checks: true = handled here (no dialog). Only for Invite() calls of
        /// mobile clients.</summary>
        public static bool Divert(Client.GameClient client, string text, Action<Client.GameClient> join, int seconds)
        {
            if (!inInvite || !IsMobile(client))
                return false;
            try
            {
                Refresh();
                var st = states.GetValue(client, _ => new State());
                DateTime now = DateTime.Now;
                Mode mode = st.Own ?? DefaultMode;
                if (st.Own == null && MobileNovice.QuietEvents(client)) mode = Mode.Chat;// QahrMobile: no event dialogs for beginners
                int window = JoinWindow > 0 ? JoinWindow : (seconds > 0 ? seconds : 60);
                lock (st)
                {
                    st.Join = join;
                    st.Text = text;
                    st.JoinUntil = now.AddSeconds(window);
                    if (mode == Mode.Dialog || (mode == Mode.Limited && (now - st.LastDialog).TotalSeconds >= DialogInterval))
                    {
                        st.LastDialog = now;
                        return false;// the normal dialog
                    }
                }
                string line = (text ?? "").Trim();
                if (join != null)
                    line += " Type @join to go (" + window + "s).";
                client.SendSysMesage(line, Game.MsgServer.MsgMessage.ChatMode.System, ChatColor);
                return true;
            }
            catch (Exception e)
            {
                try { MyConsole.SaveException(e); } catch { }
                return false;
            }
        }

        /// <summary>MsgMessage.MsgHandler: "@join" and "@events chat|limited|dialog". True = handled.</summary>
        public static bool ChatCommand(Client.GameClient client, string message)
        {
            if (client == null || client.Player == null || string.IsNullOrEmpty(message) || message[0] != '@')
                return false;
            string[] p = message.Substring(1).Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (p.Length == 0)
                return false;
            string cmd = p[0].ToLowerInvariant();
            if (MobileVipMenu.ChatCommand(client, cmd)) return true;// QahrMobile: "@vip" (MobileVipMenu.cs)
            if (cmd != "join" && cmd != "events")
                return false;
            if (!IsMobile(client))
                return false;
            var st = states.GetValue(client, _ => new State());
            if (cmd == "events")
            {
                Mode m;
                if (p.Length > 1 && TryMode(p[1], out m))
                {
                    lock (st) st.Own = m;
                    client.SendSysMesage("Event invitations: " + m.ToString().ToLowerInvariant() + ".", Game.MsgServer.MsgMessage.ChatMode.System, ChatColor);
                }
                else
                {
                    Refresh();
                    client.SendSysMesage("Event invitations: " + (st.Own ?? DefaultMode).ToString().ToLowerInvariant()
                        + ". Type @events chat, @events limited or @events dialog.", Game.MsgServer.MsgMessage.ChatMode.System, ChatColor);
                }
                return true;
            }
            Action<Client.GameClient> join;
            lock (st)
            {
                join = DateTime.Now <= st.JoinUntil ? st.Join : null;
                st.Join = null;
            }
            if (join == null)
            {
                client.SendSysMesage("There is no event invitation to join right now.", Game.MsgServer.MsgMessage.ChatMode.System, ChatColor);
                return true;
            }
            try
            {
                join(client);
            }
            catch (Exception e)
            {
                try { MyConsole.SaveException(e); } catch { }
            }
            return true;
        }
    }
}
