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
    public class ChaoticDance
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                switch (ClientSpell.ID)
                {
                    case (ushort)Role.Flags.SpellID.ChaoticDance:
                        {
                            user.Player.ChaoticDance = 0;
                            if (!user.Player.ContainFlag((MsgUpdate.Flags)DBSpell.Status))
                            {
                                user.Send(stream.InteractionCreate(Attack));
                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID, 0, 0, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                                MsgSpell.SetStream(stream);
                                MsgSpell.Send(user);

                                user.Player.AddSpellFlag((MsgUpdate.Flags)DBSpell.Status, (int)DBSpell.Second, true);
                                user.Player.SendUpdate(stream, (MsgUpdate.Flags)DBSpell.Status, (uint)DBSpell.Second, (int)DBSpell.DamageOnHuman / 1000, (int)DBSpell.DamageOnHuman % 100, (long)DBSpell.Damage2, (long)DBSpell.Damage3, (MsgUpdate.DataType)146, true);
                                user.Player.ActiveChaoticDance = true;
                                user.Player.IncreasingHP += (uint)DBSpell.DamageOnHuman / 1000;
                                user.Player.IncreaseasStaminaPercent += (uint)(DBSpell.DamageOnHuman % 1000);
                                user.Player.RestoringHP += (uint)DBSpell.Damage2;
                                user.Player.RestoringHPStamp = System.DateTime.Now;
                                user.Player.RestoringStaminaStamp = System.DateTime.Now;
                                byte MaxStamina = (byte)(user.Player.HeavenBlessing > 0 ? 150 : 100);
                                // user.UpdateStamina(stream, (uint)MaxStamina);
                                user.Equipment.QueryEquipment(user.Equipment.Alternante);
                                InteractQuery ChaoticDance = new InteractQuery
                                {
                                    UID = user.Player.UID,
                                    X = Attack.X,
                                    Y = Attack.Y,
                                    SpellID = (ushort)Role.Flags.SpellID.ChaoticDanceAttack,
                                    AtkType = (ushort)MsgAttackPacket.AttackID.Magic,
                                };
                                user.Player.RandomSpell = (ushort)Role.Flags.SpellID.ChaoticDanceAttack;
                                Game.MsgServer.MsgAttackPacket.ProcescMagic(user, stream, ChaoticDance, true);
                            }
                            break;
                        }
                }
            }
        }
    }
}
