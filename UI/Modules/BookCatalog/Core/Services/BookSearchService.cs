using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using  LIBBRARY_MANAGER.Model;

namespace LIBBRARY_MANAGER.UI.Modules.BookCatalog.Core.Services
{
    
        /// <summary>
        /// Service de recherche multi-champs ultra-performant
        /// </summary>
        public class BookSearchService
        {
            /// <summary>
            /// Recherche dans tous les champs pertinents
            /// </summary>
            public IEnumerable<Book> Search(IEnumerable<Book> books, string query)
            {
                if (string.IsNullOrWhiteSpace(query))
                    return books;

                var searchTerms = query.ToLowerInvariant()
                    .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                return books.Where(book => searchTerms.All(term =>
                    (book.Title?.ToLowerInvariant().Contains(term) ?? false) ||
                    (book.Author?.ToLowerInvariant().Contains(term) ?? false) ||
                    (book.ISBN?.ToLowerInvariant().Contains(term) ?? false) ||
                    (book.Category?.ToLowerInvariant().Contains(term) ?? false) ||
                    (book.Publisher?.ToLowerInvariant().Contains(term) ?? false) ||
                    (book.Description?.ToLowerInvariant().Contains(term) ?? false)
                )).ToList();
            }

            /// <summary>
            /// Recherche par titre uniquement
            /// </summary>
            public IEnumerable<Book> SearchByTitle(IEnumerable<Book> books, string title)
            {
                if (string.IsNullOrWhiteSpace(title))
                    return books;

                var term = title.ToLowerInvariant();
                return books.Where(b => b.Title?.ToLowerInvariant().Contains(term) ?? false);
            }

            /// <summary>
            /// Recherche par auteur uniquement
            /// </summary>
            public IEnumerable<Book> SearchByAuthor(IEnumerable<Book> books, string author)
            {
                if (string.IsNullOrWhiteSpace(author))
                    return books;

                var term = author.ToLowerInvariant();
                return books.Where(b => b.Author?.ToLowerInvariant().Contains(term) ?? false);
            }

            /// <summary>
            /// Recherche par ISBN
            /// </summary>
            public Book? SearchByISBN(IEnumerable<Book> books, string isbn)
            {
                if (string.IsNullOrWhiteSpace(isbn))
                    return null;

                var cleanIsbn = isbn.Replace("-", "").Replace(" ", "");
                return books.FirstOrDefault(b =>
                    b.ISBN?.Replace("-", "").Replace(" ", "")
                    .Equals(cleanIsbn, StringComparison.OrdinalIgnoreCase) ?? false
                );
            }

            /// <summary>
            /// Suggestions de recherche (fuzzy matching basique)
            /// </summary>
            public IEnumerable<string> GetSuggestions(IEnumerable<Book> books, string query, int maxResults = 5)
            {
                if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
                    return Enumerable.Empty<string>();

                var term = query.ToLowerInvariant();
                var suggestions = new HashSet<string>();

                // Suggestions depuis titres
                suggestions.UnionWith(books
                    .Where(b => b.Title?.ToLowerInvariant().Contains(term) ?? false)
                    .Select(b => b.Title)
                    .Take(maxResults));

                // Suggestions depuis auteurs
               if (suggestions.Count < maxResults)
                {
                    suggestions.UnionWith(books
                        .Where(b => b.Author?.ToLowerInvariant().Contains(term) ?? false)
                        .Select(b => b.Author)
                        .Take(maxResults - suggestions.Count));
                }

                return suggestions.Take(maxResults);
            }
        }
    }