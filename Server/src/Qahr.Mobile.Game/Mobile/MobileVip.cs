using System;

namespace Qahr.Mobile.Phone
{
    /// <summary>
    /// VIP panel functions of the mobile client that have no handler on the server (see docs\VIP_REBORN.md).
    /// The packet formats are in Data\mobile_layouts\vip.txt.
    /// </summary>
    public static unsafe class MobileVip
    {
        /// <summary>CDlgVipRepairAll::OnBtnRepair sends CMsgItem::Create(heroUid, 0, 15) (MsgItem action 15 =
        /// repair every worn item that is not broken). The price is the one of MsgItem RepairItem, summed.</summary>
        public static void RepairAll(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (client.IsConnectedInterServer())
                return;
            if (client.Player.VipLevel == 0)
            {
                client.SendSysMesage("Repair All is a VIP service.", Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.red);
                return;
            }
            ulong total = 0;
            var items = client.Equipment.CurentEquip;
            foreach (var item in items)
            {
                if (item == null || item.Durability == 0 || item.Durability >= item.MaximDurability)
                    continue;
                total += RepairCost(item);
            }
            if (total == 0)
            {
                client.SendSysMesage("Your equipment does not need repairs.", Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.yellow);// QahrMobile: answer the VIP menu
                return;
            }
            if (client.Player.Money < total)
            {
                client.SendSysMesage("You need " + total + " silver to repair all your equipment.", Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.red);
                return;
            }
            client.Player.Money -= total;
            client.Player.SendUpdate(stream, client.Player.Money, Game.MsgServer.MsgUpdate.DataType.Money);
            foreach (var item in items)
            {
                if (item == null || item.Durability == 0 || item.Durability >= item.MaximDurability)
                    continue;
                item.Durability = item.MaximDurability;
                item.Mode = Role.Flags.ItemMode.Update;
                item.Send(client, stream);
            }
        }

        static ulong RepairCost(Game.MsgServer.MsgGameItem item)
        {
            Database.ItemType.DBItem db;
            uint price = Pool.ItemsBase.TryGetValue(item.ITEM_ID, out db) ? db.GoldWorth : 0;
            double quality;
            switch (item.ITEM_ID % 10)
            {
                case 9: quality = 1.125; break;
                case 8: quality = 0.975; break;
                case 7: quality = 0.9; break;
                case 6: quality = 0.825; break;
                default: quality = 0.75; break;
            }
            ulong cost = 0;
            if (price > 0)
                cost = (ulong)Math.Ceiling((price * (item.MaximDurability - item.Durability) / item.MaximDurability) * quality);
            return Math.Max(1, cost);
        }

        /// <summary>The mobile city-teleport list (viptrans.ini) has three points the PC list has not:
        /// WindPlain-Farm (7), WindPlain-Bridge (8), Desert-Village (32); vip.txt sends them as 32, 33, 34.</summary>
        public static void UnknownTeleport(Client.GameClient user, uint location)
        {
            user.SendSysMesage("This teleport point is not available.", Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.red);
        }
    }
}
