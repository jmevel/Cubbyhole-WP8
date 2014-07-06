using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Cubbyhole.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;

namespace Cubbyhole.API
{
    public delegate void ErrorReceivedEventHandler(object sender, ErrorEventArgs e);
    public class UserAPI
    {
        private Uri _baseAddress { get; set; }
        private Uri _paypalAddress { get; set; }
        public event ErrorReceivedEventHandler ErrorReceived;

        public UserAPI(Uri baseAddress)
        {
            _baseAddress = new Uri(baseAddress + "users/");
            _paypalAddress = new Uri(baseAddress + "payment_url/");
        }

        public async Task<User> DeleteUser(int userId, string userToken)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(this._baseAddress + "?id=" + userId);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("token", userToken);
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
                }
            }
            

            User result = null;
            try
            {
                result = JsonConvert.DeserializeObject<User>(responseContent);
            }
            catch (Exception)
            {
                if (ErrorReceived != null)
                {
                    var newError = new Error(true, responseContent);
                    this.ErrorReceived(this, new ErrorEventArgs(newError));
                }
            }
            return result;
        }

        public async Task<User> UpdateUser(User user)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(this._baseAddress.ToString() + user.Id + "/");

            var content = JsonConvert.SerializeObject(user);
            HttpContent httpContent = new StringContent(content, Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(user.Token);

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
                }
            }

            User result = null;
            try
            {
                result = JsonConvert.DeserializeObject<User>(responseContent);
            }
            catch (Exception)
            {
                if (ErrorReceived != null)
                {
                    var newError = new Error(true, responseContent);
                    this.ErrorReceived(this, new ErrorEventArgs(newError));
                }
            }
            return result;
        }

        public async Task<User> Login(string email, string password)
        {
            User result = null;
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(this._baseAddress.ToString().Replace("users/", "login/"));
            string content = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\"}";
            HttpContent httpContent = new StringContent(content, Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.PostAsync(client.BaseAddress, httpContent);
            string responseContent = await response.Content.ReadAsStringAsync();

            try
            {
                response.EnsureSuccessStatusCode();

                try
                {
                    result = JsonConvert.DeserializeObject<User>(responseContent);
                }
                catch (Exception ex)
                {
                    if (ErrorReceived != null)
                    {
                        var newError = new Error(true, responseContent);
                        this.ErrorReceived(this, new ErrorEventArgs(newError));
                        return null;
                    }
                }
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

        public async Task<User> GetUserById(int userId, string userToken)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(this._baseAddress + "?id=" + userId);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //TODO ça a changé: demander à charles-edouard
            client.DefaultRequestHeaders.Add("token", userToken);
            HttpResponseMessage response = await client.GetAsync(client.BaseAddress);

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                if (ErrorReceived != null)
                {
                    var newError = new Error(false, response.StatusCode + ":" + response.ReasonPhrase);
                    this.ErrorReceived(this, new ErrorEventArgs(newError));
                }
            }
            string responseContent = await response.Content.ReadAsStringAsync();

            User result = null;
            try
            {
                result = JsonConvert.DeserializeObject<User>(responseContent);
            }
            catch (Exception)
            {
                if (ErrorReceived != null)
                {
                    var newError = new Error(true, responseContent);
                    this.ErrorReceived(this, new ErrorEventArgs(newError));
                }
            }
            return result;
        }

        public async Task<User> CreateUser(User user)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = this._baseAddress;

            var content = JsonConvert.SerializeObject(user);
            HttpContent httpContent = new StringContent(content, Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.PostAsync(client.BaseAddress, httpContent);
            var responseContent = await response.Content.ReadAsStringAsync();

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                if (ErrorReceived != null)
                {
                    Error error = new Error(true, responseContent);
                    this.ErrorReceived(this, new ErrorEventArgs(error));
                    return null;
                }
            }

            User result = null;
            try
            {
                result = JsonConvert.DeserializeObject<User>(responseContent);
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

        public async Task<List<User>> GetAllUsers(string adminToken)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = this._baseAddress;
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("token", adminToken);
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
                }
            }

            List<User> result = null;
            try
            {
                result = JsonConvert.DeserializeObject<List<User>>(responseContent);
            }
            catch (Exception)
            {
                if (ErrorReceived != null)
                {
                    var newError = new Error(true, responseContent);
                    this.ErrorReceived(this, new ErrorEventArgs(newError));
                }
            }
            return result;
        }

        public async Task<string> GetPaypalUrl(string token)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = this._paypalAddress;

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token);

            HttpResponseMessage response = await client.GetAsync(client.BaseAddress);
            var responseContent = await response.Content.ReadAsStringAsync();

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                if (ErrorReceived != null)
                {
                    Error error = new Error(true, responseContent);
                    this.ErrorReceived(this, new ErrorEventArgs(error));
                }
            }

            return responseContent;
        }
    }
}
