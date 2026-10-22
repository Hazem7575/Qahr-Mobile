using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{
    public class FatalSpin
    {

        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                switch (ClientSpell.ID)
                {
                    case (ushort)Role.Flags.SpellID.FatalSpin:
                        {

                            try
                            {
                                user.Player.View.SendView(stream.InteractionCreate(Attack), true);


                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                    , 0, Attack.X, Attack.Y, ClientSpell.ID
                                    , ClientSpell.Level, ClientSpell.UseSpellSoul);
                                Algoritms.InLineAlgorithm Line = new Algoritms.InLineAlgorithm(user.Player.X, Attack.X, user.Player.Y, Attack.Y, user.Map, DBSpell.Range, 0, ClientSpell.ID);

                                byte LineRange = 2;
                                uint Experience = 0;
                                bool hit = false;
                                foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                                {
                                    MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                    if (Role.Core.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < DBSpell.Range)
                                    {
                                        if (Line.InLine(attacked.X, attacked.Y, LineRange))
                                        {
                                            if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                            {
                                                MsgSpellAnimation.SpellObj AnimationObj;
                                                Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                               
                                                Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                                MsgSpell.Targets.Enqueue(AnimationObj);
                                                hit = true;
                                            }
                                        }
                                    }
                                }
                                foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                                {
                                    var attacked = targer as Role.Player;
                                    if (Role.Core.GetDistance(user.Player.X, user.Player.Y, targer.X, targer.Y) <= 1)
                                    {
                                        if (Line.InLine(attacked.X, attacked.Y, LineRange))
                                        {
                                            if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                            {
                                                MsgSpellAnimation.SpellObj AnimationObj;
                                                Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                                
                                               
                                                ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                                hit = true;
                                                MsgSpell.Targets.Enqueue(AnimationObj);
                                            }
                                        }
                                    }
                                }
                                foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                                {
                                    var attacked = targer as Role.SobNpc;
                                    if (Role.Core.GetDistance(user.Player.X, user.Player.Y, targer.X, targer.Y) < DBSpell.Range)
                                    {
                                        if (Line.InLine(attacked.X, attacked.Y, LineRange))
                                        {
                                            if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                            {
                                                MsgSpellAnimation.SpellObj AnimationObj;
                                                Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                               
                                                Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                              
                                                hit = true;
                                                MsgSpell.Targets.Enqueue(AnimationObj);
                                            }
                                        }
                                    }
                                }
                                Updates.IncreaseExperience.Up(stream, user, Experience);
                                Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                                if (hit)
                                {
                                    MsgSpell.SetStream(stream); MsgSpell.Send(user);
                                }
                            }
                            catch (Exception e)
                            {
                                MyConsole.WriteException(e);
                            }

                            break;
                        }
                    case (ushort)Role.Flags.SpellID.DragonPunch:
                        {
                            try
                            {
                                user.Player.View.SendView(stream.InteractionCreate(Attack), true);

                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                    , 0, Attack.X, Attack.Y, ClientSpell.ID
                                    , ClientSpell.Level, ClientSpell.UseSpellSoul);
                                Algoritms.InLineAlgorithm Line = new Algoritms.InLineAlgorithm(user.Player.X, Attack.X, user.Player.Y, Attack.Y, user.Map, DBSpell.Range, 0, ClientSpell.ID);

                                byte LineRange = 0;
                                uint Experience = 0;
                                Role.IMapObj lastAttacked = null;
                                foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                                {
                                    MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                    if (Role.Core.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < DBSpell.Range)
                                    {
                                        if (Line.InLine(attacked.X, attacked.Y, LineRange))
                                        {
                                            if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                            {
                                                MsgSpellAnimation.SpellObj AnimationObj;
                                                Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                               
                                                Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                                MsgSpell.Targets.Enqueue(AnimationObj);
                                                lastAttacked = attacked;
                                            }
                                        }
                                    }
                                }
                                foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                                {
                                    var attacked = targer as Role.Player;
                                    if (Role.Core.GetDistance(user.Player.X, user.Player.Y, targer.X, targer.Y) < DBSpell.Range)
                                    {
                                        if (Line.InLine(attacked.X, attacked.Y, LineRange))
                                        {
                                            if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                            {
                                                MsgSpellAnimation.SpellObj AnimationObj;
                                                Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                               
                                                ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                                MsgSpell.Targets.Enqueue(AnimationObj);
                                                lastAttacked = attacked;
                                            }
                                        }
                                    }
                                }
                                foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                                {
                                    var attacked = targer as Role.SobNpc;
                                    if (Role.Core.GetDistance(user.Player.X, user.Player.Y, targer.X, targer.Y) < DBSpell.Range)
                                    {
                                        if (Line.InLine(attacked.X, attacked.Y, LineRange))
                                        {
                                            if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                            {
                                                MsgSpellAnimation.SpellObj AnimationObj;
                                                Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                               
                                                Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                                MsgSpell.Targets.Enqueue(AnimationObj);
                                            }
                                        }
                                    }
                                }
                                Updates.IncreaseExperience.Up(stream, user, Experience);
                                Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                                MsgSpell.SetStream(stream);
                                MsgSpell.Send(user);
                                #region ShadowFist(BlueRune)
                                Game.MsgServer.MsgSpell shadowfist;
                                if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.ShadowFist, out shadowfist) && lastAttacked != null)
                                {
                                    var shadowDB = Pool.Magic[shadowfist.ID][shadowfist.Level];
                                    Attack.X = lastAttacked.X;
                                    Attack.Y = lastAttacked.Y;

                                    MsgSpell = new MsgSpellAnimation(user.Player.UID
                                   , 0, Attack.X, Attack.Y, shadowfist.ID
                                   , shadowfist.Level, shadowfist.UseSpellSoul);

                                    foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                                    {
                                        MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                        if (Role.Core.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < shadowDB.Range)
                                        {
                                            if (Line.InLine(attacked.X, attacked.Y, LineRange))
                                            {
                                                if (CheckAttack.CanAttackMonster.Verified(user, attacked, shadowDB))
                                                {
                                                    MsgSpellAnimation.SpellObj AnimationObj;
                                                    Calculate.Physical.OnMonster(user.Player, attacked, shadowDB, out AnimationObj);
                                                    Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                                }
                                            }
                                        }
                                    }
                                    foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                                    {
                                        var attacked = targer as Role.Player;
                                        if (Role.Core.GetDistance(user.Player.X, user.Player.Y, targer.X, targer.Y) < shadowDB.Range)
                                        {
                                            if (Line.InLine(attacked.X, attacked.Y, LineRange))
                                            {
                                                if (CheckAttack.CanAttackPlayer.Verified(user, attacked, shadowDB))
                                                {
                                                    MsgSpellAnimation.SpellObj AnimationObj;
                                                    Calculate.Physical.OnPlayer(user.Player, attacked, shadowDB, out AnimationObj);
                                                    ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                                }
                                            }
                                        }
                                    }
                                    Updates.IncreaseExperience.Up(stream, user, Experience);
                                    MsgSpell.SetStream(stream);
                                    MsgSpell.Send(user);
                                }
                                #endregion

                            }
                            catch (Exception e)
                            {
                                MyConsole.WriteException(e);
                            }
                            break;
                        }
                }
            }
        }
    }
}
