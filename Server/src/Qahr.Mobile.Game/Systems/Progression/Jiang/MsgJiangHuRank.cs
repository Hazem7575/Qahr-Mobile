using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Qahr.Mobile.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static void GetJiangHuRank(this ServerSockets.Packet stream, out ushort Pages)
        {
            stream.ReadUInt8();//4
            Pages = stream.ReadUInt16();//5
        }
        
        public static unsafe ServerSockets.Packet JiangHuRankCreate(this ServerSockets.Packet stream, ushort Page, byte MyRank, byte PageRegistred
            , byte Registred)
        {
            stream.InitWriter();

            stream.Write(Page);//4
            stream.Write(MyRank);//6
            stream.Write(PageRegistred);//7
            stream.Write(Registred);//8


            return stream;
        }
        public static unsafe ServerSockets.Packet AddItemJiangHuRank(this ServerSockets.Packet stream, Role.Instance.JiangHu jiang)
        {

            stream.Write(jiang.Rank);
            stream.Write(jiang.Inner_Strength);
            stream.Write((uint)jiang.Level);
            stream.Write(jiang.Name, 32);
            stream.Write(jiang.CustomizedName, 32);
            // QahrMobile: the mobile OK_PLAYER_INFO_MSG is 81 bytes, it goes on with the look (face icon) at 73 and
            // the uid at 77 (CRankJiangHuItem::SetInfo, CRankBaseItem::SetIsSelf)
            Client.GameClient online;
            stream.Write(Pool.GamePoll.TryGetValue(jiang.UID, out online) ? online.Player.Mesh : 0u);
            stream.Write(jiang.UID);

            return stream;
        }
        public static unsafe ServerSockets.Packet JiangHuRankFinalize(this ServerSockets.Packet stream)
        {
            stream.Finalize(GamePackets.MsgOwnKongRank);
            return stream;
        }

    }
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct MsgJiangHuRank
    {
        [PacketAttribute(GamePackets.MsgOwnKongRank)]
        private static void Process(Client.GameClient user, ServerSockets.Packet stream)
        {


            ushort Pages;
            stream.GetJiangHuRank(out Pages);


            Pages -= 1;
            
            var info = Role.Instance.JiangHu.JiangHuRanking.TopRank100.ToArray();
            if (info != null)
            {
                const int max = 10;
                int offset = Pages * max;
                int count = Math.Min(max, Math.Max(0, info.Length - offset));


                byte myrank = 0;
                if (user.Player.MyJiangHu != null)
                {
                    if (user.Player.MyJiangHu.Rank < 100)
                        myrank = user.Player.MyJiangHu.Rank;
                }
                if (Pages == 0)
                    Pages = 1;
                stream.JiangHuRankCreate((ushort)(Pages * 256), myrank, (byte)count, (byte)Math.Min(100, info.Length));

                for (int x = 0; x < count; x++)
                {
                    if (info.Length > offset + x)
                    {
                        var element = info[offset + x];
                        if (element.Rank < 100)
                        {
                            stream.AddItemJiangHuRank(element);
                        }
                    }
                }
                user.Send(stream.JiangHuRankFinalize());
            }
        }
    }
}
