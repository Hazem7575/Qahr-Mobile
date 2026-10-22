using Qahr.Mobile.Database;
using Qahr.Mobile.Game.MsgServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgFloorItem;
using Qahr.Mobile.Role.Instance;
namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{
    public class FiveStarLianzhuWater
    {

        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                switch (ClientSpell.ID)
                {
                    case (ushort)Role.Flags.SpellID.StarChainWater:
                        {
                            if (user.IsWatching())
                            {
                                user.SendSysMesage("This spell not work on this map..");
                                break;
                            }
                            if (user.Player.Map == 700)
                                break;
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , 0, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            Role.IMapObj target;
                            ushort CountRevive = 0;
                            foreach (var targer in user.Player.View.Roles(Role.MapObjectType.Player))
                            {

                                Role.Player attacked = targer as Role.Player;

                                if (attacked.Alive || attacked.ContainFlag(MsgUpdate.Flags.SoulShackle))
                                {
                                    break;
                                }



                                if (attacked.MyGuild != null && user.Player.MyGuild != null)
                                {
                                    if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= 5 && CountRevive <= 5)
                                    {
                                        if (attacked.MyGuild != null && user.Player.MyGuild != null)
                                        {
                                            if (attacked.MyGuild.Info.GuildID == user.Player.MyGuild.Info.GuildID || user.Player.MyGuild.Ally.ContainsKey(attacked.MyGuild.Info.GuildID))
                                            {
                                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked.UID, 0, MsgAttackPacket.AttackEffect.None));
                                                attacked.Revive(stream);
                                            }
                                            else break;
                                        }
                                    }
                                    else return;
                                }
                                else return;

                            }

                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, DBSpell.Duration, DBSpells);




                            if (!user.Equipment.FreeEquip((Role.Flags.ConquerItem)4) && user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BlessingTouch))
                            {

                                MsgGameItem BackSword;
                                if (user.Equipment.TryGetEquip(Role.Flags.ConquerItem.RightWeapon, out BackSword))
                                {

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
                }
            }
        }
    }
}