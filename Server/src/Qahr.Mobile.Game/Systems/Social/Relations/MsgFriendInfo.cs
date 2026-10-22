using Qahr.Mobile.Client;
using Qahr.Mobile.Database;
using Qahr.Mobile.Role.Instance;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class MsgFriendInfo
    {
        [ProtoContract]
        public class FriendProtoStructure
        {
            [ProtoMember(1, IsRequired = true)]//0 
            public uint Type;

            [ProtoMember(2, IsRequired = true)]//1 
            public uint unk1;

            [ProtoMember(3, IsRequired = true)]
            public List<ConstructsProto> Items;

            [ProtoContract]
            public class ConstructsProto
            {
                [ProtoMember(1, IsRequired = true)]
                public uint UID = 0;//5700932 

                [ProtoMember(2, IsRequired = true)]
                public uint Online = 0;//1 offline 0 

                [ProtoMember(3, IsRequired = true)]
                public uint Mesh = 0;//1551008 //GuildFlag Class 8192 

                [ProtoMember(4, IsRequired = true)]
                public uint Level = 0;//9 

                [ProtoMember(5, IsRequired = true)]
                public uint Class = 0;//14022 

                [ProtoMember(6, IsRequired = true)]
                public string Name;//PIrateaa 

                [ProtoMember(7, IsRequired = true)]
                public string Spouse;//NOMATE_NAME@@ 

                [ProtoMember(8, IsRequired = true)]
                public string Description;//Here 

                [ProtoMember(9, IsRequired = true)]
                public uint GuildID = 0;//12504 

                // [ProtoFix 2026-10-14] client CUserInfoPB #10 is a string (was uint, never filled). Meaning unknown -> send empty.
                [ProtoMember(10, IsRequired = true)]
                public string unk8 = string.Empty;

                [ProtoMember(11, IsRequired = true)]
                public Role.Flags.GuildMemberRank GuildRank;

                [ProtoMember(12, IsRequired = true)]
                public uint PkPoints = 0;//0 
            }

        }
        public static unsafe ServerSockets.Packet CreateFriendInfo(this ServerSockets.Packet stream, FriendProtoStructure obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgRelationInfo);
            return stream;

        }
        public static unsafe ServerSockets.Packet CreateViewRelationInfo(this ServerSockets.Packet stream, Client.GameClient Target, bool enemy)
        {
            if (enemy == true)
            {
                stream.InitWriter();
                var Info = new MsgFriendInfo.FriendProtoStructure() { Type = 3, unk1 = 1, Items = new List<MsgFriendInfo.FriendProtoStructure.ConstructsProto>() };
                Info.Items.Add(new MsgFriendInfo.FriendProtoStructure.ConstructsProto()
                {
                    UID = Target.Player.UID,
                    Online = 1,
                    Mesh = Target.Player.Mesh,
                    Level = Target.Player.Level,
                    Class = Target.Player.Class,
                    Name = Target.Player.Name,
                    Spouse = Target.Player.Spouse,
                    GuildID = Target.Player.GuildID,
                    Description = Target.Player.Description,
                    GuildRank = Target.Player.GuildRank,
                    PkPoints = Target.Player.PKPoints

                });
                stream.ProtoBufferSerialize(Info);
                stream.Finalize(GamePackets.MsgRelationInfo);
                return stream;
            }
            else
            {
                stream.InitWriter();
                var Info = new MsgFriendInfo.FriendProtoStructure() { Type = 2, unk1 = 1, Items = new List<MsgFriendInfo.FriendProtoStructure.ConstructsProto>() };
                Info.Items.Add(new MsgFriendInfo.FriendProtoStructure.ConstructsProto()
                {
                    UID = Target.Player.UID,
                    Online = 1,
                    Mesh = Target.Player.Mesh,
                    Level = Target.Player.Level,
                    Class = Target.Player.Class,
                    Name = Target.Player.Name,
                    Spouse = Target.Player.Spouse,
                    GuildID = Target.Player.GuildID,
                    Description = Target.Player.Description,
                    GuildRank = Target.Player.GuildRank,
                    PkPoints = Target.Player.PKPoints

                });
                stream.ProtoBufferSerialize(Info);
                stream.Finalize(GamePackets.MsgRelationInfo);
                return stream;
            }

        }
        public static void GetFriendInfo(this ServerSockets.Packet stream, out FriendProtoStructure pQuery)
        {
            pQuery = new FriendProtoStructure();
            pQuery = stream.ProtoBufferDeserialize<FriendProtoStructure>(pQuery);
        }
    }
}
