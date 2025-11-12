using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using LIBBRARY_MANAGER.Model;
using LIBBRARY_MANAGER.UI.Modules.HomeSpace.Items.BasicListItems.Views;

namespace LIBBRARY_MANAGER.UI.Modules.HomeSpace.Items.BasicListItems.ViewModel
{
   
        public class GenericItemViewModel
        {
            // --- Champs textuels génériques ---
            public string Info1 { get; set; } = string.Empty;
            public string Info2 { get; set; } = string.Empty;
            public string Info3 { get; set; } = string.Empty;
            public string Info4 { get; set; } = string.Empty;
            public string Info5 { get; set; } = string.Empty;

            // --- Couleurs d’arrière-plan et texte ---
            public Brush Info1Background { get; set; } = Brushes.Transparent;
            public Brush Info2Background { get; set; } = Brushes.Transparent;
            public Brush Info3Background { get; set; } = Brushes.Transparent;
            public Brush Info4Background { get; set; } = Brushes.Transparent;
            public Brush Info5Background { get; set; } = Brushes.Transparent;

            public Brush Info1Foreground { get; set; } = Brushes.Black;
            public Brush Info2Foreground { get; set; } = Brushes.Black;
            public Brush Info3Foreground { get; set; } = Brushes.Black;
            public Brush Info4Foreground { get; set; } = Brushes.Black;
            public Brush Info5Foreground { get; set; } = Brushes.Black;

            public Brush BorderColor { get; set; } = Brushes.Transparent;
            public Brush BackgroundColor { get; set; } = Brushes.Transparent;

            // ============================================================
            //  🧩 Construction à partir d’une entité Modification
            // ============================================================
            public static GenericItemViewModel FromModification(Modification mod)
            {
                if (mod == null)
                    throw new ArgumentNullException(nameof(mod));

                var vm = new GenericItemViewModel
                {
                    BackgroundColor = Brushes.Transparent,
                    BorderColor = new SolidColorBrush(Color.FromRgb(150, 180, 200)) // léger gris-bleu
                };

                // 🎨 Palette pastel
                Brush blue = new SolidColorBrush(Color.FromRgb(170, 200, 255));
                Brush green = new SolidColorBrush(Color.FromRgb(180, 240, 200));
                Brush orange = new SolidColorBrush(Color.FromRgb(255, 220, 170));
                Brush gray = new SolidColorBrush(Color.FromRgb(180, 180, 180));

                switch (mod.Type)
                {
                    case ModificationType.ChangeReturnDate:
                        vm.Info1 = "🕓 Changement de date";
                        vm.Info1Foreground = blue;

                        vm.Info2 = $"Réf : {mod.Ref_Modification}";
                        vm.Info2Foreground = gray;

                        vm.Info3 = $"Ancienne : {mod.OldReturnDate?.ToString("dd/MM/yyyy") ?? "N/A"}";
                        vm.Info3Foreground = orange;

                        vm.Info4 = $"Nouvelle : {mod.NewReturnDate?.ToString("dd/MM/yyyy") ?? "N/A"}";
                        vm.Info4Foreground = green;
                        break;

                    case ModificationType.ReturnLoan:
                        vm.Info1 = "📘 Retour d’emprunt";
                        vm.Info1Foreground = green;

                        vm.Info2 = $"Réf : {mod.Ref_Modification}";
                        vm.Info2Foreground = gray;

                        string subscriber = mod.Loan_?.Subscriber?.Name_User ?? "Abonné inconnu";
                        string book = mod.Loan_?.Book?.Title ?? "Livre inconnu";

                        vm.Info3 = $"Livre : {book}";
                        vm.Info3Foreground = orange;

                        vm.Info4 = $"Par : {subscriber}";
                        vm.Info4Foreground = blue;
                        break;

                    default:
                        vm.Info1 = $"Modification : {mod.Type}";
                        vm.Info1Foreground = gray;
                        break;
                }

                return vm;
        }

        // ============================================================
        //  🧱 Création directe du contrôle WPF (BasicListItemView)
        // ============================================================
        public static BasicListItemView CreateModificationItem(Modification mod)
        {
            var vm = FromModification(mod);

            var view = new BasicListItemView
            {
                DataContext = vm
            };

            return view;
        }
        // --- Construction à partir d’une entité Loan ---
        public static GenericItemViewModel FromLoan(Loan loan)
        {
            if (loan == null)
                throw new ArgumentNullException(nameof(loan));

            var vm = new GenericItemViewModel
            {
                BackgroundColor = Brushes.Transparent,
                BorderColor = new SolidColorBrush(Color.FromRgb(150, 180, 200)) // léger gris-bleu
            };

            // palettes pastel
            Brush pastelBlue = new SolidColorBrush(Color.FromRgb(175, 200, 255));
            Brush pastelGreen = new SolidColorBrush(Color.FromRgb(200, 240, 200));
            Brush pastelOrange = new SolidColorBrush(Color.FromRgb(255, 230, 180));
            Brush pastelRed = new SolidColorBrush(Color.FromRgb(255, 200, 200));
            Brush softGray = new SolidColorBrush(Color.FromRgb(170, 170, 170));
            Brush neutral = new SolidColorBrush(Color.FromRgb(120, 120, 120));

            // Info mapping (Info1..Info5)
            // Info1: statut + icône
            // Info2: référence du prêt
            // Info3: livre
            // Info4: abonné
            // Info5: dates / état / pénalité
            vm.Info1 = loan.IsActive
                ? (loan.IsLate ? $"⚠️ En retard ({loan.DaysLate}j)" : "✅ Actif")
                : "✔️ Terminé";
            vm.Info1Foreground = loan.IsActive
                ? (loan.IsLate ? pastelOrange : pastelGreen)
                : softGray;

            vm.Info2 = $"Réf: {loan.Ref_Loan}";
            vm.Info2Foreground = neutral;

            vm.Info3 = $"Livre: {loan.Book?.Title ?? "—"}";
            vm.Info3Foreground = pastelBlue;

            vm.Info4 = $"Abonné: {loan.Subscriber?.Name_User ?? "—"}";
            vm.Info4Foreground = pastelBlue;

            // construire Info5: dates et pénalité éventuelle
            string borrowStr = loan.BorrowDate.ToString("dd/MM/yyyy");
            string returnStr = loan.ReturnDate.ToString("dd/MM/yyyy");
            string penaltyStr = (loan.Penalty.HasValue && loan.Penalty.Value > 0)
                ? $" · Pénalité: {loan.Penalty.Value:C}"
                : string.Empty;

            vm.Info5 = $"Emprunt: {borrowStr} → Retour: {returnStr}{penaltyStr}";
            vm.Info5Foreground = loan.IsLate ? pastelRed : neutral;

            // individual backgrounds remain transparent (per request),
            // but you can give faint highlight for each info if desired:
            vm.Info1Background = Brushes.Transparent;
            vm.Info2Background = Brushes.Transparent;
            vm.Info3Background = Brushes.Transparent;
            vm.Info4Background = Brushes.Transparent;
            vm.Info5Background = Brushes.Transparent;

            return vm;
        }

        // --- Création directe du contrôle WPF (BasicListItemView) pour un Loan ---
        public static BasicListItemView CreateLoanItem(Loan loan)
        {
            var vm = FromLoan(loan);

            var view = new BasicListItemView
            {
                DataContext = vm
            };

            return view;
        }

    }
}
