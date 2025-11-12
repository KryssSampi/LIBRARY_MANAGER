using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using LIBBRARY_MANAGER.Model;
using LIBBRARY_MANAGER.UI.Modules.BookCatalog.Config;
using LIBBRARY_MANAGER.Views.BookViews;
using static LIBBRARY_MANAGER.Model.Book;

namespace LIBBRARY_MANAGER.ViewModel.BookViewModels
{
    /// <summary>
    /// ViewModel pour l'affichage d'un livre (adaptateur Model → UI)
    /// </summary>
    public class BookViewModel : INotifyPropertyChanged
    {
        private readonly Book _book;

        #region Propriétés du modèle

        public long BookId => _book.BookId;
        public string Title => _book.Title;
        public string Author => _book.Author;
        public string ISBN => _book.ISBN;
        public string? Category => _book.Category;
        public string? Description => _book.Description;
        public DateTime? PublishDate => _book.PublishDate;
        public string? Publisher => _book.Publisher;
        public string? Language => _book.Language ?? "Français";
        public int Quantity => _book.Quantity;
        public bool IsAvailable => _book.IsAvailable;
        public DateTime DateAdded => _book.DateAdded;
        public string? CoverUrl => _book.CoverUrl;

        #endregion

        #region Propriétés calculées pour l'UI

        /// <summary>
        /// URL de la couverture avec fallback
        /// </summary>
        public string DisplayCoverUrl =>
            !string.IsNullOrEmpty(CoverUrl)
                ? CoverUrl
                : "pack://application:,,,/Assets/Images/default-book-cover.png";

        /// <summary>
        /// Catégorie formatée pour l'affichage
        /// </summary>
        public string DisplayCategory
        {
            get
            {
                if (string.IsNullOrEmpty(Category))
                    return "Non catégorisé";

                if (Enum.TryParse<CategoryAllowed>(Category, out var cat))
                {
                    var config = CategoryConfiguration.GetConfig(cat);
                    return config?.DisplayName ?? Category;
                }

                return Category;
            }
        }

        /// <summary>
        /// Date de publication formatée
        /// </summary>
        public string DisplayPublishDate =>
            PublishDate?.ToString("dd/MM/yyyy") ?? "Date inconnue";

        /// <summary>
        /// Année de publication (pour BookGridItem)
        /// </summary>
        public string PublishYear =>
            PublishDate?.Year.ToString() ?? "N/A";

        /// <summary>
        /// Date d'ajout formatée
        /// </summary>
        public string DisplayAddedDate => DateAdded.ToString("dd/MM/yyyy");

        /// <summary>
        /// Statut de disponibilité avec couleur
        /// </summary>
        public string AvailabilityText
        {
            get
            {
                if (Quantity == 0) return "Rupture de stock";
                if (Quantity <= 3) return $"Stock faible ({Quantity})";
                if (Quantity <= 10) return $"Disponible ({Quantity})";
                return $"En stock ({Quantity})";
            }
        }

        /// <summary>
        /// Date formatée pour BookDetailPanel
        /// </summary>
        public string PublishDateFormatted => DisplayPublishDate;

        /// <summary>
        /// Couleur selon disponibilité
        /// </summary>
        public SolidColorBrush AvailabilityColor
        {
            get
            {
                if (Quantity == 0)
                    return new SolidColorBrush(Color.FromRgb(244, 67, 54)); // Rouge
                if (Quantity <= 3)
                    return new SolidColorBrush(Color.FromRgb(255, 152, 0)); // Orange
                if (Quantity <= 10)
                    return new SolidColorBrush(Color.FromRgb(76, 175, 80)); // Vert
                return new SolidColorBrush(Color.FromRgb(33, 150, 243)); // Bleu
            }
        }

        /// <summary>
        /// Couleur de la catégorie
        /// </summary>
        public Brush CategoryColor
        {
            get
            {
                if (string.IsNullOrEmpty(Category))
                    return Brushes.Gray;

                if (Enum.TryParse<CategoryAllowed>(Category, out var cat))
                {
                    var config = CategoryConfiguration.GetConfig(cat);
                    return config?.IconColor ?? Brushes.Gray;
                }

                return Brushes.Gray;
            }
        }

        /// <summary>
        /// Badges visuels
        /// </summary>
        public bool IsNew => _book.IsNew;
        public bool IsLowStock => _book.IsLowStock;
        public bool IsPopular => Quantity < 5; // Logique simple

        #endregion

        #region Constructeur

        public BookViewModel(Book book)
        {
            _book = book ?? throw new ArgumentNullException(nameof(book));
        }

        #endregion

        #region Méthodes statiques de conversion

        /// <summary>
        /// Convertit un Book en BookListItem (mode liste)
        /// </summary>
        public static BookListItem GetListItem(Book book)
        {
            var vm = new BookViewModel(book);

            return new BookListItem
            {
                DataContext = vm,
                DisplayCoverUrl = vm.DisplayCoverUrl,
                Title = vm.Title,
                Author = vm.Author,
                DisplayCategory = vm.DisplayCategory,
                ISBN = vm.ISBN,
                Publisher = vm.Publisher ?? "Non renseigné",
                PublicationDate = vm.PublishDate ?? DateTime.MinValue,
                Language = vm.Language,
                Quantity = vm.Quantity,
                AvailabilityColor = vm.AvailabilityColor,
                AddedOn = vm.DateAdded,
                IsNew = vm.IsNew,
                IsPopular = vm.IsPopular,
                IsSelected = false
            };
        }

        /// <summary>
        /// Convertit un Book en BookGridItem (mode grille)
        /// </summary>
        public static BookGridItem GetGridItem(Book book)
        {
            var vm = new BookViewModel(book);

            return new BookGridItem
            {
                DataContext = vm
            };
        }

        #endregion

        #region Accès au modèle original

        /// <summary>
        /// Récupère le Book original
        /// </summary>
        public Book GetBook() => _book;

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Notifie tous les changements (après modification du modèle)
        /// </summary>
        public void RefreshAll()
        {
            OnPropertyChanged(nameof(Quantity));
            OnPropertyChanged(nameof(IsAvailable));
            OnPropertyChanged(nameof(AvailabilityText));
            OnPropertyChanged(nameof(AvailabilityColor));
            OnPropertyChanged(nameof(IsLowStock));
        }

        #endregion
    }
}