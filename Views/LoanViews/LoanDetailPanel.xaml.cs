using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LIBBRARY_MANAGER.Data;
using LIBBRARY_MANAGER.Model;
using Microsoft.EntityFrameworkCore;
using System.Windows.Media;

namespace LIBBRARY_MANAGER.Views.LoanViews
{
    public partial class LoanDetailPanel : UserControl
    {
        #region Dependency Properties

        public static readonly DependencyProperty CloseCommandProperty =
            DependencyProperty.Register(
                nameof(CloseCommand),
                typeof(ICommand),
                typeof(LoanDetailPanel),
                new PropertyMetadata(null));

        public ICommand CloseCommand
        {
            get => (ICommand)GetValue(CloseCommandProperty);
            set => SetValue(CloseCommandProperty, value);
        }

        public static readonly DependencyProperty ReturnCommandProperty =
            DependencyProperty.Register(
                nameof(ReturnCommand),
                typeof(ICommand),
                typeof(LoanDetailPanel),
                new PropertyMetadata(null));

        public ICommand ReturnCommand
        {
            get => (ICommand)GetValue(ReturnCommandProperty);
            set => SetValue(ReturnCommandProperty, value);
        }

        #endregion

        #region Constructeur

        public LoanDetailPanel()
        {
            InitializeComponent();
            CloseBtn.MouseLeftButtonDown += (s, e) => OnCloseRequested();
        }

        #endregion

        #region Event Handlers

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            OnCloseRequested();
        }

        private void OnCloseRequested()
        {
            if (CloseCommand != null && CloseCommand.CanExecute(null))
            {
                CloseCommand.Execute(null);
            }
        }

        #endregion
    }

    }
