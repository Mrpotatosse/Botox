using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels.Ipc;
using System.Text;
using System.Threading.Tasks;

namespace SocketHook
{
    public struct HookElement 
    {
        public int ProcessId;
        public string ChannelName;
        public IpcServerChannel IpcServer;
    }
}
