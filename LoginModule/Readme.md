
## README  : Module Login

###  Module d'Authentification - Login

Le module **Login** gère l'authentification des utilisateurs (Staff et Subscribers) dans l'application NOCTUA. Il vérifie les identifiants, initialise la session utilisateur et redirige vers l'espace utilisateur approprié (HomeSpace).

***

### Structure du Module

```
/Login
├── Views/
│   ├── MainWindow.xaml           # Fenêtre de connexion principale
│   └── MainWindow.xaml.cs         # Code-behind de la fenêtre de connexion
├── ViewModels/
│   └── (logique dans le code-behind pour ce module)
├── Models/
│   ├── User.cs                    # Classe abstraite de base
│   ├── StaffMember.cs             # Utilisateur de type personnel
│   └── Subscriber.cs              # Utilisateur de type abonné
└── Data/
    └── LibraryDbContext.cs        # Contexte Entity Framework Core
```

***

### Vues (XAML)

#### `MainWindow.xaml`
Fenêtre principale d'authentification avec :
- **Champs de saisie** : Nom d'utilisateur et Mot de passe
- **Bouton de connexion** : Déclenche la validation des identifiants
- **Liens utiles** : Inscription et Mot de passe oublié (fonctionnalités futures)
- **Design inspiré** : Thème bibliothèque/livresque avec une image de fond

**Aperçu visuel** :

[Login Screen](/Assets/Login_img.png)

#### 1. **User.cs** (Classe abstraite)
Représente un utilisateur générique.

**Propriétés principales** :
- `Id_User` (PK, auto-incrémenté)
- `Name_User` (nom complet)
- `Adresse_Mail` (email, unique)
- `Password_Hash` (mot de passe chiffré SHA-256)
- `Num_Telephone`, `Adresse`, `Date_Creation`

**Méthodes** :
- `HashPassword(string plainPassword)` : Hache le mot de passe
- `VerifyPassword(string plainPassword)` : Vérifie le mot de passe

***

#### 2. **StaffMember.cs** (Personnel)
Hérite de `User`. Représente un membre du personnel de la bibliothèque.

**Propriétés supplémentaires** :
- `Poste` (ex : Bibliothécaire, Directeur)
- `YearHired` (année d'embauche)
- `Ref_Staff` (référence unique auto-générée)
- `Modifications` (collection de modifications effectuées)

**Méthodes métier** :
- `ReturnLoan(Loan loan)` : Enregistre le retour d'un prêt
- `ChangeReturnDate(Loan loan, DateTime newDate)` : Modifie la date de retour

***

#### 3. **Subscriber.cs** (Abonné)
Hérite de `User`. Représente un abonné de la bibliothèque.

**Propriétés supplémentaires** :
- `Fidelity` (points de fidélité, décimal)
- `Ref_Subscriber` (référence unique auto-générée)
- `Loans` (collection de prêts associés)

**Méthodes métier** :
- `BorrowBook(Book book)` : Crée un nouveau prêt
- `IncreaseFidelity(decimal points)` / `DecreaseFidelity(decimal points)`

***

### Base de données (DbContext)

#### **LibraryDbContext.cs**
Contexte Entity Framework Core pour accéder à la base de données SQLite.

**DbSets** :
- `Users` : Tous les utilisateurs (polymorphisme TPH - Table Per Hierarchy)
- `StaffMembers` : Membres du personnel
- `Subscribers` : Abonnés
- `Books`, `Loans`, `Modifications` (autres entités)

**Configuration** :
- Héritage **TPH** : User, StaffMember, Subscriber partagent la table `Users`
- Index unique sur `Adresse_Mail`, `Ref_Staff`, `Ref_Subscriber`
- Valeur par défaut `Date_Creation = CURRENT_TIMESTAMP`

***

### Fonctionnement du Module

1. **Saisie des identifiants** : L'utilisateur entre son nom d'utilisateur (email) et son mot de passe.
2. **Validation** :
   - Recherche de l'utilisateur dans `DbContext.Users` via `Adresse_Mail`
   - Vérification du mot de passe avec `VerifyPassword()`
3. **Initialisation de session** :
   - Stockage de l'utilisateur connecté dans `App._currentconnected_User`
   - Distinction entre `StaffMember` et `Subscriber` pour adapter les droits
4. **Redirection** :
   - Fermeture de `MainWindow`
   - Ouverture de `HomeSpace` (tableau de bord utilisateur)

**Code clé (extrait de `MainWindow.xaml.cs`)** :
```csharp
private void LoginButton_Click(object sender, RoutedEventArgs e)
{
    string email = UsernameTextBox.Text.Trim();
    string password = PasswordBox.Password;

    using (var context = new LibraryDbContext())
    {
        var user = context.Users.FirstOrDefault(u => u.Adresse_Mail == email);
        
        if (user != null && user.VerifyPassword(password))
        {
            App._currentconnected_User = user;
            var homeSpace = new HomeSpace();
            homeSpace.Show();
            this.Close();
        }
        else
        {
            MessageBox.Show("Identifiants invalides !");
        }
    }
}
```

***

### Outils et Technologies

| Outil / Technologie           | Utilisation                                          |
|-------------------------------|------------------------------------------------------|
| **WPF (Windows Presentation Foundation)** | Framework UI pour fenêtres et binding XAML |
| **Entity Framework Core**     | ORM pour accès base de données SQLite                |
| **SQLite**                    | Base de données relationnelle légère                 |
| **SHA-256**                   | Algorithme de hachage de mot de passe                |
| **MVVM (partiel)**            | Séparation logique / présentation (code-behind ici)  |
| **.NET 6+**                   | Plateforme de développement                          |

***

### Fonctionnalités Clés

Authentification sécurisée par email + mot de passe hashé  
Différenciation Staff / Subscriber  
Persistance de session utilisateur  
Interface élégante et thématique  
Gestion d'erreurs utilisateur (identifiants invalides)

***

### Améliorations Futures

- Implémentation du "Mot de passe oublié" (reset via email)
- Inscription en ligne pour nouveaux abonnés
- Double authentification (2FA)
- Gestion de sessions expirables (timeout automatique)

***
