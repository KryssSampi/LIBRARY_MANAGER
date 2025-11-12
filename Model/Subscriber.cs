using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIBBRARY_MANAGER.Model
{
    public class Subscriber : User
    {
        private const decimal MIN_FIDELITY_TO_BORROW = 1.00m;
        private const decimal MAX_FIDELITY = 10.00m;
        private const decimal DEFAULT_FIDELITY = 5.00m;

        [Required]
        [Range(0, 10)]
        public decimal Fidelity { get; set; } = DEFAULT_FIDELITY;

        [Required, MaxLength(80)]
        public string Ref_Subscriber { get; set; } = string.Empty; // ✅ Changé de private set à public set

        public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();

        [NotMapped]
        public bool CanBorrow => Fidelity >= MIN_FIDELITY_TO_BORROW;

        [NotMapped]
        public int ActiveLoansCount => Loans?.Count(l => l.IsActive) ?? 0;

        // ✅ CORRECTION MAJEURE: Constructeur sans génération aléatoire
        public Subscriber() : base()
        {
            // ❌ SUPPRIMÉ: Id_User = new Random().Next();
            // ❌ SUPPRIMÉ: GenerateReference();
            // La référence sera générée APRÈS SaveChanges dans DbContext
        }

        /// <summary>
        /// Génère la référence unique pour cet abonné. À appeler APRÈS que Id_User soit assigné.
        /// </summary>
        public void GenerateReference()
        {
            if (Id_User == 0)
                throw new InvalidOperationException("Impossible de générer la référence avant l'assignation de Id_User.");

            Ref_Subscriber = $"SUB-{Date_Creation:yyyyMMdd}{Id_User:D10}";
        }

        /// <summary>
        /// Augmente la fidélité après un retour à temps.
        /// </summary>
        public void IncreaseFidelity(decimal amount = 0.2m)
        {
            Fidelity = Math.Min(Fidelity + amount, MAX_FIDELITY);
        }

        /// <summary>
        /// Diminue la fidélité après un retour en retard.
        /// </summary>
        public void DecreaseFidelity(decimal amount)
        {
            Fidelity = Math.Max(Fidelity - amount, 0);
        }

        /// <summary>
        /// Valide si l'abonné peut emprunter un livre.
        /// </summary>
        public bool ValidateCanBorrow(out string? errorMessage)
        {
            errorMessage = null;

            if (!CanBorrow)
            {
                errorMessage = $"Votre fidélité ({Fidelity:F2}) est insuffisante. Minimum requis: {MIN_FIDELITY_TO_BORROW:F2}";
                return false;
            }

            // Règle métier: limite de 5 emprunts actifs simultanés
            if (ActiveLoansCount >= 5)
            {
                errorMessage = "Vous avez atteint le maximum d'emprunts actifs (5).";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Génère des abonnés de démonstration
        /// </summary>
        public static ObservableCollection<Subscriber> GenerateDemoSubscribers()
        {
            var random = new Random();
            var subscribers = new ObservableCollection<Subscriber>();

            var firstNames = new[] { "Jean", "Marie", "Pierre", "Sophie", "Luc", "Anne", "Paul", "Julie", "Marc", "Claire" };
            var lastNames = new[] { "Martin", "Bernard", "Dubois", "Thomas", "Robert", "Richard", "Petit", "Durand", "Leroy", "Moreau" };

            for (int i = 1; i <= 50; i++)
            {
                var firstName = firstNames[random.Next(firstNames.Length)];
                var lastName = lastNames[random.Next(lastNames.Length)];

                subscribers.Add(new Subscriber
                {
                    // ✅ Ne pas définir Ref_Subscriber ici - sera généré par DbContext
                    Name_User = $"{firstName} {lastName}",
                    Adresse_Mail = $"{firstName.ToLower()}.{lastName.ToLower()}@email.com",
                    Fidelity = (decimal)(random.NextDouble() * 10),
                    Date_Creation = DateTime.Now.AddDays(-random.Next(1, 1000))
                });
            }

            return subscribers;
        }
    }
}