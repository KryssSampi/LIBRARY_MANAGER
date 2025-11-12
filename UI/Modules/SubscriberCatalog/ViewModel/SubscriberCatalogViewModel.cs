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
using LIBBRARY_MANAGER.UI.Modules.Common.Interfaces;
using LIBBRARY_MANAGER.Views.SubscriberView;
using LIBBRARY_MANAGER.UI.Modules.SubscriberCatalog.Core.Services;
using LIBBRARY_MANAGER.UI.Common.Items.MainView;
using LIBBRARY_MANAGER.UI.Modules.BookCatalog.ViewModels;
using LIBBRARY_MANAGER.UI.Modules.BookCatalog.Core.TestCase;
using LIBBRARY_MANAGER.ViewModel.SubscribersViewModel;


namespace LIBBRARY_MANAGER.UI.Modules.SubscriberCatalog.ViewModels
{
  
        public class SubscribersCatalogViewModel : INotifyPropertyChanged, ICatalogViewModel
        {
            #region Services
            private readonly SubscriberFilterService _filterService;
            private readonly SubscriberSearchService _searchService;
            private readonly SubscriberSortService _sortService; 
            #endregion

            #region Collections
            private ObservableCollection<Subscriber> _allSubscribers;
            private ObservableCollection<Subscriber> _displayedSubscribers;
            private ObservableCollection<FilterLabelViewModel> _activeFilters;
            private ObservableCollection<ColumnHeaderViewModel> _headers;

            public ObservableCollection<Subscriber> AllSubscribers
            {
                get => _allSubscribers;
                private set { _allSubscribers = value; OnPropertyChanged(); }
            }

            public ObservableCollection<Subscriber> DisplayedSubscribers
            {
                get => _displayedSubscribers;
                set
                {
                    _displayedSubscribers = value;
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

            public IEnumerable DisplayedItems => DisplayedSubscribers;
            #endregion

            #region Propriétés publiques
            private string _searchText = string.Empty;
            public string SearchText
            {
                get => _searchText;
                set { _searchText = value; OnPropertyChanged(); }
            }

            private ViewMode _currentViewMode = ViewMode.List;
            ViewMode ICatalogViewModel._currentviewmode => _currentViewMode;

            // ✅ AJOUT: Propriétés de tri
            private string _currentSortColumn = "Nom Complet";
            public string CurrentSortColumn
            {
                get => _currentSortColumn;
                set { _currentSortColumn = value; OnPropertyChanged(); }
            }

            private bool _sortAscending = true;
            public bool SortAscending
            {
                get => _sortAscending;
                set { _sortAscending = value; OnPropertyChanged(); }
            }

            public int TotalResults => DisplayedSubscribers?.Count ?? 0;
            public bool HasResults => TotalResults > 0;
            public bool HasFilters => ActiveFilters?.Any() ?? false;

            public string CatalogTitle => "Gestion des Abonnés";

            public string EmptyMessage => string.IsNullOrWhiteSpace(SearchText)
                ? "Aucun abonné enregistré"
                : $"Aucun abonné trouvé pour '{SearchText}'";
            #endregion

            #region Events
            public event EventHandler<Subscriber> SubsDetailRequested;
            public event EventHandler ViewModeChanged;
            #endregion

            #region Commandes
            public ICommand SearchCommand { get; }
            public ICommand ClearSearchCommand { get; }
            public ICommand SortCommand { get; }
            public ICommand ToggleViewModeCommand { get; }
            public ICommand ShowDetailCommand { get; }
            public ICommand RemoveFilterCommand { get; }
            public ICommand ReloadCommand { get; }
            public ICommand BackCommand { get; set; }

            public string ItemsName => "Abonné(e)";
            public bool IsListMode => _currentViewMode == ViewMode.List;
            #endregion

            #region Constructeur
            public SubscribersCatalogViewModel()
            {
                _filterService = new SubscriberFilterService();
                _searchService = new SubscriberSearchService();
                _sortService = new SubscriberSortService(); // ✅ AJOUT

                // ✅ Initialiser les collections AVANT d'appeler Initialize()
                AllSubscribers = new ObservableCollection<Subscriber>();
                DisplayedSubscribers = new ObservableCollection<Subscriber>();
                ActiveFilters = new ObservableCollection<FilterLabelViewModel>();
                Headers = GetDefaultHeaders();

                // Initialiser les commandes
                SearchCommand = new RelayCommand(ExecuteSearch);
                ClearSearchCommand = new RelayCommand(ClearSearch);
                SortCommand = new RelayCommand<string>(SortByColumn);
                ToggleViewModeCommand = new RelayCommand(ToggleViewMode);
                ShowDetailCommand = new RelayCommand<Subscriber>(ShowSubscriberDetail);
                RemoveFilterCommand = new RelayCommand<object>(RemoveFilter);
                ReloadCommand = new RelayCommand(ReloadData);
                BackCommand = new RelayCommand(GoBack);

                LogInfo("SubscribersCatalogViewModel créé avec tri");
            }
            #endregion

            #region Initialisation
            public void Initialize()
            {
                try
                {
                    LogInfo("Initialisation du catalogue abonnés...");

                // Charger les données de test
                var testSubscribers = App.LibraryDbContext.Subscribers.ToList();


                if(testSubscribers is null )
                 SubscriberCatalogSample.FakeSubscribers();


                    AllSubscribers = new ObservableCollection<Subscriber>(testSubscribers);
                    DisplayedSubscribers = new ObservableCollection<Subscriber>(testSubscribers);

                    // ✅ Initialiser les commandes de tri sur les headers
                    InitializeHeaderCommands();

                    LogInfo($"✅ {testSubscribers.Count} abonnés chargés avec tri configuré");
                }
                catch (Exception ex)
                {
                    LogError("Erreur lors de l'initialisation", ex);
                }
            }

            // ✅ NOUVEAU: Initialiser les commandes sur les headers
            private void InitializeHeaderCommands()
            {
                foreach (var header in Headers)
                {
                    header.SortCommand = new RelayCommand(() =>
                    {
                        SortByColumn(header.SortKey);
                    });
                }
            }

            public void Cleanup()
            {
                DisplayedSubscribers?.Clear();
                AllSubscribers?.Clear();
                ActiveFilters?.Clear();
                LogInfo("Catalogue abonnés nettoyé");
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
                    var results = _searchService.Search(AllSubscribers, SearchText);
                    UpdateDisplayedSubscribers(results);
                    LogInfo($"Recherche '{SearchText}': {DisplayedSubscribers.Count} résultat(s)");
                }
                catch (Exception ex)
                {
                    LogError("Erreur lors de la recherche", ex);
                }
            }

            private void ClearSearch()
            {
                SearchText = string.Empty;
                DisplayedSubscribers = new ObservableCollection<Subscriber>(AllSubscribers);
                ApplySort(); // ✅ Réappliquer le tri
                LogInfo("Recherche effacée");
            }

            // ✅ FIX: Implémentation complète du tri
            private void SortByColumn(string columnName)
            {
                if (string.IsNullOrEmpty(columnName)) return;

                try
                {
                    // Inverser le sens du tri si on clique sur la même colonne
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
                _currentViewMode = _currentViewMode == ViewMode.List ? ViewMode.Grid : ViewMode.List;
                OnPropertyChanged(nameof(ICatalogViewModel.CurrentViewMode));
                OnPropertyChanged(nameof(IsListMode));
                ViewModeChanged?.Invoke(this, EventArgs.Empty);
                LogInfo($"Mode d'affichage: {_currentViewMode}");
            }

            private void ShowSubscriberDetail(Subscriber sub)
            {
                if (sub == null)
                {
                    LogWarning("Abonné null pour ShowDetail");
                    return;
                }

                LogInfo($"Demande d'affichage des détails pour: {sub.Name_User}");
                SubsDetailRequested?.Invoke(this, sub);
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
                // Navigation à implémenter selon votre architecture
            }
            #endregion

            #region Tri/Filtres
            // ✅ NOUVEAU: Méthode ApplySort utilisant le service
            private void ApplySort()
            {
                if (DisplayedSubscribers == null || !DisplayedSubscribers.Any())
                    return;

                try
                {
                    var sorted = _sortService.SortByColumnName(
                        DisplayedSubscribers,
                        CurrentSortColumn,
                        SortAscending
                    );

                    DisplayedSubscribers = new ObservableCollection<Subscriber>(sorted);

                    LogInfo($"✅ Tri appliqué: {CurrentSortColumn} ({(SortAscending ? "↑" : "↓")})");
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
                    var filtered = _filterService.ApplyFilters(AllSubscribers, ActiveFilters);
                    UpdateDisplayedSubscribers(filtered);
                }
                catch (Exception ex)
                {
                    LogError("Erreur lors de l'application des filtres", ex);
                }
            }
            #endregion

            #region Helpers
            private void UpdateDisplayedSubscribers(IEnumerable<Subscriber> subscribers)
            {
                DisplayedSubscribers = new ObservableCollection<Subscriber>(subscribers ?? Enumerable.Empty<Subscriber>());

                // ✅ Réappliquer le tri après filtrage
                if (!string.IsNullOrEmpty(CurrentSortColumn))
                {
                    ApplySort();
                }
            }

        private ObservableCollection<ColumnHeaderViewModel> GetDefaultHeaders()
        {
            return new ObservableCollection<ColumnHeaderViewModel>
    {
        new ColumnHeaderViewModel("Référence", allowFilter: false)
        {
            SortKey = "Référence",
            SortCommand = new RelayCommand(() =>_sortService.SortByReference(AllSubscribers)),
            SortBackCommand = new RelayCommand(() => _sortService.SortByReference(AllSubscribers,false))
        },

        new ColumnHeaderViewModel("Nom Complet", allowFilter: false)
        {
            SortKey = "Nom Complet",
            SortCommand = new RelayCommand(() => _sortService.SortByFullName(AllSubscribers)),
            SortBackCommand = new RelayCommand(() =>_sortService.SortByFullName(AllSubscribers,false))
        },

        new ColumnHeaderViewModel("Email", allowFilter: false)
        {
            SortKey = "Email",
            SortCommand = new RelayCommand(() => _sortService.SortByEmail(AllSubscribers)),
            SortBackCommand = new RelayCommand(() => _sortService.SortByEmail(AllSubscribers,false))
        },

        new ColumnHeaderViewModel("Fidélité", allowFilter: false)
        {
            SortKey = "Fidélité",
            SortCommand = new RelayCommand(() => _sortService.SortByFidelity(AllSubscribers)),
            SortBackCommand = new RelayCommand(() => _sortService.SortByFidelity(AllSubscribers,false))
        },

        new ColumnHeaderViewModel("Emprunts Actifs", allowFilter: false)
        {
            SortKey = "Emprunts Actifs",
            SortCommand = new RelayCommand(() => _sortService.SortByActiveLoans(AllSubscribers)),
            SortBackCommand = new RelayCommand(() => _sortService.SortByActiveLoans(AllSubscribers, false))
        },

        new ColumnHeaderViewModel("Peut Emprunter", allowFilter: false)
        {
            SortKey = "Peut Emprunter",
            SortCommand = new RelayCommand(() => _sortService.SortByCanBorrow(AllSubscribers, true)),
            SortBackCommand = new RelayCommand(() => _sortService.SortByCanBorrow(AllSubscribers, false))
        },

        new ColumnHeaderViewModel("Date d'Inscription", allowFilter: false)
        {
            SortKey = "Date d'Inscription",
            SortCommand = new RelayCommand(() => _sortService.SortByRegistrationDate(AllSubscribers,true)),
            SortBackCommand = new RelayCommand(() => _sortService.SortByRegistrationDate(AllSubscribers, false))
        }
    };
        }

        #endregion

        #region ICatalogViewModel.CreateDisplayItem
        public UIElement CreateDisplayItem(object item)
            {
                if (item is not Subscriber sub)
                {
                    LogWarning($"Item n'est pas un Subscriber: {item?.GetType().Name}");
                    return null;
                }

                try
                {
                    // ✅ Créer le ViewModel
                    var viewModel = new SubscriberItemViewModel(sub);

                    // ✅ Créer la vue et lui assigner le ViewModel
                    var subscriberItem = new SubscriberItemView
                    {
                        DataContext = viewModel
                    };

                    // ✅ IMPORTANT: Connecter la commande ShowDetail directement sur le contrôle
                    subscriberItem.ShowDetailCommand = new RelayCommand(() => ShowSubscriberDetail(sub));

                    LogInfo($"Item créé pour {sub.Name_User} avec commande connectée");

                    return subscriberItem;
                }
                catch (Exception ex)
                {
                    LogError($"Erreur création item pour {sub.Name_User}", ex);

                    // Fallback: affichage basique
                    return new TextBlock
                    {
                        Text = $"{sub.Ref_Subscriber} - {sub.Name_User}",
                        Margin = new Thickness(8, 4, 8, 4),
                        FontSize = 14
                    };
                }
            }
            #endregion

            #region Logging
            private void LogInfo(string message)
            {
                System.Diagnostics.Debug.WriteLine($"[INFO] [SubscribersCatalogVM] {message}");
            }

            private void LogWarning(string message)
            {
                System.Diagnostics.Debug.WriteLine($"[WARNING] [SubscribersCatalogVM] {message}");
            }

            private void LogError(string message, Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR] [SubscribersCatalogVM] {message}: {ex.Message}");
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
