using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cubbyhole.Models;

namespace Cubbyhole.API.Mock
{
    #pragma warning disable
    public class DiskEntityMock
    {
        public async static Task<List<Folder>> GetFakesFolders()
        {
            var folders = new List<Folder>();

            Folder folder1 = new Folder();
            folder1.CreationDate = DateTime.Now;
            folder1.UpdateDate = DateTime.Now;
            folder1.Name = "Documents";
            folder1.Permission = PermissionRight.read_write;
            //folder1.Size = 2546978230;
            List<File> files = new List<File>();
            File file1 = new File();
            File file2 = new File();
            files.Add(file1);
            files.Add(file2);
            folder1.Files = files;

            Folder folder2 = new Folder();
            folder2.CreationDate = DateTime.Now;
            folder2.UpdateDate = DateTime.Now;
            folder2.Name = "Shared";
            folder2.Permission = PermissionRight.read_write;
            //folder2.Size = 45789362145;
            List<File> files2 = new List<File>();
            File file12 = new File();
            File file22 = new File();
            files.Add(file12);
            files.Add(file22);
            folder2.Files = files;

            folders.Add(folder1);
            folders.Add(folder2);
            return folders;
        }

        public async static Task<List<File>> GetFakesFiles()
        {
            List<File> files = new List<File>();

            File file1 = new File();
            file1.CreationDate = DateTime.Now;
            file1.UpdateDate = DateTime.Now;
            file1.Name = "file 1";
            file1.Permission = PermissionRight.read_write;
            file1.Size = 5030;
            files.Add(file1);

            File file2 = new File();
            file2.CreationDate = DateTime.Now;
            file2.UpdateDate = DateTime.Now;
            file2.Name = "file 2";
            file2.Permission = PermissionRight.read_write;
            file2.Size = 7302;
            files.Add(file2);


            File file3 = new File();
            file3.CreationDate = DateTime.Now;
            file3.UpdateDate = DateTime.Now;
            file3.Name = "file 3";
            file3.Permission = PermissionRight.read_write;
            file3.Size = 7302;
            files.Add(file3);

            return files;
        }
    }
}
