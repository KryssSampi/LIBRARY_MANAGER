
## README: Module Account Manager

### Gestionnaire de Profil et Paramètres Utilisateur

Le module **AccountManager** est un module complet de gestion de profil utilisateur (StaffMember) permettant la modification sécurisée des informations personnelles, professionnelles et du mot de passe, avec validation en temps réel et feedback visuel élégant.

***

### Structure du Module

```
/AccountStaffManager
├── Views/
│   ├── AccountManager.xaml            # Interface principale du module
│   └── AccountManager.xaml.cs         # Code-behind
├── ViewModels/
│   └── AccountManagerViewModel.cs     # Logique métier et validation
├── Items/ (Composants réutilisables)
│   ├── EditerLabel.xaml               # Champ éditable avec crayon
│   ├── EditerLabel.xaml.cs
│   ├── AvatarEditor.xaml              # Éditeur d'avatar avec popup
│   ├── AvatarEditor.xaml.cs
│   ├── ImageGridSelector.xaml         # Sélecteur de galerie d'images
│   └── ImageGridSelector.xaml.cs
└── Models/
    ├── StaffMember.cs                 # Entité utilisateur Staff
    └── User.cs                        # Classe de base abstraite
```

***

### Vue (XAML) - AccountManager

**Layout principal** : Grid 2 colonnes (35% / 65%)

#### **Colonne Gauche (35%)** : Avatar + Informations Statiques

1. **AvatarEditor** (composant custom)
   - Taille : 180x180 px
   - Bordure circulaire DarkBlue
   - Bouton d'édition (icône crayon) → ouvre popup de sélection
   - Binding : `AvatarSource`

2. **Username + Ref**
   - Nom : `{Binding Name}` (FontSize 24, Bold, DarkBlue)
   - Référence : `@{Binding RefStaff}` (FontSize 16, LightBlue)

3. **Carte Informations Professionnelles** (Border arrondi blanc + ombre)
   - **Poste** : `{Binding Poste}` (ex: Bibliothécaire Principal)
   - **Ancienneté** : `{Binding Anciennete} ans` (calculé depuis YearHired)
   - **Membre depuis** : `{Binding DateCreation, StringFormat='dd MMMM yyyy'}`
   - **Âge** : `{Binding Age} ans` (calculé depuis BirthDate, nullable)

#### **Colonne Droite (65%)** : Formulaire d'Édition

**En-tête** : "Paramètres du Compte" (FontSize 28, Bold)

**Champs éditables** (ScrollViewer) :

1. **Nom Complet*** (requis)
   - Composant : `EditerLabel`
   - Validation : Non vide, longueur min 3 caractères
   - Indicateur visuel : Check vert (valide) / Cross rouge (invalide)
   - Message d'erreur contextuel

2. **Date de Naissance**
   - Composant : `DatePicker` dans Border stylisé
   - Validation : Âge >= 18 ans
   - Calcul automatique de l'âge

3. **Adresse**
   - Composant : `EditerLabel`
   - Validation : Aucune (optionnel)

4. **Adresse Email*** (requis)
   - Composant : `EditerLabel`
   - Validation : Format email valide (Regex)
   - Indicateurs Check/Cross
   - Message d'erreur : "Format email invalide"

5. **Numéro de Téléphone**
   - Composant : `EditerLabel`
   - Validation : Format téléphone (optionnel mais validé si rempli)
   - Indicateurs Check/Cross

6. **Section Mot de Passe** (collapsible)
   - Bouton "🔐 Modifier le mot de passe" → toggle section
   - **Nouveau mot de passe** :
     - `PasswordBox` avec bouton Show/Hide (Eye icon)
     - Validation force : Faible/Moyen/Fort
     - Messages : "Mot de passe trop court", "Fort (contient majuscules, chiffres, symboles)"
   - **Confirmer mot de passe** :
     - `EditerLabel` en mode password
     - Validation : Égalité avec nouveau mot de passe
     - Message : "Les mots de passe ne correspondent pas"

**Indicateur de modifications** :
- Badge jaune avec icône info : "Vous avez des modifications non sauvegardées"
- Visible si `HasChanges == true`

**Boutons d'action** :
- **Réinitialiser** (rouge #DC3545)
  - Command : `ResetCommand`
  - IsEnabled : `{Binding HasChanges}`
  - Réinitialise tous les champs aux valeurs originales
- **Sauvegarder** (vert #28A745)
  - Command : `SaveCommand`
  - IsEnabled : `{Binding CanSave}` (HasChanges + toutes validations OK)
  - Sauvegarde en base de données

***

### ViewModel - AccountManagerViewModel

**Propriétés principales** :

```csharp
// Données originales (lecture seule)
private StaffMember _currentStaff;
public string RefStaff => _currentStaff?.Ref_Staff ?? "N/A";
public string Poste => _currentStaff?.Poste ?? "Employé";
public int Anciennete => _currentStaff?.Anciennete ?? 0;
public DateTime DateCreation => _currentStaff?.Date_Creation ?? DateTime.Now;

// Propriétés éditables
private string _name;
public string Name { get; set; } // avec validation OnPropertyChanged

private DateTime? _birthDate;
public DateTime? BirthDate { get; set; } // recalcule Age

private string _email;
public string Email { get; set; } // avec validation Regex

private string _telephone;
public string Telephone { get; set; } // validation format

private string _adresse;
public string Adresse { get; set; }

// Mot de passe
private string _newPassword;
public string NewPassword { get; set; } // avec validation force

private string _confirmPassword;
public string ConfirmPassword { get; set; } // avec validation égalité

// États
public bool HasChanges { get; set; } // true si modifications détectées
public bool CanSave { get; set; } // true si HasChanges + validations OK
public bool IsPasswordEditMode { get; set; } // toggle section password
public bool ShowValidationIndicators { get; set; } // affiche Check/Cross après soumission

// Messages de validation
public string NameError { get; set; }
public string EmailError { get; set; }
public string TelephoneError { get; set; }
public string PasswordError { get; set; }
public string PasswordStrength { get; set; }

// Couleurs des bordures (dynamiques selon validation)
public SolidColorBrush NameBorderColor { get; set; }
public SolidColorBrush EmailBorderColor { get; set; }
public SolidColorBrush TelephoneBorderColor { get; set; }
public SolidColorBrush PasswordBorderColor { get; set; }

// Couleurs des messages d'erreur
public SolidColorBrush NameErrorColor { get; set; }
public SolidColorBrush EmailErrorColor { get; set; }
public SolidColorBrush TelephoneErrorColor { get; set; }
public SolidColorBrush PasswordErrorColor { get; set; }
public SolidColorBrush PasswordStrengthColor { get; set; }

// Validation booléenne (pour converter Check/Cross)
public bool NameIsValid { get; set; }
public bool EmailIsValid { get; set; }
public bool TelephoneIsValid { get; set; }
public bool PasswordIsValid { get; set; }

// Avatar
private ImageSource _avatarSource;
public ImageSource AvatarSource { get; set; }

// Age calculé
public int? Age => BirthDate != null 
    ? (DateTime.Now.Year - BirthDate.Value.Year) : null;
```

**Commandes** :

```csharp
public ICommand SaveCommand { get; } // RelayCommand avec CanExecute
public ICommand ResetCommand { get; }
public ICommand TogglePasswordEditCommand { get; }
```

***

### Logique de Validation

#### **1. Validation du Nom**

```csharp
private void ValidateName()
{
    if (string.IsNullOrWhiteSpace(Name))
    {
        NameError = "Le nom est obligatoire";
        NameErrorColor = Brushes.Red;
        NameBorderColor = Brushes.Red;
        NameIsValid = false;
    }
    else if (Name.Length < 3)
    {
        NameError = "Le nom doit contenir au moins 3 caractères";
        NameErrorColor = Brushes.Red;
        NameBorderColor = Brushes.Red;
        NameIsValid = false;
    }
    else
    {
        NameError = string.Empty;
        NameBorderColor = FindResource("PrimaryBlue") as SolidColorBrush;
        NameIsValid = true;
    }
    UpdateCanSave();
}
```

#### **2. Validation de l'Email**

```csharp
private void ValidateEmail()
{
    if (string.IsNullOrWhiteSpace(Email))
    {
        EmailError = "L'email est obligatoire";
        EmailIsValid = false;
        EmailBorderColor = Brushes.Red;
    }
    else if (!Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
    {
        EmailError = "Format email invalide";
        EmailIsValid = false;
        EmailBorderColor = Brushes.Red;
    }
    else
    {
        EmailError = string.Empty;
        EmailIsValid = true;
        EmailBorderColor = FindResource("PrimaryBlue") as SolidColorBrush;
    }
    UpdateCanSave();
}
```

#### **3. Validation du Téléphone**

```csharp
private void ValidateTelephone()
{
    // Si vide, pas d'erreur (optionnel)
    if (string.IsNullOrWhiteSpace(Telephone))
    {
        TelephoneError = string.Empty;
        TelephoneIsValid = true;
        TelephoneBorderColor = FindResource("PrimaryBlue") as SolidColorBrush;
        return;
    }
    
    // Validation format (10 chiffres, optionnel +, -, espaces)
    if (!Regex.IsMatch(Telephone, @"^[\d\s\-\+\(\)]{10,15}$"))
    {
        TelephoneError = "Format invalide (10-15 chiffres)";
        TelephoneIsValid = false;
        TelephoneBorderColor = Brushes.Red;
    }
    else
    {
        TelephoneError = string.Empty;
        TelephoneIsValid = true;
        TelephoneBorderColor = FindResource("PrimaryBlue") as SolidColorBrush;
    }
    UpdateCanSave();
}
```

#### **4. Validation du Mot de Passe**

```csharp
private void ValidatePassword()
{
    if (!IsPasswordEditMode) return; // Skip si pas en mode édition mot de passe
    
    if (string.IsNullOrWhiteSpace(NewPassword))
    {
        PasswordError = "Le mot de passe est obligatoire";
        PasswordIsValid = false;
        PasswordBorderColor = Brushes.Red;
        return;
    }
    
    // Vérification force
    int strength = CalculatePasswordStrength(NewPassword);
    
    if (strength < 2)
    {
        PasswordStrength = "⚠️ Mot de passe faible";
        PasswordStrengthColor = Brushes.Red;
        PasswordIsValid = false;
    }
    else if (strength == 2)
    {
        PasswordStrength = "🔶 Mot de passe moyen";
        PasswordStrengthColor = Brushes.Orange;
        PasswordIsValid = true;
    }
    else
    {
        PasswordStrength = "✅ Mot de passe fort";
        PasswordStrengthColor = Brushes.LimeGreen;
        PasswordIsValid = true;
    }
    
    // Vérification correspondance
    if (NewPassword != ConfirmPassword)
    {
        PasswordError = "Les mots de passe ne correspondent pas";
        PasswordErrorColor = Brushes.Red;
        PasswordIsValid = false;
    }
    else
    {
        PasswordError = string.Empty;
        PasswordIsValid = true;
    }
    
    UpdateCanSave();
}

private int CalculatePasswordStrength(string password)
{
    int score = 0;
    if (password.Length >= 8) score++;
    if (Regex.IsMatch(password, @"[A-Z]")) score++; // Majuscule
    if (Regex.IsMatch(password, @"[a-z]")) score++; // Minuscule
    if (Regex.IsMatch(password, @"\d")) score++; // Chiffre
    if (Regex.IsMatch(password, @"[\W_]")) score++; // Symbole
    
    return score >= 4 ? 3 : (score >= 2 ? 2 : 1); // Fort / Moyen / Faible
}
```

***

### Commandes (ICommand)

#### **SaveCommand**

```csharp
private void ExecuteSave()
{
    // Déclencher les validations
    ShowValidationIndicators = true;
    ValidateName();
    ValidateEmail();
    ValidateTelephone();
    if (IsPasswordEditMode) ValidatePassword();
    
    // Vérifier si toutes les validations passent
    if (!CanSave) return;
    
    using (var context = new LibraryDbContext())
    {
        var staff = context.StaffMembers.Find(_currentStaff.Id_User);
        
        // Appliquer les modifications
        staff.Name_User = Name;
        staff.Adresse_Mail = Email;
        staff.Adresse = Adresse;
        staff.Num_Telephone = Telephone;
        staff.BirthDate = BirthDate;
        
        // Mot de passe (si modifié)
        if (IsPasswordEditMode && !string.IsNullOrWhiteSpace(NewPassword))
        {
            staff.HashPassword(NewPassword);
        }
        
        context.SaveChanges();
    }
    
    // Mise à jour de l'objet courant
    _currentStaff = context.StaffMembers.Find(_currentStaff.Id_User);
    
    // Réinitialisation des états
    HasChanges = false;
    ShowValidationIndicators = false;
    IsPasswordEditMode = false;
    
    MessageBox.Show("✅ Profil mis à jour avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
}
```

#### **ResetCommand**

```csharp
private void ExecuteReset()
{
    // Recharger les valeurs originales
    Name = _currentStaff.Name_User;
    Email = _currentStaff.Adresse_Mail;
    Adresse = _currentStaff.Adresse;
    Telephone = _currentStaff.Num_Telephone;
    BirthDate = _currentStaff.BirthDate;
    
    // Réinitialiser les validations
    ShowValidationIndicators = false;
    HasChanges = false;
    IsPasswordEditMode = false;
    NewPassword = string.Empty;
    ConfirmPassword = string.Empty;
    
    // Réinitialiser les couleurs de bordure
    NameBorderColor = FindResource("PrimaryBlue") as SolidColorBrush;
    EmailBorderColor = FindResource("PrimaryBlue") as SolidColorBrush;
    TelephoneBorderColor = FindResource("PrimaryBlue") as SolidColorBrush;
}
```

***

### Composants Réutilisables

#### **1. EditerLabel** (Champ éditable avec crayon)

**Propriétés** :
- `LabelTitle` : Titre du champ (ex: "Nom Complet *")
- `EditableInfos` : Valeur bindée (TwoWay)
- `IsReadOnly` : true par défaut, false quand édition activée
- `LabelTitleColor`, `LabelColor`, `InfoColor`, `PencilColor` : couleurs personnalisables
- `CheckIsVisible`, `CrossIsVisible` : visibilité des indicateurs (géré par parent)

**Fonctionnement** :
- Clic sur crayon → `IsReadOnly = false` + focus TextBox
- Événement `EditCommand` déclenché pour notifier le parent

#### **2. AvatarEditor** (Éditeur d'avatar avec popup)

**Propriétés** :
- `AvatarSource` : ImageSource bindée
- `AvatarSize` : Taille de l'avatar (180px)
- `BorderBrush`, `DefaultBackground` : styles
- `EditButtonSize`, `EditIconSize` : tailles du bouton d'édition
- `AvatarFolderPath` : chemin du dossier d'avatars prédéfinis

**Fonctionnement** :
- Clic sur bouton "Modifier" → Popup s'ouvre
- Popup contient `ImageGridSelector` (galerie d'images)
- Sélection d'une image → `AvatarSource` mis à jour
- Support drag & drop de fichiers locaux

#### **3. ImageGridSelector** (Galerie d'images)

**Propriétés** :
- `ItemsSource` : Collection d'images (chemins ou URLs)
- `SelectedItem` : Image sélectionnée (bindée TwoWay)
- `Columns` : Nombre de colonnes de la grille (3 par défaut)

**Fonctionnement** :
- Affiche une grille d'images cliquables
- Sélection → bordure bleue + événement `SelectionChanged`
- Intégration avec AvatarEditor via `SetTargetContainer()`

***

### Base de Données

**Modèles utilisés** :
- `StaffMember` : Entité principale
- `User` (classe de base) : Contient propriétés communes

**Méthodes de l'entité StaffMember** :
```csharp
public class StaffMember : User
{
    public string Poste { get; set; }
    public int YearHired { get; set; }
    public string Ref_Staff { get; set; } // Auto-généré
    
    // Calcul ancienneté
    public int Anciennete => DateTime.Now.Year - YearHired;
    
    // Méthodes métier
    public void UpdateProfile(string name, string email, DateTime? birthDate, string adresse, string telephone)
    {
        Name_User = name;
        Adresse_Mail = email;
        BirthDate = birthDate;
        Adresse = adresse;
        Num_Telephone = telephone;
    }
}
```

***

### Technologies et Outils

| Technologie               | Utilisation                                          |
|---------------------------|------------------------------------------------------|
| **WPF (XAML)**            | Interface utilisateur                                |
| **MVVM Pattern**          | Séparation Vue / ViewModel / Modèle                  |
| **INotifyPropertyChanged**| Binding bidirectionnel réactif                       |
| **ICommand (RelayCommand)**| Commandes liées aux boutons                         |
| **Regex**                 | Validation format email, téléphone                   |
| **Entity Framework Core** | Accès base de données, SaveChanges()                 |
| **MultiBinding + Converter**| Visibilité Check/Cross selon validation            |
| **DependencyProperty**    | Propriétés bindables des composants custom           |
| **PasswordBox**           | Champ sécurisé pour mot de passe                     |
| **DatePicker**            | Sélection de date de naissance                       |

***

### Fonctionnalités Clés

✅ **Validation temps réel** : Feedback immédiat sur chaque champ  
✅ **Indicateurs visuels** : Check vert / Cross rouge après soumission  
✅ **Force mot de passe** : Calcul dynamique (Faible/Moyen/Fort)  
✅ **Avatar personnalisable** : Galerie + drag & drop  
✅ **Détection de modifications** : Badge d'avertissement  
✅ **Sauvegarde atomique** : Transaction EF Core  
✅ **Réinitialisation** : Retour aux valeurs originales  
✅ **Composants réutilisables** : EditerLabel, AvatarEditor, ImageGridSelector  
✅ **UX soignée** : Animations, ombres portées, couleurs dynamiques  

***

### Améliorations Futures

- **Historique des modifications** : Traçabilité des changements (table Modifications)
- **Upload d'avatar custom** : Serveur de fichiers/CDN
- **Double authentification** : Code SMS/Email avant changement de password
- **Validation asynchrone** : Vérifier unicité email en base
- **Mode sombre** : Thème personnalisable
- **Export de profil** : PDF récapitulatif

***
