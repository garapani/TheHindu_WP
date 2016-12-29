using System;
using TheHindu.Utils;

namespace TheHindu
{
    public class CategoriesChangedEventArgs : EventArgs
    {
        public ListOfCategories ListOfcategories;

        public CategoriesChangedEventArgs(ListOfCategories categories)
        {
            ListOfcategories = categories;
        }
    }
}