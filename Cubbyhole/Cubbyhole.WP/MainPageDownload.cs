using Microsoft.Phone.BackgroundTransfer;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Windows.Storage;

namespace Cubbyhole.WP
{
    public partial class MainPage
    {    

        private void CancelButton_Click(object sender, EventArgs e)
        {
            string transferID = ((Button)sender).Tag as string;
            _viewModelLocator.MainViewModel.RemoveTransferRequest(transferID);
            _viewModelLocator.MainViewModel.UpdateRequestsList();
        }
    }
}
