using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CommunityToolkit.Mvvm.ComponentModel;

namespace LIBBRARY_MANAGER.Model
{
    public class Book : ObservableObject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long BookId { get; set; }

        [Required, MaxLength(150)]
        public string Title { get; set; } = null!;

        [Required, MaxLength(100)]
        public string Author { get; set; } = null!;

        [Required, MaxLength(50)]
        public string ISBN { get; set; } = null!;

        [MaxLength(50)]
        public string? Category { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        private DateTime? _publishDate;
        public DateTime? PublishDate
        {
            get => _publishDate;
            set
            {
                if (value.HasValue)
                {
                    _publishDate = new DateTime(value.Value.Year, 1, 1);
                }
                else
                {
                    _publishDate = null;
                }
            }
        }

        [MaxLength(500)]
        public string? Publisher { get; set; }

        [MaxLength(50)]
        public string? Language { get; set; } = "Français";

        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; } = 1;

        [Required]
        public bool IsAvailable { get; set; } = true;

        public DateTime DateAdded { get; private set; }

        [Required]
        public bool IsRemoved { get; set; } = false;

        [MaxLength(500)]
        public string? CoverUrl { get; set; }

        // Propriétés calculées (non mappées en BDD)
        [NotMapped]
        public bool IsNew => DateAdded > DateTime.Now.AddDays(-30);

        [NotMapped]
        public bool IsLowStock => Quantity > 0 && Quantity <= 3;

        [NotMapped]
        public string AvailabilityStatus
        {
            get
            {
                if (Quantity == 0) return "Rupture";
                if (Quantity <= 3) return "Stock faible";
                if (Quantity <= 10) return "Disponible";
                return "En stock";
            }
        }

        public enum CategoryAllowed
        {
            // 🎯 Toutes les catégories
            Tous,

            // 🧠 Sciences & Savoirs
            Science,
            Technologie,
            Informatique,
            Médecine,
            Mathématiques,
            Philosophie,
            Psychologie,
            Éducation,

            // 📚 Littérature
            Roman,
            Nouvelle,
            Poésie,
            Théâtre,
            Biographie,
            Essai,
            Classique,

            // 🌍 Histoire & Société
            Histoire,
            Politique,
            Économie,
            Société,
            Droit,
            Religion,
            Géographie,

            // 🎨 Arts & Culture
            Art,
            Musique,
            Cinéma,
            Architecture,
            Design,
            Photographie,

            // 🌿 Nature & Vie pratique
            Nature,
            Environnement,
            Cuisine,
            Jardinage,
            Santé,
            Sport,
            Voyage,

            // 👶 Jeunesse & Divertissement
            Jeunesse,
            Manga,
            BandeDessinee,
            Fantasy,
            ScienceFiction,
            Policier,
            Aventure,
            Romance,
            Horreur
        }

        public Book()
        {
            DateAdded = DateTime.Now;
        }

        /// <summary>
        /// Tente de décrémenter la quantité (emprunt)
        /// </summary>
        public bool TryDecreaseQuantity()
        {
            if (Quantity > 0)
            {
                Quantity--;
                IsAvailable = Quantity > 0;
                return true;
            }
            IsAvailable = false;
            return false;
        }

        /// <summary>
        /// Incrémente la quantité (retour/ajout)
        /// </summary>
        public void IncreaseQuantity(int count = 1)
        {
            if (count <= 0)
                throw new ArgumentException("Le nombre doit être positif.", nameof(count));

            Quantity += count;
            IsAvailable = true;
        }

        /// <summary>
        /// Clone le livre (utile pour édition)
        /// </summary>
        public Book Clone()
        {
            return new Book
            {
                BookId = this.BookId,
                Title = this.Title,
                Author = this.Author,
                ISBN = this.ISBN,
                Category = this.Category,
                Description = this.Description,
                PublishDate = this.PublishDate,
                Publisher = this.Publisher,
                Language = this.Language,
                Quantity = this.Quantity,
                IsAvailable = this.IsAvailable,
                DateAdded = this.DateAdded,
                IsRemoved = this.IsRemoved,
                CoverUrl = this.CoverUrl
            };
        }
    }
}