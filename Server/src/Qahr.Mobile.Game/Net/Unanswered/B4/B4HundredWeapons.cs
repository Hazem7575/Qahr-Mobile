// [Unanswered B4 2026-10-14] 2398 CMsgHundredWeaponsInfo (client -> server request).
// Client Process 0x117dfd6: Type 2 = full list (own if OwnerUID == hero uid -> PostCmd 0xf7e, else other-player view 0xfc9,
// OwnerUID 0 -> "not available" tip); Type 3 = one weapon's detail (reads Weapons[0], so it must exist) -> PostCmd 0xf80.
// Read-only: replies use the existing builders in MsgHundredWeaponsInfo.cs. Target must be an online player.
using System;
using System.Collections.Generic;
using ProtoBuf;
using Qahr.Mobile.Game;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.Unanswered.B4
{
    public static class B4HundredWeapons
    {
        // Request contract with plain uint fields: the server enum (WeaponsType) would throw on unknown client values.
        [ProtoContract]
        public class Request
        {
            [ProtoMember(1)] public uint Type;
            [ProtoMember(2)] public uint OwnerUID;
            [ProtoMember(3)] public List<RequestWeapon> Weapons = new List<RequestWeapon>();
        }
        [ProtoContract]
        public class RequestWeapon
        {
            [ProtoMember(1)] public uint SubType;
        }

        public const uint TypeRequestInfo = 2, TypeQueryOne = 3;

        [PacketAttribute(GamePackets.MsgHundredWeaponsInfo)]
        public static void Handle(Client.GameClient client, ServerSockets.Packet stream)
        {
            try
            {
                if (!B4Util.Ready(client)) return;
                if (!B4Throttle.Allow(client, B4Throttle.Slot2398)) return;
                var req = B4Util.ReadProto<Request>(stream);
                if (req == null) return;

                Client.GameClient target = ResolveTarget(client, req.OwnerUID);
                switch (req.Type)
                {
                    case TypeRequestInfo:
                        {
                            // Offline/unknown owner: same reply as the view-equipment path (OwnerUID 0 -> client tip).
                            if (target == null || target.HundredWeapons == null || target.HundredWeapons.Objects == null)
                                target = null;
                            B4Util.Send(client, stream.CreateHundredWeaponsInfo(target, MsgHundredWeaponsInfo.ActionID.RequestInfo));
                            break;
                        }
                    case TypeQueryOne:
                        {
                            if (target == null || target.HundredWeapons == null || target.HundredWeapons.Objects == null) return;
                            if (req.Weapons == null || req.Weapons.Count == 0) return;
                            uint sub = req.Weapons[0].SubType;
                            if (sub > ushort.MaxValue || !Enum.IsDefined(typeof(Database.MagicType.WeaponsType), (ushort)sub)) return;
                            Role.Instance.HundredWeaponInfo obj;
                            if (!target.HundredWeapons.Objects.TryGetValue((Database.MagicType.WeaponsType)(ushort)sub, out obj) || obj == null) return;
                            // Never reply Type 3 without Weapons[0]: the client reads it unconditionally.
                            B4Util.Send(client, stream.CreateHundredWeaponsInfo(obj, target, MsgHundredWeaponsInfo.ActionID.QueryOne));
                            break;
                        }
                    default:
                        return; // 0 Add / 1 Update are server -> client only
                }
            }
            catch (Exception e) { B4Util.Log(e); }
        }

        /// <summary>0 or own uid = self; otherwise only an online player (never a DB lookup).</summary>
        static Client.GameClient ResolveTarget(Client.GameClient client, uint ownerUid)
        {
            if (ownerUid == 0 || ownerUid == client.Player.UID) return client;
            Client.GameClient other;
            if (Pool.GamePoll.TryGetValue(ownerUid, out other) && other != null && other.Player != null) return other;
            return null;
        }
    }
}
