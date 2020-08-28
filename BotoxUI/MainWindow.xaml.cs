using BotoxSharedModel.Models.Actors;
using BotoxSharedModel.Models.Maps;
using BotoxUI.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BotoxUI
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public event Action<int> OpenDofusAction;

        private static readonly HomePage Home = new HomePage();
        private static readonly Dictionary<int, ClientPage> Clients = new Dictionary<int, ClientPage>();

        public MainWindow()
        {
            InitializeComponent();

            MainFrame.Navigate(Home);
        }

        public void AddLeftPanel(CustomButton button)
        {
            button.Margin = new Thickness(1);

            if (Clients.ContainsKey(button.Id))
                Clients.Remove(button.Id);
                       
            Clients.Add(button.Id, new ClientPage());
            
            LeftPanel.Items.Add(button);
        }

        private void LeftPanel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (CustomButton item in LeftPanel.Items)
                item.IsSelected = false;

            if (LeftPanel.SelectedItem is CustomButton custom)
            {
                custom.IsSelected = true;
                if (Clients[custom.Id].HasModel)
                {
                    MainFrame.Navigate(Clients[custom.Id]);
                }
                else
                {
                    MainFrame.Navigate(Home);
                }
            }
        }

        private delegate void OpenDofus(int count);
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await Dispatcher.BeginInvoke(new OpenDofus(OpenDofusAction), 1);
        }

        public void SetSelected(int id, PlayerModel model)
        {
            foreach (CustomButton item in LeftPanel.Items)
            {
                if(item.Id == id)
                {
                    if (model is null)
                    {
                        item.SelectionColor = new SolidColorBrush(Colors.Red);
                        item.Text = "Waiting ...";
                        Clients[id].SetCharacter(null);
                        if (item.IsSelected)
                            MainFrame.Navigate(Home);
                    }
                    else
                    {
                        item.SelectionColor = new SolidColorBrush(Colors.Green);
                        item.Text = model.Name;
                        Clients[id].SetCharacter(model);
                        if(item.IsSelected)
                            MainFrame.Navigate(Clients[id]);                   
                    }
                    return;
                }
            }
        }

        public void SetMap(int id, MapModel model)
        {
            foreach (CustomButton item in LeftPanel.Items)
            {
                if (item.Id == id)
                {
                    Clients[id].SetMap(model);
                    return;
                }
            }
        }

        public void AddMapElement(int id, ActorModel model)
        {
            foreach (CustomButton item in LeftPanel.Items)
            {
                if (item.Id == id)
                {
                    Clients[id].AddElement(model);
                    return;
                }
            }
        }

        public void RemoveMapElement(int id, double elementId)
        {
            foreach (CustomButton item in LeftPanel.Items)
            {
                if (item.Id == id)
                {
                    Clients[id].RemoveElement(elementId);
                    return;
                }
            }
        }

        public void Remove(int id)
        {
            int i = 0;
            while(LeftPanel.Items.GetItemAt(i) is CustomButton btn && btn.Id != id)
            {
                i++;
            }

            Clients.Remove(id);
            CustomButton _btn = LeftPanel.Items.GetItemAt(i) as CustomButton;

            Dispatcher.Invoke(() =>
            {
                if (_btn.IsSelected)
                {
                    MainFrame.Navigate(Home);
                }

                LeftPanel.Items.RemoveAt(i);
            });
        }
    }
}
