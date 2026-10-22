using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Database
{
    public class MagicType : Dictionary<ushort, Dictionary<ushort, MagicType.Magic>>
    {
        public enum WeaponsType : ushort
        {
            Boxing = 0,
            Blade = 410,
            Sword = 420,
            Backsword = 421,
            Hook = 430,
            Whip = 440,
            Mace = 441,
            Axe = 450,
            Hammer = 460,
            Crutch = 470,
            Club = 480,
            Scepter = 481,
            Dagger = 490,
            Prod = 491,
            Fan = 492,
            Flute = 493,
            Glaive = 510,
            Scythe = 511,
            Epee = 520,
            Zither = 521,
            Lute = 522,
            Poleaxe = 530,
            LongHammer = 540,
            Spear = 560,
            Pickaxe = 562,
            Spade = 570,
            Halbert = 580,
            Wand = 561,
            Bow = 500,
            NinjaSword = 601,
            ThrowingKnife = 613,
            MagicSword = 721,
            Shield = 900,
            Other = 422,
            EpicArcher = 606,
            DuneBlade = 608,
            PrayerBeads = 610,
            Rapier = 611,
            Pistol = 612,
            CrossSaber = 614,
            TwistedClaw = 616,
            Nunchaku = 617,
            Fist = 624,
            EpicTao = 620,
            WindFan = 626,
            OceanDominator = 670,
            EpicRapier = 671,
            Flashaxe = 680,
            Stormhammer = 681
        }
        public enum MagicSort
        {
            Type = 0,
            Attack = 1,
            Recruit = 2,
            Cross = 3,
            Sector = 4,
            Bomb = 5,
            AttachStatus = 6,
            DetachStatus = 7,
            Square = 8,
            JumpAttack = 9,
            RandomTransport = 10,
            DispatchXp = 11,
            Collide = 12,
            SerialCut = 13,
            Line = 14,
            AtkRange = 15,
            AttackStatus = 16,
            CallTeamMember = 17,
            RecordTransportSpell = 18,
            Transform = 19,
            AddMana = 20,
            LayTrap = 21,
            Dance = 22,
            CallPet = 23,
            Vampire = 24,
            Instead = 25,
            DecLife = 26,
            Toxic = 27,
            ShurikenVortex = 28,
            CounterKill = 29,

            Spook = 30,
            WarCry = 31,
            Riding = 32,
            Shock = 34,
            ChainBolt = 37,
            StarArrow = 38,
            DragonWhirl = 40,
            RemoveBuffers = 46,
            Tranquility = 47,
            DirectAttack = 48,
            Compasion = 50,
            Auras = 51,
            ShieldBlock = 52,
            Oblivion = 53,
            WhirlwindKick = 54,
            PhysicalSpells = 55,
            ScurvyBomb = 56,
            CannonBarrage = 57,
            BlackSpot = 58,
            Summon = 72,
            BombLine = 60,
            MoveLine = 61,
            AddBlackSpot = 62,
            PirateXpSkill = 63,
            ChargingVortex = 64,
            MortalDrag = 65,
            KineticSpark = 67,
            BladeFlurry = 68,
            SectorBack = 69,
            BreathFocus = 70,
            FatalCross = 71,
            Summons = 72,
            FatalSpin = 73,
            DragonCyclone = 75,
            StraightFist = 76,
            Perimeter = 78,
            AirKick = 79,
            Strike = 81,
            SectorPasive = 82,
            ManiacDance = 83,
            Omnipotence = 85,
            Pounce = 84,
            BurntFrost = 86,

            Rectangle = 87,
            RemoveStamin = 88,
            PetAttachStatus = 89,
            Wildwind = 90,
            SwirlingStorm = 91,
            Pitching = 92,
            ThunderRampage = 94,
            HeavensWrath = 95,
            HellVortex = 98,
            TripleAttack = 99,
            ArrowBlades = 103,
            CrackShot = 104,
            PirateSkill = 107,
            BeastControl = 108,

            Kunpeng = 109,
            SuanniCommand = 110,
            LeeLong1 = 111,
            SupremeLeadership = 112,
            ChaoticDance = 113,
            ChaoticDanceAttack = 114,
            WeaponCombo = 115,
            CrescentChop = 116,
            //Whirlwind = 117,
            ManiacDance1 = 118,
            BurningSun = 119,
            // SunsetShine = 120,
            // BenefitShower = 122,
            BearsCare = 123,
            ApePistol = 124,
            SkyCrack = 125,
            TripleBlasts = 126,
            SuperSpeedGun = 96,
        }
        public class Magic
        {
            public ushort ID;
            public int Second;
            public string Name;
            public MagicSort Type;
            public byte Level;
            public ushort UseMana;
            public float Damage;
            public int GDamage;
            public int Rate = 0;
            public uint Experience;
            public ushort Range;
            public ushort Sector;
            public uint Duration;
            public uint WeaponType;
            public byte UseStamina;
            public uint GiveHitPoints;
            public bool AttackInFly, isTrojanArchiveSkill;
            public int Status;
            public ushort NeedLevel = 0;
            public uint CpsCost = 0;
            public int MaxTargets = 0;
            public byte IncreaseStamin = 0;
            public ushort CoolDown = 100;
            public byte AutoLearn = 0;

            public int ColdTime = 0;
            public bool IsSpellWithColdTime
            {
                get { return ColdTime > 0; }
            }
            public int Damage2;
            public int Damage3;
            public int DamageOnHuman;
            public int DamageOnMonster;
            public int NewDamage;
            public int NewDamage2;
            public int NewDamage3;
            public int StatusData1;
            public int StatusData2;
            public int StatusData0;
            public int Width;//43  
            public int NewDamage4;
            public bool Passive;
            public bool isRuneSkill;
        }
        public static List<Role.Flags.SpellID> RandomSpells = new List<Role.Flags.SpellID>()
        {
           Role.Flags.SpellID.Phoenix, Role.Flags.SpellID.Rage, Role.Flags.SpellID.Boreas, Role.Flags.SpellID.Earthquake
           , Role.Flags.SpellID.Halt,Role.Flags.SpellID.Phoenix
           , Role.Flags.SpellID.Rage, Role.Flags.SpellID.Roamer, Role.Flags.SpellID.Seizer, Role.Flags.SpellID.Snow
           , Role.Flags.SpellID.WideStrike,Role.Flags.SpellID.Phoenix,
            Role.Flags.SpellID.Windstorm, Role.Flags.SpellID.Poison,Role.Flags.SpellID.Phoenix
        };
        public void Load()
        {
            try
            {
                WindowsAPI.IniFile ini = new WindowsAPI.IniFile("MagicEffect.ini");
                if (File.Exists(Program.ServerConfig.DbLocation + "magictype.txt"))
                {
                    string[] Lines = File.ReadAllLines((Program.ServerConfig.DbLocation + "magictype.txt"));

                    foreach (var linen in Lines)
                    {
                        var line = linen.Split(new string[] { "@@" }, StringSplitOptions.RemoveEmptyEntries);
                        Magic spell = new Magic();

                        spell.ID = ushort.Parse(line[1]);
                        spell.isRuneSkill = ini.ReadByte(spell.ID.ToString() + "00", "IsRuneSkill", 0) != 0;
                        spell.AttackInFly = byte.Parse(line[5]) == 0;
                        spell.isTrojanArchiveSkill = (spell.ID >= (ushort)Role.Flags.SpellID.ThunderStrike && spell.ID <= (ushort)Role.Flags.SpellID.AxeShadow);
                        if (!AttackInFlay(spell) && spell.AttackInFly)
                            spell.AttackInFly = AttackInFlay(spell);

                        spell.Type = (MagicSort)byte.Parse(line[2]);

                        spell.Name = line[3];
                        spell.Second = int.Parse(line[7]);

                        spell.Level = byte.Parse(line[8]);
                        spell.UseMana = ushort.Parse(line[9]);
                        spell.GDamage = int.Parse(line[10]);

                        if (spell.ID == (ushort)Role.Flags.SpellID.BloodTide || spell.ID == (ushort)Role.Flags.SpellID.SubstitutionAttack || spell.ID == (ushort)Role.Flags.SpellID.Substitution || spell.ID == (ushort)Role.Flags.SpellID.RevengeTail || spell.ID == (ushort)Role.Flags.SpellID.Duel || spell.ID == (ushort)Role.Flags.SpellID.Sacrifice || spell.ID == (ushort)Role.Flags.SpellID.DragonSwing || spell.ID == (ushort)Role.Flags.SpellID.RiseofTaoism || spell.ID == (ushort)Role.Flags.SpellID.FuryStrike || spell.ID == (ushort)Role.Flags.SpellID.FineRain)
                            spell.Damage = int.Parse(line[10]);
                        else if (spell.ID == (ushort)Role.Flags.SpellID.ToxicFog)
                            spell.Damage = (byte)((double.Parse(line[10], System.Globalization.CultureInfo.InvariantCulture) % 100));
                        else
                        {
                            if ((spell.Type == MagicSort.AddMana || spell.Type == MagicSort.Recruit || spell.Type == MagicSort.Attack) && double.Parse(line[10], System.Globalization.CultureInfo.InvariantCulture) < 10000 || spell.Type == MagicSort.Auras)
                                spell.Damage = int.Parse(line[10]);
                            else
                                spell.Damage = Math.Min((float)(double.Parse(line[10], System.Globalization.CultureInfo.InvariantCulture) % 1000), 500); // [CombatAudit 2026-10-14] cap kept here (PvP default path); monster damage uses the uncapped official value via Calculate.Base.MonsterPower()
                        }
                        if (spell.ID == 12580 || spell.ID == 12590 || spell.ID == 12600)
                            spell.Damage = 100;

                        spell.CoolDown = ushort.Parse(line[33]);
                        if (spell.ID == 10415)
                            spell.CoolDown = 800;
                        if (spell.Damage == 0)
                            spell.Damage = 100;

                        if (spell.ID == 12870)
                            spell.Damage = 50;
                        if (spell.ID == 12990)
                            spell.Damage = 50;
                        if (spell.ID == 13000)
                            spell.Damage = 50;
                        //22850
                        if (spell.ID == 22850)
                            spell.ColdTime = 5000;
                        if (spell.ID == 10930)
                            spell.ColdTime = 0;
                        spell.CoolDown = 0;
                        if (spell.ID == 10490)
                            spell.Damage = 70;
                        if (spell.ID == 3306)
                            spell.Damage = 1;
                        if (spell.ID == 12380)
                            spell.Damage = 10;
                        if (spell.ID == 12080)
                            spell.Range = 1;
                        if (spell.ID == 16510)
                            spell.ColdTime = 6000;
                        if (spell.ID == 16470)
                            spell.ColdTime = 6000;
                        if (spell.ID == (ushort)Role.Flags.SpellID.FlowKnack)
                            spell.Damage = uint.Parse(line[10]);
                        spell.Rate = int.Parse(line[12]);
                        if (spell.ID == (ushort)Role.Flags.SpellID.Celestial)
                            spell.Range = 1;
                        spell.Rate = spell.Rate * 1;
                        if (spell.ID == (ushort)Role.Flags.SpellID.Celestial)
                            spell.Rate = 20;
                        spell.MaxTargets = int.Parse(line[14]);
                        spell.Experience = uint.Parse(line[18]);
                        spell.NeedLevel = ushort.Parse(line[20]);

                        spell.WeaponType = uint.Parse(line[22]);
                        spell.UseStamina = byte.Parse(line[29]);
                        if (spell.ID == (ushort)Role.Flags.SpellID.ToxicFog)
                            spell.Duration = 20;
                        else
                            spell.Duration = uint.Parse(line[13]);
                        spell.CpsCost = ushort.Parse(line[48]);
                        spell.AutoLearn = byte.Parse(line[30]);

                        spell.DamageOnHuman = int.Parse(line[35]);

                        spell.Damage2 = int.Parse(line[36]);
                        if (spell.ID == (ushort)Role.Flags.SpellID.HellVortex)
                            spell.Damage2 = byte.Parse(line[36]);
                        spell.Damage3 = int.Parse(line[37]);

                        spell.DamageOnMonster = int.Parse(line[44]);

                        spell.ColdTime = int.Parse(line[47]);

                        spell.Range = ushort.Parse(line[15]);//14
                        spell.Status = int.Parse(line[16]);
                        if (spell.ID == (ushort)Role.Flags.SpellID.StarChainFire)
                            spell.ColdTime = 5000;
                        spell.Sector = (ushort)(spell.Range * 20);

                        var subtypes = new List<ushort>();
                        if (spell.WeaponType != 0 && spell.WeaponType != 50000)
                        {
                            var subtype1 = (ushort)(spell.WeaponType % 1000);
                            if (subtype1 == 60000)
                                subtype1 = 614;
                            var subtype2 = (ushort)((spell.WeaponType / 1000) % 1000);
                            if (subtype1 > 0) subtypes.Add(subtype1);
                            if (subtype2 > 0) subtypes.Add(subtype2);

                        }
                        if (spell.ID == 12070)
                            spell.Damage = 104;
                        spell.NewDamage = int.Parse(line[45]);
                        spell.NewDamage2 = int.Parse(line[46]);
                        spell.NewDamage3 = int.Parse(line[51]);
                        spell.NewDamage4 = int.Parse(line[52]);
                        spell.Passive = spell.isTrojanArchiveSkill || ini.ReadString(spell.ID.ToString() + "00", "DescEx", "").Contains("Passive");

                        if (this.ContainsKey(spell.ID))
                        {
                            if (!this[spell.ID].ContainsKey(spell.Level))
                                this[spell.ID].Add(spell.Level, spell);
                        }
                        else
                        {
                            this.Add(spell.ID, new Dictionary<ushort, Magic>());
                            this[spell.ID].Add(spell.Level, spell);
                        }

                        if (subtypes.Count != 0)
                        {
                            switch (spell.ID)
                            {
                                case 5010:
                                case 7020:
                                case 1290:
                                case 1260:
                                case 5030:
                                case 5040:
                                case 7000:
                                case 7010:
                                case 7030:
                                case 1250:
                                case 5050:
                                case 5020:
                                case 10490:
                                case (ushort)Role.Flags.SpellID.Windstorm:
                                case (ushort)Role.Flags.SpellID.Roamer:
                                case 1300:
                                case (ushort)Role.Flags.SpellID.MortalStrike:
                                case 12240:
                                case (ushort)Role.Flags.SpellID.Sector:
                                case (ushort)Role.Flags.SpellID.Rectangle:
                                case (ushort)Role.Flags.SpellID.Circle:

                                case (ushort)Role.Flags.SpellID.LeftHook:
                                case (ushort)Role.Flags.SpellID.RightHook:
                                case (ushort)Role.Flags.SpellID.StraightFist:
                                case (ushort)Role.Flags.SpellID.FatalSpin:

                                case (ushort)Role.Flags.SpellID.AirStrike:
                                case (ushort)Role.Flags.SpellID.EarthSweep:
                                case (ushort)Role.Flags.SpellID.Kick:

                                case (ushort)Role.Flags.SpellID.UpSweep:
                                case (ushort)Role.Flags.SpellID.DownSweep:
                                case (ushort)Role.Flags.SpellID.Strike:

                                case (ushort)Role.Flags.SpellID.NormalAttack1:
                                case (ushort)Role.Flags.SpellID.NormalAttack2:
                                case (ushort)Role.Flags.SpellID.NormalAttack3:

                                case (ushort)Role.Flags.SpellID.LeftChop:
                                case (ushort)Role.Flags.SpellID.RightChop:
                                    foreach (var subtype in subtypes)
                                    {
                                        if (!Pool.WeaponSpells.ContainsKey(subtype))
                                            Pool.WeaponSpells.Add(subtype, new List<ushort>());
                                        if (!Pool.WeaponSpells[subtype].Contains(spell.ID))
                                            Pool.WeaponSpells[subtype].Add(spell.ID);
                                    }
                                    break;
                            }
                        }
                    }
                }
                Add((ushort)Role.Flags.SpellID.ShurikenEffect, this[(ushort)Role.Flags.SpellID.ShurikenVortex]);
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }

        }
        public bool AttackInFlay(Magic DBSpell)
        {
            if (DBSpell.ID == 1045 || DBSpell.ID == 1046
    || DBSpell.ID == 1250 || DBSpell.ID == 1260 || DBSpell.ID == 1290
    || DBSpell.ID >= 5010 && DBSpell.ID <= 5050 || DBSpell.ID == 6000
    || DBSpell.ID == 6001 || DBSpell.ID >= 7000 && DBSpell.ID <= 7040
    || DBSpell.ID == 10315 || DBSpell.ID == 10381 || DBSpell.ID == 10415
    || DBSpell.ID == 10490 || DBSpell.ID == 11000 || DBSpell.ID == 11005
    || DBSpell.ID == 11040 || DBSpell.ID == 11070 || DBSpell.ID == 11110
    || DBSpell.ID == 11140 || DBSpell.ID == 11170 || DBSpell.ID == 11180
    || DBSpell.ID == 11190 || DBSpell.ID == 11230 || DBSpell.ID == 6010)
                return false;
            else
                return true;
        }

    }

}
