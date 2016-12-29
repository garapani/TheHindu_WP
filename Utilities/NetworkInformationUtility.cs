using Microsoft.Phone.Net.NetworkInformation;
using System;
using System.Threading;

namespace Utilities
{
    public class NetworkTypeEventArgs : EventArgs
    {
        #region Constructor

        public NetworkTypeEventArgs(NetworkInterfaceType type, bool hasTimeout = false)
        {
            Type = type;
            HasTimeout = hasTimeout;
        }

        #endregion Constructor

        #region Properties

        public bool HasTimeout { get; private set; }

        public bool HasInternet
        {
            get { return Type != NetworkInterfaceType.None; }
        }

        public NetworkInterfaceType Type { get; private set; }

        #endregion Properties
    }

    /// <summary>
    /// Static class to get the NetworkInterfaceType without blocking the UI thread.
    /// </summary>
    public static class NetworkInformationUtility
    {
        #region Fields

        private static bool _isGettingNetworkType;
        private static readonly object SynchronizationObject = new object();
        private static Timer _timer;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Get the NetworkInterfaceType asynchronously.
        /// </summary>
        /// <param name="timeoutInMs">Specifies the timeout in milliseconds.</param>
        public static void GetNetworkTypeAsync(int timeoutInMs)
        {
            lock (SynchronizationObject)
            {
                if (!_isGettingNetworkType)
                {
                    _isGettingNetworkType = true;

                    if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
                    {
                        Thread thread = new Thread(GetNetworkType) { IsBackground = true };
                        thread.Start(timeoutInMs);
                    }
                    else
                    {
                        FireGetNetworkTypeCompleted(NetworkInterfaceType.None);
                    }
                }
            }
        }

        #endregion Methods

        #region Delegates

        public delegate void NetworkTypeEventHandler(object sender, NetworkTypeEventArgs e);

        #endregion Delegates

        #region Events

        public static event NetworkTypeEventHandler GetNetworkTypeCompleted;

        #endregion Events

        #region Event Handlers

        private static void OnTimerElapsed(object state)
        {
            FireGetNetworkTypeCompleted(NetworkInterfaceType.None, true);
        }

        #endregion Event Handlers

        #region Private Methods

        private static void GetNetworkType(object state)
        {
            _timer = new Timer(OnTimerElapsed, null, (int)state, 0);

            // This is a blocking call, this is why a thread is used to let the UI to be fluid
            NetworkInterfaceType type = NetworkInterface.NetworkInterfaceType;

            _timer.Dispose();
            _timer = null;

            FireGetNetworkTypeCompleted(type);
        }

        private static void FireGetNetworkTypeCompleted(NetworkInterfaceType type, bool hasTimeout = false)
        {
            lock (SynchronizationObject)
            {
                if (_isGettingNetworkType)
                {
                    _isGettingNetworkType = false;

                    NetworkTypeEventHandler networkTypeEventHandler = GetNetworkTypeCompleted;

                    if (networkTypeEventHandler != null)
                    {
                        networkTypeEventHandler(null, new NetworkTypeEventArgs(type, hasTimeout));
                    }
                }
            }
        }

        #endregion Private Methods
    }
}