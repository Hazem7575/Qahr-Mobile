using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet RouletteTableCreate(this ServerSockets.Packet stream, uint UID, uint TableNumber
            , MsgRouletteTable.TableType MoneyType, ushort X, ushort Y, ushort Mesh, byte PlayersCount)
        {
            stream.InitWriter();

            stream.Write(UID);
            stream.Write(TableNumber);
            stream.Write((uint)MoneyType);
            stream.Write(X);
            stream.Write(Y);
            stream.Write(Mesh);
            stream.Write(PlayersCount);



            // QahrMobile: the table on the map is CMsgRouletteNpcInfo (10278) on the mobile; 2011 (CMsgRouletteTable)
            // is the table window there (RouletteOpenGuiCreate) and would read this struct as a player list.
            stream.Finalize(GamePackets.MsgRouletteNpcInfo);
            return stream;
        }

    }
    public class MsgRouletteTable
    {
        public enum TableType: uint
        {
            Money = 1,
            ConquerPoints =2
        }
        public uint UID;
        public uint TableNumber;
        public TableType MoneyType;
        public ushort X;
        public ushort Y;
        public ushort Mesh;
        public byte PlayersCount;//??? Player count on table?
    }
}
