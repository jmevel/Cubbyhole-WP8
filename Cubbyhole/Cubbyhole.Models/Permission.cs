using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cubbyhole.Models
{
    public class Permission
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("permission")]
        public PermissionRight PermissionGiven { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreationDate { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdateDate { get; set; }
    }

    public enum PermissionRight
    {
        read,
        read_write
    };
}
