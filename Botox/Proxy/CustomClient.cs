using Botox.Configuration;
using Botox.Extension;
using Botox.Protocol;
using Botox.Protocol.JsonField;
using BotoxNetwork.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static BotoxNetwork.IO.DofusIO;

namespace Botox.Proxy
{
    public class CustomClient : BaseClient
    {
        public CustomClient() : base()
        {

        }

        public CustomClient(Socket socket) : base(socket)
        {

        }

        public void InitEvent()
        {
            OnClientReceivedData += CustomClient_OnClientReceivedData;
            OnClientSentData += CustomClient_OnClientSentData;
        }

        private void CustomClient_OnClientSentData(byte[] obj)
        {

        }

        private void CustomClient_OnClientReceivedData(byte[] obj)
        {

        }

        public void Send(int processId, string protocolName, ProtocolJsonContent content, bool clientSide = true)
        {
            NetworkElementField message = ProtocolManager.Instance.Protocol[ProtocolKeyEnum.Messages, x => x.name == protocolName];
            Send(processId, message, content, clientSide);
        }

        public void Send(int processId, int protocolId, ProtocolJsonContent content, bool clientSide = true)
        {
            NetworkElementField message = ProtocolManager.Instance.Protocol[ProtocolKeyEnum.Messages, x => x.protocolID == protocolId];
            Send(processId, message, content, clientSide);
        }

        public void Send(int processId, NetworkElementField message, ProtocolJsonContent content, bool clientSide)
        {
            if (message is null) return;

            using(BigEndianWriter writer = new BigEndianWriter())
            {
                byte[] data = message.ToByte(content);

                int cmpLen = _cmpLen(data.Length);
                writer.WriteShort((short)((message.protocolID << 2) | cmpLen));
                ProxyManager.Instance[processId].FAKE_MESSAGE_SENT++;
                if (clientSide)
                {
                    writer.WriteUnsignedInt(ProxyManager.Instance[processId].FAKE_MSG_INSTANCE_ID);
                }
                switch (cmpLen)
                {
                    case 0:
                        break;
                    case 1:
                        writer.WriteByte((byte)data.Length);
                        break;
                    case 2:
                        writer.WriteShort((short)data.Length);
                        break;
                    case 3:
                        writer.WriteByte((byte)((data.Length >> 16) & 255));
                        writer.WriteShort((short)(data.Length & 65535));
                        break;
                }

                writer.WriteBytes(data);
                Send(writer.Data);
                if (ConfigurationManager.Instance.Startup.show_fake_message_sent)
                {
                    Console.WriteLine($"Fake Message sent to ({RemoteIP}) : (n°{ProxyManager.Instance[processId].FAKE_MSG_INSTANCE_ID}) [{message.name} ({message.protocolID})]");
                    if (ConfigurationManager.Instance.Startup.show_message_content)
                    {
                        Console.WriteLine($"{content}");
                    }
                }
            }
        }

        private int _cmpLen(int length)
        {
            if (length > 65535) return 3;
            if (length > 255) return 2;
            if (length > 0) return 1;
            return 0;
        }
    }
}
