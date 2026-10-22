using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet GuildInformationCreate(this ServerSockets.Packet stream, MsgGuildInformation info,int MembersCount)
        {
            stream.InitWriter();


            stream.InitWriter();
            stream.Write(info.GuildID);//4
            stream.Write(info.SilverFund);//8
            stream.Write(info.ConquerPointFund);//16
            stream.Write(MembersCount);//20
            stream.Write(info.MyRank);
            stream.Write(info.LeaderName, 32);
            stream.Write((ushort)info.Level);
            stream.Write((byte)0);
            stream.Write(info.CreateTime);
            stream.Write(info.Leaderid);
            stream.Write(info.Material);
            stream.Write(info.RecruitmentOFF);
            stream.Write(info.Recruitment_Battle_Power);
            stream.Write(info.RecruitmentON);
            stream.Write(info.Prestige);
            stream.Write((uint)info.Recruitment_Flag);
            stream.Finalize(GamePackets.MsgSyndicateAttributeInfo);
            return stream;

        }
    }
    public class MsgGuildInformation
    {
        public uint GuildID;
        public long SilverFund;
        public uint ConquerPointFund;
        public uint MyRank;
        public uint Level;
        public uint CreateTime;
        public string LeaderName;
        public uint Leaderid;
        public uint ArsenalBP;
        public ulong Material;
        public ulong Prestige;
        public Role.Instance.Guild.ClassFlag Recruitment_Flag;
        public uint Recruitment_Battle_Power;
        public bool RecruitmentON;
        public bool RecruitmentOFF;
        public uint HonorPillar;
        public static MsgGuildInformation Create()
        {
            MsgGuildInformation packet = new MsgGuildInformation();
            return packet;
        }
    }
}
