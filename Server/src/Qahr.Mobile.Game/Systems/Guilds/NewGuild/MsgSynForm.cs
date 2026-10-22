
using Qahr.Mobile.Client;
using Qahr.Mobile.Database;
using Qahr.Mobile.Game.MsgMonster;
using Qahr.Mobile.Role;
using Qahr.Mobile.Role.Instance;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class MsgSynForm
    {
        [ProtoContract]
        public class MsgGuildConstructInfo
        {
            public enum _Type : byte
            {
                Info = 0,
                DonateMaterial = 1,
                Upgrade = 2,
                Magic = 3,
                Enhance = 5,
                Create = 6,
                Load = 7,
            }

            public enum _FormType : byte
            {
                Keeper = 1,
                DrillHall = 2,
                Gate = 3,
                Beast = 4,
                Arsenal = 5,
                Center = 6,
            }
            [ProtoMember(1, IsRequired = true)]
            public uint Type;
            [ProtoMember(2, IsRequired = true)]
            public uint FormType;
            [ProtoMember(3, IsRequired = true)]
            public uint Beast;
            [ProtoMember(4)]
            public uint TypeUpgrade;
            [ProtoMember(5)]
            public ulong Money;
            [ProtoMember(6)]
            public uint TypeMoney;
            [ProtoMember(7)]
            public ulong DonateMaterial;
            [ProtoMember(8)]
            public ulong OK;


        }


        public static void GetGuildConstructInfo(this ServerSockets.Packet stream, out MsgSynForm.MsgGuildConstructInfo pQuery)
        {
            pQuery = new MsgSynForm.MsgGuildConstructInfo();
            pQuery = stream.ProtoBufferDeserialize<MsgSynForm.MsgGuildConstructInfo>(pQuery);
        }
        public static unsafe ServerSockets.Packet CreateGuildConstructInfo(this ServerSockets.Packet stream, MsgSynForm.MsgGuildConstructInfo obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgSynForm);
            return stream;
        }
        [PacketAttribute(GamePackets.MsgSynForm)]
        private unsafe static void Process(Client.GameClient user, ServerSockets.Packet stream)
        {
            if (user.IsVendor)
            {
                user.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                return;
            }
            if (user.InTrade)
            {
                user.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                return;
            }

            MsgSynForm.MsgGuildConstructInfo Info = (MsgSynForm.MsgGuildConstructInfo)null;
            stream.GetGuildConstructInfo(out Info);
            switch ((byte)Info.Type)
            {
                case 0:
                    {
                        if (user.Player.MyGuild == null || user.Player.MyGuildMember == null)
                            break;
                        Guild.Construct construct1 = user.Player.MyGuild.Constructs.Values.Where<Guild.Construct>((Func<Guild.Construct, bool>)(p => (int)p.ID == (int)Info.FormType && (long)p.Beast == (long)Info.Beast)).FirstOrDefault<Guild.Construct>();
                        if (construct1 == null)
                            break;
                        MsgSynFormInfo.MsgGuildConstruct msgGuildConstruct1 = new MsgSynFormInfo.MsgGuildConstruct();
                        msgGuildConstruct1.Material = user.Player.MyGuild.Info.Material;
                        msgGuildConstruct1.construct = new MsgSynFormInfo.MsgGuildConstruct.Construct[1];
                        msgGuildConstruct1.construct[0] = new MsgSynFormInfo.MsgGuildConstruct.Construct(construct1);
                        user.Send(stream.CreateGuildConstruct(msgGuildConstruct1));
                        break;
                    }
                case 1:
                    {
                        if (user.Player.MyGuild == null || user.Player.MyGuildMember == null)
                            break;
                        if (Info.TypeMoney == 1)
                        {
                            if ((ulong)user.Player.ConquerPoints >= Info.DonateMaterial)
                            {
                                user.Player.ConquerPoints -= (uint)Info.DonateMaterial;
                                user.Player.MyGuild.Info.Material += Info.DonateMaterial;
                                user.Player.MyDontion += (uint)Info.DonateMaterial;
                                user.Player.MyGuildMember.MoneyDonate += (long)Info.DonateMaterial;
                                user.Player.MyGuild.SendThat(user.Player);
                                user.Player.MyGuild.AddMessage("STR_SYNDICATE_EVENT_DONATE_GOODS@@" + user.Player.Name + "@@" + Info.DonateMaterial.ToString() + "@@");
                            }
                        }
                        // compare by division so DonateMaterial * 100000 cannot overflow
                        else if (Info.TypeMoney == 0 && Info.DonateMaterial > 0 && Info.DonateMaterial <= uint.MaxValue && Info.DonateMaterial <= user.Player.Money / 100000)
                        {
                            user.Player.Money -= (ulong)Info.DonateMaterial * 100000;
                            user.Player.MyGuild.Info.Material += Info.DonateMaterial;
                            user.Player.MyDontion += (uint)Info.DonateMaterial;
                            user.Player.MyGuildMember.MoneyDonate += (long)Info.DonateMaterial;
                            user.Player.MyGuild.SendThat(user.Player);
                            user.Player.MyGuild.AddMessage("STR_SYNDICATE_EVENT_DONATE_GOODS@@" + user.Player.Name + "@@" + Info.DonateMaterial.ToString() + "@@");
                        }
                        Info.OK = 1;
                        user.Send(stream.CreateGuildConstructInfo(Info));
                        break;
                    }
                case 2:
                    {
                        if (user.Player.MyGuild == null || user.Player.MyGuildMember == null)
                            break;
                        Guild.Construct construct2 = user.Player.MyGuild.Constructs.Values.Where<Guild.Construct>((Func<Guild.Construct, bool>)(p => (int)p.ID == (int)Info.FormType && (long)p.Beast == (long)Info.Beast)).FirstOrDefault<Guild.Construct>();
                        syn_formtype.formtype form;
                        if (construct2 == null || !syn_formtype.TryGetValue(Info.FormType * 100 + Info.Beast, construct2.Level, out form) || construct2 == null)
                            break;
                        switch (Info.TypeUpgrade)
                        {
                            case 0:
                                {
                                    // spending guild Material is for the leader/deputy only
                                    if (user.Player.MyGuildMember.Rank != Role.Flags.GuildMemberRank.GuildLeader
                                        && user.Player.MyGuildMember.Rank != Role.Flags.GuildMemberRank.DeputyLeader)
                                        break;
                                    if (Info.Money <= ulong.MaxValue / 100000 && user.Player.MyGuild.Info.Material >= Info.Money)
                                    {
                                        user.Player.MyGuild.Info.Material -= Info.Money;
                                        construct2.Exp += Info.Money * 100000;
                                        if (construct2.Exp >= form.EXP)
                                        {
                                            ++construct2.Level;
                                            construct2.Exp -= form.EXP;
                                            user.Player.MyGuild.AddMessage("STR_SYNDICATE_EVENT_UPGRADE_BUILDING@@" + ((MsgSynForm.MsgGuildConstructInfo._FormType)Info.FormType).ToString() + "@@" + construct2.Level.ToString() + "@@");
                                        }
                                        break;
                                    }
                                    break;

                                }
                            case 1:
                                {
                                    if (user.Player.Money >= (ulong)Info.Money)
                                    {
                                        user.Player.Money -= (ulong)Info.Money;
                                        construct2.Exp += Info.Money;
                                        if (construct2.Exp >= form.EXP)
                                        {
                                            ++construct2.Level;
                                            construct2.Exp -= form.EXP;
                                            user.Player.MyGuild.AddMessage("STR_SYNDICATE_EVENT_UPGRADE_BUILDING@@" + ((MsgSynForm.MsgGuildConstructInfo._FormType)Info.FormType).ToString() + "@@" + construct2.Level.ToString() + "@@");
                                        }
                                        break;
                                    }
                                    break;
                                }
                        }
                        MsgSynFormInfo.MsgGuildConstruct msgGuildConstruct2 = new MsgSynFormInfo.MsgGuildConstruct();
                        msgGuildConstruct2.Material = user.Player.MyGuild.Info.Material;
                        msgGuildConstruct2.construct = new MsgSynFormInfo.MsgGuildConstruct.Construct[1];
                        msgGuildConstruct2.construct[0] = new MsgSynFormInfo.MsgGuildConstruct.Construct(construct2);
                        user.Send(stream.CreateGuildConstruct(msgGuildConstruct2));
                        if (construct2.ID == 6)
                            user.Player.MyGuild.Info.Level = construct2.Level;
                        if (construct2.ID == 5)
                        {
                            user.Player.MyGuild.Info.ArsenalBP = construct2.Level;
                            foreach (Role.Instance.Guild.Member member in user.Player.MyGuild.Members.Values)
                            {
                                GameClient client;
                                if (Pool.GamePoll.TryGetValue(member.UID, out client))
                                    client.Player.GuildBattlePower = user.Player.MyGuild.Info.ArsenalBP;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        if (user.Player.Map != 1038 || user.Player.MyGuild == null)
                            break;
                        NewPetGuild.Death(user, stream);
                        Guild.Construct construct3 = user.Player.MyGuild.Constructs.Values.Where<Guild.Construct>((Func<Guild.Construct, bool>)(p => (int)p.ID == (int)Info.FormType && (long)p.Beast == (long)Info.Beast)).FirstOrDefault<Guild.Construct>();
                        if (construct3 == null)
                            break;
                        uint data = (uint)construct3.Data.Data;
                        switch (Info.Beast)
                        {
                            case 0:
                                NewPetGuild.AddPet(user, data, 4, user.Player.X, user.Player.Y, stream);
                                break;
                            case 1:
                                NewPetGuild.AddPet(user, data, 5, user.Player.X, user.Player.Y, stream);
                                user.Player.AddFlag(MsgUpdate.Flags.LotusDemon, 2592000, false);
                                break;
                            case 2:
                                NewPetGuild.AddPet(user, data, 6, user.Player.X, user.Player.Y, stream);
                                user.Player.AddFlag(MsgUpdate.Flags.SierraBeast, 2592000, false);
                                break;
                        }
                        break;

                    }
                case 7:
                    {
                        if (user.Player.MyGuild == null)
                            break;
                        int count = user.Player.MyGuild.Constructs.Values.Count;
                        MsgSynFormInfo.MsgGuildConstruct msgGuildConstruct3 = new MsgSynFormInfo.MsgGuildConstruct();
                        msgGuildConstruct3.Type = 4U;
                        msgGuildConstruct3.Material = user.Player.MyGuild.Info.Material;
                        msgGuildConstruct3.construct = new MsgSynFormInfo.MsgGuildConstruct.Construct[count];
                        for (int index = 0; index < count; ++index)
                            msgGuildConstruct3.construct[index] = new MsgSynFormInfo.MsgGuildConstruct.Construct(user.Player.MyGuild.Constructs.Values.ToArray<Guild.Construct>()[index]);
                        user.Send(stream.CreateGuildConstruct(msgGuildConstruct3));
                        user.Send(stream.CreateGuildConstructInfo(Info));
                        break;
                    }
            }
        }
    }
}


