using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Qahr.Mobile.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet CreateBossHarmRankList(this ServerSockets.Packet stream, MsgBossHarmRanking obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgBossHarmRanking);
            return stream;
        }
        public static unsafe void GetBossHarm(this ServerSockets.Packet stream, out MsgBossHarmRanking pQuery)
        {
            pQuery = new MsgBossHarmRanking();
            pQuery = stream.ProtoBufferDeserialize<MsgBossHarmRanking>(pQuery);
        }
        [PacketAttribute(GamePackets.MsgBossHarmRanking)]
        private unsafe static void Process(Client.GameClient client, ServerSockets.Packet stream)
        {

            MsgBossHarmRanking Info;
            stream.GetBossHarm(out Info);
            switch (Info.Type)
            {
                case (uint)MsgBossHarmRanking.RankAction.Show: // [ProtoFix 2026-10-14] Type is uint now
                    {
                        if (client.Map.View.MapContain(Role.MapObjectType.Monster, Info.MonsterID))
                        {
                            var monster = client.Map.View.GetMapObject<MsgMonster.MonsterRole>(Role.MapObjectType.Monster, Info.MonsterID);
                            if (monster == null)
                                break;
                            // snapshot under the score lock, highest score first
                            MsgBossHarmRankingEntry[] hunters;
                            lock (monster.scoreObj)
                            {
                                if (monster.Hunters == null)
                                    break;
                                hunters = monster.Hunters.Values.OrderByDescending(p => p.HunterScore).ToArray();
                            }
                            client.Send(stream.CreateBossHarmRankList(new MsgBossHarmRanking() { Type = (uint)MsgBossHarmRanking.RankAction.ShowRespondForFirstThree, MonsterID = Info.MonsterID, Hunters = hunters.Take(3).ToArray() })); // [ProtoFix 2026-10-14] Type is uint now
                            if (hunters.Length > 3)
                                client.Send(stream.CreateBossHarmRankList(new MsgBossHarmRanking() { Type = (uint)MsgBossHarmRanking.RankAction.ShowRespondForTheRest, MonsterID = Info.MonsterID, Hunters = hunters.Skip(3).ToArray() })); // [ProtoFix 2026-10-14] Type is uint now
                        }
                        break;
                    }
            }
        }
    }
    [ProtoContract]
    public struct MsgBossHarmRanking
    {
        [Flags]
        public enum RankAction : int
        {
            Show = 0,
            ShowRespondForFirstThree = 1,
            ShowRespondForTheRest = 2,
        }
        [ProtoMember(1, IsRequired = true)]
        public uint Type; // [ProtoFix 2026-10-14] client CMsgBossDmgPB #1 is uint32 (was int)
        [ProtoMember(2, IsRequired = true)]
        public uint MonsterID;
        [ProtoMember(3, IsRequired = true)]
        public MsgBossHarmRankingEntry[] Hunters;
    }
    [ProtoContract]
    public class MsgBossHarmRankingEntry
    {

        [ProtoMember(1, IsRequired = true)]
        public uint Rank;
        [ProtoMember(2, IsRequired = true)]
        public uint ServerID;
        [ProtoMember(3, IsRequired = true)]
        public uint HunterUID;
        [ProtoMember(4, IsRequired = true)]
        public string HunterName;
        [ProtoMember(5, IsRequired = true)]
        public uint HunterScore;
    }
}
