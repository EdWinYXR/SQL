using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBBaseClass
{
    public enum DBType
    {
        SQLServer,
        Oracle,
        MySQL,
    }
    public abstract class CDBAccess
    {
        protected object asynclockobj = new object();
        protected string m_ConnectString;
        protected string m_Server;
        protected string m_Port;
        protected string m_DataBase;
        protected string m_User;
        protected string m_Password;
        protected string m_ConnectName;
        protected DBType m_Type;
        public DBType DBType { get { return m_Type; } }
        public string ConnectName { get { return m_ConnectName; } }
        protected CDBAccess(DBType type, string conName) { m_Type = type; m_ConnectName = conName; }
        public abstract bool Init(string strServer, string strPort, string strDataBase, string strUser, string strPwd);
        public abstract bool Open();
        public abstract bool Close();
        public abstract bool TestConnectStatus();
        public abstract DataTable DoQuery(string m_sqlstr);
        public abstract int DoNonQuery(string m_sqlstr);
        public abstract int DoNonQuery(List<string> m_sqlstrList);
        public abstract DataSet DOQueryPro(string strProcName, object ob);
    }
}
