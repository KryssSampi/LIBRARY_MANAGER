using System;
using System.Collections.Generic;
using System.IO;
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

namespace LIBBRARY_MANAGER.UI.Modules.AccountStaffManager.Items
{
    /// <summary>
    /// Logique d'interaction pour ImageGridSelector.xaml
        /// UserControl pour sélectionner des images dans un dossier et les afficher dans un conteneur cible
        /// </summary>
        public partial class ImageGridSelector : UserControl
        {
            private static Border _targetContainer;
            private static UIElement _previousContent;
            private static UIElement _currentContent;
            private List<string> _imagePaths;
            private string _folderPath;


            // ===========================
            // 🎨 Propriétés visuelles
            // ===========================

            public Brush BackgroundBrush
            {
                get => (Brush)GetValue(BackgroundBrushProperty);
                set => SetValue(BackgroundBrushProperty, value);
            }
            public static readonly DependencyProperty BackgroundBrushProperty =
                DependencyProperty.Register(nameof(BackgroundBrush), typeof(Brush), typeof(ImageGridSelector));

            public Brush HeaderBackground
            {
                get => (Brush)GetValue(HeaderBackgroundProperty);
                set => SetValue(HeaderBackgroundProperty, value);
            }
            public static readonly DependencyProperty HeaderBackgroundProperty =
                DependencyProperty.Register(nameof(HeaderBackground), typeof(Brush), typeof(ImageGridSelector));

            public Brush FooterBackground
            {
                get => (Brush)GetValue(FooterBackgroundProperty);
                set => SetValue(FooterBackgroundProperty, value);
            }
            public static readonly DependencyProperty FooterBackgroundProperty =
                DependencyProperty.Register(nameof(FooterBackground), typeof(Brush), typeof(ImageGridSelector));

            public Brush GridBackground
            {
                get => (Brush)GetValue(GridBackgroundProperty);
                set => SetValue(GridBackgroundProperty, value);
            }
            public static readonly DependencyProperty GridBackgroundProperty =
                DependencyProperty.Register(nameof(GridBackground), typeof(Brush), typeof(ImageGridSelector));

            public Brush ForegroundBrush
            {
                get => (Brush)GetValue(ForegroundBrushProperty);
                set => SetValue(ForegroundBrushProperty, value);
            }
            public static readonly DependencyProperty ForegroundBrushProperty =
                DependencyProperty.Register(nameof(ForegroundBrush), typeof(Brush), typeof(ImageGridSelector));

            public Brush AccentBrush
            {
                get => (Brush)GetValue(AccentBrushProperty);
                set => SetValue(AccentBrushProperty, value);
            }
            public static readonly DependencyProperty AccentBrushProperty =
                DependencyProperty.Register(nameof(AccentBrush), typeof(Brush), typeof(ImageGridSelector));

            // ===========================
            // 🧭 Boutons
            // ===========================

            public Brush UndoBackground
            {
                get => (Brush)GetValue(UndoBackgroundProperty);
                set => SetValue(UndoBackgroundProperty, value);
            }
            public static readonly DependencyProperty UndoBackgroundProperty =
                DependencyProperty.Register(nameof(UndoBackground), typeof(Brush), typeof(ImageGridSelector));

            public Brush UndoForeground
            {
                get => (Brush)GetValue(UndoForegroundProperty);
                set => SetValue(UndoForegroundProperty, value);
            }
            public static readonly DependencyProperty UndoForegroundProperty =
                DependencyProperty.Register(nameof(UndoForeground), typeof(Brush), typeof(ImageGridSelector));

            public Brush ApplyBackground
            {
                get => (Brush)GetValue(ApplyBackgroundProperty);
                set => SetValue(ApplyBackgroundProperty, value);
            }
            public static readonly DependencyProperty ApplyBackgroundProperty =
                DependencyProperty.Register(nameof(ApplyBackground), typeof(Brush), typeof(ImageGridSelector));

            public Brush ApplyForeground
            {
                get => (Brush)GetValue(ApplyForegroundProperty);
                set => SetValue(ApplyForegroundProperty, value);
            }
            public static readonly DependencyProperty ApplyForegroundProperty =
                DependencyProperty.Register(nameof(ApplyForeground), typeof(Brush), typeof(ImageGridSelector));

            // ===========================
            // 🧠 Contenu
            // ===========================

            public string ImageCountText
            {
                get => (string)GetValue(ImageCountTextProperty);
                set => SetValue(ImageCountTextProperty, value);
            }
            public static readonly DependencyProperty ImageCountTextProperty =
                DependencyProperty.Register(nameof(ImageCountText), typeof(string), typeof(ImageGridSelector), new PropertyMetadata("Aucune image chargée"));

            public ImageGridSelector()
            {
                InitializeComponent();
            }

            /// <summary>
            /// Initialise le grid avec un dossier spécifique
            /// </summary>
            public void LoadImagesFromFolder(string folderPath="/UI/Modules/AccountStaffManager/Assets")
            {
                _folderPath = folderPath;
                _imagePaths = new List<string>();

                if (!Directory.Exists(folderPath))
                {
                    MessageBox.Show($"Le dossier '{folderPath}' n'existe pas.", "Erreur",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Récupérer tous les fichiers images
                string[] extensions = { "*.jpg", "*.jpeg", "*.png" };
                foreach (var ext in extensions)
                {
                    _imagePaths.AddRange(Directory.GetFiles(folderPath, ext, SearchOption.TopDirectoryOnly));
                }

                if (_imagePaths.Count == 0)
                {
                    MessageBox.Show("Aucune image trouvée dans ce dossier.", "Information",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Afficher le nombre d'images
                ImageCountText  = $"{_imagePaths.Count} image(s) trouvée(s)";

                // Créer les boutons circulaires
                CreateImageButtons();
            }

            /// <summary>
            /// Définit le conteneur cible pour afficher les images
            /// </summary>
            public static void SetTargetContainer(Border container)
            {
                _targetContainer = container;
            }

            private void CreateImageButtons()
            {
                ImageGrid.Children.Clear();
                ImageGrid.RowDefinitions.Clear();
                ImageGrid.ColumnDefinitions.Clear();

                int columns = 5; // Nombre de colonnes dans le grid
                int rows = (int)Math.Ceiling(_imagePaths.Count / (double)columns);

                // Créer les définitions de colonnes et lignes
                for (int i = 0; i < columns; i++)
                {
                    ImageGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                }
                for (int i = 0; i < rows; i++)
                {
                    ImageGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                }

                // Créer un bouton circulaire pour chaque image
                for (int i = 0; i < _imagePaths.Count; i++)
                {
                    var imagePath = _imagePaths[i];
                    var button = CreateCircularImageButton(imagePath);

                    int row = i / columns;
                    int col = i % columns;

                    Grid.SetRow(button, row);
                    Grid.SetColumn(button, col);

                    ImageGrid.Children.Add(button);
                }
            }

            private Button CreateCircularImageButton(string imagePath)
            {
                var button = new Button
                {
                    Width = 80,
                    Height = 80,
                    Margin = new Thickness(5),
                    Style = null,
                    Background = Brushes.Transparent,
                    BorderThickness = new Thickness(0),
                    Cursor = System.Windows.Input.Cursors.Hand
                };

                // Créer un Ellipse avec l'image comme remplissage
                var ellipse = new Ellipse
                {
                    Width = 80,
                    Height = 80,
                    Stroke = (SolidColorBrush)Application.Current.TryFindResource("AM_Accent") ?? new SolidColorBrush(Color.FromRgb(32, 78, 122)),
                    StrokeThickness = 2
                };

                try
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
                    bitmap.DecodePixelWidth = 80;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();

                    ellipse.Fill = new ImageBrush
                    {
                        ImageSource = bitmap,
                        Stretch = Stretch.UniformToFill
                    };
                }
                catch (Exception ex)
                {
                    var fallbackBrush = (SolidColorBrush)Application.Current.TryFindResource("AM_Panel_Dark") ?? new SolidColorBrush(Color.FromRgb(14, 26, 44));
                    ellipse.Fill = fallbackBrush;
                    Console.WriteLine($"Erreur lors du chargement de l'image: {ex.Message}");
                }

                // Ajouter un effet de survol
                button.MouseEnter += (s, e) =>
                {
                    ellipse.StrokeThickness = 3;
                    var accentBrush = (SolidColorBrush)Application.Current.TryFindResource("AM_Foreground") ?? new SolidColorBrush(Color.FromRgb(230, 235, 245));
                    ellipse.Stroke = accentBrush;
                };

                button.MouseLeave += (s, e) =>
                {
                    ellipse.StrokeThickness = 2;
                    var accentBrush = (SolidColorBrush)Application.Current.TryFindResource("AM_Accent") ?? new SolidColorBrush(Color.FromRgb(32, 78, 122));
                    ellipse.Stroke = accentBrush;
                };

                button.Content = ellipse;
                button.Click += (s, e) => OnImageButtonClick(imagePath);
                button.ToolTip = System.IO.Path.GetFileName(imagePath);

                return button;
            }

        private static void OnImageButtonClick(string imagePath)
        {
            if (_targetContainer == null)
            {
                MessageBox.Show("Aucun conteneur cible n'a été défini.", "Erreur",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Sauvegarder le contenu actuel comme précédent
            if (_targetContainer.Child is not null)
            {
                _previousContent = _currentContent;
            }

            // Créer une image
            var image = new Image
            {
                Stretch = Stretch.UniformToFill,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                image.Source = bitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement de l'image: {ex.Message}", "Erreur",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Conteneur pour appliquer le masque circulaire
            var grid = new Grid
            {
                Width = 150,
                Height = 150,
                Margin = new Thickness(10),
            };

            // Masque circulaire dynamique
            grid.Loaded += (s, e) =>
            {
                double radius = Math.Min(grid.ActualWidth, grid.ActualHeight) / 2;
                grid.Clip = new EllipseGeometry(new Point(radius, radius), radius, radius);
            };

            grid.Children.Add(image);

            _currentContent = grid;

            // Rafraîchir le conteneur cible
            _targetContainer.Child = null;
            _targetContainer.Child = grid;
        }


        private void UndoButton_Click(object sender, RoutedEventArgs e)
            {
                if (_previousContent == null)
                {
                    MessageBox.Show("Aucune image précédente disponible.", "Information",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                if (_targetContainer == null)
                {
                    MessageBox.Show("Aucun conteneur cible n'a été défini.", "Erreur",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Échanger le contenu actuel et précédent
                var temp = _currentContent;
                _currentContent = _previousContent;
                _previousContent = temp;

            // Remettre le contenu précédent dans le conteneur
            _targetContainer.Child = null;
            _targetContainer.Child = _currentContent;
            }

            private void ApplyButton_Click(object sender, RoutedEventArgs e)
            {
                if (_currentContent == null)
                {
                    MessageBox.Show("Aucune image à appliquer.", "Information",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Effacer le cache - l'image actuelle est maintenant figée
                _previousContent = null;

                MessageBox.Show("Image appliquée avec succès! Le cache a été effacé.", "Succès",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }