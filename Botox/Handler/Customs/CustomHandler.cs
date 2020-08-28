using Botox.FastAction;
using Botox.Protocol;
using Botox.Protocol.JsonField;
using Botox.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Botox.Handler.Customs
{
    public class CustomHandler : IMessageHandler
    {
        [Handler(1)]
        public void HandleProtocolRequiredMessage(ProxyElement proxy, NetworkElementField message, ProtocolJsonContent content)
        {
            Console.WriteLine($"Test : {content["requiredVersion"]}");
        }

        [Handler(5523)]
        public void HandleExchangeRequestedTradeMessage(ProxyElement proxy, NetworkElementField message, ProtocolJsonContent content)
        {
            int type = content["exchangeType"];
            long targetId = content["target"];
            long sourceId = content["source"];

            FastActionManager.Instance.SendLeaveDialog(proxy);
        }
    }
}
