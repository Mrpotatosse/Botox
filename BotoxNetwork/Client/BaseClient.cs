using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BotoxNetwork.Client
{
    public abstract class BaseClient
    {
        // 2 4 8 16 32 64 128 256 512 1024 2048 4096 8192 16384 32768 65536 131072
        // 1 2 3 4  5  6  7   8   9   10   11   12   13   14    15    16    17
        private static readonly int BUFFER_LENGTH = (int)Math.Pow(2,15);

        private byte[] ReceivedBuffer { get; set; }
        private byte[] SentBuffer { get; set; }

        protected Socket ClientSocket { get; set; }

        public event Action OnClientConnected;
        public event Action OnClientDisconnected;

        public event Action<byte[]> OnClientReceivedData;
        public event Action<byte[]> OnClientSentData;

        public event Action<Exception> OnErrorHandled;

        public IPEndPoint RemoteIP => ClientSocket.RemoteEndPoint as IPEndPoint;
        public IPEndPoint LocalIP => ClientSocket.LocalEndPoint as IPEndPoint;

        public bool IsRunning
        {
            get
            {
                if (ClientSocket != null && ClientSocket.Connected)
                {
                    try
                    {
                        if (ClientSocket.Poll(0, SelectMode.SelectRead))
                        {
                            if (ClientSocket.Receive(new byte[1], SocketFlags.Peek) == 0)
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
                return false;
            }
        }

        private bool _leaveRequest { get; set; } = false;

        public BaseClient(Socket socket)
        {
            ClientSocket = socket;
            Init();
        }

        public BaseClient() : this(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
        {

        }

        private void Init()
        {
            ReceivedBuffer = new byte[BUFFER_LENGTH];
        }

        public void Connect(IPEndPoint ip)
        {
            try
            {
                ClientSocket.BeginConnect(ip, ClientConnected, ClientSocket);
            }
            catch (Exception e)
            {
                OnErrorHandled?.Invoke(e);
            }
        }

        protected virtual void ClientConnected(IAsyncResult ar)
        {
            OnClientConnected?.Invoke();

            Receive();
        }

        public void Disconnect()
        {
            try
            {
                if (IsRunning)
                {
                    ClientSocket.BeginDisconnect(false, ClientDisconnected, ClientSocket);
                    _leaveRequest = true;
                }
            }
            catch (Exception e)
            {
                OnErrorHandled?.Invoke(e);
            }
        }

        protected virtual void ClientDisconnected(IAsyncResult ar)
        {
            OnClientDisconnected?.Invoke();
        }

        private MemoryStream _rcvBuffer { get; set; } = new MemoryStream();
        public void Receive()
        {
            try
            {                    
                ClientSocket.BeginReceive(ReceivedBuffer, 0, ReceivedBuffer.Length, SocketFlags.None, DataReceived, ClientSocket);
            }
            catch (Exception e)
            {
                OnErrorHandled?.Invoke(e);
            }
        }
        
        protected virtual void DataReceived(IAsyncResult ar)
        {
            ClientSocket = (Socket)ar.AsyncState;

            int len = ClientSocket.EndReceive(ar, out SocketError error);

            if (IsRunning)
            {
                switch (error)
                {
                    case SocketError.Success:
                        if (len > 0)
                        {
                            _rcvBuffer.Write(ReceivedBuffer, 0, len);

                            OnClientReceivedData?.Invoke(_rcvBuffer.ToArray());
                            ReceivedBuffer = new byte[BUFFER_LENGTH];

                            _rcvBuffer.Dispose();
                            _rcvBuffer = new MemoryStream();

                            Receive();
                        }
                        return;
                }
            }
            else
            {
                if (!_leaveRequest)
                {
                    OnClientDisconnected?.Invoke();
                }
            }
        }

        public void Send(byte[] data)
        {
            try
            {
                SentBuffer = data;
                ClientSocket.BeginSend(SentBuffer, 0, SentBuffer.Length, SocketFlags.None, SentData, ClientSocket);
            }
            catch (Exception e)
            {
                OnErrorHandled?.Invoke(e);
            }
        }

        private void SentData(IAsyncResult ar)
        {
            OnClientSentData?.Invoke(SentBuffer);
        }
    }
}
