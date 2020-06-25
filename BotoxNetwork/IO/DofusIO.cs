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

        // naywin io
        /*internal class CustomInt64
        {
            public long Low { get; set; }

            public long High { get; set; }

            public CustomInt64()
            {
            }

            public CustomInt64(long Low, long High)
            {
                this.Low = Low;
                this.High = High;
            }

            public static CustomInt64 FromNumber(long Long)
            {
                return new CustomInt64(Long, (long)Math.Floor(Long / 4294967296.0));
            }

            public long ToNumber()
            {
                return High * 4294967296 + Low;
            }
        }

        internal static unsafe class BigEndian
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static short ReadInt16(byte* P)
            {
                return (short)(P[0] << 8 | P[1]);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static int ReadInt32(byte* P)
            {
                return P[0] << 24 | P[1] << 16 | P[2] << 8 | P[3];
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static long ReadInt64(byte* P)
            {
                var Lo = ReadInt32(P);
                var Hi = ReadInt32(P + 4);
                return (long)Lo << 32 | (uint)Hi;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void WriteInt16(byte* P, short S)
            {
                P[0] = (byte)(S >> 8);
                P[1] = (byte)S;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void WriteInt32(byte* P, int I)
            {
                P[0] = (byte)(I >> 24);
                P[1] = (byte)(I >> 16);
                P[2] = (byte)(I >> 8);
                P[3] = (byte)I;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void WriteInt64(byte* P, long L)
            {
                P[0] = (byte)(L >> 56);
                P[1] = (byte)(L >> 48);
                P[2] = (byte)(L >> 40);
                P[3] = (byte)(L >> 32);
                P[4] = (byte)(L >> 24);
                P[5] = (byte)(L >> 16);
                P[6] = (byte)(L >> 8);
                P[7] = (byte)L;
            }
        }

        internal static unsafe class Reinterpret
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static long DoubleAsInt64(double Double)
                => *(long*)&Double;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static double Int64AsDouble(long Long)
                => *(double*)&Long;
        }

        public unsafe class BigEndianWriter : BinaryWriter
        {
            protected readonly byte[] Buffer;

            public byte[] Data
            {
                get
                {
                    var Stream = (MemoryStream)BaseStream;
                    return Stream.ToArray();
                }
            }

            public BigEndianWriter() : base(new MemoryStream(), Encoding.UTF8)
            {
                Buffer = new byte[16];
            }

            public BigEndianWriter(byte[] Data) : base(new MemoryStream(Data), Encoding.UTF8)
            {
                Buffer = new byte[16];
            }

            public void WriteInt(int Int)
            {
                fixed (byte* P = Buffer)
                    BigEndian.WriteInt32(P, Int);
                OutStream.Write(Buffer, 0, 4);
            }

            public void WriteByte(byte Byte)
            {
                Write(Byte);
            }

            public void WriteSByte(sbyte SByte)
            {
                Write(SByte);
            }

            public void WriteShort(int Short)
            {
                fixed (byte* P = Buffer)
                    BigEndian.WriteInt16(P, (short)Short);
                OutStream.Write(Buffer, 0, 2);
            }

            public void WriteShort(short Short)
            {
                fixed (byte* P = Buffer)
                    BigEndian.WriteInt16(P, Short);
                OutStream.Write(Buffer, 0, 2);
            }

            public void WriteUTF(string String)
            {
                var Bytes = Encoding.UTF8.GetBytes(String);
                WriteShort(Bytes.Length);
                WriteBytes(Bytes);
            }

            public void WriteBoolean(bool Bool)
            {
                Write(Bool);
            }

            public void WriteBytes(byte[] Bytes)
            {
                Write(Bytes);
            }

            public void WriteDouble(double Double)
            {
                fixed (byte* P = Buffer)
                    BigEndian.WriteInt64(P, Reinterpret.DoubleAsInt64(Double));
                OutStream.Write(Buffer, 0, 8);
            }

            public void WriteVarInt(int Int)
            {
                using (var Loc2 = new BigEndianWriter())
                {
                    if (Int >= 0 && Int <= 127)
                    {
                        Loc2.WriteByte((byte)Int);
                        WriteBytes(Loc2.Data);
                        return;
                    }

                    var Loc3 = Int;
                    var Loc4 = new BigEndianWriter();

                    while (Loc3 != 0)
                    {
                        Loc4.WriteByte((byte)(Loc3 & 127));

                        var Reader = new BigEndianReader(Loc4.Data);
                        var Loc5 = Reader.ReadByte();

                        Loc4 = new BigEndianWriter(Reader.Data);
                        Loc3 = Loc3 >> 7;

                        if (Loc3 > 0)
                        {
                            Loc5 = (byte)(Loc5 | 128);
                        }

                        Loc2.WriteByte(Loc5);
                        Reader.Dispose();
                    }

                    WriteBytes(Loc2.Data);
                    Loc4.Dispose();
                }
            }

            public void WriteVarShort(short Short)
            {
                using (var Loc2 = new BigEndianWriter())
                {
                    if (Short >= 0 && Short <= 127)
                    {
                        Loc2.WriteByte((byte)Short);
                        WriteBytes(Loc2.Data);
                        return;
                    }

                    var Loc3 = Short & 65535;
                    var Loc4 = new BigEndianWriter();

                    while (Loc3 != 0)
                    {
                        Loc4.WriteByte((byte)(Loc3 & 127));

                        var Reader = new BigEndianReader(Loc4.Data);
                        int Loc5 = Reader.ReadByte();

                        Loc4 = new BigEndianWriter(Reader.Data);
                        Loc3 = Loc3 >> 7;

                        if (Loc3 > 0)
                        {
                            Loc5 = Loc5 | 128;
                        }

                        Loc2.WriteByte((byte)Loc5);
                        Reader.Dispose();
                    }

                    WriteBytes(Loc2.Data);
                    Loc4.Dispose();
                }
            }

            public void WriteVarLong(long Long)
            {
                var Final = CustomInt64.FromNumber(Long);
                if (Final.High == 0)
                {
                    WriteInt32(Final.Low);
                }
                else
                {
                    for (var I = 0; I < 4; I++)
                    {
                        WriteByte((byte)((Final.Low & 127) | 128));
                        Final.Low = Final.Low >> 7;
                    }

                    if ((Final.High & 268435455 << 3) == 0)
                    {
                        WriteByte((byte)(Final.High << 4 | Final.Low));
                    }
                    else
                    {
                        WriteByte((byte)((Final.High << 4 | Final.Low) & 127 | 128));
                        WriteInt32(Final.High >> 3);
                    }
                }
            }

            public void WriteInt32(long Int)
            {
                while (Int >= 128)
                {
                    WriteByte((byte)(Int & 127 | 128));
                    Int = Int >> 7;
                }

                WriteByte((byte)Int);
            }

            public int BytesAvailable => (int)(BaseStream.Length - BaseStream.Position);
        }

        public unsafe class BigEndianReader : BinaryReader
        {
            protected readonly byte[] Buffer;

            public byte[] Data
            {
                get
                {
                    var Stream = (MemoryStream)BaseStream;
                    return Stream.ToArray();
                }
            }

            public BigEndianReader(Stream Stream) : base(Stream, Encoding.UTF8)
            {
                Buffer = new byte[16];
            }

            public BigEndianReader(byte[] Data) : base(new MemoryStream(Data), Encoding.UTF8)
            {
                Buffer = new byte[16];
            }

            public int ReadInt()
            {
                FillBuffer(4);
                fixed (byte* P = Buffer)
                    return BigEndian.ReadInt32(P);
            }

            public string ReadUTF()
            {
                return Encoding.UTF8.GetString(ReadBytes(ReadShort()));
            }

            public short ReadShort()
            {
                FillBuffer(2);
                fixed (byte* P = Buffer)
                    return BigEndian.ReadInt16(P);
            }

            public override bool ReadBoolean()
            {
                return ReadByte() == 1;
            }

            public override double ReadDouble()
            {
                FillBuffer(8);
                fixed (byte* P = Buffer)
                    return Reinterpret.Int64AsDouble(BigEndian.ReadInt64(P));
            }

            public int ReadVarInt()
            {
                var Loc1 = 0;
                var Loc2 = 0;

                while (Loc2 < 32)
                {
                    var Loc4 = ReadByte();
                    var Loc3 = (Loc4 & 128) == 128;
                    if (Loc2 > 0)
                    {
                        Loc1 = Loc1 + ((Loc4 & 127) << Loc2);
                    }
                    else
                    {
                        Loc1 = Loc1 + (Loc4 & 127);
                    }

                    Loc2 = Loc2 + 7;
                    if (!Loc3)
                    {
                        return Loc1;
                    }
                }

                throw new InvalidDataException();
            }

            public short ReadVarShort()
            {
                var Loc1 = 0;
                var Loc2 = 0;

                while (Loc2 < 16)
                {
                    int Loc4 = ReadByte();

                    var Loc3 = (Loc4 & 128) == 128;

                    if (Loc2 > 0)
                    {
                        Loc1 = Loc1 + ((Loc4 & 127) << Loc2);
                    }
                    else
                    {
                        Loc1 = Loc1 + (Loc4 & 127);
                    }

                    Loc2 = Loc2 + 7;

                    if (Loc3) continue;

                    if (Loc1 > 32767)
                    {
                        Loc1 = Loc1 - 65536;
                    }

                    return (short)Loc1;
                }

                throw new InvalidDataException();
            }

            public long ReadVarLong()
            {
                int Loc3;
                var Loc2 = new CustomInt64();
                var Loc4 = 0;

                while (true)
                {
                    Loc3 = ReadByte();
                    if (Loc4 == 28)
                    {
                        break;
                    }

                    if (Loc3 >= 128)
                    {
                        Loc2.Low = Loc2.Low | (uint)(Loc3 & 127) << Loc4;
                        Loc4 = Loc4 + 7;
                        continue;
                    }

                    Loc2.Low = Loc2.Low | (uint)Loc3 << Loc4;
                    return Loc2.ToNumber();
                }

                if (Loc3 > 128)
                {
                    Loc3 = Loc3 & 127;
                    Loc2.Low = Loc2.Low | (uint)Loc3 << Loc4;
                    Loc2.High = (uint)(Loc3 >> 4);
                    Loc4 = 3;
                    while (true)
                    {
                        Loc3 = ReadByte();
                        if (Loc4 < 32)
                        {
                            if (Loc3 >= 128)
                            {
                                Loc2.High = Loc2.High | (uint)(Loc3 & 127) << Loc4;
                            }
                            else
                            {
                                break;
                            }
                        }

                        Loc4 = Loc4 + 7;
                    }

                    Loc2.High = Loc2.High | (uint)Loc3 << Loc4;
                    return Loc2.ToNumber();
                }

                Loc2.Low = Loc2.Low | (uint)Loc3 << Loc4;
                Loc2.High = (uint)(Loc3 >> 4);
                return Loc2.ToNumber();
            }

            protected override void FillBuffer(int Count)
            {
                var Read = 0;
                do
                {
                    var N = BaseStream.Read(Buffer, Read, Count - Read);
                    if (N == 0)
                        throw new EndOfStreamException();
                    Read += N;
                } while (Read < Count);
            }

            public int BytesAvailable => (int)(BaseStream.Length - BaseStream.Position);
        }*/
    }
}
