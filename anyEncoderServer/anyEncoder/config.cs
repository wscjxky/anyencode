namespace anyEncoder
{
    using System;
    using System.Windows.Forms;

    internal class config
    {
        private static IniFiles configini = new IniFiles(Application.StartupPath + @"\config.ini");
        public static string errlog = (Application.StartupPath + @"\log\errlog.txt");

        public static int maxerr
        {
            get
            {
                return configini.ReadInteger("encoder", "maxerr", 1);
            }
        }

        public static int maxtask
        {
            get
            {
                return configini.ReadInteger("system", "maxtask", 1);
            }
        }

        public static string mssql
        {
            get
            {
                return ("Data Source=" + configini.ReadString("system", "sqlserver", "") + ";Initial Catalog=" + configini.ReadString("system", "database", "") + ";user id=" + configini.ReadString("system", "username", "") + ";password=" + configini.ReadString("system", "password", ""));
            }
        }

        public static string safecode
        {
            get
            {
                return configini.ReadString("system", "safecode", "");
            }
        }
    }
}

