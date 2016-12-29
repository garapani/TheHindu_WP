using GalaSoft.MvvmLight.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework.Media;
using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Resources;

namespace TheHindu.Views
{
    public partial class ImageViewer : PhoneApplicationPage
    {
        private string _imagePath = "";

        public ImageViewer()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string imagePath = string.Empty;
            NavigationContext.QueryString.TryGetValue("Id", out imagePath);
            if (!string.IsNullOrEmpty(imagePath))
            {
                SetImageSource(imagePath);
            }
        }

        private void SetImageSource(string imageSource)
        {
            if (!String.IsNullOrEmpty(imageSource))
            {
                _imagePath = imageSource;
                Uri uri = new Uri(imageSource, UriKind.RelativeOrAbsolute);
                if (!uri.IsAbsoluteUri)
                {
                    uri = new Uri(new Uri("ms-appx://"), imageSource);
                }
                MyImage.Source = new BitmapImage(uri)
                {
                    CreateOptions = BitmapCreateOptions.BackgroundCreation,
                    //DecodePixelHeight = 1024
                };
            }
        }

        private bool NetWorkAvailable()
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                return true;
            }
            return false;
        }

        private void saveImage_Click(object sender, EventArgs e)
        {
            if (!NetWorkAvailable())
            {
                MessageBox.Show("Sorry, There is some problem with internet connectivity");
                return;
            }

            if (!string.IsNullOrEmpty(_imagePath))
            {
                string fileName = Path.GetFileName(_imagePath);
                var webClient = new WebClient();
                webClient.OpenReadCompleted += (object websender, OpenReadCompletedEventArgs ex) =>
                    {
                        try
                        {
                            if (ex != null)
                            {
                                if (ex.Cancelled == true)
                                {
                                    MessageBox.Show("Some problem in downloaing the image");
                                    return;
                                }

                                if (ex.Error != null)
                                {
                                    MessageBox.Show(string.Format("Some problem in downloaing the image: {0}", ex.Error.ToString()));
                                    return;
                                }
                                else
                                {
                                    var streamResourceInfo = new StreamResourceInfo(ex.Result, null);
                                    var userStoreForApplication = IsolatedStorageFile.GetUserStoreForApplication();
                                    if (userStoreForApplication.FileExists(fileName))
                                    {
                                        userStoreForApplication.DeleteFile(fileName);
                                    }

                                    var isolatedStorageFileStream = userStoreForApplication.CreateFile(fileName);

                                    var bitmapImage = new BitmapImage { CreateOptions = BitmapCreateOptions.None };
                                    bitmapImage.SetSource(streamResourceInfo.Stream);

                                    var writeableBitmap = new WriteableBitmap(bitmapImage);
                                    writeableBitmap.SaveJpeg(isolatedStorageFileStream, writeableBitmap.PixelWidth, writeableBitmap.PixelHeight, 0, 85);

                                    isolatedStorageFileStream.Close();
                                    isolatedStorageFileStream = userStoreForApplication.OpenFile(fileName, FileMode.Open, FileAccess.Read);

                                    // Save the image to the camera roll or saved pictures album.
                                    var mediaLibrary = new MediaLibrary();
                                    // Save the image to the saved pictures album.
                                    Picture picture = mediaLibrary.SavePicture(fileName, isolatedStorageFileStream);
                                    if (picture.Name.Contains(fileName))
                                    {
                                        DispatcherHelper.UIDispatcher.BeginInvoke(() =>
                                        {
                                            MessageBox.Show(string.Format("successfully saved the image in picture hub"));
                                        });
                                    }
                                    else
                                    {
                                        DispatcherHelper.UIDispatcher.BeginInvoke(() =>
                                        {
                                            MessageBox.Show(string.Format("Sorry, Failed to save the image"));
                                        });
                                    }
                                    isolatedStorageFileStream.Close();
                                }
                            }
                        }
                        catch (Exception)
                        {
                            DispatcherHelper.UIDispatcher.BeginInvoke(() =>
                               {
                                   MessageBox.Show(string.Format("Sorry, Failed to save the image"));
                               });
                        }
                    };

                webClient.OpenReadAsync(new Uri(_imagePath, UriKind.RelativeOrAbsolute));
            }
            else
            {
                MessageBox.Show("Sorry, failed to save the image");
            }
        }

        private double _imageScale = 1d;
        private Point _imageTranslation = new Point(0, 0);
        private Point _fingerOne;
        private Point _fingerTwo;
        private double _previousScale;

        private void OnPinchStarted(object s, PinchStartedGestureEventArgs e)
        {
            _fingerOne = e.GetPosition(MyImage, 0);
            _fingerTwo = e.GetPosition(MyImage, 1);
            _previousScale = 1;
        }

        private void OnPinchDelta(object s, PinchGestureEventArgs e)
        {
            try
            {
                var newScale = e.DistanceRatio / _previousScale;
                var currentFingerOne = e.GetPosition(MyImage, 0);
                var currentFingerTwo = e.GetPosition(MyImage, 1);
                var translationDelta = GetTranslationOffset(currentFingerOne,
                currentFingerTwo, _fingerOne, _fingerTwo, _imageTranslation, newScale);
                _fingerOne = currentFingerOne;
                _fingerTwo = currentFingerTwo;
                _previousScale = e.DistanceRatio;
                UpdatePicture(newScale, translationDelta);
            }
            catch (Exception)
            { }
        }

        private Point GetTranslationOffset(Point currentFingerOne, Point currentFingerTwo,
        Point oldFingerOne, Point oldFingerTwo, Point currentPosition, double scale)
        {
            try
            {
                var newFingerOnePosition = new Point(
                    currentFingerOne.X + (currentPosition.X - oldFingerOne.X) * scale,
                    currentFingerOne.Y + (currentPosition.Y - oldFingerOne.Y) * scale);
                var newFingerTwoPosition = new Point(
                    currentFingerTwo.X + (currentPosition.X - oldFingerTwo.X) * scale,
                    currentFingerTwo.Y + (currentPosition.Y - oldFingerTwo.Y) * scale);
                var newPosition = new Point(
                    (newFingerOnePosition.X + newFingerTwoPosition.X) / 2,
                    (newFingerOnePosition.Y + newFingerTwoPosition.Y) / 2);
                return new Point(
                    newPosition.X - currentPosition.X,
                    newPosition.Y - currentPosition.Y);
            }
            catch (Exception)
            {
                return new Point();
            }
        }

        private void UpdatePicture(double scaleFactor, Point delta)
        {
            var newscale = _imageScale * scaleFactor;
            var transform = (CompositeTransform)MyImage.RenderTransform;
            if (newscale > 1)
            {
                ApplicationBar.IsVisible = false;
                _imageScale *= scaleFactor;
                _imageTranslation = new Point
                (_imageTranslation.X + delta.X, _imageTranslation.Y + delta.Y);
                transform.ScaleX = _imageScale;
                transform.ScaleY = _imageScale;
                transform.TranslateX = _imageTranslation.X;
                transform.TranslateY = _imageTranslation.Y;
            }
            else
            {
                ApplicationBar.IsVisible = true;
                transform.TranslateX = 0;
                transform.TranslateY = 0;
                transform.ScaleX = transform.ScaleY = 1;
                _imageTranslation = new Point(0, 0);
            }
        }
    }
}