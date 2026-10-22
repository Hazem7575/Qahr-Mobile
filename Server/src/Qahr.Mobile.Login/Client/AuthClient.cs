// * Created by ElmistRo
// * Copyright © 2010-2014
// * ElmistRo - Project

using System;
using System.Net.Sockets;
using Qahr.Mobile.Login.Network.Sockets;
using Qahr.Mobile.Login.Network;

namespace Qahr.Mobile.Login.Client
{
    public unsafe class AuthClient
    {
        private ClientWrapper _socket;
        public Auth.TQAuthProtect.Authentication Info;
        public Database.AccountTable Account;
        public Network.Cryptography.AuthCryptography Cryptographer;
        public int PasswordSeed;
        /// <summary>Mobile login: account and server name from 1797, waiting for the 1799 proof; IsMobile = reply with 1055.</summary>
        public string MobileAccount, MobileServer;
        public bool IsMobile;
        /// <summary>QahrMobile: the "H2:" hashed proof of a 1799 login (MobileLoginProof), null for a password login.</summary>
        public string MobileProof;
        /// <summary>QahrMobile: the 1799 session was a valid one-time Google login token (MobileGoogleLogin), no password to check.</summary>
        public bool GoogleVerified;
        public ConcurrentPacketQueue Queue;
        public AuthClient(ClientWrapper socket)
        {
            Queue = new ConcurrentPacketQueue(0);
            _socket = socket;
        }
        public void Send(byte[] buffer)
        {
            byte[] _buffer = new byte[buffer.Length];
            Buffer.BlockCopy(buffer, 0, _buffer, 0, buffer.Length);
            Cryptographer.Encrypt(_buffer);
            _socket.Send(_buffer);
        }
        public string IP
        {
            get { return _socket.IP; }

        }
        public void Disconnect()
        {
            _socket.Disconnect();
        }
        public void Send(Interfaces.IPacket buffer)
        {

            Send(buffer.ToArray());
        }
    }
}