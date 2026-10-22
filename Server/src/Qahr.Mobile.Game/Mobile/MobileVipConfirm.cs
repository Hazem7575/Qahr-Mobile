using System;
using System.Collections.Concurrent;

namespace Qahr.Mobile.Phone
{
    /// <summary>
    /// VIP team teleport: the teammate's answer (MsgVipHandler type 3 TeammateTeleport, mobile CMsgVipUserHandle 10126 sent by
    /// CMessageBoxCmdProcess::ProcessOk for the 0xc8d / 0xc8e box with the transport id the box was opened with) was accepted
    /// from anyone, for any city, with no VIP and no request. Offer() records the request sent to a teammate (type 2), Take()
    /// lets exactly that answer through once, before the countdown (plus a few seconds) runs out.
    /// </summary>
    public static class MobileVipConfirm
    {
        sealed class Pending { public Game.MsgServer.MsgVipHandler.VIPTeleportLocations Location; public DateTime Until; }
        static readonly ConcurrentDictionary<uint, Pending> pending = new ConcurrentDictionary<uint, Pending>();

        public static void Offer(uint memberUid, Game.MsgServer.MsgVipHandler.VIPTeleportLocations location, uint countdown)
        {
            pending[memberUid] = new Pending { Location = location, Until = DateTime.Now.AddSeconds(countdown + 5) };
        }

        public static bool Take(uint uid, Game.MsgServer.MsgVipHandler.VIPTeleportLocations location)
        {
            Pending p;
            if (!pending.TryRemove(uid, out p))
                return false;
            return p.Location == location && DateTime.Now <= p.Until;
        }
    }
}
