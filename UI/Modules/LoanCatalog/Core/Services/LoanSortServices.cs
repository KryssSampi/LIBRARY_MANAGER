using System;
using System.Collections.Generic;
using System.Linq;
using LIBBRARY_MANAGER.Model;

namespace LIBBRARY_MANAGER.UI.Modules.LoanCatalog.Core.Services
{
    /// <summary>
    /// Service de tri pour les prêts
    /// </summary>
    public class LoanSortService
    {
        /// <summary>
        /// Trie la collection selon le nom de colonne
        /// </summary>
        public IEnumerable<Loan> SortByColumnName(
            IEnumerable<Loan> loans,
            string columnName,
            bool ascending = true)
        {
            if (loans == null || !loans.Any())
                return Enumerable.Empty<Loan>();

            IOrderedEnumerable<Loan> sorted = columnName switch
            {
                "Référence" => ascending
                    ? loans.OrderBy(l => l.Ref_Loan)
                    : loans.OrderByDescending(l => l.Ref_Loan),

                "Abonné" => ascending
                    ? loans.OrderBy(l => l.Subscriber?.Name_User ?? string.Empty)
                    : loans.OrderByDescending(l => l.Subscriber?.Name_User ?? string.Empty),

                "Livre" => ascending
                    ? loans.OrderBy(l => l.Book?.Title ?? string.Empty)
                    : loans.OrderByDescending(l => l.Book?.Title ?? string.Empty),

                "Date d'Emprunt" => ascending
                    ? loans.OrderBy(l => l.BorrowDate)
                    : loans.OrderByDescending(l => l.BorrowDate),

                "Date de Retour" => ascending
                    ? loans.OrderBy(l => l.ReturnDate)
                    : loans.OrderByDescending(l => l.ReturnDate),

                "Statut" => ascending
                    ? loans.OrderBy(l => l.IsActive).ThenBy(l => l.IsLate)
                    : loans.OrderByDescending(l => l.IsActive).ThenByDescending(l => l.IsLate),

                "Pénalité" => ascending
                    ? loans.OrderBy(l => l.Penalty ?? 0)
                    : loans.OrderByDescending(l => l.Penalty ?? 0),

                _ => loans.OrderBy(l => l.BorrowDate)
            };

            return sorted;
        }
    }
}