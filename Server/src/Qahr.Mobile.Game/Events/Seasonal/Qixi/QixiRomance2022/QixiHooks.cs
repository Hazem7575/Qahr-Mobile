// ============================================================================
//  [QixiRomance2022 2026-10-15] QixiHooks: the two server hooks of the Qixi festival group (one line each):
//    Game\MsgNpc\Procesor.cs  ExecuteNpcInvoker.OnDequeue (first thing)  -> Hooks.OnNpc(...)   NPC + item-dialog replies
//    MY System\MsgItem\MsgItemUsuagePacket.cs  UseItem (after the DBItem lookup) -> Hooks.UseItem(...)  festival items/boxes
//  Each festival of the group answers only its own official NPC ids / item ids, and only while it runs (NPCs) .
// ============================================================================
using System;
using Qahr.Mobile.QixiRomance2022;

namespace Qahr.Mobile.QixiKit
{
    public static class Hooks
    {
        /// <summary>Test seam: is the NPC in the player's view and close (&lt;= 18 cells)?</summary>
        public static Func<Client.GameClient, uint, bool> NpcInView = (c, npc) =>
        {
            try
            {
                Game.MsgNpc.Npc obj;
                if (c.Map == null || !c.Map.SearchNpcInScreen(npc, c.Player.X, c.Player.Y, c.Player.DynamicID, out obj)) return false;
                return Math.Max(Math.Abs(c.Player.X - obj.X), Math.Abs(c.Player.Y - obj.Y)) <= 18;
            }
            catch { return false; }
        };

        /// <summary>Dialog text in chunks the client accepts.</summary>
        public static void Text(Game.MsgNpc.Dialog d, string text)
        {
            text = (text ?? "").Trim();
            while (text.Length > 0)
            {
                int n = Math.Min(text.Length, 200);
                if (n < text.Length) { int sp = text.LastIndexOf(' ', n - 1, n); if (sp > 100) n = sp + 1; }
                d.AddText(text.Substring(0, n));
                text = text.Substring(n);
            }
        }

        /// <summary>Procesor hook. True = answered (the NPC belongs to a running Qixi festival of this group, or it is our item dialog).</summary>
        public static bool OnNpc(Client.GameClient c, ServerSockets.Packet stream, uint npcid, byte option, string input)
        {
            if (c == null || c.Player == null) return false;
            try
            {
                // item dialogs (ActiveNpc = the item id)
                if (npcid == QixiRomance2022Ui.Silk || npcid == QixiRomance2022Ui.Blessing)
                {
                    if (c.ActiveNpc != npcid) return false;
                    if (option == 0 || option == 255 || Kit.Count(c, npcid) == 0) return true;
                    if (Kit.Throttled("qxdlg", c.Player.UID, 300)) return true;
                    QixiRomance2022Ui.OnItemDialog(c, stream, npcid, option);
                    return true;
                }
                if (QixiRomance2022Ui.OwnsNpc(npcid))
                {
                    if (!QixiRomance2022Ui.Running) return false;
                    if (option == 255) return true;
                    if (!NpcInView(c, npcid)) return true;
                    if (Kit.Throttled("qxnpc", c.Player.UID, 250)) return true;
                    QixiRomance2022Ui.OnNpc(c, stream, npcid, option);
                    return true;
                }
            }
            catch (Exception e) { MyConsole.SaveException(e); return true; }
            return false;
        }

        /// <summary>MsgItemUsuagePacket.UseItem hook. True = handled (the caller returns).</summary>
        public static bool UseItem(Client.GameClient c, Game.MsgServer.MsgGameItem item, ServerSockets.Packet stream)
        {
            if (c == null || c.Player == null || item == null) return false;
            try
            {
                if (QixiRomance2022Ui.OwnsItem(item.ITEM_ID))
                {
                    if (Kit.Throttled("qxuse", c.Player.UID, 300)) return true;
                    return QixiRomance2022Ui.UseItem(c, item, stream);
                }
            }
            catch (Exception e) { MyConsole.SaveException(e); return true; }
            return false;
        }
    }
}
