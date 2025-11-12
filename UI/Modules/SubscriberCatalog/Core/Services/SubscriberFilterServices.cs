using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LIBBRARY_MANAGER.Model;
using LIBBRARY_MANAGER.UI.Common.Items.MainView;
using LIBBRARY_MANAGER.UI.Modules.BookCatalog.Items;

namespace LIBBRARY_MANAGER.UI.Modules.SubscriberCatalog.Core.Services
{
   
        /// <summary>
        /// Service de filtrage pour les abonnés
        /// Gère tous les types de filtres applicables aux abonnés
        /// </summary>
        public class SubscriberFilterService
        {
            /// <summary>
            /// Applique tous les filtres actifs sur la liste d'abonnés
            /// </summary>
            public List<Subscriber> ApplyFilters(
                IEnumerable<Subscriber> subscribers,
                IEnumerable<FilterLabelViewModel> activeFilters)
            {
                if (subscribers == null) return new List<Subscriber>();
                if (activeFilters == null || !activeFilters.Any()) return subscribers.ToList();

                var filtered = subscribers.ToList();

                foreach (var filter in activeFilters)
                {
                    filtered = filter.Type switch
                    {
                        "Fidélité" => FilterByFidelity(filtered, filter.Value),
                        "Emprunts" => FilterByActiveLoans(filtered, filter.Value),
                        "Statut" => FilterByCanBorrow(filtered, filter.Value),
                        "Inscription" => FilterByRegistrationDate(filtered, filter.Value),
                        _ => filtered
                    };
                }

                return filtered;
            }

            /// <summary>
            /// Filtre par niveau de fidélité
            /// </summary>
            private List<Subscriber> FilterByFidelity(List<Subscriber> subscribers, string filterValue)
            {
                return filterValue switch
                {
                    "Excellent (8+)" => subscribers.Where(s => s.Fidelity >= 8).ToList(),
                    "Bon (5-7)" => subscribers.Where(s => s.Fidelity >= 5 && s.Fidelity < 8).ToList(),
                    "Faible (<5)" => subscribers.Where(s => s.Fidelity < 5).ToList(),
                    _ => subscribers
                };
            }

            /// <summary>
            /// Filtre par nombre d'emprunts actifs
            /// </summary>
            private List<Subscriber> FilterByActiveLoans(List<Subscriber> subscribers, string filterValue)
            {
                return filterValue switch
                {
                    "Aucun" => subscribers.Where(s => s.ActiveLoansCount == 0).ToList(),
                    "1-2" => subscribers.Where(s => s.ActiveLoansCount >= 1 && s.ActiveLoansCount <= 2).ToList(),
                    "3+" => subscribers.Where(s => s.ActiveLoansCount >= 3).ToList(),
                    _ => subscribers
                };
            }

            /// <summary>
            /// Filtre par capacité d'emprunt
            /// </summary>
            private List<Subscriber> FilterByCanBorrow(List<Subscriber> subscribers, string filterValue)
            {
                return filterValue switch
                {
                    "Oui" => subscribers.Where(s => s.CanBorrow).ToList(),
                    "Non" => subscribers.Where(s => !s.CanBorrow).ToList(),
                    _ => subscribers
                };
            }

            /// <summary>
            /// Filtre par date d'inscription
            /// </summary>
            private List<Subscriber> FilterByRegistrationDate(List<Subscriber> subscribers, string filterValue)
            {
                var now = DateTime.Now;

                return filterValue switch
                {
                    "Cette semaine" => subscribers.Where(s =>
                        s.Date_Creation >= now.AddDays(-7)).ToList(),
                    "Ce mois" => subscribers.Where(s =>
                        s.Date_Creation >= now.AddMonths(-1)).ToList(),
                    "Cette année" => subscribers.Where(s =>
                        s.Date_Creation >= now.AddYears(-1)).ToList(),
                    "Plus ancien" => subscribers.Where(s =>
                        s.Date_Creation < now.AddYears(-1)).ToList(),
                    _ => subscribers
                };
            }

            /// <summary>
            /// Crée un filtre pour la fidélité
            /// </summary>
            public FilterLabelViewModel CreateFidelityFilter(string value)
            {
                return new FilterLabelViewModel
                {
                    Id = Guid.NewGuid(),
                    Type = "Fidélité",
                    Value = value
                };
            }

            /// <summary>
            /// Crée un filtre pour les emprunts actifs
            /// </summary>
            public FilterLabelViewModel CreateActiveLoansFilter(string value)
            {
                return new FilterLabelViewModel
                {
                    Id = Guid.NewGuid(),
                    Type = "Emprunts",
                    Value = value
                };
            }

            /// <summary>
            /// Crée un filtre pour le statut d'emprunt
            /// </summary>
            public FilterLabelViewModel CreateCanBorrowFilter(string value)
            {
                return new FilterLabelViewModel
                {
                    Id = Guid.NewGuid(),
                    Type = "Statut",
                    Value = value
                };
            }

            /// <summary>
            /// Crée un filtre pour la date d'inscription
            /// </summary>
            public FilterLabelViewModel CreateRegistrationDateFilter(string value)
            {
                return new FilterLabelViewModel
                {
                    Id = Guid.NewGuid(),
                    Type = "Inscription",
                    Value = value
                };
            }

            /// <summary>
            /// Obtient les statistiques de fidélité
            /// </summary>
            public Dictionary<string, int> GetFidelityStats(IEnumerable<Subscriber> subscribers)
            {
                var list = subscribers.ToList();
                return new Dictionary<string, int>
                {
                    ["Excellent (8+)"] = list.Count(s => s.Fidelity >= 8),
                    ["Bon (5-7)"] = list.Count(s => s.Fidelity >= 5 && s.Fidelity < 8),
                    ["Faible (<5)"] = list.Count(s => s.Fidelity < 5)
                };
            }

            /// <summary>
            /// Obtient les statistiques d'emprunts actifs
            /// </summary>
            public Dictionary<string, int> GetActiveLoansStats(IEnumerable<Subscriber> subscribers)
            {
                var list = subscribers.ToList();
                return new Dictionary<string, int>
                {
                    ["Aucun"] = list.Count(s => s.ActiveLoansCount == 0),
                    ["1-2"] = list.Count(s => s.ActiveLoansCount >= 1 && s.ActiveLoansCount <= 2),
                    ["3+"] = list.Count(s => s.ActiveLoansCount >= 3)
                };
            }

            /// <summary>
            /// Obtient les statistiques de capacité d'emprunt
            /// </summary>
            public Dictionary<string, int> GetCanBorrowStats(IEnumerable<Subscriber> subscribers)
            {
                var list = subscribers.ToList();
                return new Dictionary<string, int>
                {
                    ["Oui"] = list.Count(s => s.CanBorrow),
                    ["Non"] = list.Count(s => !s.CanBorrow)
                };
            }

            /// <summary>
            /// Obtient les statistiques par période d'inscription
            /// </summary>
            public Dictionary<string, int> GetRegistrationStats(IEnumerable<Subscriber> subscribers)
            {
                var list = subscribers.ToList();
                var now = DateTime.Now;

                return new Dictionary<string, int>
                {
                    ["Cette semaine"] = list.Count(s => s.Date_Creation >= now.AddDays(-7)),
                    ["Ce mois"] = list.Count(s => s.Date_Creation >= now.AddMonths(-1) && s.Date_Creation < now.AddDays(-7)),
                    ["Cette année"] = list.Count(s => s.Date_Creation >= now.AddYears(-1) && s.Date_Creation < now.AddMonths(-1)),
                    ["Plus ancien"] = list.Count(s => s.Date_Creation < now.AddYears(-1))
                };
            }
        }
    }