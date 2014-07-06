using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Cubbyhole.Models
{
    public class Plan
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("price")]
        public float Price { get; set; }

        [JsonProperty("duration")]
        public int Duration { get; set; }

        [JsonProperty("max_storage")]
        public int MaxStorage { get; set; }

        [JsonProperty("max_bandwidth")]
        public int MaxBandwidth { get; set; }

        [JsonProperty("max_quota_shared")]
        public int MaxQuotaSHared { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreationDate { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdateDate { get; set; }

    
    }
}
