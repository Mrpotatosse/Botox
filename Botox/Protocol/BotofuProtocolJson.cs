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
    }
}
