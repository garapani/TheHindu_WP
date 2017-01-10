using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace TheHindu.Model
{
    public static class Util
    {
        public static IEnumerable<string> Splice(this string s, int spliceLength)
        {
            if (s == null)
                throw new ArgumentNullException("s");
            if (spliceLength < 1)
                throw new ArgumentOutOfRangeException("spliceLength");

            if (s.Length == 0)
                yield break;
            var start = 0;
            for (var end = spliceLength; end < s.Length; end += spliceLength)
            {
                yield return s.Substring(start, spliceLength);
                start = end;
            }
            yield return s.Substring(start);
        }
    }

    public class DatabaseOperations
    {
        private static DatabaseOperations _instance = null;
        private object _articleDbLock;
        private object _cateogryDbLock;
        private object _slideDbLock;
        private SQLite.SQLiteAsyncConnection _dbConnection;

        private DatabaseOperations()
        {
            _articleDbLock = new object();
            _cateogryDbLock = new object();
            _slideDbLock = new object();

            _dbConnection = new SQLite.SQLiteAsyncConnection(Path.Combine(ApplicationData.Current.LocalFolder.Path, "articlesDb.sqlite"));
        }

        public static DatabaseOperations GetInstance()
        {
            if (_instance == null)
            {
                _instance = new DatabaseOperations();
                _instance.CreateDatabaseIfNotCreated();
            }
            return _instance;
        }

        public async Task AddCategoryAsync(CategoryDetailsModel categoryDetails)
        {
            if (_dbConnection != null)
            {
                try
                {
                    var allCategories = from category in _dbConnection.Table<CategoryDetailsModel>() where category.CategoryName == categoryDetails.CategoryName select category;
                    if (allCategories != null && await allCategories.CountAsync() >= 1)
                    {
                        await _dbConnection.UpdateAsync(categoryDetails);
                    }
                    else
                    {
                        await _dbConnection.InsertAsync(categoryDetails);
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                }
            }
            else
            {
                Debug.WriteLine("failed to get sqlite connection");
            }
        }

        public async Task AddOrUpdateArticleAsync(Article articleToAddorUpdate)
        {
            if (_dbConnection != null)
            {
                try
                {
                    var allArticles = from article in _dbConnection.Table<Article>() where article.ArticleId == articleToAddorUpdate.ArticleId select article;
                    if (allArticles != null && await allArticles.CountAsync() >= 1)
                    {
                        await _dbConnection.UpdateAsync(articleToAddorUpdate);
                    }
                    else
                    {
                        await _dbConnection.InsertAsync(articleToAddorUpdate);
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                }
            }
            else
            {
                Debug.WriteLine("failed to get sqlite connection");
            }
        }

        public async Task AddSlideShowAsync(SlideShowDetails slideShowDetails)
        {
            if (_dbConnection != null)
            {
                try
                {
                    var allSlides = from slideShow in _dbConnection.Table<SlideShowDetails>() where slideShow.Id == slideShowDetails.Id select slideShow;
                    if (allSlides != null && await allSlides.CountAsync() >= 1)
                    {
                        await _dbConnection.UpdateAsync(slideShowDetails);
                    }
                    else
                    {
                        await _dbConnection.InsertAsync(slideShowDetails);
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                }
            }
            else
            {
                Debug.WriteLine("failed to get sqlite connection");
            }
        }

        public async Task AddSubSlideShowAsync(string slideShowId, Slidedetail subSlide)
        {
            if (_dbConnection != null)
            {
                try
                {
                    var allSlides = from slideShow in _dbConnection.Table<SlideShowDetails>() where slideShow.Id == slideShowId select slideShow;
                    if (allSlides != null && await allSlides.CountAsync() >= 1)
                    {
                        SlideShowDetails tempArticle = await allSlides.FirstOrDefaultAsync();
                        if (tempArticle.SlideDetails.Find(o => o.ImageUrl == subSlide.ImageUrl) == null)
                        {
                            tempArticle.SlideDetails.Add(subSlide);
                        }
                        else
                        {
                            tempArticle.SlideDetails.Find(o => o.ImageUrl == subSlide.ImageUrl).ImageDescription = subSlide.ImageDescription;
                        }
                        await _dbConnection.UpdateAsync(tempArticle);
                    }
                    else
                    {
                        //await _dbConnection.InsertAsync(slideShowDetails);
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                }
            }
            else
            {
                Debug.WriteLine("failed to get sqlite connection");
            }
        }

        public async Task DeleteAllArticlesAsync()
        {
            if (_dbConnection != null)
            {
                try
                {
                    var allArticles = await _dbConnection.Table<Article>().ToListAsync();
                    foreach (var article in allArticles)
                    {
                        await _dbConnection.DeleteAsync(article);
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                }
            }
            else
            {
                Debug.WriteLine("failed to get sqlite connection");
            }
        }

        public async Task DeleteArticleAsync(string Id, string category)
        {
            if (_dbConnection != null)
            {
                try
                {
                    var allArticles = from article in _dbConnection.Table<Article>() where article.ArticleId == Id select article;
                    if (allArticles != null && await allArticles.CountAsync() >= 1)
                    {
                        var temp = await allArticles.FirstOrDefaultAsync();
                        await _dbConnection.DeleteAsync(temp);
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                }
            }
            else
            {
                Debug.WriteLine("failed to get sqlite connection");
            }
        }

        public async Task DeleteArticlesAsync(string category)
        {
            if (_dbConnection != null)
            {
                try
                {
                    var allArticles = from article in _dbConnection.Table<Article>() where article.Category == category select article;
                    foreach (var article in await allArticles.ToListAsync())
                    {
                        await _dbConnection.DeleteAsync(article);
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                }
            }
            else
            {
                Debug.WriteLine("failed to get sqlite connection");
            }
        }

        public async Task DeleteCategoryAsync(string categoryName)
        {
            if (_dbConnection != null)
            {
                try
                {
                    var allcategories = from category in _dbConnection.Table<CategoryDetailsModel>() where category.CategoryName.ToLower() == categoryName.ToLower() select category;
                    if (allcategories != null)
                    {
                        foreach (var category in await allcategories.ToListAsync())
                        {
                            await _dbConnection.DeleteAsync(category);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                }
            }
            else
            {
                Debug.WriteLine("failed to get sqlite connection");
            }
        }

        public async Task DeleteExpiredArticlesAsync()
        {
            if (_dbConnection != null)
            {
                try
                {
                    var allArticles = await _dbConnection.Table<Article>().ToListAsync();
                    foreach (var article in allArticles)
                    {
                        if ((DateTime.UtcNow - article.PublishDate).TotalHours >= 24 && article.IsSaved == false)
                        {
                            await _dbConnection.DeleteAsync(article);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                }
            }
            else
            {
                Debug.WriteLine("failed to get sqlite connection");
            }
        }

        public async Task DeleteSlideShowAsync(SlideShowDetails slideShowDetails)
        {
            if (_dbConnection != null)
            {
                try
                {
                    var allSlides = from slide in _dbConnection.Table<SlideShowDetails>() where slide.Id == slideShowDetails.Id select slide;
                    foreach (var slide in await allSlides.ToListAsync())
                    {
                        await _dbConnection.DeleteAsync(slide);
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                }
            }
            else
            {
                Debug.WriteLine("failed to get sqlite connection");
            }
        }

        public async Task<Article> GetArticleAsync(string Id, string category = null)
        {
            Article tempArticle = null;
            if (_dbConnection != null)
            {
                try
                {
                    var allArticles = from article in _dbConnection.Table<Article>() where article.ArticleId == Id && article.Category == category select article;
                    tempArticle = await allArticles.FirstOrDefaultAsync();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                }
            }
            else
            {
                Debug.WriteLine("failed to get sqlite connection");
            }
            return tempArticle;
        }

        public async Task<List<CategoryDetailsModel>> GetCategoriesAsync()
        {
            List<CategoryDetailsModel> list = null;
            if (_dbConnection != null)
            {
                try
                {
                    list = await _dbConnection.Table<CategoryDetailsModel>().ToListAsync();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                }
            }
            else
            {
                Debug.WriteLine("failed to get sqlite connection");
            }
            return list;
        }

        public async Task<List<Article>> GetCategoryArticlesAsync(string category)
        {
            List<Article> list = null;
            if (_dbConnection != null)
            {
                try
                {
                    var allArticles = from article in _dbConnection.Table<Article>() where article.Category == category select article;
                    if (allArticles != null)
                    {
                        list = await allArticles.ToListAsync();
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                }
            }
            else
            {
                Debug.WriteLine("failed to get sqlite connection");
            }
            return list;
        }

        public async Task<CategoryDetailsModel> GetCategoryDetailAsync(string categoryName)
        {
            CategoryDetailsModel categoryObj = null;
            if (_dbConnection != null)
            {
                try
                {
                    var allCategories = from category in _dbConnection.Table<CategoryDetailsModel>() where category.CategoryName == categoryName select category;
                    if (allCategories != null)
                    {
                        categoryObj = await allCategories.FirstOrDefaultAsync();
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                }
            }
            else
            {
                Debug.WriteLine("failed to get sqlite connection");
            }
            return categoryObj;
        }

        public async Task<List<CategoryDetailsModel>> GetFavCategoriesAsync()
        {
            List<CategoryDetailsModel> list = null;
            if (_dbConnection != null)
            {
                try
                {
                    var allCategories = from category in _dbConnection.Table<CategoryDetailsModel>() where category.IsPinned == true select category;
                    if (allCategories != null)
                    {
                        list = await allCategories.ToListAsync();
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                }
            }
            else
            {
                Debug.WriteLine("failed to get sqlite connection");
            }
            return list;
        }

        public async Task<IList<Article>> GetSavedArticlesAsync()
        {
            IList<Article> list = null;
            if (_dbConnection != null)
            {
                try
                {
                    var allArticles = from article in _dbConnection.Table<Article>() where article.IsSaved == true select article;
                    if (allArticles != null)
                    {
                        list = await allArticles.ToListAsync();
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                }
            }
            else
            {
                Debug.WriteLine("failed to get sqlite connection");
            }
            return list;
        }

        public bool IsCategoriesDbAlreadyCreated()
        {
            if (_dbConnection != null)
            {
                var temp = _dbConnection.Table<CategoryDetailsModel>();
                if (temp != null)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task UpdateCategoryAsync(CategoryDetailsModel categoryModel)
        {
            if (_dbConnection != null)
            {
                try
                {
                    var allCategories = from category in _dbConnection.Table<CategoryDetailsModel>() where category.CategoryName.ToLower() == categoryModel.CategoryName.ToLower() select category;
                    if (allCategories != null)
                    {
                        CategoryDetailsModel tempCategory = await allCategories.FirstOrDefaultAsync();
                        if (tempCategory != null)
                        {
                            tempCategory.CategoryUrl = categoryModel.CategoryUrl;
                            tempCategory.IsGroup = categoryModel.IsGroup;
                            tempCategory.Order = categoryModel.Order;
                            tempCategory.ParentCategory = categoryModel.ParentCategory;
                            tempCategory.IsWorking = categoryModel.IsWorking;
                            await _dbConnection.UpdateAsync(tempCategory);
                        }
                        else
                        {
                            await _dbConnection.InsertAsync(categoryModel);
                        }
                    }
                    else
                    {
                        await _dbConnection.InsertAsync(categoryModel);
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                }
            }
            else
            {
                Debug.WriteLine("failed to get db connection");
            }
        }

        public async Task UpdateCategoryLastAccessedValueAsync(string categoryName, DateTime lastFetchedDateTime)
        {
            if (_dbConnection != null)
            {
                try
                {
                    var allCategories = from category in _dbConnection.Table<CategoryDetailsModel>() where category.CategoryName == categoryName select category;
                    if (allCategories != null)
                    {
                        CategoryDetailsModel tempCategory = await allCategories.FirstOrDefaultAsync();
                        if (tempCategory != null)
                        {
                            tempCategory.LastFetchedTime = lastFetchedDateTime;
                            await _dbConnection.UpdateAsync(tempCategory);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                }
            }
            else
            {
                Debug.WriteLine("failed to get db connection");
            }
        }

        public async Task UpdateCategoryPinValueAsync(string categoryName, bool isPinned)
        {
            if (_dbConnection != null)
            {
                try
                {
                    var allCategories = from category in _dbConnection.Table<CategoryDetailsModel>() where category.CategoryName == categoryName select category;
                    if (allCategories != null)
                    {
                        CategoryDetailsModel tempCategory = await allCategories.FirstOrDefaultAsync();
                        tempCategory.IsPinned = isPinned;
                        await _dbConnection.UpdateAsync(tempCategory);
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                }
            }
            else
            {
                Debug.WriteLine("failed to get db connection");
            }
        }

        private void CreateDatabaseIfNotCreated()
        {
            try
            {
                _dbConnection.CreateTableAsync<Article>();
                _dbConnection.CreateTableAsync<CategoryDetailsModel>();
            }
            catch (Exception)
            {
            }
        }
    }
}