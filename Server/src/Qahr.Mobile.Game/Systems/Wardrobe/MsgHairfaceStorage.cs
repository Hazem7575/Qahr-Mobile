using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class MsgHairfaceStorage
    {
        public enum Type : byte
        {
            Hairstyle = 0,
            Avatar = 1,
            TexasTable = 2,
            CardBack = 3,
            TexasBet = 4,
            Level = 5,
            Map = 6,
            Dealer = 7,
            Carpet = 8,
            Frame = 9,
            Emoji = 10,
            Head = 11,
            ProfileCard = 12,
        }

        public enum Actions : byte
        {
            Login = 0,
            Add = 1,
            Equip = 3,
            Buy = 4,
            AddColor = 5,
            EquipColor = 6,
            BuyColor = 7,
            Avatars = 8,
            HairStyles = 9,
            UnEquip = 10,
            View = 11,
        }

        [ProtoContract]
        public class MsgHairfaceStorageProto
        {
            [ProtoMember(1, IsRequired = true)]
            public Actions Type;

            [ProtoMember(2)]
            public Operation Hair;

            [ProtoMember(3)]
            public Operation HairColor;

            // [ProtoFix 2026-10-14] client CMsgHairFaceStoragePB #4 is a SINGLE CMsgHairFaceRuleParamPB (5 x uint32), was Item[]
            [ProtoMember(4)]
            public RuleParam AvatarsOrHairTap;

            [ProtoMember(5)]
            public Item[] Items;

            [ProtoMember(6, IsRequired = true)]
            public uint UID;
        }

        [ProtoContract]
        public class Item
        {
            [ProtoMember(1, IsRequired = true)]
            public Type Type;

            [ProtoMember(2, IsRequired = true)]
            public uint ID;

            [ProtoMember(3, IsRequired = true)]
            public bool Equipped;

            [ProtoMember(4, IsRequired = true)]
            public uint Unk4;

            [ProtoMember(5, IsRequired = true)]
            public long[] Colors;
        }

        // [ProtoFix 2026-10-14] matches client CMsgHairFaceRuleParamPB (f1..f5 required uint32).
        // Field order mirrors the old Item layout (Type, ID, Equipped, Unk4, Colors[0]) so the bytes stay the same.
        [ProtoContract]
        public class RuleParam
        {
            [ProtoMember(1, IsRequired = true)]
            public uint f1;

            [ProtoMember(2, IsRequired = true)]
            public uint f2;

            [ProtoMember(3, IsRequired = true)]
            public uint f3;

            [ProtoMember(4, IsRequired = true)]
            public uint f4;

            [ProtoMember(5, IsRequired = true)]
            public uint f5;
        }

        [ProtoContract]
        public class Operation
        {
            [ProtoMember(1, IsRequired = true)]
            public uint ID;

            [ProtoMember(2, IsRequired = true)]
            public uint dwParam;
        }

        // =========================================================
        // Create Packet
        // =========================================================

        public static unsafe ServerSockets.Packet CreateHairface(
            this ServerSockets.Packet stream,
            MsgHairfaceStorageProto obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgHairFaceStorage);

            return stream;
        }

        // =========================================================
        // Read Packet
        // =========================================================

        public static unsafe void GetHairface(
            this ServerSockets.Packet stream,
            out MsgHairfaceStorageProto pQuery)
        {
            pQuery = new MsgHairfaceStorageProto();
            pQuery = stream.ProtoBufferDeserialize<MsgHairfaceStorageProto>(pQuery);
        }

        // =========================================================
        // Process
        // =========================================================

        [PacketAttribute(GamePackets.MsgHairFaceStorage)]
        private unsafe static void Process(
            Client.GameClient client,
            ServerSockets.Packet stream)
        {
            MsgHairfaceStorageProto Info;

            stream.GetHairface(out Info);

            switch (Info.Type)
            {
                // =================================================
                // View
                // =================================================

                case Actions.View:
                    {
                        break;
                    }

                // =================================================
                // Login
                // =================================================

                case Actions.Login:
                    {
                        Info.UID = client.Player.UID;

                        var HairfaceItems =
                            client.HairfaceStorage.Objects
                            .Where(i =>
                                i.Type >= Type.Hairstyle &&
                                i.Type <= Type.ProfileCard)
                            .ToArray();

                        var lists =
                            new GenericSplit()
                            .Lists<Database.HairfaceStorageTable.Hairface>(
                                40,
                                HairfaceItems
                            );

                        foreach (var list in lists.Values)
                        {
                            Info.Items = new Item[list.Count];

                            for (int i = 0; i < Info.Items.Length; i++)
                            {
                                var storageItem = list[i];

                                Info.Items[i] = new Item();

                                Info.Items[i].ID =
                                    storageItem.ID;

                                Info.Items[i].Type =
                                    storageItem.Type;

                                // =====================================
                                // Equipped
                                // =====================================

                                Info.Items[i].Equipped =
                                    (
                                        storageItem.Type == Type.Hairstyle &&
                                        storageItem.ID ==
                                        client.Player.Hair % 1000
                                    )
                                    ||
                                    (
                                        storageItem.Type == Type.Avatar &&
                                        storageItem.ID ==
                                        client.Player.Face
                                    )
                                    ||
                                    (
                                        storageItem.Equiped
                                    );

                                // =====================================
                                // Unk4
                                //
                                // ProfileCard:
                                // 1 = Owned / Unlocked
                                // 0 = Locked
                                //
                                // باقي الأنواع = 0
                                // =====================================

                                if (storageItem.Type == Type.ProfileCard)
                                {
                                    Info.Items[i].Unk4 = 1;
                                }
                                else
                                {
                                    Info.Items[i].Unk4 = 0;
                                }

                                // =====================================
                                // Hair Colors
                                // =====================================

                                if (storageItem.Type == Type.Hairstyle)
                                {
                                    Info.Items[i].Colors =
                                        new long[2];

                                    Info.Items[i].Colors[0] =
                                        storageItem.EquippedColor;

                                    string bin = "";

                                    if (storageItem.Colors != null)
                                    {
                                        for (
                                            byte z = 0;
                                            z < storageItem.Colors.Length;
                                            z++)
                                        {
                                            bin +=
                                                Convert.ToString(
                                                    storageItem.Colors[z]
                                                );
                                        }
                                    }

                                    try
                                    {
                                        if (!string.IsNullOrEmpty(bin))
                                        {
                                            Info.Items[i].Colors[1] =
                                                Convert.ToInt64(
                                                    BaseFunc.ReverseString(bin),
                                                    2
                                                );
                                        }
                                    }
                                    catch
                                    {
                                        Info.Items[i].Colors[1] = 0;
                                    }
                                }
                            }

                            client.Send(
                                stream.CreateHairface(Info)
                            );
                        }

                        break;
                    }

                // =================================================
                // HairStyles / Avatars
                // =================================================

                case Actions.HairStyles:
                case Actions.Avatars:
                    {
                        // [ProtoFix 2026-10-14] single rule-param (all zero), same bytes as the old one-element Item list
                        Info.AvatarsOrHairTap =
                            new RuleParam();

                        client.Send(
                            stream.CreateHairface(Info)
                        );

                        break;
                    }

                // =================================================
                // Buy
                // =================================================

                case Actions.Buy:
                    {
                        if (client.IsVendor)
                        {
                            client.CreateBoxDialog(
                                "Sorry, You Must Cancel Your Shop First."
                            );

                            return;
                        }

                        if (client.InTrade)
                        {
                            client.CreateBoxDialog(
                                "Sorry, You Must Close The Trade First."
                            );

                            return;
                        }

                        if (Info.Hair == null)
                            return;

                        var item =
                            Database.HairfaceStorageTable.Hairfaces
                            .FirstOrDefault(i =>
                                i.ID == Info.Hair.dwParam &&
                                i.Type ==
                                (Type)Info.Hair.ID
                            );

                        if (
                            item != null &&
                            client.Player.Money >= item.Cost
                        )
                        {
                            client.Player.Money -=
                                item.Cost;

                            client.HairfaceStorage.Add(item);

                            client.CreateBoxDialog(
                                "You successfully bought : " +
                                item.Name +
                                " for " +
                                item.Cost +
                                " Money!"
                            );
                        }

                        break;
                    }

                // =================================================
                // Equip
                // =================================================

                case Actions.Equip:
                    {
                        if (Info.Hair == null)
                            return;

                        var item =
                            client.HairfaceStorage.Objects
                            .FirstOrDefault(i =>
                                i.ID == Info.Hair.dwParam &&
                                i.Type ==
                                (Type)Info.Hair.ID
                            );

                        if (item != null)
                        {
                            client.HairfaceStorage.Equip(item);

                            if (item.Type == Type.Emoji)
                            {
                                client.Player.Emoji =
                                    item.ID;
                            }
                        }

                        break;
                    }

                // =================================================
                // UnEquip
                // =================================================

                case Actions.UnEquip:
                    {
                        if (Info.Hair == null)
                            return;

                        var item =
                            client.HairfaceStorage.Objects
                            .FirstOrDefault(i =>
                                i.ID == Info.Hair.dwParam &&
                                i.Type ==
                                (Type)Info.Hair.ID
                            );

                        if (item != null)
                        {
                            client.HairfaceStorage.UnEquip(item);
                        }

                        break;
                    }

                // =================================================
                // Equip Color
                // =================================================

                case Actions.EquipColor:
                    {
                        if (Info.HairColor == null)
                            return;

                        client.HairfaceStorage.EquipColor(
                            Info.HairColor.ID,
                            (byte)Info.HairColor.dwParam
                        );

                        break;
                    }

                // =================================================
                // Buy Color
                // =================================================

                case Actions.BuyColor:
                    {
                        if (Info.HairColor == null)
                            return;

                        var item =
                            Database.HairfaceStorageTable.HairColors
                            .FirstOrDefault(i =>
                                i.ID == Info.HairColor.ID &&
                                i.Color ==
                                Info.HairColor.dwParam
                            );

                        if (item != null)
                        {
                            if (
                                client.Player.Money >=
                                item.Cost
                            )
                            {
                                client.Player.Money -=
                                    item.Cost;

                                client.HairfaceStorage.AddColor(item);
                            }
                        }

                        break;
                    }
            }
        }
    }
}
