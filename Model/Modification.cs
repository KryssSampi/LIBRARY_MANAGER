using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CommunityToolkit.Mvvm.ComponentModel;

namespace LIBBRARY_MANAGER.Model
{
    public enum ModificationType
    {
        ChangeReturnDate,
        ReturnLoan
    }

    public partial class Modification : ObservableObject
    {
        private long modificationId;
        private long staffMemberId;
        private StaffMember staffMember = null!;
        private long loanId;
        private Loan loan = null!;
        private ModificationType type;
        private string ref_Modification = string.Empty;
        private DateTime modificationDate;
        private string description = string.Empty;
        private DateTime? oldReturnDate;
        private DateTime? newReturnDate;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ModificationId
        {
            get => modificationId;
            set => SetProperty(ref modificationId, value);
        }

        [Required]
        public long StaffMemberId
        {
            get => staffMemberId;
            set => SetProperty(ref staffMemberId, value);
        }

        public virtual StaffMember StaffMember_
        {
            get => staffMember;
            set
            {
                if (SetProperty(ref staffMember, value) && value != null)
                {
                    StaffMemberId = value.Id_User;
                }
            }
        }

        [Required]
        public long LoanId
        {
            get => loanId;
            set => SetProperty(ref loanId, value);
        }

        public virtual Loan Loan_
        {
            get => loan;
            set
            {
                if (SetProperty(ref loan, value) && value != null)
                {
                    LoanId = value.LoanId;
                }
            }
        }

        [Required]
        public ModificationType Type
        {
            get => type;
            set
            {
                if (SetProperty(ref type, value))
                    OnPropertyChanged(nameof(Description));
            }
        }

        [Required, MaxLength(80)]
        public string Ref_Modification
        {
            get => ref_Modification;
            set => SetProperty(ref ref_Modification, value);
        }

        [Required]
        public DateTime ModificationDate
        {
            get => modificationDate;
            set => SetProperty(ref modificationDate, value);
        }

        [MaxLength(500)]
        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }

        public DateTime? OldReturnDate
        {
            get => oldReturnDate;
            set => SetProperty(ref oldReturnDate, value);
        }

        public DateTime? NewReturnDate
        {
            get => newReturnDate;
            set => SetProperty(ref newReturnDate, value);
        }

        // Constructeur EF Core
        public Modification()
        {
            ModificationDate = DateTime.Now;
        }

        public Modification(ModificationType type, Loan loan, StaffMember staff, DateTime? newReturn = null) : this()
        {
            if (loan == null)
                throw new ArgumentNullException(nameof(loan));
            if (staff == null)
                throw new ArgumentNullException(nameof(staff));

            Type = type;

            Loan_ = loan;
            LoanId = loan.LoanId;

            StaffMember_ = staff;
            StaffMemberId = staff.Id_User;

            if (type == ModificationType.ChangeReturnDate)
            {
                OldReturnDate = loan.ReturnDate;
                NewReturnDate = newReturn;
            }

            Description = GenerateDescription();
        }

        public void GenerateReference()
        {
            if (ModificationId == 0)
                throw new InvalidOperationException("Impossible de générer la référence avant l'assignation de ModificationId.");

            Ref_Modification = $"MOD-{ModificationDate:yyyyMMdd}-{ModificationId:D8}";
        }

        private string GenerateDescription()
        {
            try
            {
                switch (Type)
                {
                    case ModificationType.ChangeReturnDate:
                        string oldDate = OldReturnDate?.ToString("dd/MM/yyyy") ?? "N/A";
                        string newDate = NewReturnDate?.ToString("dd/MM/yyyy") ?? "N/A";
                        return $"Changement de date de retour : de {oldDate} à {newDate}";

                    case ModificationType.ReturnLoan:
                        string subscriberName = Loan_?.Subscriber?.Name_User ?? "Inconnu";
                        string bookTitle = Loan_?.Book?.Title ?? "Livre inconnu";
                        return $"Retour du livre '{bookTitle}' par {subscriberName}";

                    default:
                        return "Modification d'emprunt.";
                }
            }
            catch
            {
                return $"Modification de type {Type}";
            }
        }
    }
}