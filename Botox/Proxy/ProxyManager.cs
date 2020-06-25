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
        private IList<CustomProxy> Proxys { get; set; } = new List<CustomProxy>();
        public int Port
        {
            get
            {
                if(Proxys.LastOrDefault() is CustomProxy proxy)
                {
                    return proxy.Port + 1;
                }
                return 666;
            }
        }

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
