namespace VodFile
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Xml;

    public sealed class Conn
    {
        public static SqlConnection connectionReader = null;
        public static readonly string connectionString = ConfigurationManager.ConnectionStrings["mssql"].ConnectionString;
        private static Hashtable parmCache = Hashtable.Synchronized(new Hashtable());

        private static SqlCommand BuildIntCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            using (SqlCommand command = BuildQueryCommand(connection, storedProcName, parameters))
            {
                command.Parameters.Add(new SqlParameter("ReturnValue", SqlDbType.Int, 4, ParameterDirection.ReturnValue, false, 0, 0, string.Empty, DataRowVersion.Default, null));
                return command;
            }
        }

        private static SqlCommand BuildQueryCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            using (SqlCommand command = new SqlCommand(storedProcName, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                if (parameters != null)
                {
                    foreach (SqlParameter parameter in parameters)
                    {
                        if ((parameter.SqlDbType == SqlDbType.DateTime) && (((DateTime) parameter.Value) == DateTime.MinValue))
                        {
                            parameter.Value = DateTime.Now;
                        }
                        if (parameter.Value == null)
                        {
                            parameter.Value = DBNull.Value;
                        }
                        command.Parameters.Add(parameter);
                    }
                }
                return command;
            }
        }

        public static void CloseConnectionReader()
        {
            try
            {
                connectionReader.Close();
                connectionReader.Dispose();
                connectionReader = null;
            }
            catch
            {
            }
        }

        public static object DeserializeModel(byte[] b, object SampleModel)
        {
            if ((b == null) || (b.Length == 0))
            {
                return SampleModel;
            }
            object obj2 = new object();
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream serializationStream = new MemoryStream();
            try
            {
                serializationStream.Write(b, 0, b.Length);
                serializationStream.Position = 0;
                obj2 = formatter.Deserialize(serializationStream);
                serializationStream.Close();
            }
            catch
            {
            }
            return obj2;
        }

        public static int ExecuteNonQuery(string SQLString)
        {
            int num2;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        num2 = command.ExecuteNonQuery();
                    }
                    catch (SqlException exception)
                    {
                        connection.Close();
                        throw new Exception(SQLString + exception.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            return num2;
        }

        public static int ExecuteNonQuery(string storedProcName, IDataParameter[] parameters)
        {
            int num2;
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                num2 = BuildIntCommand(connection, storedProcName, parameters).ExecuteNonQuery();
            }
            catch (SqlException exception)
            {
                connection.Close();
                throw new Exception(exception.Message);
            }
            finally
            {
                if (connection != null)
                {
                    connection.Dispose();
                }
            }
            return num2;
        }

        public static int ExecuteNonQuery(string SQLString, params SqlParameter[] cmdParms)
        {
            int num2;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand();
                try
                {
                    PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                    int num = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    num2 = num;
                }
                catch (SqlException exception)
                {
                    throw new Exception(exception.Message);
                }
                finally
                {
                    if (cmd != null)
                    {
                        cmd.Dispose();
                    }
                }
            }
            return num2;
        }

        public static SqlDataReader ExecuteReader(string SQLString)
        {
            try
            {
                connectionReader = new SqlConnection(connectionString);
                connectionReader.Open();
                SqlCommand cmd = new SqlCommand(SQLString, connectionReader);
                PrepareCommand(cmd, connectionReader, null, SQLString, null);
                SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return reader;
            }
            catch
            {
                if (connectionReader != null)
                {
                    connectionReader.Close();
                }
                return null;
            }
        }

        public static SqlDataReader ExecuteReader(string SQLString, params SqlParameter[] cmdParms)
        {
            SqlDataReader reader2;
            try
            {
                connectionReader = new SqlConnection(connectionString);
                connectionReader.Open();
                SqlCommand cmd = new SqlCommand(SQLString, connectionReader);
                PrepareCommand(cmd, connectionReader, null, SQLString, cmdParms);
                SqlDataReader reader = cmd.ExecuteReader();
                cmd.Parameters.Clear();
                reader2 = reader;
            }
           catch (SqlException exception)
            {
                connectionReader.Close();
                throw new Exception(exception.Message);
            }
            return reader2;
        }

        public static object ExecuteScalar(string SQLString)
        {
            object obj3;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(SQLString, connection);
                try
                {
                    connection.Open();
                    object objA = command.ExecuteScalar();
                    if (object.Equals(objA, null) || object.Equals(objA, DBNull.Value))
                    {
                        return null;
                    }
                    obj3 = objA;
                }
                catch (SqlException exception)
                {
                    connection.Close();
                    throw new Exception(exception.Message);
                }
                finally
                {
                    if (command != null)
                    {
                        command.Dispose();
                    }
                }
            }
            return obj3;
        }

        public static object ExecuteScalar(string SQLString, params SqlParameter[] cmdParms)
        {
            object obj3;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand();
                try
                {
                    PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                    object objA = cmd.ExecuteScalar();
                    cmd.Parameters.Clear();
                    if (object.Equals(objA, null) || object.Equals(objA, DBNull.Value))
                    {
                        return null;
                    }
                    obj3 = objA;
                }
                catch (SqlException exception)
                {
                    throw new Exception(exception.Message);
                }
                finally
                {
                    if (cmd != null)
                    {
                        cmd.Dispose();
                    }
                }
            }
            return obj3;
        }

        public static int ExecuteSql(string SQLString, object content)
        {
            int num2;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(SQLString, connection);
                SqlParameter parameter = new SqlParameter("@content", SqlDbType.NText) {
                    Value = content
                };
                command.Parameters.Add(parameter);
                try
                {
                    connection.Open();
                    num2 = command.ExecuteNonQuery();
                }
                catch (SqlException exception)
                {
                    throw new Exception(exception.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
            }
            return num2;
        }

        public static int ExecuteSql(string SQLString, object content, SqlDbType dbtype)
        {
            int num2;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(SQLString, connection);
                SqlParameter parameter = new SqlParameter("@content", dbtype) {
                    Value = content
                };
                command.Parameters.Add(parameter);
                try
                {
                    connection.Open();
                    num2 = command.ExecuteNonQuery();
                }
                catch (SqlException exception)
                {
                    throw new Exception(exception.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
            }
            return num2;
        }

        public static int ExecuteSqlInsertImg(string strSQL, byte[] fs)
        {
            int num2;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(strSQL, connection))
                {
                    SqlParameter parameter = new SqlParameter("@fs", SqlDbType.Image) {
                        Value = fs
                    };
                    command.Parameters.Add(parameter);
                    try
                    {
                        connection.Open();
                        num2 = command.ExecuteNonQuery();
                    }
                    catch (SqlException exception)
                    {
                        throw new Exception(exception.Message);
                    }
                }
            }
            return num2;
        }

        public static void ExecuteSqlTran(List<string> SQLStringList)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand {
                    Connection = connection
                };
                SqlTransaction transaction = connection.BeginTransaction();
                command.Transaction = transaction;
                try
                {
                    for (int i = 0; i < SQLStringList.Count; i++)
                    {
                        string str = SQLStringList[i].ToString();
                        if (str.Trim().Length > 1)
                        {
                            command.CommandText = str;
                            command.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
                catch (SqlException exception)
                {
                    transaction.Rollback();
                    throw new Exception(exception.Message);
                }
            }
        }

        public static void ExecuteSqlTran(Hashtable SQLStringList)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    SqlCommand cmd = new SqlCommand();
                    try
                    {
                        foreach (DictionaryEntry entry in SQLStringList)
                        {
                            string cmdText = entry.Key.ToString();
                            SqlParameter[] cmdParms = (SqlParameter[]) entry.Value;
                            PrepareCommand(cmd, connection, transaction, cmdText, cmdParms);
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public static bool Exists(string strSql)
        {
            if (GetDataSet(strSql).Tables[0].Rows.Count == 0)
            {
                return false;
            }
            return true;
        }

        public static bool Exists(string strSql, params SqlParameter[] cmdParms)
        {
            if (GetDataSet(strSql, cmdParms).Tables[0].Rows.Count == 0)
            {
                return false;
            }
            return true;
        }

        public static SqlParameter[] GetCachedParameters(string cacheKey)
        {
            SqlParameter[] parameterArray = (SqlParameter[]) parmCache[cacheKey];
            if (parameterArray == null)
            {
                return null;
            }
            SqlParameter[] parameterArray2 = new SqlParameter[parameterArray.Length];
            int index = 0;
            int length = parameterArray.Length;
            while (index < length)
            {
                parameterArray2[index] = (SqlParameter) ((ICloneable) parameterArray[index]).Clone();
                index++;
            }
            return parameterArray2;
        }

        public static DataSet GetDataSet(string SQLString)
        {
            if (!string.IsNullOrEmpty(SQLString))
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    DataSet dataSet = new DataSet();
                    try
                    {
                        connection.Open();
                        new SqlDataAdapter(SQLString, connection).Fill(dataSet, "ds");
                    }
                    catch (SqlException exception)
                    {
                        throw new Exception(SQLString + exception.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
                    return dataSet;
                }
            }
            return null;
        }

        public static DataSet GetDataSet(string SQLString, params SqlParameter[] cmdParms)
        {
            DataSet set2;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand();
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    DataSet dataSet = new DataSet();
                    try
                    {
                        adapter.Fill(dataSet, "ds");
                        cmd.Parameters.Clear();
                    }
                    catch (SqlException exception)
                    {
                        throw new Exception(exception.Message);
                    }
                    set2 = dataSet;
                }
            }
            return set2;
        }

        public static DataSet GetDataSet(string sql, int min, int max)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DataSet dataSet = new DataSet();
                try
                {
                    connection.Open();
                    new SqlDataAdapter(sql, connection).Fill(dataSet, min, max, "ds");
                }
                catch (SqlException exception)
                {
                    connection.Close();
                    throw new Exception(exception.Message);
                }
                return dataSet;
            }
        }

        public static DataSet GetDataSet(string SQLString, int min, int max, params SqlParameter[] cmdParms)
        {
            DataSet set2;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand();
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    DataSet dataSet = new DataSet();
                    try
                    {
                        adapter.Fill(dataSet, min, max, "ds");
                        cmd.Parameters.Clear();
                    }
                    catch (SqlException exception)
                    {
                        throw new Exception(exception.Message);
                    }
                    set2 = dataSet;
                }
            }
            return set2;
        }

        public static int GetMaxID(string FieldName, string TableName)
        {
            using (SqlDataReader reader = ExecuteReader("SELECT MAX(" + FieldName + ") FROM " + TableName))
            {
                if (reader.Read())
                {
                    return ((reader[0] == DBNull.Value) ? 0 : Convert.ToInt32(reader[0]));
                }
                return 0;
            }
        }

        public static SqlParameter MakeInParam(string ParamName, SqlDbType DbType, object Value)
        {
            return MakeParam(ParamName, DbType, 0, ParameterDirection.Input, Value);
        }

        public static SqlParameter MakeInParam(string ParamName, SqlDbType DbType, int Size, object Value)
        {
            return MakeParam(ParamName, DbType, Size, ParameterDirection.Input, Value);
        }

        public static SqlParameter MakeOutParam(string ParamName, SqlDbType DbType, int Size)
        {
            return MakeParam(ParamName, DbType, Size, ParameterDirection.Output, null);
        }

        public static SqlParameter MakeParam(string ParamName, SqlDbType DbType, int Size, ParameterDirection Direction, object Value)
        {
            SqlParameter parameter;
            if (Size > 0)
            {
                parameter = new SqlParameter(ParamName, DbType, Size);
            }
            else
            {
                parameter = new SqlParameter(ParamName, DbType);
            }
            parameter.Direction = Direction;
            if ((Direction != ParameterDirection.Output) || (Value != null))
            {
                parameter.Value = Value;
            }
            return parameter;
        }

        public static string ModelToXML(object model)
        {
            XmlDocument document = new XmlDocument();
            XmlElement newChild = document.CreateElement("Model");
            document.AppendChild(newChild);
            if (model != null)
            {
                foreach (PropertyInfo info in model.GetType().GetProperties())
                {
                    XmlElement element2 = document.CreateElement(info.Name);
                    if (info.GetValue(model, null) != null)
                    {
                        element2.InnerText = info.GetValue(model, null).ToString();
                    }
                    else
                    {
                        element2.InnerText = "[Null]";
                    }
                    newChild.AppendChild(element2);
                }
            }
            return document.OuterXml;
        }

        public static DataSet PagerDataset(out int rowsTotal, string tableName, string fieldName, string keyName, int pageSize, int pageNow, string stringOrder, string stringWhere)
        {
            SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@rowsTotal", SqlDbType.Int), new SqlParameter("@tableName", SqlDbType.NVarChar, 800), new SqlParameter("@fieldName", SqlDbType.NVarChar, 800), new SqlParameter("@keyName", SqlDbType.NVarChar, 200), new SqlParameter("@pageSize", SqlDbType.Int), new SqlParameter("@pageNow", SqlDbType.Int), new SqlParameter("@stringOrder", SqlDbType.NVarChar, 200), new SqlParameter("stringWhere", SqlDbType.NVarChar, 800) };
            parameters[0].Direction = ParameterDirection.Output;
            parameters[1].Value = tableName;
            parameters[2].Value = (fieldName == null) ? "*" : fieldName;
            parameters[3].Value = keyName;
            parameters[4].Value = (pageSize == 0) ? int.Parse(ConfigurationManager.AppSettings["PageSize"]) : pageSize;
            parameters[5].Value = pageNow;
            parameters[6].Value = stringOrder;
            parameters[7].Value = stringWhere;
            DataSet set = RunProcedure("web_pager", parameters, "pager");
            rowsTotal = (int) parameters[0].Value;
            return set;
        }

        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, string cmdText, SqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
            {
                cmd.Transaction = trans;
            }
            cmd.CommandType = CommandType.Text;
            if (cmdParms != null)
            {
                foreach (SqlParameter parameter in cmdParms)
                {
                    if (parameter.Value == null)
                    {
                        parameter.Value = DBNull.Value;
                    }
                    else if (((parameter.SqlDbType == SqlDbType.DateTime) || (parameter.SqlDbType == SqlDbType.SmallDateTime)) && (((DateTime) parameter.Value) == DateTime.MinValue))
                    {
                        parameter.Value = DateTime.Now;
                    }
                    cmd.Parameters.Add(parameter);
                }
            }
        }

        public static SqlDataReader RunProcedure(string storedProcName, IDataParameter[] parameters)
        {
            SqlDataReader reader2;
            try
            {
                connectionReader = new SqlConnection(connectionString);
                connectionReader.Open();
                SqlCommand command = BuildQueryCommand(connectionReader, storedProcName, parameters);
                command.CommandType = CommandType.StoredProcedure;
                reader2 = command.ExecuteReader();
            }
            catch
            {
                if (connectionReader != null)
                {
                    connectionReader.Close();
                }
                throw;
            }
            return reader2;
        }

        public static DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DataSet dataSet = new DataSet();
                connection.Open();
                new SqlDataAdapter { SelectCommand = BuildQueryCommand(connection, storedProcName, parameters) }.Fill(dataSet, tableName);
                connection.Close();
                return dataSet;
            }
        }

        public static int RunProcedure(string storedProcName, IDataParameter[] parameters, out int rowsAffected)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = BuildIntCommand(connection, storedProcName, parameters);
                rowsAffected = command.ExecuteNonQuery();
                int num = (int) command.Parameters["ReturnValue"].Value;
                connection.Close();
                return num;
            }
        }

        public static byte[] SerializeModel(object obj)
        {
            if (obj != null)
            {
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream serializationStream = new MemoryStream();
                formatter.Serialize(serializationStream, obj);
                serializationStream.Position = 0;
                byte[] buffer = new byte[serializationStream.Length];
                serializationStream.Read(buffer, 0, buffer.Length);
                serializationStream.Close();
                return buffer;
            }
            return new byte[0];
        }

        public static int UpdateRecord(string tbName, string fldName)
        {
            return UpdateRecord(tbName, fldName, "");
        }

        public static int UpdateRecord(string tbName, string fldName, string strWhere)
        {
            int num2;
            SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@tbName", SqlDbType.NVarChar), new SqlParameter("@fldName", SqlDbType.NVarChar), new SqlParameter("@strWhere", SqlDbType.NVarChar) };
            parameters[0].Value = tbName;
            parameters[1].Value = fldName;
            parameters[2].Value = strWhere;
            num2 = RunProcedure("UpdateRecord", parameters, out num2);
            return 0;
        }

        public static object XMLToModel(string xml, object SampleModel)
        {
            if (!string.IsNullOrEmpty(xml))
            {
                XmlDocument document = new XmlDocument();
                document.LoadXml(xml);
                foreach (XmlNode node in document.SelectSingleNode("Model").ChildNodes)
                {
                    foreach (PropertyInfo info in SampleModel.GetType().GetProperties())
                    {
                        if (node.Name == info.Name)
                        {
                            if (node.InnerText != "[Null]")
                            {
                                if (info.PropertyType == typeof(Guid))
                                {
                                    info.SetValue(SampleModel, new Guid(node.InnerText), null);
                                }
                                else
                                {
                                    info.SetValue(SampleModel, Convert.ChangeType(node.InnerText, info.PropertyType), null);
                                }
                            }
                            else
                            {
                                info.SetValue(SampleModel, null, null);
                            }
                        }
                    }
                }
            }
            return SampleModel;
        }
    }
}

