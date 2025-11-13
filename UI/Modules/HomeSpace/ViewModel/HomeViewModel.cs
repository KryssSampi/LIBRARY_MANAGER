using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;
using LIBBRARY_MANAGER.Model;
using CommunityToolkit.Mvvm.Input;

namespace LIBBRARY_MANAGER.UI.Modules.HomeSpace.ViewModel
{
     public partial class HomeViewModel : ObservableObject
    {

        // 🧩 Commandes observables
        [ObservableProperty]
        private ICommand openLoansCommand;

        [ObservableProperty]
        private ICommand returnCommand;

        [ObservableProperty]
        private ICommand openSubscribersCommand;

        [ObservableProperty]
        private ICommand openCatalogueCommand;

        [ObservableProperty]
        private ICommand borrowCommand;

        private StaffMember currentuser1;

   
        public StaffMember currentuser { get => currentuser1; set => SetProperty(ref currentuser1, value); }

        public HomeViewModel(StaffMember? staffMember = null)
        {
            if (staffMember != null)
            {
                currentuser = staffMember;
            }
        }
        public HomeViewModel() { }  


    }
}
