// ============================================================================
//  [CastleMystery 2026-10-14] Valentine 2025 "Castle Startle" (client tMsg_Info["DlgValentinesDay"] = 10139).
//  Shown while the festival CastleMystery runs: the official Fgui top-bar button FGUI_ActivityMenu['10139']
//  (fguibtnconfig, label 'Castle Startle', clickfunc Lover2025Kua__ClickButton) via festival.json icon_show/icon_hide.
//  The older lua button DlgValentinesDayBtn (ValentinesDay_Func_ShowBtn) sends the same click; it is not shown.
//
//  The 2025 main window (Castle Key draw, DlgValentinesDay_MainOpen Code 100 + a DlgID) has no 2025 data in this client
//  (Server_Key / valentinesdayinfo are from 2024: only DlgID 1329 exists, the 2023 New Year draw). So the button does
//  what the event is about on this server: it starts / resumes the Questinfo chain 8422-8429 in the private copy of the
//  official instance 1003099 (quests.json "SecretCastle"). Everything else of Lover2025Kua (draw, rank, stage rewards)
//  gets a system message.
//
//  Hook: Base\LuaUi\LuaPush.cs HandleCallback (msg 10139), one tagged line. Helper: Base\BandsTrouble\StoryEntry.cs.
// ============================================================================
using System;
using System.Collections.Concurrent;
using SE = Qahr.Mobile.FestivalStory.StoryEntry;

namespace Qahr.Mobile.CastleMystery
{
    public static class CastleMysteryUi
    {
        public const uint LuaMsg = 10139;
        public const string Festival = "CastleMystery";
        public const int MinLevel = 80;
        public static readonly uint[] Chain = { 8422, 8423, 8424, 8425, 8426, 8427, 8428, 8429 };

        public static bool Running { get { try { return Festivals.FestivalManager.IsRunning(Festival); } catch { return false; } } }

        static readonly ConcurrentDictionary<uint, long> _last = new ConcurrentDictionary<uint, long>();
        static bool Throttled(uint uid, int ms)
        {
            long now = Environment.TickCount & 0x7FFFFFFF, prev;
            if (_last.TryGetValue(uid, out prev) && now - prev >= 0 && now - prev < ms) return true;
            _last[uid] = now;
            return false;
        }

        /// <summary>True = answered (LuaPush.HandleCallback, msgId 10139).</summary>
        public static bool OnLua(Client.GameClient c, string func, string[] args)
        {
            if (c == null || c.Player == null || string.IsNullOrEmpty(func)) return false;
            if (!func.StartsWith("Lover2025Kua", StringComparison.Ordinal) && !func.StartsWith("StageTemplate_", StringComparison.Ordinal)
                && !func.StartsWith("ValentinesDay2025Story_", StringComparison.Ordinal)) return false;
            if (!Running) { SE.Msg(c, "Castle Startle has ended."); return true; }
            if (Throttled(c.Player.UID, 3000)) return true;
            if (func == "Lover2025Kua__ClickButton" || func == "ValentinesDay2025Story_EnterInsatnce")// [CastleMystery 2026-10-15] ItemGet list "enter the castle story"
            {
                SE.Enter(c, Chain, MinLevel, "Please return to Twin City before entering Auter Castle.", "You have uncovered the secrets of Auter Castle. Thank you!");
                return true;
            }
            if (func == "Lover2025Kua_EnterKuaLand") { SE.Msg(c, "Love Garden is a cross-server land, so it can`t be visited from this server."); return true; }
            if (func == "Lover2025Kua_EmoneyShop") { SE.Msg(c, "Castle Keys are sold by Suyi [CastleStartle] in Twin City (362,440). Castle treasures: use a Castle Key, or ask Suyi."); return true; }
            SE.Msg(c, "The Castle Key draw window and the Castle Startle ranking aren`t in this client. Use a Castle Key from your bag, or visit Suyi in Twin City (362,440).");
            return true;
        }
    }
}
