using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace LIBBRARY_MANAGER.UI.Modules.BookCatalog.Core.Services
{
        /// <summary>
        /// Service de navigation réutilisable avec animations fluides
        /// </summary>
        public class NavigationService
        {
            private ContentControl? _navigationFrame;
            private UserControl? _currentView;

            public UserControl? CurrentView => _currentView;

            /// <summary>
            /// Initialise le service avec le ContentControl cible
            /// </summary>
            public void Initialize(ContentControl frame)
            {
                _navigationFrame = frame ?? throw new ArgumentNullException(nameof(frame));
            }

            /// <summary>
            /// Navigation simple sans animation
            /// </summary>
            public void NavigateTo(UserControl targetView)
            {
                if (_navigationFrame == null)
                    throw new InvalidOperationException("NavigationService non initialisé");

                if (targetView == null || targetView == _currentView)
                    return;

                _navigationFrame.Content = targetView;
                _currentView = targetView;
            }

            /// <summary>
            /// Navigation avec animation de fondu
            /// </summary>
            public void NavigateToWithFade(UserControl targetView,
                int fadeOutDuration = 200,
                int fadeInDuration = 250)
            {
                if (_navigationFrame == null)
                    throw new InvalidOperationException("NavigationService non initialisé");

                if (targetView == null || targetView == _currentView)
                    return;

                if (_currentView != null)
                {
                    // Fade out de la vue actuelle
                    var fadeOut = new DoubleAnimation
                    {
                        From = 1.0,
                        To = 0.0,
                        Duration = TimeSpan.FromMilliseconds(fadeOutDuration),
                        EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
                    };

                    fadeOut.Completed += (s, e) =>
                    {
                        // Changement de vue
                        _navigationFrame.Content = targetView;
                        _currentView = targetView;

                        // Fade in de la nouvelle vue
                        var fadeIn = new DoubleAnimation
                        {
                            From = 0.0,
                            To = 1.0,
                            Duration = TimeSpan.FromMilliseconds(fadeInDuration),
                            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                        };

                        _navigationFrame.BeginAnimation(UIElement.OpacityProperty, fadeIn);
                    };

                    _navigationFrame.BeginAnimation(UIElement.OpacityProperty, fadeOut);
                }
                else
                {
                    // Première navigation
                    _navigationFrame.Content = targetView;
                    _currentView = targetView;

                    var fadeIn = new DoubleAnimation
                    {
                        From = 0.0,
                        To = 1.0,
                        Duration = TimeSpan.FromMilliseconds(fadeInDuration),
                        EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                    };

                    _navigationFrame.BeginAnimation(UIElement.OpacityProperty, fadeIn);
                }
            }

            /// <summary>
            /// Navigation avec animation de slide
            /// </summary>
            public void NavigateToWithSlide(UserControl targetView,
                SlideDirection direction = SlideDirection.Left,
                int duration = 300)
            {
                if (_navigationFrame == null)
                    throw new InvalidOperationException("NavigationService non initialisé");

                if (targetView == null || targetView == _currentView)
                    return;

                // TODO: Implémenter slide animation
                // Pour l'instant, fallback sur fade
                NavigateToWithFade(targetView);
            }

            /// <summary>
            /// Retour en arrière
            /// </summary>
            public bool CanGoBack { get; private set; }

            public void GoBack()
            {
                // TODO: Implémenter historique de navigation
                throw new NotImplementedException("Historique de navigation à implémenter");
            }

            /// <summary>
            /// Efface l'historique
            /// </summary>
            public void ClearHistory()
            {
                _currentView = null;
            }
        }

        public enum SlideDirection
        {
            Left,
            Right,
            Up,
            Down
        }
    }
