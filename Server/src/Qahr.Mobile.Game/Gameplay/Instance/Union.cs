using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.Role.Instance
{
   public class Union
    {

       public class Member
       {
           public static Member CreateMember(Client.GameClient user, MilitaryRanks Rank = MilitaryRanks.Member)
           {
               Member obj = new Member();
               obj.Name = user.Player.Name;
               obj.Level = user.Player.Level;
               obj.UID = user.Player.UID;
               obj.Owner = user;
               obj.Rank = Rank;
               obj.Mesh = user.Player.Mesh;
               obj.Class = user.Player.Class;
               obj.NobilityRank = user.Player.NobilityRank;

               return obj;
           }
           public static Member CreateMember(Guild.Member user, MilitaryRanks Rank = MilitaryRanks.Member)
           {
               Member obj = new Member();
               obj.Name = user.Name;
               obj.Level = (ushort)user.Level;
               obj.UID = user.UID;
               obj.Rank = Rank;
               obj.Mesh = user.Mesh;
               obj.Class = user.Class;
               obj.NobilityRank = (Nobility.NobilityRank)user.NobilityRank;

               return obj;
           }
           public byte ReceiveKick = 0;
           public MilitaryRanks Rank = MilitaryRanks.Member;
           public uint Exploits = 0;
           public uint MyTreasury;
           public uint UID;
           public uint Class;
           public uint Mesh;//complete mesh?? or body.
           public ushort Level;
           public Instance.Nobility.NobilityRank NobilityRank = Nobility.NobilityRank.Serf;
           public string Name = "";
           public Client.GameClient Owner;//sa nu uiti sa updatez pe login si out.
           public bool IsOnline
           {
               get { return Owner != null; }
           }

           

           public enum Ranks : uint
           {
               Member = 0,
               Emperror = 1u << 0,
               LeftPremier = 1u << 1,
               RightPremier = 1u << 2,
               LeftMarshal = 1u << 3,
               RightMarshal = 1u << 4,
               SecGeneral = 1u << 5,
               DefGeneral = 1u << 6,
               ParGeneral = 1u << 7,
               CoGeneral = 1u << 8,
               ImprialGuard = 1 << 9,
               Empress = 1 << 10,
               SeniorConc = 1 << 11,
               Conc = 1 << 12, 
           }

           public static Ranks GetRank(MilitaryRanks _rank)
           {
               switch (_rank)
               {
                   case MilitaryRanks.CO_Gen:
                       return Ranks.CoGeneral;
                   case MilitaryRanks.Conc:
                       return Ranks.Conc;
                   case MilitaryRanks.DEF_Gen:
                       return Ranks.DefGeneral;
                   case MilitaryRanks.Emperor:
                       return Ranks.Emperror;
                   case MilitaryRanks.Empress:
                       return Ranks.Empress;
                   case MilitaryRanks.Imprl_Guard:
                       return Ranks.ImprialGuard;
                   case MilitaryRanks.LeftMar:
                       return Ranks.LeftMarshal;
                   case MilitaryRanks.LeftPre:
                       return Ranks.LeftPremier;
                   case MilitaryRanks.Member:
                       return Ranks.Member;
                   case MilitaryRanks.PAC_Gen:
                       return Ranks.ParGeneral;
                   case MilitaryRanks.RightMar:
                       return Ranks.RightMarshal;
                   case MilitaryRanks.RightPre:
                       return Ranks.RightPremier;
                   case MilitaryRanks.SEC_Gen:
                       return Ranks.SecGeneral;
                   case MilitaryRanks.SeniorConc:
                       return Ranks.SeniorConc;
               }
               return Ranks.Member;
           }

          
           public enum MilitaryRanks : uint 
           { 
               Member = 0,
               Emperor = 1000,
               LeftPre = 2000, 
               RightPre = 2010, 
               LeftMar = 2020, 
               RightMar = 2030, 
               SEC_Gen = 2040, 
               DEF_Gen = 2050, 
               PAC_Gen = 2060, 
               CO_Gen = 2070, 
               Imprl_Guard = 3000, 
               Empress = 4000, 
               SeniorConc = 401,//4011,
               Conc = 402,//4021,
           }
       }
       public static ConcurrentDictionary<uint, Union> UnionPoll = new ConcurrentDictionary<uint, Union>();
       public ConcurrentDictionary<uint, Guild> Guilds = new ConcurrentDictionary<uint, Guild>();
       public ConcurrentDictionary<uint, Member> Members = new ConcurrentDictionary<uint, Member>();
       public ConcurrentDictionary<uint, Member> ImperialGuards = new ConcurrentDictionary<uint, Member>();
       public ConcurrentDictionary<uint, Member> ImperialHarem = new ConcurrentDictionary<uint, Member>();

       public Guild[] GetOrderGuilds
       {
           get { return Guilds.Values.OrderBy(p => p.Members.Count).ToArray(); }
       }

       public static Union[] GetOrderyUnions
       {
           get { return UnionPoll.Values.OrderByDescending(p => p.GoldBrick).ToArray(); }
       }

       public int GetImperialHaremCount(Member.MilitaryRanks rank)
       {
           return ImperialHarem.Values.Where(p => p.Rank == rank).Count();
       }
       public bool IsImperialHaremRank(Member.MilitaryRanks rank)
       {
           return rank == Member.MilitaryRanks.Empress || rank == Member.MilitaryRanks.Conc || rank == Member.MilitaryRanks.SeniorConc;
       }
       public Member[] GetOnlineMembers
       {
           get { return Members.Values.OrderByDescending(p => p.IsOnline).ToArray(); }
       }

       public System.SafeDictionary<Member.MilitaryRanks, Member> UnionRanks = new System.SafeDictionary<Member.MilitaryRanks, Member>();

       public Member[] UnionOfficials
       {
           get { return UnionRanks.Values.OrderBy(p => p.Rank).Take(9).ToArray(); }
       }
       public Member[] GetImperialHarem
       {
           get { return ImperialHarem.Values.ToArray(); }
       }
       public Member GetOfficialMemberByUID(uint UID)
       {
           return UnionRanks.Values.Where(p => p.UID == UID).FirstOrDefault();
       }
       public Member GetImperialHaremMemberByUID(uint UID)
       {
           return ImperialHarem.Values.Where(p => p.UID == UID).FirstOrDefault();
       }
       public void CheckUpHarem(Member member, Member.MilitaryRanks Rank)
       {
          
           foreach (var user in ImperialHarem.Values)
           {
               if (user.Rank == Rank && user.UID != member.UID)
               {
                   user.Rank = Member.MilitaryRanks.Member;
                   if (member.IsOnline)
                   {
                       using (var rec = new ServerSockets.RecycledPacket())
                       {
                           var stream = rec.GetStream();
                           SendMyInfo(stream, member.Owner);
                       }
                   }
                   else
                   {
                       Database.ServerDatabase.LoginQueue.TryEnqueue(user);
                   }
                 Member memb;
                 ImperialHarem.TryRemove(user.UID,out memb);
                   break;
               }
           }
       }

       public void UpdataDBRank(Member member, Member.MilitaryRanks Rank)
       {
           if (IsImperialHaremRank(Rank))
           {
               if (!ImperialHarem.ContainsKey(member.UID))
                   ImperialHarem.TryAdd(member.UID, member);
           }
           else if (Rank == Member.MilitaryRanks.Imprl_Guard)
           {
               if (!ImperialGuards.ContainsKey(member.UID))
                   ImperialGuards.TryAdd(member.UID, member);
           }
           else // if (Rank != Member.MilitaryRanks.Emperor)
           {
               if (!UnionRanks.ContainsKey(Rank))
                   UnionRanks.Add(Rank, member);
           }

       }

       public void UpdateUnioOfficials(Member member, Member.MilitaryRanks Rank, bool remove)
       {
           if (IsImperialHaremRank(Rank))
           {
               if (ImperialGuards.ContainsKey(member.UID))
               {
                   Member memb;
                   ImperialGuards.TryRemove(member.UID, out memb);
               }
               if (UnionRanks.ContainsKey(member.Rank))
               {
                   if (UnionRanks[member.Rank].UID == member.UID)
                   {
                       UnionRanks.Remove(member.Rank);
                   }
               }

               if (ImperialHarem.ContainsKey(member.UID))
                   ImperialHarem[member.UID] = member;
               else
               {
                   if (Rank != Member.MilitaryRanks.Member)
                       ImperialHarem.TryAdd(member.UID, member);
               }
               member.Rank = Rank;
               if (member.IsOnline)
               {
                   using (var rec = new ServerSockets.RecycledPacket())
                   {
                       var stream = rec.GetStream();
                       SendMyInfo(stream, member.Owner);
                   }
               }
               else
               {
                   Database.ServerDatabase.LoginQueue.TryEnqueue(member);
               }
               return;
           }
           if (Rank == Member.MilitaryRanks.Imprl_Guard)
           {
               if (UnionRanks.ContainsKey(member.Rank))
               {
                   if (UnionRanks[member.Rank].UID == member.UID)
                   {
                       UnionRanks.Remove(member.Rank);
                   }
               }
               if (ImperialGuards.ContainsKey(member.UID))
                   ImperialGuards[member.UID] = member;
               else
                   ImperialGuards.TryAdd(member.UID, member);

               member.Rank = Rank;
               if (member.IsOnline)
               {
                   using (var rec = new ServerSockets.RecycledPacket())
                   {
                       var stream = rec.GetStream();
                       SendMyInfo(stream, member.Owner);
                   }
               }
               else
               {
                   Database.ServerDatabase.LoginQueue.TryEnqueue(member);
               }
               return;
           }
           if (remove)
           {
               if (ImperialHarem.ContainsKey(member.UID))
               {
                   Member memb;
                   ImperialHarem.TryRemove(member.UID, out memb);
                   member.Rank = Member.MilitaryRanks.Member;
                   if (member.IsOnline)
                   {
                       using (var rec = new ServerSockets.RecycledPacket())
                       {
                           var stream = rec.GetStream();
                           SendMyInfo(stream, member.Owner);
                       }
                   }
                   else
                   {
                       Database.ServerDatabase.LoginQueue.TryEnqueue(member);
                   }
                   return;
               }

               if (ImperialGuards.ContainsKey(member.UID))
               {
                   Member memb;
                   ImperialGuards.TryRemove(member.UID, out memb);
                   member.Rank = Member.MilitaryRanks.Member;
                   if (member.IsOnline)
                   {
                       using (var rec = new ServerSockets.RecycledPacket())
                       {
                           var stream = rec.GetStream();
                           SendMyInfo(stream, member.Owner);
                       }
                   }
                   else
                   {
                       Database.ServerDatabase.LoginQueue.TryEnqueue(member);
                   }
                   return;
               }
              

               if (UnionRanks.ContainsKey(member.Rank))
               {
                   UnionRanks.Remove(member.Rank);
                   member.Rank = Member.MilitaryRanks.Member;
                   if (member.IsOnline)
                   {
                       using (var rec = new ServerSockets.RecycledPacket())
                       {
                           var stream = rec.GetStream();
                           SendMyInfo(stream, member.Owner);
                       }
                   }
                   else
                   {
                       Database.ServerDatabase.LoginQueue.TryEnqueue(member);
                   }
               }
               return;
           }
           //if was guard.
           if (ImperialGuards.ContainsKey(member.UID))
           {
               Member memb;
               ImperialGuards.TryRemove(member.UID, out memb);
           }
           //if was in harem.
           if (ImperialHarem.ContainsKey(member.UID))
           {
               Member memb;
               ImperialHarem.TryRemove(member.UID, out memb);
           }
           if (UnionRanks.ContainsKey(Rank))
           {
               UnionRanks[Rank].Rank = Member.MilitaryRanks.Member;
               if (UnionRanks[Rank].IsOnline)
               {
                   using (var rec = new ServerSockets.RecycledPacket())
                   {
                       var stream = rec.GetStream();
                       SendMyInfo(stream, UnionRanks[Rank].Owner);
                   }
               }
               else
               {
                   // نكتب صاحب المنصب القديم (اللي اتشال) مش العضو الجديد
                   Database.ServerDatabase.LoginQueue.TryEnqueue(UnionRanks[Rank]);
               }
               UnionRanks.Remove(Rank);
           }

           if (Rank != Member.MilitaryRanks.Member)
           {
               member.Rank = Rank;
               UnionRanks.Add(Rank, member);
           }
           if (member.IsOnline)
           {
               using (var rec = new ServerSockets.RecycledPacket())
               {
                   var stream = rec.GetStream();
                   SendMyInfo(stream, member.Owner);
               }
           }
           else
           {
               Database.ServerDatabase.LoginQueue.TryEnqueue(member);
           }

       }

       public static Union[] Top3Unions
       {
           get
           {//Where(p => p.GoldBrick != 0)
               return UnionPoll.Values.OrderByDescending(p => p.GoldBrick).Take(3).ToArray();
             }
       }

       public Member GetMemberByName(string name)
       {
           foreach (var guild in Guilds.Values)
           {
               foreach (var user in guild.Members.Values)
               {
                   if (user.Name == name)
                       return user.UnionMem;
               }
           }
           foreach (var user in Members.Values)
               if (user.Name == name)
                   return user;
           return null;
       }

       public string Name = "";
       public uint UID;
       public uint GoldBrick;
       public ulong Treasury = 0;
       public uint Stipend = 0;//??????
       public string Emperor = "";
       public uint EmperrorUID = 0;
       public byte IsKingdom = 0;
       public uint PlunderID;
       public uint InvadingID;
       public string Bulletin = "None";
       public string Title = "";
       public string PlunderTarget = "None";
       public string InvadingUnion = "None";
       public string RecruitDeclaration = "None";

       public bool CanSave = true;
       


       public static bool CheckName(string name)
       {
           if (!BaseFunc.NameStrCheck(name))
               return false;
           foreach (var obj in UnionPoll.Values)
               if (obj.Name.ToLower() == name.ToLower())
                   return false;
           return true;
       }
       public void Quit(ServerSockets.Packet stream, Member member)
       {
           if (Members.TryRemove(member.UID, out member))
           {
               if (member.IsOnline)
               {
                   member.Owner.Player.MyUnion = null;
                   member.Owner.Player.UnionMemeber = null;

                   SendOverheadLeagueInfo(stream, member.Owner);
                   member.Owner.Player.View.ReSendView(stream);
               }
               else
               {
                   member.ReceiveKick = 1;
                   Database.ServerDatabase.LoginQueue.TryEnqueue(member);
               }
           }
       }
       public void SendOverheadLeagueInfo(ServerSockets.Packet stream, Client.GameClient user)
       {
           MsgOverheadLeagueInfo.OverheadLeagueInfo info = new MsgOverheadLeagueInfo.OverheadLeagueInfo()
           {
                Type =0, ID = Database.GroupServerList.MyServerInfo.ID, UID = user.Player.UID
           };
           user.Send(stream.CreateOverheadLeagueInfo(info));
       }
       public static Union Create(ServerSockets.Packet stream, Client.GameClient user, string UnionName)
       {
           Union obj = new Union();
           obj.Name = UnionName;
           obj.Emperor = user.Player.Name;
           obj.UID = user.Player.GuildID;
           //obj.IsKingdom = 1;
           obj.EmperrorUID = user.Player.UID;

        
           // لو الـ ID موجود TryAdd بيفشل بصمت: نطبع تحذير بس (من غير تغيير السلوك)
           if (!UnionPoll.TryAdd(obj.UID, obj))
               MyConsole.WriteLine("[Union] Create: UnionPoll already has ID " + obj.UID + " (" + UnionName + ") - not added.");


           user.Player.MyUnion = obj;
           user.Player.UnionMemeber = Member.CreateMember(user, Member.MilitaryRanks.Emperor);

           user.Player.MyGuildMember.UnionMem = user.Player.UnionMemeber;
          
           user.Player.UnionMemeber.Owner = user;
           obj.UpdateUnioOfficials(user.Player.UnionMemeber, Member.MilitaryRanks.Emperor, false);

           obj.SendMyInfo(stream, user);
           obj.AddEmperror(user.Player.UnionMemeber);

           return obj;
       }
       public void AddEmperror(Member member)
       {
           member.Rank = Member.MilitaryRanks.Emperor;
           if (UnionRanks.ContainsKey(Member.MilitaryRanks.Emperor))
               UnionRanks[Member.MilitaryRanks.Emperor] = member;
           else
           {
             
               UnionRanks.Add(Member.MilitaryRanks.Emperor, member);
           }
       }
       public void RemoveEmperor()
       {
           Member member;
           if (UnionRanks.TryGetValue(Member.MilitaryRanks.Emperor, out member))
           {
               member.Rank = Member.MilitaryRanks.Member;
           }
       }
       public Member GetEmperor()
       {
           Member member;
           UnionRanks.TryGetValue(Member.MilitaryRanks.Emperor, out member);
           return member;
       }
       public static void CheckGuildWar(ServerSockets.Packet stream)
       {
           var obj = UnionPoll.Values.Where(p => p.IsKingdom == 1).FirstOrDefault();
           // runs every war: the kingdom moves when another union's emperor wins
           {
               var _guild = Game.MsgTournaments.MsgSchedules.GuildWar.Winner;
               if (_guild.GuildID != 0)
               {
                   Role.Instance.Guild guild;
                   if (Role.Instance.Guild.GuildPoll.TryGetValue(_guild.GuildID, out guild))
                   {
                       if (guild.GetUnion != null && guild.GetUnion != obj)
                       {
                           if (guild.GetUnion.EmperrorUID == guild.GetGuildLeader.UID)
                           {
                               if (obj != null)
                                   obj.UpdateToUnion(stream);
                               guild.GetUnion.UpdateToKingdom(stream);
                           }
                       }
                   }
               }
           }
       }
       public void UpdateToKingdom(ServerSockets.Packet stream)
       {
           IsKingdom = 1;
           UpdateAllMembers(stream);
       }
       public void UpdateToUnion(ServerSockets.Packet stream)
       {
           IsKingdom = 0;
           foreach (var user in ImperialHarem.Values)
           {
               user.Rank = Member.MilitaryRanks.Member;
               if (!user.IsOnline)
               {
                   Database.ServerDatabase.LoginQueue.TryEnqueue(user);
               }
              
           }
           foreach (var user in ImperialGuards.Values)
           {
               user.Rank = Member.MilitaryRanks.Member;
               if (!user.IsOnline)
               {
                   Database.ServerDatabase.LoginQueue.TryEnqueue(user);
               }
             
           }
           foreach (var user in UnionRanks.Values)
           {
               if (user.Rank == Member.MilitaryRanks.Emperor)
                   continue;
               user.Rank = Member.MilitaryRanks.Member;
               if (!user.IsOnline)
               {
                   Database.ServerDatabase.LoginQueue.TryEnqueue(user);
               }
            
           }
           ImperialHarem.Clear();
           ImperialGuards.Clear();
           //UnionRanks.Clear();
           UpdateAllMembers(stream);
       }
       public void UpdateAllMembers(ServerSockets.Packet stream)
       {
           foreach (var user in Members.Values)
           {
               if (user.IsOnline)
               {
                   SendMyInfo(stream,user.Owner);
               }
           }
           foreach (var guild in Guilds.Values)
           {
               foreach (var memb in guild.Members.Values)
               {
                   if (memb.IsOnline)
                   {
                       Client.GameClient Owner;
                       if (Pool.GamePoll.TryGetValue(memb.UID, out Owner))
                       {
                           SendMyInfo(stream, Owner);
                       }
                   }
               }
           }
       }
       public void SendMyInfo(ServerSockets.Packet stream, Client.GameClient user)
       {
           if (user == null)
               return;
           user.Player.SendUpdate(stream, (uint)Member.GetRank(user.Player.UnionMemeber.Rank), Game.MsgServer.MsgUpdate.DataType.UnionRank);
          if(IsKingdom >0)
              user.Send(stream.LeagueOptCreate(MsgLeagueOpt.ActionID.MyUnion, user.Player.MyUnion.UID, 0,
                  user.Player.MyUnion.UID, ""));
            

          user.Player.Send(stream.CreateLeagueMainRank(new MsgLeagueMainRank.MsgUnionRank()
           {
               Type = 0,
               UnionID = UID,
               UID = user.Player.UID,
               Name = Name,
               PlayerRank = (uint)Member.GetRank(user.Player.UnionMemeber.Rank),
               UnionRank = IsKingdom > 0 ? MsgLeagueMainRank.RankType.Kingdom : MsgLeagueMainRank.RankType.Union,
               ServerID = user.Player.ServerID,
           }));
       }
       public void AddGuild(ServerSockets.Packet stream, Role.Instance.Guild guild)
       {
           if (!Guilds.ContainsKey(guild.Info.GuildID))
           {
               guild.UnionID = UID;

               Guilds.TryAdd(guild.Info.GuildID, guild);
               foreach (var user in guild.Members.Values)
               {
                   if (user.UID == EmperrorUID)
                   {
                       continue;
                   }
                   user.UnionMem = Member.CreateMember(user, Member.MilitaryRanks.Member);
                   if (user.IsOnline)
                   {
                       Client.GameClient _Owner;
                       if (Pool.GamePoll.TryGetValue(user.UID, out _Owner))
                       {
                           user.UnionMem.Owner = _Owner;
                           _Owner.Player.UnionMemeber = user.UnionMem;
                           _Owner.Player.MyUnion = this;
                           SendMyInfo(stream, _Owner);
                       }
                   }
               }
           }
       }
       public void ExpelGuild(ServerSockets.Packet stream, Role.Instance.Guild guild)
       {
           if (Guilds.TryRemove(guild.Info.GuildID, out guild))
           {
               foreach (var member in guild.Members.Values)
               {

                   if (member.IsOnline)
                   {
                       Client.GameClient Owner;
                       if (Pool.GamePoll.TryGetValue(member.UID, out Owner))
                       {
                           Owner.Player.MyUnion = null;
                           Owner.Player.UnionMemeber = null;
                           SendOverheadLeagueInfo(stream, Owner);
                           Owner.Player.View.ReSendView(stream);
                       }
                   }
                   else if (member.UnionMem != null)
                   {
                       // نبعت عضو اليونيون (يصفّر بيانات اليونيون بس) مش Guild.Member اللي كان بيصفّر GuildRank
                       member.UnionMem.ReceiveKick = 1;
                       Database.ServerDatabase.LoginQueue.TryEnqueue(member.UnionMem);
                   }
                   member.UnionMem = null;
               }
               guild.UnionID = 0;
           }
       }
       public void AddOtherMember(ServerSockets.Packet stream, Client.GameClient user, bool SendInfo = true)
       {
           if (Program.ServerConfig.IsInterServer == true && user.OnInterServer)
           {
               Member member = Member.CreateMember(user, Member.MilitaryRanks.Member);
               member.Owner = user;
               user.Player.MyUnion = this;
               user.Player.UnionMemeber = member;
               user.Player.UnionMemeber.Owner = user;
               Members.TryAdd(member.UID, member);
               if (SendInfo)
                   SendMyInfo(stream, user);
           }
           else if (!Members.ContainsKey(user.Player.UID))
           {
               Member member = Member.CreateMember(user, Member.MilitaryRanks.Member);
               member.Owner = user;
               user.Player.MyUnion = this;
               user.Player.UnionMemeber = member;
               user.Player.UnionMemeber.Owner = user;
               Members.TryAdd(member.UID, member);
               SendMyInfo(stream, user);
           }
       }
       public void AddOtherMember(ServerSockets.Packet stream, Member user)
       {
           if (!Members.ContainsKey(user.UID))
           {
               Members.TryAdd(user.UID, user);
           }
       }
       public void SendUnionMsg(ServerSockets.Packet stream)
       {
           foreach (var user in Pool.GamePoll.Values)
           {
               if (user.Player.MyUnion != null && user.Player.MyUnion.UID == this.UID)
                   user.Send(stream);
           }
       }
     
    }
}
