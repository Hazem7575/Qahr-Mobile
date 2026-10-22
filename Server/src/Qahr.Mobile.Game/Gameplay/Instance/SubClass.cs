using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.Role.Instance
{
    public class SubClass
    {
        public Database.DBLevExp.Sort ActiveSubclass;
        public ushort StudyPoints = 0;

        public ConcurrentDictionary<
            Database.DBLevExp.Sort,
            Game.MsgServer.MsgSubClass.SubClases> src;

        public SubClass()
        {
            src = new ConcurrentDictionary<
                Database.DBLevExp.Sort,
                Game.MsgServer.MsgSubClass.SubClases>();
        }

        public void CreateSpawn(Client.GameClient client)
        {
            client.Player.ActiveSublass = ActiveSubclass;
            client.Player.SubClassHasPoints = GetHashPoint();
        }

        public uint GetHashPoint()
        {
            uint num = 0;

            foreach (var item in src.Values)
            {
                num += (uint)(
                    Math.Pow(10, (byte)(item.ID - 1)) * item.Phrase
                );
            }

            return num;
        }

        public void UpdateStatus(Client.GameClient client)
        {
            foreach (var item in src.Values)
            {
                switch (item.ID)
                {
                    case Database.DBLevExp.Sort.Apothecary: // Done 1
                        {
                            uint Detoxication = 0;

                            switch (item.Level)
                            {
                                case 1:
                                    Detoxication = 4;   // 8%
                                    break;

                                case 2:
                                    Detoxication = 8;   // 16%
                                    break;

                                case 3:
                                    Detoxication = 12;  // 24%
                                    break;

                                case 4:
                                    Detoxication = 16;  // 32%
                                    break;

                                case 5:
                                    Detoxication = 20;  // 40%
                                    break;

                                case 6:
                                    Detoxication = 24;  // 48%
                                    break;

                                case 7:
                                    Detoxication = 28;  // 56%
                                    break;

                                case 8:
                                    Detoxication = 32;  // 64%
                                    break;

                                case 9:
                                    Detoxication = 36;  // 72%
                                    break;

                            }

                            client.Status.Detoxication += Detoxication;

                            break;
                        }

                    case Database.DBLevExp.Sort.ChiMaster: // Done 2
                        {
                            uint Immunity = 0;

                            switch (item.Level)
                            {
                                case 1:
                                    Immunity = 50;  // 1%
                                    break;

                                case 2:
                                    Immunity = 100;  // 2%
                                    break;

                                case 3:
                                    Immunity = 150;  // 3%
                                    break;

                                case 4:
                                    Immunity = 200;  // 4%
                                    break;

                                case 5:
                                    Immunity = 300;  // 6%
                                    break;

                                case 6:
                                    Immunity = 400;  // 8%
                                    break;

                                case 7:
                                    Immunity = 500; // 10%
                                    break;

                                case 8:
                                    Immunity = 600; // 12%
                                    break;

                                case 9:
                                    Immunity = 750; // 15%
                                    break;
                            }

                            client.Status.Immunity += Immunity;

                            break;
                        }

                    case Database.DBLevExp.Sort.MartialArtist: // Done 3
                        {
                            uint CriticalStrike = 0;

                            switch (item.Level)
                            {
                                case 1:
                                    CriticalStrike = 50;  // 1%
                                    break;

                                case 2:
                                    CriticalStrike = 100;  // 2%
                                    break;

                                case 3:
                                    CriticalStrike = 150;  // 3%
                                    break;

                                case 4:
                                    CriticalStrike = 200;  // 4%
                                    break;

                                case 5:
                                    CriticalStrike = 300;  // 6%
                                    break;

                                case 6:
                                    CriticalStrike = 400;  // 8%
                                    break;

                                case 7:
                                    CriticalStrike = 500; // 10%
                                    break;

                                case 8:
                                    CriticalStrike = 600; // 12%
                                    break;

                                case 9:
                                    CriticalStrike = 750; // 15%
                                    break;
                            }

                            client.Status.CriticalStrike += CriticalStrike;

                            break;
                        }

                    case Database.DBLevExp.Sort.Performer: // Done 4
                        {
                            uint Attack = 0;

                            switch (item.Level)
                            {
                                case 1:
                                    Attack = 0;
                                    break;

                                case 2:
                                    Attack = 100;
                                    break;

                                case 3:
                                    Attack = 150;
                                    break;

                                case 4:
                                    Attack = 200;
                                    break;

                                case 5:
                                    Attack = 250;
                                    break;

                                case 6:
                                    Attack = 300;
                                    break;

                                case 7:
                                    Attack = 350;
                                    break;

                                case 8:
                                    Attack = 400;
                                    break;

                                case 9:
                                    Attack = 500;
                                    break;
                            }

                            client.Status.MagicAttack += Attack;

                            client.Status.MaxAttack += Attack;
                            client.Status.MinAttack += Attack;

                            break;
                        }

                    case Database.DBLevExp.Sort.Sage: // Done 5
                        {
                            uint penetration = 0;

                            switch (item.Level)
                            {
                                case 1:
                                    penetration = 50;   // 1%
                                    break;

                                case 2:
                                    penetration = 100;  // 2%
                                    break;

                                case 3:
                                    penetration = 150;  // 3%
                                    break;

                                case 4:
                                    penetration = 200;  // 4%
                                    break;

                                case 5:
                                    penetration = 300;  // 6%
                                    break;

                                case 6:
                                    penetration = 400;  // 8%
                                    break;

                                case 7:
                                    penetration = 500;  // 10%
                                    break;

                                case 8:
                                    penetration = 600;  // 12%
                                    break;

                                case 9:
                                    penetration = 750;  // 15%
                                    break;
                            }

                            client.Status.Penetration += penetration;

                            break;
                        }

                    case Database.DBLevExp.Sort.Warlock: // Done 6
                        {
                            uint SkillCStrike = 0;

                            switch (item.Level)
                            {
                                case 1:
                                    SkillCStrike = 50;  // 1%
                                    break;

                                case 2:
                                    SkillCStrike = 100;  // 2%
                                    break;

                                case 3:
                                    SkillCStrike = 150;  // 3%
                                    break;

                                case 4:
                                    SkillCStrike = 200;  // 4%
                                    break;

                                case 5:
                                    SkillCStrike = 300;  // 6%
                                    break;

                                case 6:
                                    SkillCStrike = 400;  // 8%
                                    break;

                                case 7:
                                    SkillCStrike = 500; // 10%
                                    break;

                                case 8:
                                    SkillCStrike = 600; // 12%
                                    break;

                                case 9:
                                    SkillCStrike = 750; // 15%
                                    break;
                            }

                            client.Status.SkillCStrike += SkillCStrike;

                            break;
                        }


                    case Database.DBLevExp.Sort.Wrangler: // Done 7
                        {
                            uint MaxHP = 0;

                            switch (item.Level)
                            {
                                case 1:
                                    MaxHP = 50;
                                    break;

                                case 2:
                                    MaxHP = 100;
                                    break;

                                case 3:
                                    MaxHP = 150;
                                    break;

                                case 4:
                                    MaxHP = 200;
                                    break;

                                case 5:
                                    MaxHP = 250;
                                    break;

                                case 6:
                                    MaxHP = 300;
                                    break;

                                case 7:
                                    MaxHP = 400;
                                    break;

                                case 8:
                                    MaxHP = 500;
                                    break;

                                case 9:
                                    MaxHP = 600;
                                    break;
                            }

                            client.Status.MaxHitpoints += MaxHP;

                            break;
                        }
                }
            }
        }

        public bool LearnNewSubClass(
            Database.DBLevExp.Sort type,
            Client.GameClient user,
            ServerSockets.Packet stream)
        {
            return user.Player.SubClass.SendLearn(
                user,
                Game.MsgServer.MsgSubClass.SubClases.Create(type),
                stream
            );
        }

        public bool LearnFixedSubClass(
            Database.DBLevExp.Sort type,
            Client.GameClient user,
            ServerSockets.Packet stream)
        {
            return user.Player.SubClass.SendLearn(
                user,
                Game.MsgServer.MsgSubClass.SubClases.CreateFixed(type),
                stream
            );
        }

        public void SetPhrase(
            Database.DBLevExp.Sort type,
            byte phrase,
            Client.GameClient user,
            ServerSockets.Packet stream)
        {
            Game.MsgServer.MsgSubClass.SubClases record;

            if (src.TryGetValue(type, out record))
            {
                record.Phrase = phrase;

                user.Player.SubClass.SendPromoted(
                    user,
                    record,
                    stream
                );

                user.Equipment.QueryEquipment(
                    user.Equipment.Alternante
                );
            }
        }

        public byte GetClassLevel(Database.DBLevExp.Sort type)
        {
            Game.MsgServer.MsgSubClass.SubClases record;

            if (src.TryGetValue(type, out record))
            {
                return record.Level;
            }

            return 0;
        }

        public byte GetClassPhrase(Database.DBLevExp.Sort type)
        {
            Game.MsgServer.MsgSubClass.SubClases record;

            if (src.TryGetValue(type, out record))
            {
                return record.Phrase;
            }

            return 0;
        }

        public unsafe void AddStudyPoints(
            Client.GameClient client,
            ushort Study,
            ServerSockets.Packet stream)
        {
            StudyPoints = (ushort)Math.Min(
                ushort.MaxValue,
                (int)(StudyPoints + Study)
            );

#if Arabic
            client.SendSysMesage(
                "You received " + Study.ToString() + " StudyPoints.",
                MsgMessage.ChatMode.System
            );
#else
            client.SendSysMesage(
                "You received " + Study.ToString() + " StudyPoints.",
                MsgMessage.ChatMode.System
            );
#endif

            UpdateStudyPacket(client, stream);
        }

        public void RemoveStudy(
            Client.GameClient client,
            ushort Study,
            ServerSockets.Packet stream)
        {
            if (StudyPoints > Study)
            {
                StudyPoints -= Study;
                UpdateStudyPacket(client, stream);
            }
        }

        public unsafe void UpdateStudyPacket(
            Client.GameClient client,
            ServerSockets.Packet stream)
        {
            client.Player.SendString(
                stream,
                Game.MsgServer.MsgStringPacket.StringID.Effect,
                true,
                new string[1] { "zf2-e300" }
            );

            client.Send(
                stream.SubClassCreate(
                    MsgSubClass.Action.Animation,
                    StudyPoints,
                    0,
                    null
                )
            );
        }

        public unsafe bool SendLearn(
            Client.GameClient client,
            Game.MsgServer.MsgSubClass.SubClases item,
            ServerSockets.Packet stream)
        {
            if (!src.ContainsKey(item.ID))
            {
                src.TryAdd(item.ID, item);

                client.Send(
                    stream.SubClassCreate(
                        MsgSubClass.Action.LearnSubClass,
                        item.ID,
                        item.Level,
                        0
                    )
                );

                return true;
            }

            return false;
        }

        public unsafe void SendPromoted(
            Client.GameClient client,
            Game.MsgServer.MsgSubClass.SubClases item,
            ServerSockets.Packet stream)
        {
            client.Send(
                stream.SubClassCreate(
                    MsgSubClass.Action.MartialPromoted,
                    item.ID,
                    item.Phrase,
                    0
                )
            );
        }

        public unsafe void ShowGui(
            Client.GameClient client,
            ServerSockets.Packet stream)
        {
            client.Send(
                stream.SubClassCreate(
                    MsgSubClass.Action.ShowGUI,
                    StudyPoints,
                    0,
                    src.Values.ToArray()
                )
            );
        }

        public override string ToString()
        {
            Database.DBActions.WriteLine writer =
                new Database.DBActions.WriteLine('#');

            writer
                .Add(StudyPoints)
                .Add((byte)ActiveSubclass)
                .Add(src.Count);

            foreach (var prof in src.Values)
            {
                writer
                    .Add((byte)prof.ID)
                    .Add(prof.Level)
                    .Add(prof.Phrase);
            }

            return writer.Close();
        }

        public void Load(string Base_line)
        {
            if (Base_line == "" || Base_line == null)
                return;

            Database.DBActions.ReadLine reader =
                new Database.DBActions.ReadLine(Base_line, '#');

            StudyPoints = reader.Read((ushort)0);

            ActiveSubclass =
                (Database.DBLevExp.Sort)reader.Read((byte)0);

            int count = reader.Read((int)0);

            for (byte x = 0; x < count; x++)
            {
                Game.MsgServer.MsgSubClass.SubClases prof =
                    Game.MsgServer.MsgSubClass.SubClases.Create(
                        (Database.DBLevExp.Sort)reader.Read((byte)0)
                    );

                prof.Level = reader.Read((byte)0);
                prof.Phrase = reader.Read((byte)0);

                if (!src.ContainsKey(prof.ID))
                    src.TryAdd(prof.ID, prof);
            }
        }

        public void UpdateStundyPoints(
            Client.GameClient client,
            ushort amount,
            ServerSockets.Packet stream)
        {
            client.Player.SubClass.StudyPoints += amount;
            UpdateStudyPacket(client, stream);
        }
    }
}
