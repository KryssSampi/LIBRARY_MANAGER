
***

# README Module BookCatalog (Mise à Jour Complète)

## Vue d'Ensemble

Le module **BookCatalog** est le **cœur de l'application de gestion de bibliothèque**, offrant une **expérience utilisateur complète en deux étapes** :

1. **HomeView** : Écran d'accueil avec **grille de catégories animée** et recherche globale
2. **MainView (BookCatalogView)** : Catalogue interactif avec **recherche avancée, filtrage multi-critères, tri, et double affichage (Liste/Grille)**

**Complexité** : ⭐⭐⭐⭐⭐⭐ (Module le plus sophistiqué de l'application)

---

##  Architecture Complète (Version Finale)

```
/BookCatalog
├── BookCatalog.xaml & .cs                      # Orchestrateur principal + Navigation
├── /Views
│   ├── /HomeView
│   │   ├── HomeView.xaml & .cs                 # Page d'accueil avec catégories
│   │   └── /Style
│   │       └── StyleManager.xaml               # Styles visuels
│   └── /MainView
│       └── BookCatalogView.xaml & .cs          # Vue catalogue principale
├── /ViewModels
│   ├── BookCatalogMainViewModel.cs             # ViewModel principal (ICatalogViewModel)
│   ├── BookViewModel.cs                        # Adaptateur Book → UI (NOUVEAU ✨)
│   └── BookItemViewModel.cs                    # ViewModel item liste/grille
├── /Core
│   ├── /Services
│   │   ├── BookSearchService.cs                # Recherche multi-champs
│   │   ├── BookFilterService.cs                # Filtrage 9 dimensions
│   │   ├── BookSortService.cs                  # Tri 9 colonnes
│   │   ├── CategoryCountService.cs             # Comptage par catégorie
│   │   └── NavigationService.cs                # Navigation avec animations
│   └── /Interfaces
│       └── ICatalogViewModel.cs                # Interface commune
├── /Items
│   ├── CategoryGrid.xaml & .cs                 # Grille de tuiles de catégories
│   ├── CategoryTile.xaml & .cs                 # Tuile animée (une catégorie)
│   ├── SearchBar.xaml & .cs                    # Barre de recherche réutilisable
│   ├── **BookListItemView.xaml & .cs**         # Item mode liste (IMPLÉMENTÉ ✨)
│   └── **BookGridItemView.xaml & .cs**         # Item mode grille (IMPLÉMENTÉ ✨)
├── /Panels
│   ├── **BookDetailPanel.xaml & .cs**          # Panneau latéral détails (IMPLÉMENTÉ ✨)
│   └── **BookBorrowPanel.xaml & .cs**          # Panneau latéral emprunt (IMPLÉMENTÉ ✨)
├── /Config
│   └── CategoryConfig.cs                       # Configuration 40+ catégories
├── /Helpers
│   └── GetPathHelpers.cs                       # Helpers pour chemins assets
└── /Common (Partagé)
    └── GenericMainView.xaml & .cs              # Vue générique réutilisable
```

***

## Composants Implémentés (Analyse Détaillée)

### **1. BookViewModel.cs** (Adaptateur Model → UI) 

**Responsabilité** : Transformer un `Book` en propriétés UI-friendly avec conversion automatique

**Propriétés du modèle (Pass-through)** :
```csharp
public long BookId => _book.BookId;
public string Title => _book.Title;
public string Author => _book.Author;
public string ISBN => _book.ISBN;
public string? Category => _book.Category;
public string? Description => _book.Description;
public DateTime? PublishDate => _book.PublishDate;
public string? Publisher => _book.Publisher;
public string? Language => _book.Language ?? "Français";
public int Quantity => _book.Quantity;
public bool IsAvailable => _book.IsAvailable;
public DateTime DateAdded => _book.DateAdded;
public string? CoverUrl => _book.CoverUrl;
```

**Propriétés calculées pour l'UI** :

| Propriété | Calcul | Exemple |
|-----------|--------|---------|
| `DisplayCoverUrl` | Fallback si vide | `pack://application:,,,/Assets/default-book-cover.png` |
| `DisplayCategory` | Enum → Texte formaté | `CategoryAllowed.Science` → `"Science"` |
| `DisplayPublishDate` | `dd/MM/yyyy` | `2021-05-15` → `"15/05/2021"` |
| `PublishYear` | Année seule | `2021` |
| `DisplayAddedDate` | `dd/MM/yyyy` | `"12/11/2025"` |
| `AvailabilityText` | Texte coloré | `"Stock faible (3)"` ou `"Rupture de stock"` |
| `AvailabilityColor` | Brush (4 couleurs) | Vert/Orange/Bleu/Rouge selon Quantity |
| `CategoryColor` | Couleur thématique | Récupère de `CategoryConfiguration` |
| `IsNew` | Propriété Book | Badge "NOUVEAU" si vrai |
| `IsLowStock` | `Quantity < 5` | Badge ou indicateur |
| `IsPopular` | `Quantity < 5` (logique simple) | Badge "POPULAIRE" |

**Couleurs de disponibilité** :
```csharp
public SolidColorBrush AvailabilityColor
{
    get
    {
        if (Quantity == 0)
            return new SolidColorBrush(Color.FromRgb(244, 67, 54)); // 🔴 Rouge
        if (Quantity <= 3)
            return new SolidColorBrush(Color.FromRgb(255, 152, 0)); // 🟠 Orange
        if (Quantity <= 10)
            return new SolidColorBrush(Color.FromRgb(76, 175, 80));  // 🟢 Vert
        return new SolidColorBrush(Color.FromRgb(33, 150, 243)); // 🔵 Bleu
    }
}
```

**Méthodes de conversion** :
```csharp
// Convertit un Book en BookListItem (mode liste)
public static BookListItem GetListItem(Book book)
{
    var vm = new BookViewModel(book);
    return new BookListItem
    {
        DataContext = vm,
        DisplayCoverUrl = vm.DisplayCoverUrl,
        Title = vm.Title,
        Author = vm.Author,
        DisplayCategory = vm.DisplayCategory,
        ISBN = vm.ISBN,
        Publisher = vm.Publisher ?? "Non renseigné",
        PublicationDate = vm.PublishDate ?? DateTime.MinValue,
        Language = vm.Language,
        Quantity = vm.Quantity,
        AvailabilityColor = vm.AvailabilityColor,
        AddedOn = vm.DateAdded,
        IsNew = vm.IsNew,
        IsPopular = vm.IsPopular,
        IsSelected = false
    };
}

// Convertit un Book en BookGridItem (mode grille)
public static BookGridItem GetGridItem(Book book)
{
    var vm = new BookViewModel(book);
    return new BookGridItem { DataContext = vm };
}

// Accès au modèle original
public Book GetBook() => _book;
```

**Rafraîchissement des données** :
```csharp
public void RefreshAll()
{
    OnPropertyChanged(nameof(Quantity));
    OnPropertyChanged(nameof(IsAvailable));
    OnPropertyChanged(nameof(AvailabilityText));
    OnPropertyChanged(nameof(AvailabilityColor));
    OnPropertyChanged(nameof(IsLowStock));
}
```

***

### **2. BookListItemView** (Mode Liste - 80px de hauteur)

**Layout** : Grid avec **10 colonnes** redimensionnables

**Colonnes** :
1. **Titre + Badges** (2*) : Titre Bold 16px + Badges "NOUVEAU"/"POPULAIRE"
2. **Auteur** (1.2*) : TextBlock 14px gris
3. **Catégorie** (1*) : Nom catégorie
4. **ISBN** (1*) : Texte 13px gris avec Tooltip
5. **Éditeur** (1*) : TextBlock 14px centré
6. **Année** (1*) : Format YYYY
7. **Langue** (1*) : Code langue
8. **Quantité** (1*) : Ellipse colorée + nombre
9. **Date ajoutée** (1*) : Format `dd MMM yyyy`
10. **Actions** (Auto) : 2 IconButtons (Œil + Bookmark)

**Design** :
- **Gradient Background** : Linear (#FAFAFA → #F4F7FA)
- **Border** : Gris clair #E0E0E0 (bottom only)
- **Padding** : 10,15,25,10
- **Height** : 80px

**Animations** :
```xml
<!-- Hover -->
<Trigger Property="IsMouseOver" Value="True">
    <ColorAnimation To="#E6EEF9"/>
    <DoubleAnimation ScaleX="1.05" Duration="0:0:0.25"/>
    <DoubleAnimation ScaleY="1.05" Duration="0:0:0.25"/>
</Trigger>

<!-- Appui -->
<Trigger Property="IsPressed" Value="True">
    <Setter BorderBrush="#4285F4"/>
    <Setter BorderThickness="0,0,4,1"/>
    <Setter Background LinearGradient "#E8F0FE" → "#D2E3FC"/>
</Trigger>
```

**Code-behind** :
```csharp
#region Dependency Properties
public ICommand ShowDetailCommand { get; set; }
public ICommand BorrowCommand { get; set; }
#endregion

private void EyeBtn_Click(object sender, MouseButtonEventArgs e)
{
    e.Handled = true;
    if (ShowDetailCommand?.CanExecute(DataContext) ?? false)
        ShowDetailCommand.Execute(DataContext);
}

private void BorrowBtn_Click(object sender, MouseButtonEventArgs e)
{
    e.Handled = true;
    if (BorrowCommand?.CanExecute(DataContext) ?? false)
        BorrowCommand.Execute(DataContext);
}
```

***

### **3. BookGridItemView** (Mode Grille - Card 280x380px)

**Structure** :
- **Image** : 280x380px (pleine hauteur)
- **Overlay footer** : Titre (2 lignes) + Auteur + Catégorie + Actions
- **Context menu** : Clic droit ou bouton menu

**Propriétés DependencyProperty** :
```csharp
public ICommand BorrowBtnClick { get; set; }
public ICommand ShowDetailBtnClick { get; set; }
```

**Interactions** :
```csharp
// ✅ Menu contextuel sur clic droit
private void BookGridItem_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
{
    e.Handled = true;
    ActionMenu.IsOpen = true;
}

// ✅ Popup sur clic bouton menu
private void OnMenuButtonClick(object sender, RoutedEventArgs e)
{
    e.Handled = true;
    ActionMenu.IsOpen = true;
}
```

***

### **4. BookDetailPanel** (Panneau latéral 400px)

**Structure à 3 zones** :

#### **Header (Background #1565C0)** :
- Titre : "Détails du Livre" (White, Bold)
- IconButton Close (Cercle transparent)

#### **Contenu scrollable** :
1. **Couverture** : 400px hauteur, DropShadow
2. **Titre** : FontSize 24, Bold
3. **Auteur** : "par [Nom]" (Lien bleu #1565C0)
4. **Grille d'infos** (Background #F5F5F5, CornerRadius 8) :
   - ISBN
   - Catégorie
   - Date de publication
   - Disponibilité (avec Ellipse colorée + texte)
5. **Description** : TextBlock avec TextWrapping

#### **Footer** :
- **Bouton "Emprunter ce livre"** : #1565C0, Hover #1976D2
- **Bouton "Fermer"** : Transparent, Border #1565C0

**Code-behind** :
```csharp
public ICommand BorrowBtnClick { get; set; }
public ICommand CloseCommand { get; set; }

private void OnCloseRequested()
{
    if (CloseCommand?.CanExecute(null) ?? false)
        CloseCommand.Execute(null);
}

public Button FooterCloseButton => FindName("FooterCloseBtn") as Button;
public IconButton HeaderCloseButton => CloseBtn;
```

***

### **5. BookBorrowPanel** (Panneau latéral emprunt 400px) 

**Structure à 3 zones** :

#### **Header (Background #4CAF50)** :
- Titre : "Emprunter un Livre" (White)
- IconButton Close

#### **Contenu scrollable** :

1. **Résumé du livre** (Background #F5F5F5) :
   - Miniature couverture (80x100)
   - Titre, Auteur, ISBN

2. **Zone saisie abonné** :
   - TextBox "ID de l'abonné" (Format nombre)
   - Validation embarquée

3. **Sélection date de retour** :
   - DateSelector personnalisé
   - Boutons rapides : 7j / 14j / 21j

4. **Notes (optionnel)** :
   - TextBox 80px MultiLine

#### **Footer** :
- **Bouton "Confirmer l'emprunt"** : #4CAF50, Hover #66BB6A
- **Bouton "Annuler"** : Transparent

**Logique métier complète** :

```csharp
private void ConfirmBorrow_Click(object sender, RoutedEventArgs e)
{
    try
    {
        // ✅ Validations
        if (string.IsNullOrWhiteSpace(SubscriberSearchBox.Text))
        {
            ShowError("Veuillez entrer l'ID de l'abonné.");
            return;
        }
        
        if (!int.TryParse(SubscriberSearchBox.Text, out int subscriberId))
        {
            ShowError("L'ID de l'abonné doit être un nombre valide.");
            return;
        }
        
        if (!ReturnDatePicker.SelectedDate.HasValue)
        {
            ShowError("Veuillez sélectionner une date de retour.");
            return;
        }
        
        if (ReturnDatePicker.SelectedDate.Value.Date <= DateTime.Today)
        {
            ShowError("La date de retour doit être dans le futur.");
            return;
        }
        
        // ✅ Création de l'emprunt
        CreateLoan(subscriberId, ReturnDatePicker.SelectedDate.Value);
    }
    catch (Exception ex)
    {
        ShowError($"Erreur lors de la confirmation:\n{ex.Message}");
    }
}

private void CreateLoan(int subscriberId, DateTime returnDate)
{
    var context = new LibraryDbContext();
    
    // ✅ Vérifier le livre
    var book = context.Books.FirstOrDefault(b => b.BookId == _currentBook.BookId);
    if (book == null || book.Quantity <= 0)
    {
        ShowError("Ce livre n'est pas disponible.");
        return;
    }
    
    // ✅ Vérifier l'abonné
    var subscriber = context.Subscribers
        .Include(s => s.Loans)
        .FirstOrDefault(s => s.Id_User == subscriberId);
    
    if (subscriber == null)
    {
        ShowError("Cet abonné n'existe pas.");
        return;
    }
    
    // ✅ Vérifier la capacité
    if (!subscriber.ValidateCanBorrow(out string errorMsg))
    {
        ShowError(errorMsg);
        return;
    }
    
    // ✅ Vérifier double emprunt
    var existingLoan = context.Loans.FirstOrDefault(l =>
        l.BookId == book.BookId &&
        l.SubscriberId == subscriberId &&
        l.IsActive
    );
    
    if (existingLoan != null)
    {
        ShowError("Cet abonné a déjà emprunté ce livre.");
        return;
    }
    
    // ✅ Créer l'emprunt
    var loan = new Loan(subscriber, book)
    {
        ReturnDate = returnDate,
        IsActive = true,
        Penalty = 0
    };
    
    // ✅ Dialog de confirmation
    var msg = $"Souhaitez-vous confirmer cet enregistrement?\n\n" +
              $"📚 Livre: {book.Title}\n" +
              $"👤 Abonné: {subscriber.Name_User}\n" +
              $"📅 Date emprunt: {loan.BorrowDate:dd/MM/yyyy}\n" +
              $"📅 Date retour: {loan.ReturnDate:dd/MM/yyyy}";
    
    if (!ShowConfirmation(msg))
    {
        ShowInfo("Enregistrement annulé par l'utilisateur.");
        return;
    }
    
    // ✅ Sauvegarder en DB
    context.Loans.Add(loan);
    book.Quantity--;
    if (book.Quantity == 0)
        book.IsAvailable = false;
    
    subscriber.IncreaseFidelity(0.2m);
    
    int saved = context.SaveChanges();
    
    if (saved > 0)
    {
        context.GenerateReferences();
        
        ShowSuccess(
            $"✅ Emprunt enregistré!\n\n" +
            $"📚 Livre: {book.Title}\n" +
            $"👤 Abonné: {subscriber.Name_User}\n" +
            $"🔖 Référence: {loan.Ref_Loan}\n" +
            $"📅 Date emprunt: {loan.BorrowDate:dd/MM/yyyy}\n" +
            $"📅 Date retour: {loan.ReturnDate:dd/MM/yyyy}\n" +
            $"📊 Quantité restante: {book.Quantity}"
        );
        
        BorrowCommand?.Execute(null);
        OnCloseRequested();
    }
}
```

**Boutons rapides** :
```csharp
private void Set7Days_Click(object sender, RoutedEventArgs e) => SetReturnDate(7);
private void Set14Days_Click(object sender, RoutedEventArgs e) => SetReturnDate(14);
private void Set21Days_Click(object sender, RoutedEventArgs e) => SetReturnDate(21);

private void SetReturnDate(int days)
{
    ReturnDatePicker.SelectedDate = DateTime.Today.AddDays(days);
}
```

***

## Flux de Données Complet (Mise à Jour)

```
[DB: Books Table]
        ↓
[BookViewModel.GetListItem(book)]
        ↓
[BookListItemView avec DependencyProperties]
        ↓
[User clique Eye → ShowDetailCommand]
        ↓
[BookDetailPanel affichée (Slide-in 400px)]
        ↓
[Affichage complet du livre avec styling]
        ↓
[User clique "Emprunter ce livre"]
        ↓
[BookBorrowPanel affichée (Slide-in 400px)]
        ↓
[Saisie ID abonné + Sélection date]
        ↓
[Validation complète (10+ chèques)]
        ↓
[Dialog de confirmation]
        ↓
[Création Loan + Update Book.Quantity]
        ↓
[Update Subscriber.Fidelity]
        ↓
[SaveChanges() + GenerateReferences()]
        ↓
[Message succès détaillé]
        ↓
[Fermeture panneaux + Refresh catalogue]
```

***

##  Scénarios d'Utilisation Complets

### **Scénario 1 : Emprunt depuis la liste**
1. User clique Eye sur une ligne
2. BookDetailPanel s'affiche (Slide-in depuis droite)
3. User clique "Emprunter ce livre"
4. BookBorrowPanel s'affiche
5. User saisit "123" (ID abonné)
6. User clique "14 jours" (Raccourci)
7. User clique "Confirmer l'emprunt"
8. ✅ Dialog confirmation, puis SaveChanges()
9. Message success avec détails (Ref, Quantité restante)
10. Panels fermés, catalogue rafraîchi

### **Scénario 2 : Consultation depuis la grille**
1. User survole card (Scale 1.05 + ombre)
2. User clique sur l'image ou "Voir détails"
3. BookDetailPanel s'affiche
4. User lit les infos complètes
5. User clique "Fermer" ou X
6. Panel se ferme (Fade-out)

### **Scénario 3 : Gestion erreurs d'emprunt**
1. User entre un ID inexistant → ShowError()
2. User entre un abonné sans fidélité → ShowError() + Description règle
3. User choisit date passée → ShowError()
4. User essaie double emprunt → ShowError()
5. User clique "Annuler" → Panel ferme sans action

---

## Patterns & Bonnes Pratiques Implémentées

✅ **DependencyProperty** : Bindage WPF natif  
✅ **RelayCommand** : MVVM Light  
✅ **INotifyPropertyChanged** : Réactivité UI  
✅ **Binding** : DataContext du parent  
✅ **Validation multi-niveaux** : UI + Métier + DB  
✅ **Try-Catch avec logging** : Debug.WriteLine()  
✅ **MessageBox** : UX utilisateur  
✅ **Animations fluides** : Storyboard + EasingFunction  
✅ **Scroll viewer** : Contenu long  
✅ **Gradient brush** : Design moderne  
✅ **Convention naming** : _private, camelCase  
✅ **Séparation concerns** : ViewModel / View / Service  

***

##  Résumé des Fonctionnalités Clés

✅ **BookViewModel** : Adaptateur élégant et puissant  
✅ **BookListItemView** : Liste professionnelle avec animations  
✅ **BookGridItemView** : Grille responsive avec context menu  
✅ **BookDetailPanel** : Détails complets + action emprunt  
✅ **BookBorrowPanel** : Workflow complet d'emprunt  
✅ **Validations embarquées** : 10+ règles métier  
✅ **Génération référence** : Automatique post-save  
✅ **Update fidélité** : +0.2 par emprunt  
✅ **Décrémentation quantité** : Automatique  
✅ **Dialog confirmation** : Double-validation  
✅ **Messages détaillés** : Success/Error/Info  
✅ **Code-behind minimal** : Logique dans ViewModel  

***