using Botox.Extension;
using Botox.Protocol.JsonField;
using Botox.Proxy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BotoxNetwork.IO.DofusIO;

namespace Botox.Protocol
{
    public class MessageInformation
    {
        private readonly bool ClientSide;
        private BigEndianReader Reader { get; set; }
        public Buffer Information { get; set; }

        public event Action<NetworkElementField, ProtocolJsonContent> OnMessageParsed;
        
        public MessageInformation(bool clientSide)
        {
            ClientSide = clientSide;
            Reader = new BigEndianReader();
            Information = new Buffer();
        }

        public void InitBuild(byte[] data)
        {
            if (data.Length > 0)
                Reader.Add(data, 0, data.Length);

            if (Information.Build(Reader, ClientSide))
            {
                if (NetworkBase != null)
                    OnMessageParsed?.Invoke(NetworkBase, Content);

                Information = null;
                Information = new Buffer();

                byte[] r = Reader.ReadBytes((int)Reader.BytesAvailable);

                InitBuild(r);
            }
        }      

        public NetworkElementField NetworkBase
        {
            get
            {
                return ProtocolManager.Instance.GetNetwork(x => x.protocolID == Information.MessageId);
            }
        }

        private ProtocolJsonContent Content
        {
            get
            {
                return NetworkBase.Parse(new BigEndianReader(Information.Data));
            }
        }
    }
}
