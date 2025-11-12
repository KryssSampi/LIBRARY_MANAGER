using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace LIBBRARY_MANAGER.UI.Common.Items.MainView
{

    public partial class ColumnHeadersPanel : UserControl
    {
        public static readonly DependencyProperty HeadersProperty =
            DependencyProperty.Register(
                nameof(Headers),
                typeof(ObservableCollection<ColumnHeaderViewModel>),
                typeof(ColumnHeadersPanel),
                new PropertyMetadata(null, OnHeadersChanged));

        public ObservableCollection<ColumnHeaderViewModel> Headers
        {
            get => (ObservableCollection<ColumnHeaderViewModel>)GetValue(HeadersProperty);
            set => SetValue(HeadersProperty, value);
        }

        private static void OnHeadersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColumnHeadersPanel panel)
            {
                if (e.NewValue is ObservableCollection<ColumnHeaderViewModel> newHeaders)
                {
                    // 🔄 écoute les changements internes de la collection
                    newHeaders.CollectionChanged += (s, ev) => panel.InitializeHeaders();
                }

                panel.InitializeHeaders();
            }
        }


        public static readonly DependencyProperty IsListModeProperty =
            DependencyProperty.Register(nameof(IsListMode), typeof(bool), typeof(ColumnHeadersPanel), new PropertyMetadata(true));

        public bool IsListMode
        {
            get => (bool)GetValue(IsListModeProperty);
            set => SetValue(IsListModeProperty, value);
        }

        // Commande de tri, passée à chaque header avec la colonne comme paramètre
        public static readonly DependencyProperty SortCommandProperty =
            DependencyProperty.Register(nameof(SortCommand), typeof(ICommand), typeof(ColumnHeadersPanel));


        public ICommand SortCommand
        {
            get => (ICommand)GetValue(SortCommandProperty);
            set => SetValue(SortCommandProperty, value);
        }

        public ColumnHeadersPanel()
        {
            InitializeComponent();

            if (Headers is not null)
                InitializeHeaders();

        }

        // Gestion du tri pour un seul header actif à la fois
        public void OnHeaderClicked(ColumnHeaderViewModel clickedHeader)
        {
            foreach (var header in Headers)
            {
                if (header == clickedHeader)
                {
                    // Toggle la direction à chaque clic
                    header.IsSorted = true;
                    header.SortAscending = !header.SortAscending;
                }
                else
                {
                    header.IsSorted = false;
                }
            }

            // Relai la commande globale du tri si bindée
            SortCommand?.Execute(clickedHeader.HeaderKey);
        }
        private void InitializeHeaders()
        {
            if (Headers == null || ColumnHeaderGrid_ == null)
                return;

            ColumnHeaderGrid_.Children.Clear();

            int rowIndex = 0;
            foreach (var header in Headers)
            {
                var columnHeader = new ColumnHeader
                {
                    DataContext = header,
                    HeaderName = header.Label,
                    AllowFilter = header.AllowFilter,
                    FilterMenuItems = header.FilterOptions,
                    Foreground = this.Foreground,
                    SortCommand = header.SortCommand,
                    SortBackCommand = header.SortBackCommand,
                    ClickCommand = new RelayCommand<ColumnHeader>((ColumnHeader) =>
                    {
                        ClearAllCheckedFilters();
                        RemoveSorted(ColumnHeader);

                    })
                  
                };

                // Exemple si tu veux placer chaque header dans une row
                Grid.SetRow(columnHeader, rowIndex++);
                ColumnHeaderGrid_.Children.Add(columnHeader);
            }
        }


        private void RemoveSorted(ColumnHeader Execept)
        {

            if(Headers != null)
            {
                foreach(ColumnHeader header  in ColumnHeaderGrid_.Children)
                {
                    if(header != Execept )
                    header.IsSorted = false;
                }
            }
        }

        public void ClearAllCheckedFilters()
        {
            if (Headers == null)
                return;

            foreach (var header in Headers)
            {
                if (header.FilterOptions == null)
                    continue;

                foreach (var option in header.FilterOptions)
                {
                    option.IsChecked = false;
                }
            }
        }

  }
    // ViewModel d’un colonne
    public class ColumnHeaderViewModel : ObservableObject
        {
            public string HeaderKey { get; }
            public string Label { get; }
            public bool AllowFilter { get; set; }
            public ObservableCollection<FilterMenuItem> FilterOptions { get; set; } = new();
        public string SortKey { get; set; }
        public bool IsSorted { get; set; }
            public bool SortAscending { get; set; }

        public ICommand SortCommand { get; set; }

        public ICommand SortBackCommand { get; set; }

        public ColumnHeaderViewModel(string label, bool allowFilter = false )
            {
                Label = label;
                AllowFilter = allowFilter;
            }
        }


    }
