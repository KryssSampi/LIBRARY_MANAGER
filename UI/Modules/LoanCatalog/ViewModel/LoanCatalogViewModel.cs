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
using LIBBRARY_MANAGER.UI.Modules.Common.Interfaces;
using LIBBRARY_MANAGER.UI.Modules.BookCatalog.ViewModels;
using LIBBRARY_MANAGER.UI.Modules.LoanCatalog.Core.Services;
using LIBBRARY_MANAGER.UI.Modules.LoanCatalog.Core.TestCase;
using LIBBRARY_MANAGER.Views.LoanViews;
using LIBBRARY_MANAGER.ViewModel.LoanViewModels;

namespace LIBBRARY_MANAGER.UI.Modules.LoanCatalog.ViewModel
{
    /// <summary>
    /// ViewModel principal du catalogue de prêts
    /// </summary>
    public class LoanCatalogViewModel : INotifyPropertyChanged, ICatalogViewModel
    {
        #region Services
        private readonly LoanSearchService _searchService;
        private readonly LoanFilterService _filterService;
        private readonly LoanSortService _sortService;
        #endregion

        #region Collections
        private ObservableCollection<Loan> _allLoans;
        private ObservableCollection<Loan> _displayedLoans;
        private ObservableCollection<FilterLabelViewModel> _activeFilters;
        private ObservableCollection<ColumnHeaderViewModel> _headers;

        public ObservableCollection<Loan> AllLoans
        {
            get => _allLoans;
            private set { _allLoans = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Loan> DisplayedLoans
        {
            get => _displayedLoans;
            set
            {
                _displayedLoans = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalResults));
                OnPropertyChanged(nameof(HasResults));
                OnPropertyChanged(nameof(DisplayedItems));
            }
        }

        public ObservableCollection<FilterLabelViewModel> ActiveFilters
        {
            get => _activeFilters ??= new ObservableCollection<FilterLabelViewModel>();
            set { _activeFilters = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasFilters)); }
        }

        public ObservableCollection<ColumnHeaderViewModel> Headers
        {
            get => _headers ??= GetDefaultHeaders();
            set { _headers = value; OnPropertyChanged(); }
        }

        public IEnumerable DisplayedItems => DisplayedLoans;
        #endregion

        #region Propriétés publiques
        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); }
        }

        private ViewMode _currentViewMode = ViewMode.List;
        ViewMode ICatalogViewModel._currentviewmode => ViewMode.List;

        private string _currentSortColumn = "Date d'Emprunt";
        public string CurrentSortColumn
        {
            get => _currentSortColumn;
            set { _currentSortColumn = value; OnPropertyChanged(); }
        }

        private bool _sortAscending = false; // Par défaut, tri décroissant (plus récents en premier)
        public bool SortAscending
        {
            get => _sortAscending;
            set { _sortAscending = value; OnPropertyChanged(); }
        }

        public int TotalResults => DisplayedLoans?.Count ?? 0;
        public bool HasResults => TotalResults > 0;
        public bool HasFilters => ActiveFilters?.Any() ?? false;

        public string CatalogTitle => "Gestion des Emprunts";

        public string EmptyMessage => string.IsNullOrWhiteSpace(SearchText)
            ? "Aucun emprunt enregistré"
            : $"Aucun emprunt trouvé pour '{SearchText}'";
        #endregion

        #region Events
        public event EventHandler<Loan> LoanDetailRequested;
        public event EventHandler<Loan> LoanReturnRequested;
        public event EventHandler ViewModeChanged;
        #endregion

        #region Commandes
        public ICommand SearchCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand SortCommand { get; }
        public ICommand ToggleViewModeCommand { get; }
        public ICommand ShowDetailCommand { get; }
        public ICommand ShowReturnPanelCommand { get; }
        public ICommand RemoveFilterCommand { get; }
        public ICommand ReloadCommand { get; }
        public ICommand BackCommand { get; set; }

        public string ItemsName => "Emprunt";
        #endregion

        #region Constructeur
        public LoanCatalogViewModel()
        {
            _searchService = new LoanSearchService();
            _filterService = new LoanFilterService();
            _sortService = new LoanSortService();

            AllLoans = new ObservableCollection<Loan>();
            DisplayedLoans = new ObservableCollection<Loan>();
            ActiveFilters = new ObservableCollection<FilterLabelViewModel>();
            Headers = GetDefaultHeaders();

            // Initialiser les commandes
            SearchCommand = new RelayCommand(ExecuteSearch);
            ClearSearchCommand = new RelayCommand(ClearSearch);
            SortCommand = new RelayCommand<string>(SortByColumn);
            ToggleViewModeCommand = new RelayCommand(ToggleViewMode);
            ShowDetailCommand = new RelayCommand<Loan>(ShowLoanDetail);
            ShowReturnPanelCommand = new RelayCommand<Loan>(ShowReturnPanel);
            RemoveFilterCommand = new RelayCommand<object>(RemoveFilter);
            ReloadCommand = new RelayCommand(ReloadData);
            BackCommand = new RelayCommand(GoBack);

            LogInfo("LoanCatalogViewModel créé");
        }
        #endregion

        #region Initialisation
        public void Initialize()
        {
            try
            {
                LogInfo("Initialisation du catalogue emprunts...");
                var testLoans = App.LibraryDbContext.Loans.ToList();
                if (testLoans == null) {
                   testLoans = LoanCatalogSample.FakeLoans();
                    }
                AllLoans = new ObservableCollection<Loan>(testLoans);
                DisplayedLoans = new ObservableCollection<Loan>(testLoans);

                // Tri initial par date (plus récents en premier)
                ApplySort();

                LogInfo($"✅ {testLoans.Count} emprunts chargés");
            }
            catch (Exception ex)
            {
                LogError("Erreur lors de l'initialisation", ex);
            }
        }

        public void Cleanup()
        {
            DisplayedLoans?.Clear();
            AllLoans?.Clear();
            ActiveFilters?.Clear();
            LogInfo("Catalogue emprunts nettoyé");
        }
        #endregion

        #region Méthodes Commandes
        private void ExecuteSearch()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                ClearSearch();
                return;
            }

            try
            {
                var results = _searchService.Search(AllLoans, SearchText);
                UpdateDisplayedLoans(results);
                LogInfo($"Recherche '{SearchText}': {DisplayedLoans.Count} résultat(s)");
            }
            catch (Exception ex)
            {
                LogError("Erreur lors de la recherche", ex);
            }
        }

        private void ClearSearch()
        {
            SearchText = string.Empty;
            DisplayedLoans = new ObservableCollection<Loan>(AllLoans);
            ApplySort();
            LogInfo("Recherche effacée");
        }

        private void SortByColumn(string columnName)
        {
            if (string.IsNullOrEmpty(columnName)) return;

            try
            {
                if (CurrentSortColumn == columnName)
                {
                    SortAscending = !SortAscending;
                }
                else
                {
                    CurrentSortColumn = columnName;
                    SortAscending = true;
                }

                ApplySort();
                LogInfo($"Tri: {CurrentSortColumn} ({(SortAscending ? "ASC" : "DESC")})");
            }
            catch (Exception ex)
            {
                LogError("Erreur lors du tri", ex);
            }
        }

        private void ToggleViewMode()
        {
            // Les prêts sont toujours en mode liste
            LogInfo("Mode liste uniquement pour les prêts");
        }

        private void ShowLoanDetail(Loan loan)
        {
            if (loan == null)
            {
                LogWarning("Prêt null pour ShowDetail");
                return;
            }

            LogInfo($"🎯 ShowLoanDetail appelé pour: {loan.Ref_Loan}");
            LogInfo($"📤 Abonnés connectés: {(LoanDetailRequested != null ? LoanDetailRequested.GetInvocationList().Length : 0)}");

            LoanDetailRequested?.Invoke(this, loan);
        }

        private void ShowReturnPanel(Loan loan)
        {
            if (loan == null)
            {
                LogWarning("Prêt null pour ShowReturnPanel");
                return;
            }

            if (!loan.IsActive)
            {
                LogWarning($"Prêt {loan.Ref_Loan} déjà retourné");
                return;
            }

            LogInfo($"Demande d'ouverture du panneau de retour pour: {loan.Ref_Loan}");
            LoanReturnRequested?.Invoke(this, loan);
        }

        private void RemoveFilter(object filterId)
        {
            var filter = ActiveFilters.FirstOrDefault(f => f.Id.Equals(filterId));
            if (filter != null)
            {
                ActiveFilters.Remove(filter);
                ApplyFilters();
                LogInfo($"Filtre supprimé: {filter.Value}");
            }
        }

        private void ReloadData()
        {
            try
            {
                Initialize();
                ActiveFilters.Clear();
                SearchText = string.Empty;
                LogInfo("Données rechargées");
            }
            catch (Exception ex)
            {
                LogError("Erreur lors du rechargement", ex);
            }
        }

        private void GoBack()
        {
            LogInfo("Retour demandé");
        }
        #endregion

        #region Tri/Filtres
        private void ApplySort()
        {
            if (DisplayedLoans == null || !DisplayedLoans.Any())
                return;

            try
            {
                var sorted = _sortService.SortByColumnName(DisplayedLoans, CurrentSortColumn, SortAscending);
                DisplayedLoans = new ObservableCollection<Loan>(sorted);
            }
            catch (Exception ex)
            {
                LogError("Erreur lors du tri", ex);
            }
        }

        private void ApplyFilters()
        {
            try
            {
                var filtered = _filterService.ApplyFilters(AllLoans, ActiveFilters);
                UpdateDisplayedLoans(filtered);
            }
            catch (Exception ex)
            {
                LogError("Erreur lors de l'application des filtres", ex);
            }
        }
        #endregion

        #region Helpers
        private void UpdateDisplayedLoans(IEnumerable<Loan> loans)
        {
            DisplayedLoans = new ObservableCollection<Loan>(loans ?? Enumerable.Empty<Loan>());

            if (!string.IsNullOrEmpty(CurrentSortColumn))
            {
                ApplySort();
            }
        }

        private ObservableCollection<ColumnHeaderViewModel> GetDefaultHeaders()
        {
            // Création des en-têtes de colonnes
            var headers = new ObservableCollection<ColumnHeaderViewModel>
    {
        new ColumnHeaderViewModel("Référence", allowFilter: false),
        new ColumnHeaderViewModel("Abonné", allowFilter: false),
        new ColumnHeaderViewModel("Livre", allowFilter: false),
        new ColumnHeaderViewModel("Date d'Emprunt", allowFilter: false),
        new ColumnHeaderViewModel("Date de Retour", allowFilter: false),
        new ColumnHeaderViewModel("Statut", allowFilter: false),
        new ColumnHeaderViewModel("Pénalité", allowFilter: false)
    };

            // 🔗 Lier les commandes de tri à chaque header
            foreach (var header in headers)
            {
                // Commande de tri ASC
                header.SortCommand = new RelayCommand(() =>
                {
                    try
                    {
                        CurrentSortColumn = header.Label;
                        SortAscending = true;

                        var sorted = _sortService.SortByColumnName(AllLoans, header.Label, ascending: true);
                        DisplayedLoans = new ObservableCollection<Loan>(sorted);
                        OnPropertyChanged(nameof(DisplayedLoans));

                        LogInfo($"Tri ASC appliqué sur {header.Label}");
                    }
                    catch (Exception ex)
                    {
                        LogError($"Erreur tri ASC sur {header.Label}", ex);
                    }
                });

                // Commande de tri DESC
                header.SortBackCommand = new RelayCommand(() =>
                {
                    try
                    {
                        CurrentSortColumn = header.Label;
                        SortAscending = false;

                        var sorted = _sortService.SortByColumnName(AllLoans, header.Label, ascending: false);
                        DisplayedLoans = new ObservableCollection<Loan>(sorted);
                        OnPropertyChanged(nameof(DisplayedLoans));

                        LogInfo($"Tri DESC appliqué sur {header.Label}");
                    }
                    catch (Exception ex)
                    {
                        LogError($"Erreur tri DESC sur {header.Label}", ex);
                    }
                });
            }

            return headers;
        }

        #endregion

        #region ICatalogViewModel.CreateDisplayItem
        public UIElement CreateDisplayItem(object item)
        {
            if (item is not Loan loan)
            {
                LogWarning($"Item n'est pas un Loan: {item?.GetType().Name}");
                return null;
            }

            try
            {
                var viewModel = new LoanItemViewModel(loan);

                var loanItem = new LoanItemView
                {
                    DataContext = viewModel
                };

                // ✅ Connecter les commandes
                loanItem.ViewDetailsCommand = new RelayCommand(() => ShowLoanDetail(loan));
                loanItem.MarkReturnedCommand = new RelayCommand(
                    () => ShowReturnPanel(loan),
                    () => loan.IsActive
                );

                LogInfo($"Item créé pour {loan.Ref_Loan} avec commandes connectées");

                return loanItem;
            }
            catch (Exception ex)
            {
                LogError($"Erreur création item pour {loan.Ref_Loan}", ex);

                return new TextBlock
                {
                    Text = $"{loan.Ref_Loan} - {loan.Book?.Title}",
                    Margin = new Thickness(8, 4, 8, 4),
                    FontSize = 14
                };
            }
        }
        #endregion

        #region Logging
        private void LogInfo(string message)
        {
            System.Diagnostics.Debug.WriteLine($"[INFO] [LoanCatalogVM] {message}");
        }

        private void LogWarning(string message)
        {
            System.Diagnostics.Debug.WriteLine($"[WARNING] [LoanCatalogVM] {message}");
        }

        private void LogError(string message, Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ERROR] [LoanCatalogVM] {message}: {ex.Message}");
            if (ex.StackTrace != null)
            {
                System.Diagnostics.Debug.WriteLine($"  StackTrace: {ex.StackTrace}");
            }
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        #endregion
    }
}