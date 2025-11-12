using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LIBBRARY_MANAGER.Views.LoanViews
{
    public partial class LoanItemView : UserControl
    {
        #region Dependency Properties

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(
                nameof(IsSelected),
                typeof(bool),
                typeof(LoanItemView),
                new PropertyMetadata(false));

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public static readonly DependencyProperty ViewDetailsCommandProperty =
            DependencyProperty.Register(
                nameof(ViewDetailsCommand),
                typeof(ICommand),
                typeof(LoanItemView),
                new PropertyMetadata(null));

        public ICommand ViewDetailsCommand
        {
            get => (ICommand)GetValue(ViewDetailsCommandProperty);
            set => SetValue(ViewDetailsCommandProperty, value);
        }

        public static readonly DependencyProperty MarkReturnedCommandProperty =
            DependencyProperty.Register(
                nameof(MarkReturnedCommand),
                typeof(ICommand),
                typeof(LoanItemView),
                new PropertyMetadata(null));

        public ICommand MarkReturnedCommand
        {
            get => (ICommand)GetValue(MarkReturnedCommandProperty);
            set => SetValue(MarkReturnedCommandProperty, value);
        }

        #endregion

        #region Constructeur

        public LoanItemView()
        {
            InitializeComponent();

            System.Diagnostics.Debug.WriteLine("[LoanItemView] Contrôle créé");
        }

        #endregion

        #region Debugging

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == ViewDetailsCommandProperty)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[LoanItemView] ViewDetailsCommand changé: " +
                    $"{(e.NewValue != null ? "Connecté" : "NULL")}"
                );
            }

            if (e.Property == MarkReturnedCommandProperty)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[LoanItemView] MarkReturnedCommand changé: " +
                    $"{(e.NewValue != null ? "Connecté" : "NULL")}"
                );
            }

            if (e.Property == DataContextProperty)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[LoanItemView] DataContext changé: " +
                    $"{e.NewValue?.GetType().Name ?? "NULL"}"
                );
            }
        }

        #endregion
    }
}