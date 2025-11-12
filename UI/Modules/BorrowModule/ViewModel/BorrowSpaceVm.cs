using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LIBBRARY_MANAGER.Data;
using LIBBRARY_MANAGER.Model;

namespace LIBBRARY_MANAGER.UI.Modules.BorrowModule.ViewModel
{
    public partial class BorrowSpaceVm : ObservableObject
    {
        [ObservableProperty]
        public ICommand getHomeCommad;

        [ObservableProperty]
        public ICommand getLoansCommand;

        [ObservableProperty]
        public ICommand confirmationCommand;

        [ObservableProperty]
        public string isbnLivre = string.Empty;

        [ObservableProperty]
        public string idMembre = string.Empty;

        [ObservableProperty]
        public string errorMessage = string.Empty;

        [ObservableProperty]
        private DateTime? selectedReturnDate = DateTime.Now.AddDays(14);

        public BorrowSpaceVm()
        {
            confirmationCommand = new RelayCommand(Confirmer);
        }

        [RelayCommand]
        public void Confirmer()
        {
            ErrorMessage = string.Empty;

            // Validation des champs
            if (string.IsNullOrWhiteSpace(IsbnLivre))
            {
                ErrorMessage = "Veuillez entrer l'ISBN du livre.";
                return;
            }

            if (string.IsNullOrWhiteSpace(IdMembre))
            {
                ErrorMessage = "Veuillez entrer l'ID de l'abonné.";
                return;
            }

            if (!int.TryParse(IdMembre, out int membreId))
            {
                ErrorMessage = "L'ID de l'abonné doit être un nombre.";
                return;
            }

            if (!SelectedReturnDate.HasValue)
            {
                ErrorMessage = "Veuillez sélectionner une date de retour.";
                return;
            }

            if (SelectedReturnDate.Value.Date <= DateTime.Now.Date)
            {
                ErrorMessage = "La date de retour doit être dans le futur.";
                return;
            }

            try
            {
                using (var context = new LibraryDbContext())
                {
                    // Vérifier si le livre existe
                    Book livre = context.Books.FirstOrDefault(l => l.ISBN == IsbnLivre);
                    if (livre == null)
                    {
                        ErrorMessage = "Ce livre n'existe pas dans la base de données.";
                        return;
                    }

                    // Vérifier la disponibilité
                    if (livre.Quantity <= 0)
                    {
                        ErrorMessage = "Ce livre n'est pas disponible (quantité = 0).";
                        return;
                    }

                    // Vérifier si le membre existe
                    Subscriber membre = context.Subscribers.FirstOrDefault(m => m.Id_User == membreId);
                    if (membre == null)
                    {
                        ErrorMessage = "Ce membre n'existe pas dans la base de données.";
                        return;
                    }

                    // Vérifier si le membre peut emprunter
                    if (!membre.ValidateCanBorrow(out string errorMsg))
                    {
                        ErrorMessage = errorMsg;
                        return;
                    }

                    // Vérifier si le membre a déjà emprunté ce livre (et ne l'a pas retourné)
                    var empruntExistant = context.Loans
                        .FirstOrDefault(e => e.Book.ISBN == IsbnLivre &&
                                            e.SubscriberId == membreId &&
                                            e.IsActive == true);

                    if (empruntExistant != null)
                    {
                        ErrorMessage = "Ce membre a déjà emprunté ce livre et ne l'a pas encore retourné.";
                        return;
                    }

                    // Créer le nouvel emprunt
                    var emprunt = new Loan(membre,livre)
                    {
                        ReturnDate = SelectedReturnDate.Value,
                        IsActive = true,
                        Penalty = 0,
                    };

                    // Ajouter l'emprunt
                    context.Loans.Add(emprunt);

                    // Mettre à jour la quantité du livre
                    livre.Quantity--;
                    if (livre.Quantity == 0)
                    {
                        livre.IsAvailable = false;
                    }

                    // Incrémenter la fidélité du membre
                    membre.IncreaseFidelity();

                    // Sauvegarder les changements
                    int savedRecords = context.SaveChanges();

                    // Générer la référence de l'emprunt après l'insertion
                    context.GenerateReferences();

                    // Vérifier que les changements ont bien été sauvegardés
                    if (savedRecords == 0)
                    {
                        ErrorMessage = "Aucune modification n'a été enregistrée dans la base de données.";
                        MessageBox.Show("Erreur: Aucune modification enregistrée!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Message de succès
                    MessageBox.Show(
                        $"✅ Emprunt enregistré avec succès!\n\n" +
                        $"📚 Livre: {livre.Title}\n" +
                        $"👤 Membre: {membre.Name_User}\n" +
                        $"🔖 Référence: {emprunt.Ref_Loan}\n" +
                        $"📅 Date d'emprunt: {emprunt.BorrowDate:dd/MM/yyyy}\n" +
                        $"📅 Date de retour prévue: {emprunt.ReturnDate:dd/MM/yyyy}\n" +
                        $"📊 Quantité restante: {livre.Quantity}",
                        "Succès",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );

                    // Réinitialiser les champs
                    IsbnLivre = string.Empty;
                    IdMembre = string.Empty;
                    SelectedReturnDate = DateTime.Now.AddDays(14);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur lors de l'enregistrement: {ex.Message}";
                MessageBox.Show(
                    $"❌ Une erreur est survenue:\n\n{ex.Message}\n\n{ex.InnerException?.Message}",
                    "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        [RelayCommand]
        private void Accueil()
        {
            GetHomeCommad?.Execute(null);
        }

        [RelayCommand]
        private void VerifierEmprunts()
        {
            try
            {
                GetLoansCommand?.Execute(null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}