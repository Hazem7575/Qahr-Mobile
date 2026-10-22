using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.MsgInterServer
{
   public static class Core
   {
       public static DateTime JoinCrossFlagStamp = new DateTime();
       static bool _IsCrossFlagOpen;
       public static bool IsCrossFlagOpen
       {
           get
           {
               return _IsCrossFlagOpen == true && DateTime.Now < JoinCrossFlagStamp;
           }
           set
           {
               _IsCrossFlagOpen = true;
           }
       }
       public static DateTime JoinGWFlagStamp = new DateTime();
       static bool _IsGWFlagOpen;
       public static bool IsGWFlagOpen
       {
           get
           {
               return _IsGWFlagOpen == true && DateTime.Now < JoinGWFlagStamp;
           }
           set
           {
               _IsGWFlagOpen = true;
           }
       }
       public static DateTime JoinCrossEliteStamp = new DateTime();
       static bool _IsCrossPkOpen;
       public static bool IsCrossPkOpen
       {
           get
           {
               return _IsCrossPkOpen == true && DateTime.Now < JoinCrossEliteStamp;
           }
           set
           {
               _IsCrossPkOpen = true;
           }
       }
      
    }
}
