using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Cubbyhole.Models;
using Cubbyhole.API;
using Cubbyhole.WP.Resources;
using Cubbyhole.WP.Utils;
using Microsoft.Phone.Controls;
//using Microsoft.Phone.Maps.Services;
using Microsoft.Phone.Net.NetworkInformation;

namespace Cubbyhole.WP.ViewModels
{
    public delegate void UndeterminateLoadingEventHandler(object sender, EventArgs e, bool isStarting);
    //public delegate void PaypalUriRetreivedEventHandler(object sender, EventArgs e, Uri paypalUri);
    public class LoginRegisterViewModel
    {
        private InternetConnectionManagerLocator _internetConnectionManagerLocator { get; set; }
        private InformerManagerLocator _informerManagerLocator { get; set; }
        //private LocationManager _locationHelper { get; set; }
        private InAppPurchaseManager _inAppPurchaseManager { get; set; }
        private User _user { get; set; }
        private PlanAPI _planApi { get; set; }
        private UserAPI _userApi { get; set; }

        public ObservableCollection<Plan> Plans { get; private set; }
        public event UndeterminateLoadingEventHandler UndeterminateLoading;

        //public event PaypalUriRetreivedEventHandler PaypalUriRetreived;


        public LoginRegisterViewModel()
        {
            _internetConnectionManagerLocator = new InternetConnectionManagerLocator();
            _informerManagerLocator = new InformerManagerLocator();
            //_locationHelper = new LocationManager();
            _inAppPurchaseManager = new InAppPurchaseManager();
            _user = new User();
            _planApi = new PlanAPI(new Uri(AppResources.BaseAddress));
            _userApi = new UserAPI(new Uri(AppResources.BaseAddress));

            _internetConnectionManagerLocator.InternetConnectionManager.ConnectionStateChanged += InternetConnectionManager_ConnectionStateChanged;
            _userApi.ErrorReceived += _userApi_ErrorReceived;

            Plans = new ObservableCollection<Plan>();

            GetPlans();
            //_locationHelper.GetPhoneLocation();
        }

        async void _inAppPurchaseManager_PlanPurchased(object sender, EventArgs e, Plan plan)
        {
            if (plan != null)
            {
                _user.PlanId = plan.Id;
            }
            else
            {
                _user.PlanId = 1;
            }
            _user.RegistrationPaidPlanDate = DateTime.Now;
            _user = await _userApi.UpdateUser(_user);
            if (_user != null)
            {
                AuthorizeAccess();
                _inAppPurchaseManager.PlanPurchased -= _inAppPurchaseManager_PlanPurchased;
                if (UndeterminateLoading != null)
                {
                    UndeterminateLoading(this, new EventArgs(), false);
                }
            }
        }

        public async Task BuyPlan(int planId)
        {
            Plan plan = await _planApi.GetPlanById(planId);
            if (planId != 1)
            {
                _inAppPurchaseManager.PlanPurchased += _inAppPurchaseManager_PlanPurchased;
                _inAppPurchaseManager.BuyProductByName(plan);
            }
            else
            {
                AuthorizeAccess();
            }
        }

        private void AuthorizeAccess()
        {
            if (_user != null)
            {
                IsolatedStorageSettings.ApplicationSettings.Add("user", _user);
                try
                {
                    ((PhoneApplicationFrame)Application.Current.RootVisual).Navigate(new Uri("/MainPage.xaml",
                                                                                              UriKind.Relative));
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public async Task RegisterUser(User user)
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains("user") == true)
            {
                IsolatedStorageSettings.ApplicationSettings.Remove("user");
            }
            _user = user;
            if (UndeterminateLoading != null)
            {
                UndeterminateLoading(this, new EventArgs(), true);
            }
            _user = await _userApi.CreateUser(_user);
            if (_user != null)
            {
                await BuyPlan(_user.PlanId);
            }
            if (UndeterminateLoading != null)
            {
                UndeterminateLoading(this, new EventArgs(), false);
            }
        }

        public async Task Login(string email, string password)
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains("user") == true)
            {
                IsolatedStorageSettings.ApplicationSettings.Remove("user");
            }
            if (UndeterminateLoading != null)
            {
                UndeterminateLoading(this, new EventArgs(), true);
            }
            _user = await _userApi.Login(email, password);
            if (UndeterminateLoading != null)
            {
                UndeterminateLoading(this, new EventArgs(), false);
            }
            if(_user!=null)
            {
                AuthorizeAccess();
            }
        }

        private async void GetPlans()
        {
            List<Plan> plans = await _planApi.GetPlans();
            
            //List<Plan> plans = await Cubbyhole.API.Mock.PlanMock.Get4FakePlans();
            foreach (Plan plan in plans)
            {
                this.Plans.Add(plan);
            }
            //this.Plans = plans;
        }

        void InternetConnectionManager_ConnectionStateChanged(object sender, NetworkNotificationEventArgs e)
        {
            if (_internetConnectionManagerLocator.InternetConnectionManager.IsInternetConnectionAvailable() == false)
            {
                _informerManagerLocator.InformerManager.AddMessage("Error", "Connection lost");
            }
        }

        private void _userApi_ErrorReceived(object sender, ErrorEventArgs e)
        {
            _informerManagerLocator.InformerManager.AddMessage(e.Error.Title, e.Error.Message);
        }
    }
}
