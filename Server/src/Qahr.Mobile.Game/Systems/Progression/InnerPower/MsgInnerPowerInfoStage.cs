using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class InnerPowerStage
    {
        public enum ActionID : ushort
        {
            UpdateStage = 1,
            UpdateScore = 2
        }

    }
    public unsafe static partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet InnerPowerStageInfo(this ServerSockets.Packet stream, InnerPowerStage.ActionID action, uint UID, Role.Instance.InnerPower.Stage stage)
        {
            stream.InitWriter();

            stream.Write(UID);
            stream.Write(stage.Score);
            stream.Write(0);
            stream.Write((ushort)action);
            var array_gongs = stage.NeiGongs.Where(p => p.Unlocked).ToArray();
            stream.Write((ushort)array_gongs.Length);
            stream.Write(stage.GetNoumberAtributes(array_gongs));
            for (int x = 0; x < array_gongs.Length; x++)
            {
                var element = array_gongs[x];
                stream.Write((ushort)element.ID);
                stream.Write(element.level);
                stream.Write(element.Score);
                stream.Write((byte)(element.Complete ? 1 : 0));
            }
            var DBStage = Database.InnerPowerTable.Stages[stage.ID - 1];
            for (int x = 0; x < array_gongs.Length; x++)
            {
                var element = array_gongs[x];
                for (int y = 0; y < DBStage.NeiGongAtributes[x].AtributesType.Length; y++)
                {
                    stream.Write((ushort)element.ID);
                    var atribut = DBStage.NeiGongAtributes[x];
                    stream.Write((byte)atribut.AtributesType[y]);
                    stream.Write((uint)((atribut.AtributesValues[y] / atribut.MaxLevel) * element.level));
                }
            }
            stream.Finalize(GamePackets.MsgInnerStrengthInfo);
            return stream;
        }

    }
}
