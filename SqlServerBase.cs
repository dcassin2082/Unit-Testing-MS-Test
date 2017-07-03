using Avengers.RepositoryModels;
using PSPRS.Avengers.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using PSPRS.Avengers.enums;

namespace PSPRS.Avengers.Repository
{
    public abstract class SqlServerBase : IDisposable
    {
        private const int TIMEOUT = 120;    // This sucks...

        protected string connectionString;
        protected SqlConnection connection;
        protected SqlCommand command;
        protected SqlTransaction transaction;

        public SqlServerBase(string connectionString)
        {
            this.connectionString = connectionString;
        }

        protected List<T> ExecuteReader<T>(string storedProc) where T : new()
        {
            List<T> list;

            using (this.command = CreateCommand(storedProc))
            {
                OpenConnection();

                using (SqlDataReader r = this.command.ExecuteReader())
                {
                    list = WalkObjectGraph<T>(r);
                }

                this.command.Connection.Close();
            }

            return list;
        }


        protected List<T> ExecuteReader<T, P>(string storedProc,
            P parameterModel) where T : new()
        {
            List<T> list = new List<T>();

            using (SqlCommand command = CreateCommand(storedProc))
            {
                command.CommandTimeout = TIMEOUT;
                AddSqlParameters<P>(command, parameterModel);

                OpenConnection();

                using (SqlDataReader r = command.ExecuteReader())
                {
                    list = WalkObjectGraph<T>(r);
                }

                command.Connection.Close();
            }

            return list;
        }

        private void AddSqlParameters<T>(SqlCommand command, T model)
        {
            PropertyInfo[] properties = model.GetType().GetProperties();

            foreach (PropertyInfo property in properties)
            {
                Type pType = property.PropertyType;

                if (!pType.IsArray && !pType.IsPrimitive && !pType.IsValueType && !(pType == typeof(string)))
                {
                    AddSqlParameters(command, property.GetValue(model));

                    continue;
                }


                SqlParameter sqlParam = new SqlParameter();

                sqlParam.ParameterName = property.Name;

                if(property.GetType().IsEnum)
                {
                    sqlParam.SqlValue = (int)property.GetValue(model);
                }
                else
                {
                    sqlParam.SqlValue = property.GetValue(model);
                }
                if (null == sqlParam.SqlValue)
                {
                    sqlParam.SqlValue = DBNull.Value;
                }

                sqlParam.SqlDbType = MapSqlDataType(property.PropertyType);

                command.Parameters.Add(sqlParam);
            }
        }

        private SqlDbType MapSqlDataType(Type propertyType)
        {
            Type checkedType = propertyType;

            // if this is a Nullable<T> then use <T>
            if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                checkedType = propertyType.GenericTypeArguments[0];
            }

            if (checkedType.Equals(typeof(string)))
            {
                return SqlDbType.VarChar;
            }
            else if (checkedType.Equals(typeof(int)))
            {
                return SqlDbType.Int;
            }
            else if (checkedType.Equals(typeof(Int64)))
            {
                return SqlDbType.Int;
            }
            else if (checkedType.Equals(typeof(DateTime)))
            {
                return SqlDbType.DateTime;
            }
            else if (checkedType.Equals(typeof(byte[])))
            {
                return SqlDbType.VarBinary;
            }
            else if (checkedType.Equals(typeof(FileInfoType)))
            {
                return SqlDbType.Int;
            }
            else if (checkedType.Equals(typeof(bool)))
            {
                return SqlDbType.Bit;
            }
            else if (propertyType.Equals(typeof(decimal)))
            {
                return SqlDbType.Decimal;
            }
            throw new MissingSqlParameterDataTypeException(propertyType.Name);
        }

        private List<T> WalkObjectGraph<T>(IDataReader reader) where T : new()
        {
            List<T> list = new List<T>();

            IEnumerable<string> sqlColumns = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();

            while (reader.Read())
            {
                T newModel = new T();

                PropertyInfo[] properties = typeof(T).GetProperties();

                foreach (PropertyInfo p in properties)
                {
                    GetProperties(p, reader, newModel, sqlColumns);
                }

                list.Add(newModel);
            }

            return list;
        }

        private void GetProperties<T>(PropertyInfo property,
            IDataReader reader,
            T newModel,
            IEnumerable<string> sqlColumns)
        {
            Type pType = property.PropertyType;

            if (pType.IsPrimitive || pType.IsValueType || pType == typeof(string))
            {
                bool excludeFromDatabase = !(property.GetCustomAttribute<TableColumnAttribute>()?.ExcludeFromDatabase ?? false);

                if (excludeFromDatabase)
                {
                    SetColumnData(property, reader, newModel, sqlColumns);
                }
            }
            else
            {
                var domainChildProperty = property.GetValue(newModel);

                if (domainChildProperty == null)
                {
                    domainChildProperty = Activator.CreateInstance(pType);

                    property.SetValue(newModel, domainChildProperty);
                }

                PropertyInfo[] properties = domainChildProperty.GetType().GetProperties();

                foreach (PropertyInfo p in properties)
                {
                    GetProperties(p, reader, domainChildProperty, sqlColumns);
                }
            }
        }

        private void SetColumnData<T>(PropertyInfo property,
            IDataReader reader, T newModel,
            IEnumerable<string> sqlColumns)
        {
            string propColumns = string.Empty;

            propColumns = property.GetCustomAttribute<TableColumnAttribute>(true)?.ColumnName ?? property.Name;

            SetColumnData(reader, sqlColumns, newModel, property, propColumns);
        }

        private void SetColumnData<T>(IDataReader reader,
            IEnumerable<string> sqlColumns,
            T newModel,
            PropertyInfo property,
            string propColumns)
        {
            bool found = false;
            string propColumnTrimmed = string.Empty;

            foreach (string sqlColumnName in propColumns.Split(','))
            {
                propColumnTrimmed = sqlColumnName.Trim();

                if (sqlColumns.Contains(propColumnTrimmed, StringComparer.CurrentCultureIgnoreCase))
                {
                    if (reader[propColumnTrimmed] == DBNull.Value)
                    {
                        property.SetValue(newModel, null);
                    }
                    else
                    {
                        property.SetValue(newModel, reader[propColumnTrimmed]);
                    }

                    found = true;

                    break;
                }
            }

            if (!found)
            {
                throw new MissingSqlColumnException(propColumnTrimmed); 
            }
        }

        protected void Commit()
        {
            if (this.transaction != null)
            {
                this.transaction.Commit();
                this.connection.Close();
            }
        }

        protected int ExecuteNonQueryWithTransaction<P>(string storedProc, P parameterModel)
        {
            int result = -1;

            using (SqlCommand command = CreateCommand(storedProc))
            {
                AddSqlParameters(command, parameterModel);

                OpenConnection();

                StartTransaction(command);

                result = command.ExecuteNonQuery();
            }

            return result;
        }

        private void StartTransaction(SqlCommand command)
        {
            if (this.transaction == null)
            {
                this.transaction = command.Connection.BeginTransaction();
            }

            command.Transaction = this.transaction;
        }

        protected int ExecuteNonQuery<P>(string storedProc, P parameterModel)
        {
            int result = -1;

            using (SqlCommand command = CreateCommand(storedProc))
            {
                AddSqlParameters(command, parameterModel);

                OpenConnection();

                result = command.ExecuteNonQuery();

                command.Connection.Close();
            }

            return result;
        }

        protected SqlCommand CreateCommand(string storedProc)
        {
            if (this.connection == null)
            {
                this.connection = new SqlConnection(this.connectionString);
            }

            return new SqlCommand()
            {
                CommandText = storedProc,
                CommandType = CommandType.StoredProcedure,
                Connection = this.connection
            };
        }

        protected void OpenConnection()
        {
            if (this.connection.State == ConnectionState.Closed)
            {
                this.connection.Open();
            }
        }

        public void Dispose()
        {
            if (this.connection != null)
            {
                if (this.connection.State != ConnectionState.Closed)
                {
                    this.connection.Close();
                }
            }
        }
    }
}
