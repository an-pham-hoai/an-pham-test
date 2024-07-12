using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Server.Model
{
    public class RMSError
    {
        public string Error { get; set; }

        public RMSError(string error)
        {
            Error = error;
        }
    }
}