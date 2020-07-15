using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Botox.Protocol.JsonField
{
    public class EnumerationField
    {
        public string entries_type { get; set; }
        public JObject members { get; set; }
        public string name { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// flemme de ouf de faire une class pour members ducoup on va se contenter de ce truc assez stylé x)
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string this[string key]
        {
            get
            {
                JToken current = members.First;
                while(current.ToObject<JProperty>() is JProperty prop && prop.Name != key)
                {
                    if (current.Next is null) throw new KeyNotFoundException();
                    current = current.Next;
                }

                return current.First.ToString();
            }
        }
    }
}
