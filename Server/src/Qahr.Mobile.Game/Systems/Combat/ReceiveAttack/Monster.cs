using Qahr.Mobile.Role.Instance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Qahr.Mobile.Game.MsgServer.AttackHandler.ReceiveAttack
{
    public class Monster
    {
        public static uint Execute(ServerSockets.Packet stream, MsgSpellAnimation.SpellObj obj, Client.GameClient client, MsgMonster.MonsterRole monster)
        {
            global::Qahr.Mobile.Phone.MobilePassiveAggro.Hit(monster, client);// QahrMobile: a passive monster fights back against who hit it
            #region Bow Special Damage

            bool HasSpecialBow = false;
            MsgGameItem EquipItem = null;

            // Normal Right Weapon
            if (client.Equipment.TryGetEquip(
                Role.Flags.ConquerItem.RightWeapon,
                out EquipItem))
            {
                if (EquipItem != null &&
                    Database.ItemType.IsBow(EquipItem.ITEM_ID))
                {
                    HasSpecialBow = true;
                }
            }

            // Alternate Right Weapon
            if (!HasSpecialBow &&
                client.Equipment.TryGetEquip(
                    Role.Flags.ConquerItem.AleternanteRightWeapon,
                    out EquipItem))
            {
                if (EquipItem != null &&
                    Database.ItemType.IsBow(EquipItem.ITEM_ID))
                {
                    HasSpecialBow = true;
                }
            }

            // Special Monsters
            if (HasSpecialBow)
            {
                uint monsterId = monster.Family.ID;

                if (monsterId == 4220 || // Nemesis Tyrant
                    monsterId == 4171 || // Snow Banshee
                    monsterId == 4212 || // Thrilling Spook
                    monsterId == 3976 || // Bloody Banshee
                    monsterId == 3977 || // Chilling Spook
                    monsterId == 3971 || // Dragon Wraith
                    monsterId == 3970 || // Queen Of Evil
                    monsterId == 3978)   // Nether Tyrant
                {
                    obj.Damage = 10000;


                }
            }

            #endregion


            if (client == null)
                return 0;
            client.Player.AttackHit = true;
            MsgYuanshen.Magics(client, monster, 0);
            client.MyArchives.ReceiveAttack(monster);

            #region SupremeLeadership
            if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SupremeLeadership))
            {
                Database.MagicType.Magic SuanniCommand = Pool.Magic[(ushort)Role.Flags.SpellID.SupremeLeadership][client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.SupremeLeadership].Level];
                if (Role.Core.Rate(SuanniCommand.Rate))
                {
                    InteractQuery InteractQuery = new InteractQuery();
                    InteractQuery.SpellID = (ushort)Role.Flags.SpellID.SupremeLeadership;
                    InteractQuery.SpellLevel = client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.SupremeLeadership].Level;
                    InteractQuery.X = client.Player.X;
                    InteractQuery.Y = client.Player.Y;
                    InteractQuery.UID = client.Player.UID;
                    InteractQuery.OpponentUID = monster.UID;
                    MsgAttackPacket.ProcescMagic(client, stream, InteractQuery, true);
                }
            }
            #endregion

            #region StarFlow
            if (client.MyArchives.isOpen(Archives.TypeID.StoneCracker))
            {
                #region StarFlow
                if (client.Player.StarFlow >= 0 && client.Player.StarFlow <= 2)
                {
                    client.Player.StarFlow++;
                    if (client.Player.StarFlow == 3)
                    {
                        client.Player.StarFlow = 4;
                        client.Player.RemoveFlag(MsgUpdate.Flags.StarFlow);
                        client.Player.AddSpellFlag(MsgUpdate.Flags.StarFlowEx, 10, true);
                        InteractQuery AttackPaket = new InteractQuery();
                        AttackPaket.OpponentUID = monster.UID;
                        AttackPaket.UID = client.Player.UID;
                        AttackPaket.X = client.Player.X;
                        AttackPaket.Y = client.Player.Y;
                        AttackPaket.SpellID = (ushort)Role.Flags.SpellID.StarFlow;
                        MsgAttackPacket.ProcescMagic(client, stream, AttackPaket, true);
                    }
                    else
                    {
                        if (client.Player.ContainFlag(MsgUpdate.Flags.StarFlow))
                        {
                            client.Player.AddSpellFlag(MsgUpdate.Flags.StarFlow, Role.StatusFlagsBigVector32.PermanentFlag, true);
                            client.Player.SendUpdate(stream, MsgUpdate.Flags.StarFlow, 0, (uint)client.Player.StarFlow, 0, MsgUpdate.DataType.StarFlow);
                        }
                    }
                }
                if (client.Player.StarFlow == 4)
                    client.Player.StarFlow = 0;
                #endregion
            }
            #endregion

            #region ThundercloudSight
            if (client.Player != null && monster != null)
            {
                client.Player.ThundercloudSight = monster.UID;
            }
            else
            {
                return 0;
            }
            #endregion]

            #region ArhivesTrojan

            if (client.Player.ContainFlag(MsgUpdate.Flags.CleanSweep))
                obj.Damage += (uint)(obj.Damage * client.Player.CleanSweepPower / 100);
            if (client.Player.ContainFlag(MsgUpdate.Flags.AxeShadow))
                obj.Damage += (uint)(obj.Damage * client.Player.AxeShadowPower / 100);
            #endregion


            #region DragonHeart
            if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonHeart))
            {
                Database.MagicType.Magic DragonHeart = Pool.Magic[(ushort)Role.Flags.SpellID.DragonHeart][client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.DragonHeart].Level];
                if (Role.Core.Rate(DragonHeart.Rate))
                {


                    InteractQuery InteractQuery = new InteractQuery();
                    InteractQuery.SpellID = (ushort)Role.Flags.SpellID.DragonHeart;
                    InteractQuery.SpellLevel = client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.DragonHeart].Level;
                    InteractQuery.X = client.Player.X;
                    InteractQuery.Y = client.Player.Y;
                    InteractQuery.UID = client.Player.UID;
                    InteractQuery.OpponentUID = monster.UID;
                    MsgAttackPacket.ProcescMagic(client, stream, InteractQuery, true);
                }
            }
            if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.KunpengHeart))
            {
                Database.MagicType.Magic KunpengHeart = Pool.Magic[(ushort)Role.Flags.SpellID.KunpengHeart][client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.KunpengHeart].Level];
                if (Role.Core.Rate(KunpengHeart.Rate))
                {
                    InteractQuery InteractQuery = new InteractQuery();
                    InteractQuery.SpellID = (ushort)Role.Flags.SpellID.KunpengHeart;
                    InteractQuery.SpellLevel = client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.KunpengHeart].Level;
                    InteractQuery.X = client.Player.X;
                    InteractQuery.Y = client.Player.Y;
                    InteractQuery.UID = client.Player.UID;
                    InteractQuery.OpponentUID = monster.UID;
                    MsgAttackPacket.ProcescMagic(client, stream, InteractQuery, true);
                }
            }
            if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.KunpengRocket))
            {
                Database.MagicType.Magic KunpengHeart = Pool.Magic[(ushort)Role.Flags.SpellID.KunpengRocket][client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.KunpengRocket].Level];
                int IncRate = 0;
                if (client.Player.ContainFlag(MsgUpdate.Flags.Spreadyourwings))
                {
                    Database.MagicType.Magic Hitthewaterthreethousand = Pool.Magic[(ushort)Role.Flags.SpellID.Hitthewaterthreethousand][client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.Hitthewaterthreethousand].Level];
                    IncRate = Hitthewaterthreethousand.DamageOnHuman;
                }
                if (Role.Core.Rate(KunpengHeart.Rate + IncRate))
                {

                    InteractQuery InteractQuery = new InteractQuery();
                    InteractQuery.SpellID = (ushort)Role.Flags.SpellID.KunpengRocket;
                    InteractQuery.SpellLevel = client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.KunpengRocket].Level;
                    InteractQuery.X = client.Player.X;
                    InteractQuery.Y = client.Player.Y;
                    InteractQuery.UID = client.Player.UID;
                    InteractQuery.OpponentUID = monster.UID;
                    MsgAttackPacket.ProcescMagic(client, stream, InteractQuery, true);
                }
            }
            if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SuanniHeart))
            {
                Database.MagicType.Magic SuanniHeart = Pool.Magic[(ushort)Role.Flags.SpellID.SuanniHeart][client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.SuanniHeart].Level];
                if (Role.Core.Rate(SuanniHeart.Damage2))
                {
                    InteractQuery InteractQuery = new InteractQuery();
                    InteractQuery.SpellID = (ushort)Role.Flags.SpellID.SuanniHeart;
                    InteractQuery.SpellLevel = client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.SuanniHeart].Level;
                    InteractQuery.X = client.Player.X;
                    InteractQuery.Y = client.Player.Y;
                    InteractQuery.UID = client.Player.UID;
                    InteractQuery.OpponentUID = monster.UID;
                    MsgAttackPacket.ProcescMagic(client, stream, InteractQuery, true);
                }
            }
            if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SuanniCommand))
            {
                Database.MagicType.Magic SuanniCommand = Pool.Magic[(ushort)Role.Flags.SpellID.SuanniCommand][client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.SuanniCommand].Level];
                if (Role.Core.Rate(SuanniCommand.Rate))
                {
                    InteractQuery InteractQuery = new InteractQuery();
                    InteractQuery.SpellID = (ushort)Role.Flags.SpellID.SuanniCommand;
                    InteractQuery.SpellLevel = client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.SuanniCommand].Level;
                    InteractQuery.X = client.Player.X;
                    InteractQuery.Y = client.Player.Y;
                    InteractQuery.UID = client.Player.UID;
                    InteractQuery.OpponentUID = monster.UID;
                    MsgAttackPacket.ProcescMagic(client, stream, InteractQuery, true);
                }
            }
            #endregion

            #region XPBooster
            if (client.Player.OnXPSkill() != MsgUpdate.Flags.Normal)
            {
                byte level = 0;
                if (client.Rune.IsEquipped("XPBooster", ref level) || client.Rune.IsEquipped("XP Devourer", ref level))
                {
                    if (client.Player.XPBooster > 0)
                    {
                        client.Player.IncreaseXPDuration(1);
                        client.Player.XPBooster--;
                        client.Player.SendUpdate(stream, (MsgUpdate.Flags)0, (uint)(client.Player.XPBooster == 0 ? 1 : 0), 0, 1, MsgUpdate.DataType.XPDuration);

                    }
                }
            }
            #endregion

            #region SageMode
            if (Database.AtributesStatus.IsNinja(client.Player.Class) && !Database.ItemType.IsScepter(client.Equipment.RightWeapon)
                    && !Database.ItemType.IsScepter(client.Equipment.LeftWeapon) && !Database.ItemType.IsKnife(client.Equipment.RightWeapon)
                    && !Database.ItemType.IsKnife(client.Equipment.LeftWeapon) && !Database.ItemType.IsBow(client.Equipment.RightWeapon))
            {
                foreach (var Ninja in NinjaFile.gouyu_immortals.Values)
                {
                    if ((client.MyNinja.Levels > 17 ? 17 : client.MyNinja.Levels) == Ninja.Level)
                    {
                        if (Role.Core.Rate(Ninja.Rate / 100))
                        {
                            if (!client.Player.ContainFlag(MsgUpdate.Flags.SageMode))
                            {
                                client.MyNinja.SageMode(Ninja.Seconds);

                            }
                        }
                    }
                }
            }
            #endregion

            #region DragonPierce
            if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonPierce))
            {
                Database.MagicType.Magic DragonPierces = Pool.Magic[(ushort)Role.Flags.SpellID.DragonPierce][client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.DragonPierce].Level];

                List<byte> CanUse = new List<byte>();
                if (!client.Player.ContainFlag(MsgUpdate.Flags.DragonPierceCrit))
                    CanUse.Add(1);
                if (!client.Player.ContainFlag(MsgUpdate.Flags.DragonPierceTortoise))
                    CanUse.Add(2);
                if (!client.Player.ContainFlag(MsgUpdate.Flags.DragonPierceBreak))
                    CanUse.Add(3);
                if (!client.Player.ContainFlag(MsgUpdate.Flags.DragonPierceStamina))
                    CanUse.Add(4);
                if (CanUse.Count != 0)
                {
                    switch (CanUse[Pool.GetRandom.Next(0, CanUse.Count)])
                    {
                        case 1:
                            client.Player.AddFlag(MsgUpdate.Flags.DragonPierceCrit, (int)DragonPierces.Duration, true);
                            client.Player.SendUpdate(stream, MsgUpdate.Flags.DragonPierceCrit, (uint)DragonPierces.Duration, 0, 0, MsgUpdate.DataType.ArchiveSkill);
                            break;
                        case 2:
                            client.Player.AddFlag(MsgUpdate.Flags.DragonPierceTortoise, (int)DragonPierces.Duration, true);
                            client.Player.SendUpdate(stream, MsgUpdate.Flags.DragonPierceTortoise, (uint)DragonPierces.Duration, (uint)DragonPierces.DamageOnHuman, 0, MsgUpdate.DataType.ArchiveSkill);
                            break;
                        case 3:
                            client.Player.AddFlag(MsgUpdate.Flags.DragonPierceBreak, (int)DragonPierces.Duration, true);
                            client.Player.SendUpdate(stream, MsgUpdate.Flags.DragonPierceBreak, (uint)DragonPierces.Duration, (uint)DragonPierces.Damage2, 0, MsgUpdate.DataType.ArchiveSkill);
                            break;
                        case 4:
                            client.Player.AddFlag(MsgUpdate.Flags.DragonPierceStamina, (int)DragonPierces.Duration, true);
                            client.Player.SendUpdate(stream, MsgUpdate.Flags.DragonPierceStamina, (uint)DragonPierces.Duration, 0, (uint)DragonPierces.Damage3, MsgUpdate.DataType.ArchiveSkill);
                            break;
                    }

                }
            }

            #endregion

            #region Bloodlust
            if (client.MyArchives.isOpen(Archives.TypeID.Bloodlust))
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    #region Immersion
                    if (!client.Player.ContainFlag(MsgUpdate.Flags.Immersion))
                    {
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Immersion))
                        {
                            Database.MagicType.Magic Immersion = Pool.Magic[(ushort)Role.Flags.SpellID.Immersion][client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.Immersion].Level];
                            if (Role.Core.Rate(Immersion.Damage3))
                            {
                                InteractQuery InteractQuery = new InteractQuery();
                                InteractQuery.SpellID = Immersion.ID;
                                InteractQuery.SpellLevel = Immersion.Level;
                                InteractQuery.X = monster.X;
                                InteractQuery.Y = monster.Y;
                                InteractQuery.OpponentUID = client.Player.UID;
                                MsgAttackPacket.ProcescMagic(client, stream, InteractQuery, true);
                            }
                        }
                    }
                    #endregion
                }
            }
            #endregion

            #region SweepMyth
            if (monster.Boss == 0)
            {
                Database.MythSoulAttributes.Attribute Sweep;
                if (Database.MythSoulAttributes.Attributes[Database.MythSoulAttributes.Type.SWeep].TryGetValue(client.Status.SweepLevel, out Sweep))
                {
                    obj.Damage = (uint)(obj.Damage * Sweep.Damage / 100);
                }
            }
            #endregion

            #region DrainingTouch
            if (client.PerfectionStatus.DrainingTouch > 0)
            {
                if (AttackHandler.Calculate.Base.Rate(client.PerfectionStatus.DrainingTouch))
                {
                    client.Player.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
                    {
                        Effect = MsgRefineEffect.RefineEffects.DrainingTouch,
                        Id = client.Player.UID,
                        dwParam = client.Player.UID
                    }), true);

                    bool update = false;
                    if (client.Player.HitPoints < client.Player.Owner.Status.MaxHitpoints)
                    {
                        update = true;
                        client.Player.HitPoints = (int)client.Player.Owner.Status.MaxHitpoints;
                    }
                    if (client.Player.Mana < client.Player.Owner.Status.MaxMana)
                    {
                        update = true;
                        client.Player.Mana = (ushort)client.Player.Owner.Status.MaxMana;
                    }
                    if (update)
                    {
                        client.Player.SendUpdateHP();
                        client.Player.SendUpdate(stream, client.Player.Mana, MsgUpdate.DataType.Mana, false);
                    }
                }
            }
            #endregion

            #region SupremeLeadership
            if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SupremeLeadership))
            {
                Database.MagicType.Magic SupremeLeadership = Pool.Magic[(ushort)Role.Flags.SpellID.SupremeLeadership][client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.SupremeLeadership].Level];
                if (Role.Core.Rate(SupremeLeadership.Rate % 10))
                {

                    InteractQuery InteractQuery = new InteractQuery();
                    InteractQuery.SpellID = (ushort)Role.Flags.SpellID.SupremeLeadership;
                    InteractQuery.SpellLevel = client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.SupremeLeadership].Level;
                    InteractQuery.X = client.Player.X;
                    InteractQuery.Y = client.Player.Y;
                    InteractQuery.UID = client.Player.UID;
                    InteractQuery.OpponentUID = monster.UID;
                    MsgAttackPacket.ProcescMagic(client, stream, InteractQuery, true);
                }
            }
            #endregion

            if (client.Player.ContainFlag(MsgUpdate.Flags.BonePulse))
                obj.Effect = MsgAttackPacket.AttackEffect.Destroy;

            #region Damage

            if (client.Fake && client.Player.Event)
            {
                obj.Damage = (uint)Pool.GetRandom.Next(200000, 500000);
            }

            if (monster.HitPoints <= obj.Damage)
            {
                // score the killing blow too, before Dead ranks the hunters
                if (monster.Boss == 1 && monster.Alive)
                    monster.UpdateScore(stream, client, obj.Damage);

                client.Map.SetMonsterOnTile(monster.X, monster.Y, false);
                global::Qahr.Mobile.QuellDemons.QuellDemonsModule.OnKill(client, monster);// [QuellDemons 2026-10-14] ranking rewards / demon counter (no-op for other monsters)
                global::Qahr.Mobile.Phone.MobileDaily.OnMonsterKill(client, monster);// QahrMobile: mobile daily Labyrinth / World Boss
                monster.Dead(stream, client, client.Player.UID, client.Map);

                if (client.Team != null)
                    client.Team.ShareExperience(stream, client, monster);
            }
            else
            {
                monster.HitPoints -= obj.Damage;

                if (monster.Boss == 1)
                {
                    monster.UpdateScore(stream, client, obj.Damage);
                    Game.MsgServer.MsgUpdate Upd = new Game.MsgServer.MsgUpdate(stream, monster.UID, 1);
                    stream = Upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.MaxHitpoints, monster.Family.MaxHealth);
                    stream = Upd.GetArray(stream);
                    client.Player.View.SendView(stream, true);
                    stream = Upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.Hitpoints, monster.HitPoints);
                    stream = Upd.GetArray(stream);
                    client.Player.View.SendView(stream, true);
                }
            }

            if ((monster.Family.Settings & MsgMonster.MonsterSettings.Guard) != MsgMonster.MonsterSettings.Guard)
            {
                if (obj.Damage > monster.Family.MaxHealth)
                    return (uint)AdjustExp(monster.Family.MaxHealth, client.Player.Level, monster.Level);
                else
                    return (uint)AdjustExp((int)obj.Damage, client.Player.Level, monster.Level);
            }
            else

                return 0;
            #endregion



        }

        public static int AdjustExp(int nDamage, int nAtkLev, int nDefLev)
        {
            #region ExpRaduce
            int nExp = nDamage;

            int nNameType = Calculate.Base.GetNameType(nAtkLev, nDefLev);
            int nDeltaLev = nAtkLev - nDefLev;
            if (nNameType == Calculate.Base.StatusConstants.NAME_GREEN)
            {
                if (nDeltaLev >= 3 && nDeltaLev <= 5)
                    nExp = nExp * 70 / 100;
                else if (nDeltaLev > 5 && nDeltaLev <= 10)
                    nExp = nExp * 20 / 100;
                else if (nDeltaLev > 10 && nDeltaLev <= 20)
                    nExp = nExp * 10 / 100;
                else if (nDeltaLev > 20)
                    nExp = nExp * 5 / 100;
            }
            else if (nNameType == Calculate.Base.StatusConstants.NAME_RED)
            {
                nExp = (int)(nExp * 1.3);
            }
            else if (nNameType == Calculate.Base.StatusConstants.NAME_BLACK)
            {
                if (nDeltaLev >= -10 && nDeltaLev < -5)
                    nExp = (int)(nExp * 1.5);
                else if (nDeltaLev >= -20 && nDeltaLev < -10)
                    nExp = (int)(nExp * 1.8);
                else if (nDeltaLev < -20)
                    nExp = (int)(nExp * 2.3);
            }

            return Math.Max(10, nExp);
            #endregion
        }
    }
}
