using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cubbyhole.Models;
using Cubbyhole.API;
using Cubbyhole.WP.Resources;
using Cubbyhole.WP.Utils;
using Microsoft.Phone.Net.NetworkInformation;
using System.IO.IsolatedStorage;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Phone.BackgroundTransfer;
using Windows.Storage;
using Windows.System;
using Windows.Storage.Streams;

namespace Cubbyhole.WP.ViewModels
{
    public delegate void BlockingLoadingEventHandler(object sender, EventArgs e, bool isLoading);

    public delegate void SelectedEntityChangedEventHandler(object sender, EventArgs e, bool isFolder);

    public partial class MainViewModel : INotifyPropertyChanged
    {
        private InformerManagerLocator _informerManagerLocator { get; set; }
        private InternetConnectionManagerLocator _internetConnectionManagerLocator { get; set; }
        private User _user { get; set; }
        private FolderAPI _folderApi { get; set; }
        private FileAPI _fileApi { get; set; }
        private Folder _rootFolder { get; set; }
        private int _currentFolderId { get; set; }
        private List<int> _foldersIdTree { get; set; }

        private DiskEntity _selectedEntity { get; set; }
        private DiskEntity _copiedOrCutedEntity { get; set; }
        private bool _isCopyAction { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        private string _renameTextBox;
        public string RenameTextBox
        {
            get { return _renameTextBox; }
            set
            {
                _renameTextBox = value;
                NotifyPropertyChanged();
            }
        }
        public System.IO.Stream EntityToUploadStream { get; set; }

        private string _uploadTextBox;
        public string UploadTextBox
        {
            get { return _uploadTextBox; }
            set
            {
                _uploadTextBox = value;
                NotifyPropertyChanged();
            }
        }
        public string UploadLocation { get; set; }

        public ObservableCollection<DiskEntity> Entities { get; private set; }
        public ObservableCollection<DiskEntity> LocalEntities { get; private set; }
        public ObservableCollection<Permission> Permissions { get; private set; }

        public event BlockingLoadingEventHandler BlockingLoading;

        public event SelectedEntityChangedEventHandler SelectedEntityChanged;

        public MainViewModel()
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains("user") == false)
            {

            }
            else
            {
                _informerManagerLocator = new InformerManagerLocator();
                _internetConnectionManagerLocator = new InternetConnectionManagerLocator();
                _internetConnectionManagerLocator.InternetConnectionManager.ConnectionStateChanged += InternetConnectionManager_ConnectionStateChanged;

                _folderApi = new FolderAPI(new Uri(AppResources.BaseAddress));
                _folderApi.ErrorReceived += _folderApi_ErrorReceived;

                _fileApi = new FileAPI(new Uri(AppResources.BaseAddress));
                _fileApi.ErrorReceived += _fileApi_ErrorReceived;

                _foldersIdTree = new List<int>();

                Entities = new ObservableCollection<DiskEntity>();
                LocalEntities = new ObservableCollection<DiskEntity>();
                Permissions = new ObservableCollection<Permission>();

                //transferRequests = new ObservableCollection<BackgroundTransferRequest>();
                transferRequests = new ObservableCollection<CustomBackgroundTransferRequest>();

                using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (!isoStore.DirectoryExists("/shared/transfers"))
                    {
                        isoStore.CreateDirectory("/shared/transfers");
                    }
                }

                InitDownload();
            }
        }
       
        public async void Init()
        {
            Entities.Clear();
            _user = (User)IsolatedStorageSettings.ApplicationSettings["user"];
            _rootFolder = await _folderApi.GetRootFolder(_user.Token);
            _currentFolderId = _rootFolder.Id;
            GetEntitites();
            GetLocalEntities();
        }

        public async void GetEntitites()
        {
            Entities.Clear();

            //List<Folder> folders = await API.Mock.DiskEntityMock.GetFakesFolders();
            //List<File> files = await API.Mock.DiskEntityMock.GetFakesFiles();
            List<Folder> folders = await _folderApi.GetSubFolders(_currentFolderId, _user.Token);
            foreach (var folder in folders)
            {
                var folderTest = DetermineIcon(folder);
                Entities.Add(folderTest);
            }

            List<File> files = await _fileApi.GetFiles(_currentFolderId, _user.Token);
            foreach (var file in files)
            {
                var filerTest = DetermineIcon(file);
                Entities.Add(filerTest);
            }
        }

        public async void CreateFolder(string folderName)
        {
            Folder newFolder = new Folder();
            newFolder.ParentId = _rootFolder.Id;
            newFolder.Name = folderName;
            Folder folderResult = await _folderApi.CreateRootFolder(newFolder, _user.Token);
            var foldertest = DetermineIcon(folderResult);
            Entities.Add(foldertest);
        }

        public void Logout()
        {
            IsolatedStorageSettings.ApplicationSettings.Remove("user");
            ((PhoneApplicationFrame)Application.Current.RootVisual).Navigate(new Uri("/LoginRegisterViewModel.xaml",
                                                                                              UriKind.Relative));
        }

        public async void PasteEntity()
        {
            if (_isCopyAction)
            {
                // Not supported by the API, if I want to copy a file I have to download it and upload it again
                // This is stupid
            }
            else
            {
                (_copiedOrCutedEntity as File).FolderId = _currentFolderId;
                var newFile = await _fileApi.UpdateFile(_copiedOrCutedEntity as File, _user.Token);
                var fileWithIcon = DetermineIcon(newFile);
                Entities.Add(fileWithIcon);
                _copiedOrCutedEntity = null;
            }
        }

        public async void ProceedWithItem(int entityIndex)
        {
            if (Entities[entityIndex].GetType().Equals(typeof(Folder)))
            {
                // If it's a folder we enter inside
                _foldersIdTree.Add(_currentFolderId);
                _currentFolderId = Entities[entityIndex].Id;
                GetEntitites();
            }
            else
            {
                try
                {
                    BlockingLoading(this, new EventArgs(), true);
                    var httpStream = await _fileApi.DownloadFile(Entities[entityIndex] as File, _user.Token);

                    using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                    {

                        var storageFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("temp", CreationCollisionOption.OpenIfExists);


                        var extension = Entities[entityIndex].Name.Split('.').Last();
                        IsolatedStorageFileStream stream = new IsolatedStorageFileStream("temp/tempFile." + extension, System.IO.FileMode.OpenOrCreate, store);
                        httpStream.CopyTo(stream);
                        stream.Close();

                        var storageFile = await storageFolder.GetFileAsync("tempFile." + extension);
                        await Launcher.LaunchFileAsync(storageFile);
                    }
                }
                catch (Exception)
                {
                    _informerManagerLocator.InformerManager.AddMessage("Error", "Unable to open this file");
                }
                finally
                {
                    BlockingLoading(this, new EventArgs(), false);
                }
            }
        }

        public void CancelBlockingDownload()
        {
            _fileApi.DownloadFileClient.CancelPendingRequests();
        }

        public bool CanGoBack()
        {
            if (_foldersIdTree.Count >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void GoBack()
        {
            _currentFolderId = _foldersIdTree.Last();
            _foldersIdTree.RemoveAt(_foldersIdTree.Count - 1);
            GetEntitites();
        }

        public DiskEntity SelectedEntity
        {
            get
            { 
                return _selectedEntity; 
            }
            set 
            { 
                _selectedEntity = value;
                if (SelectedEntityChanged != null)
                {
                    SelectedEntityChanged(this, new EventArgs(), _selectedEntity.GetType().Equals(typeof(Folder)) ? true : false);
                }
            }
        }

        public async void UpdateEntity(DiskEntity entity)
        {
            DiskEntity newEntity = null;
            if (entity.GetType().Equals(typeof(Folder)))
            {
                newEntity = await _folderApi.UpdateFolder(entity as Folder, _user.Token);
            }
            else
            {
                newEntity = await _fileApi.UpdateFile(entity as File, _user.Token);
            }
            if (newEntity != null)
            {
                newEntity = DetermineIcon(newEntity);
                int index = Entities.IndexOf(Entities.Where(e => e.Id.Equals(entity.Id)).FirstOrDefault());
                Entities.RemoveAt(index);
                Entities.Insert(index, newEntity);
            }
        }

        void InternetConnectionManager_ConnectionStateChanged(object sender, NetworkNotificationEventArgs e)
        {
            if (_internetConnectionManagerLocator.InternetConnectionManager.IsInternetConnectionAvailable() == false)
            {
                _informerManagerLocator.InformerManager.AddMessage("Error", "Connection lost");
            }
        }

        void _folderApi_ErrorReceived(object sender, ErrorEventArgs e)
        {
            _informerManagerLocator.InformerManager.AddMessage(e.Error.Title, e.Error.Message);
        }

        void _fileApi_ErrorReceived(object sender, ErrorEventArgs e)
        {
            _informerManagerLocator.InformerManager.AddMessage(e.Error.Title, e.Error.Message);
        }


        private DiskEntity DetermineIcon(DiskEntity entity)
        {
            if (entity.GetType().Equals(typeof(Folder)))
            {
                entity.Icon = new BitmapImage(new Uri("/Resources/Images/folder.png", UriKind.Relative));
            }
            else
            {
                if (((File)entity).ContentType.StartsWith("application"))
                {
                    entity.Icon = new BitmapImage(new Uri("/Resources/Images/applicationFile.png", UriKind.Relative));
                }
                else if (((File)entity).ContentType.StartsWith("audio"))
                {
                    entity.Icon = new BitmapImage(new Uri("/Resources/Images/audioFile.png", UriKind.Relative));
                }
                else if (((File)entity).ContentType.StartsWith("image"))
                {
                    entity.Icon = new BitmapImage(new Uri("/Resources/Images/imageFile.png", UriKind.Relative));
                }
                else if (((File)entity).ContentType.StartsWith("text"))
                {
                    entity.Icon = new BitmapImage(new Uri("/Resources/Images/textFile.png", UriKind.Relative));
                }
                else if (((File)entity).ContentType.StartsWith("video"))
                {
                    entity.Icon = new BitmapImage(new Uri("/Resources/Images/videoFile.png", UriKind.Relative));
                }
                else
                {
                    entity.Icon = new BitmapImage(new Uri("/Resources/Images/unknownFile.png", UriKind.Relative));
                }
            }
            return entity;
        }

        public void SelectEntity(DiskEntity selectedEntity)
        {
            SelectedEntity = Entities.Where(e=>e.Id.Equals(selectedEntity.Id)).FirstOrDefault();
        }

        public async void DeleteEntity(DiskEntity selectedEntity)
        {
           var entity = Entities.Where(e => e.Id.Equals(selectedEntity.Id)).FirstOrDefault();
           if (entity.GetType().Equals(typeof(File)))
           {
               bool result = await _fileApi.DeleteFile(entity as File, _user.Token);
           }
           else
           {
               bool result = await _folderApi.DeleteFolder(entity as Folder, _user.Token);
           }
           // If it's true the entity has just been removed
           // If it's false it means the entity has already been removed
           // So in both cases we remove it from the list
           Entities.Remove(entity);
        }

        public Uri GetAnonymousShareLink(DiskEntity selectedEntity)
        {
            if (selectedEntity.GetType().Equals(typeof(Folder)))
            {
                return _folderApi.GetAnonymousShareLink(selectedEntity as Folder);
            }
            else
            {
                return _fileApi.GetAnonymousShareLink(selectedEntity as File);
            }
        }

        public async void ShareEntity(DiskEntity selectedEntity, string recipientEmail, string permissionString)
        {
            PermissionRight permission = (PermissionRight)Enum.Parse(typeof(PermissionRight), permissionString);
            if (selectedEntity.GetType().Equals(typeof(Folder)))
            {
                var result = await _folderApi.ShareFolder(selectedEntity as Folder, recipientEmail, permission, _user.Token);
            }
            else
            {
                var result = await _fileApi.ShareFile(selectedEntity as File, recipientEmail, permission, _user.Token);
            }
        }

        public async void GetSharedUsers(DiskEntity selectedEntity)
        {
            if (selectedEntity.GetType().Equals(typeof(Folder)))
            {
                List<Permission> whyCantIDoItInOnlyOneStep = await _folderApi.GetSharedUsers(selectedEntity as Folder, _user.Token);
                Permissions.Clear();
                foreach (var permission in whyCantIDoItInOnlyOneStep)
                {
                    Permissions.Add(permission);
                }
            }
            else
            {
                List<Permission> whyCantIDoItInOnlyOneStep = await _fileApi.GetSharedUsers(selectedEntity as File, _user.Token);
                Permissions = new ObservableCollection<Permission>(whyCantIDoItInOnlyOneStep);
            }
        }

        public async void DeletePermission(string recipientEmail)
        {
            var permission = Permissions.Where(p => p.Email.Equals(recipientEmail)).FirstOrDefault();
            if (permission != null)
            {
                bool result = false;
                if (SelectedEntity.GetType().Equals(typeof(Folder)))
                {
                   result = await _folderApi.DeletePermission(permission, _user.Token);
                }
                else
                {

                }
                if (result == true)
                {
                    Permissions.Remove(permission);
                }
            }
        }

        public void CutEntity(DiskEntity selectedEntity)
        {
            if (selectedEntity.GetType().Equals(typeof(File)))
            {
                _copiedOrCutedEntity = selectedEntity;
            }
        }


        public void DownloadFile(File file)
        {
            if (BackgroundTransferService.Requests.Count() >= 5)
            {
                _informerManagerLocator.InformerManager.AddMessage("Error", "The maximum number of background file transfer requests for this application has been exceeded");
            }
            //var toto = await _fileApi.DownloadFile(file, _user.Token);


            Uri transferUri = _fileApi.GetDownloadUrl(file);
            BackgroundTransferRequest transferRequest = new BackgroundTransferRequest(transferUri);

            // Set the transfer method. GET and POST are supported.
            transferRequest.Method = "GET";
            transferRequest.Headers["Authorization"] = _user.Token;
            Uri downloadUri = new Uri("shared/transfers/" + file.Name, UriKind.RelativeOrAbsolute);
            transferRequest.DownloadLocation = downloadUri;
            transferRequest.Tag = file.Size + "~" + file.ContentType.Split('/').First() + "~" + file.Name;
            transferRequest.TransferPreferences = TransferPreferences.AllowCellularAndBattery;
            try
            {
                BackgroundTransferService.Add(transferRequest);
            }
            catch (InvalidOperationException ex)
            {
                _informerManagerLocator.InformerManager.AddMessage("Unable to add background transfer request", ex.Message);
            }
            catch (Exception)
            {
                _informerManagerLocator.InformerManager.AddMessage("Error", "Unable to add background transfer request.");
            }
            if (transferRequest != null)
            {
                transferRequest.TransferProgressChanged += transfer_TransferProgressChanged;
                transferRequest.TransferStatusChanged += transfer_TransferStatusChanged;
                //this.transferRequests.ToList().Add(transferRequest);
                UpdateRequestsList();
            }
        }

        public async void UploadFile()
        {
           /* ApplicationData.Current.LocalFolder.CreateFileAsync()
           var storageFile = await ApplicationData.Current.LocalFolder.GetFileAsync(storagepath);
            await Launcher.LaunchFileAsync(storageFile)*/
            if (UploadTextBox.Equals(""))
            {
                _informerManagerLocator.InformerManager.AddMessage("Error", "You have to specify a name");
            }
            else
            {
                var file = await _fileApi.UploadFile(EntityToUploadStream, UploadTextBox, "image/jpeg", _currentFolderId, _user.Token);
                EntityToUploadStream = null;
                var fileWithIcon = DetermineIcon(file);
                Entities.Add(file);
            }           
        }

        public void UploadFileByBackgroundTranfer()
        {
            if (BackgroundTransferService.Requests.Count() >= 5)
            {
                _informerManagerLocator.InformerManager.AddMessage("Error", "The maximum number of background file transfer requests for this application has been exceeded");
                return;
            }

            var boundary = Guid.NewGuid().ToString();
            var mimeType = "image/jpeg";
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (isoStore.FileExists("shared/transfers/" + UploadTextBox))
                {
                    isoStore.DeleteFile("shared/transfers/" + UploadTextBox);
                }
                IsolatedStorageFileStream fileStream = isoStore.CreateFile("shared/transfers/" + UploadTextBox);

                // Define header of the multipart form data concerning the body of the request
                // We have to do it because it's possible to upload several files
                StringBuilder content = new StringBuilder();
                content.AppendLine(String.Format("--{0}", boundary));
                content.AppendLine(String.Format("Content-Disposition: form-data; name=\"file\"; filename=\"{0}\"", UploadTextBox));
                content.AppendLine(String.Format("Content-Type: {0}", mimeType));

                content.AppendLine();

                // Writing the header in the request
                byte[] contentAsBytes = Encoding.UTF8.GetBytes(content.ToString());
                fileStream.Write(contentAsBytes, 0, contentAsBytes.Length);

                content.Clear();

                EntityToUploadStream.Position = 0;

                byte[] buffer = new byte[16 * 1024];
                int read;
                while ((read = EntityToUploadStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    fileStream.Write(buffer, 0, read);
                }

                content.AppendLine();

                // Footer of body
                content.AppendLine(String.Format("--{0}--", boundary));

                contentAsBytes = Encoding.UTF8.GetBytes(content.ToString());
                fileStream.Write(contentAsBytes, 0, contentAsBytes.Length);

                fileStream.Close();
                EntityToUploadStream = null;
            }

            Uri transferUri = _fileApi.GetUploadUrl(_currentFolderId);
            BackgroundTransferRequest transferRequest = new BackgroundTransferRequest(transferUri);

            transferRequest.Method = "POST";
            transferRequest.Headers["Authorization"] = _user.Token;

            // Headet of the multi part request
            transferRequest.Headers["Content-Type"] = string.Format("multipart/form-data; boundary={0}", boundary);
            transferRequest.UploadLocation = new Uri("shared/transfers/" + UploadTextBox, UriKind.Relative);
            transferRequest.Tag = UploadTextBox;
            transferRequest.TransferPreferences = TransferPreferences.AllowCellularAndBattery;
            try
            {
                BackgroundTransferService.Add(transferRequest);
            }
            catch (InvalidOperationException ex)
            {
                _informerManagerLocator.InformerManager.AddMessage("Unable to add background transfer request", ex.Message);
            }
            catch (Exception)
            {
                _informerManagerLocator.InformerManager.AddMessage("Error", "Unable to add background transfer request.");
            }
            if (transferRequest != null)
            {
                transferRequest.TransferProgressChanged += transfer_TransferProgressChanged;
                transferRequest.TransferStatusChanged += transfer_TransferStatusChanged;
                UpdateRequestsList();
            }
        }
    }
}
