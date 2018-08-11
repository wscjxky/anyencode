namespace anyEncoder
{
    using System;
    using System.ComponentModel;
    using System.Configuration.Install;
    using System.ServiceProcess;

    [RunInstaller(true)]
    public class myServiceInstall : Installer
    {
        private ServiceInstaller myServiceInstall1 = new ServiceInstaller();
        private ServiceProcessInstaller serviceProcessInstaller1 = new ServiceProcessInstaller();

        public myServiceInstall()
        {
            this.serviceProcessInstaller1.Account = ServiceAccount.LocalSystem;
            this.myServiceInstall1.ServiceName = "anyEncoder";
            this.myServiceInstall1.StartType = ServiceStartMode.Automatic;
            base.Installers.AddRange(new Installer[] { this.serviceProcessInstaller1, this.myServiceInstall1 });
        }
    }
}

