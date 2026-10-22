using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.MsgInterServer.Instance
{
   public static class CrossElitePKTournament
    {

       public static Dictionary<uint, List<Game.MsgTournaments.MsgEliteGroup.FighterStats>> RanksElite = new Dictionary<uint, List<Game.MsgTournaments.MsgEliteGroup.FighterStats>>();

       public static void AddRanks(uint Rank, List<Game.MsgTournaments.MsgEliteGroup.FighterStats> list)
       {
           if (RanksElite.ContainsKey(Rank))
               RanksElite.Remove(Rank);
           RanksElite.Add(Rank, list);
       }
       public static bool GetReward(Client.GameClient user, ServerSockets.Packet stream)
       {
           foreach (var rank in RanksElite)
           {
               foreach (var obj in rank.Value)
               {
                   if (obj.RealUID == user.Player.UID && obj.ClaimReward == 0)
                   {
                       if (rank.Key == 3)
                       {
                           switch (obj.CrossEliteRank)
                           {
                               case 1:
                                   {
                                       user.Player.AddSpecialTitle(MsgTitleStorage.TitleType.Legendary, stream);
                                       break;
                                   }
                               case 2:
                                   {
                                       user.Player.AddSpecialTitle(MsgTitleStorage.TitleType.Peerless, stream);
                                       break;
                                   }
                               case 3:
                                   {
                                       user.Player.AddSpecialTitle(MsgTitleStorage.TitleType.Outstanding, stream);
                                       break;
                                   }
                               default:
                                   {
                                       user.Player.AddSpecialTitle(MsgTitleStorage.TitleType.Expert, stream);
                                       break;
                                   }
                          
                           }
                       }
                       user.Inventory.AddItemWitchStack(3300035, 0, (ushort)RewardCount(obj.CrossEliteRank, rank.Key), stream);
                       obj.ClaimReward = 1;
                       string MSG = "Congratulation to " + user.Player.Name + " ! he/she managed to get rank " + obj.CrossEliteRank + " on [CrossElitePKTournament] and claimed (" + RewardCount(obj.CrossEliteRank, rank.Key).ToString() + ")ElitePKPointCoupon.";
                       Server.SendGlobalPacket(new MsgMessage(MSG, MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
                       return true;
                   }
               }
           }
           return false;
       }
       public static int RewardCount(uint Rank, uint Group)
       {
           switch (Group)
           {
               case 3:
                   {
                       switch (Rank)
                       {
                           case 1: return 5000;
                           case 2: return 4000; 
                           case 3: return 3500;
                           default: return 3000;
                       }
                   }
               case 2:
                   {
                       switch (Rank)
                       {
                           case 1: return 3000;
                           case 2: return 2500;
                           case 3: return 2000;
                           default: return 1500;
                       }
                   }
               case 1:
                   {
                       switch (Rank)
                       {
                           case 1: return 2000;
                           case 2: return 1500;
                           case 3: return 1200;
                           default: return 1000;
                       }
                   }
           }
           return 0;
       }
       public static void Save()
       {
           Database.DBActions.Write writer = new Database.DBActions.Write("\\CrossElitePk.ini");

           foreach (var item in RanksElite)
           {
               var Tournament = item.Value;
               for (int i = 0; i < Tournament.Count; i++)
               {
                   Database.DBActions.WriteLine writerline = new Database.DBActions.WriteLine('/');
                   var element = Tournament[i];
                   writerline.Add(item.Key).Add(element.CrossEliteRank).Add(element.UID).Add(element.Name).Add(element.Mesh).Add(element.ServerID).Add(element.RealUID).Add(element.ClaimReward).Add(element.FrameID);
                   writer.Add(writerline.Close());
               }
           }
           writer.Execute(Database.DBActions.Mode.Open);
       }
       public static void Load()
       {
          
       }
    }
}
