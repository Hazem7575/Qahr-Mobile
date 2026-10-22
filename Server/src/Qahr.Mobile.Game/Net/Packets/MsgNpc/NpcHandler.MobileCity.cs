using System;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.Game.MsgNpc
{
    // QahrMobile: city content that the official mobile client places in the main cities but the server lacked
    // (see GitHub\docs\CITY_CONTENT.md):
    //   - ExtremePKManager (uid 4699, mobile npcx 6420) at Twin City 443,246: mapdestination.ini [1002-1-8]
    //     "Extreme PK Tournament". The old PC handler ExtremePk (NpcHandler.10.MysterCheast3.cs) is reused.
    //   - Slot machines (NPC type 60, ids from ini\slotnpc.ini): Twin City 15459-15462, Market 9826-9828 (+ 9817).
    //     The mobile opens the slot window by itself for these ids; the spin is MsgMachine.cs (cost from the mesh).
    //
    // Routing: Procesor.ExecuteNpcInvoker.OnDequeue calls MobileCityNpc(...) before the NpcAttribute lookup, so these
    // scripts work without a restart (the NpcAttribute registry is built once at server start).
    public static class MobileCityNpcIds
    {
        public const uint
            ExtremePkManager = 4699,
            TcSilverSlot = 15459,
            TcCpSlot1 = 15460,
            TcCpSlot2 = 15461,
            TcCpSlot3 = 15462,
            MarketSilverSlot = 9817,
            MarketCpSlot1 = 9826,
            MarketCpSlot2 = 9827,
            MarketCpSlot3 = 9828;
    }

    public unsafe partial class NpcHandler
    {
        /// <summary>Returns true when the npc id is one of the mobile city NPCs and its script ran.</summary>
        public static bool MobileCityNpc(Client.GameClient client, ServerSockets.Packet stream, uint npcid, byte option, string input)
        {
            switch (npcid)
            {
                case MobileCityNpcIds.ExtremePkManager:
                    if (client.Player.Map != 1002) return false;
                    ExtremePk(client, stream, option, input, npcid);
                    return true;
                case MobileCityNpcIds.TcSilverSlot:
                case MobileCityNpcIds.TcCpSlot1:
                case MobileCityNpcIds.TcCpSlot2:
                case MobileCityNpcIds.TcCpSlot3:
                case MobileCityNpcIds.MarketSilverSlot:
                case MobileCityNpcIds.MarketCpSlot1:
                case MobileCityNpcIds.MarketCpSlot2:
                case MobileCityNpcIds.MarketCpSlot3:
                    // The slot window is opened by the mobile client itself; a talk to the machine needs no dialog.
                    return true;
                case 9600040: ServicesHallGuide(client, stream, option); return true;
                case 9600041: ServicesHallReturn(client, stream, option); return true;
            }
            return false;
        }

        // Services hall (map 11412, the house03 garden): the PC NPCs that the official mobile Twin City does not have
        // (rebirth, marriage, guild, house, name change, PC quests...) stand there, so Twin City looks like the official
        // one. 9600040 stands in Twin City and sends players in, 9600041 in the hall sends them back.
        public static void ServicesHallGuide(Client.GameClient client, ServerSockets.Packet stream, byte option)
        {
            Dialog data = new Dialog(client, stream);
            if (option == 0)
            {
                data.AddText("The masters of rebirth, marriage, guilds, houses, names and many old quests now wait in the Services Hall.")
                    .AddText(" Shall I take you there?")
                    .AddOption("Take me to the Services Hall.", 1)
                    .AddOption("Not now.", 255)
                    .AddAvatar(30).FinalizeDialog();
            }
            else if (option == 1)
                client.Teleport(48, 51, 11412);
        }

        public static void ServicesHallReturn(Client.GameClient client, ServerSockets.Packet stream, byte option)
        {
            Dialog data = new Dialog(client, stream);
            if (option == 0)
            {
                data.AddText("Welcome to the Services Hall. When you are done, I can send you back to Twin City.")
                    .AddOption("Back to Twin City.", 1)
                    .AddOption("I will stay a while.", 255)
                    .AddAvatar(30).FinalizeDialog();
            }
            else if (option == 1)
                client.Teleport(430, 374, 1002);
        }
    }
}
