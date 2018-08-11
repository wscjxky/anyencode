using System;
using System.Collections.Generic;
using System.Web;

namespace VodFile.common
{
    public class MyConn
    {
        private static string cnStr = System.Configuration.ConfigurationManager.ConnectionStrings["mssql"].ConnectionString;
        public static string ConnectionString
        {
            get
            {
                return MyConn.cnStr;
            }
            set
            {
                MyConn.cnStr = value;
            }

        }
    }
}