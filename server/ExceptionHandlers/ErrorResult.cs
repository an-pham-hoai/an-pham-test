using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Server.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Server.ExceptionHandlers
{
    public class ErrorResult
    {
        public int StatusCode { get; set; }

        public string Message { get; set; } = "One or more errors occured.";

        [XmlIgnore]
        [JsonIgnore]
        public ICollection<ErrorParams> Errors_I { get; set; }

        public List<ErrorParams> Errors { get { return Errors_I.ToList(); } }

        public bool IsSuccess { get; set; }

        public ErrorResult()
        {
            Errors_I = new HashSet<ErrorParams>();
        }

        public ErrorResult(ModelStateDictionary modelState)
            : this()
        {
            if (modelState != null && modelState.Keys.Count() > 0)
            {
                var ts = modelState.Keys.SelectMany(key => modelState[key].Errors.Select(x => new ErrorParams(key, x.ErrorMessage))).ToList();
                Errors_I = ts;
            }
        }

        public ErrorResult(ModelStateDictionary modelState, string message, int statusCode)
            : this(modelState)
        {
            Message = message;
            StatusCode = statusCode;
        }

        public ErrorResult(List<ErrorParams> errors)
        {
            Errors_I = errors;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public string ToXML()
        {
            using (var stringwriter = new System.IO.StringWriter())
            {
                var serializer = new XmlSerializer(this.GetType());
                serializer.Serialize(stringwriter, this);
                return stringwriter.ToString();
            }
        }
    }
}
