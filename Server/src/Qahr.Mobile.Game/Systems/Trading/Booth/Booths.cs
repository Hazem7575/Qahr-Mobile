using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Qahr.Mobile.Client;
using Qahr.Mobile.Game;
using Qahr.Mobile.Role;
using Qahr.Mobile.Game.MsgServer;
using Qahr.Mobile.Role.Instance;
using Qahr.Mobile.Database;

namespace Qahr.Mobile
{
    public class Booths
    {
        public enum BoothType
        {
            Booth = 0,
            SpawnPlayer = 1
        }
        public class booth
        {
            public uint UID;

            public ushort Mesh = 100;

            public string Name;

            public ushort Map1;

            public ushort X;

            public ushort Y;

            public List<string> Items;

            public BoothType Type;

            public byte Costtype;

            // public string BotMessage = "Selling Items [Boothing AI]";

            public uint Garment = 194300;

            public uint Head = 112259;

            public uint WeaponR = 601439;

            public uint WeaponL = 601439;

            public uint Armor = 135259;

            public uint Body = 1008;

            public uint HairStyle = 89;

            public uint Action = 250;
        }
        public static System.SafeDictionary<uint, booth> Boooths = new System.SafeDictionary<uint, booth>();
        public static void Load()
        {
            // [BoothsOff 2026-10-13] the file may be missing or empty (shop bots disabled): load nothing
            string boothsFile = Program.ServerConfig.DbLocation + "Booths.txt";
            if (!File.Exists(boothsFile))
                return;
            string[] text = File.ReadAllLines(boothsFile);
            booth booth = new booth();
            for (int x = 0; x < text.Length; x++)
            {
                string line = text[x];
                string[] split = line.Split('=');
                if (split[0] == "ID")
                {
                    if (booth.UID == 0)
                        booth.UID = uint.Parse(split[1]);
                    else
                    {
                        if (!Boooths.ContainsKey(booth.UID))
                        {
                            Boooths.Add(booth.UID, booth);
                            booth = new booth();
                            booth.UID = uint.Parse(split[1]);
                        }
                    }
                }
                else if (split[0] == "Type")
                {
                    booth.Type = (BoothType)byte.Parse(split[1]);
                }
                else if (split[0] == "Name")
                {
                    booth.Name = split[1];
                }
                //  else if (split[0] == "BotMessage")
                // {
                //   booth.BotMessage = split[1];
                // }
                else if (split[0] == "Garment")
                {
                    booth.Garment = uint.Parse(split[1]);
                }
                else if (split[0] == "Head")
                {
                    booth.Head = uint.Parse(split[1]);
                }
                else if (split[0] == "WeaponR")
                {
                    booth.WeaponR = uint.Parse(split[1]);
                }
                else if (split[0] == "WeaponL")
                {
                    booth.WeaponL = uint.Parse(split[1]);
                }
                else if (split[0] == "Armor")
                {
                    booth.Armor = uint.Parse(split[1]);
                }
                else if (split[0] == "Mesh")
                {
                    booth.Mesh = ushort.Parse(split[1]);
                }
                else if (split[0] == "Map")
                {
                    booth.Map1 = ushort.Parse(split[1]);
                }
                else if (split[0] == "X")
                {
                    booth.X = ushort.Parse(split[1]);
                }
                else if (split[0] == "Y")
                {
                    booth.Y = ushort.Parse(split[1]);
                }
                else if (split[0] == "Body")
                {
                    booth.Body = ushort.Parse(split[1]);
                }
                else if (split[0] == "ItemAmount")
                {
                    booth.Items = new List<string>(ushort.Parse(split[1]));
                }
                else if (split[0] == "Action")
                {
                    booth.Action = ushort.Parse(split[1]);

                }
                else if (split[0].Contains("Item") && split[0] != "ItemAmount")
                {
                    string name = split[1];
                    booth.Items.Add(name);
                }
            }
            if (booth.UID != 0 && !Boooths.ContainsKey(booth.UID)) // [BoothsOff 2026-10-13] never add an empty entry
                Boooths.Add(booth.UID, booth);
            CreateBooths();
        }
        public Booths(GameClient client)
        {
            client.Player = new Player(client);
            client.Inventory = new Inventory(client);
            client.Equipment = new Role.Instance.Equip(client);

            client.Warehouse = new Warehouse(client);
            client.MyProfs = new Proficiency(client);
            client.MySpells = new Role.Instance.Spell(client);
            client.Achievement = new AchievementCollection();
            client.Status = new MsgStatus();
            client.HundredWeapons = new Role.Instance.HundredWeapons(client);
            client.Player.SubClass = new Role.Instance.SubClass();
            client.Player.MyUnion = new Role.Instance.Union();
            client.Player.MyChi = new Role.Instance.Chi(client.Player.UID);
            client.Player.InnerPower = new Role.Instance.InnerPower(client.Player.Name, client.Player.UID);
            client.Player.Associate = new Role.Instance.Associate.MyAsociats(client.Player.UID);
            client.Rune = new Role.Instance.Rune(client);
            client.Beasts = new Role.Instance.Beasts(client);
            client.Player.MyClan = new Role.Instance.Clan();
            DataCore.LoadClient(client.Player);
        }

        public static void CreateBooths()
        {
            foreach (var bo in Boooths.Values)
            {
                Game.Booth booth = new Game.Booth();
                SobNpc Base = new SobNpc();
                Base.UID = bo.UID;
                if (Booth.Booths2.ContainsKey(Base.UID))
                    Booth.Booths2.Remove(Base.UID);
                Booth.Booths2.Add(Base.UID, booth);
                Base.ObjType = MapObjectType.SobNpc;
                Base.Mesh = (Role.SobNpc.StaticMesh)bo.Mesh;
                Base.Type = Role.Flags.NpcType.Booth;
                Base.Name = bo.Name;
                Base.Map = bo.Map1;
                Base.Booth = booth;
                Base.X = bo.X;
                Base.Y = bo.Y;

                booth.Base = Base;
                if (bo.Type == BoothType.SpawnPlayer)
                {
                    #region SpawnPlayer
                    using (ServerSockets.RecycledPacket packet = new ServerSockets.RecycledPacket())
                    {
                        ServerSockets.Packet stream = packet.GetStream();
                        uint key = bo.UID + 1;
                        if (!Pool.GamePoll.ContainsKey(key))
                        {
                            GameClient client = new GameClient(null, false);

                            client.MyBooths = new Booths(client);

                            client.Fake = true;

                            client.Player.Name = Base.Name;

                            client.Player.Body = (ushort)bo.Body;

                            client.Player.Hair = (ushort)bo.HairStyle;

                            client.Player.HairColor = 3;

                            client.Player.Angle = (Flags.ConquerAngle)((byte)(bo.Mesh % 10));

                            client.Player.UID = key;

                            client.Status.MaxHitpoints = 65000;

                            client.Player.HitPoints = 65000;

                            client.Player.X = bo.X;

                            client.Player.Y = bo.Y;

                            client.Player.Map = bo.Map1;

                            client.Player.Level = 140;

                            client.Player.ExtraBattlePower = 522;

                            client.Player.Action = (Flags.ConquerAction)bo.Action;

                            client.Map = Pool.ServerMaps[Base.Map];

                            client.Map.Enquer(client);
                            EditBotBoothsByMT(client, bo.Name);
                            client.Player.GarmentId = bo.Garment;

                            client.Player.RightWeaponId = bo.WeaponR;

                            client.Player.LeftWeaponId = bo.WeaponL;

                            Base.UID = bo.UID;

                            Base.Mesh = (SobNpc.StaticMesh)(((int)(SobNpc.StaticMesh.Vendor)) - 6 + ((int)(client.Player.Angle)));

                            if (client.Player.Angle == (Flags.ConquerAngle)2)
                                Base.X = (ushort)(bo.X - 1);
                            else if (client.Player.Angle == (Flags.ConquerAngle)4)
                                Base.Y = (ushort)(bo.Y - 1);
                            else
                                Base.X = (ushort)(bo.X + 1);
                            // MsgMessage message = new MsgMessage(bo.BotMessage, "All", client.Player.Name, MsgMessage.MsgColor.black, MsgMessage.ChatMode.HawkMessage);

                            //  booth.HawkMessage = message;

                            client.MyBooth = booth;

                        }
                    }
                    #endregion

                }
                if (Pool.ServerMaps.ContainsKey(Base.Map))
                {
                    Pool.ServerMaps[Base.Map].View.EnterMap<Role.IMapObj>(Base);
                }
                for (int i = 0; i < bo.Items.Count; i++)
                {
                    var line = bo.Items[i].Split(new string[] { "@@", "@" }, StringSplitOptions.RemoveEmptyEntries);
                    #region booth
                    Game.BoothItem item = new Game.BoothItem();

                    item.Item = new MsgGameItem();
                    item.Item.UID = MsgGameItem.ItemUID.Next;
                    item.Item.ITEM_ID = uint.Parse(line[0]);
                    if (line.Length >= 2)
                        item.Cost = uint.Parse(line[1]);
                    if (line.Length >= 3)
                        item.Item.Plus = byte.Parse(line[2]);
                    if (line.Length >= 4)
                        item.Item.Enchant = byte.Parse(line[3]);
                    if (line.Length >= 5)
                        item.Item.Bless = byte.Parse(line[4]);
                    if (line.Length >= 6)
                        item.Item.SocketOne = (Role.Flags.Gem)byte.Parse(line[5]);
                    if (line.Length >= 7)
                        item.Item.SocketTwo = (Role.Flags.Gem)byte.Parse(line[6]);
                    if (line.Length >= 8)
                        item.Item.StackSize = ushort.Parse(line[7]);

                    Database.ItemType.DBItem CIBI = new Database.ItemType.DBItem();
                    if (Pool.ItemsBase.TryGetValue(item.Item.ITEM_ID, out CIBI))
                    {
                        if (CIBI == null)
                            break;
                        item.Item.Durability = CIBI.Durability;
                        item.Item.MaximDurability = CIBI.Durability;

                        if (bo.Costtype == 2)
                        {
                            item.Cost_Type = Game.BoothItem.CostType.Silvers;

                        }
                        else
                        {
                            item.Cost_Type = Game.BoothItem.CostType.ConquerPoints;
                        }
                        booth.ItemList.Add(item.Item.UID, item);

                    }
                    #endregion
                }
            }
        }

        public static void EditBotBoothsByMT(Client.GameClient client, string Name)
        {
            #region Edit
            using (ServerSockets.RecycledPacket packet = new ServerSockets.RecycledPacket())
            {
                ServerSockets.Packet stream = packet.GetStream();
                if (Name == "MarineTreasure")
                {
                    client.Player.Class = 7057;
                    if (!client.MyArchives.Items.ContainsKey(Archives.TypeID.LavaNut))
                    {
                        client.MyArchives.AddItem(Archives.TypeID.LavaNut, 1, 0, 1, 0);

                    }
                    client.Player.AddSpellFlag(MsgUpdate.Flags.Sense, (int)Role.StatusFlagsBigVector32.PermanentFlag, false);
                }
                if (Name == "TaoistItems")
                {
                    client.Player.Class = 14057;
                    if (!client.MyArchives.Items.ContainsKey(Archives.TypeID.BirthDeath))
                    {
                        client.MyArchives.AddItem(Archives.TypeID.BirthDeath, 1, 0, 1, 0);

                    }
                    client.Player.AddSpellFlag(MsgUpdate.Flags.FlameShield, (int)Role.StatusFlagsBigVector32.PermanentFlag, false);
                }
                if (Name == "TwistedNote")
                {
                    client.Player.Class = 4057;
                    if (!client.MyArchives.Items.ContainsKey(Archives.TypeID.ThornCutter))
                    {
                        client.MyArchives.AddItem(Archives.TypeID.ThornCutter, 1, 0, 1, 0);

                    }
                    client.Player.AddSpellFlag(MsgUpdate.Flags.WindArrow, (int)Role.StatusFlagsBigVector32.PermanentFlag, false);
                    client.Player.AddSpellFlag(MsgUpdate.Flags.PoisonArrow, (int)Role.StatusFlagsBigVector32.PermanentFlag, false);
                    client.Player.AddSpellFlag(MsgUpdate.Flags.IceArrow, (int)Role.StatusFlagsBigVector32.PermanentFlag, false);
                    client.Player.AddSpellFlag(MsgUpdate.Flags.ThunderArrow, (int)Role.StatusFlagsBigVector32.PermanentFlag, false);
                    client.Player.AddSpellFlag(MsgUpdate.Flags.FireArrow, (int)Role.StatusFlagsBigVector32.PermanentFlag, false);
                }
                if (Name == "WarMyths")
                {
                    client.Player.Class = 2057;
                    if (!client.MyArchives.Items.ContainsKey(Archives.TypeID.Redcurse))
                    {
                        client.MyArchives.AddItem(Archives.TypeID.Redcurse, 1, 0, 1, 0);

                    }
                    client.Player.AddSpellFlag(MsgUpdate.Flags.Immersion, (int)Role.StatusFlagsBigVector32.PermanentFlag, false);
                    client.Player.AddSpellFlag(MsgUpdate.Flags.Insouciance, (int)Role.StatusFlagsBigVector32.PermanentFlag, false);
                    client.Player.AddSpellFlag(MsgUpdate.Flags.WildDash, (int)Role.StatusFlagsBigVector32.PermanentFlag, false);
                }
                if (Name == "NinpoScroll")
                {
                    client.Player.NiniaP0 = 9;
                    client.Player.AddSpellFlag(MsgUpdate.Flags.SageMode, (int)Role.StatusFlagsBigVector32.PermanentFlag, false);
                    client.Player.AddSpellFlag(MsgUpdate.Flags.HellVortex, (int)Role.StatusFlagsBigVector32.PermanentFlag, false);
                }
                if (Name == "TrojanArchive")
                {
                    client.Player.AddSpellFlag(MsgUpdate.Flags.CelestialDance, (int)Role.StatusFlagsBigVector32.PermanentFlag, false);
                }
                if (Name == "Runes's")
                {
                    client.Player.AddSpellFlag(MsgUpdate.Flags.BloodTide, (int)Role.StatusFlagsBigVector32.PermanentFlag, false);
                }
                if (Name == "GarmentS")
                {
                    client.Player.AddSpellFlag(MsgUpdate.Flags.BloodTide, (int)Role.StatusFlagsBigVector32.PermanentFlag, false);
                }
                if (Name == "OtherItems")
                {
                    client.Player.AddSpellFlag(MsgUpdate.Flags.BloodTide, (int)Role.StatusFlagsBigVector32.PermanentFlag, false);
                    client.Player.AddFlag(MsgUpdate.Flags.Ride, Role.StatusFlagsBigVector32.PermanentFlag, false, 1);

                    client.Vigor = client.Status.MaxVigor;

                    client.Send(stream.ServerInfoCreate(client.Vigor));
                    client.Player.MountArmorId = 200777;
                }

            }
            #endregion
        }
    }
}