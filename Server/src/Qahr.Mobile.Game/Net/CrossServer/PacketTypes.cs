using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.MsgInterServer
{
   public class PacketTypes
    {
       public const ushort
              ProgramChat = 3010,
           ProgramOnlineInfo = 3011,
           InterServer_NobilityRank = 3100,
           InterServer_TeamRank = 114,
           InterServer_RoleInfo = 113,
           InterServer_SpecialTitles = 112,
           InterServer_CheckTransfer = 111,
           InterServer_Achievement = 110,
           InterServer_Chi = 109,
           InterServer_JiangHu = 108,
           InterServer_InnerPower = 107,
           InterServer_NobilityInfo = 106,
           InterServer_EliteRank = 105,
           InterServer_CreateItem = 104,
           InterServer_ConnectionInfo = 103,
           InterServer_UnionInfo = 102,
           InterServer_GuildInfo = 101,
           InterServer_UnionRanks = 100,
           InterServer_StringText = 140,
             InterServer_RuneBeast = 141,
                 InterServer_CrossPoker = 142,
           InterServer_ServerInfo = 99,
       InterServer_Loader = 151;
    }
}
