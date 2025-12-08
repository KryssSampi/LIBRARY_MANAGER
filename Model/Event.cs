using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LIBBRARY_MANAGER.Model
{
    public class Event : ObservableObject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long EventId { get; set; }

        [Required, MaxLength(150)]
        public string Title { get; set; } = null!;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public DateTime DateAdded { get; private set; } = DateTime.Now;

        [NotMapped]
        public bool IsUpcoming => StartDate > DateTime.Now;

        [NotMapped]
        public bool IsOngoing => StartDate <= DateTime.Now && EndDate >= DateTime.Now;

        [NotMapped]
        public bool IsFinished => EndDate < DateTime.Now;

        public Event() { }
    }
}
