using Qahr.Mobile.Role;
using Qahr.Mobile.Role.Instance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{

    public class Rectangle
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                switch (ClientSpell.ID)
                {
                    #region SkillsArcherMele

                    case (ushort)Role.Flags.SpellID.WarSuit401NormalATK1:
                    case (ushort)Role.Flags.SpellID.WarSuit401NormalATK2:
                        {
                            IMapObj mapObj;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out mapObj, MapObjectType.Player) || user.Player.View.TryGetValue(Attack.OpponentUID, out mapObj, MapObjectType.Monster))
                            {
                                Attack.X = mapObj.X;
                                Attack.Y = mapObj.Y;
                            }
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                           , 0, Attack.X, Attack.Y, ClientSpell.ID
                           , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            Algoritms.InLineAlgorithm Line = new Algoritms.InLineAlgorithm(user.Player.X, Attack.X, user.Player.Y, Attack.Y, user.Map, 20, 0, ClientSpell.ID);
                            byte Range = 2;
                            uint Exp = 0;

                            foreach (IMapObj role in user.Player.View.Roles(MapObjectType.Monster))
                            {
                                Game.MsgMonster.MonsterRole monsterRole = role as Game.MsgMonster.MonsterRole;
                                if (Line.InLine(monsterRole.X, monsterRole.Y, Range, false))
                                {
                                    if (CheckAttack.CanAttackMonster.Verified(user, monsterRole, DBSpell))
                                    {



                                        MsgSpellAnimation.SpellObj SpellObj;
                                        Calculate.Physical.OnMonster(user.Player, monsterRole, DBSpell, out SpellObj);
                                        SpellObj.Damage = SpellObj.Damage * 50 / 100; // Mohe Done (Attack)
                                        Exp += ReceiveAttack.Monster.Execute(stream, SpellObj, user, monsterRole);

                                        MsgSpell.Targets.Enqueue(SpellObj);
                                    }
                                }
                            }
                            foreach (IMapObj role in user.Player.View.Roles(MapObjectType.Player))
                            {
                                Role.Player attacked = role as Role.Player;
                                if (Line.InLine(attacked.X, attacked.Y, Range))
                                {
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                    {
                                        MsgSpellAnimation.SpellObj SpellObj;
                                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out SpellObj);
                                        Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(SpellObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(SpellObj);
                                    }
                                }
                            }
                            foreach (IMapObj role in user.Player.View.Roles(MapObjectType.SobNpc))
                            {
                                var attacked = role as SobNpc;
                                if (Line.InLine(attacked.X, attacked.Y, 0))
                                {
                                    if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                    {


                                        MsgSpellAnimation.SpellObj SpellObj;
                                        Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out SpellObj);
                                        Exp += ReceiveAttack.Npc.Execute(stream, SpellObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(SpellObj);
                                    }
                                }
                            }
                            if (MsgSpell.Targets.Count == 0)
                                break;

                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            if (MsgSpell.Targets.Count > 0)
                                user.Player.UpdateArrowBlades(stream, 1);
                            user.Send(stream);


                            break;
                        }
                    case (ushort)Role.Flags.SpellID.WarSuit402NormalATK1:
                    case (ushort)Role.Flags.SpellID.WarSuit402NormalATK2:
                    case (ushort)Role.Flags.SpellID.WarSuit402NormalATK3:
                        {
                            IMapObj mapObj;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out mapObj, MapObjectType.Player) || user.Player.View.TryGetValue(Attack.OpponentUID, out mapObj, MapObjectType.Monster))
                            {
                                Attack.X = mapObj.X;
                                Attack.Y = mapObj.Y;
                            }
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                           , 0, Attack.X, Attack.Y, ClientSpell.ID
                           , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            Algoritms.InLineAlgorithm Line = new Algoritms.InLineAlgorithm(user.Player.X, Attack.X, user.Player.Y, Attack.Y, user.Map, 20, 0, ClientSpell.ID);
                            byte Range = 2;
                            uint Exp = 0;

                            foreach (IMapObj role in user.Player.View.Roles(MapObjectType.Monster))
                            {
                                Game.MsgMonster.MonsterRole monsterRole = role as Game.MsgMonster.MonsterRole;
                                if (Line.InLine(monsterRole.X, monsterRole.Y, Range, false))
                                {
                                    if (CheckAttack.CanAttackMonster.Verified(user, monsterRole, DBSpell))
                                    {



                                        MsgSpellAnimation.SpellObj SpellObj;
                                        Calculate.Physical.OnMonster(user.Player, monsterRole, DBSpell, out SpellObj);
                                        SpellObj.Damage = SpellObj.Damage * 50 / 100; // Mohe Done (Attack)
                                        Exp += ReceiveAttack.Monster.Execute(stream, SpellObj, user, monsterRole);

                                        MsgSpell.Targets.Enqueue(SpellObj);
                                    }
                                }
                            }
                            foreach (IMapObj role in user.Player.View.Roles(MapObjectType.Player))
                            {
                                Role.Player attacked = role as Role.Player;
                                if (Line.InLine(attacked.X, attacked.Y, Range))
                                {
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                    {
                                        MsgSpellAnimation.SpellObj SpellObj;
                                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out SpellObj);
                                        Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(SpellObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(SpellObj);
                                    }
                                }
                            }
                            foreach (IMapObj role in user.Player.View.Roles(MapObjectType.SobNpc))
                            {
                                var attacked = role as SobNpc;
                                if (Line.InLine(attacked.X, attacked.Y, 0))
                                {
                                    if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                    {


                                        MsgSpellAnimation.SpellObj SpellObj;
                                        Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out SpellObj);
                                        Exp += ReceiveAttack.Npc.Execute(stream, SpellObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(SpellObj);
                                    }
                                }
                            }
                            if (MsgSpell.Targets.Count == 0)
                                break;
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);


                            break;
                        }

                    case (ushort)Role.Flags.SpellID.NormalATK1:
                    case (ushort)Role.Flags.SpellID.NormalATK2:
                    case (ushort)Role.Flags.SpellID.NormalATK3:
                        {
                            IMapObj mapObj;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out mapObj, MapObjectType.Player) || user.Player.View.TryGetValue(Attack.OpponentUID, out mapObj, MapObjectType.Monster))
                            {
                                Attack.X = mapObj.X;
                                Attack.Y = mapObj.Y;
                            }
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                           , 0, Attack.X, Attack.Y, ClientSpell.ID
                           , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            Algoritms.InLineAlgorithm Line = new Algoritms.InLineAlgorithm(user.Player.X, Attack.X, user.Player.Y, Attack.Y, user.Map, 20, 0, ClientSpell.ID);
                            byte Range = 2;
                            uint Exp = 0;

                            foreach (IMapObj role in user.Player.View.Roles(MapObjectType.Monster))
                            {
                                Game.MsgMonster.MonsterRole monsterRole = role as Game.MsgMonster.MonsterRole;
                                if (Line.InLine(monsterRole.X, monsterRole.Y, Range, false))
                                {
                                    if (CheckAttack.CanAttackMonster.Verified(user, monsterRole, DBSpell))
                                    {



                                        MsgSpellAnimation.SpellObj SpellObj;
                                        Calculate.Physical.OnMonster(user.Player, monsterRole, DBSpell, out SpellObj);
                                        SpellObj.Damage = SpellObj.Damage * 50 / 100; // Mohe Done (Attack)
                                        Exp += ReceiveAttack.Monster.Execute(stream, SpellObj, user, monsterRole);

                                        MsgSpell.Targets.Enqueue(SpellObj);
                                    }
                                }
                            }
                            foreach (IMapObj role in user.Player.View.Roles(MapObjectType.Player))
                            {
                                Role.Player attacked = role as Role.Player;
                                if (Line.InLine(attacked.X, attacked.Y, Range))
                                {
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                    {
                                        MsgSpellAnimation.SpellObj SpellObj;
                                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out SpellObj);
                                        Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(SpellObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(SpellObj);
                                    }
                                }
                            }
                            foreach (IMapObj role in user.Player.View.Roles(MapObjectType.SobNpc))
                            {
                                var attacked = role as SobNpc;
                                if (Line.InLine(attacked.X, attacked.Y, 0))
                                {
                                    if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                    {


                                        MsgSpellAnimation.SpellObj SpellObj;
                                        Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out SpellObj);
                                        Exp += ReceiveAttack.Npc.Execute(stream, SpellObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(SpellObj);
                                    }
                                }
                            }
                            if (MsgSpell.Targets.Count == 0)
                                break;

                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);


                            break;
                        }



                    #endregion
                    #region Mele Archive Warrior (Dragonhowl)
                    case (ushort)Role.Flags.SpellID.WarSuit201NormalATK1:
                    case (ushort)Role.Flags.SpellID.WarSuit201NormalATK2:
                    case (ushort)Role.Flags.SpellID.WarSuit201NormalATK3:
                        {
                            if (!Database.AtributesStatus.IsWarrior(user.Player.Class))
                            {
                                user.SendSysMesage("Sorry, Only Warrior Character Can Use This Spell !");
                                break;
                            }
                            Role.IMapObj curTarget;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out curTarget, Role.MapObjectType.Player) || user.Player.View.TryGetValue(Attack.OpponentUID, out curTarget, Role.MapObjectType.Monster))
                            {
                                Attack.X = curTarget.X;
                                Attack.Y = curTarget.Y;
                            }

                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                          , 0, Attack.X, Attack.Y, ClientSpell.ID
                          , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            Algoritms.InLineAlgorithm Line = new Algoritms.InLineAlgorithm(user.Player.X, Attack.X, user.Player.Y, Attack.Y, user.Map, 20, 0, ClientSpell.ID);
                            byte LineRange = 1;

                            uint Experience = 0;
                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if ((attacked.Family.Settings & MsgMonster.MonsterSettings.Guard) == MsgMonster.MonsterSettings.Guard)
                                    continue;
                                if (Role.Core.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) <= 24)
                                {
                                    if (Line.InLine(attacked.X, attacked.Y, LineRange))
                                    {
                                        if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                        {


                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                            AnimationObj.Damage = AnimationObj.Damage * 200 / 100;
                                            Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);

                                            MsgSpell.Targets.Enqueue(AnimationObj);

                                        }
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                var attacked = targer as Role.Player;
                                if (AttackHandler.Calculate.Base.GetDistance(user.Player.X, user.Player.Y, targer.X, targer.Y) <= 24 || user.nSaveMele)
                                {
                                    if (Line.InLine(attacked.X, attacked.Y, LineRange, false, user.nSaveMele))
                                    {
                                        if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                        {


                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                            AnimationObj.Damage = AnimationObj.Damage * 80 / 100;
                                            ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                            MsgSpell.Targets.Enqueue(AnimationObj);
                                            if (user.OnAutoAttack == false)
                                            {
                                                unsafe
                                                {
                                                    InteractQuery _attack = new InteractQuery();
                                                    _attack.Damage = (int)AnimationObj.Damage;
                                                    _attack.X = attacked.X;
                                                    _attack.Y = attacked.Y;
                                                    _attack.OpponentUID = attacked.UID;
                                                    _attack.AtkType = (ushort)MsgAttackPacket.AttackID.Physical;
                                                    _attack.UID = user.Player.UID;
                                                    _attack.Effect = (uint)AnimationObj.Effect;
                                                    user.Send(stream.InteractionCreate(_attack));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                var attacked = targer as Role.SobNpc;
                                if (Role.Core.GetDistance(user.Player.X, user.Player.Y, targer.X, targer.Y) <= 24)
                                {
                                    if (Line.InLine(attacked.X, attacked.Y, LineRange))
                                    {
                                        if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                        {
                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                            AnimationObj.Damage = AnimationObj.Damage * 200 / 100;
                                            Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                            MsgSpell.Targets.Enqueue(AnimationObj);
                                            if (user.OnAutoAttack == false)
                                            {
                                                unsafe
                                                {
                                                    InteractQuery _attack = new InteractQuery();
                                                    _attack.Damage = (int)AnimationObj.Damage;
                                                    _attack.X = attacked.X;
                                                    _attack.Y = attacked.Y;
                                                    _attack.OpponentUID = attacked.UID;
                                                    _attack.AtkType = (ushort)MsgAttackPacket.AttackID.Physical;
                                                    _attack.UID = user.Player.UID;
                                                    _attack.Effect = (uint)AnimationObj.Effect;
                                                    user.Send(stream.InteractionCreate(_attack));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (MsgSpell.Targets.Count == 0)
                                return;
                            Updates.IncreaseExperience.Up(stream, user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    #endregion
                    #region Mele Archive Warrior (Bloodlust)
                    case (ushort)Role.Flags.SpellID.WarSuit202NormalATK1:
                    case (ushort)Role.Flags.SpellID.WarSuit202NormalATK2:
                    case (ushort)Role.Flags.SpellID.WarSuit202NormalATK3:
                        {
                            Role.IMapObj curTarget;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out curTarget, Role.MapObjectType.Player) || user.Player.View.TryGetValue(Attack.OpponentUID, out curTarget, Role.MapObjectType.Monster))
                            {
                                Attack.X = curTarget.X;
                                Attack.Y = curTarget.Y;
                            }

                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                          , 0, Attack.X, Attack.Y, ClientSpell.ID
                          , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            Algoritms.InLineAlgorithm Line = new Algoritms.InLineAlgorithm(user.Player.X, Attack.X, user.Player.Y, Attack.Y, user.Map, 20, 0, ClientSpell.ID);

                            byte LineRange = 2;

                            uint Experience = 0;
                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if ((attacked.Family.Settings & MsgMonster.MonsterSettings.Guard) == MsgMonster.MonsterSettings.Guard)
                                    continue;
                                if (Role.Core.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) <= 4)
                                {
                                    if (Line.InLine(attacked.X, attacked.Y, LineRange))
                                    {
                                        if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                        {


                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                            AnimationObj.Damage = AnimationObj.Damage * 200 / 100;
                                            Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);

                                            MsgSpell.Targets.Enqueue(AnimationObj);
                                            if (user.OnAutoAttack == false)
                                            {
                                                unsafe
                                                {
                                                    InteractQuery _attack = new InteractQuery();
                                                    _attack.Damage = (int)AnimationObj.Damage;
                                                    _attack.X = attacked.X;
                                                    _attack.Y = attacked.Y;
                                                    _attack.OpponentUID = attacked.UID;
                                                    _attack.AtkType = (ushort)MsgAttackPacket.AttackID.Physical;
                                                    _attack.UID = user.Player.UID;
                                                    _attack.Effect = (uint)AnimationObj.Effect;
                                                    user.Send(stream.InteractionCreate(_attack));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                var attacked = targer as Role.Player;
                                if (AttackHandler.Calculate.Base.GetDistance(user.Player.X, user.Player.Y, targer.X, targer.Y) <= DBSpell.Range || user.nSaveMele)
                                {
                                    if (Line.InLine(attacked.X, attacked.Y, LineRange, false, user.nSaveMele))
                                    {
                                        if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                        {
                                            if (Pool.Constants.MapCounterHits.Contains(user.Player.Map))
                                                user.Player.HitShoot += 1;

                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                            AnimationObj.Damage = AnimationObj.Damage * 80 / 100;
                                            ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                            MsgSpell.Targets.Enqueue(AnimationObj);
                                            if (user.OnAutoAttack == false)
                                            {
                                                unsafe
                                                {
                                                    InteractQuery _attack = new InteractQuery();
                                                    _attack.Damage = (int)AnimationObj.Damage;
                                                    _attack.X = attacked.X;
                                                    _attack.Y = attacked.Y;
                                                    _attack.OpponentUID = attacked.UID;
                                                    _attack.AtkType = (ushort)MsgAttackPacket.AttackID.Physical;
                                                    _attack.UID = user.Player.UID;
                                                    _attack.Effect = (uint)AnimationObj.Effect;
                                                    user.Send(stream.InteractionCreate(_attack));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                var attacked = targer as Role.SobNpc;
                                if (Role.Core.GetDistance(user.Player.X, user.Player.Y, targer.X, targer.Y) <= DBSpell.Range)
                                {
                                    if (Line.InLine(attacked.X, attacked.Y, LineRange))
                                    {
                                        if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                        {
                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                            AnimationObj.Damage = AnimationObj.Damage * 200 / 100;
                                            Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                            MsgSpell.Targets.Enqueue(AnimationObj);
                                            if (user.OnAutoAttack == false)
                                            {
                                                unsafe
                                                {
                                                    InteractQuery _attack = new InteractQuery();
                                                    _attack.Damage = (int)AnimationObj.Damage;
                                                    _attack.X = attacked.X;
                                                    _attack.Y = attacked.Y;
                                                    _attack.OpponentUID = attacked.UID;
                                                    _attack.AtkType = (ushort)MsgAttackPacket.AttackID.Physical;
                                                    _attack.UID = user.Player.UID;
                                                    _attack.Effect = (uint)AnimationObj.Effect;
                                                    user.Send(stream.InteractionCreate(_attack));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (MsgSpell.Targets.Count == 0)
                                return;
                            Updates.IncreaseExperience.Up(stream, user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    #endregion
                    #region Mele Archive Warrior (Redcurse)
                    case (ushort)Role.Flags.SpellID.WarSuit203NormalATK1:
                    case (ushort)Role.Flags.SpellID.WarSuit203NormalATK2:
                    case (ushort)Role.Flags.SpellID.WarSuit203NormalATK3:
                        {
                            Role.IMapObj curTarget;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out curTarget, Role.MapObjectType.Player) || user.Player.View.TryGetValue(Attack.OpponentUID, out curTarget, Role.MapObjectType.Monster))
                            {
                                Attack.X = curTarget.X;
                                Attack.Y = curTarget.Y;
                            }

                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                           , 0, Attack.X, Attack.Y, ClientSpell.ID
                           , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            Algoritms.InLineAlgorithm Line = new Algoritms.InLineAlgorithm(user.Player.X, Attack.X, user.Player.Y, Attack.Y, user.Map, 20, 0, ClientSpell.ID);
                            byte LineRange = 2;

                            uint Experience = 0;
                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if ((attacked.Family.Settings & MsgMonster.MonsterSettings.Guard) == MsgMonster.MonsterSettings.Guard)
                                    continue;
                                if (Role.Core.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) <= 4)
                                {
                                    if (Line.InLine(attacked.X, attacked.Y, LineRange))
                                    {
                                        if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                        {


                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                            AnimationObj.Damage = AnimationObj.Damage * 200 / 100;
                                            Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);

                                            MsgSpell.Targets.Enqueue(AnimationObj);
                                            if (user.OnAutoAttack == false)
                                            {
                                                unsafe
                                                {
                                                    InteractQuery _attack = new InteractQuery();
                                                    _attack.Damage = (int)AnimationObj.Damage;
                                                    _attack.X = attacked.X;
                                                    _attack.Y = attacked.Y;
                                                    _attack.OpponentUID = attacked.UID;
                                                    _attack.AtkType = (ushort)MsgAttackPacket.AttackID.Physical;
                                                    _attack.UID = user.Player.UID;
                                                    _attack.Effect = (uint)AnimationObj.Effect;
                                                    user.Send(stream.InteractionCreate(_attack));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                var attacked = targer as Role.Player;
                                if (AttackHandler.Calculate.Base.GetDistance(user.Player.X, user.Player.Y, targer.X, targer.Y) <= DBSpell.Range || user.nSaveMele)
                                {
                                    if (Line.InLine(attacked.X, attacked.Y, LineRange, false, user.nSaveMele))
                                    {
                                        if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                        {
                                            if (Pool.Constants.MapCounterHits.Contains(user.Player.Map))
                                                user.Player.HitShoot += 1;

                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                            AnimationObj.Damage = AnimationObj.Damage * 80 / 100;
                                            ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                            MsgSpell.Targets.Enqueue(AnimationObj);
                                            if (user.OnAutoAttack == false)
                                            {
                                                unsafe
                                                {
                                                    InteractQuery _attack = new InteractQuery();
                                                    _attack.Damage = (int)AnimationObj.Damage;
                                                    _attack.X = attacked.X;
                                                    _attack.Y = attacked.Y;
                                                    _attack.OpponentUID = attacked.UID;
                                                    _attack.AtkType = (ushort)MsgAttackPacket.AttackID.Physical;
                                                    _attack.UID = user.Player.UID;
                                                    _attack.Effect = (uint)AnimationObj.Effect;
                                                    user.Send(stream.InteractionCreate(_attack));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                var attacked = targer as Role.SobNpc;
                                if (Role.Core.GetDistance(user.Player.X, user.Player.Y, targer.X, targer.Y) <= DBSpell.Range)
                                {
                                    if (Line.InLine(attacked.X, attacked.Y, LineRange))
                                    {
                                        if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                        {
                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                            AnimationObj.Damage = AnimationObj.Damage * 200 / 100;
                                            Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                            MsgSpell.Targets.Enqueue(AnimationObj);
                                            if (user.OnAutoAttack == false)
                                            {
                                                unsafe
                                                {
                                                    InteractQuery _attack = new InteractQuery();
                                                    _attack.Damage = (int)AnimationObj.Damage;
                                                    _attack.X = attacked.X;
                                                    _attack.Y = attacked.Y;
                                                    _attack.OpponentUID = attacked.UID;
                                                    _attack.AtkType = (ushort)MsgAttackPacket.AttackID.Physical;
                                                    _attack.UID = user.Player.UID;
                                                    _attack.Effect = (uint)AnimationObj.Effect;
                                                    user.Send(stream.InteractionCreate(_attack));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (MsgSpell.Targets.Count == 0)
                                return;
                            Updates.IncreaseExperience.Up(stream, user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    #endregion
                    case (ushort)Role.Flags.SpellID.Rectangle:
                        {
                            Role.IMapObj curTarget;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out curTarget, Role.MapObjectType.Player) || user.Player.View.TryGetValue(Attack.OpponentUID, out curTarget, Role.MapObjectType.Monster))
                            {
                                Attack.X = curTarget.X;
                                Attack.Y = curTarget.Y;
                            }

                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            Algoritms.InLineAlgorithm Line = new Algoritms.InLineAlgorithm(user.Player.X, Attack.X, user.Player.Y, Attack.Y, user.Map, DBSpell.Range, 0, ClientSpell.ID);

                            byte LineRange = 2;

                            uint Experience = 0;
                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if ((attacked.Family.Settings & MsgMonster.MonsterSettings.Guard) == MsgMonster.MonsterSettings.Guard)
                                    continue;
                                if (Role.Core.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) <= DBSpell.Range)
                                {
                                    if (Line.InLine(attacked.X, attacked.Y, LineRange))
                                    {
                                        if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                        {


                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);

                                            Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);

                                            MsgSpell.Targets.Enqueue(AnimationObj);
                                            if (user.OnAutoAttack == false)
                                            {
                                                unsafe
                                                {
                                                    InteractQuery _attack = new InteractQuery();
                                                    _attack.Damage = (int)AnimationObj.Damage;
                                                    _attack.X = attacked.X;
                                                    _attack.Y = attacked.Y;
                                                    _attack.OpponentUID = attacked.UID;
                                                    _attack.AtkType = (ushort)MsgAttackPacket.AttackID.Physical;
                                                    _attack.UID = user.Player.UID;
                                                    _attack.Effect = (uint)AnimationObj.Effect;
                                                    user.Send(stream.InteractionCreate(_attack));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                var attacked = targer as Role.Player;
                                if (Role.Core.GetDistance(user.Player.X, user.Player.Y, targer.X, targer.Y) <= DBSpell.Range)
                                {
                                    if (Line.InLine(attacked.X, attacked.Y, LineRange))
                                    {
                                        if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                        {
                                            if (Pool.Constants.MapCounterHits.Contains(user.Player.Map))
                                                user.Player.HitShoot += 1;

                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                            AnimationObj.Damage = AnimationObj.Damage * 50 / 100; // Abdullah Khaled Done (Attack)
                                            ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                            MsgSpell.Targets.Enqueue(AnimationObj);
                                            if (user.OnAutoAttack == false)
                                            {
                                                unsafe
                                                {
                                                    InteractQuery _attack = new InteractQuery();
                                                    _attack.Damage = (int)AnimationObj.Damage;
                                                    _attack.X = attacked.X;
                                                    _attack.Y = attacked.Y;
                                                    _attack.OpponentUID = attacked.UID;
                                                    _attack.AtkType = (ushort)MsgAttackPacket.AttackID.Physical;
                                                    _attack.UID = user.Player.UID;
                                                    _attack.Effect = (uint)AnimationObj.Effect;
                                                    user.Send(stream.InteractionCreate(_attack));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                var attacked = targer as Role.SobNpc;
                                if (Role.Core.GetDistance(user.Player.X, user.Player.Y, targer.X, targer.Y) <= DBSpell.Range)
                                {
                                    // if (Line.InLine(attacked.X, attacked.Y, LineRange))
                                    {
                                        if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                        {
                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);

                                            Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                            MsgSpell.Targets.Enqueue(AnimationObj);
                                            if (user.OnAutoAttack == false)
                                            {
                                                unsafe
                                                {
                                                    InteractQuery _attack = new InteractQuery();
                                                    _attack.Damage = (int)AnimationObj.Damage;
                                                    _attack.X = attacked.X;
                                                    _attack.Y = attacked.Y;
                                                    _attack.OpponentUID = attacked.UID;
                                                    _attack.AtkType = (ushort)MsgAttackPacket.AttackID.Physical;
                                                    _attack.UID = user.Player.UID;
                                                    _attack.Effect = (uint)AnimationObj.Effect;
                                                    user.Send(stream.InteractionCreate(_attack));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (MsgSpell.Targets.Count == 0)
                                return;
                            Updates.IncreaseExperience.Up(stream, user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.NormalAttack3:
                        {
                            Role.IMapObj curTarget;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out curTarget, Role.MapObjectType.Player) || user.Player.View.TryGetValue(Attack.OpponentUID, out curTarget, Role.MapObjectType.Monster))
                            {
                                Attack.X = curTarget.X;
                                Attack.Y = curTarget.Y;
                            }
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                 , 0, Attack.X, Attack.Y, ClientSpell.ID
                                 , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            Algoritms.Sector sector = new Algoritms.Sector(user.Player.X, user.Player.Y, Attack.X, Attack.Y);
                            sector.Arrange(5 * 20, 5);
                            uint Experience = 0;
                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (sector.Inside(attacked.X, attacked.Y))
                                {
                                    if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                        AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                        Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                        if (user.OnAutoAttack == false)
                                        {
                                            InteractQuery _attack = new InteractQuery();
                                            _attack.Damage = (int)AnimationObj.Damage;
                                            _attack.X = attacked.X;
                                            _attack.Y = attacked.Y;
                                            _attack.OpponentUID = attacked.UID;
                                            _attack.AtkType = (ushort)MsgAttackPacket.AttackID.Physical;
                                            _attack.UID = user.Player.UID;
                                            _attack.Effect = (uint)AnimationObj.Effect;
                                            user.Send(stream.InteractionCreate(_attack));
                                        }
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                var attacked = targer as Role.Player;
                                if (sector.Inside(attacked.X, attacked.Y, user.nSaveMele))
                                {
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                        AnimationObj.Damage = AnimationObj.Damage * 50 / 100; // Abdullah Khaled Done (Attack)
                                        AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                        AnimationObj.Hit = 0;
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                        if (user.OnAutoAttack == false)
                                        {

                                            InteractQuery _attack = new InteractQuery();
                                            _attack.Damage = (int)AnimationObj.Damage;
                                            _attack.X = attacked.X;
                                            _attack.Y = attacked.Y;
                                            _attack.OpponentUID = attacked.UID;
                                            _attack.AtkType = (ushort)MsgAttackPacket.AttackID.Physical;
                                            _attack.UID = user.Player.UID;
                                            _attack.Effect = (uint)AnimationObj.Effect;
                                            user.Send(stream.InteractionCreate(_attack));

                                        }
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                var attacked = targer as Role.SobNpc;
                                if (sector.Inside(attacked.X, attacked.Y))
                                {
                                    if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                        AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                        Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                        if (user.OnAutoAttack == false)
                                        {
                                            InteractQuery _attack = new InteractQuery();
                                            _attack.Damage = (int)AnimationObj.Damage;
                                            _attack.X = attacked.X;
                                            _attack.Y = attacked.Y;
                                            _attack.OpponentUID = attacked.UID;
                                            _attack.AtkType = (ushort)MsgAttackPacket.AttackID.Physical;
                                            _attack.UID = user.Player.UID;
                                            _attack.Effect = (uint)AnimationObj.Effect;
                                            user.Send(stream.InteractionCreate(_attack));

                                        }
                                    }
                                }

                            }

                            if (MsgSpell.Targets.Count == 0)
                                return;
                            Updates.IncreaseExperience.Up(stream, user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user, user.OnAutoAttack ? true : false);
                            break;
                        }

                }
            }
        }
    }
}