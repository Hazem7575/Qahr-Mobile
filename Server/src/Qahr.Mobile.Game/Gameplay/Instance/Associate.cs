using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.Role.Instance
{
    public class Associate
    {

        public const byte Friends = 1, Enemy = 2, Partener = 3, Mentor = 4, Apprentice = 5, PKExplorer = 6;

        public class Member
        {
            public uint UID = 0;
            public ulong Timer = 0;
            public uint ExpBalls = 0;
            public uint Stone = 0;
            public uint KillMap = 0;
            public uint Blessing = 0;
            public string Name = "";
            public string Description = "";
            public uint Mesh = 0;
            public uint Frame = 0;
            public ushort Level = 0;
            public ushort KillsCount = 0;
            public ushort BattlePower = 0;

            public bool IsOnline { get { return Pool.GamePoll.ContainsKey(UID); } }

            public int GetTimerLeft()
            {
                if (Timer == 0)
                    return 0;
                int timer = (int)(new TimeSpan((long)Timer).TotalMinutes - new TimeSpan(DateTime.Now.Ticks).TotalMinutes);
                if (timer <= 0)
                {
                    Timer = 0;
                    return 0;
                }

                return timer;
            }
            public override string ToString()
            {
                Database.DBActions.WriteLine writer = new Database.DBActions.WriteLine('/');
                writer.Add(UID).Add(Timer).Add(ExpBalls).Add(Stone).Add(Blessing).Add(Name).Add(KillsCount).Add(BattlePower).Add(KillMap).Add(Description).Add(Mesh).Add(Level);
                return writer.Close();
            }

        }
        public class MyAsociats
        {
            public ConcurrentDictionary<byte, ConcurrentDictionary<uint, Member>> Associat = new ConcurrentDictionary<byte, ConcurrentDictionary<uint, Member>>();

            public ConcurrentDictionary<uint, Client.GameClient> OnlineApprentice = new ConcurrentDictionary<uint, Client.GameClient>();


            public bool HaveAsociats()
            {
                foreach (var items in Associat)
                {
                    if (items.Key != PKExplorer && items.Key != Enemy)
                        if (items.Value.Count > 0)
                            return true;
                }
                return false;
            }

            public bool Online = false;
            public Client.GameClient MyClient;
            public uint MyUID = 0;
            public uint Mentor_Stones = 0;
            public uint Mentor_ExpBalls = 0;
            public uint Mentor_Blessing = 0;

            public MyAsociats(uint uid)
            {
                MyUID = uid;
            }
            public bool OnUse()
            {
                if (MyUID != 0)
                {
                    if (!Associates.ContainsKey(MyUID))
                        Associates.TryAdd(MyUID, this);
                }
                else
                    return false;

                return true;
            }

            public bool Contain(byte Mode, uint UID)
            {
                if (Associat.ContainsKey(Mode))
                {
                    if (Associat[Mode].ContainsKey(UID))
                        return true;
                }
                return false;
            }
            public bool Remove(byte Mode, uint UID)
            {
                if (!Associat.ContainsKey(Mode))
                    return false;
                if (!Associat[Mode].ContainsKey(UID))
                    return false;

                Member mem;
                return Associat[Mode].TryRemove(UID, out mem);
            }
            public bool AllowAdd(byte Mode, uint UID, byte amout)
            {
                if (!Associat.ContainsKey(Mode))
                    return true;
                if (Associat[Mode].ContainsKey(UID))
                    return false;
                if (Associat[Mode].Count < amout)
                    return true;

                return false;
            }
            public void Add(byte mode, Member member)
            {

                if (!OnUse())
                    return;

                if (Associat.ContainsKey(mode))
                    Associat[mode].TryAdd(member.UID, member);
                else
                {
                    Associat.TryAdd(mode, new ConcurrentDictionary<uint, Member>());
                    Associat[mode].TryAdd(member.UID, member);
                }
            }
            public void AddPartener(Client.GameClient Owner, Role.Player client)
            {
                if (AllowAdd(Partener, client.UID, 10))
                {
                    Member member = new Member()
                    {
                        UID = client.UID,
                        Mesh = client.Mesh,
                        Level = client.Level,
                        Name = client.Name,
                        Description = client.Description,
                        Timer = (ulong)DateTime.Now.AddDays(3).Ticks
                    };
                    Add(Partener, member);
                }
                else
                {
#if Arabic
                     Owner.SendSysMesage("Sorry, you used the limit of partener`s");
#else
                    Owner.SendSysMesage("Sorry, you used the limit of partener`s");
#endif

                }
            }
            public void AddAprrentice(Client.GameClient Owner, Role.Player client)
            {
                if (AllowAdd(Apprentice, client.UID, (byte)Database.TutorInfo.AddAppCount(Owner)))
                {
                    if (OnlineApprentice.TryAdd(client.UID, client.Owner))
                    {
                        Member member = new Member()
                        {
                            UID = client.UID,
                            Name = client.Name
                        };
                        Add(Apprentice, member);
                    }
                }
                else
                {
#if Arabic
                     Owner.SendSysMesage("Sorry, you used the limit of aprrentice`s");
#else
                    Owner.SendSysMesage("Sorry, you used the limit of aprrentice`s");
#endif

                }
            }
            public void AddMentor(Client.GameClient Owner, Role.Player client)
            {
                if (AllowAdd(Mentor, client.UID, 1))
                {
                    Member member = new Member()
                    {
                        UID = client.UID,
                        Name = client.Name

                    };
                    Add(Mentor, member);
                }
            }
            public void AddFriends(Client.GameClient Owner, Role.Player client)
            {
                if (AllowAdd(Friends, client.UID, 50))
                {
                    Member member = new Member()
                    {
                        UID = client.UID,
                        Mesh = client.Mesh,
                        Level = client.Level,
                        Name = client.Name,
                        Description = client.Description

                    };
                    Add(Friends, member);
                }
                else
                {
#if Arabic
                     Owner.SendSysMesage("Sorry, you used the limit of frinds");
#else
                    Owner.SendSysMesage("Sorry, you used the limit of frinds");
#endif

                }
            }
            public uint GetTimePkExplorer()
            {
                uint valu = 0;
                DateTime timer = DateTime.Now;
                valu = (uint)((timer.Month * 1000000) + (timer.Day * 10000) + (timer.Hour * 100) + timer.Minute);
                return valu;
            }
            public void AddPKExplorer(Client.GameClient Owner, Role.Player client)
            {
                if (!OnUse())
                    return;

                Member member = new Member()
                {
                    UID = client.UID,
                    Timer = GetTimePkExplorer(),
                    BattlePower = (ushort)client.BattlePower,
                    Name = client.Name
                };
                member.Timer = (uint)GetTimePkExplorer();
                member.KillsCount++;
                member.KillMap = client.Map;
                if (Associat.ContainsKey(PKExplorer))
                {
                    Member Gmem;
                    if (Associat[PKExplorer].TryGetValue(member.UID, out Gmem))
                    {
                        Gmem.Timer = GetTimePkExplorer();
                        Gmem.KillsCount++;
                        Gmem.KillMap = client.Map;
                        Gmem.BattlePower = (ushort)client.BattlePower;
                    }
                    else
                    {
                        if (AllowAdd(PKExplorer, member.UID, 50))
                        {
                            Associat[PKExplorer].TryAdd(member.UID, member);
                        }
                        else
                        {
                            var remover = Associat[PKExplorer].Values.ToArray()[0];
                            Remove(PKExplorer, remover.UID);
                            if (AllowAdd(PKExplorer, member.UID, 50))
                                Add(PKExplorer, member);
                        }
                    }
                }
                else
                {
                    Associat.TryAdd(PKExplorer, new ConcurrentDictionary<uint, Member>());
                    Associat[PKExplorer].TryAdd(member.UID, member);
                }
            }
            public Member[] GetPkExplorerRank()
            {
                if (Associat.ContainsKey(PKExplorer))
                {
                    var rnk = Associat[PKExplorer].Values.OrderBy(kill => kill.KillsCount).ToArray();
                    return rnk;
                }
                return new Member[0];
            }

            public void AddEnemy(Client.GameClient Owner, Role.Player Killer)
            {
                Member member = new Member()
                {
                    UID = Killer.UID,
                    Name = Killer.Name,
                    Level = Killer.Level,
                    Description = Killer.Description,
                    Mesh = Killer.Face
                };

                if (AllowAdd(Enemy, Killer.UID, 20))
                {
                    Add(Enemy, member);
                }
                else
                {
                    var remover = Associat[Enemy].Values.ToArray()[0];
                    Remove(Enemy, remover.UID);
                    if (AllowAdd(Enemy, Killer.UID, 20))
                        Add(Enemy, member);
                }
                unsafe
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();

                        Owner.Send(stream.KnowPersonsCreate(MsgKnowPersons.Action.AddEnemy, Killer.UID, true, Killer.Name, (uint)Killer.NobilityRank, Killer.Mesh / 1000, Killer.Level, Killer.Description));

                    }
                }
            }
            public unsafe void OnLoading(Client.GameClient client)
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    Game.MsgServer.MsgApprenticeInformation mentorandapprentice = Game.MsgServer.MsgApprenticeInformation.Create();
                    foreach (var typ in Associat)
                    {
                        foreach (Member mem in typ.Value.Values)
                        {
                            if (typ.Key == Apprentice)
                            {
                                Client.GameClient clients;
                                if (Pool.GamePoll.TryGetValue(mem.UID, out clients))
                                {
                                    if (client.Player.Associate.OnlineApprentice.TryAdd(clients.Player.UID, clients))
                                    {
                                        var my_apprentice = Associat[Apprentice][clients.Player.UID];
                                        mentorandapprentice.Mode = Game.MsgServer.MsgApprenticeInformation.Action.Apprentice;
                                        mentorandapprentice.Mentor_ID = client.Player.UID;
                                        mentorandapprentice.Apprentice_ID = clients.Player.UID;
                                        mentorandapprentice.Apprentice_Blessing = (ushort)my_apprentice.Blessing;
                                        mentorandapprentice.Apprentice_Composing = (ushort)my_apprentice.Stone;
                                        mentorandapprentice.Apprentice_Experience = (ushort)my_apprentice.ExpBalls;
                                        mentorandapprentice.Class = clients.Player.Class;
                                        mentorandapprentice.Enrole_date = (uint)my_apprentice.Timer;
                                        mentorandapprentice.Mesh = clients.Player.Mesh;
                                        mentorandapprentice.Level = (byte)clients.Player.Level;
                                        mentorandapprentice.Online = 1;
                                        mentorandapprentice.PkPoints = clients.Player.PKPoints;
                                        mentorandapprentice.WriteString(client.Player.Name, clients.Player.Spouse, clients.Player.Name);
                                        client.Send(mentorandapprentice.GetArray(stream));



                                        clients.Player.SetMentorBattlePowers(client.Player.GetShareBattlePowers((uint)clients.Player.RealBattlePower), (uint)client.Player.RealBattlePower);
                                        mentorandapprentice.Mode = Game.MsgServer.MsgApprenticeInformation.Action.Mentor;
                                        mentorandapprentice.Class = client.Player.Class;
                                        mentorandapprentice.Enrole_date = (uint)my_apprentice.Timer;
                                        mentorandapprentice.Mesh = client.Player.Mesh;
                                        mentorandapprentice.Level = (byte)client.Player.Level;
                                        mentorandapprentice.Online = 1;
                                        mentorandapprentice.PkPoints = client.Player.PKPoints;
                                        mentorandapprentice.Shared_Battle_Power = client.Player.GetShareBattlePowers((uint)clients.Player.RealBattlePower);
                                        mentorandapprentice.WriteString(client.Player.Name, clients.Player.Spouse, clients.Player.Name);
                                        clients.Send(mentorandapprentice.GetArray(stream));
                                    }
                                }
                                else
                                {

                                    mentorandapprentice.Class = 0;
                                    mentorandapprentice.Mesh = 0;
                                    mentorandapprentice.Level = 0;
                                    mentorandapprentice.Online = 0;
                                    mentorandapprentice.PkPoints = 0;
                                    mentorandapprentice.Shared_Battle_Power = 0;

                                    mentorandapprentice.Mode = Game.MsgServer.MsgApprenticeInformation.Action.Apprentice;
                                    mentorandapprentice.Mentor_ID = MyUID;
                                    mentorandapprentice.Apprentice_ID = mem.UID;
                                    mentorandapprentice.Enrole_date = (uint)mem.Timer;
                                    mentorandapprentice.WriteString("NULL", "NULL", mem.Name);
                                    client.Send(mentorandapprentice.GetArray(stream));

                                }
                            }
                            if (typ.Key == Mentor)
                            {

                                Client.GameClient clients;
                                if (Pool.GamePoll.TryGetValue(mem.UID, out clients))
                                {
                                    Member apprentice;
                                    if (clients.Player.Associate.Associat[Apprentice].TryGetValue(client.Player.UID, out apprentice))
                                    {
                                        if (clients.Player.Associate.OnlineApprentice.TryAdd(client.Player.UID, client))
                                        {
                                            mentorandapprentice.Mode = Game.MsgServer.MsgApprenticeInformation.Action.Apprentice;
                                            mentorandapprentice.Mentor_ID = clients.Player.UID;
                                            mentorandapprentice.Apprentice_ID = client.Player.UID;
                                            mentorandapprentice.Apprentice_Blessing = (ushort)apprentice.Blessing;
                                            mentorandapprentice.Apprentice_Composing = (ushort)apprentice.Stone;
                                            mentorandapprentice.Apprentice_Experience = (ushort)apprentice.ExpBalls;
                                            mentorandapprentice.Class = client.Player.Class;
                                            mentorandapprentice.Enrole_date = (uint)apprentice.Timer;
                                            mentorandapprentice.Mesh = client.Player.Mesh;
                                            mentorandapprentice.Level = (byte)client.Player.Level;
                                            mentorandapprentice.Online = 1;
                                            mentorandapprentice.PkPoints = client.Player.PKPoints;
                                            mentorandapprentice.WriteString(clients.Player.Name, client.Player.Spouse, client.Player.Name);
                                            clients.Send(mentorandapprentice.GetArray(stream));



                                            client.Player.SetMentorBattlePowers(clients.Player.GetShareBattlePowers((uint)client.Player.RealBattlePower), (uint)clients.Player.RealBattlePower);
                                            mentorandapprentice.Mode = Game.MsgServer.MsgApprenticeInformation.Action.Mentor;
                                            mentorandapprentice.Class = clients.Player.Class;
                                            mentorandapprentice.Enrole_date = (uint)apprentice.Timer;
                                            mentorandapprentice.Mesh = clients.Player.Mesh;
                                            mentorandapprentice.Level = (byte)clients.Player.Level;
                                            mentorandapprentice.Online = 1;
                                            mentorandapprentice.PkPoints = clients.Player.PKPoints;
                                            mentorandapprentice.Shared_Battle_Power = clients.Player.GetShareBattlePowers((uint)client.Player.RealBattlePower);
                                            mentorandapprentice.WriteString(clients.Player.Name, client.Player.Spouse, client.Player.Name);
                                            client.Send(mentorandapprentice.GetArray(stream));
                                        }
                                    }
                                }
                                else
                                {
                                    mentorandapprentice.Class = 0;
                                    mentorandapprentice.Mesh = 0;
                                    mentorandapprentice.Level = 0;
                                    mentorandapprentice.Online = 0;
                                    mentorandapprentice.PkPoints = 0;
                                    mentorandapprentice.Shared_Battle_Power = 0;

                                    mentorandapprentice.Mode = Game.MsgServer.MsgApprenticeInformation.Action.Mentor;
                                    mentorandapprentice.Mentor_ID = mem.UID;
                                    mentorandapprentice.Apprentice_ID = MyUID;
                                    mentorandapprentice.Enrole_date = (uint)mem.Timer;
                                    mentorandapprentice.WriteString(mem.Name, "", "");
                                    client.Send(mentorandapprentice.GetArray(stream));
                                }
                            }
                            if (typ.Key == Friends)
                            {
                                Client.GameClient Targer;
                                if (Pool.GamePoll.TryGetValue(mem.UID, out Targer))
                                {
                                    client.Send(stream.KnowPersonsCreate(MsgKnowPersons.Action.AddFriend, mem.UID, true, mem.Name, (uint)Targer.Player.NobilityRank, Targer.Player.Mesh / 1000, Targer.Player.Level, Targer.Player.Description));
                                    Targer.Send(stream.KnowPersonsCreate(MsgKnowPersons.Action.AddOnline, client.Player.UID, true, client.Player.Name, (uint)client.Player.NobilityRank, client.Player.Mesh / 1000, client.Player.Level, client.Player.Description));
                                }
                                else
                                    client.Send(stream.KnowPersonsCreate(MsgKnowPersons.Action.AddFriend, mem.UID, false, mem.Name, 0, 0, mem.Level, mem.Description));

                            }
                            if (typ.Key == Enemy)
                            {
                                Client.GameClient Targer;
                                if (Pool.GamePoll.TryGetValue(mem.UID, out Targer))
                                {

                                    client.Send(stream.KnowPersonsCreate(MsgKnowPersons.Action.AddEnemy, mem.UID, true, mem.Name, (uint)Targer.Player.NobilityRank, Targer.Player.Mesh / 1000, Targer.Player.Level, Targer.Player.Description));

                                }
                                else

                                    client.Send(stream.KnowPersonsCreate(MsgKnowPersons.Action.AddEnemy, mem.UID, false, mem.Name, 0, 0, mem.Level, mem.Description));

                            }
                            if (typ.Key == Partener)
                            {
                                Client.GameClient Targer;
                                if (Pool.GamePoll.TryGetValue(mem.UID, out Targer))
                                {
                                    client.Send(stream.TradePartnerCreate(mem.UID, MsgTradePartner.Action.AddPartner, true, mem.GetTimerLeft(), mem.Name, mem.Level, mem.Description, mem.Mesh));

                                    Targer.Send(stream.TradePartnerCreate(client.Player.UID, MsgTradePartner.Action.AddOnline, true, mem.GetTimerLeft(), client.Player.Name, client.Player.Level, client.Player.Description, client.Player.Mesh));
                                }
                                else client.Send(stream.TradePartnerCreate(mem.UID, MsgTradePartner.Action.AddPartner, false, mem.GetTimerLeft(), mem.Name, mem.Level, mem.Description, mem.Mesh));

                            }
                        }
                    }
                }

            }
            public unsafe void OnDisconnect(ServerSockets.Packet stream, Client.GameClient client)
            {
                Game.MsgServer.MsgApprenticeInformation mentorandapprentice = Game.MsgServer.MsgApprenticeInformation.Create();

                foreach (Client.GameClient clients in Pool.GamePoll.Values)
                {
                    foreach (var typ in Associat)
                    {
                        if (!typ.Value.ContainsKey(clients.Player.UID))
                            continue;
                        if (typ.Key == Apprentice)
                        {
                            mentorandapprentice.Mentor_ID = client.Player.UID;
                            mentorandapprentice.Apprentice_ID = clients.Player.UID;
                            clients.Player.SetMentorBattlePowers(0, 0);
                            mentorandapprentice.Mode = Game.MsgServer.MsgApprenticeInformation.Action.Mentor;
                            mentorandapprentice.Online = 0;
                            mentorandapprentice.WriteString(client.Player.Name, clients.Player.Spouse, clients.Player.Name);
                            clients.Send(mentorandapprentice.GetArray(stream));

                        }
                        if (typ.Key == Mentor)
                        {
                            mentorandapprentice.Mode = Game.MsgServer.MsgApprenticeInformation.Action.Apprentice;
                            mentorandapprentice.Mentor_ID = clients.Player.UID;
                            mentorandapprentice.Apprentice_ID = client.Player.UID;
                            mentorandapprentice.Online = 0;
                            mentorandapprentice.WriteString(clients.Player.Name, client.Player.Spouse, client.Player.Name);
                            clients.Send(mentorandapprentice.GetArray(stream));

                        }
                        if (typ.Key == Friends)
                        {

                            clients.Send(stream.KnowPersonsCreate(MsgKnowPersons.Action.AddOffline, client.Player.UID, false, client.Player.Name, 0, client.Player.Face, client.Player.Level, client.Player.Description));
                        }
                        if (typ.Key == Partener)
                        {

                            clients.Send(stream.TradePartnerCreate(client.Player.UID, MsgTradePartner.Action.AddOffline, false, 0, client.Player.Name, client.Player.Level, client.Player.Description, client.Player.Mesh));
                        }
                    }
                }
            }
            public IEnumerable<string> ToStringMember()
            {
                foreach (var typ in Associat)
                    foreach (Member member in typ.Value.Values)
                        yield return MyUID + "/" + typ.Key + "/" + Mentor_ExpBalls + "/" + Mentor_Blessing + "/" + Mentor_Stones +
                            "/" + 0 + "/" + 0 + "/" + 0 + "/" + member.ToString();
            }
        }
        public static ConcurrentDictionary<uint, MyAsociats> Associates = new ConcurrentDictionary<uint, MyAsociats>();
        public static void RemoveOffline(byte Mode, uint UID, uint OnRemove)
        {
            MyAsociats associate;
            if (Associates.TryGetValue(UID, out associate))
            {
                associate.Remove(Mode, OnRemove);
            }
        }

        public static void Save()
        {
            try
            {
                using (Database.DBActions.Write _wr = new Database.DBActions.Write("Associate.txt"))
                {
                    foreach (var x in Associates)
                    {
                        foreach (var member in x.Value.ToStringMember())
                        {
                            _wr.Add(member);
                        }
                    }
                    _wr.Execute(Database.DBActions.Mode.Open);
                }
            }
            catch (Exception e)
            {
                MyConsole.WriteLine(e.ToString());
            }
        }
        public static void Load()
        {
            try
            {
                using (Database.DBActions.Read r = new Database.DBActions.Read("Associate.txt"))
                {
                    if (r.Reader())
                    {
                        int count = r.Count;
                        for (uint x = 0; x < count; x++)
                        {
                            string[] data = r.ReadString("").Split('/');
                            uint UID = uint.Parse(data[0]);
                            byte Mod = byte.Parse(data[1]);
                            uint MentorExpBalls = uint.Parse(data[2]);
                            uint MentorBless = uint.Parse(data[3]);
                            uint MentorStone = uint.Parse(data[4]);
                            Member membru = new Member();
                            membru.UID = uint.Parse(data[8]);
                            membru.Timer = ulong.Parse(data[9]);
                            membru.ExpBalls = uint.Parse(data[10]);
                            membru.Stone = uint.Parse(data[11]);
                            membru.Blessing = uint.Parse(data[12]);
                            membru.Name = data[13];
                            membru.KillsCount = ushort.Parse(data[14]);
                            membru.BattlePower = ushort.Parse(data[15]);
                            membru.KillMap = uint.Parse(data[16]);
                            membru.Description = data[17];
                            membru.Mesh = uint.Parse(data[18]);
                            membru.Level = ushort.Parse(data[19]);
                            if (Associates.ContainsKey(UID))
                            {
                                if (Associates[UID].Associat.ContainsKey(Mod))
                                    Associates[UID].Associat[Mod].TryAdd(membru.UID, membru);
                                else
                                {
                                    Associates[UID].Associat.TryAdd(Mod, new ConcurrentDictionary<uint, Member>());
                                    Associates[UID].Associat[Mod].TryAdd(membru.UID, membru);
                                }
                            }
                            else
                            {
                                MyAsociats assoc = new MyAsociats(UID);
                                assoc.MyUID = UID;
                                assoc.Mentor_ExpBalls = MentorExpBalls;
                                assoc.Mentor_Blessing = MentorBless;
                                assoc.Mentor_Stones = MentorStone;
                                assoc.Associat.TryAdd(Mod, new ConcurrentDictionary<uint, Member>());
                                assoc.Associat[Mod].TryAdd(membru.UID, membru);
                                Associates.TryAdd(UID, assoc);

                            }
                        }
                    }
                }
                // [perf] كان GC.Collect() هنا: GC كامل بيوقّف كل الثريدات (~21ms على heap 150MB). الـ runtime بيجمع لوحده.
            }
            catch (Exception e)
            {
                MyConsole.WriteLine(e.ToString());
            }
        }
    }
}
