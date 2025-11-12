using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LIBBRARY_MANAGER.UI.Modules.BookCatalog.Helpers;
using static LIBBRARY_MANAGER.Model.Book;

namespace LIBBRARY_MANAGER.UI.Modules.BookCatalog.Views.HomeView
{
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
            InitializeComponent();

            string relativePath = "UI/Common/Assets/BookCatalog_Images/HomeView_Background.png";
            Image_Background.ImageSource = GetPathHelpers.GetPathImagesource(relativePath);

            InitializeCategoryGrid();
        }

        #region Initialization

        private void InitializeCategoryGrid()
        {
            // Abonnement à l'event de sélection de catégorie
            CategoriesGrid.OnCategorySelected += HandleCategorySelected;

            //  Charger les données depuis la base de données
            Loaded += (s, e) => LoadCategoryData();
        }

        #endregion

        #region Data Loading

        /// <summary>
        /// ✅ CORRECTION : Charge les compteurs depuis App.LibraryDbContext
        /// </summary>
        private void LoadCategoryData()
        {
            try
            {
                // Charger depuis la base de données
                CategoriesGrid.LoadCountsFromDatabase();

                System.Diagnostics.Debug.WriteLine("[HomeView] ✅ Compteurs de catégories chargés depuis la base de données");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[HomeView] ❌ Erreur chargement catégories: {ex.Message}");

                MessageBox.Show(
                    $"Erreur lors du chargement des catégories:\n{ex.Message}",
                    "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        #endregion

        #region Virtual Methods - Category Selection

        /// <summary>
        /// Méthode virtuelle appelée lors de la sélection d'une catégorie
        /// </summary>
        protected virtual void OnCategorySelected(CategoryAllowed category)
        {
            System.Diagnostics.Debug.WriteLine($"[HomeView] 📂 Catégorie sélectionnée: {category}");

            // Cette méthode sera surchargée par BookCatalog.xaml.cs
            // pour naviguer vers la MainView avec le filtre de catégorie
        }

        #endregion

        /// <summary>
        /// Gère la sélection d'une catégorie depuis le CategoryGrid
        /// </summary>
        private void HandleCategorySelected(CategoryAllowed category)
        {
            OnCategorySelected(category);
        }

        /// <summary>
        /// ✅ NOUVELLE MÉTHODE : Rafraîchit les compteurs depuis la base
        /// </summary>
        public void RefreshCategoryCounts()
        {
            LoadCategoryData();
        }

        private void OnSearchRequested(string obj)
        {

        }
    }
}