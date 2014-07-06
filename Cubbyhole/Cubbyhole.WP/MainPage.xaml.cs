using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Cubbyhole.WP.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Cubbyhole.WP.Resources;
using Cubbyhole.Models;
using Cubbyhole.API;
using Cubbyhole.WP.Utils;
using System.ComponentModel;
using System.Windows.Data;
using Microsoft.Phone.Tasks;

namespace Cubbyhole.WP
{
    public partial class MainPage : PhoneApplicationPage
    {
        private ViewModelLocator _viewModelLocator { get; set; }
        private InformerManagerLocator _informerManagerLocator { get; set; }

        PhotoChooserTask photoChooserTask = null;

        public MainPage()
        {
            InitializeComponent();
            _viewModelLocator = new ViewModelLocator();
            _informerManagerLocator = new InformerManagerLocator();
            _informerManagerLocator.InformerManager.NewMessage += InformerManager_NewMessage;

            photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += new EventHandler<PhotoResult>(PhotoChooserTask_Completed);

            _viewModelLocator.MainViewModel.BlockingLoading += MainViewModel_BlockingLoading;

            //_viewModelLocator.MainViewModel.SelectedEntityChanged += MainViewModel_SelectedEntityChanged;

        }

        // FInally I don't care about this
        /*
        void MainViewModel_SelectedEntityChanged(object sender, EventArgs e, bool isFolder)
        {
            if (isFolder)
            {
                
            }
        }*/

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _viewModelLocator.MainViewModel.Init();
            _viewModelLocator.MainViewModel.UpdateRequestsList();
        } 

        void MainViewModel_BlockingLoading(object sender, EventArgs e, bool isLoading)
        {
            if (isLoading == true)
            {
                this.Loading_Layer.Visibility = Visibility.Visible;
            }
            else
            {
                this.Loading_Layer.Visibility = Visibility.Collapsed;
            }
        }

        private void Button_NewFolder_Click(object sender, EventArgs e)
        {
            folder_create_popup.IsOpen = true;
        }

        private void Button_Paste_Click(object sender, EventArgs e)
        {
            _viewModelLocator.MainViewModel.PasteEntity();
            var pasteMenuItem = ApplicationBar.MenuItems[0];
            (pasteMenuItem as ApplicationBarMenuItem).IsEnabled = false;
        }

        private void Button_Refresh_Click(object sender, EventArgs e)
        {
            _viewModelLocator.MainViewModel.GetEntitites();
        }

        private void Button_Logout_Click(object sender, EventArgs e)
        {
            _viewModelLocator.MainViewModel.Logout();
        }

        private void Entity_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            _viewModelLocator.MainViewModel.ProceedWithItem(ItemsListBox.SelectedIndex);
        }

        private void Entity_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var selectedEntity = (sender as Grid).DataContext as DiskEntity;
            _viewModelLocator.MainViewModel.SelectEntity(selectedEntity);
        }

        private void Entity_Rename_Click(object sender, RoutedEventArgs e)
        {
            var selectedEntity = _viewModelLocator.MainViewModel.SelectedEntity;
            if (selectedEntity.GetType().Equals(typeof(File)))
            {
                var nameParts = selectedEntity.Name.Split('.');
                string extension = nameParts.Last();
                int index = selectedEntity.Name.IndexOf(extension);
                string name = selectedEntity.Name.Substring(0, index-1);
                _viewModelLocator.MainViewModel.RenameTextBox = name;
            }
            else
            {
                _viewModelLocator.MainViewModel.RenameTextBox = selectedEntity.Name;
            }
            entity_rename_popup.IsOpen = true;
        }

        private void Rename_Ok_Click(object sender, RoutedEventArgs e)
        {
            entity_rename_popup.IsOpen = false;
            if (_viewModelLocator.MainViewModel.RenameTextBox.Equals(""))
            {
                _informerManagerLocator.InformerManager.AddMessage("Error", "You have to give a name to the file");
            }
            else
            {
                BindingExpression expression = entity_rename_textbox.GetBindingExpression(TextBox.TextProperty);
                expression.UpdateSource();
                var selectedEntity = _viewModelLocator.MainViewModel.SelectedEntity;
                if (selectedEntity.GetType().Equals(typeof(File)))
                {
                    var nameParts = selectedEntity.Name.Split('.');
                    string extension = nameParts.Last();
                    _viewModelLocator.MainViewModel.SelectedEntity.Name = _viewModelLocator.MainViewModel.RenameTextBox + "." + extension;
                }
                else
                {
                    _viewModelLocator.MainViewModel.SelectedEntity.Name = _viewModelLocator.MainViewModel.RenameTextBox;
                }
                _viewModelLocator.MainViewModel.UpdateEntity(_viewModelLocator.MainViewModel.SelectedEntity);
            }
        }

        private void Rename_Cancel_Click(object sender, RoutedEventArgs e)
        {
            entity_rename_popup.IsOpen = false;
        }

        private void Entity_Delete_Click(object sender, RoutedEventArgs e)
        {
            _viewModelLocator.MainViewModel.DeleteEntity(_viewModelLocator.MainViewModel.SelectedEntity);
        }

        private void CreateFolder_Ok_Click(object sender, RoutedEventArgs e)
        {
            folder_create_popup.IsOpen = false;
            if (folder_create_textbox.Text.Equals(""))
            {
                _informerManagerLocator.InformerManager.AddMessage("Error", "You have to give a name to the folder");
            }
            else
            {
                _viewModelLocator.MainViewModel.CreateFolder(folder_create_textbox.Text);
            }
            folder_create_textbox.Text = "";
        }

        private void CreateFolder_Cancel_Click(object sender, RoutedEventArgs e)
        {
            folder_create_textbox.Text = "";
            folder_create_popup.IsOpen = false;
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            if (this.folder_create_popup.IsOpen)
            {
                folder_create_popup.IsOpen = false;
                e.Cancel = true;
            }
            else if (this.entity_rename_popup.IsOpen)
            {
                entity_rename_popup.IsOpen = false;
                e.Cancel = true;
            }
            else if (this.entity_upload_popup.IsOpen)
            {
                entity_upload_popup.IsOpen = false;
                e.Cancel = true;
            }
            else if (this.entity_share_popup.IsOpen)
            {
                entity_share_popup.IsOpen = false;
                e.Cancel = true;
            }
            else if (this.entity_share_popup.IsOpen)
            {
                entity_share_popup.IsOpen = false;
                e.Cancel = true;
            }
            else if (this.Loading_Layer.Visibility.Equals(Visibility.Visible))
            {
                _viewModelLocator.MainViewModel.CancelBlockingDownload();
                this.Loading_Layer.Visibility = Visibility.Collapsed;
                e.Cancel = true;
            }
            else if (_viewModelLocator.MainViewModel.CanGoBack())
            {
                _viewModelLocator.MainViewModel.GoBack();
                e.Cancel = true;
            }
            else
            {
                base.OnBackKeyPress(e);
            }

        }

        private void Entity_Share_Click(object sender, RoutedEventArgs e)
        {
            var selectedEntity = _viewModelLocator.MainViewModel.SelectedEntity;
            var anonymousLink = _viewModelLocator.MainViewModel.GetAnonymousShareLink(selectedEntity);
            //entity_share_popup_anonymousLink.NavigateUri = anonymousLink;
            entity_share_popup_anonymousLink.Text = anonymousLink.ToString();
            entity_share_popup.IsOpen = true;
        }

        private void EntityShare_Ok_Click(object sender, RoutedEventArgs e)
        {
            var selectedEntity = _viewModelLocator.MainViewModel.SelectedEntity;
            ListPickerItem permissionPicker = entity_share_permission.SelectedItem as ListPickerItem;
            string permission = permissionPicker.Content.ToString().Replace(" & ", "_").ToLower();
            _viewModelLocator.MainViewModel.ShareEntity(selectedEntity, entity_share_textbox.Text, permission);
            entity_share_popup.IsOpen = false;
        }

        private void EntityShare_Cancel_Click(object sender, RoutedEventArgs e)
        {
            entity_share_popup.IsOpen = false;
        }

        private void Entity_SharedUsers_Click(object sender, RoutedEventArgs e)
        {
            var selectedEntity = _viewModelLocator.MainViewModel.SelectedEntity;
            if (selectedEntity.GetType().Equals(typeof(File)))
            {
                _informerManagerLocator.InformerManager.AddMessage("Sorry", "Unable to do this on a file for the moment");
            }
            else
            {
                _viewModelLocator.MainViewModel.GetSharedUsers(selectedEntity);
                entity_sharedUsers_popup.IsOpen = true;
            }
        }

        private void Entity_sharedUsers_Ok_Click(object sender, RoutedEventArgs e)
        {
            entity_sharedUsers_popup.IsOpen = false;
        }

        private void Entity_sharedUsers_Cancel_Click(object sender, RoutedEventArgs e)
        {
            entity_sharedUsers_popup.IsOpen = false;
        }

        private void Entity_Cut_Click(object sender, RoutedEventArgs e)
        {
            var selectedEntity = _viewModelLocator.MainViewModel.SelectedEntity;
            if (selectedEntity.GetType().Equals(typeof(Folder)))
            {
                _informerManagerLocator.InformerManager.AddMessage("Sorry", "Unable to do this on a folder for the moment");
            }
            else
            {
                _viewModelLocator.MainViewModel.CutEntity(selectedEntity);
                var pasteMenuItem = ApplicationBar.MenuItems[0];
                (pasteMenuItem as ApplicationBarMenuItem).IsEnabled = true;
            }
        }

        private void Upload_Ok_Click(object sender, RoutedEventArgs e)
        {
            entity_upload_popup.IsOpen = false;
            //_viewModelLocator.MainViewModel.UploadFile();
            _viewModelLocator.MainViewModel.UploadFileByBackgroundTranfer();
        }

        private void Upload_Cancel_Click(object sender, RoutedEventArgs e)
        {
            entity_upload_popup.IsOpen = false;
        }

        void InformerManager_NewMessage(object sender, EventArgs e)
        {
            var message = _informerManagerLocator.InformerManager.GetMessages().Last();
            MessageBox.Show(message.Content, message.Title, MessageBoxButton.OK);
        }

        private void LinkTest_Click(object sender, RoutedEventArgs e)
        {
            var hyperlinkButton = sender as HyperlinkButton;
            string email = hyperlinkButton.Tag.ToString();
            _viewModelLocator.MainViewModel.DeletePermission(email);
        }

        private void Entity_Download_Click(object sender, RoutedEventArgs e)
        {
            var selectedEntity = _viewModelLocator.MainViewModel.SelectedEntity;
            if (selectedEntity.GetType().Equals(typeof(Folder)))
            {
                _informerManagerLocator.InformerManager.AddMessage("Sorry", "Downloading a folder is not possible for the moment.");
            }
            else
            {
                _viewModelLocator.MainViewModel.DownloadFile(selectedEntity as File);
            }
        }

        private void Button_UploadPicture_Click(object sender, EventArgs e)
        {
            photoChooserTask.Show();
        }

        void PhotoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult != TaskResult.OK)
            {
                return;
            }
            _viewModelLocator.MainViewModel.SelectedEntity = new File();
            _viewModelLocator.MainViewModel.UploadTextBox = e.OriginalFileName.Split('\\').Last();
            _viewModelLocator.MainViewModel.UploadLocation = e.OriginalFileName;
            this.entity_upload_popup.IsOpen = true;
            _viewModelLocator.MainViewModel.EntityToUploadStream = e.ChosenPhoto;
        }
    }

   
}