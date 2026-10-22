using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe void GetItemLock(this ServerSockets.Packet stream, out uint UID, out MsgItemLock.TypeLock Typ, out byte LockedMode, out uint dwPram)
        {
            UID = stream.ReadUInt32();
            Typ = (MsgItemLock.TypeLock)stream.ReadUInt8();
            LockedMode = stream.ReadUInt8();
            uint unKnow = stream.ReadUInt16();
            dwPram = stream.ReadUInt32();
        }

        public static unsafe ServerSockets.Packet ItemLockCreate(this ServerSockets.Packet stream, uint UID,  MsgItemLock.TypeLock Typ,  byte LockedMode,  uint dwPram)
        {
            stream.InitWriter();

            stream.Write(UID);
            stream.Write((byte)Typ);
            stream.Write(LockedMode);
            stream.Write((ushort)0);
            stream.Write(dwPram);

            stream.Finalize(GamePackets.MsgEquipLock);

            return stream;
        }

    }
    public unsafe struct MsgItemLock
    {
        public enum TypeLock : byte
        {
            RequestLock = 0, RequestUnlock = 1, UnlockDate = 2
        }

        [PacketAttribute(GamePackets.MsgEquipLock)]
        public unsafe static void MsgHandler(Client.GameClient user, ServerSockets.Packet packet)
        {

             uint UID;
          MsgItemLock.TypeLock Typ;
            byte LockedMode;
            uint dwPram;

            packet.GetItemLock(out UID, out Typ, out LockedMode, out dwPram);

            switch (Typ)
            {
                case TypeLock.RequestLock:
                    {
                        MsgGameItem GameItem;
                        if (user.TryGetItem(UID, out GameItem))
                        {
                            LockedMode = 1;
                            GameItem.Locked = 1;
                            GameItem.Mode = Role.Flags.ItemMode.Update;
                            GameItem.Send(user,packet);


                            user.Send(packet.ItemLockCreate(UID, Typ, LockedMode, dwPram));
                        }
                        break;
                    }
                case TypeLock.RequestUnlock:
                    {
                        MsgGameItem GameItem;
                        if (user.TryGetItem(UID, out GameItem))
                        {
                            if (GameItem.Locked == 1)
                            {

                               
                                LockedMode = 2;

                               user.Send(packet.ItemLockCreate(UID, Typ, LockedMode, dwPram));

                               GameItem.UnLockTimer = Role.Core.CreateTimer(DateTime.Now.AddDays(5));
                                GameItem.Locked = 2;
                                GameItem.Mode = Role.Flags.ItemMode.Update;
                                GameItem.Send(user,packet);
                            }
                        }
                        break;
                    }
            }

        }
    }
}
