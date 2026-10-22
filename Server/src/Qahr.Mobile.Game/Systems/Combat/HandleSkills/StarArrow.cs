using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{
  public  class StarArrow
    {
      public unsafe static void ExecuteExecute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
      {
          Database.MagicType.Magic DBSpell;
          MsgSpell ClientSpell;
          if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
          {
              switch (ClientSpell.ID)
              {
                  case (ushort)Role.Flags.SpellID.StarArrow:
                      {
                          MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);
                          uint Experience = 0;
                          Role.IMapObj target;
                          if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                          {
                              MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                              user.Map.View.MoveTo<Role.IMapObj>(attacked, user.Player.X, user.Player.Y);
                              attacked.X = user.Player.X;
                              attacked.Y = user.Player.Y;
                                  
                              if (Role.Core.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < DBSpell.Range)
                              {
                                  if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                  {
                                      MsgSpellAnimation.SpellObj AnimationObj;
                                      Calculate.Range.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                     
                                     Experience += ReceiveAttack.Monster.Execute(stream,AnimationObj, user, attacked);
                                      MsgSpell.Targets.Enqueue(AnimationObj);
                                  }
                              }
                          }
                          else if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                          {
                              var attacked = target as Role.Player;
                              attacked.Owner.Shift(user.Player.X, user.Player.Y,stream, false);
                              if (Role.Core.GetDistance(user.Player.X, user.Player.Y, target.X, target.Y) < DBSpell.Range)
                              {

                                  if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                  {
                                      MsgSpellAnimation.SpellObj AnimationObj;
                                      Calculate.Range.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                     
                                      ReceiveAttack.Player.Execute(AnimationObj, user, attacked);

                                      AnimationObj.MoveX = user.Player.X;
                                      AnimationObj.MoveY = user.Player.Y;
                                      MsgSpell.Targets.Enqueue(AnimationObj);
                                  }
                              }

                          }
                          else if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc))
                          {
                              var attacked = target as Role.SobNpc;
                              if (Role.Core.GetDistance(user.Player.X, user.Player.Y, target.X, target.Y) < DBSpell.Range)
                              {

                                  if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                  {
                                      MsgSpellAnimation.SpellObj AnimationObj;
                                      Calculate.Range.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                     
                                      Experience += ReceiveAttack.Npc.Execute(stream,AnimationObj, user, attacked);
                                      AnimationObj.Hit = 0;
                                      MsgSpell.Targets.Enqueue(AnimationObj);
                                  }
                              }

                          }
                          Updates.IncreaseExperience.Up(stream,user, Experience);
                          Updates.UpdateSpell.CheckUpdate(stream,user,Attack, Experience, DBSpells);
                          MsgSpell.SetStream(stream);
                          MsgSpell.Send(user);

                          break;
                      }
              }
          }
      }
    }
}
