using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.ServerSockets
{
    public class SocketThread
    {
        private System.Threading.Thread ThreadItem;
        private ServerSocket Socket;
        public SocketThread(ServerSocket socket)
        {
            this.Socket = socket;
            ThreadItem = new System.Threading.Thread(Process);
        }
        public void Start()
        {
            ThreadItem.Start();
        }

        // الثريد ده مكانش بيعمل أي شغل (لوب Sleep(1) فاضي) وكان بيتقفل بـ Thread.Abort (مش مدعوم في .NET 10).
        // دلوقتي بيستنى إشارة الإيقاف من غير ما يصحى كل 1ms، و Close بيوقفه بشكل آمن.
        private readonly System.Threading.ManualResetEvent StopSignal = new System.Threading.ManualResetEvent(false);

        public void Process()
        {
            try
            {
                StopSignal.WaitOne();
            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
        }
        public void Close()
        {
            StopSignal.Set();
        }

    }
}