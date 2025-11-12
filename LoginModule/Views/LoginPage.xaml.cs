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
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using LIBBRARY_MANAGER.LoginModule.ViewModel;
using LIBBRARY_MANAGER.UI.Modules.BookCatalog.Helpers;

namespace LIBBRARY_MANAGER.LoginModule.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginPage : UserControl
    {
    
    // Indique si le mot de passe est visible ou non
        private bool isPasswordVisible = false;
        public static readonly DependencyProperty AfterLoginCommandProperty =
    DependencyProperty.Register(
        nameof(ThenLoginCommand),
        typeof(ICommand),
        typeof(LoginPage),
        new PropertyMetadata(null));

        public ICommand ThenLoginCommand
        {
            get { return (ICommand)GetValue(AfterLoginCommandProperty); }
            set { SetValue(AfterLoginCommandProperty, value); }
        }

        public LoginPage()
        {
            InitializeComponent();
            MainFrame.Source = GetPathHelpers.GetPathUri("/LoginModule/Ressources/Videos/media.mp4");
            Titre.Source = GetPathHelpers.GetPathImagesource("/LoginModule/Ressources/Images/85e289383c89c603475d254818c426811c27c768.png");
            Logo.Source = GetPathHelpers.GetPathImagesource("/LoginModule/Ressources/Images/wise-owl-reading-book-vintage-style-learning-icon-vector-57215705.png");
            Livres1.Source = GetPathHelpers.GetPathImagesource("/LoginModule/Ressources/Images/Gemini_Generated_Image_ogly0togly0togly.png");
            Livres2.Source = GetPathHelpers.GetPathImagesource("/LoginModule/Ressources/Images/pngtree-blue-book-image_1325902.jpg");
            EyeIcon.Source = GetPathHelpers.GetPathImagesource("/LoginModule/Ressources/Images/Icons/eye-svgrepo-com.png");
            Loaded += Bind;
        }
        private void BackgroundVideo_MediaEnded(object sender, RoutedEventArgs e)
        {
            var media = sender as MediaElement;
            media.Position = TimeSpan.Zero;
            media.Play();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.1); // Intervalle de 10 millisecondes
            timer.Tick += (sender, args) =>
            {
                if (media.Position >= TimeSpan.FromSeconds(15)) // Arrête à 15 secondes
                {
                    media.Stop();
                    timer.Stop();
                }
            };
            timer.Start();
        }
        private void Bind(object sender, RoutedEventArgs e)
        {
        
            if(this.DataContext is LoginViewModel viewModel)
            {
                viewModel.AfterLoginCommand = ThenLoginCommand;
            }
        }
        // Gère le clic sur le bouton pour afficher/masquer le mot de passe
        private void TogglePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            isPasswordVisible = !isPasswordVisible;

            if (isPasswordVisible)
            {
                // Affiche le mot de passe en tant que texte
                PasswordTextBox.Text = PasswordBox.Password;
                PasswordTextBox.Visibility = Visibility.Visible;
                PasswordBox.Visibility = Visibility.Collapsed;
                EyeIcon.Source = GetPathHelpers.GetPathImagesource("/LoginModule/Ressources/Images/Icons/eye-crossed-svgrepo-com.png");
            }
            else
            {
                // Cache le mot de passe
                PasswordBox.Password = PasswordTextBox.Text;
                PasswordBox.Visibility = Visibility.Visible;
                PasswordTextBox.Visibility = Visibility.Collapsed;
                EyeIcon.Source = GetPathHelpers.GetPathImagesource("/LoginModule/Ressources/Images/Icons/eye-svgrepo-com.png");
            }
        }

        // Met à jour le mot de passe dans le ViewModel lorsque le PasswordBox change
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel viewModel)
            {
                viewModel.Password = PasswordBox.Password;
            }
        }

        // Met à jour le mot de passe dans le ViewModel lorsque le TextBox change
        private void PasswordTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DataContext
                is LoginViewModel viewModel)
                viewModel.Password = PasswordTextBox.Text;
        }
    }
}
