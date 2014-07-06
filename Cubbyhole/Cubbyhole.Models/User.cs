using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Cubbyhole.Models
{
    public class User
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("firstname")]
        public string Firstname { get; set; }

        [JsonProperty("lastname")]
        public string Lastname { get; set; }

        [JsonProperty("address")]
        public string Street { get; set; }

        [JsonProperty("postal_code")]
        public string Zip { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("plan_id")]
        public int PlanId { get; set; }

        [JsonProperty("paid")]
        public bool HasPaid { get; set; }

        [JsonProperty("registration_paid_plan_date")]
        public DateTime RegistrationPaidPlanDate { get; set; }

        [JsonProperty("used_space")]
        public float UsedSpace { get; set; }

        [JsonProperty("used_bandwidth")]
        public float UsedBandwidth { get; set; }

        [JsonProperty("used_quota_shared")]
        public float UsedQuotaShared { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreationDate { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdateDate { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

    }
}
