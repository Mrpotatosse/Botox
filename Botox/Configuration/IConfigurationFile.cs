using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Botox.Configuration
{
    public interface IConfigurationFile
    {
        [JsonIgnore]
        string LOCATION { get; }
    }
}
