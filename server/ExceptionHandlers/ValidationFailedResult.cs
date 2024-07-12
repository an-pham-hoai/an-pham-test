using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Server.ExceptionHandlers
{
    public class ValidationFailedResult : ObjectResult
    {
        public ValidationFailedResult(string message, int statusCode, ModelStateDictionary modelState = null) : base(new ErrorResult(modelState, message, statusCode))
        {
            StatusCode = statusCode;
        }
    }
}
