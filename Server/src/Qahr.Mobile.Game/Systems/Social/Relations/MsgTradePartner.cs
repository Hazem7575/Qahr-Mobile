using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{


    public unsafe static class MsgTradePartner
    {
        public enum Action : byte
        {
            RequestPartnership = 0,
            RejectRequest = 1,
            AddOnline = 2,
            AddOffline = 3,
            BreakPartnership = 4,
            AddPartner = 5
        }

        public static unsafe ServerSockets.Packet TradePartnerInfoCreate(this ServerSockets.Packet stream, Client.GameClient user)
        {
            stream.InitWriter();

            var Info = new MsgFriendInfo.FriendProtoStructure();
            Info.Type = 4;
            Info.unk1 = 1;
            Info.Items = new List<MsgFriendInfo.FriendProtoStructure.ConstructsProto>();
            Info.Items.Add(new MsgFriendInfo.FriendProtoStructure.ConstructsProto()
            {
                UID = user.Player.UID,
                Online = 1,
                Mesh = user.Player.Mesh,
                Level = user.Player.Level,
                Class = user.Player.Class,
                Name = user.Player.Name,
                Spouse = user.Player.Spouse,
                GuildID = user.Player.GuildID,
                Description = user.Player.Description,
                GuildRank = user.Player.GuildRank,
                PkPoints = user.Player.PKPoints

            });
            stream.ProtoBufferSerialize(Info);
            stream.Finalize(GamePackets.MsgRelationInfo);

            return stream;
        }
        public static unsafe void GetTradePartner(this ServerSockets.Packet stream, out uint UID, out Action mode, out bool online, out int HoursLeft)
        {
            UID = stream.ReadUInt32();
            mode = (Action)stream.ReadInt8();
            online = stream.ReadInt8() == 1;
            HoursLeft = stream.ReadInt32();
        }

        public static unsafe ServerSockets.Packet TradePartnerCreate(this ServerSockets.Packet stream, uint UID, Action Typ, bool online, int HoursLeft, string Name, ushort Level, string Description, uint Face)
        {
            stream.InitWriter();

            stream.Write(UID);//4
            stream.Write((byte)Typ);//8
            stream.Write((byte)(online == true ? 1 : 0));//9
            stream.Write(HoursLeft);//10
            stream.Write((long)0);//14
            stream.Write((ushort)Face);//22Face
            stream.Write((uint)Level);//24
            stream.Write(Description, 32);//28
            stream.ZeroFill(32);//60
            stream.Write(Name, 32);//92

            stream.Finalize(GamePackets.TradePartner);
            return stream;
        }

        [PacketAttribute(GamePackets.TradePartner)]
        public unsafe static void HandlerTradePartner(Client.GameClient user, ServerSockets.Packet stream)
        {
            uint UID;
            Action Mode;
            bool Online;
            int HoursLeft;
            stream.GetTradePartner(out UID, out Mode, out Online, out HoursLeft);


            switch (Mode)
            {
                case Action.RequestPartnership:
                    {
                        if (user.Player.Associate.AllowAdd(Role.Instance.Associate.Partener, UID, 10))
                        {
                            Role.IMapObj obj;
                            if (user.Player.View.TryGetValue(UID, out obj, Role.MapObjectType.Player))
                            {
                                Role.Player player = obj as Role.Player;

                                user.Player.TradePartner = UID;
                                if (player.TradePartner == user.Player.UID)
                                {
                                    if (user.Player.Associate.AllowAdd(Role.Instance.Associate.Partener, player.UID, 10))
                                    {
                                        if (player.Associate.AllowAdd(Role.Instance.Associate.Partener, user.Player.UID, 10))
                                        {
                                            user.Player.Associate.AddPartener(user, player);
                                            player.Associate.AddPartener(player.Owner, user.Player);

                                            HoursLeft = (int)(new TimeSpan(DateTime.Now.AddDays(3).Ticks).TotalMinutes - new TimeSpan(DateTime.Now.Ticks).TotalMinutes);

                                            player.Owner.Send(stream.TradePartnerCreate(user.Player.UID, Action.AddPartner, true, HoursLeft, user.Player.Name, user.Player.Level, user.Player.Description, user.Player.Mesh));
                                            user.Send(stream.TradePartnerCreate(player.UID, Action.AddPartner, true, HoursLeft, player.Name, player.Level, player.Description, player.Mesh));
                                        }
                                    }

                                }
                                else
                                {
                                    player.Owner.Send(stream.RelationCreate(user.Player, player));
                                    player.Owner.Send(stream.TradePartnerCreate(user.Player.UID, Action.RequestPartnership, true, HoursLeft, user.Player.Name, user.Player.Level, user.Player.Description, user.Player.Mesh));
                                }
                            }
                        }

                        break;
                    }
                case Action.RejectRequest:
                    {
                        user.Player.TradePartner = 0;
                        Role.IMapObj obj;
                        if (user.Player.View.TryGetValue(UID, out obj, Role.MapObjectType.Player))
                        {
                            Role.Player player = obj as Role.Player;

                            player.Owner.Send(stream.TradePartnerCreate(user.Player.UID, Action.RejectRequest, true, HoursLeft, user.Player.Name, user.Player.Level, user.Player.Description, user.Player.Mesh));
                        }
                        break;
                    }
                case Action.BreakPartnership:
                    {
                        if (user.Player.Associate.Remove(Role.Instance.Associate.Partener, UID))
                        {
                            user.Send(stream.TradePartnerCreate(UID, Action.BreakPartnership, Online, HoursLeft, "", user.Player.Level, "", user.Player.Mesh));


                            Client.GameClient target;
                            if (Pool.GamePoll.TryGetValue(UID, out target))
                            {
                                if (target.Player.Associate.Remove(Role.Instance.Associate.Partener, user.Player.UID))
                                {
                                    target.Send(stream.TradePartnerCreate(user.Player.UID, Action.BreakPartnership, Online, HoursLeft, "", user.Player.Level, "", user.Player.Mesh));
                                }
                            }
                            else
                                Role.Instance.Associate.RemoveOffline(Role.Instance.Associate.Partener, UID, user.Player.UID);
                        }
                        break;
                    }

            }

        }
    }
}
