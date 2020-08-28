using Botox.Configuration;
using Botox.Extension;
using Botox.FastAction;
using Botox.Hook;
using Botox.Proxy;
using BotoxSharedModel.Models.Actors;
using BotoxSharedModel.Models.Maps;
using BotoxUI;
using BotoxUI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Botox.UI
{
    public class UIManager : Singleton<UIManager>
    {
        private MainWindow UI { get; set; }

        private void InitFastEvent()
        {
            FastEventManager.Instance.OnPlayerSelected += Instance_OnPlayerSelected;
            FastEventManager.Instance.OnPlayerSelectedDisconnect += Instance_OnPlayerSelectedDisconnect;

            FastEventManager.Instance.OnSelectedEnterMap += Instance_OnSelectedEnterMap;

            FastEventManager.Instance.OnElementRemovedMap += Instance_OnElementRemovedMap;
            FastEventManager.Instance.OnPlayerEnterMap += Instance_OnPlayerEnterMap;
        }

        private void Instance_OnPlayerEnterMap(PlayerModel obj)
        {
            int id = ProxyManager.Instance[x => x.CharacterSelected.MapId == obj.MapId];
            try
            {
                UI.Dispatcher.Invoke(() =>
                {
                    UI.AddMapElement(id, obj);
                });
            }
            catch (TaskCanceledException)
            {

            }
        }

        private void Instance_OnElementRemovedMap(double obj, double mapId)
        {
            int id = ProxyManager.Instance[x => x.CharacterSelected.MapId == mapId];
            try
            {
                UI.Dispatcher.Invoke(() =>
                {
                    UI.RemoveMapElement(id, obj);
                });
            }
            catch (TaskCanceledException)
            {

            }
        }

        private void Instance_OnSelectedEnterMap(MapModel obj)
        {
            int id = ProxyManager.Instance[x => obj.Actors.FirstOrDefault(v => v.Id == x.CharacterSelected.Id) != null];
            try
            {
                UI.Dispatcher.Invoke(() =>
                {
                    UI.SetMap(id, obj);
                });
            }
            catch(TaskCanceledException)
            {

            }
        }

        private void Instance_OnPlayerSelectedDisconnect(int obj)
        {
            try
            {
                UI.Dispatcher.Invoke(() =>
                {
                    UI.SetSelected(obj, null);
                });
            }
            catch(TaskCanceledException)
            {
                // to do
            }
        }

        private void Instance_OnPlayerSelected(PlayerModel obj)
        {
            int id = ProxyManager.Instance[obj.Name];
            try
            {
                UI.Dispatcher.Invoke(() =>
                {
                    UI.SetSelected(id, obj);
                });
            }
            catch (TaskCanceledException)
            {

            }
        }

        public void Init()
        {
            if (!ConfigurationManager.Instance.Startup.show_ui)
                return;

            Thread thread = new Thread(new ThreadStart(() =>
            {
                Application app = new Application();
                UI = CreateUI();
                app.Run(UI);
            }));

            thread.SetApartmentState(ApartmentState.STA);

            thread.Start();

            InitFastEvent();
        }

        private MainWindow CreateUI()
        {
            MainWindow window = new MainWindow();

            window.OpenDofusAction += Window_OpenDofusAction;

            return window;
        }

        private void Window_OpenDofusAction(int obj)
        {
            HookManager.Instance.InitHook(ConfigurationManager.Instance.Startup.dofus_location,
                                          ConfigurationManager.Instance.Startup.dll_location,
                                          obj);
        }

        public void OpenPage(int id)
        {
            if (!ConfigurationManager.Instance.Startup.show_ui)
                return;

            CustomButton btn = new CustomButton()
            {
                Height = 50,
                Text = "Waiting ...",
                SelectionColor = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red),
                IsSelected = false,
                Id = id
            };

            UI.AddLeftPanel(btn);
        }

        public void RemovePage(int id)
        {
            if (!ConfigurationManager.Instance.Startup.show_ui)
                return;

            UI.Remove(id);
        }
    }
}
