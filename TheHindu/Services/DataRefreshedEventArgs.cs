using System;

namespace TheHindu.Services
{
    public enum DataType
    {
        TopStoriesArticles,
        BreakingNewsArticles,
        Articles,
        Categories,
    }

    public class DataRefreshedEventArgs : EventArgs
    {
        #region Constructor

        public DataRefreshedEventArgs(DataType dataType, bool isSuccess)
        {
            DataType = dataType;
            IsSuccess = isSuccess;
        }

        #endregion Constructor

        #region Properties

        public bool IsSuccess { get; private set; }

        public DataType DataType { get; private set; }

        #endregion Properties
    }
}