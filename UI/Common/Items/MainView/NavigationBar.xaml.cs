using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LIBBRARY_MANAGER.UI.Common.Items;
using MahApps.Metro.IconPacks;

namespace LIBBRARY_MANAGER.UI.Common.Items.MainView
{
 
        /// <summary>
        /// Barre de navigation du catalogue
        /// Fournit : retour, recherche, changement de vue, filtres, reload
        /// </summary>
        public partial class NavigationBar : UserControl
        {
        #region Dependency Properties

        #region Dependency Properties (Visibilité des boutons)

        public bool IsBackButtonVisible
        {
            get => (bool)GetValue(IsBackButtonVisibleProperty);
            set => SetValue(IsBackButtonVisibleProperty, value);
        }
        public static readonly DependencyProperty IsBackButtonVisibleProperty =
            DependencyProperty.Register(
                nameof(IsBackButtonVisible),
                typeof(bool),
                typeof(NavigationBar),
                new PropertyMetadata(true));

        public bool IsSearchButtonVisible
        {
            get => (bool)GetValue(IsSearchButtonVisibleProperty);
            set => SetValue(IsSearchButtonVisibleProperty, value);
        }
        public static readonly DependencyProperty IsSearchButtonVisibleProperty =
            DependencyProperty.Register(
                nameof(IsSearchButtonVisible),
                typeof(bool),
                typeof(NavigationBar),
                new PropertyMetadata(true));

        public bool IsToggleViewButtonVisible
        {
            get => (bool)GetValue(IsToggleViewButtonVisibleProperty);
            set => SetValue(IsToggleViewButtonVisibleProperty, value);
        }
        public static readonly DependencyProperty IsToggleViewButtonVisibleProperty =
            DependencyProperty.Register(
                nameof(IsToggleViewButtonVisible),
                typeof(bool),
                typeof(NavigationBar),
                new PropertyMetadata(true));

        public bool IsFilterButtonVisible
        {
            get => (bool)GetValue(IsFilterButtonVisibleProperty);
            set => SetValue(IsFilterButtonVisibleProperty, value);
        }
        public static readonly DependencyProperty IsFilterButtonVisibleProperty =
            DependencyProperty.Register(
                nameof(IsFilterButtonVisible),
                typeof(bool),
                typeof(NavigationBar),
                new PropertyMetadata(true));

        public bool IsReloadButtonVisible
        {
            get => (bool)GetValue(IsReloadButtonVisibleProperty);
            set => SetValue(IsReloadButtonVisibleProperty, value);
        }
        public static readonly DependencyProperty IsReloadButtonVisibleProperty =
            DependencyProperty.Register(
                nameof(IsReloadButtonVisible),
                typeof(bool),
                typeof(NavigationBar),
                new PropertyMetadata(true));

        #endregion

        /// <summary>
        /// Texte de recherche bindable
        /// </summary>
        public string SearchText
            {
                get => (string)GetValue(SearchTextProperty);
                set => SetValue(SearchTextProperty, value);
            }
            public static readonly DependencyProperty SearchTextProperty =
                DependencyProperty.Register(
                    nameof(SearchText),
                    typeof(string),
                    typeof(NavigationBar),
                    new PropertyMetadata(string.Empty));

            /// <summary>
            /// Indique si le mode grille est actif
            /// </summary>
            public bool IsGridMode
            {
                get => (bool)GetValue(IsGridModeProperty);
                set => SetValue(IsGridModeProperty, value);
            }
            public static readonly DependencyProperty IsGridModeProperty =
                DependencyProperty.Register(
                    nameof(IsGridMode),
                    typeof(bool),
                    typeof(NavigationBar),
                    new PropertyMetadata(false));

            /// <summary>
            /// Couleur du foreground pour les éléments (icônes, texte)
            /// </summary>
            public Brush ItemsForeground
            {
                get => (Brush)GetValue(ItemsForegroundProperty);
                set => SetValue(ItemsForegroundProperty, value);
            }
            public static readonly DependencyProperty ItemsForegroundProperty =
                DependencyProperty.Register(
                    nameof(ItemsForeground),
                    typeof(Brush),
                    typeof(NavigationBar),
                    new PropertyMetadata(new SolidColorBrush(Color.FromRgb(117, 117, 117)))); // #757575

            /// <summary>
            /// Couleur de fond du panneau de navigation
            /// </summary>
            public Brush PanelBackground
            {
                get => (Brush)GetValue(PanelBackgroundProperty);
                set => SetValue(PanelBackgroundProperty, value);
            }
            public static readonly DependencyProperty PanelBackgroundProperty =
                DependencyProperty.Register(
                    nameof(PanelBackground),
                    typeof(Brush),
                    typeof(NavigationBar),
                    new PropertyMetadata(Brushes.White));

            /// <summary>
            /// Commande exécutée au clic sur le bouton retour
            /// </summary>
            public ICommand BackCommand
            {
                get => (ICommand)GetValue(BackCommandProperty);
                set => SetValue(BackCommandProperty, value);
            }
            public static readonly DependencyProperty BackCommandProperty =
                DependencyProperty.Register(
                    nameof(BackCommand),
                    typeof(ICommand),
                    typeof(NavigationBar),
                    new PropertyMetadata(null));

            /// <summary>
            /// Commande exécutée au clic sur le bouton recherche
            /// </summary>
            public ICommand SearchCommand
            {
                get => (ICommand)GetValue(SearchCommandProperty);
                set => SetValue(SearchCommandProperty, value);
            }
            public static readonly DependencyProperty SearchCommandProperty =
                DependencyProperty.Register(
                    nameof(SearchCommand),
                    typeof(ICommand),
                    typeof(NavigationBar),
                    new PropertyMetadata(null));

            /// <summary>
            /// Commande exécutée au clic sur le bouton changement de vue
            /// </summary>
            public ICommand ToggleViewCommand
            {
                get => (ICommand)GetValue(ToggleViewCommandProperty);
                set => SetValue(ToggleViewCommandProperty, value);
            }
            public static readonly DependencyProperty ToggleViewCommandProperty =
                DependencyProperty.Register(
                    nameof(ToggleViewCommand),
                    typeof(ICommand),
                    typeof(NavigationBar),
                    new PropertyMetadata(null));

            /// <summary>
            /// Commande exécutée au clic sur le bouton reload
            /// </summary>
            public ICommand ReloadCommand
            {
                get => (ICommand)GetValue(ReloadCommandProperty);
                set => SetValue(ReloadCommandProperty, value);
            }
            public static readonly DependencyProperty ReloadCommandProperty =
                DependencyProperty.Register(
                    nameof(ReloadCommand),
                    typeof(ICommand),
                    typeof(NavigationBar),
                    new PropertyMetadata(null));

            #endregion

            #region Constructeur

            public NavigationBar()
            {
                InitializeComponent();

                // Initialisation des valeurs par défaut
                InitializeDefaults();

                // Événements
                KeyDown += OnKeyDown;
            }

            #endregion

            #region Méthodes privées

            /// <summary>
            /// Initialise les valeurs par défaut
            /// </summary>
            private void InitializeDefaults()
            {
            // Les valeurs par défaut sont définies dans les DependencyProperty
            // Pas besoin d'initialisation supplémentaire
            SwitchViewModeBtn.AddHandler(IconButton.ClickEvent, new RoutedEventHandler(SwitchIconKind));
           
            }

        private void SwitchIconKind(object sender, RoutedEventArgs e)
        {
            if (SwitchViewModeBtn.IconKind == PackIconMaterialKind.ViewGridOutline)
            {
                SwitchViewModeBtn.IconKind = PackIconMaterialKind.ViewList;
                return;
            }
            else if (SwitchViewModeBtn.IconKind == PackIconMaterialKind.ViewList)
            {
                SwitchViewModeBtn.IconKind = PackIconMaterialKind.ViewGridOutline;
                return;
            }
        }



        /// <summary>
        /// Gère la touche Entrée pour déclencher la recherche
        /// </summary>
        private void OnKeyDown(object sender, KeyEventArgs e)
            {
                if (e.Key == Key.Return && SearchCommand?.CanExecute(null) == true)
                {
                    SearchCommand.Execute(null);
                    e.Handled = true;
                }
            }

            #endregion

            #region Méthodes publiques

            /// <summary>
            /// Définit la couleur de foreground pour tous les éléments
            /// </summary>
            /// <param name="color">Couleur hex (ex: #757575)</param>
            public void SetItemsForegroundColor(string color)
            {
                try
                {
                    ItemsForeground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Erreur lors de la conversion de couleur: {ex.Message}");
                }
            }

            /// <summary>
            /// Définit la couleur de fond du panneau
            /// </summary>
            /// <param name="color">Couleur hex (ex: #FFFFFF)</param>
            public void SetPanelBackgroundColor(string color)
            {
                try
                {
                    PanelBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Erreur lors de la conversion de couleur: {ex.Message}");
                }
            }

            /// <summary>
            /// Focus sur la barre de recherche
            /// </summary>
            public void FocusSearchBox()
            {
                // Le focus sera sur le TextBox de la barre de recherche
            }

            /// <summary>
            /// Efface le texte de recherche
            /// </summary>
            public void ClearSearchText()
            {
                SearchText = string.Empty;
            }

            /// <summary>
            /// Définit le mode de vue
            /// </summary>
            /// <param name="isGridMode">true pour grille, false pour liste</param>
            public void SetViewMode(bool isGridMode)
            {
                IsGridMode = isGridMode;
            }

            #endregion

            #region Thèmes prédéfinis

            /// <summary>
            /// Applique le thème clair
            /// </summary>
            public void ApplyLightTheme()
            {
                PanelBackground = Brushes.White;
                ItemsForeground = new SolidColorBrush(Color.FromRgb(117, 117, 117)); // #757575
            }

            /// <summary>
            /// Applique le thème sombre
            /// </summary>
            public void ApplyDarkTheme()
            {
                PanelBackground = new SolidColorBrush(Color.FromRgb(33, 33, 33)); // #212121
                ItemsForeground = new SolidColorBrush(Color.FromRgb(255, 255, 255)); // #FFFFFF
            }

            /// <summary>
            /// Applique le thème bleu NOCTUA
            /// </summary>
            public void ApplyNoctuaTheme()
            {
                PanelBackground = new SolidColorBrush(Color.FromRgb(21, 101, 192)); // #1565C0
                ItemsForeground = Brushes.White;
            }

            #endregion
        }
    }
