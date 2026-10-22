using Qahr.Mobile.Client;
using ProtoBuf;
using System;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {

        public static void GetExchangeShopGoodsInfo(this ServerSockets.Packet stream, out MsgExchangeShopGoodsInfo pQuery)
        {
            pQuery = new MsgExchangeShopGoodsInfo();
            pQuery = stream.ProtoBufferDeserialize<MsgExchangeShopGoodsInfo>(pQuery);
        }
        public static unsafe ServerSockets.Packet CreateExchangeShop(this ServerSockets.Packet stream, MsgExchangeShopGoodsInfo obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgExchangeShop);

            return stream;
        }
    }
    [ProtoContract]
    public class MsgExchangeShopGoodsInfo
    {
        public enum _Type : byte
        {
            Refresh = 1,
            Show = 2,
            ShowRebate = 3,
        }
        [ProtoMember(1, IsRequired = true)]
        public uint Type;
        [ProtoMember(2, IsRequired = true)]
        public uint NpcID;
        [ProtoMember(3, IsRequired = true)]
        public uint ShopID;

        [PacketAttribute(GamePackets.MsgExchangeShop)]
        private static void Process2(GameClient user, ServerSockets.Packet stream)
        {
            MsgExchangeShopGoodsInfo pQuery;
            stream.GetExchangeShopGoodsInfo(out pQuery);
            switch (pQuery.Type)
            {
                case (byte)_Type.Refresh:
                    {
                        user.CreateBoxDialog("Sorry, This System Is Closed.");
                        break;
                    }
                //if (pQuery.ShopID == 168)
                //{
                //    //if (user.Player.MyDontion >= 50)
                //    //{
                //    //    if (pQuery.ShopID == 168)
                //    //    {
                //    //        user.MyExchangeShop.GetRandomEx(168, 8);
                //    //        user.MyExchangeShop.Open(stream, pQuery.NpcID, 168);
                //    //        user.Player.MyDontion -= 50;
                //    //        user.Player.SendUpdate(stream, (long)user.Player.MyDontion, MsgUpdate.DataType.MyDontion);

                //    //    }

                //    }
                //}
                //    if (pQuery.ShopID == 137)
                //    {
                //        if (user.Inventory.Contain(3322775, 1))
                //        {
                //            user.Inventory.Remove(3322775, 1, stream);
                //            user.MyExchangeShop.GetRandomReb(137, 9);
                //            user.MyExchangeShop.OpenReb(stream, pQuery.NpcID, 137);
                //        }
                //    }
                //    break;
                //}
                case (byte)_Type.Show:
                    {
                        user.CreateBoxDialog("Sorry, This System Is Closed.");
                        break;
                    }

                //#region ShopRebate
                //if (pQuery.NpcID == 24702)
                //{
                //    user.MyExchangeShop.OpenReb(stream, pQuery.NpcID, 137);
                //}
                //#endregion

                //#region ShopGuild
                //if (pQuery.NpcID == 26523)
                //{
                //    user.Player.SendUpdate(stream, (long)user.Player.MyDontion, MsgUpdate.DataType.MyDontion);
                //    user.MyExchangeShop.Open(stream, pQuery.NpcID, 168);
                //}
                //#endregion

                //#region ShopLottery
                //if (pQuery.NpcID == 29261)
                //{
                //    user.MyExchangeShop.Open(stream, pQuery.NpcID, pQuery.ShopID);
                //}
                //#endregion
                //case (byte)_Type.ShowRebate:
                //    {

                //        pQuery.ShopID = 13;
                //        user.Send(stream.CreateExchangeShop(pQuery));
                //        break;
                //    }
            }
        }


    }
}
