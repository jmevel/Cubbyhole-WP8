using Cubbyhole.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Cubbyhole.API
{
    public class FolderAPI
    {
        private Uri BaseAddress { get; set; }
        public event ErrorReceivedEventHandler ErrorReceived;

        public FolderAPI(Uri baseAddress)
        {
            BaseAddress = new Uri(baseAddress + "folders/");
        }

        public async Task<Folder> GetRootFolder(string token)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(this.BaseAddress + "root/");

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token);

            HttpResponseMessage response = await client.GetAsync(client.BaseAddress);
            string responseContent = await response.Content.ReadAsStringAsync();
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                if (ErrorReceived != null)
                {
                    var newError = new Error(true, responseContent);
                    this.ErrorReceived(this, new ErrorEventArgs(newError));
                    return null;
                }
            }

            Folder result = null;
            try
            {
                result = JsonConvert.DeserializeObject<Folder>(responseContent);
            }
            catch (Exception)
            {
                if (ErrorReceived != null)
                {
                    var newError = new Error(true, responseContent);
                    this.ErrorReceived(this, new ErrorEventArgs(newError));
                    return null;
                }
            }
            return result;
        }

        public async Task<List<Folder>> GetSubFolders(int folderId, string token)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(this.BaseAddress.ToString() + folderId + "/subfolders/");

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token);

            HttpResponseMessage response = await client.GetAsync(client.BaseAddress);
            string responseContent = await response.Content.ReadAsStringAsync();
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                if (ErrorReceived != null)
                {
                    var newError = new Error(true, responseContent);
                    this.ErrorReceived(this, new ErrorEventArgs(newError));
                    return null;
                }
            }

            List<Folder> result = null;
            try
            {
                result = JsonConvert.DeserializeObject<List<Folder>>(responseContent);
            }
            catch (Exception)
            {
                if (ErrorReceived != null)
                {
                    var newError = new Error(true, responseContent);
                    this.ErrorReceived(this, new ErrorEventArgs(newError));
                    return null;
                }
            }
            return result;
        }

        public async Task<Folder> CreateRootFolder(Folder folder, string token)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = this.BaseAddress;

            var content = JsonConvert.SerializeObject(folder);
            HttpContent httpContent = new StringContent(content, Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token);

            HttpResponseMessage response = await client.PostAsync(client.BaseAddress, httpContent);
            string responseContent = await response.Content.ReadAsStringAsync();
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                if (ErrorReceived != null)
                {
                    var newError = new Error(true, responseContent);
                    this.ErrorReceived(this, new ErrorEventArgs(newError));
                    return null;
                }
            }

            Folder result = null;
            try
            {
                result = JsonConvert.DeserializeObject<Folder>(responseContent);
            }
            catch (Exception)
            {
                if (ErrorReceived != null)
                {
                    var newError = new Error(true, responseContent);
                    this.ErrorReceived(this, new ErrorEventArgs(newError));
                    return null;
                }
            }
            return result;
        }

        public async Task<Folder> UpdateFolder(Folder folder, string token)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(this.BaseAddress.ToString() + folder.Id + "/");

            var content = JsonConvert.SerializeObject(folder);
            HttpContent httpContent = new StringContent(content, Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token);

            HttpResponseMessage response = await client.PutAsync(client.BaseAddress, httpContent);
            string responseContent = await response.Content.ReadAsStringAsync();
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                if (ErrorReceived != null)
                {
                    var newError = new Error(true, responseContent);
                    this.ErrorReceived(this, new ErrorEventArgs(newError));
                    return null;
                }
            }

            Folder result = null;
            try
            {
                result = JsonConvert.DeserializeObject<Folder>(responseContent);
            }
            catch (Exception)
            {
                if (ErrorReceived != null)
                {
                    var newError = new Error(true, responseContent);
                    this.ErrorReceived(this, new ErrorEventArgs(newError));
                    return null;
                }
            }
            return result;
        }

        public async Task<bool> DeleteFolder(Folder folder, string token)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(this.BaseAddress.ToString() + folder.Id + "/");

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token);

            HttpResponseMessage response = await client.DeleteAsync(client.BaseAddress);
            string responseContent = await response.Content.ReadAsStringAsync();
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                if (ErrorReceived != null)
                {
                    var newError = new Error(true, responseContent);
                    this.ErrorReceived(this, new ErrorEventArgs(newError));
                    return false;
                }
            }
            return true;
        }

        public Uri GetAnonymousShareLink(Folder folder)
        {
            return new Uri(this.BaseAddress.ToString().Replace("/folders/", ":8080/anon/") + folder.Url + "/");            
        }

        public async Task<bool> ShareFolder(Folder folder, string recipientEmail, PermissionRight permission, string token)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(this.BaseAddress.ToString().Replace("folders/", "sharedfolders/"));

            var content = "{\"folder_id\":" + folder.Id + ",\"email\":\"" + recipientEmail + "\",\"permission\":\"" + permission + "\"}";
            HttpContent httpContent = new StringContent(content, Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token);

            HttpResponseMessage response = await client.PostAsync(client.BaseAddress, httpContent);
            string responseContent = await response.Content.ReadAsStringAsync();
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                if (ErrorReceived != null)
                {
                    var newError = new Error(true, responseContent);
                    this.ErrorReceived(this, new ErrorEventArgs(newError));
                    return false;
                }
            }
            return true;
        }

        public async Task<List<Permission>> GetSharedUsers(Folder folder, string token)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(this.BaseAddress.ToString() + folder.Id + "/share/");           
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token);

            HttpResponseMessage response = await client.GetAsync(client.BaseAddress);
            string responseContent = await response.Content.ReadAsStringAsync();
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                if (ErrorReceived != null)
                {
                    var newError = new Error(true, responseContent);
                    this.ErrorReceived(this, new ErrorEventArgs(newError));
                    return null;
                }
            }
            List<Permission> result = null;
            try
            {
                result = JsonConvert.DeserializeObject<List<Permission>>(responseContent);
            }
            catch (Exception)
            {
                if (ErrorReceived != null)
                {
                    var newError = new Error(true, responseContent);
                    this.ErrorReceived(this, new ErrorEventArgs(newError));
                    return null;
                }
            }
            return result;
        }

        public async Task<bool> DeletePermission(Permission permission, string token)
        {

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(this.BaseAddress.ToString().Replace("folders/", "sharedfolders/") + permission.Id);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token);

            HttpResponseMessage response = await client.DeleteAsync(client.BaseAddress);
            string responseContent = await response.Content.ReadAsStringAsync();
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                if (ErrorReceived != null)
                {
                    var newError = new Error(true, responseContent);
                    this.ErrorReceived(this, new ErrorEventArgs(newError));
                    return false;
                }
            }
            return true;
        }

        /*public async Task<string> GetResourcesByAnonymous(Folder folder)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(this.BaseAddress.ToString().Replace("folders/", "resources/") + folder.Url + "/anonymous/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.GetAsync(client.BaseAddress);
            string responseContent = await response.Content.ReadAsStringAsync();
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                if (ErrorReceived != null)
                {
                    var newError = new Error(true, responseContent);
                    this.ErrorReceived(this, new ErrorEventArgs(newError));
                    return null;
                }
            }
            return responseContent;
        }    */   
    }
}
