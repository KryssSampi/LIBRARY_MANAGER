using System;
using System.Collections.Generic;
using System.Windows.Media;
using MahApps.Metro.IconPacks;
using LIBBRARY_MANAGER.Model;
using static LIBBRARY_MANAGER.Model.Book;

namespace LIBBRARY_MANAGER.UI.Modules.BookCatalog.Config
{
    /// <summary>
    /// Modèle de configuration pour chaque catégorie
    /// </summary>
    public class CategoryConfig
    {
        public CategoryAllowed Category { get; set; }
        public string DisplayName { get; set; }
        public PackIconMaterialKind Icon { get; set; }
        public Brush IconColor { get; set; }
        public Brush BackgroundBrush { get; set; } // Utilisé pour extraire la couleur de bordure
        public Brush BorderColor { get; set; } // Couleur de bordure extraite
    }

    /// <summary>
    /// Configuration centralisée de toutes les catégories avec leurs styles
    /// </summary>
    public static class CategoryConfiguration
    {
        private static readonly Dictionary<CategoryAllowed, CategoryConfig> _configurations;

        static CategoryConfiguration()
        {
            _configurations = new Dictionary<CategoryAllowed, CategoryConfig>
            {
                //Tous
                    {
            CategoryAllowed.Tous,
            new CategoryConfig
            {
                Category = CategoryAllowed.Tous,
                DisplayName = "Tous les livres",
                Icon = PackIconMaterialKind.BookMultiple, // Ou BookshelfOutline
                IconColor = new SolidColorBrush(Color.FromRgb(21, 101, 192)), // Bleu NOCTUA
                BackgroundBrush = CreateGradientBrush("#E3F2FD", "#BBDEFB")
            }
        },
                // 🧠 SCIENCES & SAVOIRS
                {
                    CategoryAllowed.Science,
                    new CategoryConfig
                    {
                        Category = CategoryAllowed.Science,
                        DisplayName = "Science",
                        Icon = PackIconMaterialKind.Flask,
                        IconColor = new SolidColorBrush(Color.FromRgb(56, 142, 60)), // Vert
                        BackgroundBrush = CreateGradientBrush("#E8F5E9", "#C8E6C9")
                    }
                },
                {
                    CategoryAllowed.Technologie,
                    new CategoryConfig
                    {
                        Category = CategoryAllowed.Technologie,
                        DisplayName = "Technologie",
                        Icon = PackIconMaterialKind.Cog,
                        IconColor = new SolidColorBrush(Color.FromRgb(123, 31, 162)), // Violet
                        BackgroundBrush = CreateGradientBrush("#F3E5F5", "#E1BEE7")
                    }
                },
                {
                    CategoryAllowed.Informatique,
                    new CategoryConfig
                    {
                        Category = CategoryAllowed.Informatique,
                        DisplayName = "Informatique",
                        Icon = PackIconMaterialKind.Laptop,
                        IconColor = new SolidColorBrush(Color.FromRgb(21, 101, 192)), // Bleu foncé
                        BackgroundBrush = CreateGradientBrush("#E3F2FD", "#BBDEFB")
                    }
                },
                {
                    CategoryAllowed.Médecine,
                    new CategoryConfig
                    {
                        Category = CategoryAllowed.Médecine,
                        DisplayName = "Médecine",
                        Icon = PackIconMaterialKind.Stethoscope,
                        IconColor = new SolidColorBrush(Color.FromRgb(244, 67, 54)), // Rouge
                        BackgroundBrush = CreateGradientBrush("#FFEBEE", "#FFCDD2")
                    }
                },
                {
                    CategoryAllowed.Mathématiques,
                    new CategoryConfig
                    {
                        Category = CategoryAllowed.Mathématiques,
                        DisplayName = "Mathématiques",
                        Icon = PackIconMaterialKind.Function,
                        IconColor = new SolidColorBrush(Color.FromRgb(0, 150, 136)), // Teal
                        BackgroundBrush = CreateGradientBrush("#E0F2F1", "#B2DFDB")
                    }
                },
                {
                    CategoryAllowed.Philosophie,
                    new CategoryConfig
                    {
                        Category = CategoryAllowed.Philosophie,
                        DisplayName = "Philosophie",
                        Icon = PackIconMaterialKind.Brain,
                        IconColor = new SolidColorBrush(Color.FromRgb(93, 64, 55)), // Marron
                        BackgroundBrush = CreateGradientBrush("#EFEBE9", "#D7CCC8")
                    }
                },
                {
                    CategoryAllowed.Psychologie,
                    new CategoryConfig
                    {
                        Category = CategoryAllowed.Psychologie,
                        DisplayName = "Psychologie",
                        Icon = PackIconMaterialKind.HeadLightbulb,
                        IconColor = new SolidColorBrush(Color.FromRgb(156, 39, 176)), // Violet clair
                        BackgroundBrush = CreateGradientBrush("#F3E5F5", "#E1BEE7")
                    }
                },
                {
                    CategoryAllowed.Éducation,
                    new CategoryConfig
                    {
                        Category = CategoryAllowed.Éducation,
                        DisplayName = "Éducation",
                        Icon = PackIconMaterialKind.School,
                        IconColor = new SolidColorBrush(Color.FromRgb(255, 152, 0)), // Orange
                        BackgroundBrush = CreateGradientBrush("#FFF3E0", "#FFE0B2")
                    }
                },

                // 📚 LITTÉRATURE
                {
                    CategoryAllowed.Roman,
                    new CategoryConfig
                    {
                        Category = CategoryAllowed.Roman,
                        DisplayName = "Roman",
                        Icon = PackIconMaterialKind.BookOpenPageVariant,
                        IconColor = new SolidColorBrush(Color.FromRgb(21, 101, 192)), // Bleu
                        BackgroundBrush = CreateGradientBrush("#E3F2FD", "#BBDEFB")
                    }
                },
                {
                    CategoryAllowed.Nouvelle,
                    new CategoryConfig
                    {
                        Category = CategoryAllowed.Nouvelle,
                        DisplayName = "Nouvelle",
                        Icon = PackIconMaterialKind.BookVariant,
                        IconColor = new SolidColorBrush(Color.FromRgb(66, 165, 245)), // Bleu clair
                        BackgroundBrush = CreateGradientBrush("#E1F5FE", "#B3E5FC")
                    }
                },
                {
                    CategoryAllowed.Poésie,
                    new CategoryConfig
                    {
                        Category = CategoryAllowed.Poésie,
                        DisplayName = "Poésie",
                        Icon = PackIconMaterialKind.FormatQuoteClose,
                        IconColor = new SolidColorBrush(Color.FromRgb(233, 30, 99)), // Rose
                        BackgroundBrush = CreateGradientBrush("#FCE4EC", "#F8BBD0")
                    }
                },
                {
                    CategoryAllowed.Théâtre,
                    new CategoryConfig
                    {
                        Category = CategoryAllowed.Théâtre,
                        DisplayName = "Théâtre",
                        Icon = PackIconMaterialKind.DramaMasks,
                        IconColor = new SolidColorBrush(Color.FromRgb(156, 39, 176)), // Violet
                        BackgroundBrush = CreateGradientBrush("#F3E5F5", "#E1BEE7")
                    }
                },
                {
                    CategoryAllowed.Biographie,
                    new CategoryConfig
                    {
                        Category = CategoryAllowed.Biographie,
                        DisplayName = "Biographie",
                        Icon = PackIconMaterialKind.AccountBox,
                        IconColor = new SolidColorBrush(Color.FromRgb(121, 85, 72)), // Marron clair
                        BackgroundBrush = CreateGradientBrush("#EFEBE9", "#D7CCC8")
                    }
                },
                {
                    CategoryAllowed.Essai,
                    new CategoryConfig
                    {
                        Category = CategoryAllowed.Essai,
                        DisplayName = "Essai",
                        Icon = PackIconMaterialKind.FileDocument,
                        IconColor = new SolidColorBrush(Color.FromRgb(96, 125, 139)), // Bleu-gris
                        BackgroundBrush = CreateGradientBrush("#ECEFF1", "#CFD8DC")
                    }
                },
                {
                    CategoryAllowed.Classique,
                    new CategoryConfig
                    {
                        Category = CategoryAllowed.Classique,
                        DisplayName = "Classique",
                        Icon = PackIconMaterialKind.BookOpenBlankVariant,
                        IconColor = new SolidColorBrush(Color.FromRgb(84, 110, 122)), // Gris-bleu
                        BackgroundBrush = CreateGradientBrush("#ECEFF1", "#CFD8DC")
                    }
                },

                // 🌍 HISTOIRE & SOCIÉTÉ
                {
                    CategoryAllowed.Histoire,
                    new CategoryConfig
                    {
                        Category = CategoryAllowed.Histoire,
                        DisplayName = "Histoire",
                        Icon = PackIconMaterialKind.Castle,
                        IconColor = new SolidColorBrush(Color.FromRgb(245, 124, 0)), // Orange
                        BackgroundBrush = CreateGradientBrush("#FFF3E0", "#FFE0B2")
                    }
                },
                {
                    CategoryAllowed.Politique,
                    new CategoryConfig
                    {
                        Category = CategoryAllowed.Politique,
                        DisplayName = "Politique",
                        Icon = PackIconMaterialKind.Bank,
                        IconColor = new SolidColorBrush(Color.FromRgb(63, 81, 181)), // Indigo
                        BackgroundBrush = CreateGradientBrush("#E8EAF6", "#C5CAE9")
                    }
                },
                {
                    CategoryAllowed.Économie,
                    new CategoryConfig
                    {
                        Category = CategoryAllowed.Économie,
                        DisplayName = "Économie",
                        Icon = PackIconMaterialKind.ChartLine,
                        IconColor = new SolidColorBrush(Color.FromRgb(0, 150, 136)), // Teal
                        BackgroundBrush = CreateGradientBrush("#E0F2F1", "#B2DFDB")
                    }
                },
                {
                    CategoryAllowed.Société,
                    new CategoryConfig
                    {
                        Category = CategoryAllowed.Société,
                        DisplayName = "Société",
                        Icon = PackIconMaterialKind.AccountGroup,
                        IconColor = new SolidColorBrush(Color.FromRgb(76, 175, 80)), // Vert
                        BackgroundBrush = CreateGradientBrush("#E8F5E9", "#C8E6C9")
                    }
                },
                {
                    CategoryAllowed.Droit,
                    new CategoryConfig
                    {
                        Category = CategoryAllowed.Droit,
                        DisplayName = "Droit",
                        Icon = PackIconMaterialKind.Gavel,
                        IconColor = new SolidColorBrush(Color.FromRgb(96, 125, 139)), // Bleu-gris
                        BackgroundBrush = CreateGradientBrush("#ECEFF1", "#CFD8DC")
                    }
                },
                {
                    CategoryAllowed.Religion,
                    new CategoryConfig
                    {
                        Category = CategoryAllowed.Religion,
                        DisplayName = "Religion",
                        Icon = PackIconMaterialKind.Church,
                        IconColor = new SolidColorBrush(Color.FromRgb(121, 85, 72)), // Marron
                        BackgroundBrush = CreateGradientBrush("#EFEBE9", "#D7CCC8")
                    }
                },
                {
                    CategoryAllowed.Géographie,
                    new CategoryConfig
                    {
                        Category = CategoryAllowed.Géographie,
                        DisplayName = "Géographie",
                        Icon = PackIconMaterialKind.Earth,
                        IconColor = new SolidColorBrush(Color.FromRgb(3, 169, 244)), // Bleu ciel
                        BackgroundBrush = CreateGradientBrush("#E1F5FE", "#B3E5FC")
                    }
                },

                // 🎨 ARTS & CULTURE
                {
                    CategoryAllowed.Art,
                    new CategoryConfig
                    {
                        Category = CategoryAllowed.Art,
                        DisplayName = "Art",
                        Icon = PackIconMaterialKind.Palette,
                        IconColor = new SolidColorBrush(Color.FromRgb(194, 24, 91)), // Rose foncé
                        BackgroundBrush = CreateGradientBrush("#FCE4EC", "#F8BBD0")
                    }
                },
                {
                    CategoryAllowed.Musique,
                    new CategoryConfig
                    {
                        Category = CategoryAllowed.Musique,
                        DisplayName = "Musique",
                        Icon = PackIconMaterialKind.MusicNote,
                        IconColor = new SolidColorBrush(Color.FromRgb(156, 39, 176)), // Violet
                        BackgroundBrush = CreateGradientBrush("#F3E5F5", "#E1BEE7")
                    }
                },
                {
                    CategoryAllowed.Cinéma,
                    new CategoryConfig
                    {
                        Category = CategoryAllowed.Cinéma,
                        DisplayName = "Cinéma",
                        Icon = PackIconMaterialKind.MovieOpen,
                        IconColor = new SolidColorBrush(Color.FromRgb(233, 30, 99)), // Rose
                        BackgroundBrush = CreateGradientBrush("#FCE4EC", "#F8BBD0")
                    }
                },
                {
                    CategoryAllowed.Architecture,
                    new CategoryConfig
                    {
                        Category = CategoryAllowed.Architecture,
                        DisplayName = "Architecture",
                        Icon = PackIconMaterialKind.Domain,
                        IconColor = new SolidColorBrush(Color.FromRgb(96, 125, 139)), // Bleu-gris
                        BackgroundBrush = CreateGradientBrush("#ECEFF1", "#CFD8DC")
                    }
                },
                {
                    CategoryAllowed.Design,
                    new CategoryConfig
                    {
                        Category = CategoryAllowed.Design,
                        DisplayName = "Design",
                        Icon = PackIconMaterialKind.Draw,
                        IconColor = new SolidColorBrush(Color.FromRgb(233, 30, 99)), // Rose
                        BackgroundBrush = CreateGradientBrush("#FCE4EC", "#F8BBD0")
                    }
                },
                {
                    CategoryAllowed.Photographie,
                    new CategoryConfig
                    {
                        Category = CategoryAllowed.Photographie,
                        DisplayName = "Photographie",
                        Icon = PackIconMaterialKind.Camera,
                        IconColor = new SolidColorBrush(Color.FromRgb(121, 85, 72)), // Marron
                        BackgroundBrush = CreateGradientBrush("#EFEBE9", "#D7CCC8")
                    }
                },

                // 🌿 NATURE & VIE PRATIQUE
                {
                    CategoryAllowed.Nature,
                    new CategoryConfig
                    {
                        Category = CategoryAllowed.Nature,
                        DisplayName = "Nature",
                        Icon = PackIconMaterialKind.Tree,
                        IconColor = new SolidColorBrush(Color.FromRgb(76, 175, 80)), // Vert
                        BackgroundBrush = CreateGradientBrush("#E8F5E9", "#C8E6C9")
                    }
                },
                {
                    CategoryAllowed.Environnement,
                    new CategoryConfig
                    {
                        Category = CategoryAllowed.Environnement,
                        DisplayName = "Environnement",
                        Icon = PackIconMaterialKind.Leaf,
                        IconColor = new SolidColorBrush(Color.FromRgb(56, 142, 60)), // Vert foncé
                        BackgroundBrush = CreateGradientBrush("#E8F5E9", "#C8E6C9")
                    }
                },
                {
                    CategoryAllowed.Cuisine,
                    new CategoryConfig
                    {
                        Category = CategoryAllowed.Cuisine,
                        DisplayName = "Cuisine",
                        Icon = PackIconMaterialKind.ChefHat,
                        IconColor = new SolidColorBrush(Color.FromRgb(255, 152, 0)), // Orange
                        BackgroundBrush = CreateGradientBrush("#FFF3E0", "#FFE0B2")
                    }
                },
                {
                    CategoryAllowed.Jardinage,
                    new CategoryConfig
                    {
                        Category = CategoryAllowed.Jardinage,
                        DisplayName = "Jardinage",
                        Icon = PackIconMaterialKind.Flower,
                        IconColor = new SolidColorBrush(Color.FromRgb(139, 195, 74)), // Vert clair
                        BackgroundBrush = CreateGradientBrush("#F1F8E9", "#DCEDC8")
                    }
                },
                {
                    CategoryAllowed.Santé,
                    new CategoryConfig
                    {
                        Category = CategoryAllowed.Santé,
                        DisplayName = "Santé",
                        Icon = PackIconMaterialKind.HeartPulse,
                        IconColor = new SolidColorBrush(Color.FromRgb(244, 67, 54)), // Rouge
                        BackgroundBrush = CreateGradientBrush("#FFEBEE", "#FFCDD2")
                    }
                },
                {
                    CategoryAllowed.Sport,
                    new CategoryConfig
                    {
                        Category = CategoryAllowed.Sport,
                        DisplayName = "Sport",
                        Icon = PackIconMaterialKind.Basketball,
                        IconColor = new SolidColorBrush(Color.FromRgb(255, 87, 34)), // Orange vif
                        BackgroundBrush = CreateGradientBrush("#FBE9E7", "#FFCCBC")
                    }
                },
                {
                    CategoryAllowed.Voyage,
                    new CategoryConfig
                    {
                        Category = CategoryAllowed.Voyage,
                        DisplayName = "Voyage",
                        Icon = PackIconMaterialKind.AirplaneTakeoff,
                        IconColor = new SolidColorBrush(Color.FromRgb(3, 169, 244)), // Bleu ciel
                        BackgroundBrush = CreateGradientBrush("#E1F5FE", "#B3E5FC")
                    }
                },

                // 👶 JEUNESSE & DIVERTISSEMENT
                {
                    CategoryAllowed.Jeunesse,
                    new CategoryConfig
                    {
                        Category = CategoryAllowed.Jeunesse,
                        DisplayName = "Jeunesse",
                        Icon = PackIconMaterialKind.TeddyBear,
                        IconColor = new SolidColorBrush(Color.FromRgb(249, 168, 37)), // Jaune-or
                        BackgroundBrush = CreateGradientBrush("#FFF9C4", "#FFF59D")
                    }
                },
                {
                    CategoryAllowed.Manga,
                    new CategoryConfig
                    {
                        Category = CategoryAllowed.Manga,
                        DisplayName = "Manga",
                        Icon = PackIconMaterialKind.BookOpenPageVariantOutline,
                        IconColor = new SolidColorBrush(Color.FromRgb(233, 30, 99)), // Rose
                        BackgroundBrush = CreateGradientBrush("#FCE4EC", "#F8BBD0")
                    }
                },
                {
                    CategoryAllowed.BandeDessinee,
                    new CategoryConfig
                    {
                        Category = CategoryAllowed.BandeDessinee,
                        DisplayName = "Bande Dessinée",
                        Icon = PackIconMaterialKind.ThoughtBubble,
                        IconColor = new SolidColorBrush(Color.FromRgb(255, 193, 7)), // Jaune
                        BackgroundBrush = CreateGradientBrush("#FFF9C4", "#FFF59D")
                    }
                },
                {
                    CategoryAllowed.Fantasy,
                    new CategoryConfig
                    {
                        Category = CategoryAllowed.Fantasy,
                        DisplayName = "Fantasy",
                        Icon = PackIconMaterialKind.AutoFix,
                        IconColor = new SolidColorBrush(Color.FromRgb(103, 58, 183)), // Violet profond
                        BackgroundBrush = CreateGradientBrush("#EDE7F6", "#D1C4E9")
                    }
                },
                {
                    CategoryAllowed.ScienceFiction,
                    new CategoryConfig
                    {
                        Category = CategoryAllowed.ScienceFiction,
                        DisplayName = "Science-Fiction",
                        Icon = PackIconMaterialKind.Rocket,
                        IconColor = new SolidColorBrush(Color.FromRgb(33, 150, 243)), // Bleu
                        BackgroundBrush = CreateGradientBrush("#E3F2FD", "#BBDEFB")
                    }
                },
                {
                    CategoryAllowed.Policier,
                    new CategoryConfig
                    {
                        Category = CategoryAllowed.Policier,
                        DisplayName = "Policier",
                        Icon = PackIconMaterialKind.Magnify,
                        IconColor = new SolidColorBrush(Color.FromRgb(96, 125, 139)), // Gris-bleu
                        BackgroundBrush = CreateGradientBrush("#ECEFF1", "#CFD8DC")
                    }
                },
                {
                    CategoryAllowed.Aventure,
                    new CategoryConfig
                    {
                        Category = CategoryAllowed.Aventure,
                        DisplayName = "Aventure",
                        Icon = PackIconMaterialKind.Compass,
                        IconColor = new SolidColorBrush(Color.FromRgb(255, 152, 0)), // Orange
                        BackgroundBrush = CreateGradientBrush("#FFF3E0", "#FFE0B2")
                    }
                },
                {
                    CategoryAllowed.Romance,
                    new CategoryConfig
                    {
                        Category = CategoryAllowed.Romance,
                        DisplayName = "Romance",
                        Icon = PackIconMaterialKind.Heart,
                        IconColor = new SolidColorBrush(Color.FromRgb(233, 30, 99)), // Rose
                        BackgroundBrush = CreateGradientBrush("#FCE4EC", "#F8BBD0")
                    }
                },
                {
                    CategoryAllowed.Horreur,
                    new CategoryConfig
                    {
                        Category = CategoryAllowed.Horreur,
                        DisplayName = "Horreur",
                        Icon = PackIconMaterialKind.GhostOutline,
                        IconColor = new SolidColorBrush(Color.FromRgb(96, 125, 139)), // Gris foncé
                        BackgroundBrush = CreateGradientBrush("#ECEFF1", "#CFD8DC")
                    }
                }
            };
        }

        /// <summary>
        /// Obtient la configuration d'une catégorie
        /// </summary>
        public static CategoryConfig GetConfig(CategoryAllowed category)
        {
            return _configurations.TryGetValue(category, out var config) ? config : null;
        }

        /// <summary>
        /// Obtient toutes les configurations
        /// </summary>
        public static IEnumerable<CategoryConfig> GetAllConfigs()
        {
            return _configurations.Values;
        }

        /// <summary>
        /// Crée un gradient brush à partir de deux couleurs hex
        /// </summary>
        public static LinearGradientBrush CreateGradientBrush(string startColor, string endColor )
        {
            // Petite fonction locale pour assombrir une couleur
            static Color DarkenColor(Color color, double factor = 0.85)
            {
                return Color.FromRgb(
                    (byte)(color.R * factor),
                    (byte)(color.G * factor),
                    (byte)(color.B * factor)
                );
            }

            // Conversion et assombrissement
            var start = (Color)ColorConverter.ConvertFromString(startColor);
            var end = (Color)ColorConverter.ConvertFromString(endColor);

            start = DarkenColor(start);
            end = DarkenColor(end);

            // Création du dégradé
            var brush = new LinearGradientBrush
            {
                StartPoint = new System.Windows.Point(0, 0),
                EndPoint = new System.Windows.Point(0, 1)
            };

            brush.GradientStops.Add(new GradientStop(start, 0));
            brush.GradientStops.Add(new GradientStop(end, 1));

            return brush;
        }

    } 
}