using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Server.Model;
using Server.BusinessLogic;
using System.Reflection;
using Newtonsoft.Json;

namespace Server.Controllers
{
    /// <summary>
    /// Serverless controller. Provides generic solution on processing back-end logic.
    /// </summary>
    [Route("[controller]")]
    [EnableCors("MyPolicy")]
    [ApiController]
    public class GenericController : Controller
    {
        /// <summary>
        /// Executes a query on current database (ie sql that returns one or many vectors)
        /// </summary>
        /// <param name="sql">The encrypted sql</param>
        /// <returns>The serialized string of result</returns>
        [Route("[action]")]
        [HttpPost]
        public TObject<string> Query(Sql sql)
        {
            var sqlDecrypted = CryptoBC.Instance.DecryptStringAES(sql.sql);
            var result = GenericBC.Instance.Query(sqlDecrypted);
            //return new TObject<string>() { Value = CryptoBC.Instance.EncryptStringAES(result) };
            return new TObject<string>() { Value = result };
        }

        /// <summary>
        /// Executes batch queries on current database. The queries can on multiple tables.
        /// </summary>
        /// <param name="sql">The encrypted sqls</param>
        /// <returns>The serialized string of result</returns>
        [Route("[action]")]
        [HttpPost]
        public TObject<string> Queries(List<Sql> sqls)
        {
            List<string> decryptedSqls = new List<string>();
            foreach (Sql sql in sqls)
            {
                decryptedSqls.Add(CryptoBC.Instance.DecryptStringAES(sql.sql));
            }
            var result = GenericBC.Instance.Queries(decryptedSqls);
            return new TObject<string>() { Value = result };
        }

        /// <summary>
        /// Executes a non-query on current database (ie sql that returns nothing)
        /// </summary>
        /// <param name="sql">The encrypted sql</param>
        /// <returns>True if no error occurs, otherwise false</returns>
        [Route("[action]")]
        [HttpPost]
        public bool NonQuery(Sql sql)
        {
            var sqlDecrypted = CryptoBC.Instance.DecryptStringAES(sql.sql);
            return GenericBC.Instance.NonQuery(sqlDecrypted);
        }

        /// <summary>
        /// Executes a scalar query on current database (ie sql that returns primitive data types)
        /// </summary>
        /// <param name="sql">The input sql</param>
        /// <returns>The string representation of result object</returns>
        [Route("[action]")]
        [HttpPost]
        public TObject<string> Scalar(Sql sql)
        {
            var sqlDecrypted = CryptoBC.Instance.DecryptStringAES(sql.sql);
            string result = GenericBC.Instance.Scalar(sqlDecrypted);
            return new TObject<string>() { Value = result };
        }

        /// <summary>
        /// Performs a transaction for an sql list (ie all or nothing are executed)
        /// </summary>
        /// <param name="sqls">The encrypted sqls</param>
        /// <returns>True if all executed successfully, false if nothing executed</returns>
        [Route("[action]")]
        [HttpPost]
        public bool Transaction(List<Sql> sqls)
        {
            List<string> decryptedSqls = new List<string>();
            foreach (Sql sql in sqls)
            {
                decryptedSqls.Add(CryptoBC.Instance.DecryptStringAES(sql.sql));
            }

            return GenericBC.Instance.Transaction(decryptedSqls);
        }

        [Route("[action]")]
        [HttpPost]
        public bool BulkInsert(BulkInput bulkInput)
        {
            Type typeEntity = Type.GetType($"Server.Model.{bulkInput.Entity}");
            Type typeDAC = Type.GetType($"Server.DataAccess.{bulkInput.Entity}DAC");
            PropertyInfo propertyInstance = typeDAC.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            dynamic dac = (dynamic)propertyInstance.GetValue(null, null);
            dac.BulkInsert(bulkInput.Jsons);
            return true;
        }
    }
}
