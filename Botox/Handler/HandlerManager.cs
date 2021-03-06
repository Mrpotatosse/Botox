﻿using Botox.Extension;
using Botox.Protocol;
using Botox.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Botox.Handler
{
    public class HandlerManager : Singleton<HandlerManager>
    {
        private IEnumerable<MethodInfo> _handlers { get; set; }

        public HandlerManager()
        {
            IEnumerable<Type> types = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.GetInterface("IMessageHandler") is Type);
            _handlers = types.Select(x => Activator.CreateInstance(x) as IMessageHandler).SelectMany(x => x.GetHandler());
        }

        public int Check
        {
            get
            {
                return _handlers.Count();
            }
        }

        public void Handle(uint protocolId, ProxyElement proxy, ProtocolJsonContent content)
        {
            MethodInfo method = _handlers.FirstOrDefault(x => x.GetCustomAttribute<HandlerAttribute>().ProtocolId == protocolId);
            Handle(method, proxy, content);
        }

        public void Handle(string protocolName, ProxyElement proxy, ProtocolJsonContent content)
        {
            MethodInfo method = _handlers.FirstOrDefault(x => x.GetCustomAttribute<HandlerAttribute>().ProtocolName == protocolName);
            Handle(method, proxy, content);
        }

        private void Handle(MethodInfo method, ProxyElement proxy, ProtocolJsonContent content)
        {
            if (method is null) return;
            object obj = Activator.CreateInstance(method.DeclaringType);
            method.Invoke(obj, new object[] { proxy, method.GetCustomAttribute<HandlerAttribute>().Message, content });
        }
    }
}
