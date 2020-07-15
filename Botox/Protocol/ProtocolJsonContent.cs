using Botox.Protocol.JsonField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Botox.Protocol
{
    public class ProtocolJsonContent
    {
        public Dictionary<string, dynamic> fields { get; set; } = new Dictionary<string, dynamic>();

        public dynamic this[string key]
        {
            get
            {
                if(fields.ContainsKey(key))
                    return fields[key];
                return null;
            }
            set
            {
                if (fields.ContainsKey(key))
                {
                    fields[key] = value;
                }
                else
                {
                    fields.Add(key, value);
                }
            }
        }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
        }
    }
}
