using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cubbyhole.Models;

namespace Cubbyhole.API
{
    public class ErrorEventArgs : EventArgs
    {
        public ErrorEventArgs()
        {

        }

        public ErrorEventArgs(Error error)
        {
            Error = error;
        }
        public Error Error { get; set; }
    }
}
