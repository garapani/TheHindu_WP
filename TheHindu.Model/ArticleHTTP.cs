using System;
using System.Collections.Generic;
using TheHindu.Model.Helpers;

namespace TheHindu.Model
{
    public class PhotosClass
    {
        public string Caption { get; set; }

        public string Thumb { get; set; }

        public string Img { get; set; }
    }

    public class ArticleHttp
    {
        #region Constructor

        public ArticleHttp()
        {
        }

        public ArticleHttp(string newsItemId, string thumb)
        {
            NewsItemId = newsItemId;
            Thumb = GetThumbnailFormat(thumb);
        }

        public ArticleHttp(string newsItemId, string headLine, string story, string dateLine, string byLine, string thumb, string photo, string photoCaption, string url)
        {
            NewsItemId = newsItemId;
            HeadLine = headLine;
            Story = story;
            DateLine = dateLine;
            ByLine = byLine;
            Thumb = thumb;
            Photo = photo;
            WebUrl = url;
            PhotoCaption = photoCaption;
        }

        #endregion Constructor

        #region Properties

        public string Story { get; set; }

        public string Thumb { get; set; }

        public string WebUrl { get; set; }

        public string Atypes { get; set; }

        public string Caption { get; set; }

        public string ByLine { get; set; }

        public string HeadLine { get; set; }

        public string Photo { get; set; }

        public string DateLine { get; set; }

        public string PhotoCaption { get; set; }

        public List<object> Photos { get; set; }

        public string NewsItemId { get; set; }

        public bool IsRead { get; set; }

        public string DateString
        {
            get
            {
                if (!string.IsNullOrEmpty(DateLine))
                {
                    string tempDate = DateLine.Replace("IST", "");
                    DateTime tempDateTime = DateTime.Now;
                    DateTime.TryParse(tempDate, out tempDateTime);
                    return DateHelper.GetDateDifferenceText(tempDateTime);
                }
                else
                {
                    return null;
                }
            }
            private set { }
        }

        public DateTime ArticleDate
        {
            get
            {
                if (!string.IsNullOrEmpty(DateLine))
                {
                    string tempDate = DateLine.Replace("IST", "");
                    DateTime tempDateTime = DateTime.Now;
                    DateTime.TryParse(tempDate, out tempDateTime);
                    return tempDateTime;
                }
                else
                {
                    return DateTime.Now;
                }
            }
            private set { }
        }

        public string Category { get; set; }

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

    public class RootObject
    {
        public List<ArticleHttp> NewsItem { get; set; }
    }
}