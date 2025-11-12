using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LIBBRARY_MANAGER.Model;
using LIBBRARY_MANAGER.UI.Common.Items.MainView;
using LIBBRARY_MANAGER.UI.Modules.BookCatalog.Items;
using static global::LIBBRARY_MANAGER.Model.Book;



namespace LIBBRARY_MANAGER.UI.Modules.BookCatalog.Core.Services
{

        /// <summary>
        /// Service de filtrage dynamique haute performance
        /// </summary>
        public class BookFilterService
        {
            /// <summary>
            /// Applique tous les filtres actifs
            /// </summary>
            public IEnumerable<Book> ApplyFilters(
                IEnumerable<Book> books,
                IEnumerable<FilterLabelViewModel> activeFilters)
            {
                if (books == null || !books.Any())
                    return Enumerable.Empty<Book>();

                if (activeFilters == null || !activeFilters.Any())
                    return books;



                var filtered = books;

                foreach (var filter in activeFilters)
                {
                    filtered = ApplySingleFilter(filtered, filter);
                }

                return filtered;
            }

            /// <summary>
            /// Applique un filtre unique
            /// </summary>
            private IEnumerable<Book> ApplySingleFilter(IEnumerable<Book> books, FilterLabelViewModel filter)
            {
                var filterType = filter.Type?.ToLowerInvariant();
                var filterValue = filter.Value?.ToLowerInvariant();

                return filterType switch
                {
                    "catégorie" or "category" => FilterByCategory(books, filterValue),
                    "auteur" or "author" => FilterByAuthor(books, filterValue),
                    "titre" or "title" => FilterByTitle(books, filterValue),
                    "éditeur" or "publisher" => FilterByPublisher(books, filterValue),
                    "date de publication" or "publishdate" => FilterByPublishDate(books, filterValue),
                    "langue" or "language" => FilterByLanguage(books, filterValue),
                    "quantité" or "quantity" => FilterByQuantity(books, filterValue),
                    "disponibilité" or "availability" => FilterByAvailability(books, filterValue),
                    _ => books
                };
            }

            #region Filtres spécifiques

            public IEnumerable<Book> FilterByCategory(IEnumerable<Book> books, string? categoryValue)
            {
                if (string.IsNullOrWhiteSpace(categoryValue))
                    return books;

                return books.Where(b =>
                    b.Category?.Equals(categoryValue, StringComparison.OrdinalIgnoreCase) ?? false
                );
            }

            public IEnumerable<Book> FilterByAuthor(IEnumerable<Book> books, string? authorFilter)
            {
                if (string.IsNullOrWhiteSpace(authorFilter))
                    return books;

                return authorFilter switch
                {
                    "nom de famille a–m" => books.Where(b =>
                        !string.IsNullOrEmpty(b.Author) &&
                        char.ToUpper(b.Author[0]) >= 'A' &&
                        char.ToUpper(b.Author[0]) <= 'M'
                    ),
                    "nom de famille n–z" => books.Where(b =>
                        !string.IsNullOrEmpty(b.Author) &&
                        char.ToUpper(b.Author[0]) >= 'N' &&
                        char.ToUpper(b.Author[0]) <= 'Z'
                    ),
                    _ => books.Where(b =>
                        b.Author?.ToLowerInvariant().Contains(authorFilter) ?? false
                    )
                };
            }

            public IEnumerable<Book> FilterByTitle(IEnumerable<Book> books, string? titleFilter)
            {
                if (string.IsNullOrWhiteSpace(titleFilter))
                    return books;

                return titleFilter switch
                {
                    "commence par a–e" => books.Where(b =>
                        !string.IsNullOrEmpty(b.Title) &&
                        char.ToUpper(b.Title[0]) >= 'A' &&
                        char.ToUpper(b.Title[0]) <= 'E'
                    ),
                    "commence par f–j" => books.Where(b =>
                        !string.IsNullOrEmpty(b.Title) &&
                        char.ToUpper(b.Title[0]) >= 'F' &&
                        char.ToUpper(b.Title[0]) <= 'J'
                    ),
                    "commence par k–o" => books.Where(b =>
                        !string.IsNullOrEmpty(b.Title) &&
                        char.ToUpper(b.Title[0]) >= 'K' &&
                        char.ToUpper(b.Title[0]) <= 'O'
                    ),
                    "commence par p–t" => books.Where(b =>
                        !string.IsNullOrEmpty(b.Title) &&
                        char.ToUpper(b.Title[0]) >= 'P' &&
                        char.ToUpper(b.Title[0]) <= 'T'
                    ),
                    "commence par u–z" => books.Where(b =>
                        !string.IsNullOrEmpty(b.Title) &&
                        char.ToUpper(b.Title[0]) >= 'U' &&
                        char.ToUpper(b.Title[0]) <= 'Z'
                    ),
                    _ => books.Where(b =>
                        b.Title?.ToLowerInvariant().Contains(titleFilter) ?? false
                    )
                };
            }

            public IEnumerable<Book> FilterByPublisher(IEnumerable<Book> books, string? publisherFilter)
            {
                if (string.IsNullOrWhiteSpace(publisherFilter))
                    return books;

                if (publisherFilter == "autres")
                {
                    var knownPublishers = new[] { "gallimard", "flammarion", "actes sud", "penguin random house" };
                    return books.Where(b =>
                        !string.IsNullOrEmpty(b.Publisher) &&
                        !knownPublishers.Any(kp => b.Publisher.ToLowerInvariant().Contains(kp))
                    );
                }

                return books.Where(b =>
                    b.Publisher?.ToLowerInvariant().Contains(publisherFilter) ?? false
                );
            }

            public IEnumerable<Book> FilterByPublishDate(IEnumerable<Book> books, string? dateFilter)
            {
                if (string.IsNullOrWhiteSpace(dateFilter))
                    return books;

                return dateFilter switch
                {
                    "avant 2000" => books.Where(b =>
                        b.PublishDate.HasValue && b.PublishDate.Value.Year < 2000
                    ),
                    "2000–2010" => books.Where(b =>
                        b.PublishDate.HasValue &&
                        b.PublishDate.Value.Year >= 2000 &&
                        b.PublishDate.Value.Year <= 2010
                    ),
                    "2011–2020" => books.Where(b =>
                        b.PublishDate.HasValue &&
                        b.PublishDate.Value.Year >= 2011 &&
                        b.PublishDate.Value.Year <= 2020
                    ),
                    "depuis 2021" => books.Where(b =>
                        b.PublishDate.HasValue && b.PublishDate.Value.Year >= 2021
                    ),
                    _ => books
                };
            }

            public IEnumerable<Book> FilterByLanguage(IEnumerable<Book> books, string? languageFilter)
            {
                if (string.IsNullOrWhiteSpace(languageFilter))
                    return books;

                if (languageFilter == "autre")
                {
                    var knownLanguages = new[] { "français", "anglais", "espagnol" };
                    return books.Where(b =>
                        !string.IsNullOrEmpty(b.Language) &&
                        !knownLanguages.Any(kl => b.Language.ToLowerInvariant().Contains(kl))
                    );
                }

                return books.Where(b =>
                    b.Language?.ToLowerInvariant().Contains(languageFilter) ?? false
                );
            }

            public IEnumerable<Book> FilterByQuantity(IEnumerable<Book> books, string? quantityFilter)
            {
                if (string.IsNullOrWhiteSpace(quantityFilter))
                    return books;

                return quantityFilter switch
                {
                    "0 – en rupture" => books.Where(b => b.Quantity == 0),
                    "1 à 5 exemplaires" => books.Where(b => b.Quantity >= 1 && b.Quantity <= 5),
                    "6 à 10 exemplaires" => books.Where(b => b.Quantity >= 6 && b.Quantity <= 10),
                    "11 et plus" => books.Where(b => b.Quantity >= 11),
                    _ => books
                };
            }

            public IEnumerable<Book> FilterByAvailability(IEnumerable<Book> books, string? availabilityFilter)
            {
                if (string.IsNullOrWhiteSpace(availabilityFilter))
                    return books;

                return availabilityFilter switch
                {
                    "disponible" => books.Where(b => b.IsAvailable),
                    "indisponible" or "rupture" => books.Where(b => !b.IsAvailable),
                    "stock faible" => books.Where(b => b.IsLowStock),
                    _ => books
                };
            }

            #endregion

            /// <summary>
            /// Filtre par catégorie (enum)
            /// </summary>
            public IEnumerable<Book> FilterByCategoryEnum(IEnumerable<Book> books, CategoryAllowed category)
        {
            var categoryName = category.ToString();
            //  Si "Tous", retourner tous les livres
            if (category == CategoryAllowed.Tous)
            {
                return books.Where(b => !b.IsRemoved);
            }
            return books.Where(b =>
                    b.Category?.Equals(categoryName, StringComparison.OrdinalIgnoreCase) ?? false
                );
            }

            /// <summary>
            /// Obtient uniquement les livres disponibles
            /// </summary>
            public IEnumerable<Book> GetAvailableBooks(IEnumerable<Book> books)
            {
                return books.Where(b => b.IsAvailable && !b.IsRemoved);
            }

            /// <summary>
            /// Obtient uniquement les nouveaux livres (ajoutés récemment)
            /// </summary>
            public IEnumerable<Book> GetNewBooks(IEnumerable<Book> books, int daysSinceAdded = 30)
            {
                var cutoffDate = DateTime.Now.AddDays(-daysSinceAdded);
                return books.Where(b => b.DateAdded >= cutoffDate);
            }
        }
    }