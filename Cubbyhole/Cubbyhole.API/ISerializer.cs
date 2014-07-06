using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Cubbyhole.API
{
    public interface ISerializer
    {
        string ContentType { get; }

        Stream Serialize<T>(T instance) where T : class;

        T Deserialize<T>(Stream stream);
    }
}
