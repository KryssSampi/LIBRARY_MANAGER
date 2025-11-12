using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using LIBBRARY_MANAGER.UI.Common.Items;

namespace LIBBRARY_MANAGER.UI.Common.Items.MainView
{
    public partial class ActiveFiltersPanel : UserControl
    {
        // ========== PROPRIÉTÉS DÉPENDANTES ==========

        public ObservableCollection<FilterLabelViewModel> Filters
        {
            get => (ObservableCollection<FilterLabelViewModel>)GetValue(FiltersProperty);
            set => SetValue(FiltersProperty, value);
        }
        public static readonly DependencyProperty FiltersProperty =
            DependencyProperty.Register(nameof(Filters), typeof(ObservableCollection<FilterLabelViewModel>),
                typeof(ActiveFiltersPanel),
                new PropertyMetadata(new ObservableCollection<FilterLabelViewModel>(), OnFiltersChanged));

        public ICommand CloseFilterCommand
        {
            get => (ICommand)GetValue(CloseFilterCommandProperty);
            set => SetValue(CloseFilterCommandProperty, value);
        }
        public static readonly DependencyProperty CloseFilterCommandProperty =
            DependencyProperty.Register(nameof(CloseFilterCommand), typeof(ICommand),
                typeof(ActiveFiltersPanel), new PropertyMetadata(null));
        public ICommand ResetFilterCommand
        {
            get => (ICommand)GetValue(ResetFilterProperty);
            set => SetValue(ResetFilterProperty, value);

        }
        public static readonly DependencyProperty ResetFilterProperty =
            DependencyProperty.Register(nameof(ResetFilterCommand), typeof(ICommand), typeof(ActiveFiltersLabel), new PropertyMetadata(null));

        public Brush FilterForeground
        {
            get => (Brush)GetValue(FilterForegroundProperty);
            set => SetValue(FilterForegroundProperty, value);
        }
        public static readonly DependencyProperty FilterForegroundProperty =
            DependencyProperty.Register(nameof(FilterForeground), typeof(Brush),
                typeof(ActiveFiltersPanel), new PropertyMetadata(Brushes.Black));

        public Brush FilterBackground
        {
            get => (Brush)GetValue(FilterBackgroundProperty);
            set => SetValue(FilterBackgroundProperty, value);
        }
        public static readonly DependencyProperty FilterBackgroundProperty =
            DependencyProperty.Register(nameof(FilterBackground), typeof(Brush),
                typeof(ActiveFiltersPanel), new PropertyMetadata(Brushes.White));

        // ✅ Nouvelle propriété : contrôle la visibilité du panneau
        public bool IsPanelVisible
        {
            get => (bool)GetValue(IsPanelVisibleProperty);
            set => SetValue(IsPanelVisibleProperty, value);
        }
        public static readonly DependencyProperty IsPanelVisibleProperty =
            DependencyProperty.Register(nameof(IsPanelVisible), typeof(bool),
                typeof(ActiveFiltersPanel), new PropertyMetadata(true));

        // ========== CONSTRUCTEUR ==========
        public ActiveFiltersPanel()
        {
            InitializeComponent();
           

            Loaded += (s, e) => RefreshFilterLabels();
        }

        // ========== MÉTHODES STATIQUES ==========
        private static void OnFiltersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ActiveFiltersPanel panel)
            {
                if (e.OldValue is ObservableCollection<FilterLabelViewModel> oldCollection)
                    oldCollection.CollectionChanged -= panel.OnFiltersCollectionChanged;

                if (e.NewValue is ObservableCollection<FilterLabelViewModel> newCollection)
                    newCollection.CollectionChanged += panel.OnFiltersCollectionChanged;

                panel.RefreshFilterLabels();
            }
        }


        // ========== ÉVÉNEMENTS ==========
        private void OnFiltersCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RefreshFilterLabels();
        }

        // ========== MISE À JOUR UI ==========
        public void RefreshFilterLabels()
        {
            FiltersGrid.Children.Clear();

            if (Filters == null || Filters.Count == 0)
            {
                IsPanelVisible = false;
                return;
            }

            IsPanelVisible = true;

            foreach (var filter in Filters)
            {
                var label = new ActiveFiltersLabel
                {
                    TypeText = filter.Type,
                    ValueText = filter.Value,
                    FilterId = filter.Id,
                    CloseCommand = CloseFilterCommand,
                    Foreground = FilterForeground,
                    LabelBackground = FilterBackground
                };

                // Gestion interne du clic pour suppression immédiate (optionnelle)
                label.CloseButton.AddHandler(IconButton.ClickEvent, new RoutedEventHandler((sender, e) =>
                {
                    Filters.Remove(filter);
                }));

                FiltersGrid.Children.Add(label);
            }
        }
        private void ResetFilterBtn_Click(object sender , MouseButtonEventArgs e)
        {

            IsPanelVisible = false;
            Filters = [];
        }

    }

    public class FilterLabelViewModel
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public object Id { get; set; }
    }
}
