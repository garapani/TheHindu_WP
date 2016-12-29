using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using TheHindu.Model;
using TheHindu.Services;

namespace TheHindu.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public string favCategoryPage = "Favorites";
        public string categoriesPage = "Categories";

        #region Fields

        private const int BatchSize = 5;
        private readonly DataService _dataService;
        private readonly TransitionFrame _rootFrame;
        private bool _moreTopStoriesAvailable = true;
        private bool _moreBreakingNewsAvailable = true;
        private static bool _isLoadedOnce = false;

        #endregion Fields

        #region Properties

        public bool HasInternet { get; private set; }

        public bool IsCachedModeMessageDisplayed { get; set; }

        public bool IsRefreshingArticles { get; set; }

        public Article HeadLineArticle { get; private set; }

        public ObservableCollection<Article> TopStoriesArticles { get; set; }

        public bool IsRefreshingTopStoriesArticles { get; set; }

        public ObservableCollection<Article> BreakingNewsArticles { get; set; }

        public bool IsRefreshingBreakingNewsArticles { get; set; }

        //public bool IsDataSavingMode { get; set; }
        public Article CurrentArticle { get; set; }

        private ObservableCollection<CategoryDetailsModel> _favCategories;

        public ObservableCollection<CategoryDetailsModel> FavCategories
        {
            get
            {
                return _favCategories;
            }
            set
            {
                _favCategories = value;
                RaisePropertyChanged("FavCategories");
            }
        }

        public bool IsRefreshingCategories { get; set; }

        private ObservableCollection<CategoryDetailsModel> _listOfcategories;

        public ObservableCollection<CategoryDetailsModel> ListOfCategories
        {
            get
            {
                return _listOfcategories;
            }
            set
            {
                _listOfcategories = value;
                RaisePropertyChanged("ListOfCategories");
            }
        }

        public bool IsNoFavorites { get; set; }

        public bool IsRefreshing { get; set; }

        public string ActivePage { get; set; }

        public RelayCommand LoadedCommand { get; private set; }

        public RelayCommand LoadTopStoriesCommand { get; private set; }

        public RelayCommand LoadBreakingNewsCommand { get; private set; }

        public RelayCommand RefreshCommand { get; private set; }

        public RelayCommand ShowSavedArticlesCommand { get; private set; }

        public RelayCommand ShowSettingsCommand { get; private set; }

        public RelayCommand ShowAboutCommand { get; private set; }

        public RelayCommand ReadArticleCommand { get; private set; }

        public RelayCommand ReadCurrentArticleCommand { get; private set; }

        public RelayCommand LoadFavCategoriesCommand { get; private set; }

        public RelayCommand LoadCategoiresCommand { get; private set; }

        public RelayCommand ShowShareTheAppCommand { get; private set; }

        public RelayCommand ShowRateTheAppCommand { get; private set; }

        #endregion Properties

        #region Constructor

        public MainViewModel(DataService dataService)
        {
            try
            {
                if (DesignerProperties.IsInDesignTool) return;
                _rootFrame = (App.Current as App).RootFrame;
                if (_favCategories == null)
                    _favCategories = new ObservableCollection<CategoryDetailsModel>();
                if (_listOfcategories == null)
                    _listOfcategories = new ObservableCollection<CategoryDetailsModel>();

                _dataService = dataService;
                Task.Run(() =>
                {
                    if (DatabaseOperations.GetInstance().IsCategoriesDbAlreadyCreated() == false)
                    {
                        MoveReferenceDatabase();
                    }
                }
                    );

                IsRefreshingTopStoriesArticles = true;
                //IsRefreshingTopStoriesArticles = _dataService.IsRefreshingTopNewsArticles;
                IsRefreshingBreakingNewsArticles = _dataService.IsRefreshingBreakingNewsArticles;

                if (CurrentArticle == null)
                {
                    CurrentArticle = new Article();
                }
                LoadBreakingNewsCommand = new RelayCommand(RefreshBreakingNews, () => MoreBreakingNewsAvailable);
                LoadTopStoriesCommand = new RelayCommand(RefreshTopNews, () => MoreTopStoriesAvailable);
                LoadedCommand = new RelayCommand(RefreshData);
                RefreshCommand = new RelayCommand(ForceRefreshData);
                ShowSavedArticlesCommand = new RelayCommand(ShowSavedArticles);
                ShowSettingsCommand = new RelayCommand(ShowSettings);
                ShowAboutCommand = new RelayCommand(ShowAbout);
                ReadArticleCommand = new RelayCommand(ReadArticle);
                ReadCurrentArticleCommand = new RelayCommand(ReadCurrentArticle);
                LoadFavCategoriesCommand = new RelayCommand(LoadFavCategories);
                LoadCategoiresCommand = new RelayCommand(LoadOrRefreshCategories);
                ShowShareTheAppCommand = new RelayCommand(ShowShareTheAppFunc);
                ShowRateTheAppCommand = new RelayCommand(ShowRateTheAppFunc);
                HasInternet = true;
                _dataService.OnDataRefreshed += DataServiceOnDataRefreshed;
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("MainViewModel:" + exception);
                }
            }
        }

        #endregion Constructor

        #region Event Handlers

        private async void DataServiceOnDataRefreshed(object sender, DataRefreshedEventArgs dataRefreshedEventArgs)
        {
            await DispatcherHelper.UIDispatcher.InvokeAsync(() =>
             {
                 try
                 {
                     switch (dataRefreshedEventArgs.DataType)
                     {
                         case DataType.TopStoriesArticles:
                             UpdateHeadLine();
                             UpdateTopStoriesArticles();
                             HasInternet = dataRefreshedEventArgs.IsSuccess;

                             if (!dataRefreshedEventArgs.IsSuccess && !IsCachedModeMessageDisplayed)
                             {
                                 IsCachedModeMessageDisplayed = true;
                             }
                             IsRefreshingTopStoriesArticles = false;
                             break;

                         case DataType.BreakingNewsArticles:
                             UpdateBreakingStoriesArticles();
                             HasInternet = dataRefreshedEventArgs.IsSuccess;

                             if (!dataRefreshedEventArgs.IsSuccess && !IsCachedModeMessageDisplayed)
                             {
                                 IsCachedModeMessageDisplayed = true;
                             }
                             IsRefreshingBreakingNewsArticles = false;
                             break;
                         case DataType.Categories:
                             IsRefreshingCategories = true;
                             if (ListOfCategories != null && ListOfCategories.Count != 0)
                             {
                                 IsRefreshingCategories = false;
                                 return;
                             }
                             else if (ListOfCategories == null)
                             {
                                 ListOfCategories = new ObservableCollection<CategoryDetailsModel>();
                             }
                             ListOfCategories.Clear();
                             foreach (var category in _dataService.ListOfCategories)
                             {
                                 ListOfCategories.Add(category);
                             }
                             IsRefreshingCategories = false;
                             break;
                     }
                 }
                 catch (Exception exception)
                 {
                     if (Debugger.IsAttached)
                     {
                         Debug.WriteLine("MainViewModel:" + exception);
                     }
                 }
             });
        }

        #endregion Event Handlers

        #region Command

        private bool MoreTopStoriesAvailable
        {
            get
            {
                return _moreTopStoriesAvailable;
            }
        }

        private bool MoreBreakingNewsAvailable
        {
            get
            {
                return _moreBreakingNewsAvailable;
            }
        }

        private async void RefreshData()
        {
            if (_isLoadedOnce != false) return;
            try
            {
                var factory = new TaskFactory();
                UpdateHeadLine();
                await factory.StartNew(async () =>
                {
                    if (await _dataService.CanRefreshTopArticlesAsync())
                    {
                        DispatcherHelper.UIDispatcher.BeginInvoke(() => { IsRefreshingTopStoriesArticles = true; });
                        await _dataService.RefreshTopArticlesAsync(false);
                    }
                    else
                    {
                        DispatcherHelper.UIDispatcher.BeginInvoke(() =>
                        {
                            IsRefreshingTopStoriesArticles = false;
                            UpdateTopStoriesArticles();
                        });
                    }
                });

                await factory.StartNew(async () =>
                {
                    if (await _dataService.CanRefreshBreakingNewsArticlesAsync())
                    {
                        DispatcherHelper.UIDispatcher.BeginInvoke(() => { IsRefreshingBreakingNewsArticles = true; });
                        await _dataService.RefreshBreakingNewsArticlesAsync(false);
                    }
                    else
                    {
                        DispatcherHelper.UIDispatcher.BeginInvoke(() =>
                        {
                            IsRefreshingBreakingNewsArticles = false;
                            UpdateBreakingStoriesArticles();
                        });
                    }
                });
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("MainViewModel:" + exception);
                }
            }
            _isLoadedOnce = true;
        }

        private void ForceRefreshData()
        {
            try
            {
                if (ActivePage == DataType.TopStoriesArticles.ToString())
                {
                    if (_dataService.NetWorkAvailable())
                    {
                        IsRefreshingTopStoriesArticles = true;
                        var factory = new TaskFactory();
                        factory.StartNew(async () => { await _dataService.RefreshTopArticlesAsync(true); });
                    }
                    else
                    {
                        IsRefreshingTopStoriesArticles = false;
                        //DispatcherHelper.UIDispatcher.BeginInvoke(() =>
                        //{
                        _rootFrame.Navigate(new Uri("/Views/NetworkError.xaml", UriKind.Relative));
                        //});
                    }
                }
                else if (ActivePage == DataType.BreakingNewsArticles.ToString())
                {
                    if (_dataService.NetWorkAvailable())
                    {
                        IsRefreshingBreakingNewsArticles = true;
                        var factory = new TaskFactory();
                        factory.StartNew(async () => { await _dataService.RefreshBreakingNewsArticlesAsync(true); });
                    }
                    else
                    {
                        IsRefreshingBreakingNewsArticles = false;
                        //DispatcherHelper.UIDispatcher.BeginInvoke(() =>
                        //{
                        _rootFrame.Navigate(new Uri("/Views/NetworkError.xaml", UriKind.Relative));
                        //});
                    }
                }
                else if (ActivePage == favCategoryPage || ActivePage == categoriesPage)
                {
                    if (_dataService.NetWorkAvailable())
                    {
                        IsRefreshingCategories = true;
                        var factory = new TaskFactory();
                        factory.StartNew(() => { RefreshCategories(); });
                    }
                    else
                    {
                        IsRefreshingCategories = false;
                        _rootFrame.Navigate(new Uri("/Views/NetworkError.xaml", UriKind.Relative));
                    }
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("MainViewModel:" + exception);
                }
            }
        }

        public void RefreshBreakingNews()
        {
            if (BreakingNewsArticles == null)
            {
                BreakingNewsArticles = new ObservableCollection<Article>();
            }
            var expectedCount = BreakingNewsArticles.Count + BatchSize;
            if (_dataService.BreakingNewsArticles.Count >= expectedCount)
            {
                var newArticles = _dataService.BreakingNewsArticles.GetRange(BreakingNewsArticles.Count, BatchSize);
                foreach (var article in newArticles)
                {
                    if (BreakingNewsArticles.FirstOrDefault(o => o.ArticleId == article.ArticleId) == null)
                    {
                        BreakingNewsArticles.Add(article);
                    }
                }
            }
            _moreBreakingNewsAvailable = expectedCount == BreakingNewsArticles.Count;
        }

        public void RefreshTopNews()
        {
            if (TopStoriesArticles == null)
            {
                TopStoriesArticles = new ObservableCollection<Article>();
            }
            var expectedCount = TopStoriesArticles.Count + BatchSize;
            if (_dataService.TopStoriesArticles.Count > expectedCount)
            {
                var newArticles = _dataService.TopStoriesArticles.GetRange(TopStoriesArticles.Count, BatchSize);
                foreach (var article in newArticles)
                {
                    if (TopStoriesArticles.FirstOrDefault(o => o.ArticleId == article.ArticleId) == null)
                    {
                        TopStoriesArticles.Add(article);
                    }
                }
            }
            _moreTopStoriesAvailable = expectedCount == TopStoriesArticles.Count;
        }

        private void ShowSavedArticles()
        {
            _rootFrame.Navigate(new Uri("/Views/SavedArticlesPage.xaml", UriKind.Relative));
        }

        private void ShowSettings()
        {
            _rootFrame.Navigate(new Uri("/Views/SettingsPage.xaml", UriKind.Relative));
        }

        private void ShowAbout()
        {
            _rootFrame.Navigate(new Uri("/Views/AboutPage.xaml", UriKind.Relative));
        }

        private void ReadArticle()
        {
            if (_dataService.NetWorkAvailable())
            {
                if (HeadLineArticle != null)
                {
                    _rootFrame.Navigate(new Uri(string.Format("/Views/ArticlesPages/TopStoriesArticlesPage.xaml?Id={0}&Category={1}", HeadLineArticle.ArticleId, DataService.TopStories), UriKind.Relative));
                }
            }
            else
            {
                if (HeadLineArticle != null && !string.IsNullOrEmpty(HeadLineArticle.FullDescription))
                {
                    _rootFrame.Navigate(new Uri(string.Format("/Views/ArticlesPages/TopStoriesArticlesPage.xaml?Id={0}&Category={1}", HeadLineArticle.ArticleId, DataService.TopStories), UriKind.Relative));
                }
                else
                {
                    DispatcherHelper.UIDispatcher.BeginInvoke(() =>
                    {
                        _rootFrame.Navigate(new Uri("/Views/NetworkError.xaml", UriKind.Relative));
                    });
                }
            }
            //_dataService.SetCurrentArticle(HeadLineArticle);
        }

        private void ReadCurrentArticle()
        {
            try
            {
                if (CurrentArticle == null) return;
                var canGoToArticle = false;

                if (!string.IsNullOrEmpty(CurrentArticle.FullDescription))
                {
                    canGoToArticle = true;
                }
                else
                {
                    if (_dataService.NetWorkAvailable())
                    {
                        canGoToArticle = true;
                    }
                    else
                    {
                        DispatcherHelper.UIDispatcher.BeginInvoke(() =>
                        {
                            _rootFrame.Navigate(new Uri("/Views/NetworkError.xaml", UriKind.Relative));
                        });
                    }
                }
                if (!canGoToArticle) return;
                switch (CurrentArticle.Category)
                {
                    case DataService.BreakingNews:
                        _rootFrame.Navigate(new Uri(string.Format("/Views/ArticlesPages/BreakingNewsArticlesPage.xaml?Id={0}&Category={1}", CurrentArticle.ArticleId, CurrentArticle.Category), UriKind.Relative));
                        break;

                    case DataService.TopStories:
                        _rootFrame.Navigate(new Uri(string.Format("/Views/ArticlesPages/TopStoriesArticlesPage.xaml?Id={0}&Category={1}", CurrentArticle.ArticleId, CurrentArticle.Category), UriKind.Relative));
                        break;

                    default:
                        {
                            _rootFrame.Navigate(new Uri(string.Format("/Views/ArticlesPages/ArticlePage.xaml?Id={0}&Category={1}", CurrentArticle.ArticleId, CurrentArticle.Category), UriKind.Relative));
                            break;
                        }
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("MainViewModel:" + exception);
                }
            }
        }

        private void UpdateNotifications(Article newArticle)
        {
            try
            {
                const string tempJpeg = "liveTile.jpg";
                var isLiveTileSupport = true;
                var viewModelLocator = App.Current.Resources["Locator"] as ViewModelLocator;
                if (viewModelLocator != null && viewModelLocator.SettingsViewModel != null && viewModelLocator.SettingsViewModel.Settings != null)
                {
                    if (viewModelLocator.SettingsViewModel.Settings.IsLiveTileSupport == false)
                    {
                        isLiveTileSupport = false;
                    }
                }
                if (newArticle == null || string.IsNullOrEmpty(newArticle.Title) ||
                    string.IsNullOrEmpty(newArticle.Thumbnail)) return;
                if (isLiveTileSupport == true)
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

                                var bitmapImage = new BitmapImage { CreateOptions = BitmapCreateOptions.DelayCreation };
                                bitmapImage.SetSource(streamResourceInfo.Stream);

                                var writeableBitmap = new WriteableBitmap(bitmapImage);
                                writeableBitmap.SaveJpeg(isolatedStorageFileStream, writeableBitmap.PixelWidth, writeableBitmap.PixelHeight, 0, 85);
                                isolatedStorageFileStream.Close();
                                DispatcherHelper.UIDispatcher.BeginInvoke(
                                    delegate
                                    {
                                        UpdateLiveTile(HeadLineArticle);
                                    });
                            }
                        };
                        webClient.OpenReadAsync(new Uri(HeadLineArticle.Thumbnail, UriKind.Absolute));
                    }
                    catch (Exception exception)
                    {
                        if (Debugger.IsAttached)
                        {
                            Debug.WriteLine("MainViewModel:" + exception);
                        }
                    }
                }
                else
                {
                    try
                    {
                        var appTile = ShellTile.ActiveTiles.First();
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
                            Debug.WriteLine("MainViewModel:" + exception);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("MainViewModel:" + exception);
                }
            }
        }

        private static void UpdateLiveTile(Article newArticle)
        {
            try
            {
                if (newArticle == null) return;
                var appTile = ShellTile.ActiveTiles.First();
                if (appTile == null) return;
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
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("MainViewModel:" + exception);
                }
            }
        }

        private async void LoadFavCategories()
        {
            try
            {
                if (_favCategories != null && _favCategories.Count == 0)
                {
                    var tempCategory = await _dataService.GetFavoriteCategoriesAsync();
                    if (tempCategory != null)
                    {
                        foreach (var temp in tempCategory)
                        {
                            _favCategories.Add(temp);
                        }
                    }
                }

                if (_favCategories != null && _favCategories.Count > 0)
                {
                    IsNoFavorites = false;
                }
                else
                {
                    IsNoFavorites = true;
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("MainViewModel:" + exception);
                }
            }
        }

        private async void LoadOrRefreshCategories()
        {
            DispatcherHelper.UIDispatcher.BeginInvoke(() =>
                        {
                            IsRefreshingCategories = true;
                        });
            try
            {
                DispatcherHelper.UIDispatcher.BeginInvoke(() =>
                        {
                            IsRefreshingCategories = false;
                            if (ListOfCategories == null || ListOfCategories.Count != 0) return;
                        });
                var categories = new List<CategoryDetailsModel>();
                foreach (var category in await _dataService.GetCategoryDetailsAsync())
                {
                    categories.Add(category);
                }
                if (categories != null && categories.Count >= 1)
                {
                    DispatcherHelper.UIDispatcher.BeginInvoke(() =>
                        {
                            ListOfCategories.Clear();
                            foreach (var category in categories.OrderBy(o => o.Order))
                            {
                                ListOfCategories.Add(category);
                            }
                        });
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("MainViewModel:" + exception);
                }
            }
            DispatcherHelper.UIDispatcher.BeginInvoke(() =>
                        {
                            IsRefreshingCategories = false;
                        });
        }

        private async void RefreshCategories()
        {
            DispatcherHelper.UIDispatcher.BeginInvoke(() =>
            {
                IsRefreshingCategories = true;

                if (ListOfCategories == null)
                {
                    ListOfCategories = new ObservableCollection<CategoryDetailsModel>();
                }
            });
            try
            {
                var categories = new List<CategoryDetailsModel>();
                foreach (var category in await _dataService.GetCategoryDetailsAsync(true))
                {
                    categories.Add(category);
                }
                if (categories != null && categories.Count >= 1)
                {
                    DispatcherHelper.UIDispatcher.BeginInvoke(() =>
                    {
                        ListOfCategories.Clear();
                        foreach (var category in categories.OrderBy(o => o.Order))
                        {
                            ListOfCategories.Add(category);
                        }
                    });
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("MainViewModel:" + exception);
                }
            }
            DispatcherHelper.UIDispatcher.BeginInvoke(() =>
            {
                IsRefreshingCategories = false;
                if (ListOfCategories != null && ListOfCategories.Count > 0)
                {
                    if (FavCategories == null)
                    {
                        FavCategories = new ObservableCollection<CategoryDetailsModel>();
                    }
                    FavCategories.Clear();
                    foreach (var category in ListOfCategories)
                    {
                        if (category.IsPinned == true)
                        {
                            FavCategories.Add(category);
                        }
                    }
                }
            });
        }

        #endregion Command

        #region Private Methods

        private static void MoveReferenceDatabase()
        {
            var iso = IsolatedStorageFile.GetUserStoreForApplication();

            using (var input = Application.GetResourceStream(new Uri("CategoryDB.sdf", UriKind.Relative)).Stream)
            {
                using (var output = iso.CreateFile("CategoryDB.sdf"))
                {
                    var readBuffer = new byte[4096];
                    var bytesRead = -1;

                    // Copy the file from the installation folder to the local folder.
                    while ((bytesRead = input.Read(readBuffer, 0, readBuffer.Length)) > 0)
                    {
                        output.Write(readBuffer, 0, bytesRead);
                    }
                }
            }
        }

        private void UpdateHeadLine()
        {
            try
            {
                if (_dataService == null || _dataService.HeadLineArticle == null ||
                    string.IsNullOrEmpty(_dataService.HeadLineArticle.ArticleId)) return;
                if (HeadLineArticle == null)
                {
                    HeadLineArticle = _dataService.HeadLineArticle;
                    UpdateNotifications(HeadLineArticle);
                }
                else if (HeadLineArticle.ArticleId != _dataService.HeadLineArticle.ArticleId)
                {
                    HeadLineArticle = _dataService.HeadLineArticle;
                    UpdateNotifications(HeadLineArticle);
                }

            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("MainViewModel:" + exception);
                }
            }
        }

        private void UpdateTopStoriesArticles()
        {
            try
            {
                if (_dataService.TopStoriesArticles == null || _dataService.TopStoriesArticles.Count <= 0) return;
                if (_dataService.TopStoriesArticles.Count >= 10)
                {
                    var newArticles = new List<Article>(_dataService.TopStoriesArticles.GetRange(0, 10));
                    if (newArticles.Count <= 0) return;
                    newArticles = newArticles.OrderBy(x => x.PublishDate).ToList();
                    if (TopStoriesArticles == null)
                    {
                        TopStoriesArticles = new ObservableCollection<Article>();
                    }
                    foreach (var article in newArticles)
                    {
                        if (TopStoriesArticles.FirstOrDefault(o => o.ArticleId == article.ArticleId) == null)
                        {
                            TopStoriesArticles.Insert(0, article);
                        }
                    }
                    //newArticles.ForEach(o => TopStoriesArticles.Add(o));
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("MainViewModel:" + exception);
                }
            }
        }

        private void UpdateBreakingStoriesArticles()
        {
            try
            {
                if (_dataService.BreakingNewsArticles == null || _dataService.BreakingNewsArticles.Count <= 0) return;
                if (_dataService.BreakingNewsArticles.Count >= 10)
                {
                    var newArticles = new List<Article>(_dataService.BreakingNewsArticles.GetRange(0, 10));
                    if (newArticles.Count <= 0) return;
                    newArticles = newArticles.OrderBy(x => x.PublishDate).ToList();
                    if (BreakingNewsArticles == null)
                    {
                        BreakingNewsArticles = new ObservableCollection<Article>();
                    }
                    foreach (var article in newArticles)
                    {
                        if (BreakingNewsArticles.FirstOrDefault(o => o.ArticleId == article.ArticleId) == null)
                        {
                            BreakingNewsArticles.Insert(0, article);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("MainViewModel:" + exception);
                }
            }
        }

        private void ShowShareTheAppFunc()
        {
            _rootFrame.Navigate(new Uri("/Views/SharePage.xaml", UriKind.Relative));
        }

        private static void ShowRateTheAppFunc()
        {
            var mp = new MarketplaceReviewTask();
            mp.Show();
        }

        #endregion Private Methods
    }
}