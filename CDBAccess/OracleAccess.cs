using DBBaseClass;
using Net;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace DBAccess
{
    public  class OracleAccess : CDBAccess, IDisposable
    {
        OracleConnection conn = null;
        public OracleAccess(DBType type, string conName)
        : base(type, conName)
        {
            //CDBLog.Instance.Init();
        }
        public void Dispose()
        {

        }
        /// <summary>
        /// 初始化连接
        /// </summary>
        /// <param name="strServer">ip</param>
        /// <param name="strPort">端口</param>
        /// <param name="strDataBase">用户服务名SID</param>
        /// <param name="strUser">用户名</param>
        /// <param name="strPwd">密码</param>
        /// <returns></returns>
        public override bool Init(string strServer, string strPort, string strDataBase, string strUser, string strPwd)
        {
            try
            {
                string ConString = string.Format("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1}))(CONNECT_DATA=(SID={2})));Persist Security Info=True;User ID={3};Password={4};",
                    strServer, strPort, strDataBase, strUser, strPwd);
                conn = new OracleConnection(ConString);
                return true;
            }
            catch (Exception ex)
            {
                CDBLog.Instance.WriteLog(LogLevel.ERROR_LOG_LEVEL, "Init: " + ex.Message);
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
                    conn.Open();
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
                    conn.Close();
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
                if (conn.State == System.Data.ConnectionState.Closed || conn.State == System.Data.ConnectionState.Broken)
                {
                    ret = false;
                }
            }
            return ret;
        }

        //查询
        public override System.Data.DataTable DoQuery(string m_sqlstr)
        {
            DataTable m_dataTable = new DataTable();
            lock (asynclockobj)
            {
                OracleDataReader reader = null;
                try
                {
                    conn.Open();
                    OracleCommand Command = new OracleCommand(m_sqlstr, conn);
                    reader = Command.ExecuteReader();
                    m_dataTable.Load(reader);
                }
                catch (Exception err)
                {
                    CDBLog.Instance.WriteLog(LogLevel.ERROR_LOG_LEVEL, m_sqlstr + ": " + err.Message);
                    Debug.WriteLine(err.Message.ToString());
                }
                finally
                {
                    if (reader != null)
                    {
                        reader.Close();
                    }
                    conn.Close();
                }
            }
            return m_dataTable;
        }

        //更改
        public override int DoNonQuery(string m_sqlstr)
        {
            int ret = 0;
            lock (asynclockobj)
            {
                OracleTransaction tx = null;
                try
                {
                    conn.Open();
                    //开启事务
                    tx = conn.BeginTransaction();
                    OracleCommand mySqlCommand = new OracleCommand();
                    mySqlCommand.Connection = conn;
                    mySqlCommand.Transaction = tx;
                    mySqlCommand.CommandText = m_sqlstr;
                    ret = mySqlCommand.ExecuteNonQuery();
                    //提交事务
                    tx.Commit();
                }
                catch (Exception E)
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
                    conn.Close();
                    CDBLog.Instance.WriteLog(LogLevel.INFO_LOG_LEVEL, m_sqlstr);
                }
            }
            return ret;
        }
        //多条更改
        public override int DoNonQuery(List<string> m_sqlstrList)
        {
            int ret = 0;
            lock (asynclockobj)
            {
                OracleTransaction tx = null;
                try
                {
                    conn.Open();
                    //开启事务
                    tx = conn.BeginTransaction();
                    foreach (string sqlStr in m_sqlstrList)
                    {
                        OracleCommand mySqlCommand = new OracleCommand();
                        mySqlCommand.Connection = conn;
                        mySqlCommand.Transaction = tx;
                        mySqlCommand.CommandText = sqlStr;
                        mySqlCommand.ExecuteNonQuery();
                    }
                    //提交事务
                    tx.Commit();
                }
                catch (Exception E)
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
                    conn.Close();
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
        ///调用存储过程
        /// </summary>
        /// <param name="strProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数 类型是OracleParameter[]</param>
        /// <returns></returns>
        public override DataSet DOQueryPro(string strProcName, object ob)
        {
            DataSet dt = new DataSet();
            if(!(ob is OracleParameter[]))
            {
                CDBLog.Instance.WriteLog(LogLevel.ERROR_LOG_LEVEL, "DOQueryPro : 参数不为OracleParameter[]类型");
                return dt;
            }
            OracleParameter[] parameters = (OracleParameter[])ob;
            try
            {
                lock (asynclockobj)
                {
                    conn.Open();

                    OracleCommand cmd = new OracleCommand(strProcName, conn);
                    cmd.CommandText = strProcName;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = conn;
                    if (parameters != null)
                    {
                        // 添加参数
                        cmd.Parameters.AddRange(parameters);
                    }
                    // 取数据
                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
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
                foreach (OracleParameter orP in parameters)
                {
                    str += string.Format("parameterName={0},Value={1}\r\n", orP.ParameterName, orP.Value);
                }
                CDBLog.Instance.WriteLog(LogLevel.INFO_LOG_LEVEL, str);
                conn.Close();
            }
            return dt;
        }
    }
}

