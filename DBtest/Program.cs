using DBAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Net;
using DBClient;

namespace DBtest
{
    class Program
    {
        static void Main(string[] args)
        {
            CDBClient.Instance.Init(DBBaseClass.DBType.Oracle, "orcale", "192.168.3.114", 1521, "ProPerDBSid", "MES", "qwasZX12");
            bool bo = CDBClient.Instance.Open("orcale");
        }
    }
}
