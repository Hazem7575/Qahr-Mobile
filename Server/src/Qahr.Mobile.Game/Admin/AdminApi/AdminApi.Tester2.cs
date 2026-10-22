using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Qahr.Mobile.AdminApi
{
    public static partial class AdminApiServer
    {
        /// <summary>
        /// "clientpacket" name=player hex="..." [as=mobile|server]: feeds a packet to the server as if the player's
        /// client had sent it (for two-player tests where one side cannot be driven by hand, e.g. accepting a trade).
        /// as=mobile (default): the bytes are what the mobile client sends (mobile id); they go through
        /// Program.Game_Receive, so the mobile -> server conversion and the packet log run as for a real packet.
        /// as=server: the bytes are a server-format packet and go straight to the server handler.
        /// The u16 length at offset 0 is fixed automatically. Development mode only.
        /// </summary>
        /// <summary>"pkcheck" name=attacker target=name: the state the physical attack handler checks (dev only).</summary>
        private static Dictionary<string, object> PkCheck(AdminArgs a)
        {
            if (!ServerMode.IsDevelopment) throw new AdminError("development tool");
            var c = FindOnline(a);
            Client.GameClient t = null;
            foreach (var x in Pool.GamePoll.Values)
                if (x != null && x.Player != null && x.Player.Name == a.Str("target")) { t = x; break; }
            if (t == null) throw new AdminError("target not online");
            var r = Ok("pkcheck");
            r["pkMode"] = c.Player.PkMode.ToString();
            r["onChangeAction"] = c.OnChangeAction;
            r["nSaveMele"] = c.nSaveMele;
            r["hitInMele"] = c.Player.HitInMele;
            r["inView"] = c.Player.View.Contains(t.Player);
            r["distance"] = (int)Role.Core.GetDistance(c.Player.X, c.Player.Y, t.Player.X, t.Player.Y);
            r["targetAllowAttack"] = t.Player.AllowAttack();
            r["targetAlive"] = t.Player.Alive;
            r["targetInvisible"] = t.Player.Invisible;
            r["attackSpeedMs"] = c.Equipment.AttackSpeed(true);
            var q = new Game.MsgServer.InteractQuery { UID = c.Player.UID, OpponentUID = t.Player.UID, X = t.Player.X, Y = t.Player.Y, AtkType = 2 };
            try { r["verified"] = Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(c, t.Player, null, true, q); }
            catch (Exception e) { r["verified"] = "exception " + e.Message; }
            return r;
        }

        private static unsafe Dictionary<string, object> ClientPacket(AdminArgs a)
        {
            if (!ServerMode.IsDevelopment) throw new AdminError("clientpacket is a development tool (server is in production mode)");// QahrMobile: test helper, development mode only
            var c = FindOnline(a);
            string hex = (a.Str("hex") ?? "").Replace(" ", "").Replace("-", "");
            if (hex.Length < 8 || (hex.Length & 1) != 0) throw new AdminError("hex= needs a whole packet (header included)");
            byte[] pkt = Convert.FromHexString(hex);
            if (pkt.Length > 4096) throw new AdminError("packet too big");
            pkt[0] = (byte)pkt.Length; pkt[1] = (byte)(pkt.Length >> 8);
            ushort id = (ushort)(pkt[2] | (pkt[3] << 8));
            bool server = string.Equals(a.Str("as"), "server", StringComparison.OrdinalIgnoreCase);
            if (string.Equals(a.Str("as"), "send", StringComparison.OrdinalIgnoreCase))
            {
                // as=send: a server-format packet sent TO this player (converted for the mobile on the way)
                var buf = new byte[pkt.Length + 8];// GameClient.Send writes the seal in the last 8 bytes
                Buffer.BlockCopy(pkt, 0, buf, 0, pkt.Length);
                c.Send(buf);
                Log("clientpacket", c.Player.Name, "sent to client " + id + " len " + pkt.Length, true);
                return Ok("sent packet " + id + " (" + pkt.Length + " B) to " + c.Player.Name);
            }

            var stream = ServerSockets.PacketRecycle.Take();
            Marshal.Copy(pkt, 0, (IntPtr)stream.Memory, pkt.Length);
            stream.Size = pkt.Length;
            stream.Seek(2);
            if (server)
            {
                stream.Seek(4);
                Action<Client.GameClient, ServerSockets.Packet> h;
                if (!Pool.MsgInvoker.TryGetInvoker(id, out h)) throw new AdminError("no server handler for " + id);
                try { h(c, stream); }
                finally { ServerSockets.PacketRecycle.Reuse(stream); }
            }
            else
                Program.Game_Receive(c.Socket, stream);   // reads the id at 2, converts, dispatches, recycles
            Log("clientpacket", c.Player.Name, (server ? "server " : "mobile ") + id + " len " + pkt.Length, true);
            return Ok("fed " + (server ? "server" : "mobile") + " packet " + id + " (" + pkt.Length + " B) as " + c.Player.Name);
        }
    }
}
