using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgFloorItem;
using Qahr.Mobile.Game.MsgServer.AttackHandler.Algoritms;


namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{
    public class MoveLine
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
               
                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                   , 0, Attack.X, Attack.Y, ClientSpell.ID
                , ClientSpell.Level, ClientSpell.UseSpellSoul);
                byte dist = (byte)DBSpell.DamageOnMonster;
                List<Algoritms.InLineAlgorithm.coords> coord = Algoritms.MoveCoords.CheckBladeTeampsCoords(user.Player.X, user.Player.Y, Attack.X
                     , Attack.Y, user.Map, dist);
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

           
                if (user.Rune.IsEquipped("TideTrap") && user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.TideTrap))
                {
                    MsgSpell.bomb = 1;
                }
                else
                {

                    user.Map.View.MoveTo<Role.IMapObj>(user.Player, MsgSpell.X, MsgSpell.Y);
                    user.Player.X = MsgSpell.X;
                    user.Player.Y = MsgSpell.Y;
                    MsgGameItem RightWeapon;
                    MsgGameItem LeftWeapon;
                    if (user.Equipment.TryGetEquip(Role.Flags.ConquerItem.RightWeapon, out RightWeapon)
                        && user.Equipment.TryGetEquip(Role.Flags.ConquerItem.LeftWeapon, out LeftWeapon))
                    {
                        if (Database.ItemType.IsPirateEpicWeapon(RightWeapon.ITEM_ID) && Database.ItemType.IsPirateEpicWeapon(LeftWeapon.ITEM_ID))
                        {
                            MsgSpell.bomb = 2;
                        }
                    }
                
                }

                uint Experience = 0;
             
                foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                {
                    bool hit = false;
                    for (int j = 0; j < coord.Count; j++)
                        if (Calculate.Base.GetDDistance(target.X, target.Y, (ushort)coord[j].X, (ushort)coord[j].Y) <= 1.5)
                            hit = true;
                    if (hit)
                    {
                        MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                        if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                        {
                            MsgSpellAnimation.SpellObj AnimationObj;
                            Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                           
                            Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                            MsgSpell.Targets.Enqueue(AnimationObj);
                            if (target.Alive)
                            {
                                MsgSpell blackspotSpell;
                                Database.MagicType.Magic blackspotDBSpell;
                                if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.Blackspot, out blackspotSpell))
                                {
                                    Dictionary<ushort, Database.MagicType.Magic> bDBSpells;
                                    if (Pool.Magic.TryGetValue((ushort)Role.Flags.SpellID.Blackspot, out bDBSpells))
                                    {
                                        if (bDBSpells.TryGetValue(blackspotSpell.Level, out blackspotDBSpell))
                                        {
                                            if (Role.Core.Rate(blackspotDBSpell.Rate))
                                            {
                                                attacked.BlackSpot = true;
                                                attacked.Stamp_BlackSpot = DateTime.Now.AddSeconds((int)blackspotDBSpell.Duration);
                                                user.Player.View.SendView(stream.BlackspotCreate(true, attacked.UID), true);
                                            }
                                        }
                                    }
                                }
                            }

                        }
                    }
                }
                foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                {
                    bool hit = false;
                    for (int j = 0; j < coord.Count; j++)
                        if (Calculate.Base.GetDDistance(targer.X, targer.Y, (ushort)coord[j].X, (ushort)coord[j].Y) <= 1.5)
                            hit = true;
                    if (hit)
                    {
                        var attacked = targer as Role.Player;

                        if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                        {

                            MsgSpellAnimation.SpellObj AnimationObj;
                            Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                            AnimationObj.Damage = AnimationObj.Damage * 78 / 100;
                            ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                            MsgSpell.Targets.Enqueue(AnimationObj);
                            if (attacked.Alive)
                            {
                                MsgSpell blackspotSpell;
                                Database.MagicType.Magic blackspotDBSpell;
                                if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.Blackspot, out blackspotSpell))
                                {
                                    Dictionary<ushort, Database.MagicType.Magic> bDBSpells;
                                    if (Pool.Magic.TryGetValue((ushort)Role.Flags.SpellID.Blackspot, out bDBSpells))
                                    {
                                        if (bDBSpells.TryGetValue(blackspotSpell.Level, out blackspotDBSpell))
                                        {
                                            if (Role.Core.Rate(blackspotDBSpell.Rate))
                                            {
                                                attacked.BlackSpot = true;
                                                attacked.EagleEyeCountDown = true;
                                                attacked.Stamp_BlackSpot = DateTime.Now.AddSeconds((int)blackspotDBSpell.Duration);
                                                user.Player.View.SendView(stream.BlackspotCreate(true, attacked.UID), true);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
                foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                {
                    bool hit = false;
                    for (int j = 0; j < coord.Count; j++)
                        if (Calculate.Base.GetDDistance(targer.X, targer.Y, (ushort)coord[j].X, (ushort)coord[j].Y) <= 1.5)
                            hit = true;
                    if (hit)
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
                if (MsgSpell.bomb > 0 && MsgSpell.Targets.Count > 0)
                {
                    byte distr = (byte)DBSpell.DamageOnMonster;
                    coord = Algoritms.MoveCoords.CheckBladeTeampsCoords(user.Player.X, user.Player.Y, Attack.X
                                      , Attack.Y, user.Map, distr);
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

                    if (!user.Player.FloorSpells.ContainsKey((ushort)Role.Flags.SpellID.TideTrap))
                        user.Player.FloorSpells.TryAdd((ushort)Role.Flags.SpellID.TideTrap, new Role.FloorSpell.ClientFloorSpells(user.Player.UID, Attack.X, Attack.Y, 0, Pool.Magic[(ushort)Role.Flags.SpellID.TideTrap][0], user.Map));
                    var FloorItem = new Role.FloorSpell(Game.MsgFloorItem.MsgItemPacket.TideTrap, Attack.X, (ushort)Attack.Y, 14, DBSpell, 4000);
                    FloorItem.FloorPacket.ItemOwnerUID = user.Player.UID;
                    FloorItem.FloorPacket.OwnerX = user.Player.X;
                    FloorItem.FloorPacket.OwnerY = user.Player.Y;
                    FloorItem.FloorPacket.Name = "trap";
                    FloorItem.FloorPacket.MapID = user.Player.Map;
                    user.Player.FloorSpells[(ushort)Role.Flags.SpellID.TideTrap].AddItem(FloorItem);
                    using (var recycledPacket = new ServerSockets.RecycledPacket())
                    {
                        var streamm = recycledPacket.GetStream();
                        user.Player.View.SendView(streamm.ItemPacketCreate(FloorItem.FloorPacket), true);
                    }
                }

            }
        
        }
    }
}