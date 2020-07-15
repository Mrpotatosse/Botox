using Botox.Extension;
using Botox.Protocol.JsonField;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static BotoxNetwork.IO.DofusIO;

namespace Botox.Protocol
{
    public class ProtocolManager : Singleton<ProtocolManager>
    {        
        public static readonly string JSON_PROTOCOL_LOCATION = "./updatedProtocol.json";
        public static readonly string JSON_PROTOCOL_URL = "https://cldine.gitlab.io/-/protocol-autoparser/-/jobs/691246963/artifacts/protocol.json";
        
        public readonly BotofuProtocolJson Protocol;

        private bool _isUpdated;
        public void UpdateProtocol()
        {
            if (_isUpdated) return;

            try
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(JSON_PROTOCOL_URL, JSON_PROTOCOL_LOCATION);
                }

                _isUpdated = true;
            }
            catch(Exception e)
            {
                Console.WriteLine($"{e}");
            }
        }

        private string JsonContent
        {
            get
            {
                return File.ReadAllText(JSON_PROTOCOL_LOCATION);
            }
        }

        public ProtocolManager()
        {
            if (!File.Exists(JSON_PROTOCOL_LOCATION))
            {
                UpdateProtocol();
            }

            Protocol = Newtonsoft.Json.JsonConvert.DeserializeObject<BotofuProtocolJson>(JsonContent, new Newtonsoft.Json.JsonSerializerSettings() { Formatting = Newtonsoft.Json.Formatting.Indented });
        }

        public NetworkElementField GetNetwork(Func<NetworkElementField, bool> predicat, bool message = true)
        {
            if (message)
                return Protocol.messages.FirstOrDefault(predicat);
            return Protocol.types.FirstOrDefault(predicat);
        }

        public EnumerationField GetEnum(string name)
        {
            return Protocol.enumerations.FirstOrDefault(x => x.name == name);
        }
    }
}
