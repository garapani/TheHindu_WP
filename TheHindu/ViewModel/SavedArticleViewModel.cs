using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Phone.Tasks;
using System;
using System.ComponentModel;
using System.Diagnostics;
using TheHindu.Model;
using TheHindu.Services;

namespace TheHindu.ViewModel
{
    public class SavedArticlePropertyChangedMessage
    {
    }

    public class SavedArticlePropertyIntializedMessage : EventArgs
    {
    }

    public class SavedArticleViewModel : ViewModelBase
    {
        #region Fields

        private readonly DataService _dataService;

        #endregion Fields

        #region Constructor

        public SavedArticleViewModel(DataService dataService)
        {
            try
            {
                if (!DesignerProperties.IsInDesignTool)
                {
                    _dataService = dataService;

                    _dataService.OnSavedArticleChanged += DataServiceOnSavedArticleChanged;
                    OnSavedArticleInitialized += DataServiceOnArticleIntialized;
                    Article = _dataService.CurrentSavedArticle;

                    PreviousArticleCommand = new RelayCommand(MoveToPreviousArticle, () => CanMoveToPreviousArticle);
                    NextArticleCommand = new RelayCommand(MoveToNextArticle, () => CanMoveToNextArticle);
                    ShareEmailArticleCommand = new RelayCommand(ShareEmailArticle);
                    ShareArticleCommand = new RelayCommand(ShareArticle);
                    DeleteSavedArticleCommand = new RelayCommand(DeleteSavedArticle);
                    ShowSettingsCommand = new RelayCommand(ShowSettings);
                    OpenInIeCommand = new RelayCommand(OpenInIe);
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("SavedArticleViewModel:" + exception);
                }
            }
        }

        #endregion Constructor

        #region Properties

        public Article Article { get; set; }

        public RelayCommand PreviousArticleCommand { get; private set; }

        public RelayCommand NextArticleCommand { get; private set; }

        public RelayCommand ShareEmailArticleCommand { get; private set; }

        public RelayCommand ShareArticleCommand { get; private set; }

        public RelayCommand DeleteSavedArticleCommand { get; private set; }

        public RelayCommand ShowSettingsCommand { get; private set; }

        public RelayCommand OpenInIeCommand { get; private set; }

        #endregion Properties

        #region Commands

        private bool CanMoveToPreviousArticle
        {
            get { return _dataService.CanMoveToPreviousSavedArticle; }
        }

        private bool CanMoveToNextArticle
        {
            get { return _dataService.CanMoveToNextSavedArticle; }
        }

        private void MoveToPreviousArticle()
        {
            NotifyCurrentArticleInitialized();
            _dataService.MoveToPreviousSavedArticle();
        }

        private void MoveToNextArticle()
        {
            NotifyCurrentArticleInitialized();
            _dataService.MoveToNextSavedArticle();
        }

        private void ShareEmailArticle()
        {
            try
            {
                EmailComposeTask emailComposeTask = new EmailComposeTask
                {
                    Subject = string.Format("TheHindu: {0}", Article.Title),
                    Body =
                        string.Format(
                            "Hi,\n\nThis article will interest you: {0}\n\n{1}\n\nSent by the TheHindu application for Windows Phone",
                            Article.Title, Article.Url)
                };
                emailComposeTask.Show();
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("SavedArticleViewModel:" + exception);
                }
            }
        }

        private void ShareArticle()
        {
            try
            {
                ShareLinkTask shareLinkTask = new ShareLinkTask
                {
                    Title = "Read this article!",
                    LinkUri = new Uri(Article.Url, UriKind.Absolute),
                    Message = Article.Url
                };

                shareLinkTask.Show();
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("SavedArticleViewModel:" + exception);
                }
            }
        }

        private async void DeleteSavedArticle()
        {
            try
            {
                await _dataService.DeleteSavedArticleAsync(Article);

                DeleteSavedArticleCommand.RaiseCanExecuteChanged();
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("SavedArticleViewModel:" + exception);
                }
            }
        }

        private void OpenInIe()
        {
            try
            {
                if (Article != null)
                {
                    var wbt = new WebBrowserTask();
                    wbt.Uri = new Uri(Article.Url, UriKind.Absolute);
                    wbt.Show();
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("SavedArticleViewModel:" + exception);
                }
            }
        }

        private void ShowSettings()
        {
            var rootFrame = (App.Current as App).RootFrame;
            rootFrame.Navigate(new Uri("/Views/SettingsPage.xaml", UriKind.Relative));
        }

        #endregion Commands

        #region Event Handlers

        public event EventHandler<SavedArticlePropertyIntializedMessage> OnSavedArticleInitialized;

        private void DataServiceOnSavedArticleChanged(object sender, ArticleChangedEventArgs articleChangedEventArgs)
        {
            try
            {
                DispatcherHelper.UIDispatcher.BeginInvoke(delegate
                {
                    if (articleChangedEventArgs.Article == null)
                    {
                        var rootFrame = (App.Current as App).RootFrame;
                        rootFrame.GoBack();
                    }
                    else
                    {
                        Article = articleChangedEventArgs.Article;

                        Messenger.Default.Send(new SavedArticlePropertyChangedMessage());

                        NextArticleCommand.RaiseCanExecuteChanged();
                        PreviousArticleCommand.RaiseCanExecuteChanged();
                        DeleteSavedArticleCommand.RaiseCanExecuteChanged();
                    }
                });
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("SavedArticleViewModel:" + exception);
                }
            }
        }

        private void NotifyCurrentArticleInitialized()
        {
            OnSavedArticleInitialized(null, null);
        }

        private void DataServiceOnArticleIntialized(object sender, SavedArticlePropertyIntializedMessage articleChangedEventArgs)
        {
            Messenger.Default.Send(new SavedArticlePropertyIntializedMessage());
        }

        public void ClearArticle()
        {
            Article = null;
        }

        #endregion Event Handlers
    }
}