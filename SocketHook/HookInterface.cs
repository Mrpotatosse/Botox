using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SocketHook
{
    public class HookInterface : MarshalByRefObject
    {
        public virtual void NotifyInstalled(string processName) => Console.WriteLine($"Injected on process : {processName}");
        public virtual void Message(string message) => Console.WriteLine($"{message}");
        public virtual void Error(Exception exception) => Console.WriteLine($"{exception}");
        public virtual void IpRedirected(IPEndPoint ip, int processId, int redirectionPort) => Console.WriteLine($"Ip redirected from '{ip}' to '127.0.0.1:{redirectionPort}' (processId:'{processId}')");

        public virtual void Ping() => Console.WriteLine("Socket Hook loaded");
    }
}
