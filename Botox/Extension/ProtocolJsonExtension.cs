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
            if (type == "ByteArray") return true;

            return ProtocolManager.Instance.GetNetwork(x => x.name == type) is null 
                && ProtocolManager.Instance.GetNetwork(x => x.name == type, false) is null; 
        }

        public static ProtocolJsonContent Parse(this NetworkElementField field, ref BigEndianReader reader, ProtocolJsonContent content = null)
        {
            if (content is null) content = new ProtocolJsonContent();
            if (field is null) return content;

            try
            {
                if (field.super_serialize)
                {
                    NetworkElementField super = ProtocolManager.Instance.GetNetwork(x => x.name == field.super);
                    if (super is null)
                        super = ProtocolManager.Instance.GetNetwork(x => x.name == field.super, false);
                    content = super.Parse(ref reader, content);

                }
                IEnumerable<ClassField> boolWrapper = field.fields.Where(x => x.boolean_byte_wrapper_position != null).OrderBy(x => x.boolean_byte_wrapper_position);
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

                bool is_null = false;
                if (field.write_false_if_null_method != null && field.write_false_if_null_method != "")
                {
                    string check_null_method = $"Read{field.write_false_if_null_method.Replace("write", "")}";
                    is_null = check_null_method._readMethod(ref reader) == 0; 
                }

                if (is_null) return null;

                if (field.write_type_id_method is null || field.write_type_id_method is "")
                {
                    network_element = ProtocolManager.Instance.GetNetwork(x => x.name == field.type, false);
                }
                else
                {
                    string read_id_method = $"Read{field.write_type_id_method.Replace("write", "")}";
                    dynamic protocol_id = read_id_method._readMethod(ref reader);
                    network_element = ProtocolManager.Instance.GetNetwork(x => x.protocolID == protocol_id, false);
                }

                return network_element.Parse(ref reader);
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
    }
}
