namespace anyEncoder
{
    using System;

    public class AlreadyRunningException : ApplicationException
    {
        public AlreadyRunningException() : base("Asynchronous operation already running")
        {
        }
    }
}

