using Botox.Protocol.JsonField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Botox.Protocol
{
    public class BotofuProtocolJson
    {
        public EnumerationField[] enumerations { get; set; }
        public NetworkElementField[] messages { get; set; }
        public NetworkElementField[] types { get; set; }
        
        public NetworkElementField this[ProtocolKeyEnum key, Func<NetworkElementField, bool> predicat]
        {
            get
            {
                if((key & ProtocolKeyEnum.Messages) == ProtocolKeyEnum.Messages && messages.FirstOrDefault(predicat) is NetworkElementField message)                
                    return message;                

                if ((key & ProtocolKeyEnum.Types) == ProtocolKeyEnum.Types && types.FirstOrDefault(predicat) is NetworkElementField type)
                    return type;

                return null;
            }
        }

        public EnumerationField this[Func<EnumerationField, bool> predicat]
        {
            get
            {
                return enumerations.FirstOrDefault(predicat);
            }
        }
    }
}
