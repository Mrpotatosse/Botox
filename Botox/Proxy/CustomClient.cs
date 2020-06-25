using BotoxNetwork.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

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
    }
}
