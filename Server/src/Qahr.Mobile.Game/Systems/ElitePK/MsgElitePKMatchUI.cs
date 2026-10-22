using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet ElitePKMatchUICreate(this ServerSockets.Packet stream, MsgElitePKMatchUI.State state
            , MsgElitePKMatchUI.EffectTyp effect, uint OpponentUID, string OpponentName, uint TimeLeft)
        {
            stream.InitWriter();

            stream.Write((uint)state);//4
            stream.Write((uint)effect);//8

            stream.Write(OpponentUID);//12
            stream.Write((uint)0);//16
            stream.Write(OpponentName, 32);//20
            stream.ZeroFill(12);

            stream.Write(TimeLeft);//36
            stream.Write((uint)0);
            stream.Finalize(GamePackets.MsgElitePKArenic);

            return stream;
        }

    }
    public unsafe struct MsgElitePKMatchUI
    {
        public enum State : uint
        {
            Information = 1,
            BeginMatch = 2,
            Effect = 3,
            EndMatch = 4
        }
        public enum EffectTyp : uint
        {
            Effect_Win = 1,
            Effect_Lose = 0
        }

    }
}