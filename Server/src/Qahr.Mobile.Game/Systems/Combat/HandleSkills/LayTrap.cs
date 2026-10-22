using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgFloorItem;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{
    public class LayTrap
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                switch (ClientSpell.ID)
                {
                    case (ushort)Role.Flags.SpellID.DragonRising:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            if (!user.Player.FloorSpells.ContainsKey(ClientSpell.ID))
                            {
                                user.Player.FloorSpells.TryAdd(ClientSpell.ID, new Role.FloorSpell.ClientFloorSpells(user.Player.UID, user.Player.X, user.Player.Y, ClientSpell.SoulLevel, DBSpell, user.Map));
                            }
                            var FloorItem = new Role.FloorSpell((uint)Game.MsgFloorItem.MsgItemPacket.DragonRising, (ushort)Attack.X, (ushort)Attack.Y, 33, DBSpell, 3000, 1000);
                            FloorItem.FloorPacket.m_X = Attack.X;
                            FloorItem.FloorPacket.m_Y = Attack.Y;
                            FloorItem.FloorPacket.ItemOwnerUID = user.Player.UID;
                            FloorItem.FloorPacket.m_Color = 33;
                            FloorItem.FloorPacket.Name = "STR_TRAP_ID_@@" + (uint)FloorItem.FloorPacket.m_ID;
                            user.Player.FloorSpells[ClientSpell.ID].AddItem(FloorItem);
                            user.Player.View.SendView(stream.ItemPacketCreate(FloorItem.FloorPacket), true);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.FloraWard:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            MsgItem DropItem = new MsgItem(new MsgGameItem() { Color = (Qahr.Mobile.Role.Flags.Color)11, ITEM_ID = 4030 }, Attack.X, Attack.Y, MsgItem.ItemType.Effect, 0, user.Player.DynamicID, user.Player.Map, user.Player.UID, false, user.Map, (int)DBSpell.Duration);
                            DropItem.TimeStamp = 5;
                            DropItem.OwnerEffert = user;
                            DropItem.DBSkill = DBSpell;
                            if (user.Map.EnqueueItem(DropItem))
                                DropItem.SendAll(stream, MsgDropID.Effect);
                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);


                            break;
                        }
                    case (ushort)Role.Flags.SpellID.SubstitutionAttack:
                        {
                            if (user.Player.ContainFlag(MsgUpdate.Flags.SubstitutionAttack))
                            {
                                user.Player.RemoveFlag(MsgUpdate.Flags.SubstitutionAttack);
                                user.Player.SendUpdate(stream, MsgUpdate.Flags.Substitution, (uint)0, (uint)60000, (uint)0, MsgUpdate.DataType.AzureShield, true);
                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                                if (!user.Player.FloorSpells.ContainsKey(ClientSpell.ID))
                                    user.Player.FloorSpells.TryAdd(ClientSpell.ID, new Role.FloorSpell.ClientFloorSpells(user.Player.UID, user.Player.X, user.Player.Y, ClientSpell.SoulLevel, DBSpell, user.Map));
                                var FloorItem = new Role.FloorSpell(Game.MsgFloorItem.MsgItemPacket.SubstitutionFloor, (ushort)Attack.X, (ushort)Attack.Y, 23, DBSpell, 1000);
                                FloorItem.FloorPacket.m_X = Attack.X;
                                FloorItem.FloorPacket.m_Y = Attack.Y;
                                FloorItem.FloorPacket.OwnerX = user.Player.X;
                                FloorItem.FloorPacket.OwnerY = user.Player.Y;
                                FloorItem.FloorPacket.GuildID = user.Player.GuildID;
                                FloorItem.FloorPacket.ItemOwnerUID = user.Player.UID;
                                FloorItem.FloorPacket.Name = "STR_TRAP_ID_4050@@";
                                FloorItem.FloorPacket.m_Color = 14;
                                user.Player.FloorSpells[ClientSpell.ID].AddItem(FloorItem);
                                user.Player.View.SendView(stream.ItemPacketCreate(FloorItem.FloorPacket), true);
                                MsgSpell.SetStream(stream);
                                MsgSpell.Send(user);
                            }
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.MysticalMelody:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            if (!user.Player.FloorSpells.ContainsKey(ClientSpell.ID))
                                user.Player.FloorSpells.TryAdd(ClientSpell.ID, new Role.FloorSpell.ClientFloorSpells(user.Player.UID, user.Player.X, user.Player.Y, ClientSpell.SoulLevel, DBSpell, user.Map));
                            var FloorItem = new Role.FloorSpell(Game.MsgFloorItem.MsgItemPacket.MysticalMelody, (ushort)Attack.X, (ushort)Attack.Y, 23, DBSpell, 4000, 2000);
                            FloorItem.FloorPacket.m_X = Attack.X;
                            FloorItem.FloorPacket.m_Y = Attack.Y;
                            FloorItem.FloorPacket.OwnerX = user.Player.X;
                            FloorItem.FloorPacket.OwnerY = user.Player.Y;
                            FloorItem.FloorPacket.GuildID = user.Player.GuildID;
                            FloorItem.FloorPacket.ItemOwnerUID = user.Player.UID;
                            FloorItem.FloorPacket.Name = "STR_TRAP_ID_4040@@";
                            FloorItem.FloorPacket.m_Color = 23;
                            user.Player.FloorSpells[ClientSpell.ID].AddItem(FloorItem);
                            user.Player.View.SendView(stream.ItemPacketCreate(FloorItem.FloorPacket), true);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    #region Mele Archive (Ninja)
                    case (ushort)Role.Flags.SpellID.WaterShockwave:
                    case (ushort)Role.Flags.SpellID.WaterShockwavePassive:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            if (!user.Player.FloorSpells.ContainsKey(ClientSpell.ID))
                            {
                                user.Player.FloorSpells.TryAdd(ClientSpell.ID, new Role.FloorSpell.ClientFloorSpells(user.Player.UID, user.Player.X, user.Player.Y, ClientSpell.SoulLevel, DBSpell, user.Map));
                            }
                            var FloorItem = new Role.FloorSpell(Game.MsgFloorItem.MsgItemPacket.WaterShockwave, (ushort)Attack.X, (ushort)Attack.Y, 23, DBSpell, 3000, 1000);
                            FloorItem.FloorPacket.OwnerX = Attack.X;
                            FloorItem.FloorPacket.OwnerY = Attack.Y;
                            FloorItem.FloorPacket.m_X = user.Player.X;
                            FloorItem.FloorPacket.m_Y = user.Player.Y;
                            FloorItem.FloorPacket.GuildID = user.Player.GuildID;
                            FloorItem.FloorPacket.ItemOwnerUID = user.Player.UID;
                            FloorItem.FloorPacket.m_Color = 23;
                            FloorItem.FloorPacket.Name = "STR_TRAP_ID_2537@@";
                            user.Player.FloorSpells[ClientSpell.ID].AddItem(FloorItem);
                            user.Player.View.SendView(stream.ItemPacketCreate(FloorItem.FloorPacket), true);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.FlameofDestruction:
                    case (ushort)Role.Flags.SpellID.FlameofDestructionPassive:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            if (!user.Player.FloorSpells.ContainsKey(ClientSpell.ID))
                            {
                                user.Player.FloorSpells.TryAdd(ClientSpell.ID, new Role.FloorSpell.ClientFloorSpells(user.Player.UID, user.Player.X, user.Player.Y, ClientSpell.SoulLevel, DBSpell, user.Map));
                            }
                            var FloorItem = new Role.FloorSpell(Game.MsgFloorItem.MsgItemPacket.FlameofDestruction, (ushort)Attack.X, (ushort)Attack.Y, 14, DBSpell, 3000, 1000);
                            FloorItem.FloorPacket.m_X = Attack.X;
                            FloorItem.FloorPacket.m_Y = Attack.Y;
                            FloorItem.FloorPacket.OwnerX = user.Player.X;
                            FloorItem.FloorPacket.OwnerY = user.Player.Y;
                            FloorItem.FloorPacket.GuildID = user.Player.GuildID;
                            FloorItem.FloorPacket.ItemOwnerUID = 0;
                            FloorItem.FloorPacket.m_Color = 14;
                            user.Player.FloorSpells[ClientSpell.ID].AddItem(FloorItem);
                            user.Player.View.SendView(stream.ItemPacketCreate(FloorItem.FloorPacket), true);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.DustDetachment:
                    case (ushort)Role.Flags.SpellID.DustDetachmentPassive:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            if (!user.Player.FloorSpells.ContainsKey(ClientSpell.ID))
                            {
                                user.Player.FloorSpells.TryAdd(ClientSpell.ID, new Role.FloorSpell.ClientFloorSpells(user.Player.UID, user.Player.X, user.Player.Y, ClientSpell.SoulLevel, DBSpell, user.Map));
                            }
                            var FloorItem = new Role.FloorSpell(Game.MsgFloorItem.MsgItemPacket.DustDetachment, (ushort)Attack.X, (ushort)Attack.Y, 14, DBSpell, 3000, 1000);
                            FloorItem.FloorPacket.m_X = Attack.X;
                            FloorItem.FloorPacket.m_Y = Attack.Y;
                            FloorItem.FloorPacket.OwnerX = user.Player.X;
                            FloorItem.FloorPacket.OwnerY = user.Player.Y;
                            FloorItem.FloorPacket.GuildID = user.Player.GuildID;
                            FloorItem.FloorPacket.ItemOwnerUID = 0;
                            FloorItem.FloorPacket.m_Color = 14;
                            user.Player.FloorSpells[ClientSpell.ID].AddItem(FloorItem);
                            user.Player.View.SendView(stream.ItemPacketCreate(FloorItem.FloorPacket), true);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.SickleWind:
                    case (ushort)Role.Flags.SpellID.SickleWindPassive:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            if (!user.Player.FloorSpells.ContainsKey(ClientSpell.ID))
                            {
                                user.Player.FloorSpells.TryAdd(ClientSpell.ID, new Role.FloorSpell.ClientFloorSpells(user.Player.UID, user.Player.X, user.Player.Y, ClientSpell.SoulLevel, DBSpell, user.Map));
                            }
                            var FloorItem = new Role.FloorSpell(Game.MsgFloorItem.MsgItemPacket.SickleWind, (ushort)Attack.X, (ushort)Attack.Y, 14, DBSpell, 3000, 1000);
                            FloorItem.FloorPacket.m_X = Attack.X;
                            FloorItem.FloorPacket.m_Y = Attack.Y;
                            FloorItem.FloorPacket.OwnerX = user.Player.X;
                            FloorItem.FloorPacket.OwnerY = user.Player.Y;
                            FloorItem.FloorPacket.GuildID = user.Player.GuildID;
                            FloorItem.FloorPacket.ItemOwnerUID = 0;
                            FloorItem.FloorPacket.m_Color = 14;
                            user.Player.FloorSpells[ClientSpell.ID].AddItem(FloorItem);
                            user.Player.View.SendView(stream.ItemPacketCreate(FloorItem.FloorPacket), true);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    #endregion
                    case (ushort)Role.Flags.SpellID.PeaceofStomper:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);


                            Role.IMapObj _target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out _target, Role.MapObjectType.Player)
                                || user.Player.View.TryGetValue(Attack.OpponentUID, out _target, Role.MapObjectType.Monster)
                                || user.Player.View.TryGetValue(Attack.OpponentUID, out _target, Role.MapObjectType.SobNpc))
                            {
                                if (!user.Player.FloorSpells.ContainsKey(DBSpell.ID))
                                    user.Player.FloorSpells.TryAdd(DBSpell.ID, new Role.FloorSpell.ClientFloorSpells(user.Player.UID, (ushort)_target.X, (ushort)_target.Y, ClientSpell.SoulLevel, DBSpell, user.Map));
                                var FloorItem = new Role.FloorSpell(Game.MsgFloorItem.MsgItemPacket.HorrorofStomper, (ushort)_target.X, _target.Y, 14, DBSpell, 2000);
                                FloorItem.FloorPacket.DontShow = 1;
                                FloorItem.FloorPacket.OwnerX = user.Player.X;
                                FloorItem.FloorPacket.OwnerY = user.Player.Y;
                                FloorItem.FloorPacket.GuildID = user.Player.GuildID;
                                FloorItem.FloorPacket.ItemOwnerUID = user.Player.UID;
                                FloorItem.FloorPacket.m_Color = 14;
                                user.Player.FloorSpells[DBSpell.ID].AddItem(FloorItem);
                                user.Player.View.SendView(stream.ItemPacketCreate(FloorItem.FloorPacket), true);
                            }

                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 10000, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.BloomofDeath:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                     , 0, Attack.X, Attack.Y, ClientSpell.ID
                                     , ClientSpell.Level, ClientSpell.UseSpellSoul, 1);


                            uint Experience = 0;
                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) <= 4)
                                {
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
                                var attacked = targer as Role.Player;
                                if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) <= 4)
                                {
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                        // [CombatAudit 2026-10-14] removed custom nerf (official tooltip 180-245% = power): AnimationObj.Damage = AnimationObj.Damage * 50 / 100; // Mohe Done (Attack)
                                        AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);

                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }

                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                var attacked = targer as Role.SobNpc;
                                if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) <= 4)
                                {
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

                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 10000, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.SkyFall:
                        {
                            if (!Database.AtributesStatus.IsThunderStriker(user.Player.Class)) break;
                            if (user.Player.ThunderStrikerEnergy >= 100)
                            {
                                user.Player.ThunderStrikerEnergy -= 100;
                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                    , 0, Attack.X, Attack.Y, ClientSpell.ID
                                    , ClientSpell.Level, ClientSpell.UseSpellSoul, 1);


                                uint Experience = 0;
                                foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                                {
                                    MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                    if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= 4)
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
                                    if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= 4)
                                    {
                                        if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                        {
                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                            // [CombatAudit 2026-10-14] removed custom nerf (official SkyFall tooltip = status_data0-30000): AnimationObj.Damage = AnimationObj.Damage * 90 / 100; // تعديل الدمج
                                            ReceiveAttack.Player.Execute(AnimationObj, user, attacked);

                                            MsgSpell.Targets.Enqueue(AnimationObj);
                                        }
                                    }

                                }
                                foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                                {
                                    var attacked = targer as Role.SobNpc;
                                    if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= 4)
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

                                Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 10000, DBSpells);
                                MsgSpell.SetStream(stream);
                                MsgSpell.Send(user);
                            }
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.HorrorofStomper:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, user.Player.X, user.Player.Y, ClientSpell.ID
                                , 0, ClientSpell.UseSpellSoul);


                            Role.IMapObj _target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out _target, Role.MapObjectType.Player)
                                || user.Player.View.TryGetValue(Attack.OpponentUID, out _target, Role.MapObjectType.Monster)
                                || user.Player.View.TryGetValue(Attack.OpponentUID, out _target, Role.MapObjectType.SobNpc))
                            {
                                if (!user.Player.FloorSpells.ContainsKey(DBSpell.ID))
                                    user.Player.FloorSpells.TryAdd(DBSpell.ID, new Role.FloorSpell.ClientFloorSpells(user.Player.UID, (ushort)_target.X, (ushort)_target.Y, ClientSpell.SoulLevel, DBSpell, user.Map));
                                var FloorItem = new Role.FloorSpell(Game.MsgFloorItem.MsgItemPacket.HorrorofStomper, (ushort)_target.X, _target.Y, 14, DBSpell, 1000);
                                FloorItem.FloorPacket.DontShow = 1;
                                FloorItem.FloorPacket.OwnerX = user.Player.X;
                                FloorItem.FloorPacket.OwnerY = user.Player.Y;
                                FloorItem.FloorPacket.GuildID = user.Player.GuildID;
                                FloorItem.FloorPacket.Angle = user.Player.Angle;
                                FloorItem.FloorPacket.ItemOwnerUID = user.Player.UID;
                                FloorItem.FloorPacket.m_Color = 14;
                                user.Player.FloorSpells[ClientSpell.ID].AddItem(FloorItem);
                                user.Player.View.SendView(stream.ItemPacketCreate(FloorItem.FloorPacket), true);
                            }

                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 10000, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.TwilightDance:
                        {

                            Attack.UID = user.Player.UID;
                            Attack.OpponentUID = user.Player.UID;
                            Attack.Damage = 0;
                            Attack.AtkType = 0;

                            user.Player.TwilightDance = 0;
                            user.Send(stream.InteractionCreate(Attack));

                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            Algoritms.LayTrapThree Line = new Algoritms.LayTrapThree(user.Player.X, Attack.X, user.Player.Y, Attack.Y, 160);

                            int Stamp = 0;
                            byte Color = 0;
                            List<MsgFloorItem.MsgItem> Items = new List<MsgFloorItem.MsgItem>();
                            foreach (var coords in Line.LCoords)
                            {
                                if (!user.Player.FloorSpells.ContainsKey(ClientSpell.ID))
                                    user.Player.FloorSpells.TryAdd(ClientSpell.ID, new Role.FloorSpell.ClientFloorSpells(user.Player.UID, Attack.X, Attack.Y, ClientSpell.SoulLevel, DBSpell, user.Map));

                                var FloorItem = new Role.FloorSpell(Game.MsgFloorItem.MsgItemPacket.TwilightDance, (ushort)coords.X, (ushort)coords.Y, Color, DBSpell, Stamp);
                                FloorItem.FloorPacket.UnKnow += 1;
                                FloorItem.FloorPacket.OwnerX = user.Player.X;
                                FloorItem.FloorPacket.OwnerY = user.Player.Y;
                                FloorItem.FloorPacket.Name = "trap";
                                FloorItem.FloorPacket.ItemOwnerUID = user.Player.UID;
                                user.Player.FloorSpells[ClientSpell.ID].AddItem(FloorItem);
                                user.Player.View.SendView(stream.ItemPacketCreate(FloorItem.FloorPacket), true);
                                Stamp += 300;
                            }

                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 5000, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);


                            break;
                        }

                    case (ushort)Role.Flags.SpellID.InfernalEcho:
                        {

                            Attack.UID = user.Player.UID;
                            Attack.OpponentUID = user.Player.UID;
                            Attack.Damage = 0;
                            Attack.AtkType = 0;


                            user.Send(stream.InteractionCreate(Attack));

                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);


                            Algoritms.RandomFourLayTraps location = new Algoritms.RandomFourLayTraps(user.Player.X, user.Player.Y);

                            foreach (var coords in location.Coords)
                            {

                                if (!user.Player.FloorSpells.ContainsKey(ClientSpell.ID))
                                    user.Player.FloorSpells.TryAdd(ClientSpell.ID, new Role.FloorSpell.ClientFloorSpells(user.Player.UID, Attack.X, Attack.Y, ClientSpell.SoulLevel, DBSpell, user.Map));
                                var FloorItem = new Role.FloorSpell(Game.MsgFloorItem.MsgItemPacket.InfernalEcho, (ushort)coords.X, (ushort)coords.Y, 14, DBSpell, 4000);
                                user.Player.FloorSpells[ClientSpell.ID].AddItem(FloorItem);

                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj() { Damage = 1, MoveX = user.Player.X, Hit = 1, MoveY = user.Player.Y, UID = FloorItem.FloorPacket.m_UID });
                                user.Player.View.SendView(stream.ItemPacketCreate(FloorItem.FloorPacket), true);
                            }


                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 10000, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);


                            break;
                        }
                    case (ushort)Role.Flags.SpellID.SpaceLeap:
                        {

                            if (Pool.Constants.SkillsNotAvailableHere.Contains(user.Player.Map))
                            {
                                user.SendSysMesage("Sorry, This Rune Not Work In This Map.");
                                return;
                            }

                            if (user.Player.Map == 5061 || user.Player.Map == 5062 || user.Player.Map == 5063 || user.Player.Map == 5064 || user.Player.Map == 5065 || user.Player.Map == 5066 || user.Player.Map == 5051 || user.Player.Map == 5052 || user.Player.Map == 5053 || user.Player.Map == 5054 || user.Player.Map == 5055 || user.Player.Map == 5056 || user.Player.Map == 5057 || user.Player.Map == 5058)
                            {
                                user.SendSysMesage("Sorry, This Rune Not Work In This Map.");
                                return;
                            }

                            if (user.Player.Map == 11 || user.Player.Map == 7 || user.Player.Map == 5 || user.Player.Map == 15 || user.Player.Map == 14 || user.Player.Map == 12 || user.Player.Map == 8 || user.Player.Map == 20 || user.Player.Map == 1730)
                            {
                                user.SendSysMesage("Sorry, This Rune Not Work In This Map.");
                                return;
                            }

                            if (user.Player.Map == 12667 || user.Player.Map == 3030 || user.Player.Map == 12275 || user.Player.Map == 12602 || user.Player.Map == 10741 || user.Player.Map == 2071 || user.Player.Map == 3030 || user.Player.Map == 10133 || user.Player.Map == 3071)
                            {
                                user.SendSysMesage("Sorry, This Rune Not Work In This Map.");
                                return;
                            }

                            if (user.Player.Map == 10134 || user.Player.Map == 12099 || user.Player.Map == 10133 || user.Player.Map == 3071 || user.Player.Map == 22348 || user.Player.Map == 12344 || user.Player.Map == 10070 || user.Player.Map == 10504 || user.Player.Map == 12344 || user.Player.Map == 6521 || user.Player.Map == 95152)
                            {
                                user.SendSysMesage("Sorry, This Rune Not Work In This Map.");
                                return;
                            }
                            if (user.Player.Map == 12546 || user.Player.Map == 13249 || user.Player.Map == 12602 || user.Player.Map == 12275 || user.Player.Map == 12602 || user.Player.Map == 13140 || user.Player.Map == 12546 || user.Player.Map == 12800 || user.Player.Map == 11331 || user.Player.Map == 12799 || user.Player.Map == 11996 || user.Player.Map == 12665 || user.Player.Map == 12601 || user.Player.Map == 12816 || user.Player.Map == 12099 || user.Player.Map == 66666 || user.Player.Map == 33333 || user.Player.Map == 22222 || user.Player.Map == 88888)
                            {
                                user.SendSysMesage("Sorry, This Rune Not Work In This Map.");
                                return;
                            }
                            if (user.Player.Map == 10133 || user.Player.Map == 12546 || user.Player.Map == 11222 || user.Player.Map == 10741 || user.Player.Map == 2071 || user.Player.Map == 12602 || user.Player.Map == 12275 || user.Player.Map == 3071)
                            {
                                user.SendSysMesage("Sorry, This Rune Not Work In This Map.");
                                return;
                            }
                            if (user.InQualifier() || user.Map.ID > 10000000) break;
                            if (!Database.AtributesStatus.IsWater(user.Player.Class)) break;

                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            if (!user.Player.FloorSpells.ContainsKey(ClientSpell.ID))
                                user.Player.FloorSpells.TryAdd(ClientSpell.ID, new Role.FloorSpell.ClientFloorSpells(user.Player.UID, Attack.X, Attack.Y, ClientSpell.SoulLevel, DBSpell, user.Map));
                            var FloorItem = new Role.FloorSpell(Game.MsgFloorItem.MsgItemPacket.SpaceLeap, Attack.X, (ushort)Attack.Y, 17, DBSpell, 17000);
                            FloorItem.FloorPacket.FlowerType = 3;
                            FloorItem.FloorPacket.Life = FloorItem.FloorPacket.MaxLife = 5;
                            FloorItem.FloorPacket.ItemOwnerUID = user.Player.UID;
                            FloorItem.FloorPacket.GuildID = user.Player.GuildID;
                            FloorItem.FloorPacket.Name = "SpaceLeap";
                            FloorItem.FloorPacket.MapID = user.Player.Map;
                            user.Player.FloorSpells[ClientSpell.ID].AddItem(FloorItem);
                            using (var recycledPacket = new ServerSockets.RecycledPacket())
                            {
                                var streamm = recycledPacket.GetStream();
                                user.Player.View.SendView(streamm.ItemPacketCreate(FloorItem.FloorPacket), true);
                            }
                            if (user.Team != null)
                            {
                                foreach (var target in user.Team.GetMembers())
                                {
                                    using (var recycledPacket = new ServerSockets.RecycledPacket())
                                    {
                                        var streamm = recycledPacket.GetStream();
                                        target.Send(streamm.ActionCreate(new ActionQuery() { dwParam = user.Player.UID, Timestamp = (uint)Time32.timeGetTime().GetHashCode(), Type = ActionType.PortalInvite, RemainTime = 11, Strings = new string[1] { user.Player.Name } }));
                                    }
                                }
                            }

                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj() { Damage = 0, Hit = 1, MoveY = user.Player.Y, UID = user.Player.UID });


                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 10000, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);


                            break;
                        }
                    case (ushort)Role.Flags.SpellID.WrathoftheEmperor:
                        {



                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);


                            Role.IMapObj _target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out _target, Role.MapObjectType.Player)
                                || user.Player.View.TryGetValue(Attack.OpponentUID, out _target, Role.MapObjectType.Monster)
                                || user.Player.View.TryGetValue(Attack.OpponentUID, out _target, Role.MapObjectType.SobNpc))
                            {
                                if (!user.Player.FloorSpells.ContainsKey(ClientSpell.ID))
                                    user.Player.FloorSpells.TryAdd(ClientSpell.ID, new Role.FloorSpell.ClientFloorSpells(user.Player.UID, Attack.X, Attack.Y, ClientSpell.SoulLevel, DBSpell, user.Map));
                                var FloorItem = new Role.FloorSpell(Game.MsgFloorItem.MsgItemPacket.WrathoftheEmperor, (ushort)_target.X, (ushort)_target.Y, 14, DBSpell, 1500);
                                user.Player.FloorSpells[ClientSpell.ID].AddItem(FloorItem);

                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj() { Damage = 1, MoveX = user.Player.X, Hit = 1, MoveY = user.Player.Y, UID = FloorItem.FloorPacket.m_UID });
                                user.Player.View.SendView(stream.ItemPacketCreate(FloorItem.FloorPacket), true);

                            }

                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 10000, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);


                            break;
                        }
                    case (ushort)Role.Flags.SpellID.AuroraLotus:
                        {
                            if (user.Player.Map == 5061 || user.Player.Map == 5062 || user.Player.Map == 5063 || user.Player.Map == 5064 || user.Player.Map == 5065 || user.Player.Map == 5066 || user.Player.Map == 5051 || user.Player.Map == 5052 || user.Player.Map == 5053 || user.Player.Map == 5054 || user.Player.Map == 5055 || user.Player.Map == 5056 || user.Player.Map == 5057 || user.Player.Map == 5058)
                            {
                                user.SendSysMesage("Sorry, This Spell Not Work In This Map.");
                                return;
                            }
                            if (user.Player.Map == 10133 || user.Player.Map == 2071 || user.Player.Map == 3071 || user.Player.Map == 3030 || user.Player.Map == 12602 || user.Player.Map == 12275 || user.Player.Map == 10741 || user.Player.Map == 12546 || user.Player.Map == 13249 || user.Player.Map == 12602 || user.Player.Map == 13140 || user.Player.Map == 12546 || user.Player.Map == 12800 || user.Player.Map == 11331 || user.Player.Map == 11996 || user.Player.Map == 12665 || user.Player.Map == 12601 || user.Player.Map == 66666 || user.Player.Map == 33333 || user.Player.Map == 22222 || user.Player.Map == 88888)
                            {
                                user.SendSysMesage("Sorry, This Spell Not Work In This Map.");
                                return;
                            }
                            if (user.Player.TaoistPower >= 9)
                            {
                                Attack.UID = user.Player.UID;
                                Attack.OpponentUID = user.Player.UID;
                                Attack.Damage = 0;
                                Attack.AtkType = 0;


                                user.Send(stream.InteractionCreate(Attack));

                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                    , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                                    , ClientSpell.Level, ClientSpell.UseSpellSoul);


                                if (Server.AddFloor(stream, user.Map, Game.MsgFloorItem.MsgItemPacket.AuroraLotus, Attack.X, Attack.Y, ClientSpell.Level, DBSpell, user, user.Player.GuildID, user.Player.UID, user.Player.DynamicID, "STR_TRAP_ID_1310@@", true))
                                {
                                    MsgSpell.SetStream(stream);
                                    MsgSpell.Send(user);
                                    Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 10000, DBSpells);
                                    user.Player.TaoistPower = 0;
                                    user.Player.UpdateTaoPower(stream);
                                    user.Player.RemoveFlag(MsgUpdate.Flags.FullPowerWater);
                                }
                                else
                                {
#if Arabic
                                     user.SendSysMesage("Invalid Aurora location.");
#else
                                    user.SendSysMesage("Invalid Aurora location.");
#endif
                                }
                            }


                            break;
                        }
                    case (ushort)Role.Flags.SpellID.FlameLotus:
                        {
                            if (user.Player.TaoistPower >= 9)
                            {
                                Attack.UID = user.Player.UID;
                                Attack.OpponentUID = user.Player.UID;
                                Attack.Damage = 0;
                                Attack.AtkType = 0;


                                user.Send(stream.InteractionCreate(Attack));

                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                    , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                                    , ClientSpell.Level, ClientSpell.UseSpellSoul);

                                if (user.Map.AddFloor(stream, Game.MsgFloorItem.MsgItemPacket.FlameLotus, Attack.X, Attack.Y, ClientSpell.Level, DBSpell, user, user.Player.GuildID, user.Player.UID, user.Player.DynamicID, "FlameLotus", true))
                                {
                                    MsgSpell.SetStream(stream);
                                    MsgSpell.Send(user);

                                    Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 10000, DBSpells);
                                    user.Player.TaoistPower = 0;
                                    user.Player.UpdateTaoPower(stream);

                                    user.Player.RemoveFlag(MsgUpdate.Flags.FullPowerFire);
                                }
                                else
                                {

                                    user.SendSysMesage("Invalid Aurora location.");



                                }
                            }

                            break;
                        }
                    case (ushort)Role.Flags.SpellID.DaggerStorm:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);


                            MsgServer.MsgGameItem item = new MsgGameItem();
                            item.Color = (Role.Flags.Color)2;

                            if (ClientSpell.UseSpellSoul == 0)
                                item.ITEM_ID = MsgFloorItem.MsgItemPacket.NormalDaggerStorm;
                            else if (ClientSpell.UseSpellSoul == 1)
                                item.ITEM_ID = MsgFloorItem.MsgItemPacket.SoulOneDaggerStorm;
                            else if (ClientSpell.UseSpellSoul == 2)
                                item.ITEM_ID = MsgFloorItem.MsgItemPacket.SoulTwoDaggerStorm;

                            MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(item, Attack.X, Attack.Y, MsgFloorItem.MsgItem.ItemType.Effect, 0, user.Player.DynamicID, user.Player.Map
                                   , user.Player.UID, false, user.Map, 4);
                            DropItem.MsgFloor.Name = "trap";
                            DropItem.MsgFloor.m_Color = 11;
                            DropItem.MsgFloor.FlowerType = 11;
                            DropItem.MsgFloor.ItemOwnerUID = user.Player.UID;
                            DropItem.MsgFloor.GuildID = user.Player.GuildID;
                            DropItem.OwnerEffert = user;
                            DropItem.DBSkill = DBSpell;

                            if (user.Map.EnqueueItem(DropItem))
                            {
                                DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Effect);
                            }
                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));

                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 100, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                }
            }
        }
    }
}
