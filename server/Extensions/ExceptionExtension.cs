using Server.ExceptionHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Extensions
{
    public static class ExceptionExtension
    {
        public static IEnumerable<ErrorParams> GetExceptionsRecursively(this Exception exception)
        {
            if (exception == null)
            {
                yield return new ErrorParams();
            }

            if (exception.InnerException != null)
            {
                foreach (ErrorParams errorResult in GetExceptionsRecursively(exception.InnerException))
                {
                    if (errorResult.Exception != null && !string.IsNullOrEmpty(errorResult.Exception.StackTrace))
                    {
                        errorResult.Message += Environment.NewLine + errorResult.Exception.StackTrace;
                    }

                    yield return errorResult;
                }
            }

            yield return new ErrorParams(exception.Source, exception.Message, exception);
        }
    }
}
