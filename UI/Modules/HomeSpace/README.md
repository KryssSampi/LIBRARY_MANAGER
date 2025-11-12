
## README : Module HomeSpace

### Espace Utilisateur - HomeSpace

Le module **HomeSpace** est le tableau de bord principal après authentification. Il affiche un aperçu personnalisé des activités de l'utilisateur (emprunts, événements, notifications) et propose une navigation rapide vers les principales fonctionnalités de l'application.

***

### Structure du Module

```
/HomeSpace
├── Views/
│   ├── HomeSpace.xaml             # Interface du tableau de bord
│   ├── HomeSpace.xaml.cs          # Code-behind
│   ├── Components/
│       ├── Header.xaml            # Composant d'en-tête (profil utilisateur)
│       ├── Header.xaml.cs
│       ├── IconButton.xaml        # Bouton stylisé réutilisable
│       └── IconButton.xaml.cs
├── ViewModels/
│   └── HomeViewModel.cs           # ViewModel pour le binding de données
├── Models/
│   ├── User.cs, StaffMember.cs, Subscriber.cs
│   ├── Loan.cs                    # Gestion des prêts
│   └── Book.cs                    # Informations sur les livres
└── Data/
    └── LibraryDbContext.cs
```

***

### Vues (XAML)

#### `HomeSpace.xaml`
Tableau de bord avec sections :
- **Header personnalisé** : Affiche nom de l'utilisateur, photo, et bouton déconnexion
- **Cartes d'action rapide** :
  - Emprunter un Livre
  - Catalogue des Livres
  - Gestion des Abonnées (Staff uniquement)
  - Gestionnaire des Emprunts (Staff uniquement)
  - Calendrier des Événements
  - Effectuer un Retour
- **Message d'accueil** : "Bienvenue [Nom] !" avec citation inspirante (Einstein)
- **Section "Opérations du jour"** : Liste des prêts/retours/activités récentes

**Aperçu visuel** :

![HomeSpace](/Assets/HomeSpace_img.png)

**1. `Header.xaml`**
En-tête responsive avec :
- Avatar utilisateur
- Nom complet
- Bouton de déconnexion avec icône
- Navigation hamburger (menu latéral)

**2. `IconButton.xaml`**
Bouton stylisé avec :
- Icône personnalisable
- Texte de label
- Gestion d'événements Click
- Propriétés de dépendance pour réutilisabilité

***

### Modèles (Models)

#### 1. **Loan.cs** (Prêt)
Représente un emprunt de livre.

**Propriétés principales** :
- `LoanId` (PK)
- `BookId`, `SubscriberId` (FK)
- `BorrowDate`, `ReturnDate`
- `IsActive` (prêt en cours ou retourné)
- `Penalty` (pénalité de retard, nullable)
- `Ref_Loan` (référence unique générée)
- **Navigation properties** : `Book`, `Subscriber`, `Modifications`

**Propriétés calculées** :
- `IsLate` : Détermine si le prêt est en retard
- `DaysLate` : Nombre de jours de retard

***

#### 2. **Book.cs** (Livre)
**Propriétés principales** :
- `BookId`, `Title`, `Author`, `ISBN`
- `Category`, `Description`, `PublishDate`
- `Quantity`, `IsAvailable`
- `CoverUrl` (URL de la couverture)

***

### ViewModel

#### **HomeViewModel.cs**
Fournit les données pour le binding de la vue.

**Propriétés exposées** :
- `CurrentUser` : Utilisateur connecté (StaffMember ou Subscriber)
- `RecentLoans` : Liste des prêts récents à afficher
- `TodayOperations` : Opérations du jour (pour le staff)

**Commandes (ICommand)** :
- `NavigateToCatalogCommand`
- `NavigateToLoansCommand`
- `LogoutCommand`

***

### Base de données (DbContext)

Utilise **LibraryDbContext** avec accès aux tables :
- `Loans` : Récupération des prêts récents/actifs
- `Books` : Informations des livres empruntés
- `Users` : Données de l'utilisateur connecté

**Exemple de requête (récupération des prêts actifs)** :
```csharp
var activeLoans = context.Loans
    .Include(l => l.Book)
    .Include(l => l.Subscriber)
    .Where(l => l.IsActive)
    .OrderByDescending(l => l.BorrowDate)
    .Take(5)
    .ToList();
```

***

### Fonctionnement du Module

1. **Chargement de la vue** :
   - Récupération de `App._currentconnected_User`
   - Affichage du nom et des informations utilisateur dans le Header
2. **Affichage contextuel** :
   - Si **Staff** : affiche toutes les cartes d'action (gestion abonnés, emprunts)
   - Si **Subscriber** : affiche uniquement catalogue, emprunter, événements
3. **Navigation** :
   - Clic sur une carte → Ouvre la vue/module correspondant
   - Exemple : "Catalogue des Livres" → `BookCatalogView`
4. **Déconnexion** :
   - Clic sur bouton logout → Réinitialise `App._currentconnected_User` → Retour à `MainWindow`

***

### Outils et Technologies

| Outil / Technologie           | Utilisation                                          |
|-------------------------------|------------------------------------------------------|
| **WPF (XAML)**                | Interface utilisateur déclarative et responsive      |
| **MVVM Pattern**              | Séparation Vue / ViewModel / Modèle                  |
| **Entity Framework Core**     | Accès base de données et requêtes LINQ               |
| **CommunityToolkit.Mvvm**     | ObservableObject, RelayCommand pour MVVM             |
| **Material Design Icons**     | Icônes modernes pour boutons                         |
| **Data Binding**              | Synchronisation automatique UI ↔ ViewModel           |

***

### Fonctionnalités Clés

✅ Tableau de bord personnalisé selon le rôle (Staff / Subscriber)  
✅ Accès rapide aux principales fonctionnalités  
✅ Affichage des opérations récentes et du jour  
✅ Composants réutilisables (Header, IconButton)  
✅ Navigation fluide vers les modules spécialisés  
✅ Déconnexion sécurisée

***

### Améliorations Futures

- Widget de statistiques en temps réel (prêts du mois, livres populaires)
- Notifications push pour retards/événements
- Personnalisation du thème (mode sombre/clair)
- Raccourcis clavier pour navigation rapide
- Intégration d'un calendrier interactif

***
