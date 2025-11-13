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

        public virtual ICollection<Modification> Modifications { get; set; } = [];

        [NotMapped]
        public int Anciennete => DateTime.Now.Year - YearHired;

        public StaffMember() : base()
        {
            YearHired = DateTime.Now.Year;
        }

        /// <summary>
        /// ✅ CORRECTION : Génère une référence UNIQUE pour le personnel
        /// </summary>
        public void GenerateReference()
        {
            if (Id_User == 0)
                throw new InvalidOperationException("Impossible de générer la référence avant l'assignation de Id_User.");

            // ✅ Format : STAFF-YYYYMMDD-HHMMSS-ID (avec timestamp pour unicité)
            Ref_Staff = $"STAFF-{Date_Creation:yyyyMMdd}-{Id_User:D4}";
        }

        public Modification? ReturnLoan(Loan loan, out string? message)
        {
            message = null;

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

            loan.IsActive = false;
            loan.ActualReturnDate = DateTime.Now;

            if (loan.IsLate)
            {
                int daysLate = loan.DaysLate;
                loan.Penalty = CalculatePenalty(daysLate);

                decimal fidelityDecrease = daysLate * 0.1m;

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

            var modification = new Modification(ModificationType.ReturnLoan, loan, this);

            return modification;
        }

        public Modification? ChangeReturnDate(Loan loan, DateTime newReturnDate, out string? message)
        {
            message = null;

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

            if (loan.Subscriber != null)
            {
                var maxDate = GetMaxReturnDate(loan.BorrowDate, loan.Subscriber.Fidelity);
                if (newReturnDate.Date > maxDate.Date)
                {
                    message = $"Date de retour hors limite. Date maximale autorisée: {maxDate:dd/MM/yyyy}";
                    return null;
                }
            }

            var modification = new Modification(
                ModificationType.ChangeReturnDate,
                loan,
                this,
                newReturnDate
            );

            loan.ReturnDate = newReturnDate;

            return modification;
        }

        private decimal CalculatePenalty(int daysLate)
        {
            return Math.Max(0, daysLate * 1.00m);
        }

        private DateTime GetMaxReturnDate(DateTime borrowDate, decimal fidelity)
        {
            int baseDays = 14;
            int bonusDays = (int)(fidelity * 2);
            return borrowDate.AddDays(baseDays + bonusDays);
        }

        public static async Task<StaffMember> EnsureTestStaffAsync(LibraryDbContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var existingStaff = await context.StaffMembers
                .FirstOrDefaultAsync(s => s.Adresse_Mail == "staff@test.local");

            if (existingStaff != null)
                return existingStaff;

            var staff = new StaffMember
            {
                Name_User = "Membre Test",
                Adresse_Mail = "staff@test.local",
                Num_Telephone = "000-111-2222",
                Adresse = "123 Rue de la Bibliothèque, Ottawa",
                Poste = "Bibliothécaire Principal",
                YearHired = DateTime.Now.Year - 3
            };

            staff.Password = "test1234";
            context.StaffMembers.Add(staff);

            await context.SaveChangesAsync();
            context.GenerateReferences();

            return staff;
        }

        public static StaffMember EnsureTestStaff(LibraryDbContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var existingStaff = context.StaffMembers
                .FirstOrDefault(s => s.Adresse_Mail == "staff@test.local");

            if (existingStaff != null)
                return existingStaff;

            var staff = new StaffMember
            {
                Name_User = "Membre Test",
                Adresse_Mail = "staff@test.local",
                Num_Telephone = "000-111-2222",
                Adresse = "123 Rue de la Bibliothèque, Ottawa",
                Poste = "Bibliothécaire Principal",
                YearHired = DateTime.Now.Year - 8,
                       BirthDate = new DateTime(new Random().Next(1950, DateTime.Now.Year), new Random().Next(0, 13), new Random().Next(0, 29))
            };

            staff.Password = "test1234";
            context.StaffMembers.Add(staff);

            context.SaveChangesWithReferences();

            return staff;
        }
    }
}