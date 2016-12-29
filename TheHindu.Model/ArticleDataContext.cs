using System.Data.Linq;

namespace TheHindu.Model
{
    public class ArticleDataContext : DataContext
    {
        public static string DbConnectionString = @"isostore:/Databases.sdf";

        public ArticleDataContext(string dbConnectionString)
            : base(dbConnectionString)
        {
        }

        public Table<Article> ArticlesFromTable
        {
            get
            {
                return this.GetTable<Article>();
            }
        }
    }
}