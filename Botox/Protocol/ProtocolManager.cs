using Botox.Extension;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static BotoxNetwork.IO.DofusIO;

namespace Botox.Protocol
{
    public class ProtocolManager : Singleton<ProtocolManager>
    {        
        public readonly ProtocolJsonElement[] Protocol;

        public ProtocolManager()
        {
            JObject json = Newtonsoft.Json.JsonConvert.DeserializeObject<JObject>(File.ReadAllText("./dofusprotocol.json"), new Newtonsoft.Json.JsonSerializerSettings() { Formatting = Newtonsoft.Json.Formatting.Indented });
            Protocol = json.Children().Select(x => Newtonsoft.Json.JsonConvert.DeserializeObject<ProtocolJsonElement>(x.First.ToString())).ToArray();
        }

        public ProtocolJsonElement Get(int id, bool message = true)
        {
            IEnumerable<ProtocolJsonElement> id_match = Protocol.Where(x => x.protocolId == id);
            if(id_match.Count() > 1)
            {
                if (message) return id_match.FirstOrDefault(x => x.name.Replace("Message", "") != x.name);
                else return id_match.FirstOrDefault(x => x.name.Replace("Message", "") == x.name);
            }
            return id_match.FirstOrDefault();
        }
    }
}
