using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace LIBBRARY_MANAGER.UI.Common.Items.MainView
{
    /// <summary>
    /// Logique d'interaction pour SearchResultMessage.xaml
    /// </summary>
    public partial class SearchResultMessage : UserControl
    {
        public static readonly DependencyProperty IsVisibleProperty_ =
            DependencyProperty.Register(nameof(IsVisible_), typeof(bool), typeof(SearchResultMessage), new PropertyMetadata(false));


        // 1. Définition des DP "brutes" à binder/setter
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register(nameof(Message), typeof(string), typeof(SearchResultMessage),
            new PropertyMetadata("", OnDependencyChanged));

        public static readonly DependencyProperty ResultCountProperty =
            DependencyProperty.Register(nameof(ResultCount), typeof(int), typeof(SearchResultMessage),
            new PropertyMetadata(0, OnDependencyChanged));

        // 2. Propriétés de binding/set direct (ViewModel ou XAML)
        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        public int ResultCount
        {
            get => (int)GetValue(ResultCountProperty);
            set => SetValue(ResultCountProperty, value);
        }

        // 3. Propriétés calculées pour affichage (XAML)
        public string FormattedMessage =>
            string.IsNullOrWhiteSpace(Message)
                ? "Aucune recherche effectuée."
                : $"Résultat(s) pour la recherche « {Message} »";

        public string FormattedResultCount =>
            ResultCount > 0
                ? $"({ResultCount} livre{(ResultCount > 1 ? "s" : "")} trouvé{(ResultCount > 1 ? "s" : "")})"
                : "(Aucun livre trouvé)";

        // 4. Mise à jour de l'affichage lorsque la DP change
        private static void OnDependencyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SearchResultMessage srm)
            {
                srm.OnPropertyChanged(nameof(FormattedMessage));
                srm.OnPropertyChanged(nameof(FormattedResultCount));
            }
        }

        // Implémentation INotifyPropertyChanged pour binding XAML
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));



        public bool IsVisible_
        {
            get => (bool)GetValue(IsVisibleProperty_);
            set => SetValue(IsVisibleProperty_, value);
        }

        public SearchResultMessage()
        {
            InitializeComponent();
        }
    }
}