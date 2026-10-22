using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{
    public class EffectMP
    {
        public unsafe static void Execute(InteractQuery Attack, Client.GameClient user,ServerSockets.Packet stream,  Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
    
            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                , 0, user.Player.X, user.Player.Y, (ushort)Role.Flags.SpellID.EffectMP
                , 0, 0);


            uint Damage = Calculate.Base.CalculateHealtDmg(300, user.Status.MaxMana, (uint)(user.Player.Mana));
            user.Player.Mana += (ushort)Damage;
            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, Damage, MsgAttackPacket.AttackEffect.None));

            MsgSpell.SetStream(stream);
            MsgSpell.Send(user);

        }

    }
}
