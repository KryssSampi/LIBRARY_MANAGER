Parfait ! Voici les **deux README complets** pour les modules **Emprunt (Borrow)** et **Retour (Return)** – les deux cœurs opérationnels de l'application NOCTUA.

---

## README  : Module Emprunt (Borrow)

### Module d'Enregistrement d'Emprunts - BorrowSpace

Le module **Emprunt** permet l'enregistrement rapide et sécurisé des nouveaux prêts de livres. Sans recherche complexe, il utilise l'**ISBN du livre** et l'**ID de l'abonné** pour créer instantanément un nouvel emprunt avec vérifications métier complètes.

**Cas d'usage** : Un bibliothécaire scanne le code-barres du livre et rentre l'ID de l'abonné → l'application enregistre l'emprunt et met à jour les stocks.

---

### Structure du Module

```
/BorrowModule
├── Views/
│   ├── BorrowSpace.xaml            # Interface du formulaire d'emprunt
│   └── BorrowSpace.xaml.cs         # Code-behind
├── ViewModels/
│   └── BorrowSpaceVm.cs            # Logique métier et binding MVVM
├── Models/
│   ├── Loan.cs                     # Entité emprunt
│   ├── Book.cs                     # Référence livre
│   ├── Subscriber.cs               # Référence abonné
│   └── Modification.cs             # Historique des modifications
└── Data/
    └── LibraryDbContext.cs         # Contexte Entity Framework
```

***

### Vue (XAML) - BorrowSpace

**Interface simple et rapide** :
- **ISBN du Livre** : TextBox pour saisir/scanner le code ISBN
- **ID de l'abonné** : TextBox pour rentrer l'ID numérique
- **Date de retour prévue** : DatePicker (par défaut 14 jours + bonus fidélité)
- **Bouton "Confirmer"** : Valide et enregistre l'emprunt
- **Bouton "Accueil"** : Retour au HomeSpace
- **Bouton "Liste des emprunts"** : Affiche les emprunts actifs
- **Affichage des erreurs** : Message d'erreur en temps réel

**Aperçu visuel** :
Le formulaire affiche « **Enregistrer Les Emprunts Ici !** » avec deux champs de saisie et une illustration bibliothèque.

***

### Modèles principaux

#### 1. **Loan.cs** (Emprunt)
**Propriétés** :
- `LoanId` : Identifiant unique (PK, auto-incrémenté)
- `BookId`, `Book` : Lien vers le livre emprunté
- `SubscriberId`, `Subscriber` : Lien vers l'abonné
- `BorrowDate` : Date/heure d'emprunt (par défaut : now)
- `ReturnDate` : Date de retour prévue (calculée + bonus fidélité)
- `ActualReturnDate` : Date effective du retour (null si pas retourné)
- `IsActive` : Booléen (true = en cours, false = retourné)
- `Penalty` : Pénalité appliquée (null si aucune)
- `Ref_Loan` : Référence unique générée (ex: `LOAN-20251112-00000001`)
- **Navigation** : `Modifications` (historique des modifications du prêt)

**Propriétés calculées** :
- `IsLate` : \`true\` si `IsActive == true` ET `DateTime.Now > ReturnDate`
- `DaysLate` : Nombre de jours de retard

**Constructeurs** :
```csharp
public Loan(Subscriber subscriber, Book book, int durationDays = 14)
```
- Valide que subscriber et book existent en base
- Définit les FK ET les navigation properties
- Calcule `ReturnDate = BorrowDate + durationDays + (Subscriber.Fidelity * 2)`

***

#### 2. **Book.cs** (Livre)
**Propriétés pertinentes** :
- `BookId`, `ISBN`, `Title`, `Author`
- `Quantity` : Nombre de copies disponibles
- `IsAvailable` : Booléen (true si Quantity > 0)

***

#### 3. **Subscriber.cs** (Abonné)
**Propriétés pertinentes** :
- `Id_User`, `Name_User`, `Adresse_Mail`
- `Fidelity` : Points de fidélité (décimal)
- `Loans` : Collection de prêts associés

**Méthodes** :
- `ValidateCanBorrow(out string errorMsg)` : Vérifie les conditions d'emprunt
- `IncreaseFidelity()` : Ajoute des points (appel à chaque emprunt)
- `DecreaseFidelity(decimal points)` : Réduit les points en cas de retard

***

### ViewModel - BorrowSpaceVm

**Propriétés Observable** (binding XAML) :
- `IsbnLivre` : ISBN saisi par l'utilisateur
- `IdMembre` : ID de l'abonné saisi
- `ErrorMessage` : Message d'erreur affiché en temps réel
- `SelectedReturnDate` : Date de retour choisie (par défaut : today + 14j)
- `ConfirmationCommand` : RelayCommand pour confirmer l'emprunt

**Logique métier dans `Confirmer()` (synchrone)** :

1. **Validation des champs** :
   - Vérifie que ISBN et IdMembre ne sont pas vides
   - Vérifie que IdMembre est bien un nombre
   - Vérifie que SelectedReturnDate est dans le futur

2. **Requêtes base de données** (LibraryDbContext) :
   ```csharp
   Book livre = context.Books.FirstOrDefault(l => l.ISBN == IsbnLivre);
   Subscriber membre = context.Subscribers.FirstOrDefault(m => m.Id_User == membreId);
   ```

3. **Vérifications métier** :
   - Livre existe et disponible (`Quantity > 0`)
   - Abonné existe et peut emprunter (`ValidateCanBorrow()`)
   - Pas d'emprunt actif du même livre par le même abonné

4. **Création et sauvegarde de l'emprunt** :
   ```csharp
   var emprunt = new Loan(membre, livre) { ReturnDate = SelectedReturnDate.Value };
   context.Loans.Add(emprunt);
   livre.Quantity--;
   membre.IncreaseFidelity();
   context.SaveChanges();
   context.GenerateReferences(); // Génère Ref_Loan
   ```

5. **Feedback utilisateur** :
   - Message de succès avec détails (livre, membre, référence, dates)
   - Réinitialisation des champs
   - Ou message d'erreur détaillé en cas de problème

***

### Base de données

**DbContext** : `LibraryDbContext`

**Tables utilisées** :
- `Books` : Lecture + mise à jour (Quantity, IsAvailable)
- `Subscribers` : Lecture + mise à jour (Fidelity)
- `Loans` : Insertion + lecture

**Transaction** : L'ajout du livre, la mise à jour du stock, et l'incrémentation de fidélité se font en une seule transaction (atomique).

***

### Flux opérationnel

1. **Ouverture du module** : Formulaire vierge, focus sur le champ ISBN
2. **Saisie ISBN** : L'utilisateur scanne ou tape le code ISBN
3. **Saisie ID abonné** : L'utilisateur entre l'ID du membre
4. **Sélection date retour** : (Optionnel) Modifie la date par défaut si besoin
5. **Clic "Confirmer"** :
   - Validation complète
   - Vérification du livre et du membre
   - Création et sauvegarde de l'emprunt
   - Mise à jour du stock et fidélité
   - Message de succès
   - Réinitialisation du formulaire

6. **Navigation** :
   - "Accueil" : Retour au HomeSpace
   - "Liste des emprunts" : Ouvre un dialogue montrant les emprunts actifs du jour

***

### Outils et Technologies

| Outil                         | Utilisation                                          |
|-------------------------------|------------------------------------------------------|
| **WPF (XAML)**                | Interface utilisateur                                |
| **MVVM (MVVM Toolkit)**       | Pattern d'architecture, `ObservableProperty`        |
| **Entity Framework Core**     | Accès base de données, requêtes LINQ                |
| **LibraryDbContext**          | Gestion des transactions, sauvegarde atomique       |
| **RelayCommand**              | Commandes liées aux boutons                         |
| **DateTime**                  | Calcul des dates et bonus fidélité                  |

***

### Fonctionnalités clés

✅ **Enregistrement rapide** : ISBN + ID abonné suffisent  
✅ **Vérifications complètes** : Existance livre/abonné, disponibilité, droits d'emprunt  
✅ **Calcul automatique de date** : Bonus fidélité intégré  
✅ **Mise à jour atomique** : Stock, fidélité, référence générée  
✅ **Gestion d'erreurs** : Messages clairs et guidés  
✅ **Historique** : Chaque emprunt est tracé avec référence unique  

***

### Améliorations futures

- Support du **code-barres** (scanneur USB)
- **Notifications** si livre déjà emprunté par cet abonné
- **Récupération automatique des données** du livre/abonné après scan/ID
- **Impression de reçu** d'emprunt
- **Restriction de date** (plage fixe, ex: max 30 jours)

***