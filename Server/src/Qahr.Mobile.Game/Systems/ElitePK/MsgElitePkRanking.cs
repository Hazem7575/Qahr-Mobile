using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet ElitePkRankingCreate(this ServerSockets.Packet stream, MsgElitePkRanking.RankType rank, uint Group, MsgElitePKBrackets.GuiTyp GroupStatus, uint Count, uint UID)
        {
            stream.InitWriter();
            stream.Write((uint)rank);//rank);//4
            stream.Write(Group);//8
            stream.Write((uint)GroupStatus);//12
            stream.Write(Count);//16
            stream.Write(1);//20
            stream.Write(2);//24
            stream.Write(UID);//28
            stream.Write(0);//32

            stream.Write(0);//32
            return stream;
        }
        public static unsafe void GetElitePkRanking(this ServerSockets.Packet stream, out MsgElitePkRanking.RankType rank, out uint Group, out MsgElitePKBrackets.GuiTyp GroupStatus, out uint Count, out uint UID)
        {

            rank = (MsgElitePkRanking.RankType)stream.ReadUInt32();
            Group = stream.ReadUInt32();
            GroupStatus = (MsgElitePKBrackets.GuiTyp)stream.ReadUInt32();
            Count = stream.ReadUInt32();
            stream.ReadUInt32();
            UID = stream.ReadUInt32();
            stream.SeekForward(8);
        }
        public static unsafe void GetItemElitePkRanking(this ServerSockets.Packet stream, out MsgTournaments.MsgEliteGroup.FighterStats status)
        {
            status = new MsgTournaments.MsgEliteGroup.FighterStats();
            status.CrossEliteRank = (uint)stream.ReadUInt32();
            status.Name = stream.ReadCString(32);
            status.Mesh = (uint)stream.ReadUInt32();
            stream.ReadUInt32();
            status.ServerID = stream.ReadUInt32();
            status.UID = stream.ReadUInt32();
            status.RealUID = stream.ReadUInt32();
            status.ClaimReward = (byte)stream.ReadUInt32();
        }
        public static unsafe ServerSockets.Packet AddItemElitePkRanking(this ServerSockets.Packet stream, MsgTournaments.MsgEliteGroup.FighterStats status, uint Rank)
        {
            stream.Write(Rank);//40 -100
            stream.Write(status.Name, 32);//44 - 104
            stream.Write(status.Mesh);//76
            stream.Write(0);//80
            stream.Write(status.FrameID);//84
            stream.Write(status.UID);//88
            stream.Write(status.RealUID);//92
            stream.Write((uint)status.ClaimReward);//96 - 160 - 224
            stream.Write((uint)0);//100
            return stream;
        }
        public static unsafe ServerSockets.Packet InterServerElitePkRankingFinalize(this ServerSockets.Packet stream)
        {
            stream.Finalize(MsgInterServer.PacketTypes.InterServer_EliteRank);
            return stream;
        }

        public static unsafe ServerSockets.Packet ElitePkRankingFinalize(this ServerSockets.Packet stream)
        {
         
            stream.Finalize(GamePackets.MsgElitePKGameRankInfo);
            return stream;
        }
        public static unsafe void GetElitePkRanking(this ServerSockets.Packet stream, out MsgElitePkRanking.RankType rank, out uint Group)
        {
            rank = (MsgElitePkRanking.RankType)stream.ReadUInt32();
            Group = stream.ReadUInt32();
        }
    }
    public unsafe struct MsgElitePkRanking
    {
        public enum RankType : uint
        {
            Top8 = 0,
            Top8Cross = 3,
            Top3 = 2,
            Top3Cross = 4
        }
        [PacketAttribute(GamePackets.MsgElitePKGameRankInfo)]
        private static void Poroces(Client.GameClient user, ServerSockets.Packet stream)
        {

            try
            {
                uint Group;
                MsgElitePkRanking.RankType rank;
                stream.GetElitePkRanking(out rank, out Group);
                if (rank == RankType.Top8Cross)
                {
                    List<MsgTournaments.MsgEliteGroup.FighterStats> array = new List<MsgTournaments.MsgEliteGroup.FighterStats>();
                    if (MsgInterServer.Instance.CrossElitePKTournament.RanksElite.TryGetValue(Group, out array))
                    {
                        stream.ElitePkRankingCreate(MsgElitePkRanking.RankType.Top8Cross, Group, MsgElitePKBrackets.GuiTyp.GUI_Top8Ranking, (uint)(array.Count), user.Player.UID);
                        for (int x = 0; x < array.Count; x++)
                            stream.AddItemElitePkRanking(array[x], array[x].CrossEliteRank);
                        user.Send(stream.ElitePkRankingFinalize());
                    }
                    else
                    {
                        stream.ElitePkRankingCreate(MsgElitePkRanking.RankType.Top8Cross, Group, MsgElitePKBrackets.GuiTyp.GUI_Top8Ranking, 0, user.Player.UID);
                        user.Send(stream.ElitePkRankingFinalize());
                    }
                }
                else
                {
                    var tournament = MsgTournaments.MsgEliteTournament.EliteGroups[Math.Min(3, (int)Group)];
                    if (tournament.Top8 == null)
                        return;
                    if (tournament.Top8.Length == 0)
                        return;
                    if (tournament.Top8[0] == null)
                        return;
                    if (tournament.State >= MsgElitePKBrackets.GuiTyp.GUI_Top1)
                    {
                        if (tournament.State == MsgElitePKBrackets.GuiTyp.GUI_Top1)
                        {
                            if (tournament.Top8[2] != null)
                            {

                                stream.ElitePkRankingCreate(RankType.Top3, Group, tournament.State, 1, user.Player.UID);

                                stream.AddItemElitePkRanking(tournament.Top8[2], 3);
                                user.Send(stream.ElitePkRankingFinalize());

                            }
                        }
                        else
                        {
                            stream.ElitePkRankingCreate(RankType.Top3, Group, tournament.State, 3, user.Player.UID);

                            for (int i = 0; i < 3; i++)
                                stream.AddItemElitePkRanking(tournament.Top8[i], (uint)(i + 1));

                            user.Send(stream.ElitePkRankingFinalize());

                        }
                    }
                    else
                    {
                        stream.ElitePkRankingCreate(RankType.Top8, Group, tournament.State, 8, user.Player.UID);
                        for (int i = 0; i < 8; i++)
                        {
                            if (tournament.Top8[i] == null)
                                break;
                            stream.AddItemElitePkRanking(tournament.Top8[i], (uint)(i + 1));
                        }
                        user.Send(stream.ElitePkRankingFinalize());
                    }
                    if (tournament.Proces != MsgTournaments.ProcesType.Dead && tournament.State >= MsgElitePKBrackets.GuiTyp.GUI_Top1)
                    {
                        stream.ElitePKBracketsCreate(MsgElitePKBrackets.Action.RequestInformation, 0, 0, (MsgTournaments.MsgEliteTournament.BPGroupTyp)Group, tournament.State, 1, 0);

                        user.Send(stream.ElitePKBracketsFinalize());
                    }
                }
            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
        }

    }
}
