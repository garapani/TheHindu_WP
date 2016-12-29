using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TheHindu.Model;

namespace TheHindu.Client
{
    public class TheHinduClient
    {
        #region Methods

        public async Task<List<Article>> GetArticlesAsync(string category, string categoryUrl)
        {
            if (string.IsNullOrEmpty(category) || string.IsNullOrEmpty(categoryUrl))
                return null;
            var handler = new HttpClientHandler();
            if (handler.SupportsAutomaticDecompression)
            {
                handler.AutomaticDecompression = DecompressionMethods.GZip |
                                                 DecompressionMethods.Deflate;
            }
            try
            {
                var httpClient = new HttpClient(handler);
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (iPhone; CPU iPhone OS 8_0 like Mac OS X) AppleWebKit/600.1.3 (KHTML, like Gecko) Version/8.0 Mobile/12A4345d Safari/600.1.4");
                var tempString = await httpClient.GetStringAsync(categoryUrl);

                if (categoryUrl.ToLower().Contains("temp-th.appspot"))
                {
                    return ParseArticlesAsync(tempString, true);
                }
                else
                {
                    return ParseArticlesAsync(tempString, false);
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("WinSourceClient:" + exception);
                }
            }
            return null;
        }

        public async Task<List<CategoryDetailsModel>> GetCategoryDetailsAsync()
        {
            var handler = new HttpClientHandler();
            if (handler.SupportsAutomaticDecompression)
            {
                handler.AutomaticDecompression = DecompressionMethods.GZip |
                                                 DecompressionMethods.Deflate;
            }
            try
            {
                var url = "http://thehindu.thevillagesoftware.com/api/Categories";
                var httpClient = new HttpClient(handler);
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (iPhone; CPU iPhone OS 8_0 like Mac OS X) AppleWebKit/600.1.3 (KHTML, like Gecko) Version/8.0 Mobile/12A4345d Safari/600.1.4");
                var tempString = await httpClient.GetStringAsync(url);
                if (!string.IsNullOrEmpty(tempString))
                {
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        DateFormatHandling = DateFormatHandling.IsoDateFormat,
                        DateTimeZoneHandling = DateTimeZoneHandling.Utc
                    };

                    return JsonConvert.DeserializeObject<List<CategoryDetailsModel>>(tempString, settings);
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("WinSourceClient:" + exception);
                }
            }
            return null;
        }

        public async Task<List<Article>> GetTopArticlesAsync(int page = 0)
        {
            var categoryUrl = string.Format("http://thehindu.thevillagesoftware.com/api/Articles?categoryName=Top Stories&page={0}", page);
            var handler = new HttpClientHandler();
            if (handler.SupportsAutomaticDecompression)
            {
                handler.AutomaticDecompression = DecompressionMethods.GZip |
                                                 DecompressionMethods.Deflate;
            }

            try
            {
                var httpClient = new HttpClient(handler);
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (iPhone; CPU iPhone OS 8_0 like Mac OS X) AppleWebKit/600.1.3 (KHTML, like Gecko) Version/8.0 Mobile/12A4345d Safari/600.1.4");
                var tempString = await httpClient.GetStringAsync(categoryUrl);
                return ParseArticlesAsync(tempString, false);
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("WinSourceClient:" + exception);
                }
            }
            return null;
        }

        public async Task<List<Article>> GetBreakingNewsArticlesAsync(int page = 0)
        {
            var categoryUrl = string.Format("http://thehindu.thevillagesoftware.com/api/Articles?categoryName=Breaking News&page={0}", page);
            var handler = new HttpClientHandler();
            if (handler.SupportsAutomaticDecompression)
            {
                handler.AutomaticDecompression = DecompressionMethods.GZip |
                                                 DecompressionMethods.Deflate;
            }

            try
            {
                var httpClient = new HttpClient(handler);
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (iPhone; CPU iPhone OS 8_0 like Mac OS X) AppleWebKit/600.1.3 (KHTML, like Gecko) Version/8.0 Mobile/12A4345d Safari/600.1.4");
                var tempString = await httpClient.GetStringAsync(categoryUrl);
                return ParseArticlesAsync(tempString, false);
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("WinSourceClient:" + exception);
                }
            }
            return null;
        }

        private List<Article> ParseArticlesAsync(string articlesString, bool http = true)
        {
            var listOfArticles = new List<Article>();
            if (http == true)
            {
                try
                {
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        DateFormatHandling = DateFormatHandling.IsoDateFormat,
                        DateTimeZoneHandling = DateTimeZoneHandling.Utc
                    };

                    var newArticles = JsonConvert.DeserializeObject<RootObject>(articlesString, settings);
                    if (newArticles != null && newArticles.NewsItem != null && newArticles.NewsItem.Count > 0)
                    {
                        var temp = from article in newArticles.NewsItem
                                   select new Article(article.WebUrl, HttpUtility.HtmlDecode(article.HeadLine).Replace("&nbsp;", ""), HttpUtility.HtmlDecode(article.Story).Replace("&nbsp;", ""), article.ArticleDate, article.ByLine, !string.IsNullOrEmpty(article.Photo) && article.Photo.Contains("g.jpg") ? article.Photo.Replace("g.jpg", "c.jpg") : article.Photo, !string.IsNullOrEmpty(article.Photo) && article.Photo.Contains("g.jpg") ? article.Photo.Replace("g.jpg", "e.jpg") : article.Photo, article.PhotoCaption, article.Story);
                        listOfArticles = temp.ToList<Article>();
                    }
                }
                catch (Exception exception)
                {
                    if (Debugger.IsAttached)
                    {
                        Debug.WriteLine("WinSourceClient:" + exception);
                    }
                }
            }
            else
            {
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                };

                var newArticles = JsonConvert.DeserializeObject<List<Article>>(articlesString, settings);
                if (newArticles != null)
                {
                    newArticles.ForEach(o =>
                    {
                        o.Author = o.Author.Replace("\n", string.Empty);
                        o.HdThumbnail = string.IsNullOrEmpty(o.Thumbnail) ? string.Empty : o.Thumbnail.Replace("i.jpg", "g.jpg");
                    });
                    listOfArticles = newArticles;
                }
            }
            return listOfArticles;
        }

        public async Task<SlideShowDetails> GetSlideShowDetailsAsync(string id)
        {
            var slideShowDetails = new SlideShowDetails();
            slideShowDetails.Id = id;

            var articleDescription = await GetArticleDescriptionAsync(id);
            if (!string.IsNullOrEmpty(articleDescription))
            {
                var document = new HtmlDocument();
                document.LoadHtml(articleDescription);

                try
                {
                    var pageNumberNode = document.DocumentNode.SelectSingleNode("//div[@class='wx-articlePageNavBar-pageNumber']");
                    if (pageNumberNode != null)
                    {
                        if (!string.IsNullOrEmpty(pageNumberNode.InnerText))
                        {
                            string[] delimiterChars = { "/" };
                            var numbers = pageNumberNode.InnerText.Split(delimiterChars, StringSplitOptions.None);
                            if (numbers.Length == 1)
                            {
                                slideShowDetails.Count = int.Parse(numbers[0]);
                            }
                            else if (numbers.Length == 2)
                            {
                                slideShowDetails.Count = int.Parse(numbers[1]);
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    if (Debugger.IsAttached)
                    {
                        Debug.WriteLine("WinSourceClient:" + exception);
                    }
                }
                try
                {
                    var nextImageNode = document.DocumentNode.SelectSingleNode("//a[@class='nextbox']");
                    if (nextImageNode != null)
                    {
                        var url = "http://m.thehindu.com" + nextImageNode.Attributes.AsQueryable().First(o => o.Name.ToLower() == "href").Value;
                        if (url.ToLower().Contains("?image="))
                        {
                            slideShowDetails.Url = url.Substring(0, url.ToLower().IndexOf("?image="));
                        }
                        else
                        {
                            slideShowDetails.Url = url;
                        }
                    }
                }
                catch (Exception exception)
                {
                    if (Debugger.IsAttached)
                    {
                        Debug.WriteLine("WinSourceClient:" + exception);
                    }
                }
            }
            return slideShowDetails;
        }

        public async Task<Article> GetCurrentArticleAsync(Article article)
        {
            try
            {
                var description = await GetArticleDescriptionAsync(string.Format("http://thehindu.thevillagesoftware.com/Api/articles?id={0}&category={1}", article.ArticleId, article.Category));
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                };

                article = JsonConvert.DeserializeObject<Article>(description, settings);
                if (article != null)
                {
                    await DatabaseOperations.GetInstance().AddOrUpdateArticleAsync(article);
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("WinSourceClient:" + exception);
                }
            }
            return article;
        }

        private void ParseDescription(string pageContent, ref string articledescription, ref string thumbnailDescription, ref string author, ref string publishdateDescription)
        {
            try
            {
                var htmldocument = new HtmlDocument();
                htmldocument.LoadHtml(pageContent);
                if (htmldocument.DocumentNode != null)
                {
                    var maincontentNode = htmldocument.DocumentNode.SelectSingleNode("//div[@class='wx-articleDisplay-text']");

                    if (maincontentNode != null)
                    {
                        articledescription = HttpUtility.HtmlDecode(maincontentNode.InnerHtml);
                    }

                    var authorNode = htmldocument.DocumentNode.SelectSingleNode("//div[@class='wx-articleDisplay-meta']");

                    if (authorNode != null)
                    {
                        author = HttpUtility.HtmlDecode(authorNode.InnerText);
                        if (!string.IsNullOrEmpty(author))
                        {
                            var authors = author.Split('|');
                            if (authors.Length > 0)
                            {
                                author = authors[0];
                                if (!string.IsNullOrEmpty(author))
                                {
                                    if (author.Contains("Updated:"))
                                    {
                                        author = author.Replace("Updated:", "");
                                    }
                                }
                            }
                        }
                    }

                    author = author.Trim();
                    maincontentNode = htmldocument.DocumentNode.SelectSingleNode("//div[@class='wx-articleDisplay-imageCaption']");

                    if (maincontentNode != null)
                    {
                        thumbnailDescription = HttpUtility.HtmlDecode(maincontentNode.InnerText);
                    }
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("WinSourceClient:" + exception);
                }
            }
        }

        private async Task<string> GetArticleDescriptionAsync(string url)
        {
            try
            {
                var handler = new HttpClientHandler();
                if (handler.SupportsAutomaticDecompression)
                {
                    handler.AutomaticDecompression = DecompressionMethods.GZip |
                                                     DecompressionMethods.Deflate;
                }
                var httpClient = new HttpClient(handler);
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (iPhone; CPU iPhone OS 8_0 like Mac OS X) AppleWebKit/600.1.3 (KHTML, like Gecko) Version/8.0 Mobile/12A4345d Safari/600.1.4");
                return await httpClient.GetStringAsync(url);
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("WinSourceClient:" + exception);
                }
            }
            return null;
        }

        #endregion Methods
    }
}