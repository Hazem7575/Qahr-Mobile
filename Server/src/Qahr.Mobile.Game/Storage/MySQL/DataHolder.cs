using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// [net10] MySql.Data 6.7 is .NET Framework only; the .NET 10 build (Qahr.Mobile.Net10.csproj) uses MySqlConnector.
using MYSQLCOMMAND = MySqlConnector.MySqlCommand;
using MYSQLREADER = MySqlConnector.MySqlDataReader;
using MYSQLCONNECTION = MySqlConnector.MySqlConnection;

namespace Qahr.Mobile.DBFunctionality
{
    public unsafe static class DataHolder
    {
        public static String ConnectionString;
        public static Boolean CreateConnection(String host, String username, String password, String database, int port = 3306)
        {
            ConnectionString = "Server=" + host + ";Port=" + port + ";Database=" + database + ";Uid=" + username + ";Password=" + password + ";Persist Security Info=True;Pooling=true; Min Pool Size = 2; Max Pool Size = 100;CharSet=utf8mb4;"
                // [net10] MySqlConnector defaults differ from MySql.Data 6.7: it would try TLS (SslMode=Preferred);
                // 6.7 connected without TLS, so keep that. Everything else above means the same in both drivers.
                + "SslMode=None;"
                // [perf] MySqlConnector بيعمل reset للاتصال (رحلة زيادة للقاعدة) كل ما ياخده من الـ pool. MySql.Data 6.7
                // (نسخة 4.8) مكانش بيعمل كده. اتقاس: 0.57ms -> 0.28ms لكل استعلام. مفيش session state في السورس
                // (الـ transactions كلها using => rollback لو ماحصلش Commit)، فده آمن.
                + "ConnectionReset=false;"
                ; return true;
        }
        public static MYSQLCONNECTION MySqlConnection
        {
            get
            {
                MYSQLCONNECTION conn = new MYSQLCONNECTION();
                conn.ConnectionString = ConnectionString;
                return conn;
            }
        }
    }
}
