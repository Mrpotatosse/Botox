using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Botox.Protocol
{
    public class ProtocolJsonVar
    {
        public string name { get; set; }
        public string length { get; set; }
        public string type { get; set; }
        public bool optional { get; set; }
    }
}
