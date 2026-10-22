using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qahr.Mobile.Game.MsgServer
{
    [ProtoContract]
    public class MsgSwordAncestor
    {
        [System.Flags]
        public enum TypeID
        {
            Uknow = 0,
            Loading = 1,
            Upgrade = 3,
            View = 5,
        }
        [ProtoMember(1, IsRequired = true)]
        public uint Type;
        [ProtoMember(2, IsRequired = true)]
        public uint Level;//Level
        [ProtoMember(3, IsRequired = true)]
        public ulong Exp; // [ProtoFix 2026-10-14] client CMsgSwordAncestorPB.Exp is uint64 (was uint)
        [ProtoMember(4, IsRequired = true)]
        public Items[] SelectMaterial;
        [ProtoMember(5, IsRequired = true)]
        public uint UID;
        [ProtoMember(6, IsRequired = true)]
        public bool Unlocked;
        [ProtoContract]
        public class Items
        {
            [ProtoMember(1, IsRequired = true)]
            public uint UID;
            [ProtoMember(2, IsRequired = true)]
            public uint Size;
        }
        [System.Flags]
        public enum TypeExp : uint
        {
            ItemID = 0,
        }
        public static uint GetExp(TypeExp type, uint ID, uint Size)
        {
            uint Exp = 0;
            switch (type)
            {
                case TypeExp.ItemID:
                    {
                        switch (ID)
                        {
                            case 3321098:
                                {
                                    Exp += 10 * Size;
                                    break;
                                }
                        }
                        return Exp;
                    }
            }
            return Exp;
        }
        public ServerSockets.Packet ToArray()
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                stream.InitWriter();
                stream.ProtoBufferSerialize(this);
                stream.Finalize((ushort)Game.GamePackets.MsgPowerFocus);
                return stream;
            }
        }
        public static implicit operator ServerSockets.Packet(MsgSwordAncestor obj)
        {
            return obj.ToArray();
        }
        public static void UpdateSkill(Client.GameClient client)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                var Info = Database.SwordAncestorType.SwordAncestor.Values.Where(i => i.Index == client.HundredWeapons.SwordAncestorLevel + 1).FirstOrDefault();
                if (Info != null)
                {
                    client.MySpells.Remove((ushort)Info.SkillID, stream);
                    client.MySpells.Remove((ushort)Info.SkillID2, stream);
                    client.MySpells.Remove((ushort)Info.SkillID3, stream);
                    client.MySpells.Add(stream, (ushort)Info.SkillID, (ushort)Info.SkillLevel);
                    if (Info.Level == 5)
                    {
                        client.MySpells.Add(stream, (ushort)Info.SkillID2, (ushort)Info.SkillLevel2);
                    }
                    client.MySpells.Add(stream, (ushort)Info.SkillID3, (ushort)Info.SkillLevel3);
                }
            }
        }
        [Packet((ushort)Game.GamePackets.MsgPowerFocus)]
        private static unsafe void Process(Client.GameClient client, ServerSockets.Packet stream)
        {
            var Info = new MsgSwordAncestor();
            Info = stream.ProtoBufferDeserialize(Info);
            switch ((TypeID)Info.Type)
            {
                case TypeID.Upgrade:
                    {
                        Info.UID = client.Player.UID;
                        uint addExp = 0;
                        uint Exp = 0;
                        foreach (var item in Info.SelectMaterial)
                        {
                            Game.MsgServer.MsgGameItem ItemInfo;
                            if (client.Inventory.TryGetItem(item.UID, out ItemInfo))
                            {
                                if (client.Inventory.Contain(ItemInfo.ITEM_ID, (ushort)item.Size))
                                {
                                    addExp += GetExp(TypeExp.ItemID, ItemInfo.ITEM_ID, item.Size);
                                    client.Inventory.RemoveStackItem(ItemInfo.ITEM_ID, (ushort)item.Size, stream);
                                }
                            }
                        }
                        client.HundredWeapons.SwordAncestorExp += addExp;
                        if (client.HundredWeapons.SwordAncestorLevel == 0)
                        {
                            Exp = 500;
                        }
                        if (client.HundredWeapons.SwordAncestorLevel == 1)
                        {
                            Exp = 2000;
                        }
                        if (client.HundredWeapons.SwordAncestorLevel == 2)
                        {
                            Exp = 4000;
                        }
                        if (client.HundredWeapons.SwordAncestorLevel == 3)
                        {
                            Exp = 7000;
                        }
                        if (client.HundredWeapons.SwordAncestorLevel == 4)
                        {
                            Exp = 12000;
                        }
                        if (client.HundredWeapons.SwordAncestorLevel == 5)
                        {
                            Exp = 12000;
                        }
                        byte level = 5;
                        while (client.HundredWeapons.SwordAncestorExp >= Exp && client.HundredWeapons.SwordAncestorLevel < level)
                        {
                            client.HundredWeapons.SwordAncestorLevel++;
                            client.HundredWeapons.SwordAncestorExp = 0;
                        }
                        if (client.HundredWeapons.SwordAncestorLevel >= 5)
                            client.HundredWeapons.SwordAncestorLevel = 5;
                        Info.Exp = client.HundredWeapons.SwordAncestorExp;
                        Info.Level = client.HundredWeapons.SwordAncestorLevel;
                        client.Send(Info);
                        UpdateSkill(client);
                        break;
                    }
                case TypeID.View:
                    {
                        Client.GameClient Target;
                        if (Pool.GamePoll.TryGetValue((uint)Info.UID, out Target))
                        {
                            Info.Level = Target.HundredWeapons.SwordAncestorLevel;
                            Info.Exp = Target.HundredWeapons.SwordAncestorExp;
                            client.Send(Info);
                        }
                        break;
                    }
            }
        }
    }
}
