using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using SuperSocket.ClientEngine;
using WebSocketHandShake;

namespace WebSocket4Net
{
    public partial class WebSocket
    {
        internal TcpClientSession Client { get; private set; }

        /// <summary>
        /// Gets the last active time of the websocket.
        /// </summary>
        public DateTime LastActiveTime { get; internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether [enable auto send ping].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable auto send ping]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableAutoSendPing { get; set; }

        /// <summary>
        /// Gets or sets the interval of ping auto sending, in seconds.
        /// </summary>
        /// <value>
        /// The auto send ping internal.
        /// </value>
        public int AutoSendPingInterval { get; set; }

        protected const string UserAgentKey = "UserAgent";

        internal Uri TargetUri { get; private set; }

        internal List<KeyValuePair<string, string>> Cookies { get; private set; }

        public WebSocketState State { get; private set; }

        public bool Handshaked { get; private set; }

        //private static ProtocolProcessorFactory m_ProtocolProcessorFactory;

        internal bool NotSpecifiedVersion { get; private set; }

        private Timer m_PingTimer;

        static WebSocket()
        {
            //m_ProtocolProcessorFactory = new ProtocolProcessorFactory(new DraftHybi10Processor());
        }

        private EndPoint ResolveUri(string uri, int defaultPort, out int port)
        {
            TargetUri = new Uri(uri);

            IPAddress ipAddress;

            EndPoint remoteEndPoint;

            port = TargetUri.Port;

            if (port <= 0)
                port = defaultPort;

            if (IPAddress.TryParse(TargetUri.Host, out ipAddress))
                remoteEndPoint = new IPEndPoint(ipAddress, port);
            else
                remoteEndPoint = new DnsEndPoint(TargetUri.Host, port);

            return remoteEndPoint;
        }

        TcpClientSession CreateClient(string uri)
        {
            int port;
            var targetEndPoint = ResolveUri(uri, 80, out port);

            return new AsyncTcpSession(targetEndPoint);
        }

#if false//!SILVERLIGHT

        TcpClientSession CreateSecureClient(string uri)
        {
            int hostPos = uri.IndexOf('/', m_SecureUriPrefix.Length);

            if (hostPos < 0)//wss://localhost or wss://localhost:xxxx
            {
                hostPos = uri.IndexOf(':', m_SecureUriPrefix.Length, uri.Length - m_SecureUriPrefix.Length);

                if (hostPos < 0)
                    uri = uri + ":" + m_SecurePort + "/";
                else
                    uri = uri + "/";
            }
            else if (hostPos == m_SecureUriPrefix.Length)//wss://
            {
                throw new ArgumentException("Invalid uri", "uri");
            }
            else//wss://xxx/xxx
            {
                int colonPos = uri.IndexOf(':', m_SecureUriPrefix.Length, hostPos - m_SecureUriPrefix.Length);

                if (colonPos < 0)
                {
                    uri = uri.Substring(0, hostPos) + ":" + m_SecurePort + uri.Substring(hostPos);
                }
            }

            int port;
            var targetEndPoint = ResolveUri(uri, m_SecurePort, out port);

            if (port == m_SecurePort)
                HandshakeHost = TargetUri.Host;
            else
                HandshakeHost = TargetUri.Host + ":" + port;

            return new SslStreamTcpSession(targetEndPoint);
        }

#endif

        private void Initialize(string uri)
        {   
            State = WebSocketState.None;

            TcpClientSession client = CreateClient(uri);

            client.Connected += new EventHandler(client_Connected);
            client.Error += new EventHandler<ErrorEventArgs>(client_Error);
            client.Closed += new EventHandler(client_Closed);
            client.DataReceived += new EventHandler<DataEventArgs>(client_DataReceived);

            Client = client;

            //Ping auto sending is enabled by default
            EnableAutoSendPing = true;
        }
        void client_Connected(object sender, EventArgs e)
        {
            OnConnected();
        }

        void client_Error(object sender, ErrorEventArgs e)
        {
            OnError(e);
        }

        void client_Closed(object sender, EventArgs e)
        {
            OnClosed();
        }

        void client_DataReceived(object sender, DataEventArgs e)
        {
            OnDataReceived(e.Data, e.Offset, e.Length);
        }

        public int ReceiveBufferSize
        {
            get { return Client.ReceiveBufferSize; }
            set { Client.ReceiveBufferSize = value; }
        }

        public void Open()
        {
            State = WebSocketState.Connecting;

            Client.Connect();
        }

        //private static IProtocolProcessor GetProtocolProcessor(WebSocketVersion version)
        //{
        //    var processor = m_ProtocolProcessorFactory.GetProcessorByVersion(version);

        //    if (processor == null)
        //        throw new ArgumentException("Invalid websocket version");

        //    return processor;
        //}

        void OnConnected()
        {
            //握手
            //byte[] handshakeBuffer  = new byte[] { 0x00, 0x04, 0x00, 0x1E };
            //Client.Send(handshakeBuffer, 0, handshakeBuffer.Length);

            //Handshake.Instance(this);
            OnHandshaked();
        }

        public void OnHandshaked()
        {
            State = WebSocketState.Open;

            Handshaked = true;

            if (m_Opened == null)
                return;

            m_Opened(this, EventArgs.Empty);
        }

        private EventHandler m_Opened;

        public event EventHandler Opened
        {
            add { m_Opened += value; }
            remove { m_Opened -= value; }
        }

        private EventHandler<MessageReceivedEventArgs> m_MessageReceived;

        public event EventHandler<MessageReceivedEventArgs> MessageReceived
        {
            add { m_MessageReceived += value; }
            remove { m_MessageReceived -= value; }
        }

        internal void FireMessageReceived(string message)
        {
            if (m_MessageReceived == null)
                return;

            m_MessageReceived(this, new MessageReceivedEventArgs(message));
        }

        private EventHandler<DataReceivedEventArgs> m_DataReceived;

        public event EventHandler<DataReceivedEventArgs> DataReceived
        {
            add { m_DataReceived += value; }
            remove { m_DataReceived -= value; }
        }

        internal void FireDataReceived(byte[] data)
        {
            if (m_DataReceived == null)
                return;

            m_DataReceived(this, new DataReceivedEventArgs(data));
        }

        private const string m_NotOpenSendingMessage = "You must send data by websocket after websocket is opened!";

        private bool EnsureWebSocketOpen()
        {
            if (!Handshaked)
            {
                OnError(new Exception(m_NotOpenSendingMessage));
                return false;
            }

            return true;
        }

        public void Send(string message)
        {
            if (!EnsureWebSocketOpen())
                return;

            byte[] playloadData = Encoding.UTF8.GetBytes(message);
            Client.Send(playloadData, 0, playloadData.Length);
        }

        public void Send(byte[] data)
        {
            Send(data, 0, data.Length);
        }

        public void Send(byte[] data, int offset, int length)
        {
            if (!EnsureWebSocketOpen())
                return;

            Client.Send(data, 0, length);
        }

        public void Send(IList<ArraySegment<byte>> segments)
        {
            if (!EnsureWebSocketOpen())
                return;

            Client.Send(segments);
        }

        private void OnClosed()
        {
            var fireBaseClose = false;

            if (State == WebSocketState.Closing || State == WebSocketState.Open)
                fireBaseClose = true;

            State = WebSocketState.Closed;

            if (fireBaseClose)
                FireClosed();
        }
		
		public void GodClose()
		{
			CloseWithoutHandshake();
		}
		
        public void Close()
        {
            //The websocket never be opened
            if (State == WebSocketState.None)
            {
                State = WebSocketState.Closed;
                OnClosed();
                return;
            }

            Close(string.Empty);
        }

        public void Close(string reason)
        {
            //关闭socket
            State = WebSocketState.Closing;
        }

        internal void CloseWithoutHandshake()
        {
            Client.Close();
        }

        private void OnDataReceived(byte[] data, int offset, int length)
        {
            FireDataReceived(data);
        }

        internal void FireError(Exception error)
        {
            OnError(error);
        }

        private EventHandler m_Closed;

        public event EventHandler Closed
        {
            add { m_Closed += value; }
            remove { m_Closed -= value; }
        }

        private void FireClosed()
        {
            if (m_PingTimer != null)
            {
                m_PingTimer.Change(Timeout.Infinite, Timeout.Infinite);
                m_PingTimer.Dispose();
                m_PingTimer = null;
            }

            var handler = m_Closed;

            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        private EventHandler<ErrorEventArgs> m_Error;

        public event EventHandler<ErrorEventArgs> Error
        {
            add { m_Error += value; }
            remove { m_Error -= value; }
        }

        private void OnError(ErrorEventArgs e)
        {
            if (m_Error == null)
                return;

            State = WebSocketState.Closed;
            m_Error(this, e);
        }

        private void OnError(Exception e)
        {
            OnError(new ErrorEventArgs(e));
        }
    }
}
