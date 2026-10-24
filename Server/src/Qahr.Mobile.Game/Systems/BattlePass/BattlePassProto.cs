// Martial Token (BattlePass) - wire format. Reverse-engineered from the 7957 client (Conquer.exe md5 D1803D09,
// protobuf-lite 2.6.1 classes in 3drole\protocolbuffer\msgbattlepass*.pb.cc). Field numbers and types are from the
// client's SerializeWithCachedSizes / MergePartialFromCodedStream; meanings from the CMsg*::Process code.
//
//   2064 MsgBattlePass       CMsgBattlePassPB      { 1 uint32 Type, 2 uint64 Value, 3 repeated uint64 List }
//        client -> server  Type 1 claim one reward (Value = BattlepassScoreReward index)
//                          Type 2 + Type 4 sent when the window is opened (FUN_00f06ec2(0x3b8)); no value
//                          Type 5 buy token (Value = privilege 1|2, List[0] = pay method 0 item / 1 silver / 2 CPs)
//                          Type 6 claim all (List = reward indexes)
//        server -> client  Type 0 season id (0 = hide the top-bar icon, panel key 22)
//                          Type 1 claimed (Value = index)            Type 6 claimed (List = indexes)
//                          Type 2 (Value stored, never read)         Type 5 token bought (Value = privilege)
//   2379 MsgBattlePassUser   CMsgBattlePassUserInfoPB { 1 repeated { 1 uint32 Type, 2 uint64 Value } }
//        attr 0 points inside the current level, 1 Hero Points (store currency), 2 privilege (token 0/1/2),
//        3 level, 4 points this week, 101..113 claimed-reward bitmask of reward_position group 1..13,
//        201 "+N points" chat notice. When the window is pending (opened by the click) this packet opens it.
//   2043 MsgBattlePassTaskList { 1 repeated { 1 uint32 TaskIndex, 2 uint64 Progress, 3 uint32 Status } }
//        Status 2 = active (default), 1 = claimed once (Pay Multiply offered), 0 = finished.
//   2273 MsgBattlePassTask   { 1 uint32 Type, 2 repeated uint64 Values }
//        client -> server  Type 0 [taskIndex]  = Claim / Free Multiply / Pay Multiply (after the CPs confirm)
//        server -> client  Type 0 [taskIndex, newStatus, pointsGained]   Type 2 [taskIndex, progress]
using ProtoBuf;

namespace Qahr.Mobile.BattlePass
{
    [ProtoContract]
    public class BpMsg
    {
        [ProtoMember(1, IsRequired = true)] public uint Type;
        [ProtoMember(2, IsRequired = true)] public ulong Value;
        [ProtoMember(3)] public ulong[] List;
    }

    /// <summary>Client request of 2064: no required fields (Type 2/4 come without a value).</summary>
    [ProtoContract]
    public class BpMsgIn
    {
        [ProtoMember(1)] public uint Type;
        [ProtoMember(2)] public ulong Value;
        [ProtoMember(3)] public ulong[] List;
    }

    [ProtoContract]
    public class BpUserAttr
    {
        [ProtoMember(1, IsRequired = true)] public uint Type;
        [ProtoMember(2, IsRequired = true)] public ulong Value;
    }

    [ProtoContract]
    public class BpUserInfo
    {
        [ProtoMember(1)] public BpUserAttr[] Attrs;
    }

    [ProtoContract]
    public class BpTaskItem
    {
        [ProtoMember(1, IsRequired = true)] public uint Index;
        [ProtoMember(2, IsRequired = true)] public ulong Progress;
        [ProtoMember(3, IsRequired = true)] public uint Status;
    }

    [ProtoContract]
    public class BpTaskList
    {
        [ProtoMember(1)] public BpTaskItem[] Items;
    }

    [ProtoContract]
    public class BpTaskMsg
    {
        [ProtoMember(1, IsRequired = true)] public uint Type;
        [ProtoMember(2)] public ulong[] Values;
    }

    [ProtoContract]
    public class BpTaskMsgIn
    {
        [ProtoMember(1)] public uint Type;
        [ProtoMember(2)] public ulong[] Values;
    }

    public static class BattlePassPackets
    {
        public const ushort MsgBattlePass = Game.GamePackets.MsgBattlePass;            // 2064
        public const ushort MsgBattlePassTask = Game.GamePackets.MsgBattlePassTask;    // 2273
        public const ushort MsgBattlePassTaskList = Game.GamePackets.MsgBattlePassTaskList; // 2043
        public const ushort MsgBattlePassUser = Game.GamePackets.MsgBattlePassUser;    // 2379

        public static void Send(Client.GameClient client, ushort id, object proto)
        {
            if (client == null || client.Player == null) return;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                stream.InitWriter();
                stream.ProtoBufferSerialize(proto);
                stream.Finalize(id);
                client.Send(stream);
            }
        }

        [PacketAttribute(Game.GamePackets.MsgBattlePass)]
        private static void OnBattlePass(Client.GameClient client, ServerSockets.Packet stream)
        {
            try
            {
                if (client == null || client.Player == null || !client.FullLoading) return;
                var req = stream.ProtoBufferDeserialize(new BpMsgIn());
                if (req == null) return;
                BattlePassManager.OnClientMsg(client, req);
            }
            catch (System.Exception e) { MyConsole.SaveException(e); }
        }

        [PacketAttribute(Game.GamePackets.MsgBattlePassTask)]
        private static void OnBattlePassTask(Client.GameClient client, ServerSockets.Packet stream)
        {
            try
            {
                if (client == null || client.Player == null || !client.FullLoading) return;
                var req = stream.ProtoBufferDeserialize(new BpTaskMsgIn());
                if (req == null) return;
                BattlePassManager.OnClientTaskMsg(client, req);
            }
            catch (System.Exception e) { MyConsole.SaveException(e); }
        }
    }
}
