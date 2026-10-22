using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe void GetRouletteAction(this ServerSockets.Packet stream, out MsgRouletteAction.Action Mode)
        {
            Mode = (MsgRouletteAction.Action)stream.ReadUInt8();
        }

    }
    public unsafe struct MsgRouletteAction
    {
        public enum Action : byte
        {
            Join = 3,
            RemovePlayer =2,
            ShowSpectators = 1,
            Record = 0
        }

        [PacketAttribute(GamePackets.MsgRouletteAction)]
        private static void Poroces(Client.GameClient user, ServerSockets.Packet stream)
        {
            Action Mode;

            stream.GetRouletteAction(out Mode);

            switch (Mode)
            {
                case Action.Join:
                    {
                        Database.Roulettes.RouletteTable table;
                        if (user.WatchRoulette != 0)
                        {
                            if (Database.Roulettes.RoulettesPoll.TryGetValue(user.WatchRoulette, out table))
                            {
                                table.RemoveWatch(user.Player.UID);
                                table.AddPlayer(stream,user);
                            }
                        }
                        break;
                    }
                case Action.Record:
                    {

                        Database.Roulettes.RouletteTable table;
                        if (Database.Roulettes.RoulettesPoll.TryGetValue(user.PlayRouletteUID, out table))
                        {
                            var Winner = table.RegistredPlayers.Values.Where(p => p.Winning > 0).ToArray();
                            var Ranks = Winner.OrderByDescending(p => p.Winning).ToArray();


                            user.Send(stream.RouletteRecordCreate(Ranks, (byte)table.LuckyNumber));
                        }
                        if (user.WatchRoulette != 0)
                        {
                            if (Database.Roulettes.RoulettesPoll.TryGetValue(user.WatchRoulette, out table))
                            {
                                var Winner = table.RegistredPlayers.Values.Where(p => p.Winning > 0).ToArray();
                                var Ranks = Winner.OrderByDescending(p => p.Winning).ToArray();

                                user.Send(stream.RouletteRecordCreate(Ranks, (byte)table.LuckyNumber));
                            }
                        }
                        break;
                    }
                case Action.RemovePlayer:
                    {
                        Database.Roulettes.RouletteTable table;
                        if (user.PlayRouletteUID != 0)
                        {
                            if (Database.Roulettes.RoulettesPoll.TryGetValue(user.PlayRouletteUID, out table))
                            {
                                table.RemovePlayer(user);
                            }
                        }
                        else if (user.WatchRoulette != 0)
                        {
                            if (Database.Roulettes.RoulettesPoll.TryGetValue(user.WatchRoulette, out table))
                            {
                                table.RemoveWatch(user.Player.UID);
                            }
                        }
                        break;
                    }
                case Action.ShowSpectators://not sure
                    {
                        Database.Roulettes.RouletteTable table;
                        if (user.PlayRouletteUID != 0)
                        {
                            if (Database.Roulettes.RoulettesPoll.TryGetValue(user.PlayRouletteUID, out table))
                            {
                                foreach (var client in table.ClientsWatch.Values)
                                    user.Send(stream.RouletteScreenCreate(client.Player.UID));
                                
                            }
                        }
                        break;
                    }
            }
        }
    }
}
