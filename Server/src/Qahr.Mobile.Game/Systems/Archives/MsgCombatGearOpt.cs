using Qahr.Mobile.Client;
using Qahr.Mobile.Database;
using Qahr.Mobile.Role.Instance;
using Qahr.Mobile.ServerSockets;
using ProtoBuf;
using System;
using System.Linq;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet CreateArchviesInfo(this ServerSockets.Packet stream, MsgCombatGearOpt.ProtoStructure obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgCombatGearOpt);
            return stream;
        }
        public static unsafe void GetArchviesInfo(this ServerSockets.Packet stream, out MsgCombatGearOpt.ProtoStructure pQuery)
        {
            pQuery = new MsgCombatGearOpt.ProtoStructure();
            pQuery = stream.ProtoBufferDeserialize<MsgCombatGearOpt.ProtoStructure>(pQuery);
        }
    }
    public static class MsgCombatGearOpt
    {
        [ProtoContract]
        public class ProtoStructure
        {
            [Flags]
            public enum TypeID
            {
                Upgrade = 0,
                Open = 1,
                Close = 2,
                Transform = 3,
                Cancel = 4,
                Add = 5,
                Remove = 6,
                New = 7,
                UpgradeTaoist = 8,
                IncreaseMastery = 9,
                UpgradeAllTaoist = 10,
                NewRuneSolts = 11,
                AddScore = 14,
                UpgradeDune = 15,
                BuyMartials = 16,


            }
            [Flags]
            public enum TypeExp : uint
            {
                item = 0x0,
                Plus = 0x1,
                Super = 0x2
            }
            [ProtoMember(1, IsRequired = true)]
            public uint Type;
            [ProtoMember(2, IsRequired = true)]
            public Archives.TypeID ID;
            [ProtoMember(3, IsRequired = true)]
            public uint Transform = 0;
            [ProtoMember(4, IsRequired = true)]
            public uint UID = 0;
            [ProtoMember(5, IsRequired = true)]
            public uint Exp = 0;
            // [ProtoFix 2026-10-14] client CMsgCombatGearOptPB #6 is a single uint32 (was uint[], never filled).
            // Not IsRequired so 0 is omitted, same bytes as the old null array.
            [ProtoMember(6)]
            public uint U;
            [ProtoMember(7)]
            public ItemProto[] SelectMaterial;
            [ProtoContract]
            public class ItemProto
            {
                [ProtoMember(1, IsRequired = true)]
                public uint UID = 0;
                [ProtoMember(2, IsRequired = true)]
                public uint Size = 0;
            }

            private static uint GetDuneStageProgress(uint level)
            {
                switch ((level - 1) / 3)
                {
                    case 0: return 40;
                    case 1: return 80;
                    case 2: return 150;
                    case 3: return 250;
                    case 4: return 500;
                    case 5: return 1000;
                    case 6: return 1500;
                    case 7: return 2000;
                    case 8: return 2500;
                    default: return 2500;
                }
            }
            public static readonly uint[] Solt = new uint[] { 0, 2400, 7200, 14400 };
            [PacketAttribute(GamePackets.MsgCombatGearOpt)]
            private static void Process(Client.GameClient user, ServerSockets.Packet stream)
            {
                if (user.InTrade || user.IsVendor || !user.Player.Alive)
                    return;
                ProtoStructure obj;

                stream.GetArchviesInfo(out obj);
                switch (obj.Type)
                {
                    case (uint)TypeID.Upgrade:
                        {
                            Archives.Item OBJ;
                            uint Progress = 0;
                            if (user.MyArchives.Items.TryGetValue(obj.ID, out OBJ))
                            {
                                MsgGameItem ITEM;
                                // Dedup the client UID list so one material can't be counted several times.
                                System.Collections.Generic.HashSet<uint> seenCgUIDs = new System.Collections.Generic.HashSet<uint>();
                                foreach (var items in obj.SelectMaterial)
                                {
                                    if (!seenCgUIDs.Add(items.UID))
                                        continue;
                                    // Use the FULL uint size for the check, EXP and removal (no 16-bit cut),
                                    // and reject sizes beyond the stack.
                                    uint size = items.Size;
                                    if (user.Inventory.TryGetItem(items.UID, out ITEM)
                                        && size >= 1 && size <= ushort.MaxValue && size <= ITEM.StackSize)
                                    {
                                        if (user.Inventory.Contain(ITEM.ITEM_ID, size)
                                            || user.Inventory.Contain(ITEM.ITEM_ID, size, 1))
                                        {
                                            Progress += GetExp(TypeExp.item, ITEM.ITEM_ID, size);
                                            if (Progress == 0)
                                            {
                                                user.CreateBoxDialog("This Item Can't Use Here");
                                                break;
                                            }
                                            if (ITEM.SocketTwo != 0)
                                                Progress += 400;
                                            else if (ITEM.SocketOne != 0)
                                                Progress += 100;
                                            user.Inventory.RemoveStackItem(ITEM.ITEM_ID, (ushort)size, stream);
                                        }
                                    }
                                }
                                obj.UID = 1;
                                OBJ.Progress += Progress;
                                byte level = 0;
                                if (OBJ.ItemID >= Role.Instance.Archives.TypeID.Dragonhowl && OBJ.ItemID <= Role.Instance.Archives.TypeID.Redcurse)
                                {
                                    level = 50;
                                }
                                if (OBJ.ItemID >= Role.Instance.Archives.TypeID.StoneCracker && OBJ.ItemID <= Role.Instance.Archives.TypeID.ThornCutter)
                                {
                                    level = 60;
                                }
                                if (OBJ.ItemID >= Role.Instance.Archives.TypeID.HeavenlyTiger && OBJ.ItemID <= Role.Instance.Archives.TypeID.CosmicRoc)
                                {
                                    level = 60;
                                }
                                if (OBJ.ItemID >= Role.Instance.Archives.TypeID.Dragon && OBJ.ItemID <= Role.Instance.Archives.TypeID.Suanni)
                                {
                                    level = 60;
                                }
                                while (OBJ.Progress >= OBJ.DBItem.Exp && OBJ.Level < level)
                                {
                                    ++OBJ.Level;
                                }
                                if (OBJ.Progress >= 1000000000)
                                    OBJ.Progress = 1000000000;
                                MsgCombatGear.ProtoStructure.Create(user, OBJ, MsgCombatGear.ProtoStructure.Action.Update);
                                user.Send(stream.CreateArchviesInfo(obj));
                                if (user.MyArchives.isOpen() != null)
                                    user.MyArchives.Open(user.MyArchives.isOpen().ItemID);
                                user.MyArchives.LoadingCombatHeart();
                                user.MyArchives.UpdateRank();
                                user.MyArchives.Check_Heaven_Taoist();
                            }
                            break;
                        }

                    #region Open
                    case (uint)TypeID.Open:
                        {
                            #region Warrior
                            if (AtributesStatus.IsWarrior(user.Player.Class))
                            {
                                user.MyArchives.Open(obj.ID);
                                obj.UID = 1;
                                user.Send(stream.CreateArchviesInfo(obj));
                                Archives.Item ItemWarrior = user.MyArchives.isOpen();
                                if (ItemWarrior != null)
                                {
                                    obj.Type = (uint)TypeID.Open;
                                    obj.UID = user.Player.UID;
                                    obj.Exp = ItemWarrior.Progress;
                                    obj.Transform = ItemWarrior.dwParam;
                                    user.Player.View.SendView(stream.CreateArchviesInfo(obj), false);
                                }
                            }
                            #endregion
                            #region Archer
                            if (AtributesStatus.IsArcher(user.Player.Class))
                            {

                                user.MyArchives.Open(obj.ID);
                                obj.UID = 1;
                                user.Send(stream.CreateArchviesInfo(obj));
                                Archives.Item ItemArcher = user.MyArchives.isOpen();
                                if (ItemArcher != null)
                                {
                                    obj.Type = (uint)TypeID.New;
                                    obj.UID = user.Player.UID;
                                    obj.Exp = ItemArcher.Progress;
                                    obj.Transform = ItemArcher.dwParam;
                                    user.Player.View.SendView(stream.CreateArchviesInfo(obj), false);
                                }
                            }

                            #endregion
                            #region Tao
                            if (AtributesStatus.IsTaoist(user.Player.Class))
                            {

                                user.MyArchives.Open(obj.ID);
                                obj.UID = 1;
                                user.Send(stream.CreateArchviesInfo(obj));
                                Archives.Item ItemTao = user.MyArchives.isOpen();
                                if (ItemTao != null)
                                {
                                    obj.Type = (uint)TypeID.New;
                                    obj.UID = user.Player.UID;
                                    obj.Exp = ItemTao.Progress;
                                    obj.Transform = ItemTao.dwParam;
                                    user.Player.View.SendView(stream.CreateArchviesInfo(obj), false);
                                }
                            }

                            #endregion

                            #region IsMonk
                            if (AtributesStatus.IsMonk(user.Player.Class))
                            {
                                user.MyArchives.Open(obj.ID);
                                obj.UID = 1;
                                user.Send(stream.CreateArchviesInfo(obj));
                                Archives.Item ItemMonk = user.MyArchives.isOpen();
                                if (ItemMonk != null)
                                {
                                    obj.Type = (uint)TypeID.New;
                                    obj.UID = user.Player.UID;
                                    obj.Exp = ItemMonk.Progress;
                                    obj.Transform = ItemMonk.dwParam;
                                    user.Player.View.SendView(stream.CreateArchviesInfo(obj), false);
                                }
                            }

                            #endregion
                            #region Pirate
                            if (AtributesStatus.IsPirate(user.Player.Class))
                            {

                                user.MyArchives.Open(obj.ID);
                                obj.UID = 1;
                                user.Send(stream.CreateArchviesInfo(obj));
                                Archives.Item ItemTao = user.MyArchives.isOpen();
                                if (ItemTao != null)
                                {
                                    obj.Type = (uint)TypeID.New;
                                    obj.UID = user.Player.UID;
                                    obj.Exp = ItemTao.Progress;
                                    obj.Transform = ItemTao.dwParam;
                                    user.Player.View.SendView(stream.CreateArchviesInfo(obj), false);
                                }
                            }

                            #endregion
                            #region Dune
                            if (AtributesStatus.IsDune(user.Player.Class))
                            {

                                user.MyArchives.Open(obj.ID);
                                obj.UID = 1;
                                user.Send(stream.CreateArchviesInfo(obj));
                                Archives.Item ItemTao = user.MyArchives.isOpen();
                                if (ItemTao != null)
                                {
                                    obj.Type = (uint)TypeID.New;
                                    obj.UID = user.Player.UID;
                                    obj.Exp = ItemTao.Progress;
                                    obj.Transform = ItemTao.dwParam;
                                    user.Player.View.SendView(stream.CreateArchviesInfo(obj), false);
                                }
                            }

                            #endregion
                            user.Equipment.QueryEquipment(user.Equipment.Alternante);

                            break;
                        }
                    #endregion

                    #region Transform
                    case (uint)TypeID.Transform:
                        {
                            Archives.Item obj5;
                            if (AtributesStatus.IsWarrior(user.Player.Class) && user.MyArchives.Items.TryGetValue(obj.ID, out obj5) && obj5.Level == 50)
                            {
                                obj5.dwParam = obj.Transform;

                                MsgCombatGear.ProtoStructure.Create(user, obj5, MsgCombatGear.ProtoStructure.Action.Update);
                                obj.UID = 1;
                                user.Send(stream.CreateArchviesInfo(obj));
                            }
                            break;
                        }
                    #endregion

                    #region Cancel
                    case (uint)TypeID.Cancel:
                        {
                            Archives.Item obj7;
                            if (AtributesStatus.IsWarrior(user.Player.Class) && user.MyArchives.Items.TryGetValue(obj.ID, out obj7) && obj7.Level == 50)
                            {
                                obj7.dwParam = 0;

                                MsgCombatGear.ProtoStructure.Create(user, obj7, MsgCombatGear.ProtoStructure.Action.Update);
                                obj.UID = 1;
                                user.Send(stream.CreateArchviesInfo(obj));
                            }
                            break;
                        }
                    #endregion

                    #region Add
                    case (uint)TypeID.Add:
                        {
                            Archives.Item weapon;
                            if (AtributesStatus.IsWarrior(user.Player.Class) && user.MyArchives.Items.TryGetValue(obj.ID, out weapon) && weapon.Level == 50)
                            {
                                foreach (ItemProto items in obj.SelectMaterial)
                                {
                                    MsgGameItem Item;
                                    if (user.Inventory.TryGetItem(items.UID, out Item))
                                    {
                                        if (items.Size <= 7)
                                        {
                                            if (weapon.Animas.Length >= obj.Transform - 1)
                                                weapon.Animas[(int)obj.Transform - 1].AnimaID[(int)items.Size - 1] = Item.ITEM_ID;
                                            user.Inventory.Remove(Item.ITEM_ID, 1, stream);
                                            MsgCombatGearSkin.Skin.Create(user, weapon);
                                        }
                                        else
                                            break;
                                    }
                                }

                                obj.UID = 1;
                                user.Send(stream.CreateArchviesInfo(obj));
                            }
                            break;
                        }
                    #endregion

                    #region Remove
                    case (uint)TypeID.Remove:
                        {
                            Archives.Item weapon;
                            if (AtributesStatus.IsWarrior(user.Player.Class) && user.MyArchives.Items.TryGetValue(obj.ID, out weapon) && weapon.Level == 50)
                            {
                                foreach (ItemProto items in obj.SelectMaterial)
                                {
                                    if (items.Size <= 7)
                                    {
                                        if (weapon.Animas.Length >= obj.Transform - 1)
                                        {
                                            uint ID = weapon.Animas[(int)obj.Transform - 1].AnimaID[(int)items.Size - 1];
                                            weapon.Animas[(int)obj.Transform - 1].AnimaID[(int)items.Size - 1] = 0;
                                            user.Inventory.AddItemWitchStack(ID, (byte)0, (ushort)1, stream);
                                        }
                                    }
                                    else
                                        break;
                                }

                                MsgCombatGearSkin.Skin.Create(user, weapon);
                                obj.UID = 1;
                                user.Send(stream.CreateArchviesInfo(obj));
                            }
                            break;
                        }
                    #endregion

                    #region UpgradeTao
                    case (uint)TypeID.UpgradeTaoist:
                        {
                            Archives.Item weapon;
                            if (user.MyArchives.Items.TryGetValue(obj.ID, out weapon))
                            {
                                CombatGear.Items combat;
                                if (CombatGear.TryGetValue((uint)weapon.ItemID, weapon.Level, out combat))
                                {
                                    uint cost = combat.Exp - weapon.Progress;
                                    if (user.MyArchives.Reiki >= cost && weapon.Level < 60)
                                    {
                                        user.MyArchives.Reiki -= cost;
                                        user.MyArchives.AddReiki(0);

                                        weapon.Progress = combat.Exp;
                                        weapon.Level++;
                                    }
                                    MsgCombatGear.ProtoStructure.Create(user, weapon, MsgCombatGear.ProtoStructure.Action.Update);
                                    user.Send(stream.CreateArchviesInfo(obj));
                                }
                                user.MyArchives.UpdateRank();
                                if (user.MyArchives.isOpen() != null)
                                    user.MyArchives.Open(user.MyArchives.isOpen().ItemID);
                            }
                            break;
                        }
                    case (uint)TypeID.IncreaseMastery:
                        {
                            Archives.Item weapon;
                            if (user.MyArchives.MasteryPoints >= obj.Transform && user.MyArchives.Items.TryGetValue(obj.ID, out  weapon))
                            {
                                user.MyArchives.MasteryPoints -= obj.Transform;
                                weapon.MasteryPoints += obj.Transform;
                                obj.Transform = weapon.MasteryPoints;
                                obj.UID = 1;
                            }
                            user.Send(stream.CreateArchviesInfo(obj));
                            if (user.MyArchives.isOpen() != null)
                                user.MyArchives.Open(user.MyArchives.isOpen().ItemID);
                            break;
                        }
                    case (uint)TypeID.UpgradeAllTaoist:
                        {
                            Archives.Item weapon;
                            if (user.MyArchives.Items.TryGetValue(obj.ID, out  weapon))
                            {
                                CombatGear.Items combat;
                                if (CombatGear.TryGetValue((uint)weapon.ItemID, weapon.Level, out combat))
                                {
                                    uint cost = combat.Exp - weapon.Progress;
                                    while (user.MyArchives.Reiki >= cost && weapon.Level < 60)
                                    {
                                        user.MyArchives.Reiki -= cost;
                                        user.MyArchives.AddReiki(0);
                                        weapon.Level++;
                                        weapon.Progress = combat.Exp;

                                        CombatGear.TryGetValue((uint)weapon.ItemID, weapon.Level, out combat);
                                        cost = combat.Exp - weapon.Progress;
                                        obj.UID = 1;
                                    }
                                    MsgCombatGear.ProtoStructure.Create(user, weapon, MsgCombatGear.ProtoStructure.Action.Update);
                                    user.Send(stream.CreateArchviesInfo(obj));
                                }
                                user.MyArchives.UpdateRank();

                            }
                            break;
                        }
                    #endregion

                    #region Pirate New Packet Rune Plus++
                    case (uint)TypeID.NewRuneSolts:
                        {
                            Archives.Item weapon;
                            if (AtributesStatus.IsPirate(user.Player.Class) && user.MyArchives.Items.TryGetValue(obj.ID, out weapon))
                            {
                                obj.UID = 1;
                                if (user.MyArchives.UniversalFragment >= Solt[weapon.DBItem.Level] && weapon.Level < 4)
                                {
                                    user.MyArchives.UniversalFragment -= Solt[weapon.DBItem.Level];
                                    MsgCombatGearTao.AddFragemnt(user);
                                    weapon.Level++;
                                    user.Send(stream.CreateTao(new MsgCombatGearTao.ProtoStructure() { Action = MsgCombatGearTao.Action.Into, UniversalFragment = user.MyArchives.UniversalFragment }));
                                    MsgCombatGear.ProtoStructure.Create(user, weapon, MsgCombatGear.ProtoStructure.Action.Update);
                                    user.Send(stream.CreateArchviesInfo(obj));
                                }
                            }
                            user.MyArchives.UpdateNut();
                            break;
                        }
                    #endregion

                    #region PirateNewAddScore
                    case (uint)TypeID.AddScore:
                        {
                            Archives.Item weapon;
                            if (AtributesStatus.IsPirate(user.Player.Class) && user.MyArchives.Items.TryGetValue(obj.ID, out weapon))
                            {
                                if (obj.Exp == 0)
                                {
                                    if (user.MyArchives.UniversalFragment > 0)
                                    {
                                        user.MyArchives.UniversalFragment -= 1;
                                        weapon.MasteryPoints += 15;
                                    }
                                }
                                else if (obj.Exp == 1)
                                {
                                    if (user.MyArchives.UniversalFragment > 0)
                                    {
                                        weapon.MasteryPoints += user.MyArchives.UniversalFragment;
                                        user.MyArchives.UniversalFragment = 0;
                                    }

                                }
                                obj.UID = 1;
                                user.Send(stream.CreateTao(new MsgCombatGearTao.ProtoStructure() { Action = MsgCombatGearTao.Action.Into, UniversalFragment = user.MyArchives.UniversalFragment }));
                                MsgCombatGear.ProtoStructure.Create(user, weapon, MsgCombatGear.ProtoStructure.Action.Update);
                                user.Send(stream.CreateArchviesInfo(obj));
                                user.MyArchives.UpdateRank();
                                MsgCombatGearTao.AddFragemnt(user);
                            }
                            break;
                        }
                    #endregion

                    case 15:
                        {
                            Archives.Item weapon;
                            if (user.MyArchives.Items.TryGetValue(obj.ID, out weapon))
                            {
                                CombatGear.Items combat;
                                if (CombatGear.TryGetValue((uint)weapon.ItemID, weapon.Level, out combat))
                                {
                                    if (obj.Transform == 1)
                                    {
                                        uint cc = GetDuneStageProgress(weapon.Level);

                                        if (user.Player.DunePoints >= cc)
                                        {
                                            user.Player.DunePoints -= cc;
                                            user.MyArchives.AddDunePoints(0);
                                            weapon.Progress += cc;
                                            if (weapon.Progress >= weapon.DBItem.Exp && weapon.Level < 27)
                                            {
                                                weapon.Level++;
                                            }
                                        }
                                    }
                                    else if (obj.Transform == 0)
                                    {
                                        uint cc = weapon.DBItem.Exp - weapon.Progress;
                                        if (user.Player.DunePoints < cc)
                                        {
                                            cc = user.Player.DunePoints;
                                        }
                                        if (user.Player.DunePoints >= cc)
                                        {
                                            user.Player.DunePoints -= cc;
                                            user.MyArchives.AddDunePoints(0);
                                            weapon.Progress += cc;
                                            if (weapon.Progress >= weapon.DBItem.Exp && weapon.Level < 27)
                                            {
                                                weapon.Level++;
                                            }
                                        }
                                    }

                                    user.Send(stream.CreateArchives(new MsgCombatGear.ProtoStructure
                                    {
                                        Type = 6,
                                        UID = user.MyArchives.DuneTotalScore,
                                        Member4 = 0
                                    }));

                                    MsgCombatGear.ProtoStructure.Create(user, weapon, MsgCombatGear.ProtoStructure.Action.Update);
                                    user.Send(stream.CreateArchviesInfo(obj));
                                }
                                user.MyArchives.UpdateRank();
                                if (user.MyArchives.isOpen() != null)
                                    user.MyArchives.Open(user.MyArchives.isOpen().ItemID);
                            }

                            break;
                        }
                    case 16:
                        {
                            uint Cost = 1;
                            uint count = obj.Transform * 15;
                            uint totalCost = Cost * count;

                            if (totalCost < 0)
                                return;

                            if (user.Player.ConquerPoints >= totalCost)
                            {
                                user.Player.ConquerPoints -= totalCost;
                                user.MyArchives.AddDunePoints(count);
                            }
                            else
                            {
                                totalCost = Cost * count / 15;
                                if (user.Player.ConquerPoints >= totalCost)
                                {
                                    user.Player.ConquerPoints -= totalCost;
                                    user.MyArchives.AddDunePoints(count);
                                }
                            }
                            obj.UID = 1;
                            user.Send(stream.CreateArchviesInfo(obj));
                            break;
                        }
                }

            }
            public static uint GetExp(TypeExp type, uint ID = 0, uint Size = 0)
            {

                uint Progress = 0;
                switch (type)
                {
                    default:
                        return Progress;
                    case TypeExp.item:
                        {
                            switch (ID)
                            {
                                case 3335125:
                                    Progress += 120 * Size;
                                    break;
                                case 3335126:
                                    Progress += 480 * Size;
                                    break;
                                case 3335127:
                                    Progress += 1440 * Size;
                                    break;
                                case 3335128:
                                    Progress += 4320 * Size;
                                    break;
                                case 3333400:
                                    Progress += 120 * Size;
                                    break;
                                case 3333401:
                                    Progress += 480 * Size;
                                    break;
                                case 3333402:
                                    Progress += 1440 * Size;
                                    break;
                                case 3333403:
                                    Progress += 4320 * Size;
                                    break;
                                case 3339453:
                                    Progress += 120 * Size;
                                    break;
                                case 3339454:
                                    Progress += 480 * Size;
                                    break;
                                case 3339455:
                                    Progress += 1440 * Size;
                                    break;
                                case 3339456:
                                    Progress += 4320 * Size;
                                    break;


                                case 3351308:
                                    Progress += 120 * Size;
                                    break;
                                case 3351309:
                                    Progress += 480 * Size;
                                    break;
                                case 3351310:
                                    Progress += 1440 * Size;
                                    break;
                                case 3351311:
                                    Progress += 4320 * Size;
                                    break;



                                //case 730001:
                                //    Progress += 40 * Size;
                                //    break;
                                //case 730002:
                                //    Progress += 160 * Size;
                                //    break;
                                //case 730003:
                                //    Progress += 400 * Size;
                                //    break;
                                //case 730004:
                                //    Progress += 1440 * Size;
                                //    break;
                                //case 730005:
                                //    Progress += 4320 * Size;
                                //    break;
                                //case 730006:
                                //    Progress += 12960 * Size;
                                //    break;
                                //case 730007:
                                //    Progress += 38880 * Size;
                                //    break;
                                //case 730008:
                                //    Progress += 116640 * Size;
                                //break;
                            }
                            return Progress;
                        }
                    case TypeExp.Plus:
                        {
                            switch (ID)
                            {
                                case 1:
                                    Progress += 40;
                                    break;
                                case 2:
                                    Progress += 160;
                                    break;
                                case 3:
                                    Progress += 400;
                                    break;
                                case 4:
                                    Progress += 1440;
                                    break;
                                case 5:
                                    Progress += 4320;
                                    break;
                                case 6:
                                    Progress += 12960;
                                    break;
                                case 7:
                                    Progress += 38880;
                                    break;
                                case 8:
                                    Progress += 116640;
                                    break;
                            }
                            return Progress;
                        }
                    case TypeExp.Super:
                        {
                            switch ((byte)(ID % 10))
                            {
                                case 6:
                                    Progress++;
                                    break;
                                case 7:
                                    Progress += 2;
                                    break;
                                case 8:
                                    Progress += 6;
                                    break;
                                case 9:
                                    Progress += 20;
                                    break;
                            }

                            return Progress;
                        }
                }

            }
        }
    }
}