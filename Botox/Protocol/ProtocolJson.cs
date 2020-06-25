using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Botox.Protocol
{
    public class ProtocolJson
    {
        public ProtocolJsonElement ProtocolRequired { get; set; }// 1

        public ProtocolJsonElement StatisticData { get; set; }
        public ProtocolJsonElement StatisticDataBoolean { get; set; }

        public ProtocolJsonElement CharacterCreationRequestMessage { get; set; }
        public ProtocolJsonElement CharactersListWithRemodelingMessage { get; set; }

        public override string ToString()
        {
            return $"protocol elements count found : {GetType().GetProperties().Length}";
        }
    }
}
