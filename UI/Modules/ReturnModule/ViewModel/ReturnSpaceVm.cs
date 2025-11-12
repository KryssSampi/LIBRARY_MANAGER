using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using LIBBRARY_MANAGER.Data;
using LIBBRARY_MANAGER.Model;
using System.Windows.Input;

namespace LIBBRARY_MANAGER.UI.Modules.ReturnModule.ViewModel
{
    public partial class ReturnSpaceVm : ObservableObject
    {
        public event Action? NavigationRequested;

        [ObservableProperty]
        private string isbnLivre = string.Empty;

        [ObservableProperty]
        private string idMembre = string.Empty;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        private ICommand getHomeCommad;

        [ObservableProperty]
        private ICommand confirmationCommand;

        public ReturnSpaceVm()
        {
            confirmationCommand = new RelayCommand(Confirmer);
        }

        [RelayCommand]
        private void Confirmer()
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

            try
            {
                using (var context = new LibraryDbContext())
                {
                    // Vérifier si le livre existe
                    Book livre = context.Books.FirstOrDefault(l => l.ISBN == IsbnLivre);
                    if (livre == null)
                    {
                        ErrorMessage = "Ce livre n'existe pas.";
                        return;
                    }

                    // Vérifier si le membre existe
                    Subscriber membre = context.Subscribers.FirstOrDefault(m => m.Id_User == membreId);
                    if (membre == null)
                    {
                        ErrorMessage = "Ce membre n'existe pas.";
                        return;
                    }

                    // Rechercher l'emprunt actif pour ce livre et ce membre
                    Loan emprunt = context.Loans
                        .Include(e => e.Book)
                        .Include(e => e.Subscriber)
                        .FirstOrDefault(e => e.Book.ISBN == IsbnLivre &&
                                            e.SubscriberId == membreId &&
                                            e.IsActive == true);

                    if (emprunt == null)
                    {
                        ErrorMessage = "Aucun emprunt actif trouvé pour ce livre et ce membre.";
                        return;
                    }

                    // Calculer les jours de retard
                    var dateRetourEffective = DateTime.Now;
                    var joursRetard = (dateRetourEffective.Date - emprunt.ReturnDate.Date).Days;
                    decimal penalite = 0;

                    if (joursRetard > 0)
                    {
                        // 1$ par jour de retard
                        penalite = joursRetard * 1.0m;
                        membre.DecreaseFidelity(joursRetard * 0.1m);
                    }
                    else
                    {
                        // Bonus pour retour à temps
                        membre.IncreaseFidelity(0.2m);
                    }

                    // Mettre à jour l'emprunt
                    emprunt.IsActive = false;
                    emprunt.Penalty = penalite;
                    // IMPORTANT: Ne pas modifier ReturnDate ici, il garde la date prévue

                    // Remettre le livre disponible
                    livre.Quantity++;
                    livre.IsAvailable = true;

                    // Sauvegarder les modifications
                    try
                    {
                        var nbChanges = context.SaveChanges();

                        if (nbChanges == 0)
                        {
                            ErrorMessage = "Aucune modification n'a été enregistrée.";
                            return;
                        }

                        string message = $"✅ Retour enregistré avec succès!\n\n" +
                                       $"📚 Livre: {livre.Title}\n" +
                                       $"👤 Membre: {membre.Name_User}\n" +
                                       $"🔖 Référence: {emprunt.Ref_Loan}\n" +
                                       $"📅 Date d'emprunt: {emprunt.BorrowDate:dd/MM/yyyy}\n" +
                                       $"📅 Date prévue: {emprunt.ReturnDate:dd/MM/yyyy}\n" +
                                       $"📅 Date effective: {dateRetourEffective:dd/MM/yyyy}\n" +
                                       $"📊 Quantité disponible: {livre.Quantity}\n" +
                                       $"⭐ Fidélité: {membre.Fidelity:F2}/10\n";

                        if (joursRetard > 0)
                        {
                            message += $"\n⚠️ Retard: {joursRetard} jour(s)\n" +
                                      $"💰 Pénalité: {penalite:C}";

                            if (joursRetard > 30)
                            {
                                message += "\n\n⚠️ Le membre a perdu beaucoup de fidélité (retard > 30 jours)";
                            }
                        }
                        else
                        {
                            message += "\n✅ Retour à temps - Bonus de fidélité accordé";
                        }

                        MessageBox.Show(
                            message,
                            "Retour enregistré",
                            MessageBoxButton.OK,
                            joursRetard > 0 ? MessageBoxImage.Warning : MessageBoxImage.Information
                        );
                    }
                    catch (DbUpdateException dbEx)
                    {
                        ErrorMessage = $"Erreur BD: {dbEx.InnerException?.Message ?? dbEx.Message}";
                        MessageBox.Show(
                            $"Erreur de base de données:\n\n{dbEx.InnerException?.Message ?? dbEx.Message}",
                            "Erreur",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error
                        );
                        return;
                    }

                    // Réinitialiser les champs
                    IsbnLivre = string.Empty;
                    IdMembre = string.Empty;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur: {ex.Message}";
                MessageBox.Show(
                    $"Erreur complète:\n\n" +
                    $"Message: {ex.Message}\n\n" +
                    $"Type: {ex.GetType().Name}\n\n" +
                    $"Inner: {ex.InnerException?.Message}",
                    "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        [RelayCommand]
        private void Accueil()
        {
            getHomeCommad?.Execute(null);
        }

        [RelayCommand]
        private void VerifierEmpruntsActifs()
        {
            try
            {
                using (var context = new LibraryDbContext())
                {
                    var empruntsActifs = context.Loans
                        .Include(e => e.Book)
                        .Include(e => e.Subscriber)
                        .Where(e => e.IsActive == true)
                        .ToList();

                    var message = $"📊 Total d'emprunts actifs: {empruntsActifs.Count}\n\n";

                    foreach (var emp in empruntsActifs.Take(10))
                    {
                        var joursRetard = (DateTime.Now.Date - emp.ReturnDate.Date).Days;
                        string statut = joursRetard > 0 ? $"⚠️ Retard: {joursRetard}j" : "✅ À temps";

                        message += $"Livre: {emp.Book?.Title ?? emp.Book.ISBN}\n";
                        message += $"Membre: {emp.Subscriber?.Name_User ?? $"ID:{emp.SubscriberId}"}\n";
                        message += $"Référence: {emp.Ref_Loan}\n";
                        message += $"{statut}\n";
                        message += "-------------------\n";
                    }

                    if (empruntsActifs.Count > 10)
                    {
                        message += $"\n... et {empruntsActifs.Count - 10} autres emprunts.";
                    }

                    MessageBox.Show(message, "Emprunts actifs", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}