using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{
    public class RemoveStamin
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                switch (ClientSpell.ID)
                {
                    case (ushort)Role.Flags.SpellID.FrostGazeI:
                    case (ushort)Role.Flags.SpellID.FrostGazeII:
                    case (ushort)Role.Flags.SpellID.FrostGazeIII:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            uint Experience = 0;

                           
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                var attacked = targer as Role.Player;
                                if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) <= DBSpell.Range)
                                {
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                    {
                                        if (Role.Core.Rate(100 - 5 * (attacked.BattlePower - user.Player.BattlePower)))
                                        {
                                            uint removeStamin = (uint)DBSpell.Damage;
                                            byte level = 0;
                                            int Rate = 0;
                                            if (attacked.Owner.Rune.IsEquipped("Consolidation", ref level))
                                            {
                                                switch (level)
                                                {
                                                    case 1: Rate = 30; break;
                                                    case 2: Rate = 35; break;
                                                    case 3: Rate = 40; break;
                                                    case 4: Rate = 45; break;
                                                    case 5: Rate = 55; break;
                                                    case 6: Rate = 60; break;
                                                    case 7: Rate = 65; break;
                                                    case 8: Rate = 70; break;
                                                    case 9: Rate = 80; break;
                                                }
                                                if (!Role.Core.Rate(Rate))
                                                {
                                                    if (attacked.Stamina >= removeStamin)
                                                    {
                                                        attacked.Stamina -= (ushort)removeStamin;
                                                    }
                                                    else
                                                        attacked.Stamina = 0;

                                                    attacked.SendUpdate(stream, attacked.Stamina, MsgUpdate.DataType.Stamina, false);
                                                }
                                            }
                                       
                                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj() { Damage = removeStamin, UID = targer.UID, Hit = 1 });
                                        }
                                        else
                                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj() { Damage = 0, UID = targer.UID, Hit = 0 });
                                    }
                                }
                            }
                            Updates.IncreaseExperience.Up(stream, user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream); MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.FrostArrows:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            uint Experience = 0;


                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                var attacked = targer as Role.Player;
                                if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= DBSpell.Range && Calculate.Base.GetDistance(user.Player.X, user.Player.Y, Attack.X, Attack.Y) <= 8)
                                {
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                                    {
                                        byte itemLevel = 0;
                                        int Consolidation = 0;
                                        if (attacked.Owner.Rune.IsEquipped("Consolidation", ref itemLevel))
                                        {
                                            switch (itemLevel)
                                            {
                                                case 1: Consolidation = 30; break;
                                                case 2: Consolidation = 35; break;
                                                case 3: Consolidation = 40; break;
                                                case 4: Consolidation = 45; break;
                                                case 5: Consolidation = 55; break;
                                                case 6: Consolidation = 60; break;
                                                case 7: Consolidation = 65; break;
                                                case 8: Consolidation = 70; break;
                                                case 9: Consolidation = 80; break;
                                            }
                                        }
                                        if (!Role.Core.Rate(Consolidation))
                                        {
                                            attacked.AddFlag(MsgUpdate.Flags.FrostArrows, (int)DBSpell.Duration, true);
                                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj() { Damage = 0, UID = targer.UID, Hit = 1 });
                                        }
                                    }
                                }
                            }
                            Updates.IncreaseExperience.Up(stream, user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream); MsgSpell.Send(user);
                            break;
                        }
                   
                }
            }
        }
    }
}
