using Server.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Server.BusinessLogic;

namespace Server.BusinessLogic
{
    /// <summary>
    /// A serverless style data access component
    /// </summary>
    public class GenericBC
    {
        #region Singleton

        public static GenericBC Instance { get; } = new GenericBC();

        private GenericBC() { }

        #endregion

        #region Methods

        /// <summary>
        /// Executes a query on current database (ie sql that returns one or many vectors)
        /// </summary>
        /// <param name="sql">The input sql</param>
        /// <returns>The serialized string of result</returns>
        public string Query(string sql)
        {
            string result = string.Empty;
            using (SqlConnection sqlConnection = new SqlConnection(Config.Instance.ConnectionString))
            {
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection) { CommandTimeout = 300 };
                try
                {
                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        var r = Serialize(reader);
                        result = JsonConvert.SerializeObject(r, Formatting.Indented);
                    }
                }
                catch(Exception ex)
                {

                }
                

                sqlConnection.Close();
            }

            return result;
        }

        /// <summary>
        /// Executes batch queries on current database. The queries can on multiple tables.
        /// </summary>
        /// <param name="sqls">The input sqls</param>
        /// <returns></returns>
        public string Queries(List<string> sqls)
        {
            string result = string.Empty;
            using (SqlConnection sqlConnection = new SqlConnection(Config.Instance.ConnectionString))
            {
                sqlConnection.Open();
                List<object> objs = new List<object>();

                foreach (string sql in sqls)
                {
                    SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection) { CommandTimeout = 300 };

                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        var r = Serialize(reader);
                        objs.Add(r);
                    }
                }

                result = JsonConvert.SerializeObject(objs, Formatting.Indented);
                sqlConnection.Close();
            }

            return result;
        }

        /// <summary>
        /// Executes a non-query on current database (ie sql that returns nothing)
        /// </summary>
        /// <param name="sql">The input sql</param>
        /// <returns>True if no error occurs, otherwise false</returns>
        public bool NonQuery(string sql)
        {
            long result = 0;
            using (SqlConnection sqlConnection = new SqlConnection(Config.Instance.ConnectionString))
            {
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection) { CommandTimeout = 300 };
                result = sqlCommand.ExecuteNonQuery();
                sqlConnection.Close();
            }

            return result > 0;
        }

        /// <summary>
        /// Executes a scalar query on current database (ie sql that returns primitive data types)
        /// </summary>
        /// <param name="sql">The input sql</param>
        /// <returns>The string representation of result object</returns>
        public string Scalar(string sql)
        {
            object result = 0;
            using (SqlConnection sqlConnection = new SqlConnection(Config.Instance.ConnectionString))
            {
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection) { CommandTimeout = 300 };
                result = sqlCommand.ExecuteScalar();
                sqlConnection.Close();
            }


            return result != null ? result.ToString() : null;
        }

        /// <summary>
        /// Performs a transaction for an sql list (ie all or nothing are executed)
        /// </summary>
        /// <param name="sqls">The input sql list</param>
        /// <returns>True if all executed successfully, false if nothing executed</returns>
        public bool Transaction(List<string> sqls)
        {
            using (SqlConnection sqlConnection = new SqlConnection(Config.Instance.ConnectionString))
            {
                sqlConnection.Open();
                SqlTransaction transaction = sqlConnection.BeginTransaction();

                try
                {
                    foreach (string sql in sqls)
                    {
                        SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection, transaction);
                        sqlCommand.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    //LogBC.Instance.Log(e);
                    return false;
                }

                sqlConnection.Close();
            }

            return true;
        }

        /// <summary>
        /// Serialize objects from database
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public IEnumerable<Dictionary<string, object>> Serialize(SqlDataReader reader)
        {
            var results = new List<Dictionary<string, object>>();
            var cols = new List<string>();
            for (var i = 0; i < reader.FieldCount; i++)
                cols.Add(reader.GetName(i));

            while (reader.Read())
                results.Add(SerializeRow(cols, reader));

            return results;
        }

        private Dictionary<string, object> SerializeRow(IEnumerable<string> cols, SqlDataReader reader)
        {
            var result = new Dictionary<string, object>();
            foreach (var col in cols)
                result.Add(col, reader[col]);
            return result;
        }

        #endregion
    }
}
