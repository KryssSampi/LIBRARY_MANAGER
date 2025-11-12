Parfait ! Je vais maintenant créer un **README ultra-complet et détaillé** pour le module **LoanCatalog** (Catalogue des Emprunts), qui est effectivement très complexe avec sa vue générique réutilisable, ses items sophistiqués, et ses panneaux latéraux interactifs.

***

# README  : Module LoanCatalog (Gestion des Emprunts)

## Vue d'Ensemble

Le module **LoanCatalog** est un **système de gestion complète des emprunts** de bibliothèque, construit sur une **architecture générique réutilisable**. Il permet de visualiser, rechercher, filtrer, trier et gérer tous les prêts de livres avec des panneaux latéraux interactifs pour les détails et les retours.

**Complexité** : ⭐⭐⭐⭐⭐ (Module le plus sophistiqué de l'application)

---

## Architecture Complète

### **Structure Hiérarchique**

```
/LoanCatalog
├── LoanCatalog.xaml & .cs                    # Orchestrateur principal
├── /Core
│   ├── /Services
│   │   ├── LoanSearchService.cs              # Service de recherche
│   │   ├── LoanFilterService.cs              # Service de filtrage
│   │   └── LoanSortService.cs                # Service de tri
│   └── /Interfaces
│       └── ICatalogViewModel.cs              # Interface commune aux catalogues
├── /ViewModels
│   ├── LoanCatalogViewModel.cs               # ViewModel principal du catalogue
│   ├── LoanItemViewModel.cs                  # ViewModel d'un item de liste
│   ├── LoanDetailViewModel.cs                # ViewModel panneau détails
│   └── LoanReturnViewModel.cs                # ViewModel panneau retour
├── /Views
│   ├── LoanItemView.xaml & .cs               # Item de la liste (ligne)
│   ├── LoanDetailPanel.xaml & .cs            # Panneau latéral détails
│   └── LoanReturnPanel.xaml & .cs            # Panneau latéral retour
└── /Common (Partagé avec BookCatalog, SubscriberCatalog)
    ├── GenericMainView.xaml & .cs            # Vue générique réutilisable
    └── /Items
        ├── NavigationBar.xaml                # Barre de navigation
        ├── ActiveFiltersPanel.xaml           # Panneau filtres actifs
        ├── SearchResultMessage.xaml          # Message de résultats
        ├── ColumnHeadersPanel.xaml           # En-têtes de colonnes
        ├── StatusBar.xaml                    # Barre de statut
        └── Header.xaml                       # En-tête de page
```

***

## Cas d'Utilisation Implémentés

### **1. Visualisation de la Liste des Emprunts**
- Affichage de tous les emprunts (actifs et retournés)
- Tri par défaut par date d'emprunt (plus récents en premier)
- Affichage en mode liste uniquement (mode grille désactivé)
- Colonnes affichées :
  - **Référence** : `LOAN-20251112-00000001`
  - **Abonné** : Nom complet
  - **Livre** : Titre
  - **Date d'Emprunt** : Format `dd MMM yyyy`
  - **Date de Retour Prévue** : Format `dd MMM yyyy`
  - **Statut** : Visual avec couleur (En cours / Retourné / En retard)
  - **Pénalité** : Montant ou `-`
  - **Actions** : Boutons Voir détails + Marquer retourné

### **2. Recherche en Temps Réel**
- Champs de recherche : Référence, Nom abonné, Titre livre, Auteur, ISBN
- Mise à jour instantanée de la liste
- Message dynamique : "X résultats pour 'texte'"
- Aucun délai (pas de debouncing)

### **3. Tri Multi-Colonnes**
- Clic sur en-tête de colonne → tri ascendant
- Re-clic → tri descendant
- Indicateur visuel (flèche ↑ ou ↓)
- Colonnes triables :
  - Référence (alphabétique)
  - Abonné (alphabétique par nom)
  - Livre (alphabétique par titre)
  - Date d'Emprunt (chronologique)
  - Date de Retour (chronologique)
  - Statut (En cours > Retourné > En retard)
  - Pénalité (montant croissant/décroissant)

### **4. Filtrage Avancé**
- **Par Statut** : En cours / Retourné / En retard
- **Par Pénalité** : Avec pénalité / Sans pénalité
- Filtres cumulables
- Panneau "Filtres actifs" avec badges supprimables
- Bouton "Réinitialiser tous les filtres"

### **5. Affichage des Détails d'un Emprunt**
- Clic sur bouton "Œil" ou sur la ligne → panneau latéral slide-in
- Informations complètes :
  - Référence du prêt
  - Livre (titre + couverture si disponible)
  - Abonné (nom + infos)
  - Dates (emprunt, retour prévu, retour effectif)
  - Statut visuel avec couleur
  - Pénalité (si applicable)
  - Retard (nombre de jours)
- **Fonctionnalité avancée** : Modification de la date de retour
  - Bouton "Modifier" → DatePicker éditable
  - Validation : Date >= aujourd'hui
  - Sauvegarde → Enregistrement d'une `Modification` dans l'historique
  - Génération de référence : `MOD-20251112-00000001`

### **6. Retour de Livre avec Confirmation**
- Clic sur bouton "Marquer retourné" → panneau latéral dédié
- Affichage récapitulatif :
  - Livre, Abonné, Dates
  - **Calcul automatique** :
    - Retard (si applicable)
    - Pénalité calculée (1€/jour de retard)
    - Impact sur la fidélité
- Zone de notes (optionnel)
- Bouton "Confirmer le retour" :
  - Mise à jour `IsActive = false`
  - Enregistrement `ActualReturnDate`
  - Application de la pénalité
  - Mise à jour fidélité abonné (+bonus si à temps, -malus si retard)
  - Incrémentation du stock livre
  - Message de confirmation détaillé
  - Rafraîchissement automatique de la liste

### **7. Réactualisation Automatique**
- Après modification de date → refresh
- Après retour de livre → refresh depuis DB
- Préservation des filtres et tri appliqués

***

## Modèles de Données

### **1. Loan (Emprunt)**

```csharp
public class Loan : ObservableObject
{
    // Clés et relations
    public long LoanId { get; set; }                    // PK
    public long BookId { get; set; }                    // FK Book
    public virtual Book Book { get; set; }
    public long SubscriberId { get; set; }              // FK Subscriber
    public virtual Subscriber Subscriber { get; set; }
    
    // Dates
    public DateTime BorrowDate { get; set; }            // Par défaut: DateTime.Now
    public DateTime ReturnDate { get; set; }            // Calculée: BorrowDate + durée + bonus fidélité
    public DateTime? ActualReturnDate { get; set; }     // Null si pas encore retourné
    
    // Statut
    public bool IsActive { get; set; } = true;          // false après retour
    public decimal? Penalty { get; set; }               // null ou montant pénalité
    public string Ref_Loan { get; set; }                // Référence générée
    
    // Navigation
    public virtual ICollection<Modification> Modifications { get; set; }
    
    // Propriétés calculées
    public bool IsLate => IsActive && DateTime.Now.Date > ReturnDate.Date;
    public int DaysLate => IsLate ? (DateTime.Now.Date - ReturnDate.Date).Days : 0;
    
    // Constructeur
    public Loan(Subscriber subscriber, Book book, int durationDays = 14)
    {
        // Validation + calcul ReturnDate avec bonus fidélité
        ReturnDate = BorrowDate.AddDays(durationDays + (subscriber.Fidelity * 2));
    }
}
```

### **2. Modification (Historique)**

```csharp
public class Modification : ObservableObject
{
    public long ModificationId { get; set; }
    public ModificationType Type { get; set; }          // ChangeReturnDate / ReturnLoan
    public long LoanId { get; set; }
    public virtual Loan Loan { get; set; }
    public long StaffMemberId { get; set; }
    public virtual StaffMember StaffMember { get; set; }
    public DateTime ModificationDate { get; set; }
    public string Description { get; set; }              // Généré automatiquement
    public DateTime? OldReturnDate { get; set; }
    public DateTime? NewReturnDate { get; set; }
    public string Ref_Modification { get; set; }         // Ex: MOD-20251112-00000001
}

public enum ModificationType
{
    ChangeReturnDate,
    ReturnLoan
}
```

***

## ViewModels Détaillés

### **1. LoanCatalogViewModel** (ViewModel Principal)

**Responsabilités** :
- Gestion de la collection complète des emprunts
- Coordination Recherche / Filtrage / Tri
- Implémentation de `ICatalogViewModel` (interface commune)
- Gestion des commandes utilisateur

**Propriétés** :

```csharp
public class LoanCatalogViewModel : INotifyPropertyChanged, ICatalogViewModel
{
    // Services injectés
    private readonly LoanSearchService _searchService;
    private readonly LoanFilterService _filterService;
    private readonly LoanSortService _sortService;
    
    // Collections
    public ObservableCollection<Loan> AllLoans { get; set; }
    public ObservableCollection<Loan> DisplayedLoans { get; set; }
    public ObservableCollection<FilterLabelViewModel> ActiveFilters { get; set; }
    public ObservableCollection<ColumnHeaderViewModel> Headers { get; set; }
    
    // Propriétés publiques
    public string SearchText { get; set; }
    public string CurrentSortColumn { get; set; } = "Date d'Emprunt";
    public bool SortAscending { get; set; } = false;        // Par défaut DESC
    public int TotalResults => DisplayedLoans?.Count ?? 0;
    public bool HasResults => TotalResults > 0;
    public bool HasFilters => ActiveFilters?.Any() ?? false;
    public string CatalogTitle => "Gestion des Emprunts";
    public string EmptyMessage => string.IsNullOrWhiteSpace(SearchText)
        ? "Aucun emprunt enregistré"
        : $"Aucun emprunt trouvé pour '{SearchText}'";
    
    // IEnumerable pour compatibilité GenericMainView
    public IEnumerable DisplayedItems => DisplayedLoans;
    
    // Events
    public event EventHandler<Loan> LoanDetailRequested;
    public event EventHandler<Loan> LoanReturnRequested;
    public event EventHandler ViewModeChanged;
    
    // Commandes
    public ICommand SearchCommand { get; }
    public ICommand ClearSearchCommand { get; }
    public ICommand SortCommand { get; }
    public ICommand ToggleViewModeCommand { get; }
    public ICommand ShowDetailCommand { get; }
    public ICommand ShowReturnPanelCommand { get; }
    public ICommand RemoveFilterCommand { get; }
    public ICommand ReloadCommand { get; }
    public ICommand BackCommand { get; set; }
}
```

**Méthodes clés** :

```csharp
// Initialisation depuis DB ou données de test
public void Initialize()
{
    var testLoans = App.LibraryDbContext.Loans.ToList();
    AllLoans = new ObservableCollection<Loan>(testLoans);
    DisplayedLoans = new ObservableCollection<Loan>(testLoans);
    
    // Tri initial par date DESC
    SortByColumn("Date d'Emprunt");
}

// Exécution de la recherche
private void ExecuteSearch()
{
    var results = _searchService.Search(AllLoans, SearchText);
    results = _filterService.ApplyFilters(results, ActiveFilters);
    results = _sortService.SortByColumnName(results, CurrentSortColumn, SortAscending);
    
    DisplayedLoans = new ObservableCollection<Loan>(results);
    OnPropertyChanged(nameof(DisplayedLoans));
}

// Création d'un item de liste (appelé par GenericMainView)
public UIElement CreateDisplayItem(object item)
{
    if (item is not Loan loan) return null;
    
    var viewModel = new LoanItemViewModel(loan);
    var loanItem = new LoanItemView { DataContext = viewModel };
    
    // Connexion des commandes
    loanItem.ViewDetailsCommand = new RelayCommand(() => ShowLoanDetail(loan));
    loanItem.MarkReturnedCommand = new RelayCommand(
        () => ShowReturnPanel(loan),
        () => loan.IsActive
    );
    
    return loanItem;
}

// Affichage des détails
private void ShowLoanDetail(Loan loan)
{
    LoanDetailRequested?.Invoke(this, loan);
}

// Affichage du panneau de retour
private void ShowReturnPanel(Loan loan)
{
    LoanReturnRequested?.Invoke(this, loan);
}
```

**Initialisation des Headers** :

```csharp
private ObservableCollection<ColumnHeaderViewModel> GetDefaultHeaders()
{
    var headers = new ObservableCollection<ColumnHeaderViewModel>
    {
        new() { Label = "Référence", Width = 1.5, IsSortable = true },
        new() { Label = "Abonné", Width = 1, IsSortable = true },
        new() { Label = "Livre", Width = 1, IsSortable = true },
        new() { Label = "Date d'Emprunt", Width = 1, IsSortable = true },
        new() { Label = "Date de Retour", Width = 1, IsSortable = true },
        new() { Label = "Statut", Width = 1, IsSortable = true },
        new() { Label = "Pénalité", Width = 1, IsSortable = true },
        new() { Label = "Actions", Width = 0, IsSortable = false }
    };
    
    // Connexion des commandes de tri
    foreach (var header in headers.Where(h => h.IsSortable))
    {
        header.SortAscCommand = new RelayCommand(() => { /* Tri ASC */ });
        header.SortDescCommand = new RelayCommand(() => { /* Tri DESC */ });
    }
    
    return headers;
}
```

***

### **2. LoanItemViewModel** (Item de Liste)

**Responsabilité** : Adapter un `Loan` pour l'affichage dans `LoanItemView`

```csharp
public class LoanItemViewModel : INotifyPropertyChanged
{
    private readonly Loan _loan;
    
    // Propriétés du modèle (lecture seule)
    public Loan LoanInstance => _loan;
    public string RefLoan => _loan.Ref_Loan;
    public string SubscriberName => _loan.Subscriber?.Name_User ?? "Inconnu";
    public string BookTitle => _loan.Book?.Title ?? "Inconnu";
    public DateTime BorrowDate => _loan.BorrowDate;
    public DateTime ReturnDate => _loan.ReturnDate;
    public bool IsActive => _loan.IsActive;
    public bool IsLate => _loan.IsLate;
    public int DaysLate => _loan.DaysLate;
    public decimal? Penalty => _loan.Penalty;
    
    // Propriétés calculées pour l'UI
    public string StatusLabel
    {
        get
        {
            if (!IsActive) return "Retourné";
            if (IsLate) return $"En retard ({DaysLate}j)";
            return "En cours";
        }
    }
    
    public Brush StatusColor
    {
        get
        {
            if (!IsActive) return new SolidColorBrush(Color.FromRgb(117, 117, 117)); // Gris
            if (IsLate) return new SolidColorBrush(Color.FromRgb(244, 67, 54));      // Rouge
            return new SolidColorBrush(Color.FromRgb(76, 175, 80));                  // Vert
        }
    }
    
    public string PenaltyText => Penalty.HasValue ? $"{Penalty:C2}" : "-";
    
    public Brush PenaltyColor
    {
        get
        {
            if (!Penalty.HasValue || Penalty == 0)
                return new SolidColorBrush(Color.FromRgb(117, 117, 117)); // Gris
            return new SolidColorBrush(Color.FromRgb(244, 67, 54)); // Rouge
        }
    }
    
    public string BorrowDateFormatted => BorrowDate.ToString("dd MMM yyyy");
    public string ReturnDateFormatted => ReturnDate.ToString("dd MMM yyyy");
    
    // Méthode de rafraîchissement après modification
    public void RefreshAll()
    {
        OnPropertyChanged(nameof(IsActive));
        OnPropertyChanged(nameof(IsLate));
        OnPropertyChanged(nameof(StatusLabel));
        OnPropertyChanged(nameof(StatusColor));
        // ... etc
    }
}
```

***

### **3. LoanDetailViewModel** (Panneau Détails)

**Responsabilité** : Gestion du panneau latéral de détails avec édition de date

```csharp
public class LoanDetailViewModel : INotifyPropertyChanged
{
    private LibraryDbContext _context;
    private readonly StaffMember _currentStaff;
    private Loan _loan;
    
    // Propriétés du modèle
    public string Ref_Loan => _loan.Ref_Loan;
    public Book Book => _loan.Book;
    public Subscriber Subscriber => _loan.Subscriber;
    public DateTime BorrowDate => _loan.BorrowDate;
    
    private DateTime _returnDate;
    public DateTime ReturnDate
    {
        get => _returnDate;
        set
        {
            if (_returnDate != value)
            {
                _returnDate = value;
                OnPropertyChanged();
                UpdateCanSaveReturnDate();
            }
        }
    }
    
    public bool IsActive => _loan.IsActive;
    public bool IsLate => _loan.IsLate;
    public int DaysLate => _loan.DaysLate;
    public decimal? Penalty => _loan.Penalty;
    
    // Propriétés pour l'édition de date
    private bool _isEditingReturnDate = false;
    public bool IsEditingReturnDate
    {
        get => _isEditingReturnDate;
        set
        {
            _isEditingReturnDate = value;
            OnPropertyChanged();
            if (value) NewReturnDate = ReturnDate; // Initialiser
        }
    }
    
    private DateTime _newReturnDate;
    public DateTime NewReturnDate
    {
        get => _newReturnDate;
        set
        {
            _newReturnDate = value;
            OnPropertyChanged();
            UpdateCanSaveReturnDate();
        }
    }
    
    private string _returnDateErrorMessage;
    public string ReturnDateErrorMessage
    {
        get => _returnDateErrorMessage;
        set { _returnDateErrorMessage = value; OnPropertyChanged(); }
    }
    
    private bool _canSaveReturnDate = false;
    public bool CanSaveReturnDate
    {
        get => _canSaveReturnDate;
        set { _canSaveReturnDate = value; OnPropertyChanged(); }
    }
    
    // Commandes
    public ICommand StartEditReturnDateCommand { get; }
    public ICommand SaveReturnDateCommand { get; }
    public ICommand CancelEditReturnDateCommand { get; }
    public ICommand OpenReturnPanelCommand { get; }
    public ICommand CloseCommand { get; }
    
    // Events
    public event EventHandler CloseRequested;
    public event EventHandler<Loan> ReturnPanelRequested;
    public event EventHandler LoanModified;
    
    // Constructeur
    public LoanDetailViewModel(Loan loan)
    {
        _loan = loan ?? throw new ArgumentNullException(nameof(loan));
        _returnDate = loan.ReturnDate;
        _currentStaff = ((App)Application.Current).CurrentUser as StaffMember;
        
        // Initialisation des commandes
        StartEditReturnDateCommand = new RelayCommand(StartEditReturnDate, () => IsActive);
        SaveReturnDateCommand = new RelayCommand(SaveReturnDate, () => CanSaveReturnDate);
        CancelEditReturnDateCommand = new RelayCommand(CancelEditReturnDate);
        OpenReturnPanelCommand = new RelayCommand(OpenReturnPanel, () => IsActive);
        CloseCommand = new RelayCommand(() => CloseRequested?.Invoke(this, EventArgs.Empty));
    }
    
    // Validation de la nouvelle date
    private void UpdateCanSaveReturnDate()
    {
        if (!IsEditingReturnDate)
        {
            CanSaveReturnDate = false;
            return;
        }
        
        if (NewReturnDate == ReturnDate)
        {
            ReturnDateErrorMessage = "Aucune modification détectée";
            CanSaveReturnDate = false;
            return;
        }
        
        if (NewReturnDate < DateTime.Now.Date)
        {
            ReturnDateErrorMessage = "La date ne peut pas être dans le passé";
            CanSaveReturnDate = false;
            return;
        }
        
        ReturnDateErrorMessage = null;
        CanSaveReturnDate = true;
    }
    
    // Sauvegarde de la modification de date
    private void SaveReturnDate()
    {
        try
        {
            _context = new LibraryDbContext();
            
            // Recharger le prêt depuis la DB
            var loanInDb = _context.Loans
                .Include(l => l.Book)
                .Include(l => l.Subscriber)
                .FirstOrDefault(l => l.LoanId == _loan.LoanId);
            
            if (loanInDb == null || !loanInDb.IsActive)
            {
                ShowError("Le prêt n'est plus actif");
                return;
            }
            
            // Recharger le staff
            var staffInDb = _context.StaffMembers.Find(_currentStaff.Id_User);
            
            // Créer la modification via le staff
            var modification = staffInDb.ChangeReturnDate(
                loanInDb,
                NewReturnDate,
                out string errorMessage
            );
            
            if (modification == null)
            {
                ShowError(errorMessage);
                return;
            }
            
            // Ajouter la modification et sauvegarder
            _context.Modifications.Add(modification);
            _context.SaveChangesWithReferences(); // Génère Ref_Modification
            
            // Mettre à jour le modèle local
            _loan.ReturnDate = NewReturnDate;
            ReturnDate = NewReturnDate;
            IsEditingReturnDate = false;
            
            // Notifier les changements
            LoanModified?.Invoke(this, EventArgs.Empty);
            
            ShowSuccess($"✅ Date de retour modifiée avec succès!\n\n" +
                       $"Nouvelle date: {NewReturnDate:dd MMMM yyyy}\n" +
                       $"Référence modification: {modification.Ref_Modification}");
        }
        catch (Exception ex)
        {
            LogError("Erreur lors de la modification", ex);
            ShowError($"Erreur: {ex.Message}");
        }
        finally
        {
            _context?.Dispose();
        }
    }
}
```

***

### **4. LoanReturnViewModel** (Panneau Retour)

**Responsabilité** : Gérer le processus de retour complet

```csharp
public class LoanReturnViewModel : INotifyPropertyChanged
{
    private LibraryDbContext _context;
    private readonly StaffMember _currentStaff;
    private Loan _loan;
    
    // Propriétés du modèle (lecture seule)
    public string Ref_Loan => _loan.Ref_Loan;
    public Book Book => _loan.Book;
    public Subscriber Subscriber => _loan.Subscriber;
    public DateTime BorrowDate => _loan.BorrowDate;
    public DateTime ReturnDate => _loan.ReturnDate;
    public bool IsLate => _loan.IsLate;
    public int DaysLate => _loan.DaysLate;
    public decimal? Penalty => _loan.Penalty;
    
    // Propriétés pour le formulaire
    private string _returnNotes = string.Empty;
    public string ReturnNotes
    {
        get => _returnNotes;
        set { _returnNotes = value; OnPropertyChanged(); }
    }
    
    private bool _isProcessing = false;
    public bool IsProcessing
    {
        get => _isProcessing;
        set
        {
            _isProcessing = value;
            OnPropertyChanged();
            (ConfirmReturnCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }
    }
    
    // Propriétés calculées
    public Loan SelectedLoan => _loan;
    public string StatusMessage => IsLate
        ? $"⚠️ Ce prêt est en retard de {DaysLate} jour(s)"
        : "✅ Ce prêt est à jour";
    
    public Brush StatusColor => IsLate
        ? new SolidColorBrush(Color.FromRgb(244, 67, 54)) // Rouge
        : new SolidColorBrush(Color.FromRgb(76, 175, 80)); // Vert
    
    public string PenaltyMessage => IsLate
        ? $"Pénalité calculée: {DaysLate * 1.0m:C2}"
        : "Aucune pénalité";
    
    public string FidelityImpact => IsLate
        ? $"Fidélité: -{DaysLate * 0.1m:F1} points"
        : "Fidélité: +0.2 points";
    
    // Commandes
    public ICommand ConfirmReturnCommand { get; }
    public ICommand CancelCommand { get; }
    
    // Events
    public event EventHandler CloseRequested;
    public event EventHandler<(Loan loan, string notes)> ReturnConfirmed;
    
    // Constructeur
    public LoanReturnViewModel(Loan loan)
    {
        _loan = loan ?? throw new ArgumentNullException(nameof(loan));
        _currentStaff = ((App)Application.Current).CurrentUser as StaffMember;
        
        ConfirmReturnCommand = new RelayCommand(ConfirmReturn, () => !IsProcessing);
        CancelCommand = new RelayCommand(() => CloseRequested?.Invoke(this, EventArgs.Empty));
    }
    
    // Confirmation du retour
    private void ConfirmReturn()
    {
        try
        {
            IsProcessing = true;
            _context = new LibraryDbContext();
            
            // Recharger le prêt avec relations
            var loanInDb = _context.Loans
                .Include(l => l.Book)
                .Include(l => l.Subscriber)
                .FirstOrDefault(l => l.LoanId == _loan.LoanId);
            
            if (loanInDb == null || !loanInDb.IsActive)
            {
                ShowError("Ce prêt n'est plus actif");
                return;
            }
            
            var book = loanInDb.Book;
            var subscriber = loanInDb.Subscriber;
            
            // Recharger le staff
            var staffInDb = _context.StaffMembers.Find(_currentStaff.Id_User);
            
            // ✅ Calcul du retard et de la pénalité
            var joursRetard = (DateTime.Now.Date - loanInDb.ReturnDate.Date).Days;
            decimal penalite = joursRetard > 0 ? joursRetard * 1.0m : 0;
            
            // ✅ Gestion de la fidélité
            if (joursRetard > 0)
            {
                subscriber.DecreaseFidelity(joursRetard * 0.1m);
            }
            else
            {
                subscriber.IncreaseFidelity(0.2m);
            }
            
            // ✅ Mise à jour du prêt
            loanInDb.IsActive = false;
            loanInDb.ActualReturnDate = DateTime.Now;
            loanInDb.Penalty = penalite;
            
            // ✅ Mise à jour du stock
            book.Quantity++;
            book.IsAvailable = true;
            
            // ✅ Créer la modification (historique)
            var modification = new Modification(
                ModificationType.ReturnLoan,
                loanInDb,
                staffInDb
            );
            _context.Modifications.Add(modification);
            
            // ✅ Sauvegarder avec génération de références
            _context.SaveChangesWithReferences();
            
            // ✅ Message de confirmation détaillé
            var message = $"✅ Retour enregistré avec succès!\n\n" +
                         $"📚 Livre: {book.Title}\n" +
                         $"👤 Membre: {subscriber.Name_User}\n" +
                         $"🔖 Référence: {loanInDb.Ref_Loan}\n" +
                         $"📅 Date d'emprunt: {loanInDb.BorrowDate:dd/MM/yyyy}\n" +
                         $"📅 Date prévue: {loanInDb.ReturnDate:dd/MM/yyyy}\n" +
                         $"📅 Date effective: {DateTime.Now:dd/MM/yyyy}\n" +
                         $"📊 Quantité disponible: {book.Quantity}\n" +
                         $"⭐ Fidélité: {subscriber.Fidelity:F2}/10\n";
            
            if (joursRetard > 0)
            {
                message += $"\n⚠️ Retard: {joursRetard} jour(s)\n" +
                          $"💰 Pénalité: {penalite:C2}";
            }
            
            ShowSuccess(message);
            
            // Notifier l'événement
            ReturnConfirmed?.Invoke(this, (_loan, ReturnNotes));
        }
        catch (Exception ex)
        {
            LogError("Erreur lors du retour", ex);
            ShowError($"Erreur: {ex.Message}");
        }
        finally
        {
            IsProcessing = false;
            _context?.Dispose();
        }
    }
}
```

***

## 🎨 Vues (XAML) Détaillées

### **1. LoanItemView.xaml** (Item de Liste)

**Layout** : Grid à 8 colonnes

```xml
<Grid.ColumnDefinitions>
    <ColumnDefinition Width="1.5*"/>  <!-- Référence -->
    <ColumnDefinition Width="1*"/>    <!-- Abonné -->
    <ColumnDefinition Width="1*"/>    <!-- Livre -->
    <ColumnDefinition Width="1*"/>    <!-- Date Emprunt -->
    <ColumnDefinition Width="1*"/>    <!-- Date Retour -->
    <ColumnDefinition Width="1*"/>    <!-- Statut -->
    <ColumnDefinition Width="1*"/>    <!-- Pénalité -->
    <ColumnDefinition Width="Auto"/>  <!-- Actions -->
</Grid.ColumnDefinitions>
```

**Colonnes** :

1. **Référence** : TextBlock binding `{Binding RefLoan}`, FontSize 13, SemiBold
2. **Abonné** : TextBlock binding `{Binding SubscriberName}`, FontSize 14
3. **Livre** : TextBlock binding `{Binding BookTitle}`, TextTrimming Ellipsis
4. **Date Emprunt** : TextBlock binding `{Binding BorrowDateFormatted}`
5. **Date Retour** : TextBlock binding `{Binding ReturnDateFormatted}`
6. **Statut** : StackPanel horizontal
   - Ellipse (8x8 px) binding `{Binding StatusColor}`
   - TextBlock binding `{Binding StatusLabel}`, même couleur
7. **Pénalité** : TextBlock binding `{Binding PenaltyText}`, couleur dynamique
8. **Actions** : StackPanel horizontal
   - IconButton "Eye" (Voir détails)
   - IconButton "Backburger" (Marquer retourné)

**Animations** :

```xml
<ControlTemplate.Triggers>
    <!-- Hover → Scale 1.05 + Background change -->
    <Trigger Property="IsMouseOver" Value="True">
        <DoubleAnimation To="1.05" Duration="0:0:0.25">
            <DoubleAnimation.EasingFunction>
                <CubicEase EasingMode="EaseOut"/>
            </DoubleAnimation.EasingFunction>
        </DoubleAnimation>
        <ColorAnimation To="#E6EEF9" Duration="0:0:0.3"/>
    </Trigger>
    
    <!-- Pressed → Background #D2E3FC + BorderThickness highlight -->
    <Trigger Property="IsPressed" Value="True">
        <Setter Property="BorderBrush" Value="#4285F4"/>
        <Setter Property="BorderThickness" Value="0,0,4,1"/>
    </Trigger>
</ControlTemplate.Triggers>
```

***

### **2. LoanDetailPanel.xaml** (Panneau Détails)

**Structure** :

```xml
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto"/>   <!-- Header bleu -->
        <RowDefinition Height="*"/>      <!-- Contenu scrollable -->
        <RowDefinition Height="Auto"/>   <!-- Footer actions -->
    </Grid.RowDefinitions>
```

**Header** (Background #1565C0) :
- TextBlock "Détails de l'Emprunt" (FontSize 20, White)
- IconButton Close (Circle, IconKind "Close")

**Contenu (ScrollViewer)** :
- **Référence** : Grande police (24, Bold)
- **Blocs d'informations** (Border arrondis, Background #F5F5F5) :
  1. Livre → `{Binding Book.Title}`
  2. Abonné → `{Binding Subscriber.Name_User}`
  3. Date d'Emprunt → `{Binding BorrowDate, StringFormat='{}{0:dd MMM yyyy}'}`
  4. **Date de Retour** (MODIFIABLE) :
     - Affichage normal → TextBlock
     - Mode édition → DatePicker + boutons Sauvegarder/Annuler
     - Bouton "Modifier" → `Command="{Binding StartEditReturnDateCommand}"`
     - Validation : Message d'erreur binding `{Binding ReturnDateErrorMessage}`
  5. Statut → Ellipse + TextBlock (couleur dynamique)
  6. Pénalité → Si non null, affichage montant
  7. Retard → Nombre de jours

**Footer** :
- **Bouton principal** : "📦 Clôturer cet emprunt"
  - Background #1565C0
  - Command `{Binding OpenReturnPanelCommand}`
  - IsEnabled `{Binding IsActive}` (désactivé si déjà retourné)
- **Bouton secondaire** : "Fermer"
  - Background Transparent, Bordure bleu
  - Command `{Binding CloseCommand}`

***

### **3. LoanReturnPanel.xaml** (Panneau Retour)

**Structure identique** (Header / Content / Footer)

**Contenu spécifique** :

1. **Bloc Récapitulatif** :
   - Référence, Livre, Abonné, Dates

2. **Bloc Calculs** (Background #FFF9C4 si retard, #E8F5E9 sinon) :
   - **Message de statut** : `{Binding StatusMessage}` (couleur dynamique)
   - **Pénalité** : `{Binding PenaltyMessage}`
   - **Impact fidélité** : `{Binding FidelityImpact}`

3. **Zone de notes** :
   - TextBox multilignes
   - Binding `{Binding ReturnNotes, Mode=TwoWay}`
   - PlaceholderText : "Notes sur l'état du livre (optionnel)..."

**Footer** :

```xml
<Button Content="✅ Confirmer le retour"
        Background="#4CAF50"
        Command="{Binding ConfirmReturnCommand}"
        IsEnabled="{Binding IsProcessing, Converter={StaticResource InverseBoolConverter}}">
    <!-- Spinner si IsProcessing = true -->
</Button>

<Button Content="Annuler"
        Background="Transparent"
        BorderBrush="#DC3545"
        Command="{Binding CancelCommand}"/>
```

***

## 🔧 Services Métier

### **1. LoanSearchService**

```csharp
public class LoanSearchService
{
    public IEnumerable<Loan> Search(IEnumerable<Loan> loans, string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
            return loans;
        
        var lowerSearch = searchText.ToLower().Trim();
        
        return loans.Where(loan =>
            loan.Ref_Loan?.ToLower().Contains(lowerSearch) == true ||
            loan.Subscriber?.Name_User?.ToLower().Contains(lowerSearch) == true ||
            loan.Book?.Title?.ToLower().Contains(lowerSearch) == true ||
            loan.Book?.Author?.ToLower().Contains(lowerSearch) == true ||
            loan.Book?.ISBN?.ToLower().Contains(lowerSearch) == true
        );
    }
}
```

### **2. LoanFilterService**

```csharp
public class LoanFilterService
{
    public IEnumerable<Loan> ApplyFilters(
        IEnumerable<Loan> loans,
        IEnumerable<FilterLabelViewModel> activeFilters)
    {
        if (loans == null || !loans.Any()) return Enumerable.Empty<Loan>();
        if (activeFilters == null || !activeFilters.Any()) return loans;
        
        var result = loans;
        
        foreach (var filter in activeFilters)
        {
            result = ApplyFilter(result, filter);
        }
        
        return result;
    }
    
    private IEnumerable<Loan> ApplyFilter(
        IEnumerable<Loan> loans,
        FilterLabelViewModel filter)
    {
        if (filter.Type == "Statut")
        {
            return filter.Value switch
            {
                "En cours" => loans.Where(l => l.IsActive),
                "Retourné" => loans.Where(l => !l.IsActive),
                "En retard" => loans.Where(l => l.IsLate),
                _ => loans
            };
        }
        
        if (filter.Type == "Pénalité")
        {
            return filter.Value switch
            {
                "Avec pénalité" => loans.Where(l => l.Penalty.HasValue && l.Penalty > 0),
                "Sans pénalité" => loans.Where(l => !l.Penalty.HasValue || l.Penalty == 0),
                _ => loans
            };
        }
        
        return loans;
    }
}
```

### **3. LoanSortService**

```csharp
public class LoanSortService
{
    public IEnumerable<Loan> SortByColumnName(
        IEnumerable<Loan> loans,
        string columnName,
        bool ascending = true)
    {
        if (loans == null || !loans.Any())
            return Enumerable.Empty<Loan>();
        
        IOrderedEnumerable<Loan> sorted = columnName switch
        {
            "Référence" => ascending
                ? loans.OrderBy(l => l.Ref_Loan)
                : loans.OrderByDescending(l => l.Ref_Loan),
            
            "Abonné" => ascending
                ? loans.OrderBy(l => l.Subscriber?.Name_User ?? string.Empty)
                : loans.OrderByDescending(l => l.Subscriber?.Name_User ?? string.Empty),
            
            "Livre" => ascending
                ? loans.OrderBy(l => l.Book?.Title ?? string.Empty)
                : loans.OrderByDescending(l => l.Book?.Title ?? string.Empty),
            
            "Date d'Emprunt" => ascending
                ? loans.OrderBy(l => l.BorrowDate)
                : loans.OrderByDescending(l => l.BorrowDate),
            
            "Date de Retour" => ascending
                ? loans.OrderBy(l => l.ReturnDate)
                : loans.OrderByDescending(l => l.ReturnDate),
            
            "Statut" => ascending
                ? loans.OrderBy(l => l.IsActive).ThenBy(l => l.IsLate)
                : loans.OrderByDescending(l => l.IsActive).ThenByDescending(l => l.IsLate),
            
            "Pénalité" => ascending
                ? loans.OrderBy(l => l.Penalty ?? 0)
                : loans.OrderByDescending(l => l.Penalty ?? 0),
            
            _ => loans.OrderBy(l => l.BorrowDate)
        };
        
        return sorted;
    }
}
```

***

## GenericMainView (Vue Générique Réutilisable)

### **Concept**

`GenericMainView` est une **vue générique** qui peut afficher n'importe quel catalogue (Livres, Abonnés, Emprunts) en implémentant l'interface `ICatalogViewModel`.

**Avantages** :
- Code unique pour tous les catalogues
- Composants modulaires et réutilisables
- Maintenance simplifiée
- Cohérence visuelle

### **Architecture**

```xml
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto"/>   <!-- Header -->
        <RowDefinition Height="Auto"/>   <!-- Separator -->
        <RowDefinition Height="Auto"/>   <!-- NavigationBar -->
        <RowDefinition Height="*"/>      <!-- Main Content -->
        <RowDefinition Height="Auto"/>   <!-- StatusBar -->
    </Grid.RowDefinitions>
```

**Composants intégrés** :

1. **Header** : Logo + Titre + Image latérale
2. **NavigationBar** : Recherche + Boutons (Retour, Reload, ToggleView)
3. **ActiveFiltersPanel** : Badges de filtres actifs
4. **SearchResultMessage** : "X résultats pour 'texte'"
5. **ColumnHeadersPanel** : En-têtes de colonnes triables
6. **ItemsGrid** : UniformGrid contenant les items
7. **SidePanelBorder** : Panneau latéral slide-in (détails/actions)
8. **StatusBar** : Nombre total d'items

### **Interface ICatalogViewModel**

```csharp
public interface ICatalogViewModel : INotifyPropertyChanged
{
    // Collections
    IEnumerable DisplayedItems { get; }
    ObservableCollection<FilterLabelViewModel> ActiveFilters { get; }
    ObservableCollection<ColumnHeaderViewModel> Headers { get; }
    
    // Propriétés
    string SearchText { get; set; }
    string CatalogTitle { get; }
    string EmptyMessage { get; }
    int TotalResults { get; }
    bool HasResults { get; }
    bool HasFilters { get; }
    ViewMode _currentviewmode { get; }
    string ItemsName { get; }
    
    // Méthodes
    void Initialize();
    void Cleanup();
    UIElement CreateDisplayItem(object item); // Créer un item de liste
    
    // Commandes
    ICommand SearchCommand { get; }
    ICommand ClearSearchCommand { get; }
    ICommand SortCommand { get; }
    ICommand ToggleViewModeCommand { get; }
    ICommand RemoveFilterCommand { get; }
    ICommand ReloadCommand { get; }
    ICommand BackCommand { get; set; }
}
```

### **Fonctionnement**

```csharp
public partial class GenericMainView : UserControl
{
    public ICatalogViewModel ViewModel { get; private set; }
    
    public GenericMainView(ICatalogViewModel viewModel)
    {
        ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        DataContext = ViewModel;
        InitializeComponent();
        
        Loaded += OnLoaded;
    }
    
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        ViewModel?.Initialize();
        ConfigureComponents();
        SubscribeToEvents();
        UpdateDisplay();
    }
    
    // Configuration des composants
    private void ConfigureComponents()
    {
        NavigationBar_.DataContext = ViewModel;
        NavigationBar_._SearchTextBox.TextChanged += SearchTextBox_TextChanged;
        NavigationBar_.ToggleViewCommand = ViewModel.ToggleViewModeCommand;
        
        ActiveFiltersPanel_.DataContext = ViewModel;
        ActiveFiltersPanel_.Filters = ViewModel.ActiveFilters;
        ActiveFiltersPanel_.CloseFilterCommand = ViewModel.RemoveFilterCommand;
        
        ColumnHeaderPanel_.DataContext = ViewModel;
        ColumnHeaderPanel_.Headers = ViewModel.Headers;
        
        StatusBar_.DataContext = ViewModel;
    }
    
    // Mise à jour de l'affichage
    private void UpdateDisplay()
    {
        try
        {
            ItemsGrid.Children.Clear();
            
            if (!ViewModel.HasResults)
            {
                // Afficher message "Aucun résultat"
                var emptyMessage = new TextBlock
                {
                    Text = ViewModel.EmptyMessage,
                    FontSize = 18,
                    Foreground = Brushes.Gray,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(20)
                };
                ItemsGrid.Children.Add(emptyMessage);
                return;
            }
            
            // Créer les items via le ViewModel
            foreach (var item in ViewModel.DisplayedItems)
            {
                var uiElement = ViewModel.CreateDisplayItem(item);
                if (uiElement != null)
                {
                    ItemsGrid.Children.Add(uiElement);
                }
            }
            
            SyncSearchAndResults();
        }
        catch (Exception ex)
        {
            LogError("Erreur UpdateDisplay", ex);
        }
    }
    
    // Synchronisation du message de recherche
    private void SyncSearchAndResults()
    {
        if (SearchResultMessage_ == null) return;
        
        int actualCount = ItemsGrid.Children.Count;
        
        // Exclure le message "Aucun résultat"
        var hasEmptyMessage = ItemsGrid.Children.OfType<TextBlock>()
            .Any(tb => tb.Text == ViewModel.EmptyMessage);
        
        if (hasEmptyMessage) actualCount = 0;
        
        SearchResultMessage_.Message = NavigationBar_._SearchTextBox.Text;
        SearchResultMessage_.ResultCount = actualCount;
        SearchResultMessage_.IsVisible_ = !string.IsNullOrWhiteSpace(ViewModel.SearchText);
    }
}
```

***

## Orchestrateur LoanCatalog.xaml.cs

**Responsabilité** : Pont entre `GenericMainView` et les panneaux latéraux

```csharp
public partial class LoanCatalog : UserControl
{
    private LoanCatalogViewModel _viewModel;
    private GenericMainView _mainView;
    private readonly LibraryDbContext _context;
    
    public LoanCatalog()
    {
        InitializeComponent();
        
        _context = App.LibraryDbContext;
        _viewModel = new LoanCatalogViewModel();
        _mainView = new GenericMainView(_viewModel);
        
        SetupNavigation();
        SetupEvents();
        
        this.MainContent.Content = _mainView;
        Loaded += LoanCatalog_Loaded;
    }
    
    private void SetupEvents()
    {
        _viewModel.LoanDetailRequested += OnLoanDetailRequested;
        _viewModel.LoanReturnRequested += OnLoanReturnRequested;
    }
    
    // Affichage du panneau de détails
    private void OnLoanDetailRequested(object sender, Loan loan)
    {
        var loanInDb = _context.Loans
            .Include(l => l.Book)
            .Include(l => l.Subscriber)
            .Include(l => l.Modifications)
            .FirstOrDefault(l => l.LoanId == loan.LoanId);
        
        if (loanInDb == null)
        {
            MessageBox.Show("Ce prêt n'existe plus", "Erreur");
            return;
        }
        
        var detailViewModel = new LoanDetailViewModel(loanInDb);
        
        // S'abonner aux événements
        detailViewModel.CloseRequested += (s, e) => CloseSidePanel();
        detailViewModel.ReturnPanelRequested += (s, l) =>
        {
            CloseSidePanel();
            OnLoanReturnRequested(this, l);
        };
        detailViewModel.LoanModified += (s, e) => RefreshLoansFromDatabase();
        
        var detailPanel = new LoanDetailPanel { DataContext = detailViewModel };
        
        detailPanel.Loaded += (s, e) =>
        {
            detailPanel.CloseBtn.AddHandler(
                IconButton.ClickEvent,
                new RoutedEventHandler((sender, args) => CloseSidePanel())
            );
        };
        
        ShowSidePanel(detailPanel);
    }
    
    // Affichage du panneau de retour
    private void OnLoanReturnRequested(object sender, Loan loan)
    {
        var loanInDb = _context.Loans
            .Include(l => l.Book)
            .Include(l => l.Subscriber)
            .FirstOrDefault(l => l.LoanId == loan.LoanId);
        
        if (loanInDb == null || !loanInDb.IsActive)
        {
            MessageBox.Show("Ce prêt n'est plus actif", "Erreur");
            return;
        }
        
        var returnViewModel = new LoanReturnViewModel(loanInDb);
        
        returnViewModel.CloseRequested += (s, e) => CloseSidePanel();
        returnViewModel.ReturnConfirmed += (s, args) =>
        {
            var (returnedLoan, notes) = args;
            CloseSidePanel();
            RefreshLoansFromDatabase();
        };
        
        var returnPanel = new LoanReturnPanel { DataContext = returnViewModel };
        
        returnPanel.Loaded += (s, e) =>
        {
            returnPanel.CloseBtn.AddHandler(
                IconButton.ClickEvent,
                new RoutedEventHandler((sender, args) => CloseSidePanel())
            );
        };
        
        ShowSidePanel(returnPanel);
    }
    
    // Gestion du panneau latéral
    private void ShowSidePanel(UIElement content)
    {
        _mainView.DisplayPanel.Content = content;
        _mainView.SidePanelBorder.Visibility = Visibility.Visible;
        AnimatePanelOpen();
    }
    
    private void CloseSidePanel()
    {
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
            _mainView.DisplayPanel.Content = null;
        };
        
        _mainView.SidePanelBorder.BeginAnimation(UIElement.OpacityProperty, fadeOut);
    }
    
    private void AnimatePanelOpen()
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
    
    // Rafraîchissement depuis la DB
    private void RefreshLoansFromDatabase()
    {
        var loansFromDb = _context.Loans
            .Include(l => l.Book)
            .Include(l => l.Subscriber)
            .Include(l => l.Modifications)
            .OrderByDescending(l => l.BorrowDate)
            .ToList();
        
        _viewModel.AllLoans.Clear();
        foreach (var loan in loansFromDb)
        {
            _viewModel.AllLoans.Add(loan);
        }
        
        _viewModel.ReloadCommand?.Execute(null);
    }
}
```

***

## Technologies et Outils

| Technologie               | Utilisation                                          |
|---------------------------|------------------------------------------------------|
| **WPF (XAML)**            | Interface utilisateur déclarative                    |
| **MVVM Pattern**          | Séparation Vue / ViewModel / Modèle                  |
| **Entity Framework Core** | Accès base de données, Include(), SaveChanges()      |
| **MVVM Light Toolkit**    | RelayCommand, INotifyPropertyChanged                 |
| **ObservableCollection**  | Binding bidirectionnel réactif                       |
| **LINQ**                  | Requêtes de recherche, filtrage, tri                 |
| **Animations WPF**        | DoubleAnimation, ColorAnimation, Storyboard          |
| **DependencyProperty**    | Propriétés bindables des UserControls                |
| **ICatalogViewModel**     | Interface générique pour réutilisabilité             |
| **EventAggregator Pattern**| Communication inter-ViewModels                      |

***

## Flux de Données Complet

```
[DB: Loans Table]
        ↓
[LoanCatalog.Initialize()]
        ↓
[LoanCatalogViewModel.AllLoans ← Context.Loans.ToList()]
        ↓
[Services: Search → Filter → Sort]
        ↓
[LoanCatalogViewModel.DisplayedLoans]
        ↓
[GenericMainView.UpdateDisplay()]
        ↓
[Création des LoanItemView via CreateDisplayItem()]
        ↓
[Ajout à ItemsGrid.Children]
        ↓
[Affichage dans UI]
        ↓
[User clique "Voir détails"]
        ↓
[LoanDetailRequested Event → OnLoanDetailRequested()]
        ↓
[Rechargement depuis DB avec Include()]
        ↓
[Création LoanDetailViewModel + LoanDetailPanel]
        ↓
[Affichage panneau latéral (ShowSidePanel)]
        ↓
[User modifie date OU clique "Clôturer"]
        ↓
[Sauvegarde DB + Génération Modification]
        ↓
[LoanModified Event → RefreshLoansFromDatabase()]
        ↓
[Mise à jour AllLoans + Réapplication des filtres]
        ↓
[UpdateDisplay() → UI rafraîchie]
```

***

## Fonctionnalités Clés

✅ **Architecture générique** : Réutilisable pour tous les catalogues  
✅ **Recherche en temps réel** : Multi-champs, instantanée  
✅ **Tri multi-colonnes** : ASC/DESC avec indicateur visuel  
✅ **Filtrage avancé** : Cumulable avec badges supprimables  
✅ **Panneaux latéraux** : Slide-in avec animations fluides  
✅ **Édition de date** : Avec validation et historique  
✅ **Retour complet** : Calcul auto pénalité + fidélité  
✅ **Rafraîchissement intelligent** : Depuis DB après modification  
✅ **Feedback détaillé** : MessageBox complet après chaque action  
✅ **Separation of Concerns** : Services dédiés Search/Filter/Sort  
✅ **Event-driven** : Communication via événements typés  

***

## Améliorations Futures

- **Export CSV/PDF** : Liste filtrée
- **Graphiques statistiques** : Retards par mois, top emprunts
- **Notifications** : Email/SMS automatiques retards
- **Historique complet** : Timeline des modifications par emprunt
- **Mode hors-ligne** : Cache local avec synchronisation
- **Impression** : Reçu de retour avec code-barres

***
