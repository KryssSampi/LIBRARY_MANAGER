using System.Configuration;
using System.Data;
using System.Windows;
using LIBBRARY_MANAGER.Data;
using LIBBRARY_MANAGER.Model;
using Microsoft.EntityFrameworkCore;

namespace LIBBRARY_MANAGER
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static LibraryDbContext LibraryDbContext { get; set; } = new LibraryDbContext();
        public static StaffMember _currentconnected_User { get; set; } = new StaffMember();

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialiser le contexte de base de données
            LibraryDbContext = new LibraryDbContext();

            // S'assurer que la base de données est créée
            try
            {
                // Créer la base de données si elle n'existe pas
                LibraryDbContext.Database.EnsureCreated();

                // Alternative avec migrations (recommandé en production):
                // LibraryDbContext.Database.Migrate();

                Console.WriteLine("✅ Base de données initialisée");

                // Initialiser les données de test si la base est vide
                await TestDataInitializer.InitializeTestDataAsync(LibraryDbContext);

                // S'assurer qu'un utilisateur de test existe
                EnsureTestUser();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erreur lors de l'initialisation de la base de données:\n\n{ex.Message}",
                    "Erreur de démarrage",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void EnsureTestUser()
        {
            try
            {
                var staff = StaffMember.EnsureTestStaff(LibraryDbContext);

                if (staff != null)
                {
                    _currentconnected_User = staff;
                    Console.WriteLine($"✅ Utilisateur de test connecté: {staff.Name_User} ({staff.Ref_Staff})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠ Erreur lors de la création de l'utilisateur de test: {ex.Message}");
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Nettoyer les ressources
            LibraryDbContext?.Dispose();
            base.OnExit(e);
        }
    }
}