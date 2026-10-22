
using Qahr.Mobile.Client;
using Qahr.Mobile.ServerSockets;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class MsgMailList
    {
        [ProtoContract]
        public class CMsgMailListProto
        {
            [ProtoMember(1, IsRequired = true)]
            public uint Type; // [ProtoFix 2026-10-14] client CMsgMailListPB #1 is uint32 (was int)
            [ProtoMember(2, IsRequired = true)]
            public int Count;
            [ProtoMember(3, IsRequired = true)]
            public CMsgMailListProto.Mail[] MailList;
            [ProtoContract]
            public class Mail
            {
                public Mail(Game.MsgServer.PrizeInfo obj)
                {
                    Id = obj.id;
                    Money = obj.Money;
                    EMoney = obj.EMoney;
                    Expiration_Date = obj.Expiration_Date;
                    Itemid = obj.itemid;
                    Emoney_Record_Type = obj.Emoney_Record_Type;
                    Flag = obj.Flag; ;
                    Sender_Name = obj.Sender_name;
                    Title = obj.title;
                }
                [ProtoMember(1, IsRequired = true)]
                public ulong Id;

                [ProtoMember(2, IsRequired = true)]
                public ulong Money;

                [ProtoMember(3, IsRequired = true)]
                public ulong EMoney;

                [ProtoMember(4, IsRequired = true)]
                public ulong Expiration_Date;

                [ProtoMember(5, IsRequired = true)]
                public ulong Itemid;

                [ProtoMember(6, IsRequired = true)]
                public ulong Emoney_Record_Type;

                [ProtoMember(7, IsRequired = true)]
                public ulong Flag;

                [ProtoMember(8, IsRequired = true)]
                public long Member8;//0

                [ProtoMember(9, IsRequired = true)]
                public string Title;

                [ProtoMember(10, IsRequired = true)]
                public string Sender_Name;

                [ProtoMember(11, IsRequired = true)]
                public long Member11;//0
            }
            [PacketAttribute(GamePackets.MsgMailList)]
            private unsafe static void Process(Client.GameClient user, ServerSockets.Packet stream)
            {
                var mailList = user.PrizeInfo.Values.OrderByDescending(p => p.id).ToList();
                if (mailList.Count != 0)
                {
                    MsgMailList.CMsgMailListProto Mail = new MsgMailList.CMsgMailListProto();
                    Mail.Type = 0;
                    Mail.Count = 1;
                    int num = mailList.Count;
                    for (int i = 0; i < (mailList.Count / 10) + 1; i++)
                    {
                        Mail.MailList = new Mail[num > 10 ? 10 : num];
                        for (int x = 0; x < Mail.MailList.Length && num > 0; x++)
                        {
                            Mail.MailList[x] = new Mail(mailList[num - 1]);
                            num--;
                        }
                        user.Send(stream.CreateMailList(Mail));
                    }
                }
            }
        }
        public static ServerSockets.Packet CreateMailList(this ServerSockets.Packet stream, Game.MsgServer.MsgMailList.CMsgMailListProto obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgMailList);
            return stream;
        }

    }
       
}
        

