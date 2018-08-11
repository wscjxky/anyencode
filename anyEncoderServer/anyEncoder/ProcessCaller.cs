namespace anyEncoder
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Threading;
    using System.Windows.Forms;

    public class ProcessCaller : anyEncoder.AsyncOperation
    {
        public long _oldTotalProcessorTime;
        public string Arguments;
        public string FileName;
        public string fuid;
        public int ID;
        public ProcessPriorityClass PriorityClass;
        private Process process;
        public int SleepTime;
        public string tsn;
        public string WorkingDirectory;

        public event DataReceivedHandler StdErrReceived;

        public event DataReceivedHandler StdOutReceived;

        public ProcessCaller(ISynchronizeInvoke isi) : base(isi)
        {
            this.PriorityClass = ProcessPriorityClass.Normal;
            this.SleepTime = 500;
            this.ID = 0;
            this._oldTotalProcessorTime = 0L;
        }

        protected override void DoWork()
        {
            this.StartProcess();
            while (!this.process.HasExited)
            {
                Thread.Sleep(this.SleepTime);
                if (base.CancelRequested)
                {
                    this.process.Kill();
                    base.AcknowledgeCancel();
                }
            }
        }

        protected virtual void ReadStdErr()
        {
            string str;
            while ((str = this.process.StandardError.ReadLine()) != null)
            {
                base.FireAsync(this.StdErrReceived, new object[] { this, new anyEncoder.DataReceivedEventArgs(str, this.tsn, this.process.Id) });
            }
        }

        protected virtual void ReadStdOut()
        {
            string str;
            while ((str = this.process.StandardOutput.ReadLine()) != null)
            {
                base.FireAsync(this.StdOutReceived, new object[] { this, new anyEncoder.DataReceivedEventArgs(str, this.tsn, this.process.Id) });
            }
        }

        protected virtual void StartProcess()
        {
            this.process = new Process();
            this.process.StartInfo.UseShellExecute = false;
            this.process.StartInfo.RedirectStandardOutput = true;
            this.process.StartInfo.RedirectStandardError = true;
            this.process.StartInfo.CreateNoWindow = true;
            this.process.StartInfo.FileName = this.FileName;
            this.process.StartInfo.Arguments = this.Arguments;
            this.process.StartInfo.WorkingDirectory = this.WorkingDirectory;
            this.process.Start();
            this.process.PriorityClass = this.PriorityClass;
            this.ID = this.process.Id;
            new MethodInvoker(this.ReadStdOut).BeginInvoke(null, null);
            new MethodInvoker(this.ReadStdErr).BeginInvoke(null, null);
        }

        public bool HasExited
        {
            get
            {
                try
                {
                    this.process.Refresh();
                    return this.process.HasExited;
                }
                catch
                {
                    return true;
                }
            }
        }

        public long oldTotalProcessorTime
        {
            get
            {
                return this._oldTotalProcessorTime;
            }
            set
            {
                this._oldTotalProcessorTime = value;
            }
        }

        public long PeakVirtualMemorySize64
        {
            get
            {
                return this.process.PeakVirtualMemorySize64;
            }
        }

        public DateTime StartTime
        {
            get
            {
                try
                {
                    return this.process.StartTime;
                }
                catch
                {
                    return DateTime.Now;
                }
            }
        }

        public TimeSpan TotalProcessorTime
        {
            get
            {
                try
                {
                    this.process.Refresh();
                    return this.process.TotalProcessorTime;
                }
                catch
                {
                    return new TimeSpan();
                }
            }
        }

        public long WorkingSet64
        {
            get
            {
                try
                {
                    this.process.Refresh();
                    return this.process.WorkingSet64;
                }
                catch
                {
                    return 0L;
                }
            }
        }
    }
}

