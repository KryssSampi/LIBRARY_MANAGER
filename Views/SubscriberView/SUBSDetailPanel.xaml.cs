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

namespace LIBBRARY_MANAGER.Views.SubscriberView
{
        public partial class SUBSDetailPanel : UserControl
        {
            // ----- Propriétés bindables -----
            public string Name_User
            {
                get => (string)GetValue(Name_UserProperty);
                set => SetValue(Name_UserProperty, value);
            }
            public static readonly DependencyProperty Name_UserProperty =
                DependencyProperty.Register(nameof(Name_User), typeof(string), typeof(SUBSDetailPanel), new PropertyMetadata(string.Empty));

            public string Ref_Subscriber
            {
                get => (string)GetValue(Ref_SubscriberProperty);
                set => SetValue(Ref_SubscriberProperty, value);
            }
            public static readonly DependencyProperty Ref_SubscriberProperty =
                DependencyProperty.Register(nameof(Ref_Subscriber), typeof(string), typeof(SUBSDetailPanel), new PropertyMetadata(string.Empty));

            public string Adresse_Mail
            {
                get => (string)GetValue(Adresse_MailProperty);
                set => SetValue(Adresse_MailProperty, value);
            }
            public static readonly DependencyProperty Adresse_MailProperty =
                DependencyProperty.Register(nameof(Adresse_Mail), typeof(string), typeof(SUBSDetailPanel), new PropertyMetadata(string.Empty));

            public string Num_Telephone
            {
                get => (string)GetValue(Num_TelephoneProperty);
                set => SetValue(Num_TelephoneProperty, value);
            }
            public static readonly DependencyProperty Num_TelephoneProperty =
                DependencyProperty.Register(nameof(Num_Telephone), typeof(string), typeof(SUBSDetailPanel), new PropertyMetadata(string.Empty));

            public string Adresse
            {
                get => (string)GetValue(AdresseProperty);
                set => SetValue(AdresseProperty, value);
            }
            public static readonly DependencyProperty AdresseProperty =
                DependencyProperty.Register(nameof(Adresse), typeof(string), typeof(SUBSDetailPanel), new PropertyMetadata(string.Empty));

            public DateTime BirthDate
            {
                get => (DateTime)GetValue(BirthDateProperty);
                set => SetValue(BirthDateProperty, value);
            }
            public static readonly DependencyProperty BirthDateProperty =
                DependencyProperty.Register(nameof(BirthDate), typeof(DateTime), typeof(SUBSDetailPanel), new PropertyMetadata(DateTime.MinValue));

            public int Age
            {
                get => (int)GetValue(AgeProperty);
                set => SetValue(AgeProperty, value);
            }
            public static readonly DependencyProperty AgeProperty =
                DependencyProperty.Register(nameof(Age), typeof(int), typeof(SUBSDetailPanel), new PropertyMetadata(0));

            // ---- "Fidelity" ----
            public double Fidelity
            {
                get => (double)GetValue(FidelityProperty);
                set => SetValue(FidelityProperty, value);
            }
            public static readonly DependencyProperty FidelityProperty =
                DependencyProperty.Register(nameof(Fidelity), typeof(double), typeof(SUBSDetailPanel), new PropertyMetadata(0.0));

            // ----- Emprunts actifs -----
            public int ActiveLoansCount
            {
                get => (int)GetValue(ActiveLoansCountProperty);
                set => SetValue(ActiveLoansCountProperty, value);
            }
            public static readonly DependencyProperty ActiveLoansCountProperty =
                DependencyProperty.Register(nameof(ActiveLoansCount), typeof(int), typeof(SUBSDetailPanel), new PropertyMetadata(0));

            public Brush ActiveLoansColor
            {
                get => (Brush)GetValue(ActiveLoansColorProperty);
                set => SetValue(ActiveLoansColorProperty, value);
            }
            public static readonly DependencyProperty ActiveLoansColorProperty =
                DependencyProperty.Register(nameof(ActiveLoansColor), typeof(Brush), typeof(SUBSDetailPanel), new PropertyMetadata(Brushes.Gray));

            // ---- Commandes ----
            public ICommand ViewLoansCommand
            {
                get => (ICommand)GetValue(ViewLoansCommandProperty);
                set => SetValue(ViewLoansCommandProperty, value);
            }
            public static readonly DependencyProperty ViewLoansCommandProperty =
                DependencyProperty.Register(nameof(ViewLoansCommand), typeof(ICommand), typeof(SUBSDetailPanel), new PropertyMetadata(null));

            public ICommand CloseCommand
            {
                get => (ICommand)GetValue(CloseCommandProperty);
                set => SetValue(CloseCommandProperty, value);
            }
            public static readonly DependencyProperty CloseCommandProperty =
                DependencyProperty.Register(nameof(CloseCommand), typeof(ICommand), typeof(SUBSDetailPanel), new PropertyMetadata(null));

            public SUBSDetailPanel()
            {
                InitializeComponent();
                // Optionnel : attach events for buttons if needed directly in code-behind
                CloseBtn.MouseLeftButtonDown += (s, e) => OnCloseRequested();
            }

            // Event for Footer close button
            private void CloseButton_Click(object sender, RoutedEventArgs e)
            {
                OnCloseRequested();
            }

            // Permet de tout fermer via l'event, et de brancher la commande CloseCommand (si bindée)
            private void OnCloseRequested()
            {
                if (CloseCommand != null && CloseCommand.CanExecute(null))
                {
                    CloseCommand.Execute(null);
                }
            }

            // Accès aux éléments visuels clés
            public Button FooterCloseButton => FindName("FooterCloseBtn") as Button;
            public UI.Common.Items.IconButton HeaderCloseButton => CloseBtn;
            // Et ainsi de suite si tu veux exposer d'autres éléments critiques.
        }
    }
