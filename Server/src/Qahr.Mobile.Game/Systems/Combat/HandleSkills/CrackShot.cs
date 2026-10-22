using Qahr.Mobile.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgFloorItem;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{
    public class CrackShot
    {

        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                switch (ClientSpell.ID)
                {
                    case (ushort)Role.Flags.SpellID.CrackShot:
                        {


                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , 0, 0, 0, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            Role.IMapObj target;
                            List<byte> CanUse = new List<byte>();
                            uint SpellEffect = 0;
                            bool Active = false;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                {

                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                    AnimationObj.Damage = AnimationObj.Damage * 10 / 500; // Mohe Done (Attack)

                                    ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                    Active = true;
                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {
                                var attacked = target as Role.Player;
                                if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);

                                    ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                    Active = true;
                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc))
                            {
                                var attacked = target as Role.SobNpc;
                                if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                {
                                    short distance = Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y);
                                    if (distance <= DBSpell.Range)
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                        ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                        Active = true;
                                    }
                                }
                            }
                            if (Active)
                            {

                                if (user.Player.ContainFlag(MsgUpdate.Flags.FireArrow))
                                    CanUse.Add(1);
                                if (user.Player.ContainFlag(MsgUpdate.Flags.IceArrow))
                                    CanUse.Add(2);
                                if (user.Player.ContainFlag(MsgUpdate.Flags.PoisonArrow))
                                    CanUse.Add(3);
                                if (user.Player.ContainFlag(MsgUpdate.Flags.ThunderArrow))
                                    CanUse.Add(4);
                                if (user.Player.ContainFlag(MsgUpdate.Flags.WindArrow))
                                    CanUse.Add(5);
                                if (CanUse.Count != 0)
                                    SpellEffect = CanUse[Pool.GetRandom.Next(0, CanUse.Count)];
                                MsgSpell user_spell = null;

                                if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.ElementalArrowData, out user_spell))
                                {
                                    Database.MagicType.Magic Data = Pool.Magic[user_spell.ID][user_spell.Level];
                                    CanUse = new List<byte>();
                                    if (!user.Player.ContainFlag(MsgUpdate.Flags.FireArrow))
                                        CanUse.Add(1);
                                    if (!user.Player.ContainFlag(MsgUpdate.Flags.IceArrow))
                                        CanUse.Add(2);

                                    if (!user.Player.ContainFlag(MsgUpdate.Flags.PoisonArrow))
                                        CanUse.Add(3);
                                    if (!user.Player.ContainFlag(MsgUpdate.Flags.ThunderArrow))
                                        CanUse.Add(4);
                                    if (!user.Player.ContainFlag(MsgUpdate.Flags.WindArrow))
                                        CanUse.Add(5);
                                    if (CanUse.Count != 0)
                                    {
                                        switch (CanUse[Pool.GetRandom.Next(0, CanUse.Count)])
                                        {
                                            case 1:
                                                {
                                                    user.Player.AddSpellFlag(MsgUpdate.Flags.FireArrow, 10, true);
                                                    user.Player.SendUpdate(stream, MsgUpdate.Flags.FireArrow, 10, (uint)100, (uint)0, MsgUpdate.DataType.ArchiveSkill);
                                                    break;
                                                }
                                            case 2:
                                                {
                                                    user.Player.AddSpellFlag(MsgUpdate.Flags.IceArrow, 10, true);
                                                    user.Player.SendUpdate(stream, MsgUpdate.Flags.IceArrow, 10, (uint)Data.DamageOnHuman, (uint)Data.Damage2, MsgUpdate.DataType.ArchiveSkill);
                                                    break;
                                                }
                                            case 3:
                                                {
                                                    user.Player.AddSpellFlag(MsgUpdate.Flags.PoisonArrow, 10, true);
                                                    user.Player.SendUpdate(stream, MsgUpdate.Flags.PoisonArrow, 10, (uint)Data.Damage3, (uint)Data.Damage3, MsgUpdate.DataType.ArchiveSkill);
                                                    break;
                                                }
                                            case 4:
                                                {
                                                    user.Player.AddSpellFlag(MsgUpdate.Flags.ThunderArrow, 10, true);
                                                    user.Player.SendUpdate(stream, MsgUpdate.Flags.ThunderArrow, 10, (uint)200, 0, MsgUpdate.DataType.ArchiveSkill);
                                                    break;
                                                }
                                            case 5:
                                                {
                                                    user.Player.AddSpellFlag(MsgUpdate.Flags.WindArrow, 10, true);
                                                    user.Player.SendUpdate(stream, MsgUpdate.Flags.WindArrow, 10, (uint)Data.DamageOnMonster, 0, MsgUpdate.DataType.ArchiveSkill);
                                                    break;
                                                }
                                        }
                                    }
                                }
                                user.MyArchives.Handel(target, false);
                                MsgSpell.bomb = SpellEffect;
                                user.Equipment.QueryEquipment(user.Equipment.Alternante);
                            }
                            Updates.IncreaseExperience.Up(stream, user, 1000);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 1000, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }


                }
            }

        }

    }
}
