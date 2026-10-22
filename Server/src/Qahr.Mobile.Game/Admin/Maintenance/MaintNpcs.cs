using System;
using Qahr.Mobile.Game.MsgNpc;

namespace Qahr.Mobile.MaintNpc
{
    /// <summary>
    /// [MaintNpc 2026-10-15] Placed Twin City NPCs that used to answer "Under maintenance, come back later".
    /// Texts = client ini\Server_Key.lua (official). Their full systems are not on this server, so these are
    /// informative dialogs only: nothing is given or taken. Engine quests of these NPCs (QuestEngine) are shown
    /// first by Procesor; this script is the "Something else." / no-quest answer.
    ///  23492 UniversalBankManager  tOneDay_CyclicTaskLogic[23492] (New Hourly Hunting)
    ///  23746 DragonMessenger       tDragonSoulTask[23746] (Anima / Anima Trial)
    ///  30767 LeeJuechen            tTaoistNewAreaTask[30767] (Awakening Master)
    /// </summary>
    public static class MaintNpcs
    {
        const ushort Face = 10;

        [NpcAttribute((NpcID)23492)]
        public static void UniversalBankManager(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            try
            {
                var d = new Dialog(client, stream);
                d.AddText("*New Hourly Hunting*\n");
                d.AddText("Description: Complete specific hunting quests to earn fabulous rewards and luxurious mount armor.\n");
                d.AddOption("Okay.", 255).AddAvatar(Face).FinalizeDialog();
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
        }

        [NpcAttribute((NpcID)23746)]
        public static void DragonMessenger(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            try
            {
                var d = new Dialog(client, stream);
                switch (Option)
                {
                    case 1:// Option111 What're the effects of Anima? (Text121-128)
                        d.AddText("*Anima Effects*\n");
                        d.AddText("1. Once embedded on equipment, it`ll provides bonus attribute to the equipment and bonus BP to the user.\n");
                        d.AddText("2. The embedded Anima can be removed and you can sell it by right clicking the Anima in your inventory.\n");
                        d.AddText("3. Provides bonus BP equal to that shared from a full-BP Mentor.\n");
                        d.AddText("Anima Trial: Heroes challenging the Infernal Dragon in the Cursed Lair have a chance to receive Anima.\n");
                        d.AddText("The Anima Trial is not open on this server yet.");
                        d.AddOption("More details about bonus BP.", 2);
                        d.AddOption("I see.", 255).AddAvatar(Face).FinalizeDialog();
                        break;
                    case 2:// Option122 More details about bonus BP. (Text411-4110)
                        d.AddText("*Anima Effects*\n[Total Anima Phases] - [Bonus BP]\n");
                        d.AddText("1 - 10%\n11 - 15%\n21 - 20%\n31 - 25%\n41 - 30%\n61 - 35%\n71 - 50%\n");
                        d.AddOption("I see.", 255).AddAvatar(Face).FinalizeDialog();
                        break;
                    default:// Text111-114
                        d.AddText("Dragon Messenger:\n");
                        d.AddText("Animae are formed by spirits of all creatures and empowered by the universe to spawn advanced dragons. ");
                        d.AddText("As an invaluable treasure of the dragon race, Anima provides various magic effects.\n");
                        d.AddOption("What`re the effects of Anima?", 1);
                        d.AddOption("Okay.", 255).AddAvatar(Face).FinalizeDialog();
                        break;
                }
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
        }

        [NpcAttribute((NpcID)30767)]
        public static void LeeJuechen(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            try
            {
                var d = new Dialog(client, stream);
                d.AddText("If you have master a War Suit, I can guide you to unlock the War Suit of other classes to get stronger Power of Awakening!\n");
                d.AddText("The Awakening quests are not open on this server yet.");
                d.AddOption("Think twice.", 255).AddAvatar(Face).FinalizeDialog();
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
        }
    }
}
