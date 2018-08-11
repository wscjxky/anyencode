namespace anyEncoder
{
    using System;

    public class DataReceivedEventArgs : EventArgs
    {
        public int Pid;
        public string Text;
        public string Tsn;

        public DataReceivedEventArgs(string text, string tsn, int pid)
        {
            this.Text = text;
            this.Tsn = tsn;
            this.Pid = pid;
        }
    }
}

