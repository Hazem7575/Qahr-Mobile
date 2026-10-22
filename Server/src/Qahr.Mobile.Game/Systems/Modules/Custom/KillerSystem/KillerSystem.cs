using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.Game.MsgTournaments
{
    public class KillerSystem
    {
        public enum KillFlag : byte
        {
            None = 0,
            FirstBloodKill = 1,
            DoubleKill = 2,
            TripleKill = 3,
            UnStoppable = 4
        }

        public static string GetSound(KillerSystem user)
        {
            switch (user.Flag)
            {
                case KillFlag.FirstBloodKill: return "sound/fc1.wav";
                case KillFlag.DoubleKill: return "sound/fc2.wav";
                case KillFlag.TripleKill: return "sound/fc3.wav";
                case KillFlag.UnStoppable: return "sound/fc4.wav";
            }
            return "none";
        }

        public KillFlag Flag = KillFlag.None;
        public bool Dead = false;
        public uint UID = 0;
        public void CheckDead(uint _UID)
        {
            if (_UID == UID)
                Dead = true;
        }
        public KillerSystem()
        {

        }

        public void Update(Client.GameClient client)
        {
            if (UID == 0)
                UID = client.Player.UID;
            if (UID == client.Player.UID && Flag < KillerSystem.KillFlag.UnStoppable && Dead == false)
            {
                switch (Flag)
                {
                    case KillFlag.None: Flag = KillFlag.FirstBloodKill; break;
                    case KillFlag.FirstBloodKill: Flag = KillFlag.DoubleKill; break;
                    case KillFlag.DoubleKill: Flag = KillFlag.TripleKill; break;
                    case KillFlag.TripleKill: Flag = KillFlag.UnStoppable; break;
                }
                Send(client);
            }
        }
        public void Send(Client.GameClient client)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                using (var rec2 = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    var stream2 = rec2.GetStream();

                    int Reward = (int)(10000 * (byte)this.Flag);
                    client.Player.ConquerPoints += 10000;

                    Game.MsgServer.MsgStringPacket packet = new Game.MsgServer.MsgStringPacket();
                    packet.ID = MsgServer.MsgStringPacket.StringID.Sound;
                    packet.Strings = new string[2];
                    packet.Strings[0] = GetSound(this);
                    packet.Strings[1] = "1";
                    stream = stream.StringPacketCreate(packet);

                    var msg = new MsgMessage("[" + this.Flag.ToString() + "]" + client.Player.Name + " received " + Reward.ToString() + " ConquerPoints.", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System);
                    stream2 = msg.GetArray(stream2);
                    foreach (var user in Pool.GamePoll.Values)
                    {
                        if (user.Player.Map == client.Player.Map && user.Player.DynamicID == client.Player.DynamicID)
                        {
                            user.Send(stream);
                            user.Send(stream2);
                        }
                    }
                }
            }
        }
    }
}
