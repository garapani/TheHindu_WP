using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TheHindu.Model
{
    public enum EnumListOfThemes
    {
        Dark,
        Light
    }

    internal enum EnumFontSize
    {
        Small, //= 18.667,
        Normal,// = 20,
        Medium,// = 22.667,
        Large,// = 32
    }

    internal enum EnumFontFamily
    {
        SegoeWp,
        TimesNewRoman,
        Arial,
        Georgia,
        Calibri,
        Verdana,
    }

    public class Settings : INotifyPropertyChanged
    {
        #region Fields

        private bool _isRefreshAutomatic;

        //private bool _isDataSavingEnabled = false;
        private bool _isDownloadingArticleOffline = true;

        private bool _isToastNotificationUsed = true;
        private bool _isQuietHoursUsed = false;
        private bool _isLiveTileSupport = true;
        private string _selectedTheme;

        #endregion Fields

        #region Properties

        public string SelectedTheme
        {
            get
            {
                return _selectedTheme;
            }
            set
            {
                if (_selectedTheme != value)
                {
                    _selectedTheme = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("SelectedTheme"));
                    }
                }
            }
        }

        private int _fontSize = 18;

        public int FontSize
        {
            get
            {
                return _fontSize;
            }
            set
            {
                if (_fontSize != value)
                {
                    _fontSize = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("FontSize"));
                    }
                }
            }
        }

        private string _fontfamily = EnumFontFamily.SegoeWp.ToString();

        public string FontFamily
        {
            get
            {
                return _fontfamily;
            }
            set
            {
                if (_fontfamily != value)
                {
                    _fontfamily = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("FontFamily"));
                    }
                }
            }
        }

        private int _bigfontSize = 30;

        public int BigFontSize
        {
            get
            {
                return _bigfontSize;
            }
            set
            {
                if (_bigfontSize != value)
                {
                    _bigfontSize = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("BigFontSize"));
                    }
                }
            }
        }

        private List<string> _themes;

        public List<string> Themes
        {
            get
            {
                if (_themes == null)
                    _themes = new List<string>();
                _themes.Clear();
                _themes.Add(EnumListOfThemes.Dark.ToString());
                _themes.Add(EnumListOfThemes.Light.ToString());
                return _themes;
            }
            private set
            {
                //if (PropertyChanged != null)
                //{
                //    PropertyChanged(this, new PropertyChangedEventArgs("Themes"));
                //}
            }
        }

        public bool IsRefreshAutomatic
        {
            get { return _isRefreshAutomatic; }

            set
            {
                if (_isRefreshAutomatic != value)
                {
                    _isRefreshAutomatic = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("IsRefreshAutomatic"));
                }
            }
        }

        //public bool IsDataSavingEnabled
        //{
        //    get { return _isDataSavingEnabled; }

        //    set
        //    {
        //        if (_isDataSavingEnabled != value)
        //        {
        //            _isDataSavingEnabled = value;
        //            if (PropertyChanged != null)
        //                PropertyChanged(this, new PropertyChangedEventArgs("IsDataSavingEnabled"));
        //        }
        //    }
        //}

        public bool IsDownloadingArticlesOffline
        {
            get { return _isDownloadingArticleOffline; }

            set
            {
                if (_isDownloadingArticleOffline != value)
                {
                    _isDownloadingArticleOffline = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("IsDownloadingArticleOffline"));
                }
            }
        }

        public bool IsToastNotificationUsed
        {
            get { return _isToastNotificationUsed; }

            set
            {
                if (_isToastNotificationUsed != value)
                {
                    _isToastNotificationUsed = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("IsToastNotificationUsed"));
                }
            }
        }

        public bool IsLiveTileSupport
        {
            get
            {
                return _isLiveTileSupport;
            }
            set
            {
                if (_isLiveTileSupport != value)
                {
                    _isLiveTileSupport = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("IsLiveTileSupport"));
                }
            }
        }

        public bool IsQuietHoursUsed
        {
            get
            {
                return _isQuietHoursUsed;
            }
            set
            {
                if (_isQuietHoursUsed != value)
                {
                    _isQuietHoursUsed = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("IsQuietHoursUsed"));
                }
            }
        }

        private DateTime _quietHoursStartTime = DateTime.Now;

        public DateTime QuietHoursStartTime
        {
            get
            {
                return _quietHoursStartTime;
            }
            set
            {
                if (_quietHoursStartTime != value)
                {
                    _quietHoursStartTime = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("QuietHoursStartTime"));
                    }
                }
            }
        }

        private DateTime _quietHoursEndTime = DateTime.Now;

        public DateTime QuietHoursEndTime
        {
            get
            {
                return _quietHoursEndTime;
            }
            set
            {
                if (_quietHoursEndTime != value)
                {
                    _quietHoursEndTime = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("QuietHoursEndTime"));
                    }
                }
            }
        }

        #endregion Properties

        #region Event Handlers

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Event Handlers
    }
}