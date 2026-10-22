using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet ActivityTasksCreate(this ServerSockets.Packet stream, MsgActivityTasks.Action mode, Role.Instance.Activeness.Task[] Items = null)
        {
            stream.InitWriter();

            stream.Write((byte)mode);
            if (Items != null)
            {
                stream.Write((byte)Items.Length);

                for (int x = 0; x < Items.Length; x++)
                {
                    stream.Write(Items[x].ID);
                    stream.Write(Items[x].Completed);
                    stream.Write(Items[x].Progres);
                }
            }
            else
                stream.Write((byte)0);

            stream.Finalize(GamePackets.MsgActivityTask);
            return stream;
        }


    }
    public unsafe struct MsgActivityTasks
    {
        public enum Action : byte
        {
            InitializeList = 3
        }    
    }
}
