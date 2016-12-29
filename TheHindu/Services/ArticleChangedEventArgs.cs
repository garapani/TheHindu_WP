using System;
using TheHindu.Model;

namespace TheHindu.Services
{
    public class ArticleChangedEventArgs : EventArgs
    {
        #region Constructor

        public ArticleChangedEventArgs(Article article)
        {
            Article = article;
        }

        #endregion Constructor

        #region Properties

        public Article Article { get; private set; }

        #endregion Properties
    }
}