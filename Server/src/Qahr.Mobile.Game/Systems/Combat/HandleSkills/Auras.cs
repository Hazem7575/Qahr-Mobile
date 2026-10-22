using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{
    public class Auras
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                switch (ClientSpell.ID)
                {
                    case (ushort)Role.Flags.SpellID.MagicDefender:
                        {
                            if (user.Player.ContainFlag(MsgUpdate.Flags.Ride))
                                user.Player.RemoveFlag(MsgUpdate.Flags.Ride);

                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                        , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                                        , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            if (!user.Player.ContainFlag(MsgUpdate.Flags.MagicDefender))
                            {
                                user.Player.AddFlag(MsgUpdate.Flags.MagicDefender, (int)DBSpell.Duration, true);
                                user.Player.SendUpdate(stream, Game.MsgServer.MsgUpdate.Flags.MagicDefender, DBSpell.Duration
           , 0, ClientSpell.Level, Game.MsgServer.MsgUpdate.DataType.AzureShield, true);
                            }
                            else
                            {
                                user.Player.RemoveFlag(MsgUpdate.Flags.MagicDefender);
                                user.Player.SendUpdate(stream, Game.MsgServer.MsgUpdate.Flags.MagicDefender, 0
               , 0, ClientSpell.Level, Game.MsgServer.MsgUpdate.DataType.AzureShield, true);
                            }

                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 200, DBSpells);


                            break;
                        }
                    case (ushort)Role.Flags.SpellID.FendAura:
                    case (ushort)Role.Flags.SpellID.TyrantAura:
                    case (ushort)Role.Flags.SpellID.MetalAura:
                    case (ushort)Role.Flags.SpellID.WatherAura:
                    case (ushort)Role.Flags.SpellID.WoodAura:
                    case (ushort)Role.Flags.SpellID.FireAura:
                    case (ushort)Role.Flags.SpellID.EarthAura:
                        {
                            MsgUpdate.Flags Aura = MsgUpdate.Flags.Normal;
                            if (ClientSpell.ID == (ushort)Role.Flags.SpellID.FendAura)
                                Aura = MsgUpdate.Flags.FendAura;
                            else if (ClientSpell.ID == (ushort)Role.Flags.SpellID.TyrantAura)
                                Aura = MsgUpdate.Flags.TyrantAura;
                            else if (ClientSpell.ID == (ushort)Role.Flags.SpellID.MetalAura)
                                Aura = MsgUpdate.Flags.MetalAura;
                            else if (ClientSpell.ID == (ushort)Role.Flags.SpellID.WoodAura)
                                Aura = MsgUpdate.Flags.WoodAura;
                            else if (ClientSpell.ID == (ushort)Role.Flags.SpellID.WatherAura)
                                Aura = MsgUpdate.Flags.WaterAura;
                            else if (ClientSpell.ID == (ushort)Role.Flags.SpellID.FireAura)
                                Aura = MsgUpdate.Flags.FireAura;
                            else if (ClientSpell.ID == (ushort)Role.Flags.SpellID.EarthAura)
                                Aura = MsgUpdate.Flags.EartAura;

                            if (Aura == MsgUpdate.Flags.Normal)
                                return;

                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                        , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                                        , ClientSpell.Level, ClientSpell.UseSpellSoul);


                            bool IncreaseExp = false;
                            if (user.Player.UID == Attack.OpponentUID)
                            {
                                IncreaseExp = user.Player.AddAura(Aura, DBSpell, Role.StatusFlagsBigVector32.PermanentFlag);

                            }
                            else
                            {
                                Role.IMapObj target;
                                if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                                {
                                    Role.Player attacked = target as Role.Player;
                                    IncreaseExp = attacked.AddAura(Aura, DBSpell, Role.StatusFlagsBigVector32.PermanentFlag);
                                }
                            }

                            if (user.Team != null)
                            {
                                foreach (var target in user.Team.GetMembers())
                                {
                                    if (target.Player.UID == user.Player.UID)
                                        continue;
                                    if (target.Player.UseAura == Aura)
                                        continue;
                                    if (Role.Core.GetDistance(user.Player.X, user.Player.Y, target.Player.X, target.Player.Y) < 18)
                                    {
                                        target.Player.AddAura(Aura, DBSpell , Role.StatusFlagsBigVector32.PermanentFlag);
                                        MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(target.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                                    }
                                }
                            }

                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            if (IncreaseExp)
                                Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 200, DBSpells);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.NobleSpirit:
                        {
                            MsgUpdate.Flags Aura = MsgUpdate.Flags.Normal;

                            if (ClientSpell.ID == (ushort)Role.Flags.SpellID.NobleSpirit)
                                Aura = MsgUpdate.Flags.NobleSpirit;

                            if (Aura == MsgUpdate.Flags.Normal)
                                return;

                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                       , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                                       , ClientSpell.Level, ClientSpell.UseSpellSoul);


                            bool IncreaseExp = false;
                            if (user.Player.UID == Attack.OpponentUID)
                            {
                                IncreaseExp = user.Player.AddAura(Aura, DBSpell, 20);

                            }
                            else
                            {
                                Role.IMapObj target;
                                if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                                {
                                    Role.Player attacked = target as Role.Player;
                                    IncreaseExp = attacked.AddAura(Aura, DBSpell, 20);
                                }
                            }

                            if (user.Team != null)
                            {
                                foreach (var target in user.Team.GetMembers())
                                {
                                    if (target.Player.UID == user.Player.UID)
                                        continue;
                                    if (target.Player.UseAura == Aura)
                                        continue;
                                    if (Role.Core.GetDistance(user.Player.X, user.Player.Y, target.Player.X, target.Player.Y) < 18)
                                    {
                                        target.Player.AddAura(Aura, DBSpell, 20);
                                        MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(target.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                                    }
                                }
                            }

                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            if (IncreaseExp)
                                Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 200, DBSpells);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.WackeSpirit:
                        {
                            MsgUpdate.Flags Aura = MsgUpdate.Flags.Normal;

                            if (ClientSpell.ID == (ushort)Role.Flags.SpellID.WackeSpirit)
                                Aura = MsgUpdate.Flags.WackeSpirit;

                            if (Aura == MsgUpdate.Flags.Normal)
                                return;

                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                       , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                                       , ClientSpell.Level, ClientSpell.UseSpellSoul);


                            bool IncreaseExp = false;
                            if (user.Player.UID == Attack.OpponentUID)
                            {
                                IncreaseExp = user.Player.AddAura(Aura, DBSpell, Role.StatusFlagsBigVector32.PermanentFlag);
                               

                            }
                            else
                            {
                                Role.IMapObj target;
                                if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                                {
                                    Role.Player attacked = target as Role.Player;
                                    IncreaseExp = attacked.AddAura(Aura, DBSpell, Role.StatusFlagsBigVector32.PermanentFlag);
                                }
                            }

                            if (user.Team != null)
                            {
                                foreach (var target in user.Team.GetMembers())
                                {
                                    if (target.Player.UID == user.Player.UID)
                                        continue;
                                    if (target.Player.UseAura == Aura)
                                        continue;
                                    if (Role.Core.GetDistance(user.Player.X, user.Player.Y, target.Player.X, target.Player.Y) < 18)
                                    {
                                        target.Player.AddAura(Aura, DBSpell, 30);
                                        MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(target.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                                    }
                                }
                            }

                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            if (IncreaseExp)
                                Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 200, DBSpells);
                            break;
                        }
                }
            }
        }
    }
}
