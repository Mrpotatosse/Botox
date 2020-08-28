using Botox.FastAction;
using Botox.FastAction.Models.Enums;
using Botox.Protocol;
using Botox.Protocol.JsonField;
using Botox.Proxy;
using BotoxSharedModel.Models.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Botox.Handler.Customs.Characters
{
    public class SelectionHandler : IMessageHandler
    {
        [Handler(153)]
        public void HandleCharacterSelectedSuccessMessage(ProxyElement proxy, NetworkElementField message, ProtocolJsonContent content)
        {
            PlayerModel model = new PlayerModel()
            {
                Id = content["infos"]["id"],
                Name = content["infos"]["name"],
                Level = content["infos"]["level"],
                IsMerchant = false
            };

            ProxyManager.Instance[proxy.ProcessId].CharacterSelected = model;

            FastEventManager.Instance.Handle(FastEventEnum.PlayerSelected, model);
        }
    }
}
