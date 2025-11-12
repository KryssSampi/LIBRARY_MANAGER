using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace LIBBRARY_MANAGER.UI.Common.Items.MainView
{
    /// <summary>
    /// Représente un label de filtre actif (ex: "Auteur : Hugo" ✕)
    /// </summary>
    public partial class ActiveFiltersLabel : UserControl
    {
        #region Dependency Properties

        public string TypeText
        {
            get => (string)GetValue(TypeTextProperty);
            set => SetValue(TypeTextProperty, value);
        }
        public static readonly DependencyProperty TypeTextProperty =
            DependencyProperty.Register(nameof(TypeText), typeof(string), typeof(ActiveFiltersLabel), new PropertyMetadata(string.Empty));

        public string ValueText
        {
            get => (string)GetValue(ValueTextProperty);
            set => SetValue(ValueTextProperty, value);
        }
        public static readonly DependencyProperty ValueTextProperty =
            DependencyProperty.Register(nameof(ValueText), typeof(string), typeof(ActiveFiltersLabel), new PropertyMetadata(string.Empty));

        public object FilterId
        {
            get => GetValue(FilterIdProperty);
            set => SetValue(FilterIdProperty, value);
        }
        public static readonly DependencyProperty FilterIdProperty =
            DependencyProperty.Register(nameof(FilterId), typeof(object), typeof(ActiveFiltersLabel), new PropertyMetadata(null));

        public ICommand CloseCommand
        {
            get => (ICommand)GetValue(CloseCommandProperty);
            set => SetValue(CloseCommandProperty, value);

        }
        public static readonly DependencyProperty CloseCommandProperty =
            DependencyProperty.Register(nameof(CloseCommand), typeof(ICommand), typeof(ActiveFiltersLabel), new PropertyMetadata(null));

        

        public Brush Foreground
        {
            get => (Brush)GetValue(ForegroundProperty);
            set => SetValue(ForegroundProperty, value);
        }
        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register(nameof(Foreground), typeof(Brush), typeof(ActiveFiltersLabel), new PropertyMetadata(Brushes.Black));

        public Brush LabelBackground
        {
            get => (Brush)GetValue(LabelBackgroundProperty);
            set => SetValue(LabelBackgroundProperty, value);
        }
        public static readonly DependencyProperty LabelBackgroundProperty =
            DependencyProperty.Register(nameof(LabelBackground), typeof(Brush), typeof(ActiveFiltersLabel), new PropertyMetadata(Brushes.White));

        #endregion


        public ActiveFiltersLabel()
        {
            InitializeComponent();
        }
    }
}
