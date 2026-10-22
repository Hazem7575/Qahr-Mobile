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
    public class SuanniCommand
    {

        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                switch (ClientSpell.ID)
                {
                    case (ushort)Role.Flags.SpellID.SuanniCommand:
                        {
                            if (user.Player.SuanniCommandCount > 5)
                                break;
                            user.Player.SuanniCommandCount += 1;
                            user.Send(stream.InteractionCreate(Attack));
                          
                            if (user.Player.SuanniCommandCount == 1)
                                
                            {
                                user.Player.AddSpellFlag(MsgUpdate.Flags.Commanding, (int)DBSpell.Duration, true);
                                user.Player.SendUpdate(stream, MsgUpdate.Flags.Commanding, (uint)DBSpell.Duration, (uint)DBSpell.DamageOnHuman, (uint)user.Player.SuanniCommandCount, MsgUpdate.DataType.ArchiveSkill, true);
                            }
                            else if (user.Player.SuanniCommandCount == 2)
                            {
                                user.Player.AddSpellFlag(MsgUpdate.Flags.Commanding, (int)DBSpell.Duration, true);
                                user.Player.SendUpdate(stream, MsgUpdate.Flags.Commanding, (uint)DBSpell.Duration, (uint)DBSpell.DamageOnHuman * user.Player.SuanniCommandCount, (uint)user.Player.SuanniCommandCount, MsgUpdate.DataType.ArchiveSkill, true);
                            }
                            else if (user.Player.SuanniCommandCount == 3)
                            {
                                user.Player.AddSpellFlag(MsgUpdate.Flags.Commanding, (int)DBSpell.NewDamage4 % 10, true);
                                user.Player.SendUpdate(stream, MsgUpdate.Flags.Commanding, (uint)DBSpell.NewDamage4 % 10, (uint)DBSpell.DamageOnHuman * user.Player.SuanniCommandCount, (uint)user.Player.SuanniCommandCount, MsgUpdate.DataType.ArchiveSkill, true);
                            }
                            else if (user.Player.SuanniCommandCount == 4)
                            {
                                user.Player.AddSpellFlag(MsgUpdate.Flags.Commanding, (int)DBSpell.NewDamage4 % 10, true);
                                user.Player.SendUpdate(stream, MsgUpdate.Flags.Commanding, (uint)DBSpell.NewDamage4 % 10, (uint)DBSpell.DamageOnHuman * user.Player.SuanniCommandCount, (uint)user.Player.SuanniCommandCount, MsgUpdate.DataType.ArchiveSkill, true);
                            }
                            else if (user.Player.SuanniCommandCount == 5)
                            {
                                user.Player.AddSpellFlag(MsgUpdate.Flags.Commanding, (int)DBSpell.DamageOnHuman, true);
                                user.Player.SendUpdate(stream, MsgUpdate.Flags.Commanding, (uint)DBSpell.DamageOnHuman, (uint)DBSpell.DamageOnHuman * 6, (uint)user.Player.SuanniCommandCount, MsgUpdate.DataType.ArchiveSkill, true);
                            }

                            else if (user.Player.SuanniCommandCount == 1)
                            {
                                user.Player.AddSpellFlag(MsgUpdate.Flags.Commanding, (int)DBSpell.Duration, true);
                                user.Player.SendUpdate(stream, MsgUpdate.Flags.Commanding, (uint)DBSpell.Duration, (uint)DBSpell.DamageOnHuman, (uint)user.Player.SuanniCommandCount, MsgUpdate.DataType.SuanniCommand, true);
                            }
                            else if (user.Player.SuanniCommandCount == 2)
                            {
                                user.Player.AddSpellFlag(MsgUpdate.Flags.Commanding, (int)DBSpell.Duration, true);
                                user.Player.SendUpdate(stream, MsgUpdate.Flags.Commanding, (uint)DBSpell.Duration, (uint)DBSpell.DamageOnHuman * user.Player.SuanniCommandCount, (uint)user.Player.SuanniCommandCount, MsgUpdate.DataType.SuanniCommand, true);
                            }
                            else if (user.Player.SuanniCommandCount == 3)
                            {
                                user.Player.AddSpellFlag(MsgUpdate.Flags.Commanding, (int)DBSpell.Duration, true);
                                user.Player.SendUpdate(stream, MsgUpdate.Flags.Commanding, (uint)DBSpell.NewDamage4 % 10, (uint)DBSpell.DamageOnHuman * user.Player.SuanniCommandCount, (uint)user.Player.SuanniCommandCount, MsgUpdate.DataType.SuanniCommand, true);
                            }
                            else if (user.Player.SuanniCommandCount == 4)
                            {
                                user.Player.SendUpdate(stream, MsgUpdate.Flags.Commanding, (uint)DBSpell.NewDamage4 % 10, (uint)DBSpell.DamageOnHuman * user.Player.SuanniCommandCount, (uint)user.Player.SuanniCommandCount, MsgUpdate.DataType.SuanniCommand, true);
                            }
                            else if (user.Player.SuanniCommandCount == 5)
                            {
                                user.Player.AddSpellFlag(MsgUpdate.Flags.Commanding, (int)DBSpell.Duration, true);
                                user.Player.SendUpdate(stream, MsgUpdate.Flags.Commanding, (uint)DBSpell.DamageOnHuman, (uint)DBSpell.DamageOnHuman * 6, (uint)user.Player.SuanniCommandCount, MsgUpdate.DataType.SuanniCommand, true);
                            }
                            if (user.Team != null)
                            {
                                foreach (var target in user.Team.GetMembers())
                                {
                                    
                                    if (Role.Core.GetDistance(user.Player.X, user.Player.Y, target.Player.X, target.Player.Y) < 18)
                                    {
                                        if (target.Player.SuanniCommandCount == 1)
                                        {
                                            target.Player.AddSpellFlag(MsgUpdate.Flags.Commanding, (int)DBSpell.Duration, true);
                                            target.Player.SendUpdate(stream, MsgUpdate.Flags.Commanding, (uint)DBSpell.Duration, (uint)DBSpell.DamageOnHuman, user.Player.SuanniCommandCount , MsgUpdate.DataType.ArchiveSkill, true);
                                        }
                                        else if (target.Player.SuanniCommandCount == 2)
                                        {
                                            target.Player.AddSpellFlag(MsgUpdate.Flags.Commanding, (int)DBSpell.Duration, true);
                                            target.Player.SendUpdate(stream, MsgUpdate.Flags.Commanding, (uint)DBSpell.Duration, (uint)DBSpell.DamageOnHuman * user.Player.SuanniCommandCount, (uint)user.Player.SuanniCommandCount, MsgUpdate.DataType.ArchiveSkill, true);
                                        }
                                        else if (target.Player.SuanniCommandCount == 3)
                                        {
                                            target.Player.AddSpellFlag(MsgUpdate.Flags.Commanding, (int)DBSpell.NewDamage4 % 10, true);
                                            target.Player.SendUpdate(stream, MsgUpdate.Flags.Commanding, (uint)DBSpell.NewDamage4 % 10, (uint)DBSpell.DamageOnHuman * user.Player.SuanniCommandCount, (uint)user.Player.SuanniCommandCount, MsgUpdate.DataType.ArchiveSkill, true);
                                        }
                                        else if (target.Player.SuanniCommandCount == 4)
                                        {
                                            target.Player.AddSpellFlag(MsgUpdate.Flags.Commanding, (int)DBSpell.NewDamage4 % 10, true);
                                            target.Player.SendUpdate(stream, MsgUpdate.Flags.Commanding, (uint)DBSpell.NewDamage4 % 10, (uint)DBSpell.DamageOnHuman * user.Player.SuanniCommandCount, (uint)user.Player.SuanniCommandCount, MsgUpdate.DataType.ArchiveSkill, true);
                                        }
                                        else if (target.Player.SuanniCommandCount == 5)
                                        {
                                            target.Player.AddSpellFlag(MsgUpdate.Flags.Commanding, (int)DBSpell.DamageOnHuman, true);
                                            target.Player.SendUpdate(stream, MsgUpdate.Flags.Commanding, (uint)DBSpell.DamageOnHuman, (uint)DBSpell.DamageOnHuman * 6, (uint)user.Player.SuanniCommandCount, MsgUpdate.DataType.ArchiveSkill, true);
                                        }

                                        else if (target.Player.SuanniCommandCount == 1)
                                        {
                                            target.Player.AddSpellFlag(MsgUpdate.Flags.Commanding, (int)DBSpell.Duration, true);
                                            target.Player.SendUpdate(stream, MsgUpdate.Flags.Commanding, (uint)DBSpell.Duration, (uint)DBSpell.DamageOnHuman, (uint)user.Player.SuanniCommandCount, MsgUpdate.DataType.SuanniCommand, true);
                                        }
                                        else if (target.Player.SuanniCommandCount == 2)
                                        {
                                            target.Player.AddSpellFlag(MsgUpdate.Flags.Commanding, (int)DBSpell.Duration, true);
                                            target.Player.SendUpdate(stream, MsgUpdate.Flags.Commanding, (uint)DBSpell.Duration, (uint)DBSpell.DamageOnHuman * user.Player.SuanniCommandCount, (uint)user.Player.SuanniCommandCount, MsgUpdate.DataType.SuanniCommand, true);
                                        }
                                        else if (target.Player.SuanniCommandCount == 3)
                                        {
                                            target.Player.AddSpellFlag(MsgUpdate.Flags.Commanding, (int)DBSpell.Duration, true);
                                            target.Player.SendUpdate(stream, MsgUpdate.Flags.Commanding, (uint)DBSpell.NewDamage4 % 10, (uint)DBSpell.DamageOnHuman * user.Player.SuanniCommandCount, (uint)user.Player.SuanniCommandCount, MsgUpdate.DataType.SuanniCommand, true);
                                        }
                                        else if (target.Player.SuanniCommandCount == 4)
                                        {
                                            target.Player.SendUpdate(stream, MsgUpdate.Flags.Commanding, (uint)DBSpell.NewDamage4 % 10, (uint)DBSpell.DamageOnHuman * user.Player.SuanniCommandCount, (uint)user.Player.SuanniCommandCount, MsgUpdate.DataType.SuanniCommand, true);
                                        }
                                        else if (target.Player.SuanniCommandCount == 5)
                                        {
                                            target.Player.AddSpellFlag(MsgUpdate.Flags.Commanding, (int)DBSpell.Duration, true);
                                            target.Player.SendUpdate(stream, MsgUpdate.Flags.Commanding, (uint)DBSpell.DamageOnHuman, (uint)DBSpell.DamageOnHuman * 6, (uint)user.Player.SuanniCommandCount, MsgUpdate.DataType.SuanniCommand, true);
                                        }

                                    }
                                }
                            }
                           
                            
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID, 0, 0, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                   
                  
                   
                }
            }
        }
   

    }
}