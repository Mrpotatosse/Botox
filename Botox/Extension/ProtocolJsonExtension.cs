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

        /// <summary>
        /// Pas encore fini , il reste encore des choses à finir
        /// </summary>
        /// <param name="field"></param>
        /// <param name="reader"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static ProtocolJsonContent Parse(this NetworkElementField field, BigEndianReader reader, ProtocolJsonContent content = null)
        {
            if (content is null) content = new ProtocolJsonContent();
            if (field is null) return content;

            try
            {
                NetworkElementField super = field.super != null && field.super != "" ? ProtocolManager.Instance.GetNetwork(x => x.name == field.super) : null;
                if (super != null)
                    content = super.Parse(reader, content);

                IEnumerable<ClassField> boolWrapper = field.fields.Where(x => x.boolean_byte_wrapper_position.HasValue).OrderBy(x => x.boolean_byte_wrapper_position);
                IEnumerable<ClassField> vars = field.fields.Where(x => !boolWrapper.Contains(x)).OrderBy(x => x.position);

                byte flag = 0;
                foreach(ClassField _bool in boolWrapper)
                {
                    if (_bool.boolean_byte_wrapper_position % 8 == 0)
                        flag = reader.ReadByte();

                    content[_bool.name] = BooleanByteWrapper.GetFlag(flag, (byte)_bool.boolean_byte_wrapper_position.Value);
                }

                foreach(ClassField _var in vars)
                {
                    /*if (_var.is_vector)
                    {
                        dynamic[] array = null;

                        if (_var.constant_length.HasValue)
                        {
                            array = new dynamic[_var.constant_length.Value];
                        }
                        else
                        {
                            array = new dynamic[1];
                        }

                        for(int i = 0; i < array.Length; i++)
                        {
                            if (IsPrimitiv(_var.type))
                            {
                                array[i] = "";
                            }
                            else
                            {
                                array[i] = ProtocolManager.Instance.GetNetwork(x => x.name == _var.type, false).Parse(reader);
                            }
                        }

                        content[_var.name] = array;
                    }
                    else
                    {
                        if (IsPrimitiv(_var.type))
                        {
                            content[_var.name] = "";
                        }
                        else
                        {
                            content[_var.name] = ProtocolManager.Instance.GetNetwork(x => x.name == _var.type, false).Parse(reader);
                        }
                    }*/
                }
            }
            catch(Exception e)
            {
                Console.WriteLine($"{e}");
            }

            return content;
        }

        /*private static dynamic Parse(this ClassField field, BigEndianReader reader)
        {
            if (IsPrimitiv(field.type))
            {
                string read_method = $"Read{field.write_method.Replace("write", "")}";                
                return typeof(BigEndianReader).GetMethod(read_method).Invoke(reader, new object[0]);
            }
            else
            {
                NetworkElementField network_element = null;
                if(field.write_type_id_method != null && field.write_type_id_method != "")
                {
                    string read_method = $"Read{field.write_type_id_method.Replace("write", "")}";
                    dynamic protocolId = typeof(BigEndianReader).GetMethod(read_method).Invoke(reader, new object[0]);

                    network_element = ProtocolManager.Instance.GetNetwork(x => x.protocolID == protocolId, false);
                }
                else
                {
                    network_element = ProtocolManager.Instance.GetNetwork(x => x.name == field.type, false);
                }

                return network_element.Parse(reader);
            }
        }

        // labot
        public static ProtocolJsonContent Parse(this ProtocolJsonElement element, BigEndianReader reader, ProtocolJsonContent content = null)
        {
            if (content is null)
            {
                content = new ProtocolJsonContent();
            }

            if (element is null) return content;

            try
            {
                ProtocolJsonElement parent = element.parent == null ? null : ProtocolManager.Instance.Get(element.parent);
                if (parent != null)
                    content = Parse(parent, reader, content);

                int i = 0;
                byte flag = 0;
                while (i < element.boolVars.Length)
                {
                    if (i % 8 == 0)
                    {
                        flag = reader.ReadByte();
                    }

                    content[element.boolVars[i].name] = BooleanByteWrapper.GetFlag(flag, (byte)(i % 8));
                    i++;
                }

                i = 0;
                while (i < element.vars.Length)
                {
                    if (element.vars[i].IsArray(reader, out int length))
                    {
                        object[] array = new object[length];
                        for (int k = 0; k < length; k++)
                        {
                            if (element.vars[i].IsPrimitive(reader, out object value))
                            {
                                array[k] = value;
                            }
                            else if (element.vars[i].IsObject(reader, out ProtocolJsonContent objectArray))
                            {
                                array[k] = objectArray;
                            }
                        }

                        content[element.vars[i].name] = array;
                    }
                    else
                    {
                        if (element.vars[i].IsPrimitive(reader, out object value))
                        {
                            content[element.vars[i].name] = value;
                        }
                        else if (element.vars[i].IsObject(reader, out ProtocolJsonContent objectArray))
                        {
                            content[element.vars[i].name] = objectArray;
                        }
                    }

                    i++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e}");
            }

            return content;
        }

        public static bool IsPrimitive(this ProtocolJsonVar var, BigEndianReader reader, out object value, bool len = false)
        {
            value = null;
            string primitiv = len ? var.length : var.type;
            if (PrimitiveVar.Contains(primitiv))
            {
                MethodInfo info = typeof(BigEndianReader).GetMethod($"Read{primitiv}");
                value = info.Invoke(reader, new object[0]);

                return true;
            }
            return false;
        }

        public static bool IsArray(this ProtocolJsonVar var, BigEndianReader reader, out int length)
        {
            length = 0;

            if (var.length is null)
                return false;

            if(int.TryParse(var.length, out length))
            {
                return true;
            }  
            
            if(var.IsPrimitive(reader, out object value, true))
            {
                length = int.Parse(value.ToString());
                return true;
            }

            return false;
        } 

        public static bool IsObject(this ProtocolJsonVar var, BigEndianReader reader, out ProtocolJsonContent value)
        {
            value = null;

            int id = -1;
            if(var.type == "false")
            {
                id = reader.ReadUnsignedShort();
            }

            ProtocolJsonElement element = id > 0 ? ProtocolManager.Instance.Get(id, false) : ProtocolManager.Instance.Get(var.type);
            if(!(element is null))
            {
                value = element.Parse(reader);
                return true;
            }

            return false;
        }

        private static readonly string[] PrimitiveVar = new string[]
        {
            "Boolean", "Byte", "Double", "Float", "Int", "Short", "UTF", "UnsignedByte", "UnsignedInt", "UnsignedShort", "VarInt", "VarLong", "VarShort", "VarUhInt", "VarUhLong", "VarUhShort"
        };  */      
    }
}
