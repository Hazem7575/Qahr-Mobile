using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using Qahr.Mobile.Database;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class MsgRankMemberShow
    {
        [Flags]
        public enum Mode : uint
        {
            Show = 0,
            Viewer = 1
        }
        [ProtoContract]
        public class MsgRankMemberShowProto
        {
            [ProtoMember(1, IsRequired = true)]
            public uint Type;
            [ProtoMember(2, IsRequired = true)]
            public Entry[] Items;
            [ProtoContract]
            public class Entry
            {
                [ProtoMember(1, IsRequired = true)]
                public uint Type;
                [ProtoMember(2, IsRequired = true)]
                public uint Rank;
                [ProtoMember(3, IsRequired = true)]
                public uint EntityUID;
                [ProtoMember(4, IsRequired = true)]
                public string Name = "";
                [ProtoMember(5, IsRequired = true)]
                public string GuildName = "";
                [ProtoMember(6, IsRequired = true)]
                public uint Mesh;
                [ProtoMember(7, IsRequired = true)]
                public uint HairStyle;
                [ProtoMember(8, IsRequired = true)]
                public uint Head;
                [ProtoMember(9, IsRequired = true)]
                public uint Garment;
                [ProtoMember(10, IsRequired = true)]
                public uint LeftWeapon;
                [ProtoMember(11, IsRequired = true)]
                public uint LefttWeaponAccessory;
                [ProtoMember(12, IsRequired = true)]
                public uint RightWeapon;
                [ProtoMember(13, IsRequired = true)]
                public uint RightWeaponAccessory;
                [ProtoMember(14, IsRequired = true)]
                public uint MountArmor;
                [ProtoMember(15, IsRequired = true)]
                public uint Armor;
                [ProtoMember(16, IsRequired = true)]
                public uint Wing;
                [ProtoMember(17, IsRequired = true)]
                public uint WingPlus;
                [ProtoMember(18, IsRequired = true)]
                public uint Title;
                [ProtoMember(19, IsRequired = true)]
                public uint Flag;
                [ProtoMember(20, IsRequired = true)]
                public uint Profession;
                [ProtoMember(21, IsRequired = true)]
                public uint WeaponWar;
                [ProtoMember(22, IsRequired = true)]
                public uint WeaponLevel;
                [ProtoMember(23, IsRequired = true)]
                public uint unk22222;
                [ProtoMember(24, IsRequired = true)]
                public long unk222222;
            }
        }
        public static unsafe ServerSockets.Packet CreateMsgRankMemberShow(this ServerSockets.Packet stream, MsgRankMemberShowProto obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgRankMemberShow);

            return stream;
        }
        public static unsafe void GetRankMemberShow(this ServerSockets.Packet stream, out MsgRankMemberShowProto pQuery)
        {
            pQuery = new MsgRankMemberShowProto();
            pQuery = stream.ProtoBufferDeserialize<MsgRankMemberShowProto>(pQuery);
        }
        [PacketAttribute(GamePackets.MsgRankMemberShow)]
        private unsafe static void Process(Client.GameClient client, ServerSockets.Packet stream)
        {

            MsgRankMemberShowProto pQuery;
            stream.GetRankMemberShow(out pQuery);
            switch ((Mode)pQuery.Type)
            {
                case Mode.Viewer:
                    {
                        Client.GameClient Target;
                        if (Pool.GamePoll.TryGetValue(pQuery.Items[0].EntityUID, out Target))
                        {
                            if (Target.Equipment == null || Target.Equipment.CurentEquip == null) return;

                            client.Send(Target.Player.GetArray(stream, true));
                            foreach (var item in Target.Equipment.CurentEquip)
                            {
                                if (item != null)
                                {
                                    client.Send(stream.ItemViewCreate(Target.Player.UID, 0, item, MsgItemView.ActionMode.ViewEquip));
                                    item.SendItemExtra(client, stream);
                                }
                            }
                            MsgUserAbilityScore.UserAbilityScore info = new MsgUserAbilityScore.UserAbilityScore();
                            info.type = 1;
                            info.Level = Target.Player.Level;
                            info.UID = Target.Player.UID;
                            info.Items = new MsgUserAbilityScore.AbilityScore[Target.PrestigePoints.Length];
                            for (int x = 0; x < Target.PrestigePoints.Length; x++)
                            {
                                info.Items[x] = new MsgUserAbilityScore.AbilityScore();
                                info.Items[x].Position = (uint)(x + 1);
                                info.Items[x].Points = Target.PrestigePoints[x];
                            }
                            client.Send(stream.UserAbilityScoreCreate(info));
                        }
                        else
                        {

                            client.Send(stream.CreateMsgRankMemberShow(new MsgRankMemberShowProto()
                            {
                                Type = (uint)Mode.Viewer,
                                Items = new MsgRankMemberShowProto.Entry[1]
                            }));

                        }
                        break;
                    }
                case Mode.Show:
                    {
                        if (pQuery.Items == null || pQuery.Items.Length == 0) break;
                        switch (pQuery.Items[0].Type)
                        {
                            case 1:
                                {

                                    var BestOf = Database.PrestigeRanking.BestOfTheWorld;
                                    if (BestOf != null)
                                    {
                                        client.Send(stream.CreateMsgRankMemberShow(new MsgRankMemberShowProto()
                                        {
                                            Type = (uint)Mode.Show,

                                            Items = new MsgRankMemberShowProto.Entry[1]
                             {
                                  new MsgRankMemberShowProto.Entry()
                                  {
                                        Type = pQuery.Items[0].Type,
                                        EntityUID = BestOf.UID,
                                        Flag = BestOf.Flag,
                                        
                                        Garment = BestOf.Garment != 0 ? BestOf.Garment : BestOf.Armor,
                                        GuildName = BestOf.GuildName,
                                        HairStyle = BestOf.HairStyle,
                                        Head = BestOf.Head,
                                        LefttWeaponAccessory =BestOf.LefttWeaponAccessory,
                                        LeftWeapon =BestOf.LeftWeapon,
                                        Mesh =BestOf.Mesh,
                                        MountArmor =BestOf.MountArmor,
                                        Name = BestOf.Name,
                                        Rank = 1,
                                        RightWeapon =BestOf.RightWeapon,
                                        RightWeaponAccessory =BestOf.RightWeaponAccessory,
                                        Title = BestOf.Title,
                                        Armor = BestOf.Armor,
                                        Wing = BestOf.Wing,
                                        WingPlus = BestOf.WingPlus
                                        
                                  }
                             }
                                        }));

                                    }
                                    byte prestigeRank = Convert.ToByte(Database.PrestigeRanking.GetMyRank(Database.PrestigeRanking.GetIndex(client.Player.Class), client.Player.UID));
                                    client.Send(stream.CreateProfLevUp(new MsgProfLevUp.MsgProfLevUpProto() { Type = 1, Rank = prestigeRank }));
                                    break;
                                }
                                  
                            case 2://WeaponArchive
                            case 3:
                            case 4:
                            case 5:
                            case 6:
                            case 7:
                            case 8:
                            case 9:
                            case 10:
                            case 11:
                                {
                                    var BestOf = Database.HWRank.BestOf((Database.HWRank.Type)(pQuery.Items[0].Type - 2));
                                    if (BestOf != null)
                                    {
                                        client.Send(stream.CreateMsgRankMemberShow(new MsgRankMemberShowProto()
                                        {
                                            Type = (uint)Mode.Show,

                                            Items = new MsgRankMemberShowProto.Entry[1]
                             {
                                  new MsgRankMemberShowProto.Entry()
                                  {
                                        Type = pQuery.Items[0].Type,
                                        EntityUID = BestOf.UID,
                                        Flag = BestOf.Flag,
                                        Garment =BestOf.Garment,
                                        GuildName = BestOf.GuildName,
                                        HairStyle = BestOf.HairStyle,
                                        Head = BestOf.Head,
                                        LefttWeaponAccessory =BestOf.LefttWeaponAccessory,
                                        LeftWeapon =BestOf.LeftWeapon,
                                        Mesh =BestOf.Mesh,
                                        MountArmor =BestOf.MountArmor,
                                        Name = BestOf.Name,
                                        Rank = 1,
                                        RightWeapon =BestOf.RightWeapon,
                                        RightWeaponAccessory =BestOf.RightWeaponAccessory,
                                        Title = BestOf.Title,
                                        Armor = BestOf.Armor,
                                        Wing = BestOf.Wing,
                                        WingPlus = BestOf.WingPlus
                                        
                                  }
                             }
                                        }));

                                    }
                                    break;
                                }
                            case 76:
                                {
                                    var obj = YuanshenRank.BestOf(YuanshenRank.Type.Overall_EonSpirit);
                                    if (obj == null)
                                        return;
                                    MsgRankMemberShow.MsgRankMemberShowProto YuanshenRanks = new MsgRankMemberShow.MsgRankMemberShowProto();
                                    YuanshenRanks.Type = 0;
                                    YuanshenRanks.Items = new MsgRankMemberShow.MsgRankMemberShowProto.Entry[1];
                                    YuanshenRanks.Items[0] = new MsgRankMemberShow.MsgRankMemberShowProto.Entry();
                                    YuanshenRanks.Items[0].Type = pQuery.Items[0].Type;
                                    YuanshenRanks.Items[0].EntityUID = obj.UID;
                                    YuanshenRanks.Items[0].Flag = obj.Flag;
                                    if (obj.Armor > 0)
                                        YuanshenRanks.Items[0].Garment = obj.Armor;
                                    if (obj.Garment > 0)
                                        YuanshenRanks.Items[0].Garment = obj.Garment;
                                    YuanshenRanks.Items[0].GuildName = obj.GuildName;
                                    YuanshenRanks.Items[0].HairStyle = obj.HairStyle;
                                    YuanshenRanks.Items[0].Head = obj.Head;
                                    YuanshenRanks.Items[0].LefttWeaponAccessory = obj.LefttWeaponAccessory;
                                    YuanshenRanks.Items[0].LeftWeapon = obj.LeftWeapon;
                                    YuanshenRanks.Items[0].Mesh = obj.Mesh;
                                    YuanshenRanks.Items[0].MountArmor = obj.MountArmor;
                                    YuanshenRanks.Items[0].Name = obj.Name;
                                    YuanshenRanks.Items[0].Rank = 1;
                                    YuanshenRanks.Items[0].RightWeapon = obj.RightWeapon;
                                    YuanshenRanks.Items[0].RightWeaponAccessory = obj.RightWeaponAccessory;
                                    YuanshenRanks.Items[0].Title = obj.Title;
                                    YuanshenRanks.Items[0].Armor = 0;
                                    YuanshenRanks.Items[0].Wing = obj.Wing;
                                    YuanshenRanks.Items[0].WingPlus = obj.WingPlus;

                                    YuanshenRanks.Items[0].Profession = obj.Class;
                                    YuanshenRanks.Items[0].WeaponWar = client.MyArchives.isOpen() != null ? (uint)client.MyArchives.isOpen().ItemID : 0;
                                    YuanshenRanks.Items[0].WeaponLevel = client.MyArchives.isOpen() != null ? (uint)client.MyArchives.isOpen().Level : 0;
                                    client.Send(stream.CreateMsgRankMemberShow(YuanshenRanks));
                                    break;
                                }
                            case 12:
                                {
                                    var info = Game.MsgTournaments.MsgArena.Top3.Where(p => p != null).ToArray();
                                    MsgRankMemberShowProto ArenaRank = new MsgRankMemberShowProto();
                                    ArenaRank.Type = 0;
                                    ArenaRank.Items = new MsgRankMemberShowProto.Entry[info.Count()];
                                    for (int x = 0; x < info.Count(); x++)
                                    {
                                        var BestOf = info[x];
                                        if (BestOf == null)
                                            break;
                                        ArenaRank.Items[x] = new MsgRankMemberShowProto.Entry();
                                        ArenaRank.Items[x].Type = pQuery.Items[0].Type;
                                        ArenaRank.Items[x].EntityUID = BestOf.UID;
                                        ArenaRank.Items[x].Flag = BestOf.Flag;
                                        ArenaRank.Items[x].Garment = BestOf.Garment != 0 ? BestOf.Garment : BestOf.Armor;
                                        ArenaRank.Items[x].GuildName = BestOf.GuildName;
                                        ArenaRank.Items[x].HairStyle = BestOf.HairStyle;
                                        ArenaRank.Items[x].Head = BestOf.Head;
                                        ArenaRank.Items[x].LefttWeaponAccessory = BestOf.LefttWeaponAccessory;
                                        ArenaRank.Items[x].LeftWeapon = BestOf.LeftWeapon;
                                        ArenaRank.Items[x].Mesh = BestOf.Mesh;
                                        ArenaRank.Items[x].MountArmor = BestOf.MountArmor;
                                        ArenaRank.Items[x].Name = BestOf.Name;
                                        ArenaRank.Items[x].Rank = (uint)x + 1;
                                        ArenaRank.Items[x].RightWeapon = BestOf.RightWeapon;
                                        ArenaRank.Items[x].RightWeaponAccessory = BestOf.RightWeaponAccessory;
                                        ArenaRank.Items[x].Title = BestOf.Title;
                                        ArenaRank.Items[x].Armor = BestOf.Armor;
                                        ArenaRank.Items[x].Wing = BestOf.Wing;
                                        ArenaRank.Items[x].WingPlus = BestOf.WingPlus;


                                    }
                                    client.Send(stream.CreateMsgRankMemberShow(ArenaRank));
                                    break;
                                }
                            case 13:
                            case 14:
                            case 15:
                            case 16:
                            case 17:
                            case 18:
                                {
                                    var BestOf = Database.NinjaRank.BestOf((Database.NinjaRank.Type)(pQuery.Items[0].Type - 13));
                                    if (BestOf != null)
                                    {
                                        MsgRankMemberShowProto obj = new MsgRankMemberShowProto();
                                        obj.Type = (uint)Mode.Show;
                                        obj.Items = new MsgRankMemberShowProto.Entry[1];
                                        obj.Items[0] = new MsgRankMemberShowProto.Entry();
                                        obj.Items[0].Type = pQuery.Items[0].Type;
                                        obj.Items[0].EntityUID = BestOf.UID;
                                        obj.Items[0].Flag = BestOf.Flag;
                                        if (BestOf.Armor != 0) obj.Items[0].Garment = BestOf.Armor;
                                        if (BestOf.Garment != 0) obj.Items[0].Garment = BestOf.Garment;
                                        obj.Items[0].GuildName = BestOf.GuildName;
                                        obj.Items[0].HairStyle = BestOf.HairStyle;
                                        obj.Items[0].Head = BestOf.Head;
                                        obj.Items[0].LefttWeaponAccessory = BestOf.LefttWeaponAccessory;
                                        obj.Items[0].LeftWeapon = BestOf.LeftWeapon;
                                        obj.Items[0].Mesh = BestOf.Mesh;
                                        obj.Items[0].MountArmor = BestOf.MountArmor;
                                        obj.Items[0].Name = BestOf.Name;
                                        obj.Items[0].Rank = 1;
                                        obj.Items[0].RightWeapon = BestOf.RightWeapon;
                                        obj.Items[0].RightWeaponAccessory = BestOf.RightWeaponAccessory;
                                        obj.Items[0].Title = BestOf.Title;
                                        obj.Items[0].Armor = 0;
                                        obj.Items[0].Wing = BestOf.Wing;
                                        obj.Items[0].WingPlus = BestOf.WingPlus;
                                        client.Send(stream.CreateMsgRankMemberShow(obj));
                                    }
                                    break;
                                }
                            case 19:
                            case 20:
                            case 21:
                            case 22:
                                {
                                    var obj = WarriorRank.BestOf((WarriorRank.Type)((int)pQuery.Items[0].Type - 19));
                                    if (obj == null)
                                        return;
                                    MsgRankMemberShow.MsgRankMemberShowProto WarriorRanks = new MsgRankMemberShow.MsgRankMemberShowProto();
                                    WarriorRanks.Type = 0;
                                    WarriorRanks.Items = new MsgRankMemberShow.MsgRankMemberShowProto.Entry[1];
                                    WarriorRanks.Items[0] = new MsgRankMemberShow.MsgRankMemberShowProto.Entry();
                                    WarriorRanks.Items[0].Type = pQuery.Items[0].Type;
                                    WarriorRanks.Items[0].EntityUID = obj.UID;
                                    WarriorRanks.Items[0].Flag = obj.Flag;
                                    if (obj.Armor > 0)
                                        WarriorRanks.Items[0].Garment = obj.Armor;
                                    if (obj.Garment > 0)
                                        WarriorRanks.Items[0].Garment = obj.Garment;
                                    WarriorRanks.Items[0].GuildName = obj.GuildName;
                                    WarriorRanks.Items[0].HairStyle = obj.HairStyle;
                                    WarriorRanks.Items[0].Head = obj.Head;
                                    WarriorRanks.Items[0].LefttWeaponAccessory = obj.LefttWeaponAccessory;
                                    WarriorRanks.Items[0].LeftWeapon = obj.LeftWeapon;
                                    WarriorRanks.Items[0].Mesh = obj.Mesh;
                                    WarriorRanks.Items[0].MountArmor = obj.MountArmor;
                                    WarriorRanks.Items[0].Name = obj.Name;
                                    WarriorRanks.Items[0].Rank = 1;
                                    WarriorRanks.Items[0].RightWeapon = obj.RightWeapon;
                                    WarriorRanks.Items[0].RightWeaponAccessory = obj.RightWeaponAccessory;
                                    WarriorRanks.Items[0].Title = obj.Title;
                                    WarriorRanks.Items[0].Armor = 0;
                                    WarriorRanks.Items[0].Wing = obj.Wing;
                                    WarriorRanks.Items[0].WingPlus = obj.WingPlus;
                                    WarriorRanks.Items[0].WeaponWar = obj.WeaponWar;
                                    WarriorRanks.Items[0].Profession = obj.Class;
                                    client.Send(stream.CreateMsgRankMemberShow(WarriorRanks));
                                    break;
                                }
                            case 23:
                                return;
                            case 24:
                                return;
                            case 25:
                                return;
                            case 26:
                                {
                                    var obj = ArcherRank.BestOf((ArcherRank.Type)((int)pQuery.Items[0].Type - 26));
                                    if (obj == null)
                                        return;
                                    MsgRankMemberShow.MsgRankMemberShowProto ArcherRanks = new MsgRankMemberShow.MsgRankMemberShowProto();
                                    ArcherRanks.Type = 0;
                                    ArcherRanks.Items = new MsgRankMemberShow.MsgRankMemberShowProto.Entry[1];
                                    ArcherRanks.Items[0] = new MsgRankMemberShow.MsgRankMemberShowProto.Entry();
                                    ArcherRanks.Items[0].Type = pQuery.Items[0].Type;
                                    ArcherRanks.Items[0].EntityUID = obj.UID;
                                    ArcherRanks.Items[0].Flag = obj.Flag;
                                    if (obj.Armor > 0)
                                        ArcherRanks.Items[0].Garment = obj.Armor;
                                    if (obj.Garment > 0)
                                        ArcherRanks.Items[0].Garment = obj.Garment;
                                    ArcherRanks.Items[0].GuildName = obj.GuildName;
                                    ArcherRanks.Items[0].HairStyle = obj.HairStyle;
                                    ArcherRanks.Items[0].Head = obj.Head;
                                    ArcherRanks.Items[0].LefttWeaponAccessory = obj.LefttWeaponAccessory;
                                    ArcherRanks.Items[0].LeftWeapon = obj.LeftWeapon;
                                    ArcherRanks.Items[0].Mesh = obj.Mesh;
                                    ArcherRanks.Items[0].MountArmor = obj.MountArmor;
                                    ArcherRanks.Items[0].Name = obj.Name;
                                    ArcherRanks.Items[0].Rank = 1;
                                    ArcherRanks.Items[0].RightWeapon = obj.RightWeapon;
                                    ArcherRanks.Items[0].RightWeaponAccessory = obj.RightWeaponAccessory;
                                    ArcherRanks.Items[0].Title = obj.Title;
                                    ArcherRanks.Items[0].Armor = 0;
                                    ArcherRanks.Items[0].Wing = obj.Wing;
                                    ArcherRanks.Items[0].WingPlus = obj.WingPlus;
                                    ArcherRanks.Items[0].WeaponWar = obj.WeaponWar;
                                    ArcherRanks.Items[0].Profession = obj.Class;
                                    client.Send(stream.CreateMsgRankMemberShow(ArcherRanks));
                                    break;
                                }
                            case 27:
                            case 28:
                            case 29:
                            case 30:
                                {
                                    var obj = ArcherRank.BestOf((ArcherRank.Type)((int)pQuery.Items[0].Type - 27));
                                    if (obj == null)
                                        return;
                                    MsgRankMemberShow.MsgRankMemberShowProto ArcherRanks = new MsgRankMemberShow.MsgRankMemberShowProto();
                                    ArcherRanks.Type = 0;
                                    ArcherRanks.Items = new MsgRankMemberShow.MsgRankMemberShowProto.Entry[1];
                                    ArcherRanks.Items[0] = new MsgRankMemberShow.MsgRankMemberShowProto.Entry();
                                    ArcherRanks.Items[0].Type = pQuery.Items[0].Type;
                                    ArcherRanks.Items[0].EntityUID = obj.UID;
                                    ArcherRanks.Items[0].Flag = obj.Flag;
                                    if (obj.Armor > 0)
                                        ArcherRanks.Items[0].Garment = obj.Armor;
                                    if (obj.Garment > 0)
                                        ArcherRanks.Items[0].Garment = obj.Garment;
                                    ArcherRanks.Items[0].GuildName = obj.GuildName;
                                    ArcherRanks.Items[0].HairStyle = obj.HairStyle;
                                    ArcherRanks.Items[0].Head = obj.Head;
                                    ArcherRanks.Items[0].LefttWeaponAccessory = obj.LefttWeaponAccessory;
                                    ArcherRanks.Items[0].LeftWeapon = obj.LeftWeapon;
                                    ArcherRanks.Items[0].Mesh = obj.Mesh;
                                    ArcherRanks.Items[0].MountArmor = obj.MountArmor;
                                    ArcherRanks.Items[0].Name = obj.Name;
                                    ArcherRanks.Items[0].Rank = 1;
                                    ArcherRanks.Items[0].RightWeapon = obj.RightWeapon;
                                    ArcherRanks.Items[0].RightWeaponAccessory = obj.RightWeaponAccessory;
                                    ArcherRanks.Items[0].Title = obj.Title;
                                    ArcherRanks.Items[0].Armor = 0;
                                    ArcherRanks.Items[0].Wing = obj.Wing;
                                    ArcherRanks.Items[0].WingPlus = obj.WingPlus;
                                    ArcherRanks.Items[0].WeaponWar = obj.WeaponWar;
                                    ArcherRanks.Items[0].Profession = obj.Class;
                                    client.Send(stream.CreateMsgRankMemberShow(ArcherRanks));
                                    break;
                                }
                            case 31:
                                return;
                            case 32:
                                return;
                            case 33:
                            case 34:
                            case 35:
                            case 36:
                            case 37:
                            case 38:
                            case 39:
                                {
                                    if (AtributesStatus.IsWater(client.Player.Class))
                                    {
                                        var obj = WaterRank.BestOf((WaterRank.Type)((int)pQuery.Items[0].Type - 33));
                                        if (obj == null)
                                            return;
                                        MsgRankMemberShow.MsgRankMemberShowProto TaoRankS = new MsgRankMemberShow.MsgRankMemberShowProto();
                                        TaoRankS.Type = 0;
                                        TaoRankS.Items = new MsgRankMemberShow.MsgRankMemberShowProto.Entry[1];
                                        TaoRankS.Items[0] = new MsgRankMemberShow.MsgRankMemberShowProto.Entry();
                                        TaoRankS.Items[0].Type = pQuery.Items[0].Type;
                                        TaoRankS.Items[0].EntityUID = obj.UID;
                                        TaoRankS.Items[0].Flag = obj.Flag;
                                        if (obj.Armor > 0)
                                            TaoRankS.Items[0].Garment = obj.Armor;
                                        if (obj.Garment > 0)
                                            TaoRankS.Items[0].Garment = obj.Garment;
                                        TaoRankS.Items[0].GuildName = obj.GuildName;
                                        TaoRankS.Items[0].HairStyle = obj.HairStyle;
                                        TaoRankS.Items[0].Head = obj.Head;
                                        TaoRankS.Items[0].LefttWeaponAccessory = obj.LefttWeaponAccessory;
                                        TaoRankS.Items[0].LeftWeapon = obj.LeftWeapon;
                                        TaoRankS.Items[0].Mesh = obj.Mesh;
                                        TaoRankS.Items[0].MountArmor = obj.MountArmor;
                                        TaoRankS.Items[0].Name = obj.Name;
                                        TaoRankS.Items[0].Rank = 1;
                                        TaoRankS.Items[0].RightWeapon = obj.RightWeapon;
                                        TaoRankS.Items[0].RightWeaponAccessory = obj.RightWeaponAccessory;
                                        TaoRankS.Items[0].Title = obj.Title;
                                        TaoRankS.Items[0].Armor = 0;
                                        TaoRankS.Items[0].Wing = obj.Wing;
                                        TaoRankS.Items[0].WingPlus = obj.WingPlus;
                                        TaoRankS.Items[0].WeaponWar = obj.WeaponWar;
                                        TaoRankS.Items[0].Profession = obj.Class;
                                        client.Send(stream.CreateMsgRankMemberShow(TaoRankS));
                                    }
                                    else if (AtributesStatus.IsFire(client.Player.Class))
                                    {
                                        var obj = FireRank.BestOf((FireRank.Type)((int)pQuery.Items[0].Type - 33));
                                        if (obj == null)
                                            return;
                                        MsgRankMemberShow.MsgRankMemberShowProto TaoRankS = new MsgRankMemberShow.MsgRankMemberShowProto();
                                        TaoRankS.Type = 0;
                                        TaoRankS.Items = new MsgRankMemberShow.MsgRankMemberShowProto.Entry[1];
                                        TaoRankS.Items[0] = new MsgRankMemberShow.MsgRankMemberShowProto.Entry();
                                        TaoRankS.Items[0].Type = pQuery.Items[0].Type;
                                        TaoRankS.Items[0].EntityUID = obj.UID;
                                        TaoRankS.Items[0].Flag = obj.Flag;
                                        if (obj.Armor > 0)
                                            TaoRankS.Items[0].Garment = obj.Armor;
                                        if (obj.Garment > 0)
                                            TaoRankS.Items[0].Garment = obj.Garment;
                                        TaoRankS.Items[0].GuildName = obj.GuildName;
                                        TaoRankS.Items[0].HairStyle = obj.HairStyle;
                                        TaoRankS.Items[0].Head = obj.Head;
                                        TaoRankS.Items[0].LefttWeaponAccessory = obj.LefttWeaponAccessory;
                                        TaoRankS.Items[0].LeftWeapon = obj.LeftWeapon;
                                        TaoRankS.Items[0].Mesh = obj.Mesh;
                                        TaoRankS.Items[0].MountArmor = obj.MountArmor;
                                        TaoRankS.Items[0].Name = obj.Name;
                                        TaoRankS.Items[0].Rank = 1;
                                        TaoRankS.Items[0].RightWeapon = obj.RightWeapon;
                                        TaoRankS.Items[0].RightWeaponAccessory = obj.RightWeaponAccessory;
                                        TaoRankS.Items[0].Title = obj.Title;
                                        TaoRankS.Items[0].Armor = 0;
                                        TaoRankS.Items[0].Wing = obj.Wing;
                                        TaoRankS.Items[0].WingPlus = obj.WingPlus;
                                        TaoRankS.Items[0].WeaponWar = obj.WeaponWar;
                                        TaoRankS.Items[0].Profession = obj.Class;
                                        client.Send(stream.CreateMsgRankMemberShow(TaoRankS));
                                    }
                                    break;
                                }
                            case 40:
                                {
                                    var obj = PirateRank.BestOf((PirateRank.Type)((int)pQuery.Items[0].Type - 40));
                                    if (obj == null)
                                        return;
                                    MsgRankMemberShow.MsgRankMemberShowProto TaoRankS = new MsgRankMemberShow.MsgRankMemberShowProto();
                                    TaoRankS.Type = 0;
                                    TaoRankS.Items = new MsgRankMemberShow.MsgRankMemberShowProto.Entry[1];
                                    TaoRankS.Items[0] = new MsgRankMemberShow.MsgRankMemberShowProto.Entry();
                                    TaoRankS.Items[0].Type = pQuery.Items[0].Type;
                                    TaoRankS.Items[0].EntityUID = obj.UID;
                                    TaoRankS.Items[0].Flag = obj.Flag;
                                    if (obj.Armor > 0)
                                        TaoRankS.Items[0].Garment = obj.Armor;
                                    if (obj.Garment > 0)
                                        TaoRankS.Items[0].Garment = obj.Garment;
                                    TaoRankS.Items[0].GuildName = obj.GuildName;
                                    TaoRankS.Items[0].HairStyle = obj.HairStyle;
                                    TaoRankS.Items[0].Head = obj.Head;
                                    TaoRankS.Items[0].LefttWeaponAccessory = obj.LefttWeaponAccessory;
                                    TaoRankS.Items[0].LeftWeapon = obj.LeftWeapon;
                                    TaoRankS.Items[0].Mesh = obj.Mesh;
                                    TaoRankS.Items[0].MountArmor = obj.MountArmor;
                                    TaoRankS.Items[0].Name = obj.Name;
                                    TaoRankS.Items[0].Rank = 1;
                                    TaoRankS.Items[0].RightWeapon = obj.RightWeapon;
                                    TaoRankS.Items[0].RightWeaponAccessory = obj.RightWeaponAccessory;
                                    TaoRankS.Items[0].Title = obj.Title;
                                    TaoRankS.Items[0].Armor = 0;
                                    TaoRankS.Items[0].Wing = obj.Wing;
                                    TaoRankS.Items[0].WingPlus = obj.WingPlus;
                                    TaoRankS.Items[0].WeaponWar = obj.WeaponWar;
                                    TaoRankS.Items[0].Profession = obj.Class;
                                    client.Send(stream.CreateMsgRankMemberShow(TaoRankS));
                                    break;
                                }
                            case 42:
                                {
                                    var obj = PirateRank.BestOf((PirateRank.Type)((int)pQuery.Items[0].Type - 42));
                                    if (obj == null)
                                        return;
                                    MsgRankMemberShow.MsgRankMemberShowProto TaoRankS = new MsgRankMemberShow.MsgRankMemberShowProto();
                                    TaoRankS.Type = 0;
                                    TaoRankS.Items = new MsgRankMemberShow.MsgRankMemberShowProto.Entry[1];
                                    TaoRankS.Items[0] = new MsgRankMemberShow.MsgRankMemberShowProto.Entry();
                                    TaoRankS.Items[0].Type = pQuery.Items[0].Type;
                                    TaoRankS.Items[0].EntityUID = obj.UID;
                                    TaoRankS.Items[0].Flag = obj.Flag;
                                    if (obj.Armor > 0)
                                        TaoRankS.Items[0].Garment = obj.Armor;
                                    if (obj.Garment > 0)
                                        TaoRankS.Items[0].Garment = obj.Garment;
                                    TaoRankS.Items[0].GuildName = obj.GuildName;
                                    TaoRankS.Items[0].HairStyle = obj.HairStyle;
                                    TaoRankS.Items[0].Head = obj.Head;
                                    TaoRankS.Items[0].LefttWeaponAccessory = obj.LefttWeaponAccessory;
                                    TaoRankS.Items[0].LeftWeapon = obj.LeftWeapon;
                                    TaoRankS.Items[0].Mesh = obj.Mesh;
                                    TaoRankS.Items[0].MountArmor = obj.MountArmor;
                                    TaoRankS.Items[0].Name = obj.Name;
                                    TaoRankS.Items[0].Rank = 1;
                                    TaoRankS.Items[0].RightWeapon = obj.RightWeapon;
                                    TaoRankS.Items[0].RightWeaponAccessory = obj.RightWeaponAccessory;
                                    TaoRankS.Items[0].Title = obj.Title;
                                    TaoRankS.Items[0].Armor = 0;
                                    TaoRankS.Items[0].Wing = obj.Wing;
                                    TaoRankS.Items[0].WingPlus = obj.WingPlus;
                                    TaoRankS.Items[0].WeaponWar = obj.WeaponWar;
                                    TaoRankS.Items[0].Profession = obj.Class;
                                    client.Send(stream.CreateMsgRankMemberShow(TaoRankS));
                                    break;
                                }
                            case 47:
                            case 48:
                            case 49:
                            case 50:
                            case 51:
                                {
                                    var obj = LeeLongRank.BestOf((LeeLongRank.Type)((int)pQuery.Items[0].Type - 47));
                                    if (obj == null)
                                        return;
                                    MsgRankMemberShow.MsgRankMemberShowProto LeeLongRanks = new MsgRankMemberShow.MsgRankMemberShowProto();
                                    LeeLongRanks.Type = 0;
                                    LeeLongRanks.Items = new MsgRankMemberShow.MsgRankMemberShowProto.Entry[1];
                                    LeeLongRanks.Items[0] = new MsgRankMemberShow.MsgRankMemberShowProto.Entry();
                                    LeeLongRanks.Items[0].Type = pQuery.Items[0].Type;
                                    LeeLongRanks.Items[0].EntityUID = obj.UID;
                                    LeeLongRanks.Items[0].Flag = obj.Flag;
                                    if (obj.Armor > 0)
                                        LeeLongRanks.Items[0].Garment = obj.Armor;
                                    if (obj.Garment > 0)
                                        LeeLongRanks.Items[0].Garment = obj.Garment;
                                    LeeLongRanks.Items[0].GuildName = obj.GuildName;
                                    LeeLongRanks.Items[0].HairStyle = obj.HairStyle;
                                    LeeLongRanks.Items[0].Head = obj.Head;
                                    LeeLongRanks.Items[0].LefttWeaponAccessory = obj.LefttWeaponAccessory;
                                    LeeLongRanks.Items[0].LeftWeapon = obj.LeftWeapon;
                                    LeeLongRanks.Items[0].Mesh = obj.Mesh;
                                    LeeLongRanks.Items[0].MountArmor = obj.MountArmor;
                                    LeeLongRanks.Items[0].Name = obj.Name;
                                    LeeLongRanks.Items[0].Rank = 1;
                                    LeeLongRanks.Items[0].RightWeapon = obj.RightWeapon;
                                    LeeLongRanks.Items[0].RightWeaponAccessory = obj.RightWeaponAccessory;
                                    LeeLongRanks.Items[0].Title = obj.Title;
                                    LeeLongRanks.Items[0].Armor = 0;
                                    LeeLongRanks.Items[0].Wing = obj.Wing;
                                    LeeLongRanks.Items[0].WingPlus = obj.WingPlus;
                                    LeeLongRanks.Items[0].WeaponWar = obj.WeaponWar;
                                    LeeLongRanks.Items[0].Profession = obj.Class;
                                    client.Send(stream.CreateMsgRankMemberShow(LeeLongRanks));
                                    break;
                                }

                            case 91:
                            case 92:
                            case 93:
                            case 94:
                            case 95:
                                {
                                    var obj = MonkRanks.BestOf((MonkRanks.Type)((int)pQuery.Items[0].Type - 91));
                                    if (obj == null)
                                        return;
                                    MsgRankMemberShow.MsgRankMemberShowProto LeeLongRanks = new MsgRankMemberShow.MsgRankMemberShowProto();
                                    LeeLongRanks.Type = 0;
                                    LeeLongRanks.Items = new MsgRankMemberShow.MsgRankMemberShowProto.Entry[1];
                                    LeeLongRanks.Items[0] = new MsgRankMemberShow.MsgRankMemberShowProto.Entry();
                                    LeeLongRanks.Items[0].Type = pQuery.Items[0].Type;
                                    LeeLongRanks.Items[0].EntityUID = obj.UID;
                                    LeeLongRanks.Items[0].Flag = obj.Flag;
                                    if (obj.Armor > 0)
                                        LeeLongRanks.Items[0].Garment = obj.Armor;
                                    if (obj.Garment > 0)
                                        LeeLongRanks.Items[0].Garment = obj.Garment;
                                    LeeLongRanks.Items[0].GuildName = obj.GuildName;
                                    LeeLongRanks.Items[0].HairStyle = obj.HairStyle;
                                    LeeLongRanks.Items[0].Head = obj.Head;
                                    LeeLongRanks.Items[0].LefttWeaponAccessory = obj.LefttWeaponAccessory;
                                    LeeLongRanks.Items[0].LeftWeapon = obj.LeftWeapon;
                                    LeeLongRanks.Items[0].Mesh = obj.Mesh;
                                    LeeLongRanks.Items[0].MountArmor = obj.MountArmor;
                                    LeeLongRanks.Items[0].Name = obj.Name;
                                    LeeLongRanks.Items[0].Rank = 1;
                                    LeeLongRanks.Items[0].RightWeapon = obj.RightWeapon;
                                    LeeLongRanks.Items[0].RightWeaponAccessory = obj.RightWeaponAccessory;
                                    LeeLongRanks.Items[0].Title = obj.Title;
                                    LeeLongRanks.Items[0].Armor = 0;
                                    LeeLongRanks.Items[0].Wing = obj.Wing;
                                    LeeLongRanks.Items[0].WingPlus = obj.WingPlus;
                                    LeeLongRanks.Items[0].WeaponWar = obj.WeaponWar;
                                    LeeLongRanks.Items[0].Profession = obj.Class;
                                    client.Send(stream.CreateMsgRankMemberShow(LeeLongRanks));
                                    break;
                                }
                            case 54:
                                {
                                    for (int x = 0; x < pQuery.Items.Length; x++)
                                    {
                                        #region RedRoses
                                        if (pQuery.Items[x].Type == 54)
                                        {
                                            var RedRoses = Pool.GirlsFlowersRanking.RedRoses.Values.Where(p => p.Rank == 1 && p.Amount > 0).ToArray().FirstOrDefault();
                                            if (RedRoses != null)
                                            {

                                                pQuery.Items[x].EntityUID = RedRoses.UID;
                                                pQuery.Items[x].Name = RedRoses.Name;
                                                pQuery.Items[x].GuildName = RedRoses.GuildName;
                                                pQuery.Items[x].Mesh = RedRoses.Mesh;
                                                pQuery.Items[x].HairStyle = RedRoses.HairStyle;
                                                pQuery.Items[x].Head = RedRoses.Head;
                                                pQuery.Items[x].Garment = RedRoses.Garment != 0 ? RedRoses.Garment : RedRoses.Armor;
                                                pQuery.Items[x].LeftWeapon = RedRoses.LeftWeapon;
                                                pQuery.Items[x].LefttWeaponAccessory = RedRoses.LefttWeaponAccessory;
                                                pQuery.Items[x].RightWeapon = RedRoses.RightWeapon;
                                                pQuery.Items[x].RightWeaponAccessory = RedRoses.RightWeaponAccessory;
                                                pQuery.Items[x].MountArmor = RedRoses.MountArmor;
                                                pQuery.Items[x].Armor = RedRoses.Armor;
                                                pQuery.Items[x].Wing = RedRoses.Wing;
                                                pQuery.Items[x].WingPlus = RedRoses.WingPlus;
                                                pQuery.Items[x].Title = RedRoses.Title;
                                                pQuery.Items[x].Flag = RedRoses.Flag;
                                            }

                                        }
                                        #endregion
                                        #region Orchids
                                        if (pQuery.Items[x].Type == 55)
                                        {
                                            var Orchids = Pool.GirlsFlowersRanking.Orchids.Values.Where(p => p.Rank == 1 && p.Amount > 0).ToArray().FirstOrDefault();
                                            if (Orchids != null)
                                            {
                                                pQuery.Items[x].EntityUID = Orchids.UID;
                                                pQuery.Items[x].Name = Orchids.Name;
                                                pQuery.Items[x].GuildName = Orchids.GuildName;
                                                pQuery.Items[x].Mesh = Orchids.Mesh;
                                                pQuery.Items[x].HairStyle = Orchids.HairStyle;
                                                pQuery.Items[x].Head = Orchids.Head;
                                                pQuery.Items[x].Garment = Orchids.Garment != 0 ? Orchids.Garment : Orchids.Armor;
                                                pQuery.Items[x].LeftWeapon = Orchids.LeftWeapon;
                                                pQuery.Items[x].LefttWeaponAccessory = Orchids.LefttWeaponAccessory;
                                                pQuery.Items[x].RightWeapon = Orchids.RightWeapon;
                                                pQuery.Items[x].RightWeaponAccessory = Orchids.RightWeaponAccessory;
                                                pQuery.Items[x].MountArmor = Orchids.MountArmor;
                                                pQuery.Items[x].Armor = Orchids.Armor;
                                                pQuery.Items[x].Wing = Orchids.Wing;
                                                pQuery.Items[x].WingPlus = Orchids.WingPlus;
                                                pQuery.Items[x].Title = Orchids.Title;
                                                pQuery.Items[x].Flag = Orchids.Flag;
                                            }
                                        }
                                        #endregion
                                        #region Lilies
                                        if (pQuery.Items[x].Type == 56)
                                        {
                                            var Lilies = Pool.GirlsFlowersRanking.Lilies.Values.Where(p => p.Rank == 1 && p.Amount > 0).ToArray().FirstOrDefault();
                                            if (Lilies != null)
                                            {
                                                pQuery.Items[x].EntityUID = Lilies.UID;
                                                pQuery.Items[x].Name = Lilies.Name;
                                                pQuery.Items[x].GuildName = Lilies.GuildName;
                                                pQuery.Items[x].Mesh = Lilies.Mesh;
                                                pQuery.Items[x].HairStyle = Lilies.HairStyle;
                                                pQuery.Items[x].Head = Lilies.Head;
                                                pQuery.Items[x].Garment = Lilies.Garment != 0 ? Lilies.Garment : Lilies.Armor;
                                                pQuery.Items[x].LeftWeapon = Lilies.LeftWeapon;
                                                pQuery.Items[x].LefttWeaponAccessory = Lilies.LefttWeaponAccessory;
                                                pQuery.Items[x].RightWeapon = Lilies.RightWeapon;
                                                pQuery.Items[x].RightWeaponAccessory = Lilies.RightWeaponAccessory;
                                                pQuery.Items[x].MountArmor = Lilies.MountArmor;
                                                pQuery.Items[x].Armor = Lilies.Armor;
                                                pQuery.Items[x].Wing = Lilies.Wing;
                                                pQuery.Items[x].WingPlus = Lilies.WingPlus;
                                                pQuery.Items[x].Title = Lilies.Title;
                                                pQuery.Items[x].Flag = Lilies.Flag;
                                            }
                                        }
                                        #endregion
                                        #region Tulips
                                        if (pQuery.Items[x].Type == 57)
                                        {
                                            var Tulips = Pool.GirlsFlowersRanking.Tulips.Values.Where(p => p.Rank == 1 && p.Amount > 0).ToArray().FirstOrDefault();
                                            if (Tulips != null)
                                            {
                                                pQuery.Items[x].EntityUID = Tulips.UID;
                                                pQuery.Items[x].Name = Tulips.Name;
                                                pQuery.Items[x].GuildName = Tulips.GuildName;
                                                pQuery.Items[x].Mesh = Tulips.Mesh;
                                                pQuery.Items[x].HairStyle = Tulips.HairStyle;
                                                pQuery.Items[x].Head = Tulips.Head;
                                                pQuery.Items[x].Garment = Tulips.Garment != 0 ? Tulips.Garment : Tulips.Armor;
                                                pQuery.Items[x].LeftWeapon = Tulips.LeftWeapon;
                                                pQuery.Items[x].LefttWeaponAccessory = Tulips.LefttWeaponAccessory;
                                                pQuery.Items[x].RightWeapon = Tulips.RightWeapon;
                                                pQuery.Items[x].RightWeaponAccessory = Tulips.RightWeaponAccessory;
                                                pQuery.Items[x].MountArmor = Tulips.MountArmor;
                                                pQuery.Items[x].Armor = Tulips.Armor;
                                                pQuery.Items[x].Wing = Tulips.Wing;
                                                pQuery.Items[x].WingPlus = Tulips.WingPlus;
                                                pQuery.Items[x].Title = Tulips.Title;
                                                pQuery.Items[x].Flag = Tulips.Flag;
                                            }
                                        }

                                        #endregion
                                        #region Peonies
                                        if (pQuery.Items[x].Type == 62)
                                        {
                                            var Peonies = Pool.GirlsFlowersRanking.Peonies.Values.Where(p => p.Rank == 1 && p.Amount > 0).ToArray().FirstOrDefault();
                                            if (Peonies != null)
                                            {
                                                pQuery.Items[x].EntityUID = Peonies.UID;
                                                pQuery.Items[x].Name = Peonies.Name;
                                                pQuery.Items[x].GuildName = Peonies.GuildName;
                                                pQuery.Items[x].Mesh = Peonies.Mesh;
                                                pQuery.Items[x].HairStyle = Peonies.HairStyle;
                                                pQuery.Items[x].Head = Peonies.Head;
                                                pQuery.Items[x].Garment = Peonies.Garment != 0 ? Peonies.Garment : Peonies.Armor;
                                                pQuery.Items[x].LeftWeapon = Peonies.LeftWeapon;
                                                pQuery.Items[x].LefttWeaponAccessory = Peonies.LefttWeaponAccessory;
                                                pQuery.Items[x].RightWeapon = Peonies.RightWeapon;
                                                pQuery.Items[x].RightWeaponAccessory = Peonies.RightWeaponAccessory;
                                                pQuery.Items[x].MountArmor = Peonies.MountArmor;
                                                pQuery.Items[x].Armor = Peonies.Armor;
                                                pQuery.Items[x].Wing = Peonies.Wing;
                                                pQuery.Items[x].WingPlus = Peonies.WingPlus;
                                                pQuery.Items[x].Title = Peonies.Title;
                                                pQuery.Items[x].Flag = Peonies.Flag;

                                            }
                                        }
                                        #endregion

                                    }
                                    client.Send(stream.CreateMsgRankMemberShow(pQuery));
                                    break;
                                }
                            case 58:
                                {
                                    for (int x = 0; x < pQuery.Items.Length; x++)
                                    {
                                        #region Kiss
                                        if (pQuery.Items[x].Type == 58)
                                        {
                                            var RedRoses = Pool.BoysFlowersRanking.RedRoses.Values.Where(p => p.Rank == 1 && p.Amount > 0).ToArray().FirstOrDefault();
                                            if (RedRoses != null)
                                            {

                                                pQuery.Items[x].EntityUID = RedRoses.UID;
                                                pQuery.Items[x].Name = RedRoses.Name;
                                                pQuery.Items[x].GuildName = RedRoses.GuildName;
                                                pQuery.Items[x].Mesh = RedRoses.Mesh;
                                                pQuery.Items[x].HairStyle = RedRoses.HairStyle;
                                                pQuery.Items[x].Head = RedRoses.Head;
                                                pQuery.Items[x].Garment = RedRoses.Garment != 0 ? RedRoses.Garment : RedRoses.Armor;
                                                pQuery.Items[x].LeftWeapon = RedRoses.LeftWeapon;
                                                pQuery.Items[x].LefttWeaponAccessory = RedRoses.LefttWeaponAccessory;
                                                pQuery.Items[x].RightWeapon = RedRoses.RightWeapon;
                                                pQuery.Items[x].RightWeaponAccessory = RedRoses.RightWeaponAccessory;
                                                pQuery.Items[x].MountArmor = RedRoses.MountArmor;
                                                pQuery.Items[x].Armor = RedRoses.Armor;
                                                pQuery.Items[x].Wing = RedRoses.Wing;
                                                pQuery.Items[x].WingPlus = RedRoses.WingPlus;
                                                pQuery.Items[x].Title = RedRoses.Title;
                                                pQuery.Items[x].Flag = RedRoses.Flag;
                                            }

                                        }
                                        #endregion
                                        #region Beer
                                        if (pQuery.Items[x].Type == 59)
                                        {
                                            var Orchids = Pool.BoysFlowersRanking.Orchids.Values.Where(p => p.Rank == 1 && p.Amount > 0).ToArray().FirstOrDefault();
                                            if (Orchids != null)
                                            {
                                                pQuery.Items[x].EntityUID = Orchids.UID;
                                                pQuery.Items[x].Name = Orchids.Name;
                                                pQuery.Items[x].GuildName = Orchids.GuildName;
                                                pQuery.Items[x].Mesh = Orchids.Mesh;
                                                pQuery.Items[x].HairStyle = Orchids.HairStyle;
                                                pQuery.Items[x].Head = Orchids.Head;
                                                pQuery.Items[x].Garment = Orchids.Garment != 0 ? Orchids.Garment : Orchids.Armor;
                                                pQuery.Items[x].LeftWeapon = Orchids.LeftWeapon;
                                                pQuery.Items[x].LefttWeaponAccessory = Orchids.LefttWeaponAccessory;
                                                pQuery.Items[x].RightWeapon = Orchids.RightWeapon;
                                                pQuery.Items[x].RightWeaponAccessory = Orchids.RightWeaponAccessory;
                                                pQuery.Items[x].MountArmor = Orchids.MountArmor;
                                                pQuery.Items[x].Armor = Orchids.Armor;
                                                pQuery.Items[x].Wing = Orchids.Wing;
                                                pQuery.Items[x].WingPlus = Orchids.WingPlus;
                                                pQuery.Items[x].Title = Orchids.Title;
                                                pQuery.Items[x].Flag = Orchids.Flag;
                                            }
                                        }
                                        #endregion
                                        #region Jade
                                        if (pQuery.Items[x].Type == 60)
                                        {
                                            var Lilies = Pool.BoysFlowersRanking.Lilies.Values.Where(p => p.Rank == 1 && p.Amount > 0).ToArray().FirstOrDefault();
                                            if (Lilies != null)
                                            {
                                                pQuery.Items[x].EntityUID = Lilies.UID;
                                                pQuery.Items[x].Name = Lilies.Name;
                                                pQuery.Items[x].GuildName = Lilies.GuildName;
                                                pQuery.Items[x].Mesh = Lilies.Mesh;
                                                pQuery.Items[x].HairStyle = Lilies.HairStyle;
                                                pQuery.Items[x].Head = Lilies.Head;
                                                pQuery.Items[x].Garment = Lilies.Garment != 0 ? Lilies.Garment : Lilies.Armor;
                                                pQuery.Items[x].LeftWeapon = Lilies.LeftWeapon;
                                                pQuery.Items[x].LefttWeaponAccessory = Lilies.LefttWeaponAccessory;
                                                pQuery.Items[x].RightWeapon = Lilies.RightWeapon;
                                                pQuery.Items[x].RightWeaponAccessory = Lilies.RightWeaponAccessory;
                                                pQuery.Items[x].MountArmor = Lilies.MountArmor;
                                                pQuery.Items[x].Armor = Lilies.Armor;
                                                pQuery.Items[x].Wing = Lilies.Wing;
                                                pQuery.Items[x].WingPlus = Lilies.WingPlus;
                                                pQuery.Items[x].Title = Lilies.Title;
                                                pQuery.Items[x].Flag = Lilies.Flag;
                                            }
                                        }
                                        #endregion
                                        #region Lilies
                                        if (pQuery.Items[x].Type == 61)
                                        {
                                            var Tulips = Pool.BoysFlowersRanking.Tulips.Values.Where(p => p.Rank == 1 && p.Amount > 0).ToArray().FirstOrDefault();
                                            if (Tulips != null)
                                            {
                                                pQuery.Items[x].EntityUID = Tulips.UID;
                                                pQuery.Items[x].Name = Tulips.Name;
                                                pQuery.Items[x].GuildName = Tulips.GuildName;
                                                pQuery.Items[x].Mesh = Tulips.Mesh;
                                                pQuery.Items[x].HairStyle = Tulips.HairStyle;
                                                pQuery.Items[x].Head = Tulips.Head;
                                                pQuery.Items[x].Garment = Tulips.Garment != 0 ? Tulips.Garment : Tulips.Armor;
                                                pQuery.Items[x].LeftWeapon = Tulips.LeftWeapon;
                                                pQuery.Items[x].LefttWeaponAccessory = Tulips.LefttWeaponAccessory;
                                                pQuery.Items[x].RightWeapon = Tulips.RightWeapon;
                                                pQuery.Items[x].RightWeaponAccessory = Tulips.RightWeaponAccessory;
                                                pQuery.Items[x].MountArmor = Tulips.MountArmor;
                                                pQuery.Items[x].Armor = Tulips.Armor;
                                                pQuery.Items[x].Wing = Tulips.Wing;
                                                pQuery.Items[x].WingPlus = Tulips.WingPlus;
                                                pQuery.Items[x].Title = Tulips.Title;
                                                pQuery.Items[x].Flag = Tulips.Flag;
                                            }
                                        }

                                        #endregion
                                        #region Peonies
                                        if (pQuery.Items[x].Type == 62)
                                        {
                                            var Peonies = Pool.BoysFlowersRanking.Peonies.Values.Where(p => p.Rank == 1 && p.Amount > 0).ToArray().FirstOrDefault();
                                            if (Peonies != null)
                                            {
                                                pQuery.Items[x].EntityUID = Peonies.UID;
                                                pQuery.Items[x].Name = Peonies.Name;
                                                pQuery.Items[x].GuildName = Peonies.GuildName;
                                                pQuery.Items[x].Mesh = Peonies.Mesh;
                                                pQuery.Items[x].HairStyle = Peonies.HairStyle;
                                                pQuery.Items[x].Head = Peonies.Head;
                                                pQuery.Items[x].Garment = Peonies.Garment != 0 ? Peonies.Garment : Peonies.Armor;
                                                pQuery.Items[x].LeftWeapon = Peonies.LeftWeapon;
                                                pQuery.Items[x].LefttWeaponAccessory = Peonies.LefttWeaponAccessory;
                                                pQuery.Items[x].RightWeapon = Peonies.RightWeapon;
                                                pQuery.Items[x].RightWeaponAccessory = Peonies.RightWeaponAccessory;
                                                pQuery.Items[x].MountArmor = Peonies.MountArmor;
                                                pQuery.Items[x].Armor = Peonies.Armor;
                                                pQuery.Items[x].Wing = Peonies.Wing;
                                                pQuery.Items[x].WingPlus = Peonies.WingPlus;
                                                pQuery.Items[x].Title = Peonies.Title;
                                                pQuery.Items[x].Flag = Peonies.Flag;

                                            }
                                        }
                                        #endregion

                                    }
                                    client.Send(stream.CreateMsgRankMemberShow(pQuery));
                                    break;

                                }
                            case 64:
                                {
                                    var obj = AstredgeRank.BestOf(AstredgeRank.Type.AstredgeWeekly);
                                    if (obj == null)
                                        return;
                                    MsgRankMemberShow.MsgRankMemberShowProto AstredgeRanks = new MsgRankMemberShow.MsgRankMemberShowProto();
                                    AstredgeRanks.Type = 0;
                                    AstredgeRanks.Items = new MsgRankMemberShow.MsgRankMemberShowProto.Entry[1];
                                    AstredgeRanks.Items[0] = new MsgRankMemberShow.MsgRankMemberShowProto.Entry();
                                    AstredgeRanks.Items[0].Type = pQuery.Items[0].Type;
                                    AstredgeRanks.Items[0].EntityUID = obj.UID;
                                    AstredgeRanks.Items[0].Flag = obj.Flag;
                                    if (obj.Armor > 0)
                                        AstredgeRanks.Items[0].Garment = obj.Armor;
                                    if (obj.Garment > 0)
                                        AstredgeRanks.Items[0].Garment = obj.Garment;
                                    AstredgeRanks.Items[0].GuildName = obj.GuildName;
                                    AstredgeRanks.Items[0].HairStyle = obj.HairStyle;
                                    AstredgeRanks.Items[0].Head = obj.Head;
                                    AstredgeRanks.Items[0].LefttWeaponAccessory = obj.LefttWeaponAccessory;
                                    AstredgeRanks.Items[0].LeftWeapon = obj.LeftWeapon;
                                    AstredgeRanks.Items[0].Mesh = obj.Mesh;
                                    AstredgeRanks.Items[0].MountArmor = obj.MountArmor;
                                    AstredgeRanks.Items[0].Name = obj.Name;
                                    AstredgeRanks.Items[0].Rank = 1;
                                    AstredgeRanks.Items[0].RightWeapon = obj.RightWeapon;
                                    AstredgeRanks.Items[0].RightWeaponAccessory = obj.RightWeaponAccessory;
                                    AstredgeRanks.Items[0].Title = obj.Title;
                                    AstredgeRanks.Items[0].Armor = 0;
                                    AstredgeRanks.Items[0].Wing = obj.Wing;
                                    AstredgeRanks.Items[0].WingPlus = obj.WingPlus;
                                    AstredgeRanks.Items[0].WeaponWar = obj.WeaponWar;
                                    AstredgeRanks.Items[0].Profession = obj.Class;
                                    client.Send(stream.CreateMsgRankMemberShow(AstredgeRanks));
                                    break;
                                }
                            case 66:
                                {
                                    var obj = AstredgeRank.BestOf(AstredgeRank.Type.ViodragonClub);
                                    if (obj == null)
                                        return;
                                    MsgRankMemberShow.MsgRankMemberShowProto AstredgeRanks = new MsgRankMemberShow.MsgRankMemberShowProto();
                                    AstredgeRanks.Type = 0;
                                    AstredgeRanks.Items = new MsgRankMemberShow.MsgRankMemberShowProto.Entry[1];
                                    AstredgeRanks.Items[0] = new MsgRankMemberShow.MsgRankMemberShowProto.Entry();
                                    AstredgeRanks.Items[0].Type = pQuery.Items[0].Type;
                                    AstredgeRanks.Items[0].EntityUID = obj.UID;
                                    AstredgeRanks.Items[0].Flag = obj.Flag;
                                    if (obj.Armor > 0)
                                        AstredgeRanks.Items[0].Garment = obj.Armor;
                                    if (obj.Garment > 0)
                                        AstredgeRanks.Items[0].Garment = obj.Garment;
                                    AstredgeRanks.Items[0].GuildName = obj.GuildName;
                                    AstredgeRanks.Items[0].HairStyle = obj.HairStyle;
                                    AstredgeRanks.Items[0].Head = obj.Head;
                                    AstredgeRanks.Items[0].LefttWeaponAccessory = obj.LefttWeaponAccessory;
                                    AstredgeRanks.Items[0].LeftWeapon = obj.LeftWeapon;
                                    AstredgeRanks.Items[0].Mesh = obj.Mesh;
                                    AstredgeRanks.Items[0].MountArmor = obj.MountArmor;
                                    AstredgeRanks.Items[0].Name = obj.Name;
                                    AstredgeRanks.Items[0].Rank = 1;
                                    AstredgeRanks.Items[0].RightWeapon = obj.RightWeapon;
                                    AstredgeRanks.Items[0].RightWeaponAccessory = obj.RightWeaponAccessory;
                                    AstredgeRanks.Items[0].Title = obj.Title;
                                    AstredgeRanks.Items[0].Armor = 0;
                                    AstredgeRanks.Items[0].Wing = obj.Wing;
                                    AstredgeRanks.Items[0].WingPlus = obj.WingPlus;
                                    AstredgeRanks.Items[0].WeaponWar = obj.WeaponWar;
                                    AstredgeRanks.Items[0].Profession = obj.Class;
                                    client.Send(stream.CreateMsgRankMemberShow(AstredgeRanks));
                                    break;
                                }
                            case 67:
                                {
                                    var obj = AstredgeRank.BestOf(AstredgeRank.Type.LoveForEver);
                                    if (obj == null)
                                        return;
                                    MsgRankMemberShow.MsgRankMemberShowProto AstredgeRanks = new MsgRankMemberShow.MsgRankMemberShowProto();
                                    AstredgeRanks.Type = 0;
                                    AstredgeRanks.Items = new MsgRankMemberShow.MsgRankMemberShowProto.Entry[1];
                                    AstredgeRanks.Items[0] = new MsgRankMemberShow.MsgRankMemberShowProto.Entry();
                                    AstredgeRanks.Items[0].Type = pQuery.Items[0].Type;
                                    AstredgeRanks.Items[0].EntityUID = obj.UID;
                                    AstredgeRanks.Items[0].Flag = obj.Flag;
                                    if (obj.Armor > 0)
                                        AstredgeRanks.Items[0].Garment = obj.Armor;
                                    if (obj.Garment > 0)
                                        AstredgeRanks.Items[0].Garment = obj.Garment;
                                    AstredgeRanks.Items[0].GuildName = obj.GuildName;
                                    AstredgeRanks.Items[0].HairStyle = obj.HairStyle;
                                    AstredgeRanks.Items[0].Head = obj.Head;
                                    AstredgeRanks.Items[0].LefttWeaponAccessory = obj.LefttWeaponAccessory;
                                    AstredgeRanks.Items[0].LeftWeapon = obj.LeftWeapon;
                                    AstredgeRanks.Items[0].Mesh = obj.Mesh;
                                    AstredgeRanks.Items[0].MountArmor = obj.MountArmor;
                                    AstredgeRanks.Items[0].Name = obj.Name;
                                    AstredgeRanks.Items[0].Rank = 1;
                                    AstredgeRanks.Items[0].RightWeapon = obj.RightWeapon;
                                    AstredgeRanks.Items[0].RightWeaponAccessory = obj.RightWeaponAccessory;
                                    AstredgeRanks.Items[0].Title = obj.Title;
                                    AstredgeRanks.Items[0].Armor = 0;
                                    AstredgeRanks.Items[0].Wing = obj.Wing;
                                    AstredgeRanks.Items[0].WingPlus = obj.WingPlus;
                                    AstredgeRanks.Items[0].WeaponWar = obj.WeaponWar;
                                    AstredgeRanks.Items[0].Profession = obj.Class;
                                    client.Send(stream.CreateMsgRankMemberShow(AstredgeRanks));
                                    break;
                                }
                            case 68:
                                {
                                    var obj = AstredgeRank.BestOf(AstredgeRank.Type.HeartLock);
                                    if (obj == null)
                                        return;
                                    MsgRankMemberShow.MsgRankMemberShowProto AstredgeRanks = new MsgRankMemberShow.MsgRankMemberShowProto();
                                    AstredgeRanks.Type = 0;
                                    AstredgeRanks.Items = new MsgRankMemberShow.MsgRankMemberShowProto.Entry[1];
                                    AstredgeRanks.Items[0] = new MsgRankMemberShow.MsgRankMemberShowProto.Entry();
                                    AstredgeRanks.Items[0].Type = pQuery.Items[0].Type;
                                    AstredgeRanks.Items[0].EntityUID = obj.UID;
                                    AstredgeRanks.Items[0].Flag = obj.Flag;
                                    if (obj.Armor > 0)
                                        AstredgeRanks.Items[0].Garment = obj.Armor;
                                    if (obj.Garment > 0)
                                        AstredgeRanks.Items[0].Garment = obj.Garment;
                                    AstredgeRanks.Items[0].GuildName = obj.GuildName;
                                    AstredgeRanks.Items[0].HairStyle = obj.HairStyle;
                                    AstredgeRanks.Items[0].Head = obj.Head;
                                    AstredgeRanks.Items[0].LefttWeaponAccessory = obj.LefttWeaponAccessory;
                                    AstredgeRanks.Items[0].LeftWeapon = obj.LeftWeapon;
                                    AstredgeRanks.Items[0].Mesh = obj.Mesh;
                                    AstredgeRanks.Items[0].MountArmor = obj.MountArmor;
                                    AstredgeRanks.Items[0].Name = obj.Name;
                                    AstredgeRanks.Items[0].Rank = 1;
                                    AstredgeRanks.Items[0].RightWeapon = obj.RightWeapon;
                                    AstredgeRanks.Items[0].RightWeaponAccessory = obj.RightWeaponAccessory;
                                    AstredgeRanks.Items[0].Title = obj.Title;
                                    AstredgeRanks.Items[0].Armor = 0;
                                    AstredgeRanks.Items[0].Wing = obj.Wing;
                                    AstredgeRanks.Items[0].WingPlus = obj.WingPlus;
                                    AstredgeRanks.Items[0].WeaponWar = obj.WeaponWar;
                                    AstredgeRanks.Items[0].Profession = obj.Class;
                                    client.Send(stream.CreateMsgRankMemberShow(AstredgeRanks));
                                    break;
                                }
                            case 69:
                                {
                                    var obj = AstredgeRank.BestOf(AstredgeRank.Type.WarlordSpear);
                                    if (obj == null)
                                        return;
                                    MsgRankMemberShow.MsgRankMemberShowProto AstredgeRanks = new MsgRankMemberShow.MsgRankMemberShowProto();
                                    AstredgeRanks.Type = 0;
                                    AstredgeRanks.Items = new MsgRankMemberShow.MsgRankMemberShowProto.Entry[1];
                                    AstredgeRanks.Items[0] = new MsgRankMemberShow.MsgRankMemberShowProto.Entry();
                                    AstredgeRanks.Items[0].Type = pQuery.Items[0].Type;
                                    AstredgeRanks.Items[0].EntityUID = obj.UID;
                                    AstredgeRanks.Items[0].Flag = obj.Flag;
                                    if (obj.Armor > 0)
                                        AstredgeRanks.Items[0].Garment = obj.Armor;
                                    if (obj.Garment > 0)
                                        AstredgeRanks.Items[0].Garment = obj.Garment;
                                    AstredgeRanks.Items[0].GuildName = obj.GuildName;
                                    AstredgeRanks.Items[0].HairStyle = obj.HairStyle;
                                    AstredgeRanks.Items[0].Head = obj.Head;
                                    AstredgeRanks.Items[0].LefttWeaponAccessory = obj.LefttWeaponAccessory;
                                    AstredgeRanks.Items[0].LeftWeapon = obj.LeftWeapon;
                                    AstredgeRanks.Items[0].Mesh = obj.Mesh;
                                    AstredgeRanks.Items[0].MountArmor = obj.MountArmor;
                                    AstredgeRanks.Items[0].Name = obj.Name;
                                    AstredgeRanks.Items[0].Rank = 1;
                                    AstredgeRanks.Items[0].RightWeapon = obj.RightWeapon;
                                    AstredgeRanks.Items[0].RightWeaponAccessory = obj.RightWeaponAccessory;
                                    AstredgeRanks.Items[0].Title = obj.Title;
                                    AstredgeRanks.Items[0].Armor = 0;
                                    AstredgeRanks.Items[0].Wing = obj.Wing;
                                    AstredgeRanks.Items[0].WingPlus = obj.WingPlus;
                                    AstredgeRanks.Items[0].WeaponWar = obj.WeaponWar;
                                    AstredgeRanks.Items[0].Profession = obj.Class;
                                    client.Send(stream.CreateMsgRankMemberShow(AstredgeRanks));
                                    break;
                                }
                            #region DuneRanks
                            case 102:
                            case 103:
                            case 104:
                            case 105:
                            case 106:
                                {
                                    var obj = DuneWandererRank.BestOf((DuneWandererRank.Type)((int)pQuery.Items[0].Type - 102));
                                    if (obj == null)
                                    {
                                        return;
                                    }

                                    MsgRankMemberShow.MsgRankMemberShowProto DuneWandererRanks = new MsgRankMemberShow.MsgRankMemberShowProto();
                                    DuneWandererRanks.Type = 0;
                                    DuneWandererRanks.Items = new MsgRankMemberShow.MsgRankMemberShowProto.Entry[1];
                                    DuneWandererRanks.Items[0] = new MsgRankMemberShow.MsgRankMemberShowProto.Entry();
                                    DuneWandererRanks.Items[0].Type = pQuery.Items[0].Type;
                                    DuneWandererRanks.Items[0].EntityUID = obj.UID;
                                    DuneWandererRanks.Items[0].Flag = obj.Flag;
                                    if (obj.Armor > 0)
                                    {
                                        DuneWandererRanks.Items[0].Garment = obj.Armor;
                                    }

                                    if (obj.Garment > 0)
                                    {
                                        DuneWandererRanks.Items[0].Garment = obj.Garment;
                                    }

                                    DuneWandererRanks.Items[0].GuildName = obj.GuildName;
                                    DuneWandererRanks.Items[0].HairStyle = obj.HairStyle;
                                    DuneWandererRanks.Items[0].Head = obj.Head;
                                    DuneWandererRanks.Items[0].LefttWeaponAccessory = obj.LefttWeaponAccessory;
                                    DuneWandererRanks.Items[0].LeftWeapon = obj.LeftWeapon;
                                    DuneWandererRanks.Items[0].Mesh = obj.Mesh;
                                    DuneWandererRanks.Items[0].MountArmor = obj.MountArmor;
                                    DuneWandererRanks.Items[0].Name = obj.Name;
                                    DuneWandererRanks.Items[0].Rank = 1;
                                    DuneWandererRanks.Items[0].RightWeapon = obj.RightWeapon;
                                    DuneWandererRanks.Items[0].RightWeaponAccessory = obj.RightWeaponAccessory;
                                    DuneWandererRanks.Items[0].Title = obj.Title;
                                    DuneWandererRanks.Items[0].Armor = 0;
                                    DuneWandererRanks.Items[0].Wing = obj.Wing;
                                    DuneWandererRanks.Items[0].WingPlus = obj.WingPlus;
                                    DuneWandererRanks.Items[0].WeaponWar = obj.WeaponWar;
                                    DuneWandererRanks.Items[0].Profession = obj.Class;
                                    client.Send(stream.CreateMsgRankMemberShow(DuneWandererRanks));
                                    break;
                                }
                            #endregion
                        }
                        break;
                    }
            }
        }
    }
}