using System.Data.Linq;

namespace TheHindu.Model
{
    internal class SlideshowDetailsDataContext : DataContext
    {
        public static string DbConnectionString = @"isostore:/slideshow.sdf";

        public SlideshowDetailsDataContext(string dbConnectionString)
            : base(dbConnectionString)
        {
        }

        public Table<SlideShowDetails> SlideShowDetailsFromTable
        {
            get
            {
                return this.GetTable<SlideShowDetails>();
            }
        }
    }
}