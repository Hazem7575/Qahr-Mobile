using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Role.Instance
{
    public class Beasts
    {
        public Client.GameClient Owner;
        public Beasts(Client.GameClient client)
        {
            Owner = client;
            FixedLevel = 1;
        }
        [Flags]
        public enum HammerRunes : uint
        {
            None = 0,
            OneRune = 1,
            TwoRune = 3,
            ThreeRune = 7,
            FourRune = 15,
            FiveRune = 31,
            SixRune = 45,
        }
        public bool Activated;
        public uint Points;
        private byte _fixedlevel;
        public uint Flag;
        public byte FixedLevel
        {
            get
            {
                return _fixedlevel;
            }
            set
            {
                while (value > 10)
                {
                    value = 1;

                }
                if (Level == 100 && Points >= 48000)
                {
                    value = 9;
                }
                _fixedlevel = value;
            }
        }
        public byte GetCount()
        {
            byte Count = 0;
            Count += 5;
            Database.BeastsAtrribute.Attribute attr;
            if (Database.BeastsAtrribute.Attributes.TryGetValue(Level, out attr))
            {
                Count += attr.YellowRuneSlots;
            }
            if ((this.Flag & (uint)HammerRunes.OneRune) == (uint)HammerRunes.OneRune)
                Count++;
            if ((this.Flag & (uint)HammerRunes.TwoRune) == (uint)HammerRunes.TwoRune)
                Count++;
            if ((this.Flag & (uint)HammerRunes.ThreeRune) == (uint)HammerRunes.ThreeRune)
                Count++;
            if ((this.Flag & (uint)HammerRunes.FourRune) == (uint)HammerRunes.FourRune)
                Count++;
            if ((this.Flag & (uint)HammerRunes.FiveRune) == (uint)HammerRunes.FiveRune)
                Count++;
            if ((this.Flag & (uint)HammerRunes.SixRune) == (uint)HammerRunes.SixRune)
                Count++;
            return Count;
        }
        private byte _level;
        public byte Level
        {
            get
            {
                return _level;
            }
            set
            {
                while (value > Database.BeastsAtrribute.Attributes.Count)
                {
                    value--;
                    FixedLevel++;
                }
                _level = value;
            }
        }
        public uint FruitToday = 0;
        public uint TotalPoints
        {
            get
            {
                uint points = Points;
                for (byte i = 1; i < Level; i++)
                    points += Database.BeastsAtrribute.Attributes[i].RequiredPoints;
                return points;
            }
            set
            {
                if (Activated)
                    Level = 1;
                for (byte i = 0; i < Database.BeastsAtrribute.Attributes.Count; i++)
                    if (value >= Database.BeastsAtrribute.Attributes[(byte)(i + 1)].RequiredPoints)
                    {
                        value -= Database.BeastsAtrribute.Attributes[(byte)(i + 1)].RequiredPoints;
                        Level++;
                    }
                    else break;
                Points += value;
                while (Points >= Database.BeastsAtrribute.Attributes.Values.LastOrDefault().RequiredPoints)
                {
                    Points -= Database.BeastsAtrribute.Attributes.Values.LastOrDefault().RequiredPoints;
                    FixedLevel++;
                }
            }
        }
        public byte Phase
        {
            get
            {
                return Activated ? (byte)(1 + (Level / 10)) : (byte)0;
            }
        }
    }
}