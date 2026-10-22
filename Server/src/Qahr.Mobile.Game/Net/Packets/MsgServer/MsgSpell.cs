using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using ProtoBuf;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static class SpellPacket
    {
        [ProtoContract]
        public class MsgMagicInfoProto
        {
            [ProtoMember(1, IsRequired = true)]
            public ulong Experience;

            [ProtoMember(2, IsRequired = true)]
            public ulong ID;

            [ProtoMember(3, IsRequired = true)]
            public ulong Level;

            [ProtoMember(4, IsRequired = true)]
            public ulong Type;

            [ProtoMember(5, IsRequired = true)]
            public ulong UseSoul;

            [ProtoMember(6, IsRequired = true)]
            public ulong Soul;//Missing

            [ProtoMember(7, IsRequired = true)]
            public ulong HashLevelSoul;

            [ProtoMember(8, IsRequired = true)]
            public ulong TotalSeconds;

            [ProtoMember(9)]
            public ulong Member9;

            [ProtoMember(10)]
            public ulong Member10;
        }

        public static unsafe void GetSpell(this ServerSockets.Packet stream, out MsgSpell spell)
        {
            spell = new MsgSpell();
            var Info = new MsgMagicInfoProto();
            Info = stream.ProtoBufferDeserialize(Info);
            spell.Experience = (int)Info.Experience;
            spell.ID = global::Qahr.Mobile.Phone.MobileMagic.ToServer((ushort)Info.ID); // QahrMobile: mobile magic type -> server
            spell.Level = (ushort)Info.Level;
            spell.Type = (ushort)Info.Type;
            spell.UseSoul = (MsgSpell.UseSpellSoulTyp)Info.UseSoul;
            spell.Soul = (byte)Info.Soul;
            spell.HashLevelSoul = (ulong)Info.HashLevelSoul;
        }
        public static unsafe ServerSockets.Packet SpellCreate(this ServerSockets.Packet stream, MsgSpell spell)
        {
            stream.InitWriter();
            var Info = new MsgMagicInfoProto()
            {
                Experience = (ulong)spell.Experience,
                ID = global::Qahr.Mobile.Phone.MobileMagic.ToMobile(spell.ID), // QahrMobile: mobile magic type (Data\mobile_magic.txt)
                Level = global::Qahr.Mobile.Phone.MobileMagic.LevelToMobile(spell.ID, spell.Level), // QahrMobile: capped to the mobile's levels
                Type = spell.Type,
                UseSoul = (ulong)spell.UseSoul,
                Soul = spell.Soul,
                HashLevelSoul = (ulong)spell.HashLevelSoul,
            };
            if (spell.IsSpellWithColdTime == true)
            {
                var Span = spell.ColdTime - DateTime.Now;
                Info.TotalSeconds = (ulong)Span.TotalSeconds * 4;
            }
            stream.ProtoBufferSerialize(Info);
            stream.Finalize(GamePackets.MsgMagicInfo);
            return stream;
        }
    }
    public unsafe class MsgSpell
    {
        public enum UseSpellSoulTyp : uint
        {
            None = 0,
            One = 9,
            Two = 12,
            Three = 16,
            Four = 24,
            Five = 32,
            Six = 40,
            World = 48,
        }
        public DateTime ColdTime = new DateTime();
        public int GetColdTime
        {
            get
            {
                return (ColdTime.AllMilliseconds() - DateTime.Now.AllMilliseconds());
            }
        }
        public bool IsSpellWithColdTime = false;

        public ushort Size;
        public ushort PacketType;
        public int PacketStamp;
        public int Experience;
        public ushort ID;
        public ushort UnKnow;
        public ushort Level;
        public ushort Type;
        public UseSpellSoulTyp UseSoul;
        public byte Soul;
        public ulong HashLevelSoul;
        public DateTime StampSpell = new DateTime();
        public byte SoulLevel
        {
            get
            {
                return _levelHu;
            }
            set
            {
                _levelHu = value;
                if (value > 0) HashLevelSoul |= (1UL << 1);
                if (value > 1) HashLevelSoul |= (1UL << 4);
                if (value > 2) HashLevelSoul |= (1UL << 8);
                if (value > 3) HashLevelSoul |= (1UL << 32);
                if (value > 5) HashLevelSoul |= (1UL << 64);
                if (value > 6) HashLevelSoul |= (1UL << 128);
                if (value > 7) HashLevelSoul |= (1UL << 256);
            }
        }

        public byte PreviousLevel;

        private byte _levelHu;



        public byte UseSpellSoul
        {
            get
            {
                if (UseSoul == UseSpellSoulTyp.One)
                    return 1;
                if (UseSoul == UseSpellSoulTyp.Two)
                    return 2;
                if (UseSoul == UseSpellSoulTyp.Three)
                    return 3;
                if (UseSoul == UseSpellSoulTyp.Four)
                    return 4;
                if (UseSoul == UseSpellSoulTyp.Five)
                    return 5;
                if (UseSoul == UseSpellSoulTyp.Six)
                    return 6;
                if (UseSoul == UseSpellSoulTyp.World)
                    return 7;
                return 0;
            }
            set
            {
                if (value == 1)
                    UseSoul = UseSpellSoulTyp.One;
                else if (value == 2)
                    UseSoul = UseSpellSoulTyp.Two;
                else if (value == 3)
                    UseSoul = UseSpellSoulTyp.Three;
                else if (value == 4)
                    UseSoul = UseSpellSoulTyp.Four;
                else if (value == 5)
                    UseSoul = UseSpellSoulTyp.Five;
                else if (value == 6)
                    UseSoul = UseSpellSoulTyp.Six;
                else if (value == 7)
                    UseSoul = UseSpellSoulTyp.World;
                else
                    UseSoul = UseSpellSoulTyp.None;
            }
        }


        [PacketAttribute(Game.GamePackets.MsgMagicInfo)]
        public unsafe static void HandlerSpell(Client.GameClient client, ServerSockets.Packet stream)
        {

            MsgSpell spell;

            stream.GetSpell(out spell);

            MsgSpell ClientSpell;
            if (client.MySpells.ClientSpells.TryGetValue(spell.ID, out ClientSpell))
            {
                if (ClientSpell.SoulLevel >= spell.UseSpellSoul)
                {
                    ClientSpell.UseSoul = spell.UseSoul;
                    client.MySpells.ClientSpells[spell.ID] = ClientSpell;

                    spell.Type = 3;

                    client.Send(stream.SpellCreate(spell));
                }
            }
        }
    }
}
