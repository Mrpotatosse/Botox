using Botox.Configuration;
using Botox.Handler;
using Botox.Hook;
using Botox.Protocol;
using Botox.Proxy;
using EasyHook;
using SocketHook;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace Botox
{
    public class Program
    {        
        public static void Main(string[] args)
        {
            ProtocolManager.Instance.UpdateProtocol();
            Console.WriteLine($"Handler found : {HandlerManager.Instance.Check}");
                        
            try
            {
                // test link https://louisabraham.github.io/LaBot/decoder.html?hex=63920000000E0102800221CBD546139F218BC99C472E61FF56BBBB40119B600968659E7477A472B2975E96DA6CFF23E75AEB4E1FFE8114E8517BDBF745F576AC67A7CD376F4D9FB0B72D66E06DC34D607A38C9E51FBBF1AB5E52D9488994FE3838E844B4083D99DA23A34BF9DAD713688F1F36B64DBC0AF8E9C05B71DDAFDBC06B840D3327FFC237AB5C35F0FB5AE5DD28F8F1FA1DFC6EBC29E81C823FEDB04E99E3CD7E0754A74ED4DFC1F3A301D2BC55BFECB26EC3488B0313D03825388313E8306975D3FD9D9F369146F3C2949695CFEDA30FAD86ECE0BD0FA5421D7F704A571AFD085FA3ED693DCA90D15BDD5217FBFE533DA9DE85CFB09688A4500424E8B8D40CBC5E44FFD5D46E&from_client=true
                
                HookManager.Instance.InitHook(ConfigurationManager.Instance.Startup.dofus_location, 
                                              ConfigurationManager.Instance.Startup.dll_location,
                                              ConfigurationManager.Instance.Startup.client_count);
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e}");
            }

            Console.ReadLine();
        }
    }
}
