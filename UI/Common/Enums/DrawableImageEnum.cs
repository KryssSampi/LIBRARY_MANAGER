using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LIBBRARY_MANAGER.UI.Common.Enums
{
    /// <summary>
    /// Enum auto-générée depuis les images du dossier Assets/Drawable
    /// Pour ajouter une nouvelle icône : placez simplement l'image dans le dossier
    /// </summary>
    public enum DrawableImage
    {
        None = 0,
        // Ces valeurs seront générées automatiquement par le T4 Template
        // Ou utilisez le code ci-dessous pour générer manuellement

        // Exemples (à remplacer par vos vraies images) :
        Book,
        User,
        Search,
        Settings,
        Logout,
        Add,
        Edit,
        Delete,
        Save,
        Cancel,
        Home,
        Library,
        Dashboard,
        Report,
        Calendar,
        Notification,
        Profile,
        Help
    }

    /// <summary>
    /// Helper pour obtenir le chemin de l'image depuis l'enum
    /// </summary>
    public static class DrawableImageHelper
    {
        private const string DRAWABLE_FOLDER = "/UI/Common/Items/IconButton/Icon_Img";
        private static readonly Dictionary<DrawableImage, string> _imageCache = new();

        static DrawableImageHelper()
        {
            LoadImagePaths();
        }

        /// <summary>
        /// Charge tous les chemins d'images disponibles
        /// </summary>
        private static void LoadImagePaths()
        {
            _imageCache.Clear();
            _imageCache[DrawableImage.None] = null;

            try
            {
                var basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DRAWABLE_FOLDER);

                if (!Directory.Exists(basePath))
                {
                    // Essayer le chemin relatif pour le designer
                    basePath = Path.Combine("../../", DRAWABLE_FOLDER);
                }

                if (Directory.Exists(basePath))
                {
                    var extensions = new[] { ".png", ".jpg", ".jpeg", ".svg", ".ico", ".bmp" };
                    var files = Directory.GetFiles(basePath, "*.*")
                        .Where(f => extensions.Contains(Path.GetExtension(f).ToLower()))
                        .ToList();

                    foreach (var file in files)
                    {
                        var fileName = Path.GetFileNameWithoutExtension(file);

                        // Convertir le nom de fichier en valeur enum
                        // "book_icon.png" -> "BookIcon"
                        var enumName = ConvertToEnumName(fileName);

                        if (Enum.TryParse<DrawableImage>(enumName, true, out var enumValue))
                        {
                            // Pack URI pour WPF
                            _imageCache[enumValue] = $"/{typeof(DrawableImageHelper).Assembly.GetName().Name};component/{DRAWABLE_FOLDER}/{Path.GetFileName(file)}";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors du chargement des images: {ex.Message}");
            }
        }

        /// <summary>
        /// Convertit un nom de fichier en nom d'enum valide
        /// </summary>
        private static string ConvertToEnumName(string fileName)
        {
            // Remplace les caractères spéciaux par des underscores
            fileName = System.Text.RegularExpressions.Regex.Replace(fileName, @"[^a-zA-Z0-9_]", "_");

            // Première lettre en majuscule, reste en PascalCase
            var parts = fileName.Split(new[] { '_', '-', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return string.Join("", parts.Select(p => char.ToUpper(p[0]) + p.Substring(1).ToLower()));
        }

        /// <summary>
        /// Obtient le Pack URI de l'image
        /// </summary>
        public static string GetImagePath(DrawableImage image)
        {
            return _imageCache.TryGetValue(image, out var path) ? path : null;
        }

        /// <summary>
        /// Vérifie si une image existe
        /// </summary>
        public static bool HasImage(DrawableImage image)
        {
            return image != DrawableImage.None && _imageCache.ContainsKey(image);
        }

        /// <summary>
        /// Obtient toutes les images disponibles
        /// </summary>
        public static IEnumerable<DrawableImage> GetAvailableImages()
        {
            return _imageCache.Keys.Where(k => k != DrawableImage.None);
        }

        /// <summary>
        /// Recharge les chemins d'images (utile si des images sont ajoutées à l'exécution)
        /// </summary>
        public static void Refresh()
        {
            LoadImagePaths();
        }
    }
}