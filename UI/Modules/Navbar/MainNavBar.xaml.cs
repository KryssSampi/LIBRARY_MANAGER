using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using MahApps.Metro.IconPacks;

namespace LIBBRARY_MANAGER.UI.Modules.Navbar
{
    public partial class ResponsiveNavBar : UserControl
    {
        #region Constants
        private const double BUTTON_HEIGHT = 56;
        private const double AVATAR_SECTION_HEIGHT = 90;
        private const double BOTTOM_SECTION_HEIGHT = 140;
        private const double SEPARATOR_HEIGHT = 25;
        private const double ANIMATION_DURATION = 0.3;
        #endregion

        #region Fields
        private Button _currentActiveButton;
        private bool _isExpanded = false;
        private bool _isInitialized = false;
        #endregion

        #region Constructor
        public ResponsiveNavBar()
        {
            InitializeComponent();
            InitializeNavItems();
            Loaded += OnLoaded;
        }
        #endregion

        #region Initialization
        private void InitializeNavItems()
        {
            AllNavItems = new ObservableCollection<NavItem>
            {
                new NavItem { Id = "Home", Label = "Accueil", IconKind = PackIconMaterialKind.Home },
                new NavItem { Id = "Catalog", Label = "Catalogue de livres", IconKind = PackIconMaterialKind.Bookshelf },
                new NavItem { Id = "Members", Label = "Liste des Abonnés", IconKind = PackIconMaterialKind.AccountGroup },
                new NavItem { Id = "BorrowManager", Label = "Gestionnaire des emprunts", IconKind = PackIconMaterialKind.BookCog },
                new NavItem { Id = "Borrow", Label = "Faire un Emprunt", IconKind = PackIconMaterialKind.BookArrowRight },
                new NavItem { Id = "Return", Label = "Retourner un Livre", IconKind = PackIconMaterialKind.BookArrowLeftOutline },
                new NavItem { Id = "Events", Label = "Événements", IconKind = PackIconMaterialKind.Calendar }
            };

            AccountManagerItem = new NavItem
            {
                Id = "Account",
                Label = "Mon Profil",
                IconKind = PackIconMaterialKind.AccountCog
            };

            LogoutItem = new NavItem
            {
                Id = "Logout",
                Label = "Déconnexion",
                IconKind = PackIconMaterialKind.LogoutVariant
            };

            VisibleNavItems = new ObservableCollection<NavItem>();
            HiddenNavItems = new ObservableCollection<NavItem>();
        }
        #endregion

        #region Dependency Properties

        public ObservableCollection<NavItem> AllNavItems { get; private set; }

        public ObservableCollection<NavItem> VisibleNavItems
        {
            get => (ObservableCollection<NavItem>)GetValue(VisibleNavItemsProperty);
            set => SetValue(VisibleNavItemsProperty, value);
        }
        public static readonly DependencyProperty VisibleNavItemsProperty =
            DependencyProperty.Register(nameof(VisibleNavItems), typeof(ObservableCollection<NavItem>),
                typeof(ResponsiveNavBar), new PropertyMetadata(null));

        public ObservableCollection<NavItem> HiddenNavItems
        {
            get => (ObservableCollection<NavItem>)GetValue(HiddenNavItemsProperty);
            set => SetValue(HiddenNavItemsProperty, value);
        }
        public static readonly DependencyProperty HiddenNavItemsProperty =
            DependencyProperty.Register(nameof(HiddenNavItems), typeof(ObservableCollection<NavItem>),
                typeof(ResponsiveNavBar), new PropertyMetadata(null));

        public bool HasHiddenItems
        {
            get => (bool)GetValue(HasHiddenItemsProperty);
            set => SetValue(HasHiddenItemsProperty, value);
        }
        public static readonly DependencyProperty HasHiddenItemsProperty =
            DependencyProperty.Register(nameof(HasHiddenItems), typeof(bool), typeof(ResponsiveNavBar),
                new PropertyMetadata(false));

        public NavItem AccountManagerItem
        {
            get => (NavItem)GetValue(AccountManagerItemProperty);
            set => SetValue(AccountManagerItemProperty, value);
        }
        public static readonly DependencyProperty AccountManagerItemProperty =
            DependencyProperty.Register(nameof(AccountManagerItem), typeof(NavItem), typeof(ResponsiveNavBar));

        public NavItem LogoutItem
        {
            get => (NavItem)GetValue(LogoutItemProperty);
            set => SetValue(LogoutItemProperty, value);
        }
        public static readonly DependencyProperty LogoutItemProperty =
            DependencyProperty.Register(nameof(LogoutItem), typeof(NavItem), typeof(ResponsiveNavBar));

        // Commands
        public ICommand HomeCommand
        {
            get => (ICommand)GetValue(HomeCommandProperty);
            set => SetValue(HomeCommandProperty, value);
        }
        public static readonly DependencyProperty HomeCommandProperty =
            DependencyProperty.Register(nameof(HomeCommand), typeof(ICommand), typeof(ResponsiveNavBar));

        public ICommand CatalogCommand
        {
            get => (ICommand)GetValue(CatalogCommandProperty);
            set => SetValue(CatalogCommandProperty, value);
        }
        public static readonly DependencyProperty CatalogCommandProperty =
            DependencyProperty.Register(nameof(CatalogCommand), typeof(ICommand), typeof(ResponsiveNavBar));

        public ICommand MembersCommand
        {
            get => (ICommand)GetValue(MembersCommandProperty);
            set => SetValue(MembersCommandProperty, value);
        }
        public static readonly DependencyProperty MembersCommandProperty =
            DependencyProperty.Register(nameof(MembersCommand), typeof(ICommand), typeof(ResponsiveNavBar));

        public ICommand BorrowManagerCommand
        {
            get => (ICommand)GetValue(BorrowManagerCommandProperty);
            set => SetValue(BorrowManagerCommandProperty, value);
        }
        public static readonly DependencyProperty BorrowManagerCommandProperty =
            DependencyProperty.Register(nameof(BorrowManagerCommand), typeof(ICommand), typeof(ResponsiveNavBar));

        public ICommand BorrowCommand
        {
            get => (ICommand)GetValue(BorrowCommandProperty);
            set => SetValue(BorrowCommandProperty, value);
        }
        public static readonly DependencyProperty BorrowCommandProperty =
            DependencyProperty.Register(nameof(BorrowCommand), typeof(ICommand), typeof(ResponsiveNavBar));

        public ICommand ReturnCommand
        {
            get => (ICommand)GetValue(ReturnCommandProperty);
            set => SetValue(ReturnCommandProperty, value);
        }
        public static readonly DependencyProperty ReturnCommandProperty =
            DependencyProperty.Register(nameof(ReturnCommand), typeof(ICommand), typeof(ResponsiveNavBar));

        public ICommand AccountManagerCommand
        {
            get => (ICommand)GetValue(AccountManagerCommandProperty);
            set => SetValue(AccountManagerCommandProperty, value);
        }
        public static readonly DependencyProperty AccountManagerCommandProperty =
            DependencyProperty.Register(nameof(AccountManagerCommand), typeof(ICommand), typeof(ResponsiveNavBar));

        public ICommand LogoutCommand
        {
            get => (ICommand)GetValue(LogoutCommandProperty);
            set => SetValue(LogoutCommandProperty, value);
        }
        public static readonly DependencyProperty LogoutCommandProperty =
            DependencyProperty.Register(nameof(LogoutCommand), typeof(ICommand), typeof(ResponsiveNavBar));

        public ICommand EventsCommand
        {
            get => (ICommand)GetValue(EventsCommandProperty);
            set => SetValue(EventsCommandProperty, value);
        }
        public static readonly DependencyProperty EventsCommandProperty =
            DependencyProperty.Register(nameof(EventsCommand), typeof(ICommand), typeof(ResponsiveNavBar));

        #endregion

        #region Event Handlers

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _isInitialized = true;
            UpdateVisibleItems();

            // Sélectionner le premier item par défaut
            Dispatcher.InvokeAsync(() =>
            {
                SelectFirstItem();
            }, System.Windows.Threading.DispatcherPriority.Loaded);
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_isInitialized && e.HeightChanged)
            {
                UpdateVisibleItems();
            }
        }

        private void NavBar_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!_isExpanded)
            {
                _isExpanded = true;
                AnimateWidth(220, ANIMATION_DURATION);
                AnimateAllTextBlocks(true);
            }
        }

        private void NavBar_MouseLeave(object sender, MouseEventArgs e)
        {
            if (_isExpanded)
            {
                _isExpanded = false;
                AnimateWidth(80, ANIMATION_DURATION);
                AnimateAllTextBlocks(false);
            }
        }

        private void AvatarButton_Click(object sender, MouseButtonEventArgs e)
        {
            // Exécuter la commande Account Manager
            HandleNavigation(AccountButton, AccountManagerItem);
        }

        private void NavButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is NavItem item)
            {
                HandleNavigation(button, item);
            }
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            OverflowMenuPopup.IsOpen = !OverflowMenuPopup.IsOpen;
        }

        private void OverflowMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is NavItem item)
            {
                OverflowMenuPopup.IsOpen = false;

                // Trouver le bouton correspondant dans les visibles (si déplacé)
                // Sinon on ne met pas de sélection visuelle pour les items cachés
                ResetAllButtonStyles();
                ExecuteCommand(item.Id);
            }
        }

        #endregion

        #region Navigation Logic

        private void HandleNavigation(Button clickedButton, NavItem item)
        {
            if (_currentActiveButton == clickedButton )
                return;

            if(clickedButton != LogoutButton) 
            { 
            // Animation et changement de style
            ResetAllButtonStyles();

            // Appliquer le style actif au bouton cliqué
            clickedButton.Style = (Style)FindResource("NavButtonActiveStyle");

            // Désactiver le hit test sur le bouton actif
            clickedButton.IsHitTestVisible = false;

            _currentActiveButton = clickedButton;
            }
            // Exécuter la commande
            ExecuteCommand(item.Id);

            // Fermer le popup si ouvert
            if (OverflowMenuPopup.IsOpen)
            {
                OverflowMenuPopup.IsOpen = false;
            }
        }

        private void ExecuteCommand(string itemId)
        {
            ICommand command = itemId switch
            {
                "Home" => HomeCommand,
                "Catalog" => CatalogCommand,
                "Members" => MembersCommand,
                "BorrowManager" => BorrowManagerCommand,
                "Borrow" => BorrowCommand,
                "Return" => ReturnCommand,
                "Account" => AccountManagerCommand,
                "Logout" => LogoutCommand,
                "Events" => EventsCommand,
                _ => null
            };

            if (command?.CanExecute(null) == true)
            {
                command.Execute(null);
            }
        }

        private void SelectFirstItem()
        {
            if (VisibleNavItems?.Count > 0)
            {
                // Trouver le premier bouton dans le conteneur
                Dispatcher.InvokeAsync(() =>
                {
                    var firstContainer = VisibleButtonsContainer.ItemContainerGenerator.ContainerFromIndex(0);
                    if (firstContainer != null)
                    {
                        var firstButton = FindVisualChild<Button>(firstContainer);
                        if (firstButton != null && firstButton.Tag is NavItem firstItem)
                        {
                            HandleNavigation(firstButton, firstItem);
                        }
                    }
                }, System.Windows.Threading.DispatcherPriority.Loaded);
            }
        }

        #endregion

        #region Visual State Management

        private void UpdateVisibleItems()
        {
            if (!_isInitialized || ActualHeight == 0) return;

            double availableHeight = ActualHeight - AVATAR_SECTION_HEIGHT - BOTTOM_SECTION_HEIGHT - (SEPARATOR_HEIGHT * 2);
            int maxVisibleItems = Math.Max(0, (int)(availableHeight / BUTTON_HEIGHT));

            if (maxVisibleItems < AllNavItems.Count)
            {
                maxVisibleItems = Math.Max(0, maxVisibleItems - 1);
            }

            var visible = AllNavItems.Take(maxVisibleItems).ToList();
            var hidden = AllNavItems.Skip(maxVisibleItems).ToList();

            Dispatcher.Invoke(() =>
            {
                VisibleNavItems.Clear();
                HiddenNavItems.Clear();

                foreach (var item in visible)
                    VisibleNavItems.Add(item);

                foreach (var item in hidden)
                    HiddenNavItems.Add(item);

                HasHiddenItems = hidden.Any();
            });
        }

        private void ResetAllButtonStyles()
        {
            // Reset tous les boutons visibles
            ResetButtonsInContainer(VisibleButtonsContainer);

            // Reset les boutons fixes
            ResetButton(AccountButton);
            ResetButton(LogoutButton);
            ResetButton(MenuButton);
        }

        private void ResetButtonsInContainer(ItemsControl container)
        {
            if (container == null) return;

            for (int i = 0; i < container.Items.Count; i++)
            {
                var itemContainer = container.ItemContainerGenerator.ContainerFromIndex(i);
                if (itemContainer != null)
                {
                    var button = FindVisualChild<Button>(itemContainer);
                    if (button != null)
                    {
                        ResetButton(button);
                    }
                }
            }
        }

        private void ResetButton(Button button)
        {
            if (button == null) return;

            button.Style = (Style)FindResource("NavButtonStyle");
            button.IsHitTestVisible = true;

            // Réinitialiser la couleur de l'icône
            UpdateIconColor(button, (Brush)new BrushConverter().ConvertFrom("#F1F1F1"));
        }

        private void UpdateIconColor(Button button, Brush color )
        {
            if (button == null) return;

            // Chercher l'IconButton dans le contenu
            var iconButton = FindVisualChild<FrameworkElement>(button, el =>
                el.GetType().Name == "IconButton");

            if (iconButton != null)
            {
                var iconColorProp = iconButton.GetType().GetProperty("IconColor");
                if (iconColorProp != null)
                {
                    iconColorProp.SetValue(iconButton, color);
                }
            }
        }

        #endregion

        #region Animations

        private void AnimateWidth(double toWidth, double durationSeconds)
        {
            double fromWidth = double.IsNaN(Width) ? ActualWidth : Width;

            var animation = new DoubleAnimation
            {
                From = fromWidth,
                To = toWidth,
                Duration = TimeSpan.FromSeconds(durationSeconds),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };

            BeginAnimation(WidthProperty, animation);
        }

        private void AnimateAllTextBlocks(bool show)
        {
            AnimateTextBlocksInContainer(VisibleButtonsContainer, show);
            AnimateTextBlockInButton(AccountButton, show);
            AnimateTextBlockInButton(LogoutButton, show);
            AnimateTextBlockInButton(MenuButton, show);
        }

        private void AnimateTextBlocksInContainer(ItemsControl container, bool show)
        {
            if (container == null) return;

            for (int i = 0; i < container.Items.Count; i++)
            {
                var itemContainer = container.ItemContainerGenerator.ContainerFromIndex(i);
                if (itemContainer != null)
                {
                    var button = FindVisualChild<Button>(itemContainer);
                    if (button != null)
                    {
                        AnimateTextBlockInButton(button, show);
                    }
                }
            }
        }

        private void AnimateTextBlockInButton(Button button, bool show)
        {
            if (button == null) return;

            var textBlock = FindVisualChild<TextBlock>(button, "NavText");
            if (textBlock == null) return;

            var translateTransform = textBlock.RenderTransform as TranslateTransform;

            if (show)
            {
                textBlock.Visibility = Visibility.Visible;

                var opacityAnim = new DoubleAnimation
                {
                    To = 1,
                    Duration = TimeSpan.FromSeconds(0.3),
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                };

                var translateAnim = new DoubleAnimation
                {
                    To = 0,
                    Duration = TimeSpan.FromSeconds(0.3),
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                };

                textBlock.BeginAnimation(UIElement.OpacityProperty, opacityAnim);
                translateTransform?.BeginAnimation(TranslateTransform.XProperty, translateAnim);
            }
            else
            {
                var opacityAnim = new DoubleAnimation
                {
                    To = 0,
                    Duration = TimeSpan.FromSeconds(0.2),
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
                };

                var translateAnim = new DoubleAnimation
                {
                    To = -15,
                    Duration = TimeSpan.FromSeconds(0.2),
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
                };

                opacityAnim.Completed += (s, e) => { textBlock.Visibility = Visibility.Collapsed; };

                textBlock.BeginAnimation(UIElement.OpacityProperty, opacityAnim);
                translateTransform?.BeginAnimation(TranslateTransform.XProperty, translateAnim);
            }
        }

        #endregion

        #region Helper Methods

        private T FindVisualChild<T>(DependencyObject parent, string name = null) where T : DependencyObject
        {
            if (parent == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T typedChild)
                {
                    if (string.IsNullOrEmpty(name))
                        return typedChild;

                    if (child is FrameworkElement fe && fe.Name == name)
                        return typedChild;
                }

                var result = FindVisualChild<T>(child, name);
                if (result != null)
                    return result;
            }

            return null;
        }
        public  enum NavAction
        {
            Home,
            Catalog,
            Members,
            BorrowManager,
            Borrow,
            Return,
            Account,
            Logout
        }

        public void Click_On(NavAction action)
        {
            string itemId = action.ToString();

            // Cas spéciaux : sections fixes
            if (action == NavAction.Account)
            {
                HandleNavigation(AccountButton, AccountManagerItem);
                return;
            }

            if (action == NavAction.Logout)
            {
                HandleNavigation(LogoutButton, LogoutItem);
                return;
            }

            // Cherche l'item dans la liste
            var targetItem = AllNavItems.FirstOrDefault(i => i.Id == itemId);
            if (targetItem == null)
                return;

            // Trouve le bouton correspondant dans la liste visible
            Button? targetButton = null;

            for (int i = 0; i < VisibleButtonsContainer.Items.Count; i++)
            {
                var itemContainer = VisibleButtonsContainer.ItemContainerGenerator.ContainerFromIndex(i);
                if (itemContainer == null) continue;

                var button = FindVisualChild<Button>(itemContainer);
                if (button?.Tag is NavItem item && item.Id == targetItem.Id)
                {
                    targetButton = button;
                    break;
                }
            }

            // Si le bouton est visible
            if (targetButton != null)
            {
                HandleNavigation(targetButton, targetItem);
            }
            else
            {
                // Sinon on exécute directement la commande (menu caché)
                ExecuteCommand(targetItem.Id);
            }
        }


        private T FindVisualChild<T>(DependencyObject parent, Func<T, bool> predicate) where T : DependencyObject
        {
            if (parent == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T typedChild && predicate(typedChild))
                    return typedChild;

                var result = FindVisualChild(child, predicate);
                if (result != null)
                    return result;
            }

            return null;
        }

        #endregion
    }

    #region NavItem Class
    public class NavItem
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public PackIconMaterialKind IconKind { get; set; }
    }
    #endregion
}