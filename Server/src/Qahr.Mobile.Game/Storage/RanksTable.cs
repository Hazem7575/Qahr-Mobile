using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Database
{
    public static class RanksTable
    {
        public const byte MaxRegisters = 30;

        public enum TopType : uint
        {

            Trojan = 1,
            Warrior = 2,
            Archer = 3,
            Ninja = 4,
            Monk = 5,
            Pirate = 6,
            Longlee = 7,
            Taoist = 8,
            Water = 9,
            Fire = 10,
            Nobility = 11,
            Arena = 12,
            ConquerPoints = 13,
            Count = 14,
            DuneWanderer = 15
        }

        public unsafe struct Item
        {
            public uint Class;
            public uint Level;
            public ulong NobilityDonation;
            public unsafe fixed sbyte szName[16];
            public uint ConquerPoints;
            public string Name
            {
                get
                {
                    fixed (sbyte* ptr = szName) { return new string(ptr); }
                }
                set
                {
                    fixed (sbyte* ptr = szName)
                    {
                        for (int x = 0; x < value.Length; x++)
                        {
                            *(sbyte*)(ptr + x) = (sbyte)value[x];
                        }
                    }
                }
            }
        }



        public static Dictionary<TopType, List<Item>> ArrayTops = new Dictionary<TopType, List<Item>>();

        public static void Initialize()
        {
            List<Item> ArrayPlayers = new List<Item>();

            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");

            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
            {
                ini.FileName = fname;
                Item msg = new Item();
                msg.NobilityDonation = ini.ReadUInt64("Character", "DonationNobility", 0);
                msg.Class = ini.ReadByte("Character", "Class", 0);
                msg.Level = ini.ReadByte("Character", "Level", 0);
                msg.Name = ini.ReadString("Character", "Name", "None");
                msg.ConquerPoints = ini.ReadUInt32("Character", "ConquerPoints", 0);
                ArrayPlayers.Add(msg);
            }

            ArrayTops.Add(TopType.Trojan, CreateRanks(p => Database.AtributesStatus.IsTrojan((byte)p.Class), ArrayPlayers));
            ArrayTops.Add(TopType.Warrior, CreateRanks(p => Database.AtributesStatus.IsWarrior((byte)p.Class), ArrayPlayers));
            ArrayTops.Add(TopType.Archer, CreateRanks(p => Database.AtributesStatus.IsArcher((byte)p.Class), ArrayPlayers));
            ArrayTops.Add(TopType.Ninja, CreateRanks(p => Database.AtributesStatus.IsNinja((byte)p.Class), ArrayPlayers));
            ArrayTops.Add(TopType.Monk, CreateRanks(p => Database.AtributesStatus.IsMonk((byte)p.Class), ArrayPlayers));
            ArrayTops.Add(TopType.Pirate, CreateRanks(p => Database.AtributesStatus.IsPirate((byte)p.Class), ArrayPlayers));
            ArrayTops.Add(TopType.Longlee, CreateRanks(p => Database.AtributesStatus.IsLee((byte)p.Class), ArrayPlayers));
            ArrayTops.Add(TopType.Taoist, CreateRanks(p => Database.AtributesStatus.IsTaoist((byte)p.Class), ArrayPlayers));
            ArrayTops.Add(TopType.Water, CreateRanks(p => Database.AtributesStatus.IsWater((byte)p.Class), ArrayPlayers));
            ArrayTops.Add(TopType.Fire, CreateRanks(p => Database.AtributesStatus.IsFire((byte)p.Class), ArrayPlayers));
            ArrayTops.Add(TopType.DuneWanderer, CreateRanks(p => Database.AtributesStatus.IsDune((byte)p.Class), ArrayPlayers));
            ArrayTops.Add(TopType.ConquerPoints, CreateRanks(null, ArrayPlayers));
            ArrayTops.Add(TopType.Nobility, new List<Item>());

            var arraynobility = ArrayPlayers.OrderByDescending(p => p.NobilityDonation).ToArray();
            int count = 0;
            foreach (var item in arraynobility)
            {
                ArrayTops[TopType.Nobility].Add(item);
                count++;
                if (count == MaxRegisters)
                    break;
            }

            //create arena ranks.
            ArrayTops.Add(TopType.Arena, new List<Item>());
            var arrayArenaRanks = Game.MsgTournaments.MsgArena.ArenaPoll.Values.OrderByDescending(p => p.Info.CurrentHonor).ToArray();
            count = 0;
            foreach (var item in arrayArenaRanks)
            {
                ArrayTops[TopType.Arena].Add(new Item() { Class = item.Class, Level = item.Level, Name = item.Name, NobilityDonation = item.Info.CurrentHonor });
                count++;
                if (count == MaxRegisters)
                    break;
            }
        }


        public static List<Item> CreateRanks(Func<Item, bool> pre, List<Item> arrayitems)
        {
            List<Item> Items = new List<Item>();

            var items = new Item[0];
            if (pre != null)
                items = arrayitems.Where(p => pre(p)).ToArray();
            else
                items = arrayitems.ToArray();

            var ranks = items.OrderByDescending(p => p.Level).ToArray();

            int count = 0;
            foreach (var item in ranks)
            {
                Items.Add(item);
                count++;
                if (count == MaxRegisters)
                    break;
            }
            return Items;
        }

    }
}
