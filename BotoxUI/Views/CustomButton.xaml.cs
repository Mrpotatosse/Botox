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
    /// Logique d'interaction pour CustomButton.xaml
    /// </summary>
    public partial class CustomButton : UserControl
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("TextProperty", typeof(string), typeof(CustomButton));
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelectedProperty", typeof(bool), typeof(CustomButton));
        public static readonly DependencyProperty SelectionColorProperty = DependencyProperty.Register("SelectionColorProperty", typeof(SolidColorBrush), typeof(CustomButton));

        public SolidColorBrush SelectionColor
        {
            get
            {
                return IsSelectedDock.Background as SolidColorBrush;
            }
            set
            {
                Dispatcher.Invoke(() => IsSelectedDock.Background = value);
            }
        }

        private bool _isSelected { get; set; } = false;
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                Dispatcher.Invoke(() =>
                {
                    _isSelected = value;

                    if (value)
                    {
                        IsSelectedDock.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        IsSelectedDock.Visibility = Visibility.Collapsed;
                    }
                });
            }
        }

        public string Text
        {
            get
            {
                return ContentText.Text;
            }
            set
            {
                Dispatcher.Invoke(() => ContentText.Text = value);
            }
        }

        public int Id { get; set; } 

        public CustomButton()
        {
            InitializeComponent();
        }
    }
}
