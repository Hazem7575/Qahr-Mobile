using Qahr.Mobile.Role.Instance;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Qahr.Mobile.Database
{
    public class MythSoulAttributes
    {
        public static Dictionary<Type, Dictionary<uint, Attribute>> Attributes = new Dictionary<Type, Dictionary<uint, Attribute>>();
        public enum Type
        {
            Venom = 1,
            Bloodthirst = 4,
            Etherial = 8,
            Elan = 17,
            Solid = 18,
            Meditation = 19,
            SWeep = 20,
            Superpower = 21,
            Oracle = 22,
            Numb = 23,
            Frost = 24,
            Bash = 25,
            Luck = 26,
            Crack = 27,
            Vigour = 28,
            Demolition = 29,
            Discipline = 30,
            Safeguard = 31,
        }
        public class Attribute
        {
            public Type Type;
            public uint Level, Seconds, Damage;
            public double Rate;
        }
        public static void Load()
        {
            if (File.Exists(Program.ServerConfig.DbLocation + "MythSoulValue.ini"))
            {
                string[] Lines = File.ReadAllLines(Program.ServerConfig.DbLocation + "MythSoulValue.ini");
                foreach (var line in Lines)
                {
                    var spilitline = line.Split(new string[] { "@@" }, StringSplitOptions.RemoveEmptyEntries);
                    Attribute attr = new Attribute();
                    attr.Type = (Type)uint.Parse(spilitline[0]);
                    attr.Level = uint.Parse(spilitline[1]);
                    attr.Rate = double.Parse(spilitline[2], System.Globalization.CultureInfo.InvariantCulture);
                    attr.Damage = uint.Parse(spilitline[3]);
                    attr.Seconds = uint.Parse(spilitline[4]);
                    if (!Attributes.ContainsKey(attr.Type))
                        Attributes.Add(attr.Type, new Dictionary<uint, Attribute>());
                    Attributes[attr.Type].Add(attr.Level, attr);
                }
            }
        }

    }
    public class MythTable
    {

        public static double GetEdge(uint Level, Qahr.Mobile.Client.GameClient client)
        {

            switch (Level)
            {
                case 1: return 100;
                case 2: return 200;
                case 3: return 300;
                case 4: return 400;
                case 5: return 500;
                case 6: return 600;
                case 7: return 700;
                case 8: return 800;
                case 9: return 820;
                case 10: return 840;
                case 11: return 860;
                case 12: return 880;
                case 13: return 900;
                case 14: return 920;
                case 15: return 940;
                case 16: return 960;
                case 17: return 980;
                case 18: return 1000;
                case 19: return 1020;
                case 20: return 1040;
                case 21: return 1060;
                case 22: return 1080;
                case 23: return 1100;
                case 24: return 1120;
                case 25: return 1140;
                case 26: return 1160;
                case 27: return 1180;
                case 28: return 1200;
                case 29: return 1220;
                case 30: return 1240;
                case 31: return 1260;
                case 32: return 1280;
                case 33: return 1300;
                case 34: return 1320;
                case 35: return 1340;
                case 36: return 1360;
                case 37: return 1380;
                case 38: return 1400;
                case 39: return 1420;
                case 40: return 1440;
                case 41: return 1460;
                case 42: return 1480;

            }
            return 0;
        }
        public static double GetEthereal(uint Level, Qahr.Mobile.Client.GameClient client)
        {

            switch (Level)
            {
                case 1: return 3;
                case 2: return 4;
                case 3: return 5;
                case 4: return 6;
                case 5: return 7;
                case 6: return 8;
                case 7: return 9;
                case 8: return 10;
                case 9: return 11;
                case 10: return 12;
                case 11: return 13;
                case 12: return 13.4;
                case 13: return 13.8;
                case 14: return 14.2;
                case 15: return 14.6;
                case 16: return 15;
                case 17: return 15.4;
                case 18: return 15.8;
                case 19: return 16.2;
                case 20: return 16.6;
                case 21: return 16.8;
                case 22: return 17;
                case 23: return 17.2;
                case 24: return 17.3;
                case 25: return 17.4;
                case 26: return 17.5;
                case 27: return 17.6;
                case 28: return 17.7;
                case 29: return 17.8;
                case 30: return 17.9;
                case 31: return 18;
                case 32: return 18.1;
                case 33: return 18.2;
                case 34: return 18.3;
                case 35: return 18.4;
                case 36: return 18.5;
                case 37: return 18.6;
                case 38: return 18.7;
                case 39: return 18.8;
                case 40: return 18.9;
                case 41: return 19;
                case 42: return 19.1;

            }
            return 0;
        }
        public static double GetHawkeye(uint Level, Qahr.Mobile.Client.GameClient client)
        {

            switch (Level)
            {
                case 1: return 3;
                case 2: return 4;
                case 3: return 5;
                case 4: return 6;
                case 5: return 7;
                case 6: return 8;
                case 7: return 9;
                case 8: return 10;
                case 9: return 11;
                case 10: return 12;
                case 11: return 13;
                case 12: return 13.4;
                case 13: return 13.8;
                case 14: return 14.2;
                case 15: return 14.6;
                case 16: return 15;
                case 17: return 15.4;
                case 18: return 15.8;
                case 19: return 16.2;
                case 20: return 16.6;
                case 21: return 16.8;
                case 22: return 17;
                case 23: return 17.2;
                case 24: return 17.3;
                case 25: return 17.4;
                case 26: return 17.5;
                case 27: return 17.6;
                case 28: return 17.7;
                case 29: return 17.8;
                case 30: return 17.9;
                case 31: return 18;
                case 32: return 18.1;
                case 33: return 18.2;
                case 34: return 18.3;
                case 35: return 18.4;
                case 36: return 18.5;
                case 37: return 18.6;
                case 38: return 18.7;
                case 39: return 18.8;
                case 40: return 18.9;
                case 41: return 19;
                case 42: return 19.1;

            }
            return 0;
        }

        public static double GetDiscipline(uint Level, Qahr.Mobile.Client.GameClient client)
        {

            switch (Level)
            {
                case 1: return 50;
                case 2: return 100;
                case 3: return 150;
                case 4: return 200;
                case 5: return 350;
                case 6: return 500;
            }
            return 0;
        }
        public static double GetSafeguard(uint Level, Qahr.Mobile.Client.GameClient client)
        {

            switch (Level)
            {
                case 1: return 50;
                case 2: return 100;
                case 3: return 150;
                case 4: return 200;
                case 5: return 350;
                case 6: return 500;
            }
            return 0;
        }
        public static void GetMythSouls(Qahr.Mobile.Client.GameClient client)
        {
            byte SkySoarer = 0;
            if (client.Player.Owner.Rune.IsEquipped("Sky Soarer", ref SkySoarer))
            {

                byte rate = (byte)(5 * SkySoarer);
                switch (SkySoarer)
                {

                    case 9: rate = 50; break;
                }
                // client.Player.Owner.SendSysMesage("Rune Sky Soarer Activated: Waiting for HitRate " + (uint)(GetEthereal(client.Status.EtherealLevel, client) * 100) + " To Hit", Qahr.Mobile.Game.MsgServer.MsgMessage.ChatMode.System, Qahr.Mobile.Game.MsgServer.MsgMessage.MsgColor.red, false, true);

                client.Status.DodgeRate = (uint)(GetEthereal(client.Status.EtherealLevel, client) * 100);
                client.Status.HitRate = (uint)(GetHawkeye(client.Status.HitRate + rate, client) * 100);
                GetHawkeye(client.Status.HawkeyeLevel, client);
                GetEdge(client.Status.EdgeLevel, client);
                if (client.Status.VenomLevel > 0)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)20000))
                            client.MySpells.Add(stream, (ushort)20000, (ushort)client.Status.VenomLevel);
                    }
                }
            }

            client.Status.DodgeRate = (uint)(GetEthereal(client.Status.EtherealLevel, client) * 100);
            client.Status.DashRate += (uint)(GetDiscipline(client.Status.Discipline, client));
            client.Status.Resist += (uint)(GetSafeguard(client.Status.Safeguard, client));
            client.Status.HitRate = (uint)(GetHawkeye(client.Status.HawkeyeLevel, client) * 100);
            GetEdge(client.Status.EdgeLevel, client);
            if (client.Status.VenomLevel > 0)
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Venom))
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Venom, (ushort)client.Status.VenomLevel);
                }
            }
        }

        public static List<MythSoulEXP> Myths = new List<MythSoulEXP>();


        public static Dictionary<uint, MythSoulEXP> RandomMythSoul = new Dictionary<uint, MythSoulEXP>();
        public static Dictionary<uint, MythSoulEXP> MythSoulExpList = new Dictionary<uint, MythSoulEXP>();
        public static MythSoulEXP[] item;

        public class MythSoulEXP
        {

            public uint ItemType;
            public MythAttribute type;
            public uint value;
            public uint Score;
            public uint Progress;
            public uint Exp;
            public uint Unk1;
            public uint Unk2;
        }
        public enum MythAttribute
        {
            None = 0,
            MaxHP = 1,
            PAttack = 2,
            PDefense = 3,
            MAttack = 4,
            MDefense = 5,
            FinalPAttack = 6,
            FinalPDamage = 7,
            FinalMAttack = 8,
            FinalMDamage = 9,
            PStrike = 10,
            MStrike = 11,
            Immunity = 12,
            Break = 13,
            AntiBreak = 14,

        }
        public static void Load()
        {
            if (File.Exists(Program.ServerConfig.DbLocation + "speffect_attribute.txt"))
            {
                string[] Lines = File.ReadAllLines((Program.ServerConfig.DbLocation + "speffect_attribute.txt"));
                foreach (var line in Lines)
                {
                    var spilitline = line.Split(new string[] { "@@", " " }, StringSplitOptions.RemoveEmptyEntries);
                    MythSoulEXP irc = new MythSoulEXP();
                    irc.ItemType = Convert.ToUInt32(spilitline[0]);
                    irc.type = (MythAttribute)Convert.ToByte(spilitline[1]);
                    irc.value = Convert.ToUInt32(spilitline[2]);
                    irc.Score = Convert.ToUInt32(spilitline[3]);
                    irc.Progress = Convert.ToUInt32(spilitline[4]);
                    irc.Exp = Convert.ToUInt32(spilitline[5]);
                    irc.Unk1 = Convert.ToUInt32(spilitline[6]);
                    irc.Unk2 = Convert.ToUInt32(spilitline[7]);
                    if (!MythSoulExpList.ContainsKey(irc.ItemType))
                        MythSoulExpList.Add(irc.ItemType, irc);
                    byte Level = (byte)(irc.ItemType % 10);
                    uint Type = (irc.ItemType % 1000) - Level;
                    if (Type == 10 || Type == 40 || Type == 80 || Type == 170 || Type == 180 || Type == 20 || Type == 60 || Type == 30 || Type == 140 || Type == 360)
                    {
                        if (!RandomMythSoul.ContainsKey(irc.ItemType))
                            RandomMythSoul.Add(irc.ItemType, irc);
                    }

                    Myths.Add(irc);
                }
            }
        }
        public static void LoadMyth()
        {
            Load();
            MythSoulAttributes.Load();
        }

    }
}
