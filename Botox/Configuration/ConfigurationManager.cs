using Botox.Extension;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Botox.Configuration
{
    public class ConfigurationManager : Singleton<ConfigurationManager>
    {        
        public StartupConfiguration Startup
        {
            get
            {
                return GetConfig<StartupConfiguration>();
            }
        }

        public T GetConfig<T>() where T : class, IConfigurationFile
        {
            T obj = Activator.CreateInstance<T>();
            string location = typeof(T).GetProperty("LOCATION").GetValue(obj).ToString();

            try
            {
                string content = File.ReadAllText(location);
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(content, new Newtonsoft.Json.JsonSerializerSettings() { Formatting = Newtonsoft.Json.Formatting.Indented });
            }
            catch(FileNotFoundException)
            {
                string content = Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(location, content);

                return GetConfig<T>();
            }
            catch
            {
                return null;
            }                             
        }

        public void SaveConfig<T>(T obj) where T : class, IConfigurationFile
        {
            string content = Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(obj.LOCATION, content);
        }
    }
}
