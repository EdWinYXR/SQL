using DBBaseClass;
using MySql.Data.MySqlClient;
using Net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace DBAccess
{
    public  class MySQLAccess: CDBAccess, IDisposable
    {
        MySqlConnection conn;
        public MySQLAccess(DBType type, string conName)
        : base(type, conName)
        {
           // CDBLog.Instance.Init();
        }
        public void Dispose()
        {

        }
        //初始化连接数据库
        public override bool Init(string strServer, string strPort, string strDataBase, string strUser, string strPwd)
        {
            try
            {
                m_ConnectString = string.Format("Data Source={0}; Port={1}; Database={2}; User Id={3}; Password={4};SslMode=None;CharSet=utf8",
              strServer, strPort, strDataBase, strUser, strPwd);

                conn = new MySqlConnection(m_ConnectString);
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
        //readonly String ConString = "server=127.0.0.1;port=3306;user=root;password=12qwasZX;Database=mrms_geely";
        // server=127.0.0.1/localhost 代表本机，端口号port默认是3306可以不写

        //查询
        public override System.Data.DataTable DoQuery(string m_sqlstr)
        {
            DataTable m_dataTable = new DataTable();
            lock (asynclockobj)
            {
                MySqlDataReader reader = null;
                try
                {
                    conn.Open();
                    MySqlCommand mySqlCommand = new MySqlCommand(m_sqlstr, conn);
                    reader = mySqlCommand.ExecuteReader();
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
                MySqlTransaction tx = null;
                try
                {
                    conn.Open();
                    //开启事务
                    tx = conn.BeginTransaction();
                    MySqlCommand mySqlCommand = new MySqlCommand();
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
                MySqlTransaction tx = null;
                try
                {
                    conn.Open();
                    //开启事务
                    tx = conn.BeginTransaction();
                    foreach (string sqlStr in m_sqlstrList)
                    {
                        MySqlCommand mySqlCommand = new MySqlCommand();
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
        /// <param name="parameters">存储过程参数 类型是MySqlParameter[]</param>
        /// <returns></returns>
        public override DataSet DOQueryPro(string strProcName, object ob)
        {
            DataSet dt = new DataSet();
            if (!(ob is MySqlParameter[]))
            {
                CDBLog.Instance.WriteLog(LogLevel.ERROR_LOG_LEVEL, "DOQueryPro : 参数不为MySqlParameter[]类型");
                return dt;
            }
            MySqlParameter[] parameters = (MySqlParameter[])ob;
            try
            {
                lock (asynclockobj)
                {
                    conn.Open();

                    MySqlCommand cmd = new MySqlCommand(strProcName, conn);
                    cmd.CommandText = strProcName;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = conn;
                    if (parameters != null)
                    {
                        // 添加参数
                        cmd.Parameters.AddRange(parameters);
                    }
                    // 取数据
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
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
                foreach (MySqlParameter orP in parameters)
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

