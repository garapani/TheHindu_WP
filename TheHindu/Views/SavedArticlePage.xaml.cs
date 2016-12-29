using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;
using TheHindu.ViewModel;

namespace TheHindu.Views
{
    public partial class SavedArticlePage
    {
        #region Fields

        private readonly SavedArticleViewModel _savedArticleViewModel;

        #endregion Fields

        #region Constructor

        public SavedArticlePage()
        {
            InitializeComponent();
            _savedArticleViewModel = DataContext as SavedArticleViewModel;
            Messenger.Default.Register<SavedArticlePropertyChangedMessage>(this, action => DispatcherHelper.CheckBeginInvokeOnUI(UpdateStory));
            Messenger.Default.Register<SavedArticlePropertyIntializedMessage>(this, action => DispatcherHelper.CheckBeginInvokeOnUI(InitializePage));
        }

        #endregion Constructor

        #region PhoneApplicationPage Overrides

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ApplicationBar.IsVisible = true;
            UpdateFontStyleButtons();
            UpdateStory();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ApplicationBar.IsVisible = false;
            if (_savedArticleViewModel != null)
            {
                _savedArticleViewModel.ClearArticle();
            }
            base.OnNavigatedFrom(e);
        }

        #endregion PhoneApplicationPage Overrides

        #region Private Methods

        private void InitializePage()
        {
            ApplicationBar.IsVisible = false;
            Progress.Visibility = System.Windows.Visibility.Visible;
            ContentPanel.Visibility = System.Windows.Visibility.Collapsed;
        }

        #endregion Private Methods

        private void GestureListener_Flick(object sender, Microsoft.Phone.Controls.FlickGestureEventArgs e)
        {
            try
            {
                if (e != null && _savedArticleViewModel != null)
                {
                    if (e.HorizontalVelocity < 0)
                    {
                        if (_savedArticleViewModel.NextArticleCommand.CanExecute(null))
                            _savedArticleViewModel.NextArticleCommand.Execute(null);
                    }
                    if (e.HorizontalVelocity > 0)
                    {
                        if (_savedArticleViewModel.PreviousArticleCommand.CanExecute(null))
                            _savedArticleViewModel.PreviousArticleCommand.Execute(null);
                    }
                }
            }
            catch (Exception)
            { }
        }

        private void theBrowser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            //webBrowser.Visibility = System.Windows.Visibility.Visible;
            Articlepage.Visibility = Visibility.Visible;
        }

        private void theBrowser_NavigationFailed(object sender, System.Windows.Navigation.NavigationFailedEventArgs e)
        {
            MessageBox.Show("There was an error loading the page.");
        }

        private void theBrowser_ScriptNotify(object sender, Microsoft.Phone.Controls.NotifyEventArgs e)
        {
        }

        private void btnSmallFont_Click(object sender, RoutedEventArgs e)
        {
            SettingsClass settings = App.Current.Resources["Settings"] as SettingsClass;
            if (settings != null)
            {
                settings.FontSize = 18.667;
            }
            UpdateStory();
            BtnSmallFont.Background = new SolidColorBrush(Colors.LightGray);
            BtnNormalFont.Background = new SolidColorBrush(Colors.Transparent);
            BtnMediumFont.Background = new SolidColorBrush(Colors.Transparent);
            BtnMediumLargeFont.Background = new SolidColorBrush(Colors.Transparent);
            BtnLargeFont.Background = new SolidColorBrush(Colors.Transparent);
        }

        private void btnNormalFont_Click(object sender, RoutedEventArgs e)
        {
            SettingsClass settings = App.Current.Resources["Settings"] as SettingsClass;
            if (settings != null)
            {
                settings.FontSize = 20;
            }
            UpdateStory();
            BtnSmallFont.Background = new SolidColorBrush(Colors.Transparent);
            BtnNormalFont.Background = new SolidColorBrush(Colors.LightGray);
            BtnMediumFont.Background = new SolidColorBrush(Colors.Transparent);
            BtnMediumLargeFont.Background = new SolidColorBrush(Colors.Transparent);
            BtnLargeFont.Background = new SolidColorBrush(Colors.Transparent);
        }

        private void btnMediumFont_Click(object sender, RoutedEventArgs e)
        {
            SettingsClass settings = App.Current.Resources["Settings"] as SettingsClass;
            if (settings != null)
            {
                settings.FontSize = 22.667;
            }
            UpdateStory();
            BtnSmallFont.Background = new SolidColorBrush(Colors.Transparent);
            BtnNormalFont.Background = new SolidColorBrush(Colors.Transparent);
            BtnMediumFont.Background = new SolidColorBrush(Colors.LightGray);
            BtnMediumLargeFont.Background = new SolidColorBrush(Colors.Transparent);
            BtnLargeFont.Background = new SolidColorBrush(Colors.Transparent);
        }

        private void btnMediumLargeFont_Click(object sender, RoutedEventArgs e)
        {
            SettingsClass settings = App.Current.Resources["Settings"] as SettingsClass;
            if (settings != null)
            {
                settings.FontSize = 25.333;
            }
            UpdateStory();
            BtnSmallFont.Background = new SolidColorBrush(Colors.Transparent);
            BtnNormalFont.Background = new SolidColorBrush(Colors.Transparent);
            BtnMediumFont.Background = new SolidColorBrush(Colors.Transparent);
            BtnMediumLargeFont.Background = new SolidColorBrush(Colors.LightGray);
            BtnLargeFont.Background = new SolidColorBrush(Colors.Transparent);
        }

        private void btnLargeFont_Click(object sender, RoutedEventArgs e)
        {
            SettingsClass settings = App.Current.Resources["Settings"] as SettingsClass;
            if (settings != null)
            {
                settings.FontSize = 32;
            }
            UpdateStory();
            BtnSmallFont.Background = new SolidColorBrush(Colors.Transparent);
            BtnNormalFont.Background = new SolidColorBrush(Colors.Transparent);
            BtnMediumFont.Background = new SolidColorBrush(Colors.Transparent);
            BtnMediumLargeFont.Background = new SolidColorBrush(Colors.Transparent);
            BtnLargeFont.Background = new SolidColorBrush(Colors.LightGray);
        }

        private void btnSegoeWPFont_Click(object sender, RoutedEventArgs e)
        {
            SettingsClass settings = App.Current.Resources["Settings"] as SettingsClass;
            settings.FontFamily = "Segoe WP";
            BtnSegoeWpFont.Background = new SolidColorBrush(Colors.LightGray);
            BtnCalibriFont.Background = new SolidColorBrush(Colors.Transparent);
            BtnGeorgia.Background = new SolidColorBrush(Colors.Transparent);
            UpdateStory();
        }

        private void btnCalibriFont_Click(object sender, RoutedEventArgs e)
        {
            SettingsClass settings = App.Current.Resources["Settings"] as SettingsClass;
            if (settings != null)
            {
                settings.FontFamily = "Calibri";
            }
            BtnSegoeWpFont.Background = new SolidColorBrush(Colors.Transparent);
            BtnCalibriFont.Background = new SolidColorBrush(Colors.LightGray);
            BtnGeorgia.Background = new SolidColorBrush(Colors.Transparent);
            UpdateStory();
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            FontStyleSelection.Visibility = System.Windows.Visibility.Visible;
            ApplicationBar.IsVisible = false;
        }

        private void ContentPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            FontStyleSelection.Visibility = System.Windows.Visibility.Collapsed;
            ApplicationBar.IsVisible = true;
        }

        private void btnGeorgia_Click(object sender, RoutedEventArgs e)
        {
            SettingsClass settings = App.Current.Resources["Settings"] as SettingsClass;
            if (settings != null)
            {
                settings.FontFamily = "Georgia";
            }
            BtnSegoeWpFont.Background = new SolidColorBrush(Colors.Transparent);
            BtnCalibriFont.Background = new SolidColorBrush(Colors.Transparent);
            BtnGeorgia.Background = new SolidColorBrush(Colors.LightGray);
            UpdateStory();
        }

        private void UpdateStory()
        {
            //synth.CancelAll();
            ContentPanel.Visibility = System.Windows.Visibility.Visible;
            ScrollViewer.ScrollToVerticalOffset(0);
            ScrollViewer.ScrollToHorizontalOffset(0);
            SettingsClass settings = App.Current.Resources["Settings"] as SettingsClass;
            Story.Html = "";

            if (settings.FontFamily == "Georgia")
            {
                HeadLine.FontFamily = new FontFamily("/Assets/Fonts/Roboto.ttf#Roboto");
                Story.FontFamily = new FontFamily("/Assets/Fonts/Roboto.ttf#Roboto");
                PhotoCaption.FontFamily = new FontFamily("/Assets/Fonts/Roboto.ttf#Roboto");
            }
            else if (settings.FontFamily.ToLower() == "calibri")
            {
                HeadLine.FontFamily = new FontFamily("/Assets/Fonts/ArbutusSlab-regular.ttf#Arbutus Slab");
                Story.FontFamily = new FontFamily("/Assets/Fonts/ArbutusSlab-regular.ttf#Arbutus Slab");
                PhotoCaption.FontFamily = new FontFamily("/Assets/Fonts/ArbutusSlab-regular.ttf#Arbutus Slab");
            }
            else
            {
                HeadLine.FontFamily = new FontFamily(settings.FontFamily);
                Story.FontFamily = new FontFamily(settings.FontFamily);
                PhotoCaption.FontFamily = new FontFamily(settings.FontFamily);
            }

            Story.FontSize = settings.FontSize;
            if (settings.SelectedForeground == "White")
            {
                Story.Foreground = new SolidColorBrush(Colors.White);
            }
            else
            {
                Story.Foreground = new SolidColorBrush(Colors.Black);
            }
            if (_savedArticleViewModel != null && _savedArticleViewModel.Article != null)
            {
                Story.Html = _savedArticleViewModel.Article.FullDescription;
            }
            Progress.Visibility = System.Windows.Visibility.Collapsed;
            ApplicationBar.IsVisible = true;
            FontStyleSelection.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void UpdateFontStyleButtons()
        {
            BtnSmallFont.Background = new SolidColorBrush(Colors.Transparent);
            BtnNormalFont.Background = new SolidColorBrush(Colors.Transparent);
            BtnMediumFont.Background = new SolidColorBrush(Colors.Transparent);
            BtnMediumLargeFont.Background = new SolidColorBrush(Colors.Transparent);
            BtnLargeFont.Background = new SolidColorBrush(Colors.Transparent);

            SettingsClass settings = App.Current.Resources["Settings"] as SettingsClass;
            if (settings != null)
            {
                if (settings.FontSize == 18.667)
                {
                    BtnSmallFont.Background = new SolidColorBrush(Colors.LightGray);
                }
                else if (settings.FontSize == 20)
                {
                    BtnNormalFont.Background = new SolidColorBrush(Colors.LightGray);
                }
                else if (settings.FontSize == 22.667)
                {
                    BtnMediumFont.Background = new SolidColorBrush(Colors.LightGray);
                }
                else if (settings.FontSize == 25.333)
                {
                    BtnMediumLargeFont.Background = new SolidColorBrush(Colors.LightGray);
                }
                else if (settings.FontSize == 32)
                {
                    BtnLargeFont.Background = new SolidColorBrush(Colors.LightGray);
                }
                else
                {
                    settings.FontSize = 22.667;
                    BtnMediumFont.Background = new SolidColorBrush(Colors.LightGray);
                }
            }
            BtnCalibriFont.Background = new SolidColorBrush(Colors.Transparent);
            BtnSegoeWpFont.Background = new SolidColorBrush(Colors.Transparent);
            BtnGeorgia.Background = new SolidColorBrush(Colors.Transparent);

            BtnCalibriFont.FontFamily = new System.Windows.Media.FontFamily("Calibri");
            BtnSegoeWpFont.FontFamily = new FontFamily("Segoe WP");
            BtnGeorgia.FontFamily = new FontFamily("/Assets/fonts/Georgia.ttf");
            if (settings != null)
            {
                if (settings.FontFamily == "Segoe WP")
                {
                    BtnSegoeWpFont.Background = new SolidColorBrush(Colors.LightGray);
                }
                else if (settings.FontFamily == "Georgia")
                {
                    BtnGeorgia.Background = new SolidColorBrush(Colors.LightGray);
                }
                else
                {
                    BtnCalibriFont.Background = new SolidColorBrush(Colors.LightGray);
                }
            }
            BtnBlack.Background = new SolidColorBrush(Colors.Transparent);
            BtnWhite.Background = new SolidColorBrush(Colors.Transparent);
            if (settings != null)
            {
                if (settings.SelectedBackground == "White")
                {
                    BtnWhite.Background = new SolidColorBrush(Colors.LightGray);
                }
                else
                {
                    BtnBlack.Background = new SolidColorBrush(Colors.LightGray);
                }
            }
        }

        private void btnBlack_Click(object sender, RoutedEventArgs e)
        {
            BtnWhite.Background = new SolidColorBrush(Colors.Transparent);
            BtnBlack.Background = new SolidColorBrush(Colors.LightGray);

            SettingsClass settings = App.Current.Resources["Settings"] as SettingsClass;
            if (settings != null)
            {
                settings.SelectedForeground = "White";
                settings.SelectedBackground = "Black";
            }
            UpdateStory();
        }

        private void btnWhite_Click(object sender, RoutedEventArgs e)
        {
            BtnWhite.Background = new SolidColorBrush(Colors.LightGray);
            BtnBlack.Background = new SolidColorBrush(Colors.Transparent);

            SettingsClass settings = App.Current.Resources["Settings"] as SettingsClass;
            if (settings != null)
            {
                settings.SelectedForeground = "Black";
                settings.SelectedBackground = "White";
            }
            UpdateStory();
        }
    }
}