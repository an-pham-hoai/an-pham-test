using System;
using System.Collections.Generic;
using System.Net;

namespace Server.ExceptionHandlers
{
    public class BusinessValidationException : Exception
    {
        public ICollection<ErrorParams> Errors { get; }
        public HttpStatusCode StatusCode { get; }

        public BusinessValidationException(ICollection<ErrorParams> businessErrors, HttpStatusCode statusCode, string message = null) : base(message)
        {
            Errors = businessErrors;
            StatusCode = statusCode;
        }

        public BusinessValidationException(HttpStatusCode statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
