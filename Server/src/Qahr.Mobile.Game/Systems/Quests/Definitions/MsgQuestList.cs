using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe void GetQuestList(this ServerSockets.Packet stream, out  MsgQuestList.QuestMode Mode, out ushort Count, out uint QuestID, out MsgQuestList.QuestListItem.QuestStatus QuestMode
            , out uint QuestTimer)
        {
            Mode = (MsgQuestList.QuestMode)stream.ReadUInt16();//4
            Count = stream.ReadUInt16();//6
            stream.ReadUInt64();//8
            stream.ReadUInt64();
            QuestID = stream.ReadUInt32();//24
            QuestMode = (MsgQuestList.QuestListItem.QuestStatus)stream.ReadUInt32();//28
            QuestTimer = stream.ReadUInt32();//32

        }
        public static unsafe ServerSockets.Packet QuestListCreate(this ServerSockets.Packet stream, MsgQuestList.QuestMode Mode, ushort Count)
        {
            stream.InitWriter();

            stream.Write((ushort)Mode);//4
            stream.Write(Count);//6
            stream.ZeroFill(16);//9
            return stream;
        }
        public static unsafe ServerSockets.Packet AddItemQuestList(this ServerSockets.Packet stream, MsgQuestList.QuestListItem item)
        {
            
            stream.Write(item.UID);//24
            stream.Write((uint)item.Status);//28
            stream.Write(item.Time);//32
            return stream;
        }
        public static unsafe ServerSockets.Packet QuestListFinalize(this ServerSockets.Packet stream)
        {
            stream.Finalize(GamePackets.MsgTaskStatus);
            return stream;
        }
    }



    public unsafe struct MsgQuestList
    {
        public class QuestListItem
        {
            public enum QuestStatus : uint
            {
                Accepted = 0,
                Finished = 1,
                Available = 2
            }
            public uint UID;
            public QuestStatus Status;
            public uint Time;
            public uint[] Intentions = new uint[1];
        }
        public enum QuestMode : ushort
        {
            AcceptQuest = 1,
            QuitQuest = 2,
            Review = 3,
            FinishQuest = 4,
            ShowBountyHall = 10,
            RefreshBountyHall = 11,
            AcceptBountyHall = 12,
            CancelBountyHall = 13,
            FinishBountyHall = 15,
        }


        /// <summary>
        /// Server-side validation for accepting a quest sent by the client. [Audit C1]
        /// Verifies class, level range, prerequisite quests, and - when the quest declares
        /// a begin NPC - that this NPC is actually on screen near the player. Lenient where
        /// a quest leaves a field unset (0) so legit quests are never blocked.
        /// </summary>
        private static bool CanClientAcceptQuest(Client.GameClient user, Database.QuestInfo.DBQuest quest)
        {
            if (quest == null)
                return false;

            // already accepted / finished -> let the existing Accept() no-op handle it
            if (user.Player.QuestGUI.src.ContainsKey(quest.MissionId))
                return true;

            // class restriction
            if (quest.Profession != null && !quest.CanAcceptQuest(user.Player.Class))
                return false;

            // level range (only enforced when the quest declares a bound)
            ushort level = user.Player.Level;
            if (quest.Lv_min > 0 && level < quest.Lv_min)
                return false;
            if (quest.Lv_max > 0 && level > quest.Lv_max)
                return false;

            // prerequisite quests must be finished
            // [QuestFix 2026-10-13] "a|b|c" = ANY one of them (client semantics, e.g. class alternatives
            // 650|655|656...); "a,b,c" / "a&b" = ALL of them. It used to require all of an "a|b|c" list.
            if (quest.Prequest != null && quest.Prequest.Length > 0)
            {
                bool anyDeclared = false, anyFinished = false;
                for (int i = 0; i < quest.Prequest.Length; i++)
                {
                    uint pre = quest.Prequest[i];
                    if (pre == 0)
                        continue;
                    anyDeclared = true;
                    bool finished = user.Player.QuestGUI.CheckQuest(pre, QuestListItem.QuestStatus.Finished);
                    if (finished)
                        anyFinished = true;
                    else if (!quest.PrequestAny)
                        return false;
                }
                if (quest.PrequestAny && anyDeclared && !anyFinished)
                    return false;
            }

            // the begin NPC must be in view range on the current map (anti remote-accept)
            if (quest.BeginNpcId != null && quest.BeginNpcId.ID != 0)
            {
                if (user.Map == null)
                    return false;
                Game.MsgNpc.Npc npc;
                if (!user.Map.SearchNpcInScreen(quest.BeginNpcId.ID, user.Player.X, user.Player.Y, user.Player.DynamicID, out npc))
                    return false;
            }

            return true;
        }

        [PacketAttribute(GamePackets.MsgTaskStatus)]
        private static void Process(Client.GameClient user, ServerSockets.Packet stream)
        {

            MsgQuestList.QuestMode Mode;
            ushort Count;
            uint QuestID; 
            MsgQuestList.QuestListItem.QuestStatus QuestaMode;
             uint QuestTimer;

             stream.GetQuestList(out Mode, out Count, out QuestID, out QuestaMode, out QuestTimer);

            switch (Mode)
            {
                case QuestMode.FinishBountyHall:
                    {
                        // [QuestFix 2026-10-13] was FinishQuest(QuestID) for ANY id with no checks (+600 EXP ball,
                        // farmable with cancel/accept). FinishBountyHall checks hall mission + hall-accepted +
                        // server-tracked objectives + daily cap.
                        // [QuestEngine] engine bounties: objectives re-checked + bag space before, the bounty prize after
                        // (FinishBountyHall is the once-only gate: Accepted -> Finished, daily cap).
                        if (!global::Qahr.Mobile.QuestEngine.QuestEngineCore.BeforeBountyFinish(user, QuestID))
                            break;
                        if (user.Player.QuestGUI.FinishBountyHall(QuestID))
                            global::Qahr.Mobile.QuestEngine.QuestEngineCore.AfterBountyFinish(user, QuestID);

                        break;
                    }
                case QuestMode.ShowBountyHall:
                    {
                        user.Player.QuestGUI.ShowAll(stream);

                        break;
                    }
                case QuestMode.RefreshBountyHall:
                    {
                        user.Player.QuestGUI.RefreshBountyHall(stream);
                        break;
                    }
                case QuestMode.AcceptBountyHall:
                    {
                        Database.QuestInfo.DBQuest quest;
                        if (Database.QuestInfo.AllQuests.TryGetValue(QuestID, out quest))
                        {
                            user.Player.QuestGUI.AcceptBountyHall(quest, QuestTimer);
                        }
                        break;
                    }
                case QuestMode.CancelBountyHall:
                    {
                        QuestListItem n_quest;
                        // [QuestFix 2026-10-13] only an accepted (unfinished) bounty can be cancelled; removing a
                        // finished one let the client accept/finish it again for another EXP ball.
                        if (Role.Instance.Quests.IsBountyHallMission(QuestID) && user.Player.QuestGUI.src.TryGetValue(QuestID, out n_quest)
                            && n_quest.Status == QuestListItem.QuestStatus.Accepted)
                        {
                            user.Player.QuestGUI.ForgetBounty(n_quest.UID);
                            user.Player.QuestGUI.SendSinglePacket(n_quest, Game.MsgServer.MsgQuestList.QuestMode.QuitQuest);
                            user.Player.QuestGUI.RemoveQuest(n_quest.UID);
                        }
                        
                        break;
                    }
                case QuestMode.AcceptQuest:
                    {
                        if (user.Player.QuestGUI.AllowAccept())
                        {
                            Database.QuestInfo.DBQuest quest;
                            if (Database.QuestInfo.AllQuests.TryGetValue(QuestID, out quest))
                            {
                                if (!CanClientAcceptQuest(user, quest))
                                    break;
                                // [QuestEngine] engine rules of engine-owned quests (clan, daily pool pick, bag space for handed items)
                                if (!global::Qahr.Mobile.QuestEngine.QuestEngineCore.AllowPanelAccept(user, quest.MissionId))
                                    break;

                                if (Database.QuestInfo.IsKingDomMission(quest.MissionId))
                                {
                                    QuestListItem _quest;
                                    user.Player.QuestGUI.AcceptKingDomMission(quest, 0, out _quest);
                                }
                                else
                                {
                                    user.Player.QuestGUI.Accept(quest, QuestTimer);
                                    global::Qahr.Mobile.QuestEngine.QuestEngineCore.AfterPanelAccept(user, quest.MissionId);// onAccept actions (teleport / items / boss)
                                }
                            }
                        }
                        break;
                    }
                case QuestMode.FinishQuest:
                    {
                        // [QuestEngine] engine quests only, every objective re-checked on the server and the finish
                        // NPC must be in view; hand-coded quests still finish only through their NPC scripts.
                        global::Qahr.Mobile.QuestEngine.QuestEngineCore.OnClientFinish(user, QuestID);

                        break;
                    }
                case QuestMode.QuitQuest:
                    {
                        QuestListItem n_quest;
                        // [QuestFix 2026-10-13] only an accepted (unfinished) quest can be quit; quitting a finished
                        // quest reset it so its rewards could be farmed again.
                        if (user.Player.QuestGUI.src.TryGetValue(QuestID, out n_quest) && n_quest.Status == QuestListItem.QuestStatus.Accepted)
                        {
                            user.Player.QuestGUI.ForgetBounty(n_quest.UID);
                            user.Player.QuestGUI.SendSinglePacket(n_quest, Game.MsgServer.MsgQuestList.QuestMode.QuitQuest);
                            user.Player.QuestGUI.RemoveQuest(n_quest.UID);
                        }
                        
                        break;
                    }
                case QuestMode.Review:
                    {
                        // QahrMobile: the mobile asks for the status of its own quest ids (80 per packet): answer those
                        var mobileIds = global::Qahr.Mobile.QuestEngine.MobileQuestStatus.ReadRequest(stream, Count);
                        if (mobileIds != null)
                        {
                            global::Qahr.Mobile.QuestEngine.MobileQuestStatus.Answer(user, stream, mobileIds);
                            break;
                        }
                        if (Count < 80)
                            user.Player.QuestGUI.SendFullGUI(stream);
                        break;
                    }
            }
        }
    }
}
