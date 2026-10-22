using ProtoBuf;
using System;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet CreateMsgScreenChatSkinOpt(this ServerSockets.Packet stream, MsgMsgScreenChatSkinOpt pQuery)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(pQuery);
            stream.Finalize(GamePackets.MsgScreenChatSkinOpt);
            return stream;
        }

        public static void GetMsgScreenChatSkinOpt(this ServerSockets.Packet stream, out MsgMsgScreenChatSkinOpt pQuery)
        {
            pQuery = stream.ProtoBufferDeserialize<MsgMsgScreenChatSkinOpt>(new MsgMsgScreenChatSkinOpt());
        }
    }

    [ProtoContract]
    public class MsgMsgScreenChatSkinOpt
    {
        [Flags]
        public enum ActionID : uint
        {
            Add = 1,
        }

        private static string GetChatSkinName(uint id)
        {
            switch (id)
            {
                case 10000: return "Dragon`s Grace";
                case 10002: return "Poetic Charm";
                case 10003: return "Blessed Yongding";
                case 10005: return "The Spine of Dunes";
                case 10006: return "Drunk Dream";
                case 10007: return "Cute SpineofDunes";
                case 10008: return "ZADICO Chat Bubble";
                case 10009: return "Blaze Ball Bubble";
                case 10010: return "Blaze Ball Bubble";
                default: return "Unknown";
            }
        }

        public enum ChatSkin : uint
        {
            DragonsGrace = 10000,        // Chat Bubble ( 1 )
            PoeticCharm = 10002,         // Chat Bubble ( 2 )
            BlessedYongding = 10003,     // Chat Bubble ( 3 )
            TheSpineofDunes = 10005,     // Chat Bubble ( 4 )
            DrunkDream = 10006,          // Chat Bubble ( 5 )
            CuteSpineofDunes = 10007,    // Chat Bubble ( 6 )
            ZADICOChatBubble = 10008,    // Chat Bubble ( 7 )
            BlazeBallBubble = 10009,     // Chat Bubble ( 8 )
            BlazeBallBubble2 = 10010,     // Chat Bubble ( 9 )
            BlazeBallBubble3 = 10001,     // Chat Bubble ( 10 )
        }

        [ProtoMember(1, IsRequired = true)]
        public uint TotNum;

        [ProtoMember(2, IsRequired = true)]
        public ActionID Action;

        [PacketAttribute(GamePackets.MsgScreenChatSkinOpt)]
        public unsafe static void Process(Client.GameClient user, ServerSockets.Packet stream)
        {
            var Info = stream.ProtoBufferDeserialize<MsgMsgScreenChatSkinOpt>(new MsgMsgScreenChatSkinOpt());

            switch (Info.Action)
            {
                case ActionID.Add:
                    {
                        if (Info.TotNum == 0)
                            Info.TotNum = user.Player.ChatSkinID;

                        if (!Enum.IsDefined(typeof(ChatSkin), Info.TotNum))
                            Info.TotNum = user.Player.ChatSkinID;

                        if (user.Player.ChatSkinID == Info.TotNum)
                            return;

                        user.Player.ChatSkinID = Info.TotNum;

                        Database.ServerDatabase.SaveChatSkin(user);

                        string skinName = GetChatSkinName(Info.TotNum);
                        user.CreateBoxDialog(string.Format("{0} has been successfully activated", skinName));

                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var p = rec.GetStream();

                            user.Send(p.CreateMsgScreenChatSkinOpt(new MsgMsgScreenChatSkinOpt()
                            {
                                Action = ActionID.Add,
                                TotNum = Info.TotNum
                            }));
                        }

                        break;
                    }
            }
        }
    }
}