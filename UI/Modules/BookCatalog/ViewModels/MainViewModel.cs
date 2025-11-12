using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using LIBBRARY_MANAGER.Model;
using LIBBRARY_MANAGER.UI.Common.Items.MainView;
using LIBBRARY_MANAGER.UI.Modules.BookCatalog.Config;
using LIBBRARY_MANAGER.UI.Modules.BookCatalog.Core.Services;
using LIBBRARY_MANAGER.UI.Modules.Common.Interfaces;
using LIBBRARY_MANAGER.ViewModel.BookViewModels;
using LIBBRARY_MANAGER.Views.BookViews;
using static LIBBRARY_MANAGER.Model.Book;

namespace LIBBRARY_MANAGER.UI.Modules.BookCatalog.ViewModels
{
    public enum ViewMode
    {
        List,
        Grid
    }

    public class BookCatalogMainViewModel : INotifyPropertyChanged, ICatalogViewModel
    {
        #region === Services ===
        public readonly BookSearchService _searchService;
        public readonly BookFilterService _filterService;
        public readonly BookSortService _sortService;
        public readonly CategoryCountService _categoryCountService;
        #endregion

        #region === Collections ===
        private ObservableCollection<Book> _allBooks;
        private ObservableCollection<Book> _displayedBooks;
        private ObservableCollection<ColumnHeaderViewModel> _headers;
        private ObservableCollection<FilterLabelViewModel> _activeFilters;

        public ObservableCollection<Book> AllBooks
        {
            get => _allBooks;
            set { _allBooks = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Book> DisplayedBooks
        {
            get => _displayedBooks;
            set
            {
                _displayedBooks = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalResults));
                OnPropertyChanged(nameof(HasResults));
                OnPropertyChanged(nameof(DisplayedItems));
            }
        }

        public ObservableCollection<ColumnHeaderViewModel> Headers
        {
            get => _headers ??= GetDefaultHeaders();
            set { _headers = value; OnPropertyChanged(); }
        }

        public ObservableCollection<FilterLabelViewModel> ActiveFilters
        {
            get => _activeFilters ??= new ObservableCollection<FilterLabelViewModel>();
            set { _activeFilters = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasFilters)); }
        }

        public IEnumerable DisplayedItems => DisplayedBooks;
        #endregion

        #region === Propriétés ===
        private string _searchText = string.Empty;
        private CategoryAllowed? _currentCategory;
        private ViewMode _currentViewMode = ViewMode.List;
        private string _sortColumn = "Title";
        private bool _sortAscending = true;

        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); }
        }

        public int TotalResults => DisplayedBooks?.Count ?? 0;
        public bool HasResults => TotalResults > 0;
        public bool HasFilters => ActiveFilters?.Any() ?? false;

        ViewMode ICatalogViewModel._currentviewmode => _currentViewMode;

        public string CatalogTitle =>
            CurrentCategory.HasValue ? $"Catalogue - {CategoryDisplayName}" : "Catalogue de Livres";

        public string EmptyMessage =>
            !string.IsNullOrWhiteSpace(SearchText)
                ? $"Aucun livre trouvé pour '{SearchText}'"
                : CurrentCategory.HasValue
                    ? $"Aucun livre dans la catégorie {CategoryDisplayName}"
                    : "Aucun livre disponible";

        public CategoryAllowed? CurrentCategory
        {
            get => _currentCategory;
            set
            {
                _currentCategory = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CategoryDisplayName));
                OnPropertyChanged(nameof(CatalogTitle));
                OnPropertyChanged(nameof(EmptyMessage));
                ApplyFiltersAndSort();
            }
        }

        public string CategoryDisplayName =>
            !CurrentCategory.HasValue
                ? "Tous les livres"
                : CategoryConfiguration.GetConfig(CurrentCategory.Value)?.DisplayName
                    ?? CurrentCategory.Value.ToString();

        public bool IsListMode => _currentViewMode == ViewMode.List;
        public string ItemsName => "Livre";
        public bool IsListView => _currentViewMode == ViewMode.List;
        public bool IsGridView => _currentViewMode == ViewMode.Grid;
        public string CurrentViewMode => _currentViewMode.ToString();
        #endregion

        #region === Commandes ===
        public ICommand SearchCommand { get; private set; }
        public ICommand ClearSearchCommand { get; private set; }
        public ICommand ToggleViewModeCommand { get; private set; }
        public ICommand SortCommand { get; private set; }
        public ICommand RemoveFilterCommand { get; private set; }
        public ICommand ReloadCommand { get; private set; }
        public ICommand BackCommand { get; set; }
        public ICommand ShowBookDetailCommand { get; private set; }
        public ICommand ShowBorrowPanelCommand { get; private set; }
        #endregion

        #region === Events ===
        public event EventHandler<Book> BookDetailRequested;
        public event EventHandler<Book> BorrowRequested;
        public event EventHandler ViewModeChanged;
        #endregion

        #region === Constructeur ===
        public BookCatalogMainViewModel()
        {
            _searchService = new BookSearchService();
            _filterService = new BookFilterService();
            _sortService = new BookSortService();
            _categoryCountService = new CategoryCountService();

            AllBooks = new ObservableCollection<Book>();
            DisplayedBooks = new ObservableCollection<Book>();
            ActiveFilters = new ObservableCollection<FilterLabelViewModel>();

            Initializeheader();
            InitializeCommands();
        }
        #endregion

        #region === Initialisation ===
        public void Initialize()
        {
            try
            {
                LoadBooksFromDatabase();
                LogInfo($"✅ Catalogue initialisé avec {AllBooks.Count} livres depuis la base de données");
            }
            catch (Exception ex)
            {
                LogError("Erreur lors de l'initialisation", ex);
                MessageBox.Show($"Erreur de chargement:\n{ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Initializeheader()
        {
            Headers = GetDefaultHeaders();
            foreach (var header in Headers)
            {
                header.SortCommand = new RelayCommand(() => ExecuteSort(header.SortKey));
                header.SortBackCommand = new RelayCommand(() => ExecuteSort(header.SortKey));
            }
        }

        public void Cleanup()
        {
            AllBooks?.Clear();
            DisplayedBooks?.Clear();
            ActiveFilters?.Clear();
            LogInfo("Catalogue nettoyé");
        }

        /// <summary>
        /// ✅ CORRECTION : Charge les livres depuis App.LibraryDbContext
        /// </summary>
        private void LoadBooksFromDatabase()
        {
            try
            {
                var context = App.LibraryDbContext;
                var books = context.Books
                    .Where(b => !b.IsRemoved)
                    .OrderBy(b => b.Title)
                    .ToList();

                AllBooks = new ObservableCollection<Book>(books);
                DisplayedBooks = new ObservableCollection<Book>(books);

                LogInfo($"✅ {books.Count} livres chargés depuis la base de données");
            }
            catch (Exception ex)
            {
                LogError("Erreur chargement depuis DB", ex);
                AllBooks = new ObservableCollection<Book>();
                DisplayedBooks = new ObservableCollection<Book>();
            }
        }
        #endregion

        #region === Filtrage et Tri ===
        private void ApplyFiltersAndSort()
        {
            try
            {
                var result = AllBooks.AsEnumerable();

                // ✅ Filtre par catégorie
                if (CurrentCategory.HasValue && CurrentCategory != CategoryAllowed.Tous)
                {
                    result = _filterService.FilterByCategoryEnum(result, CurrentCategory.Value);
                }

                // ✅ Filtre par recherche
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    result = _searchService.Search(result, SearchText);
                }

                // ✅ Filtres actifs
                if (ActiveFilters.Any())
                {
                    result = _filterService.ApplyFilters(result, ActiveFilters);
                }

                // ✅ Tri
                result = _sortService.SortByColumnName(result, _sortColumn, _sortAscending);

                DisplayedBooks = new ObservableCollection<Book>(result);
                LogInfo($"✅ Filtrage terminé: {DisplayedBooks.Count} résultats");
            }
            catch (Exception ex)
            {
                LogError("Erreur lors du filtrage", ex);
            }
        }

        public static ObservableCollection<ColumnHeaderViewModel> GetDefaultHeaders()
        {
            return new ObservableCollection<ColumnHeaderViewModel>
            {
                new ColumnHeaderViewModel("Titre", allowFilter: false) { SortKey = "Title" },
                new ColumnHeaderViewModel("Auteur", allowFilter: false) { SortKey = "Author" },
                new ColumnHeaderViewModel("Catégorie", allowFilter: false) { SortKey = "Category" },
                new ColumnHeaderViewModel("ISBN", allowFilter: false) { SortKey = "ISBN" },
                new ColumnHeaderViewModel("Éditeur", allowFilter: false) { SortKey = "Publisher" },
                new ColumnHeaderViewModel("Publication", allowFilter: false) { SortKey = "PublishDate" },
                new ColumnHeaderViewModel("Langue", allowFilter: false) { SortKey = "Language" },
                new ColumnHeaderViewModel("Quantité", allowFilter: false) { SortKey = "Quantity" },
                new ColumnHeaderViewModel("Ajouté le", allowFilter: false) { SortKey = "DateAdded" }
            };
        }
        #endregion

        #region === Command Handlers ===
        private void InitializeCommands()
        {
            SearchCommand = new RelayCommand(ExecuteSearch);
            ClearSearchCommand = new RelayCommand(ExecuteClearSearch);
            ToggleViewModeCommand = new RelayCommand(ExecuteToggleViewMode);
            SortCommand = new RelayCommand<string>(ExecuteSort);
            RemoveFilterCommand = new RelayCommand<object>(ExecuteRemoveFilter);
            ReloadCommand = new RelayCommand(ExecuteReload);
            BackCommand = new RelayCommand(() => LogInfo("Retour au menu principal"));
            ShowBookDetailCommand = new RelayCommand<object>(ExecuteShowBookDetail);
            ShowBorrowPanelCommand = new RelayCommand<object>(ExecuteShowBorrowPanel);
        }

        private void ExecuteSearch()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                ExecuteClearSearch();
                return;
            }
            ApplyFiltersAndSort();
            LogInfo($"🔍 Recherche '{SearchText}': {DisplayedBooks.Count} résultat(s)");
        }

        public void ExecuteClearSearch()
        {
            SearchText = string.Empty;
            ApplyFiltersAndSort();
            LogInfo("🔍 Recherche effacée");
        }

        private void ExecuteToggleViewMode()
        {
            _currentViewMode = _currentViewMode == ViewMode.List ? ViewMode.Grid : ViewMode.List;
            OnPropertyChanged(nameof(CurrentViewMode));
            OnPropertyChanged(nameof(IsListMode));
            OnPropertyChanged(nameof(IsGridView));
            ViewModeChanged?.Invoke(this, EventArgs.Empty);
            LogInfo($"🔄 Mode d'affichage: {_currentViewMode}");
        }

        private void ExecuteSort(string columnName)
        {
            if (string.IsNullOrWhiteSpace(columnName)) return;
            _sortAscending = _sortColumn == columnName ? !_sortAscending : true;
            _sortColumn = columnName;
            ApplyFiltersAndSort();
            LogInfo($"📊 Tri: {_sortColumn} ({(_sortAscending ? "ASC" : "DESC")})");
        }

        private void ExecuteRemoveFilter(object filterId)
        {
            var filter = ActiveFilters.FirstOrDefault(f => f.Id == filterId);
            if (filter == null) return;
            ActiveFilters.Remove(filter);
            ApplyFiltersAndSort();
            LogInfo($"🗑️ Filtre supprimé: {filter.Value}");
        }

        private void ExecuteReload()
        {
            LoadBooksFromDatabase();
            ActiveFilters.Clear();
            SearchText = string.Empty;
            CurrentCategory = null;
            LogInfo("🔄 Catalogue rechargé depuis la base de données");
        }

        private void ExecuteShowBookDetail(object parameter)
        {
            Book book = null;
            if (parameter is Book b) book = b;
            else if (parameter is BookViewModel vm) book = vm.GetBook();

            if (book != null)
            {
                LogInfo($"📖 Affichage détails: {book.Title}");
                BookDetailRequested?.Invoke(this, book);
            }
        }

        private void ExecuteShowBorrowPanel(object parameter)
        {
            Book book = null;
            if (parameter is Book b) book = b;
            else if (parameter is BookViewModel vm) book = vm.GetBook();

            if (book != null)
            {
                LogInfo($"📚 Emprunt demandé: {book.Title}");
                BorrowRequested?.Invoke(this, book);
            }
        }
        #endregion

        #region === Génération d'items ===
        public UIElement CreateDisplayItem(object item)
        {
            if (item is not Book book)
            {
                LogWarning($"⚠️ Item n'est pas un Book: {item?.GetType().Name}");
                return null;
            }

            try
            {
                var bookViewModel = new BookViewModel(book);

                if (_currentViewMode == ViewMode.Grid)
                {
                    var gridItem = BookViewModel.GetGridItem(book);
                    gridItem.ShowDetailBtnClick = ShowBookDetailCommand;
                    gridItem.BorrowBtnClick = ShowBorrowPanelCommand;
                    gridItem.DataContext = bookViewModel;
                    return gridItem;
                }
                else
                {
                    var listItem = BookViewModel.GetListItem(book);
                    listItem.ShowDetailCommand = ShowBookDetailCommand;
                    listItem.BorrowCommand = ShowBorrowPanelCommand;
                    listItem.DataContext = bookViewModel;
                    return listItem;
                }
            }
            catch (Exception ex)
            {
                LogError($"❌ Erreur création item pour {book.Title}", ex);
                return new TextBlock
                {
                    Text = $"{book.Title} — {book.Author}",
                    Margin = new Thickness(8, 4, 8, 4),
                    FontSize = 14
                };
            }
        }
        #endregion

        #region === Logging ===
        private void LogInfo(string message)
            => System.Diagnostics.Debug.WriteLine($"[INFO] [BookCatalogVM] {message}");

        private void LogWarning(string message)
            => System.Diagnostics.Debug.WriteLine($"[WARNING] [BookCatalogVM] {message}");

        private void LogError(string message, Exception ex)
            => System.Diagnostics.Debug.WriteLine($"[ERROR] [BookCatalogVM] {message}: {ex.Message}");
        #endregion

        #region === INotifyPropertyChanged ===
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}