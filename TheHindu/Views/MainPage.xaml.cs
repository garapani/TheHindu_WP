using Microsoft.Phone.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Shell;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;
using TheHindu.Model;
using TheHindu.Services;
using TheHindu.ViewModel;

namespace TheHindu.Views
{
    public partial class MainPage
    {
        #region Constructor

        private readonly MainViewModel _mainViewModel;

        public MainPage()
        {
            InitializeComponent();
            ViewModelLocator viewModelLocator = App.Current.Resources["Locator"] as ViewModelLocator;
            if (viewModelLocator != null) _mainViewModel = viewModelLocator.MainViewModel;
        }

        #endregion Constructor

        #region Event Handlers

        private void PhoneApplicationPageLoaded(object sender, RoutedEventArgs e)
        {
            //SetApplicationBarVisibility();
        }

        #endregion Event Handlers

        private void Category_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            try
            {
                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    CategoryDetailsModel category = ((CategoryDetailsModel)((sender as FrameworkElement).DataContext));

                    if (category != null)
                    {
                        NavigationService.Navigate(new Uri("/Views/CategoryPages/CategoryPage.xaml?category=" + category.CategoryName, UriKind.Relative));
                    }
                }
                else
                {
                    NavigationService.Navigate(new Uri("/Views/NetworkError.xaml", UriKind.Relative));
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("MainPage.xaml.cs:" + exception);
                }
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CategoryDetailsModel category = ((CategoryDetailsModel)((sender as FrameworkElement).DataContext));
                if (category != null)
                {
                    ViewModelLocator viewModelLocator = App.Current.Resources["Locator"] as ViewModelLocator;
                    if (category.IsPinned == false)
                    {
                        category.IsPinned = true;
                        await viewModelLocator.DataService.AddFavCategoryAsync(category.CategoryName);
                        viewModelLocator.MainViewModel.FavCategories.Add(category);
                    }
                    else
                    {
                        category.IsPinned = false;
                        await viewModelLocator.DataService.RemoveFavCategoryAsync(category.CategoryName);
                        CategoryDetailsModel tempCategory = null;
                        foreach (CategoryDetailsModel item in viewModelLocator.MainViewModel.FavCategories)
                        {
                            if (item.CategoryName == category.CategoryName)
                            {
                                tempCategory = item;
                                break;
                            }
                        }
                        if (tempCategory != null)
                        {
                            viewModelLocator.MainViewModel.FavCategories.Remove(tempCategory);
                        }
                    }

                    if (viewModelLocator.MainViewModel.FavCategories.Count > 0)
                    {
                        viewModelLocator.MainViewModel.IsNoFavorites = false;
                    }
                    else
                    {
                        viewModelLocator.MainViewModel.IsNoFavorites = true;
                    }
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("MainPage.xaml.cs:" + exception);
                }
            }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            ApplicationBar.IsVisible = false;
            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ApplicationBar.IsVisible = true;
            ApplicationBar.Mode = ApplicationBarMode.Minimized;
            if (e.NavigationMode == NavigationMode.New)
            {
                string id, category;
                NavigationContext.QueryString.TryGetValue("Id", out id);
                NavigationContext.QueryString.TryGetValue("Category", out category);
                if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(category))
                {
                    switch (category)
                    {
                        case DataService.TopStories:
                            {
                                NavigationService.Navigate(new Uri(string.Format("/Views/ArticlesPages/TopStoriesArticlesPage.xaml?Id={0}&Category={1}", id, category), UriKind.RelativeOrAbsolute));
                                break;
                            }
                        case DataService.BreakingNews:
                            {
                                NavigationService.Navigate(new Uri(string.Format("/Views/ArticlesPages/BreakingNewsArticlesPage.xaml?Id={0}&Category={1}", id, category), UriKind.RelativeOrAbsolute));
                                break;
                            }
                        default:
                            {
                                NavigationService.Navigate(new Uri(string.Format("/Views/ArticlesPages/ArticlePage.xaml?Id={0}&Category={1}", id, category), UriKind.RelativeOrAbsolute));
                                break;
                            }
                    }
                }
            }
        }

        private void Pivot_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ApplicationBar.IsVisible = true;
            ViewModelLocator viewModelLocator = App.Current.Resources["Locator"] as ViewModelLocator;
            if (viewModelLocator != null && viewModelLocator.MainViewModel != null)
            {
                switch ((sender as Pivot).SelectedIndex)
                {
                    case 1:
                        {
                            //BreakingNewsGrid.Visibility = Visibility.Visible;
                            //TopStoriesGrid.Visibility = Visibility.Visible;
                            //HeadLinesGrid.Visibility = Visibility.Visible;
                            //CategoriesGrid.Visibility = Visibility.Collapsed;
                            //FavoritesGrid.Visibility = Visibility.Collapsed;
                            viewModelLocator.MainViewModel.ActivePage = TheHindu.Services.DataType.BreakingNewsArticles.ToString();
                        }
                        break;

                    case 0:
                        {
                            //BreakingNewsGrid.Visibility = Visibility.Visible;
                            //TopStoriesGrid.Visibility = Visibility.Collapsed;
                            //HeadLinesGrid.Visibility = Visibility.Visible;
                            //CategoriesGrid.Visibility = Visibility.Visible;
                            //FavoritesGrid.Visibility = Visibility.Collapsed;
                            viewModelLocator.MainViewModel.ActivePage = TheHindu.Services.DataType.TopStoriesArticles.ToString();
                        }
                        break;

                    case 2:
                        {
                            //BreakingNewsGrid.Visibility = Visibility.Visible;
                            //TopStoriesGrid.Visibility = Visibility.Visible;
                            //HeadLinesGrid.Visibility = Visibility.Collapsed;
                            //CategoriesGrid.Visibility = Visibility.Collapsed;
                            //FavoritesGrid.Visibility = Visibility.Visible;
                            viewModelLocator.MainViewModel.ActivePage = TheHindu.Services.DataType.TopStoriesArticles.ToString();
                        }
                        break;

                    case 3:
                        {
                            //BreakingNewsGrid.Visibility = Visibility.Collapsed;
                            //TopStoriesGrid.Visibility = Visibility.Collapsed;
                            //HeadLinesGrid.Visibility = Visibility.Collapsed;
                            //CategoriesGrid.Visibility = Visibility.Collapsed;
                            //FavoritesGrid.Visibility = Visibility.Visible;
                            viewModelLocator.MainViewModel.ActivePage = viewModelLocator.MainViewModel.favCategoryPage;
                        }
                        break;

                    case 4:
                        {
                            //BreakingNewsGrid.Visibility = Visibility.Collapsed;
                            //TopStoriesGrid.Visibility = Visibility.Collapsed;
                            //HeadLinesGrid.Visibility = Visibility.Visible;
                            //CategoriesGrid.Visibility = Visibility.Visible;
                            //FavoritesGrid.Visibility = Visibility.Visible;
                            viewModelLocator.MainViewModel.ActivePage = viewModelLocator.MainViewModel.categoriesPage;
                        }
                        break;

                    default:
                        {
                            ApplicationBar.IsVisible = false;
                            break;
                        }
                }
            }
        }

        private void adControlCategories_ErrorOccurred(object sender, Microsoft.Advertising.AdErrorEventArgs e)
        {
            //adControlLatestNews.Visibility = Visibility.Collapsed;
            //inMobiAd.Visibility = Visibility.Visible;
        }

        private void adControlSecondAd_ErrorOccurred(object sender, Microsoft.Advertising.AdErrorEventArgs e)
        {
        }

        private void MyOtherApps_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/MyAppsPage.xaml", UriKind.Relative));
        }
    }
}