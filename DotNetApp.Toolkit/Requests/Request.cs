using Microsoft.Phone.Net.NetworkInformation;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DotNetApp.Toolkit.Requests
{
    public enum ContentType
    {
        None,
        TextXml,
        TextHtml,
        ApplicationJson
    }

    public class RequestArgs
    {
        #region Constructor

        public RequestArgs(bool isSuccess, uint responseTimeInMs)
        {
            IsSuccess = isSuccess;
            ResponseTimeInMs = responseTimeInMs;
        }

        #endregion

        #region Properties

        public bool IsSuccess { get; set; }
        public uint ResponseTimeInMs { get; private set; }

        #endregion

        #region Methods

        public void AddRequestResult(RequestArgs requestArgs)
        {
            if (!requestArgs.IsSuccess)
            {
                IsSuccess = false;
            }

            ResponseTimeInMs += requestArgs.ResponseTimeInMs;
        }

        #endregion
    }

    public class RequestResponseEventArgs : AsyncCompletedEventArgs
    {
        #region Constructor

        public RequestResponseEventArgs(string result, AsyncCompletedEventArgs asyncCompletedEventArgs)
            : base(asyncCompletedEventArgs.Error, asyncCompletedEventArgs.Cancelled, asyncCompletedEventArgs.UserState)
        {
            Result = result;
        }

        #endregion

        #region Properties

        public string Result { get; private set; }

        #endregion
    }

    public abstract class Request<T> : IRequest<T> where T : RequestArgs
    {
        #region Constants

        protected static readonly string[] ContentTypeString = new[] { "", "text/xml", "text/html", "application/json" };

        #endregion

        #region Fields

        private DateTime _startTime;

        #endregion

        #region Constructors

        protected Request(Action<T> callback = null)
        {
            Callback = callback;
        }

        #endregion

        #region Properties

        public Action<T> Callback { get; set; }
        protected uint ResponseTimeInMs { get; private set; }
        protected WebClient WebClient { get; set; }
        protected abstract string Link { get; }

        protected virtual string Post
        {
            get { return null; }
        }

        protected virtual ContentType ContentType
        {
            get { return ContentType.None; }
        }

        #endregion

        #region Methods

        public async Task<string> Send()
        {
            //if (WebClient != null)
            //{
            //    throw new Exception("Send can be called only once.");
            //}

            var handler = new HttpClientHandler();
            if (handler.SupportsAutomaticDecompression)
            {
                handler.AutomaticDecompression = DecompressionMethods.GZip |
                                                 DecompressionMethods.Deflate;
            }
            var httpClient = new HttpClient(handler);
            return await httpClient.GetStringAsync(Link);   

            //WebClient = new WebClient();
            //if (ContentType != ContentType.None)
            //{
            //    WebClient.Headers["Content-Type"] = ContentTypeString[(int)ContentType];
            //     //Accept-Encoding: gzip, deflate
                
            //}
            //WebClient.Headers["Accept-Encoding"] = "gzip, deflate";
            //_startTime = DateTime.Now;

            //string post = Post;

            //if (post != null)
            //{
            //    WebClient.UploadStringCompleted += OnRequestPostCompleted;
            //    WebClient.UploadStringAsync(new Uri(Link), "POST", post);
            //}
            //else
            //{
            //    WebClient.DownloadStringCompleted += OnRequestGetCompleted;
            //    if (Link != null)
            //        WebClient.DownloadStringAsync(new Uri(Link));
            //}
        }

        #endregion

        #region Event Handlers

        protected virtual void OnRequestCompleted(RequestResponseEventArgs requestResponseEventArgs)
        {
        }

        #endregion

        #region Protected Methods

        protected void InvokeCallback(T requestArgs)
        {
            if (Callback != null)
            {
                Callback.Invoke(requestArgs);
            }
        }

        protected static string GetElementValue(XElement xElement, string name)
        {
            string value = "";

            try
            {
                XElement element = xElement.Element(name);

                if (element != null)
                {
                    value = element.Value;
                }
            }
            catch (Exception venkatException)
            {
/*#if DEBUG
                Debug.WriteLine(venkatException.StackTrace);
#endif */
            }

            return value;
        }

        protected static string GetElementAttributeValue(XElement xElement, string name, string attribute)
        {
            string value = "";

            try
            {
                XElement element = xElement.Element(name);

                if (element != null)
                {
                    XAttribute xAttribute = element.Attribute(attribute);

                    if (xAttribute != null)
                    {
                        value = xAttribute.Value;
                    }
                }
            }
            catch (Exception venkatException)
            {
/*#if DEBUG
                Debug.WriteLine(venkatException.StackTrace);
#endif */
            }

            return value;
        }

        protected static uint GetElementAttributeUintValue(XElement xElement, string name, string attribute)
        {
            uint value = 0;

            try
            {
                XElement element = xElement.Element(name);

                if (element != null)
                {
                    XAttribute xAttribute = element.Attribute(attribute);

                    if (xAttribute != null)
                    {
                        UInt32.TryParse(xAttribute.Value, out value);
                    }
                }
            }
            catch (Exception venkatException)
            {
/*#if DEBUG
                Debug.WriteLine(venkatException.StackTrace);
#endif */
            }

            return value;
        }

        protected static uint GetElementUintValue(XElement xElement, string name)
        {
            uint value;
            UInt32.TryParse(GetElementValue(xElement, name), out value);

            return value;
        }

        protected static int GetElementIntValue(XElement xElement, string name)
        {
            int value;
            Int32.TryParse(GetElementValue(xElement, name), out value);

            return value;
        }

        protected static bool GetElementBoolValue(XElement xElement, string name)
        {
            bool value;
            Boolean.TryParse(GetElementValue(xElement, name).ToLower(), out value);

            return value;
        }

        #endregion

        #region Event Handlers

        private void OnRequestGetCompleted(object sender, DownloadStringCompletedEventArgs downloadStringCompleted)
        {
            try
            {
                CalculateResponseTime();

                
                OnRequestCompleted(new RequestResponseEventArgs(downloadStringCompleted.Result, downloadStringCompleted));
            }
            catch (Exception venkatException)
            {
/*#if DEBUG
                Debug.WriteLine(venkatException.StackTrace);
#endif */
            }
        }

        private void OnRequestPostCompleted(object sender, UploadStringCompletedEventArgs uploadStringCompleted)
        {
            try
            {
                CalculateResponseTime();

                OnRequestCompleted(new RequestResponseEventArgs(uploadStringCompleted.Result, uploadStringCompleted));
            }
            catch (Exception venkatException)
            {
/*#if DEBUG
                Debug.WriteLine(venkatException.StackTrace);
#endif */
            }
        }

        #endregion

        #region Private Methods

        private void CalculateResponseTime()
        {
            ResponseTimeInMs = (uint)((DateTime.Now - _startTime).TotalMilliseconds);
        }

        #endregion
    }
}