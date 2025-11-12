using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using LIBBRARY_MANAGER.Data;
using LIBBRARY_MANAGER.Model;
using LIBBRARY_MANAGER.UI.Modules.Common.Views;
using LIBBRARY_MANAGER.Views.LoanViews;
using LIBBRARY_MANAGER.ViewModel.LoanViewModels;
using LIBBRARY_MANAGER.UI.Modules.LoanCatalog.ViewModel;
using Microsoft.EntityFrameworkCore;
using LoanDetailViewModel = LIBBRARY_MANAGER.ViewModel.LoanViewModels.LoanDetailViewModel;

namespace LIBBRARY_MANAGER.UI.Modules.LoanCatalog
{
    /// <summary>
    /// Orchestrateur principal du module LoanCatalog
    /// ✅ Gestion complète des prêts avec panneaux de détails et de retour
    /// </summary>
    public partial class LoanCatalog : UserControl
    {
        #region Propriétés
        private LoanCatalogViewModel _viewModel;
        private GenericMainView _mainView;
        private readonly LibraryDbContext _context;
        #endregion

        #region Constructeur
        public LoanCatalog()
        {
            InitializeComponent();

            try
            {
                // Récupérer le contexte global
                _context = App.LibraryDbContext;

                // Initialisation du ViewModel
                _viewModel = new LoanCatalogViewModel();

                // Création de la vue principale
                _mainView = new GenericMainView(_viewModel);

                // Configuration de la navigation
                SetupNavigation();

                // Configuration des events
                SetupEvents();

                // Affichage de la vue principale
                this.MainContent.Content = _mainView;

                // Initialisation après le chargement
                Loaded += LoanCatalog_Loaded;

                LogInfo("LoanCatalog initialisé avec succès");
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
                });

                // Masquer le bouton retour par défaut
                _mainView.NavigationBar_.IsBackButtonVisible = false;

                // Masquer le bouton de changement de vue (liste uniquement)
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
                // ✅ S'abonner aux événements du ViewModel
                _viewModel.LoanDetailRequested += OnLoanDetailRequested;
                _viewModel.LoanReturnRequested += OnLoanReturnRequested;

                LogInfo("Événements configurés");
            }
            catch (Exception ex)
            {
                LogError("Erreur configuration événements", ex);
            }
        }

        private void LoanCatalog_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // ✅ Initialiser le ViewModel après le chargement complet
                if (_viewModel.AllLoans == null || _viewModel.AllLoans.Count == 0)
                {
                    _viewModel.Initialize();
                }

                LogInfo($"Catalogue chargé: {_viewModel.TotalResults} emprunts");
            }
            catch (Exception ex)
            {
                LogError("Erreur lors du chargement", ex);
            }
        }
        #endregion

        #region Gestion des événements
        /// <summary>
        /// Affiche le panneau de détails d'un prêt
        /// </summary>
        private void OnLoanDetailRequested(object sender, Loan loan)
        {
            try
            {
                LogInfo($"📋 Demande d'affichage des détails pour: {loan.Ref_Loan}");

                // ✅ Recharger le prêt depuis la DB avec toutes les relations
                var loanInDb = _context.Loans
                    .Include(l => l.Book)
                    .Include(l => l.Subscriber)
                    .Include(l => l.Modifications)
                    .FirstOrDefault(l => l.LoanId == loan.LoanId);

                if (loanInDb == null)
                {
                    MessageBox.Show("Ce prêt n'existe plus dans la base de données.",
                        "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Créer le ViewModel du panneau de détails
                var detailViewModel = new LoanDetailViewModel(loanInDb);

                // S'abonner aux événements du panneau
                detailViewModel.CloseRequested += (s, e) => CloseSidePanel();
                detailViewModel.ReturnPanelRequested += (s, loanToReturn) =>
                {
                    CloseSidePanel();
                    OnLoanReturnRequested(this, loanToReturn);
                };
                detailViewModel.LoanModified += (s, e) =>
                {
                    // ✅ Rafraîchir la liste après modification
                    RefreshLoansFromDatabase();
                };

                // Créer le panneau de détails
                var detailPanel = new LoanDetailPanel
                {
                    DataContext = detailViewModel
                };

                // Connecter les boutons de fermeture
                detailPanel.Loaded += (s, e) =>
                {
                    detailPanel.CloseBtn.AddHandler(
                        UI.Common.Items.IconButton.ClickEvent,
                        new RoutedEventHandler((btnSender, btnE) => CloseSidePanel())
                    );
                };

                ShowSidePanel(detailPanel);
            }
            catch (Exception ex)
            {
                LogError("Erreur affichage détails prêt", ex);
                MessageBox.Show($"Erreur lors de l'affichage des détails:\n{ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Affiche le panneau de retour d'un prêt
        /// </summary>
        private void OnLoanReturnRequested(object sender, Loan loan)
        {
            try
            {
                LogInfo($"📦 Demande d'ouverture du panneau de retour pour: {loan.Ref_Loan}");

                // ✅ Vérifier dans la DB
                var loanInDb = _context.Loans
                    .Include(l => l.Book)
                    .Include(l => l.Subscriber)
                    .FirstOrDefault(l => l.LoanId == loan.LoanId);

                if (loanInDb == null)
                {
                    MessageBox.Show("Ce prêt n'existe plus dans la base de données.",
                        "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!loanInDb.IsActive)
                {
                    MessageBox.Show("Ce prêt a déjà été retourné.",
                        "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // ✅ Créer le ViewModel du panneau de retour
                var returnViewModel = new LIBBRARY_MANAGER.ViewModel.LoanViewModels.LoanReturnViewModel(loanInDb);

                // S'abonner aux événements du ViewModel
                returnViewModel.CloseRequested += (s, e) => CloseSidePanel();
                returnViewModel.ReturnConfirmed += (s, args) =>
                {
                    var (returnedLoan, notes) = args;
                    LogInfo($"✅ Retour confirmé pour {returnedLoan.Ref_Loan}");

                    CloseSidePanel();

                    // ✅ Rafraîchir la liste depuis la DB
                    RefreshLoansFromDatabase();
                };

                // Créer le panneau de retour avec le ViewModel
                var returnPanel = new LoanReturnPanel
                {
                    DataContext = returnViewModel
                };

                // Connecter les boutons de fermeture
                returnPanel.Loaded += (s, e) =>
                {
                    returnPanel.CloseBtn.AddHandler(
                        UI.Common.Items.IconButton.ClickEvent,
                        new RoutedEventHandler((btnSender, btnE) => CloseSidePanel())
                    );
                };

                ShowSidePanel(returnPanel);
            }
            catch (Exception ex)
            {
                LogError("Erreur affichage panneau retour", ex);
                MessageBox.Show($"Erreur lors de l'affichage du panneau de retour:\n{ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region Refresh depuis la DB
        /// <summary>
        /// ✅ Rafraîchit la liste des emprunts depuis la base de données
        /// </summary>
        private void RefreshLoansFromDatabase()
        {
            try
            {
                LogInfo("🔄 Rafraîchissement de la liste depuis la DB...");

                // Recharger les emprunts depuis la DB
                var loansFromDb = _context.Loans
                    .Include(l => l.Book)
                    .Include(l => l.Subscriber)
                    .Include(l => l.Modifications)
                    .OrderByDescending(l => l.BorrowDate)
                    .ToList();

                // Mettre à jour le ViewModel
                _viewModel.AllLoans.Clear();
                foreach (var loan in loansFromDb)
                {
                    _viewModel.AllLoans.Add(loan);
                }

                // Réappliquer les filtres et tri
                _viewModel.ReloadCommand?.Execute(null);

                LogInfo($"✅ Liste rafraîchie: {loansFromDb.Count} emprunts");
            }
            catch (Exception ex)
            {
                LogError("Erreur lors du rafraîchissement", ex);
                MessageBox.Show($"Erreur lors du rafraîchissement:\n{ex.Message}",
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
            System.Diagnostics.Debug.WriteLine($"[INFO] [LoanCatalog] {message}");
        }

        private void LogWarning(string message)
        {
            System.Diagnostics.Debug.WriteLine($"[WARNING] [LoanCatalog] {message}");
        }

        private void LogError(string message, Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ERROR] [LoanCatalog] {message}");
            System.Diagnostics.Debug.WriteLine($"  Exception: {ex.Message}");
            if (ex.StackTrace != null)
            {
                System.Diagnostics.Debug.WriteLine($"  StackTrace: {ex.StackTrace}");
            }
        }
        #endregion
    }
}