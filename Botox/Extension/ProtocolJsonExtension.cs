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
                    content[_var.name] = null;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine($"{e}");
            }

            return content;
        }
    }
}
