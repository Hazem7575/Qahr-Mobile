using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Qahr.Mobile.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe void GetTeamElitePKBrackets(this ServerSockets.Packet stream, out MsgTeamElitePKBrackets.Action mode, out ushort Page, out ushort packetno
            , out uint MatchCount, out MsgTournaments.MsgEliteTournament.GroupTyp Group, out MsgTeamElitePKBrackets.GuiTyp guityp
            , out ushort TimeLeft, out uint TotalMatches, bool skillTeam)//MsgTeamPKMatchInfo
        {
            mode = (MsgTeamElitePKBrackets.Action)stream.ReadUInt16();//4
            Page = stream.ReadUInt16();//6
            packetno = stream.ReadUInt16();//8
            MatchCount = stream.ReadUInt32();//10
            Group = (MsgTournaments.MsgEliteTournament.GroupTyp)stream.ReadUInt16();//14
            guityp = (MsgTeamElitePKBrackets.GuiTyp)stream.ReadUInt16();//16
            TimeLeft = stream.ReadUInt16();//18
            if (!skillTeam)
            {
                stream.ReadUInt8();//20
                TotalMatches = stream.ReadUInt32();//21
            }
            else
            {
                TotalMatches = stream.ReadUInt32();//20
                stream.ReadUInt8();//24
            }
        }

        public static unsafe ServerSockets.Packet AddItemTeamElitePKBrackets(this ServerSockets.Packet Writer, MsgTournaments.MsgTeamEliteGroup.Match match, bool skillTeam)
        {
         
            Writer.Write(match.ID);
            Writer.Write((ushort)match.Teams.Count);//29
            Writer.Write(match.Index);//31
            Writer.Write((ushort)match.Flag);//33
            var array = match.GetMatchStats;//35
            for (int x = 0; x < 3; x++)
            {
                Qahr.Mobile.Game.MsgTournaments.MsgTeamEliteGroup.FighterStats element = null;
                if (array.Length > x)
                {
                    element = array[x];
                }
                if (element == null)
                {
                    if (!skillTeam)
                        Writer.Write(0u);
                    Writer.Write("", 80);
                }
                else
                {
                    if (!skillTeam)
                        Writer.Write(0u);
                    Writer.Write(element.UID);//35
                    Writer.Write(element.LeaderUID);//39
                    Writer.Write(element.LeaderMesh);//43
                    Writer.Write(element.Name, 64);//47
                    Writer.Write((uint)element.Flag == 0 ? 1 : (uint)element.Flag);//111
                }
            }
            return Writer;
        }
        public static unsafe ServerSockets.Packet MsgTeamEliteBracketsCreate(this ServerSockets.Packet stream, MsgTeamElitePKBrackets.Action mode, ushort Page,
            ushort PacketNo,
             uint MatchCount, MsgTournaments.MsgEliteTournament.GroupTyp Group, MsgTeamElitePKBrackets.GuiTyp guityp
            , ushort TimeLeft, uint TotalMatches, bool skillTeam, ushort PacketNo2)
        {
            stream.InitWriter();

            stream.Write((ushort)mode);//4
            stream.Write(Page);//6
            stream.Write((ushort)PacketNo);//8
            stream.Write(MatchCount);//10
            stream.Write((ushort)Group);//14
            stream.Write((ushort)guityp);//16
            stream.Write(TimeLeft);//18
            if (!skillTeam)
            {
                stream.Write((byte)PacketNo2);//20
                stream.Write(TotalMatches);//21
            }
            else
            {
                stream.Write(TotalMatches);//20
                stream.Write((byte)PacketNo2);//24
            }

            return stream;
        }
        public static unsafe ServerSockets.Packet TeamElitePKBracketsFinalize(this ServerSockets.Packet stream, ushort ID)
        {
            stream.Finalize(ID);
            return stream;
        }
    }

    public unsafe struct MsgTeamElitePKBrackets
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
            EPK_State = 6
        }

        [PacketAttribute(GamePackets.MsgTeamPKMatchInfo)]
        private static void PorocesTeamPk(Client.GameClient user, ServerSockets.Packet stream)
        {
            try
            {
                MsgTeamElitePKBrackets.Action mode;
                ushort Page;
                ushort packetno;
                uint MatchCount;

                MsgTournaments.MsgEliteTournament.GroupTyp Group;
                MsgTeamElitePKBrackets.GuiTyp guityp;
                ushort TimeLeft;
                uint TotalMatches;

                stream.GetTeamElitePKBrackets(out mode, out Page, out packetno, out MatchCount, out Group, out guityp, out TimeLeft, out TotalMatches, false);
                stream.MsgTeamEliteBracketsCreate(mode, Page, packetno, MatchCount, Group, guityp, TimeLeft, TotalMatches, false, 0);
                user.Send(stream.TeamElitePKBracketsFinalize(GamePackets.MsgTeamPKMatchInfo));
                if (mode == Action.RequestInformation || mode == Action.InitialList || mode == Action.UpdateList)
                {
                    var tournament = MsgTournaments.MsgTeamPkTournament.EliteGroups[Math.Min(3, (int)Group)];
                    if (tournament.Proces != MsgTournaments.ProcesType.Dead)
                    {
                        if (tournament.State >= GuiTyp.GUI_Top4Qualifier)
                        {
                            if (tournament.Matches != null)
                            {
                                tournament.SendBrackets(tournament.Matches.GetValues(), user, false, Page, Action.GUIEdit, true);
                                if (tournament.State >= GuiTyp.GUI_Top2Qualifier)
                                    tournament.SendBrackets(tournament.ArrayMatchesTop3(), user, false, 1, Action.GUIEdit, true);
                            }
                            else
                                tournament.SendBrackets(tournament.ArrayMatchesTop3(), user, false, 0, Action.GUIEdit, true);
                        }
                        else
                        {
                            if (tournament.Matches != null)
                                tournament.SendBrackets(tournament.Matches.GetValues(), user, false, Page, Action.UpdateList, true);
                        }
                    }
                }
            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }

        }
        [PacketAttribute(GamePackets.MsgTeamPopPKMatchInfo)]
        private static void PorocesSkillTeam(Client.GameClient user, ServerSockets.Packet stream)
        {
            try
            {
                MsgTeamElitePKBrackets.Action mode;
                ushort Page;
                ushort packetno;
                uint MatchCount;

                MsgTournaments.MsgEliteTournament.GroupTyp Group;
                MsgTeamElitePKBrackets.GuiTyp guityp;
                ushort TimeLeft;
                uint TotalMatches;

                stream.GetTeamElitePKBrackets(out mode, out Page, out packetno, out MatchCount, out Group, out guityp, out TimeLeft, out TotalMatches, true);

                stream.MsgTeamEliteBracketsCreate(mode, Page, packetno, MatchCount, Group, guityp, TimeLeft, TotalMatches, true, 0);

                user.Send(stream.TeamElitePKBracketsFinalize(GamePackets.MsgTeamPopPKMatchInfo));

                if (mode == Action.RequestInformation || mode == Action.InitialList || mode == Action.UpdateList)
                {
                    var tournament = MsgTournaments.MsgSkillTeamPkTournament.EliteGroups[Math.Min(3, (int)Group)];
                    if (tournament.Proces != MsgTournaments.ProcesType.Dead)
                    {
                        if (tournament.State >= GuiTyp.GUI_Top4Qualifier)
                        {
                            if (tournament.Matches != null)
                            {
                                tournament.SendBrackets(tournament.Matches.GetValues(), user, false, Page, Action.GUIEdit, true);
                                if (tournament.State >= GuiTyp.GUI_Top2Qualifier)
                                {
                                    tournament.SendBrackets(tournament.ArrayMatchesTop3(), user, false, 1, Action.GUIEdit, true);
                                }
                            }
                            else
                                tournament.SendBrackets(tournament.ArrayMatchesTop3(), user, false, 0, Action.GUIEdit, true);
                        }
                        else
                        {
                            if (tournament.Matches != null)
                                tournament.SendBrackets(tournament.Matches.GetValues(), user, false, Page, Action.UpdateList, true);
                        }
                    }
                }
            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }

        }
    }
}
