// ============================================================================
//  AdminApi - GM chat commands from tools\GmStudio (see tools\GmStudio\server_patch\README.md).
//
//  op "listgms"   -> the online staff characters (who can run a command).
//  op "runcommand"-> {"as":"<online GM name or uid>","command":"@level 140"[,"caller":"GmStudio@PC"]}
//     Runs the text through MsgMessage.ChatCommands(client, msg) exactly as if that GM typed it in
//     chat: same staff check (IsStaffAccount, or the legacy "[GM]" name rule ChatCommands uses),
//     same owner gates (client.IsOwner inside the commands), same chat-ban refusal as MsgHandler,
//     same GM chat log (ServerLogs.AddChatLog). Like every other AdminApi op it runs on the single
//     AdminApi thread, the way chat commands run on the GM's socket thread.
//     The MsgTalk packets the server sends to THAT GM from THIS thread during the call are decoded
//     and returned as "sysMessages" (hook: GameClient.Send -> AdminApiServer.CaptureOutgoing).
// ============================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Qahr.Mobile.AdminApi
{
    public static partial class AdminApiServer
    {
        public const int MaxGmCommandLength = 250;
        private const int MaxCapturedMessages = 200;

        // Capture state. Only one runcommand runs at a time (AdminApi handles requests one by one),
        // but the lock keeps it correct even if the core is called from somewhere else.
        private static readonly object _runLock = new object();
        private static volatile Client.GameClient _captureClient;
        private static int _captureThread;
        private static List<string> _captured;

        /// <summary>True only while a runcommand is running (checked first by GameClient.Send).</summary>
        internal static bool CaptureActive { get { return _captureClient != null; } }

        /// <summary>Called by GameClient.Send(...) for every outgoing packet. Costs one field compare
        /// unless a runcommand is in progress for this very client.</summary>
        internal static unsafe void CaptureOutgoing(Client.GameClient client, ServerSockets.Packet p)
        {
            if (_captureClient == null || !ReferenceEquals(_captureClient, client) || p == null) return;
            try
            {
                if (Thread.CurrentThread.ManagedThreadId != _captureThread || p.Memory == null || p.Size < 4) return;
                byte* m = p.Memory;
                int len = *(ushort*)m;
                ushort type = *(ushort*)(m + 2);
                if (type != Game.GamePackets.MsgTalk) return;
                if (len > p.Size || len < 4) len = p.Size;
                var body = new byte[len - 4];
                for (int i = 0; i < body.Length; i++) body[i] = m[4 + i];
                AddCaptured(body);
            }
            catch { }
        }

        internal static void CaptureOutgoing(Client.GameClient client, byte[] buffer)
        {
            if (_captureClient == null || !ReferenceEquals(_captureClient, client) || buffer == null) return;
            try
            {
                if (Thread.CurrentThread.ManagedThreadId != _captureThread || buffer.Length < 4) return;
                if (BitConverter.ToUInt16(buffer, 2) != Game.GamePackets.MsgTalk) return;
                int len = BitConverter.ToUInt16(buffer, 0);
                if (len == 0) len = buffer.Length - 8;
                if (len > buffer.Length || len < 4) len = buffer.Length;
                var body = new byte[len - 4];
                Buffer.BlockCopy(buffer, 4, body, 0, body.Length);
                AddCaptured(body);
            }
            catch { }
        }

        private static void AddCaptured(byte[] protoBody)
        {
            var list = _captured;
            if (list == null || list.Count >= MaxCapturedMessages) return;
            Game.MsgServer.MsgMessage.MsgTalkProto proto;
            using (var ms = new System.IO.MemoryStream(protoBody))
                proto = ProtoBuf.Serializer.Deserialize<Game.MsgServer.MsgMessage.MsgTalkProto>(ms);
            if (proto == null || proto.Content == null || proto.Content.Length < 4) return;
            string text = proto.Content[3];
            if (!string.IsNullOrEmpty(text)) list.Add(text);
        }

        // ------------------------------------------------------------------ staff helpers
        /// <summary>The exact gate MsgMessage.ChatCommands uses for the GM command block.</summary>
        internal static bool CanUseGmCommands(Client.GameClient c)
        {
            return c != null && c.Player != null && (c.IsStaffAccount || (c.Player.Name ?? "").Contains("[GM]"));
        }

        private static Dictionary<string, object> GmRow(Client.GameClient c)
        {
            var d = new Dictionary<string, object>();
            d["name"] = c.Player.Name;
            d["uid"] = c.Player.UID;
            d["isOwner"] = SafeBool(() => c.IsOwner);
            d["staffAccount"] = SafeBool(() => c.IsStaffAccount);   // false = allowed only by the legacy [GM] name rule
            d["map"] = c.Player.Map;
            d["x"] = c.Player.X;
            d["y"] = c.Player.Y;
            d["level"] = c.Player.Level;
            return d;
        }

        private static List<Client.GameClient> OnlineGms()
        {
            return OnlineClients().Where(CanUseGmCommands).OrderBy(c => c.Player.Name, StringComparer.OrdinalIgnoreCase).ToList();
        }

        // ------------------------------------------------------------------ listgms
        private static Dictionary<string, object> ListGms()
        {
            var list = new List<object>();
            foreach (var c in OnlineGms())
            {
                try { list.Add(GmRow(c)); } catch { }
            }
            var d = Ok(list.Count == 0 ? "no staff GM is online" : list.Count + " staff GM(s) online");
            d["gms"] = list;
            return d;
        }

        // ------------------------------------------------------------------ runcommand
        /// <summary>Trims, removes line breaks, adds the leading '@'. Throws AdminError when unusable.</summary>
        internal static string NormalizeGmCommand(string raw)
        {
            string cmd = (raw ?? "").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim();
            if (cmd.Length == 0) throw new AdminError("missing 'command'");
            if (!cmd.StartsWith("@")) cmd = "@" + cmd;
            if (cmd.Length < 2 || cmd[1] == ' ' || cmd[1] == '@') throw new AdminError("bad command (expected '@name args...')");
            if (cmd.Length > MaxGmCommandLength) throw new AdminError("command too long (max " + MaxGmCommandLength + " characters)");
            return cmd;
        }

        private static Client.GameClient ResolveGm(AdminArgs a)
        {
            string who = (a.Str("as") ?? a.Str("name") ?? a.Str("uid") ?? "").Trim();
            var gms = OnlineGms();
            if (gms.Count == 0)
                throw new AdminError("no staff GM is online: log in with a GM character first, then run the command as that character");
            if (who.Length == 0)
            {
                if (gms.Count == 1) return gms[0];
                throw new AdminError("missing 'as' (online GMs: " + string.Join(", ", gms.Select(g => g.Player.Name)) + ")");
            }
            Client.GameClient c = null;
            uint uid;
            if (uint.TryParse(who, out uid))
                c = OnlineClients().FirstOrDefault(x => x.Player.UID == uid);
            if (c == null)
                c = OnlineClients().FirstOrDefault(x => string.Equals(x.Player.Name, who, StringComparison.OrdinalIgnoreCase));
            if (c == null)
                throw new AdminError("'" + who + "' is not online (online GMs: " + string.Join(", ", gms.Select(g => g.Player.Name)) + ")");
            if (!CanUseGmCommands(c))
                throw new AdminError(Who(c) + " is not a staff account, GM commands are refused");
            return c;
        }

        /// <summary>Result of one GM command run.</summary>
        internal sealed class GmRunResult
        {
            public bool Handled;
            public string Refused;            // non-null = not run at all
            public List<string> Messages = new List<string>();
        }

        /// <summary>Core: run one chat command as <paramref name="gm"/> through ChatCommands and capture
        /// what the server sends back to that GM. Never bypasses the staff/owner checks.</summary>
        internal static GmRunResult RunGmCommandCore(Client.GameClient gm, string command)
        {
            var r = new GmRunResult();
            if (!CanUseGmCommands(gm)) { r.Refused = "not a staff account, GM commands are refused"; return r; }
            if (gm.Player.IsStillBanned) { r.Refused = "this GM is chat-banned, the chat would refuse the command"; return r; } // same as MsgMessage.MsgHandler
            command = NormalizeGmCommand(command);

            var msg = new Game.MsgServer.MsgMessage(command, "GmStudio", gm.Player.Name,
                Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Talk);
            msg._From = gm.Player.Name;   // MsgHandler forces the real name too

            lock (_runLock)
            {
                _captured = r.Messages;
                _captureThread = Thread.CurrentThread.ManagedThreadId;
                _captureClient = gm;
                try
                {
                    r.Handled = Game.MsgServer.MsgMessage.ChatCommands(gm, msg);
                }
                finally
                {
                    _captureClient = null;
                    _captured = null;
                    _captureThread = 0;
                }
            }
            return r;
        }

        private static Dictionary<string, object> RunCommand(AdminArgs a)
        {
            string command = NormalizeGmCommand(a.Str("command"));
            string caller = (a.Str("caller") ?? "").Replace("\r", " ").Replace("\n", " ").Trim();
            if (caller.Length > 80) caller = caller.Substring(0, 80);
            if (caller.Length == 0) caller = "unknown tool";
            var gm = ResolveGm(a);

            var r = RunGmCommandCore(gm, command);
            string tag = "by " + caller + " (127.0.0.1)";
            if (r.Refused != null)
            {
                Log("runcommand", Who(gm), tag + ": " + command + " => REFUSED: " + r.Refused, false);
                var f = Fail(Who(gm) + ": " + r.Refused);   // returned (not thrown) so it is logged once
                f["message"] = f["error"];
                f["sysMessages"] = new List<object>();
                return f;
            }
            Log("runcommand", Who(gm), tag + ": " + command + (r.Handled ? "" : " => the command threw (wrong arguments?)") +
                " | replies: " + string.Join(" / ", r.Messages.Take(5)) + (r.Messages.Count > 5 ? " ..." : ""), r.Handled);

            Dictionary<string, object> d;
            if (r.Handled)
                d = Ok(command + " ran as " + Who(gm) + (r.Messages.Count == 0 ? " (the server sent no chat reply)" : ""));
            else
            {
                d = Fail("the server could not run " + command + " (wrong or missing arguments?)");
                d["message"] = d["error"];
            }
            d["as"] = GmRow(gm);
            d["command"] = command;
            d["sysMessages"] = r.Messages.Cast<object>().ToList();
            return d;
        }
    }
}
