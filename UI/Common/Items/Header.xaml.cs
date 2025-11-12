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
using LIBBRARY_MANAGER.UI.Modules.BookCatalog.Helpers;

namespace LIBBRARY_MANAGER.UI.Common.Items.MainView
{

        /// <summary>
        /// Header composable avec logo, titre et sous-titre
        /// Réutilisable dans toute l'application
        /// </summary>
        public partial class Header : UserControl
    {
            #region Dependency Properties

            /// <summary>
            /// Logo (ImageSource) à afficher
            /// </summary>
            /// 
 public ImageSource Logo
{
    get => (ImageSource) GetValue(LogoProperty);
        set => SetValue(LogoProperty, value);
}

public static readonly DependencyProperty LogoProperty =
    DependencyProperty.Register(nameof(Logo), typeof(ImageSource), typeof(Header), new PropertyMetadata(null));

        public string LogoPath
        {
            get => (string)GetValue(LogoPathProperty);
            set => SetValue(LogoPathProperty, value);
        }

        public static readonly DependencyProperty LogoPathProperty =
            DependencyProperty.Register(nameof(LogoPath), typeof(string), typeof(Header), new PropertyMetadata(null, (d, e) =>
            {
                if (e.NewValue is string path)
                {
                    ((Header)d).Logo = GetPathHelpers.GetPathImagesource(path);
                }
            }));

        public ImageSource SideImage
        {
            get => (ImageSource)GetValue(SideImageProperty);
            set => SetValue(SideImageProperty, value);
        }

        public static readonly DependencyProperty SideImageProperty=
            DependencyProperty.Register(nameof(SideImage
                ), typeof(ImageSource), typeof(Header), new PropertyMetadata(null));

        public string SideImagePath
        {
            get => (string)GetValue(SideImagePathProperty);
            set => SetValue(SideImagePathProperty, value);
        }

        public static readonly DependencyProperty SideImagePathProperty =
            DependencyProperty.Register(nameof(SideImagePath
                ), typeof(string), typeof(Header), new PropertyMetadata(null, (d, e) =>
            {
                if (e.NewValue is string path)
                {
                    ((Header)d).SideImage
                    = GetPathHelpers.GetPathImagesource(path);
                }
            }));


        /// <summary>
        /// Titre principal (ex: "NOCTUA Library")
        /// </summary>
        public string Title
            {
                get => (string)GetValue(TitleProperty);
                set => SetValue(TitleProperty, value);
            }
            public static readonly DependencyProperty TitleProperty =
                DependencyProperty.Register(
                    nameof(Title),
                    typeof(string),
                    typeof(Header),
                    new PropertyMetadata(string.Empty));

            /// <summary>
            /// Sous-titre descriptif (ex: "Explorez notre collection")
            /// </summary>
            public string Subtitle
            {
                get => (string)GetValue(SubtitleProperty);
                set => SetValue(SubtitleProperty, value);
            }
            public static readonly DependencyProperty SubtitleProperty =
                DependencyProperty.Register(
                    nameof(Subtitle),
                    typeof(string),
                    typeof(Header),
                    new PropertyMetadata(string.Empty));

            /// <summary>
            /// Hauteur du logo
            /// </summary>
            public double LogoHeight
            {
                get => (double)GetValue(LogoHeightProperty);
                set => SetValue(LogoHeightProperty, value);
            }
            public static readonly DependencyProperty LogoHeightProperty =
                DependencyProperty.Register(
                    nameof(LogoHeight),
                    typeof(double),
                    typeof(Header),
                    new PropertyMetadata(60.0));

            /// <summary>
            /// Largeur du logo
            /// </summary>
            public double LogoWidth
            {
                get => (double)GetValue(LogoWidthProperty);
                set => SetValue(LogoWidthProperty, value);
            }
            public static readonly DependencyProperty LogoWidthProperty =
                DependencyProperty.Register(
                    nameof(LogoWidth),
                    typeof(double),
                    typeof(Header),
                    new PropertyMetadata(60.0));

            /// <summary>
            /// Taille du titre
            /// </summary>
            public double TitleFontSize
            {
                get => (double)GetValue(TitleFontSizeProperty);
                set => SetValue(TitleFontSizeProperty, value);
            }
            public static readonly DependencyProperty TitleFontSizeProperty =
                DependencyProperty.Register(
                    nameof(TitleFontSize),
                    typeof(double),
                    typeof(Header),
                    new PropertyMetadata(32.0));

            /// <summary>
            /// Taille du sous-titre
            /// </summary>
            public double SubtitleFontSize
            {
                get => (double)GetValue(SubtitleFontSizeProperty);
                set => SetValue(SubtitleFontSizeProperty, value);
            }
            public static readonly DependencyProperty SubtitleFontSizeProperty =
                DependencyProperty.Register(
                    nameof(SubtitleFontSize),
                    typeof(double),
                    typeof(Header),
                    new PropertyMetadata(16.0));

            /// <summary>
            /// Couleur du titre
            /// </summary>
            public System.Windows.Media.Brush TitleColor
            {
                get => (System.Windows.Media.Brush)GetValue(TitleColorProperty);
                set => SetValue(TitleColorProperty, value);
            }
            public static readonly DependencyProperty TitleColorProperty =
                DependencyProperty.Register(
                    nameof(TitleColor),
                    typeof(System.Windows.Media.Brush),
                    typeof(Header),
                    new PropertyMetadata(new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 21, 101, 192)))); // #1565C0

            /// <summary>
            /// Couleur du sous-titre
            /// </summary>
            public System.Windows.Media.Brush SubtitleColor
            {
                get => (System.Windows.Media.Brush)GetValue(SubtitleColorProperty);
                set => SetValue(SubtitleColorProperty, value);
            }
            public static readonly DependencyProperty SubtitleColorProperty =
                DependencyProperty.Register(
                    nameof(SubtitleColor),
                    typeof(System.Windows.Media.Brush),
                    typeof(Header),
                    new PropertyMetadata(new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 117, 117, 117)))); // #757575

            /// <summary>
            /// Espacement entre le logo et le texte
            /// </summary>
            public double LogoMargin
            {
                get => (double)GetValue(LogoMarginProperty);
                set => SetValue(LogoMarginProperty, value);
            }
            public static readonly DependencyProperty LogoMarginProperty =
                DependencyProperty.Register(
                    nameof(LogoMargin),
                    typeof(double),
                    typeof(Header),
                    new PropertyMetadata(20.0));

            /// <summary>
            /// Espacement global du header
            /// </summary>
            public Thickness HeaderMargin
            {
                get => (Thickness)GetValue(HeaderMarginProperty);
                set => SetValue(HeaderMarginProperty, value);
            }
            public static readonly DependencyProperty HeaderMarginProperty =
                DependencyProperty.Register(
                    nameof(HeaderMargin),
                    typeof(Thickness),
                    typeof(Header),
                    new PropertyMetadata(new Thickness(0, 0, 0, 30)));

            #endregion

            #region Constructeur

            public Header()
            {
                InitializeComponent();

                // Initialisation des valeurs par défaut
                Title = "NOCTUA Library";
                Subtitle = "Explorez notre collection de livres";

                // Charger le logo par défaut si pas de logo fourni
                if (Logo == null)
                {
                    LoadDefaultLogo();
                }
            }

            #endregion

            #region Méthodes Privées

            /// <summary>
            /// Charge le logo par défaut depuis les ressources
            /// </summary>
            private void LoadDefaultLogo()
            {
                try
                {
                    // Chemin par défaut vers le logo
                    var uri = new Uri("pack://application:,,,/Assets/Logos/noctua-logo.png");
                    Logo = new BitmapImage(uri);
                }
                catch (Exception ex)
                {
                    // Si le logo par défaut n'existe pas, continuer sans erreur
                    System.Diagnostics.Debug.WriteLine($"Impossible de charger le logo par défaut: {ex.Message}");
                }
            }

            #endregion

            #region Méthodes Publiques

            /// <summary>
            /// Définit le logo par chemin de fichier
            /// </summary>
            /// <param name="filePath">Chemin du fichier image</param>
            public void SetLogoFromFile(string filePath)
            {
                try
                {
                    if (System.IO.File.Exists(filePath))
                    {
                        var uri = new Uri(filePath, UriKind.Absolute);
                        Logo = new BitmapImage(uri);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Erreur lors du chargement du logo: {ex.Message}");
                }
            }

            /// <summary>
            /// Définit le logo via une URI pack
            /// </summary>
            /// <param name="packUri">URI pack (ex: pack://application:,,,/Assets/logo.png)</param>
            public void SetLogoFromPack(string packUri)
            {
                try
                {
                    Logo = new BitmapImage(new Uri(packUri));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Erreur lors du chargement du logo: {ex.Message}");
                }
            }

            /// <summary>
            /// Définit le titre et le sous-titre en une seule méthode
            /// </summary>
            public void SetHeader(string title, string subtitle)
            {
                Title = title;
                Subtitle = subtitle;
            }

            /// <summary>
            /// Applique un style complet au header (couleurs, tailles...)
            /// </summary>
            public void ApplyStyle(double titleSize, double subtitleSize,
                                   System.Windows.Media.Brush titleColor,
                                   System.Windows.Media.Brush subtitleColor)
            {
                TitleFontSize = titleSize;
                SubtitleFontSize = subtitleSize;
                TitleColor = titleColor;
                SubtitleColor = subtitleColor;
            }

            #endregion
        }
    }
