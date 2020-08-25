using Botox.Protocol;
using Botox.Protocol.JsonField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static BotoxNetwork.IO.DofusIO;

namespace Botox.Extension
{
    public static class ProtocolJsonExtension
    {
        public static bool IsPrimitiv(string type)
        {
            return ProtocolManager.Instance.Protocol[ProtocolKeyEnum.MessageAndTypes, x => x.name == type] is null;
        }

        /// <summary>
        /// PAS ENCORE FINI
        /// </summary>
        /// <param name="field"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static byte[] ToByte(this NetworkElementField field, ProtocolJsonContent content)
        {
            if (content is null) return new byte[0];
            if (field is null) throw new Exception("field cannot be null");

            try
            {
                using (BigEndianWriter writer = new BigEndianWriter())
                {
                    if (field.super_serialize)
                    {
                        NetworkElementField super = ProtocolManager.Instance.Protocol[ProtocolKeyEnum.MessageAndTypes, x => x.name == field.super];
                        byte[] super_data = super.ToByte(content);
                        writer.WriteBytes(super_data);
                    }

                    IEnumerable<ClassField> boolWrapper = field.fields.Where(x => x.use_boolean_byte_wrapper).OrderBy(x => x.boolean_byte_wrapper_position);
                    IEnumerable<ClassField> vars = field.fields.Where(x => !boolWrapper.Contains(x)).OrderBy(x => x.position);

                    if (boolWrapper.Count() > 0)
                    {
                        byte[] flags = new byte[boolWrapper.LastOrDefault().position.Value + 1];

                        foreach (ClassField _bool in boolWrapper)
                        {
                            flags[_bool.position.Value] = BooleanByteWrapper.SetFlag(flags[_bool.position.Value], (byte)((_bool.boolean_byte_wrapper_position.Value - 1) % 8), content[_bool.name]);
                        }

                        writer.WriteBytes(flags);
                    }

                    foreach (ClassField _var in vars)
                    {
                        Parse(_var, content[_var.name], writer);
                    }

                    return writer.Data;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e}");
                return new byte[0];
            }
        }

        public static void Parse(ClassField field, dynamic value, BigEndianWriter writer)
        {
            if (field.is_vector || field.type == "ByteArray")
            {
                string write_len_method = field.write_length_method.Replace("write", "Write");
                _writeMethod(write_len_method, value.Length, ref writer);

                for(int i = 0; i < value.Length; i++)
                {
                    _writeElement(field, value[i], ref writer);
                }
            }
            else
            {
                _writeElement(field, value, ref writer);
            }
        }

        private static void _writeMethod(string write_method, dynamic value, ref BigEndianWriter writer)
        {
            switch (write_method)
            {
                case "WriteByte": writer.WriteByte((byte)value); return;
                case "WriteUnsignedByte": writer.WriteUnsignedByte((sbyte)value); return;
                case "WriteShort": writer.WriteShort((short)value); return;
                case "WriteUnsignedShort": writer.WriteUnsignedShort((ushort)value); return;
                case "WriteVarShort": writer.WriteVarShort((short)value); return;
                case "WriteVarUhShort": writer.WriteVarUhShort((ushort)value); return;
                case "WriteInt": writer.WriteInt((int)value); return;
                case "WriteUnsignedInt": writer.WriteUnsignedInt((uint)value); return;
                case "WriteVarInt": writer.WriteVarInt((int)value); return;
                case "WriteVarUhInt": writer.WriteVarUhInt((uint)value); return;
                case "WriteLong": writer.WriteLong(value); return;
                case "WriteUnsignedLong": writer.WriteUnsignedLong(value); return;
                case "WriteVarLong": writer.WriteVarLong(value); return;
                case "WriteVarUhLong": writer.WriteVarUhLong(value); return;
                case "WriteUTF": writer.WriteUTF(value); return;
                case "WriteDouble": writer.WriteDouble((double)value); return;
                case "WriteFloat": writer.WriteFloat((float)value); return;
                case "WriteBoolean": writer.WriteBoolean(value); return;
                default:
                    throw new NotImplementedException($"Method : '{write_method}' is not implemented");
            }
        }

        public static void _writeElement(ClassField field, dynamic value, ref BigEndianWriter writer)
        {
            if (IsPrimitiv(field.type))
            {
                string write_method = field.write_method.Replace("write", "Write");
                _writeMethod(write_method, value, ref writer);
            }
            else
            {
                NetworkElementField var_type = null;
                bool is_null = value is null;

                if(is_null && field.write_false_if_null_method != null && field.write_false_if_null_method != "")
                {
                    string check_null_method = field.write_false_if_null_method.Replace("write", "Write");
                    _writeMethod(check_null_method, 0, ref writer);
                    return;
                }

                if (is_null) throw new Exception($"{var_type.name} cannot be null");

                if(field.write_type_id_method is null || field.write_type_id_method is "")
                {
                    var_type = ProtocolManager.Instance.Protocol[ProtocolKeyEnum.Types, x => x.name == field.type];
                }
                else
                {
                    string write_type_id_method = field.write_type_id_method.Replace("write", "Write");
                    _writeMethod(write_type_id_method, value["protocol_id"], ref writer);

                    var_type = ProtocolManager.Instance.Protocol[ProtocolKeyEnum.Types, x => x.protocolID == value["protocol_id"]];
                }

                writer.WriteBytes(var_type.ToByte(value as ProtocolJsonContent));
            }
        }

        #region Reader
        public static ProtocolJsonContent Parse(this NetworkElementField field, ref BigEndianReader reader, ProtocolJsonContent content = null)
        {
            if (content is null) content = new ProtocolJsonContent();
            if (field is null) return content;

            try
            {
                if (field.super_serialize)
                {
                    NetworkElementField super = ProtocolManager.Instance.Protocol[ProtocolKeyEnum.MessageAndTypes, x => x.name == field.super];
                    content = super.Parse(ref reader, content);
                }
                IEnumerable<ClassField> boolWrapper = field.fields.Where(x => x.use_boolean_byte_wrapper).OrderBy(x => x.boolean_byte_wrapper_position);
                IEnumerable<ClassField> vars = field.fields.Where(x => !boolWrapper.Contains(x)).OrderBy(x => x.position);

                byte flag = 0;
                for (byte i = 0;i<boolWrapper.Count();i++)
                {
                    ClassField _bool = boolWrapper.ElementAt(i);
                    
                    if (i % 8 == 0)
                        flag = reader.ReadByte();

                    content[_bool.name] = BooleanByteWrapper.GetFlag(flag, i);
                }

                foreach(ClassField _var in vars)
                {
                    content[_var.name] = _var.Parse(ref reader);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine($"{e}");
            }

            return content;
        }

        public static dynamic Parse(this ClassField field, ref BigEndianReader reader)
        {
            if (field.is_vector || field.type == "ByteArray")
            {
                dynamic[] array = null;
                if (field.constant_length.HasValue)
                {
                    array = new dynamic[field.constant_length.Value];
                }
                else
                {
                    string read_length_method = $"Read{field.write_length_method.Replace("write", "")}";
                    array = new dynamic[read_length_method._readMethod(ref reader)];
                }

                for(int i = 0; i < array.Length; i++)
                {
                    array[i] = field._readElement(ref reader);
                }

                return array;
            }
            else
            {
                return field._readElement(ref reader);
            }
        }

        private static dynamic _readElement(this ClassField field, ref BigEndianReader reader)
        {
            if (IsPrimitiv(field.type))
            {
                string read_method = $"Read{field.write_method.Replace("write", "")}";
                return read_method._readMethod(ref reader);
            }
            else
            {
                NetworkElementField network_element = null;
                ProtocolJsonContent content = null;

                bool is_null = false;
                if (field.write_false_if_null_method != null && field.write_false_if_null_method != "")
                {
                    string check_null_method = $"Read{field.write_false_if_null_method.Replace("write", "")}";
                    is_null = check_null_method._readMethod(ref reader) == 0; 
                }

                if (is_null) return null;

                if (field.write_type_id_method is null || field.write_type_id_method is "")
                {
                    network_element = ProtocolManager.Instance.Protocol[ProtocolKeyEnum.Types, x => x.name == field.type];
                }
                else
                {
                    string read_id_method = $"Read{field.write_type_id_method.Replace("write", "")}";
                    dynamic protocol_id = read_id_method._readMethod(ref reader);
                    content = new ProtocolJsonContent();
                    content["protocol_id"] = protocol_id;

                    network_element = ProtocolManager.Instance.Protocol[ProtocolKeyEnum.Types, x => x.protocolID == protocol_id];
                }

                return network_element.Parse(ref reader, content);
            }
        }

        private static dynamic _readMethod(this string reading_method,ref BigEndianReader reader)
        {
            try
            {
                dynamic value = typeof(BigEndianReader).GetMethod(reading_method).Invoke(reader, new object[0]);
                return value;
            }
            catch
            {
                return null;
            }
        }
        #endregion 
    }
}
