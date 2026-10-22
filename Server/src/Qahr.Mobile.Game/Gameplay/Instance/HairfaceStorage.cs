using Qahr.Mobile.Game.MsgServer;
using System.Collections.Generic;
using System.Linq;

namespace Qahr.Mobile.Role.Instance
{
    public class HairfaceStorage
    {
        private List<Database.HairfaceStorageTable.Hairface> items;
        private Database.HairfaceStorageTable.Hairface[] objects;
        private Client.GameClient Owner;

        public HairfaceStorage(Client.GameClient client)
        {
            Owner = client;
            items = new List<Database.HairfaceStorageTable.Hairface>();
            objects = new Database.HairfaceStorageTable.Hairface[0];
        }

        // =========================================================
        // Add Item
        // =========================================================

        public bool Add(Database.HairfaceStorageTable.Hairface item, bool Send = true)
        {
            if (item == null)
                return false;

            if (!Exists(item.ID, item.Type))
            {
                items.Add(item);
                objects = items.ToArray();

                if (Send)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var streamm = rec.GetStream();

                        Owner.Send(
                            streamm.CreateHairface(
                                new MsgHairfaceStorage.MsgHairfaceStorageProto()
                                {
                                    Type = MsgHairfaceStorage.Actions.Add,

                                    Items = new MsgHairfaceStorage.Item[1]
                                    {
                                        new MsgHairfaceStorage.Item()
                                        {
                                            Type = item.Type,
                                            ID = item.ID,
                                            Equipped = false,
                                            Unk4 = 0,
                                            Colors = new long[2]
                                            {
                                                0,
                                                1
                                            }
                                        }
                                    }
                                }
                            )
                        );
                    }
                }

                return true;
            }

            return false;
        }

        // =========================================================
        // Add Color
        // =========================================================

        public bool AddColor(Database.HairfaceStorageTable.HairColor item)
        {
            if (item == null)
                return false;

            if (Exists(item.ID, MsgHairfaceStorage.Type.Hairstyle))
            {
                var hair = items.FirstOrDefault(i =>
                    i.ID == item.ID &&
                    i.Type == MsgHairfaceStorage.Type.Hairstyle);

                if (hair == null)
                    return false;

                if (hair.Colors == null || item.Color >= hair.Colors.Length)
                    return false;

                hair.Colors[item.Color] = 1;
                objects = items.ToArray();

                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var streamm = rec.GetStream();

                    Owner.Send(
                        streamm.CreateHairface(
                            new MsgHairfaceStorage.MsgHairfaceStorageProto()
                            {
                                Type = MsgHairfaceStorage.Actions.AddColor,

                                HairColor = new MsgHairfaceStorage.Operation()
                                {
                                    ID = item.ID,
                                    dwParam = item.Color
                                }
                            }
                        )
                    );
                }

                return true;
            }

            return false;
        }

        // =========================================================
        // Can Equip
        // =========================================================

        public bool CanEquip(Database.HairfaceStorageTable.Hairface hair)
        {
            if (hair == null)
                return false;

            if (hair.Type >= MsgHairfaceStorage.Type.TexasTable)
                return true;

            return
                (
                    hair.Sex == 0 ||
                    (
                        Role.Core.IsBoy(Owner.Player.Body) &&
                        hair.Sex == 1
                    ) ||
                    (
                        Role.Core.IsGirl(Owner.Player.Body) &&
                        hair.Sex == 2
                    )
                )
                &&
                (
                    Owner.Player.VipLevel >= hair.RequiredVIPLevel
                )
                &&
                (
                    hair.Classes == null ||
                    hair.Classes.Count == 0 ||
                    hair.Classes.Contains(
                        (byte)(Owner.Player.Class / 1000)
                    )
                );
        }

        // =========================================================
        // Equip
        // =========================================================

        public bool Equip(Database.HairfaceStorageTable.Hairface hair)
        {
            if (hair == null)
                return false;

            if (Exists(hair.ID, hair.Type) && CanEquip(hair))
            {
                if (
                    hair.Type >= MsgHairfaceStorage.Type.TexasTable &&
                    hair.Type <= MsgHairfaceStorage.Type.Emoji &&
                    hair.Type != MsgHairfaceStorage.Type.Frame
                )
                {
                    var ItemsEquped = items
                        .Where(i =>
                            i.ID != hair.ID &&
                            i.Type == hair.Type &&
                            i.Equiped)
                        .FirstOrDefault();

                    if (ItemsEquped != null)
                        UnEquip(ItemsEquped);

                    var current = items.FirstOrDefault(i =>
                        i.ID == hair.ID &&
                        i.Type == hair.Type);

                    if (current != null)
                        current.Equiped = true;

                    objects = items.ToArray();
                }

                else if (hair.Type == MsgHairfaceStorage.Type.Frame)
                {
                    var ItemsEquped = items
                        .Where(i =>
                            i.ID != hair.ID &&
                            i.Type == hair.Type &&
                            i.Equiped)
                        .FirstOrDefault();

                    if (ItemsEquped != null)
                        UnEquip(ItemsEquped);

                    var current = items.FirstOrDefault(i =>
                        i.ID == hair.ID &&
                        i.Type == hair.Type);

                    if (current != null)
                        current.Equiped = true;

                    objects = items.ToArray();

                    Owner.Player.FrameID = (ushort)hair.ID;
                }

                else if (hair.Type == MsgHairfaceStorage.Type.Hairstyle)
                {
                    Owner.Player.Hair = (ushort)hair.ID;
                    Owner.Player.HairColor = hair.EquippedColor;
                }

                else if (hair.Type == MsgHairfaceStorage.Type.Avatar)
                {
                    Owner.Player.Face = (ushort)hair.ID;
                }

                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var streamm = rec.GetStream();

                    Owner.Send(
                        streamm.CreateHairface(
                            new MsgHairfaceStorage.MsgHairfaceStorageProto()
                            {
                                Type = MsgHairfaceStorage.Actions.Equip,

                                Hair = new MsgHairfaceStorage.Operation()
                                {
                                    ID = (byte)hair.Type,
                                    dwParam = hair.ID
                                }
                            }
                        )
                    );
                }

                return true;
            }

            return false;
        }

        // =========================================================
        // UnEquip
        // =========================================================

        public bool UnEquip(Database.HairfaceStorageTable.Hairface hair)
        {
            if (hair == null)
                return false;

            if (Exists(hair.ID, hair.Type) && CanEquip(hair))
            {
                var current = items.FirstOrDefault(i =>
                    i.ID == hair.ID &&
                    i.Type == hair.Type);

                if (current == null)
                    return false;

                current.Equiped = false;
                objects = items.ToArray();

                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var streamm = rec.GetStream();

                    Owner.Send(
                        streamm.CreateHairface(
                            new MsgHairfaceStorage.MsgHairfaceStorageProto()
                            {
                                Type = MsgHairfaceStorage.Actions.UnEquip,

                                Hair = new MsgHairfaceStorage.Operation()
                                {
                                    ID = (byte)hair.Type,
                                    dwParam = hair.ID
                                }
                            }
                        )
                    );
                }

                return true;
            }

            return false;
        }

        // =========================================================
        // Equip Color
        // =========================================================

        public bool EquipColor(uint ID, byte Color)
        {
            if (!Exists(ID, MsgHairfaceStorage.Type.Hairstyle))
                return false;

            var hair = items.FirstOrDefault(i =>
                i.ID == ID &&
                i.Type == MsgHairfaceStorage.Type.Hairstyle);

            if (hair == null ||
                hair.Colors == null ||
                Color >= hair.Colors.Length)
                return false;

            if (hair.Colors[Color] != 1)
                return false;

            if (Owner.Player.Hair % 1000 != ID)
                return false;

            hair.EquippedColor = Color;
            objects = items.ToArray();

            Owner.Player.HairColor = Color;

            using (var rec = new ServerSockets.RecycledPacket())
            {
                var streamm = rec.GetStream();

                Owner.Send(
                    streamm.CreateHairface(
                        new MsgHairfaceStorage.MsgHairfaceStorageProto()
                        {
                            Type = MsgHairfaceStorage.Actions.EquipColor,

                            HairColor = new MsgHairfaceStorage.Operation()
                            {
                                ID = ID,
                                dwParam = Color
                            }
                        }
                    )
                );
            }

            return true;
        }

        // =========================================================
        // Exists
        // =========================================================

        /// <summary>[PuppetTheater 2026-10-15] removes a timed avatar frame / emoji when it expires (server side; the client
        /// list refreshes on the next login). False = not owned.</summary>
        public bool Remove(uint ID, MsgHairfaceStorage.Type type)
        {
            int n = items.RemoveAll(i => i != null && i.ID == ID && i.Type == type);
            if (n == 0) return false;
            objects = items.ToArray();
            return true;
        }
        public bool Exists(uint ID, MsgHairfaceStorage.Type type)
        {
            return Objects.Any(i =>
                i.ID == ID &&
                i.Type == type);
        }

        // =========================================================
        // Objects
        // =========================================================

        public Database.HairfaceStorageTable.Hairface[] Objects
        {
            get
            {
                return objects;
            }
        }

        // =========================================================
        // Save
        // =========================================================

        public override string ToString()
        {
            var file =
                new Database.DBActions.WriteLine('/')
                .Add(Objects.Length);

            foreach (var item in Objects)
            {
                file.Add((byte)item.Type);
                file.Add(item.ID);
                file.Add(item.EquippedColor);

                if (item.Colors == null)
                    item.Colors = new byte[0];

                file.Add(item.Colors.Length);

                for (int i = 0; i < item.Colors.Length; i++)
                    file.Add(item.Colors[i]);

                file.Add(item.Equiped ? (byte)1 : (byte)0);
            }

            return file.Close();
        }

        // =========================================================
        // Load
        // =========================================================

        public void Load(string line)
        {
            if (string.IsNullOrEmpty(line))
                return;

            Database.DBActions.ReadLine reader =
                new Database.DBActions.ReadLine(line, '/');

            int count = reader.Read((int)0);

            for (int i = 0; i < count; i++)
            {
                var item =
                    new Database.HairfaceStorageTable.Hairface()
                    {
                        Type =
                            (MsgHairfaceStorage.Type)
                            reader.Read((byte)0),

                        ID =
                            reader.Read((uint)0),

                        EquippedColor =
                            reader.Read((byte)0)
                    };

                int colorCount = reader.Read((int)0);

                if (colorCount < 0)
                    colorCount = 0;

                item.Colors = new byte[colorCount];

                for (int x = 0; x < item.Colors.Length; x++)
                {
                    item.Colors[x] =
                        reader.Read((byte)0);
                }

                item.Equiped =
                    reader.Read((byte)0) == 1;

                var baseItem =
                    Database.HairfaceStorageTable.Hairfaces
                    .FirstOrDefault(z =>
                        z.ID == item.ID &&
                        z.Type == item.Type);

                if (baseItem != null)
                {
                    item.Classes = baseItem.Classes;
                    item.Cost = baseItem.Cost;
                    item.Name = baseItem.Name;
                    item.RareLevel = baseItem.RareLevel;
                    item.RequiredVIPLevel = baseItem.RequiredVIPLevel;
                    item.Sex = baseItem.Sex;
                    item.Type = baseItem.Type;

                    Add(item, false);
                }
            }

            // IMPORTANT:
            // لا يوجد هنا AddFreeProfileCards()
            // الـ Profile Cards يتم فتحها من الـ NPC فقط.
        }
    }
}
