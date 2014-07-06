using Cubbyhole.Models;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System;

namespace Cubbyhole.WP.ViewModels
{
    public partial class MainViewModel
    {
        private DiskEntity _selectedLocalEntity { get; set; }
        public DiskEntity SelectedLocalEntity
        {
            get
            {
                return _selectedLocalEntity;
            }
            set
            {
                _selectedLocalEntity = value;
            }
        }

        public async void GetLocalEntities()
        {
            LocalEntities.Clear();
            var localFiles = await ApplicationData.Current.LocalFolder.GetFilesAsync();
            foreach (var localFile in localFiles)
            {
                if (!localFile.Name.Equals("__ApplicationSettings"))
                {
                    var file = new File();
                    file.CreationDate = localFile.DateCreated.DateTime;
                    var nameAndContentType = localFile.Name.Split('~');
                    file.ContentType = nameAndContentType[1];
                    file.Name = nameAndContentType[2];
                    //file.Path = localFile.Attributes;
                    var test = localFile.Attributes;
                    file.Path = localFile.Path.Split('\\').Last();
                    var fileWithIcon = DetermineIcon(file);
                    LocalEntities.Add(fileWithIcon);
                }
            }
        }

        public async void ProceedWithItemLocal(int entityIndex)
        {
            if (LocalEntities[entityIndex].GetType().Equals(typeof(File)))
            {
                var file = LocalEntities[entityIndex] as File;
                var storageFile = await ApplicationData.Current.LocalFolder.GetFileAsync(file.Path); 
                await Launcher.LaunchFileAsync(storageFile);
            }
            else
            {
                // NO folder in Local directory, I'm lazy
            }
        }

        public void SelectLocalEntity(DiskEntity selectedEntity)
        {
            SelectedLocalEntity = LocalEntities.Where(e => e.CreationDate.Equals(selectedEntity.CreationDate)).FirstOrDefault();
        }

        public void DeleteLocalEntity(DiskEntity selectedLocalEntity)
        {
            if (selectedLocalEntity.GetType().Equals(typeof(File)))
            {
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    store.DeleteFile((selectedLocalEntity as File).Path);
                }
            }
            else
            {
                // No folders in local storage, I'm lazy
            }
            LocalEntities.Remove(selectedLocalEntity);
        }

        public void UpdateLocalEntity(DiskEntity entity)
        {
            if (entity.GetType().Equals(typeof(File)))
            {
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    string oldName = (entity as File).Path.Split('~').Last();
                    string finalDestination = (entity as File).Path.Replace(oldName, entity.Name);
                    store.MoveFile((entity as File).Path, finalDestination);
                    GetLocalEntities();
                }
            }
        }
    }
}
