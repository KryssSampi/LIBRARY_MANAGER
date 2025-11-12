using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using LIBBRARY_MANAGER.Model;
using LIBBRARY_MANAGER.Views.SubscriberView;

namespace LIBBRARY_MANAGER.ViewModel.SubscribersViewModel
{
    /// <summary>
    /// ViewModel pour l'affichage d'un abonné dans la liste
    /// </summary>
    public class SubscriberItemViewModel : INotifyPropertyChanged
    {
        #region Propriétés du modèle

        private readonly Subscriber _subscriber;

        /// <summary>
        /// Instance du modèle Subscriber
        /// </summary>
        public Subscriber SubscriberInstance => _subscriber;

        /// <summary>
        /// Référence de l'abonné
        /// </summary>
        public string Ref_Subscriber => _subscriber.Ref_Subscriber;

        /// <summary>
        /// Nom complet de l'abonné
        /// </summary>
        public string FullName => _subscriber.Name_User;

        /// <summary>
        /// Adresse email
        /// </summary>
        public string Email => _subscriber.Adresse_Mail;

        /// <summary>
        /// Score de fidélité
        /// </summary>
        public decimal Fidelity => _subscriber.Fidelity;

        /// <summary>
        /// Couleur de la fidélité selon le score
        /// </summary>
        public Brush FidelityColor => GetFidelityColor(_subscriber.Fidelity);

        /// <summary>
        /// Nombre d'emprunts actifs
        /// </summary>
        public int ActiveLoansCount => _subscriber.ActiveLoansCount;

        /// <summary>
        /// Indique si l'abonné peut emprunter
        /// </summary>
        public bool CanBorrow => _subscriber.CanBorrow;

        /// <summary>
        /// Date de création du compte
        /// </summary>
        public DateTime Date_Creation => _subscriber.Date_Creation;

        #endregion

        #region Constructeur

        public SubscriberItemViewModel(Subscriber subscriber)
        {
            _subscriber = subscriber ?? throw new ArgumentNullException(nameof(subscriber));

            System.Diagnostics.Debug.WriteLine(
                $"[SubscriberItemViewModel] Créé pour {FullName}"
            );
        }

        #endregion

        #region Méthodes utilitaires

        /// <summary>
        /// Retourne la couleur selon le score de fidélité
        /// </summary>
        private Brush GetFidelityColor(decimal fidelity)
        {
            if (fidelity >= 8m)
                return new SolidColorBrush(Color.FromRgb(76, 175, 80)); // Vert
            if (fidelity >= 5m)
                return new SolidColorBrush(Color.FromRgb(255, 152, 0)); // Orange
            return new SolidColorBrush(Color.FromRgb(244, 67, 54)); // Rouge
        }

        /// <summary>
        /// Retourne l'instance du modèle Subscriber
        /// </summary>
        public Subscriber GetSubscriber() => _subscriber;

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
            OnPropertyChanged(nameof(FullName));
            OnPropertyChanged(nameof(Email));
            OnPropertyChanged(nameof(Fidelity));
            OnPropertyChanged(nameof(FidelityColor));
            OnPropertyChanged(nameof(ActiveLoansCount));
            OnPropertyChanged(nameof(CanBorrow));
        }

        #endregion
    }
}

// ============================================================
// ✅ VIEWMODEL DU PANNEAU DE DÉTAIL
// ============================================================
public class SubscriberDetailViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public string Name_User { get; }
        public string Ref_Subscriber { get; }
        public string Adresse_Mail { get; }
        public string Num_Telephone { get; }
        public string Adresse { get; }
        public DateTime BirthDate { get; }
        public int Age { get; }
    private decimal _fidelity;
    public decimal Fidelity
    {
        get { return _fidelity; }
        set
        {
            if (_fidelity != value)
            {
                _fidelity = value;
                OnPropertyChanged(nameof(Fidelity));
            }
        }
    }

    public int ActiveLoansCount { get; }

        public Brush ActiveLoansColor => ActiveLoansCount > 0 ? Brushes.Green : Brushes.Gray;

        public ICommand CloseCommand { get; set; }
        public ICommand ViewLoansCommand { get; }

        public SubscriberDetailViewModel(Subscriber sub)
        {
            Name_User = sub.Name_User;
            Ref_Subscriber = sub.Ref_Subscriber;
            Adresse_Mail = sub.Adresse_Mail;
            Num_Telephone = sub.Num_Telephone;
            Adresse = sub.Adresse;
            Fidelity = sub.Fidelity;
            ActiveLoansCount = sub.ActiveLoansCount;

            CloseCommand = new RelayCommand(_ => SubscriberDetailDisplayService.Close());
            ViewLoansCommand = new RelayCommand(_ => OpenLoansList());
        }

        private void OpenLoansList()
        {
            // Exemple : tu pourras remplacer par ta propre logique
            System.Diagnostics.Debug.WriteLine($"Affichage des emprunts de {Name_User}");
        }

        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    // ============================================================
    // ✅ RELAYCOMMAND UTILITAIRE
    // ============================================================
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Func<object?, bool>? _canExecute;

        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = _ => execute();
            if (canExecute != null)
                _canExecute = _ => canExecute();
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

        public void Execute(object? parameter) => _execute(parameter);

        public event EventHandler? CanExecuteChanged;
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    // ============================================================
    // ✅ SERVICE D’AFFICHAGE (Mock)
    // ============================================================
    public static class SubscriberDetailDisplayService
    {
        private static ContentControl? _displayTarget;

        public static void RegisterTarget(ContentControl target)
        {
            _displayTarget = target;
        }

        public static void Show(UserControl detailPanel)
        {
            if (_displayTarget != null)
                _displayTarget.Content = detailPanel;
        }

        public static void Close()
        {
            if (_displayTarget != null)
                _displayTarget.Content = null;
        }
    }

