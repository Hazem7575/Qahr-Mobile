using Qahr.Mobile.Game.MsgTournaments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{
    public class DetachStatus
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                switch (ClientSpell.ID)
                {
                    case (ushort)Role.Flags.SpellID.ArcherBane:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , 0, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);


                            Role.IMapObj target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {
                                Role.Player attacked = target as Role.Player;
                                if (attacked.ContainFlag(MsgUpdate.Flags.Fly) && !attacked.ContainFlag(MsgUpdate.Flags.HeavensWrath) && !attacked.ContainFlag(MsgUpdate.Flags.Infinity))
                                {
                                    if (Calculate.Base.Rate(DBSpell.Rate + (user.Player.BattlePower - attacked.BattlePower)))
                                    {
                                        attacked.RemoveFlag(MsgUpdate.Flags.Fly);
                                        uint damage = (uint)((double)attacked.HitPoints * 0.1d);
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                        AnimationObj.Damage = damage;
                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked.UID, damage, MsgAttackPacket.AttackEffect.None));
                                    }
                                    else
                                    {
                                        var clientobj = new MsgSpellAnimation.SpellObj(attacked.UID, MsgSpell.SpellID, MsgAttackPacket.AttackEffect.None);
                                        clientobj.Hit = 0;
                                        MsgSpell.Targets.Enqueue(clientobj);
                                    }
                                }
                            }
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 250, DBSpells);

                            break;
                        }
                    case (ushort)Role.Flags.SpellID.Revive:
                        {
                            if (user.IsWatching())
                            {
#if Arabic
                               user.SendSysMesage("This spell not work on this map..");
#else
                                user.SendSysMesage("This spell not work on this map.");
#endif

                                break;
                            }
                            if (user.Player.Map == 40021 || user.Player.Map == 5051 || user.Player.Map == 5053 || user.Player.Map == 5054 || user.Player.Map == 5055 || user.Player.Map == 5052 || user.Player.Map == 5061 || user.Player.Map == 5062 || user.Player.Map == 5063)
                            {
                                user.SendSysMesage("You can't use this skill right now!");
                                break;

                            }
                            // if (user.Player.MyGuild != null)


                            //if (user.Player.MyGuild.Members)
                            //{ }
                            //if (user.Player.Name.Contains("[GM]"))
                            //    break;

                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , 0, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            bool revive = false;
                            user.Player.RemoveFlag(MsgUpdate.Flags.XPList);

                            Role.IMapObj target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {
                                Role.Player attacked = target as Role.Player;
                                if (!attacked.ContainFlag(MsgUpdate.Flags.SoulShackle))
                                {
                                    MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked.UID, 0, MsgAttackPacket.AttackEffect.None));
                                    attacked.Revive(stream);
                                    revive = true;
                                }
                            }

                            if (revive)
                            {
                                MsgSpell.SetStream(stream);
                                MsgSpell.Send(user);

                                Updates.UpdateSpell.CheckUpdate(stream, user, Attack, DBSpell.Duration, DBSpells);


                                if (!user.Equipment.FreeEquip((Role.Flags.ConquerItem)4) && user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BlessingTouch))
                                {

                                    /*  MsgGameItem BackSword;
                                      if (user.Equipment.TryGetEquip(Role.Flags.ConquerItem.RightWeapon, out BackSword))
                                      {
                                          if (Database.ItemType.IsTaoistEpicWeapon(BackSword.ITEM_ID))
                                          {
                                              if (user.Player.TaoistPower < 10)
                                              {
                                                  user.Player.TaoistPower += 1;
                                                  user.Player.UpdateTaoPower(stream);
                                              }*/

                                    byte change = 100;

                                    MsgGameItem hossus;
                                    if (user.Equipment.TryGetEquip(Role.Flags.ConquerItem.LeftWeapon, out hossus))
                                    {
                                        if (Database.ItemType.IsHossu(hossus.ITEM_ID))
                                        {
                                            var dbItem = Pool.ItemsBase[hossus.ITEM_ID];
                                            change += (byte)(dbItem.Level / 3);
                                        }
                                    }

                                    if (AttackHandler.Calculate.Base.Rate(change))
                                    {
                                        if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                                        {
                                            Role.Player attacked = target as Role.Player;

                                            attacked.AddSpellFlag(MsgUpdate.Flags.Stigma, (int)80, true);
                                            attacked.AddSpellFlag(MsgUpdate.Flags.MagicShield, (int)120, true);
                                            attacked.AddSpellFlag(MsgUpdate.Flags.StarOfAccuracy, (int)60, true);


                                            Attack.SpellID = (ushort)Role.Flags.SpellID.BlessingTouch;
                                            var Spell = Pool.Magic[Attack.SpellID];

                                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, (uint)(change * 100), Spell);

                                        }
                                    }
                                }
                            }
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.Pray:
                        {
                            if (!Database.AtributesStatus.IsWater(user.Player.Class))
                            {
                                user.SendSysMesage("Sorry, Only Water Character Can Use This Spell !");
                                break;
                            }
                            if (user.IsWatching())
                            {
                                user.SendSysMesage("This spell not work on this map.");
                                break;
                            }                            
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , 0, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            Role.IMapObj target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {
                                Role.Player attacked = target as Role.Player;

                                if (attacked.ContainFlag(MsgUpdate.Flags.SoulShackle))
                                {
                                    // user.SendSysMesage("This Acc Shackle ");
                                    return;
                                    #region arena by Mohy ElDin ...
                                }
                                if (user.Player.Map == 5061 || user.Player.Map == 1730 || user.Player.Map == 5062 || user.Player.Map == 5063 || user.Player.Map == 5064 || user.Player.Map == 5065 || user.Player.Map == 5066 || user.Player.Map == 5051 || user.Player.Map == 5052 || user.Player.Map == 5053 || user.Player.Map == 5054 || user.Player.Map == 5055 || user.Player.Map == 5056 || user.Player.Map == 5057 || user.Player.Map == 5058)
                                {
                                    //user.CreateBoxDialog("Xman Conquer Here..");
                                    break;


                                    #endregion
                                }

                                if (attacked.MyGuild != null && user.Player.MyGuild != null)
                                {
                                    if (attacked.MyGuild.Info.GuildID == user.Player.MyGuild.Info.GuildID || user.Player.MyGuild.Ally.ContainsKey(attacked.MyGuild.Info.GuildID))
                                    {
                                        MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked.UID, 0, MsgAttackPacket.AttackEffect.None));
                                        attacked.Revive(stream);
                                    }
                                    else return;
                                }
                                else return;
                                // if (attacked.MyGuild.Info.GuildID == user.Player.MyGuild.Info.GuildID || user.Player.MyGuild.Ally.ContainsKey(attacked.MyGuild.Info.GuildID))
                                // {
                                //   MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked.UID, 0, MsgAttackPacket.AttackEffect.None));
                                //attacked.Revive(stream);
                                //}
                            }

                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, DBSpell.Duration, DBSpells);



                            if (!user.Equipment.FreeEquip((Role.Flags.ConquerItem)4) && user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BlessingTouch))
                            {

                                MsgGameItem BackSword;
                                if (user.Equipment.TryGetEquip(Role.Flags.ConquerItem.RightWeapon, out BackSword))
                                {
                                    /*if (Database.ItemType.IsTaoistEpicWeapon(BackSword.ITEM_ID))
                                    {
                                        if (user.Player.TaoistPower < 10)
                                        {
                                            user.Player.TaoistPower += 1;
                                            user.Player.UpdateTaoPower(stream);
                                        }*/

                                    byte change = 100;

                                    MsgGameItem hossus;
                                    if (user.Equipment.TryGetEquip(Role.Flags.ConquerItem.LeftWeapon, out hossus))
                                    {
                                        if (Database.ItemType.IsHossu(hossus.ITEM_ID))
                                        {
                                            var dbItem = Pool.ItemsBase[hossus.ITEM_ID];
                                            change += (byte)(dbItem.Level / 3);
                                        }
                                    }

                                    if (AttackHandler.Calculate.Base.Rate(change))
                                    {
                                        if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                                        {
                                            Role.Player attacked = target as Role.Player;

                                            attacked.AddSpellFlag(MsgUpdate.Flags.Stigma, (int)80, true);
                                            attacked.AddSpellFlag(MsgUpdate.Flags.MagicShield, (int)120, true);
                                            attacked.AddSpellFlag(MsgUpdate.Flags.StarOfAccuracy, (int)60, true);


                                            Attack.SpellID = (ushort)Role.Flags.SpellID.BlessingTouch;
                                            var Spell = Pool.Magic[Attack.SpellID];

                                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, (uint)(change * 100), Spell);

                                        }
                                    }
                                }
                            }
                        }

                        break;
                }
            }
        }
    }
}

