using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.MsgInterServer.Instance
{
   public class Union
    {

       public static System.SafeDictionary<uint, Union> InfoUnions = new System.SafeDictionary<uint, Union>();
       public uint ServerID;
       public uint GoldBricks;
       public string Name;
       public string LeaderName;

       public static void AddUnion(uint _serverid, uint _goldbricks, string _name, string _leadername)
       {
           Union obj;
           if (InfoUnions.TryGetValue(_serverid, out obj))
           {
               obj.Name = _name;
               obj.LeaderName = _leadername;
               obj.GoldBricks = _goldbricks;
           }
           else
           {
               obj = new Union();
               obj.Name = _name;
               obj.LeaderName = _leadername;
               obj.GoldBricks = _goldbricks;
               obj.ServerID = _serverid;
               InfoUnions.Add(obj.ServerID, obj);
           }
       }

       public static void AddToUnion(ServerSockets.Packet stream, Client.GameClient user, uint UID, Role.Instance.Union.Member.MilitaryRanks rank, string name, string LeaderName, byte IsKingDom)
       {
           Role.Instance.Union obj;
           if (Role.Instance.Union.UnionPoll.TryGetValue(UID, out obj))
           {
               obj.AddOtherMember(stream, user,false);
               obj.CanSave = false;
               user.Player.UnionMemeber.Rank = rank;
               user.Player.MyUnion.SendMyInfo(stream, user);
           }
           else
           {
               obj = new Role.Instance.Union();
               obj.Name = name;
               obj.CanSave = false;
               obj.Emperor = LeaderName;
               obj.IsKingdom = IsKingDom;
               obj.UID = UID;
               obj.AddOtherMember(stream, user, false);
               user.Player.UnionMemeber.Rank = rank;
               user.Player.MyUnion.SendMyInfo(stream, user);


               Role.Instance.Union.UnionPoll.TryAdd(UID, obj);
           }
       }


    }
}
