using Qahr.Mobile.Game;
using Qahr.Mobile.Client;
using Qahr.Mobile.Game.MsgServer;
using ProtoBuf;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Qahr.Mobile
{
    [Flags]
    public enum MsgMedalStorageType : uint
    {
        Load = 0,
        Create = 1,
        Equip = 3,
        UnEquip = 4,
        View = 6,
    }

    [ProtoContract]
    public class MsgMedalStorage
    {
        [ProtoMember(1, IsRequired = true)]
        public MsgMedalStorageType Type;

        [ProtoMember(2, IsRequired = true)]
        public uint Id;

        [ProtoMember(3, IsRequired = true)]
        public uint OK;

        [ProtoMember(4, IsRequired = true)]
        public List<Medal> Medals = new List<Medal>();

        [ProtoContract]
        public class Medal
        {
            [ProtoMember(1, IsRequired = true)]
            public uint u1;

            [ProtoMember(2, IsRequired = true)]
            public uint Type;

            [ProtoMember(3, IsRequired = true)]
            public bool Activate;

            [ProtoMember(4, IsRequired = true)]
            public ulong DateTime;

            [ProtoMember(5, IsRequired = true)]
            public uint DelTime;
        }

        public ServerSockets.Packet ToArray()
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                stream.InitWriter();
                stream.ProtoBufferSerialize(this);
                stream.Finalize(Game.GamePackets.MsgMedalStorage);

                return stream;
            }
        }

        public static implicit operator ServerSockets.Packet(MsgMedalStorage obj)
        {
            return obj.ToArray();
        }
    }

    public class MedalStorage
    {
        public GameClient User;

        public MedalStorage(GameClient user)
        {
            User = user;
        }

        public class Medal
        {
            public uint UserId;
            public uint u1;
            public uint Type;
            public bool Activate;
            public ulong DateTime;

            // Permanent Medal
            public uint DelTime;
        }

        public ConcurrentDictionary<uint, Medal> Medals =
            new ConcurrentDictionary<uint, Medal>();

        // =========================================================
        // Permanent Time
        // =========================================================

        private const uint PermanentTime = uint.MaxValue;

        // =========================================================
        // Timestamp
        // =========================================================

        public static int Timestamp()
        {
            return Convert.ToInt32(
                (DateTime.UtcNow -
                 new DateTime(1970, 1, 1, 0, 0, 0, 0).ToUniversalTime())
                .TotalSeconds
            );
        }

        // =========================================================
        // Build Packet
        // =========================================================

        public MsgMedalStorage GetMsgMedals(
            MsgMedalStorageType type,
            uint UserId,
            params Medal[] Medals)
        {
            MsgMedalStorage msg = new MsgMedalStorage()
            {
                Type = type,
                Id = UserId,
            };

            foreach (var Medal in Medals)
            {
                msg.Medals.Add(new MsgMedalStorage.Medal()
                {
                    Activate = Medal.Activate,

                    // Always Permanent
                    DelTime = PermanentTime,

                    DateTime = Medal.DateTime,
                    Type = Medal.Type,
                    u1 = Medal.u1,
                });
            }

            return msg;
        }

        // =========================================================
        // Loading
        // =========================================================

        public void Loading()
        {
            // Create all medals as Permanent

            Create(1);
            Create(2);
            Create(3);
            Create(4);
            Create(5);
            Create(6);
            Create(7);
            Create(8);
            Create(9);
            Create(10);
            Create(11);
            Create(12);
            Create(13);

            // Send all medals to Client

            User.Send(
                GetMsgMedals(
                    MsgMedalStorageType.Load,
                    0,
                    Medals.Values.ToArray()
                )
            );
        }

        // =========================================================
        // Medal Stats
        // =========================================================

        public uint Immunity;
        public uint Resist;
        public uint Counteraction;
        public uint Parry;

        public void Update()
        {
            Immunity = 0;
            Resist = 0;
            Counteraction = 0;
            Parry = 0;

            Medal medal;

            // Commander
            if (Medals.TryGetValue(2, out medal))
            {
                if (medal.Activate)
                {
                    Counteraction += 10;
                    Immunity += 100;
                    Resist += 500;
                }
            }

            // Renowned Hero
            if (Medals.TryGetValue(3, out medal))
            {
                if (medal.Activate)
                {
                    Parry += 100;
                }
            }

            // =====================================================
            // Add Medal 12 Stats Here If Needed
            // =====================================================
            //
            // if (Medals.TryGetValue(12, out medal))
            // {
            //     if (medal.Activate)
            //     {
            //         ...
            //     }
            // }
            //
            // =====================================================
        }

        public void Update(MsgStatus status)
        {
            status.Counteraction += Counteraction;
            status.Immunity += Immunity;
            status.Resist += Resist;
            status.Parry += Parry;
        }

        // =========================================================
        // Create Permanent Medal
        // =========================================================

        public void Create(uint type, uint delTime = 0)
        {
            if (type >= 1 && type <= 13)
            {
                if (!Medals.ContainsKey(type))
                {
                    Medal medal = new Medal()
                    {
                        UserId = User.Player.UID,

                        Type = type,

                        DateTime = ulong.Parse(
                            DateTime.Now.ToString("yyyyMMddHHmmss")
                        ),

                        // Permanent
                        DelTime = PermanentTime,

                        Activate = false,
                    };

                    Medals.TryAdd(medal.Type, medal);

                    User.Send(
                        GetMsgMedals(
                            MsgMedalStorageType.Create,
                            User.Player.UID,
                            medal
                        )
                    );

                    Update();
                }
            }
        }

        // =========================================================
        // UnEquip
        // =========================================================

        public void UnEquip(MsgMedalStorage msg)
        {
            Medal medal;

            if (Medals.TryGetValue(msg.Id, out medal))
            {
                User.Player.Medal = 0;

                medal.Activate = false;

                // Keep Permanent
                medal.DelTime = PermanentTime;

                msg.OK = 1;

                User.Send(msg);

                Update();

                User.Equipment.QueryEquipment(
                    User.Equipment.Alternante
                );
            }
        }

        // =========================================================
        // Equip
        // =========================================================

        public void Equip(MsgMedalStorage msg)
        {
            foreach (var clear in Medals.Values)
            {
                if (clear.Activate)
                {
                    User.Player.Medal = 0;

                    clear.Activate = false;

                    // Keep Permanent
                    clear.DelTime = PermanentTime;
                }
            }

            Medal medal;

            if (Medals.TryGetValue(msg.Id, out medal))
            {
                User.Player.Medal = msg.Id;

                medal.Activate = true;

                // Keep Permanent
                medal.DelTime = PermanentTime;

                msg.OK = 1;

                User.Send(msg);
            }

            msg.OK = 1;

            User.Send(msg);

            Update();

            User.Equipment.QueryEquipment(
                User.Equipment.Alternante
            );
        }

        // =========================================================
        // View Other Player Medals
        // =========================================================

        public void View(MsgMedalStorage msg)
        {
            Client.GameClient Target;

            if (Pool.GamePoll.TryGetValue(msg.Id, out Target))
            {
                User.Send(
                    GetMsgMedals(
                        MsgMedalStorageType.View,
                        Target.Player.UID,
                        Target.MedalStorage.Medals.Values.ToArray()
                    )
                );
            }
        }

        // =========================================================
        // Load From Database
        // =========================================================

        public void Load(string line)
        {
            Database.DBActions.ReadLine reader =
                new Database.DBActions.ReadLine(line, '/');

            int nCount = reader.Read((int)0);

            for (int count = 0; count < nCount; count++)
            {
                uint nUserId = reader.Read((uint)0);

                uint nu1 = reader.Read((uint)0);

                uint nType = reader.Read((uint)0);

                bool nActivate =
                    reader.Read((byte)0) == 1;

                ulong nDateTime =
                    reader.Read((ulong)0);

                // Read old value from Database
                uint nOldDelTime =
                    reader.Read((uint)0);

                // =================================================
                // Ignore old expiration time.
                // Make every loaded medal Permanent.
                // =================================================

                uint nDelTime = PermanentTime;

                if (nType >= 1 && nType <= 13)
                {
                    if (nActivate)
                    {
                        User.Player.Medal = nType;
                    }

                    Medals.Add(
                        nType,
                        new Medal()
                        {
                            UserId = nUserId,

                            u1 = nu1,

                            Type = nType,

                            Activate = nActivate,

                            DateTime = nDateTime,

                            // Permanent
                            DelTime = nDelTime,
                        }
                    );
                }
            }

            Update();
        }

        // =========================================================
        // Save To Database
        // =========================================================

        public override string ToString()
        {
            var file =
                new Database.DBActions.WriteLine('/');

            file.Add(Medals.Count);

            foreach (var att in Medals.Values)
            {
                file.Add(att.UserId);

                file.Add(att.u1);

                file.Add(att.Type);

                file.Add(
                    (byte)(att.Activate ? 1 : 0)
                );

                file.Add(att.DateTime);

                // Always save as Permanent
                file.Add(PermanentTime);
            }

            return file.Close();
        }

        // =========================================================
        // Process Packet
        // =========================================================

        [Packet(Game.GamePackets.MsgMedalStorage)]
        private unsafe static void Process(
            GameClient user,
            ServerSockets.Packet stream)
        {
            MsgMedalStorage msg =
                new MsgMedalStorage();

            msg =
                stream.ProtoBufferDeserialize<MsgMedalStorage>(msg);

            switch (msg.Type)
            {
                case MsgMedalStorageType.View:

                    user.MedalStorage.View(msg);

                    break;

                case MsgMedalStorageType.Equip:

                    user.MedalStorage.Equip(msg);

                    break;

                case MsgMedalStorageType.UnEquip:

                    user.MedalStorage.UnEquip(msg);

                    break;

                default:

                    Console.WriteLine(
                        "CMsgMedalStorage Type: " +
                        msg.Type
                    );

                    break;
            }
        }
    }
}
