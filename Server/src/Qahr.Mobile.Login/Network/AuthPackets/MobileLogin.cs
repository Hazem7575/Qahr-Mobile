using System;
using System.Text;

namespace Qahr.Mobile.Login.Network.AuthPackets
{
    /// <summary>
    /// Login of the Conquer Online Mobile client (1.1.2.3). The patched APK replaces the NetDragon SDK login with an
    /// account/password screen (D:\conquer\_private\comobile); the rest of the login is the client's own flow:
    ///   C->S 1797 MsgSDKLoginEx        account (0x2C, 64) + server name (0xEC, 48); no password
    ///   S->C 1798 MsgSDKLoginChallenge the client only checks the type, then sends the proof
    ///   C->S 1799 MsgSDKLoginProof     session string (4, up to 1000) = the password typed on the login screen
    ///   S->C 1055 MsgConnectEx         ticket halves (4, 8; the client sends both back in 10124), port (12), IP (20); failure = 0 at 4 + the code at 8
    ///   C->S 1518 / 1100               legality / device info after the reply, nothing to answer
    /// The mobile client knows neither TQProtect nor the TQCipher AES layer, so 1055 is sent plain.
    /// </summary>
    public static class MobileLogin
    {
        public const ushort SDKLoginEx = 1797;
        public const ushort SDKLoginChallenge = 1798;
        public const ushort SDKLoginProof = 1799;
        public const ushort ConnectEx = 1055;
        public const ushort ConnectLegalitySpec = 1518, AfterLoginInfo = 1100;

        /// <summary>Account and server name of a 1797 packet; false when it is malformed.</summary>
        public static bool ReadLoginEx(byte[] packet, out string account, out string server)
        {
            account = server = null;
            if (packet.Length < 0xEC + 48)
                return false;
            account = CString(packet, 0x2C, 64);
            server = CString(packet, 0xEC, 48);
            return account.Length > 0 && server.Length > 0;
        }

        /// <summary>The session string of a 1799 packet (the password), or null.</summary>
        public static string ReadProof(byte[] packet)
        {
            if (packet.Length < 5)
                return null;
            string session = CString(packet, 4, Math.Min(1000, packet.Length - 4));
            return session.Length > 0 && session.Length <= 32 ? session : null;
        }

        public static byte[] Challenge()
        {
            byte[] buffer = new byte[8];
            Writer.WriteUInt16(8, 0, buffer);
            Writer.WriteUInt16(SDKLoginChallenge, 2, buffer);
            return buffer;
        }

        /// <summary>The mobile form of a Forward reply. ready = the ticket halves, IP and port are set.</summary>
        public static byte[] Reply(Forward fw)
        {
            bool ready = fw.Identifier != 0 && fw.Port != 0;
            byte[] buffer = new byte[52];
            Writer.WriteUInt16(52, 0, buffer);
            Writer.WriteUInt16(ConnectEx, 2, buffer);
            if (ready)
            {
                Writer.WriteUInt32(fw.Identifier, 4, buffer);
                Writer.WriteUInt32(fw.State, 8, buffer);// the client sends [4] and [8] back to the game server (10124)
                Writer.WriteUInt32(fw.Port, 12, buffer);
                Writer.WriteUInt32(fw.State, 16, buffer);
                Writer.WriteString(fw.IP.TrimEnd('\0'), 20, buffer);
            }
            else
                Writer.WriteUInt32((uint)fw.Type, 8, buffer);
            return buffer;
        }

        static string CString(byte[] buffer, int offset, int max)
        {
            int end = offset;
            while (end < offset + max && end < buffer.Length && buffer[end] != 0)
                end++;
            return Encoding.ASCII.GetString(buffer, offset, end - offset).Trim();
        }
    }
}
