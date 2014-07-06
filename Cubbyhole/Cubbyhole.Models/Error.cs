using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cubbyhole.Models
{
    public class Error
    {
        public Error(bool isServerResponse, string errorMessage)
        {
            if (isServerResponse == true)
            {
                this.Title = "Sorry";
                this.Message = errorMessage.Replace("{", "")
                                            .Replace("}", "")
                                            .Replace("[", "")
                                            .Replace("]", "")
                                            .Replace("\\", "")
                                            .Replace("\"", "")
                                            .Replace(":", " ");
            }
            else
            {
                this.Title = "Error";
                this.Message = errorMessage;
            }

        }

        public string Title;
        public string Message;
    }
}
