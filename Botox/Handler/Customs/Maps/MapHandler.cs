﻿using Botox.FastAction;
using Botox.FastAction.Models.Enums;
using Botox.Protocol;
using Botox.Protocol.JsonField;
using Botox.Proxy;
using BotoxSharedModel.Models.Actors;
using BotoxSharedModel.Models.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Botox.Handler.Customs.Maps
{
    public class MapHandler : IMessageHandler
    {
        [Handler(226)]
        public void HandleMapComplementaryInformationsDataMessage(ProxyElement proxy, NetworkElementField message, ProtocolJsonContent content)
        {
            MapModel map = new MapModel() { MapId = content["mapId"] };
            ProxyManager.Instance[proxy.ProcessId].CharacterSelected.MapId = map.MapId;

            foreach (ProtocolJsonContent actor in content["actors"])
            {
                ActorModel model = Parse(actor, content["mapId"]);
                if (model != null)
                {
                    map.Actors.Add(model);
                }
            }

            FastEventManager.Instance.Handle(FastEventEnum.PlayerSelectedEnterMap, map);
        }

        [Handler(5632)]
        public void HandleGameRolePlayShowActorMessage(ProxyElement proxy, NetworkElementField message, ProtocolJsonContent content)
        {
            double mapId = ProxyManager.Instance[proxy.ProcessId].CharacterSelected.MapId;
            ActorModel model = Parse(content["informations"], mapId);

            if(model != null)
            {
                FastEventManager.Instance.Handle(FastEventEnum.PlayerEnterMap, model);
            }
        }

        [Handler(251)]
        public void HandleGameContextRemoveElementMessage(ProxyElement proxy, NetworkElementField message, ProtocolJsonContent content)
        {
            double mapId = ProxyManager.Instance[proxy.ProcessId].CharacterSelected.MapId;
            FastEventManager.Instance.Handle(FastEventEnum.ElementRemovedMap, content["id"], mapId);
        }


        private ActorModel Parse(ProtocolJsonContent content, double mapId)
        {
            // GameRolePlayCharacterInformations or 
            // GameRolePlayMerchantInformations
            if (content["protocol_id"] == 36 || content["protocol_id"] == 129)
            {
                ProtocolJsonContent humanoidInfo = content["humanoidInfo"];
                ProtocolJsonContent alignmentInfos = content["alignmentInfos"];
                short level = (short)(alignmentInfos is null ? 0 : (alignmentInfos["characterPower"] - content["contextualId"]));
                
                return new PlayerModel()
                {
                    Id = content["contextualId"],
                    Level = level,
                    Name = content["name"],
                    IsMerchant = content["protocol_id"] == 129,
                    MapId = mapId
                };
            }

            return null;
        }
    }
}
