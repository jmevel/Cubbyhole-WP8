using Cubbyhole.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cubbyhole.API
{
    public class FileAPI
    {
        private Uri BaseAddress { get; set; }
        public event ErrorReceivedEventHandler ErrorReceived;

        // To cancel the downloading request if needed
        public HttpClient DownloadFileClient { get; private set; }

        public FileAPI(Uri baseAddress)
        {
            BaseAddress = baseAddress;
        }

        public async Task<List<File>> GetFiles(int folderId, string token)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(this.BaseAddress.ToString() + "folders/" + folderId + "/resources/");

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

            List<File> result = null;
            try
            {
                result = JsonConvert.DeserializeObject<List<File>>(responseContent);
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

        public async Task<File> UpdateFile(File file, string token)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(this.BaseAddress.ToString() + "resources/" + file.Id + "/");

            var content = JsonConvert.SerializeObject(file);
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

            File result = null;
            try
            {
                result = JsonConvert.DeserializeObject<File>(responseContent);
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

        public async Task<bool> DeleteFile(File file, string token)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(this.BaseAddress.ToString() + "resources/" + file.Id + "/");

            var content = JsonConvert.SerializeObject(file);
            HttpContent httpContent = new StringContent(content, Encoding.UTF8, "application/json");
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

        public Uri GetAnonymousShareLink(File file)
        {
            return new Uri(this.BaseAddress.ToString() + ":8080/anondl/" + file.Url + "/");
        }

        public async Task<bool> ShareFile(File file, string recipientEmail, PermissionRight permission, string token)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(this.BaseAddress.ToString() + "sharedresources/");

            var content = "{\"resource_id\":" + file.Id + ",\"email\":\"" + recipientEmail + "\",\"permission\":\"" + permission + "\"}";
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

        public async Task<List<Permission>> GetSharedUsers(File file, string token)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(this.BaseAddress.ToString() + "files/" + file.Id + "/share/");
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

        public Uri GetDownloadUrl(File file)
        {
            return new Uri(this.BaseAddress.ToString() + "resources/download/" + file.Id + "/");
        }

        public Uri GetUploadUrl(int folderId)
        {
            return new Uri(this.BaseAddress.ToString() + "folders/" + folderId + "/resources/");
        }

        public async Task<File> UploadFile(Stream fileStream, string fileName, string contentType, int folderId, string token)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(this.BaseAddress.ToString() + "folders/" + folderId + "/resources/");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token);

            // I don't know what is it used for, I'm lazy to search, but it's good to know we can use it
            /*var boundary = Guid.NewGuid().ToString();
            var content = new MultipartFormDataContent(boundary);*/

            var content = new MultipartFormDataContent();
            var streamContent = new StreamContent(fileStream);
            streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);

            content.Add(streamContent, "file", fileName);

            var response = await client.PostAsync(client.BaseAddress, content);
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
            File result = null;
            try
            {
                result = JsonConvert.DeserializeObject<File>(responseContent);
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


        public async Task<Stream> DownloadFile(File file, string token)
        {
            DownloadFileClient = new HttpClient();
            DownloadFileClient.BaseAddress = new Uri(this.BaseAddress.ToString() + "resources/" + file.Id + "/download/");
            DownloadFileClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            DownloadFileClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token);

            HttpResponseMessage response = await DownloadFileClient.GetAsync(DownloadFileClient.BaseAddress, HttpCompletionOption.ResponseContentRead);
            var httpStream = await response.Content.ReadAsStreamAsync();
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                if (ErrorReceived != null)
                {
                    var newError = new Error(true, "Unable to download this file");
                    this.ErrorReceived(this, new ErrorEventArgs(newError));
                    return null;
                }
            }
            return httpStream;
        }


        /*public async Task<string> GetResourcesByAnonymous(File file)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(this.BaseAddress.ToString() + "resources/" + file.Url + "/download_anonymous/");
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
        }*/
    }
}
