// [Unanswered B6 2026-10-14] 2421 CMsgOperatingAct (operating / promotional activities status). Raw struct, not protobuf.
//
// Client (Conquer.exe 7957, Process @0xdb782e, Create @0xd8c43e checks id 0x975):
//   +0 u16 len, +2 u16 id, +4 u16 type, +8 u16 count, +0xe u32 ids[count]
//   type 1: marks every listed activity id active, then PostCmd(0xd2a). type 3/4: popup / follow-up. others ignored.
// The server runs no "operating activity" system, so the official inactive state is type 1 with count 0:
// nothing is marked active, the UI just refreshes (PostCmd 0xd2a). Status only, nothing is granted.
// Exception: requests that list the VIP levels activity (Data\mobile_vip_levels.txt "window") go to
// Mobile\Vip\VipWelfareUi (type 0 = the Welfare tab opened, type 2 = Claim the next VIP level).
using System;

namespace Qahr.Mobile.Unanswered.B6
{
    public static class B6OperatingAct
    {
        public const ushort Id = Game.GamePackets.MsgOperatingAct;// 2421
        public const ushort TypeQuery = 0, TypeList = 1, TypeSingle = 2;

        /// <summary>Body after the 4-byte header: type(u16) pad(u16) count(u16) pad(u16 + u16) = 10 bytes, ids follow at +0xe.</summary>
        public static ServerSockets.Packet BuildEmptyList(ServerSockets.Packet stream)
        {
            stream.InitWriter();
            stream.Write(TypeList);  // +4 type 1 = list reply
            stream.Write((ushort)0); // +6
            stream.Write((ushort)0); // +8 count = 0 (no active activity)
            stream.Write((uint)0);   // +0xa..+0xd
            stream.Finalize(Id);
            return stream;
        }

        /// <summary>True when the request lists the VIP levels activity (count u16 at +8, ids u32 from +0xe).</summary>
        static bool ListsVipActivity(ServerSockets.Packet stream)
        {
            uint act = Phone.VipWelfareUi.ActId;
            if (act == 0 || stream.Size < 0xe) return false;
            stream.Seek(8);
            int count = stream.ReadUInt16();
            for (int i = 0; i < count && 0xe + i * 4 + 4 <= stream.Size; i++)
            {
                stream.Seek(0xe + i * 4);
                if (stream.ReadUInt32() == act) return true;
            }
            return false;
        }

        /// <summary>True when the request deserves the empty list (a query: type 0 list or type 2 single id).</summary>
        public static bool ShouldAnswer(ushort type) { return type == TypeQuery || type == TypeSingle; }

        [PacketAttribute(Id)]
        private static void OnOperatingAct(Client.GameClient client, ServerSockets.Packet stream)
        {
            try
            {
                if (!B6Util.Ready(client) || !B6Throttle.Allow(client, 3)) return;
                if (stream.Size < 6) return;// no type field
                stream.Seek(4);
                ushort type = stream.ReadUInt16();
                if (!ShouldAnswer(type)) return;// 1/3/4 are server->client types; never echo them
                if (ListsVipActivity(stream))
                {
                    // the VIP levels (Welfare "Credit Rebate"): type 0 = the tab opened, type 2 = Claim (ids at +0xe)
                    if (type == TypeSingle) Phone.VipWelfareUi.OnClaim(client);
                    else Phone.VipWelfareUi.OnQuery(client);
                    return;// no empty list: its PostCmd 0xcf1 would make the client clear the VIP progress
                }
                client.Send(BuildEmptyList(stream));
            }
            catch (Exception e) { MyConsole.SaveException(e); }
        }
    }
}
