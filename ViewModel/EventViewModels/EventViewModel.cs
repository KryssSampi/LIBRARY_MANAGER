using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LIBBRARY_MANAGER.Data;
using LIBBRARY_MANAGER.Model;
using LIBBRARY_MANAGER.UI.Modules.EventCatalog.Items;
using LIBBRARY_MANAGER.Views.EventViews;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LIBBRARY_MANAGER.ViewModel.EventViewModels
{
    public class EventViewModel : ObservableObject
    {
        public ObservableCollection<EventViewModel> Events { get; } = new();

        public Event Model { get; }

        // Constructeur
        public EventViewModel(Event model)
        {
            Model = model;
        }

        // Constructeur
        public EventViewModel() { }

        // Properties
        public string Title => Model?.Title ?? string.Empty;

        public string Description =>
            string.IsNullOrWhiteSpace(Model?.Description)
            ? "Aucune description"
            : Model.Description;

        public DateTime StartDate => Model?.StartDate ?? DateTime.MinValue;
        public DateTime EndDate => Model?.EndDate ?? DateTime.MinValue;

        // Load from SQLite
        public void LoadEventsFromDatabase()
        {
            Events.Clear();

            using (var db = new LibraryDbContext())
            {
                var list = db.Events
                             .OrderBy(e => e.StartDate)
                             .ToList();

                foreach (var evt in list)
                    Events.Add(new EventViewModel(evt)); 
            }
        }
    }
}