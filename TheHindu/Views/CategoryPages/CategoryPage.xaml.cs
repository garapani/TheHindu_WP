using Microsoft.Phone.Controls;
using System.Windows.Navigation;
using TheHindu.ViewModel;

namespace TheHindu.Views
{
    public partial class CategoryPage : PhoneApplicationPage
    {
        private CategoryPageViewModel _categoryPageViewModel;

        public CategoryPage()
        {
            InitializeComponent();
            _categoryPageViewModel = DataContext as CategoryPageViewModel;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            string category = NavigationContext.QueryString["category"];
            this.PageName.Text = category;
            if (e.NavigationMode != NavigationMode.Back)
            {
                await _categoryPageViewModel.GetCategoryArticlesAsync(category);
            }
            base.OnNavigatedTo(e);
        }

        private void adControlLatestNews_ErrorOccurred(object sender, Microsoft.Advertising.AdErrorEventArgs e)
        {
            //adControlArticle.Visibility = System.Windows.Visibility.Collapsed;
            //inMobiAd.Visibility = System.Windows.Visibility.Visible;
        }
    }
}