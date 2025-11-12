using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using GalaSoft.MvvmLight.CommandWpf;
using LIBBRARY_MANAGER.UI.Modules.BookCatalog.ViewModels;
using LIBBRARY_MANAGER.UI.Modules.BookCatalog.Views.HomeView;
using LIBBRARY_MANAGER.UI.Modules.BookCatalog.Views.MainView;
using LIBBRARY_MANAGER.UI.Modules.Common.Views;
using static LIBBRARY_MANAGER.Model.Book;

namespace LIBBRARY_MANAGER.UI.Modules.BookCatalog
{
    /// <summary>
    /// Orchestrateur principal du module BookCatalog
    /// ✅ TOUS LES BUGS CORRIGÉS
    /// </summary>
    public partial class BookCatalog : UserControl
    {
        #region Propriétés

        private BookCatalogMainViewModel _mainViewModel;
        private HomeView _homeView;
        private BookCatalogView _mainView;
        private UserControl? _currentView;

        #endregion

        #region Constructeur

        public BookCatalog()
        {
            InitializeComponent();

            try
            {
                _mainViewModel = new BookCatalogMainViewModel();
                InitializeViews();
                SetupNavigation();

                // Affichage initial de la HomeView
                NavigateToHome(animate: false);

                // ✅ FIX: Charger les données après le rendu complet
                Loaded += BookCatalog_Loaded;

                LogInfo("✅ BookCatalog initialisé avec succès");
            }
            catch (Exception ex)
            {
                LogError("Erreur lors de l'initialisation", ex);
                MessageBox.Show($"Erreur d'initialisation:\n{ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Initialisation

        private void InitializeViews()
        {
            try
            {
                _homeView = new HomeView();
                _mainView = new BookCatalogView(_mainViewModel);

                LogInfo("✅ Vues créées");
            }
            catch (Exception ex)
            {
                LogError("Erreur création vues", ex);
                throw;
            }
        }

        private void SetupNavigation()
        {
            try
            {
                // HomeView → MainView (sélection de catégorie)
                _homeView.CategoriesGrid.OnCategorySelected += OnCategorySelected;

                // MainView → HomeView (bouton retour)
                _mainView.NavigationBar_.BackCommand = new RelayCommand(() =>
                {
                    LogInfo("🔙 Bouton retour cliqué - Navigation vers HomeView");
                    NavigateToHome(animate: true);
                });

                LogInfo("✅ Navigation configurée");
            }
            catch (Exception ex)
            {
                LogError("Erreur configuration navigation", ex);
                throw;
            }
        }

        private void BookCatalog_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LogInfo("📥 Chargement du BookCatalog...");

                // ✅ Initialiser le ViewModel (charge depuis DB)
                _mainViewModel.Initialize();

                // ✅ Mettre à jour les compteurs APRÈS chargement
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    UpdateCategoryCounts();
                    LogInfo($"✅ Compteurs mis à jour: {_mainViewModel.AllBooks.Count} livres chargés");
                }), System.Windows.Threading.DispatcherPriority.Loaded);
            }
            catch (Exception ex)
            {
                LogError("Erreur lors du chargement", ex);
            }
        }

        #endregion

        #region Navigation

        private void NavigateToHome(bool animate = true)
        {
            try
            {
                // ✅ Rafraîchir les compteurs depuis la base
                UpdateCategoryCounts();

                NavigateTo(_homeView, animate);

                LogInfo("📂 Navigation vers HomeView");
            }
            catch (Exception ex)
            {
                LogError("Erreur navigation HomeView", ex);
            }
        }

        private void OnCategorySelected(CategoryAllowed category)
        {
            try
            {
                LogInfo($"📂 Catégorie sélectionnée: {category}");

                // Charger les livres de la catégorie
                _mainViewModel.CurrentCategory = category;

                // ✅ Appliquer le filtre si ce n'est pas "Tous"
                if (category != CategoryAllowed.Tous)
                {
                    _mainViewModel.ActiveFilters.Clear();
                    var categoryName = _mainViewModel.CategoryDisplayName;
                    _mainViewModel.ActiveFilters.Add(new UI.Common.Items.MainView.FilterLabelViewModel
                    {
                        Id = $"cat_{category}",
                        Type = "Catégorie",
                        Value = categoryName
                    });
                }
                else
                {
                    _mainViewModel.ActiveFilters.Clear();
                }

                // Rafraîchir l'affichage
                _mainViewModel.ExecuteClearSearch();

                // Naviguer vers MainView
                NavigateTo(_mainView, animate: true);

                LogInfo($"✅ Navigation vers MainView - {_mainViewModel.DisplayedBooks.Count} livres affichés");
            }
            catch (Exception ex)
            {
                LogError($"Erreur sélection catégorie {category}", ex);
                MessageBox.Show($"Erreur lors du chargement:\n{ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NavigateTo(UserControl targetView, bool animate = true)
        {
            if (targetView == null)
            {
                LogWarning("⚠️ targetView est null");
                return;
            }

            if (targetView == _currentView)
            {
                LogInfo("ℹ️ Vue déjà affichée");
                return;
            }

            try
            {
                if (animate && _currentView != null)
                {
                    var fadeOut = new DoubleAnimation
                    {
                        From = 1.0,
                        To = 0.0,
                        Duration = TimeSpan.FromMilliseconds(200),
                        EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
                    };

                    fadeOut.Completed += (s, e) =>
                    {
                        MainContent.Content = targetView;
                        _currentView = targetView;

                        var fadeIn = new DoubleAnimation
                        {
                            From = 0.0,
                            To = 1.0,
                            Duration = TimeSpan.FromMilliseconds(250),
                            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                        };

                        MainContent.BeginAnimation(OpacityProperty, fadeIn);
                    };

                    MainContent.BeginAnimation(OpacityProperty, fadeOut);
                }
                else
                {
                    MainContent.Content = targetView;
                    _currentView = targetView;

                    if (animate)
                    {
                        var fadeIn = new DoubleAnimation
                        {
                            From = 0.0,
                            To = 1.0,
                            Duration = TimeSpan.FromMilliseconds(250),
                            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                        };
                        MainContent.BeginAnimation(OpacityProperty, fadeIn);
                    }
                }

                LogInfo($"✅ Navigation vers {targetView.GetType().Name}");
            }
            catch (Exception ex)
            {
                LogError("Erreur pendant la navigation", ex);
                MainContent.Content = targetView;
                _currentView = targetView;
            }
        }

        #endregion

        #region Mise à jour des compteurs

        /// <summary>
        /// ✅ CORRECTION : Met à jour les compteurs depuis la base de données
        /// </summary>
        private void UpdateCategoryCounts()
        {
            try
            {
                // Charger les compteurs depuis la base de données
                var counts = _mainViewModel._categoryCountService.GetCategoryCounts(_mainViewModel.AllBooks);

                if (_homeView?.CategoriesGrid != null)
                {
                    _homeView.CategoriesGrid.UpdateCategoryCounts(counts);
                    LogInfo($"✅ Compteurs mis à jour: {counts.Count} catégories, {_mainViewModel.AllBooks.Count} livres au total");
                }
                else
                {
                    LogWarning("⚠️ CategoriesGrid est null");
                }
            }
            catch (Exception ex)
            {
                LogError("Erreur mise à jour compteurs", ex);
            }
        }

        #endregion

        #region Logging

        private void LogInfo(string message)
            => System.Diagnostics.Debug.WriteLine($"[INFO] [BookCatalog] {message}");

        private void LogWarning(string message)
            => System.Diagnostics.Debug.WriteLine($"[WARNING] [BookCatalog] {message}");

        private void LogError(string message, Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ERROR] [BookCatalog] {message}");
            System.Diagnostics.Debug.WriteLine($"  Exception: {ex.Message}");
            if (ex.StackTrace != null)
            {
                System.Diagnostics.Debug.WriteLine($"  StackTrace: {ex.StackTrace}");
            }
        }

        #endregion
    }
}