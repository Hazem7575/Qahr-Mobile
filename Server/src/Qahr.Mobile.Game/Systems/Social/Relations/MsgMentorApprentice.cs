using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static class MsgMentorApprentice
    {
        public enum Action : uint
        {
            RequestApprentice = 1,
            RequestMentor = 2,
            LeaveMentor = 3,
            ExpellApprentice = 4,
            AcceptRequestApprentice = 8,
            AcceptRequestMentor = 9,
            DumpApprentice = 18,
            DumpMentor = 19
        }
        public static unsafe void GetMentorApprentice(this ServerSockets.Packet stream, out Action Mode, out uint UID, out uint dwParam, out uint ShareBattlePower, out bool Online)
        {
            Mode = (Action)stream.ReadUInt32();
            UID = stream.ReadUInt32();
            dwParam = stream.ReadUInt32();
            ShareBattlePower = stream.ReadUInt32();
            Online = stream.ReadInt8() == 1;
        }
        public static unsafe ServerSockets.Packet MentorApprenticeCreate(this ServerSockets.Packet stream, Action Mode, uint UID, uint dwParam, uint ShareBattlePower, bool Online, string Name)
        {
            stream.InitWriter();

            stream.Write((uint)Mode);
            stream.Write(UID);//4
            stream.Write(dwParam);//8
            stream.Write(ShareBattlePower);
            stream.Write((byte)(Online == true ? 1 : 0));
            stream.Write((byte)Name.Length);
            stream.Write(Name, 32);

            stream.ZeroFill(1);

            stream.Finalize(GamePackets.MsgGuide);
            return stream;
        }


        [PacketAttribute(GamePackets.MsgGuide)]
        public unsafe static void HandlerMentorAndApprentice(Client.GameClient user, ServerSockets.Packet stream)
        {
            Action Mode;
            uint UID;
            uint dwParam;
            uint ShareBattlePower;
            bool Online;
            stream.GetMentorApprentice(out Mode, out UID, out dwParam, out ShareBattlePower, out Online);

            switch (Mode)
            {
                case Action.LeaveMentor:
                    {
                        if (user.Player.MyMentor != null)
                        {
                            if (Pool.GamePoll.ContainsKey(user.Player.MyMentor.MyUID))
                            {
                                Client.GameClient remover;
                                if (user.Player.MyMentor.OnlineApprentice.TryRemove(user.Player.UID, out remover))
                                {
                                    user.Send(stream.MentorApprenticeCreate(Action.DumpMentor, UID, dwParam, 0, false, ""));

                                    if (user.Player.MyMentor.MyClient != null)
                                    {

                                        user.Player.MyMentor.MyClient.Send(stream.MentorApprenticeCreate(Action.DumpApprentice, UID, dwParam, ShareBattlePower, false, user.Player.Name));
                                    }
                                    user.Player.Associate.Remove(Role.Instance.Associate.Mentor, user.Player.MyMentor.MyUID);
                                    user.Player.MyMentor.Remove(Role.Instance.Associate.Apprentice, user.Player.UID);
                                    user.Player.SetMentorBattlePowers(0, 0);
                                    user.Player.MyMentor = null;
                                }
                            }
                            else
                            {
                                user.Send(stream.MentorApprenticeCreate(Action.DumpMentor, UID, dwParam, 0, false, ""));

                                Role.Instance.Associate.RemoveOffline(Role.Instance.Associate.Apprentice, user.Player.MyMentor.MyUID, user.Player.UID);
                                user.Player.Associate.Remove(Role.Instance.Associate.Mentor, user.Player.MyMentor.MyUID);
                                user.Player.MyMentor = null;
                            }
                        }
                        break;
                    }
                case Action.ExpellApprentice:
                    {
                        Client.GameClient pClient;
                        if (Pool.GamePoll.TryGetValue(dwParam, out pClient))
                        {

                            pClient.Send(stream.MentorApprenticeCreate(Action.DumpMentor, UID, dwParam, ShareBattlePower, false, ""));

                            if (pClient.Player.MyMentor != null)
                            {
                                Client.GameClient remover;
                                pClient.Player.MyMentor.OnlineApprentice.TryRemove(pClient.Player.UID, out remover);
                                user.Player.Associate.Remove(Role.Instance.Associate.Apprentice, pClient.Player.UID);
                                pClient.Player.Associate.Remove(Role.Instance.Associate.Mentor, user.Player.UID);
                            }
                            user.Player.Associate.Remove(Role.Instance.Associate.Apprentice, dwParam);
                            pClient.Player.MyMentor = null;
                        }
                        else
                        {

                            Role.Instance.Associate.RemoveOffline(Role.Instance.Associate.Mentor, dwParam, user.Player.UID);
                            user.Player.Associate.Remove(Role.Instance.Associate.Apprentice, dwParam);
                        }

                        break;
                    }
                case Action.RequestMentor:
                    {
                        Role.IMapObj obj;
                        if (user.Player.View.TryGetValue(dwParam, out obj, Role.MapObjectType.Player))
                        {
                            Role.Player player = obj as Role.Player;
                            player.Owner.Send(stream.MentorApprenticeCreate(Action.AcceptRequestMentor, user.Player.UID, player.UID, (uint)user.Player.BattlePower, true, user.Player.Name));
                            player.Owner.Send(stream.RelationCreate(user.Player, player));

                        }
                        break;
                    }
                case Action.RequestApprentice:
                    {
                        Role.IMapObj obj;
                        if (user.Player.View.TryGetValue(dwParam, out obj, Role.MapObjectType.Player))
                        {
                            Role.Player player = obj as Role.Player;
                            player.Owner.Send(stream.MentorApprenticeCreate(Action.AcceptRequestApprentice, user.Player.UID, player.UID, (uint)user.Player.BattlePower, true, user.Player.Name));
                            player.Owner.Send(stream.RelationCreate(user.Player, player));
                        }
                        break;
                    }
                case Action.AcceptRequestApprentice:
                    {
                        Role.IMapObj obj;
                        if (user.Player.View.TryGetValue(UID, out obj, Role.MapObjectType.Player))
                        {
                            Role.Player target = obj as Role.Player;
                            if (ShareBattlePower == 1)
                            {
                                if (target.Associate.AllowAdd(Role.Instance.Associate.Apprentice, user.Player.UID, 10))
                                {
                                    if (user.Player.Associate.AllowAdd(Role.Instance.Associate.Mentor, target.UID, 1))
                                    {
                                        user.Player.Associate.AddMentor(user, target);
                                        target.Associate.AddAprrentice(target.Owner, user.Player);

                                        uint EnroleDate = (uint)(DateTime.Now.Year * 10000 + DateTime.Now.Month * 100 + DateTime.Now.Day);

                                        user.Player.MyMentor = target.Associate;

                                        user.Player.SetMentorBattlePowers((uint)target.GetShareBattlePowers((uint)user.Player.RealBattlePower), (uint)target.RealBattlePower);

                                        MsgApprenticeInformation Information = MsgApprenticeInformation.Create();
                                        Information.Mode = MsgApprenticeInformation.Action.Mentor;
                                        Information.Mentor_ID = target.UID;
                                        Information.Apprentice_ID = user.Player.UID;
                                        Information.Enrole_date = EnroleDate;
                                        Information.Level = (byte)target.Level;
                                        Information.Class = target.Class;
                                        Information.PkPoints = target.PKPoints;
                                        Information.Mesh = target.Mesh;
                                        Information.Online = 1;
                                        Information.Shared_Battle_Power = target.GetShareBattlePowers((uint)user.Player.RealBattlePower);
                                        Information.WriteString(target.Name, user.Player.Spouse, user.Player.Name);
                                        user.Send(Information.GetArray(stream));


                                        Information.Mode = MsgApprenticeInformation.Action.Apprentice;
                                        Information.Mentor_ID = target.UID;
                                        Information.Apprentice_ID = user.Player.UID;

                                        Information.Enrole_date = EnroleDate;
                                        Information.Level = (byte)user.Player.Level;
                                        Information.Class = user.Player.Class;
                                        Information.PkPoints = user.Player.PKPoints;
                                        Information.Mesh = user.Player.Mesh;
                                        Information.Online = 1;
                                        Information.WriteString(target.Name, user.Player.Spouse, user.Player.Name);
                                        target.Owner.Send(Information.GetArray(stream));


                                    }
                                }
                            }
                            else
                            {
#if Arabic
                                    target.Owner.SendSysMesage(user.Player.Name + " declined your request.");
#else
                                target.Owner.SendSysMesage(user.Player.Name + " declined your request.");
#endif

                            }
                        }
                        break;
                    }
                case Action.AcceptRequestMentor:
                    {

                        Role.IMapObj obj;
                        if (user.Player.View.TryGetValue(UID, out obj, Role.MapObjectType.Player))
                        {
                            Role.Player target = obj as Role.Player;
                            if (ShareBattlePower == 1)
                            {
                                if (target.Associate.AllowAdd(Role.Instance.Associate.Mentor, user.Player.UID, 1))
                                {
                                    if (user.Player.Associate.AllowAdd(Role.Instance.Associate.Apprentice, target.UID, 10))
                                    {
                                        user.Player.Associate.AddAprrentice(user, target);
                                        target.Associate.AddMentor(target.Owner, user.Player);

                                        uint EnroleDate = (uint)(DateTime.Now.Year * 10000 + DateTime.Now.Month * 100 + DateTime.Now.Day);

                                        target.MyMentor = user.Player.Associate;
                                        target.SetMentorBattlePowers(user.Player.GetShareBattlePowers((uint)target.RealBattlePower), (uint)user.Player.RealBattlePower);

                                        MsgApprenticeInformation Information = MsgApprenticeInformation.Create();
                                        Information.Mode = MsgApprenticeInformation.Action.Mentor;
                                        Information.Mentor_ID = user.Player.UID;
                                        Information.Apprentice_ID = target.UID;
                                        Information.Enrole_date = EnroleDate;
                                        Information.Level = (byte)user.Player.Level;
                                        Information.Class = user.Player.Class;
                                        Information.PkPoints = user.Player.PKPoints;
                                        Information.Mesh = user.Player.Mesh;
                                        Information.Online = 1;
                                        Information.Shared_Battle_Power = user.Player.GetShareBattlePowers((uint)target.RealBattlePower);
                                        Information.WriteString(user.Player.Name, target.Spouse, target.Name);
                                        target.Owner.Send(Information.GetArray(stream));

                                        Information.Mode = MsgApprenticeInformation.Action.Apprentice;
                                        Information.Mentor_ID = user.Player.UID;
                                        Information.Apprentice_ID = target.UID;
                                        Information.Enrole_date = EnroleDate;
                                        Information.Level = (byte)target.Level;
                                        Information.Class = target.Class;
                                        Information.PkPoints = target.PKPoints;
                                        Information.Mesh = target.Mesh;
                                        Information.Online = 1;
                                        Information.WriteString(user.Player.Name, target.Spouse, target.Name);
                                        user.Send(Information.GetArray(stream));


                                    }
                                }
                            }
                            else
                            {
#if Arabic
                                    target.Owner.SendSysMesage(user.Player.Name + " declined your request.");
#else
                                target.Owner.SendSysMesage(user.Player.Name + " declined your request.");
#endif

                            }
                        }

                        break;
                    }
            }
        }
    }
}
