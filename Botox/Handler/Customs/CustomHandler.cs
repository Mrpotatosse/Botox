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
        public void HandleProtocolRequiredMessage(CustomClient dofusClient, CustomClient server, NetworkElementField message, ProtocolJsonContent content)
        {
            Console.WriteLine($"Test : {content["requiredVersion"]}");
        }

        [Handler(153)]
        public void HandleCharacterSelectedSuccessMessage(CustomClient dofusClient, CustomClient server, NetworkElementField message, ProtocolJsonContent content)
        {
            Console.WriteLine($"Character selected : {content["infos"]["name"]} {content["infos"]["level"]}");
        }

        [Handler(226)]
        public void HandleMapComplementaryInformationsDataMessage(CustomClient dofusClient, CustomClient server, NetworkElementField message, ProtocolJsonContent content)
        {
            Task.Delay(TimeSpan.FromSeconds(10)).ContinueWith(task =>
            {
                FastActionManager.Instance.SendChatMessage(server, "CHANNEL_GLOBAL", "Hello World !");
            });
        }
    }
}
