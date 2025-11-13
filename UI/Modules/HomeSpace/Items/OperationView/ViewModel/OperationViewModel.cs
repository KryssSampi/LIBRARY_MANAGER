using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using LIBBRARY_MANAGER.Model;

namespace LIBBRARY_MANAGER.UI.Modules.HomeSpace.ViewModel
{
    public partial class OperationsViewModel : ObservableObject
    {
        private readonly StaffMember _currentStaff;

        [ObservableProperty]
        private ObservableCollection<OperationItemViewModel> todayLoans = new();

        [ObservableProperty]
        private ObservableCollection<OperationItemViewModel> todayModifications = new();

        [ObservableProperty]
        private int loansCount = 0;

        [ObservableProperty]
        private int modificationsCount = 0;

        public OperationsViewModel(StaffMember currentStaff)
        {
            _currentStaff = currentStaff ?? throw new ArgumentNullException(nameof(currentStaff));
            LoadTodayOperations();
        }

        /// <summary>
        /// Charge les opérations du jour de l'utilisateur actuel
        /// </summary>
        public void LoadTodayOperations()
        {
            try
            {
                var today = DateTime.Now.Date;
                var staffId = _currentStaff.Id_User;

                // ✅ Charger les emprunts du jour
                var loans = App.LibraryDbContext.Loans
                    .Include(l => l.Book)
                    .Include(l => l.Subscriber)
                    .Where(l => l.BorrowDate.Date == today)
                    .AsNoTracking()
                    .OrderByDescending(l => l.BorrowDate)
                    .ToList();

                // ✅ Charger les modifications du jour de cet utilisateur
                var modifications = App.LibraryDbContext.Modifications
                    .Include(m => m.Loan_)
                        .ThenInclude(l => l.Book)
                    .Include(m => m.Loan_)
                        .ThenInclude(l => l.Subscriber)
                    .Include(m => m.StaffMember_)
                    .Where(m => m.ModificationDate.Date == today && m.StaffMemberId == staffId)
                    .AsNoTracking()
                    .OrderByDescending(m => m.ModificationDate)
                    .ToList();

                // ✅ Convertir en ViewModels
                TodayLoans = new ObservableCollection<OperationItemViewModel>(
                    loans.Select(OperationItemViewModel.FromLoan)
                );

                TodayModifications = new ObservableCollection<OperationItemViewModel>(
                    modifications.Select(OperationItemViewModel.FromModification)
                );

                LoansCount = TodayLoans.Count;
                ModificationsCount = TodayModifications.Count;

                Console.WriteLine($"✅ Opérations chargées: {LoansCount} emprunts, {ModificationsCount} modifications");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur LoadTodayOperations: {ex.Message}");
            }
        }

        /// <summary>
        /// Rafraîchir les données (appelé manuellement si besoin)
        /// </summary>
        public void Refresh()
        {
            LoadTodayOperations();
        }
    }

    // ============================================
    // ViewModel pour un item d'opération
    // ============================================
    public class OperationItemViewModel
    {
        public string OperationTitle { get; set; } = string.Empty;
        public string Reference { get; set; } = string.Empty;
        public string BookTitle { get; set; } = string.Empty;
        public string SubscriberName { get; set; } = string.Empty;
        public DateTime OperationDate { get; set; }
        public string StatusText { get; set; } = string.Empty;

        // Styles visuels
        public Brush IconBackground { get; set; } = Brushes.LightBlue;
        public string IconKind { get; set; } = "Information";
        public Brush IconColor { get; set; } = Brushes.White;
        public Brush TitleColor { get; set; } = Brushes.Black;
        public Brush StatusBackground { get; set; } = Brushes.LightGray;
        public Brush StatusForeground { get; set; } = Brushes.White;

        /// <summary>
        /// Créer un ViewModel depuis un Loan
        /// </summary>
        public static OperationItemViewModel FromLoan(Loan loan)
        {
            var vm = new OperationItemViewModel
            {
                Reference = loan.Ref_Loan,
                BookTitle = loan.Book?.Title ?? "Livre inconnu",
                SubscriberName = loan.Subscriber?.Name_User ?? "Abonné inconnu",
                OperationDate = loan.BorrowDate
            };

            // ✅ Couleurs selon l'état
            if (loan.IsActive)
            {
                if (loan.IsLate)
                {
                    vm.OperationTitle = "EMPRUNT EN RETARD";
                    vm.StatusText = $"{loan.DaysLate}j retard";
                    vm.IconBackground = new SolidColorBrush(Color.FromRgb(231, 76, 60));
                    vm.IconKind = "AlertCircle";
                    vm.IconColor = Brushes.White;
                    vm.TitleColor = new SolidColorBrush(Color.FromRgb(231, 76, 60));
                    vm.StatusBackground = new SolidColorBrush(Color.FromRgb(231, 76, 60));
                    vm.StatusForeground = Brushes.White;
                }
                else
                {
                    vm.OperationTitle = "EMPRUNT EN COURS";
                    int daysLeft = (loan.ReturnDate - DateTime.Now).Days;
                    vm.StatusText = daysLeft <= 3 ? $"{daysLeft}j restant" : "En cours";
                    vm.IconBackground = new SolidColorBrush(Color.FromRgb(46, 204, 113));
                    vm.IconKind = "BookCheck";
                    vm.IconColor = Brushes.White;
                    vm.TitleColor = new SolidColorBrush(Color.FromRgb(46, 204, 113));
                    vm.StatusBackground = daysLeft <= 3
                        ? new SolidColorBrush(Color.FromRgb(243, 156, 18))
                        : new SolidColorBrush(Color.FromRgb(46, 204, 113));
                    vm.StatusForeground = Brushes.White;
                }
            }
            else
            {
                vm.OperationTitle = "EMPRUNT RETOURNÉ";
                vm.StatusText = "Terminé";
                vm.IconBackground = new SolidColorBrush(Color.FromRgb(149, 165, 166));
                vm.IconKind = "CheckCircle";
                vm.IconColor = Brushes.White;
                vm.TitleColor = new SolidColorBrush(Color.FromRgb(149, 165, 166));
                vm.StatusBackground = new SolidColorBrush(Color.FromRgb(149, 165, 166));
                vm.StatusForeground = Brushes.White;
            }

            return vm;
        }

        /// <summary>
        /// Créer un ViewModel depuis une Modification
        /// </summary>
        public static OperationItemViewModel FromModification(Modification mod)
        {
            var vm = new OperationItemViewModel
            {
                Reference = mod.Ref_Modification,
                BookTitle = mod.Loan_?.Book?.Title ?? "Livre inconnu",
                SubscriberName = mod.Loan_?.Subscriber?.Name_User ?? "Abonné inconnu",
                OperationDate = mod.ModificationDate
            };

            switch (mod.Type)
            {
                case ModificationType.ChangeReturnDate:
                    vm.OperationTitle = "PROLONGATION";
                    vm.StatusText = "Date modifiée";
                    vm.IconBackground = new SolidColorBrush(Color.FromRgb(52, 152, 219));
                    vm.IconKind = "CalendarEdit";
                    vm.IconColor = Brushes.White;
                    vm.TitleColor = new SolidColorBrush(Color.FromRgb(52, 152, 219));
                    vm.StatusBackground = new SolidColorBrush(Color.FromRgb(52, 152, 219));
                    vm.StatusForeground = Brushes.White;
                    break;

                case ModificationType.ReturnLoan:
                    vm.OperationTitle = "RETOUR EFFECTUÉ";
                    vm.StatusText = "Validé";
                    vm.IconBackground = new SolidColorBrush(Color.FromRgb(46, 204, 113));
                    vm.IconKind = "BookArrowLeft";
                    vm.IconColor = Brushes.White;
                    vm.TitleColor = new SolidColorBrush(Color.FromRgb(46, 204, 113));
                    vm.StatusBackground = new SolidColorBrush(Color.FromRgb(46, 204, 113));
                    vm.StatusForeground = Brushes.White;
                    break;

                case ModificationType.CancelLoan:
                    vm.OperationTitle = "ANNULATION";
                    vm.StatusText = "Annulé";
                    vm.IconBackground = new SolidColorBrush(Color.FromRgb(231, 76, 60));
                    vm.IconKind = "Cancel";
                    vm.IconColor = Brushes.White;
                    vm.TitleColor = new SolidColorBrush(Color.FromRgb(231, 76, 60));
                    vm.StatusBackground = new SolidColorBrush(Color.FromRgb(231, 76, 60));
                    vm.StatusForeground = Brushes.White;
                    break;

                default:
                    vm.OperationTitle = "MODIFICATION";
                    vm.StatusText = "Modifié";
                    vm.IconBackground = new SolidColorBrush(Color.FromRgb(155, 89, 182));
                    vm.IconKind = "Cog";
                    vm.IconColor = Brushes.White;
                    vm.TitleColor = new SolidColorBrush(Color.FromRgb(155, 89, 182));
                    vm.StatusBackground = new SolidColorBrush(Color.FromRgb(155, 89, 182));
                    vm.StatusForeground = Brushes.White;
                    break;
            }

            return vm;
        }
    }
}