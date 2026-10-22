using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgFloorItem;
namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{
    public class Wildwind
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                switch (ClientSpell.ID)
                {
                    case (ushort)Role.Flags.SpellID.DragonTransformationPassive:
                        {
                            if ((int)Attack.X != (int)user.Player.X && Attack.X != (ushort)0 && user.Map.ValidLocation(Attack.X, Attack.Y))
                            {

                                Algoritms.InLineAlgorithm Line = new Algoritms.InLineAlgorithm(user.Player.X, Attack.X, user.Player.Y, Attack.Y, user.Map, DBSpell.Range, 0);
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
                                        if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
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
                    case (ushort)Role.Flags.SpellID.DragonSlash:
                        {

                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                               , 0, Attack.X, Attack.Y, ClientSpell.ID
                                               , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            Algoritms.InLineAlgorithm Line = new Algoritms.InLineAlgorithm(user.Player.X, Attack.X, user.Player.Y, Attack.Y, user.Map, DBSpell.Range, 0, ClientSpell.ID);

                            uint Experience = 0;
                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                if (Line.InLine(target.X, target.Y, 2))
                                {
                                    MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                    if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                        Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                if (Line.InLine(targer.X, targer.Y, 0))
                                {
                                    var attacked = targer as Role.Player;
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                    {

                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                        AnimationObj.Damage = AnimationObj.Damage * 50 / 100;
                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                                else if (Line.InLine(targer.X, targer.Y, 2))
                                {
                                    var attacked = targer as Role.Player;
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj, false, 1);
                                        AnimationObj.Damage = AnimationObj.Damage * 50 / 100;
                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }

                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                if (Line.InLine(targer.X, targer.Y, 2))
                                {
                                    var attacked = targer as Role.SobNpc;
                                    if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                        Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                            }
                            if (user.Player.AddFlag(MsgUpdate.Flags.DivingDragon, (int)DBSpell.Duration, true))
                                user.Player.SendUpdate(stream, MsgUpdate.Flags.DivingDragon, (uint)DBSpell.Duration, (uint)DBSpell.Damage2, DBSpell.Level, MsgUpdate.DataType.ArchiveSkill);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.WildDashAttack:
                        {
                            if ((int)Attack.X != (int)user.Player.X && Attack.X != (ushort)0)
                            {

                                Algoritms.InLineAlgorithm Line = new Algoritms.InLineAlgorithm(user.Player.X, Attack.X, user.Player.Y, Attack.Y, user.Map, DBSpell.Range, 0);
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
                                        if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
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
                                break;
                            }

                            break;
                        }
                    case (ushort)Role.Flags.SpellID.RevengeAttack:
                        {
                            if ((int)Attack.X != (int)user.Player.X && Attack.X != (ushort)0 && user.Map.ValidLocation(Attack.X, Attack.Y))
                            {

                                Algoritms.InLineAlgorithm Line = new Algoritms.InLineAlgorithm(user.Player.X, Attack.X, user.Player.Y, Attack.Y, user.Map, DBSpell.Range, 0);
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
                                        if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                        {
                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);

                                            ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                            MsgSpell.Targets.Enqueue(AnimationObj);
                                            if (!attacked.ContainFlag(MsgUpdate.Flags.DefenseDecreasing))
                                            {
                                                attacked.Owner.Player.PDefence = (uint)DBSpell.Damage2 / 10000;
                                                attacked.Owner.Player.MDefence = (uint)DBSpell.Damage2 % 1000;
                                                attacked.AddSpellFlag(MsgUpdate.Flags.DefenseDecreasing, (int)30, true);
                                                attacked.SendUpdate(stream, MsgUpdate.Flags.DefenseDecreasing, 30, (uint)DBSpell.Damage2 % 1000, DBSpell.Level, MsgUpdate.DataType.ArchiveSkill);
                                                attacked.Owner.Equipment.QueryEquipment(attacked.Owner.Equipment.Alternante);

                                            }

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
                                break;
                            }

                            break;
                        }
                    case (ushort)Role.Flags.SpellID.Ironbone:
                    case (ushort)Role.Flags.SpellID.IronbonePassive:
                        {
                            if (Attack.X == user.Player.X || Attack.X == 0)
                                break;
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, 0, 0, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            List<Algoritms.InLineAlgorithm.coords> coord = Algoritms.MoveCoords.CheckBladeTeampsCoords(user.Player.X, user.Player.Y, Attack.X
                   , Attack.Y, user.Map, 12);
                            Attack.X = user.Player.X;
                            Attack.Y = user.Player.Y;
                            for (int i = 0; i < coord.Count; i++)
                            {
                                if (user.Map.ValidLocation((ushort)coord[i].X, (ushort)coord[i].Y)
                                    && CheckAttack.CheckFloors.CheckGuildWar(user, coord[coord.Count - 1].X, coord[coord.Count - 1].Y))
                                {
                                    Attack.X = (ushort)coord[i].X;
                                    Attack.Y = (ushort)coord[i].Y;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            MsgSpell.X = Attack.X;
                            MsgSpell.Y = Attack.Y;

                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                bool hit = false;
                                for (int j = 0; j < coord.Count; j++)
                                    if (Calculate.Base.GetDDistance(target.X, target.Y, (ushort)coord[j].X, (ushort)coord[j].Y) <= 2)
                                        hit = true;
                                if (hit)
                                {
                                    MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                    if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                        AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                        ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                bool hit = false;
                                for (int j = 0; j < coord.Count; j++)
                                    if (Calculate.Base.GetDDistance(targer.X, targer.Y, (ushort)coord[j].X, (ushort)coord[j].Y) <= 2)
                                        hit = true;
                                if (hit)
                                {
                                    var attacked = targer as Role.Player;
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                                    {

                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                        AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }

                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                bool hit = false;
                                for (int j = 0; j < coord.Count; j++)
                                    if (Calculate.Base.GetDDistance(targer.X, targer.Y, (ushort)coord[j].X, (ushort)coord[j].Y) <= 2)
                                        hit = true;
                                if (hit)
                                {
                                    var attacked = targer as Role.SobNpc;
                                    if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                        AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                        ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                            }
                            MsgSpell.SetStream(stream);
                            MsgSpell.SendRole(user);
                            MsgSpell.Send(user);
                            if (ClientSpell.ID == (ushort)Role.Flags.SpellID.Ironbone)
                            {
                                user.Shift(MsgSpell.X, MsgSpell.Y, stream, false);
                            }
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.Roamer:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            Algoritms.Sector SpellSector = new Algoritms.Sector(user.Player.X, user.Player.Y, Attack.X, Attack.Y);
                            SpellSector.Arrange(DBSpell.Sector, DBSpell.Range);
                            uint Experience = 0;
                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (SpellSector.Inside(attacked.X, attacked.Y)
                                    && Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < 3 || user.nSaveMele)
                                {
                                    if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);

                                        Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);

                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                var attacked = targer as Role.Player;
                                if (SpellSector.Inside(attacked.X, attacked.Y) && Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < 3 || user.nSaveMele)
                                {
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                        // [CombatAudit 2026-10-14] removed custom nerf (calc already equals the official tooltip value): AnimationObj.Damage = AnimationObj.Damage * 30 / 100;

                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                        if (attacked.AddFlag(MsgUpdate.Flags.RisingPhoenix, (int)DBSpell.Duration, true, 1))
                                            attacked.SendUpdate(stream, MsgUpdate.Flags.RisingPhoenix, DBSpell.Duration, DBSpell.Damage3, DBSpell.Level, MsgUpdate.DataType.ArchiveSkill);
                                    }
                                }

                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                var attacked = targer as Role.SobNpc;
                                if (SpellSector.Inside(attacked.X, attacked.Y))
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
                            Updates.IncreaseExperience.Up(stream, user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream); MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.WildFireball:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, user.Player.X, user.Player.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            Algoritms.LayTrapThree Line = new Algoritms.LayTrapThree(user.Player.X, Attack.X, user.Player.Y, Attack.Y, 15);
                            int Stamp = 0;
                            if (!user.Player.FloorSpells.ContainsKey(ClientSpell.ID))
                            {
                                user.Player.FloorSpells.TryAdd(ClientSpell.ID, new Role.FloorSpell.ClientFloorSpells(user.Player.UID, Attack.X, Attack.Y, ClientSpell.SoulLevel, DBSpell, user.Map));
                            }
                            bool hit = false;
                            foreach (var coords in Line.LCoords)
                            {
                                var FloorItem = new Role.FloorSpell(Game.MsgFloorItem.MsgItemPacket.WildFireball, (ushort)coords.X, (ushort)coords.Y, 20, DBSpell, Stamp);
                                FloorItem.FloorPacket.OwnerX = (ushort)coords.X;
                                FloorItem.FloorPacket.OwnerY = (ushort)coords.Y;
                                FloorItem.FloorPacket.m_X = user.Player.X;
                                FloorItem.FloorPacket.m_Y = user.Player.Y;
                                FloorItem.FloorPacket.Name = "trap";
                                FloorItem.FloorPacket.ItemOwnerUID = user.Player.UID;
                                FloorItem.FloorPacket.GuildID = user.Player.GuildID;
                                user.Player.FloorSpells[ClientSpell.ID].AddItem(FloorItem);
                                Stamp += 500;
                                if (!hit)
                                {
                                    user.Player.View.SendView(stream.ItemPacketCreate(FloorItem.FloorPacket), true);
                                    hit = true;
                                }
                            }
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.WildFireballPassive:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, user.Player.X, user.Player.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            Algoritms.LayTrapThree Line = new Algoritms.LayTrapThree(user.Player.X, Attack.X, user.Player.Y, Attack.Y, 15);
                            int Stamp = 0;
                            if (!user.Player.FloorSpells.ContainsKey(ClientSpell.ID))
                            {
                                user.Player.FloorSpells.TryAdd(ClientSpell.ID, new Role.FloorSpell.ClientFloorSpells(user.Player.UID, Attack.X, Attack.Y, ClientSpell.SoulLevel, DBSpell, user.Map));
                            }
                            bool hit = false;
                            foreach (var coords in Line.LCoords)
                            {
                                var FloorItem = new Role.FloorSpell(Game.MsgFloorItem.MsgItemPacket.WildFireball, (ushort)coords.X, (ushort)coords.Y, 20, DBSpell, Stamp);
                                FloorItem.FloorPacket.OwnerX = (ushort)coords.X;
                                FloorItem.FloorPacket.OwnerY = (ushort)coords.Y;
                                FloorItem.FloorPacket.m_X = user.Player.X;
                                FloorItem.FloorPacket.m_Y = user.Player.Y;
                                FloorItem.FloorPacket.Name = "trap";
                                FloorItem.FloorPacket.ItemOwnerUID = user.Player.UID;
                                FloorItem.FloorPacket.GuildID = user.Player.GuildID;
                                user.Player.FloorSpells[ClientSpell.ID].AddItem(FloorItem);
                                Stamp += 500;
                                if (!hit)
                                {
                                    user.Player.View.SendView(stream.ItemPacketCreate(FloorItem.FloorPacket), true);
                                    hit = true;
                                }
                            }
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.WhirlShuriken:
                    case (ushort)Role.Flags.SpellID.WhirlShurikenPassive:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            Algoritms.InLineAlgorithm Line = new Algoritms.InLineAlgorithm(user.Player.X, Attack.X, user.Player.Y, Attack.Y, user.Map, DBSpell.Range, 0);
                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                if (Line.InLine(target.X, target.Y, 2))
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
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                if (Line.InLine(targer.X, targer.Y, 2))
                                {
                                    var attacked = targer as Role.Player;
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);

                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                        #region WhirlSigilAura
                                        Role.Instance.Ninja.Item item;
                                        if (user.MyNinja.TryGetValueEquip(Role.Instance.Ninja.ItemType.WhirlSigilAura, out item))
                                        {
                                            if (attacked.Alive)
                                            {
                                                attacked.WhirlSigilDg = 0;
                                                if (attacked.AddFlag(MsgUpdate.Flags.WhirlSigilAura, (int)2, true, 1))
                                                    attacked.SendUpdate(stream, MsgUpdate.Flags.WhirlSigilAura, 2, (uint)item.DBItem.Damage, item.Level, MsgUpdate.DataType.ArchiveSkill);
                                            }

                                        }
                                        #endregion
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                if (Line.InLine(targer.X, targer.Y, 2))
                                {
                                    var attacked = targer as Role.SobNpc;
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
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.LightningSlash:
                    case (ushort)Role.Flags.SpellID.LightningSlashPassive:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            Algoritms.InLineAlgorithm Line = new Algoritms.InLineAlgorithm(user.Player.X, Attack.X, user.Player.Y, Attack.Y, user.Map, DBSpell.Range, 0);
                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                if (Line.InLine(target.X, target.Y, DBSpell.MaxTargets))
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
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                if (Line.InLine(targer.X, targer.Y, DBSpell.MaxTargets))
                                {
                                    var attacked = targer as Role.Player;
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);

                                        #region SlashSealProwess
                                        Role.Instance.Ninja.Item item;
                                        if (user.MyNinja.TryGetValueEquip(Role.Instance.Ninja.ItemType.SlashSealProwess, out item))
                                        {
                                            if (attacked.Alive)
                                            {
                                                attacked.DrainsHP = (attacked.Owner.Status.MaxHitpoints * 30) / 100;
                                                if (attacked.AddFlag(MsgUpdate.Flags.SlashSeal, (int)4, true, 1))
                                                    attacked.SendUpdate(stream, MsgUpdate.Flags.SlashSeal, 4, (uint)attacked.DrainsHP, item.Level, MsgUpdate.DataType.ArchiveSkill);
                                            }

                                        }
                                        #endregion

                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                if (Line.InLine(targer.X, targer.Y, DBSpell.MaxTargets))
                                {
                                    var attacked = targer as Role.SobNpc;
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
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.AxeShadow:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                               , 0, Attack.X, Attack.Y, ClientSpell.ID
                                               , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            Algoritms.InLineAlgorithm Line = new Algoritms.InLineAlgorithm(user.Player.X, Attack.X, user.Player.Y, Attack.Y, user.Map, DBSpell.Range, 0);


                            uint Experience = 0;
                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                if (Line.InLine(target.X, target.Y, 2))
                                {
                                    MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                    if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                        // [CombatAudit 2026-10-14] removed custom nerf (official AxeShadow 500% to monsters = power): AnimationObj.Damage = AnimationObj.Damage * 400 / 100;
                                        Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                if (Line.InLine(targer.X, targer.Y, 2) || user.nSaveMele)
                                {
                                    var attacked = targer as Role.Player;
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                    {

                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                        // [CombatAudit 2026-10-14] removed custom nerf (calc already equals the official tooltip value): AnimationObj.Damage = AnimationObj.Damage * 65 / 100;
                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                        if (attacked.Alive)
                                        {
                                            attacked.BleedDamage = 3000;
                                            attacked.AddSpellFlag(MsgUpdate.Flags.Bleed, DBSpell.Damage2 % 100, true, 1);
                                        }

                                    }
                                }

                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                if (Line.InLine(targer.X, targer.Y, 2))
                                {
                                    var attacked = targer as Role.SobNpc;
                                    if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                        AnimationObj.Damage = AnimationObj.Damage * 500 / 100;
                                        Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                            }
                            Updates.IncreaseExperience.Up(stream, user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.SendRole(user);
                            MsgSpell.Send(user);

                            user.Player.AddSpellFlag(MsgUpdate.Flags.AxeShadow, (int)DBSpell.Duration, true);
                            user.Player.SendUpdate(stream, MsgUpdate.Flags.AxeShadow, DBSpell.Duration, 0, 0, MsgUpdate.DataType.ArchiveSkill);
                            user.Player.AxeShadowPower = (byte)DBSpell.Damage3;
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.SeaBurial:
                        {
                            uint xX = Attack.X, yY = Attack.Y;
                            user.Map.Pushback(ref xX, ref  yY, Role.Core.GetAngle(user.Player.X, user.Player.Y, Attack.X, Attack.Y), 18);
                            Attack.X = (ushort)xX;
                            Attack.Y = (ushort)yY;
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                               , 0, (ushort)xX, (ushort)yY, ClientSpell.ID
                                               , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            Algoritms.InLineAlgorithm Line = new Algoritms.InLineAlgorithm(user.Player.X, Attack.X, user.Player.Y, Attack.Y, user.Map, 18, 0);


                            uint Experience = 0;
                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                if (Line.InLine(target.X, target.Y, 2))
                                {
                                    MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                    if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                        AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                        Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                if (Line.InLine(targer.X, targer.Y, 2))
                                {
                                    var attacked = targer as Role.Player;
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                                    {

                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                        AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);

                                    }
                                }

                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                if (Line.InLine(targer.X, targer.Y, 2))
                                {
                                    var attacked = targer as Role.SobNpc;
                                    if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                        AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                        Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                            }
                            Updates.IncreaseExperience.Up(stream, user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.SendRole(user);
                            MsgSpell.Send(user);

                            if (!user.Player.FloorSpells.ContainsKey(ClientSpell.ID))
                                user.Player.FloorSpells.TryAdd(ClientSpell.ID, new Role.FloorSpell.ClientFloorSpells(user.Player.UID, user.Player.X, user.Player.Y, ClientSpell.SoulLevel, DBSpell, user.Map));
                            var FloorItem = new Role.FloorSpell(Game.MsgFloorItem.MsgItemPacket.SeaBurial, user.Player.X, user.Player.Y, 16, DBSpell, 3000);
                            FloorItem.FloorPacket.FlowerType = 3;
                            FloorItem.FloorPacket.ItemOwnerUID = user.Player.UID;
                            FloorItem.FloorPacket.GuildID = user.Player.GuildID;
                            FloorItem.FloorPacket.OwnerX = (ushort)xX;
                            FloorItem.FloorPacket.OwnerY = (ushort)yY;
                            FloorItem.FloorPacket.Name = "SeaBurialTrap";
                            FloorItem.FloorPacket.Timer = 3;
                            FloorItem.FloorPacket.MapID = user.Player.Map;
                            user.Player.FloorSpells[ClientSpell.ID].AddItem(FloorItem);
                            using (var recFi = new ServerSockets.RecycledPacket())
                                user.Player.View.SendView(recFi.GetStream().ItemPacketCreate(FloorItem.FloorPacket), true);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.Wildwind:
                        {

                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                               , 0, Attack.X, Attack.Y, ClientSpell.ID
                                               , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            byte Distance = (byte)Calculate.Base.GetDistance(Attack.X, Attack.Y, user.Player.X, user.Player.Y);
                            if (Distance >= DBSpell.Range)
                                break;

                            Algoritms.InLineAlgorithm Line = new Algoritms.InLineAlgorithm(user.Player.X, Attack.X, user.Player.Y, Attack.Y, user.Map, Distance, 0);


                            uint Experience = 0;
                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                if (Line.InLine(target.X, target.Y, 2))
                                {
                                    MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                    if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                        Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                if (Line.InLine(targer.X, targer.Y, 2))
                                {
                                    var attacked = targer as Role.Player;
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                    {

                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }

                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                if (Line.InLine(targer.X, targer.Y, 2))
                                {
                                    var attacked = targer as Role.SobNpc;
                                    if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                    {
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
                            MsgSpell.SendRole(user);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.Megabolt:
                        {

                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                               , 0, Attack.X, Attack.Y, ClientSpell.ID
                                               , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            byte Distance = (byte)Calculate.Base.GetDistance(Attack.X, Attack.Y, user.Player.X, user.Player.Y);
                            if (Distance >= DBSpell.Range)
                                break;

                            Algoritms.InLineAlgorithm Line = new Algoritms.InLineAlgorithm(user.Player.X, Attack.X, user.Player.Y, Attack.Y, user.Map, Distance, 0);


                            uint Experience = 0;
                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                if (Line.InLine(target.X, target.Y, 1))
                                {
                                    MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                    if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                    {
                                        if (Calculate.Base.GetDistance(attacked.X, attacked.Y, user.Player.X, user.Player.Y) > 11) continue;
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);

                                        Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                if (Line.InLine(targer.X, targer.Y, 1))
                                {
                                    var attacked = targer as Role.Player;
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                    {
                                        if (Calculate.Base.GetDistance(attacked.X, attacked.Y, user.Player.X, user.Player.Y) > 11) continue;
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                        // [CombatAudit 2026-10-14] removed custom nerf (official Megabolt tooltip 98-120% = status_data0-30000): AnimationObj.Damage = AnimationObj.Damage * 90 / 100; // تعديل الدمج
                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);

                                    }
                                }

                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                if (Line.InLine(targer.X, targer.Y, 1))
                                {
                                    var attacked = targer as Role.SobNpc;
                                    if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                    {
                                        if (Calculate.Base.GetDistance(attacked.X, attacked.Y, user.Player.X, user.Player.Y) > 11) continue;
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
                            MsgSpell.SendRole(user);
                            MsgSpell.Send(user);
                            break;
                        }
                }
            }
        }
    }
}