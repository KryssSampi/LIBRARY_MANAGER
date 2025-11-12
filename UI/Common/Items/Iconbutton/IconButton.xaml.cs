using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MahApps.Metro.IconPacks;
using LIBBRARY_MANAGER.UI.Common.Enums;

namespace LIBBRARY_MANAGER.UI.Common.Items
{
    public enum ButtonShape
    {
        Square,
        Rounded,
        Circle,
        Pill
    }

    public partial class IconButton : UserControl
    {
        #region Constructeur

        public IconButton()
        {
            InitializeComponent();

            // === Apparence générale ===
            Background ??= Brushes.Transparent;

            BorderBrush ??= Brushes.Transparent;

            // === Couleurs ===
            IconColor ??= Brushes.White;
            TextColor ??= Brushes.White;
            HoverBackground ??= new SolidColorBrush(Color.FromArgb(50, 255, 255, 255));
            HoverIconColor ??= IconColor;
            ButtonColor ??= Brushes.Transparent;

            // === Dimensions ===
            if (double.IsNaN(ButtonWidth))
                ButtonWidth = double.NaN;

            if (double.IsNaN(ButtonHeight) || ButtonHeight == 0)
                ButtonHeight = 40.0;

            if (IconSize <= 0)
                IconSize = 24.0;

            if (ImageSize <= 0)
                ImageSize = 32.0;

            if (TextSize <= 0)
                TextSize = 14.0;

            if (ButtonPadding == default)
                ButtonPadding = new Thickness(10);

            if (IconMargin == default)
                IconMargin = new Thickness(0);

            if (TextMargin == default)
                TextMargin = new Thickness(8, 0, 0, 0);

            if (BorderThickness == default)
                BorderThickness = new Thickness(0);

            // === Ombre ===
            if (ShadowColor == default)
                ShadowColor = Colors.Black;

            if (ShadowOpacity <= 0)
                ShadowOpacity = 0.3;

            if (ShadowBlur <= 0)
                ShadowBlur = 8.0;

            if (ShadowDepth <= 0)
                ShadowDepth = 2.0;

            // === Texte ===
            if (TextWeight == default)
                TextWeight = FontWeights.Normal;

            if (string.IsNullOrWhiteSpace(Text))
                TextVisibility = Visibility.Collapsed;
            else
                TextVisibility = Visibility.Visible;

            // === Animation ===
            if (HoverScale <= 0)
                HoverScale = 1.1;

            if (PressedScale <= 0)
                PressedScale = 0.95;

            if (HoverOpacity <= 0)
                HoverOpacity = 0.3;

            // === Forme ===
            if (CornerRadius == default)
                CornerRadius = new CornerRadius(6);

            UpdateShape();
        }

        #endregion

        #region Dependency Properties - Mode Image

        // Active le mode Image au lieu d'Icône
        public bool UseImage
        {
            get => (bool)GetValue(UseImageProperty);
            set => SetValue(UseImageProperty, value);
        }
        public static readonly DependencyProperty UseImageProperty =
            DependencyProperty.Register(
                nameof(UseImage),
                typeof(bool),
                typeof(IconButton),
                new PropertyMetadata(false, OnUseImageChanged));

        // Enum pour sélectionner l'image depuis le dossier drawable
        public DrawableImage Img
        {
            get => (DrawableImage)GetValue(ImgProperty);
            set => SetValue(ImgProperty, value);
        }
        public static readonly DependencyProperty ImgProperty =
            DependencyProperty.Register(
                nameof(Img),
                typeof(DrawableImage),
                typeof(IconButton),
                new PropertyMetadata(DrawableImage.None, OnImgChanged));

        // Source de l'image (calculée automatiquement depuis Img)
        public ImageSource ImageSource
        {
            get => (ImageSource)GetValue(ImageSourceProperty);
            private set => SetValue(ImageSourceProperty, value);
        }
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register(
                nameof(ImageSource),
                typeof(ImageSource),
                typeof(IconButton),
                new PropertyMetadata(null));

        // Taille de l'image (indépendante de IconSize)
        public double ImageSize
        {
            get => (double)GetValue(ImageSizeProperty);
            set => SetValue(ImageSizeProperty, value);
        }
        public static readonly DependencyProperty ImageSizeProperty =
            DependencyProperty.Register(
                nameof(ImageSize),
                typeof(double),
                typeof(IconButton),
                new PropertyMetadata(32.0));

        #endregion

        #region Dependency Properties - Forme

        public ButtonShape Shape
        {
            get => (ButtonShape)GetValue(ShapeProperty);
            set => SetValue(ShapeProperty, value);
        }
        public static readonly DependencyProperty ShapeProperty =
            DependencyProperty.Register(
                nameof(Shape),
                typeof(ButtonShape),
                typeof(IconButton),
                new PropertyMetadata(ButtonShape.Rounded, OnShapeChanged));

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(
                nameof(CornerRadius),
                typeof(CornerRadius),
                typeof(IconButton),
                new PropertyMetadata(new CornerRadius(5)));

        #endregion

        #region Dependency Properties - Icône

        public PackIconMaterialKind IconKind
        {
            get => (PackIconMaterialKind)GetValue(IconKindProperty);
            set => SetValue(IconKindProperty, value);
        }
        public static readonly DependencyProperty IconKindProperty =
            DependencyProperty.Register(
                nameof(IconKind),
                typeof(PackIconMaterialKind),
                typeof(IconButton),
                new PropertyMetadata(PackIconMaterialKind.Home));

        public double IconSize
        {
            get => (double)GetValue(IconSizeProperty);
            set => SetValue(IconSizeProperty, value);
        }
        public static readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register(
                nameof(IconSize),
                typeof(double),
                typeof(IconButton),
                new PropertyMetadata(24.0));

        public Brush IconColor
        {
            get => (Brush)GetValue(IconColorProperty);
            set => SetValue(IconColorProperty, value);
        }
        public static readonly DependencyProperty IconColorProperty =
            DependencyProperty.Register(
                nameof(IconColor),
                typeof(Brush),
                typeof(IconButton),
                new PropertyMetadata(Brushes.White));

        public Thickness IconMargin
        {
            get => (Thickness)GetValue(IconMarginProperty);
            set => SetValue(IconMarginProperty, value);
        }
        public static readonly DependencyProperty IconMarginProperty =
            DependencyProperty.Register(
                nameof(IconMargin),
                typeof(Thickness),
                typeof(IconButton),
                new PropertyMetadata(new Thickness(0)));

        #endregion

        #region Dependency Properties - Texte

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                nameof(Text),
                typeof(string),
                typeof(IconButton),
                new PropertyMetadata(string.Empty, OnTextChanged));

        public double TextSize
        {
            get => (double)GetValue(TextSizeProperty);
            set => SetValue(TextSizeProperty, value);
        }
        public static readonly DependencyProperty TextSizeProperty =
            DependencyProperty.Register(
                nameof(TextSize),
                typeof(double),
                typeof(IconButton),
                new PropertyMetadata(14.0));

        public Brush TextColor
        {
            get => (Brush)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }
        public static readonly DependencyProperty TextColorProperty =
            DependencyProperty.Register(
                nameof(TextColor),
                typeof(Brush),
                typeof(IconButton),
                new PropertyMetadata(Brushes.White));

        public FontWeight TextWeight
        {
            get => (FontWeight)GetValue(TextWeightProperty);
            set => SetValue(TextWeightProperty, value);
        }
        public static readonly DependencyProperty TextWeightProperty =
            DependencyProperty.Register(
                nameof(TextWeight),
                typeof(FontWeight),
                typeof(IconButton),
                new PropertyMetadata(FontWeights.Normal));

        public Thickness TextMargin
        {
            get => (Thickness)GetValue(TextMarginProperty);
            set => SetValue(TextMarginProperty, value);
        }
        public static readonly DependencyProperty TextMarginProperty =
            DependencyProperty.Register(
                nameof(TextMargin),
                typeof(Thickness),
                typeof(IconButton),
                new PropertyMetadata(new Thickness(8, 0, 0, 0)));

        public Visibility TextVisibility
        {
            get => (Visibility)GetValue(TextVisibilityProperty);
            set => SetValue(TextVisibilityProperty, value);
        }
        public static readonly DependencyProperty TextVisibilityProperty =
            DependencyProperty.Register(
                nameof(TextVisibility),
                typeof(Visibility),
                typeof(IconButton),
                new PropertyMetadata(Visibility.Collapsed));

        #endregion

        #region Dependency Properties - Dimensions et Couleurs

        public Brush HoverBackground
        {
            get => (Brush)GetValue(HoverBackgroundProperty);
            set => SetValue(HoverBackgroundProperty, value);
        }
        public static readonly DependencyProperty HoverBackgroundProperty =
            DependencyProperty.Register(
                nameof(HoverBackground),
                typeof(Brush),
                typeof(IconButton),
                new PropertyMetadata(null));

        public double HoverOpacity
        {
            get => (double)GetValue(HoverOpacityProperty);
            set => SetValue(HoverOpacityProperty, value);
        }
        public static readonly DependencyProperty HoverOpacityProperty =
            DependencyProperty.Register(
                nameof(HoverOpacity),
                typeof(double),
                typeof(IconButton),
                new PropertyMetadata(0.3));

        public double HoverScale
        {
            get => (double)GetValue(HoverScaleProperty);
            set => SetValue(HoverScaleProperty, value);
        }
        public static readonly DependencyProperty HoverScaleProperty =
            DependencyProperty.Register(nameof(HoverScale), typeof(double), typeof(IconButton),
                new PropertyMetadata(1.1));

        public double PressedScale
        {
            get => (double)GetValue(PressedScaleProperty);
            set => SetValue(PressedScaleProperty, value);
        }
        public static readonly DependencyProperty PressedScaleProperty =
            DependencyProperty.Register(nameof(PressedScale), typeof(double), typeof(IconButton),
                new PropertyMetadata(0.95));

        public Brush HoverIconColor
        {
            get => (Brush)GetValue(HoverIconColorProperty);
            set => SetValue(HoverIconColorProperty, value);
        }
        public static readonly DependencyProperty HoverIconColorProperty =
            DependencyProperty.Register(
                nameof(HoverIconColor),
                typeof(Brush),
                typeof(IconButton),
                new PropertyMetadata(null));

        public double ButtonWidth
        {
            get => (double)GetValue(ButtonWidthProperty);
            set => SetValue(ButtonWidthProperty, value);
        }
        public static readonly DependencyProperty ButtonWidthProperty =
            DependencyProperty.Register(
                nameof(ButtonWidth),
                typeof(double),
                typeof(IconButton),
                new PropertyMetadata(double.NaN));

        public Brush ButtonColor
        {
            get => (Brush)GetValue(ButtonColorProperty);
            set => SetValue(ButtonColorProperty, value);
        }
        public static readonly DependencyProperty ButtonColorProperty =
            DependencyProperty.Register(
                nameof(ButtonColor),
                typeof(Brush),
                typeof(IconButton),
                new PropertyMetadata(Brushes.Transparent));

        public new Thickness BorderThickness
        {
            get => (Thickness)GetValue(ButtonThicknessProperty);
            set
            {
                SetValue(ButtonThicknessProperty, value);
                MainButton.BorderThickness = this.BorderThickness;
                this.BorderThickness = new Thickness(0);
            }
        }
        public static readonly DependencyProperty ButtonThicknessProperty =
            DependencyProperty.Register(
                nameof(BorderThickness),
                typeof(Thickness),
                typeof(IconButton),
                new PropertyMetadata(new Thickness(2)));


        public Thickness SealedThickness
        {
            get => (Thickness)GetValue(SealedThicknessProperty);
            set
            {
                SetValue(SealedThicknessProperty, value);
                MainButton.BorderThickness = this.BorderThickness;
                this.BorderThickness = new Thickness(1);
            }
        }
        public static readonly DependencyProperty SealedThicknessProperty =
            DependencyProperty.Register(
                nameof(SealedThickness),
                typeof(Thickness),
                typeof(IconButton),
                  new PropertyMetadata(new Thickness(0)));

        public double ButtonHeight
        {
            get => (double)GetValue(ButtonHeightProperty);
            set => SetValue(ButtonHeightProperty, value);
        }
        public static readonly DependencyProperty ButtonHeightProperty =
            DependencyProperty.Register(
                nameof(ButtonHeight),
                typeof(double),
                typeof(IconButton),
                new PropertyMetadata(40.0));

        public Thickness ButtonPadding
        {
            get => (Thickness)GetValue(ButtonPaddingProperty);
            set => SetValue(ButtonPaddingProperty, value);
        }
        public static readonly DependencyProperty ButtonPaddingProperty =
            DependencyProperty.Register(
                nameof(ButtonPadding),
                typeof(Thickness),
                typeof(IconButton),
                new PropertyMetadata(new Thickness(10)));

        #endregion

        #region Dependency Properties - Ombre

        public Color ShadowColor
        {
            get => (Color)GetValue(ShadowColorProperty);
            set => SetValue(ShadowColorProperty, value);
        }
        public static readonly DependencyProperty ShadowColorProperty =
            DependencyProperty.Register(
                nameof(ShadowColor),
                typeof(Color),
                typeof(IconButton),
                new PropertyMetadata(Colors.Black));
        public new Brush BorderBrush
        {
            get => (Brush)GetValue(BorderBrushProperty);
            set => SetValue(BorderBrushProperty, value);
        }
        public static new readonly DependencyProperty BorderBrushProperty =
            DependencyProperty.Register(
                nameof(BorderBrush),
                typeof(Brush),
                typeof(IconButton),
                new PropertyMetadata(Brushes.Transparent));


        public Brush SealedBrush
        {
            get => (Brush)GetValue(SealedBrushProperty);
            set => SetValue(SealedBrushProperty, value);
        }
        public static readonly DependencyProperty SealedBrushProperty =
            DependencyProperty.Register(
                nameof(SealedBrush),
                typeof(Brush),
                typeof(IconButton),
                new PropertyMetadata(Brushes.Transparent));

        public double ShadowOpacity
        {
            get => (double)GetValue(ShadowOpacityProperty);
            set => SetValue(ShadowOpacityProperty, value);
        }
        public static readonly DependencyProperty ShadowOpacityProperty =
            DependencyProperty.Register(
                nameof(ShadowOpacity),
                typeof(double),
                typeof(IconButton),
                new PropertyMetadata(0.3));

        public double ShadowBlur
        {
            get => (double)GetValue(ShadowBlurProperty);
            set => SetValue(ShadowBlurProperty, value);
        }
        public static readonly DependencyProperty ShadowBlurProperty =
            DependencyProperty.Register(
                nameof(ShadowBlur),
                typeof(double),
                typeof(IconButton),
                new PropertyMetadata(8.0));

        public double ShadowDepth
        {
            get => (double)GetValue(ShadowDepthProperty);
            set => SetValue(ShadowDepthProperty, value);
        }
        public static readonly DependencyProperty ShadowDepthProperty =
            DependencyProperty.Register(
                nameof(ShadowDepth),
                typeof(double),
                typeof(IconButton),
                new PropertyMetadata(2.0));

        #endregion

        #region Dependency Properties - Command

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(
                nameof(Command),
                typeof(ICommand),
                typeof(IconButton),
                new PropertyMetadata(null));

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(
                nameof(CommandParameter),
                typeof(object),
                typeof(IconButton),
                new PropertyMetadata(null));

        #endregion

        #region Events

        public RoutedEvent Click { get; set; }

        public static RoutedEvent ClickEvent { get; set; } = Button.ClickEvent;

        private void MainButton_Click(object sender, RoutedEventArgs e)
        {
            if (Click is not null)
                RaiseEvent(new RoutedEventArgs(Click, this));

            if (ClickEvent is not null)
                RaiseEvent(new RoutedEventArgs(ClickEvent, this));

            // Toggle eye icon (comportement spécial)
            if (!UseImage)
            {
                if (this.IconKind == PackIconMaterialKind.EyeClosed)
                {
                    this.IconKind = PackIconMaterialKind.Eye;
                }
                else if (this.IconKind == PackIconMaterialKind.Eye)
                {
                    this.IconKind = PackIconMaterialKind.EyeClosed;
                }
                if (this.IconKind == PackIconMaterialKind.ViewGridOutline)
                {
                    this.IconKind = PackIconMaterialKind.ViewList;
                }
                else if (this.IconKind == PackIconMaterialKind.ViewList)
                {
                    this.IconKind = PackIconMaterialKind.ViewGridOutline;
                }

            }
        }

        #endregion

        #region Private Methods

        private static void OnShapeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is IconButton button)
            {
                button.UpdateShape();
            }
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is IconButton button)
            {
                button.TextVisibility = string.IsNullOrEmpty(button.Text)
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            }
        }

        private static void OnUseImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is IconButton button)
            {
                button.UpdateImageSource();
            }
        }

        private static void OnImgChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is IconButton button && button.UseImage)
            {
                button.UpdateImageSource();
            }
        }

        private void UpdateShape()
        {
            switch (Shape)
            {
                case ButtonShape.Square:
                    CornerRadius = new CornerRadius(0);
                    break;
                case ButtonShape.Rounded:
                    CornerRadius = new CornerRadius(5);
                    break;
                case ButtonShape.Circle:
                    CornerRadius = new CornerRadius(ButtonHeight / 2);
                    ButtonWidth = ButtonHeight;
                    break;
                case ButtonShape.Pill:
                    CornerRadius = new CornerRadius(ButtonHeight / 2);
                    break;
            }
        }

        private void UpdateImageSource()
        {
            if (!UseImage || Img == DrawableImage.None)
            {
                ImageSource = null;
                return;
            }

            try
            {
                var imagePath = DrawableImageHelper.GetImagePath(Img);
                if (!string.IsNullOrEmpty(imagePath))
                {
                    ImageSource = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
                }
                else
                {
                    ImageSource = null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur chargement image {Img}: {ex.Message}");
                ImageSource = null;
            }
        }

        #endregion
    }
}