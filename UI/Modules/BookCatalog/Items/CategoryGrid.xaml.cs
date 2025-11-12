using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LIBBRARY_MANAGER.Model;
using LIBBRARY_MANAGER.UI.Modules.BookCatalog.Config;
using static LIBBRARY_MANAGER.Model.Book;

namespace LIBBRARY_MANAGER.UI.Modules.BookCatalog.Items
{
    public partial class CategoryGrid : UserControl
    {
        #region Fields

        private Dictionary<CategoryAllowed, CategoryTile> _tiles;
        private Dictionary<CategoryAllowed, int> _categoryCounts;

        #endregion

        #region Events

        public event Action<CategoryAllowed> OnCategorySelected;

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register(
                nameof(Columns),
                typeof(int),
                typeof(CategoryGrid),
                new PropertyMetadata(4, OnColumnsChanged));

        public int Columns
        {
            get => (int)GetValue(ColumnsProperty);
            set => SetValue(ColumnsProperty, value);
        }

        public static readonly DependencyProperty RowsProperty =
            DependencyProperty.Register(
                nameof(Rows),
                typeof(int?),
                typeof(CategoryGrid),
                new PropertyMetadata(null, OnRowsChanged));

        public int? Rows
        {
            get => (int?)GetValue(RowsProperty);
            set => SetValue(RowsProperty, value);
        }

        public static readonly DependencyProperty ShowOnlyWithBooksProperty =
            DependencyProperty.Register(
                nameof(ShowOnlyWithBooks),
                typeof(bool),
                typeof(CategoryGrid),
                new PropertyMetadata(false, OnShowOnlyWithBooksChanged));

        public bool ShowOnlyWithBooks
        {
            get => (bool)GetValue(ShowOnlyWithBooksProperty);
            set => SetValue(ShowOnlyWithBooksProperty, value);
        }

        #endregion

        #region Constructor

        public CategoryGrid()
        {
            InitializeComponent();
            _tiles = new Dictionary<CategoryAllowed, CategoryTile>();
            _categoryCounts = new Dictionary<CategoryAllowed, int>();

            Loaded += CategoryGrid_Loaded;
        }

        #endregion

        #region Initialization

        private void CategoryGrid_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCategories();
        }

        public void LoadCategories()
        {
            CategoriesContainer.Children.Clear();
            _tiles.Clear();

            var configs = CategoryConfiguration.GetAllConfigs().ToList();

            // Trier pour mettre "Tous" en premier
            configs = configs.OrderBy(c => c.Category == CategoryAllowed.Tous ? 0 : 1)
                             .ThenBy(c => c.DisplayName)
                             .ToList();

            // Filtrer si nécessaire
            if (ShowOnlyWithBooks)
            {
                configs = configs.Where(c =>
                    c.Category == CategoryAllowed.Tous ||
                    GetCategoryCount(c.Category) > 0
                ).ToList();
            }

            foreach (var config in configs)
            {
                CreateTile(config);
            }

            UpdateGridLayout();
        }

        #endregion

        #region Tile Creation

        private void CreateTile(CategoryConfig config)
        {
            var tile = new CategoryTile();
            tile.ConfigureFromCategoryConfig(config);

            int count = GetCategoryCount(config.Category);
            tile.UpdateBookCount(count);

            tile.OnCategoryClicked += HandleCategoryClick;

            _tiles[config.Category] = tile;
            CategoriesContainer.Children.Add(tile);
        }

        #endregion

        #region Event Handling

        private void HandleCategoryClick(CategoryAllowed category)
        {
            OnCategorySelected?.Invoke(category);
            System.Diagnostics.Debug.WriteLine($"[CategoryGrid] 📂 Catégorie sélectionnée: {category}");
        }

        #endregion

        #region Public Methods - Data Management

        public void UpdateCategoryCounts(Dictionary<CategoryAllowed, int> counts)
        {
            _categoryCounts = counts ?? new Dictionary<CategoryAllowed, int>();

            foreach (var kvp in _tiles)
            {
                int count = GetCategoryCount(kvp.Key);
                kvp.Value.UpdateBookCount(count);
            }

            if (ShowOnlyWithBooks)
            {
                LoadCategories();
            }

            System.Diagnostics.Debug.WriteLine($"[CategoryGrid] ✅ Compteurs mis à jour: {_categoryCounts.Count} catégories");
        }

        public void UpdateCategoryCount(CategoryAllowed category, int count)
        {
            _categoryCounts[category] = count;

            if (_tiles.TryGetValue(category, out var tile))
            {
                tile.UpdateBookCount(count);
            }

            if (ShowOnlyWithBooks)
            {
                LoadCategories();
            }
        }

        /// <summary>
        /// ✅ CORRECTION : Charge les compteurs depuis App.LibraryDbContext
        /// </summary>
        public void LoadCountsFromDatabase()
        {
            try
            {
                var context = App.LibraryDbContext;

                var books = context.Books
                    .Where(b => !b.IsRemoved)
                    .ToList();

                var counts = new Dictionary<CategoryAllowed, int>();

                foreach (CategoryAllowed category in Enum.GetValues(typeof(CategoryAllowed)))
                {
                    if (category == CategoryAllowed.Tous)
                    {
                        counts[category] = books.Count;
                    }
                    else
                    {
                        var categoryName = category.ToString();
                        counts[category] = books.Count(b =>
                            b.Category?.Equals(categoryName, StringComparison.OrdinalIgnoreCase) ?? false
                        );
                    }
                }

                UpdateCategoryCounts(counts);

                System.Diagnostics.Debug.WriteLine($"[CategoryGrid] ✅ {books.Count} livres chargés, {counts.Count} catégories comptées");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CategoryGrid] ❌ Erreur chargement DB: {ex.Message}");

                // Fallback : compteurs vides
                var emptyCounts = new Dictionary<CategoryAllowed, int>();
                foreach (CategoryAllowed category in Enum.GetValues(typeof(CategoryAllowed)))
                {
                    emptyCounts[category] = 0;
                }
                UpdateCategoryCounts(emptyCounts);
            }
        }

        #endregion

        #region Private Helpers

        private int GetCategoryCount(CategoryAllowed category)
        {
            return _categoryCounts.TryGetValue(category, out int count) ? count : 0;
        }

        private void UpdateGridLayout()
        {
            CategoriesContainer.Columns = Columns;

            if (Rows.HasValue)
            {
                CategoriesContainer.Rows = Rows.Value;
            }
            else
            {
                int totalTiles = CategoriesContainer.Children.Count;
                CategoriesContainer.Rows = (int)Math.Ceiling((double)totalTiles / Columns);
            }
        }

        #endregion

        #region Dependency Property Changed Handlers

        private static void OnColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CategoryGrid grid)
            {
                grid.UpdateGridLayout();
            }
        }

        private static void OnRowsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CategoryGrid grid)
            {
                grid.UpdateGridLayout();
            }
        }

        private static void OnShowOnlyWithBooksChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CategoryGrid grid && grid.IsLoaded)
            {
                grid.LoadCategories();
            }
        }

        #endregion

        #region Public Utility Methods

        public IEnumerable<CategoryAllowed> GetDisplayedCategories()
        {
            return _tiles.Keys;
        }

        public bool IsCategoryDisplayed(CategoryAllowed category)
        {
            return _tiles.ContainsKey(category);
        }

        public int GetTotalBookCount()
        {
            return _categoryCounts.Values.Sum();
        }

        public void Refresh()
        {
            LoadCountsFromDatabase();
        }

        #endregion
    }
}