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
using CommunityToolkit.Mvvm.ComponentModel;
using LIBBRARY_MANAGER.Model;
using LIBBRARY_MANAGER.UI.Modules.BookCatalog.Helpers;
using LIBBRARY_MANAGER.UI.Modules.HomeSpace.Items.BasicListItems.ViewModel;
using LIBBRARY_MANAGER.UI.Modules.HomeSpace.ViewModel;

namespace LIBBRARY_MANAGER.UI.Modules.HomeSpace
{
    /// <summary>
    /// Logique d'interaction pour HomeSpace.xaml
    /// </summary>
    public partial class HomeSpace : UserControl
    {
       ObservableCollection<ObservableObject> Operations { get; set; }
        
        public HomeSpace(StaffMember? member = null)
        {
            InitializeComponent();
            if (member != null)
            {
                this.DataContext = new HomeViewModel(member);
            }
            if(Operations is not null)
            Operations.CollectionChanged += Operations_CollectionChanged;

            Image1.Source = GetPathHelpers.GetPathImagesource("/UI/Modules/HomeSpace/Assets/image1.png");
            Image2.Source = GetPathHelpers.GetPathImagesource("/UI/Modules/HomeSpace/Assets/image2.png");
        }

        private void Operations_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (Operations == null)
            {
                return;
            }
            foreach( var item in Operations)
            {
                if( item is  Loan loan)
                {
                   OperationsGrid.Children.Add(GenericItemViewModel.CreateLoanItem(loan));
                }
                if (item is Modification mod)
                {
                    OperationsGrid.Children.Add(GenericItemViewModel.CreateModificationItem(mod));
                }
            }
        }
    }
}
