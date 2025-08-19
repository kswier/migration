using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data;

namespace Library
{
    public class ADODB
    {
        private bool isConnected;
        private SqlConnection sqlConn = null;
        internal SqlTransaction sqlTrans = null;
        public bool usePooling = false;
        public String serverName;
        public String userName;
        public String password;
        public String databaseName;
        public String applicationName;
        public bool integratedSecurity = false;
        public bool inTransaction = false;

        public ADODB()
        {
            serverName = "";
            userName = "";
            password = "";
            databaseName = "";
            isConnected = false;
            sqlConn = null;
        }

        ~ADODB()
        {
            // don't set connected to false in finalizer - it will auto close when set connection to null
            //connected = false;
            sqlConn = null;
        }

        public SqlConnection connection
        {
            get { return sqlConn; }
        }

        public bool connected
        {
            get
            {
                return isConnected;
            }

            set
            {
                if (value != isConnected)
                {
                    if (value)
                    {
                        string constr = connectionString();
                        sqlConn = new SqlConnection(constr);
                        sqlConn.Open();

                        execSQL("set ansi_warnings off");
                        execSQL("set ansi_nulls off");
                    }
                    else
                    {
                        if (sqlConn != null)
                        {
                            // don't close - just dispose - it returns it to the pool
                            if (!usePooling)
                            {
                                try
                                {
                                    if (sqlConn.State == ConnectionState.Open)
                                        sqlConn.Close();
                                    sqlConn.Dispose();
                                }
                                catch
                                {
                                    // ignore
                                }
                            }
                            sqlConn = null;
                        }
                    }
                    isConnected = value;
                }
            }
        }

        public string connectionString()
        {
            return connectionString(serverName, databaseName, userName, password, usePooling, integratedSecurity);
        }

        public static string connectionString(string serverName, string dbName, string userName, string password, bool usePooling, bool integratedSecurity)
        {
            string constr = "data source=" + serverName + ";initial catalog=" + dbName + ";User ID=" + userName + ";Password=" + password;
            if (!usePooling)
                constr += ";Pooling=false";
            if (integratedSecurity)
                constr += ";Integrated Security=SSPI";
            return constr;
        }

        public static SqlConnection getConnection(string serverName, string dbName, string userName, string password, bool usePooling, bool integratedSecurity)
        {
            SqlConnection db = new SqlConnection();
            String conStr = connectionString(serverName, dbName, userName, password, usePooling, integratedSecurity);
            db.ConnectionString = conStr;
            return db;
        }

        public void execSQL(string query)
        {
            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Transaction = sqlTrans;
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        public void startTransaction()
        {
            sqlTrans = sqlConn.BeginTransaction();
            inTransaction = true;
        }

        public void commit()
        {
            sqlTrans.Commit();
            inTransaction = false;
        }

        public void rollback()
        {
            sqlTrans.Rollback();
            inTransaction = false;
        }

        public void close()
        {
            sqlConn.Close();
        }
    }

    public class ADOQuery
    {
        private ADODB aDB;
        private SqlCommand cmd = null;
        private SqlDataReader reader = null;
        private bool isEof;
        private Dictionary<string, int> fieldCache = null;
        public string sql;
        public bool hasRows;
        

        public ADOQuery(ADODB db)
        {
            aDB = db;
            sql = "";
            isEof = false;
            fieldCache = new Dictionary<string, int>();
        }

        ~ADOQuery()
        {
            close();
        }

        public int findField(string colName)
        {
            bool contains = fieldCache.ContainsKey(colName);
            if (contains)
            {
                return fieldCache[colName];
            }
            else
            {
                int fld = -1;
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (reader.GetName(i).Equals(colName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        fieldCache[colName] = i;
                        fld = i;
                        break;
                    }
                }

               return fld;

                /*
                return false;
                try
                {
                    int fld = reader.GetOrdinal(colName);
                    fieldCache[colName] = fld;
                    return fld;
                }
                catch
                {
                    return -1;
                }
                */
            }
        }

        private SqlCommand command(string query)
        {
            SqlCommand cmd = new SqlCommand(query, aDB.connection);
            cmd.Transaction = aDB.sqlTrans;
            return cmd;
        }

        public bool eof
        {
            get { return isEof; }
        }

        public void next()
        {
            isEof = !reader.Read();
        }

        public void open()
        {
            fieldCache.Clear();
            cmd = command(sql);
            reader = cmd.ExecuteReader();
            hasRows = reader.HasRows;
            isEof = !reader.Read();
        }

        public void open(string query)
        {
            fieldCache.Clear();
            cmd = command(query);
            reader = cmd.ExecuteReader();
            hasRows = reader.HasRows;
            isEof = !reader.Read();
        }

        public void close()
        {
            if (reader != null)
            {
                reader.Close();
                reader.Dispose();
                reader = null;
            }
            if (cmd != null)
            {
                cmd.Dispose();
                cmd = null;
            }
        }

        public void execSQL()
        {
            cmd = command(sql);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            cmd = null;
        }

        public void execSQL(string query)
        {
            cmd = command(query);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            cmd = null;
        }

        public int fieldCount
        {
            get
            {
                if (reader != null) 
                    return reader.FieldCount; 
                else 
                    return 0;
            }
        }

        public string[] fieldNames
        {
            get
            {
                int cnt = fieldCount;
                string[] names = new String[cnt];
                if (reader != null)
                {
                    for (int i = 0; i < fieldCount; i++)
                    {
                        names[i] = reader.GetName(i);
                    }
                }
                return names;
            }
        }

        public string fieldTypeString(string fieldName)
        {
            string rslt = "";
            if (reader != null)
            {
                int fld = reader.GetOrdinal(fieldName);
                rslt = reader.GetFieldType(fld).FullName;
            }
            return rslt;
        }

        public Type fieldType(string fieldName)
        {
            if (reader != null)
            {
                int fld = reader.GetOrdinal(fieldName);
                return reader.GetFieldType(fld);
            }
            return null;
        }

        public object fieldValue(string fieldName)
        {
            object retval = null;
            if (reader != null)
            {
                int fld = reader.GetOrdinal(fieldName);
                retval = reader.GetValue(fld);
            }
            return retval;
        }

        public bool fieldIsNull(string fieldName)
        {
            bool retval = true;
            if (reader != null)
            {
                int fld = findField(fieldName);
                if (fld >= 0) retval = reader.IsDBNull(fld);
            }
            return retval;
        }

        public string fieldAsString(string fieldName)
        {
            string rslt = "";
            if (reader != null)
            {
                int fld = reader.GetOrdinal(fieldName);
                if (reader.IsDBNull(fld)) return rslt;
                Type typ = reader.GetFieldType(fld);
                if (typ == typeof(System.String)) rslt = reader.GetString(fld);
                if (typ == typeof(System.Int16)) rslt = reader.GetInt16(fld).ToString();
                if (typ == typeof(System.Int32)) rslt = reader.GetInt32(fld).ToString();
                if (typ == typeof(System.Int64)) rslt = reader.GetInt64(fld).ToString();
                if (typ == typeof(System.Boolean)) rslt = reader.GetBoolean(fld).ToString();
                if (typ == typeof(System.DateTime)) rslt = reader.GetDateTime(fld).ToString();
                if (typ == typeof(System.Decimal)) rslt = reader.GetDecimal(fld).ToString();
                if (typ == typeof(System.Single)) rslt = reader.GetFloat(fld).ToString();
                if (typ == typeof(System.Double)) rslt = reader.GetDouble(fld).ToString();
                if (typ == typeof(System.Object)) rslt = reader.GetValue(fld).ToString();
                if (typ == typeof(System.Byte)) rslt = Convert.ToChar(reader.GetByte(fld)).ToString();
                if (typ == typeof(System.Guid)) rslt = reader.GetGuid(fld).ToString();
                //if (typ == typeof(System.Byte[])) rslt = reader.GetBytes(
                //string ft = reader.GetFieldType(fld).FullName;
                //if (ft == "System.String") rslt = reader.GetString(fld);
            }
            return rslt;
        }

        public int fieldAsInteger(string fieldName)
        {
            int rslt = 0;
            if (reader != null)
            {
                int fld = reader.GetOrdinal(fieldName);
                if (reader.IsDBNull(fld)) return rslt;
                Type typ = reader.GetFieldType(fld);
                if (typ == typeof(System.Int16)) rslt = (int)reader.GetInt16(fld);
                if (typ == typeof(System.Int32)) rslt = (int)reader.GetInt32(fld);
                if (typ == typeof(System.Int64)) rslt = (int)reader.GetInt64(fld);
                if (typ == typeof(System.Single)) rslt = (int)reader.GetFloat(fld);
                if (typ == typeof(System.Double)) rslt = (int)reader.GetDouble(fld);

                if (typ == typeof(System.Boolean)) rslt = Convert.ToInt32(reader.GetBoolean(fld));
                if (typ == typeof(System.DateTime)) rslt = (int)reader.GetDateTime(fld).ToOADate();
                if (typ == typeof(System.Decimal)) rslt = (int)reader.GetDecimal(fld);
                if (typ == typeof(System.Object)) rslt = 0;
                if (typ == typeof(System.Byte)) rslt = (int)reader.GetByte(fld);
                if (typ == typeof(System.Guid)) rslt = 0;
                if (typ == typeof(System.String)) rslt = reader.GetString(fld).strToIntDef(0);

            }
            return rslt;
        }

        public bool fieldAsBool(string fieldName)
        {
            bool rslt = false;
            if (reader != null)
            {
                int fld = reader.GetOrdinal(fieldName);
                if (reader.IsDBNull(fld)) return rslt;
                Type typ = reader.GetFieldType(fld);
                if (typ == typeof(System.Int16)) rslt = (reader.GetInt16(fld) == 1);
                if (typ == typeof(System.Int32)) rslt = (reader.GetInt32(fld) == 1);
                if (typ == typeof(System.Int64)) rslt = (reader.GetInt64(fld) == 1);
                if (typ == typeof(System.Single)) rslt = (reader.GetFloat(fld) == 1);
                if (typ == typeof(System.Double)) rslt = (reader.GetDouble(fld) == 1);

                if (typ == typeof(System.Boolean)) rslt = reader.GetBoolean(fld);
                if (typ == typeof(System.DateTime)) rslt = ((int)reader.GetDateTime(fld).ToOADate() == 1);
                if (typ == typeof(System.Decimal)) rslt = ((int)reader.GetDecimal(fld) == 1);
                if (typ == typeof(System.Object)) rslt = false;
                if (typ == typeof(System.Byte)) rslt = ((int)reader.GetByte(fld) == 1);
                if (typ == typeof(System.Guid)) rslt = false;
                if (typ == typeof(System.String))
                {
                    rslt = Convert.ToBoolean(reader.GetString(fld));
                    /*
                    rslt = ((reader.GetString(fld).strToIntDef(0) == 1) ||
                           (reader.GetString(fld).ToUpper() == "TRUE"));
                    */
                }

            }
            return rslt;
        }

        public Guid fieldAsGuid(string fieldName)
        {
            Guid rslt = Guid.Empty;
            if (reader != null)
            {
                int fld = reader.GetOrdinal(fieldName);
                if (reader.IsDBNull(fld))
                    return rslt;
                else
                    rslt = reader.GetGuid(fld);
            }

            return rslt;

        }

        public Nullable<int> fieldAsNullableInteger(string fieldName)
        {
            Nullable<int> rslt = 0;
            if (reader != null)
            {
                int fld = reader.GetOrdinal(fieldName);
                if (reader.IsDBNull(fld)) return null;
                Type typ = reader.GetFieldType(fld);
                if (typ == typeof(System.Int16)) rslt = (int)reader.GetInt16(fld);
                if (typ == typeof(System.Int32)) rslt = (int)reader.GetInt32(fld);
                if (typ == typeof(System.Int64)) rslt = (int)reader.GetInt64(fld);
                if (typ == typeof(System.Single)) rslt = (int)reader.GetFloat(fld);
                if (typ == typeof(System.Double)) rslt = (int)reader.GetDouble(fld);

                if (typ == typeof(System.Boolean)) rslt = Convert.ToInt32(reader.GetBoolean(fld));
                if (typ == typeof(System.DateTime)) rslt = (int)reader.GetDateTime(fld).ToOADate();
                if (typ == typeof(System.Decimal)) rslt = (int)reader.GetDecimal(fld);
                if (typ == typeof(System.Object)) rslt = null;
                if (typ == typeof(System.Byte)) rslt = (int)reader.GetByte(fld);
                if (typ == typeof(System.Guid)) rslt = null;
                if (typ == typeof(System.String)) rslt = reader.GetString(fld).strToIntDef(0);

            }
            return rslt;
        }

        public double fieldAsDouble(string fieldName)
        {
            double rslt = 0;
            if (reader != null)
            {
                int fld = reader.GetOrdinal(fieldName);
                if (reader.IsDBNull(fld)) return rslt;
                Type typ = reader.GetFieldType(fld);
                if (typ == typeof(System.Int16)) rslt = (double)reader.GetInt16(fld);
                if (typ == typeof(System.Int32)) rslt = (double)reader.GetInt32(fld);
                if (typ == typeof(System.Int64)) rslt = (double)reader.GetInt64(fld);
                if (typ == typeof(System.Single)) rslt = (double)reader.GetFloat(fld);
                if (typ == typeof(System.Double)) rslt = reader.GetDouble(fld);

                if (typ == typeof(System.Boolean)) rslt = Convert.ToInt32(reader.GetBoolean(fld));
                if (typ == typeof(System.DateTime)) rslt = reader.GetDateTime(fld).ToOADate();
                if (typ == typeof(System.Decimal)) rslt = (double)reader.GetDecimal(fld);
                if (typ == typeof(System.Object)) rslt = 0;
                if (typ == typeof(System.Byte)) rslt = (int)reader.GetByte(fld);
                if (typ == typeof(System.Guid)) rslt = 0;
                if (typ == typeof(System.String)) rslt = reader.GetString(fld).strToFloatDef(0);

            }
            return rslt;
        }

        public decimal fieldAsDecimal(string fieldName)
        {
            decimal rslt = 0;
            if (reader != null)
            {
                int fld = reader.GetOrdinal(fieldName);
                if (reader.IsDBNull(fld)) return rslt;
                Type typ = reader.GetFieldType(fld);
                if (typ == typeof(System.Int16)) rslt = (decimal)reader.GetInt16(fld);
                if (typ == typeof(System.Int32)) rslt = (decimal)reader.GetInt32(fld);
                if (typ == typeof(System.Int64)) rslt = (decimal)reader.GetInt64(fld);
                if (typ == typeof(System.Single)) rslt = (decimal)reader.GetFloat(fld);
                if (typ == typeof(System.Double)) rslt = (decimal)reader.GetDouble(fld);

                if (typ == typeof(System.Boolean)) rslt = Convert.ToInt32(reader.GetBoolean(fld));
                if (typ == typeof(System.DateTime)) rslt = (decimal)reader.GetDateTime(fld).ToOADate();
                if (typ == typeof(System.Decimal)) rslt = (decimal)reader.GetDecimal(fld);
                if (typ == typeof(System.Object)) rslt = 0;
                if (typ == typeof(System.Byte)) rslt = (int)reader.GetByte(fld);
                if (typ == typeof(System.Guid)) rslt = 0;
                if (typ == typeof(System.String)) rslt = (decimal)reader.GetString(fld).strToFloatDef(0);

            }
            return rslt;
        }

        public DateTime fieldAsDateTime(string fieldName)
        {
            DateTime rslt = new DateTime(0);
            DateTime nullDate = new DateTime(0);
            if (reader != null)
            {
                int fld = reader.GetOrdinal(fieldName);
                if (reader.IsDBNull(fld)) return rslt;
                Type typ = reader.GetFieldType(fld);
                if (typ == typeof(System.DateTime)) rslt = reader.GetDateTime(fld);
                if (typ == typeof(System.Int16)) rslt = DateTime.FromOADate(reader.GetInt16(fld));
                if (typ == typeof(System.Int32)) rslt = DateTime.FromOADate(reader.GetInt32(fld));
                if (typ == typeof(System.Int64)) rslt = DateTime.FromOADate(reader.GetInt64(fld));
                if (typ == typeof(System.Single)) rslt = DateTime.FromOADate(reader.GetFloat(fld));
                if (typ == typeof(System.Double)) rslt = DateTime.FromOADate(reader.GetDouble(fld));

                if (typ == typeof(System.Boolean)) rslt = nullDate;

                if (typ == typeof(System.Decimal)) rslt = DateTime.FromOADate((double)reader.GetDecimal(fld));
                if (typ == typeof(System.Object)) rslt = nullDate;
                if (typ == typeof(System.Byte)) rslt = DateTime.FromOADate(reader.GetByte(fld));
                if (typ == typeof(System.Guid)) rslt = nullDate;
                if (typ == typeof(System.String)) rslt = reader.GetString(fld).strToDateTimeDef(nullDate);

            }
            return rslt;
        }

        public Nullable<DateTime> fieldAsNullableDateTime(string fieldName)
        {
            Nullable<DateTime> rslt = new DateTime(0);
            if (reader != null)
            {
                int fld = reader.GetOrdinal(fieldName);
                if (reader.IsDBNull(fld)) return null;
                Type typ = reader.GetFieldType(fld);
                if (typ == typeof(System.DateTime)) rslt = reader.GetDateTime(fld);
                if (typ == typeof(System.Int16)) rslt = DateTime.FromOADate(reader.GetInt16(fld));
                if (typ == typeof(System.Int32)) rslt = DateTime.FromOADate(reader.GetInt32(fld));
                if (typ == typeof(System.Int64)) rslt = DateTime.FromOADate(reader.GetInt64(fld));
                if (typ == typeof(System.Single)) rslt = DateTime.FromOADate(reader.GetFloat(fld));
                if (typ == typeof(System.Double)) rslt = DateTime.FromOADate(reader.GetDouble(fld));

                if (typ == typeof(System.Boolean)) rslt = null;

                if (typ == typeof(System.Decimal)) rslt = DateTime.FromOADate((double)reader.GetDecimal(fld));
                if (typ == typeof(System.Object)) rslt = null;
                if (typ == typeof(System.Byte)) rslt = DateTime.FromOADate(reader.GetByte(fld));
                if (typ == typeof(System.Guid)) rslt = null;
                if (typ == typeof(System.String)) rslt = reader.GetString(fld).strToDateTimeDef(DateTime.FromOADate(0));

            }
            return rslt;
        }

        public Int64 fieldAsInt64(string fieldName)
        {
            Int64 rslt = 0;
            if (reader != null)
            {
                int fld = reader.GetOrdinal(fieldName);
                if (reader.IsDBNull(fld)) return rslt;
                Type typ = reader.GetFieldType(fld);
                if (typ == typeof(System.Int16)) rslt = (Int64)reader.GetInt16(fld);
                if (typ == typeof(System.Int32)) rslt = (Int64)reader.GetInt32(fld);
                if (typ == typeof(System.Int64)) rslt = (Int64)reader.GetInt64(fld);
                if (typ == typeof(System.Single)) rslt = (Int64)reader.GetFloat(fld);
                if (typ == typeof(System.Double)) rslt = (Int64)reader.GetDouble(fld);

                if (typ == typeof(System.Boolean)) rslt = Convert.ToInt64(reader.GetBoolean(fld));
                if (typ == typeof(System.DateTime)) rslt = (Int64)reader.GetDateTime(fld).ToOADate();
                if (typ == typeof(System.Decimal)) rslt = (Int64)reader.GetDecimal(fld);
                if (typ == typeof(System.Object)) rslt = 0;
                if (typ == typeof(System.Byte)) rslt = (Int64)reader.GetByte(fld);
                if (typ == typeof(System.Guid)) rslt = 0;
                if (typ == typeof(System.String)) rslt = reader.GetString(fld).strToIntDef(0);

            }
            return rslt;
        }
    }


}
