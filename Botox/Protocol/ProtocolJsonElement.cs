using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Botox.Protocol
{
    public class ProtocolJsonElement 
    {
        public string name { get; set; }
        public string parent { get; set; }
        public int protocolId { get; set; }
        public ProtocolJsonVar[] vars { get; set; }
        public ProtocolJsonVar[] boolVars { get; set; }
        public bool hashFunction { get; set; }

        public override string ToString()
        {
            return $"{name} ({protocolId})";
        }
    }
}
