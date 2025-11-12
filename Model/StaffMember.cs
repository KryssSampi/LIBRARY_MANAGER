using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LIBBRARY_MANAGER.Data;
using Microsoft.EntityFrameworkCore;

namespace LIBBRARY_MANAGER.Model
{
    public class StaffMember : User
    {
        private const decimal FIDELITY_BONUS_ON_TIME = 0.2m;

        [Required, MaxLength(50)]
        public string Poste { get; set; } = "Employé";

        [Required]
        public int YearHired { get; set; }

        [Required, MaxLength(80)]
        public string Ref_Staff { get; set; } = string.Empty;

        public virtual ICollection<Modification> Modifications { get; set; } = new List<Modification>();

        [NotMapped]
        public int Anciennete => DateTime.Now.Year - YearHired;

        public StaffMember() : base()
        {
            YearHired = DateTime.Now.Year;
            // Ref_Staff sera généré dans DbContext.SaveChanges après obtention de Id_User
        }

        /// <summary>
        /// Génère la référence unique pour ce membre du personnel. À appeler APRÈS que Id_User soit assigné.
        /// </summary>
        public void GenerateReference()
        {
            if (Id_User == 0)
                throw new InvalidOperationException("Impossible de générer la référence avant l'assignation de Id_User.");
            Ref_Staff = $"STAFF-{Date_Creation:yyyyMMdd}{Id_User:D4}";
        }

        // ✅ MÉTHODE CORRIGÉE pour ReturnLoan dans StaffMember.cs

        /// <summary>
        /// ✅ CORRECTION: Traite le retour d'un emprunt avec gestion correcte du contexte EF
        /// </summary>
        public Modification? ReturnLoan(Loan loan, out string? message)
        {
            message = null;

            // Validations basiques
            if (loan == null)
            {
                message = "L'emprunt ne peut pas être null.";
                return null;
            }

            if (!loan.IsActive)
            {
                message = "Cet emprunt a déjà été retourné.";
                return null;
            }

            // ✅ IMPORTANT: Ne pas modifier directement les entités de navigation
            // EF Core va gérer ça via les FK

            // Traitement du retour
            loan.IsActive = false;
            loan.ActualReturnDate = DateTime.Now;

            // ✅ CORRECTION: Ne pas appeler Book.IncreaseQuantity() directement
            // Laissez le ViewModel gérer ça dans le contexte approprié

            // Gestion de la fidélité et pénalités
            if (loan.IsLate)
            {
                int daysLate = loan.DaysLate;
                loan.Penalty = CalculatePenalty(daysLate);

                decimal fidelityDecrease = daysLate * 0.1m;

                // ✅ Vérifier que Subscriber est chargé
                if (loan.Subscriber != null)
                {
                    loan.Subscriber.DecreaseFidelity(fidelityDecrease);
                }
                else
                {
                    message = "ATTENTION: L'abonné n'a pas été chargé. La fidélité n'a pas été mise à jour.";
                }
            }
            else
            {
                if (loan.Subscriber != null)
                {
                    loan.Subscriber.IncreaseFidelity(FIDELITY_BONUS_ON_TIME);
                }
            }

            // Création de la modification
            var modification = new Modification(ModificationType.ReturnLoan, loan, this);

            return modification;
        }

        /// <summary>
        /// ✅ CORRECTION: Change la date de retour d'un emprunt
        /// </summary>
        public Modification? ChangeReturnDate(Loan loan, DateTime newReturnDate, out string? message)
        {
            message = null;

            // Validations
            if (loan == null)
            {
                message = "L'emprunt ne peut pas être null.";
                return null;
            }

            if (!loan.IsActive)
            {
                message = "Impossible de prolonger un emprunt inactif.";
                return null;
            }

            if (newReturnDate.Date <= DateTime.Now.Date)
            {
                message = "La nouvelle date de retour doit être dans le futur.";
                return null;
            }

            // ✅ Vérification de la date maximale selon la fidélité
            if (loan.Subscriber != null)
            {
                var maxDate = GetMaxReturnDate(loan.BorrowDate, loan.Subscriber.Fidelity);
                if (newReturnDate.Date > maxDate.Date)
                {
                    message = $"Date de retour hors limite. Date maximale autorisée: {maxDate:dd/MM/yyyy}";
                    return null;
                }
            }

            // Création de la modification AVANT de changer la date
            var modification = new Modification(
                ModificationType.ChangeReturnDate,
                loan,
                this,
                newReturnDate
            );

            // ✅ Modifier la date APRÈS avoir créé la modification
            loan.ReturnDate = newReturnDate;

            return modification;
        }

     
        /// <summary>
        /// Calcule le montant de la pénalité selon les jours de retard.
        /// </summary>
        private decimal CalculatePenalty(int daysLate)
        {
            // Règle métier: 1€ par jour de retard
            return Math.Max(0, daysLate * 1.00m);
        }

        /// <summary>
        /// Calcule la date de retour maximale selon la fidélité de l'abonné.
        /// </summary>
        private DateTime GetMaxReturnDate(DateTime borrowDate, decimal fidelity)
        {
            // Règle métier: 14 jours de base + 2 jours par point de fidélité
            int baseDays = 14;
            int bonusDays = (int)(fidelity * 2);
            return borrowDate.AddDays(baseDays + bonusDays);
        }

        /// <summary>
        /// Crée (si nécessaire) et retourne un membre du personnel de test cohérent.
        /// </summary>
        public static async Task<StaffMember> EnsureTestStaffAsync(LibraryDbContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            // Vérifie si un staff test existe déjà
            var existingStaff = await context.StaffMembers
                .FirstOrDefaultAsync(s => s.Adresse_Mail == "staff@test.local");

            if (existingStaff != null)
                return existingStaff;

            // Crée un nouveau staff cohérent
            var staff = new StaffMember
            {
                Name_User = "Membre Test",
                Adresse_Mail = "staff@test.local",
                Num_Telephone = "000-111-2222",
                Adresse = "123 Rue de la Bibliothèque, Ottawa",
                Poste = "Bibliothécaire Principal",
                YearHired = DateTime.Now.Year - 3
            };

            staff.Password = "test1234"; // ⚠️ uniquement pour dev/test
            context.StaffMembers.Add(staff);

            await context.SaveChangesAsync();
            context.GenerateReferences();

            return staff;
        }

        /// <summary>
        /// Version synchrone.
        /// </summary>
        public static StaffMember EnsureTestStaff(LibraryDbContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            // Vérifie si un staff test existe déjà
            var existingStaff = context.StaffMembers
                .FirstOrDefault(s => s.Adresse_Mail == "staff@test.local");

            if (existingStaff != null)
                return existingStaff;

            // Crée un nouveau staff cohérent
            var staff = new StaffMember
            {
                Name_User = "Membre Test",
                Adresse_Mail = "staff@test.local",
                Num_Telephone = "000-111-2222",
                Adresse = "123 Rue de la Bibliothèque, Ottawa",
                Poste = "Bibliothécaire Principal",
                YearHired = DateTime.Now.Year - 3
            };

            staff.Password = "test1234"; // ⚠️ uniquement pour dev/test
            context.StaffMembers.Add(staff);

            context.SaveChanges();
            context.GenerateReferences();

            return staff;
        }
    }
}
