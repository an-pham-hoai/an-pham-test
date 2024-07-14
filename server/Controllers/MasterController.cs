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
using System.Linq;

namespace Server.Controllers
{
    [Route("[controller]")]
    [EnableCors("MyPolicy")]
    [ApiController]
    public class MasterController : Controller
    {
        public static List<User> users = new List<User>();

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

        [Route("[action]")]
        [HttpPost]
        public List<User> UserState(User user)
        {
            long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            User stateUser = users.Find(t => t.Id == user.Id);
            if (stateUser != null)
            {
                stateUser.Timestamp = now;
                stateUser.QuizSessionId = user.QuizSessionId;
            }
            else
            {
                users.Add(user);
            }

            return users.Where(t => t.Timestamp > now - 10000).ToList();
        }

    }
}
