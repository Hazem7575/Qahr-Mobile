using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{
    public class Venom
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                switch (ClientSpell.ID)
                {
                    case (ushort)20000:
                        {
                            if (user.Status.VenomLevel > 0)
                            {
                                user.Send(stream.InteractionCreate(Attack));
                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                                Role.IMapObj target;
                                if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                                {
                                    var attacked = target as Role.Player;
                                    if (Role.Core.GetDistance(user.Player.X, user.Player.Y, target.X, target.Y) < DBSpell.Range)
                                    {

                                        if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                                        {
                                            if (!attacked.ContainFlag(MsgUpdate.Flags.VenomMyth))
                                            {
                                                attacked.VenomDamage = DBSpell.DamageOnHuman;
                                                attacked.AddFlag(MsgUpdate.Flags.VenomMyth, (int)6, true, 1);
                                                MsgUpdate upd = new MsgUpdate(stream, attacked.UID, 1);
                                                upd.Append(stream, MsgUpdate.DataType.ArchiveSkill, (uint)MsgUpdate.Flags.VenomMyth, (uint)6, (uint)DBSpell.DamageOnHuman, user.Status.VenomLevel);
                                                attacked.Send(upd.GetArray(stream));
                                            }
                                        }
                                    }
                                }
                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                                MsgSpell.SetStream(stream);
                                MsgSpell.Send(user);
                            }
                            break;
                        }
                }
            }
        }
    }
}
