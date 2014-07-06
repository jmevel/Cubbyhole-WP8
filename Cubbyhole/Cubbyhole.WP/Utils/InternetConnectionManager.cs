using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Phone.Net.NetworkInformation;

namespace Cubbyhole.WP.Utils
{
    public delegate void ConnectionSateChangedEventHandler(object sender, NetworkNotificationEventArgs e);

    public class InternetConnectionManager
    {
        private bool _isInternetConnectionAvailable;

        public event ConnectionSateChangedEventHandler ConnectionStateChanged;

        public InternetConnectionManager()
        {
            DeviceNetworkInformation.NetworkAvailabilityChanged += DeviceNetworkInformation_NetworkAvailabilityChanged;
            _isInternetConnectionAvailable = NetworkInterface.GetIsNetworkAvailable();

        }

        public bool IsInternetConnectionAvailable()
        {
            /*TestEvent(this, new EventArgs());
            _isInternetConnectionAvailable = NetworkInterface.GetIsNetworkAvailable();*/
            return _isInternetConnectionAvailable;
        }
        

        void DeviceNetworkInformation_NetworkAvailabilityChanged(object sender, NetworkNotificationEventArgs e)
        {
            if (e.NetworkInterface.InterfaceState.Equals(ConnectState.Disconnected))
            {
                _isInternetConnectionAvailable = false;
            }
            else
            {
                _isInternetConnectionAvailable = true;
            }
            ConnectionStateChanged(this, e);
        }
    }

    public class InternetConnectionManagerLocator
    {
        private static InternetConnectionManager _manager;
        public InternetConnectionManager InternetConnectionManager
        {
            get
            {
                return _manager ?? (_manager = new InternetConnectionManager());
            }
        }
    }
}
