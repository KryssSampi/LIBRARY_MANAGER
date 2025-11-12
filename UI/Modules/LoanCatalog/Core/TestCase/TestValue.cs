using System;
using System.Collections.Generic;
using System.Linq;
using LIBBRARY_MANAGER.Model;
using LIBBRARY_MANAGER.UI.Modules.BookCatalog.Core.TestCase;

namespace LIBBRARY_MANAGER.UI.Modules.LoanCatalog.Core.TestCase
{
    /// <summary>
    /// Données de test pour le catalogue de prêts
    /// </summary>
    public static class LoanCatalogSample
    {
        public static List<Loan> FakeLoans()
        {
            var books = BookCatalogSample.GetSampleBooks();
            var subscribers = SubscriberCatalogSample.FakeSubscribers();

            var loans = new List<Loan>();
            var random = new Random(42);

            // Créer 50 prêts de test
            for (int i = 0; i < random.Next(30,100); i++)
            {
                var book = books[random.Next(books.Count)];
                var subscriber = subscribers[random.Next(subscribers.Count)];

                var borrowDate = DateTime.Now.AddDays(-random.Next(1, 90));
                var returnDate = borrowDate.AddDays(14 + random.Next(0, 15));
                var isActive = random.Next(100) < 70; // 70% de prêts actifs

                var loan = new Loan(subscriber,book)
                {
                    LoanId = i + 1,
                    Book =book,
                    Subscriber = subscriber,
                    ReturnDate = returnDate,
                    IsActive = isActive
                };

                // Générer la référence
                loan.GenerateReference();

                // Ajouter une pénalité si en retard et actif
                if (loan.IsLate && loan.IsActive)
                {
                    loan.Penalty = loan.DaysLate * 0.50m; // 0.50€ par jour de retard
                }

                loans.Add(loan);
            }

            return loans;
        }
    }
}