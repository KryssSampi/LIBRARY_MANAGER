using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LIBBRARY_MANAGER.Model;

namespace LIBBRARY_MANAGER.UI.Modules.AccountStaffManager.ViewModel
{
    public class AccountManagerViewModel : INotifyPropertyChanged
    {
        private StaffMember _currentStaff;
        private string _avatarPath;
        private bool _hasChanges;

        // Propriétés originales (lecture seule)
        public string RefStaff => _currentStaff?.Ref_Staff ?? "N/A";
        public string Poste => _currentStaff?.Poste ?? "Employé";
        public int Anciennete => _currentStaff?.Anciennete ?? 0;
        public DateTime DateCreation => _currentStaff?.Date_Creation ?? DateTime.Now;

        // Propriétés éditables
        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                    ValidateName();
                    HasChanges = true;
                }
            }
        }

        private DateTime? _birthDate;
        public DateTime? BirthDate
        {
            get => _birthDate;
            set
            {
                if (_birthDate != value)
                {
                    _birthDate = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Age));
                    ValidateBirthDate();
                    HasChanges = true;
                }
            }
        }

        public int? Age => _birthDate.HasValue
            ? DateTime.Now.Year - _birthDate.Value.Year - (DateTime.Now.DayOfYear < _birthDate.Value.DayOfYear ? 1 : 0)
            : null;

        private string _adresse;
        public string Adresse
        {
            get => _adresse;
            set
            {
                if (_adresse != value)
                {
                    _adresse = value;
                    OnPropertyChanged();
                    HasChanges = true;
                }
            }
        }

        private string _email;
        public string Email
        {
            get => _email;
            set
            {
                if (_email != value)
                {
                    _email = value;
                    OnPropertyChanged();
                    ValidateEmail();
                    HasChanges = true;
                }
            }
        }

        private string _telephone;
        public string Telephone
        {
            get => _telephone;
            set
            {
                if (_telephone != value)
                {
                    _telephone = value;
                    OnPropertyChanged();
                    ValidateTelephone();
                    HasChanges = true;
                }
            }
        }

        // Mot de passe
        private string _newPassword;
        public string NewPassword
        {
            get => _newPassword;
            set
            {
                if (_newPassword != value)
                {
                    _newPassword = value;
                    OnPropertyChanged();
                    ValidatePassword();
                    HasChanges = true;
                }
            }
        }

        private string _confirmPassword;
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                if (_confirmPassword != value)
                {
                    _confirmPassword = value;
                    OnPropertyChanged();
                    ValidatePassword();
                    HasChanges = true;
                }
            }
        }

        private bool _isPasswordEditMode;
        public bool IsPasswordEditMode
        {
            get => _isPasswordEditMode;
            set
            {
                _isPasswordEditMode = value;
                OnPropertyChanged();
                if (!value)
                {
                    NewPassword = string.Empty;
                    ConfirmPassword = string.Empty;
                }
            }
        }

        // États de validation
        private bool _nameIsValid = true;
        public bool NameIsValid
        {
            get => _nameIsValid;
            set { _nameIsValid = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanSave)); }
        }

        private string _nameError = "";
        public string NameError
        {
            get => _nameError;
            set { _nameError = value; OnPropertyChanged(); }
        }

        private bool _emailIsValid = true;
        public bool EmailIsValid
        {
            get => _emailIsValid;
            set { _emailIsValid = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanSave)); }
        }

        private string _emailError = "";
        public string EmailError
        {
            get => _emailError;
            set { _emailError = value; OnPropertyChanged(); }
        }

        private bool _telephoneIsValid = true;
        public bool TelephoneIsValid
        {
            get => _telephoneIsValid;
            set { _telephoneIsValid = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanSave)); }
        }

        private string _telephoneError = "";
        public string TelephoneError
        {
            get => _telephoneError;
            set { _telephoneError = value; OnPropertyChanged(); }
        }

        private bool _birthDateIsValid = true;
        public bool BirthDateIsValid
        {
            get => _birthDateIsValid;
            set { _birthDateIsValid = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanSave)); }
        }

        private string _birthDateError = "";
        public string BirthDateError
        {
            get => _birthDateError;
            set { _birthDateError = value; OnPropertyChanged(); }
        }

        private bool _passwordIsValid = true;
        public bool PasswordIsValid
        {
            get => _passwordIsValid;
            set { _passwordIsValid = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanSave)); }
        }

        private string _passwordError = "";
        public string PasswordError
        {
            get => _passwordError;
            set { _passwordError = value; OnPropertyChanged(); }
        }

        private string _passwordStrength = "";
        public string PasswordStrength
        {
            get => _passwordStrength;
            set { _passwordStrength = value; OnPropertyChanged(); }
        }

        private Brush _passwordStrengthColor = Brushes.White;
        public Brush PasswordStrengthColor
        {
            get => _passwordStrengthColor;
            set { _passwordStrengthColor = value; OnPropertyChanged(); }
        }

        public bool HasChanges
        {
            get => _hasChanges;
            set
            {
                _hasChanges = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanSave));
            }
        }

        public bool CanSave
        {
            get
            {
                bool basicValid = HasChanges && NameIsValid && EmailIsValid && TelephoneIsValid && BirthDateIsValid;

                // Si on modifie le mot de passe, il doit être valide
                if (IsPasswordEditMode && !string.IsNullOrEmpty(NewPassword))
                {
                    return basicValid && PasswordIsValid;
                }

                return basicValid;
            }
        }

        public string AvatarPath
        {
            get => _avatarPath;
            set
            {
                _avatarPath = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(AvatarSource));
            }
        }

        public ImageSource AvatarSource
        {
            get
            {
                if (string.IsNullOrEmpty(_avatarPath))
                    return null;

                try
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(_avatarPath, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    return bitmap;
                }
                catch
                {
                    return null;
                }
            }
        }

        // Commandes
        public ICommand SaveCommand { get; }
        public ICommand ResetCommand { get; }
        public ICommand TogglePasswordEditCommand { get; }

        public AccountManagerViewModel(StaffMember? Personnel = null)
        {
            SaveCommand = new RelayCommand(SaveChanges, () => CanSave);
            ResetCommand = new RelayCommand(ResetChanges);
            TogglePasswordEditCommand = new RelayCommand(TogglePasswordEdit);
            if (Personnel is not null)
            {

                this._currentStaff = Personnel;
                initializeEditableInfos();
            }
            else
            {
                CreateFictionalStaff();
            }
        }

        private void CreateFictionalStaff()
        {
            _currentStaff = new StaffMember
            {
                Id_User = 1001,
                Name_User = "Sophie Marchand",
                BirthDate = new DateTime(1992, 6, 15),
                Adresse = "25 Rue de la République, 75011 Paris",
                Adresse_Mail = "sophie.marchand@noctua-library.fr",
                Num_Telephone = "+33 6 78 94 52 31",
                Poste = "Bibliothécaire Senior",
                YearHired = 2018,
                Password = "SecurePass2024!"
            };

            _currentStaff.GenerateReference();

            initializeEditableInfos();

            OnPropertyChanged("");
            HasChanges = false;
        }
        private void initializeEditableInfos()
        {

            _name ??= _currentStaff.Name_User;
            _birthDate ??= _currentStaff.BirthDate;
            _adresse ??= _currentStaff.Adresse;
            _email ??= _currentStaff.Adresse_Mail;
            _telephone ??= _currentStaff.Num_Telephone;
        }

        private void ValidateName()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                NameIsValid = false;
                NameError = "Le nom ne peut pas être vide";
            }
            else if (Name.Length < 3)
            {
                NameIsValid = false;
                NameError = "Le nom doit contenir au moins 3 caractères";
            }
            else if (Name.Length > 100)
            {
                NameIsValid = false;
                NameError = "Le nom ne peut pas dépasser 100 caractères";
            }
            else
            {
                NameIsValid = true;
                NameError = "";
            }
        }

        private void ValidateEmail()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                EmailIsValid = true;
                EmailError = "";
                return;
            }

            var emailRegex = new System.Text.RegularExpressions.Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!emailRegex.IsMatch(Email))
            {
                EmailIsValid = false;
                EmailError = "Format d'email invalide";
            }
            else if (Email.Length > 100)
            {
                EmailIsValid = false;
                EmailError = "L'email ne peut pas dépasser 100 caractères";
            }
            else
            {
                EmailIsValid = true;
                EmailError = "";
            }
        }

        private void ValidateTelephone()
        {
            if (string.IsNullOrWhiteSpace(Telephone))
            {
                TelephoneIsValid = true;
                TelephoneError = "";
                return;
            }

            var cleanPhone = System.Text.RegularExpressions.Regex.Replace(Telephone, @"[^\d+]", "");
            if (cleanPhone.Length < 10)
            {
                TelephoneIsValid = false;
                TelephoneError = "Numéro de téléphone invalide";
            }
            else if (Telephone.Length > 20)
            {
                TelephoneIsValid = false;
                TelephoneError = "Le numéro ne peut pas dépasser 20 caractères";
            }
            else
            {
                TelephoneIsValid = true;
                TelephoneError = "";
            }
        }

        private void ValidateBirthDate()
        {
            if (!BirthDate.HasValue)
            {
                BirthDateIsValid = true;
                BirthDateError = "";
                return;
            }

            if (BirthDate.Value > DateTime.Now)
            {
                BirthDateIsValid = false;
                BirthDateError = "La date de naissance ne peut pas être dans le futur";
            }
            else if (BirthDate.Value.Year < 1900)
            {
                BirthDateIsValid = false;
                BirthDateError = "Date de naissance invalide";
            }
            else if (Age < 18)
            {
                BirthDateIsValid = false;
                BirthDateError = "Le personnel doit avoir au moins 18 ans";
            }
            else
            {
                BirthDateIsValid = true;
                BirthDateError = "";
            }
        }

        private void ValidatePassword()
        {
            if (!IsPasswordEditMode || string.IsNullOrEmpty(NewPassword))
            {
                PasswordIsValid = true;
                PasswordError = "";
                PasswordStrength = "";
                return;
            }

            // Vérifier la force du mot de passe
            bool hasUpper = NewPassword.Any(char.IsUpper);
            bool hasLower = NewPassword.Any(char.IsLower);
            bool hasDigit = NewPassword.Any(char.IsDigit);
            bool hasSpecial = NewPassword.Any(ch => !char.IsLetterOrDigit(ch));
            int length = NewPassword.Length;

            int score = 0;
            if (length >= 8) score++;
            if (hasUpper) score++;
            if (hasLower) score++;
            if (hasDigit) score++;
            if (hasSpecial) score++;

            if (length < 8)
            {
                PasswordStrength = "❌ Trop court (min. 8 caractères)";
                PasswordStrengthColor = new SolidColorBrush(Colors.Red);
            }
            else if (score >= 4)
            {
                PasswordStrength = "✅ Fort";
                PasswordStrengthColor = new SolidColorBrush(Colors.LimeGreen);
            }
            else if (score >= 3)
            {
                PasswordStrength = "⚠️ Moyen";
                PasswordStrengthColor = new SolidColorBrush(Colors.Orange);
            }
            else
            {
                PasswordStrength = "❌ Faible";
                PasswordStrengthColor = new SolidColorBrush(Colors.Red);
            }

            // Vérifier la correspondance
            if (string.IsNullOrEmpty(ConfirmPassword))
            {
                PasswordIsValid = false;
                PasswordError = "Veuillez confirmer le mot de passe";
            }
            else if (NewPassword != ConfirmPassword)
            {
                PasswordIsValid = false;
                PasswordError = "Les mots de passe ne correspondent pas";
            }
            else if (score < 3)
            {
                PasswordIsValid = false;
                PasswordError = "Le mot de passe est trop faible";
            }
            else
            {
                PasswordIsValid = true;
                PasswordError = "";
            }
        }

        private void TogglePasswordEdit()
        {
            IsPasswordEditMode = !IsPasswordEditMode;
            if (!IsPasswordEditMode)
            {
                NewPassword = string.Empty;
                ConfirmPassword = string.Empty;
                PasswordIsValid = true;
                PasswordError = "";
                PasswordStrength = "";
            }
        }

        private void SaveChanges()
        {
            if (!CanSave) return;

            _currentStaff.Name_User = Name;
            _currentStaff.BirthDate = BirthDate;
            _currentStaff.Adresse = Adresse;
            _currentStaff.Adresse_Mail = Email;
            _currentStaff.Num_Telephone = Telephone;

            if (IsPasswordEditMode && !string.IsNullOrEmpty(NewPassword))
            {
                _currentStaff.Password = NewPassword;
            }

            HasChanges = false;
            IsPasswordEditMode = false;

            MessageBox.Show(
                "Vos informations ont été modifiées avec succès !",
                "Succès",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }
       private void ResetStyle() { 
       
        
        }
        private void ResetChanges()
        {
            if (!HasChanges) return;

            var result = MessageBox.Show(
                "Êtes-vous sûr de vouloir annuler toutes les modifications ?",
                "Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                Name = _currentStaff.Name_User;
                BirthDate = _currentStaff.BirthDate;
                Adresse = _currentStaff.Adresse;
                Email = _currentStaff.Adresse_Mail;
                Telephone = _currentStaff.Num_Telephone;
                IsPasswordEditMode = false;
                NewPassword = string.Empty;
                ConfirmPassword = string.Empty;

                HasChanges = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute();

        public void Execute(object parameter) => _execute();
    }
}