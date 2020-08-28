using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Botox.FastAction.Models.Enums
{
    public enum FastEventEnum
    {
        PlayerSelected = 0,
        PlayerSelectedDisconnect,

        PlayerSelectedEnterMap,
        PlayerSelectedLeaveMap,

        PlayerEnterMap,
        ElementRemovedMap
    }
}
