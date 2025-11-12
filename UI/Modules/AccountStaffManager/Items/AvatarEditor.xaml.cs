using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using LIBBRARY_MANAGER.UI.Common.Items;
using LIBBRARY_MANAGER.UI.Modules.BookCatalog.Helpers;

namespace LIBBRARY_MANAGER.UI.Modules.AccountStaffManager.Items
{
    public partial class AvatarEditor : UserControl
    {
        public AvatarEditor()
        {
            InitializeComponent();

            Loaded += AvatarEditor_Loaded;
            ModifyAvatarBtn.Command = new RelayCommand(() =>
            {
                this.AvatarPopup.IsOpen = true;
            });
            if(AvatarSource == null) { 
            AvatarSource = GetPathHelpers.GetPathImagesource("UI\\Modules\\AccountStaffManager\\Assets\\wise-owl-reading-book-vintage-style-learning-icon-vector-57215705.png");

    }
}

        private void AvatarEditor_Loaded(object sender, RoutedEventArgs e)
        {
            // Configurer le conteneur cible pour l'ImageGridSelector
            ImageGridSelector.SetTargetContainer(AvatarImageBorder);

            // Charger les images d'avatar disponibles
            if (!string.IsNullOrEmpty(AvatarFolderPath))
            {
                try
                {
                    ImageSelector.LoadImagesFromFolder(AvatarFolderPath);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Erreur lors du chargement des avatars: {ex.Message}");
                }
            }
        }

        // ===== PROPRIÉTÉS DE DÉPENDANCE =====


        public static readonly DependencyProperty AvatarSourceProperty =
            DependencyProperty.Register(nameof(AvatarSource), typeof(ImageSource), typeof(AvatarEditor),
                new PropertyMetadata(null, OnAvatarSourceChanged));

        public ImageSource AvatarSource
        {
            get => (ImageSource)GetValue(AvatarSourceProperty);
            set => SetValue(AvatarSourceProperty, value);
        }

        private static void OnAvatarSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AvatarEditor editor && e.NewValue is ImageSource source)
            {
                editor.AvatarImageBrush.ImageSource = source;
            }
        }

        public static readonly DependencyProperty AvatarFolderPathProperty =
            DependencyProperty.Register(nameof(AvatarFolderPath), typeof(string), typeof(AvatarEditor),
                new PropertyMetadata(string.Empty));

        public string AvatarFolderPath
        {
            get => (string)GetValue(AvatarFolderPathProperty);
            set => SetValue(AvatarFolderPathProperty, value);
        }

        public static readonly DependencyProperty AvatarSizeProperty =
            DependencyProperty.Register(nameof(AvatarSize), typeof(double), typeof(AvatarEditor),
                new PropertyMetadata(180.0));

        public double AvatarSize
        {
            get => (double)GetValue(AvatarSizeProperty);
            set => SetValue(AvatarSizeProperty, value);
        }

        public static readonly DependencyProperty BorderBrushProperty =
            DependencyProperty.Register(nameof(BorderBrush), typeof(Brush), typeof(AvatarEditor),
                new PropertyMetadata(Brushes.SlateBlue));

        public Brush BorderBrush
        {
            get => (Brush)GetValue(BorderBrushProperty);
            set => SetValue(BorderBrushProperty, value);
        }

        public static readonly DependencyProperty DefaultBackgroundProperty =
            DependencyProperty.Register(nameof(DefaultBackground), typeof(Brush), typeof(AvatarEditor),
                new PropertyMetadata(Brushes.SlateBlue));

        public Brush DefaultBackground
        {
            get => (Brush)GetValue(DefaultBackgroundProperty);
            set => SetValue(DefaultBackgroundProperty, value);
        }

        public static readonly DependencyProperty DefaultIconSizeProperty =
            DependencyProperty.Register(nameof(DefaultIconSize), typeof(double), typeof(AvatarEditor),
                new PropertyMetadata(90.0));

        public double DefaultIconSize
        {
            get => (double)GetValue(DefaultIconSizeProperty);
            set => SetValue(DefaultIconSizeProperty, value);
        }

        public static readonly DependencyProperty EditButtonBackgroundProperty =
            DependencyProperty.Register(nameof(EditButtonBackground), typeof(Brush), typeof(AvatarEditor),
                new PropertyMetadata(Brushes.Transparent));

        public Brush EditButtonBackground
        {
            get => (Brush)GetValue(EditButtonBackgroundProperty);
            set => SetValue(EditButtonBackgroundProperty, value);
        }

        public static readonly DependencyProperty EditButtonSizeProperty =
            DependencyProperty.Register(nameof(EditButtonSize), typeof(double), typeof(AvatarEditor),
                new PropertyMetadata(45.0));

        public double EditButtonSize
        {
            get => (double)GetValue(EditButtonSizeProperty);
            set => SetValue(EditButtonSizeProperty, value);
        }

        public static readonly DependencyProperty EditButtonContainerProperty =
            DependencyProperty.Register(nameof(EditButtonContainer), typeof(double), typeof(AvatarEditor),
                new PropertyMetadata(55.0));

        public double EditButtonContainer
        {
            get => (double)GetValue(EditButtonContainerProperty);
            set => SetValue(EditButtonContainerProperty, value);
        }

        public static readonly DependencyProperty EditIconSizeProperty =
            DependencyProperty.Register(nameof(EditIconSize), typeof(double), typeof(AvatarEditor),
                new PropertyMetadata(22.0));

        public double EditIconSize
        {
            get => (double)GetValue(EditIconSizeProperty);
            set => SetValue(EditIconSizeProperty, value);
        }

        public static readonly DependencyProperty HeaderBackgroundProperty =
            DependencyProperty.Register(nameof(HeaderBackground), typeof(Brush), typeof(AvatarEditor),
                new PropertyMetadata(Brushes.SlateBlue));

        public Brush HeaderBackground
        {
            get => (Brush)GetValue(HeaderBackgroundProperty);
            set => SetValue(HeaderBackgroundProperty, value);
        }

        public static readonly DependencyProperty FooterBackgroundProperty =
            DependencyProperty.Register(nameof(FooterBackground), typeof(Brush), typeof(AvatarEditor),
                new PropertyMetadata(Brushes.SlateBlue));

        public Brush FooterBackground
        {
            get => (Brush)GetValue(FooterBackgroundProperty);
            set => SetValue(FooterBackgroundProperty, value);
        }

        public static readonly DependencyProperty AccentBrushProperty =
            DependencyProperty.Register(nameof(AccentBrush), typeof(Brush), typeof(AvatarEditor),
                new PropertyMetadata(Brushes.LightBlue));

        public Brush AccentBrush
        {
            get => (Brush)GetValue(AccentBrushProperty);
            set => SetValue(AccentBrushProperty, value);
        }

        // ===== ÉVÉNEMENTS =====

        public static readonly RoutedEvent AvatarChangedEvent =
            EventManager.RegisterRoutedEvent(nameof(AvatarChanged), RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(AvatarEditor));

        public event RoutedEventHandler AvatarChanged
        {
            add => AddHandler(AvatarChangedEvent, value);
            remove => RemoveHandler(AvatarChangedEvent, value);
        }

        private void OnEditClick(object sender, MouseButtonEventArgs e)
        {
            AvatarPopup.IsOpen = true;
        }

        private void AvatarPopup_Closed(object sender, EventArgs e)
        {
            // Déclencher l'événement de changement d'avatar
            RaiseEvent(new RoutedEventArgs(AvatarChangedEvent));
        }
    }
}