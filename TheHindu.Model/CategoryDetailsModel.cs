using SQLite;
using System;
using TheHindu.Common;

namespace TheHindu.Model
{
    public class CategoryDetailsModel : BindableBase
    {
        //private bool StrEQYes(string str)
        //{
        //    return string.Equals(str, "yes", StringComparison.OrdinalIgnoreCase);
        //}
        private string _categoryName;

        [PrimaryKey]
        public string CategoryName
        {
            get
            {
                return _categoryName;
            }
            set
            {
                SetProperty(ref _categoryName, value);
            }
        }

        private string _categoryUrl;

        public string CategoryUrl
        {
            get
            {
                return _categoryUrl;
            }
            set
            {
                SetProperty(ref _categoryUrl, value);
            }
        }

        private int _order;

        public int Order
        {
            get
            {
                return _order;
            }
            set
            {
                SetProperty(ref _order, value);
            }
        }

        //private string _isPinnedString;
        //public string IsPinnedString
        //{
        //    get;
        //    set;
        //}

        private bool _isPinned;

        public bool IsPinned
        {
            get
            {
                return _isPinned;
            }
            set
            {
                SetProperty(ref _isPinned, value);
            }
        }

        private bool _isWorking;

        public bool IsWorking
        {
            get
            {
                return _isWorking;
            }
            set
            {
                SetProperty(ref _isWorking, value);
            }
        }

        //[Column]
        //public string IsGroupString
        //{
        //    get;
        //    set;
        //}

        private bool _isGroup;

        public bool IsGroup
        {
            get
            {
                return _isGroup;
            }
            set
            {
                SetProperty(ref _isGroup, value);
            }
        }

        private string _parentCategory;

        public string ParentCategory
        {
            get
            {
                return _parentCategory;
            }
            set
            {
                SetProperty(ref _parentCategory, value);
            }
        }

        private DateTime _lastFetchedTime;

        public DateTime LastFetchedTime
        {
            get
            {
                return _lastFetchedTime;
            }
            set
            {
                SetProperty(ref _lastFetchedTime, value);
            }
        }

        //private ListOfCategoriesGroup _group;

        //[System.Xml.Serialization.XmlElementAttribute("Group")]
        //public ListOfCategoriesGroup Group
        //{
        //    get
        //    {
        //        return this._group;
        //    }
        //    set
        //    {
        //        SetProperty(ref this._group, value);
        //    }
        //}
    }
}