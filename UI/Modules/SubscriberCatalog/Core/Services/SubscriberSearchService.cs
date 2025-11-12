using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using global::LIBBRARY_MANAGER.Model;
namespace LIBBRARY_MANAGER.UI.Modules.SubscriberCatalog.Core.Services
{
  
        /// <summary>
        /// Service de recherche pour les abonnés
        /// Effectue des recherches multi-critères sur les propriétés des abonnés
        /// </summary>
        public class SubscriberSearchService
        {
            /// <summary>
            /// Effectue une recherche sur tous les champs pertinents des abonnés
            /// </summary>
            public List<Subscriber> Search(IEnumerable<Subscriber> subscribers, string searchText)
            {
                if (string.IsNullOrWhiteSpace(searchText))
                    return subscribers.ToList();

                var searchTerms = searchText.ToLower().Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

                return subscribers.Where(sub =>
                    searchTerms.Any(term =>
                        MatchesReference(sub, term) ||
                        MatchesName(sub, term) ||
                        MatchesEmail(sub, term) ||
                        MatchesFidelity(sub, term) ||
                        MatchesActiveLoans(sub, term) ||
                        MatchesRegistrationDate(sub, term)
                    )
                ).ToList();
            }

            /// <summary>
            /// Recherche par référence d'abonné
            /// </summary>
            private bool MatchesReference(Subscriber sub, string term)
            {
                return !string.IsNullOrEmpty(sub.Ref_Subscriber) &&
                       sub.Ref_Subscriber.ToLower().Contains(term);
            }

            /// <summary>
            /// Recherche par nom complet (nom et prénom)
            /// </summary>
            private bool MatchesName(Subscriber sub, string term)
            {
                return !string.IsNullOrEmpty(sub.Name_User) &&
                       sub.Name_User.ToLower().Contains(term);
            }

            /// <summary>
            /// Recherche par email
            /// </summary>
            private bool MatchesEmail(Subscriber sub, string term)
            {
                return !string.IsNullOrEmpty(sub.Adresse_Mail) &&
                       sub.Adresse_Mail.ToLower().Contains(term);
            }

            /// <summary>
            /// Recherche par niveau de fidélité (avec termes intelligents)
            /// </summary>
            private bool MatchesFidelity(Subscriber sub, string term)
            {
                // Recherche directe par valeur numérique
                if (decimal.TryParse(term, out decimal fidelityValue))
                {
                    return Math.Abs(sub.Fidelity - fidelityValue) < 0.5m;
                }

                // Recherche par termes descriptifs
                return term switch
                {
                    "excellent" => sub.Fidelity >= 8,
                    "tres bon" => sub.Fidelity >= 7,
                    "bon" => sub.Fidelity >= 5 && sub.Fidelity < 8,
                    "moyen" => sub.Fidelity >= 3 && sub.Fidelity < 5,
                    "faible" => sub.Fidelity < 3,
                    _ => false
                };
            }

            /// <summary>
            /// Recherche par nombre d'emprunts actifs
            /// </summary>
            private bool MatchesActiveLoans(Subscriber sub, string term)
            {
                // Recherche directe par valeur numérique
                if (int.TryParse(term, out int loansCount))
                {
                    return sub.ActiveLoansCount == loansCount;
                }

                // Recherche par termes descriptifs
                return term switch
                {
                    "aucun" or "zero" or "0" => sub.ActiveLoansCount == 0,
                    "actif" or "emprunts" => sub.ActiveLoansCount > 0,
                    "nombreux" or "beaucoup" => sub.ActiveLoansCount >= 3,
                    _ => false
                };
            }

            /// <summary>
            /// Recherche par date d'inscription
            /// </summary>
            private bool MatchesRegistrationDate(Subscriber sub, string term)
            {
                var now = DateTime.Now;

                return term switch
                {
                    "nouveau" or "recent" or "nouvelle" =>
                        sub.Date_Creation >= now.AddMonths(-1),
                    "ancien" or "ancienne" =>
                        sub.Date_Creation < now.AddYears(-1),
                    "semaine" =>
                        sub.Date_Creation >= now.AddDays(-7),
                    "mois" =>
                        sub.Date_Creation >= now.AddMonths(-1),
                    "annee" or "année" =>
                        sub.Date_Creation >= now.AddYears(-1),
                    _ => CheckDateByYear(sub, term)
                };
            }

            /// <summary>
            /// Vérifie si la date contient l'année recherchée
            /// </summary>
            private bool CheckDateByYear(Subscriber sub, string term)
            {
                if (int.TryParse(term, out int year))
                {
                    return sub.Date_Creation.Year == year;
                }
                return false;
            }

            /// <summary>
            /// Recherche avancée avec filtres spécifiques
            /// </summary>
            public List<Subscriber> AdvancedSearch(
                IEnumerable<Subscriber> subscribers,
                string searchText,
                decimal? minFidelity = null,
                decimal? maxFidelity = null,
                int? minLoans = null,
                int? maxLoans = null,
                bool? canBorrow = null,
                DateTime? registeredAfter = null,
                DateTime? registeredBefore = null)
            {
                var results = Search(subscribers, searchText);

                // Appliquer les filtres additionnels
                if (minFidelity.HasValue)
                    results = results.Where(s => s.Fidelity >= minFidelity.Value).ToList();

                if (maxFidelity.HasValue)
                    results = results.Where(s => s.Fidelity <= maxFidelity.Value).ToList();

                if (minLoans.HasValue)
                    results = results.Where(s => s.ActiveLoansCount >= minLoans.Value).ToList();

                if (maxLoans.HasValue)
                    results = results.Where(s => s.ActiveLoansCount <= maxLoans.Value).ToList();

                if (canBorrow.HasValue)
                    results = results.Where(s => s.CanBorrow == canBorrow.Value).ToList();

                if (registeredAfter.HasValue)
                    results = results.Where(s => s.Date_Creation >= registeredAfter.Value).ToList();

                if (registeredBefore.HasValue)
                    results = results.Where(s => s.Date_Creation <= registeredBefore.Value).ToList();

                return results;
            }

            /// <summary>
            /// Recherche par référence exacte
            /// </summary>
            public Subscriber SearchByReference(IEnumerable<Subscriber> subscribers, string reference)
            {
                return subscribers.FirstOrDefault(s =>
                    s.Ref_Subscriber.Equals(reference, StringComparison.OrdinalIgnoreCase));
            }

            /// <summary>
            /// Recherche par email exact
            /// </summary>
            public Subscriber SearchByEmail(IEnumerable<Subscriber> subscribers, string email)
            {
                return subscribers.FirstOrDefault(s =>
                    s.Adresse_Mail.Equals(email, StringComparison.OrdinalIgnoreCase));
            }

            /// <summary>
            /// Obtient les suggestions de recherche basées sur le texte entré
            /// </summary>
            public List<string> GetSearchSuggestions(IEnumerable<Subscriber> subscribers, string partialText)
            {
                if (string.IsNullOrWhiteSpace(partialText) || partialText.Length < 2)
                    return new List<string>();

                var suggestions = new List<string>();
                var term = partialText.ToLower();

                // Suggestions de noms
                suggestions.AddRange(
                    subscribers
                        .Where(s => !string.IsNullOrEmpty(s.Name_User) &&
                                   s.Name_User.ToLower().Contains(term))
                        .Select(s => s.Name_User)
                        .Distinct()
                        .Take(5)
                );

                // Suggestions d'emails
                suggestions.AddRange(
                    subscribers
                        .Where(s => !string.IsNullOrEmpty(s.Adresse_Mail) &&
                                   s.Adresse_Mail.ToLower().Contains(term))
                        .Select(s => s.Adresse_Mail)
                        .Distinct()
                        .Take(3)
                );

                // Suggestions de références
                suggestions.AddRange(
                    subscribers
                        .Where(s => !string.IsNullOrEmpty(s.Ref_Subscriber) &&
                                   s.Ref_Subscriber.ToLower().Contains(term))
                        .Select(s => s.Ref_Subscriber)
                        .Distinct()
                        .Take(3)
                );

                return suggestions.Distinct().Take(10).ToList();
            }

            /// <summary>
            /// Calcule un score de pertinence pour le classement des résultats
            /// </summary>
            public int CalculateRelevanceScore(Subscriber subscriber, string searchText)
            {
                var score = 0;
                var term = searchText.ToLower();

                // Score élevé pour correspondance exacte de référence
                if (subscriber.Ref_Subscriber?.ToLower() == term)
                    score += 100;
                else if (subscriber.Ref_Subscriber?.ToLower().Contains(term) == true)
                    score += 50;

                // Score élevé pour correspondance d'email
                if (subscriber.Adresse_Mail?.ToLower() == term)
                    score += 90;
                else if (subscriber.Adresse_Mail?.ToLower().Contains(term) == true)
                    score += 40;

                // Score pour correspondance de nom
                if (subscriber.Name_User?.ToLower().StartsWith(term) == true)
                    score += 70;
                else if (subscriber.Name_User?.ToLower().Contains(term) == true)
                    score += 30;

                return score;
            }

            /// <summary>
            /// Recherche avec classement par pertinence
            /// </summary>
            public List<Subscriber> SearchWithRanking(IEnumerable<Subscriber> subscribers, string searchText)
            {
                var results = Search(subscribers, searchText);

                return results
                    .OrderByDescending(s => CalculateRelevanceScore(s, searchText))
                    .ThenBy(s => s.Name_User)
                    .ToList();
            }
        }
    }