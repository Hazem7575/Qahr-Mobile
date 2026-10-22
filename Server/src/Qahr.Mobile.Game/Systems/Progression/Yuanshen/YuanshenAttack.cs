using Qahr.Mobile.Database;
using Qahr.Mobile.Game.MsgServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgFloorItem;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{
    public class YuanshenAttack
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (Game.MsgServer.AttackHandler.CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                switch (ClientSpell.ID)
                {
                    case (ushort)Qahr.Mobile.Role.Enums.SpellID.WarriorNormalATK:
                        {
                            if ((int)Attack.X != (int)user.Player.X && Attack.X != (ushort)0 && user.Map.ValidLocation(Attack.X, Attack.Y))
                            {

                                Algoritms.InLineAlgorithm Line = new Algoritms.InLineAlgorithm(user.Player.X, Attack.X, user.Player.Y, Attack.Y, user.Map, (byte)DBSpell.Range, 0);
                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                                foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                                {
                                    if ((int)Calculate.Base.GetDistance(Attack.X, Attack.Y, target.X, target.Y) <= DBSpell.MaxTargets && Line.InLine(target.X, target.Y, 2))
                                    {
                                        MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                        if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                        {
                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);

                                            ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                            MsgSpell.Targets.Enqueue(AnimationObj);

                                        }
                                    }
                                }
                                foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Player))
                                {
                                    if ((int)Calculate.Base.GetDistance(Attack.X, Attack.Y, target.X, target.Y) <= DBSpell.MaxTargets && Line.InLine(target.X, target.Y, 2))
                                    {
                                        var attacked = target as Role.Player;
                                        if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                                        {
                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);

                                            ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                            MsgSpell.Targets.Enqueue(AnimationObj);

                                        }
                                    }
                                }
                                foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                                {
                                    if ((int)Calculate.Base.GetDistance(Attack.X, Attack.Y, target.X, target.Y) <= DBSpell.MaxTargets && Line.InLine(target.X, target.Y, 2))
                                    {
                                        var attacked = target as Role.SobNpc;
                                        if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                        {
                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);

                                            ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                            MsgSpell.Targets.Enqueue(AnimationObj);

                                        }
                                    }
                                }
                                MsgSpell.SetStream(stream);
                                MsgSpell.SendRole(user);
                                MsgSpell.Send(user);
                                user.Shift(Attack.X, Attack.Y, stream, false);
                            }
                            break;

                        }
                    case (ushort)Qahr.Mobile.Role.Enums.SpellID.CityRazing:
                        {

                            user.Send(stream.InteractionCreate(Attack));
                            var MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            user.Player.AddFlag((MsgUpdate.Flags)Qahr.Mobile.Role.Enums.Flag.CityRazing, (int)2, true);
                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            //For His Team if He Has
                            if (user.Team != null)
                            {
                                foreach (var target in user.Team.GetMembers())
                                {
                                    if (target.Player.Alive)
                                    {
                                        if (Role.Core.GetDistance(user.Player.X, user.Player.Y, target.Player.X, target.Player.Y) < 10 && target.Player.UID != user.Player.UID)
                                        {
                                            target.Player.AddFlag((MsgUpdate.Flags)Qahr.Mobile.Role.Enums.Flag.CityRazing, (int)2, true);
                                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(target.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                                        }
                                    }
                                }
                            }
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Qahr.Mobile.Role.Enums.SpellID.SwordBody:
                        {

                            user.Send(stream.InteractionCreate(Attack));
                            var MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            if (user.Player.AddFlag((MsgUpdate.Flags)Qahr.Mobile.Role.Enums.Flag.SwordBody, (int)DBSpell.Duration, true))
                            {
                                if (user.Player.ContainFlag((MsgUpdate.Flags)Qahr.Mobile.Role.Enums.Flag.DivineArrival))
                                {
                                    user.Player.SwordBodyMax = (uint)DBSpell.Damage2;
                                    user.Player.SwordBodyPercentage = (uint)DBSpell.GDamage % 100;
                                }
                                else
                                {
                                    user.Player.SwordBodyMax = (uint)DBSpell.DamageOnHuman;
                                    user.Player.SwordBodyPercentage = (uint)DBSpell.GDamage / 1000;
                                }
                                var info = new NewUpdateProto();
                                info.UID = user.Player.UID;
                                info.Updates.Add(new NewUpdateProto.NewUpdateQuery()
                                {
                                    Type = MsgUpdate.DataType.ArchiveSkill,
                                    Flag = (ulong)Qahr.Mobile.Role.Enums.Flag.SwordBody,
                                    Time = DBSpell.Duration,
                                    Damage = user.Player.SwordBodyPercentage,
                                    SpellLevel = user.EonspiritSystem.Values.Where(o => o.Position == (ushort)Qahr.Mobile.Role.Enums.EonspiritPosition.EonspiritActive).FirstOrDefault().ITEM_ID % 100,
                                    ManaRecoveryTime = 0
                                });
                                user.Send(info);
                            }
                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    // case (ushort)Role.Flags.SpellID.GreatAwakening:
                    //{
                    //    user.Send(stream.InteractionCreate(Attack));
                    //    if (!user.Player.ContainFlag(MsgUpdate.Flags.GreatAwakening))
                    //    {
                    //        user.Player.AddSpellFlag(MsgUpdate.Flags.GreatAwakening, 10, true);
                    //        user.Player.SendUpdate(stream, MsgUpdate.Flags.GreatAwakening, 10, (uint)DBSpell.GDamage / 10, (uint)0, MsgUpdate.DataType.ArchiveSkill, true);
                    //        user.Player.AddSpellFlag(MsgUpdate.Flags.GreatAwakening, 10, false);
                    //        MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID, 0, 0, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                    //        MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                    //        MsgSpell.SetStream(stream);
                    //        MsgSpell.Send(user);
                    //    }
                    //    break;
                    //}
                    //{
                    //    MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                    //        , 0, Attack.X, Attack.Y, ClientSpell.ID
                    //        , ClientSpell.Level, ClientSpell.UseSpellSoul);
                    //    foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                    //    {
                    //        MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                    //        if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= 8)
                    //        {
                    //            if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                    //            {
                    //                MsgSpellAnimation.SpellObj AnimationObj;
                    //                Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);

                    //                ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                    //                MsgSpell.Targets.Enqueue(AnimationObj);
                    //            }
                    //        }
                    //    }
                    //    foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                    //    {
                    //        var attacked = targer as Role.Player;
                    //        if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= 8)
                    //        {
                    //            if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                    //            {
                    //                MsgSpellAnimation.SpellObj AnimationObj;
                    //                Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);

                    //                ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                    //                attacked.Owner.Player.FutilityDg = (uint)DBSpell.DamageOnHuman;
                    //                //  user.Player.AddFlag(MsgUpdate.Flags.GrandDoctrine, 6, true);
                    //                //  user.Player.SendUpdate(stream, MsgUpdate.Flags.GrandDoctrine, 6, 1, 0, (uint)DBSpell.Damage3, 8, MsgUpdate.DataType.ArchiveSkill, true);

                    //                if (attacked.AddFlag(MsgUpdate.Flags.GreatAwakening, 6, true))
                    //                    attacked.SendUpdate(stream, MsgUpdate.Flags.GreatAwakening, 6, (uint)attacked.Owner.Player.FutilityDg, (uint)attacked.Owner.Player.FutilityDg, MsgUpdate.DataType.ArchiveSkill, true);
                    //                MsgSpell.Targets.Enqueue(AnimationObj);
                    //            }
                    //        }
                    //    }
                    //    foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                    //    {
                    //        var attacked = targer as Role.SobNpc;
                    //        if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= 8)
                    //        {
                    //            if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                    //            {
                    //                MsgSpellAnimation.SpellObj AnimationObj;
                    //                Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);


                    //                ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                    //                MsgSpell.Targets.Enqueue(AnimationObj);
                    //            }
                    //        }
                    //    }
                    //    MsgSpell.SetStream(stream);
                    //    MsgSpell.Send(user);
                    //user.Player.AddSpellFlag(MsgUpdate.Flags.GreatAwakening, 10, true);
                    //user.Player.SendUpdate(stream, MsgUpdate.Flags.GreatAwakening, 10, (uint)DBSpell.GDamage / 10, (uint)0, MsgUpdate.DataType.ArchiveSkill, true);
                    //user.Player.AddSpellFlag(MsgUpdate.Flags.GreatAwakening, 10, false);




                    //  break;
                    //}
                    //case (ushort)Role.Flags.SpellID.GrandDoctrine:
                    //    {
                    //        user.Send(stream.InteractionCreate(Attack));
                    //        if (!user.Player.ContainFlag(MsgUpdate.Flags.GrandDoctrine))
                    //        {
                    //            user.Player.AddSpellFlag(MsgUpdate.Flags.GrandDoctrine, 5, true);
                    //            user.Player.SendUpdate(stream, MsgUpdate.Flags.GrandDoctrine, 5, 1, (uint)0, MsgUpdate.DataType.ArchiveSkill, true);
                    //            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID, 0, 0, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                    //            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                    //            MsgSpell.SetStream(stream);
                    //            MsgSpell.Send(user);
                    //        }
                    //        break;
                    //    }
                    case (ushort)Qahr.Mobile.Role.Enums.SpellID.BenefitShower:
                        {

                            user.Send(stream.InteractionCreate(Attack));
                            var MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            if (user.Player.AddFlag((MsgUpdate.Flags)Qahr.Mobile.Role.Enums.Flag.BenefitShower, (int)DBSpell.Rate, true))
                            {
                                var infouser = new NewUpdateProto();
                                infouser.UID = user.Player.UID;
                                infouser.Updates.Add(new NewUpdateProto.NewUpdateQuery()
                                {
                                    Type = MsgUpdate.DataType.ArchiveSkill,
                                    Flag = (ulong)Qahr.Mobile.Role.Enums.Flag.BenefitShower,
                                    Time = (ulong)DBSpell.Rate,
                                    Damage = (ulong)DBSpell.DamageOnHuman,
                                    SpellLevel = (ulong)DBSpell.Damage2,
                                    ManaRecoveryTime = 0
                                });
                                user.Send(infouser);
                                user.Player.BenefitShowerHP += (uint)DBSpell.DamageOnHuman;
                            }
                            user.Player.HitPoints += DBSpell.GDamage;
                            if (user.Player.HitPoints >= user.Status.MaxHitpoints)
                                user.Player.HitPoints = (int)user.Status.MaxHitpoints;

                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            //For His Team if He Has
                            if (user.Team != null)
                            {
                                foreach (var target in user.Team.GetMembers())
                                {
                                    if (target.Player.Alive)
                                    {
                                        if (Role.Core.GetDistance(user.Player.X, user.Player.Y, target.Player.X, target.Player.Y) < 18 && target.Player.UID != user.Player.UID)
                                        {
                                            if (target.Player.AddFlag((MsgUpdate.Flags)Qahr.Mobile.Role.Enums.Flag.BenefitShower, (int)DBSpell.Rate, true))
                                            {

                                                var infotarget = new NewUpdateProto();
                                                infotarget.UID = user.Player.UID;
                                                infotarget.Updates.Add(new NewUpdateProto.NewUpdateQuery()
                                                {
                                                    Type = MsgUpdate.DataType.ArchiveSkill,
                                                    Flag = (ulong)Qahr.Mobile.Role.Enums.Flag.BenefitShower,
                                                    Time = (ulong)DBSpell.Rate,
                                                    Damage = (ulong)DBSpell.DamageOnHuman,
                                                    SpellLevel = (ulong)DBSpell.Damage2,
                                                    ManaRecoveryTime = 0
                                                });
                                                user.Send(infotarget);
                                                target.Player.BenefitShowerHP += (uint)DBSpell.DamageOnHuman;
                                            }
                                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(target.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                                            target.Player.HitPoints += DBSpell.GDamage;
                                            if (target.Player.HitPoints >= target.Status.MaxHitpoints)
                                                target.Player.HitPoints = (int)target.Status.MaxHitpoints;
                                        }
                                    }
                                }
                            }
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Qahr.Mobile.Role.Enums.SpellID.AmazingSpeed:
                        {

                            user.Send(stream.InteractionCreate(Attack));
                            var MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            user.Player.AddFlag((MsgUpdate.Flags)Qahr.Mobile.Role.Enums.Flag.AmazingSpeedActive, (int)DBSpell.Duration, true);
                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            if (user.Player.AmazingSpeedLayer == 0)
                            {
                                var info = new NewUpdateProto();
                                info.UID = user.Player.UID;
                                info.Updates.Add(new NewUpdateProto.NewUpdateQuery()
                                {
                                    Type = MsgUpdate.DataType.ArchiveSkill,
                                    Flag = (ulong)Qahr.Mobile.Role.Enums.Flag.AmazingSpeedActive,
                                    Time = DBSpell.Duration,
                                    Damage = (ulong)(DBSpell.GDamage / 1000),
                                    SpellLevel = (ulong)(DBSpell.Damage3 % 100),
                                    RestoringHP = user.EonspiritSystem.Values.Where(o => o.Position == (ushort)Qahr.Mobile.Role.Enums.EonspiritPosition.EonspiritActive).FirstOrDefault().ITEM_ID % 100,
                                    ManaRecoveryTime = 0,
                                });
                                user.Send(info);
                            }
                            if (user.Player.AmazingSpeedLayer == 1)
                            {
                                var info = new NewUpdateProto();
                                info.UID = user.Player.UID;
                                info.Updates.Add(new NewUpdateProto.NewUpdateQuery()
                                {
                                    Type = MsgUpdate.DataType.ArchiveSkill,
                                    Flag = (ulong)Qahr.Mobile.Role.Enums.Flag.AmazingSpeedActive,
                                    Time = DBSpell.Duration,
                                    Damage = (ulong)(DBSpell.Damage2 / 1000),
                                    SpellLevel = (ulong)(DBSpell.Damage3 % 100),
                                    RestoringHP = user.EonspiritSystem.Values.Where(o => o.Position == (ushort)Qahr.Mobile.Role.Enums.EonspiritPosition.EonspiritActive).FirstOrDefault().ITEM_ID % 100,
                                    ManaRecoveryTime = 0,
                                });
                                user.Send(info);
                            }
                            if (user.Player.AmazingSpeedLayer == 2)
                            {
                                var info = new NewUpdateProto();
                                info.UID = user.Player.UID;
                                info.Updates.Add(new NewUpdateProto.NewUpdateQuery()
                                {
                                    Type = MsgUpdate.DataType.ArchiveSkill,
                                    Flag = (ulong)Qahr.Mobile.Role.Enums.Flag.AmazingSpeedActive,
                                    Time = DBSpell.Duration,
                                    Damage = (ulong)(DBSpell.Damage3 / 1000),
                                    SpellLevel = (ulong)(DBSpell.Damage3 % 100),
                                    RestoringHP = user.EonspiritSystem.Values.Where(o => o.Position == (ushort)Qahr.Mobile.Role.Enums.EonspiritPosition.EonspiritActive).FirstOrDefault().ITEM_ID % 100,
                                    ManaRecoveryTime = 0,
                                });
                                user.Send(info);
                                user.Player.AmazingSpeedLayer = 0;
                            }
                            user.Player.AmazingSpeedLayer++;
                            user.Equipment.QueryEquipment(user.Equipment.Alternante);
                            break;
                        }
                    case (ushort)Qahr.Mobile.Role.Enums.SpellID.DivineArrival://WarmasterLady= 478;
                        {

                            user.Send(stream.InteractionCreate(Attack));
                            var MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            user.Player.AddFlag((MsgUpdate.Flags)DBSpell.Status, (int)DBSpell.Duration, true);


                            MsgSpell speedSpell;
                            if (user.MySpells.ClientSpells.TryGetValue(
                                (ushort)Qahr.Mobile.Role.Enums.SpellID.AmazingSpeed,
                                out speedSpell))
                            {
                                var SpeedDBSpell = Pool.Magic[speedSpell.ID][speedSpell.Level];

                                user.Player.AddFlag(
                                    (MsgUpdate.Flags)Qahr.Mobile.Role.Enums.Flag.AmazingSpeedActive,
                                    15,
                                    true);

                                var info = new NewUpdateProto();
                                info.UID = user.Player.UID;
                                info.Updates.Add(new NewUpdateProto.NewUpdateQuery()
                                {
                                    Type = MsgUpdate.DataType.ArchiveSkill,
                                    Flag = (ulong)Qahr.Mobile.Role.Enums.Flag.AmazingSpeedActive,
                                    Time = 15,
                                    Damage = (ulong)(SpeedDBSpell.GDamage / 1000),
                                    SpellLevel = (ulong)(SpeedDBSpell.Damage3 % 100),
                                    ManaRecoveryTime = 0
                                });
                                user.Send(info);
                            }


                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            user.Player.SendUpdate(
       stream,
       (MsgUpdate.Flags)Qahr.Mobile.Role.Enums.Flag.DivineArrival,
       (uint)DBSpell.Duration,
       user.EonspiritSystem.Values.Where(o => o.Position == (ushort)Qahr.Mobile.Role.Enums.EonspiritPosition.EonspiritActive).FirstOrDefault().ITEM_ID,
       0,
       MsgUpdate.DataType.ArchiveSkill,
       true
   );
                            user.Player.EonspiritCurrentEnergy = 0;
                            MsgYuanshen.UpdateEnergy(user, user.Player.EonspiritLevel);
                            if (MsgYuanshen.GetItemName(user.EonspiritSystem.Values.Where(o => o.Position == (ushort)Qahr.Mobile.Role.Enums.EonspiritPosition.EonspiritActive).FirstOrDefault().ITEM_ID) == Qahr.Mobile.Role.Enums.EonspiritItem.VictoriousBuddha)
                            {
                                MsgSpell user_spell = null;
                                if (user.MySpells.ClientSpells.TryGetValue((ushort)Qahr.Mobile.Role.Enums.SpellID.AmazingSpeed, out user_spell))
                                {
                                    var Spell = Pool.Magic[user_spell.ID][user_spell.Level];
                                    user.Player.AddFlag((MsgUpdate.Flags)Qahr.Mobile.Role.Enums.Flag.AmazingSpeed, (int)DBSpell.Duration, true);
                                    var info = new NewUpdateProto();
                                    info.UID = user.Player.UID;
                                    info.Updates.Add(new NewUpdateProto.NewUpdateQuery()
                                    {
                                        Type = MsgUpdate.DataType.ArchiveSkill,
                                        Flag = (ulong)Qahr.Mobile.Role.Enums.Flag.AmazingSpeed,
                                        Time = 3600,
                                        Damage = (ulong)(Spell.Damage3 / 1000),
                                        SpellLevel = (ulong)(Spell.Damage3 % 100),
                                        RestoringHP = user.EonspiritSystem.Values.Where(o => o.Position == (ushort)Qahr.Mobile.Role.Enums.EonspiritPosition.EonspiritActive).FirstOrDefault().ITEM_ID % 100,
                                        ManaRecoveryTime = 0,
                                    });
                                    user.Send(info);
                                }
                            }
                            user.Equipment.QueryEquipment(user.Equipment.Alternante);
                            break;
                        }

                }
            }
        }
    }
}