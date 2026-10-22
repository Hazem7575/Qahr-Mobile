using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet CaptureTheFlagRankingsCreate(this ServerSockets.Packet stream, MsgCaptureTheFlagRankings.ActionID Mode
            , uint Page, uint Dwparam1, uint dwparam2, uint dwparam3, ulong dwparam4)
        {
            stream.InitWriter();
            stream.Write((ushort)Mode);
            stream.Write(Page);//6
            stream.Write(Dwparam1);//10
            stream.Write(dwparam2);//14
            stream.Write((ushort)1);
            stream.Write(dwparam3);//18
            stream.Write(dwparam4);//22
            //stream.Write(count);

            return stream;
        }
        public static unsafe ServerSockets.Packet AddItemCaptureTheFlagRankings(this ServerSockets.Packet stream,
            string name, uint DwParam1, uint DwParam2, ulong DwParam3, uint DwParam4, uint DwParam5)
        {
            stream.Write(name, 72);
            stream.Write(DwParam1);//16
            stream.Write(DwParam2);//20
            stream.Write(DwParam3);//4
            stream.Write(DwParam4);//32
            stream.Write(DwParam5);//32// BaseScore
            return stream;
        }
        public static unsafe ServerSockets.Packet AddItemCaptureTheFlagRankings(this ServerSockets.Packet stream, uint DwPram1,
            ulong DwParam2, string Name, uint ID)
        {

            stream.Write(DwPram1);
            stream.Write(DwParam2);
            stream.Write(Name, 72);
            stream.Write(ID);
            return stream;
        }
        public static unsafe ServerSockets.Packet AddItemCaptureTheFlagRankings(this ServerSockets.Packet stream, uint DwPram1,
         uint DwParam2, uint DwParam3, ulong DwParam4, uint ID, string Name)
        {
            stream.Write(DwPram1);
            stream.Write(DwParam2);
            stream.Write(DwParam3);
            stream.Write(DwParam4);
            stream.Write(ID);
            stream.Write(Name, 72);
            return stream;
        }
        public static unsafe ServerSockets.Packet AddItemCaptureTheFlagRankings(this ServerSockets.Packet stream, string Name, uint DwParam)
        {
            stream.Write(Name, 32);
            stream.Write(DwParam);
            return stream;
        }

        public static unsafe ServerSockets.Packet CaptureTheFlagRankingsFinalize(this ServerSockets.Packet stream)
        {
            stream.Finalize(GamePackets.CaptureTheFlagRankings);

            return stream;
        }
        public static unsafe void GetCaptureTheFlagRankings(this ServerSockets.Packet stream, out  MsgCaptureTheFlagRankings.ActionID mode, out uint Dwparam
            , out uint Dwparam1, out uint dwparam2, out uint dwparam3, out uint dwparam4)
        {
            mode = (MsgCaptureTheFlagRankings.ActionID)stream.ReadUInt16();
            Dwparam = stream.ReadUInt32();
            Dwparam1 = stream.ReadUInt32();
            dwparam2 = stream.ReadUInt32();
            stream.ReadUInt16();
            dwparam3 = stream.ReadUInt32();
            dwparam4 = stream.ReadUInt32();
        }
    }
    public class MsgCaptureTheFlagRankings
    {
        public enum ActionID : ushort
        {
            RewardRanking = 0,
            Info = 1,
            SetRewardConquerPoints = 3,
            SetRewardMoney = 4,
            SetRewardAll = 5,
            RankMembers = 8,
            UpdatePoint = 13,
            ArenaGui = 9

        }
        [PacketAttribute(GamePackets.CaptureTheFlagRankings)]
        private unsafe static void Poroces(Client.GameClient user, ServerSockets.Packet stream)
        {

            MsgCaptureTheFlagRankings.ActionID mode; uint Dwparam;
            uint Dwparam1; uint dwparam2; uint dwparam3; uint dwparam4;
            stream.GetCaptureTheFlagRankings(out mode, out Dwparam, out Dwparam1, out dwparam2, out dwparam3, out dwparam4);
            switch (mode)
            {
                case ActionID.SetRewardConquerPoints:
                    {
                        if (user.Player.MyGuild == null || user.Player.MyGuildMember == null)
                            break;
                        if (user.Player.MyGuildMember.Rank != Role.Flags.GuildMemberRank.GuildLeader)
                            break;

                        if (user.Player.MyGuild.Info.ConquerPointFund >= dwparam3)
                        {
                            user.Player.MyGuild.Info.ConquerPointFund -= dwparam3;
                            user.Player.MyGuild.CTF_Next_ConquerPoints += dwparam3;
                        }
                        else
                        {
#if Arabic
                                user.SendSysMesage("Your guild not have sufficient Conquer Points.");
#else
                            user.SendSysMesage("Your guild not have sufficient Conquer Points.");
#endif

                        }


                        stream.CaptureTheFlagRankingsCreate(mode, 0, 0, 0, user.Player.MyGuild != null ? user.Player.MyGuild.CTF_Next_ConquerPoints : 0
                            , user.Player.MyGuild != null ? user.Player.MyGuild.CTF_Next_Money : 0);
                        stream.CaptureTheFlagRankingsFinalize();
                        user.Send(stream);

                        break;
                    }
                case ActionID.SetRewardMoney:
                    {
                        if (user.Player.MyGuild == null || user.Player.MyGuildMember == null)
                            break;
                        if (user.Player.MyGuildMember.Rank != Role.Flags.GuildMemberRank.GuildLeader)
                            break;

                        if (user.Player.MyGuild.Info.SilverFund >= dwparam4)
                        {
                            user.Player.MyGuild.Info.SilverFund -= dwparam4;
                            user.Player.MyGuild.CTF_Next_Money += dwparam4;
                        }
                        else
                        {
#if Arabic
                             user.SendSysMesage("Your guild not have sufficient Money.");
#else
                            user.SendSysMesage("Your guild not have sufficient Money.");
#endif

                        }

                        stream.CaptureTheFlagRankingsCreate(mode, 0, 0, 0, user.Player.MyGuild != null ? user.Player.MyGuild.CTF_Next_ConquerPoints : 0
                            , user.Player.MyGuild != null ? user.Player.MyGuild.CTF_Next_Money : 0);
                        stream.CaptureTheFlagRankingsFinalize();
                        user.Send(stream);

                        break;
                    }
                case ActionID.SetRewardAll:
                    {
                        if (user.Player.MyGuild == null || user.Player.MyGuildMember == null)
                            break;
                        if (user.Player.MyGuildMember.Rank != Role.Flags.GuildMemberRank.GuildLeader)
                            break;

                        if (user.Player.MyGuild.Info.ConquerPointFund >= dwparam3)
                        {
                            user.Player.MyGuild.Info.ConquerPointFund -= dwparam3;
                            user.Player.MyGuild.CTF_Next_ConquerPoints += dwparam3;
                        }
                        else
                        {
#if Arabic
                               user.SendSysMesage("Your guild not have sufficient Conquer Points.");
#else
                            user.SendSysMesage("Your guild not have sufficient Conquer Points.");
#endif

                        }
                        if (user.Player.MyGuild.Info.SilverFund >= dwparam4)
                        {
                            user.Player.MyGuild.Info.SilverFund -= dwparam4;
                            user.Player.MyGuild.CTF_Next_Money += dwparam4;
                        }
                        else
                        {
#if Arabic
                               user.SendSysMesage("Your guild not have sufficient Money.");
#else
                            user.SendSysMesage("Your guild not have sufficient Money.");
#endif

                        }


                        stream.CaptureTheFlagRankingsCreate(mode, 0, 0, 0, user.Player.MyGuild != null ? user.Player.MyGuild.CTF_Next_ConquerPoints : 0
                            , user.Player.MyGuild != null ? user.Player.MyGuild.CTF_Next_Money : 0);
                        stream.CaptureTheFlagRankingsFinalize();
                        user.Send(stream);

                        break;
                    }
                case ActionID.Info:
                    {
                        if (user.Player.MyGuild == null || user.Player.MyGuildMember == null)
                            break;

                        var array = user.Player.MyGuild.Members.Values.Where(p => p.CTF_Exploits != 0).ToArray();
                        var Rank = array.OrderByDescending(p => p.CTF_Exploits).ToArray();

                        const int max = 5;
                        int offset = (int)(Dwparam1 - 1) * max;
                        int count = Math.Min(max, Math.Max(0, Rank.Length - offset));

                        stream.CaptureTheFlagRankingsCreate(mode, Dwparam, (uint)Rank.Length, (uint)count// echo @6 (panel id)
                               , user.Player.MyGuild != null ? user.Player.MyGuild.CTF_Next_ConquerPoints : 0
                            , user.Player.MyGuild != null ? user.Player.MyGuild.CTF_Next_Money : 0);

                        for (int x = 0; x < count; x++)
                        {
                            if (Rank.Length > offset + x)
                            {
                                var element = Rank[offset + x];

                                stream.AddItemCaptureTheFlagRankings((uint)(offset + x), element.CTF_Exploits, element.RewardConquerPoints, element.RewardMoney, element.UID, element.Name);
                            }
                        }
                        stream.CaptureTheFlagRankingsFinalize();
                        user.Send(stream);

                        break;
                    }
                case ActionID.RankMembers:
                    {
                        if (user.Player.MyGuild == null || user.Player.MyGuildMember == null)
                            break;

                        var array = user.Player.MyGuild.Members.Values.Where(p => p.CTF_Exploits != 0).ToArray();
                        var Rank = array.OrderByDescending(p => p.CTF_Exploits).ToArray();

                        const int max = 5;
                        int offset = (int)(Dwparam1 - 1) * max;
                        int count = Math.Min(max, Math.Max(0, Rank.Length - offset));

                        stream.CaptureTheFlagRankingsCreate(mode, Dwparam, (uint)Rank.Length, (uint)count// echo @6 (panel id)
                            , user.Player.MyGuildMember != null ? user.Player.MyGuildMember.CTF_Exploits : 0, 0);

                        for (int x = 0; x < count; x++)
                        {
                            if (Rank.Length > offset + x)
                            {
                                var element = Rank[offset + x];
                                stream.AddItemCaptureTheFlagRankings(element.Name, element.CTF_Exploits);
                            }
                        }
                        stream.CaptureTheFlagRankingsFinalize();
                        user.Send(stream);

                        break;
                    }
                case ActionID.RewardRanking:
                    {
                        var array = Role.Instance.Guild.GuildPoll.Values.Where(p => p.CTF_Next_ConquerPoints != 0).OrderByDescending(p => p.CTF_Next_ConquerPoints).ToArray();

                        const int max = 5;
                        int offset = (int)(Dwparam1 - 1) * max;
                        int count = Math.Min(max, Math.Max(0, array.Length - offset));

                        stream.CaptureTheFlagRankingsCreate(mode, Dwparam, (uint)array.Length, (uint)(count)// echo @6 (panel id)
                            , user.Player.MyGuild != null ? user.Player.MyGuild.CTF_Next_ConquerPoints : 0
                            , user.Player.MyGuild != null ? user.Player.MyGuild.CTF_Next_Money : 0);

                        for (int x = 0; x < count; x++)
                        {
                            if (array.Length > offset + x)
                            {
                                var element = array[offset + x];
                                stream.AddItemCaptureTheFlagRankings(element.CTF_Next_ConquerPoints, element.CTF_Next_Money, element.GuildName, element.Info.GuildID);
                            }
                        }
                        stream.CaptureTheFlagRankingsFinalize();
                        user.Send(stream);


                        break;
                    }
                case ActionID.ArenaGui:
                    {
                        if (MsgTournaments.MsgSchedules.CaptureTheFlag.Proces == MsgTournaments.ProcesType.Alive)
                        {
                            var array = MsgTournaments.MsgSchedules.CaptureTheFlag.RegistredGuilds.Values.Where(p => p.CTF_Exploits != 0).OrderByDescending(p => p.CTF_Exploits).ToArray();

                            stream.CaptureTheFlagRankingsCreate(mode, 1, 0, (uint)Math.Min(array.Length, 8), user.Player.MyGuild != null ? user.Player.MyGuild.CTF_Exploits : 0, 0);
                            for (int x = 0; x < Math.Min(array.Length, 8); x++)
                            {
                                var element = array[x];
                                stream.AddItemCaptureTheFlagRankings(element.GuildName, element.CTF_Exploits, (uint)element.Members.Count, element.CTF_Next_Money, element.CTF_Next_ConquerPoints, 0);
                            }
                            stream.CaptureTheFlagRankingsFinalize();
                            user.Send(stream);
                        }
                        else
                        {
                            var array = Role.Instance.Guild.GuildPoll.Values.Where(p => p.CTF_Exploits != 0).OrderByDescending(p => p.CTF_Exploits).ToArray();

                            stream.CaptureTheFlagRankingsCreate(mode, 1, 0, (uint)Math.Min(array.Length, 8), user.Player.MyGuild != null ? user.Player.MyGuild.CTF_Exploits : 0, 0);

                            for (int x = 0; x < Math.Min(array.Length, 8); x++)
                            {
                                var element = array[x];
                                uint[] reward = element.GetLeaderReward();
                                stream.AddItemCaptureTheFlagRankings(element.GuildName, element.CTF_Exploits, (uint)element.Members.Count, (uint)reward[0], (uint)reward[1], 0);
                            }
                            stream.CaptureTheFlagRankingsFinalize();
                            user.Send(stream);
                        }
                        break;
                    }
            }

        }

    }
}
