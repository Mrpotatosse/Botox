using Botox.Configuration;
using Botox.Extension;
using Botox.Handler;
using Botox.Protocol;
using Botox.Protocol.JsonField;
using BotoxNetwork.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public static uint FAKE_MESSAGE_SENT = 0;
        public static uint LAST_GLOBAL_INSTANCE_ID = 0;
        public static uint SERVER_MESSAGE_RCV = 0;

        public static uint FAKE_MSG_INSTANCE_ID => FAKE_MESSAGE_SENT + LAST_GLOBAL_INSTANCE_ID + SERVER_MESSAGE_RCV;            

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

        private void ServerMessageInformation_OnMessageParsed(NetworkElementField obj, ProtocolJsonContent con)
        {
            CustomProxy.SERVER_MESSAGE_RCV++;
            if (ConfigurationManager.Instance.Startup.show_message)
            {
                Console.WriteLine($"[Server({FakeClient.RemoteIP})] {obj.name} ({obj.protocolID})");
                if (ConfigurationManager.Instance.Startup.show_message_content)
                {
                    Console.WriteLine($"{con}");
                }
            }

            /*byte[] data = obj.ToByte(con);
            bool check = ServerMessageInformation.Information.Data.Same(data, out int diff, true);
            if (!check && diff >= 0)
                Console.WriteLine($"SAME ? {check} [{diff}:({data[diff]},{ServerMessageInformation.Information.Data[diff]})] -> {obj.name}");*/

            HandlerManager.Instance.Handle((uint)obj.protocolID, Client, FakeClient, con);
        }

        private void ClientMessageInformation_OnMessageParsed(NetworkElementField obj, ProtocolJsonContent con)
        {
            CustomProxy.LAST_GLOBAL_INSTANCE_ID = ClientMessageInformation.Information.InstanceId;
            CustomProxy.SERVER_MESSAGE_RCV = 0;
            uint instance_id = ClientMessageInformation.Information.InstanceId + CustomProxy.FAKE_MESSAGE_SENT;
            if (ConfigurationManager.Instance.Startup.show_message)
            {
                Console.WriteLine($"[Client({FakeClient.RemoteIP})] (n°{instance_id}) {obj.name} ({obj.protocolID})");
                if (ConfigurationManager.Instance.Startup.show_message_content)
                {
                    Console.WriteLine($"{con}");
                }
            }

            /*byte[] data = obj.ToByte(con);
            bool check = ClientMessageInformation.Information.Data.Same(data, out int diff);
            if(!check)
                Console.WriteLine($"SAME ? {check} [{diff}:({data[diff]},{ClientMessageInformation.Information.Data[diff]})] -> {obj.name}");*/
            
            FakeClient.Send(ClientMessageInformation.Information.ReWriteInstanceId(instance_id));
            HandlerManager.Instance.Handle((uint)obj.protocolID, Client, FakeClient ,con);
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
            if (ConfigurationManager.Instance.Startup.show_data)
                Console.WriteLine($"- - - - DATA len : ({obj.Length})- - - -\n{obj.ToHexString()}\n- - - - END DATA - - - -\n");
            ClientMessageInformation.InitBuild(obj);
        }

        private void Proxy_FakeClient_OnClientReceivedData(byte[] obj)
        {
            if (ConfigurationManager.Instance.Startup.show_data)
                Console.WriteLine($"- - - - DATA len : ({obj.Length})- - - -\n{obj.ToHexString()}\n- - - - END DATA - - - -\n");
            ServerMessageInformation.InitBuild(obj);
            // forward message from server
            Client.Send(obj);
        }
    }
}
