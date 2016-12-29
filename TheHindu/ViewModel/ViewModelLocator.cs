using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using System.ComponentModel;
using TheHindu.Services;

namespace TheHindu.ViewModel
{
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
                SimpleIoc.Default.Register<DataService>();
                SimpleIoc.Default.Register<MainViewModel>();
                SimpleIoc.Default.Register<ArticleViewModel>(true);
                SimpleIoc.Default.Register<SlideshowViewModel>(true);
                SimpleIoc.Default.Register<SavedArticlesViewModel>();
                SimpleIoc.Default.Register<SavedArticleViewModel>(true);
                SimpleIoc.Default.Register<SettingsViewModel>();
                SimpleIoc.Default.Register<CategoryPageViewModel>();
            }
        }

        #region Methods

        public DataService DataService
        {
            get { return ServiceLocator.Current.GetInstance<DataService>(); }
        }

        public MainViewModel MainViewModel
        {
            get { return ServiceLocator.Current.GetInstance<MainViewModel>(); }
        }

        public ArticleViewModel ArticleViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ArticleViewModel>(); }
        }

        public SlideshowViewModel SlideshowViewModel
        {
            get { return ServiceLocator.Current.GetInstance<SlideshowViewModel>(); }
        }

        public SavedArticlesViewModel SavedArticlesViewModel
        {
            get { return ServiceLocator.Current.GetInstance<SavedArticlesViewModel>(); }
        }

        public SavedArticleViewModel SavedArticleViewModel
        {
            get { return ServiceLocator.Current.GetInstance<SavedArticleViewModel>(); }
        }

        public SettingsViewModel SettingsViewModel
        {
            get { return ServiceLocator.Current.GetInstance<SettingsViewModel>(); }
        }

        public CategoryPageViewModel CategoryPageViewModel
        {
            get { return ServiceLocator.Current.GetInstance<CategoryPageViewModel>(); }
        }

        public void Cleanup()
        {
            DataService.Save();
            _isSaved = true;
        }

        private bool _isSaved = false;

        public bool IsSaved()
        {
            return _isSaved;
        }

        #endregion Methods
    }
}