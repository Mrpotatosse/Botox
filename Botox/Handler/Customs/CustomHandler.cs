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

        [Handler(153)]
        public void HandleCharacterSelectedSuccessMessage(ProxyElement proxy, NetworkElementField message, ProtocolJsonContent content)
        {
            Console.WriteLine($"Character selected : {content["infos"]["name"]} {content["infos"]["level"]}");

        }

        [Handler(226)]
        public void HandleMapComplementaryInformationsDataMessage(ProxyElement proxy, NetworkElementField message, ProtocolJsonContent content)
        {
            Task.Delay(TimeSpan.FromSeconds(15)).ContinueWith(task =>
            {
                FastActionManager.Instance.SendChatMessage(proxy, "CHANNEL_GLOBAL", "Hello World !");
            });
        }
    }
}
