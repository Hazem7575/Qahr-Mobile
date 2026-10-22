using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgTournaments
{
    public class MsgExtremePk : ITournament
    {
       public const uint RewardConquerPoints = 2000;

       public ProcesType Process { get; set; }
       public DateTime StartTimer = new DateTime();
       public DateTime ScoreStamp = new DateTime();
       public DateTime InfoTimer = new DateTime();
       public Role.GameMap Map;
       public uint DinamicID, Secounds = 0;
       public KillerSystem KillSystem;
       public TournamentType Type { get; set; }
       public MsgExtremePk(TournamentType _type)
       {
           Type = _type;
           Process = ProcesType.Dead;
       }
       public void Open()
       {
           if (Process == ProcesType.Dead)
           {
               KillSystem = new KillerSystem();
               Map = Pool.ServerMaps[1505];
               DinamicID = Map.GenerateDynamicID();

               MsgSchedules.SendInvitation("ExtremePk", "ConquerPoints", 410, 354, 1002, 0, 60);

             
               StartTimer= DateTime.Now;
               InfoTimer= DateTime.Now;
               Secounds = 60;
               Process = ProcesType.Idle;
           }
       }
       public void Revive(DateTime Timer, Client.GameClient user)
       {
           if (user.Player.Alive == false && Process != ProcesType.Dead)
           {
               if (InTournament(user))
               {
                   if (user.Player.DeadStamp.AddSeconds(4) < Timer)
                   {
                       ushort x = 0;
                       ushort y = 0;
                       Map.GetRandCoord(ref x, ref y);
                       user.Teleport(x, y, Map.ID, DinamicID);
                   }
               }
           }
       }
       public bool Join(Client.GameClient user,ServerSockets.Packet stream)
       {
           if (Process == ProcesType.Idle)
           {
               user.Player.XtremePkPoints = 1000;
               ushort x = 0;
               ushort y = 0;

               Map.GetRandCoord(ref x, ref y);
               user.Teleport(x, y, Map.ID, DinamicID);
               return true;
           }
           return false;
       }

       public void CheckUp()
       {
           if (Process == ProcesType.Idle)
           {
               if (DateTime.Now > StartTimer.AddMinutes(1))
               {

                   Process = ProcesType.Alive;
                   StartTimer= DateTime.Now;
               }
               else if (DateTime.Now > InfoTimer.AddSeconds(10))
               {
                   Secounds -= 10;

                   InfoTimer= DateTime.Now;
               }
           }
           if (Process == ProcesType.Alive)
           {
               DateTime Now= DateTime.Now;

               if (Now > StartTimer.AddMinutes(10))
               {

                  var array = MapPlayers().OrderByDescending(p => p.Player.XtremePkPoints).ToArray();
                   if (array.Length > 0)
                   {
                       var Winner = array.First();


                       Winner.Player.ConquerPoints += (uint)RewardConquerPoints;
                       using (var rec = new ServerSockets.RecycledPacket())
                       {
                           var stream = rec.GetStream();
                           if (Winner.Inventory.HaveSpace(2))
                               Winner.Inventory.Add(stream, Database.ItemType.PowerExpBall, 2);
                           else
                               Winner.Inventory.AddReturnedItem(stream, Database.ItemType.PowerExpBall, 2);

                       }

                       int x = 1;
                       foreach (var user in array)
                       {
                           if (x > 1)
                           {
                               user.Player.ConquerPoints += (uint)(RewardConquerPoints / x);

                           }
                           x++;
                           user.Teleport(410, 354, 1002);
                       }
                   }
                   Process = ProcesType.Dead;
               }

               if (Now > ScoreStamp)
               {
                     using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        var array = MapPlayers().OrderByDescending(p => p.Player.XtremePkPoints).ToArray();

                        foreach (var user in MapPlayers())
                        {

                      
                        }

                        int x = 0;
                        foreach (var obj in array)
                        {
                            if (x == 4)
                                break;

                            x++;
                        }
                      
                    }
              

                   ScoreStamp = Now.AddSeconds(4);
               }

             
           }
       }

       public void SharePoints(Client.GameClient user, Client.GameClient Killer)
       {
           uint xtremepoints = (uint)(user.Player.XtremePkPoints / 5);
         
           Killer.Player.XtremePkPoints += xtremepoints;
           user.Player.XtremePkPoints -= xtremepoints;


          }

       public void SendMapPacket(ServerSockets.Packet stream)
       {
           foreach (var user in MapPlayers())
               user.Send(stream);
       }

       public bool InTournament(Client.GameClient user)
       {
           if (Map == null)
               return false;
           return user.Player.Map == Map.ID && user.Player.DynamicID == DinamicID;
       }

       public Client.GameClient[] MapPlayers()
       {
           return Map.Values.Where(p => InTournament(p)).ToArray();
       }
    }
}
