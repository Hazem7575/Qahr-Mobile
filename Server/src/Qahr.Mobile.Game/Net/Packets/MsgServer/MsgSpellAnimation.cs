using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using System.Collections.Concurrent;

namespace Qahr.Mobile.Game.MsgServer
{
    public class MsgSpellAnimation : IDisposable
    {
        [ProtoContract]
        public class SpellUseProto
        {
            [ProtoMember(1, IsRequired = true)]
            public uint Attacker;

            [ProtoMember(2)]
            public uint Attacked;

            [ProtoMember(3)]
            public ushort X;

            [ProtoMember(4)]
            public ushort Y;

            [ProtoMember(5, IsRequired = true)]
            public ushort SpellID;

            [ProtoMember(6, IsRequired = true)]
            public byte SpellLevel;

            [ProtoMember(7, IsRequired = true)]
            public ushort NextSpell;

            [ProtoMember(8, IsRequired = true)]
            public byte SoulLevel;

            [ProtoMember(9, IsRequired = true)]
            public byte SpellEffect;

            [ProtoMember(10)]
            public Target[] Targets;
        }

        [ProtoContract]
        public class Target
        {
            [ProtoMember(1, IsRequired = true)]
            public uint UID;

            [ProtoMember(2, IsRequired = true)]
            public uint Damage;

            [ProtoMember(3, IsRequired = true)]
            public bool Hit;

            [ProtoMember(4, IsRequired = true)]
            public ulong Flag;

            [ProtoMember(6, IsRequired = true)]
            public ushort newX;

            [ProtoMember(7, IsRequired = true)]
            public ushort newY;

            [ProtoMember(8, IsRequired = true)]
            public uint Unknown;

            [ProtoMember(9, IsRequired = true)]
            public uint TailedBeast;
        }
        public class SpellObj
        {
            public static SpellObj ShallowCopy(SpellObj obj)
            {
                return (SpellObj)obj.MemberwiseClone();
            }

            public uint UID;
            public uint Damage;
            public uint Hit;
            public MsgAttackPacket.AttackEffect Effect;
            public uint MoveX;
            public uint MoveY;
            public uint x1;
            public uint y2;
            public uint xx1;
            public uint yy2;
            public uint xxx1;
            public uint yyy2;
            public uint TailedBeast;
            public uint SpellID = 0;
            public SpellObj()
            {
                Hit = 1;
            }
            public SpellObj(uint _target, uint _damage)
            {
                UID = _target;
                Damage = _damage;
                Hit = 1;
            }
            public SpellObj(uint _target, uint _damage
                , MsgAttackPacket.AttackEffect _effect)
            {
                UID = _target;
                Damage = _damage;
                Effect = _effect;
                Hit = 1;
            }

            public SpellObj(uint _target, uint _damage
               , MsgAttackPacket.AttackEffect _effect, ushort x, ushort y)
            {
                UID = _target;
                Damage = _damage;
                Effect = _effect;
                Hit = 1;
                MoveY = y;
                MoveX = x;
                x1 = xx1 = xx1 = x;
                y2 = yy2 = yyy2 = y;
            }
        }

        public System.Collections.Generic.Queue<SpellObj> Targets;
        public uint UID;
        public uint OpponentUID;
        public ushort X;
        public ushort Y;
        public ushort SpellID;
        public byte LevelSoul;
        public ushort SpellLevel;
        public ushort NextSpell;
        public uint bomb = 0;

        public MsgSpellAnimation()
        {
            Targets = new System.Collections.Generic.Queue<SpellObj>();
        }

        public MsgSpellAnimation(uint _uid, uint oponnent, ushort _x, ushort _y, ushort _spell, ushort _spelllevel, byte _levelsoul, uint boomb = 0)
        {
            Targets = new System.Collections.Generic.Queue<SpellObj>();
            UID = _uid;
            OpponentUID = oponnent;
            X = _x;
            Y = _y;
            SpellID = _spell;
            SpellLevel = _spelllevel;
            LevelSoul = _levelsoul;
            bomb = boomb;
        }
        private unsafe ServerSockets.Packet CreateAnimation(System.Collections.Generic.Queue<SpellObj> Spells, ServerSockets.Packet stream)
        {
            stream.InitWriter();
            var proto = new SpellUseProto()
            {
                Attacker = UID,
                SpellID = global::Qahr.Mobile.Phone.MobileMagic.ToMobile(SpellID), // QahrMobile: mobile magic type (Data\mobile_magic.txt)
                SpellLevel = (byte)global::Qahr.Mobile.Phone.MobileMagic.LevelToMobile(SpellID, SpellLevel), // QahrMobile
                SpellEffect = (byte)bomb,
                NextSpell = global::Qahr.Mobile.Phone.MobileMagic.ToMobile(NextSpell), // QahrMobile
                SoulLevel = LevelSoul
            };
            if (OpponentUID != 0)
                proto.Attacked = OpponentUID;
            else
            {
                proto.X = X;
                proto.Y = Y;
            }
            proto.Targets = new Target[Spells.Count];
            Client.GameClient client = Pool.GamePoll.ContainsKey(UID) ? Pool.GamePoll[UID] : null;
            var array = Spells.ToArray();
            for (int i = 0; i < proto.Targets.Count(); i++)
            {
                proto.Targets[i] = new Target();
                proto.Targets[i].UID = array[i].UID;
                proto.Targets[i].Damage = array[i].Damage;
                proto.Targets[i].Hit = array[i].Hit > 0;
                proto.Targets[i].Flag = (ulong)array[i].Effect;
                proto.Targets[i].newX = (ushort)array[i].MoveX;
                proto.Targets[i].newY = (ushort)array[i].MoveY;
                if (proto.Targets[i].Damage > 0)
                    proto.Targets[i].TailedBeast = client != null && client.Beasts.Activated ? (uint)((10 + (client.Beasts.Phase - 1)) * 150) : 0;
            }
            stream.ProtoBufferSerialize(proto);
            stream.Finalize(Game.GamePackets.MsgMagicEffect);
            return stream;
        }

        public ServerSockets.Packet _Stream;

        public unsafe void SetStream(ServerSockets.Packet stream)
        {
            _Stream = stream;
        }

        private bool TryDequeue(System.Collections.Generic.Queue<SpellObj> Data, out SpellObj obj)
        {
            if (Data.Count != 0)
            {
                obj = Data.Dequeue();
                return true;
            }
            obj = null;
            return false;
        }

        public void JustMe(Client.GameClient user)
        {
            if (Targets.Count <= 30)
            {
                user.Send(CreateAnimation(Targets, _Stream));
            }
            else
            {
                System.Collections.Generic.Dictionary<uint, System.Collections.Generic.Queue<SpellObj>> BigArray = new System.Collections.Generic.Dictionary<uint, System.Collections.Generic.Queue<SpellObj>>();
                var TargetsArray = Targets.ToArray();
                uint count = 0;
                for (int x = 0; x < TargetsArray.Length; x++)
                {
                    if (x % 30 == 0)
                    {
                        count++;
                        BigArray.Add(count, new System.Collections.Generic.Queue<SpellObj>());
                    }
                    BigArray[count].Enqueue(TargetsArray[x]);
                }
                foreach (var small_array in BigArray.Values)
                    user.Send(CreateAnimation(small_array, _Stream));
            }
        }

        public void Send(Client.GameClient user, bool self = true)
        {
            if (Targets.Count <= 30)
            {
                user.Player.View.SendView(CreateAnimation(Targets, _Stream), self);
            }
            else
            {
                System.Collections.Generic.Dictionary<uint, System.Collections.Generic.Queue<SpellObj>> BigArray = new System.Collections.Generic.Dictionary<uint, System.Collections.Generic.Queue<SpellObj>>();
                var TargetsArray = Targets.ToArray();
                uint count = 0;
                for (int x = 0; x < TargetsArray.Length; x++)
                {
                    if (x % 30 == 0)
                    {
                        count++;
                        BigArray.Add(count, new System.Collections.Generic.Queue<SpellObj>());
                    }
                    BigArray[count].Enqueue(TargetsArray[x]);
                }
                foreach (var small_array in BigArray.Values)
                    user.Player.View.SendView(CreateAnimation(small_array, _Stream), self);
            }
        }

        public void SendRole(Client.GameClient user)
        {
            if (Targets.Count <= 30)
            {
                user.Player.View.Role(false, CreateAnimation(Targets, _Stream));
            }
            else
            {
                System.Collections.Generic.Dictionary<uint, System.Collections.Generic.Queue<SpellObj>> BigArray = new System.Collections.Generic.Dictionary<uint, System.Collections.Generic.Queue<SpellObj>>();
                var TargetsArray = Targets.ToArray();
                uint count = 0;
                for (int x = 0; x < TargetsArray.Length; x++)
                {
                    if (x % 30 == 0)
                    {
                        count++;
                        BigArray.Add(count, new System.Collections.Generic.Queue<SpellObj>());
                    }
                    BigArray[count].Enqueue(TargetsArray[x]);
                }
                foreach (var small_array in BigArray.Values)
                    user.Player.View.Role(false, CreateAnimation(small_array, _Stream));
            }
        }

        public void Send(MsgMonster.MonsterRole monster)
        {
            if (Targets.Count <= 30)
            {
                monster.Send(CreateAnimation(Targets, _Stream));
            }
            else
            {
                System.Collections.Generic.Dictionary<uint, System.Collections.Generic.Queue<SpellObj>> BigArray = new System.Collections.Generic.Dictionary<uint, System.Collections.Generic.Queue<SpellObj>>();
                var TargetsArray = Targets.ToArray();
                uint count = 0;
                for (int x = 0; x < TargetsArray.Length; x++)
                {
                    if (x % 30 == 0)// x % 30 = 0
                    {
                        count++;
                        BigArray.Add(count, new System.Collections.Generic.Queue<SpellObj>());
                    }
                    BigArray[count].Enqueue(TargetsArray[x]);
                }
                foreach (var small_array in BigArray.Values)
                    monster.Send(CreateAnimation(small_array, _Stream));
            }
        }

        public void Dispose()
        {
            Targets = null;
        }
    }
}

