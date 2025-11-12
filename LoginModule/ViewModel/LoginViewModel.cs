using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using System.Windows.Input;
using System.Security.Cryptography;
using LIBBRARY_MANAGER.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;
using LIBBRARY_MANAGER.LoginModule.Views;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace LIBBRARY_MANAGER.LoginModule.ViewModel
{
    public partial class LoginViewModel : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
    {
        // Creation du contexte de la base de donnees
        private Data.LibraryDbContext _context;

        // Observable properties pour la liaison ou binding avec la vue

        [ObservableProperty]
        public string username;

        [ObservableProperty]
        public string password;


        [ObservableProperty]
        public string errorMessage;

        [ObservableProperty]
        public SolidColorBrush messageColor;


        // Commande pour la connexion
        public ICommand LoginCommand { get; }


        public ICommand AfterLoginCommand { get; set; } 

        // Constructeur
        public LoginViewModel()
        {
            _context = new Data.LibraryDbContext();
            LoginCommand = new RelayCommand(Login);
        }

        // Methode de connexion
        private void Login()
        {

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                MessageColor = new SolidColorBrush(Colors.Red);
                ErrorMessage = "Veuillez entrer un nom d'utilisateur et un mot de passe.";
                return;
            }
            try
            {
                string hashedPassword = User.HashPassword(Password);
                var user = _context.StaffMembers
                    .FirstOrDefault(u => u.Name_User == Username && u.HashedPassword == hashedPassword);

                if (user != null)
                {
                    MessageColor = new SolidColorBrush(Colors.Green);
                    ErrorMessage = "Connexion réussie!";
                    Task.Delay(20000);                  
                    App._currentconnected_User = user;
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    AfterLoginCommand?.Execute(null);
                    // Logique supplémentaire après une connexion réussie peut être ajoutée ici
                    // Ajouter le code vers la vue principale ou autre ici
                }
                else
                    MessageColor = new SolidColorBrush(Colors.Red);
                    ErrorMessage = "Nom d'utilisateur ou mot de passe incorrect.";
            }
            catch (Exception ex)
            {
                ErrorMessage = "Erreur lors de la connexion à la base de données : " + ex;
            }
        }

        // Methode pour hasher le mot de passe
        private static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }
    }
}
