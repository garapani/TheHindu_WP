using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System;
using System.Diagnostics;
using System.Windows.Navigation;

namespace TheHindu.Views
{
    public partial class MyAppsPage : PhoneApplicationPage
    {
        public MyAppsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private void outlookGrid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            try
            {
                Microsoft.Phone.Tasks.MarketplaceDetailTask mdt = new MarketplaceDetailTask();
                mdt.ContentIdentifier = "c14185ec-ef9a-4ec0-a223-4de814abb838";
                mdt.ContentType = MarketplaceContentType.Applications;
                mdt.Show();
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("MyAppsPage.xaml.cs:" + exception);
                }
            }
        }

        private void businessLineGrid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            try
            {
                Microsoft.Phone.Tasks.MarketplaceDetailTask mdt = new MarketplaceDetailTask();
                mdt.ContentIdentifier = "c147a7dd-038e-4ba3-8d9b-2d68662670bb";
                mdt.ContentType = MarketplaceContentType.Applications;
                mdt.Show();
            }
            catch (Exception)
            { }
        }

        private void techCrunchGrid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            try
            {
                Microsoft.Phone.Tasks.MarketplaceDetailTask mdt = new MarketplaceDetailTask();
                mdt.ContentIdentifier = "4fef72c1-0ef9-40ce-a777-e948a925f0d3";
                mdt.ContentType = MarketplaceContentType.Applications;
                mdt.Show();
            }
            catch (Exception)
            { }
        }

        private void dailywallpapersgrid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            try
            {
                Microsoft.Phone.Tasks.MarketplaceDetailTask mdt = new MarketplaceDetailTask();
                mdt.ContentIdentifier = "9fd8506e-c90f-48de-b0ca-37e6cddcf28d";
                mdt.ContentType = MarketplaceContentType.Applications;
                mdt.Show();
            }
            catch (Exception)
            { }
        }
    }
}