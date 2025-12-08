using System.Configuration;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LIBBRARY_MANAGER.LoginModule;
using LIBBRARY_MANAGER.UI.Modules.AccountStaffManager;
using LIBBRARY_MANAGER.UI.Modules.BookCatalog;
using LIBBRARY_MANAGER.UI.Modules.BorrowModule;
using LIBBRARY_MANAGER.UI.Modules.BorrowModule.ViewModel;
using LIBBRARY_MANAGER.UI.Modules.HomeSpace;
using LIBBRARY_MANAGER.UI.Modules.HomeSpace.ViewModel;
using LIBBRARY_MANAGER.UI.Modules.LoanCatalog;
using LIBBRARY_MANAGER.UI.Modules.ReturnModule;
using LIBBRARY_MANAGER.UI.Modules.ReturnModule.ViewModel;
using LIBBRARY_MANAGER.UI.Modules.SubscriberCatalog;
using LIBBRARY_MANAGER.UI.Modules.EventCatalog;
using static LIBBRARY_MANAGER.UI.Modules.Navbar.ResponsiveNavBar;

namespace LIBBRARY_MANAGER
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        AccountManager account;
        BookCatalog Catalogue;
        LoanCatalog Gestionnaire_Demprunts;
        SubscriberCatalog Gestionnaire_DAbonnee;
        HomeSpace Homepage;
        BorrowSpace Faire_Emprunt;
        ReturnSpace Faire_Retour;
        EventMainView EventManager;

        private bool IsExtended = false;

        public MainWindow()
        {
            InitializeComponent();
            InitisalizeViews();
            InitializeNavigation();
            InitializeSizer();
            MainContent.Content = Homepage;
        }
        private void InitisalizeViews()
        {
            account = new AccountManager(App._currentconnected_User);
            Homepage = new HomeSpace(App._currentconnected_User);
            Catalogue = new BookCatalog();
            Gestionnaire_Demprunts = new LoanCatalog();
            Gestionnaire_DAbonnee = new SubscriberCatalog();
            EventManager = new EventMainView();

        }

        private void SwitchContent(UserControl control)
        {
            MainContent.Content = control;
        }
        private void InitializeNavigation()
        {
            MainNavbar.AccountManagerCommand = new RelayCommand(_ => SwitchContent(new AccountManager(App._currentconnected_User)));
            MainNavbar.BorrowManagerCommand = new RelayCommand(_ => SwitchContent(Gestionnaire_Demprunts));
            MainNavbar.CatalogCommand = new RelayCommand(_ => SwitchContent(Catalogue));
            MainNavbar.MembersCommand = new RelayCommand(_ => SwitchContent(Gestionnaire_DAbonnee));
            MainNavbar.HomeCommand = new RelayCommand(_ => SwitchContent(Homepage));
            MainNavbar.BorrowCommand = new RelayCommand(_ =>
            {
                Faire_Emprunt = new BorrowSpace();
                if (Faire_Emprunt.DataContext is BorrowSpaceVm BsVm)
                {
                    BsVm.GetHomeCommad = new RelayCommand(_ => MainNavbar.Click_On(NavAction.Home));

                    BsVm.GetLoansCommand = new RelayCommand(_ => MainNavbar.Click_On(NavAction.BorrowManager));

                }
                SwitchContent(Faire_Emprunt);
            });
            MainNavbar.ReturnCommand = new RelayCommand(_ =>
            {
                Faire_Retour = new ReturnSpace();
                if (Faire_Retour.DataContext is ReturnSpaceVm BsVm)
                {
                    BsVm.GetHomeCommad = new RelayCommand(_ => MainNavbar.Click_On(NavAction.Home));


                }
                SwitchContent(Faire_Retour);
            });
            MainNavbar.LogoutCommand = new RelayCommand(_ => LogOut());
            MainNavbar.EventsCommand = new RelayCommand(_ => SwitchContent(EventManager));

            if (Homepage.DataContext is HomeViewModel home)
            {
                home.OpenLoansCommand = new RelayCommand(_ => MainNavbar.Click_On(NavAction.BorrowManager));
                home.OpenCatalogueCommand = new RelayCommand(_ => MainNavbar.Click_On(NavAction.Catalog));
                home.OpenSubscribersCommand = new RelayCommand(_ => MainNavbar.Click_On(NavAction.Members));
                home.BorrowCommand = new RelayCommand(_=> MainNavbar.Click_On(NavAction.Borrow));
                home.ReturnCommand = new RelayCommand(_=> MainNavbar.Click_On(NavAction.Return));
            }

        }
        private void InitializeSizer()
        {
            ReduceBtn.Command = new RelayCommand(_ =>
            {
                this.WindowState = WindowState.Minimized;
            });

            ResizeBtn.Command = new RelayCommand(ResizeWindow);

            CloseBtn.Command = new RelayCommand(LogOut);

        }
        /// <summary>
        /// Déconnexion 
        /// </summary>
        private  void LogOut()
        {
            MessageBoxResult result = MessageBox.Show("Êtes-vous sûr de vouloir quitter ?", "Confirmation", MessageBoxButton.OKCancel, MessageBoxImage.Question);

            if (result == MessageBoxResult.OK)
            {
                System.Diagnostics.Debug.WriteLine("l'utilisateur c'est déconnecter");

                new Login_Page().Show(); 
                this.Close();

            }

        }
             
        private void ResizeWindow()
        {
            if (IsExtended)
            {
                this.Width = 1125;
                this.Height = 600;
             
                IsExtended = false;
                ResizeBtn.IconKind = MahApps.Metro.IconPacks.PackIconMaterialKind.ArrowTopRightBottomLeft;

             
      
            }
            else  
            {
                this.Top = 0;
                this.Left = 0;
                this.Width = SystemParameters.WorkArea.Width;
                this.Height = SystemParameters.WorkArea.Height;
                ResizeBtn.IconKind = MahApps.Metro.IconPacks.PackIconMaterialKind.ArrowCollapse;
                IsExtended = true;

            }
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!IsExtended)
            {
                // Permet de déplacer la fenêtre quand on clique n'importe où dans le contenu
                if (e.ButtonState == MouseButtonState.Pressed)
                    this.DragMove();
            }
        }

    }
}