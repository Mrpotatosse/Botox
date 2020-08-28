using BotoxSharedModel.Models.Actors;
using BotoxSharedModel.Models.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace BotoxUI.Views
{
    /// <summary>
    /// Logique d'interaction pour ClientPage.xaml
    /// </summary>
    public partial class ClientPage : Page
    {
        public ClientPage()
        {
            InitializeComponent();
        }

        private PlayerModel Model { get; set; }

        public bool HasModel => Model != null;

        public void SetCharacter(PlayerModel model)
        {
            Dispatcher.Invoke(() =>
            {
                Model = model;

                if (model is null)
                {
                    IdTxt.Text = "";
                    MapIdTxt.Text = "";
                    NameTxt.Text = "";
                    LevelTxt.Text = "";
                }
                else
                {
                    IdTxt.Text = $"{model.Id}";
                    MapIdTxt.Text = $"{model.MapId}";
                    NameTxt.Text = $"{model.Name}";
                    LevelTxt.Text = $"{model.Level}";
                }
            });
        }

        public void SetMap(MapModel model)
        {
            MapList.Items.Clear();

            foreach(ActorModel actor in model.Actors)
            {
                if(actor.Id == Model.Id)
                {
                    SetCharacter(actor as PlayerModel);
                }
                else
                {
                    MapList.Items.Add(actor);
                }                
            }
        }

        public void RemoveElement(double id)
        {
            int i = 0;
            while (i < MapList.Items.Count && MapList.Items[i] is ActorModel model && model.Id != id)
            {
                i++;
            }

            if(i < MapList.Items.Count && (MapList.Items[i] as ActorModel).Id == id)
            {
                MapList.Items.RemoveAt(i);
            }
        }

        public void AddElement(ActorModel model)
        {
            foreach (ActorModel actor in MapList.Items)
            {
                if (actor.Id == model.Id)
                    return;
            }

            MapList.Items.Add(model);
        }
    }
}
