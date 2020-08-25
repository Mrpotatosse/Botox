using Botox.Protocol;
using Botox.Protocol.JsonField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Botox.Handler
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class HandlerAttribute : Attribute
    {
        public int ProtocolId { get; set; } = -1;
        public string ProtocolName { get; set; } = null;

        public NetworkElementField Message { get; private set; } 

        public HandlerAttribute(uint protocolId)
        {
            ProtocolId = (int)protocolId;
            Init();
        }

        public HandlerAttribute(string protocolName)
        {
            ProtocolName = protocolName;
            Init();
        }

        private void Init()
        {
            if(ProtocolId is -1)
            {
                Message = ProtocolManager.Instance.Protocol[ProtocolKeyEnum.Messages, x => x.name == ProtocolName];
                ProtocolId = Message?.protocolID ?? -1;
            }
            else if(ProtocolName is null)
            {
                Message = ProtocolManager.Instance.Protocol[ProtocolKeyEnum.Messages, x => x.protocolID == ProtocolId];
                ProtocolName = Message?.name ?? "NO_MESSAGE_FOUND";
            }

            if(Message is null)
            {
                Console.WriteLine($"Cannot init Handler attribute error on : Message[id:{ProtocolId} name:{ProtocolName}]");
            }
        }
    }
}
