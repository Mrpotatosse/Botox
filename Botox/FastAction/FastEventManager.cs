using Botox.Extension;
using Botox.FastAction.Models.Enums;
using BotoxSharedModel.Models.Actors;
using BotoxSharedModel.Models.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Botox.FastAction
{
    public class FastEventManager : Singleton<FastEventManager>
    {
        public event Action<PlayerModel> OnPlayerSelected;
        public event Action<int> OnPlayerSelectedDisconnect;

        public event Action<MapModel> OnSelectedEnterMap;
        public event Action<double> OnSelectedLeaveMap;

        public event Action<PlayerModel> OnPlayerEnterMap;
        public event Action<double, double> OnElementRemovedMap;

        public void Handle(FastEventEnum key, params object[] param)
        {
            switch (key)
            {
                case FastEventEnum.PlayerSelected: OnPlayerSelected?.Invoke(param[0] as PlayerModel); return;
                case FastEventEnum.PlayerSelectedDisconnect: OnPlayerSelectedDisconnect?.Invoke((int)param[0]); return;

                case FastEventEnum.PlayerSelectedEnterMap: OnSelectedEnterMap?.Invoke(param[0] as MapModel); return;
                case FastEventEnum.PlayerSelectedLeaveMap: OnSelectedLeaveMap?.Invoke((double)param[0]); return;

                case FastEventEnum.PlayerEnterMap: OnPlayerEnterMap?.Invoke(param[0] as PlayerModel); return;
                case FastEventEnum.ElementRemovedMap: OnElementRemovedMap?.Invoke((double)param[0], (double)param[1]); return;
            }
        }
    }
}
