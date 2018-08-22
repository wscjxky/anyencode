namespace anyEncoder
{
    using System;
    using System.ServiceProcess;
    using System.Threading;
    using System.Windows.Forms;

    internal class myService : ServiceBase
    {
        public myService()
        {
            base.ServiceName = "anyEncoder";
          
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        protected override void OnStart(string[] args)
        {
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

