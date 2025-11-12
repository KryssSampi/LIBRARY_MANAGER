using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using global::LIBBRARY_MANAGER.UI.Modules.BookCatalog.Items;
using global::LIBBRARY_MANAGER.ViewModel.SubscribersViewModel;
using LIBBRARY_MANAGER.UI.Modules.Common.Views;
using LIBBRARY_MANAGER.UI.Modules.SubscriberCatalog.ViewModels;

namespace LIBBRARY_MANAGER.UI.Common.Modules.SubscribersCatalog.Views
{

    /// <summary>
    /// Vue principale pour le catalogue des abonnés
    /// Affiche la liste des abonnés avec recherche, tri et filtrage
    /// </summary>
    /// <summary>
    /// Vue spécialisée pour le catalogue d'abonnés
    /// Utilise GenericMainView avec SubscribersCatalogViewModel
    /// </summary>
    public class SubscribersCatalogView : GenericMainView
    {
        public SubscribersCatalogView() : base(new SubscribersCatalogViewModel())
        {
            // Configuration spécifique aux abonnés
  
            ShowColumnHeaders = true; // Afficher les headers
            NavigationForeground = new SolidColorBrush(Color.FromRgb(76, 175, 80)); // Vert
        }

        public SubscribersCatalogView(SubscribersCatalogViewModel viewModel) : base(viewModel)
        {
            ShowColumnHeaders = true;
            NavigationForeground = new SolidColorBrush(Color.FromRgb(76, 175, 80));
        }
    }
}