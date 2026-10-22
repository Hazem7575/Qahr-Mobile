// * Created by ElmistRo
// * Copyright © 2010-2014
// * ElmistRo - Project

using System;
using System.Threading;
using System.Threading.Generic;
using Qahr.Mobile.Login.Network.Sockets;

namespace Qahr.Mobile.Login
{
    public unsafe class World
    {
        public static StaticPool ReceivePool, SendPool;
        public TimerRule<ClientWrapper> ConnectionReceive, ConnectionReview, ConnectionSend;

        public void Init()
        {
            ConnectionReview = new TimerRule<Network.Sockets.ClientWrapper>(connectionReview, 1000, ThreadPriority.Lowest);
            ConnectionReceive = new TimerRule<Network.Sockets.ClientWrapper>(connectionReceive, 1, ThreadPriority.Highest);
            ConnectionSend = new TimerRule<Network.Sockets.ClientWrapper>(connectionSend, 1, ThreadPriority.Highest);
        }
        public World()
        {
            ReceivePool = new StaticPool(24).Run();
            SendPool = new StaticPool(24).Run();
        }

        private void connectionReview(Network.Sockets.ClientWrapper wrapper, int time)
        {
            Network.Sockets.ClientWrapper.TryReview(wrapper);
        }
        private void connectionReceive(Network.Sockets.ClientWrapper wrapper, int time)
        {
            Network.Sockets.ClientWrapper.TryReceive(wrapper);
        }
        private void connectionSend(Network.Sockets.ClientWrapper wrapper, int time)
        {
            Network.Sockets.ClientWrapper.TrySend(wrapper);
        }

        public static IDisposable Subscribe<T>(TimerRule<T> rule, T param, StaticPool pool)
        {
            return pool.Subscribe<T>(rule, param);
        }
        public static IDisposable Subscribe<T>(TimerRule<T> rule, T param, StandalonePool pool)
        {
            return pool.Subscribe<T>(rule, param);
        }
       
    }
}