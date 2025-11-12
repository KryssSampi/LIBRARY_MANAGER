using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CommunityToolkit.Mvvm.ComponentModel;

namespace LIBBRARY_MANAGER.Model
{
    public partial class Loan : ObservableObject
    {
        private long loanId;
        private long bookId;
        private Book book = null!;
        private long subscriberId;
        private Subscriber subscriber = null!;
        private DateTime borrowDate;
        private DateTime returnDate;
        private DateTime? actualReturnDate;
        private bool isActive = true;
        private decimal? penalty;
        private string ref_Loan = string.Empty;
        private ICollection<Modification> modifications = new List<Modification>();

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long LoanId
        {
            get => loanId;
            set => SetProperty(ref loanId, value);
        }

        [Required]
        public long BookId
        {
            get => bookId;
            set => SetProperty(ref bookId, value);
        }

        public virtual Book Book
        {
            get => book;
            set
            {
                if (SetProperty(ref book, value) && value != null)
                {
                    BookId = value.BookId;
                }
            }
        }

        [Required]
        public long SubscriberId
        {
            get => subscriberId;
            set => SetProperty(ref subscriberId, value);
        }

        public virtual Subscriber Subscriber
        {
            get => subscriber;
            set
            {
                if (SetProperty(ref subscriber, value) && value != null)
                {
                    SubscriberId = value.Id_User;
                }
            }
        }

        [Required]
        public DateTime BorrowDate
        {
            get => borrowDate;
            set => SetProperty(ref borrowDate, value);
        }

        [Required]
        public DateTime ReturnDate
        {
            get => returnDate;
            set
            {
                if (SetProperty(ref returnDate, value))
                {
                    OnPropertyChanged(nameof(IsLate));
                    OnPropertyChanged(nameof(DaysLate));
                }
            }
        }

        /// <summary>
        /// Date effective du retour (null si pas encore retourné)
        /// </summary>
        public DateTime? ActualReturnDate
        {
            get => actualReturnDate;
            set => SetProperty(ref actualReturnDate, value);
        }

        [Required]
        public bool IsActive
        {
            get => isActive;
            set
            {
                if (SetProperty(ref isActive, value))
                {
                    OnPropertyChanged(nameof(IsLate));
                    OnPropertyChanged(nameof(DaysLate));
                }
            }
        }

        [Range(0, double.MaxValue)]
        public decimal? Penalty
        {
            get => penalty;
            set => SetProperty(ref penalty, value);
        }

        [Required, MaxLength(80)]
        public string Ref_Loan
        {
            get => ref_Loan;
            set => SetProperty(ref ref_Loan, value);
        }

        public virtual ICollection<Modification> Modifications
        {
            get => modifications;
            set => SetProperty(ref modifications, value);
        }

        [NotMapped]
        public bool IsLate => IsActive && DateTime.Now.Date > ReturnDate.Date;

        [NotMapped]
        public int DaysLate => IsLate ? (DateTime.Now.Date - ReturnDate.Date).Days : 0;

        // Constructeur EF Core
        public Loan()
        {
            BorrowDate = DateTime.Now;
        }

        /// <summary>
        /// ✅ CORRECTION: Constructeur pour créer un nouvel emprunt
        /// </summary>
        public Loan(Subscriber subscriber, Book book, int durationDays = 14) : this()
        {
            if (subscriber == null)
                throw new ArgumentNullException(nameof(subscriber));
            if (book == null)
                throw new ArgumentNullException(nameof(book));

            // ✅ CORRECTION: Vérifier que les IDs sont valides
            if (subscriber.Id_User == 0)
                throw new InvalidOperationException("L'abonné doit être sauvegardé en base avant de créer un emprunt.");

            if (book.BookId == 0)
                throw new InvalidOperationException("Le livre doit être sauvegardé en base avant de créer un emprunt.");

            // ✅ Définir les propriétés de navigation ET les FK
            Subscriber = subscriber;
            SubscriberId = subscriber.Id_User;

            Book = book;
            BookId = book.BookId;

            // Calculer la date de retour en fonction de la fidélité
            int bonusDays = (int)(subscriber.Fidelity * 2);
            ReturnDate = BorrowDate.AddDays(durationDays + bonusDays);

            IsActive = true;
        }

        public void GenerateReference()
        {
            if (LoanId == 0)
                throw new InvalidOperationException("Impossible de générer la référence avant l'assignation de LoanId.");

            Ref_Loan = $"LOAN-{BorrowDate:yyyyMMdd}-{LoanId:D8}";
        }
    }
}