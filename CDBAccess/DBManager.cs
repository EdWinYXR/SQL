using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBAccess;
using DBBaseClass;

/*
    个人微信：a7761075
    个人邮箱：yinxurong@darsh.cn
    创建时间：2022/6/29 15:38:17
    主要用途：初始化调用DB
    更改记录：
       时间：2022/06/30      内容：实现单例化，在程序开始时初始化DBManager的init
*/
namespace DBClient
{
    public  class CDBClient
    {
        public static CDBClient Instance = new Lazy<CDBClient>(() => new CDBClient()).Value;

        public  Dictionary<string,CDBAccess> _DBTypeAccess = new Dictionary<string,CDBAccess>();
        /// <summary>
        /// 初始换个人连接
        /// </summary>
        /// <param name="dBType">数据库类型</param>
        /// <param name="conName">连接名称</param>
        /// <param name="server">IP</param>
        /// <param name="port">端口号</param>
        /// <param name="dataBase">数据库名称/SID</param>
        /// <param name="username">用户名称</param>
        /// <param name="password">用户密码</param>
        /// <returns></returns>
        public bool Init(DBType dBType,string conName,string server,int port, string dataBase, string username,string password)
        {
            CDBAccess access;
            switch (dBType)
            {
                case DBType.MySQL:
                    access = new MySQLAccess(DBType.MySQL, conName);
                    break;
                case DBType.SQLServer:
                    access = new SQLServerAccess(DBType.SQLServer,conName);
                    break;
                case DBType.Oracle:
                    access = new OracleAccess(DBType.Oracle, conName);
                    break;
                default:
                    return false;
            }
            bool bo = access.Init(server, port.ToString(), dataBase, username, password);
            if (bo)
            {
                _DBTypeAccess.Add(conName, access);
                return true;
            }
            return false;
        }
        public  bool Open(string conName)
        {
            return _DBTypeAccess[conName].Open();
        }
        public bool Close(string conName)
        {
            return _DBTypeAccess[conName].Close();
        }
        public  bool TestConnectStatus(string conName)
        {
            return _DBTypeAccess[conName].TestConnectStatus();
        }
        public  System.Data.DataTable DoQuery(string conName, string m_sqlstr)
        {
            return _DBTypeAccess[conName].DoQuery(m_sqlstr);
        }
        public  int DoNonQuery(string conName,string m_sqlstr)
        {
            return  _DBTypeAccess[conName].DoNonQuery(m_sqlstr);
        }
        public  int DoNonQuery(string conName, List<string> m_sqlstrList)
        {
            return _DBTypeAccess[conName].DoNonQuery(m_sqlstrList);
        }
        public System.Data.DataSet DOQueryPro(string conName,string strProcName, object ob)
        {
           return _DBTypeAccess[conName].DOQueryPro(strProcName, ob);
        }
    }
}
