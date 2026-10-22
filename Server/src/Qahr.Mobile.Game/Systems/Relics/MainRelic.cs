using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qahr.Mobile.MY_System.Relic
{
    public unsafe class MainRelic
    {
        public uint Rate = 0;
        public int Point = 0;
        public uint Full = 0;
        public uint Resonate = 0;
        public bool Unlocked = true;

        // Relic Transform state.  These fields are persisted with the
        // Character section so a pending result is never silently reused.
        public int RelicTransformDay = 0;
        public byte RelicTransformBoosts = 0;
        public bool RelicTransformPending = false;
        public uint PendingItemUID = 0;
        public byte PendingSlot = 0;
        public uint PendingAttribute1 = 0;
        public uint PendingAttribute2 = 0;
        public uint PendingAttribute3 = 0;
        public uint PendingAttribute4 = 0;
        public uint PendingAttribute5 = 0;

        public Client.GameClient _user;
        public MainRelic(Client.GameClient user)
        {
            _user = user;
        }

        // الموضع اللي العميل بيتوقعه لخانات الـ Resonance.
        // كان بيتكتب 14 (MinorRilec) وده مكان معدات حقيقي واقع بين Relics=13
        // و RightWeaponAccessory=15، فالعميل كان بيفسره كمعدة وما بيرسمش الخانات.
        // الصح 224 (NewRelicFuse) وهو في نطاق الحاويات الافتراضية مع RelicFuse1=213.
        public static readonly ushort ResonancePosition = (ushort)Role.Flags.ConquerItem.NewRelicFuse;

        // بتقبل القيمة القديمة كمان عشان الريليكات المحفوظة بـ 14 ما تضيعش عند التحميل.
        public static bool IsResonancePosition(ushort position)
        {
            return position == (ushort)Role.Flags.ConquerItem.NewRelicFuse
                || position == (ushort)Role.Flags.ConquerItem.MinorRilec;
        }

        public void ResetDailyTransformBoostsIfNeeded()
        {
            DateTime now = DateTime.Now;
            int today = (now.Year * 10000) + (now.Month * 100) + now.Day;
            if (RelicTransformDay != today)
            {
                RelicTransformDay = today;
                RelicTransformBoosts = 0;
            }
        }

        public byte TransformBoostsRemaining
        {
            get
            {
                ResetDailyTransformBoostsIfNeeded();
                return RelicTransformBoosts >= Game.MsgServer.RelicTransformConfig.DailyBoostLimit
                    ? (byte)0
                    : (byte)(Game.MsgServer.RelicTransformConfig.DailyBoostLimit - RelicTransformBoosts);
            }
        }

        public bool CanUseTransformBoost()
        {
            return TransformBoostsRemaining > 0;
        }

        public void UseTransformBoost()
        {
            ResetDailyTransformBoostsIfNeeded();
            if (RelicTransformBoosts < Game.MsgServer.RelicTransformConfig.DailyBoostLimit)
                RelicTransformBoosts++;
        }

        public void BeginPendingTransform(Game.MsgServer.MsgGameItem relic, byte slot)
        {
            if (relic == null || relic.RelicAttributes == null || relic.RelicAttributes.Length < 5)
                return;

            RelicTransformPending = true;
            PendingItemUID = relic.UID;
            PendingSlot = slot;
            PendingAttribute1 = relic.RelicAttributes[0].Data;
            PendingAttribute2 = relic.RelicAttributes[1].Data;
            PendingAttribute3 = relic.RelicAttributes[2].Data;
            PendingAttribute4 = relic.RelicAttributes[3].Data;
            PendingAttribute5 = relic.RelicAttributes[4].Data;
        }

        public bool ConfirmPendingTransform()
        {
            if (!RelicTransformPending)
                return false;
            ClearPendingTransform();
            return true;
        }

        public bool RevertPendingTransform(Game.MsgServer.MsgGameItem relic)
        {
            if (!RelicTransformPending || relic == null || relic.UID != PendingItemUID
                || relic.RelicAttributes == null || relic.RelicAttributes.Length < 5)
                return false;

            relic.RelicAttributes[0].Data = PendingAttribute1;
            relic.RelicAttributes[1].Data = PendingAttribute2;
            relic.RelicAttributes[2].Data = PendingAttribute3;
            relic.RelicAttributes[3].Data = PendingAttribute4;
            relic.RelicAttributes[4].Data = PendingAttribute5;
            ClearPendingTransform();
            return true;
        }

        public void ClearPendingTransform()
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

        public Game.MsgServer.MsgGameItem[] GetResonanceItems()
        {
            var items = _user.MinorRelics.Values.OrderBy(i => i.UID).ToArray();
            var occupied = new HashSet<uint>(items.Where(i => i.EonspiritActived >= 1 && i.EonspiritActived <= 4)
                .Select(i => i.EonspiritActived));
            var seen = new HashSet<uint>();
            foreach (var item in items)
            {
                if (item.EonspiritActived >= 1 && item.EonspiritActived <= 4 && seen.Add(item.EonspiritActived))
                    continue;
                for (uint slot = 1; slot <= 4; slot++)
                    if (occupied.Add(slot))
                    {
                        item.EonspiritActived = slot;
                        seen.Add(slot);
                        break;
                    }
            }
            return items;
        }

        public void Apply(Client.GameClient user, ServerSockets.Packet stream)
        {
            foreach (var item in GetResonanceItems())
            {
                item.Position = ResonancePosition;
                item.Mode = Role.Flags.ItemMode.AddItem;

                user.AddMinorRelic(item);
                item.Send(user, stream);

            }
            user.SyncMinorRelicBag();
            user.Equipment.QueryEquipment(user.Equipment.Alternante);
        }

    }
}
