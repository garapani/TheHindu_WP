using GalaSoft.MvvmLight.Threading;
using Microsoft.Phone.Shell;
using System;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using TheHindu.Model;
using TheHindu.ViewModel;

namespace TheHindu.Views
{
    public enum EnumListOfThemes
    {
        Dark,
        Light
    }

    internal enum EnumFontSize
    {
        Small,// = 15,
        Normal,// = 18,
        Medium,// = 20,
        Large,// = 25
    }

    internal enum EnumFontFamily
    {
        SegoeWp,
        TimesNewRoman,
        Arial,
        Georgia,
        Calibri,
        Verdana,
    }

    public partial class SettingsPage
    {
        private SettingsViewModel _settingViewModel;

        #region Constructor

        public SettingsPage()
        {
            InitializeComponent();
            _settingViewModel = DataContext as SettingsViewModel;
            SettingsClass settings = App.Current.Resources["Settings"] as SettingsClass;
            if (settings != null)
            {
                if (settings.SelectedBackground == "Black")
                {
                    BtnLight.Background = new SolidColorBrush(Colors.Transparent);
                    BtnDark.Background = new SolidColorBrush(Colors.LightGray);
                }
                else
                {
                    BtnLight.Background = new SolidColorBrush(Colors.LightGray);
                    BtnDark.Background = new SolidColorBrush(Colors.Transparent);
                }
            }
        }

        #endregion Constructor

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (LiveTileSupport.IsChecked == false)
            {
                try
                {
                    ShellTile appTile = ShellTile.ActiveTiles.First();
                    if (appTile != null)
                    {
                        appTile.Update(new FlipTileData()
                        {
                            BackContent = "",
                            WideBackContent = "",
                            SmallBackgroundImage = new Uri("/Assets/Icons/FlipCycleTileSmall.png", UriKind.RelativeOrAbsolute),
                            BackgroundImage = new Uri("/Assets/Icons/FlipCycleTileMedium.png", UriKind.RelativeOrAbsolute),
                            WideBackgroundImage = new Uri("/Assets/Icons/FlipCycleTileLarge.png", UriKind.RelativeOrAbsolute),
                            BackTitle = "The Hindu",
                            Title = "The Hindu"
                        });
                    }
                }
                catch (Exception exception)
                {
                    if (Debugger.IsAttached)
                    {
                        Debug.WriteLine("SettingPage.xaml.cs:" + exception);
                    }
                }
            }
            base.OnNavigatedFrom(e);
        }

        private void IsDataSavingEnabled_Checked(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("You must relaunch this app to apply changes", "Data Saving Mode", MessageBoxButton.OK);
        }

        private void btnDark_Click(object sender, RoutedEventArgs e)
        {
            SettingsClass settings = App.Current.Resources["Settings"] as SettingsClass;
            if (settings != null)
            {
                settings.SelectedBackground = "Black";
                settings.SelectedForeground = "White";
                BtnLight.Background = new SolidColorBrush(Colors.Transparent);
                BtnDark.Background = new SolidColorBrush(Colors.LightGray);
            }
        }

        private void btnLight_Click(object sender, RoutedEventArgs e)
        {
            SettingsClass settings = App.Current.Resources["Settings"] as SettingsClass;
            if (settings != null)
            {
                settings.SelectedBackground = "White";
                settings.SelectedForeground = "Black";
                BtnLight.Background = new SolidColorBrush(Colors.LightGray);
                BtnDark.Background = new SolidColorBrush(Colors.Transparent);
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            StartTime.Value = DateTime.Now;
            EndTime.Value = DateTime.Now;
        }

        private void LiveTileSupport_Checked(object sender, RoutedEventArgs e)
        {
            if (LiveTileSupport.IsChecked == false)
            {
                try
                {
                    ShellTile appTile = ShellTile.ActiveTiles.First();
                    if (appTile != null)
                    {
                        appTile.Update(new FlipTileData()
                        {
                            BackContent = "",
                            WideBackContent = "",
                            SmallBackgroundImage = new Uri("/Assets/Icons/FlipCycleTileSmall.png", UriKind.RelativeOrAbsolute),
                            BackgroundImage = new Uri("/Assets/Icons/FlipCycleTileMedium.png", UriKind.RelativeOrAbsolute),
                            WideBackgroundImage = new Uri("/Assets/Icons/FlipCycleTileLarge.png", UriKind.RelativeOrAbsolute),
                            BackTitle = "The Hindu",
                            Title = "The Hindu"
                        });
                    }
                }
                catch (Exception exception)
                {
                    if (Debugger.IsAttached)
                    {
                        Debug.WriteLine("SettingPage.xaml.cs:" + exception);
                    }
                }
            }
            else
            {
                ViewModelLocator viewModelLocator = App.Current.Resources["Locator"] as ViewModelLocator;
                if (viewModelLocator != null && viewModelLocator.MainViewModel != null && viewModelLocator.MainViewModel.HeadLineArticle != null)
                {
                    UpdateNotifications(viewModelLocator.MainViewModel.HeadLineArticle);
                }
            }
        }

        private void UpdateNotifications(Article newArticle)
        {
            try
            {
                string tempJpeg = "liveTile.jpg";

                if (newArticle != null && !string.IsNullOrEmpty(newArticle.Title) && !string.IsNullOrEmpty(newArticle.Thumbnail))
                {
                    try
                    {
                        var webClient = new WebClient();
                        webClient.OpenReadCompleted += (object sender, OpenReadCompletedEventArgs e) =>
                        {
                            if (e != null && e.Error == null && e.Cancelled == false)
                            {
                                var streamResourceInfo = new StreamResourceInfo(e.Result, null);

                                var userStoreForApplication = IsolatedStorageFile.GetUserStoreForApplication();
                                if (userStoreForApplication.FileExists("Shared/ShellContent/" + tempJpeg))
                                {
                                    userStoreForApplication.DeleteFile("Shared/ShellContent/" + tempJpeg);
                                }

                                var isolatedStorageFileStream = userStoreForApplication.CreateFile("Shared/ShellContent/" + tempJpeg);

                                var bitmapImage = new BitmapImage { CreateOptions = BitmapCreateOptions.None };
                                bitmapImage.SetSource(streamResourceInfo.Stream);

                                var writeableBitmap = new WriteableBitmap(bitmapImage);
                                writeableBitmap.SaveJpeg(isolatedStorageFileStream, writeableBitmap.PixelWidth, writeableBitmap.PixelHeight, 0, 85);
                                isolatedStorageFileStream.Close();
                                DispatcherHelper.UIDispatcher.BeginInvoke(
                                    delegate
                                    {
                                        UpdateLiveTile(newArticle);
                                    });
                            }
                        };
                        webClient.OpenReadAsync(new Uri(newArticle.Thumbnail, UriKind.Absolute));
                    }
                    catch (Exception exception)
                    {
                        if (Debugger.IsAttached)
                        {
                            Debug.WriteLine("SettingPage.xaml.cs:" + exception);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("SettingPage.xaml.cs:" + exception);
                }
            }
        }

        private void UpdateLiveTile(Article newArticle)
        {
            try
            {
                if (newArticle != null)
                {
                    ShellTile appTile = ShellTile.ActiveTiles.First();
                    if (appTile != null)
                    {
                        if (string.IsNullOrEmpty(newArticle.Thumbnail))
                        {
                            appTile.Update(new FlipTileData
                            {
                                BackContent = newArticle.Title,
                                WideBackContent = newArticle.Title,
                                BackTitle = "The Hindu"
                            });
                        }
                        else
                        {
                            var uri = new Uri("isostore:/Shared/ShellContent/" + "liveTile.jpg", UriKind.Absolute);
                            appTile.Update(new FlipTileData
                            {
                                BackContent = newArticle.Title,
                                WideBackContent = newArticle.Title,
                                BackgroundImage = uri,
                                WideBackgroundImage = uri,
                                BackTitle = "The Hindu"
                            });
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("SettingPage.xaml.cs:" + exception);
                }
            }
        }
    }
}