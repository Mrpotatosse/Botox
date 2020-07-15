using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Botox.Protocol.JsonField
{
    public class ClassField
    {
        public string name { get; set; }
        public string type { get; set; } 
        public string default_value { get; set; }
        public int? position { get; set; }

        public string self_serialize_method { get; set; }
        public string write_type_id_method { get; set; }
        public string write_method { get; set; }
        public string write_length_method { get; set; }
        public string write_false_if_null_method { get; set; }
        public Limits bounds { get; set; }
        public int? boolean_byte_wrapper_position { get; set; }
        public int? constant_length { get; set; }
        public bool is_vector { get; set; }
    }
}
