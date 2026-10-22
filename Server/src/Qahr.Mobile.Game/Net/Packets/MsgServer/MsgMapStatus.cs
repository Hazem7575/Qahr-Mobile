using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe partial class MsgBuilder
    {
        [ProtoContract]
        public class MapStatusProto
        {
            [ProtoMember(1, IsRequired = true)]
            public uint ID;
            [ProtoMember(2, IsRequired = true)]
            public uint BaseID;
            [ProtoMember(3, IsRequired = true)]
            public string Status;
            [ProtoMember(4)]
            public uint Weather;
            [ProtoMember(5)]
            public uint ID2;
        }
        public static unsafe ServerSockets.Packet MapStatusCreate(this ServerSockets.Packet stream, uint MapID, uint BaseID, ulong Status)
        {
            stream.InitWriter();
            if (MapID == 1002)
            {
                stream.ProtoBufferSerialize(new MsgBuilder.MapStatusProto()
                {
                    ID = 1002,
                    BaseID = 1002,
                    Status = "3,13,19,46,47",
                    ID2 = 1002
                });
            }
            // Revive Here In Map [ Deity Land ]
            else if (MapID == 10250)
            {
                stream.ProtoBufferSerialize(new MsgBuilder.MapStatusProto()
                {
                    ID = 10250,
                    BaseID = 10250,
                    Status = "3,13,19,46,47",
                    ID2 = 10250
                });
            }
            else if (MapID == 12288)
            {
                stream.ProtoBufferSerialize(new MsgBuilder.MapStatusProto()
                {
                    ID = 12288,
                    BaseID = 12288,
                    Status = "3,13,19,46,47",
                    ID2 = 12288
                });
            }
            else if (MapID == 9991)
            {
                stream.ProtoBufferSerialize(new MsgBuilder.MapStatusProto()
                {
                    ID = 9991,
                    BaseID = 9991,
                    Status = "3,13,19,46,47",
                    ID2 = 9991
                });
            }
            else if (MapID == 9992)
            {
                stream.ProtoBufferSerialize(new MsgBuilder.MapStatusProto()
                {
                    ID = 9992,
                    BaseID = 9992,
                    Status = "3,13,19,46,47",
                    ID2 = 9992
                });
            }
            else if (MapID == 9993)
            {
                stream.ProtoBufferSerialize(new MsgBuilder.MapStatusProto()
                {
                    ID = 9993,
                    BaseID = 9993,
                    Status = "3,13,19,46,47",
                    ID2 = 9993
                });
            }
            else if (MapID == 9990)
            {
                stream.ProtoBufferSerialize(new MsgBuilder.MapStatusProto()
                {
                    ID = 9990,
                    BaseID = 9990,
                    Status = "3,13,19,46,47",
                    ID2 = 9990
                });
            }
            // Revive Here In Map [ Dragon Island ]
            else if (MapID == 10137)
            {
                stream.ProtoBufferSerialize(new MsgBuilder.MapStatusProto()
                {
                    ID = 10137,
                    BaseID = 10137,
                    Status = "3,13,19,46,47",
                    ID2 = 10137
                });
            }
            else if (MapID == 10364)
            {
                stream.ProtoBufferSerialize(new MsgBuilder.MapStatusProto()
                {
                    ID = 10364,
                    BaseID = 10364,
                    Status = "3,13,19,46,47",
                    ID2 = 10364
                });
            }
            else if (MapID == 1036)
            {
                stream.ProtoBufferSerialize(new MsgBuilder.MapStatusProto()
                {
                    ID = 1036,
                    BaseID = 1036,
                    Status = "1,2,3,4",
                    ID2 = 1036
                });
            }
            else if (MapID == 2057)
            {
                stream.ProtoBufferSerialize(new MsgBuilder.MapStatusProto()
                {
                    ID = 2057,
                    BaseID = 2057,
                    Status = "1,2,16,20,30,31,51,60",
                    ID2 = 2057
                });
            }
            else
            {
                stream.ProtoBufferSerialize(new MsgBuilder.MapStatusProto()
                {
                    ID = MapID,
                    BaseID = BaseID,
                    Status = TypeList(Status),
                    ID2 = MapID
                });
            }
            stream.Finalize(GamePackets.MsgMapInfo);
            return stream;
        }

        /// <summary>
        /// QahrMobile: the mobile client reads the map type as a list of flag bit numbers ("3,13,19"; CGameMap::TestMapType)
        /// and ignored the server's single number (the GameMap.dat "type" bit mask). Bit 4 is the market (booths, no
        /// fighting), so it was taken out of the fixed lists above (only 1036 Market keeps it): with it no attack was sent.
        /// </summary>
        static string TypeList(ulong mask)
        {
            var bits = new List<string>();
            for (int i = 0; i < 64; i++)
                if ((mask >> i & 1) != 0)
                    bits.Add(i.ToString());
            return string.Join(",", bits);
        }
    }
}
