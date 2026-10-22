using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe void GetProficiency(this ServerSockets.Packet stream, out MsgProficiency prof)
        {
            prof = new MsgProficiency();
            prof.ID = stream.ReadUInt32();
            prof.Level = stream.ReadUInt32();
            prof.Experience = stream.ReadUInt32();
            prof.NeededExperience = stream.ReadUInt32();
        }
        public static unsafe ServerSockets.Packet ProficiencyCreate(this ServerSockets.Packet stream, uint ID, uint Level, uint Experience, uint NeededExperience)
        {
            NeededExperience = BaseFunc.ProficiencyLevelExperience((byte)Level);
            stream.InitWriter();

            stream.Write(ID);
            stream.Write(Level);
            stream.Write(Experience);
            stream.Write(NeededExperience);//UID);

            stream.Finalize(GamePackets.MsgWeaponSkill);
            return stream;
        }
    }
    public class MsgProficiency
    {
        public uint ID;
        public uint Level;
        public uint Experience;
        public uint NeededExperience;
        public byte PreviouseLevel;
    }
}
