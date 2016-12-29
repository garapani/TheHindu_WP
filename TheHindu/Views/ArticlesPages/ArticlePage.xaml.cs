using Microsoft.Phone.Controls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;
using TheHindu.Model;
using TheHindu.ViewModel;

namespace TheHindu.Views
{
    public partial class ArticlePage
    {
        #region Fields

        private double _fontSize = 20;
        private readonly ArticleViewModel _articleViewModel;

        private System.Windows.Threading.Dispatcher dispatcher { get; set; }

        #endregion Fields

        #region Constructor

        public ArticlePage()
        {
            InitializeComponent();
            ApplicationBar.IsVisible = false;
            ViewModelLocator viewModelLocator = App.Current.Resources["Locator"] as ViewModelLocator;
            if (viewModelLocator != null) _articleViewModel = viewModelLocator.ArticleViewModel;
        }

        #endregion Constructor

        #region PhoneApplicationPage Overrides

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                base.OnNavigatedTo(e);

                if (_articleViewModel == null) return;
                ApplicationBar.IsVisible = true;
                if (e.NavigationMode == NavigationMode.New)
                {
                    string id, category;
                    NavigationContext.QueryString.TryGetValue("Id", out id);
                    NavigationContext.QueryString.TryGetValue("Category", out category);
                    Article article = await _articleViewModel.ReadArticleAsync(id, category);
                    if (article != null)
                    {
                        OnePivot.DataContext = _articleViewModel.Article = article;
                        UpdateStory();
                        UpdateFontStyleButtons();
                    }
                    else
                    {
                        NavigationService.GoBack();
                    }
                }
                else
                {
                    if (_articleViewModel.Article != null)
                    {
                        string id = _articleViewModel.Article.ArticleId;
                        string category = _articleViewModel.Article.Category;
                        OnePivot.DataContext = _articleViewModel.Article = await _articleViewModel.ReadArticleAsync(id, category);
                        UpdateStory();
                        UpdateFontStyleButtons();
                    }
                    else
                    {
                        NavigationService.GoBack();
                    }
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("ArticlePage.xaml.cs:" + exception);
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            try
            {
                if (e.NavigationMode == NavigationMode.Back)
                {
                    _previousIndex = 0;
                    if (_articleViewModel != null)
                    {
                        ApplicationBar.IsVisible = false;
                        _articleViewModel.ClearArticle();
                    }
                }
                base.OnNavigatedFrom(e);
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("ArticlePage.xaml.cs:" + exception);
                }
            }
        }

        #endregion PhoneApplicationPage Overrides

        #region Private Methods

        private void UpdateAllStories()
        {
            UpdateStory();
            UpdateSecondPivot();
            UpdateThirdPivot();
        }

        private void UpdateStory()
        {
            ScrollViewer.ScrollToVerticalOffset(0);
            ScrollViewer.ScrollToHorizontalOffset(0);
            SettingsClass settings = App.Current.Resources["Settings"] as SettingsClass;
            Story.Html = "";
            if (settings != null)
            {
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
            }
            Article temp = ScrollViewer.DataContext as Article;
            if (temp != null)
            {
                Story.Html = "";
                Story.Html = temp.FullDescription;
            }
            ApplicationBar.IsVisible = true;
            FontStyleSelection.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void UpdateSecondPivot()
        {
            SecondScrollBar.ScrollToVerticalOffset(0);
            SecondScrollBar.ScrollToHorizontalOffset(0);
            SettingsClass settings = App.Current.Resources["Settings"] as SettingsClass;
            if (settings != null)
            {
                if (settings.FontFamily == "Georgia")
                {
                    SecondPivotHeadLine.FontFamily = new FontFamily("/Assets/Fonts/Roboto.ttf#Roboto");
                    SecondPivotStory.FontFamily = new FontFamily("/Assets/Fonts/Roboto.ttf#Roboto");
                    SecondPivotPhotoCaption.FontFamily = new FontFamily("/Assets/Fonts/Roboto.ttf#Roboto");
                }
                else if (settings.FontFamily.ToLower() == "calibri")
                {
                    SecondPivotHeadLine.FontFamily = new FontFamily("/Assets/Fonts/ArbutusSlab-regular.ttf#Arbutus Slab");
                    SecondPivotStory.FontFamily = new FontFamily("/Assets/Fonts/ArbutusSlab-regular.ttf#Arbutus Slab");
                    SecondPivotPhotoCaption.FontFamily = new FontFamily("/Assets/Fonts/ArbutusSlab-regular.ttf#Arbutus Slab");
                }
                else
                {
                    SecondPivotHeadLine.FontFamily = new FontFamily(settings.FontFamily);
                    SecondPivotStory.FontFamily = new FontFamily(settings.FontFamily);
                    SecondPivotPhotoCaption.FontFamily = new FontFamily(settings.FontFamily);
                }
                SecondPivotStory.FontSize = settings.FontSize;
                if (settings.SelectedForeground == "White")
                {
                    SecondPivotStory.Foreground = new SolidColorBrush(Colors.White);
                }
                else
                {
                    SecondPivotStory.Foreground = new SolidColorBrush(Colors.Black);
                }
            }
            Article temp = SecondScrollBar.DataContext as Article;
            if (temp != null)
            {
                SecondPivotStory.Html = "";
                SecondPivotStory.Html = temp.FullDescription;
            }
            ApplicationBar.IsVisible = true;
            FontStyleSelection.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void UpdateThirdPivot()
        {
            ThirdScrollBar.ScrollToVerticalOffset(0);
            ThirdScrollBar.ScrollToHorizontalOffset(0);
            SettingsClass settings = App.Current.Resources["Settings"] as SettingsClass;
            if (settings != null)
            {
                if (settings.FontFamily == "Georgia")
                {
                    ThirdPivotHeadLine.FontFamily = new FontFamily("/Assets/Fonts/Roboto.ttf#Roboto");
                    ThirdPivotStory.FontFamily = new FontFamily("/Assets/Fonts/Roboto.ttf#Roboto");
                    ThirdPivotPhotoCaption.FontFamily = new FontFamily("/Assets/Fonts/Roboto.ttf#Roboto");
                }
                else if (settings.FontFamily.ToLower() == "calibri")
                {
                    ThirdPivotHeadLine.FontFamily = new FontFamily("/Assets/Fonts/ArbutusSlab-regular.ttf#Arbutus Slab");
                    ThirdPivotStory.FontFamily = new FontFamily("/Assets/Fonts/ArbutusSlab-regular.ttf#Arbutus Slab");
                    ThirdPivotPhotoCaption.FontFamily = new FontFamily("/Assets/Fonts/ArbutusSlab-regular.ttf#Arbutus Slab");
                }
                else
                {
                    ThirdPivotHeadLine.FontFamily = new FontFamily(settings.FontFamily);
                    ThirdPivotStory.FontFamily = new FontFamily(settings.FontFamily);
                    ThirdPivotPhotoCaption.FontFamily = new FontFamily(settings.FontFamily);
                }
                ThirdPivotStory.FontSize = settings.FontSize;
                if (settings.SelectedForeground == "White")
                {
                    ThirdPivotStory.Foreground = new SolidColorBrush(Colors.White);
                }
                else
                {
                    ThirdPivotStory.Foreground = new SolidColorBrush(Colors.Black);
                }
            }
            Article temp = ThirdScrollBar.DataContext as Article;
            if (temp != null)
            {
                ThirdPivotStory.Html = "";
                ThirdPivotStory.Html = temp.FullDescription;
            }
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

        private void UpdateAppBar()
        {
            ApplicationBar.IsVisible = true;
        }

        private void InitializePage()
        {
            ApplicationBar.IsVisible = false;
        }

        private void ContentPanel_DoubleTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (_fontSize == 20)
            {
                _fontSize = 22.667;
            }
            else
            {
                _fontSize = 20;
            }
        }

        private void btnSmallFont_Click(object sender, RoutedEventArgs e)
        {
            SettingsClass settings = App.Current.Resources["Settings"] as SettingsClass;
            if (settings != null)
            {
                settings.FontSize = 18.667;
            }
            UpdateAllStories();
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
            UpdateAllStories();
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
            UpdateAllStories();
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
            UpdateAllStories();
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
            UpdateAllStories();
            BtnSmallFont.Background = new SolidColorBrush(Colors.Transparent);
            BtnNormalFont.Background = new SolidColorBrush(Colors.Transparent);
            BtnMediumFont.Background = new SolidColorBrush(Colors.Transparent);
            BtnMediumLargeFont.Background = new SolidColorBrush(Colors.Transparent);
            BtnLargeFont.Background = new SolidColorBrush(Colors.LightGray);
        }

        private void btnSegoeWPFont_Click(object sender, RoutedEventArgs e)
        {
            SettingsClass settings = App.Current.Resources["Settings"] as SettingsClass;
            if (settings != null)
            {
                settings.FontFamily = "Segoe WP";
            }
            BtnSegoeWpFont.Background = new SolidColorBrush(Colors.LightGray);
            BtnCalibriFont.Background = new SolidColorBrush(Colors.Transparent);
            BtnGeorgia.Background = new SolidColorBrush(Colors.Transparent);
            UpdateAllStories();
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
            UpdateAllStories();
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
            UpdateAllStories();
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
            UpdateAllStories();
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
            UpdateAllStories();
        }

        private void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (_articleViewModel != null && _articleViewModel.Article != null && !string.IsNullOrEmpty(_articleViewModel.Article.HdThumbnail))
            {
                var rootFrame = (App.Current as App).RootFrame;
                rootFrame.Navigate(new Uri(string.Format("/Views/ImageViewer.xaml?Id={0}", _articleViewModel.Article.HdThumbnail), UriKind.Relative));
            }
        }

        private static int _previousIndex = 0;

        private async void Pivot_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Pivot pivotcontrol = sender as Pivot;
            if (pivotcontrol == null) return;
            switch (pivotcontrol.SelectedIndex)
            {
                case 0:
                    {
                        if (_articleViewModel != null)
                        {
                            ApplicationBar.IsVisible = false;
                            SecondScrollBar.Visibility = Visibility.Collapsed;
                            ThirdScrollBar.Visibility = Visibility.Collapsed;

                            ScrollViewer.Visibility = Visibility.Visible;
                            if (_previousIndex == 2)
                            {
                                if (_articleViewModel.NextArticle != null)
                                {
                                    OnePivot.DataContext = await _articleViewModel.ReadArticleAsync(_articleViewModel.NextArticle.ArticleId, _articleViewModel.NextArticle.Category);
                                    UpdateStory();
                                }
                            }
                            else if (_previousIndex == 1)
                            {
                                if (_articleViewModel.PreviousArticle != null)
                                {
                                    OnePivot.DataContext = await _articleViewModel.ReadArticleAsync(_articleViewModel.PreviousArticle.ArticleId, _articleViewModel.PreviousArticle.Category);
                                    UpdateStory();
                                }
                            }
                            else
                            {
                                if (_articleViewModel.Article != null)
                                {
                                    _articleViewModel.Article = await _articleViewModel.ReadArticleAsync(_articleViewModel.Article.ArticleId, _articleViewModel.Article.Category);
                                    OnePivot.DataContext = _articleViewModel.Article;
                                    UpdateStory();
                                }
                            }
                        }
                        _previousIndex = 0;
                        ApplicationBar.IsVisible = true;
                    }
                    break;

                case 1:
                    {
                        if (_articleViewModel != null)
                        {
                            ApplicationBar.IsVisible = false;
                            ScrollViewer.Visibility = Visibility.Collapsed;
                            SecondScrollBar.Visibility = Visibility.Visible;
                            ThirdScrollBar.Visibility = Visibility.Collapsed;

                            SecondPivot.DataContext = null;
                            if (_previousIndex == 0)
                            {
                                if (_articleViewModel.NextArticle != null)
                                {
                                    SecondPivot.DataContext = await _articleViewModel.ReadArticleAsync(_articleViewModel.NextArticle.ArticleId, _articleViewModel.NextArticle.Category);
                                    UpdateSecondPivot();
                                }
                            }
                            else if (_previousIndex == 2)
                            {
                                if (_articleViewModel.PreviousArticle != null)
                                {
                                    SecondPivot.DataContext = await _articleViewModel.ReadArticleAsync(_articleViewModel.PreviousArticle.ArticleId, _articleViewModel.PreviousArticle.Category);
                                    UpdateSecondPivot();
                                }
                            }
                        }
                        _previousIndex = 1;
                        ApplicationBar.IsVisible = true;
                    }
                    break;

                case 2:
                    {
                        if (_articleViewModel != null)
                        {
                            ApplicationBar.IsVisible = false;
                            ScrollViewer.Visibility = Visibility.Collapsed;
                            SecondScrollBar.Visibility = Visibility.Collapsed;
                            ThirdScrollBar.Visibility = Visibility.Visible;

                            ThirdPivot.DataContext = null;
                            if (_previousIndex == 1)
                            {
                                if (_articleViewModel.NextArticle != null)
                                {
                                    ThirdPivot.DataContext = await _articleViewModel.ReadArticleAsync(_articleViewModel.NextArticle.ArticleId, _articleViewModel.NextArticle.Category);
                                    UpdateThirdPivot();
                                }
                            }
                            else if (_previousIndex == 0)
                            {
                                if (_articleViewModel.PreviousArticle != null)
                                {
                                    ThirdPivot.DataContext = await _articleViewModel.ReadArticleAsync(_articleViewModel.PreviousArticle.ArticleId, _articleViewModel.PreviousArticle.Category);
                                    UpdateThirdPivot();
                                }
                            }
                        }
                        _previousIndex = 2;
                        ApplicationBar.IsVisible = true;
                    }
                    break;
            }
        }

        #endregion Private Methods
    }
}