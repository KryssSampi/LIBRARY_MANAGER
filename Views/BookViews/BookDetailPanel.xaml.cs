using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LIBBRARY_MANAGER.UI.Common.Items;
using LIBBRARY_MANAGER.Views.SubscriberView;

namespace LIBBRARY_MANAGER.Views.BookViews
{

    public partial class BookDetailPanel : UserControl
    {
        // ✅ Correction du owner type
        public static readonly DependencyProperty BorrowBtnClickProperty =
            DependencyProperty.Register(nameof(BorrowBtnClick), typeof(ICommand), typeof(BookDetailPanel));
        public ICommand CloseCommand
        {
            get => (ICommand)GetValue(CloseCommandProperty);
            set => SetValue(CloseCommandProperty, value);
        }
        public static readonly DependencyProperty CloseCommandProperty =
            DependencyProperty.Register(nameof(CloseCommand), typeof(ICommand), typeof(BookDetailPanel), new PropertyMetadata(null));


        public ICommand BorrowBtnClick
        {
            get => (ICommand)GetValue(BorrowBtnClickProperty);
            set => SetValue(BorrowBtnClickProperty, value);
        }

        public BookDetailPanel()
        {
            InitializeComponent();
            Loaded += (_, __) =>
            {
                // branchement propre après chargement complet du visuel
                CloseBtn.MouseLeftButtonDown += (s, e) => OnCloseRequested();
            };
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

