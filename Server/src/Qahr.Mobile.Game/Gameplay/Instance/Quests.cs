using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.Role.Instance
{
    //questid = 10 <- An~accessory in bird 
    //questid = 11 <- DesertBloodExp in desert
    public class Quests
    {



        public Role.Player Player;
        public Dictionary<uint, Game.MsgServer.MsgQuestList.QuestListItem> src;
        public Dictionary<uint, Game.MsgServer.MsgQuestList.QuestListItem> AcceptedQuests;

        public Quests(Role.Player _owner)
        {
            Player = _owner;
            AcceptedQuests = new Dictionary<uint, MsgQuestList.QuestListItem>();
            src = new Dictionary<uint, Game.MsgServer.MsgQuestList.QuestListItem>();

        }

        // ===== [QuestFix 2026-10-13] Bounty Hall =====
        // Before: the client could finish ANY quest with FinishBountyHall (no checks), then cancel the
        // finished quest and accept/finish it again, each time getting the 600 EXP ball (SendSinglePacket)
        // -> unlimited EXP, and it could also skip quests needed as prerequisites.
        // Now: a bounty can only be finished if (1) it is a Bounty Hall mission (>= 590000), (2) it was
        // offered by the hall and accepted from the hall in this session, (3) the SERVER marked its
        // objectives done (MarkBountyObjectivesDone - no tracker calls it yet, so nothing can be finished
        // until real tracking exists), (4) the player is under the daily cap. Finished quests can't be
        // cancelled/quit any more, so a finished bounty stays finished until the daily reset.
        public const uint BountyHallFirstMission = 590000;
        public const byte BountyHallDailyLimit = 10;
        public const int BountyHallOffers = 3;
        public static bool IsBountyHallMission(uint UID)
        {
            return UID >= BountyHallFirstMission;
        }
        private readonly HashSet<uint> BountyHallOffered = new HashSet<uint>();
        private readonly HashSet<uint> BountyHallAccepted = new HashSet<uint>();
        private readonly HashSet<uint> BountyHallObjectivesDone = new HashSet<uint>();

        /// <summary>Server-side tracking calls this when a hall bounty's objectives are really complete.</summary>
        public void MarkBountyObjectivesDone(uint UID)
        {
            if (BountyHallAccepted.Contains(UID) && IsActiveQuest(UID))
                BountyHallObjectivesDone.Add(UID);
        }
        public bool FinishBountyHall(uint UID)
        {
            if (!IsBountyHallMission(UID))
                return false;
            if (!BountyHallAccepted.Contains(UID) || !IsActiveQuest(UID))
                return false;
            if (!BountyHallObjectivesDone.Contains(UID))
                return false;
            if (Player.BountyHallToday >= BountyHallDailyLimit)
                return false;
            if (!FinishQuest(UID))// sends FinishQuest (+ the EXP ball) exactly once: Accepted -> Finished
                return false;
            Player.BountyHallToday++;
            global::Qahr.Mobile.BattlePass.BattlePassManager.AddProgress(Player.Owner, 128);// [BattlePass] task type 128 "Join in Bounty Hall"
            BountyHallAccepted.Remove(UID);
            BountyHallObjectivesDone.Remove(UID);
            return true;
        }
        public void ForgetBounty(uint UID)
        {
            BountyHallAccepted.Remove(UID);
            BountyHallObjectivesDone.Remove(UID);
        }
        /// <summary>Daily reset: clears the cap and makes the finished bounties available again.</summary>
        public void ResetBountyHallDay()
        {
            Player.BountyHallToday = 0;
            BountyHallOffered.Clear();
            var finished = src.Values.Where(p => IsBountyHallMission(p.UID) && p.Status == MsgQuestList.QuestListItem.QuestStatus.Finished).Select(p => p.UID).ToArray();
            foreach (var uid in finished)
            {
                ForgetBounty(uid);
                RemoveQuest(uid);
            }
        }
        private bool BountyFitsPlayer(Database.QuestInfo.DBQuest quest)
        {
            if (quest.Lv_min > 0 && Player.Level < quest.Lv_min)
                return false;
            if (quest.Lv_max > 0 && Player.Level > quest.Lv_max)
                return false;
            if (quest.Profession != null && !quest.CanAcceptQuest(Player.Class))
                return false;
            return true;
        }
        /// <summary>Picks up to 3 random bounties the player can still take (not accepted/finished, level/class fit).</summary>
        private void RollBountyHallOffers()
        {
            BountyHallOffered.Clear();
            // [QuestIntegration] ER-7: only bounties the quest engine can complete (their monsters exist and spawn)
            var pool = Database.QuestInfo.AllQuests.Values.Where(p => IsBountyHallMission(p.MissionId) && !src.ContainsKey(p.MissionId) && BountyFitsPlayer(p) && global::Qahr.Mobile.QuestEngine.QuestEngineCore.IsBountyWorkable(p.MissionId)).Select(p => p.MissionId).ToList();
            while (pool.Count > 0 && BountyHallOffered.Count < BountyHallOffers)
            {
                int index = Program.GetRandom.Next(0, pool.Count);
                BountyHallOffered.Add(pool[index]);
                pool.RemoveAt(index);
            }
        }
        private unsafe void SendBountyHallOffers(ServerSockets.Packet stream, MsgQuestList.QuestMode mode)
        {
            // keep only offers that are still available (an accepted one leaves the board)
            BountyHallOffered.RemoveWhere(uid => src.ContainsKey(uid));
            var offers = BountyHallOffered.ToArray();
            stream.QuestListCreate(mode, (ushort)offers.Length);
            foreach (var uid in offers)
                stream.AddItemQuestList(new MsgQuestList.QuestListItem() { UID = uid, Status = (Game.MsgServer.MsgQuestList.QuestListItem.QuestStatus)6, Time = 0 });
            Player.Owner.Send(stream.QuestListFinalize());
        }
        // ===== end Bounty Hall =====
        public unsafe void SetKingDomQuestObjectives(ServerSockets.Packet stream, uint UID, params uint[] Intentions)
        {
            if (Database.QuestInfo.IsKingDomMission(UID))
            {
                if (src.ContainsKey(UID))
                {
                    var item = src[UID];
                    if (item.Status != MsgQuestList.QuestListItem.QuestStatus.Finished)
                    {
                        if (Intentions.Length > item.Intentions.Length)//check the size.
                        {
                            var _Intentions = new uint[item.Intentions.Length];
                            for (int x = 0; x < _Intentions.Length; x++)
                                _Intentions[x] = item.Intentions[x];
                            item.Intentions = new uint[Intentions.Length];
                            for (int x = 0; x < _Intentions.Length; x++)
                                item.Intentions[x] = _Intentions[x];
                        }
                        for (int x = 0; x < Intentions.Length; x++)
                            item.Intentions[x] = Intentions[x];

                        src[UID] = item;
                        Player.Owner.Send(stream.MsgQuestDataCreate(0, UID, item.Intentions));
                    }
                }
                return;
            }
        }
        public void GetQuestObjectives(uint UID, out uint[] Intentions)
        {
            if (src.ContainsKey(UID))
            {
                Intentions = new uint[src[UID].Intentions.Length];
                for (int x = 0; x < src[UID].Intentions.Length; x++)
                    Intentions[x] = src[UID].Intentions[x];
            }
            else
                Intentions = new uint[1];
        }
        public void SetQuestObjectives(ServerSockets.Packet stream, uint UID, params uint[] Intentions)
        {
            if (src.ContainsKey(UID) && IsActiveQuest(UID))
            {
                var item = src[UID];
                if (Intentions.Length > item.Intentions.Length)//check the size.
                {
                    var _Intentions = new uint[item.Intentions.Length];
                    for (int x = 0; x < _Intentions.Length; x++)
                        _Intentions[x] = item.Intentions[x];
                    item.Intentions = new uint[Intentions.Length];
                    for (int x = 0; x < _Intentions.Length; x++)
                        item.Intentions[x] = _Intentions[x];
                }
                for (int x = 0; x < Intentions.Length; x++)
                    item.Intentions[x] = Intentions[x];

                src[UID] = item;
                Player.Owner.Send(stream.MsgQuestDataCreate(0, UID, item.Intentions));
            }
        }
        public unsafe void IncreaseQuestObjectives(ServerSockets.Packet stream, uint UID, params uint[] Intentions)
        {

            if (Database.QuestInfo.IsKingDomMission(UID))
            {
                if (src.ContainsKey(UID))
                {
                    var item = src[UID];
                    if (item.Status != MsgQuestList.QuestListItem.QuestStatus.Finished)
                    {
                        if (Intentions.Length > item.Intentions.Length)//check the size.
                        {
                            var _Intentions = new uint[item.Intentions.Length];
                            for (int x = 0; x < _Intentions.Length; x++)
                                _Intentions[x] = item.Intentions[x];
                            item.Intentions = new uint[Intentions.Length];
                            for (int x = 0; x < _Intentions.Length; x++)
                                item.Intentions[x] = _Intentions[x];
                        }
                        for (int x = 0; x < Intentions.Length; x++)
                            item.Intentions[x] += Intentions[x];

                        src[UID] = item;
                        Player.Owner.Send(stream.MsgQuestDataCreate(0, UID, item.Intentions));
                    }
                }
                return;
            }
            if (IsActiveQuest(UID) && AcceptedQuests.ContainsKey(UID))
            {
                var item = AcceptedQuests[UID];
                if (Intentions.Length > item.Intentions.Length)//check the size.
                {
                    var _Intentions = new uint[item.Intentions.Length];
                    for (int x = 0; x < _Intentions.Length; x++)
                        _Intentions[x] = item.Intentions[x];
                    item.Intentions = new uint[Intentions.Length];
                    for (int x = 0; x < _Intentions.Length; x++)
                        item.Intentions[x] = _Intentions[x];
                }
                for (int x = 0; x < Intentions.Length; x++)
                    item.Intentions[x] += Intentions[x];

                AcceptedQuests[UID] = item;
                Player.Owner.Send(stream.MsgQuestDataCreate(0, UID, item.Intentions));
            }
        }

        public unsafe void SendAutoPatcher(string Text, uint map, ushort x, ushort y, uint NpcUid)
        {
            Player.MessageBox(Text, p =>
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    ActionQuery action = new ActionQuery()
                    {
                        ObjId = p.Player.UID,
                        Type = ActionType.AutoPatcher,
                        NpcID = NpcUid, // [ProtoFix 2026-10-14] ActionQuery.NpcID is uint now
                        PositionX = x,
                        PositionY = y,
                        MapID = map
                        
                    };
                    p.Send(stream.ActionCreate(action));


                }

            }, null, 0);
        }
        public unsafe void SendAutoPatcher(uint map, ushort x, ushort y, uint NpcUid)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                ActionQuery action = new ActionQuery()
                {
                    ObjId = Player.UID,
                    Type = ActionType.AutoPatcher,
                    NpcID = NpcUid, // [ProtoFix 2026-10-14] ActionQuery.NpcID is uint now
                    PositionX = x,
                    PositionY = y,
                    MapID = map
                };
                Player.Send(stream.ActionCreate(action));
            }
        }
        public void RemoveQuest(uint UID)
        {
            if (src.ContainsKey(UID))
                src.Remove(UID);
            if (AcceptedQuests.ContainsKey(UID))
                AcceptedQuests.Remove(UID);
            var n_quest = new Game.MsgServer.MsgQuestList.QuestListItem()
            {
                UID = UID,
                Status = MsgQuestList.QuestListItem.QuestStatus.Available,
                Time = 0
            };
            SendSinglePacket(n_quest, Game.MsgServer.MsgQuestList.QuestMode.CancelBountyHall);
        }
        public bool IsActiveQuest(uint UID)
        {
            return CheckQuest(UID, Game.MsgServer.MsgQuestList.QuestListItem.QuestStatus.Accepted);
        }
        public bool CheckKingDomQuest(uint UID, params uint[] Intentions)
        {
            if (src.ContainsKey(UID))
            {
                try
                {
                    var quest = src[UID];
                    bool isdone = true;
                    for (int x = 0; x < Intentions.Length; x++)
                        if (quest.Intentions[x] < Intentions[x])
                            isdone = false;
                    return isdone;
                }
                catch
                {
                    for (int x = 0; x < 10; x++)
                        MyConsole.WriteLine("Error on quest : " + UID);
                }
              
            }
            return false;
        }
        public bool CheckObjectives(uint UID, params uint[] Intentions)
        {
            if (AcceptedQuests.ContainsKey(UID))
            {
                try
                {
                    var quest = AcceptedQuests[UID];
                    bool isdone = true;
                    for (int x = 0; x < Intentions.Length; x++)
                        if (quest.Intentions[x] < Intentions[x])
                            isdone = false;
                    return isdone;
                }
                catch
                {
                    for (int x = 0; x < 10; x++)
                        MyConsole.WriteLine("Error on quest : " + UID);
                }
                
            }
            
            return false;
           
        }
        public bool CheckQuest(uint UID, Game.MsgServer.MsgQuestList.QuestListItem.QuestStatus status)
        {
            if (Database.QuestInfo.IsKingDomMission(UID))
            {
                return src.Values.Where((p) => p.UID == UID && p.Status == status).Count() == 1;
            }
            if (status == MsgQuestList.QuestListItem.QuestStatus.Accepted)
            {
                return AcceptedQuests.ContainsKey(UID);
            }
            return src.Values.Where((p) => p.UID == UID && p.Status == status).Count() == 1;
        }
        public bool FinishQuest(uint UID)
        {
            if (src.ContainsKey(UID))
            {
                var item = src[UID];
                if (item.Status != Game.MsgServer.MsgQuestList.QuestListItem.QuestStatus.Finished)
                {
                    item.Status = Game.MsgServer.MsgQuestList.QuestListItem.QuestStatus.Finished;
                    src[UID] = item;
                    if (AcceptedQuests.ContainsKey(UID))
                        AcceptedQuests.Remove(UID);
                    SendSinglePacket(item, Game.MsgServer.MsgQuestList.QuestMode.FinishQuest);
                    global::Qahr.Mobile.Phone.MobileDaily.OnQuestFinished(Player, UID);// QahrMobile: mobile daily quest tasks
                    return true;
                }
            }
            return false;
        }

        public bool AcceptKingDomMission(Database.QuestInfo.DBQuest quest, uint time, out Game.MsgServer.MsgQuestList.QuestListItem _quest)
        {
            _quest = null;
            if (Database.QuestInfo.IsKingDomMission(quest.MissionId))
            {
                if (!src.ContainsKey(quest.MissionId))
                {
                    var n_quest = new Game.MsgServer.MsgQuestList.QuestListItem()
                    {
                        UID = quest.MissionId,
                        Status = MsgQuestList.QuestListItem.QuestStatus.Accepted,
                        Time = time,
                        Intentions = new uint[quest.Intentions]
                    };
                    src.Add(quest.MissionId, n_quest);
                    SendSinglePacket(n_quest, Game.MsgServer.MsgQuestList.QuestMode.AcceptQuest);
                    _quest = n_quest;
                    return true;
                }
                else
                {
                    _quest = src[quest.MissionId];
                    return src[quest.MissionId].Status != MsgQuestList.QuestListItem.QuestStatus.Finished;
                }
            }
            return false;
        }


        public unsafe bool Accept(Database.QuestInfo.DBQuest quest, uint time)
        {


            if (!src.ContainsKey(quest.MissionId))
            {
                if (quest.MissionId == 1469)
                    quest.Intentions = 4;
                if (quest.MissionId == 409)
                    quest.Intentions = 3;
                var n_quest = new Game.MsgServer.MsgQuestList.QuestListItem()
                {
                    UID = quest.MissionId,
                    Status = MsgQuestList.QuestListItem.QuestStatus.Accepted,
                    Time = time,
                    Intentions = new uint[quest.Intentions]
                };
                AcceptedQuests.Add(n_quest.UID, n_quest);
                src.Add(n_quest.UID, n_quest);
                SendSinglePacket(n_quest, Game.MsgServer.MsgQuestList.QuestMode.AcceptQuest);
            }
            return true;
        }
        public unsafe bool AcceptBountyHall(Database.QuestInfo.DBQuest quest, uint time)
        {
            // [QuestFix 2026-10-13] only a bounty the hall offered to this player, within the accept and daily limits
            if (quest == null || !IsBountyHallMission(quest.MissionId) || !BountyHallOffered.Contains(quest.MissionId))
                return false;
            if (!AllowAccept() || Player.BountyHallToday >= BountyHallDailyLimit || !BountyFitsPlayer(quest))
                return false;
            if (!src.ContainsKey(quest.MissionId))
            {
                BountyHallOffered.Remove(quest.MissionId);
                BountyHallAccepted.Add(quest.MissionId);
                var n_quest = new Game.MsgServer.MsgQuestList.QuestListItem()
                {
                    UID = quest.MissionId,
                    Status = MsgQuestList.QuestListItem.QuestStatus.Accepted,
                    Time = time,
                    Intentions = new uint[quest.Intentions]
                };
                AcceptedQuests.Add(n_quest.UID, n_quest);
                src.Add(n_quest.UID, n_quest);
                SendSinglePacket(n_quest, Game.MsgServer.MsgQuestList.QuestMode.AcceptBountyHall);
            }
            return true;
        }
        public unsafe void SendSinglePacket(Game.MsgServer.MsgQuestList.QuestListItem data, Game.MsgServer.MsgQuestList.QuestMode mode)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                stream.QuestListCreate(mode, 1);
                stream.AddItemQuestList(data);
                Player.Owner.Send(stream.QuestListFinalize());
                if (mode == MsgQuestList.QuestMode.FinishQuest)
                {
                    // [QuestEngineFix 2026-10-14] not official: no more ball for engine-owned / Bounty quests (see PaysFinishExpBall)
                    if (Player.Level < 130 && global::Qahr.Mobile.QuestEngine.QuestEngineCore.PaysFinishExpBall(data.UID))
                        Player.Owner.GainExpBall(600, false, Flags.ExperienceEffect.None, false, false);
                }
            }
        }

        public int AcceptQuestsCount()
        {
            return src.Values.Where((p) => p.Status == Game.MsgServer.MsgQuestList.QuestListItem.QuestStatus.Accepted).Count();
        }
        public bool AllowAccept()
        {
            return AcceptQuestsCount() < 5;
        }
        public unsafe void SendFullGUI(ServerSockets.Packet stream)
        {
            if (Database.AtributesStatus.IsLee(Player.Class))
            {
                if (!src.ContainsKey(6435))//first Quest
                {
                    var n_quest = new Game.MsgServer.MsgQuestList.QuestListItem()
                    {
                        UID = 6435,
                        Status = Game.MsgServer.MsgQuestList.QuestListItem.QuestStatus.Accepted,
                        Time = 0

                    };
                    AcceptedQuests.Add(n_quest.UID, n_quest);
                    src.Add(n_quest.UID, n_quest);
#if Arabic
                    SendAutoPatcher("Son of Dragon, you're destined for the Power of Dragon. Pay a visit to the previous holder, Kungfu King, for more details", 1002, 269, 217, 17222);
#else
                    SendAutoPatcher("Son of Dragon, you're destined for the Power of Dragon. Pay a visit to the previous holder, Kungfu King, for more details", 1002, 269, 217, 17222);
#endif

                }
            }
            else
            {
                if (!src.ContainsKey(804))//first Quest 804
                {
                    var n_quest = new Game.MsgServer.MsgQuestList.QuestListItem()
                    {
                        UID = 804,
                        Status = Game.MsgServer.MsgQuestList.QuestListItem.QuestStatus.Accepted,
                        Time = 0

                    };
                    AcceptedQuests.Add(n_quest.UID, n_quest);
                    src.Add(n_quest.UID, n_quest);
#if Arabic
                      SendAutoPatcher("Welcome!~Pay~a~visit~to~the~Kungfu~Boy~to~learn~more~about~the~world!", 1002, 270, 223, 5673);
#else
                    SendAutoPatcher("Welcome!~Pay~a~visit~to~the~Kungfu~Boy~to~learn~more~about~the~world!", 1002, 270, 223, 5673);
#endif

                }
            }
            Dictionary<int, Queue<Game.MsgServer.MsgQuestList.QuestListItem>> Collection = new Dictionary<int, Queue<Game.MsgServer.MsgQuestList.QuestListItem>>();
            Collection.Add(0, new Queue<Game.MsgServer.MsgQuestList.QuestListItem>());

            int count = 0;
            var Array = Database.QuestInfo.AllQuests.Values.ToArray();
            for (uint x = 0; x < Array.Length; x++)
            {
                if (x % 80 == 0)
                {
                    count++;
                    Collection.Add(count, new Queue<Game.MsgServer.MsgQuestList.QuestListItem>());
                }
                if (src.ContainsKey(Array[x].MissionId))
                    Collection[count].Enqueue(src[Array[x].MissionId]);
                else
                {
                    var quest = new Game.MsgServer.MsgQuestList.QuestListItem()
                    {
                        UID = Array[x].MissionId,
                        Status = Game.MsgServer.MsgQuestList.QuestListItem.QuestStatus.Available,
                        Time = 0
                    };
                    Collection[count].Enqueue(quest);
                }
            }



            foreach (var aray in Collection.Values)
            {
                Queue<Game.MsgServer.MsgQuestList.QuestListItem> ItemArray = aray;

                ushort CCount = (ushort)ItemArray.Count;
                stream.QuestListCreate(MsgQuestList.QuestMode.Review, CCount);
                for (byte x = 0; x < CCount; x++)
                {
                    stream.AddItemQuestList(ItemArray.Dequeue());
                }
                Player.Owner.Send(stream.QuestListFinalize());
            }
        }
        public unsafe void ShowAll(ServerSockets.Packet stream)
        {
            // [QuestFix 2026-10-13] was: always the first 3 quests >= 590000 (accepted/finished ones included).
            // Now: up to 3 random bounties this player can still take; reopening the hall keeps the same board.
            BountyHallOffered.RemoveWhere(uid => src.ContainsKey(uid));
            if (BountyHallOffered.Count == 0)
                RollBountyHallOffers();
            SendBountyHallOffers(stream, MsgQuestList.QuestMode.ShowBountyHall);
        }
        public unsafe void RefreshBountyHall(ServerSockets.Packet stream)
        {
            // [QuestFix 2026-10-13] a new random board (same rules as ShowAll)
            RollBountyHallOffers();
            SendBountyHallOffers(stream, MsgQuestList.QuestMode.RefreshBountyHall);
        }
    }
}
