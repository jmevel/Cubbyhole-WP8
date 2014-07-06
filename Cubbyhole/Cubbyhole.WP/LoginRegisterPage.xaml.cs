using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Cubbyhole.API;
using Cubbyhole.Models;
using Cubbyhole.WP.Utils;
using Microsoft.Phone.Controls;
//using Microsoft.Phone.Maps.Services;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Shell;
//using GestureEventArgs = System.Windows.Input.GestureEventArgs;
using System.Threading.Tasks;

namespace Cubbyhole.WP
{
    public partial class LoginRegisterPage : PhoneApplicationPage
    {
        private ViewModelLocator _viewModelLocator { get; set; }
        private InformerManagerLocator _informerManagerLocator { get; set; }
        //private LocationManager _locationHelper { get; set; }
        private User _user { get; set; }

        public LoginRegisterPage()
        {
            InitializeComponent();

            _viewModelLocator = new ViewModelLocator();
            _informerManagerLocator = new InformerManagerLocator();
            //_locationHelper = new LocationManager();
            _user = new User();

            _informerManagerLocator.InformerManager.NewMessage += InformerManager_NewMessage;
            //_locationHelper.LocationRetrieved += _locationHelper_LocationRetrieved;

            _viewModelLocator.LoginRegisterViewModel.UndeterminateLoading += LoginRegisterViewModel_UndeterminateLoading;
           // _viewModelLocator.LoginRegisterViewModel.PaypalUriRetreived += LoginRegisterViewModel_PaypalUriRetreived;
            
        }

       /* void LoginRegisterViewModel_PaypalUriRetreived(object sender, EventArgs e, Uri paypalUri)
        {
            MainPivot.Visibility = System.Windows.Visibility.Collapsed;
            /*PaypalWebBrower.Visibility = System.Windows.Visibility.Visible;
            this.PaypalWebBrower.Navigate(paypalUri);
            //PaypalWebBrower.Navigated+=PaypalWebBrower_Navigated;
        }*/

        void LoginRegisterViewModel_UndeterminateLoading(object sender, EventArgs e, bool isStarting)
        {
            if (isStarting)
            {
                this.ProgressBarUndeterminateLoading.Visibility = Visibility.Visible;
            }
            else
            {
                this.ProgressBarUndeterminateLoading.Visibility = Visibility.Collapsed;
            }
        }

        void InformerManager_NewMessage(object sender, EventArgs e)
        {
            var message = _informerManagerLocator.InformerManager.GetMessages().Last();
            MessageBox.Show(message.Content, message.Title, MessageBoxButton.OK);
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var isValidEmail = Regex.IsMatch(this.LoginEmailTextBox.Text,
                @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,24}))$",
                RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            if (isValidEmail == false)
            {
                MessageBox.Show("Email address you informed is incorrect format", "Error", MessageBoxButton.OK);
            }
            else if (string.IsNullOrEmpty(this.LoginEmailTextBox.Text))
            {
                MessageBox.Show("Please enter an email address", "Error", MessageBoxButton.OK);
            }
            else if (string.IsNullOrEmpty(this.LoginPasswordBox.Password))
            {
                MessageBox.Show("Please enter a password", "Error", MessageBoxButton.OK);
            }
            else
            {
                _viewModelLocator.LoginRegisterViewModel.Login(this.LoginEmailTextBox.Text,
                                                               this.LoginPasswordBox.Password);
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var isValidEmail = Regex.IsMatch(this.RegisterEmailTextBox.Text,
                @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,24}))$",
                RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            if (isValidEmail == false)
            {
                MessageBox.Show("Email address you informed is incorrect format", "Error", MessageBoxButton.OK);
            }
            else if (string.IsNullOrEmpty(this.RegisterEmailTextBox.Text))
            {
                MessageBox.Show("Please enter an email address", "Error", MessageBoxButton.OK);
            }
            else if (string.IsNullOrEmpty(this.RegisterPasswordBox.Password))
            {
                MessageBox.Show("Please enter a password", "Error", MessageBoxButton.OK);
            }
           
            else if (this.RegisterPasswordBox.Password != this.RegisterPasswordBoxConfirmation.Password)
            {
                MessageBox.Show("Password and password confirmation don't match", "Error", MessageBoxButton.OK);
            }
            else
            {
                _user.Email = this.RegisterEmailTextBox.Text;
                _user.Password = this.RegisterPasswordBox.Password;

                _user.CreationDate = DateTime.Now;
                _user.RegistrationPaidPlanDate = DateTime.Now;
                _viewModelLocator.LoginRegisterViewModel.RegisterUser(_user);
            }
        }

        private void PlanListBox_OnTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            _user.PlanId = PlanListBox.SelectedIndex + 1;
        }

       /* void PaypalWebBrower_Navigated(object sender, NavigationEventArgs e)
        {
            if (e.Uri.AbsoluteUri.StartsWith("sdgsdbdsg"))
            {
                //PaypalWebBrower.Visibility = System.Windows.Visibility.Collapsed;
                MainPivot.Visibility = System.Windows.Visibility.Visible;
            }
        }*/

        /*private void _locationHelper_LocationRetrieved(object sender, MapLocation e)
        {
            _user.City = e.Information.Address.City;
            _user.Country = e.Information.Address.County;
            _user.Street = e.Information.Address.Street;
            _user.Zip = e.Information.Address.PostalCode;
        }*/
    }
}