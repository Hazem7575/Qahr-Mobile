using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Qahr.Mobile.Database
{
    public class RoleQuests
    {
        public unsafe static void Save(Client.GameClient user)
        {
            string path = Program.ServerConfig.DbLocation + "\\Quests\\" + user.Player.UID + ".bin";
            string tmp = path + ".tmp";
            WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
            try
            {
                // snapshot الأول (الـ Dictionary مش thread-safe) - لو اتعدّلت أثناء النسخ نعيد المحاولة
                Game.MsgServer.MsgQuestList.QuestListItem[] quests = null;
                for (int attempt = 0; attempt < 3 && quests == null; attempt++)
                {
                    try { quests = user.Player.QuestGUI.src.Values.ToArray(); }
                    catch (InvalidOperationException) { }
                }
                if (quests == null)
                {
                    MyConsole.WriteLine("[RoleQuests.Save] snapshot failed for " + user.Player.UID + " - kept old file");
                    return;
                }
                // نكتب على ملف tmp وبعدين نستبدل بيه الأصلي (لو حصل exception الأصلي يفضل سليم)
                bool written = false;
                long expected = 4;// حجم الملف المتوقع للتأكد إن الكتابة كاملة
               if (binary.Open(tmp, FileMode.Create))
               {
                   int AmountQuests = quests.Length;
                   binary.Write(&AmountQuests, sizeof(int));
                   foreach (var quest in quests)
                   {
                       uint UID = quest.UID;
                       uint Status = (uint)quest.Status;
                       uint Time = quest.Time;
                       

                       int size = quest.Intentions.Length;
                       binary.Write(&UID, sizeof(uint));
                       binary.Write(&Status, sizeof(uint));
                       binary.Write(&Time, sizeof(uint));
                       binary.Write(&size, sizeof(int));
                       for (int x = 0; x < size; x++)
                       {
                           uint Intention = quest.Intentions[x];
                           binary.Write(&Intention, sizeof(uint));
                       }

                       expected += 16 + 4L * size;
                   }
                   binary.Close();
                   written = true;
               }
                // نستبدل الأصلي بس لو الـ tmp اتكتب كامل
                if (written && new FileInfo(tmp).Length == expected)
                {
                    if (File.Exists(path))
                        File.Replace(tmp, path, null);
                    else
                        File.Move(tmp, path);
                    global::Qahr.Mobile.Database.Storage.PlayerBinaryStorage.OnBlobSaved(path); // [DB-only] keep the DB blob fresh (load reads it)
                }
                else if (written)
                    MyConsole.WriteLine("[RoleQuests.Save] incomplete write for " + user.Player.UID + " - kept old file");
            }
            catch (Exception e)
            {
                MyConsole.WriteLine("[RoleQuests.Save ERROR] " + user.Player.UID + " " + e);
            }
            finally
            {
                binary.Close();
                try { if (File.Exists(tmp)) File.Delete(tmp); }
                catch { }
            }
        }
        public unsafe static void Load(Client.GameClient user)
        {
            WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
            if (binary.Open(Program.ServerConfig.DbLocation + "\\Quests\\" + user.Player.UID + ".bin", FileMode.Open))
            try // finally بيقفل الملف دايمًا (قبل كده كان بيفضل مفتوح)
            {
                int AmountQuests = user.Player.QuestGUI.src.Count;
                binary.Read(&AmountQuests, sizeof(int));
                for (int x = 0; x < AmountQuests; x++)
                {
                    
                    uint _UID;
                    uint _Status;
                    uint _Time;
                    int Intentions;
                    uint[] QIntentions;
                    binary.Read(&_UID, sizeof(uint));
                    binary.Read(&_Status, sizeof(uint));
                    binary.Read(&_Time, sizeof(uint));
                    binary.Read(&Intentions, sizeof(int));
                    QIntentions = new uint[Intentions];
                    for (int i = 0; i < Intentions; i++)
                    {
                        uint QIntention;
                        binary.Read(&QIntention, sizeof(uint));
                        QIntentions[i] = QIntention;
                    }
                  
                    var quest = new Game.MsgServer.MsgQuestList.QuestListItem()
                    {
                        UID = _UID,
                        Status = (Game.MsgServer.MsgQuestList.QuestListItem.QuestStatus)_Status,
                        Time = _Time,
                        Intentions = QIntentions
                    };
                   
                    if (!user.Player.QuestGUI.src.ContainsKey(quest.UID))
                        user.Player.QuestGUI.src.Add(quest.UID, quest);
                    if (quest.Status == Game.MsgServer.MsgQuestList.QuestListItem.QuestStatus.Accepted)
                        user.Player.QuestGUI.AcceptedQuests.Add(quest.UID, quest);
                }
            }
            finally
            {
                binary.Close();
            }
        }
    }
}
