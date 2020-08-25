﻿using Botox.Extension;
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
        public void SendChatMessage(ProxyElement proxy, string channelName, string content)
        {
            if(byte.TryParse(ProtocolManager.Instance.Protocol.enumerations.FirstOrDefault(x => x.name == "ChatActivableChannelsEnum")[channelName], out byte channel))
            {
                SendChatMessage(proxy, channel, content);
            }
            else
            {
                Console.WriteLine("Send chat error");
            }
        }

        public void SendChatMessage(ProxyElement proxy, byte channel, string content)
        {
            proxy.SendServer(861, new ProtocolJsonContent()
            {
                fields =
                {
                    { "channel", channel },
                    { "content", content }
                }
            });
        }
    }
}
