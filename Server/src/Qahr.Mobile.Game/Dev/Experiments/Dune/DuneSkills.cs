using Qahr.Mobile.Database;
using Qahr.Mobile.Game.MsgFloorItem;
using Qahr.Mobile.Role;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{
    public class DuneSkills
    {
        public static void UpdateEnergy(Client.GameClient client)
        {
            if (client == null || client.Player == null || client.Player.Alive == false
                || client.Player.DuneEnergy >= 240 || client.Player.DuneEnergy2 >= 240
                || client.Player.DuneEnergy3 >= 240 || !AtributesStatus.IsDune(client.Player.Class))
                return;
            if (DateTime.Now > client.Player.DuneStampPower.AddSeconds(1))
            {
                client.Player.DuneEnergy += 10;
                client.Player.DuneEnergy2 += 10;
                client.Player.DuneEnergy3 += 10;

                if (client.Player.DuneEnergy > 240)
                    client.Player.DuneEnergy = 240;

                if (client.Player.DuneEnergy2 > 240)
                    client.Player.DuneEnergy2 = 240;

                if (client.Player.DuneEnergy3 > 240)
                    client.Player.DuneEnergy3 = 240;


                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var pkt = rec.GetStream();

                    if (pkt == null)
                        return;

                    var info = new MsgUpdate(pkt, client.Player.UID);

                    info.AppendDune(pkt, client.Player.DuneEnergy, client.Player.DuneEnergy2, client.Player.DuneEnergy3);

                    var array = info.GetArray(pkt);
                    if (array != null)
                    {
                        client.Send(array);
                    }
                    client.Player.DuneStampPower = DateTime.Now;
                }
            }
        }
        public static bool CheckSpells(ushort ID)
        {
            return ID == (ushort)Flags.SpellIDDune.MoonwardLeap
                || ID == (ushort)Flags.SpellIDDune.SwallowDive
                || ID == (ushort)Flags.SpellIDDune.SwallowDive2
                || ID == (ushort)Flags.SpellIDDune.TempestStrike
                || ID == (ushort)Flags.SpellIDDune.CliffCrusher
                || ID == (ushort)Flags.SpellIDDune.WandererNormalATK
                || ID == (ushort)Flags.SpellIDDune.FinalStand
                || ID == (ushort)Flags.SpellIDDune.LonelyBattle
                || ID == (ushort)Flags.SpellIDDune.SheathParry
                || ID == (ushort)Flags.SpellIDDune.FleetingShadow
                || ID == (ushort)Flags.SpellIDDune.TideReversal || ID == (ushort)Flags.SpellIDDune.SkyStep;
        }
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                switch (ClientSpell.ID)
                {
                    case (ushort)Role.Flags.SpellIDDune.MoonwardLeap:
                        {
                            if (!user.Player.Alive)
                                return;
                            if (!Database.AtributesStatus.IsDune(user.Player.Class))
                            {
                                user.SendSysMesage("Sorry, Only Dune Wanderer Character Can Use This Spell !");
                                break;
                            }
                            if (user.IsWatching())
                            {
                                user.SendSysMesage("Sorry, You Cannot Use This Skill In Watching Mode .");
                                break;
                            }
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , 0, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            user.Player.AddSpellFlag(MsgUpdate.Flags.MoonwardLeap, (int)DBSpell.Duration, true);
                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, DBSpell.Duration, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    #region SkyStep
                    case (ushort)Role.Flags.SpellIDDune.SkyStep:
                        {
                            if (user.Player.ContainFlag(MsgUpdate.Flags.Ride))
                                user.Player.RemoveFlag(MsgUpdate.Flags.Ride);

                            if (!user.Player.Alive)
                                return;
                            if (!Database.AtributesStatus.IsDune(user.Player.Class))
                            {
                                user.SendSysMesage("Sorry, Only Dune Wanderer Character Can Use This Spell !");
                                break;
                            }
                            if (user.IsWatching())
                            {
                                user.SendSysMesage("Sorry, You Cannot Use This Skill In Watching Mode .");
                                break;
                            }
                            int test = 10;

                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , 0, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            user.Player.AddFlag(MsgUpdate.Flags.SkyStep, test, true);

                            user.Player.AddFlag(MsgUpdate.Flags.SkyStepFlag2, 10, true);
                            user.Player.SendUpdate(stream, MsgUpdate.Flags.SkyStepFlag2, 10, (uint)0, (uint)0, MsgUpdate.DataType.ArchiveSkill, true);

                            if (user.Player.ContainFlag(MsgUpdate.Flags.Fly))
                                user.Player.UpdateFlag(MsgUpdate.Flags.Fly, test, true, 0);
                            else
                                user.Player.AddFlag(MsgUpdate.Flags.Fly, test, true);

                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, (uint)test, MsgAttackPacket.AttackEffect.None));

                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, (uint)test, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            break;
                        }
                    #endregion
                    #region Swallow Dive
                    case (ushort)Role.Flags.SpellIDDune.SwallowDive:
                        {
                            user.Player.AttackStamp = DateTime.Now.AddMilliseconds(1000);
                            if (!user.Player.Alive)
                                return;
                            if (!Database.AtributesStatus.IsDune(user.Player.Class))
                            {
                                user.SendSysMesage("Sorry, Only Dune Wanderer Character Can Use This Spell !");
                                break;
                            }
                            if (user.IsWatching())
                            {
                                user.SendSysMesage("Sorry, You Cannot Use This Skill In Watching Mode .");
                                break;
                            }
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , 0, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            user.Player.AddSpellFlag(MsgUpdate.Flags.SwallowDive, (int)DBSpell.Duration, true);
                            user.Player.AddFlag(MsgUpdate.Flags.SwallowDive, (int)DBSpell.Duration, true);
                            user.Player.SendUpdate(stream, MsgUpdate.Flags.SwallowDive, 10, (uint)0, (uint)0, MsgUpdate.DataType.ArchiveSkill, true);
                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, DBSpell.Duration, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            break;
                        }
                    #endregion
                    case (ushort)Flags.SpellIDDune.SwallowDive2:
                        {
                            user.Player.AttackStamp = DateTime.Now.AddMilliseconds(1000);
                            uint Experience = 0;
                            if ((int)Attack.X != (int)user.Player.X && Attack.X != (ushort)0 && user.Map.ValidLocation(Attack.X, Attack.Y))
                            {

                                Algoritms.InLineAlgorithm Line = new Algoritms.InLineAlgorithm(user.Player.X, Attack.X, user.Player.Y, Attack.Y, user.Map, (byte)DBSpell.Range, 0);
                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                                foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                                {
                                    if ((int)Calculate.Base.GetDistance(Attack.X, Attack.Y, target.X, target.Y) <= DBSpell.Range && Line.InLine(target.X, target.Y, 2))
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
                                foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Player))
                                {
                                    if ((int)Calculate.Base.GetDistance(Attack.X, Attack.Y, target.X, target.Y) <= DBSpell.Range && Line.InLine(target.X, target.Y, 2))
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
                                    if ((int)Calculate.Base.GetDistance(Attack.X, Attack.Y, target.X, target.Y) <= DBSpell.Range && Line.InLine(target.X, target.Y, 2))
                                    {
                                        var attacked = target as Role.SobNpc;
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
                                Updates.UpdateSpell.CheckUpdate(stream, user, Attack, DBSpell.Duration, DBSpells);
                                MsgSpell.SetStream(stream);
                                MsgSpell.SendRole(user);
                                MsgSpell.Send(user);
                                user.Shift(Attack.X, Attack.Y, stream, false);
                                break;
                            }

                            break;
                        }

                    case (ushort)Role.Flags.SpellIDDune.TempestStrike:
                        {
                            if (!user.Player.Alive)
                                return;
                            if (!Database.AtributesStatus.IsDune(user.Player.Class))
                            {
                                user.SendSysMesage("Sorry, Only Dune Wanderer Character Can Use This Spell !");
                                break;
                            }
                            if (user.IsWatching())
                            {
                                user.SendSysMesage("Sorry, You Cannot Use This Skill In Watching Mode .");
                                break;
                            }
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
                                            AnimationObj.Damage = AnimationObj.Damage * 100 / 500; // تعديل الدمج 
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
                                            AnimationObj.Damage = AnimationObj.Damage * 20 / 100; // تعديل الدمج 
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
                                Updates.IncreaseExperience.Up(stream, user, 600);
                                Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 600, DBSpells);
                                MsgSpell.SetStream(stream);
                                MsgSpell.Send(user);
                                user.Player.AttackStamp = DateTime.Now.AddMilliseconds(500);

                            }
                            break;

                        }
                    case (ushort)Role.Flags.SpellIDDune.WandererNormalATK:
                        {
                            if (!user.Player.Alive)
                                return;
                            if (!Database.AtributesStatus.IsDune(user.Player.Class))
                            {
                                user.SendSysMesage("Sorry, Only Dune Wanderer Character Can Use This Spell !");
                                break;
                            }
                            if (user.IsWatching())
                            {
                                user.SendSysMesage("Sorry, You Cannot Use This Skill In Watching Mode .");
                                break;
                            }
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
                                            // [CombatAudit 2026-10-14] removed custom nerf (official Dune tooltip values now applied in Physical.cs (monsters = power-30000)): AnimationObj.Damage = AnimationObj.Damage * 100 / 500; // تعديل الدمج
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
                                            // [CombatAudit 2026-10-14] removed custom nerf (official Dune tooltip values now applied in Physical.cs (monsters = power-30000)): AnimationObj.Damage = AnimationObj.Damage * 20 / 100; // تعديل الدمج
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
                                Updates.IncreaseExperience.Up(stream, user, 600);
                                Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 600, DBSpells);
                                MsgSpell.SetStream(stream);
                                MsgSpell.Send(user);

                            }
                            break;

                        }
                    case (ushort)Role.Flags.SpellIDDune.CliffCrusher:
                        {
                            if (!user.Player.Alive)
                                return;
                            if (!Database.AtributesStatus.IsDune(user.Player.Class))
                            {
                                user.SendSysMesage("Sorry, Only Dune Wanderer Character Can Use This Spell !");
                                break;
                            }
                            if (user.IsWatching())
                            {
                                user.SendSysMesage("Sorry, You Cannot Use This Skill In Watching Mode .");
                                break;
                            }
                            Role.IMapObj curTarget;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out curTarget, Role.MapObjectType.Player) || user.Player.View.TryGetValue(Attack.OpponentUID, out curTarget, Role.MapObjectType.Monster))
                            {
                                Attack.X = curTarget.X;
                                Attack.Y = curTarget.Y;
                            }
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            Algoritms.InLineAlgorithm Line = new Algoritms.InLineAlgorithm(user.Player.X, Attack.X, user.Player.Y, Attack.Y, user.Map, DBSpell.Range, 0, ClientSpell.ID);
                            byte LineRange = 2;
                            uint Experience = 0;
                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if ((attacked.Family.Settings & MsgMonster.MonsterSettings.Guard) == MsgMonster.MonsterSettings.Guard)
                                {
                                    continue;
                                }
                                if (Role.Core.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) <= DBSpell.Range)
                                {
                                    if (Line.InLine(attacked.X, attacked.Y, LineRange))
                                    {
                                        if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                        {
                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                            // [CombatAudit 2026-10-14] removed custom nerf (official Dune tooltip values now applied in Physical.cs (monsters = power-30000)): AnimationObj.Damage = AnimationObj.Damage * 100 / 500; // تعديل الدمج
                                            Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                            MsgSpell.Targets.Enqueue(AnimationObj);
                                            if (user.OnAutoAttack == false)
                                            {
                                                unsafe
                                                {
                                                    InteractQuery _attack = new InteractQuery();
                                                    _attack.Damage = (int)AnimationObj.Damage;
                                                    _attack.X = attacked.X;
                                                    _attack.Y = attacked.Y;
                                                    _attack.OpponentUID = attacked.UID;
                                                    _attack.AtkType = (ushort)MsgAttackPacket.AttackID.Physical;
                                                    _attack.UID = user.Player.UID;
                                                    _attack.Effect = (uint)AnimationObj.Effect;
                                                    user.Send(stream.InteractionCreate(_attack));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                var attacked = targer as Role.Player;
                                if (Role.Core.GetDistance(user.Player.X, user.Player.Y, targer.X, targer.Y) <= DBSpell.Range)
                                {
                                    if (Line.InLine(attacked.X, attacked.Y, LineRange))
                                    {
                                        if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                                        {
                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                            ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                            MsgSpell.Targets.Enqueue(AnimationObj);
                                            if (user.OnAutoAttack == false)
                                            {
                                                unsafe
                                                {
                                                    InteractQuery _attack = new InteractQuery();
                                                    _attack.Damage = (int)AnimationObj.Damage;
                                                    _attack.X = attacked.X;
                                                    _attack.Y = attacked.Y;
                                                    _attack.OpponentUID = attacked.UID;
                                                    _attack.AtkType = (ushort)MsgAttackPacket.AttackID.Physical;
                                                    _attack.UID = user.Player.UID;
                                                    _attack.Effect = (uint)AnimationObj.Effect;
                                                    user.Send(stream.InteractionCreate(_attack));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                var attacked = targer as Role.SobNpc;
                                if (Role.Core.GetDistance(user.Player.X, user.Player.Y, targer.X, targer.Y) <= DBSpell.Range)
                                {
                                    {
                                        if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                        {
                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);

                                            Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                            MsgSpell.Targets.Enqueue(AnimationObj);
                                            if (user.OnAutoAttack == false)
                                            {
                                                unsafe
                                                {
                                                    InteractQuery _attack = new InteractQuery();
                                                    _attack.Damage = (int)AnimationObj.Damage;
                                                    _attack.X = attacked.X;
                                                    _attack.Y = attacked.Y;
                                                    _attack.OpponentUID = attacked.UID;
                                                    _attack.AtkType = (ushort)MsgAttackPacket.AttackID.Physical;
                                                    _attack.UID = user.Player.UID;
                                                    _attack.Effect = (uint)AnimationObj.Effect;
                                                    user.Send(stream.InteractionCreate(_attack));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (MsgSpell.Targets.Count == 0)
                            {
                                return;
                            }
                            Updates.IncreaseExperience.Up(stream, user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Role.Flags.SpellIDDune.FinalStand:
                        {
                            if (!user.Player.Alive)
                                return;
                            if (!Database.AtributesStatus.IsDune(user.Player.Class))
                            {
                                user.SendSysMesage("Sorry, Only Dune Wanderer Character Can Use This Spell !");
                                break;
                            }
                            if (user.IsWatching())
                            {
                                user.SendSysMesage("Sorry, You Cannot Use This Skill In Watching Mode .");
                                break;
                            }
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            if (!user.Player.FloorSpells.ContainsKey(ClientSpell.ID))
                            {
                                user.Player.FloorSpells.TryAdd(ClientSpell.ID, new Role.FloorSpell.ClientFloorSpells(user.Player.UID, user.Player.X, user.Player.Y, ClientSpell.SoulLevel, DBSpell, user.Map));
                            }
                            var FloorItem = new Role.FloorSpell(Game.MsgFloorItem.MsgItemPacket.FinalStand, (ushort)Attack.X, (ushort)Attack.Y, 23, DBSpell, 1000, 500);
                            FloorItem.FloorPacket.m_X = Attack.X;
                            FloorItem.FloorPacket.m_Y = Attack.Y;
                            FloorItem.FloorPacket.OwnerX = user.Player.X;
                            FloorItem.FloorPacket.OwnerY = user.Player.Y;
                            FloorItem.FloorPacket.GuildID = user.Player.GuildID;
                            FloorItem.FloorPacket.ItemOwnerUID = user.Player.UID;
                            FloorItem.FloorPacket.m_Color = 14;
                            FloorItem.FloorPacket.FlowerType = 31;
                            FloorItem.FloorPacket.Name = "STR_TRAP_ID_@@" + (uint)FloorItem.FloorPacket.m_ID;
                            user.Player.FloorSpells[ClientSpell.ID].AddItem(FloorItem);
                            user.Player.View.SendView(stream.ItemPacketCreate(FloorItem.FloorPacket), true);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            uint Experience = 0;
                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < DBSpell.MaxTargets)
                                {
                                    if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                        // [CombatAudit 2026-10-14] removed custom nerf (official Dune tooltip values now applied in Physical.cs (monsters = power-30000)): AnimationObj.Damage = AnimationObj.Damage * 100 / 500; // تعديل الدمج
                                        Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                var attacked = targer as Role.Player;
                                if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < DBSpell.MaxTargets)
                                {
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
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
                                var attacked = targer as Role.SobNpc;
                                if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < DBSpell.MaxTargets)
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
                            user.Player.AttackStamp = DateTime.Now.AddMilliseconds(500);
                            break;
                        }
                    case (ushort)Role.Flags.SpellIDDune.LonelyBattle:
                        {
                            if (!user.Player.Alive)
                                return;
                            if (!Database.AtributesStatus.IsDune(user.Player.Class))
                            {
                                user.SendSysMesage("Sorry, Only Dune Wanderer Character Can Use This Spell !");
                                break;
                            }
                            if (user.IsWatching())
                            {
                                user.SendSysMesage("Sorry, You Cannot Use This Skill In Watching Mode .");
                                break;
                            }
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            if (!user.Player.FloorSpells.ContainsKey(ClientSpell.ID))
                            {
                                user.Player.FloorSpells.TryAdd(ClientSpell.ID, new Role.FloorSpell.ClientFloorSpells(user.Player.UID, user.Player.X, user.Player.Y, ClientSpell.SoulLevel, DBSpell, user.Map));
                            }
                            var FloorItem = new Role.FloorSpell(Game.MsgFloorItem.MsgItemPacket.LonelyBattle, (ushort)Attack.X, (ushort)Attack.Y, 23, DBSpell, 3000, 900);
                            FloorItem.FloorPacket.m_X = Attack.X;
                            FloorItem.FloorPacket.m_Y = Attack.Y;
                            FloorItem.FloorPacket.OwnerX = user.Player.X;
                            FloorItem.FloorPacket.OwnerY = user.Player.Y;
                            FloorItem.FloorPacket.GuildID = user.Player.GuildID;
                            FloorItem.FloorPacket.ItemOwnerUID = user.Player.UID;
                            FloorItem.FloorPacket.m_Color = 14;
                            FloorItem.FloorPacket.FlowerType = 36;
                            FloorItem.FloorPacket.Name = "STR_TRAP_ID_@@" + (uint)FloorItem.FloorPacket.m_ID;
                            user.Player.FloorSpells[ClientSpell.ID].AddItem(FloorItem);
                            user.Player.View.SendView(stream.ItemPacketCreate(FloorItem.FloorPacket), true);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            user.Player.AttackStamp = DateTime.Now.AddMilliseconds(500);
                            break;
                        }
                    case (ushort)Role.Flags.SpellIDDune.FleetingShadow:
                        {
                            if (!user.Player.Alive)
                                return;
                            if (!Database.AtributesStatus.IsDune(user.Player.Class))
                            {
                                user.SendSysMesage("Sorry, Only Dune Wanderer Character Can Use This Spell !");
                                break;
                            }
                            if (user.IsWatching())
                            {
                                user.SendSysMesage("Sorry, You Cannot Use This Skill In Watching Mode .");
                                break;
                            }
                            if (user.Player.ContainFlag(MsgUpdate.Flags.XPList) == false)
                            {
                                break;
                            }
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                             , 0, Attack.X, Attack.Y, ClientSpell.ID
                             , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            user.Player.RemoveFlag(MsgUpdate.Flags.XPList);
                            user.Player.AddFlag(MsgUpdate.Flags.FleetingShadow, 35, true);
                            user.Player.OpenXpSkill(MsgUpdate.Flags.FleetingShadow, 35);

                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Role.Flags.SpellIDDune.TideReversal:
                        {
                            if (!user.Player.Alive)
                                return;
                            if (!Database.AtributesStatus.IsDune(user.Player.Class))
                            {
                                user.SendSysMesage("Sorry, Only Dune Wanderer Character Can Use This Spell !");
                                break;
                            }
                            if (user.IsWatching())
                            {
                                user.SendSysMesage("Sorry, You Cannot Use This Skill In Watching Mode .");
                                break;
                            }
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
                                MsgSpell.Send(user);
                            }
                            break;
                        }
                }
            }
        }
        public static void ExecuteSkyStepAuto(Client.GameClient client, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {

            if (client == null || client.Player == null)
                return;

            var player = client.Player;

            if (!player.ContainFlag(MsgUpdate.Flags.SkyStep))
                return;

            Database.MagicType.Magic DBSpell = null;

            foreach (var kvp in DBSpells)
            {
                if (kvp.Value.ID == 25780)
                {
                    DBSpell = kvp.Value;
                    break;
                }
            }

            if (DBSpell == null)
            {
                Console.WriteLine("Sorry, Sky Step Skill Not Found In DB Spells .");
                return;
            }

            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(
                    player.UID,
                    0,
                    player.X,
                    player.Y,
                    (ushort)Role.Flags.SpellIDDune.SkyStep,
                    DBSpell.Level,
                    0);

                #region Monsters
                foreach (Role.IMapObj obj in player.View.Roles(Role.MapObjectType.Monster))
                {
                    var monster = obj as MsgMonster.MonsterRole;

                    if (Role.Core.GetDistance(player.X, player.Y, monster.X, monster.Y) <= 7)
                    {
                        if (CheckAttack.CanAttackMonster.Verified(client, monster, DBSpell))
                        {
                            MsgSpellAnimation.SpellObj anim;
                            Calculate.Physical.OnMonster(player, monster, DBSpell, out anim);

                            anim.Damage = (uint)((anim.Damage * 300) / 100);

                            ReceiveAttack.Monster.Execute(stream, anim, client, monster);
                            MsgSpell.Targets.Enqueue(anim);
                        }
                    }
                }
                #endregion

                #region Players
                foreach (Role.IMapObj obj in player.View.Roles(Role.MapObjectType.Player))
                {
                    var target = obj as Role.Player;

                    if (Role.Core.GetDistance(player.X, player.Y, target.X, target.Y) <= 7)
                    {
                        if (CheckAttack.CanAttackPlayer.Verified(client, target, DBSpell))
                        {
                            MsgSpellAnimation.SpellObj anim;
                            Calculate.Physical.OnPlayer(player, target, DBSpell, out anim);

                            anim.Damage = anim.Damage * 10 / 100;

                            ReceiveAttack.Player.Execute(anim, client, target);
                            MsgSpell.Targets.Enqueue(anim);
                        }
                    }
                }

                #endregion

                if (MsgSpell.Targets.Count > 0)
                {
                    MsgSpell.SetStream(stream);
                    MsgSpell.Send(client);
                }

            }

        }

    }
}