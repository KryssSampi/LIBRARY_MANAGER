using System;
using System.Collections.Generic;
using System.Linq;
using LIBBRARY_MANAGER.Model;
using LIBBRARY_MANAGER.UI.Common.Items.MainView;

namespace LIBBRARY_MANAGER.UI.Modules.LoanCatalog.Core.Services
{
    /// <summary>
    /// Service de filtrage pour les prêts
    /// </summary>
    public class LoanFilterService
    {
        /// <summary>
        /// Applique les filtres actifs sur la collection
        /// </summary>
        public IEnumerable<Loan> ApplyFilters(
            IEnumerable<Loan> loans,
            IEnumerable<FilterLabelViewModel> activeFilters)
        {
            if (loans == null || !loans.Any())
                return Enumerable.Empty<Loan>();

            if (activeFilters == null || !activeFilters.Any())
                return loans;

            var result = loans;

            foreach (var filter in activeFilters)
            {
                result = ApplyFilter(result, filter);
            }

            return result;
        }

        private IEnumerable<Loan> ApplyFilter(
            IEnumerable<Loan> loans,
            FilterLabelViewModel filter)
        {
            if (filter.Type == "Statut")
            {
                return filter.Value switch
                {
                    "En cours" => loans.Where(l => l.IsActive),
                    "Retourné" => loans.Where(l => !l.IsActive),
                    "En retard" => loans.Where(l => l.IsLate),
                    _ => loans
                };
            }

            if (filter.Type == "Pénalité")
            {
                return filter.Value switch
                {
                    "Avec pénalité" => loans.Where(l => l.Penalty.HasValue && l.Penalty > 0),
                    "Sans pénalité" => loans.Where(l => !l.Penalty.HasValue || l.Penalty == 0),
                    _ => loans
                };
            }

            return loans;
        }

        /// <summary>
        /// Filtre par statut
        /// </summary>
        public IEnumerable<Loan> FilterByStatus(IEnumerable<Loan> loans, bool isActive)
        {
            return loans.Where(l => l.IsActive == isActive);
        }

        /// <summary>
        /// Filtre par retard
        /// </summary>
        public IEnumerable<Loan> FilterByLate(IEnumerable<Loan> loans, bool isLate)
        {
            return loans.Where(l => l.IsLate == isLate);
        }

        /// <summary>
        /// Filtre par pénalité
        /// </summary>
        public IEnumerable<Loan> FilterByPenalty(IEnumerable<Loan> loans, bool hasPenalty)
        {
            return hasPenalty
                ? loans.Where(l => l.Penalty.HasValue && l.Penalty > 0)
                : loans.Where(l => !l.Penalty.HasValue || l.Penalty == 0);
        }
    }
}