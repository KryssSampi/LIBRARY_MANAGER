using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace LIBBRARY_MANAGER.Views.BookViews
{
    public partial class BookListItem : UserControl
    {
        #region Dependency Properties

        // ✅ Commande pour afficher les détails
        public static readonly DependencyProperty ShowDetailCommandProperty =
            DependencyProperty.Register(
                nameof(ShowDetailCommand),
                typeof(ICommand),
                typeof(BookListItem),
                new PropertyMetadata(null));

        public ICommand ShowDetailCommand
        {
            get => (ICommand)GetValue(ShowDetailCommandProperty);
            set => SetValue(ShowDetailCommandProperty, value);
        }

        // ✅ Commande pour emprunter
        public static readonly DependencyProperty BorrowCommandProperty =
            DependencyProperty.Register(
                nameof(BorrowCommand),
                typeof(ICommand),
                typeof(BookListItem),
                new PropertyMetadata(null));

        public ICommand BorrowCommand
        {
            get => (ICommand)GetValue(BorrowCommandProperty);
            set => SetValue(BorrowCommandProperty, value);
        }

        // ✅ Propriétés d'affichage
        public string DisplayCoverUrl { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string DisplayCategory { get; set; }
        public string ISBN { get; set; }
        public string Publisher { get; set; }
        public DateTime PublicationDate { get; set; }
        public string Language { get; set; }
        public int Quantity { get; set; }
        public Brush AvailabilityColor { get; set; }
        public DateTime AddedOn { get; set; }
        public bool IsNew { get; set; }
        public bool IsPopular { get; set; }
        public bool IsSelected { get; set; }

        #endregion

        public BookListItem()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ✅ FIX: Handler pour le bouton Eye (Voir détails)
        /// </summary>
        private void EyeBtn_Click(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true; // Empêcher la propagation au bouton parent

            if (ShowDetailCommand != null && ShowDetailCommand.CanExecute(DataContext))
            {
                ShowDetailCommand.Execute(DataContext);
                System.Diagnostics.Debug.WriteLine($"[BookListItem] 👁️ Eye button clicked for: {Title}");
            }
        }

        /// <summary>
        /// ✅ FIX: Handler pour le bouton Borrow (Emprunter)
        /// </summary>
        private void BorrowBtn_Click(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true; // Empêcher la propagation au bouton parent

            if (BorrowCommand != null && BorrowCommand.CanExecute(DataContext))
            {
                BorrowCommand.Execute(DataContext);
                System.Diagnostics.Debug.WriteLine($"[BookListItem] 📚 Borrow button clicked for: {Title}");
            }
        }
    }
}