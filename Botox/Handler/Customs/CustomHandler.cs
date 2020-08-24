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
        public void HandleProtocolRequiredMessage(CustomClient client, NetworkElementField message, ProtocolJsonContent content)
        {
            Console.WriteLine($"Test : {content["requiredVersion"]}");
        }

        [Handler(153)]
        public void HandleCharacterSelectedSuccessMessage(CustomClient client, NetworkElementField message, ProtocolJsonContent content)
        {
            Console.WriteLine($"Character selected : {content["infos"]["name"]} {content["infos"]["level"]}");
        }
    }
}
