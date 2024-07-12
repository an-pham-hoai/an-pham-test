using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Server.BusinessLogic;
using Server.DataAccess;
using Server.Model;
using System;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Server.Controllers
{
    [Route("[controller]")]
    [EnableCors("MyPolicy")]
    [ApiController]
    public class MasterController : Controller
    {
        [Route("[action]")]
        [HttpGet]
        public string IP()
        {
            var remoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress;
            return remoteIpAddress.ToString();
        }

        /// <summary>
        /// Get db schema
        /// </summary>
        /// <returns></returns>
        [Route("[action]")]
        [HttpGet]
        public TObject<string> GetSchema()
        {
            var schema = UserDAC.Instance.GetDatabaseSchema();
            var s = JsonConvert.SerializeObject(schema);
            var sd = CryptoBC.Instance.EncryptStringAES(s);
            return new TObject<string>() { Value = sd };
        }

    }
}
