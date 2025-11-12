using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using LIBBRARY_MANAGER.Model;
using LIBBRARY_MANAGER.UI.Modules.Common.Views;
using LIBBRARY_MANAGER.UI.Modules.SubscriberCatalog.ViewModels;
using LIBBRARY_MANAGER.Views.SubscriberView;
using LIBBRARY_MANAGER.ViewModel.SubscribersViewModel;

namespace LIBBRARY_MANAGER.UI.Modules.SubscriberCatalog
{
    /// <summary>
    /// Orchestrateur principal du module SubscriberCatalog
    /// </summary>
    public partial class SubscriberCatalog : UserControl
    {
        #region Propriétés

        private SubscribersCatalogViewModel _viewModel;
        private GenericMainView _mainView;

        #endregion

        #region Constructeur

        public SubscriberCatalog()
        {
            InitializeComponent();

            try
            {
                // Initialisation du ViewModel
                _viewModel = new SubscribersCatalogViewModel();

                // Création de la vue principale
                _mainView = new GenericMainView(_viewModel);

                // Configuration de la navigation
                SetupNavigation();

                // Configuration des events
                SetupEvents();

                // Affichage de la vue principale
                MainContent.Content = _mainView;

                // Initialisation après le chargement
                Loaded += SubscribersCatalog_Loaded;

                LogInfo("SubscribersCatalog initialisé avec succès");
            }
            catch (Exception ex)
            {
                LogError("Erreur lors de l'initialisation", ex);
                MessageBox.Show($"Erreur d'initialisation:\n{ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Initialisation

        private void SetupNavigation()
        {
            try
            {
                // Configuration du bouton retour (si nécessaire)
                _mainView.NavigationBar_.BackCommand = new GalaSoft.MvvmLight.CommandWpf.RelayCommand(() =>
                {
                    LogInfo("Bouton retour cliqué");
                    // Navigation vers la vue précédente
                });

                // Masquer le bouton retour par défaut
                _mainView.NavigationBar_.IsBackButtonVisible = false;

                _mainView.NavigationBar_.IsToggleViewButtonVisible = false;
                LogInfo("Navigation configurée");
            }
            catch (Exception ex)
            {
                LogError("Erreur configuration navigation", ex);
            }
        }

        private void SetupEvents()
        {
            try
            {
                // ✅ S'abonner à l'événement de demande de détails
                _viewModel.SubsDetailRequested += OnSubscriberDetailRequested;

                LogInfo("Événements configurés");
            }
            catch (Exception ex)
            {
                LogError("Erreur configuration événements", ex);
            }
        }

        private void SubscribersCatalog_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // ✅ Initialiser le ViewModel après le chargement complet
                if (_viewModel.AllSubscribers == null || _viewModel.AllSubscribers.Count == 0)
                {
                    _viewModel.Initialize();
                }

                LogInfo($"Catalogue chargé: {_viewModel.TotalResults} abonnés");
            }
            catch (Exception ex)
            {
                LogError("Erreur lors du chargement", ex);
            }
        }

        #endregion

        #region Gestion des événements

        /// <summary>
        /// Affiche le panneau de détails d'un abonné
        /// </summary>
        private void OnSubscriberDetailRequested(object sender, Subscriber subscriber)
        {
            try
            {
                LogInfo($"Demande d'affichage des détails pour: {subscriber.Name_User}");

                // Créer le panneau de détails
                var detailPanel = new SUBSDetailPanel
                {
                    DataContext = new SubscriberDetailViewModel(subscriber)
                };

                // Connecter le bouton de fermeture
                detailPanel.Loaded += (s, e) =>
                {
                    detailPanel.CloseBtn.AddHandler(
                        UI.Common.Items.IconButton.ClickEvent,
                        new RoutedEventHandler((btnSender, btnE) => CloseSidePanel())
                    );
                };

                // Afficher le panneau
                ShowSidePanel(detailPanel);
            }
            catch (Exception ex)
            {
                LogError("Erreur affichage détails abonné", ex);
                MessageBox.Show($"Erreur lors de l'affichage des détails:\n{ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Gestion du panneau latéral

        private void ShowSidePanel(UIElement content)
        {
            try
            {
                if (_mainView?.DisplayPanel == null || _mainView?.SidePanelBorder == null)
                {
                    LogWarning("DisplayPanel ou SidePanelBorder est null");
                    return;
                }

                // Mettre le contenu
                _mainView.DisplayPanel.Content = content;

                // Afficher le panneau
                _mainView.SidePanelBorder.Visibility = Visibility.Visible;

                // Animer l'ouverture
                AnimatePanelOpen();

                LogInfo("Panneau latéral affiché");
            }
            catch (Exception ex)
            {
                LogError("Erreur affichage panneau latéral", ex);
            }
        }

        private void CloseSidePanel()
        {
            try
            {
                if (_mainView?.SidePanelBorder == null) return;

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
                    _mainView.SidePanelBorder.Visibility = Visibility.Collapsed;
                    if (_mainView.DisplayPanel != null)
                    {
                        _mainView.DisplayPanel.Content = null;
                    }
                };

                _mainView.SidePanelBorder.BeginAnimation(UIElement.OpacityProperty, fadeOut);
            }
            catch (Exception ex)
            {
                LogError("Erreur fermeture panneau", ex);

                // Fallback
                if (_mainView?.SidePanelBorder != null)
                {
                    _mainView.SidePanelBorder.Visibility = Visibility.Collapsed;
                }
                if (_mainView?.DisplayPanel != null)
                {
                    _mainView.DisplayPanel.Content = null;
                }
            }
        }

        private void AnimatePanelOpen()
        {
            if (_mainView?.SidePanelBorder == null) return;

            try
            {
                var fadeIn = new DoubleAnimation
                {
                    From = 0,
                    To = 1,
                    Duration = TimeSpan.FromMilliseconds(300),
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                };

                _mainView.SidePanelBorder.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            }
            catch (Exception ex)
            {
                LogError("Erreur animation panneau", ex);
            }
        }

        #endregion

        #region Logging

        private void LogInfo(string message)
        {
            System.Diagnostics.Debug.WriteLine($"[INFO] [SubscribersCatalog] {message}");
        }

        private void LogWarning(string message)
        {
            System.Diagnostics.Debug.WriteLine($"[WARNING] [SubscribersCatalog] {message}");
        }

        private void LogError(string message, Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ERROR] [SubscribersCatalog] {message}");
            System.Diagnostics.Debug.WriteLine($"  Exception: {ex.Message}");
            if (ex.StackTrace != null)
            {
                System.Diagnostics.Debug.WriteLine($"  StackTrace: {ex.StackTrace}");
            }
        }

        #endregion
    }
}