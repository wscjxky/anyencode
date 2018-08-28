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
                return ("Data Source=123.57.157.64,58522\\SQLEXPRESS;Initial Catalog=voddb;user id=sa;password=HuiTeng168;MultipleActiveResultSets=True;");
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

