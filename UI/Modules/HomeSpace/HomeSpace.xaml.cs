using System.Windows;
using System.Windows.Controls;
using LIBBRARY_MANAGER.Model;
using LIBBRARY_MANAGER.UI.Modules.BookCatalog.Helpers;
using LIBBRARY_MANAGER.UI.Modules.HomeSpace.ViewModel;

namespace LIBBRARY_MANAGER.UI.Modules.HomeSpace
{
    /// <summary>
    /// ✅ VERSION SIMPLIFIÉE - Sans polling, juste MVVM pur
    /// </summary>
    public partial class HomeSpace : UserControl
    {
        public HomeViewModel HomeVM { get; set; }
        public OperationsViewModel OperationsVM { get; }

        public HomeSpace(StaffMember? member = null)
        {
            InitializeComponent();

            var currentStaff = member ?? App._currentconnected_User;

            // ✅ Initialiser les ViewModels
            HomeVM = new HomeViewModel(currentStaff);
            OperationsVM = new OperationsViewModel(currentStaff);

            // ✅ Set DataContext
            DataContext = this;

            // Images
            Image1.Source = GetPathHelpers.GetPathImagesource("/UI/Modules/HomeSpace/Assets/image1.png");
            Image2.Source = GetPathHelpers.GetPathImagesource("/UI/Modules/HomeSpace/Assets/image2.png");

            // ✅ Charger les opérations au démarrage
            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Charger les opérations
            OperationsVM.LoadTodayOperations();
        }

        /// <summary>
        /// ✅ Méthode publique pour rafraîchir depuis l'extérieur
        /// (ex: après avoir créé un emprunt/modification)
        /// </summary>
        public void RefreshOperations()
        {
            OperationsVM.Refresh();
        }
    }
}