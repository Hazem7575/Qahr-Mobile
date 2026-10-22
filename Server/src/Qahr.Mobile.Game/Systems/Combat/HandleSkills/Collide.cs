using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{
    public class Collide
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack,user, DBSpells, out ClientSpell, out DBSpell))
            {
                Role.Flags.ConquerAngle direction = (Role.Flags.ConquerAngle)Attack.OpponentUID;

                Attack.AtkType = (ushort)MsgAttackPacket.AttackID.Dash;
                Attack.SpellID = ClientSpell.ID;
                Attack.SpellLevel = ClientSpell.Level;

                ushort awalkX = user.Player.X, awalkY = user.Player.Y;
                Role.Core.IncXY(direction, ref awalkX, ref awalkY);
                user.Map.View.MoveTo<Role.IMapObj>(user.Player, awalkX, awalkY);
                user.Player.X = awalkX;
                user.Player.Y = awalkY;
                user.Player.View.Role(false,null);



                Role.IMapObj target;
                if (user.Player.View.SameLocation(Role.MapObjectType.Player, out target))
                {
                    Role.Player attacked = target as Role.Player;

                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell, true, Attack))
                    {
                        MsgSpellAnimation.SpellObj AnimationObj;
                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                       
                        Attack.Damage = (int)AnimationObj.Damage;
                        Attack.Effect = (uint)AnimationObj.Effect;
                        ushort walkX = attacked.X, walkY = attacked.Y;
                        Role.Core.IncXY(direction, ref walkX, ref walkY);
                        user.Map.View.MoveTo<Role.IMapObj>(attacked, walkX, walkY);
                        attacked.X = walkX;
                        attacked.Y = walkY;
                        attacked.View.Role(false,null);
                        Attack.OpponentUID = attacked.UID;
                        user.Player.View.SendView(stream.InteractionCreate(Attack), true);
                    }
                }
                else if (user.Player.View.SameLocation(Role.MapObjectType.Monster, out target))
                {
                    Game.MsgMonster.MonsterRole attacked = target as Game.MsgMonster.MonsterRole;
                    if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                    {
                        ushort walkX = attacked.X, walkY = attacked.Y;
                        Role.Core.IncXY(direction, ref walkX, ref walkY);
                        user.Map.View.MoveTo<Role.IMapObj>(attacked, walkX, walkY);
                        attacked.X = walkX;
                        attacked.Y = walkY;

                        MsgSpellAnimation.SpellObj AnimationObj;
                        Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                        ReceiveAttack.Monster.Execute(stream,AnimationObj, user, attacked);
                       
                        Attack.Damage = (int)AnimationObj.Damage;
                        Attack.Effect = (uint)AnimationObj.Effect;

                        Attack.OpponentUID = attacked.UID;
                        user.Player.View.SendView(stream.InteractionCreate(Attack), true);
                      
                        return;
                    }
                }

                
            }
        }
    }
}
