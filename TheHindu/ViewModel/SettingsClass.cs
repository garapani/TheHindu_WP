using System;
using System.ComponentModel;
using Utilities;

namespace TheHindu.ViewModel
{
    public enum EnumListOfThemes
    {
        Dark,
        Light
    }

    internal enum EnumFontSize
    {
        Small = 19,
        Normal = 22,
        Medium = 25,
        Large = 32,
    }

    internal enum EnumFontFamily
    {
        SegoeWp,
        Pescadero,
        Arial,
        Georgia,
        Calibri,
        Verdana,
    }

    public class SettingsClass : INotifyPropertyChanged
    {
        public SettingsClass()
        {
        }

        private readonly object _readLock = new object();

        public void AddOrUpdateValue(string key, Object value)
        {
            IsolatedStorageHelper.SaveSettingValueImmediately(key, value);
        }

        public T GetValueOrDefault<T>(string key, T defaultValue)
        {
            T value = IsolatedStorageHelper.GetObject<T>(key);
            if (value == null)
            {
                return defaultValue;
            }
            else
            {
                return value;
            }
        }

        #region Properties

        public string SelectedBackground
        {
            get
            {
                return IsolatedStorageHelper.GetString("SelectedBackground", "White");
            }
            set
            {
                AddOrUpdateValue("SelectedBackground", value);
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("SelectedBackground"));
                }
            }
        }

        public string SelectedForeground
        {
            get
            {
                return IsolatedStorageHelper.GetString("SelectedForeground", "Black");
            }
            set
            {
                AddOrUpdateValue("SelectedForeground", value);
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("SelectedForeground"));
                }
            }
        }

        public bool IsPhoneTheme
        {
            get
            {
                return IsolatedStorageHelper.GetBool("IsPhoneTheme", true);
            }
            set
            {
                AddOrUpdateValue("IsPhoneTheme", value);
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("IsPhoneTheme"));
                }
            }
        }

        public double FontSize
        {
            get
            {
                return IsolatedStorageHelper.GetFloat("FontSize", 22.667f);
            }
            set
            {
                AddOrUpdateValue("FontSize", value);
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("FontSize"));
                }
            }
        }

        public string FontFamily
        {
            get
            {
                return IsolatedStorageHelper.GetString("FontFamily", EnumFontFamily.Calibri.ToString());
            }
            set
            {
                AddOrUpdateValue("FontFamily", value);
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("FontFamily"));
                }
            }
        }

        public bool IsRefreshAutomatic
        {
            get
            {
                return IsolatedStorageHelper.GetBool("IsRefreshAutomatic", false);
            }
            set
            {
                AddOrUpdateValue("IsRefreshAutomatic", value);
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("IsRefreshAutomatic"));
                }
            }
        }

        public bool IsDataSavingEnabled
        {
            get
            {
                return IsolatedStorageHelper.GetBool("IsDataSavingEnabled", false);
            }
            set
            {
                AddOrUpdateValue("IsDataSavingEnabled", value);
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("IsDataSavingEnabled"));
                }
            }
        }

        public bool IsDownloadingArticlesOffline
        {
            get
            {
                return IsolatedStorageHelper.GetBool("IsDownloadingArticlesOffline", true);
            }
            set
            {
                AddOrUpdateValue("IsDownloadingArticlesOffline", value);
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("IsDownloadingArticlesOffline"));
                }
            }
        }

        public bool IsToastNotificationUsed
        {
            get
            {
                return IsolatedStorageHelper.GetBool("IsToastNotificationUsed", true);
            }
            set
            {
                AddOrUpdateValue("IsToastNotificationUsed", value);
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("IsToastNotificationUsed"));
                }
            }
        }

        public bool IsQuietHoursUsed
        {
            get
            {
                return IsolatedStorageHelper.GetBool("IsQuietHoursUsed", false);
            }
            set
            {
                AddOrUpdateValue("IsQuietHoursUsed", value);
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("IsQuietHoursUsed"));
                }
            }
        }

        #endregion Properties

        #region Event Handlers

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Event Handlers
    }
}