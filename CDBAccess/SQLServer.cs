using DBBaseClass;
using Net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace DBAccess
{
    internal sealed class SQLServerAccess : CDBAccess, IDisposable
    {
        private SqlConnection m_Connection;
        public SQLServerAccess(DBType type, string conName)
            : base(type, conName)
        {
           //CDBLog.Instance.Init();
        }

        public void Dispose()
        {

        }

        public override bool Init(string strServer, string strPort, string strDataBase, string strUser, string strPwd)
        {
            try
            {
                m_ConnectString = string.Format("Data Source={0}; Database={1}; User Id={2}; Password={3}",
                  strServer, strDataBase, strUser, strPwd);

                m_Connection = new SqlConnection(m_ConnectString);

                return true;
            }
            catch (Exception ex)
            {
                CDBLog.Instance.WriteLog(LogLevel.ERROR_LOG_LEVEL, "Init:" + ex.Message);
                return false;
            }
        }

        public override bool Open()
        {
            bool ret = true;
            lock (asynclockobj)
            {
                try
                {
                    m_Connection.Open();
                }
                catch (Exception)
                {
                    ret = false;
                }
            }
            return ret;
        }

        public override bool Close()
        {
            bool ret = true;
            lock (asynclockobj)
            {
                try
                {
                    m_Connection.Close();
                }
                catch (Exception)
                {
                    ret = false;
                }
            }
            return ret;
        }

        public override bool TestConnectStatus()
        {
            bool ret = true;
            lock (asynclockobj)
            {
                if (m_Connection.State == System.Data.ConnectionState.Closed || m_Connection.State == System.Data.ConnectionState.Broken)
                {
                    ret = false;
                }
            }
            return ret;
        }

        public override System.Data.DataTable DoQuery(string m_sqlstr)
        {
            DataTable m_dataTable = null;
            lock (asynclockobj)
            {
                SqlDataReader reader = null;
                try
                {
                    m_Connection.Open();
                    //根据sql和数据库连接执行查询
                    SqlCommand SqlCommand = new SqlCommand(m_sqlstr, m_Connection);
                    DataSet dataSet = new DataSet();
                    SqlDataAdapter adapter = new SqlDataAdapter(SqlCommand);
                    adapter.Fill(dataSet);
                    m_dataTable = dataSet.Tables[0];
                    //CDBLog.Instance.WriteLog(LogLevel.INFO_LOG_LEVEL, m_sqlstr);
                }
                catch (Exception err)
                {
                    //Debug.WriteLine(err.Message.ToString());
                    CDBLog.Instance.WriteLog(LogLevel.ERROR_LOG_LEVEL, m_sqlstr + ": " + err.Message);
                }
                finally
                {
                    if (reader != null)
                    {
                        reader.Close();
                    }
                    m_Connection.Close();
                }
            }
            return m_dataTable;
        }
        /// <summary>
        /// 返回受影响行数
        /// </summary>
        /// <param name="m_sqlstr"></param>
        /// <returns></returns>
        public override int DoNonQuery(string m_sqlstr)
        {
            int ret = 0;
            lock (asynclockobj)
            {
                SqlTransaction tx = null;
                try
                {
                    m_Connection.Open();
                    //开启事务
                    tx = m_Connection.BeginTransaction();
                    SqlCommand SqlCommand = new SqlCommand();
                    SqlCommand.Connection = m_Connection;
                    SqlCommand.Transaction = tx;
                    SqlCommand.CommandText = m_sqlstr;
                    ret= SqlCommand.ExecuteNonQuery();
                    //提交事务
                    tx.Commit();
                    CDBLog.Instance.WriteLog(LogLevel.INFO_LOG_LEVEL, m_sqlstr);
                }
                catch (System.Data.SqlClient.SqlException E)
                {
                    ret = -1;
                    if (tx != null)
                    {
                        tx.Rollback();
                    }
                    Debug.WriteLine(E.Message.ToString());
                    CDBLog.Instance.WriteLog(LogLevel.ERROR_LOG_LEVEL, m_sqlstr + ": " + E.Message);
                }
                finally
                {
                    m_Connection.Close();
                }
            }
            return ret;
        }

        public override int DoNonQuery(List<string> m_sqlstrList)
        {
            int ret = 0;
            lock (asynclockobj)
            {
                SqlTransaction tx = null;
                try
                {
                    m_Connection.Open();
                    //开启事务
                    tx = m_Connection.BeginTransaction();
                    foreach (string sqlStr in m_sqlstrList)
                    {
                        SqlCommand SqlCommand = new SqlCommand();
                        SqlCommand.Connection = m_Connection;
                        SqlCommand.Transaction = tx;
                        SqlCommand.CommandText = sqlStr;
                        SqlCommand.ExecuteNonQuery();
                    }
                    //提交事务
                    tx.Commit();
                }
                catch (System.Data.SqlClient.SqlException E)
                {
                    ret = -1;
                    if (tx != null)
                    {
                        tx.Rollback();
                    }
                    Debug.WriteLine(E.Message.ToString());
                    string logStr = "transaction start--\r\n";
                    foreach (string m_sqlstr in m_sqlstrList)
                    {
                        logStr += m_sqlstr + "\r\n";
                    }
                    logStr += "transaction end. \r\n";
                    CDBLog.Instance.WriteLog(LogLevel.ERROR_LOG_LEVEL, logStr + "\r\n" + E.Message);
                }
                finally
                {
                    m_Connection.Close();
                    string logStr = "transaction start--\r\n";
                    foreach (string m_sqlstr in m_sqlstrList)
                    {
                        logStr += m_sqlstr + "\r\n";
                    }
                    logStr += "transaction end. \r\n";
                    CDBLog.Instance.WriteLog(LogLevel.INFO_LOG_LEVEL, logStr);
                }
            }
            return ret;
        }
        /// <summary>
        /// 存错过程
        /// </summary>
        /// <param name="strProcName">过程名</param>
        /// <param name="ob">参数 SqlParameter[]类型</param>
        /// <returns></returns>
        public override DataSet DOQueryPro(string strProcName, object ob)
        {
            DataSet dt = new DataSet();
            if (!(ob is SqlParameter[]))
            {
                CDBLog.Instance.WriteLog(LogLevel.ERROR_LOG_LEVEL, "DOQueryPro : 参数不为 SqlParameter[]类型");
                return dt;
            }
            SqlParameter[] parameters = (SqlParameter[])ob;
            try
            {
                lock (asynclockobj)
                {
                    m_Connection.Open();

                    SqlCommand cmd = new SqlCommand(strProcName, m_Connection);
                    cmd.CommandText = strProcName;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = m_Connection;
                    if (parameters != null)
                    {
                        // 添加参数
                        cmd.Parameters.AddRange(parameters);
                    }
                    // 取数据
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                CDBLog.Instance.WriteLog(LogLevel.ERROR_LOG_LEVEL, "QueryPro :" + ex.Message);
            }
            finally
            {
                string str = "Pro parameter：\r\n";
                foreach (SqlParameter orP in parameters)
                {
                    str += string.Format("parameterName={0},Value={1}\r\n", orP.ParameterName, orP.Value);
                }
                CDBLog.Instance.WriteLog(LogLevel.INFO_LOG_LEVEL, str);
                m_Connection.Close();
            }
            return dt;
        }
    }
}
