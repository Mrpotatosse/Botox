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
        // readonly property
        private readonly bool ClientSide;
        private BigEndianWriter Buffer { get; set; }
        public byte[] FullData
        {
            get
            {
                return Buffer.Data;
            }
        }

        // public property
        public ushort? Header { get; set; } = null;
        public uint? InstanceId { get; set; } = null;
        public uint Length { get; set; } = 0;
        public ushort? ProtocolId
        {
            get
            {
                if (Header is null) return null;
                return (ushort)(Header >> 2);
            }
        }
        public ushort? DynamicHeader
        {
            get
            {
                if (Header is null) return null;
                return (ushort)(Header & 3);
            }
        }

        private int Offset { get; set; }
        public byte[] MessageData { get; private set; }
        public bool Parsed => MessageData?.Length  >= Length && MessageJson != null;
        public ProtocolJsonElement MessageJson
        {
            get
            {
                if (ProtocolId is null) return null;
                return ProtocolManager.Instance.Get(ProtocolId.Value);
            }
        }
        
        public MessageInformation(bool clientSide)
        {
            Offset = 0;
            Buffer = new BigEndianWriter();
            ClientSide = clientSide;
        }

        public void Build(byte[] data)
        {
            Buffer.WriteBytes(data);

            using (BigEndianReader reader = new BigEndianReader(FullData))
            {
                ReadHeader(reader);

                if (Length <= reader.BytesAvailable)
                {
                    MessageData = reader.ReadBytes((int)Length);
                }
                else
                {
                    if(Length > 99999)
                        Clear();
                }
            }
        }

        private void ReadMessageLength(BigEndianReader reader)
        {
            switch (DynamicHeader)
            {
                case 1:
                    Length = (uint)reader.ReadUnsignedByte(); return;
                case 2:
                    Length = reader.ReadUnsignedShort(); return;
                case 3:
                    Length = (uint)(((reader.ReadByte() & 255) << 16) + ((reader.ReadByte() & 255) << 8) + (reader.ReadByte() & 255)); return;
                default: return;
            }
        }

        private void ReadHeader(BigEndianReader reader)
        {
            if (reader.BytesAvailable >= 2)
            {
                Header = reader.ReadUnsignedShort();

                if (reader.BytesAvailable >= DynamicHeader)
                {
                    if (ClientSide)
                    {
                        InstanceId = reader.ReadUnsignedInt();
                    }

                    ReadMessageLength(reader);
                }
            }
        }

        public void Clear()
        {
            Header = null;
            Length = 0;
            Offset = 0;
            MessageData = new byte[0];
            Buffer.Dispose();
            Buffer.Clear();
        }   
    }
}
