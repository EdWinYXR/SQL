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
                    时间：              内容：
*/
namespace DBManager
{
    internal  class DBInit
    {
        public string _server { get; set; }
        public string _port { get; set; }
        public string _dataBase { get; set; }
        public string _user { get; set; }
        public string _pwd { get; set; }
        public DBType _dBType { get; set; }
        public string _conName { get; set; }
    }
    public class CDBManager
    {
        public static CDBManager Instance = new Lazy<CDBManager>(() => new CDBManager()).Value;

        private  Dictionary<string,CDBAccess> _DBTypeAccess = new Dictionary<string,CDBAccess>();

        private DBInit dBInit = new DBInit();
        public bool Init()
        {
            CDBAccess access;
            switch (dBInit._dBType)
            {
                case DBType.MySQL:
                    access = new MySQLAccess(DBType.MySQL, dBInit. _conName);
                    break;
                case DBType.SQLServer:
                    access = new SQLServerAccess(DBType.SQLServer, dBInit._conName);
                    break;
                case DBType.Oracle:
                    access = new OracleAccess(DBType.Oracle, dBInit._conName);
                    break;
                default:
                    return false;
            }
            bool bo = access.Init(dBInit._server, dBInit._port, dBInit._dataBase, dBInit._user, dBInit._pwd);
            if (bo)
            {
                _DBTypeAccess.Add(dBInit._conName, access);
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
