using System;

namespace Provisioning
{
    public class ThdEventArgs : EventArgs
    {
        public int ThreadsCount { get; set; }

        public ThdEventArgs(int threadsCount)
        {
            ThreadsCount = threadsCount;
        }
    }
}
