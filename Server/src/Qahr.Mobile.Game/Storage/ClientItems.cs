using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.IO;

namespace Qahr.Mobile.Database
{
    public class ClientItems
    {
        public struct Perfection
        {
            public uint ItemUID;
            public uint Level;
            public uint Progres;
            public uint OwnerUID;
            public unsafe fixed sbyte _OwnerName[32];
            public unsafe fixed sbyte _SpecialText[32];

            public unsafe string OwnerName
            {
                get { fixed (sbyte* bp = _OwnerName) { return new string(bp); } }
                set
                {
                    string text = value;
                    fixed (sbyte* bp = _OwnerName)
                    {
                        for (int i = 0; i < text.Length; i++)
                            bp[i] = (sbyte)text[i];
                    }
                }
            }
            public unsafe string SpecialText
            {
                get { fixed (sbyte* bp = _SpecialText) { return new string(bp); } }
                set
                {
                    string text = value;
                    fixed (sbyte* bp = _SpecialText)
                    {
                        for (int i = 0; i < text.Length; i++)
                            bp[i] = (sbyte)text[i];
                    }
                }
            }
        }
        public struct DBItem
        {
            public uint UID;
            public uint ITEM_ID;
            public ushort Durability;
            public ushort MaximDurability;
            public ushort Position;
            public uint SocketProgress;
            public Role.Flags.Gem SocketOne;
            public Role.Flags.Gem SocketTwo;
            public Role.Flags.ItemEffect Effect;
            public byte Plus;
            public byte Bless;
            public byte Bound;
            public byte Enchant;
            public byte Suspicious;
            public byte Locked;

            public uint PlusProgress;
            public ushort Inscribed;
            public uint Activate;
            public uint TimeLeftInMinutes;
            public ushort StackSize;
            public uint WH_ID;
            public Role.Flags.Color Color;
            public int UnLockTimer;

            public uint ItemPoints;
            public uint RemainingTime;
            public uint UpsurgeID;
            public uint test4;

            //Artefacts 
            public uint PurificationItemID;
            public uint PurificationLevel;
            public uint PurificationDuration;
            public long PurificationAddedOn;

            //Refinary
            public uint EffectID;
            public uint EffectLevel;
            public uint EffectPercent;
            public uint EffectPercent2;
            public uint EffectDuration;
            public long EffectAddedOn;

            public uint DepositeCount;

            public uint AnimaItemID;
            public uint MythSoulID;
            public uint MythSoulProgress;
            public uint Mutacion;
            public uint RuneEXP;
            public uint Attribute1;
            public uint Attribute2;
            public uint Attribute3;
            public uint Attribute4;
            public uint Attribute5;
            public uint YuanshenIndex;
            public ulong YuanshenTime;
            public long Timer;
            public uint RelicResonance;
            public uint Mutacion2;
            public Perfection GetPerfectionInfo(Game.MsgServer.MsgGameItem DataItem)
            {
                Perfection info = new Perfection();
                info.ItemUID = DataItem.UID;
                info.Level = DataItem.PerfectionLevel;
                info.Progres = DataItem.PerfectionProgress;
                info.OwnerUID = DataItem.OwnerUID;
                info.OwnerName = DataItem.OwnerName;
                info.SpecialText = DataItem.Signature;
                return info;
            }
            public DBItem GetDBItem(Game.MsgServer.MsgGameItem DataItem)
            {
                this.UID = DataItem.UID;
                this.ITEM_ID = DataItem.ITEM_ID;
                this.Durability = DataItem.Durability;
                this.MaximDurability = DataItem.MaximDurability;
                this.Position = DataItem.Position;
                this.SocketProgress = DataItem.SocketProgress;
                this.SocketOne = DataItem.SocketOne;
                this.SocketTwo = DataItem.SocketTwo;
                this.Effect = DataItem.Effect;
                this.Plus = DataItem.Plus;
                this.Bless = DataItem.Bless;
                this.Bound = DataItem.Bound;
                this.Enchant = DataItem.Enchant;
                this.Suspicious = DataItem.Suspicious;
                this.Locked = DataItem.Locked;
                this.PlusProgress = DataItem.PlusProgress;
                this.Activate = DataItem.Activate;
                this.StackSize = (ushort)DataItem.StackSize;
                this.WH_ID = DataItem.WH_ID;
                this.Color = DataItem.Color;
                this.ItemPoints = DataItem.ItemPoints;
                this.PurificationItemID = DataItem.Purification.PurificationItemID;
                this.PurificationAddedOn = DataItem.Purification.AddedOn.Ticks;
                this.PurificationDuration = DataItem.Purification.PurificationDuration;
                this.PurificationLevel = DataItem.Purification.PurificationLevel;
                this.EffectAddedOn = DataItem.Refinary.AddedOn.Ticks;
                this.EffectDuration = DataItem.Refinary.EffectDuration;
                this.EffectID = DataItem.Refinary.EffectID;
                this.EffectLevel = DataItem.Refinary.EffectLevel;
                this.EffectPercent = DataItem.Refinary.EffectPercent;
                this.EffectPercent2 = DataItem.Refinary.EffectPercent2;
                this.DepositeCount = (uint)DataItem.Deposite.Count;
                this.UnLockTimer = DataItem.UnLockTimer;
                this.RemainingTime = DataItem.RemainingTime;
                this.UpsurgeID = DataItem.UpsurgeID;
                this.AnimaItemID = DataItem.AnimaItemID;
                this.MythSoulID = DataItem.MythSoulID;
                this.MythSoulProgress = DataItem.MythSoulProgress;
                this.Mutacion = DataItem.Mutacion;
                this.RuneEXP = DataItem.RuneEXP;
                this.Attribute1 = (uint)DataItem.RelicAttributes[0];
                this.Attribute2 = (uint)DataItem.RelicAttributes[1];
                this.Attribute3 = (uint)DataItem.RelicAttributes[2];
                this.Attribute4 = (uint)DataItem.RelicAttributes[3];
                this.Attribute5 = (uint)DataItem.RelicAttributes[4];
                Timer = DataItem.TimeStamp.Ticks;
                Mutacion2 = DataItem.Mutacion2;
                YuanshenIndex = DataItem.EonspiritActived;
                YuanshenTime = DataItem.YuanshenTime;
                return this;
            }

            public Game.MsgServer.MsgGameItem GetDataItem()
            {
                Game.MsgServer.MsgGameItem DataItem = new Game.MsgServer.MsgGameItem();
                DataItem.UID = this.UID;
                DataItem.Effect = this.Effect;
                DataItem.ITEM_ID = this.ITEM_ID;
                DataItem.Durability = this.Durability;
                DataItem.MaximDurability = this.MaximDurability;
                DataItem.Position = this.Position;
                DataItem.SocketProgress = this.SocketProgress;
                DataItem.SocketOne = this.SocketOne;
                DataItem.SocketTwo = this.SocketTwo;
                DataItem.Plus = this.Plus;
                DataItem.Bless = this.Bless;
                DataItem.Bound = this.Bound;
                DataItem.Enchant = this.Enchant;
                DataItem.Suspicious = this.Suspicious;
                DataItem.Locked = this.Locked;
                DataItem.PlusProgress = this.PlusProgress;
                DataItem.Activate = this.Activate;
                DataItem.TimeLeftInMinutes = this.TimeLeftInMinutes;
                DataItem.StackSize = this.StackSize;
                DataItem.ItemPoints = this.ItemPoints;
                ItemType.DBItem dbItem;
                if (Pool.ItemsBase.TryGetValue(this.ITEM_ID, out dbItem))
                    DataItem.RemainingTime = dbItem.StackSize > (ushort)1 ? 0U : uint.MaxValue;
                DataItem.UpsurgeID = this.UpsurgeID;
                DataItem.UnLockTimer = this.UnLockTimer;
                DataItem.WH_ID = this.WH_ID;
                DataItem.Color = this.Color;
                DataItem.Purification.ItemUID = this.UID;
                DataItem.Purification.PurificationItemID = this.PurificationItemID;
                DataItem.Purification.PurificationLevel = this.PurificationLevel;
                DataItem.Purification.PurificationDuration = this.PurificationDuration;

                DataItem.Purification.AddedOn = DateTime.FromBinary(PurificationAddedOn > DateTime.MinValue.Ticks && PurificationAddedOn < DateTime.MaxValue.Ticks ? PurificationAddedOn : 0);
                if (!DataItem.Purification.InLife)
                    DataItem.Purification = new Game.MsgServer.MsgItemExtra.Purification();

                DataItem.Refinary.ItemUID = UID;
                DataItem.Refinary.EffectID = EffectID;
                DataItem.Refinary.EffectLevel = EffectLevel;
                DataItem.Refinary.EffectPercent = EffectPercent;
                DataItem.Refinary.EffectPercent2 = EffectPercent2;
                DataItem.Refinary.EffectDuration = EffectDuration;
                DataItem.Refinary.AddedOn = DateTime.FromBinary(EffectAddedOn);
                if (!DataItem.Refinary.InLife)
                    DataItem.Refinary = new Game.MsgServer.MsgItemExtra.Refinery();


                DataItem.AnimaItemID = this.AnimaItemID;
                DataItem.MythSoulID = this.MythSoulID;
                DataItem.MythSoulProgress = this.MythSoulProgress;
            
                DataItem.RuneEXP = this.RuneEXP;
                DataItem.RelicAttributes = new Role.Instance.RelicAttribute[5];
                DataItem.RelicAttributes[0] = new Role.Instance.RelicAttribute(Attribute1);
                DataItem.RelicAttributes[1] = new Role.Instance.RelicAttribute(Attribute2);
                DataItem.RelicAttributes[2] = new Role.Instance.RelicAttribute(Attribute3);
                DataItem.RelicAttributes[3] = new Role.Instance.RelicAttribute(Attribute4);
                DataItem.RelicAttributes[4] = new Role.Instance.RelicAttribute(Attribute5);
                if (Timer > 0)
                {
                    DataItem.TimeStamp = DateTime.FromBinary(Timer);
                    if (DateTime.Now >= DataItem.TimeStamp)
                    {

                        DataItem.Delete = true;
                    }
                    else
                    {
                        TimeSpan timeSpan = DataItem.TimeStamp - DateTime.Now;
                        DataItem.TimeLeftInMinutes = (uint)timeSpan.TotalSeconds;
                    }
                }

                DataItem.Mutacion = this.Mutacion;
                DataItem.Mutacion2 = Mutacion2;

                DataItem.EonspiritActived = YuanshenIndex;
                DataItem.YuanshenTime = YuanshenTime;
                return DataItem;
            }

        }
    }
}