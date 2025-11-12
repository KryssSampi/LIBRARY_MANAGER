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

namespace LIBBRARY_MANAGER.UI.Common.Items.MainView
{
    /// <summary>
    /// Logique d'interaction pour StatusBar.xaml
    /// </summary>
    public partial class StatusBar : UserControl
    {
        public static readonly DependencyProperty TotalBooksProperty =
            DependencyProperty.Register(nameof(TotalBooks), typeof(int), typeof(StatusBar), new PropertyMetadata(0));

        public static readonly DependencyProperty ViewModeProperty =
            DependencyProperty.Register(nameof(ViewMode), typeof(string), typeof(StatusBar), new PropertyMetadata("Liste"));

        public int TotalBooks
        {
            get => (int)GetValue(TotalBooksProperty);
            set => SetValue(TotalBooksProperty, value);
        }

        public string ViewMode
        {
            get => (string)GetValue(ViewModeProperty);
            set => SetValue(ViewModeProperty, value);
        }

        public static readonly DependencyProperty ItemsNameProperty =
            DependencyProperty.Register(nameof(ItemsName), typeof(string), typeof(StatusBar), new PropertyMetadata("Liste"));
        public string ItemsName
        {
            get => (string)GetValue(ItemsNameProperty);
            set => SetValue(ItemsNameProperty, value);
        }
        public StatusBar()
        {
            InitializeComponent();
        }
    }
}