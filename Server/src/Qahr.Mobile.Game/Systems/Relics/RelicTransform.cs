using System;
using System.Collections.Generic;
using System.Linq;
using Qahr.Mobile.Role.Instance;

namespace Qahr.Mobile.Game.MsgServer
{
    /// <summary>
    /// Server-side settings for the Relic Transform feature.
    /// </summary>
    public static class RelicTransformConfig
    {
        public const uint RefinedKunlunJade = 3356818;
        public const uint PrismStone = 3353634;
        public const uint RelicCrystal = 3306885;
        public const uint SilverCost = 500000;

        // Minimum prestige required to use Relic Transform.
        public const uint MinimumPrestigeScore = 1;

        // Maximum number of boosts per day.
        public const byte DailyBoostLimit = 5;

        // Change these values according to your server settings.
        public const double BaseSuccessRate = 100.0d;
        public const double SuccessRateBoost = 0.0d;

        // 1st, 2nd, 3rd, 4th and 5th target attribute respectively.
        public static readonly byte[] TargetMultipliers =
            new byte[] { 25, 50, 75, 100, 100 };

        public static double GetSuccessRate(bool useBoost)
        {
            double rate = BaseSuccessRate;

            if (useBoost)
                rate += SuccessRateBoost;

            return Math.Min(100.0d, Math.Max(0.0d, rate));
        }
    }

    public enum RelicTransformCommand : uint
    {
        Open = 0,
        Select = 1,
        Attempt = 2,
        Confirm = 3,
        Revert = 4
    }

    /// <summary>
    /// Serializable shape for the character-level transform state.
    /// </summary>
    public sealed class RelicTransformState
    {
        public int RelicTransformDay;
        public byte RelicTransformBoosts;

        public bool RelicTransformPending;

        public uint PendingItemUID;
        public byte PendingSlot;

        public uint PendingAttribute1;
        public uint PendingAttribute2;
        public uint PendingAttribute3;
        public uint PendingAttribute4;
        public uint PendingAttribute5;

        /// <summary>
        /// Completely resets the pending transform state.
        /// This is important after a failed transformation so the client
        /// does not remain locked inside the Transform interface.
        /// </summary>
        public void ClearPending()
        {
            RelicTransformPending = false;

            PendingItemUID = 0;
            PendingSlot = 0;

            PendingAttribute1 = 0;
            PendingAttribute2 = 0;
            PendingAttribute3 = 0;
            PendingAttribute4 = 0;
            PendingAttribute5 = 0;
        }
    }

    public static class RelicTransformService
    {
        #region Attribute Mapping

        public static bool GetTargetAttribute(
            MsgChiInfo.ChiAttribute source,
            out MsgChiInfo.ChiAttribute target)
        {
            switch (source)
            {
                case MsgChiInfo.ChiAttribute.CriticalStrike:
                case MsgChiInfo.ChiAttribute.SkillCriticalStrike:

                    target = MsgChiInfo.ChiAttribute.DashRate;
                    return true;

                case MsgChiInfo.ChiAttribute.Immunity:

                    target = MsgChiInfo.ChiAttribute.ResistRate;
                    return true;

                case MsgChiInfo.ChiAttribute.LuckyStrike:

                    target = MsgChiInfo.ChiAttribute.StrikeLock;
                    return true;

                default:

                    target = MsgChiInfo.ChiAttribute.None;
                    return false;
            }
        }

        public static bool IsTransformable(
            MsgChiInfo.ChiAttribute attribute)
        {
            MsgChiInfo.ChiAttribute target;

            return GetTargetAttribute(attribute, out target);
        }

        public static bool IsTransformed(
            MsgChiInfo.ChiAttribute attribute)
        {
            return attribute == MsgChiInfo.ChiAttribute.DashRate
                || attribute == MsgChiInfo.ChiAttribute.ResistRate
                || attribute == MsgChiInfo.ChiAttribute.StrikeLock;
        }

        #endregion

        #region Values

        public static uint GetTargetValue(
            uint originalBaseValue,
            int targetCount)
        {
            if (targetCount < 1)
                targetCount = 1;

            if (targetCount > RelicTransformConfig.TargetMultipliers.Length)
                targetCount = RelicTransformConfig.TargetMultipliers.Length;

            ulong value =
                (ulong)originalBaseValue
                * RelicTransformConfig.TargetMultipliers[targetCount - 1];

            value /= 100UL;

            return value > uint.MaxValue
                ? uint.MaxValue
                : (uint)value;
        }

        public static uint GetDuplicateBonus(
            MsgChiInfo.ChiAttribute type,
            int count)
        {
            if (type == MsgChiInfo.ChiAttribute.None)
                return 0;

            if (count <= 0)
                return 0;

            if (Database.RuneTable.xuanbaoadditionattr == null)
                return 0;

            var row =
                Database.RuneTable.xuanbaoadditionattr.FirstOrDefault(
                    i => i.Type == (byte)type
                      && i.Num == count);

            if (row == null)
                return 0;

            if (row.Value <= 0)
                return 0;

            return (uint)row.Value;
        }

        public static uint GetBaseValue(
            RelicAttribute attribute,
            RelicAttribute[] attributes)
        {
            if (attribute.Type == MsgChiInfo.ChiAttribute.None)
                return 0;

            int count = attributes == null
                ? 1
                : attributes.Count(i => i.Type == attribute.Type);

            uint bonus =
                GetDuplicateBonus(attribute.Type, count);

            return attribute.Value >= bonus
                ? attribute.Value - bonus
                : 0;
        }

        #endregion

        #region Attribute Normalization

        private static uint[] CaptureBaseValues(
            RelicAttribute[] attributes,
            out bool[] epics)
        {
            uint[] baseValues =
                new uint[attributes.Length];

            epics =
                new bool[attributes.Length];

            for (int i = 0; i < attributes.Length; i++)
            {
                if (attributes[i].Type ==
                    MsgChiInfo.ChiAttribute.None)
                {
                    continue;
                }

                baseValues[i] =
                    GetBaseValue(
                        attributes[i],
                        attributes);

                epics[i] =
                    attributes[i].Epic;
            }

            return baseValues;
        }

        private static void ReapplyDuplicateBonuses(
            RelicAttribute[] attributes,
            uint[] baseValues,
            bool[] epics)
        {
            for (int i = 0; i < attributes.Length; i++)
            {
                if (attributes[i].Type ==
                    MsgChiInfo.ChiAttribute.None)
                {
                    attributes[i].Data = 0;
                    continue;
                }

                RelicAttribute normalized =
                    attributes[i];

                // LuckyStrike is always treated as 100 Epic.
                if (normalized.Type ==
                    MsgChiInfo.ChiAttribute.LuckyStrike)
                {
                    normalized.Value = 100;
                    normalized.Epic = true;
                }
                else
                {
                    int count =
                        attributes.Count(
                            p => p.Type == normalized.Type);

                    ulong value =
                        (ulong)baseValues[i]
                        + GetDuplicateBonus(
                            normalized.Type,
                            count);

                    normalized.Value =
                        value > uint.MaxValue
                            ? uint.MaxValue
                            : (uint)value;

                    normalized.Epic =
                        epics[i];
                }

                attributes[i] =
                    normalized;
            }
        }

        /// <summary>
        /// Removes existing duplicate bonuses and reapplies them once.
        /// </summary>
        public static void NormalizeAttributes(
            MsgGameItem relic)
        {
            if (relic == null)
                return;

            if (relic.RelicAttributes == null)
                return;

            if (relic.RelicAttributes.Length == 0)
                return;

            var attributes =
                relic.RelicAttributes;

            bool[] epics;

            uint[] baseValues =
                CaptureBaseValues(
                    attributes,
                    out epics);

            ReapplyDuplicateBonuses(
                attributes,
                baseValues,
                epics);
        }

        #endregion

        #region Transform Slots

        public static List<int> GetTransformableSlots(
            MsgGameItem relic)
        {
            var slots =
                new List<int>();

            if (relic == null)
                return slots;

            if (relic.RelicAttributes == null)
                return slots;

            for (int i = 0;
                i < relic.RelicAttributes.Length;
                i++)
            {
                var attribute =
                    relic.RelicAttributes[i];

                if (attribute.Value > 0
                    && IsTransformable(attribute.Type))
                {
                    slots.Add(i);
                }
            }

            return slots;
        }

        public static int CountTransformedAttributes(
            MsgGameItem relic)
        {
            if (relic == null)
                return 0;

            if (relic.RelicAttributes == null)
                return 0;

            return relic.RelicAttributes.Count(
                i => i.Value > 0
                  && IsTransformed(i.Type));
        }

        public static bool HasTransformedAttribute(
            MsgGameItem relic)
        {
            return CountTransformedAttributes(relic) > 0;
        }

        #endregion

        #region Transformation

        /// <summary>
        /// Applies the actual transformation to the selected attribute.
        ///
        /// This method only changes the relic.
        /// It does NOT manage packets or the UI state.
        /// </summary>
        public static bool ApplyTransformation(
            MsgGameItem relic,
            int slot)
        {
            if (relic == null)
                return false;

            if (relic.RelicAttributes == null)
                return false;

            if (slot < 0 ||
                slot >= relic.RelicAttributes.Length)
            {
                return false;
            }

            RelicAttribute source =
                relic.RelicAttributes[slot];

            MsgChiInfo.ChiAttribute target;

            if (source.Value == 0)
                return false;

            if (!GetTargetAttribute(
                source.Type,
                out target))
            {
                return false;
            }

            bool[] epics;

            uint[] baseValues =
                CaptureBaseValues(
                    relic.RelicAttributes,
                    out epics);

            uint originalBaseValue =
                baseValues[slot];

            int targetCount =
                relic.RelicAttributes.Count(
                    i => i.Type == target) + 1;

            RelicAttribute transformed =
                source;

            transformed.Type =
                target;

            baseValues[slot] =
                GetTargetValue(
                    originalBaseValue,
                    targetCount);

            epics[slot] =
                false;

            transformed.Value =
                baseValues[slot];

            transformed.Epic =
                false;

            relic.RelicAttributes[slot] =
                transformed;

            /*
             * Reapply duplicate bonuses using the original
             * base values. This prevents duplicate bonuses
             * from being compounded after repeated transforms.
             */
            ReapplyDuplicateBonuses(
                relic.RelicAttributes,
                baseValues,
                epics);

            return true;
        }

        #endregion

        #region Pending State Helpers

        /// <summary>
        /// Safely clears a pending transformation.
        ///
        /// Call this when:
        /// - transformation fails
        /// - player cancels
        /// - player reverts
        /// - item is removed
        /// - invalid request is detected
        /// </summary>
        public static void ClearPending(
            RelicTransformState state)
        {
            if (state == null)
                return;

            state.ClearPending();
        }

        /// <summary>
        /// Checks whether the pending state belongs to
        /// the specified relic.
        /// </summary>
        public static bool IsPendingForItem(
            RelicTransformState state,
            uint itemUID)
        {
            if (state == null)
                return false;

            if (!state.RelicTransformPending)
                return false;

            if (state.PendingItemUID == 0)
                return false;

            return state.PendingItemUID == itemUID;
        }

        /// <summary>
        /// Clears pending state if it belongs to the specified item.
        /// </summary>
        public static void ClearPendingForItem(
            RelicTransformState state,
            uint itemUID)
        {
            if (state == null)
                return;

            if (!state.RelicTransformPending)
                return;

            if (state.PendingItemUID != itemUID)
                return;

            state.ClearPending();
        }

        #endregion

        #region Relic Validation

        public static bool IsRelic(
            MsgGameItem item)
        {
            return item != null
                && Database.ItemType.ItemPosition(
                    item.ITEM_ID)
                    == (ushort)Role.Flags.ConquerItem.Relics;
        }

        #endregion
    }
}
