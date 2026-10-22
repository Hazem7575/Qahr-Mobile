using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{
    public class Sering
    {

        public static void Proces(Client.GameClient user, Role.Player attacked, ServerSockets.Packet stream)
        {
            if (!user.Equipment.FreeEquip((Role.Flags.ConquerItem)4) && user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SearingTouch))
            {

                MsgGameItem BackSword;
                if (user.Equipment.TryGetEquip(Role.Flags.ConquerItem.RightWeapon, out BackSword))
                {
                    if (Database.ItemType.IsTaoistEpicWeapon(BackSword.ITEM_ID))
                    {


                        byte change = 10;
                        uint Experience = 0;
                        MsgGameItem hossus;
                        if (user.Equipment.TryGetEquip(Role.Flags.ConquerItem.LeftWeapon, out hossus))
                        {
                            if (Database.ItemType.IsHossu(hossus.ITEM_ID))
                            {
                                var dbItem = Pool.ItemsBase[hossus.ITEM_ID];
                                change += (byte)(dbItem.Level / 4);
                            }
                        }

                        if (AttackHandler.Calculate.Base.Rate(Math.Min(100, change + 10))) 
                        {

                            if (attacked.ContainFlag(MsgUpdate.Flags.lianhuaran04))
                            {
                                if (user.Player.TaoistPower < 10)
                                {
                                    if (Database.ItemType.IsTaoistEpicWeapon(user.Equipment.RightWeapon))
                                    {
                                        user.Player.TaoistPower += 1;
                                        user.Player.UpdateTaoPower(stream);
                                    }
                                }
                            }

                            InteractQuery Attack = new InteractQuery();

                            Attack.SpellID = (ushort)Role.Flags.SpellID.SearingTouch;
                            var DBSpells = Pool.Magic[(ushort)Role.Flags.SpellID.SearingTouch];
                            MsgServer.MsgSpell ClientSpell = user.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.SearingTouch];
                            var DBSpell = DBSpells[ClientSpell.Level];

                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(attacked.UID
       , 0, attacked.X, attacked.Y, ClientSpell.ID
       , ClientSpell.Level, ClientSpell.UseSpellSoul, 1);


                            foreach (Role.IMapObj atarget in attacked.View.Roles(Role.MapObjectType.Monster))
                            {

                                MsgMonster.MonsterRole aattacked = atarget as MsgMonster.MonsterRole;
                                if (CheckAttack.CanAttackMonster.Verified(user, aattacked, DBSpell))
                                {
                                    if (aattacked.UID != user.Player.UID)
                                    {
                                        if (Calculate.Base.GetDistance(attacked.X, attacked.Y, aattacked.X, aattacked.Y) < 6)
                                        {
                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            Calculate.Magic.OnMonster(user.Player, aattacked, DBSpell, out AnimationObj);
                                           
                                            if (AnimationObj.Damage > aattacked.HitPoints)
                                                AnimationObj.Damage = (uint)(aattacked.HitPoints - 1);
                                            Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, aattacked);
                                            MsgSpell.Targets.Enqueue(AnimationObj);
                                        }
                                    }
                                }
                            }
                            foreach (Role.IMapObj atarger in attacked.View.Roles(Role.MapObjectType.Player))
                            {
                                var aattacked = atarger as Role.Player;
                                if (aattacked.UID != user.Player.UID)
                                {
                                    if (CheckAttack.CanAttackPlayer.Verified(user, aattacked, DBSpell))
                                    {
                                        if (Calculate.Base.GetDistance(attacked.X, attacked.Y, aattacked.X, aattacked.Y) < 3)
                                        {
                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            Calculate.Magic.OnPlayer(user.Player, aattacked, DBSpell, out AnimationObj);
                                           
                                            if (AnimationObj.Damage > aattacked.HitPoints)
                                                AnimationObj.Damage = (uint)(aattacked.HitPoints - 1);
                                            ReceiveAttack.Player.Execute(AnimationObj, user, aattacked);
                                            MsgSpell.Targets.Enqueue(AnimationObj);
                                        }
                                    }
                                }
                            }

                            Updates.IncreaseExperience.Up(stream, user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, (uint)(change * 100), DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);


                            attacked.RemoveFlag(MsgUpdate.Flags.lianhuaran04);
                        }
                        else if (attacked.ContainFlag(MsgUpdate.Flags.lianhuaran03))
                        {
                            attacked.RemoveFlag(MsgUpdate.Flags.lianhuaran03);
                            attacked.AddFlag(MsgUpdate.Flags.lianhuaran04, 20, true);
                        }
                        else if (attacked.ContainFlag(MsgUpdate.Flags.lianhuaran02))
                        {
                            attacked.RemoveFlag(MsgUpdate.Flags.lianhuaran02);
                            attacked.AddSpellFlag(MsgUpdate.Flags.lianhuaran03, 20, true);
                        }
                        else if (attacked.ContainFlag(MsgUpdate.Flags.lianhuaran01))
                        {
                            attacked.RemoveFlag(MsgUpdate.Flags.lianhuaran01);
                            attacked.AddSpellFlag(MsgUpdate.Flags.lianhuaran02, 20, true);
                        }
                        else
                            attacked.AddSpellFlag(MsgUpdate.Flags.lianhuaran01, 20, true);
                    }
                }

            }
        }
    }
}


