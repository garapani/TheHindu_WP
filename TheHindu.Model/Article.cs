using SQLite;
using System;
using System.Collections.Generic;
using System.Net;
using TheHindu.Common;
using TheHindu.Model.Helpers;

namespace TheHindu.Model
{
    public class Slidedetail
    {
        public string ImageUrl
        {
            get;
            set;
        }

        public string ImageDescription
        {
            get;
            set;
        }
    }

    public class SlideShowDetails : BindableBase
    {
        private string _id;

        [PrimaryKey]
        public string Id
        {
            get
            {
                return _id;
            }
            set
            {
                SetProperty(ref _id, value);
            }
        }

        private int _count;

        public int Count
        {
            get
            {
                return _count;
            }
            set
            {
                SetProperty(ref _count, value);
            }
        }

        private string _url;

        public string Url
        {
            get
            {
                return _url;
            }
            set
            {
                SetProperty(ref _url, value);
            }
        }

        private List<Slidedetail> _slideDetails;

        public List<Slidedetail> SlideDetails
        {
            get
            {
                return _slideDetails;
            }
            set
            {
                SetProperty(ref _slideDetails, value);
            }
        }
    }

    public class Article : BindableBase
    {
        #region Constructor

        public Article()
        {
        }

        public Article(string id, string thumbnail)
        {
            ArticleId = id;
            Thumbnail = GetThumbnailFormat(thumbnail);
        }

        //public Article(string id, string title, string description, DateTime publishDate, string author, string thumbnail, string thumbnailDescription)
        //{
        //    Id = id;
        //    Title = title;
        //    Description = description;
        //    PublishDate = publishDate;
        //    Author = author;
        //    Thumbnail = thumbnail;
        //    ThumbnailDescription = thumbnailDescription;
        //}

        public Article(string id, string title, string description, DateTime publishDate, string author, string thumbnail, string hdthumbnail = null, string thumbanailDescription = null, string fullDescription = null)
        {
            ArticleId = id;
            Title = title;
            Description = description;
            PublishDate = publishDate;
            HdThumbnail = hdthumbnail;
            Author = author;
            Thumbnail = thumbnail;
            ThumbnailDescription = thumbanailDescription;
            FullDescription = fullDescription;
        }

        #endregion Constructor

        #region Properties

        private string _id;

        //public string Id
        //{
        //    get
        //    {
        //        return _id;
        //    }
        //    set
        //    {
        //        SetProperty(ref _id, value);
        //    }
        //}
        private string _Id;

        [PrimaryKey]
        public string ArticleId
        {
            get
            {
                return _Id;
            }
            set
            {
                SetProperty(ref _Id, value);
            }
        }

        private string _title;

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                SetProperty(ref _title, value);
            }
        }

        private string _category;

        public string Category
        {
            get
            {
                return _category;
            }
            set
            {
                SetProperty(ref _category, value);
            }
        }

        private DateTime _publishDate;

        public DateTime PublishDate
        {
            get
            {
                return _publishDate;
            }
            set
            {
                SetProperty(ref _publishDate, value);
            }
        }

        //private string _publishDateDescription;

        public string PublishDateDescription
        {
            get
            {
                return PublishDate.ToShortTimeString();
            }
            set
            {
                _id = value;
            }
        }

        private string _author;

        public string Author
        {
            get
            {
                return _author;
            }
            set
            {
                SetProperty(ref _author, value);
            }
        }

        private string _thumbnail;

        public string Thumbnail
        {
            get
            {
                if (!string.IsNullOrEmpty(_thumbnail))
                {                    
                    _thumbnail = _thumbnail.Replace("http://cdn.mobstac.com/m/img/?src=", "");
                    if (_thumbnail.Contains("&w="))
                    {
                        _thumbnail = _thumbnail.Remove(_thumbnail.IndexOf("&w="));
                    }
                    _thumbnail = _thumbnail.Replace("g.jpg", "i.jpg");
                    _thumbnail = _thumbnail.Replace("e.jpg", "i.jpg");
                    _thumbnail = _thumbnail.Replace("f.jpg", "f.jpg");
                    _thumbnail = _thumbnail.Trim();
                    _thumbnail = HttpUtility.UrlDecode(_thumbnail);
                }
                return _thumbnail;
            }

            set
            {
                SetProperty(ref _thumbnail, value);
            }
        }

        private string _hdThumbnail;

        public string HdThumbnail
        {
            get
            {
                if (string.IsNullOrEmpty(_hdThumbnail) && !string.IsNullOrEmpty(_thumbnail))
                {
                    //_hdThumbnail = _thumbnail.Replace(".jpg&w=100", ".jpg&w=300");
                    _hdThumbnail = _thumbnail.Replace("i.jpg", "g.jpg");
                }
                return _hdThumbnail;
            }

            set
            {
                SetProperty(ref _hdThumbnail, value);
            }
        }

        private string _thumbNailDescription;

        public string ThumbnailDescription
        {
            get
            {
                return _thumbNailDescription;
            }

            set
            {
                SetProperty(ref _thumbNailDescription, value);
            }
        }

        private string _description;

        public string Description
        {
            get
            {
                return _description;
            }

            set
            {
                SetProperty(ref _description, value);
            }
        }

        private string _fullDescription;

        public string FullDescription
        {
            get
            {
                return _fullDescription;
            }

            set
            {
                SetProperty(ref _fullDescription, value);
            }
        }

        public string Url
        {
            get { return ArticleId; }
            private set
            {
                _Id = value;
            }
        }

        public string DateDifference
        {
            get
            {
                return DateHelper.GetDateDifferenceText(PublishDate);
            }
            private set { _id = value; }
        }

        private bool _isSaved;

        public bool IsSaved
        {
            get
            {
                return _isSaved;
            }
            set
            {
                SetProperty(ref _isSaved, value);
            }
        }

        #endregion Properties

        #region Private Methods

        private string GetThumbnailFormat(string image)
        {
            int positionPoint = image.LastIndexOf('.');
            string extension = image.Substring(positionPoint, 4);
            string imageWithoutExtension = image.Remove(positionPoint, 4);

            return string.Format("{0}-150x150{1}", imageWithoutExtension, extension);
        }

        #endregion Private Methods
    }
}