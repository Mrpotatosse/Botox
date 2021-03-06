﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotoxSharedModel.Models.Actors
{
    public class PlayerModel : ActorModel
    {
        public string Name { get; set; }
        public short Level { get; set; }
        public bool IsMerchant { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
        }
    }
}
