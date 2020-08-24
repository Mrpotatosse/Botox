using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Botox.Configuration
{
    public class StartupConfiguration : IConfigurationFile
    {
        public string LOCATION => "./startup.json";

        public string dofus_location { get; set; }
        public string dll_location { get; set; }

        public int client_count { get; set; }

        public bool show_message { get; set; }
        public bool show_message_content { get; set; }

        public StartupConfiguration()
        {
            dofus_location = "D:/DofusApp/Dofus.exe";
            dll_location = "./SocketHook.dll";

            client_count = 1;

            show_message = false;
            show_message_content = false;
        }
    }
}
