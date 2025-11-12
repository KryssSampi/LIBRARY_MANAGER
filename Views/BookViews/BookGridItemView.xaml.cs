using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LIBBRARY_MANAGER.UI.Common.Items;

namespace LIBBRARY_MANAGER.Views.BookViews
{
    public partial class BookGridItem : UserControl
    {
        // ✅ DependencyProperties pour les commandes
        public static readonly DependencyProperty BorrowBtnClickProperty =
            DependencyProperty.Register(nameof(BorrowBtnClick), typeof(ICommand), typeof(BookGridItem));

        public static readonly DependencyProperty ShowDetailBtnClickProperty =
            DependencyProperty.Register(nameof(ShowDetailBtnClick), typeof(ICommand), typeof(BookGridItem));

        public ICommand BorrowBtnClick
        {
            get => (ICommand)GetValue(BorrowBtnClickProperty);
            set => SetValue(BorrowBtnClickProperty, value);
        }

        public ICommand ShowDetailBtnClick
        {
            get => (ICommand)GetValue(ShowDetailBtnClickProperty);
            set => SetValue(ShowDetailBtnClickProperty, value);
        }

        public BookGridItem()
        {
            InitializeComponent();

            // ✅ CORRECTION: Gérer le clic sur le bouton menu pour ouvrir le popup
            MenuButton.AddHandler(IconButton.ClickEvent, new RoutedEventHandler(OnMenuButtonClick));
        }

        private void BookGridItem_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Empêcher la propagation de l'événement au bouton parent
            e.Handled = true;

            // Ouvrir le popup
            ActionMenu.IsOpen = true;

            System.Diagnostics.Debug.WriteLine("[BookGridItem] Menu contextuel ouvert");
        }

        private void OnMenuButtonClick(object sender, RoutedEventArgs e)
        {
            // Empêcher la propagation de l'événement au bouton parent
            e.Handled = true;

            // Ouvrir le popup
            ActionMenu.IsOpen = true;

            System.Diagnostics.Debug.WriteLine("[BookGridItem] Menu contextuel ouvert");
        }



    }
}