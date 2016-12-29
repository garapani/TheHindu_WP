using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System;
using TheHindu.Helper;

namespace TheHindu
{
    public partial class SharePage : PhoneApplicationPage
    {
        public SharePage()
        {
            InitializeComponent();
        }

        private void ShareViaSocialNetwork_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ShareLinkTask shareLinkTask = new ShareLinkTask();
            shareLinkTask.Title = "The Hindu App";
            var storeUri = DeepLinkHelper.BuildApplicationDeepLink();
            shareLinkTask.LinkUri = new Uri(storeUri);
            shareLinkTask.Message = "\"The Hindu News App\" for Windows phone 8 and 8.1";
            shareLinkTask.Show();
        }

        private void ShareViaMail_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var storeUri = DeepLinkHelper.BuildApplicationDeepLink();
            EmailComposeTask emailComposeTask = new EmailComposeTask()
            {
                Subject = "Try \"The Hindu News App\" for windows phone 8 and 8.1",
                Body = "\"The Hindu News App\" is a easy way to customize and read the news which you are interested in. You can pin your interests and start reading in one click away.Please click on this link " + storeUri,
            };

            emailComposeTask.Show();
        }

        private void ShareViaSMS_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var storeUri = DeepLinkHelper.BuildApplicationDeepLink();
            SmsComposeTask smsComposeTask = new SmsComposeTask()
            {
                Body = "Try \"The Hindu News App\" for windows phone 8 and 8.1. It's great!. " + storeUri,
            };

            smsComposeTask.Show();
        }
    }
}