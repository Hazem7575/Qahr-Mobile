using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{
    public class WarCry
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack,user, DBSpells, out ClientSpell, out DBSpell))
            {
                Attack.SpellID = (ushort)Role.Flags.SpellID.WarCry;

                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                    , 0, Attack.X, Attack.Y, ClientSpell.ID
                    , ClientSpell.Level, ClientSpell.UseSpellSoul);
                Algoritms.Sector SpellSector = new Algoritms.Sector(user.Player.X, user.Player.Y, Attack.X, Attack.Y);
                SpellSector.Arrange(DBSpell.Sector, DBSpell.Range);

                foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                {
                    var attacked = targer as Role.Player;

                    if (!attacked.ContainFlag(MsgUpdate.Flags.Ride))
                        continue;

                    bool CanAttack = user.Equipment.SteedPlus > attacked.Owner.Equipment.SteedPlus;
                    if (user.Equipment.SteedPlus == attacked.Owner.Equipment.SteedPlus)
                        CanAttack = user.Equipment.SteedPlusPorgres > attacked.Owner.Equipment.SteedPlusPorgres;

                    if (CanAttack)
                    {
                        attacked.RemoveFlag(MsgUpdate.Flags.Ride);
                      //  user.Player.Stamina = (ushort)Math.Min((int)user.Player.Stamina, 0);
                      //  user.Player.SendUpdate(stream, user.Player.Stamina, Game.MsgServer.MsgUpdate.DataType.Stamina);
                        MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj() { UID = attacked.UID, Hit =1 });

                        Attack.OpponentUID = attacked.UID;
                        Attack.X = attacked.X;
                        Attack.Y = attacked.Y;
                        user.Player.View.SendView(stream.InteractionCreate(Attack), true);
                    }

                }

                MsgSpell.SetStream(stream);
                MsgSpell.Send(user);
            }
        }
    }
}
