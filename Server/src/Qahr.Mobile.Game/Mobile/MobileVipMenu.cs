using System;
using System.Runtime.CompilerServices;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.Phone
{
    /// <summary>
    /// VIP services for mobile clients. The mobile 1.1.2.3 still has the PC VIP windows (CDlgVip, CDlgVipTransport,
    /// CDlgVipRepairAll, CDlgVipDepot), but nothing opens them: CDlgMain only constructs / destroys them (CDlgMain+0xcd20
    /// etc., no other reference in the library) and no Lua / fairygui window replaces them ("VIP" in the mobile UI is the
    /// battle pass). So a mobile player reaches the services through an NPC-style dialog opened with the chat command
    /// "@vip" (MobileVipMenu.ChatCommand, called from MobileEvents.ChatCommand):
    ///   City Teleport / Team Teleport -> MsgVipHandler.Teleport (+ the teammate confirmation 2204 type 2, which the
    ///   mobile shows), Repair All -> MobileVip.RepairAll, Warehouse -> MsgAction OpenDialog 4 (the mobile opens its
    ///   depot window for the last NPC it clicked); the MsgPackage requests that follow carry that NPC id, so
    ///   MsgWarehouse maps them to the chosen city warehouse (MapWarehouse).
    /// VIP level 1 is the free level every player keeps (PoolProcesses "Expire VIP" drops a lapsed VIP to 1);
    /// the paid services here need VIP 2+ that has not expired.
    /// </summary>
    public static class MobileVipMenu
    {
        public const uint MenuNpc = 9900210;// no NPC has this id; ActiveNpc routes the dialog answers here
        const int MinLevel = 2;

        sealed class State { public uint Warehouse; public DateTime Until; }
        static readonly ConditionalWeakTable<Client.GameClient, State> states = new ConditionalWeakTable<Client.GameClient, State>();

        static readonly string[] Cities = { "Twin City", "Phoenix Castle", "Ape City", "Desert City", "Bird Island" };
        static readonly MsgVipHandler.VIPTeleportLocations[] CityLocations =
        {
            MsgVipHandler.VIPTeleportLocations.TwinCity, MsgVipHandler.VIPTeleportLocations.PhoenixCastle,
            MsgVipHandler.VIPTeleportLocations.ApeCity, MsgVipHandler.VIPTeleportLocations.DesertCity,
            MsgVipHandler.VIPTeleportLocations.BirdIland,
        };
        static readonly string[] Warehouses = { "Twin City", "Phoenix Castle", "Ape City", "Desert City", "Bird Island", "Market" };
        static readonly Game.MsgNpc.NpcID[] WarehouseIds =
        {
            Game.MsgNpc.NpcID.WHTwin, Game.MsgNpc.NpcID.wHPheonix, Game.MsgNpc.NpcID.WHApe,
            Game.MsgNpc.NpcID.WHDesert, Game.MsgNpc.NpcID.WHBird, Game.MsgNpc.NpcID.WHMarket,
        };

        /// <summary>"@vip": true = handled.</summary>
        public static bool ChatCommand(Client.GameClient client, string cmd)
        {
            if (cmd == "vip" && MobileEvents.IsMobile(client)) return MobileVipUi.Open(client);// QahrMobile: the VIP window (MobileVipUi.cs); "@vipmenu" = this dialog
            if (cmd != "vipmenu" || !MobileEvents.IsMobile(client))
                return false;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                client.ActiveNpc = MenuNpc;
                Show(client, rec.GetStream(), 0);
            }
            return true;
        }

        // QahrMobile: used by the VIP window (MobileVipUi.cs)
        public const int MinLevelValue = MinLevel;
        public static string[] CityNames { get { return Cities; } }
        public static string[] WarehouseNames { get { return Warehouses; } }
        /// <summary>Runs one service of this menu: 3 repair all, 10+i city teleport, 20+i team teleport, 30+i warehouse.</summary>
        public static void Service(Client.GameClient client, ServerSockets.Packet stream, byte option) { Show(client, stream, option); }

        /// <summary>True when the VIP level is paid and not expired.</summary>
        public static bool HasVip(Client.GameClient client)
        {
            return client.Player.VipLevel >= MinLevel && client.Player.ExpireVip > DateTime.Now;
        }

        static string Status(Client.GameClient client)
        {
            if (HasVip(client))
                return "VIP " + client.Player.VipLevel + ", valid until " + client.Player.ExpireVip.ToString("yyyy-MM-dd HH:mm") + ".";
            return "You have VIP " + client.Player.VipLevel + " (free level). City teleport, repair all and the remote warehouse need VIP "
                + MinLevel + " or higher.";
        }

        /// <summary>Procesor.ExecuteNpc hook: the answers of the menu dialog. True = handled.</summary>
        public static bool OnNpc(Client.GameClient client, ServerSockets.Packet stream, uint npcid, byte option, string input)
        {
            if (npcid != MenuNpc)
                return false;
            try
            {
                Show(client, stream, option);
            }
            catch (Exception e)
            {
                try { MyConsole.SaveException(e); } catch { }
            }
            return true;
        }

        static void Show(Client.GameClient client, ServerSockets.Packet stream, byte option)
        {
            var d = new Game.MsgNpc.Dialog(client, stream);
            if (option == 0 || option == 5)// 0 = opened by @vip, 5 = "Back" (the server ignores dialog answers 0 and 255)
            {
                d.AddText("VIP services. " + Status(client));
                if (HasVip(client))
                {
                    d.AddOption("City teleport.", 1).AddOption("City teleport with my team.", 2)
                        .AddOption("Repair all my equipment.", 3).AddOption("Open a warehouse.", 4);
                }
                d.AddOption("Close.", 255).AddAvatar(30).FinalizeDialog();
                return;
            }
            if (!HasVip(client))
            {
                d.AddText(Status(client)).AddOption("Close.", 255).AddAvatar(30).FinalizeDialog();
                return;
            }
            if (option == 1 || option == 2)
            {
                d.AddText(option == 1 ? "Where do you want to go?" : "Where should your team go? Your teammates are asked to follow.");
                for (int i = 0; i < Cities.Length; i++)
                    d.AddOption(Cities[i] + ".", (byte)((option == 1 ? 10 : 20) + i));
                d.AddOption("Back.", 5).AddAvatar(30).FinalizeDialog();
                return;
            }
            if (option == 3)
            {
                MobileVip.RepairAll(client, stream);
                return;
            }
            if (option == 4)
            {
                d.AddText("Which warehouse do you want to open?");
                for (int i = 0; i < Warehouses.Length; i++)
                    d.AddOption(Warehouses[i] + ".", (byte)(30 + i));
                d.AddOption("Back.", 5).AddAvatar(30).FinalizeDialog();
                return;
            }
            if (option >= 10 && option < 10 + Cities.Length)
            {
                MsgVipHandler.Teleport(client, CityLocations[option - 10]);
                return;
            }
            if (option >= 20 && option < 20 + Cities.Length)
            {
                var loc = CityLocations[option - 20];
                if (client.Team != null)
                {
                    var p = stream.VipHandlerCreate(MsgVipHandler.VIPTeleportTypes.TeammateConfirmation, loc, 15, client.Player.Name);
                    foreach (var member in client.Team.Temates)
                        if (member.client.Player.UID != client.Player.UID)
                        {
                            MobileVipConfirm.Offer(member.client.Player.UID, loc, 15);// the teammate's answer must match this request
                            member.client.Send(p);
                        }
                }
                MsgVipHandler.Teleport(client, loc);
                return;
            }
            if (option >= 30 && option < 30 + Warehouses.Length)
            {
                if (client.Player.HitPoints == 0)
                    return;
                var st = states.GetValue(client, _ => new State());
                lock (st)
                {
                    st.Warehouse = (uint)WarehouseIds[option - 30];
                    st.Until = DateTime.Now.AddMinutes(10);
                }
                client.Send(stream.ActionCreate(new ActionQuery
                {
                    Type = ActionType.OpenDialog,
                    ObjId = client.Player.UID,
                    dwParam = DialogCommands.Warehouse,
                    NpcID = (uint)WarehouseIds[option - 30],// CMsgActionPB field 10
                    PositionX = client.Player.X,
                    PositionY = client.Player.Y
                }));
            }
        }

        /// <summary>MsgWarehouse hook: after "Open a warehouse" the mobile depot window sends the id of the last NPC the
        /// player clicked; such requests go to the chosen warehouse (for 10 minutes after the last use).</summary>
        public static uint MapWarehouse(Client.GameClient client, uint npcId)
        {
            State st;
            if (client == null || !states.TryGetValue(client, out st))
                return npcId;
            lock (st)
            {
                if (st.Warehouse == 0 || DateTime.Now > st.Until || npcId == client.Player.UID
                    || Role.Instance.Warehouse.IsWarehouse((Game.MsgNpc.NpcID)npcId) || !HasVip(client))
                    return npcId;
                st.Until = DateTime.Now.AddMinutes(10);
                return st.Warehouse;
            }
        }
    }
}
