using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {

    }
    public class MsgClientInfo
    {
        [ProtoContract]
        public class ProtoStructure
        {
            [ProtoMember(1, IsRequired = true)]
            public uint UID;

            [ProtoMember(2, IsRequired = true)]
            public uint AparenceType;

            [ProtoMember(3, IsRequired = true)]
            public uint Mesh;

            [ProtoMember(4, IsRequired = true)]
            public uint Hair;

            [ProtoMember(5, IsRequired = true)]
            public ulong Money;

            [ProtoMember(6, IsRequired = true)]
            public uint ConquerPoints;

            [ProtoMember(7, IsRequired = true)]
            public long Experience;

            [ProtoMember(8, IsRequired = true)]
            public ushort IDServer;

            [ProtoMember(9, IsRequired = true)]
            public long SetLocationType;

            [ProtoMember(10, IsRequired = true)]
            public long SpecialTitleID;

            [ProtoMember(11, IsRequired = true)]
            public long VirtutePoints;

            [ProtoMember(12, IsRequired = true)]
            public long HeavenBlessing;

            [ProtoMember(13, IsRequired = true)]
            public uint Strength;

            [ProtoMember(14, IsRequired = true)]
            public uint Agility;

            [ProtoMember(15, IsRequired = true)]
            public uint Vitality;

            [ProtoMember(16, IsRequired = true)]
            public uint Spirit;

            [ProtoMember(17, IsRequired = true)]
            public uint Atributes;

            [ProtoMember(18, IsRequired = true)]
            public uint HitPoints; // [ProtoFix 2026-10-14] client CMsgUserInfoPB #18 is uint32 (was int)

            [ProtoMember(19, IsRequired = true)]
            public uint Mana;

            [ProtoMember(20, IsRequired = true)]
            public uint PKPoints;

            [ProtoMember(21, IsRequired = true)]
            public ushort Level;

            [ProtoMember(22, IsRequired = true)]
            public uint Class;

            [ProtoMember(23, IsRequired = true)]
            public uint FirstClass;

            [ProtoMember(24, IsRequired = true)]
            public uint SecoundeClass;

            [ProtoMember(25, IsRequired = true)]
            public ulong NobilityRank;

            [ProtoMember(26, IsRequired = true)]
            public byte Reborn;

            [ProtoMember(27, IsRequired = true)]
            public long autoallot;

            [ProtoMember(28, IsRequired = true)]
            public uint QuizPoints;

            [ProtoMember(29, IsRequired = true)]
            public ulong MainFlag;

            [ProtoMember(30, IsRequired = true)]
            public long Enilghten;

            [ProtoMember(31, IsRequired = true)]
            public long EnlightenReceive;

            [ProtoMember(32, IsRequired = true)]
            public long Mentorachieve;

            [ProtoMember(33, IsRequired = true)]
            public long MentorDay;

            [ProtoMember(34, IsRequired = true)]
            public uint VipLevel;

            [ProtoMember(35, IsRequired = true)]
            public ushort MyTitle;

            [ProtoMember(36, IsRequired = true)]
            public uint BoundConquerPoints; // [ProtoFix 2026-10-14] client CMsgUserInfoPB #36 is uint32 (was int)

            [ProtoMember(37, IsRequired = true)]
            public ulong ActiveSublass;

            [ProtoMember(38, IsRequired = true)]
            public ulong SubClass;

            [ProtoMember(39, IsRequired = true)]
            public long RacePoints;

            [ProtoMember(40, IsRequired = true)]
            public long CountryID;

            [ProtoMember(41, IsRequired = true)]
            public long LeagueContributionLev;

            [ProtoMember(42, IsRequired = true)]
            public long SoldIdentity;

            [ProtoMember(43, IsRequired = true)]
            public long ProfExperience;

            [ProtoMember(44, IsRequired = true)]
            public long RegisterDay;

            [ProtoMember(45, IsRequired = true)]
            public long FaceFrame;

            [ProtoMember(46, IsRequired = true)]
            public string Names;

            [ProtoMember(47, IsRequired = true)]
            public string Spouse;

            [ProtoMember(48, IsRequired = true)]
            public long MaxFloor;

            [ProtoMember(49, IsRequired = true)]
            public long AccountFlag;

            [ProtoMember(50, IsRequired = true)]
            public string Description;

            [ProtoMember(51, IsRequired = true)]
            public ushort FrameID;

            [ProtoMember(52, IsRequired = true)]
            public ushort idFBLookFace;
        }
    }
    public unsafe static partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet ClientInfo(this ServerSockets.Packet stream, Role.Player client, int inittransfer = 0)
        {
            stream.InitWriter();
            var proto = new MsgClientInfo.ProtoStructure()
            {
                UID = client.UID,
                AparenceType = client.AparenceType,
                Mesh = client.Mesh,
                Hair = client.Hair,
                Money = client.Money,
                ConquerPoints = (uint)client.ConquerPoints,
                IDServer = (ushort)Database.GroupServerList.MyServerInfo.ID,
                SetLocationType = client.SetLocationType,
                SpecialTitleID = client.SpecialTitleID,
                VirtutePoints = client.VirtutePoints,
                HeavenBlessing = client.HeavenBlessing,
                Strength = client.Strength,
                Agility = client.Agility,
                Vitality = client.Vitality,
                Spirit = client.Spirit,
                Atributes = client.Atributes,
                HitPoints = (uint)System.Math.Max(0, client.HitPoints), // [ProtoFix 2026-10-14] clamp: a negative int would go out as a 10-byte varint
                Mana = client.Mana,
                PKPoints = client.PKPoints,
                Level = client.Level,
                Class = client.Class,
                FirstClass = client.FirstClass,
                SecoundeClass = client.SecoundeClass,
                NobilityRank = (ulong)client.NobilityRank,
                Reborn = client.Reborn,
                autoallot= inittransfer,
                QuizPoints = client.QuizPoints,
                MainFlag = (ulong)client.MainFlag,
                Enilghten = client.Enilghten,
                EnlightenReceive = client.EnlightenReceive,
                VipLevel = client.VipLevel,
                MyTitle = client.MyTitle,
                BoundConquerPoints = (uint)System.Math.Max(0, client.BoundConquerPoints), // [ProtoFix 2026-10-14] clamp negative
                SubClass = (ulong)client.SubClass.GetHashPoint(),
                ActiveSublass = (ulong)client.ActiveSublass,
                RacePoints = client.RacePoints,
                CountryID = client.CountryID,
                LeagueContributionLev = 0,
                SoldIdentity = 28433125,
                //ProfExperience = 2400,
                RegisterDay = 4638,
                FaceFrame = client.Face,
                Names = client.Name,
                Spouse = client.Spouse,
                MaxFloor = 0,
                AccountFlag = 0,
                Description = client.Description,
                FrameID= client.FrameID,
            };
            stream.ProtoBufferSerialize(proto);
            stream.Finalize(GamePackets.MsgUserInfo);
            return stream;
        }
        public static void GetHeroInfo(this ServerSockets.Packet stream, Client.GameClient Owner, out MsgClientInfo.ProtoStructure pQuery, out Role.Player user)
        {
            pQuery = new MsgClientInfo.ProtoStructure();
            pQuery = stream.ProtoBufferDeserialize<MsgClientInfo.ProtoStructure>(pQuery);
            user = new Role.Player(Owner);
            user.InitTransfer = (uint)pQuery.autoallot;
            user.RealUID = pQuery.UID;
            user.AparenceType = pQuery.AparenceType;

            user.Body = (ushort)(pQuery.Mesh % 10000);
            user.Face = (ushort)(pQuery.Mesh / 10000);
            user.Hair = pQuery.Hair;
            user.Money = pQuery.Money;
            user.ConquerPoints = (uint)pQuery.ConquerPoints;
            user.Experience = (ulong)pQuery.Experience;
            user.ServerID = pQuery.IDServer;
            user.SetLocationType = (ushort)pQuery.SetLocationType;
            user.SpecialTitleID = 0;
            user.SpecialWingID = 0;
            user.VirtutePoints = (uint)pQuery.VirtutePoints;
            user.HeavenBlessing = stream.ReadInt32();
            user.Strength = (ushort)pQuery.Strength;
            user.Agility = (ushort)pQuery.Agility;
            user.Vitality = (ushort)pQuery.Vitality;
            user.Spirit = (ushort)pQuery.Spirit;
            user.Atributes = (ushort)pQuery.Atributes;
            user.HitPoints = (int)System.Math.Min(pQuery.HitPoints, (uint)int.MaxValue); // [ProtoFix 2026-10-14] uint -> int without wrap
            user.Mana = (ushort)pQuery.Mana;
            user.PKPoints = (ushort)pQuery.PKPoints;
            user.Level = pQuery.Level;
            user.Class = pQuery.Class;
            user.FirstClass = pQuery.FirstClass;
            user.SecoundeClass = pQuery.SecoundeClass;
            user.NobilityRank = (Role.Instance.Nobility.NobilityRank)pQuery.NobilityRank;
            user.Reborn = pQuery.Reborn;
            user.QuizPoints = pQuery.QuizPoints;
            user.MainFlag = (Qahr.Mobile.Role.Player.MainFlagType)pQuery.MainFlag;
            user.Enilghten = (ushort)pQuery.Enilghten;
            user.EnlightenReceive = (ushort)pQuery.EnlightenReceive;
            user.VipLevel = (byte)pQuery.VipLevel;
            user.MyTitle = (byte)pQuery.MyTitle;
            user.BoundConquerPoints = (int)System.Math.Min(pQuery.BoundConquerPoints, (uint)int.MaxValue); // [ProtoFix 2026-10-14] uint -> int without wrap
            user.CountryID = (ushort)pQuery.CountryID;
            user.Face = (ushort)pQuery.FaceFrame;
            user.Name = pQuery.Names;
            user.Spouse = pQuery.Spouse;
            user.Description = pQuery.Description;
        }
       
        public static unsafe ServerSockets.Packet LoaderHeroInfo(this ServerSockets.Packet stream, Role.Player client, byte inittransfer = 0)
        {
            stream.InitWriter();
            stream.Write(client.UID);
            stream.Write(client.Mesh);
            stream.Write((ushort)Database.GroupServerList.MyServerInfo.ID);
            stream.Write((byte)client.Level);
            stream.Write((byte)client.NobilityRank);
            stream.Write((byte)client.VipLevel);
            stream.Write((uint)client.Class);
            stream.Write((ushort)0);
            stream.Write(client.Name);
            stream.Finalize(GamePackets.MsgUserInfo * 10);
            return stream;
        }

    }

}
