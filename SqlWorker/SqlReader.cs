using System;
using System.ComponentModel;
using System.Data;

namespace GitWCore.SqlWorker
{
    /// <summary>
    /// Implémentation customisée d'un IDataReader
    /// </summary>
    public class SqlReader : IDataReader
    {
        private readonly IDataReader _reader;

        /// <inheritdoc/>
        public SqlReader(IDataReader reader)
        {
            _reader = reader;
        }

        /// <inheritdoc/>
        public object this[int i] => _reader[i];
        /// <inheritdoc/>
        public object this[string name] => _reader[name];
        /// <inheritdoc/>
        public int Depth => _reader.Depth;
        /// <inheritdoc/>
        public bool IsClosed => _reader.IsClosed;
        /// <inheritdoc/>
        public int RecordsAffected => _reader.RecordsAffected;
        /// <inheritdoc/>
        public int FieldCount => _reader.FieldCount;

        /// <inheritdoc/>
        public void Close() => _reader.Close();
        /// <inheritdoc/>
        public void Dispose() => _reader.Close();
        /// <inheritdoc/>
        public bool GetBoolean(int i) => _reader.GetBoolean(i);
        /// <inheritdoc/>
        public byte GetByte(int i) => _reader.GetByte(i);
        /// <inheritdoc/>
        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length) => _reader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        /// <inheritdoc/>
        public char GetChar(int i) => _reader.GetChar(i);
        /// <inheritdoc/>
        public long GetChars(int i, long fieldOffset, char[] buffer, int bufferoffset, int length) => _reader.GetChars(i, fieldOffset, buffer, bufferoffset, length);
        /// <inheritdoc/>
        public IDataReader GetData(int i) => new SqlReader(_reader.GetData(i));
        /// <inheritdoc/>
        public string GetDataTypeName(int i) => _reader.GetDataTypeName(i);
        /// <inheritdoc/>
        public DateTime GetDateTime(int i) => _reader.GetDateTime(i);
        /// <inheritdoc/>
        public decimal GetDecimal(int i) => _reader.GetDecimal(i);
        /// <inheritdoc/>
        public double GetDouble(int i) => _reader.GetDouble(i);
        /// <inheritdoc/>
        public Type GetFieldType(int i) => _reader.GetFieldType(i);
        /// <inheritdoc/>
        public float GetFloat(int i) => _reader.GetFloat(i);
        /// <inheritdoc/>
        public Guid GetGuid(int i) => _reader.GetGuid(i);
        /// <inheritdoc/>
        public short GetInt16(int i) => _reader.GetInt16(i);
        /// <inheritdoc/>
        public int GetInt32(int i) => _reader.GetInt32(i);
        /// <inheritdoc/>
        public long GetInt64(int i) => _reader.GetInt64(i);
        /// <inheritdoc/>
        public string GetName(int i) => _reader.GetName(i);
        /// <inheritdoc/>
        public int GetOrdinal(string name) => _reader.GetOrdinal(name);
        /// <inheritdoc/>
        public DataTable GetSchemaTable() => _reader.GetSchemaTable();
        /// <inheritdoc/>
        public string GetString(int i) => _reader.GetString(i);
        /// <inheritdoc/>
        public object GetValue(int i) => _reader.GetValue(i);
        /// <inheritdoc/>
        public int GetValues(object[] values) => _reader.GetValues(values);
        /// <inheritdoc/>
        public bool IsDBNull(int i) => _reader.IsDBNull(i);
        /// <inheritdoc/>
        public bool NextResult() => _reader.NextResult();
        /// <inheritdoc/>
        public bool Read() => _reader.Read();

        /// <summary>
        /// Récupère une valeur de type T dans un SqlataReader, reçu après exécution d'une requête 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T Get<T>(string fieldName, T defaultValue)
        {
            try
            {
                Type conversionType = typeof(T);
                if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                {
                    NullableConverter nullableConverter = new NullableConverter(conversionType);
                    conversionType = nullableConverter.UnderlyingType;
                }

                return ((this[fieldName] is DBNull) ? defaultValue : (T)Convert.ChangeType(this[fieldName], conversionType));
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Récupère une valeur de type T dans un SqlataReader, reçu après exécution d'une requête 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public T Get<T>(string fieldName)
        {
            return Get(fieldName, default(T));
        }
    }
}
