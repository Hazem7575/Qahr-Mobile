using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class MsgNameChange
    {
        public static unsafe ServerSockets.Packet NameChangeCreate(this ServerSockets.Packet stream, ActionID mode, ushort EditsCount, ushort EditsAllowed, string Name)
        {
            stream.InitWriter();

            stream.Write((ushort)mode);
            stream.Write(EditsCount);
            stream.Write(EditsAllowed);
            stream.Write(Name, 32);
            stream.Finalize(GamePackets.NameChange);

            return stream;
        }
        public static unsafe void GetNameChange(this ServerSockets.Packet stream, out ActionID mode, out ushort EditsCount, out ushort EditsAllowed, out string Name)
        {
            mode = (ActionID)stream.ReadUInt16();
            EditsCount = stream.ReadUInt16();
            EditsAllowed = stream.ReadUInt16();
            Name = stream.ReadCString(32);
        }
        public enum ActionID : ushort
        {
            Request = 0,
            Success = 1,
            NameTaken = 2,
            DialogInfo = 3,
            FreeChange = 4,
        }

        public static ExecuteLogin NameChangeQueue = new ExecuteLogin();

        public class ExecuteLogin : ConcurrentSmartThreadQueue<ChangeObject>
        {

            public ExecuteLogin()
                : base(5)
            {
                Start(10);
            }
            public void TryEnqueue(ChangeObject obj)
            {
                base.Enqueue(obj);
            }
            protected unsafe override void OnDequeue(ChangeObject obj, int time)
            {
                try
                {
                    Client.GameClient user = obj.user;
                    string Name = obj.Name;

                    switch (obj.mode)
                    {
                        case ActionID.Request:
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    if (user.Player.ConquerPoints < 810)
                                        break;
                                    if (user.InTrade) return;
                                    if (Name != null && Name != "" && Name.Length >= 4 && BaseFunc.NameStrCheck(Name) && user.Player.NameEditCount < 5 && !Pool.NameUsed.Contains<int>(Name.GetHashCode()))
                                    {
                                        user.Player.NameEditCount += 1;
                                        user.Send(stream.NameChangeCreate(ActionID.Success, user.Player.NameEditCount, 5, Name));

                                        MsgMessage messaj = new MsgMessage(user.Player.Name + " changed the name to be " + Name + " .The new name will be registered, immediately.", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System);

                                        String folderN = DateTime.Now.Year + "-" + DateTime.Now.Month,
                   Path = "Cache\\Logs\\ChangeName\\",
                   NewPath = System.IO.Path.Combine(Path, folderN);
                                        if (!File.Exists(NewPath + folderN))
                                        {
                                            System.IO.Directory.CreateDirectory(System.IO.Path.Combine(Path, folderN));
                                        }
                                        if (!File.Exists(NewPath + "\\" + DateTime.Now.Day + ".txt"))
                                        {
                                            using (System.IO.FileStream fs = System.IO.File.Create(NewPath + "\\" + DateTime.Now.Day + ".txt"))
                                            {
                                                fs.Close();
                                            }
                                        }
                                        using (System.IO.StreamWriter file = new System.IO.StreamWriter(NewPath + "\\" + DateTime.Now.Day + ".txt", true))
                                        {
                                            file.WriteLine("UserName[Acc] |" + user.Player.Name + "| UIDAccount |" + user.Player.UID + "|  NameEdit  [" + Name + "] [The Clock] " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second);
                                            file.Close();
                                        }
                                        Server.SendGlobalPacket(messaj.GetArray(stream));

                                        Role.Instance.Nobility Nobility;
                                        if (Pool.NobilityRanking.TryGetValue(user.Player.UID, out Nobility))
                                        {
                                            Nobility.Name = Name;
                                            user.Send(stream.NobilityIconCreate(Nobility));
                                        }

                                        if (user.Player.MyChi != null)
                                        {
                                            user.Player.MyChi.Name = Name;
                                            foreach (var chi in user.Player.MyChi)
                                                chi.Name = Name;
                                        }
                                        if (user.Player.MyJiangHu != null)
                                            user.Player.MyJiangHu.Name = Name;
                                        if (user.Player.InnerPower != null)
                                            user.Player.InnerPower.Name = Name;
                                        if (user.Player.MyGuild != null && user.Player.MyGuildMember != null)
                                        {
                                            user.Player.MyGuildMember.Name = Name;
                                            if (user.Player.MyGuildMember.Rank == Role.Flags.GuildMemberRank.GuildLeader)
                                            {
                                                user.Player.MyGuild.Info.LeaderName = Name;
                                            }
                                        }
                                        if (user.Player.MyClan != null && user.Player.MyClanMember != null)
                                        {
                                            user.Player.MyClanMember.Name = Name;
                                            if (user.Player.MyClanMember.Rank == Role.Instance.Clan.Ranks.Leader)
                                                user.Player.MyClan.LeaderName = Name;
                                        }
                                        if (user.ArenaStatistic != null)
                                            user.ArenaStatistic.Name = Name;                                        
                                        if (user.Player.Flowers != null)
                                        {
                                            foreach (var flower in user.Player.Flowers)
                                                if (flower != null)
                                                    flower.Name = Name;
                                        }

                                        user.Player.Name = Name;
                                        user.Player.View.ReSendView(stream);

                                        if (user.Player.Spouse != null)
                                        {
                                            Client.GameClient spouse;
                                            if (Pool.GamePoll.TryGetValue(user.Player.SpouseUID, out spouse))
                                            {
                                                spouse.Player.Spouse = Name;
                                                spouse.Player.SendString(stream, MsgStringPacket.StringID.Spouse, false, Name);
                                            }
                                            else
                                            {
                                                user.ClientFlag |= Client.ServerFlag.UpdateSpouse;
                                                Database.ServerDatabase.LoginQueue.Enqueue(user);
                                            }
                                        }
                                        user.Player.ConquerPoints -= 810;
                                        lock (Pool.NameUsed)
                                            Pool.NameUsed.Add(Name.GetHashCode());

                                        try
                                        {
                                            Role.Instance.Associate.MyAsociats MyAsociation;
                                            if (Role.Instance.Associate.Associates.TryGetValue(user.Player.UID, out MyAsociation))
                                            {
                                                foreach (var table in MyAsociation.Associat.Values)
                                                {
                                                    if (table == null)
                                                        continue;
                                                    if (table.Values == null)
                                                        continue;
                                                    foreach (var _ass in table.Values)
                                                    {
                                                        if (_ass == null)
                                                            continue;
                                                        Role.Instance.Associate.MyAsociats associat;
                                                        if (Role.Instance.Associate.Associates.TryGetValue(_ass.UID, out associat))
                                                        {
                                                            foreach (var _table in associat.Associat)
                                                            {

                                                                if (_table.Value == null)
                                                                    continue;
                                                                Role.Instance.Associate.Member my_relation;
                                                                if (_table.Value.TryGetValue(user.Player.UID, out my_relation))
                                                                    my_relation.Name = Name;
                                                                if (associat.MyClient != null)
                                                                {
                                                                    if (_table.Key == (byte)Role.Instance.Associate.Friends)
                                                                    {
                                                                        associat.MyClient.Send(stream.KnowPersonsCreate(MsgKnowPersons.Action.RemovePerson, user.Player.UID, true, "", (uint)user.Player.NobilityRank, user.Player.Mesh / 1000, user.Player.Level, user.Player.Description));
                                                                        associat.MyClient.Send(stream.KnowPersonsCreate(MsgKnowPersons.Action.AddFriend, user.Player.UID, true, Name, (uint)user.Player.NobilityRank, user.Player.Mesh / 1000, user.Player.Level, user.Player.Description));
                                                                    }
                                                                    else if (_table.Key == (byte)Role.Instance.Associate.Partener)
                                                                    {
                                                                        associat.MyClient.Send(stream.TradePartnerCreate(user.Player.UID, MsgTradePartner.Action.BreakPartnership, true, 0, "", user.Player.Level, user.Player.Description, user.Player.Mesh));
                                                                        associat.MyClient.Send(stream.TradePartnerCreate(user.Player.UID, MsgTradePartner.Action.AddPartner, true, _ass.GetTimerLeft(), Name, user.Player.Level, user.Player.Description, user.Player.Mesh));
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                        }
                                        catch (Exception e)
                                        {
                                            MyConsole.SaveException(e);
                                        }
                                    }
                                    else
                                        user.Send(stream.NameChangeCreate(ActionID.NameTaken, 0, 0, Name));
                                }
                                break;
                            }
                    }
                }
                catch (Exception e)
                {
                    MyConsole.SaveException(e);
                }
            }
        }
        public class ChangeObject
        {
            public Client.GameClient user;
            public bool Bypass = false;
            public ActionID mode;
            public ushort EditsCount;
            public ushort EditsAllowed;
            public string Name = "";
        }

        [PacketAttribute(GamePackets.NameChange)]
        public unsafe static void Proces(Client.GameClient user, ServerSockets.Packet stream)
        {
            if (!user.Player.OnMyOwnServer)
                return;
            ChangeObject obj = new ChangeObject();
            obj.user = user;
            stream.GetNameChange(out obj.mode, out  obj.EditsCount, out  obj.EditsAllowed, out  obj.Name);
            NameChangeQueue.TryEnqueue(obj);
        }
        public unsafe static void ChangeName(Client.GameClient user, string name, bool bypass = false)
        {
            ChangeObject obj = new ChangeObject();
            obj.user = user;
            obj.Bypass = bypass;
            obj.Name = name;
            obj.mode = ActionID.Request;
            NameChangeQueue.TryEnqueue(obj);
        }
    }
}
