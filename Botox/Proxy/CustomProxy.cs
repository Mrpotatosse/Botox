using Botox.Extension;
using Botox.Protocol;
using BotoxNetwork.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static BotoxNetwork.IO.DofusIO;

namespace Botox.Proxy
{
    // labot : https://louisabraham.github.io/LaBot/decoder.html?hex=
    //         https://louisabraham.github.io/LaBot/protocol.js
    public class CustomProxy : BaseServer<CustomClient>
    {
        private IList<ProxyElement> Elements { get; set; }        
        public int ProcessId { get; private set; }

        public CustomProxy(int serverPort, int processId) : base(serverPort)
        {
            ProcessId = processId;
            Elements = new List<ProxyElement>();

            OnClientJoined += CustomProxy_OnClientJoined;
            OnClientLeaved += CustomProxy_OnClientLeaved;

            OnErrorHandled += CustomProxy_OnErrorHandled;
        }

        private void CustomProxy_OnErrorHandled(Exception obj)
        {
            Console.WriteLine($"{obj}");
        }

        public void AddClient(IPEndPoint remoteIp)
        {
            ProxyElement element = new ProxyElement()
            {
                FakeClient = new CustomClient(),
                FakeClientRemoteIp = remoteIp
            };

            Elements.Add(element);
        }

        private void CustomProxy_OnClientLeaved(CustomClient obj)
        {
            if(Elements.FirstOrDefault(x => x.Client == obj) is ProxyElement element)
            {
                Elements.Remove(element);
            }
        }

        private void CustomProxy_OnClientJoined(CustomClient obj)
        {
            if(Elements.FirstOrDefault(x => x.Client is null) is ProxyElement element)
            {
                element.Client = obj;
                element.Init();
            }
        }
    }

    class ProxyElement
    {
        private readonly object ReceiverLock = new object();

        public CustomClient Client { get; set; }
        public CustomClient FakeClient { get; set; }
        public IPEndPoint FakeClientRemoteIp { get; set; }

        public MessageInformation ClientMessageInformation { get; set; }
        public MessageInformation ServerMessageInformation { get; set; }

        public void Init()
        {
            FakeClient.OnClientReceivedData += Proxy_FakeClient_OnClientReceivedData;
            Client.OnClientReceivedData += Proxy_Client_OnClientReceivedData;

            FakeClient.OnClientDisconnected += Proxy_FakeClient_OnClientDisconnected;
            Client.OnClientDisconnected += Proxy_Client_OnClientDisconnected;

            ClientMessageInformation = new MessageInformation(true);
            ServerMessageInformation = new MessageInformation(false);

            ClientMessageInformation.OnMessageParsed += ClientMessageInformation_OnMessageParsed;
            ServerMessageInformation.OnMessageParsed += ServerMessageInformation_OnMessageParsed;

            FakeClient.Connect(FakeClientRemoteIp);
        }

        private void ServerMessageInformation_OnMessageParsed(ProtocolJsonElement message)
        {
            Console.WriteLine($"[Server {FakeClient.RemoteIP}]{message}");

        }

        private void ClientMessageInformation_OnMessageParsed(ProtocolJsonElement message)
        {
            Console.WriteLine($"[Client {FakeClient.RemoteIP}]{message}");
        }

        private void Proxy_Client_OnClientDisconnected()
        {
            if (FakeClient.IsRunning)
                FakeClient.Disconnect();
        }

        private void Proxy_FakeClient_OnClientDisconnected()
        {
            if (Client.IsRunning)
                Client.Disconnect();
        }

        private void Proxy_Client_OnClientReceivedData(byte[] obj)
        {
            ClientMessageInformation.Build(obj);
            FakeClient.Send(obj);
        }

        private void Proxy_FakeClient_OnClientReceivedData(byte[] obj)
        {
            ServerMessageInformation.Build(obj);
            Client.Send(obj);
        }
    }
}
