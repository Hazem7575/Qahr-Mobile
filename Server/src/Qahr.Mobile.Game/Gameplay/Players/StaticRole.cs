using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.Role
{
    public class StaticRole : IMapObj
    {
        public Role.StatusFlagsBigVector32 BitVector;
        public StaticRole(ushort x, ushort y, string _Namee = "Flag", uint FlagMesh = 8910)
        {
            AllowDynamic = false;
            Name = _Namee;
            X = x;
            Y = y;
            UID = (uint)(x * 1000 + y);
            Mesh = FlagMesh;
            Level = 1;
            ObjType = MapObjectType.StaticRole;
            BitVector = new Role.StatusFlagsBigVector32(32 * 18);
        }
        public Role.GameMap GMap
        {

            get { return Pool.ServerMaps[Map]; }
        }
        public bool AllowDynamic { get; set; }
        public uint IndexInScreen { get; set; }
        public bool IsTrap() { return false; }
        public unsafe string Name { get; set; }
        public unsafe uint Mesh { get; set; }
        public unsafe uint UID { get; set; }
        public unsafe byte Level { get; set; }
        public unsafe uint HitPoints { get; set; }
        public uint Map { get; set; }
        public uint DynamicID { get; set; }
        public Role.MapObjectType ObjType { get; set; }
        public ushort X { get; set; }
        public ushort Y { get; set; }
        public unsafe Role.Flags.ConquerAction Action { get; set; }
        public unsafe Role.Flags.ConquerAngle Facing { get; set; }

        public uint SetBy = 0;
        public bool Viable = false;

        public bool QuestionMark { get { return Mesh == 767; } }
        public Game.MsgServer.MsgRacePotion.RaceItemType Type
        {
            get
            {
                switch (Mesh)
                {
                    case 760: return Game.MsgServer.MsgRacePotion.RaceItemType.GuardPotion;
                    case 761: return Game.MsgServer.MsgRacePotion.RaceItemType.FrozenTrap;
                    case 762: return Game.MsgServer.MsgRacePotion.RaceItemType.SluggishPotion;
                    case 763: return Game.MsgServer.MsgRacePotion.RaceItemType.DizzyHammer;
                    case 764: return Game.MsgServer.MsgRacePotion.RaceItemType.RestorePotion;
                    case 765: return Game.MsgServer.MsgRacePotion.RaceItemType.ScreamBomb;
                    case 766: return Game.MsgServer.MsgRacePotion.RaceItemType.TransformItem;
                    case 768: return Game.MsgServer.MsgRacePotion.RaceItemType.SpiritPotion;
                    case 769: return Game.MsgServer.MsgRacePotion.RaceItemType.ExcitementPotion;
                    case 770: return Game.MsgServer.MsgRacePotion.RaceItemType.ChaosBomb;
                    default:
                        {

                            int val = (int)Game.MsgServer.MsgRacePotion.RaceItemType.TransformItem;
                            while (val == (int)Game.MsgServer.MsgRacePotion.RaceItemType.TransformItem || val == (int)Game.MsgServer.MsgRacePotion.RaceItemType.FrozenTrap)
                                val = Pool.GetRandom.Next((int)Game.MsgServer.MsgRacePotion.RaceItemType.ChaosBomb, (int)Game.MsgServer.MsgRacePotion.RaceItemType.SuperExcitementPotion);
                            return (Game.MsgServer.MsgRacePotion.RaceItemType)val;
                        }
                }
            }
        }

        private static Tuple<uint, string, ushort>[] raceItems = new[]
            {
#if Arabic
                new  Tuple<uint, string, ushort>(760,"GuardPotion",1),
                new  Tuple<uint, string, ushort>(762,"SluggishPotion",3),
                new  Tuple<uint, string, ushort>(763,"DizzyHammer",1),
                new  Tuple<uint, string, ushort>(764,"RestorePotion",3 ),
                new  Tuple<uint, string, ushort>(765,"ScreamBomb",1 ),
                new  Tuple<uint, string, ushort>(766,"SuperExclamationMark",1),
                new  Tuple<uint, string, ushort>(767,"SuperQuestionMark",1),
                new  Tuple<uint, string, ushort>(768,"SpiritPotion",1),
                new  Tuple<uint, string, ushort>(769,"ExcitementPotion",1),
                new  Tuple<uint, string, ushort>(770,"ChaosBomb",1)
#else
                new  Tuple<uint, string, ushort>(760,"GuardPotion",1),
                new  Tuple<uint, string, ushort>(762,"SluggishPotion",3),
                new  Tuple<uint, string, ushort>(763,"DizzyHammer",1),
                new  Tuple<uint, string, ushort>(764,"RestorePotion",3 ),
                new  Tuple<uint, string, ushort>(765,"ScreamBomb",1 ),
                new  Tuple<uint, string, ushort>(766,"SuperExclamationMark",1),
                new  Tuple<uint, string, ushort>(767,"SuperQuestionMark",1),
                new  Tuple<uint, string, ushort>(768,"SpiritPotion",1),
                new  Tuple<uint, string, ushort>(769,"ExcitementPotion",1),
                new  Tuple<uint, string, ushort>(770,"ChaosBomb",1)
#endif
              
            };


        public void Pick()
        {
            Viable = true;
            var itemData = raceItems[Pool.GetRandom.Next(raceItems.Length)];
            Mesh = itemData.Item1;
            Name = itemData.Item2;
            Level = (byte)itemData.Item3;

        }
        public void DoFrozenTrap(uint setter)
        {
            SetBy = setter;
            Viable = true;

            Mesh = 761;
#if Arabic
             Name = "Frozen Trap";
#else
            Name = "Frozen Trap";
#endif

            Level = 1;

        }

        public bool AddFlag(Game.MsgServer.MsgUpdate.Flags Flag, int Secounds, bool RemoveOnDead, int StampSecounds = 0)
        {
            if (!BitVector.ContainFlag((int)Flag))
            {
                BitVector.TryAdd((int)Flag, Secounds, RemoveOnDead, StampSecounds);
                UpdateFlagOffset();
                return true;
            }
            return false;
        }
        public bool RemoveFlag(Game.MsgServer.MsgUpdate.Flags Flag, Role.GameMap map)
        {
            if (BitVector.ContainFlag((int)Flag))
            {
                BitVector.TryRemove((int)Flag);
                UpdateFlagOffset();
                return true;
            }
            return false;
        }
        private unsafe void UpdateFlagOffset(bool SendScreem = true)
        {
            if (SendScreem)
                SendUpdate(BitVector.bits, Game.MsgServer.MsgUpdate.DataType.StatusFlag);
        }
        public unsafe void SendUpdate(uint[] Value, Game.MsgServer.MsgUpdate.DataType datatype)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                Game.MsgServer.MsgUpdate packet = new Game.MsgServer.MsgUpdate(stream, UID, 1);
                stream = packet.Append(stream, datatype, Value);
                stream = packet.GetArray(stream);
                Send(stream);
            }
        }
        public bool UpdateFlag(Game.MsgServer.MsgUpdate.Flags Flag, int Secounds, bool SetNewTimer, int MaxTime)
        {
            return BitVector.UpdateFlag((int)Flag, Secounds, SetNewTimer, MaxTime);
        }

        public unsafe bool MoveTo(ushort _x, ushort _y)
        {
            if (_x == X && _y == Y)
            {
                return false;
            }
            Role.Flags.ConquerAngle dir = Role.Core.GetAngle(X, Y, _x, _y);
            ushort WalkX = X; ushort WalkY = Y;
            Role.Core.IncXY(dir, ref WalkX, ref WalkY);

            GMap.View.MoveTo<Role.IMapObj>(this, WalkX, WalkY);
            X = WalkX;
            Y = WalkY;

            WalkQuery walk = new WalkQuery()
            {
                Direction = (byte)dir,
                Running = 2,
                UID = UID,
                MapID = GMap.ID
            };
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                Send(stream.MovementCreate(walk));
            }
            return true;
        }


        public unsafe void SendString(ServerSockets.Packet stream, Game.MsgServer.MsgStringPacket.StringID id, params string[] args)
        {
            Game.MsgServer.MsgStringPacket packet = new Game.MsgServer.MsgStringPacket();
            packet.ID = id;
            packet.UID = UID;

            packet.Strings = args;
            Send(stream.StringPacketCreate(packet));
        }

        public unsafe ServerSockets.Packet GetArray(ServerSockets.Packet stream, bool view)
        {

            stream.InitWriter();
            var proto = new Role.Player.SpawnPacketProto()
            {
                UID = UID,
                Mesh = Mesh,
                MonsterLevel = Level,
                Hitpoints = 0,
                X = X,
                Y = Y,
                Action = (ushort)Action,
                Level = Level,
                Name = System.Text.Encoding.UTF8.GetBytes(Name ?? "")
            };
            proto.StatusFlags = new ulong[BitVector.bits.Length];
            for (int x = 0; x < BitVector.bits.Length; x++)
                proto.StatusFlags[x] = BitVector.bits[x];

            stream.ProtoBufferSerialize(proto);
            stream.Finalize(Game.GamePackets.MsgPlayer);


            return stream;
        }

        public bool Alive { get { return HitPoints > 0; } }

        public void RemoveRole(IMapObj obj)
        {

        }

        public unsafe void Send(ServerSockets.Packet msg)
        {
            foreach (var obj in GMap.View.Roles(MapObjectType.Player, X, Y, p => CanSee(p)))
                obj.Send(msg);
        }
        public bool CanSee(Role.IMapObj obj)
        {
            if (obj.Map != Map)
                return false;
            if (obj.DynamicID != DynamicID)
                return false;
            if (obj.UID == UID)
                return false;
            return Role.Core.GetDistance(obj.X, obj.Y, X, Y) <= 18;
        }
    }
}
