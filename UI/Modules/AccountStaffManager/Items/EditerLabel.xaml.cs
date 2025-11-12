using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace LIBBRARY_MANAGER.UI.Modules.AccountStaffManager.Items
{
    public partial class EditerLabel : UserControl
    {
        public EditerLabel()
        {
            InitializeComponent();
            Pencil.Command = new RelayCommand(() =>
            {
                if (IsReadOnly)
                {
                    InfoBox.IsEnabled = true;
                    IsReadOnly = false;
                    InfoBox.Focus();
                    InfoBox.CaretIndex = InfoBox.Text.Length;
                }
                EditCommand?.Execute(EditableInfos);
            });
         
        }

        // 🔹 Label principal
        public static readonly DependencyProperty LabelTitleProperty =
            DependencyProperty.Register(nameof(LabelTitle), typeof(string), typeof(EditerLabel), new PropertyMetadata(string.Empty));
        public string LabelTitle
        {
            get => (string)GetValue(LabelTitleProperty);
            set => SetValue(LabelTitleProperty, value);
        }

        // 🔹 Texte éditable
        public static readonly DependencyProperty EditableInfosProperty =
            DependencyProperty.Register(nameof(EditableInfos), typeof(string), typeof(EditerLabel), new PropertyMetadata(string.Empty));
        public string EditableInfos
        {
            get => (string)GetValue(EditableInfosProperty);
            set => SetValue(EditableInfosProperty, value);
        }

        // 🔹 Mode lecture / édition
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(EditerLabel), new PropertyMetadata(true));
        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        // 🔹 Commande externe
        public static readonly DependencyProperty EditCommandProperty =
            DependencyProperty.Register(nameof(EditCommand), typeof(ICommand), typeof(EditerLabel), new PropertyMetadata(null));
        public ICommand EditCommand
        {
            get => (ICommand)GetValue(EditCommandProperty);
            set => SetValue(EditCommandProperty, value);
        }

        // 🎨 Couleurs / Styles
        public static readonly DependencyProperty LabelTitleColorProperty =
            DependencyProperty.Register(nameof(LabelTitleColor), typeof(Brush), typeof(EditerLabel), new PropertyMetadata(Brushes.White));
        public Brush LabelTitleColor
        {
            get => (Brush)GetValue(LabelTitleColorProperty);
            set => SetValue(LabelTitleColorProperty, value);
        }

        public static readonly DependencyProperty LabelColorProperty =
            DependencyProperty.Register(nameof(LabelColor), typeof(Brush), typeof(EditerLabel), new PropertyMetadata(Brushes.SlateBlue));
        public Brush LabelColor
        {
            get => (Brush)GetValue(LabelColorProperty);
            set => SetValue(LabelColorProperty, value);
        }

        public static readonly DependencyProperty InfoColorProperty =
            DependencyProperty.Register(nameof(InfoColor), typeof(Brush), typeof(EditerLabel), new PropertyMetadata(Brushes.White));
        public Brush InfoColor
        {
            get => (Brush)GetValue(InfoColorProperty);
            set => SetValue(InfoColorProperty, value);
        }

        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register(nameof(BackgroundColor), typeof(Brush), typeof(EditerLabel), new PropertyMetadata(Brushes.Transparent));
        public Brush BackgroundColor
        {
            get => (Brush)GetValue(BackgroundColorProperty);
            set => SetValue(BackgroundColorProperty, value);
        }

        public static readonly DependencyProperty PencilColorProperty =
            DependencyProperty.Register(nameof(PencilColor), typeof(Brush), typeof(EditerLabel), new PropertyMetadata(Brushes.SlateBlue));
        public Brush PencilColor
        {
            get => (Brush)GetValue(PencilColorProperty);
            set => SetValue(PencilColorProperty, value);
        }

        public static readonly DependencyProperty IconKindProperty =
            DependencyProperty.Register(nameof(IconKind), typeof(string), typeof(EditerLabel), new PropertyMetadata("Pencil"));
        public string IconKind
        {
            get => (string)GetValue(IconKindProperty);
            set => SetValue(IconKindProperty, value);
        }

        public static readonly DependencyProperty PencilSizeProperty =
            DependencyProperty.Register(nameof(PencilSize), typeof(double), typeof(EditerLabel), new PropertyMetadata(16.0));
        public double PencilSize
        {
            get => (double)GetValue(PencilSizeProperty);
            set => SetValue(PencilSizeProperty, value);
        }

        // 🟢 Bord arrondi
        public static readonly DependencyProperty CornerRadiusValueProperty =
            DependencyProperty.Register(nameof(CornerRadiusValue), typeof(CornerRadius), typeof(EditerLabel), new PropertyMetadata(new CornerRadius(15)));
        public CornerRadius CornerRadiusValue
        {
            get => (CornerRadius)GetValue(CornerRadiusValueProperty);
            set => SetValue(CornerRadiusValueProperty, value);
        }

        // ✏️ Taille du texte et du titre
        public static readonly DependencyProperty TextTitleSizeProperty =
            DependencyProperty.Register(nameof(TextTitleSize), typeof(double), typeof(EditerLabel), new PropertyMetadata(13.0));
        public double TextTitleSize
        {
            get => (double)GetValue(TextTitleSizeProperty);
            set => SetValue(TextTitleSizeProperty, value);
        }

        public static readonly DependencyProperty InfoSizeProperty =
            DependencyProperty.Register(nameof(InfoSize), typeof(double), typeof(EditerLabel), new PropertyMetadata(14.0));
        public double InfoSize
        {
            get => (double)GetValue(InfoSizeProperty);
            set => SetValue(InfoSizeProperty, value);
        }

        // 🧩 Icônes de validation / erreur
        public static readonly DependencyProperty CrossIsVisibleProperty =
            DependencyProperty.Register(nameof(CrossIsVisible), typeof(bool), typeof(EditerLabel), new PropertyMetadata(false));
        public bool CrossIsVisible
        {
            get => (bool)GetValue(CrossIsVisibleProperty);
            set => SetValue(CrossIsVisibleProperty, value);
        }

        public static readonly DependencyProperty CheckIsVisibleProperty =
            DependencyProperty.Register(nameof(CheckIsVisible), typeof(bool), typeof(EditerLabel), new PropertyMetadata(false));
        public bool CheckIsVisible
        {
            get => (bool)GetValue(CheckIsVisibleProperty);
            set => SetValue(CheckIsVisibleProperty, value);
        }

        // 🗯️ Message d'état
        public static readonly DependencyProperty StateMessageTextProperty =
            DependencyProperty.Register(nameof(StateMessageText), typeof(string), typeof(EditerLabel), new PropertyMetadata(string.Empty));
        public string StateMessageText
        {
            get => (string)GetValue(StateMessageTextProperty);
            set => SetValue(StateMessageTextProperty, value);
        }

        public static readonly DependencyProperty StateMessageColorProperty =
            DependencyProperty.Register(nameof(StateMessageColor), typeof(Brush), typeof(EditerLabel), new PropertyMetadata(Brushes.White));
        public Brush StateMessageColor
        {
            get => (Brush)GetValue(StateMessageColorProperty);
            set => SetValue(StateMessageColorProperty, value);
        }
        public static readonly DependencyProperty StateMessageSizeProperty =
    DependencyProperty.Register(nameof(StateMessageSize), typeof(double), typeof(EditerLabel), new PropertyMetadata(16.0));
        public double StateMessageSize
        {
            get => (double)GetValue(StateMessageSizeProperty);
            set => SetValue(StateMessageSizeProperty, value);
        }


        // 🖱️ Interaction du crayon
        private void OnPencilClick(object sender, MouseButtonEventArgs e)
        {
            if (IsReadOnly)
            {
                InfoBox.IsEnabled = true;
                IsReadOnly = false;
                InfoBox.Focus();
                InfoBox.CaretIndex = InfoBox.Text.Length;
            }
            EditCommand?.Execute(EditableInfos);
        }
    }
}
