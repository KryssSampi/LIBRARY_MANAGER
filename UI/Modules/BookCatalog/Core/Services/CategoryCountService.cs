using System;
using System.Collections.Generic;
using System.Linq;
using LIBBRARY_MANAGER.Model;
using static LIBBRARY_MANAGER.Model.Book;

namespace LIBBRARY_MANAGER.UI.Modules.BookCatalog.Core.Services
{
    /// <summary>
    /// Service de comptage des livres par catégorie (optimisé pour HomeView)
    /// ✅ CORRIGÉ : Namespace unique, logique "Tous" clarifiée
    /// </summary>
    public class CategoryCountService
    {
        /// <summary>
        /// Obtient le nombre de livres disponibles par catégorie
        /// </summary>
        public Dictionary<CategoryAllowed, int> GetCategoryCounts(IEnumerable<Book> books)
        {
            if (books == null || !books.Any())
                return new Dictionary<CategoryAllowed, int>();

            var availableBooks = books.Where(b => !b.IsRemoved).ToList();
            var counts = new Dictionary<CategoryAllowed, int>();

            foreach (CategoryAllowed category in Enum.GetValues(typeof(CategoryAllowed)))
            {
                if (category == CategoryAllowed.Tous)
                {
                    // ✅ "Tous" = total de tous les livres disponibles
                    counts[category] = availableBooks.Count;
                }
                else
                {
                    var categoryName = category.ToString();
                    var count = availableBooks.Count(b =>
                        b.Category?.Equals(categoryName, StringComparison.OrdinalIgnoreCase) ?? false
                    );
                    counts[category] = count;
                }
            }

            return counts;
        }

        /// <summary>
        /// Obtient le nombre de livres pour une catégorie spécifique
        /// </summary>
        public int GetCategoryCount(IEnumerable<Book> books, CategoryAllowed category)
        {
            if (books == null || !books.Any())
                return 0;

            var availableBooks = books.Where(b => !b.IsRemoved);

            if (category == CategoryAllowed.Tous)
            {
                return availableBooks.Count();
            }

            var categoryName = category.ToString();
            return availableBooks.Count(b =>
                b.Category?.Equals(categoryName, StringComparison.OrdinalIgnoreCase) ?? false
            );
        }

        /// <summary>
        /// Obtient les catégories avec au moins 1 livre
        /// </summary>
        public IEnumerable<CategoryAllowed> GetNonEmptyCategories(IEnumerable<Book> books)
        {
            var counts = GetCategoryCounts(books);
            return counts.Where(kvp => kvp.Value > 0).Select(kvp => kvp.Key);
        }

        /// <summary>
        /// Obtient les statistiques complètes
        /// </summary>
        public CategoryStatistics GetStatistics(IEnumerable<Book> books)
        {
            var availableBooks = books?.Where(b => !b.IsRemoved).ToList() ?? new List<Book>();
            var counts = GetCategoryCounts(availableBooks);

            return new CategoryStatistics
            {
                TotalBooks = availableBooks.Count,
                TotalCategories = counts.Count(kvp => kvp.Value > 0 && kvp.Key != CategoryAllowed.Tous),
                MostPopularCategory = counts
                    .Where(kvp => kvp.Key != CategoryAllowed.Tous)
                    .OrderByDescending(kvp => kvp.Value)
                    .FirstOrDefault().Key,
                CategoryCounts = counts,
                AvailableBooksCount = availableBooks.Count(b => b.IsAvailable),
                UnavailableBooksCount = availableBooks.Count(b => !b.IsAvailable)
            };
        }
    }

    /// <summary>
    /// Statistiques des catégories
    /// </summary>
    public class CategoryStatistics
    {
        public int TotalBooks { get; set; }
        public int TotalCategories { get; set; }
        public CategoryAllowed MostPopularCategory { get; set; }
        public Dictionary<CategoryAllowed, int> CategoryCounts { get; set; } = new();
        public int AvailableBooksCount { get; set; }
        public int UnavailableBooksCount { get; set; }
    }
}