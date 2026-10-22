using Qahr.Mobile.Role;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgFloorItem
{
    public unsafe static partial class MsgBuilder
    {
        [ProtoContract]
        public class MsgItemPacketProto
        {
            [ProtoMember(1)]
            public uint m_UID;
            [ProtoMember(2)]
            public uint m_ID;
            [ProtoMember(4)]
            public ushort m_X;
            [ProtoMember(5)]
            public ushort m_Y;
            [ProtoMember(7)]
            public uint Life;
            [ProtoMember(8)]
            public MsgDropID Type;
            [ProtoMember(11)]
            public ushort MaxLife;
            [ProtoMember(12)]
            public uint Plus;//editor By Jo 
            [ProtoMember(14)]
            public uint m_Color;
            [ProtoMember(15)]
            public uint ItemOwnerUID;
            [ProtoMember(16)]
            public uint GuildID;
            [ProtoMember(17)]
            public uint FlowerType;
            [ProtoMember(20)]
            public ulong Timer;
            [ProtoMember(23)]
            public uint OwnerX;
            [ProtoMember(24)]
            public uint OwnerY;
            [ProtoMember(25)]
            public ulong[] Values;
            [ProtoMember(26)]
            public string Name;
            // [ProtoFix 2026-10-14] client CMsgMapItemPB #28 is int32 (varint), was string. Never filled; 0 = not sent.
            [ProtoMember(28)]
            public int Name1;

        }
        public static unsafe void GetItemPacket(this ServerSockets.Packet stream, out uint uid)
        {
            var proto = stream.ProtoBufferDeserialize<MsgItemPacketProto>(new MsgItemPacketProto());
            uid = proto.m_UID;


        }
        public static unsafe ServerSockets.Packet ItemPacketCreate(this ServerSockets.Packet stream, MsgItemPacket Item)
        {
            stream.InitWriter();

            var data = new MsgItemPacketProto
            {
                m_UID = Item.m_UID,
                m_ID = Item.m_ID,
                m_X = Item.m_X,
                m_Y = Item.m_Y,
                MaxLife = Item.MaxLife,
                Type = Item.DropType,
                Life = Item.Life,

                m_Color = Item.m_Color,
                // QahrMobile: the mobile CMsgMapItem reads the effect kind from the plus byte (offset 23), like 6609
                Plus = (Item.DropType == MsgDropID.Effect || Item.DropType == MsgDropID.RemoveEffect) && Item.Plus == 0 ? Item.m_Color : Item.Plus,
                ItemOwnerUID = Item.ItemOwnerUID,
                GuildID = Item.GuildID,
                FlowerType = Item.FlowerType,
                Timer = Item.Timer,
                Values = Item.Value,
                OwnerX = Item.OwnerX,
                OwnerY = Item.OwnerY,
                Name = Item.Name,
            };

            stream.ProtoBufferSerialize(data);
            stream.Finalize(GamePackets.MsgMapItem);
            return stream;
        }
    }

    public unsafe class MsgItemPacket
    {
        public enum EffectMonsters : uint
        {
            None = 0,
            EarthquakeLeftRight = 1,
            EarthquakeUpDown = 2,
            Night = 4,
            EarthquakeAndNight = 5
        }

        public const uint
            DBShowerEffect = 17,
    TwilightDance = 40,
    NormalDaggerStorm = 50,
    SoulOneDaggerStorm = 41,
    SoulTwoDaggerStorm = 42,//46
    InfernalEcho = 1001390,
    WrathoftheEmperor = 1001380,
    AuroraLotus = 930,
    SpaceLeap = 930,
    FlameLotus = 940,
    ImmortalDestroyer = 4480,
    //RageofWar = 1500,
    ShadowofChaser = 1550,
    ClapBomb = 25190,

    HorrorofStomper = 1530,
    PeaceofStomper = 1540,

    LonelyBattle = 5160,
FinalStand = 5200,

    Thundercloud = 3843,
        #region NewMonk
 VajraRing = 4970,
DragonRising1 = 4510,
ZenStaff = 4847,
QuellingRobe = 4960,
FlowerTouch = 4890,
    PalmHill = 4860,
        #endregion
            SeaBurial = 1850,
            BloomOfDeath = 1860,
            TideTrap = 1080,
            SickleWind = 2540,
            FlameofDestruction = 2530,
            WaterShockwave = 2560,
            FireArrow = 3600,
            ChaoticDance = 3530,
            FloraWard = 4030,
            MysticalMelody = 4040,
            SubstitutionFloor = 4050,
            DustDetachment = 2570,
           SandMist = 4130,
            LavaSea = 4150,
            StarVolcano = 4160,
            PheasantBeak = 4170,
            IceAge = 4180,
            LavaSeaPassive = 4190,
            StarVolcanoPassive = 4200,
            PheasantBeakPassive = 4210,
            IceAgePassive = 4180,
            StarChainFire = 4260,
              DragonRising = 4270,
 
            WildFireball = 18456;


        public ulong[] Value;
        public uint m_UID;
        public uint m_ID;
        public ushort m_X;
        public ushort m_Y;
        public ushort MaxLife;
        public MsgDropID DropType;
        public uint Life;
        public byte m_Color;
        public byte m_Color2;
        public uint ItemOwnerUID;
        public byte DontShow;
        public uint GuildID;
        public Role.Flags.ConquerAngle Angle;
        public byte FlowerType;
        public ulong Timer;
        public string Name;
        public uint UnKnow;
        public byte Plus;
        public uint MapID;

        public bool Hide;

        public ushort OwnerX;
        public ushort OwnerY;

        public static MsgItemPacket Create()
        {
            MsgItemPacket item = new MsgItemPacket();
            return item;
        }

        [PacketAttribute(GamePackets.MsgMapItem)]
        public unsafe static void FloorMap(Client.GameClient client, ServerSockets.Packet packet)
        {

            if (client.InTrade)
                return;
            if (!client.Player.OnMyOwnServer)
                return;

            uint m_UID;

            packet.GetItemPacket(out m_UID);

            MsgFloorItem.MsgItem MapItem;
            if (client.Map.View.TryGetObject<MsgFloorItem.MsgItem>(m_UID, Role.MapObjectType.Item, client.Player.X, client.Player.Y, out MapItem))
            {
                if (MapItem.ToMySelf)
                {
                    if (!MapItem.ExpireMySelf)
                    {
                        if (MapItem.ItemOwner != client.Player.UID)
                        {
                            if (client.Team != null)
                            {
                                if (MapItem.Typ != MsgItem.ItemType.Money &&
                                    (!client.Team.IsTeamMember(MapItem.ItemOwner) || !client.Team.PickupItems))
                                {

                                    client.SendSysMesage("You have to wait a little bit before you can pick up any items dropped from monsters killed by other players.");


                                    return;
                                }
                                else if (MapItem.Typ == MsgItem.ItemType.Money)
                                {
                                    if (!client.Team.PickupMoney)
                                    {

                                        client.SendSysMesage("You have to wait a little bit before you can pick up any items dropped from monsters killed by other players.");


                                        return;
                                    }
                                }
                            }
                            else if (client.Team == null)
                            {
                                if (MapItem.Typ == MsgItem.ItemType.Money)
                                {

                                    client.SendSysMesage("You have to wait a little bit before you can pick up any items dropped from monsters killed by other players.");


                                    return;
                                }
                                else
                                {

                                    client.SendSysMesage("You have to wait a little bit before you can pick up any items dropped from monsters killed by other players.");


                                    return;
                                }
                            }
                        }
                    }
                }
                if (Role.Core.GetDistance(client.Player.X, client.Player.Y, MapItem.MsgFloor.m_X, MapItem.MsgFloor.m_Y) <= 5)
                {
                    switch (MapItem.Typ)
                    {

                        case MsgItem.ItemType.Money:
                            {
                                //if (client.PokerPlayer != null)
                                //    return;
                                client.Player.Money += MapItem.Gold;
                                client.Player.SendUpdate(packet, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                MapItem.SendAll(packet, MsgDropID.Remove);
                                client.Map.cells[MapItem.MsgFloor.m_X, MapItem.MsgFloor.m_Y] &= ~Role.MapFlagType.Item;
                                client.Map.View.LeaveMap<Role.IMapObj>(MapItem);

                                client.SendSysMesage("You have picked up a " + MapItem.Gold + " silvers.");



                                break;
                            }
                        case MsgItem.ItemType.Item:
                            {
                                Database.ItemType.DBItem DBItem;
                                if (client.Player.OnAutoHunt == true)
                                {

                                    if (client.Warehouse.ClientItems.ContainsKey(client.Player.UID))
                                    {
                                        if (client.Warehouse.ClientItems[client.Player.UID].Count < client.Player.InventorySashCount)
                                        {
                                            if (Pool.ItemsBase.TryGetValue(MapItem.MsgFloor.m_ID, out DBItem))
                                            {
                                                client.Map.cells[MapItem.MsgFloor.m_X, MapItem.MsgFloor.m_Y] &= ~Role.MapFlagType.Item;
                                                client.Inventory.Add(MapItem.ItemBase, DBItem, packet);
                                                client.Map.View.LeaveMap<Role.IMapObj>(MapItem);
                                                MapItem.SendAll(packet, MsgDropID.Remove);
                                                uint Amount = MapItem.ItemBase.StackSize;

                                                if (Amount > 1)
                                                    client.SendSysMesage("You obtained " + Amount + " " + DBItem.Name + ".", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                                else
                                                    client.SendSysMesage("You obtained " + DBItem.Name + ".", MsgServer.MsgMessage.ChatMode.System);
                                            }
                                            if (DBItem.ID == 711352)
                                            {
                                                client.Player.QuestGUI.IncreaseQuestObjectives(packet, 1311, 1);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (client.Inventory.HaveSpace(1))
                                    {
                                        if (Pool.ItemsBase.TryGetValue(MapItem.MsgFloor.m_ID, out DBItem))
                                        {


                                            client.Map.cells[MapItem.MsgFloor.m_X, MapItem.MsgFloor.m_Y] &= ~Role.MapFlagType.Item;
                                            if (MapItem.ItemBase.StackSize > 1)
                                            {
                                                client.Inventory.Update(MapItem.ItemBase, Role.Instance.AddMode.ADD, packet);
                                            }
                                            else
                                                client.Inventory.Add(MapItem.ItemBase, DBItem, packet);
                                            client.Map.View.LeaveMap<Role.IMapObj>(MapItem);
                                            MapItem.SendAll(packet, MsgDropID.Remove);

                                            uint Amount = MapItem.ItemBase.StackSize;

                                            if (Amount > 1)
                                                client.SendSysMesage("You obtained " + Amount + " " + DBItem.Name + ".", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                            else
                                                client.SendSysMesage("You obtained " + DBItem.Name + ".", MsgServer.MsgMessage.ChatMode.System);
                                            if (DBItem.ID == 711352)
                                            {
                                                client.Player.QuestGUI.IncreaseQuestObjectives(packet, 1311, 1);
                                            }
                                        }
                                    }
                                }

                                break;
                            }
                        case MsgItem.ItemType.Cps:
                            {
                                client.Player.ConquerPoints += (uint)MapItem.ConquerPoints;

                                MapItem.SendAll(packet, MsgDropID.Remove);
                                client.Map.cells[MapItem.MsgFloor.m_X, MapItem.MsgFloor.m_Y] &= ~Role.MapFlagType.Item;
                                client.Map.View.LeaveMap<Role.IMapObj>(MapItem);

                                client.SendSysMesage("You obtained " + Pool.ItemsBase[MapItem.MsgFloor.m_ID].Name + " and Found " + MapItem.ConquerPoints + " CPs.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);


                                break;
                            }
                    }
                }
            }
        }

        public void SendScreen(ServerSockets.Packet stream)
        {
            if (stream == null)
                return;
            foreach (var kv in Pool.GamePoll) // [perf] live lock-free enumeration (no .Values snapshot)
            {
                var Client = kv.Value;
                if (!(Client.Player.Map == MapID && Client.Player.DynamicID == DynamicID && Core.GetDistance(m_X, m_Y, Client.Player.X, Client.Player.Y) <= 20))
                    continue;
                Client.Send(stream);
            }
        }

        public uint DynamicID { get; set; }

        public static uint Disorder { get; set; }
    }
}
