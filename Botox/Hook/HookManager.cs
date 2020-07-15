using Botox.Extension;
using EasyHook;
using SocketHook;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace Botox.Hook
{
    public class HookManager : Singleton<HookManager>
    {
        private Dictionary<int, int> PortUsedByProcess { get; set; } = new Dictionary<int, int>();

        private bool IsPortUsed(int port)
        {
            if (PortUsedByProcess.ContainsValue(port))
                return true;

            var global = IPGlobalProperties.GetIPGlobalProperties();
            var tcp = global.GetActiveTcpListeners();

            return tcp.FirstOrDefault(x => x.Port == port) != null;
        }

        public int Port
        {
            get
            {
                int port = 666;                
                while (IsPortUsed(port))
                {
                    port = Math.Max(666, (port + 1) % short.MaxValue);
                }
                return port;
            }
        }

        public void InitHook(string exePath, string hookDllPath)
        {
            int port = Port;

            HookElement element = new HookElement();
            {
                element.IpcServer = RemoteHooking.IpcCreateServer<CustomHookInterface>(ref element.ChannelName, WellKnownObjectMode.Singleton);
            }

            RemoteHooking.CreateAndInject(
                exePath,
                string.Empty,
                0x00000004,
                InjectionOptions.DoNotRequireStrongName,
                hookDllPath,
                hookDllPath,
                out element.ProcessId,
                element.ChannelName,
                port);
                       
            PortUsedByProcess.Add(element.ProcessId, port);
            Process process = Process.GetProcessById(element.ProcessId);

            process.EnableRaisingEvents = true;

            process.Exited += (obj, arg) =>
            {
                PortUsedByProcess.Remove(process.Id);
            };
        }
    }
}
