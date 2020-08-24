using Botox.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Botox.Extension
{
    public static class HandlerExtension
    {
        public static IEnumerable<MethodInfo> GetHandler(this IMessageHandler handler)
        {
            return handler.GetType().GetMethods().Where(x => x.GetCustomAttribute<HandlerAttribute>() is HandlerAttribute);
        }
    }
}
