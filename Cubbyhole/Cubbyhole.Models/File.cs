using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Net;

namespace Cubbyhole.Models
{
    public class File : DiskEntity
    {
        public File()
        {

        }

        public File(File file) : base(file)
        {
            this.ContentType = file.ContentType;
            this.FolderId = file.FolderId;
            this.Md5 = file.Md5;
            this.Path = file.Path;
            this.Size = file.Size;
        }

        [JsonProperty("folder_id")]
        public int FolderId { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("content_type")]
        public string ContentType { get; set; }

        [JsonProperty("md5")]
        public string Md5 { get; set; }

        private double _size;

        [JsonProperty("size")]
        public double Size 
        {
            get
            {
                return this._size;
            }
            set
            {
                this._size = value;
                string[] sizes = { "B", "KB", "MB", "GB" };
                double len = value;
                int order = 0;
                while (len >= 1024 && order + 1 < sizes.Length)
                {
                    order++;
                    len = len / 1024;
                }
                this.ReadableSize = String.Format("{0:0.##} {1}", len, sizes[order]);
            } 
        }

        public string ReadableSize { get; set; }
    }
}
