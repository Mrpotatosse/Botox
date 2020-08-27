using Botox.FastAction;
using Botox.FastAction.Models.Actors;
using Botox.Protocol;
using Botox.Protocol.JsonField;
using Botox.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Botox.Handler.Customs
{
    public class CustomHandler : IMessageHandler
    {
        [Handler(1)]
        public void HandleProtocolRequiredMessage(ProxyElement proxy, NetworkElementField message, ProtocolJsonContent content)
        {
            Console.WriteLine($"Test : {content["requiredVersion"]}");
        }

        [Handler(153)]
        public void HandleCharacterSelectedSuccessMessage(ProxyElement proxy, NetworkElementField message, ProtocolJsonContent content)
        {
            ProxyManager.Instance[proxy.ProcessId].CharacterSelected = new PlayerModel()
            {
                Id = content["infos"]["id"],
                Name = content["infos"]["name"],
                Level = content["infos"]["level"]
            };

            Console.WriteLine($"{ProxyManager.Instance[proxy.ProcessId].CharacterSelected}");
        }

        [Handler(226)]
        public void HandleMapComplementaryInformationsDataMessage(ProxyElement proxy, NetworkElementField message, ProtocolJsonContent content)
        {
            ProxyManager.Instance[proxy.ProcessId].CharacterSelected.MapId = content["mapId"];

            /*Task.Delay(TimeSpan.FromSeconds(15)).ContinueWith(task =>
            {
                PlayerModel model = ProxyManager.Instance[proxy.ProcessId].CharacterSelected;
                if (model.MapId == content["mapId"])
                {
                    string rnd_str = "abcdefghijklmnopqrstuvwxyz";
                    string rnd_str_up = rnd_str.ToUpper();
                    string rnd_str_num = "0123456789";
                    string rnd = "";

                    Random rnd_c = new Random();

                    int len = rnd_c.Next(4, 6);
                    
                    for(int i = 0; i < len; i++)
                    {
                        byte r = (byte)rnd_c.Next(0, 3);
                        if(r == 0)
                        {
                            rnd += rnd_str[rnd_c.Next(0, rnd_str.Length)];
                        }
                        if (r == 1)
                        {
                            rnd += rnd_str_up[rnd_c.Next(0, rnd_str_up.Length)];
                        }
                        if (r == 2)
                        {
                            rnd += rnd_str_num[rnd_c.Next(0, rnd_str_num.Length)];
                        }
                    }

                    FastActionManager.Instance.SendChatMessage(proxy, "CHANNEL_GLOBAL", $"Hello World ! Je suis {model.Name} de niveau {model.Level} ! {rnd}");
                }
            });*/
        }

        [Handler(5523)]
        public void HandleExchangeRequestedTradeMessage(ProxyElement proxy, NetworkElementField message, ProtocolJsonContent content)
        {
            int type = content["exchangeType"];
            long targetId = content["target"];
            long sourceId = content["source"];
            
            if(targetId == ProxyManager.Instance[proxy.ProcessId].CharacterSelected.Id)
            {
                FastActionManager.Instance.SendAcceptExchange(proxy);
            }
        }
    }
}
