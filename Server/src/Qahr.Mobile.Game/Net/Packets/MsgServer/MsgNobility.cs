using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    #region The Class Edit By Mohy ElDin ...
    public unsafe static partial class MsgBuilder
    {
        public static unsafe void GetNobility(this ServerSockets.Packet stream, out MsgNobility.NobilityAction mode, out ulong UID, out MsgNobility.DonationTyp donationtyp)
        {
            mode = (MsgNobility.NobilityAction)stream.ReadInt32();
            UID = stream.ReadUInt64();
            stream.SeekForward(88);
            donationtyp = (MsgNobility.DonationTyp)stream.ReadUInt8();
        }
        public static unsafe ServerSockets.Packet NobilityIconCreate(this ServerSockets.Packet stream, Role.Instance.Nobility nobility)
        {
            stream.InitWriter();
            stream.Write((uint)MsgNobility.NobilityAction.Icon);//4
            stream.Write(nobility.UID);//8
            string StrList = "" + nobility.UID + " " + nobility.Donation + " " + (byte)nobility.Rank + " " + nobility.Position + "";
            stream.ZeroFill(108);
            stream.Write(StrList);
            stream.Finalize(GamePackets.NewMsgNobility);
            return stream;
        }
    }
    public unsafe struct MsgNobility
    {
        public enum NobilityAction : uint
        {
            Donate = 1,
            RankListen = 2,
            Icon = 3,
            NobilityInformarion = 4,
        }
        public enum DonationTyp : byte
        {
            Money = 0,
            ConquerPoints = 1,
            //BoundCps = 2
        }
        [PacketAttribute(GamePackets.NewMsgNobility)]
        public unsafe static void HandlerNobility(Client.GameClient user, ServerSockets.Packet stream)
        {
            NobilityAction Action;
            ulong UID;
            DonationTyp donationtyp;
            stream.GetNobility(out Action, out UID, out donationtyp);

            switch (Action)
            {
                case NobilityAction.Donate:
                    {
                        if (!user.Player.OnMyOwnServer)
                            return;

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

                        switch (donationtyp)
                        {
                            case DonationTyp.Money:
                                {

                                    if (!user.Player.OnMyOwnServer)
                                        return;

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

                                    if (UID < 3000000)
                                    {
                                        user.SendSysMesage("You need to donate at least 3,000,000 silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                                        break;
                                    }

                                    if (user.Player.Money >= (ulong)UID)
                                    {
                                        user.Player.Money -= (ulong)UID;
                                        user.Player.SendUpdate(stream, user.Player.Money, MsgUpdate.DataType.Money);
                                        user.Player.Nobility.Donation += UID;
                                        user.Send(stream.NobilityIconCreate(user.Player.Nobility));
                                        Pool.NobilityRanking.UpdateRank(user.Player.Nobility);
                                    }
                                    else
                                    {
                                        user.SendSysMesage("you don't have enough Money to donate , or less number to donate.", MsgMessage.ChatMode.System);
                                        return;
                                    }
                                    break;
                                }
                            case DonationTyp.ConquerPoints:
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

                                    user.CreateBoxDialog("You Can Only Donate Money.");
                                    break;
                                }
                        }
                        break;
                    }
                case NobilityAction.RankListen:
                    {
                        int page = (int)UID;
                        var info = Pool.NobilityRanking.GetArray();
                        try
                        {
                            const int max = 10;
                            int offset = page * max;
                            int count = Math.Min(max, Math.Max(0, info.Length - offset));
                            // real row count and page count (only the top 50 are listed)
                            int rows = 0;
                            for (int x = 0; x < count; x++)
                                if (info[offset + x].Position < 50)
                                    rows++;
                            int pages = (Math.Min(50, info.Length) + max - 1) / max;

                            stream.InitWriter();

                            stream.Write((uint)NobilityAction.RankListen);
                            stream.Write((ushort)page);
                            stream.Write((ushort)pages);
                            stream.Write((ushort)rows);
                            stream.ZeroFill(102);
                            stream.Write((uint)1);
                            for (int x = 0; x < count; x++)
                            {
                                if (info.Length > offset + x)
                                {
                                    var element = info[offset + x];
                                    if (element.Position < 50)
                                    {
                                        // QahrMobile: the mobile PEERAGE_SORT_INFO is 72 bytes (CPlayer::AddContributeRankInfo):
                                        // uid 0, lookface 8, name 20 (32), donation u64 56, rank 64, position 68
                                        stream.Write(element.UID);
                                        stream.Write((uint)0);
                                        stream.Write(element.Mesh);
                                        stream.ZeroFill(8);
                                        stream.Write(element.Name, 32);
                                        stream.ZeroFill(4);
                                        stream.Write(element.Donation);
                                        stream.Write((uint)element.Rank);
                                        stream.Write((uint)element.Position);
                                    }
                                }
                            }
                            stream.Finalize(GamePackets.NewMsgNobility);

                            user.Send(stream);
                        }
                        catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
                        break;
                    }
                case NobilityAction.NobilityInformarion:
                    {
                        stream.InitWriter();
                        stream.Write((uint)NobilityAction.NobilityInformarion);
                        stream.Write(Pool.NobilityRanking.KnightDonation);
                        stream.Write(uint.MaxValue);
                        stream.Write((uint)Role.Instance.Nobility.NobilityRank.Knight);
                        stream.Write(Pool.NobilityRanking.BaronDonation);
                        stream.Write(uint.MaxValue);
                        stream.Write((uint)Role.Instance.Nobility.NobilityRank.Baron);
                        stream.Write(Pool.NobilityRanking.EarlDonation);
                        stream.Write(uint.MaxValue);
                        stream.Write((uint)Role.Instance.Nobility.NobilityRank.Earl);
                        stream.Write(Pool.NobilityRanking.DukeDonation);
                        stream.Write(uint.MaxValue);
                        stream.Write((uint)Role.Instance.Nobility.NobilityRank.Duke);
                        stream.Write(Pool.NobilityRanking.PrinceDonation);
                        stream.Write(uint.MaxValue);
                        stream.Write((uint)Role.Instance.Nobility.NobilityRank.Prince);
                        stream.Write(Pool.NobilityRanking.KingDonation);
                        stream.Write(uint.MaxValue);
                        stream.Write((uint)Role.Instance.Nobility.NobilityRank.King);
                        stream.Finalize(GamePackets.NewMsgNobility);
                        user.Send(stream);
                        break;
                    }

            }

        }
    }
}
    #endregion
