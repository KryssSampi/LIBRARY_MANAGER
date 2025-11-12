using System.Windows;
using System.Windows.Controls;
using LIBBRARY_MANAGER.Model;
using LIBBRARY_MANAGER.UI.Modules.AccountStaffManager.ViewModel;

namespace LIBBRARY_MANAGER.UI.Modules.AccountStaffManager
{
    public partial class AccountManager : UserControl
    {
        private AccountManagerViewModel ViewModel => DataContext as AccountManagerViewModel;


        // Propriété pour le chemin du dossier d'avatars
        public string AvatarFolderPath { get; set; }

        public AccountManager(StaffMember? staffMember = null)
        {

            InitializeComponent();
            DataContext = new AccountManagerViewModel(staffMember);
            // Définir le chemin des avatars
            AvatarFolderPath = System.IO.Path.Combine(
                AppContext.BaseDirectory, "..", "..", "..",
                "UI", "Modules", "AccountStaffManager", "Assets"
            );

            // Configurer l'AvatarEditor
            try
            {
                AvatarEditor.AvatarFolderPath = AvatarFolderPath;
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors du chargement des avatars: {ex.Message}");
            }
        }

        private void NewPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null && sender is PasswordBox passwordBox)
            {
                ViewModel.NewPassword = passwordBox.Password;
            }
        }
    }
}