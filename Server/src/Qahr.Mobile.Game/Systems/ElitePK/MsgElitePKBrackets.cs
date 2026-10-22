using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Qahr.Mobile.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe void GetElitePKBrackets(this ServerSockets.Packet stream, out MsgElitePKBrackets.Action mode, out ushort Page
            , out uint MatchCount, out MsgTournaments.MsgEliteTournament.BPGroupTyp Group, out MsgElitePKBrackets.GuiTyp guityp
            , out ushort TimeLeft, out uint TotalMatches)
        {
            mode = (MsgElitePKBrackets.Action)stream.ReadUInt16();
            Page = stream.ReadUInt16();
            ushort unknow = stream.ReadUInt16();
            MatchCount = stream.ReadUInt32();
            Group = (MsgTournaments.MsgEliteTournament.BPGroupTyp)stream.ReadUInt16();
            guityp = (MsgElitePKBrackets.GuiTyp)stream.ReadUInt16();
            TimeLeft = stream.ReadUInt16();
            stream.ReadUInt8();
            TotalMatches = stream.ReadUInt32();
        }

        public static unsafe ServerSockets.Packet ElitePKBracketsCreate(this ServerSockets.Packet stream, MsgElitePKBrackets.Action mode, ushort Page
           , uint MatchCount, MsgTournaments.MsgEliteTournament.BPGroupTyp Group, MsgElitePKBrackets.GuiTyp guityp
           , ushort TimeLeft, uint TotalMatches, uint PacketNo = 0, uint PacketNo2 = 0)
        {
            stream.InitWriter();
            stream.Write((ushort)mode);//4 5
            stream.Write(Page);//6 7
            stream.Write((ushort)PacketNo);//8 9
            stream.Write(MatchCount);//10 11 12 13
            stream.Write((ushort)Group);//14 15
            stream.Write((ushort)guityp);//16
            stream.Write(TimeLeft);//18
            stream.Write((byte)PacketNo2);//20
            stream.Write(TotalMatches);//21
            return stream;
        }
        public static unsafe ServerSockets.Packet AddItemElitePKBrackets(this ServerSockets.Packet stream, MsgTournaments.MsgEliteGroup.Match match)
        {
            if (match == null)
            {
                stream.ZeroFill(160);
                return stream;
            }
            stream.Write(match.ID);

            stream.Write((ushort)match.Players.Count);

            stream.Write(match.Index);

            stream.Write((ushort)match.Flag);
            var array = match.GetMatchStats;

            for (int x = 0; x < 3; x++)
            {
                Game.MsgTournaments.MsgEliteGroup.FighterStats element = null;
                if (array.Length > x)
                {
                    element = array[x];
                }
                if (element == null)
                {
                    stream.ZeroFill(50);
                }
                else
                {
                    stream.Write(element.UID);
                    stream.Write(element.Mesh);
                    stream.Write(element.Name, 32);
                    stream.Write(element.ServerID);
                    stream.Write((uint)element.Flag);
                    stream.Write((ushort)(element.Advanced == true ? 1 : 0));
                }
            }
            return stream;
        }

        public static unsafe ServerSockets.Packet ElitePKBracketsFinalize(this ServerSockets.Packet stream, bool finish = true, int count = 0)
        {
            if (!finish)
            {
                int old = stream.Position;
                stream.Seek(20);
                stream.Write((byte)(0));//Finish
                stream.Write(count);
                stream.Seek(old);
            }
            stream.Finalize(GamePackets.MsgPKEliteMatchInfo);
            return stream;
        }
    }
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct MsgElitePKBrackets
    {
        public enum GuiTyp : ushort
        {
            GUI_Top8Ranking = 0,
            GUI_Knockout = 3,
            GUI_Top8Qualifier = 4,
            GUI_Top4Qualifier = 5,
            GUI_Top2Qualifier = 6,
            GUI_Top3 = 7,
            GUI_Top1 = 8,
            GUI_ReconstructTop = 9
        }
        public enum Action : ushort
        {
            InitialList = 0,
            StaticUpdate = 1,
            GUIEdit = 2,
            UpdateList = 3,
            RequestInformation = 4,
            StopWagers = 5,
            EPK_State = 6
        }

        [PacketAttribute(GamePackets.MsgPKEliteMatchInfo)]
        private static void Poroces(Client.GameClient user, ServerSockets.Packet stream)
        {
            try
            {
                MsgElitePKBrackets.Action mode;
                ushort Page;
                uint MatchCount;
                MsgTournaments.MsgEliteTournament.BPGroupTyp Group;
                MsgElitePKBrackets.GuiTyp guityp;
                ushort TimeLeft;
                uint TotalMatches;
                stream.GetElitePKBrackets(out mode, out Page, out MatchCount, out Group, out guityp, out TimeLeft, out TotalMatches);
                if (mode == Action.RequestInformation)
                {
                    var tournament = MsgTournaments.MsgEliteTournament.EliteGroups[Math.Min(3, (int)Group)];
                    if (tournament.Proces != MsgTournaments.ProcesType.Dead)
                    {
                        if (tournament.Proces == MsgTournaments.ProcesType.Idle)
                        {
                            stream.ElitePKBracketsCreate(mode, Page, MatchCount, Group, guityp, TimeLeft, TotalMatches);

                            user.Send(stream.ElitePKBracketsFinalize());
                            return;
                        }
                        stream.ElitePKBracketsCreate(mode, Page, MatchCount, Group, tournament.State, 1, TotalMatches);
                        user.Send(stream.ElitePKBracketsFinalize());
                    }
                    else
                    {
                        stream.ElitePKBracketsCreate(mode, Page, MatchCount, Group, guityp, 0, TotalMatches);
                        user.Send(stream.ElitePKBracketsFinalize());
                    }
                }
                if (mode == Action.InitialList)
                {
                    var tournament = MsgTournaments.MsgEliteTournament.EliteGroups[Math.Min(3, (int)Group)];
                    if (tournament.Proces != MsgTournaments.ProcesType.Dead)
                    {
                        stream.ElitePKBracketsCreate(mode, Page, MatchCount, Group, tournament.State, 1, TotalMatches);
                        user.Send(stream.ElitePKBracketsFinalize());
                        if (tournament.State >= GuiTyp.GUI_Top4Qualifier)
                        {
                            if (tournament.Matches == null)
                            {
                                if (tournament.FinalMatch != null)
                                {
                                    tournament.SendBrackets(tournament.FinalMatch.GetValues(), user, false, Page, Action.GUIEdit, true);
                                    tournament.SendBrackets(tournament.FinalMatch.GetValues(), user, false, Page, Action.UpdateList, sendmatch: true, PacketNo: (ushort)1);
                                    if (tournament.State == GuiTyp.GUI_ReconstructTop)
                                    {
                                        MsgElitePkRanking.RankType RankType = MsgElitePkRanking.RankType.Top3;
                                        stream.ElitePkRankingCreate(RankType, (uint)Group, tournament.State, 3, user.Player.UID);
                                        for (int i = 0; i < 3; i++)
                                            stream.AddItemElitePkRanking(tournament.Top8[i], (uint)(i + 1));
                                        user.Send(stream.ElitePkRankingFinalize());
                                    }
                                    else
                                    {
                                        MsgElitePkRanking.RankType RankType = MsgElitePkRanking.RankType.Top3;
                                        stream.ElitePkRankingCreate(RankType, (uint)Group, tournament.State, 1, user.Player.UID);
                                        if (tournament.Top8[2] != null)
                                        {
                                            stream.AddItemElitePkRanking(tournament.Top8[2], 3);
                                            user.Send(stream.ElitePkRankingFinalize());
                                        }
                                    }
                                }
                                return;
                            }
                            tournament.SendBrackets(tournament.Matches.GetValues(), user, false, Page, Action.GUIEdit, true);
                            if (tournament.ThreeQualiferMatch != null)
                            {
                                tournament.SendBrackets(tournament.ThreeQualiferMatch.GetValues(), user, false, Page, Action.InitialList, true);
                            }
                            else if (tournament.Top4Matches != null)
                            {
                                if (tournament.FinalMatch != null)
                                    tournament.SendBrackets(tournament.FinalMatch.GetValues(), user, false, Page, Action.InitialList, true);
                                else
                                    tournament.SendBrackets(tournament.Top4Matches.GetValues(), user, false, Page, Action.InitialList, true);
                                if (tournament.State == MsgElitePKBrackets.GuiTyp.GUI_Top1)
                                {
                                    if (tournament.Top8[2] != null)
                                    {
                                        MsgElitePkRanking.RankType RankType = MsgElitePkRanking.RankType.Top3;
                                        stream.ElitePkRankingCreate(RankType, (uint)Group, tournament.State, 1, user.Player.UID);
                                        stream.AddItemElitePkRanking(tournament.Top8[2], 3);
                                        user.Send(stream.ElitePkRankingFinalize());

                                    }
                                }
                                else
                                {
                                    MsgElitePkRanking.RankType RankType = MsgElitePkRanking.RankType.Top3;
                                    stream.ElitePkRankingCreate(RankType, (uint)Group, tournament.State, 3, user.Player.UID);
                                    for (int i = 0; i < 3; i++)
                                        stream.AddItemElitePkRanking(tournament.Top8[i], (uint)(i + 1));
                                    user.Send(stream.ElitePkRankingFinalize());

                                }
                            }
                            else tournament.SendBrackets(tournament.Matches.GetValues(), user, false, Page, Action.InitialList, true);
                        }
                        else
                        {

                            if (tournament.Matches != null)
                            {
                                stream.ElitePKBracketsCreate(Action.GUIEdit, Page, MatchCount, Group, guityp, TimeLeft, TotalMatches);
                                user.Send(stream.ElitePKBracketsFinalize());
                                tournament.SendBrackets(tournament.Matches.GetValues(), user, false, Page, Action.UpdateList, true);
                            }
                        }
                    }
                }

            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
        }
    }
}

