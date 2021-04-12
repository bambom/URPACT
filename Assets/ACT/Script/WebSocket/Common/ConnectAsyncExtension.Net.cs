using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace SuperSocket.ClientEngine
{
    public static partial class ConnectAsyncExtension
    {
        private static void ConnectAsyncInternal(this EndPoint remoteEndPoint, ConnectedCallback callback, object state)
        {
             var e = CreateSocketAsyncEventArgs(remoteEndPoint, callback, state);
             var socket = new Socket(remoteEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
             socket.ConnectAsync(e);
        }
    }
}
