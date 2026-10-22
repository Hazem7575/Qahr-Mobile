using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler.CheckAttack
{
    public class CheckItems
    {

        public static void AttackDurability(Client.GameClient client, ServerSockets.Packet stream)
        {
            //if (client.Player.Rate(4))
            //{
            //bool dura_zero = false;
            //foreach (var item in client.Equipment.CurentEquip)
            //{
            //    if (item != null)
            //    {
            //        if (item.Position == (ushort)Role.Flags.ConquerItem.RightWeapon
            //            || item.Position == (ushort)Role.Flags.ConquerItem.LeftWeapon
            //            || item.Position == (ushort)Role.Flags.ConquerItem.AleternanteRightWeapon
            //            || item.Position == (ushort)Role.Flags.ConquerItem.AleternanteLeftWeapon
            //            || item.Position == (ushort)Role.Flags.ConquerItem.Ring
            //            || item.Position == (ushort)Role.Flags.ConquerItem.AleternanteRing
            //              || item.Position == (ushort)Role.Flags.ConquerItem.Fan
            //              || item.Position == (ushort)Role.Flags.ConquerItem.RidingCrop)
            //        {
            //            byte durability = (byte)Pool.GetRandom.Next(1, Math.Max(2, (int)(item.MaximDurability / 1000)));
            //            if (item.Durability > durability)
            //                item.Durability -= durability;
            //            else
            //            {
            //                item.Durability = 0;
            //                dura_zero = true;
            //            }
            //            item.Mode = Role.Flags.ItemMode.Update;

            //            item.Send(client, stream);
            //        }

            //    }
            //}
            //if (dura_zero)
            //    client.Equipment.QueryEquipment(client.Equipment.Alternante);
            //}
        }
        public static void RespouseDurability(Client.GameClient client)
        {
            //if (client.Player.Rate(4))
            //{
            //using (var rec = new ServerSockets.RecycledPacket())
            //{
            //    var stream = rec.GetStream();

            //    bool dura_zero = false;
            //    foreach (var item in client.Equipment.CurentEquip)
            //    {
            //        if (item != null)
            //        {
            //            if (item.Position == (ushort)Role.Flags.ConquerItem.Armor
            //                || item.Position == (ushort)Role.Flags.ConquerItem.AleternanteArmor
            //                || item.Position == (ushort)Role.Flags.ConquerItem.Necklace
            //                || item.Position == (ushort)Role.Flags.ConquerItem.AleternanteNecklace
            //                || item.Position == (ushort)Role.Flags.ConquerItem.Boots
            //                || item.Position == (ushort)Role.Flags.ConquerItem.AleternanteBoots
            //                 || item.Position == (ushort)Role.Flags.ConquerItem.Head
            //                 || item.Position == (ushort)Role.Flags.ConquerItem.AleternanteHead
            //                 || item.Position == (ushort)Role.Flags.ConquerItem.Tower)
            //            {
            //                byte durability = (byte)Pool.GetRandom.Next(1, Math.Max(2, (int)(item.MaximDurability / 1000)));
            //                if (item.Durability > durability)
            //                    item.Durability -= durability;
            //                else
            //                {
            //                    item.Durability = 0;
            //                    dura_zero = true;
            //                }
            //                item.Mode = Role.Flags.ItemMode.Update;


            //                item.Send(client, stream);
            //            }

            //        }
            //    }
            //    if (dura_zero)
            //        client.Equipment.QueryEquipment(client.Equipment.Alternante);
            //}
            //}
        }
    }
}
