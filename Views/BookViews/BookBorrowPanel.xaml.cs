using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LIBBRARY_MANAGER.Data;
using LIBBRARY_MANAGER.Model;
using LIBBRARY_MANAGER.ViewModel.BookViewModels;
using Microsoft.EntityFrameworkCore;

namespace LIBBRARY_MANAGER.Views.BookViews
{
    public partial class BookBorrowPanel : UserControl
    {
        #region Dependency Properties

        public ICommand BorrowCommand
        {
            get => (ICommand)GetValue(BorrowCommandProperty);
            set => SetValue(BorrowCommandProperty, value);
        }

        public static readonly DependencyProperty BorrowCommandProperty =
            DependencyProperty.Register(nameof(BorrowCommand), typeof(ICommand), typeof(BookBorrowPanel));

        public ICommand CloseCommand
        {
            get => (ICommand)GetValue(CloseCommandProperty);
            set => SetValue(CloseCommandProperty, value);
        }

        public static readonly DependencyProperty CloseCommandProperty =
            DependencyProperty.Register(nameof(CloseCommand), typeof(ICommand), typeof(BookBorrowPanel));

        #endregion

        private Book _currentBook;

        public BookBorrowPanel()
        {
            InitializeComponent();

            // ✅ Initialiser la date par défaut (14 jours)
            ReturnDatePicker.SelectedDate = DateTime.Today.AddDays(14);

            // ✅ Événements
            Loaded += BookBorrowPanel_Loaded;
            CloseBtn.MouseLeftButtonDown += (s, e) => OnCloseRequested();
        }

        private void BookBorrowPanel_Loaded(object sender, RoutedEventArgs e)
        {
            // ✅ Récupérer le livre depuis le DataContext
            if (DataContext is BookViewModel vm)
            {
                _currentBook = vm.GetBook();
                System.Diagnostics.Debug.WriteLine($"[BookBorrowPanel] 📚 Livre chargé: {_currentBook.Title}");
            }

            // ✅ Connecter le bouton de confirmation
            var confirmButton = FindName("ConfirmBorrowButton") as Button;
            if (confirmButton != null)
            {
                confirmButton.Click += ConfirmBorrow_Click;
            }
            else
            {
                // Fallback: chercher dans le template
                var footerButtons = this.FindVisualChildren<Button>();
                var confirmBtn = footerButtons.FirstOrDefault(b => b.Content?.ToString() == "Confirmer l'emprunt");
                if (confirmBtn != null)
                {
                    confirmBtn.Click += ConfirmBorrow_Click;
                }
            }
        }

        /// <summary>
        /// ✅ NOUVELLE FONCTIONNALITÉ : Confirmer l'emprunt
        /// </summary>
        private void ConfirmBorrow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validation des champs
                if (string.IsNullOrWhiteSpace(SubscriberSearchBox.Text))
                {
                    ShowError("Veuillez entrer l'ID de l'abonné.");
                    return;
                }

                if (!int.TryParse(SubscriberSearchBox.Text, out int subscriberId))
                {
                    ShowError("L'ID de l'abonné doit être un nombre valide.");
                    return;
                }

                if (!ReturnDatePicker.SelectedDate.HasValue)
                {
                    ShowError("Veuillez sélectionner une date de retour.");
                    return;
                }

                if (ReturnDatePicker.SelectedDate.Value.Date <= DateTime.Today)
                {
                    ShowError("La date de retour doit être dans le futur.");
                    return;
                }

                // ✅ Création de l'emprunt dans la base de données
                CreateLoan(subscriberId, ReturnDatePicker.SelectedDate.Value);
            }
            catch (Exception ex)
            {
                ShowError($"Erreur lors de la confirmation:\n{ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[BookBorrowPanel] ❌ Erreur: {ex.Message}");
            }
        }

        /// <summary>
        /// ✅ Crée l'emprunt dans la base de données
        /// </summary>
        private void CreateLoan(int subscriberId, DateTime returnDate)
        {
            try
            {
                var context = new LibraryDbContext();

                // Vérifier si le livre existe et est disponible
                var book = context.Books
                    .FirstOrDefault(b => b.BookId == _currentBook.BookId);

                if (book == null)
                {
                    ShowError("Ce livre n'existe plus dans la base de données.");
                    return;
                }

                if (book.Quantity <= 0)
                {
                    ShowError("Ce livre n'est pas disponible (quantité = 0).");
                    return;
                }

                // Vérifier si l'abonné existe
                var subscriber = context.Subscribers
                    .Include(s => s.Loans)
                    .FirstOrDefault(s => s.Id_User == subscriberId);

                if (subscriber == null)
                {
                    ShowError("Cet abonné n'existe pas.");
                    return;
                }

                // Vérifier la capacité d'emprunt
                if (!subscriber.ValidateCanBorrow(out string errorMsg))
                {
                    ShowError(errorMsg);
                    return;
                }

                // Vérifier si l'abonné a déjà emprunté ce livre
                var existingLoan = context.Loans
                    .FirstOrDefault(l => l.BookId == book.BookId &&
                                        l.SubscriberId == subscriberId &&
                                        l.IsActive);

                if (existingLoan != null)
                {
                    ShowError("Cet abonné a déjà emprunté ce livre et ne l'a pas encore retourné.");
                    return;
                }
                // ✅ Créer l'emprunt
                var loan = new Loan(subscriber, book)
                {
                    ReturnDate = returnDate,
                    IsActive = true,
                    Penalty = 0
                };
                // --- Exemple d'appel dans ton code principal ---
                var confirmationMessage =
                    $"Souhaitez-vous confirmer cet enregistrement ?\n\n" +
                    $"📚 Livre: {book.Title}\n" +
                    $"👤 Abonné: {subscriber.Name_User}\n" +
                    $"📅 Date d'emprunt: {loan.BorrowDate:dd/MM/yyyy}\n" +
                    $"📅 Date de retour: {loan.ReturnDate:dd/MM/yyyy}";

                if (!ShowConfirmation(confirmationMessage))
                {
                   
                    ShowInfo("L'enregistrement de l'emprunt a été annulé par l'utilisateur.");
                    return;
                }


                context.Loans.Add(loan);

                // Décrémenter la quantité
                book.Quantity--;
                if (book.Quantity == 0)
                {
                    book.IsAvailable = false;
                }

                // Augmenter la fidélité
                subscriber.IncreaseFidelity(0.2m);

                // Sauvegarder
                int saved = context.SaveChanges();

                if (saved > 0)
                {
                    // Générer la référence
                    context.GenerateReferences();

                    // Message de succès
                    ShowSuccess(
                        $"✅ Emprunt enregistré avec succès!\n\n" +
                        $"📚 Livre: {book.Title}\n" +
                        $"👤 Abonné: {subscriber.Name_User}\n" +
                        $"🔖 Référence: {loan.Ref_Loan}\n" +
                        $"📅 Date d'emprunt: {loan.BorrowDate:dd/MM/yyyy}\n" +
                        $"📅 Date de retour: {loan.ReturnDate:dd/MM/yyyy}\n" +
                        $"📊 Quantité restante: {book.Quantity}"
                    );

                    //Mise à jour de l'affichage 
                    BorrowCommand?.Execute(null);

                    // Fermer le panneau
                    OnCloseRequested();
                }
                else
                {
                    ShowError("Aucune modification n'a été enregistrée.");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Erreur lors de la création de l'emprunt:\n{ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[BookBorrowPanel] ❌ Erreur DB: {ex.Message}");
            }
        }

        /// <summary>
        /// Affiche un message d'erreur
        /// </summary>
        private void ShowError(string message)
        {
            Statut_Message.Text = $"❌ {message}";
            Statut_Message.Foreground = System.Windows.Media.Brushes.Red;
            Statut_Message.Visibility = Visibility.Visible;

            MessageBox.Show(message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Affiche un message de succès
        /// </summary>
        private void ShowSuccess(string message)
        {
            MessageBox.Show(message, "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private bool ShowConfirmation(string message)
        {
            var result = MessageBox.Show(
                message,
                "Confirmation requise",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            return result == MessageBoxResult.Yes;
        }

        private void ShowInfo(string message)
        {
            MessageBox.Show(message, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Ferme le panneau
        /// </summary>
        private void OnCloseRequested()
        {
            if (CloseCommand != null && CloseCommand.CanExecute(null))
            {
                CloseCommand.Execute(null);
            }
        }

        // Boutons rapides pour la date
        private void Set7Days_Click(object sender, RoutedEventArgs e) => SetReturnDate(7);
        private void Set14Days_Click(object sender, RoutedEventArgs e) => SetReturnDate(14);
        private void Set21Days_Click(object sender, RoutedEventArgs e) => SetReturnDate(21);

        private void SetReturnDate(int days)
        {
            ReturnDatePicker.SelectedDate = DateTime.Today.AddDays(days);
        }
    }

    /// <summary>
    /// Helper pour trouver les enfants visuels
    /// </summary>
    public static class VisualTreeHelperExtensions
    {
        public static System.Collections.Generic.IEnumerable<T> FindVisualChildren<T>(this DependencyObject parent)
            where T : DependencyObject
        {
            if (parent == null) yield break;

            for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);

                if (child is T tChild)
                    yield return tChild;

                foreach (var grandChild in FindVisualChildren<T>(child))
                    yield return grandChild;
            }
        }
    }
}