using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace LIBBRARY_MANAGER.UI.Common.Converters
{
    /// <summary>
    /// Convertit bool vers Visibility (true = Visible, false = Collapsed)
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility == Visibility.Visible;
            }
            return false;
        }
    }

    /// <summary>
    /// Convertit bool vers Visibility inversé (true = Collapsed, false = Visible)
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class InverseBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Visibility.Collapsed : Visibility.Visible;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility != Visibility.Visible;
            }
            return true;
        }
    }
    /// <summary>
    /// Convertit un booléen en couleur (vert si vrai, rouge si faux)
    /// </summary>
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool canBorrow)
            {
                return canBorrow
                    ? new SolidColorBrush(Color.FromRgb(46, 125, 50))   // ✅ Vert (autorisé)
                    : new SolidColorBrush(Color.FromRgb(198, 40, 40));  // 🚫 Rouge (interdit)
            }

            // Valeur par défaut neutre
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// Convertit un booléen en texte lisible ("Oui"/"Non" ou "Autorisé"/"Suspendu")
    /// </summary>
    public class BoolToTextConverter : IValueConverter
    {
        public string TrueText { get; set; } = "Autorisé";
        public string FalseText { get; set; } = "Suspendu";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool canBorrow)
                return canBorrow ? TrueText : FalseText;

            return FalseText;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class PenaltyToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Brushes.Transparent;

            if (value is decimal penalty)
            {
                return penalty > 0 ? Brushes.Red : Brushes.Green;
            }

            // Support facultatif pour double / float
            if (value is double d)
            {
                return d > 0 ? Brushes.Red : Brushes.Green;
            }

            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class LoanStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isActive)
                return isActive ? Brushes.Green : Brushes.Gray;
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
    public class LoanLateColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isLate)
                return isLate ? Brushes.Red : Brushes.Green;
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
    public class LoanStatusToTextConverter : IValueConverter
    {
        // Convertit un bool (IsActive) en libellé lisible.
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isActive)
            {
                return isActive ? "En cours" : "Terminé";
            }

            // Si nullable / indéfini, renvoyer une chaîne neutre
            return "—";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
    /// <summary>
    /// Convertisseur pour inverser une valeur booléenne
    /// </summary>
    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return false;
        }
    }

    /// <summary>
    /// Convertisseur pour transformer une string vide/null en Visibility
    /// </summary>
    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && !string.IsNullOrEmpty(str))
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// Convertisseur MultiBinding pour afficher les indicateurs de validation
    /// Affiche l'indicateur seulement si ShowValidationIndicators est true ET si le champ a la validation demandée
    /// </summary>
    public class ValidationVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2)
                return Visibility.Collapsed;

            // values[0] = ShowValidationIndicators (bool)
            // values[1] = IsValid (bool) - true pour check, false pour cross

            if (values[0] is bool showIndicators && values[1] is bool isValid)
            {
                // Afficher uniquement si on a déjà soumis ET que la condition est remplie
                return showIndicators && isValid ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class NullToVisibilityReverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}