using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cubbyhole.Models;
using Newtonsoft.Json.Linq;

namespace Cubbyhole.API
{

    public class DiskEntityConverter : JsonClassChooserConverter<DiskEntity>
    {
        protected override Meta ComputeType(JObject jObject)
        {
            if (jObject["file_id"] != null)
            {
                return new File();
            }

            else
            {
                return new Folder();
            }
        }
    }
}
