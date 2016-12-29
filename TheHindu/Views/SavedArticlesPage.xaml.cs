using System.Windows.Navigation;
using TheHindu.ViewModel;

namespace TheHindu.Views
{
    public partial class SavedArticlesPage
    {
        #region Constructor

        public SavedArticlesPage()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Overrides

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.NavigationMode == NavigationMode.Back)
            {
                SavedArticlesViewModel savedArticlesViewModel = DataContext as SavedArticlesViewModel;

                if (savedArticlesViewModel == null || savedArticlesViewModel.Articles == null || savedArticlesViewModel.Articles.Count == 0)
                {
                    NavigationService.GoBack();
                }
            }
        }

        #endregion Overrides
    }
}