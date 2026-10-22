using System.Collections.Generic;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{
    public class CounterKill
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                switch (ClientSpell.ID)
                {
                    case (ushort)Role.Flags.SpellID.HeavensWonder:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) <= 9)
                                {
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
                                var attacked = targer as Role.Player;
                                if (targer.UID == Attack.OpponentUID)
                                {
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                    {
                                        int DivineAnnihilationRate = 0;
                                        bool CanUse = true;
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        if (attacked.Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DivineAnnihilation))
                                        {
                                            var DivineEmptiness = Pool.Magic[(ushort)Role.Flags.SpellID.DivineAnnihilation][attacked.Owner.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.DivineAnnihilation].Level];
                                            DivineAnnihilationRate = DivineEmptiness.DamageOnHuman / 100;
                                            if (!Role.Core.Rate(DivineAnnihilationRate))
                                            {
                                                CanUse = false;
                                            }
                                        }
                                        if (CanUse)
                                        {
                                            Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                            AnimationObj.Damage = AnimationObj.Damage * 215 / 100;
                                            ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                            MsgSpell.Targets.Enqueue(AnimationObj);
                                        }
                                    }
                                }

                            }
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                }
            }
        }
    }
}