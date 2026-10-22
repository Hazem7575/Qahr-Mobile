using System.Collections.Generic;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{
    public class SwordAncestorAttack
    {
        public static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (Game.MsgServer.AttackHandler.CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                switch (ClientSpell.ID)
                {
                    case (ushort)Role.Flags.SpellID.WeaponCombo:
                        {
                            if (System.Time32.Now >= user.Player.WeaponCombo.AddMilliseconds((int)DBSpell.ColdTime))
                            {
                                user.Send(stream.InteractionCreate(Attack));
                                var MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                                MsgSpell.SetStream(stream);
                                MsgSpell.Send(user);
                                user.Player.WeaponCombo = System.Time32.Now;
                                #region SongofPhoenix
                                if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SongofPhoenix))
                                {
                                    Database.MagicType.Magic DBSpells1 = Pool.Magic[(ushort)Role.Flags.SpellID.SongofPhoenix][user.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.SongofPhoenix].Level];
                                    user.Player.AddFlag(MsgUpdate.Flags.SongofPhoenix, (int)DBSpells1.Duration, true);
                                    user.Player.SongofPhoenixPower = (byte)DBSpells1.Damage3;
                                }
                                #endregion
                                #region AxeShadow
                                if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AxeShadow))
                                {
                                    Database.MagicType.Magic DBSpells1 = Pool.Magic[(ushort)Role.Flags.SpellID.AxeShadow]
                                    [user.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.AxeShadow].Level];
                                    user.Player.AddSpellFlag(MsgUpdate.Flags.AxeShadow, (int)DBSpells1.Duration, true);
                                    user.Player.SendUpdate(stream, MsgUpdate.Flags.AxeShadow, DBSpells1.Duration, 0, 0, MsgUpdate.DataType.ArchiveSkill);
                                    user.Player.AxeShadowPower = (byte)DBSpells1.Damage3;
                                }
                                #endregion
                                #region DeathSigh
                                if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DeathSigh))
                                {
                                    Database.MagicType.Magic DBSpells1 = Pool.Magic[(ushort)Role.Flags.SpellID.DeathSigh]
                                    [user.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.DeathSigh].Level];
                                    user.Player.AddFlag(MsgUpdate.Flags.DeathSigh, (int)DBSpells1.Duration, true);
                                    user.Player.DeathSighPassive = (uint)DBSpells1.Damage2;
                                    user.Player.DeathSighActive = (uint)DBSpells1.Damage3;
                                    user.Player.SendUpdate(stream, MsgUpdate.Flags.DeathSigh, DBSpells1.Duration, 0, 0, MsgUpdate.DataType.ArchiveSkill);
                                }
                                #endregion
                                #region HawksAmbition
                                if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.HawksAmbition))
                                {
                                    Database.MagicType.Magic DBSpells1 = Pool.Magic[(ushort)Role.Flags.SpellID.HawksAmbition]
                                    [user.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.HawksAmbition].Level];
                                    user.Player.AddFlag(MsgUpdate.Flags.HawksAmbition, (int)DBSpells1.Duration, true);

                                    user.Player.HawksAmbitionPower = (uint)DBSpells1.Damage;
                                    user.Player.SendUpdate(stream, MsgUpdate.Flags.HawksAmbition, DBSpells1.Duration, 0, 0, MsgUpdate.DataType.ArchiveSkill);
                                }
                                #endregion
                                #region SacredBlessing
                                if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SacredBlessing))
                                {
                                    Database.MagicType.Magic DBSpells1 = Pool.Magic[(ushort)Role.Flags.SpellID.SacredBlessing]
                                    [user.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.SacredBlessing].Level];
                                    foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                                    {
                                        var attacked = targer as Role.Player;
                                        if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) <= DBSpells1.Range || user.nSaveMele)
                                        {
                                            if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                            {
                                                if (attacked.Alive)
                                                {

                                                    attacked.BleedDamage = 3000;
                                                    attacked.AddSpellFlag(MsgUpdate.Flags.Bleed, 5, true, 1);
                                                }
                                            }
                                        }
                                    }
                                    user.Player.AddSpellFlag(MsgUpdate.Flags.SacredBlessing, (int)DBSpells1.Duration, false);
                                }
                                #endregion
                                #region DeadlyStrike
                                if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DeadlyStrike))
                                {
                                    Database.MagicType.Magic DBSpells1 = Pool.Magic[(ushort)Role.Flags.SpellID.DeadlyStrike]
                                    [user.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.DeadlyStrike].Level];
                                    foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                                    {
                                        var attacked = targer as Role.Player;
                                        if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) <= DBSpells1.Range || user.nSaveMele)
                                        {
                                            if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                            {
                                                if (attacked.Alive)
                                                {
                                                    attacked.AddFlag(MsgUpdate.Flags.DeadlyStrike, (int)DBSpells1.Duration, true);
                                                    attacked.SendUpdate(stream, MsgUpdate.Flags.DeadlyStrike, (uint)DBSpells1.Duration, 0, 0, MsgUpdate.DataType.ArchiveSkill);
                                                    attacked.DeadlyStrikePower = (uint)DBSpells1.Damage3;
                                                }
                                            }
                                        }
                                    }
                                }
                                #endregion
                                #region HookMoon
                                if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.HookMoon))
                                {
                                    Database.MagicType.Magic DBSpells1 = Pool.Magic[(ushort)Role.Flags.SpellID.HookMoon]
                                    [user.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.HookMoon].Level];
                                    foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                                    {
                                        var attacked = targer as Role.Player;
                                        if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) <= DBSpells1.Range || user.nSaveMele)
                                        {
                                            if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                            {
                                                if (attacked.Alive)
                                                {
                                                    if (attacked.AddFlag(MsgUpdate.Flags.HookMoon, (int)DBSpells1.Duration, true))
                                                    {
                                                        attacked.HookMoonAttackedPower = (uint)(100 - (DBSpells1.Level >= 7 ? 50 : 30));
                                                        attacked.SendUpdate(stream, MsgUpdate.Flags.HookMoon, DBSpells1.Duration, 0, 0, MsgUpdate.DataType.ArchiveSkill);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                #endregion
                                #region ThunderStrike
                                if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ThunderStrike))
                                {
                                    Database.MagicType.Magic DBSpells1 =
                                        Pool.Magic[(ushort)Role.Flags.SpellID.ThunderStrike]
                                        [user.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.ThunderStrike].Level];

                                    foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                                    {
                                        var attacked = targer as Role.Player;

                                        if (attacked == null)
                                            continue;

                                        if (Calculate.Base.GetDistance(
                                                user.Player.X,
                                                user.Player.Y,
                                                attacked.X,
                                                attacked.Y) <= DBSpells1.Range || user.nSaveMele)
                                        {
                                            if (CheckAttack.CanAttackPlayer.Verified(
                                                    user,
                                                    attacked,
                                                    DBSpell,
                                                    true,
                                                    Attack))
                                            {
                                                // Dizzy يبدأ فقط من Level 8
                                                if (attacked.Alive && DBSpells1.Level >= 8)
                                                {
                                                    int BattelPower = attacked.BattlePower - user.Player.BattlePower;

                                                    switch (BattelPower)
                                                    {
                                                        case 1:
                                                            if (Calculate.Base.Rate(20))
                                                                attacked.AddFlag(MsgUpdate.Flags.Dizzy, 1, true);
                                                            break;

                                                        case 2:
                                                            if (Calculate.Base.Rate(40))
                                                                attacked.AddFlag(MsgUpdate.Flags.Dizzy, 1, true);
                                                            break;

                                                        case 3:
                                                            if (Calculate.Base.Rate(60))
                                                                attacked.AddFlag(MsgUpdate.Flags.Dizzy, 1, true);
                                                            break;

                                                        case 4:
                                                            if (Calculate.Base.Rate(80))
                                                                attacked.AddFlag(MsgUpdate.Flags.Dizzy, 1, true);
                                                            break;

                                                        case 5:
                                                            if (Calculate.Base.Rate(100))
                                                                attacked.AddFlag(MsgUpdate.Flags.Dizzy, 1, true);
                                                            break;
                                                    }

                                                    // لو اللاعب أقوى أو مساوي للخصم
                                                    if (user.Player.BattlePower >= attacked.BattlePower)
                                                    {
                                                        attacked.AddFlag(
                                                            MsgUpdate.Flags.Dizzy,
                                                            1,
                                                            true);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                #endregion
                                #region SongofPhoenix
                                if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.CleanSweep))
                                {
                                    Database.MagicType.Magic DBSpells1 = Pool.Magic[(ushort)Role.Flags.SpellID.CleanSweep]
                                        [user.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.CleanSweep].Level];
                                    user.Player.CleanSweepPower = (uint)DBSpells1.Damage3;
                                    user.Player.SendUpdate(stream, Game.MsgServer.MsgUpdate.Flags.CleanSweep, DBSpells1.Duration
           , user.Player.CleanSweepPower, user.Player.CleanSweepPower, Game.MsgServer.MsgUpdate.DataType.ArchiveSkill, true);

                                    user.Player.AddSpellFlag(MsgUpdate.Flags.CleanSweep, (int)DBSpells1.Duration, true);
                                }
                                #endregion
                            }
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.GrowFromHurt:
                        {
                            user.Send(stream.InteractionCreate(Attack));
                            var MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            user.Player.AddSpellFlag((MsgUpdate.Flags)DBSpell.Status, (int)DBSpell.Duration, true);
                            user.Player.SendUpdate(stream, (MsgUpdate.Flags)DBSpell.Status, (uint)DBSpell.Duration, (uint)DBSpell.Damage2 / 10000000, user.Player.GrowFromHurtHitpoints, MsgUpdate.DataType.ArchiveSkill, true);
                            user.Player.GrowFromHurtStamp = System.Time32.Now;
                            break;
                        }
                }
            }
        }
    }
}