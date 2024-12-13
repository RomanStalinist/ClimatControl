using DemoExam.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;

namespace DemoExam.Data
{
    public class Db : IDb, IDisposable
    {
        private SqlConnection Connection { get; set; }
        public static TraceLevel TraceLevel { get; private set; }
        public string ConnectionString => "Server=localhost;Database=ClimateControl;User Id=roman;Password=fnaf;TrustServerCertificate=True";

        public readonly string[] MyRequestsWhereClause = { "CustomerId = @CustomerId" };
        public readonly object[] MyRequestsWhereArgs = { App.User?.Id };
        public readonly string[] CommentsSentToMeWhereClause = { "RequestId = @RequestId" };

        public Db(TraceLevel traceLevel = TraceLevel.Info)
        {
            TraceLevel = traceLevel;
            Connection = new SqlConnection(ConnectionString);
        }

        public IEnumerable<T> Select<T>(string[] columns, string[] whereClause, object[] whereArgs) where T : IEntity
        {
            Connection.Open();
            Debug.WriteLineIf(TraceLevel is TraceLevel.Info, "Connection opened");

            var cmd = Connection.CreateCommand();
            string tableName = TableInfo.FromEntityType<T>();
            var columnsString = columns is null || columns.Length == 0
                ? "*"
                : string.Join(", ", columns);

            cmd.CommandText = $"SELECT {columnsString} FROM {tableName}";

            if (whereClause != null && whereClause.Length > 0 && whereArgs.Length == whereClause.Length)
            {
                cmd.CommandText += " WHERE " + string.Join(" AND ", whereClause);
        
                for (int i = 0; i < whereClause.Length; i++)
                {
                    var parameterName = whereClause[i].Split('=')[1].Trim();
                    cmd.Parameters.AddWithValue(parameterName, whereArgs[i]);
                }
            }

            Debug.WriteLineIf(TraceLevel is TraceLevel.Info, cmd.CommandText);

            var reader = cmd.ExecuteReader();
            var results = new List<T>();

            while (reader.Read())
            {
                var item = Activator.CreateInstance<T>();
                foreach (var property in item.GetType().GetProperties())
                {
                    try
                    {
                        var value = reader[property.Name];
                        property.SetValue(item, value is DBNull ? null : value);
                    }
                    catch (IndexOutOfRangeException ioore)
                    {
                        Debug.WriteLineIf(TraceLevel is TraceLevel.Info, $"Skipping column {ioore.Message}");
                    }
                }
                results.Add(item);
            }

            Connection.Close();
            Debug.WriteLineIf(TraceLevel is TraceLevel.Info, "Connection closed");
            return results;
        }

        public IEnumerable<T> RawSelect<T>(string sql) where T : IEntity
        {
            Connection.Open();
            Debug.WriteLineIf(TraceLevel is TraceLevel.Info, "Connection opened");

            var cmd = Connection.CreateCommand();
            cmd.CommandText = sql;
            Debug.WriteLineIf(TraceLevel is TraceLevel.Info, cmd.CommandText);

            var reader = cmd.ExecuteReader();
            var results = new List<T>();

            while (reader.Read())
            {
                var item = Activator.CreateInstance<T>();
                foreach (var property in item.GetType().GetProperties())
                {
                    try
                    {
                        var value = reader[property.Name];
                        property.SetValue(item, value is DBNull ? null : value);
                    }
                    catch (IndexOutOfRangeException ioore)
                    {
                        Debug.WriteLineIf(TraceLevel is TraceLevel.Info, $"Skipping column {ioore.Message}");
                    }
                }
                results.Add(item);
            }

            Connection.Close();
            Debug.WriteLineIf(TraceLevel is TraceLevel.Info, "Connection closed");
            return results;
        }

        public int Insert<T>(T item) where T : IEntity
        {
            Connection.Open();
            Debug.WriteLineIf(TraceLevel is TraceLevel.Info, "Connection opened");

            var cmd = Connection.CreateCommand();
            string tableName = TableInfo.FromEntityType<T>();
            var columnNames = new List<string>();
            var valuePlaceholders = new List<string>();

            foreach (var property in item.GetType().GetProperties())
            {
                var propertyValue = property.GetValue(item);
                var propertyName = property.Name;

                if (propertyName is "Id") continue;

                columnNames.Add(propertyName);
                valuePlaceholders.Add($"@{propertyName}");
                cmd.Parameters.AddWithValue($"@{propertyName}", propertyValue);
            }

            cmd.CommandText = $"INSERT INTO {tableName} ({string.Join(", ", columnNames)}) VALUES ({string.Join(", ", valuePlaceholders)})";

            Debug.WriteLineIf(TraceLevel is TraceLevel.Info, cmd.CommandText);

            var result = cmd.ExecuteNonQuery();

            Connection.Close();
            Debug.WriteLineIf(TraceLevel is TraceLevel.Info, "Connection closed");
            return result;
        }

        public int Delete<T>(T item) where T : IEntity
        {
            Connection.Open();
            Debug.WriteLineIf(TraceLevel is TraceLevel.Info, "Connection opened");

            int result;
            var cmd = Connection.CreateCommand();
            string tableName = TableInfo.FromEntityType<T>();
            var whereClause = new List<string>();
            var idProperty = item.GetType().GetProperty("Id");

            if (idProperty != null)
            {
                cmd.Parameters.AddWithValue("@Id", idProperty.GetValue(item));
                cmd.CommandText = $"DELETE FROM {tableName} WHERE Id = @Id";

                Debug.WriteLineIf(TraceLevel is TraceLevel.Info, cmd.CommandText);

                result = cmd.ExecuteNonQuery();
                Connection.Close();
                return result;
            }

            foreach (var property in item.GetType().GetProperties())
            {
                var propertyValue = property.GetValue(item);
                var propertyName = property.Name;

                if (propertyValue != null && propertyName != "Id")
                {
                    whereClause.Add($"{propertyName} = @{propertyName}");
                    cmd.Parameters.AddWithValue($"@{propertyName}", propertyValue);
                }
            }

            if (whereClause.Count == 0)
                throw new InvalidOperationException("Empty properties in whereClause");

            cmd.CommandText = $"DELETE FROM {tableName} WHERE {string.Join(" AND ", whereClause)}";

            Debug.WriteLineIf(TraceLevel is TraceLevel.Info, cmd.CommandText);

            result = cmd.ExecuteNonQuery();
            Connection.Close();
            Debug.WriteLineIf(TraceLevel is TraceLevel.Info, "Connection closed");
            return result;
        }

        public int Update<T>(T item) where T : IEntity
        {
            Connection.Open();
            Debug.WriteLineIf(TraceLevel is TraceLevel.Info, "Connection opened");

            var cmd = Connection.CreateCommand();
            string tableName = TableInfo.FromEntityType<T>();
            var setClause = new List<string>();
            var idProperty = item.GetType().GetProperty("Id")
                ?? throw new InvalidOperationException("Item must have a property named 'Id'.");
            var idValue = idProperty.GetValue(item)
                ?? throw new InvalidOperationException("Id property cannot be null.");

            foreach (var property in item.GetType().GetProperties())
            {
                if (property.Name == "Id") continue; // Не обновляем Id
                
                var propertyValue = property.GetValue(item);
                setClause.Add($"{property.Name} = @{property.Name}");
                cmd.Parameters.AddWithValue($"@{property.Name}", propertyValue ?? DBNull.Value);
            }

            cmd.CommandText = $"UPDATE {tableName} SET {string.Join(", ", setClause)} WHERE Id = @Id";
            cmd.Parameters.AddWithValue("@Id", idValue);

            Debug.WriteLineIf(TraceLevel is TraceLevel.Info, cmd.CommandText);

            var result = cmd.ExecuteNonQuery();
            Connection.Close();
            Debug.WriteLineIf(TraceLevel is TraceLevel.Info, "Connection closed");
            return result;
        }

        public void Dispose()
        {
            Connection.Dispose();
            Debug.WriteLineIf(TraceLevel is TraceLevel.Info, "Connection disposed");
        }
    }
}
