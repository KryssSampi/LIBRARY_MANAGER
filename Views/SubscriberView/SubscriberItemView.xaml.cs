using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LIBBRARY_MANAGER.Model;

namespace LIBBRARY_MANAGER.Views.SubscriberView
{
    public partial class SubscriberItemView : UserControl
    {
        public static readonly DependencyProperty SubscriberProperty =
            DependencyProperty.Register(
                nameof(Subscriber),
                typeof(Subscriber),
                typeof(SubscriberItemView),
                new PropertyMetadata(null, OnSubscriberChanged));

        public Subscriber Subscriber
        {
            get => (Subscriber)GetValue(SubscriberProperty);
            set => SetValue(SubscriberProperty, value);
        }

        public static readonly DependencyProperty ShowDetailCommandProperty =
            DependencyProperty.Register(
                nameof(ShowDetailCommand),
                typeof(ICommand),
                typeof(SubscriberItemView),
                new PropertyMetadata(null));

        public ICommand ShowDetailCommand
        {
            get => (ICommand)GetValue(ShowDetailCommandProperty);
            set => SetValue(ShowDetailCommandProperty, value);
        }

        public SubscriberItemView()
        {
            InitializeComponent();

            System.Diagnostics.Debug.WriteLine("[SubscriberItemView] Control créé");
        }

        private static void OnSubscriberChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (SubscriberItemView)d;
            var subscriber = (Subscriber)e.NewValue;

            if (subscriber != null)
            {
                // ✅ Le DataContext sera géré par le ViewModel
                System.Diagnostics.Debug.WriteLine($"[SubscriberItemView] Subscriber changé: {subscriber.Name_User}");
            }
        }
    }
}