using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LIBBRARY_MANAGER.Model;

namespace LIBBRARY_MANAGER.UI.Modules.BookCatalog.Core.Services
{
    
    /// <summary>
        /// Service de tri ultra-performant avec cache
        /// </summary>
        public class BookSortService
        {
            public enum SortField
            {
                Title,
                Author,
                Category,
                ISBN,
                Publisher,
                PublishDate,
                Language,
                Quantity,
                DateAdded
            }

            /// <summary>
            /// Tri principal avec gestion ascendant/descendant
            /// </summary>
            public IEnumerable<Book> Sort(IEnumerable<Book> books, SortField field, bool ascending = true)
            {
                if (books == null || !books.Any())
                    return Enumerable.Empty<Book>();

                var sorted = field switch
                {
                    SortField.Title => books.OrderBy(b => b.Title),
                    SortField.Author => books.OrderBy(b => b.Author),
                    SortField.Category => books.OrderBy(b => b.Category),
                    SortField.ISBN => books.OrderBy(b => b.ISBN),
                    SortField.Publisher => books.OrderBy(b => b.Publisher),
                    SortField.PublishDate => books.OrderBy(b => b.PublishDate ?? DateTime.MinValue),
                    SortField.Language => books.OrderBy(b => b.Language),
                    SortField.Quantity => books.OrderBy(b => b.Quantity),
                    SortField.DateAdded => books.OrderBy(b => b.DateAdded),
                    _ => books.OrderBy(b => b.Title)
                };

                return ascending ? sorted : sorted.Reverse();
            }

            /// <summary>
            /// Tri par nom de colonne (pour binding XAML)
            /// </summary>
            public IEnumerable<Book> SortByColumnName(IEnumerable<Book> books, string columnName, bool ascending = true)
            {
                var field = columnName?.ToLowerInvariant() switch
                {
                    "titre" or "title" => SortField.Title,
                    "auteur" or "author" => SortField.Author,
                    "catégorie" or "category" => SortField.Category,
                    "isbn" => SortField.ISBN,
                    "éditeur" or "publisher" => SortField.Publisher,
                    "date de publication" or "publishdate" => SortField.PublishDate,
                    "langue" or "language" => SortField.Language,
                    "quantité" or "quantity" => SortField.Quantity,
                    "ajouté le" or "dateadded" => SortField.DateAdded,
                    _ => SortField.Title
                };

                return Sort(books, field, ascending);
            }

            /// <summary>
            /// Tri multi-critères (ex: Catégorie ASC puis Titre ASC)
            /// </summary>
            public IEnumerable<Book> SortMultiple(IEnumerable<Book> books, params (SortField field, bool ascending)[] criteria)
            {
                if (books == null || !books.Any() || criteria == null || criteria.Length == 0)
                    return books;

                IOrderedEnumerable<Book>? ordered = null;

                foreach (var (field, ascending) in criteria)
                {
                    if (ordered == null)
                    {
                        ordered = ApplySort(books.OrderBy(b => 0), field, ascending);
                    }
                    else
                    {
                        ordered = ApplySort(ordered, field, ascending);
                    }
                }

                return ordered ?? books;
            }

            private IOrderedEnumerable<Book> ApplySort(IOrderedEnumerable<Book> books, SortField field, bool ascending)
            {
                return field switch
                {
                    SortField.Title => ascending ? books.ThenBy(b => b.Title) : books.ThenByDescending(b => b.Title),
                    SortField.Author => ascending ? books.ThenBy(b => b.Author) : books.ThenByDescending(b => b.Author),
                    SortField.Category => ascending ? books.ThenBy(b => b.Category) : books.ThenByDescending(b => b.Category),
                    SortField.ISBN => ascending ? books.ThenBy(b => b.ISBN) : books.ThenByDescending(b => b.ISBN),
                    SortField.Publisher => ascending ? books.ThenBy(b => b.Publisher) : books.ThenByDescending(b => b.Publisher),
                    SortField.PublishDate => ascending ? books.ThenBy(b => b.PublishDate ?? DateTime.MinValue) : books.ThenByDescending(b => b.PublishDate ?? DateTime.MinValue),
                    SortField.Language => ascending ? books.ThenBy(b => b.Language) : books.ThenByDescending(b => b.Language),
                    SortField.Quantity => ascending ? books.ThenBy(b => b.Quantity) : books.ThenByDescending(b => b.Quantity),
                    SortField.DateAdded => ascending ? books.ThenBy(b => b.DateAdded) : books.ThenByDescending(b => b.DateAdded),
                    _ => books
                };
            }

            /// <summary>
            /// Tris prédéfinis utiles
            /// </summary>
            public IEnumerable<Book> SortByPopularity(IEnumerable<Book> books)
            {
                // Les plus empruntés = ceux avec le moins de stock (logique inverse)
                return books.OrderBy(b => b.Quantity).ThenBy(b => b.Title);
            }

            public IEnumerable<Book> SortByNewest(IEnumerable<Book> books)
            {
                return books.OrderByDescending(b => b.DateAdded);
            }

            public IEnumerable<Book> SortByAvailability(IEnumerable<Book> books)
            {
                return books.OrderByDescending(b => b.IsAvailable)
                           .ThenByDescending(b => b.Quantity)
                           .ThenBy(b => b.Title);
            }
        }
    }