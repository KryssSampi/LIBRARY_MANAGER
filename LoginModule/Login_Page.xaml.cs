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
using System.Windows.Shapes;
using LIBBRARY_MANAGER.Model;

namespace LIBBRARY_MANAGER.LoginModule
{
    /// <summary>
    /// Logique d'interaction pour Login_Page.xaml
    /// </summary>
    public partial class Login_Page : Window
    {
        public  Login_Page()
        {
            InitializeComponent();
            LoginPage.ThenLoginCommand = new RelayCommand(Close);
            CloseBtn.Command = new RelayCommand(Close);
            ReduceBtn.Command = new RelayCommand(_ => { this.WindowState = WindowState.Minimized; });
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Permet de déplacer la fenêtre quand on clique n'importe où dans le contenu
            if (e.ButtonState == MouseButtonState.Pressed)
                this.DragMove();
        }
        
    }
}
