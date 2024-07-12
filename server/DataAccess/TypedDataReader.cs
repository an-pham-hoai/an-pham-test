using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Server.DataAccess
{
   /// <summary>
   /// This class is a strong typed DataReader.
   /// </summary>
   public class TypedDataReader : IDataReader
   {
      #region Fields

      private IDataReader _dataReader;

      #endregion

      #region Properties

      /// <summary>
      /// Get a reference to the real data reader.
      /// </summary>
      protected IDataReader DataReader
      {
         get { return _dataReader; }
      }

      #endregion

      #region Constructors

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="dataReader">The real DataReader object containing the data.</param>
      public TypedDataReader(IDataReader dataReader)
      {
         _dataReader = dataReader;
      }

      #endregion

      #region IdataRecord

      /// <summary>
      /// Gets a boolean value from the datareader.
      /// </summary>
      /// <param name="name">Name of the column containing the value.</param>
      /// <returns>Returns false for null.</returns>
      public bool GetBoolean(string name)
      {
         return GetBoolean(_dataReader.GetOrdinal(name));
      }

      /// <summary>
      /// Gets a boolean value from the datareader.
      /// </summary>
      /// <param name="i">Ordinal column position of the value.</param>
      /// <returns>Returns false for null.</returns>
      public virtual bool GetBoolean(int i)
      {
         if (IsDBNull(i))
         {
            return false;
         }
         {
            return _dataReader.GetBoolean(i);
         }
      }

      /// <summary>
      /// Gets a char value from the datareader.
      /// </summary>
      /// <param name="name">Name of the column containing the value.</param>
      /// <returns>Returns Char.MinValue for null.</returns>
      public char GetChar(string name)
      {
         return GetChar(_dataReader.GetOrdinal(name));
      }

      /// <summary>
      /// Gets a byte value from the datareader.
      /// </summary>
      /// <param name="i">Ordinal column position of the value.</param>
      /// <returns>Char.MinValue for null.</returns>
      public virtual char GetChar(int i)
      {
         if (IsDBNull(i))
         {
            return char.MinValue;
         }
         else
         {
            char[] myChar = new char[1];
            _dataReader.GetChars(i, 0, myChar, 0, 1);

            return myChar[0];
         }
      }

      /// <summary>
      /// Gets a string value from the datareader.
      /// </summary>
      /// <param name="name">Name of the column containing the value.</param>
      /// <returns>Returns empty string for null.</returns>
      public string GetString(string name)
      {
         return GetString(_dataReader.GetOrdinal(name));
      }

      /// <summary>
      /// Gets a string value from the datareader.
      /// </summary>
      /// <param name="i">Ordinal column position of the value.</param>
      /// <returns>Returns empty string for null.</returns>
      public virtual string GetString(int i)
      {
         if (IsDBNull(i))
         {
            return string.Empty;
         }
         else
         {
            return _dataReader.GetString(i);
         }
      }

      /// <summary>
      /// Gets a byte value from the datareader.
      /// </summary>
      /// <param name="name">Name of the column containing the value.</param>
      /// <returns>Returns 0 for null.</returns>
      public byte GetByte(string name)
      {
         return GetByte(_dataReader.GetOrdinal(name));
      }

      /// <summary>
      /// Gets a byte value from the datareader.
      /// </summary>
      /// <param name="i">Ordinal column position of the value.</param>
      /// <returns>Returns 0 for null.</returns>
      public virtual byte GetByte(int i)
      {
         if (IsDBNull(i))
         {
            return 0;
         }
         else
         {
            return _dataReader.GetByte(i);
         }
      }

      /// <summary>
      /// Gets an short value from the datareader.
      /// </summary>
      /// <param name="name">Name of the column containing the value.</param>
      /// <returns>Returns 0 for null.</returns>      
      public short GetInt16(string name)
      {
         return GetInt16(_dataReader.GetOrdinal(name));
      }

      /// <summary>
      /// Gets a short value from the datareader.
      /// </summary>
      /// <param name="i">Ordinal column position of the value.</param>
      /// <returns>Returns 0 for null.</returns>      
      public virtual short GetInt16(int i)
      {
         if (IsDBNull(i))
         {
            return 0;
         }
         else
         {
            return _dataReader.GetInt16(i);
         }
      }

      /// <summary>
      /// Gets an integer value from the datareader.
      /// </summary>
      /// <param name="name">Name of the column containing the value.</param>
      /// <returns>Returns 0 for null.</returns>      
      public int GetInt32(string name)
      {
         return GetInt32(_dataReader.GetOrdinal(name));
      }

      /// <summary>
      /// Gets an integer value from the datareader.
      /// </summary>
      /// <param name="i">Ordinal column position of the value.</param>
      /// <returns>Returns 0 for null.</returns>      
      public virtual int GetInt32(int i)
      {
         if (IsDBNull(i))
         {
            return 0;
         }
         else
         {
            return _dataReader.GetInt32(i);
         }
      }

      /// <summary>
      /// Gets a long value from the datareader.
      /// </summary>
      /// <param name="name">Name of the column containing the value.</param>
      /// <returns>Returns 0 for null.</returns>      
      public Int64 GetInt64(string name)
      {
         return GetInt64(_dataReader.GetOrdinal(name));
      }

      /// <summary>
      /// Gets a long value from the datareader.
      /// </summary>
      /// <param name="i">Ordinal column position of the value.</param>
      /// <returns>Returns 0 for null.</returns>      
      public virtual Int64 GetInt64(int i)
      {
         if (IsDBNull(i))
         {
            return 0;
         }
         else
         {
            return _dataReader.GetInt64(i);
         }
      }

      /// <summary>
      /// Gets a float value from the datareader.
      /// </summary>
      /// <param name="name">Name of the column containing the value.</param>
      /// <returns>Returns 0 for null.</returns>      
      public float GetFloat(string name)
      {
         return GetFloat(_dataReader.GetOrdinal(name));
      }

      /// <summary>
      /// Gets a float value from the datareader.
      /// </summary>
      /// <param name="i">Ordinal column position of the value.</param>
      /// <returns>Returns 0 for null.</returns>      
      public virtual float GetFloat(int i)
      {
         if (IsDBNull(i))
         {
            return 0;
         }
         else
         {
            return _dataReader.GetFloat(i);
         }
      }

      /// <summary>
      /// Gets a double value from the datareader.
      /// </summary>
      /// <param name="name">Name of the column containing the value.</param>
      /// <returns>Returns 0 for null.</returns>      
      public double GetDouble(string name)
      {
         return GetDouble(_dataReader.GetOrdinal(name));
      }

      /// <summary>
      /// Gets a double value from the datareader.
      /// </summary>
      /// <param name="i">Ordinal column position of the value.</param>
      /// <returns>Returns 0 for null.</returns>      
      public virtual double GetDouble(int i)
      {
         if (IsDBNull(i))
         {
            return 0;
         }
         else
         {
            return _dataReader.GetDouble(i);
         }
      }

      /// <summary>
      /// Gets a decimal value from the datareader.
      /// </summary>
      /// <param name="name">Name of the column containing the value.</param>
      /// <returns>Returns 0 for null.</returns>      
      public decimal GetDecimal(string name)
      {
         return GetDecimal(_dataReader.GetOrdinal(name));
      }

      /// <summary>
      /// Gets a decimal value from the datareader.
      /// </summary>
      /// <param name="i">Ordinal column position of the value.</param>
      /// <returns>Returns 0 for null.</returns>      
      public virtual decimal GetDecimal(int i)
      {
         if (IsDBNull(i))
         {
            return 0;
         }
         else
         {
            return _dataReader.GetDecimal(i);
         }
      }

      /// <summary>
      /// Gets a DateTime value from the datareader.
      /// </summary>
      /// <param name="name">Name of the column containing the value.</param>
      /// <returns>Returns DateTime.MinValue for null.</returns>      
      public virtual DateTime GetDateTime(string name)
      {
         return GetDateTime(_dataReader.GetOrdinal(name));
      }

      /// <summary>
      /// Gets a DateTime value from the datareader.
      /// </summary>
      /// <param name="i">Ordinal column position of the value.</param>
      /// <returns>Returns DateTime.MinValue for null.</returns>      
      public virtual DateTime GetDateTime(int i)
      {
         if (IsDBNull(i))
         {
            return DateTime.MinValue;
         }
         else
         {
            return _dataReader.GetDateTime(i);
         }
      }

      /// <summary>
      /// Gets an object value from the datareader.
      /// </summary>
      /// <param name="name">Name of the column containing the value.</param>
      /// <returns>Returns object.</returns>      
      public object GetValue(string name)
      {
         return GetValue(_dataReader.GetOrdinal(name));
      }

      /// <summary>
      /// Gets an object value from the datareader.
      /// </summary>
      /// <param name="i">Ordinal column position of the value.</param>
      /// <returns>Returns object.</returns>      
      public virtual object GetValue(int i)
      {
         if (IsDBNull(i))
         {
            return null;
         }
         else
         {
            return _dataReader.GetValue(i);
         }
      }

      /// <summary>
      /// Gets a Guid value from the datareader.
      /// </summary>
      /// <param name="name">Name of the column containing the value.</param>
      /// <returns>Returns Guid.Empty for null.</returns>      
      public System.Guid GetGuid(string name)
      {
         return GetGuid(_dataReader.GetOrdinal(name));
      }

      /// <summary>
      /// Gets a long value from the datareader.
      /// </summary>
      /// <param name="name">Name of the column containing the value.</param>
      /// <returns>Returns 0 for null.</returns>      
      public Guid? GetNullableGuid(string name)
      {
         return GetNullableGuid(_dataReader.GetOrdinal(name));
      }

      /// <summary>
      /// Gets a boolean value from the datareader.
      /// </summary>
      /// <param name="i">Ordinal column position of the value.</param>
      /// <returns>Returns false for null.</returns>
      public Guid? GetNullableGuid(int i)
      {
         if (IsDBNull(i))
         {
            return null;
         }
         else
         {
            return _dataReader.GetGuid(i);
         }
      }


      /// <summary>
      /// Gets a Guid value from the datareader.
      /// </summary>
      /// <param name="i">Ordinal column position of the value.</param>
      /// <returns>Returns Guid.Empty for null.</returns>      
      public virtual System.Guid GetGuid(int i)
      {
         if (IsDBNull(i))
         {
            return Guid.Empty;
         }
         else
         {
            return _dataReader.GetGuid(i);
         }
      }

      /// <summary>
      /// Check if column at possition i is a DBNull
      /// </summary>
      /// <param name="i">Ordinal column position of the value.</param>
      /// <returns>Return true for null.</returns>
      public virtual bool IsDBNull(int i)
      {
         return _dataReader.IsDBNull(i);
      }

      /// <summary>
      /// Invokes the GetData method of the underlying datareader.
      /// </summary>
      /// <param name="name">Name of the column containing the value.</param>
      /// <returns>IDataReader</returns>
      public IDataReader GetData(string name)
      {
         return GetData(_dataReader.GetOrdinal(name));
      }

      /// <summary>
      /// Invokes the GetData method of the underlying datareader.
      /// </summary>
      /// <param name="i">Ordinal column position of the value.</param>
      /// <returns>IDataReader</returns>
      public virtual IDataReader GetData(int i)
      {
         return _dataReader.GetData(i);
      }

      /// <summary>
      /// Invokes the GetDataTypeName method of the underlying datareader.
      /// </summary>
      /// <param name="name">Name of the column containing the value.</param>
      /// <returns>Data type name</returns>
      public string GetDataTypeName(string name)
      {
         return GetDataTypeName(_dataReader.GetOrdinal(name));
      }

      /// <summary>
      /// Invokes the GetDataTypeName method of the underlying datareader.
      /// </summary>
      /// <param name="i">Ordinal column position of the value.</param>
      /// <returns>Data type name</returns>
      public virtual string GetDataTypeName(int i)
      {
         return _dataReader.GetDataTypeName(i);
      }

      /// <summary>
      /// Invokes the GetFieldType method of the underlying datareader.
      /// </summary>
      /// <param name="name">Name of the column containing the value.</param>
      /// <returns>Field type</returns>
      public Type GetFieldType(string name)
      {
         return GetFieldType(_dataReader.GetOrdinal(name));
      }

      /// <summary>
      /// Invokes the GetFieldType method of the underlying datareader.
      /// </summary>
      /// <param name="i">Ordinal column position of the value.</param>
      /// <returns>Field type</returns>
      public virtual Type GetFieldType(int i)
      {
         return _dataReader.GetFieldType(i);
      }

      /// <summary>
      /// Invokes the GetName method of the underlying datareader.
      /// </summary>
      /// <param name="i">Ordinal column position of the value.</param>
      /// <returns>Name of column i</returns>
      public virtual string GetName(int i)
      {
         return _dataReader.GetName(i);
      }

      /// <summary>
      /// Gets an ordinal value from the datareader.
      /// </summary>
      /// <param name="name">Name of the column containing the value.</param>
      /// <returns>Ordinal of column name</returns>
      public int GetOrdinal(string name)
      {
         return _dataReader.GetOrdinal(name);
      }

      /// <summary>
      /// Invokes the GetValues method of the underlying datareader.
      /// </summary>
      public int GetValues(object[] values)
      {
         return _dataReader.GetValues(values);
      }


      /// <summary>
      /// Returns a value from the datareader.
      /// </summary>
      /// <param name="name">Name of the column containing the value.</param>
      public object this[string name]
      {
         get
         {
            object val = _dataReader[name];

            if (DBNull.Value.Equals(val))
            {
               return null;
            }
            else
            {
               return val;
            }
         }
      }

      /// <summary>
      /// Returns a value from the datareader.
      /// </summary>
      /// <param name="i">Ordinal column position of the value.</param>
      public virtual object this[int i]
      {
         get
         {
            if (IsDBNull(i))
            {
               return null;
            }
            else
            {
               return _dataReader[i];
            }
         }
      }

      /// <summary>
      /// Invokes the GetBytes method of the underlying datareader.
      /// </summary>
      /// <param name="name">Name of the column containing the value.</param>
      /// <param name="fieldOffset"></param>
      /// <param name="buffer"></param>
      /// <param name="bufferoffset"></param>
      /// <param name="length"></param>
      /// <returns>Returns 0 for null.</returns>

      public Int64 GetBytes(string name, Int64 fieldOffset, byte[] buffer, int bufferoffset, int length)
      {
         return GetBytes(_dataReader.GetOrdinal(name), fieldOffset, buffer, bufferoffset, length);
      }

      /// <summary>
      /// Invoke the GetBytes method of the realdatareader.
      /// </summary>
      /// <param name="i">Ordinal column position of the value.</param>
      /// <param name="fieldOffset"></param>
      /// <param name="buffer"></param>
      /// <param name="bufferoffset"></param>
      /// <param name="length"></param>
      /// <returns>Returns 0 for null.</returns>
      public virtual Int64 GetBytes(int i, Int64 fieldOffset, byte[] buffer, int bufferoffset, int length)
      {
         if (_dataReader.IsDBNull(i))
            return 0;
         else
            return _dataReader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
      }

      /// <summary>
      /// Invokes the GetChars method of the underlying datareader.
      /// </summary>
      /// <param name="name">Name of the column containing the value.</param>
      /// <param name="fieldoffset"></param>
      /// <param name="buffer"></param>
      /// <param name="bufferoffset"></param>
      /// <param name="length"></param>
      /// <returns>Returns 0 for null.</returns>
      public Int64 GetChars(string name, Int64 fieldoffset, char[] buffer, int bufferoffset, int length)
      {
         return GetChars(_dataReader.GetOrdinal(name), fieldoffset, buffer, bufferoffset, length);
      }

      /// <summary>
      /// Invokes the GetChars method of the underlying datareader.
      /// </summary>
      /// <param name="i">Ordinal column position of the value.</param>
      /// <param name="fieldoffset"></param>
      /// <param name="buffer"></param>
      /// <param name="bufferoffset"></param>
      /// <param name="length"></param>
      /// <returns>Returns 0 for null.</returns>
      public virtual Int64 GetChars(int i, Int64 fieldoffset, char[] buffer, int bufferoffset, int length)
      {
         if (_dataReader.IsDBNull(i))
         {
            return 0;
         }
         else
         {
            return _dataReader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
         }
      }

      #endregion

      #region IdataRecord - Support Nullable

      /// <summary>
      /// Gets a boolean value from the datareader.
      /// </summary>
      /// <param name="name">Name of the column containing the value.</param>
      /// <returns>Returns false for null.</returns>
      public bool? GetNullableBoolean(string name)
      {
         return GetNullableBoolean(_dataReader.GetOrdinal(name));
      }

      /// <summary>
      /// Gets a boolean value from the datareader.
      /// </summary>
      /// <param name="i">Ordinal column position of the value.</param>
      /// <returns>Returns false for null.</returns>
      public bool? GetNullableBoolean(int i)
      {
         if (IsDBNull(i))
         {
            return null;
         }
         else
         {
            return _dataReader.GetBoolean(i);
         }
      }

      /// <summary>
      /// Gets a char value from the datareader.
      /// </summary>
      /// <param name="name">Name of the column containing the value.</param>
      /// <returns>Returns Char.MinValue for null.</returns>
      public char? GetNullableChar(string name)
      {
         return GetNullableChar(_dataReader.GetOrdinal(name));
      }

      /// <summary>
      /// Gets a byte value from the datareader.
      /// </summary>
      /// <param name="i">Ordinal column position of the value.</param>
      /// <returns>Char.MinValue for null.</returns>
      public char? GetNullableChar(int i)
      {
         if (IsDBNull(i))
         {
            return null;
         }
         else
         {
            char[] myChar = new char[1];
            _dataReader.GetChars(i, 0, myChar, 0, 1);

            return myChar[0];
         }
      }

      ///// <summary>
      ///// Gets a string value from the datareader.
      ///// </summary>
      ///// <param name="name">Name of the column containing the value.</param>
      ///// <returns>Returns empty string for null.</returns>
      //public string GetString(string name)
      //{
      //   return GetString(_dataReader.GetOrdinal(name));
      //}

      ///// <summary>
      ///// Gets a string value from the datareader.
      ///// </summary>
      ///// <param name="i">Ordinal column position of the value.</param>
      ///// <returns>Returns empty string for null.</returns>
      //public virtual string GetString(int i)
      //{
      //   if (IsDBNull(i))
      //   {
      //      return string.Empty;
      //   }
      //   else
      //   {
      //      return _dataReader.GetString(i);
      //   }
      //}

      /// <summary>
      /// Gets a byte value from the datareader.
      /// </summary>
      /// <param name="name">Name of the column containing the value.</param>
      /// <returns>Returns 0 for null.</returns>
      public byte? GetNullableByte(string name)
      {
         return GetNullableByte(_dataReader.GetOrdinal(name));
      }

      /// <summary>
      /// Gets a byte value from the datareader.
      /// </summary>
      /// <param name="i">Ordinal column position of the value.</param>
      /// <returns>Returns 0 for null.</returns>
      public byte? GetNullableByte(int i)
      {
         if (IsDBNull(i))
         {
            return null;
         }
         else
         {
            return _dataReader.GetByte(i);
         }
      }

      /// <summary>
      /// Gets an short value from the datareader.
      /// </summary>
      /// <param name="name">Name of the column containing the value.</param>
      /// <returns>Returns 0 for null.</returns>      
      public short? GetNullableInt16(string name)
      {
         return GetNullableInt16(_dataReader.GetOrdinal(name));
      }

      /// <summary>
      /// Gets a short value from the datareader.
      /// </summary>
      /// <param name="i">Ordinal column position of the value.</param>
      /// <returns>Returns 0 for null.</returns>      
      public short? GetNullableInt16(int i)
      {
         if (IsDBNull(i))
         {
            return null;
         }
         else
         {
            return _dataReader.GetInt16(i);
         }
      }

      /// <summary>
      /// Gets an integer value from the datareader.
      /// </summary>
      /// <param name="name">Name of the column containing the value.</param>
      /// <returns>Returns 0 for null.</returns>      
      public int? GetNullableInt32(string name)
      {
         return GetNullableInt32(_dataReader.GetOrdinal(name));
      }

      /// <summary>
      /// Gets an integer value from the datareader.
      /// </summary>
      /// <param name="i">Ordinal column position of the value.</param>
      /// <returns>Returns 0 for null.</returns>      
      public int? GetNullableInt32(int i)
      {
         if (IsDBNull(i))
         {
            return null;
         }
         else
         {
            return _dataReader.GetInt32(i);
         }
      }

      /// <summary>
      /// Gets a long value from the datareader.
      /// </summary>
      /// <param name="name">Name of the column containing the value.</param>
      /// <returns>Returns 0 for null.</returns>      
      public Int64? GetNullableInt64(string name)
      {
         return GetNullableInt64(_dataReader.GetOrdinal(name));
      }

      /// <summary>
      /// Gets a long value from the datareader.
      /// </summary>
      /// <param name="i">Ordinal column position of the value.</param>
      /// <returns>Returns 0 for null.</returns>      
      public Int64? GetNullableInt64(int i)
      {
         if (IsDBNull(i))
         {
            return null;
         }
         else
         {
            return _dataReader.GetInt64(i);
         }
      }

      /// <summary>
      /// Gets a float value from the datareader.
      /// </summary>
      /// <param name="name">Name of the column containing the value.</param>
      /// <returns>Returns 0 for null.</returns>      
      public float? GetNullableFloat(string name)
      {
         return GetNullableFloat(_dataReader.GetOrdinal(name));
      }

      /// <summary>
      /// Gets a float value from the datareader.
      /// </summary>
      /// <param name="i">Ordinal column position of the value.</param>
      /// <returns>Returns 0 for null.</returns>      
      public float? GetNullableFloat(int i)
      {
         if (IsDBNull(i))
         {
            return null;
         }
         else
         {
            return _dataReader.GetFloat(i);
         }
      }

      /// <summary>
      /// Gets a double value from the datareader.
      /// </summary>
      /// <param name="name">Name of the column containing the value.</param>
      /// <returns>Returns 0 for null.</returns>      
      public double? GetNullableDouble(string name)
      {
         return GetNullableDouble(_dataReader.GetOrdinal(name));
      }

      /// <summary>
      /// Gets a double value from the datareader.
      /// </summary>
      /// <param name="i">Ordinal column position of the value.</param>
      /// <returns>Returns 0 for null.</returns>      
      public double? GetNullableDouble(int i)
      {
         if (IsDBNull(i))
         {
            return null;
         }
         else
         {
            return _dataReader.GetDouble(i);
         }
      }

      /// <summary>
      /// Gets a decimal value from the datareader.
      /// </summary>
      /// <param name="name">Name of the column containing the value.</param>
      /// <returns>Returns 0 for null.</returns>      
      public decimal? GetNullableDecimal(string name)
      {
         return GetNullableDecimal(_dataReader.GetOrdinal(name));
      }

      /// <summary>
      /// Gets a decimal value from the datareader.
      /// </summary>
      /// <param name="i">Ordinal column position of the value.</param>
      /// <returns>Returns 0 for null.</returns>      
      public decimal? GetNullableDecimal(int i)
      {
         if (IsDBNull(i))
         {
            return null;
         }
         else
         {
            return _dataReader.GetDecimal(i);
         }
      }

      /// <summary>
      /// Gets a DateTime value from the datareader.
      /// </summary>
      /// <param name="name">Name of the column containing the value.</param>
      /// <returns>Returns DateTime.MinValue for null.</returns>      
      public DateTime? GetNullableDateTime(string name)
      {
         return GetNullableDateTime(_dataReader.GetOrdinal(name));
      }

      /// <summary>
      /// Gets a DateTime value from the datareader.
      /// </summary>
      /// <param name="i">Ordinal column position of the value.</param>
      /// <returns>Returns DateTime.MinValue for null.</returns>      
      public DateTime? GetNullableDateTime(int i)
      {
         if (IsDBNull(i))
         {
            return null;
         }
         else
         {
            return _dataReader.GetDateTime(i);
         }
      }

      /// <summary>
      /// Invokes the GetBytes method of the underlying datareader.
      /// </summary>
      /// <param name="name">Name of the column containing the value.</param>
      /// <param name="fieldOffset"></param>
      /// <param name="buffer"></param>
      /// <param name="bufferoffset"></param>
      /// <param name="length"></param>
      /// <returns>Returns 0 for null.</returns>

      public Int64? GetNullableBytes(string name, Int64 fieldOffset, byte[] buffer, int bufferoffset, int length)
      {
         return GetNullableBytes(_dataReader.GetOrdinal(name), fieldOffset, buffer, bufferoffset, length);
      }

      /// <summary>
      /// Invoke the GetBytes method of the realdatareader.
      /// </summary>
      /// <param name="i">Ordinal column position of the value.</param>
      /// <param name="fieldOffset"></param>
      /// <param name="buffer"></param>
      /// <param name="bufferoffset"></param>
      /// <param name="length"></param>
      /// <returns>Returns 0 for null.</returns>
      public Int64? GetNullableBytes(int i, Int64 fieldOffset, byte[] buffer, int bufferoffset, int length)
      {
         if (_dataReader.IsDBNull(i))
            return null;
         else
            return _dataReader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
      }

      /// <summary>
      /// Invokes the GetChars method of the underlying datareader.
      /// </summary>
      /// <param name="name">Name of the column containing the value.</param>
      /// <param name="fieldoffset"></param>
      /// <param name="buffer"></param>
      /// <param name="bufferoffset"></param>
      /// <param name="length"></param>
      /// <returns>Returns 0 for null.</returns>
      public Int64? GetNullableChars(string name, Int64 fieldoffset, char[] buffer, int bufferoffset, int length)
      {
         return GetNullableChars(_dataReader.GetOrdinal(name), fieldoffset, buffer, bufferoffset, length);
      }

      /// <summary>
      /// Invokes the GetChars method of the underlying datareader.
      /// </summary>
      /// <param name="i">Ordinal column position of the value.</param>
      /// <param name="fieldoffset"></param>
      /// <param name="buffer"></param>
      /// <param name="bufferoffset"></param>
      /// <param name="length"></param>
      /// <returns>Returns 0 for null.</returns>
      public Int64? GetNullableChars(int i, Int64 fieldoffset, char[] buffer, int bufferoffset, int length)
      {
         if (_dataReader.IsDBNull(i))
         {
            return null;
         }
         else
         {
            return _dataReader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
         }
      }

      #endregion

      #region IdataRecord - Support Type

      public object GetByType(Type type, string propertyName)
      {
         if (type == typeof(Guid))
         {
            return this.GetGuid(propertyName);
         }
         else if (type == typeof(Guid?))
         {
            return this.GetNullableGuid(propertyName);
         }

         return null;
      }

      #endregion

      #region IDisposable

      private bool _isDisposed;

      /// <summary>
      /// Dispose this object
      /// </summary>
      /// <param name="disposing">Disposing object</param>
      protected virtual void Dispose(bool disposing)
      {
         if (!_isDisposed)
         {
            if (disposing)
            {
               _dataReader.Dispose();
            }

            _isDisposed = true;
         }
      }

      /// <summary>
      /// Dispose from garbage collection
      /// </summary>
      public void Dispose()
      {
         Dispose(true);
         GC.SuppressFinalize(this);
      }

      #endregion

      #region IDataReader

      /// <summary>
      /// Reads the next row of data from the datareader.
      /// </summary>
      public bool Read()
      {
         return _dataReader.Read();
      }

      /// <summary>
      /// Moves to the next result set in the datareader.
      /// </summary>
      public bool NextResult()
      {
         return _dataReader.NextResult();
      }

      /// <summary>
      /// Closes the datareader.
      /// </summary>
      public void Close()
      {
         _dataReader.Close();
      }

      /// <summary>
      /// Returns the IsClosed property value from the datareader.
      /// </summary>
      public bool IsClosed
      {
         get { return _dataReader.IsClosed; }
      }

      /// <summary>
      /// Returns the RecordsAffected property value from the underlying datareader.
      /// </summary>
      public int RecordsAffected
      {
         get { return _dataReader.RecordsAffected; }
      }

      /// <summary>
      /// Gets the depth property value from the datareader.
      /// </summary>
      public int Depth
      {
         get { return _dataReader.Depth; }
      }

      /// <summary>
      /// Invokes the GetSchemaTable method of the underlying datareader.
      /// </summary>
      /// <returns>Schema table</returns>
      public DataTable GetSchemaTable()
      {
         return _dataReader.GetSchemaTable();
      }

      #endregion

      #region Helper Methods

      /// <summary>
      /// Gets the FieldCount property from the datareader.
      /// </summary>
      public int FieldCount
      {
         get { return _dataReader.FieldCount; }
      }

      #endregion
   }
}