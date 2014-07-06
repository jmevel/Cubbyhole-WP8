using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Cubbyhole.API
{
     public class JsonSerializer : ISerializer
    {
        private readonly Newtonsoft.Json.JsonSerializer _serializer;

        public string ContentType { get; private set; }

        public JsonSerializer()
        {

            _serializer = new Newtonsoft.Json.JsonSerializer();

            ContentType = "application/json";
        }

        public Stream Serialize<T>(T instance) where T : class
        {
            var writer = new StreamWriter(new MemoryStream(), Encoding.UTF8);
            _serializer.Serialize(writer, instance);

            writer.Flush();
            writer.BaseStream.Seek(0, SeekOrigin.Begin);

            return writer.BaseStream;
        }

        public T Deserialize<T>(Stream stream)
        {
            var serializer = new Newtonsoft.Json.JsonSerializer();

            using (var reader = new StreamReader(stream))
            {
                return serializer.Deserialize<T>(new JsonTextReader(reader));
            }
        }
    }
}

