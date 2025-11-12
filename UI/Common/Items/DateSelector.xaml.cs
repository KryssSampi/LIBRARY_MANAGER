using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LIBBRARY_MANAGER.UI.Common.Items
{
    /// <summary>
    /// Contrôle de sélection de date personnalisé.
    /// Permet la sélection d'année, mois, jour + synchronisation avec un DatePicker.
    /// Déclenche une commande lorsqu'une date valide est changée.
    /// </summary>
    public partial class DateSelector : UserControl
    {
        private DateTime? _lastValidDate;

        public DateSelector()
        {
            InitializeComponent();
            _lastValidDate = SelectedDate;
            this.LayoutUpdated += (_, _) => ValidateAndSyncDate();
        }

        #region Dependency Properties

        // 🔹 Date complète sélectionnée
        public static readonly DependencyProperty SelectedDateProperty =
            DependencyProperty.Register(nameof(SelectedDate), typeof(DateTime?), typeof(DateSelector),
                new FrameworkPropertyMetadata(DateTime.Now, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnDateChanged));

        public DateTime? SelectedDate
        {
            get => (DateTime?)GetValue(SelectedDateProperty);
            set => SetValue(SelectedDateProperty, value);
        }

        // 🔹 Message d’erreur
        public static readonly DependencyProperty ErrorMessageProperty =
            DependencyProperty.Register(nameof(ErrorMessage), typeof(string), typeof(DateSelector));

        public string ErrorMessage
        {
            get => (string)GetValue(ErrorMessageProperty);
            set => SetValue(ErrorMessageProperty, value);
        }

        // 🔹 Statut d’erreur (affichage du message)
        public static readonly DependencyProperty HasErrorProperty =
            DependencyProperty.Register(nameof(HasError), typeof(bool), typeof(DateSelector));

        public bool HasError
        {
            get => (bool)GetValue(HasErrorProperty);
            set => SetValue(HasErrorProperty, value);
        }

        // 🔹 Commande exécutée lors du changement de date
        public static readonly DependencyProperty OnDateChangedCommandProperty =
            DependencyProperty.Register(nameof(OnDateChangedCommand), typeof(ICommand), typeof(DateSelector),
                new PropertyMetadata(null));

        public ICommand OnDateChangedCommand
        {
            get => (ICommand)GetValue(OnDateChangedCommandProperty);
            set => SetValue(OnDateChangedCommandProperty, value);
        }

        #endregion

        #region Sub-properties (Year, Month, Day)

        public int Year
        {
            get => SelectedDate?.Year ?? DateTime.Now.Year;
            set
            {
                if (SelectedDate == null) SelectedDate = DateTime.Now;
                TryUpdateDate(value, Month, Day);
            }
        }

        public int Month
        {
            get => SelectedDate?.Month ?? DateTime.Now.Month;
            set
            {
                if (SelectedDate == null) SelectedDate = DateTime.Now;
                TryUpdateDate(Year, value, Day);
            }
        }

        public int Day
        {
            get => SelectedDate?.Day ?? DateTime.Now.Day;
            set
            {
                if (SelectedDate == null) SelectedDate = DateTime.Now;
                TryUpdateDate(Year, Month, value);
            }
        }

        #endregion

        #region Core Logic

        private static void OnDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DateSelector control)
                control.OnSelectedDateChanged(e.OldValue as DateTime?, e.NewValue as DateTime?);
        }

        private void OnSelectedDateChanged(DateTime? oldValue, DateTime? newValue)
        {
            SyncFromDate();

            if (newValue.HasValue && newValue != oldValue)
            {
                // 🔸 Exécute la commande liée (MVVM)
                if (OnDateChangedCommand != null && OnDateChangedCommand.CanExecute(newValue))
                    OnDateChangedCommand.Execute(newValue);
            }
        }

        private void SyncFromDate()
        {
            if (SelectedDate is DateTime date)
            {
                HasError = false;
                ErrorMessage = string.Empty;
                _lastValidDate = date;
            }
        }

        private void TryUpdateDate(int year, int month, int day)
        {
            try
            {
                var newDate = new DateTime(year, month, day);
                SelectedDate = newDate;
                HasError = false;
                ErrorMessage = string.Empty;
                _lastValidDate = newDate;
            }
            catch
            {
                HasError = true;
                ErrorMessage = "⚠ Date invalide, retour à la dernière valeur valide.";
                if (_lastValidDate.HasValue)
                    SelectedDate = _lastValidDate;
            }
        }

        private void ValidateAndSyncDate()
        {
            if (SelectedDate.HasValue && SelectedDate != _lastValidDate)
            {
                _lastValidDate = SelectedDate;
                HasError = false;
                ErrorMessage = string.Empty;

                // 🔸 Déclenche la commande même en update indirect (via calendrier)
                if (OnDateChangedCommand != null && OnDateChangedCommand.CanExecute(SelectedDate))
                    OnDateChangedCommand.Execute(SelectedDate);
            }
        }

        #endregion
    }
}
