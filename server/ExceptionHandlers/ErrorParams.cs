using System;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Server.ExceptionHandlers
{
    public class ErrorParams
    {
        public string Index { get; set; }

        public string DocNo { get; set; }

        //public string Source { get; set; }

        public string Message { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public Exception Exception { get; }

        public ErrorParams() { }

        public ErrorParams(string field, string message, string index="", string source = null)
        {
            Index = (!string.IsNullOrEmpty(index)) ? index : null;
            DocNo = (!string.IsNullOrEmpty(field)) ? field : null;
            //Source = (!string.IsNullOrEmpty(source)) ? source : null;
            Message = message;
        }

        public ErrorParams(string source, string message, Exception exception, string index = "")
        {
            Index = (!string.IsNullOrEmpty(index)) ? index : null;
            //Source = source != string.Empty ? source : null;
            Message = message;
            Exception = exception;
        }
    }
}
