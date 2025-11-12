## README : Module Navigation Bar (ResponsiveNavBar)

### Barre de Navigation Responsive et Adaptive

Le module **ResponsiveNavBar** est une barre de navigation latérale moderne, intelligente et hautement interactive qui s'adapte automatiquement à la hauteur de la fenêtre. Elle offre une expérience utilisateur fluide avec des animations élégantes et un système d'overflow intelligent.

***

### Structure du Module

```
/Navbar
├── Views/
│   ├── ResponsiveNavBar.xaml           # Interface XAML de la navbar
│   └── MainNavBar.xaml.cs              # Logique code-behind
├── Models/
│   └── NavItem.cs                      # Classe de représentation d'un item
└── Common/
    └── IconButton.xaml                  # Composant d'icône réutilisable
```

***

### Vue (XAML) - ResponsiveNavBar

**Sections principales** :

1. **Avatar Section** (Row 0)
   - Bouton avatar circulaire de 45x45 px
   - Bordure blanche de 2px
   - Cliquable → redirige vers AccountManager
   - Icône Material Design "Account"

2. **Separator** (Row 1)
   - Ligne horizontale de séparation (2px)
   - Couleur DarkBlue

3. **Navigation Items Container** (Row 2, scrollable)
   - Liste dynamique de boutons générés par binding
   - `ItemsControl` bindé à `VisibleNavItems`
   - Bouton "Menu" (overflow) visible si `HasHiddenItems == true`
   - ScrollViewer avec style moderne

4. **Bottom Section** (Row 3)
   - Bouton "Mon Profil" (AccountManager)
   - Separator
   - Bouton "Déconnexion"

5. **Overflow Menu Popup**
   - Popup ancré sur `MenuButton`
   - Placement : Right, Offset horizontal +10px
   - Fond PrimaryBlue + bordure DarkBlue + ombre portée
   - ScrollViewer limité à 400px de hauteur max
   - Bindé à `HiddenNavItems`

**États visuels** :
- **Collapsed** (défaut, 80px largeur) : Icônes seules, labels masqués
- **Expanded** (hover/interaction, 220px) : Icônes + labels visibles
- **Transition** : Animation fluide de 300ms avec CubicEase

***

### Modèles (NavItem)

```csharp
public class NavItem
{
    public string Id { get; set; }           // Identifiant unique
    public string Label { get; set; }        // Texte affiché
    public PackIconMaterialKind IconKind { get; set; }  // Icône Material Design
}
```

**Items de navigation par défaut** :

| Id            | Label                        | Icône                    |
|---------------|------------------------------|--------------------------|
| Home          | Accueil                      | Home                     |
| Catalog       | Catalogue de livres          | Bookshelf                |
| Members       | Liste des Abonnés            | AccountGroup             |
| BorrowManager | Gestionnaire des emprunts    | BookCog                  |
| Borrow        | Faire un Emprunt             | BookArrowRight           |
| Return        | Retourner un Livre           | BookArrowLeftOutline     |
| Account       | Mon Profil                   | AccountCog               |
| Logout        | Déconnexion                  | LogoutVariant            |

***

### Fonctionnement du Module

#### **1. Initialisation**

```csharp
private void InitializeNavItems()
{
    AllNavItems = new ObservableCollection<NavItem>
    {
        new NavItem { Id = "Home", Label = "Accueil", IconKind = PackIconMaterialKind.Home },
        new NavItem { Id = "Catalog", Label = "Catalogue de livres", IconKind = PackIconMaterialKind.Bookshelf },
        // ... autres items
    };
    UpdateVisibleItems(); // Calcul initial
}
```

#### **2. Système d'Overflow Intelligent**

Le module calcule dynamiquement combien d'items peuvent être affichés selon la hauteur disponible :

```csharp
private void OnSizeChanged(object sender, SizeChangedEventArgs e)
{
    if (!_isInitialized) return;
    UpdateVisibleItems();
}

private void UpdateVisibleItems()
{
    double availableHeight = MainGrid.ActualHeight 
        - AVATAR_SECTION_HEIGHT 
        - BOTTOM_SECTION_HEIGHT 
        - (2 * SEPARATOR_HEIGHT);
    
    int maxVisibleButtons = (int)(availableHeight / BUTTON_HEIGHT);
    int itemsToShow = Math.Min(maxVisibleButtons, AllNavItems.Count);
    
    VisibleNavItems.Clear();
    HiddenNavItems.Clear();
    
    for (int i = 0; i < AllNavItems.Count; i++)
    {
        if (i < itemsToShow)
            VisibleNavItems.Add(AllNavItems[i]);
        else
            HiddenNavItems.Add(AllNavItems[i]);
    }
    
    HasHiddenItems = HiddenNavItems.Count > 0;
}
```

**Constantes utilisées** :
- `BUTTON_HEIGHT = 56` px
- `AVATAR_SECTION_HEIGHT = 90` px
- `BOTTOM_SECTION_HEIGHT = 140` px
- `SEPARATOR_HEIGHT = 25` px

#### **3. Animation Expand/Collapse**

**Au survol de la navbar** :
```csharp
private void NavBar_MouseEnter(object sender, MouseEventArgs e)
{
    if (!_isExpanded)
    {
        _isExpanded = true;
        AnimateNavBar(220); // Expansion à 220px
        AnimateLabels(true); // Affichage des labels
    }
}

private void NavBar_MouseLeave(object sender, MouseEventArgs e)
{
    if (_isExpanded)
    {
        _isExpanded = false;
        AnimateNavBar(80); // Collapse à 80px
        AnimateLabels(false); // Masquage des labels
    }
}
```

**Animation de la largeur** :
```csharp
private void AnimateNavBar(double targetWidth)
{
    DoubleAnimation animation = new DoubleAnimation
    {
        To = targetWidth,
        Duration = TimeSpan.FromSeconds(ANIMATION_DURATION),
        EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
    };
    MainGrid.BeginAnimation(WidthProperty, animation);
}
```

**Animation des labels** :
```csharp
private void AnimateLabels(bool show)
{
    foreach (var button in GetAllButtons())
    {
        TextBlock label = FindVisualChild<TextBlock>(button, "NavText");
        TranslateTransform transform = label?.RenderTransform as TranslateTransform;
        
        if (show)
        {
            label.Visibility = Visibility.Visible;
            // Animation Opacity 0 → 1
            // Animation TranslateX -15 → 0
        }
        else
        {
            // Animation inverse
            label.Visibility = Visibility.Collapsed;
        }
    }
}
```

#### **4. Gestion de la Navigation**

**Clic sur un bouton de navigation** :
```csharp
private void NavButton_Click(object sender, RoutedEventArgs e)
{
    Button clickedButton = sender as Button;
    NavItem item = clickedButton?.Tag as NavItem;
    
    if (item == null) return;
    
    // Mise à jour du bouton actif
    UpdateActiveButton(clickedButton);
    
    // Déclenchement de l'événement de navigation
    NavigationRequested?.Invoke(this, item.Id);
}

private void UpdateActiveButton(Button newActiveButton)
{
    // Retirer le style actif du bouton précédent
    if (_currentActiveButton != null)
        _currentActiveButton.Style = FindResource("NavButtonStyle") as Style;
    
    // Appliquer le style actif au nouveau bouton
    newActiveButton.Style = FindResource("NavButtonActiveStyle") as Style;
    _currentActiveButton = newActiveButton;
}
```

**Gestion des items du menu overflow** :
```csharp
private void OverflowMenuItem_Click(object sender, RoutedEventArgs e)
{
    Button clickedItem = sender as Button;
    NavItem item = clickedItem?.Tag as NavItem;
    
    OverflowMenuPopup.IsOpen = false; // Fermer le popup
    NavigationRequested?.Invoke(this, item?.Id);
}

private void MenuButton_Click(object sender, RoutedEventArgs e)
{
    OverflowMenuPopup.IsOpen = !OverflowMenuPopup.IsOpen;
}
```

***

### Styles XAML Personnalisés

#### **NavButtonStyle** (Style de base)

- Background : Transparent par défaut
- Border circulaire de 40x40 px pour l'icône
- Label avec Opacity 0 et translateX -15 (caché par défaut)
- **Hover** : Background → #2055A5, Scale 1.05
- **Pressed** : Scale 0.95

#### **NavButtonActiveStyle** (Bouton actif)

- Background : #F1F1F1 (blanc cassé)
- Border icône : PrimaryBlue
- Icône couleur : DarkBlue
- Label couleur : DarkBlue

#### **OverflowMenuItemStyle**

- Background transparent
- Hover : Background → DarkBlue
- Layout horizontal : Icône 20px + Label

***

### Propriétés de Dépendance

```csharp
// Collection de tous les items
public ObservableCollection<NavItem> AllNavItems { get; set; }

// Items visibles dans la navbar
public ObservableCollection<NavItem> VisibleNavItems
{
    get => (ObservableCollection<NavItem>)GetValue(VisibleNavItemsProperty);
    set => SetValue(VisibleNavItemsProperty, value);
}

// Items cachés (overflow)
public ObservableCollection<NavItem> HiddenNavItems
{
    get => (ObservableCollection<NavItem>)GetValue(HiddenNavItemsProperty);
    set => SetValue(HiddenNavItemsProperty, value);
}

// Indicateur de présence d'items cachés
public bool HasHiddenItems
{
    get => (bool)GetValue(HasHiddenItemsProperty);
    set => SetValue(HasHiddenItemsProperty, value);
}

// Items Account et Logout (section bottom)
public NavItem AccountManagerItem { get; set; }
public NavItem LogoutItem { get; set; }
```

***

### Technologies et Outils

| Technologie               | Utilisation                                          |
|---------------------------|------------------------------------------------------|
| **WPF (XAML)**            | Interface utilisateur déclarative                    |
| **MahApps.Metro.IconPack**| Bibliothèque d'icônes Material Design                |
| **Storyboard Animations** | Animations fluides (Width, Opacity, TranslateX)      |
| **ObservableCollection**  | Binding bidirectionnel pour navigation dynamique     |
| **DependencyProperty**    | Propriétés bindables XAML                            |
| **VisualTreeHelper**      | Parcours de l'arbre visuel pour animations           |
| **Event Delegation**      | Événement `NavigationRequested` pour découplage      |

***

### Fonctionnalités Clés

✅ **Adaptation automatique** : Calcul responsive selon la hauteur de fenêtre  
✅ **Overflow intelligent** : Menu contextuel pour items cachés  
✅ **Animations fluides** : Expansion/collapse avec easing  
✅ **État actif visuel** : Distinction claire du module courant  
✅ **Hover effects** : Scale + changement de couleur  
✅ **Composants réutilisables** : IconButton modulaire  
✅ **Découplage** : Événement de navigation sans dépendances directes  

***

### Améliorations Futures

- **Persistance de l'état** : Sauvegarder le dernier item actif
- **Raccourcis clavier** : Navigation rapide (Ctrl+1, Ctrl+2, etc.)
- **Badges de notification** : Compteurs sur items (ex: emprunts en retard)
- **Thème personnalisable** : Mode clair/sombre
- **Groupes de navigation** : Sections pliables/dépliables

***
