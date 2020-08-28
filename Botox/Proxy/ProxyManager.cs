using Botox.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Botox.Proxy
{
    public class ProxyManager : Singleton<ProxyManager>
    {
        public CustomProxy this[int processId]
        {
            get
            {
                return Proxys.FirstOrDefault(x => x.ProcessId == processId);
            }
        }

        public int this[Func<CustomProxy, bool> predicat]
        {
            get
            {
                return Proxys.FirstOrDefault(predicat).ProcessId;
            }
        }

        public int this[string characterName]
        {
            get
            {
                return Proxys.FirstOrDefault(x => x.CharacterSelected.Name == characterName).ProcessId;
            }
        }

        private IList<CustomProxy> Proxys { get; set; } = new List<CustomProxy>();
        
        public void Redirect(IPEndPoint ip, int processId, int redirectionPort)
        {
            if(Proxys.FirstOrDefault(x => x.ProcessId == processId) is CustomProxy proxy)
            {
                proxy.AddClient(ip);
            }
            else
            {
                CustomProxy _proxy = new CustomProxy(redirectionPort, processId);
                _proxy.AddClient(ip);
                Proxys.Add(_proxy);
                _proxy.Start();
            }
        }
    }
}
