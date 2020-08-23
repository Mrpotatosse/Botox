﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Botox.Protocol.JsonField
{
    public class NetworkElementField
    {
        public ClassField[] fields { get; set; }
        public string name { get; set; }
        public int protocolID { get; set; }
        public string super { get; set; }
        public string super_serialize { get; set; }
        public string supernamespace { get; set; }
        public bool use_hash_function { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
        }
    }
}