using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace LIBBRARY_MANAGER.UI.Modules.BookCatalog.Items
{
    /// <summary>
    /// Barre de recherche réutilisable
    /// ✅ CORRIGÉ : Event OnSearchRequested avec signature compatible
    /// </summary>
    public partial class SearchBar : UserControl
    {
        #region Dependency Properties

        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register(
                nameof(Placeholder),
                typeof(string),
                typeof(SearchBar),
                new PropertyMetadata("Rechercher un livre..."));

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public static readonly DependencyProperty CardBackgroundProperty =
            DependencyProperty.Register(
                nameof(CardBackground),
                typeof(Brush),
                typeof(SearchBar),
                new PropertyMetadata(Brushes.White));

        public Brush CardBackground
        {
            get => (Brush)GetValue(CardBackgroundProperty);
            set => SetValue(CardBackgroundProperty, value);
        }

        public static readonly DependencyProperty ShadowColorProperty =
            DependencyProperty.Register(
                nameof(ShadowColor),
                typeof(Color),
                typeof(SearchBar),
                new PropertyMetadata(Colors.Blue));

        public Color ShadowColor
        {
            get => (Color)GetValue(ShadowColorProperty);
            set => SetValue(ShadowColorProperty, value);
        }

        public static readonly DependencyProperty ButtonColorProperty =
            DependencyProperty.Register(
                nameof(ButtonColor),
                typeof(Brush),
                typeof(SearchBar),
                new PropertyMetadata(new SolidColorBrush(Colors.Blue)));

        public Brush ButtonColor
        {
            get => (Brush)GetValue(ButtonColorProperty);
            set => SetValue(ButtonColorProperty, value);
        }

        public static readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register(
                nameof(IconSize),
                typeof(double),
                typeof(SearchBar),
                new PropertyMetadata(24.0));

        public double IconSize
        {
            get => (double)GetValue(IconSizeProperty);
            set => SetValue(IconSizeProperty, value);
        }

        #endregion

        #region Events

        /// <summary>
        ///  Event avec signature Action<string>
        /// </summary>
        public event Action<string> OnSearchRequested;

        #endregion

        #region Constructor

        public SearchBar()
        {
            InitializeComponent();

            // ✅ Connecter l'event au bouton de recherche
            Loaded += (s, e) =>
            {
                // Trouver le bouton de recherche dans le template
                var searchButton = this.FindName("SearchButton") as UI.Common.Items.IconButton;
                if (searchButton != null)
                {
                    searchButton.MouseLeftButtonDown += SearchButton_Click;
                }
            };
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Obtient le texte de recherche
        /// </summary>
        public string GetSearchText()
        {
            return SearchBox?.Text?.Trim() ?? string.Empty;
        }

        /// <summary>
        /// Définit le texte de recherche
        /// </summary>
        public void SetSearchText(string text)
        {
            if (SearchBox != null)
            {
                SearchBox.Text = text;
            }
        }

        /// <summary>
        /// Efface le texte de recherche
        /// </summary>
        public void ClearSearch()
        {
            SetSearchText(string.Empty);
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// ✅ CORRECTION : Handler compatible avec KeyDown
        /// </summary>
        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TriggerSearch();
                e.Handled = true;
            }
        }

        /// <summary>
        /// ✅ CORRECTION : Handler compatible avec Click
        /// </summary>
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            TriggerSearch();
        }

        /// <summary>
        /// Déclenche la recherche
        /// </summary>
        private void TriggerSearch()
        {
            var searchText = GetSearchText();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                OnSearchRequested?.Invoke(searchText);
                System.Diagnostics.Debug.WriteLine($"[SearchBar] 🔍 Recherche déclenchée: '{searchText}'");
            }
        }

        #endregion
    }
}