using System;
using System.Collections.Generic;
using System.Linq;
using LIBBRARY_MANAGER.Model;

namespace LIBBRARY_MANAGER.UI.Modules.SubscriberCatalog.Core.Services
{
    /// <summary>
    /// Service de tri pour les abonnés
    /// </summary>
    public class SubscriberSortService
    {
        /// <summary>
        /// Trie les abonnés selon le nom de colonne et la direction
        /// </summary>
        public IEnumerable<Subscriber> SortByColumnName(
            IEnumerable<Subscriber> subscribers,
            string columnName,
            bool ascending)
        {
            if (subscribers == null || !subscribers.Any())
                return Enumerable.Empty<Subscriber>();

            if (string.IsNullOrWhiteSpace(columnName))
                return subscribers;

            try
            {
                columnName = columnName.Trim().ToLower();

                var sorted = columnName switch
                {
                    "référence" or "reference" or "ref" =>
                        SortByReference(subscribers, ascending),

                    "nom complet" or "nom" or "fullname" or "name" =>
                        SortByFullName(subscribers, ascending),

                    "email" or "adresse_mail" or "courriel" =>
                        SortByEmail(subscribers, ascending),

                    "fidélité" or "fidelite" or "fidelity" =>
                        SortByFidelity(subscribers, ascending),

                    "emprunts actifs" or "emprunts" or "activeloans" =>
                        SortByActiveLoans(subscribers, ascending),

                    "peut emprunter" or "canborrow" or "statut" =>
                        SortByCanBorrow(subscribers, ascending),

                    "date d'inscription" or "inscription" or "date_creation" =>
                        SortByRegistrationDate(subscribers, ascending),

                    "téléphone" or "telephone" or "num_telephone" =>
                        SortByTelephone(subscribers, ascending),

                    "adresse" =>
                        SortByAddress(subscribers, ascending),


                    _ => subscribers // Aucun tri si colonne inconnue
                };

                return sorted;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SubscriberSortService] Erreur tri: {ex.Message}");
                return subscribers;
            }
        }

        #region 🔹 Méthodes de tri individuelles

        public IEnumerable<Subscriber> SortByReference(IEnumerable<Subscriber> subscribers, bool ascending = true) =>
            ascending
                ? subscribers.OrderBy(s => s.Ref_Subscriber)
                : subscribers.OrderByDescending(s => s.Ref_Subscriber);

        public IEnumerable<Subscriber> SortByFullName(IEnumerable<Subscriber> subscribers, bool ascending = true) =>
            ascending
                ? subscribers.OrderBy(s => s.Name_User)
                : subscribers.OrderByDescending(s => s.Name_User);

        public IEnumerable<Subscriber> SortByEmail(IEnumerable<Subscriber> subscribers, bool ascending = true) =>
            ascending
                ? subscribers.OrderBy(s => s.Adresse_Mail)
                : subscribers.OrderByDescending(s => s.Adresse_Mail);

        public IEnumerable<Subscriber> SortByFidelity(IEnumerable<Subscriber> subscribers, bool ascending = true) =>
            ascending
                ? subscribers.OrderBy(s => s.Fidelity)
                : subscribers.OrderByDescending(s => s.Fidelity);

        public IEnumerable<Subscriber> SortByActiveLoans(IEnumerable<Subscriber> subscribers, bool ascending = true) =>
            ascending
                ? subscribers.OrderBy(s => s.ActiveLoansCount)
                : subscribers.OrderByDescending(s => s.ActiveLoansCount);

        public IEnumerable<Subscriber> SortByCanBorrow(IEnumerable<Subscriber> subscribers, bool ascending = true) =>
            ascending
                ? subscribers.OrderByDescending(s => s.CanBorrow) // true avant false
                : subscribers.OrderBy(s => s.CanBorrow);

        public IEnumerable<Subscriber> SortByRegistrationDate(IEnumerable<Subscriber> subscribers, bool ascending = true) =>
            ascending
                ? subscribers.OrderBy(s => s.Date_Creation)
                : subscribers.OrderByDescending(s => s.Date_Creation);

        public IEnumerable<Subscriber> SortByTelephone(IEnumerable<Subscriber> subscribers, bool ascending = true) =>
            ascending
                ? subscribers.OrderBy(s => s.Num_Telephone)
                : subscribers.OrderByDescending(s => s.Num_Telephone);

        public IEnumerable<Subscriber> SortByAddress(IEnumerable<Subscriber> subscribers, bool ascending = true) =>
            ascending
                ? subscribers.OrderBy(s => s.Adresse)
                : subscribers.OrderByDescending(s => s.Adresse);


        #endregion

        #region 🔹 Tri personnalisé et utilitaires

        /// <summary>
        /// Tri multi-colonnes
        /// </summary>
        public IEnumerable<Subscriber> SortMultiple(
            IEnumerable<Subscriber> subscribers,
            params (string column, bool ascending)[] sortCriteria)
        {
            if (sortCriteria == null || sortCriteria.Length == 0)
                return subscribers;

            IOrderedEnumerable<Subscriber> orderedQuery = null;

            foreach (var (column, ascending) in sortCriteria)
            {
                orderedQuery = orderedQuery == null
                    ? ApplySort(subscribers, column, ascending)
                    : ApplyThenBy(orderedQuery, column, ascending);
            }

            return orderedQuery ?? subscribers;
        }

        private IOrderedEnumerable<Subscriber> ApplySort(
            IEnumerable<Subscriber> subscribers,
            string column,
            bool ascending)
        {
            return ascending
                ? subscribers.OrderBy(s => GetComparableValue(s, column))
                : subscribers.OrderByDescending(s => GetComparableValue(s, column));
        }

        private IOrderedEnumerable<Subscriber> ApplyThenBy(
            IOrderedEnumerable<Subscriber> ordered,
            string column,
            bool ascending)
        {
            return ascending
                ? ordered.ThenBy(s => GetComparableValue(s, column))
                : ordered.ThenByDescending(s => GetComparableValue(s, column));
        }

        private object GetComparableValue(Subscriber s, string column)
        {
            column = column.ToLower();
            return column switch
            {
                "référence" => s.Ref_Subscriber,
                "nom" or "fullname" => s.Name_User,
                "email" => s.Adresse_Mail,
                "fidélité" => s.Fidelity,
                "emprunts" => s.ActiveLoansCount,
                "peut emprunter" => s.CanBorrow,
                "date d'inscription" => s.Date_Creation,
                "téléphone" => s.Num_Telephone,
                "adresse" => s.Adresse,
                _ => s.Name_User
            };
        }

        #endregion
    }
}
