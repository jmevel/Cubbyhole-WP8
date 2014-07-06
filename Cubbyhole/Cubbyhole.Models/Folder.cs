using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Cubbyhole.Models
{
    public class Folder : DiskEntity
    {
        public Folder()
        {

        }

        public Folder(Folder folder) : base(folder)
        {
            this.ParentId = folder.ParentId;
            this.Files = folder.Files;
        }

        [JsonProperty("parent")]
        public int ParentId { get; set; }

        public List<File> Files { get; set; }
    }
}
