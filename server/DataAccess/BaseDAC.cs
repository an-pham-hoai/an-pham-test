using Server.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.IO;
using System.Data.SqlClient;
using System.Reflection;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Data;
using Server.Extensions;
using Server.BusinessLogic;

namespace Server.DataAccess
{
    /// <summary>
    /// Base class for DAC components.
    /// </summary>
    /// <typeparam name="T">A BaseEntity.</typeparam>
    public abstract class BaseDAC<T> where T : BaseEntity
    {
        #region Protected

        /// <summary>
        /// Gets the connection string to database.
        /// </summary>
        protected virtual string ConnectionString
        {
            get { return Config.Instance.ConnectionString; }
        }

        /// <summary>
        /// In default implementation for creating/editing entities, if exact match not found, and this flag is turned on, BaseDAC will use AI
        /// to guess the entity property name
        /// </summary>
        protected bool UseAIMapping { get; } = true;

        /// <summary>
        /// Creates an entity from data reader.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <returns>The result entity.</returns>
        protected virtual T CreateEntity(TypedDataReader dataReader)
        {
            var cols = new List<string>();
            for (var i = 0; i < dataReader.FieldCount; i++) cols.Add(dataReader.GetName(i));
            var keyValues = new Dictionary<string, object>();
            foreach (var col in cols)
            {
                if (!keyValues.ContainsKey(col)) keyValues.Add(col, dataReader[col]);
            }
            T t = (T)Activator.CreateInstance(typeof(T));

            List<string> fields = GetTableFieldNames();
            foreach (var kv in keyValues)
            {
                PropertyInfo property = GetProperty(t, kv.Key, fields);
                if (property == null) continue; //database table has field but entity not have
                var value = kv.Value;
                if (value == null) continue;
                Type propertyType = property.PropertyType;
                var targetType = IsNullableType(propertyType) ? Nullable.GetUnderlyingType(propertyType) : propertyType;
                value = ConvertDB2Entity(targetType, value);
                SetPropertyValue(t, property, value);
            }
            return t;
        }

        private void SetPropertyValue(T entity, PropertyInfo property, object value)
        {
            try
            {
                if (!property.CanWrite) throw new Exception($"Please define setter for property '{property.Name}' of class '{entity.GetType().Name}'");
                property.SetValue(entity, value, null);
            }
            catch (Exception)
            {
                throw new Exception($"Error when setting property '{property.Name}' with value '{value.ToString()}' to entity '{entity.GetType().Name}'");
            }
        }

        /// <summary>
        /// Default converter when reading objects from database.
        /// </summary>
        /// <param name="targetType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected object ConvertDB2Entity(Type targetType, object value)
        {
            if (targetType.IsEnum) value = Enum.ToObject(targetType, value);
            else if (targetType.IsGenericType && targetType.GetGenericTypeDefinition().Equals(typeof(List<>)))
            {
                Type subType = targetType.GetGenericArguments()[0];
                if (subType == typeof(int)) value = Helper.FromCSV<int>(value.ToString());
                if (subType == typeof(long)) value = Helper.FromCSV<long>(value.ToString());
                if (subType == typeof(double)) value = Helper.FromCSV<double>(value.ToString());
                if (subType == typeof(decimal)) value = Helper.FromCSV<decimal>(value.ToString());
                if (subType == typeof(float)) value = Helper.FromCSV<float>(value.ToString());
                if (subType == typeof(bool)) value = Helper.FromCSV<bool>(value.ToString());
                if (subType == typeof(string)) value = Helper.FromCSV<string>(value.ToString());
            }
            else value = Convert.ChangeType(value, targetType);

            return value;
        }

        /// <summary>
        /// Default converter when writing objects to database
        /// </summary>
        /// <param name="property"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected object ConvertEntity2DB(PropertyInfo property, T entity)
        {
            object v = property.GetValue(entity);
            if (v == null) return null;
            if (v is List<int>) v = Helper.ToCSV(v as List<int>);
            if (v is List<long>) v = Helper.ToCSV(v as List<long>);
            if (v is List<double>) v = Helper.ToCSV(v as List<double>);
            if (v is List<decimal>) v = Helper.ToCSV(v as List<decimal>);
            if (v is List<float>) v = Helper.ToCSV(v as List<float>);
            if (v is List<bool>) v = Helper.ToCSV(v as List<bool>);
            if (v is List<string>) v = Helper.ToCSV(v as List<string>);
            return v;
        }

        /// <summary>
        /// Checks if a Type is nullable, for ex, int? . Note: int? will be translated to Nullable<int> by the compiler.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        }

        /// <summary>
        /// Get sql statement for selecting an entity. For example: SELECT * FROM [dbo].[Table] WHERE Id=@p_Id
        /// </summary>
        /// <returns>The select sql statement.</returns>
        protected virtual string GetSelectStatement()
        {
            return $@"
            SELECT * FROM {GetTableName()} 
            WHERE [{nameof(BaseEntity.Id)}] = @{nameof(BaseEntity.Id)} 
            AND [{nameof(BaseEntity.IsDeleted)}] = 0
            ";
        }

        /// <summary>
        /// Gets the sql statement for selecting all entities in the table.
        /// For example: SELECT * FROM [dbo].[Table]
        /// </summary>
        /// <returns>The select sql statement.</returns>
        protected virtual string GetSelectAllStatement()
        {
            return $@"
            SELECT * FROM {GetTableName()} 
            WHERE [{nameof(BaseEntity.IsDeleted)}] = 0
            ";
        }

        /// <summary>
        /// Gets the sql statement for inserting a row into the database table.
        /// For example: INSERT INTO [dbo].[Table] (...) VALUES (...)
        /// </summary>
        /// <returns>The insert sql statement.</returns>
        protected virtual string GetInsertStatement()
        {
            List<string> fields = GetTableFieldNames().Where(t => !t.Equals(nameof(BaseEntity.Id), StringComparison.CurrentCultureIgnoreCase)).ToList();
            List<string> parameters = fields.Select(t => "@" + t).ToList();
            fields = fields.Select(t => $"[{t}]").ToList();

            return $@"
            INSERT INTO {GetTableName()}
            (
                {Helper.ToCSV(fields)}
            )
            OUTPUT Inserted.ID
            VALUES
            (
                {Helper.ToCSV(parameters)}
            )
            ";
        }

        /// <summary>
        /// Gets the sql statement for updating. For example: UPDATE [dbo].[Table] SET ... WHERE ...
        /// </summary>
        /// <returns>The update sql statement.</returns>
        protected virtual string GetUpdateStatement()
        {
            List<string> fields = GetTableFieldNames().Where(t => !t.Equals(nameof(BaseEntity.Id), StringComparison.CurrentCultureIgnoreCase)).ToList();
            fields = fields.Select(t => $"[{t}] = @{t}").ToList();
            return $@"
            UPDATE {GetTableName()}
            SET {Helper.ToCSV(fields)}
            WHERE [{nameof(BaseEntity.Id)}] = @{nameof(BaseEntity.Id)}";
        }

        /// <summary>
        /// Get the sql statement for deleting. For example: DELETE FROM [dbo].[Table] WHERE ...
        /// </summary>
        /// <returns>The delete sql statement.</returns>
        protected virtual string GetDeleteStatement()
        {
            return $@"
            UPDATE {GetTableName()} 
            SET [{nameof(BaseEntity.IsDeleted)}] = 1 
            WHERE [{nameof(BaseEntity.Id)}] = @{nameof(BaseEntity.Id)}
            ";
        }

        /// <summary>
        /// Gets sql statement for joining this entity with a parent entity.
        /// For example: SELECT * FROM [dbo].[ParentTable] AS P INNER JOIN [dbo].[Table] AS C ON 
        /// P.Id=C.ParentId WHERE P.Id=@p_ParentId
        /// </summary>
        /// <param name="parentType">The type of the parent entity.</param>
        /// <returns>The sql statement.</returns>
        protected virtual string GetJoinStatement(Type parentType)
        {
            return string.Empty;
        }

        /// <summary>
        /// Fills up the select command with parameters.
        /// </summary>
        /// <param name="sqlCommand">The select sql command.</param>
        /// <param name="id">The entity's id.</param>
        protected virtual void FillSelectCommandParams(SqlCommand sqlCommand, long id)
        {
            sqlCommand.Parameters.AddWithValue($"@{nameof(BaseEntity.Id)}", id);
        }

        /// <summary>
        /// Fills up the insert sql command with parameters.
        /// </summary>
        /// <param name="sqlCommand">The insert sql command.</param>
        /// <param name="entity">The inserted entity.</param>
        protected virtual void FillInsertCommandParams(SqlCommand sqlCommand, T entity)
        {
            List<string> fields = GetTableFieldNames();
            foreach (string field in fields)
            {
                PropertyInfo property = GetProperty(entity, field, fields);
                if (property != null) sqlCommand.Parameters.AddWithValue($"@{field}", ConvertEntity2DB(property, entity));
                else sqlCommand.Parameters.AddWithValue($"@{field}", null); //here database table has a redundant field that entity not have
            }
        }

        /// <summary>
        /// Get property of an entity by a hint
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="hint">The property name or its description. For ex, if class Student has property 'SecurityQuestion', hint can be: 'SecurityQuestion', 'securedquestion', 'question for security', etc</param>
        /// <param name="tableFieldNames">Current table field names, we pass this param here to avoid calling GetTableFieldNames multiple times</param>
        /// <returns>The target PropertyInfo</returns>
        private PropertyInfo GetProperty(T entity, string hint, List<string> tableFieldNames)
        {
            List<PropertyInfo> properties = entity.GetType().GetProperties().ToList();
            if (properties.Count == 0) throw new Exception($"Please define property for blank class '{entity.GetType().Name}'");

            //exact match first
            foreach (PropertyInfo property in properties)
            {
                if (property.Name.ToLower() == hint.ToLower()) return property;
            }

            if (!UseAIMapping) return null;

            //no exact match found, use AI to find entity property
            List<int> distances = new List<int>();
            foreach (PropertyInfo property in properties)
            {
                distances.Add(Helper.LevenshteinDistance(hint, property.Name));
            }

            PropertyInfo target = properties.Find(t => Helper.LevenshteinDistance(hint, t.Name) == distances.Min());
            bool wrongAI = tableFieldNames.Find(t => t.ToLower() == target.Name.ToLower()) != null; //AI wrongly found an exact match, which never the case when code flows here
            double ratio = (double)distances.Min() / (target.Name.Length + hint.Length);
            if (wrongAI || ratio > 0.5) return null;

            return target;
        }

        /// <summary>
        /// Fills up the update sql command with parameters.
        /// </summary>
        /// <param name="sqlCommand">The update sql command.</param>
        /// <param name="entity">The updated entity.</param>
        protected virtual void FillUpdateCommandParams(SqlCommand sqlCommand, T entity)
        {
            FillInsertCommandParams(sqlCommand, entity);
        }

        /// <summary>
        /// Fills up the delete command with parameters.
        /// </summary>
        /// <param name="sqlCommand">The delete sql command.</param>
        /// <param name="id">The entity's id.</param>
        protected virtual void FillDeleteCommandParams(SqlCommand sqlCommand, long id, long campaignId)
        {
            sqlCommand.Parameters.AddWithValue("CampaignId", campaignId);
        }

        protected virtual void FillDeleteCommandParams(SqlCommand sqlCommand, long id)
        {
            sqlCommand.Parameters.AddWithValue($"@{nameof(BaseEntity.Id)}", id);
        }

        /// <summary>
        /// Fill up the command with parameters.
        /// </summary>
        /// <param name="sqlCommand">The join sql command.</param>
        /// <param name="parentType">Type of parent entity.</param>
        /// <param name="parentId">Id of parent entity.</param>
        protected virtual void FillJoinCommandParams(SqlCommand sqlCommand, Type parentType, long parentId)
        {
            sqlCommand.Parameters.AddWithValue("@ParentId", parentId);
        }

        /// <summary>
        /// Get database table name
        /// </summary>
        /// <returns></returns>
        protected virtual string GetTableName()
        {
            return $"{typeof(T).Name}";
        }

        #endregion

        #region Public Methods

        public List<long> GetDeletedIds()
        {
            try
            {
                string sql = string.Format(@"
                SELECT [Id] FROM {0}
                WHERE [IsDeleted] = 1
                ", GetTableName());
                List<long> ids = new List<long>();

                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection) { CommandTimeout = 300 };

                    using (TypedDataReader dataReader = new TypedDataReader(sqlCommand.ExecuteReader()))
                    {
                        while (dataReader.Read())
                        {
                            long id = dataReader.GetInt64("Id");
                            ids.Add(id);
                        }
                    }

                    sqlConnection.Close();
                }

                return ids;
            }
            catch (Exception ex)
            {
                //LogBC.Instance.Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Get entity by its id.
        /// </summary>
        /// <param name="id">Entity's id.</param>
        /// <returns>The entity if it exists; otherwise, null.</returns>
        public T Get(long id)
        {
            try
            {
                string sql = GetSelectStatement();
                if (sql == string.Empty)
                {
                    return null;
                }

                T entity = null;

                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection) { CommandTimeout = 300 };

                    FillSelectCommandParams(sqlCommand, id);

                    using (TypedDataReader dataReader = new TypedDataReader(sqlCommand.ExecuteReader()))
                    {
                        if (dataReader.Read())
                        {
                            entity = CreateEntity(dataReader);
                        }
                    }

                    sqlConnection.Close();
                }

                return entity;
            }
            catch (Exception ex)
            {
                //LogBC.Instance.Log(ex);
                throw;
            }
        }

        public T GetIncludeDeleted(long id)
        {
            try
            {
                string sql = $@"
                SELECT * FROM {GetTableName()}
                WITH(NOLOCK) WHERE [Id] = @Id
                ";
                T entity = null;

                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection) { CommandTimeout = 300 };
                    sqlCommand.Parameters.AddWithValue("@Id", id);

                    using (TypedDataReader dataReader = new TypedDataReader(sqlCommand.ExecuteReader()))
                    {
                        if (dataReader.Read())
                        {
                            entity = CreateEntity(dataReader);
                        }
                    }

                    sqlConnection.Close();
                }

                return entity;
            }
            catch (Exception ex)
            {
                //LogBC.Instance.Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Get all entities
        /// </summary>      
        /// <returns>Entity collection</returns>
        public List<T> GetAll()
        {
            try
            {
                string sql = GetSelectAllStatement();
                if (string.IsNullOrEmpty(sql))
                {
                    return new List<T>();
                }

                List<T> ts = new List<T>();

                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection) { CommandTimeout = 300 };
                    using (TypedDataReader dataReader = new TypedDataReader(sqlCommand.ExecuteReader()))
                    {
                        while (dataReader.Read())
                        {
                            T entity = CreateEntity(dataReader);
                            ts.Add(entity);
                        }
                    }

                    sqlConnection.Close();
                }

                return ts;
            }
            catch (Exception ex)
            {
                //LogBC.Instance.Log(ex);
                throw;
            }
        }

        public List<T> GetAllIncludeDeleted()
        {
            try
            {
                string sql = string.Format(@"
                SELECT * FROM {0}
                ", GetTableName());
                List<T> ts = new List<T>();

                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection) { CommandTimeout = 300 };

                    using (TypedDataReader dataReader = new TypedDataReader(sqlCommand.ExecuteReader()))
                    {
                        while (dataReader.Read())
                        {
                            T entity = CreateEntity(dataReader);
                            entity.IsDeleted = dataReader.GetBoolean("IsDeleted");
                            ts.Add(entity);
                        }
                    }

                    sqlConnection.Close();
                }

                return ts;
            }
            catch (Exception ex)
            {
                //LogBC.Instance.Log(ex);
                throw;
            }
        }

        public List<T> GetByIds(List<long> ids)
        {
            try
            {
                if (ids.Count == 0) return new List<T>();
                string sql = string.Format(@"
                SELECT * FROM {0}
                WHERE [Id] IN ({1})", GetTableName(), GetCommaIds(ids));
                List<T> ts = new List<T>();

                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection) { CommandTimeout = 300 };

                    using (TypedDataReader dataReader = new TypedDataReader(sqlCommand.ExecuteReader()))
                    {
                        while (dataReader.Read())
                        {
                            T entity = CreateEntity(dataReader);
                            ts.Add(entity);
                        }
                    }

                    sqlConnection.Close();
                }

                return ts;
            }
            catch (Exception ex)
            {
                //LogBC.Instance.Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Insert entity into the database
        /// </summary>
        /// <param name="entity">entity</param>
        /// <param name="transaction">The transaction from outer scope, if any</param>
        public virtual long Insert(T entity, SqlTransaction transaction = null)
        {
            SqlConnection sqlConnection = null;
            try
            {
                long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                entity.CreatedDate = now;
                entity.ModifiedDate = now;

                string sql = GetInsertStatement();
                if (string.IsNullOrEmpty(sql)) return 0;

                long insertedId = 0;
                sqlConnection = transaction == null ? new SqlConnection(ConnectionString) : transaction.Connection;
                if (transaction == null) sqlConnection.Open();
                SqlCommand sqlCommand = transaction == null ? new SqlCommand(sql, sqlConnection) : new SqlCommand(sql, sqlConnection, transaction);
                FillInsertCommandParams(sqlCommand, entity);

                foreach (SqlParameter parameter in sqlCommand.Parameters)
                {
                    if (parameter.Value == null) parameter.Value = DBNull.Value;
                }

                object obj = sqlCommand.ExecuteScalar();
                if (obj != null) insertedId = long.Parse(obj.ToString());
                AfterInsert(entity, transaction);

                return insertedId;
            }
            catch (Exception ex)
            {
                //LogBC.Instance.Log(ex);
                throw;
            }
            finally
            {
                //if no transaction used, then release the connection to prevent memory leak
                //otherwise, that's responsibility of outer scope
                if (sqlConnection != null && transaction == null) sqlConnection.Dispose();
            }
        }

        public virtual void AfterInsert(T entity, SqlTransaction transaction = null)
        {

        }

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">The input entity</param>
        /// <param name="transaction">The transaction from outer scope, if any</param>
        public virtual long Update(T entity, SqlTransaction transaction = null)
        {
            SqlConnection sqlConnection = null;
            try
            {
                entity.ModifiedDate = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                string sql = GetUpdateStatement();
                if (string.IsNullOrEmpty(sql)) return 0;

                sqlConnection = transaction == null ? new SqlConnection(ConnectionString) : transaction.Connection;
                if (transaction == null) sqlConnection.Open();
                SqlCommand sqlCommand = transaction == null ? new SqlCommand(sql, sqlConnection) : new SqlCommand(sql, sqlConnection, transaction);
                FillUpdateCommandParams(sqlCommand, entity);

                foreach (SqlParameter parameter in sqlCommand.Parameters)
                {
                    if (parameter.Value == null) parameter.Value = DBNull.Value;
                }

                sqlCommand.ExecuteNonQuery();
                AfterUpdate(entity, transaction);

                return entity.Id;
            }
            catch (Exception ex)
            {
                //LogBC.Instance.Log(ex);
                throw;
            }
            finally
            {
                //if no transaction used, then release the connection to prevent memory leak
                //otherwise, that's responsibility of outer scope
                if (sqlConnection != null && transaction == null) sqlConnection.Dispose();
            }
        }

        public virtual void BulkUpdate(List<T> ts)
        {
            if (ts.Count == 0) return;
            SqlConnection sqlConnection = null;

            try
            {
                List<string> fields = GetTableFieldNames();
                string sql = string.Empty;
                sqlConnection = new SqlConnection(ConnectionString);
                sqlConnection.Open();

                foreach (T t in ts)
                {
                    t.ModifiedDate = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    List<string> setters = new List<string>();

                    foreach (string field in fields)
                    {
                        if (field.ToLower() == "id") continue;
                        PropertyInfo property = GetProperty(t, field, fields);
                        var obj = ConvertEntity2DB(property, t);

                        if (obj == null) setters.Add($"[{field}] = NULL");
                        else
                        {
                            string o_str = obj.ToString().Replace("'", "''");
                            if (obj.IsNumber()) setters.Add($"[{field}] = {o_str}");
                            else setters.Add($"[{field}] = '{o_str}'");
                        }
                    }

                    string tsql = $@"
                    UPDATE {GetTableName()}
                    SET {Helper.ToCSV(setters)}
                    WHERE [{nameof(BaseEntity.Id)}] = {t.Id}";

                    sql += $@"
                    {tsql}
                    ";
                }

                SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection);
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                //LogBC.Instance.Log(ex);
                throw;
            }
            finally
            {
                if (sqlConnection != null) sqlConnection.Dispose();
            }
        }

        public virtual void AfterUpdate(T entity, SqlTransaction transaction = null)
        {

        }

        /// <summary>
        /// Soft delete an entity
        /// </summary>
        /// <param name="id">identity of entity</param>
        public virtual long Delete(long id, SqlTransaction transaction = null)
        {
            SqlConnection sqlConnection = null;
            try
            {
                string sql = GetDeleteStatement();
                if (string.IsNullOrEmpty(sql)) return 0;

                sqlConnection = transaction == null ? new SqlConnection(ConnectionString) : transaction.Connection;
                if (transaction == null) sqlConnection.Open();
                SqlCommand sqlCommand = transaction == null ? new SqlCommand(sql, sqlConnection) : new SqlCommand(sql, sqlConnection, transaction);
                FillDeleteCommandParams(sqlCommand, id);

                sqlCommand.ExecuteNonQuery();
                AfterDelete(id, transaction);
                return id;
            }
            catch (Exception ex)
            {
                //LogBC.Instance.Log(ex);
                throw;
            }
            finally
            {
                //if no transaction used, then release the connection to prevent memory leak
                //otherwise, that's responsibility of outer scope
                if (sqlConnection != null && transaction == null) sqlConnection.Dispose();
            }
        }

        public virtual void AfterDelete(long id, SqlTransaction transaction = null)
        {

        }

        public virtual long DeleteByIds(List<long> ids)
        {
            try
            {
                string sql = $@"
                UPDATE {GetTableName()}
                SET [IsDeleted] = 1
                WHERE [Id] IN ({Helper.ToCSV(ids)})
                ";
                int numberOfRowsAffedted = 0;

                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection) { CommandTimeout = 300 };
                    numberOfRowsAffedted = sqlCommand.ExecuteNonQuery();
                    sqlConnection.Close();
                }

                return numberOfRowsAffedted;
            }
            catch (Exception ex)
            {
                //LogBC.Instance.Log(ex);
                throw;
            }
        }

        public int DeleteReal(long id)
        {
            try
            {
                string sql = string.Format(@"
                DELETE FROM {0}
                WHERE [Id] = @Id
                ", GetTableName());
                int numberOfRowsAffedted = 0;

                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection) { CommandTimeout = 300 };
                    sqlCommand.Parameters.AddWithValue("@Id", id);

                    numberOfRowsAffedted = sqlCommand.ExecuteNonQuery();
                    sqlConnection.Close();
                }

                return numberOfRowsAffedted;
            }
            catch (Exception ex)
            {
                //LogBC.Instance.Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Get list of entities joined by the table with its parent table.
        /// </summary>
        /// <param name="parentType">Type of parent entity.</param>
        /// <param name="parentId">Id of parent entity.</param>
        /// <returns>List of entities.</returns>
        public List<T> Join(Type parentType, long parentId)
        {
            List<T> ts = new List<T>();
            string sqlTrace = "";
            try
            {
                string sql = GetJoinStatement(parentType);
                if (string.IsNullOrEmpty(sql))
                {
                    return new List<T>();
                }
                sqlTrace = sql;
                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection) { CommandTimeout = 300 };

                    FillJoinCommandParams(sqlCommand, parentType, parentId);

                    using (TypedDataReader dataReader = new TypedDataReader(sqlCommand.ExecuteReader()))
                    {
                        while (dataReader.Read())
                        {
                            ts.Add(CreateEntity(dataReader));
                        }

                        dataReader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(sqlTrace);
                //LogBC.Instance.Log(ex);
                throw;
            }

            return ts;
        }

        /// <summary>
        /// Get entities number satisfied the AND conditions fieldNames[i] = fieldValues[i]
        /// </summary>
        /// <param name="fieldNames">Array of field names</param>
        /// <param name="fieldValues">Array of field values</param>
        /// <returns>The result count</returns>
        public int CountByFields(string[] fieldNames, object[] fieldValues)
        {
            try
            {
                string clause = " 1 = 1 ";

                for (int i = 0; i < fieldNames.Length; i++)
                {
                    if (fieldNames[i] == "*" && fieldValues[i] is Condition)
                    {
                        clause += $" AND ({(fieldValues[i] as Condition).condition}) ";
                    }
                    else
                    {
                        if (fieldValues[i] == null)
                        {
                            clause += $" AND {fieldNames[i]} IS NULL ";
                        }
                        else
                        {
                            if (fieldValues[i] is Condition) clause += $" AND {fieldNames[i]} {(fieldValues[i] as Condition).condition}";
                            else clause += $" AND {fieldNames[i]} = @{fieldNames[i].Replace("[", "").Replace("]", "")}";
                        }
                    }
                }

                string sql = $@"
                SELECT COUNT(*) FROM {GetTableName()} 
                WHERE {clause} 
                AND [IsDeleted] = 0";
                int count = 0;

                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection) { CommandTimeout = 300 };

                    for (int i = 0; i < fieldNames.Length; i++)
                    {
                        if (fieldValues[i] is Condition)
                        {
                            sqlCommand.Parameters.AddWithValue($"@{fieldNames[i].Replace("[", "").Replace("]", "")}", (fieldValues[i] as Condition).condition);
                        }
                        else
                        {
                            sqlCommand.Parameters.AddWithValue($"@{fieldNames[i].Replace("[", "").Replace("]", "")}", fieldValues[i]);
                        }
                    }

                    foreach (SqlParameter parameter in sqlCommand.Parameters)
                    {
                        if (parameter.Value == null)
                        {
                            parameter.Value = DBNull.Value;
                        }
                    }

                    object obj = sqlCommand.ExecuteScalar();
                    count = Int32.Parse(obj.ToString());
                    sqlConnection.Close();
                }

                return count;
            }
            catch (Exception ex)
            {
                //LogBC.Instance.Log(ex);
                throw;
            }
        }

        public int CountByFields(string[] fieldNames, object[] fieldValues, string dateField, string fromDate, string toDate)
        {
            try
            {
                string clause = " 1 = 1 ";

                foreach (string fieldName in fieldNames)
                {
                    clause += $" AND {fieldName} = @{fieldName.Replace("[", "").Replace("]", "")}";
                }

                string sql = $@"
                SELECT COUNT(*) FROM {GetTableName()} 
                WHERE {clause} AND {dateField} BETWEEN @FromDate AND @ToDate AND [IsDeleted] = 0";
                int count = 0;

                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection) { CommandTimeout = 300 };

                    for (int i = 0; i < fieldNames.Length; i++)
                    {
                        sqlCommand.Parameters.AddWithValue($"@{fieldNames[i].Replace("[", "").Replace("]", "")}", fieldValues[i]);
                    }
                    sqlCommand.Parameters.AddWithValue("@FromDate", fromDate);
                    sqlCommand.Parameters.AddWithValue("@ToDate", toDate);

                    foreach (SqlParameter parameter in sqlCommand.Parameters)
                    {
                        if (parameter.Value == null)
                        {
                            parameter.Value = DBNull.Value;
                        }
                    }

                    object obj = sqlCommand.ExecuteScalar();
                    count = Int32.Parse(obj.ToString());
                    sqlConnection.Close();
                }

                return count;
            }
            catch (Exception ex)
            {
                //LogBC.Instance.Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Get first entity satisfied the AND conditions fieldNames[i] = fieldValues[i] <para/>
        /// Examples: <para/>
        /// student = StudentDAC.Instance.GetSingleByFields(new string[]{"Age", "Gender"}, new object[]{20, "male"}); <para/>
        /// kitchen = KitchenDAC.Instance.GetSingleByFields(new string[]{"Email"}, new object[]{new Condition("LIKE N'%bob%'")}); <para/>
        /// student = StudentDAC.Instance.GetSingleByFields(new string[]{"Age", "Grade"}, new object[]{new Condition(">20"), new Condition("< 8.5")}); <para/>
        /// student = StudentDAC.Instance.GetSingleByFields(new string[]{"Age"}, new object[]{new Condition(">=20"), new Condition("ORDER BY [Name] ASC")}); <para/>
        /// </summary>
        /// <param name="fieldNames">Array of field names</param>
        /// <param name="fieldValues">Array of field values</param>
        /// <returns>The result entity</returns>
        public T GetSingleByFields(string[] fieldNames, object[] fieldValues)
        {
            List<T> ts = GetListByFields(fieldNames, fieldValues);
            return ts.Count > 0 ? ts[0] : null;
        }

        public List<long> GetAllIds()
        {
            try
            {
                string sql = $@"
                SELECT [Id] FROM {GetTableName()} 
                ";

                List<long> ts = new List<long>();

                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection) { CommandTimeout = 300 };

                    using (TypedDataReader dataReader = new TypedDataReader(sqlCommand.ExecuteReader()))
                    {
                        while (dataReader.Read())
                        {
                            long id = dataReader.GetInt64("Id");
                            ts.Add(id);
                        }
                    }

                    sqlConnection.Close();
                }

                return ts;
            }
            catch (Exception ex)
            {
                //LogBC.Instance.Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Get all entities satisfied the AND conditions fieldNames[i] = fieldValues[i] <para/>
        /// Examples: <para/>
        /// students = StudentDAC.Instance.GetListByFields(new string[]{"Age", "Gender"}, new object[]{20, "male"}); <para/>
        /// kitchens = KitchenDAC.Instance.GetListByFields(new string[]{"Email"}, new object[]{new Condition("LIKE N'%bob%'")}); <para/>
        /// students = StudentDAC.Instance.GetListByFields(new string[]{"Age", "Grade"}, new object[]{new Condition(">20"), new Condition("< 8.5")}); <para/>
        /// students = StudentDAC.Instance.GetListByFields(new string[]{"Age"}, new object[]{new Condition(">=20"), new Condition("ORDER BY [Name] ASC")}); <para/>
        /// 
        /// Pagination example: get 10 students of class 12 of page 3 <para/>
        /// students = StudentDAC.Instance.GetListByFields(new string[]{"ClassId"}, new object[]{new Condition("cls-12"), new Condition("ORDER BY [CreatedDate] ASC")}, 20, 10);
        /// </summary>
        /// <param name="fieldNames">Array of field names</param>
        /// <param name="fieldValues">Array of field values</param>
        /// <param name="getDeletedValues">Optional parameter. Boolean value indicating whether to fetch soft deleted records or not.</param>
        /// <param name="fromOffset">Optional parameter. Used for pagination. Only fetch results from specified offset.</param>
        /// <param name="rowCount">Optional parameter. Used for pagination. Only fetch maximum rowCount records.</param>
        /// <returns>The result entities</returns>
        public List<T> GetListByFields(string[] fieldNames, object[] fieldValues, bool getDeletedValues = false, int fromOffset = 0, int rowCount = 0)
        {
            try
            {
                string clause = " 1 = 1 ";
                string deletedClause = getDeletedValues ? "" : $" AND [{nameof(BaseEntity.IsDeleted)}] = 0";

                for (int i = 0; i < fieldNames.Length; i++)
                {
                    if (fieldNames[i] == "*" && fieldValues[i] is Condition)
                    {
                        clause += $" AND ({(fieldValues[i] as Condition).condition}) ";
                    }
                    else
                    {
                        if (fieldValues[i] == null)
                        {
                            clause += $" AND {fieldNames[i]} IS NULL ";
                        }
                        else
                        {
                            if (fieldValues[i] is Condition) clause += $" AND {fieldNames[i]} {(fieldValues[i] as Condition).condition}";
                            else clause += $" AND {fieldNames[i]} = @{fieldNames[i].Replace("[", "").Replace("]", "")}";
                        }
                    }
                }

                string postCondition = string.Empty;
                Condition orderBy = null;

                for (int i = fieldNames.Length; i < fieldValues.Length; i++)
                {
                    //if (fieldNames[i] == "*") continue;
                    if (fieldValues[i] is Condition)
                    {
                        Condition condition = fieldValues[i] as Condition;
                        postCondition += $"{condition.condition} ";
                        if (condition.condition.ToLower().Contains("order by")) orderBy = condition;
                    }
                }

                if (fromOffset >= 0 && rowCount > 0)
                {
                    //pagination
                    if (orderBy == null)
                    {
                        postCondition += " ORDER BY [Id] DESC ";
                    }
                    postCondition += $@" OFFSET {fromOffset}
                    ROWS FETCH NEXT {rowCount}
                    ROWS ONLY ";
                }

                string sql = $@"
                SELECT * FROM {GetTableName()} 
                WHERE {clause} 
                {deletedClause}
                {postCondition}
                ";

                List<T> ts = new List<T>();

                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection) { CommandTimeout = 300 };

                    for (int i = 0; i < fieldNames.Length; i++)
                    {
                        if (fieldNames[i] == "*") continue;
                        if (!(fieldValues[i] is Condition)) sqlCommand.Parameters.AddWithValue($"@{fieldNames[i].Replace("[", "").Replace("]", "")}", fieldValues[i]);
                    }

                    foreach (SqlParameter parameter in sqlCommand.Parameters)
                    {
                        if (parameter.Value == null) parameter.Value = DBNull.Value;
                    }

                    using (TypedDataReader dataReader = new TypedDataReader(sqlCommand.ExecuteReader()))
                    {
                        while (dataReader.Read())
                        {
                            T entity = CreateEntity(dataReader);
                            ts.Add(entity);
                        }
                    }

                    sqlConnection.Close();
                }

                return ts;
            }
            catch (Exception ex)
            {
                //LogBC.Instance.Log(ex);
                throw;
            }
        }

        public List<T> JoinByParentIds(string parentColumn, List<long> parentIds)
        {
            List<T> ts = new List<T>();
            try
            {
                string sql = string.Format(@"
                SELECT * FROM {0}
                WHERE {1} IN ({2})
                AND [IsDeleted] = 0", GetTableName(), parentColumn, GetCommaIds(parentIds));

                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection) { CommandTimeout = 300 };

                    using (TypedDataReader dataReader = new TypedDataReader(sqlCommand.ExecuteReader()))
                    {
                        while (dataReader.Read())
                        {
                            ts.Add(CreateEntity(dataReader));
                        }

                        dataReader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                //LogBC.Instance.Log(ex);
                throw;
            }

            return ts;
        }

        /// <summary>
        /// Get all entities which column has value in the input value list
        /// </summary>
        /// <typeparam name="V">The value type</typeparam>
        /// <param name="column">The column name to filter</param>
        /// <param name="values">The input value list</param>
        /// <returns>All entities satisfied the condition</returns>
        public List<T> WhereColumnIn<V>(string column, List<V> values, bool discardDeletedEntities = true)
        {
            List<T> ts = new List<T>();
            try
            {
                if (values.Count == 0) return new List<T>();
                var deleteStatement = discardDeletedEntities ? $" AND [{nameof(BaseEntity.IsDeleted)}] = 0" : string.Empty;
                string sql = $@"
                SELECT * FROM {GetTableName()}
                WHERE {column} IN ({Helper.ToCSV<V>(values)})
                {deleteStatement}
                ";

                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection) { CommandTimeout = 300 };

                    using (TypedDataReader dataReader = new TypedDataReader(sqlCommand.ExecuteReader()))
                    {
                        while (dataReader.Read())
                        {
                            ts.Add(CreateEntity(dataReader));
                        }

                        dataReader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                //LogBC.Instance.Log(ex);
                throw;
            }

            return ts;
        }

        /// <summary>
        /// Get all entities which column has value in the input value array
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="columns"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public List<T> WhereColumnsIn(string[] columns, dynamic[][] values)
        {
            List<T> ts = new List<T>();
            try
            {
                if (values.Length == 0) return new List<T>();

                string clause = " 1 = 1 ";

                for (int i = 0; i < columns.Length; i++)
                {
                    clause += $@"
                    AND {columns[i]} IN ({Helper.ToCSV(values[i].ToList())})
                    ";
                }

                string sql = $@"
                SELECT * FROM {GetTableName()}
                WHERE {clause}
                AND [{nameof(BaseEntity.IsDeleted)}] = 0
                ";

                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection) { CommandTimeout = 300 };

                    using (TypedDataReader dataReader = new TypedDataReader(sqlCommand.ExecuteReader()))
                    {
                        while (dataReader.Read())
                        {
                            ts.Add(CreateEntity(dataReader));
                        }

                        dataReader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                //LogBC.Instance.Log(ex);
                throw;
            }

            return ts;
        }

        /// <summary>
        /// Checks if an entity exists in database or not.
        /// </summary>
        /// <param name="entity">entity</param>
        /// <returns></returns>
        public bool Exist(long id)
        {
            try
            {
                T e = Get(id);
                return e != null;
            }
            catch (Exception ex)
            {
                //LogBC.Instance.Log(ex);
                throw;
            }
        }

        public string GetCommaIds(List<long> ids)
        {
            string strIds = "-1";
            foreach (int id in ids)
            {
                strIds += "," + id;
            }

            return strIds;
        }

        public SchemaDB GetDatabaseSchema()
        {
            try
            {
                SchemaDB schema = new SchemaDB();
                string sql = $@"
                SELECT TABLE_NAME, COLUMN_NAME
                FROM INFORMATION_SCHEMA.COLUMNS
                ";

                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection) { CommandTimeout = 300 };

                    using (TypedDataReader dataReader = new TypedDataReader(sqlCommand.ExecuteReader()))
                    {
                        while (dataReader.Read())
                        {
                            string table = dataReader.GetString("TABLE_NAME");
                            string col = dataReader.GetString("COLUMN_NAME");
                            SchemaTable schemaTable = schema.Tables.Find(t => t.Name.Equals(table, StringComparison.CurrentCultureIgnoreCase));
                            if (schemaTable == null)
                            {
                                schemaTable = new SchemaTable() { Name = table };
                                schema.Tables.Add(schemaTable);
                            }
                            if (schemaTable.Fields.Find(t => t.Equals(col, StringComparison.CurrentCultureIgnoreCase)) == null)
                            {
                                schemaTable.Fields.Add(col);
                            }
                        }

                        dataReader.Close();
                    }
                }

                return schema;
            }
            catch (Exception ex)
            {
                //LogBC.Instance.Log(ex);
                throw;
            }
        }

        private long getTableFieldNameCalculationTime = 0;
        private List<string> tableFieldNames = new List<string>();

        public List<string> GetTableFieldNames()
        {
            try
            {
                long delta = DateTimeOffset.Now.ToUnixTimeMilliseconds() - getTableFieldNameCalculationTime;
                if (delta < 20 * 1000)
                {
                    return tableFieldNames;
                }

                string tb = GetTableName().Replace("dbo.", "").Replace("[", "").Replace("]", "");
                List<string> names = new List<string>();
                string sql = $@"
                SELECT COLUMN_NAME
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_NAME = N'{tb}'
                ";

                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection) { CommandTimeout = 300 };

                    using (TypedDataReader dataReader = new TypedDataReader(sqlCommand.ExecuteReader()))
                    {
                        while (dataReader.Read())
                        {
                            string col = dataReader.GetString("COLUMN_NAME");
                            names.Add(col);
                        }

                        dataReader.Close();
                    }
                }

                tableFieldNames = names;
                getTableFieldNameCalculationTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                return names;
            }
            catch (Exception ex)
            {
                //LogBC.Instance.Log(ex);
                throw;
            }
        }

        public long GetMaxPrimaryKey()
        {
            try
            {
                string sql = $@"
                SELECT MAX({nameof(BaseEntity.Id)})
                FROM {GetTableName()}
                ";

                long maxValue = 0;
                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection) { CommandTimeout = 300 };

                    using (TypedDataReader dataReader = new TypedDataReader(sqlCommand.ExecuteReader()))
                    {
                        while (dataReader.Read())
                        {
                            maxValue = dataReader.GetInt64(0);
                        }

                        dataReader.Close();
                    }
                }

                return maxValue;
            }
            catch (Exception ex)
            {
                //LogBC.Instance.Log(ex);
                throw;
            }
        }

        public void DisableIndex()
        {
            try
            {
                string sql = $@"
                ALTER INDEX ALL ON {GetTableName()}
                DISABLE
                ";

                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection) { CommandTimeout = 300 };
                    sqlCommand.ExecuteNonQuery();

                }
            }
            catch (Exception ex)
            {
                //LogBC.Instance.Log(ex);
                throw;
            }
        }

        public void EnableIndex()
        {
            try
            {
                string sql = $@"
                ALTER INDEX ALL ON {GetTableName()}
                REBUILD
                ";

                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection) { CommandTimeout = 300 };

                    sqlCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                //LogBC.Instance.Log(ex);
                throw;
            }
        }

        protected object GetValue(object value)
        {
            if (value == null) return "NULL";
            if (value is string)
            {
                value = value.ToString().Replace("'", "''");
                return $"N'{value}'";
            }

            if (value is bool)
            {
                return ((bool)value) ? 1 : 0;
            }

            return value.ToString();
        }

        public int CountSqlCol(string sql, string col, int value)
        {
            try
            {
                int count = 0;
                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection) { CommandTimeout = 300 };
                    sqlCommand.Parameters.AddWithValue(col, value);
                    object obj = sqlCommand.ExecuteScalar();
                    count = Int32.Parse(obj.ToString());
                    sqlConnection.Close();
                }
                return count;
            }
            catch (Exception ex)
            {
                //LogBC.Instance.Log(ex);
                throw;
            }
        }

        public void BulkInsertJsons(List<string> jsons)
        {
            List<T> ts = new List<T>();
            foreach (string json in jsons)
            {
                T t = JsonConvert.DeserializeObject<T>(json);
                ts.Add(t);
            }

            BulkInsert(ts);
        }

        public void BulkInsert(List<T> ts)
        {
            try
            {
                if (ts.Count == 0) return;
                string[] copyParameters = GetTableFieldNames().ToArray();
                var table = new DataTable();

                using (SqlConnection connection = new SqlConnection(Config.Instance.ConnectionString))
                {
                    connection.Open();

                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                    {
                        bulkCopy.DestinationTableName = GetTableName();
                        using (var adapter = new SqlDataAdapter($"SELECT TOP 0 * FROM " + GetTableName(), connection))
                        {
                            adapter.Fill(table);
                        };

                        bulkCopy.BatchSize = 5000;
                        foreach (T t in ts)
                        {
                            var row = table.NewRow();
                            foreach (string field in copyParameters)
                            {
                                dynamic value = t.GetType().GetProperty(field).GetValue(t);
                                value = value ?? (value == 0 ? value : DBNull.Value);
                                row[field] = value;
                            }

                            table.Rows.Add(row);
                        }

                        //var fields = copyParameters;
                        bulkCopy.WriteToServer(table);
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                //LogBC.Instance.Log(ex);
                throw;
            }
        }

        public void Truncate()
        {
            try
            {
                string sql = $@"
                TRUNCATE TABLE {GetTableName()}
                ";

                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection) { CommandTimeout = 300 };
                    sqlCommand.ExecuteNonQuery();

                }
            }
            catch (Exception ex)
            {
                //LogBC.Instance.Log(ex);
                throw;
            }
        }

        #endregion
    }
}