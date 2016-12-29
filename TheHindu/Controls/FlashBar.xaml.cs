using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace TheHindu.Controls
{
    public partial class FlashBar : UserControl
    {
        private Storyboard _fadeInSb = new Storyboard();
        private Storyboard _fadeOutSb = new Storyboard();

        private bool _isReallyAnimating;
        private bool _hasBeenToggled;

        public bool DoNotShowOnFirstToggleOfIsAnimating { get; set; }

        public bool IsAnimating
        {
            get { return (bool)GetValue(IsAnimatingProperty); }
            set { SetValue(IsAnimatingProperty, value); }
        }

        public static readonly DependencyProperty IsAnimatingProperty =
            DependencyProperty.Register("IsAnimating", typeof(bool), typeof(FlashBar), new PropertyMetadata(false, IsAnimatingChanged));

        private static void IsAnimatingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var fb = d as FlashBar;
            if (true.Equals(e.NewValue))
                fb.Show();
        }

        public FlashBar()
        {
            InitializeComponent();
            InitStoryBoard();
        }

        private void InitStoryBoard()
        {
            SetupSb(0.5D, 1D, TimeSpan.FromMilliseconds(200D), _fadeInSb);
            SetupSb(1D, 0, TimeSpan.FromMilliseconds(750D), _fadeOutSb);

            _fadeInSb.Completed += (s, e) => { _fadeOutSb.Begin(); };
            _fadeOutSb.Completed += (s, e) =>
            {
                TheRect.Visibility = Visibility.Collapsed;
                _isReallyAnimating = false;
            };
        }

        private void SetupSb(double from, double to, TimeSpan duration, Storyboard sb)
        {
            var animation = new DoubleAnimation { To = to, From = from, Duration = duration };
            sb.Children.Add(animation);
            Storyboard.SetTarget(animation, TheRect);
            Storyboard.SetTargetProperty(animation, new PropertyPath(FrameworkElement.OpacityProperty));
        }

        public void Show()
        {
            if (_isReallyAnimating) return;

            if (!_hasBeenToggled)
            {
                _hasBeenToggled = true;
                if (DoNotShowOnFirstToggleOfIsAnimating) return;
            }

            TheRect.Visibility = Visibility.Visible;
            _isReallyAnimating = true;
            _fadeInSb.Begin();
        }
    }
}