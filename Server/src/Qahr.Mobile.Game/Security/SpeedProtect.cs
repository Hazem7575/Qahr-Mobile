namespace Qahr.Mobile.Game.MsgLoader
{
    using System;

    /// <summary>
    /// [Audit H3] Speed/movement protection.
    /// Previously every method started with `return;` so the whole file was dead code.
    /// It is now enabled with CORRECTED timing (the interval is measured against the
    /// previous call using DateTime.Now, not a stale "previous" stamp) and deliberately
    /// conservative thresholds: only clearly impossible intervals are counted, the
    /// counter is decremented on every normal step, and the disconnect threshold is high.
    /// This catches sustained speed-hacking while leaving laggy/normal players alone.
    /// </summary>
    public class SpeedProtect
    {
        // minimum milliseconds between two consecutive events before we consider it suspicious.
        private const double MinJumpMs = 90.0;      // walking jump
        private const double MinJumpRideMs = 70.0;  // mounted jump
        private const double MinMoveMs = 80.0;      // single-step walk/run
        private const double MinFallMs = 60.0;

        // number of sustained violations before the connection is dropped.
        private const int DisconnectThreshold = 12;

        private static bool Exempt(Client.GameClient pClient)
        {
            // never punish players whose movement speed is legitimately boosted/altered.
            return pClient.Player.OnSpeedFlags()
                || pClient.Player.OnTransform
                || pClient.Player.TransformationID == 207
                || pClient.Player.TransformationID == 267
                || pClient.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.ManiacDance)
                || !pClient.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Normal);
        }

        public static void JumpProdect(Client.GameClient pClient)
        {
            lock (pClient.JumpLock)
            {
                // interval since the PREVIOUS jump (current interval, not the stale one).
                double mill = (DateTime.Now - pClient.Player.JumpingStamp).TotalMilliseconds;
                pClient.Player.PreviousJump = pClient.Player.JumpingStamp;
                pClient.Player.JumpingStamp = DateTime.Now;

                bool isRiding = pClient.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Ride);
                double min = isRiding ? MinJumpRideMs : MinJumpMs;

                // mill <= 0 happens on the very first jump (default DateTime) -> ignore.
                if (mill > 0 && mill < min && !Exempt(pClient))
                {
                    pClient.Player.CountJumpHack++;
                    if (pClient.Player.CountJumpHack >= DisconnectThreshold)
                    {
                        pClient.Player.CountJumpHack = 0;
                        Console.WriteLine("Player " + pClient.Player.Name + " Disconnect Speed Hack (Jump)");
                        pClient.Socket.Disconnect();
                    }
                }
                else if (pClient.Player.CountJumpHack > 0)
                {
                    pClient.Player.CountJumpHack--;
                }
            }
        }

        public static void MovementProdect(Client.GameClient pClient)
        {
            lock (pClient.JumpLock)
            {
                double mill = (DateTime.Now - pClient.Player.MoveingStamp).TotalMilliseconds;
                pClient.Player.PreviousMove = pClient.Player.MoveingStamp;
                pClient.Player.MoveingStamp = DateTime.Now;

                if (mill > 0 && mill < MinMoveMs && !Exempt(pClient)
                    && !pClient.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Ride))
                {
                    pClient.Player.CountMoveHack++;
                    if (pClient.Player.CountMoveHack >= DisconnectThreshold)
                    {
                        pClient.Player.CountMoveHack = 0;
                        Console.WriteLine("Player " + pClient.Player.Name + " Disconnect MovementProdect Hack");
                        pClient.Socket.Disconnect();
                    }
                }
                else if (pClient.Player.CountMoveHack > 0)
                {
                    pClient.Player.CountMoveHack--;
                }
            }
        }

        public static void FallSpeedProtect(Client.GameClient pClient)
        {
            lock (pClient.JumpLock)
            {
                double mill = (DateTime.Now - pClient.Player.FallStamp).TotalMilliseconds;
                pClient.Player.PreviousFall = pClient.Player.FallStamp;
                pClient.Player.FallStamp = DateTime.Now;

                if (mill > 0 && mill < MinFallMs && !Exempt(pClient))
                {
                    pClient.Player.CountFallHack++;
                    if (pClient.Player.CountFallHack >= DisconnectThreshold)
                    {
                        pClient.Player.CountFallHack = 0;
                        Console.WriteLine("Player " + pClient.Player.Name + " Disconnect Fall Speed Hack");
                        pClient.Socket.Disconnect();
                    }
                }
                else if (pClient.Player.CountFallHack > 0)
                {
                    pClient.Player.CountFallHack--;
                }
            }
        }
    }
}
