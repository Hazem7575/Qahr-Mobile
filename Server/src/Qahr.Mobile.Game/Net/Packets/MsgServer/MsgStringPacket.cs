using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        [ProtoContract]
        public class MsgStringProto
        {
            [ProtoMember(2)]
            public uint UID;//d
            [ProtoMember(3)]
            public ushort X;//d
            [ProtoMember(4)]
            public ushort Y;//d
            [ProtoMember(5)]
            public MsgStringPacket.StringID ID;
            [ProtoMember(6)]
            public string[] Strings;
        }


        public static unsafe void GetStringPacket(this ServerSockets.Packet stream, out MsgStringPacket str)
        {
            str = new MsgStringPacket();

            var proto = stream.ProtoBufferDeserialize<MsgStringProto>(new MsgStringProto());
            str.UID = proto.UID;
            str.ID = proto.ID;
            str.Strings = proto.Strings;
        }

        public static unsafe ServerSockets.Packet StringPacketCreate(this ServerSockets.Packet stream, MsgStringPacket pac)
        {
            stream.InitWriter();
            var data = new MsgStringProto()
            {
                UID = pac.UID,
                //ID = (uint)pac.ID,
                X = pac.X,
                Y = pac.Y,
                Strings = pac.Strings,
                ID = pac.ID,
                //Life = pac.Life,
            };
            //stream.InitWriter();



            //if (pac.ID == MsgStringPacket.StringID.LocationEffect)
            //{
            //    stream.Write(pac.X);
            //    stream.Write(pac.Y);
            //}
            //else
            //    stream.Write(pac.UID);

            //stream.Write((byte)pac.ID);

            //if (pac.Strings != null)
            //{
            //    stream.Write(pac.Strings);
            //}
            //else
            //    stream.ZeroFill(3);

            stream.ProtoBufferSerialize(data);
            stream.Finalize(GamePackets.MsgName);
            return stream;
        }
    }
    public unsafe class MsgStringPacket
    {
        public enum StringID : byte
        {
            Fireworks = 1,
            GuildName = 3,
            Spouse = 6,
            LocationEffect = 9,
            Effect = 10,
            GuildList = 11,
            Unknown = 13,
            ViewEquipSpouse = 16,
            StartGamble = 17,
            EndGamble = 18,
            Sound = 20,
            GuildAllies = 21,
            GuildEnemies = 22,
            WhisperDetails = 26,
            ServerName = 61

        }

        public string[] Strings;
        public uint UID;
        public ushort X;
        public ushort Y;
        public byte StringsLength;
        public StringID ID;


        [PacketAttribute(GamePackets.MsgName)]
        private static void Handler(Client.GameClient user, ServerSockets.Packet stream)
        {
            MsgStringPacket Str;

            stream.GetStringPacket(out Str);

            switch (Str.ID)
            {

                case StringID.GuildName:
                    {

                        if (user.InTrade) return;
                        // only the guild leader may rename the guild
                        if (user.Player.MyGuild == null || user.Player.MyGuildMember == null
                            || user.Player.MyGuildMember.Rank != Role.Flags.GuildMemberRank.GuildLeader)
                            return;
                        if (Str.Strings == null || Str.Strings.Length == 0) return;
                        string guildname = Str.Strings[0];
                        if (user.Player.ConquerPoints >= 1075)
                        {
                            if (Role.Instance.Guild.AllowToCreate(guildname))
                            {
                                user.Player.ConquerPoints -= 1075;
                                Role.Instance.Guild.RegisterChangeName(user.Player.MyGuild.GuildID, guildname);
                                user.Player.MyGuild.GuildName = guildname;
                                foreach (var client in Pool.GamePoll.Values.Where(u => u.Player.GuildID == user.Player.MyGuild.GuildID))
                                {
                                    client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                }
                                foreach (var guild in Role.Instance.Guild.GuildPoll.Values)
                                {
                                    if (guild.Ally.ContainsKey(user.Player.MyGuild.GuildID))
                                    {
                                        guild.Ally[user.Player.MyGuild.GuildID].GuildName = guildname;

                                        guild.SendGuildAlly(stream, false, null);
                                        user.Player.MyGuild.SendGuildAlly(stream, false, null);
                                    }
                                    if (guild.Enemy.ContainsKey(user.Player.MyGuild.GuildID))
                                    {
                                        guild.Enemy[user.Player.MyGuild.GuildID].GuildName = guildname;

                                        guild.SendGuilEnnemy(stream, false, null);
                                        user.Player.MyGuild.SendGuilEnnemy(stream, false, null);
                                    }
                                }
                                user.Player.SendString(stream, MsgStringPacket.StringID.GuildName, user.Player.MyGuild.Info.GuildID, true, user.Player.MyGuild.GuildName + " " + user.Player.MyGuild.Info.LeaderName + " " + user.Player.MyGuild.Info.Level.ToString() + " " + user.Player.MyGuild.Members.Count.ToString() + " 83");
                                user.Player.MyGuild.SendThat(user.Player);

                            }
                            else
                            {
                                user.CreateBoxDialog("Failed~to~change~name~Guild.~The~name~has~been~used~or~you~use~invalid~characters.");
                            }
                        }
                        else
                        {
                            user.CreateBoxDialog("You~Dont~Have~1075~Cps");
                        }
                        break;
                    }
                case (StringID)29: Phone.MobileWhisperQuery.Handle(user, stream, Str); break; // QahrMobile: the mobile's player menu query (tap a player)
                case StringID.ViewEquipSpouse:
                    {
                        Client.GameClient viewClient;
                        if (Pool.GamePoll.TryGetValue(Str.UID, out viewClient))
                        {
                            Str.Strings = new string[1];
                            Str.Strings[0] = viewClient.Player.Spouse;
                            Str.StringsLength = (byte)viewClient.Player.Spouse.Length;
                            Str.UID = viewClient.Player.UID;
                            user.Send(stream.StringPacketCreate(Str));
                        }
                        break;
                    }
                case StringID.WhisperDetails:
                    {

                        foreach (var Target in Pool.GamePoll.Values)
                        {
                            if (Target.Player.Name == Str.Strings[0])
                            {
                                Str.Strings = new string[2];
                                Str.Strings[0] = Target.Player.Name;
                                Str.StringsLength = 0;
                                Str.StringsLength += (byte)Target.Player.Name.Length;
                                string otherstring = "";
                                otherstring += Target.Player.UID + " ";
                                otherstring += Target.Player.Level + " ";
                                otherstring += Target.Player.BattlePower + " #";
                                if (Target.Player.MyGuild != null)
                                {
                                    otherstring += Target.Player.MyGuild.GuildName + " # ";
                                }
                                else
                                    otherstring += " # ";
                                otherstring += Target.Player.Spouse + " ";
                                otherstring += (byte)(Target.Player.NobilityRank) + " ";
                                //1 girl
                                //2 = boy

                                if (Target.Player.Body % 10 == 7)
                                    otherstring += "2";
                                else
                                    otherstring += "1";
                                Str.StringsLength += (byte)otherstring.Length;
                                Str.Strings[1] = otherstring;
                                user.Send(stream.StringPacketCreate(Str));
                            }
                        }
                        break;
                    }
            }


        }

    }
}
