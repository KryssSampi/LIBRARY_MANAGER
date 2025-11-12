

## README : Module Retour (Return)

### Module d'Enregistrement de Retours - ReturnSpace

Le module **Retour** gère l'enregistrement des livres retournés par les abonnés. Comme le module emprunt, il fonctionne **sans recherche**, utilisant l'**ISBN** et l'**ID de l'abonné** pour identifier et clôturer automatiquement un prêt actif.

**Cas d'usage** : Un abonné revient avec un livre → le bibliothécaire scanne/rentre ISBN + ID → l'application clôture l'emprunt, applique éventuellement une pénalité de retard, et met à jour la fidélité.

***

### Structure du Module

```
/ReturnModule
├── Views/
│   ├── ReturnSpace.xaml            # Interface du formulaire de retour
│   └── ReturnSpace.xaml.cs         # Code-behind
├── ViewModels/
│   └── ReturnSpaceVm.cs            # Logique métier
├── Models/
│   ├── Loan.cs                     # Entité emprunt
│   ├── Book.cs                     # Référence livre
│   ├── Subscriber.cs               # Référence abonné
│   └── Modification.cs             # Historique des modifications
└── Data/
    └── LibraryDbContext.cs         # Contexte Entity Framework
```

***

### Vue (XAML) - ReturnSpace

**Interface simple** :
- **ISBN du Livre** : TextBox pour saisir/scanner le ISBN
- **ID de l'abonné** : TextBox pour l'ID du membre qui retourne
- **Bouton "Confirmer"** : Valide et clôt l'emprunt
- **Bouton "Accueil"** : Retour au HomeSpace
- **Bouton "Liste des emprunts"** : Affiche les emprunts actifs pour vérification
- **Affichage des erreurs** : Message d'erreur instantané

**Aperçu visuel** :
Le formulaire affiche « **Effectuer Les Retours Ici !** » avec illustration et champs de saisie.

---

### Modèles principaux

#### 1. **Loan.cs** (Emprunt)
**Propriétés pertinentes au retour** :
- `LoanId` : Identifiant
- `Book`, `Subscriber` : Références de navigation
- `BorrowDate`, `ReturnDate` : Dates prévues
- `ActualReturnDate` : Date effective du retour (définie au retour)
- `IsActive` : Booléen (becomes false après retour)
- `Penalty` : Pénalité appliquée (calculée au retour si retard)
- `Ref_Loan` : Référence unique
- **Propriétés calculées** :
  - `IsLate` : `true` si retard
  - `DaysLate` : Nombre de jours de retard

***

#### 2. **Modification.cs** (Historique)
Entité qui trace **toute modification** d'emprunt.

**Propriétés** :
- `ModificationId` : PK
- `Type` : Enum (`ChangeReturnDate`, `ReturnLoan`)
- `LoanId`, `Loan_` : Lien vers l'emprunt
- `StaffMemberId`, `StaffMember_` : Qui a effectué la modification
- `ModificationDate` : Quand
- `Description` : Description générée automatiquement
- `OldReturnDate`, `NewReturnDate` : Pour les changements de date
- `Ref_Modification` : Référence générée (ex: `MOD-20251112-00000001`)

**Méthodes** :
```csharp
public Modification(ModificationType type, Loan loan, StaffMember staff, DateTime? newReturn = null)
public void GenerateReference()
private string GenerateDescription() // Génère description selon le type
```

***

### ViewModel - ReturnSpaceVm

**Propriétés Observable** :
- `IsbnLivre` : ISBN du livre retourné
- `IdMembre` : ID de l'abonné
- `ErrorMessage` : Message d'erreur
- `ConfirmationCommand` : Commande de retour

**Logique métier dans `Confirmer()` (synchrone)** :

1. **Validation des champs** :
   - ISBN et IdMembre non vides
   - IdMembre est un nombre

2. **Recherche de l'emprunt actif** :
   ```csharp
   Loan emprunt = context.Loans
       .Include(e => e.Book)
       .Include(e => e.Subscriber)
       .FirstOrDefault(e => e.Book.ISBN == IsbnLivre &&
                            e.SubscriberId == membreId &&
                            e.IsActive == true);
   ```

3. **Calcul du retard et pénalité** :
   ```csharp
   var joursRetard = (DateTime.Now.Date - emprunt.ReturnDate.Date).Days;
   decimal penalite = joursRetard > 0 ? joursRetard * 1.0m : 0;
   ```

4. **Gestion de la fidélité** :
   - **Si retard** : `membre.DecreaseFidelity(joursRetard * 0.1m)` + pénalité
   - **Si à temps** : `membre.IncreaseFidelity(0.2m)` bonus

5. **Mise à jour de l'emprunt** :
   ```csharp
   emprunt.IsActive = false;
   emprunt.ActualReturnDate = DateTime.Now;
   emprunt.Penalty = penalite;
   livre.Quantity++;
   livre.IsAvailable = true;
   context.SaveChanges();
   ```

6. **Feedback détaillé** :
   - MessageBox affichant : livre, abonné, référence, dates, retard, pénalité, fidélité
   - Distinction visuelle : Retard (icône avertissement 🟠) vs À temps (icône info 🔵)
   - Notification spéciale si retard > 30 jours

---

### Base de données

**DbContext** : `LibraryDbContext`

**Tables utilisées** :
- `Loans` : Recherche de l'emprunt + mise à jour (`IsActive`, `ActualReturnDate`, `Penalty`)
- `Books` : Mise à jour (Quantity, IsAvailable)
- `Subscribers` : Mise à jour (Fidelity)
- `Modifications` : Enregistrement optionnel de l'historique

**Vérifications** :
- Livre existe (`FirstOrDefault` renvoie null si introuvable)
- Abonné existe
- Un seul emprunt actif du même livre par ce membre à la fois

***

### Flux opérationnel

1. **Ouverture du module** : Formulaire vierge
2. **Retour du livre par l'abonné** : Présentation du livre et ID
3. **Saisie ISBN + ID** : Scan ou frappe manuelle
4. **Clic "Confirmer"** :
   - Recherche de l'emprunt actif
   - Calcul du retard et pénalité
   - Mise à jour fidélité (augmentation ou diminution)
   - Réinitialisation du stock
   - Clôture de l'emprunt (`IsActive = false`)
   - MessageBox complète (retard, pénalité, fidélité mise à jour)
   - Réinitialisation du formulaire

5. **Navigation** :
   - "Accueil" : Retour au HomeSpace
   - "Vérifier emprunts actifs" : Affiche les emprunts non retournés (utile pour audit)

***

### Exemple de flux complet

**Scénario** :
- Livre emprunté le 2025-11-01, retour prévu 2025-11-15
- Retour effectué le 2025-11-18 → 3 jours de retard
- Pénalité : 3 × 1€ = 3€
- Fidélité : -0.3 point

**MessageBox affiché** :
```
✅ Retour enregistré avec succès!

📚 Livre: [Titre du livre]
👤 Membre: [Nom de l'abonné]
🔖 Référence: LOAN-20251102-00000042
📅 Date d'emprunt: 02/11/2025
📅 Date prévue: 15/11/2025
📅 Date effective: 18/11/2025
📊 Quantité disponible: 3
⭐ Fidélité: 4.70/10

⚠️ Retard: 3 jour(s)
💰 Pénalité: $3.00
```

***

### Outils et Technologies

| Outil                         | Utilisation                                          |
|-------------------------------|------------------------------------------------------|
| **WPF (XAML)**                | Interface utilisateur                                |
| **MVVM Toolkit**              | ObservableProperty, RelayCommand                     |
| **Entity Framework Core**     | Requêtes LINQ, relation Include()                    |
| **LibraryDbContext**          | Transactions et SaveChanges atomique                 |
| **DateTime**                  | Calcul de retard/jours                               |
| **Decimal**                   | Calcul précis des pénalités et fidélité              |

***

### Fonctionnalités clés

✅ **Retour rapide** : ISBN + ID suffisent  
✅ **Calcul automatique** : Retard, pénalité, fidélité impactée  
✅ **Distinction visuelle** : À temps (vert) vs En retard (orange)  
✅ **Atomicité** : Stock, fidélité, emprunt mis à jour en une seule transaction  
✅ **Feedback complet** : Détails d'emprunt et état final d'une seule requête  
✅ **Audit trail** : Modifications peuvent être enregistrées (extension future)  

***

### Améliorations futures

- **Enregistrement automatique des Modifications** (historique complet)
- **Alertes de retard** : SMS/Email au retour en retard
- **Impression de reçu** : Détails du retour et pénalités
- **Statistiques par abonné** : Nombre de retards, fidélité tendance
- **Règles de fidélité personnalisées** : Par catégorie de livre, abonné VIP, etc.

***
