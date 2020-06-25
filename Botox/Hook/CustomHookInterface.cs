using Botox.Proxy;
using SocketHook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Botox.Hook
{
    public class CustomHookInterface : HookInterface
    {
        public override void IpRedirected(IPEndPoint ip, int processId, int redirectionPort)
        {
            ProxyManager.Instance.Redirect(ip, processId, redirectionPort);
        }
    }
}
