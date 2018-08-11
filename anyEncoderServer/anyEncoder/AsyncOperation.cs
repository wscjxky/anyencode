namespace anyEncoder
{
    using System;
    using System.ComponentModel;
    using System.Threading;
    using System.Windows.Forms;

    public abstract class AsyncOperation
    {
        private bool cancelAcknowledgedFlag;
        private bool cancelledFlag;
        private bool completeFlag;
        private ThreadExceptionEventHandler Failed;
        private bool failedFlag;
        private ISynchronizeInvoke isiTarget;
        private bool isRunning;

        public event EventHandler Cancelled;

        public event EventHandler Completed;

        public AsyncOperation(ISynchronizeInvoke target)
        {
            this.isiTarget = target;
            this.isRunning = false;
        }

        protected void AcknowledgeCancel()
        {
            lock (this)
            {
                this.cancelAcknowledgedFlag = true;
                this.isRunning = false;
                Monitor.Pulse(this);
                this.FireAsync(this.Cancelled, new object[] { this, EventArgs.Empty });
            }
        }

        public virtual void Cancel()
        {
            lock (this)
            {
                this.cancelledFlag = true;
            }
        }

        public bool CancelAndWait()
        {
            lock (this)
            {
                this.cancelledFlag = true;
                while (!this.IsDone)
                {
                    Monitor.Wait(this, 0x3e8);
                }
            }
            return !this.HasCompleted;
        }

        private void CompleteOperation()
        {
            lock (this)
            {
                this.completeFlag = true;
                this.isRunning = false;
                Monitor.Pulse(this);
                this.FireAsync(this.Completed, new object[] { this, EventArgs.Empty });
            }
        }

        protected abstract void DoWork();
        private void FailOperation(Exception e)
        {
            lock (this)
            {
                this.failedFlag = true;
                this.isRunning = false;
                Monitor.Pulse(this);
                this.FireAsync(this.Failed, new object[] { this, new ThreadExceptionEventArgs(e) });
            }
        }

        protected void FireAsync(Delegate dlg, params object[] pList)
        {
            if (dlg != null)
            {
                this.Target.BeginInvoke(dlg, pList);
            }
        }

        private void InternalStart()
        {
            this.cancelledFlag = false;
            this.completeFlag = false;
            this.cancelAcknowledgedFlag = false;
            this.failedFlag = false;
            try
            {
                this.DoWork();
            }
            catch (Exception exception)
            {
                try
                {
                    this.FailOperation(exception);
                }
                catch
                {
                }
                if (exception is SystemException)
                {
                }
            }
            lock (this)
            {
                if (!(this.cancelAcknowledgedFlag || this.failedFlag))
                {
                    this.CompleteOperation();
                }
            }
        }

        public void Start()
        {
            lock (this)
            {
                if (this.isRunning)
                {
                    throw new AlreadyRunningException();
                }
                this.isRunning = true;
            }
            new MethodInvoker(this.InternalStart).BeginInvoke(null, null);
        }

        public bool WaitUntilDone()
        {
            lock (this)
            {
                while (!this.IsDone)
                {
                    Monitor.Wait(this, 0x3e8);
                }
            }
            return this.HasCompleted;
        }

        protected bool CancelRequested
        {
            get
            {
                lock (this)
                {
                    return this.cancelledFlag;
                }
            }
        }

        protected bool HasCompleted
        {
            get
            {
                lock (this)
                {
                    return this.completeFlag;
                }
            }
        }

        public bool IsDone
        {
            get
            {
                lock (this)
                {
                    return ((this.completeFlag || this.cancelAcknowledgedFlag) || this.failedFlag);
                }
            }
        }

        protected ISynchronizeInvoke Target
        {
            get
            {
                return this.isiTarget;
            }
        }
    }
}

