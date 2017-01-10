using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TheHindu.Model;
using TheHindu.Services;

namespace TheHindu.ViewModel
{
    public class CategoryPageViewModel : ViewModelBase
    {
        private readonly DataService _dataService;
        private string _selectedCategory;

        public RelayCommand LoadCategoryArticles { get; internal set; }

        public Article CurrentArticle { get; private set; }

        public RelayCommand ReadCurrentArticleCommand { get; private set; }

        public RelayCommand RefreshCommand { get; private set; }

        public CategoryPageViewModel(DataService dataService)
        {
            try
            {
                if (!DesignerProperties.IsInDesignTool)
                {
                    _dataService = dataService;
                    IsRefreshingArticles = true;
                    //_dataService.OnDataRefreshed += DataServiceOnDataRefreshed;
                    ReadCurrentArticleCommand = new RelayCommand(ReadCurrentArticle);
                    RefreshCommand = new RelayCommand(RefreshArticles);
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("CategoryPageViewModel:" + exception);
                }
            }
        }

        private async void RefreshArticles()
        {
            await GetCategoryArticlesAsync(_selectedCategory, true);
        }

        private async void ReadCurrentArticle()
        {
            try
            {
                bool canGoArticlePage = false;
                if (CurrentArticle != null && !string.IsNullOrEmpty(CurrentArticle.FullDescription))
                {
                    canGoArticlePage = true;
                }
                else if (CurrentArticle != null && string.IsNullOrEmpty(CurrentArticle.FullDescription))
                {
                    if (_dataService.NetWorkAvailable())
                    {
                        canGoArticlePage = true;
                    }
                    else
                    {
                        var rootFrame = (App.Current as App).RootFrame;
                        rootFrame.Navigate(new Uri("/Views/NetworkError.xaml", UriKind.Relative));
                    }
                }

                if (canGoArticlePage)
                {
                    var rootFrame = (App.Current as App).RootFrame;
                    switch (CurrentArticle.Category)
                    {
                        case DataService.BreakingNews:
                            rootFrame.Navigate(new Uri(string.Format("/Views/ArticlesPages/BreakingNewsArticlesPage.xaml?Id={0}&Category={1}", CurrentArticle.ArticleId, CurrentArticle.Category), UriKind.Relative));
                            break;

                        case DataService.TopStories:
                            rootFrame.Navigate(new Uri(string.Format("/Views/ArticlesPages/TopStoriesArticlesPage.xaml?Id={0}&Category={1}", CurrentArticle.ArticleId, CurrentArticle.Category), UriKind.Relative));
                            break;

                        //case DataService.Slideshows:
                        //    rootFrame.Navigate(new Uri(string.Format("/Views/ArticlesPages/SlideShowPage.xaml?Id={0}&Category={1}", CurrentArticle.Id, CurrentArticle.Category), UriKind.Relative));
                        //    break;
                        default:
                            rootFrame.Navigate(new Uri(string.Format("/Views/ArticlesPages/ArticlePage.xaml?Id={0}&Category={1}", CurrentArticle.ArticleId, CurrentArticle.Category), UriKind.Relative));
                            break;
                    }
                    CurrentArticle = await _dataService.ReadArticleAsync(CurrentArticle);
                }
                else
                {
                    var rootFrame = (App.Current as App).RootFrame;
                    rootFrame.Navigate(new Uri("/Views/NetworkError.xaml", UriKind.Relative));
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("CategoryPageViewModel:" + exception);
                }
            }
        }

        public async Task GetCategoryArticlesAsync(string category, bool bForce = false)
        {
            try
            {
                if (_dataService.NetWorkAvailable())
                {
                    if (Articles == null || Articles.Count == 0)
                    {
                        bForce = true;
                    }
                    if (_selectedCategory != category || bForce == true)
                    {
                        if (Articles != null)
                        {
                            Articles.Clear();
                        }
                        else
                        {
                            Articles = new ObservableCollection<Article>();
                        }
                        IsRefreshingArticles = true;
                        _selectedCategory = category;
                        var articles = await _dataService.LoadCategryArticlesAsync(category);

                        bool canRefresh = await _dataService.CanRefreshCategoryAsync(category);
                        if (articles == null || articles.Count == 0 || canRefresh || bForce)
                        {
                            string categoryUrl = _dataService.GetCategoryUrl(category);
                            var newArticles = await _dataService.GetCategoryArticlesAsync(category, categoryUrl);

                            if (newArticles != null)
                            {
                                if (articles == null)
                                {
                                    articles = newArticles.ToList();
                                }
                                else
                                {
                                    foreach (var article in newArticles.Where(article => articles.Find(o => o.ArticleId == article.ArticleId) == null))
                                    {
                                        articles.Add(article);
                                    }
                                }
                            }
                        }
                        if (articles != null)
                        {
                            if (Articles != null)
                            {
                                Articles.Clear();
                            }
                            else
                            {
                                Articles = new ObservableCollection<Article>();
                            }
                            foreach (var article in articles.OrderByDescending(o => o.PublishDate).ToList())
                            {
                                Articles.Add(article);
                            }
                        }

                        IsRefreshingArticles = false;
                    }
                }
                else
                {
                    var rootFrame = (App.Current as App).RootFrame;
                    rootFrame.Navigate(new Uri("/Views/NetworkError.xaml", UriKind.Relative));
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("CategoryPageViewModel:" + exception);
                }
            }
        }

        private ObservableCollection<Article> _articles;

        public ObservableCollection<Article> Articles
        {
            get
            {
                return _articles;
            }
            private set
            {
                this.Set<ObservableCollection<Article>>(ref _articles, value);
            }
        }

        private bool _hasInternet;

        public bool HasInternet
        {
            get { return _hasInternet; }
            internal set
            {
                this.Set<bool>(ref _hasInternet, value);
            }
        }

        private bool _isCachedModeMessageDisplayed = true;

        public bool IsCachedModeMessageDisplayed
        {
            get
            {
                return _isCachedModeMessageDisplayed;
            }
            set
            {
                this.Set<bool>(ref _isCachedModeMessageDisplayed, value);
            }
        }

        private bool _isRefreshingArticles = false;

        public bool IsRefreshingArticles
        {
            get { return _isRefreshingArticles; }
            set
            {
                this.Set<bool>(ref _isRefreshingArticles, value);
            }
        }
    }
}