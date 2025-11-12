using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using LIBBRARY_MANAGER.Data;
using LIBBRARY_MANAGER.Model;
using LIBBRARY_MANAGER.UI.Common.Items;
using LIBBRARY_MANAGER.Views.SubscriberView;
using Microsoft.EntityFrameworkCore;

namespace LIBBRARY_MANAGER.Views.LoanViews
{

    public partial class LoanReturnPanel : UserControl
    {
        public ICommand? ReturnCommand
        {
            get => (ICommand)GetValue(ReturnCommandProperty);
            set => SetValue(ReturnCommandProperty, value);
        }

        public static readonly DependencyProperty ReturnCommandProperty =
            DependencyProperty.Register(nameof(ReturnCommand), typeof(ICommand), typeof(LoanReturnPanel));

        public ICommand CloseCommand
        {
            get => (ICommand)GetValue(CloseCommandProperty);
            set => SetValue(CloseCommandProperty, value);
        }
        public static readonly DependencyProperty CloseCommandProperty =
            DependencyProperty.Register(nameof(CloseCommand), typeof(ICommand), typeof(LoanReturnPanel), new PropertyMetadata(null));

        public LoanReturnPanel()
        {
            InitializeComponent();
            // Optionnel : attach events for buttons if needed directly in code-behind
            CloseBtn.MouseLeftButtonDown += (s, e) => OnCloseRequested();
        }

        // Event for Footer close button
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            OnCloseRequested();
        }

        // Permet de tout fermer via l'event, et de brancher la commande CloseCommand (si bindée)
        private void OnCloseRequested()
        {
            if (CloseCommand != null && CloseCommand.CanExecute(null))
            {
                CloseCommand.Execute(null);
            }
        }

        // Accès aux éléments visuels clés
        public Button FooterCloseButton => FindName("FooterCloseBtn") as Button;
        public UI.Common.Items.IconButton HeaderCloseButton => CloseBtn;
        // Et ainsi de suite si tu veux exposer d'autres éléments critiques.
    }

    }
