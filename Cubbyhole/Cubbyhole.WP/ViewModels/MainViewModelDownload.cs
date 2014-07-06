using Cubbyhole.WP.Utils;
using Microsoft.Phone.BackgroundTransfer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubbyhole.WP.ViewModels
{
    public partial class MainViewModel
    {
        public ObservableCollection<CustomBackgroundTransferRequest> transferRequests { get; private set; }

        bool WaitingForExternalPower;
        bool WaitingForExternalPowerDueToBatterySaverMode;
        bool WaitingForNonVoiceBlockingNetwork;
        bool WaitingForWiFi;

        private Dictionary<string, string> _contentTypes { get; set; }

        private void InitDownload()
        {
            // Reset all of the user action booleans on page load.
            WaitingForExternalPower = false;
            WaitingForExternalPowerDueToBatterySaverMode = false;
            WaitingForNonVoiceBlockingNetwork = false;
            WaitingForWiFi = false;


            // When the page loads, refresh the list of file transfers.
            InitialTansferStatusCheck();
        }

        private void InitialTansferStatusCheck()
        {
            UpdateRequestsList();

            foreach (var CustomTransfer in transferRequests)
            {
                CustomTransfer.Request.TransferStatusChanged += new EventHandler<BackgroundTransferEventArgs>(transfer_TransferStatusChanged);
                CustomTransfer.Request.TransferProgressChanged += new EventHandler<BackgroundTransferEventArgs>(transfer_TransferProgressChanged);
                ProcessTransfer(CustomTransfer.Request);
            }

            if (WaitingForExternalPower)
            {
                _informerManagerLocator.InformerManager.AddMessage("Warning", "You have one or more file transfers waiting for external power. Connect your device to external power to continue transferring.");
            }
            if (WaitingForExternalPowerDueToBatterySaverMode)
            {
                _informerManagerLocator.InformerManager.AddMessage("Warning", "You have one or more file transfers waiting for external power. Connect your device to external power or disable Battery Saver Mode to continue transferring.");
            }
            if (WaitingForNonVoiceBlockingNetwork)
            {
                _informerManagerLocator.InformerManager.AddMessage("Warning", "You have one or more file transfers waiting for a network that supports simultaneous voice and data.");
            }
            if (WaitingForWiFi)
            {
                _informerManagerLocator.InformerManager.AddMessage("Warning", "You have one or more file transfers waiting for a WiFi connection. Connect your device to a WiFi network to continue transferring.");
            }
        }

        public void UpdateRequestsList()
        {
            // The Requests property returns new references, so make sure that
            // you dispose of the old references to avoid memory leaks.
            if (transferRequests != null)
            {
                foreach (var CustomRequest in transferRequests)
                {
                    CustomRequest.Request.Dispose();
                }
            }
            
            transferRequests.Clear();
            foreach (var request in BackgroundTransferService.Requests.ToList())
            {
                transferRequests.Add(new CustomBackgroundTransferRequest(request));
            }
           // transferRequests = new ObservableCollection<BackgroundTransferRequest>(BackgroundTransferService.Requests);
        }

        void transfer_TransferStatusChanged(object sender, BackgroundTransferEventArgs e)
        {
            ProcessTransfer(e.Request);
        }

        void transfer_TransferProgressChanged(object sender, BackgroundTransferEventArgs e)
        {
            UpdateRequestsList();
        }

        private void ProcessTransfer(BackgroundTransferRequest transfer)
        {
            switch (transfer.TransferStatus)
            {
                case TransferStatus.Completed:

                    if (transfer.StatusCode == 200 || transfer.StatusCode == 206)
                    {
                        RemoveTransferRequest(transfer.RequestId);
                        using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                        {
                            string filename = transfer.Tag;
                            if (isoStore.FileExists(filename))
                            {
                                isoStore.DeleteFile(filename);
                            }
                            if (isoStore.FileExists(transfer.DownloadLocation.OriginalString))
                            {
                                isoStore.MoveFile(transfer.DownloadLocation.OriginalString, filename);
                                GetLocalEntities();
                                UpdateRequestsList();
                            }
                        }
                    }
                    else if (transfer.StatusCode == 201)
                    {
                        RemoveTransferRequest(transfer.RequestId);
                        using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                        {
                            string filename = transfer.Tag;
                            if (isoStore.FileExists(filename))
                            {
                                isoStore.DeleteFile(filename);
                            }
                            GetEntitites();
                            UpdateRequestsList();
                        }
                    }
                    else
                    {
                        RemoveTransferRequest(transfer.RequestId);
                        if (transfer.TransferError != null)
                        {
                            _informerManagerLocator.InformerManager.AddMessage("Transfer error", "A problem accured when transfering the file " + transfer.Tag);
                        }
                    }
                    break;

                case TransferStatus.WaitingForExternalPower:
                    WaitingForExternalPower = true;
                    break;

                case TransferStatus.WaitingForExternalPowerDueToBatterySaverMode:
                    WaitingForExternalPowerDueToBatterySaverMode = true;
                    break;

                case TransferStatus.WaitingForNonVoiceBlockingNetwork:
                    WaitingForNonVoiceBlockingNetwork = true;
                    break;

                case TransferStatus.WaitingForWiFi:
                    WaitingForWiFi = true;
                    break;
            }
        }

        public void RemoveTransferRequest(string transferID)
        {
            // Use Find to retrieve the transfer request with the specified ID.
            BackgroundTransferRequest transferToRemove = BackgroundTransferService.Find(transferID);

            // try to remove the transfer from the background transfer service.
            try
            {
                if (transferToRemove != null)
                {
                    BackgroundTransferService.Remove(transferToRemove);
                }
            }
            catch (Exception ex)
            {
                _informerManagerLocator.InformerManager.AddMessage("Error", ex.Message);
            }
        }
    }
}
