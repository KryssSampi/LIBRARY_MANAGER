using LIBBRARY_MANAGER.Model;
using LIBBRARY_MANAGER.ViewModel.EventViewModels;
using System;
using System.Collections.Generic;
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

namespace LIBBRARY_MANAGER.UI.Modules.EventCatalog
{
    /// <summary>
    /// Interaction logic for EventMainView.xaml
    /// </summary>
    public partial class EventMainView : UserControl
    {
        public EventViewModel ViewModel { get; }

        public EventMainView()
        {
            InitializeComponent();

            ViewModel = new EventViewModel();

            DataContext = ViewModel;

            ViewModel.LoadEventsFromDatabase();
        }
    }
}
