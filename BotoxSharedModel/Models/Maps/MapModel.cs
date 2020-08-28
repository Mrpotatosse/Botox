using BotoxSharedModel.Models.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotoxSharedModel.Models.Maps
{
    public class MapModel
    {
        public double MapId { get; set; }
        public List<ActorModel> Actors { get; set; } = new List<ActorModel>();

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
        }
    }
}
