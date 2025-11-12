using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace LIBBRARY_MANAGER.UI.Common.Items.MainView
{
    public partial class ColumnHeader : UserControl, INotifyPropertyChanged
    {
        // ──────────────────────────────
        // 🟦 Dépendances existantes
        // ──────────────────────────────
        public static readonly DependencyProperty HeaderNameProperty =
            DependencyProperty.Register(nameof(HeaderName), typeof(string), typeof(ColumnHeader));

        public string HeaderName
        {
            get => (string)GetValue(HeaderNameProperty);
            set => SetValue(HeaderNameProperty, value);
        }

        public static readonly DependencyProperty AllowFilterProperty =
            DependencyProperty.Register(nameof(AllowFilter), typeof(bool), typeof(ColumnHeader), new PropertyMetadata(false));

        public bool AllowFilter
        {
            get => (bool)GetValue(AllowFilterProperty);
            set => SetValue(AllowFilterProperty, value);
        }

        public static readonly DependencyProperty IsSortedProperty =
            DependencyProperty.Register(nameof(IsSorted), typeof(bool), typeof(ColumnHeader), new PropertyMetadata(false));

        public bool IsSorted
        {
            get => (bool)GetValue(IsSortedProperty);
            set { SetValue(IsSortedProperty, value); OnPropertyChanged(nameof(SortGlyph)); }
        }

        public static readonly DependencyProperty SortDirectionProperty =
            DependencyProperty.Register(nameof(SortDirection), typeof(bool), typeof(ColumnHeader));

        public bool SortDirection
        {
            get => (bool)GetValue(SortDirectionProperty);
            set { SetValue(SortDirectionProperty, value); OnPropertyChanged(nameof(SortGlyph)); }
        }

        // ──────────────────────────────
        // 🟢 Nouvelles Commandes
        // ──────────────────────────────
        public static readonly DependencyProperty SortCommandProperty =
            DependencyProperty.Register(nameof(SortCommand), typeof(ICommand), typeof(ColumnHeader));

        public ICommand SortCommand
        {
            get => (ICommand)GetValue(SortCommandProperty);
            set => SetValue(SortCommandProperty, value);
        }


        public static readonly DependencyProperty SortBackCommandProperty =
            DependencyProperty.Register(nameof(SortBackCommand), typeof(ICommand), typeof(ColumnHeader));

        public ICommand SortBackCommand
        {
            get => (ICommand)GetValue(SortBackCommandProperty);
            set => SetValue(SortBackCommandProperty, value);
        }
        public ICommand ClickCommand
        {
            get => (ICommand)GetValue(ClickCommandProperty);
            set => SetValue(ClickCommandProperty, value);
        }

        public static readonly DependencyProperty ClickCommandProperty =
            DependencyProperty.Register(nameof(ClickCommand), typeof(ICommand), typeof(ColumnHeader));




        // ──────────────────────────────
        // 🟨 Filtrage & Apparence
        // ──────────────────────────────
        public static readonly DependencyProperty FilterMenuItemsProperty =
            DependencyProperty.Register(nameof(FilterMenuItems), typeof(ObservableCollection<FilterMenuItem>), typeof(ColumnHeader),
                new PropertyMetadata(new ObservableCollection<FilterMenuItem>()));

        public ObservableCollection<FilterMenuItem> FilterMenuItems
        {
            get => (ObservableCollection<FilterMenuItem>)GetValue(FilterMenuItemsProperty);
            set => SetValue(FilterMenuItemsProperty, value);
        }

        public static new readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register(nameof(Background), typeof(Brush), typeof(ColumnHeader), new PropertyMetadata(Brushes.Transparent));
        public new Brush Background
        {
            get => (Brush)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }

        public static new readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register(nameof(Foreground), typeof(Brush), typeof(ColumnHeader), new PropertyMetadata(Brushes.Black));
        public new Brush Foreground
        {
            get => (Brush)GetValue(ForegroundProperty);
            set => SetValue(ForegroundProperty, value);
        }

        public string SortGlyph => !IsSorted ? "" : (SortDirection ? "▲" : "▼");

        // ──────────────────────────────
        // 🧠 Constructeur
        // ──────────────────────────────
        public ColumnHeader()
        {
            InitializeComponent();
            MouseLeftButtonUp += OnHeaderClicked;
        }

        // ──────────────────────────────
        // ⚙️ Logique de clic
        // ──────────────────────────────
        private void OnHeaderClicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ClickCommand.Execute(this);
            if (!SortDirection)
            {
                // 1er clic : tri ascendant
                IsSorted = true;
                SortDirection = true;
                SortCommand?.Execute(HeaderName);
            }
            else
            {
                // Alternance
                SortDirection = !SortDirection;
                if (SortDirection)
                    SortBackCommand?.Execute(HeaderName);
                else
                    SortCommand?.Execute(HeaderName);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    // ──────────────────────────────
    // 🧩 Classe de filtre inchangée
    // ──────────────────────────────
    public class FilterMenuItem : INotifyPropertyChanged
    {
        private bool _isChecked;
        private string _label = "";
        private string _value = "";

        public string Label
        {
            get => _label;
            set { _label = value; OnPropertyChanged(nameof(Label)); }
        }

        public bool IsChecked
        {
            get => _isChecked;
            set { _isChecked = value; OnPropertyChanged(nameof(IsChecked)); }
        }

        public string? Value
        {
            get => _value;
            set { _value = value; OnPropertyChanged(nameof(Value)); }
        }

        public ICommand? Command { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
