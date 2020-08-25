using Botox.Extension;
using Botox.Protocol;
using Botox.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Botox.FastAction
{
    public class FastActionManager : Singleton<FastActionManager>
    {
        public void SendChatMessage(CustomClient server, string channelName, string content)
        {
            if(byte.TryParse(ProtocolManager.Instance.Protocol.enumerations.FirstOrDefault(x => x.name == "ChatActivableChannelsEnum")[channelName], out byte channel))
            {
                SendChatMessage(server, channel, content);
            }
            else
            {
                Console.WriteLine("Send chat error");
            }
        }

        public void SendChatMessage(CustomClient server, byte channel, string content)
        {
            server.Send(861, new ProtocolJsonContent()
            {
                fields =
                {
                    { "channel", channel },
                    { "content", content }
                }
            }, true);
        }
    }
}
