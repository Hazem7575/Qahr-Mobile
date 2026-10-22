// ============================================================================
//  [HazemTest 2026-10-15] Feasibility spike: a CUSTOM client window (not official) for future custom events.
//  Client files (removable): ini\luaui\fairygui\window\fgui_hazem_test\*.dat (3-Way encrypted Lua),
//  fgui\hazem_test.fui (generated package), ani\hazem_test.ani + data\interface\fgui\hazem_test\hz_logo.dds (new image).
//  The Lua module registers itself lazily, so the server first ui_requires it, then calls HazemTest_Open.
//  Button -> "</F>ClientManager_Callback</N>19990</S>HazemTest_Click</N>1" (msg 19990, unused by the client).
//  Hook: Base\LuaUi\LuaPush.cs HandleCallback (msg 19990), tagged.
// ============================================================================
using System;

namespace Qahr.Mobile.HazemTest
{
    public static class HazemTestUi
    {
        public const uint LuaMsg = 19990;
        public const string Module = "fairygui/window/fgui_hazem_test/fgui_hazem_testmain";

        /// <summary>Opens the custom window. pack/com = "hazem_test"/"main_hazem" (own .fui) or "luaui_common"/"com_msgtip".</summary>
        public static bool Open(Client.GameClient c, string title, string text, string button, string pack = "hazem_test", string com = "main_hazem")
        {
            if (c == null || c.Player == null) return false;
            LuaUi.LuaPush.Send(c, "StrToTable|ui_require(\"" + Module + "\")");
            return LuaUi.LuaPush.Send(c, "HazemTest_Open|{['Pack']=" + LuaUi.LuaPush.Q(pack) + ",['Com']=" + LuaUi.LuaPush.Q(com)
                + ",['MsgId']=" + LuaMsg + ",['Title']=" + LuaUi.LuaPush.Q(title) + ",['Text']=" + LuaUi.LuaPush.Q(text)
                + ",['Btn']=" + LuaUi.LuaPush.Q(button) + "}");
        }

        public static bool Close(Client.GameClient c)
        {
            return LuaUi.LuaPush.Send(c, "HazemTest_Close");
        }

        /// <summary>LuaPush.HandleCallback (msg 19990). True = answered.</summary>
        public static bool OnLua(Client.GameClient c, string func, string[] args)
        {
            if (c == null || c.Player == null || func != "HazemTest_Click") return false;
            string which = args != null && args.Length > 0 ? args[0] : "";
            try
            {
                c.SendSysMesage("[ScriptHazem] Custom window button " + which + " reached the server (msg 19990).",
                    Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.yellow);
            }
            catch { }
            if (which == "1") Close(c);
            return true;
        }
    }
}
