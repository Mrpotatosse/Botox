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

        private bool _update { get; set; } = false;
        public bool UpdateProtocol(bool force = false)
        {
            if (_update && !force) return true;

            try
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(JSON_PROTOCOL_URL, JSON_PROTOCOL_LOCATION);
                }

                Console.WriteLine($"Protocol is up-to-date");
                _update = true;
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine($"{e}");
                return false;
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
            _ctorInit(ref Protocol);
        }

        private void _ctorInit(ref BotofuProtocolJson protocol)
        {
            if (!UpdateProtocol() && !File.Exists(JSON_PROTOCOL_LOCATION))
            {
                Console.WriteLine("protocol.json is missing and cannot be downloaded : frère paye ta co fait un effort xD");
                Console.ReadKey();
                Environment.Exit(0);
            }

            protocol = Newtonsoft.Json.JsonConvert.DeserializeObject<BotofuProtocolJson>(JsonContent, new Newtonsoft.Json.JsonSerializerSettings() { Formatting = Newtonsoft.Json.Formatting.Indented });
        }

        public List<NetworkElementField> GetSuper(NetworkElementField field)
        {
            List<NetworkElementField> result = new List<NetworkElementField>();

            if (field.super != "NetworkMessage")
            {
                result.AddRange(GetSuper(Protocol[ProtocolKeyEnum.Messages, x => x.name == field.super]));
            }

            return result;
        }
    }
}
