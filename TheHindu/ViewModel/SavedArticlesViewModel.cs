using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.Controls;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using TheHindu.Model;
using TheHindu.Services;

namespace TheHindu.ViewModel
{
    public class SavedArticlesViewModel : ViewModelBase
    {
        #region Fields

        private readonly DataService _dataService;

        private readonly TransitionFrame _rootFrame;

        #endregion Fields

        #region Constructor

        public SavedArticlesViewModel(DataService dataService)
        {
            try
            {
                if (DesignerProperties.IsInDesignTool) return;
                _rootFrame = (App.Current as App).RootFrame;
                _dataService = dataService;
                if (CurrentArticle == null)
                {
                    CurrentArticle = new Article();
                }
                ReadCurrentArticleCommand = new RelayCommand(ReadCurrentArticle);
                DeleteAllArticlesCommand = new RelayCommand(DeleteAllArticles);

                _dataService.OnSavedArticlesChanged += DataServiceOnSavedArticlesChanged;

                Articles = new ObservableCollection<Article>(_dataService.SavedArticles);
                if (Articles != null && Articles.Count <= 0)
                {
                    IsNoSavedArticles = true;
                }
                else
                {
                    IsNoSavedArticles = false;
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("SavedArticlesViewModel:" + exception);
                }
            }
        }

        #endregion Constructor

        #region Properties

        public ObservableCollection<Article> Articles { get; set; }

        public Article CurrentArticle { get; set; }

        public bool IsNoSavedArticles { get; set; }

        public RelayCommand ReadCurrentArticleCommand { get; private set; }

        public RelayCommand DeleteAllArticlesCommand { get; private set; }

        public RelayCommand LoadedCommand { get; private set; }

        #endregion Properties

        #region Command

        private void ReadCurrentArticle()
        {
            try
            {
                if (CurrentArticle != null && !string.IsNullOrEmpty(CurrentArticle.ArticleId))
                {
                    _dataService.ReadSavedArticle(CurrentArticle);
                    _rootFrame.Navigate(new Uri("/Views/SavedArticlePage.xaml", UriKind.Relative));
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("SavedArticlesViewModel:" + exception);
                }
            }
        }

        private void DeleteAllArticles()
        {
            try
            {
                _dataService.DeleteAllSavedArticles();
                Articles.Clear();
                _rootFrame.GoBack();
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("SavedArticlesViewModel:" + exception);
                }
            }
        }

        #endregion Command

        #region Event Handlers

        private void DataServiceOnSavedArticlesChanged(object sender, EventArgs e)
        {
            Articles = new ObservableCollection<Article>(_dataService.SavedArticles);
            if (Articles.Count <= 0)
            {
                IsNoSavedArticles = true;
            }
            else
            {
                IsNoSavedArticles = false;
            }
        }

        #endregion Event Handlers
    }
}