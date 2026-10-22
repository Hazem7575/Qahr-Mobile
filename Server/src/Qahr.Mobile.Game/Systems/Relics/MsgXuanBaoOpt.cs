using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using Qahr.Mobile.Role.Instance;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class RelicLimit
    {
        public const uint CriticalStrike = 2250;
        public const uint SkillCStrike = 2250;
        public const uint Immunity = 2250;
        public const uint Breakthrough = 225;
        public const uint Counteraction = 225;
        public const uint LuckyStrike = 500;
        public const uint MaxHP = 6250;
        public const uint PhysicalAttack = 3500;
        public const uint MagicAttack = 6000;
        public const uint MagicDefense = 1700;
        public const uint FinalPAttack = 4250;
        public const uint FinalMAttack = 3700;
        public const uint FinalPDamage = 4250;
        public const uint FinalMDamage = 3700;
    }
    public static class MsgXuanBaoOpt
    {
        [Flags]
        public enum ActionIds
        {
            FuseRelic = 0,
            SelectRelic = 1,
            BuyRelic = 4,
            Synergy = 5,
            RemoveReconace = 7,//Remove Reconace Relic
            View1 = 8,//Reconace Relic Attrebiute View
            UnLock = 9,//Unlock 2nd Reconance
            View2 = 10,//??
            TransformAttempt = 11,
            TransformRestore = 12,
            Transform = 13,
        }

        [ProtoContract]
        public class XuanBaoOpt
        {
            [ProtoMember(1, IsRequired = true)]
            public uint ActionID;

            [ProtoMember(2, IsRequired = true)]
            public uint MainRelic;

            [ProtoMember(3, IsRequired = true)]
            public uint FuseRelic;

            [ProtoMember(4)]
            public uint BigRateRelic;

            [ProtoMember(5)]
            public uint Member5;

            [ProtoMember(6)]
            public uint Times; // [ProtoFix 2026-10-14] client CMsgXuanBaoOptPB #6 is uint32 (was int)

            [ProtoMember(7)]
            public uint Success;

            // [ProtoFix 2026-10-14] client CMsgXuanBaoOptPB #8 is a STRING, not bool. The old bool (varint) was
            // skipped by the client and made protobuf-net fail if the client ever sent #8. The bool stays as
            // server-side state only; #8 is now a string that is left null (not sent) until its meaning is known.
            [ProtoIgnore]
            public bool Unlocked;

            [ProtoMember(8)]
            public string Field8Text;

            [ProtoMember(9)]
            public List<ItemData> Items = new List<ItemData>();

            [ProtoMember(10)]
            public uint Test10;
        }
        [ProtoContract]
        public class ItemData
        {
            [ProtoMember(1)]
            public uint Id = 0;

            [ProtoMember(2)]
            public uint Size = 0;
        }
        private static readonly uint[][] FuseCost = new[]
     {
        new uint[] { 300000 },
        new uint[] { 300000, 500000},
        new uint[] { 300000, 500000, 1000000},
        new uint[] { 500000, 1000000, 2000000, 5000000},
        new uint[] { 1000000, 2000000, 5000000, 10000000, 20000000},
    };
        private unsafe static uint GetXuanBaoFuseCost(uint totalnum, uint samenum, uint _default)
        {
            if (FuseCost.Length > totalnum && FuseCost[totalnum].Length > samenum)
            {
                return FuseCost[totalnum][samenum];
            }

            return _default;
        }
        public static uint GetMaxAttributte(uint type)
        {
            var ComposeLimitDB = Database.RuneTable.composelimit.Where(i => i.Attribute == (Game.MsgServer.MsgChiInfo.ChiAttribute)type).ToArray().FirstOrDefault();
            if (ComposeLimitDB != null)
            {
                return ComposeLimitDB.Max;
            }

            return 0;
        }
        public static uint GetMinxAttributte(uint type)
        {
            var ComposeLimitDB = Database.RuneTable.composelimit.Where(i => i.Attribute == (Game.MsgServer.MsgChiInfo.ChiAttribute)type).ToArray().FirstOrDefault();
            if (ComposeLimitDB != null)
            {
                return ComposeLimitDB.Min;
            }

            return 0;
        }
        public static uint GetAttributID(uint value, Game.MsgServer.MsgChiInfo.ChiAttribute type, bool new_type = false)
        {
            if (new_type)
            {
                var max_attribute = GetMaxAttributte((uint)type);
                if (max_attribute / 3 < value)
                {
                    return value * 1000 + ((uint)type % 100) + 100;
                }
            }

            return value * 1000 + (uint)type;
        }
        private static int GetRelicAttributesCount(Role.Instance.RelicAttribute[] MainRelic, Role.Instance.RelicAttribute[] FuseRelic)
        {
            int L1Count = MainRelic.Count(i => i.Type != MsgChiInfo.ChiAttribute.None);
            int L2Count = FuseRelic.Count(i => i.Type != MsgChiInfo.ChiAttribute.None);
            if (L1Count == L2Count)
            {
                return L1Count;
            }

            int ResultCount = L1Count;
            if (L2Count > L1Count && L2Count == 5)
            {
                ResultCount = Role.Core.Rate(10) ? Math.Min(Pool.GetRandom.Next(L1Count, L2Count + 1), L2Count) : ((L1Count + L2Count) / 2);
                return ResultCount;
            }
            return ((L1Count + L2Count) / 2);
        }
        public static bool RelicAttribute(uint ID, Game.MsgServer.MsgChiInfo.ChiAttribute attribute)
        {
            if (attribute == MsgChiInfo.ChiAttribute.None)
            {
                return false;
            }

            switch (ID)
            {
                case 4100001://Featured (M-Attack)
                    {
                        if (attribute == Game.MsgServer.MsgChiInfo.ChiAttribute.SkillCriticalStrike
                         || attribute == Game.MsgServer.MsgChiInfo.ChiAttribute.CriticalStrike
                         || attribute == Game.MsgServer.MsgChiInfo.ChiAttribute.HPAdd
                         || attribute == Game.MsgServer.MsgChiInfo.ChiAttribute.AddAttack)
                        {
                            return false;
                        }

                        break;
                    }
                case 4100002://Featured (Max-HP)
                    {
                        if (attribute == Game.MsgServer.MsgChiInfo.ChiAttribute.SkillCriticalStrike
                            || attribute == Game.MsgServer.MsgChiInfo.ChiAttribute.CriticalStrike
                            || attribute == Game.MsgServer.MsgChiInfo.ChiAttribute.AddMagicAttack
                            || attribute == Game.MsgServer.MsgChiInfo.ChiAttribute.AddAttack)
                        {
                            return false;
                        }

                        break;
                    }
                case 4100003://Featured (M-Strike)
                    {
                        if (attribute == Game.MsgServer.MsgChiInfo.ChiAttribute.HPAdd
                            || attribute == Game.MsgServer.MsgChiInfo.ChiAttribute.CriticalStrike
                            || attribute == Game.MsgServer.MsgChiInfo.ChiAttribute.AddMagicAttack
                            || attribute == Game.MsgServer.MsgChiInfo.ChiAttribute.AddAttack)
                        {
                            return false;
                        }

                        break;
                    }
                case 4100004://Featured (P-Attack)
                    {
                        if (attribute == Game.MsgServer.MsgChiInfo.ChiAttribute.HPAdd
                            || attribute == Game.MsgServer.MsgChiInfo.ChiAttribute.CriticalStrike
                            || attribute == Game.MsgServer.MsgChiInfo.ChiAttribute.AddMagicAttack
                            || attribute == Game.MsgServer.MsgChiInfo.ChiAttribute.SkillCriticalStrike)
                        {
                            return false;
                        }

                        break;
                    }
                case 4100005://Featured (P-Strike)
                    {
                        if (attribute == Game.MsgServer.MsgChiInfo.ChiAttribute.HPAdd
                            || attribute == Game.MsgServer.MsgChiInfo.ChiAttribute.AddAttack
                            || attribute == Game.MsgServer.MsgChiInfo.ChiAttribute.AddMagicAttack
                            || attribute == Game.MsgServer.MsgChiInfo.ChiAttribute.SkillCriticalStrike)
                        {
                            return false;
                        }

                        break;
                    }
            }
            return true;
        }
        public static List<uint> XRelicFuse(Game.MsgServer.MsgGameItem MainRelic, Game.MsgServer.MsgGameItem FuseRelic)
        {
            List<Tuple<Game.MsgServer.MsgChiInfo.ChiAttribute, uint>> Attributes = new List<Tuple<Game.MsgServer.MsgChiInfo.ChiAttribute, uint>>();
            foreach (var atrribute in MainRelic.RelicAttributes.Where(i => RelicAttribute(MainRelic.ITEM_ID, i.Type)))
            {
                var new_type = atrribute.Type;
                var new_power = atrribute / 1000;

                int CountX = MainRelic.RelicAttributes.Count(p => p.Type == atrribute.Type);
                var xuanbaoadditionattr = Database.RuneTable.xuanbaoadditionattr.FirstOrDefault(i => i.Type == (byte)atrribute.Type && i.Num == CountX);
                if (xuanbaoadditionattr != null)
                {
                    if (new_power > xuanbaoadditionattr.Value)
                    {
                        new_power -= (ushort)xuanbaoadditionattr.Value;
                    }
                }

                Attributes.Add(new Tuple<Game.MsgServer.MsgChiInfo.ChiAttribute, uint>(new_type, new_power));
            }
            foreach (var atrribute in FuseRelic.RelicAttributes.Where(i => RelicAttribute(MainRelic.ITEM_ID, i.Type)))
            {
                var new_type = atrribute.Type;
                var new_power = atrribute / 1000;

                int CountX = FuseRelic.RelicAttributes.Count(p => p.Type == atrribute.Type);
                var xuanbaoadditionattr = Database.RuneTable.xuanbaoadditionattr.FirstOrDefault(i => i.Type == (byte)atrribute.Type && i.Num == CountX);
                if (xuanbaoadditionattr != null)
                {
                    if (new_power > xuanbaoadditionattr.Value)
                    {
                        new_power -= (ushort)xuanbaoadditionattr.Value;
                    }
                }


                if (!CheckUpAttributes(Attributes, new Tuple<Game.MsgServer.MsgChiInfo.ChiAttribute, uint>(new_type, new_power)))
                {
                    Attributes.Add(new Tuple<Game.MsgServer.MsgChiInfo.ChiAttribute, uint>(new_type, new_power));
                }
            }

            if (Role.Core.Rate(1))
            {
                var new_type = Game.MsgServer.MsgChiInfo.ChiAttribute.LuckyStrike;
                uint new_power = 100;
                Attributes.Add(new Tuple<Game.MsgServer.MsgChiInfo.ChiAttribute, uint>(new_type, new_power));
            }
            List<uint> GenerateAttributes = new List<uint>();
            int Count = GetRelicAttributesCount(MainRelic.RelicAttributes.ToArray(), FuseRelic.RelicAttributes.ToArray());
            for (int x = 0; x < Count; x++)
            {

                var PickAttribut = Attributes.Pick();
                int Duplicate = GenerateAttributes.Count(i => i % 100 == (int)PickAttribut.Item1);
                while (Duplicate > 0 && !Role.Core.Rate(35 / Duplicate == 0 ? 1 : Duplicate))
                {
                    Duplicate = GenerateAttributes.Count(i => i % 100 == (int)PickAttribut.Item1);
                    PickAttribut = Attributes.Pick();
                }
                var attvalue = (ushort)Math.Min(PickAttribut.Item2 + Math.Min(Pool.GetRandom.Next(0, 10), (int)GetMinxAttributte((uint)PickAttribut.Item1)), GetMaxAttributte((uint)PickAttribut.Item1));
                GenerateAttributes.Add(GetAttributID(attvalue, PickAttribut.Item1));




            }
            return GenerateAttributes;
        }
        private static bool CheckUpAttributes(List<Tuple<Game.MsgServer.MsgChiInfo.ChiAttribute, uint>> attributes, Tuple<Game.MsgServer.MsgChiInfo.ChiAttribute, uint> attribut)
        {
            Tuple<Game.MsgServer.MsgChiInfo.ChiAttribute, uint> ToRemove = null;
            Tuple<Game.MsgServer.MsgChiInfo.ChiAttribute, uint> ToAdd = null;
            bool Found = false;
            foreach (var oldattr in attributes)
            {
                if (oldattr.Item1 == attribut.Item1)
                {
                    uint min = Math.Min(oldattr.Item2, attribut.Item2);
                    uint max = Math.Max(oldattr.Item2, attribut.Item2);
                    ToAdd = new Tuple<Game.MsgServer.MsgChiInfo.ChiAttribute, uint>(oldattr.Item1, (uint)Pool.GetRandom.Next((int)min, (int)max));
                    ToRemove = oldattr;
                    Found = true;
                    break;
                }
            }
            if (Found)
            {
                attributes.Remove(ToRemove);
                attributes.Add(ToAdd);
            }
            return Found;
        }
        public static unsafe ServerSockets.Packet CreateXuanBaoOpt(this ServerSockets.Packet stream, XuanBaoOpt obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgXuanBaoOpt);

            return stream;
        }
        public static unsafe void GetRelicFuse(this ServerSockets.Packet stream, out XuanBaoOpt pQuery)
        {
            pQuery = new XuanBaoOpt();
            pQuery = stream.ProtoBufferDeserialize<XuanBaoOpt>(pQuery);
        }
        public static void Loading(Client.GameClient user, ServerSockets.Packet stream)
        {
            if (user.MainRelic.Resonate >= 300)
            {
                user.MainRelic.Unlocked = true;
            }

            var packet = new MsgXuanBaoOpt.XuanBaoOpt
            {
                Times = (uint)System.Math.Max(0, user.MainRelic.Point), // [ProtoFix 2026-10-14] clamp negative
                Unlocked = user.MainRelic.Unlocked,
                FuseRelic = user.MainRelic.Resonate
            };

            user.Send(stream.CreateXuanBaoOpt(packet));
        }
        public static void xuanbaoadditionattr(MsgGameItem relic)
        {
            for (int x = 0; x < relic.RelicAttributes.Length; x++)
            {
                int Count = relic.RelicAttributes.Count(p => p.Type == relic.RelicAttributes[x].Type);
                var xuanbaoadditionattr = Database.RuneTable.xuanbaoadditionattr.FirstOrDefault(i => i.Type == (byte)relic.RelicAttributes[x].Type && i.Num == Count);
                if (xuanbaoadditionattr != null)
                {
                    relic.RelicAttributes[x].Value += (ushort)xuanbaoadditionattr.Value;
                }
                if (relic.RelicAttributes[x].Type == MsgChiInfo.ChiAttribute.LuckyStrike)
                {
                    relic.RelicAttributes[x].Value = 100;
                    relic.RelicAttributes[x].Epic = true;
                }
            }
        }
        private static void SyncResonanceSlots(Client.GameClient user, ServerSockets.Packet stream)
        {
            // نفس المسار اللي بيشتغل عند الدخول بالظبط: بيبعت العناصر، وحقيبة الريليك
            // (SyncMinorRelicBag) اللي العميل محتاجها عشان يعرض الخانات، وبيحدّث المعدات.
            // من غيرها كانت العناصر بتتبعت والخانات تفضل فاضية في الواجهة.
            user.MainRelic.Apply(user, stream);
        }
        private static void RefreshResonance(Client.GameClient user, ServerSockets.Packet stream)
        {
            // لازم نعيد إرسال حقيبة الريليك كمان، وإلا الريليك بيتحفظ في الخانة
            // بس الواجهة تفضل فاضية لحد ما اللاعب يخرج ويدخل تاني.
            user.MainRelic.Apply(user, stream);
            user.Equipment.QueryEquipment(user.Equipment.Alternante);
            // Send the original equipped relic again so the client refreshes its Resonance view.
            foreach (var main in user.Equipment.ClientItems.Values.Where(i =>
                i.Position == (ushort)Role.Flags.ConquerItem.Relics ||
                i.Position == (ushort)Role.Flags.ConquerItem.AleternanteRelics))
            {
                main.Mode = Role.Flags.ItemMode.Update;
                main.Send(user, stream);
            }
        }
        private static RelicTransformCommand ResolveTransformCommand(XuanBaoOpt query)
        {
            if (query.ActionID == 11) return RelicTransformCommand.Attempt;
            if (query.ActionID == 12) return RelicTransformCommand.Revert;
            return query.MainRelic == 0 ? RelicTransformCommand.Open : RelicTransformCommand.Select;
        }

        private static void SendTransformResponse(Client.GameClient user, ServerSockets.Packet stream,
            uint action, uint uid, RelicTransformCommand command, bool success, int slot)
        {
            user.Send(stream.CreateXuanBaoOpt(new XuanBaoOpt
            {
                ActionID = action,
                MainRelic = uid,
                Success = success ? 1u : 0u,
                // Native client field 10 releases the outstanding request gate.
                Test10 = success && command == RelicTransformCommand.Attempt ? 1u : 2u
            }));
        }

        private static void SaveTransform(Client.GameClient user)
        {
            Database.ServerDatabase.SaveClientItems(user);
            Database.ServerDatabase.SaveRelicTransformState(user);
            var writer = new WindowsAPI.IniFile("\\Users\\" + user.Player.UID + ".ini");
            writer.Write<ulong>("Character", "Money", user.Player.Money);
        }

        private static void ProcessTransform(Client.GameClient user, ServerSockets.Packet stream, XuanBaoOpt query)
        {
            var command = ResolveTransformCommand(query);
            if (command == RelicTransformCommand.Open || (query.ActionID == 11 && query.MainRelic == 0))
            {
                user.MainRelic.ClearPendingTransform();
                Database.ServerDatabase.SaveRelicTransformState(user);
                user.Send(stream.CreateXuanBaoOpt(new XuanBaoOpt { ActionID = query.ActionID }));
                return;
            }

            MsgGameItem relic;
            if (!user.Inventory.TryGetItem(query.MainRelic, out relic) || relic.Position != 0 ||
                !RelicTransformService.IsRelic(relic))
            {
                if (command == RelicTransformCommand.Attempt)
                    user.SendSysMesage("This relic must be in your inventory to Transform.");
                SendTransformResponse(user, stream, query.ActionID, query.MainRelic, command, false, -1);
                return;
            }
            if (command == RelicTransformCommand.Select)
            {
                user.Send(stream.CreateXuanBaoOpt(new XuanBaoOpt { ActionID = 13, MainRelic = relic.UID }));
                return;
            }
            if (command == RelicTransformCommand.Revert)
            {
                if (user.MainRelic.RevertPendingTransform(relic))
                {
                    relic.Mode = Role.Flags.ItemMode.Update;
                    relic.Send(user, stream);
                    SaveTransform(user);
                }
                SendTransformResponse(user, stream, query.ActionID, relic.UID, command, false, -1);
                return;
            }

            // Confirm is local in the native client. A new attempt accepts the last result.
            user.MainRelic.ConfirmPendingTransform();
            Database.ServerDatabase.SaveRelicTransformState(user);
            var slots = RelicTransformService.GetTransformableSlots(relic);
            if (slots.Count == 0 || relic.RelicAttributes.Length != 5 ||
                (relic.GetPrestigeScore < RelicTransformConfig.MinimumPrestigeScore &&
                 !RelicTransformService.HasTransformedAttribute(relic)))
            {
                user.SendSysMesage("This relic has no attribute that can be Transformed.");
                SendTransformResponse(user, stream, query.ActionID, relic.UID, command, false, -1);
                return;
            }
            uint[] ids = { RelicTransformConfig.RefinedKunlunJade, RelicTransformConfig.PrismStone, RelicTransformConfig.RelicCrystal };
            uint[] counts = { 1, 10, 15 };
            var costs = new List<Tuple<MsgGameItem, ushort>>();
            for (int i = 0; i < ids.Length; i++)
            {
                uint remaining = counts[i];
                foreach (var item in user.Inventory.ClientItems.Values.Where(p => p.ITEM_ID == ids[i] && p.Position == 0).OrderBy(p => p.UID))
                {
                    ushort take = (ushort)Math.Min(remaining, (uint)Math.Max(1, (int)item.StackSize));
                    costs.Add(Tuple.Create(item, take));
                    remaining -= take;
                    if (remaining == 0) break;
                }
                if (remaining != 0)
                {
                    user.SendSysMesage("Not enough Transform materials.");
                    SendTransformResponse(user, stream, query.ActionID, relic.UID, command, false, -1);
                    return;
                }
            }
            if (user.Player.Money < RelicTransformConfig.SilverCost)
            {
                user.SendSysMesage("Not enough silver to Transform.");
                SendTransformResponse(user, stream, query.ActionID, relic.UID, command, false, -1);
                return;
            }
            foreach (var cost in costs) user.Inventory.RemoveStackItem(cost.Item1.UID, cost.Item2, stream);
            user.Player.Money -= RelicTransformConfig.SilverCost;
            user.Player.SendUpdate(stream, user.Player.Money, MsgUpdate.DataType.Money);
            int slot = slots[0];
            bool success = Pool.GetRandom.Next(0, 1000000) < RelicTransformConfig.GetSuccessRate(false) * 10000;
            if (success)
            {
                user.MainRelic.BeginPendingTransform(relic, (byte)slot);
                success = RelicTransformService.ApplyTransformation(relic, slot);
            }
            if (!success) user.MainRelic.ClearPendingTransform();
            SaveTransform(user);
            relic.Mode = Role.Flags.ItemMode.Update;
            relic.Send(user, stream);
            SendTransformResponse(user, stream, query.ActionID, relic.UID, command, success, slot);
        }
        [PacketAttribute(GamePackets.MsgXuanBaoOpt)]
        private unsafe static void Process(Client.GameClient user, ServerSockets.Packet stream)
        {

            XuanBaoOpt pQuery;
            stream.GetRelicFuse(out pQuery);

            if (user.MainRelic == null)
            {
                // من غير الحماية دي الـ lock تحت كان بيرمي استثناء والعميل ميوصلوش أي رد.
                user.MainRelic = new Qahr.Mobile.MY_System.Relic.MainRelic(user);
            }

            try
            {
                lock (Database.ServerDatabase.SavingObj)
                    lock (user.MainRelic)
                        switch (pQuery.ActionID)
                        {
                            case (uint)ActionIds.TransformAttempt:
                            case (uint)ActionIds.TransformRestore:
                            case (uint)ActionIds.Transform:
                                ProcessTransform(user, stream, pQuery);
                                break;
                            case (uint)ActionIds.RemoveReconace://Remove
                                {
                                    var new_relic = user.MinorRelics.Values.FirstOrDefault(p => p.EonspiritActived == pQuery.BigRateRelic);
                                    if (new_relic != null)
                                    {
                                        if (!user.Inventory.HaveSpace(1))
                                        {
                                            user.SendSysMesage("Please make one space in your inventory before removing Resonance.");
                                            break;
                                        }
                                        MsgGameItem removed;
                                        user.MinorRelics.TryRemove(new_relic.UID, out removed);
                                        user.MinorRilec.Remove(new_relic.UID);
                                        user.Equipment.ClientItems.TryRemove(new_relic.UID, out removed);
                                        new_relic.Position = 0;
                                        new_relic.WH_ID = 0;
                                        new_relic.EonspiritActived = 0;
                                        user.Inventory.ClientItems[new_relic.UID] = new_relic;
                                        new_relic.Mode = Role.Flags.ItemMode.AddItem;
                                        new_relic.Send(user, stream);
                                        pQuery.MainRelic = new_relic.UID;
                                        RefreshResonance(user, stream);
                                        user.Send(stream.CreateXuanBaoOpt(pQuery));
                                        Database.ServerDatabase.SaveClient(user);
                                    }
                                    break;
                                }
                            case (uint)ActionIds.View1://Relic Minor 1 View
                                {

                                    pQuery.Times = 7;
                                    pQuery.BigRateRelic = 60;
                                    pQuery.Unlocked = user.MainRelic.Unlocked;

                                    user.Send(stream.CreateXuanBaoOpt(pQuery));
                                    // The client drops its Resonance slot items when switching tabs;
                                    // resend whatever relics are actually placed so they don't appear empty/blank.
                                    SyncResonanceSlots(user, stream);
                                    break;
                                }
                            case (uint)ActionIds.View2://Relic Minor 2 View
                                {
                                    pQuery.FuseRelic = 300;

                                    pQuery.BigRateRelic = 300;
                                    pQuery.Unlocked = user.MainRelic.Unlocked;

                                    user.Send(stream.CreateXuanBaoOpt(pQuery));
                                    SyncResonanceSlots(user, stream);
                                    break;
                                }

                            case (uint)ActionIds.UnLock://unlock >> Done
                                {
                                    if (pQuery.Items == null || pQuery.Items.Count == 0)
                                        break;
                                    var unlockItems = new List<Tuple<MsgGameItem, ushort, uint>>();
                                    var seen = new HashSet<uint>();
                                    bool valid = true;
                                    foreach (var entry in pQuery.Items)
                                    {
                                        MsgGameItem stone;
                                        if (!seen.Add(entry.Id) || entry.Size == 0 || entry.Size > ushort.MaxValue ||
                                            !user.Inventory.TryGetItem(entry.Id, out stone) || stone.Position != 0 ||
                                            (stone.ITEM_ID != 723694 && stone.ITEM_ID != 723695) ||
                                            entry.Size > Math.Max(1, (int)stone.StackSize))
                                        {
                                            valid = false;
                                            break;
                                        }
                                        unlockItems.Add(Tuple.Create(stone, (ushort)entry.Size, stone.ITEM_ID == 723694 ? 10u : 100u));
                                    }
                                    if (!valid) break;
                                    foreach (var entry in unlockItems)
                                    {
                                        user.Inventory.RemoveStackItem(entry.Item1.UID, entry.Item2, stream);
                                        user.MainRelic.Resonate = (uint)Math.Min(uint.MaxValue,
                                            (ulong)user.MainRelic.Resonate + (ulong)entry.Item2 * entry.Item3);
                                    }
                                    if (user.MainRelic.Resonate >= 300) user.MainRelic.Unlocked = true;
                                    pQuery.FuseRelic = user.MainRelic.Resonate;
                                    pQuery.BigRateRelic = 2;
                                    pQuery.Times = (uint)System.Math.Max(0, user.MainRelic.Point); // [ProtoFix 2026-10-14] clamp negative
                                    pQuery.Success = user.MainRelic.Rate;
                                    pQuery.Unlocked = user.MainRelic.Unlocked;
                                    user.Send(stream.CreateXuanBaoOpt(pQuery));
                                    Database.ServerDatabase.SaveClient(user);
                                    break;
                                }
                            case (uint)ActionIds.Synergy://Synergy
                                {

                                    uint slot = pQuery.BigRateRelic;
                                    if (slot < 1 || slot > 4)
                                    {
                                        // Every Synergy request must get a reply: the native client
                                        // keeps the Resonate button gated until it sees this packet.
                                        pQuery.Success = 0;
                                        pQuery.Test10 = 2;
                                        user.Send(stream.CreateXuanBaoOpt(pQuery));
                                        break;
                                    }
                                    // كل شرط بيتفحص لوحده وبيبعت للاعب رسالة تقوله السبب بالظبط،
                                    // بدل شرط واحد كبير مابيقولش إيه اللي فشل.
                                    MsgGameItem relic;
                                    if (!user.Inventory.TryGetItem(pQuery.FuseRelic, out relic))
                                    {
                                        user.SendSysMesage("The relic must be in your inventory, not equipped.");
                                        pQuery.Success = 0;
                                        pQuery.Test10 = 2;
                                        user.Send(stream.CreateXuanBaoOpt(pQuery));
                                        break;
                                    }
                                    if (!RelicTransformService.IsRelic(relic))
                                    {
                                        user.SendSysMesage("This item is not recognized as a relic.");
                                        pQuery.Success = 0;
                                        pQuery.Test10 = 2;
                                        user.Send(stream.CreateXuanBaoOpt(pQuery));
                                        break;
                                    }
                                    if (relic.Position != 0)
                                    {
                                        user.SendSysMesage("The relic must be in your inventory, not equipped.");
                                        pQuery.Success = 0;
                                        pQuery.Test10 = 2;
                                        user.Send(stream.CreateXuanBaoOpt(pQuery));
                                        break;
                                    }
                                    if (user.MinorRelics.ContainsKey(relic.UID))
                                    {
                                        user.SendSysMesage("This relic is already in a Resonance slot.");
                                        pQuery.Success = 0;
                                        pQuery.Test10 = 2;
                                        user.Send(stream.CreateXuanBaoOpt(pQuery));
                                        break;
                                    }
                                    if (user.MinorRelics.Values.Any(i => i.EonspiritActived == slot))
                                    {
                                        user.SendSysMesage("Remove the relic already in this Resonance slot first.");
                                        pQuery.Success = 0;
                                        pQuery.Test10 = 2;
                                        user.Send(stream.CreateXuanBaoOpt(pQuery));
                                        break;
                                    }
                                    uint materialCount = slot == 1 ? 10u : 20u;
                                    uint moneyCost = slot == 1 ? 200000u : 400000u;
                                    var materials = user.Inventory.ClientItems.Values
                                        .Where(i => i.ITEM_ID == 3353634 && i.Position == 0).OrderBy(i => i.UID).ToArray();
                                    long haveMaterials = materials.Sum(i => (long)Math.Max(1, (int)i.StackSize));
                                    if (user.Player.Money < moneyCost)
                                    {
                                        user.SendSysMesage("You need " + moneyCost + " silver for this Resonance slot.");
                                        pQuery.Success = 0;
                                        pQuery.Test10 = 2;
                                        user.Send(stream.CreateXuanBaoOpt(pQuery));
                                        break;
                                    }
                                    if (haveMaterials < materialCount)
                                    {
                                        user.SendSysMesage("You need " + materialCount + " Prism Stones (you have " + haveMaterials + ").");
                                        pQuery.Success = 0;
                                        pQuery.Test10 = 2;
                                        user.Send(stream.CreateXuanBaoOpt(pQuery));
                                        break;
                                    }
                                    // Charge the server-defined cost once. Never delete arbitrary packet item UIDs.
                                    uint remaining = materialCount;
                                    foreach (var material in materials)
                                    {
                                        ushort take = (ushort)Math.Min(remaining, (uint)Math.Max(1, (int)material.StackSize));
                                        user.Inventory.RemoveStackItem(material.UID, take, stream);
                                        remaining -= take;
                                        if (remaining == 0) break;
                                    }
                                    user.Player.Money -= moneyCost;
                                    user.Player.SendUpdate(stream, user.Player.Money, MsgUpdate.DataType.Money);
                                    if (Role.Core.Rate(10))
                                    {
                                        MsgGameItem removed;

                                        user.Inventory.ClientItems.TryRemove(relic.UID, out removed);
                                        user.Equipment.ClientItems.TryRemove(relic.UID, out removed);

                                        user.Send(stream.ItemUsageCreate(
                                            MsgItemUsuagePacket.ItemUsuageID.RemoveInventory,
                                            relic.UID, 0, 0, 0, 0, 0));

                                        relic.Position = Qahr.Mobile.MY_System.Relic.MainRelic.ResonancePosition;
                                        relic.WH_ID = 0;
                                        relic.EonspiritActived = slot;

                                        user.AddMinorRelic(relic);

                                        relic.Mode = Role.Flags.ItemMode.AddItem;
                                        relic.Send(user, stream);

                                        RefreshResonance(user, stream);
                                        user.Equipment.QueryEquipment(user.Equipment.Alternante);

                                        // SUCCESS
                                        pQuery.Success = 1;

                                        // لا تبعت Test10 = 1 هنا إذا السورس القديم هو المرجع
                                        // pQuery.Test10 = 1;
                                    }
                                    else
                                    {
                                        user.MainRelic.Point = Math.Max(0, user.MainRelic.Point - 1);

                                        pQuery.Times = (uint)System.Math.Max(0, user.MainRelic.Point); // [ProtoFix 2026-10-14] clamp negative

                                        // FAILURE
                                        pQuery.Test10 = 2;
                                        pQuery.Success = 0;                                        
                                    }

                                    user.Send(stream.CreateXuanBaoOpt(pQuery));
                                    Database.ServerDatabase.SaveClient(user);
                                    break;
                                }
                            //case 4://buy Relic
                            //    {
                            //        return;
                            //        var amount = Math.Min(pQuery.FuseRelic, 100);
                            //        if (pQuery.MainRelic == 1)//Advanced
                            //        {
                            //            for (int i = 0; i < amount; i++)
                            //            {
                            //                if (user.Player.ConquerPoints >= 9999)
                            //                {
                            //                    user.Player.ConquerPoints -= 9999;
                            //                    user.Inventory.AddRandomRelicL5(stream, bound: false);
                            //                }
                            //            }
                            //        }
                            //        else
                            //        {
                            //            for (int i = 0; i < amount; i++)
                            //            {
                            //                if (user.Player.ConquerPoints >= 1000)
                            //                {
                            //                    user.Player.ConquerPoints -= 1000;
                            //                    user.Inventory.AddRandomRelicL1(stream, false);
                            //                }
                            //            }
                            //        }
                            //        break;
                            //    }
                            case (uint)ActionIds.FuseRelic:
                                {
                                    MsgGameItem MainRelic;
                                    if (user.Inventory.TryGetItem(pQuery.MainRelic, out MainRelic))
                                    {
                                        MsgGameItem FuseRelic;
                                        if (user.Inventory.TryGetItem(pQuery.FuseRelic, out FuseRelic))
                                        {
                                            if (Database.ItemType.ItemPosition(MainRelic.ITEM_ID) != (ushort)Role.Flags.ConquerItem.Relic
                                               || Database.ItemType.ItemPosition(FuseRelic.ITEM_ID) != (ushort)Role.Flags.ConquerItem.Relic)
                                            {
                                                return;
                                            }
                                            uint Cost = GetXuanBaoFuseCost(Math.Max((uint)MainRelic.RelicAttributes.Count(), (uint)FuseRelic.RelicAttributes.Count()) - 1, 0, 100000);
                                            if (user.Player.Money >= Cost)
                                            {
                                                user.Player.Money -= Cost;
                                                user.Player.SendUpdate(stream, user.Player.Money, MsgUpdate.DataType.Money);


                                                user.Warehouse.Clear((uint)Game.MsgNpc.NpcID.RelicFuse);

                                                var Attributes = XRelicFuse(MainRelic, FuseRelic);
                                                var new_relic = new Game.MsgServer.MsgGameItem() { UID = pQuery.BigRateRelic = Pool.ITEM_Counter.Next, StackSize = 1, Position = (ushort)Role.Flags.ConquerItem.RelicFuse3, ITEM_ID = MainRelic.ITEM_ID, RelicAttributes = new RelicAttribute[5], Mode = Role.Flags.ItemMode.AddItem };
                                                new_relic.Durability = new_relic.MaximDurability = (ushort)(Pool.GetRandom.Next(30, 101) * 100);
                                                if (Attributes.Count > 0)
                                                {
                                                    for (int O = 0; O < Attributes.Count; O++)
                                                    {
                                                        new_relic.RelicAttributes[O] = new RelicAttribute(Attributes[O]);
                                                        var ComposeLimitDB = Database.RuneTable.composelimit.Where(i => i.Attribute == new_relic.RelicAttributes[O].Type).ToArray().FirstOrDefault();
                                                        if (new_relic.RelicAttributes[O].Value >= ComposeLimitDB.Max)
                                                        {
                                                            new_relic.RelicAttributes[O].Value = (ushort)ComposeLimitDB.Max;
                                                            new_relic.RelicAttributes[O].Epic = true;
                                                        }
                                                    }
                                                    xuanbaoadditionattr(new_relic);

                                                }

                                                MainRelic.Position = (ushort)Role.Flags.ConquerItem.RelicFuse1;
                                                FuseRelic.Position = (ushort)Role.Flags.ConquerItem.RelicFuse2;
                                                user.Warehouse.AddItem(new_relic, (uint)Game.MsgNpc.NpcID.RelicFuse);
                                                user.Warehouse.AddItem(MainRelic, (uint)Game.MsgNpc.NpcID.RelicFuse);
                                                user.Warehouse.AddItem(FuseRelic, (uint)Game.MsgNpc.NpcID.RelicFuse);
                                                new_relic.Send(user, stream);

                                                MsgGameItem BestRelic = new_relic;
                                                pQuery.BigRateRelic = BestRelic.UID;
                                                user.Send(stream.CreateXuanBaoOpt(pQuery));

                                                user.Inventory.Update(MainRelic, Role.Instance.AddMode.REMOVE, stream);
                                                user.Inventory.Update(FuseRelic, Role.Instance.AddMode.REMOVE, stream);
                                            }
                                            else
                                            {
                                                user.SendSysMesage("You don`t have enough Money to FuseRelics");
                                            }
                                        }
                                    }
                                    break;
                                }
                            case (uint)ActionIds.SelectRelic:
                                {
                                    MsgGameItem item;
                                    if (user.Warehouse.TryGetItem((uint)Game.MsgNpc.NpcID.RelicFuse, pQuery.MainRelic, out item))
                                    {
                                        if (!user.Inventory.HaveSpace(1))
                                        {
                                            user.SendSysMesage("Please make one more space in your inventory.");
                                            break;
                                        }

                                        item.WH_ID = 0;
                                        user.Inventory.Update(item, Role.Instance.AddMode.ADD, stream);
                                        user.Warehouse.Clear((uint)Game.MsgNpc.NpcID.RelicFuse);
                                        user.Send(stream.CreateXuanBaoOpt(pQuery));

                                    }
                                    break;
                                }
                            default:
                                {
                                    MyConsole.WriteLine("[Relic] Unhandled Action >> " + pQuery.ActionID, ConsoleColor.DarkRed);
                                    // لازم نرد حتى على الأكشن غير المعروف، وإلا الواجهة تفضل مستنية ومش بترد.
                                    pQuery.Success = 0;
                                    pQuery.Test10 = 2;
                                    user.Send(stream.CreateXuanBaoOpt(pQuery));
                                    break;
                                }
                        }
            }
            catch (Exception e)
            {
                MyConsole.WriteException(e);
                // أي استثناء كان بيخلي الزرار يعلق من غير رد. نبعت رد فشل عشان الواجهة تفضل شغالة.
                try
                {
                    pQuery.Success = 0;
                    pQuery.Test10 = 2;
                    user.Send(stream.CreateXuanBaoOpt(pQuery));
                }
                catch { }
            }
        }
    }
}
