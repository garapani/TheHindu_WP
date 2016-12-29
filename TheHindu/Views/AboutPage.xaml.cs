using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Navigation;
using TheHindu.Helper;

namespace TheHindu.Views
{
    public partial class AboutUs : PhoneApplicationPage
    {
        public AboutUs()
        {
            InitializeComponent();
        }

        private void Rate_Click(object sender, RoutedEventArgs e)
        {
            var mp = new MarketplaceReviewTask();
            mp.Show();
        }

        private void Share_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/SharePage.xaml", UriKind.Relative));
        }

        private void Feedback_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EmailComposeTask emailComposeTask = new EmailComposeTask
                {
                    To = "vgarapani@thevillagesoftware.com",
                    Subject = "Feedback on TheHindu App",
                };
                emailComposeTask.Show();
            }
            catch (Exception)
            { }
        }

        private void CrashLogs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (storage.FileExists("logfile.txt"))
                    {
                        IsolatedStorageFileStream fs = null;
                        try
                        {
                            fs = storage.OpenFile("logfile.txt", FileMode.Open);
                            using (StreamReader reader = new StreamReader(fs))
                            {
                                Logger.Load(reader);
                                fs = null;
                            }
                        }
                        finally
                        {
                            //if (fs != null)
                            //{
                            //    fs.Dispose();
                            //}
                        }

                        string logContent = Logger.GetStoredLog();
                        if (!string.IsNullOrEmpty(logContent))
                        {
                            EmailComposeTask email = new EmailComposeTask();
                            email.To = "vgarapani@thevillagesoftware.com";
                            email.Subject = "crash dump of the Hindu App";
                            email.Body = logContent;
                            email.Show();
                        }
                        else
                        {
                            MessageBox.Show("Till now, no crashes occured", "The Hindu", MessageBoxButton.OK);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Till now, no crashes occured", "The Hindu", MessageBoxButton.OK);
                    }
                }
            }
            catch (Exception venkatException)
            {
                Logger.WriteLine(venkatException);
            }
        }

        private void MyApps_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/MyAppsPage.xaml", UriKind.Relative));
        }
    }
}