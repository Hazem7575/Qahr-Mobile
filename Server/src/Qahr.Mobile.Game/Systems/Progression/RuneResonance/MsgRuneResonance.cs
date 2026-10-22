using ProtoBuf;
using System;
using System.Collections.Generic;

namespace Qahr.Mobile
{
    public class MsgResonanceRune
    {
        [ProtoContract]
        public class MsgResonanceProto
        {
            [ProtoMember(1)]
            public ActionID Type;

            [ProtoMember(2)]
            public ulong UID;

            [ProtoMember(3)]
            public uint Unk3; // [ProtoFix 2026-10-14] client CMsgRunePB #3 is uint32 (was int)

            [ProtoMember(4)]
            public int Unk4;

            [ProtoMember(5)]
            public List<RuneItem> Items;

            [ProtoMember(6)]
            public List<RuneValue> Values;
        }

        [ProtoContract]
        public class RuneValue
        {
            [ProtoMember(1)]
            public uint Key;

            [ProtoMember(2)]
            public uint Value;
        }

        [ProtoContract]
        public class RuneItem
        {
            [ProtoMember(1)]
            public byte Slot;

            [ProtoMember(2)]
            public uint ItemID;

            [ProtoMember(3)]
            public byte Unknown;
        }
        public enum ActionID : int
        {
            Create = 0,
            Resonate = 1,
            Login = 2,
            Unseal = 3,
            unk1 = 4,
            unk2 = 5
        }
        public enum UpdataID : int
        {
            Create = 0,
            Resonate = 1,
            Login = 2,
            Unseal = 3,
            unk1 = 4,
            unk2 = 5
        }
        [PacketAttribute(2604)]
        private unsafe static void Process(
            Client.GameClient client,
            ServerSockets.Packet stream)
        {
            MsgResonanceProto info;

            stream.GetRuneResonance(out info);

            if (info == null)
                return;

            switch (info.Type)
            {
                case ActionID.Login:
                    {
                        HandleLogin(
                            client,
                            stream);

                        break;
                    }

                case ActionID.Resonate:
                    {
                        HandleResonate(
                            client,
                            stream);

                        break;
                    }

                case ActionID.Create:
                    {
                        HandleCreate(
                            client,
                            stream,
                            info);

                        break;
                    }

                case ActionID.Unseal:
                    {
                        Role.Instance.RuneUnseal.HandleRequest(
                            client,
                            stream,
                            info);

                        break;
                    }

                default:
                    {
                        break;
                    }
            }
        }
        private static void HandleLogin(
            Client.GameClient client,
            ServerSockets.Packet stream)
        {
            bool slot1 =
                client.Player.ResinanceRuneOne > 0;

            bool slot2 =
                client.Player.ResinanceRuneTwo > 0;

            bool slot3 =
                client.Player.ResinanceRunethree > 0;

            bool slot4 =
                client.Player.ResinanceRunefour > 0;

            bool slot5 =
                client.Player.ResinanceRunefive > 0;

            MsgResonanceProto packet =
                new MsgResonanceProto();

            packet.Type = ActionID.Login;
            packet.UID = client.Player.UID;
            packet.Unk3 = 3;
            packet.Unk4 = 0;
            packet.Items = LoadCurrentRuneSlots(client);

            client.Send(
                stream.CreateRuneResonance(packet));
        }
        private static void HandleResonate(
            Client.GameClient client,
            ServerSockets.Packet stream)
        {
            MsgResonanceProto packet =
                new MsgResonanceProto();

            packet.Type = ActionID.Resonate;
            packet.UID = client.Player.UID;
            packet.Unk3 = 3;
            packet.Unk4 = 2;
            packet.Items = LoadCurrentRuneSlots(client);

            client.Send(
                stream.CreateRuneResonance(packet));
        }
        private static void HandleCreate(
            Client.GameClient client,
            ServerSockets.Packet stream,
            MsgResonanceProto info)
        {
            if (info.Unk4 != 1 &&
                info.Unk4 != 2)
                return;

            uint mainMaterialID;
            ushort mainMaterialAmount;

            uint stoneID;
            ushort stoneAmount;

            if (info.Unk4 == 1)
            {
                mainMaterialID = 3314254;
                mainMaterialAmount = 6;

                stoneID = 4060001;
                stoneAmount = 900;
            }
            else
            {
                mainMaterialID = 3314256;
                mainMaterialAmount = 3;

                stoneID = 4040001;
                stoneAmount = 900;
            }

            bool hasMainMaterial =
                client.Inventory.Contain(
                    mainMaterialID,
                    mainMaterialAmount);

            bool hasStone =
                client.Inventory.Contain(
                    stoneID,
                    stoneAmount);

            bool hasHammer =
                client.Inventory.Contain(
                    3324226,
                    3);

            if (!hasMainMaterial)
            {

                SendCreateFailed(
                    client,
                    stream);

                return;
            }

            if (!hasStone &&
                !hasHammer)
            {
                SendCreateFailed(
                    client,
                    stream);

                return;
            }
            client.Inventory.RemoveStackItem(
                mainMaterialID,
                mainMaterialAmount,
                stream);

            if (hasStone)
            {
                client.Inventory.RemoveStackItem(
                    stoneID,
                    stoneAmount,
                    stream);
            }
            else
            {
                client.Inventory.RemoveStackItem(
                    3324226,
                    (ushort)3,
                    stream);
            }
            if (!Role.Core.Rate(3))
            {
                SendCreateFailed(
                    client,
                    stream);
                return;
            }
            ProcessCreateRunes(
                client,
                stream,
                info);
        }
        private static void ProcessCreateRunes(
            Client.GameClient client,
            ServerSockets.Packet stream,
            MsgResonanceProto info)
        {
            if (info.Items == null)
            {
                SendCreateFailed(
                    client,
                    stream);

                return;
            }

            bool created = false;

            foreach (RuneItem rune in info.Items)
            {
                if (rune == null)
                    continue;

                if (rune.Slot < 1 ||
                    rune.Slot > 5)
                    continue;

                if (IsRuneSlotUnlocked(
                    client,
                    rune.Slot))
                {
                    continue;
                }

                switch (rune.Slot)
                {
                    case 1:
                        {
                            client.Player.ResinanceRuneOne = 1;

                            SendCreateResponse(
                                client,
                                stream,
                                1,
                                4039609);

                            created = true;

                            break;
                        }

                    case 2:
                        {
                            client.Player.ResinanceRuneTwo = 1;

                            SendCreateResponse(
                                client,
                                stream,
                                2,
                                4070709);

                            created = true;

                            break;
                        }

                    case 3:
                        {
                            client.Player.ResinanceRunethree = 1;

                            SendCreateResponse(
                                client,
                                stream,
                                3,
                                4071409);

                            created = true;

                            break;
                        }

                    case 4:
                        {
                            client.Player.ResinanceRunefour = 1;

                            SendCreateResponse(
                                client,
                                stream,
                                4,
                                4035209);

                            created = true;

                            break;
                        }

                    case 5:
                        {
                            client.Player.ResinanceRunefive = 1;

                            SendCreateResponse(
                                client,
                                stream,
                                5,
                                4030309);

                            created = true;

                            break;
                        }
                }

                if (created)
                    break;
            }

            if (!created)
            {
                SendCreateFailed(
                    client,
                    stream);
            }
        }

        private static void SendCreateResponse(
            Client.GameClient client,
            ServerSockets.Packet stream,
            byte slot,
            uint itemID)
        {
            MsgResonanceProto packet =
                new MsgResonanceProto();

            packet.Type = ActionID.Create;
            packet.UID = client.Player.UID;
            packet.Unk3 = 3;
            packet.Unk4 = 2;

            packet.Items =
                new List<RuneItem>();

            packet.Items.Add(
                new RuneItem
                {
                    Slot = slot,
                    ItemID = itemID,
                    Unknown = 0
                });

            client.Send(
                stream.CreateRuneResonance(packet));
        }

        private static void SendCreateFailed(
            Client.GameClient client,
            ServerSockets.Packet stream)
        {
            MsgResonanceProto packet =
                new MsgResonanceProto();

            packet.Type = ActionID.Create;
            packet.UID = client.Player.UID;
            packet.Unk3 = 3;
            packet.Unk4 = 0;

            packet.Items =
                LoadRuneItemsFaild(
                    client.Player.UID);

            client.Send(
                stream.CreateRuneResonance(packet));
        }
        private static bool IsRuneSlotUnlocked(
            Client.GameClient client,
            byte slot)
        {
            switch (slot)
            {
                case 1:
                    return client.Player.ResinanceRuneOne > 0;

                case 2:
                    return client.Player.ResinanceRuneTwo > 0;

                case 3:
                    return client.Player.ResinanceRunethree > 0;

                case 4:
                    return client.Player.ResinanceRunefour > 0;

                case 5:
                    return client.Player.ResinanceRunefive > 0;
            }

            return false;
        }
        private static List<RuneItem> LoadCurrentRuneSlots(
            Client.GameClient client)
        {
            List<RuneItem> result =
                new List<RuneItem>();

            RuneItem slot1 =
                new RuneItem();

            slot1.Slot = 1;

            if (client.Player.ResinanceRuneOne > 0)
            {
                slot1.ItemID = 4039609;
                slot1.Unknown = 1;
            }
            else
            {
                slot1.ItemID = 0;
                slot1.Unknown = 0;
            }

            result.Add(slot1);

            RuneItem slot2 =
                new RuneItem();

            slot2.Slot = 2;

            if (client.Player.ResinanceRuneTwo > 0)
            {
                slot2.ItemID = 4070709;
                slot2.Unknown = 1;
            }
            else
            {
                slot2.ItemID = 0;
                slot2.Unknown = 0;
            }

            result.Add(slot2);

            RuneItem slot3 =
                new RuneItem();

            slot3.Slot = 3;

            if (client.Player.ResinanceRunethree > 0)
            {
                slot3.ItemID = 4071409;
                slot3.Unknown = 1;
            }
            else
            {
                slot3.ItemID = 0;
                slot3.Unknown = 0;
            }

            result.Add(slot3);

            RuneItem slot4 =
                new RuneItem();

            slot4.Slot = 4;

            if (client.Player.ResinanceRunefour > 0)
            {
                slot4.ItemID = 4035209;
                slot4.Unknown = 1;
            }
            else
            {
                slot4.ItemID = 0;
                slot4.Unknown = 0;
            }

            result.Add(slot4);

            RuneItem slot5 =
                new RuneItem();

            slot5.Slot = 5;

            if (client.Player.ResinanceRunefive > 0)
            {
                slot5.ItemID = 4030309;
                slot5.Unknown = 1;
            }
            else
            {
                slot5.ItemID = 0;
                slot5.Unknown = 0;
            }

            result.Add(slot5);

            RuneItem slot6 =
                new RuneItem();

            slot6.Slot = 6;
            slot6.ItemID = 0;
            slot6.Unknown = 0;

            result.Add(slot6);

            return result;
        }
        public static List<RuneItem> LoadRuneItems(
            ulong uid)
        {
            List<RuneItem> result =
                new List<RuneItem>();

            result.Add(
                new RuneItem
                {
                    Slot = 1,
                    ItemID = 4039609,
                    Unknown = 0
                });

            result.Add(
                new RuneItem
                {
                    Slot = 2,
                    ItemID = 4037709,
                    Unknown = 0
                });

            result.Add(
                new RuneItem
                {
                    Slot = 3,
                    ItemID = 4071409,
                    Unknown = 0
                });

            result.Add(
                new RuneItem
                {
                    Slot = 4,
                    ItemID = 4035209,
                    Unknown = 0
                });

            result.Add(
                new RuneItem
                {
                    Slot = 5,
                    ItemID = 4030309,
                    Unknown = 0
                });

            result.Add(
                new RuneItem
                {
                    Slot = 6,
                    ItemID = 0,
                    Unknown = 0
                });

            return result;
        }
        public static List<RuneItem> LoadRuneItemsOne(
            ulong uid)
        {
            List<RuneItem> result =
                new List<RuneItem>();

            result.Add(
                new RuneItem
                {
                    Slot = 1,
                    ItemID = 4039609,
                    Unknown = 1
                });

            result.Add(
                new RuneItem
                {
                    Slot = 2,
                    ItemID = 0,
                    Unknown = 0
                });

            result.Add(
                new RuneItem
                {
                    Slot = 3,
                    ItemID = 0,
                    Unknown = 0
                });

            result.Add(
                new RuneItem
                {
                    Slot = 4,
                    ItemID = 0,
                    Unknown = 0
                });

            result.Add(
                new RuneItem
                {
                    Slot = 5,
                    ItemID = 0,
                    Unknown = 0
                });

            result.Add(
                new RuneItem
                {
                    Slot = 6,
                    ItemID = 0,
                    Unknown = 0
                });

            return result;
        }
        public static List<RuneItem> LoadRuneItemsFaild(
            ulong uid)
        {
            List<RuneItem> result =
                new List<RuneItem>();

            result.Add(
                new RuneItem
                {
                    Slot = 1,
                    ItemID = 0,
                    Unknown = 0
                });

            result.Add(
                new RuneItem
                {
                    Slot = 2,
                    ItemID = 0,
                    Unknown = 0
                });

            result.Add(
                new RuneItem
                {
                    Slot = 3,
                    ItemID = 0,
                    Unknown = 0
                });

            result.Add(
                new RuneItem
                {
                    Slot = 4,
                    ItemID = 0,
                    Unknown = 0
                });

            result.Add(
                new RuneItem
                {
                    Slot = 5,
                    ItemID = 0,
                    Unknown = 0
                });

            result.Add(
                new RuneItem
                {
                    Slot = 6,
                    ItemID = 0,
                    Unknown = 0
                });

            return result;
        }
        private static List<RuneItem> LoadCreatedRuneItems()
        {
            List<RuneItem> result =
                new List<RuneItem>();

            result.Add(
                new RuneItem
                {
                    Slot = 1,
                    ItemID = 4039609,
                    Unknown = 0
                });

            return result;
        }
        private static List<RuneItem> LoadCreatedRuneItems2()
        {
            List<RuneItem> result =
                new List<RuneItem>();

            result.Add(
                new RuneItem
                {
                    Slot = 2,
                    ItemID = 4070709,
                    Unknown = 0
                });

            return result;
        }
        private static List<RuneItem> LoadCreatedRuneItems3()
        {
            List<RuneItem> result =
                new List<RuneItem>();

            result.Add(
                new RuneItem
                {
                    Slot = 3,
                    ItemID = 4071409,
                    Unknown = 0
                });

            return result;
        }
        private static List<RuneItem> LoadCreatedRuneItems4()
        {
            List<RuneItem> result =
                new List<RuneItem>();

            result.Add(
                new RuneItem
                {
                    Slot = 4,
                    ItemID = 4035209,
                    Unknown = 0
                });

            return result;
        }
        private static List<RuneItem> LoadCreatedRuneItems5()
        {
            List<RuneItem> result =
                new List<RuneItem>();

            result.Add(
                new RuneItem
                {
                    Slot = 5,
                    ItemID = 4030309,
                    Unknown = 0
                });

            return result;
        }
        private static List<RuneItem> InitializeRuneSlots()
        {
            List<RuneItem> result =
                new List<RuneItem>();

            result.Add(
                new RuneItem
                {
                    Slot = 1,
                    ItemID = 0,
                    Unknown = 0
                });

            result.Add(
                new RuneItem
                {
                    Slot = 2,
                    ItemID = 0,
                    Unknown = 0
                });

            result.Add(
                new RuneItem
                {
                    Slot = 3,
                    ItemID = 0,
                    Unknown = 0
                });

            result.Add(
                new RuneItem
                {
                    Slot = 4,
                    ItemID = 0,
                    Unknown = 0
                });

            result.Add(
                new RuneItem
                {
                    Slot = 5,
                    ItemID = 0,
                    Unknown = 0
                });

            result.Add(
                new RuneItem
                {
                    Slot = 6,
                    ItemID = 0,
                    Unknown = 0
                });

            return result;
        }
    }
}
