using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using MahApps.Metro.IconPacks;
using LIBBRARY_MANAGER.Model;
using static LIBBRARY_MANAGER.Model.Book;
using System.Configuration;
using LIBBRARY_MANAGER.UI.Modules.BookCatalog.Config;

namespace LIBBRARY_MANAGER.UI.Modules.BookCatalog.Items
{
    public partial class CategoryTile : UserControl
    {
        #region Dependency Properties

        // Catégorie (enum)
        public static readonly DependencyProperty CategoryProperty =
            DependencyProperty.Register(
                nameof(Category),
                typeof(CategoryAllowed),
                typeof(CategoryTile),
                new PropertyMetadata(CategoryAllowed.Roman));

        public CategoryAllowed Category
        {
            get => (CategoryAllowed)GetValue(CategoryProperty);
            set => SetValue(CategoryProperty, value);
        }

        // Titre affiché
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                nameof(Title),
                typeof(string),
                typeof(CategoryTile),
                new PropertyMetadata(string.Empty));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        // Compteur de livres
        public static readonly DependencyProperty BookCountProperty =
            DependencyProperty.Register(
                nameof(BookCount),
                typeof(string),
                typeof(CategoryTile),
                new PropertyMetadata("0 livres"));

        public string BookCount
        {
            get => (string)GetValue(BookCountProperty);
            set => SetValue(BookCountProperty, value);
        }

        // Icône
        public static readonly DependencyProperty IconKindProperty =
            DependencyProperty.Register(
                nameof(IconKind),
                typeof(PackIconMaterialKind),
                typeof(CategoryTile),
                new PropertyMetadata(PackIconMaterialKind.BookOpenPageVariant));

        public PackIconMaterialKind IconKind
        {
            get => (PackIconMaterialKind)GetValue(IconKindProperty);
            set => SetValue(IconKindProperty, value);
        }

        // Couleur de l'icône
        public static readonly DependencyProperty IconColorProperty =
            DependencyProperty.Register(
                nameof(IconColor),
                typeof(Brush),
                typeof(CategoryTile),
                new PropertyMetadata(Brushes.Blue));

        public Brush IconColor
        {
            get => (Brush)GetValue(IconColorProperty);
            set => SetValue(IconColorProperty, value);
        }

        // Couleur de bordure (remplace TileBackground)
        public static readonly DependencyProperty BorderColorProperty =
            DependencyProperty.Register(
                nameof(BorderColor),
                typeof(Brush),
                typeof(CategoryTile),
                new PropertyMetadata(Brushes.Blue));

        public Brush BorderColor
        {
            get => (Brush)GetValue(BorderColorProperty);
            set => SetValue(BorderColorProperty, value);
        }

        // Background de la tuile (toujours blanc)
        public static readonly DependencyProperty TileBackgroundProperty =
            DependencyProperty.Register(
                nameof(TileBackground),
                typeof(Brush),
                typeof(CategoryTile),
                new PropertyMetadata(Brushes.White));

        public Brush TileBackground
        {
            get => (Brush)GetValue(TileBackgroundProperty);
            private set => SetValue(TileBackgroundProperty, value);
        }

        #endregion

        #region Events

        // Event déclenché lors du clic
        public event Action<CategoryAllowed> OnCategoryClicked;

        #endregion

        #region Constructor

        public CategoryTile()
        {
            InitializeComponent();
            DataContext = this;

            // Forcer le fond blanc
            TileBackground = Brushes.White;

            // Gestion des events
            MouseDown += CategoryTile_MouseDown;
            MouseEnter += CategoryTile_MouseEnter;
            MouseLeave += CategoryTile_MouseLeave;
        }

        #endregion

        #region Event Handlers

        private void CategoryTile_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OnCategoryClicked?.Invoke(Category);
        }

        private void CategoryTile_MouseEnter(object sender, MouseEventArgs e)
        {
            AnimateHover(1.05);
        }

        private void CategoryTile_MouseLeave(object sender, MouseEventArgs e)
        {
            AnimateHover(1.0);
        }

        #endregion

        #region Animation

        private void AnimateHover(double scale)
        {
            var duration = TimeSpan.FromMilliseconds(200);
            var easing = new QuadraticEase { EasingMode = EasingMode.EaseOut };

            // Animation du scale
            var animX = new DoubleAnimation(scale, duration) { EasingFunction = easing };
            var animY = new DoubleAnimation(scale, duration) { EasingFunction = easing };

            TileScale.BeginAnimation(ScaleTransform.ScaleXProperty, animX);
            TileScale.BeginAnimation(ScaleTransform.ScaleYProperty, animY);

            // Animation de la bordure
            if (scale > 1.0)
            {
                // Hover: Bordure plus épaisse et plus lumineuse
                AnimateBorder(6, 1.3);
            }
            else
            {
                // Normal: Bordure fine
                AnimateBorder(4, 1.0);
            }
        }

        private void AnimateBorder(double thickness, double brightnessFactor)
        {
            var duration = TimeSpan.FromMilliseconds(200);
            var easing = new QuadraticEase { EasingMode = EasingMode.EaseOut };

            // Animation de l'épaisseur de bordure
            var thicknessAnim = new ThicknessAnimation(
                new Thickness(thickness),
                duration
            )
            {
                EasingFunction = easing
            };

            TileBorder.BeginAnimation(Border.BorderThicknessProperty, thicknessAnim);

            if(TileBorder.BorderBrush is SolidColorBrush solidBrush)
            {
                // Fallback : convertir SolidColorBrush en gradient
                string colorHex = ColorToHex(solidBrush.Color);

                // Assombrir légèrement pour créer un dégradé
                var darkenedColor = AdjustBrightness(solidBrush.Color, 1.8); // 10% plus foncé
                string darkenedHex = ColorToHex(darkenedColor);

                // Créer un gradient
                var gradientBrush = CategoryConfiguration.CreateGradientBrush(darkenedHex,colorHex);
                TileBorder.BorderBrush = gradientBrush;

                // Animer le gradient
                for (int i = 0; i < gradientBrush.GradientStops.Count; i++)
                {
                    var currentStop = gradientBrush.GradientStops[i];
                    var targetColor = AdjustBrightness(currentStop.Color, brightnessFactor);

                    var colorAnim = new ColorAnimation(targetColor, duration)
                    {
                        EasingFunction = easing
                    };

                    currentStop.BeginAnimation(GradientStop.ColorProperty, colorAnim);
                }
            }
        }
        /// <summary>
        /// Convertit une couleur Brush (SolidColorBrush) en chaîne hex
        /// </summary>
        private static string ColorToHex(Color color)
        {
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        private Color AdjustBrightness(Color color, double factor)
        {
            byte r = (byte)Math.Min(255, color.R * factor);
            byte g = (byte)Math.Min(255, color.G * factor);
            byte b = (byte)Math.Min(255, color.B * factor);

            return Color.FromArgb(color.A, r, g, b);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Configure la tuile depuis un CategoryConfig
        /// </summary>
        public void ConfigureFromCategoryConfig(Config.CategoryConfig config)
        {
            if (config == null) return;

            Category = config.Category;
            Title = config.DisplayName;
            IconKind = config.Icon;
            IconColor = config.IconColor;

            // Extraire la couleur principale du gradient pour la bordure
            if (config.BackgroundBrush is LinearGradientBrush gradientBrush &&
                gradientBrush.GradientStops.Count > 0)
            {
                // Utiliser la première couleur du gradient comme couleur de bordure
                BorderColor = new SolidColorBrush(gradientBrush.GradientStops[0].Color);
            }
            else
            {
                BorderColor = config.IconColor;
            }

            // Toujours forcer le fond blanc
            TileBackground = Brushes.White;
        }

        /// <summary>
        /// Met à jour le compteur de livres
        /// </summary>
        public void UpdateBookCount(int count)
        {
            BookCount = count == 0 ? "Aucun livre" :
                       count == 1 ? "1 livre" :
                       $"{count} livres";
        }

        #endregion
    }
}