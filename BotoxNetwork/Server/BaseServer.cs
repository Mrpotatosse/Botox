using BotoxNetwork.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BotoxNetwork.Server
{
    public abstract class BaseServer<T> where T : BaseClient
    {
        protected Socket ServerSocket { get; set; }

        public event Action OnServerStarted;
        public event Action OnServerStoped;

        public event Action<T> OnClientJoined;
        public event Action<T> OnClientLeaved;

        public event Action<Exception> OnErrorHandled;
        public event Action<IPAddress> OnForcingDetected;

        public bool IsRunning { get; private set; }
        public int Port { get; private set; }

        public IPEndPoint IP { get; private set; }

        public List<T> Clients { get; set; }
        private Dictionary<IPAddress, byte> IpForce { get; set; }

        public BaseServer(int serverPort)
        {
            Port = serverPort;
            IP = new IPEndPoint(IPAddress.Any, Port);

            IpForce = new Dictionary<IPAddress, byte>();

            Init();
        }

        private void Init()
        {
            IsRunning = false;

            ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                ReceiveTimeout = -1,
                SendTimeout = -1,
                NoDelay = true
            };

            Clients = new List<T>();
        }

        public void Start()
        {
            if (IsRunning)
                throw new Exception("server is already running");

            IsRunning = true;
            ServerSocket.Bind(IP);
            ServerSocket.Listen(32);

            OnServerStarted?.Invoke();
            AcceptClient();
        }

        private void AcceptClient()
        {
            ServerSocket.BeginAccept(ClientJoined, ServerSocket);
        }

        private void ClientJoined(IAsyncResult ar)
        {
            try
            {
                ServerSocket = (Socket)ar.AsyncState;
                T client = (T)Activator.CreateInstance(typeof(T), ServerSocket.EndAccept(ar));

                client.OnClientDisconnected += () =>
                {
                    OnClientLeaved?.Invoke(client);
                    Clients.Remove(client);
                };

                OnClientJoined?.Invoke(client);
                client.Receive();

                Clients.Add(client);
                AcceptClient();

                // limit same connexion  
                /*if (Clients.Where(x => x.RemoteIP.Address.ToString() == client.RemoteIP.Address.ToString()).Count() >= 8)
                {
                    if (IpForce.ContainsKey(client.RemoteIP.Address))
                    {
                        // limit spam
                        if (++IpForce[client.RemoteIP.Address] >= 100)
                        {
                            // action on forcing
                            OnForcingDetected?.Invoke(client.RemoteIP.Address);
                        }
                    }
                    else
                    {
                        IpForce.Add(client.RemoteIP.Address, 1);
                    }
                    client.Disconnect();
                    return;
                }*/
            }
            catch (Exception e)
            {
                OnErrorHandled?.Invoke(e);
            }
        }

        public void Stop()
        {
            if (!IsRunning)
                throw new Exception("server is not running");

            ServerSocket.Close();

            OnServerStoped?.Invoke();

            Init();
        }
    }
}
