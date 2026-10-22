using Qahr.Mobile.Client;
using Qahr.Mobile.Database;
using Qahr.Mobile.Database.DBActions;
using Qahr.Mobile.Game.MsgServer;
using Qahr.Mobile.Game.MsgServer.AttackHandler;
using Qahr.Mobile.Game.MsgServer.AttackHandler.Calculate;
using Qahr.Mobile.ServerSockets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Qahr.Mobile.Role.Instance
{
    public class Astredge
    {
        [Flags]
        public enum TypeID : uint
        {
            None = 0,
            ViodragonClub = 1,
            LoveForEver = 2,
            HeartLock = 3,
            WarlordSpear = 4
        }

        public ConcurrentDictionary<TypeID, Item> Items;
        public Client.GameClient user;

        public Astredge(Client.GameClient _user)
        {
            user = _user;
            Items = new ConcurrentDictionary<TypeID, Item>();
        }
        public class Item
        {
            public uint UserID;
            public TypeID ItemID;
            public byte Level;
            public uint Progress;

            public god_weapons_exp.EXPAstredge DBItem
            {
                get
                {
                    god_weapons_exp.EXPAstredge Type;
                    if (!god_weapons_exp.TryGetValue((uint)this.ItemID, this.Level, out Type))
                        return new god_weapons_exp.EXPAstredge();

                    return Type;
                }
            }

            public god_weapons_type.ItemsAstredge Astredge
            {
                get
                {
                    god_weapons_type.ItemsAstredge Type;
                    if (!god_weapons_type.TryGetValue((uint)this.ItemID, this.Level, out Type))
                        return new god_weapons_type.ItemsAstredge();

                    return Type;
                }
            }
        }

        public void AddItem(TypeID ID, byte Level)
        {
            if (Level > 20) Level = 20;

            if (!Items.ContainsKey(ID))
            {
                Item item = new Item
                {
                    UserID = user.Player.UID,
                    ItemID = ID,
                    Level = Level
                };

                Items[ID] = item;

                GET_Skill(item.ItemID);

                CMsgGodWeapons.CMsgGodWeaponsPB.Add(user, item);

                using (ServerSockets.RecycledPacket rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    var OpenDialog = new ActionQuery()
                    {
                        ObjId = user.Player.UID,
                        dwParam = 3856,
                        Type = 126,
                        PositionX = user.Player.X,
                        PositionY = user.Player.Y
                    };

                    user.Send(stream.ActionCreate(OpenDialog));
                }
            }
        }

        public void GET_Skill(TypeID ID)
        {
            using (ServerSockets.RecycledPacket rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                if (Items.ContainsKey(ID))
                {
                    var list = user.MyAstredge.Items.Values
     .Where(p => p.ItemID >= TypeID.ViodragonClub &&
                 p.ItemID <= TypeID.WarlordSpear)
     .ToArray();

                    foreach (var Item in list)
                    {
                        god_weapons_type.ItemsAstredge Itemss;

                        if (Database.god_weapons_type.TryGetValue((uint)Item.ItemID, Item.Level, out Itemss))
                        {
                            if (Itemss.SkillFirst[0] > 0)
                            {
                                user.MySpells.Add(stream,
                                    (ushort)Itemss.SkillFirst[0],
                                    (byte)Itemss.SkillFirst[1]);
                            }

                            if (Itemss.SkillMele[0] > 0)
                            {
                                user.MySpells.Add(stream,
                                    (ushort)Itemss.SkillMele[0],
                                    (byte)Itemss.SkillMele[1]);
                            }
                        }
                    }
                }
            }
        }

        public void Close()
        {
            using (var rec = new RecycledPacket())
            {
                var stream = rec.GetStream();

                user.Send(stream.CreateCMsgGodWeapons(
                    new CMsgGodWeapons.CMsgGodWeaponsPB()
                    {
                        Action = CMsgGodWeapons.CMsgGodWeaponsPB.ActionID.Close2
                    }));

                if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ChaoticDance))
                    user.MySpells.Remove((ushort)Role.Flags.SpellID.ChaoticDance, stream);

                if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SuperSpeedGun))
                    user.MySpells.Remove((ushort)Role.Flags.SpellID.SuperSpeedGun, stream);

                if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ChaoticDanceAttack))
                    user.MySpells.Remove((ushort)Role.Flags.SpellID.ChaoticDanceAttack, stream);
            }
        }

        public uint ViodtagonClub
        {
            get
            {
                uint s = 0;
                foreach (var i in Items.Values)
                    if (i.ItemID == TypeID.ViodragonClub)
                        s += i.Progress;
                return s;
            }
        }

        public uint LoveForEver
        {
            get
            {
                uint s = 0;
                foreach (var i in Items.Values)
                    if (i.ItemID == TypeID.LoveForEver)
                        s += i.Progress;
                return s;
            }
        }

        public uint HeartLock
        {
            get
            {
                uint s = 0;
                foreach (var i in Items.Values)
                    if (i.ItemID == TypeID.HeartLock)
                        s += i.Progress;
                return s;
            }
        }

        public uint WarlordSpear
        {
            get
            {
                uint s = 0;
                foreach (var i in Items.Values)
                    if (i.ItemID == TypeID.WarlordSpear)
                        s += i.Progress;
                return s;
            }
        }

        public uint WeeklyScore
        {
            get
            {
                uint score = 0;

                foreach (var i in Items.Values)
                {
                    if (i == null)
                        continue;

                    switch (i.ItemID)
                    {
                        case TypeID.ViodragonClub:
                        case TypeID.LoveForEver:
                        case TypeID.HeartLock:
                        case TypeID.WarlordSpear:
                            score += i.Progress;
                            break;
                    }
                }

                return score;
            }
        }

        public void UpdateRank()
        {
            AstredgeRank.Ranks[AstredgeRank.Type.ViodragonClub]
                .UpdateItem(CreateEntry(AstredgeRank.Type.ViodragonClub, ViodtagonClub));

            AstredgeRank.Ranks[AstredgeRank.Type.LoveForEver]
                .UpdateItem(CreateEntry(AstredgeRank.Type.LoveForEver, LoveForEver));

            AstredgeRank.Ranks[AstredgeRank.Type.HeartLock]
                .UpdateItem(CreateEntry(AstredgeRank.Type.HeartLock, HeartLock));

            AstredgeRank.Ranks[AstredgeRank.Type.WarlordSpear]
                .UpdateItem(CreateEntry(AstredgeRank.Type.WarlordSpear, WarlordSpear));

            AstredgeRank.Ranks[AstredgeRank.Type.AstredgeWeekly]
                .UpdateItem(CreateEntry(AstredgeRank.Type.AstredgeWeekly, WeeklyScore));
        }

        private AstredgeRank.Entry CreateEntry(AstredgeRank.Type type, uint points)
        {
            AstredgeRank.Entry e = new AstredgeRank.Entry();

            e.Type = type;
            e.TotalPoints = points;
            e.UID = user.Player.UID;
            e.Name = user.Player.Name;
            e.Level = (byte)user.Player.Level;
            e.Class = user.Player.Class;
            e.Mesh = user.Player.Mesh;

            e.AddInfo(user);

            return e;
        }

        public void Loading()
        {
            var items = user.MyAstredge.Items.Values
              .Where(p => p.ItemID >= TypeID.ViodragonClub &&
            p.ItemID <= TypeID.WarlordSpear)
                .ToArray();

            if (items.Length > 0)
            {
                foreach (var item in Items.Values)
                {
                    if (user.Player.Reborn == 2 && user.Player.Level >= 120)
                    {
                        var packet = new CMsgGodWeapons.CMsgGodWeaponsPB()
                        {
                            Action = CMsgGodWeapons.CMsgGodWeaponsPB.ActionID.Login,
                            AstredgeInfo = new List<CMsgGodWeapons.InfoAstredge>(),
                        };

                        packet.AstredgeInfo.Add(new CMsgGodWeapons.InfoAstredge()
                        {
                            Type = item.ItemID,
                            AstredgeLevel = item.Level,
                            AstredgeProgress = item.Progress,
                            GUISKILL = 1,
                            Unk5 = 0
                        });

                        packet.Items[0] = 7803;
                        packet.Items[1] = 7819;
                        packet.Items[2] = 7845;
                        packet.Unk5 = 11;

                        Close();
                        GET_Skill(item.ItemID);

                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            user.Send(stream.CreateCMsgGodWeapons(packet));
                        }
                    }
                    else
                    {
                        Close();
                    }
                }
            }
        }

        public override string ToString()
        {
            WriteLine file = new WriteLine('/');

            file.Add(Items.Count);

            foreach (Item att in Items.Values)
            {
                file.Add((uint)att.ItemID);
                file.Add(att.Level);
                file.Add(att.Progress);
            }

            return file.Close();
        }

        public void Load(string line)
        {
            ReadLine reader = new ReadLine(line, '/');

            int count = reader.Read(0);

            for (int i = 0; i < count; i++)
            {
                Item item = new Item
                {
                    ItemID = (TypeID)reader.Read((uint)0),
                    Level = reader.Read((byte)0),
                    Progress = reader.Read((uint)0)
                };

                if (!Items.ContainsKey(item.ItemID))
                    Items[item.ItemID] = item;
            }
        }
    }
}