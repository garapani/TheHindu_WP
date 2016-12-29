using GalaSoft.MvvmLight;
using System.ComponentModel;
using TheHindu.Model;
using TheHindu.Services;

namespace TheHindu.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        #region Constructor

        public SettingsViewModel(DataService dataService)
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                Settings = dataService.Settings;
            }
        }

        #endregion Constructor

        #region Properties

        public Settings Settings { get; private set; }

        #endregion Properties
    }
}