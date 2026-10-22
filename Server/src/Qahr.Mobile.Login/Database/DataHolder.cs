// * Created by ElmistRo
// * Copyright © 2010-2014
// * ElmistRo - Project

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Qahr.Mobile.Login.Database
{
    // [net10] MySqlConnector (see Net10Compat.cs).
    using MYSQLCOMMAND = MySqlConnector.MySqlCommand;
    using MYSQLREADER = MySqlConnector.MySqlDataReader;
    using MYSQLCONNECTION = MySqlConnector.MySqlConnection;

    public unsafe static class DataHolder
    {
        private static string ConnectionString = "";
        public static void CreateConnection(string database, string password, string user = "root", string host = "localhost", int port = 3306)
        {
            ConnectionString = "Server=" + host + ";Port=" + port + ";Database=" + database + ";Uid=" + user + ";Password=" + password + ";Persist Security Info=True;Pooling=true; Min Pool Size = 2; Max Pool Size = 30;CharSet=utf8mb4;"
                // [net10] زي GameServer: من غير TLS (زي MySql.Data 6.7)، ومن غير reset للاتصال مع كل استخدام من الـ pool
                // (رحلة زيادة للقاعدة). اتقاس: 0.57ms -> 0.28ms لكل استعلام، والدخول فيه 3 استعلامات.
                + "SslMode=None;ConnectionReset=false;"
                ;
        }
        public static MYSQLCONNECTION   MySqlConnection
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