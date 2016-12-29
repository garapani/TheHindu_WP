using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Phone.Tasks;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using TheHindu.Helper;
using TheHindu.Model;
using TheHindu.Services;

namespace TheHindu.ViewModel
{
    public class ArticleViewModel : ViewModelBase
    {
        #region Fields

        private readonly DataService _dataService;

        #endregion Fields

        #region Constructor

        public ArticleViewModel(DataService dataService)
        {
            try
            {
                if (!DesignerProperties.IsInDesignTool)
                {
                    _dataService = dataService;
                    ShareEmailArticleCommand = new RelayCommand(ShareEmailArticle);
                    ShareArticleCommand = new RelayCommand(ShareArticle);
                    SaveArticleCommand = new RelayCommand(SaveArticle, () => CanSaveCurrentArticle);
                    ShowSettingsCommand = new RelayCommand(ShowSettings);
                    OpenInIeCommand = new RelayCommand(OpenInIe);
                    ShowRateTheappCommand = new RelayCommand(ShowRateTheappFunc);
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("ArticleViewModel:" + exception);
                }
            }
        }

        #endregion Constructor

        #region Properties

        private Article _article;

        private bool _isFetchingArticle = false;

        private Article _nextArticle;

        private Article _previousArticle;

        public Article Article
        {
            get
            {
                return _article;
            }
            set
            {
                _article = value;
                RaisePropertyChanged("Article");
            }
        }

        public bool IsFetchingArticle
        {
            get
            {
                return _isFetchingArticle;
            }
            set
            {
                _isFetchingArticle = value;
                RaisePropertyChanged("IsFetchingArticle");
            }
        }

        public Article NextArticle
        {
            get
            {
                return _nextArticle;
            }
            set
            {
                _nextArticle = value;
                RaisePropertyChanged("NextArticle");
            }
        }

        public Article PreviousArticle
        {
            get
            {
                return _previousArticle;
            }
            set
            {
                _previousArticle = value;
                RaisePropertyChanged("PreviousArticle");
            }
        }

        public Settings Settings
        {
            get
            {
                return _dataService.Settings;
            }
        }

        #endregion Properties

        #region Commands

        public RelayCommand NextArticleCommand { get; internal set; }

        public RelayCommand OpenInIeCommand { get; private set; }

        public RelayCommand PreviousArticleCommand { get; internal set; }

        public RelayCommand SaveArticleCommand { get; private set; }

        public RelayCommand ShareArticleCommand { get; private set; }

        public RelayCommand ShareEmailArticleCommand { get; private set; }

        public RelayCommand ShowRateTheappCommand { get; private set; }

        public RelayCommand ShowSettingsCommand { get; private set; }

        #endregion Commands

        private bool CanSaveCurrentArticle
        {
            get { return _dataService.CanSaveCurrentArticle; }
        }

        #region private methods

        private void DataServiceOnSavedArticleChanged(object sender, ArticleChangedEventArgs articleChangedEventArgs)
        {
            SaveArticleCommand.RaiseCanExecuteChanged();
        }

        private void OpenInIe()
        {
            var wbt = new WebBrowserTask { Uri = new Uri(Article.Url, UriKind.Absolute) };
            wbt.Show();
        }

        private async void SaveArticle()
        {
            await _dataService.SaveCurrentArticleAsync(Article);
            SaveArticleCommand.RaiseCanExecuteChanged();
        }

        private void ShareArticle()
        {
            try
            {
                var shareStatusTask = new ShareStatusTask
                {
                    Status = Article.Url.ToString()
                };
                shareStatusTask.Show();
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("ArticleViewModel:" + exception);
                }
            }
        }

        private void ShareEmailArticle()
        {
            try
            {
                var storeUri = DeepLinkHelper.BuildApplicationDeepLink();
                var emailComposeTask = new EmailComposeTask
                {
                    Subject = string.Format("TheHindu: {0}", Article.Title),
                    Body =
                        string.Format(
                            "Hi,\n\nThis article will interest you: {0}\n\n{1}\n\nSent by the \"The Hindu News App\" for Windows Phone 8.For the App you can click this link. " + storeUri,
                            Article.Title, Article.Url)
                };
                emailComposeTask.Show();
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("ArticleViewModel:" + exception);
                }
            }
        }

        private static void ShowRateTheappFunc()
        {
            new MarketplaceReviewTask().Show();
        }

        private void ShowSettings()
        {
            var rootFrame = (App.Current as App).RootFrame;
            rootFrame.Navigate(new Uri("/Views/SettingsPage.xaml", UriKind.Relative));
        }

        #endregion private methods

        public void ClearArticle()
        {
            Article = null;
            NextArticle = null;
            PreviousArticle = null;
        }

        public async Task<Article> ReadArticleAsync(string id, string category)
        {
            var tempArticle = new Article();
            try
            {
                DispatcherHelper.UIDispatcher.BeginInvoke(() => { IsFetchingArticle = true; });
                var temp = await _dataService.GetArticlesFromId(id, category);
                if (temp != null)
                {
                    tempArticle = Article = await _dataService.ReadArticleAsync(temp);
                    NextArticle = _dataService.GetNextArticle();
                    PreviousArticle = _dataService.GetPreviousArticle();
                    SaveArticleCommand.RaiseCanExecuteChanged();
                }
                else
                {
                    return null;
                }
                DispatcherHelper.UIDispatcher.BeginInvoke(() => { IsFetchingArticle = false; });
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("ArticleViewModel:" + exception);
                }
            }
            return tempArticle;
        }
    }
}