using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe void GetTransformFairy(this ServerSockets.Packet stream, out MsgTransformFairy.Action action, out uint fairy, out uint uid)
        {
            action = (MsgTransformFairy.Action)stream.ReadUInt32();
            uint unknow = stream.ReadUInt32();
            fairy = stream.ReadUInt32();
            uid = stream.ReadUInt32();
        }
        public static unsafe ServerSockets.Packet TransformFairyCreate(this ServerSockets.Packet stream, MsgTransformFairy.Action action, uint fairy, uint uid)
        {
            stream.InitWriter();

            stream.Write((uint)action);
            stream.Write((uint)0);//unknow
            stream.Write(fairy);
            stream.Write(uid);

            stream.Finalize(GamePackets.MsgSuitStatus);
            return stream;
        }
    }
    public unsafe struct MsgTransformFairy
    {
        public enum Action : uint
        {
            Active = 1,
            Dezactive = 2
        }
        public Action Mode;
        public uint UnKnow;
        public uint FairyType;
        public uint UID;

        public static MsgTransformFairy Create()
        {
            MsgTransformFairy fairy = new MsgTransformFairy();
            return fairy;
        }
        [PacketAttribute(GamePackets.MsgSuitStatus)]
        public unsafe static void Handler(Client.GameClient user, ServerSockets.Packet stream)
        {
            Action Mode;
            uint fairy;
            uint uid;
            stream.GetTransformFairy(out Mode, out fairy, out uid);

            switch (Mode)
            {
                case Action.Active:
                    {
                        if (user.Player.OnFairy)
                        {
                            user.Player.FairySpawn.Mode = Action.Dezactive;
                            user.Player.View.SendView(stream.TransformFairyCreate(Action.Dezactive, user.Player.FairySpawn.FairyType, user.Player.FairySpawn.UID), true);
                            user.Player.OnFairy = false;
                            user.Player.FairySpawn = default(MsgTransformFairy);
                        }
                        user.Player.OnFairy = true;
                        uid = user.Player.UID;
                        user.Player.View.SendView(stream.TransformFairyCreate(Action.Active, fairy, uid), true);
                        user.Player.FairySpawn = Create();
                        user.Player.FairySpawn.Mode = Mode;
                        user.Player.FairySpawn.FairyType = fairy;
                        user.Player.FairySpawn.UID = uid;
                        break;
                    }
                case Action.Dezactive:
                    {
                        user.Player.OnFairy = false;
                        uid = user.Player.UID;
                        user.Player.View.SendView(stream.TransformFairyCreate(Action.Dezactive, fairy, uid), true);
                        user.Player.FairySpawn = default(MsgTransformFairy);
                        break;
                    }
            }
        }
    }
}
