using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using GalaSoft.MvvmLight.CommandWpf;
using LIBBRARY_MANAGER.Model;
using LIBBRARY_MANAGER.UI.Modules.Common.Interfaces;
using LIBBRARY_MANAGER.UI.Modules.BookCatalog.ViewModels;
using LIBBRARY_MANAGER.UI.Modules.SubscriberCatalog.ViewModels;
using LIBBRARY_MANAGER.ViewModel.BookViewModels;
using LIBBRARY_MANAGER.Views.BookViews;
using LIBBRARY_MANAGER.Views.SubscriberView;
using LIBBRARY_MANAGER.ViewModel.SubscribersViewModel;

namespace LIBBRARY_MANAGER.UI.Modules.Common.Views
{
    public partial class GenericMainView : UserControl
    {
        #region Propriétés

        public ICatalogViewModel ViewModel { get; private set; }

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty NavigationForegroundProperty =
            DependencyProperty.Register(
                nameof(NavigationForeground),
                typeof(Brush),
                typeof(GenericMainView),
                new PropertyMetadata(new SolidColorBrush(Color.FromRgb(21, 101, 192))));

        public Brush NavigationForeground
        {
            get => (Brush)GetValue(NavigationForegroundProperty);
            set => SetValue(NavigationForegroundProperty, value);
        }

        public static readonly DependencyProperty ShowColumnHeadersProperty =
            DependencyProperty.Register(
                nameof(ShowColumnHeaders),
                typeof(bool),
                typeof(GenericMainView),
                new PropertyMetadata(true));

        public bool ShowColumnHeaders
        {
            get => (bool)GetValue(ShowColumnHeadersProperty);
            set => SetValue(ShowColumnHeadersProperty, value);
        }

        #endregion

        #region Constructeur

        public GenericMainView(ICatalogViewModel viewModel)
        {
            ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            DataContext = ViewModel;

            InitializeComponent();

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;

            LogInfo($"GenericMainView créé pour '{ViewModel.CatalogTitle}'");
        }

        #endregion

        #region Cycle de vie

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LogInfo("OnLoaded démarré");

                ViewModel?.Initialize();
                ConfigureComponents();
                SubscribeToEvents();
                UpdateDisplay();
                UpdateColumnHeadersVisibility();

                // ✅ FIX: Synchronisation initiale
                SyncSearchAndResults();


                LogInfo($"{ViewModel.CatalogTitle} chargé avec succès");
            }
            catch (Exception ex)
            {
                LogError("Erreur lors du chargement", ex);
                ShowError("Erreur lors du chargement de la vue", ex);
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                UnsubscribeFromEvents();
                ViewModel?.Cleanup();
                LogInfo($"{ViewModel.CatalogTitle} déchargé");
            }
            catch (Exception ex)
            {
                LogError("Erreur lors du déchargement", ex);
            }
        }

        #endregion

        #region Configuration

        private void ConfigureComponents()
        {
            try
            {
                // ✅ Configuration du DataContext pour chaque composant
                if (NavigationBar_ != null)
                {
                    NavigationBar_.DataContext = ViewModel;
                    NavigationBar_.ItemsForeground = NavigationForeground;
                    NavigationBar_.PanelBackground = Brushes.White;

                    // ✅ FIX: Écouter les changements de texte en temps réel
                    NavigationBar_._SearchTextBox.TextChanged += SearchTextBox_TextChanged;

                    // ✅ Connecter la commande ToggleView
                    NavigationBar_.ToggleViewCommand = ViewModel.ToggleViewModeCommand;
                }

                if (ActiveFiltersPanel_ != null)
                {
                    ActiveFiltersPanel_.DataContext = ViewModel;
                    ActiveFiltersPanel_.Filters = ViewModel.ActiveFilters;
                    ActiveFiltersPanel_.CloseFilterCommand = ViewModel.RemoveFilterCommand;

                    // ✅ FIX: Connecter la commande de reset
                    ActiveFiltersPanel_.ResetFilterCommand = new RelayCommand(() =>
                    {
                        ViewModel.ActiveFilters.Clear();
                        ActiveFiltersPanel_.IsPanelVisible = false;
                        LogInfo("Tous les filtres ont été réinitialisés");
                    });
                }

                if (SearchResultMessage_ != null)
                {
                    SearchResultMessage_.DataContext = ViewModel;
                    SearchResultMessage_.Foreground = NavigationForeground;
                }

                if (StatusBar_ != null)
                {
                    StatusBar_.DataContext = ViewModel;
                    StatusBar_.ViewMode = ViewModel.CurrentViewMode;
                }

                if (ColumnHeaderPanel_ != null)
                {
                    ColumnHeaderPanel_.DataContext = ViewModel;
                    ColumnHeaderPanel_.Headers = ViewModel.Headers;
                    ColumnHeaderPanel_.IsListMode = ViewModel.IsListMode;
                }

                LogInfo("Composants configurés");
            }
            catch (Exception ex)
            {
                LogError("Erreur configuration composants", ex);
            }
        }

        #endregion

        #region Abonnements

        // ✅ FIX: Synchronisation complète SearchText → Message → ResultCount
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var searchText = NavigationBar_._SearchTextBox.Text;

                // Mettre à jour le ViewModel
                ViewModel.SearchText = searchText;

                // Synchroniser immédiatement le message
                SyncSearchAndResults();

                LogInfo($"Recherche synchronisée: '{searchText}'");
            }
            catch (Exception ex)
            {
                LogError("Erreur synchronisation recherche", ex);
            }
        }

        // ✅ NOUVELLE MÉTHODE: Synchronisation centralisée
        private void SyncSearchAndResults()
        {
            if (SearchResultMessage_ == null || ItemsGrid == null) return;

            try
            {
                // Compter les enfants réels de ItemsGrid
                int actualCount = ItemsGrid.Children.Count;

                // Exclure le message "vide" s'il existe
                var hasEmptyMessage = ItemsGrid.Children.OfType<TextBlock>()
                    .Any(tb => tb.Text == ViewModel.EmptyMessage);

                if (hasEmptyMessage)
                    actualCount = 0;

                // Mettre à jour le message de résultats
                SearchResultMessage_.Message = NavigationBar_._SearchTextBox.Text;
                SearchResultMessage_.ResultCount = actualCount;

                // Afficher/masquer selon si recherche active
                SearchResultMessage_.IsVisible_ = !string.IsNullOrWhiteSpace(ViewModel.SearchText);

                LogInfo($"✅ Sync: Message='{ViewModel.SearchText}', Count={actualCount}, Visible={SearchResultMessage_.IsVisible_}");
            }
            catch (Exception ex)
            {
                LogError("Erreur synchronisation", ex);
            }
        }

        private void SubscribeToEvents()
        {
            if (ViewModel == null) return;

            ViewModel.PropertyChanged += OnViewModelPropertyChanged;

            if (ViewModel.ActiveFilters != null)
            {
                // ✅ FIX: Réagir aux changements de filtres
                ViewModel.ActiveFilters.CollectionChanged += (s, e) =>
                {
                    UpdateDisplay();
                    SyncSearchAndResults();
                };
            }

            // ✅ S'abonner aux events spécifiques selon le type de ViewModel
            if (ViewModel is BookCatalogMainViewModel bookVM)
            {
                bookVM.ViewModeChanged += OnViewModeChanged;
                bookVM.BookDetailRequested += OnBookDetailRequested;
                bookVM.BorrowRequested += OnBorrowRequested;
                LogInfo("✅ Events BookCatalog connectés");
            }
            else if (ViewModel is SubscribersCatalogViewModel subsVM)
            {
                subsVM.ViewModeChanged += OnViewModeChanged;
                subsVM.SubsDetailRequested += OnSubscriberDetailRequested;
                LogInfo("✅ Events SubscribersCatalog connectés");
            }

            LogInfo("Événements souscrits");
        }

        private void OnViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                LogInfo($"PropertyChanged: {e.PropertyName}");

                switch (e.PropertyName)
                {
                    case nameof(ICatalogViewModel.DisplayedItems):
                        UpdateDisplay();
                        SyncSearchAndResults(); // ✅ FIX
                        break;
                    case nameof(ICatalogViewModel.TotalResults):
                        UpdateStatusBar();
                        SyncSearchAndResults(); // ✅ FIX
                        break;
                    case nameof(ICatalogViewModel.SearchText):
                        SyncSearchAndResults(); // ✅ FIX
                        break;
                    case "IsListMode":
                        UpdateColumnHeadersVisibility();
                        break;
                }
            }
            catch (Exception ex)
            {
                LogError($"Erreur dans PropertyChanged handler pour {e.PropertyName}", ex);
            }
        }

        private void OnViewModeChanged(object sender, EventArgs e)
        {
            try
            {
                LogInfo($"ViewMode changé vers: {ViewModel.CurrentViewMode}");
                UpdateDisplay();
                UpdateColumnHeadersVisibility();
            }
            catch (Exception ex)
            {
                LogError("Erreur dans OnViewModeChanged", ex);
            }
        }

        private void OnBookDetailRequested(object sender, Book book)
        {
            try
            {
                LogInfo($"📖 Demande d'affichage détails pour: {book.Title}");

                var detailPanel = new BookDetailPanel
                {
                    DataContext = new BookViewModel(book)
                };

                // ✅ Connecter la commande BorrowBtnClick
                if (ViewModel is BookCatalogMainViewModel bookVM)
                {
                    detailPanel.BorrowBtnClick = new RelayCommand(() =>
                    {
                        LogInfo("📚 Bouton Emprunter cliqué depuis le panneau détails");
                        CloseSidePanel();
                        bookVM.ShowBorrowPanelCommand.Execute(book);
                    });
                }

                // ✅ Connecter les boutons de fermeture
                detailPanel.Loaded += (s, e) =>
                {
                    detailPanel.CloseBtn.AddHandler(
                        UI.Common.Items.IconButton.ClickEvent,
                        new RoutedEventHandler((btnSender, btnE) => CloseSidePanel())
                    );
                };

                // ✅ Connecter CloseCommand
                detailPanel.CloseCommand = new RelayCommand(() => CloseSidePanel());

                ShowSidePanel(detailPanel);
            }
            catch (Exception ex)
            {
                LogError("Erreur affichage détails", ex);
                MessageBox.Show($"Erreur lors de l'affichage des détails:\n{ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnBorrowRequested(object sender, Book book)
        {
            try
            {
                LogInfo($"📚 Demande d'emprunt pour: {book.Title}");

                var borrowPanel = new BookBorrowPanel
                {
                    DataContext = new BookViewModel(book)
                };

                // ✅ Connecter les boutons de fermeture
                borrowPanel.Loaded += (s, e) =>
                {
                    borrowPanel.CloseBtn.AddHandler(
                        UI.Common.Items.IconButton.ClickEvent,
                        new RoutedEventHandler((btnSender, btnE) => CloseSidePanel())
                    );
                };
                borrowPanel.BorrowCommand = new RelayCommand(UpdateDisplay);
                // ✅ Connecter CloseCommand
                borrowPanel.CloseCommand = new RelayCommand(() => CloseSidePanel());

                ShowSidePanel(borrowPanel);
            }
            catch (Exception ex)
            {
                LogError("Erreur affichage emprunt", ex);
                MessageBox.Show($"Erreur lors de l'affichage du panneau d'emprunt:\n{ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnSubscriberDetailRequested(object sender, Subscriber subscriber)
        {
            try
            {
                LogInfo($"👤 Demande d'affichage détails abonné: {subscriber.Name_User}");

                var detailPanel = new SUBSDetailPanel
                {
                    DataContext = new SubscriberDetailViewModel(subscriber)
                };

                // ✅ Connecter les boutons de fermeture
                detailPanel.Loaded += (s, e) =>
                {
                    detailPanel.CloseBtn.AddHandler(
                        UI.Common.Items.IconButton.ClickEvent,
                        new RoutedEventHandler((btnSender, btnE) => CloseSidePanel())
                    );

                    // ✅ FIX: Connecter le bouton footer aussi
                    detailPanel.CloseLabel.Click += (btnSender, btnE) => CloseSidePanel();
                };

                // ✅ Connecter CloseCommand au ViewModel
                var detailVM = detailPanel.DataContext as SubscriberDetailViewModel;
                if (detailVM != null)
                {
                    // Remplacer la commande Close par notre logique
                    detailVM.CloseCommand = new RelayCommand(() => CloseSidePanel());
                }

                ShowSidePanel(detailPanel);
            }
            catch (Exception ex)
            {
                LogError("Erreur affichage détails abonné", ex);
                MessageBox.Show($"Erreur lors de l'affichage des détails:\n{ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UnsubscribeFromEvents()
        {
            if (ViewModel != null)
            {
                ViewModel.PropertyChanged -= OnViewModelPropertyChanged;

                if (ViewModel is BookCatalogMainViewModel bookVM)
                {
                    bookVM.ViewModeChanged -= OnViewModeChanged;
                    bookVM.BookDetailRequested -= OnBookDetailRequested;
                    bookVM.BorrowRequested -= OnBorrowRequested;
                }
                else if (ViewModel is SubscribersCatalogViewModel subsVM)
                {
                    subsVM.ViewModeChanged -= OnViewModeChanged;
                    subsVM.SubsDetailRequested -= OnSubscriberDetailRequested;
                }
            }
        }

        #endregion

        #region Gestion du panneau latéral

        private void ShowSidePanel(UIElement content)
        {
            try
            {
                if (DisplayPanel == null || SidePanelBorder == null)
                {
                    LogWarning("DisplayPanel ou SidePanelBorder est null");
                    return;
                }

                // Mettre le contenu
                DisplayPanel.Content = content;

                // Afficher le panneau
                SidePanelBorder.Visibility = Visibility.Visible;

                // Animer l'ouverture
                AnimatePanelOpen();

                LogInfo("Panneau latéral affiché avec succès");
            }
            catch (Exception ex)
            {
                LogError("Erreur lors de l'affichage du panneau latéral", ex);
            }
        }

        private void CloseSidePanel()
        {
            try
            {
                if (SidePanelBorder == null) return;

                LogInfo("Fermeture du panneau latéral");

                // Animation de fade-out
                var fadeOut = new DoubleAnimation
                {
                    From = 1,
                    To = 0,
                    Duration = TimeSpan.FromMilliseconds(200),
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
                };

                fadeOut.Completed += (s, e) =>
                {
                    SidePanelBorder.Visibility = Visibility.Collapsed;
                    if (DisplayPanel != null)
                    {
                        DisplayPanel.Content = null;
                    }

                    LogInfo("Panneau latéral fermé");
                };

                SidePanelBorder.BeginAnimation(OpacityProperty, fadeOut);
            }
            catch (Exception ex)
            {
                LogError("Erreur fermeture panneau", ex);

                // Fallback
                if (SidePanelBorder != null)
                {
                    SidePanelBorder.Visibility = Visibility.Collapsed;
                }
                if (DisplayPanel != null)
                {
                    DisplayPanel.Content = null;
                }
            }
        }

        private void AnimatePanelOpen()
        {
            if (SidePanelBorder == null) return;

            try
            {
                var fadeIn = new DoubleAnimation
                {
                    From = 0,
                    To = 1,
                    Duration = TimeSpan.FromMilliseconds(300),
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                };

                SidePanelBorder.BeginAnimation(OpacityProperty, fadeIn);
            }
            catch (Exception ex)
            {
                LogError("Erreur animation panneau", ex);
            }
        }

        #endregion

        #region Mise à jour de l'affichage

        private void UpdateDisplay()
        {
            if (ViewModel == null || ItemsGrid == null)
            {
                LogWarning("ViewModel ou ItemsGrid null");
                return;
            }

            try
            {
                ItemsGrid.Children.Clear();

                var items = ViewModel.DisplayedItems?.Cast<object>().ToList();

                if (items == null || !items.Any())
                {
                    ShowEmptyMessage();
                    SyncSearchAndResults(); // ✅ FIX
                    LogInfo("Aucun item à afficher");
                    return;
                }

                // ✅ Adapter le nombre de colonnes selon le mode
                ItemsGrid.Columns = ViewModel._currentviewmode == ViewMode.Grid ? 5 : 1;

                LogInfo($"Mode: {ViewModel.CurrentViewMode}, Colonnes: {ItemsGrid.Columns}");

                foreach (var item in items)
                {
                    try
                    {
                        var uiElement = ViewModel.CreateDisplayItem(item);
                        if (uiElement != null)
                        {
                            ItemsGrid.Children.Add(uiElement);
                        }
                    }
                    catch (Exception itemEx)
                    {
                        LogError($"Erreur création item: {itemEx.Message}", itemEx);
                    }
                }

                // ✅ FIX: Synchroniser après l'ajout des items
                SyncSearchAndResults();

                LogInfo($"✅ {items.Count} items affichés");
            }
            catch (Exception ex)
            {
                LogError("Erreur lors de la mise à jour de l'affichage", ex);
                ShowError("Erreur d'affichage", ex);
            }
        }

        private void UpdateColumnHeadersVisibility()
        {
            if (ColumnHeaderPanel_ == null)
            {
                LogWarning("ColumnHeaderPanel_ est null");
                return;
            }

            try
            {
                // ✅ Les headers ne sont visibles qu'en mode Liste
                bool shouldBeVisible = ViewModel._currentviewmode == ViewMode.List && ShowColumnHeaders;

                ColumnHeaderPanel_.Visibility = shouldBeVisible ? Visibility.Visible : Visibility.Collapsed;

                LogInfo($"Headers visibility: {ColumnHeaderPanel_.Visibility}");
            }
            catch (Exception ex)
            {
                LogError("Erreur mise à jour visibilité headers", ex);
            }
        }

        private void ShowEmptyMessage()
        {
            try
            {
                var messageBlock = new TextBlock
                {
                    Text = ViewModel.EmptyMessage,
                    FontSize = 18,
                    Foreground = Brushes.Gray,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 50, 0, 0)
                };

                ItemsGrid.Children.Add(messageBlock);
            }
            catch (Exception ex)
            {
                LogError("Erreur affichage message vide", ex);
            }
        }

        private void UpdateSearchMessage()
        {
            SyncSearchAndResults(); // ✅ Utiliser la méthode centralisée
        }

        private void UpdateStatusBar()
        {
            if (StatusBar_ == null) return;

            try
            {
                // ✅ FIX: Compter les enfants réels de ItemsGrid
                int actualCount = ItemsGrid.Children.Count;

                // Exclure le message vide
                var hasEmptyMessage = ItemsGrid.Children.OfType<TextBlock>()
                    .Any(tb => tb.Text == ViewModel.EmptyMessage);

                if (hasEmptyMessage)
                    actualCount = 0;

                StatusBar_.TotalBooks = actualCount;
                StatusBar_.ViewMode = ViewModel.CurrentViewMode.ToString();

                LogInfo($"StatusBar mis à jour: {actualCount} items, Mode={ViewModel.CurrentViewMode}");
            }
            catch (Exception ex)
            {
                LogError("Erreur mise à jour status bar", ex);
            }
        }

        #endregion

        #region Logging

        private void ShowError(string message, Exception ex)
        {
            var errorMsg = $"{message}\n\n{ex.Message}";
            MessageBox.Show(errorMsg, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            LogError(message, ex);
        }

        private void LogInfo(string message)
        {
            System.Diagnostics.Debug.WriteLine($"[INFO] [GenericMainView] {message}");
        }

        private void LogWarning(string message)
        {
            System.Diagnostics.Debug.WriteLine($"[WARNING] [GenericMainView] {message}");
        }

        private void LogError(string message, Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ERROR] [GenericMainView] {message}");
            System.Diagnostics.Debug.WriteLine($"  Exception: {ex.Message}");
            if (ex.StackTrace != null)
            {
                System.Diagnostics.Debug.WriteLine($"  StackTrace: {ex.StackTrace}");
            }
        }

        #endregion
    }
}