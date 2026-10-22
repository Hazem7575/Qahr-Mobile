using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.WindowsAPI;
namespace Qahr.Mobile
{
    public static class MsgHandShake
    {
        public static ServerSockets.Packet Handshake(this ServerSockets.Packet msg, byte[] p, byte[] g, byte[] publicKey)
        {
            var size = 75 + p.Length + g.Length + publicKey.Length;
            msg.Seek(11);                   // padding
            msg.Write(size - 11);           //
            msg.Write(10);                  // junk length
            msg.SeekForward(10);            // junk
            msg.Write(8);                   // client ivec
            msg.ZeroFill(8);                //
            msg.Write(8);                   // server ivec
            msg.ZeroFill(8);                //
            msg.Write(p.Length);            // p
            p.Iterate(msg.Write);           //
            msg.Write(g.Length);            // g
            g.Iterate(msg.Write);           //
            msg.Write(publicKey.Length);    // publicKey
            publicKey.Iterate(msg.Write);   //
            msg.ZeroFill(2);                // 2 spaces
            msg.WriteSeal();
            msg.Size = msg.Position;
            return msg;
        }
    }
}
