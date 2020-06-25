using Botox.Extension;
using EasyHook;
using SocketHook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace Botox.Hook
{
    public class HookManager : Singleton<HookManager>
    {
        public void InitHook(string exePath, string hookDllPath, int port)
        {
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
        }
    }
}
