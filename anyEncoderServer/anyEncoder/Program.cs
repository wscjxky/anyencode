namespace anyEncoder
{
    using System;
    using System.ServiceProcess;
    using System.Threading;
    using System.Windows.Forms;

    internal static class Program
    {
        [STAThread]
        private static void Main(string[] Args)
        {
            string str = null;
            if (Args.Length > 0)
            {
                str = Args[0];
            }
            if (str == "/d")
            {
                Run();
            }
            else
            {
                //Run();
                ServiceBase.Run(new ServiceBase[] { new myService() });
            }
        }

        public static void Run()
        {
            bool flag;
            startlog("开始启动...");
            startlog("正在清理进程");
            func.killall();
            Mutex mutex = new Mutex(true, Application.ProductName, out flag);
            if (flag)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                startlog("开始初始化窗口");
                Application.Run(new main());
                mutex.ReleaseMutex();
            }
            else
            {
                startlog("有一个和本程序相同的应用程序已经在运行，请不要同时运行多个本程序。 这个程序即将退出。");
                Application.Exit();
                Environment.Exit(0);
            }
            func.killall();
        }

        public static void startlog(string log)
        {
            new IniFiles(Application.StartupPath + @"\stat.ini").WriteString("system", "startlog", log);
        }
    }
}

