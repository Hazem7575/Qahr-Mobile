using System;
using System.Collections.Generic;
using System.IO;

namespace Qahr.Mobile.Game.MsgNpc
{
    // QahrMobile: fallback dialog for the quest NPCs of the official mobile client (its ini\questinfo.ini BeginNpcId /
    // FinishNpcId / [npc ...] ids) that were added to the server from Data\mobile_quests\quest_npcs*.txt.
    //
    // The quest engine answers first (QuestEngineCore.OnNpc: accept / progress / finish of the quests this NPC begins or
    // finishes). Only when it has nothing for the player does this script show the NPC's own text, taken from the mobile
    // questinfo.ini (Content of a quest it begins, else the IntentionDesp of a quest it finishes), so a click on a quest
    // NPC is never silent.
    //
    // Data: Data\mobile_quests\quest_npcs_dialog.txt, "npcId<TAB>name<TAB>text" ('#' = comment). Reloaded when the file
    // changes (checked on click). Ids with a hand-written [NpcAttribute] script are not in the file.
    //
    // Routing: Procesor.ExecuteNpcInvoker.OnDequeue calls MobileQuestNpc(...) right after MobileVideoNpc(...), before the
    // NpcAttribute lookup, so this works under Hot Reload without a restart.
    public unsafe partial class NpcHandler
    {
        sealed class MobileQuestNpcText
        {
            public string Name;
            public string Text;
        }

        static readonly object mobileQuestNpcLock = new object();
        static Dictionary<uint, MobileQuestNpcText> mobileQuestNpcs = new Dictionary<uint, MobileQuestNpcText>();
        static DateTime mobileQuestNpcsStamp = DateTime.MinValue;
        static DateTime mobileQuestNpcsChecked = DateTime.MinValue;

        static string MobileQuestNpcsPath
        {
            get { return Path.Combine(Program.ServerConfig.DbLocation ?? "", "mobile_quests", "quest_npcs_dialog.txt"); }
        }

        static Dictionary<uint, MobileQuestNpcText> MobileQuestNpcTable()
        {
            lock (mobileQuestNpcLock)
            {
                if ((DateTime.UtcNow - mobileQuestNpcsChecked).TotalSeconds < 2)
                    return mobileQuestNpcs;
                mobileQuestNpcsChecked = DateTime.UtcNow;
                try
                {
                    string path = MobileQuestNpcsPath;
                    if (!File.Exists(path))
                    {
                        mobileQuestNpcs = new Dictionary<uint, MobileQuestNpcText>();
                        mobileQuestNpcsStamp = DateTime.MinValue;
                        return mobileQuestNpcs;
                    }
                    DateTime stamp = File.GetLastWriteTimeUtc(path);
                    if (stamp == mobileQuestNpcsStamp)
                        return mobileQuestNpcs;
                    var map = new Dictionary<uint, MobileQuestNpcText>();
                    foreach (string raw in File.ReadAllLines(path))
                    {
                        if (raw.Length == 0 || raw[0] == '#') continue;
                        string[] p = raw.Split('\t');
                        uint id;
                        if (p.Length < 3 || !uint.TryParse(p[0].Trim(), out id)) continue;
                        map[id] = new MobileQuestNpcText { Name = p[1].Trim(), Text = p[2].Trim() };
                    }
                    mobileQuestNpcs = map;
                    mobileQuestNpcsStamp = stamp;
                    MyConsole.WriteLine("[Mobile] quest NPC dialogs loaded: " + map.Count);
                }
                catch (Exception e)
                {
                    MyConsole.WriteLine("[Mobile] " + MobileQuestNpcsPath + ": " + e.Message, ConsoleColor.Red);
                }
                return mobileQuestNpcs;
            }
        }

        /// <summary>Returns true when the npc id is a mobile quest NPC from quest_npcs_dialog.txt and its dialog was shown.</summary>
        public static bool MobileQuestNpc(Client.GameClient client, ServerSockets.Packet stream, uint npcid, byte option, string input)
        {
            MobileQuestNpcText npc;
            if (!MobileQuestNpcTable().TryGetValue(npcid, out npc))
                return false;
            if (option != 0)
                return true; // "Goodbye." (255) or a stale option: nothing to do
            var data = new Dialog(client, stream);
            string text = npc.Text ?? "";
            while (text.Length > 0)
            {
                int n = Math.Min(text.Length, 200);
                if (n < text.Length)
                {
                    int sp = text.LastIndexOf(' ', n - 1, n);
                    if (sp > 100) n = sp + 1;
                }
                data.AddText(text.Substring(0, n));
                text = text.Substring(n);
            }
            data.AddOption("Goodbye.", 255).AddAvatar(10).FinalizeDialog();
            return true;
        }
    }
}
