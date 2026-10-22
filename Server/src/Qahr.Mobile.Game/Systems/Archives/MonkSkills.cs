using System;
using System.Collections.Generic;
using Qahr.Mobile.Game.MsgFloorItem;
namespace Qahr.Mobile.Game.MsgServer.AttackHandler
// All Skills Archive Monk Edit By Mohy ElDin ...
{
    public class MonkSkills
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                switch (ClientSpell.ID)
                {
                    case (ushort)Role.Flags.SpellID.ClapBomb:
                        {
                            if (user.Player.Map == 10250)
                            {
                                user.SendSysMesage("Sorry, You Can`t Use Gale Bomb In Deity Land .");
                                return;
                            }
                            if (user.Player.Map == 10137)
                            {
                                user.SendSysMesage("Sorry, You Can`t Use Gale Bomb In Dragon Island .");
                                return;
                            }
                            if (user.Player.Map == 1038)
                            {
                                user.SendSysMesage("Sorry, You Can`t Use Gale Bomb In Guild War .");
                                return;
                            }
                            if (user.Player.Map == 12879 || user.Player.Map == 12099 || user.Player.Map == 13140 || user.Player.Map == 11996 || user.Player.Map == 1138 || user.Player.Map == 10504 || user.Player.Map == 6521 || user.Player.Map == 6521 || user.Player.Map == 2057 || user.Player.Map == 2071)
                            {
                                user.SendSysMesage("Sorry, You Can`t Use Gale Bomb In This Map .");
                                return;
                            }
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                 , 0, Attack.X, Attack.Y, ClientSpell.ID
                                 , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            int num = 0;

                            switch (ClientSpell.Level)
                            {
                                case 0:
                                case 1:
                                    num = 3;
                                    break;
                                case 2:
                                case 3:
                                    num = 4;
                                    break;
                                default:
                                    num = 5;
                                    break;
                            }
                            uint Experience = 0;
                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= 6)
                                {
                                    if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                    {
                                        if (num < 1) break;
                                        num--;
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);

                                        Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                        AnimationObj.Hit = 1;
                                        AnimationObj.MoveX = target.X;
                                        AnimationObj.MoveY = target.Y;
                                        Pool.ServerMaps[attacked.Map].Pushback(ref AnimationObj.MoveX, ref AnimationObj.MoveY, user.Player.Angle, 5);

                                        user.Map.View.MoveTo<Role.IMapObj>(attacked, (ushort)AnimationObj.MoveX, (ushort)AnimationObj.MoveY);
                                        attacked.X = (ushort)AnimationObj.MoveX;
                                        attacked.Y = (ushort)AnimationObj.MoveY;

                                        MsgSpell.Targets.Enqueue(AnimationObj);

                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                var attacked = targer as Role.Player;
                                if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= 6)
                                {
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                    {
                                        if (num < 1) break;
                                        num--;
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);


                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);


                                        AnimationObj.Hit = 1;
                                        AnimationObj.MoveX = targer.X;
                                        AnimationObj.MoveY = targer.Y;
                                        byte NatureShieldL = 0;
                                        if (Role.Core.Rate(50))
                                        {
                                            if (attacked.Owner.Rune.IsEquipped("NatureShield", ref NatureShieldL) || attacked.Owner.Rune.IsEquipped("Indestructible Balance", ref NatureShieldL))
                                            {
                                                byte Rate = 0;
                                                switch (NatureShieldL)
                                                {
                                                    case 1: Rate = 5; break;
                                                    case 2: Rate = 10; break;
                                                    case 3: Rate = 15; break;
                                                    case 4: Rate = 20; break;
                                                    case 5: Rate = 30; break;
                                                    case 6: Rate = 40; break;
                                                    case 7: Rate = 50; break;
                                                    case 8: Rate = 60; break;
                                                    case 9: Rate = 70; break;
                                                }
                                                if (Calculate.Base.Rate(Rate))
                                                {
                                                    AnimationObj.Hit = 0;
                                                    AnimationObj.Effect = MsgAttackPacket.AttackEffect.NatureShield;
                                                    continue;
                                                }
                                            }
                                            else
                                            {
                                                attacked.Owner.Map.Pushback(ref AnimationObj.MoveX, ref AnimationObj.MoveY, user.Player.Angle, 5);

                                                if (!CheckAttack.CheckFloors.CheckGuildWar(attacked.Owner, (ushort)AnimationObj.MoveX, (ushort)AnimationObj.MoveY))
                                                {
                                                    continue;
                                                }

                                                user.Map.View.MoveTo<Role.IMapObj>(attacked, (ushort)AnimationObj.MoveX, (ushort)AnimationObj.MoveY);
                                                attacked.X = (ushort)AnimationObj.MoveX;
                                                attacked.Y = (ushort)AnimationObj.MoveY;
                                                MsgSpell.Targets.Enqueue(AnimationObj);
                                                attacked.View.Role(false, null);
                                                user.Player.AttackStamp = DateTime.Now.AddSeconds(2);

                                            }
                                        }
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                var attacked = targer as Role.SobNpc;
                                if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= 6)
                                {
                                    if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                    {
                                        if (num < 1) break;
                                        num--;
                                        if (user.Player.Map == 1038 || user.Player.Map == 1138)
                                        {
                                            return;
                                        }
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);

                                        Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                            }
                            Updates.IncreaseExperience.Up(stream, user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            user.Player.HitInMele = false;
                            break;
                        }                   
                    case (ushort)Role.Flags.SpellID.VajraPalm: // Done 2 ( Skill Archive Monk )
                        {
                            if (!user.Player.Alive)
                                return;
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
                                            // [CombatAudit 2026-10-14] removed custom nerf (official VajraPalm 100-245% vs players now applied in Physical.cs): AnimationObj.Damage = AnimationObj.Damage * 20 / 100; // تعديل الدمج
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
                    case (ushort)Role.Flags.SpellID.VajraRing: // Done 3 ( Skill Archive Monk )
                        {
                            if (!user.Player.Alive)
                                return;
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            if (!user.Player.FloorSpells.ContainsKey(ClientSpell.ID))
                            {
                                user.Player.FloorSpells.TryAdd(ClientSpell.ID, new Role.FloorSpell.ClientFloorSpells(user.Player.UID, user.Player.X, user.Player.Y, ClientSpell.SoulLevel, DBSpell, user.Map));
                            }
                            var FloorItem = new Role.FloorSpell(Game.MsgFloorItem.MsgItemPacket.VajraRing, (ushort)Attack.X, (ushort)Attack.Y, 11, DBSpell, 3000, 750);
                            FloorItem.FloorPacket.m_X = Attack.X;
                            FloorItem.FloorPacket.m_Y = Attack.Y;
                            FloorItem.FloorPacket.OwnerX = user.Player.X;
                            FloorItem.FloorPacket.OwnerY = user.Player.Y;
                            FloorItem.FloorPacket.GuildID = user.Player.GuildID;
                            FloorItem.FloorPacket.ItemOwnerUID = user.Player.UID;
                            FloorItem.FloorPacket.Name = "STR_TRAP_ID_4879@@";
                            FloorItem.FloorPacket.m_Color = 11;
                            FloorItem.FloorPacket.Timer = 5;
                            FloorItem.FloorPacket.MapID = user.Player.Map;

                            user.Player.FloorSpells[ClientSpell.ID].AddItem(FloorItem);
                            user.Player.View.SendView(stream.ItemPacketCreate(FloorItem.FloorPacket), true);
                            user.Player.AddFlag(MsgUpdate.Flags.VajraRing, (int)DBSpell.Second, true);
                            user.Player.SendUpdate(stream, MsgUpdate.Flags.VajraRing, (uint)DBSpell.Second, (uint)DBSpell.Damage, 3000, MsgUpdate.DataType.ArchiveSkill);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;


                        }
                    case (ushort)Role.Flags.SpellID.ClawStrike: // Done 4 ( Skill Archive Monk )
                        {
                            if (!user.Player.Alive)
                                return;
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            Algoritms.Fan fan = new Algoritms.Fan(user.Player.X, user.Player.Y, Attack.X, Attack.Y, 8, 160);
                            uint Experience = 0;
                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (fan.IsInFan(target.X, target.Y))
                                {
                                    if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);

                                        Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                        user.Player.AttackStamp = DateTime.Now.AddMilliseconds(500);

                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                var attacked = targer as Role.Player;
                                if (fan.IsInFan(attacked.X, attacked.Y))
                                {
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                        AnimationObj.Hit = 0;
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                        user.Player.AttackStamp = DateTime.Now.AddMilliseconds(500);
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                var attacked = targer as Role.SobNpc;
                                if (fan.IsInFan(attacked.X, attacked.Y))
                                {
                                    if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);

                                        Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                        user.Player.AttackStamp = DateTime.Now.AddMilliseconds(500);
                                    }
                                }

                            }
                            Updates.IncreaseExperience.Up(stream, user, Experience);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            user.Player.AttackStamp = DateTime.Now.AddMilliseconds(500);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.ZenStaff: // Done 5 ( Skill Archive Monk )
                        {
                            if (!user.Player.Alive)
                                return;
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            if (!user.Player.FloorSpells.ContainsKey(ClientSpell.ID))
                            {
                                user.Player.FloorSpells.TryAdd(ClientSpell.ID, new Role.FloorSpell.ClientFloorSpells(user.Player.UID, user.Player.X, user.Player.Y, ClientSpell.SoulLevel, DBSpell, user.Map));
                            }
                            var FloorItem = new Role.FloorSpell(Game.MsgFloorItem.MsgItemPacket.ZenStaff, (ushort)Attack.X, (ushort)Attack.Y, 23, DBSpell, 3000, 900);

                            FloorItem.FloorPacket.m_X = Attack.X;
                            FloorItem.FloorPacket.m_Y = Attack.Y;
                            FloorItem.FloorPacket.OwnerX = user.Player.X;
                            FloorItem.FloorPacket.OwnerY = user.Player.Y;
                            FloorItem.FloorPacket.GuildID = user.Player.GuildID;
                            FloorItem.FloorPacket.ItemOwnerUID = user.Player.UID;
                            FloorItem.FloorPacket.Name = "STR_TRAP_ID_@@" + (uint)FloorItem.FloorPacket.m_ID;
                            user.Player.FloorSpells[ClientSpell.ID].AddItem(FloorItem);
                            user.Player.View.SendView(stream.ItemPacketCreate(FloorItem.FloorPacket), true);

                            user.Player.AddFlag(MsgUpdate.Flags.ZenStaff, 7, true);
                            user.Player.SendUpdate(stream, MsgUpdate.Flags.ZenStaff, (uint)DBSpell.Duration, (uint)DBSpell.Damage, 3000, MsgUpdate.DataType.ArchiveSkill);

                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.BellShield: // Done 6 ( Skill Archive Monk )
                        {
                            if (!user.Player.Alive)
                                return;
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= 8)
                                {
                                    if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);

                                        ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                var attacked = targer as Role.Player;
                                if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= 8)
                                {
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                        attacked.Owner.Player.FutilityDg = (uint)DBSpell.DamageOnHuman;

                                        if (attacked.AddFlag(MsgUpdate.Flags.BellShield, (int)DBSpell.Duration, true))
                                            attacked.SendUpdate(stream, MsgUpdate.Flags.BellShield, DBSpell.Duration, (uint)attacked.Owner.Player.FutilityDg, (uint)attacked.Owner.Player.FutilityDg, MsgUpdate.DataType.ArchiveSkill, true);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                var attacked = targer as Role.SobNpc;
                                if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= 8)
                                {
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
                            MsgSpell.Send(user);
                            user.Player.AddFlag(MsgUpdate.Flags.BellShield, 6, true);
                            user.Player.SendUpdate(stream, MsgUpdate.Flags.BellShield, 6, (int)DBSpell.Damage, 120000, (int)DBSpell.Damage3, 8, MsgUpdate.DataType.ArchiveSkill, true);

                            user.Player.AddSpellFlag(MsgUpdate.Flags.BellShield, (int)DBSpell.Duration, false);



                            break;
                        }
                    case (ushort)Role.Flags.SpellID.QuellingRobe: // Done 7 ( Skill Archive Monk )
                        {
                            if (!user.Player.Alive)
                                return;
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            if (!user.Player.FloorSpells.ContainsKey(ClientSpell.ID))
                            {
                                user.Player.FloorSpells.TryAdd(ClientSpell.ID, new Role.FloorSpell.ClientFloorSpells(user.Player.UID, user.Player.X, user.Player.Y, ClientSpell.SoulLevel, DBSpell, user.Map));
                            }
                            var FloorItem = new Role.FloorSpell(Game.MsgFloorItem.MsgItemPacket.QuellingRobe, (ushort)Attack.X, (ushort)Attack.Y, 14, DBSpell, 3000, 900);
                            FloorItem.FloorPacket.m_X = Attack.X;
                            FloorItem.FloorPacket.m_Y = Attack.Y;
                            FloorItem.FloorPacket.OwnerX = user.Player.X;
                            FloorItem.FloorPacket.OwnerY = user.Player.Y;
                            FloorItem.FloorPacket.GuildID = user.Player.GuildID;
                            FloorItem.FloorPacket.ItemOwnerUID = user.Player.UID;
                            FloorItem.FloorPacket.Name = "STR_TRAP_ID_4861@@";
                            FloorItem.FloorPacket.m_Color = 14;
                            user.Player.FloorSpells[ClientSpell.ID].AddItem(FloorItem);
                            user.Player.View.SendView(stream.ItemPacketCreate(FloorItem.FloorPacket), true);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)(Role.Flags.SpellID.VioletBowl): // Done 8 ( Skill Archive Monk )
                        {
                            if (!user.Player.Alive)
                                return;
                            user.Send(stream.InteractionCreate(Attack));
                            if (!user.Player.ContainFlag(MsgUpdate.Flags.VioletBowl))
                            {
                                user.Player.AddSpellFlag(MsgUpdate.Flags.VioletBowl, (int)DBSpell.Duration, true);
                                user.Player.SendUpdate(stream, (MsgUpdate.Flags)DBSpell.Status, (uint)DBSpell.Duration, (uint)DBSpell.GDamage / 10, (uint)0, MsgUpdate.DataType.ArchiveSkill, true);
                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID, 0, 0, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                                MsgSpell.SetStream(stream);
                                MsgSpell.Send(user);
                            }
                            break;
                        }
                    case (ushort)(Role.Flags.SpellID.BoneForging): // Done 9 ( Skill Archive Monk )
                        {
                            if (!user.Player.Alive)
                                return;
                            user.Send(stream.InteractionCreate(Attack));
                            if (!user.Player.ContainFlag(MsgUpdate.Flags.BoneForging))
                            {
                                user.Player.AddSpellFlag(MsgUpdate.Flags.BoneForging, (int)DBSpell.Duration, true);
                                user.Player.SendUpdate(stream, (MsgUpdate.Flags)DBSpell.Status, (uint)DBSpell.Duration, (uint)DBSpell.GDamage / 10, (uint)0, MsgUpdate.DataType.ArchiveSkill, true);
                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID, 0, 0, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                                MsgSpell.SetStream(stream);
                                MsgSpell.Send(user);
                            }
                            break;
                        }
                    case (ushort)(Role.Flags.SpellID.SilentBreath): // Done 10 ( Skill Archive Monk )
                        {
                            if (!user.Player.Alive)
                                return;
                            user.Send(stream.InteractionCreate(Attack));
                            if (!user.Player.ContainFlag(MsgUpdate.Flags.SilentBreath))
                            {
                                user.Player.AddSpellFlag(MsgUpdate.Flags.SilentBreath, (int)DBSpell.Duration, true);
                                user.Player.SendUpdate(stream, (MsgUpdate.Flags)DBSpell.Status, (uint)DBSpell.Duration, (uint)DBSpell.GDamage / 10, (uint)0, MsgUpdate.DataType.ArchiveSkill, true);
                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID, 0, 0, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                                MsgSpell.SetStream(stream);
                                MsgSpell.Send(user);
                            }
                            break;
                        }
                    case (ushort)(Role.Flags.SpellID.OnefingerZen): // Done 11 ( Skill Archive Monk )
                        {
                            if (!user.Player.Alive)
                                return;
                            user.Send(stream.InteractionCreate(Attack));
                            if (!user.Player.ContainFlag((MsgUpdate.Flags)DBSpell.Status))
                            {
                                user.Player.AddSpellFlag((MsgUpdate.Flags)DBSpell.Status, (int)DBSpell.Duration, true);
                                user.Player.SendUpdate(stream, (MsgUpdate.Flags)DBSpell.Status, (uint)DBSpell.Duration, (uint)DBSpell.GDamage, (uint)0, MsgUpdate.DataType.ArchiveSkill, true);
                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID, 0, 0, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                                MsgSpell.SetStream(stream);
                                MsgSpell.Send(user);
                            }
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.FlowerTouch: // Done 12 ( Skill Archive Monk )
                        {
                            if (!user.Player.Alive)
                                return;
                            user.Send(stream.InteractionCreate(Attack));
                            if (!user.Player.ContainFlag(MsgUpdate.Flags.FlowerTouch))
                            {
                                user.Player.AddSpellFlag(MsgUpdate.Flags.FlowerTouch, (int)DBSpell.Duration, true);
                                user.Player.SendUpdate(stream, (MsgUpdate.Flags)DBSpell.Status, (uint)DBSpell.Duration, (uint)DBSpell.GDamage / 10, (uint)0, MsgUpdate.DataType.ArchiveSkill, true);
                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID, 0, 0, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                                MsgSpell.SetStream(stream);
                                MsgSpell.Send(user);
                            }
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.FlowerTouchATK: // Done 13 ( Skill Archive Monk )
                        {
                            if (!user.Player.Alive)
                                return;
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            if (!user.Player.FloorSpells.ContainsKey(ClientSpell.ID))
                            {
                                user.Player.FloorSpells.TryAdd(ClientSpell.ID, new Role.FloorSpell.ClientFloorSpells(user.Player.UID, user.Player.X, user.Player.Y, ClientSpell.SoulLevel, DBSpell, user.Map));
                            }
                            var FloorItem = new Role.FloorSpell(Game.MsgFloorItem.MsgItemPacket.FlowerTouch
                                , (ushort)Attack.X, (ushort)Attack.Y, 23, DBSpell, 3000, 900);

                            FloorItem.FloorPacket.m_X = Attack.X;
                            FloorItem.FloorPacket.m_Y = Attack.Y;
                            FloorItem.FloorPacket.OwnerX = user.Player.X;
                            FloorItem.FloorPacket.OwnerY = user.Player.Y;
                            FloorItem.FloorPacket.GuildID = user.Player.GuildID;
                            FloorItem.FloorPacket.ItemOwnerUID = user.Player.UID;
                            FloorItem.FloorPacket.Name = "STR_TRAP_ID_@@" + (uint)FloorItem.FloorPacket.m_ID;
                            user.Player.FloorSpells[ClientSpell.ID].AddItem(FloorItem);
                            user.Player.View.SendView(stream.ItemPacketCreate(FloorItem.FloorPacket), true);
                            user.Send(stream.InteractionCreate(Attack));
                            user.Player.AddSpellFlag(MsgUpdate.Flags.FlowerTouch, (int)DBSpell.Duration, true);
                            user.Player.SendUpdate(stream, (MsgUpdate.Flags)DBSpell.Status, (uint)DBSpell.Duration, (uint)DBSpell.GDamage / 10, (uint)0, MsgUpdate.DataType.ArchiveSkill, true);

                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 10000, DBSpells);

                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                }
            }
        }

    }
}