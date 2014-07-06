using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Cubbyhole.Models
{
    public abstract class DiskEntity
    {
        public DiskEntity()
        {

        }

        public DiskEntity(DiskEntity entity)
        {
            this.CreationDate = entity.CreationDate;
            this.Icon = entity.Icon;
            this.Id = entity.Id;
            this.Name = entity.Name;
            this.Permission = entity.Permission;
            this.UpdateDate = entity.UpdateDate;
            this.UserId = entity.UserId;
        }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("permission")]
        public PermissionRight Permission { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreationDate { get; set; }

        public string ReadableCreationDate
        {
            get
            {
                return string.Format("{0:yyyy/MM/dd}", CreationDate);
            }
        }

        [JsonProperty("updated_at")]
        public DateTime UpdateDate { get; set; }        

        public string ReadableUpdateDate()
        {
            return string.Format("{0:yyyy/MM/dd}", UpdateDate);
        }
 
        [JsonProperty("user_id_creator")]
        public int UserId { get; set; }

        public Object Icon { get; set; }
    }
}
