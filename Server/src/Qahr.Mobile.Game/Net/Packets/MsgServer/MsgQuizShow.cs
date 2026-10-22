using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
   public static class MsgQuizShow
    {
       public enum AcotionID : ushort
       {
           Open = 1,
           SendQuestion = 2,
           ReceiveToping = 3,
           SendToping = 4,
           HistoryBestsRank = 5,
           GiveAwaser = 3,
           Remove = 8

       }
       public static unsafe void GetQuizShow(this ServerSockets.Packet stream, out AcotionID type, out byte Answer)
       {
         type = (AcotionID)stream.ReadUInt16();
         stream.SeekForward(2);
         Answer = stream.ReadUInt8();
       }
       public static unsafe ServerSockets.Packet QuizShowCreate(this ServerSockets.Packet stream, AcotionID type, ushort DwParam1, ushort DwParam2, byte DwParam3
           , byte DwParam4, ushort DwParam5, ushort DwParam6, ushort DwParam7)
       {
           stream.InitWriter();
           stream.Write((byte)type); stream.Write((byte)9);
           for (int x = 0; x < 10; x++)
           {
               stream.Write((byte)2);
               stream.Write((byte)8);
           }
        
           stream.ZeroFill(20);
           stream.Finalize(GamePackets.MsgQuiz);
           return stream;

       }
       public static unsafe ServerSockets.Packet QuizShowCreate(this ServerSockets.Packet stream, AcotionID type, ushort DwParam1, ushort DwParam2, byte DwParam3
         , byte DwParam4, ushort DwParam5, ushort DwParam6, ushort DwParam7, params string[] texts)
       {
           stream.InitWriter();
           stream.Write((ushort)1);
           stream.Write(DwParam1);//noumber
           stream.Write(DwParam2);//question level 0= none ,1 or 2.
           stream.Write(DwParam3);//time?
           stream.Write(DwParam4);//11
           stream.Write(DwParam5);//14
           stream.Write(DwParam6);//my quiz points
           stream.Write(DwParam7);//18
           stream.ZeroFill(6);
           stream.Write(texts);
           stream.Finalize(GamePackets.MsgQuiz);
           return stream;

       }
       public static unsafe ServerSockets.Packet QuizShowCreate(this ServerSockets.Packet stream, AcotionID type, ushort DwParam1, ushort DwParam2, byte DwParam3
       , byte DwParam4, ushort DwParam5, ushort DwParam6, ushort DwParam7, params Client.GameClient[] users)
       {
           stream.InitWriter();
           stream.Write((ushort)type);
           stream.Write(DwParam1);//question number
           stream.Write(DwParam2);//question level 1 or 2.
           stream.Write(DwParam3);//10
           stream.Write(DwParam4);//11
           stream.Write(DwParam5);//14
           stream.Write(DwParam6);//my quiz points
           stream.Write(DwParam7);//18
           stream.ZeroFill(2);
           stream.Write(users.Length);

           for (int x = 0; x < Math.Min(3, users.Length); x++)
           {
               var element = users[x];
               stream.Write(element.Player.Name, 32);
               stream.Write((ushort)element.QuizShowPoints);
               stream.Write((ushort)element.QuizRank);
           }

           stream.Finalize(GamePackets.MsgQuiz);
           return stream;

       }

       [PacketAttribute(GamePackets.MsgQuiz)]
       private unsafe static void Process(Client.GameClient user, ServerSockets.Packet stream)
       {
           MyConsole.PrintPacketAdvanced(stream.Memory, stream.Size);
           AcotionID type ;
           byte Answer;
           stream.GetQuizShow(out type, out Answer);
           switch (type)
           {
               case AcotionID.SendQuestion:
                   {
                      
                       break;
                   }
               case AcotionID.Remove:
                   {
                     
                       break;
                   }
           }

       }

    }
}
