using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class MsgRacePotion
    {
        public enum RaceItemType : ushort
        {
            Null = 8329,
            ChaosBomb = 8330,
            SpiritPotion = 8331,
            ExcitementPotion = 8332,
            FrozenTrap = 8333,
            ScreamBomb = 8334,
            SluggishPotion = 8335,
            GuardPotion = 8336,
            DizzyHammer = 8337,
            TransformItem = 8338,
            RestorePotion = 8339,
            SuperExcitementPotion = 8340,
        }
        public class RacePotion
        {
            public ushort Amount;
            public RaceItemType PotionType;
            public int Location;
            public uint dwParam;
        }

        public static ServerSockets.Packet CreateRecePotion(this ServerSockets.Packet stream, RacePotion item)
        {
            stream.InitWriter();

            stream.Write(item.Amount);
            stream.Write((ushort)item.PotionType);
            stream.Write(item.Location);
            stream.Write(item.dwParam);

            stream.Finalize(GamePackets.MsgRaceTrackProp);
            return stream;
        }
        public static unsafe void GetRecePotion(this ServerSockets.Packet stream, out RacePotion item)
        {
            item = new RacePotion();
            item.Amount = stream.ReadUInt16();
            item.PotionType = (RaceItemType)stream.ReadUInt16();
            item.Location = stream.ReadInt32();
            item.dwParam = stream.ReadUInt32();

        }
        [PacketAttribute(GamePackets.MsgRaceTrackProp)]
        private static void Poroces(Client.GameClient user, ServerSockets.Packet stream)
        {
            RacePotion pot;
            stream.GetRecePotion(out pot);

            int i = pot.Location - 1;
            if (i < 0 || i > 4)
                return;
            if (user.Player.RacePotions[i] == null)
                return;
            var potion = user.Player.RacePotions[i];
            potion.Count--;

            pot.PotionType = potion.Type;
            pot.Amount = (ushort)potion.Count;
            user.Send(stream.CreateRecePotion(pot));

            user.ApplyRacePotion(potion.Type, pot.dwParam);

            if (potion.Count == 0)
                user.Player.RacePotions[i] = null;
        }
    }
}
