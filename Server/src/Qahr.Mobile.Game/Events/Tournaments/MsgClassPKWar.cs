using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgTournaments
{
    public class MsgClassPKWar
    {
        public const ushort MapID = 1770;

        public const string FilleName = "\\ClassPkWar.ini";

        public enum TournamentType : byte
        {
            Trojan = 0,
            Warrior = 1,
            Archer = 2,
            Ninja = 3,
            Monk = 4,
            Pirate = 5,
            Fire = 6,
            Water = 7,
            LongLee = 8,
            WindWalker = 9,
            Thunderstriker = 10,
            Count = 11
        }
        public enum TournamentLevel : byte
        {
            Level_1_99 = 0,
            Level_100_119 = 1,
            Level_120_130 = 2,
            Level_130_140 = 3,
            Count = 4
        }


        public War[][] PkWars;
        public ProcesType Proces;

        public MsgClassPKWar(ProcesType _Proces)
        {
            Proces = _Proces;

            PkWars = new War[(byte)TournamentType.Count][];

            for (TournamentType i = TournamentType.Trojan; i < TournamentType.Count; i++)
            {
                PkWars[(byte)i] = new War[(byte)TournamentLevel.Count];

                for (TournamentLevel x = TournamentLevel.Level_1_99; x < TournamentLevel.Count; x++)
                {
                    PkWars[(byte)i][(byte)x] = new War(i, x, ProcesType.Dead);
                }
            }
        }
        internal void Start()
        {
            // stop hook: close the last run once every war is won or past its 1-hour round
            if (Proces != ProcesType.Dead && PkWars.All(t => t.All(w => w.Proces == ProcesType.Dead || DateTime.Now > w.FinishTimer.AddHours(1))))
                Stop();
            if (Proces == ProcesType.Dead)
            {
                for (TournamentType i = TournamentType.Trojan; i < TournamentType.Count; i++)
                {
                    PkWars[(byte)i] = new War[(byte)TournamentLevel.Count];

                    for (TournamentLevel x = TournamentLevel.Level_1_99; x < TournamentLevel.Count; x++)
                    {
                        PkWars[(byte)i][(byte)x] = new War(i, x, ProcesType.Dead);
                    }
                }
                foreach (var tournament in PkWars)
                {
                    foreach (var war in tournament)
                    {
                        war.Start(Pool.ServerMaps[MapID]);
                    }
                }
                // one invite for the whole event (was one per war)
                foreach (var client in Pool.GamePoll.Values)
                {
                    client.Player.MessageBox("", new Action<Client.GameClient>(p => p.Teleport(Places.TwinCity, 0)), null, 60, MsgServer.MsgStaticMessage.Messages.ClassPk);
                }
                Proces = ProcesType.Idle;
            }
        }
        internal void Stop()
        {
            if (Proces != ProcesType.Dead)
            {
                foreach (var tournament in PkWars)
                {
                    foreach (var war in tournament)
                    {
                        war.Stop();
                    }
                }
                Proces = ProcesType.Dead;
            }
        }

        internal MsgServer.MsgUpdate.Flags ObtainAura(TournamentType typ)
        {
            switch (typ)
            {
                case TournamentType.Trojan: return MsgServer.MsgUpdate.Flags.TopTrojan;
                case TournamentType.Warrior: return MsgServer.MsgUpdate.Flags.TopWarrior;
                case TournamentType.Archer: return MsgServer.MsgUpdate.Flags.TopArcher;
                case TournamentType.Ninja: return MsgServer.MsgUpdate.Flags.TopNinja;
                case TournamentType.Monk: return MsgServer.MsgUpdate.Flags.TopMonk;
                case TournamentType.Pirate: return MsgServer.MsgUpdate.Flags.TopPirate;
                case TournamentType.Fire: return MsgServer.MsgUpdate.Flags.TopFireTaoist;
                case TournamentType.Water: return MsgServer.MsgUpdate.Flags.TopWaterTaoist;
                case TournamentType.LongLee: return MsgServer.MsgUpdate.Flags.TopDragonLee;
                case TournamentType.WindWalker: return MsgServer.MsgUpdate.Flags.TopWindWalker;
                case TournamentType.Thunderstriker: return MsgServer.MsgUpdate.Flags.TopThunderstriker;
            }
            return MsgServer.MsgUpdate.Flags.Normal;
        }

        public War GetMyWar(Client.GameClient client)
        {
            foreach (var tournament in PkWars)
            {
                foreach (var war in tournament)
                {
                    if (war.DinamicID == client.Player.DynamicID)
                        return war;
                }
            }
            return null;
        }
        internal static TournamentType GetMyTournamentType(Client.GameClient client)
        {
            if (Database.AtributesStatus.IsTrojan(client.Player.Class))
                return TournamentType.Trojan;
            if (Database.AtributesStatus.IsWarrior(client.Player.Class))
                return TournamentType.Warrior;
            if (Database.AtributesStatus.IsArcher(client.Player.Class))
                return TournamentType.Archer;
            if (Database.AtributesStatus.IsNinja(client.Player.Class))
                return TournamentType.Ninja;
            if (Database.AtributesStatus.IsMonk(client.Player.Class))
                return TournamentType.Monk;
            if (Database.AtributesStatus.IsPirate(client.Player.Class))
                return TournamentType.Pirate;
            if (Database.AtributesStatus.IsWater(client.Player.Class))
                return TournamentType.Water;
            if (Database.AtributesStatus.IsFire(client.Player.Class))
                return TournamentType.Fire;
            if (Database.AtributesStatus.IsLee(client.Player.Class))
                return TournamentType.LongLee;
            if (Database.AtributesStatus.IsWindWalker(client.Player.Class))
                return TournamentType.WindWalker;
            if (Database.AtributesStatus.IsThunderStriker(client.Player.Class))
                return TournamentType.Thunderstriker;

            return TournamentType.Count;
        }
        internal static TournamentLevel GetMyTournamentLevel(Client.GameClient client)
        {

            if (client.Player.Level >= 130)
                return TournamentLevel.Level_130_140;
            return TournamentLevel.Count;
        }
        internal ProcesType GetWar(Client.GameClient client, out War mywar)
        {
            var typ = GetMyTournamentType(client);
            var level = GetMyTournamentLevel(client);
            var tournament = PkWars[(byte)typ];
            foreach (var war in tournament)
            {
                if (war.Typ == typ && war.Level == level)
                {
                    mywar = war;
                    return war.Proces;
                }
            }
            mywar = null;
            return ProcesType.Dead;
        }
        internal void LoginClient(Client.GameClient client)
        {
            foreach (var tournament in PkWars)
            {
                foreach (var war in tournament)
                {
                    if (war.Winner == client.Player.UID)
                    {
                        client.Player.AddFlag(war.LastFlag, Role.StatusFlagsBigVector32.PermanentFlag, false);
                    }
                }
            }
        }
        internal void Save()
        {
            Database.DBActions.Write writer = new Database.DBActions.Write(FilleName);
            foreach (var tournament in PkWars)
            {
                foreach (var war in tournament)
                {
                    Database.DBActions.WriteLine line = new Database.DBActions.WriteLine('/');
                    line.Add((byte)war.Typ).Add((byte)war.Level).Add(war.Winner).Add((int)war.LastFlag);
                    writer.Add(line.Close());
                }
            }
            writer.Execute(Database.DBActions.Mode.Open);
        }
        internal void Load()
        {
            Database.DBActions.Read reader = new Database.DBActions.Read(FilleName);
            if (reader.Reader())
            {
                for (int x = 0; x < reader.Count; x++)
                {
                    Database.DBActions.ReadLine line = new Database.DBActions.ReadLine(reader.ReadString(""), '/');
                    byte typ = line.Read((byte)0);
                    byte level = line.Read((byte)0);
                    uint Winner = line.Read((uint)0);
                    MsgServer.MsgUpdate.Flags LastFlag = (MsgServer.MsgUpdate.Flags)line.Read((int)0);

                    PkWars[typ][level].Winner = Winner;
                    PkWars[typ][level].LastFlag = LastFlag;
                }
            }
        }
        public class War
        {
            public uint Winner = 0;
            public byte ReceiveReward = 0;
            public MsgServer.MsgUpdate.Flags LastFlag = 0;

            public TournamentLevel Level;
            public TournamentType Typ;
            public ProcesType Proces;
            public uint DinamicID;
            public DateTime FinishTimer = new DateTime();

            public War(TournamentType _typ, TournamentLevel _level, ProcesType proces)
            {
                Level = _level;
                Proces = proces;
                Typ = _typ;
            }
            public void Start(Role.GameMap map)
            {
                if (Proces == ProcesType.Dead)
                {


                    Proces = ProcesType.Alive;
                    FinishTimer = DateTime.Now.AddMinutes(30);
                    DinamicID = map.GenerateDynamicID();
                }
            }
            internal void Stop()
            {
                Proces = ProcesType.Dead;
            }
            public bool CheckJoin()
            {
                if (Proces == ProcesType.Dead)
                    return false;
                if (DateTime.Now < FinishTimer)
                    return true;
                else
                {
                    Proces = ProcesType.Alive;
                    return false;
                }
            }
            public bool IsFinish(Client.GameClient client)
            {
                if (Proces == ProcesType.Alive)
                {
                    var arrayMap = client.Map.Values.Where(p => p.Player.DynamicID == DinamicID && p.Player.Alive).ToArray();
                    if (arrayMap.Length == 1)
                    {
                        return true;
                    }
                }
                return false;
            }
            public void GetMyReward(Client.GameClient client, ServerSockets.Packet stream)
            {
                if (IsFinish(client))
                {
                    Proces = ProcesType.Dead;

                    //Remove old winner -----------------------
                    foreach (var user in Pool.GamePoll.Values)
                    {
                        if (user.Player != null)
                        {
                            if (user.Player.UID == Winner)
                            {
                                user.Player.RemoveFlag(LastFlag);
                            }
                        }
                    }
                    //------------------------------------------

                    var aura = MsgSchedules.ClassPkWar.ObtainAura(Typ);
                    if (aura != MsgServer.MsgUpdate.Flags.Normal)
                        client.Player.AddFlag(aura, Role.StatusFlagsBigVector32.PermanentFlag, false);

                    //  client.Inventory.AddItemWitchStack(3006749, 0, 2500, stream, false); // 2.500 TwinCity[Coin]

                    client.Player.ConquerPoints += 1; // 1 CP

                    LastFlag = aura;
                    Winner = client.Player.UID;

                    MsgSchedules.SendSysMesage("Congratulations, to " + client.Player.Name + " Won " + Typ.ToString() + " Class PK War (" + Level.ToString() + ") , he received Top " + Typ.ToString() + " and 1 CP.", MsgServer.MsgMessage.ChatMode.TopLeftSystem, MsgServer.MsgMessage.MsgColor.white);

                    client.Teleport(Places.TwinCity, 0);
                }
            }
        }
    }
}
