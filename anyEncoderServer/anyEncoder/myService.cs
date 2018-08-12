namespace anyEncoder
{
    using System;
    using System.ServiceProcess;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;

    internal class myService : ServiceBase
    {
        public myService()
        {
            base.ServiceName = "anyEncoder_new";
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        protected override void OnStart(string[] args)
        {
            System.IO.File.WriteAllText(@".\test1.txt", "hahah" + DateTime.Now, Encoding.UTF8);
            Thread thread = new Thread(new ThreadStart(this.StartListen));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        protected override void OnStop()
        {
            func.killall();
            Application.Exit();
        }
        
        public void StartListen()
        {
            Program.Run();
        }
    }
}

