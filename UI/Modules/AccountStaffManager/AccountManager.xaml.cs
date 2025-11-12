using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using LIBBRARY_MANAGER.Model;
using LIBBRARY_MANAGER.UI.Modules.AccountStaffManager.Items;
using LIBBRARY_MANAGER.UI.Modules.AccountStaffManager.ViewModel;

namespace LIBBRARY_MANAGER.UI.Modules.AccountStaffManager
{
    public partial class AccountManager : UserControl
    {
        private AccountManagerViewModel ViewModel => DataContext as AccountManagerViewModel;


        // Propriété pour le chemin du dossier d'avatars
        public string AvatarFolderPath { get; set; }

        public static readonly DependencyProperty PasswordShowedProperty =
            DependencyProperty.Register("PasswordShowed", typeof(string), typeof(AccountManager),
                new PropertyMetadata(string.Empty));

        public string PasswordShowed
        {
            get { return (string)GetValue(PasswordShowedProperty); }
            set { SetValue(PasswordShowedProperty, value); }
        }

        public static readonly DependencyProperty PasswordIsShowedProperty =
            DependencyProperty.Register("PasswordIsShowed", typeof(bool), typeof(AccountManager),
                new PropertyMetadata(false));

        public bool PasswordIsShowed
        {
            get { return (bool)GetValue(PasswordIsShowedProperty); }
            set { SetValue(PasswordIsShowedProperty, value); NewPasswordBox.Password = PasswordShowed; }
        }
        public AccountManager(StaffMember? staffMember = null)
        {

            InitializeComponent();
            DataContext = new AccountManagerViewModel(staffMember);
            // Définir le chemin des avatars
            AvatarFolderPath = System.IO.Path.Combine(
                AppContext.BaseDirectory, "..", "..", "..",
                "UI", "Modules", "AccountStaffManager", "Assets"
            );
            ViewModel.ResetStateCommand = new RelayCommand(ResetState);

            // Configurer l'AvatarEditor
            try
            {
                AvatarEditor.AvatarFolderPath = AvatarFolderPath;
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors du chargement des avatars: {ex.Message}");
            }
            Showpswd.Command = new RelayCommand(ShowPswd_);
                
        }

        private void PswdShowed_TextChanged(object sender, TextChangedEventArgs e)
        {
     
        }

        private void ShowPswd_()
        {
          
            if(!PasswordIsShowed)
            PasswordShowed = NewPasswordBox.Password.ToString();

            if(PasswordIsShowed)
            NewPasswordBox.Password = PasswordShowed;

            PasswordIsShowed = !PasswordIsShowed;
        }

        public void ResetState()
        {
            
            foreach(EditerLabel label in GetAllEditerLabels(FormulaireModif as Grid))
            {
                label.InfoBox.IsEnabled  = false;
            }
        }
        private void NewPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null && sender is PasswordBox passwordBox)
            {
                ViewModel.NewPassword = passwordBox.Password;
               
            }
        }
        /// <summary>
        /// Méthode récursive qui parcourt l'arbre visuel pour trouver tous les EditerLabel.
        /// </summary>
        public static List<EditerLabel> GetAllEditerLabels(DependencyObject parent)
        {
            var result = new List<EditerLabel>();
            GetAllEditerLabelsRecursiveInternal(parent, result);
            return result;
        }

        private static void GetAllEditerLabelsRecursiveInternal(DependencyObject parent, List<EditerLabel> list)
        {
            if (parent == null) return;
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is EditerLabel editerLabel)
                    list.Add(editerLabel);
                GetAllEditerLabelsRecursiveInternal(child, list);
            }
        }
    }
}