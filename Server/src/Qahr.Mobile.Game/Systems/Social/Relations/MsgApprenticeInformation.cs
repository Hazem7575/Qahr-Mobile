using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe class MsgApprenticeInformation
    {
        public enum Action : uint
        {
            Mentor = 1,
            Apprentice = 2
        }
        public ushort Length;
        public ushort PacketID;
        public Action Mode;
        public uint Mentor_ID;
        public uint Apprentice_ID;
        public uint Mesh;
        public uint Shared_Battle_Power;
        public uint _999999;
        public uint Enrole_date;
        public uint Level;
        public uint Class;
        public ushort PkPoints;
        public uint Online;//56
        public uint UnKnow2;
        public ulong Apprentice_Experience;
        public ushort Apprentice_Blessing;
        public ushort Apprentice_Composing;
        //public unsafe fixed sbyte aStrings[74];

        public string MentorName;
        public string ApprenticeSpouse;
        public string ApprenticeName;


        public void Fill(Client.GameClient client)
        {
            Level = (byte)client.Player.Level;
            Class = client.Player.Class;
            PkPoints = client.Player.PKPoints;
            Mesh = client.Player.Mesh;
            Online = 1;
        }
        public unsafe void WriteString(string _MentorName, string _ApprenticeSpouse, string _ApprenticeName)
        {
            MentorName = _MentorName;
            ApprenticeSpouse = _ApprenticeSpouse;
            ApprenticeName = _ApprenticeName;
        }
        public ServerSockets.Packet GetArray(ServerSockets.Packet stream)
        {
            stream.InitWriter();

            stream.Write((uint)Mode);
            stream.Write(Mentor_ID);
            stream.Write(Apprentice_ID);
            stream.Write(Mesh);
            stream.Write(Shared_Battle_Power);
            stream.Write(_999999);
            stream.Write(Enrole_date);
            stream.Write(Level);
            stream.Write(Class);
            stream.Write(PkPoints);
            stream.ZeroFill(18);
            stream.Write((uint)Online);
            stream.Write(UnKnow2);
            stream.ZeroFill(4);
            stream.Write(Apprentice_Experience);
            stream.Write(Apprentice_Blessing);
            stream.Write(Apprentice_Composing);
            stream.Write(MentorName, ApprenticeName, ApprenticeSpouse);

            stream.Finalize(PacketID);

            return stream;
        }
        public static MsgApprenticeInformation Create()
        {
            MsgApprenticeInformation packet = new MsgApprenticeInformation();
            packet.Length = 150;
            packet.PacketID = GamePackets.MsgGuideInfo;
            packet._999999 = 999999;
            return packet;
        }

    }
}
