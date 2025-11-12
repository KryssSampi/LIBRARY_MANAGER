using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using LIBBRARY_MANAGER.Data;
using LIBBRARY_MANAGER.Model;
using Microsoft.EntityFrameworkCore;

namespace LIBBRARY_MANAGER.ViewModel.LoanViewModels
{
    /// <summary>
    /// ViewModel pour l'affichage d'un prêt dans la liste
    /// </summary>
    public class LoanItemViewModel : INotifyPropertyChanged
    {
        #region Propriétés du modèle

        private readonly Loan _loan;

        /// <summary>
        /// Instance du modèle Loan
        /// </summary>
        public Loan LoanInstance => _loan;

        /// <summary>
        /// Référence du prêt
        /// </summary>
        public string RefLoan => _loan.Ref_Loan;

        /// <summary>
        /// Nom de l'abonné
        /// </summary>
        public string SubscriberName => _loan.Subscriber?.Name_User ?? "Inconnu";

        /// <summary>
        /// Titre du livre
        /// </summary>
        public string BookTitle => _loan.Book?.Title ?? "Inconnu";

        /// <summary>
        /// Date d'emprunt
        /// </summary>
        public DateTime BorrowDate => _loan.BorrowDate;

        /// <summary>
        /// Date de retour prévue
        /// </summary>
        public DateTime ReturnDate => _loan.ReturnDate;

        /// <summary>
        /// Indique si le prêt est actif
        /// </summary>
        public bool IsActive => _loan.IsActive;

        /// <summary>
        /// Indique si le prêt est en retard
        /// </summary>
        public bool IsLate => _loan.IsLate;

        /// <summary>
        /// Nombre de jours de retard
        /// </summary>
        public int DaysLate => _loan.DaysLate;

        /// <summary>
        /// Pénalité éventuelle
        /// </summary>
        public decimal? Penalty => _loan.Penalty;

        #endregion

        #region Propriétés calculées pour l'UI

        /// <summary>
        /// Label du statut
        /// </summary>
        public string StatusLabel
        {
            get
            {
                if (!IsActive) return "Retourné";
                if (IsLate) return $"En retard ({DaysLate}j)";
                return "En cours";
            }
        }

        /// <summary>
        /// Couleur du statut
        /// </summary>
        public Brush StatusColor
        {
            get
            {
                if (!IsActive)
                    return new SolidColorBrush(Color.FromRgb(117, 117, 117)); // Gris
                if (IsLate)
                    return new SolidColorBrush(Color.FromRgb(244, 67, 54)); // Rouge
                return new SolidColorBrush(Color.FromRgb(76, 175, 80)); // Vert
            }
        }

        /// <summary>
        /// Texte de la pénalité formaté
        /// </summary>
        public string PenaltyText => Penalty.HasValue ? $"{Penalty:C2}" : "-";

        /// <summary>
        /// Couleur de la pénalité
        /// </summary>
        public Brush PenaltyColor
        {
            get
            {
                if (!Penalty.HasValue || Penalty == 0)
                    return new SolidColorBrush(Color.FromRgb(117, 117, 117)); // Gris
                return new SolidColorBrush(Color.FromRgb(244, 67, 54)); // Rouge
            }
        }

        /// <summary>
        /// Date d'emprunt formatée
        /// </summary>
        public string BorrowDateFormatted => BorrowDate.ToString("dd MMM yyyy");

        /// <summary>
        /// Date de retour formatée
        /// </summary>
        public string ReturnDateFormatted => ReturnDate.ToString("dd MMM yyyy");

        #endregion

        #region Constructeur

        public LoanItemViewModel(Loan loan)
        {
            _loan = loan ?? throw new ArgumentNullException(nameof(loan));

            System.Diagnostics.Debug.WriteLine(
                $"[LoanItemViewModel] Créé pour {RefLoan} - {BookTitle}"
            );
        }

        #endregion

        #region Méthodes utilitaires

        /// <summary>
        /// Retourne l'instance du modèle Loan
        /// </summary>
        public Loan GetLoan() => _loan;

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Rafraîchit toutes les propriétés (après modification du modèle)
        /// </summary>
        public void RefreshAll()
        {
            OnPropertyChanged(nameof(IsActive));
            OnPropertyChanged(nameof(IsLate));
            OnPropertyChanged(nameof(DaysLate));
            OnPropertyChanged(nameof(Penalty));
            OnPropertyChanged(nameof(StatusLabel));
            OnPropertyChanged(nameof(StatusColor));
            OnPropertyChanged(nameof(PenaltyText));
            OnPropertyChanged(nameof(PenaltyColor));
        }

        #endregion
    }
    /// <summary>
    /// ViewModel pour le panneau de détails d'un prêt
    /// </summary>
    /// <summary>
    /// ViewModel pour le panneau de détails d'un prêt
    /// </summary>

    /// <summary>
    /// ViewModel pour le panneau de détails d'un prêt avec modification de date
    /// </summary>
    public class LoanDetailViewModel : INotifyPropertyChanged
    {
        #region Services
        private LibraryDbContext _context;
        private readonly StaffMember _currentStaff;
        #endregion

        #region Propriétés du modèle
        private Loan _loan;

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
                    OnPropertyChanged(nameof(IsLate));
                    OnPropertyChanged(nameof(DaysLate));
                    UpdateCanSaveReturnDate();
                }
            }
        }

        public bool IsActive => _loan.IsActive;
        public decimal? Penalty => _loan.Penalty;
        public bool IsLate => IsActive && DateTime.Now.Date > ReturnDate.Date;
        public int DaysLate => IsLate ? (DateTime.Now.Date - ReturnDate.Date).Days : 0;
        #endregion

        #region Propriétés pour modification de date
        private DateTime _newReturnDate;
        public DateTime NewReturnDate
        {
            get => _newReturnDate;
            set
            {
                if (_newReturnDate != value)
                {
                    _newReturnDate = value;
                    OnPropertyChanged();
                    UpdateCanSaveReturnDate();
                }
            }
        }

        private bool _isEditingReturnDate = false;
        public bool IsEditingReturnDate
        {
            get => _isEditingReturnDate;
            set
            {
                if (_isEditingReturnDate != value)
                {
                    _isEditingReturnDate = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _returnDateErrorMessage;
        public string ReturnDateErrorMessage
        {
            get => _returnDateErrorMessage;
            set
            {
                if (_returnDateErrorMessage != value)
                {
                    _returnDateErrorMessage = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(HasReturnDateError));
                }
            }
        }

        public bool HasReturnDateError => !string.IsNullOrEmpty(ReturnDateErrorMessage);
        #endregion

        #region Propriétés calculées
        public string StatusLabel => IsActive ? "En cours" : "Retourné";

        public Brush StatusColor
        {
            get
            {
                if (!IsActive)
                    return new SolidColorBrush(Color.FromRgb(117, 117, 117));
                if (IsLate)
                    return new SolidColorBrush(Color.FromRgb(244, 67, 54));
                return new SolidColorBrush(Color.FromRgb(76, 175, 80));
            }
        }

        public Brush PenaltyColor => Penalty.HasValue && Penalty > 0
            ? new SolidColorBrush(Color.FromRgb(244, 67, 54))
            : new SolidColorBrush(Color.FromRgb(117, 117, 117));
        #endregion

        #region Commandes
        public ICommand CloseCommand { get; }
        public ICommand OpenReturnPanelCommand { get; }
        public ICommand EditReturnDateCommand { get; }
        public ICommand SaveReturnDateCommand { get; }
        public ICommand CancelEditReturnDateCommand { get; }
        #endregion

        #region Events
        public event EventHandler CloseRequested;
        public event EventHandler<Loan> ReturnPanelRequested;
        public event EventHandler LoanModified;
        #endregion

        #region Constructeur
        public LoanDetailViewModel(Loan loan)
        {
            _loan = loan ?? throw new ArgumentNullException(nameof(loan));
            _currentStaff = App._currentconnected_User;

            if (_currentStaff == null)
                throw new InvalidOperationException("Aucun membre du personnel connecté");

            // Initialiser les dates
            _returnDate = _loan.ReturnDate;
            _newReturnDate = _loan.ReturnDate;

            // Initialiser les commandes
            CloseCommand = new RelayCommand(() => CloseRequested?.Invoke(this, EventArgs.Empty));

            OpenReturnPanelCommand = new RelayCommand(
                () =>
                {
                    LogInfo($"OpenReturnPanelCommand exécutée pour {Ref_Loan}");
                    ReturnPanelRequested?.Invoke(this, _loan);
                },
                () => IsActive
            );

            EditReturnDateCommand = new RelayCommand(
                () => StartEditingReturnDate(),
                () => IsActive
            );

            SaveReturnDateCommand = new RelayCommand(
                 () => SaveReturnDate(),
                () => CanSaveReturnDate()
               
            );

            CancelEditReturnDateCommand = new RelayCommand(() => CancelEditingReturnDate());

            LogInfo($"LoanDetailViewModel créé pour {Ref_Loan}, IsActive={IsActive}");
        }
        #endregion

        #region Méthodes de modification de date
        private void StartEditingReturnDate()
        {
            _context = new LibraryDbContext();
            IsEditingReturnDate = true;
            NewReturnDate = ReturnDate;
            ReturnDateErrorMessage = null;
            LogInfo($"Édition de la date de retour commencée pour {Ref_Loan}");
        }

        private void CancelEditingReturnDate()
        {
            IsEditingReturnDate = false;
            NewReturnDate = ReturnDate;
            ReturnDateErrorMessage = null;
            LogInfo($"Édition de la date de retour annulée pour {Ref_Loan}");
        }

        private bool CanSaveReturnDate()
        {
            if (!IsActive || !IsEditingReturnDate)
                return false;

            // La nouvelle date doit être différente
            if (NewReturnDate.Date == ReturnDate.Date)
                return false;

            // La nouvelle date doit être dans le futur
            if (NewReturnDate.Date <= DateTime.Now.Date)
                return false;

            return true;
        }

        private void UpdateCanSaveReturnDate()
        {
            (SaveReturnDateCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// ✅ MÉTHODE CORRIGÉE: Sauvegarde de la date de retour
        /// </summary>
        private void SaveReturnDate()
        {
            LibraryDbContext context = null;

            try
            {
                LogInfo($"💾 Tentative de modification de la date de retour pour {Ref_Loan}");
                LogInfo($"   Ancienne date: {ReturnDate:dd/MM/yyyy}");
                LogInfo($"   Nouvelle date: {NewReturnDate:dd/MM/yyyy}");

                // ✅ Créer un nouveau contexte propre
                context = new LibraryDbContext();

                // ✅ Recharger le prêt avec ses dépendances
                var loanInDb = context.Loans
                    .Include(l => l.Book)
                    .Include(l => l.Subscriber)
                    .FirstOrDefault(l => l.LoanId == _loan.LoanId);

                if (loanInDb == null)
                {
                    ShowError("Le prêt n'existe plus dans la base de données.");
                    return;
                }

                if (!loanInDb.IsActive)
                {
                    ShowError("Ce prêt a déjà été retourné.");
                    IsEditingReturnDate = false;
                    return;
                }

                // ✅ Recharger le staff connecté
                var staffInDb = context.StaffMembers.Find(_currentStaff.Id_User);
                if (staffInDb == null)
                {
                    ShowError("Membre du personnel introuvable.");
                    return;
                }

                // ✅ Créer la modification via le staff rechargé
                var modification = staffInDb.ChangeReturnDate(
                    loanInDb,
                    NewReturnDate,
                    out string errorMessage
                );

                if (modification == null)
                {
                    ReturnDateErrorMessage = errorMessage;
                    ShowError(errorMessage);
                    return;
                }

                // ✅ Ajouter la modification
                context.Modifications.Add(modification);

                // ✅ Sauvegarder avec génération de référence
                context.SaveChangesWithReferences();

                // ✅ Mettre à jour le modèle local
                _loan.ReturnDate = NewReturnDate;
                ReturnDate = NewReturnDate;
                IsEditingReturnDate = false;
                ReturnDateErrorMessage = null;

                // Notifier les changements
                OnPropertyChanged(nameof(ReturnDate));
                OnPropertyChanged(nameof(IsLate));
                OnPropertyChanged(nameof(DaysLate));
                OnPropertyChanged(nameof(StatusColor));

                LoanModified?.Invoke(this, EventArgs.Empty);

                ShowSuccess($"✅ Date de retour modifiée avec succès!\n\n" +
                           $"Nouvelle date: {NewReturnDate:dd MMMM yyyy}\n" +
                           $"Référence modification: {modification.Ref_Modification}");

                LogInfo($"✅ Date de retour modifiée avec succès pour {Ref_Loan}");
            }
            catch (DbUpdateException dbEx)
            {
                LogError("Erreur base de données lors de la modification", dbEx);
                var innerMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                ShowError($"Erreur lors de la sauvegarde:\n{innerMessage}");
            }
            catch (Exception ex)
            {
                LogError("Erreur lors de la modification de la date", ex);
                ShowError($"Erreur inattendue:\n{ex.Message}");
            }
            finally
            {
                // ✅ IMPORTANT: Dispose du contexte
                context?.Dispose();
            }
        }

        #endregion

        #region Méthodes utilitaires
        private void ShowSuccess(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(message, "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            });
        }

        private void ShowError(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            });
        }

        private void LogInfo(string message)
        {
            System.Diagnostics.Debug.WriteLine($"[INFO] [LoanDetailVM] {message}");
        }

        private void LogError(string message, Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ERROR] [LoanDetailVM] {message}: {ex.Message}");
            if (ex.StackTrace != null)
            {
                System.Diagnostics.Debug.WriteLine($"  StackTrace: {ex.StackTrace}");
            }
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
    /// <summary>
    /// Simple RelayCommand pour les commandes
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;

        public void Execute(object parameter) => _execute();

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }


    /// <summary>
    /// ViewModel pour le panneau de retour de livre avec confirmation
    /// </summary>
    public class LoanReturnViewModel : INotifyPropertyChanged
    {
        #region Services
        private LibraryDbContext _context;
        private readonly StaffMember _currentStaff;
        #endregion

        #region Propriétés du modèle
        private Loan _loan;

        public string Ref_Loan => _loan.Ref_Loan;
        public Book Book => _loan.Book;
        public Subscriber Subscriber => _loan.Subscriber;
        public DateTime BorrowDate => _loan.BorrowDate;
        public DateTime ReturnDate => _loan.ReturnDate;
        public bool IsLate => _loan.IsLate;
        public int DaysLate => _loan.DaysLate;
        public decimal? Penalty => _loan.Penalty;
        #endregion

        #region Propriétés pour le formulaire
        private string _returnNotes = string.Empty;
        public string ReturnNotes
        {
            get => _returnNotes;
            set
            {
                _returnNotes = value;
                OnPropertyChanged();
            }
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
        #endregion

        #region Propriétés calculées
        public Loan SelectedLoan => _loan;

        public string BookTitle => Book?.Title ?? "Inconnu";
        public string SubscriberFullName => Subscriber?.Name_User ?? "Inconnu";
        public string ReturnDateFormatted => $"Date prévue : {ReturnDate:dd MMM yyyy}";
        public string DaysLateText => $"Jours de retard : {DaysLate}";
        public string PenaltyText => Penalty.HasValue ? $"Pénalité : {Penalty:C2}" : "Pénalité : Aucune";

        public Brush LateColor => IsLate
            ? new SolidColorBrush(Color.FromRgb(244, 67, 54))
            : new SolidColorBrush(Color.FromRgb(117, 117, 117));

        public Brush PenaltyColor => Penalty.HasValue && Penalty > 0
            ? new SolidColorBrush(Color.FromRgb(244, 67, 54))
            : new SolidColorBrush(Color.FromRgb(117, 117, 117));

        // Propriétés de récapitulatif pour confirmation
        public string ConfirmationSummary
        {
            get
            {
                var summary = $"📚 Livre : {BookTitle}\n";
                summary += $"👤 Abonné : {SubscriberFullName}\n";
                summary += $"📅 Date d'emprunt : {BorrowDate:dd/MM/yyyy}\n";
                summary += $"📅 Date de retour prévue : {ReturnDate:dd/MM/yyyy}\n\n";

                if (IsLate)
                {
                    summary += $"⚠️ Retard : {DaysLate} jour(s)\n";
                    summary += $"💰 Pénalité : {Penalty:C2}\n";
                    summary += $"📉 Impact fidélité : -{DaysLate * 0.1m:F2} points\n\n";
                }
                else
                {
                    summary += $"✅ Retour à temps\n";
                    summary += $"⭐ Bonus fidélité : +0.20 points\n\n";
                }

                if (!string.IsNullOrWhiteSpace(ReturnNotes))
                {
                    summary += $"📝 Notes :\n{ReturnNotes}";
                }

                return summary;
            }
        }
        #endregion

        #region Commandes
        public ICommand ConfirmReturnCommand { get; }
        public ICommand CloseCommand { get; }
        #endregion

        #region Events
        public event EventHandler CloseRequested;
        public event EventHandler<(Loan loan, string notes)> ReturnConfirmed;
        #endregion

        #region Constructeur
        public LoanReturnViewModel(Loan loan)
        {
            _loan = loan ?? throw new ArgumentNullException(nameof(loan));
            _currentStaff = App._currentconnected_User;

            if (_currentStaff == null)
                throw new InvalidOperationException("Aucun membre du personnel connecté");

            ConfirmReturnCommand = new RelayCommand(

                 () =>  ConfirmReturn(),
                () => _loan.IsActive && !IsProcessing
            );

            CloseCommand = new RelayCommand(() =>
            {
                LogInfo("Fermeture du panneau de retour demandée");
                CloseRequested?.Invoke(this, EventArgs.Empty);
            });

            LogInfo($"LoanReturnVM créé pour {Ref_Loan}");
        }
        #endregion

        #region Méthodes principales
        // ConfirmReturn dans LoanReturnViewModel
        /// <summary>
        /// ✅ MÉTHODE CORRIGÉE: Confirmation du retour avec gestion correcte du contexte EF
        /// </summary>
        private void ConfirmReturn()
        {
            LibraryDbContext context = null;

            try
            {
                var confirmResult = ShowConfirmationDialog();
                if (confirmResult != MessageBoxResult.Yes)
                {
                    LogInfo("Retour annulé par l'utilisateur");
                    return;
                }

                IsProcessing = true;
                LogInfo($"🔄 Traitement du retour pour {Ref_Loan}...");

                // ✅ CORRECTION: Créer un NOUVEAU contexte propre
                context = new LibraryDbContext();

                // ✅ Recharger le prêt avec TOUTES ses dépendances
                var loanInDb = context.Loans
                    .Include(l => l.Book)
                    .Include(l => l.Subscriber)
                    .FirstOrDefault(l => l.LoanId == _loan.LoanId);

                if (loanInDb == null)
                {
                    ShowError("Le prêt n'existe plus dans la base de données.");
                    return;
                }

                if (!loanInDb.IsActive)
                {
                    ShowError("Ce prêt a déjà été retourné.");
                    CloseRequested?.Invoke(this, EventArgs.Empty);
                    return;
                }

                // ✅ Recharger le staff connecté dans CE contexte
                var staffInDb = context.StaffMembers.Find(_currentStaff.Id_User);
                if (staffInDb == null)
                {
                    ShowError("Membre du personnel introuvable.");
                    return;
                }

                // ✅ Créer la modification via le staff rechargé
                var modification = staffInDb.ReturnLoan(loanInDb, out string errorMessage);

                if (modification == null)
                {
                    ShowError($"Erreur lors du retour:\n{errorMessage}");
                    return;
                }

                // Ajouter les notes
                if (!string.IsNullOrWhiteSpace(ReturnNotes))
                {
                    modification.Description += $"\n\nNotes: {ReturnNotes}";
                }

                // ✅ CORRECTION: Augmenter la quantité du livre
                if (loanInDb.Book != null)
                {
                    loanInDb.Book.IncreaseQuantity();
                }

                // ✅ Ajouter la modification AU contexte
                context.Modifications.Add(modification);

                // ✅ SAUVEGARDER avec génération de références
                context.SaveChangesWithReferences();

                // ✅ Mettre à jour le modèle local APRÈS sauvegarde réussie
                _loan.IsActive = false;
                _loan.ActualReturnDate = DateTime.Now;

                ShowSuccessMessage(loanInDb, modification);
                ReturnConfirmed?.Invoke(this, (_loan, ReturnNotes));

                LogInfo($"✅ Retour traité avec succès pour {Ref_Loan}");
            }
            catch (DbUpdateException dbEx)
            {
                LogError("Erreur base de données lors du retour", dbEx);

                // ✅ Message d'erreur plus détaillé
                var innerMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                ShowError($"Erreur lors de la sauvegarde:\n{innerMessage}\n\nDétails: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                LogError("Erreur lors du traitement du retour", ex);
                ShowError($"Erreur inattendue:\n{ex.Message}");
            }
            finally
            {
                IsProcessing = false;

                // ✅ IMPORTANT: Dispose du contexte
                context?.Dispose();
            }
        }

        private MessageBoxResult ShowConfirmationDialog()
        {
            return Application.Current.Dispatcher.Invoke(() =>
            {
                var message = "⚠️ CONFIRMATION DE RETOUR\n\n" + ConfirmationSummary + "\n\n";
                message += "Confirmez-vous le retour de ce livre ?";

                return MessageBox.Show(
                    message,
                    "Confirmer le retour",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question,
                    MessageBoxResult.No
                );
            });
        }

        private void ShowSuccessMessage(Loan loan, Modification modification)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var message = $"✅ RETOUR ENREGISTRÉ AVEC SUCCÈS\n\n";
                message += $"📚 Livre : {loan.Book?.Title}\n";
                message += $"👤 Abonné : {loan.Subscriber?.Name_User}\n";
                message += $"📋 Référence : {loan.Ref_Loan}\n\n";

                if (loan.IsLate)
                {
                    message += $"💰 Pénalité appliquée : {loan.Penalty:C2}\n";
                    message += $"📉 Fidélité réduite de {loan.DaysLate * 0.1m:F2} points\n";
                }
                else
                {
                    message += $"⭐ Retour à temps - Bonus de fidélité accordé!\n";
                }

                message += $"\n📄 Référence modification : {modification.Ref_Modification}\n";
                message += $"👨‍💼 Traité par : {_currentStaff.Name_User}";

                MessageBox.Show(
                    message,
                    "Retour confirmé",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            });
        }

        private void ShowError(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            });
        }

        private void LogInfo(string message)
        {
            System.Diagnostics.Debug.WriteLine($"[INFO] [LoanReturnVM] {message}");
        }

        private void LogError(string message, Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ERROR] [LoanReturnVM] {message}: {ex.Message}");
            if (ex.StackTrace != null)
            {
                System.Diagnostics.Debug.WriteLine($"  StackTrace: {ex.StackTrace}");
            }
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}