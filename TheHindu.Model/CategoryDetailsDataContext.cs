using System.Data.Linq;

namespace TheHindu.Model
{
    public class CategoryDetailsDataContext : DataContext
    {
        public static string DbConnectionString = @"isostore:/CategoryDB.sdf";

        public CategoryDetailsDataContext(string dbConnectionString)
            : base(dbConnectionString)
        {
        }

        public Table<CategoryDetailsModel> CategoryDetailsFromTable
        {
            get
            {
                return this.GetTable<CategoryDetailsModel>();
            }
        }
    }
}