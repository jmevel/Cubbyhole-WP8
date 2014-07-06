using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Cubbyhole.Models;
using Newtonsoft.Json;

namespace Cubbyhole.API
{
    public class PlanAPI
    {
        private Uri BaseAddress { get; set; }
        public event ErrorReceivedEventHandler ErrorReceived;

        public PlanAPI(Uri baseAddress)
        {
            BaseAddress = new Uri(baseAddress + "plans/");
        }

        public async Task<List<Plan>> GetPlans()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = this.BaseAddress;

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

            List<Plan> result = null;
            try
            {
                result = JsonConvert.DeserializeObject<List<Plan>>(responseContent);
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

        public async Task<Plan> CreatePlan(Plan plan, string adminToken)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = this.BaseAddress;

            var content = JsonConvert.SerializeObject(plan);
            HttpContent httpContent = new StringContent(content, Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Token", adminToken);

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
                }
            }


            Plan result = null;
            try
            {
                result = JsonConvert.DeserializeObject<Plan>(responseContent);
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

        public async Task<Plan> GetPlanById(int planId)
        {
            Plan plan2 = new Plan();
            //TODO change from Mock to Real
            //List<Plan> plans = await GetPlans();
            List<Plan> plans = await Mock.PlanMock.Get4FakePlans();
            foreach (var plan in plans)
            {
                if (plan.Id == planId)
                {
                    return plan;
                }
            }
            return null;
        }

        public async Task<Plan> UpdatePlan(Plan plan, string adminToken)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = this.BaseAddress;
            var content = JsonConvert.SerializeObject(plan);
            HttpContent httpContent = new StringContent(content, Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("token", adminToken);
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


            Plan result = null;
            try
            {
                result = JsonConvert.DeserializeObject<Plan>(responseContent);
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

        public async Task<Plan> DeletePlan(int planId, string adminToken)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(this.BaseAddress + "?id=" + planId);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("token", adminToken);
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

            Plan result = null;
            try
            {
                result = JsonConvert.DeserializeObject<Plan>(responseContent);
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
    }
}
