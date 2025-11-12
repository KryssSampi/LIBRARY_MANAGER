using System;
using System.Collections.Generic;
using System.Linq;
using LIBBRARY_MANAGER.Model;

namespace LIBBRARY_MANAGER.UI.Modules.LoanCatalog.Core.Services
{
    /// <summary>
    /// Service de recherche pour les prêts
    /// </summary>
    public class LoanSearchService
    {
        /// <summary>
        /// Recherche dans la collection de prêts
        /// </summary>
        public IEnumerable<Loan> Search(IEnumerable<Loan> loans, string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return loans;

            var lowerSearch = searchText.ToLower().Trim();

            return loans.Where(loan =>
                // Référence du prêt
                loan.Ref_Loan?.ToLower().Contains(lowerSearch) == true ||

                // Nom de l'abonné
                loan.Subscriber?.Name_User?.ToLower().Contains(lowerSearch) == true ||

                // Titre du livre
                loan.Book?.Title?.ToLower().Contains(lowerSearch) == true ||

                // Auteur du livre
                loan.Book?.Author?.ToLower().Contains(lowerSearch) == true ||

                // ISBN du livre
                loan.Book?.ISBN?.ToLower().Contains(lowerSearch) == true
            );
        }
    }
}