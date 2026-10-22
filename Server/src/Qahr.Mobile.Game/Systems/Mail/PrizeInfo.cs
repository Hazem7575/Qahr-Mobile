using Qahr.Mobile.Game.MsgServer;
using Qahr.Mobile.Client;
using Qahr.Mobile.ServerSockets;
using Qahr.Mobile.WindowsAPI;
using System;
using System.IO;

namespace Qahr.Mobile.Game.MsgServer
{
    public class PrizeInfo
    {
        public string title;
        public string Sender_name;
        public string content;
        public uint id;
        public uint Expiration_Date;
        public ulong Money;
        public uint EMoney;
        public uint itemid;
        public uint Emoney_Record_Type;
        public uint Flag;
        public DateTime TimeStamp;

        public PrizeInfo()
        {
        }

        public PrizeInfo(Client.GameClient client, string _title, string _Sender_name, string _content, uint _Expiration_Date, ulong _Money, uint _EMoney, uint _itemid, uint _Flag, uint _Emoney_Record_Type)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                uint NewID = 1;
                var stream = rec.GetStream();
                id = NewID++;
                while (client.PrizeInfo.ContainsKey(this.id))
                {
                    id = NewID++;
                }
                title = _title;
                Sender_name = _Sender_name;
                content = _content;
                Expiration_Date = _Expiration_Date;
                TimeStamp = DateTime.Now.AddSeconds(Expiration_Date);
                Money = _Money;
                EMoney = _EMoney;
                itemid = _itemid;
                Flag = _Flag;
                Emoney_Record_Type = _Emoney_Record_Type;
                MsgMailList.CMsgMailListProto Mail = new MsgMailList.CMsgMailListProto();
                Mail.Type = 2;
                Mail.Count = 1;
                Mail.MailList = new MsgMailList.CMsgMailListProto.Mail[1];
                Mail.MailList[0] = new MsgMailList.CMsgMailListProto.Mail(this);
                client.Send(stream.CreateMailList(Mail));
                client.PrizeInfo.Add(this.id, this);
                MsgMailNotify.Loading(client);
            }
        }

  
    }
}