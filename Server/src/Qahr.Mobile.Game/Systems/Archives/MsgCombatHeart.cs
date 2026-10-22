using Qahr.Mobile.Client;
using ProtoBuf;
using System.Collections.Generic;
using System.Linq;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class MsgCombatHeart
    {
        [ProtoContract]
        public class MsgCombatHeartPB
        {
            public enum ActionID : uint
            {
                Active = 1,
                Unk1 = 2,
                Update = 3,
                Unk3 = 4,
                Unk4 = 5,
            }
            [ProtoMember(1, IsRequired = true)]
            public ActionID Action;

            [ProtoMember(2, IsRequired = true)]
            public uint Level;

            [ProtoMember(3, IsRequired = true)]
            public long Progress;

            [ProtoMember(4, IsRequired = true)]
            public List<CMsgCombatHeartItemPB> Info;

            [ProtoMember(5, IsRequired = true)]
            public uint UID;
        }
        [ProtoContract]
        public class CMsgCombatHeartItemPB
        {
            [ProtoMember(1, IsRequired = true)]
            public uint UID = 0;

            [ProtoMember(2, IsRequired = true)]
            public uint Count = 0;
        }
        public static unsafe ServerSockets.Packet CreateCombatHeart(this ServerSockets.Packet stream, MsgCombatHeart.MsgCombatHeartPB obj)
        {

            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.CMsgCombatHeart);
            return stream;
        }
        public static unsafe void GetCombatHeart(this ServerSockets.Packet stream, out MsgCombatHeart.MsgCombatHeartPB pQuery)
        {
            pQuery = new MsgCombatHeart.MsgCombatHeartPB();
            pQuery = stream.ProtoBufferDeserialize<MsgCombatHeart.MsgCombatHeartPB>(pQuery);
        }
        public static readonly long[] UpdateProgress = new long[] { 69984, 139968, 209952, 279936, 349920 };
        [PacketAttribute(GamePackets.CMsgCombatHeart)]
        private unsafe static void Process(GameClient client, ServerSockets.Packet stream)
        {
            if (client.InTrade  || client.IsVendor)
                return;
            MsgCombatHeart.MsgCombatHeartPB pQuery;
            stream.GetCombatHeart(out pQuery);
            switch (pQuery.Action)
            {
                case MsgCombatHeartPB.ActionID.Update:
                    {

                        uint Progress = 0;
                        MsgGameItem ITEM;
                        foreach (var items in pQuery.Info)
                        {
                            if (client.Inventory.TryGetItem(items.UID, out ITEM))
                            {
                                if (client.Inventory.Contain(ITEM.ITEM_ID, (ushort)items.Count))
                                {
                                    Progress += MsgCombatGearOpt.ProtoStructure.GetExp(MsgCombatGearOpt.ProtoStructure.TypeExp.item, ITEM.ITEM_ID, items.Count);
                                    Progress += MsgCombatGearOpt.ProtoStructure.GetExp(MsgCombatGearOpt.ProtoStructure.TypeExp.Plus, ITEM.Plus);
                                    Progress += MsgCombatGearOpt.ProtoStructure.GetExp(MsgCombatGearOpt.ProtoStructure.TypeExp.Super, ITEM.ITEM_ID);
                                    if (ITEM.SocketTwo != 0)
                                        Progress += 400;
                                    else if (ITEM.SocketOne != 0)
                                        Progress += 100;
                                    client.Inventory.RemoveStackItem(ITEM.ITEM_ID, (ushort)items.Count, stream);
                                }
                            }
                        }

                        client.MyArchives.LevelCombatProgress += (int)Progress;
                        while (client.MyArchives.LevelCombatHeart < UpdateProgress.Length && client.MyArchives.LevelCombatProgress >= UpdateProgress[client.MyArchives.LevelCombatHeart] && client.MyArchives.LevelCombatHeart < 5)
                        {
                            client.MyArchives.LevelCombatHeart++;
                        }
                        pQuery.UID = client.Player.UID;
                        pQuery.Level = client.MyArchives.LevelCombatHeart;
                        pQuery.Progress = client.MyArchives.LevelCombatProgress;
                        client.Send(stream.CreateCombatHeart(pQuery));
                        AddSkillCombatHeart(client);
                        client.MyArchives.UpdateRank();
                        break;
                    }
                default:
                    MyConsole.WriteLine("CMsgCombatHeart Not Find Action: " + pQuery.Action);
                    break;
            }
        }
        public static void AddSkillCombatHeart(Client.GameClient user)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                if (!user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SupremeLeadership))
                {
                    user.MySpells.Add(stream, (ushort)Role.Flags.SpellID.SupremeLeadership, (ushort)user.MyArchives.LevelCombatHeart);
                }
                else
                {
                    user.MySpells.Add(stream, (ushort)Role.Flags.SpellID.SupremeLeadership, (ushort)user.MyArchives.LevelCombatHeart);
                }
                if (!user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DivineAnnihilation))
                {
                    user.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DivineAnnihilation, (ushort)user.MyArchives.LevelCombatHeart);
                }
                else
                {
                    user.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DivineAnnihilation, (ushort)user.MyArchives.LevelCombatHeart);
                }
            }
        }
    }

}
