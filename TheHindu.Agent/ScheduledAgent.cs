using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using TheHindu.Client;
using TheHindu.Model;
using Utilities;

namespace TheHindu.Agent
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        #region Constants

        private const int ExpairyDays = 2;
        private const string LastUpdateVerificationPropertyName = "LastUpdateVerification";
        private const string TopStories = "Top Stories";

        #endregion Constants

        #region Fields

        private static volatile bool _classInitialized;
        private TheHinduClient _theHinduClient;
        private DateTime _lastCheck;

        #endregion Fields

        #region Constructor

        public ScheduledAgent()
        {
            if (!_classInitialized)
            {
                _classInitialized = true;

                Deployment.Current.Dispatcher.BeginInvoke(delegate { Application.Current.UnhandledException += ScheduledAgent_UnhandledException; });
            }
        }

        #endregion Constructor

        #region Event Handlers

        protected override async void OnInvoke(ScheduledTask task)
        {
            try
            {
                _lastCheck = IsolatedStorageHelper.GetDateTime(LastUpdateVerificationPropertyName, new DateTime(1, 1, 1));
                if (_lastCheck != null)
                {
                    if (_lastCheck.Year != 1)
                    {
                        if (IsUpdateRequired())
                        {
                            await UpdateAsync();
                        }
                        else
                        {
                            NotifyComplete();
                        }
                    }
                    else
                    {
                        SaveSettingValueImmediately(LastUpdateVerificationPropertyName, DateTime.Now);
                        NotifyComplete();
                    }
                }
                else
                {
                    SaveSettingValueImmediately(LastUpdateVerificationPropertyName, DateTime.Now);
                    NotifyComplete();
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("ScheduledAgent:" + exception);
                }
                NotifyComplete();
            }
        }

        private void ScheduledAgent_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            //if (System.Diagnostics.Debugger.IsAttached)
            //{
            //    System.Diagnostics.Debugger.Break();
            //}
            if (e != null)
                e.Handled = true;
        }

        #endregion Event Handlers

        #region Private Methods

        private bool IsUpdateRequired()
        {
            DateTime now = DateTime.Now;
            return (now - _lastCheck).TotalMinutes >= 30;
        }

        private async Task UpdateAsync()
        {
            try
            {
                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    SaveSettingValueImmediately(LastUpdateVerificationPropertyName, DateTime.Now);
                    await UpdateInBackGroundAsync();
                }
                else
                {
                    NotifyComplete();
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("ScheduledAgent:" + exception);
                }
                NotifyComplete();
            }
        }

        private async Task UpdateInBackGroundAsync()
        {
            try
            {
                _theHinduClient = new TheHinduClient();
                var topNews = await _theHinduClient.GetTopArticlesAsync();

                if (topNews != null && topNews.Count > 0)
                {
                    topNews = topNews.OrderByDescending(o => o.PublishDate).ToList();
                    List<Article> savedArticles = await LoadTopStoriesArticlesAsync();
                    uint newArticlesCount = 0;
                    Article newArticle = null;
                    if (savedArticles != null)
                    {
                        foreach (Article article in topNews)
                        {
                            bool isNew = savedArticles.All(savedArticle => article.ArticleId != savedArticle.ArticleId);
                            if (isNew)
                            {
                                if (newArticle == null && !string.IsNullOrEmpty(article.Thumbnail))
                                {
                                    newArticle = article;
                                }
                                newArticlesCount++;
                                savedArticles.Add(article);
                            }
                        }
                    }
                    else
                    {
                        savedArticles = new List<Article>();
                        foreach (Article article in topNews)
                        {
                            if (newArticle == null && !string.IsNullOrEmpty(article.Thumbnail))
                            {
                                newArticle = article;
                            }
                            newArticlesCount++;
                            savedArticles.Add(article);
                        }
                    }

                    savedArticles.ForEach(o => o.Category = TopStories);
                    if (savedArticles.Count > 0)
                    {
                        await SaveTopStoriesArticlesAsync(savedArticles);
                    }

                    if (newArticlesCount > 0 && newArticle != null)
                    {
                        var lastNotifiedArticle = IsolatedStorageHelper.GetObject<string>("LastNotifiedArticle");
                        if (newArticle.Title != lastNotifiedArticle)
                        {
                            await UpdateNotificationsAsync((int)newArticlesCount, newArticle);
                            IsolatedStorageHelper.SaveObject<string>("LastNotifiedArticle", newArticle.Title);
                        }
                    }

                    NotifyComplete();
                }
                else
                {
                    NotifyComplete();
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("ScheduledAgent:" + exception);
                }
                NotifyComplete();
            }
        }

        private async Task UpdateNotificationsAsync(int newArticlesCount, Article newArticle)
        {
            if (newArticle != null)
            {
                Settings settings = IsolatedStorageHelper.GetObject<Settings>("Settings") ?? new Settings();
                newArticlesCount = IsolatedStorageHelper.GetInt("LastNewArticlesCount", 0) + newArticlesCount;

                if (settings != null && settings.IsToastNotificationUsed == true)
                {
                    bool canNotify = true;
                    if (settings.IsQuietHoursUsed == true)
                    {
                        if ((settings.QuietHoursStartTime.TimeOfDay < DateTime.Now.TimeOfDay) && (DateTime.Now.TimeOfDay < settings.QuietHoursEndTime.TimeOfDay))
                        {
                            canNotify = false;
                        }
                    }

                    if (canNotify)
                    {
                        try
                        {
                            ShellToast shellToast = new ShellToast { Content = newArticle.Title, NavigationUri = new Uri(string.Format("/Views/MainPage.xaml?Id={0}&Category={1}", newArticle.ArticleId, TopStories), UriKind.Relative), Title = "The Hindu" };
                            //if (IsTargetedVersion)
                            //{
                            //    SetProperty(shellToast, "Sound", new Uri(@"\Sounds\MyToastSound.mp3", UriKind.RelativeOrAbsolute));
                            //}

                            shellToast.Show();
                        }
                        catch (Exception exception)
                        {
                            if (Debugger.IsAttached)
                            {
                                Debug.WriteLine("ScheduledAgent:" + exception);
                            }
                        }
                    }
                }

                if (settings != null && settings.IsLiveTileSupport == true)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(newArticle.Thumbnail))
                            await DownLoadLiveTileAndShowAsync(newArticle);
                    }
                    catch (Exception exception)
                    {
                        if (Debugger.IsAttached)
                        {
                            Debug.WriteLine("ScheduledAgent:" + exception);
                        }
                    }
                }
                else
                {
                    try
                    {
                        ShellTile appTile = ShellTile.ActiveTiles.First();
                        if (appTile != null)
                            appTile.Delete();
                    }
                    catch (Exception exception)
                    {
                        if (Debugger.IsAttached)
                        {
                            Debug.WriteLine("ScheduledAgent:" + exception);
                        }
                    }
                }

                SaveSettingValueImmediately("LastNewArticlesCount", newArticlesCount);
                NotifyComplete();
            }
        }

        private static Version _targetVersion = new Version(8, 0, 10492);

        private static bool IsTargetedVersion { get { return Environment.OSVersion.Version >= _targetVersion; } }

        private static void SetProperty(object instance, string name, object value)
        {
            var setMethod = instance.GetType().GetProperty(name).GetSetMethod();
            setMethod.Invoke(instance, new object[] { value });
        }

        private async Task DownLoadLiveTileAndShowAsync(Article article)
        {
            try
            {
                string liveTileImagePath = "Shared/ShellContent/" + "liveTile.jpg";

                var imageStream = await GetImageStreamAsync(article.Thumbnail);
                if (imageStream != null)
                {
                    var userStoreForApplication = IsolatedStorageFile.GetUserStoreForApplication();
                    if (userStoreForApplication.FileExists(liveTileImagePath))
                    {
                        userStoreForApplication.DeleteFile(liveTileImagePath);
                    }

                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        try
                        {
                            var isolatedStorageFileStream = userStoreForApplication.CreateFile(liveTileImagePath);
                            BitmapImage bitmapImage = new BitmapImage();
                            bitmapImage.CreateOptions = BitmapCreateOptions.None;
                            bitmapImage.SetSource(imageStream);
                            var writeableBitmap = new WriteableBitmap(bitmapImage);
                            writeableBitmap.SaveJpeg(isolatedStorageFileStream, writeableBitmap.PixelWidth, writeableBitmap.PixelHeight, 0, 85);
                            isolatedStorageFileStream.Close();
                        }
                        catch (Exception exception)
                        {
                            if (Debugger.IsAttached)
                            {
                                Debug.WriteLine("ScheduledAgent:" + exception);
                            }
                        }

                        try
                        {
                            var uri = new Uri(string.Format("isostore:/{0}", liveTileImagePath), UriKind.Absolute);
                            if (userStoreForApplication.FileExists(liveTileImagePath))
                            {
                                ShellTile appTile = ShellTile.ActiveTiles.First();
                                if (appTile != null)
                                {
                                    appTile.Update(new FlipTileData
                                    {
                                        BackContent = article.Title,
                                        WideBackContent = article.Title,
                                        BackgroundImage = uri,
                                        WideBackgroundImage = uri,
                                        BackTitle = "The Hindu",
                                        Title = "The Hindu"
                                    });
                                    NotifyComplete();
                                }
                            }
                        }
                        catch (Exception exception)
                        {
                            if (Debugger.IsAttached)
                            {
                                Debug.WriteLine("ScheduledAgent:" + exception);
                            }
                            NotifyComplete();
                        }
                    });
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("ScheduledAgent:" + exception);
                }
            }
        }

        private async Task<Stream> GetImageStreamAsync(string imageUrl)
        {
            HttpClient client = new HttpClient();
            var responseMessage = await client.GetAsync(imageUrl, HttpCompletionOption.ResponseContentRead);
            return await responseMessage.Content.ReadAsStreamAsync();
        }

        private void UpdateLiveTile(Article newArticle)
        {
            try
            {
                if (newArticle != null)
                {
                    ShellTile appTile = ShellTile.ActiveTiles.First();
                    if (appTile != null)
                    {
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
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("ScheduledAgent:" + exception);
                }
            }
        }

        private static void SaveSettingValueImmediately(string propertyName, object value)
        {
            try
            {
                IsolatedStorageHelper.SaveSettingValueImmediately(propertyName, value);
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("ScheduledAgent:" + exception);
                }
            }
        }

        private async Task<List<Article>> LoadTopStoriesArticlesAsync()
        {
            List<Article> articles = new List<Article>();
            try
            {
                articles = await DatabaseOperations.GetInstance().GetCategoryArticlesAsync(TopStories);
                if (articles != null && articles.Count > 0)
                {
                    articles = articles.OrderByDescending(o => o.PublishDate).ToList();
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("ScheduledAgent:" + exception);
                }
            }

            return articles;
        }

        private async Task SaveTopStoriesArticlesAsync(List<Article> articles)
        {
            try
            {
                foreach (var article in articles)
                {
                    await DatabaseOperations.GetInstance().AddOrUpdateArticleAsync(article);
                }
                await DatabaseOperations.GetInstance().UpdateCategoryLastAccessedValueAsync(TopStories, DateTime.UtcNow);
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("ScheduledAgent:" + exception);
                }
            }
        }

        #endregion Private Methods
    }
}