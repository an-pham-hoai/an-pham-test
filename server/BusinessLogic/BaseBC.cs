using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Server.DataAccess;
using Server.ExceptionHandlers;
using Server.Extensions;
using Server.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Server.BusinessLogic
{
    public abstract class BaseBC<T> where T : BaseEntity
    {
        #region Base

        protected abstract BaseDAC<T> DAC { get; }

        /// <summary>
        /// Gets full realized entity by id.
        /// </summary>
        /// <param name="id">The entity id</param>
        /// <returns>The entity with completed children sub tree</returns>
        public virtual T Get(long id)
        {
            var t = DAC.Get(id);
            FullRealize(t);
            return t;
        }

        /// <summary>
        /// Adds entity to db.
        /// </summary>
        /// <param name="t">The input entity</param>
        /// <param name="transaction">The transaction from outer scope, if any</param>
        /// <returns>The id of inserted entity</returns>
        public virtual long Insert(T t, SqlTransaction transaction = null)
        {
            return DAC.Insert(t, transaction);
        }

        /// <summary>
        /// Updates entity to db.
        /// </summary>
        /// <param name="t">The input entity</param>
        /// <param name="transaction">The transaction from outer scope, if any</param>
        /// <returns>The id of updated entity</returns>
        public virtual long Update(T t, SqlTransaction transaction = null)
        {
            return DAC.Update(t, transaction);
        }

        /// <summary>
        /// Soft deletes entity from db.
        /// </summary>
        /// <param name="t">The input entity</param>
        /// <param name="transaction">The transaction from outer scope, if any</param>
        /// <returns>The id of deleted entity</returns>
        public virtual long Delete(T t, SqlTransaction transaction = null)
        {
            return DAC.Delete(t.Id, transaction);
        }

        /// <summary>
        /// Gets all children entities of input entity
        /// </summary>
        /// <param name="t"></param>
        public virtual void FullRealize(T t)
        {

        }

        /// <summary>
        /// Gets all children entities of input entity list
        /// </summary>
        /// <param name="ts"></param>
        public virtual void FullRealize(List<T> ts)
        {
            foreach (T t in ts)
            {
                FullRealize(t);
            }
        }

        protected virtual bool IsInsert(T t)
        {
            return t != null && t.Id == 0;
        }

        /// <summary>
        /// Transactional save flow for single entity
        /// </summary>
        /// <param name="t"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public virtual long Save(T t, SqlTransaction transaction = null)
        {
            SqlConnection sqlConnection = null;
            bool rootFlow = transaction == null;

            try
            {
                sqlConnection = rootFlow ? new SqlConnection(Config.Instance.ConnectionString) : transaction.Connection;
                if (rootFlow)
                {
                    sqlConnection.Open();
                    transaction = sqlConnection.BeginTransaction();
                }

                long r = 0;
                if (t != null)
                {
                    if (t.CreatedDate == 0) t.CreatedDate = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    t.ModifiedDate = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                    if (t.IsDeleted)
                    {
                        r = Delete(t, transaction);
                    }
                    else if (IsInsert(t))
                    {
                        t.Id = Insert(t, transaction);
                        r = t.Id;
                    }
                    else
                    {
                        r = Update(t, transaction);
                    }
                }

                if (rootFlow) transaction.Commit();
                return r;
            }
            catch (Exception ex)
            {
                if (rootFlow) transaction.Rollback();
                //LogBC.Instance.Log(ex);
                throw;
            }
            finally
            {
                //if no transaction used, then release the connection to prevent memory leak
                //otherwise, that's responsibility of outer scope
                if (rootFlow && sqlConnection != null) sqlConnection.Dispose();
            }
        }

        /// <summary>
        /// Transactional save flow for list of entities
        /// </summary>
        /// <param name="ts"></param>
        /// <param name="transaction"></param>
        public virtual void Save(List<T> ts, SqlTransaction transaction = null)
        {
            SqlConnection sqlConnection = null;
            bool rootFlow = transaction == null;

            try
            {
                sqlConnection = rootFlow ? new SqlConnection(Config.Instance.ConnectionString) : transaction.Connection;
                if (rootFlow)
                {
                    sqlConnection.Open();
                    transaction = sqlConnection.BeginTransaction();
                }

                foreach (T t in ts)
                {
                    Save(t, transaction);
                }

                if (rootFlow) transaction.Commit();
            }
            catch (Exception ex)
            {
                if (rootFlow) transaction.Rollback();
                //LogBC.Instance.Log(ex);
                throw;
            }
            finally
            {
                //if no transaction used, then release the connection to prevent memory leak
                //otherwise, that's responsibility of outer scope
                if (rootFlow && sqlConnection != null) sqlConnection.Dispose();
            }
        }

        #endregion
    }
}
