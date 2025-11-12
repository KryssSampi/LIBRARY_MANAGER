using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LIBBRARY_MANAGER.UI.Modules.BookCatalog.ViewModels;
using LIBBRARY_MANAGER.UI.Modules.Common.Views;
using System.Windows.Media;

namespace LIBBRARY_MANAGER.UI.Modules.BookCatalog.Views.MainView
{

    /// <summary>
    /// Vue spécialisée pour le catalogue de livres
    /// Utilise GenericMainView avec BookCatalogMainViewModel
    /// </summary>
    public class BookCatalogView : GenericMainView
    {
        public BookCatalogView() : base(new BookCatalogMainViewModel())
        {
            // Configuration spécifique aux livres
            ShowColumnHeaders = true; // Afficher les headers de colonnes
            NavigationForeground = new SolidColorBrush(Color.FromRgb(21, 101, 192)); // Bleu
        }

        public BookCatalogView(BookCatalogMainViewModel viewModel) : base(viewModel)
        {
            ShowColumnHeaders = true;
            NavigationForeground = new SolidColorBrush(Color.FromRgb(21, 101, 192));
        }
    }

}
