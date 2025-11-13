using System.Windows.Media;
using LIBBRARY_MANAGER.Model;
using LIBBRARY_MANAGER.UI.Modules.HomeSpace.Items.BasicListItems.Views;

namespace LIBBRARY_MANAGER.UI.Modules.HomeSpace.Items.BasicListItems.ViewModel
{
    public class GenericItemViewModel
    {
        // Identifiant unique pour la synchronisation
        public string UniqueId { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string OperationType { get; set; } = string.Empty; // "Loan" ou "Modification"

        // Données principales
        public string Title { get; set; } = string.Empty;
        public string Reference { get; set; } = string.Empty;
        public string BookInfo { get; set; } = string.Empty;
        public string UserInfo { get; set; } = string.Empty;
        public string DateInfo { get; set; } = string.Empty;
        public string StatusInfo { get; set; } = string.Empty;

        // Styles visuels
        public Brush TitleColor { get; set; } = Brushes.Black;
        public Brush StatusColor { get; set; } = Brushes.Gray;
        public Brush AccentColor { get; set; } = new SolidColorBrush(Color.FromRgb(3, 112, 195));
        public Brush BackgroundColor { get; set; } = Brushes.White;
        public Brush BorderColor { get; set; } = new SolidColorBrush(Color.FromRgb(220, 220, 220));

        // Icône
        public string IconKind { get; set; } = "Information";
        public Brush IconColor { get; set; } = new SolidColorBrush(Color.FromRgb(3, 112, 195));

        // ============================================
        // CONSTRUCTION À PARTIR D'UN LOAN
        // ============================================
        public static GenericItemViewModel FromLoan(Loan loan)
        {
            if (loan == null)
                throw new ArgumentNullException(nameof(loan));

            var vm = new GenericItemViewModel
            {
                UniqueId = $"LOAN-{loan.LoanId}",
                Timestamp = loan.BorrowDate,
                OperationType = "Loan"
            };

            // Couleurs selon l'état
            Brush activeGreen = new SolidColorBrush(Color.FromRgb(46, 204, 113));
            Brush warningOrange = new SolidColorBrush(Color.FromRgb(243, 156, 18));
            Brush errorRed = new SolidColorBrush(Color.FromRgb(231, 76, 60));
            Brush completedGray = new SolidColorBrush(Color.FromRgb(149, 165, 166));

            if (loan.IsActive)
            {
                if (loan.IsLate)
                {
                    vm.Title = $"⚠️ EMPRUNT EN RETARD";
                    vm.TitleColor = errorRed;
                    vm.AccentColor = errorRed;
                    vm.BorderColor = errorRed;
                    vm.IconKind = "AlertCircle";
                    vm.IconColor = errorRed;
                    vm.StatusInfo = $"En retard de {loan.DaysLate} jour{(loan.DaysLate > 1 ? "s" : "")}";
                    vm.StatusColor = errorRed;
                }
                else
                {
                    vm.Title = "📗 EMPRUNT EN COURS";
                    vm.TitleColor = activeGreen;
                    vm.AccentColor = activeGreen;
                    vm.BorderColor = activeGreen;
                    vm.IconKind = "BookCheck";
                    vm.IconColor = activeGreen;

                    int daysLeft = (loan.ReturnDate - DateTime.Now).Days;
                    if (daysLeft <= 3)
                    {
                        vm.StatusInfo = $"⏰ À rendre dans {daysLeft} jour{(daysLeft > 1 ? "s" : "")}";
                        vm.StatusColor = warningOrange;
                    }
                    else
                    {
                        vm.StatusInfo = $"✓ Retour prévu le {loan.ReturnDate:dd/MM/yyyy}";
                        vm.StatusColor = activeGreen;
                    }
                }
            }
            else
            {
                vm.Title = "✅ EMPRUNT RETOURNÉ";
                vm.TitleColor = completedGray;
                vm.AccentColor = completedGray;
                vm.BorderColor = completedGray;
                vm.IconKind = "CheckCircle";
                vm.IconColor = completedGray;

                if (loan.ActualReturnDate.HasValue)
                {
                    bool onTime = loan.ActualReturnDate.Value <= loan.ReturnDate;
                    vm.StatusInfo = onTime
                        ? "Retourné à temps"
                        : $"Retourné avec {(loan.ActualReturnDate.Value - loan.ReturnDate).Days}j de retard";
                    vm.StatusColor = onTime ? activeGreen : warningOrange;
                }
            }

            // Informations principales
            vm.Reference = $"Réf: {loan.Ref_Loan}";
            vm.BookInfo = $"📚 {loan.Book?.Title ?? "Livre inconnu"}";
            vm.UserInfo = $"👤 {loan.Subscriber?.Name_User ?? "Abonné inconnu"}";
            vm.DateInfo = $"📅 Emprunté le {loan.BorrowDate:dd/MM/yyyy à HH:mm}";

            // Pénalité si applicable
            if (loan.Penalty.HasValue && loan.Penalty.Value > 0)
            {
                vm.DateInfo += $" • Pénalité: {loan.Penalty.Value:C}";
            }

            return vm;
        }

        // ============================================
        // CONSTRUCTION À PARTIR D'UNE MODIFICATION
        // ============================================
        public static GenericItemViewModel FromModification(Modification mod)
        {
            if (mod == null)
                throw new ArgumentNullException(nameof(mod));

            var vm = new GenericItemViewModel
            {
                UniqueId = $"MOD-{mod.ModificationId}",
                Timestamp = mod.ModificationDate,
                OperationType = "Modification"
            };

            // Couleurs thématiques
            Brush blueInfo = new SolidColorBrush(Color.FromRgb(52, 152, 219));
            Brush greenSuccess = new SolidColorBrush(Color.FromRgb(46, 204, 113));
            Brush purpleAction = new SolidColorBrush(Color.FromRgb(155, 89, 182));

            switch (mod.Type)
            {
                case ModificationType.ChangeReturnDate:
                    vm.Title = "🔄 CHANGEMENT DE DATE";
                    vm.TitleColor = blueInfo;
                    vm.AccentColor = blueInfo;
                    vm.BorderColor = blueInfo;
                    vm.IconKind = "CalendarEdit";
                    vm.IconColor = blueInfo;

                    vm.StatusInfo = "Date de retour modifiée";
                    vm.StatusColor = blueInfo;

                    if (mod.OldReturnDate.HasValue && mod.NewReturnDate.HasValue)
                    {
                        vm.DateInfo = $"📅 {mod.OldReturnDate.Value:dd/MM/yyyy} → {mod.NewReturnDate.Value:dd/MM/yyyy}";
                    }
                    break;

                case ModificationType.ReturnLoan:
                    vm.Title = "✅ RETOUR EFFECTUÉ";
                    vm.TitleColor = greenSuccess;
                    vm.AccentColor = greenSuccess;
                    vm.BorderColor = greenSuccess;
                    vm.IconKind = "BookArrowLeft";
                    vm.IconColor = greenSuccess;

                    vm.StatusInfo = "Livre retourné avec succès";
                    vm.StatusColor = greenSuccess;
                    vm.DateInfo = $"📅 Le {mod.ModificationDate:dd/MM/yyyy à HH:mm}";
                    break;

                case ModificationType.CancelLoan:
                    vm.Title = "❌ ANNULATION D'EMPRUNT";
                    vm.TitleColor = new SolidColorBrush(Color.FromRgb(231, 76, 60));
                    vm.AccentColor = vm.TitleColor;
                    vm.BorderColor = vm.TitleColor;
                    vm.IconKind = "Cancel";
                    vm.IconColor = vm.TitleColor;

                    vm.StatusInfo = "Emprunt annulé";
                    vm.StatusColor = vm.TitleColor;
                    vm.DateInfo = $"📅 Le {mod.ModificationDate:dd/MM/yyyy à HH:mm}";
                    break;

                default:
                    vm.Title = "⚙️ MODIFICATION";
                    vm.TitleColor = purpleAction;
                    vm.AccentColor = purpleAction;
                    vm.BorderColor = purpleAction;
                    vm.IconKind = "Cog";
                    vm.IconColor = purpleAction;

                    vm.StatusInfo = mod.Type.ToString();
                    vm.StatusColor = purpleAction;
                    vm.DateInfo = $"📅 Le {mod.ModificationDate:dd/MM/yyyy à HH:mm}";
                    break;
            }

            vm.Reference = $"Réf: {mod.Ref_Modification}";

            // Informations du prêt associé
            if (mod.Loan_ != null)
            {
                vm.BookInfo = $"📚 {mod.Loan_.Book?.Title ?? "Livre inconnu"}";
                vm.UserInfo = $"👤 {mod.Loan_.Subscriber?.Name_User ?? "Abonné inconnu"}";
            }

            // Staff member qui a fait la modification
            if (mod.StaffMember_ != null)
            {
                vm.UserInfo += $" • Par: {mod.StaffMember_.Name_User}";
            }

            return vm;
        }

        // ============================================
        // CRÉATION DES VUES
        // ============================================
        public static BasicListItemView CreateLoanItem(Loan loan)
        {
            var vm = FromLoan(loan);
            return new BasicListItemView { DataContext = vm };
        }

        public static BasicListItemView CreateModificationItem(Modification mod)
        {
            var vm = FromModification(mod);
            return new BasicListItemView { DataContext = vm };
        }
    }
}
