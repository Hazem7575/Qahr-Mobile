// ============================================================================
//  [FguiGuide 2026-10-15] Shared helper: the client's official first-use guides (Fgui_Common_Guide window).
//  Client (luaui decrypted): fairygui/window/config/guideinfo.dat FguiGuideInfo[<Id>][<Key>] holds every step (portrait,
//  official Cn_Res text STR_FGUI_<Id>_<Key>, hand / highlight position, NextGuide chain, ClientFunc). The SERVER only
//  opens a step: g_function Public_Fgui_Guide_OpenView(sParam) with {Code=200,Data={Id=<guide id>,Key='<key>'}}.
//  Guide ids with steps in this client: 10052 10410 10510 10549 10553 10568 10611 10667 10728 10753 10789 10831 10840
//  10850 10900 10978 150273 150274 150279 150306 150344 150348 150349 150381 150382 150408 150416 150428 150440 150447.
//  Usage: FguiGuide.ShowOnce(client, 150348, "1_1", msgId, seen, mark) with the festival's own persisted state
//  (seen/mark callbacks), so each step shows once per player and continues where the player left off.
// ============================================================================
using System;

namespace Qahr.Mobile.Festivals
{
    public static class FguiGuide
    {
        public static string Call(uint guideId, string key, string exitFunc = null)
        {
            // ExitFunc: fgui_common_guidemediator onExit calls Public_CallServerFunc(Id, ExitFunc) when the guide closes
            // (Id doubles as the msg id: it only fits guides whose id is the festival's lua msg, e.g. 10768).
            return "Public_Fgui_Guide_OpenView|{['Code']=200,['Data']={['Id']=" + guideId + ",['Key']='" + (key ?? "").Replace("'", "") + "'"
                + (string.IsNullOrEmpty(exitFunc) ? "" : ",['ExitFunc']='" + exitFunc.Replace("'", "") + "'") + "}}";
        }
        public static void Show(Client.GameClient c, uint guideId, string key, uint msgId, string exitFunc)
        {
            try { LuaUi.LuaPush.Send(c, Call(guideId, key, exitFunc), msgId); } catch { }
        }
        /// <summary>Opens one guide step now.</summary>
        public static void Show(Client.GameClient c, uint guideId, string key, uint msgId)
        {
            try { LuaUi.LuaPush.Send(c, Call(guideId, key), msgId); } catch { }
        }
        /// <summary>Opens the step once: seen(key) false -> mark(key) (persist first) -> show. True = shown now.</summary>
        public static bool ShowOnce(Client.GameClient c, uint guideId, string key, uint msgId, Func<string, bool> seen, Action<string> mark)
        {
            if (c == null || c.Player == null || seen == null || mark == null) return false;
            string k = guideId + ":" + key;
            if (seen(k)) return false;
            mark(k);
            Show(c, guideId, key, msgId);
            return true;
        }
    }
}
