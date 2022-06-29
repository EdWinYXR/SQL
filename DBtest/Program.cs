using DBAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Net;

namespace DBtest
{
    class Program
    {
        static void Main(string[] args)
        {
            DBManager.CDBManager.Instance.DoQuery("MRM", "");
            //OracleAccess oracle = new OracleAccess(DBBaseClass.DBType.Oracle, "orcale");
            //oracle.Init("192.168.3.114", "1521", "ProPerDBSid", "MES", "qwasZX12");
            ////bool bo = oracle.Open();
            //string add = string.Format("INSERT INTO \"User\"(\"username\",\"password\",\"Level\") VALUES('{0}','{1}','{2}')",
            //    "test", "qwaszx", 1);
            //int adfesdfi = oracle.DoNonQuery(add);
        }
    }
}
