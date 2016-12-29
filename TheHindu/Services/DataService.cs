using GalaSoft.MvvmLight;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Threading.Tasks;
using TheHindu.Client;
using TheHindu.Model;
using Utilities;
using NetworkInterface = System.Net.NetworkInformation.NetworkInterface;

namespace TheHindu.Services
{
    public class DataService : ViewModelBase
    {
        #region Constants

        private const int ExpairyDays = 2;
        public const string TopStories = "Top Stories";
        public const string BreakingNews = "Breaking News";
        private const string HeadLineArticleFilename = "HeadLine.txt";
        private const string SavedArticlesFilename = "SavedArticles.txt";
        private const string AgentName = "TheHindu.Agent";

        #endregion Constants

        #region Fields

        private readonly TheHinduClient _theHinduClient = new TheHinduClient();

        #endregion Fields

        #region Constructor

        public DataService()
        {
            InitializeLiveTile();
            LoadSettings();
        }

        #endregion Constructor

        #region Properties

        public Settings Settings { get; private set; }

        public bool IsDataLoaded { get; private set; }

        public bool IsTopStoriesLoaded { get; private set; }

        public bool IsBreakingNewsLoaded { get; private set; }

        public bool IsArticlesLoaded { get; private set; }

        public bool IsRefreshingArticles { get; private set; }

        public List<Article> Articles { get; private set; }

        public bool IsRefreshingTopNewsArticles { get; private set; }

        public List<Article> TopStoriesArticles { get; private set; }

        public bool IsRefreshingBreakingNewsArticles { get; private set; }

        public List<Article> BreakingNewsArticles { get; private set; }

        public bool IsRefreshingSelectedCategoryArticles { get; private set; }

        public List<Article> SelectedCategoryArticles { get; private set; }

        private Article _currentArticle;

        public Article CurrentArticle
        {
            get { return _currentArticle; }
            set
            { this.Set<Article>("CurrentArticle", ref _currentArticle, value); }
        }

        public bool CanSaveCurrentArticle { get; private set; }

        public List<Article> SavedArticles { get; private set; }

        public Article CurrentSavedArticle { get; private set; }

        public bool CanMoveToPreviousSavedArticle { get; private set; }

        public bool CanMoveToNextSavedArticle { get; private set; }

        public Article HeadLineArticle { get; set; }

        public List<CategoryDetailsModel> ListOfCategories { get; set; }

        #endregion Properties

        #region Methods

        public async Task LoadAsync()
        {
            try
            {                
                TaskFactory factory = new TaskFactory();
                await factory.StartNew(async () => await DatabaseOperations.GetInstance().DeleteExpiredArticlesAsync());
                await factory.StartNew(async () => await LoadCategoryDetailsAsync());
                await factory.StartNew(async () => await LoadBreakingNewsArticlesAsync());
                await factory.StartNew(async () => await LoadTopStoriesArticlesAsync());
                await factory.StartNew(async () => await LoadSavedArticlesAsync());
                LoadCurrentDetails();
                if (Settings.IsRefreshAutomatic && NetWorkAvailable())
                {
                    if (await CanRefreshTopArticlesAsync())
                    {
                        await RefreshTopArticlesAsync(true);
                    }
                    if (await CanRefreshBreakingNewsArticlesAsync())
                    {
                        await RefreshBreakingNewsArticlesAsync(true);
                    }
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("DataService:" + exception);
                }
            }
        }

        public void Save()
        {
            try
            {
                SaveSettings();
                UpdateAgent();
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("DataService:" + exception);
                }
            }
        }

        public bool CanRefreshData()
        {
            return !IsRefreshingArticles && !IsDataLoaded;
        }

        public async Task<bool> CanRefreshTopArticlesAsync()
        {
            return NetWorkAvailable() && await CanRefreshCategoryAsync(TopStories);
        }

        public async Task<bool> CanRefreshBreakingNewsArticlesAsync()
        {
            return NetWorkAvailable() && await CanRefreshCategoryAsync(BreakingNews);
        }

        public bool CanRefreshArticles()
        {
            return !IsRefreshingArticles && !IsArticlesLoaded;
        }

        public async Task RefreshTopArticlesAsync(bool isForce, int page = 0)
        {
            try
            {
                var presentTime = DateTime.UtcNow;
                var refresh = false;
                refresh = await CanRefreshCategoryAsync(TopStories);
                if (refresh || isForce || page != 0)
                {
                    if (!IsRefreshingTopNewsArticles || isForce || page != 0)
                    {
                        IsRefreshingTopNewsArticles = true;
                        var topNews = await _theHinduClient.GetTopArticlesAsync(page);
                        if (topNews != null && topNews.Count > 0)
                        {
                            HeadLineArticle = topNews[0];                            
                            if (HeadLineArticle != null && !string.IsNullOrEmpty(HeadLineArticle.Thumbnail))
                            {
                                HeadLineArticle.HdThumbnail = HeadLineArticle.Thumbnail.Replace("i.jpg", "e.jpg");
                            }

                            topNews = topNews.OrderByDescending(o => o.PublishDate).ToList();

                            foreach (var newArticle in topNews)
                            {
                                newArticle.Category = TopStories;
                                if (TopStoriesArticles.Find(o => o.ArticleId == newArticle.ArticleId) == null)
                                {
                                    TopStoriesArticles.Add(newArticle);
                                }
                                else
                                {
                                    var oldArticle = TopStoriesArticles.Find(o => o.ArticleId == newArticle.ArticleId);
                                    if (oldArticle != null && oldArticle.FullDescription != newArticle.FullDescription)
                                    {
                                        oldArticle.FullDescription = newArticle.FullDescription;
                                    }
                                }
                            }

                            IsTopStoriesLoaded = true;
                            await DatabaseOperations.GetInstance().UpdateCategoryLastAccessedValueAsync(TopStories, DateTime.UtcNow);
                            SaveHeadLine();
                            await SaveTopStoriesArticlesAsync();
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("DataService:" + exception);
                }
            }
            TopStoriesArticles = TopStoriesArticles.OrderByDescending(o => o.PublishDate).ToList();
            IsRefreshingTopNewsArticles = false;
            NotifyDataRefreshed(DataType.TopStoriesArticles, true);
        }

        public async Task RefreshBreakingNewsArticlesAsync(bool isForce, int page = 0)
        {
            try
            {
                var presentTime = DateTime.UtcNow;
                var refresh = false;
                refresh = await CanRefreshCategoryAsync(BreakingNews);
                if (refresh || isForce || page != 0)
                {
                    if (!IsRefreshingBreakingNewsArticles || isForce || page != 0)
                    {
                        IsRefreshingBreakingNewsArticles = true;

                        var newBreakingNews = await _theHinduClient.GetBreakingNewsArticlesAsync(page);
                        if (newBreakingNews != null)
                        {
                            newBreakingNews = newBreakingNews.OrderByDescending(o => o.PublishDate).ToList();
                            foreach (var newArticle in newBreakingNews)
                            {
                                newArticle.Category = BreakingNews;
                                if (BreakingNewsArticles.Find(o => o.ArticleId == newArticle.ArticleId) == null)
                                {
                                    BreakingNewsArticles.Add(newArticle);
                                }
                                else
                                {
                                    var oldArticle = BreakingNewsArticles.Find(o => o.ArticleId == newArticle.ArticleId);
                                    if (oldArticle != null && oldArticle.FullDescription != newArticle.FullDescription)
                                    {
                                        oldArticle.FullDescription = newArticle.FullDescription;
                                    }
                                }
                            }

                            IsBreakingNewsLoaded = true;
                            await DatabaseOperations.GetInstance().UpdateCategoryLastAccessedValueAsync(BreakingNews, DateTime.UtcNow);
                            await SaveBreakingNewsArticlesAsync();
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("DataService:" + exception);
                }
            }
            BreakingNewsArticles = BreakingNewsArticles.OrderByDescending(o => o.PublishDate).ToList();
            IsRefreshingBreakingNewsArticles = false;
            NotifyDataRefreshed(DataType.BreakingNewsArticles, true);
        }

        public async Task<bool> CanRefreshCategoryAsync(string category)
        {
            var canFetch = true;

            var categoryObj = await DatabaseOperations.GetInstance().GetCategoryDetailAsync(category);
            if (categoryObj == null) return true;
            var diff = DateTime.UtcNow - categoryObj.LastFetchedTime;
            if (diff.TotalMinutes <= 5)
                canFetch = false;
            return canFetch;
        }

        public async Task<List<Article>> LoadCategryArticlesAsync(string category)
        {
            if (Articles == null)
            {
                Articles = new List<Article>();
            }
            Articles.Clear();
            var listOfArticles = new List<Article>();
            var oldArticles = await DatabaseOperations.GetInstance().GetCategoryArticlesAsync(category);
            if (oldArticles != null && oldArticles.Count != 0)
            {
                Articles.AddRange(oldArticles);
                listOfArticles.AddRange(oldArticles);
            }
            return listOfArticles;
        }

        public string GetCategoryUrl(string category)
        {
            return string.Format("http://thehindu.thevillagesoftware.com/api/Articles?categoryName={0}", category);
        }

        public async Task<List<CategoryDetailsModel>> GetCategoryDetailsAsync(bool bForece = false)
        {
            DateTime dateTime = IsolatedStorageHelper.GetDateTime("LastTimeRefreshOfCategory", DateTime.Now.AddHours(-25));
            var diff = DateTime.Now - dateTime;
            if (diff.TotalHours >= 24 || ListOfCategories.Count == 0 || bForece)
            {
                var categories = await _theHinduClient.GetCategoryDetailsAsync();
                if (categories != null)
                {
                    foreach (var category in categories)
                    {
                        await DatabaseOperations.GetInstance().UpdateCategoryAsync(category);
                    }
                    IsolatedStorageHelper.SaveSettingValueImmediately("LastTimeRefreshOfCategory", DateTime.Now);
                }
                await LoadCategoryDetailsAsync();
            }
            else if (ListOfCategories == null || ListOfCategories.Count > 0)
            {
                await LoadCategoryDetailsAsync();
            }
            return ListOfCategories;
        }

        public async Task LoadCategoryDetailsAsync()
        {
            if (ListOfCategories == null)
            {
                ListOfCategories = new List<CategoryDetailsModel>();
            }
            else
            {
                ListOfCategories.Clear();
            }
            try
            {
                var categories = await DatabaseOperations.GetInstance().GetCategoriesAsync();
                if (categories != null)
                {
                    foreach (var category in categories)
                    {
                        if (category.IsWorking == true)
                        {
                            ListOfCategories.Add(category);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            NotifyDataRefreshed(DataType.Categories, true);
        }

        public async Task<List<Article>> GetCategoryArticlesAsync(string category, string categoryUrl)
        {
            var listOfArticles = new List<Article>();
            try
            {
                var newArticles = await _theHinduClient.GetArticlesAsync(category, categoryUrl);

                if (newArticles != null)
                {
                    foreach (var article in newArticles)
                    {
                        if (listOfArticles.Find(o => o.ArticleId == article.ArticleId) == null)
                        {
                            listOfArticles.Add(article);
                        }
                    }
                }
                await DatabaseOperations.GetInstance().UpdateCategoryLastAccessedValueAsync(category, DateTime.UtcNow);
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("DataService:" + exception);
                }
            }

            if (Articles == null)
            {
                Articles = new List<Article>();
            }
            foreach (var article in listOfArticles)
            {
                if (Articles.Find(o => o.ArticleId == article.ArticleId) == null)
                {
                    Articles.Add(article);
                }
            }
            await SaveArticlesAsync();

            return listOfArticles;
        }

        public async Task<Article> ReadArticleAsync(Article article)
        {
            var tempArticle = new Article();
            try
            {
                var dbArticle = await GetArticleFromDbAsync(article.ArticleId, article.Category);
                if (dbArticle != null && !string.IsNullOrEmpty(dbArticle.FullDescription))
                {
                    tempArticle = dbArticle;
                    SetCurrentArticle(tempArticle);
                }
                else
                {
                    if (string.IsNullOrEmpty(article.FullDescription))
                    {
                        if (NetWorkAvailable())
                        {
                            tempArticle = await _theHinduClient.GetCurrentArticleAsync(article);
                            await UpdateStories(tempArticle);
                            SetCurrentArticle(tempArticle);
                        }
                    }
                    else
                    {
                        tempArticle = article;
                        SetCurrentArticle(article);
                    }
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("DataService:" + exception);
                }
            }
            return tempArticle;
        }

        private async Task<Article> GetArticleFromDbAsync(string Id, string category = null)
        {
            return await DatabaseOperations.GetInstance().GetArticleAsync(Id, category);
        }

        public async Task<SlideShowDetails> GetSlideShowDetailsAsync(string id)
        {
            SlideShowDetails slideShowDetails = null;
            slideShowDetails = await _theHinduClient.GetSlideShowDetailsAsync(id);

            if (slideShowDetails != null)
            {
                if (slideShowDetails.SlideDetails == null)
                {
                    slideShowDetails.SlideDetails = new List<Slidedetail>();
                    await DatabaseOperations.GetInstance().AddSlideShowAsync(slideShowDetails);
                }
            }
            return slideShowDetails;
        }

        public async Task<Article> GetArticlesFromId(string id, string category)
        {
            try
            {
                switch (category)
                {
                    case TopStories:
                        if (TopStoriesArticles == null)
                            return null;
                        if (TopStoriesArticles.Count == 0)
                        {
                            await LoadTopStoriesArticlesAsync();
                        }
                        return TopStoriesArticles != null ? TopStoriesArticles.Find(o => o.ArticleId == id) : null;

                    case BreakingNews:
                        return BreakingNewsArticles != null ? BreakingNewsArticles.Find(o => o.ArticleId == id) : null;

                    default:
                        return Articles != null ? Articles.Find(o => o.ArticleId == id) : null;
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("DataService:" + exception);
                }
                return null;
            }
        }

        public Article GetSavedArticlesFromId(string id)
        {
            if (SavedArticles != null)
            {
                CurrentSavedArticle = SavedArticles.Find(o => o.ArticleId == id);
                UpdateSavedArticleMoveStates();
                return CurrentSavedArticle;
            }
            else
            {
                return null;
            }
        }

        public bool NetWorkAvailable()
        {
            return NetworkInterface.GetIsNetworkAvailable();
        }

        public void ReadSavedArticle(Article article)
        {
            try
            {
                SetCurrentSavedArticle(article);
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("DataService:" + exception);
                }
            }
        }

        public void MoveToPreviousSavedArticle()
        {
            if (SavedArticles == null) return;
            for (var articleIndex = 0; articleIndex < SavedArticles.Count; articleIndex++)
            {
                if (SavedArticles[articleIndex].ArticleId != CurrentSavedArticle.ArticleId) continue;
                if (articleIndex - 1 >= 0)
                {
                    SetCurrentSavedArticle(SavedArticles[articleIndex - 1]);
                }
                break;
            }
        }

        public void MoveToNextSavedArticle()
        {
            if (SavedArticles == null) return;
            for (var articleIndex = 0; articleIndex < SavedArticles.Count; articleIndex++)
            {
                if (SavedArticles[articleIndex].ArticleId != CurrentSavedArticle.ArticleId) continue;
                if (articleIndex + 1 < SavedArticles.Count)
                {
                    SetCurrentSavedArticle(SavedArticles[articleIndex + 1]);
                }

                break;
            }
        }

        public async Task SaveCurrentArticleAsync(Article article)
        {
            try
            {
                if (IsArticleAlreadySaved(article)) return;
                CanSaveCurrentArticle = false;
                article.IsSaved = true;

                SavedArticles.Add(article);

                await SaveSavedArticlesAsync();

                NotifySavedArticlesChanged();
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("DataService:" + exception);
                }
            }
        }

        public async void DeleteAllSavedArticles()
        {
            try
            {
                if (SavedArticles != null)
                {
                    SavedArticles.ForEach(o => o.IsSaved = false);
                }
                await SaveSavedArticlesAsync();
                SavedArticles.Clear();
                CurrentSavedArticle = null;
                CanSaveCurrentArticle = true;
                NotifySavedArticlesChanged();
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("DataService:" + exception);
                }
            }
        }

        public async Task DeleteSavedArticleAsync(Article article)
        {
            if (article == null) return;
            try
            {
                SavedArticles.Find(o => o.ArticleId == article.ArticleId).IsSaved = false;
                await SaveSavedArticlesAsync();
                SavedArticles.Remove(article);
                CurrentSavedArticle = SavedArticles.Count > 0 ? SavedArticles[0] : null;
                UpdateSavedArticleMoveStates();
                NotifyCurrentSavedArticleChanged(CurrentSavedArticle);
                NotifySavedArticlesChanged();
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("DataService:" + exception);
                }
            }
        }

        public bool IsArticleAlreadySaved(Article article)
        {
            if (article != null && SavedArticles != null)
            {
                return SavedArticles.Any(savedArticle => savedArticle.ArticleId == article.ArticleId);
            }
            return false;
        }

        public async Task<List<CategoryDetailsModel>> GetFavoriteCategoriesAsync()
        {
            List<CategoryDetailsModel> listOfFavCategories = null;

            try
            {
                listOfFavCategories = await DatabaseOperations.GetInstance().GetFavCategoriesAsync();
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("DataService:" + exception);
                }
            }
            return listOfFavCategories;
        }

        public async Task<List<CategoryDetailsModel>> GetAllCategoriesAsync()
        {
            List<CategoryDetailsModel> listOfCategories = null;

            try
            {
                listOfCategories = await DatabaseOperations.GetInstance().GetCategoriesAsync();
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("DataService:" + exception);
                }
            }
            return listOfCategories;
        }

        public async Task AddFavCategoryAsync(string category)
        {
            try
            {
                await DatabaseOperations.GetInstance().UpdateCategoryPinValueAsync(category, true);
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("DataService:" + exception);
                }
            }
        }

        public async Task RemoveFavCategoryAsync(string category)
        {
            try
            {
                await DatabaseOperations.GetInstance().UpdateCategoryPinValueAsync(category, false);
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("DataService:" + exception);
                }
            }
        }

        #endregion Methods

        #region Events

        public event EventHandler<DataRefreshedEventArgs> OnDataRefreshed;

        public event EventHandler<ArticleChangedEventArgs> OnArticleChanged;

        public event EventHandler<ArticleChangedEventArgs> OnPreviousArticleChanged;

        public event EventHandler<ArticleChangedEventArgs> OnNextArticleChanged;

        public event EventHandler OnSavedArticlesChanged;

        public event EventHandler<ArticleChangedEventArgs> OnSavedArticleChanged;

        #endregion Events

        #region Private Methods

        public async Task LoadTopStoriesArticlesAsync()
        {
            if (TopStoriesArticles == null)
                TopStoriesArticles = new List<Article>();
            try
            {
                var articles = await DatabaseOperations.GetInstance().GetCategoryArticlesAsync(TopStories);

                if (articles != null && articles.Count > 0)
                {
                    articles = articles.OrderByDescending(o => o.PublishDate).ToList();
                    foreach (var article in articles)
                    {
                        if (TopStoriesArticles.Find(o => o.ArticleId == article.ArticleId) == null)
                            TopStoriesArticles.Add(article);
                    }
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("DataService:" + exception);
                }
            }
            IsRefreshingTopNewsArticles = false;
            NotifyDataRefreshed(DataType.TopStoriesArticles, true);
            IsTopStoriesLoaded = true;
        }

        public void LoadHeadLineArticle()
        {
            try
            {
                var oldHeadLineArticle = new Article();
                using (var isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (isolatedStorageFile.FileExists(HeadLineArticleFilename))
                    {
                        try
                        {
                            IsolatedStorageFileStream stream = null;
                            stream = isolatedStorageFile.OpenFile(HeadLineArticleFilename, FileMode.Open);
                            using (TextReader textReader = new StreamReader(stream))
                            {
                                var json = textReader.ReadToEnd();
                                JsonSerializerSettings settings = new JsonSerializerSettings
                                {
                                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                                };

                                oldHeadLineArticle = JsonConvert.DeserializeObject<Article>(json, settings);
                                stream = null;
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                if (oldHeadLineArticle == null)
                {
                    HeadLineArticle = new Article();
                }
                else
                {
                    if (HeadLineArticle == null)
                        HeadLineArticle = new Article();
                    HeadLineArticle = oldHeadLineArticle;
                }
            }
            catch (Exception exception)
            {
                HeadLineArticle = new Article();
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("DataService:" + exception);
                }
            }
        }

        private async Task LoadBreakingNewsArticlesAsync()
        {
            try
            {
                if (BreakingNewsArticles == null)
                    BreakingNewsArticles = new List<Article>();
                var articles = await DatabaseOperations.GetInstance().GetCategoryArticlesAsync(BreakingNews);
                if (articles != null && articles.Count > 0)
                {
                    articles = articles.OrderByDescending(o => o.PublishDate).ToList();

                    foreach (var article in articles.Where(article => BreakingNewsArticles.Find(o => o.ArticleId == article.ArticleId) == null))
                    {
                        BreakingNewsArticles.Add(article);
                    }
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("DataService:" + exception);
                }
            }
            IsBreakingNewsLoaded = true;
            IsRefreshingBreakingNewsArticles = false;
            NotifyDataRefreshed(DataType.BreakingNewsArticles, true);
        }

        private async Task SaveArticlesAsync()
        {
            try
            {
                foreach (var article in Articles)
                {
                    await DatabaseOperations.GetInstance().AddOrUpdateArticleAsync(article);
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("DataService:" + exception);
                }
            }
        }

        private async Task SaveTopStoriesArticlesAsync()
        {
            try
            {
                foreach (var article in TopStoriesArticles)
                {
                    await DatabaseOperations.GetInstance().AddOrUpdateArticleAsync(article);
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("DataService:" + exception);
                }
            }
        }

        private async Task SaveTopStoriesArticle(Article article)
        {
            try
            {
                await DatabaseOperations.GetInstance().AddOrUpdateArticleAsync(article);
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("DataService:" + exception);
                }
            }
        }

        private void SaveHeadLine()
        {
            try
            {
                using (IsolatedStorageFile isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    try
                    {
                        IsolatedStorageFileStream stream = null;
                        stream = isolatedStorageFile.CreateFile(HeadLineArticleFilename);
                        using (TextWriter textWriter = new StreamWriter(stream))
                        {
                            string jsonArticles = JsonConvert.SerializeObject(HeadLineArticle);
                            textWriter.Write(jsonArticles);
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("DataService:" + exception);
                }
            }
        }

        private async Task SaveBreakingNewsArticlesAsync()
        {
            try
            {
                foreach (var article in BreakingNewsArticles)
                {
                    await DatabaseOperations.GetInstance().AddOrUpdateArticleAsync(article);
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("DataService:" + exception);
                }
            }
        }

        private async Task SaveBreakingNewsArticleAsync(Article article)
        {
            try
            {
                await DatabaseOperations.GetInstance().AddOrUpdateArticleAsync(article);
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("DataService:" + exception);
                }
            }
        }

        private void SetCurrentArticle(Article article)
        {
            CurrentArticle = article;
            UpdateArticleMoveStates();
        }

        public async Task LoadSavedArticlesAsync()
        {
            if (SavedArticles == null)
                SavedArticles = new List<Article>();
            try
            {
                var articles = await DatabaseOperations.GetInstance().GetSavedArticlesAsync();

                if (articles == null || articles.Count <= 0) return;
                articles = articles.OrderByDescending(o => o.PublishDate).ToList();
                foreach (var article in articles.Where(article => SavedArticles.Find(o => o.ArticleId == article.ArticleId) == null))
                {
                    SavedArticles.Add(article);
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("DataService:" + exception);
                }
            }
            NotifySavedArticlesChanged();
        }

        private async Task SaveSavedArticlesAsync()
        {
            try
            {
                if (SavedArticles == null || SavedArticles.Count <= 0) return;
                foreach (var article in SavedArticles)
                {
                    await DatabaseOperations.GetInstance().AddOrUpdateArticleAsync(article);
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("DataService:" + exception);
                }
            }
            NotifySavedArticlesChanged();
        }

        private void SetCurrentSavedArticle(Article article)
        {
            CurrentSavedArticle = article;
            UpdateSavedArticleMoveStates();
            NotifyCurrentSavedArticleChanged(CurrentSavedArticle);
        }

        #endregion Private Methods

        private void InitializeLiveTile()
        {
            IsolatedStorageHelper.SaveSettingValueImmediately("LastNewArticlesCount", 0);
            var appTile = ShellTile.ActiveTiles.First();

            if (appTile != null)
            {
                appTile.Update(new StandardTileData { Count = 0 });
            }
        }

        private void UpdateAgent()
        {
            if (Settings != null && Settings.IsDownloadingArticlesOffline == true)
            {
                StartAgent();
            }
            else
            {
                StopAgentIfStarted();
            }
        }

        private static void StartAgent()
        {
            StopAgentIfStarted();

            try
            {
                var action = ScheduledActionService.Find(AgentName);
                if (action == null)
                {
                    ScheduledActionService.Add(new PeriodicTask(AgentName) { Description = "Periodically download articles in the background if the Internet connection is Wi-Fi or Ethernet." });
                }
#if DEBUG
                ScheduledActionService.LaunchForTest(AgentName, new TimeSpan(0, 0, 0, 3));
#endif
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("DataService:" + exception);
                }
            }
        }

        private static void StopAgentIfStarted()
        {
            if (ScheduledActionService.Find(AgentName) != null)
            {
                ScheduledActionService.Remove(AgentName);
            }
        }

        private void LoadCurrentDetails()
        {
            CurrentArticle = IsolatedStorageHelper.GetObject<Article>("CurrentArticle");
            if (CurrentArticle != null)
            {
                UpdateArticleMoveStates();
            }

            CurrentSavedArticle = IsolatedStorageHelper.GetObject<Article>("CurrentSavedArticle");
            if (CurrentSavedArticle != null)
            {
                UpdateSavedArticleMoveStates();
            }
        }

        private void LoadSettings()
        {
            Settings = IsolatedStorageHelper.GetObject<Settings>("Settings") ?? new Settings();
        }

        private void SaveSettings()
        {
            IsolatedStorageHelper.SaveObject<Settings>("Settings", Settings);
            if (CurrentArticle != null)
            {
                IsolatedStorageHelper.SaveObject<Article>("CurrentArticle", CurrentArticle);
            }

            if (CurrentSavedArticle != null)
            {
                IsolatedStorageHelper.SaveObject<Article>("CurrentSavedArticle", CurrentSavedArticle);
            }
        }

        public Article GetPreviousArticle()
        {
            try
            {
                if (CurrentArticle != null && CurrentArticle.Category == TopStories)
                {
                    if (TopStoriesArticles == null)
                        return null;

                    var index = TopStoriesArticles.FindIndex(o => o.ArticleId == CurrentArticle.ArticleId);
                    return index == 0
                        ? TopStoriesArticles[TopStoriesArticles.Count - 1]
                        : TopStoriesArticles[index - 1];
                }
                else if (CurrentArticle != null && CurrentArticle.Category == BreakingNews)
                {
                    if (BreakingNewsArticles == null)
                        return null;
                    var index = BreakingNewsArticles.FindIndex(o => o.ArticleId == CurrentArticle.ArticleId);
                    return index == 0 ? BreakingNewsArticles[BreakingNewsArticles.Count - 1] : BreakingNewsArticles[index - 1];
                }
                else
                {
                    if (Articles == null)
                        return null;
                    var index = Articles.FindIndex(o => o.ArticleId == CurrentArticle.ArticleId);
                    return index == 0 ? Articles[Articles.Count - 1] : Articles[index - 1];
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("DataService:" + exception);
                }
            }
            return null;
        }

        public Article GetNextArticle()
        {
            try
            {
                if (CurrentArticle != null && CurrentArticle.Category == TopStories)
                {
                    if (TopStoriesArticles == null)
                        return null;

                    var index = TopStoriesArticles.FindIndex(o => o.ArticleId == CurrentArticle.ArticleId);
                    return index == TopStoriesArticles.Count - 1 ? TopStoriesArticles[0] : TopStoriesArticles[index + 1];
                }
                else if (CurrentArticle != null && CurrentArticle.Category == BreakingNews)
                {
                    if (BreakingNewsArticles == null)
                        return null;
                    var index = BreakingNewsArticles.FindIndex(o => o.ArticleId == CurrentArticle.ArticleId);
                    return index == BreakingNewsArticles.Count - 1 ? BreakingNewsArticles[0] : BreakingNewsArticles[index + 1];
                }
                else
                {
                    if (Articles == null)
                        return null;
                    var index = Articles.FindIndex(o => o.ArticleId == CurrentArticle.ArticleId);
                    return index == Articles.Count - 1 ? Articles[0] : Articles[index + 1];
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("DataService:" + exception);
                }
            }
            return null;
        }

        private void UpdateArticleMoveStates()
        {
            try
            {
                CanSaveCurrentArticle = !IsArticleAlreadySaved(CurrentArticle);
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("DataService:" + exception);
                }
            }
        }

        private void UpdateSavedArticleMoveStates()
        {
            try
            {
                CanMoveToPreviousSavedArticle = false;
                CanMoveToNextSavedArticle = false;

                for (var articleIndex = 0; articleIndex < SavedArticles.Count; articleIndex++)
                {
                    if (SavedArticles[articleIndex].ArticleId == CurrentSavedArticle.ArticleId)
                    {
                        CanMoveToPreviousSavedArticle = articleIndex > 0;
                        CanMoveToNextSavedArticle = articleIndex + 1 != SavedArticles.Count;

                        break;
                    }
                }

                CanSaveCurrentArticle = !IsArticleAlreadySaved(CurrentSavedArticle);
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("DataService:" + exception);
                }
            }
        }

        #region Notify Events

        private void NotifyDataRefreshed(DataType dataType, bool isSuccess)
        {
            var dataRefreshedEventHandler = OnDataRefreshed;

            if (dataRefreshedEventHandler != null)
            {
                dataRefreshedEventHandler(this, new DataRefreshedEventArgs(dataType, isSuccess));
            }
        }

        private void NotifyCurrentArticleChanged(Article article)
        {
            var articleChangedEventHandler = OnArticleChanged;

            if (articleChangedEventHandler != null)
            {
                articleChangedEventHandler(this, new ArticleChangedEventArgs(article));
            }
        }

        private void NotifyPreviousArticleChanged(Article article)
        {
            var articleChangedEventHandler = OnPreviousArticleChanged;

            if (articleChangedEventHandler != null)
            {
                articleChangedEventHandler(this, new ArticleChangedEventArgs(article));
            }
        }

        private void NotifyNextArticleChanged(Article article)
        {
            var articleChangedEventHandler = OnNextArticleChanged;

            if (articleChangedEventHandler != null)
            {
                articleChangedEventHandler(this, new ArticleChangedEventArgs(article));
            }
        }

        private void NotifyCurrentSavedArticleChanged(Article article)
        {
            var articleChangedEventHandler = OnSavedArticleChanged;

            if (articleChangedEventHandler != null)
            {
                articleChangedEventHandler(this, new ArticleChangedEventArgs(article));
            }
        }

        private void NotifySavedArticlesChanged()
        {
            var savedArticlesChangedEventHandler = OnSavedArticlesChanged;

            if (savedArticlesChangedEventHandler != null)
            {
                savedArticlesChangedEventHandler(this, EventArgs.Empty);
            }
        }

        private async Task UpdateStories(Article article)
        {
            try
            {
                if (article != null && HeadLineArticle != null && article.ArticleId == HeadLineArticle.ArticleId)
                {
                    HeadLineArticle = article;
                    SaveHeadLine();
                }

                var temp = TopStoriesArticles.Find(o => article != null && o.ArticleId == article.ArticleId);
                if (temp != null)
                {
                    temp = article;
                    await SaveTopStoriesArticle(temp);
                }

                temp = null;
                temp = BreakingNewsArticles.Find(o => article != null && o.ArticleId == article.ArticleId);
                if (temp != null)
                {
                    temp = article;
                    await SaveBreakingNewsArticleAsync(temp);
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("DataService:" + exception);
                }
            }
        }

        #endregion Notify Events
    }
}