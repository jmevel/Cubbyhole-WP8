using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cubbyhole.WP.Utils;
using Cubbyhole.WP.ViewModels;

namespace Cubbyhole.WP
{
    public class ViewModelLocator
    {
        public static readonly Uri MainPageUri = new Uri("/MainPage.xaml", UriKind.Relative);
        public static readonly Uri LoginRegisterUri = new Uri("/LoginRegisterPage.xaml", UriKind.Relative);

        private static MainViewModel _mainViewModel;
        public MainViewModel MainViewModel
        {
            get
            {
                return _mainViewModel ?? (_mainViewModel = new MainViewModel());
            }
        }

        private static LoginRegisterViewModel _loginRegisterViewModel;
        public LoginRegisterViewModel LoginRegisterViewModel { get { return _loginRegisterViewModel ?? (_loginRegisterViewModel = new LoginRegisterViewModel()); } }

       
    }
}
