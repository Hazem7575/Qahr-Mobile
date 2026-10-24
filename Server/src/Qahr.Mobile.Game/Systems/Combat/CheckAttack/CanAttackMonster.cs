using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler.CheckAttack
{
    public class CanAttackMonster
    {
        public static bool Verified(Client.GameClient client, MsgMonster.MonsterRole attacked
            , Database.MagicType.Magic DBSpell)
        {
            if ((attacked.Family.Settings & MsgMonster.MonsterSettings.Reviver) == MsgMonster.MonsterSettings.Reviver)
                return false;

            if ((attacked.Family.Settings & MsgMonster.MonsterSettings.Lottus) == MsgMonster.MonsterSettings.Lottus && attacked.OwnerFloor != null && client.Player.UID == attacked.OwnerFloor.Player.UID)
                return false;

            if (!attacked.Alive)
                return false;
            if (attacked.Family.ID == 5385)
            {
                return false;
            }
            if ((attacked.Family.Settings & MsgMonster.MonsterSettings.Guard) == MsgMonster.MonsterSettings.Guard)
            {
                if (client.Player.PkMode != Role.Flags.PKMode.PK)
                    return false;
                else
                {
                    client.Player.AddFlag(MsgUpdate.Flags.FlashingName, 30, true);
                }
            }

            if (attacked.Name.Contains("[GM]") || attacked.Name.Contains("[PM]"))
            {
                return false;
            }
            
            return true;
        }
    }
}
