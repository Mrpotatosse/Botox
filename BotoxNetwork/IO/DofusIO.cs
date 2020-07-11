using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BotoxNetwork.IO
{
    public class DofusIO
    {
        public class BigEndianReader : IDisposable
        {
            public const int INT_SIZE = 32;
            public const int SHORT_SIZE = 16;
            public const int SHORT_MIN_VALUE = -0x8000;
            public const int SHORT_MAX_VALUE = 0x7FFF;
            public const int USHORT_MAX_VALUE = 0x10000;
            public const int CHUNCK_BIT_SIZE = 7;
            public static readonly int MAX_ENCODING_LENGHT = (int)Math.Ceiling((double)INT_SIZE / CHUNCK_BIT_SIZE);
            public const int MASK_10000000 = 0x80;
            public const int MASK_01111111 = 0x7F;

            #region Properties

            private BinaryReader m_reader;

            /// <summary>
            ///   Gets availiable bytes number in the buffer
            /// </summary>
            public long BytesAvailable
            {
                get { return m_reader.BaseStream.Length - m_reader.BaseStream.Position; }
            }

            public long Position
            {
                get
                {
                    return m_reader.BaseStream.Position;
                }
            }


            public Stream BaseStream
            {
                get
                {
                    return m_reader.BaseStream;
                }
            }

            public byte[] Data
            {
                get
                {
                    var pos = BaseStream.Position;

                    var data = new byte[BaseStream.Length];
                    BaseStream.Position = 0;
                    BaseStream.Read(data, 0, (int)BaseStream.Length);

                    BaseStream.Position = pos;

                    return data;
                }
            }

            #endregion

            #region Initialisation

            /// <summary>
            ///   Initializes a new instance of the <see cref = "BigEndianReader" /> class.
            /// </summary>
            public BigEndianReader()
            {
                m_reader = new BinaryReader(new MemoryStream(), Encoding.UTF8);
            }

            /// <summary>
            ///   Initializes a new instance of the <see cref = "BigEndianReader" /> class.
            /// </summary>
            /// <param name = "stream">The stream.</param>
            public BigEndianReader(Stream stream)
            {
                m_reader = new BinaryReader(stream, Encoding.UTF8);
            }

            /// <summary>
            ///   Initializes a new instance of the <see cref = "BigEndianReader" /> class.
            /// </summary>
            /// <param name = "tab">Memory buffer.</param>
            public BigEndianReader(byte[] tab)
            {
                m_reader = new BinaryReader(new MemoryStream(tab), Encoding.UTF8);
            }

            #endregion

            #region Private Methods

            /// <summary>
            ///   Read bytes in big endian format
            /// </summary>
            /// <param name = "count"></param>
            /// <returns></returns>
            private byte[] ReadBigEndianBytes(int count)
            {
                var bytes = new byte[count];
                int i;
                for (i = count - 1; i >= 0; i--)
                    bytes[i] = (byte)BaseStream.ReadByte();
                return bytes;
            }

            #endregion

            #region Public Method

            public int ReadVarInt()
            {
                int value = 0;
                int size = 0;
                while (size < INT_SIZE)
                {
                    var b = ReadByte();
                    bool bit = (b & MASK_10000000) == MASK_10000000;
                    if (size > 0)
                        value |= ((b & MASK_01111111) << size);
                    else
                        value |= (b & MASK_01111111);
                    size += CHUNCK_BIT_SIZE;
                    if (!bit)
                        return value;
                }

                throw new Exception("Overflow varint : too much data");
            }

            public uint ReadVarUInt()
            {
                return unchecked((uint)ReadVarInt());
            }

            public short ReadVarShort()
            {
                int value = 0;
                int size = 0;
                while (size < SHORT_SIZE)
                {
                    var b = ReadByte();
                    bool bit = (b & MASK_10000000) == MASK_10000000;
                    if (size > 0)
                        value |= ((b & MASK_01111111) << size);
                    else
                        value |= (b & MASK_01111111);
                    size += CHUNCK_BIT_SIZE;
                    if (!bit)
                    {
                        if (value > SHORT_MAX_VALUE)
                            value = value - USHORT_MAX_VALUE;

                        return (short)value;
                    }
                }

                throw new Exception("Overflow varint : too much data");
            }

            public ushort ReadVarUShort()
            {
                return unchecked((ushort)ReadVarShort());
            }

            public long ReadVarLong()
            {
                int low = 0;
                int high = 0;
                int size = 0;
                byte lastByte = 0;
                while (size < 28)
                {
                    lastByte = m_reader.ReadByte();
                    if ((lastByte & MASK_10000000) == MASK_10000000)
                    {
                        low |= ((lastByte & MASK_01111111) << size);
                        size += 7;
                    }
                    else
                    {
                        low |= lastByte << size;
                        return low;
                    }
                }
                lastByte = m_reader.ReadByte();
                if ((lastByte & MASK_10000000) == MASK_10000000)
                {
                    low |= (lastByte & MASK_01111111) << size;
                    high = (lastByte & MASK_01111111) >> 4;
                    size = 3;
                    while (size < 32)
                    {
                        lastByte = m_reader.ReadByte();
                        if ((lastByte & MASK_10000000) == MASK_10000000)
                            high |= (lastByte & MASK_01111111) << size;
                        else break;
                        size += 7;
                    }
                    high |= lastByte << size;
                    return (low & 0xFFFFFFFF) | ((long)high << 32);
                }
                low |= lastByte << size;
                high = lastByte >> 4;
                return (low & 0xFFFFFFFF) | (long)high << 32;
            }

            public ulong ReadVarULong()
            {
                return unchecked((ulong)ReadVarLong());
            }

            /// <summary>
            ///   Read a Short from the Buffer
            /// </summary>
            /// <returns></returns>
            public short ReadShort()
            {
                return BitConverter.ToInt16(ReadBigEndianBytes(2), 0);
            }

            /// <summary>
            ///   Read a int from the Buffer
            /// </summary>
            /// <returns></returns>
            public int ReadInt()
            {
                return BitConverter.ToInt32(ReadBigEndianBytes(4), 0);
            }

            /// <summary>
            ///   Read a long from the Buffer
            /// </summary>
            /// <returns></returns>
            public Int64 ReadLong()
            {
                return Int64.FromNumber(BitConverter.ToInt64(ReadBigEndianBytes(8), 0));
            }

            /// <summary>
            ///   Read a Float from the Buffer
            /// </summary>
            /// <returns></returns>
            public float ReadFloat()
            {
                return BitConverter.ToSingle(ReadBigEndianBytes(4), 0);
            }

            /// <summary>
            ///   Read a UShort from the Buffer
            /// </summary>
            /// <returns></returns>
            public ushort ReadUnsignedShort()
            {
                return BitConverter.ToUInt16(ReadBigEndianBytes(2), 0);
            }

            /// <summary>
            ///   Read a int from the Buffer
            /// </summary>
            /// <returns></returns>
            public UInt32 ReadUnsignedInt()
            {
                return BitConverter.ToUInt32(ReadBigEndianBytes(4), 0);
            }

            /// <summary>
            ///   Read a long from the Buffer
            /// </summary>
            /// <returns></returns>
            public UInt64 ReadUnsignedLong()
            {
                return BitConverter.ToUInt64(ReadBigEndianBytes(8), 0);
            }

            /// <summary>
            ///   Read a byte from the Buffer
            /// </summary>
            /// <returns></returns>
            public byte ReadByte()
            {
                return m_reader.ReadByte();
            }

            public sbyte ReadUnsignedByte()
            {
                return m_reader.ReadSByte();
            }

            /// <summary>
            ///   Returns N bytes from the buffer
            /// </summary>
            /// <param name = "n">Number of read bytes.</param>
            /// <returns></returns>
            public byte[] ReadBytes(int n)
            {
                return m_reader.ReadBytes(n);
            }

            /// <summary>
            /// Returns N bytes from the buffer
            /// </summary>
            /// <param name = "n">Number of read bytes.</param>
            /// <returns></returns>
            public BigEndianReader ReadBytesInNewBigEndianReader(int n)
            {
                return new BigEndianReader(m_reader.ReadBytes(n));
            }

            /// <summary>
            ///   Read a Boolean from the Buffer
            /// </summary>
            /// <returns></returns>
            public Boolean ReadBoolean()
            {
                return m_reader.ReadByte() == 1;
            }

            /// <summary>
            ///   Read a Char from the Buffer
            /// </summary>
            /// <returns></returns>
            public Char ReadChar()
            {
                return (char)ReadUnsignedShort();
            }

            /// <summary>
            ///   Read a Double from the Buffer
            /// </summary>
            /// <returns></returns>
            public Double ReadDouble()
            {
                return BitConverter.ToDouble(ReadBigEndianBytes(8), 0);
            }

            /// <summary>
            ///   Read a Single from the Buffer
            /// </summary>
            /// <returns></returns>
            public Single ReadSingle()
            {
                return BitConverter.ToSingle(ReadBigEndianBytes(4), 0);
            }

            /// <summary>
            ///   Read a string from the Buffer
            /// </summary>
            /// <returns></returns>
            public string ReadUTF()
            {
                ushort length = ReadUnsignedShort();

                byte[] bytes = ReadBytes(length);
                return Encoding.UTF8.GetString(bytes);
            }

            /// <summary>
            ///   Read a string from the Buffer
            /// </summary>
            /// <returns></returns>
            public string ReadUTF7BitLength()
            {
                int length = ReadInt();

                byte[] bytes = ReadBytes(length);
                return Encoding.UTF8.GetString(bytes);
            }

            /// <summary>
            ///   Read a string from the Buffer
            /// </summary>
            /// <returns></returns>
            public string ReadUTFBytes(ushort len)
            {
                byte[] bytes = ReadBytes(len);

                return Encoding.UTF8.GetString(bytes);
            }

            /// <summary>
            ///   Skip bytes
            /// </summary>
            /// <param name = "n"></param>
            public void SkipBytes(int n)
            {
                int i;
                for (i = 0; i < n; i++)
                {
                    m_reader.ReadByte();
                }
            }


            public void Seek(int offset, SeekOrigin seekOrigin)
            {
                m_reader.BaseStream.Seek(offset, seekOrigin);
            }

            /// <summary>
            ///   Add a bytes array to the end of the buffer
            /// </summary>
            public void Add(byte[] data, int offset, int count)
            {
                long pos = m_reader.BaseStream.Position;

                m_reader.BaseStream.Position = m_reader.BaseStream.Length;
                m_reader.BaseStream.Write(data, offset, count);
                m_reader.BaseStream.Position = pos;
            }

            #endregion

            #region Dispose

            /// <summary>
            ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                if (m_reader == null)
                    return;

                m_reader.Dispose();
                m_reader = null;
            }

            #endregion
        }
        public class BigEndianWriter : IDisposable
        {
            public const int INT_SIZE = 32;
            public const int SHORT_SIZE = 16;
            public const int SHORT_MIN_VALUE = -0x8000;
            public const int SHORT_MAX_VALUE = 0x7FFF;
            public const int USHORT_MAX_VALUE = 0xFFFF;
            public const int CHUNCK_BIT_SIZE = 7;
            public static readonly int MAX_ENCODING_LENGHT = (int)Math.Ceiling((double)INT_SIZE / CHUNCK_BIT_SIZE);
            public const int MASK_10000000 = 0x80;
            public const int MASK_01111111 = 0x7F;

            #region Properties

            private BinaryWriter m_writer;

            public Stream BaseStream
            {
                get { return m_writer.BaseStream; }
            }

            /// <summary>
            ///   Gets available bytes number in the buffer
            /// </summary>
            public long BytesAvailable
            {
                get { return m_writer.BaseStream.Length - m_writer.BaseStream.Position; }
            }

            public long Position
            {
                get { return m_writer.BaseStream.Position; }
                set
                {
                    m_writer.BaseStream.Position = value;
                }
            }

            public byte[] Data
            {
                get
                {
                    var pos = m_writer.BaseStream.Position;

                    var data = new byte[m_writer.BaseStream.Length];
                    m_writer.BaseStream.Position = 0;
                    m_writer.BaseStream.Read(data, 0, (int)m_writer.BaseStream.Length);

                    m_writer.BaseStream.Position = pos;

                    return data;
                }
            }

            #endregion

            #region Initialisation

            /// <summary>
            /// Initializes a new instance of the <see cref="BigEndianWriter"/> class.
            /// </summary>
            public BigEndianWriter()
            {
                m_writer = new BinaryWriter(new MemoryStream(), Encoding.UTF8);
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="BigEndianWriter"/> class.
            /// </summary>
            /// <param name="stream">The stream.</param>
            public BigEndianWriter(Stream stream)
            {
                m_writer = new BinaryWriter(stream, Encoding.UTF8);
            }

            #endregion

            #region Private Methods

            /// <summary>
            ///   Reverse bytes and write them into the buffer
            /// </summary>
            private void WriteBigEndianBytes(byte[] endianBytes)
            {
                for (int i = endianBytes.Length - 1; i >= 0; i--)
                {
                    m_writer.Write(endianBytes[i]);
                }
            }

            #endregion

            #region Public Methods

            public void WriteVarInt(int @int)
            {
                var value = unchecked((uint)@int);

                if (value <= MASK_01111111)
                {
                    m_writer.Write((byte)value);
                    return;
                }

                int i = 0;
                while (value != 0)
                {
                    var b = (byte)(value & MASK_01111111);
                    i++;
                    value >>= CHUNCK_BIT_SIZE;
                    if (value > 0)
                        b |= MASK_10000000;
                    m_writer.Write(b);
                }
            }

            public void WriteVarUInt(uint @uint)
            {
                WriteVarInt(unchecked((int)@uint));
            }

            public void WriteVarShort(short @short)
            {
                var value = unchecked((ushort)@short);

                if (value <= MASK_01111111)
                {
                    m_writer.Write((byte)value);
                    return;
                }

                int i = 0;
                while (value != 0)
                {
                    var b = (byte)(value & MASK_01111111);
                    i++;
                    value >>= CHUNCK_BIT_SIZE;
                    if (value > 0)
                        b |= MASK_10000000;
                    m_writer.Write(b);
                }
            }

            public void WriteVarUShort(ushort @ushort)
            {
                WriteVarShort(unchecked((short)@ushort));
            }

            public void WriteVarLong(long @long)
            {
                var value = unchecked((ulong)@long);

                if (value >> 32 == 0)
                {
                    WriteVarInt((int)value);
                    return;
                }

                var low = value & 0xFFFFFFFF;
                var high = value >> 32;
                for (int i = 0; i < 4; i++)
                {
                    m_writer.Write((byte)(low & MASK_01111111 | MASK_10000000));
                    low >>= 7;
                }
                if ((high & 0xFFFFFFF8) == 0) // only 3 first bits are non zeros
                {
                    m_writer.Write((byte)(high << 4 | low));
                }
                else
                {
                    m_writer.Write((byte)((high << 4 | low) & MASK_01111111 | MASK_10000000));
                    high >>= 3;
                    while (high >= 0x80)
                    {
                        m_writer.Write((byte)(high & MASK_01111111 | MASK_10000000));
                        high >>= 7;
                    }
                    m_writer.Write((byte)high);
                }
            }

            public void WriteVarULong(ulong @ulong)
            {
                WriteVarLong(unchecked((long)@ulong));
            }

            /// <summary>
            ///   Write a Short into the buffer
            /// </summary>
            /// <returns></returns>
            public void WriteShort(short @short)
            {
                WriteBigEndianBytes(BitConverter.GetBytes(@short));
            }

            /// <summary>
            ///   Write a int into the buffer
            /// </summary>
            /// <returns></returns>
            public void WriteInt(int @int)
            {
                WriteBigEndianBytes(BitConverter.GetBytes(@int));
            }

            /// <summary>
            ///   Write a long into the buffer
            /// </summary>
            /// <returns></returns>
            public void WriteLong(Int64 @long)
            {
                WriteBigEndianBytes(BitConverter.GetBytes(@long.ToNumber()));
            }

            /// <summary>
            ///   Write a UShort into the buffer
            /// </summary>
            /// <returns></returns>
            public void WriteUShort(ushort @ushort)
            {
                WriteBigEndianBytes(BitConverter.GetBytes(@ushort));
            }

            /// <summary>
            ///   Write a int into the buffer
            /// </summary>
            /// <returns></returns>
            public void WriteUInt(UInt32 @uint)
            {
                WriteBigEndianBytes(BitConverter.GetBytes(@uint));
            }

            /// <summary>
            ///   Write a long into the buffer
            /// </summary>
            /// <returns></returns>
            public void WriteULong(UInt64 @ulong)
            {
                WriteBigEndianBytes(BitConverter.GetBytes(@ulong));
            }

            /// <summary>
            ///   Write a byte into the buffer
            /// </summary>
            /// <returns></returns>
            public void WriteByte(byte @byte)
            {
                m_writer.Write(@byte);
            }

            public void WriteSByte(sbyte @byte)
            {
                m_writer.Write(@byte);
            }
            /// <summary>
            ///   Write a Float into the buffer
            /// </summary>
            /// <returns></returns>
            public void WriteFloat(float @float)
            {
                WriteBigEndianBytes(BitConverter.GetBytes(@float));
            }

            /// <summary>
            ///   Write a Boolean into the buffer
            /// </summary>
            /// <returns></returns>
            public void WriteBoolean(Boolean @bool)
            {
                if (@bool)
                {
                    m_writer.Write((byte)1);
                }
                else
                {
                    m_writer.Write((byte)0);
                }
            }

            /// <summary>
            ///   Write a Char into the buffer
            /// </summary>
            /// <returns></returns>
            public void WriteChar(Char @char)
            {
                WriteBigEndianBytes(BitConverter.GetBytes(@char));
            }

            /// <summary>
            ///   Write a Double into the buffer
            /// </summary>
            public void WriteDouble(Double @double)
            {
                WriteBigEndianBytes(BitConverter.GetBytes(@double));
            }

            /// <summary>
            ///   Write a Single into the buffer
            /// </summary>
            /// <returns></returns>
            public void WriteSingle(Single @single)
            {
                WriteBigEndianBytes(BitConverter.GetBytes(@single));
            }

            /// <summary>
            ///   Write a string into the buffer
            /// </summary>
            /// <returns></returns>
            public void WriteUTF(string str)
            {
                var bytes = Encoding.UTF8.GetBytes(str);
                var len = (ushort)bytes.Length;
                WriteUShort(len);

                int i;
                for (i = 0; i < len; i++)
                    m_writer.Write(bytes[i]);
            }

            /// <summary>
            ///   Write a string into the buffer
            /// </summary>
            /// <returns></returns>
            public void WriteUTFBytes(string str)
            {
                var bytes = Encoding.UTF8.GetBytes(str);
                var len = bytes.Length;
                int i;
                for (i = 0; i < len; i++)
                    m_writer.Write(bytes[i]);
            }

            /// <summary>
            ///   Write a bytes array into the buffer
            /// </summary>
            /// <returns></returns>
            public void WriteBytes(byte[] data)
            {
                m_writer.Write(data);
            }


            public void Seek(int offset)
            {
                Seek(offset, SeekOrigin.Begin);
            }

            public void Seek(int offset, SeekOrigin seekOrigin)
            {
                m_writer.BaseStream.Seek(offset, seekOrigin);
            }


            public void Clear()
            {
                m_writer = new BinaryWriter(new MemoryStream(), Encoding.UTF8);
            }

            #endregion

            #region Dispose

            public void Dispose()
            {
                m_writer.Flush();
                m_writer.Dispose();
                m_writer = null;
            }

            #endregion
        }

        public class BooleanByteWrapper
        {
            public static byte SetFlag(byte flag, byte offset, bool value)
            {
                return value ? (byte)(flag | (1 << offset)) : (byte)(flag & 255 - (1 << offset));
            }

            public static bool GetFlag(byte flag, byte offset)
            {
                return (flag & (byte)(1 << offset)) != 0;
            }
        }

        public class Int64
        {
            public long Low { get; set; }

            public long High { get; set; }

            public Int64() { }

            public Int64(long low, long high)
            {
                Low = low;
                High = high;
            }

            public static Int64 FromNumber(long @long)
            {
                return new Int64(@long, (long)Math.Floor(@long / 4294967296.0));
            }

            public long ToNumber()
            {
                return High * 4294967296 + Low;
            }
        }
    }
}
