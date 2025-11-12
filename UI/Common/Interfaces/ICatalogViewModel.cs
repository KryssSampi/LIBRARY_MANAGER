using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using LIBBRARY_MANAGER.UI.Common.Items.MainView;
using LIBBRARY_MANAGER.UI.Modules.BookCatalog.Items;
using LIBBRARY_MANAGER.UI.Modules.BookCatalog.ViewModels;


namespace LIBBRARY_MANAGER.UI.Modules.Common.Interfaces
    {
        /// <summary>
        /// Interface commune pour tous les ViewModels de catalogue
        /// Permet de créer une vue MainView réutilisable
        /// </summary>
        public interface ICatalogViewModel : INotifyPropertyChanged
        {
            #region Collections

            /// <summary>
            /// Headers des colonnes pour le mode liste
            /// </summary>
            ObservableCollection<ColumnHeaderViewModel> Headers { get; }

            /// <summary>
            /// Filtres actifs
            /// </summary>
            ObservableCollection<FilterLabelViewModel> ActiveFilters { get; }

            /// <summary>
            /// Items affichés (générique, chaque ViewModel fournit ses propres items)
            /// </summary>
            System.Collections.IEnumerable DisplayedItems { get; }

            #endregion

            #region Propriétés de recherche et filtrage

            /// <summary>
            /// Texte de recherche
            /// </summary>
            string SearchText { get; set; }

            /// <summary>
            /// Indique si des filtres sont actifs
            /// </summary>
            bool HasFilters { get; }

            /// <summary>
            /// Nombre total de résultats
            /// </summary>
            int TotalResults { get; }

        #endregion

        #region Propriétés d'affichage

        /// <summary>
        /// Mode de vue actuel (pour les catalogues qui supportent plusieurs vues)
        /// </summary>
        ViewMode _currentviewmode { get; }

        string CurrentViewMode => _currentviewmode.ToString();

        bool IsListMode => _currentviewmode == ViewMode.List;
        
            /// <summary>
            /// Titre du catalogue (ex: "Catalogue de Livres", "Liste des Abonnés")
            /// </summary>
         string CatalogTitle { get; }

        /// <summary>
        /// Message pour les résultats vides
        /// </summary>
        string EmptyMessage { get; }

        /// <summary>
        /// Nom de l'item traité par le catalog
        /// </summary>
        string ItemsName { get; }

        #endregion

        #region Commandes

        /// <summary>
        /// Commande de recherche
        /// </summary>
        ICommand SearchCommand { get; }

            /// <summary>
            /// Commande pour effacer la recherche
            /// </summary>
            ICommand ClearSearchCommand { get; }

            /// <summary>
            /// Commande de tri
            /// </summary>
            ICommand SortCommand { get; }

            /// <summary>
            /// Commande pour supprimer un filtre
            /// </summary>
            ICommand RemoveFilterCommand { get; }

            /// <summary>
            /// Commande de rechargement
            /// </summary>
            ICommand ReloadCommand { get; }

            /// <summary>
            /// Commande de retour
            /// </summary>
            ICommand BackCommand { get; }
        ICommand ToggleViewModeCommand { get; }

        #endregion

        #region Méthodes

        /// <summary>
        /// Initialise le ViewModel
        /// </summary>
        void Initialize();

            /// <summary>
            /// Nettoie les ressources
            /// </summary>
            void Cleanup();

            /// <summary>
            /// Crée un élément UI pour un item (délégué à chaque ViewModel)
            /// </summary>
            System.Windows.UIElement CreateDisplayItem(object item);

            #endregion
        }
    }