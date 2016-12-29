using BugSense;
using BugSense.Core.Model;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Xml;
using TheHindu.Helper;
using TheHindu.ViewModel;
using Windows.Foundation;
using Windows.System.Threading;

namespace TheHindu
{
    public partial class App
    {
        #region Fields

        private bool _isPhoneApplicationInitialized;
        private static string _version;

        #endregion Fields

        #region Constructors

        public App()
        {
            UnhandledException += ApplicationUnhandledException;
            InitializeComponent();
            InitializePhoneApplication();

            TiltEffect.SetIsTiltEnabled(RootFrame, true);
            DispatcherHelper.Initialize();
            try
            {
                BugSenseHandler.Instance.InitAndStartSession(new ExceptionManager(Current), RootFrame, "4a268ccc");
                UnhandledException += App_UnhandledException;
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("App.xaml.cs:" + exception);
                }
            }
        }

        private async void App_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            try
            {
                LimitedCrashExtraDataList extrasExtraDataList = new LimitedCrashExtraDataList
                {
                    new CrashExtraData("The Hindu", e.ExceptionObject.Message),
                    new CrashExtraData("The Hindu", e.ExceptionObject.StackTrace),
                };

                BugSenseResponseResult sendResult = await BugSenseHandler.Instance.SendExceptionAsync(e.ExceptionObject, extrasExtraDataList);
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("App.xaml.cs:" + exception);
                }
            }
        }

        #endregion Constructors

        #region Properties

        public TransitionFrame RootFrame { get; private set; }

        public static string Version
        {
            get
            {
                if (_version == null)
                {
                    try
                    {
                        string assemblyName = typeof(App).Assembly.ToString();

                        if (assemblyName.IndexOf("Version=") >= 0)
                        {
                            assemblyName = assemblyName.Substring(assemblyName.IndexOf("Version=") + "Version=".Length);
                            assemblyName = assemblyName.Substring(0, assemblyName.IndexOf(","));

                            string[] versions = assemblyName.Split('.');

                            _version = string.Format("{0}.{1}", versions[0], versions[1]);
                        }
                    }
                    catch (Exception)
                    { }
                }

                return _version;
            }
        }

        #endregion Properties

        private string _logFile = "logfile.txt";

        #region Event Handlers

        private async void ApplicationLaunching(object sender, LaunchingEventArgs e)
        {
            ViewModelLocator viewModelLocator = Current.Resources["Locator"] as ViewModelLocator;
            try
            {
                await ThreadPool.RunAsync(async (IAsyncAction operation) =>
                {
                    if (viewModelLocator != null)
                    {
                        viewModelLocator.DataService.LoadHeadLineArticle();
                        await viewModelLocator.DataService.LoadTopStoriesArticlesAsync();
                    }
                });
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("App.xaml.cs:" + exception);
                }
            }

            await ThreadPool.RunAsync(async (IAsyncAction operation) =>
            {
                if (viewModelLocator != null)
                {
                    await viewModelLocator.DataService.LoadAsync();
                }
            });
            try
            {
                GoogleAnalytics.EasyTracker.GetTracker().SendView("Launch");
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("App.xaml.cs:" + exception);
                }
            }
        }

        private async void ApplicationActivated(object sender, ActivatedEventArgs e)
        {
            if (!e.IsApplicationInstancePreserved)
            {
                ViewModelLocator viewModelLocator = Current.Resources["Locator"] as ViewModelLocator;
                try
                {
                    await ThreadPool.RunAsync(async (IAsyncAction operation) =>
                    {
                        if (viewModelLocator != null)
                        {
                            viewModelLocator.DataService.LoadHeadLineArticle();
                            await viewModelLocator.DataService.LoadTopStoriesArticlesAsync();
                        }
                    });
                }
                catch (Exception exception)
                {
                    if (Debugger.IsAttached)
                    {
                        Debug.WriteLine("App.xaml.cs:" + exception);
                    }
                }

                await ThreadPool.RunAsync(async (IAsyncAction operation) =>
                {
                    if (viewModelLocator != null)
                    {
                        await viewModelLocator.DataService.LoadAsync();
                    }
                });
            }
        }

        private void ApplicationDeactivated(object sender, DeactivatedEventArgs e)
        {
            ViewModelLocator viewModelLocator = Current.Resources["Locator"] as ViewModelLocator;

            if (viewModelLocator != null && !viewModelLocator.IsSaved())
            {
                viewModelLocator.Cleanup();
            }
            //SaveLogToFile();
        }

        private void ApplicationClosing(object sender, ClosingEventArgs e)
        {
            ViewModelLocator viewModelLocator = Current.Resources["Locator"] as ViewModelLocator;

            if (viewModelLocator != null && !viewModelLocator.IsSaved())
            {
                viewModelLocator.Cleanup();
            }
            //SaveLogToFile();
        }

        private void OnRootFrameNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            //Logger.WriteLine("Navigation Failed");

            //SaveLogToFile();

            //if (System.Diagnostics.Debugger.IsAttached)
            //{
            //    System.Diagnostics.Debugger.Break();
            //}
        }

        private void OnCompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            RootVisual = RootFrame;

            RootFrame.Navigated -= OnCompleteInitializePhoneApplication;
        }

        private void ApplicationUnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            try
            {
                GoogleAnalytics.EasyTracker.GetTracker().SendException(e.ExceptionObject.Message + e.ExceptionObject.StackTrace, false);
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("App.xaml.cs:" + exception);
                }
            }
            //Logger.WriteLine("Unhandled Exception");
            //Logger.WriteLine(e.ExceptionObject);
            //SaveLogToFile();

            //if (System.Diagnostics.Debugger.IsAttached)
            //{
            //    System.Diagnostics.Debugger.Break();
            //}
            if (e != null)
            {
                Exception exception = e.ExceptionObject;

                if ((exception is XmlException || exception is NullReferenceException) && exception.ToString().ToUpper().Contains("INNERACTIVE"))
                {
                    Debug.WriteLine("Handled Inneractive exception {0}", exception);
                    e.Handled = true;
                    return;
                }
                else if (exception is NullReferenceException && exception.ToString().ToUpper().Contains("SOMA"))
                {
                    Debug.WriteLine("Handled Smaato null reference exception {0}", exception);
                    e.Handled = true;
                    return;
                }
                else if ((exception is System.IO.IOException || exception is NullReferenceException) && exception.ToString().ToUpper().Contains("GOOGLE"))
                {
                    Debug.WriteLine("Handled Google exception {0}", exception);
                    e.Handled = true;
                    return;
                }
                else if (exception is ObjectDisposedException && exception.ToString().ToUpper().Contains("MOBFOX"))
                {
                    e.Handled = true;
                    return;
                }
                else if ((exception is NullReferenceException) && exception.ToString().ToUpper().Contains("MICROSOFT.ADVERTISING"))
                {
                    e.Handled = true;
                    return;
                }
            }
        }

        #endregion Event Handlers

        #region Private Methods

        private void InitializePhoneApplication()
        {
            if (!_isPhoneApplicationInitialized)
            {
                RootFrame = new TransitionFrame()
                {
                    Background = new SolidColorBrush(Colors.White)
                };

                RootFrame.Navigated += OnCompleteInitializePhoneApplication;
                RootFrame.NavigationFailed += OnRootFrameNavigationFailed;

                _isPhoneApplicationInitialized = true;
            }
        }

        private void SaveLogToFile()
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                IsolatedStorageFileStream fs = null;
                try
                {
                    if (storage != null)
                    {
                        fs = storage.CreateFile(_logFile);
                        using (StreamWriter writer = new StreamWriter(fs))
                        {
                            Logger.Save(writer);
                            fs = null;
                        }
                    }
                }
                finally
                {
                    //if (fs != null)
                    //    fs.Dispose();
                }
            }
        }

        private void LoadLogFromFile()
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (storage != null)
                {
                    if (storage.FileExists(_logFile))
                    {
                        IsolatedStorageFileStream fs = null;
                        try
                        {
                            fs = storage.OpenFile(_logFile, FileMode.Open);
                            using (StreamReader reader = new StreamReader(fs))
                            {
                                Logger.Load(reader);
                                fs = null;
                            }
                        }
                        finally
                        {
                            //if (fs != null)
                            //    fs.Dispose();
                        }
                    }
                }
            }
        }

        #endregion Private Methods
    }
}